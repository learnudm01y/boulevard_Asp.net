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
    public class VehicalModelDataAccess
    {
        public IUnitOfWork uow;

        public VehicalModelDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from Vehical Model
        /// </summary>
        /// <returns></returns>
        public async Task<List<VehicalModel>> GetAll()
        {
            try
            {
                return await uow.VehicalModelRepository.GetAll().Where(a => a.Status != "Delete").Include(p => p.Brand).OrderByDescending(t => t.VehicalModelId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Vehical Model By Id
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<VehicalModel> GetVehicalModelById(int modelId)
        {
            try
            {
                return await uow.VehicalModelRepository.GetAll(a => a.VehicalModelId == modelId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Vehical Model By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<VehicalModel> GetVehicalModelByKey(string key)
        {
            try
            {
                return await uow.VehicalModelRepository.GetAll(a => a.VehicalModelKey.ToString() == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create Vehical Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<VehicalModel> Create(VehicalModel model)
        {
            try
            {
                model.VehicalModelKey = Guid.NewGuid();
                return await uow.VehicalModelRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update Vehical Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<VehicalModel> Update(VehicalModel model)
        {
            try
            {
                return await uow.VehicalModelRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}