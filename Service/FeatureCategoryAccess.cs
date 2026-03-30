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
    public class FeatureCategoryAccess
    {
        public IUnitOfWork uow;
        public FeatureCategoryAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<FeatureCategory>> GetAll()
        {
            try
            {
                    return await uow.FeatureCategoryRepository.Get().Where(e => e.IsDelete==false).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        
        public async Task<List<FeatureCategory>> GetAllByFCatagoryKey(string fCatagoryKey)
        {
            try
            {
                var data = new List<FeatureCategory>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    data = await uow.FeatureCategoryRepository.Get().Where(e => e.IsActive && e.FeatureCategoryKey.ToString() == fCatagoryKey && e.IsDelete==false).ToListAsync();
                }
                else
                {
                    data = await uow.FeatureCategoryRepository.Get().Where(e => e.IsActive && e.IsDelete == false).ToListAsync();
                }
                return data;
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
        public async Task<FeatureCategory> GetById(int id)
        {
            try
            {
                return await uow.FeatureCategoryRepository.GetById(id);
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
        public async Task<FeatureCategory> GetByKey(Guid key)
        {
            try
            {
                return await uow.FeatureCategoryRepository.Get().Where(t => t.FeatureCategoryKey == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
         
        /// <summary>
        /// Get Feature Category Name
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetFeatureCategoryName(string fCatagoryKey)
        {
            try
            {
                return await uow.FeatureCategoryRepository.Get().Where(t => t.FeatureCategoryKey.ToString() == fCatagoryKey).Select(n => n.Name).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        //public async Task<string> GetFeatureByCategoryName(string featureCategoryName)
        //{
        //    try
        //    {
        //        return await uow.FeatureCategoryRepository.Get().Where(t => t.Name.ToString() == featureCategoryName.ToLower().Trim()).Select(n => n.Name).FirstOrDefaultAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        return null;
        //    }
        //}
        /// <summary>
        /// Insert
        /// </summary>
        /// <returns></returns>
        public async Task Insert(FeatureCategory node)
        {
            try
            {
                node.FeatureCategoryKey = Guid.NewGuid();
                node.IsActive = true;
                node.IsDelete = false;
                await uow.FeatureCategoryRepository.Add(node);
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
        public async Task Update(FeatureCategory node)
        {
            try
            {
                //node.IsActive = true;
                await uow.FeatureCategoryRepository.Edit(node);
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
        public async Task<bool> Delete(Guid key, int updateby)
        {
            try
            {
                var exitResult = await GetByKey(key);
                if (exitResult != null)
                {
                    exitResult.IsDelete = true;
                    await uow.FeatureCategoryRepository.Edit(exitResult);
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
            string Url = "/Content/Upload/FeatureCategory";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }

    }
}