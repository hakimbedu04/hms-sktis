using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SKTISWebsite.Controllers;

namespace SKTISWebsite.Tests
{
    [TestClass]
    public class MasterGenListControllerTest
    {
        private IMasterDataBLL _bll;
        private MasterGenListController _controller;

        [TestInitialize]
        public void TestSetup()
        {
            _bll = Substitute.For<IMasterDataBLL>();
            _controller = new MasterGenListController(_bll);
        }

        [TestMethod]
        public void GetListGroupSelectList_IsCorrect()
        {
            //arrange
            var listGroups = new List<KeyValuePair<string,string>>();
            listGroups.Add(new KeyValuePair<string,string>("Brand Family","BrandFamily"));
            listGroups.Add(new KeyValuePair<string, string>("Maintenance Item Type", "MtncItemType"));
            _bll.GetListGroupsEnum().ReturnsForAnyArgs(listGroups);

            //act
            JsonResult actual = _controller.GetListGroupSelectList();

            //assert
            List<KeyValuePair<string,string>> result = actual.Data as List<KeyValuePair<string,string>>;
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(new KeyValuePair<string, string>("Brand Family", "BrandFamily"), result[0]);
            Assert.AreEqual(new KeyValuePair<string, string>("Maintenance Item Type", "MtncItemType"), result[1]);
        }

        //[TestMethod]
        //public void GenerateExcel_IsCorrect()
        //{
        //    //arrange
        //    _bll.GetMstGeneralLists(null).ReturnsForAnyArgs(new List<MstGeneralListCompositeDTO>());

        //    //act
        //    var result = _controller.GenerateExcel(null);

        //    //assert
        //    Assert.IsInstanceOfType(result, typeof (FileStreamResult));

        //    var streamResult = (FileStreamResult)result;
        //    //var zipInputStream = new ZipInputStream(streamResult.FileStream);

        //    //Assert.IsNotNull(zipInputStream);

        //    //var zipEntry = zipInputStream.GetNextEntry();
        //   // Assert.AreEqual("The Simpsons", zipEntry.Name);
        //}
    }
}
