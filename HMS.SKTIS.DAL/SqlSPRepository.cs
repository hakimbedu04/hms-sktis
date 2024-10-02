using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.Contracts;

namespace HMS.SKTIS.DAL
{
    public class SqlSPRepository : ISqlSPRepository
    {
        private SKTISEntities _context;

        public SqlSPRepository(SKTISEntities context)
        {
            _context = context;
        }

        public IEnumerable<GetLocations_Result> GetLocations(string locationCode, int level = -1)
        {
            return _context.GetLocations(locationCode, level);
        }

        public IEnumerable<GetMstGenLocationsByParentCode_Result> GetMstGenLocationsByParentCode(GetMstGenLocationsByParentCodeInput input)
        {
            return _context.GetMstGenLocationsByParentCode(input.ParentLocationCode, input.SortExpression, input.SortOrder);
        }

        public IEnumerable<GetLastChildLocation_Result> GetLastChildLocationByLocationCode(string locationCode)
        {
            return _context.GetLastChildLocation(locationCode);
        }

        public IEnumerable<GetUserAndEmailPlanTPUSubmit_Result> GetUserAndEmailPlantTpuSubmit(GetPlanTPUsInput input)
        {
            var functionName = HMS.SKTIS.Core.Enums.PageName.TargetProductionUnit.ToString();
            var buttonName = HMS.SKTIS.Core.Enums.ButtonName.Submit.ToString();
            return _context.GetUserAndEmailPlanTPUSubmit(functionName, buttonName, input.LocationCode, input.BrandCode, input.Shift, input.KPSYear, input.KPSWeek);
        }

        public IEnumerable<GetUserAndEmail_Result> GetUserAndEmail(GetUserAndEmailInput input)
        {
            return _context.GetUserAndEmail(input.FunctionName, input.ButtonName, input.LocationCode, input.UnitCode, input.BrandCode, input.Shift, input.KpsYear, input.KpsWeek, input.IDFlow);
        }

        public void InsertEmailPlantTpu(MailInput input)
        {
            _context.InsertMail(input.FromName, input.FromEmailAddress, input.ToName, input.ToEmailAddress, input.Subject, input.BodyEmail);
        }

        public void InsertEmail(MailInput input)
        {
            _context.InsertMail(input.FromName, input.FromEmailAddress, input.ToName, input.ToEmailAddress, input.Subject, input.BodyEmail);
        }

        public IEnumerable<RoleButtonWagesApprovalDetail_Result> RoleButtonWagesApprovalDetail(ButtonStateInput input)
        {
            return _context.RoleButtonWagesApprovalDetail(input.LocationCode, input.UnitCode, input.Shift, input.Date,input.BrandGroupCode, input.BrandCode, input.RoleId, input.revisiontype, input.currentDate);
        }

        public IEnumerable<GetProductionCardApprovalList_Result> GetProductionCardApprovalList(string CurrentUser, DateTime StartDate, string TransactionStatus, int IDResponsibility)
        {
            return _context.GetProductionCardApprovalList(CurrentUser, StartDate, TransactionStatus, IDResponsibility).OrderBy(c => new { c.LocationCode, c.UnitCode, c.BrandCode });
        }

        public IEnumerable<GetLocationsByLevel_Result> GetLocationsByLevel(string sourceLocationCode, int level = 0)
        {
            return _context.GetLocationsByLevel(sourceLocationCode, level);
        }

        public IEnumerable<GetReportSummaryDailyProductionTargets_Result> GetReportSummaryDailyProductionTargets(int year, int week, int comma, string location)
        {
            return _context.GetReportSummaryDailyProductionTargets(year, week, comma, location);
        }

