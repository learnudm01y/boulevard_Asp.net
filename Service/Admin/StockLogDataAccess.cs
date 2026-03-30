using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
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
    public class StockLogDataAccess
    {
        public IUnitOfWork uow;

        public StockLogDataAccess()
        {
            uow = new UnitOfWork();
        }


        public async Task StockInProduct(int productId, int stockin, int adminId, int productPriceId)
        {
            try
            {
                var model = new StockLog();
                model.StockKey = Guid.NewGuid();
                model.ProductId = productId;
                model.StockDate = DateTimeHelper.DubaiTime();
                model.StockIn = stockin;
                model.StockOut = 0;
                model.ProductPriceId = productPriceId;
                model.CreateDate = DateTimeHelper.DubaiTime();
                model.CreatedBy = adminId;
                model.StockType = "In";
                model.OrderMasterId = 0;
                model.UserType = "Admin";
                var product = await uow.ProductRepository.GetById(productId);
                if (product != null)
                {
                    model.FeatureCategoryId = product.FeatureCategoryId.Value;
                }
                StockLog stockOut = await uow.StockLogRepository.Add(model);

                //var productPrice = await uow.ProductPriceRepository.Get().Where(s => s.ProductId == productId && s.ProductPriceId == productId).FirstOrDefaultAsync();

                //if (productPrice != null)
                //{
                //    productPrice.ProductStock = productPrice.ProductStock + stockOut.StockIn;
                //    await uow.ProductPriceRepository.Edit(productPrice);
                //}

            }
            catch (Exception ex)
            {

            }
        }

        public async Task StockIn(int productId, int stockin, int adminId,int productPriceId)
        {
            try
            {
                var model = new StockLog();
                model.StockKey = Guid.NewGuid();
                model.ProductId = productId;
                model.StockDate = DateTimeHelper.DubaiTime();
                model.StockIn = stockin;
                model.StockOut = 0;
                model.ProductPriceId = productPriceId;
                model.CreateDate = DateTimeHelper.DubaiTime();
                model.CreatedBy = adminId;
                model.StockType = "In";
                model.OrderMasterId = 0;
                model.UserType = "Admin";
                var product = await uow.ProductRepository.GetById(productId);
                if (product != null)
                {
                    model.FeatureCategoryId = product.FeatureCategoryId.Value;
                }
                StockLog stockOut = await uow.StockLogRepository.Add(model);

                var productPrice = await uow.ProductPriceRepository.Get().Where(s => s.ProductId == productId && s.ProductPriceId == productId).FirstOrDefaultAsync();

                if (productPrice != null)
                {
                    productPrice.ProductStock = productPrice.ProductStock + stockOut.StockIn;
                    await uow.ProductPriceRepository.Edit(productPrice);
                }

            }
            catch (Exception ex)
            {
               
            }
        }
        
        public async Task StockOut(int productId, int orderId, int stockout, int memberId,int productPriceId)
        {
            try
            {
                var model = new StockLog();
                model.StockKey = Guid.NewGuid();
                model.ProductId = productId;
                model.StockDate = DateTimeHelper.DubaiTime();
                model.StockIn = 0;
                model.StockOut = stockout;
                model.CreateDate = DateTimeHelper.DubaiTime();
                model.CreatedBy = memberId;
                model.ProductPriceId = productPriceId;
                model.StockType = "Out";
                model.OrderMasterId = orderId;
                model.UserType = "Member";
                var product = await uow.ProductRepository.GetById(productId);
                if (product != null)
                {
                    model.FeatureCategoryId = product.FeatureCategoryId.Value;
                }
                StockLog stockOut = await uow.StockLogRepository.Add(model);

                var productPrice = await uow.ProductPriceRepository.Get().Where(s => s.ProductId == productId && s.ProductPriceId == productPriceId).FirstOrDefaultAsync();
              
                if (productPrice != null)
                {
                    productPrice.ProductStock = productPrice.ProductStock - stockOut.StockOut;
                    await uow.ProductPriceRepository.Edit(productPrice);
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        public async Task<List<StockLog>> GetAllStockLogByProductId(int productId)
        {
            try
            {
                var stockLog = await uow.StockLogRepository.GetAll().Where(a => a.ProductId == productId).Include(p => p.Product).Include(f => f.FeatureCategory).OrderByDescending(t => t.StockLogId).ToListAsync();

                foreach (var item in stockLog)
                {
                    var productPrice = await uow.ProductPriceRepository.Get().Where(a => a.ProductPriceId == item.ProductPriceId).FirstOrDefaultAsync();
                    item.ProductPrice = productPrice != null ? productPrice : new ProductPrice();
                }
                return stockLog;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<List<ProductTypeMaster>> GetAllProductType()
        {
            try
            {
                return await uow.ProductTypeMasterRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.ProductTypeId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}