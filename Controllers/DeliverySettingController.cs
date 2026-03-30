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
    public class DeliverySettingController : BaseController
    {
        public DeliverySettingsServiceAccess _deliveryService;
        public DeliverySettingController()
        {
            _deliveryService = new DeliverySettingsServiceAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetDeliverySettings(int featureecategoryid = 0)
        {
            var result = await _deliveryService.GetDeliveryInfo(featureecategoryid);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("No data found",result);
            }
        }
    }
}
