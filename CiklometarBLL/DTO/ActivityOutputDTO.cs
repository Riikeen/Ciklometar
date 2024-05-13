using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class ActivityOutputDTO
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public long ActivityId { get; set; }
        public long Distance { get; set; }
        public string Type { get; set; }
        public long AvgSpeed { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public string AthleteId { get; set; }
        public int Moving_time { get; set; }
        public int Elapsed_time { get; set; }
        public DateTime Event_time { get; set; }
    }
}
