using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class VehicalModel:BaseEntity
    {
        public int VehicalModelId { get; set; }
        public Guid VehicalModelKey { get; set; }
        [StringLength(250)]
        public string VehicalModelName { get; set; }

        public string VehicalModelNameAr { get; set; }

        public string ModelDetails { get; set; }

        public string ModelDetailsAr { get; set; }

        [ForeignKey(nameof(Brand))]
        public int? BrandId { get; set; }
       
        public virtual Brand Brand { get; set; }
    }
}