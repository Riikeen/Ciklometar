using CiklometarDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace CiklometarBLL.DTO
{
   public class RoleDTO
   {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public UserType UserType { get; set; }
    }
}
