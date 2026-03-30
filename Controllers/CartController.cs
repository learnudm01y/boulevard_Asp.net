using Boulevard.RequestModels;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Boulevard.Controllers
{
    public class CartController : BaseController
    {
        public CartServiceAccess _cartService;
        public CartController()
        {
            _cartService = new CartServiceAccess();
        }

       
        public async Task<IHttpActionResult> AddOrRemoveCart(CartListRequest model)
        {
          
           

            var wish = await _cartService.AddOrRemoveCart(model);
            if (wish != null)
            {
                return SuccessMessage(wish, wish);
            }
            else
            {
                return ErrorMessage("An error occured, please try again later!");
                
            }
        }

        
        public async Task<IHttpActionResult> RemoveCart(int memberId,int featureCategoryId)
        {
            var wish = await _cartService.RemoveCart(memberId, featureCategoryId);
            if (wish)
            {
                return SuccessMessage(wish);
            }
            else
            {
                return ErrorMessage("An error occured, please try again later!");

            }
        }




        public async Task<IHttpActionResult> AddOrRemoveCartService(CartServiceRequest model)
        {



            var wish = await _cartService.AddOrRemoveCartService(model);
            if (wish != null)
            {
                return SuccessMessage(wish, wish);
            }
            else
            {
                return ErrorMessage("An error occured, please try again later!");

            }
        }


        public async Task<IHttpActionResult> RemoveCartService(int memberId, int featureCategoryId)
        {
            var wish = await _cartService.RemoveCartService(memberId, featureCategoryId);
            if (wish)
            {
                return SuccessMessage(wish);
            }
            else
            {
                return ErrorMessage("An error occured, please try again later!");

            }
        }

        public async Task<IHttpActionResult> GetCartProducts(int memberId,string tempId, int featureCategoryId,string lang="en")
        {
            var result = await _cartService.GetCartListProducts(memberId, tempId, featureCategoryId);
            return SuccessMessage(result);
        }
        public async Task<IHttpActionResult> GetCartService(int memberId, int featureCategoryId,int serviceId, string lang = "en")
        {
            var result = await _cartService.GetCartListServices(memberId, featureCategoryId, serviceId, lang);
            return SuccessMessage(result);
        }
        


        public async Task<IHttpActionResult> GetCartCount(int memberId, string tempId, int featureCategoryId)
        {
            var result = await _cartService.GetCartListProductsCount(memberId, tempId, featureCategoryId);
            return SuccessMessage(result);
        }

    }
}
