using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CiklometarBLL.DTO
{
    public class RankedRideDTO
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationData { get; set; }
        public DateTime Timestamp { get; set; }
        public long DistanceCycled { get; set; }
        public long TotalTimeCycled { get; set; }
        public long AverageSpeed { get; set; }
        [JsonIgnore]
        public string AthleteId { get; set; }
    }
}
