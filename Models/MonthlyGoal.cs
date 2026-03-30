using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Models
{
    public class MonthlyGoal
    {
        public int MonthlyGoalId { get; set; }
        public decimal MonthlyGoalAmount { get; set; }
        public bool IsActive { get; set; }
        public int? UpdateBy { get; set; } 
        public  DateTime? UpdateDate { get; set; }
    }
}