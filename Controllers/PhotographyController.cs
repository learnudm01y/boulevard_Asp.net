using Boulevard.Service.WebAPI;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class PhotographyController : BaseController
    {
        // FeatureCategoryId=22 (Photography banners are assigned to this ID in the DB)
        private const int PHOTOGRAPHY_FC_ID = 22;

        private readonly WebHtmlServiceAccess _webHtmlService;

        public PhotographyController()
        {
            _webHtmlService = new WebHtmlServiceAccess();
        }

        // GET api/v1/photography/banners
        public async Task<IHttpActionResult> GetBanners(string PageIdentifier = "", string lang = "en")
        {
            var result = await _webHtmlService.GetAll(PageIdentifier, PHOTOGRAPHY_FC_ID, lang);
            if (result != null)
                return SuccessMessage(result);
            return ErrorMessage("No data found");
        }
    }
}
