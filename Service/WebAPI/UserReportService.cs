using Boulevard.BaseRepository;
using Boulevard.Models;
using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class UserReportService
    {
        private readonly IUnitOfWork uow;

        public UserReportService()
        {
            uow = new UnitOfWork();

        }
        public async Task<bool> SaveUserReport(UserReport rpt)
        {
            try
            {
                rpt.LastUpdate = DateTime.UtcNow;
                rpt.IsActive = true;
                var ss = await uow.UserReportRepository.Add(rpt);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<List<UserReportResponse>> GetUserReport(int memberId/*, int page = 0, int page_size = 10*/)
        {
            try
            {
                var resultList = new List<UserReportResponse>();
                var reports = await uow.UserReportRepository.Get().Where(s => s.IsActive == true && s.MemberId == memberId).OrderByDescending(s => s.UserReportId)./*.Skip(page).Take(page_size).*/ToListAsync();

                if (reports.Count() > 0 && reports != null)
                {
                    foreach (var report in reports)
                    {
                        var result = new UserReportResponse();
                        result.MemberId = memberId;
                        result.UserReportId = report.UserReportId;
                        result.Title = report.Title;
                        result.Comments = report.Comments;
                        result.ReportDate = report.LastUpdate;
                        result.Response = await uow.UserReportDetailsRepository.Get().Where(s => s.UserReportId == report.UserReportId).FirstOrDefaultAsync();
                        resultList.Add(result);
                    }
                }
                return resultList;
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}