using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class MemberRequest
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
      
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string PhoneCode { get; set; }
        public string Password { get; set; }
        public string RetypePassword { get; set; }

        public string Image { get; set; }

        public string MemberFireBaseToken { get; set; }

        public bool ThirdPartyLogin { get; set; }


        public string ThirdPartyLoginKey { get; set; }


        public string ThirdPartyLoginFrom { get; set; }
    }
}