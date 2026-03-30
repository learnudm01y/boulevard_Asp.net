using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class UserActivity
    {
        [Key]
        public long UserActivityId { get; set; }
        public int UserId { get; set; }
        [StringLength(200)]
        public string Details { get; set; }
        [StringLength(100)]
        public string Action { get; set; }
        [StringLength(300)]
        public string Url { get; set; }
        [StringLength(20)]
        public string Ip { get; set; }
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public string Time { get; set; }
    }
}