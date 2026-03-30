using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Razor.Tokenizer;

namespace Boulevard.Service.WebAPI
{
    public class PushNotificationAccess
    {
        public IUnitOfWork uow;
        public PushNotificationAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<bool> SendInvoiceMemberNotification(int masterId = 0, string title = "", string message = "", string messageTr = "", string tittleTr = "")
        {



            //var masterOrder = db.Invoices.Find(masterId);
            ////var bookingRequest = db.BookingRequests.Find(masterOrder.BookingRequestId);
            //if (masterOrder == null)
            //    return false;
            try
            {
                var memberFirebase = await uow.MemberFireBaseRepository.Get().Where(e => e.MemberId == masterId).FirstOrDefaultAsync();
                //if (await uow.MemberRepository.Get().Where(s => s.MemberId == masterId).Select(s => s.IsNotificationDisable).FirstOrDefaultAsync() == false)
                //{



                    if (memberFirebase != null)
                    {
                        //var notificationData = new NotificationRequest
                        //{
                        //    Title = title,
                        //    DeviceId = memberFirebase.FirebaseToken,
                        //    Message = message,
                        //    TittleTurkish = tittleTr,
                        //    MessageTurkish = messageTr,
                        //};

                        try
                        {
                            //var pushObj = PushNotification.SendPushNotification(notificationData);
                            new SendPushNotificationNewVersion().GenerateFCM_Auth_SendNotifcn(memberFirebase.FirebaseToken,title, message);

                        }
                        catch (Exception ex)
                        {
                            var errorMessage = ex.Message.ToString();
                        }
                    //}



                }
                else
                {
                    return false;
                }
                await SaveNotification(masterId, message, title);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<Notification> SaveNotification(int UserId, String Message, String title)
        {
            //var db = new IraqShoppingDbContext();
            var memberNotification = new Notification();
            //memberNotification.NotificationKey = Guid.NewGuid();
            memberNotification.UserId = UserId;
         
            memberNotification.Message = Message;
            memberNotification.Title = title;

            //
            memberNotification.IsSent = false;
            memberNotification.IsReceived = false;
            memberNotification.IsSeen = false;
            memberNotification.IsReceived = false;
            memberNotification.SentBy = 0;
            memberNotification.ReceivedBy = 0;
            memberNotification.SeenBy = 0;
            memberNotification.SentAt = DateTimeHelper.DubaiTime();
            memberNotification.ReceivedAt = DateTimeHelper.DubaiTime();
            memberNotification.SeenAt = DateTimeHelper.DubaiTime();
            memberNotification.Status = true;
            memberNotification.LastModified = DateTimeHelper.DubaiTime();
           
            //


            await uow.NotificationRepository.Add(memberNotification);

            return memberNotification;
        }
    }
}