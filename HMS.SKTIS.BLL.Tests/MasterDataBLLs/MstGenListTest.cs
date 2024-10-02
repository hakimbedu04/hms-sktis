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
        public List<MstGenList> GetMstGeneralListsStub()
        {
            var mstGeneralLists = new List<MstGenList>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstGeneralList = new MstGenList();
                mstGeneralLists.Add(mstGeneralList);
            }
            return mstGeneralLists;
        }

        [TestMethod]
        public void GetMstGeneralLists_NotNull()
        {
            //arange
            var mstGeneralListsStub = GetMstGeneralListsStub();
            _mstGenListRepo.Get().ReturnsForAnyArgs(mstGeneralListsStub);
            var input = new GetMstGenListsInput();

            //act
            var mstGeneralLists = _bll.GetMstGeneralLists(input);

            //assert
            Assert.IsNotNull(mstGeneralLists);
            Assert.AreEqual(RowCount, mstGeneralLists.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertMstGeneralList_WhenKeyExist_ThrownException()
        {
            //arrange
            _mstGenListRepo.GetByID().ReturnsForAnyArgs(x => new MstGenList());

            try
            {
                //act
                _bll.InsertMstGeneralList(new MstGeneralListDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.KeyExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertMstGeneralList_WhenValid_Insert()
        {
            //arrange
            var mstGeneralListDTO = new MstGeneralListDTO { ListGroup = "Brand Family", ListDetail = "XXX" };

            //act
            _bll.InsertMstGeneralList(mstGeneralListDTO);

            //assert
            _mstGenListRepo.Received()
                .Insert(Arg.Is<MstGenList>(m => m.ListGroup == "Brand Family" && m.ListDetail == "XXX"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateMstGeneralList_WhenNull_ThrowException()
        {
            //arrange
            _mstGenListRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateMstGeneralList(new MstGeneralListDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateMstGeneralList_WhenValid_Update()
        {
            //arrange
            var mstGeneralListDTO = new MstGeneralListDTO
            {
                ListGroup = "Brand Family",
                ListDetail = "XXX",
                Remark = "NewRemark"
            };
            var dbMstGeneralList = new MstGenList()
            {
                ListGroup = "Brand Family",
                ListDetail = "XXX",
                Remark = "OldRemark"
            };

            _mstGenListRepo.GetByID().ReturnsForAnyArgs(dbMstGeneralList);

            //act
            _bll.UpdateMstGeneralList(mstGeneralListDTO);

            //assert
            Assert.AreEqual(mstGeneralListDTO.Remark, dbMstGeneralList.Remark);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
