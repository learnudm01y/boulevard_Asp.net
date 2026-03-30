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
    public class FeatureCategoryController : BaseController
    {
        private FeatureCategoryServiceAccess _fService;
        public FeatureCategoryController()
        {
            _fService = new FeatureCategoryServiceAccess();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllFeatureCategory(string lang="en")
        {
            var result = await _fService.GetAll(lang);

            return SuccessMessage(result);
        }

    }
}
