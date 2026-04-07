using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class FeatureCategory
    {
        [Key]
        public int FeatureCategoryId { get; set; }
        public Guid FeatureCategoryKey { get; set; }
        public string Name { get; set; }

        public string NameAr { get; set; }
        [StringLength(180)]
        public string Image { get; set; }
        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }

        [StringLength(100)]
        public string FeatureType { get; set; }

        public bool IsWaitForApproval { get; set; }

        public bool IsQuoteEnable { get; set; }

        public double? DeliveryCharge { get; set; }
        public double? ServiceFee { get; set; }

        /// <summary>
        /// Commission percentage deducted from each order in this category (e.g. 3.00 = 3%)
        /// </summary>
        public decimal? CommissionRate { get; set; }

        [NotMapped]
        public int CategoryWiseOrderCount { get; set; }

        [NotMapped]
        public List<ProductSmallDetailsResponse> Products { get; set; }


        [NotMapped]
        public List<SmallServiceDetailsResponse> Services { get; set; }
    }
}