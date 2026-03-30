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
    public class ProductTypeController : BaseController
    {
        public ProductTypeService _airportAccess;
        public ProductTypeController()
        {
            _airportAccess = new ProductTypeService();
        }
        // GET: WebHtml
        public async Task<IHttpActionResult> GetAllProductTypes(string lang="en")
        {
            var result = await _airportAccess.GetAllProductTypesAsync(lang);
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
