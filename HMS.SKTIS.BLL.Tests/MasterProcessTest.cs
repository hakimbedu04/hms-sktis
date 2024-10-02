using System;
using HMS.SKTIS.BusinessObjects;
using System.Collections.Generic;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public class MasterProcessTest : MasterDataBLLTests
    {
        [TestMethod]
        public void MasterProcess_ContainData()
        {
            var mstProsesInput = new MstProcessInput()
                                 {
                                     ProcessName = "Giling"
                                 };

            var FakeMstProcessDTOs = ListMstProcessDTOs();
            _masterDataBLL.GetMstProcessLists(mstProsesInput).Returns(FakeMstProcessDTOs);
            var result = _masterDataBLL.GetMstProcessLists(mstProsesInput);
            Assert.AreEqual(1, result.Count);
        }


        public List<MstProcessDTO> ListMstProcessDTOs()
        {
            return new List<MstProcessDTO>
            {
                new MstProcessDTO
                {
                    ProcessName = "Giling",
                    ProcessOrder = 1,
                    StatusActive = true,
                    Remark = "ramaks",
                    CreatedBy = "admin",
                    CreatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000"),
                    UpdatedBy = "admin",
                    UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000")
                }
            };
        }
    }
}
