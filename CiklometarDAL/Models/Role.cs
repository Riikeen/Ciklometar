using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace CiklometarDAL.Models
{
    
    public enum UserType
    {
        Admin,
        Cyclist
    }
    public class Role
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public UserType UserType { get; set; }
        public User User { get; set; }
        public Organization Organization { get; set; }
       
    }
        
}
