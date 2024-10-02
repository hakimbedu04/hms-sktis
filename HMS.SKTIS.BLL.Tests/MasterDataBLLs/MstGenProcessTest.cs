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
        public List<MstGenProcess> GetMstGenProcessesStub()
        {
            var mstGenProcesses = new List<MstGenProcess>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mstGenProcess = new MstGenProcess();
                mstGenProcesses.Add(mstGenProcess);
            }
            return mstGenProcesses;
        }

        [TestMethod]
        public void GetMasterProcesses_NotNull()
        {
            //arange
            var mstGenProcessesStub = GetMstGenProcessesStub();
            _mstGenProcessRepo.Get().ReturnsForAnyArgs(mstGenProcessesStub);
            var input = new GetMstGenProcessesInput();

            //act
            var mstGenProcesses = _bll.GetMasterProcesses(input);

            //assert
            Assert.IsNotNull(mstGenProcesses);
            Assert.AreEqual(RowCount, mstGenProcesses.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertMasterProcess_WhenKeyExist_ThrownException()
        {
            //arrange
            var mstGenProcessDTO = new MstGenProcessDTO() { ProcessGroup = "ROLLING" };
            _mstGenProcessRepo.GetByID(mstGenProcessDTO.ProcessGroup).ReturnsForAnyArgs(x => new MstGenProcess());

            try
            {
                //act
                _bll.InsertMasterProcess(mstGenProcessDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.ProcessGroupExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertMasterProcess_WhenProcessIdentifierExist_ThrownException()
        {
            //arrange
            var mstGenProcessDTO = new MstGenProcessDTO() { ProcessIdentifier = "1", ProcessGroup = "ROLLING" };
            _mstGenProcessRepo.GetByID(mstGenProcessDTO.ProcessGroup).ReturnsNullForAnyArgs();
            _mstGenProcessRepo.Get().ReturnsForAnyArgs(x => new List<MstGenProcess>() { new MstGenProcess() });

            try
            {
                //act
                _bll.InsertMasterProcess(mstGenProcessDTO);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.ProcessIdentifierExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertMasterProcess_WhenValid_Insert()
        {
            //arrange
            var mstGenProcessDTO = new MstGenProcessDTO { ProcessGroup = "ROLLING", Remark = "XXX" };

            //act
            _bll.InsertMasterProcess(mstGenProcessDTO);

            //assert
            _mstGenProcessRepo.Received()
                .Insert(Arg.Is<MstGenProcess>(m => m.ProcessGroup == "ROLLING" && m.Remark == "XXX"));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateMasterProcess_WhenNull_ThrowException()
        {
            //arrange
            _mstGenProcessRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateMasterProcess(new MstGenProcessDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateMasterProcess_WhenValid_Update()
        {
            //arrange
            var mstGenProcessDTO = new MstGenProcessDTO
            {
                ProcessGroup = "CUTTING",
                Remark = "NewRemark"
            };
            var dbMstGenProcess = new MstGenProcess
            {
                ProcessGroup = "CUTTING",
                Remark = "OldRemark"
            };

            _mstGenProcessRepo.GetByID(mstGenProcessDTO.ProcessGroup).ReturnsForAnyArgs(dbMstGenProcess);

            //act
            _bll.UpdateMasterProcess(mstGenProcessDTO);

            //assert
            Assert.AreEqual(mstGenProcessDTO.Remark, dbMstGenProcess.Remark);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
