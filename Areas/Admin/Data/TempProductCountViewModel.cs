using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Areas.Admin.Data
{
    public class TempProductCountViewModel
    {
        public int DoneCount { get; set; }
        public int TotalCount { get; set; }
        public int TotalNew { get; set; }
        public int TotalDuplicate { get; set; }
        public int ProductId { get; set; }
        public string fCatagoryKey { get; set; }

        public bool IsPackage { get; set; }
    }
}