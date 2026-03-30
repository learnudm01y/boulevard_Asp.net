using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Module
    {
        public Module()
        {
            this.ChildModule = new List<Module>();
        }
        [Key]
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        [StringLength(10)]
        public string Status { get; set; }
        public int? ParentId { get; set; }
        public int PositionId { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        [NotMapped]
        public bool ActiveRole { get; set; }
        [NotMapped]
        public virtual List<Module> ChildModule { get; set; }
        [StringLength(10)]
        public string ModuleCode { get; set; }
        public string ModuleNameAr { get; set; }
    }
}