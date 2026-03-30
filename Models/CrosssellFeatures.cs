using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class CrosssellFeatures:BaseEntity
    {
        public int CrosssellFeaturesId { get; set; }

        public Guid CrosssellFeaturesKey { get; set; }

        [StringLength(50)]
        public string CrosssellFeaturesType { get; set; }


        public int CrosssellFeaturesTypeId { get; set; }

        public int RelatedFeatureId { get; set; }

        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }
    }
}