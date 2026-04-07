using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Brand : BaseEntity
    {
        public Brand()
        {
            this.Products = new List<ProductSmallDetailsResponse>();
        }
        public int BrandId { get; set; }

        public Guid BrandKey { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        public string Details { get; set; }
        public string Image { get; set; }
        public string MediumImage { get; set; }
        public string LargeImage { get; set; }

        public bool? IsFeature {get;set;}

        public bool? IsTrenbding { get; set; }

        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }


        [NotMapped]
        public List<ProductSmallDetailsResponse> Products { get; set; }
        [NotMapped]
        public string FeatureCategoryKey { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }=false;

        [StringLength(250)]
        public string TitleAr { get; set; }
        public string DetailsAr { get; set; }

        public bool IsDeliveryEnabled { get; set; }
    }
}