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
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics.Metrics;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Packaging;
using System.Drawing.Drawing2D;

namespace Boulevard.Areas.Admin.Controllers
{
    public class ServiceController : BaseController
    {
        private AirportDataAccess _airportDataAccess;
        private AirportServiceDataAccess _airportServiceDataAccess;
        private ServiceAccess _serviceAccess;
        private FeatureCategoryAccess _featureCategoryAccess;
        private CountryDataAccess _countryDataAccess;
        private CityDataAccess _cityDataAccess;
        private readonly CategoryAccess _categoryAccess;
        private readonly ServiceCategoryDataAccess _serviceCategoryDataAccess;
        private readonly TempServiceDataAccess _tempServiceDataAccess;
        private BoulevardDbContext _context;

        public ServiceController()
        {
            _airportDataAccess = new AirportDataAccess();
            _airportServiceDataAccess = new AirportServiceDataAccess();
            _serviceAccess = new ServiceAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _countryDataAccess = new CountryDataAccess();
            _cityDataAccess = new CityDataAccess();
            _categoryAccess = new CategoryAccess();
            _serviceCategoryDataAccess = new ServiceCategoryDataAccess();
            _tempServiceDataAccess = new TempServiceDataAccess();
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

        [HttpGet]
        public async Task<ActionResult> GetPagedServices(string fCatagoryKey, string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Services.Where(s => s.Status == "Active");

                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await _context.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(s => s.FeatureCategoryId == fcId);
                }

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(s => s.Name.ToLower().Contains(term));
                }

                int total = await query.CountAsync();

                var rows = await query
                    .OrderByDescending(s => s.ServiceId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new
                    {
                        s.ServiceKey,
                        s.Name,
                        s.ServiceHour,
                        s.Address,
                        s.Ratings
                    })
                    .ToListAsync();

                return Json(new { success = true, total, rows }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> IndexForChildService(string fCatagoryKey)
        {
            ViewBag.FCatagoryKey = fCatagoryKey;
            if (!string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetPagedChildServices(string fCatagoryKey, string searchTerm, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 10) pageSize = 10;
                if (pageSize > 500) pageSize = 500;
                var query = _context.Services.Where(s => s.ParentId > 0 && s.Status == "Active");
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await _context.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(s => s.FeatureCategoryId == fcId);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(s => s.Name.ToLower().Contains(term));
                }
                var ctx = _context;
                var rows = await query
                    .OrderByDescending(s => s.ServiceId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new {
                        serviceKey = s.ServiceKey,
                        parentServiceName = ctx.Services.Where(p => p.ServiceId == s.ParentId).Select(p => p.Name).FirstOrDefault(),
                        name = s.Name,
                        status = s.Status
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
        public async Task<ActionResult> GetPagedChildServicesCount(string fCatagoryKey, string searchTerm)
        {
            try
            {
                var query = _context.Services.Where(s => s.ParentId > 0 && s.Status == "Active");
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out Guid fcGuid))
                {
                    var fcId = await _context.featureCategories
                        .Where(f => f.FeatureCategoryKey == fcGuid)
                        .Select(f => f.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fcId > 0)
                        query = query.Where(s => s.FeatureCategoryId == fcId);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    query = query.Where(s => s.Name.ToLower().Contains(term));
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
        public async Task<ActionResult> MotorServiceIndex()
        {
            List<Boulevard.Models.Service> list = new List<Boulevard.Models.Service>();
            Boulevard.Models.Service obj1 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "Bicycle Fix",
                Description = "We provide easy bicycle repair solutions at your doorsteps at your convenience",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Fujairah",
                Ratings = 4.5,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Service/rp1.jpg"
            };
            Boulevard.Models.Service obj2 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "Doctor Bikes",
                Description = "DAT Bike Shop having experience of 35 years in Bicycle Repair Dubai, Old Bicycle Repair in Dubai, Bicycle and Bike Service in Dubai, Sharjah, Ajman and all through UAE",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Ajman",
                Ratings = 4.1,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Service/rp2.png"
            };
            Boulevard.Models.Service obj3 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "Cosmo Bikers",
                Description = "DAT Bike Shop having experience of 35 years in Bicycle Repair Dubai, Old Bicycle Repair in Dubai, Bicycle and Bike Service in Dubai, Sharjah, Ajman and all through UAE",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Sharjah",
                Ratings = 4.2,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Service/rp3.png"
            };
            Boulevard.Models.Service obj4 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "Cleaver Fixer",
                Description = "DAT Bike Shop having experience of 35 years in Bicycle Repair Dubai, Old Bicycle Repair in Dubai, Bicycle and Bike Service in Dubai, Sharjah, Ajman and all through UAE",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Dubai",
                Ratings = 5,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Service/rp4.png"
            };
            Boulevard.Models.Service obj5 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "Orgenja Bikers",
                Description = "DAT Bike Shop having experience of 35 years in Bicycle Repair Dubai, Old Bicycle Repair in Dubai, Bicycle and Bike Service in Dubai, Sharjah, Ajman and all through UAE",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Abu Dhabi",
                Ratings = 3.8,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Service/rp5.jpeg"
            };
            list.Add(obj1);
            list.Add(obj2);
            list.Add(obj3);
            list.Add(obj4);
            list.Add(obj5);
            return View(list);
        }
        [HttpGet]
        public async Task<ActionResult> Create(Guid? Key, string fCatagoryKey)
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

            //await CatagoryDropDown(fCatagoryKey);

