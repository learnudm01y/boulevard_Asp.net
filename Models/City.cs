using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class City : BaseEntity
    {
        [Key]
        public int CityId { get; set; }
        public Guid CityKey { get; set; }
        public string CityName { get; set; }
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        public string CityNameAr { get; set; }
    }
}