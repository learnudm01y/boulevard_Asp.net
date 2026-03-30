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
    public class MemberShipDataAccess
    {
        public IUnitOfWork uow;

        public MemberShipDataAccess()
        {
            uow = new UnitOfWork();
        }
        /// <summary>
        /// Get All MemberShip
        /// </summary>
        /// <returns></returns>
        public async Task<List<MemberShip>> GetAll()
        {
            try
            {
                return await uow.MemberShipRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.MemberShipId).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Get MemberShip By Id
        /// </summary>
        /// <param name="memberShipId"></param>
        /// <returns></returns>
        public async Task<MemberShip> GetMemberShipById(int memberShipId)
        {
            try
            {
                return await uow.MemberShipRepository.GetAll(a => a.MemberShipId == memberShipId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Get MemberShip By Key
        /// </summary>
        /// <param name="memberShipKey"></param>
        /// <returns></returns>
        public async Task<MemberShip> GetMemberShipByKey(string memberShipKey)
        {
            try
            {
                return await uow.MemberShipRepository.GetAll(a => a.MemberShipKey.ToString() == memberShipKey).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Create MemberShip
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MemberShip> Create(MemberShip model)
        {
            try
            {
                model.MemberShipKey = Guid.NewGuid();
                model.CreateDate = DateTimeHelper.DubaiTime();
                model.CreateBy = 1;
                model.Status = "Active";
                return await uow.MemberShipRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Update MemberShip
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task Update(MemberShip model)
        {
            try
            {
                model.UpdateDate = DateTimeHelper.DubaiTime();
                model.UpdateBy = 1;
                var db = new BoulevardDbContext();
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                //return await uow.MemberShipRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        /// <summary>
        /// Delete MemberShip
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string key)
        {
            try
            {
                var exitResult = await GetMemberShipByKey(key);
                if (exitResult != null)
                {
                    exitResult.DeleteBy = 1;
                    exitResult.DeleteDate = DateTimeHelper.DubaiTime();
                    exitResult.Status = "Deleted";
                    await uow.MemberShipRepository.Edit(exitResult);
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
        /// <param name="MembershipBanner"></param>
        /// <returns></returns>
        public string UploadImage(HttpPostedFileBase MembershipBanner)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/MemberShip";
            ImageName = MediaHelper.UploadImage(MembershipBanner, Url);
            return ImageName;
        }
        
        public string UploadMembershipBannerAr(HttpPostedFileBase MembershipBanner)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/MemberShip";
            ImageName = MediaHelper.UploadImage(MembershipBanner, Url);
            return ImageName;
        }
    }
}