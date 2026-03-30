using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ProductTypeMaster
    {
        [Key]
        public int ProductTypeId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string NameAr { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(200)]
        public string DescriptionAr { get; set; }

        [StringLength(100)]
        public string DeliveryTime { get; set; }

        [StringLength(10)]
        public string Status { get; set; } = "Active";

        [StringLength(500)]
        public string Pickup_Building { get; set; }

        [StringLength(500)]
        public string Pickup_Street { get; set; }

        [StringLength(500)]
        public string Pickup_Area { get; set; }

        [StringLength(500)]
        public string Pickup_City { get; set; }


        [StringLength(100)]
        public string Latitute { get; set; }

        [StringLength(100)]
        public string Longitute { get; set; }

        [StringLength(100)]
        public string PickUpContactNo { get; set; }

        [StringLength(100)]
        public string PickUpContactName { get; set; }
    }
}
