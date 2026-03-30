using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ServiceAmenity:BaseEntity
    {
        public int ServiceAmenityId { get; set; }

        public Guid ServiceAmenityKey { get; set; }

        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }

        [StringLength(100)]
        public  string AmenitiesName { get; set; }
        [StringLength(100)]
        public string AmenitiesNameAr { get; set; }

        [StringLength(100)]
        public string AmenitiesLogo { get; set; }

        [NotMapped]
        public string FeatureCategoryKey { get; set; }

    }
}