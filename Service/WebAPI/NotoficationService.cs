using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class NotoficationService
    {
        public IUnitOfWork uow;
        public NotoficationService()
        {
            uow = new UnitOfWork();
        }
        public static Notification SaveNotification(int userId, string userType, string message, string title, string tittleTr = "", string messageTr = "")
        {
            var db = new BoulevardDbContext();
            var notification = new Notification();
            notification.UserId = userId;
          
            notification.Message = message;
            notification.Title = title;
           
          
            notification.LastModified = DateTime.Now;

            db.Notifications.Add(notification);
            db.SaveChanges();

            return notification;
        }

        #region Notifications
        public static NotificationResponse NotificationInfo(Notification notification, string lang = "en")
        {
            var dateString = DateTimeHelper.CurrentTimeMillis(notification.LastModified).ToString();
            dateString = dateString.Remove(dateString.Length - 3);
            var time = notification.LastModified.ToShortTimeString();
            return new NotificationResponse
            {
                NotificationId = notification.NotificationId,
                Message = lang == "en" ? notification.Message ?? "" : notification.Title,
                UserId = notification.UserId,
                LastModified = notification.LastModified,
                Status = notification.Status,
                Title = lang == "en" ? notification.Title : notification.Title,
               
                IsSeen = notification.IsSeen,
                date = dateString,
                Time = time,
               


            };
        }
        public static object NotificationsList(int userId, string lang = "en")
        {

            var db = new BoulevardDbContext();
            //var users = db.Members.Any(s => s.MemberId == userId && s.IsEmailNotificationEnable == true);
            //if (users == true)
            //{
            var Notifications = db.Notifications.Where(e => e.UserId == userId && e.Status == true).ToList();
            var notificationList = new List<NotificationResponse>();

            foreach (var notification in Notifications)
            {
                if (notification != null)
                    notificationList.Add(NotificationInfo(notification, lang));
            }



            var ss = notificationList.GroupBy(o => o.LastModified.Date, (key, items) => new
            {
                Date = key.ToString("MM/dd/yyyy"),

                Messages = items.OrderByDescending(o => o.NotificationId).ToList()
            });

            return ss.OrderByDescending(s => s.Date).ToList();
            //}
            //else
            //{
            //    return null;
            //}

        }

        public static object NotificationsListforHeader(int userId, string usertype, string lang = "en")
        {

            var db = new BoulevardDbContext();
            //var users = db.Members.Any(s => s.MemberId == userId && s.IsEmailNotificationEnable == true);
            //if (users == true)
            //{
            var Notifications = db.Notifications.Where(e => e.UserId == userId  && e.Status == true).OrderByDescending(s => s.NotificationId).Take(5).ToList();
            var notificationList = new List<NotificationResponse>();

            foreach (var notification in Notifications)
            {
                if (notification != null)
                    notificationList.Add(NotificationInfo(notification, lang));
            }



            var ss = notificationList.GroupBy(o => o.LastModified.Date, (key, items) => new
            {
                Date = key.ToString("MM/dd/yyyy"),

                Messages = items.OrderBy(o => o.NotificationId).ToList()
            });

            return ss.OrderByDescending(s => s.Date).ToList();
            //}
            //else
            //{
            //    return null;
            //}

        }

        public static int UnseenNotificationCount(int userId, string usertype, string lang = "en")
        {
            try
            {
                var db = new BoulevardDbContext();

                var Notifications = db.Notifications.Where(e => e.UserId == userId  && e.Status == true && e.IsSeen == false).ToList();

                return Notifications.Count();
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public bool NotificationSeen(int userId, string usertype)
        {
            var db = new BoulevardDbContext();

            var member = db.Notifications.Where(e => e.UserId == userId  && e.IsSeen == false).ToList();

            if (member == null)
                return false;

            foreach (var mem in member)
            {
                mem.IsSeen = true;
                mem.SeenBy = userId;
                mem.SeenAt = DateTime.Now;

                uow.NotificationRepository.Edit(mem);
            }

            return true;
        }

        public bool UpdateNotificationSeen(int userId, string usertype, int notificationId = 0)
        {
            var db = new BoulevardDbContext();
            if (notificationId == 0)
            {
                var member = db.Notifications.Where(e => e.UserId == userId  && e.IsSeen == false).ToList();

                if (member == null)
                    return false;
                foreach (var mem in member)
                {
                    mem.IsSeen = true;
                    mem.SeenBy = userId;
                    mem.SeenAt = DateTime.Now;


                    uow.NotificationRepository.Edit(mem);
                    //db.Entry(member).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                }
            }
            else
            {
                var member = db.Notifications.FirstOrDefault(e => e.UserId == userId && e.NotificationId == notificationId );
                if (member == null)
                    return false;

                member.IsSeen = true;
                member.SeenBy = userId;
                member.SeenAt = DateTime.Now;

                db.Entry(member).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
            }

            return true;
        }

        public static bool UpdateNotificationReceived(int userId, int notificationId)
        {
            var db = new BoulevardDbContext();
            var member = db.Notifications.FirstOrDefault(e => e.UserId == userId && e.NotificationId == notificationId );
            if (member == null)
                return false;

            member.IsReceived = true;
            member.ReceivedBy = userId;
            member.ReceivedAt = DateTime.Now;

            db.Entry(member).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return true;
        }




        public bool UpdateNotificationClear(int userId, string usertype, int notificationId = 0)
        {
            var db = new BoulevardDbContext();
            if (notificationId == 0)
            {
                var member = db.Notifications.Where(e => e.UserId == userId  && e.Status == true).ToList();

                if (member == null)
                    return false;
                foreach (var mem in member)
                {
                    mem.Status = false;
                    mem.LastModified = DateTime.Now;


                    uow.NotificationRepository.Edit(mem);
                    //db.Entry(member).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                }
            }
            else
            {
                var member = db.Notifications.FirstOrDefault(e => e.UserId == userId && e.NotificationId == notificationId );
                if (member == null)
                    return false;

                member.IsSeen = true;
                member.SeenBy = userId;
                member.SeenAt = DateTime.Now;

                db.Entry(member).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
            }

            return true;
        }

        #endregion





        public static NotificationResponse GetNotificationbyId(int notificationId)
        {
            SeenNotification(notificationId);

            var db = new BoulevardDbContext();

            var notification = db.Notifications.Where(s => s.NotificationId == notificationId).FirstOrDefault();

            NotificationResponse notificationResponse = new NotificationResponse();

            notificationResponse.NotificationId = notification.NotificationId;
            notificationResponse.Title = notification.Title;
            notificationResponse.Message = notification.Message;
            notificationResponse.LastModified = notification.LastModified;

            return notificationResponse;
        }

        static void SeenNotification(int NotificationId)
        {
            var db = new BoulevardDbContext();

            var notification = db.Notifications.Where(s => s.NotificationId == NotificationId).FirstOrDefault();

            notification.IsSeen = true;
            notification.SeenAt = DateTime.UtcNow;

            db.Entry(notification).State = EntityState.Modified;
            db.SaveChanges();
        }

        public bool SendNotificationRequest(NotificationRequest notificationRequest, string checkBoxMemberString, string usertype = "")
        {
            try
            {
                List<int> list = new List<int>();

                if (checkBoxMemberString.Length > 0)
                {
                    checkBoxMemberString = checkBoxMemberString + ',';
                }

                list = RetriveSubCategoryId(checkBoxMemberString.Trim());

                foreach (var _listId in list)
                {


                    string title = notificationRequest.Title;
                    string message = notificationRequest.Message;

                    new PushNotificationAccess().SendInvoiceMemberNotification(_listId, title, message);
                }

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public List<int> RetriveSubCategoryId(string checkBoxCategoryString)
        {
            List<int> subCategoryList = new List<int>();

            while (checkBoxCategoryString.Length > 0)
            {
                string str = checkBoxCategoryString.Substring(0, checkBoxCategoryString.IndexOf(",")).Trim();
                checkBoxCategoryString = checkBoxCategoryString.Replace(str + ',', null).Trim();
                subCategoryList.Add(Convert.ToInt32(str.Trim()));
            }

            return subCategoryList;
        }
    }
}