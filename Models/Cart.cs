using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string TempId { get; set; }
        public int MemberId { get; set; }

        public int ProductPriceId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public int FeatureCategoryId { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public DateTime LastModified { get; set; }

        [DefaultValue(false)]
        public bool IsDelete { get; set; }
    }
}