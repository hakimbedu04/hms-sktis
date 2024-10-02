using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using System.Linq;
using HMS.SKTIS.Core;

namespace HMS.SKTIS.Contracts
{
    public interface ISqlSPRepository
    {
        IEnumerable<GetLocations_Result> GetLocations(string locationCode, int level = -1);
        IEnumerable<GetLocationsByLevel_Result> GetLocationsByLevel(string sourceLocationCode, int level = 0);
        IEnumerable<GetMstGenLocationsByParentCode_Result> GetMstGenLocationsByParentCode(GetMstGenLocationsByParentCodeInput input);
        IEnumerable<GetLastChildLocation_Result> GetLastChildLocationByLocationCode(string locationCode);
        IEnumerable<GetUserAndEmailPlanTPUSubmit_Result> GetUserAndEmailPlantTpuSubmit(GetPlanTPUsInput input);
        IEnumerable<GetUserAndEmail_Result> GetUserAndEmail(GetUserAndEmailInput input);
        void InsertEmailPlantTpu(MailInput input);
        void InsertEmail(MailInput input);
        IEnumerable<GetReportSummaryDailyProductionTargets_Result> GetReportSummaryDailyProductionTargets(int year, int week, int comma, string location);
        Task GeneratePlantTPK(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode,
            int shift);
        Task GenerateWPP(string userName);

        IEnumerable<GetReportSummaryProcessTargets_Result> GetReportSummaryProcessTargets(string locationCode, int? year, int? week, DateTime? dateFrom, DateTime? dateTo, int comma, string FilterType);
        IEnumerable<GetTPOReportsProduction_Result> GetTpoReportsProduction(HMS.SKTIS.BusinessObjects.Inputs.TPOFee.GetTPOReportsProductionInput input);
        IEnumerable<GetEquipmentRequirementReport_Result> GetMaintenanceEquipmentRequirementReport(string locationCode, string brandGroupCode, float? userPackage, DateTime date);
        IEnumerable<GetEquipmenrRequiremenrReport2_Result> GetMaintenanceEquipmentRequirementReport2(string locationCode, string brandGroupCodeFrom, string brandGroupCodeTo, DateTime date);
        IEnumerable<GetEquipmentRequirementItem_Result> GetEquipmentRequirementSummary(string locationCode, string brandGroupCode, DateTime date);
        IQueryable<int?> GetRealStock(string locationCode, string itemCode, DateTime date);
        IEnumerable<GetWorkerBrandAssignmentPlanningPlantTPK_Result> GetWorkerBrandAssignmentPlanningPlantTpk(GetPlanPlantAllocation filter);
        Task RunSSISProductionEntryPlant(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode, string unitCode);
        Task RunSSISProductionEntryTPO(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode);
        int ExeTransactionLog(string separator, string page, int year, int week, string code_1, string code_2, string code_3, string code_4, string code_5, string code_6, string code_7, string code_8, string code_9, DateTime TransactionDate, string ActionButton, DateTime ActionTime, string UserName, string TransactionCode, string Message, int? IDRole);

        Task ExeTransactionLogTask(string separator, string page, int year, int week, string code_1, string code_2, string code_3, string code_4, string code_5, string code_6, string code_7, string code_8, string code_9, DateTime TransactionDate, string ActionButton, DateTime ActionTime, string UserName, string TransactionCode, string Message, int? IDRole);

        #region Production Execution

        #region TPO

        #region Production Entry Verification
        int TPOProductionEntryVerificationGenerateReport(string LocationCode, string BrandCode, int? KPSYear, int? KPSWeek, DateTime? ProductionDate, string userName);

        Task TpoProductionEntryVerificationGenerateReportTask(string LocationCode, string BrandCode, int? KPSYear, int? KPSWeek, DateTime? ProductionDate, string userName);
        int TPOProductionEntryVerificationCancelReport(string LocationCode, string BrandCode, int? KPSYear, int? KPSWeek, DateTime? ProductionDate);
        Task GenerateAllAsyncSequenceTask(List<string> input, string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy, string updatedBy, string unitCode, bool generateTpo);
        void GenerateByProcess(string locationCode, string brand, List<string> input, int? year, int? week, string createdBy, string updatedBy, DateTime? date, string unitCode);

        //Task GenerateByProcessTaskLoop(string locationCode, string brand, List<string> input, int? year, int? week, string createdBy, string updatedBy, DateTime? date, string unitCode);
        Task GenerateByProcessTask(string locationCode, string brand, string processCode, int? year, int? week, string createdBy, string updatedBy, DateTime? date, string unitCode);
        void GenerateTpoFee(string locationCode, string brandCode, int? year, int? week, DateTime? productionDate,string userName, bool generateTpo);
        #endregion

