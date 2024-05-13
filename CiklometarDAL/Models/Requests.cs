using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarDAL.Models
{
    public class Requests : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
