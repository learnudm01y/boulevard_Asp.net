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
    public class AirportController : BaseController
    {
        public AirportAccess _airportAccess;
        public AirportController()
        {
            _airportAccess = new AirportAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetAllAirports()
        {
            var result = await _airportAccess.GetAll();
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetAirportById(int airportId)
        {
            var result = await _airportAccess.GetById(airportId);
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
