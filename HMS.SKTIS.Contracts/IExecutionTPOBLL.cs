using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects;
using Excel;

namespace HMS.SKTIS.Contracts
{
    public interface IExecutionTPOBLL
    {
        #region TPO Actual Work Hour
        ExeTPOActualWorkHoursDTO InsertUpdateExeTpoActualWorkHours(ExeTPOActualWorkHoursDTO post);
        List<ExeTPOActualWorkHoursDTO> GetExeTpoActualWorkHours(GetExePlantActualWorkHoursInput input);
        #endregion
        
        #region TPO Production Entry
        List<ExeTPOProductionViewDTO> GetExeTPOProductionEntry(GetExeTPOProductionInput input);

        List<ExeTPOProductionViewDTO> getAbsentActualProdNotNullByProdEntryCodeAndStatusIdentifier(String TransactionCode, String statusEmployee);
        ExeTPOProductionViewDTO SaveExeTPOProductionEntry(ExeTPOProductionViewDTO ExeTPOProductionDTO, string OriginalEmpStatus, bool verifySystem);
        List<PlanTPOTPKCompositeDTO> GetTpoTpkValue(string locationCode, string brand, int year, int week, DateTime? date, string process, string status);
        List<string> GetEmpStatusFromExeTPOProductionEntry(string locationCode, string brandCode, DateTime? date);
        List<string> GetStatusEmpActiveByLocationAndDate(string locationCode, DateTime? date, string BrandCode, string ProcessGroup);
        List<string> GetBrandCodeFromExeTPOProductionEntryVerificationByLocationDate(string locationcode, DateTime? date);
        int GetStatusIdentifierFromTpoProductionByStatusEmp(string statusEmp);
        void UpdateTPOProductionEntryWorkerCount(string ProductionCode, string Status, MstTPOProductionGroupDTO MstProductionGroup, ExeTPOProductionEntryVerificationViewDTO entry, string Username);
        void InsertGroupTPOProductionEntry(string ProductionCode, string Status,
            MstTPOProductionGroupDTO MstProductionGroup, ExeTPOProductionEntryVerificationViewDTO entry, string Username);
        void DeleteTPOProductionEntry(string ProductionCode, string StatusEmployee, string ProductionGroup);
        List<string> GetStatusEmpActiveByLocationAndDateTPOTPK(string locationCode, DateTime? date);
        #endregion

        #region TPO Production Entry Verification
        List<ExeTPOProductionEntryVerificationViewDTO> GetExeTPOProductionEntryVerification(GetExeTPOProductionEntryVerificationInput input);
        List<ExeTPOProductionEntryVerificationViewDTO> GetExeTPOProductionEntryVerificationBetweenProductionDates(string LocationCode, string Process, DateTime StartDate, DateTime? EndDate);
        ExeTPOProductionEntryVerificationDTO SaveExeTPOProductionEntryVerification(ExeTPOProductionEntryVerificationDTO ExeTPOProductionVerificationDTO);
        List<ExePlantActualWorkHoursDTO> GetActualWorkHoursByProductionEntryVerification(GetExeTPOProductionEntryVerificationInput input);
        List<ExeTPOActualWorkHoursDTO> GetTpoActualWorkHoursByProductionEntryVerivication(GetExeTPOProductionEntryVerificationInput input);

        List<PlanTPOTPKCompositeDTO> GetTpoTpkValueDistinct(string locationCode, string brand, int year, int week, DateTime? date,string processGroup);
        int TPOProductionEntryVerificationGenerateReport(GetExeTPOProductionEntryVerificationInput input, string userName);

        void TpoProductionEntryVerificationGenerateReportAsync(GetExeTPOProductionEntryVerificationInput input, string userName);

        void GenerateAllAsyncSequence(List<string> input, string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy, string updatedBy, string unitCode, bool generateTpo);
        int TPOProductionEntryVerificationCancelReport(GetExeTPOProductionEntryVerificationInput input);
        List<ExeTPOProductionEntryVerificationDTO> GeTpoProductionEntryVerifications(GetExeTPOProductionEntryVerificationInput input);
        List<string> GetBrandCodeFromExeTPOProductionEntryVerification(GetExeTPOProductionEntryVerificationInput input);
        void InsertTPOExeReportByGroups(string locationCode, string brand, int? year, int? week, DateTime? date,string createdBy);

        void InsertTPOExeReportByGroupsAsync(string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy);
        void UpdateVerifyAndFlagTPOEntryVerification(string productionCode, bool verifySystem, bool verifyManual, bool flagManual);
        string GetStatusTPOFee(GetExeTPOProductionEntryVerificationInput input);
        IEnumerable<string> GetListProcessVerification(string locationCode, string brandCode, DateTime productionDate);
        #endregion

        void UploadExcelTPOEntryDaily(IExcelDataReader excelReader, string username, IEnumerable<string> listUserAccesLocationCode);

        void SubmitTPOEntryVerificationSP(string locationCode, string brandCode, int year, int week, DateTime prodDate, string process, string user, int role);
    }
}
