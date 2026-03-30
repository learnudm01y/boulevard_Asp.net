using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class FeatureCategoryController : Controller
    {
        private FeatureCategoryAccess _featureCategoryAccess;
        public FeatureCategoryController() 
        {
            _featureCategoryAccess= new FeatureCategoryAccess();
        }
        public async Task<ActionResult> Index()
        {
           IEnumerable<FeatureCategory> list=await _featureCategoryAccess.GetAll();
            return View(list);
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key)
        {
            ViewBag.featureType = new List<SelectListItem>{ new SelectListItem{
                Text="Product",
                Value = "Product"
            },
            new SelectListItem{
                Text="Service",
                Value = "Service"
            }};
            if (Key == null || Key == Guid.Empty)
            {
                FeatureCategory node = new FeatureCategory();
                return View(node);
            }
            else
            {
                FeatureCategory node = await _featureCategoryAccess.GetByKey(Key.Value);
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(FeatureCategory model, HttpPostedFileBase Image)
        {

            if (Image != null)
            {
                model.Image = _featureCategoryAccess.UploadImage(Image);
            }
            if (model.FeatureCategoryKey == Guid.Empty)
            {             
                await _featureCategoryAccess.Insert(model);
            }
            else
            {
                await _featureCategoryAccess.Update(model);
            }
            return RedirectToAction("Index", "FeatureCategory");
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
               return  await _featureCategoryAccess.Delete(Key.Value,1);
            }
        }

        #region Test Task
        public ActionResult GroceryCategoryIndex()
        {
            return View();
        }
        public ActionResult GroceryCategoryAdd()
        {
            return View();
        }
        public ActionResult GroceryCategoryEdit()
        {
            return View();
        }
        
        public ActionResult ChocolateAndFlawerCategoryIndex()
        {
            return View();
        }
        public ActionResult ChocolateAndFlawerCategoryAdd()
        {
            return View();
        }
        public ActionResult ChocolateAndFlawerCategoryEdit()
        {
            return View();
        }
        #endregion
    }
}