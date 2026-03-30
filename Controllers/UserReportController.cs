using Boulevard.Models;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Boulevard.Controllers
{
    public class UserReportController : BaseController
    {
        private UserReportService _userreportService;
        public UserReportController()
        {
            _userreportService = new UserReportService();
        }

        public async Task<IHttpActionResult> SaveQuestion(UserReport qst)
        {
            try
            {
                var result = await _userreportService.SaveUserReport(qst);
                return Ok(new
                {
                    Data = new
                    {

                    },
                    code = HttpStatusCode.OK,
                    message = "Success",
                    isSuccess = result
                });
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public async Task<IHttpActionResult> GetUserReportresponse(int memberId)
        {
            try
            {
                var result = await _userreportService.GetUserReport(memberId);
                return Ok(new
                {
                    Data = new
                    {
                        result
                    },
                    code = HttpStatusCode.OK,
                    message = "Success",
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}