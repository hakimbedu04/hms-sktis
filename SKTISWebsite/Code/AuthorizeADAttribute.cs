using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HMS.SKTIS.BLL;
using HMS.SKTIS.BusinessObjects;

namespace SKTISWebsite.Code
{
    public class AuthorizeADAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentException("httpContext");
            }

            //IPrincipal user = httpContext.User;
            UserBLL bll = DependencyResolver.Current.GetService<UserBLL>();
            MstADTemp login = bll.GetLogin(httpContext.User.Identity.Name);
            if (login == null)
            {
                return false;
            }

            CustomPrincipal principal = new CustomPrincipal(httpContext.User.Identity.Name);
            principal.Username = login.Name;
            HttpContext.Current.User = principal;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "Error", action = "UserNotAuthorized" }));
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "UserNotRegistered" }));
            }
        }


    }
}