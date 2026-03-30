using Boulevard.BaseRepository;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class MembershipService
    {
        public IUnitOfWork uow;
        public MembershipService()
        {
            uow = new UnitOfWork();
        }
        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
        public async Task<MemberShip> getMemberShipDetails(string lang="en")
        {
            try
            {
                var ss = await uow.MemberShipRepository.Get().Where(s => s.Status == "Active" ).FirstOrDefaultAsync();
                if (ss != null)
                {
                    if (!string.IsNullOrEmpty(ss.MembershipBanner))
                    {
                        ss.MembershipBanner = link + ss.MembershipBanner;
                    }

                    ss.Title = lang == "en" ? ss.Title : ss.TitleAr;
                    ss.Description = lang == "en" ? ss.Description : ss.DescriptionAr;
                    ss.Benefits = lang == "en" ? ss.Benefits : ss.BenefitsAr;
                    ss.DiscountInfo = await uow.MemberShipDiscountCategoryRepository.Get().Where(s => s.MemberShipId == ss.MemberShipId).ToListAsync();
                    return ss;
                }
                else
                {
                    return new MemberShip();
                }
            }
            catch (Exception)
            {

                return new MemberShip();
            }
        }

        public async Task<bool> CreateMembershipSubscription(int memberId, int membershipId)
        {
            try
            {
                var datetime = Helper.DateTimeHelper.DubaiTime();
                var subscription = await uow.MemberSubscriptionRepository.Get().AnyAsync(s => s.MemberShipId == membershipId && s.MemberId == memberId && s.StartDate <= datetime && s.EndDate >= datetime && s.Status=="Active");
                if (subscription ==true)
                {

                    return false;
                }
                else
                {
                    var membershipt = await uow.MemberShipRepository.Get().Where(s => s.MemberShipId == membershipId).FirstOrDefaultAsync();
                    if (membershipt != null)
                    {
                        var createsubscription = new MemberSubscription();
                        createsubscription.MemberShipId = membershipId;
                        createsubscription.MemberId = memberId;
                        createsubscription.StartDate = Helper.DateTimeHelper.DubaiTime();
                        createsubscription.EndDate = Helper.DateTimeHelper.DubaiTime().AddMonths(membershipt.MembershipValidityInMonth);
                        createsubscription.Status = "Active";
                        createsubscription.CreatedDate = Helper.DateTimeHelper.DubaiTime();
                        createsubscription.UpdatedAt = Helper.DateTimeHelper.DubaiTime();
                        await uow.MemberSubscriptionRepository.Add(createsubscription);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

    }
}