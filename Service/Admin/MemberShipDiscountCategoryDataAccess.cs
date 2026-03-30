using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Microsoft.Ajax.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static Dapper.SqlMapper;
using Boulevard.Contexts;

namespace Boulevard.Service.Admin
{
    public class MemberShipDiscountCategoryDataAccess
    {
        public IUnitOfWork uow;

        public MemberShipDiscountCategoryDataAccess()
        {
            uow = new UnitOfWork();
        }
        /// <summary>
        /// Get All MemberShipDiscountCategory
        /// </summary>
        /// <returns></returns>
        public async Task<List<MemberShipDiscountCategory>> GetAll()
        {
            try
            {
                var data = await uow.MemberShipDiscountCategoryRepository.GetAll().OrderByDescending(t => t.MemberShipDiscountCategoryId).ToListAsync();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var memberShip = await uow.MemberShipRepository.Get().Where(a => a.MemberShipId == item.MemberShipId).FirstOrDefaultAsync();
                        item.MemberShip = memberShip != null ? memberShip : new MemberShip();
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Get MemberShip Discount Category By Id
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public async Task<MemberShipDiscountCategory> GetMemberShipDiscountCategoryById(int modelId)
        {
            try
            {
                return await uow.MemberShipDiscountCategoryRepository.GetAll(a => a.MemberShipDiscountCategoryId == modelId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        public async Task<MemberShipDiscountCategory> GetMemberShipDiscountCategoryByMemberShipId(int memberShipId, int fCategoryId)
        {
            try
            {
                var ss =  await uow.MemberShipDiscountCategoryRepository.Get().Where(a => a.MemberShipId == memberShipId && a.FeatureCategoryId == fCategoryId).FirstOrDefaultAsync();
                return ss;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Create MemberShipDiscountCategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberShipDiscountCategory> Create(MemberShipDiscountCategory model)
        {
            try
            {
                model.UpdateAt = DateTimeHelper.DubaiTime();
                return await uow.MemberShipDiscountCategoryRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Update MemberShipDiscountCategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberShipDiscountCategory> Update(MemberShipDiscountCategory model)
        {
            try
            {
                var context = new BoulevardDbContext();
                var request = new MemberShipDiscountCategory();
                request.MemberShipDiscountCategoryId = model.MemberShipDiscountCategoryId;
                request.MemberShipId = model.MemberShipId;
                request.FeatureCategoryId = model.FeatureCategoryId;
                request.MemberShipDiscountType = model.MemberShipDiscountType;
                request.MemberShipDiscountTypeAr = model.MemberShipDiscountTypeAr;
                request.MemberShipDiscountAmount = model.MemberShipDiscountAmount;

                request.UpdateAt = DateTimeHelper.DubaiTime();

                context.Entry(request).State = EntityState.Modified;
                context.SaveChanges();

                return request;
               
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}