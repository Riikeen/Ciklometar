using CiklometarDAL;
using CiklometarDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL.Services
{
    public class SeedData
    {
        public static void SeedSuperAdmin(CiklometarContext context)
        {
            if(!context.Users.Any(u => u.IsSuperAdmin == true))
            {
                var salt = BLLFunctions.CreateSalt();
                var superAdmin = new User()
                {
                    FirstName = "SuperAdmin",
                    LastName = "",
                    Nickname = "SuperAdmin",
                    IsSuperAdmin = true,
                    Salt = salt,
                    Email = "ciklometar@admin.com",
                    Password = BLLFunctions.CreateHash("ciklometaradmin",salt)
                    
                };
                context.Add(superAdmin);
                context.SaveChanges();
            }
        }
    }
}
