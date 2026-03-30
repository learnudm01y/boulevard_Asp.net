using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Models
{
    public class ServiceType :BaseEntity
    {
        public ServiceType()
        {
            this.ServiceTypeImages = new List<string>();
            this.ServiceCategories = new List<Category>();
            this.City = new City();
            this.Country = new Country();
        }
        [Key]
        public int ServiceTypeId { get; set; }

        public Guid ServiceTypeKey { get; set; }

        [StringLength(250)]
        public string ServiceTypeName { get; set; }
        [StringLength(250)]
        public string ServiceTypeNameAr { get; set; }

        public int PersoneQuantity { get; set; }


       
        public int AdultQuantity { get; set; }

        public int ChildrenQuantity { get; set; }

        public string Description { get; set; }
        public string DescriptionAr { get; set; }

        public int ServiceHour { get; set; }

        public int ServiceMin { get; set; }

        [StringLength(100)]
        public string Size { get; set; }
        [StringLength(100)]
        public string SizeAr { get; set; }

        [StringLength(100)]
        public string Image { get; set; }

        [StringLength(100)]
        public string PaymentType { get; set; }

        public bool IsPackage { get; set; }
        public double Price { get; set; }
        [StringLength(100)]
        public string Latitute { get; set; }

        [StringLength(100)]
        public string Logitute { get; set; }
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
        [AllowHtml]
        public string BigDescription { get; set; }

        [AllowHtml]
        public string BigDescriptionAr { get; set; }

        public int? CityId { get; set; }
     
        public int? CountryId { get; set; }
       
        public string Address { get; set; }
        [AllowHtml]
        public string ServicePrice { get; set; }
        [AllowHtml]
        public string ServicePriceAr { get; set; }

        [NotMapped]
        public string FeatureCategoryKey { get; set; }

        [NotMapped]
        public double DiscountPrice { get; set; }


        [NotMapped]
        public City City { get; set; }

        [NotMapped]
        public Country Country { get; set; }

        [NotMapped]
        public OfferDiscount DiscountInformation { get; set; }

		[NotMapped]

        public bool IsFavourite { get; set; }

        [NotMapped]

        public int  CartQuantity { get; set; }

        [NotMapped]
        public string OfferName { get; set; }
        [NotMapped]
        public int OfferDiscount { get; set; }
        [NotMapped]
        public string OfferDiscountType { get; set; }

        [NotMapped]
        public int CategoryId { get; set; }
        [NotMapped]
        public virtual Category Category { get; set; }

        [NotMapped]


        public MemberShipDiscountCategory MemberShipInformation { get; set; }


        [NotMapped]
        public string MemberShipName { get; set; }
        [NotMapped]
        public string DiscountType { get; set; }
        [NotMapped]
        public double DiscountAmount { get; set; }

        [NotMapped]
        public bool IsMemberShipOffer { get; set; }
        [NotMapped]
        public bool IsOffer { get; set; }

        [NotMapped]
        public List<string> ServiceTypeImages { get; set; }

        [NotMapped]
        public List<Category> CategoryTree { get; set; }
        [NotMapped]
        public List<Category> ServiceCategories { get; set; }
        [NotMapped]
        public string SelectedCategoryId { get; set; }

        [NotMapped]
        public List<string> ImageUrl { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; } = false;

        [NotMapped]
        public List<ServiceTypeFile> ServiceTypeFiles { get; set; }

        [NotMapped]

        public double AvgRatings { get; set; }

        [NotMapped]

        public double TotalReview { get; set; }
    }
}