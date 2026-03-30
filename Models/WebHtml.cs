using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Models
{
    public class WebHtml: BaseEntity
    {
        [Key]
        public long WebHtmlId { get; set; }
        public Guid WebHtmlkey { get; set; }
        [StringLength(250)]
        public string Identifier { get; set; }
        //[StringLength(100)]
        //public string PageIdentifier { get; set; }
        [StringLength(250)]
        public string Title { get; set; }
        [StringLength(500)]
        public string SubTitle { get; set; }
        [StringLength(500)]
        [AllowHtml]
        public string SmallDetailsOne { get; set; }
        [StringLength(500)]
        [AllowHtml]
        public string SmallDetailsTwo { get; set; }
        [AllowHtml]
        public string BigDetailsOne { get; set; }

        [AllowHtml]
        public string BigDetailsTwo { get; set; }
        [StringLength(250)]
        public string TitleAr { get; set; }
        [StringLength(500)]
        public string SubTitleAr { get; set; }
        [StringLength(500)]
        [AllowHtml]
        public string SmallDetailsOneAr { get; set; }
        [StringLength(500)]
        [AllowHtml]
        public string SmallDetailsTwoAr { get; set; }
        [AllowHtml]
        public string BigDetailsOneAr { get; set; }

        [AllowHtml]
        public string BigDetailsTwoAr { get; set; }
        [AllowHtml]
        [StringLength(150)]
        public string PictureOne { get; set; }
        [StringLength(150)]
        public string PictureTwo { get; set; }
        [StringLength(150)]
        public string PictureThree { get; set; }

        [StringLength(150)]
        public string PictureOneAr { get; set; }
        [StringLength(100)]
        public string ButtonText { get; set; }
        [StringLength(200)]
        public string ButtonLink { get; set; }  

        public int BrandId { get; set; }

        public int CategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }
        [ForeignKey("FeatureCategory")]
        public int? FeatureCategoryId { get; set; }
        [NotMapped]
        public string FeatureCategoryKey { get; set; }
        public string FeatureType { get; set; }

        public int FeatureTypeId { get; set; }

      

        [NotMapped]
        public virtual List<Category> CategoryTree { get; set; }
        [NotMapped]
        public virtual List<Brand> BrandTree { get; set; }
    }
}