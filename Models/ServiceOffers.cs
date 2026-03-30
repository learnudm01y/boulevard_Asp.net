using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ServiceOffers
    {
        public int ServiceOffersId { get; set; }

        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }

        public Service Service { get; set; }



        [ForeignKey(nameof(ServiceType))]
        public int? ServiceTypeId { get; set; }

        public ServiceType ServiceType { get; set; }

        public bool IsFeature { get; set; }

        [ForeignKey(nameof(OfferInformation))]
        public int? OfferInformationId { get; set; }
        public virtual OfferInformation OfferInformation { get; set; }
    }
}