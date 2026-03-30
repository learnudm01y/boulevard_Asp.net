using Boulevard.Areas.Admin.Data;
using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Boulevard.Helper;
using Boulevard.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Diagnostics.Metrics;
using System.Data.Entity;
using System.Web.UI.WebControls;

namespace Boulevard.Service.Admin
{
    public class TempServiceDataAccess
    {
        public IUnitOfWork uow;

        public TempServiceDataAccess()
        {
            uow = new UnitOfWork();
        }

        public TempProductCountViewModel GetTempServiceCount()
        {
            var db = new BoulevardDbContext();
            TempProductCountViewModel tempProductCount = new TempProductCountViewModel();

            int DoneCount = db.TempServices.Count();
            int TotalDuplicate = (from temp in db.TempServices
                                  where db.Services.Any(f => f.Name == temp.Name)
                                  select temp).Count();

            int TotalNew = (from temp in db.TempServices
                            where !db.Services.Any(f => f.Name == temp.Name)
                            select temp).Count();

            tempProductCount.DoneCount = DoneCount;
            if (db.TempServices.Count() > 0)
            {
                var db1 = new BoulevardDbContext();
                tempProductCount.TotalCount = db1.TempServices.Count() > 0 ? db1.TempServices.FirstOrDefault().ExcelCount : 0;
            }
            //tempProductCount.TotalCount = db.TempProducts.Count() > 0 ? db.TempProducts.FirstOrDefault().ExcelCount : 0;
            tempProductCount.TotalNew = TotalNew;
            tempProductCount.TotalDuplicate = TotalDuplicate;

            return tempProductCount;
        }

        public void DeleteTempServices()
        {
            var db = new BoulevardDbContext();
            db.TempServices.RemoveRange(db.TempServices);
            db.SaveChanges();
        }

