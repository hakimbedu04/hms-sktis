using System.Threading.Tasks;

namespace HMS.SKTIS.Contracts
{
    public interface ISSISPackageService
    {
        void RunWPPSSISPackage(string userName);

        void RunPlanTPKSSISPackage(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode, int shift);

        void RunSSISProductionEntryPlant(string userName, int kpsYear, int kpsWeek, string locationCode,string brandCode, string unitCode);
        void RunSSISProductionEntryTPO(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode);
    }
}
