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
    public class PaymentMethodController : BaseController
    {
        public PaymentMethodServiceAccess _paymentService;
        public PaymentMethodController()
        {
            _paymentService = new PaymentMethodServiceAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetAllPaymentMethod(string lang="en")
        {
            var result = await _paymentService.getallpayment(lang);
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
