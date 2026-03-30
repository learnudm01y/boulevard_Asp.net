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
    public class PropertyInformationDataAccess
    {
        public IUnitOfWork uow;

        public PropertyInformationDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All Property Information
        /// </summary>
        /// <returns></returns>
        public async Task<List<PropertyInformation>> GetAll()
        {
            try
            {
                return await uow.PropertyInformationRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.PropertyInformationId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Property Information By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PropertyInformation> GetPropertyInformationById(int id)
        {
            try
            {
                return await uow.PropertyInformationRepository.GetAll(a => a.PropertyInformationId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Property Information By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<PropertyInformation> GetPropertyInformationByKey(string key)
        {
            try
            {
                var result = await uow.PropertyInformationRepository.GetAll(a => a.PropertyInfoKey.ToString() == key).FirstOrDefaultAsync();
                result.ExteriorsImages = await uow.ServiceTypeFileRepository.GetAll(s => s.ServiceTypeId == result.ServiceTypeId && s.FileSource == "Exterior").ToListAsync();
                result.InteriorsImages = await uow.ServiceTypeFileRepository.GetAll(s => s.ServiceTypeId == result.ServiceTypeId && s.FileSource == "Interior").ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create Property Information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<PropertyInformation> Create(PropertyInformation model)
        {
            try
            {
                model.PropertyInfoKey = Guid.NewGuid();
                model.CreateBy = 1;
                model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                return await uow.PropertyInformationRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update Property Information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<PropertyInformation> Update(PropertyInformation model)
        {
            try
            {
                model.UpdateBy = 1;
                model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                await uow.PropertyInformationRepository.Edit(model);
                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}