using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Models;
using Boulevard.RequestModels;
using Boulevard.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Razor.Parser;

namespace Boulevard.Service.WebAPI
{
    public class CartServiceAccess
    {
        public IUnitOfWork uow;
        public CartServiceAccess()
        {
            uow = new UnitOfWork();
        }
        public async Task<string> AddOrRemoveCart(CartListRequest request)
        {

            var model = new Cart();
            try
            {
                if (request.MemberId == 0)
                {
                    model = await uow.CartRepository.Get().FirstOrDefaultAsync(x => x.ProductId == request.ProductId  && x.FeatureCategoryId==request.FeatureCategoryId && x.TempId == request.TempId && x.ProductPriceId==request.ProductPriceId);
                }
                else 
                {
                    model = await uow.CartRepository.Get().FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.MemberId == request.MemberId && x.FeatureCategoryId == request.FeatureCategoryId && x.ProductPriceId == request.ProductPriceId);
                }


                if (model != null)
                {
                    //if (model.Quantity == request.Quantity)
                    //{
                    //    db.Carts.Remove(model);
                    //    db.SaveChanges();

                    //    return "Removed from Cart";
                    //}
                    //else 
                    if (request.Quantity == 0)
                    {
                         uow.CartRepository.Remove(model.CartId);

                        return "Removed from Cart";
                    }
                    else
                    {
                        model.Quantity = request.Quantity;
                        await uow.CartRepository.Edit(model);
                        return "Added to Cart";
                    }
                }
                else
                {
                    var ProductsQty = await uow.ProductPriceRepository.Get().Where(s=>s.ProductId== request.ProductId && s.ProductPriceId==request.ProductPriceId).Select(s=>s.ProductStock).FirstOrDefaultAsync();
                    if (ProductsQty < request.Quantity)
                    {
                        return "This Product does not have enough stock to order";
                    }
                    model = new Models.Cart();
                    model.MemberId = request.MemberId;
                    model.TempId = request.TempId;
                    model.ProductId = request.ProductId;
                    model.ProductPriceId = request.ProductPriceId;
                    model.FeatureCategoryId = request.FeatureCategoryId;
                    model.Quantity = request.Quantity;
                    model.Status = "Active";
                    model.LastModified = DateTime.UtcNow;
                    model.IsDelete = false;

                    await uow.CartRepository.Add(model);

                    return "Added to Cart";
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        public async Task<bool> RemoveCart(int memberId,int featureCategoryId)
        {
            
            try
            {
             
                var model = await uow.CartRepository.Get().Where(x => x.MemberId == memberId && x.FeatureCategoryId == featureCategoryId).ToListAsync();
                if (model.Count() == 0)
                {
                    return true;
                }
                else
                {
                    foreach (var item in model)
                    {
                        uow.CartRepository.Remove(item.CartId);
                    }
                }
                return true;

            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<string> AddOrRemoveCartService(CartServiceRequest request)
        {
            CartService model;

            try
            {
                model = await uow.CartServiceRepository.Get()
                    .FirstOrDefaultAsync(x =>
                        x.MemberId == request.MemberId &&
                        x.ServiceId == request.ServiceId &&
                        x.ServiceTypeId == request.ServiceTypeId &&
                        x.FeatureCategoryId == request.FeatureCategoryId);

                if (model != null)
                {
                    if (request.Quantity == 0)
                    {
                        // HARD DELETE (same as your old Cart code)
                        uow.CartServiceRepository.Remove(model.CartServiceId);
                        return "Removed from Cart";
                    }
                    else
                    {
                        model.Quantity = request.Quantity;
                        model.LastModified = DateTime.UtcNow;

                        await uow.CartServiceRepository.Edit(model);
                        return "Added to Cart";
                    }
                }
                else
                {
                    if (request.Quantity == 0)
                        return "Invalid quantity";

                    model = new CartService
                    {
                        MemberId = request.MemberId,
                        ServiceId = request.ServiceId,
                        ServiceTypeId = request.ServiceTypeId,
                        FeatureCategoryId = request.FeatureCategoryId,
                        Quantity = request.Quantity,
                        Status = "Active",
                        LastModified = DateTime.UtcNow,
                        IsDelete = false
                    };

                    await uow.CartServiceRepository.Add(model);
                    return "Added to Cart";
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<bool> RemoveCartService(int memberId, int featureCategoryId)
        {
            try
            {
                var list = await uow.CartServiceRepository.Get()
                    .Where(x => x.MemberId == memberId &&
                                x.FeatureCategoryId == featureCategoryId)
                    .ToListAsync();

                if (!list.Any())
                    return true;

                foreach (var item in list)
                {
                    uow.CartServiceRepository.Remove(item.CartServiceId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<SmallServiceDetailsResponse>> GetCartListServices(int memberId, int featureCategoryId, int serviceId, string lang = "en")
        {

            try
            {
                var result = new List<SmallServiceDetailsResponse>();
                var serviceTypeids = await uow.CartServiceRepository.Get().Where(x => x.MemberId == memberId && x.FeatureCategoryId == featureCategoryId && x.ServiceId == serviceId).Select(s => s.ServiceTypeId).ToListAsync();


                if (serviceTypeids.Count() > 0 || serviceTypeids != null)
                {
                    //var services = await uow.ServiceTypesRepository.Get().Where(s => serviceTypeids.Contains(s.ServiceTypeId) && s.Status == "Active").Select(s => s.ServiceId).Distinct().ToListAsync();

                    //if (services.Count() == 1)
                    //{
                    var ServiceResult = await new ServiceAccess().GetSmallServices(serviceId, 0, memberId, lang, serviceTypeids);
                    if (ServiceResult != null)
                    {
                        result.Add(ServiceResult);
                    }
                    //}
                    //else
                    //{
                    //foreach (var serviceTypeId in serviceTypeids)
                    //{
                    //    var serviceType = await uow.ServiceTypesRepository.Get().Where(s => s.ServiceTypeId == serviceTypeId && s.Status == "Active").FirstOrDefaultAsync();
                    //    if (serviceType != null)
                    //    {
                    //var ServiceResult = await new ServiceAccess().GetSmallServices(serviceId, 0, memberId, lang);
                    //if (ServiceResult != null)
                    //{
                    //    result.Add(ServiceResult);
                    //}
                    //    }
                    //}

                    //}


                }


                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
               
        }


    
        public async Task<List<ProductSmallDetailsResponse>> GetCartListProducts(int memberId,string tempId,int featureCategoryId,string lang="en")
        {
            try
            {
              var result = new List<ProductSmallDetailsResponse>();
                var productIds = await uow.CartRepository.Get().Where(x => x.MemberId == memberId && x.FeatureCategoryId==featureCategoryId).ToListAsync();

                if (productIds!=null && productIds.Count() == 0)
                {
                    return null;
                }

               
                //list = list.ForEach(i=>i.Quantity==db.Carts.Where(s=>s.MemberId && s.ProductId))
                foreach (var pr in productIds)
                {
                    var list = await new ProductServiceAccess().getSmallDetailsProducts(pr.ProductId, memberId, false, lang, pr.ProductPriceId);
                    if (list != null)
                    {
                        //if (list.ProductPrices != null && list.ProductPrices.Count() > 0)
                        //{
                        //    foreach (var ss in list.ProductPrices)
                        //    {
                        //        if (ss.ProductPriceId == pr.ProductPriceId)
                        //        {
                        //            ss.ProductQuantity = pr.Quantity;
                        //        }
                        //    }
                        //}
                        list.Quantity = pr.Quantity;

                        result.Add(list);
                    }

                }
                return result;
            }
            catch (Exception)
            {

                return null;
            }
        }


        public async Task<int> GetCartListProductsCount(int memberId,string tempid,int featureCategoryId)
        {
            try
            {
                int productIds = 0;
                if (memberId == 0)
                {
                    productIds = await uow.CartRepository.Get().AnyAsync(s => s.MemberId == memberId && s.FeatureCategoryId==featureCategoryId) == true ? await uow.CartRepository.Get().Where(x => x.MemberId == memberId && x.FeatureCategoryId == featureCategoryId).CountAsync() : 0;
                }
                else
                {
                    productIds = await uow.CartRepository.Get().AnyAsync(s => s.MemberId == memberId && s.FeatureCategoryId == featureCategoryId) == true ? await uow.CartRepository.Get().Where(x => x.MemberId == memberId && x.FeatureCategoryId == featureCategoryId).CountAsync() : 0;
                }

                return productIds;
            }
            catch (Exception)
            {

                return 0;
            }



        }
    }
}