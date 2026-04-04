using Boulevard.BaseRepository;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.Services.Description;


namespace Boulevard.Service.WebAPI
{
    public class ServiceAccess
    {
        public IUnitOfWork uow;
        public ServiceAccess()
        {
            uow = new UnitOfWork();
        }
        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
        //public async Task<List<SmallServiceDetailsResponse>> GetPackaedges()
        //{
        //    var listofservices = new List<SmallServiceDetailsResponse>();
        //    var allPackaedges = await uow.ServiceRepository.Get().Where(p => p.IsPackage == true).ToListAsync();
        //    if (allPackaedges != null && allPackaedges.Count > 0)
        //    {
        //        foreach (var obj in allPackaedges)
        //        {
        //            var ss = await GetSmallServices(obj.ServiceId);
        //            if (ss != null)
        //            {
        //                listofservices.Add(ss);
        //            }
        //        }
        //    }
        //    return listofservices;
        //}
        public async Task<PaginatedResponse<SmallServiceDetailsResponse>> GetPackaedges(int featureCategoryId, int pageNumber, int pageSize,string lang="en")
        {
            var listofservices = new List<SmallServiceDetailsResponse>();
            var allPackaedges = await uow.ServiceRepository.Get()
                                        .Where(p =>p.FeatureCategoryId== featureCategoryId && p.IsPackage == true)
                                        .OrderBy(p => p.ServiceId) // Order by a property like ServiceId
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
            if (allPackaedges != null && allPackaedges.Count > 0)
            {
                foreach (var obj in allPackaedges)
                {
                    var ss = await GetSmallServices(obj.ServiceId,0,0,lang);
                    if (ss != null)
                    {
                        listofservices.Add(ss);
                    }
                }
            }

            var totalRecords = await uow.ServiceRepository.Get().Where(p => p.IsPackage == true).CountAsync();

            var response = new PaginatedResponse<SmallServiceDetailsResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Data = listofservices
            };

