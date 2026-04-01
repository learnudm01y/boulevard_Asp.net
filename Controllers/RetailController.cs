using Boulevard.Service.WebAPI;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class RetailController : BaseController
    {
        private static readonly Guid RETAIL_KEY = new Guid("F1A2B3C4-D5E6-4F70-8B9C-0D1E2F3A4B5C");
        private static int _cachedFcId = 0;

        private readonly ProductServiceAccess _productService;
        private readonly CategoryServiceAccess _categoryService;
        private readonly BrandServiceAccess _brandService;
        private readonly WebHtmlServiceAccess _webHtmlService;
        private readonly OfferServiceAccess _offerService;
        private readonly FeatureCategoryServiceAccess _featureCategoryService;

        public RetailController()
        {
            _productService = new ProductServiceAccess();
            _categoryService = new CategoryServiceAccess();
            _brandService = new BrandServiceAccess();
            _webHtmlService = new WebHtmlServiceAccess();
            _offerService = new OfferServiceAccess();
            _featureCategoryService = new FeatureCategoryServiceAccess();
        }

        private async Task<int> GetFeatureCategoryId()
        {
            if (_cachedFcId > 0) return _cachedFcId;
            var all = await _featureCategoryService.GetAll();
            var item = all.FirstOrDefault(x => x.FeatureCategoryKey == RETAIL_KEY);
            _cachedFcId = item != null ? item.FeatureCategoryId : 0;
            return _cachedFcId;
        }

        // GET api/v1/retail/banners
        public async Task<IHttpActionResult> GetBanners(string PageIdentifier = "", string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _webHtmlService.GetAll(PageIdentifier, fcId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/categories
        public async Task<IHttpActionResult> GetCategories(string type = "All", bool isTop = false, bool isTrending = false, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _categoryService.GetParentChildWiseCategories(fcId, type, isTop, isTrending, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/category-products
        public async Task<IHttpActionResult> GetCategoryProducts(int categoryId, string keyword = "", int size = 0, int count = 0, int brandId = 0, int memberId = 0, bool IsScheduled = false, string lang = "en", int productType = 1)
        {
            var result = await _categoryService.GetCategorywiseProduct(categoryId, keyword, size, count, brandId, memberId, IsScheduled, lang, productType);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/parent-category-products
        public async Task<IHttpActionResult> GetParentCategoryProducts(int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _categoryService.GetAllParent(fcId, size, count, memberId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/brands
        public async Task<IHttpActionResult> GetBrands(string type = "All", bool isFeature = false, bool isTrending = false)
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _brandService.GetAll(fcId, type, isFeature, isTrending);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/brand-products
        public async Task<IHttpActionResult> GetBrandProducts(int brandId, string keyword = "", int size = 0, int count = 0, int memberId = 0)
        {
            var result = await _brandService.GetBrandWithProduct(brandId, keyword, size, count, memberId);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/products
        public async Task<IHttpActionResult> GetProducts(string keyword = "", int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _productService.GetProductBySearching(fcId, keyword, size, count, memberId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/product-details
        public async Task<IHttpActionResult> GetProductDetails(int productId, int memberId = 0, string lang = "en")
        {
            var result = await _productService.getProductDetails(productId, memberId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/best-sellers
        public async Task<IHttpActionResult> GetBestSellers(int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _productService.BestSellingProducts(fcId, size, count, memberId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/related-products
        public async Task<IHttpActionResult> GetRelatedProducts(int productId, int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _productService.RelatedProducts(fcId, productId, size, count, memberId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/more-products
        public async Task<IHttpActionResult> GetMoreProducts(int productId, int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _productService.AddMoreProducts(fcId, productId, size, count, memberId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/tags
        public async Task<IHttpActionResult> GetTags()
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _productService.GetProductTags(fcId);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/products-by-tag
        public async Task<IHttpActionResult> GetProductsByTag(int tagId, int size = 0, int count = 0, int memberId = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _productService.GettagsProduct(fcId, tagId, size, count, memberId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }

        // GET api/v1/retail/offers/products
        public async Task<IHttpActionResult> GetTrendingProductOffers(int size = 10, int count = 0, int memberId = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _offerService.TrendingOfferproducts(fcId, size, count, memberId, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessageNull("No data found", result);
        }

        // GET api/v1/retail/offers/brands
        public async Task<IHttpActionResult> GetTrendingBrandOffers(int size = 10, int count = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _offerService.TrendingOfferBrands(fcId, size, count, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessageNull("No data found", result);
        }

        // GET api/v1/retail/offers/categories
        public async Task<IHttpActionResult> GetTrendingCategoryOffers(int size = 10, int count = 0, string lang = "en")
        {
            int fcId = await GetFeatureCategoryId();
            var result = await _offerService.TrendingOfferCategory(fcId, size, count, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessageNull("No data found", result);
        }
    }
}
