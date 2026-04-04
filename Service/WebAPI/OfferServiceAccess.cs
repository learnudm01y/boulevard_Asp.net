using Boulevard.BaseRepository;
using Boulevard.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages;
using Boulevard.Helper;
using Swashbuckle.Swagger;
using System.Web.Services.Description;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace Boulevard.Service.WebAPI
{
    public class OfferServiceAccess
    {

        public IUnitOfWork uow;
        public OfferServiceAccess()
        {
            uow = new UnitOfWork();
        }
        static DateTime dubaitime = DateTimeHelper.DubaiTime();
        static string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
        public async Task<OfferDiscount> ProductDiscountCheck(int productId,int featturecategoryId, string lang = "en")
        {
            try
            {
                double discountAmount = 0.0;
                await updateAllOfferAvaibality();
                var offers = new List<OfferInformation>();
                if (featturecategoryId > 0)
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.FeatureCategoryId == featturecategoryId && p.IsTimeLimit == false && p.FeatureType == "Product").ToListAsync();
                }
                else
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active"  && p.IsTimeLimit == false && p.FeatureType == "Product").ToListAsync();
                }

                //var timeOffers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.FeatureCategoryId == featturecategoryId && p.IsTimeLimit == true && p.StartDate<dubaitime && p.EndDate> dubaitime && p.FeatureType == "Product").ToListAsync();
                //if (timeOffers != null && timeOffers.Count() > 0)
                //{
                //    offers.AddRange(timeOffers);
                //}
                if (offers != null && offers.Count() > 0)
                {
                    foreach (var offer in offers)
                    {
                        if (offer.IsBrand == true && offer.IsProduct==false && offer.IsCategory==false)
                        {
                            var brands = await uow.BrandOfferRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).Select(s => s.BrandId).ToListAsync();
                            if (brands != null && brands.Count() > 0)
                            {
                                var productAmount = await uow.ProductRepository.Get().AnyAsync(s => s.ProductId == productId && brands.Contains(s.BrandId.Value));
                                if (productAmount ==true)
                                {
                                    var discount = await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).FirstOrDefaultAsync();
                                    if (discount != null)
                                    {
                                        return discount;
                                        //if (discount.DiscountType.ToLower() == "Percentage")
                                        //{
                                        //    discountAmount = Convert.ToDouble(productAmount - ((productAmount * discount.DiscountAmount) / 100));
                                        //}
                                        //else
                                        //{
                                        //    discountAmount = Convert.ToDouble(productAmount - discount.DiscountAmount);
                                        //    return discountAmount;
                                        //}
                                    }
                                }
                            }
                        }
                        else if (offer.IsBrand == false && offer.IsProduct == true && offer.IsCategory == false)
                        {
                            var products = await uow.ProductOfferRepository.Get().AnyAsync(s => s.OfferInformationId == offer.OfferInformationId && s.ProductId == productId);
                            if (products == true)
                            {
                              
                                    var discount = await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).FirstOrDefaultAsync();
                                    if (discount != null)
                                    {
                                        return discount;
                                        //if (discount.DiscountType.ToLower() == "Percentage")
                                        //{
                                        //    discountAmount = Convert.ToDouble(productAmount - ((productAmount * discount.DiscountAmount) / 100));
                                        //    return discountAmount;
                                        //}
                                        //else
                                        //{
                                        //    discountAmount = Convert.ToDouble(productAmount - discount.DiscountAmount);
                                        //    return discountAmount;
                                        //}
                                    }
                                
                            }
                        }

                        else if (offer.IsCategory == true && offer.IsBrand == false && offer.IsProduct == false)
                        {
                            var categories = await uow.CategoryOfferRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId ).Select(s=>s.CategoryId).ToListAsync();
                            if (categories != null && categories.Count() > 0)
                            {
                                var products = await uow.ProductCategoryRepository.Get().AnyAsync(s => s.ProductId == productId && categories.Contains(s.CategoryId));


                            if (products == true)
                                {
                                    
                                        var discount = await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).FirstOrDefaultAsync();
                                        if (discount != null)
                                        {
                                            return discount;
                                            //if (discount.DiscountType.ToLower() == "Percentage")
                                            //{
                                            //    discountAmount = Convert.ToDouble(productAmount - ((productAmount * discount.DiscountAmount) / 100));
                                            //    return discountAmount;
                                            //}
                                            //else
                                            //{
                                            //    discountAmount = Convert.ToDouble(productAmount - discount.DiscountAmount);
                                            //    return discountAmount;
                                            //}
                                        }
                                    
                                }
                            }
                        }

                        if (offer != null)
                        {
                            offer.Title = lang=="en"?offer.Title : offer.TitleAr;
                            offer.Description = lang == "en" ? offer.Description : offer.DescriptionAr;
                            offer.FeatureType = lang == "en" ? offer.FeatureType : offer.FeatureTypeAr;
                           
                        }
                    }
                }
                return null; 
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }


        public async Task<OfferDiscount> ServiceDiscountCheck(int ServiceId,int ServiceTypeId, int featturecategoryId,string lang="en")
        {
            try
            {
                var offers = new List<OfferInformation>();
                double discountAmount = 0.0;
                await updateAllOfferAvaibality();
                if (featturecategoryId > 0)
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.FeatureCategoryId == featturecategoryId && p.IsTimeLimit == false && p.FeatureType == "Service").OrderBy(s => s.IsService).ToListAsync();
                }
                else
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active"  && p.IsTimeLimit == false && p.FeatureType == "Service").OrderBy(s => s.IsService).ToListAsync();
                }

                //var timeOffers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.FeatureCategoryId == featturecategoryId && p.IsTimeLimit == true && p.StartDate < dubaitime && p.EndDate > dubaitime && p.FeatureType == "Service").OrderBy(s => s.IsService).ToListAsync();
                //if (timeOffers != null && timeOffers.Count() > 0)
                //{
                //    offers.AddRange(timeOffers);
                //}
                if (offers != null && offers.Count() > 0)
                {
                    foreach (var offer in offers)
                    {
                        //if (offer.IsBrand == true && offer.IsProduct == false && offer.IsCategory == false)
                        //{
                        //    var brands = await uow.BrandOfferRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).Select(s => s.BrandId).ToListAsync();
                        //    if (brands != null && brands.Count() > 0)
                        //    {
                        //        var productAmount = await uow.ProductRepository.Get().Where(s => s.ProductId == productId && brands.Contains(s.BrandId.Value)).Select(s => s.ProductPrice).FirstOrDefaultAsync();
                        //        if (productAmount > 0)
                        //        {
                        //            var discount = await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).FirstOrDefaultAsync();
                        //            if (discount != null)
                        //            {
                        //                return discount;
                        //                //if (discount.DiscountType.ToLower() == "Percentage")
                        //                //{
                        //                //    discountAmount = Convert.ToDouble(productAmount - ((productAmount * discount.DiscountAmount) / 100));
                        //                //}
                        //                //else
                        //                //{
                        //                //    discountAmount = Convert.ToDouble(productAmount - discount.DiscountAmount);
                        //                //    return discountAmount;
                        //                //}
                        //            }
                        //        }
                        //    }
                        //}
                         if (offer.IsBrand == false && offer.IsService == true && offer.IsCategory == false)
                        {
                            var services = await uow.ServiceOffersRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId && s.ServiceId == ServiceId).ToListAsync();
                            if (services!=null && services.Count()>0)
                            {
                                foreach (var service in services)
                                {
                                    if (service.ServiceTypeId == null)
                                    {
                                        var discount = await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).FirstOrDefaultAsync();
                                        if (discount != null)
                                        {
                                            return discount;

                                        }
                                    }
                                    else if(service.ServiceTypeId == ServiceTypeId)
                                    {
                                        var discount = await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).FirstOrDefaultAsync();
                                        if (discount != null)
                                        {
                                            return discount;

                                        }

                                    }
                                }
                                }
                              
                            
                        }

                        else if (offer.IsCategory == true && offer.IsBrand == false && offer.IsService == false)
                        {
                            var categories = await uow.CategoryOfferRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).Select(s => s.CategoryId).ToListAsync();
                            if (categories != null && categories.Count() > 0)
                            {
                                var services = await uow.ServiceCategoryRepository.Get().AnyAsync(s => s.ServiceId == ServiceId && categories.Contains(s.CategoryId));


                                if (services == true)
                                {
                                   
                                        var discount = await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == offer.OfferInformationId).FirstOrDefaultAsync();
                                        if (discount != null)
                                        {
                                            return discount;
                                            //if (discount.DiscountType.ToLower() == "Percentage")
                                            //{
                                            //    discountAmount = Convert.ToDouble(productAmount - ((productAmount * discount.DiscountAmount) / 100));
                                            //    return discountAmount;
                                            //}
                                            //else
                                            //{
                                            //    discountAmount = Convert.ToDouble(productAmount - discount.DiscountAmount);
                                            //    return discountAmount;
                                            //}
                                        }
                                    }
                                
                            }
                        }
                        if (offer != null)
                        {
                            offer.Title = lang == "en" ? offer.Title : offer.TitleAr;
                            offer.Description = lang == "en" ? offer.Description : offer.DescriptionAr;
                            offer.FeatureType = lang == "en" ? offer.FeatureType : offer.FeatureTypeAr;

                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task updateAllOfferAvaibality()
        {
            try
            {
              
                var offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit==true && p.EndDate < dubaitime).ToListAsync();
                if (offers != null && offers.Count() > 0)
                {
                    foreach (var offer in offers)
                    {
                        offer.Status = "Finished";
                        offer.UpdateDate = DateTimeHelper.DubaiTime();
                        await uow.OfferInformationRepository.Edit(offer);
                    }
                }
            }
            catch (Exception)
            {

              
            }
        }

        public async Task<OfferInformation> TrendingOfferBrands(int featureCategoryId, int size = 10, int count = 0, string lang = "en")
        {
            try
            {
                var offers = new OfferInformation();
                await updateAllOfferAvaibality();
                if (featureCategoryId > 0)
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == true && p.IsProduct == false && p.IsCategory == false  && p.FeatureCategoryId == featureCategoryId && p.IsService == false && p.FeatureType == "Product").FirstOrDefaultAsync();
                    if (offers != null)
                    {
                        var banners = await uow.OfferBannerRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).ToListAsync();

                        if (banners != null && banners.Count() > 0)
                        {
                            foreach (var banner in banners)
                            {
                                if (!string.IsNullOrEmpty(banner.BannerImage))
                                {
                                    banner.BannerImage = link + banner.BannerImage;
                                }
                                banner.OfferInformation = null;
                            }
                            offers.Banners = banners;
                        }

                        var brandOfferes = await uow.BrandOfferRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).OrderByDescending(s => s.BrandId).Select(s => s.BrandId).Skip(count).Take(size).ToListAsync();
                        if (brandOfferes != null && brandOfferes.Count() > 0)
                        {
                            var brands = new List<Brand>();
                            if (featureCategoryId > 0)
                            {
                                brands = await uow.BrandRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId && brandOfferes.Contains(s.BrandId) && s.Status == "Active").ToListAsync();
                            }
                            else
                            {
                                brands = await uow.BrandRepository.Get().Where(s => brandOfferes.Contains(s.BrandId) && s.Status == "Active").ToListAsync();
                            }

                            if (brands != null && brands.Count() > 0)
                            {

                                foreach (var item in brands)
                                {
                                    if (!string.IsNullOrEmpty(item.Image))
                                    {
                                        item.Image = link + item.Image;
                                    }

                                }

                                offers.Brands = brands;

                            }
                        }


                        offers.Title = lang == "en" ? offers.Title : offers.TitleAr;
                        offers.Description = lang == "en" ? offers.Description : offers.DescriptionAr;
                        offers.FeatureType = lang == "en" ? offers.FeatureType : offers.FeatureTypeAr;

                    }
                    return offers;
                }
                else
                {
                    var OfferList = new List<int>();
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == true && p.IsProduct == false && p.IsCategory == false && p.IsTrending == true  && p.IsService == false && p.FeatureType == "Product").FirstOrDefaultAsync();
                    OfferList = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == true && p.IsProduct == false && p.IsCategory == false && p.IsTrending == true && p.IsService == false && p.FeatureType == "Product").Select(s => s.OfferInformationId).ToListAsync();
                    if (offers != null)
                    {
                        var banners = await uow.OfferBannerRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).ToListAsync();

                        if (banners != null && banners.Count() > 0)
                        {
                            foreach (var banner in banners)
                            {
                                if (!string.IsNullOrEmpty(banner.BannerImage))
                                {
                                    banner.BannerImage = link + banner.BannerImage;
                                }
                                banner.OfferInformation = null;
                            }
                            offers.Banners = banners;
                        }
                        var brands = new List<Brand>();
                        var brandOfferes = new List<int>();
                        if (OfferList != null && OfferList.Count() > 0)
                        {
                             brandOfferes = await uow.BrandOfferRepository.Get().Where(s => OfferList.Contains(s.OfferInformationId.Value)).OrderByDescending(s => s.BrandId).Select(s => s.BrandId).Skip(count).Take(size).ToListAsync();
                        }
                        else
                        {
                             brandOfferes = await uow.BrandOfferRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).OrderByDescending(s => s.BrandId).Select(s => s.BrandId).Skip(count).Take(size).ToListAsync();
                        }
                       
                        if (brandOfferes != null && brandOfferes.Count() > 0)
                        {
                            
                            if (featureCategoryId > 0)
                            {
                                brands = await uow.BrandRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId && brandOfferes.Contains(s.BrandId) && s.Status == "Active").ToListAsync();
                            }
                            else
                            {
                                brands = await uow.BrandRepository.Get().Where(s => brandOfferes.Contains(s.BrandId) && s.Status == "Active").ToListAsync();
                            }

                            if (brands != null && brands.Count() > 0)
                            {

                                foreach (var item in brands)
                                {
                                    if (!string.IsNullOrEmpty(item.Image))
                                    {
                                        item.Image = link + item.Image;
                                    }

                                }

                                offers.Brands = brands;

                            }
                        }


                        offers.Title = lang == "en" ? offers.Title : offers.TitleAr;
                        offers.Description = lang == "en" ? offers.Description : offers.DescriptionAr;
                        offers.FeatureType = lang == "en" ? offers.FeatureType : offers.FeatureTypeAr;

                    }
                    return offers;
                }
              
            }
            catch (Exception)
            {

                return null;
            }
        }


        public async Task<OfferInformation> TrendingOfferCategory(int featureCategoryId, int size = 10, int count = 0, string lang = "en")
        {
            try
            {
                  var offers = new OfferInformation();
                await updateAllOfferAvaibality();
                if (featureCategoryId > 0)
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == false && p.IsCategory == true  && p.FeatureCategoryId == featureCategoryId && p.IsService == false && p.FeatureType == "Product").FirstOrDefaultAsync();
                    if (offers != null)
                    {
                        var banners = await uow.OfferBannerRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).ToListAsync();

                        if (banners != null && banners.Count() > 0)
                        {
                            foreach (var banner in banners)
                            {
                                if (!string.IsNullOrEmpty(banner.BannerImage))
                                {
                                    banner.BannerImage = link + banner.BannerImage;
                                }
                                banner.OfferInformation = null;
                            }
                            offers.Banners = banners;
                        }

                        var categoryOfferes = await uow.CategoryOfferRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).OrderByDescending(s => s.CategoryId).Select(s => s.CategoryId).Skip(count).Take(size).ToListAsync();
                        if (categoryOfferes != null && categoryOfferes.Count() > 0)
                        {


                            foreach (var item in categoryOfferes)
                            {
                                var cat = await new CategoryServiceAccess().Get(item);
                                if (cat != null)
                                {
                                    offers.Categories.Add(cat);
                                }

                            }


                        }
                        offers.Title = lang == "en" ? offers.Title : offers.TitleAr;
                        offers.Description = lang == "en" ? offers.Description : offers.DescriptionAr;
                        offers.FeatureType = lang == "en" ? offers.FeatureType : offers.FeatureTypeAr;
                    }
                    return offers;
                }
                else
                {
                    var OfferList = new List<int>();
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == false && p.IsCategory == true && p.IsTrending == true  && p.IsService == false && p.FeatureType == "Product").FirstOrDefaultAsync();
                    OfferList = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == false && p.IsCategory == true && p.IsTrending == true && p.IsService == false && p.FeatureType == "Product").Select(s=>s.OfferInformationId).ToListAsync();
                    if (offers != null)
                    {
                        var banners = await uow.OfferBannerRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).ToListAsync();

                        if (banners != null && banners.Count() > 0)
                        {
                            foreach (var banner in banners)
                            {
                                if (!string.IsNullOrEmpty(banner.BannerImage))
                                {
                                    banner.BannerImage = link + banner.BannerImage;
                                }
                                banner.OfferInformation = null;
                            }
                            offers.Banners = banners;
                        }
                        var categoryOfferes = new List<int>();
                        if (OfferList.Count() > 0 && OfferList != null)
                        {
                            categoryOfferes = await uow.CategoryOfferRepository.Get().Where(s => OfferList.Contains(s.OfferInformationId.Value)).OrderByDescending(s => s.CategoryId).Select(s => s.CategoryId).Skip(count).Take(size).ToListAsync();
                        }
                        else
                        {
                            categoryOfferes = await uow.CategoryOfferRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).OrderByDescending(s => s.CategoryId).Select(s => s.CategoryId).Skip(count).Take(size).ToListAsync();
                        }
                        
                        if (categoryOfferes != null && categoryOfferes.Count() > 0)
                        {


                            foreach (var item in categoryOfferes)
                            {
                                var cat = await new CategoryServiceAccess().Get(item);
                                if (cat != null)
                                {
                                    offers.Categories.Add(cat);
                                }

                            }


                        }
                        offers.Title = lang == "en" ? offers.Title : offers.TitleAr;
                        offers.Description = lang == "en" ? offers.Description : offers.DescriptionAr;
                        offers.FeatureType = lang == "en" ? offers.FeatureType : offers.FeatureTypeAr;
                    }
                    return offers;
                }
             
            }
            catch (Exception)
            {

                return null;
            }
        }


        public async Task<OfferInformation> TrendingOfferproducts(int featureCategoryId, int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {
            try
            {
                var offers = new OfferInformation();
                var offerList = new List<int>();
                await updateAllOfferAvaibality();
                if (featureCategoryId > 0)
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == true && p.IsCategory == false && p.FeatureCategoryId == featureCategoryId && p.IsService == false && p.FeatureType == "Product").FirstOrDefaultAsync();
                }
                else
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == true && p.IsCategory == false && p.IsTrending == true  && p.IsService == false && p.FeatureType == "Product").FirstOrDefaultAsync();
                    offerList = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == true && p.IsCategory == false && p.IsTrending == true && p.IsService == false && p.FeatureType == "Product").Select(s=>s.OfferInformationId).ToListAsync();
                }
                if (offers != null)
                {
                    var banners = await uow.OfferBannerRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).ToListAsync();

                    if (banners != null && banners.Count() > 0)
                    {
                        foreach (var banner in banners)
                        {
                            if (!string.IsNullOrEmpty(banner.BannerImage))
                            {
                                banner.BannerImage = link + banner.BannerImage;
                            }
                            banner.OfferInformation = null;
                        }
                        offers.Banners = banners;
                    }
                    var ProductsOfferes = new List<int>();
                    if (offerList.Count() > 0 && offerList != null)
                    {
                        ProductsOfferes = await uow.ProductOfferRepository.Get().Where(s => offerList.Contains(s.OfferInformationId.Value)).OrderByDescending(s => s.ProductId).Select(s => s.ProductId).Skip(count).Take(size).ToListAsync();
                    }
                    else
                    {
                         ProductsOfferes = await uow.ProductOfferRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).OrderByDescending(s => s.ProductId).Select(s => s.ProductId).Skip(count).Take(size).ToListAsync();
                    }
                      
                    if (ProductsOfferes != null && ProductsOfferes.Count() > 0)
                    {
                        foreach (var product in ProductsOfferes)
                        {
                            var smallproduct = await new ProductServiceAccess().getSmallDetailsProducts(product,memberId);
                            if (smallproduct != null)
                            {
                                offers.Products.Add(smallproduct);
                            }
                        }

                        
                    }
                    offers.Title = lang == "en" ? offers.Title : offers.TitleAr;
                    offers.Description = lang == "en" ? offers.Description : offers.DescriptionAr;
                    offers.FeatureType = lang == "en" ? offers.FeatureType : offers.FeatureTypeAr;
                }
                return offers;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<OfferInformation> TrendingOfferCategoryService(int featureCategoryId, int size = 10, int count = 0, string lang = "en")
        {
            try
            {
                var offers = new OfferInformation();
                var offerList = new List<int>();
                await updateAllOfferAvaibality();

                if (featureCategoryId > 0)
                {
                     offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == false && p.IsCategory == true  && p.FeatureCategoryId == featureCategoryId && p.IsService == false && p.FeatureType == "Service").FirstOrDefaultAsync();
                }
                else
                {
                    offers = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == false && p.IsCategory == true && p.IsTrending == true  && p.IsService == false && p.FeatureType == "Service").FirstOrDefaultAsync();

                    offerList = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == false && p.IsCategory == true && p.IsTrending == true && p.IsService == false && p.FeatureType == "Service").Select(s=>s.OfferInformationId).ToListAsync();
                }
                if (offers != null)
                {
                    var banners = await uow.OfferBannerRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).ToListAsync();

                    if (banners != null && banners.Count() > 0)
                    {
                        foreach (var banner in banners)
                        {
                            if (!string.IsNullOrEmpty(banner.BannerImage))
                            {
                                banner.BannerImage = link + banner.BannerImage;
                            }
                            banner.OfferInformation = null;
                        }
                        offers.Banners = banners;
                    }

                    var categoryOfferes = new List<int>();
                    if (offerList.Count() > 0 && offerList != null)
                    {
                        categoryOfferes = await uow.CategoryOfferRepository.Get().Where(s => offerList.Contains(s.OfferInformationId.Value)).OrderByDescending(s => s.CategoryId).Select(s => s.CategoryId).Skip(count).Take(size).ToListAsync();
                    }
                    else
                    {
                        categoryOfferes = await uow.CategoryOfferRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).OrderByDescending(s => s.CategoryId).Select(s => s.CategoryId).Skip(count).Take(size).ToListAsync();
                    }
                    if (categoryOfferes != null && categoryOfferes.Count() > 0)
                    {

                        foreach (var item in categoryOfferes)
                        {
                            var cat = await new CategoryServiceAccess().Get(item);
                            if (cat != null)
                            {
                                offers.Categories.Add(cat);
                            }

                        }


                    }
                    offers.Title = lang == "en" ? offers.Title : offers.TitleAr;
                    offers.Description = lang == "en" ? offers.Description : offers.DescriptionAr;
                    offers.FeatureType = lang == "en" ? offers.FeatureType : offers.FeatureTypeAr;
                }
                return offers;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<List<OfferInformation>> TrendingOfferServices(int featureCategoryId, int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {
            try
            {
                var offersList = new List<OfferInformation>();
                await updateAllOfferAvaibality();
                if (featureCategoryId > 0)
                {
                    // Filter by category + trending — do not restrict by IsService/IsCategory flags
                    // (admin-created service offers may use IsCategory=true, IsService=false)
                    offersList = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == false && p.IsTrending == true && p.FeatureCategoryId == featureCategoryId).ToListAsync();
                }
                else
                {
                    offersList = await uow.OfferInformationRepository.Get().Where(p => p.Status.ToLower() == "active" && p.IsTimeLimit == false && p.IsBrand == false && p.IsProduct == false && p.IsTrending == true && p.FeatureType == "Service").ToListAsync();
                }
                if (offersList != null && offersList.Count()>0)
                {
                    foreach (var offers in offersList) {
                        var banners = await uow.OfferBannerRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).ToListAsync();

                        if (banners != null && banners.Count() > 0)
                        {
                            foreach (var banner in banners)
                            {
                                if (!string.IsNullOrEmpty(banner.BannerImage))
                                {
                                    banner.BannerImage = link + banner.BannerImage;
                                }
                                banner.OfferInformation = null;
                            }
                            offers.Banners = banners;
                        }

                        var servicesOffers = await uow.ServiceOffersRepository.Get().Where(s => s.OfferInformationId == offers.OfferInformationId).OrderByDescending(s => s.ServiceId).Skip(count).Take(size).ToListAsync();
                        if (servicesOffers != null && servicesOffers.Count() > 0)
                        {
                            if (servicesOffers.Count() == 1)
                            {
                                if (servicesOffers.FirstOrDefault().ServiceTypeId == null)
                                {
                                    var smallServices = await new ServiceAccess().GetSmallServices(servicesOffers.FirstOrDefault().ServiceId);
                                    if (smallServices != null)
                                    {
                                        offers.RedirectToServiceDetails = true;
                                        offers.ServicesDetails = smallServices;
                                    }
                                }
                                else 
                                {
                                    var smallServices = await new ServiceAccess().GetSmallServices(servicesOffers.FirstOrDefault().ServiceId, servicesOffers.FirstOrDefault().ServiceTypeId.Value);
                                    if (smallServices != null)
                                    {
                                        offers.RedirectToServiceDetails = true;
                                        offers.ServicesDetails = smallServices;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var service in servicesOffers)
                                {

                                    var smallServices = await new ServiceAccess().GetSmallServices(service.ServiceId,service.ServiceTypeId.Value,memberId);
                                    if (smallServices != null)
                                    {
                                        offers.Services.Add(smallServices);
                                    }
                                }
                            }


                        }

                        if (offers != null)
                        {
                            offers.Title = lang == "en" ? offers.Title : offers.TitleAr;
                            offers.Description = lang == "en" ? offers.Description : offers.DescriptionAr;
                            offers.FeatureType = lang == "en" ? offers.FeatureType : offers.FeatureTypeAr;
                        }
                    }
                 
                }
                return offersList;
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}