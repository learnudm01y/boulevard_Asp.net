using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ProductOffer
    {
        public int ProductOfferId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public bool IsFeature { get; set; }

        [ForeignKey(nameof(OfferInformation))]
        public int? OfferInformationId { get; set; }
        public virtual OfferInformation OfferInformation { get; set; }

    }
}