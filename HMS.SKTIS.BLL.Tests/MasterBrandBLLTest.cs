using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public class MasterBrandBLLTest
    {
        private IMasterDataBLL _masterDataBll;

        [TestInitialize]
        public void Setup()
        {
            _masterDataBll = Substitute.For<IMasterDataBLL>();
            BLLMapper.Initialize();
        }

        [TestMethod]
        public void GetBrandIsCorrect()
        {
            var dummy = new List<BrandDTO>
            {
                new BrandDTO
                {
                    BrandCode = "FA029194.12",
                    BrandGroupCode = "DSS12SR-20",
                    Description = "DJI SAM SOE KRETEK 11",
                    EffectiveDate = Convert.ToDateTime("1/1/2015"),
                    ExpiredDate = Convert.ToDateTime("2/11/2015"),
                    Remark = "Remark",
                    UpdatedBy = "PMI/mnatalia",
                    UpdatedDate = Convert.ToDateTime("11/02/2015 14:38:21")
                }
            };

            GetBrandInput input = new GetBrandInput();

            _masterDataBll.GetBrand(Arg.Any<GetBrandInput>()).Returns(dummy);

            var result = _masterDataBll.GetBrand(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void AutoMapperIsCorrect()
        {
            var dummy = new List<MstBrand>
            {
                new MstBrand
                {
                    BrandCode = "FA029194.12",
                    BrandGroupCode = "DSS12SR-20",
                    Description = "DJI SAM SOE KRETEK 11",
                    EffectiveDate = Convert.ToDateTime("1/1/2015"),
                    ExpiredDate = Convert.ToDateTime("2/11/2015"),
                    Remark = "Remark",
                    UpdatedBy = "PMI/mnatalia",
                    UpdatedDate = Convert.ToDateTime("11/02/2015 14:38:21")
                }
            };

            var result = Mapper.Map<List<BrandDTO>>(dummy);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
    }
}
