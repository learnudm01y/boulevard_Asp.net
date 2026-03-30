using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Areas.Admin.Data
{
    public class UserActivityTimeLine
    {
        public UserActivityTimeLine()
        {
            this.UserActivities = new List<UserActivity>();
        }
        public DateTime Date { get; set; }
        public List<UserActivity> UserActivities { get; set; }
    }
}