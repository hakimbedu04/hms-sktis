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
using SKTISWebsite.Models.MasterBrand;

namespace SKTISWebsite.Tests
{
    //[TestClass]
    //public class MasterBrandControllerTest
    //{
    //    private IMasterDataBLL _masterDataBll;
    //    private MasterBrandController _controller;

    //    [TestInitialize]
    //    public void Setup()
    //    {
    //        _masterDataBll = Substitute.For<IMasterDataBLL>();
    //        _controller = new MasterBrandController(_masterDataBll);
    //        SKTISWebsiteMapper.Initialize();
    //    }

    //    [TestMethod]
    //    public void IndexIsCorrect()
    //    {
    //        var BrandGroupDto = new List<BrandGroupDTO>
    //        {
    //            new BrandGroupDTO
    //            {
    //                BrandGroupCode = "FA029194.12"
    //            }
    //        };

    //        _masterDataBll.GetBrandGroupsDistinct().Returns(BrandGroupDto);
    //        ViewResult viewResult = _controller.Index() as ViewResult;

    //        Assert.AreEqual("Index", viewResult.ViewName);
    //    }
    //}
}