        #endregion

        #endregion        

        #region Utilities

        #region Transaction History and Flow

        IQueryable<int?> RoleButtonChecker(string tPOFeeCode, int iDRole, string page, string button);
        IEnumerable<GetTransactionHistory_Result> GetTransactionHistory(string transactionCode, string pageSource);

        IEnumerable<GetInventoryView_Result> GetInventoryView(string date, string locationCode, string itemType);

        IEnumerable<GetTransactionHistoryWagesProdcardCorrection_Result> GetTransactionHistoryWagesProdcardCorrection(string transactionCode);

        IEnumerable<GetTransactionFlow_Result> GetTransactionFlow(string functionName);

        #endregion

        IEnumerable<GetUserAdByRoleLocation_Result> GetUserAdByRoleLocation(string roleCode, string location);

        #endregion

        void SubmitTpoTpk(string locationCode, string brandCode, int? kpsYear, int? kpsWeek, string userName);
        IEnumerable<RoleButtonWagesApprovalDetail_Result> RoleButtonWagesApprovalDetail(ButtonStateInput input);
        void InsertPlantExeReportByGroups(string locationCode, string unit, string brand, int? shift, int? year,int? week, DateTime? date, string groupCode, string createdBy);
        void InsertTPOExeReportByGroups(string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy);
        Task InsertTPOExeReportByGroupsTask(string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy);
        void InsertPlantExeReportByProcess(string locationCode, string brand, string process, int? year, int? week, string createdBy, string updatedBy, DateTime productionDate, string unitCode);
        Task InsertPlantExeReportByProcessTask(string locationCode, string brand, string process, int? year, int? week, string createdBy, string updatedBy, DateTime productionDate, string unitCode);
        void DefaultExeReportByProcess(string locationCode, string brandCode, string unitCode, int? year, int? week, DateTime productionDate, string createdBy, string updatedBy);
        void InsertDefaultExeReportByProcess(string locationCode, string brandCode, string unitCode, int? year, int? week, DateTime productionDate, string createdBy, string updatedBy);
        void switchBrandExeReportByProcess(string locationCode, string brandGroupCode, DateTime productionDate);
        void recalculateStockExeReportByProcess(string locationCode, string brandCode, DateTime productionDate);
        IEnumerable<GetExeReportByGroupAnnualy_Result> GetExeReportByGroupsAnnualy(string locationCode, string unitCode, int? shift, string process, string brand, int? year);
        IEnumerable<GetExeReportByGroupDaily_Result> GetExeReportByGroupsDaily(string locationCode, string unitCode, int? shift, string process, string brand, DateTime? dateFrom, DateTime? dateTo);
        IEnumerable<ExeReportByProcessFunc_Result> GetExeReportByProcessFunc(DateTime startDate, DateTime endDate, string locationCode, string unitCode, int shift, string brandCode);
        IEnumerable<ExeReportByProcessFuncParent_Result> GetExeReportByProcessFuncParent(DateTime startDate, DateTime endDate, string locationCode, string unitCode, int shift, string brandCode);

        IEnumerable<GetWagesProductionCardApprovalView_Result> GetWagesProductionCardApprovalView(string locationCode, string unitCode, string revisionType);
        
        #region Wages Absent Report
        IEnumerable<GetProcessFromProdCard_Result> GetProcessFromProdCard(DateTime StarDate, DateTime EndDate, string LocationCode, string UnitCode);
        IEnumerable<GetWagesReportAbsentDialy_Result> GetWagesAbsentReportDialy(DateTime ProductionDate, string LocationCode, string UnitCode, string ProcessGroup);
        IEnumerable<WAGES_ABSENT_REPORT_BYGROUP_Result> GetWagesAbsentReportMore(DateTime StarDate, DateTime EndDate, string LocationCode, string UnitCode, string ProcessGroup);
        IEnumerable<WAGES_ABSENT_REPORT_BYEMPLOYEE_Result> GetWagesAbsentReportDetailDialy(DateTime startDate, DateTime endDate, string LocationCode, string UnitCode, string ProcessGroup, string GroupCode);
        IEnumerable<WAGES_ABSENT_REPORT_BYEMPLOYEE_DETAIL_Result> GetWagesAbsentReportDetailDialy(DateTime startDate, DateTime endDate, string employeeID);
        #endregion

        #region EMS Source Data
        IEnumerable<GetEMSSourceData_Result> GetReportEMSSourceDataDaily(string locationCode, string brandCode, DateTime dateFrom, DateTime dateTo);

        IEnumerable<string> GetEmsSourceDataBrandCode(string locationCode, DateTime? dateFrom, DateTime? dateTo);

        #endregion

