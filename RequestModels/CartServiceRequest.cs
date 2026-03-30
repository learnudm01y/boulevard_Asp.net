using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class CartServiceRequest
    {
        public int MemberId { get; set; }

        public int ServiceId { get; set; }

        public int ServiceTypeId { get; set; }

        public int FeatureCategoryId { get; set; }

        public int Quantity { get; set; }
    }
}