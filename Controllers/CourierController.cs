using Boulevard.RequestModels;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Boulevard.Controllers
{
    public class CourierController : BaseController
    {
        public CourierService _coAccess;
        public CourierController()
        {
            _coAccess = new CourierService();
        }
        // GET: WebHtml
       
        public async Task<IHttpActionResult> PostCourier(int orderId)
        {
            var result = await _coAccess.CreateExpressShipmentAsync(orderId);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateCourierStatus(
        [FromBody] CourierOrderStatusResponse request)
        {
            try
            {
                // Check API Key
                var apiKey = Request.Headers.GetValues("X-API-KEY")?.FirstOrDefault();
                if (apiKey != "Jeebly123")
                    return BadRequest("Invalid API Key");

                var result = await _coAccess.UpdateCourierOrderAsync(request);
                if (result ==true)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("did not Updated");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        public async Task<IHttpActionResult> CancelOrderForCourier(int orderId)
        {
            var result = await _coAccess.CancelExpressShipmentAsync(orderId);
            if (result == true)
            {
                return SuccessMessage(result);
            }
            else
            {
                return BadRequest("Cancelation not success");
            }
        }

    }
}
