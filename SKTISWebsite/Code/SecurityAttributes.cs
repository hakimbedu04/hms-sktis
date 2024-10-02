using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Code
{
    public class SecurityAttributes
    {
        /// <summary>
        /// allow to deny access to logged in users, show only to anonymous users
        /// </summary>
        public class DenyNotAnonymousAttribute : AuthorizeAttribute
        {
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                return !base.AuthorizeCore(httpContext);
            }
        }
    }
}