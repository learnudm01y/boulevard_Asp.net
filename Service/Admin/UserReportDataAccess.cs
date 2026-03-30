using Boulevard.BaseRepository;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Boulevard.Service.Admin
{
    public class UserReportDataAccess
    {
        public IUnitOfWork uow;

        public UserReportDataAccess()
        {
            uow = new UnitOfWork();
        }

        public async Task<List<UserReport>> GetuserReport()
        {
            try
            {
                var result = await uow.UserReportRepository.Get().OrderByDescending(s => s.LastUpdate).ToListAsync();
                if (result.Count > 0)
                {
                    foreach (var userReport in result)
                    {
                        userReport.Member = await uow.MemberRepository.Get().Where(a => a.MemberId == userReport.MemberId).FirstOrDefaultAsync();
                    }
                }
                return result;

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<UserReport> GetuserReportById(int reportId)
        {
            try
            {
                //var userReport = new UserReport();
                var userReport = await uow.UserReportRepository.Get().Where(s => s.UserReportId == reportId).FirstOrDefaultAsync();

                if (userReport != null)
                {
                    var memberData = await uow.MemberRepository.Get().Where(s => s.MemberId == userReport.MemberId).FirstOrDefaultAsync();

                    userReport.Member = memberData != null ? memberData : new Member();

                    var response = uow.UserReportDetailsRepository.IsExist(s => s.UserReportId == reportId);
                    if (response == true)
                    {
                        userReport.IsGiveResponse = true;
                        userReport.Response = await uow.UserReportDetailsRepository.Get().Where(s => s.UserReportId == reportId).Select(s => s.Response).FirstOrDefaultAsync();
                    }
                    return userReport;
                }
                else
                {
                    return new UserReport();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task PostResponse(int userReportId, string responce)
        {
            try
            {
                var userReportData = await GetuserReportById(userReportId);
                if (userReportData.Response == null)
                {
                    var postData = new UserReportDetails();
                    postData.UserReportId = userReportId;
                    postData.Response = responce;
                    //postData.ReportType = userReportData.ReportType;
                    postData.LastUpdate = Helper.DateTimeHelper.DubaiTime();
                    await uow.UserReportDetailsRepository.Add(postData);
                }
                else
                {
                    var postData = await uow.UserReportDetailsRepository.Get().Where(a => a.UserReportId == userReportData.UserReportId).FirstOrDefaultAsync();
                    //postData.UserReportId = userReportId;
                    postData.Response = responce;
                    //postData.ReportType = userReportData.ReportType;
                    postData.LastUpdate = Helper.DateTimeHelper.DubaiTime();
                    await uow.UserReportDetailsRepository.Edit(postData);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}