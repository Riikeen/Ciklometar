using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CiklometarBLL.DTO
{
    public class UserRegisterDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public string StravaId { get; set; }
    }
}
