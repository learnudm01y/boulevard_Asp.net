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
    public class OfferDiscountDataAccess
    {
        public IUnitOfWork uow;
        public OfferDiscountDataAccess()
        {
            uow = new UnitOfWork();
        }
        /// <summary>
        /// Get All Offer Discount
        /// </summary>
        /// <returns></returns>
        public async Task<List<OfferDiscount>> GetAll()
        {
            try
            {
                return await uow.OfferDiscountRepository.Get().ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Get All By Offer InformationId
        /// </summary>
        /// <param name="offerInformationId"></param>
        /// <returns></returns>
        public async Task<OfferDiscount> GetAllByOfferInformationId(int offerInformationId)
        {
            try
            {
                return await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == offerInformationId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Insert Offer Discount
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<OfferDiscount> Insert(OfferDiscount node)
        {
            try
            {
                return await uow.OfferDiscountRepository.Add(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Update Offer Discount
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<OfferDiscount> Update(OfferDiscount node)
        {
            try
            {
                return await uow.OfferDiscountRepository.Edit(node);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }
    }
}