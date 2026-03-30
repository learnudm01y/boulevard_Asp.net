using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ServiceLandmark : BaseEntity
    {
        [Key]
        public int ServiceLandmarkId { get; set; }
        public Guid ServiceLandmarkKey { get; set; }
        public string Name{ get; set; }
        public string NameAr { get; set; }
        public string Address{ get; set; }
        public double Distance{ get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }

    }
}