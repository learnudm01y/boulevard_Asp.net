using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class OfferDiscount
    {
        public int OfferDiscountId { get; set; }
        [StringLength(100)]
        public string DiscountType { get; set; }

     
        public int DiscountAmount { get; set; }
        [ForeignKey(nameof(OfferInformation))]
        public int? OfferInformationId { get; set; }
        public virtual OfferInformation OfferInformation { get; set; }
        [StringLength(100)]
        public string DiscountTypeAr { get; set; }
    }
}