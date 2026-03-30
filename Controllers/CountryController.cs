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
    public class CountryController : BaseController
    {
        public CountryAccess _countryAccess;
        public CountryController()
        {
            _countryAccess = new CountryAccess();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetAllCountries(string lang="en")
        {
            var result = await _countryAccess.GetAll(lang);
            if (result != null)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessage("No data found");
            }
        }

        public async Task<IHttpActionResult> GetCountryById(int countryId, string lang = "en")
        {
            var result = await _countryAccess.GetById(countryId, lang);
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
