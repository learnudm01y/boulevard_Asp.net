using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Areas.Admin.Data
{
    public class MemberViewModel
    {
        //public MemberViewModel()
        //{
        //    this.Members = new List<Member>();
        //}

        public string QuickSearchQuery { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecord { get; set; }
        public IQueryable<Member> Members { get; set; }
    }
}