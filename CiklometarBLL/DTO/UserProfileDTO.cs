using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class UserProfileDTO
    {
        public UserDTO UserData { get; set; }
        public bool CompareUserIsProfileUser { get; set; }
        public bool ProfileUserIsNotCyclist { get; set; }
        public bool CompareUserIsNotCyclist { get; set; }
        public List<OrganizationDTO> Organizations { get; set; }
        public List<RankedRideDTO> Rides { get; set; }
        public List<RankedRideDTO> RidesToCompareTo { get; set; }
    }
}
