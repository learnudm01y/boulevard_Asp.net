using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class AirportService : BaseEntity
    {
        [Key]
        public int AirportServiceId { get; set; }

        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }

        [ForeignKey(nameof(Airport))]
        public int AirportId { get; set; }
        public virtual Airport Airport { get; set; }


    }
}