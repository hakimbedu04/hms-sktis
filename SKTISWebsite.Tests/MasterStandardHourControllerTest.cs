using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SKTISWebsite.Controllers;
using SKTISWebsite.Models.Factories.Contract;
//using SKTISWebsite.Models.MasterStandardHour;

namespace SKTISWebsite.Tests
{
    //[TestClass]
    //public class MasterStandardHourControllerTest
    //{
    //    private AbstractSelectList _abstractSelectList;
    //    private IMasterDataBLL _masterDataBll;
    //   // private MasterStandardHourController _controller;

    //    [TestInitialize]
    //    public void Setup()
    //    {
    //        _abstractSelectList = Substitute.For<AbstractSelectList>();
    //        _masterDataBll = Substitute.For<IMasterDataBLL>();
    //        //_controller = new MasterStandardHourController(_abstractSelectList, _masterDataBll);
    //        SKTISWebsiteMapper.Initialize();
    //    }

    //    [TestMethod]
    //    public void IndexReturnsCorrectView()
    //    {
    //        var dummy = new List<MstGeneralListCompositeDTO>
    //        {
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Day Type",
    //                ListDetail = "Holiday",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            },
    //            new MstGeneralListCompositeDTO
    //            {
    //                ListGroup = "Day Type",
    //                ListDetail = "Non Holiday",
    //                StatusActive = true,
    //                Remark = null,
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
    //                UpdatedBy = "Admin"
    //            }
    //        };

    //        var dummyListStandardHoursDto = new List<StandardHourDTO>
    //        {
    //            new StandardHourDTO
    //            {
    //                DayType = "Holiday",
    //                DayName = "Sunday",
    //                JknHour = 7,
    //                Jl1Hour = 1,
    //                Jl2Hour = 2,
    //                Jl3Hour = 3,
    //                Jl4Hour = 4,
    //                UpdatedBy = "Admin",
    //                UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000")
    //            }
    //        };

    //        var dummyCreateDayType = new SelectList(dummy, Enums.DropdownOption.ListGroup.ToString(),
    //            Enums.DropdownOption.ListDetail.ToString());

    //        _masterDataBll.GetGeneralLists(Arg.Any<string[]>()).Returns(dummy);
    //      //  _abstractSelectList.CreateDayType().Returns(dummyCreateDayType);
    //       // _masterDataBll.GetStandardHours(Arg.Any<BaseInput>()).Returns(dummyListStandardHoursDto);

    //        //ViewResult viewResult = _controller.Index() as ViewResult;
    //        //Assert.IsNotNull(viewResult);
    //        //Assert.AreEqual(viewResult.ViewName, "Index");
    //        //Assert.AreEqual(2, (viewResult.Model as IndexMasterStandardHourViewModel).DayType.Count());
            
    //    }

//    }


}
