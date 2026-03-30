using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ServiceImage:BaseEntity
    {
        [Key]
        public int ServiceImageId { get; set; }
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
        [StringLength(180)]
        public string Image { get; set; }
        public bool IsFeature { get; set; }
    }
}