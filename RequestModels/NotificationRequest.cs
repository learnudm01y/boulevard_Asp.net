using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.RequestModels
{
    public class NotificationRequest
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}