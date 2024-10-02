using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Account;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.UtilSecurityResponsibilities;
using HMS.SKTIS.BLL;
using System.IO;
using HMS.SKTIS.BLL.UtilitiesBLL;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace SKTISWebsite.Controllers
{
    //[AuthorizeAD]
    public class BaseController : Controller
    {
        public List<string> ButtonAccess = new List<string>();
        protected int PageID;
        protected string Page = null;
        protected string Ignore = null;
        public void SetPage(string Page) {
            this.Page = Page;
        }
        public int GetPageID() {
            var res = CurrentUser.Responsibility;
            if (res.Page == null || this.Page == null)
                return 0;
            var page = this.Page.Split('/');
            if (page.Length == 1)
            {
                if (res.Page.ContainsKey(this.Page)) {
                    return res.Page[this.Page].PageID;
                }
            }
            else
            {
                Dictionary<string, ResponsibilityPage> data = (Dictionary<string, ResponsibilityPage>)res.Page;

                for (var i = 0; i < (page.Length - 1); i++)
                {
                    if (!data.ContainsKey(page[i]))
                    {
                        return 0;
                    }
                    data = data[page[i]].Child;
                }
                if (data.ContainsKey(page.Last()))
                {
                    return data[page.Last()].PageID;
                }
            }
            return 0;
        }
        public UserSession CurrentUser
        {
            get
            {
                if (Session["CurrentUser"] == null)
                {
                    setCurrentUser();
                }
                var currentUserFromSession = Session["CurrentUser"] as UserSession;
                if (currentUserFromSession != null)
                {
                    if (!currentUserFromSession.Username.Equals(User.Identity.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        setCurrentUser();
                    }
                }
                return (UserSession)Session["CurrentUser"];

                //return new UserSession()
                //{
                //    Name = (HttpContext.User as CustomPrincipal) != null ? (HttpContext.User as CustomPrincipal).Identity.Name : "pmi\admin",
                //    Location = new UserSessionLocation() { Code = "SKT", Name = "SKT" }
                //};
            }
        }
        public string GetUserName() {
            return CurrentUser.Username;
        }
        public void setCurrentUser() {
            UserBLL bll = DependencyResolver.Current.GetService<UserBLL>();
            UtilitiesBLL util = DependencyResolver.Current.GetService<UtilitiesBLL>();
            MstADTemp user = bll.GetLogin(User.Identity.Name);
            UserSession login = null;
            if (user != null) {
                login = new UserSession();
                login.Name = user.Name;
                login.Username = user.UserAD;
                var listRes = util.GetListResponsibility(user.UserAD);
                if(listRes.Count() > 0){
                    var res = util.GetResponsibilityPage(listRes[0].IDResponsibility);
                    if (listRes.Count() == 1)
                    {
                        login.Responsibility = res;
                    }
                    else {
                        login.Responsibility = new Responsibility();                        
                    }
                    login.Location = Mapper.Map<List<UserSessionLocation>>(res.Location);
                }else{
                    login.Responsibility = new Responsibility();
                    login.Location = null;
                }
                
            }
            Session["CurrentUser"] = login;
        }

        public void setResponsibility(int IDResponsibility)
        {
            UtilitiesBLL util = DependencyResolver.Current.GetService<UtilitiesBLL>();
            var user = CurrentUser;
            var listRes = util.GetListResponsibility(user.Username).Select(s => s.IDResponsibility).ToList();
            if (!listRes.Contains(IDResponsibility)) {
                RedirectToAction("Index", "Home");
            }
            if(user != null){
                var res = util.GetResponsibilityPage(IDResponsibility);
                user.Responsibility = res;
                user.Location = Mapper.Map<List<UserSessionLocation>>(res.Location);
                Session["CurrentUser"] = user;
            }
        }
        public UserSessionLocation CurrentUserLocation
        {
            get { return CurrentUser.Location[0]; }
        }

        protected void PermissionIgnore(string action) {
            this.Ignore = action;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = CurrentUser;
            var descriptor = filterContext.ActionDescriptor;
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerDescriptor.ControllerName;
            UtilitiesBLL util = DependencyResolver.Current.GetService<UtilitiesBLL>();

            var skipPermission = false;

            if(this.Ignore != null){
                var ignoreAction = Ignore.Split(';').ToList();
                if(ignoreAction.Contains(actionName)){
                    skipPermission = true;
                }
            }

            if (user == null)
            {
                if (controllerName != "Login")
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        // the controller action was invoked with an AJAX request
                        Response.ClearContent();
                        Response.StatusCode = 401;
                        Response.End();
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Error" }, { "action", "UserNotRegistered" } });
                        return;
                    }
                }

            //    if (!User.Identity.IsAuthenticated)
            //    {
            //        return;
            //    }
            //    //else
            //    //{
            //    //    filterContext.Result = new RedirectToRouteResult(
            //    //        new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });
            //    //    return;
            //    //}
            //}

            //if (controllerName != "DistrictSelected" && controllerName != "Login")
            //{
            //    if (user.IsHaveMoreThan1District)
            //    {
            //        if (string.IsNullOrEmpty(user.District))
            //        {
            //            filterContext.Result = new RedirectToRouteResult(
            //                new RouteValueDictionary { { "controller", "DistrictSelected" }, { "action", "Index" } });
            //            return;
            //        }

            //    }

            //}
            //if (user != null)
            //{
            //    if (!_accountBll.GetUserAccesAvailabe(CurrentUser))
            //    {
            //        Session.Clear();
            //        FormsAuthentication.SignOut();
            //        //RedirectToAction("Logout", "Login");
            //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Error" }, { "action", "Unauthorized" } });
            //        return;
            //    }
            //    if (!user.IsAdmin)
            //    {

            //        if (controllerName == "UserLog")
            //        {
            //            filterContext.Result = new RedirectToRouteResult(
            //                new RouteValueDictionary { { "controller", "Error" }, { "action", "Unauthorized" } });
            //            return;
            //        }
            //    }

            }

            if (controllerName != "Login" && controllerName != "Home" && !skipPermission)
            {
                if(user.Responsibility.ResponsibilityName == null){
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        // the controller action was invoked with an AJAX request
                        Response.ClearContent();
                        Response.StatusCode = 401;
                        Response.End();
                    }
                    else
                    {
                        filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Home" }, { "action", "index" } });
                        return;
                    }
                }
                if (GetPageID() == 0)
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "controller", "Error" }, { "action", "UserNotAuthorized" } });
                    return;
                }
                var button = util.GetResponsibilityButton(user.Responsibility.Role, GetPageID());
                ViewBag.ButtonAccess = button.Button;            
            }

            if (controllerName != "Login")
            {
                var listRes = util.GetListResponsibility(user.Username);
                if (listRes.Count == 0)
                {
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary { { "controller", "Error" }, { "action", "UserNotAuthorized" } });
                    return;
                }
                ViewBag.listResponsibility = Mapper.Map<List<UtilSecurityResponsibilitiesViewModel>>(listRes); ;
                ViewBag.ResponName = user.Responsibility.ResponsibilityName;
            }


            base.OnActionExecuting(filterContext);


        }
    }
}