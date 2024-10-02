using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public partial class MaintenanceBLLTests
    {
        private const int RowCount = 10;

        private IUnitOfWork _unitOfWork;
        private IMasterDataBLL _masterDataBLL;
        private IMaintenanceBLL _bll;
        
        private IGenericRepository<MntcEquipmentMovement> _equipmentMovementRepo;
        private IGenericRepository<MntcInventory> _maintenanceInventoryRepo;

        [TestInitialize]
        public void TestSetup()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _masterDataBLL = Substitute.For<IMasterDataBLL>();

            _equipmentMovementRepo = Substitute.For<IGenericRepository<MntcEquipmentMovement>>();
            _maintenanceInventoryRepo = Substitute.For<IGenericRepository<MntcInventory>>();

            _unitOfWork.GetGenericRepository<MntcEquipmentMovement>().ReturnsForAnyArgs(_equipmentMovementRepo);
            _unitOfWork.GetGenericRepository<MntcInventory>().ReturnsForAnyArgs(_maintenanceInventoryRepo);

            _bll = new MaintenanceBLL(_unitOfWork, _masterDataBLL);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _unitOfWork = null;
            _masterDataBLL = null;
            _bll = null;

            _equipmentMovementRepo = null;
            _maintenanceInventoryRepo = null;
        }
    }
}
