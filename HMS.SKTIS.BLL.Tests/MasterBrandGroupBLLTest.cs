using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public class MasterBrandGroupBLLTest
    {
        private IMasterDataBLL _masterDataBll;
        
        [TestInitialize]
        public void Setup()
        {
            _masterDataBll = Substitute.For<IMasterDataBLL>();
            BLLMapper.Initialize();
        }

        [TestMethod]
        public void GetBrandGroupsIsCorrect()
        {
            var dummy = new List<BrandGroupDTO>
            {
                new BrandGroupDTO
                {
                    BrandGroupCode = "PAS12SR-20",
                    BrandFamily = "DSS",
                    Pack = "HardPack",
                    Class = "Regular",
                    StickPerPack = 12,
                    PackPerSlof = 20,
                    SlofPerBal = 10,
                    BalPerBox = 5,
                    SktBrandCode = "PAS12",
                    Description = "PENAMAS SOFTPACK REG 12",
                    CigarretteWeight = 1.24,
                    StickPerBox = 2400,
                    StickPerSlot = 240,
                    StickPerBal = 2400,
                    StatusActive = true,
                    Remark = "Remark",
                    UpdatedDate = Convert.ToDateTime("07/07/2015"),
                    UpdatedBy = "Oka"
                }
            };

            //_masterDataBll.GetBrandGroups().Returns(dummy);
            //var result = _masterDataBll.GetBrandGroups();

            //Assert.AreEqual(result.Count, 1);

        }

        [TestMethod]
        public void AutoMapperIsCorrect()
        {
            var dummy = new List<MstBrandGroup>
            {
                new MstBrandGroup
                {
                    BalPerBox = 5,
                    BrandFamily = "DSS",
                    BrandGroupCode = "PAS12SR-20",
                    CigarretteWeight = 1.24,
                    Class = "Regular",
                    CreatedBy = "Oka",
                    CreatedDate = Convert.ToDateTime("07/07/2015"),
                    Description = "PANAMAS SOFTPACK REG 12",
                    Pack = "HardPack",
                    PackPerSlof = 20,
                    Remark = "Remark",
                    SKTBrandCode = "PAS12",
                    SlofPerBal = 10,
                    StatusActive = true,
                    StickPerBal = 2400,
                    StickPerBox = 2400,
                    StickPerPack = 12,
                    StickPerSlot = 240,
                    UpdatedBy = "Admin",
                    UpdatedDate = Convert.ToDateTime("07/07/2015")
                }
            };



            var result = Mapper.Map<List<BrandGroupDTO>>(dummy);

            Assert.IsNotNull(result);
        }
    }
}
