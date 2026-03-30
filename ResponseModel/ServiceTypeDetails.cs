using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.ResponseModel
{
    public class ServiceTypeDetails
    {
        public ServiceTypeDetails()
            {
            this.ServiceTypeImages = new List<string>();
            this.ProjectPlan = new List<string>();
            this.ProjectVideos = new List<string>();
            this.CloserFacilities = new List<ServiceTypeAmenity>();
            this.ProjectMaterials = new List<ServiceTypeAmenity>();
            this.ProjectUtilities = new List<ServiceTypeAmenity>();
            this.Amenities = new List<ServiceTypeAmenity>();
            this.Contracts = new List<string>();
            this.NewProjectPlan = new List<ProjectPlanByTitleResponse>();
        }
        public int ServiceTypeId { get; set; }
       
        public string ServiceTypeName { get; set; }
        public int BedQuantity { get; set; }

        public int BathroomQuantity { get; set; }

        public string Description { get; set; }

     

        public string Size { get; set; }

 
        public string MainImage { get; set; }

        [StringLength(100)]
        public string PaymentType { get; set; }

        public double Price { get; set; }
        [StringLength(100)]
        public string Latitute { get; set; }

        [StringLength(100)]
        public string Logitute { get; set; }
   
  
        public City City { get; set; }

        public Country Country { get; set; }

        public string Address { get; set; }

        public bool IsFavourite { get; set; }

        public List<string> ServiceTypeImages { get; set; }

        public List<ServiceTypeAmenity> CloserFacilities { get; set; }

        public List<ServiceTypeAmenity> ProjectMaterials { get; set; }

        public List<ServiceTypeAmenity> ProjectUtilities { get; set; }
        public List<ServiceTypeAmenity>  Amenities { get; set; }

        public List<string> ProjectPlan { get; set; }


        public List<ProjectPlanByTitleResponse> NewProjectPlan { get; set; }

        public PropertyInformation PropertyInformation { get; set; }

        public List<string> ProjectVideos { get; set; }
        public List<string> Contracts { get; set; }


    }
}