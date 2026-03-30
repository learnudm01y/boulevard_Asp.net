using Boulevard.Models;
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
    public class FAQController : BaseController
    {
        // GET: FAQ
        private FAQService _faqService;
        public FAQController()
        {
            _faqService = new FAQService();
        }

        public async Task<IHttpActionResult> GetAlFAQ(int featurecategoryId = 0)
        {
            try
            {
                var result = await _faqService.GetAll(featurecategoryId);
                if (result != null)
                {
                    return SuccessMessage(result);
                }
                else
                {
                    return ErrorMessage("No data found");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}