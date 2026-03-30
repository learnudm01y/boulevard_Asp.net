using Boulevard.Service.Admin;
using Boulevard.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Boulevard.Contexts;
using Boulevard.Models;

namespace Boulevard.Areas.Admin.Controllers
{
    public class PackageController : BaseController
    {
        private ServiceAccess _serviceAccess;
        private FeatureCategoryAccess _featureCategoryAccess;
        private CountryDataAccess _countryDataAccess;
        private CityDataAccess _cityDataAccess;
        private readonly CategoryAccess _categoryAccess;
        private readonly ServiceCategoryDataAccess _serviceCategoryDataAccess;


        public PackageController()
        {
            _serviceAccess = new ServiceAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _countryDataAccess = new CountryDataAccess();
            _cityDataAccess = new CityDataAccess();
            _categoryAccess = new CategoryAccess();
            _serviceCategoryDataAccess = new ServiceCategoryDataAccess();
        }

        // GET: Admin/Package
        public async Task<ActionResult> Index(string fCatagoryKey)
        {
            IEnumerable<Boulevard.Models.Service> list = await _serviceAccess.GetAllPackageByFeatureCategory(fCatagoryKey);
            ViewBag.FCatagoryKey = fCatagoryKey;
            if (!string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
            }
            return View(list);
        }

