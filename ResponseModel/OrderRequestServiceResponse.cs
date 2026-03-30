using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class OrderRequestServiceResponse
    {
        public OrderRequestServiceResponse() 
        {
            this.OrderRequestServiceDetailList = new List<OrderRequestServiceDetailsResponse>();
        }

        public int OrderId { get; set; }
        public string BookingId { get; set; }
        public string BookingMemberType { get; set; }
        public string MemberName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public double TotalPrice { get; set; }
        public double ExtraCharge { get; set; }
        public string BookingDate { get; set; }

		public string BookingTime { get; set; }

        public double ServicePrice { get; set; }

        public string PaymentStatus { get; set; }
        public double QuotedPrice { get; set; }

        public string QuotationNote { get; set; }
        [StringLength(200)]
        public string QuotationFileLink { get; set; }

        public bool IsQuoteSystem { get; set; }

        public bool IsApprovalSystem { get; set; }

        public bool IsPackage { get; set; } 

        public bool IsApprovedByAdmin { get; set; }

        public MemberAddress MemberAddress { get; set; }

		public MemberVehicalInfo VehicalInfo { get; set; }
		public List<OrderRequestServiceDetailsResponse> OrderRequestServiceDetailList { get; set; }

        public DateTime? InTime{ get; set; }
        public DateTime? OutTime{ get; set; }

        public int FromAirportId { get; set; }
        public int ToAirportId { get; set; }
    }

    public class OrderRequestServiceDetailsResponse
    {
        public int? ServiceId { get; set; }
        public int? ServiceTypeId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public double GrossPrice { get; set; }

        public bool IsOfferDiscount { get; set; }

        public OfferInformation OfferInformation { get; set; }

        public MemberShip MembershipInformation { get; set; }

        public bool IsMembershipOrder { get; set; }

        [StringLength(100)]
        public string DiscountType { get; set; }

        public double DiscountAmount { get; set; }


    }

}