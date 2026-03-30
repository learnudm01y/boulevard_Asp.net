using Boulevard.Areas.Admin.Data;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Linq;

namespace Boulevard.Areas.Admin.Controllers
{
    public class ServiceTypeController : Controller
    {
        private ServiceTypeAccess _serviceTypeAccess;
        private ServiceAccess _serviceAccess;
        private readonly FeatureCategoryAccess _featureCategoryAccess;
        private readonly ServiceCategoryDataAccess _serviceCategoryDataAccess;
        private readonly CategoryAccess _categoryAccess;
        private readonly ServiceTypeFileDataAccess _serviceTypeFileDataAccess;
        private readonly TempServiceTypeDataAccess _tempServiceTypeDataAccess;
        private readonly BoulevardDbContext _context;

        public ServiceTypeController()
        {
            _serviceAccess = new ServiceAccess();
            _serviceTypeAccess = new ServiceTypeAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _serviceCategoryDataAccess = new ServiceCategoryDataAccess();
            _categoryAccess = new CategoryAccess();
            _tempServiceTypeDataAccess = new TempServiceTypeDataAccess();
            _serviceTypeFileDataAccess = new ServiceTypeFileDataAccess();
            _context = new BoulevardDbContext();
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
        
        public async Task<ActionResult> IndexV2(string fCatagoryKey)
        {
            ViewBag.FCatagoryKey = fCatagoryKey;
            if (!string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
            }
            return View();
        }
        public async Task<ActionResult> IndexMotorTest()
        {
            List<ServiceType> list = new List<ServiceType>();
            ServiceType obj1 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Oil Filling",
                Description = "Full Tank Only",
                Image = "Doctor Bikes",
                Size = "100",
                Price = 655
            };
            ServiceType obj2 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Fuel Replace",
                Description = "Quick Service",
                Image = "Doctor Bikes",
                Size = "100",
                Price = 750
            };
            ServiceType obj3 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Tyre Replace",
                Description = "Using Machine",
                Image = "Doctor Bikes",
                Size = "100",
                Price = 460
            };
            ServiceType obj4 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Tyre Presure ",
                Description = "Auto Machine",
                Image = "Doctor Bikes",
                Size = "100",
                Price = 300
            };
            ServiceType obj5 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Break Repair",
                Description = "Laser Treatment",
                Image = "Doctor Bikes",
                Size = "100",
                Price = 500
            };
            list.Add(obj1);
            list.Add(obj2);
            list.Add(obj3);
            list.Add(obj4);
            list.Add(obj5);
            return View(list);
        }
        [HttpGet]
        public async Task<ActionResult> CreateMotorTest(Guid? Key)
        {
            ViewBag.service = new SelectList(await _serviceAccess.GetAllMotorServices(), "ServiceId", "Name");
            if (Key == null || Key == Guid.Empty)
            {
                ServiceType node = new ServiceType();
                return View(node);
            }
            else
            {
                ServiceType node = new ServiceType()
                {
                    ServiceTypeKey = Guid.NewGuid(),
                    ServiceTypeName = "Oil Filling",
                    Description = "Full Tank Only",
                    Size = "100",
                    Price = 655
                };
                return View(node);
            }
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key, string fCatagoryKey)
        {
            //ViewBag.service = new SelectList(await _serviceAccess.GetAll(), "ServiceId", "Name");
            if (string.IsNullOrEmpty(fCatagoryKey))
            {
                var nodeData = await _serviceTypeAccess.GetByKey(Key.Value);
                var service = await _serviceAccess.GetById(nodeData.ServiceId);
                var fCatagory = await _featureCategoryAccess.GetById(service.FeatureCategoryId);
                ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagory.FeatureCategoryKey.ToString()), "ServiceId", "Name");
                ViewBag.Catagory = new SelectList(await _categoryAccess.GetAllByFeatureCategory(fCatagory.FeatureCategoryKey.ToString()), "CategoryId", "CategoryName");
            }
            else
            {
                ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagoryKey), "ServiceId", "Name");
                ViewBag.Catagory = new SelectList(await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey), "CategoryId", "CategoryName");
            }

            if (Key == null || Key == Guid.Empty)
            {
                //await CatagoryDropDown(fCatagoryKey);
                ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                ServiceType node = new ServiceType();
                return View(node);
            }
            else
            {
                var nodeData = await _serviceTypeAccess.GetByKey(Key.Value);
                if (nodeData != null)
                {
                    var catagory = await _serviceCategoryDataAccess.GetServiceCategoryById(nodeData.ServiceId);
                    if (catagory != null)
                    {
                        nodeData.CategoryId = catagory.CategoryId;
                    }
                    var service = await _serviceAccess.GetById(nodeData.ServiceId);
                    var fCatagory = await _featureCategoryAccess.GetById(service.FeatureCategoryId);
                    ViewBag.FeatureCategoryKey = fCatagory.FeatureCategoryKey.ToString();
                    //await CatagoryDropDown(fCatagory.FeatureCategoryKey.ToString());
                    if (!string.IsNullOrEmpty(fCatagory.FeatureCategoryKey.ToString()))
                    {
                        ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagory.FeatureCategoryKey.ToString());
                    }
                    return View(nodeData);
                }
                else
                {
                    return View(new ServiceType());
                }
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(ServiceType model, HttpPostedFileBase Image)
        {
            var db = new BoulevardDbContext();
            if (Image != null)
            {
                model.Image = _serviceTypeAccess.UploadImage(Image);
            }
            if (model.ServiceTypeKey == null || model.ServiceTypeKey == Guid.Empty)
            {
                var serviceData = await _serviceTypeAccess.Insert(model);
                var featureCategoryId = db.Services.Where(s => s.ServiceId == serviceData.ServiceId).Select(s => s.FeatureCategoryId).FirstOrDefault();
                var fCatagory = await _featureCategoryAccess.GetById(featureCategoryId);
                if (fCatagory.FeatureCategoryId != 9)
                {
                    var serviceCategory = new ServiceCategory();
                    serviceCategory.CategoryId = model.CategoryId;
                    serviceCategory.ServiceId = serviceData.ServiceId;
                    serviceCategory.ServiceTypeId = serviceData.ServiceTypeId;
                    await _serviceCategoryDataAccess.Create(serviceCategory);
                }
            }
            else
            {
                var result = await _serviceTypeAccess.Update(model);
                //var service = await _serviceAccess.GetById(result.ServiceId);
                var featureCategoryId = db.Services.Where(s => s.ServiceId == result.ServiceId).Select(s => s.FeatureCategoryId).FirstOrDefault();
                var fCatagory = await _featureCategoryAccess.GetById(featureCategoryId);
                ViewBag.FeatureCategoryKey = fCatagory.FeatureCategoryKey;
                if (!string.IsNullOrEmpty(result.FeatureCategoryKey))
                {
                    if (fCatagory.FeatureCategoryId != 9)
                    {
                        var serviceData = db.ServiceCategories.Where(a => a.ServiceTypeId == result.ServiceTypeId).FirstOrDefault();
                        if (serviceData != null)
                        {
                            db.ServiceCategories.Remove(serviceData);
                            db.SaveChanges();
                            var serviceCategory = new ServiceCategory();
                            serviceCategory.CategoryId = model.CategoryId;
                            serviceCategory.ServiceId = result.ServiceId;
                            serviceCategory.ServiceTypeId = result.ServiceTypeId;
                            await _serviceCategoryDataAccess.Create(serviceCategory);
                        }
                        else
                        {
                            var serviceCategory = new ServiceCategory();
                            serviceCategory.CategoryId = model.CategoryId;
                            serviceCategory.ServiceId = result.ServiceId;
                            serviceCategory.ServiceTypeId = result.ServiceTypeId;
                            await _serviceCategoryDataAccess.Create(serviceCategory);
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("Index", "ServiceType", new { fCatagoryKey = model.FeatureCategoryKey });
            }
            else
            {
                return RedirectToAction("Index", "ServiceType");
            }
        }
        
        [HttpGet]
        public async Task<ActionResult> CreateV2(Guid? Key, string fCatagoryKey)
        {
            //ViewBag.service = new SelectList(await _serviceAccess.GetAll(), "ServiceId", "Name");
            if (string.IsNullOrEmpty(fCatagoryKey))
            {
                var nodeData = await _serviceTypeAccess.GetByKey(Key.Value);
                var service = await _serviceAccess.GetById(nodeData.ServiceId);
                var fCatagory = await _featureCategoryAccess.GetById(service.FeatureCategoryId);
                ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagory.FeatureCategoryKey.ToString()), "ServiceId", "Name");
                ViewBag.Catagory = new SelectList(await _categoryAccess.GetAllByFeatureCategory(fCatagory.FeatureCategoryKey.ToString()), "CategoryId", "CategoryName");
            }
            else
            {
                if (fCatagoryKey == "f4309df5-9121-41ad-831a-994c46b62766" || fCatagoryKey == "c286a46b-5b9a-4519-bb10-8d47ec254ffb")
                {
                    ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategoryForChildService(fCatagoryKey), "ServiceId", "Name");

                }
                else
                {
                    ViewBag.service = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagoryKey), "ServiceId", "Name");
                }    

                ViewBag.Catagory = new SelectList(await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey), "CategoryId", "CategoryName");
            }

            List<Category> AllCategories = await _categoryAccess.GetAllByFeatureCategory(fCatagoryKey);

            if (Key == null || Key == Guid.Empty)
            {
                //await CatagoryDropDown(fCatagoryKey);
                ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                ServiceType node = new ServiceType();
                if (AllCategories != null)
                {
                    node.CategoryTree = await _categoryAccess.GetParentChildWiseCategories(AllCategories);
                }
                return View(node);
            }
            else
            {
                var nodeData = await _serviceTypeAccess.GetByKey(Key.Value);
                if (nodeData != null)
                {
                    var catagory = await _serviceCategoryDataAccess.GetServiceCategoryById(nodeData.ServiceId);
                    if (catagory != null)
                    {
                        nodeData.CategoryId = catagory.CategoryId;
                    }
                    var service = await _serviceAccess.GetById(nodeData.ServiceId);
                    var fCatagory = await _featureCategoryAccess.GetById(service.FeatureCategoryId);
                    ViewBag.FeatureCategoryKey = fCatagory.FeatureCategoryKey.ToString();
                    //await CatagoryDropDown(fCatagory.FeatureCategoryKey.ToString());
                    if (!string.IsNullOrEmpty(fCatagory.FeatureCategoryKey.ToString()))
                    {
                        ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagory.FeatureCategoryKey.ToString());
                    }
                    ServiceType node = new ServiceType();
                    if (AllCategories != null)
                    {
                        nodeData.CategoryTree = await _categoryAccess.GetParentChildWiseCategories(AllCategories);
                    }
                    return View(nodeData);
                }
                else
                {
                    return View(new ServiceType());
                }
            }
        }
        [HttpPost]
        //[ValidateInput(false)]
        public async Task<ActionResult> CreateV2(ServiceType model, HttpPostedFileBase Image, List<HttpPostedFileBase> images)
        {
            var db = new BoulevardDbContext();
            if (Image != null)
            {
                model.Image = _serviceTypeAccess.UploadImage(Image);
            }

            if (model.ServiceTypeKey == null || model.ServiceTypeKey == Guid.Empty)
            {
                var serviceData = await _serviceTypeAccess.Insert(model);

                if (images != null)
                {
                    if (images.FirstOrDefault() != null)
                    {
                        foreach (var extrImage in images)
                        {
                            ServiceTypeFile serviceTypeFile = new ServiceTypeFile();
                            serviceTypeFile.ServiceTypeId = serviceData.ServiceTypeId;
                            serviceTypeFile.FileSource = "TypeImage";
                            serviceTypeFile.FileType = "Image";
                            serviceTypeFile.ServiceAmenityId = 0;
                            serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(extrImage);
                            await _serviceTypeFileDataAccess.Create(serviceTypeFile);
                        }
                    }
                    
                }
            }
            else
            {
                var result = await _serviceTypeAccess.Update(model);
                //var service = await _serviceAccess.GetById(result.ServiceId);
                if (images != null && images.FirstOrDefault() != null)
                {
                    //var db = new BoulevardDbContext();
                    var serviceTypeFileNode = await db.ServiceTypeFiles.Where(s => s.ServiceTypeId == result.ServiceTypeId && s.FileSource == "TypeImage").ToListAsync();
                    if (serviceTypeFileNode != null)
                    {
                        foreach (var serviceTypeFile in serviceTypeFileNode)
                        {
                            db.ServiceTypeFiles.Remove(serviceTypeFile);
                            db.SaveChanges();
                        }
                    }
                    foreach (var intrImage in images)
                    {
                        ServiceTypeFile serviceTypeFile = new ServiceTypeFile();
                        serviceTypeFile.ServiceTypeId = result.ServiceTypeId;
                        serviceTypeFile.FileSource = "TypeImage";
                        serviceTypeFile.FileType = "Image";
                        serviceTypeFile.ServiceAmenityId = 0;
                        serviceTypeFile.FileLocation = _serviceTypeFileDataAccess.UploadImage(intrImage);
                        await _serviceTypeFileDataAccess.Create(serviceTypeFile);
                    }
                }

            }
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("IndexV2", "ServiceType", new { fCatagoryKey = model.FeatureCategoryKey });
            }
            else
            {
                return RedirectToAction("IndexV2", "ServiceType");
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
                return await _serviceTypeAccess.Delete(Key.Value, 1);
            }
        }

        [HttpGet]
        public async Task<bool> DeleteImage(int ImageId)
        {

            return await _serviceTypeAccess.DeleteProductImage(ImageId);

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


        #region Bulk Upload

        public async Task<ActionResult> AddServiceTypeBulk(string message = "", string fCatagoryKey = "")
        {
            var tempService = _tempServiceTypeDataAccess.GetTempServiceTypeCount();
            ViewBag.NewRecord = tempService.TotalCount;
            ViewBag.Message = message;
            ViewBag.FCatagoryKey = fCatagoryKey;
            tempService.fCatagoryKey = fCatagoryKey;
            return View(tempService);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddServiceTypeBulk(TempProductCountViewModel model)
        {
            var Password = Request.Form["Password"] ?? "";
            _tempServiceTypeDataAccess.DeleteTempServicesType();
            if (Request.Files.Count > 0)
            {
                try
                {
                    var feacherCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.fCatagoryKey));

                    StringBuilder queryString = new StringBuilder();
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();
                    int readDone = 0;
                    if (Request.Files.Count > 0)
                    {
                        var file1 = Request.Files[0];
                        string fileExtension = Path.GetExtension(file1.FileName);

                        if (fileExtension == ".xls" || fileExtension == ".xlsx")
                        {
                            string rootpath = "/Content/uploads/ExcelFiles";

                            var generator = new Random();
                            var randKey = generator.Next(0, 1000000).ToString("D6");
                            var fileName = randKey + "_image_" + Regex.Replace(file1.FileName, @"\s+", "");
                            var path = Path.Combine(Server.MapPath("~" + rootpath), fileName);

                            if (!Directory.Exists(Server.MapPath("~" + rootpath)))
                            {
                                Directory.CreateDirectory(Server.MapPath("~" + rootpath));
                            }
                            file1.SaveAs(path);
                            string filePath = rootpath + fileName;

                            string excelConnectionString = string.Empty;
                            if (!string.IsNullOrEmpty(Password))
                            {
                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Password=" + Password + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            }
                            else
                            {
                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }

                            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                            excelConnection.Open();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return RedirectToAction(nameof(AddServiceTypeBulk), new { message = "No File Found." });
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }

                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                            {
                                dataAdapter.Fill(ds);
                                excelConnection.Close();
                            }

                            var dataTable = ds.Tables[0];

                            #region Check Excel Column
                            TempServiceType tempServiceType = new TempServiceType();
                            //product.SrNo = dataTable.Rows[0]["Sr.No"].ToString();
                            tempServiceType.ServiceName = dataTable.Rows[0]["ServiceName"].ToString();
                            tempServiceType.ServiceTypeName = dataTable.Rows[0]["ServiceTypeName"].ToString();
                            tempServiceType.PersoneQuantity = dataTable.Rows[0]["PersoneQuantity"].ToString();
                            tempServiceType.Description = dataTable.Rows[0]["Description"].ToString();
                            tempServiceType.Size = dataTable.Rows[0]["Size"].ToString();
                            tempServiceType.Images = dataTable.Rows[0]["Images"].ToString();
                            tempServiceType.Price = dataTable.Rows[0]["Price"].ToString();
                            tempServiceType.AdultQuantity = dataTable.Rows[0]["AdultQuantity"].ToString();
                            tempServiceType.ChildrenQuantity = dataTable.Rows[0]["ChildrenQuantity"].ToString();
                            tempServiceType.PaymentType = dataTable.Rows[0]["PaymentType"].ToString();
                            tempServiceType.ServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                            tempServiceType.ServiceMinutes = dataTable.Rows[0]["ServiceMinutes"].ToString();
                            tempServiceType.FileType = dataTable.Rows[0]["FileType"].ToString();
                            tempServiceType.File = dataTable.Rows[0]["File"].ToString();
                            tempServiceType.AmenitiesName = dataTable.Rows[0]["AmenitiesName"].ToString();
                            tempServiceType.AmenitiesLogo = dataTable.Rows[0]["AmenitiesLogo"].ToString();
                            tempServiceType.AmenitiesType = dataTable.Rows[0]["AmenitiesType"].ToString();



                            #endregion


                            int counter = 0;
                            foreach (DataRow objDataRow in dataTable.Rows)
                            {
                                if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                    continue;
                                counter++;
                            }

                            List<TempServiceType> list = new List<TempServiceType>();
                            List<TempServiceType> Duplicatelist = new List<TempServiceType>();
                            string productModel = string.Empty;

                            StringBuilder xmlStore = new StringBuilder();

                            using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                            {
                                writer.WriteStartElement("Root");

                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    TempServiceType tempService = new TempServiceType();
                                    tempServiceType.ServiceName = objDataRow["ServiceName"].ToString().Trim();
                                    tempServiceType.ServiceTypeName = objDataRow["ServiceTypeName"].ToString().Trim();
                                    tempServiceType.PersoneQuantity = objDataRow["PersoneQuantity"].ToString().Trim();
                                    tempServiceType.Description = objDataRow["Description"].ToString().Trim();
                                    tempServiceType.Size = objDataRow["Size"].ToString().Trim();
                                    tempServiceType.Images = objDataRow["Images"].ToString().Trim();
                                    tempServiceType.Price = objDataRow["Price"].ToString().Trim();
                                    tempServiceType.AdultQuantity = objDataRow["AdultQuantity"].ToString().Trim();
                                    tempServiceType.ChildrenQuantity = objDataRow["ChildrenQuantity"].ToString().Trim();
                                    tempServiceType.PaymentType = objDataRow["PaymentType"].ToString().Trim();
                                    tempServiceType.ServiceHour = objDataRow["ServiceHour"].ToString().Trim();
                                    tempServiceType.ServiceMinutes = objDataRow["ServiceMinutes"].ToString().Trim();
                                    tempServiceType.FileType = objDataRow["FileType"].ToString().Trim();
                                    tempServiceType.File = objDataRow["File"].ToString().Trim();
                                    tempServiceType.AmenitiesName = objDataRow["AmenitiesName"].ToString().Trim();
                                    tempServiceType.AmenitiesLogo = objDataRow["AmenitiesLogo"].ToString().Trim();
                                    tempServiceType.AmenitiesType = objDataRow["AmenitiesType"].ToString().Trim();
                                    tempServiceType.ExcelCount = counter;
                                    list.Add(tempServiceType);

                                    // Xml File Create
                                    writer.WriteStartElement("ServiceType");
                                    writer.WriteAttributeString("serviceName", tempServiceType.ServiceName ?? " ");
                                    writer.WriteAttributeString("serviceTypeName", tempServiceType.ServiceTypeName ?? " ");
                                    writer.WriteAttributeString("personeQuantity", tempServiceType.PersoneQuantity ?? " ");
                                    writer.WriteAttributeString("description", tempServiceType.Description ?? " ");
                                    writer.WriteAttributeString("size", tempServiceType.Size ?? " ");
                                    writer.WriteAttributeString("images", tempServiceType.Images ?? " ");
                                    writer.WriteAttributeString("price", tempServiceType.Price ?? " ");
                                    writer.WriteAttributeString("adultQuantity", tempServiceType.AdultQuantity ?? " ");
                                    writer.WriteAttributeString("childrenQuantity", tempServiceType.ChildrenQuantity ?? " ");
                                    writer.WriteAttributeString("paymentType", tempServiceType.PaymentType ?? " ");
                                    writer.WriteAttributeString("serviceHour", tempServiceType.ServiceHour ?? " ");
                                    writer.WriteAttributeString("serviceMinutes", tempServiceType.ServiceMinutes ?? " ");
                                    writer.WriteAttributeString("fileType", tempServiceType.FileType ?? " ");
                                    writer.WriteAttributeString("file", tempServiceType.File ?? " ");
                                    writer.WriteAttributeString("amenitiesName", tempServiceType.AmenitiesName ?? " ");
                                    writer.WriteAttributeString("amenitiesLogo", tempServiceType.AmenitiesLogo ?? " ");
                                    writer.WriteAttributeString("amenitiesType", tempServiceType.AmenitiesType ?? " ");
                                    writer.WriteAttributeString("excelCount", tempServiceType.ExcelCount.ToString() ?? "0");
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }

                            if (list.Count() > 0)
                            {
                                await _tempServiceTypeDataAccess.AddTempServiceType(xmlStore.ToString(), feacherCategory.FeatureCategoryId);
                                return RedirectToAction(nameof(AddServiceTypeBulk), new { message = "", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });
                            }
                            else
                            {
                                return RedirectToAction(nameof(AddServiceTypeBulk), new { message = "List is Empty", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });
                            }

                        }
                        else
                        {
                            return RedirectToAction(nameof(AddServiceTypeBulk), new { message = "Invalid File Formate. Please Upload .xls or .xlsx format.", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(AddServiceTypeBulk), new { message = "No File Found.", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction(nameof(AddServiceTypeBulk), new { message = ex.Message });
                }
            }
            else
            {
                return RedirectToAction(nameof(AddServiceTypeBulk), new { message = "No File Found." });
            }
        }

        public async Task<ActionResult> DeleteServiceType(string fCatagoryKey)
        {
            _tempServiceTypeDataAccess.DeleteTempServicesType();
            return RedirectToAction("AddServiceTypeBulk", new { message = "", fCatagoryKey = fCatagoryKey });
        }

        public async Task<ActionResult> UpdateAllTemptoServiceType(string fCatagoryKey)
        {
            try
            {
                var feacherCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
                _tempServiceTypeDataAccess.AddServiceType(feacherCategory.FeatureCategoryId);
                _tempServiceTypeDataAccess.DeleteTempServicesType();
                return RedirectToAction("AddServiceTypeBulk", new { message = "All Records Successfully Added to Member.", fCatagoryKey = fCatagoryKey });
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<JsonResult> ExcelImportCount()
        {
            var tempProduct = _tempServiceTypeDataAccess.GetTempServiceTypeCount();
            var record = new
            {
                doneCount = tempProduct.DoneCount,
                totalCount = tempProduct.TotalCount,
                totalNew = tempProduct.TotalNew,
                totalDuplicate = tempProduct.TotalDuplicate
            };
            return Json(record, JsonRequestBehavior.AllowGet);
        }




        #endregion

        [HttpGet]
        public async Task<ActionResult> GetPagedServiceTypes(string fCatagoryKey, string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var query = _context.ServiceTypes.AsQueryable();
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await _context.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(st => st.Service.FeatureCategoryId == fcId);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(st => st.ServiceTypeName.ToLower().Contains(term));
                }
                var rows = await query
                    .OrderByDescending(st => st.ServiceTypeId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(st => new {
                        serviceTypeKey = st.ServiceTypeKey,
                        serviceTypeName = st.ServiceTypeName,
                        serviceName = st.Service.Name,
                        personQuantity = st.PersoneQuantity,
                        adultQuantity = st.AdultQuantity,
                        childrenQuantity = st.ChildrenQuantity,
                        size = st.Size,
                        price = st.Price,
                        image = st.Image
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
        public async Task<ActionResult> GetPagedServiceTypesCount(string fCatagoryKey, string searchTerm)
        {
            try
            {
                var query = _context.ServiceTypes.AsQueryable();
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await _context.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(st => st.Service.FeatureCategoryId == fcId);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(st => st.ServiceTypeName.ToLower().Contains(term));
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

    }
}