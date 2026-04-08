using Boulevard.Service.WebAPI;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class LaundryController : BaseController
    {
        // FeatureCategoryId=21 (Laundry banners are assigned to this ID in the DB)
        private const int LAUNDRY_FC_ID = 21;

        private readonly WebHtmlServiceAccess _webHtmlService;

        public LaundryController()
        {
            _webHtmlService = new WebHtmlServiceAccess();
        }

        // GET api/v1/laundry/banners
        public async Task<IHttpActionResult> GetBanners(string PageIdentifier = "", string lang = "en")
        {
            var result = await _webHtmlService.GetAll(PageIdentifier, LAUNDRY_FC_ID, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }
    }
}
