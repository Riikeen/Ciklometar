using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CiklometarDAL.Models
{
    public class Activity
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public long ActivityId { get; set; }
        public long Distance { get; set; }
        public string Type { get; set; }
        public long AvgSpeed { get; set; }
        public Point EndLocation { get; set; }
        public string AthleteId { get; set; }
        public DateTime Event_time { get; set; }
        public int Elapsed_time { get; set; }
        public int Moving_time { get; set; }

    }
}
