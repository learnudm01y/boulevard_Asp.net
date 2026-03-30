using Boulevard.Areas.Admin.Data;
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
    public class MemberDataAccess
    {
        public IUnitOfWork uow;

        public MemberDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from Member
        /// </summary>
        /// <returns></returns>
        public async Task<List<Member>> GetAll()
        {
            try
            {
                var members = await uow.MemberRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.MemberId).ToListAsync();
                if (members != null)
                {
                    foreach (var memberNode in members)
                    {
                        var memberShipId = await uow.MemberSubscriptionRepository.Get().Where(t => t.MemberId == memberNode.MemberId).Select(i => i.MemberShipId).FirstOrDefaultAsync();
                        if(memberShipId > 0)
                        {
                            var memberShipName = await uow.MemberShipRepository.Get().Where(a => a.MemberShipId == memberShipId).Select(a => a.Title).FirstOrDefaultAsync();
                            if(memberShipName != null)
                            {
                                memberNode.MemberShipName = memberShipName;
                            }
                        }
                    }
                }
                return members;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
        
        public async Task<MemberViewModel> GetAllMember(MemberViewModel model)
        {
            try
            {

                //IQueryable<Member> data;
                var data = uow.MemberRepository.Get().Where(e => e.Status == "Active").OrderByDescending(s => s.MemberId);
                if (model.QuickSearchQuery != null)
                {
                    data = (IOrderedQueryable<Models.Member>)data.Where(t => t.Name.ToLower().Contains(model.QuickSearchQuery.Trim().ToLower())
                    || (t.Email != null && t.Email.ToLower().Contains(model.QuickSearchQuery.Trim().ToLower()))
                    || (t.PhoneNumber != null && t.PhoneNumber.ToLower().Contains(model.QuickSearchQuery.Trim().ToLower()))
                    || t.PhoneCode.ToLower().Contains(model.QuickSearchQuery.Trim().ToLower()));
                }
                var result = data.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize);
                model.TotalRecord = data.Count();
                model.Members = result;
                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Member By Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public async Task<Member> GetMemberById(long memberId)
        {
            try
            {
                return await uow.MemberRepository.GetAll(a => a.MemberId == memberId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get Member By Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<Member> GetMemberByKey(string key)
        {
            try
            {
                return await uow.MemberRepository.GetAll(a => a.MemberKey.ToString() == key).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create Member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Member> Create(Member model)
        {
            try
            {
                model.MemberKey = Guid.NewGuid();
                return await uow.MemberRepository.Add(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Update Member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Member> Update(Member model)
        {
            try
            {
                return await uow.MemberRepository.Edit(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public string UploadImage(HttpPostedFileBase PictureOne)
        {
            string ImageName = string.Empty;
            string Url = "/Content/Upload/Member";
            ImageName = MediaHelper.UploadImage(PictureOne, Url);
            return ImageName;
        }
    }
}