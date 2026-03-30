using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class UserReport
    {
        public int UserReportId { get; set; }
        public int MemberId { get; set; }

        public string Title { get; set; }
        public string Comments { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastUpdate { get; set; }

        [NotMapped]
        public Member Member { get; set; }

        //[NotMapped]
        //public Member ReportMember { get; set; }

        //[NotMapped]
        //public PostComment ForumPost { get; set; }

        [NotMapped]
        public bool IsGiveResponse { get; set; }
        [NotMapped]
        public string Response { get; set; }
        [NotMapped]
        public string MemberName { get; set; }
    }
}