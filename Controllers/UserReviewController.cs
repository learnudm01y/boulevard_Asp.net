using Boulevard.RequestModels;
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
    public class UserReviewController : BaseController
    {
        public UserReviewServiceAccess _userReviewService;
        public UserReviewController()
        {
            _userReviewService = new UserReviewServiceAccess();
        }
        // GET: WebHtml
        [HttpPost]
        public async Task<IHttpActionResult> UserReviewAdd(UserReviewRequest request)
        {
            var result = await _userReviewService.AddUserReview(request);
            if (result == true)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("Some problem Occurs",result);
            }
        }
    }
}
