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
    public class RoleController : Controller
    {
        private readonly RoleAccess _roleAccess;
        public RoleController()
        {
            _roleAccess = new RoleAccess();
        }

        public async Task<ActionResult> Index()
        {
            IEnumerable<Role> list = await _roleAccess.GetAll();
            return View(list);
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key)
        {
            if (Key == null || Key == Guid.Empty)
            {
                Role node = new Role();
                return View(node);
            }
            else
            {
                Role node = await _roleAccess.GetByKey(Key.Value);
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(Role model)
        {
            if (model.RoleKey == Guid.Empty)
            {
                await _roleAccess.Insert(model);
            }
            else
            {
                await _roleAccess.Update(model);
            }
            return RedirectToAction("Index", "Role");
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
                return await _roleAccess.Delete(Key.Value, 1);
            }
        }


    }
}