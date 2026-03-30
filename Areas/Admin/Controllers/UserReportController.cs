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
    public class UserReportController : Controller
    {
        private readonly UserReportDataAccess _userReportDataAccess;
        public UserReportController()
        {
            _userReportDataAccess = new UserReportDataAccess();
        }
        // GET: Admin/UserReport
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetReportandhelp()
        {
            try
            {
                var utilities = await _userReportDataAccess.GetuserReport();

                if (utilities.Count > 0)
                {
                    return View(utilities);
                }
                else
                {
                    return View(new List<UserReport>());
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<ActionResult> ReportandhelpDetails(int reportId)
        {
            try
            {
                var utilities = await _userReportDataAccess.GetuserReportById(reportId);

                if (utilities != null)
                {
                    return View(utilities);
                }
                else
                {
                    return View(new UserReport());
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public async Task<JsonResult> PostResponse(int userReportId, string responce)
        {
            try
            {
                await _userReportDataAccess.PostResponse(userReportId, responce);

                return Json(new { success = true, responseText = "Successfull" });
            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Failed" });
            }
        }
    }
}
