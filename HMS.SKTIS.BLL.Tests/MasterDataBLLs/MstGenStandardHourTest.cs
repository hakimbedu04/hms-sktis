using System;
using System.Collections.Generic;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace HMS.SKTIS.BLL.Tests
{
    public partial class MasterDataBLLTest
    {
        public List<MstGenStandardHour> GetMstGenStandardHoursStub()
        {
            var mstGenStandardHours = new List<MstGenStandardHour>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstGenStandardHour = new MstGenStandardHour();
                mstGenStandardHours.Add(mstGenStandardHour);
            }
            return mstGenStandardHours;
        }

        [TestMethod]
        public void GetStandardHours_NotNull()
        {
            //arange
            var mstGenStandardHourStub = GetMstGenStandardHoursStub();
            _mstGenStandardHourRepo.Get().ReturnsForAnyArgs(mstGenStandardHourStub);
            var input = new BaseInput();

            //act
            var mstGenStandardHours = _bll.GetStandardHours(input);

            //assert
            Assert.IsNotNull(mstGenStandardHours);
            Assert.AreEqual(RowCount, mstGenStandardHours.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertStandardHour_WhenKeyExist_ThrownException()
        {
            //arrange
            _mstGenStandardHourRepo.GetByID().ReturnsForAnyArgs(x => new MstGenStandardHour());
            var standardHoursDTO = new MstGenStandardHourDTO() {DayName = "MONDAY"};
            try
            {
                //act
                _bll.InsertStandardHour(standardHoursDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.KeyExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertStandardHour_WhenDayEmpty_ThrownException()
        {
            //arrange
            _mstGenStandardHourRepo.GetByID().ReturnsForAnyArgs(x => new MstGenStandardHour());
            var standardHoursDTO = new MstGenStandardHourDTO();
            try
            {
                //act
                _bll.InsertStandardHour(standardHoursDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.EmptyDay.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertStandardHour_WhenValid_Insert()
        {
            //arrange
            var mstGenStandardHourDTO = new MstGenStandardHourDTO { DayType = "Holiday", DayName = "Monday" };

            //act
            _bll.InsertStandardHour(mstGenStandardHourDTO);

            //assert
            _mstGenStandardHourRepo.Received()
                .Insert(Arg.Is<MstGenStandardHour>(m => m.DayType == "Holiday" && m.Day == 1 && m.DayName == "Monday"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateStandardHour_WhenNull_ThrowException()
        {
            //arrange
            _mstGenStandardHourRepo.GetByID().ReturnsNullForAnyArgs();
            var standardHoursDTO = new MstGenStandardHourDTO() {DayName = "MONDAY"};
            try
            {
                //act
                _bll.UpdateStandardHour(standardHoursDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateStandardHour_WhenDayEmpty_ThrownException()
        {
            //arrange
            _mstGenStandardHourRepo.GetByID().ReturnsForAnyArgs(x => new MstGenStandardHour());
            var standardHoursDTO = new MstGenStandardHourDTO();
            try
            {
                //act
                _bll.InsertStandardHour(standardHoursDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.EmptyDay.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateStandardHour_WhenValid_Update()
        {
            //arrange
            var mstGenStandardHourDTO = new MstGenStandardHourDTO
            {
                DayType = "Holiday",
                DayName = "Monday",
                JknHour = 7
            };
            var dbMstGenStandardHour = new MstGenStandardHour()
            {
                DayType = "Holiday",
                Day = 1,
                JknHour = 5
            };

            _mstGenStandardHourRepo.GetByID().ReturnsForAnyArgs(dbMstGenStandardHour);

            //act
            _bll.UpdateStandardHour(mstGenStandardHourDTO);

            //assert
            Assert.AreEqual(mstGenStandardHourDTO.JknHour, dbMstGenStandardHour.JknHour);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
