using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class CommonProductTag:BaseEntity
    {
        public int CommonProductTagId { get; set; }

        [StringLength(250)]
        public string TagName { get; set; }

        [ForeignKey(nameof(FeatureCategory))]
        public int? FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }

        [StringLength(100)]
        public string Status {get;set;}
        [StringLength(250)]
        public string TagNameAr { get; set; }
    }
}