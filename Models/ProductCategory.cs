using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ProductCategory
    {
        public int ProductCategoryId { get; set; }
        public virtual Category Category { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        [StringLength(10)]
        public string Status { get; set; }
    }
}