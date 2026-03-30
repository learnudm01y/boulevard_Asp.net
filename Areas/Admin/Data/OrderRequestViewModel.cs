using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Areas.Admin.Data
{
    public class OrderRequestViewModel
    {
        public DateTime Date { get; set; }
        public string FormertDate { get; set; }
        public string Time { get; set; }
        public int Count { get; set; }
    }
}