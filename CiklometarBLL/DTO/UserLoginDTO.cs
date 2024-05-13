using CiklometarDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CiklometarBLL.DTO
{
   public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
