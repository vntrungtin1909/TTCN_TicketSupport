using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketSupport.ViewModels
{
    public class CaptchaRespone
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTimestamp { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }
    }
}