using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class ServiceLandmarkController : BaseController
    {
        private ServiceAccess _serviceAccess;
        private ServiceLandmarkAccess _serviceLandmarkAccess;

        public ServiceLandmarkController()
        {
            _serviceAccess = new ServiceAccess();
            _serviceLandmarkAccess = new ServiceLandmarkAccess();

        }
        public async Task<ActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key)
        {
            ViewBag.service = new SelectList(await _serviceAccess.GetAll(), "ServiceId", "Name");
            if (Key == null || Key == Guid.Empty)
            {
                ServiceLandmark node = new ServiceLandmark(); 
                return View(node);
            }
            else
            {
                ServiceLandmark node = await _serviceLandmarkAccess.GetByKey(Key.Value);
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(ServiceLandmark model)
        {
            var user = GetUser();
            if(model.ServiceLandmarkKey == null || model.ServiceLandmarkKey == Guid.Empty)
            {
                model.CreateBy = user.UserId;
                await _serviceLandmarkAccess.Insert(model);
            }
            else
            {
                model.UpdateBy = user.UserId;
                await _serviceLandmarkAccess.Update(model);
            }
            return RedirectToAction("Index", "ServiceLandmark");
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
                return await _serviceLandmarkAccess.Delete(Key.Value, 1);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedServiceLandmarks(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var db = new BoulevardDbContext();
                var query = db.ServiceLandmark.AsQueryable();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(sl => sl.Name.ToLower().Contains(term));
                }
                var rows = await query
                    .OrderByDescending(sl => sl.ServiceLandmarkId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(sl => new {
                        landmarkKey = sl.ServiceLandmarkKey,
                        name = sl.Name,
                        serviceName = sl.Service.Name,
                        distance = sl.Distance,
                        latitude = sl.Latitude,
                        longitude = sl.Longitude
                    })
                    .ToListAsync();
                return Json(new { success = true, rows }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedServiceLandmarksCount(string searchTerm)
        {
            try
            {
                var db = new BoulevardDbContext();
                var query = db.ServiceLandmark.AsQueryable();
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(sl => sl.Name.ToLower().Contains(term));
                }
                int total = await query.CountAsync();
                return Json(new { success = true, total }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}