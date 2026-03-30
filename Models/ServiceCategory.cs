using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ServiceCategory
    {
        public int ServiceCategoryId { get; set; }
        public virtual Category Category { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(Service))]
        public int? ServiceId { get; set; }
        public virtual Service Service { get; set; }

        [ForeignKey(nameof(ServiceType))]
        public int? ServiceTypeId { get; set; }

        public ServiceType ServiceType { get; set; }


        [StringLength(10)]
        public string Status { get; set; }
        [NotMapped]
        public string FeatureCategoryKey { get; set; }
    }
}