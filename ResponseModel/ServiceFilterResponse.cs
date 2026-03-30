using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class ServiceFilterResponse
    {
        public ServiceFilterResponse() 
        {
            this.Categories = new List<Category>();
            this.PropertyType = new List<string>();
            this.PricePeriod = new List<string>();
                 this.Furnishing = new List<string>();
           
            this.BedRooms = new List<int>();
            this.BathRooms = new List<int>();
            this.Amenities = new List<ServiceTypeAmenity>();



        }
        public List<Category> Categories { get; set; }
        public List<string> PropertyType { get; set; }

        public List<String> PricePeriod { get; set; }

        public double Minprice { get; set; }

        public double Maxprice { get; set; }

        public List<int> BedRooms { get; set; }

        public List<int> BathRooms { get; set; }

        public List<string> Furnishing {get;set;}

        public int MaxSize { get; set; }

        public int MinSize { get; set; }

       


        public List<ServiceTypeAmenity> Amenities { get; set; }




    }
}