using Boulevard.Contexts;
using Boulevard.Helper;
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
    public class OrderRequestServiceController : BaseController
    {
        private readonly OrderRequestServiceDataAccess _orderRequestServiceDataAccess;
        private readonly FeatureCategoryAccess _featureCategoryAccess;
        private readonly CityDataAccess _cityDataAccess;
        private readonly CountryDataAccess _countryDataAccess;
        private readonly BoulevardDbContext _context;


        public OrderRequestServiceController()
        {
            _orderRequestServiceDataAccess = new OrderRequestServiceDataAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _cityDataAccess = new CityDataAccess();
            _countryDataAccess = new CountryDataAccess();
            _context = new BoulevardDbContext();
        }
        // GET: Admin/OrderRequestService
        public async Task<ActionResult> Index(string fCatagoryKey)
        {
            try
            {
                ViewBag.FCatagoryKey = fCatagoryKey;
                ViewBag.BookingStatuses = new List<string> { "Active", "Cancelled", "Completed", "Confirmed", "Pending" };

                // Resolve FeatureCategory name + ID in one query
                int? fCatIdResolved = null;
                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out var fCatGuid))
                {
                    var cat = await _context.featureCategories
                        .Where(a => a.FeatureCategoryKey == fCatGuid)
                        .Select(a => new { a.FeatureCategoryId, a.Name })
                        .FirstOrDefaultAsync();
                    if (cat != null)
                    {
                        ViewBag.FCatagoryName = cat.Name;
                        fCatIdResolved = cat.FeatureCategoryId;
                    }
                }

                // Pre-fetch first 20 rows server-side so the table is visible
                // the moment the HTML arrives — no AJAX round-trip needed for
                // the initial page load. Only the COUNT is loaded via AJAX.
                IQueryable<OrderRequestService> q = _context.OrderRequestService;
                if (fCatIdResolved.HasValue)
                    q = q.Where(s => s.FeatureCategoryId == fCatIdResolved.Value);

                var first20 = await q
                    .OrderByDescending(s => s.OrderRequestServiceId)
                    .Take(20)
                    .Select(s => new
                    {
                        s.OrderRequestServiceId,
                        s.BookingId,
                        s.FirstName, s.LastName, s.MemberNameTitle,
                        MemberName    = s.Member.Name,
                        MemberEmail   = s.Member.Email,
                        MemberPhone   = s.Member.PhoneNumber,
                        s.Email, s.PhoneNo, s.ExtraCharge, s.TotalPrice,
                        s.IsPackage, s.BookingDate, s.BookingTime, s.BookingStatus
                    })
                    .ToListAsync();

                var formattedRows = first20.Select(s => new
                {
                    s.OrderRequestServiceId,
                    s.BookingId,
                    MemberName    = (!string.IsNullOrEmpty(s.FirstName)
                        ? ((s.MemberNameTitle ?? "") + " " + s.FirstName + " " + (s.LastName ?? "")).Trim()
                        : s.MemberName ?? ""),
                    Email         = s.Email ?? s.MemberEmail ?? "",
                    PhoneNo       = s.PhoneNo ?? s.MemberPhone ?? "",
                    ExtraCharge   = s.ExtraCharge.ToString("F2"),
                    TotalPrice    = (s.IsPackage ? "0.00" : s.TotalPrice.ToString("F2")),
                    BookingDate   = s.BookingDate.ToString("dd/MM/yyyy"),
                    BookingTime   = s.BookingTime ?? "",
                    BookingStatus = s.BookingStatus ?? ""
                }).ToList();

                var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeHtml
                };
                ViewBag.InitialRows    = Newtonsoft.Json.JsonConvert.SerializeObject(formattedRows, jsonSettings);
                ViewBag.InitialHasMore = first20.Count == 20;

                return View(Enumerable.Empty<OrderRequestService>());
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return RedirectToAction("Index", "Dashboard");
            }
        }

        // Returns the COUNT only — called after rows are already rendered.
        [HttpPost]
        public async Task<JsonResult> GetPagedServiceOrdersCount(string fCatagoryKey, string bookingStatus)
        {
            try
            {
                IQueryable<OrderRequestService> query = _context.OrderRequestService;

                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out var fCatGuid))
                {
                    var fCatId = await _context.featureCategories
                        .Where(a => a.FeatureCategoryKey == fCatGuid)
                        .Select(a => (int?)a.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fCatId.HasValue)
                        query = query.Where(s => s.FeatureCategoryId == fCatId.Value);
                }

                if (!string.IsNullOrEmpty(bookingStatus))
                    query = query.Where(s => s.BookingStatus == bookingStatus);

                int total = await query.CountAsync();
                return Json(new { success = true, total });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, total = 0 });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetPagedServiceOrders(string fCatagoryKey, string bookingStatus, int page, int pageSize)
        {
            try
            {
                if (pageSize < 10 || pageSize > 500) pageSize = 20;
                if (page < 1) page = 1;

                IQueryable<OrderRequestService> query = _context.OrderRequestService;

                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out var fCatGuid))
                {
                    var fCatId = await _context.featureCategories
                        .Where(a => a.FeatureCategoryKey == fCatGuid)
                        .Select(a => (int?)a.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fCatId.HasValue)
                        query = query.Where(s => s.FeatureCategoryId == fCatId.Value);
                }

                if (!string.IsNullOrEmpty(bookingStatus))
                    query = query.Where(s => s.BookingStatus == bookingStatus);

                // COUNT is deferred — fetched separately after rows render.
                var rows = await query
                    .OrderByDescending(s => s.OrderRequestServiceId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new
                    {
                        s.OrderRequestServiceId,
                        s.BookingId,
                        s.FirstName,
                        s.LastName,
                        s.MemberNameTitle,
                        MemberName = s.Member.Name,
                        MemberEmail = s.Member.Email,
                        MemberPhone = s.Member.PhoneNumber,
                        s.Email,
                        s.PhoneNo,
                        s.ExtraCharge,
                        s.TotalPrice,
                        s.IsPackage,
                        s.BookingDate,
                        s.BookingTime,
                        s.BookingStatus,
                        s.PaymentStatus,
                        s.IsApprovedByAdmin
                    })
                    .ToListAsync();

                var formattedRows = rows.Select(s => new
                {
                    s.OrderRequestServiceId,
                    s.BookingId,
                    MemberName = (!string.IsNullOrEmpty(s.FirstName)
                        ? ((s.MemberNameTitle ?? "") + " " + s.FirstName + " " + (s.LastName ?? "")).Trim()
                        : s.MemberName ?? ""),
                    Email = s.Email ?? s.MemberEmail ?? "",
                    PhoneNo = s.PhoneNo ?? s.MemberPhone ?? "",
                    ExtraCharge = s.ExtraCharge.ToString("F2"),
                    TotalPrice = (s.IsPackage ? "0.00" : s.TotalPrice.ToString("F2")),
                    BookingDate = s.BookingDate.ToString("dd/MM/yyyy"),
                    BookingTime = s.BookingTime ?? "",
                    BookingStatus = s.BookingStatus ?? "",
                    PaymentStatus = s.PaymentStatus ?? "",
                    s.IsApprovedByAdmin
                }).ToList();

                return Json(new { success = true, rows = formattedRows });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, rows = new object[0] });
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {

                var data = await _orderRequestServiceDataAccess.Details(id);
                if (data != null)
                {
                    var fCategory = await _featureCategoryAccess.GetById(data.FeatureCategoryId.Value);
                    ViewBag.FCatagoryKey = fCategory.FeatureCategoryKey.ToString();
                    ViewBag.Package = data.IsPackage;
                }
                return View(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<ActionResult> Invoice(int id)
        {
            try
            {

                var data = await _orderRequestServiceDataAccess.Details(id);
                if (data != null)
                {
                    var fCategory = await _featureCategoryAccess.GetById(data.FeatureCategoryId.Value);
                    ViewBag.FCatagoryKey = fCategory.FeatureCategoryKey.ToString();
                    ViewBag.Package = data.IsPackage;
                }
                return View(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<ActionResult> DelivaryShip(int id)
        {
            try
            {

                var data = await _orderRequestServiceDataAccess.Details(id);
                if (data != null)
                {
                    var fCategory = await _featureCategoryAccess.GetById(data.FeatureCategoryId.Value);
                    ViewBag.FCatagoryKey = fCategory.FeatureCategoryKey.ToString();
                    ViewBag.Package = data.IsPackage;
                }
                return View(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }


        #region Order Service

        public async Task<ActionResult> CreateInvoice(string fCatagoryKey)
        {
            try
            {
                await LoadDdl();

                var feacherCategory = await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey);
                var model = new OrderRequestService();
                model.FeatureCategory = feacherCategory.FirstOrDefault();
                model.BookingDate = Helper.DateTimeHelper.DubaiTime();

                model.FeatureCategoryId = model.FeatureCategory.FeatureCategoryId;
                model.OrderRequestServiceDetailList = new List<OrderRequestServiceDetails>();
                var orderProduct = new OrderRequestServiceDetails();
                orderProduct.Service = new Boulevard.Models.Service();
                model.OrderRequestServiceDetailList.Add(orderProduct);

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task LoadDdl()
        {
        


            var role = await _countryDataAccess.GetAll();
            List<SelectListItem> selectRole = role.Select(l => new SelectListItem
            {
                Value = l.CountryId.ToString(),
                Text = l.CountryName

            }).ToList();
            ViewBag.Country = selectRole;

            var city = await _cityDataAccess.GetAll();
            var data1 = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = ""
            };
            List<SelectListItem> selectCity = new List<SelectListItem>();
            selectCity.Add(data1);
            ViewBag.City = selectCity;

        }

        #endregion

        [HttpPost]
        public JsonResult Approved(int orderRequestServiceId)
        {
            try
            {
                var db = new BoulevardDbContext();
                var orderRequestService = db.OrderRequestService.Where(t => t.OrderRequestServiceId == orderRequestServiceId).FirstOrDefault();
                if(orderRequestService != null)
                {
                    orderRequestService.IsApprovedByAdmin = true;
                    orderRequestService.ApprovedBy = GetUser().UserId;
                    db.Entry(orderRequestService).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new { success = true, responseText = "Successfull" });
            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Failed" });
            }
        }
        
        [HttpPost]
        public ActionResult Quotation(int orderRequestServiceId, double? quotedPrice, string quotationNote, HttpPostedFileBase quotationFileLink)
        {
            try
            {
                var db = new BoulevardDbContext();
                var result = new OrderRequestService();
                var fileLink = "";
                if (quotationFileLink != null)
                {
                    var path = "Content/uploads/InsuranceQuotationPrice";
                    fileLink = ImageProcess.UploadFileImport(quotationFileLink, path);
                }
                var orderRequestService = db.OrderRequestService.Where(t => t.OrderRequestServiceId == orderRequestServiceId).FirstOrDefault();
                if(orderRequestService != null)
                {
                    orderRequestService.QuotedPrice = quotedPrice.Value;
                    orderRequestService.QuotationNote = quotationNote;
                    orderRequestService.QuotationFileLink = fileLink;
                    orderRequestService.IsApprovedByAdmin = true;
                    orderRequestService.ApprovedBy = GetUser().UserId;
                    db.Entry(orderRequestService).State = EntityState.Modified;
                    db.SaveChanges();
                }



                return RedirectToAction("Details", "OrderRequestService", new { id = orderRequestService.OrderRequestServiceId });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult QuotationForPackage(int orderRequestServiceId, double? quotedPrice, string quotationNote, HttpPostedFileBase quotationFileLink)
        {
            try
            {
                var db = new BoulevardDbContext();
                var result = new OrderRequestService();
                var fileLink = "";
                if (quotationFileLink != null)
                {
                    var path = "Content/uploads/HotelAndFlightQuotationPrice";
                    fileLink = ImageProcess.UploadFileImport(quotationFileLink, path);
                }
                var orderRequestService = db.OrderRequestService.Where(t => t.OrderRequestServiceId == orderRequestServiceId).FirstOrDefault();
                if (orderRequestService != null)
                {
                    orderRequestService.QuotedPrice = quotedPrice.Value;
                    orderRequestService.QuotationNote = quotationNote;
                    orderRequestService.QuotationFileLink = fileLink;
                    orderRequestService.IsApprovedByAdmin = true;
                    orderRequestService.ApprovedBy = GetUser().UserId;
                    db.Entry(orderRequestService).State = EntityState.Modified;
                    db.SaveChanges();
                }



                return RedirectToAction("Details", "OrderRequestService", new { id = orderRequestService.OrderRequestServiceId });
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<ActionResult> MotorsIndex()
        {
            return View();
        }

        public async Task<ActionResult> MotorsDetails()
        {
            return View();
        }

        public async Task<ActionResult> SalonIndex()
        {
            return View();
        }

        public async Task<ActionResult> SalonDetails()
        {
            return View();
        }

        public async Task<ActionResult> MedicalIndex()
        {
            return View();
        }

        public async Task<ActionResult> MedicalDetails()
        {
            return View();
        }
    }
}