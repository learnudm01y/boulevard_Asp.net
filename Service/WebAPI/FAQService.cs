using Boulevard.BaseRepository;
using Boulevard.Models;
using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class FAQService
    {
        public IUnitOfWork uow;
        public FAQService()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        ///  Get All
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        //public async Task<List<FaqResponse>> GetAll()
        //{
        //    try
        //    {
        //        var faqList = new List<FaqService>();
        //        faqList = await uow.FaqServiceRepository.Get().Where(e => e.IsActive == true).ToListAsync();

        //        if (faqList != null && faqList.Count() > 0)
        //        {
        //            foreach (var faq in faqList)
        //            {
        //                var faqresult = new FaqResponse();
        //                faqresult.FaqServiceId = faq.FaqServiceId;
        //                faqresult.FaqTitle = faq.FaqTitle;
        //                faqresult.FaqDescription = faq.FaqDescription;
        //                faqList.Add(faqresult);

        //            }
        //        }
        //        return faqList;
        //    }
        //    catch (Exception ex)
        //    {

        //        return null;
        //    }
        //}

        public async Task<List<FaqService>> GetAll(int featurecategoryId=0)
        {
            try
            {

                var faqList = await uow.FaqServiceRepository.Get().Where(e => e.Status == "Active" && e.FeatureTypeId== featurecategoryId).ToListAsync();
                return faqList;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}