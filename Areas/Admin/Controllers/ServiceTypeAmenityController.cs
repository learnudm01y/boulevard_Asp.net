using Boulevard.Contexts;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class ServiceTypeAmenityController : Controller
    {
        private readonly ServiceTypeAmenityDataAccess _serviceTypeAmenityDataAccess;
        private readonly ServiceTypeAccess _serviceTypeAccess;
        private readonly ServiceTypeFileDataAccess _serviceTypeFileDataAccess;
        public ServiceTypeAmenityController()
        {
            _serviceTypeAmenityDataAccess = new ServiceTypeAmenityDataAccess();
            _serviceTypeAccess = new ServiceTypeAccess();
            _serviceTypeFileDataAccess = new ServiceTypeFileDataAccess();
        }
        // GET: Admin/ServiceTypeAmenity
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
                ServiceTypeAmenity data = new ServiceTypeAmenity();
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = await _serviceTypeAmenityDataAccess.GetServiceTypeAmenityByKey(key);
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
        public async Task<ActionResult> CreateAndUpdate(ServiceTypeAmenity model, HttpPostedFileBase amenitiesLogo, HttpPostedFileBase serviceTypeFiles)
        {
            try
            {
                if (model != null)
                {
                    ServiceTypeFile serviceTypeFile = new ServiceTypeFile();

                    if (model.ServiceAmenityId == 0)
                    {
                        if (amenitiesLogo != null)
                        {
                            model.AmenitiesLogo = _serviceTypeAmenityDataAccess.UploadImage(amenitiesLogo);
                        }
                        model.CreateBy = 1;
                        model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                        var createDataNode = await _serviceTypeAmenityDataAccess.Create(model);

                        if (serviceTypeFiles != null)
                        {
                            serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(serviceTypeFiles);

                            FileInfo fi = new FileInfo(serviceTypeFiles.FileName);
                            string extension = fi.Extension;

                            serviceTypeFile.ServiceTypeId = createDataNode.ServiceTypeId;
                            serviceTypeFile.FileSource = "ServiceTypeAmenity";
                            if (extension != null && (extension.EndsWith(".png") || extension.EndsWith(".jpg") || extension.EndsWith(".jpeg") || extension.EndsWith(".gif") ||
                                extension.EndsWith(".bmp") || extension.EndsWith(".tif") || extension.EndsWith(".webp") || extension.EndsWith(".raw")))
                            {
                                serviceTypeFile.FileType = "Image";
                            }
                            if (extension != null && (extension.EndsWith(".pdf") || extension.EndsWith(".txt") || extension.EndsWith(".doc") || extension.EndsWith(".docx") || extension.EndsWith(".xls") || extension.EndsWith(".rtf")))
                            {
                                serviceTypeFile.FileType = "File";
                            }
                            serviceTypeFile.ServiceAmenityId = createDataNode.ServiceAmenityId;
                            await _serviceTypeFileDataAccess.Create(serviceTypeFile);
                        }
                    }
                    else
                    {
                        var serviceAmenity = await _serviceTypeAmenityDataAccess.GetServiceTypeAmenityById(model.ServiceAmenityId);
                        if (serviceAmenity != null)
                        {
                            if (amenitiesLogo != null)
                            {
                                model.AmenitiesLogo = _serviceTypeAmenityDataAccess.UploadImage(amenitiesLogo);
                            }
                            else
                            {
                                model.AmenitiesLogo = serviceAmenity.AmenitiesLogo;
                            }
                        }

                        //var updateDataNode = await _serviceTypeAmenityDataAccess.Update(serviceAmenity);
                        var updateDataNode = await _serviceTypeAmenityDataAccess.Update(model);

                        if (serviceTypeFiles != null)
                        {
                            var serviceTypeFileNode = await _serviceTypeFileDataAccess.GetServiceTypeFileByServiceAmenityId(serviceAmenity.ServiceAmenityId);

                            if (serviceTypeFileNode == null)
                            {
                                serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(serviceTypeFiles);

                                FileInfo fi1 = new FileInfo(serviceTypeFiles.FileName);
                                string extension1 = fi1.Extension;

                                serviceTypeFile.ServiceTypeId = updateDataNode.ServiceTypeId;
                                serviceTypeFile.FileSource = "ServiceTypeAmenity";
                                if (extension1 != null && (extension1.EndsWith(".png") || extension1.EndsWith(".jpg") || extension1.EndsWith(".jpeg") || extension1.EndsWith(".gif") ||
                                    extension1.EndsWith(".bmp") || extension1.EndsWith(".tif") || extension1.EndsWith(".webp") || extension1.EndsWith(".raw")))
                                {
                                    serviceTypeFile.FileType = "Image";
                                }
                                if (extension1 != null && (extension1.EndsWith(".pdf") || extension1.EndsWith(".txt") || extension1.EndsWith(".doc") || extension1.EndsWith(".docx") || extension1.EndsWith(".xls") || extension1.EndsWith(".rtf")))
                                {
                                    serviceTypeFile.FileType = "File";
                                }
                                serviceTypeFile.ServiceAmenityId = updateDataNode.ServiceAmenityId;
                                await _serviceTypeFileDataAccess.Create(serviceTypeFile);
                            }

                            else
                            {

                                if (serviceTypeFiles != null)
                                {
                                    serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(serviceTypeFiles);
                                }
                                else
                                {
                                    serviceTypeFile.FileLocation = serviceTypeFileNode.FileLocation;
                                }
                                serviceTypeFile.ServiceTypeId = model.ServiceTypeId;
                                serviceTypeFile.FileSource = "ServiceTypeAmenity";
                                FileInfo fi = new FileInfo(serviceTypeFiles.FileName);
                                string extension = fi.Extension;
                                if (extension != null && (extension.EndsWith(".png") || extension.EndsWith(".jpg") || extension.EndsWith(".jpeg") || extension.EndsWith(".gif")))
                                {
                                    serviceTypeFile.FileType = "Image";
                                }
                                if (extension != null && (extension.EndsWith(".pdf") || extension.EndsWith(".txt") || extension.EndsWith(".doc") || extension.EndsWith(".docx") || extension.EndsWith(".xls")))
                                {
                                    serviceTypeFile.FileType = "File";
                                }
                                //serviceTypeFile.ServiceTypeFileId = serviceTypeFileNode.ServiceTypeFileId;
                                serviceTypeFile.ServiceAmenityId = model.ServiceAmenityId;
                                serviceTypeFile.LastUpdate = Boulevard.Helper.DateTimeHelper.CreateDate();
                                //await _serviceTypeFileDataAccess.Update(serviceTypeFile);
                                var db = new BoulevardDbContext();
                                db.Entry(serviceTypeFile).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }

                return RedirectToAction("Index", "ServiceTypeAmenity");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "ServiceTypeAmenity");
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                ServiceTypeAmenity modelData = await _serviceTypeAmenityDataAccess.GetServiceTypeAmenityById(id);
                modelData.DeleteDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                modelData.DeleteBy = id;
                modelData.Status = "Deleted";
                await _serviceTypeAmenityDataAccess.Update(modelData);
                return RedirectToAction("Index", "ServiceTypeAmenity");
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetPagedServiceTypeAmenities(string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var db = new BoulevardDbContext();
                var query = db.ServiceTypeAmenities.Where(a => a.Status != "Deleted");
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(a => a.AmenitiesName.ToLower().Contains(term));
                }
                var rows = await query
                    .OrderByDescending(a => a.ServiceAmenityId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new {
                        serviceAmenityId = a.ServiceAmenityId,
                        serviceAmenityKey = a.ServiceAmenityKey,
                        serviceTypeName = a.ServiceType.ServiceTypeName,
                        amenitiesName = a.AmenitiesName,
                        amenitiesType = a.AmenitiesType,
                        amenitiesLogo = a.AmenitiesLogo,
                        status = a.Status
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
        public async Task<ActionResult> GetPagedServiceTypeAmenitiesCount(string searchTerm)
        {
            try
            {
                var db = new BoulevardDbContext();
                var query = db.ServiceTypeAmenities.Where(a => a.Status != "Deleted");
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(a => a.AmenitiesName.ToLower().Contains(term));
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