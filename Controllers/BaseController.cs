using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Boulevard.Controllers
{
    public class BaseController : ApiController
    {
        public IHttpActionResult ErrorMessage(string message)
        {
            return Content(HttpStatusCode.OK, new
            {
                result = new List<object>(),
                code = HttpStatusCode.NotAcceptable,
                message = message,
                isSuccess = false
            });
            ;
        }

        public IHttpActionResult ErrorMessageNull(string message, dynamic data)
        {
            return Content(HttpStatusCode.OK, new
            {
                result = data,
                code = HttpStatusCode.NotAcceptable,
                message = message,
                isSuccess = false
            });
            ;
        }

        public IHttpActionResult SuccessMessage(dynamic data)
        {
            return Content(HttpStatusCode.OK, new
            {
                result = data,
                code = HttpStatusCode.OK,
                message = "success",
                isSuccess = true
            });
        }

        public IHttpActionResult SuccessMessage(dynamic data, string message)
        {
            return Content(HttpStatusCode.OK, new
            {
                result = new
                {
                    Result = data
                },
                code = HttpStatusCode.OK,
                message = message,
                isSuccess = true
            });
        }

        public IHttpActionResult NoDataFoundMessage(dynamic data)
        {
            return Content(HttpStatusCode.OK, new
            {
                result = new
                {
                    Result = data
                },
                code = HttpStatusCode.OK,
                message = "No Data Found",
                isSuccess = true
            });
        }

        public IHttpActionResult InternelServerError()
        {
            return Content(HttpStatusCode.OK, new
            {
                result = new { Result = new { } },
                code = HttpStatusCode.InternalServerError,
                message = "An error occured, please try again later!",
                isSuccess = false
            });
        }
    }
}
