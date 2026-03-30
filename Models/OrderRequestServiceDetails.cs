using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class OrderRequestServiceDetails
    {
        public int OrderRequestServiceDetailsId { get; set; }

        [ForeignKey("OrderRequestService")]
        public int OrderRequestServiceId { get; set; }
        public virtual OrderRequestService OrderRequestService { get; set; }

        [ForeignKey("Service")]
        public int? ServiceId { get; set; }
        public virtual Service Service { get; set; }

        [ForeignKey("ServiceType")]
        public int? ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        [NotMapped]
        public virtual List<ServiceType> ServiceTypeList { get; set; }

        public double GrossPrice { get; set; }

        [ForeignKey(nameof(OfferInformation))]
        public int? OfferInformationId { get; set; }
        public virtual OfferInformation OfferInformation { get; set; }

        public bool IsMembershipOrder { get; set; }

        [StringLength(100)]
        public string DiscountType { get; set; }

        public double DiscountAmount { get; set; }

        public int MembershipId { get; set; }

        public int Quantity { get; set; }



        [NotMapped]
        public string OfferName { get; set; }
        [NotMapped]
        public int OfferDiscount { get; set; }
        [NotMapped]
        public string OfferDiscountType { get; set; }

        [NotMapped]
        public string MemberShipName { get; set; }

        [NotMapped]
        public bool IsMemberShipOffer { get; set; }
        [NotMapped]
        public bool IsOffer { get; set; }
    }
}