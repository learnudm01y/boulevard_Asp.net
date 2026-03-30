using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boulevard.Areas.Admin.Data
{
    public class LoginDetailsViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Image { get; set; }
        public int RoleId { get; set; }
    }
}