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
    public class BrandController : BaseController
    {
        public BrandServiceAccess _brandService;
        public BrandController()
        {
            _brandService = new BrandServiceAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetBrandAll(int featureecategoryid = 0,string type="All", bool isFeature = false, bool isTrending = false)
        {
            var result = await _brandService.GetAll( featureecategoryid, type,isFeature,isTrending);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetBrandProducts(int brandId,string keyword="", int size = 0, int count = 0, int memberId = 0)
        {
            var result = await _brandService.GetBrandWithProduct(brandId, keyword,size, count,memberId);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        
    }
}
