using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class OrderRequestServiceDetailsRequest
    {
        public int ServiceId { get; set; }
        public int ServiceTypeId { get; set; }
        public double GrossPrice { get; set; }

        public int OfferInformationId { get; set; }

        public bool IsMembershipOrder { get; set; }


        public string MembershipDiscountType { get; set; }

        public double MembershipDiscountAmount { get; set; }

        public int MembershipId { get; set; }

        public int Quantity { get; set; }
    }
}