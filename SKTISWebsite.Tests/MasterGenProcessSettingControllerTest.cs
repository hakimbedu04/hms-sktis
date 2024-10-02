using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SKTISWebsite.Code;
using SKTISWebsite.Controllers;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenProccessSetting;

namespace SKTISWebsite.Tests
{
    [TestClass]
    public partial class MasterGenProcessSettingControllerTest
    {

        private IMasterDataBLL _bll;
        private IApplicationService _svc;
        private MasterGenProcessSettingController _controller;

        [TestInitialize]
        public void TestSetup()
        {
            _bll = Substitute.For<IMasterDataBLL>();
            _svc = Substitute.For<IApplicationService>();

            _controller = new MasterGenProcessSettingController(_svc, _bll);
        }

        [TestMethod]
        public void IndexReturningCorrectView()
        {
            // Arrange
            var listLocationCodes = new List<MstGenLocationDTO>();
            var listBrandGroups = new List<MstGenBrandGroupDTO>();
            var listProcessGroup = new List<MstGenProcessDTO>();

            _bll.GetAllLocations(null).ReturnsForAnyArgs(listLocationCodes);
            _bll.GetAllBrandGroups().ReturnsForAnyArgs(listBrandGroups);
            _bll.GetAllMasterProcess().ReturnsForAnyArgs(listProcessGroup);
            _bll.GetMasterProcesses(null).ReturnsForAnyArgs(listProcessGroup);

            // Act
            ViewResult viewResult = _controller.Index() as ViewResult;

            // Assert
            Assert.AreEqual(viewResult.ViewName, "index");
        }

        [TestMethod]
        public void GetProcessSettingsIsCorrect()
        {
            // Arrange
            var param = new GetMasterProcessSettingsInput
            {
                BrandCode = "SAH1HR-1",
                Process = "Rolling",
                PageSize = 10,
            };

            var listProcessSettings = new List<MstGenProcessSettingDTO>
            {
                new MstGenProcessSettingDTO
                {
                    IDProcess = 11,
                    BrandGroupCode = "SAH1HR-1",
                    ProcessGroup = "Rolling",
                    //ProcessIdentifier = "1",
                    StdStickPerHour = 1,
                    MinStickPerHour = 2,
                    UOMEblek = null,
                    Remark = "remark",
                    CreatedDate = Convert.ToDateTime("2015-08-21 10:48:00.967"),
                    CreatedBy = "PMI/User",
                    UpdatedDate = Convert.ToDateTime("2015-08-21 10:48:00.967"),
                    UpdatedBy = "PMI/User"
                }
            };
            _bll.GetMasterProcessSettings(param).Returns(listProcessSettings);

            // Act
            JsonResult actual = _controller.GetProcessSettings(param);

            // Assert
            PageResult<MstGenProccessSettingViewModel> result = actual.Data as PageResult<MstGenProccessSettingViewModel>;
            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual("SAH1HR-1", result.Results[0].BrandGroupCode);
            Assert.AreEqual("Rolling", result.Results[0].ProcessGroup);

        }
    }
}
