using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class MemberShipDiscountCategory
    {
        public int MemberShipDiscountCategoryId { get; set; }

        public int MemberShipId { get; set; }

        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }

        [StringLength(100)]
        public string MemberShipDiscountType { get; set; }

       
        public decimal MemberShipDiscountAmount { get; set; }

        public DateTime UpdateAt { get; set; }

        [NotMapped]
        public virtual MemberShip MemberShip { get; set; }

        [StringLength(100)]
        public string MemberShipDiscountTypeAr { get; set; }

    }
}