        [HttpGet]
        public async Task<ActionResult> CreatePackage(Guid? Key, string fCatagoryKey)
        {
            if (string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAll(), "FeatureCategoryId", "Name");
                ViewBag.Catagory = new SelectList(await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey), "CategoryId", "CategoryName");
            }
            else
            {
                ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey), "FeatureCategoryId", "Name");
                ViewBag.Catagory = new SelectList(await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey), "CategoryId", "CategoryName");
            }

            //ViewBag.propertyType = new List<SelectListItem>{ new SelectListItem{
            //        Text="Hotel",
            //        Value = "Hotel"
            //    },
            //    new SelectListItem{
            //        Text="Apartment",
            //        Value = "Apartment"
            //    }};
            if (Key == null || Key == Guid.Empty)
            {
                await LoadDdlV2();
                ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                Boulevard.Models.Service node = new Boulevard.Models.Service();
                node.ServiceImages = new List<ServiceImage> { new ServiceImage() };
                return View(node);
            }
            else
            {
                await LoadDdl();
                Boulevard.Models.Service node = await _serviceAccess.GetByKey(Key.Value);
                if (node != null)
                {
                    var catagory = await _serviceCategoryDataAccess.GetServiceCategoryById(node.ServiceId);
                    if (catagory != null)
                    {
                        node.CategoryId = catagory.CategoryId;
                    }
                    FeatureCategory featureCategory = await _featureCategoryAccess.GetById(node.FeatureCategoryId);
                    ViewBag.FeatureCategoryKey = featureCategory.FeatureCategoryKey.ToString();
                    if (!string.IsNullOrEmpty(fCatagoryKey))
                    {
                        //if (fCatagoryKey == "6440039B-6E5A-4E65-A0E4-F38B69C46C8C")
                        //{
                        //    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                        //}
                        ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                    }
                    return View(node);
                }
                else
                {
                    return View(new Boulevard.Models.Service());
                }
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreatePackage(Boulevard.Models.Service model, List<HttpPostedFileBase> Images)
        {
            Boulevard.Models.Service node = await _serviceAccess.GetByKey(model.ServiceKey);
            if (Images != null && Images.Count() > 0)
            {
                foreach (var img in Images)
                {
                    if (img != null)
                        model.ServiceImageURLs.Add(_serviceAccess.UploadImage(img));
                }
            }

            if (model.ServiceKey == null || model.ServiceKey == Guid.Empty)
            {
                var featureCategory = new FeatureCategory();
                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.FeatureCategoryKey));
                    model.FeatureCategoryId = featureCategory.FeatureCategoryId;
                }
                model.CreateBy = GetUser().UserId;
                var serviceData = await _serviceAccess.InsertForPackage(model);
                if (featureCategory.FeatureCategoryId > 0)
                {
                    if (featureCategory.FeatureCategoryId == 9)
                    {
                        var serviceCategory = new ServiceCategory();
                        serviceCategory.CategoryId = model.CategoryId;
                        serviceCategory.ServiceId = serviceData.ServiceId;
                        await _serviceCategoryDataAccess.Create(serviceCategory);
                    }
                }
            }
            else
            {
                model.FeatureCategoryId = node.FeatureCategoryId;
                model.UpdateBy = GetUser().UserId;
                var result = await _serviceAccess.UpdateForPackage(model);
                if (model.FeatureCategoryId == 9)
                {
                    var db = new BoulevardDbContext();
                    var serviceData = db.ServiceCategories.Where(a => a.ServiceId == result.ServiceId).FirstOrDefault();
                    if (serviceData != null)
                    {
                        db.ServiceCategories.Remove(serviceData);
                        db.SaveChanges();
                        var serviceCategory = new ServiceCategory();
                        serviceCategory.CategoryId = model.CategoryId;
                        serviceCategory.ServiceId = result.ServiceId;
                        await _serviceCategoryDataAccess.Create(serviceCategory);
                    }
                    else
                    {
                        var serviceCategory = new ServiceCategory();
                        serviceCategory.CategoryId = model.CategoryId;
                        serviceCategory.ServiceId = result.ServiceId;
                        await _serviceCategoryDataAccess.Create(serviceCategory);
                    }
                }
            }
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("Index", "Package", new { fCatagoryKey = model.FeatureCategoryKey });
            }
            else
            {
                return RedirectToAction("Index", "Package");
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
                return await _serviceAccess.Delete(Key.Value, 1);
            }
        }


        #region Drop Down

        private async Task LoadDdl()
        {
            var role = await _countryDataAccess.GetAll();
            var item = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = "Select Country"
            };
            List<SelectListItem> selectCountry = role.Select(l => new SelectListItem
            {
                Value = l.CountryId.ToString(),
                Text = l.CountryName

            }).ToList();
            selectCountry.Add(item);
            ViewBag.Country = selectCountry;

            var city = await _cityDataAccess.GetAll();
            var data = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = ""
            };
            List<SelectListItem> selectCity = city.Select(l => new SelectListItem
            {
                Value = l.CityId.ToString(),
                Text = l.CityName

            }).ToList();
            selectCity.Add(data);
            ViewBag.City = selectCity;

            //var Catagory = await _categoryAccess.GetAll();
            //var itemCatagory = new SelectListItem
            //{
            //    Value = "0",
            //    Selected = true,
            //    Text = "Select Catagory"
            //};
            //List<SelectListItem> selectCatagory = Catagory.Select(l => new SelectListItem
            //{
            //    Value = l.CategoryId.ToString(),
            //    Text = l.CategoryName

            //}).ToList();
            //selectCatagory.Add(itemCatagory);
            //ViewBag.Catagory = selectCatagory;
        }

        private async Task LoadDdlV2()
        {
            var role = await _countryDataAccess.GetAll();
            var item = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = "Select Country"
            };
            List<SelectListItem> selectCountry = role.Select(l => new SelectListItem
            {
                Value = l.CountryId.ToString(),
                Text = l.CountryName

            }).ToList();
            selectCountry.Add(item);
            ViewBag.Country = selectCountry;

            var city = await _cityDataAccess.GetAll();
            var data = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = ""
            };
            List<SelectListItem> selectCity = new List<SelectListItem>();
            selectCity.Add(data);
            ViewBag.City = selectCity;
        }

        private async Task CatagoryDropDown(string fCatagoryKey)
        {
            var Catagory = new List<Category>();
            if (!string.IsNullOrEmpty(fCatagoryKey))
            {
                Catagory = await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey);
            }
            else
            {
                Catagory = await _categoryAccess.GetAll();
            }
            var itemCatagory = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = "Select Catagory"
            };
            List<SelectListItem> selectCatagory = Catagory.Select(l => new SelectListItem
            {
                Value = l.CategoryId.ToString(),
                Text = l.CategoryName

            }).ToList();
            selectCatagory.Add(itemCatagory);
            ViewBag.Catagory = selectCatagory;
        }

        #endregion

    }
}