using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class DemoController : BaseController
    {
        public async Task<IHttpActionResult> demoapi(int featureecategoryid = 0, int size = 10, int count = 0)
        {
            var result = "";
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
