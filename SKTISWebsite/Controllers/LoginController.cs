using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Account;
using AutoMapper;
using System.Web.Configuration;

namespace SKTISWebsite.Controllers
{
    public class LoginController : BaseController
    {
        private IUserBLL _userBll;
        private IUtilitiesBLL _utilBll;
        private IFormsAuthentication _formsAuthentication;
        private bool _debug;

        public LoginController(IUserBLL userBll, IUtilitiesBLL utilBLL, IFormsAuthentication formsAuthentication)
        {
            _userBll = userBll;
            _utilBll = utilBLL;
            _formsAuthentication = formsAuthentication;
            _debug = Convert.ToBoolean(WebConfigurationManager.AppSettings["debug"]);
        }

        /// <summary>
        /// index login
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// post login
        /// </summary>
        /// <param name="model">the usermodel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(UserModel model)
        {
            MstADTemp user = _userBll.GetLogin(model.UserAD);

            if (user != null)
            {
                UserSession login =  new UserSession();
                login.Username = user.UserAD;
                login.Name = user.Name;
                var listRes = _utilBll.GetListResponsibility(user.UserAD);
                if (listRes.Count() > 0)
                {
                    var res = _utilBll.GetResponsibilityPage(listRes[0].IDResponsibility);
                    if (listRes.Count() == 1)
                    {
                        login.Responsibility = res;
                    }
                    else
                    {
                        login.Responsibility = new Responsibility();
                    }
                    login.Location = Mapper.Map<List<UserSessionLocation>>(res.Location);
                }
                else
                {
                    login.Responsibility = new Responsibility();
                    login.Location = null;
                }
                Session["CurrentUser"] = login;
                _formsAuthentication.SignIn(model.UserAD, true);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Username incorrect";
            return PartialView("Index");
        }

        /// <summary>
        /// logout
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Logout()
        {
            Session.Clear();

            if (_debug)
            {
                _formsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
	}
}