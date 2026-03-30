using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.Service;
using Boulevard.Service.Admin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace Boulevard.Areas.Admin.Controllers
{
    public class PharmacyController : Controller
    {
        private ServiceAccess _serviceAccess;
        private FeatureCategoryAccess _featureCategoryAccess;
        private CountryDataAccess _countryDataAccess;
        private CityDataAccess _cityDataAccess;

        public PharmacyController()
        {
            _serviceAccess = new ServiceAccess();
            _featureCategoryAccess = new FeatureCategoryAccess();
            _countryDataAccess = new CountryDataAccess();
            _cityDataAccess = new CityDataAccess();

        }
    
        public async Task<ActionResult> SalonServiceIndex()
        {
            List<Boulevard.Models.Service> list = new List<Boulevard.Models.Service>();
            Boulevard.Models.Service obj1 = new Boulevard.Models.Service() 
            {
                ServiceKey=Guid.NewGuid(),
                Name = "Opra Salon",
                Description = "We Provide High Quality Service",
                ServiceHour = 13,
                FeatureCategoryId=1,
                CityId=1,
                Address= "Fujairah",
                Ratings=4.5,
                Latitute= "25.2048",
                Logitute= "55.2708",
                SpokenLanguages= "Arabic,English,Bangla",
                AboutUs= "/Content/Salon/1.jpeg"
            };
            Boulevard.Models.Service obj2 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "Quick Buck Salon",
                Description = "We Provide High Quality Service",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Ajman",
                Ratings = 4.1,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Salon/2.jpeg"
            };
            Boulevard.Models.Service obj3 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "AK Salon",
                Description = "We Provide High Quality Service",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Sharjah",
                Ratings = 4.2,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Salon/3.jpeg"
            };
            Boulevard.Models.Service obj4 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "Grom From",
                Description = "We Provide High Quality Service",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Dubai",
                Ratings = 5,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Salon/4.jpeg"
            };
            Boulevard.Models.Service obj5 = new Boulevard.Models.Service()
            {
                ServiceKey = Guid.NewGuid(),
                Name = "AliBaBa Salon",
                Description = "We Provide High Quality Service",
                ServiceHour = 13,
                FeatureCategoryId = 1,
                CityId = 1,
                Address = "Abu Dhabi",
                Ratings = 3.8,
                Latitute = "25.2048",
                Logitute = "55.2708",
                SpokenLanguages = "Arabic,English,Bangla",
                AboutUs = "/Content/Salon/5.png"
            };
            list.Add(obj1);
            list.Add(obj2);
            list.Add(obj3);
            list.Add(obj4); 
            list.Add(obj5);
            return View(list);
        }
        [HttpGet]
        
        public async Task<ActionResult> SalonServiceCreate(Guid? Key)
        {
            ViewBag.countries = new SelectList(await _countryDataAccess.GetAll(), "CountryId", "CountryName");

            if (Key == null || Key == Guid.Empty)
            {
                Boulevard.Models.Service node = new Boulevard.Models.Service();
                node.ServiceImages = new List<ServiceImage> { new ServiceImage() };
                return View(node);
            }
            else
            {

                Boulevard.Models.Service node = new Boulevard.Models.Service()
                {
                    ServiceKey = Guid.NewGuid(),
                    Name = "AliBaBa Salon",
                    Description = "We Provide High Quality Service",
                    ServiceHour = 13,
                    FeatureCategoryId = 1,
                    CityId = 1,
                    Address = "Abu Dhabi",
                    Ratings = 3.8,
                    Latitute = "25.2048",
                    Logitute = "55.2708",
                    SpokenLanguages = "Arabic,English,Bangla",
                    AboutUs = "/Content/Salon/5.png"
                };

                node.ServiceImages = new List<ServiceImage>();
                ServiceImage img = new ServiceImage() 
                {
                    Image= "/Content/Salon/5.png"
                };
                node.ServiceImages.Add(img);
                return View(node);
            }
        }

        public async Task<ActionResult> SalonServiceTypeIndex()
        {
            List<ServiceType> list = new List<ServiceType>();
            ServiceType obj1 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Hair Cut",
                Description = "Quick Service",
                Image = "Opra Salon",
                Size = "100",
                Price = 655
            };
            ServiceType obj2 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Facial Wash",
                Description = "Quick Service",
                Image = "Opra Salon",
                Size = "100",
                Price = 750
            };
            ServiceType obj3 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Facial Shave",
                Description = "Quick Service",
                Image = "Opra Salon",
                Size = "100",
                Price = 460
            };
            ServiceType obj4 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Manicure",
                Description = "Quick Service",
                Image = "Opra Salon",
                Size = "100",
                Price = 300
            };
            ServiceType obj5 = new ServiceType()
            {
                ServiceTypeKey = Guid.NewGuid(),
                ServiceTypeName = "Pedicure",
                Description = "Laser Treatment",
                Image = "Opra Salon",
                Size = "100",
                Price = 500
            };
            list.Add(obj1);
            list.Add(obj2);
            list.Add(obj3);
            list.Add(obj4);
            list.Add(obj5);
            return View(list);
        }
        [HttpGet]
        public async Task<ActionResult> SalonServiceTypeCreate(Guid? Key)
        {
            ViewBag.service = new SelectList(await _serviceAccess.GetAllSalonServices(), "ServiceId", "Name");
            if (Key == null || Key == Guid.Empty)
            {
                ServiceType node = new ServiceType();
                return View(node);
            }
            else
            {
                ServiceType node = new ServiceType()
                {
                    ServiceTypeKey = Guid.NewGuid(),
                    ServiceTypeName = "Pedicure",
                    Description = "Laser Treatment",
                    Image = "Opra Salon",
                    Size = "100",
                    Price = 500
                };
                return View(node);
            }
        }
        public async Task<ActionResult> SalonCategoryIndex()
        {

            List<ServiceAmenity> list = new List<ServiceAmenity>();

            ServiceAmenity obj1 = new ServiceAmenity()
            {
                AmenitiesName = "Male",
                AmenitiesLogo = "Opra Salon",
                ServiceAmenityKey = Guid.NewGuid()
            };
            ServiceAmenity obj2 = new ServiceAmenity()
            {
                AmenitiesName = "Female",
                AmenitiesLogo = "Opra Salon",
                ServiceAmenityKey = Guid.NewGuid()
            };
            ServiceAmenity obj3 = new ServiceAmenity()
            {
                AmenitiesName = "Unisex",
                AmenitiesLogo = "Opra Salon",
                ServiceAmenityKey = Guid.NewGuid()
            };
            
            list.Add(obj1);
            list.Add(obj2);
            list.Add(obj3);
            return View(list);
        }

        [HttpGet]
        public async Task<ActionResult> SalonCategoryCreate(Guid? Key)
        {
            ViewBag.service = new SelectList(await _serviceAccess.GetAllSalonServices(), "ServiceId", "Name");

            if (Key == null || Key == Guid.Empty)
            {
                ServiceAmenity node = new ServiceAmenity();
                return View(node);
            }
            else
            {
                ServiceAmenity node = new ServiceAmenity()
                {
                    AmenitiesName = "Unisex",
                    AmenitiesLogo = "Opra Salon",
                    ServiceId=1,
                    ServiceAmenityKey = Guid.NewGuid()
                };
                return View(node);
            }
        }
        public async Task<ActionResult> SalonBannerIndex()
        {

            List<WebHtml> List = new List<WebHtml>();
            WebHtml obj1 = new WebHtml()
            {
                WebHtmlkey = Guid.NewGuid(),
                Identifier = "Top Banner",
                Title = "Get Exiting Offer og 35% Off..!",
                SubTitle = "This Sunday for every Platinum Members.",
                SmallDetailsOne = "35% Off",
                SmallDetailsTwo = "Hot Sunday",
                BigDetailsOne = "All of our Platinum members will get 35% off on every service.",
                BigDetailsTwo = "All of our Platinum members will get 35% off on every service.",
                PictureOne = "/Content/Salon/6.png",
                PictureTwo = "/Content/Salon/6.png",
                PictureThree = "/Content/Salon/6.png",
                FeatureCategoryId = 1,
                ButtonLink = "Opra Salon"
            };
            WebHtml obj2 = new WebHtml()
            {
                WebHtmlkey = Guid.NewGuid(),
                Identifier = "Bottom Banner",
                Title = "25% off on Friday!",
                SubTitle = "25% off on Friday!",
                SmallDetailsOne = "25% Off",
                SmallDetailsTwo = "Hot Fariday",
                BigDetailsOne = "Quicker then Anything Around..!",
                BigDetailsTwo = "Quicker then Anything Around..!",
                PictureOne = "/Content/Salon/7.png",
                PictureTwo = "/Content/Salon/7.png",
                PictureThree = "/Content/Salon/7.png",
                FeatureCategoryId = 1,
                ButtonLink = "Opra Salon"
            };
            List.Add(obj1);
            List.Add(obj2);
            return View(List);


        }
        [HttpGet]
        public async Task<ActionResult> SalonBannerCreate(string key)
        {
            try
            {
                ViewBag.service = new SelectList(await _serviceAccess.GetAllSalonServices(), "ServiceId", "Name");
                WebHtml data = new WebHtml();
             
                if (string.IsNullOrEmpty(key))
                {
                    return View(data);
                }
                else
                {
                    data = new WebHtml()
                    {
                        WebHtmlkey = Guid.NewGuid(),
                        Identifier = "Bottom Banner",
                        Title = "25% off on Friday!",
                        SubTitle = "25% off on Friday!",
                        SmallDetailsOne = "25% Off",
                        SmallDetailsTwo = "Hot Fariday",
                        BigDetailsOne = "Quicker then Anything Around..!",
                        BigDetailsTwo = "Quicker then Anything Around..!",
                        PictureOne = "/Content/Salon/7.png",
                        PictureTwo = "/Content/Salon/7.png",
                        PictureThree = "/Content/Salon/7.png",
                        FeatureCategoryId = 1,
                        ButtonLink = "Opra Salon"
                    };
                    return View(data);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }
    }
}