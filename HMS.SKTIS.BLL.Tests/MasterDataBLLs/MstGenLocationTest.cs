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
        public List<MstGenLocation> GetMstGenLocationsStub()
        {
            var mstGenLocations = new List<MstGenLocation>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstGenLocation = new MstGenLocation();
                mstGenLocations.Add(mstGenLocation);
            }
            return mstGenLocations;
        }

        [TestMethod]
        public void GetMstLocationLists_NotNull()
        {
            //arange
            var mstGenLocationsStub = GetMstGenLocationsStub();
            _mstGenLocationRepo.Get().ReturnsForAnyArgs(mstGenLocationsStub);
            var input = new GetMstGenLocationInput();

            //act
            var mstLocationLists = _bll.GetMstLocationLists(input);

            //assert
            Assert.IsNotNull(mstLocationLists);
            Assert.AreEqual(RowCount, mstLocationLists.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertLocation_WhenKeyExist_ThrownException()
        {
            //arrange
            var mstGenLocationDTO = new MstGenLocationDTO
            {
                LocationCode = "SKT"
            };
            _mstGenLocationRepo.GetByID(mstGenLocationDTO.LocationCode).ReturnsForAnyArgs(x => new MstGenLocation());

            try
            {
                //act
                _bll.InsertLocation(mstGenLocationDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.KeyExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertLocation_WhenValid_Insert()
        {
            //arrange
            var mstGenLocationDTO = new MstGenLocationDTO() { LocationCode = "SKT", LocationName = "XXX" };

            //act
            _bll.InsertLocation(mstGenLocationDTO);

            //assert
            _mstGenLocationRepo.Received()
                .Insert(Arg.Is<MstGenLocation>(m => m.LocationCode == "SKT" && m.LocationName == "XXX"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateLocation_WhenNull_ThrowException()
        {
            //arrange
            _mstGenLocationRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateLocation(new MstGenLocationDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateLocation_WhenValid_Update()
        {
            //arrange
            var mstGenLocationDTO = new MstGenLocationDTO
            {
                LocationCode = "SKT",
                Remark = "NewRemark"
            };
            var dbMstGenLocation = new MstGenLocation()
            {
                LocationCode = "SKT",
                Remark = "OldRemark"
            };

            _mstGenLocationRepo.GetByID(mstGenLocationDTO.LocationCode).ReturnsForAnyArgs(dbMstGenLocation);

            //act
            _bll.UpdateLocation(mstGenLocationDTO);

            //assert
            Assert.AreEqual(mstGenLocationDTO.Remark, dbMstGenLocation.Remark);
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateLocation_WhenLocationCodeAndParentSame_ThrowException()
        {
            var locationDTO = new MstGenLocationDTO()
            {
                LocationCode = "SKT",
                ParentLocationCode = "SKT"
            };

            var dbMstGenLocation = new MstGenLocation()
            {
                LocationCode = "SKT"
            };

            _mstGenLocationRepo.GetByID(locationDTO.LocationCode).ReturnsForAnyArgs(dbMstGenLocation);

            try
            {
                //act
                _bll.UpdateLocation(locationDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.LocationCodeAndParentSame.ToString(), ex.Code);
                throw;
            }
        }

         [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertLocation_WhenLocationCodeAndParentSame_ThrowException()
        {
            var locationDTO = new MstGenLocationDTO()
            {
                LocationCode = "ABCD",
                ParentLocationCode = "ABCD"
            };

            _mstGenLocationRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.InsertLocation(locationDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.LocationCodeAndParentSame.ToString(), ex.Code);
                throw;
            }
        }
    }
}
