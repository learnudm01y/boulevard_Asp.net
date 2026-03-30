using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class OrderRequestServiceRequest
    {
        public int OrderRequestServiceId { get; set; }
        public string BookingMemberType { get; set; }
        public long MemberId { get; set; }
        public string MemberNameTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneCode { get; set; }
        public string PhoneNo { get; set; }
        public double ExtraCharge { get; set; }
        public double TotalPrice { get; set; }

        public string Comments { get; set; }
        public string BookingDate { get; set; }
        public double DeliveryCharge { get; set; }
        public double ServiceCharge { get; set; }
        public string BookingTime {  get; set; }

		public long? MemberAddressId { get; set; }

		public int MemberVehicalInfoId { get; set; }
		public int featureCategoryId { get; set; }

        public string PassportCopy { get; set; }

        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }

        public string PaytmentTransectionId { get; set; }

        public int PaymentMethodId { get; set; }



        public List<OrderRequestServiceDetailsRequest> OrderRequestServiceDetailList { get; set; }

        public bool IsPackage {  get; set; }

        public int FromAirportId { get; set; }
        public int ToAirportId { get; set; }
    }
}