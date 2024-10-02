using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SKTISWebsite.Controllers;
using SKTISWebsite.Models.Factories.Contract;
using EnumHelper = HMS.SKTIS.Utils.EnumHelper;

namespace SKTISWebsite.Tests
{
    //[TestClass]
    //public class MasterBrandGroupControllerTest
    //{
    //    private IMasterDataBLL _masterDataBll;
    //    private AbstractSelectList _abstractSelectList;
    //    private MasterBrandGroupController _controller;

    //    [TestInitialize]
    //    public void Setup()
    //    {
    //        _masterDataBll = Substitute.For<IMasterDataBLL>();
    //        _abstractSelectList = Substitute.For<AbstractSelectList>();
    //        _controller = new MasterBrandGroupController(_masterDataBll, _abstractSelectList);
    //        SKTISWebsiteMapper.Initialize();
    //    }

    //    [TestMethod]
    //    public void IndexReturnCorrectView()
    //    {
    //        var dummy = new List<MstGeneralListCompositeDTO>
    //        {
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Brand Family",
    //                ListDetail = "PAS",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Brand Family",
    //                ListDetail = "DSS",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Brand Family",
    //                ListDetail = "SAH",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Brand Code",
    //                ListDetail = "PAS12",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Brand Code",
    //                ListDetail = "DSS12",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Brand Code",
    //                ListDetail = "SAH12",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Pack",
    //                ListDetail = "SoftPack",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Pack",
    //                ListDetail = "HardPack",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Class",
    //                ListDetail = "Regular",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Class",
    //                ListDetail = "Non Regular",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            }
    //        };

    //        var dummyCreateBrandFamily = new SelectList
    //            (dummy.Where(p => p.ListGroup == EnumHelper.GetDescription(Enums.MasterGeneralList.BrandFamily)), Enums.DropdownOption.ListGroup.ToString(), Enums.DropdownOption.ListDetail.ToString());
    //        //var dummyCreateBrandCode = new SelectList
    //        //    (dummy.Where(p => p.ListGroup == EnumHelper.GetDescription(Enums.MasterGeneralList.BrandCode)),
    //        //        Enums.DropdownOption.ListGroup.ToString(), Enums.DropdownOption.ListDetail.ToString());
    //        var dummyCreatePack = new SelectList
    //            (dummy.Where(p => p.ListGroup == EnumHelper.GetDescription(Enums.MasterGeneralList.Pack)),
    //                Enums.DropdownOption.ListGroup, Enums.DropdownOption.ListDetail.ToString());
    //        var dummyCreateClass = new SelectList
    //            (dummy.Where(p => p.ListGroup == EnumHelper.GetDescription(Enums.MasterGeneralList.Class)),
    //                Enums.DropdownOption.ListGroup, Enums.DropdownOption.ListDetail.ToString());
    //        var dummyListBrandGroupDTO = new List<BrandGroupDTO>
    //        {
    //            new BrandGroupDTO
    //            {
    //                BrandGroupCode = "PAS12SR-20",
    //                BrandFamily = "DSS",
    //                Pack = "HardPack",
    //                Class = "Regular",
    //                StickPerPack = 12,
    //                PackPerSlof = 20,
    //                SlofPerBal = 10,
    //                BalPerBox = 5,
    //                SktBrandCode = "PAS12",
    //                Description = "PENAMAS SOFTPACK REG 12",
    //                CigarretteWeight = 1.24,
    //                StickPerBox = 2400,
    //                StickPerSlot = 240,
    //                StickPerBal = 2400,
    //                StatusActive = true,
    //                Remark = "Remark",
    //                UpdatedDate = Convert.ToDateTime("07/07/2015"),
    //                UpdatedBy = "Oka"
    //            }
    //        };

    //        _masterDataBll.GetGeneralLists(Arg.Any<string[]>()).Returns(dummy);
    //        _abstractSelectList.CreateBrandFamily().Returns(dummyCreateBrandFamily);
    //        //_abstractSelectList.CreateBrandCode().Returns(dummyCreateBrandCode);
    //        _abstractSelectList.CreatePack().Returns(dummyCreatePack);
    //        _abstractSelectList.CreateClass().Returns(dummyCreateClass);
    //        //_masterDataBll.GetBrandGroups().Returns(dummyListBrandGroupDTO);

    //        ViewResult viewResult = _controller.Index() as ViewResult;
    //        Assert.IsNotNull(viewResult);
    //        Assert.AreEqual(viewResult.ViewName, "Index");
    //        //Assert.AreEqual(1, (viewResult.Model as IndexMasterBrandGroupViewModel).Details.Count);
    //        Assert.AreEqual(3, (viewResult.Model as IndexMasterBrandGroupViewModel).BrandFamily.Count());
    //        //Assert.AreEqual(3, (viewResult.Model as IndexMasterBrandGroupViewModel).BrandCode.Count());
    //        Assert.AreEqual(2, (viewResult.Model as IndexMasterBrandGroupViewModel).Pack.Count());
    //        Assert.AreEqual(2, (viewResult.Model as IndexMasterBrandGroupViewModel).Class.Count());

    //    }
    //}
}
