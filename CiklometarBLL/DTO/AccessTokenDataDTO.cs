using CiklometarDAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class AccessTokenDataDTO
    {
        //whatever data you put into the access token
        //realize that it won't be changed until a new one is issued
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public bool IsSuperAdmin { get; set; }
        public List<Guid> AdminOfOrganizations { get; set; }
        public List<Guid> MemberOfOrganizations { get; set; }
        public List<Guid> OrganizationsSentRequestTo { get; set; }
        public bool NoCredentials { get; set; }
        public bool ConnectedWithStrava { get; set; }
    }
}
