using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class NotificationController : ApiController
    {
        [HttpGet]
        public IHttpActionResult NotificationsGet(int userId,  string lang = "en")
        {
            //var member = MemberDataAccess.IsMemberExist(userId);
            //if (member == null)
            //    return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.NotFound, message = "Member not found!", isSuccess = false });


            return Content(HttpStatusCode.OK, new
            {
                data = new
                {
                    notifications = NotoficationService.NotificationsList(userId, lang)
                },
                code = HttpStatusCode.OK,
                message = "success",
                isSuccess = true
            });
        }

        [HttpGet]
        public IHttpActionResult NotificationsSeen(int userId, string userType, int notificationId)
        {
            //var member = MemberDataAccess.IsMemberExist(userId);
            //if (member == null)
            //    return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.NotFound, message = "Member not found!", isSuccess = false });


            return Content(HttpStatusCode.OK, new
            {
                data = new
                {
                    notifications = new NotoficationService().UpdateNotificationSeen(userId, userType, notificationId)
                },
                code = HttpStatusCode.OK,
                message = "success",
                isSuccess = true
            });
        }


        [HttpGet]
        public IHttpActionResult NotificationsClear(int userId, string userType, int notificationId)
        {
            //var member = MemberDataAccess.IsMemberExist(userId);
            //if (member == null)
            //    return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.NotFound, message = "Member not found!", isSuccess = false });


            return Content(HttpStatusCode.OK, new
            {
                data = new
                {
                    notifications = new NotoficationService().UpdateNotificationClear(userId, userType, notificationId)
                },
                code = HttpStatusCode.OK,
                message = "success",
                isSuccess = true
            });
        }

        [HttpGet]
        public IHttpActionResult NotificationsReceived(int userId, int notificationId)
        {
            //var member = MemberDataAccess.IsMemberExist(userId);
            //if (member == null)
            //    return Content(HttpStatusCode.OK, new { data = new { }, code = HttpStatusCode.NotFound, message = "Member not found!", isSuccess = false });


            return Content(HttpStatusCode.OK, new
            {
                data = new
                {
                    notifications = NotoficationService.UpdateNotificationReceived(userId, notificationId)
                },
                code = HttpStatusCode.OK,
                message = "success",
                isSuccess = true
            });
        }
    }
}
