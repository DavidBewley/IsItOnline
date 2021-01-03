using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IsTheServerUp.Models
{
    [JsonObject("DiscordSecrets")]
    public class DiscordSecrets
    {
        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("ServerId")]
        public ulong ServerId { get; set; }

        [JsonProperty("ChannelId")]
        public ulong ChannelId { get; set; }

        [JsonProperty("PollingTimeInSeconds")]
        public int PollingTimeInSeconds { get; set; }

        [JsonProperty("IpAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("Port")]
        public ushort Port { get; set; }

        [JsonProperty("AuthorIcon")]
        public string AuthorIcon { get; set; }
    }
}
