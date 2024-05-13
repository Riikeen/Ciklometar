using CiklometarDAL.Models;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
   public class OrganizationProfileDTO
    {
        public OrganizationDTO OrganizationData { get; set; }
        public bool UserIsInOrganization { get; set; }
        public bool UserAlreadySentJoinRequest { get; set; }
        public List<OrganizationRankingDTO> RankedRidesByMembers { get; set; }
    }
}
