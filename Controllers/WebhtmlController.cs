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
    public class WebhtmlController : BaseController
    {
        public WebHtmlServiceAccess _webHtmlService;
        public WebhtmlController()
        {
            _webHtmlService = new WebHtmlServiceAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetByPageIdentifier(string PageIdentifier, int featureecategoryid=0,string lang="en")
        {
            var result = await _webHtmlService.GetAll(PageIdentifier, featureecategoryid,lang);
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
