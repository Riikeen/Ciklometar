using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class LocationResponseDTO
    {
        public Guid OrganizationId { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
    }
}
