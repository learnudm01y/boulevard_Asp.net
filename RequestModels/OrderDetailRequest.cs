using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class OrderDetailRequest
    {
        public int ProductId { get; set; }

        public int ProductPriceId { get; set; }


        public int Quantity { get; set; }

        public double GrossPrice { get; set; }

        public int OfferInformationId { get; set; }
        public bool IsMembershipOrder { get; set; }


        public string MembershipDiscountType { get; set; }

        public double MembershipDiscountAmount { get; set; }

        public int MembershipId { get; set; }
    }
}