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
using System.Web.Services.Description;
using System.Xml.Linq;

namespace Boulevard.Service
{
    public class ServiceAmenityAccess
    {
        public IUnitOfWork uow;
        public ServiceAmenityAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<ServiceAmenity>> GetAll()
        {
            try
            {
                return await uow.ServiceAmenityRepository.Get().Where(e => e.Status.ToLower() == "active").Include(p => p.Service).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Get All ServiceAmenity By FeatureCategory
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<ServiceAmenity>> GetAllServiceAmenityByFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var serviceAmenityList = new List<ServiceAmenity>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {

                    var fCategoryId = await uow.FeatureCategoryRepository.Get().Where(a => a.FeatureCategoryKey.ToString() == fCatagoryKey).Select(a => a.FeatureCategoryId).FirstOrDefaultAsync();

                    var service = await uow.ServiceRepository.Get().Where(a => a.FeatureCategoryId == fCategoryId).ToListAsync();
                    if (service != null)
                    {
                        foreach (var serviceNode in service)
                        {
                            var serviceAmenity = await uow.ServiceAmenityRepository.GetAll().ToListAsync();
                            if (serviceAmenity != null)
                            {
                                foreach (var serviceAmenityNode in serviceAmenity.Where(a => a.ServiceId == serviceNode.ServiceId))
                                {
                                    serviceAmenityList.Add(serviceAmenityNode);
                                }
                            }
                        }
                    }
                }
                else
                {
                    serviceAmenityList = await uow.ServiceAmenityRepository.Get().Where(e => e.Status.ToLower() == "active").Include(p => p.Service).ToListAsync();
                }
                return serviceAmenityList;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Get All ServiceAmenity For Package
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<ServiceAmenity>> GetAllServiceAmenityForPackage(string fCatagoryKey)
        {
            try
            {
                var serviceAmenityList = new List<ServiceAmenity>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {

                    var fCategoryId = await uow.FeatureCategoryRepository.Get().Where(a => a.FeatureCategoryKey.ToString() == fCatagoryKey).Select(a => a.FeatureCategoryId).FirstOrDefaultAsync();

                    var service = await uow.ServiceRepository.Get().Where(a => a.FeatureCategoryId == fCategoryId && a.IsPackage == true).ToListAsync();
                    if (service != null)
                    {
                        foreach (var serviceNode in service)
                        {
                            var serviceAmenity = await uow.ServiceAmenityRepository.GetAll().ToListAsync();
                            if (serviceAmenity != null)
                            {
                                foreach (var serviceAmenityNode in serviceAmenity.Where(a => a.ServiceId == serviceNode.ServiceId))
                                {
                                    serviceAmenityList.Add(serviceAmenityNode);
                                }
                            }
                        }
                    }
                }
                else
                {
                    serviceAmenityList = await uow.ServiceAmenityRepository.Get().Where(e => e.Status.ToLower() == "active").Include(p => p.Service).ToListAsync();
                }
                return serviceAmenityList;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceAmenity> GetById(int id)
        {
            try
            {
                return await uow.ServiceAmenityRepository.GetById(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get By key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ServiceAmenity> GetByKey(Guid key)
        {
            try
            {
                return await uow.ServiceAmenityRepository.Get().Where(t => t.ServiceAmenityKey == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <returns></returns>
        public async Task Insert(ServiceAmenity node)
        {
            try
            {
                node.ServiceAmenityKey = Guid.NewGuid();
                node.CreateDate = DateTimeHelper.DubaiTime();

                node.Status = "Active";
                await uow.ServiceAmenityRepository.Add(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        public async Task Update(ServiceAmenity node)
        {
            try
            {
                node.UpdateDate = DateTimeHelper.DubaiTime();
                node.Status = "Active";
                await uow.ServiceAmenityRepository.Edit(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateby"></param>
        /// <returns></returns>
        public async Task<bool> Delete(Guid id, int updateby)
        {
            try
            {
                var exitResult = await GetByKey(id);
                if (exitResult != null)
                {
                    exitResult.DeleteBy = 1;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    exitResult.Status = "Deleted";
                    await uow.ServiceAmenityRepository.Edit(exitResult);
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

        /// <summary>
        /// Upload Image
        /// </summary>
        /// <param name="flagImage"></param>
        /// <returns></returns>
        public string UploadImage(HttpPostedFileBase flagImage)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/ServiceAmenity";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }

    }
}