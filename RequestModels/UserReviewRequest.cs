using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class UserReviewRequest
    {
        public int UserId { get; set; }

       

        public string FeatureType { get; set; }

        public int FeatureTypeId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }

        public string Details { get; set; }

        public List<string> ReviewImages { get; set; }
    }
}