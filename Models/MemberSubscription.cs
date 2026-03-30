using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class MemberSubscription
    {
        public int MemberSubscriptionId { get; set; }

        [ForeignKey(nameof(MemberShip))]
        public int MemberShipId { get; set; }

        public MemberShip MemberShip { get; set; }
   


        public int MemberId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}