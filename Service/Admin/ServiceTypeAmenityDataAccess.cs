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
using System.Xml.Linq;

namespace Boulevard.Service.Admin
{
    public class ServiceTypeAmenityDataAccess
    {
        public IUnitOfWork uow;

        public ServiceTypeAmenityDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All Service Type Amenity
        /// </summary>
        /// <returns></returns>
        public async Task<List<ServiceTypeAmenity>> GetAll()
        {
            try
            {
                return await uow.ServiceTypeAmenityRepository.GetAll().Where(a => a.Status != "Deleted").OrderByDescending(t => t.ServiceAmenityId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Service Type Amenity By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ServiceTypeAmenity> GetServiceTypeAmenityById(int id)
        {
            try
            {
                return await uow.ServiceTypeAmenityRepository.GetAll(a => a.ServiceAmenityId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Service Type Amenity By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ServiceTypeAmenity> GetServiceTypeAmenityByKey(string key)
        {
            try
            {
                var result = await uow.ServiceTypeAmenityRepository.GetAll(a => a.ServiceAmenityKey.ToString() == key).FirstOrDefaultAsync();
                if (result != null)
                {
                    result.ServiceTypeFile = await uow.ServiceTypeFileRepository.Get().Where(a => a.ServiceAmenityId == result.ServiceAmenityId).FirstOrDefaultAsync();
                    if (result.ServiceTypeFile != null)
                    {
                        result.FileLink = result.ServiceTypeFile.FileLocation;
                        result.FileType = result.ServiceTypeFile.FileType;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create Service Type Amenity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceTypeAmenity> Create(ServiceTypeAmenity model)
        {
            try
            {
                model.ServiceAmenityKey = Guid.NewGuid();
                return await uow.ServiceTypeAmenityRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        //public async Task<ServiceTypeAmenity> Update(ServiceTypeAmenity model)
        //{
        //    try
        //    {
        //        var oldNode = await GetServiceTypeAmenityById(model.ServiceAmenityId);
        //        oldNode.UpdateBy = 1;
        //        oldNode.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
        //        oldNode.ServiceAmenityId = model.ServiceAmenityId;
        //        oldNode.AmenitiesType = model.AmenitiesType;
        //        oldNode.AmenitiesName = model.AmenitiesName;
        //        oldNode.LinkedWithFile = model.LinkedWithFile;
        //        oldNode.Status = model.Status;
        //        oldNode.ServiceTypeId = model.ServiceTypeId;
        //        oldNode.ServiceAmenityKey = model.ServiceAmenityKey;
        //        return await uow.ServiceTypeAmenityRepository.Edit(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        throw;
        //    }
        //}

        /// <summary>
        /// Update Service Type Amenity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceTypeAmenity> Update(ServiceTypeAmenity model)
        {
            try
            {
                var db = new BoulevardDbContext();
                model.UpdateBy = 1;
                model.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return model;
            }
            catch (Exception ex)
            {
                return null;
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
            string Url = "/Content/Upload/ServiceTypeAmenity";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }
    }
}