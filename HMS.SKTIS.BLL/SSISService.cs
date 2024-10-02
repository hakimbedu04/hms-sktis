using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Contracts;

namespace HMS.SKTIS.BLL
{
    public class SSISService : ISSISPackageService
    {
        private IUnitOfWork _uow;
        private ISqlSPRepository _sqlSpRepo;


        public SSISService(IUnitOfWork uow)
        {
            _uow = uow;
            _sqlSpRepo = _uow.GetSPRepository();
        }

        public void RunWPPSSISPackage(string userName)
        {
            _sqlSpRepo.GenerateWPP(userName);
        }

        public void RunPlanTPKSSISPackage(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode, int shift)
        {

            _sqlSpRepo.GeneratePlantTPK(userName, kpsYear, kpsWeek, locationCode, brandCode, shift);
        }

        public void RunSSISProductionEntryPlant(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode, string unitCode)
        {
            _sqlSpRepo.RunSSISProductionEntryPlant(userName, kpsYear, kpsWeek, locationCode, brandCode, unitCode);
        }
        public void RunSSISProductionEntryTPO(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode)
        {
            _sqlSpRepo.RunSSISProductionEntryTPO(userName, kpsYear, kpsWeek, locationCode, brandCode);
        }
    }
}