        public async Task GeneratePlantTPK(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode, int shift)
        {
            // Do asynchronous work.
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.RunSSISPlantTPK(userName, kpsYear, kpsWeek, locationCode, brandCode, shift));
            }
        }

        public async Task GenerateWPP(string userName)
        {
            // Do asynchronous work.
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.RunSSISWPP(userName));
            }
        }

        public IEnumerable<GetReportSummaryProcessTargets_Result> GetReportSummaryProcessTargets(string locationCode, int? year, int? week, DateTime? dateFrom, DateTime? dateTo, int comma, String FilterType)
        {
            return _context.GetReportSummaryProcessTargets(locationCode, year, week, dateFrom, dateTo, comma, FilterType);
        }

        public IEnumerable<GetTPOReportsProduction_Result> GetTpoReportsProduction(HMS.SKTIS.BusinessObjects.Inputs.TPOFee.GetTPOReportsProductionInput input)
        {
            return _context.GetTPOReportsProduction(input.LocationCode, input.YearFrom, input.YearTo, input.WeekFrom, input.WeekTo, input.Month, input.Year, input.DateFrom, input.DateTo, input.FilterType);
        }

        public IEnumerable<GetEquipmentRequirementReport_Result> GetMaintenanceEquipmentRequirementReport(string locationCode, string brandGroupCode, float? userPackage, DateTime date)
        {
            return _context.GetEquipmentRequirementReport(locationCode, brandGroupCode, userPackage, date);
        }

        public IEnumerable<GetWorkerBrandAssignmentPlanningPlantTPK_Result> GetWorkerBrandAssignmentPlanningPlantTpk(GetPlanPlantAllocation filter)
        {
            return _context.GetWorkerBrandAssignmentPlanningPlantTPK(filter.GroupCode, filter.ProcessSettingsCode, filter.Year, filter.Week, filter.UnitCode, filter.LocationCode, filter.BrandCode, filter.Shift, filter.PkPlantStartProdDate);
        }

        public IEnumerable<GetEquipmenrRequiremenrReport2_Result> GetMaintenanceEquipmentRequirementReport2(string locationCode, string brandGroupCodeFrom, string brandGroupCodeTo, DateTime date)
        {
            return _context.GetEquipmenrRequiremenrReport2(locationCode, brandGroupCodeFrom, brandGroupCodeTo, date);
        }
        public IEnumerable<GenerateP1TemplateGL_Result> GenerateP1TemplateGL(DateTime ClosingDate, int KPSWeek, int KPSYear, string LocationCode)
        {
            return _context.GenerateP1TemplateGL(ClosingDate, KPSWeek, KPSYear, LocationCode);
        }

        public IEnumerable<GenerateP1TemplateAP_Result> GenerateP1TemplateAP(string LocationCode, int KPSWeek, int KPSYear)
        {
            return _context.GenerateP1TemplateAP(LocationCode, KPSWeek, KPSYear);
        }
        public IEnumerable<GetEquipmentRequirementItem_Result> GetEquipmentRequirementSummary(string locationCode, string brandGroupCode, DateTime date)
        {
            return _context.GetEquipmentRequirementItem(locationCode, brandGroupCode, date);
        }
        public IQueryable<int?> GetRealStock(string locationCode, string itemCode, DateTime date)
        {
            return _context.GetRealStock(locationCode, itemCode, date);
        }

        public async Task RunSSISProductionEntryPlant(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode, string unitCode)
        {
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.RunSSISProductionEntryPlant(userName, kpsYear, kpsWeek, locationCode, brandCode, unitCode));
            }
        }

        public async Task RunSSISProductionEntryTPO(string userName, int kpsYear, int kpsWeek, string locationCode, string brandCode)
        {
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.RunSSISProductionEntryTPO(userName, kpsYear, kpsWeek, locationCode, brandCode));
            }
        }

        public int ExeTransactionLog(string separator, string page, int year, int week, string code_1, string code_2, string code_3, string code_4, string code_5, string code_6, string code_7, string code_8, string code_9, DateTime TransactionDate, string ActionButton, DateTime ActionTime, string UserName, string TransactionCode, string Message, int? IDRole)
        {
            return _context.TransactionLog(separator, page, year, week, code_1, code_2, code_3, code_4, code_5, code_6, code_7, code_8, code_9, TransactionDate, ActionButton, ActionTime, UserName, TransactionCode, Message, IDRole);
        }

        public async Task ExeTransactionLogTask(string separator, string page, int year, int week, string code_1, string code_2, string code_3, string code_4, string code_5, string code_6, string code_7, string code_8, string code_9, DateTime TransactionDate, string ActionButton, DateTime ActionTime, string UserName, string TransactionCode, string Message, int? IDRole)
        {
            using (var dbContext = new SKTISEntities())
            {
                await
                    Task.Run(() => dbContext.TransactionLog(separator, page, year, week, code_1, code_2, code_3, code_4, code_5,code_6, code_7, code_8, code_9, TransactionDate, ActionButton, ActionTime, UserName,TransactionCode, Message, IDRole));
            }
        }

        #region Production Execution

        #region TPO

        #region Production Entry Verification
        public int TPOProductionEntryVerificationGenerateReport(string LocationCode, string BrandCode, int? KPSYear, int? KPSWeek, DateTime? ProductionDate, string userName)
        {
            return _context.TPOProductionEntryVerificationGenerateReport(LocationCode, BrandCode, KPSYear, KPSWeek, ProductionDate, userName);
        }

        public async Task TpoProductionEntryVerificationGenerateReportTask(string LocationCode, string BrandCode, int? KPSYear, int? KPSWeek, DateTime? ProductionDate, string userName)
        {
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.TPOProductionEntryVerificationGenerateReport(LocationCode, BrandCode, KPSYear, KPSWeek, ProductionDate, userName));
            }
        }

        public async Task SubmitTPOEntryVerificationSP(string locationCode, string brandCode, int year, int week, DateTime prodDate, string process, string user, int role)
        {
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.SUBMIT_TPO_ENTRY_VERIFICATION(locationCode, brandCode, year, week, prodDate, process, user, role));
            }
        }

        public async Task GenerateAllAsyncSequenceTask(List<string> input, string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy, string updatedBy, string unitCode, bool generateTpo)
        {
            // generate report by group
             Task.Run(() => InsertTPOExeReportByGroupsTask(locationCode, brand, year, week, date, createdBy)).Wait();

            //generate report by process
            foreach (var processCode in input)
            {
                var code = processCode;
                
                    Task.Run(
                        () =>
                            GenerateByProcessTask(locationCode, brand, code, year, week, createdBy, updatedBy, date, unitCode)).Wait();
            }

            //generate tpo fee
            if (!generateTpo) return;
             Task.Run(() => GenerateTpoFeeTask(locationCode, brand, year, week, date, createdBy, generateTpo)).Wait();
        }

        public  async Task InsertTPOExeReportByGroupsTask(string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy)
        {
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.InsertTPOExeReportByGroups(locationCode, brand, year, week, date, createdBy));
            }
        }

        public async Task GenerateByProcessTask(string locationCode, string brand, string processCode, int? year, int? week, string createdBy, string updatedBy, DateTime? date, string unitCode)
        {
            using (var dbContext = new SKTISEntities())
            {
                    await Task.Run(() => dbContext.BaseExeReportByProcess(locationCode, brand, processCode, year.ToString(), week.ToString(), createdBy, updatedBy, date, unitCode));
            }
        }

        public  async Task GenerateTpoFeeTask(string locationCode, string brandCode, int? year, int? week, DateTime? productionDate, string userName, bool generateTpo)
        {
            
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.TPOProductionEntryVerificationGenerateReport(locationCode, brandCode, year, week, productionDate, userName));
            }
        }

        public void GenerateByProcess(string locationCode, string brand, List<string> input , int? year, int? week, string createdBy,string updatedBy, DateTime? date, string unitCode)
        {
            using (var dbContext = new SKTISEntities())
            {
                foreach (var process in input)
                {
                    dbContext.BaseExeReportByProcess(locationCode, brand,  process,  year.ToString(),  week.ToString(), createdBy, updatedBy,  date, unitCode);
                }
            }
        }

        public void GenerateTpoFee(string locationCode, string brandCode, int? year, int? week, DateTime? productionDate, string userName, bool generateTpo)
        {
            if (!generateTpo) return;
            using (var dbContext = new SKTISEntities())
            {
                dbContext.TPOProductionEntryVerificationGenerateReport(locationCode, brandCode, year, week, productionDate, userName);
            }
        }

        

        

        public int TPOProductionEntryVerificationCancelReport(string LocationCode, string BrandCode, int? KPSYear, int? KPSWeek, DateTime? ProductionDate)
        {
            return _context.TPOProductionEntryVerificationCancelReport(LocationCode, BrandCode, KPSYear, KPSWeek, ProductionDate);
        }
        #endregion

        #endregion

        #endregion

        #region Utilities

        #region Transaction History and Flow

        public IQueryable<Nullable<int>> RoleButtonChecker(string tPOFeeCode, int iDRole, string page, string button)
        {
            return _context.RoleButtonChecker(tPOFeeCode, iDRole, page, button);
        }

        public IEnumerable<GetInventoryView_Result> GetInventoryView(string date, string locationCode, string itemType)
        {
            return _context.GetInventoryView(date, locationCode, itemType);
        }

        public IEnumerable<MaintenanceExecutionInventoryFunction_Result> MaintenanceExecutionInventoryFunction(DateTime date, string locationCode, string itemType, string userAD)
        {
            return _context.MaintenanceExecutionInventoryFunction(date, locationCode, itemType, userAD);
        }

        public IEnumerable<MntcInventoryAllFunction_Result> MntcInventoryAllFunction(DateTime date, string locationCode, string itemStatus, string itemCode, string unitCode, string userAD)
        {
            return _context.MntcInventoryAllFunction(date, locationCode, itemStatus, itemCode, unitCode, userAD);
        }

        public void MaintenanceExecutionInventoryProcedure(DateTime date, string locationCode, string QParam, string userAD)
        {
            _context.MaintenanceExecutionInventoryProcedure(date, locationCode, QParam, userAD);
        }

        public IEnumerable<MntcEquipmentStockFunction_Result> MntcEquipmentStockFunction(DateTime date, string locationCode, string unitCode, string userAD)
        {
            return _context.MntcEquipmentStockFunction(date, locationCode, unitCode, userAD);
        }

        public void MntcEquipmentStockProcedure(string locationCode, string unitCode, DateTime? inventoryDate, string qParam, string userAD)
        {
            _context.MntcEquipmentStockProcedure(locationCode, unitCode, inventoryDate, qParam, userAD);
        }

        public void MntcInventoryAllProcedure(DateTime date, string locationCode, string QParam, string userAD)
        {
            _context.MntcInventoryAllProcedure(date, locationCode, QParam, userAD);
        }

        public IEnumerable<GetTransactionHistory_Result> GetTransactionHistory(string transactionCode, string pageSource)
        {
            return _context.GetTransactionHistory(transactionCode, pageSource);
        }

        public IEnumerable<GetTransactionHistoryWagesProdcardCorrection_Result> GetTransactionHistoryWagesProdcardCorrection(string transactionCode)
        {
            return _context.GetTransactionHistoryWagesProdcardCorrection(transactionCode);

        }
        public IEnumerable<GetTransactionFlow_Result> GetTransactionFlow(string functionName)
        {
            return _context.GetTransactionFlow(functionName);
        }

        #endregion

        public IEnumerable<GetUserAdByRoleLocation_Result> GetUserAdByRoleLocation(string roleCode, string location)
        {
            return _context.GetUserAdByRoleLocation(roleCode, location);
        }

        #endregion

        public void SubmitTpoTpk(string locationCode, string brandCode, int? kpsYear, int? kpsWeek, string userName)
        {
            _context.SP_SubmitTpoTpk(locationCode, brandCode, kpsYear, kpsWeek, userName);
        }

        public void InsertPlantExeReportByGroups(string locationCode, string unit, string brand, int? shift, int? year, int? week, DateTime? date, string groupCode, string createdBy)
        {
            _context.InsertPlantExeReportByGroups(locationCode, unit, brand, shift, year, week, date, groupCode, createdBy);
        }
        public void InsertTPOExeReportByGroups(string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy)
        {
            _context.InsertTPOExeReportByGroups(locationCode, brand, year, week, date, createdBy);
        }

        
        #region Report By Process
        public void InsertPlantExeReportByProcess(string locationCode, string brand, string process, int? year, int? week, string createdBy, string updatedBy, DateTime productionDate, string unitCode)
        {
            _context.BaseExeReportByProcess(locationCode, brand, process, year.ToString(), week.ToString(), createdBy,
                updatedBy, productionDate, unitCode);
        }

        public async Task InsertPlantExeReportByProcessTask(string locationCode, string brand, string process, int? year, int? week, string createdBy, string updatedBy, DateTime productionDate, string unitCode)
        {
            using (var dbContext = new SKTISEntities())
            {
                //int milliseconds = 5000;
                //await Task.Delay(milliseconds);
                await Task.Run(() => dbContext.BaseExeReportByProcess(locationCode, brand, process, year.ToString(), week.ToString(), createdBy, updatedBy, productionDate, unitCode));
            }
        }

        public void GenerateByProcessGenerator(string locationCode, string brand, string process, int? year, int? week, string createdBy, string updatedBy, DateTime productionDate, string unitCode) 
        {
            _context.BaseExeReportByProcessGenerator(locationCode, brand, process, year.ToString(), week.ToString(), createdBy,
               updatedBy, productionDate, unitCode);
        }

        public void DefaultExeReportByProcess(string locationCode, string brandCode,string unitCode, int? year, int? week, DateTime productionDate, string createdBy, string updatedBy)
        {
            _context.DefaultExeReportByProcess(locationCode,brandCode,unitCode,year,week,productionDate,createdBy,updatedBy);
        }

        public void InsertDefaultExeReportByProcess(string locationCode, string brandCode, string unitCode, int? year, int? week, DateTime productionDate, string createdBy, string updatedBy)
        {
            _context.InsertDefaultExeReportByProcess(locationCode, brandCode, unitCode, year, week, productionDate,
                createdBy, updatedBy);
        }

        public void switchBrandExeReportByProcess(string locationCode, string brandGroupCode, DateTime productionDate)
        {
            _context.SwitchingBrandExeReportByProcess(locationCode, brandGroupCode, productionDate);
        }
        public void recalculateStockExeReportByProcess(string locationCode, string brandCode, DateTime productionDate)
        {
            _context.RecalculateStockExeReportByProcess(productionDate, locationCode, brandCode);
        }
        #endregion
        public IEnumerable<GetExeReportByGroupDaily_Result> GetExeReportByGroupsDaily(string locationCode, string unitCode, int? shift, string process, string brand, DateTime? dateFrom, DateTime? dateTo)
        {
            return _context.GetExeReportByGroupDaily(locationCode, unitCode, shift, process, brand, dateFrom, dateTo);
        }

        public IEnumerable<ExeReportByProcessFunc_Result> GetExeReportByProcessFunc(DateTime startDate, DateTime endDate, string locationCode, string unitCode, int shift, string brandCode)
        {
            return _context.ExeReportByProcessFunc(startDate,endDate,locationCode,unitCode,shift,brandCode);
        }

        public IEnumerable<GetWagesProductionCardApprovalView_Result> GetWagesProductionCardApprovalView(string locationCode, string unitCode, string revisionType)
        {
            return _context.GetWagesProductionCardApprovalView(locationCode, unitCode, revisionType);
        }

        public IEnumerable<ExeReportByProcessFuncParent_Result> GetExeReportByProcessFuncParent(DateTime startDate, DateTime endDate, string locationCode, string unitCode, int shift, string brandCode) {
            return _context.ExeReportByProcessFuncParent(startDate, endDate, locationCode, unitCode, shift, brandCode);
        }

        public IEnumerable<GetExeReportByGroupAnnualy_Result> GetExeReportByGroupsAnnualy(string locationCode, string unitCode, int? shift, string process, string brand, int? year)
        {
            return _context.GetExeReportByGroupAnnualy(locationCode, unitCode, shift, process, brand, year);
        }

        #region Wages Report Absent
        public IEnumerable<GetProcessFromProdCard_Result> GetProcessFromProdCard(DateTime StarDate, DateTime EndDate, string LocationCode, string UnitCode)
        {
            return _context.GetProcessFromProdCard(StarDate, EndDate, LocationCode, UnitCode);
        }
        public IEnumerable<GetWagesReportAbsentDialy_Result> GetWagesAbsentReportDialy(DateTime ProductionDate, string LocationCode, string UnitCode, string ProcessGroup)
        {
            return _context.GetWagesReportAbsentDialy(ProductionDate, LocationCode, UnitCode, ProcessGroup);
        }

        public IEnumerable<WAGES_ABSENT_REPORT_BYGROUP_Result> GetWagesAbsentReportMore(DateTime StarDate, DateTime EndDate, string LocationCode, string UnitCode, string ProcessGroup)
        {
            return _context.WAGES_ABSENT_REPORT_BYGROUP(StarDate, EndDate, LocationCode, UnitCode, ProcessGroup);
        }

        public IEnumerable<WAGES_ABSENT_REPORT_BYEMPLOYEE_Result> GetWagesAbsentReportDetailDialy(DateTime startDate, DateTime endDate, string LocationCode, string UnitCode, string ProcessGroup, string GroupCode)
        {
            return _context.WAGES_ABSENT_REPORT_BYEMPLOYEE(startDate, endDate, LocationCode, UnitCode, GroupCode, ProcessGroup);
        }

        public IEnumerable<WAGES_ABSENT_REPORT_BYEMPLOYEE_DETAIL_Result> GetWagesAbsentReportDetailDialy(DateTime startDate, DateTime endDate, string employeeID) 
        {
            return _context.WAGES_ABSENT_REPORT_BYEMPLOYEE_DETAIL(startDate, endDate, employeeID);
        }
        #endregion Wages Report Absent

        #region EMS Source Data
        public IEnumerable<GetEMSSourceData_Result> GetReportEMSSourceDataDaily(string locationCode, string brandCode, DateTime dateFrom, DateTime dateTo)
        {
            return _context.GetEMSSourceData(locationCode, brandCode, dateFrom, dateTo);
        }

        public IEnumerable<string> GetEmsSourceDataBrandCode(string locationCode, DateTime? dateFrom, DateTime? dateTo)
        {
            return _context.EMSSourceDataBrandCodeView(locationCode, dateFrom, dateTo);
        }
        #endregion

        #region Exe Report Daily Production Achievement
        public IEnumerable<GetExeReportDailyProductionAchievement_Result> GetExeReportDailyProductionAchievement(string location, int week, int year)
        {
            return _context.GetExeReportDailyProductionAchievement(location, week, year);
        }
        #endregion

        #region Production Planning Individual Capacity
        public IEnumerable<GetPlanPlantIndividualCapacityByReference_Result> GetPlanPlantIndividualCapacityByReference(string locationCode, string unitCode, string brandGroupCode, string processGroup, string groupCode, int workhour, DateTime? startDate, DateTime? endDate)
        {
            return _context.GetPlanPlantIndividualCapacityByReference(locationCode, unitCode, brandGroupCode, processGroup, groupCode, workhour, startDate, endDate);
        }
        #endregion

        #region For Report Available Position Number
        public IEnumerable<AvailablePositionNumberView_Result> GetReportAvailablePositionNumberView1(string location, string groupCode, string unitCode)
        {
            return _context.AvailablePositionNumberView(location, groupCode,unitCode);
        }
        #endregion

        #region Report - Production and Stock Report by Process
        public IEnumerable<GetReportProdStockProcessView_Result> GetReportProdStockProcessView(string locationCode, string unitCode, DateTime? dateFrom, DateTime? dateTo)
        {
            return _context.GetReportProdStockProcessView(locationCode, unitCode, dateFrom, dateTo);
        }

        public IEnumerable<GetReportProdStockProcessAllUnitView_Result> GetReportProdStockProcessAllUnitView(string locationCode, DateTime? dateFrom, DateTime? dateTo)
        {
            return _context.GetReportProdStockProcessAllUnitView(locationCode, dateFrom, dateTo);
        }
        #endregion

        public IEnumerable<GetMaintenanceEquipmentStockView_Result> GetMaintenanceEquipmentStockView(string locationCode, string unitCode, DateTime inventoryDate)
        {
            return _context.GetMaintenanceEquipmentStockView(locationCode, unitCode, inventoryDate.Date);
        }

        public IEnumerable<GetMaintenanceEquipmentStockFunction_Result> GetMaintenanceEquipmentStockFunction(string locationCode, string unitCode, DateTime inventoryDate)
        {
            return _context.GetMaintenanceEquipmentStockFunction(locationCode, unitCode, inventoryDate.Date);
        }

        public decimal? GetWorkHour(string locationCode, string unit, string group, string brand, int? shift, string process, DateTime? prodDate, int? year, int? week, int? dayOfWeek)
        {
            return _context.GetWorkHour(locationCode, unit, group, brand, shift, process, prodDate, year, week, dayOfWeek).FirstOrDefault();
        }
        public int? GetAbsensi(string productionEntryCode, string absentCode)
        {
            return _context.GetAbsensi(productionEntryCode, absentCode).FirstOrDefault();
        }

        public void RunSSISUpdateReportByGroupWeekly()
        {
            _context.RunSSISProductionReportByGroupWeekly();
        }
        public void RunSSISUpdateReportByGroupMonthly() 
        {
            _context.RunSSISProductionReportByGroupMonthly();
        }

        public void InsertWorkerAssignment_SP(ExePlantWorkerAssignment workerAssignment)
        { 
            _context.INSERT_WORKER_ASSIGNMENT
            (
                workerAssignment.SourceLocationCode,
                workerAssignment.SourceUnitCode,
                workerAssignment.SourceShift,
                workerAssignment.SourceProcessGroup,
                workerAssignment.SourceGroupCode,
                workerAssignment.SourceBrandCode,
                workerAssignment.DestinationLocationCode,
                workerAssignment.DestinationUnitCode,
                workerAssignment.DestinationShift,
                workerAssignment.DestinationProcessGroup,
                workerAssignment.DestinationGroupCode,
                workerAssignment.DestinationGroupCodeDummy,
                workerAssignment.DestinationBrandCode,
                workerAssignment.EmployeeID,
                workerAssignment.EmployeeNumber,
                workerAssignment.StartDate,
                workerAssignment.EndDate,
                workerAssignment.CreatedBy,
                workerAssignment.UpdatedBy
            );
        }

        public void DeleteWorkerAssignment_SP(ExePlantWorkerAssignment workerAssignment)
        { 
            _context.DELETE_WORKER_ASSIGNMENT
            (
                workerAssignment.SourceLocationCode,
                workerAssignment.SourceUnitCode,
                workerAssignment.SourceShift,
                workerAssignment.SourceProcessGroup,
                workerAssignment.SourceGroupCode,
                workerAssignment.SourceBrandCode,
                workerAssignment.DestinationLocationCode,
                workerAssignment.DestinationUnitCode,
                workerAssignment.DestinationShift,
                workerAssignment.DestinationProcessGroup,
                workerAssignment.DestinationGroupCode,
                workerAssignment.DestinationGroupCodeDummy,
                workerAssignment.DestinationBrandCode,
                workerAssignment.EmployeeID,
                workerAssignment.EmployeeNumber,
                workerAssignment.StartDate,
                workerAssignment.EndDate,
                workerAssignment.CreatedBy,
                workerAssignment.UpdatedBy
            );
        }

        public void SavePlantProductionEntry_SP(ExePlantProductionEntry productionEntry, string saveType)
        {
            _context.EDIT_PLANT_PRODUCTION_ENTRY
            (
                productionEntry.ProductionEntryCode,
                productionEntry.EmployeeID,
                productionEntry.EmployeeNumber,
                productionEntry.StatusEmp,
                productionEntry.StatusIdentifier,
                productionEntry.StartDateAbsent,
                productionEntry.AbsentType,
                productionEntry.ProdCapacity,
                productionEntry.ProdTarget,
                productionEntry.ProdActual,
                productionEntry.AbsentRemark,
                productionEntry.AbsentCodeEblek,
                productionEntry.AbsentCodePayroll,
                productionEntry.UpdatedDate,
                productionEntry.UpdatedBy,
                productionEntry.IsFromAbsenteeism,
                saveType,
                productionEntry.ProductionEntryCode.Split('/')[1],
                productionEntry.ProductionEntryCode.Split('/')[3],
                productionEntry.ProductionEntryCode.Split('/')[4],
                Convert.ToInt32(productionEntry.ProductionEntryCode.Split('/')[2])
            );
        }

        public void SaveDefaultTargetActualEntry_SP(string productionEntryCode, string saveType)
        {
            _context.INSERT_DEFAULT_TARGET_ACTUAL_ENTRY(productionEntryCode, saveType);
        }

        public async Task CopyDeltaView()
        {
            // Do asynchronous work.
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.CopyDeltaView());
            }
        }

        public IEnumerable<GetDeltaViewFunction_Result> getDeltaView(string locationCode, DateTime inventoryDate)
        {
            return _context.GetDeltaViewFunction(locationCode, inventoryDate.Date);
        }

        public void GenerateProductionCard_SP(GetProductionCardInput input) 
        {
           _context.GENERATE_PRODUCTION_CARD
           (
               input.LocationCode, input.Unit, input.Shift, input.Brand,
               input.KPSYear, input.KPSWeek, input.Date, input.Group, input.UserName
           );
        }

        public IEnumerable<GetReportByStatus_Result> getReportByStatusFunc(string locationCode, string unitCode, int shift, string brandGroupCode, string brandCode, DateTime from, DateTime to) {
            return _context.GetReportByStatus(locationCode, unitCode, shift, brandGroupCode, brandCode, from, to);
        }

        public async Task UploadTPOEntryCopyTemp(string listProdEntryCode, string userName)
        {
            // Do asynchronous work.
            using (var dbContext = new SKTISEntities())
            {
                await Task.Run(() => dbContext.UPLOAD_TPODAILY_COPYFROMTEMP(listProdEntryCode, userName));
            }
        }

        public void CleanTPOEntryAndVerTemp(string listProdEntryCode) 
        {
            _context.CLEAN_TPO_ENTRY_TEMP(listProdEntryCode);
        }

        public void CleanTPOWorkHourTemp(string locationCode, string brandCode, string statusEmp, string processGroup, DateTime productionDate)
        {
            _context.CLEAN_TPO_WORKHOUR_TEMP(locationCode, brandCode, productionDate, processGroup, statusEmp);
        }

        public void CopyTPOActualWorkHourTemp(string locationCode, string brandCode, string statusEmp, string processGroup, DateTime productionDate)
        {
            _context.UPLOAD_TPODAILY_COPY_ACTUALWORKHOURTEMP(locationCode, brandCode, productionDate, processGroup, statusEmp);
        }
    }
}
