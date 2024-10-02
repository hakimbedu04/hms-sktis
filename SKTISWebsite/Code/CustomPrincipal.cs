using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SKTISWebsite.Code
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }

        public CustomPrincipal(string userAd)
        {
            this.Identity = new GenericIdentity(userAd);
        }

        public string Username { get; set; }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}