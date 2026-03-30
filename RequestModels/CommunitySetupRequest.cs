using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class CommunitySetupRequest
    {
        public long MemberId { get; set; }
        public MonthlyGoal MonthlyGoals { get; set; }
        public List<FeatureCategory> FeatureCategories { get; set; }
    }
}