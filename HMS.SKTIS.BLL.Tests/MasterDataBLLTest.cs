using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Voxteneo.WebComponents.Logger;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public partial class MasterDataBLLTest
    {
        private const int RowCount = 10;

        private ILogger _logger;
        private IUnitOfWork _unitOfWork;
        private IMasterDataBLL _bll;
        private IGenericRepository<MstGeneralList> _mstGeneralListRepo;
        private IGenericRepository<MstUnit> _mstUnitRepo;
        private IGenericRepository<MstEmployeeJobsDataActive> _mstEmployeeJobsDataActive;

        [TestInitialize]
        public void TestSetup()
        {
            _logger = Substitute.For<ILogger>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mstGeneralListRepo = Substitute.For<IGenericRepository<MstGeneralList>>();
            _mstUnitRepo = Substitute.For<IGenericRepository<MstUnit>>();
            _mstEmployeeJobsDataActive = Substitute.For<IGenericRepository<MstEmployeeJobsDataActive>>();

            _unitOfWork.GetGenericRepository<MstGeneralList>().ReturnsForAnyArgs(_mstGeneralListRepo);
            _unitOfWork.GetGenericRepository<MstUnit>().ReturnsForAnyArgs(_mstUnitRepo);
            _unitOfWork.GetGenericRepository<MstEmployeeJobsDataActive>().ReturnsForAnyArgs(_mstEmployeeJobsDataActive);

            _bll = new MasterDataBLL(_logger, _unitOfWork);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _logger = null;
            _unitOfWork = null;
            _bll = null;
            _mstGeneralListRepo = null;
            _mstUnitRepo = null;
            _mstEmployeeJobsDataActive = null;
        }

    }
}
