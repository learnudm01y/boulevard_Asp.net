using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class OfferInformation:BaseEntity
    {
        public OfferInformation() {
            Banners = new List<OfferBanner>();
            //Brands = new List<Brand>();
            Products = new List<ProductSmallDetailsResponse>();
            Categories = new List<Category>();
            Services = new List<SmallServiceDetailsResponse>();
            //ServicesDetails = new SmallServiceDetailsResponse();
        }
        public int OfferInformationId { get; set; }

        public Guid OfferInformationKey { get; set; }

        [StringLength(250)]
        public string Title { get; set; }
        public string Description { get; set; }

        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }

        public bool IsBrand { get; set; }

        public bool IsCategory { get; set; }
        public bool IsProduct { get; set; }

        public bool IsService { get; set; }

        [StringLength(100)]
        public string FeatureType { get; set; }

        public bool IsTimeLimit { get; set; }

        public bool IsTrending { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(250)]
        public string TitleAr { get; set; }
        public string DescriptionAr { get; set; }
        [StringLength(100)]
        public string FeatureTypeAr { get; set; }

        [NotMapped]

        public bool RedirectToServiceDetails { get; set; }

        [NotMapped]
        public List<OfferBanner> Banners { get; set; }

        [NotMapped]
        public List<Brand> Brands { get; set; }

        [NotMapped]
        public List<ProductSmallDetailsResponse> Products { get; set; }

        [NotMapped]
        public List<SmallServiceDetailsResponse> Services { get; set; }

        [NotMapped]
        public SmallServiceDetailsResponse ServicesDetails { get; set; }

        [NotMapped]
        public List<Category> Categories { get; set; }

        [NotMapped]
        public string FeatureCategoryKey { get; set; }

        [NotMapped]
        public bool IsFeature {  get; set; }

        [NotMapped]
        public OfferBanner OfferBanner { get; set; }
        [NotMapped]
        public string BannerImage { get; set; }

        [NotMapped]
        public List<Product> ProductList { get; set; }
        [NotMapped]
        public List<Service> ServiceList { get; set; }

        [NotMapped]
        public List<ServiceType> ServiceTypeList { get; set; }
        [NotMapped]
        public List<OfferDiscount> OfferDiscount { get; set; }
        [NotMapped]
        public string DiscountType { get; set; }
        [NotMapped]
        public string DiscountTypeAr { get; set; }
        [NotMapped]
        public int DiscountAmount { get; set; }

    }
}