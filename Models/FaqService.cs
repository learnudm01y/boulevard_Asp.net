using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class FaqService :BaseEntity
    {
        public int FaqServiceId { get; set; }
        public Guid FAQKey { get; set; }

       
        public string FaqTitle { get; set; }

        public string FaqDescription { get; set;}

        public string FeatureType { get; set; }

        public int FeatureTypeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdate { get; set; }

        [NotMapped]
        public string FeatureCategoryKey { get; set; }
        [NotMapped]
        public int ServiceId { get; set; }
        public string FaqTitleAr { get; set; }

        public string FaqDescriptionAr { get; set; }

        public string FeatureTypeAr { get; set; }

    }
}