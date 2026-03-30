using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class UserReportResponse
    {
        public UserReportResponse()
        {
            this.Response = new UserReportDetails();
        }
        public int UserReportId { get; set; }
        public int MemberId { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public DateTime ReportDate { get; set; }
        public UserReportDetails Response { get; set; }
    }
}