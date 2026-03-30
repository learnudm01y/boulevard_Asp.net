using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class MembershipController : BaseController
    {
        public MembershipService _membershipService;
        public MembershipController()
        {
            _membershipService = new MembershipService();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetSubscription(string lang="en")
        {
            var result = await _membershipService.getMemberShipDetails(lang);
          
                return SuccessMessage(result);
            
           
        }

        public async Task<IHttpActionResult> CreateSubscription(int memberId, int membershipId)
        {
            var result = await _membershipService.CreateMembershipSubscription( memberId, membershipId);
            if (result == true)
            {
                return SuccessMessage(result, "Thank you for Your subscription");
            }
            else
            {
                return ErrorMessageNull("You are already Subscribed",null);
            }


        }
    }
}
