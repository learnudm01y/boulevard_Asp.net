using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class CommunitySetupAccess
    {
        public IUnitOfWork uow;
        public CommunitySetupAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<CommunitySetupResponse> GetAll()
        {
            try
            {
                var db = new BoulevardDbContext();
                var communitySetupResponse = new CommunitySetupResponse();
                communitySetupResponse.MonthlyGoals = db.MonthlyGoals.ToList();
                communitySetupResponse.FeatureCategories = db.featureCategories.Where(t => t.IsActive).ToList();
                return communitySetupResponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }


        public async Task<bool> CreateCommunitySetup(CommunitySetupRequest communitySetupRequest)
        {
            try
            {
                var db = new BoulevardDbContext();
                if (communitySetupRequest != null)
                {
                    var member = db.Members.FirstOrDefault(t => t.MemberId == communitySetupRequest.MemberId);
                    member.MonthlyGoalId = communitySetupRequest.MonthlyGoals.MonthlyGoalId;
                    member.MonthlyGoalAmount = communitySetupRequest.MonthlyGoals.MonthlyGoalAmount;

                    await uow.MemberRepository.Edit(member);


                    foreach (var item in communitySetupRequest.FeatureCategories)
                    {
                        var golbalMemberCategory = new GolbalMemberCategory();
                        golbalMemberCategory.MemberId = communitySetupRequest.MemberId;
                        golbalMemberCategory.FeatureCategoryId = item.FeatureCategoryId;
                        golbalMemberCategory.IsActive = true;
                        golbalMemberCategory.CreatedDate = Helper.DateTimeHelper.DubaiTime();
                        db.GolbalMemberCategories.Add(golbalMemberCategory);
                        db.SaveChanges();
                    }


                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return false;
            }
        }
    }
}