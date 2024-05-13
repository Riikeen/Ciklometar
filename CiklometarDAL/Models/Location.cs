using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarDAL.Models
{
    public class Location: BaseEntity
    {
        public Guid OrganizationId { get; set; }
        public Point Coordinates { get; set; }
        public Organization Organization { get; set; }
    }
}
