using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
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
    public class ServiceAmenityController : BaseController
    {
        private ServiceAmenityAccess _serviceAmenityAccess;
        private ServiceAccess _serviceAccess;
        private readonly FeatureCategoryAccess _featureCategoryAccess;

        public ServiceAmenityController()
        {
            _serviceAccess = new ServiceAccess();
            _serviceAmenityAccess = new ServiceAmenityAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
        }
        public async Task<ActionResult> Index(string fCatagoryKey)
        {
            try
            {
                IEnumerable<ServiceAmenity> list = await _serviceAmenityAccess.GetAllServiceAmenityByFeatureCategory(fCatagoryKey);
                ViewBag.FCatagoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                return View(list);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "ServiceAmenity");
            }
        }
        
        public async Task<ActionResult> IndexForPackage(string fCatagoryKey)
        {
            try
            {
                IEnumerable<ServiceAmenity> list = await _serviceAmenityAccess.GetAllServiceAmenityForPackage(fCatagoryKey);
                ViewBag.FCatagoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                return View(list);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "ServiceAmenity");
            }
        }
        public async Task<ActionResult> IndexMotorTest()
        {



            List<ServiceAmenity> list = new List<ServiceAmenity>();

            ServiceAmenity obj1 = new ServiceAmenity()
            {
                AmenitiesName = "Light",
                AmenitiesLogo = "Doctor Bikes",
                ServiceAmenityKey = Guid.NewGuid()
            };
            ServiceAmenity obj2 = new ServiceAmenity()
            {
                AmenitiesName = "Wheel",
                AmenitiesLogo = "Doctor Bikes",
                ServiceAmenityKey = Guid.NewGuid()
            };
            ServiceAmenity obj3 = new ServiceAmenity()
            {
                AmenitiesName = "Engine",
                AmenitiesLogo = "Doctor Bikes",
                ServiceAmenityKey = Guid.NewGuid()
            };
            ServiceAmenity obj4 = new ServiceAmenity()
            {
                AmenitiesName = "Spring",
                AmenitiesLogo = "Doctor Bikes",
                ServiceAmenityKey = Guid.NewGuid()
            };
            ServiceAmenity obj5 = new ServiceAmenity()
            {
                AmenitiesName = "Steat",
                AmenitiesLogo = "Doctor Bikes",
                ServiceAmenityKey = Guid.NewGuid()
            };
            list.Add(obj1);
            list.Add(obj2);
            list.Add(obj3);
            list.Add(obj4);
            list.Add(obj5);
            return View(list);
        }
        [HttpGet]
        public async Task<ActionResult> CreateMotorTest(Guid? Key, string fCatagoryKey)
        {
            if (string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagoryKey), "ServiceId", "Name");
            }
            else
            {
                ViewBag.service = new SelectList(await _serviceAccess.GetAll(), "ServiceId", "Name");
            }

            if (Key == null || Key == Guid.Empty)
            {
                ServiceAmenity node = new ServiceAmenity();
                return View(node);
            }
            else
            {
                ServiceAmenity node = new ServiceAmenity()
                {
                    AmenitiesName = "Steat",
                    ServiceId = 1,
                    ServiceAmenityKey = Guid.NewGuid()
                };
                return View(node);
            }
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key, string fCatagoryKey)
        {
            if (string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.service = new SelectList(await _serviceAccess.GetAll(), "ServiceId", "Name");
            }
            else
            {
                ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagoryKey), "ServiceId", "Name");
            }
            if (Key == null || Key == Guid.Empty)
            {
                ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                ServiceAmenity node = new ServiceAmenity();
                return View(node);
            }
            else
            {
                ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                ServiceAmenity node = await _serviceAmenityAccess.GetByKey(Key.Value);
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(ServiceAmenity model, HttpPostedFileBase Image)
        {
            var service = await _serviceAccess.GetById(model.ServiceId);
            ViewBag.FeatureCategoryKey = service.FeatureCategoryId;
            var user = GetUser();
            if (Image != null)
            {
                model.AmenitiesLogo = _serviceAmenityAccess.UploadImage(Image);
            }
            if (model.ServiceAmenityKey == null || model.ServiceAmenityKey == Guid.Empty)
            {
                model.CreateBy = user.UserId;
                await _serviceAmenityAccess.Insert(model);
            }
            else
            {
                model.UpdateBy = user.UserId;
                await _serviceAmenityAccess.Update(model);
            }
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("Index", "ServiceAmenity", new { fCatagoryKey = model.FeatureCategoryKey });
            }
            else
            {
                return RedirectToAction("Index", "ServiceAmenity");
            }
            //return RedirectToAction("Index", "ServiceAmenity");
        }
        
        public async Task<ActionResult> CreateAmenityForPackage(Guid? Key, string fCatagoryKey)
        {
            if (string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.service = new SelectList(await _serviceAccess.GetAll(), "ServiceId", "Name");
            }
            else
            {
                ViewBag.service = new SelectList(await _serviceAccess.GetServicesByFeatureCategoryForPackage(fCatagoryKey), "ServiceId", "Name");
            }
            if (Key == null || Key == Guid.Empty)
            {
                ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                ServiceAmenity node = new ServiceAmenity();
                return View(node);
            }
            else
            {
                ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                ServiceAmenity node = await _serviceAmenityAccess.GetByKey(Key.Value);
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateAmenityForPackage(ServiceAmenity model, HttpPostedFileBase Image)
        {
            var service = await _serviceAccess.GetById(model.ServiceId);
            if (service != null)
            {
                var featureCategory = await _featureCategoryAccess.GetById(service.FeatureCategoryId);
                ViewBag.FeatureCategoryKey = featureCategory.FeatureCategoryKey;
            }
            var user = GetUser();
            if (Image != null)
            {
                model.AmenitiesLogo = _serviceAmenityAccess.UploadImage(Image);
            }
            if (model.ServiceAmenityKey == null || model.ServiceAmenityKey == Guid.Empty)
            {
                model.CreateBy = user.UserId;
                await _serviceAmenityAccess.Insert(model);
            }
            else
            {
                model.UpdateBy = user.UserId;
                await _serviceAmenityAccess.Update(model);
            }
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("IndexForPackage", "ServiceAmenity", new { fCatagoryKey = model.FeatureCategoryKey });
            }
            else
            {
                return RedirectToAction("IndexForPackage", "ServiceAmenity");
            }
            //return RedirectToAction("Index", "ServiceAmenity");
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
                return await _serviceAmenityAccess.Delete(Key.Value, 1);
            }
        }
    }
}