            //ViewBag.countries = new SelectList(await _countryDataAccess.GetAll(), "CountryId", "CountryName");
            //ViewBag.City = new SelectList(await _cityDataAccess.GetAll(), "CityId", "CityName");
            ViewBag.propertyType = new List<SelectListItem>{ new SelectListItem{
                    Text="Hotel",
                    Value = "Hotel"
                },
                new SelectListItem{
                    Text="Apartment",
                    Value = "Apartment"
                }};
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
                if (fCatagoryKey.ToUpper() == "2CDCEBCF-BA2C-4C2C-9C53-F32C06233FFD")
                {
                    node.Airports = await _airportDataAccess.GetAll();

                }
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
                    if (fCatagoryKey.ToUpper() == "2CDCEBCF-BA2C-4C2C-9C53-F32C06233FFD")
                    {
                        List<Airport> AllAirports = await _airportDataAccess.GetAll();
                        var airportSerice = await _airportServiceDataAccess.GetByServiceId(node.ServiceId);
                        foreach (Airport obj in AllAirports)
                        {
                            if (airportSerice.Any(p => p.AirportId == obj.AirportId))
                            {
                                obj.IsSelected = true;
                            }
                        }
                        node.Airports = AllAirports;
                    }
                    FeatureCategory featureCategory = await _featureCategoryAccess.GetById(node.FeatureCategoryId);
                    ViewBag.FeatureCategoryKey = featureCategory.FeatureCategoryKey.ToString();
                    if (!string.IsNullOrEmpty(fCatagoryKey))
                    {
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


        [HttpGet]
        public async Task<ActionResult> CreateMotorTest(Guid? Key)
        {
            ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAll(), "FeatureCategoryId", "Name");
            ViewBag.countries = new SelectList(await _countryDataAccess.GetAll(), "CountryId", "CountryName");
            ViewBag.propertyType = new List<SelectListItem>{ new SelectListItem{
                    Text="Hotel",
                    Value = "Hotel"
                },
                new SelectListItem{
                    Text="Apartment",
                    Value = "Apartment"
                }};
            if (Key == null || Key == Guid.Empty)
            {
                Boulevard.Models.Service node = new Boulevard.Models.Service();
                node.ServiceImages = new List<ServiceImage> { new ServiceImage() };
                return View(node);
            }
            else
            {

                Boulevard.Models.Service node = new Boulevard.Models.Service()
                {
                    ServiceKey = Guid.NewGuid(),
                    Name = "Orgenja Bikers",
                    Description = "DAT Bike Shop having experience of 35 years in Bicycle Repair Dubai, Old Bicycle Repair in Dubai, Bicycle and Bike Service in Dubai, Sharjah, Ajman and all through UAE",
                    ServiceHour = 13,
                    FeatureCategoryId = 1,
                    CityId = 1,
                    Address = "Abu Dhabi",
                    Ratings = 3.8,
                    Latitute = "25.2048",
                    Logitute = "55.2708",
                    SpokenLanguages = "Arabic,English,Bangla"
                };

                node.ServiceImages = new List<ServiceImage>();
                ServiceImage img = new ServiceImage()
                {
                    Image = "/Content/Service/rp1.jpg"
                };
                node.ServiceImages.Add(img);
                return View(node);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(Boulevard.Models.Service model, List<HttpPostedFileBase> Images)
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
                var serviceData = await _serviceAccess.Insert(model);
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
                var result = await _serviceAccess.Update(model);
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
            if (model.Airports != null && model.Airports.Count > 0)
            {
                if (model.ServiceKey != Guid.Empty)
                {
                    await _airportServiceDataAccess.RemoveByServiceId(model.ServiceId);
                }
                await _airportServiceDataAccess.InsertByServiceId(model.Airports,model.ServiceId);
            }
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("Index", "Service", new { fCatagoryKey = model.FeatureCategoryKey });
            }
            else
            {
                return RedirectToAction("Index", "Service");
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

        public JsonResult GetCityByCountryId(int countryId)
        {
            var db = new BoulevardDbContext();

            var cities = db.Cities.Where(t => t.CountryId == countryId).ToList();


            return this.Json(cities,
                       JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> DeleteImage(int ImageId)
        {

            return Json(await _serviceAccess.DeleteProductImage(ImageId), JsonRequestBehavior.AllowGet);


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



        #region Bulk Upload

        public async Task<ActionResult> AddServiceBulk(string message = "", string fCatagoryKey = "",bool isPackage=false)
        {
            var tempService = _tempServiceDataAccess.GetTempServiceCount();
            ViewBag.NewRecord = tempService.TotalCount;
            ViewBag.Message = message;
            ViewBag.FCatagoryKey = fCatagoryKey;
            ViewBag.IsPackage = isPackage.ToString();

            tempService.fCatagoryKey = fCatagoryKey;
            tempService.IsPackage = isPackage;
            return View(tempService);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddServiceBulk(TempProductCountViewModel model)
        {
            var Password = Request.Form["Password"] ?? "";
            _tempServiceDataAccess.DeleteTempServices();
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
                                return RedirectToAction(nameof(AddServiceBulk), new { message = "No File Found.", fCatagoryKey = model.fCatagoryKey.ToString(), isPackage = model.IsPackage });
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
                            List<TempService> list = new List<TempService>();
                            List<TempService> Duplicatelist = new List<TempService>();
                            string productModel = string.Empty;

                            StringBuilder xmlStore = new StringBuilder();
                            var dataTable = ds.Tables[0];
                            if (model.fCatagoryKey == "B3E3E680-C8EF-4AB2-A4AC-D75BB48A3647".ToLower() || model.fCatagoryKey == "25D8C418-2D26-4159-9D7F-970E3B933B42".ToLower() || model.fCatagoryKey == "BBC98E2D-941B-44C6-8122-0E12A2645B87".ToLower())
                            {
                                #region Check Excel Column
                                TempService tempService = new TempService();
                                //product.SrNo = dataTable.Rows[0]["Sr.No"].ToString();
                                tempService.Name = dataTable.Rows[0]["Company Name"].ToString();
                                tempService.NameAr = dataTable.Rows[0]["Company Name Arabic"].ToString();

                                //tempService.Description = dataTable.Rows[0]["Description-html"].ToString();
                                //tempService.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();

                                tempService.Category = dataTable.Rows[0]["Category"].ToString();
                                tempService.CategoryArabic = dataTable.Rows[0]["Category Arabic"].ToString();
                                tempService.CategoryImage = dataTable.Rows[0]["Category Image"].ToString();
                                tempService.CategoryIcon = dataTable.Rows[0]["Category Icon"].ToString();
                                tempService.SubCategory = dataTable.Rows[0]["SubCategory"].ToString();
                                tempService.SubCategoryArabic = dataTable.Rows[0]["SubCategory Arabic"].ToString();
                                tempService.SubCategoryImage = dataTable.Rows[0]["SubCategory Image"].ToString();
                                tempService.Country = dataTable.Rows[0]["Country Name"].ToString();
                                tempService.City = dataTable.Rows[0]["City Name"].ToString();
                                tempService.Address = dataTable.Rows[0]["Address"].ToString();
                                //tempService.AboutUs = dataTable.Rows[0]["AboutUs-html"].ToString();
                                //tempService.AboutUsAr = dataTable.Rows[0]["Aboutus Arabic-html"].ToString();
                                tempService.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                                tempService.Languages = dataTable.Rows[0]["Languages"].ToString();
                                //tempService.ScopeofService = dataTable.Rows[0]["Scope of Service-html"].ToString();
                                //tempService.ScopeOfServiceAr = dataTable.Rows[0]["Scope of Service Arabic-html"].ToString();
                                tempService.Images = dataTable.Rows[0]["Images"].ToString();
                                tempService.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                                tempService.Longitute = dataTable.Rows[0]["Longitute"].ToString();
                                tempService.CheckInTime = dataTable.Rows[0]["Service Open"].ToString();
                                tempService.CheckInTime = dataTable.Rows[0]["Service Close"].ToString();
                                //tempService.FaqTitle = dataTable.Rows[0]["FAQ Title"].ToString();
                                //tempService.FaqTitleAr = dataTable.Rows[0]["FAQ Title Arabic"].ToString();
                                //tempService.FaqDescription = dataTable.Rows[0]["FAQ Description"].ToString();
                                //tempService.FaqDescriptionAr = dataTable.Rows[0]["FAQ Description Arabic"].ToString();
                                tempService.ServiceTypeName = dataTable.Rows[0]["Service Type Name"].ToString();
                                tempService.ServiceTypeNameAr = dataTable.Rows[0]["Service Type Name Arabic"].ToString();
                                tempService.PersoneQuantity = dataTable.Rows[0]["PersoneQuantity"].ToString();

                                tempService.TypeDescription = dataTable.Rows[0]["Service Type Description"].ToString();

                                tempService.TypeDescriptionAr = dataTable.Rows[0]["Service Type Description Arabic"].ToString();
                                tempService.TypeImage = dataTable.Rows[0]["Service Type Images"].ToString();
                                tempService.TypePrice = dataTable.Rows[0]["Price"].ToString();

                                tempService.TypeServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                                tempService.TypeServiceMin = dataTable.Rows[0]["Service Min"].ToString();

                                tempService.ServiceTypeBigDescription = dataTable.Rows[0]["Service Type Big Details"].ToString();
                                tempService.ServiceTypeBigDescriptionArabic = dataTable.Rows[0]["Service Type Big Details Arabic"].ToString();
                                #endregion


                                int counter = 0;
                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    counter++;
                                }




                                using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                                {
                                    writer.WriteStartElement("Root");

                                    foreach (DataRow objDataRow in dataTable.Rows)
                                    {
                                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                            continue;
                                        TempService data = new TempService();
                                        //product.SrNo = dataTable.Rows[0]["Sr.No"].ToString();
                                        data.Name = objDataRow["Company Name"].ToString();
                                        data.NameAr = objDataRow["Company Name Arabic"].ToString();

                                        //data.Description = objDataRow["Description-html"].ToString();
                                        //data.DescriptionAr = objDataRow["Description Arabic-html"].ToString();

                                        data.Category = objDataRow["Category"].ToString();
                                        data.CategoryArabic = objDataRow["Category Arabic"].ToString();
                                        data.CategoryImage = objDataRow["Category Image"].ToString();
                                        data.CategoryIcon = objDataRow["Category Icon"].ToString();
                                        data.SubCategory = objDataRow["SubCategory"].ToString();
                                        data.SubCategoryArabic = objDataRow["SubCategory Arabic"].ToString();
                                        data.SubCategoryImage = objDataRow["SubCategory Image"].ToString();
                                        data.Country = objDataRow["Country Name"].ToString();
                                        data.City = objDataRow["City Name"].ToString();
                                        data.Address = objDataRow["Address"].ToString();
                                        //data.AboutUs = objDataRow["AboutUs-html"].ToString();
                                        //data.AboutUsAr = objDataRow["Aboutus Arabic-html"].ToString();
                                        data.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                                        data.Languages = objDataRow["Languages"].ToString();
                                        //data.ScopeofService = objDataRow["Scope of Service-html"].ToString();
                                        //data.ScopeOfServiceAr = objDataRow["Scope of Service Arabic-html"].ToString();
                                        data.Images = objDataRow["Images"].ToString();
                                        data.Latitute = objDataRow["Latitute"].ToString();
                                        data.Longitute = objDataRow["Longitute"].ToString();
                                        data.CheckInTime = objDataRow["Service Open"].ToString();
                                        data.CheckInTime = objDataRow["Service Close"].ToString();
                                        //data.FaqTitle = objDataRow["FAQ Title"].ToString();
                                        //data.FaqTitleAr = objDataRow["FAQ Title Arabic"].ToString();
                                        //data.FaqDescription = objDataRow["FAQ Description"].ToString();
                                        //data.FaqDescriptionAr = objDataRow["FAQ Description Arabic"].ToString();
                                        data.ServiceTypeName = objDataRow["Service Type Name"].ToString();
                                        data.ServiceTypeNameAr = objDataRow["Service Type Name Arabic"].ToString();
                                        data.PersoneQuantity = objDataRow["PersoneQuantity"].ToString();

                                        data.TypeDescription = objDataRow["Service Type Description"].ToString();

                                        data.TypeDescriptionAr = objDataRow["Service Type Description Arabic"].ToString();
                                        data.TypeImage = objDataRow["Service Type Images"].ToString();
                                        data.TypePrice = objDataRow["Price"].ToString();

                                        data.TypeServiceHour = objDataRow["ServiceHour"].ToString();
                                        data.TypeServiceMin = objDataRow["Service Min"].ToString();

                                        data.ServiceTypeBigDescription = objDataRow["Service Type Big Details"].ToString();
                                        data.ServiceTypeBigDescriptionArabic = objDataRow["Service Type Big Details Arabic"].ToString();
                                        data.ExcelCount = counter;
                                        list.Add(data);

                                        // Xml File Create
                                        writer.WriteStartElement("Service");


                                        writer.WriteAttributeString("name", data.Name ?? "");
                                        writer.WriteAttributeString("nameAr", data.NameAr ?? "");

                                        writer.WriteAttributeString("description", data.Description ?? "");
                                        writer.WriteAttributeString("descriptionAr", data.DescriptionAr ?? "");

                                        writer.WriteAttributeString("category", data.Category ?? "");
                                        writer.WriteAttributeString("categoryArabic", data.CategoryArabic ?? "");
                                        writer.WriteAttributeString("categoryIcon", data.CategoryIcon ?? "");
                                        writer.WriteAttributeString("categoryImage", data.CategoryImage ?? "");

                                        writer.WriteAttributeString("subCategory", data.SubCategory ?? "");
                                        writer.WriteAttributeString("subCategoryArabic", data.SubCategoryArabic ?? "");
                                        writer.WriteAttributeString("subCategoryImage", data.SubCategoryImage ?? "");
                                        writer.WriteAttributeString("subCategoryIcon", data.SubCategoryIcon ?? "");


                                        writer.WriteAttributeString("country", data.Country ?? "");
                                        writer.WriteAttributeString("city", data.City ?? "");
                                        writer.WriteAttributeString("address", data.Address ?? "");

                                        writer.WriteAttributeString("aboutUs", data.AboutUs ?? "");
                                        writer.WriteAttributeString("aboutUsAr", data.AboutUsAr ?? "");

                                        writer.WriteAttributeString("languages", data.Languages ?? "");

                                        writer.WriteAttributeString("scopeOfService", data.ScopeofService ?? "");
                                        writer.WriteAttributeString("scopeOfServiceAr", data.ScopeOfServiceAr ?? "");

                                        writer.WriteAttributeString("images", data.Images ?? "");

                                        writer.WriteAttributeString("latitude", data.Latitute ?? "");
                                        writer.WriteAttributeString("longitude", data.Longitute ?? "");

                                        writer.WriteAttributeString("checkInTime", data.CheckInTime ?? "");
                                        writer.WriteAttributeString("checkOutTime", data.CheckOutTime ?? "");

                                        writer.WriteAttributeString("faqTitle", data.FaqTitle ?? "");
                                        writer.WriteAttributeString("faqTitleAr", data.FaqTitleAr ?? "");

                                        writer.WriteAttributeString("faqDescription", data.FaqDescription ?? "");
                                        writer.WriteAttributeString("faqDescriptionAr", data.FaqDescriptionAr ?? "");

                                        writer.WriteAttributeString("serviceTypeName", data.ServiceTypeName ?? "");
                                        writer.WriteAttributeString("serviceTypeNameAr", data.ServiceTypeNameAr ?? "");

                                        writer.WriteAttributeString("personQuantity", data.PersoneQuantity ?? "");

                                        writer.WriteAttributeString("typeDescription", data.TypeDescription ?? "");
                                        writer.WriteAttributeString("typeDescriptionAr", data.TypeDescriptionAr ?? "");

                                        writer.WriteAttributeString("typeImage", data.TypeImage ?? "");
                                        writer.WriteAttributeString("typePrice", data.TypePrice ?? "");

                                        writer.WriteAttributeString("typeServiceHour", data.TypeServiceHour ?? "");
                                        writer.WriteAttributeString("typeServiceMin", data.TypeServiceMin ?? "");

                                        writer.WriteAttributeString("serviceTypeBigDescriptionArabic", data.ServiceTypeBigDescriptionArabic ?? "");
                                        writer.WriteAttributeString("serviceTypeBigDescription", data.ServiceTypeBigDescription ?? "");
                                        writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");
                                        writer.WriteEndElement();
                                    }
                                    writer.WriteEndElement();
                                }

                            }


                            //else if (model.fCatagoryKey == "25D8C418-2D26-4159-9D7F-970E3B933B42".ToLower())
                            //{
                            //    #region Check Excel Column
                            //    TempService tempService = new TempService();
                            //    //product.SrNo = dataTable.Rows[0]["Sr.No"].ToString();
                            //    tempService.Name = dataTable.Rows[0]["Salon CompanyName"].ToString();
                            //    tempService.NameAr = dataTable.Rows[0]["Salon company Name Arabic"].ToString();

                            //    tempService.Description = dataTable.Rows[0]["Description-html"].ToString();
                            //    tempService.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();

                            //    tempService.Category = dataTable.Rows[0]["Category"].ToString();
                            //    tempService.CategoryArabic = dataTable.Rows[0]["Category Arabic"].ToString();

                            //    tempService.Country = dataTable.Rows[0]["Country Name"].ToString();
                            //    tempService.City = dataTable.Rows[0]["City Name"].ToString();
                            //    tempService.Address = dataTable.Rows[0]["Address"].ToString();

                            //    tempService.AboutUs = dataTable.Rows[0]["AboutUs-html"].ToString();
                            //    tempService.AboutUsAr = dataTable.Rows[0]["AboutUs Arabic-html"].ToString();

                            //    tempService.Languages = dataTable.Rows[0]["Languages"].ToString();
                            //    tempService.ScopeofService = dataTable.Rows[0]["Scope of Service -html"].ToString();
                            //    tempService.ScopeOfServiceAr = dataTable.Rows[0]["Scope of Service Arabic-html"].ToString();

                            //    tempService.Images = dataTable.Rows[0]["Images"].ToString();
                            //    tempService.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                            //    tempService.Longitute = dataTable.Rows[0]["Longitute"].ToString();

                            //    tempService.CheckInTime = dataTable.Rows[0]["Service Open"].ToString();
                            //    tempService.CheckOutTime = dataTable.Rows[0]["Service Close"].ToString();

                            //    tempService.FaqTitle = dataTable.Rows[0]["FAQ Title"].ToString();
                            //    tempService.FaqTitleAr = dataTable.Rows[0]["FAQ Title Arabic "].ToString();
                            //    tempService.FaqDescription = dataTable.Rows[0]["FAQ Description"].ToString();
                            //    tempService.FaqDescriptionAr = dataTable.Rows[0]["FAQ Description Arabic"].ToString();

                            //    tempService.ServiceTypeName = dataTable.Rows[0]["Salon Service Type Name"].ToString();
                            //    tempService.ServiceTypeNameAr = dataTable.Rows[0]["Salon Service Type Name Arabic"].ToString();
                            //    tempService.PersoneQuantity = dataTable.Rows[0]["PersoneQuantity"].ToString();

                            //    tempService.TypeDescription = dataTable.Rows[0]["Description"].ToString();
                            //    tempService.TypeDescriptionAr = dataTable.Rows[0]["Description Arabic"].ToString();
                            //    tempService.TypeImage = dataTable.Rows[0]["Service Type Images"].ToString();
                            //    tempService.TypePrice = dataTable.Rows[0]["Price"].ToString();

                            //    tempService.TypeServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                            //    tempService.TypeServiceMin = dataTable.Rows[0]["Service Min"].ToString();

                            //    #endregion

                            //    int counter = 0;
                            //    foreach (DataRow objDataRow in dataTable.Rows)
                            //    {
                            //        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                            //            continue;
                            //        counter++;
                            //    }
                            //    using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                            //    {
                            //        writer.WriteStartElement("Root");

                            //        foreach (DataRow objDataRow in dataTable.Rows)
                            //        {
                            //            if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                            //                continue;

                            //            TempService data = new TempService();
                            //            //data.SrNo = dataTable.Rows[0]["Sr.No"].ToString();
                            //            data.Name = dataTable.Rows[0]["Salon CompanyName"].ToString();
                            //            data.NameAr = dataTable.Rows[0]["Salon company Name Arabic"].ToString();

                            //            data.Description = dataTable.Rows[0]["Description-html"].ToString();
                            //            data.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();

                            //            data.Category = dataTable.Rows[0]["Category"].ToString();
                            //            data.CategoryArabic = dataTable.Rows[0]["Category Arabic"].ToString();

                            //            data.Country = dataTable.Rows[0]["Country Name"].ToString();
                            //            data.City = dataTable.Rows[0]["City Name"].ToString();
                            //            data.Address = dataTable.Rows[0]["Address"].ToString();

                            //            data.AboutUs = dataTable.Rows[0]["AboutUs-html"].ToString();
                            //            data.AboutUsAr = dataTable.Rows[0]["AboutUs Arabic-html"].ToString();

                            //            data.Languages = dataTable.Rows[0]["Languages"].ToString();
                            //            data.ScopeofService = dataTable.Rows[0]["Scope of Service -html"].ToString();
                            //            data.ScopeOfServiceAr = dataTable.Rows[0]["Scope of Service Arabic-html"].ToString();

                            //            data.Images = dataTable.Rows[0]["Images"].ToString();
                            //            data.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                            //            data.Longitute = dataTable.Rows[0]["Longitute"].ToString();

                            //            data.CheckInTime = dataTable.Rows[0]["Service Open"].ToString();
                            //            data.CheckOutTime = dataTable.Rows[0]["Service Close"].ToString();

                            //            data.FaqTitle = dataTable.Rows[0]["FAQ Title"].ToString();
                            //            data.FaqTitleAr = dataTable.Rows[0]["FAQ Title Arabic "].ToString();
                            //            data.FaqDescription = dataTable.Rows[0]["FAQ Description"].ToString();
                            //            data.FaqDescriptionAr = dataTable.Rows[0]["FAQ Description Arabic"].ToString();

                            //            data.ServiceTypeName = dataTable.Rows[0]["Salon Service Type Name"].ToString();
                            //            data.ServiceTypeNameAr = dataTable.Rows[0]["Salon Service Type Name Arabic"].ToString();
                            //            data.PersoneQuantity = dataTable.Rows[0]["PersoneQuantity"].ToString();

                            //            data.TypeDescription = dataTable.Rows[0]["Description"].ToString();
                            //            data.TypeDescriptionAr = dataTable.Rows[0]["Description Arabic"].ToString();
                            //            data.TypeImage = dataTable.Rows[0]["Service Type Images"].ToString();
                            //            data.TypePrice = dataTable.Rows[0]["Price"].ToString();

                            //            data.TypeServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                            //            data.TypeServiceMin = dataTable.Rows[0]["Service Min"].ToString();

                            //            data.ExcelCount = counter;
                            //            list.Add(data);

                            //            writer.WriteStartElement("Service");

                            //            writer.WriteAttributeString("name", tempService.Name ?? "");
                            //            writer.WriteAttributeString("nameAr", tempService.NameAr ?? "");

                            //            writer.WriteAttributeString("description", tempService.Description ?? "");
                            //            writer.WriteAttributeString("descriptionAr", tempService.DescriptionAr ?? "");

                            //            writer.WriteAttributeString("category", tempService.Category ?? "");
                            //            writer.WriteAttributeString("categoryAr", tempService.CategoryArabic ?? "");

                            //            writer.WriteAttributeString("country", tempService.Country ?? "");
                            //            writer.WriteAttributeString("city", tempService.City ?? "");
                            //            writer.WriteAttributeString("address", tempService.Address ?? "");

                            //            writer.WriteAttributeString("aboutUs", tempService.AboutUs ?? "");
                            //            writer.WriteAttributeString("aboutUsAr", tempService.AboutUsAr ?? "");

                            //            writer.WriteAttributeString("languages", tempService.Languages ?? "");
                            //            writer.WriteAttributeString("scopeOfService", tempService.ScopeofService ?? "");
                            //            writer.WriteAttributeString("scopeOfServiceAr", tempService.ScopeOfServiceAr ?? "");

                            //            writer.WriteAttributeString("images", tempService.Images ?? "");
                            //            writer.WriteAttributeString("latitude", tempService.Latitute ?? "");
                            //            writer.WriteAttributeString("longitude", tempService.Longitute ?? "");

                            //            writer.WriteAttributeString("checkInTime", tempService.CheckInTime ?? "");
                            //            writer.WriteAttributeString("checkOutTime", tempService.CheckOutTime ?? "");

                            //            writer.WriteAttributeString("faqTitle", tempService.FaqTitle ?? "");
                            //            writer.WriteAttributeString("faqTitleAr", tempService.FaqTitleAr ?? "");
                            //            writer.WriteAttributeString("faqDescription", tempService.FaqDescription ?? "");
                            //            writer.WriteAttributeString("faqDescriptionAr", tempService.FaqDescriptionAr ?? "");

                            //            writer.WriteAttributeString("serviceTypeName", tempService.ServiceTypeName ?? "");
                            //            writer.WriteAttributeString("serviceTypeNameAr", tempService.ServiceTypeNameAr ?? "");
                            //            writer.WriteAttributeString("personQuantity", tempService.PersoneQuantity ?? "");

                            //            writer.WriteAttributeString("typeDescription", tempService.TypeDescription ?? "");
                            //            writer.WriteAttributeString("typeDescriptionAr", tempService.TypeDescriptionAr ?? "");
                            //            writer.WriteAttributeString("typeImage", tempService.TypeImage ?? "");
                            //            writer.WriteAttributeString("typePrice", tempService.TypePrice ?? "");

                            //            writer.WriteAttributeString("typeServiceHour", tempService.TypeServiceHour ?? "");
                            //            writer.WriteAttributeString("typeServiceMin", tempService.TypeServiceMin ?? "");
                            //            writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");
                            //            writer.WriteEndElement();
                            //        }

                            //        writer.WriteEndElement();
                            //    }
                            //}
                            //else if (model.fCatagoryKey == "BBC98E2D-941B-44C6-8122-0E12A2645B87".ToLower())
                            //{
                            //    #region Check Excel Column
                            //    TempService tempService = new TempService();
                            //    //product.SrNo = dataTable.Rows[0]["Sr.No"].ToString();
                            //    tempService.Name = dataTable.Rows[0]["Medical Company Name"].ToString();
                            //    tempService.NameAr = dataTable.Rows[0]["Medical Company Name Arabic"].ToString();

                            //    tempService.Description = dataTable.Rows[0]["Description-html"].ToString();
                            //    tempService.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();

                            //    tempService.Category = dataTable.Rows[0]["Category"].ToString();
                            //    tempService.CategoryArabic = dataTable.Rows[0]["Category Arabic"].ToString();

                            //    tempService.Country = dataTable.Rows[0]["Country Name"].ToString();
                            //    tempService.City = dataTable.Rows[0]["City Name"].ToString();
                            //    tempService.Address = dataTable.Rows[0]["Address"].ToString();

                            //    tempService.AboutUs = dataTable.Rows[0]["AboutUs-html"].ToString();
                            //    tempService.AboutUsAr = dataTable.Rows[0]["About us Arabic-html"].ToString();

                            //    tempService.Languages = dataTable.Rows[0]["Languages"].ToString();
                            //    tempService.ScopeofService = dataTable.Rows[0]["Scope of Service-html"].ToString();
                            //    tempService.ScopeOfServiceAr = dataTable.Rows[0]["Scope of Service Arabic-html"].ToString();

                            //    tempService.Images = dataTable.Rows[0]["Images"].ToString();
                            //    tempService.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                            //    tempService.Longitute = dataTable.Rows[0]["Longitute"].ToString();

                            //    tempService.CheckInTime = dataTable.Rows[0]["Service Open"].ToString();
                            //    tempService.CheckOutTime = dataTable.Rows[0]["Service Close"].ToString();

                            //    tempService.FaqTitle = dataTable.Rows[0]["FAQ Title"].ToString();
                            //    tempService.FaqTitleAr = dataTable.Rows[0]["FAQ Title Arabic"].ToString();
                            //    tempService.FaqDescription = dataTable.Rows[0]["FAQ Description"].ToString();
                            //    tempService.FaqDescriptionAr = dataTable.Rows[0]["FAQ Description Arabic"].ToString();

                            //    tempService.ServiceTypeName = dataTable.Rows[0]["Medical Service Type Name"].ToString();
                            //    tempService.ServiceTypeNameAr = dataTable.Rows[0]["Medical Service Type Name Arabic"].ToString();
                            //    tempService.PersoneQuantity = dataTable.Rows[0]["PersoneQuantity"].ToString();

                            //    tempService.TypeDescription = dataTable.Rows[0]["Description"].ToString();
                            //    tempService.TypeDescriptionAr = dataTable.Rows[0]["Description Arabic"].ToString();
                            //    tempService.TypeImage = dataTable.Rows[0]["Service Type Images"].ToString();
                            //    tempService.TypePrice = dataTable.Rows[0]["Price"].ToString();

                            //    tempService.TypeServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                            //    tempService.TypeServiceMin = dataTable.Rows[0]["Service Min"].ToString();
                            //    #endregion

                            //    int counter = 0;
                            //    foreach (DataRow objDataRow in dataTable.Rows)
                            //    {
                            //        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                            //            continue;
                            //        counter++;
                            //    }
                            //    using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                            //    {
                            //        writer.WriteStartElement("Root");

                            //        foreach (DataRow objDataRow in dataTable.Rows)
                            //        {
                            //            if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                            //                continue;

                            //            TempService data = new TempService();
                            //            //data.SrNo = dataTable.Rows[0]["Sr.No"].ToString();
                            //            data.Name = dataTable.Rows[0]["Medical Company Name"].ToString();
                            //            data.NameAr = dataTable.Rows[0]["Medical Company Name Arabic"].ToString();

                            //            data.Description = dataTable.Rows[0]["Description-html"].ToString();
                            //            data.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();

                            //            data.Category = dataTable.Rows[0]["Category"].ToString();
                            //            data.CategoryArabic = dataTable.Rows[0]["Category Arabic"].ToString();

                            //            data.Country = dataTable.Rows[0]["Country Name"].ToString();
                            //            data.City = dataTable.Rows[0]["City Name"].ToString();
                            //            data.Address = dataTable.Rows[0]["Address"].ToString();

                            //            data.AboutUs = dataTable.Rows[0]["AboutUs-html"].ToString();
                            //            data.AboutUsAr = dataTable.Rows[0]["About us Arabic-html"].ToString();

                            //            data.Languages = dataTable.Rows[0]["Languages"].ToString();
                            //            data.ScopeofService = dataTable.Rows[0]["Scope of Service-html"].ToString();
                            //            data.ScopeOfServiceAr = dataTable.Rows[0]["Scope of Service Arabic-html"].ToString();

                            //            data.Images = dataTable.Rows[0]["Images"].ToString();
                            //            data.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                            //            data.Longitute = dataTable.Rows[0]["Longitute"].ToString();

                            //            data.CheckInTime = dataTable.Rows[0]["Service Open"].ToString();
                            //            data.CheckOutTime = dataTable.Rows[0]["Service Close"].ToString();

                            //            data.FaqTitle = dataTable.Rows[0]["FAQ Title"].ToString();
                            //            data.FaqTitleAr = dataTable.Rows[0]["FAQ Title Arabic"].ToString();
                            //            data.FaqDescription = dataTable.Rows[0]["FAQ Description"].ToString();
                            //            data.FaqDescriptionAr = dataTable.Rows[0]["FAQ Description Arabic"].ToString();

                            //            data.ServiceTypeName = dataTable.Rows[0]["Medical Service Type Name"].ToString();
                            //            data.ServiceTypeNameAr = dataTable.Rows[0]["Medical Service Type Name Arabic"].ToString();
                            //            data.PersoneQuantity = dataTable.Rows[0]["PersoneQuantity"].ToString();

                            //            data.TypeDescription = dataTable.Rows[0]["Description"].ToString();
                            //            data.TypeDescriptionAr = dataTable.Rows[0]["Description Arabic"].ToString();
                            //            data.TypeImage = dataTable.Rows[0]["Service Type Images"].ToString();
                            //            data.TypePrice = dataTable.Rows[0]["Price"].ToString();

                            //            data.TypeServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                            //            data.TypeServiceMin = dataTable.Rows[0]["Service Min"].ToString();
                            //            data.ExcelCount = counter;
                            //            list.Add(data);

                            //            writer.WriteStartElement("Service");

                            //            writer.WriteAttributeString("name", tempService.Name ?? "");
                            //            writer.WriteAttributeString("nameAr", tempService.NameAr ?? "");

                            //            writer.WriteAttributeString("description", tempService.Description ?? "");
                            //            writer.WriteAttributeString("descriptionAr", tempService.DescriptionAr ?? "");

                            //            writer.WriteAttributeString("category", tempService.Category ?? "");
                            //            writer.WriteAttributeString("categoryAr", tempService.CategoryArabic ?? "");

                            //            writer.WriteAttributeString("country", tempService.Country ?? "");
                            //            writer.WriteAttributeString("city", tempService.City ?? "");
                            //            writer.WriteAttributeString("address", tempService.Address ?? "");

                            //            writer.WriteAttributeString("aboutUs", tempService.AboutUs ?? "");
                            //            writer.WriteAttributeString("aboutUsAr", tempService.AboutUsAr ?? "");

                            //            writer.WriteAttributeString("languages", tempService.Languages ?? "");
                            //            writer.WriteAttributeString("scopeOfService", tempService.ScopeofService ?? "");
                            //            writer.WriteAttributeString("scopeOfServiceAr", tempService.ScopeOfServiceAr ?? "");

                            //            writer.WriteAttributeString("images", tempService.Images ?? "");
                            //            writer.WriteAttributeString("latitude", tempService.Latitute ?? "");
                            //            writer.WriteAttributeString("longitude", tempService.Longitute ?? "");

                            //            writer.WriteAttributeString("checkInTime", tempService.CheckInTime ?? "");
                            //            writer.WriteAttributeString("checkOutTime", tempService.CheckOutTime ?? "");

                            //            writer.WriteAttributeString("faqTitle", tempService.FaqTitle ?? "");
                            //            writer.WriteAttributeString("faqTitleAr", tempService.FaqTitleAr ?? "");
                            //            writer.WriteAttributeString("faqDescription", tempService.FaqDescription ?? "");
                            //            writer.WriteAttributeString("faqDescriptionAr", tempService.FaqDescriptionAr ?? "");

                            //            writer.WriteAttributeString("serviceTypeName", tempService.ServiceTypeName ?? "");
                            //            writer.WriteAttributeString("serviceTypeNameAr", tempService.ServiceTypeNameAr ?? "");
                            //            writer.WriteAttributeString("personQuantity", tempService.PersoneQuantity ?? "");

                            //            writer.WriteAttributeString("typeDescription", tempService.TypeDescription ?? "");
                            //            writer.WriteAttributeString("typeDescriptionAr", tempService.TypeDescriptionAr ?? "");
                            //            writer.WriteAttributeString("typeImage", tempService.TypeImage ?? "");
                            //            writer.WriteAttributeString("typePrice", tempService.TypePrice ?? "");

                            //            writer.WriteAttributeString("typeServiceHour", tempService.TypeServiceHour ?? "");
                            //            writer.WriteAttributeString("typeServiceMin", tempService.TypeServiceMin ?? "");
                            //            writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");

                            //            writer.WriteEndElement();
                            //        }

                            //        writer.WriteEndElement();
                            //    }
                            //}

                            else if (model.fCatagoryKey == "F4309DF5-9121-41AD-831A-994C46B62766".ToLower()|| model.fCatagoryKey== "C286A46B-5B9A-4519-BB10-8D47EC254FFB".ToLower())
                            {
                                #region Check Excel Column
                                TempService tempService = new TempService();

                                // From Excel Headers
                                tempService.Category = dataTable.Rows[0]["Agency Name"].ToString();
                                tempService.CategoryArabic = dataTable.Rows[0]["Agency Name Arabic"].ToString();
                                tempService.CategoryImage = dataTable.Rows[0]["Agency Image"].ToString();
                                tempService.Name = dataTable.Rows[0]["Service Name"].ToString();
                                tempService.NameAr = dataTable.Rows[0]["Service Name Arabic"].ToString();

                                tempService.ServiceTypeName = dataTable.Rows[0]["Service Type Name"].ToString();
                                tempService.ServiceTypeNameAr = dataTable.Rows[0]["Service Type Name Arabic"].ToString();

                                tempService.SubServiceTypeName = dataTable.Rows[0]["Sub Service Type Name"].ToString();
                                tempService.SubServiceTypeNameAr = dataTable.Rows[0]["Sub Service Type Name Arabic"].ToString();

                                tempService.TypeDescription = dataTable.Rows[0]["General Information-html"].ToString();
                                tempService.TypeDescriptionAr = dataTable.Rows[0]["General Information Arabic-html"].ToString();

                                tempService.ServiceTypeBigDescription = dataTable.Rows[0]["Service Duration -html"].ToString();
                                tempService.ServiceTypeBigDescriptionArabic = dataTable.Rows[0]["Service Duration Arabic -html"].ToString();

                                tempService.ServiceTypePrice = dataTable.Rows[0]["Service Fees"].ToString();
                                tempService.ServiceTypePriceAr = dataTable.Rows[0]["Service Fees Arabic"].ToString();



                                tempService.TypePrice = dataTable.Rows[0]["Price"].ToString();
                                #endregion

                                int counter = 0;
                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    counter++;
                                }
                                using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                                {
                                    writer.WriteStartElement("Root");

                                    foreach (DataRow objDataRow in dataTable.Rows)
                                    {
                                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                            continue;
                                        TempService data = new TempService();

                                        // From Excel Headers
                                        data.Category = objDataRow["Agency Name"].ToString();
                                        data.CategoryArabic = objDataRow["Agency Name Arabic"].ToString();
                                        data.CategoryImage = objDataRow["Agency Image"].ToString();
                                        data.Name = objDataRow["Service Name"].ToString();
                                        data.NameAr = objDataRow["Service Name Arabic"].ToString();

                                        data.ServiceTypeName = objDataRow["Service Type Name"].ToString();
                                        data.ServiceTypeNameAr = objDataRow["Service Type Name Arabic"].ToString();

                                        data.SubServiceTypeName = objDataRow["Sub Service Type Name"].ToString();
                                        data.SubServiceTypeNameAr = objDataRow["Sub Service Type Name Arabic"].ToString();

                                        data.TypeDescription = objDataRow["General Information-html"].ToString();
                                        data.TypeDescriptionAr = objDataRow["General Information Arabic-html"].ToString();

                                        data.ServiceTypeBigDescription = objDataRow["Service Duration -html"].ToString();
                                        data.ServiceTypeBigDescriptionArabic = objDataRow["Service Duration Arabic -html"].ToString();

                                        data.ServiceTypePrice = objDataRow["Service Fees"].ToString();
                                        data.ServiceTypePriceAr = objDataRow["Service Fees Arabic"].ToString();



                                        data.TypePrice = objDataRow["Price"].ToString();
                                        data.ExcelCount = counter;
                                        list.Add(data);
                                        writer.WriteStartElement("Service");
                                        writer.WriteAttributeString("category", data.Category ?? "");
                                        writer.WriteAttributeString("categoryArabic", data.CategoryArabic ?? "");
                                        writer.WriteAttributeString("categoryImage", data.CategoryImage ?? "");

                                        writer.WriteAttributeString("name", data.Name ?? "");
                                        writer.WriteAttributeString("nameAr", data.NameAr ?? "");

                                        writer.WriteAttributeString("serviceTypeName", data.ServiceTypeName ?? "");
                                        writer.WriteAttributeString("serviceTypeNameAr", data.ServiceTypeNameAr ?? "");

                                        writer.WriteAttributeString("subServiceTypeName", data.SubServiceTypeName ?? "");
                                        writer.WriteAttributeString("subServiceTypeNameAr", data.SubServiceTypeNameAr ?? "");

                                        writer.WriteAttributeString("typeDescription", data.TypeDescription ?? "");
                                        writer.WriteAttributeString("typeDescriptionAr", data.TypeDescriptionAr ?? "");

                                        writer.WriteAttributeString("serviceTypeBigDescription", data.ServiceTypeBigDescription ?? "");
                                        writer.WriteAttributeString("serviceTypeBigDescriptionArabic", data.ServiceTypeBigDescriptionArabic ?? "");

                                        writer.WriteAttributeString("serviceTypePrice", data.ServiceTypePrice ?? "");
                                        writer.WriteAttributeString("serviceTypePriceAr", data.ServiceTypePriceAr ?? "");

                                        writer.WriteAttributeString("typePrice", data.TypePrice ?? "");
                                        writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");
                                        writer.WriteEndElement();
                                    }

                                    writer.WriteEndElement();
                                }
                            }

                  


                            else if (model.fCatagoryKey == "6440039B-6E5A-4E65-A0E4-F38B69C46C8C".ToLower() && model.IsPackage == false) // Replace with actual GUID for Hotel Service
                            {
                                #region Check Excel Column - Hotel Service
                                TempService tempService = new TempService();
                                tempService.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                                tempService.Name = dataTable.Rows[0]["Hotel Name"].ToString();
                                tempService.NameAr = dataTable.Rows[0]["Hotel Name Arabic"].ToString();
                                tempService.Description = dataTable.Rows[0]["Description"].ToString();
                                tempService.DescriptionAr = dataTable.Rows[0]["Description Arabic"].ToString();
                                tempService.ServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                                tempService.Category = dataTable.Rows[0]["Category"].ToString();
                                tempService.CategoryArabic = dataTable.Rows[0]["Category arabic"].ToString();
                                tempService.CategoryImage = dataTable.Rows[0]["Category Image"].ToString();
                                tempService.Country = dataTable.Rows[0]["Country Name"].ToString();
                                tempService.City = dataTable.Rows[0]["City Name"].ToString();
                                tempService.Address = dataTable.Rows[0]["Address"].ToString();
                                tempService.AboutUs = dataTable.Rows[0]["AboutUs"].ToString();
                                tempService.AboutUsAr = dataTable.Rows[0]["About us Arabic"].ToString();
                                tempService.Languages = dataTable.Rows[0]["Languages"].ToString();
                                tempService.ScopeofService = dataTable.Rows[0]["Scope of Service"].ToString();
                                tempService.ScopeOfServiceAr = dataTable.Rows[0]["Scope of Service Arabic"].ToString();
                                tempService.Images = dataTable.Rows[0]["Hotel Images"].ToString();
                                tempService.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                                tempService.Longitute = dataTable.Rows[0]["Longitute"].ToString();
                                tempService.CheckInTime = dataTable.Rows[0]["Check in Time"].ToString();
                                tempService.CheckOutTime = dataTable.Rows[0]["Check Out Time"].ToString();

                                // Hotel specific properties
                                tempService.AmenitiesName = dataTable.Rows[0]["Amenities Name"].ToString();
                                tempService.AmenitiesNameArabic = dataTable.Rows[0]["Amenities Name Arabic"].ToString();
                                tempService.AmenitiesImage = dataTable.Rows[0]["Amenites Logo"].ToString();
                                tempService.landmarkName = dataTable.Rows[0]["Landmark Name"].ToString();
                                tempService.landmarkNameArabic = dataTable.Rows[0]["Landmark name arabic"].ToString();
                                tempService.landmarkNameDistance = dataTable.Rows[0]["Landmark Distance KM"].ToString();
                                tempService.landmarkLatitute = dataTable.Rows[0]["Landmark Latitute"].ToString();
                                tempService.landmarkLongitute = dataTable.Rows[0]["Landmark Longitute"].ToString();

                                // Room specific properties
                                tempService.ServiceTypeName = dataTable.Rows[0]["Hotel Room Name"].ToString();
                                tempService.ServiceTypeNameAr = dataTable.Rows[0]["Hotel Room Name Arabic"].ToString();
                                tempService.PersoneQuantity = dataTable.Rows[0]["PersoneQuantity"].ToString();
                                tempService.AdultQuantity = dataTable.Rows[0]["AdultQuantity"].ToString();
                                tempService.ChildrenQuantity = dataTable.Rows[0]["ChildrenQuantity"].ToString();
                                tempService.TypeDescription = dataTable.Rows[0]["Description"].ToString();
                                tempService.TypeDescriptionAr = dataTable.Rows[0]["Description Arabic"].ToString();
                                tempService.Size = dataTable.Rows[0]["Size"].ToString();
                                tempService.SizeAr = dataTable.Rows[0]["Size Arabic"].ToString();
                                tempService.TypeImage = dataTable.Rows[0]["Images"].ToString();
                                tempService.TypePrice = dataTable.Rows[0]["Price"].ToString();
                                tempService.TypeServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                                #endregion

                                int counter = 0;
                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    counter++;
                                }

                                using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                                {
                                    writer.WriteStartElement("Root");

                                    foreach (DataRow objDataRow in dataTable.Rows)
                                    {
                                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                            continue;

                                        TempService data = new TempService();

                                        // Map data from tempService
                                        data.Name = tempService.Name;
                                        data.NameAr = tempService.NameAr;
                                        data.Description = tempService.Description;
                                        data.DescriptionAr = tempService.DescriptionAr;
                                        data.ServiceHour = tempService.ServiceHour;
                                        data.Category = tempService.Category;
                                        data.CategoryArabic = tempService.CategoryArabic;
                                        data.CategoryImage = tempService.CategoryImage;
                                        data.Country = tempService.Country;
                                        data.City = tempService.City;
                                        data.Address = tempService.Address;
                                        data.AboutUs = tempService.AboutUs;
                                        data.AboutUsAr = tempService.AboutUsAr;
                                        data.Languages = tempService.Languages;
                                        data.ScopeofService = tempService.ScopeofService;
                                        data.ScopeOfServiceAr = tempService.ScopeOfServiceAr;
                                        data.Images = tempService.Images;
                                        data.Latitute = tempService.Latitute;
                                        data.Longitute = tempService.Longitute;
                                        data.CheckInTime = tempService.CheckInTime;
                                        data.CheckOutTime = tempService.CheckOutTime;
                                        data.AmenitiesName = tempService.AmenitiesName;
                                        data.AmenitiesNameArabic = tempService.AmenitiesNameArabic;
                                        data.AmenitiesImage = tempService.AmenitiesImage;
                                        data.landmarkName = tempService.landmarkName;
                                        data.landmarkNameArabic = tempService.landmarkNameArabic;
                                        data.landmarkNameDistance = tempService.landmarkNameDistance;
                                        data.landmarkLatitute = tempService.landmarkLatitute;
                                        data.landmarkLongitute = tempService.landmarkLongitute;
                                        data.ServiceTypeName = tempService.ServiceTypeName;
                                        data.ServiceTypeNameAr = tempService.ServiceTypeNameAr;
                                        data.PersoneQuantity = tempService.PersoneQuantity;
                                        data.AdultQuantity = tempService.AdultQuantity;
                                        data.ChildrenQuantity = tempService.ChildrenQuantity;
                                        data.TypeDescription = tempService.TypeDescription;
                                        data.TypeDescriptionAr = tempService.TypeDescriptionAr;
                                        data.Size = tempService.Size;
                                        data.SizeAr = tempService.SizeAr;
                                        data.TypeImage = tempService.TypeImage;
                                        data.TypePrice = tempService.TypePrice;
                                        data.TypeServiceHour = tempService.TypeServiceHour;

                                        data.ExcelCount = counter;

                                        list.Add(data);

                                        writer.WriteStartElement("Service");

                                        // Write all attributes
                                        writer.WriteAttributeString("name", tempService.Name ?? "");
                                        writer.WriteAttributeString("nameAr", tempService.NameAr ?? "");
                                        writer.WriteAttributeString("description", tempService.Description ?? "");
                                        writer.WriteAttributeString("descriptionAr", tempService.DescriptionAr ?? "");
                                        writer.WriteAttributeString("serviceHour", tempService.ServiceHour ?? "");
                                        writer.WriteAttributeString("category", tempService.Category ?? "");
                                        writer.WriteAttributeString("categoryAr", tempService.CategoryArabic ?? "");
                                        writer.WriteAttributeString("categoryImage", tempService.CategoryImage ?? "");
                                        writer.WriteAttributeString("country", tempService.Country ?? "");
                                        writer.WriteAttributeString("city", tempService.City ?? "");
                                        writer.WriteAttributeString("address", tempService.Address ?? "");
                                        writer.WriteAttributeString("aboutUs", tempService.AboutUs ?? "");
                                        writer.WriteAttributeString("aboutUsAr", tempService.AboutUsAr ?? "");
                                        writer.WriteAttributeString("languages", tempService.Languages ?? "");
                                        writer.WriteAttributeString("scopeOfService", tempService.ScopeofService ?? "");
                                        writer.WriteAttributeString("scopeOfServiceAr", tempService.ScopeOfServiceAr ?? "");
                                        writer.WriteAttributeString("images", tempService.Images ?? "");
                                        writer.WriteAttributeString("latitude", tempService.Latitute ?? "");
                                        writer.WriteAttributeString("longitude", tempService.Longitute ?? "");
                                        writer.WriteAttributeString("checkInTime", tempService.CheckInTime ?? "");
                                        writer.WriteAttributeString("checkOutTime", tempService.CheckOutTime ?? "");
                                        writer.WriteAttributeString("amenitiesName", tempService.AmenitiesName ?? "");
                                        writer.WriteAttributeString("amenitiesNameArabic", tempService.AmenitiesNameArabic ?? "");
                                        writer.WriteAttributeString("amenitiesImage", tempService.AmenitiesImage ?? "");
                                        writer.WriteAttributeString("landmarkName", tempService.landmarkName ?? "");
                                        writer.WriteAttributeString("landmarkNameArabic", tempService.landmarkNameArabic ?? "");
                                        writer.WriteAttributeString("landmarkDistance", tempService.landmarkNameDistance ?? "");
                                        writer.WriteAttributeString("landmarkLatitude", tempService.landmarkLatitute ?? "");
                                        writer.WriteAttributeString("landmarkLongitude", tempService.landmarkLongitute ?? "");
                                        writer.WriteAttributeString("serviceTypeName", tempService.ServiceTypeName ?? "");
                                        writer.WriteAttributeString("serviceTypeNameAr", tempService.ServiceTypeNameAr ?? "");
                                        writer.WriteAttributeString("personQuantity", tempService.PersoneQuantity ?? "");
                                        writer.WriteAttributeString("adultQuantity", tempService.AdultQuantity ?? "");
                                        writer.WriteAttributeString("childrenQuantity", tempService.ChildrenQuantity ?? "");
                                        writer.WriteAttributeString("typeDescription", tempService.TypeDescription ?? "");
                                        writer.WriteAttributeString("typeDescriptionAr", tempService.TypeDescriptionAr ?? "");
                                        writer.WriteAttributeString("size", tempService.Size ?? "");
                                        writer.WriteAttributeString("sizeAr", tempService.SizeAr ?? "");
                                        writer.WriteAttributeString("typeImage", tempService.TypeImage ?? "");
                                        writer.WriteAttributeString("typePrice", tempService.TypePrice ?? "");
                                        writer.WriteAttributeString("typeServiceHour", tempService.TypeServiceHour ?? "");
                                        writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");
                                        writer.WriteEndElement();
                                    }

                                    writer.WriteEndElement();
                                }
                            }
                            else if (model.fCatagoryKey == "6440039B-6E5A-4E65-A0E4-F38B69C46C8C".ToLower() && model.IsPackage == true) // Replace with actual GUID for Hotel Package Service
                            {
                                #region Check Excel Column - Hotel Package Service
                                TempService tempService = new TempService();

                                tempService.Name = dataTable.Rows[0]["Package Name"].ToString();
                                tempService.NameAr = dataTable.Rows[0]["Package Name Arabic"].ToString();
                                tempService.Description = dataTable.Rows[0]["Description-html"].ToString();
                                tempService.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();
                                tempService.Country = dataTable.Rows[0]["Country Name"].ToString();
                                tempService.City = dataTable.Rows[0]["City Name"].ToString();
                                tempService.Address = dataTable.Rows[0]["Address"].ToString();
                                tempService.AboutUs = dataTable.Rows[0]["AboutUs-html"].ToString();
                                tempService.AboutUsAr = dataTable.Rows[0]["About us Arabic-html"].ToString();
                                tempService.Languages = dataTable.Rows[0]["Languages"].ToString();
                                tempService.Images = dataTable.Rows[0]["Images"].ToString();
                                tempService.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                                tempService.Longitute = dataTable.Rows[0]["Longitute"].ToString();
                                tempService.Price = dataTable.Rows[0]["price"].ToString();

                                // Package specific properties
                                tempService.AmenitiesName = dataTable.Rows[0]["Amenities Name"].ToString();
                                tempService.AmenitiesNameArabic = dataTable.Rows[0]["Amenities Name Arabic"].ToString();
                                tempService.AmenitiesImage = dataTable.Rows[0]["Amenities Images"].ToString();

                                #endregion

                                int counter = 0;
                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    counter++;
                                }

                                using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                                {
                                    writer.WriteStartElement("Root");

                                    foreach (DataRow objDataRow in dataTable.Rows)
                                    {
                                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                            continue;

                                        TempService data = new TempService();

                                        // Map data from tempService
                                        data.Name = tempService.Name;
                                        data.NameAr = tempService.NameAr;
                                        data.Description = tempService.Description;
                                        data.DescriptionAr = tempService.DescriptionAr;
                                        data.Country = tempService.Country;
                                        data.City = tempService.City;
                                        data.Address = tempService.Address;
                                        data.AboutUs = tempService.AboutUs;
                                        data.AboutUsAr = tempService.AboutUsAr;
                                        data.Languages = tempService.Languages;
                                        data.Images = tempService.Images;
                                        data.Latitute = tempService.Latitute;
                                        data.Longitute = tempService.Longitute;
                                        data.Price = tempService.Price;
                                        data.AmenitiesName = tempService.AmenitiesName;
                                        data.AmenitiesNameArabic = tempService.AmenitiesNameArabic;
                                        data.AmenitiesImage = tempService.AmenitiesImage;

                                        data.ExcelCount = counter;

                                        list.Add(data);

                                        writer.WriteStartElement("Service");

                                        writer.WriteAttributeString("name", tempService.Name ?? "");
                                        writer.WriteAttributeString("nameAr", tempService.NameAr ?? "");
                                        writer.WriteAttributeString("description", tempService.Description ?? "");
                                        writer.WriteAttributeString("descriptionAr", tempService.DescriptionAr ?? "");
                                        writer.WriteAttributeString("country", tempService.Country ?? "");
                                        writer.WriteAttributeString("city", tempService.City ?? "");
                                        writer.WriteAttributeString("address", tempService.Address ?? "");
                                        writer.WriteAttributeString("aboutUs", tempService.AboutUs ?? "");
                                        writer.WriteAttributeString("aboutUsAr", tempService.AboutUsAr ?? "");
                                        writer.WriteAttributeString("languages", tempService.Languages ?? "");
                                        writer.WriteAttributeString("images", tempService.Images ?? "");
                                        writer.WriteAttributeString("latitude", tempService.Latitute ?? "");
                                        writer.WriteAttributeString("longitude", tempService.Longitute ?? "");
                                        writer.WriteAttributeString("price", tempService.Price ?? "");
                                        writer.WriteAttributeString("amenitiesName", tempService.AmenitiesName ?? "");
                                        writer.WriteAttributeString("amenitiesNameArabic", tempService.AmenitiesNameArabic ?? "");
                                        writer.WriteAttributeString("amenitiesImage", tempService.AmenitiesImage ?? "");
                                        writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");
                                        writer.WriteEndElement();
                                    }

                                    writer.WriteEndElement();
                                }
                            }
                            else if (model.fCatagoryKey == "2CDCEBCF-BA2C-4C2C-9C53-F32C06233FFD".ToLower() && model.IsPackage == false) // Replace with actual GUID for Flight Booking Service
                            {
                                #region Check Excel Column - Flight Booking Service
                                TempService tempService = new TempService();

                                tempService.Name = dataTable.Rows[0]["AirLine Service Name"].ToString();
                                tempService.NameAr = dataTable.Rows[0]["AirLine Service Name Arabic"].ToString();
                                tempService.Description = dataTable.Rows[0]["Description-html"].ToString();
                                tempService.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();
                                tempService.Country = dataTable.Rows[0]["Country Name"].ToString();
                                tempService.City = dataTable.Rows[0]["City Name"].ToString();
                                tempService.Address = dataTable.Rows[0]["Address"].ToString();
                                tempService.AboutUs = dataTable.Rows[0]["AboutUs-html"].ToString();
                                tempService.AboutUsAr = dataTable.Rows[0]["About us Arabic-html"].ToString();
                                tempService.Languages = dataTable.Rows[0]["Languages"].ToString();
                                tempService.ScopeofService = dataTable.Rows[0]["Scope of Service-html"].ToString();
                                tempService.ScopeOfServiceAr = dataTable.Rows[0]["Scope of Service Arabic-html"].ToString();
                                tempService.Images = dataTable.Rows[0]["Images"].ToString();
                                tempService.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                                tempService.Longitute = dataTable.Rows[0]["Longitute"].ToString();
                                tempService.Price = dataTable.Rows[0]["price"].ToString();

                                // Flight specific properties
                                tempService.AirportName = dataTable.Rows[0]["Airport Name"].ToString();
                                tempService.AirportNameArabic = dataTable.Rows[0]["Airport Name Arabic"].ToString();
                                tempService.AirportCode = dataTable.Rows[0]["AirportCode"].ToString();
                                tempService.ServiceTypeName = dataTable.Rows[0]["Flight Service Type Name"].ToString();
                                tempService.ServiceTypeNameAr = dataTable.Rows[0]["Flight Service Type Name Arabic"].ToString();
                                tempService.TypeDescription = dataTable.Rows[0]["Description"].ToString();
                                tempService.TypeDescriptionAr = dataTable.Rows[0]["Description Arabic"].ToString();
                                tempService.TypeImage = dataTable.Rows[0]["Images"].ToString();
                                tempService.TypePrice = dataTable.Rows[0]["Price"].ToString();
                                tempService.PersoneQuantity = dataTable.Rows[0]["Person Quantity"].ToString();
                                tempService.AdultQuantity = dataTable.Rows[0]["Adult Quantity"].ToString();
                                tempService.ChildrenQuantity = dataTable.Rows[0]["Children Quantity"].ToString();
                                tempService.TypeServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                                tempService.TypeServiceMin = dataTable.Rows[0]["ServiceMin"].ToString();
                                tempService.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                                #endregion

                                int counter = 0;
                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    counter++;
                                }

                                using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                                {
                                    writer.WriteStartElement("Root");

                                    foreach (DataRow objDataRow in dataTable.Rows)
                                    {
                                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                            continue;

                                        TempService data = new TempService();

                                        // Map data from tempService
                                        data.Name = tempService.Name;
                                        data.NameAr = tempService.NameAr;
                                        data.Description = tempService.Description;
                                        data.DescriptionAr = tempService.DescriptionAr;
                                        data.Country = tempService.Country;
                                        data.City = tempService.City;
                                        data.Address = tempService.Address;
                                        data.AboutUs = tempService.AboutUs;
                                        data.AboutUsAr = tempService.AboutUsAr;
                                        data.Languages = tempService.Languages;
                                        data.ScopeofService = tempService.ScopeofService;
                                        data.ScopeOfServiceAr = tempService.ScopeOfServiceAr;
                                        data.Images = tempService.Images;
                                        data.Latitute = tempService.Latitute;
                                        data.Longitute = tempService.Longitute;
                                        data.Price = tempService.Price;
                                        data.AirportName = tempService.AirportName;
                                        data.AirportNameArabic = tempService.AirportNameArabic;
                                        data.AirportCode = tempService.AirportCode;
                                        data.ServiceTypeName = tempService.ServiceTypeName;
                                        data.ServiceTypeNameAr = tempService.ServiceTypeNameAr;
                                        data.TypeDescription = tempService.TypeDescription;
                                        data.TypeDescriptionAr = tempService.TypeDescriptionAr;
                                        data.TypeImage = tempService.TypeImage;
                                        data.TypePrice = tempService.TypePrice;
                                        data.PersoneQuantity = tempService.PersoneQuantity;
                                        data.AdultQuantity = tempService.AdultQuantity;
                                        data.ChildrenQuantity = tempService.ChildrenQuantity;
                                        data.TypeServiceHour = tempService.TypeServiceHour;
                                        data.TypeServiceMin = tempService.TypeServiceMin;
                                        data.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                                        data.ExcelCount = counter;

                                        list.Add(data);

                                        writer.WriteStartElement("Service");

                                        writer.WriteAttributeString("name", tempService.Name ?? "");
                                        writer.WriteAttributeString("nameAr", tempService.NameAr ?? "");
                                        writer.WriteAttributeString("description", tempService.Description ?? "");
                                        writer.WriteAttributeString("descriptionAr", tempService.DescriptionAr ?? "");
                                        writer.WriteAttributeString("country", tempService.Country ?? "");
                                        writer.WriteAttributeString("city", tempService.City ?? "");
                                        writer.WriteAttributeString("address", tempService.Address ?? "");
                                        writer.WriteAttributeString("aboutUs", tempService.AboutUs ?? "");
                                        writer.WriteAttributeString("aboutUsAr", tempService.AboutUsAr ?? "");
                                        writer.WriteAttributeString("languages", tempService.Languages ?? "");
                                        writer.WriteAttributeString("scopeOfService", tempService.ScopeofService ?? "");
                                        writer.WriteAttributeString("scopeOfServiceAr", tempService.ScopeOfServiceAr ?? "");
                                        writer.WriteAttributeString("images", tempService.Images ?? "");
                                        writer.WriteAttributeString("latitude", tempService.Latitute ?? "");
                                        writer.WriteAttributeString("longitude", tempService.Longitute ?? "");
                                        writer.WriteAttributeString("price", tempService.Price ?? "");
                                        writer.WriteAttributeString("airportName", tempService.AirportName ?? "");
                                        writer.WriteAttributeString("airportNameArabic", tempService.AirportNameArabic ?? "");
                                        writer.WriteAttributeString("airportCode", tempService.AirportCode ?? "");
                                        writer.WriteAttributeString("serviceTypeName", tempService.ServiceTypeName ?? "");
                                        writer.WriteAttributeString("serviceTypeNameAr", tempService.ServiceTypeNameAr ?? "");
                                        writer.WriteAttributeString("typeDescription", tempService.TypeDescription ?? "");
                                        writer.WriteAttributeString("typeDescriptionAr", tempService.TypeDescriptionAr ?? "");
                                        writer.WriteAttributeString("typeImage", tempService.TypeImage ?? "");
                                        writer.WriteAttributeString("typePrice", tempService.TypePrice ?? "");
                                        writer.WriteAttributeString("personQuantity", tempService.PersoneQuantity ?? "");
                                        writer.WriteAttributeString("adultQuantity", tempService.AdultQuantity ?? "");
                                        writer.WriteAttributeString("childrenQuantity", tempService.ChildrenQuantity ?? "");
                                        writer.WriteAttributeString("typeServiceHour", tempService.TypeServiceHour ?? "");
                                        writer.WriteAttributeString("typeServiceMin", tempService.TypeServiceMin ?? "");
                                        writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");
                                        writer.WriteEndElement();
                                    }

                                    writer.WriteEndElement();
                                }
                            }
                            else if (model.fCatagoryKey == "2CDCEBCF-BA2C-4C2C-9C53-F32C06233FFD".ToLower() && model.IsPackage == true) // Replace with actual GUID for Flight Package Service
                            {
                                #region Check Excel Column - Flight Package Service
                                TempService tempService = new TempService();

                                tempService.Name = dataTable.Rows[0]["Package Name"].ToString();
                                tempService.NameAr = dataTable.Rows[0]["Package Name Arabic"].ToString();
                                tempService.Description = dataTable.Rows[0]["Description-html"].ToString();
                                tempService.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();
                                tempService.Country = dataTable.Rows[0]["Country Name"].ToString();
                                tempService.City = dataTable.Rows[0]["City Name"].ToString();
                                tempService.Address = dataTable.Rows[0]["Address"].ToString();
                                tempService.AboutUs = dataTable.Rows[0]["AboutUs-html"].ToString();
                                tempService.AboutUsAr = dataTable.Rows[0]["About us Arabic-html"].ToString();
                                tempService.Languages = dataTable.Rows[0]["Languages"].ToString();
                                tempService.Images = dataTable.Rows[0]["Images"].ToString();
                                tempService.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                                tempService.Longitute = dataTable.Rows[0]["Longitute"].ToString();
                                tempService.Price = dataTable.Rows[0]["price"].ToString();

                                // Flight package specific properties
                                tempService.AmenitiesName = dataTable.Rows[0]["Amenities Name"].ToString();
                                tempService.AmenitiesNameArabic = dataTable.Rows[0]["Amenities Name Arabic"].ToString();
                                tempService.AmenitiesImage = dataTable.Rows[0]["Amenities  Images"].ToString();
                                tempService.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                                #endregion

                                int counter = 0;
                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    counter++;
                                }

                                using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                                {
                                    writer.WriteStartElement("Root");

                                    foreach (DataRow objDataRow in dataTable.Rows)
                                    {
                                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                            continue;

                                        TempService data = new TempService();

                                        // Map data from tempService
                                        data.Name = tempService.Name;
                                        data.NameAr = tempService.NameAr;
                                        data.Description = tempService.Description;
                                        data.DescriptionAr = tempService.DescriptionAr;
                                        data.Country = tempService.Country;
                                        data.City = tempService.City;
                                        data.Address = tempService.Address;
                                        data.AboutUs = tempService.AboutUs;
                                        data.AboutUsAr = tempService.AboutUsAr;
                                        data.Languages = tempService.Languages;
                                        data.Images = tempService.Images;
                                        data.Latitute = tempService.Latitute;
                                        data.Longitute = tempService.Longitute;
                                        data.Price = tempService.Price;
                                        data.AmenitiesName = tempService.AmenitiesName;
                                        data.AmenitiesNameArabic = tempService.AmenitiesNameArabic;
                                        data.AmenitiesImage = tempService.AmenitiesImage;
                                        data.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                                        data.ExcelCount = counter;

                                        list.Add(data);

                                        writer.WriteStartElement("Service");

                                        writer.WriteAttributeString("name", tempService.Name ?? "");
                                        writer.WriteAttributeString("nameAr", tempService.NameAr ?? "");
                                        writer.WriteAttributeString("description", tempService.Description ?? "");
                                        writer.WriteAttributeString("descriptionAr", tempService.DescriptionAr ?? "");
                                        writer.WriteAttributeString("country", tempService.Country ?? "");
                                        writer.WriteAttributeString("city", tempService.City ?? "");
                                        writer.WriteAttributeString("address", tempService.Address ?? "");
                                        writer.WriteAttributeString("aboutUs", tempService.AboutUs ?? "");
                                        writer.WriteAttributeString("aboutUsAr", tempService.AboutUsAr ?? "");
                                        writer.WriteAttributeString("languages", tempService.Languages ?? "");
                                        writer.WriteAttributeString("images", tempService.Images ?? "");
                                        writer.WriteAttributeString("latitude", tempService.Latitute ?? "");
                                        writer.WriteAttributeString("longitude", tempService.Longitute ?? "");
                                        writer.WriteAttributeString("price", tempService.Price ?? "");
                                        writer.WriteAttributeString("amenitiesName", tempService.AmenitiesName ?? "");
                                        writer.WriteAttributeString("amenitiesNameArabic", tempService.AmenitiesNameArabic ?? "");
                                        writer.WriteAttributeString("amenitiesImage", tempService.AmenitiesImage ?? "");
                                        writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");
                                        writer.WriteEndElement();
                                    }

                                    writer.WriteEndElement();
                                }
                            }

                            else if (model.fCatagoryKey == "DD501B2D-FE22-4C31-B340-1B4237FAB5CC".ToLower()) // Replace with actual GUID
                            {
                                #region Check Excel Column - Real Estate Service
                                TempService tempService = new TempService();

                                tempService.Name = dataTable.Rows[0]["RealState Name"].ToString();
                                tempService.NameAr = dataTable.Rows[0]["RealState Name Arabic"].ToString();
                                tempService.Description = dataTable.Rows[0]["Description-html"].ToString();
                                tempService.DescriptionAr = dataTable.Rows[0]["Description Arabic-html"].ToString();
                                tempService.ServiceHour = dataTable.Rows[0]["ServiceHour"].ToString();
                                tempService.Category = dataTable.Rows[0]["Category"].ToString();
                                tempService.CategoryArabic = dataTable.Rows[0]["Category arabic"].ToString();
                                tempService.CategoryImage = dataTable.Rows[0]["Category Image"].ToString();
                                tempService.Country = dataTable.Rows[0]["Country Name"].ToString();
                                tempService.City = dataTable.Rows[0]["City Name"].ToString();
                                tempService.Address = dataTable.Rows[0]["Address"].ToString();
                                tempService.AboutUs = dataTable.Rows[0]["AboutUs"].ToString();
                                tempService.AboutUsAr = dataTable.Rows[0]["About us Arabic"].ToString();
                                tempService.Languages = dataTable.Rows[0]["Languages"].ToString();
                                tempService.Images = dataTable.Rows[0]["Images"].ToString();
                                tempService.Latitute = dataTable.Rows[0]["Latitute"].ToString();
                                tempService.Longitute = dataTable.Rows[0]["Longitute"].ToString();

                                // Real Estate specific properties
                                tempService.ServiceTypeName = dataTable.Rows[0]["Service Type Name"].ToString();
                                tempService.ServiceTypeNameAr = dataTable.Rows[0]["Service Type Name Arabic"].ToString();
                                tempService.PersoneQuantity = dataTable.Rows[0]["PersoneQuantity"].ToString();
                                tempService.AdultQuantity = dataTable.Rows[0]["AdultQuantity"].ToString();
                                tempService.ChildrenQuantity = dataTable.Rows[0]["ChildrenQuantity"].ToString();
                                tempService.TypeDescription = dataTable.Rows[0]["Description"].ToString();
                                tempService.TypeDescriptionAr = dataTable.Rows[0]["Description Arabic"].ToString();
                                tempService.Size = dataTable.Rows[0]["Size"].ToString();
                                tempService.SizeAr = dataTable.Rows[0]["Size Arabic"].ToString();
                                tempService.TypeImage = dataTable.Rows[0]["Images"].ToString();
                                tempService.TypePrice = dataTable.Rows[0]["Price"].ToString();
                                tempService.PaymentType = dataTable.Rows[0]["Payment Type"].ToString();
                                tempService.ServiceTypeCategory = dataTable.Rows[0]["Service Type Category"].ToString();
                                tempService.ServiceTypeSubCategory = dataTable.Rows[0]["Service Type Sub Category"].ToString();
                                tempService.PropertyType = dataTable.Rows[0]["Property Type"].ToString();
                                tempService.PropertyTypeArabic = dataTable.Rows[0]["Property Type Arabic"].ToString();
                                tempService.PropertyRefNo = dataTable.Rows[0]["Ref No"].ToString();
                                tempService.PropertyPurpose = dataTable.Rows[0]["Property Purpose"].ToString();
                                tempService.Furnishing = dataTable.Rows[0]["Furnishing"].ToString();
                                tempService.FurnishingArabic = dataTable.Rows[0]["Furnishing Arabic"].ToString();
                                tempService.PropertyWhatsAppNo = dataTable.Rows[0]["PropertyWhatsAppNo"].ToString();
                                tempService.PropertyEmail = dataTable.Rows[0]["Property email"].ToString();
                                tempService.ExteriorDetails = dataTable.Rows[0]["Exterior Details"].ToString();
                                tempService.ExteriorDetailsArabic = dataTable.Rows[0]["Exterior Details Arabic"].ToString();
                                tempService.ExteriorImage = dataTable.Rows[0]["Exterior Image"].ToString();
                                tempService.InteriorDetails = dataTable.Rows[0]["Interior Details"].ToString();
                                tempService.InteriorDetailsArabic = dataTable.Rows[0]["Interior Details Arabic"].ToString();
                                tempService.InteriorImage = dataTable.Rows[0]["Interior Image"].ToString();
                                tempService.AmenitiesName = dataTable.Rows[0]["Amenities Name"].ToString();
                                tempService.AmenitiesNameArabic = dataTable.Rows[0]["Amenities name arabic"].ToString();
                                tempService.AmenitiesImage = dataTable.Rows[0]["Amenitise Image"].ToString();
                                tempService.AmenitiesFile = dataTable.Rows[0]["Amenities File"].ToString();
                                tempService.CloserPropertyName = dataTable.Rows[0]["CloserProperty Name"].ToString();
                                tempService.CloserPropertyNameArabic = dataTable.Rows[0]["CloserProperty Name Arabic"].ToString();
                                tempService.CloserPropertyLogo = dataTable.Rows[0]["CloserProperty  Logo"].ToString();
                                tempService.CloserPropertyFile = dataTable.Rows[0]["CloserProperty File"].ToString();
                                tempService.MaterialsName = dataTable.Rows[0]["Materials name"].ToString();
                                tempService.MaterialsNameArabic = dataTable.Rows[0]["Materials Name Arabic"].ToString();
                                tempService.MaterialsLogo = dataTable.Rows[0]["Materials Logo"].ToString();
                                tempService.MaterialsFile = dataTable.Rows[0]["Materials File"].ToString();
                                tempService.UtilityName = dataTable.Rows[0]["Utility Name"].ToString();
                                tempService.UtilityNameArabic = dataTable.Rows[0]["Utility Name Arabic"].ToString();
                                tempService.UtilityLogo = dataTable.Rows[0]["Utility Logo"].ToString();
                                tempService.UtilityFile = dataTable.Rows[0]["Utility File"].ToString();
                                tempService.Video = dataTable.Rows[0]["Video"].ToString();
                                #endregion

                                int counter = 0;
                                foreach (DataRow objDataRow in dataTable.Rows)
                                {
                                    if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                        continue;
                                    counter++;
                                }

                                using (XmlWriter writer = new XmlTextWriter(new StringWriter(xmlStore)))
                                {
                                    writer.WriteStartElement("Root");

                                    foreach (DataRow objDataRow in dataTable.Rows)
                                    {
                                        if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString())))
                                            continue;

                                        TempService data = new TempService();

                                        // Map all properties from tempService
                                        data.Name = tempService.Name;
                                        data.NameAr = tempService.NameAr;
                                        data.Description = tempService.Description;
                                        data.DescriptionAr = tempService.DescriptionAr;
                                        data.ServiceHour = tempService.ServiceHour;
                                        data.Category = tempService.Category;
                                        data.CategoryArabic = tempService.CategoryArabic;
                                        data.CategoryImage = tempService.CategoryImage;
                                        data.Country = tempService.Country;
                                        data.City = tempService.City;
                                        data.Address = tempService.Address;
                                        data.AboutUs = tempService.AboutUs;
                                        data.AboutUsAr = tempService.AboutUsAr;
                                        data.Languages = tempService.Languages;
                                        data.Images = tempService.Images;
                                        data.Latitute = tempService.Latitute;
                                        data.Longitute = tempService.Longitute;
                                        data.ServiceTypeName = tempService.ServiceTypeName;
                                        data.ServiceTypeNameAr = tempService.ServiceTypeNameAr;
                                        data.PersoneQuantity = tempService.PersoneQuantity;
                                        data.AdultQuantity = tempService.AdultQuantity;
                                        data.ChildrenQuantity = tempService.ChildrenQuantity;
                                        data.TypeDescription = tempService.TypeDescription;
                                        data.TypeDescriptionAr = tempService.TypeDescriptionAr;
                                        data.Size = tempService.Size;
                                        data.SizeAr = tempService.SizeAr;
                                        data.TypeImage = tempService.TypeImage;
                                        data.TypePrice = tempService.TypePrice;
                                        data.PaymentType = tempService.PaymentType;
                                        data.ServiceTypeCategory = tempService.ServiceTypeCategory;
                                        data.ServiceTypeSubCategory = tempService.ServiceTypeSubCategory;
                                        data.PropertyType = tempService.PropertyType;
                                        data.PropertyTypeArabic = tempService.PropertyTypeArabic;
                                        data.PropertyRefNo = tempService.PropertyRefNo;
                                        data.PropertyPurpose = tempService.PropertyPurpose;
                                        data.Furnishing = tempService.Furnishing;
                                        data.FurnishingArabic = tempService.FurnishingArabic;
                                        data.PropertyWhatsAppNo = tempService.PropertyWhatsAppNo;
                                        data.PropertyEmail = tempService.PropertyEmail;
                                        data.ExteriorDetails = tempService.ExteriorDetails;
                                        data.ExteriorDetailsArabic = tempService.ExteriorDetailsArabic;
                                        data.ExteriorImage = tempService.ExteriorImage;
                                        data.InteriorDetails = tempService.InteriorDetails;
                                        data.InteriorDetailsArabic = tempService.InteriorDetailsArabic;
                                        data.InteriorImage = tempService.InteriorImage;
                                        data.AmenitiesName = tempService.AmenitiesName;
                                        data.AmenitiesNameArabic = tempService.AmenitiesNameArabic;
                                        data.AmenitiesImage = tempService.AmenitiesImage;
                                        data.AmenitiesFile = tempService.AmenitiesFile;
                                        data.CloserPropertyName = tempService.CloserPropertyName;
                                        data.CloserPropertyNameArabic = tempService.CloserPropertyNameArabic;
                                        data.CloserPropertyLogo = tempService.CloserPropertyLogo;
                                        data.CloserPropertyFile = tempService.CloserPropertyFile;
                                        data.MaterialsName = tempService.MaterialsName;
                                        data.MaterialsNameArabic = tempService.MaterialsNameArabic;
                                        data.MaterialsLogo = tempService.MaterialsLogo;
                                        data.MaterialsFile = tempService.MaterialsFile;
                                        data.UtilityName = tempService.UtilityName;
                                        data.UtilityNameArabic = tempService.UtilityNameArabic;
                                        data.UtilityLogo = tempService.UtilityLogo;
                                        data.UtilityFile = tempService.UtilityFile;
                                        data.Video = tempService.Video;
                                        data.ExcelCount = counter;

                                        list.Add(data);

                                        writer.WriteStartElement("Service");

                                        // Write all attributes
                                        writer.WriteAttributeString("name", tempService.Name ?? "");
                                        writer.WriteAttributeString("nameAr", tempService.NameAr ?? "");
                                        writer.WriteAttributeString("description", tempService.Description ?? "");
                                        writer.WriteAttributeString("descriptionAr", tempService.DescriptionAr ?? "");
                                        writer.WriteAttributeString("serviceHour", tempService.ServiceHour ?? "");
                                        writer.WriteAttributeString("category", tempService.Category ?? "");
                                        writer.WriteAttributeString("categoryAr", tempService.CategoryArabic ?? "");
                                        writer.WriteAttributeString("categoryImage", tempService.CategoryImage ?? "");
                                        writer.WriteAttributeString("country", tempService.Country ?? "");
                                        writer.WriteAttributeString("city", tempService.City ?? "");
                                        writer.WriteAttributeString("address", tempService.Address ?? "");
                                        writer.WriteAttributeString("aboutUs", tempService.AboutUs ?? "");
                                        writer.WriteAttributeString("aboutUsAr", tempService.AboutUsAr ?? "");
                                        writer.WriteAttributeString("languages", tempService.Languages ?? "");
                                        writer.WriteAttributeString("images", tempService.Images ?? "");
                                        writer.WriteAttributeString("latitude", tempService.Latitute ?? "");
                                        writer.WriteAttributeString("longitude", tempService.Longitute ?? "");
                                        writer.WriteAttributeString("serviceTypeName", tempService.ServiceTypeName ?? "");
                                        writer.WriteAttributeString("serviceTypeNameAr", tempService.ServiceTypeNameAr ?? "");
                                        writer.WriteAttributeString("personQuantity", tempService.PersoneQuantity ?? "");
                                        writer.WriteAttributeString("adultQuantity", tempService.AdultQuantity ?? "");
                                        writer.WriteAttributeString("childrenQuantity", tempService.ChildrenQuantity ?? "");
                                        writer.WriteAttributeString("typeDescription", tempService.TypeDescription ?? "");
                                        writer.WriteAttributeString("typeDescriptionAr", tempService.TypeDescriptionAr ?? "");
                                        writer.WriteAttributeString("size", tempService.Size ?? "");
                                        writer.WriteAttributeString("sizeAr", tempService.SizeAr ?? "");
                                        writer.WriteAttributeString("typeImage", tempService.TypeImage ?? "");
                                        writer.WriteAttributeString("typePrice", tempService.TypePrice ?? "");
                                        writer.WriteAttributeString("paymentType", tempService.PaymentType ?? "");
                                        writer.WriteAttributeString("serviceTypeCategory", tempService.ServiceTypeCategory ?? "");
                                        writer.WriteAttributeString("serviceTypeSubCategory", tempService.ServiceTypeSubCategory ?? "");
                                        writer.WriteAttributeString("propertyType", tempService.PropertyType ?? "");
                                        writer.WriteAttributeString("propertyTypeAr", tempService.PropertyTypeArabic ?? "");
                                        writer.WriteAttributeString("propertyRefNo", tempService.PropertyRefNo ?? "");
                                        writer.WriteAttributeString("propertyPurpose", tempService.PropertyPurpose ?? "");
                                        writer.WriteAttributeString("furnishing", tempService.Furnishing ?? "");
                                        writer.WriteAttributeString("furnishingAr", tempService.FurnishingArabic ?? "");
                                        writer.WriteAttributeString("propertyWhatsAppNo", tempService.PropertyWhatsAppNo ?? "");
                                        writer.WriteAttributeString("propertyEmail", tempService.PropertyEmail ?? "");
                                        writer.WriteAttributeString("exteriorDetails", tempService.ExteriorDetails ?? "");
                                        writer.WriteAttributeString("exteriorDetailsAr", tempService.ExteriorDetailsArabic ?? "");
                                        writer.WriteAttributeString("exteriorImage", tempService.ExteriorImage ?? "");
                                        writer.WriteAttributeString("interiorDetails", tempService.InteriorDetails ?? "");
                                        writer.WriteAttributeString("interiorDetailsAr", tempService.InteriorDetailsArabic ?? "");
                                        writer.WriteAttributeString("interiorImage", tempService.InteriorImage ?? "");
                                        writer.WriteAttributeString("amenitiesName", tempService.AmenitiesName ?? "");
                                        writer.WriteAttributeString("amenitiesNameAr", tempService.AmenitiesNameArabic ?? "");
                                        writer.WriteAttributeString("amenitiesImage", tempService.AmenitiesImage ?? "");
                                        writer.WriteAttributeString("amenitiesFile", tempService.AmenitiesFile ?? "");
                                        writer.WriteAttributeString("closerPropertyName", tempService.CloserPropertyName ?? "");
                                        writer.WriteAttributeString("closerPropertyNameAr", tempService.CloserPropertyNameArabic ?? "");
                                        writer.WriteAttributeString("closerPropertyLogo", tempService.CloserPropertyLogo ?? "");
                                        writer.WriteAttributeString("closerPropertyFile", tempService.CloserPropertyFile ?? "");
                                        writer.WriteAttributeString("materialsName", tempService.MaterialsName ?? "");
                                        writer.WriteAttributeString("materialsNameAr", tempService.MaterialsNameArabic ?? "");
                                        writer.WriteAttributeString("materialsLogo", tempService.MaterialsLogo ?? "");
                                        writer.WriteAttributeString("materialsFile", tempService.MaterialsFile ?? "");
                                        writer.WriteAttributeString("utilityName", tempService.UtilityName ?? "");
                                        writer.WriteAttributeString("utilityNameAr", tempService.UtilityNameArabic ?? "");
                                        writer.WriteAttributeString("utilityLogo", tempService.UtilityLogo ?? "");
                                        writer.WriteAttributeString("utilityFile", tempService.UtilityFile ?? "");
                                        writer.WriteAttributeString("video", tempService.Video ?? "");
                                        writer.WriteAttributeString("excelCount", data.ExcelCount.ToString() ?? "0");
                                        writer.WriteEndElement();
                                    }

