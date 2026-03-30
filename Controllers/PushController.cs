using Boulevard.Helper;
using Boulevard.Service.Admin;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class PushController: BaseController
    {
        public async Task<IHttpActionResult> PushSend(int memberId)
        {
            string M_Title = "Company Profile";

            string M_Message = "Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.Please verify your email.";


            var result = await new PushNotificationAccess().SendInvoiceMemberNotification(memberId, M_Title, M_Message);
            if (result == true)
            {
                return SuccessMessage(result);
            }
            else
            {
                return InternelServerError();
            }

        }


        public async Task<IHttpActionResult> PushSendemail()
        {
           


            await new EmailService().Sendemail();
           
                return SuccessMessage("");
            

        }

        [HttpGet]
        public async Task<IHttpActionResult> SeenAdminNotification()
        {
            await new AdminNotificationDataAccess().UpdateSeenAdminNotification();
            return Content(HttpStatusCode.OK, new
            {
                data = true,
                code = HttpStatusCode.OK,
                message = "success",
                isSuccess = true
            });

        }
    }
}