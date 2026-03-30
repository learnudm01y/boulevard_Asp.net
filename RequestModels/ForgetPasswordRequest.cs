using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class ForgetPasswordRequest
    {
        public string Email { get; set; } = "";
        public int OTP { get; set; } = 0;
        public string Password { get; set; } = "";
    }
}