using Boulevard.Contexts;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class PropertyInformationController : Controller
    {
        private readonly PropertyInformationDataAccess _propertyInformationDataAccess;
        private readonly CountryDataAccess _countryDataAccess;
        private readonly CityDataAccess _cityDataAccess;
        private readonly ServiceTypeAccess _serviceTypeAccess;
        private readonly ServiceTypeFileDataAccess _serviceTypeFileDataAccess;
        public PropertyInformationController()
        {
            _propertyInformationDataAccess = new PropertyInformationDataAccess();
            _countryDataAccess = new CountryDataAccess();
            _cityDataAccess = new CityDataAccess();
            _serviceTypeAccess = new ServiceTypeAccess();
            _serviceTypeFileDataAccess = new ServiceTypeFileDataAccess();
        }
        // GET: Admin/PropertyInformation
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> CreateAndUpdate(string key)
        {
            try
            {
                await LoadDdl(13);
                PropertyInformation data = new PropertyInformation();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _propertyInformationDataAccess.GetPropertyInformationByKey(key);
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
        public async Task<ActionResult> CreateAndUpdate(PropertyInformation model, List<HttpPostedFileBase> exteriorsImages, List<HttpPostedFileBase> interiorsImages)
        {
            try
            {
                var db = new BoulevardDbContext();
                if (model != null)
                {
                    await LoadDdl(13);
                    if (model.PropertyInformationId == 0)
                    {
                        var insertData = await _propertyInformationDataAccess.Create(model);
                        if (exteriorsImages != null)
                        {
                            foreach (var extrImage in exteriorsImages)
                            {
                                ServiceTypeFile serviceTypeFile = new ServiceTypeFile();
                                serviceTypeFile.ServiceTypeId = insertData.ServiceTypeId;
                                serviceTypeFile.FileSource = "Exterior";
                                serviceTypeFile.FileType = "Image";
                                serviceTypeFile.ServiceAmenityId = 0;
                                serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(extrImage);
                                await _serviceTypeFileDataAccess.Create(serviceTypeFile);
                            }
                        }
                        
                        if (interiorsImages != null)
                        {
                            foreach (var intImage in interiorsImages)
                            {
                                ServiceTypeFile serviceTypeFile = new ServiceTypeFile();
                                serviceTypeFile.ServiceTypeId = insertData.ServiceTypeId;
                                serviceTypeFile.FileSource = "Interior";
                                serviceTypeFile.FileType = "Image";
                                serviceTypeFile.ServiceAmenityId = 0;
                                serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(intImage);
                                await _serviceTypeFileDataAccess.Create(serviceTypeFile);
                            }
                        }
                    }
                    else
                    {
                        var updateData = await _propertyInformationDataAccess.Update(model);

                        if (exteriorsImages != null && exteriorsImages.FirstOrDefault() != null)
                        {
                            //var db = new BoulevardDbContext();
                            var serviceTypeFileNode = await db.ServiceTypeFiles.Where(s => s.ServiceTypeId == updateData.ServiceTypeId && s.FileSource == "Exterior").ToListAsync();
                            if (serviceTypeFileNode != null)
                            {
                                foreach(var serviceTypeFile in serviceTypeFileNode)
                                {
                                    db.ServiceTypeFiles.Remove(serviceTypeFile);
                                    db.SaveChanges();
                                }
                            }
                            foreach (var extrImage in exteriorsImages)
                            {
                                ServiceTypeFile serviceTypeFile = new ServiceTypeFile();
                                serviceTypeFile.ServiceTypeId = updateData.ServiceTypeId;
                                serviceTypeFile.FileSource = "Exterior";
                                serviceTypeFile.FileType = "Image";
                                serviceTypeFile.ServiceAmenityId = 0;
                                serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(extrImage);
                                await _serviceTypeFileDataAccess.Create(serviceTypeFile);
                            }
                        }
                        
                        if (interiorsImages != null && interiorsImages.FirstOrDefault() != null)
                        {
                            //var db = new BoulevardDbContext();
                            var serviceTypeFileNode = await db.ServiceTypeFiles.Where(s => s.ServiceTypeId == updateData.ServiceTypeId && s.FileSource == "Interior").ToListAsync();
                            if (serviceTypeFileNode != null)
                            {
                                foreach(var serviceTypeFile in serviceTypeFileNode)
                                {
                                    db.ServiceTypeFiles.Remove(serviceTypeFile);
                                    db.SaveChanges();
                                }
                            }
                            foreach (var intrImage in interiorsImages)
                            {
                                ServiceTypeFile serviceTypeFile = new ServiceTypeFile();
                                serviceTypeFile.ServiceTypeId = updateData.ServiceTypeId;
                                serviceTypeFile.FileSource = "Interior";
                                serviceTypeFile.FileType = "Image";
                                serviceTypeFile.ServiceAmenityId = 0;
                                serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(intrImage);
                                await _serviceTypeFileDataAccess.Create(serviceTypeFile);
                            }
                        }
                    }
                }

                return RedirectToAction("Index", "PropertyInformation");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        [HttpGet]
        public async Task<bool> DeleteImage(int ImageId)
        {

            return await _serviceTypeFileDataAccess.DeleteImage(ImageId);

        }


        [HttpGet]
        public async Task<JsonResult> GetCitiesByCountryId(int id)
        {
            //var db = new QueueDbContexts();

            var departnemts = await _cityDataAccess.GetCitiesByCountryId(id);
            List<SelectListItem> selectGroup = departnemts.Select(l => new SelectListItem
            {
                Value = l.CityId.ToString(),
                Text = l.CityName

            }).ToList();
            ViewBag.City = selectGroup;
            return Json(departnemts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedPropertyInformations(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var db = new BoulevardDbContext();
                var query = db.PropertyInformations.Where(p => p.Status != "Delete");
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(p => p.RefNo.ToLower().Contains(term) || p.Type.ToLower().Contains(term));
                }
                var rows = await query
                    .OrderByDescending(p => p.PropertyInformationId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new {
                        propertyInformationId = p.PropertyInformationId,
                        propertyKey = p.PropertyInfoKey,
                        refNo = p.RefNo,
                        type = p.Type,
                        propertyPhoneNo = p.PropertyPhoneNo,
                        propertyEmail = p.PropertyEmail,
                        status = p.Status
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
        public async Task<ActionResult> GetPagedPropertyInformationsCount(string searchTerm)
        {
            try
            {
                var db = new BoulevardDbContext();
                var query = db.PropertyInformations.Where(p => p.Status != "Delete");
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(p => p.RefNo.ToLower().Contains(term) || p.Type.ToLower().Contains(term));
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

        #region Drop Down

        private async Task LoadDdl(int featureCategoryId = 0)
        {
            var country = await _countryDataAccess.GetAll();
            List<SelectListItem> selectCountry = country.Select(l => new SelectListItem
            {
                Value = l.CountryId.ToString(),
                Text = l.CountryName

            }).ToList();
            ViewBag.Country = selectCountry;
            
            var city = await _cityDataAccess.GetAll();
            List<SelectListItem> selectCity = city.Select(l => new SelectListItem
            {
                Value = l.CityId.ToString(),
                Text = l.CityName

            }).ToList();
            ViewBag.City = selectCity;
            
            var serviceType = await _serviceTypeAccess.GetAll(featureCategoryId);
            List<SelectListItem> selectServiceType = serviceType.Select(l => new SelectListItem
            {
                Value = l.ServiceTypeId.ToString(),
                Text = l.ServiceTypeName

            }).ToList();
            ViewBag.ServiceType = selectServiceType;
        }

        #endregion
    }
}