using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class StockLog
    {
        public int StockLogId { get; set; }
        public Guid StockKey { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public DateTime? StockDate { get; set; }
        public int StockIn {  get; set; }
        public int StockOut { get; set; }
        public DateTime? CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public string StockType { get; set; }
        public int OrderMasterId { get; set; }

        public int? ProductPriceId { get; set; }
        public string UserType { get; set; }
        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }
        [NotMapped]
        public virtual ProductPrice ProductPrice { get; set; }
    }
}