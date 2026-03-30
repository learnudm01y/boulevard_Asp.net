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
    public class OfferAccess
    {
        public IUnitOfWork uow;
        public OfferAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All
        /// </summary>
        /// <returns></returns>
        public async Task<List<OfferInformation>> GetAll(int? featureCategoryId)
        {
            try
            {
                if (featureCategoryId != null && featureCategoryId > 0)
                {
                    return await uow.OfferInformationRepository.Get().Where(e =>  e.FeatureCategoryId == featureCategoryId &&  e.Status == "Active").ToListAsync();
                }
                else 
                {
                    return await uow.OfferInformationRepository.Get().Where(e => e.Status == "Active").ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<List<OfferInformation>> GetAllByFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var result = new List<Boulevard.Models.OfferInformation>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                    result = await uow.OfferInformationRepository.Get().Where(e => e.Status != "Deleted" && e.FeatureCategoryId == fCatagory.FeatureCategoryId).Include(p => p.FeatureCategory).OrderByDescending(s => s.OfferInformationId).ToListAsync();
                }
                else 
                {
                    result = await uow.OfferInformationRepository.Get().Where(e => e.Status == "Deleted").ToListAsync();
                }
                return result;
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
        public async Task<OfferInformation> GetById(int id)
        {
            try
            {
                return await uow.OfferInformationRepository.GetById(id);
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
        public async Task<OfferInformation> GetByKey(Guid key)
        {
            try
            {
                return await uow.OfferInformationRepository.Get().Where(t => t.OfferInformationKey == key).FirstOrDefaultAsync();
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
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<OfferInformation> Insert(OfferInformation node)
        {
            try
            {
                node.OfferInformationKey = Guid.NewGuid();
                node.Status = "Active";
                node.CreateBy = 1;
                node.CreateDate = DateTimeHelper.DubaiTime();
                return await uow.OfferInformationRepository.Add(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="OfferInformation"></param>
        /// <returns></returns>
        public async Task<OfferInformation> Update(OfferInformation node)
        //public OfferInformation Update(OfferInformation node)
        {
            try
            {
                var oldNode = await GetByKey(node.OfferInformationKey);
                if (oldNode != null)
                {
                    oldNode.Title = node.Title;
                    oldNode.TitleAr = node.TitleAr;
                    oldNode.Description = node.Description;
                    oldNode.DescriptionAr = node.DescriptionAr;
                    oldNode.FeatureCategoryId = node.FeatureCategoryId;
                    oldNode.IsBrand = node.IsBrand;
                    oldNode.IsCategory = node.IsCategory;
                    oldNode.IsProduct = node.IsProduct;
                    oldNode.IsService = node.IsService;
                    oldNode.FeatureType = node.FeatureType;
                    oldNode.IsTimeLimit = node.IsTimeLimit;
                    oldNode.IsTrending = node.IsTrending;
                    oldNode.StartDate = node.StartDate;
                    oldNode.EndDate = node.EndDate;
                    oldNode.FeatureType = node.FeatureType;
                    oldNode.Status = node.Status;
                    oldNode.UpdateDate = DateTimeHelper.DubaiTime();
                    oldNode.UpdateBy = 1;
                }
                //var db = new BoulevardDbContext();
                //node.UpdateBy = 1;
                //node.Status = "Active";
                //node.UpdateDate = DateTimeHelper.DubaiTime();
                //db.Entry(node).State = EntityState.Modified;
                //db.SaveChanges();
                return await uow.OfferInformationRepository.Edit(oldNode);
                //return node;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
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
                    exitResult.Status = "Deleted";
                    exitResult.DeleteBy = updateby;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    await uow.OfferInformationRepository.Edit(exitResult);
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
            string Url = "/Content/Upload/OfferInformation";
            ImageName = MediaHelper.UploadImage(flagImage, Url);
            return ImageName;
        }

        public async Task<OfferBanner> InsertOfferBanner(OfferBanner node)
        {
            try
            {
                return await uow.OfferBannerRepository.Add(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<OfferBanner> GetOfferBannerById(int id)
        {
            try
            {
                return await uow.OfferBannerRepository.Get().Where(s => s.OfferInformationId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        
        public async Task<bool> IsExist(int id)
        {
            try
            {
                return  uow.OfferBannerRepository.IsExist(s => s.OfferInformationId == id);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }
    }
}