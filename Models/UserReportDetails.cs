using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class UserReportDetails
    {
        public int UserReportDetailsId { get; set; }
        public int UserReportId { get; set; }
        public string Response { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}