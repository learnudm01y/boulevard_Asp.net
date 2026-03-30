using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class FilterBodyRequest
    {
        public FilterBodyRequest()
        {
            
            this.PropertyType = new List<string>();
            this.PricePeriod = new List<string>();
            this.Furnishing = new List<string>();

            this.BedRooms = new List<int>();
            this.BathRooms = new List<int>();
            this.Amenityids = new List<int>();
        }
        public int CategoryId { get; set; }

        public int FeatureCategoryId { get; set; }

        //public int SubCategoryId { get; set; }

        public List<string> PropertyType { get; set; }

        public List<String> PricePeriod { get; set; }

        public double Minprice { get; set; }

        public double Maxprice { get; set; }

        public List<int> BedRooms { get; set; }

        public List<int> BathRooms { get; set; }

        public List<string> Furnishing { get; set; }

        public int MaxSize { get; set; }

        public int MinSize { get; set; }


        public List<int> Amenityids { get; set; }

        public string Keyword { get; set; }

        public int MemberId { get; set; }

        public int Count { get; set; }
        
        public int Size { get; set; }

        public string lang { get; set; } = "en";
    }
}