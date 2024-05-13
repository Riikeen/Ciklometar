using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class ActivitesInOrganizationDTO
    {
        public ActivityOutputDTO ActivityOutputDTO { get; set; }
        public string OrganizationName { get; set; }
        public string CyclistName { get; set; }
    }
}