                                    writer.WriteEndElement();
                                }
                            }
                            if (list.Count() > 0)
                            {
                                await _tempServiceDataAccess.AddTempService(xmlStore.ToString(), feacherCategory.FeatureCategoryId,model.IsPackage);
                                return RedirectToAction(nameof(AddServiceBulk), new { message = "", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString(), isPackage = model.IsPackage });
                            }
                            else
                            {
                                return RedirectToAction(nameof(AddServiceBulk), new { message = "List is Empty", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString(), isPackage = model.IsPackage });
                            }


                        }
                        else
                        {
                            return RedirectToAction(nameof(AddServiceBulk), new { message = "Invalid File Formate. Please Upload .xls or .xlsx format.", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString(), isPackage = model.IsPackage });
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(AddServiceBulk), new { message = "No File Found.", fCatagoryKey = feacherCategory.FeatureCategoryKey.ToString(), isPackage = model.IsPackage });
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction(nameof(AddServiceBulk), new { message = ex.Message.Replace("'", ""), fCatagoryKey = model.fCatagoryKey.ToString(), isPackage = model.IsPackage });
                }
            }
            else
            {
                return RedirectToAction(nameof(AddServiceBulk), new { message = "No File Found.", fCatagoryKey = model.fCatagoryKey.ToString(), isPackage = model.IsPackage });
            }
        }


        public async Task<ActionResult> DeleteService(string fCatagoryKey)
        {
            _tempServiceDataAccess.DeleteTempServices();
            return RedirectToAction("AddServiceBulk", new { message = "", fCatagoryKey = fCatagoryKey });
        }

        public async Task<ActionResult> UpdateAllTemptoService(string fCatagoryKey)
        {
            try
            {
                var feacherCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(fCatagoryKey));
                var result = _tempServiceDataAccess.AddService(feacherCategory.FeatureCategoryId);

                if (result == true)
                {
                    _tempServiceDataAccess.DeleteTempServices();
                    return RedirectToAction("AddServiceBulk", new { message = "All Records Successfully Added to Service.", fCatagoryKey = fCatagoryKey });
                }
                else
                {
                    return RedirectToAction("AddServiceBulk", new { message = "Something Wrong", fCatagoryKey = fCatagoryKey });
                }
              
              
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<JsonResult> ExcelImportCount()
        {
            var tempProduct = _tempServiceDataAccess.GetTempServiceCount();
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


        #region typing and insurance

        [HttpGet]
        public async Task<ActionResult> CreateForTypingAndInsurance(Guid? Key, string fCatagoryKey)
        {
            if (string.IsNullOrEmpty(fCatagoryKey))
            {
                ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAll(), "FeatureCategoryId", "Name");
                ViewBag.Catagory = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagoryKey), "ServiceId", "Name");
            }
            else
            {
                ViewBag.featureCategory = new SelectList(await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey), "FeatureCategoryId", "Name");
                ViewBag.Catagory = new SelectList(await _serviceAccess.GetAllByFeatureCategory(fCatagoryKey), "ServiceId", "Name");
            }

          
            if (Key == null || Key == Guid.Empty)
            {
               
                ViewBag.FeatureCategoryKey = fCatagoryKey.ToString();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                Boulevard.Models.Service node = new Boulevard.Models.Service();
               
              
                return View(node);
            }
            else
            {
               
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
        public async Task<ActionResult> CreateForTypingAndInsurance(Boulevard.Models.Service model)
        {
            Boulevard.Models.Service node = await _serviceAccess.GetByKey(model.ServiceKey);
           

            if (model.ServiceKey == null || model.ServiceKey == Guid.Empty)
            {
                var featureCategory = new FeatureCategory();
                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    featureCategory = await _featureCategoryAccess.GetByKey(Guid.Parse(model.FeatureCategoryKey));
                    model.FeatureCategoryId = featureCategory.FeatureCategoryId;
                }
                model.CreateBy = GetUser().UserId;
                var serviceData = await _serviceAccess.Insert(model);
               
            }
            else
            {
                model.FeatureCategoryId = node.FeatureCategoryId;
                model.UpdateBy = GetUser().UserId;
                var result = await _serviceAccess.Update(model);
               
            }
           
            if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
            {
                return RedirectToAction("IndexForChildService", "Service", new { fCatagoryKey = model.FeatureCategoryKey });
            }
            else
            {
                return RedirectToAction("IndexForChildService", "Service");
            }
        }

        #endregion

    }
}