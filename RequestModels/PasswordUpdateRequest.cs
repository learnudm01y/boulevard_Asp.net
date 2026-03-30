using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boulevard.RequestModels
{
    public class PasswordUpdateRequest
    {
        public int memberId { get; set; }
        public string OldPassword { get; set; }

       
        public string Password { get; set; }

       
        public string ConfirmPassword { get; set; }
    }
}