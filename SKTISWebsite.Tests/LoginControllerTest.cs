using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SKTISWebsite.Code;
using SKTISWebsite.Controllers;
using SKTISWebsite.Models.Account;

namespace SKTISWebsite.Tests
{
    [TestClass]
    public class LoginControllerTest
    {

        private IFormsAuthentication _formsAuthentication;
        private IUserBLL _userBll;
        private IUtilitiesBLL _utilBll;
        private IIdentity _identity;
        private LoginController _loginController;
        private HttpContextBase _httpContextBase;
        private ControllerContext _controllerContext;

        [TestInitialize]
        public void Setup()
        {
            _userBll = Substitute.For<IUserBLL>();
            _utilBll = Substitute.For<IUtilitiesBLL>();
            _identity = Substitute.For<IIdentity>();
            _formsAuthentication = Substitute.For<IFormsAuthentication>();
            _httpContextBase = Substitute.For<HttpContextBase>();
            _controllerContext = Substitute.For<ControllerContext>();

        }

        [TestMethod]
        public void LoginIndexReturnCorrectView()
        {
            // Arrange
            _loginController = new LoginController(_userBll, _utilBll, _formsAuthentication);

            // Act
            ViewResult result = _loginController.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "Index");
        }

        [TestMethod]
        public void LoginIndexPostSuccessful_ReturnCorrectView()
        {
            ////Arrange
            //_loginController = new LoginController(_userBll, _formsAuthentication);
            //UserModel user = new UserModel
            //{
            //    UserAD = "Oka",
            //    Role = "Admin"
            //};
            //MstADTemp login = new MstADTemp
            //{
            //    UserAD = "Oka",
            //    Email = "Admin@vox.com"
            //};
            //var fakeIdentity = new GenericIdentity("Oka");
            //var principal = new GenericPrincipal(fakeIdentity, null);

            //// Act
            //_userBll.GetLogin(user.UserAD).Returns(login);
            //_httpContextBase.User.Returns(principal);
            //_controllerContext.HttpContext.Returns(_httpContextBase);
            //_loginController.ControllerContext = _controllerContext;
            //var result = _loginController.Index(user) as RedirectToRouteResult;

            //// Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual("Index", result.RouteValues["action"]);
            //Assert.AreEqual("Home", result.RouteValues["Controller"]);
        }

        [TestMethod]
        public void LoginIndexPost_WrongLogin()
        {
            //// Arrange
            //Login login = null;
            //UserModel user = new UserModel
            //{
            //    UserAD = "Oka",
            //    Role = "Admin"
            //};
            //_loginController = new LoginController(_userBll, _formsAuthentication);

            //// Act
            //_userBll.GetLogin(Arg.Any<string>()).Returns(login);
            //var result = _loginController.Index(user) as PartialViewResult;

            //// Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual("Index", result.ViewName);
            //Assert.AreEqual("Username incorrect", result.ViewBag.Error);

        }
    }
}
