using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class AdminNotification
    {
        public int AdminNotificationId { get; set; }

        public int UserId { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        public string Message { get; set; }
        [StringLength(100)]
        public string FeatureType { get; set; }
        [StringLength(100)]
        public string UserType { get; set; }
        public bool IsSent { get; set; }
        public bool IsReceived { get; set; }
        public bool IsSeen { get; set; }
        public int SentBy { get; set; }
        public int ReceivedBy { get; set; }
        public int SeenBy { get; set; }

        public DateTime? SentAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime? SeenAt { get; set; }
        public bool Status { get; set; }
        public DateTime LastModified { get; set; }

   



        public int SellerId { get; set; }

        public int? OrderId { get; set; }

        [NotMapped]

        public string UserName { get; set; }

        [NotMapped]

        public string Email { get; set; }


        [NotMapped]

        public string PhoneNo { get; set; }
    }
}