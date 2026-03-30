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
    public class CityController : BaseController
    {
        public CityAccess _cityAccess;
        public CityController()
        {
            _cityAccess = new CityAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetAllCities(string lang="en")
        {
            var result = await _cityAccess.GetAll(lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetCitybyId(int cityId,string lang="en")
        {
            var result = await _cityAccess.GetById(cityId, lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetCitiesByCountryId(int countryId, string lang = "en")
        {
            var result = await _cityAccess.GetCitiesByCountryId(countryId, lang);
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
