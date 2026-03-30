using Boulevard.Areas.Admin.Data;
using Boulevard.BaseRepository;
using Boulevard.Service.Admin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IUnitOfWork uow;
        private readonly DashboardDataAccess _dashboardDataAccess;
        public DashboardController()
        {
            uow = new UnitOfWork();
            _dashboardDataAccess = new DashboardDataAccess();
        }
        // GET: Admin/Dashboard
        public async Task<ActionResult> Index()
        {
            try
            {
                var vm = new DashboardViewModel();
                // Only run the two fast queries needed for server-side rendering.
                // All heavy aggregate stats are loaded via GetDashboardStats AJAX.
                vm.Member = await uow.MemberRepository.Get()
                    .Where(s => s.Status == "Active")
                    .OrderByDescending(s => s.MemberId)
                    .Take(10).ToListAsync();
                vm.Categories = await uow.FeatureCategoryRepository
                    .GetAll(s => s.IsActive == true)
                    .OrderBy(s => s.FeatureCategoryId).ToListAsync();
                return View(vm);
            }
            catch (Exception ex)
            {
                return View(new DashboardViewModel());
            }
        }

        // Returns all dashboard summary numbers asynchronously.
        // Called via AJAX after the page renders — does not block initial load.
        [HttpPost]
        public async Task<JsonResult> GetDashboardStats()
        {
            try
            {
                DateTime currDate     = DateTime.Now;
                DateTime prevWeekDate = currDate.AddDays(-6);
                int currentMonth      = currDate.Month;

                var totalCustomer      = await uow.MemberRepository.Get().Where(s => s.Status == "Active").CountAsync();
                var totalProductOrder  = await uow.OrderRequestProductRepository.Get().CountAsync();
                var totalServiceOrder  = await uow.OrderRequestServiceRepository.Get().CountAsync();
                var totalProductSales  = await uow.OrderRequestProductRepository.Get().SumAsync(s => (double?)s.TotalPrice) ?? 0.0;
                var totalServiceSales  = await uow.OrderRequestServiceRepository.Get().SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                var totalProductOrderMonth = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.OrderDateTime.Month == currentMonth).CountAsync();
                var totalProductSaleMonth  = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.OrderDateTime.Month == currentMonth).SumAsync(s => (double?)s.TotalPrice) ?? 0.0;
                var totalProductOrderWeek  = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.DeliveryDateTime > prevWeekDate && s.DeliveryDateTime <= currDate).CountAsync();
                var totalProductSaleWeek   = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.DeliveryDateTime > prevWeekDate && s.DeliveryDateTime <= currDate).SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                var totalServiceOrderMonth = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.BookingDate.Month == currentMonth).CountAsync();
                var totalServiceSaleMonth  = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.BookingDate.Month == currentMonth).SumAsync(s => (double?)s.TotalPrice) ?? 0.0;
                var totalServiceOrderWeek  = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.BookingDate > prevWeekDate && s.BookingDate <= currDate).CountAsync();
                var totalServiceSaleWeek   = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.BookingDate > prevWeekDate && s.BookingDate <= currDate).SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                var productCatCounts = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.FeatureCategoryId != null)
                    .GroupBy(s => s.FeatureCategoryId)
                    .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                    .ToListAsync();
                var serviceCatCounts = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.FeatureCategoryId != null)
                    .GroupBy(s => s.FeatureCategoryId)
                    .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                    .ToListAsync();

                var catCounts = new Dictionary<string, int>();
                foreach (var x in productCatCounts)
                    if (x.CategoryId.HasValue)
                        catCounts[x.CategoryId.Value.ToString()] = x.Count;
                foreach (var x in serviceCatCounts)
                    if (x.CategoryId.HasValue)
                    {
                        var key = x.CategoryId.Value.ToString();
                        catCounts[key] = (catCounts.ContainsKey(key) ? catCounts[key] : 0) + x.Count;
                    }

                return Json(new
                {
                    success              = true,
                    totalCustomer,
                    totalProductOrder,
                    totalServiceOrder,
                    totalProductSales    = totalProductSales.ToString("F2"),
                    totalServiceSales    = totalServiceSales.ToString("F2"),
                    totalProductOrderMonth,
                    totalProductSaleMonth  = totalProductSaleMonth.ToString("F2"),
                    totalProductOrderWeek,
                    totalProductSaleWeek   = totalProductSaleWeek.ToString("F2"),
                    totalServiceOrderMonth,
                    totalServiceSaleMonth  = totalServiceSaleMonth.ToString("F2"),
                    totalServiceOrderWeek,
                    totalServiceSaleWeek   = totalServiceSaleWeek.ToString("F2"),
                    catCounts
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return Json(new { success = false });
            }
        }

        //[HttpGet]
        //public async Task<JsonResult> LastMonthProductOrderstData()
        //{
        //    var chartViews = new List<OrderRequestViewModel>();
        //    chartViews = await _dashboardDataAccess.GetlastMonthProductOrder();
        //    return Json(chartViews.Where(t => t.Count > 0).ToList(), JsonRequestBehavior.AllowGet);
        //}

        //[HttpGet]
        //public async Task<JsonResult> LastMonthProductSalesData()
        //{
        //    var chartViews = new List<OrderRequestViewModel>();
        //    chartViews = await _dashboardDataAccess.GetlastMonthProductOrder();
        //    return Json(chartViews.Where(t => t.Count > 0).ToList(), JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public async Task<JsonResult> LastMonthProductOrderstData()
        {
            double[] Sales = new double[12];
            int year = DateTime.Now.Year;
            var monthlyData = await uow.OrderRequestProductRepository.Get()
                .Where(s => s.CreateDate.Year == year)
                .GroupBy(s => s.CreateDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();
            foreach (var item in monthlyData)
                if (item.Month >= 1 && item.Month <= 12)
                    Sales[item.Month - 1] = item.Count;
            return Json(Sales);
        }

        [HttpPost]
        public async Task<JsonResult> LastMonthProductSalesData()
        {
            double[] Sales = new double[12];
            int year = DateTime.Now.Year;
            var monthlyData = await uow.OrderRequestProductRepository.Get()
                .Where(s => s.CreateDate.Year == year)
                .GroupBy(s => s.CreateDate.Month)
                .Select(g => new { Month = g.Key, Sum = g.Sum(s => (double?)s.TotalPrice) ?? 0 })
                .ToListAsync();
            foreach (var item in monthlyData)
                if (item.Month >= 1 && item.Month <= 12)
                    Sales[item.Month - 1] = item.Sum;
            return Json(Sales);
        }

        [HttpPost]
        public async Task<JsonResult> LastMonthServiceOrdersData()
        {
            double[] Sales = new double[12];
            int year = DateTime.Now.Year;
            var monthlyData = await uow.OrderRequestServiceRepository.Get()
                .Where(s => s.BookingDate.Year == year)
                .GroupBy(s => s.BookingDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();
            foreach (var item in monthlyData)
                if (item.Month >= 1 && item.Month <= 12)
                    Sales[item.Month - 1] = item.Count;
            return Json(Sales);
        }

        [HttpPost]
        public async Task<JsonResult> LastMonthServiceSalesData()
        {
            double[] Sales = new double[12];
            int year = DateTime.Now.Year;
            var monthlyData = await uow.OrderRequestServiceRepository.Get()
                .Where(s => s.BookingDate.Year == year)
                .GroupBy(s => s.BookingDate.Month)
                .Select(g => new { Month = g.Key, Sum = g.Sum(s => (double?)s.TotalPrice) ?? 0 })
                .ToListAsync();
            foreach (var item in monthlyData)
                if (item.Month >= 1 && item.Month <= 12)
                    Sales[item.Month - 1] = item.Sum;
            return Json(Sales);
        }

        // Single combined endpoint — replaces the 4 separate AJAX calls above.
        // Returns all chart data in one round-trip and one DB connection.
        [HttpPost]
        public async Task<JsonResult> GetAllChartsData()
        {
            int year = DateTime.Now.Year;

            double[] productOrders = new double[12];
            double[] productSales  = new double[12];
            double[] serviceOrders = new double[12];
            double[] serviceSales  = new double[12];

            var pOrders = await uow.OrderRequestProductRepository.Get()
                .Where(s => s.CreateDate.Year == year)
                .GroupBy(s => s.CreateDate.Month)
                .Select(g => new { Month = g.Key, Val = (double)g.Count() })
                .ToListAsync();

            var pSales = await uow.OrderRequestProductRepository.Get()
                .Where(s => s.CreateDate.Year == year)
                .GroupBy(s => s.CreateDate.Month)
                .Select(g => new { Month = g.Key, Val = g.Sum(s => (double?)s.TotalPrice) ?? 0 })
                .ToListAsync();

            var sOrders = await uow.OrderRequestServiceRepository.Get()
                .Where(s => s.BookingDate.Year == year)
                .GroupBy(s => s.BookingDate.Month)
                .Select(g => new { Month = g.Key, Val = (double)g.Count() })
                .ToListAsync();

            var sSales = await uow.OrderRequestServiceRepository.Get()
                .Where(s => s.BookingDate.Year == year)
                .GroupBy(s => s.BookingDate.Month)
                .Select(g => new { Month = g.Key, Val = g.Sum(s => (double?)s.TotalPrice) ?? 0 })
                .ToListAsync();

            foreach (var x in pOrders)  if (x.Month >= 1 && x.Month <= 12) productOrders[x.Month - 1] = x.Val;
            foreach (var x in pSales)   if (x.Month >= 1 && x.Month <= 12) productSales [x.Month - 1] = x.Val;
            foreach (var x in sOrders)  if (x.Month >= 1 && x.Month <= 12) serviceOrders[x.Month - 1] = x.Val;
            foreach (var x in sSales)   if (x.Month >= 1 && x.Month <= 12) serviceSales [x.Month - 1] = x.Val;

            return Json(new {
                productOrders,
                productSales,
                serviceOrders,
                serviceSales
            });
        }
    }
}