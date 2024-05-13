using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CiklometarDAL.Models
{
    public class Organization : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<Location> Locations { get; set; }
        public ICollection<UserBan> Bans { get; set; }
    }
}
