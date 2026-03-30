using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ServiceTypeFile
    {
        [Key]
        public int ServiceTypeFileId { get; set; }
        [ForeignKey("ServiceType")]
        public int? ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; }

        [StringLength(180)]
        public string FileSource { get; set; }
        [StringLength(180)]
        public string FileType { get; set; }
        [StringLength(180)]
        public string FileLocation { get; set; }

        public int ServiceAmenityId { get; set; }


        public DateTime LastUpdate { get; set; }

    }
}