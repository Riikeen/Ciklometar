using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class StravaWebhookDTO
    {
        public enum Aspect_Type
        {
            create,
            delete,
            update
        };
        public enum Object_Type 
        { 
            activity,
            athlete
        };
        public string Aspect_type { get; set; }
        public long Event_time { get; set; }
        public string Object_type { get; set; }
        public long Object_id { get; set; }
        public long Owner_id { get; set; }
        public int Subscription_id { get; set; }
    }
}
