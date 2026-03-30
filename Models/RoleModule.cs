using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Boulevard.Models
{
    public class RoleModule : BaseEntity
    {
        public RoleModule()
        {
            this.Modules = new List<Module>();
        }
        [Key]
        public int RoleModuleId { get; set; }
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        [ForeignKey(nameof(Module))]
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }     
        [NotMapped]
        public virtual List<Module> Modules { get; set; }
    }
}