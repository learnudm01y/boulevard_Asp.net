
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;


namespace Boulevard.ResponseModel
{
    public class CreateOrderCourierResponse
    {
       
        public string success { get; set; }

      
        public string message { get; set; }

        [JsonPropertyName("Order Id")]
        public string AWB_No { get; set; }
    }
}