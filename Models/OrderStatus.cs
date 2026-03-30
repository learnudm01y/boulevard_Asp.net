using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boulevard.Models
{
    public class OrderStatus : BaseEntity
    {
        public int OrderStatusId { get; set; }
        public Guid StatusKey { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string PublicName { get; set; }

        [StringLength(250)]
        public string CourierStatusName { get; set; }

        [StringLength(100)]
        public string NameAr { get; set; }

        [StringLength(250)]
        public string PublicNameAr { get; set; }

        public bool IsPublic { get; set; }
        public bool IsInternal { get; set; }
        public int Label { get; set; }



        [NotMapped]
        public bool StatusCondition { get; set; }

        [NotMapped]
        public string statusDate { get; set; }

        [NotMapped]
        public string CurrentStatus { get; set; }



    }
}