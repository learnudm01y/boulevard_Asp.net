using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service.Admin;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Razor.Tokenizer;

namespace Boulevard.Service.WebAPI
{
    public class CustomerEnqueryServiceAccess
    {
        public IUnitOfWork uow;
        public CustomerEnqueryServiceAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<bool> CreateEnquiry(CustomerEnquery enquery)
        {

            try
            {
               
                enquery.UpdatedAt = DateTimeHelper.DubaiTime();
                enquery.Status = "Active";

             await uow.CustomerEnqueryRepository.Add(enquery);

                if (enquery.CustomerEnqueryId>0)
                {
                    var emailEnquiry = await EnquiryGetById(enquery.CustomerEnqueryId);
                    if (emailEnquiry != null)
                    {
                      await  new EmailService().SendEnquiryResponse(enquery);
                    }
                    await new AdminNotificationDataAccess().SaveNotification(emailEnquiry.UserId, "A new Enquiry has Been Placed By Customer Please Check Enquiry Section For More Details", "New Enquiry ", "Member", "Enquiry", null);
                }
                return true;

            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<List<CustomerEnquery>> GetEnqueries(int userId)
        {

            try
            {

               var result = await uow.CustomerEnqueryRepository.Get().Where(s=>s.UserId==userId && s.Status=="Active").ToListAsync();

                if (result != null && result.Count() > 0)
                {

                    return result;
                }
                else
                {
                    return new List<CustomerEnquery>();
                }

            }
            catch (Exception ex)
            {

                return new List<CustomerEnquery>();
            }
        }

        public async Task<CustomerEnquery> EnquiryGetById(int customerEnqueryId)
        {
            try
            {
                var item = await uow.CustomerEnqueryRepository
                    .GetAll()
                    .Where(a => a.CustomerEnqueryId == customerEnqueryId && a.Status == "Active")
                    .FirstOrDefaultAsync();

                if (item == null)
                    return null;

                item.FeatureCategoryName = await uow.FeatureCategoryRepository
                    .Get()
                    .Where(a => a.FeatureCategoryId == item.FeatureCategoryId)
                    .Select(a => a.Name)
                    .FirstOrDefaultAsync();
                if (item.UserId > 0)
                {
                    item.UserName = await uow.MemberRepository
                   .Get()
                   .Where(a => a.MemberId == item.UserId)
                   .Select(a => a.Name)
                   .FirstOrDefaultAsync();
                }

               

                //item.UserEmail = await uow.MemberRepository
                //    .Get()
                //    .Where(a => a.MemberId == item.UserId)
                //    .Select(a => a.Email)
                //    .FirstOrDefaultAsync();

                //item.UserPhoneNo = await uow.MemberRepository
                //    .Get()
                //    .Where(a => a.MemberId == item.UserId)
                //    .Select(a => a.PhoneNumber)
                //    .FirstOrDefaultAsync();

                return item;
            }
            catch (Exception ex)
            {
               
                return null;
            }
        }
    }
}