using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class ProjectPlan:BaseEntity
    {
        [Key]
        public int ProjectPlanId { get; set; }

        [ForeignKey("ServiceType")]
        public int? ServiceTypeId { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        [StringLength(500)]

        public string Title { get; set; }

        [StringLength(500)]

        public string UniteType { get; set; }


        [StringLength(500)]

        public string Image { get; set; }

   

       
    }
}