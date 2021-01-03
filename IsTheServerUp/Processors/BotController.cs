using Discord;
using Discord.WebSocket;
using EasyTcp3;
using EasyTcp3.ClientUtils;
using IsTheServerUp.Models;
using IsTheServerUp.Repos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IsTheServerUp.Processors
{
    public class BotController
    {
        private DiscordSocketClient _client;
        private DiscordSecrets _discordSecrets;
        private Timer _timer;
        private LastStatusRepo _lastStatusRepo;

        public BotController(HttpClient httpClient, DiscordSecrets discordSecrets)
        {
            _discordSecrets = discordSecrets;
            _lastStatusRepo = new LastStatusRepo();
        }

        public async Task Listen()
        {
            _client = new DiscordSocketClient();

            await SetupTimer();

            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, _discordSecrets.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task SetupTimer()
        {
            _timer = new Timer();
            _timer.Interval = (_discordSecrets.PollingTimeInSeconds * 1000);
            _timer.Elapsed += PollServer;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            return Task.CompletedTask;
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async void PollServer(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Polling Server");

            try
            {
                using var client = new EasyTcpClient();
                var pingRecived = client.Connect(_discordSecrets.IpAddress, 25565);
                client.Dispose();

                if (pingRecived != _lastStatusRepo.WasPreviouslyOnline())
                {
                    _lastStatusRepo.SaveIsOnlineStatus(pingRecived);

                    if(pingRecived)
                        await SendServerOnlineMessage(true);
                    else
                        await SendServerOnlineMessage(false);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unable to complete command:");
                Console.WriteLine(exception.Message);
            }
        }

        private async Task SendServerOnlineMessage(bool isOnline)
        {
            Color messageColour;
            string titleMessage;
            string statusText;

            if (isOnline)
            {
                messageColour = new Color(0255127);
                titleMessage = "Server is online";
                statusText = "come online";
            }
            else
            {
                messageColour = new Color(16711680);
                titleMessage = "Server is offline";
                statusText = "gone offline";
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"{titleMessage}")
                .WithDescription($"Server with IP: **{_discordSecrets.IpAddress}:{_discordSecrets.Port}** has now {statusText}")
                .WithColor(messageColour)
                .WithFooter("Bot written by David Bewley", _discordSecrets.AuthorIcon);


            var channel = _client.GetGuild(_discordSecrets.ServerId).GetTextChannel(_discordSecrets.ChannelId);
            await channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }
}
