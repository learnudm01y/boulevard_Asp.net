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
using System.Xml.Linq;

namespace Boulevard.Areas.Admin.Controllers
{
    public class WebHtmlController : Controller
    {
        private readonly WebHtmlDataAccess _webHtmlDataAccess;
        private readonly FeatureCategoryAccess _featureCategoryAccess;
        private readonly CategoryAccess _categoryAccess;
        private readonly BrandAccess _brandAccess;
        private Boulevard.Service.ServiceAccess _serviceAccess;

        public WebHtmlController()
        {
            _serviceAccess = new Boulevard.Service.ServiceAccess();
            _webHtmlDataAccess = new WebHtmlDataAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _categoryAccess = new CategoryAccess();
            _brandAccess = new BrandAccess();
        }
        // GET: Admin/WebHtml
        public async Task<ActionResult> Index(string fCatagoryKey)
        {
            try
            {
                var data = await _webHtmlDataAccess.GetAll();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    data = await _webHtmlDataAccess.GetAllWebHtml(fCatagoryKey);
                }
                ViewBag.FCatagoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                if (data.Count > 0)
                {
                    return View(data);
                }
                else
                {
                    return View(new List<WebHtml>());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        // GET: Admin/WebHtml
        public async Task<ActionResult> IndexMotorTest()
        {

            List<WebHtml> List = new List<WebHtml>();
            WebHtml obj1 = new WebHtml()
            {
                WebHtmlkey = Guid.NewGuid(),
                Identifier = "Top Banner",
                Title = "Get Exiting Offer og 35% Off..!",
                SubTitle = "This Sunday for every Platinum Members.",
                SmallDetailsOne = "35% Off",
                SmallDetailsTwo = "Hot Sunday",
                BigDetailsOne = "All of our Platinum members will get 35% off on every service.",
                BigDetailsTwo = "All of our Platinum members will get 35% off on every service.",
                PictureOne = "/Content/Service/Banner1.png",
                PictureTwo = "/Content/Service/Banner1.png",
                PictureThree = "/Content/Service/Banner1.png",
                FeatureCategoryId = 1,
                ButtonLink = "Doctor Bikes"
            };
            WebHtml obj2 = new WebHtml()
            {
                WebHtmlkey = Guid.NewGuid(),
                Identifier = "Bottom Banner",
                Title = "Fastest Car Painter in UAE..!",
                SubTitle = "Get your car painted in just 1 day.",
                SmallDetailsOne = "35% Off",
                SmallDetailsTwo = "Hot Sunday",
                BigDetailsOne = "Quicker then Anything Around..!",
                BigDetailsTwo = "Quicker then Anything Around..!",
                PictureOne = "/Content/Service/Banner2.png",
                PictureTwo = "/Content/Service/Banner2.png",
                PictureThree = "/Content/Service/Banner2.png",
                FeatureCategoryId = 1,
                ButtonLink = "Doctor Bikes"
            };
            List.Add(obj1);
            List.Add(obj2);
            return View(List);


        }
        [HttpGet]
        public async Task<ActionResult> CreateAndUpdateMotorTest(string key)
        {
            try
            {
                await LoadDdl();
                WebHtml data = new WebHtml();
                IEnumerable<Category> result = await _categoryAccess.GetAll();
                List<Category> categoryTree = await _categoryAccess.GetParentCategories(result.ToList());
                data.CategoryTree = categoryTree;

                IEnumerable<Brand> brand = await _brandAccess.GetAll();
                List<Brand> brandTree = await _brandAccess.GetParentBrand(brand.ToList());
                data.BrandTree = brandTree;
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = new WebHtml()
                    {
                        WebHtmlkey = Guid.NewGuid(),
                        Identifier = "Bottom Banner",
                        Title = "25% off on Friday!",
                        SubTitle = "25% off on Friday!",
                        SmallDetailsOne = "25% Off",
                        SmallDetailsTwo = "Hot Fariday",
                        BigDetailsOne = "Quicker then Anything Around..!",
                        BigDetailsTwo = "Quicker then Anything Around..!",
                        PictureOne = "/Content/Salon/7.png",
                        PictureTwo = "/Content/Salon/7.png",
                        PictureThree = "/Content/Salon/7.png",
                        FeatureCategoryId = 1,
                        ButtonLink = "Opra Salon"
                    };
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key, string fCatagoryKey)
        {
            try
            {
                WebHtml data = new WebHtml();
                ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagoryKey), "ServiceId", "Name");

                ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey), "FeatureCategoryId", "Name");
                IEnumerable<Category> result = await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey);
                List<Category> categoryTree = await _categoryAccess.GetParentCategories(result.ToList());
                data.CategoryTree = categoryTree;

