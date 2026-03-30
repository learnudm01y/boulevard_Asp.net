using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Category: BaseEntity
    {
        public Category()
        {
            this.ChildCategories = new List<Category>();
            this.Products = new List<ProductSmallDetailsResponse>();
            this.Services = new List<SmallServiceDetailsResponse>();
            this.MainServices = new List<ParentSmallServiceDetailsResponse>();
        }
        [Key]
        public int CategoryId { get; set; }
        public Guid CategoryKey { get; set; }
        [StringLength(250)]
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        [StringLength(180)]
        public string Image { get; set; }

        [StringLength(250)]
        public string Icon { get; set; }
        public int? ParentId { get; set; }
        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }
        [NotMapped]
        public string FeatureCategoryKey { get; set; }

        public bool? IsTop { get; set; }

        public bool? IsTrenbding { get; set; }

        public int? AlternateFeatureCategoryId { get; set; }

        public int? AlternateServiceId { get; set; }

        public bool IsPackagecategory { get; set; }

        public int label { get; set; }

        [NotMapped]
        public virtual List<Category> ChildCategories { get; set; }

        [NotMapped]
        public virtual List<Category> CategoryTree { get; set; }

        [NotMapped]
        public List<ProductSmallDetailsResponse> Products { get; set; }

        [NotMapped]

        public List<SmallServiceDetailsResponse> Services { get; set; }

        [NotMapped]

        public List<ParentSmallServiceDetailsResponse> MainServices { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; } = false;

        [StringLength(250)]
        public string CategoryNameAr { get; set; }
        public string CategoryDescriptionAr { get; set; }
    }
}