using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class OrderRequestService
    {
        public int OrderRequestServiceId { get; set; }

        [StringLength(100)]
        public string BookingId { get; set; }

        [StringLength(100)]
        public string BookingMemberType { get; set; }

        [ForeignKey(nameof(Member))]
        public long MemberId { get; set; }
     
        public virtual Member Member { get; set; }

        [StringLength(10)]
        public string MemberNameTitle { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string PhoneCode { get; set; }

        [StringLength(100)]
        public string PhoneNo { get; set; }

   
        public double ExtraCharge { get; set; }

        public double DeliveryCharge { get; set; }
        public double ServiceCharge { get; set; }

        public double TotalPrice { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BookingDate { get; set; }
        [DataType(DataType.Time), DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [StringLength(100)]
		public string BookingTime { get; set; }

		public string BookingStatus { get; set; }

        [ForeignKey(nameof(MemberAddresses))]
        public long? MemberAddressId { get; set; }

        public virtual MemberAddress MemberAddresses { get; set; }

        [ForeignKey(nameof(MemberVehicalInfo))]
        public int? MemberVehicalInfoId { get; set; }

        public string PassportCopy {  get; set; }

        public virtual MemberVehicalInfo MemberVehicalInfo { get; set; }

        [ForeignKey(nameof(FeatureCategory))]
        public int? FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }


        [StringLength(100)]
        public string PaymentStatus { get; set; }

    
        public double QuotedPrice { get; set; }

        public string QuotationNote { get; set; }
        [StringLength(200)]
        public string QuotationFileLink { get; set; }



        public bool IsApprovedByAdmin { get; set; }
        public int ApprovedBy { get; set; }

        [StringLength(100)]
        public string PaymentTransectionId { get; set; }
        public ICollection<OrderRequestServiceDetails> OrderRequestServiceDetailList { get; set; }

        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public bool IsPackage { get; set; }


        //[NotMapped]
        //public bool IsMemberShipOffer { get; set; }
        //[NotMapped]
        //public bool IsOffer { get; set; }


        public int FromAirportId { get; set; }
        public int ToAirportId { get; set; }

        public bool IsSound { get; set; } = false;
        public bool IsAdmin { get; set; } = false;

        [ForeignKey("PaymentMethod")]
        public int? PaymentMethodId { get; set; }

        public virtual PaymentMethod PaymentMethod { get; set; }
        [NotMapped]
        public int CountryId { get; set; }
        [NotMapped]
        public int CityId { get; set; }

    }
}