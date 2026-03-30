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
    public class CommonProductTagDataAcces
    {
        public IUnitOfWork uow;
        public CommonProductTagDataAcces()
        {
            uow = new UnitOfWork();
        }
        /// <summary>
        /// Get All By FCatagoryKey
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<CommonProductTag>> GetAllByFCatagoryKey(string fCatagoryKey)
        {
            try
            {
                var node = new List<CommonProductTag>();
                var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                if (fCatagory == null)
                {
                    node = await uow.CommonProductTagRepository.Get().Where(e => e.Status.ToLower() != "Deleted").ToListAsync();
                }
                else
                {
                    node = await uow.CommonProductTagRepository.Get().Where(e => e.Status.ToLower() != "Deleted" && e.FeatureCategoryId == fCatagory.FeatureCategoryId).ToListAsync();
                }
                return node;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Create Common Product Tag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CommonProductTag> Create(CommonProductTag model)
        {
            try
            {
                model.Status = "Active";
                return await uow.CommonProductTagRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Get Common Product Tag By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CommonProductTag> GetById(int id)
        {
            try
            {
                return await uow.CommonProductTagRepository.GetById(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Delete Common Product Tag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(int id)
        {
            try
            {
                var exitResult = await GetById(id);
                if (exitResult != null)
                {
                    exitResult.DeleteBy = 1;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    exitResult.Status = "Deleted";
                    await uow.CommonProductTagRepository.Edit(exitResult);
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