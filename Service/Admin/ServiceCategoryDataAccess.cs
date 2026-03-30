using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.Admin
{
    public class ServiceCategoryDataAccess
    {
        public IUnitOfWork uow;

        public ServiceCategoryDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All From Service Category
        /// </summary>
        /// <returns></returns>
        public async Task<List<ServiceCategory>> GetAll()
        {
            try
            {
                return await uow.ServiceCategoryRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.ServiceCategoryId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get All Service Category
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<ServiceCategory>> GetAllServiceCategory(string fCatagoryKey)
        {
            try
            {
                var list = new List<ServiceCategory>();
                var serviceTypes = new ServiceCategory();
                var featureCategoryId = await uow.FeatureCategoryRepository.Get().Where(a => a.FeatureCategoryKey.ToString() == fCatagoryKey).Select(a => a.FeatureCategoryId).FirstOrDefaultAsync();
                if (featureCategoryId > 0)
                {
                    var serviceIds = await uow.ServiceRepository.Get().Where(a => a.FeatureCategoryId == featureCategoryId).Select(a => a.ServiceId).ToListAsync();
                    if (serviceIds != null)
                    {
                        foreach (var serviceId in serviceIds)
                        {
                            var serviceCategoryData = await uow.ServiceCategoryRepository.Get().Where(e => e.Status.ToLower() == "active" && e.ServiceId == serviceId).Include(p => p.Service).ToListAsync();
                            if (serviceCategoryData != null && serviceCategoryData.Count() > 0)
                            {
                                list.AddRange(serviceCategoryData);
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// Get Service Category By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceCategory> GetServiceCategoryById(int id)
        {
            try
            {
                return await uow.ServiceCategoryRepository.GetAll(a => a.ServiceId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get City By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        //public async Task<ServiceCategory> GetServiceCategoryByKey(string key)
        //{
        //    try
        //    {
        //        return await uow.ServiceCategoryRepository.GetAll(a => a.CityKey.ToString() == key).FirstOrDefaultAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        throw;
        //    }
        //}

        /// <summary>
        /// Create Service Category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceCategory> Create(ServiceCategory model)
        {
            try
            {
                model.Status = "Active";
                return await uow.ServiceCategoryRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update Service Category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceCategory> Update(ServiceCategory model)
        {
            try
            {
                return await uow.ServiceCategoryRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var exitResult = await GetServiceCategoryById(id);
                if (exitResult != null)
                {
                    exitResult.Status = "Deleted";
                    await uow.ServiceCategoryRepository.Edit(exitResult);
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