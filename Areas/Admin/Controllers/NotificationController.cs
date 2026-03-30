using Boulevard.Models;
using Boulevard.Service.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Admin/Notification
        public async Task<ActionResult> Index()
        {
            //var users = GetUserSession();

            var notifications = await new AdminNotificationDataAccess().GetAdminNotificationList();
            return View(notifications);
            //return View(db.Cities.ToList());
        }

        [HttpGet]
        public async Task<ActionResult> GetAdminMessages()
        {


            var notifications = await new AdminNotificationDataAccess().GetAdminNotification(10);
            if (notifications != null && notifications.Count > 0)
            {

                return PartialView("PartialViews/NotificationView", notifications);

            }
            else
            {
                return PartialView("PartialViews/Message", new List<AdminNotification>());
            }
        }
    }
}