using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;

namespace Boulevard.Service.Admin
{
    public class FaqServiceDataAccess
    {
        public IUnitOfWork uow;

        public FaqServiceDataAccess()
        {
            uow = new UnitOfWork();
        }
        /// <summary>
        /// Get All FaqService
        /// </summary>
        /// <returns></returns>
        public async Task<List<FaqService>> GetAll()
        {
            try
            {
                return await uow.FaqServiceRepository.GetAll().Where(a => a.Status != "Deleted" && a.FeatureTypeId == 0 && a.FeatureType == "All").OrderByDescending(t => t.FaqServiceId).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<FaqService>();
            }
        }

        /// <summary>
        /// Get All FaqService FeatureCategory
        /// </summary>
        /// <param name="fCatagoryKey"></param>
        /// <returns></returns>
        public async Task<List<FaqService>> GetAllFaqServiceFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var list = new List<FaqService>();

                //var faqService = new FaqService();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCategory = await uow.FeatureCategoryRepository.Get().Where(a => a.FeatureCategoryKey.ToString() == fCatagoryKey).FirstOrDefaultAsync();

                    var service = await uow.ServiceRepository.Get().Where(a => a.FeatureCategoryId == fCategory.FeatureCategoryId).ToListAsync();
                    if (service != null)
                    {
                        foreach (var serviceNode in service)
                        {
                            var faqServiceList = await uow.FaqServiceRepository.GetAll().ToListAsync();
                            if (faqServiceList != null)
                            {
                                foreach (var faqService in faqServiceList.Where(a => a.FeatureTypeId == serviceNode.ServiceId && a.FeatureType == "Service"))
                                {
                                    list.Add(faqService);
                                }
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return new List<FaqService>();
            }
        }
        /// <summary>
        /// Get All By FaqServiceId
        /// </summary>
        /// <param name="faqServiceId"></param>
        /// <returns></returns>
        public async Task<FaqService> GetAllByFaqServiceId(int faqServiceId)
        {
            try
            {
                return await uow.FaqServiceRepository.GetAll().Where(a => a.FaqServiceId == faqServiceId && a.FeatureType=="Service").FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return new FaqService();
            }
        }
        /// <summary>
        /// Get All By FaqServiceKey
        /// </summary>
        /// <param name="faqServiceKey"></param>
        /// <returns></returns>
        public async Task<FaqService> GetAllByFaqServiceKey(string faqServiceKey)
        {
            try
            {
                return await uow.FaqServiceRepository.Get().Where(a => a.FAQKey.ToString() == faqServiceKey).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return new FaqService();
            }
        }
        /// <summary>
        /// Create FaqService
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FaqService> Create(FaqService model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.FeatureCategoryKey))
                {
                    var fCategory = await uow.FeatureCategoryRepository.Get().Where(a => a.FeatureCategoryKey.ToString() == model.FeatureCategoryKey).FirstOrDefaultAsync();
                    if (fCategory != null)
                    {
                        model.FeatureType = fCategory.FeatureType;
                        model.FeatureTypeId = model.ServiceId;
                    }
                }
                else
                {
                    model.FeatureType = "All";
                    model.FeatureTypeId = 0;
                }
                model.CreateBy = 1;
                model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                model.FAQKey = Guid.NewGuid();
                model.IsActive = true;
                model.Status = "Active";
                model.LastUpdate = Helper.DateTimeHelper.DubaiTime();
                return await uow.FaqServiceRepository.Add(model);
            }
            catch (Exception ex)
            {
                return new FaqService();
            }
        }

        public async Task<FaqService> CreateForAll(FaqService model)
        {
            try
            {
                model.FeatureType = "All";
                model.FeatureTypeId = 0;

                model.CreateBy = 1;
                model.CreateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                model.FAQKey = Guid.NewGuid();
                model.IsActive = false;
                //model.Status = "Active";
                model.LastUpdate = Helper.DateTimeHelper.DubaiTime();
                return await uow.FaqServiceRepository.Add(model);
            }
            catch (Exception ex)
            {
                return new FaqService();
            }
        }
        /// <summary>
        /// Update FaqService
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<FaqService> Update(FaqService model)
        {
            try
            {
                var faqServiceOld = await uow.FaqServiceRepository.GetById(model.FaqServiceId);
                if (faqServiceOld != null)
                {
                    faqServiceOld.UpdateBy = 1;
                    faqServiceOld.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                    faqServiceOld.LastUpdate = Helper.DateTimeHelper.DubaiTime();
                    faqServiceOld.IsActive = false;
                    //faqServiceOld.Status = "Active";
                    faqServiceOld.FeatureType = faqServiceOld.FeatureType;
                    faqServiceOld.FeatureTypeId = faqServiceOld.FeatureTypeId;
                    faqServiceOld.FaqTitle = model.FaqTitle;
                    faqServiceOld.FaqDescription = model.FaqDescription;
                    faqServiceOld.FaqTitleAr = model.FaqTitleAr;
                    faqServiceOld.FaqDescriptionAr = model.FaqDescriptionAr;
                }

                return await uow.FaqServiceRepository.Edit(faqServiceOld);
            }
            catch (Exception ex)
            {
                return new FaqService();
            }
        }

        public async Task<FaqService> UpdateForAll(FaqService model)
        {
            try
            {
                var faqServiceOld = await uow.FaqServiceRepository.GetById(model.FaqServiceId);
                if (faqServiceOld != null)
                {
                    faqServiceOld.UpdateBy = 1;
                    faqServiceOld.UpdateDate = Boulevard.Helper.DateTimeHelper.CreateDate();
                    faqServiceOld.LastUpdate = Helper.DateTimeHelper.DubaiTime();
                    faqServiceOld.IsActive = true;
                    faqServiceOld.Status = "Active";
                    //faqServiceOld.FeatureType = model.FeatureType;
                    //faqServiceOld.FeatureTypeId = model.FeatureTypeId;
                    faqServiceOld.FaqTitle = model.FaqTitle;
                    faqServiceOld.FaqDescription = model.FaqDescription;
                    faqServiceOld.FaqTitleAr = model.FaqTitleAr;
                    faqServiceOld.FaqDescriptionAr = model.FaqDescriptionAr;
                }

                return await uow.FaqServiceRepository.Edit(faqServiceOld);
            }
            catch (Exception ex)
            {
                return new FaqService();
            }
        }

        public async Task<bool> Delete(string key, int updateby)
        {

            try
            {
                var exitResult = await GetAllByFaqServiceKey(key);
                if (exitResult != null)
                {
                    exitResult.Status = "Deleted";
                    exitResult.DeleteBy = updateby;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    await uow.FaqServiceRepository.Edit(exitResult);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}