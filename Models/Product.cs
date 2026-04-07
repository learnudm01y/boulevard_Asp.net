using Boulevard.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Boulevard.Models
{
    public class Product : BaseEntity
    {
        private decimal _productPrice;
        private double _discountPrice;
        public Product()
        {
            this.Images = new List<ProductImage>();
            this.ImageUrl = new List<string>();
            this.CategoryTree = new List<Category>();
            this.ProductCategories = new List<Category>();
            this.MemberShipInformation = new MemberShipDiscountCategory();
            this.StockLogList = new List<StockLog>();

        }
        [Key]
        public int ProductId { get; set; }
        public Guid ProductKey { get; set; }
        [StringLength(250)]
        public string ProductName { get; set; }
        [StringLength(250)]
        public string ProductSlag { get; set; }
        public string ProductDescription { get; set; }

        [StringLength(250)]
        public string ProductNameAr { get; set; }

        [StringLength(250)]
        public string AttributeCode { get; set; }

        [StringLength(250)]
        public string AttributeName { get; set; }

        [StringLength(250)]
        public string AttributeNameArabic { get; set; }
        [StringLength(250)]
        public string ProductSlagAr { get; set; }
        public string ProductDescriptionAr { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }
        [ForeignKey("FeatureCategory")]
        public int? FeatureCategoryId { get; set; }
        [NotMapped]
        public string FeatureCategoryKey { get; set; }
        public decimal ProductPrice
        {
            get => _productPrice;
            set => _productPrice = Math.Max(0, value);
        }

        public int AvgRatings { get; set; }

        public string DeliveryInfo { get; set; }

        public string DeliveryInfoArabic { get; set; }

        [ForeignKey("Brands")]
        public int? BrandId { get; set; }

        public virtual Brand Brands { get; set; }

        public string Barcode { get; set; }

        public int StockQuantity { get; set; }

        public bool IsScheduled { get; set; } = false;

        public int ProductType { get; set; }
        [NotMapped]
        public ProductType ProductTypes { get; set; }

        [NotMapped]
        public List<ProductImage> Images { get; set; }
        [NotMapped]
        public List<string> ImageUrl { get; set; }
        [NotMapped]
        public List<Category> CategoryTree { get; set; }
        [NotMapped]
        public List<Category> ProductCategories { get; set; }
        [NotMapped]
        public string SelectedCategoryId { get; set; }


        [NotMapped]
        public List<UserReview> UserReviews { get; set; }

        [NotMapped]
        public double DiscountPrice
        {
            get => _discountPrice;
            set => _discountPrice = Math.Max(0, value);
        }

        [NotMapped]
        public OfferDiscount DiscountInformation { get; set; }

        [NotMapped]

        public bool IsFavourite { get; set; }
        [NotMapped]
        public List<Product> ProductList { get; set; }
        [NotMapped]
        public bool IsUpsellProduct { get; set; }
        [NotMapped]
        public bool IsCrosssellProduct { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; } = false;

        [NotMapped]
        public int CommonProductTagId { get; set; }
        [NotMapped]
        public List<int> CommonProductTags { get; set; }

        [NotMapped]


        public MemberShipDiscountCategory MemberShipInformation { get; set; }

        [NotMapped]
        public List<StockLog> StockLogList
        {
            get; set;
        }

        [NotMapped]
        public int? CategoryId { get; set; }
        [NotMapped]
        public int? SubCategoryId { get; set; }
        [NotMapped]
        public string ProductImageUrl { get; set; }
        [NotMapped]
        public List<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

        /// <summary>
        /// ICV Boulevard Score (e.g. "100%") — imported from Excel column "ICV Boulevard Score".
        /// </summary>
        [StringLength(50)]
        public string IcvBoulevardScore { get; set; }

        /// <summary>
        /// Product origin (e.g. "Local", "Imported") — imported from Excel column "Origin".
        /// Used by the Social Impact Tracker.
        /// </summary>
        [StringLength(100)]
        public string Origin { get; set; }

    }
}