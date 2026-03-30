using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Role : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }
        public Guid RoleKey { get; set; }
        [StringLength(200)]
        public string RoleName { get; set; }
       
    }
}