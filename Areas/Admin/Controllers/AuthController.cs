using Boulevard.Areas.Admin.Data;
using Boulevard.Contexts;
using Boulevard.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace Boulevard.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserAccess _userAccess;
        private readonly BoulevardDbContext db;
        public AuthController()
        {
            _userAccess = new UserAccess();
            db = new BoulevardDbContext();
        }

        // GET: Admin/Auth
        public async Task<ActionResult> Login()
        {
            TempData["ErrorMessage"] = "";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginView login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userAccess.GetUserByAuth(login.UserName, login.Password);
                if (user != null)
                {
                    LoginDetailsViewModel userModel = new LoginDetailsViewModel()
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        Email = user.Email == null ? "" : user.Email,
                        Mobile = user.PhoneNumber == null ? "" : user.PhoneNumber,
                        Image = user.Image,
                        RoleId = user.RoleId,
                    };
                    string userData = JsonConvert.SerializeObject(userModel);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.PhoneNumber, DateTime.Now, DateTime.Now.AddDays(15), false, userData);
                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "UserName & Password Is Incorrect.";
                    return View(login);
                }

            }
            return View(login);
        }

        public async Task<ActionResult> LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Auth");
        }

    }
}