        public bool AddService(int feacherCategoryId)
        {
            try
            {
                var db = new BoulevardDbContext();
                var tempService = db.TempServices.Where(t => t.FeatureCategoryId == feacherCategoryId).ToList();
               

                foreach (var item in tempService)
                {
                  
                    


                    //Category
                    try
                    {

                        if (feacherCategoryId == 9 || feacherCategoryId == 11)
                        {
                            var category = db.Categories.FirstOrDefault(c => c.CategoryName.Trim().ToLower() == item.Category.Trim().ToLower() && c.FeatureCategoryId == item.FeatureCategoryId && c.Status == "Active");
                            if (category == null)
                            {
                                category = new Category();
                                category.CategoryKey = Guid.NewGuid();
                                category.CreateBy = 1;
                                category.CreateDate = DateTimeHelper.DubaiTime();
                                category.FeatureCategoryId = feacherCategoryId;
                                category.CategoryName = item.Category.Trim();
                                category.ParentId = 0;
                                if (!string.IsNullOrEmpty(item.CategoryArabic))
                                {
                                    category.CategoryNameAr = item.CategoryArabic.Trim();
                                }
                                category.Status = "Active";
                                if (!string.IsNullOrEmpty(item.CategoryImage))
                                {
                                    category.Image = "/Content/Upload/Category/" + item.CategoryImage;
                                }

                                if (!string.IsNullOrEmpty(item.CategoryIcon))
                                {
                                    category.Icon = "/Content/Upload/Category/" + item.CategoryIcon;
                                }
                                db.Categories.Add(category);
                                db.SaveChanges();
                            }


                      

                         

                            // Service
                            var service = db.Services.FirstOrDefault(s => s.Name.Trim().ToLower() == item.Name.Trim().ToLower() && s.FeatureCategoryId == item.FeatureCategoryId);
                            if (service == null)
                            {
                                service = new Models.Service();
                                service.ServiceKey = Guid.NewGuid();
                                service.CreateBy = 1;
                                service.CreateDate = DateTimeHelper.DubaiTime();
                                service.FeatureCategoryId = feacherCategoryId;
                                service.Name = item.Name;
                                service.NameAr = item.NameAr;

                                service.ParentId = 0;
                               


                               
                                service.Status = "Active";
                                db.Services.Add(service);
                                db.SaveChanges();
                            }

                            var servicechild = db.Services.FirstOrDefault(s => s.Name.Trim().ToLower() == item.ServiceTypeName.Trim().ToLower() && s.FeatureCategoryId == item.FeatureCategoryId && s.ParentId==service.ServiceId);
                            if (servicechild == null)
                            {
                                servicechild = new Models.Service();
                                servicechild.ServiceKey = Guid.NewGuid();
                                servicechild.CreateBy = 1;
                                servicechild.CreateDate = DateTimeHelper.DubaiTime();
                                servicechild.FeatureCategoryId = feacherCategoryId;
                                servicechild.Name = item.ServiceTypeName;
                                servicechild.NameAr = item.ServiceTypeNameAr;

                                servicechild.ParentId =service.ServiceId;

                                servicechild.Status = "Active";


                                service.Status = "Active";
                                db.Services.Add(servicechild);
                                db.SaveChanges();
                            }



                            // Add Service Type
                            var serviceType = new ServiceType
                            {
                                ServiceTypeName = item.SubServiceTypeName,
                                ServiceTypeNameAr = item.SubServiceTypeName,
                                ServiceTypeKey = Guid.NewGuid(),
                                CreateBy = 1,
                                CreateDate = DateTimeHelper.DubaiTime(),
                                PersoneQuantity = !string.IsNullOrEmpty(item.PersoneQuantity) ? Convert.ToInt32(item.PersoneQuantity) : 0,
                                Description = item.TypeDescription,
                                DescriptionAr = item.TypeDescriptionAr,

                                BigDescription = item.ServiceTypeBigDescription,
                                BigDescriptionAr = item.ServiceTypeBigDescriptionArabic,
                                Status = "Active",
                                Price = !string.IsNullOrEmpty(item.TypePrice) ? Convert.ToDouble(item.TypePrice) : 0,
                                ServicePrice = item.ServiceTypePrice,
                                ServicePriceAr = item.ServiceTypePriceAr,
                                ServiceId = servicechild.ServiceId,
                              
                            };
                            db.ServiceTypes.Add(serviceType);
                            db.SaveChanges();
                            //}
                            // Product Category

                            if (category != null)
                            {
                                var serviceCategory = new ServiceCategory();
                                serviceCategory.ServiceId = service.ServiceId;
                                serviceCategory.CategoryId = category.CategoryId;
                                serviceCategory.ServiceTypeId = serviceType.ServiceTypeId;
                                serviceCategory.Status = "Active";

                                db.ServiceCategories.Add(serviceCategory);
                                db.SaveChanges();

                             
                            }

                       
                          
                        }
                        else
                        {
                            var category = db.Categories.FirstOrDefault(c => c.CategoryName.Trim().ToLower() == item.Category.Trim().ToLower() && c.FeatureCategoryId == item.FeatureCategoryId && c.Status == "Active");
                            if (category == null)
                            {
                                category = new Category();
                                category.CategoryKey = Guid.NewGuid();
                                category.CreateBy = 1;
                                category.CreateDate = DateTimeHelper.DubaiTime();
                                category.FeatureCategoryId = feacherCategoryId;
                                category.CategoryName = item.Category.Trim();
                                category.ParentId = 0;
                                if (!string.IsNullOrEmpty(item.CategoryArabic))
                                {
                                    category.CategoryNameAr = item.CategoryArabic.Trim();
                                }
                                category.Status = "Active";
                                if (!string.IsNullOrEmpty(item.CategoryImage))
                                {
                                    category.Image = "/Content/Upload/Category/" + item.CategoryImage;
                                }

                                if (!string.IsNullOrEmpty(item.CategoryIcon))
                                {
                                    category.Icon = "/Content/Upload/Category/" + item.CategoryIcon;
                                }
                                db.Categories.Add(category);
                                db.SaveChanges();
                            }


                            var Subcatlist = new List<Category>();
                            if (!string.IsNullOrEmpty(item.SubCategory))
                            {
                                // Split both strings by comma

                                if (item.SubCategory.Contains(","))
                                {
                                    var categoryList = item.SubCategory.Split(',').Select(c => c.Trim()).ToList();
                                    var categoryListAr = string.IsNullOrEmpty(item.SubCategoryArabic) == false ? item.SubCategoryArabic.Split(',').Select(c => c.Trim()).ToList() : new List<string>();
                                    var categoryiconlist = string.IsNullOrEmpty(item.SubCategoryIcon) == false ? item.SubCategoryIcon.Split(',').Select(c => c.Trim()).ToList() : new List<string>();
                                    var categoryImageList = string.IsNullOrEmpty(item.SubCategoryImage) == false ? item.SubCategoryImage.Split(',').Select(c => c.Trim()).ToList() : new List<string>();

                                    // Make sure counts match
                                    for (int i = 0; i < categoryList.Count; i++)
                                    {
                                        var categoryName = categoryList[i];


                                        var categoryNameAr = i < categoryListAr.Count && categoryListAr.Count() > 0 ? categoryListAr[i] : string.Empty;
                                        var categoryicon = i < categoryiconlist.Count && categoryiconlist.Count() > 0 ? categoryiconlist[i] : string.Empty;
                                        var categoryimage = i < categoryImageList.Count && categoryImageList.Count() > 0 ? categoryImageList[i] : string.Empty;

                                        if (!string.IsNullOrEmpty(categoryName))
                                        {
                                            var subcategory = db.Categories.FirstOrDefault(c =>
                                                c.CategoryName.Trim().ToLower() == categoryName.ToLower() &&
                                                c.FeatureCategoryId == item.FeatureCategoryId && item.Status == "Active");

                                            if (subcategory == null)
                                            {
                                                subcategory = new Category
                                                {
                                                    CategoryKey = Guid.NewGuid(),
                                                    CreateBy = 1,
                                                    CreateDate = DateTimeHelper.DubaiTime(),
                                                    FeatureCategoryId = feacherCategoryId,
                                                    CategoryName = categoryName,
                                                    CategoryNameAr = categoryNameAr,
                                                    ParentId = category.CategoryId,
                                                    Image = string.IsNullOrEmpty(categoryimage) == false ? "/Content/Upload/Category/" + item.CategoryImage : "",
                                                    Icon = string.IsNullOrEmpty(categoryicon) == false ? "/Content/Upload/Category/" + item.CategoryIcon : "",
                                                    Status = "Active"
                                                };

                                                db.Categories.Add(subcategory);
                                                db.SaveChanges();
                                                Subcatlist.Add(subcategory);
                                            }
                                            else
                                            {
                                                Subcatlist.Add(subcategory);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var categoryName = item.SubCategory.Trim();
                                    var categoryNameAr = item.SubCategoryArabic?.Trim() ?? string.Empty;

                                    var subcategory = db.Categories.FirstOrDefault(c =>
                                        c.CategoryName.Trim().ToLower() == categoryName.ToLower() &&
                                        c.FeatureCategoryId == item.FeatureCategoryId && item.Status == "Active");

                                    if (subcategory == null)
                                    {
                                        subcategory = new Category
                                        {
                                            CategoryKey = Guid.NewGuid(),
                                            CreateBy = 1,
                                            CreateDate = DateTimeHelper.DubaiTime(),
                                            FeatureCategoryId = feacherCategoryId,
                                            ParentId = category.CategoryId,
                                            CategoryName = categoryName,
                                            CategoryNameAr = categoryNameAr,
                                            Image = string.IsNullOrEmpty(item.SubCategoryImage) == false ? "/Content/Upload/Category/" + item.SubCategoryImage : "",
                                            Icon = string.IsNullOrEmpty(item.SubCategoryIcon) == false ? "/Content/Upload/Category/" + item.SubCategoryIcon : "",
                                            Status = "Active"
                                        };

                                        db.Categories.Add(subcategory);
                                        db.SaveChanges();

                                        Subcatlist.Add(subcategory);
                                    }
                                }
                            }

                            //  Country
                            var country = db.Countries.FirstOrDefault(c => c.CountryName.Trim().ToLower() == item.Country.Trim().ToLower());
                            if (country == null)
                            {
                                country = new Country();
                                country.CountryKey = Guid.NewGuid();
                                country.CreateBy = 1;
                                country.CreateDate = DateTimeHelper.DubaiTime();
                                country.CountryName = item.Country.Trim();
                                country.CountryNameAr = item.Country.Trim();
                                //subcategory.ParentId = category.CategoryId;

                                country.Status = "Active";
                                db.Countries.Add(country);
                                db.SaveChanges();
                            }
                            var city = db.Cities.FirstOrDefault(c => c.CityName.Trim().ToLower() == item.City.Trim().ToLower());
                            if (city == null)
                            {
                                city = new City();
                                city.CityKey = Guid.NewGuid();
                                city.CreateBy = 1;
                                city.CreateDate = DateTimeHelper.DubaiTime();
                                city.CityName = item.City.Trim();
                                city.CityNameAr = item.City.Trim();
                                city.CountryId = country.CountryId;
                                //subcategory.ParentId = category.CategoryId;

                                city.Status = "Active";
                                db.Cities.Add(city);
                                db.SaveChanges();
                            }


                            // Service
                            var service = db.Services.FirstOrDefault(s => s.Name.Trim().ToLower() == item.Name.Trim().ToLower() && s.FeatureCategoryId == item.FeatureCategoryId);
                            if (service == null)
                            {
                                service = new Models.Service();
                                service.ServiceKey = Guid.NewGuid();
                                service.CreateBy = 1;
                                service.CreateDate = DateTimeHelper.DubaiTime();
                                service.FeatureCategoryId = feacherCategoryId;
                                service.Name = item.Name.Trim();
                                service.NameAr = item.NameAr.Trim();
                                service.Description = item.Description;
                                service.DescriptionAr = item.DescriptionAr;
                                service.ServiceHour = !string.IsNullOrEmpty(item.ServiceHour) ? Convert.ToInt32(item.ServiceHour) : 0;
                                service.Address = item.Address;
                                service.AboutUs = item.AboutUs;
                                service.AboutUsAr = item.AboutUsAr;
                                service.IsPackage = item.IsPackage;
                                service.SpokenLanguages = item.Languages;
                                service.ScopeOfService = item.ScopeofService;
                                service.ScopeOfServiceAr = item.ScopeOfServiceAr;
                                service.Latitute = item.Latitute;
                                service.Logitute = item.Longitute;
                                if (!string.IsNullOrEmpty(item.CheckInTime))
                                {
                                    if (DateTime.TryParse(item.CheckInTime, out DateTime dt))
                                    {
                                        TimeSpan time = dt.TimeOfDay;
                                        service.CheckInTime = time;
                                    }
                                }

                                if (!string.IsNullOrEmpty(item.CheckOutTime))
                                {
                                    if (DateTime.TryParse(item.CheckOutTime, out DateTime dt))
                                    {
                                        TimeSpan time = dt.TimeOfDay;
                                        service.CheckInTime = time;
                                    }
                                }


                                service.ServiceMin = !string.IsNullOrEmpty(item.ServiceMinutes) ? Convert.ToInt32(item.ServiceMinutes) : 0;
                                service.Price = !string.IsNullOrEmpty(item.Price) ? Convert.ToInt32(item.Price) : 0;
                                service.Status = "Active";
                                db.Services.Add(service);
                                db.SaveChanges();
                            }

                            //ServiceImage
                            if (!string.IsNullOrEmpty(item.Images))
                            {
                                var image = item.Images.Split(',');
                                foreach (var item1 in image)
                                {
                                    if (!string.IsNullOrEmpty(item1))
                                    {
                                        var serviceImage = new ServiceImage
                                        {
                                            ServiceId = service.ServiceId,
                                            Image = "/Content/Upload/Service/" + item1.Trim(),
                                            IsFeature = true,
                                            CreateBy = 1,
                                            CreateDate = DateTimeHelper.DubaiTime(),
                                            Status = "Active"
                                        };
                                        db.ServiceImages.Add(serviceImage);
                                        db.SaveChanges();
                                    }
                                }
                            }




                            //serviceCategory = new ServiceCategory();
                            //serviceCategory.ServiceId = service.ServiceId;
                            //serviceCategory.CategoryId = subcategory.CategoryId;
                            //serviceCategory.Status = "Active";

                            //db.ServiceCategories.Add(serviceCategory);
                            //db.SaveChanges();


                            //Service Type

                            //var serviceType = db.ServiceTypes.FirstOrDefault(s => s.ServiceTypeName.Trim().ToLower() == item.ServiceTypeName.Trim().ToLower() && s.ServiceId == service.ServiceId);
                            //if (serviceType == null)
                            //{
                            // Add Service Type
                            var serviceType = new ServiceType
                            {
                                ServiceTypeName = item.ServiceTypeName.Trim(),
                                ServiceTypeNameAr = item.ServiceTypeNameAr.Trim(),
                                ServiceTypeKey = Guid.NewGuid(),
                                CreateBy = 1,
                                CreateDate = DateTimeHelper.DubaiTime(),
                                PersoneQuantity = !string.IsNullOrEmpty(item.PersoneQuantity) ? Convert.ToInt32(item.PersoneQuantity) : 0,
                                Description = item.TypeDescription,
                                DescriptionAr = item.TypeDescriptionAr,
                                Size = item.Size,
                                Status = "Active",
                                Price = !string.IsNullOrEmpty(item.TypePrice) ? Convert.ToDouble(item.TypePrice) : 0,
                                AdultQuantity = item.AdultQuantity != null ? Convert.ToInt32(item.AdultQuantity) : 0,
                                ChildrenQuantity = item.ChildrenQuantity != null ? Convert.ToInt32(item.ChildrenQuantity) : 0,
                                ServiceHour = !string.IsNullOrEmpty(item.TypeServiceHour) ? Convert.ToInt32(item.TypeServiceHour) : 0,
                                ServiceMin = !string.IsNullOrEmpty(item.TypeServiceMin) ? Convert.ToInt32(item.TypeServiceMin) : 0,
                                ServiceId = service.ServiceId,
                                PaymentType = item.PaymentType,
                                BigDescription = item.ServiceTypeBigDescription,
                                BigDescriptionAr = item.ServiceTypeBigDescriptionArabic
                            };
                            db.ServiceTypes.Add(serviceType);
                            db.SaveChanges();
                            //}
                            // Product Category

                            if (category != null)
                            {
                                var serviceCategory = new ServiceCategory();
                                serviceCategory.ServiceId = service.ServiceId;
                                serviceCategory.CategoryId = category.CategoryId;
                                serviceCategory.ServiceTypeId = serviceType.ServiceTypeId;
                                serviceCategory.Status = "Active";

                                db.ServiceCategories.Add(serviceCategory);
                                db.SaveChanges();

                                if (Subcatlist != null && Subcatlist.Count() > 0)
                                {


                                    foreach (var subcategory in Subcatlist)
                                    {
                                        var serviceCategoryv2 = new ServiceCategory();
                                        serviceCategoryv2.ServiceId = service.ServiceId;
                                        serviceCategoryv2.CategoryId = subcategory.CategoryId;
                                        serviceCategoryv2.ServiceTypeId = serviceType.ServiceTypeId;
                                        serviceCategoryv2.Status = "Active";

                                        db.ServiceCategories.Add(serviceCategoryv2);
                                        db.SaveChanges();
                                    }
                                }
                            }

                            // Add Service Type Images

                            if (!string.IsNullOrEmpty(item.TypeImage))
                            {
                                var image = item.TypeImage.Split(',');
                                var counter = 0;
                                foreach (var item1 in image)
                                {
                                    counter++;
                                    if (!string.IsNullOrEmpty(item1))
                                    {
                                        if (counter == 1)
                                        {
                                            var stype = db.ServiceTypes.FirstOrDefault(s => s.ServiceTypeId == serviceType.ServiceTypeId);
                                            if (stype != null)
                                            {
                                                // Add Service Type

                                                stype.Image = "/Content/Upload/Service/" + item1;

                                                db.Entry(stype).State = EntityState.Modified;
                                                db.SaveChanges();
                                            }
                                        }
                                        else
                                        {

                                            var serviceTypeImage = new ServiceTypeFile
                                            {
                                                ServiceTypeId = serviceType.ServiceTypeId,
                                                FileLocation = "/Content/Upload/Service/" + item1,
                                                FileSource = "TypeImage",
                                                FileType = "Image",
                                                LastUpdate = DateTimeHelper.DubaiTime(),
                                            };
                                            db.ServiceTypeFiles.Add(serviceTypeImage);
                                            db.SaveChanges();

                                        }
                                    }
                                }
                            }
                            //FAQ


                            var faq = db.FaqServices.Where(s => s.FaqTitle.Trim().ToLower() == item.FaqTitle.Trim().ToLower() && s.FeatureType == "Service" && s.FeatureTypeId == service.ServiceId).FirstOrDefault();
                            if (faq != null)
                            {
                                var faqinsert = new FaqService();
                                faqinsert.FaqTitle = item.FaqTitle;
                                faq.FAQKey = Guid.NewGuid();
                                faq.FaqTitleAr = item.FaqTitleAr;
                                faq.FeatureType = "Service";
                                faq.FeatureTypeId = service.ServiceId;
                                faq.FaqDescription = item.FaqDescription;
                                faq.FaqDescriptionAr = item.FaqDescriptionAr;
                                faq.LastUpdate = DateTimeHelper.DubaiTime();

                                db.FaqServices.Add(faq);
                                db.SaveChanges();

                            }
                            //Landmark


                            if (!string.IsNullOrEmpty(item.landmarkName))
                            {
                                if (item.landmarkName.Contains(","))
                                {
                                    // Split by comma → loop
                                    var landmarkNames = item.landmarkName.Split(',');
                                    var landmarkNamesAr = string.IsNullOrEmpty(item.landmarkNameArabic) ? new string[0] : item.landmarkNameArabic.Split(',');
                                    var landmarkDistances = string.IsNullOrEmpty(item.landmarkNameDistance) ? new string[0] : item.landmarkNameDistance.Split(',');
                                    var landmarkLatitudes = string.IsNullOrEmpty(item.landmarkLatitute) ? new string[0] : item.landmarkLatitute.Split(',');
                                    var landmarkLongitudes = string.IsNullOrEmpty(item.landmarkLongitute) ? new string[0] : item.landmarkLongitute.Split(',');

                                    for (int i = 0; i < landmarkNames.Length; i++)
                                    {
                                        var landmarkName = landmarkNames[i].Trim();

                                        if (string.IsNullOrEmpty(landmarkName))
                                            continue;

                                        // Check if landmark already exists
                                        var existingLandmark = db.ServiceLandmark
                                            .FirstOrDefault(s => s.Name.Trim().ToLower() == landmarkName.ToLower() && s.ServiceId == service.ServiceId);

                                        if (existingLandmark == null)
                                        {
                                            var land = new ServiceLandmark
                                            {
                                                ServiceLandmarkKey = Guid.NewGuid(),
                                                Name = landmarkName,
                                                NameAr = landmarkNamesAr[i].Trim(),
                                                Address = "",
                                                Distance = (landmarkDistances.Length > i && !string.IsNullOrEmpty(landmarkDistances[i]))
                                                            ? Convert.ToDouble(landmarkDistances[i])
                                                            : 0,
                                                Latitude = (landmarkLatitudes.Length > i ? landmarkLatitudes[i] : ""),
                                                Longitude = (landmarkLongitudes.Length > i ? landmarkLongitudes[i] : ""),
                                                ServiceId = service.ServiceId,
                                                CreateDate = DateTimeHelper.DubaiTime(),
                                                CreateBy = 1,
                                                Status = "Active"
                                            };

                                            db.ServiceLandmark.Add(land);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    var landmark = db.ServiceLandmark.Where(s => s.Name.Trim().ToLower() == item.landmarkName.Trim().ToLower() && s.ServiceId == service.ServiceId).FirstOrDefault();
                                    if (landmark != null)
                                    {
                                        var land = new ServiceLandmark();
                                        land.ServiceLandmarkKey = Guid.NewGuid();
                                        land.Name = item.landmarkName;
                                        land.Address = "";
                                        land.Distance = Convert.ToDouble(item.landmarkNameDistance);
                                        land.Latitude = item.landmarkLatitute;
                                        land.Longitude = item.landmarkLongitute;
                                        land.ServiceId = service.ServiceId;
                                        land.CreateDate = DateTimeHelper.DubaiTime();
                                        land.CreateBy = 1;
                                        land.Status = "Active";
                                        db.ServiceLandmark.Add(landmark);
                                        db.SaveChanges();

                                    }
                                }
                            }



                            // Add Service Amenity
                            if (!string.IsNullOrEmpty(item.AmenitiesName))
                            {
                                if (item.AmenitiesName.Contains(","))
                                {
                                    // Split by comma → loop
                                    var amenityNames = item.AmenitiesName.Split(',');
                                    var amenityImages = string.IsNullOrEmpty(item.AmenitiesImage) ? new string[0] : item.AmenitiesImage.Split(',');

                                    for (int i = 0; i < amenityNames.Length; i++)
                                    {
                                        var amenityName = amenityNames[i].Trim();

                                        if (string.IsNullOrEmpty(amenityName))
                                            continue;

                                        // Check if amenity already exists
                                        var existingAmenity = db.ServiceAmenities
                                            .FirstOrDefault(s => s.AmenitiesName.Trim().ToLower() == amenityName.ToLower() && s.ServiceId == service.ServiceId);

                                        if (existingAmenity == null)
                                        {
                                            var serviceAmenity = new ServiceTypeAmenity
                                            {
                                                ServiceAmenityKey = Guid.NewGuid(),
                                                AmenitiesName = amenityName,
                                                ServiceTypeId = serviceType.ServiceTypeId,
                                                CreateBy = 1,
                                                CreateDate = DateTimeHelper.DubaiTime(),
                                                Status = "Active",
                                                AmenitiesLogo = (amenityImages.Length > i && !string.IsNullOrEmpty(amenityImages[i]))
                                                              ? "/Content/Upload/ServiceAmenity/" + amenityImages[i].Trim()
                                                              : null
                                            };

                                            db.ServiceTypeAmenities.Add(serviceAmenity);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    var existingAmenity = db.ServiceAmenities.FirstOrDefault(s => s.AmenitiesName.Trim().ToLower() == item.AmenitiesName.Trim().ToLower() && s.ServiceId == service.ServiceId);

                                    if (existingAmenity == null)
                                    {
                                        var serviceAmenity = new ServiceAmenity
                                        {
                                            ServiceAmenityKey = Guid.NewGuid(),
                                            AmenitiesName = item.AmenitiesName.Trim(),
                                            ServiceId = service.ServiceId,
                                            CreateBy = 1,
                                            CreateDate = DateTimeHelper.DubaiTime(),
                                            Status = "Active",
                                            AmenitiesLogo = !string.IsNullOrEmpty(item.AmenitiesImage) ? "/Content/Upload/ServiceAmenity/" + item.AmenitiesImage.Trim() : null
                                        };

                                        db.ServiceAmenities.Add(serviceAmenity);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            //var serviceAmenity = db.ServiceAmenities.FirstOrDefault(s => s.AmenitiesName.Trim().ToLower() == item.AmenitiesName.Trim().ToLower() && s.ServiceId == service.ServiceId);
                            //if (serviceAmenity != null && !string.IsNullOrEmpty(item.AmenitiesName))
                            //{
                            //    serviceAmenity = new ServiceAmenity
                            //    {
                            //        ServiceAmenityKey = Guid.NewGuid(),
                            //        AmenitiesName = item.AmenitiesName.Trim(),
                            //        ServiceId = service.ServiceId,
                            //        CreateBy = 1,
                            //        CreateDate = DateTimeHelper.DubaiTime(),
                            //        Status = "Active",
                            //        AmenitiesLogo = !string.IsNullOrEmpty(item.AmenitiesImage) ? "/Content/Upload/ServiceAmenity/" + item.AmenitiesImage.Trim() : null
                            //    };
                            //    db.ServiceAmenities.Add(serviceAmenity);
                            //    db.SaveChanges();
                            //}
                        }




                    }
                    catch (Exception ex)
                    {

                        continue;
                    }



                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> AddTempService(string xmlFileData, int feacherCategoryId,bool isPackage=false)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["BoulevardDbContext"].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("@tempServices_xml", xmlFileData, DbType.Xml);
                    parameters.Add("@feacherCategoryId", feacherCategoryId);
                    parameters.Add("@isPackage", isPackage);
                    await connection.ExecuteAsync("pr_upload_bulk_service", parameters, commandType: CommandType.StoredProcedure);

                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}