using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Boulevard.App_Start
{
    public class WebConfig
    {
        public static string BaseUrl => ConfigurationManager.AppSettings["baseUrl"];
    }
}