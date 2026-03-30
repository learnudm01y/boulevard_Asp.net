using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class DeliverySetting
    {
        public int DeliverySettingId { get; set; }  

        public Guid SettingKey { get; set; }

        public int DeliveryCharge { get; set; }

        public double ChargeForFree { get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        [ForeignKey(nameof(FeatureCategory))]
        public int FeatureCategoryId { get; set; }
        public virtual FeatureCategory FeatureCategory { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}