        #region Production Planning Individual Capacity
        IEnumerable<GetPlanPlantIndividualCapacityByReference_Result> GetPlanPlantIndividualCapacityByReference(string locationCode, string unitCode, string brandGroupCode, string processGroup, string groupCode, int workhour, DateTime? startDate, DateTime? endDate);
        #endregion
        
        #region Exe Report Daily Production Achievement
        IEnumerable<GetExeReportDailyProductionAchievement_Result> GetExeReportDailyProductionAchievement(string location, int week, int year);
        #endregion

        #region Report Position Available
        IEnumerable<AvailablePositionNumberView_Result> GetReportAvailablePositionNumberView1(string location, string groupCode, string unitCode);
        #endregion

        #region Report - Production and Stock Report by Process
        IEnumerable<GetReportProdStockProcessView_Result> GetReportProdStockProcessView(string locationCode, string unitCode, DateTime? dateFrom, DateTime? dateTo);
        IEnumerable<GetReportProdStockProcessAllUnitView_Result> GetReportProdStockProcessAllUnitView(string locationCode, DateTime? dateFrom, DateTime? dateTo);
        #endregion

        IEnumerable<GetMaintenanceEquipmentStockView_Result> GetMaintenanceEquipmentStockView(string locationCode, string unitCode, DateTime inventoryDate);

        IEnumerable<GetMaintenanceEquipmentStockFunction_Result> GetMaintenanceEquipmentStockFunction(string locationCode, string unitCode, DateTime inventoryDate);
        decimal? GetWorkHour(string locationCode, string unit, string group, string brand, int? shift, string process, DateTime? prodDate, int? year, int? week, int? dayOfWeek);
        int? GetAbsensi(string productionEntryCode, string absentCode);
        void RunSSISUpdateReportByGroupWeekly();
        void RunSSISUpdateReportByGroupMonthly();

        void InsertWorkerAssignment_SP(ExePlantWorkerAssignment workerAssignment);

        void DeleteWorkerAssignment_SP(ExePlantWorkerAssignment workerAssignment);

        void SavePlantProductionEntry_SP(ExePlantProductionEntry productionEntry, string saveType);

        void SaveDefaultTargetActualEntry_SP(string productionEntryCode, string saveType);
        Task CopyDeltaView();

        IEnumerable<GetDeltaViewFunction_Result> getDeltaView(string locationCode, DateTime inventoryDate);
        void GenerateProductionCard_SP(GetProductionCardInput input);

        IEnumerable<MaintenanceExecutionInventoryFunction_Result> MaintenanceExecutionInventoryFunction(
            DateTime date, string locationCode, string itemType, string userAD);

        void MaintenanceExecutionInventoryProcedure(DateTime date, string locationCode, string QParam, string userAD);

        IEnumerable<GetProductionCardApprovalList_Result> GetProductionCardApprovalList(string CurrentUser, DateTime StartDate, string TransactionStatus, int IDResponsibility);

        IEnumerable<MntcInventoryAllFunction_Result> MntcInventoryAllFunction(DateTime date, string locationCode,
            string itemStatus, string itemCode, string unitCode, string userAD);

        void MntcInventoryAllProcedure(DateTime date, string locationCode, string QParam, string userAD);

        IEnumerable<GenerateP1TemplateGL_Result> GenerateP1TemplateGL(DateTime ClosingDate, int KPSWeek, int KPSYear, string LocationCode);
        IEnumerable<GenerateP1TemplateAP_Result> GenerateP1TemplateAP(string LocationCode, int KPSWeek, int KPSYear);


        IEnumerable<MntcEquipmentStockFunction_Result> MntcEquipmentStockFunction(DateTime date, string locationCode,
            string unitCode, string userAD);

        void MntcEquipmentStockProcedure(string locationCode, string unitCode, DateTime? inventoryDate, string qParam,
            string userAD);

        IEnumerable<GetReportByStatus_Result> getReportByStatusFunc(string locationCode, string unitCode, int shift, string brandGroupCode, string brandCode, DateTime from, DateTime to);

        void GenerateByProcessGenerator(string locationCode, string brand, string process, int? year, int? week, string createdBy, string updatedBy, DateTime productionDate, string unitCode);

       Task UploadTPOEntryCopyTemp(string listProdEntryCode, string userName);

       void CleanTPOEntryAndVerTemp(string listProdEntryCode);

       void CleanTPOWorkHourTemp(string locationCode, string brandCode, string statusEmp, string processGroup, DateTime productionDate);

       void CopyTPOActualWorkHourTemp(string locationCode, string brandCode, string statusEmp, string processGroup, DateTime productionDate);

       Task SubmitTPOEntryVerificationSP(string locationCode, string brandCode, int year, int week, DateTime prodDate, string process, string user, int role);
    }
}
