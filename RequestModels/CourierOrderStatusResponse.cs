using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class CourierOrderStatusResponse
    {
        public string order_id { get; set; }
        public string status { get; set; }
        public string cancelreason { get; set; }
        public DateTime event_date_time { get; set; }
        public string rider_name { get; set; }
        public string rider_phone { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string pod_image { get; set; }
    }
}