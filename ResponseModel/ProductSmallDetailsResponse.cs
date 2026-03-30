using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class ProductSmallDetailsResponse
    {

        private decimal _productPrice;
        private double _discountPrice;
        public int ProductId { get; set; }
       
        public string ProductName { get; set; }

        public string Image { get; set; }

      

        public decimal ProductPrice
        {
            get => _productPrice;
            set => _productPrice = Math.Max(0, value);
        }

        public double DiscountPrice
        {
            get => _discountPrice;
            set => _discountPrice = Math.Max(0, value);
        }

        public int Quantity { get; set; }

       

        public bool IsFavourite { get; set; }
        public string Barcode { get; set; }
        public int StockQuantity { get; set; }

        public Brand BrandInfo { get; set; }

        public OfferDiscount DiscountInformation { get; set; }

        public MemberShipDiscountCategory MemberShipInformation { get; set; }

        public bool IsOfferDiscount { get; set; }

        public OfferInformation OfferInformation { get; set; }

        public MemberShip MembershipInformation { get; set; }

        public bool IsMembershipOrder { get; set; }

        [StringLength(100)]
        public string DiscountType { get; set; }

        public double DiscountAmount { get; set; }
        public bool IsScheduled { get; set; } = false;

        public string AttributeName { get; set; }
        [NotMapped]
        public List<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();


        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }

        public double AvrageRatings { get; set; }

        public double TotalReview { get; set; }

    }
}