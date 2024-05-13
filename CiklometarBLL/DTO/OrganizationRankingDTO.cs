using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class OrganizationRankingDTO
    { 
        public UserCyclistDTO UserData { get; set; }
        public List<RankedRideDTO> RankedRidesByThisUser { get; set; }
    }
}
