using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class CommunitySetupResponse
    {
        public List<MonthlyGoal> MonthlyGoals { get; set; } 
        public List<FeatureCategory> FeatureCategories { get; set; }
    }
}