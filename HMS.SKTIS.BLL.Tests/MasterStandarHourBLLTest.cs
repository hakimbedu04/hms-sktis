using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Voxteneo.WebComponents.Logger;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public class MasterStandarHourBLLTest
    {
        private IMasterDataBLL _masterDataBll;

        [TestInitialize]
        public void Setup()
        {
            _masterDataBll = Substitute.For<IMasterDataBLL>();
            BLLMapper.Initialize();
        }

        [TestMethod]
        public void GetStandardHoursIsCorrect()
        {
            var dummy = new List<StandardHourDTO>
            {
                new StandardHourDTO
                {
                    DayType = "Holiday",
                    DayName = "Sunday",
                    JknHour = 7,
                    Jl1Hour = 1,
                    Jl2Hour = 2,
                    Jl3Hour = 3,
                    Jl4Hour = 4,
                    UpdatedBy = "Admin",
                    UpdatedDate = Convert.ToDateTime("2015-07-03 00:00:00.000")
                }
            };

            _masterDataBll.GetStandardHours(Arg.Any<BaseInput>()).Returns(dummy);
            BaseInput baseInput = new BaseInput();
            var result = _masterDataBll.GetStandardHours(baseInput);

            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void AutoMapperIsCorrect()
        {
            var dummy = new List<MstStandardHour>
            {
                new MstStandardHour
                {
                    CreatedBy = "Oka",
                    CreatedDate = Convert.ToDateTime("07/07/2015"),
                    Day = 1,
                    DayName = "Sunday",
                    DayType = "Holiday",
                    JknHour = 7,
                    Jl1Hour = 1,
                    Jl2Hour = 2,
                    Jl3Hour = 3,
                    Jl4Hour = 4,
                    UpdatedBy = "Admin",
                    UpdatedDate = Convert.ToDateTime("07/08/2015")
                }
            };

            var result = Mapper.Map<List<StandardHourDTO>>(dummy);

            Assert.IsNotNull(result);
        }
    }
}
