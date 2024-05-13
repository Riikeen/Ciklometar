using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CiklometarDAL.Models
{
    public class StravaTokens
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        
    }
}
