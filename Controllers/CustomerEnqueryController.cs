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
    public class CustomerEnqueryController : BaseController
    {
        public CustomerEnqueryServiceAccess _enqueryService;
        public CustomerEnqueryController()
        {
            _enqueryService = new CustomerEnqueryServiceAccess();
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddCustomerenquery(CustomerEnquery request)
        {
            var result = await _enqueryService.CreateEnquiry(request);
            if (result ==true)
            {
                return SuccessMessage(result);
            }
            else
            {
                return ErrorMessageNull("No data found",result);
            }
        }

      
        public async Task<IHttpActionResult> GetCustomerEnquery(int userId)
        {
            var result = await _enqueryService.GetEnqueries(userId);
           
                return SuccessMessage(result);
            
            
        }

    }
}
