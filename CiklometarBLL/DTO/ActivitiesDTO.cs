using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CiklometarBLL.DTO
{
    public class Athlete
    {
        public int Id { get; set; }
        public int Resource_state { get; set; }
    }
 
    public class ActivitiesDTO
    {
        public long Distance { get; set; }
        public string Type { get; set; }
        public string Average_Speed { get; set; }
        public string Start_date { get; set; }
        public Athlete Athlete { get; set; }
        public Map Map { get; set; }
        public int Moving_time { get; set; }
        public int Elapsed_time { get; set; }
        
    }
}
