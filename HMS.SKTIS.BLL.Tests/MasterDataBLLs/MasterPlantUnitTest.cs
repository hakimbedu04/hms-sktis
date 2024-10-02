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
        public List<MstPlantUnitView> GetMstPlantUnitViewsStub()
        {
            var mstPlantUnitViews = new List<MstPlantUnitView>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstPlantUnitView = new MstPlantUnitView();
                mstPlantUnitViews.Add(mstPlantUnitView);
            }
            return mstPlantUnitViews;
        }

        [TestMethod]
        public void GetMstPlantUnits_NotNull()
        {
            //arange
            var mstPlantUnitsStub = GetMstPlantUnitViewsStub();
            _mstPlantUnitViewRepo.Get().ReturnsForAnyArgs(mstPlantUnitsStub);
            var input = new GetMstPlantUnitsInput();

            //act
            var mstPlantUnits = _bll.GetMstPlantUnits(input);

            //assert
            Assert.IsNotNull(mstPlantUnits);
            Assert.AreEqual(RowCount, mstPlantUnits.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertMstPlantUnit_WhenKeyExist_ThrownException()
        {
            //arrange
            var mstPlantUnitDTO = new MstPlantUnitDTO() { UnitCode = "2021", LocationCode = "SKT"};
            _mstPlantUnitRepo.GetByID(mstPlantUnitDTO.UnitCode, mstPlantUnitDTO.LocationCode).ReturnsForAnyArgs(x => new MstPlantUnit());

            try
            {
                //act
                _bll.InsertMstPlantUnit(mstPlantUnitDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.KeyExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertMstPlantUnit_WhenValid_Insert()
        {
            //arrange
            var mstPlantUnitDTO = new MstPlantUnitDTO() { LocationCode = "SKT", UnitCode = "2021", UnitName = "UNIT 1" };

            //act
            _bll.InsertMstPlantUnit(mstPlantUnitDTO);

            //assert
            _mstPlantUnitRepo.Received()
                .Insert(Arg.Is<MstPlantUnit>(m => m.LocationCode == "SKT" && m.UnitCode == "2021" && m.UnitName == "UNIT 1"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateMstPlantUnit_WhenNull_ThrowException()
        {
            //arrange
            _mstPlantUnitRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateMstPlantUnit(new MstPlantUnitDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateMstPlantUnit_WhenValid_Update()
        {
            //arrange
            var mstPlantUnitDTO = new MstPlantUnitDTO()
            {
                UnitCode = "2021",
                LocationCode = "SKT",
                Remark = "NewRemark"
            };
            var dbMstPlantUnit = new MstPlantUnit()
            {
                UnitCode = "2021",
                LocationCode = "SKT",
                Remark = "OldRemark"
            };

            _mstPlantUnitRepo.GetByID(mstPlantUnitDTO.UnitCode, mstPlantUnitDTO.LocationCode).ReturnsForAnyArgs(dbMstPlantUnit);

            //act
            _bll.UpdateMstPlantUnit(mstPlantUnitDTO);

            //assert
            Assert.AreEqual(mstPlantUnitDTO.Remark, dbMstPlantUnit.Remark);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
