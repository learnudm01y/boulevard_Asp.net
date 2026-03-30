using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class ServiceSearchingRequest
    {
        public int FeatureCategoryId { get; set; }
        public string SearchKeyword { get; set; } //servicename/ servicetype

        public int FromAirportId { get; set; }
        public int ToAirportId { get; set; }
        public int CityId { get; set; } = 0;
        public int RoomQuantity { get; set; } = 0;
        public int CountryId { get; set; } = 0;
        public int AdultQuantity { get; set; } = 0;
        public int ChildrenQuantity { get; set; } = 0;
        public string LocationKeyword { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public int Size { get; set; }
        public int Count { get; set; }

        public int CategoryId { get; set; }

        public List<int> AmenityId { get; set; }

        public string lang { get; set; } = "en";
    }
}