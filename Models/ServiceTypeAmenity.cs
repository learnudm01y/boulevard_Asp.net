using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ServiceTypeAmenity:BaseEntity
    {
        [Key]
        public int ServiceAmenityId { get; set; }

        public Guid ServiceAmenityKey { get; set; }


        [ForeignKey("ServiceType")]
        public int? ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; }

        [StringLength(100)]
        public string AmenitiesName { get; set; }
        [StringLength(100)]
        public string AmenitiesNameAr { get; set; }

        [StringLength(100)]
        public string AmenitiesLogo { get; set; }


        [StringLength(100)]
        public string AmenitiesType { get; set; }

  
        public bool LinkedWithFile { get; set; }

        [NotMapped]
        public string FileLink { get; set; }

        [NotMapped]

        public string FileType { get; set; }
        
        [NotMapped]

        public ServiceTypeFile ServiceTypeFile { get; set; }


    }
}