using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class TempServiceType
    {
        [Key]
        public int Id { get; set; }
        public string SlNo { get; set; }
        public string ServiceName { get; set; }
        public string ServiceTypeName { get; set; }
        public string PersoneQuantity { get;set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public string Images { get; set; }
        public string Price { get;set; }
        public string AdultQuantity { get; set; }
        public string ChildrenQuantity { get; set; }
        public string PaymentType { get; set; }
        public string ServiceHour { get; set; }
        public string ServiceMinutes { get; set; }
        public string FileType { get; set; }
        public string File { get; set; }
        public string AmenitiesName { get;set; }
        public string AmenitiesLogo { get; set; }
        public string AmenitiesType { get; set; }
        public int ExcelCount { get; set; }
        public int FeatureCategoryId { get; set; }
        public string Status { get; set; }

    }
}