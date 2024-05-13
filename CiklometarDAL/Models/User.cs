
using System.Collections.Generic;

namespace CiklometarDAL.Models
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Nickname { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string StravaId { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<Requests> Requests { get; set; }
        public ICollection<UserBan> Bans { get; set; }
    }
}
