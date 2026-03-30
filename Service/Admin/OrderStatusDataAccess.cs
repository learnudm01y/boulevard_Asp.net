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
    public class OrderStatusDataAccess
    {
        public IUnitOfWork uow;

        public OrderStatusDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from Order Status
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrderStatus>> GetAll()
        {
            try
            {
                return await uow.OrderStatusRepository.GetAll().Where(a => a.Status == "Active").OrderBy(t => t.OrderStatusId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Order Status By Id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<OrderStatus> GetOrderStatusById(int id)
        {
            try
            {
                return await uow.OrderStatusRepository.GetAll(a => a.OrderStatusId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Order Status By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<OrderStatus> GetOrderStatusByKey(string key)
        {
            try
            {
                return await uow.OrderStatusRepository.GetAll(a => a.StatusKey.ToString() == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create OrdervStatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OrderStatus> Create(OrderStatus model)
        {
            try
            {
                model.StatusKey = Guid.NewGuid();
                return await uow.OrderStatusRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update Order Status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<OrderStatus> Update(OrderStatus model)
        {
            try
            {
                return await uow.OrderStatusRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}