using IsTheServerUp.Models;
using IsTheServerUp.Processors;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace IsTheServerUp
{
    class Program
    {
        static void Main(string[] args) 
            => new Program().MainAsync().GetAwaiter().GetResult();

        private static DiscordSecrets _discordSecrets;
        private static HttpClient httpClient;

        private Task ConfigSetup()
        {
            StreamWriter writer = new StreamWriter("PreviousStatus.json");
            writer.Write(JsonConvert.SerializeObject(false));
            writer.Flush();
            writer.Close();
            writer.Dispose();

            var config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            _discordSecrets = config.GetSection("DiscordSecrets").Get<DiscordSecrets>();
            return Task.CompletedTask;
        }

        public async Task MainAsync()
        {
            await ConfigSetup();

            httpClient = new HttpClient();
            Console.WriteLine(_discordSecrets.Token);

            var controller = new BotController(httpClient, _discordSecrets);

            await controller.Listen();
        }
    }
}
