using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ProductImage
    {
        [Key]
        public int ProductImageId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        [StringLength(180)]
        public string Image { get; set; }
        public bool IsFeature { get; set; }
    }
}