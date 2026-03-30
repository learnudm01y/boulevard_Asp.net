using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class OrderRequestDetailsResponse
    {
        public int OrderRequestProductDetailsId { get; set; }

       
        public int OrderRequestProductId { get; set; }
    

       
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImage { get; set; }

        public int Quantity { get; set; }

        public double GrossPrice { get; set; }
    }
}