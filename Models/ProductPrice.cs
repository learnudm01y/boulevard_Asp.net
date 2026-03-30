using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ProductPrice
    {

        public int ProductPriceId { get; set; }

        public int ProductId { get; set; }

        public double ProductQuantity { get; set; }

        public double Price { get; set; }

        public int ProductStock { get; set; }

        public string Status { get; set; }

        public DateTime LastUpdateDate { get; set; }

        [NotMapped]
        public double? Discount { get; set; }


    }
}