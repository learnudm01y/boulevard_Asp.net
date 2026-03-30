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

namespace Boulevard.Service
{
    public class ServiceLandmarkAccess
    {
        public IUnitOfWork uow;
        public ServiceLandmarkAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<ServiceLandmark>> GetAll()
        {
            try
            {
                return await uow.ServiceLandmarkRepository.Get().Where(e => e.Status.ToLower()== "active").Include(p=> p.Service).ToListAsync();
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
        public async Task<ServiceLandmark> GetById(int id)
        {
            try
            {
                return await uow.ServiceLandmarkRepository.GetById(id);
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
        public async Task<ServiceLandmark> GetByKey(Guid key)
        {
            try
            {
                return await uow.ServiceLandmarkRepository.Get().Where(t => t.ServiceLandmarkKey == key).FirstOrDefaultAsync();
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
        public async Task Insert(ServiceLandmark node)
        {
            try
            {
                node.ServiceLandmarkKey = Guid.NewGuid();
                node.CreateDate = DateTimeHelper.DubaiTime();

                node.Status = "Active";
                await uow.ServiceLandmarkRepository.Add(node);
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
        public async Task Update(ServiceLandmark node)
        {
            try
            {
                node.UpdateDate = DateTimeHelper.DubaiTime();
                node.Status = "Active";
                await uow.ServiceLandmarkRepository.Edit(node);
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
                    await uow.ServiceLandmarkRepository.Edit(exitResult);
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
            string Url = "/Content/Upload/ServiceLandmark";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }
        
    }
}