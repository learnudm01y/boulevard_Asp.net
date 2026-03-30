using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class OrderSubmitRequest
    {
       
        public int MemberId { get; set; }
       
        public int MemberAddressId { get; set; }

        public double TotalPrice { get; set; }

        public double DeliveryCharge { get; set; }
        public double ServiceCharge { get; set; }

        public double Tip { get; set; }
        public int PaymentMethodId { get; set; }
        public string Comments { get; set; }

        public int featureCategoryId { get; set; }
        public DateTime? DeliveryDateTime { get; set; }

        public string PaytmentTransectionId { get; set; }


        public int ProductTypeId { get; set; } 
   

        public List<OrderDetailRequest> Details { get; set; }
    }
}