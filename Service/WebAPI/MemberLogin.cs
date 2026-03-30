using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class MemberLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirebaseToken { get; set; }
    }
}