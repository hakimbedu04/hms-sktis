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
        [TestMethod]
        public void GetEquipmentTransfers_NotNull()
        {
            //arange
            var mntcEquipmentMovementsStub = GetMntcEquipmentMovementsStub();
            _equipmentMovementRepo.Get().ReturnsForAnyArgs(mntcEquipmentMovementsStub);
            var input = new GetEquipmentTransfersInput();

            //act
            var equipmentTransfers = _bll.GetEquipmentTransfers(input);

            //assert
            Assert.IsNotNull(equipmentTransfers);
            Assert.AreEqual(RowCount, equipmentTransfers.Count);
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertEquipmentTansfer_WhenNotNull_ThrowException()
        {
            //arrange
            _equipmentMovementRepo.GetByID().ReturnsForAnyArgs(new MntcEquipmentMovement());

            try
            {
                //act
                _bll.InsertEquipmentTransfer(new EquipmentTransferDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataAlreadyExist.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertEquipmentTansfer_WhenNoInventory_ThrowException()
        {
            //arrange
            var equipmentTransfer = new EquipmentTransferDTO()
            {
                LocationCodeSource = "ID21",
                QtyTransfer = 5,
                ItemCode = "IC001"
            };
            _equipmentMovementRepo.GetByID().ReturnsNullForAnyArgs();
            _maintenanceInventoryRepo.Get().ReturnsForAnyArgs(new List<MntcInventory>());

            try
            {
                //act
                _bll.InsertEquipmentTransfer(equipmentTransfer);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.TransferQtyLargerThanAvailableStock.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void InsertEquipmentTansfer_WhenStockInventoryLessThanQtyTransfer_ThrowException()
        {
            //arrange
            var equipmentTransfer = new EquipmentTransferDTO() { LocationCodeSource = "ID21", QtyTransfer = 10 };
            _equipmentMovementRepo.GetByID().ReturnsNullForAnyArgs();
            _maintenanceInventoryRepo.Get().ReturnsForAnyArgs(new List<MntcInventory>()
            {
                new MntcInventory()
                {
                    EndingStock = 5
                }
            });

            try
            {
                //act
                _bll.InsertEquipmentTransfer(equipmentTransfer);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.TransferQtyLargerThanAvailableStock.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void InsertEquipmentTansfer_WhenValid_Insert()
        {
            //arrange
            var equipmentTransfer = new EquipmentTransferDTO()
            {
                LocationCodeSource = "ID21",
                TransferDate = DateTime.Now.Date,
                ItemCode = "IC001",
                QtyTransfer = 5
            };
            _equipmentMovementRepo.GetByID().ReturnsNullForAnyArgs();
            _maintenanceInventoryRepo.Get().ReturnsForAnyArgs(new List<MntcInventory>()
            {
                new MntcInventory()
                {
                    EndingStock = 10
                }
            });

            //act
            _bll.InsertEquipmentTransfer(equipmentTransfer);

            //assert
            _equipmentMovementRepo.Received()
                .Insert(Arg.Is<MntcEquipmentMovement>(m => m.LocationCodeSource == equipmentTransfer.LocationCodeSource
                    && m.TransferDate == equipmentTransfer.TransferDate
                    && m.ItemCode == equipmentTransfer.ItemCode
                    && m.QtyTransfer == equipmentTransfer.QtyTransfer));
            _unitOfWork.Received().SaveChanges();
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateEquipmentTansfer_WhenNull_ThrowException()
        {
            //arrange
            _equipmentMovementRepo.GetByID().ReturnsNullForAnyArgs();

            try
            {
                //act
                _bll.UpdateEquipmentTransfer(new EquipmentTransferDTO());
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.DataNotFound.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateEquipmentTansfer_WhenNoInventory_ThrowException()
        {
            //arrange
            var equipmentTransfer = new EquipmentTransferDTO() { LocationCodeSource = "ID21", QtyTransfer = 5 };
            _equipmentMovementRepo.GetByID().ReturnsForAnyArgs(new MntcEquipmentMovement());
            _maintenanceInventoryRepo.Get().ReturnsForAnyArgs(new List<MntcInventory>());

            try
            {
                //act
                _bll.UpdateEquipmentTransfer(equipmentTransfer);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.TransferQtyLargerThanAvailableStock.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod, ExpectedException(typeof(BLLException))]
        public void UpdateEquipmentTansfer_WhenStockInventoryLessThanQtyTransfer_ThrowException()
        {
            //arrange
            var equipmentTransfer = new EquipmentTransferDTO() { LocationCodeSource = "ID21", QtyTransfer = 10 };
            _equipmentMovementRepo.GetByID().ReturnsForAnyArgs(new MntcEquipmentMovement());
            _maintenanceInventoryRepo.Get().ReturnsForAnyArgs(new List<MntcInventory>()
            {
                new MntcInventory()
                {
                    EndingStock = 5
                }
            });

            try
            {
                //act
                _bll.UpdateEquipmentTransfer(equipmentTransfer);
            }
            catch (BLLException ex)
            {
                //assert
                Assert.AreEqual(ExceptionCodes.BLLExceptions.TransferQtyLargerThanAvailableStock.ToString(), ex.Code);
                throw;
            }
        }

        [TestMethod]
        public void UpdateEquipmentTransfer_WhenValid_Update()
        {
            //arrange
            var equipmentTransfer = new EquipmentTransferDTO()
            {
                TransferDate = DateTime.Now.Date,
                LocationCodeSource = "XXX",
                ItemCode = "IC123",
                QtyTransfer = 5
            };
            var dbMntcEquipmentMovement = new MntcEquipmentMovement()
            {
                TransferDate = DateTime.Now.Date,
                LocationCodeSource = "XXX",
                ItemCode = "IC123",
                QtyTransfer = 0
            };

            _equipmentMovementRepo.GetByID().ReturnsForAnyArgs(dbMntcEquipmentMovement);
            _maintenanceInventoryRepo.Get().ReturnsForAnyArgs(new List<MntcInventory>()
            {
                new MntcInventory()
                {
                    EndingStock = 10
                }
            });

            //act
            _bll.UpdateEquipmentTransfer(equipmentTransfer);

            //assert
            Assert.AreEqual(equipmentTransfer.QtyTransfer, dbMntcEquipmentMovement.QtyTransfer);
            _unitOfWork.Received().SaveChanges();
        }
    }
}
