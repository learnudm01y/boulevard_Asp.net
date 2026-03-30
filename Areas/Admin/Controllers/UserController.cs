using Boulevard.Models;
using Boulevard.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly LayoutSettingAccess _layoutSettingAccess;
        private readonly UserAccess _userAccess;
        private readonly RoleAccess _roleAccess;

        public UserController()
        {
            _userAccess = new UserAccess();
            _roleAccess = new RoleAccess();
            _layoutSettingAccess = new LayoutSettingAccess();
        }

        public async Task<ActionResult> Index()
        {
            IEnumerable<User> list = await _userAccess.GetAll();
            return View(list);
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key)
        {
            ViewBag.roles = new SelectList(await _roleAccess.GetAll(), "RoleId", "RoleName");
            if (Key == null || Key == Guid.Empty)
            {
                User node = new User();
                return View(node);
            }
            else
            {
                User node = await _userAccess.GetByKey(Key.Value);
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(User model,HttpPostedFileBase image)
        {
            if (image != null)
            {
                  model.Image=_userAccess.UploadImage(image);
            }
            if (model.UserKey == Guid.Empty)
            {
                await _userAccess.Insert(model);
            }
            else
            {
                await _userAccess.Update(model);
            }
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        public async Task<bool> Delete(Guid? Key)
        {
            if (Key == null || Key == Guid.Empty)
            {
                return false;
            }
            else
            {
                return await _userAccess.Delete(Key.Value, 1);
            }
        }

        public async Task<JsonResult> MoodChange()
        {
            _layoutSettingAccess.layoutUpdate();
            return Json("Successfull", JsonRequestBehavior.AllowGet);
        }
    }
}