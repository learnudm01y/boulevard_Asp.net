using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Boulevard.Models
{
    public class UpsellFeatures:BaseEntity
    {
        public int UpsellFeaturesId { get; set; }

        public Guid UpsellFeaturesKey { get; set; }

        [StringLength(50)]
        public string UpsellFeaturesType { get; set; }
        [StringLength(50)]
        public string UpsellFeaturesTypeAr { get; set; }


        public int UpsellFeaturesTypeId { get; set; }

        public int RelatedFeatureId { get; set; }
        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }

    }
}