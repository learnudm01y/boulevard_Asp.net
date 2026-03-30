using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class SmallServiceDetailsResponse
    {
        public SmallServiceDetailsResponse()
        {
            Images = new List<ServiceImage>();
            DiscountInformation = new OfferDiscount();
            ServiceType = new ServiceType();

            ServiceTypeList = new List<ServiceType>();
           
        }
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }
        public string ServiceAddress { get; set; }
        public decimal PackagePrice { get; set; } = 0;
        public decimal DistanceKM { get; set; } = 0;
        public string Description { get; set; }

        public City City { get; set; }
        public virtual Country Country { get; set; }

        public double Price { get; set; } 

        public List<string> Amenities { get; set; }

        public double Ratings { get; set; }

        public int ReviewCount { get; set; }

        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }

        public List<ServiceImage> Images { get; set; }


        public OfferDiscount DiscountInformation { get; set; }

        public ServiceType ServiceType { get; set; }

        public List<ServiceType> ServiceTypeList { get; set; }

       


    }


    public class ParentSmallServiceDetailsResponse
    {
        public ParentSmallServiceDetailsResponse() {
            ChildServices = new List<SmallServiceDetailsResponse>();
        }
        public int ServiceId { get; set; }

        public string ServiceName { get; set; }
        public string ServiceAddress { get; set; }
        public decimal PackagePrice { get; set; } = 0;
        public decimal DistanceKM { get; set; } = 0;
        public string Description { get; set; }

        public City City { get; set; }
        public virtual Country Country { get; set; }

        public double Price { get; set; }

        public List<string> Amenities { get; set; }

        public double Ratings { get; set; }

        public int ReviewCount { get; set; }

        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }

        public List<ServiceImage> Images { get; set; }
        public List<SmallServiceDetailsResponse> ChildServices { get; set; }
    }
}