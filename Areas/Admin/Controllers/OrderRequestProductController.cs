using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Boulevard.Service.WebAPI;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class OrderRequestProductController : Controller
    {
        private readonly OrderRequestProductDataAccess _orderRequestProductDataAccess;
        private readonly OrderStatusDataAccess _orderStatusDataAccess;
        private readonly FeatureCategoryAccess _featureCategoryAccess;
        private readonly BoulevardDbContext _context;
        private readonly CityDataAccess _cityDataAccess;
        private readonly CountryDataAccess _countryDataAccess;
        public OrderRequestServiceAccess _orderRequestServiceAccess;

        public OrderRequestProductController()
        {
            _orderRequestProductDataAccess = new OrderRequestProductDataAccess();
            _orderStatusDataAccess = new OrderStatusDataAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _context = new BoulevardDbContext();
            _cityDataAccess = new CityDataAccess();
            _countryDataAccess = new CountryDataAccess();
            _orderRequestServiceAccess = new OrderRequestServiceAccess();

        }
        // GET: Admin/OrderRequestProduct
        public async Task<ActionResult> Index(int? tabId, string fCatagoryKey)
        {
            try
            {
                ViewBag.FCatagoryKey = fCatagoryKey;
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    ViewBag.FCatagoryName = await _featureCategoryAccess.GetFeatureCategoryName(fCatagoryKey);
                }
                var orderStatus = await _orderStatusDataAccess.GetAll();
                ViewBag.OrderStatus = orderStatus;
                int orderStatusId = tabId.HasValue ? tabId.Value : orderStatus.Count > 0 ? orderStatus.Min(t => t.OrderStatusId) : 0;
                ViewBag.OrderStatusActiveId = orderStatusId;
                return View(new OrderRequestProductResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {

                var data = await _orderRequestProductDataAccess.Details(id);
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

                var data = await _orderRequestProductDataAccess.Details(id);
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

                var data = await _orderRequestProductDataAccess.Details(id);
                return View(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }



        #region  Create Invoice
        public async Task<ActionResult> CreateInvoice(string fCatagoryKey)
        {
            try
            {
                await LoadDdl();

                var feacherCategory = await _featureCategoryAccess.GetAllByFCatagoryKey(fCatagoryKey);
                var model = new OrderRequestProduct();
                model.FeatureCategory = feacherCategory.FirstOrDefault();
                model.OrderDateTime = Helper.DateTimeHelper.DubaiTime();
                model.DeliveryDateTime = Helper.DateTimeHelper.DubaiTime();
                model.FeatureCategoryId = model.FeatureCategory.FeatureCategoryId;
                var orderProduct =  new OrderRequestProductDetails();
                orderProduct.Product = new Product();
                model.OrderRequestProduxtDetails.Add(orderProduct);

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpPost]
        public async Task<ActionResult> CreateInvoice(OrderRequestProduct orderRequest)
        {
            try
            {
                await LoadDdl();
                TempData["ErrorMessage"] = "";
                var feacherCategory = await _featureCategoryAccess.GetById(orderRequest.FeatureCategoryId.Value);
                if (orderRequest.MemberId == 0)
                {
                    TempData["ErrorMessage"] = "Please select a member before proceeding.";
                    orderRequest.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                    orderRequest.FeatureCategory = feacherCategory;
                    return View(orderRequest);
                }
                if (orderRequest.MemberAddressId == 0)
                {
                    TempData["ErrorMessage"] = "Please select a address before proceeding.";
                    orderRequest.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                    orderRequest.FeatureCategory = feacherCategory;
                    return View(orderRequest);
                }
                if (orderRequest.OrderRequestProduxtDetails.Where(t=>t.Quantity > 0).Count() == 0)
                {
                    TempData["ErrorMessage"] = "Please add at least one product before proceeding.";
                    orderRequest.FeatureCategoryId = feacherCategory.FeatureCategoryId;
                    orderRequest.FeatureCategory = feacherCategory;
                    return View(orderRequest);
                }

                OrderSubmitRequest orderSubmitRequest = new OrderSubmitRequest();
                orderSubmitRequest.Details = new List<OrderDetailRequest>();
                orderSubmitRequest.MemberId = Convert.ToInt32(orderRequest.MemberId);
                orderSubmitRequest.featureCategoryId = feacherCategory.FeatureCategoryId;
                orderSubmitRequest.MemberAddressId = Convert.ToInt32(orderRequest.MemberAddressId.Value);
                orderSubmitRequest.TotalPrice = orderRequest.TotalPrice;
                orderSubmitRequest.DeliveryCharge = orderRequest.DeliveryCharge;
                orderSubmitRequest.Comments = orderRequest.Comments;
                orderSubmitRequest.PaymentMethodId = orderRequest.PaymentMethodId;
                orderSubmitRequest.DeliveryDateTime = orderRequest.DeliveryDateTime;

                foreach (var item in orderRequest.OrderRequestProduxtDetails)
                {
                    OrderDetailRequest orderDetailRequest = new OrderDetailRequest();
                    orderDetailRequest.ProductId = item.ProductId;
                    orderDetailRequest.Quantity = item.Quantity;
                    orderDetailRequest.GrossPrice = item.GrossPrice;
                    orderDetailRequest.MembershipDiscountAmount = item.DiscountAmount;
                    orderSubmitRequest.Details.Add(orderDetailRequest);
                }

                var result = await _orderRequestServiceAccess.InsertOrder(orderSubmitRequest);

                return RedirectToAction("Index", new { fCatagoryKey = feacherCategory.FeatureCategoryKey });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult SaveMember(string Name, string PhoneNumber, string Email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(PhoneNumber))
                {
                    return Json(new { success = false, message = "Name and Phone Number are required." });
                }

                // Optional: check if member already exists by phone or email
                var existingMember = _context.Members.FirstOrDefault(m => m.PhoneNumber == PhoneNumber);
                if (existingMember != null)
                {
                    return Json(new { success = false, message = "A member with this phone number already exists." });
                }

                var member = new Models.Member
                {
                    Name = Name,
                    PhoneNumber = PhoneNumber,
                    Email = Email,
                    CreateDate = Helper.DateTimeHelper.DubaiTime(),
                    MemberKey = Guid.NewGuid(),
                };

                _context.Members.Add(member);
                _context.SaveChanges();

                return Json(new { success = true, memberId = member.MemberId });
            }
            catch (Exception ex)
            {
                // You may log the exception
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }




        [HttpGet]
        public JsonResult SearchMember(string term)
        {
            var results = _context.Members
                .Where(m => m.Name.Contains(term) || m.PhoneNumber.Contains(term))
                .Select(m => new
                {
                    MemberId = m.MemberId,
                    Name = m.Name + " (" + m.PhoneNumber + ")",
                    Email = m.Email,
                    PhoneNumber = m.PhoneNumber,
                })
                .Take(10)
                .ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMemberAddresses(int memberId)
        {
            var addresses = _context.MemberAddresses
                .Where(a => a.MemberId == memberId)
                .Select(a => new
                {
                    MemberAddressId = a.MemberAddressId,
                    AddressLine1 = a.AddressLine1,// Must include AddressId
                    AddressLine2 = a.AddressLine2,
                    NearByAddress = a.NearByAddress,
                    CountryId =a.CountryId,
                    CityId = a.CityId,
                    MemberId=a.MemberId,
                })
                .ToList();

            return Json(addresses, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveAddress(MemberAddress model)
        {
            if (model.MemberAddressId == 0)
            {
                var address = new MemberAddress
                {
                    MemberId = model.MemberId,
                    CountryId = model.CountryId,
                    CityId = model.CityId,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    NearByAddress = model.NearByAddress,
                    MemberAddressKey = Guid.NewGuid(),
                    CreateDate =DateTimeHelper.DubaiTime()
                };
                _context.MemberAddresses.Add(address);
            }
            else
            {
                var existing = _context.MemberAddresses.Find(model.MemberAddressId);
                if (existing != null)
                {
                    existing.CountryId = model.CountryId;
                    existing.CityId = model.CityId;
                    existing.AddressLine1 = model.AddressLine1;
                    existing.AddressLine2 = model.AddressLine2;
                    existing.NearByAddress = model.NearByAddress;
                    existing.UpdateDate = DateTimeHelper.DubaiTime();
                }
                _context.MemberAddresses.AddOrUpdate(existing);

            }
            

            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult TriggerAction()
        {

            try
            {
                var db = new BoulevardDbContext();
                var orderProducts = db.OrderRequestProductss.Where(t => t.IsSound == false).ToList();
                var orderService = db.OrderRequestService.Where(t => t.IsSound == false).ToList();

                if (orderProducts.Any())
                {
                    foreach (var item in orderProducts)
                    {
                        item.IsSound = true; // Modify the tracked entity
                    }

                    db.SaveChanges(); // Save all changes in one call
                    return Json(new { success = true });
                }
                else if (orderService.Any())
                {
                    foreach (var item in orderService)
                    {
                        item.IsSound = true; // Modify the tracked entity
                    }

                    db.SaveChanges(); // Save all changes in one call
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }


            }
            catch (Exception)
            {
                throw;
            }
           
        }



        [HttpGet]
        public JsonResult SearchProduct(string term, int fcId)
        {
            var products = _context.Products
                .Where(p =>p.FeatureCategoryId == fcId && p.ProductName.Contains(term))
                .Select(p => new
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    StockQuantity = p.StockQuantity,
                    ProductImage = "https://boulevard.r-y-x.net/Areas/Admin/Content/assets/images/logo-dashboard.png",
                })
                .Take(10)
                .ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }


      



        #endregion


        [HttpPost]
        public async Task<JsonResult> UpdateStatus(List<Int64> productIds, int statusId)
        {
            try
            {
                await _orderRequestProductDataAccess.UpdateStatus(productIds, statusId);

                return Json(new { success = true, responseText = "Successfull" });
            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Failed" });
            }
        }

        // Returns the COUNT only — called after rows are already rendered.
        [HttpPost]
        public async Task<JsonResult> GetPagedOrdersCount(string fCatagoryKey, int statusId)
        {
            try
            {
                IQueryable<OrderRequestProduct> query = _context.OrderRequestProductss
                    .Where(a => a.Status == "Active" && a.OrderStatusId == statusId);

                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out var fCatGuid))
                {
                    var fCatId = await _context.featureCategories
                        .Where(a => a.FeatureCategoryKey == fCatGuid)
                        .Select(a => (int?)a.FeatureCategoryId)
                        .FirstOrDefaultAsync();
                    if (fCatId.HasValue)
                        query = query.Where(a => a.FeatureCategoryId == fCatId.Value);
                }

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
        public async Task<JsonResult> GetPagedOrders(string fCatagoryKey, int statusId, int page, int pageSize)
        {
            try
            {
                if (pageSize < 10 || pageSize > 500) pageSize = 20;
                if (page < 1) page = 1;

                IQueryable<OrderRequestProduct> query = _context.OrderRequestProductss
                    .Where(a => a.Status == "Active" && a.OrderStatusId == statusId);

                if (!string.IsNullOrEmpty(fCatagoryKey) && Guid.TryParse(fCatagoryKey, out var fCatGuid))
                {
                    var fCatId = await _context.featureCategories
                        .Where(a => a.FeatureCategoryKey == fCatGuid)
                        .Select(a => (int?)a.FeatureCategoryId)
                        .FirstOrDefaultAsync();

                    if (fCatId.HasValue)
                        query = query.Where(a => a.FeatureCategoryId == fCatId.Value);
                }

                // COUNT is deferred — fetched separately after rows render.
                var rows = await query
                    .OrderByDescending(t => t.OrderRequestProductId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(o => new
                    {
                        o.OrderRequestProductId,
                        o.ReadableOrderId,
                        MemberName = o.Member.Name,
                        OrderDate = o.OrderDateTime,
                        DeliveryDate = o.DeliveryDateTime,
                        o.TotalPrice,
                        OrderStatusName = o.OrderStatus.Name,
                        o.OrderStatusId
                    })
                    .ToListAsync();

                var formattedRows = rows.Select(o => new
                {
                    o.OrderRequestProductId,
                    o.ReadableOrderId,
                    MemberName = o.MemberName ?? "",
                    OrderDate = o.OrderDate.ToString("dd/MM/yyyy"),
                    DeliveryDate = o.DeliveryDate.ToString("dd/MM/yyyy"),
                    TotalPrice = o.TotalPrice.ToString("F2"),
                    OrderStatusName = o.OrderStatusName ?? "",
                    o.OrderStatusId
                }).ToList();

                return Json(new { success = true, rows = formattedRows });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false, rows = new object[0] });
            }
        }


        #region Drop Down

        private async Task LoadDdl()
        {
            var data = await _orderStatusDataAccess.GetAll();
            var item = new SelectListItem
            {
                Value = "0",
                Selected = true,
                Text = "Select Status"
            };
            List<SelectListItem> selectOrderStatus = data.Select(l => new SelectListItem
            {
                Value = l.OrderStatusId.ToString(),
                Text = l.Name

            }).ToList();
            selectOrderStatus.Add(item);
            ViewBag.OrderStatus = selectOrderStatus;


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

        public async Task<ActionResult> GroceryOrderList (int tabId=1)
        {
            ViewBag.OrderStatusActiveId = tabId;
            return View();
        }

        public async Task<ActionResult> GroceryOrderDetails(int? id = null)
        {
            try
            {
                if (!id.HasValue || id.Value <= 0)
                    return RedirectToAction(nameof(GroceryOrderList));

                var data = await _orderRequestProductDataAccess.Details(id.Value);
                return View(data);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }


        public async Task<ActionResult> FlowersandChocolatesOrderList(int tabId = 1)
        {
            ViewBag.OrderStatusActiveId = tabId;
            return View();
        }

        public async Task<ActionResult> FlowersandChocolatesOrderDetails()
        {
            try
            {


                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}