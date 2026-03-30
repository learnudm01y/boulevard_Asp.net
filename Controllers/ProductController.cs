using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class ProductController : BaseController
    {
        public ProductServiceAccess _productService;
        public ProductController()
        {
            _productService = new ProductServiceAccess();
        }

        public async Task<IHttpActionResult> GetProductDetails(int ProductId, int memberId = 0,string lang="en")
        {
            try
            {
                var result = await _productService.getProductDetails(ProductId,memberId,lang);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<IHttpActionResult> GetProductSearch(int featureCategoryId, string keyword = "", int size = 0, int count = 0,int memberId=0, string lang = "en")
        {

            var categories = await _productService.GetProductBySearching(featureCategoryId, keyword, size, count, memberId,lang);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetBestSellingProducts(int featureCategoryId, int size = 0, int count = 0,int memberId=0, string lang = "en")
        {

            var categories = await _productService.BestSellingProducts(featureCategoryId, size, count, memberId,lang);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetRelatedProducts(int featureCategoryId,int productId, int size = 0, int count = 0, int memberId = 0,string lang = "en")
        {

            var categories = await _productService.RelatedProducts(featureCategoryId,productId, size, count,memberId,lang);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetMoreProducts(int featureCategoryId,int productId, int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {

            var categories = await _productService.AddMoreProducts(featureCategoryId,productId, size, count,memberId,lang);

            if (categories != null)
            {
                return SuccessMessage(categories);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetallProductTags(int featureCategoryId)
        {

            var tags = await _productService.GetProductTags(featureCategoryId);

            if (tags != null)
            {
                return SuccessMessage(tags);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetallProductBytag(int featureCategoryId, int CommonproducttagId, int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {

            var tags = await _productService.GettagsProduct(featureCategoryId, CommonproducttagId,  size,  count ,  memberId,lang);

            if (tags != null)
            {
                return SuccessMessage(tags);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }




    }
}
