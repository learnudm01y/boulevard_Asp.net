using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using static System.Collections.Specialized.BitVector32;

namespace Boulevard.Models
{
    public class Service:BaseEntity
    {
        public Service() 
        {
            this.ServiceImageURLs = new List<string>();
			this.Faqs = new List<FaqResponse>();
            this.Airports = new List<Airport>();
		}    
        public int ServiceId { get; set; }

        public Guid ServiceKey { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        [StringLength(100)]
        public string NameAr { get; set; }
        [AllowHtml]
        public string DescriptionAr { get; set; }
        public int ServiceHour { get; set; }

        public int ServiceMin { get; set; }
        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }
        [NotMapped]
        public string FeatureCategoryKey { get; set; }
        [ForeignKey(nameof(City))]
        public int? CityId { get; set; }
        public virtual City City { get; set; }
        [ForeignKey(nameof(Country))]
        public int? CountryId { get; set; }
        public virtual Country Country { get; set; }
        public string Address { get; set; }
        public double Ratings { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        [StringLength(100)]
        public string PropertyType { get; set; }

        [StringLength(100)]
        public string Latitute { get; set; }

        [StringLength(100)]
        public string Logitute { get; set; }

        [AllowHtml]
        public string AboutUs { get; set; }
        [AllowHtml]
        public string ScopeOfService { get; set; }

        [AllowHtml]
        public string AboutUsAr { get; set; }
        [AllowHtml]
        public string ScopeOfServiceAr { get; set; }
        public string SpokenLanguages { get; set; }

        public ICollection<ServiceImage> ServiceImages { get; set; }

        public bool IsPackage { get; set; } = false;
        public decimal Price { get; set; } = 0;
        public decimal DistanceInKM { get; set; } = 0;

        public int ParentId { get; set; } = 0;

        [NotMapped]
        public List<string> ServiceImageURLs { get; set; }

		[NotMapped]
		public List<FaqResponse> Faqs { get; set; }

		public ICollection<ServiceType> ServiceTypes { get; set; }
        public ICollection<ServiceAmenity> ServiceAmenities { get; set; }

        [NotMapped]
        public List<UserReview> UserReviews { get; set; }

        public ICollection<ServiceLandmark> ServiceLandmarks { get; set; }
        [NotMapped]
        public int CategoryId { get; set; }
        [NotMapped]
        public string ParentServiceName { get; set; }

        [NotMapped]
        public virtual Category Category { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; } = false;


        [NotMapped]
        public List<Airport> Airports { get; set; }




    }
}