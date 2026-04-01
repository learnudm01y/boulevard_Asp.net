using Boulevard.Contexts;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class CommonProductTagController : Controller
    {
        private readonly CommonProductTagDataAcces _commonProductTagDataAcces;
        private FeatureCategoryAccess _featureCategoryAccess;

        public CommonProductTagController()
        {
            _commonProductTagDataAcces = new CommonProductTagDataAcces();
            _featureCategoryAccess = new FeatureCategoryAccess();
        }
        // GET: Admin/CommonProductTag
        public async Task<ActionResult> Index(string fCatagoryKey)
        {
            try
            {
                var list = new List<CommonProductTag>();
                ViewBag.FCatagoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                    var FCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
                    if (FCategory != null)
                        ViewBag.FCategoryId = FCategory.FeatureCategoryId;
                }
                list = await _commonProductTagDataAcces.GetAllByFCatagoryKey(fCatagoryKey);
                return View(list);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return View(new List<CommonProductTag>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCommonProductTag(string tagName, string fCatagoryKey)
        {
            var db = new BoulevardDbContext();
            var model = new CommonProductTag();
            var fCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
            model.TagName = tagName;
            model.FeatureCategoryId = fCategory.FeatureCategoryId;
            model.CreateDate = DateTime.Now;
            model.CreateBy = 1;
            model.Status = "Active";
            db.CommonProductTags.Add(model);
            db.SaveChanges();
            return RedirectToAction("Index", "CommonProductTag", new { fCatagoryKey = fCategory.FeatureCategoryKey });
        }

        [HttpGet]
        public async Task<bool> Delete(int id)
        {
            if (id < 0)
            {
                return false;
            }
            else
            {
                return await _commonProductTagDataAcces.Delete(id);
            }
        }
    }
}