using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Boulevard.App_Start
{
    public class CustomPrincipal : IPrincipal
    {
        #region Identity Properties

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public int RoleId { get; set; }

        #endregion

        public IIdentity Identity
        {
            get; private set;
        }


        public CustomPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}