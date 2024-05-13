using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
   public class AuthenticateResult
   {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
   }
}
