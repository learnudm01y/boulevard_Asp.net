using Boulevard.Areas.Admin.Data;
using Boulevard.BaseRepository;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.Admin
{
    public class DashboardDataAccess
    {
        public IUnitOfWork uow;

        public DashboardDataAccess()
        {
            uow = new UnitOfWork();
        }


        public async Task<DashboardViewModel> GetAll()
        {
            try
            {
                DateTime currDate = DateTime.Now;
                DateTime prevWeekDate = currDate.AddDays(-6);
                int currentMonth = currDate.Month;

                var vm = new DashboardViewModel();

                // --- Totals (sequential - EF6 single-context limitation) ---
                vm.TotalCustomer = await uow.MemberRepository.Get()
                    .Where(s => s.Status == "Active").CountAsync();

                vm.TotalProductOrder = await uow.OrderRequestProductRepository.Get().CountAsync();

                vm.TotalServiceOrder = await uow.OrderRequestServiceRepository.Get().CountAsync();

                // Use nullable SumAsync to avoid exception on empty table (SQL SUM returns NULL on no rows)
                vm.TotalProductSales = await uow.OrderRequestProductRepository.Get()
                    .SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                vm.TotalServiceSales = await uow.OrderRequestServiceRepository.Get()
                    .SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                // --- Recent members ---
                vm.Member = await uow.MemberRepository.Get()
                    .Where(s => s.Status == "Active")
                    .OrderByDescending(s => s.MemberId)
                    .Take(10).ToListAsync();

                // --- Categories list (one query) ---
                vm.Categories = await uow.FeatureCategoryRepository
                    .GetAll(s => s.IsActive == true)
                    .OrderBy(s => s.FeatureCategoryId).ToListAsync();

                // --- Category-wise order counts via two GROUP BY queries (replaces N+1 loop) ---
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

                foreach (var cat in vm.Categories)
                {
                    if (cat.FeatureType == "Product")
                    {
                        var match = productCatCounts.FirstOrDefault(c => c.CategoryId == cat.FeatureCategoryId);
                        if (match != null) cat.CategoryWiseOrderCount = match.Count;
                    }
                    else if (cat.FeatureType == "Service")
                    {
                        var match = serviceCatCounts.FirstOrDefault(c => c.CategoryId == cat.FeatureCategoryId);
                        if (match != null) cat.CategoryWiseOrderCount = match.Count;
                    }
                }

                // --- Monthly stats (single query per stat - no double AnyAsync+query) ---
                vm.TotalProductOrderMonth = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.OrderDateTime.Month == currentMonth).CountAsync();

                vm.TotalProductSaleMonth = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.OrderDateTime.Month == currentMonth)
                    .SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                vm.TotalProductOrderWeek = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.DeliveryDateTime > prevWeekDate && s.DeliveryDateTime <= currDate)
                    .CountAsync();

                vm.TotalProductSaleWeek = await uow.OrderRequestProductRepository.Get()
                    .Where(s => s.DeliveryDateTime > prevWeekDate && s.DeliveryDateTime <= currDate)
                    .SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                vm.TotalServiceOrderMonth = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.BookingDate.Month == currentMonth).CountAsync();

                vm.TotalServiceSaleMonth = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.BookingDate.Month == currentMonth)
                    .SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                vm.TotalServiceOrderWeek = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.BookingDate > prevWeekDate && s.BookingDate <= currDate)
                    .CountAsync();

                vm.TotalServiceSaleWeek = await uow.OrderRequestServiceRepository.Get()
                    .Where(s => s.BookingDate > prevWeekDate && s.BookingDate <= currDate)
                    .SumAsync(s => (double?)s.TotalPrice) ?? 0.0;

                return vm;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<List<OrderRequestViewModel>> GetlastMonthProductOrder()
        {

            var today = Helper.DateTimeHelper.DubaiTime();
            var FirstDayofMonth = new DateTime(today.Year, today.Month, 1);

            var models = new List<OrderRequestViewModel>();

            var orders10 = await uow.OrderRequestProductRepository.Get().Where(e => e.OrderDateTime >= FirstDayofMonth).ToListAsync();
            var orders = orders10.GroupBy(s => s.OrderDateTime.Date).ToList();

            foreach (var order in orders)
            {
                var orderRequestViewModel = new OrderRequestViewModel();
                orderRequestViewModel.Date = order.Key;
                orderRequestViewModel.FormertDate = order.Key.ToString("MMM dd");
                orderRequestViewModel.Time = "";
                orderRequestViewModel.Count = order.Count();
                models.Add(orderRequestViewModel);

            }

            return models;
        }


        public async Task<List<OrderRequestViewModel>> GetlastMonthServiceSales()
        {

            var today = Helper.DateTimeHelper.DubaiTime();
            var FirstDayofMonth = new DateTime(today.Year, today.Month, 1);

            var models = new List<OrderRequestViewModel>();

            var orders10 = await uow.OrderRequestServiceRepository.Get().Where(e => e.BookingDate >= FirstDayofMonth).ToListAsync();
            var orders = orders10.GroupBy(s => s.BookingDate.Date).ToList();

            foreach (var order in orders)
            {
                var orderRequestViewModel = new OrderRequestViewModel();
                orderRequestViewModel.Date = order.Key;
                orderRequestViewModel.FormertDate = order.Key.ToString("MMM dd");
                orderRequestViewModel.Time = "";
                orderRequestViewModel.Count = order.Count();
                models.Add(orderRequestViewModel);

            }

            return models;
        }
    }
}