using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class RequestsGetDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public UserDTO User { get; set; }
        public OrganizationDTO Organization { get; set; }
    }
}
