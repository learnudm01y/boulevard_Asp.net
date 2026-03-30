using Boulevard.BaseRepository;
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
    public class ProductPriceDataAccess
    {
        public IUnitOfWork uow;

        public ProductPriceDataAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<List<ProductPrice>> GetAllByProductId(int productId)
        {
            try
            {
                return await uow.ProductPriceRepository.GetAll().Where(a => a.Status == "Active" && a.ProductId == productId).OrderByDescending(t => t.ProductPriceId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<ProductPrice> GetById(int id)
        {
            try
            {
                return await uow.ProductPriceRepository.GetById(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<bool> Delete(int productPriceId)
        {
            try
            {
                var exitResult = await GetById(productPriceId);
                if (exitResult != null)
                {
                    exitResult.LastUpdateDate = DateTimeHelper.DubaiTime();
                    exitResult.Status = "Deleted";
                    await uow.ProductPriceRepository.Edit(exitResult);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }

        public async Task<bool> IsExisr(int productPriceId, int productId)
        {
            try
            {
                var result = await uow.ProductPriceRepository.Get().Where(a => a.ProductPriceId == productPriceId && a.ProductId == productId).FirstOrDefaultAsync();
                if (result != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }
    }
}