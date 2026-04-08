using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using Boulevard.Service.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Services.Description;
using System.Xml.Linq;

namespace Boulevard.Service.Admin
{
    public class OrderRequestProductDataAccess
    {
        public IUnitOfWork uow;

        public OrderRequestProductDataAccess()
        {
            uow = new UnitOfWork();
        }

        /// <summary>
        /// Get All from Country
        /// </summary>
        /// <returns></returns>
        public async Task<List<OrderRequestProductResponse>> GetAll()
        {
            try
            {
                var list = new List<OrderRequestProductResponse>();
                var orderRequest = await uow.OrderRequestProductRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.OrderRequestProductId).ToListAsync();
                foreach(var orderRequestProduct in orderRequest) 
                {
                    var Result = new OrderRequestProductResponse();
                    Result.OrderRequestProductId = orderRequestProduct.OrderRequestProductId;
                    Result.ReadableOrderId = orderRequestProduct.ReadableOrderId;
                    Result.OrderDateTime = orderRequestProduct.OrderDateTime;
                    Result.DeliveryDateTime = orderRequestProduct.DeliveryDateTime;
                    Result.Comments = orderRequestProduct.Comments;
                    Result.DeliveryCharge = orderRequestProduct.DeliveryCharge;
                    Result.TotalPrice = orderRequestProduct.TotalPrice;
                    Result.Member = await uow.MemberRepository.Get().FirstOrDefaultAsync(s => s.MemberId == orderRequestProduct.MemberId);
                    Result.MemberAddresses = await uow.MemberAddressRepository.Get().FirstOrDefaultAsync(a => a.MemberAddressId == orderRequestProduct.MemberAddressId);
                    Result.PaymentMethod = await uow.PaymentMethodRepository.Get().FirstOrDefaultAsync(a => a.PaymentMethodId == orderRequestProduct.PaymentMethodId);
                    Result.OrderStatus = await uow.OrderStatusRepository.Get().FirstOrDefaultAsync(a => a.OrderStatusId == orderRequestProduct.OrderStatusId);
                    Result.OrderStatusName = Result.OrderStatus.Name;
                    Result.OrderStatusId = Result.OrderStatus.OrderStatusId;
                    list.Add(Result);
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<List<OrderRequestProductResponse>> GetAllByFeatureCategory(string fCatagoryKey)
        {
            try
            {
                var list = new List<OrderRequestProductResponse>();
                var orderRequest = new List<OrderRequestProduct>();
                if (!string.IsNullOrEmpty(fCatagoryKey))
                {
                    var fCatagory = await uow.FeatureCategoryRepository.Get().FirstOrDefaultAsync(a => a.FeatureCategoryKey.ToString() == fCatagoryKey);
                    orderRequest = await uow.OrderRequestProductRepository.GetAll().Where(a => a.Status == "Active" && a.FeatureCategoryId == fCatagory.FeatureCategoryId).OrderByDescending(t => t.OrderRequestProductId).ToListAsync();
                }
                else
                {
                    orderRequest = await uow.OrderRequestProductRepository.GetAll().Where(a => a.Status == "Active").OrderByDescending(t => t.OrderRequestProductId).ToListAsync();
                }
                foreach (var orderRequestProduct in orderRequest) 
                {
                    var Result = new OrderRequestProductResponse();
                    Result.OrderRequestProductId = orderRequestProduct.OrderRequestProductId;
                    //Result.key = orderRequestProduct.OrderRequestProductId;
                    Result.ReadableOrderId = orderRequestProduct.ReadableOrderId;
                    Result.OrderDateTime = orderRequestProduct.OrderDateTime;
                    Result.DeliveryDateTime = orderRequestProduct.DeliveryDateTime;
                    Result.Comments = orderRequestProduct.Comments;
                    Result.DeliveryCharge = orderRequestProduct.DeliveryCharge;
                    Result.TotalPrice = orderRequestProduct.TotalPrice;
                    Result.Member = await uow.MemberRepository.Get().FirstOrDefaultAsync(s => s.MemberId == orderRequestProduct.MemberId);
                    Result.MemberAddresses = await uow.MemberAddressRepository.Get().FirstOrDefaultAsync(a => a.MemberAddressId == orderRequestProduct.MemberAddressId);
                    Result.PaymentMethod = await uow.PaymentMethodRepository.Get().FirstOrDefaultAsync(a => a.PaymentMethodId == orderRequestProduct.PaymentMethodId);
                    Result.OrderStatus = await uow.OrderStatusRepository.Get().FirstOrDefaultAsync(a => a.OrderStatusId == orderRequestProduct.OrderStatusId);
                    Result.OrderStatusName = Result.OrderStatus.Name;
                    Result.OrderStatusId = Result.OrderStatus.OrderStatusId;
                    list.Add(Result);
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        public async Task<OrderRequestProduct> Details(int id)
        {
            try
            {
                
                var orderRequest = await uow.OrderRequestProductRepository.Get().Where(a => a.OrderRequestProductId == id).Include(s=>s.Member).Include(s=>s.PaymentMethod).Include(s=>s.FeatureCategory).FirstOrDefaultAsync();
                if (orderRequest.MemberAddressId != null && orderRequest.MemberAddressId > 0)
                {
                    var memberAddress = await uow.MemberAddressRepository.Get().Where(a => a.MemberAddressId == orderRequest.MemberAddressId).FirstOrDefaultAsync();
                    if (memberAddress.CityId > 0)
                    {
                        memberAddress.City = await uow.CityRepository.Get().Where(s => s.CityId == memberAddress.CityId).FirstOrDefaultAsync();

                    }

                    if (memberAddress.CountryId > 0)
                    {
                        memberAddress.Country = await uow.CountryRepository.Get().Where(s => s.CountryId == memberAddress.CountryId).FirstOrDefaultAsync();

                    }
                    orderRequest.MemberAddresses = memberAddress != null ? memberAddress : new MemberAddress();
                }

                if (orderRequest.OrderStatusId != null && orderRequest.OrderStatusId > 0)
                {
                    var orderStatus = await uow.OrderStatusRepository.Get().FirstOrDefaultAsync(a => a.OrderStatusId == orderRequest.OrderStatusId);
                    orderRequest.OrderStatus = orderStatus != null ? orderStatus : new OrderStatus();

                   
                        orderRequest.OrderStatushistory = await new ProductServiceAccess().getStatusInfo(orderRequest.OrderStatusId.Value, orderRequest.OrderRequestProductId, "en");
                    
                  
                }
                 var OrderRequestProductDetails = await uow.OrderRequestProductDetailsRepository.GetAll(a => a.OrderRequestProductId == id).OrderByDescending(s => s.OrderRequestProductDetailsId).ToListAsync();
                if (OrderRequestProductDetails != null && OrderRequestProductDetails.Count()>0)
                {
                    foreach (var details in OrderRequestProductDetails)
                    {
                        var product = await uow.ProductRepository.Get().FirstOrDefaultAsync(s => s.ProductId == details.ProductId);
                        details.Product = product != null ? product : new Product();
                        if (product != null)
                        {
                            if (product.BrandId > 0)
                            {
                                var brand = await uow.BrandRepository.Get()
                                    .Where(s => s.BrandId == product.BrandId)
                                    .Select(s => new { s.Title, s.IsDeliveryEnabled })
                                    .FirstOrDefaultAsync();
                                if (brand != null)
                                {
                                    details.BrandName = brand.Title;
                                    details.IsDeliveryEnabled = brand.IsDeliveryEnabled;
                                }
                            }
                        }
                       
                        if (details.OfferInformationId != null && details.OfferInformationId > 0)
                        {
                            var offerInfo = await uow.OfferInformationRepository.Get().FirstOrDefaultAsync(o => o.OfferInformationId == details.OfferInformationId);
                            if (offerInfo != null)
                            {
                                details.OfferName = offerInfo.Title;

                                details.IsOffer = true;
                                var offerDiscount = await uow.OfferDiscountRepository.Get().Where(s => s.OfferInformationId == details.OfferInformationId).FirstOrDefaultAsync();
                                if (offerDiscount != null)
                                {

                                    details.OfferDiscountType = offerDiscount.DiscountType;
                                   
                                    details.DiscountAmount = offerDiscount.DiscountAmount;
                                }
                            }
                        }
                        else if (details.MembershipId != null && details.MembershipId > 0)
                        {

                            var memberShip = await uow.MemberShipRepository.Get().Where(a => a.MemberShipId == details.MembershipId).FirstOrDefaultAsync();
                            if (memberShip != null)
                            {
                                details.MemberShipName = memberShip.Title != null ? memberShip.Title : null;

                                details.IsMemberShipOffer = true;
                            }
                        }

                        orderRequest.OrderRequestProduxtDetails.Add(details);
                    }


                }
                return orderRequest;
            }
            catch (Exception ex)
            {
                return new OrderRequestProduct();
            }
        }

        public async Task UpdateStatus(List<Int64> productIds, int statusId)
        {
            try
            {
                foreach (var property in productIds)
                {
                    var db = new BoulevardDbContext();
                    var result = await uow.OrderRequestProductRepository.GetAll(a => a.OrderRequestProductId == property).FirstOrDefaultAsync();

                    result.OrderStatusId = statusId;
                    //db.Entry(result).State = EntityState.Modified;
                    //db.SaveChanges();

                    await uow.OrderRequestProductRepository.Edit(result);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}