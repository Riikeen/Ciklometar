using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CiklometarBLL.DTO
{
    public class AthleteDTO
    {
        public int Id { get; set; }
        public int Resource_state { get; set; }
    }
    public class Map
    {
        public string id { get; set; }
        public string polyline { get; set; }
        public int resource_state { get; set; }
        public string summary_polyline { get; set; }
    }

    public class ActivitySyncDTO
    {
        public long Id { get; set; }
        public long Distance { get; set; }
        public string Type { get; set; }
        public string Average_Speed { get; set; }
        public string Start_date { get; set; }
        public Athlete Athlete { get; set; }
        public Map Map { get; set; }
        public int Moving_time { get; set; }
        public int Elapsed_time { get; set; }
        public List<double> end_latlng { get; set; }
        public DateTime start_date_local { get; set; }

    }
}
