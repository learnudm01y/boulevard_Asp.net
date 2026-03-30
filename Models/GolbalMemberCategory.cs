using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class GolbalMemberCategory
    {
        [Key]
        public long Id { get; set; }
        public long MemberId { get; set; }
        public int FeatureCategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}