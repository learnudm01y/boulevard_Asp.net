using Boulevard.Contexts;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Boulevard.Service.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Boulevard.Areas.Admin.Controllers
{
    public class FaqServiceController : Controller
    {
        private readonly FaqServiceDataAccess _faqServiceDataAccess;
        private readonly FeatureCategoryAccess _featureCategoryAccess;
        private readonly Service.ServiceAccess _serviceAccess;
        public FaqServiceController()
        {
            _faqServiceDataAccess = new FaqServiceDataAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _serviceAccess = new Service.ServiceAccess();
        }
        // GET: Admin/FaqService
        public async Task<ActionResult> IndexForAll()
        {
            try
            {
                var list = await _faqServiceDataAccess.GetAll();
                if (list.Count > 0)
                {
                    return View(list);
                }
                else
                {
                    return View(new List<FaqService>());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<ActionResult> Index(string fCatagoryKey)
        {
            try
            {
                ViewBag.FCatagoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key, string fCatagoryKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                        var fCatagory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
                        if (!string.IsNullOrEmpty(fCatagoryKey))
                        {
                            ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                        }
                        ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagory.FeatureCategoryKey.ToString()), "ServiceId", "Name");
                    }
                    else
                    {
                        var oldData = await _faqServiceDataAccess.GetAllByFaqServiceKey(key);
                        oldData.ServiceId = oldData.FeatureTypeId;
                        ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                        if (!string.IsNullOrEmpty(fCatagoryKey))
                        {
                            ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                        }
                        var fCatagory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
                        ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagory.FeatureCategoryKey.ToString()), "ServiceId", "Name");
                    }
                }
                else
                {
                    ViewBag.service = new SelectList(await _serviceAccess.GetAll(), "ServiceId", "Name");
                }
                FaqService data = new FaqService();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _faqServiceDataAccess.GetAllByFaqServiceKey(key);
                    //if (data != null)
                    //{
                    //    data.ServiceId = data.FeatureTypeId;
                    //    var service = await _serviceAccess.GetById(data.FeatureTypeId);
                    //    if (service != null)
                    //    {
                    //        var fCatagory = await _featureCategoryAccess.GetById(service.FeatureCategoryId);
                    //        if (fCatagory != null)
                    //        {
                    //            ViewBag.FeatureCategoryKey = fCatagory.FeatureCategoryKey;
                    //            ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagory.FeatureCategoryKey.ToString()), "ServiceId", "Name");
                    //        }
                    //    }
                    //}
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAndUpdate(FaqService model)
        {
            try
            {
                if (model != null)
                {
                    if (model.FaqServiceId == 0)
                    {
                        await _faqServiceDataAccess.Create(model);
                    }
                    else
                    {
                        await _faqServiceDataAccess.Update(model);
                    }
                }

                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    return RedirectToAction("Index", "FaqService", new { fCatagoryKey = model.FeatureCategoryKey });
                }
                else
                {
                    return RedirectToAction("Index", "FaqService");
                }

                //return RedirectToAction("Index", "FaqService");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndUpdateForAll(string faqkey)
        {
            try
            {
                FaqService data = new FaqService();
                if (string.IsNullOrEmpty(faqkey))
                {
                    return View(data);
                }
                else
                {
                    data = await _faqServiceDataAccess.GetAllByFaqServiceKey(faqkey);
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAndUpdateForAll(FaqService model)
        {
            try
            {
                if (model != null)
                {
                    if (model.FaqServiceId == 0)
                    {
                        await _faqServiceDataAccess.CreateForAll(model);
                    }
                    else
                    {
                        await _faqServiceDataAccess.UpdateForAll(model);
                    }
                }

                return RedirectToAction("IndexForAll", "FaqService");

                //return RedirectToAction("Index", "FaqService");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<bool> Delete(string Key)
        {
            if (string.IsNullOrEmpty(Key))
            {
                return false;
            }
            else
            {
                return await _faqServiceDataAccess.Delete(Key, 1);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedFaqServices(string fCatagoryKey, string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var db = new BoulevardDbContext();
                var query = db.FaqServices.AsQueryable();
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await db.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                    {
                        var serviceIds = db.Services.Where(s => s.FeatureCategoryId == fcId).Select(s => s.ServiceId).ToList();
                        query = query.Where(f => serviceIds.Contains(f.FeatureTypeId));
                    }
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(f => f.FaqTitle.ToLower().Contains(term));
                }
                var rows = await query
                    .OrderByDescending(f => f.FaqServiceId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(f => new {
                        faqKey = f.FAQKey,
                        faqTitle = f.FaqTitle,
                        faqDescription = f.FaqDescription,
                        status = f.Status
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
        public async Task<ActionResult> GetPagedFaqServicesCount(string fCatagoryKey, string searchTerm)
        {
            try
            {
                var db = new BoulevardDbContext();
                var query = db.FaqServices.AsQueryable();
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await db.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                    {
                        var serviceIds = db.Services.Where(s => s.FeatureCategoryId == fcId).Select(s => s.ServiceId).ToList();
                        query = query.Where(f => serviceIds.Contains(f.FeatureTypeId));
                    }
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(f => f.FaqTitle.ToLower().Contains(term));
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