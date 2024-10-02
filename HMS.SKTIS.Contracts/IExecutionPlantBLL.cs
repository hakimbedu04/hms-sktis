using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.Contracts
{
    public interface IExecutionPlantBLL
    {
        #region Worker Absenteeism
        List<ExePlantWorkerAbsenteeismViewDTO> GetWorkerAbsenteeism(GetExePlantWorkerAbsenteeismInput input);
        List<ExePlantWorkerAbsenteeismViewDTO> GetWorkerAbsenteeismDaily(GetExePlantWorkerAbsenteeismInput input);
        ExePlantWorkerAbsenteeismDTO InsertWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeism);
        ExePlantWorkerAbsenteeismDTO UpdateWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeism);
        int GetMaxDayByAbsentType(string absentType);
        int CalculateEmployeeActualAndStdStickPerHour(GetExePlantWorkerAbsenteeismInput input);
        int GetMaxDayProdCard(string idEmployee, string absentType, int year);
        List<SuratPeriodeComposite> GetAbsenteeimForSuratPeriodeComposites(string employeeId, string productionDate, string locationCode, string unitCode, int shift, string groupCode, string processGroup, string brandCode, string remark);

        void CheckEblekStatusOnInsertAbsenteeism(GetExePlantWorkerAbsenteeismExcelPieceRateInput input, DateTime newStartAbsentDate, DateTime newEndAbsentDate, string employeeID);
        void CheckEblekStatusOnEditAbsenteeism(GetExePlantWorkerAbsenteeismExcelPieceRateInput input, DateTime newStartAbsentDate, DateTime oldStartDateAbsent, DateTime newEndAbsentDate, DateTime oldEndAbsentDate, string employeeID);

        ExePlantWorkerAbsenteeismDTO InsertWorkerAbsenteeism_SP(ExePlantWorkerAbsenteeismDTO workerAbsenteeism);
        ExePlantWorkerAbsenteeismDTO UpdateWorkerAbsenteeism_SP(ExePlantWorkerAbsenteeismDTO workerAbsenteeism);
        #endregion

        #region Production Entry
        List<ExePlantProductionEntryDTO> GetPlantProductionEntrys(GetExePlantProductionEntryInput input);
        string GetPlantProductionEntryBrand(string employeeID, DateTime productionDate, string shift, string unitCode, string groupCode);
        ExePlantProductionEntryDTO SaveProductionEntry(ExePlantProductionEntryDTO eblekDto);
        List<string> GetExePlantProductionEntryVerificationBrand(GetExePlantProductionEntryVerificationBrandInput input);
        List<ExePlantProductionEntryAllocationCompositeDTO> GetPlantProductionEntryAllocation(GetExePlantProductionEntryInput input);
        ExePlantProductionEntryAllocationCompositeDTO DeleteProductionEntry(ExePlantProductionEntryDTO eblekDtos);
        ExePlantProductionEntryDTO GetExePlantProductionEntryByCode(GetExePlantProductionEntryInput input);
        void SubmitProductionEntry(string locationCode, string unit, string brand, int? shift, int? year, int? week, DateTime? date, string groupCode, string createdBy);

        List<string> GetBrandCodeByLocationYearAndWeekEntryVerification(string locationCode, int? KPSYear, int? KPSWeek);
        List<ExePlantProductionEntryVerificationDTO> GetBrandCodeByLocationYearWeekProcessFromEntryVerification(string locationCode, string ProcessGroup, int? KPSYear, int? KPSWeek);

        List<ExePlantProductionEntryVerificationDTO> GetExePlantProductionEntryVerification(
            GetExePlantProductionEntryVerificationInput input);
        ExeProductionEntryMinimumValueDTO GetMinimumValueForActualProdEntry(GetExePlantProductionEntryInput input);

        List<ExePlantProductionEntryDTO> GetPlantProductionEntrysTuning(GetExePlantProductionEntryInput input);
        ExePlantProductionEntryDTO SaveProductionEntry_SP(ExePlantProductionEntryDTO eblekDto);
        void SaveDefaultTargetActualProdEntry_SP(string productionEntryCode, string saveType);
        #endregion

        #region Actual Work Hours
        bool CheckCompletedExePlantActualWorkHours(GetExePlantProductionEntryInput input);
        List<ExePlantActualWorkHoursDTO> GetExePlantActualWorkHours(GetExePlantActualWorkHoursInput input);
        ExePlantActualWorkHoursDTO InsertUpdateExePlantActualWorkHours(ExePlantActualWorkHoursDTO post);
        #endregion

        #region Worker Assignment
        List<ExePlantWorkerAssignmentDTO> GetExePlantWorkerAssignments(GetExePlantWorkerAssignmentInput input);
        ExePlantWorkerAssignmentDTO InsertWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignment);
        //void InsertWorkerAssignmentDaily(ExePlantWorkerAssignmentDTO workerAssignment);
        ExePlantWorkerAssignmentDTO UpdateWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignment);
        List<ExePlantWorkerAssignmentDTO> IsExistWorkerAssignment(GetExePlantWorkerAssignmentInput input);
        bool IsExistWorkerAbsenteeismByWorkerAssignment(GetExePlantWorkerAbsenteeismInput input);
        List<GroupCodesPopUpDTO> GetGroupCodePopUp(GetGroupCodePopUpWorkerAssignmentInput input);
        List<string> GetGroupCodePopUpDaily(string locationCode, string unitCode, string processCode);
        List<string> GetBrandFromPlantEntryVerification(string locationCode, string unitCode, int? shift);
        List<string> GetBrandFromPlantEntryVerificationByDate(string locationCode, string unitCode, int? shift, int? year, int? week, DateTime? date);
        List<string> GetBrandFromPlantTpu(string locationCode, string unitCode, int? shift, int? year, int? week);
        IEnumerable<ProductionCardDTO> GetProductionCardSubmitted(GetExePlantWorkerAbsenteeismInput input);
        void DeletetWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignment);


        ExePlantWorkerAssignmentDTO InsertWorkerAssignment_SP(ExePlantWorkerAssignmentDTO workerAssignment);
        ExePlantWorkerAssignmentDTO UpdateWorkerAssignment_SP(ExePlantWorkerAssignmentDTO workerAssignment);
        ExePlantWorkerAssignmentDTO DeleteWorkerAssignment_SP(ExePlantWorkerAssignmentDTO workerAssignment);
        #endregion

        #region Worker Balancing

        List<ExePlantWorkerBalancingViewDTO> GetExePlantWorkerLoadBalancing(GetExePlantWorkerLoadBalancingMulti input);
        List<ExePlantWorkerBalancingSingleViewDTO> GetPlantWorkerLoadBalancingSingle(GetExePlantWorkerLoadBalancingSingle input);
        
        List<ExePlantWorkerBalancingProcessPerUnitDTO> GetWorkerBalancingProcessPerUnit(string locationCode, string unitCode, int? shift, string brandCode);

        #endregion

        #region Material Usages
        List<ExePlantMaterialUsagesDTO> GetMaterialUsages(GetExePlantMaterialUsagesInput input);
        ExePlantMaterialUsagesDTO SaveMaterialUsage(ExePlantMaterialUsagesDTO materialUsagesDto);

        #endregion

        #region ExePlant Production Entry Verification
        List<ExePlantProductionEntryVerificationViewDTO> GetExePlantProductionEntryVerificationViews(GetExePlantProductionEntryVerificationInput input);
        ExePlantProductionEntryVerificationViewDTO UpdatExePlantProductionEntryVerification(ExePlantProductionEntryVerificationViewDTO verificationViewDto);
        void SaveProductionCardFromProductionEntry(ExePlantProductionEntryVerificationViewDTO verificationViewDto);
        void UpdatExePlantProductionEntryVerificationWhenSubmit(ExePlantProductionEntryVerificationViewDTO verificationViewDto);
        void SubmitProductionEntryVerification(string locationCode, string unit, string brand, int? shift, int? year,int? week, DateTime? date, string groupCode, string createdBy);

        void InsertReportByProcess(string locationCode, string brand, string process, int? year, int? week, string createdBy, string updatedBy, DateTime productionDate, string unitCode);
        void InsertReportByProcessAsync(string locationCode, string brand, string process, int? year, int? week, string createdBy, string updatedBy, DateTime productionDate, string unitCode);

        void DefaultExeReportByProcess(string locationCode, string brandCode, string unitCode, int? year, int? week, DateTime productionDate, string createdBy, string updatedBy);
        void InsertDefaultExeReportByProcess(string locationCode, string brandCode, string unitCode, int? year, int? week, DateTime productionDate, string createdBy, string updatedBy);
        void SendEmailSubmitTpotEntryVerif(GetExeTPOProductionEntryVerificationInput input, string currUserName);
        void switchBrandExeReportByProcess(string locationCode, string brandGroupCode, DateTime productionDate);
        void recalculateStockExeReportByProcess(string locationCode, string brandCode, DateTime productionDate);
        void DeleteReportByProccess(string location, string unitCode, string brandCode, DateTime productionDateFrom, DateTime productionDateTo);
        List<ExePlantProductionEntryVerificationDTO> GetPlantProductionEntryVerificationByLocationYearWeek(
            string locationCode,
            int? KPSYear,
            int? KPSWeek
        );

        void ReturnProductionEntryVerification(string productionEntryCode);

        void UpdateWorkHourReportByGroup(GetExePlantProductionEntryVerificationInput input, float? totalActualValue, string currUsername);
        void RunSSISUpdateByGroupWeekly();
        void RunSSISUpdateByGroupMonthly();
        void SaveProductionCardFromProductionEntryTuning(ExePlantProductionEntryVerificationViewDTO verificationViewDto, string brandGroupCode, MstGenStandardHourDTO mstGenStdHour);
        List<ExePlantProductionEntryVerificationViewDTO> GetVerificationForFilterGroupCode(string locationCode, string unitCode, int shift, string processGroup, DateTime productionDate);
        List<string> GetVerificationForFilterProcessGroup(string locationCode, string unitCode, int shift, DateTime productionDate);

        List<string> GetGroupCodePlantVerification(string locationCode, string unitCode, string process, int week);

        List<string> GetGroupCodePlantTPK(string locationCode, string unitCode, string process, string brandCode, int year, int week);
        #endregion

        int GetLatestProdCardRevType(string location, string unit, string brand, string process, string group, DateTime? productionDate);

        #region EMS Source Data
        List<string> GetBrandCodeByLocationDateFromDateToEntryVerification(string locationCode, DateTime? DateFrom, DateTime? DateTo);
        List<string> GetBrandCodeByLocationDateFromDateToBySp(string locationCode, DateTime? dateFrom, DateTime? dateTo);

        #endregion

        void GenerateProductionCardVerification_SP(IEnumerable<ExePlantProductionEntryVerificationViewDTO> listVerificationDTO, GetProductionCardInput input);

        bool CheckEblekSubmittedOnEditAssingment(GetExePlantWorkerAssignmentInput input);

        IEnumerable<string> GetListProcessGeneratorByProcess(string locationCode, string unitCode, string brandCode, int shift, DateTime productionDate);
        IEnumerable<string> GetListProcessVerification(string locationCode, string unitCode, string brandCode, int shift, DateTime productionDate);
        void InsertReportByProcessGenerator(string locationCode, string brand, string process, int? year, int? week, string createdBy,
            string updatedBy, DateTime productionDate, string unitCode);


        #region Multiple Absenteeism Pop Up - CR1

        IEnumerable<string> GetListAbsentActiveOnAbsenteeism();
        List<EmployeeMultipleInsertAbsenteeismDTO> InsertMultipleAbsenteeism(InsertMultipleAbsenteeismDTO datas);

        #endregion
    }
}
