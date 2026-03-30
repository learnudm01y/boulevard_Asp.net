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
    public class OfferController : BaseController
    {
        public OfferServiceAccess _offerService;
        public OfferController()
        {
            _offerService = new OfferServiceAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetTrandingBrandOffer(int featureecategoryid = 0, int size=10 ,int count=0,string lang="en")
        {
            var result = await _offerService.TrendingOfferBrands(featureecategoryid, size, count,lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("No data found",result);
            }
        }

        public async Task<IHttpActionResult> GetTrandingProductOffer(int featureecategoryid = 0, int size = 10, int count = 0,int memberid = 0, string lang = "en")
        {
            var result = await _offerService.TrendingOfferproducts(featureecategoryid, size, count,memberid, lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("No data found", result);
            }
        }


        public async Task<IHttpActionResult> GetTrandingcategoryOffer(int featureecategoryid = 0, int size = 10, int count = 0, string lang = "en")
        {
            var result = await _offerService.TrendingOfferCategory(featureecategoryid, size, count,lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("No data found", result);
            }
        }

        public async Task<IHttpActionResult> GetTrandingcategoryOfferServices(int featureecategoryid = 0, int size = 10, int count = 0, string lang = "en")
        {
            var result = await _offerService.TrendingOfferCategoryService(featureecategoryid, size, count,lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("No data found", result);
            }
        }

        public async Task<IHttpActionResult> GetTrandingServiceOffer(int featureecategoryid = 0, int size = 10, int count = 0, int memberid = 0, string lang = "en")
        {
            var result = await _offerService.TrendingOfferServices(featureecategoryid, size, count,memberid,lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("No data found", result);
            }
        }

    }
}
