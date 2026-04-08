using Boulevard.BaseRepository;
using Boulevard.Models;
using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Services.Description;
using System.Xml.XPath;

namespace Boulevard.Service.WebAPI
{
    public class ProductServiceAccess
    {
        public IUnitOfWork uow;
        public ProductServiceAccess()
        {
            uow = new UnitOfWork();
        }

        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);


        public async Task<ProductSmallDetailsResponse> getSmallDetailsProducts(int productId,int memberId=0,bool IsOrder=false, string lang = "en",int productPriceid=0)
        {
            try
            {
                var result = new ProductSmallDetailsResponse();
                var products = new Product();
                if (IsOrder == true)
                {
                    products = await uow.ProductRepository.Get().Where(s =>  s.ProductId == productId).Include(s => s.Brands).FirstOrDefaultAsync();
                }
                else
                {
                    products = await uow.ProductRepository.Get().Where(s => s.Status == "Active" && s.ProductId == productId).Include(s => s.Brands).FirstOrDefaultAsync();
                }

                if (products != null)
                {
                    result.ProductId = products.ProductId;
                    result.ProductPrice = products.ProductPrice;
                    result.ProductName = lang=="en"?products.ProductName:products.ProductNameAr;
                    result.AttributeName = lang == "en" ? products.AttributeName : products.AttributeNameArabic;
                    result.Barcode = products.Barcode;
                    result.StockQuantity = products.StockQuantity;
                    result.IsScheduled = products.IsScheduled;
                    result.ProductTypeId = products.ProductType;
                    result.AvrageRatings = products.AvgRatings;
                    result.IcvBoulevardScore = products.IcvBoulevardScore;
                    result.Origin = products.Origin;
                   var ptype = await uow.ProductTypeMasterRepository.Get().Where(s => s.ProductTypeId == products.ProductType).FirstOrDefaultAsync();
                    if (ptype != null)
                    {
                        result.ProductTypeName = lang == "en" ? ptype.Name : ptype.NameAr;
                    }
                    if (memberId > 0)
                    {
                        result.IsFavourite = await uow.FavouriteProductRepository.Get().AnyAsync(s => s.MemberId == memberId && s.ProductId == productId);
                    }
                    if (await uow.ProductImageRepository.Get().AnyAsync(s => s.ProductId == products.ProductId))
                    {
                        result.Image = link + await uow.ProductImageRepository.Get().Where(s => s.ProductId == products.ProductId).Select(s => s.Image).FirstOrDefaultAsync();
                    }
                    var discountinfo = await new OfferServiceAccess().ProductDiscountCheck(productId, products.FeatureCategoryId.Value,lang);

                    var productPriceList = await uow.ProductPriceRepository.Get().Where(w => w.Status == "Active" && w.ProductId == productId && (productPriceid==0 || w.ProductPriceId==productPriceid) ).ToListAsync();
                    result.ProductPrices = productPriceList;

                    if (discountinfo != null)
                    {
                        result.DiscountInformation = discountinfo;
                        if (discountinfo.DiscountType.ToLower() == "percentage")
                        {
                            result.DiscountPrice = Convert.ToDouble(result.ProductPrice - ((result.ProductPrice * discountinfo.DiscountAmount) / 100));
                        }
                        else
                        {
                            result.DiscountPrice = Convert.ToDouble(result.ProductPrice - discountinfo.DiscountAmount);

                        }
                        result.ProductPrices = ApplyDiscountToProductPrices(productPriceList, discountinfo.DiscountType, discountinfo.DiscountAmount);
                    }
                    else
                    {
                        if (memberId > 0)
                        {
                            var memberInfo = await new MemberServiceAccess().GetById(memberId);

                            if (memberInfo.IsAnyMembership == true && memberInfo.MemberSubscriptions != null)
                            {
                                var MembershipDiscount = await uow.MemberShipDiscountCategoryRepository.Get().Where(s => s.FeatureCategoryId == products.FeatureCategoryId && s.MemberShipId == memberInfo.MemberSubscriptions.MemberShipId).FirstOrDefaultAsync();
                                if (MembershipDiscount != null)
                                {
                                    result.MemberShipInformation = MembershipDiscount;
                                    if (MembershipDiscount.MemberShipDiscountType.ToLower() == "percentage")
                                    {
                                        result.DiscountPrice = Convert.ToDouble(result.ProductPrice - ((result.ProductPrice * MembershipDiscount.MemberShipDiscountAmount) / 100));
                                    }
                                    else
                                    {
                                        result.DiscountPrice = Convert.ToDouble(result.ProductPrice - MembershipDiscount.MemberShipDiscountAmount);

                                    }
                                    result.ProductPrices = ApplyDiscountToProductPrices(productPriceList, MembershipDiscount.MemberShipDiscountType, (double)MembershipDiscount.MemberShipDiscountAmount);
                                }

                               
                            }

                            if (await uow.CartRepository.Get().AnyAsync(x => x.MemberId == memberId && x.ProductId == productId))
                            {
                                result.Quantity = await uow.CartRepository.Get().Where(x => x.MemberId == memberId && x.ProductId == productId).Select(s => s.Quantity).FirstOrDefaultAsync();
                            }
                            
                        }
                }
                if (products.Brands != null)
                    {
                        result.BrandInfo = products.Brands;
                        result.BrandInfo.Title = lang=="en"?result.BrandInfo.Title : result.BrandInfo.TitleAr;
                        result.BrandInfo.Details = lang == "en" ? result.BrandInfo.Details : result.BrandInfo.DetailsAr;

                    }

                    if (result.ProductPrices != null && result.ProductPrices.Count() > 0)
                    {
                        var relatedprice = result.ProductPrices.Where(s => s.ProductQuantity == 1).FirstOrDefault();
                        if (relatedprice == null)
                        {
                            relatedprice = result.ProductPrices.FirstOrDefault();
                            if (relatedprice != null)
                            {
                                result.ProductPrice = Convert.ToDecimal(relatedprice.Price);
                                result.DiscountPrice = Convert.ToDouble(relatedprice.Discount);
                            }
                        }
                        else
                        {
                            result.ProductPrice = Convert.ToDecimal(relatedprice.Price);
                            result.DiscountPrice = Convert.ToDouble(relatedprice.Discount);
                        }

                       var review =  await uow.UserReviewRepository.Get().AnyAsync(p => p.FeatureType.ToLower() == "product" && p.FeatureTypeId == productId);

                        if (review == true)
                        {
                            result.TotalReview = await uow.UserReviewRepository.Get().Where(p => p.FeatureType.ToLower() == "product" && p.FeatureTypeId == productId).CountAsync();
                        }

                       
                    }
                    return result;

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


        public async Task<List<ProductSmallDetailsResponse>> GetProductBySearching(int featureCategoryId, string keyword = "", int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {

            try
            {

                if (count > 0)
                {
                    count = count * size;
                }
                var productids = new List<int>();

                var result = new List<ProductSmallDetailsResponse>();



                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var searchWords = keyword.ToLower().Split(" ".ToCharArray(),
                            StringSplitOptions.RemoveEmptyEntries);

                        productids = await uow.ProductRepository.Get().Where(s => searchWords.Any(t => s.ProductName.ToLower().Contains(t)) && s.FeatureCategoryId==featureCategoryId && s.Status == "Active").OrderByDescending(s => s.ProductId).Take(size).Skip(count).Select(s => s.ProductId).ToListAsync();
                    }
                    else
                    {
                        productids = await uow.ProductRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId && s.Status == "Active").OrderByDescending(s => s.ProductId).Take(size).Skip(count).Select(s => s.ProductId).ToListAsync();
                    }

                    if (productids.Count() > 0 || productids != null)
                    {
                        foreach (var productid in productids)
                        {
                            var productResult = await new ProductServiceAccess().getSmallDetailsProducts(productid, memberId,false,lang);
                        if (productResult != null)
                        {

                            result.Add(productResult);
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

        public async Task<Product> getProductDetails(int productId,int memberId=0,string lang="en")
        {
            try
            {
               
                var products = await uow.ProductRepository.Get().Include("Brands").Where(s => s.Status == "Active" && s.ProductId == productId).FirstOrDefaultAsync();

                if (products != null)
                {
                    products.ProductName = lang=="en"?products.ProductName : products.ProductNameAr;
                    products.ProductSlag = lang=="en"?products.ProductSlag : products.ProductSlagAr;
                    products.ProductDescription = lang=="en"? products.ProductDescription : products.ProductDescriptionAr;
                    products.AttributeName = lang == "en" ? products.AttributeName : products.AttributeNameArabic;

                    if (await uow.ProductImageRepository.Get().AnyAsync(s => s.ProductId == products.ProductId))
                    {

                       var images = await uow.ProductImageRepository.Get().Where(s => s.ProductId == products.ProductId).ToListAsync();
                        if (images.Count() > 0 && images != null)
                        {
                            foreach (var item in images)
                            {
                                if (!string.IsNullOrEmpty(item.Image))
                                {
                                    item.Image = link + item.Image;
                                    item.Product = null;
                                }
                            }
                           
                        }
                        products.Images = images;
                    }
					if (memberId > 0)
					{
						products.IsFavourite = await uow.FavouriteProductRepository.Get().AnyAsync(s => s.MemberId == memberId && s.ProductId == productId);
					}
					products.UserReviews = await uow.UserReviewRepository.Get().Where(p => p.FeatureType.ToLower() == "product" && p.FeatureTypeId == productId).OrderByDescending(s=>s.UserReviewId).ToListAsync();
					//.Include(p => p.userReviewImages)
					if (products.UserReviews != null && products.UserReviews.Count > 0)
                    {
                        foreach (var item in products.UserReviews)
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
                    var discountinfo = await new OfferServiceAccess().ProductDiscountCheck(productId, products.FeatureCategoryId.Value,lang);
                    var productPriceList= await uow.ProductPriceRepository.Get().Where(w => w.Status == "Active"&&w.ProductId==productId).ToListAsync();
                    products.ProductPrices = productPriceList;
                    if (discountinfo != null)
                    {
                        products.DiscountInformation = discountinfo;
                        if (discountinfo.DiscountType.ToLower() == "percentage")
                        {
                            products.DiscountPrice = Convert.ToDouble(products.ProductPrice - ((products.ProductPrice * discountinfo.DiscountAmount) / 100));
                        }
                        else
                        {
                            products.DiscountPrice = Convert.ToDouble(products.ProductPrice - discountinfo.DiscountAmount);

                        }

                        products.ProductPrices = ApplyDiscountToProductPrices(productPriceList, discountinfo.DiscountType, discountinfo.DiscountAmount);

                    }
                    else
                    {
                        if (memberId > 0)
                        {
                            var memberInfo = await new MemberServiceAccess().GetById(memberId);

                            if (memberInfo.IsAnyMembership == true && memberInfo.MemberSubscriptions != null)
                            {
                                var MembershipDiscount = await uow.MemberShipDiscountCategoryRepository.Get().Where(s => s.FeatureCategoryId == products.FeatureCategoryId && s.MemberShipId == memberInfo.MemberSubscriptions.MemberShipId).FirstOrDefaultAsync();
                                if (MembershipDiscount != null)
                                {
                                    products.MemberShipInformation = MembershipDiscount;
                                    if (MembershipDiscount.MemberShipDiscountType.ToLower() == "percentage")
                                    {
                                        products.DiscountPrice = Convert.ToDouble(products.ProductPrice - ((products.ProductPrice * MembershipDiscount.MemberShipDiscountAmount) / 100));
                                    }
                                    else
                                    {
                                        products.DiscountPrice = Convert.ToDouble(products.ProductPrice - MembershipDiscount.MemberShipDiscountAmount);

                                    }
                                    products.ProductPrices = ApplyDiscountToProductPrices(productPriceList, MembershipDiscount.MemberShipDiscountType, (double)MembershipDiscount.MemberShipDiscountAmount);
                                }


                            }

                        }
                    }

                    if(products.Brands!=null)
                    {
                        products.Brands.Title = lang == "en" ? products.Brands.Title : products.Brands.TitleAr;
                    }
                    if (products.ProductPrices != null && products.ProductPrices.Count() > 0)
                    {
                        var relatedprice = products.ProductPrices.Where(s => s.ProductQuantity == 1).FirstOrDefault();
                        if (relatedprice == null)
                        {
                            relatedprice = products.ProductPrices.FirstOrDefault();
                            if (relatedprice != null)
                            {
                                products.ProductPrice = Convert.ToDecimal(relatedprice.Price);
                                products.DiscountPrice = Convert.ToDouble(relatedprice.Discount);
                            }
                        }
                        else
                        {
                            products.ProductPrice = Convert.ToDecimal(relatedprice.Price);
                            products.DiscountPrice = Convert.ToDouble(relatedprice.Discount);
                        }
                    }

                }
                return products;



            }
            catch (Exception)
            {

                return null;
            }
        }


        public async Task<List<ProductSmallDetailsResponse>> RelatedProducts(int featureCategoryId,int productId, int size = 10, int count = 0,int memberId = 0, string lang = "en")
        {

            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var productids = new List<int>();

                var result = new List<ProductSmallDetailsResponse>();


                var crosssell = await uow.CrosssellFeaturesRepository.Get().Where(s => s.FeatureCategoryId== featureCategoryId && s.CrosssellFeaturesType.ToLower() == "product" && s.CrosssellFeaturesTypeId == productId).OrderByDescending(s=>s.CrosssellFeaturesId).Skip(count).Take(size).Select(s=>s.RelatedFeatureId).ToListAsync();
              


                if (crosssell.Count() > 0 || crosssell != null)
                {
                    foreach (var productid in crosssell)
                    {
                        var productResult = await new ProductServiceAccess().getSmallDetailsProducts(productid, memberId,false,lang);
                        if (productResult != null)
                        {
                            result.Add(productResult);
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

        public async Task<List<ProductSmallDetailsResponse>> AddMoreProducts(int featureCategoryId,int productId, int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {

            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var productids = new List<int>();

                var result = new List<ProductSmallDetailsResponse>();


                var crosssell = await uow.UpsellFeaturesRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId &&  s.UpsellFeaturesType.ToLower() == "product" && s.UpsellFeaturesTypeId == productId).OrderByDescending(s => s.UpsellFeaturesId).Skip(count).Take(size).Select(s => s.RelatedFeatureId).ToListAsync();



                if (crosssell.Count() > 0 || crosssell != null)
                {
                    foreach (var productid in crosssell)
                    {
                        var productResult = await new ProductServiceAccess().getSmallDetailsProducts(productid, memberId, false, lang);
                        if (productResult != null)
                        { 
                        result.Add(productResult);
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
        public async Task<List<ProductSmallDetailsResponse>> BestSellingProducts(int featureCategoryId, int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {

            try
            {
                if (count > 0)
                {
                    count = count * size;
                }
                var productids = new List<int>();

                var result = new List<ProductSmallDetailsResponse>();


                var BestSell  = await (from Order in  uow.OrderRequestProductRepository.Get()
                                      join details in  uow.OrderRequestProductDetailsRepository.Get()
                                      on Order.OrderRequestProductId equals details.OrderRequestProductId
                                      where Order.FeatureCategoryId == featureCategoryId
                                      group details by details.ProductId into g
                                      select new { ProductId = g.Key, Quantity = g.Sum(s=>s.Quantity) }).OrderByDescending(s=>s.Quantity).Skip(count).Take(size).ToListAsync();

                //await uow.OrderRequestProductDetailsRepository.Get().GroupBy( p => p.ProductId, p => p.Quantity,(key, g) => new { productId = key, count = g.Sum() }).OrderBy(s=>s.count).Skip(count).Take(size).ToListAsync();



                if (BestSell.Count() > 0 || BestSell != null)
                {
                    foreach (var productid in BestSell.Select(s=>s.ProductId))
                    {
                        var productResult = await new ProductServiceAccess().getSmallDetailsProducts(productid, memberId, false, lang);

                        if (productResult != null)
                        {
                            result.Add(productResult);
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

        public async Task<List<ProductTagResponse>> GetProductTags(int featureCategoryId)
        {

            try
            {
                var productids = new List<int>();

                var result = new List<ProductTagResponse>();


                var ProductsTag = await uow.CommonProductTagRepository.Get().Where(s => s.FeatureCategoryId == featureCategoryId && s.Status=="Active").ToListAsync();



                if (ProductsTag.Count() > 0 || ProductsTag != null)
                {
                    foreach (var tags in ProductsTag)
                    {
                        var productResult = new ProductTagResponse();
                        productResult.CommonProductTagId = tags.CommonProductTagId;
                        productResult.TagName = tags.TagName;
                        result.Add( productResult );
                    }
                }


                return result;




            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<List<ProductSmallDetailsResponse>> GettagsProduct(int featureCategoryId, int commonProductTagId, int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {

            try
            {
                if (count > 0)
                {
                    count = count * size;
                }

                var productids = new List<int>();

                var result = new List<ProductSmallDetailsResponse>();


                var products = await uow.CommonProductTagDetailsRepository.Get().Where(s => s.CommonProductTagId == commonProductTagId).OrderByDescending(s => s.ProductId).Skip(count).Take(size).Select(s => s.ProductId).ToListAsync();



                if (products.Count() > 0 || products != null)
                {
                    foreach (var productid in products)
                    {
                        var productResult = await new ProductServiceAccess().getSmallDetailsProducts(productid, memberId,false,lang);
                        if (productResult != null)
                        {
                            result.Add(productResult);
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
        public List<ProductPrice> ApplyDiscountToProductPrices(List<ProductPrice> productPriceList,string discountType= "percentage", double discountAmount=0)
        {
            if (productPriceList != null && productPriceList.Count > 0 && discountAmount > 0)
            {
                foreach (var priceItem in productPriceList)
                {
                    if (discountType?.ToLower() == "percentage")
                    {
                        priceItem.Discount = priceItem.Price - ((priceItem.Price * discountAmount) / 100);
                    }
                    else
                    {
                        priceItem.Discount = priceItem.Price - discountAmount;
                    }

                  
                    if (priceItem.Discount == 0)
                        priceItem.Discount = 0;
                }
            }
            return productPriceList;
        }


        public  async Task<List<OrderStatus>> getStatusInfo(int statusId, int orderid, string lang = "en")
        {
            try
            {
              
                var statusList = await uow.OrderStatusRepository.Get().OrderBy(s => s.Label).ToListAsync();

                foreach (var sts in statusList)
                {
                    sts.Name = lang == "en" ? sts.Name : sts.NameAr;
                    sts.PublicName = lang == "en" ? sts.PublicName : sts.PublicNameAr;
                }
                if (statusId == 7)
                {
                    foreach (var status in statusList)
                    {

                        status.StatusCondition = false;
                        var orderdate = await uow.OrderMasterStatusLogRepository.Get().Where(s => s.OrderId == orderid && s.CurrentInvoiceId == status.OrderStatusId ).Select(s => s.DateTime).FirstOrDefaultAsync();
                        var datedate = new DateTime();

                        if (orderdate != datedate)
                        {
                            status.statusDate = orderdate.ToString("ddd, dd MMM yyyy");

                        }
                        else
                        {
                            if (status.OrderStatusId == 1)
                            {
                                status.statusDate = (await uow.OrderRequestProductRepository.Get().Where(s => s.OrderRequestProductId == orderid).Select(s => s.CreateDate).FirstOrDefaultAsync()).ToString("ddd, dd MMM yyyy");

                            }

                        }

                        if (status.OrderStatusId == statusId)
                        {
                            status.StatusCondition = true;

                        }

                    }
                }
                else
                {
                    foreach (var singlestatus in statusList)
                    {

                        singlestatus.StatusCondition = false;
                        var orderdate = await uow.OrderMasterStatusLogRepository.Get().Where(s => s.OrderId == orderid && s.CurrentInvoiceId == singlestatus.OrderStatusId).Select(s => s.DateTime).FirstOrDefaultAsync();
                        var datedate = new DateTime();

                        if (orderdate != datedate)
                        {
                            singlestatus.statusDate = orderdate.ToString("ddd, dd MMM yyyy");

                        }
                        else
                        {
                            if (singlestatus.OrderStatusId == 1)
                            {
                                singlestatus.statusDate = (await uow.OrderRequestProductRepository.Get().Where(s => s.OrderRequestProductId == orderid).Select(s => s.CreateDate).FirstOrDefaultAsync()).ToString("ddd, dd MMM yyyy");

                            }

                        }


                        if (singlestatus.OrderStatusId == statusId)
                        {
                            singlestatus.StatusCondition = true;

                        }
                    }
                }



                return statusList.Where(s => s.Status == "Active").ToList();




            }
            catch (Exception)
            {

                return new List<OrderStatus>();
            }

        }

    }
}