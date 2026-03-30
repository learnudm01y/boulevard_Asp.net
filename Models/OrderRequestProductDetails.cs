using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class OrderRequestProductDetails:BaseEntity
    {
        public int OrderRequestProductDetailsId { get; set; }

        [ForeignKey("OrderRequestProduct")]
        public int OrderRequestProductId { get; set; }
        public virtual OrderRequestProduct OrderRequestProduct { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [ForeignKey("ProductPrice")]
        public int? ProductPriceId { get; set; }
        public virtual ProductPrice ProductPrice { get; set; }

        public int Quantity { get; set; }

        public double GrossPrice { get; set; }

        [ForeignKey(nameof(OfferInformation))]
        public int? OfferInformationId { get; set; }
        public virtual OfferInformation OfferInformation { get; set; }

        public bool IsMembershipOrder { get; set; }

        [StringLength(100)]
        public string DiscountType { get; set; }

        public double DiscountAmount { get; set; }

        public int MembershipId { get; set; }

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
        [NotMapped]
        public double TotalGrossPrice { get; set; }

        [NotMapped]
        public string BrandName { get; set; }

    }
}