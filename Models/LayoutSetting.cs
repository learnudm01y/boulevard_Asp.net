using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class LayoutSetting
    {
        [Key]
        public int layoutSetting { get; set; }
        [StringLength(10)]
        public string Name { get; set; }
        [StringLength(10)]
        public string LogoHeader { get; set; }
        [StringLength(10)]
        public string MainHeader { get; set; }
        [StringLength(10)]
        public string Body { get; set; }
        [StringLength(10)]
        public string SideBar { get; set; }
        public bool IsDefault { get; set; }
    }
}