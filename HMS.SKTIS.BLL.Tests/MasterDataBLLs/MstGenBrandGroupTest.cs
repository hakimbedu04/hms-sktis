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
        public List<MstGenBrandGroup> GetMstGenBrandGroupsStub()
        {
            var mstGenBrandGroups = new List<MstGenBrandGroup>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstGenBrandGroup = new MstGenBrandGroup();
                mstGenBrandGroups.Add(mstGenBrandGroup);
            }
            return mstGenBrandGroups;
        }

        [TestMethod]
        public void GetBrandGroups_NotNull()
        {
            //arange
            var mstGenBrandGroupsStub = GetMstGenBrandGroupsStub();
            _mstGenBrandGroupRepo.Get().ReturnsForAnyArgs(mstGenBrandGroupsStub);
            var input = new GetMstGenBrandGroupInput();

            //act
            var mstGenBrandGroups = _bll.GetBrandGroups(input);

            //assert
            Assert.IsNotNull(mstGenBrandGroups);
            Assert.AreEqual(RowCount, mstGenBrandGroups.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertBrandGroup_WhenKeyExist_ThrownException()
        {
            //arrange
            var mstGenBrandGroupDTO = new MstGenBrandGroupDTO()
            {
                BrandGroupCode = "DSS12HR-20"
            };

            _mstGenBrandGroupRepo.GetByID(mstGenBrandGroupDTO.BrandGroupCode).ReturnsForAnyArgs(x => new MstGenBrandGroup());

            try
            {
                //act
                _bll.InsertBrandGroup(mstGenBrandGroupDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.KeyExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertBrandGroup_WhenValid_Insert()
        {
            //arrange
            var mstGenBrandGroupDTO = new MstGenBrandGroupDTO { BrandGroupCode = "DSS12HR-20", Remark = "XXX" };

            //act
            _bll.InsertBrandGroup(mstGenBrandGroupDTO);

            //assert
            _mstGenBrandGroupRepo.Received()
                .Insert(Arg.Is<MstGenBrandGroup>(m => m.BrandGroupCode == "DSS12HR-20" && m.Remark == "XXX"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateBrandGroup_WhenNull_ThrowException()
        {
            //arrange
            _mstGenBrandGroupRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateBrandGroup(new MstGenBrandGroupDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateBrandGroup_WhenValid_Update()
        {
            //arrange
            var mstGenBrandGroupDTO = new MstGenBrandGroupDTO
            {
                BrandGroupCode = "DSS12HR-20",
                Remark = "NewRemark"
            };
            var dbMstGenBrandGroup = new MstGenBrandGroup()
            {
                BrandGroupCode = "DSS12HR-20",
                Remark = "OldRemark"
            };

            _mstGenBrandGroupRepo.GetByID(mstGenBrandGroupDTO.BrandGroupCode).ReturnsForAnyArgs(dbMstGenBrandGroup);

            //act
            _bll.UpdateBrandGroup(mstGenBrandGroupDTO);

            //assert
            Assert.AreEqual(mstGenBrandGroupDTO.Remark, dbMstGenBrandGroup.Remark);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
