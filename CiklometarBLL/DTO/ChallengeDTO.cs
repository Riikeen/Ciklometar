using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CiklometarBLL.DTO
{
    public class ChallengeDTO
    {
        [JsonPropertyName("hub.challenge")]
        public string Challenge { get; set; }
    }
}
