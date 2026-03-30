using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.Admin
{
    public class ServiceTypeFileDataAccess
    {
        public IUnitOfWork uow;
        public ServiceTypeFileDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get Service Type File By Service Amenity Id
        /// </summary>
        /// <param name="serviceTypeAmenityId"></param>
        /// <returns></returns>
        public async Task<ServiceTypeFile> GetServiceTypeFileByServiceAmenityId(int serviceTypeAmenityId)
        {
            try
            {
                return await uow.ServiceTypeFileRepository.GetAll(a => a.ServiceAmenityId == serviceTypeAmenityId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create Service Type File
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ServiceTypeFile> Create(ServiceTypeFile model)
        {
            try
            {
                model.LastUpdate = DateTime.Now;
                return await uow.ServiceTypeFileRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update Service Type File
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(ServiceTypeFile model)
        {
            try
            {
                model.LastUpdate = DateTime.Now;
                var db = new BoulevardDbContext();
                //return await uow.ServiceTypeFileRepository.Edit(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
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
            string Url = "/Content/Upload/ServiceTypeFile";
            ImageName = MediaHelper.UploadFileImport(flagImage, Url);
            return ImageName;
        }

        /// <summary>
        /// Delete Image
        /// </summary>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImage(int ImageId)
        {
            try
            {
                var exitResult = uow.ServiceTypeFileRepository.GetbyId(ImageId);
                if (exitResult != null)
                {
                    uow.ServiceTypeFileRepository.Remove(exitResult);
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