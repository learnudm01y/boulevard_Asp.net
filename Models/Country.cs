using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Country : BaseEntity
    {
        public int CountryId { get; set; }
        public Guid CountryKey { get; set; }
        public string CountryName { get; set; }
        public string CountryNameAr { get; set; }
    }
}