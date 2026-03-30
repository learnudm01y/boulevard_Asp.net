using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class DeliveryOrderForCourierRequest
    {

        
            public string order_type { get; set; }
            public string pickup_building { get; set; }
            public string pickup_street { get; set; }
            public string pickup_area { get; set; }
            public string pickup_city { get; set; }
            public string destination_name { get; set; }
            public string destination_address { get; set; }
            public string destination_building { get; set; }
            public string destination_street { get; set; }
            public string destination_area { get; set; }
            public string destination_city { get; set; }
            public string payment_method { get; set; }
            public double cod_amount { get; set; }
            public int no_of_packages { get; set; }
            public double tip_amount { get; set; }
            public string pickup_contact_phone_country_code { get; set; }
            public string pickup_contact_phone { get; set; }
            public string recipient_contact_phone_country_code { get; set; }
            public string recipient_contact_phone { get; set; }
            public string pickup_name { get; set; }
            public string recipient_name { get; set; }
            public string extra_info { get; set; }
            public string planned_start_time { get; set; }
            public string planned_delivery_time { get; set; }
            public string vehicle_type { get; set; }
            public string pick_lat { get; set; }
            public string pick_long { get; set; }
            public string drop_lat { get; set; }
            public string drop_long { get; set; }
        }
    
    
}