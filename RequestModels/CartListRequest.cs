using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class CartListRequest
    {
       
        public int ProductId { get; set; }
     
        public int MemberId { get; set; }

        public string TempId { get; set; }
        public int FeatureCategoryId { get; set; }

        public int ProductPriceId { get; set; }

        public int Quantity { get; set; } = 0;
    }
}