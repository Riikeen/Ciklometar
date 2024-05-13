using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarDAL.Models
{
    public class UserBan : BaseEntity
    {
        public string Reason { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public Guid BannedById { get; set; }
    }
}
