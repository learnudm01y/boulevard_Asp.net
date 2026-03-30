using Boulevard.App_Start;
using Boulevard.Areas.Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base

        public LoginDetailsViewModel GetUser()
        {
            var model = new LoginDetailsViewModel();
            model.UserId = (User as CustomPrincipal).UserId;
            model.Name = (User as CustomPrincipal).Name;
            model.Email = (User as CustomPrincipal).Email;
            model.Mobile = (User as CustomPrincipal).Mobile;
            model.Image = (User as CustomPrincipal).Image;
            model.RoleId = (User as CustomPrincipal).RoleId;
            return model;
        }


        public ActionResult ChangePageSizeCookieValue(int Page_Size = 10)
        {
            Response.Cookies["Page_Size"].Value = Page_Size.ToString();
            Response.Cookies["Page_Size"].Expires = DateTime.Now.AddDays(30);
            return Json(1, JsonRequestBehavior.AllowGet);
        }

    }
}