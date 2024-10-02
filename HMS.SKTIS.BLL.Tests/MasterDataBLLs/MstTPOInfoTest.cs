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
        public List<MstTPOInfo> GetMstTPOInfosStub()
        {
            var mstTPOInfos = new List<MstTPOInfo>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstTPOInfo = new MstTPOInfo();
                mstTPOInfos.Add(mstTPOInfo);
            }
            return mstTPOInfos;
        }

        [TestMethod]
        public void GetMasterTPOInfos_NotNull()
        {
            //arange
            var mstTPOInfosStub = GetMstTPOInfosStub();
            _mstTPOInfoRepo.Get().ReturnsForAnyArgs(mstTPOInfosStub);
            var input = new GetMasterTPOInfoInput();

            //act
            var mstTPOInfos = _bll.GetMasterTPOInfos(input);

            //assert
            Assert.IsNotNull(mstTPOInfos);
            Assert.AreEqual(RowCount, mstTPOInfos.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertMasterTPOInfo_WhenKeyExist_ThrownException()
        {
            //arrange
            var mstTPOInfoDTO = new MstTPOInfoDTO() { LocationCode = "IDAM" };
            _mstTPOInfoRepo.GetByID(mstTPOInfoDTO.LocationCode).ReturnsForAnyArgs(x => new MstTPOInfo());

            try
            {
                //act
                _bll.InsertMasterTPOInfo(mstTPOInfoDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.KeyExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertMasterTPOInfo_WhenValid_Insert()
        {
            //arrange
            var mstTPOInfoDTO = new MstTPOInfoDTO { LocationCode = "IDAM", VendorName = "XXX" };

            //act
            _bll.InsertMasterTPOInfo(mstTPOInfoDTO);

            //assert
            _mstTPOInfoRepo.Received()
                .Insert(Arg.Is<MstTPOInfo>(m => m.LocationCode == "IDAM" && m.VendorName == "XXX"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateMasterTPOInfo_WhenNull_ThrowException()
        {
            //arrange
            _mstTPOInfoRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateMasterTPOInfo(new MstTPOInfoDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateMasterTPOInfo_WhenValid_Update()
        {
            //arrange
            var mstTPOInfoDTO = new MstTPOInfoDTO
            {
                LocationCode = "IDAM",
                VendorName = "NewVendor"
            };
            var dbMstTPOInfo = new MstTPOInfo()
            {
                LocationCode = "IDAM",
                VendorName = "OldVendor"
            };

            _mstTPOInfoRepo.GetByID(mstTPOInfoDTO.LocationCode).ReturnsForAnyArgs(dbMstTPOInfo);

            //act
            _bll.UpdateMasterTPOInfo(mstTPOInfoDTO);

            //assert
            Assert.AreEqual(mstTPOInfoDTO.VendorName, dbMstTPOInfo.VendorName);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
