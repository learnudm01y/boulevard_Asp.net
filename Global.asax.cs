using Boulevard.App_Start;
using Boulevard.Areas.Admin.Data;
using Newtonsoft.Json;
using OfficeOpenXml;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Boulevard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Logger();
           // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {

            try
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {

                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    var serializeModel = JsonConvert.DeserializeObject<LoginDetailsViewModel>(authTicket.UserData);
                    CustomPrincipal principal = new CustomPrincipal(authTicket.Name);
                    principal.UserId = serializeModel.UserId;
                    principal.Name = serializeModel.Name;
                    principal.Email = serializeModel.Email;
                    principal.Mobile = serializeModel.Mobile;
                    principal.Image = serializeModel.Image;
                    principal.RoleId = serializeModel.RoleId;
                    HttpContext.Current.User = principal;
                }

            }
            catch (Exception)
            {

               
            }
            
           
        }

        protected void Application_BeginRequest()
        {
            // Allow CORS for all API routes (supports file:// and any origin)
            if (Request.Path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                var origin = Request.Headers["Origin"];
                Response.Headers.Set("Access-Control-Allow-Origin",
                    string.IsNullOrEmpty(origin) ? "*" : origin);
                Response.Headers.Set("Access-Control-Allow-Headers",
                    "Content-Type, Accept, Authorization, X-Requested-With");
                Response.Headers.Set("Access-Control-Allow-Methods",
                    "GET, POST, PUT, DELETE, OPTIONS");

                if (Request.HttpMethod == "OPTIONS")
                {
                    Response.StatusCode = 200;
                    Response.End();
                    return;
                }
            }
        }

        public void Logger()
        {
            var filePath = HttpRuntime.AppDomainAppPath;
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File(filePath + "Log//log.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();
        }
    }
}
