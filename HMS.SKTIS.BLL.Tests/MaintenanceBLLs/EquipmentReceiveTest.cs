using System;
using System.Collections.Generic;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace HMS.SKTIS.BLL.Tests
{
    public partial class MaintenanceBLLTests
    {
        public List<MntcEquipmentMovement> GetMntcEquipmentMovementsStub()
        {
            var mntcEquipmentMovements = new List<MntcEquipmentMovement>();
            for (int i = 1; i <= RowCount; i++)
            {
                var mntcEquipmentMovement = new MntcEquipmentMovement();
                mntcEquipmentMovements.Add(mntcEquipmentMovement);
            }
            return mntcEquipmentMovements;
        }

        [TestMethod]
        public void GetEquipmentReceives_NotNull()
        {
            //arange
            var mntcEquipmentMovementsStub = GetMntcEquipmentMovementsStub();
            _equipmentMovementRepo.Get().ReturnsForAnyArgs(mntcEquipmentMovementsStub);
            var input = new GetEquipmentReceivesInput();

            //act
            var equipmentReceives = _bll.GetEquipmentReceives(input);

            //assert
            Assert.IsNotNull(equipmentReceives);
            Assert.AreEqual(RowCount, equipmentReceives.Count);
        }

         [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateEquipmentReceive_WhenNull_ThrowException()
        {
            //arrange
            _equipmentMovementRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateEquipmentReceive(new EquipmentReceiveDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

         [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateEquipmentReceive_WhenQtyReceiveLargerThanQtyTransfer_ThrowException()
        {
            //arrange
            var equipmentReceive = new EquipmentReceiveDTO()
            {
                QtyReceive = 10,
                QtyTransfer = 5
            };

            _equipmentMovementRepo.GetByID().ReturnsForAnyArgs(new MntcEquipmentMovement());

            try
            {
                //act
                _bll.UpdateEquipmentReceive(equipmentReceive);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.QtyReceiveLargerThanQtyTransfer.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateEquipmentReceive_WhenValid_Update()
        {
            //arrange
            var equipmentReceive = new EquipmentReceiveDTO()
            {
                TransferDate = DateTime.Now.Date,
                LocationCodeSource = "XXX",
                ItemCode = "IC123",
                QtyReceive = 5,
                QtyTransfer = 10
            };
            var dbMntcEquipmentMovement = new MntcEquipmentMovement()
            {
                TransferDate = DateTime.Now.Date,
                LocationCodeSource = "XXX",
                ItemCode = "IC123",
                QtyReceive = 0
            };

            _equipmentMovementRepo.GetByID().ReturnsForAnyArgs(dbMntcEquipmentMovement);

            //act
            _bll.UpdateEquipmentReceive(equipmentReceive);

            //assert
            Assert.AreEqual(equipmentReceive.QtyReceive, dbMntcEquipmentMovement.QtyReceive);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
