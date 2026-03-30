using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class BrandOffer
    {
        public int BrandOfferId { get; set; }

        [ForeignKey(nameof(Brand))]
        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        [ForeignKey(nameof(OfferInformation))]
        public int? OfferInformationId { get; set; }
        public virtual OfferInformation OfferInformation { get; set; }


    }
}