using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Boulevard.Models
{
    public class TempService
    {
        [Key]
        public int Id { get; set; }
        public string SlNo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ServiceHour { get; set; }
        public string Category { get; set; }


        public string SubCategory { get; set; }

        public string CategoryImage { get; set; }

        public string SubCategoryImage { get; set; }

        public string CategoryIcon { get; set; }
        public string Address { get; set; }
        public string AboutUs { get; set; }
        public string Languages { get; set; }
        public string ScopeofService { get; set; }
        public string ServiceMinutes { get; set; }
        public string Price { get; set; }
        public string Images { get; set; }
        public string File { get; set; }

        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }

        public string City { get; set; }

        public string Country { get; set; }


        public string AboutUsAr { get; set; }

        public string ScopeOfServiceAr { get; set; }
        public string DescriptionAr { get; set; }
        public string NameAr { get; set; }
        public int ExcelCount { get; set; }
        public int FeatureCategoryId { get; set; }
        public string Status { get; set; }

        public string FaqTitle { get; set; }

        public string FaqDescription { get; set; }

        public string FaqTitleAr { get; set; }

        public string FaqDescriptionAr { get; set; }
        public string CategoryArabic { get; set; }
        public string SubCategoryArabic { get; set; }

        [StringLength(100)]
        public string Latitute { get; set; }

        [StringLength(100)]
        public string Longitute { get; set; }

        [StringLength(250)]
        public string ServiceTypeName { get; set; }
        [StringLength(250)]
        public string ServiceTypeNameAr { get; set; }

        [StringLength(250)]
        public string SubServiceTypeName { get; set; }
        [StringLength(250)]
        public string SubServiceTypeNameAr { get; set; }

        public string PersoneQuantity { get; set; }



        public string AdultQuantity { get; set; }

        public string ChildrenQuantity { get; set; }

        public string TypeDescription { get; set; }
        public string TypeDescriptionAr { get; set; }

  



        public string TypeServiceHour { get; set; }

        public string TypeServiceMin { get; set; }

        [StringLength(100)]
        public string Size { get; set; }
        [StringLength(100)]
        public string SizeAr { get; set; }

        [StringLength(100)]
        public string TypeImage { get; set; }

        [StringLength(100)]
        public string PaymentType { get; set; }

        public string ServiceTypeCategory { get; set; }

        public string ServiceTypeSubCategory{ get; set; }

        public string PropertyType {get;set;}
        public string PropertyTypeArabic { get; set; }


        public string PropertyRefNo { get; set; }

        public string PropertyPurpose { get; set; }

        public string Furnishing { get; set; }

        public string FurnishingArabic { get; set; }

        public string PropertyWhatsAppNo { get; set; }

        public string PropertyEmail { get; set; }

        public string ExteriorDetails { get; set; }

        public string ExteriorDetailsArabic { get; set; }
        public string ExteriorImage{ get; set; }


        public string InteriorDetails { get; set; }

        public string InteriorDetailsArabic { get; set; }
        public string InteriorImage { get; set; }

        public string TypePrice { get; set; }

        public string ServiceTypePrice{ get; set; }

        public string ServiceTypePriceAr { get; set; }
        [StringLength(100)]
        public string TypeLatitute { get; set; }

        [StringLength(100)]
        public string TypeLogitute { get; set; }

        public string AirportName { get; set; }

        public string AirportNameArabic { get; set; }

        public string AirportCode { get; set; }

        public string AmenitiesName { get; set; }

        public string AmenitiesNameArabic { get; set; }

        public string AmenitiesImage { get; set; }

        public string AmenitiesFile { get; set; }

        public string landmarkName { get; set; }

        public string landmarkNameArabic { get; set; }

        public string landmarkNameDistance { get; set; }

        public string landmarkLatitute { get; set; }

        public string landmarkLongitute { get; set; }

        public bool IsPackage { get; set; }

        public string LandmarkAddress { get; set; }


        public string CloserPropertyName { get; set; }

        public string CloserPropertyNameArabic { get; set; }

        public string CloserPropertyLogo { get; set; }

        public string CloserPropertyFile { get; set; }

        public string MaterialsName { get; set; }

        public string MaterialsNameArabic { get; set; }

        public string MaterialsLogo { get; set; }

        public string MaterialsFile { get; set; }


        public string UtilityName { get; set; }

        public string UtilityNameArabic { get; set; }

        public string UtilityLogo { get; set; }

        public string UtilityFile { get; set; }

        public string Video { get; set; }

        public string ServiceTypeBigDescription { get; set; }

        public string ServiceTypeBigDescriptionArabic { get; set; }  

        public string SubCategoryIcon { get; set; }

    }
}