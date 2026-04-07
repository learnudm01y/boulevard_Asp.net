using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class TempProduct
    {
        [Key]
        public Guid TempId { get; set; }
        public string SrNo { get; set; }
        public string Brand { get; set; }
        public string BrandArabic { get; set; }

        public string Barcode { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }

        public string SubSubCategory { get; set; }

        public string CategoryArabic { get; set; }
        public string SubCategoryArabic { get; set; }

        public string SubSubCategoryArabic { get; set; }
        public string ItemDesc { get; set; }

        public string ItemDescArabic { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeName { get; set; }
        public string Images { get; set; }
        public string Quantity { get; set; }
        public string SellingPrice { get; set; }
        public string ProductTags { get; set; }
        public string Stocks { get; set; }
        public int ExcelCount { get; set; }
        public string ProductName { get; set; }

        public string ProductNameArabic { get; set; }
        public int FeatureCategoryId { get; set; }

        public string ProductType { get; set; }
        [StringLength(250)]
        public string AttributeNameArabic { get; set; }
        public string DeliveryInfoArabic { get; set; }
        public string DeliveryInfo { get; set; }

        public string CategoryImage { get; set; }

        public string SubCategoryImage { get; set; }

        public string SubSubCategoryImage { get; set; }

        // 4th-level category (child of SubSubCategory).
        // NOTE: commas in any category name field are part of the name and are NEVER
        // used as value separators — only Quantity/Price/Stocks fields use ; or , separators.
        public string MiniCategory { get; set; }
        public string MiniCategoryArabic { get; set; }

        /// <summary>
        /// ICV Boulevard Score — read from optional Excel column "ICV Boulevard Score" (e.g. "100%").
        /// </summary>
        [StringLength(50)]
        public string IcvBoulevardScore { get; set; }

        /// <summary>
        /// Product origin (e.g. "Local", "Imported") — read from optional Excel column "Origin".
        /// </summary>
        [StringLength(100)]
        public string Origin { get; set; }

    }
}