            return response;
        }
        public async Task<List<SmallServiceDetailsResponse>> GetServices(ServiceSearchingRequest request)
        {
            try
            {
                bool anyMatch = true;
                var listofservices = new List<SmallServiceDetailsResponse>();
                var allServices = await uow.ServiceRepository.Get().Where(p => p.FeatureCategoryId == request.FeatureCategoryId && p.Status == "Active" && p.IsPackage == true).ToListAsync();
                if (allServices != null && allServices.Count() > 0)
                {
                    if (request.CategoryId > 0)
                    {
                        var serviceIds = allServices.Select(p => p.ServiceId).ToList();
                        var catservices = await uow.ServiceCategoryRepository.Get().Where(s => serviceIds.Contains(s.ServiceId.Value)).Select(s => s.ServiceId).ToListAsync();
                        if (catservices != null && catservices.Count() > 0)
                        {
                            allServices = allServices.Where(s => catservices.Contains(s.ServiceId)).ToList();
                        }
                        
                       
                }
                    if (request.AmenityId != null && request.AmenityId.Count() > 0)
                    {
                        var serviceIds = allServices.Select(p => p.ServiceId).ToList();
                        var amenities = await uow.ServiceAmenityRepository.Get().Where(s => serviceIds.Contains(s.ServiceId) && request.AmenityId.Contains(s.ServiceAmenityId)).Select(s => s.ServiceId).ToListAsync();
                        if (amenities != null && amenities.Count() > 0)
                        {
                            allServices = allServices.Where(s => amenities.Contains(s.ServiceId)).ToList();
                        }
                    }
                    if (request.CountryId > 0)
                    {
                        allServices = allServices.Where(c => c.CountryId == request.CountryId).ToList();
                    }
                    if (request.CityId > 0)
                    {
                        allServices = allServices.Where(p => p.CityId == request.CityId).ToList();
                    }

                    foreach (var node in allServices)
                    {
                        string Country = "", City = "";
                        if (node.CountryId != null && node.CountryId > 0)
                        {
                            var cnt = await uow.CountryRepository.Get().Where(p => p.CountryId == node.CountryId).FirstOrDefaultAsync();
                            if (cnt != null)
                            {
                                Country = request.lang == "en" ? cnt.CountryName : cnt.CountryNameAr;
                            }
                        }
                        if (node.CityId != null && node.CityId > 0)
                        {
                            var ct = await uow.CityRepository.Get().Where(p => p.CityId == node.CityId).FirstOrDefaultAsync();
                            if (ct != null)
                            {
                                City = request.lang == "en" ? ct.CityName : ct.CityNameAr;
                            }
                        }
                        string ConcatedAddress = node.Address + " " + City + " " + Country;
                        //Flight 
                        if (request.FromAirportId > 0 && request.ToAirportId > 0)
                        {

                            if (await uow.AirportServiceRepository.Get().AnyAsync(s => s.ServiceId == node.ServiceId && s.AirportId == request.FromAirportId) == true && await uow.AirportServiceRepository.Get().AnyAsync(s => s.ServiceId == node.ServiceId && s.AirportId == request.ToAirportId) == true)
                            {
                                anyMatch = true;
                            }
                            else
                            {
                                anyMatch = false;
                            }

                        }

                        // Check for any matches

                        if (!string.IsNullOrEmpty(request.LocationKeyword))
                        {
                            anyMatch = ConcatedAddress.ToLower().Contains(request.LocationKeyword.ToLower());
                        }
                        if (anyMatch)
                        {
                            var ServiceTypes = await uow.ServiceTypesRepository.Get().Where(p => p.ServiceId == node.ServiceId && node.Status == "Active"
                            && p.ChildrenQuantity >= request.ChildrenQuantity && p.AdultQuantity >= request.AdultQuantity).ToListAsync();
                            if (!string.IsNullOrEmpty(request.SearchKeyword))
                            {
                                ServiceTypes = ServiceTypes.Where(p => p.Service.Name.Trim().ToLower().Contains(request.SearchKeyword.Trim().ToLower()) || p.ServiceTypeName.Trim().ToLower().Contains(request.SearchKeyword.Trim().ToLower())).ToList();
                            }
                            if (request.RoomQuantity > 0)
                            {
                                ServiceTypes = ServiceTypes.Where(p => p.PersoneQuantity >= request.RoomQuantity).ToList();
                            }


                            if (ServiceTypes != null && ServiceTypes.Count > 0)
                            {
                                foreach (var obj in ServiceTypes)
                                {
                                    var ss = await GetSmallServices(node.ServiceId, obj.ServiceTypeId, 0, request.lang);
                                    if (ss != null)
                                    {
                                        listofservices.Add(ss);
                                    }
                                }

                            }

                        }


                    }
                }
                return listofservices;

            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }


        public async Task<ParentSmallServiceDetailsResponse> GetServiceOnlyTypingandInsuranceService(int serviceId, int memberId=0,string lang = "en")
        {

            try
            {
                var res = new ParentSmallServiceDetailsResponse();

                var AreaFilteredList = await uow.ServiceRepository.Get().Where(p => p.ServiceId == serviceId && p.Status == "Active" && p.ParentId == 0).Include(s => s.City).Include(s => s.Country).ToListAsync();

                            if (AreaFilteredList != null && AreaFilteredList.Count() > 0)
                            {
                                foreach (var AreaFiltered in AreaFilteredList)
                                {
                                    if (AreaFiltered != null)
                                    {
                                       
                                        res.ServiceId = AreaFiltered.ServiceId;
                                        res.ServiceName = lang == "en" ? AreaFiltered.Name : AreaFiltered.NameAr;
                                        res.City = await new CityAccess().GetById(AreaFiltered.CityId.Value, lang);
                                        res.Country = await new CountryAccess().GetById(AreaFiltered.CountryId.Value, lang);
                                        res.CheckInTime = AreaFiltered.CheckInTime;
                                        res.CheckOutTime = AreaFiltered.CheckOutTime;

                                        res.ServiceAddress = AreaFiltered.Address;
                                        res.Description = lang == "en" ? AreaFiltered.Description : AreaFiltered.DescriptionAr;

                                        var ChlidListService = await uow.ServiceRepository.Get().Where(p => p.ParentId == AreaFiltered.ServiceId && p.Status == "Active").Select(s => s.ServiceId).ToListAsync();
                                        if (ChlidListService != null && ChlidListService.Count() > 0)
                                        {
                                            foreach (var child in ChlidListService)
                                            {
                                                var ServiceResult = await new ServiceAccess().GetSmallServices(child, 0, memberId, lang);
                                                if (ServiceResult != null)
                                                {
                                                    res.ChildServices.Add(ServiceResult);
                                                }
                                            }

                                        }

                                        

                                    }
                                }

                            


                      
                    }
                    else
                    {
                        
                    }



                return res;


            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public async Task<Boulevard.Models.Service> GetServiceDetailsById(int serviceId, int memberId = 0,string lang="en")
        {
            try
            {
                Boulevard.Models.Service Service = await uow.ServiceRepository.Get().Where(p => p.ServiceId == serviceId).Include(p => p.ServiceLandmarks).Include(p => p.FeatureCategory).Include(p => p.ServiceTypes).Include(p => p.ServiceAmenities).Include(p => p.ServiceImages).Include(p => p.Country).Include(p => p.City).FirstOrDefaultAsync();
                if (Service == null) return null;
                Service.Name = lang == "en" ? Service.Name : Service.NameAr;
                if (Service.CityId.HasValue)
                    Service.City = await new CityAccess().GetById(Service.CityId.Value, lang);
                if (Service.CountryId.HasValue)
                    Service.Country = await new CountryAccess().GetById(Service.CountryId.Value, lang);
                Service.UserReviews = await uow.UserReviewRepository.Get().Where(p => p.FeatureType.ToLower() == "service" && p.FeatureTypeId == serviceId).Include(p => p.UserReviewImages).ToListAsync();
                if (Service.UserReviews != null && Service.UserReviews.Count > 0)
                {
                    foreach (var item in Service.UserReviews)
                    {
                        if (item.UserType.ToLower() == "member")
                        {
                            var Member = await uow.MemberRepository.Get().Where(p => p.MemberId == item.UserId).FirstOrDefaultAsync();
                            item.MemberName = Member.Name;
                        }
                        var userReviewImages = await uow.UserReviewImageRepository.Get().Where(s => s.UserReviewId == item.UserReviewId).ToListAsync();
                        if (userReviewImages != null && userReviewImages.Count() > 0)
                        {
                            foreach (var itemimage in userReviewImages)
                            {
                                itemimage.UserReview = null;
                                if (!string.IsNullOrEmpty(itemimage.Image))
                                {
                                    itemimage.Image = link + itemimage.Image;
                                    item.UserReviewImages.Add(itemimage);
                                }
                            }
                        }
                    }
                }

                Service.FeatureCategory.Image = link + Service.FeatureCategory.Image;
                if (Service.ServiceImages != null && Service.ServiceImages.Count > 0)
                {
                    foreach (var img in Service.ServiceImages)
                    {
                        img.Service = null;
                        if (!string.IsNullOrEmpty(img.Image))
                            img.Image = link + img.Image;
                    }
                }

                if (Service.ServiceTypes != null && Service.ServiceTypes.Count > 0)
                {
                    foreach (var img in Service.ServiceTypes)
                    {
                        img.ServiceTypeName = lang == "en" ? img.ServiceTypeName : img.ServiceTypeNameAr;
                        img.Description = lang == "en" ? img.Description : img.DescriptionAr;
                        img.Size = lang == "en" ? img.Size : img.SizeAr;
                        img.Service = null;
                        if (!string.IsNullOrEmpty(img.Image))
                            img.Image = link + img.Image;

                        var discountinfo = await new OfferServiceAccess().ServiceDiscountCheck(serviceId, img.ServiceTypeId, Service.FeatureCategoryId,lang);
                        if (discountinfo != null)
                        {
                            img.DiscountInformation = discountinfo;
                            if (discountinfo.DiscountType.ToLower() == "percentage")
                            {
                                img.DiscountPrice = Convert.ToDouble(img.Price - ((img.Price * discountinfo.DiscountAmount) / 100));
                            }
                            else
                            {
                                img.DiscountPrice = Convert.ToDouble(img.Price - discountinfo.DiscountAmount);
                            }
                        }
                        else
                        {
                            if (memberId > 0)
                            {
                                var memberInfo = await new MemberServiceAccess().GetById(memberId);

                                if (memberInfo.IsAnyMembership == true && memberInfo.MemberSubscriptions != null)
                                {
                                    var MembershipDiscount = await uow.MemberShipDiscountCategoryRepository.Get().Where(s => s.FeatureCategoryId == Service.FeatureCategoryId && s.MemberShipId == memberInfo.MemberSubscriptions.MemberShipId).FirstOrDefaultAsync();
                                    if (MembershipDiscount != null)
                                    {
                                        img.MemberShipInformation = MembershipDiscount;
                                        if (MembershipDiscount.MemberShipDiscountType.ToLower() == "percentage")
                                        {
                                            img.DiscountPrice = Convert.ToDouble(img.Price - ((img.Price * Convert.ToDouble(MembershipDiscount.MemberShipDiscountAmount)) / 100));
                                        }
                                        else
                                        {
                                            img.DiscountPrice = Convert.ToDouble(img.Price - Convert.ToDouble(MembershipDiscount.MemberShipDiscountAmount));

                                        }
                                    }


                                }

                            }
                        }
                        if (memberId > 0)
                        {
                            img.IsFavourite = await uow.FavouriteServiceRepository.Get().AnyAsync(s => s.ServiceId == img.ServiceId && s.ServiceTypeId == img.ServiceTypeId && s.MemberId == memberId);
                        }
                    }
                }
                if (Service.ServiceAmenities != null && Service.ServiceAmenities.Count > 0)
                {
                    foreach (var aminities in Service.ServiceAmenities)
                    {
                        aminities.AmenitiesName = lang == "en" ? aminities.AmenitiesName : aminities.AmenitiesNameAr;
                        aminities.Service = null;
                        if (!string.IsNullOrEmpty(aminities.AmenitiesLogo))
                            aminities.AmenitiesLogo = link + aminities.AmenitiesLogo;
                    }
                }

                //foreach (var rev in Service.UserReviews)
                //{

                //    foreach (var review in rev.userReviewImages)
                //    {
                //            review.UserReview = null;
                //        if (!string.IsNullOrEmpty(review.Image))
                //                review.Image = link + review.Image;
                //    }
                //}

                if (Service.ServiceLandmarks != null && Service.ServiceLandmarks.Count > 0)
                {
                    foreach (var land in Service.ServiceLandmarks)
                    {
                        land.Service = null;

                    }
                }


                var faqList = await uow.FaqServiceRepository.Get().Where(s => s.Status == "Active" && s.FeatureType.ToLower() == "service" && s.FeatureTypeId == serviceId).ToListAsync();
                if (faqList != null && faqList.Count() > 0)
                {
                    foreach (var faq in faqList)
                    {
                        var faqresult = new FaqResponse();
                        faqresult.FaqServiceId = faq.FaqServiceId;
                        faqresult.FaqTitle = faq.FaqTitle;
                        faqresult.FaqDescription = faq.FaqDescription;
                        Service.Faqs.Add(faqresult);

                    }
                }

                var airports = await uow.AirportServiceRepository.Get().Where(s => s.ServiceId == serviceId && s.Status=="Active").Select(s=>s.AirportId).ToListAsync();
                if (airports != null && airports.Count() > 0)
                {
                    foreach (var ss in airports)
                    {
                        var air = await uow.AirportRepository.Get().Where(s => s.AirportId == ss).FirstOrDefaultAsync();
                        if (air != null)
                        {
                            air.AirportName = lang == "en" ? air.AirportName : air.AirportNameAr;
                            Service.Airports.Add(air);
                        }
                    }
                }
                return Service;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<List<SmallServiceDetailsResponse>> GetRelatedServices(int featureId, int serviceId,string lang="en")
        {
            try
            {
                var resultList = new List<SmallServiceDetailsResponse>();

                var AreaFiltered = await uow.ServiceRepository.Get().Where(p => p.FeatureCategoryId == featureId && p.ServiceId != serviceId && p.IsPackage == false).Include(s => s.City).Include(s => s.Country).Include(s => s.ServiceTypes).ToListAsync();
                if (AreaFiltered != null && AreaFiltered.Count() > 0)
                {


                    foreach (var item in AreaFiltered)
                    {
                        var result = new SmallServiceDetailsResponse();
                        result.ServiceId = item.ServiceId;
                        result.ServiceName = lang == "en" ? item.Name : item.NameAr;
                        result.City = await new CityAccess().GetById(item.CityId.Value, lang);
                        result.Country = await new CountryAccess().GetById(item.CountryId.Value, lang);
                        result.Price = item.ServiceTypes.FirstOrDefault().Price;
                        result.Ratings = item.Ratings;
                        result.ReviewCount = await uow.UserReviewRepository.Get().Where(p => p.FeatureType.ToLower() == "service" && p.FeatureTypeId == item.ServiceId).CountAsync();
                        var imagelist = await uow.ServiceImageRepository.Get().Where(s => s.ServiceId == item.ServiceId).ToListAsync();

                        if (imagelist.Count() > 0 && imagelist != null)
                        {
                            foreach (var image in imagelist)
                            {
                                image.Service = null;
                                image.Image = link + image.Image;
                            }
                            result.Images = imagelist;
                        }
                        result.Amenities = lang=="en"?await uow.ServiceAmenityRepository.Get().Where(s => s.ServiceId == item.ServiceId).Select(s => s.AmenitiesName).ToListAsync(): await uow.ServiceAmenityRepository.Get().Where(s => s.ServiceId == item.ServiceId).Select(s => s.AmenitiesNameAr).ToListAsync();
                        resultList.Add(result);

                    }

                }


                return resultList;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }
        public async Task<List<SmallServiceDetailsResponse>> GetSimilarDestination(int featureId, int serviceId,string lang="en")
        {
            try
            {
                var listofservices = new List<SmallServiceDetailsResponse>();
                var resultList = new List<SmallServiceDetailsResponse>();
                var service = await uow.ServiceRepository.Get().Where(p => p.ServiceId == serviceId && p.IsPackage == false).FirstOrDefaultAsync();

                //var AreaFiltered = await uow.ServiceRepository.Get().Where(p => p.FeatureCategoryId == featureId && p.CountryId== service.CountryId && p.CityId == service.CityId && p.ServiceId != serviceId).Include(s => s.City).Include(s => s.Country).Include(s => s.ServiceTypes).ToListAsync();
                var AreaFiltered = await uow.ServiceRepository.Get().Where(p => p.FeatureCategoryId == featureId && p.CountryId == service.CountryId && p.CityId == service.CityId && p.ServiceId != serviceId && p.IsPackage == false).ToListAsync();
                if (AreaFiltered != null && AreaFiltered.Count() > 0)
                {





                    foreach (var item in AreaFiltered)
                    {

                        var ss = await GetSmallServices(item.ServiceId,0,0,lang);
                        if (ss != null)
                        {

                            listofservices.Add(ss);
                        }



                        //var result = new SmallServiceDetailsResponse();
                        //result.ServiceId = item.ServiceId;
                        //result.ServiceName = item.Name;
                        //result.City = item.City;
                        //result.Country = item.Country;
                        //result.Price = item.ServiceTypes.FirstOrDefault().Price;
                        //result.Ratings = item.Ratings;
                        //result.ReviewCount = await uow.UserReviewRepository.Get().Where(p => p.FeatureType.ToLower() == "service" && p.FeatureTypeId == item.ServiceId).CountAsync();
                        //var imagelist = await uow.ServiceImageRepository.Get().Where(s => s.ServiceId == item.ServiceId).ToListAsync();

                        //if (imagelist.Count() > 0 && imagelist != null)
                        //{
                        //    foreach (var image in imagelist)
                        //    {
                        //        image.Service = null;
                        //        image.Image = link + image.Image;
                        //    }
                        //    result.Images = imagelist;
                        //}
                        //result.Amenities = await uow.ServiceAmenityRepository.Get().Where(s => s.ServiceId == item.ServiceId).Select(s => s.AmenitiesName).ToListAsync();
                        //resultList.Add(result);

                    }

                }


                return listofservices;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }





        public async Task<List<SmallServiceDetailsResponse>> GetSearchingServices(int featureCategoryId, string keyword = "", int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {
            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var listofservices = new List<SmallServiceDetailsResponse>();
                var serviceIds = new List<int>();
                if (!string.IsNullOrEmpty(keyword))
                {
                    var searchWords = keyword.ToLower().Split(" ".ToCharArray(),
                        StringSplitOptions.RemoveEmptyEntries);


                    var types = await uow.ServiceTypesRepository.Get().Where(s => searchWords.Any(t => s.ServiceTypeName.ToLower().Contains(t)) && s.Status == "Active").OrderByDescending(s => s.ServiceId).ToListAsync();
                    if (types.Count() > 0 && types != null)
                        foreach (var type in types)
                        {
                            {
                                var exists = await uow.ServiceRepository.Get().AnyAsync(s => s.FeatureCategoryId == featureCategoryId && s.ServiceId == type.ServiceId && s.IsPackage == false);
                                if (exists == true)
                                {

                                    var ss = await GetSmallServices(type.ServiceId, type.ServiceTypeId, memberId,lang);
                                    if (ss != null)
                                    {

                                        listofservices.Add(ss);
                                    }


                                }
                            }
                        }
                    return listofservices;
                } 
                else
                {
                    // No keyword — return all active service types for this category
                    var categoryServiceIds = await uow.ServiceRepository.Get()
                        .Where(s => s.FeatureCategoryId == featureCategoryId && s.IsPackage == false)
                        .Select(s => s.ServiceId)
                        .ToListAsync();

                    var allTypes = await uow.ServiceTypesRepository.Get()
                        .Where(s => categoryServiceIds.Contains(s.ServiceId) && s.Status == "Active")
                        .OrderByDescending(s => s.ServiceId)
                        .ToListAsync();

                    foreach (var type in allTypes)
                    {
                        var ss = await GetSmallServices(type.ServiceId, type.ServiceTypeId, memberId, lang);
                        if (ss != null) listofservices.Add(ss);
                    }
                    return listofservices;
                }

            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<SmallServiceDetailsResponse> GetSmallServices(int serviceId, int serviceTypeId = 0, int memberid = 0, string lang = "en", List<int> serviceTypeIds = null)
        {
            try
            {
                var result = new SmallServiceDetailsResponse();

                var AreaFiltered = await uow.ServiceRepository.Get().Where(p => p.ServiceId == serviceId && p.Status == "Active").Include(s => s.City).Include(s => s.Country).Include(s => s.ServiceTypes).FirstOrDefaultAsync();
                if (AreaFiltered != null)
                {
                    result.ServiceId = AreaFiltered.ServiceId;
                    result.ServiceName = lang=="en"?AreaFiltered.Name:AreaFiltered.NameAr;
                    if (AreaFiltered.CityId.HasValue)
                    {
                        result.City = await new CityAccess().GetById(AreaFiltered.CityId.Value, lang);
                    }
                    if (AreaFiltered.CountryId.HasValue)
                    {
                        result.Country = await new CountryAccess().GetById(AreaFiltered.CountryId.Value, lang);
                    }
                
                    result.CheckInTime = AreaFiltered.CheckInTime;
                    result.CheckOutTime = AreaFiltered.CheckOutTime;
                    if (AreaFiltered.IsPackage)
                    {
                        result.ServiceAddress = AreaFiltered.Address;
                        result.Description = lang == "en" ? AreaFiltered.Description: AreaFiltered.DescriptionAr;
                        result.PackagePrice = AreaFiltered.Price;
                        result.DistanceKM = AreaFiltered.DistanceInKM;
                    }
                    if (AreaFiltered.ServiceTypes != null && AreaFiltered.ServiceTypes.Count() > 0)
                    {
                        result.Price = AreaFiltered.ServiceTypes.FirstOrDefault().Price;
                    }
                    result.Ratings = AreaFiltered.Ratings;
                    result.ReviewCount = await uow.UserReviewRepository.Get().Where(p => p.FeatureType.ToLower() == "service" && p.FeatureTypeId == AreaFiltered.ServiceId).CountAsync();
                    var imagelist = await uow.ServiceImageRepository.Get().Where(s => s.ServiceId == AreaFiltered.ServiceId).ToListAsync();

                    if (imagelist.Count() > 0 && imagelist != null)
                    {
                        foreach (var image in imagelist)
                        {
                            image.Service = null;
                            result.Images.Add(image);
                        }
                    }
                    result.Amenities = lang=="en"?await uow.ServiceAmenityRepository.Get().Where(s => s.ServiceId == AreaFiltered.ServiceId).Select(s => s.AmenitiesName).ToListAsync(): await uow.ServiceAmenityRepository.Get().Where(s => s.ServiceId == AreaFiltered.ServiceId).Select(s => s.AmenitiesNameAr).ToListAsync();
                    var discountinfo = await new OfferServiceAccess().ServiceDiscountCheck(serviceId, 0, AreaFiltered.FeatureCategoryId,lang);
                    if (discountinfo != null)
                    {
                        result.DiscountInformation = discountinfo;
                    }
                    if (serviceTypeId > 0)
                    {
                        result.ServiceType = await Getservicetype(serviceTypeId, AreaFiltered.FeatureCategoryId, memberid,lang);
                    }
                    else
                    {
                        result.ServiceTypeList = await GetAllservicetype(serviceId, AreaFiltered.FeatureCategoryId, memberid,lang,serviceTypeIds);
                    }
                }


                return result;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<ServiceType> Getservicetype(int serviceTypeId, int featureCategoryId, int memberId = 0,string lang="en")
        {
            try
            {
                var service = await uow.ServiceTypesRepository.Get().Where(s => s.Status == "Active" && s.ServiceTypeId == serviceTypeId).FirstOrDefaultAsync();
                if (service != null)
                {
                    service.ServiceTypeName = lang == "en" ? service.ServiceTypeName : service.ServiceTypeNameAr;
                    service.Description = lang == "en" ? service.Description : service.DescriptionAr;
                    service.Size = lang == "en" ? service.Size : service.SizeAr;
                    service.BigDescription = lang == "en" ? service.BigDescription : service.BigDescriptionAr;
                    service.Service = null;
                    service.AvgRatings = 0;
                    service.TotalReview = 0;
                    
                    if (!string.IsNullOrEmpty(service.Image))
                    {
                        service.ServiceTypeImages.Add(link + service.Image);
                        service.Image = link + service.Image;
                        
                    }
                        

                    var images = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == service.ServiceTypeId && s.FileType == "Image" && s.FileSource == "TypeImage").Select(s => s.FileLocation).ToListAsync();

                    if (images != null && images.Count() > 0)
                    {
                        foreach (var img in images)
                        {
                            service.ServiceTypeImages.Add(link + img);
                        }
                    }

                    var discountinfo = await new OfferServiceAccess().ServiceDiscountCheck(service.ServiceId, serviceTypeId, featureCategoryId,lang);
                    if (discountinfo != null)
                    {
                        service.DiscountInformation = discountinfo;
                        if (discountinfo.DiscountType.ToLower() == "percentage")
                        {
                            service.DiscountPrice = Convert.ToDouble(service.Price - ((service.Price * discountinfo.DiscountAmount) / 100));
                        }
                        else
                        {
                            service.DiscountPrice = Convert.ToDouble(service.Price - discountinfo.DiscountAmount);

                        }
                    }
                    else
                    {
                        if (memberId > 0)
                        {
                            var memberInfo = await new MemberServiceAccess().GetById(memberId);

                            if (memberInfo.IsAnyMembership == true && memberInfo.MemberSubscriptions != null)
                            {
                                var MembershipDiscount = await uow.MemberShipDiscountCategoryRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId && s.MemberShipId == memberInfo.MemberSubscriptions.MemberShipId).FirstOrDefaultAsync();
                                if (MembershipDiscount != null)
                                {
                                    service.MemberShipInformation = MembershipDiscount;
                                    if (MembershipDiscount.MemberShipDiscountType.ToLower() == "percentage")
                                    {
                                        service.DiscountPrice = Convert.ToDouble(service.Price - ((service.Price * Convert.ToDouble(MembershipDiscount.MemberShipDiscountAmount)) / 100));
                                    }
                                    else
                                    {
                                        service.DiscountPrice = Convert.ToDouble(service.Price - Convert.ToDouble(MembershipDiscount.MemberShipDiscountAmount));

                                    }
                                }


                            }

                        }
                    }
                    if (memberId > 0)
                    {
                        service.IsFavourite = await uow.FavouriteServiceRepository.Get().AnyAsync(s => s.ServiceId == service.ServiceId && s.ServiceTypeId == service.ServiceTypeId && s.MemberId == memberId);
                        service.CartQuantity = await uow.CartServiceRepository.Get()
                    .Where(x =>
                        x.MemberId == memberId &&
                        x.ServiceId == service.ServiceId &&
                        x.ServiceTypeId == service.ServiceTypeId).Select(s => s.Quantity).FirstOrDefaultAsync();
                    }
                    return service;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {

                return null;
            }
        }


        public async Task<List<ServiceType>> GetAllservicetype(int serviceId, int featureCategoryId, int memberId = 0,string lang="en",List<int> serviceTypeIds=null)
        {
            try
            {
                
                var services = new List<ServiceType>();
                if (serviceTypeIds == null)
                {
                    services = await uow.ServiceTypesRepository.Get().Where(s => s.Status == "Active" && s.ServiceId == serviceId).ToListAsync();
                }
                else
                {
                    services = await uow.ServiceTypesRepository.Get().Where(s => s.Status == "Active" && s.ServiceId == serviceId && serviceTypeIds.Contains(s.ServiceTypeId)).ToListAsync();
                }
               
                if (services != null && services.Count() > 0)
                {
                    foreach (var service in services)
                    {
                        service.ServiceTypeName = lang == "en" ? service.ServiceTypeName : service.ServiceTypeNameAr;
                        service.Description = lang == "en" ? service.Description : service.DescriptionAr;
                        service.Size = lang == "en" ? service.Size : service.SizeAr;
                        service.Service = null;
                        service.BigDescription = lang == "en" ? service.BigDescription : service.BigDescriptionAr;
                        service.AvgRatings = 0;
                        service.TotalReview = 0;
                        if (!string.IsNullOrEmpty(service.Image))
                        {
                            service.ServiceTypeImages.Add(link + service.Image);
                            service.Image = link + service.Image;
                            
                        }
                         

                        var images = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == service.ServiceTypeId && s.FileType == "Image" && s.FileSource == "TypeImage").Select(s => s.FileLocation).ToListAsync();

                        if (images != null && images.Count() > 0)
                        {
                            foreach (var img in images)
                            {
                                service.ServiceTypeImages.Add(link + img);
                            }
                        }

                        var discountinfo = await new OfferServiceAccess().ServiceDiscountCheck(service.ServiceId, service.ServiceTypeId, featureCategoryId, lang);
                        if (discountinfo != null)
                        {
                            service.DiscountInformation = discountinfo;
                            if (discountinfo.DiscountType.ToLower() == "percentage")
                            {
                                service.DiscountPrice = Convert.ToDouble(service.Price - ((service.Price * discountinfo.DiscountAmount) / 100));
                            }
                            else
                            {
                                service.DiscountPrice = Convert.ToDouble(service.Price - discountinfo.DiscountAmount);

                            }
                        }
                        else
                        {
                            if (memberId > 0)
                            {
                                var memberInfo = await new MemberServiceAccess().GetById(memberId);
                                if (memberInfo != null)
                                {
                                    if (memberInfo.IsAnyMembership == true && memberInfo.MemberSubscriptions != null)
                                    {
                                        var MembershipDiscount = await uow.MemberShipDiscountCategoryRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId && s.MemberShipId == memberInfo.MemberSubscriptions.MemberShipId).FirstOrDefaultAsync();
                                        if (MembershipDiscount != null)
                                        {
                                            service.MemberShipInformation = MembershipDiscount;
                                            if (MembershipDiscount.MemberShipDiscountType.ToLower() == "percentage")
                                            {
                                                service.DiscountPrice = Convert.ToDouble(service.Price - ((service.Price * Convert.ToDouble(MembershipDiscount.MemberShipDiscountAmount)) / 100));
                                            }
                                            else
                                            {
                                                service.DiscountPrice = Convert.ToDouble(service.Price - Convert.ToDouble(MembershipDiscount.MemberShipDiscountAmount));

                                            }
                                        }


                                    }
                                }
                               

                            }
                        }
                        if (memberId > 0)
                        {
                            service.IsFavourite = await uow.FavouriteServiceRepository.Get().AnyAsync(s => s.ServiceId == service.ServiceId && s.ServiceTypeId == service.ServiceTypeId && s.MemberId == memberId);
                            service.CartQuantity = await uow.CartServiceRepository.Get()
                   .Where(x =>
                       x.MemberId == memberId &&
                       x.ServiceId == service.ServiceId &&
                       x.ServiceTypeId == service.ServiceTypeId).Select(s => s.Quantity).FirstOrDefaultAsync();
                    
                    }
                    }
                    return services;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {

                return null;
            }
        }


        public async Task<SmallServiceDetailsResponse> GetSmallRealEStateServices(int serviceId, int serviceTypeId = 0, int memberid = 0,string lang="en")
        {
            try
            {
                var result = new SmallServiceDetailsResponse();

                var AreaFiltered = await uow.ServiceRepository.Get().Where(p => p.ServiceId == serviceId && p.Status == "Active").Include(s => s.City).Include(s => s.Country).Include(s => s.ServiceTypes).FirstOrDefaultAsync();
                if (AreaFiltered != null)
                {
                    result.ServiceId = AreaFiltered.ServiceId;
                    result.ServiceName = lang == "en" ? AreaFiltered.Name : AreaFiltered.NameAr;
                    result.City = await new CityAccess().GetById(AreaFiltered.CityId.Value, lang);
                    result.Country = await new CountryAccess().GetById(AreaFiltered.CountryId.Value, lang);
                    if (AreaFiltered.IsPackage)
                    {
                        result.ServiceAddress = AreaFiltered.Address;
                        result.Description = lang == "en" ? AreaFiltered.Description:AreaFiltered.DescriptionAr;
                        result.PackagePrice = AreaFiltered.Price;
                        result.DistanceKM = AreaFiltered.DistanceInKM;
                    }
                    if (AreaFiltered.ServiceTypes != null && AreaFiltered.ServiceTypes.Count() > 0)
                    {
                        result.Price = AreaFiltered.ServiceTypes.FirstOrDefault().Price;
                    }
                    result.Ratings = AreaFiltered.Ratings;
                    result.ReviewCount = await uow.UserReviewRepository.Get().Where(p => p.FeatureType.ToLower() == "service" && p.FeatureTypeId == AreaFiltered.ServiceId).CountAsync();
                    var imagelist = await uow.ServiceImageRepository.Get().Where(s => s.ServiceId == AreaFiltered.ServiceId).ToListAsync();

                    if (imagelist.Count() > 0 && imagelist != null)
                    {
                        foreach (var image in imagelist)
                        {
                            image.Service = null;
                            result.Images.Add(image);
                        }
                    }
                    result.Amenities = lang == "en" ? await uow.ServiceAmenityRepository.Get().Where(s => s.ServiceId == AreaFiltered.ServiceId).Select(s => s.AmenitiesName).ToListAsync(): await uow.ServiceAmenityRepository.Get().Where(s => s.ServiceId == AreaFiltered.ServiceId).Select(s => s.AmenitiesNameAr).ToListAsync();
                    //var discountinfo = await new OfferServiceAccess().ServiceDiscountCheck(serviceId, 0, AreaFiltered.FeatureCategoryId);
                    //if (discountinfo != null)
                    //{
                    //    result.DiscountInformation = discountinfo;
                    //}
                    if (serviceTypeId > 0)
                    {
                        result.ServiceType = await GetservicetypeRealEstate(serviceTypeId, AreaFiltered.FeatureCategoryId, memberid,lang);
                    }
                    //else
                    //{
                    //    result.ServiceTypeList = await GetAllservicetype(serviceId, AreaFiltered.FeatureCategoryId, memberid);
                    //}
                }


                return result;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<ServiceType> GetservicetypeRealEstate(int serviceTypeId, int featureCategoryId, int memberId = 0,string lang="en")
        {
            try
            {
                var service = await uow.ServiceTypesRepository.Get().Where(s => s.Status == "Active" && s.ServiceTypeId == serviceTypeId).FirstOrDefaultAsync();
                if (service != null)
                {
                    service.ServiceTypeName = lang == "en" ? service.ServiceTypeName : service.ServiceTypeNameAr;
                    service.Description = lang == "en" ? service.Description : service.DescriptionAr;
                    service.Size = lang == "en" ? service.Size : service.SizeAr;
                    service.City = await new CityAccess().GetById(service.CityId.Value, lang);
                    service.Country = await new CountryAccess().GetById(service.CountryId.Value, lang);
                
                    service.Service = null;
                    if (!string.IsNullOrEmpty(service.Image))
                        service.Image = link + service.Image;

                    var serviceTypeImages = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.FileSource == "TypeImage").ToListAsync();

                    if (serviceTypeImages != null && serviceTypeImages.Count > 0)
                    {
                        foreach (var img in serviceTypeImages)
                        {
                            img.ServiceType = null;
                            if (!string.IsNullOrEmpty(img.FileLocation))
                            {
                                service.ServiceTypeImages.Add(link + img.FileLocation);
                            }
                        }
                    }
                    if (memberId > 0)
                    {
                        service.IsFavourite = await uow.FavouriteServiceRepository.Get().AnyAsync(s => s.ServiceId == service.ServiceId && s.ServiceTypeId == service.ServiceTypeId && s.MemberId == memberId);
                    }
                    return service;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {

                return null;
            }
        }

        // Real estateIntegration 


        public async Task<List<SmallServiceDetailsResponse>> GetLatestprojectFromrealEstate(int memberid,int size,int count,int featureCategoryId,string lang="en")
        {
            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var listofservices = new List<SmallServiceDetailsResponse>();
                var resultList =new List<ServiceType>();

                var serviceids = await uow.ServiceRepository.Get().Where(p => p.Status=="Active" && p.FeatureCategoryId== featureCategoryId).Select(s=>s.ServiceId).ToListAsync();

                if (serviceids != null && serviceids.Count() > 0)
                {
                    resultList = await uow.ServiceTypesRepository.Get().Where(s => serviceids.Contains(s.ServiceId) && s.Status == "Active" && s.IsPackage==true).OrderByDescending(s=>s.ServiceTypeId).Skip(count).Take(size).ToListAsync();
                }

                
                if (resultList != null && resultList.Count() > 0)
                {





                    foreach (var item in resultList)
                    {

                        var ss = await GetSmallRealEStateServices(item.ServiceId,item.ServiceTypeId,memberid,lang);
                        if (ss != null)
                        {

                            listofservices.Add(ss);
                        }



                    }

                }


                return listofservices;
            }
            catch (Exception ex)
            {
               
                return null;
            }
        }

        public async Task<List<SmallServiceDetailsResponse>> GetLocationFromrealEstate(int memberid, int size, int count, int featureCategoryId, string lang = "en")
        {
            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var listofservices = new List<SmallServiceDetailsResponse>();
                var resultList = new List<ServiceType>();

                var serviceids = await uow.ServiceRepository.Get().Where(p => p.Status == "Active" && p.FeatureCategoryId == featureCategoryId).Select(s => s.ServiceId).ToListAsync();

                if (serviceids != null && serviceids.Count() > 0)
                {
                    resultList = await uow.ServiceTypesRepository.Get().Where(s => serviceids.Contains(s.ServiceId) && s.Status == "Active" && s.IsPackage==false).OrderBy(s => s.ServiceTypeId).Skip(count).Take(size).ToListAsync();
                }


                if (resultList != null && resultList.Count() > 0)
                {





                    foreach (var item in resultList)
                    {

                        var ss = await GetSmallRealEStateServices(item.ServiceId, item.ServiceTypeId, memberid, lang);
                        if (ss != null)
                        {

                            listofservices.Add(ss);
                        }



                    }

                }


                return listofservices;
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public async Task<List<SmallServiceDetailsResponse>> GetFeatureFromrealEstate(int memberid, int size, int count, int featureCategoryId, string lang = "en")
        {
            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var listofservices = new List<SmallServiceDetailsResponse>();
                var resultList = new List<ServiceType>();

                var serviceids = await uow.ServiceRepository.Get().Where(p => p.Status == "Active" &&  p.FeatureCategoryId == featureCategoryId).Select(s => s.ServiceId).ToListAsync();

                if (serviceids != null && serviceids.Count() > 0)
                {
                    var rand = new Random();
                    var ssresultList = await uow.ServiceTypesRepository.Get().Where(s => serviceids.Contains(s.ServiceId) && s.Status == "Active" && s.IsPackage==false).OrderBy(s => s.ServiceTypeId).Skip(count).Take(size).ToListAsync();

                    resultList = ssresultList.OrderBy(x => Guid.NewGuid()).ToList();
                }


                if (resultList != null && resultList.Count() > 0)
                {





                    foreach (var item in resultList)
                    {

                        var ss = await GetSmallRealEStateServices(item.ServiceId, item.ServiceTypeId, memberid, lang);
                        if (ss != null)
                        {

                            listofservices.Add(ss);
                        }



                    }

                }


                return listofservices;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<BigServiceDetailResponse> GetServiceDetailsrealStateById(int serviceTypeId, int memberId = 0, string lang = "en")
        {
            try
            {
                var result = new BigServiceDetailResponse();
                var serviceTypes = await uow.ServiceTypesRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId).FirstOrDefaultAsync();
                if (serviceTypes != null)
                {

                    var Service = await uow.ServiceRepository.Get().Where(p => p.ServiceId == serviceTypes.ServiceId).Include(p => p.FeatureCategory).Include(p => p.ServiceTypes).Include(p => p.ServiceImages).Include(p => p.Country).Include(p => p.City).FirstOrDefaultAsync();
                    if (Service != null)
                    {
                        result.ServiceId = Service.ServiceId;
                        result.Name = lang == "en" ? Service.Name : Service.NameAr;
                        
                        result.Aboutus = lang == "en" ? Service.AboutUs:Service.AboutUsAr;
                        if (Service.City != null)
                        {
                            result.City = await new CityAccess().GetById(Service.CityId.Value, lang); ;
                        }
                        if (Service.Country != null)
                        {
                            result.Country = await new CountryAccess().GetById(Service.CountryId.Value, lang);
                        }
                        result.Address = Service.Address;

                        if (Service.ServiceImages != null && Service.ServiceImages.Count > 0)
                        {
                            foreach (var img in Service.ServiceImages)
                            {
                                img.Service = null;
                                if (!string.IsNullOrEmpty(img.Image))
                                {
                                    result.ServiceImageURLs.Add(link + img.Image);
                                }
                            }
                        }
                        if (Service.FeatureCategory != null)
                        {
                            result.FeatureCategory = Service.FeatureCategory;
                        }

                        //Service Types 

                        result.ServiceType.ServiceTypeId = serviceTypes.ServiceTypeId;
                        result.ServiceType.ServiceTypeName = lang=="en"?serviceTypes.ServiceTypeName: serviceTypes.ServiceTypeNameAr;
                        result.ServiceType.BedQuantity = serviceTypes.AdultQuantity;
                        result.ServiceType.BathroomQuantity = serviceTypes.ChildrenQuantity;
                        result.ServiceType.Latitute = serviceTypes.Latitute;
                        result.ServiceType.Logitute = serviceTypes.Logitute;
                        result.ServiceType.Description = lang == "en" ? serviceTypes.Description: serviceTypes.DescriptionAr;
                        result.ServiceType.Size = lang == "en" ? serviceTypes.Size: serviceTypes.SizeAr;

                        if (serviceTypes.City != null)
                        {
                            result.ServiceType.City = await new CityAccess().GetById(serviceTypes.CityId.Value, lang); ;
                        }
                        if (serviceTypes.Country != null)
                        {
                            result.ServiceType.Country = await new CountryAccess().GetById(serviceTypes.CountryId.Value, lang);
                        }
                        result.ServiceType.Address = serviceTypes.Address;
                        if (!string.IsNullOrEmpty(serviceTypes.Image))
                        {
                            result.ServiceType.MainImage = link+serviceTypes.Image;
                        }

                        result.ServiceType.PaymentType = serviceTypes.PaymentType;
                        result.ServiceType.Price = serviceTypes.Price;

                        if (memberId > 0)
                        {
                            result.ServiceType.IsFavourite = await uow.FavouriteServiceRepository.Get().AnyAsync(s => s.ServiceId == Service.ServiceId && s.ServiceTypeId == serviceTypes.ServiceTypeId && s.MemberId == memberId);
                        }
                        var serviceTypeImages = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.FileSource == "TypeImage").ToListAsync();

                        if (serviceTypeImages != null && serviceTypeImages.Count > 0)
                        {
                            foreach (var img in serviceTypeImages)
                            {
                                img.ServiceType = null;
                                if (!string.IsNullOrEmpty(img.FileLocation))
                                {
                                    result.ServiceType.ServiceTypeImages.Add(link + img.FileLocation);
                                }
                            }
                        }


                        var ProjectPlans = await GetTitleWisePlans(serviceTypeId);

                        if (ProjectPlans != null && ProjectPlans.Count > 0)
                        {
                           
                          result.ServiceType.NewProjectPlan.AddRange(ProjectPlans);
                                
                        }

                        var ProVideos = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.FileSource == "TypeVideo").ToListAsync();

                        if (ProVideos != null && ProVideos.Count > 0)
                        {
                            foreach (var img in ProVideos)
                            {
                                img.ServiceType = null;
                                if (!string.IsNullOrEmpty(img.FileLocation))
                                {
                                    result.ServiceType.ProjectVideos.Add(link + img.FileLocation);
                                }
                            }
                        }

                        var property = await uow.PropertyInformationRepository.Get().Where(s => s.ServiceTypeId == serviceTypes.ServiceTypeId).Include(s=>s.City).Include(s=>s.Country).FirstOrDefaultAsync();
                        if (property!=null)
                        {
                            property.ServiceType = null;
                        
                            var exteriorfiles = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == serviceTypes.ServiceTypeId && s.FileSource == "Exterior").ToListAsync();
                            if (exteriorfiles != null && exteriorfiles.Count() > 0)
                            {
                                foreach (var img in exteriorfiles)
                                {
                                    img.ServiceType = null;
                                    if (!string.IsNullOrEmpty(img.FileLocation))
                                    {
                                        property.ExteriorsImage.Add( link + img.FileLocation);
                                    }
                                }
                            }

                            var interiorfiles = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == serviceTypes.ServiceTypeId && s.FileSource == "Interior").ToListAsync();
                            if (interiorfiles != null && interiorfiles.Count() > 0)
                            {
                                foreach (var img in interiorfiles)
                                {
                                    img.ServiceType = null;
                                    if (!string.IsNullOrEmpty(img.FileLocation))
                                    {
                                        property.InteriorsImage.Add(link + img.FileLocation);
                                    }
                                }
                            }
                            result.ServiceType.PropertyInformation = property;
                        }

                        var closerThings = await uow.ServiceTypeAmenityRepository.Get().Where(s => s.ServiceTypeId == serviceTypes.ServiceTypeId && s.AmenitiesType == "CloserProperty" && s.Status == "Active").ToListAsync();

                            if (closerThings != null && closerThings.Count > 0)
                            {
                            foreach (var img in closerThings)
                            {
                                img.ServiceType = null;
                                if (!string.IsNullOrEmpty(img.AmenitiesLogo))
                                {
                                   img.AmenitiesLogo = link + img.AmenitiesLogo;
                                }
                            }
                            result.ServiceType.CloserFacilities = closerThings;
                        }


                        var meterials = await uow.ServiceTypeAmenityRepository.Get().Where(s => s.ServiceTypeId == serviceTypes.ServiceTypeId && s.AmenitiesType == "Materials" && s.Status=="Active").ToListAsync();

                        if (meterials != null && meterials.Count > 0)
                        {
                           
                            foreach (var img in meterials)
                            {
                                img.ServiceType = null;
                                if (!string.IsNullOrEmpty(img.AmenitiesLogo))
                                {
                                    img.AmenitiesLogo = link + img.AmenitiesLogo;
                                }

                                if (img.LinkedWithFile == true)
                                {
                                    var files = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == serviceTypes.ServiceTypeId && s.FileSource == img.AmenitiesName && s.ServiceAmenityId == img.ServiceAmenityId).FirstOrDefaultAsync();
                                    if (files != null) {

                                        if (!string.IsNullOrEmpty(files.FileLocation))
                                        {
                                            img.FileLink = link + files.FileLocation;
                                        }
                                        img.FileType = files.FileType;
                                    }
                                    
                                }
                            }
                            result.ServiceType.ProjectMaterials = meterials;
                        }

                        var utilities = await uow.ServiceTypeAmenityRepository.Get().Where(s => s.ServiceTypeId == serviceTypes.ServiceTypeId && s.AmenitiesType == "Utility" && s.Status == "Active").ToListAsync();

                        if (utilities != null && utilities.Count > 0)
                        {
                            foreach (var img in utilities)
                            {
                                img.ServiceType = null;
                                if (!string.IsNullOrEmpty(img.AmenitiesLogo))
                                {
                                    img.AmenitiesLogo = link + img.AmenitiesLogo;
                                }
                            }
                            result.ServiceType.ProjectUtilities = utilities;
                        }

                        var Amenities = await uow.ServiceTypeAmenityRepository.Get().Where(s => s.ServiceTypeId == serviceTypes.ServiceTypeId && s.AmenitiesType == "Amenity" && s.Status == "Active").ToListAsync();

                        if (Amenities != null && Amenities.Count > 0)
                        {
                            foreach (var img in Amenities)
                            {
                                img.ServiceType = null;
                                if (!string.IsNullOrEmpty(img.AmenitiesLogo))
                                {
                                    img.AmenitiesLogo = link + img.AmenitiesLogo;
                                }
                            }
                            result.ServiceType.Amenities = Amenities;
                        }


                        var ProContracts = await uow.ServiceTypeFileRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.FileSource == "Contract").ToListAsync();

                        if (ProContracts != null && ProContracts.Count > 0)
                        {
                            foreach (var img in ProContracts)
                            {
                                img.ServiceType = null;
                                if (!string.IsNullOrEmpty(img.FileLocation))
                                {
                                    result.ServiceType.Contracts.Add(link + img.FileLocation);
                                }
                            }
                        }

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
               
                return null;
            }
        }


        public async Task<List<ProjectPlanByTitleResponse>> GetTitleWisePlans(int serviceTypeId)
        {
            try
            {
                var result = await uow.ProjectPlanRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId)

                .GroupBy(x => x.Title)

                .Select(g => new ProjectPlanByTitleResponse
                {
                    Title = g.Key,
                    Plans = g.Select(x => new ProjectPlanResponseDto
                    {
                        ProjectPlanId = x.ProjectPlanId,
                        ServiceTypeId = x.ServiceTypeId,

                        UniteType = x.UniteType,
                        Image = !string.IsNullOrEmpty(x.Image) ? link + x.Image : null
                    }).ToList()
                })
                .ToListAsync();

                return result;
            }
            catch (Exception)
            {

                return null;
            }

           
        }

        public async Task<ServiceFilterResponse> GetFilterResponse(int featureCategoryId,string lang="en")
        {
            try
            {
                var result = new ServiceFilterResponse();
                var categories = await new CategoryServiceAccess().GetParentChildWiseCategories(featureCategoryId, "All", false, false,lang);
                if (categories != null && categories.Count() > 0)
                {
                    result.Categories = categories;
                }

                var serviceIds = await uow.ServiceRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId).Select(s => s.ServiceId).ToListAsync();

                if (serviceIds != null && serviceIds.Count() > 0)
                {
                    var servicetypeIds = await uow.ServiceTypesRepository.Get().Where(s => serviceIds.Contains(s.ServiceId)).Select(s => s.ServiceTypeId).ToListAsync();

                    if (servicetypeIds != null && servicetypeIds.Count() > 0)
                    {
                        var propertyTypes = await uow.PropertyInformationRepository.Get().Where(s=>servicetypeIds.Contains(s.ServiceTypeId.Value)).Select(s=>s.Type).Distinct().ToListAsync();

                        if (propertyTypes != null && propertyTypes.Count() > 0)
                        {
                            result.PropertyType = propertyTypes;
                        }

                        var priceType = await uow.ServiceTypesRepository.Get().Where(s => servicetypeIds.Contains(s.ServiceTypeId)).Select(s => s.PaymentType).Distinct().ToListAsync();


                        if (priceType != null && priceType.Count() > 0)
                        {
                            result.PricePeriod = priceType;
                        }

                        var priceList  = await uow.ServiceTypesRepository.Get().Where(s => servicetypeIds.Contains(s.ServiceTypeId)).Select(s => s.Price).Distinct().ToListAsync();
                        if (priceList != null && priceList.Count() > 0)
                        {
                            result.Maxprice = priceList.Max();
                            result.Minprice = 0;
                        }

                        var BedRooms = await uow.ServiceTypesRepository.Get().Where(s => servicetypeIds.Contains(s.ServiceTypeId)).Select(s => s.AdultQuantity).Distinct().ToListAsync();
                        if (BedRooms != null && BedRooms.Count() > 0)
                        {
                            var maxBedRooms = BedRooms.Max();
                           result.BedRooms = Enumerable.Range(1, maxBedRooms).ToList();
                        }

                        var bathrooms = await uow.ServiceTypesRepository.Get().Where(s => servicetypeIds.Contains(s.ServiceTypeId)).Select(s => s.ChildrenQuantity).Distinct().ToListAsync();
                        if (bathrooms != null && bathrooms.Count() > 0)
                        {
                            var maxbathRooms = bathrooms.Max();
                            result.BathRooms = Enumerable.Range(1, maxbathRooms).ToList();
                        }

                        var Furnishing = await uow.PropertyInformationRepository.Get().Where(s => servicetypeIds.Contains(s.ServiceTypeId.Value)).Select(s => s.Furnishing).Distinct().ToListAsync();

                        if (Furnishing != null && Furnishing.Count() > 0)
                        {
                            result.Furnishing = Furnishing;
                        }


                        var Sizes = await uow.ServiceTypesRepository.Get().Where(s => servicetypeIds.Contains(s.ServiceTypeId)).Select(s => s.Size).Distinct().ToListAsync();
                        if (Sizes != null && Sizes.Count() > 0)
                        {
                            var sizeInt = new List<int>();

                            foreach (var ss in Sizes)
                            {
                                
                                sizeInt.Add(int.Parse(Regex.Match(ss, @"\d+").Value));
                            }
                            if (sizeInt != null && sizeInt.Count() > 0)
                            {
                                result.MaxSize = sizeInt.Max();
                                result.MinSize = sizeInt.Min();
                            }
                          
                        }
                       
                        var Amenity = await uow.ServiceTypeAmenityRepository.Get().Where(s => servicetypeIds.Contains(s.ServiceTypeId.Value) && s.AmenitiesType == "Amenity" && s.Status == "Active").Include(s=>s.ServiceType).ToListAsync();

                        if (Amenity != null && Amenity.Count > 0)
                        {
                            foreach (var am in Amenity)
                            {
                                am.AmenitiesName = lang=="en"?am.AmenitiesName:am.AmenitiesNameAr;
                                
                                am.ServiceType = null;
                                if (!string.IsNullOrEmpty(am.AmenitiesLogo))
                                {
                                    am.AmenitiesLogo = link + am.AmenitiesLogo;
                                }
                                am.ServiceType = null;
                               
                            }
                            //result.Amenities = new List<ServiceTypeAmenity>();
                            result.Amenities = Amenity;
                        }
                    }
                }
                return result;

            }
            catch (Exception ex)
            {

                return new ServiceFilterResponse();
            }
        }


        public async Task<List<SmallServiceDetailsResponse>> GetFilterwiseRealstateService(FilterBodyRequest body)
        {

            try
            {
                var Result = new List<SmallServiceDetailsResponse>();
                if (body.Count > 0)
                {
                    body.Count = body.Count * body.Size;
                }
                var serviceTypeids = new List<int>();

                var categories = await uow.CategoryRepository.Get().Where(s => s.Status == "Active" && s.CategoryId == body.CategoryId).FirstOrDefaultAsync();


                if (categories != null)
                {

                    var ids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                    if (ids != null && ids.Count() > 0)
                    {
                        serviceTypeids = ids.Where(id => id.HasValue).Select(id => id.Value).ToList();
                        if(body.PropertyType!=null && body.PropertyType.Count()>0)
                        {
                            var property = await uow.PropertyInformationRepository.Get().Where(s=> body.PropertyType.Contains(s.Type) && serviceTypeids.Contains(s.ServiceTypeId.Value)).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                            if (property != null && property.Count() > 0)
                            {
                                 serviceTypeids = new List<int>();
                                serviceTypeids.AddRange(property.Where(id => id.HasValue).Select(id => id.Value));
                            }
                        }


                        if (body.PricePeriod != null && body.PricePeriod.Count() > 0)
                        {
                            var priceType = await uow.ServiceTypesRepository.Get().Where(s => body.PricePeriod.Contains(s.PaymentType) && serviceTypeids.Contains(s.ServiceTypeId) && s.IsPackage == false).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                            if (priceType != null && priceType.Count() > 0)
                            {
                                serviceTypeids = new List<int>();
                                serviceTypeids.AddRange(priceType);
                            }
                        }
                        if (body.Maxprice > 0)
                        {
                            var maxp = await uow.ServiceTypesRepository.Get().Where(s => s.Price<= body.Maxprice && s.Price>=body.Minprice && serviceTypeids.Contains(s.ServiceTypeId) && s.IsPackage == false).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                            if (maxp != null && maxp.Count() > 0)
                            {
                                serviceTypeids = new List<int>();
                                serviceTypeids.AddRange(maxp);
                            }
                        }



                        if (body.BedRooms != null && body.BedRooms.Count() > 0)
                        {
                            var bed = await uow.ServiceTypesRepository.Get().Where(s => body.BedRooms.Contains(s.AdultQuantity) && serviceTypeids.Contains(s.ServiceTypeId) && s.IsPackage == false).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                            if (bed != null && bed.Count() > 0)
                            {
                                serviceTypeids = new List<int>();
                                serviceTypeids.AddRange(bed);
                            }
                        }

                        if (body.BathRooms != null && body.BathRooms.Count() > 0)
                        {
                            var bath = await uow.ServiceTypesRepository.Get().Where(s => body.BathRooms.Contains(s.ChildrenQuantity) && serviceTypeids.Contains(s.ServiceTypeId) && s.IsPackage == false).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                            if (bath != null && bath.Count() > 0)
                            {
                                serviceTypeids = new List<int>();
                                serviceTypeids.AddRange(bath);
                            }
                        }


                        if (body.Furnishing != null && body.Furnishing.Count() > 0)
                        {
                            var furninsh = await uow.PropertyInformationRepository.Get().Where(s => body.Furnishing.Contains(s.Furnishing) && serviceTypeids.Contains(s.ServiceTypeId.Value)).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                            if (furninsh != null && furninsh.Count() > 0)
                            {
                                serviceTypeids = new List<int>();
                                serviceTypeids.AddRange(furninsh.Where(id => id.HasValue).Select(id => id.Value));
                            }
                        }

                        if (body.Amenityids != null && body.Amenityids.Count() > 0)
                        {
                            var amenity = await uow.ServiceTypeAmenityRepository.Get().Where(s => body.Amenityids.Contains(s.ServiceAmenityId) && serviceTypeids.Contains(s.ServiceTypeId.Value)).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                            if (amenity != null && amenity.Count() > 0)
                            {
                                serviceTypeids = new List<int>();
                                serviceTypeids.AddRange(amenity.Where(id => id.HasValue).Select(id => id.Value));
                            }
                        }

                        //if (body.MaxSize > 0)
                        //{
                        //    var size = await uow.ServiceTypesRepository.Get().Where(s => int.Parse(Regex.Match(s.Size, @"(\d+)").Value) >= body.MinSize && int.Parse(Regex.Match(s.Size, @"(\d+)").Value) <= body.MaxSize).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                        //    if (size != null && size.Count() > 0)
                        //    {
                        //        serviceTypeids.AddRange(size);
                        //    }
                        //}

                        if (!string.IsNullOrEmpty(body.Keyword))
                        {
                            var searchWords = body.Keyword.ToLower().Split(" ".ToCharArray(),
                                StringSplitOptions.RemoveEmptyEntries);


                            
                                var result = await uow.ServiceTypesRepository.Get().Where(s => searchWords.Any(t => s.ServiceTypeName.ToLower().Contains(t)) && ids.Contains(s.ServiceTypeId) && s.Status == "Active" && s.IsPackage == false).OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId).ToListAsync();
                            if (result != null && result.Count() > 0)
                            {
                                serviceTypeids = new List<int>();
                                serviceTypeids.AddRange(result);
                            }

                            
                        }
                        //else
                        //{
                        //    serviceTypeids = await uow.ServiceCategoryRepository.Get().Where(s => s.CategoryId == categories.CategoryId && s.Status == "Active").OrderByDescending(s => s.ServiceTypeId).Select(s => s.ServiceTypeId.Value).Skip(body.Count).Take(body.Size).ToListAsync();

                        //}
                    }

                    if (serviceTypeids.Count() > 0 || serviceTypeids != null)
                    {
                        foreach (var serviceTypeId in serviceTypeids.Distinct().Skip(body.Count).Take(body.Size).ToList())
                        {
                            var serviceType = await uow.ServiceTypesRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.Status == "Active" && s.IsPackage == false).FirstOrDefaultAsync();
                            if (serviceType != null)
                            {
                                var ServiceResult = await new ServiceAccess().GetSmallRealEStateServices(serviceType.ServiceId, serviceType.ServiceTypeId, body.MemberId,body.lang);
                                if (ServiceResult != null)
                                {
                                   Result.Add(ServiceResult);
                                }
                            }
                        }
                    }


                    return Result;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public async Task<List<ServiceAmenity>> GetServiceAmenities(int featureCategoryId,string lang="en")
        {

            try
            {
                var result = new List<ServiceAmenity>();

                var serviceam = await uow.ServiceRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId && s.Status == "Active").Select(s => s.ServiceId).ToListAsync();
                if (serviceam != null && serviceam.Count() > 0)
                {
                    var amenities = await uow.ServiceAmenityRepository.Get().Where(s => serviceam.Contains(s.ServiceId)).ToListAsync();

                    if (amenities != null && amenities.Count() > 0)
                    {
                        foreach (var am in amenities)
                        {
                            var res = new ServiceAmenity();
                            res.ServiceAmenityId = am.ServiceAmenityId;
                            res.AmenitiesName = lang == "en" ? am.AmenitiesName : am.AmenitiesNameAr;
                            if (!string.IsNullOrEmpty(am.AmenitiesLogo))
                            {
                                res.AmenitiesLogo = link + am.AmenitiesLogo;
                            }

                            result.Add(res);

                        }
                    }
                }
                return result;

            }
            catch (Exception ex)
            {

                return null;
            }
        }


        public async Task<List<SmallServiceByName>> GetServiceNameTypingAndInsurance(int featureCategoryId, string lang = "en")
        {

            try
            {
                var result = new List<SmallServiceByName>();

                var serviceam = await uow.ServiceRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId && s.Status == "Active" && s.ParentId==0).Take(10).ToListAsync();
                if (serviceam != null && serviceam.Count() > 0)
                {
                   

                    
                        foreach (var am in serviceam)
                        {
                            var res = new SmallServiceByName();
                            res.ServiceId = am.ServiceId;
                            res.Title = lang == "en" ? am.Name : am.NameAr;
                            

                            result.Add(res);

                        }
                    
                }
                return result;

            }
            catch (Exception ex)
            {

                return null;
            }
        }

    }
}