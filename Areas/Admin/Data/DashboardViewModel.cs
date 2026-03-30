using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Areas.Admin.Data
{
    public class DashboardViewModel
    {
        public int TotalProductOrder {  get; set; }
        public int TotalServiceOrder {  get; set; }
        public int TotalCustomer {  get; set; }
        public double TotalProductSales { get; set; }
        public double TotalServiceSales { get; set; }
        public virtual List<Member> Member { get; set; }
        public virtual List<FeatureCategory> Categories { get; set; } = new List<FeatureCategory>();

        public int CategoryWiseOrderCount { get; set; }


        public int TotalProductOrderMonth { get; set; }
        public int TotalProductOrderWeek { get; set; }
        public int TotalServiceOrderMonth { get; set; }
        public int TotalServiceOrderWeek { get; set; }

        public double TotalProductSaleMonth { get; set; }
        public double TotalProductSaleWeek { get; set; }
        public double TotalServiceSaleMonth { get; set; }
        public double TotalServiceSaleWeek { get; set; }
    }
}