using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

  
        public int UserId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }


       

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
    }
}