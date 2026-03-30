using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class CustomerEnqueryController : Controller
    {
        private readonly CustomerEnqueryDataAccess _customerEnqueryDataAccess;
        public CustomerEnqueryController()
        {
            _customerEnqueryDataAccess = new CustomerEnqueryDataAccess();
        }
        // GET: Admin/CustomerEnquery
        public async Task<ActionResult> Index()
        {
            try
            {
                var list = await _customerEnqueryDataAccess.GetAll();
                return View(list);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
    }
}