                IEnumerable<Brand> brand = await _brandAccess.GetAllByFeatureCategory(fCatagoryKey);
                List<Brand> brandTree = await _brandAccess.GetParentBrand(brand.ToList());
                data.BrandTree = brandTree;
                if (string.IsNullOrEmpty(key))
                {
                    ViewBag.FeatureCategoryKey = fCatagoryKey;
                    if (!string.IsNullOrEmpty(fCatagoryKey))
                    {
                        var fCategory = await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey);
                        ViewBag.FCatagoryName = fCategory.Select(selector => selector.Name).FirstOrDefault();
                        ViewBag.fCategoryId = fCategory.Select(selector => selector.FeatureType).FirstOrDefault();
                    }
                    return View(data);
                }
                else
                {
                    data = await _webHtmlDataAccess.GetWebHtmlByKey(key);
                    FeatureCategory featureCategory = new FeatureCategory();
                    if (data.FeatureCategoryId.HasValue)
                    {
                        featureCategory = await _featureCategoryAccess.GetById(data.FeatureCategoryId.Value);
                        ViewBag.FeatureCategoryKey = featureCategory.FeatureCategoryKey;
                        if (!string.IsNullOrEmpty(featureCategory.FeatureCategoryKey.ToString()))
                        {
                            var fCategory = await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey);
                            ViewBag.FCatagoryName = fCategory.Select(selector => selector.Name).FirstOrDefault();
                            ViewBag.fCategoryId = fCategory.Select(selector => selector.FeatureType).FirstOrDefault();
                        }
                    }
                    
                   
                    
                    ViewBag.categoryId = data.CategoryId;
                    ViewBag.brandId = data.BrandId;
                    data.CategoryTree = categoryTree;
                    data.BrandTree = brandTree;
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAndUpdate(WebHtml model, HttpPostedFileBase PictureOne, HttpPostedFileBase PictureTwo, HttpPostedFileBase PictureThree)
        {
            try
            {
                if (model != null)
                {
                    if (model.WebHtmlId == 0)
                    {
                        if (PictureOne != null)
                        {
                            model.PictureOne = _webHtmlDataAccess.UploadPictureOne(PictureOne);
                        }

                       
                        if (PictureTwo != null)
                        {
                            model.PictureOneAr = _webHtmlDataAccess.UploadPictureTwo(PictureTwo);
                        }
                        if (PictureThree != null)
                        {
                            model.PictureThree = _webHtmlDataAccess.UploadPictureThree(PictureThree);
                        }
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        var featureCategory = new FeatureCategory();
                        if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                        {
                            featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.FeatureCategoryKey));
                            model.FeatureCategoryId = featureCategory.FeatureCategoryId;
                        }
                        await _webHtmlDataAccess.Create(model);
                    }
                    else
                    {
                        var result = await _webHtmlDataAccess.GetWebHtmlById(model.WebHtmlId);
                        if (PictureOne != null)
                        {
                            model.PictureOne = _webHtmlDataAccess.UploadPictureOne(PictureOne);
                        }
                        else
                        {
                            model.PictureOne = result.PictureOne;
                        }
                        if (PictureTwo != null)
                        {
                            model.PictureOneAr = _webHtmlDataAccess.UploadPictureTwo(PictureTwo);
                        }
                        else
                        {
                            model.PictureOneAr = result.PictureOneAr;
                        }
                        if (PictureThree != null)
                        {
                            model.PictureThree = _webHtmlDataAccess.UploadPictureThree(PictureThree);
                        }
                        else
                        {
                            model.PictureThree = result.PictureThree;
                        }

                        model.UpdateBy = 1;
                        model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        model.FeatureCategoryId = result.FeatureCategoryId;
                        //await _webHtmlDataAccess.Update(model);
                        var db = new BoulevardDbContext();
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    return RedirectToAction("Index", "WebHtml", new { fCatagoryKey = model.FeatureCategoryKey });
                }
                else
                {
                    return RedirectToAction("Index", "WebHtml");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                WebHtml modelData = await _webHtmlDataAccess.GetWebHtmlById(id);
                modelData.DeleteDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                modelData.DeleteBy = 1;
                modelData.Status = "Deleted";
                await _webHtmlDataAccess.Update(modelData);
                return RedirectToAction("Index", "WebHtml");
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Test Task
        public ActionResult WebHtmlTestIndex()
        {

            return View();
        }
        public ActionResult WebHtmlTestAdd()
        {

            return View();
        }
        public ActionResult WebHtmlTestEdit()
        {

            return View();
        }
        #endregion

        #region Drop Down
        private async Task LoadDdl()
        {
            var featureCategory = await _featureCategoryAccess.GetAll();
            var item = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = "Select Feature Category"
            };
            List<SelectListItem> selectFeatureCategory = featureCategory.Select(l => new SelectListItem
            {
                Value = l.FeatureCategoryId.ToString(),
                Text = l.Name

            }).ToList();
            selectFeatureCategory.Add(item);
            ViewBag.FeatureCategory = selectFeatureCategory;
        }
        #endregion
    }
}