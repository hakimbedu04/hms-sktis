using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SKTISWebsite.Controllers;

namespace SKTISWebsite.Tests
{
    //[TestClass]
    //public class MasterBrandGroupMaterialControllerTest
    //{
    //    private IMasterDataBLL _masterDataBll;
    //    private MasterBrandGroupMaterialController _controller;

    //    [TestInitialize]
    //    public void Setup()
    //    {
    //        _masterDataBll = Substitute.For<IMasterDataBLL>();
    //        _controller = new MasterBrandGroupMaterialController(_masterDataBll);
    //        SKTISWebsiteMapper.Initialize();
    //    }

    //    [TestMethod]
    //    public void IsIndexCorrect()
    //    {
    //        var dummy = new List<BrandGroupMaterialDTO>
    //        {
    //            new BrandGroupMaterialDTO
    //            {
    //                BrandGroupCode = "FA029194.12",
    //                MaterialName = "Tembakau",
    //                Description = "Description",
    //                Uom = "EA",
    //                Remark = "Remark",
    //                UpdatedBy = "Oka",
    //                UpdatedDate = Convert.ToDateTime("7/7/2015")
    //            }
    //        };

    //        _masterDataBll.GetBrandGroupMaterial().Returns(dummy);

    //        var result = _controller.Index() as ViewResult;

    //        Assert.IsNotNull(result);
    //        Assert.AreEqual("Index", result.ViewName);
    //    }
    //}
}
