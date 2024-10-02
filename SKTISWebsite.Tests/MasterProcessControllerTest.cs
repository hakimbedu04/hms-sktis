using System;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SKTISWebsite.Controllers;
using Voxteneo.WebComponents.Logger;

namespace SKTISWebsite.Tests
{
    /// <summary>
    /// Summary description for MasterProcessController
    /// </summary>
    [TestClass]
    public class MasterProcessControllerTest
    {
        private HttpContextBase _httpContextBase;
        private ControllerContext _controllerContext;
        private IMasterDataBLL _masterDataBLL;
        private ILogger _loger;
        
        [TestInitialize]
        public void Setup()
        {
            _masterDataBLL = Substitute.For<IMasterDataBLL>();
            _loger = Substitute.For<ILogger>();
            _httpContextBase = Substitute.For<HttpContextBase>();
            _controllerContext = Substitute.For<ControllerContext>();
        }

        [TestMethod]
        public void MasterProcess_ReturnCorrectView()
        {
            //Arrange
            var masterProcess = new MasterProcessController(_masterDataBLL, _loger);

            //Act
            var result = masterProcess.Index() as ViewResult;

            //Asert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);

        }
    }
}
