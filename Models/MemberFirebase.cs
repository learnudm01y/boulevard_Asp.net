using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class MemberFirebase
    {
        public int MemberFirebaseId { get; set; }

        public int MemberId { get; set; }


        [StringLength(250)]
        public string FirebaseToken { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}