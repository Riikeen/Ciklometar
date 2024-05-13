using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
   public class UserUpdateDTO
   {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string StravaCode { get; set; }
        public string CurrentPassword { get; set; }
   }
}
