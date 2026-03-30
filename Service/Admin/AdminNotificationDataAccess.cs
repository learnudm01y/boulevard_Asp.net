using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.Admin
{
    public class AdminNotificationDataAccess
    {

        public IUnitOfWork uow;

        public AdminNotificationDataAccess()
        {
            uow = new UnitOfWork();
        }
        public  async Task<AdminNotification> SaveNotification(int userId, String Message, String title, string UserType,string featureType="", int? orderId = null)
        {
            try
            {
                
                var admninNotification = new AdminNotification();
                admninNotification.UserId = userId;
                admninNotification.Message = Message;
                admninNotification.Title = title;
                admninNotification.LastModified = DateTimeHelper.DubaiTime();
                admninNotification.UserType = UserType;
                admninNotification.FeatureType = featureType;
                admninNotification.OrderId = orderId;
                await uow.AdminNotificationRepository.Add(admninNotification);

                return admninNotification;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<List<AdminNotification>> GetAdminNotification(int pagesize = 10)
        {
          
            var memberNotification = new List<AdminNotification>();
            
                memberNotification = await uow.AdminNotificationRepository.Get().Take(pagesize).OrderByDescending(s => s.AdminNotificationId).ToListAsync();
            

            return memberNotification;
        }


        public List<AdminNotification> GetAdminNotificationwithout(int pagesize = 10)
        {
            string cacheKey = "AdminNotifications_" + pagesize;
            var cached = HttpRuntime.Cache[cacheKey] as List<AdminNotification>;
            if (cached != null) return cached;

            var memberNotification = uow.AdminNotificationRepository.Get()
                .OrderByDescending(s => s.AdminNotificationId)
                .Take(pagesize)
                .ToList();

            HttpRuntime.Cache.Insert(cacheKey, memberNotification, null,
                DateTime.Now.AddSeconds(30), System.Web.Caching.Cache.NoSlidingExpiration);
            return memberNotification;
        }


        public async Task UpdateSeenAdminNotification()
        {
            try
            {
                

                var result = new List<AdminNotification>();
                
                    result = await uow.AdminNotificationRepository.Get().Where(s => s.IsSeen == false).ToListAsync();
                

                if (result != null && result.Count > 0)
                {
                    foreach (var res in result)
                    {
                        res.IsSeen = true;
                        await uow.AdminNotificationRepository.Edit(res);
                    }
                }


            }
            catch (Exception ex)
            {


            }


        }

        public async Task<List<AdminNotification>> GetAdminNotificationList()
        {
            
            var memberNotification = new List<AdminNotification>();
           
                memberNotification = await uow.AdminNotificationRepository.Get().Take(100).OrderByDescending(s => s.AdminNotificationId).ToListAsync();
            
            if (memberNotification != null && memberNotification.Count > 0)
            {
                foreach (var not in memberNotification)
                {
                    if (not.UserType == "Member")
                    {
                        var member = await uow.MemberRepository.Get().Where(s => s.MemberId == not.UserId).FirstOrDefaultAsync();
                        if (member != null)
                        {
                            not.UserName = member.Name;
                            not.Email = member.Email;
                            not.PhoneNo = member.PhoneNumber;
                        }
                    }
                    //else if (not.UserType == "Driver")
                    //{
                    //    var driver = db.Drivers.Where(s => s.DriverId == not.UserId).FirstOrDefault();
                    //    if (driver != null)
                    //    {
                    //        not.UserName = driver.FirstName + " " + driver.LastName;
                    //        not.Email = driver.Email;
                    //        not.PhoneNo = driver.PhoneNumber;
                    //    }
                    //}
                    //else if (not.UserType == "Delivery Company")
                    //{

                    //    var deliveryComp = db.DeliveryCompanies.Where(s => s.DeliveryCompaniesId == not.UserId).FirstOrDefault();
                    //    if (deliveryComp != null)
                    //    {
                    //        not.UserName = deliveryComp.Name;
                    //        not.Email = deliveryComp.Email;
                    //        not.PhoneNo = deliveryComp.PhoneNumber;
                    //    }
                    //}
                    //else if (not.UserType == "Business account")
                    //{
                    //    var deliveryComp = db.Company.Where(s => s.CompanyId == not.UserId).FirstOrDefault();
                    //    if (deliveryComp != null)
                    //    {
                    //        not.UserName = deliveryComp.FirstName + " " + deliveryComp.LasttName;
                    //        not.Email = deliveryComp.Email;
                    //        not.PhoneNo = deliveryComp.PhoneNumber;
                    //    }
                    //}

                }
            }
            return memberNotification;
        }
    }
}