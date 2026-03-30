using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Boulevard.Areas.Admin.Data;

namespace Boulevard.Models
{
    public class User : BaseEntity
    {
        public User()
        {
            this.UserActivityTimeLines = new List<UserActivityTimeLine>();
        }
        [Key]
        public int UserId { get; set; }
        public Guid UserKey { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Email { get; set; }
        [StringLength(30)]
        public string PhoneNumber { get; set; }
        [StringLength(180)]
        public string Image { get; set; }
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public int? LayoutId { get; set; }
        [StringLength(100)]
        public string UserName { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public List<UserActivityTimeLine> UserActivityTimeLines { get; set; }
        [StringLength(255)]
        public string OTP { get; set; }
    }
}