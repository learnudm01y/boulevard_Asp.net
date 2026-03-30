using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class BigServiceDetailResponse
    {
        public BigServiceDetailResponse()
        {
            this.ServiceImageURLs = new List<string>();
            this.ServiceType = new ServiceTypeDetails();
        }
        public int ServiceId { get; set; }

       
        public string Name { get; set; }

        public virtual City City { get; set; }


        public virtual Country Country { get; set; }

        public string Address { get; set; }

        public string Aboutus { get; set; }
     
        public List<string> ServiceImageURLs { get; set; }

        public ServiceTypeDetails ServiceType { get; set; }

        public FeatureCategory FeatureCategory { get; set; }
    }
}