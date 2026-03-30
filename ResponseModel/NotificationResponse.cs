using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class NotificationResponse
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

        public string date { get; set; }
        public bool Status { get; set; }
        public DateTime LastModified { get; set; }
        public string Time { get; set; }
    }
}