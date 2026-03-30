using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        private BrandAccess _brandAccess;
        private FeatureCategoryAccess _featureCategoryAccess;

        public BrandController()
        {
            _brandAccess = new BrandAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();

        }
        public async Task<ActionResult> Index(string fCatagoryKey)
        {
            ViewBag.FCatagoryKey = fCatagoryKey;
            if (!string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key, string fCatagoryKey)
        {
            ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey), "FeatureCategoryId", "Name");
            if (Key == null || Key == Guid.Empty)
            {
                ViewBag.FeatureCategoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                Brand node = new Brand();
                return View(node);
            }
            else
            {
                Brand node = await _brandAccess.GetByKey(Key.Value);
                FeatureCategory featureCategory = await _featureCategoryAccess.GetById(node.FeatureCategoryId);
                ViewBag.FeatureCategoryKey = featureCategory.FeatureCategoryKey;
                if (!string.IsNullOrEmpty(featureCategory.FeatureCategoryKey.ToString()))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(featureCategory.FeatureCategoryKey.ToString());
                }
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(Brand model, HttpPostedFileBase Image, HttpPostedFileBase mediumImage, HttpPostedFileBase largeImage)
        {

            if (model.BrandKey == Guid.Empty)
            {
                if (Image != null)
                {
                    model.Image = _brandAccess.UploadImage(Image);
                }
                if (mediumImage != null)
                {
                    model.MediumImage = _brandAccess.UploadMediumImage(mediumImage);
                }
                if (largeImage != null)
                {
                    model.LargeImage = _brandAccess.UploadLargeImage(largeImage);
                }
                var featureCategory = new FeatureCategory();
                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.FeatureCategoryKey));
                    model.FeatureCategoryId = featureCategory.FeatureCategoryId;
                }
                await _brandAccess.Insert(model);
            }
            else
            {
                Brand node = await _brandAccess.GetByKey(model.BrandKey);

                if (Image != null)
                {
                    model.Image = _brandAccess.UploadImage(Image);
                }
                else
                {
                    model.Image = node.Image;
                }
                if (mediumImage != null)
                {
                    model.MediumImage = _brandAccess.UploadMediumImage(mediumImage);
                }
                else
                {
                    model.MediumImage = node.MediumImage;
                }
                if (largeImage != null)
                {
                    model.LargeImage = _brandAccess.UploadLargeImage(largeImage);
                }
                else
                {
                    model.LargeImage = node.LargeImage;
                }
                model.FeatureCategoryId = node.FeatureCategoryId;
                await _brandAccess.Update(model);
            }
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("Index", "Brand", new { fCatagoryKey = model.FeatureCategoryKey});
            }
            else
            {
                return RedirectToAction("Index", "Brand");
            }
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
                return await _brandAccess.Delete(Key.Value, 1);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedBrands(string fCatagoryKey, string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var db = new BoulevardDbContext();
                var query = db.Brands.AsQueryable();
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await db.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(b => b.FeatureCategoryId == fcId);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(b => b.Title.ToLower().Contains(term));
                }
                var rows = await query
                    .OrderByDescending(b => b.BrandId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new {
                        brandKey = b.BrandKey,
                        title = b.Title,
                        featureCategoryName = b.FeatureCategory.Name,
                        isFeature = b.IsFeature,
                        isTrending = b.IsTrenbding,
                        image = b.Image
                    })
                    .ToListAsync();
                return Json(new { success = true, rows }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedBrandsCount(string fCatagoryKey, string searchTerm)
        {
            try
            {
                var db = new BoulevardDbContext();
                var query = db.Brands.AsQueryable();
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await db.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(b => b.FeatureCategoryId == fcId);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(b => b.Title.ToLower().Contains(term));
                }
                int total = await query.CountAsync();
                return Json(new { success = true, total }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Test Task
        public ActionResult ChocolateAndFlawerBrandTestIndex()
        {
            return View();
        }
        public ActionResult ChocolateAndFlawerBrandTestAdd()
        {
            return View();
        }
        public ActionResult ChocolateAndFlawerBrandTestEdit()
        {
            return View();
        }
        public ActionResult GroceryBrandTestIndex()
        {
            return View();
        }
        public ActionResult GroceryBrandTestAdd()
        {
            return View();
        }
        public ActionResult GroceryBrandTestEdit()
        {
            return View();
        }
        #endregion
    }
}

