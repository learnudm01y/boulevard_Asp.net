using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class PropertyInformation: BaseEntity
    {
        public PropertyInformation()
        {
            this.ExteriorsImage = new List<string>();
            this.InteriorsImage = new List<string>();
        }
        [Key]
        public int PropertyInformationId { get; set; }

    
        public Guid PropertyInfoKey { get; set; }

        [StringLength(100)]
        public string Type { get; set; }


        [StringLength(100)]
        public string Purpose { get; set; }
        [StringLength(100)]
        public string RefNo { get; set; }
        [StringLength(100)]
        public string Furnishing { get; set; }


        [StringLength(100)]
        public string TypeAr { get; set; }


        [StringLength(100)]
        public string PurposeAr { get; set; }
        [StringLength(100)]
        public string RefNoAr { get; set; }
        [StringLength(100)]
        public string FurnishingAr { get; set; }

        [StringLength(100)]
        public string PropertyWhatsAppNo { get; set; }

        [StringLength(100)]
        public string PropertyPhoneNo { get; set; }

        [StringLength(100)]
        public string PropertyEmail { get; set; }

        [ForeignKey("ServiceType")]
        public int? ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; }

        public string Exteriors { get; set; }

        public string Interiors { get; set; }
        public string ExteriorsAr { get; set; }

        public string InteriorsAr { get; set; }

        [ForeignKey(nameof(City))]
        public int? CityId { get; set; }
        public virtual City City { get; set; }
        [ForeignKey(nameof(Country))]
        public int? CountryId { get; set; }
        public virtual Country Country { get; set; }
        [NotMapped]

        public List<string> ExteriorsImage { get; set; }

        [NotMapped]

        public List<string> InteriorsImage { get; set; }
        
        [NotMapped]

        public List<ServiceTypeFile> ExteriorsImages { get; set; }

        [NotMapped]

        public List<ServiceTypeFile> InteriorsImages { get; set; }


    }
}