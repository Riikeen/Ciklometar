using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class HomepageDTO
    {
        public Guid Id { get; set; }
        public int Bikers { get; set; }
        public int Organizations { get; set; }
        public float DistanceTravelled { get; set; }
        public float TimeSpentRiding { get; set; }
        public float RidesUndertaken { get; set; }
    }
}
