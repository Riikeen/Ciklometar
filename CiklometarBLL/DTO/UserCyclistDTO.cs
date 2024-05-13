using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CiklometarBLL.DTO
{
    public class UserCyclistDTO
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }

        [JsonIgnore]
        public string StravaId { get; set; }
    }
}
