using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HMS.SKTIS.BLL.Tests
{
    [TestClass]
    public partial class MasterDataBLLTest
    {
        private const int RowCount = 10;

        private IUnitOfWork _unitOfWork;
        private IMasterDataBLL _bll;
        private IGenericRepository<MstGenList> _mstGenListRepo;
        private IGenericRepository<MstPlantUnit> _mstPlantUnitRepo;
        private IGenericRepository<MstPlantEmpJobsDataAcv> _mstPlantEmpJobsDataAcvRepo;
        private IGenericRepository<MstPlantProductionGroup> _mstPlantProductionGroupRepo;
        private IGenericRepository<MstGenLocation> _mstGenLocationRepo;
        private IGenericRepository<MstGenStandardHour> _mstGenStandardHourRepo;
        private IGenericRepository<MstGenBrandGroup> _mstGenBrandGroupRepo;
        private IGenericRepository<MstGenProcess> _mstGenProcessRepo;
        private IGenericRepository<MstTPOInfo> _mstTPOInfoRepo;
        private IGenericRepository<MstTPOPackage> _mstTPOPackageRepo;
        private IGenericRepository<MstPlantUnitView> _mstPlantUnitViewRepo;
        private IGenericRepository<MstPlantProductionGroupView> _mstPlantProductionGroupViewRepo;
            
        [TestInitialize]
        public void TestSetup()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _mstGenListRepo = Substitute.For<IGenericRepository<MstGenList>>();
            _mstPlantUnitRepo = Substitute.For<IGenericRepository<MstPlantUnit>>();
            _mstPlantEmpJobsDataAcvRepo = Substitute.For<IGenericRepository<MstPlantEmpJobsDataAcv>>();
            _mstPlantProductionGroupRepo = Substitute.For<IGenericRepository<MstPlantProductionGroup>>();
            _mstGenLocationRepo = Substitute.For<IGenericRepository<MstGenLocation>>();
            _mstGenStandardHourRepo = Substitute.For<IGenericRepository<MstGenStandardHour>>();
            _mstGenBrandGroupRepo = Substitute.For<IGenericRepository<MstGenBrandGroup>>();
            _mstGenProcessRepo = Substitute.For<IGenericRepository<MstGenProcess>>();
            _mstTPOInfoRepo = Substitute.For<IGenericRepository<MstTPOInfo>>();
            _mstTPOPackageRepo = Substitute.For<IGenericRepository<MstTPOPackage>>();
            _mstPlantUnitViewRepo = Substitute.For<IGenericRepository<MstPlantUnitView>>();
            _mstPlantProductionGroupViewRepo = Substitute.For<IGenericRepository<MstPlantProductionGroupView>>();

            _unitOfWork.GetGenericRepository<MstGenList>().ReturnsForAnyArgs(_mstGenListRepo);
            _unitOfWork.GetGenericRepository<MstPlantUnit>().ReturnsForAnyArgs(_mstPlantUnitRepo);
            _unitOfWork.GetGenericRepository<MstPlantEmpJobsDataAcv>().ReturnsForAnyArgs(_mstPlantEmpJobsDataAcvRepo);
            _unitOfWork.GetGenericRepository<MstPlantProductionGroup>().ReturnsForAnyArgs(_mstPlantProductionGroupRepo);
            _unitOfWork.GetGenericRepository<MstGenLocation>().ReturnsForAnyArgs(_mstGenLocationRepo);
            _unitOfWork.GetGenericRepository<MstGenStandardHour>().ReturnsForAnyArgs(_mstGenStandardHourRepo);
            _unitOfWork.GetGenericRepository<MstGenBrandGroup>().ReturnsForAnyArgs(_mstGenBrandGroupRepo);
            _unitOfWork.GetGenericRepository<MstGenProcess>().ReturnsForAnyArgs(_mstGenProcessRepo);
            _unitOfWork.GetGenericRepository<MstTPOInfo>().ReturnsForAnyArgs(_mstTPOInfoRepo);
            _unitOfWork.GetGenericRepository<MstTPOPackage>().ReturnsForAnyArgs(_mstTPOPackageRepo);
            _unitOfWork.GetGenericRepository<MstPlantUnitView>().ReturnsForAnyArgs(_mstPlantUnitViewRepo);
            _unitOfWork.GetGenericRepository<MstPlantProductionGroupView>()
                .ReturnsForAnyArgs(_mstPlantProductionGroupViewRepo);

            _bll = new MasterDataBLL(_unitOfWork);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _unitOfWork = null;
            _bll = null;
            _mstGenListRepo = null;
            _mstPlantUnitRepo = null;
            _mstPlantEmpJobsDataAcvRepo = null;
            _mstPlantProductionGroupRepo = null;
            _mstGenLocationRepo = null;
            _mstGenStandardHourRepo = null;
            _mstGenBrandGroupRepo = null;
            _mstGenProcessRepo = null;
            _mstTPOInfoRepo = null;
            _mstTPOPackageRepo = null;
            _mstPlantUnitViewRepo = null;
            _mstPlantProductionGroupViewRepo = null;
        }

    }
}
