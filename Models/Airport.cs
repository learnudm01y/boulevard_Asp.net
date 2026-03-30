using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Airport : BaseEntity
    {
        [Key]
        public int AirportId { get; set; }
        public Guid AirportKey { get; set; }
        public string AirportCode { get; set; }
        public string AirportName { get; set; }

        public string AirportNameAr { get; set; }
        public string Details { get; set; }

        public int CountryId { get; set; }

        [ForeignKey(nameof(City))]
        public int CityId { get; set; }
        public virtual City City { get; set; }
        [NotMapped]

        public bool IsSelected { get; set; } = false;

    }
}
