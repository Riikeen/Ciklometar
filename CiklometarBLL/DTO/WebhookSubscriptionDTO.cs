using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CiklometarBLL.DTO
{
    public class WebhookSubscriptionDTO
    {
        public int id { get; set; }
        public int resource_state { get; set; }
        public int application_id { get; set; }
        public string callback_url { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}




