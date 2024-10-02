using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using FastMember;
using System.Data;
using System.Data.SqlClient;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using System.Diagnostics;

namespace HMS.SKTIS.BLL.ExecutionBLL
{
    public partial class ExecutionPlantBLL : IExecutionPlantBLL
    {
        private IUnitOfWork _uow;
        private ISqlSPRepository _sqlSPRepo;
        private IGenericRepository<ExePlantWorkerAbsenteeism> _exePlantWorkerAbsenteesimRepo;
        private IGenericRepository<ExePlantWorkerAbsenteeismView> _exePlantWorkerAbsenteesimViewRepo;
        private IGenericRepository<ExePlantProductionEntry> _exePlantProductionEntryRepo;
        private IGenericRepository<MstPlantAbsentType> _exeMstPlantAbsentTypeRepo;
        private IGenericRepository<MstPlantEmpJobsDataAcv> _mstPlantEmpJobsdataAcvRepo;
        private IGenericRepository<ExePlantProductionEntryVerification> _exePlantProductionEntryVerificationRepo;
        private IGenericRepository<MstGenBrand> _mstGenBrandRepo;
        private IGenericRepository<MstGenProcessSetting> _mstGenProcessSettingRepo;
        private IGenericRepository<PlanPlantTargetProductionKelompok> _planPlantTargetProductionKelompokRepo;
        private IGenericRepository<MstGenWeek> _mstGenWeekRepo;
        private IGenericRepository<MstGenProcess> _mstGenProcess;
        private IMasterDataBLL _masterDataBll;
        private IGenericRepository<ExePlantActualWorkHoursView> _exeExePlantActualWorkHoursViewRepo;
        private IGenericRepository<ExePlantWorkerAssignment> _exePlantWorkerAssignmentRepo;
        private IGenericRepository<ExeMaterialUsage> _exeMaterialUsagesRepo;
        private IGenericRepository<ExeMaterialUsageView> _exeMaterialUsageViewRepo;
        private IGenericRepository<ExeActualWorkHour> _exePlantActualWorkHoursRepo;
        private IGenericRepository<ExeTPOActualWorkHour> _exeTpoActualWorkHourRepo;
        private IGenericRepository<WorkerAssignmentRemoval> _workerAssignmentRemovalRepository;
        private IGenericRepository<MstPlantAbsentType> _mstPlantAbsentType;
        private IGenericRepository<ExePlantProductionEntryVerificationView> _exePlantProductionEntryVerificationViewRepo;
        private IGeneralBLL _generalBll;
        private IGenericRepository<ExePlantWorkerBalancingMulti> _exePlantWorkerBalancingViewRepo;
        private IGenericRepository<ExePlantWorkerBalancingSingle> _exePlantWorkerBalancingSingleViewRepo;
        private IGenericRepository<GetLocationByResponsibilityView> _getLocationByResponsibilityViewRepo;
        private IGenericRepository<PlanPlantIndividualCapacityWorkHour> _planPlantIndividualCapacityRepo;
        private IGenericRepository<PlanTargetProductionUnit> _exePlantPlanTargetProductionUnitRepo;
        private IPlantWagesExecutionBLL _plantWagesBll;
        private IUtilitiesBLL _utilitiesBll;
        private IGenericRepository<ProductionCard> _productionCardRepo;
        private IGenericRepository<ExeProductionEntryMinimumValue> _exeProductionMinimumValue;
        private IGenericRepository<MstClosingPayroll> _mstClosingPayroll;
        private IGenericRepository<MstGenHoliday> _mstGenHoliday;
        private IGenericRepository<MstPlantProductionGroup> _mstPlantProductionGroup;
        private IGenericRepository<MstGenBrandGroup> _mstGenBrandGroup;
        private IGenericRepository<ProcessSettingsAndLocationView> _mstGenProcessSettingLocationViewRepo;
        private IGenericRepository<EMSSourceDataBrandView> _EMSSourceDataBrandViewRepo;
        private IGenericRepository<ExeReportByGroup> _exeReportByGroupRepo;
        private IGenericRepository<ExeReportByProcess> _exeReportByProcessRepo;
        private IGenericRepository<MstADTemp> _mstAdTemp;
        private IGenericRepository<MstPlantEmpJobsDataAll> _mstPlantEmpJobsdataAllRepo;

        public ExecutionPlantBLL(IUnitOfWork uow, IMasterDataBLL masterDataBll, IGeneralBLL generalBll, IPlantWagesExecutionBLL plantWagesBll, IUtilitiesBLL utilitiesBll)
        {
            _uow = uow;
            _sqlSPRepo = _uow.GetSPRepository();
            _exePlantWorkerAbsenteesimRepo = _uow.GetGenericRepository<ExePlantWorkerAbsenteeism>();
            _exePlantWorkerAbsenteesimViewRepo = _uow.GetGenericRepository<ExePlantWorkerAbsenteeismView>();
            _exePlantProductionEntryRepo = _uow.GetGenericRepository<ExePlantProductionEntry>();
            _exeExePlantActualWorkHoursViewRepo = _uow.GetGenericRepository<ExePlantActualWorkHoursView>();
            _exeMstPlantAbsentTypeRepo = _uow.GetGenericRepository<MstPlantAbsentType>();
            _mstPlantEmpJobsdataAcvRepo = _uow.GetGenericRepository<MstPlantEmpJobsDataAcv>();
            _mstGenBrandRepo = _uow.GetGenericRepository<MstGenBrand>();
            _planPlantTargetProductionKelompokRepo = _uow.GetGenericRepository<PlanPlantTargetProductionKelompok>();
            _mstGenProcessSettingRepo = _uow.GetGenericRepository<MstGenProcessSetting>();
            _exePlantProductionEntryVerificationRepo = _uow.GetGenericRepository<ExePlantProductionEntryVerification>();
            _masterDataBll = masterDataBll;
            _exePlantWorkerAssignmentRepo = _uow.GetGenericRepository<ExePlantWorkerAssignment>();
            _exeMaterialUsagesRepo = _uow.GetGenericRepository<ExeMaterialUsage>();
            _exeMaterialUsageViewRepo = _uow.GetGenericRepository<ExeMaterialUsageView>();
            _exePlantActualWorkHoursRepo = _uow.GetGenericRepository<ExeActualWorkHour>();
            _workerAssignmentRemovalRepository = _uow.GetGenericRepository<WorkerAssignmentRemoval>();
            _mstGenWeekRepo = _uow.GetGenericRepository<MstGenWeek>();
            _mstGenProcess = _uow.GetGenericRepository<MstGenProcess>();
            _mstPlantAbsentType = _uow.GetGenericRepository<MstPlantAbsentType>();
            _exePlantProductionEntryVerificationViewRepo =
                _uow.GetGenericRepository<ExePlantProductionEntryVerificationView>();
            _generalBll = generalBll;
            _exePlantWorkerBalancingViewRepo = _uow.GetGenericRepository<ExePlantWorkerBalancingMulti>();
            _exePlantWorkerBalancingSingleViewRepo = _uow.GetGenericRepository<ExePlantWorkerBalancingSingle>();

            _getLocationByResponsibilityViewRepo = _uow.GetGenericRepository<GetLocationByResponsibilityView>();
            _plantWagesBll = plantWagesBll;
            _utilitiesBll = utilitiesBll;
            _planPlantIndividualCapacityRepo = _uow.GetGenericRepository<PlanPlantIndividualCapacityWorkHour>();
            _productionCardRepo = _uow.GetGenericRepository<ProductionCard>();
            _exePlantPlanTargetProductionUnitRepo = _uow.GetGenericRepository<PlanTargetProductionUnit>();
            _exeProductionMinimumValue = _uow.GetGenericRepository<ExeProductionEntryMinimumValue>();
            _mstClosingPayroll = _uow.GetGenericRepository<MstClosingPayroll>();
            _mstGenHoliday = _uow.GetGenericRepository<MstGenHoliday>();
            _mstPlantProductionGroup = _uow.GetGenericRepository<MstPlantProductionGroup>();
            _mstGenBrandGroup = _uow.GetGenericRepository<MstGenBrandGroup>();
            _mstGenProcessSettingLocationViewRepo = _uow.GetGenericRepository<ProcessSettingsAndLocationView>();
            _EMSSourceDataBrandViewRepo = _uow.GetGenericRepository<EMSSourceDataBrandView>();
            _exeReportByGroupRepo = _uow.GetGenericRepository<ExeReportByGroup>();
            _mstAdTemp = _uow.GetGenericRepository<MstADTemp>();
            _exeReportByProcessRepo = _uow.GetGenericRepository<ExeReportByProcess>();
            _mstPlantEmpJobsdataAllRepo = _uow.GetGenericRepository<MstPlantEmpJobsDataAll>();
        }

        #region Worker Absenteeism

        /// <summary>
        /// Get all Worker Absenteeism Data
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<ExePlantWorkerAbsenteeismViewDTO> GetWorkerAbsenteeism(GetExePlantWorkerAbsenteeismInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantWorkerAbsenteeismView>();

            if (string.IsNullOrEmpty(input.LocationCode))
                input.LocationCode = "X";
            queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (string.IsNullOrEmpty(input.UnitCode))
                input.UnitCode = "X";
            queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            if (!string.IsNullOrEmpty(input.EmployeeID))
                queryFilter = queryFilter.And(m => m.EmployeeID == input.EmployeeID);

            if (input.Shift != 0)
                queryFilter = queryFilter.And(m => m.Shift == input.Shift);

            //if (string.IsNullOrEmpty(input.Process))
            //    input.Process = "X";
            //queryFilter = queryFilter.And(m => m.ProcessSettingsCode.ToLower() == input.Process.ToLower());

            if (string.IsNullOrEmpty(input.GroupCode))
                input.GroupCode = "X";
            queryFilter = queryFilter.And(m => m.GroupCode == input.GroupCode);

            if (input.TransactionDate == null)
            {
                //input.TransactionDate = DateTime.ParseExact("0", Constants.DefaultDateOnlyFormat, System.Globalization.CultureInfo.InvariantCulture);
                input.TransactionDate = DateTime.Now;
            }
            queryFilter = queryFilter.And(m => input.TransactionDate >= m.StartDateAbsent);
            queryFilter = queryFilter.And(m => input.TransactionDate <= m.EndDateAbsent);
                

            if (!string.IsNullOrEmpty(input.FilterType))
            {
                if (input.FilterType == "Year")
                {
                    queryFilter = queryFilter.And(m => m.StartDateAbsent.Year == input.Year).And(m => m.EndDateAbsent.Year == input.Year);
                }
                if (input.FilterType == "YearMonth")
                {
                    DateTime start = new DateTime(input.YearMonthFrom, input.MonthFrom, DateTime.DaysInMonth(input.YearMonthFrom, input.MonthFrom));
                    DateTime end = new DateTime(input.YearMonthTo, input.MonthTo, DateTime.DaysInMonth(input.YearMonthTo, input.MonthTo));
                    queryFilter = queryFilter
                        .And(m => m.StartDateAbsent >= start)
                        .And(m => m.EndDateAbsent <= end);
                }
                if (input.FilterType == "YearWeek")
                {
                    var YearWeekStart = _masterDataBll.GetWeekByYearAndWeek(input.YearWeekFrom, input.WeekFrom);
                    var YearWeekEnd = _masterDataBll.GetWeekByYearAndWeek(input.YearWeekTo, input.WeekTo);
                    queryFilter = queryFilter
                        .And(m => m.StartDateAbsent >= YearWeekStart.StartDate)
                        .And(m => m.EndDateAbsent <= YearWeekEnd.EndDate);
                }
                if (input.FilterType == "Date")
                {
                    //queryFilter = queryFilter.And(m => m.StartDateAbsent >= input.DateFrom).And(m => m.EndDateAbsent <= input.DateTo);
                }
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantWorkerAbsenteeismView>();

            var dbResult = _exePlantWorkerAbsenteesimViewRepo.Get(queryFilter, orderByFilter);

            var resultDto = Mapper.Map<List<ExePlantWorkerAbsenteeismViewDTO>>(dbResult);

            // Get list absent type only active in eblek
            var listAbsentFromEblek = _masterDataBll.GetAllAbsentTypesActiveInEblekOnly();

            // Assign absenteeism where came from eblek
            foreach (var item in resultDto)
            {
                if (listAbsentFromEblek.Select(c => c.AbsentType).Distinct().ToList().Contains(item.AbsentType))
                    item.IsFromEblek = true; item.EblekStatus = "LOCKED";

                if (!item.IsFromEblek)
                    item.EblekStatus = CheckEblekStatusOnGetAbsenteeism(item);
                
            }

            return resultDto;
        }

        public List<ExePlantWorkerAbsenteeismViewDTO> GetWorkerAbsenteeismDaily(GetExePlantWorkerAbsenteeismInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantWorkerAbsenteeismView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            queryFilter = queryFilter.And(m => m.Shift == input.Shift);

            if (!string.IsNullOrEmpty(input.Process))
                queryFilter = queryFilter.And(m => m.ProcessSettingsCode.ToLower() == input.Process.ToLower());
            //if (!string.IsNullOrEmpty(input.GroupCode))
            queryFilter = queryFilter.And(m => m.GroupCode.Substring(0, 1) == "5");

            if (input.TransactionDate != null)
            {
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(input.TransactionDate) >= DbFunctions.TruncateTime(m.StartDateAbsent));
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(input.TransactionDate) <= DbFunctions.TruncateTime(m.EndDateAbsent));
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantWorkerAbsenteeismView>();

            var dbResult = _exePlantWorkerAbsenteesimViewRepo.Get(queryFilter, orderByFilter);

            var result = Mapper.Map<List<ExePlantWorkerAbsenteeismViewDTO>>(dbResult);

            var listEmployeeDaily = _mstPlantEmpJobsdataAcvRepo.Get(c => c.Status == ((int)Enums.StatusEmployeeJobsData.Mandor).ToString())
                                    .Select(c => c.EmployeeID).ToList();

            return result.Where(c => listEmployeeDaily.Contains(c.EmployeeID)).ToList();
        }

        public string GetPlantProductionEntryBrand(string employeeID, DateTime productionDate, string shift, string unitCode, string groupCode)
        {
            //combine code
            var combineCode = string.Format("/{0}/{1}/{2}/",
                shift,
                unitCode,
                groupCode);

            //get source and destination production entries
            var productionEntries = _exePlantProductionEntryRepo.Get(m => m.ProductionEntryCode.Contains(combineCode)
                                                                          && m.EmployeeID == employeeID
                                                                          && m.ExePlantProductionEntryVerification.ProductionDate == productionDate).FirstOrDefault();
            if (productionEntries != null)
            {
                return productionEntries.ExePlantProductionEntryVerification.BrandCode;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Insert All Worker Absenteeism Data
        /// </summary>
        /// <param name="workerAbsenteeism"></param>
        /// <returns></returns>
        public ExePlantWorkerAbsenteeismDTO InsertWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeism)
        {
            //validate insert
            ValidateInsertWorkerAbsenteeism(workerAbsenteeism);

            if (workerAbsenteeism.GroupCode == null) workerAbsenteeism.GroupCode = _mstPlantEmpJobsdataAcvRepo.GetByID(workerAbsenteeism.EmployeeID).GroupCode;

            var dbExePlantWorkerAbsenteeism = Mapper.Map<ExePlantWorkerAbsenteeism>(workerAbsenteeism);

            dbExePlantWorkerAbsenteeism.CreatedDate = DateTime.Now;
            dbExePlantWorkerAbsenteeism.UpdatedDate = DateTime.Now;

            if (dbExePlantWorkerAbsenteeism.EndDateAbsent.Date < dbExePlantWorkerAbsenteeism.StartDateAbsent.Date) throw new BLLException(ExceptionCodes.BLLExceptions.EndDateLessThanStartDate);
            
            // Insert Worker Abseteeism
            _exePlantWorkerAbsenteesimRepo.Insert(dbExePlantWorkerAbsenteeism);

            // Get Master Absent Type
            var mstAbsentType = _mstPlantAbsentType.GetByID(dbExePlantWorkerAbsenteeism.AbsentType);

            // Update Production Entry
            var listProductionEntries = _exePlantProductionEntryRepo.Get(c => c.EmployeeID == dbExePlantWorkerAbsenteeism.EmployeeID &&
                                                                              c.ExePlantProductionEntryVerification.LocationCode == dbExePlantWorkerAbsenteeism.LocationCode &&
                                                                              c.ExePlantProductionEntryVerification.GroupCode == dbExePlantWorkerAbsenteeism.GroupCode &&
                                                                              c.ExePlantProductionEntryVerification.UnitCode == dbExePlantWorkerAbsenteeism.UnitCode &&
                                                                              c.ExePlantProductionEntryVerification.Shift == dbExePlantWorkerAbsenteeism.Shift &&
                                                                              c.ExePlantProductionEntryVerification.ProductionDate >= dbExePlantWorkerAbsenteeism.StartDateAbsent.Date &&
                                                                              c.ExePlantProductionEntryVerification.ProductionDate <= dbExePlantWorkerAbsenteeism.EndDateAbsent.Date);

            if (listProductionEntries.Any()) 
            {
                // Check Absent SLS / SLP
                if (dbExePlantWorkerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.SLS.ToString() || dbExePlantWorkerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.SLP.ToString())
                {
                    foreach (var prodEntry in listProductionEntries)
                    {
                        // Check if absent type is Multiskill Out
                        if (prodEntry.AbsentType == EnumHelper.GetDescription(Enums.SKTAbsentCode.MO))
                        {
                            // Get MinStickPerHour by ProductionEntry
                            var minStickPerhour = GetProcessSettingMinStickPerHour(prodEntry);

                            // Condition prod actual < minimal stick per hour
                            if (prodEntry.ProdActual < minStickPerhour)
                            {
                                // Get list destination production entry dummy
                                var listDestProdEntryDummys = _exePlantProductionEntryRepo.Get(c => c.EmployeeID == dbExePlantWorkerAbsenteeism.EmployeeID &&
                                                                                                    c.ExePlantProductionEntryVerification.GroupCode.Substring(1, 1) == "5" &&
                                                                                                    c.ExePlantProductionEntryVerification.ProductionDate >= dbExePlantWorkerAbsenteeism.StartDateAbsent.Date &&
                                                                                                    c.ExePlantProductionEntryVerification.ProductionDate <= dbExePlantWorkerAbsenteeism.EndDateAbsent.Date);
                                // Remove Dummy Group
                                foreach (var entryDummy in listDestProdEntryDummys)
                                {
                                    _exePlantProductionEntryRepo.Delete(entryDummy);
                                }

                                // Update source entry
                                prodEntry.AbsentType = mstAbsentType == null ? dbExePlantWorkerAbsenteeism.AbsentType : mstAbsentType.AbsentType;
                                prodEntry.AbsentCodeEblek = mstAbsentType == null ? workerAbsenteeism.SktAbsentCode : mstAbsentType.SktAbsentCode;
                                prodEntry.AbsentCodePayroll = mstAbsentType == null ? workerAbsenteeism.PayrollAbsentCode : mstAbsentType.PayrollAbsentCode;
                                prodEntry.StartDateAbsent = workerAbsenteeism.StartDateAbsent;
                                prodEntry.UpdatedDate = DateTime.Now;
                                prodEntry.IsFromAbsenteeism = true;

                                _exePlantProductionEntryRepo.Update(prodEntry);
                            }
                        }
                        else
                        {
                            // Update source entry
                            prodEntry.AbsentType = mstAbsentType == null ? dbExePlantWorkerAbsenteeism.AbsentType : mstAbsentType.AbsentType;
                            prodEntry.AbsentCodeEblek = mstAbsentType == null ? workerAbsenteeism.SktAbsentCode : mstAbsentType.SktAbsentCode;
                            prodEntry.AbsentCodePayroll = mstAbsentType == null ? workerAbsenteeism.PayrollAbsentCode : mstAbsentType.PayrollAbsentCode;
                            prodEntry.StartDateAbsent = workerAbsenteeism.StartDateAbsent;
                            prodEntry.UpdatedDate = DateTime.Now;
                            prodEntry.IsFromAbsenteeism = true;

                            _exePlantProductionEntryRepo.Update(prodEntry);
                        }
                    }

                    _uow.SaveChanges();

                    return Mapper.Map<ExePlantWorkerAbsenteeismDTO>(dbExePlantWorkerAbsenteeism);
                }
                else
                {
                    foreach (var prodEntry in listProductionEntries)
                    {
                        // Update production entry
                        prodEntry.AbsentType = mstAbsentType == null ? dbExePlantWorkerAbsenteeism.AbsentType : mstAbsentType.AbsentType;
                        prodEntry.AbsentCodeEblek = mstAbsentType == null ? workerAbsenteeism.SktAbsentCode : mstAbsentType.SktAbsentCode;
                        prodEntry.AbsentCodePayroll = mstAbsentType == null ? workerAbsenteeism.PayrollAbsentCode : mstAbsentType.PayrollAbsentCode;
                        prodEntry.StartDateAbsent = workerAbsenteeism.StartDateAbsent;
                        prodEntry.UpdatedDate = DateTime.Now;
                        prodEntry.IsFromAbsenteeism = true;

                        if (workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.C.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.S.ToString()
                                           || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.CT.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.CH.ToString()
                                           || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.SKR.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.LP1.ToString()
                                           || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.T.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.CT.ToString()
                                           || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.TL.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.A.ToString())
                        {
                            prodEntry.ProdTarget = 0;
                            prodEntry.ProdActual = 0;
                        }

                        _exePlantProductionEntryRepo.Update(prodEntry);
                    }

                    _uow.SaveChanges();

                    return Mapper.Map<ExePlantWorkerAbsenteeismDTO>(dbExePlantWorkerAbsenteeism);
                }
            }

            _uow.SaveChanges();

            return Mapper.Map<ExePlantWorkerAbsenteeismDTO>(dbExePlantWorkerAbsenteeism);
        }

        public IEnumerable<ProductionCardDTO> GetProductionCardSubmitted(GetExePlantWorkerAbsenteeismInput input)
        {
            // check production card
            var listProdCard = _productionCardRepo.Get(c => c.ProductionDate >= input.StartDateAbsent
                                                   && c.ProductionDate <= input.EndDateAbsent
                                                   && c.EmployeeID == input.EmployeeID);

            return listProdCard.Any() ? Mapper.Map<List<ProductionCardDTO>>(listProdCard) : new List<ProductionCardDTO>();
        }

        private int GetProcessSettingMinStickPerHour(ExePlantProductionEntry productionEntry)
        {
            //get ProcessGroup from employee
            var employee = _mstPlantEmpJobsdataAcvRepo.GetByID(productionEntry.EmployeeID);

            //get BrandGroup 
            var codes = productionEntry.ProductionEntryCode.Split('/');
            var brandCode = codes[5];

            var brand = _mstGenBrandRepo.GetByID(brandCode);

            // get MinStickPerHour from process setting
            var processSettings =
                _mstGenProcessSettingRepo.Get(m => m.ProcessGroup == employee.ProcessSettingsCode
                                                   && m.BrandGroupCode == brand.BrandGroupCode);

            var processSetting = processSettings.FirstOrDefault();

            return processSetting.MinStickPerHour.Value;
        }

        /// <summary>
        /// Validate if Exist Worker Absenteeism Data
        /// </summary>
        /// <param name="workerAbsenteeismToValidate"></param>
        /// <returns></returns>
        private void ValidateInsertWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        {
            //validate existing data
            ValidateExistingPlantWorkerAbsenteeismOnInsert(workerAbsenteeismToValidate);

            //validate end date
            ValidateEndDatePlantWorkerAbsenteeism(workerAbsenteeismToValidate);

            //validate date range
            ValidateDateRangePlantWorkerAbsenteeism(workerAbsenteeismToValidate);

            //validate maxday 
            ValidateMaxDaysPlantWorkerAbsenteeism(workerAbsenteeismToValidate);

            //validate absent type cannot null
            ValidateAbsentTypeCannotNull(workerAbsenteeismToValidate);

            //validate check SLS/SLP
            //ValidateCheckActualMinstickHourSLSSLP(workerAbsenteeismToValidate);
        }

        /// <summary>
        /// Update Worker Absenteeism Data
        /// </summary>
        /// <param name="workerAbsenteeismToValidate"></param>
        /// <returns></returns>
        public ExePlantWorkerAbsenteeismDTO UpdateWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeism)
        {
            var dbExePlantWorkerAbsenteeism = _exePlantWorkerAbsenteesimRepo.GetByID(workerAbsenteeism.OldValueStartDateAbsent, workerAbsenteeism.OldValueEmployeeID, workerAbsenteeism.OldValueShift);

            if (dbExePlantWorkerAbsenteeism == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            // Remove absenteeism when Absent Type is empty or null
            if(string.IsNullOrEmpty(workerAbsenteeism.AbsentType))
            {
                var listProductionEntries = _exePlantProductionEntryRepo.Get(c => c.EmployeeID == dbExePlantWorkerAbsenteeism.EmployeeID &&
                                                                                  c.ExePlantProductionEntryVerification.LocationCode == dbExePlantWorkerAbsenteeism.LocationCode &&
                                                                                  c.ExePlantProductionEntryVerification.GroupCode == dbExePlantWorkerAbsenteeism.GroupCode &&
                                                                                  c.ExePlantProductionEntryVerification.UnitCode == dbExePlantWorkerAbsenteeism.UnitCode &&
                                                                                  c.ExePlantProductionEntryVerification.Shift == dbExePlantWorkerAbsenteeism.Shift &&
                                                                                  c.ExePlantProductionEntryVerification.ProductionDate >= dbExePlantWorkerAbsenteeism.StartDateAbsent.Date &&
                                                                                  c.ExePlantProductionEntryVerification.ProductionDate <= dbExePlantWorkerAbsenteeism.EndDateAbsent.Date);

                foreach (var item in listProductionEntries)
                {
                    item.StartDateAbsent = null;
                    item.AbsentType = null;
                    item.AbsentCodeEblek = null;
                    item.AbsentCodePayroll = null;
                    item.ProdTarget = (float)item.ProdCapacity;
                    item.ProdActual = null;

                    _exePlantProductionEntryRepo.Update(item);

                }

                _exePlantWorkerAbsenteesimRepo.Delete(dbExePlantWorkerAbsenteeism);

                _uow.SaveChanges();

                return Mapper.Map<ExePlantWorkerAbsenteeismDTO>(dbExePlantWorkerAbsenteeism);
            }

            // Remove existing then Insert New (Edit)
            workerAbsenteeism.CreatedBy = dbExePlantWorkerAbsenteeism.CreatedBy;
            workerAbsenteeism.CreatedDate = dbExePlantWorkerAbsenteeism.CreatedDate;

            using (SKTISEntities context = new SKTISEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var existingAbsenteeism = context.ExePlantWorkerAbsenteeism.Find(workerAbsenteeism.OldValueStartDateAbsent, workerAbsenteeism.OldValueEmployeeID, workerAbsenteeism.OldValueShift);

                        // Delete Existing Absenteeism
                        context.ExePlantWorkerAbsenteeism.Remove(existingAbsenteeism);

                        // Get Existing Production Entry
                        var listProductionEntriesOld = context.ExePlantProductionEntries.Where(c => c.EmployeeID == dbExePlantWorkerAbsenteeism.EmployeeID &&
                                                                                      c.ExePlantProductionEntryVerification.LocationCode == dbExePlantWorkerAbsenteeism.LocationCode &&
                                                                                      c.ExePlantProductionEntryVerification.GroupCode == dbExePlantWorkerAbsenteeism.GroupCode &&
                                                                                      c.ExePlantProductionEntryVerification.UnitCode == dbExePlantWorkerAbsenteeism.UnitCode &&
                                                                                      c.ExePlantProductionEntryVerification.Shift == dbExePlantWorkerAbsenteeism.Shift &&
                                                                                      c.ExePlantProductionEntryVerification.ProductionDate >= dbExePlantWorkerAbsenteeism.StartDateAbsent.Date &&
                                                                                      c.ExePlantProductionEntryVerification.ProductionDate <= dbExePlantWorkerAbsenteeism.EndDateAbsent.Date);
                        
                        // Assign Null Old Production Entry
                        foreach (var item in listProductionEntriesOld)
                        {
                            item.StartDateAbsent = null;
                            item.AbsentType = null;
                            item.AbsentCodeEblek = null;
                            item.AbsentCodePayroll = null;
                            item.IsFromAbsenteeism = false;
                        }
                      
                        context.SaveChanges();

                        InsertAbsenteeismOnUpdate(workerAbsenteeism, context);

                        context.SaveChanges();

                        transaction.Commit();

                        return Mapper.Map<ExePlantWorkerAbsenteeismDTO>(dbExePlantWorkerAbsenteeism);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void InsertAbsenteeismOnUpdate(ExePlantWorkerAbsenteeismDTO workerAbsenteeism, SKTISEntities context) 
        {
            //validate insert on Update
            ValidateUpdateWorkerAbsenteeism(workerAbsenteeism, context);

            if (workerAbsenteeism.GroupCode == null) workerAbsenteeism.GroupCode = _mstPlantEmpJobsdataAcvRepo.GetByID(workerAbsenteeism.EmployeeID).GroupCode;

            var dbExePlantWorkerAbsenteeism = Mapper.Map<ExePlantWorkerAbsenteeism>(workerAbsenteeism);

            dbExePlantWorkerAbsenteeism.CreatedBy = workerAbsenteeism.CreatedBy;
            dbExePlantWorkerAbsenteeism.CreatedDate = workerAbsenteeism.CreatedDate;
            dbExePlantWorkerAbsenteeism.UpdatedDate = DateTime.Now;

            if (dbExePlantWorkerAbsenteeism.EndDateAbsent.Date < dbExePlantWorkerAbsenteeism.StartDateAbsent.Date) throw new BLLException(ExceptionCodes.BLLExceptions.EndDateLessThanStartDate);

            // Insert Worker Abseteeism
            context.ExePlantWorkerAbsenteeism.Add(dbExePlantWorkerAbsenteeism);

            // Get Master Absent Type
            var mstAbsentType = _mstPlantAbsentType.GetByID(dbExePlantWorkerAbsenteeism.AbsentType);

            // Update Production Entry
            var listProductionEntries = context.ExePlantProductionEntries.Where(c => c.EmployeeID == dbExePlantWorkerAbsenteeism.EmployeeID &&
                                                                                      c.ExePlantProductionEntryVerification.LocationCode == dbExePlantWorkerAbsenteeism.LocationCode &&
                                                                                      c.ExePlantProductionEntryVerification.GroupCode == dbExePlantWorkerAbsenteeism.GroupCode &&
                                                                                      c.ExePlantProductionEntryVerification.UnitCode == dbExePlantWorkerAbsenteeism.UnitCode &&
                                                                                      c.ExePlantProductionEntryVerification.Shift == dbExePlantWorkerAbsenteeism.Shift &&
                                                                                      c.ExePlantProductionEntryVerification.ProductionDate >= dbExePlantWorkerAbsenteeism.StartDateAbsent.Date &&
                                                                                      c.ExePlantProductionEntryVerification.ProductionDate <= dbExePlantWorkerAbsenteeism.EndDateAbsent.Date);

            if (listProductionEntries.Any())
            {
                // Check Absent SLS / SLP
                if (dbExePlantWorkerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.SLS.ToString() || dbExePlantWorkerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.SLP.ToString())
                {
                    foreach (var prodEntry in listProductionEntries)
                    {
                        // Check if absent type is NOT Multiskill Out
                        if (prodEntry.AbsentType == EnumHelper.GetDescription(Enums.SKTAbsentCode.MO))
                        {
                            // Get MinStickPerHour by ProductionEntry
                            var minStickPerhour = GetProcessSettingMinStickPerHour(prodEntry);

                            // Condition prod actual < minimal stick per hour
                            if (prodEntry.ProdActual < minStickPerhour)
                            {
                                // Get list destination production entry dummy
                                var listDestProdEntryDummys = context.ExePlantProductionEntries.Where(c => c.EmployeeID == dbExePlantWorkerAbsenteeism.EmployeeID &&
                                                                                                    c.ExePlantProductionEntryVerification.GroupCode.Substring(1, 1) == "5" &&
                                                                                                    c.ExePlantProductionEntryVerification.ProductionDate >= dbExePlantWorkerAbsenteeism.StartDateAbsent.Date &&
                                                                                                    c.ExePlantProductionEntryVerification.ProductionDate <= dbExePlantWorkerAbsenteeism.EndDateAbsent.Date);
                                // Remove Dummy Group
                                foreach (var entryDummy in listDestProdEntryDummys)
                                {
                                    context.ExePlantProductionEntries.Remove(entryDummy);
                                }

                                // Update source entry
                                prodEntry.AbsentType = mstAbsentType == null ? dbExePlantWorkerAbsenteeism.AbsentType : mstAbsentType.AbsentType;
                                prodEntry.AbsentCodeEblek = mstAbsentType == null ? workerAbsenteeism.SktAbsentCode : mstAbsentType.SktAbsentCode;
                                prodEntry.AbsentCodePayroll = mstAbsentType == null ? workerAbsenteeism.PayrollAbsentCode : mstAbsentType.PayrollAbsentCode;
                                prodEntry.StartDateAbsent = workerAbsenteeism.StartDateAbsent;
                                prodEntry.UpdatedDate = DateTime.Now;
                                prodEntry.IsFromAbsenteeism = true;
                            }
                        }
                        else 
                        {
                            // Update production entry
                            prodEntry.AbsentType = mstAbsentType == null ? dbExePlantWorkerAbsenteeism.AbsentType : mstAbsentType.AbsentType;
                            prodEntry.AbsentCodeEblek = mstAbsentType == null ? workerAbsenteeism.SktAbsentCode : mstAbsentType.SktAbsentCode;
                            prodEntry.AbsentCodePayroll = mstAbsentType == null ? workerAbsenteeism.PayrollAbsentCode : mstAbsentType.PayrollAbsentCode;
                            prodEntry.StartDateAbsent = workerAbsenteeism.StartDateAbsent;
                            prodEntry.UpdatedDate = DateTime.Now;
                            prodEntry.IsFromAbsenteeism = true;
                        }
                    }
                }
                else
                {
                    foreach (var prodEntry in listProductionEntries)
                    {
                        // Update production entry
                        prodEntry.AbsentType = mstAbsentType == null ? dbExePlantWorkerAbsenteeism.AbsentType : mstAbsentType.AbsentType;
                        prodEntry.AbsentCodeEblek = mstAbsentType == null ? workerAbsenteeism.SktAbsentCode : mstAbsentType.SktAbsentCode;
                        prodEntry.AbsentCodePayroll = mstAbsentType == null ? workerAbsenteeism.PayrollAbsentCode : mstAbsentType.PayrollAbsentCode;
                        prodEntry.StartDateAbsent = workerAbsenteeism.StartDateAbsent;
                        prodEntry.UpdatedDate = DateTime.Now;
                        prodEntry.IsFromAbsenteeism = true;

                        if (workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.C.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.S.ToString()
                                           || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.CT.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.CH.ToString()
                                           || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.SKR.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.LP1.ToString()
                                           || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.T.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.CT.ToString()
                                           || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.TL.ToString() || workerAbsenteeism.SktAbsentCode == Enums.SKTAbsentCode.A.ToString())
                        {
                            prodEntry.ProdTarget = 0;
                            prodEntry.ProdActual = 0;
                        }
                    }
                }
            }
        }

        private void ValidateUpdateWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate, SKTISEntities context)
        {
            //validate existing data
            ValidateExistingPlantWorkerAbsenteeismOnUpdate(workerAbsenteeismToValidate, context);

            //validate end date
            ValidateEndDatePlantWorkerAbsenteeism(workerAbsenteeismToValidate);

            //validate date range
            ValidateDateRangePlantWorkerAbsenteeismOnUpdate(workerAbsenteeismToValidate, context);

            //validate maxday 
            ValidateMaxDaysPlantWorkerAbsenteeismOnUpdate(workerAbsenteeismToValidate, context);

            //validate absent type cannot null
            ValidateAbsentTypeCannotNull(workerAbsenteeismToValidate);

            //validate check SLS/SLP
            ValidateCheckActualMinstickHourSLSSLPOnUpdate(workerAbsenteeismToValidate, context);
        }

        public void ValidateExistingPlantWorkerAbsenteeismOnUpdate(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate, SKTISEntities context)
        {
            var dbExePlantWorkerAbsenteeism = context.ExePlantWorkerAbsenteeism.Find(workerAbsenteeismToValidate.StartDateAbsent, workerAbsenteeismToValidate.EmployeeID, workerAbsenteeismToValidate.Shift);

            if (dbExePlantWorkerAbsenteeism != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist, " Already Exist Worker Absenteeism");
        }

        public void ValidateCheckActualMinstickHourSLSSLP(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        { 
            // Check Absent SLS / SLP
            if (workerAbsenteeismToValidate.SktAbsentCode == Enums.SKTAbsentCode.SLS.ToString() || workerAbsenteeismToValidate.SktAbsentCode == Enums.SKTAbsentCode.SLP.ToString())
            {
                // Update Production Entry
                var listProductionEntries = _exePlantProductionEntryRepo.Get(c => c.EmployeeID == workerAbsenteeismToValidate.EmployeeID &&
                                                                                  c.ExePlantProductionEntryVerification.LocationCode == workerAbsenteeismToValidate.LocationCode &&
                                                                                  c.ExePlantProductionEntryVerification.GroupCode == workerAbsenteeismToValidate.GroupCode &&
                                                                                  c.ExePlantProductionEntryVerification.UnitCode == workerAbsenteeismToValidate.UnitCode &&
                                                                                  c.ExePlantProductionEntryVerification.Shift == workerAbsenteeismToValidate.Shift &&
                                                                                  c.ExePlantProductionEntryVerification.ProductionDate >= workerAbsenteeismToValidate.StartDateAbsent.Date &&
                                                                                  c.ExePlantProductionEntryVerification.ProductionDate <= workerAbsenteeismToValidate.EndDateAbsent.Date);

                foreach (var prodEntry in listProductionEntries)
                {
                    // Check if absent type is Multiskill Out
                    if (prodEntry.AbsentType == EnumHelper.GetDescription(Enums.SKTAbsentCode.MO))
                    {
                        // Get MinStickPerHour by ProductionEntry
                        var minStickPerhour = GetProcessSettingMinStickPerHour(prodEntry);

                        if (prodEntry.ProdActual >= minStickPerhour)
                            throw new BLLException(ExceptionCodes.BLLExceptions.ActualGreaterOrIsSameThanMinStickHour);
                    }
                }
            }
        }

        public void ValidateCheckActualMinstickHourSLSSLPOnUpdate(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate, SKTISEntities context)
        {
            // Check Absent SLS / SLP
            if (workerAbsenteeismToValidate.SktAbsentCode == Enums.SKTAbsentCode.SLS.ToString() || workerAbsenteeismToValidate.SktAbsentCode == Enums.SKTAbsentCode.SLP.ToString())
            {
                // Update Production Entry
                var listProductionEntries = context.ExePlantProductionEntries.Where(c => c.EmployeeID == workerAbsenteeismToValidate.EmployeeID &&
                                                                                  c.ExePlantProductionEntryVerification.LocationCode == workerAbsenteeismToValidate.LocationCode &&
                                                                                  c.ExePlantProductionEntryVerification.GroupCode == workerAbsenteeismToValidate.GroupCode &&
                                                                                  c.ExePlantProductionEntryVerification.UnitCode == workerAbsenteeismToValidate.UnitCode &&
                                                                                  c.ExePlantProductionEntryVerification.Shift == workerAbsenteeismToValidate.Shift &&
                                                                                  c.ExePlantProductionEntryVerification.ProductionDate >= workerAbsenteeismToValidate.StartDateAbsent.Date &&
                                                                                  c.ExePlantProductionEntryVerification.ProductionDate <= workerAbsenteeismToValidate.EndDateAbsent.Date).ToList();

                foreach (var prodEntry in listProductionEntries)
                {
                    // Check if absent type is Multiskill Out
                    if (prodEntry.AbsentType == EnumHelper.GetDescription(Enums.SKTAbsentCode.MO))
                    {
                        // Get MinStickPerHour by ProductionEntry
                        var minStickPerhour = GetProcessSettingMinStickPerHour(prodEntry);

                        if (prodEntry.ProdActual >= minStickPerhour)
                            throw new BLLException(ExceptionCodes.BLLExceptions.ActualGreaterOrIsSameThanMinStickHour);
                    }
                }
            }
        }

        public void ValidateDateRangePlantWorkerAbsenteeismOnUpdate(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate, SKTISEntities context)
        {
            //worker absen same type
            var existingPlantWorkerAbsenteeisms = context.ExePlantWorkerAbsenteeism.Where(m => m.AbsentType == workerAbsenteeismToValidate.AbsentType && 
                                                                                                m.EmployeeID == workerAbsenteeismToValidate.EmployeeID).ToList();

            if (existingPlantWorkerAbsenteeisms.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(workerAbsenteeismToValidate.StartDateAbsent, 
                                                                                                                    workerAbsenteeismToValidate.EndDateAbsent, 
                                                                                                                    existingPlantWorkerAbsenteeism.StartDateAbsent,
                                                                                                                    existingPlantWorkerAbsenteeism.EndDateAbsent)))
            {
                var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                var errorMessage = string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion), workerAbsenteeismToValidate.StartDateAbsent.Date, 
                                                                                                                                  workerAbsenteeismToValidate.EndDateAbsent.Date);
                throw new BLLException(errorCode, errorMessage);
            }

            //worker absen different type
            //Add by Indra permana
            //refer to ticket: http://tp.voxteneo.co.id/entity/3273
            var existingPlantWorkerAbsenteeismsDifType = context.ExePlantWorkerAbsenteeism.Where(m => m.LocationCode == workerAbsenteeismToValidate.LocationCode && 
                                                                                                  m.EmployeeID == workerAbsenteeismToValidate.EmployeeID).ToList();

            if (existingPlantWorkerAbsenteeismsDifType.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(workerAbsenteeismToValidate.StartDateAbsent,
                                                                                                                           workerAbsenteeismToValidate.EndDateAbsent, 
                                                                                                                           existingPlantWorkerAbsenteeism.StartDateAbsent,
                                                                                                                           existingPlantWorkerAbsenteeism.EndDateAbsent)))
            {
                var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                var errorMessage = string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion), workerAbsenteeismToValidate.StartDateAbsent.ToShortDateString(), 
                                                                                                                                  workerAbsenteeismToValidate.EndDateAbsent.ToShortDateString());
                throw new BLLException(errorCode, errorMessage);
            }

            ////production entry
            //string entryCode = workerAbsenteeismToValidate.StartDateAbsent.Year.ToString() + "/" +
            //                    _masterDataBll.GetWeekByDate(workerAbsenteeismToValidate.StartDateAbsent).Week.ToString() + "/";

            //var existingPlantProductionEntries = context.ExePlantProductionEntries.Where(m => m.AbsentType == workerAbsenteeismToValidate.AbsentType && 
            //                                                                                   m.EmployeeID == workerAbsenteeismToValidate.EmployeeID && 
            //                                                                                   m.StartDateAbsent.HasValue && 
            //                                                                                   m.ProductionEntryCode.Contains(entryCode)).ToList();

            //if (
            //    existingPlantProductionEntries.Where(existingPlantProductionEntry => existingPlantProductionEntry.StartDateAbsent.HasValue)
            //        .Any(existingPlantProductionEntry => DataRangeIsNotUnionOrOverlap(workerAbsenteeismToValidate.StartDateAbsent,
            //                                                                          workerAbsenteeismToValidate.EndDateAbsent,
            //                                                                          existingPlantProductionEntry.StartDateAbsent.Value,
            //                                                                          existingPlantProductionEntry.StartDateAbsent.Value)))
            //{
            //    var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
            //    var errorMessage = string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion), workerAbsenteeismToValidate.StartDateAbsent.ToShortDateString(),
            //                                                                                                                      workerAbsenteeismToValidate.EndDateAbsent.ToShortDateString());
            //    throw new BLLException(errorCode, errorMessage);
            //}
        }

        private void ValidateStartDateAndEndDatePlantWorkerAbsenteeism(
            ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        {
            var dbExePlantWorkerAbsenteeismExisting = _exePlantWorkerAbsenteesimRepo.GetByID(
                workerAbsenteeismToValidate.StartDateAbsent, workerAbsenteeismToValidate.EmployeeID,
                workerAbsenteeismToValidate.Shift);

            if (workerAbsenteeismToValidate.StartDateAbsent < dbExePlantWorkerAbsenteeismExisting.StartDateAbsent)
                throw new BLLException(ExceptionCodes.BLLExceptions.StartDateLessThanCurrentStartDate);

            if (workerAbsenteeismToValidate.EndDateAbsent < dbExePlantWorkerAbsenteeismExisting.EndDateAbsent)
                throw new BLLException(ExceptionCodes.BLLExceptions.EndDateLessThanCurrentEndDate);
        }

        private void ValidateExistingPlantWorkerAbsenteeismOnInsert(
            ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        {
            var dbExePlantWorkerAbsenteeism = _exePlantWorkerAbsenteesimRepo.GetByID(
                workerAbsenteeismToValidate.StartDateAbsent, workerAbsenteeismToValidate.EmployeeID,
                workerAbsenteeismToValidate.Shift);

            if (dbExePlantWorkerAbsenteeism != null)
            {
                //if (workerAbsenteeismToValidate.IsFromWorkerAssignment)
                //    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist,
                //    " in Absenteeism");
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist,
                    " Already Exist Worker Absenteeism");
            }
        }

        private void ValidateEndDatePlantWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        {
            if (workerAbsenteeismToValidate.EndDateAbsent.Date < workerAbsenteeismToValidate.StartDateAbsent.Date)
                throw new BLLException(ExceptionCodes.BLLExceptions.EndDateLessThanStartDate);
        }

        private void ValidateEndDateMustLessThanStartDate(ExePlantWorkerAbsenteeism workerAbsenteeismToValidate) 
        {
            if(workerAbsenteeismToValidate.EndDateAbsent.Date < workerAbsenteeismToValidate.StartDateAbsent.Date)
                throw new BLLException(ExceptionCodes.BLLExceptions.EndDateLessThanStartDate);
        }

        private void ValidateDateRangePlantWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        {
            //worker absen same type
            var existingPlantWorkerAbsenteeisms =
                _exePlantWorkerAbsenteesimRepo.Get(m => m.AbsentType == workerAbsenteeismToValidate.AbsentType
                                                        && m.EmployeeID == workerAbsenteeismToValidate.EmployeeID);


            if (existingPlantWorkerAbsenteeisms.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(workerAbsenteeismToValidate.StartDateAbsent,
                workerAbsenteeismToValidate.EndDateAbsent, existingPlantWorkerAbsenteeism.StartDateAbsent,
                existingPlantWorkerAbsenteeism.EndDateAbsent)))
            {
                var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion),
                        workerAbsenteeismToValidate.StartDateAbsent.Date, workerAbsenteeismToValidate.EndDateAbsent.Date);
                throw new BLLException(errorCode, errorMessage);
            }

            //worker absen different type
            //Add by Indra permana
            //refer to ticket: http://tp.voxteneo.co.id/entity/3273
            var existingPlantWorkerAbsenteeismsDifType =
                _exePlantWorkerAbsenteesimRepo.Get(m => m.EmployeeID == workerAbsenteeismToValidate.EmployeeID);


            if (existingPlantWorkerAbsenteeismsDifType.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(workerAbsenteeismToValidate.StartDateAbsent,
                workerAbsenteeismToValidate.EndDateAbsent, existingPlantWorkerAbsenteeism.StartDateAbsent,
                existingPlantWorkerAbsenteeism.EndDateAbsent)))
            {
                var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion),
                        workerAbsenteeismToValidate.StartDateAbsent.ToShortDateString(), workerAbsenteeismToValidate.EndDateAbsent.ToShortDateString());
                throw new BLLException(errorCode, errorMessage);
            }

            ////production entry
            //string entryCode = workerAbsenteeismToValidate.StartDateAbsent.Year.ToString() +
            //                    "/" +
            //                    _masterDataBll.GetWeekByDate(workerAbsenteeismToValidate.StartDateAbsent).Week.ToString() +
            //                    "/";

            //var existingPlantProductionEntries =
            //    _exePlantProductionEntryRepo.Get(m => m.AbsentType == workerAbsenteeismToValidate.AbsentType
            //                                          && m.EmployeeID == workerAbsenteeismToValidate.EmployeeID
            //                                          && m.StartDateAbsent.HasValue
            //                                          && m.ProductionEntryCode.Contains(entryCode));

            //if (
            //    existingPlantProductionEntries.Where(
            //        existingPlantProductionEntry => existingPlantProductionEntry.StartDateAbsent.HasValue)
            //        .Any(
            //            existingPlantProductionEntry =>
            //                DataRangeIsNotUnionOrOverlap(workerAbsenteeismToValidate.StartDateAbsent,
            //                    workerAbsenteeismToValidate.EndDateAbsent,
            //                    existingPlantProductionEntry.StartDateAbsent.Value,
            //                    existingPlantProductionEntry.StartDateAbsent.Value)))
            //{
            //    var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
            //    var errorMessage =
            //        string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion),
            //            workerAbsenteeismToValidate.StartDateAbsent.Date, workerAbsenteeismToValidate.EndDateAbsent.Date);
            //    throw new BLLException(errorCode, errorMessage);
            //}
        }

        private void ValidateDateRangePlantWorkerAbsenteeismOnUpdate(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        {
            var existingPlantWorkerAbsenteeisms =
                    _exePlantWorkerAbsenteesimRepo.Get(m => m.EmployeeID == workerAbsenteeismToValidate.EmployeeID &&
                                                            m.AbsentType != workerAbsenteeismToValidate.AbsentType &&
                                                            m.StartDateAbsent != workerAbsenteeismToValidate.StartDateAbsent);

            if (existingPlantWorkerAbsenteeisms.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(workerAbsenteeismToValidate.StartDateAbsent,
                workerAbsenteeismToValidate.EndDateAbsent, existingPlantWorkerAbsenteeism.StartDateAbsent,
                existingPlantWorkerAbsenteeism.EndDateAbsent)))
            {
                var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion),
                        workerAbsenteeismToValidate.StartDateAbsent.Date, workerAbsenteeismToValidate.EndDateAbsent.Date);
                throw new BLLException(errorCode, errorMessage);
            }
        }

        //see test mehod DataRangeIsNotUnionOrOverlapTest
        private bool DataRangeIsNotUnionOrOverlap(DateTime startDate, DateTime endDate, DateTime existingStartDate,
            DateTime existingEndDate)
        {
            if (startDate.Date > endDate.Date || existingStartDate.Date > existingEndDate.Date)
                return false;
            //throw new Exception("End date must be greater that start date.");

            if (startDate.Date == existingStartDate.Date || endDate.Date == existingEndDate.Date
                || startDate.Date == existingEndDate.Date || endDate.Date == existingStartDate.Date)
                return false;

            if (startDate.Date < existingStartDate.Date)
            {
                if (endDate.Date > existingStartDate.Date && endDate.Date < existingEndDate.Date)
                    return false;

                if (endDate.Date > existingEndDate.Date)
                    return false;
            }
            else
            {
                if (existingEndDate.Date > startDate.Date && existingEndDate.Date < endDate.Date)
                    return false;

                if (existingEndDate.Date > endDate.Date)
                    return false;
            }

            return true;
        }

        private void ValidateMaxDaysPlantWorkerAbsenteeism(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        {

            //get maxday absent by absent type
            var absentMaxDays = GetMaxDayByAbsentType(workerAbsenteeismToValidate.AbsentType);

            if (absentMaxDays <= 0) return;

            if (workerAbsenteeismToValidate.StartDateAbsent == workerAbsenteeismToValidate.OldValueStartDateAbsent
                && workerAbsenteeismToValidate.EndDateAbsent == workerAbsenteeismToValidate.OldValueEndDateAbsent)
                return;

            //get current total absen day
            var totalWorkerAbsentDays = GetTotalDayAbsent(workerAbsenteeismToValidate);

            //validate if the absent max days less than current total worker absent days + user input
            var userInputAbsentDays =
                (workerAbsenteeismToValidate.EndDateAbsent.Date - workerAbsenteeismToValidate.StartDateAbsent.Date).Days;
            var maxDaysOverage = absentMaxDays - (totalWorkerAbsentDays + userInputAbsentDays);

            //validate if total input absent days > max day absent type
            if (userInputAbsentDays > absentMaxDays)
            {
                var errorCode = ExceptionCodes.BLLExceptions.MaxDayAbsentReached;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MaxDayAbsentReached),
                        workerAbsenteeismToValidate.AbsentType);
                throw new BLLException(errorCode, errorMessage);
            }
            else if (userInputAbsentDays < totalWorkerAbsentDays && userInputAbsentDays < absentMaxDays)
            {
                return;
            }

            //validate if the absent max days less than current total worker absent days
            if (totalWorkerAbsentDays >= absentMaxDays)
            {
                var errorCode = ExceptionCodes.BLLExceptions.MaxDayAbsentReached;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MaxDayAbsentReached),
                        workerAbsenteeismToValidate.AbsentType);
                throw new BLLException(errorCode, errorMessage);
            }

            if (maxDaysOverage > absentMaxDays)
            //if (maxDaysOverage > 0)
            {
                var errorCode = ExceptionCodes.BLLExceptions.TotalMaxDayAbsent;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.TotalMaxDayAbsent),
                        workerAbsenteeismToValidate.AbsentType, maxDaysOverage);
                throw new BLLException(errorCode, errorMessage);
            }

        }

        private void ValidateMaxDaysPlantWorkerAbsenteeismOnUpdate(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate, SKTISEntities context)
        {

            //get maxday absent by absent type
            var absentMaxDays = GetMaxDayByAbsentType(workerAbsenteeismToValidate.AbsentType);

            if (absentMaxDays <= 0) return;

            if (workerAbsenteeismToValidate.StartDateAbsent == workerAbsenteeismToValidate.OldValueStartDateAbsent
                && workerAbsenteeismToValidate.EndDateAbsent == workerAbsenteeismToValidate.OldValueEndDateAbsent)
                return;

            //get current total absen day
            var totalWorkerAbsentDays = GetTotalDayAbsentOnUpdate(workerAbsenteeismToValidate, context);

            //validate if the absent max days less than current total worker absent days + user input
            var userInputAbsentDays =
                (workerAbsenteeismToValidate.EndDateAbsent.Date - workerAbsenteeismToValidate.StartDateAbsent.Date).Days;
            var maxDaysOverage = absentMaxDays - (totalWorkerAbsentDays + userInputAbsentDays);

            //validate if total input absent days > max day absent type
            if (userInputAbsentDays > absentMaxDays)
            {
                var errorCode = ExceptionCodes.BLLExceptions.MaxDayAbsentReached;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MaxDayAbsentReached),
                        workerAbsenteeismToValidate.AbsentType);
                throw new BLLException(errorCode, errorMessage);
            }
            else if (userInputAbsentDays < totalWorkerAbsentDays && userInputAbsentDays < absentMaxDays)
            {
                return;
            }

            //validate if the absent max days less than current total worker absent days
            if (totalWorkerAbsentDays >= absentMaxDays)
            {
                var errorCode = ExceptionCodes.BLLExceptions.MaxDayAbsentReached;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.MaxDayAbsentReached),
                        workerAbsenteeismToValidate.AbsentType);
                throw new BLLException(errorCode, errorMessage);
            }

            if (maxDaysOverage > absentMaxDays)
            //if (maxDaysOverage > 0)
            {
                var errorCode = ExceptionCodes.BLLExceptions.TotalMaxDayAbsent;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.TotalMaxDayAbsent),
                        workerAbsenteeismToValidate.AbsentType, maxDaysOverage);
                throw new BLLException(errorCode, errorMessage);
            }

        }

        private void ValidateAbsentTypeCannotNull(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate) 
        {
            if(string.IsNullOrEmpty(workerAbsenteeismToValidate.AbsentType))
                throw new BLLException(ExceptionCodes.BLLExceptions.AbsentTypeNull);
        }

        public int GetMaxDayByAbsentType(string absentType)
        {
            return (absentType != null ? _exeMstPlantAbsentTypeRepo.GetByID(absentType).MaxDay : 0);
        }

        private int GetTotalDayAbsent(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate)
        {
            //get worker absent by absent type and start date year
            var existingPlantWorkerAbsenteeisms =
                _exePlantWorkerAbsenteesimRepo.Get(m => m.AbsentType == workerAbsenteeismToValidate.AbsentType
                                                        && m.EmployeeID == workerAbsenteeismToValidate.EmployeeID
                                                        &&
                                                        m.StartDateAbsent.Year ==
                                                        workerAbsenteeismToValidate.StartDateAbsent.Year);

            return
                existingPlantWorkerAbsenteeisms.Sum(
                    existingPlantWorkerAbsenteeism =>
                        (existingPlantWorkerAbsenteeism.EndDateAbsent.Date -
                         existingPlantWorkerAbsenteeism.StartDateAbsent.Date).Days);
        }

        private int GetTotalDayAbsentOnUpdate(ExePlantWorkerAbsenteeismDTO workerAbsenteeismToValidate, SKTISEntities context) 
        {
            //get worker absent by absent type and start date year
            var existingPlantWorkerAbsenteeisms = context.ExePlantWorkerAbsenteeism.Where(m => m.AbsentType == workerAbsenteeismToValidate.AbsentType && 
                                                                                               m.EmployeeID == workerAbsenteeismToValidate.EmployeeID && 
                                                                                               m.StartDateAbsent.Year == workerAbsenteeismToValidate.StartDateAbsent.Year).ToList();

            return
                existingPlantWorkerAbsenteeisms.Sum(existingPlantWorkerAbsenteeism => (existingPlantWorkerAbsenteeism.EndDateAbsent.Date - existingPlantWorkerAbsenteeism.StartDateAbsent.Date).Days);
        }

        /// <summary>
        /// Get Production Entry By Worker Absenteeism Parameter
        /// </summary>
        /// <param name="input">Parameter from worker absenteeism</param>
        /// <returns></returns>
        private IEnumerable<ExePlantProductionEntryDTO> GetExePlantProductionEntriesFromWorkerAbsenteeism(
            GetExePlantWorkerAbsenteeismInput input)
        {
            //combine code ProductionEntryCode
            var sourceCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}",
                Enums.CombineCode.EBL.ToString(),
                input.LocationCode,
                input.Shift,
                input.UnitCode,
                input.GroupCode,
                input.KPSYear,
                input.KPSWeek);

            var queryFilter = PredicateHelper.True<ExePlantProductionEntry>();

            queryFilter =
                queryFilter.And(
                    m => m.EmployeeID == input.EmployeeID && m.ProductionEntryCode.Contains(sourceCombineCode)
                         &&
                         (DbFunctions.TruncateTime(m.ExePlantProductionEntryVerification.ProductionDate) >=
                          input.StartDateAbsent.Date
                          &&
                          DbFunctions.TruncateTime(m.ExePlantProductionEntryVerification.ProductionDate) <=
                          input.EndDateAbsent.Date));

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantProductionEntry>();

            var dbResult = _exePlantProductionEntryRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<ExePlantProductionEntryDTO>>(dbResult);
        }

        /// <summary>
        /// Get Production Entry Verification By Worker Absenteeism Parameter
        /// </summary>
        /// <param name="input">Parameter from worker absenteeism</param>
        /// <returns></returns>
        private IEnumerable<ExePlantProductionEntryVerificationDTO>
            GetPlantProductionEntryVerificationFromWorkerAbsenteeism(GetExePlantWorkerAbsenteeismInput input)
        {
            //combine code ProductionEntryCode
            var sourceCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}",
                Enums.CombineCode.EBL.ToString(),
                input.LocationCode,
                input.Shift,
                input.UnitCode,
                input.GroupCode,
                input.KPSYear,
                input.KPSWeek);

            var dbResult = _exePlantProductionEntryRepo.Get(m => m.ProductionEntryCode.Contains(sourceCombineCode));

            return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult);
        }

        /// <summary>
        /// Calculate Employee Actual / StdStickPerHour
        /// </summary>
        /// <param name="input">Parameter from worker absenteeism</param>
        /// <returns></returns>
        public int CalculateEmployeeActualAndStdStickPerHour(GetExePlantWorkerAbsenteeismInput input)
        {
            //Variables Actual Employee in Production Entry (Default = 0)
            var actualEmployeeProdEntry = 0;
            //Variables StdStickPerHour in MstGenProcessSetting (Default = 0)
            var stdStickPerHour = 0;

            //Get Value Actual Employee in Production Entry
            var prodEntries = GetExePlantProductionEntriesFromWorkerAbsenteeism(input);
            actualEmployeeProdEntry = Convert.ToInt32(prodEntries.Select(c => c.ProdActual == null ? 0 : c.ProdActual.Value).Sum());

            //Get Production Entry Verification (Get BrandCode)
            var prodEntryVerifications = GetPlantProductionEntryVerificationFromWorkerAbsenteeism(input);
            //Get MstGenBrand by BrandCode on Production Enry Verification
            var brandCode =
                _masterDataBll.GetMstGenByBrandCode(prodEntryVerifications.Select(c => c.BrandCode).FirstOrDefault());

            //Get Value stdStickPerHour from MstGenProcessSetting by ProcessGroup and BrandGroupCode(MstGenBrand)
            var mstGenProcessSetting =
                _masterDataBll.GetStdStickPerHourByProcessAndBrandGroupCode(input.Process,
                    brandCode == null ? "" : brandCode.BrandGroupCode).FirstOrDefault();
            stdStickPerHour = mstGenProcessSetting == null
                ? 0
                : mstGenProcessSetting.StdStickPerHour == null ? 0 : mstGenProcessSetting.StdStickPerHour.Value;

            //Calculation Employee Actual / StdStickPerHour
            if (stdStickPerHour == 0) return 0;
            return (actualEmployeeProdEntry / stdStickPerHour);
        }


        public void InsertWorkerAbsenteeismFromEblek(ExePlantWorkerAbsenteeismDTO workerAbsenteeism)
        {
            //validate insert
            ValidateInsertWorkerAbsenteeism(workerAbsenteeism);

            if (workerAbsenteeism.GroupCode == null)
                workerAbsenteeism.GroupCode =
                    _mstPlantEmpJobsdataAcvRepo.GetByID(workerAbsenteeism.EmployeeID).GroupCode;

            var dbExePlantWorkerAbsenteeism = Mapper.Map<ExePlantWorkerAbsenteeism>(workerAbsenteeism);

            _exePlantWorkerAbsenteesimRepo.Insert(dbExePlantWorkerAbsenteeism);

            dbExePlantWorkerAbsenteeism.CreatedDate = DateTime.Now;
            dbExePlantWorkerAbsenteeism.UpdatedDate = DateTime.Now;
        }

        public int GetMaxDayProdCard(string idEmployee, string absentType, int year)
        {
            var absentTypes = _masterDataBll.GetMstPlantAbsentTypeById(absentType);
            if (absentTypes.MaxDay == 0) return 0;

            var queryFilter = PredicateHelper.True<ExePlantWorkerAbsenteeism>();

            queryFilter = queryFilter.And(m => m.EmployeeID == idEmployee);
            queryFilter = queryFilter.And(m => m.AbsentType == absentType);
            queryFilter = queryFilter.And(m => m.TransactionDate.Year == year);

            var dbResult = _exePlantWorkerAbsenteesimRepo.Get(queryFilter);

            var MaxDay = new List<int>();

            foreach (var exePlantWorkerAbsenteeism in dbResult)
            {
                MaxDay.AddRange(_generalBll.EachDay(exePlantWorkerAbsenteeism.StartDateAbsent, exePlantWorkerAbsenteeism.EndDateAbsent).Select(day => 1));
            }

            //return absentTypes.MaxDay.HasValue ? absentTypes.MaxDay.Value - MaxDay.Sum() : 0;
            return absentTypes.MaxDay.HasValue ? absentTypes.MaxDay.Value : 0;
        }

        public List<SuratPeriodeComposite> GetAbsenteeimForSuratPeriodeComposites(string employeeId, string productionDate, string locationCode, string unitCode, int shift, string groupCode, string processGroup, string brandCode, string remark)
        {
            var queryFilter = PredicateHelper.True<ProductionCard>();
            DateTime dd = DateTime.Parse(productionDate);
            var closingPayroll = _masterDataBll.GetClosingPayrollBeforeTodayDateTime(dd);
            var closingPayrollBefore = _masterDataBll.GetClosingPayrollThreePeriodBeforeLastClosingPayroll(closingPayroll);
            var closingPayrollAgo = _masterDataBll.GetClosingPayrollBeforeTodayDateTime(closingPayrollBefore);
            
            //queryFilter = queryFilter.And(m => m.EmployeeID == employeeId && m.LocationCode == locationCode && m.UnitCode == unitCode && m.Shift == shift && m.ProcessGroup == processGroup && m.BrandCode == brandCode);
            queryFilter = queryFilter.And(m => m.EmployeeID == employeeId);
            queryFilter = queryFilter.And(m => m.ProductionDate > closingPayrollAgo && m.ProductionDate <= closingPayroll);

            if (string.IsNullOrEmpty(remark) || remark.ToLower() == "null")
                queryFilter = queryFilter.And(m => m.EblekAbsentType == "A" || m.EblekAbsentType == "I");

            var list = new string[] { Enums.SKTAbsentCode.A.ToString(), Enums.SKTAbsentCode.I.ToString() };
            
            string[] strSplit = remark.Split(';');
            var data = new List<DateTime>();

            foreach (var item in strSplit) {
                if (item == "X") continue;
                if (item.Contains("J")) continue;
                if (String.IsNullOrEmpty(item) || item == "null") continue;
                data.Add(DateTime.ParseExact(item, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
            }

            if (string.IsNullOrEmpty(remark) || remark.ToLower() == "null")
            {
                queryFilter = queryFilter.And(m => (!m.Remark.Contains("x") || !m.Remark.Contains("X")) || (m.Remark == null));
            }
            else
            {
                //queryFilter = queryFilter.And(m => ((!m.Remark.Contains("x") || !m.Remark.Contains("X")) || m.Remark == null) && data.Contains(m.ProductionDate));
                queryFilter = queryFilter.And(m => data.Contains(m.ProductionDate));
            }

            var dbResultNoRemark = new List<ProductionCard>();

            if (!string.IsNullOrEmpty(remark) || remark.ToLower() != "null") {
                var queryFilter2 = PredicateHelper.True<ProductionCard>();
                //queryFilter2 = queryFilter2.And(m => m.EmployeeID == employeeId && m.LocationCode == locationCode && m.UnitCode == unitCode && m.Shift == shift && m.ProcessGroup == processGroup && m.BrandCode == brandCode);
                queryFilter2 = queryFilter2.And(m => m.EmployeeID == employeeId);
                queryFilter2 = queryFilter2.And(m => m.ProductionDate > closingPayrollAgo && m.ProductionDate <= closingPayroll);
                queryFilter2 = queryFilter2.And(m => m.EblekAbsentType == "A" || m.EblekAbsentType == "I");
                queryFilter2 = queryFilter2.And(m => (!m.Remark.Contains("x") || !m.Remark.Contains("X")) || (m.Remark == null));

                dbResultNoRemark = _productionCardRepo.Get(queryFilter2).Distinct().ToList();
            }

            var dbResult = _productionCardRepo.Get(queryFilter).Distinct().ToList();

            dbResult.AddRange(dbResultNoRemark);

            var result = new List<SuratPeriodeComposite>();
            foreach (var exePlantWorkerAbsenteeism in dbResult.Distinct().OrderBy(c => c.ProductionDate))
            {
                var absentType = "";
                if (exePlantWorkerAbsenteeism.EblekAbsentType == "A")
                {
                    absentType = "Alpa";
                }
                else if (exePlantWorkerAbsenteeism.EblekAbsentType == "I")
                {
                    absentType = "Ijin";
                }
                result.AddRange(_generalBll.EachDay(exePlantWorkerAbsenteeism.ProductionDate, exePlantWorkerAbsenteeism.ProductionDate).Select(day => new SuratPeriodeComposite
                {
                    AbsentType = absentType,
                    AlphaDate = day.ToShortDateString(),
                    Status = false,
                    Remark = exePlantWorkerAbsenteeism.Remark,
                    ProductionDate = day.Date
                }));
            }
            return result.Distinct().OrderBy(m => m.ProductionDate).ToList();
        }

        //public List<MstPlantAbsentTypeDTO> GetMstPlantAbsentTypeIsfromProdEntryOrNot(GetMstAbsentTypeInput input)
        //{
        //    var queryFilter = PredicateHelper.True<MstPlantAbsentType>();

        //    if (!string.IsNullOrEmpty(input.AbsentType))
        //        queryFilter = queryFilter.And(m => m.AbsentType == input.AbsentType);

        //    var dbresult = _mstPlantAbsentType.Get(queryFilter);

        //    foreach (var item in dbresult)
        //    {
        //        if (item.ActiveInAbsent == true)

        //    }



        //}
        private void ValidateAbsenteeismFieldNullable(ExePlantWorkerAbsenteeism workerAbsenteeism)
        {
            if (workerAbsenteeism.StartDateAbsent == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.StartDateAbsentNull);
            if (String.IsNullOrEmpty(workerAbsenteeism.EmployeeID))
                throw new BLLException(ExceptionCodes.BLLExceptions.EmployeeIDNull);
            if (String.IsNullOrEmpty(workerAbsenteeism.CreatedBy))
                throw new BLLException(ExceptionCodes.BLLExceptions.CreatedByNull);
            if (String.IsNullOrEmpty(workerAbsenteeism.UpdatedBy))
                throw new BLLException(ExceptionCodes.BLLExceptions.UpdatedByNull);
            if (String.IsNullOrEmpty(workerAbsenteeism.LocationCode))
                throw new BLLException(ExceptionCodes.BLLExceptions.LocationCodeNull);
            if (String.IsNullOrEmpty(workerAbsenteeism.UnitCode))
                throw new BLLException(ExceptionCodes.BLLExceptions.UnitCodeNull);
            if (String.IsNullOrEmpty(workerAbsenteeism.GroupCode))
                throw new BLLException(ExceptionCodes.BLLExceptions.GroupCodeNull);
        }

        public void CheckEblekStatusOnInsertAbsenteeism(GetExePlantWorkerAbsenteeismExcelPieceRateInput input, DateTime newStartAbsentDate, DateTime newEndAbsentDate, string employeeID) 
        {
            // Create combine code Production Entry Code
            var prodEntryCode = string.Format("{0}/{1}/{2}/{3}/{4}/",
                                Enums.CombineCode.EBL.ToString(),
                                input.LocationCode,
                                input.Shift,
                                input.UnitCode,
                                input.Group);

            var existingPlantProductionEntries = _exePlantProductionEntryRepo.Get(m => m.ProductionEntryCode.Contains(prodEntryCode)
                                                                       && m.EmployeeID == employeeID
                                                                       && m.ExePlantProductionEntryVerification.ProductionDate >= newStartAbsentDate.Date
                                                                       && m.ExePlantProductionEntryVerification.ProductionDate <= newEndAbsentDate.Date);

            if (existingPlantProductionEntries.Any()) 
            {
                var listProdEntryCode = existingPlantProductionEntries.Select(c => c.ProductionEntryCode).Distinct().ToList();

                // Get Transaction Log by list distinct Production Entry Code
                var listTranslog = _utilitiesBll.GetLatestTransLogExceptSaveEblekAbsenteeism(listProdEntryCode, Enums.PageName.PlantProductionEntry.ToString());

                foreach (var item in listTranslog)
                {
                    if (item != null)
                    {
                        var productionDate = existingPlantProductionEntries.Where(c => c.ProductionEntryCode == item.TransactionCode).Select(c => c.ExePlantProductionEntryVerification.ProductionDate).FirstOrDefault();
                        var translogRelease = _utilitiesBll.GetLatestActionTransLogExceptSave(item.TransactionCode, Enums.PageName.EblekReleaseApproval.ToString());
                        if (translogRelease != null) {
                            if (translogRelease.CreatedDate < item.CreatedDate) {
                                throw new BLLException(ExceptionCodes.BLLExceptions.EblekSubmitted, "There is <strong> Already Submitted Production Entry for Employee </strong>" + employeeID + "<strong> in " + productionDate.ToShortDateString() + " </strong>");
                            }
                        }
                        else {
                            if (item.UtilFlow.UtilFunction.FunctionName == HMS.SKTIS.Core.Enums.ButtonName.Submit.ToString())
                                throw new BLLException(ExceptionCodes.BLLExceptions.EblekSubmitted, "There is <strong> Already Submitted Production Entry for Employee </strong>" + employeeID + "<strong> in " + productionDate.ToShortDateString() + " </strong>");
                        }
                    }
                }
            }
        }

        public void CheckEblekStatusOnEditAbsenteeism(GetExePlantWorkerAbsenteeismExcelPieceRateInput input, DateTime newStartAbsentDate, DateTime oldStartDateAbsent, DateTime newEndAbsentDate, DateTime oldEndAbsentDate, string employeeID)
        {
            if (newStartAbsentDate == oldStartDateAbsent && newEndAbsentDate == oldEndAbsentDate)
            {
                CheckEblekStatusOnInsertAbsenteeism(input, newStartAbsentDate, newEndAbsentDate, employeeID);
            }
            else if (newStartAbsentDate.Date != oldStartDateAbsent.Date && newEndAbsentDate.Date != oldEndAbsentDate.Date) 
            {
                CheckEblekStatusOnInsertAbsenteeism(input, newStartAbsentDate, oldStartDateAbsent, employeeID);
            }
            else if (newStartAbsentDate.Date != oldStartDateAbsent.Date)
            {
                if (newStartAbsentDate > oldStartDateAbsent) 
                    CheckEblekStatusOnInsertAbsenteeism(input, oldStartDateAbsent, newStartAbsentDate, employeeID);
                else if (newStartAbsentDate < oldStartDateAbsent) 
                    CheckEblekStatusOnInsertAbsenteeism(input, newStartAbsentDate, oldStartDateAbsent, employeeID);
            }
            else if (newEndAbsentDate.Date != oldEndAbsentDate.Date)
            {
                newEndAbsentDate = newEndAbsentDate.AddDays(1);
                if (newEndAbsentDate > oldEndAbsentDate)
                    CheckEblekStatusOnInsertAbsenteeism(input, oldEndAbsentDate, newEndAbsentDate, employeeID);
                else if (newEndAbsentDate < oldEndAbsentDate)
                    CheckEblekStatusOnInsertAbsenteeism(input, newEndAbsentDate, oldEndAbsentDate, employeeID);
            }
        }

        private string CheckEblekStatusOnGetAbsenteeism(ExePlantWorkerAbsenteeismViewDTO dto) 
        {
            var result = "LOCKED";

            using (SKTISEntities context = new SKTISEntities())
            {
                var dateNow = DateTime.Now.Date;
                var closingPayrollCurrentWeek = DateTime.Now.Date;
                var closingPayroll2WeeksBe4Today = DateTime.Now.Date;
                
                closingPayrollCurrentWeek = context.MstClosingPayrolls.Where(c => c.ClosingDate < dateNow).Max(c => c.ClosingDate).AddDays(1);
                
                if(closingPayrollCurrentWeek != null)
                    closingPayroll2WeeksBe4Today = context.MstClosingPayrolls.Where(c => c.ClosingDate < closingPayrollCurrentWeek).Max(c => c.ClosingDate).AddDays(1);

                var listProductionEntryCode = context.ExePlantProductionEntries.Where(c => c.EmployeeID == dto.EmployeeID 
                                                                                && c.ExePlantProductionEntryVerification.ProductionDate >= closingPayroll2WeeksBe4Today
                                                                                && c.ExePlantProductionEntryVerification.ProductionDate <= closingPayrollCurrentWeek
                                                                                && c.ExePlantProductionEntryVerification.LocationCode == dto.LocationCode
                                                                                && c.ExePlantProductionEntryVerification.UnitCode == dto.UnitCode
                                                                                && c.ExePlantProductionEntryVerification.GroupCode == dto.GroupCode
                                                                            ).Select(c => c.ProductionEntryCode).Distinct().ToList();

                foreach (var entryCode in listProductionEntryCode)
	            {
		            // Get Transaction Log by Release Approval
                    var translogRelease = _utilitiesBll.GetLatestActionTransLogExceptSave(entryCode, Enums.PageName.EblekReleaseApproval.ToString());

                    if (translogRelease != null)
                    {
                       // Get Transaction Log Production Entry
                        var translogProdEntry = _utilitiesBll.GetLatestActionTransLogExceptSave(entryCode, Enums.PageName.PlantProductionEntry.ToString());
                        if(translogProdEntry != null)
                        {
                            if (translogRelease.CreatedDate > translogProdEntry.CreatedDate)
                            {
                                var isEblekReleaseApproved = translogRelease.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString();
                                if (isEblekReleaseApproved) result = "UNLOCKED";
                            }
                        }
                        
                    }
	            }

            }

            //var existingPlantProductionEntries = _exePlantProductionEntryRepo.Get(m => m.EmployeeID == dto.EmployeeID
            //                                                           && m.ExePlantProductionEntryVerification.ProductionDate >= dto.StartDateAbsent.Date
            //                                                           && m.ExePlantProductionEntryVerification.ProductionDate <= dto.EndDateAbsent.Date);

            //if (existingPlantProductionEntries.Any())
            //{
            //    var listProdEntryCode = existingPlantProductionEntries.Select(c => c.ProductionEntryCode).Distinct().ToList();

            //    // Get Transaction Log Production Entry
            //    var listTranslogProdEntry = _utilitiesBll.GetLatestTransLogExceptSaveEblekAbsenteeism(listProdEntryCode, Enums.PageName.PlantProductionEntry.ToString());

            //    // Get Transaction Log by Release Approval
            //    var listTranslogRelease = _utilitiesBll.GetLatestTransLogExceptSaveEblekAbsenteeism(listProdEntryCode, Enums.PageName.EblekReleaseApproval.ToString());

            //    foreach (var item in listTranslogProdEntry)
            //    {
            //        var isSubmitted = item.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
            //        if (isSubmitted)
            //        {
            //            var release = listTranslogRelease.Where(c => c.TransactionCode == item.TransactionCode).OrderByDescending(c => c.CreatedDate).FirstOrDefault();
            //            if (release != null)
            //            {
            //                if (release.CreatedDate > item.CreatedDate)
            //                {
            //                    var isEblekReleaseApproved = release.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString();
            //                    if (isEblekReleaseApproved) result = "UNLOCKED";
            //                }
            //            }
            //        }
            //    }
            //}

            return result;
        }

        public ExePlantWorkerAbsenteeismDTO InsertWorkerAbsenteeism_SP(ExePlantWorkerAbsenteeismDTO workerAbsenteeism)
        {
            //validate insert
            ValidateInsertWorkerAbsenteeism(workerAbsenteeism);

            if (workerAbsenteeism.GroupCode == null) workerAbsenteeism.GroupCode = _mstPlantEmpJobsdataAcvRepo.GetByID(workerAbsenteeism.EmployeeID).GroupCode;

            var dbExePlantWorkerAbsenteeism = Mapper.Map<ExePlantWorkerAbsenteeism>(workerAbsenteeism);

            dbExePlantWorkerAbsenteeism.CreatedDate = DateTime.Now;
            dbExePlantWorkerAbsenteeism.UpdatedDate = DateTime.Now;

            if (dbExePlantWorkerAbsenteeism.EndDateAbsent.Date < dbExePlantWorkerAbsenteeism.StartDateAbsent.Date) 
                throw new BLLException(ExceptionCodes.BLLExceptions.EndDateLessThanStartDate);
            
            using (SKTISEntities context = new SKTISEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.INSERT_WORKER_ABSENTEEISM
                        (
                            dbExePlantWorkerAbsenteeism.StartDateAbsent,
                            dbExePlantWorkerAbsenteeism.EmployeeID,
                            dbExePlantWorkerAbsenteeism.AbsentType,
                            dbExePlantWorkerAbsenteeism.EndDateAbsent,
                            dbExePlantWorkerAbsenteeism.SktAbsentCode,
                            dbExePlantWorkerAbsenteeism.PayrollAbsentCode,
                            dbExePlantWorkerAbsenteeism.ePaf,
                            dbExePlantWorkerAbsenteeism.Attachment,
                            dbExePlantWorkerAbsenteeism.AttachmentPath,
                            dbExePlantWorkerAbsenteeism.CreatedDate,
                            dbExePlantWorkerAbsenteeism.CreatedBy,
                            dbExePlantWorkerAbsenteeism.UpdatedDate,
                            dbExePlantWorkerAbsenteeism.UpdatedBy,
                            dbExePlantWorkerAbsenteeism.EmployeeNumber,
                            dbExePlantWorkerAbsenteeism.LocationCode,
                            dbExePlantWorkerAbsenteeism.UnitCode,
                            dbExePlantWorkerAbsenteeism.GroupCode,
                            dbExePlantWorkerAbsenteeism.TransactionDate,
                            dbExePlantWorkerAbsenteeism.Shift
                        );

                        context.SaveChanges();

                        transaction.Commit();

                        return Mapper.Map<ExePlantWorkerAbsenteeismDTO>(dbExePlantWorkerAbsenteeism);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public ExePlantWorkerAbsenteeismDTO UpdateWorkerAbsenteeism_SP(ExePlantWorkerAbsenteeismDTO workerAbsenteeism)
        {
            var dbExePlantWorkerAbsenteeism = _exePlantWorkerAbsenteesimRepo.GetByID(workerAbsenteeism.OldValueStartDateAbsent, workerAbsenteeism.OldValueEmployeeID, workerAbsenteeism.OldValueShift);

            //ValidateEndDatePlantWorkerAbsenteeism(workerAbsenteeism);

            //ValidateDateRangePlantWorkerAbsenteeism(workerAbsenteeism);

            if (dbExePlantWorkerAbsenteeism == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            // Check absent type must be active in absenteeism from master
            if (!String.IsNullOrEmpty(workerAbsenteeism.AbsentType))
            {
                var mstAbsentType = _masterDataBll.GetMstPlantAbsentTypeById(workerAbsenteeism.AbsentType);
                if (mstAbsentType == null) return workerAbsenteeism;
                else
                {
                    var activeInAbsent = mstAbsentType.ActiveInAbsent;
                    if (activeInAbsent == null) return workerAbsenteeism;
                    if (!activeInAbsent.Value) return workerAbsenteeism;
                }
            }

            using (SKTISEntities context = new SKTISEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Remove absenteeism when Absent Type is empty or null
                        if (string.IsNullOrEmpty(workerAbsenteeism.AbsentType))
                        {
                            context.DELETE_WORKER_ABSENTEEISM
                            (
                                dbExePlantWorkerAbsenteeism.StartDateAbsent,
                                dbExePlantWorkerAbsenteeism.EmployeeID,
                                dbExePlantWorkerAbsenteeism.AbsentType,
                                dbExePlantWorkerAbsenteeism.EndDateAbsent,
                                dbExePlantWorkerAbsenteeism.SktAbsentCode,
                                dbExePlantWorkerAbsenteeism.PayrollAbsentCode,
                                dbExePlantWorkerAbsenteeism.ePaf,
                                dbExePlantWorkerAbsenteeism.Attachment,
                                dbExePlantWorkerAbsenteeism.AttachmentPath,
                                dbExePlantWorkerAbsenteeism.CreatedDate,
                                dbExePlantWorkerAbsenteeism.CreatedBy,
                                dbExePlantWorkerAbsenteeism.UpdatedDate,
                                dbExePlantWorkerAbsenteeism.UpdatedBy,
                                dbExePlantWorkerAbsenteeism.EmployeeNumber,
                                dbExePlantWorkerAbsenteeism.LocationCode,
                                dbExePlantWorkerAbsenteeism.UnitCode,
                                dbExePlantWorkerAbsenteeism.GroupCode,
                                dbExePlantWorkerAbsenteeism.TransactionDate,
                                dbExePlantWorkerAbsenteeism.Shift
                            );

                            context.SaveChanges();
                        }
                        else
                        {
                            context.DELETE_WORKER_ABSENTEEISM
                            (
                                dbExePlantWorkerAbsenteeism.StartDateAbsent,
                                dbExePlantWorkerAbsenteeism.EmployeeID,
                                dbExePlantWorkerAbsenteeism.AbsentType,
                                dbExePlantWorkerAbsenteeism.EndDateAbsent,
                                dbExePlantWorkerAbsenteeism.SktAbsentCode,
                                dbExePlantWorkerAbsenteeism.PayrollAbsentCode,
                                dbExePlantWorkerAbsenteeism.ePaf,
                                dbExePlantWorkerAbsenteeism.Attachment,
                                dbExePlantWorkerAbsenteeism.AttachmentPath,
                                dbExePlantWorkerAbsenteeism.CreatedDate,
                                dbExePlantWorkerAbsenteeism.CreatedBy,
                                dbExePlantWorkerAbsenteeism.UpdatedDate,
                                dbExePlantWorkerAbsenteeism.UpdatedBy,
                                dbExePlantWorkerAbsenteeism.EmployeeNumber,
                                dbExePlantWorkerAbsenteeism.LocationCode,
                                dbExePlantWorkerAbsenteeism.UnitCode,
                                dbExePlantWorkerAbsenteeism.GroupCode,
                                dbExePlantWorkerAbsenteeism.TransactionDate,
                                dbExePlantWorkerAbsenteeism.Shift
                            );

                            context.SaveChanges();

                            context.INSERT_WORKER_ABSENTEEISM
                            (
                                workerAbsenteeism.StartDateAbsent,
                                workerAbsenteeism.EmployeeID,
                                workerAbsenteeism.AbsentType,
                                workerAbsenteeism.EndDateAbsent,
                                workerAbsenteeism.SktAbsentCode,
                                workerAbsenteeism.PayrollAbsentCode,
                                workerAbsenteeism.ePaf,
                                workerAbsenteeism.Attachment,
                                workerAbsenteeism.AttachmentPath,
                                dbExePlantWorkerAbsenteeism.CreatedDate,
                                dbExePlantWorkerAbsenteeism.CreatedBy,
                                workerAbsenteeism.UpdatedDate,
                                workerAbsenteeism.UpdatedBy,
                                workerAbsenteeism.EmployeeNumber,
                                workerAbsenteeism.LocationCode,
                                workerAbsenteeism.UnitCode,
                                workerAbsenteeism.GroupCode,
                                workerAbsenteeism.TransactionDate,
                                workerAbsenteeism.Shift
                            );

                            context.SaveChanges();
                        }

                        transaction.Commit();

                        return Mapper.Map<ExePlantWorkerAbsenteeismDTO>(dbExePlantWorkerAbsenteeism);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Production Entry
        public List<ExePlantProductionEntryDTO> GetPlantProductionEntrys(GetExePlantProductionEntryInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntry>();

            var productionEntryCode = EnumHelper.GetDescription(Core.Enums.CombineCode.EBL) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.UnitCode + "/&groupCode&/"
                //+ input.Group + "/"
                                      + input.Brand;

            //if (input.FilterType == "Year")
            productionEntryCode += "/" + input.Year;
            //if (input.FilterType == "Week")
            productionEntryCode += "/" + input.Week;

            var prodEntryCode = "";

            if (input.Date.HasValue)
            {
                productionEntryCode += "/&DAY_OF_WEEK&";
                queryFilter = queryFilter.And(m => m.ExePlantProductionEntryVerification.ProductionDate == input.Date);
                var dayNum = ((int)input.Date.Value.DayOfWeek == 0) ? 7 : (int)input.Date.Value.DayOfWeek;
                prodEntryCode = productionEntryCode.Replace("&DAY_OF_WEEK&", dayNum.ToString());
            }

            // Get Data from Production Entry
            prodEntryCode = string.IsNullOrEmpty(prodEntryCode) ? productionEntryCode.Replace("&groupCode&", input.Group) : prodEntryCode.Replace("&groupCode&", input.Group);

            queryFilter = queryFilter.And(m => m.ProductionEntryCode.Contains(prodEntryCode));

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantProductionEntry>();

            var dbResult = _exePlantProductionEntryRepo.Get(queryFilter, orderByFilter);
            var result = Mapper.Map<List<ExePlantProductionEntryDTO>>(dbResult);


            var dbWorkerAssigmnt = _exePlantWorkerAssignmentRepo.Get(m => m.DestinationLocationCode == input.LocationCode && m.DestinationUnitCode == input.UnitCode && m.DestinationProcessGroup == input.ProcessGroup && //m.SourceShift.HasValue == input.Shift &&
                         m.DestinationBrandCode == input.Brand && m.DestinationGroupCode == input.Group);

            // If not have dummy datas
            if (dbWorkerAssigmnt == null) return result;
            var listProdEntryFromAssignment = new List<ExePlantProductionEntryDTO>();

            foreach (var exePlantWorkerAssignment in dbWorkerAssigmnt)
            {
                // add DestinationGroupCode
                prodEntryCode = productionEntryCode.Replace("&groupCode&", exePlantWorkerAssignment.DestinationGroupCode).Replace("/&DAY_OF_WEEK&", "");
                queryFilter = queryFilter.And(m => m.ProductionEntryCode.Contains(prodEntryCode));
                dbResult = _exePlantProductionEntryRepo.Get(queryFilter, orderByFilter);
                listProdEntryFromAssignment.AddRange(dbResult.Select(Mapper.Map<ExePlantProductionEntryDTO>));

                // add DestinationGroupCodeDummy
                prodEntryCode = productionEntryCode.Replace("&groupCode&", exePlantWorkerAssignment.DestinationGroupCodeDummy).Replace("/&DAY_OF_WEEK&", "");
                queryFilter = queryFilter.And(m => m.ProductionEntryCode.Contains(prodEntryCode));
                dbResult = _exePlantProductionEntryRepo.Get(queryFilter, orderByFilter);
                listProdEntryFromAssignment.AddRange(dbResult.Select(Mapper.Map<ExePlantProductionEntryDTO>));
            }

            foreach (var prodEntryDto in listProdEntryFromAssignment)
            {
                var emp = _mstPlantEmpJobsdataAcvRepo.GetByID(prodEntryDto.EmployeeID);
                prodEntryDto.EmployeeName = emp == null ? "" : emp.EmployeeNumber + " - " + emp.EmployeeName;
            }

            result.AddRange(listProdEntryFromAssignment);
            //result.ForEach(m => m.AlreadySubmitFromEblekVer = _utilitiesBll.CheckDataAlreadySumbit(m.ProductionEntryCode, Enums.PageName.PlantProductionEntry.ToString()));
            //result.ForEach(m => m.AlreadySubmitFromEblekVer = _utilitiesBll.CheckDataAlreadySumbit(m.ProductionEntryCode));
            return result;
            //return result.ForEach(m => m.AlreadySubmitFromEblekVer = _utilitiesBll.CheckDataAlreadySumbit(m.ProductionEntryCode));

        }

        public List<ExePlantProductionEntryDTO> GetPlantProductionEntrysTuning(GetExePlantProductionEntryInput input) 
        {
            DateTime date = input.Date.Value;
            int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
            var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.UnitCode + "/"
                                      + input.Group + "/"
                                      + input.Brand + "/"
                                      + input.Year + "/"
                                      + input.Week + "/"
                                      + day;

            var minValue = GetMinimumValueForActualProdEntry(input);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantProductionEntry>();

            // Get List Plant Production Entry
            var dbListProdEntryResult = _exePlantProductionEntryRepo.Get(c => c.ProductionEntryCode == productionEntryCode, orderByFilter);
            var listProdEntryDTO = Mapper.Map<List<ExePlantProductionEntryDTO>>(dbListProdEntryResult);

            //get emp name from mst emp job acv
            foreach (var entry in listProdEntryDTO)
            {
                var mstEmployee = _mstPlantEmpJobsdataAllRepo.GetByID(entry.EmployeeID);
                entry.EmployeeName = mstEmployee == null ? "" : entry.EmployeeNumber.Substring(entry.EmployeeNumber.Length - 2, 2) + " - " + mstEmployee.EmployeeName;
                // Set min value actual minstickperhour
                entry.MinimumValueActual = minValue == null ? 0 : minValue.MinimalStickPerHour == null ? 0 : minValue.MinimalStickPerHour.Value;
                entry.SourceProdActual = 0f;
            }

            // Check Dummy Group from Worker Assignment
            var groupCodeDummy = string.IsNullOrEmpty(input.Group) ? "" : input.Group.Remove(1, 1).Insert(1, "5");
            var productionEntryCodeDummy = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.UnitCode + "/"
                                      + groupCodeDummy + "/"
                                      + input.Brand + "/"
                                      + input.Year + "/"
                                      + input.Week + "/"
                                      + day;

            // Get List Plant Production Entry Dummy
            var dbListProdEntryResultDummy = _exePlantProductionEntryRepo.Get(c => c.ProductionEntryCode == productionEntryCodeDummy, orderByFilter);
            var listProdEntryDTODummy = Mapper.Map<List<ExePlantProductionEntryDTO>>(dbListProdEntryResultDummy);

            foreach (var eblekDummy in listProdEntryDTODummy)
            {
                var mstEmployee = _mstPlantEmpJobsdataAllRepo.GetByID(eblekDummy.EmployeeID);
                eblekDummy.EmployeeName = mstEmployee == null ? "" : eblekDummy.EmployeeNumber + " - " + mstEmployee.EmployeeName;
                // Set min value actual minstickperhour for dummy
                var workerAssignment = _exePlantWorkerAssignmentRepo.Get(c => c.EmployeeID == eblekDummy.EmployeeID && input.Date >= c.StartDate && input.Date <= c.EndDate).FirstOrDefault();
                if (workerAssignment != null)
                {
                    if(workerAssignment.SourceProcessGroup == Enums.Process.Daily.ToString().ToUpper())
                    {
                        eblekDummy.MinimumValueActual = 0;
                        eblekDummy.SourceProdActual = 0f;
                    }
                    else if (workerAssignment.SourceProcessGroup == workerAssignment.DestinationProcessGroup)
                    {
                        var inputSourceEntry = new GetExePlantProductionEntryInput
                        {
                            LocationCode = workerAssignment.SourceLocationCode,
                            UnitCode = workerAssignment.SourceUnitCode,
                            Shift = workerAssignment.SourceShift == null ? "0" : workerAssignment.SourceShift.Value.ToString(),
                            ProcessGroup = workerAssignment.SourceProcessGroup,
                            Brand = workerAssignment.SourceBrandCode,
                            Group = workerAssignment.SourceGroupCode
                        };
                        var minValueDummy = GetMinimumValueForActualProdEntry(inputSourceEntry);
                        eblekDummy.MinimumValueActual = minValueDummy == null ? 0 : minValueDummy.MinimalStickPerHour == null ? 0 : minValueDummy.MinimalStickPerHour.Value;

                        var productionEntryCodeSource = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                      + workerAssignment.SourceLocationCode + "/"
                                      + workerAssignment.SourceShift + "/"
                                      + workerAssignment.SourceUnitCode + "/"
                                      + workerAssignment.SourceGroupCode + "/"
                                      + workerAssignment.SourceBrandCode + "/"
                                      + input.Year + "/"
                                      + input.Week + "/"
                                      + day;
                        var entrySource = _exePlantProductionEntryRepo.GetByID(productionEntryCodeSource, workerAssignment.EmployeeID);
                        eblekDummy.SourceProdActual = entrySource == null ? 0f : entrySource.ProdActual == null ? 0f : entrySource.ProdActual.Value;
                    }
                    else if( workerAssignment.SourceProcessGroup != workerAssignment.DestinationProcessGroup)
                    {
                        eblekDummy.MinimumValueActual = minValue == null ? 0 : minValue.MinimalStickPerHour == null ? 0 : minValue.MinimalStickPerHour.Value;
                        eblekDummy.SourceProdActual = 0f;
                    }
                }
            }

            listProdEntryDTO.AddRange(listProdEntryDTODummy);

            return listProdEntryDTO;
        }

        public ExePlantProductionEntryDTO SaveProductionEntry(ExePlantProductionEntryDTO eblekDto)
        {
            var dbProductionEntry = _exePlantProductionEntryRepo.GetByID(eblekDto.ProductionEntryCode, eblekDto.EmployeeID);
            if (dbProductionEntry == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //Updating this field will create/update entry in Worker Absenteeism
            if(eblekDto.AbsentType != dbProductionEntry.AbsentType)
                UpdateCreateWorkerAbsenteeism(eblekDto, dbProductionEntry);

            //dbProductionEntry.UpdatedDate = DateTime.Now;

            //#2 Entry All Target value, then click Save:
            //  If Target is NULL and Absent Type (is NULL or Tugas Lain Perusahaan, Tugas Lain Organisasi and Multiskill Out) 
            //  then Target will be filled with Capacity
            if (!dbProductionEntry.ProdTarget.HasValue && !dbProductionEntry.ProdActual.HasValue)
            {

                //if ((!eblekDto.ProdTarget.HasValue || eblekDto.ProdTarget == 0) && (string.IsNullOrEmpty(eblekDto.AbsentType)
                //    || eblekDto.AbsentCodeEblek == Enums.SKTAbsentCode.MO.ToString()
                //    || eblekDto.AbsentCodeEblek == Enums.SKTAbsentCode.LO.ToString())
                //    || eblekDto.AbsentCodeEblek == Enums.SKTAbsentCode.LP.ToString())
                //if (!eblekDto.ProdTarget.HasValue || eblekDto.ProdTarget == 0)
                if (!eblekDto.ProdTarget.HasValue)
                    eblekDto.ProdTarget = (float)eblekDto.ProdCapacity;

            }
            //#3 Entry Actual value, then click Save:
            //  If Actual is NULL then Actual will be filled with Target
            else if (dbProductionEntry.ProdTarget.HasValue && !dbProductionEntry.ProdActual.HasValue && eblekDto.SaveType == "actual")
            {
                if (!eblekDto.ProdActual.HasValue)
                    eblekDto.ProdActual = eblekDto.ProdTarget;
            }
            //#4 Entry Absent Type, then Target and Actual value are set to 0/ Zero:
            //  If Actual is not entered, Absent Type that user can choose is all Absent Type, example “Alpa”, etc, then Target and Actual value are set to 0/ Zero
            else if (dbProductionEntry.ProdTarget.HasValue && dbProductionEntry.ProdActual.HasValue)
            {
                if (!eblekDto.ProdActual.HasValue)
                {
                    eblekDto.ProdActual = 0;
                    eblekDto.ProdTarget = 0;
                }
            }

            Mapper.Map(eblekDto, dbProductionEntry);

            dbProductionEntry.AbsentType = eblekDto.AbsentType;
            if(string.IsNullOrEmpty(eblekDto.AbsentType)) dbProductionEntry.StartDateAbsent = null;

            _exePlantProductionEntryRepo.Update(dbProductionEntry);

            try
            {
                //throw (new Exception("error"));
                _uow.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return Mapper.Map<ExePlantProductionEntryDTO>(dbProductionEntry);
        }

        public void UpdateCreateWorkerAbsenteeism(ExePlantProductionEntryDTO dto, ExePlantProductionEntry dbEblek)
        {
            // Check if prod entry already has absent type
            if (!String.IsNullOrEmpty(dbEblek.AbsentType))
            {
                EditWorkerAbsenteeismFromProdEntry(dto, dbEblek);
            }
            else 
            {
                if (dbEblek.AbsentType != dto.AbsentType)
                {
                    InsertWorkerAbsenteeismFromProdEntry(dto);
                }
            }
        }

        private void InsertWorkerAbsenteeismFromProdEntry(ExePlantProductionEntryDTO dto) 
        {
            var employeeAcv = _mstPlantEmpJobsdataAcvRepo.GetByID(dto.EmployeeID);
            var employeeNumber = employeeAcv == null ? null : employeeAcv.EmployeeNumber;

            var eblekCode = dto.ProductionEntryCode.Split('/');

            var shift = Convert.ToInt32(eblekCode[2]);

            // Get absenteeism if exist previous day from start date eblek
            var previousDay = dto.StartDateAbsent.Value.AddDays(-1);

            var absenteeismPreviousDay = _exePlantWorkerAbsenteesimRepo.Get(c =>
                    c.EmployeeID == dto.EmployeeID &&
                    c.Shift == shift &&
                    c.EndDateAbsent == previousDay &&
                    c.AbsentType == dto.AbsentType).FirstOrDefault();

            // Get absenteeism if exist next day from start date eblek
            var nextDay = dto.StartDateAbsent.Value.Date.AddDays(1);
            var absenteeismNextDay = _exePlantWorkerAbsenteesimRepo.Get(c =>
                        c.EmployeeID == dto.EmployeeID &&
                        c.Shift == shift &&
                        c.StartDateAbsent == nextDay &&
                        c.AbsentType == dto.AbsentType).FirstOrDefault();

            // Check if start date eblek is NOT closing payroll/holiday/sunday
            if (!CheckClosingPayrollOrHoliday(dto.StartDateAbsent.Value, eblekCode[1]))
            {
                if (CheckClosingPayrollOrHoliday(previousDay, eblekCode[1]))
                {
                    if (absenteeismNextDay != null)
                    {
                        var newAbsenteeism = CreateObjectWorkerAbsenteeismFromEntry(dto);

                        newAbsenteeism.EndDateAbsent = absenteeismNextDay.EndDateAbsent;
                        ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                        _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                        _exePlantWorkerAbsenteesimRepo.Delete(absenteeismNextDay);
                    }
                    else
                    {
                        var existingData = _exePlantWorkerAbsenteesimRepo.GetByID(dto.StartDateAbsent,
                           dto.EmployeeID, Convert.ToInt32(eblekCode[2]));

                        if (existingData == null)
                        {
                            var newAbsenteeism = CreateObjectWorkerAbsenteeismFromEntry(dto);
                            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                        }
                        else
                        {
                            existingData.AbsentType = dto.AbsentType;
                            existingData.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                            existingData.SktAbsentCode = dto.AbsentCodeEblek;
                            existingData.PayrollAbsentCode = dto.AbsentCodePayroll;
                            existingData.CreatedDate = dto.UpdatedDate;
                            existingData.CreatedBy = dto.UpdatedBy;
                            existingData.UpdatedDate = dto.UpdatedDate;
                            existingData.UpdatedBy = dto.UpdatedBy;
                            existingData.LocationCode = eblekCode[1];
                            existingData.UnitCode = eblekCode[3];
                            existingData.GroupCode = eblekCode[4];
                            existingData.TransactionDate = DateTime.Now.Date;
                            existingData.Shift = Convert.ToInt32(eblekCode[2]);
                            existingData.EmployeeNumber = employeeNumber;

                            ValidateEndDateMustLessThanStartDate(existingData);
                            _exePlantWorkerAbsenteesimRepo.Update(existingData);
                        }
                    }
                }
                //else if (CheckClosingPayrollOrHoliday(nextDay, eblekCode[1]))
                //{
                //    if (absenteeismPreviousDay != null)
                //    {
                //        absenteeismPreviousDay.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                //        absenteeismPreviousDay.CreatedDate = dto.UpdatedDate;
                //        absenteeismPreviousDay.CreatedBy = dto.UpdatedBy;
                //        absenteeismPreviousDay.UpdatedDate = dto.UpdatedDate;
                //        absenteeismPreviousDay.UpdatedBy = dto.UpdatedBy;
                //        absenteeismPreviousDay.LocationCode = eblekCode[1];
                //        absenteeismPreviousDay.UnitCode = eblekCode[3];
                //        absenteeismPreviousDay.GroupCode = eblekCode[4];
                //        absenteeismPreviousDay.TransactionDate = DateTime.Now.Date;

                //        ValidateEndDateMustLessThanStartDate(absenteeismPreviousDay);
                //        _exePlantWorkerAbsenteesimRepo.Update(absenteeismPreviousDay);
                //    }
                //    else
                //    {
                //        var existingData = _exePlantWorkerAbsenteesimRepo.GetByID(dto.StartDateAbsent,
                //            dto.EmployeeID, Convert.ToInt32(eblekCode[2]));

                //        if (existingData == null)
                //        {
                //            var newAbsenteeism = CreateObjectWorkerAbsenteeismFromEntry(dto);

                //            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                //            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                //        }
                //        else
                //        {
                //            existingData.AbsentType = dto.AbsentType;
                //            existingData.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                //            existingData.SktAbsentCode = dto.AbsentCodeEblek;
                //            existingData.PayrollAbsentCode = dto.AbsentCodePayroll;
                //            existingData.CreatedDate = dto.UpdatedDate;
                //            existingData.CreatedBy = dto.UpdatedBy;
                //            existingData.UpdatedDate = dto.UpdatedDate;
                //            existingData.UpdatedBy = dto.UpdatedBy;
                //            existingData.LocationCode = eblekCode[1];
                //            existingData.UnitCode = eblekCode[3];
                //            existingData.GroupCode = eblekCode[4];
                //            existingData.TransactionDate = DateTime.Now.Date;
                //            existingData.Shift = Convert.ToInt32(eblekCode[2]);
                //            existingData.EmployeeNumber = employeeNumber;

                //            ValidateEndDateMustLessThanStartDate(existingData);
                //            _exePlantWorkerAbsenteesimRepo.Update(existingData);
                //        }
                //    }
                //}
                else
                {
                    if (absenteeismPreviousDay != null && absenteeismNextDay != null)
                    {
                        absenteeismPreviousDay.EndDateAbsent = absenteeismNextDay.EndDateAbsent;
                        absenteeismPreviousDay.CreatedDate = dto.UpdatedDate;
                        absenteeismPreviousDay.CreatedBy = dto.UpdatedBy;
                        absenteeismPreviousDay.UpdatedDate = dto.UpdatedDate;
                        absenteeismPreviousDay.UpdatedBy = dto.UpdatedBy;
                        absenteeismPreviousDay.LocationCode = eblekCode[1];
                        absenteeismPreviousDay.UnitCode = eblekCode[3];
                        absenteeismPreviousDay.GroupCode = eblekCode[4];
                        absenteeismPreviousDay.TransactionDate = DateTime.Now.Date;

                        _exePlantWorkerAbsenteesimRepo.Delete(absenteeismNextDay);
                        ValidateEndDateMustLessThanStartDate(absenteeismPreviousDay);
                        _exePlantWorkerAbsenteesimRepo.Update(absenteeismPreviousDay);
                    }
                    else if (absenteeismPreviousDay != null && absenteeismNextDay == null)
                    {
                        absenteeismPreviousDay.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                        absenteeismPreviousDay.CreatedDate = dto.UpdatedDate;
                        absenteeismPreviousDay.CreatedBy = dto.UpdatedBy;
                        absenteeismPreviousDay.UpdatedDate = dto.UpdatedDate;
                        absenteeismPreviousDay.UpdatedBy = dto.UpdatedBy;
                        absenteeismPreviousDay.LocationCode = eblekCode[1];
                        absenteeismPreviousDay.UnitCode = eblekCode[3];
                        absenteeismPreviousDay.GroupCode = eblekCode[4];
                        absenteeismPreviousDay.TransactionDate = DateTime.Now.Date;

                        ValidateEndDateMustLessThanStartDate(absenteeismPreviousDay);
                        _exePlantWorkerAbsenteesimRepo.Update(absenteeismPreviousDay);
                    }
                    else if (absenteeismPreviousDay == null && absenteeismNextDay != null)
                    {
                        var newAbsenteeism = CreateObjectWorkerAbsenteeismFromEntry(dto);

                        newAbsenteeism.EndDateAbsent = absenteeismNextDay.EndDateAbsent.Date;

                        ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                        _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                        _exePlantWorkerAbsenteesimRepo.Delete(absenteeismNextDay);
                    }
                    else
                    {
                        var existingData = _exePlantWorkerAbsenteesimRepo.GetByID(dto.StartDateAbsent,
                            dto.EmployeeID, Convert.ToInt32(eblekCode[2]));

                        if (existingData == null)
                        {
                            var newAbsenteeism = CreateObjectWorkerAbsenteeismFromEntry(dto);

                            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                        }
                        else
                        {
                            existingData.AbsentType = dto.AbsentType;
                            existingData.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                            existingData.SktAbsentCode = dto.AbsentCodeEblek;
                            existingData.PayrollAbsentCode = dto.AbsentCodePayroll;
                            existingData.CreatedDate = dto.UpdatedDate;
                            existingData.CreatedBy = dto.UpdatedBy;
                            existingData.UpdatedDate = dto.UpdatedDate;
                            existingData.UpdatedBy = dto.UpdatedBy;
                            existingData.LocationCode = eblekCode[1];
                            existingData.UnitCode = eblekCode[3];
                            existingData.GroupCode = eblekCode[4];
                            existingData.TransactionDate = DateTime.Now.Date;
                            existingData.Shift = Convert.ToInt32(eblekCode[2]);
                            existingData.EmployeeNumber = employeeNumber;

                            ValidateEndDateMustLessThanStartDate(existingData);
                            _exePlantWorkerAbsenteesimRepo.Update(existingData);
                        }
                    }
                }
            }
            else
            {
                if (absenteeismPreviousDay != null)
                {
                    absenteeismPreviousDay.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                    absenteeismPreviousDay.CreatedDate = dto.UpdatedDate;
                    absenteeismPreviousDay.CreatedBy = dto.UpdatedBy;
                    absenteeismPreviousDay.UpdatedDate = dto.UpdatedDate;
                    absenteeismPreviousDay.UpdatedBy = dto.UpdatedBy;
                    absenteeismPreviousDay.LocationCode = eblekCode[1];
                    absenteeismPreviousDay.UnitCode = eblekCode[3];
                    absenteeismPreviousDay.GroupCode = eblekCode[4];
                    absenteeismPreviousDay.TransactionDate = DateTime.Now.Date;

                    ValidateEndDateMustLessThanStartDate(absenteeismPreviousDay);
                    _exePlantWorkerAbsenteesimRepo.Update(absenteeismPreviousDay);
                }
                else
                {
                    var existingData = _exePlantWorkerAbsenteesimRepo.GetByID(dto.StartDateAbsent,
                           dto.EmployeeID, Convert.ToInt32(eblekCode[2]));

                    if (existingData == null)
                    {
                        var newAbsenteeism = CreateObjectWorkerAbsenteeismFromEntry(dto);

                        ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                        _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                    }
                    else
                    {
                        existingData.AbsentType = dto.AbsentType;
                        existingData.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                        existingData.SktAbsentCode = dto.AbsentCodeEblek;
                        existingData.PayrollAbsentCode = dto.AbsentCodePayroll;
                        existingData.CreatedDate = dto.UpdatedDate;
                        existingData.CreatedBy = dto.UpdatedBy;
                        existingData.UpdatedDate = dto.UpdatedDate;
                        existingData.UpdatedBy = dto.UpdatedBy;
                        existingData.LocationCode = eblekCode[1];
                        existingData.UnitCode = eblekCode[3];
                        existingData.GroupCode = eblekCode[4];
                        existingData.TransactionDate = DateTime.Now.Date;
                        existingData.Shift = Convert.ToInt32(eblekCode[2]);
                        existingData.EmployeeNumber = employeeNumber;

                        ValidateEndDateMustLessThanStartDate(existingData);
                        _exePlantWorkerAbsenteesimRepo.Update(existingData);
                    }
                }
            }
        }

        private void EditWorkerAbsenteeismFromProdEntry(ExePlantProductionEntryDTO dto, ExePlantProductionEntry dbEblek) 
        {
            var employeeAcv = _mstPlantEmpJobsdataAcvRepo.GetByID(dto.EmployeeID);
            var employeeNumber = employeeAcv == null ? null : employeeAcv.EmployeeNumber;

            var eblekCode = dto.ProductionEntryCode.Split('/');

            var shift = Convert.ToInt32(eblekCode[2]);

            // Assign absent type to null
            if (String.IsNullOrEmpty(dto.AbsentType))
            {
                // Get current absenteeism by dto prod entry
                var currAbsenteeism = _exePlantWorkerAbsenteesimRepo.Get(c =>
                                                c.EmployeeID == dto.EmployeeID &&
                                                c.Shift == shift &&
                                                dto.StartDateAbsent >= c.StartDateAbsent &&
                                                dto.StartDateAbsent <= c.EndDateAbsent).FirstOrDefault();

                if (currAbsenteeism != null)
                {
                    var newAbsenteeism = new ExePlantWorkerAbsenteeism();
                    newAbsenteeism.EmployeeID = currAbsenteeism.EmployeeID;
                    newAbsenteeism.AbsentType = currAbsenteeism.AbsentType;
                    newAbsenteeism.SktAbsentCode = currAbsenteeism.SktAbsentCode;
                    newAbsenteeism.PayrollAbsentCode = currAbsenteeism.SktAbsentCode;
                    newAbsenteeism.CreatedDate = currAbsenteeism.UpdatedDate;
                    newAbsenteeism.CreatedBy = currAbsenteeism.UpdatedBy;
                    newAbsenteeism.UpdatedDate = DateTime.Now;
                    newAbsenteeism.UpdatedBy = dto.UpdatedBy;
                    newAbsenteeism.LocationCode = currAbsenteeism.LocationCode;
                    newAbsenteeism.UnitCode = currAbsenteeism.UnitCode;
                    newAbsenteeism.GroupCode = currAbsenteeism.GroupCode;
                    newAbsenteeism.TransactionDate = DateTime.Now.Date;
                    newAbsenteeism.Shift = currAbsenteeism.Shift;
                    newAbsenteeism.EmployeeNumber = currAbsenteeism.EmployeeNumber;

                    if (currAbsenteeism.StartDateAbsent.Date == dto.StartDateAbsent.Value.Date)
                    {
                        // Check if current absenteeism only one day or not
                        if ((currAbsenteeism.EndDateAbsent.Date - currAbsenteeism.StartDateAbsent.Date).Days == 0)
                        {
                            _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                        }
                        else
                        {
                            newAbsenteeism.StartDateAbsent = currAbsenteeism.StartDateAbsent.AddDays(1);
                            newAbsenteeism.EndDateAbsent = currAbsenteeism.EndDateAbsent.Date;

                            _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                        }
                    }
                    else if (currAbsenteeism.EndDateAbsent.Date == dto.StartDateAbsent.Value.Date)
                    {
                        // Check if current absenteeism only one day or not
                        if ((currAbsenteeism.EndDateAbsent.Date - currAbsenteeism.StartDateAbsent.Date).Days == 0)
                        {
                            _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                        }
                        else
                        {
                            newAbsenteeism.StartDateAbsent = currAbsenteeism.StartDateAbsent;
                            newAbsenteeism.EndDateAbsent = currAbsenteeism.EndDateAbsent.AddDays(-1);

                            _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                        }
                    }
                    else
                    {
                        // Split current absenteeism into 2 datas
                        var newAbsenteeismSplit1 = new ExePlantWorkerAbsenteeism();
                        newAbsenteeismSplit1.StartDateAbsent = currAbsenteeism.StartDateAbsent;
                        newAbsenteeismSplit1.EmployeeID = currAbsenteeism.EmployeeID;
                        newAbsenteeismSplit1.AbsentType = currAbsenteeism.AbsentType;
                        newAbsenteeismSplit1.EndDateAbsent = dto.StartDateAbsent.Value.AddDays(-1);
                        newAbsenteeismSplit1.SktAbsentCode = currAbsenteeism.SktAbsentCode;
                        newAbsenteeismSplit1.PayrollAbsentCode = currAbsenteeism.PayrollAbsentCode;
                        newAbsenteeismSplit1.CreatedDate = currAbsenteeism.UpdatedDate;
                        newAbsenteeismSplit1.CreatedBy = currAbsenteeism.UpdatedBy;
                        newAbsenteeismSplit1.UpdatedDate = currAbsenteeism.UpdatedDate;
                        newAbsenteeismSplit1.UpdatedBy = currAbsenteeism.UpdatedBy;
                        newAbsenteeismSplit1.LocationCode = currAbsenteeism.LocationCode;
                        newAbsenteeismSplit1.UnitCode = currAbsenteeism.UnitCode;
                        newAbsenteeismSplit1.GroupCode = currAbsenteeism.GroupCode;
                        newAbsenteeismSplit1.TransactionDate = DateTime.Now.Date;
                        newAbsenteeismSplit1.Shift = currAbsenteeism.Shift;
                        newAbsenteeismSplit1.EmployeeNumber = currAbsenteeism.EmployeeNumber;

                        var newAbsenteeismSplit2 = new ExePlantWorkerAbsenteeism();
                        newAbsenteeismSplit2.StartDateAbsent = dto.StartDateAbsent.Value.AddDays(1);
                        newAbsenteeismSplit2.EmployeeID = currAbsenteeism.EmployeeID;
                        newAbsenteeismSplit2.AbsentType = currAbsenteeism.AbsentType;
                        newAbsenteeismSplit2.EndDateAbsent = currAbsenteeism.EndDateAbsent;
                        newAbsenteeismSplit2.SktAbsentCode = currAbsenteeism.SktAbsentCode;
                        newAbsenteeismSplit2.PayrollAbsentCode = currAbsenteeism.PayrollAbsentCode;
                        newAbsenteeismSplit2.CreatedDate = currAbsenteeism.UpdatedDate;
                        newAbsenteeismSplit2.CreatedBy = currAbsenteeism.UpdatedBy;
                        newAbsenteeismSplit2.UpdatedDate = currAbsenteeism.UpdatedDate;
                        newAbsenteeismSplit2.UpdatedBy = currAbsenteeism.UpdatedBy;
                        newAbsenteeismSplit2.LocationCode = currAbsenteeism.LocationCode;
                        newAbsenteeismSplit2.UnitCode = currAbsenteeism.UnitCode;
                        newAbsenteeismSplit2.GroupCode = currAbsenteeism.GroupCode;
                        newAbsenteeismSplit2.TransactionDate = DateTime.Now.Date;
                        newAbsenteeismSplit2.Shift = currAbsenteeism.Shift;
                        newAbsenteeismSplit2.EmployeeNumber = currAbsenteeism.EmployeeNumber;

                        _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                        ValidateEndDateMustLessThanStartDate(newAbsenteeismSplit1);
                        _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeismSplit1);
                        ValidateEndDateMustLessThanStartDate(newAbsenteeismSplit2);
                        _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeismSplit2);
                    }
                }
            }
            else
            {
                if (dbEblek.AbsentType != dto.AbsentType)
                {
                    // Get current absenteeism by dto prod entry
                    var currAbsenteeism = _exePlantWorkerAbsenteesimRepo.Get(c =>
                                                    c.EmployeeID == dto.EmployeeID &&
                                                    c.Shift == shift &&
                                                    dto.StartDateAbsent >= c.StartDateAbsent &&
                                                    dto.StartDateAbsent <= c.EndDateAbsent).FirstOrDefault();

                    if (currAbsenteeism != null)
                    {
                        var newAbsenteeism = CreateObjectWorkerAbsenteeismFromEntry(dto);

                        if (currAbsenteeism.StartDateAbsent.Date == dto.StartDateAbsent.Value.Date)
                        {
                            // Check if current absenteeism only one day or not
                            if ((currAbsenteeism.EndDateAbsent.Date - currAbsenteeism.StartDateAbsent.Date).Days == 0)
                            {
                                _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                                //_exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);


                                // Get absenteeism if exist previous day from start date eblek
                                var previousDay = newAbsenteeism.StartDateAbsent.AddDays(-1);
                                var absenteeismPreviousDay = _exePlantWorkerAbsenteesimRepo.Get(c =>
                                        c.EmployeeID == newAbsenteeism.EmployeeID &&
                                        c.Shift == shift &&
                                        c.EndDateAbsent == previousDay &&
                                        c.AbsentType == newAbsenteeism.AbsentType).FirstOrDefault();

                                // Get absenteeism if exist next day from start date eblek
                                var nextDay = newAbsenteeism.StartDateAbsent.Date.AddDays(1);
                                var absenteeismNextDay = _exePlantWorkerAbsenteesimRepo.Get(c =>
                                            c.EmployeeID == newAbsenteeism.EmployeeID &&
                                            c.Shift == shift &&
                                            c.StartDateAbsent == nextDay &&
                                            c.AbsentType == newAbsenteeism.AbsentType).FirstOrDefault();

                                // Check if start date eblek is NOT closing payroll/holiday/sunday
                                if (!CheckClosingPayrollOrHoliday(newAbsenteeism.StartDateAbsent, eblekCode[1]))
                                {
                                    if (CheckClosingPayrollOrHoliday(previousDay, eblekCode[1]))
                                    {
                                        if (absenteeismNextDay != null)
                                        {
                                            newAbsenteeism.EndDateAbsent = absenteeismNextDay.EndDateAbsent;

                                            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                                            _exePlantWorkerAbsenteesimRepo.Delete(absenteeismNextDay);
                                        }
                                        else
                                        {
                                            var existingData = _exePlantWorkerAbsenteesimRepo.GetByID(dto.StartDateAbsent,
                                               dto.EmployeeID, Convert.ToInt32(eblekCode[2]));

                                            if (existingData == null)
                                            {
                                                ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                                                _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                                            }
                                            else
                                            {
                                                existingData.AbsentType = dto.AbsentType;
                                                existingData.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                                                existingData.SktAbsentCode = dto.AbsentCodeEblek;
                                                existingData.PayrollAbsentCode = dto.AbsentCodePayroll;
                                                existingData.CreatedDate = dto.UpdatedDate;
                                                existingData.CreatedBy = dto.UpdatedBy;
                                                existingData.UpdatedDate = dto.UpdatedDate;
                                                existingData.UpdatedBy = dto.UpdatedBy;
                                                existingData.LocationCode = eblekCode[1];
                                                existingData.UnitCode = eblekCode[3];
                                                existingData.GroupCode = eblekCode[4];
                                                existingData.TransactionDate = DateTime.Now.Date;
                                                existingData.Shift = Convert.ToInt32(eblekCode[2]);
                                                existingData.EmployeeNumber = employeeNumber;

                                                ValidateEndDateMustLessThanStartDate(existingData);
                                                _exePlantWorkerAbsenteesimRepo.Update(existingData);
                                            }
                                        }
                                    }
                                    else if (CheckClosingPayrollOrHoliday(nextDay, eblekCode[1]))
                                    {
                                        if (absenteeismPreviousDay != null)
                                        {
                                            absenteeismPreviousDay.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                                            absenteeismPreviousDay.CreatedDate = dto.UpdatedDate;
                                            absenteeismPreviousDay.CreatedBy = dto.UpdatedBy;
                                            absenteeismPreviousDay.UpdatedDate = dto.UpdatedDate;
                                            absenteeismPreviousDay.UpdatedBy = dto.UpdatedBy;
                                            absenteeismPreviousDay.LocationCode = eblekCode[1];
                                            absenteeismPreviousDay.UnitCode = eblekCode[3];
                                            absenteeismPreviousDay.GroupCode = eblekCode[4];
                                            absenteeismPreviousDay.TransactionDate = DateTime.Now.Date;

                                            ValidateEndDateMustLessThanStartDate(absenteeismPreviousDay);
                                            _exePlantWorkerAbsenteesimRepo.Update(absenteeismPreviousDay);
                                        }
                                        else
                                        {
                                            var existingData = _exePlantWorkerAbsenteesimRepo.GetByID(dto.StartDateAbsent,
                                                dto.EmployeeID, Convert.ToInt32(eblekCode[2]));

                                            if (existingData == null)
                                            {
                                                _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                                            }
                                            else
                                            {
                                                existingData.AbsentType = dto.AbsentType;
                                                existingData.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                                                existingData.SktAbsentCode = dto.AbsentCodeEblek;
                                                existingData.PayrollAbsentCode = dto.AbsentCodePayroll;
                                                existingData.CreatedDate = dto.UpdatedDate;
                                                existingData.CreatedBy = dto.UpdatedBy;
                                                existingData.UpdatedDate = dto.UpdatedDate;
                                                existingData.UpdatedBy = dto.UpdatedBy;
                                                existingData.LocationCode = eblekCode[1];
                                                existingData.UnitCode = eblekCode[3];
                                                existingData.GroupCode = eblekCode[4];
                                                existingData.TransactionDate = DateTime.Now.Date;
                                                existingData.Shift = Convert.ToInt32(eblekCode[2]);
                                                existingData.EmployeeNumber = employeeNumber;

                                                ValidateEndDateMustLessThanStartDate(existingData);
                                                _exePlantWorkerAbsenteesimRepo.Update(existingData);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (absenteeismPreviousDay != null && absenteeismNextDay != null)
                                        {
                                            absenteeismPreviousDay.EndDateAbsent = absenteeismNextDay.EndDateAbsent;
                                            absenteeismPreviousDay.CreatedDate = dto.UpdatedDate;
                                            absenteeismPreviousDay.CreatedBy = dto.UpdatedBy;
                                            absenteeismPreviousDay.UpdatedDate = dto.UpdatedDate;
                                            absenteeismPreviousDay.UpdatedBy = dto.UpdatedBy;
                                            absenteeismPreviousDay.LocationCode = eblekCode[1];
                                            absenteeismPreviousDay.UnitCode = eblekCode[3];
                                            absenteeismPreviousDay.GroupCode = eblekCode[4];
                                            absenteeismPreviousDay.TransactionDate = DateTime.Now.Date;

                                            _exePlantWorkerAbsenteesimRepo.Delete(absenteeismNextDay);
                                            ValidateEndDateMustLessThanStartDate(absenteeismPreviousDay);
                                            _exePlantWorkerAbsenteesimRepo.Update(absenteeismPreviousDay);
                                        }
                                        else if (absenteeismPreviousDay != null && absenteeismNextDay == null)
                                        {
                                            absenteeismPreviousDay.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                                            absenteeismPreviousDay.CreatedDate = dto.UpdatedDate;
                                            absenteeismPreviousDay.CreatedBy = dto.UpdatedBy;
                                            absenteeismPreviousDay.UpdatedDate = dto.UpdatedDate;
                                            absenteeismPreviousDay.UpdatedBy = dto.UpdatedBy;
                                            absenteeismPreviousDay.LocationCode = eblekCode[1];
                                            absenteeismPreviousDay.UnitCode = eblekCode[3];
                                            absenteeismPreviousDay.GroupCode = eblekCode[4];
                                            absenteeismPreviousDay.TransactionDate = DateTime.Now.Date;

                                            ValidateEndDateMustLessThanStartDate(absenteeismPreviousDay);
                                            _exePlantWorkerAbsenteesimRepo.Update(absenteeismPreviousDay);
                                        }
                                        else if (absenteeismPreviousDay == null && absenteeismNextDay != null)
                                        {
                                            newAbsenteeism.EndDateAbsent = absenteeismNextDay.EndDateAbsent;
                                            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                                            _exePlantWorkerAbsenteesimRepo.Delete(absenteeismNextDay);
                                        }
                                        else
                                        {
                                            var existingData = _exePlantWorkerAbsenteesimRepo.GetByID(dto.StartDateAbsent,
                                                dto.EmployeeID, Convert.ToInt32(eblekCode[2]));

                                            if (existingData == null)
                                            {
                                                ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                                                _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                                            }
                                            else
                                            {
                                                existingData.AbsentType = dto.AbsentType;
                                                existingData.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                                                existingData.SktAbsentCode = dto.AbsentCodeEblek;
                                                existingData.PayrollAbsentCode = dto.AbsentCodePayroll;
                                                existingData.CreatedDate = dto.UpdatedDate;
                                                existingData.CreatedBy = dto.UpdatedBy;
                                                existingData.UpdatedDate = dto.UpdatedDate;
                                                existingData.UpdatedBy = dto.UpdatedBy;
                                                existingData.LocationCode = eblekCode[1];
                                                existingData.UnitCode = eblekCode[3];
                                                existingData.GroupCode = eblekCode[4];
                                                existingData.TransactionDate = DateTime.Now.Date;
                                                existingData.Shift = Convert.ToInt32(eblekCode[2]);
                                                existingData.EmployeeNumber = employeeNumber;

                                                ValidateEndDateMustLessThanStartDate(existingData);
                                                _exePlantWorkerAbsenteesimRepo.Update(existingData);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (absenteeismPreviousDay != null)
                                    {
                                        absenteeismPreviousDay.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                                        absenteeismPreviousDay.CreatedDate = dto.UpdatedDate;
                                        absenteeismPreviousDay.CreatedBy = dto.UpdatedBy;
                                        absenteeismPreviousDay.UpdatedDate = dto.UpdatedDate;
                                        absenteeismPreviousDay.UpdatedBy = dto.UpdatedBy;
                                        absenteeismPreviousDay.LocationCode = eblekCode[1];
                                        absenteeismPreviousDay.UnitCode = eblekCode[3];
                                        absenteeismPreviousDay.GroupCode = eblekCode[4];
                                        absenteeismPreviousDay.TransactionDate = DateTime.Now.Date;

                                        ValidateEndDateMustLessThanStartDate(absenteeismPreviousDay);
                                        _exePlantWorkerAbsenteesimRepo.Update(absenteeismPreviousDay);
                                    }
                                    else
                                    {
                                        var existingData = _exePlantWorkerAbsenteesimRepo.GetByID(dto.StartDateAbsent,
                                               dto.EmployeeID, Convert.ToInt32(eblekCode[2]));

                                        if (existingData == null)
                                        {
                                            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                                        }
                                        else
                                        {
                                            existingData.AbsentType = dto.AbsentType;
                                            existingData.EndDateAbsent = dto.StartDateAbsent.Value.Date;
                                            existingData.SktAbsentCode = dto.AbsentCodeEblek;
                                            existingData.PayrollAbsentCode = dto.AbsentCodePayroll;
                                            existingData.CreatedDate = dto.UpdatedDate;
                                            existingData.CreatedBy = dto.UpdatedBy;
                                            existingData.UpdatedDate = dto.UpdatedDate;
                                            existingData.UpdatedBy = dto.UpdatedBy;
                                            existingData.LocationCode = eblekCode[1];
                                            existingData.UnitCode = eblekCode[3];
                                            existingData.GroupCode = eblekCode[4];
                                            existingData.TransactionDate = DateTime.Now.Date;
                                            existingData.Shift = Convert.ToInt32(eblekCode[2]);
                                            existingData.EmployeeNumber = employeeNumber;

                                            ValidateEndDateMustLessThanStartDate(existingData);
                                            _exePlantWorkerAbsenteesimRepo.Update(existingData);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var newAbsenteeismStartDateChange = new ExePlantWorkerAbsenteeism();
                                newAbsenteeismStartDateChange.EmployeeID = currAbsenteeism.EmployeeID;
                                newAbsenteeismStartDateChange.AbsentType = currAbsenteeism.AbsentType;
                                newAbsenteeismStartDateChange.SktAbsentCode = currAbsenteeism.SktAbsentCode;
                                newAbsenteeismStartDateChange.PayrollAbsentCode = currAbsenteeism.PayrollAbsentCode;
                                newAbsenteeismStartDateChange.CreatedDate = currAbsenteeism.UpdatedDate;
                                newAbsenteeismStartDateChange.CreatedBy = currAbsenteeism.UpdatedBy;
                                newAbsenteeismStartDateChange.UpdatedDate = DateTime.Now;
                                newAbsenteeismStartDateChange.UpdatedBy = dto.UpdatedBy;
                                newAbsenteeismStartDateChange.LocationCode = currAbsenteeism.LocationCode;
                                newAbsenteeismStartDateChange.UnitCode = currAbsenteeism.UnitCode;
                                newAbsenteeismStartDateChange.GroupCode = currAbsenteeism.GroupCode;
                                newAbsenteeismStartDateChange.TransactionDate = DateTime.Now.Date;
                                newAbsenteeismStartDateChange.Shift = currAbsenteeism.Shift;
                                newAbsenteeismStartDateChange.EmployeeNumber = currAbsenteeism.EmployeeNumber;

                                newAbsenteeismStartDateChange.StartDateAbsent = currAbsenteeism.StartDateAbsent.AddDays(1);
                                newAbsenteeismStartDateChange.EndDateAbsent = currAbsenteeism.EndDateAbsent;

                                _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                                ValidateEndDateMustLessThanStartDate(newAbsenteeismStartDateChange);
                                _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeismStartDateChange);
                                ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                                _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                            }
                        }
                        else if (currAbsenteeism.EndDateAbsent.Date == dto.StartDateAbsent.Value.Date)
                        {
                            // Check if current absenteeism only one day or not
                            if ((currAbsenteeism.EndDateAbsent.Date - currAbsenteeism.StartDateAbsent.Date).Days == 0)
                            {
                                _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                                ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                                _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                            }
                            else
                            {
                                //var newAbsenteeismEndDateChange = new ExePlantWorkerAbsenteeism();
                                //newAbsenteeismEndDateChange.EmployeeID = currAbsenteeism.EmployeeID;
                                //newAbsenteeismEndDateChange.AbsentType = currAbsenteeism.AbsentType;
                                //newAbsenteeismEndDateChange.SktAbsentCode = currAbsenteeism.SktAbsentCode;
                                //newAbsenteeismEndDateChange.PayrollAbsentCode = currAbsenteeism.PayrollAbsentCode;
                                //newAbsenteeismEndDateChange.CreatedDate = currAbsenteeism.UpdatedDate;
                                //newAbsenteeismEndDateChange.CreatedBy = currAbsenteeism.UpdatedBy;
                                //newAbsenteeismEndDateChange.UpdatedDate = DateTime.Now;
                                //newAbsenteeismEndDateChange.UpdatedBy = dto.UpdatedBy;
                                //newAbsenteeismEndDateChange.LocationCode = currAbsenteeism.LocationCode;
                                //newAbsenteeismEndDateChange.UnitCode = currAbsenteeism.UnitCode;
                                //newAbsenteeismEndDateChange.GroupCode = currAbsenteeism.GroupCode;
                                //newAbsenteeismEndDateChange.TransactionDate = DateTime.Now.Date;
                                //newAbsenteeismEndDateChange.Shift = currAbsenteeism.Shift;
                                //newAbsenteeismEndDateChange.EmployeeNumber = currAbsenteeism.EmployeeNumber;

                                //newAbsenteeismEndDateChange.StartDateAbsent = currAbsenteeism.StartDateAbsent;
                                //newAbsenteeismEndDateChange.EndDateAbsent = currAbsenteeism.EndDateAbsent.AddDays(-1);

                                //_exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                                //ValidateEndDateMustLessThanStartDate(newAbsenteeismEndDateChange);
                                //_exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeismEndDateChange);
                                //ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                                //_exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);

                                currAbsenteeism.EndDateAbsent = currAbsenteeism.EndDateAbsent.AddDays(-1);
                                _exePlantWorkerAbsenteesimRepo.Update(currAbsenteeism);

                                InsertWorkerAbsenteeismFromProdEntry(dto);
                            }
                        }
                        else
                        {
                            // Split current absenteeism into 2 datas
                            var newAbsenteeismSplit1 = new ExePlantWorkerAbsenteeism();
                            newAbsenteeismSplit1.StartDateAbsent = currAbsenteeism.StartDateAbsent;
                            newAbsenteeismSplit1.EmployeeID = currAbsenteeism.EmployeeID;
                            newAbsenteeismSplit1.AbsentType = currAbsenteeism.AbsentType;
                            newAbsenteeismSplit1.EndDateAbsent = dto.StartDateAbsent.Value.AddDays(-1);
                            newAbsenteeismSplit1.SktAbsentCode = currAbsenteeism.SktAbsentCode;
                            newAbsenteeismSplit1.PayrollAbsentCode = currAbsenteeism.PayrollAbsentCode;
                            newAbsenteeismSplit1.CreatedDate = currAbsenteeism.UpdatedDate;
                            newAbsenteeismSplit1.CreatedBy = currAbsenteeism.UpdatedBy;
                            newAbsenteeismSplit1.UpdatedDate = currAbsenteeism.UpdatedDate;
                            newAbsenteeismSplit1.UpdatedBy = currAbsenteeism.UpdatedBy;
                            newAbsenteeismSplit1.LocationCode = currAbsenteeism.LocationCode;
                            newAbsenteeismSplit1.UnitCode = currAbsenteeism.UnitCode;
                            newAbsenteeismSplit1.GroupCode = currAbsenteeism.GroupCode;
                            newAbsenteeismSplit1.TransactionDate = DateTime.Now.Date;
                            newAbsenteeismSplit1.Shift = currAbsenteeism.Shift;
                            newAbsenteeismSplit1.EmployeeNumber = currAbsenteeism.EmployeeNumber;

                            var newAbsenteeismSplit2 = new ExePlantWorkerAbsenteeism();
                            newAbsenteeismSplit2.StartDateAbsent = dto.StartDateAbsent.Value.AddDays(1);
                            newAbsenteeismSplit2.EmployeeID = currAbsenteeism.EmployeeID;
                            newAbsenteeismSplit2.AbsentType = currAbsenteeism.AbsentType;
                            newAbsenteeismSplit2.EndDateAbsent = currAbsenteeism.EndDateAbsent;
                            newAbsenteeismSplit2.SktAbsentCode = currAbsenteeism.SktAbsentCode;
                            newAbsenteeismSplit2.PayrollAbsentCode = currAbsenteeism.PayrollAbsentCode;
                            newAbsenteeismSplit2.CreatedDate = currAbsenteeism.UpdatedDate;
                            newAbsenteeismSplit2.CreatedBy = currAbsenteeism.UpdatedBy;
                            newAbsenteeismSplit2.UpdatedDate = currAbsenteeism.UpdatedDate;
                            newAbsenteeismSplit2.UpdatedBy = currAbsenteeism.UpdatedBy;
                            newAbsenteeismSplit2.LocationCode = currAbsenteeism.LocationCode;
                            newAbsenteeismSplit2.UnitCode = currAbsenteeism.UnitCode;
                            newAbsenteeismSplit2.GroupCode = currAbsenteeism.GroupCode;
                            newAbsenteeismSplit2.TransactionDate = DateTime.Now.Date;
                            newAbsenteeismSplit2.Shift = currAbsenteeism.Shift;
                            newAbsenteeismSplit2.EmployeeNumber = currAbsenteeism.EmployeeNumber;

                            _exePlantWorkerAbsenteesimRepo.Delete(currAbsenteeism);
                            ValidateEndDateMustLessThanStartDate(newAbsenteeismSplit1);
                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeismSplit1);
                            ValidateEndDateMustLessThanStartDate(newAbsenteeismSplit2);
                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeismSplit2);
                            ValidateEndDateMustLessThanStartDate(newAbsenteeism);
                            _exePlantWorkerAbsenteesimRepo.Insert(newAbsenteeism);
                        }
                    }
                }
                else{
                    InsertWorkerAbsenteeismFromProdEntry(dto);
                }
            }
        }

        private ExePlantWorkerAbsenteeism CreateObjectWorkerAbsenteeismFromEntry(ExePlantProductionEntryDTO dto) 
        {
            var eblekCode = dto.ProductionEntryCode.Split('/');

            var employeeAcv = _mstPlantEmpJobsdataAcvRepo.GetByID(dto.EmployeeID);
            var employeeNumber = employeeAcv == null ? null : employeeAcv.EmployeeNumber;

            var newAbsenteeism = new ExePlantWorkerAbsenteeism();
            newAbsenteeism.StartDateAbsent = dto.StartDateAbsent.Value.Date;
            newAbsenteeism.EmployeeID = dto.EmployeeID;
            newAbsenteeism.AbsentType = dto.AbsentType;
            newAbsenteeism.EndDateAbsent = dto.StartDateAbsent.Value.Date;
            newAbsenteeism.SktAbsentCode = dto.AbsentCodeEblek;
            newAbsenteeism.PayrollAbsentCode = dto.AbsentCodePayroll;
            newAbsenteeism.CreatedDate = dto.UpdatedDate;
            newAbsenteeism.CreatedBy = dto.UpdatedBy;
            newAbsenteeism.UpdatedDate = dto.UpdatedDate;
            newAbsenteeism.UpdatedBy = dto.UpdatedBy;
            newAbsenteeism.LocationCode = eblekCode[1];
            newAbsenteeism.UnitCode = eblekCode[3];
            newAbsenteeism.GroupCode = eblekCode[4];
            newAbsenteeism.TransactionDate = DateTime.Now.Date;
            newAbsenteeism.Shift = Convert.ToInt32(eblekCode[2]);
            newAbsenteeism.EmployeeNumber = employeeNumber;

            return newAbsenteeism;
        }

        private bool CheckClosingPayrollOrHoliday(DateTime startDateEblek, string locationCode)
        {
            var result = false;

            // Check date closing payroll
            var closingPayroll = _mstClosingPayroll.GetByID(startDateEblek.Date);
            if (closingPayroll != null) result = true;

            // Check mst holiday
            var holiday = _mstGenHoliday.Get(c => c.HolidayDate == startDateEblek && c.LocationCode == locationCode);
            if (holiday.Any()) result = true;

            // Check if sunday
            if ((int)startDateEblek.DayOfWeek == 0) result = true;

            return result;

        }

        /// <summary>
        /// Get Brand Code from Plant Production Entry Verification By Worker Assigment Input Parameter
        /// </summary>
        /// <param name="input">Parameter from worker assignment</param>
        /// <returns></returns>
        public List<string> GetExePlantProductionEntryVerificationBrand(GetExePlantProductionEntryVerificationBrandInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            if (input.Date != null)
                queryFilter = queryFilter.And(m => m.ProductionDate == input.Date);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantProductionEntryVerification>();

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter, orderByFilter);

            var brand = dbResult.Select(c => c.BrandCode).ToList();

            return brand;
        }

        public List<ExePlantProductionEntryAllocationCompositeDTO> GetPlantProductionEntryAllocation(GetExePlantProductionEntryInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntry>();
            var productionEntryCode = EnumHelper.GetDescription(Enums.CombineCode.EBL) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.UnitCode + "/"
                                      + "&groupCode&/"
                                      + input.Brand;

            //if (input.FilterType == "Year")
            productionEntryCode += "/" + input.Year;
            //if (input.FilterType == "Week")
            productionEntryCode += "/" + input.Week;

            var prodEntryCode = productionEntryCode.Replace("&groupCode&", input.Group);
            queryFilter = queryFilter.And(m => m.ProductionEntryCode.Contains(prodEntryCode));

            if (input.Date.HasValue)
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(m.ExePlantProductionEntryVerification.ProductionDate) == DbFunctions.TruncateTime(input.Date));

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "MstPlantEmpJobsDataAll.EmployeeNumber" }, "ASC");
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantProductionEntry>();

            var dbResult = _exePlantProductionEntryRepo.Get(queryFilter, orderByFilter);

            var result = Mapper.Map<List<ExePlantProductionEntryAllocationCompositeDTO>>(dbResult);

            var dbWorkerAssigmnt = _exePlantWorkerAssignmentRepo.Get(m => m.DestinationLocationCode == input.LocationCode && m.DestinationUnitCode == input.UnitCode && m.DestinationProcessGroup == input.ProcessGroup && //m.SourceShift.HasValue == input.Shift &&
                         m.DestinationBrandCode == input.Brand && m.DestinationGroupCode == input.Group && m.TransactionDate == input.Date);

            // If not have dummy datas
            if (dbWorkerAssigmnt == null) return result;
            var listProdEntryFromAssignment = new List<ExePlantProductionEntryAllocationCompositeDTO>();

            foreach (var exePlantWorkerAssignment in dbWorkerAssigmnt)
            {
                // add DestinationGroupCode
                prodEntryCode = productionEntryCode.Replace("&groupCode&", exePlantWorkerAssignment.DestinationGroupCode);
                queryFilter = queryFilter.And(m => m.ProductionEntryCode.Contains(prodEntryCode));
                dbResult = _exePlantProductionEntryRepo.Get(queryFilter, orderByFilter);
                listProdEntryFromAssignment.AddRange(dbResult.Select(Mapper.Map<ExePlantProductionEntryAllocationCompositeDTO>));

                // add DestinationGroupCodeDummy
                prodEntryCode = productionEntryCode.Replace("&groupCode&", exePlantWorkerAssignment.DestinationGroupCodeDummy);
                queryFilter = queryFilter.And(m => m.ProductionEntryCode.Contains(prodEntryCode));
                dbResult = _exePlantProductionEntryRepo.Get(queryFilter, orderByFilter);
                listProdEntryFromAssignment.AddRange(dbResult.Select(Mapper.Map<ExePlantProductionEntryAllocationCompositeDTO>));
            }

            result.AddRange(listProdEntryFromAssignment);

            return result;
        }

        public ExePlantProductionEntryAllocationCompositeDTO DeleteProductionEntry(ExePlantProductionEntryDTO eblekDtos)
        {
            //// Insert to WorkerAssignmentRemoval
            //SaveWorkerAssignmentRemoval(eblekDtos);

            // Delete ExePlantProductionEntry
            var dbProductionEntry = _exePlantProductionEntryRepo.GetByID(eblekDtos.ProductionEntryCode, eblekDtos.EmployeeID);
            if (dbProductionEntry == null) throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            _exePlantProductionEntryRepo.Delete(dbProductionEntry);

            _uow.SaveChanges();

            #region
            // ditutup, group di verification tidak di delete, tiket http://tp.voxteneo.co.id/entity/18953
            //Delete ExePlantProductionEntryVerification if not have data on ExePlantProductionEntry
            //var dbProductionEntyCount = _exePlantProductionEntryRepo.Get(m => m.ProductionEntryCode == eblekDtos.ProductionEntryCode).Count();
            //if (dbProductionEntyCount < 1)
            //{
            //    var dbProductionEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(eblekDtos.ProductionEntryCode);
            //    if (dbProductionEntryVerification != null)
            //    {
            //        _exePlantProductionEntryVerificationRepo.Delete(dbProductionEntryVerification);
            //        _uow.SaveChanges();
            //    }
            //}
            #endregion

            return Mapper.Map<ExePlantProductionEntryAllocationCompositeDTO>(dbProductionEntry);
        }

        public ExePlantProductionEntryDTO GetExePlantProductionEntryByCode(GetExePlantProductionEntryInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntry>();

            var productionEntryCode = EnumHelper.GetDescription(Enums.CombineCode.EBL) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.UnitCode + "/"
                                      + input.Group + "/"
                                      + input.Brand;

            //if (input.FilterType == "Year")
            productionEntryCode += "/" + input.Year;
            //if (input.FilterType == "Week")
            productionEntryCode += "/" + input.Week;

            if (input.Date.HasValue)
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(m.ExePlantProductionEntryVerification.ProductionDate) == DbFunctions.TruncateTime(input.Date));

            queryFilter = queryFilter.And(m => m.ProductionEntryCode.Contains(productionEntryCode));

            var dbResult = _exePlantProductionEntryRepo.Get(queryFilter).OrderByDescending(m => m.UpdatedDate).FirstOrDefault();
            return Mapper.Map<ExePlantProductionEntryDTO>(dbResult);
        }

        private ExePlantProductionEntryDTO GetExePlantProductionEntryByCodeEmployeeID(GetExePlantProductionEntryInput input, string employeeID)
        {
            int day = input.Date.Value.DayOfWeek == 0 ? 7 : (int)input.Date.Value.DayOfWeek;

            // Create combine code
            var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.UnitCode + "/"
                                      + input.Group + "/"
                                      + input.Brand + "/"
                                      + input.Year + "/"
                                      + input.Week + "/"
                                      + day;

            var dbResult = _exePlantProductionEntryRepo.GetByID(productionEntryCode, employeeID);
            return Mapper.Map<ExePlantProductionEntryDTO>(dbResult);
        }

        public void SubmitProductionEntry(string locationCode, string unit, string brand, int? shift, int? year, int? week, DateTime? date, string groupCode, string createdBy)
        {
            _sqlSPRepo.InsertPlantExeReportByGroups(locationCode, unit, brand, shift, year, week, date, groupCode, createdBy);
        }


        public ExeProductionEntryMinimumValueDTO GetMinimumValueForActualProdEntry(GetExePlantProductionEntryInput input)
        {
            var queryFilter = PredicateHelper.True<ExeProductionEntryMinimumValue>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            var shift = Convert.ToInt32(input.Shift);

            if (!string.IsNullOrEmpty(input.Shift))
                queryFilter = queryFilter.And(m => m.Shift == shift);
            if (!string.IsNullOrEmpty(input.ProcessGroup))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.ProcessGroup);
            if (!string.IsNullOrEmpty(input.Group))
                queryFilter = queryFilter.And(m => m.GroupCode == input.Group);
            if (!string.IsNullOrEmpty(input.Brand))
                queryFilter = queryFilter.And(m => m.BrandCode == input.Brand);

            var dbresult = _exeProductionMinimumValue.Get(queryFilter).FirstOrDefault();
            var data = Mapper.Map<ExeProductionEntryMinimumValueDTO>(dbresult);

            return data;
        }

        public ExePlantProductionEntryDTO SaveProductionEntry_SP(ExePlantProductionEntryDTO eblekDto)
        {
            var dbProductionEntry = _exePlantProductionEntryRepo.GetByID(eblekDto.ProductionEntryCode, eblekDto.EmployeeID);
            if (dbProductionEntry == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (dbProductionEntry.AbsentType != eblekDto.AbsentType)
            {
                if (String.IsNullOrEmpty(dbProductionEntry.AbsentType))
                {
                    var absenteeismToValidate = new ExePlantWorkerAbsenteeismDTO
                    {
                        AbsentType = eblekDto.AbsentType,
                        StartDateAbsent = eblekDto.StartDateAbsent.Value,
                        EndDateAbsent = eblekDto.StartDateAbsent.Value,
                        EmployeeID = eblekDto.EmployeeID
                    };
                    ValidateDateRangePlantWorkerAbsenteeism(absenteeismToValidate);
                }
            }

            // State on target
            if (eblekDto.SaveType == "target")
            {
                // Check if target is not null then absent type is different from current data
                if ((eblekDto.AbsentType != dbProductionEntry.AbsentType) || (eblekDto.ProdTarget.HasValue))
                {
                    _sqlSPRepo.SavePlantProductionEntry_SP(Mapper.Map(eblekDto, dbProductionEntry), eblekDto.SaveType);
                }
            }
            else
            {
                _sqlSPRepo.SavePlantProductionEntry_SP(Mapper.Map(eblekDto, dbProductionEntry), eblekDto.SaveType);
            }
            
            return eblekDto;
        }

        public void SaveDefaultTargetActualProdEntry_SP(string productionEntryCode, string saveType)
        {
            _sqlSPRepo.SaveDefaultTargetActualEntry_SP(productionEntryCode, saveType);
        }
        #endregion

        #region Actual Work Hours
        public bool CheckCompletedExePlantActualWorkHours(GetExePlantProductionEntryInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantActualWorkHoursView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.Brand))
                queryFilter = queryFilter.And(m => m.BrandCode == input.Brand);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantActualWorkHoursView>();

            var dbResult = _exeExePlantActualWorkHoursViewRepo.Get(queryFilter, orderByFilter);

            var masterLists = Mapper.Map<List<ExePlantActualWorkHoursDTO>>(dbResult);

            if (masterLists == null) return false;

            foreach (ExePlantActualWorkHoursDTO t in masterLists)
            {
                var exist = _exePlantActualWorkHoursRepo.GetByID(input.LocationCode, input.UnitCode, Int32.Parse(input.Shift), input.Brand, input.Date, t.ProcessGroup);
                if (exist == null)
                {
                    return false;
                }

                if (exist.TimeIn == TimeSpan.Parse(Constants.DefaultTimeIn) || exist.TimeOut == TimeSpan.Parse(Constants.DefaultTimeOut) || exist.BreakTime == TimeSpan.Parse(Constants.DefaultBreakTime))
                    return false;

            }

            return true;
        }
        public bool CheckCompletedActualWorkHours(GetExePlantProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantActualWorkHoursView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantActualWorkHoursView>();

            var dbResult = _exeExePlantActualWorkHoursViewRepo.Get(queryFilter, orderByFilter);

            var masterLists = Mapper.Map<List<ExePlantActualWorkHoursDTO>>(dbResult);

            if (masterLists == null) return false;

            foreach (ExePlantActualWorkHoursDTO t in masterLists)
            {
                var exist = _exePlantActualWorkHoursRepo.GetByID(input.LocationCode, input.UnitCode, input.Shift, input.BrandCode, input.ProductionDate, t.ProcessGroup);
                if (exist == null)
                {
                    return false;
                }

                if (exist.TimeIn == TimeSpan.Parse(Constants.DefaultTimeIn) || exist.TimeOut == TimeSpan.Parse(Constants.DefaultTimeOut))
                    return false;

            }

            return true;
        }

        public List<ExePlantActualWorkHoursDTO> GetExePlantActualWorkHours(GetExePlantActualWorkHoursInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantActualWorkHoursView>();

            //if (!string.IsNullOrEmpty(input.LocationCode))
            //    queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            //if (!string.IsNullOrEmpty(input.Brand))
            //    queryFilter = queryFilter.And(m => m.BrandCode == input.Brand);

            queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode).And(m => m.BrandCode == input.Brand);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantActualWorkHoursView>();

            var dbResult = _exeExePlantActualWorkHoursViewRepo.Get(queryFilter, orderByFilter);

            var masterLists = Mapper.Map<List<ExePlantActualWorkHoursDTO>>(dbResult);

            if (masterLists == null) return null;

            foreach (ExePlantActualWorkHoursDTO t in masterLists)
            {
                var exist = _exePlantActualWorkHoursRepo.GetByID(input.LocationCode, input.UnitCode, input.Shift, input.Brand, input.Date, t.ProcessGroup);
                if (exist == null)
                {
                    t.TimeIn = TimeSpan.Parse(Constants.DefaultTimeIn);
                    t.TimeOut = TimeSpan.Parse(Constants.DefaultTimeOut);
                    t.BreakTime = TimeSpan.Parse(Constants.DefaultBreakTime);
                }
                else
                {
                    // default data if no row found
                    t.TimeIn = exist.TimeIn;
                    t.TimeOut = exist.TimeOut;
                    t.BreakTime = exist.BreakTime;
                }

                t.Shift = input.Shift;
                t.BrandCode = input.Brand;
                t.UnitCode = input.UnitCode;
            }

            return masterLists;
        }

        public ExePlantActualWorkHoursDTO InsertUpdateExePlantActualWorkHours(ExePlantActualWorkHoursDTO post)
        {
            var exist = _exePlantActualWorkHoursRepo.GetByID(post.LocationCode, post.UnitCode, post.Shift, post.BrandCode, post.ProductionDate, post.ProcessGroup);

            if (exist == null)
            { //new data insert
                var dbExePlantActualWorkHours = Mapper.Map<ExeActualWorkHour>(post);

                dbExePlantActualWorkHours.CreatedDate = DateTime.Now;
                dbExePlantActualWorkHours.UpdatedDate = DateTime.Now;
                dbExePlantActualWorkHours.CreatedBy = dbExePlantActualWorkHours.UpdatedBy;
                _exePlantActualWorkHoursRepo.Insert(dbExePlantActualWorkHours);

                _uow.SaveChanges();

                return Mapper.Map<ExePlantActualWorkHoursDTO>(dbExePlantActualWorkHours);
            }
            else
            { //update existing data
                post.CreatedBy = exist.CreatedBy;
                post.CreatedDate = exist.CreatedDate;
                Mapper.Map(post, exist);
                exist.UpdatedDate = DateTime.Now;
                _exePlantActualWorkHoursRepo.Update(exist);

                _uow.SaveChanges();

                return Mapper.Map<ExePlantActualWorkHoursDTO>(exist);
            }
        }
        #endregion

        #region Worker Assignment

        public List<ExePlantWorkerAssignmentDTO> GetExePlantWorkerAssignments(GetExePlantWorkerAssignmentInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantWorkerAssignment>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.SourceLocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.SourceUnitCode == input.UnitCode);
            if (input.Shift > 0)
                queryFilter = queryFilter.And(m => m.SourceShift == input.Shift);
            if (input.Date.HasValue && input.DateTypeFilter == "rdbDate")
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(m.TransactionDate) == DbFunctions.TruncateTime(input.Date));
            if (input.ProductionDateFrom.HasValue && input.DateTypeFilter == "rdbProductionDate")
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(m.StartDate) >= DbFunctions.TruncateTime(input.ProductionDateFrom));
            if (input.ProductionDateTo.HasValue && input.DateTypeFilter == "rdbProductionDate")
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(m.EndDate) <= DbFunctions.TruncateTime(input.ProductionDateTo));
            if (!string.IsNullOrEmpty(input.SourceBrandCode))
                queryFilter = queryFilter.And(m => m.SourceBrandCode == input.SourceBrandCode);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantWorkerAssignment>();

            var dbResult = _exePlantWorkerAssignmentRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<ExePlantWorkerAssignmentDTO>>(dbResult);
        }

        public ExePlantWorkerAssignmentDTO InsertWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignment)
        {
            // validate insert
            ValidateInsertWorkerAssignment(workerAssignment);

            // get absenttype multiskillout
            var absentType = _mstPlantAbsentType.GetByID(EnumHelper.GetDescription(Enums.SKTAbsentCode.MO));

            string remark;
            if (absentType.Remark != null && absentType.Remark.Length > 8)
            {
                remark = absentType.Remark.Substring(0, 8);
            }
            else
            {
                remark = absentType.Remark;
            }

            // create new worker absenteeism
            //var workerAbsenteeism = Mapper.Map<ExePlantWorkerAbsenteeismDTO>(workerAssignment);
            //workerAbsenteeism.SktAbsentCode = Enums.SKTAbsentCode.MO.ToString();
            //workerAbsenteeism.AbsentType = EnumHelper.GetDescription(Enums.SKTAbsentCode.MO);
            //workerAbsenteeism.PayrollAbsentCode = absentType.PayrollAbsentCode;
            //workerAbsenteeism.Shift = workerAssignment.SourceShift == null ? 0 : workerAssignment.SourceShift.Value;
            //workerAbsenteeism.TransactionDate = DateTime.Now;
            //workerAbsenteeism.CreatedDate = DateTime.Now;
            //workerAbsenteeism.UpdatedDate = DateTime.Now;
            //workerAbsenteeism.IsFromWorkerAssignment = true;

            // insert worker absenteeism
            //InsertWorkerAbsenteeism(workerAbsenteeism);

            var dbExePlantWorkerAssignment = Mapper.Map<ExePlantWorkerAssignment>(workerAssignment);

            dbExePlantWorkerAssignment.CreatedDate = DateTime.Now;
            dbExePlantWorkerAssignment.UpdatedDate = DateTime.Now;

            dbExePlantWorkerAssignment.DestinationGroupCodeDummy = String.IsNullOrEmpty(workerAssignment.DestinationGroupCode) ? null : workerAssignment.DestinationGroupCode.Remove(1, 1).Insert(1, "5");

            _exePlantWorkerAssignmentRepo.Insert(dbExePlantWorkerAssignment);

            // Checking if assignment to destination TPO location
            // Get DTO location TPO 
            var locationTpoDto = _sqlSPRepo.GetLastChildLocationByLocationCode(Enums.LocationCode.TPO.ToString());
            var listLocationTpo = locationTpoDto.Select(c => c.LocationCode).Distinct().ToList();
            var isDestinationToTpo = listLocationTpo.Contains(dbExePlantWorkerAssignment.DestinationLocationCode);

            for (DateTime date = dbExePlantWorkerAssignment.StartDate; date <= dbExePlantWorkerAssignment.EndDate; date = date.AddDays(1))
            {
                var mstGenWeek = _masterDataBll.GetWeekByDate(date);
                var week = mstGenWeek == null ? 0 : mstGenWeek.Week == null ? 0 : mstGenWeek.Week.Value;
                var year = mstGenWeek == null ? 0 : mstGenWeek.Year == null ? 0 : mstGenWeek.Year.Value;

                var sourceCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                            Enums.CombineCode.EBL,
                            dbExePlantWorkerAssignment.SourceLocationCode,
                            dbExePlantWorkerAssignment.SourceShift,
                            dbExePlantWorkerAssignment.SourceUnitCode,
                            dbExePlantWorkerAssignment.SourceGroupCode,
                            dbExePlantWorkerAssignment.SourceBrandCode,
                            year,
                            week,
                            (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var destCombineCodeDummy = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                           Enums.CombineCode.EBL,
                           dbExePlantWorkerAssignment.DestinationLocationCode,
                           dbExePlantWorkerAssignment.SourceShift,
                           dbExePlantWorkerAssignment.DestinationUnitCode,
                           dbExePlantWorkerAssignment.DestinationGroupCodeDummy,
                           dbExePlantWorkerAssignment.DestinationBrandCode,
                           year,
                           week,
                           (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var destGroupCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                           Enums.CombineCode.EBL,
                           dbExePlantWorkerAssignment.DestinationLocationCode,
                           dbExePlantWorkerAssignment.SourceShift,
                           dbExePlantWorkerAssignment.DestinationUnitCode,
                           dbExePlantWorkerAssignment.DestinationGroupCode,
                           dbExePlantWorkerAssignment.DestinationBrandCode,
                           year,
                           week,
                           (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var destProdEntryVef = _exePlantProductionEntryVerificationRepo.GetByID(destGroupCombineCode);

                var sourceProdEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(sourceCombineCode);
                if (workerAssignment.SourceProcessGroup != "DAILY")
                {
                    if (sourceProdEntryVerification != null)
                    {
                        var sourceProdEntry = _exePlantProductionEntryRepo.GetByID(sourceCombineCode, dbExePlantWorkerAssignment.EmployeeID);

                        if (sourceProdEntry != null)
                        {
                            if (isDestinationToTpo)
                            {
                                // Get Absent Type Tugas Luar
                                var absentTypeTugasLuar = _mstPlantAbsentType.GetByID(EnumHelper.GetDescription(Enums.SKTAbsentCode.TL));

                                sourceProdEntry.AbsentType = EnumHelper.GetDescription(Enums.SKTAbsentCode.TL);
                                sourceProdEntry.AbsentCodeEblek = absentTypeTugasLuar.SktAbsentCode;
                                sourceProdEntry.AbsentCodePayroll = absentTypeTugasLuar.PayrollAbsentCode;
                                sourceProdEntry.ProdActual = 0;
                                sourceProdEntry.ProdTarget = 0;
                                _exePlantProductionEntryRepo.Update(sourceProdEntry);
                            }
                            else
                            {
                                sourceProdEntry.AbsentType = EnumHelper.GetDescription(Enums.SKTAbsentCode.MO);
                                sourceProdEntry.AbsentCodeEblek = absentType.SktAbsentCode;
                                sourceProdEntry.AbsentCodePayroll = absentType.PayrollAbsentCode;
                                _exePlantProductionEntryRepo.Update(sourceProdEntry);

                                var isSameProcess = dbExePlantWorkerAssignment.SourceProcessGroup.ToLower() ==
                                                    dbExePlantWorkerAssignment.DestinationProcessGroup.ToLower();

                                var newDestProductionEntry = new ExePlantProductionEntry()
                                {
                                    ProductionEntryCode = destCombineCodeDummy,
                                    EmployeeID = dbExePlantWorkerAssignment.EmployeeID,
                                    EmployeeNumber = _masterDataBll.GetMstEmployeeJobsDataActives(dbExePlantWorkerAssignment.EmployeeID).EmployeeNumber,
                                    AbsentType = null,
                                    AbsentCodePayroll = null,
                                    AbsentCodeEblek = null,
                                    AbsentRemark = null,
                                    StartDateAbsent = dbExePlantWorkerAssignment.StartDate,
                                    StatusEmp = isSameProcess ? Enums.StatusEmp.Resmi.ToString() : Enums.StatusEmp.Multiskill.ToString(),
                                    CreatedBy = dbExePlantWorkerAssignment.CreatedBy,
                                    CreatedDate = dbExePlantWorkerAssignment.CreatedDate,
                                    UpdatedBy = dbExePlantWorkerAssignment.UpdatedBy,
                                    UpdatedDate = dbExePlantWorkerAssignment.UpdatedDate,
                                    ProdCapacity = isSameProcess ? sourceProdEntry.ProdCapacity : GetProdCapacityWorkerAssignment(workerAssignment, destProdEntryVef == null ? 0 : destProdEntryVef.WorkHour),
                                    ProdActual = isSameProcess ? sourceProdEntry.ProdActual : 0,
                                    ProdTarget = isSameProcess ? sourceProdEntry.ProdTarget : 0
                                };

                                newDestProductionEntry.StatusIdentifier =
                                    Convert.ToInt32(
                                        _masterDataBll.GetGenEmpStatusIdentifierByStatusEmp(newDestProductionEntry.StatusEmp));

                                _exePlantProductionEntryRepo.Insert(newDestProductionEntry);
                                _uow.SaveChanges();
                            }
                        }

                        //check WorkerAssignment Removal Destination/Dummy Group
                        var listExistingWorkerAssignmentRemovalSource = _workerAssignmentRemovalRepository.Get(c => c.ProductionEntryCode == sourceCombineCode
                                                                                                          && c.EmployeeID == dbExePlantWorkerAssignment.EmployeeID
                                                                                                          && c.StartDate == dbExePlantWorkerAssignment.StartDate
                                                                                                          && c.EndDate == dbExePlantWorkerAssignment.EndDate).ToList();

                        //Delete Assignment Removal if Exist in range start date and end date
                        if (listExistingWorkerAssignmentRemovalSource.Any())
                        {
                            foreach (var workerAssignmentRemoval in listExistingWorkerAssignmentRemovalSource)
                                _workerAssignmentRemovalRepository.Delete(workerAssignmentRemoval);
                        }

                        if (!isDestinationToTpo)
                        {
                            //get destination production entry verification
                            var destProdEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(destCombineCodeDummy);

                            if (destProdEntryVerification == null)
                            {
                                var newDestProdEntryVerification = new ExePlantProductionEntryVerification
                                {
                                    ProductionEntryCode = destCombineCodeDummy,
                                    LocationCode = dbExePlantWorkerAssignment.DestinationLocationCode,
                                    UnitCode = dbExePlantWorkerAssignment.DestinationUnitCode,
                                    Shift = dbExePlantWorkerAssignment.SourceShift == null ? 0 : dbExePlantWorkerAssignment.SourceShift.Value,
                                    ProcessGroup = dbExePlantWorkerAssignment.DestinationProcessGroup,
                                    ProcessOrder = _mstGenProcess.GetByID(dbExePlantWorkerAssignment.DestinationProcessGroup).ProcessOrder,
                                    GroupCode = dbExePlantWorkerAssignment.DestinationGroupCodeDummy,
                                    BrandCode = dbExePlantWorkerAssignment.DestinationBrandCode,
                                    KPSYear = Convert.ToInt32(destCombineCodeDummy.Split('/')[6]),
                                    KPSWeek = Convert.ToInt32(destCombineCodeDummy.Split('/')[7]),
                                    ProductionDate = date,
                                    WorkHour = sourceProdEntryVerification.WorkHour,
                                    TPKValue = sourceProdEntryVerification.TPKValue,
                                    TotalTargetValue = sourceProdEntryVerification.TotalTargetValue,
                                    TotalActualValue = sourceProdEntryVerification.TotalActualValue,
                                    TotalCapacityValue = sourceProdEntryVerification.TotalCapacityValue,
                                    VerifyManual = sourceProdEntryVerification.VerifyManual,
                                    VerifySystem = sourceProdEntryVerification.VerifySystem,
                                    Flag_Manual = sourceProdEntryVerification.Flag_Manual,
                                    Remark = sourceProdEntryVerification.Remark,
                                    CreatedBy = dbExePlantWorkerAssignment.CreatedBy,
                                    CreatedDate = dbExePlantWorkerAssignment.CreatedDate,
                                    UpdatedBy = dbExePlantWorkerAssignment.UpdatedBy,
                                    UpdatedDate = dbExePlantWorkerAssignment.UpdatedDate,
                                };

                                _exePlantProductionEntryVerificationRepo.Insert(newDestProdEntryVerification);
                                _uow.SaveChanges();
                            }

                            //get destination production entry
                            var destProdEntry = _exePlantProductionEntryRepo.GetByID(destCombineCodeDummy, dbExePlantWorkerAssignment.EmployeeID);

                            if (destProdEntry != null)
                            {
                                destProdEntry.AbsentType = null;
                                destProdEntry.AbsentCodeEblek = null;
                                destProdEntry.AbsentCodePayroll = null;
                                _exePlantProductionEntryRepo.Update(destProdEntry);
                            }
                            else
                            {
                                var newDestProductionEntry = new ExePlantProductionEntry()
                                {
                                    ProductionEntryCode = destCombineCodeDummy,
                                    EmployeeID = dbExePlantWorkerAssignment.EmployeeID,
                                    EmployeeNumber = _masterDataBll.GetMstEmployeeJobsDataActives(dbExePlantWorkerAssignment.EmployeeID).EmployeeNumber,
                                    AbsentType = null,
                                    AbsentCodePayroll = null,
                                    AbsentCodeEblek = null,
                                    AbsentRemark = remark,
                                    StartDateAbsent = dbExePlantWorkerAssignment.StartDate,
                                    StatusEmp = dbExePlantWorkerAssignment.SourceProcessGroup.ToLower() == dbExePlantWorkerAssignment.DestinationProcessGroup.ToLower() ? Enums.StatusEmp.Resmi.ToString() : Enums.StatusEmp.Multiskill.ToString(),
                                    CreatedBy = dbExePlantWorkerAssignment.CreatedBy,
                                    CreatedDate = dbExePlantWorkerAssignment.CreatedDate,
                                    UpdatedBy = dbExePlantWorkerAssignment.UpdatedBy,
                                    UpdatedDate = dbExePlantWorkerAssignment.UpdatedDate,
                                    ProdCapacity = GetProdCapacityWorkerAssignment(workerAssignment, destProdEntryVef == null ? 0 : destProdEntryVef.WorkHour)
                                };

                                newDestProductionEntry.StatusIdentifier =
                                    Convert.ToInt32(
                                        _masterDataBll.GetGenEmpStatusIdentifierByStatusEmp(newDestProductionEntry.StatusEmp));

                                _exePlantProductionEntryRepo.Insert(newDestProductionEntry);
                            }


                            //check WorkerAssignment Removal Destination/Dummy Group
                            var listExistingWorkerAssignmentRemovalDest = _workerAssignmentRemovalRepository.Get(c => c.ProductionEntryCode == destCombineCodeDummy
                                                                                                              && c.EmployeeID == dbExePlantWorkerAssignment.EmployeeID
                                                                                                              && c.StartDate == dbExePlantWorkerAssignment.StartDate
                                                                                                              && c.EndDate == dbExePlantWorkerAssignment.EndDate).ToList();

                            //Delete Assignment Removal if Exist in range start date and end date
                            if (listExistingWorkerAssignmentRemovalDest.Any())
                            {
                                foreach (var workerAssignmentRemoval in listExistingWorkerAssignmentRemovalDest)
                                    _workerAssignmentRemovalRepository.Delete(workerAssignmentRemoval);
                            }
                        }
                    }
                }
                else
                {
                    //hakim
                    //var destProdEntryVef = _exePlantProductionEntryVerificationRepo.GetByID(destGroupCombineCode);
                    if (destProdEntryVef != null) //destination group
                    {
                        var destProdEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(destCombineCodeDummy);
                        if (destProdEntryVerification == null)
                        {
                            var newDestProdEntryVerification = new ExePlantProductionEntryVerification
                            {
                                ProductionEntryCode = destCombineCodeDummy,
                                LocationCode = dbExePlantWorkerAssignment.DestinationLocationCode,
                                UnitCode = dbExePlantWorkerAssignment.DestinationUnitCode,
                                Shift = dbExePlantWorkerAssignment.SourceShift == null ? 0 : dbExePlantWorkerAssignment.SourceShift.Value,
                                ProcessGroup = dbExePlantWorkerAssignment.DestinationProcessGroup,
                                ProcessOrder = _mstGenProcess.GetByID(dbExePlantWorkerAssignment.DestinationProcessGroup).ProcessOrder,
                                GroupCode = dbExePlantWorkerAssignment.DestinationGroupCodeDummy,
                                BrandCode = dbExePlantWorkerAssignment.DestinationBrandCode,
                                KPSYear = Convert.ToInt32(destCombineCodeDummy.Split('/')[6]),
                                KPSWeek = Convert.ToInt32(destCombineCodeDummy.Split('/')[7]),
                                ProductionDate = date,
                                WorkHour = 0,
                                TPKValue = 0,
                                TotalTargetValue = 0,
                                TotalActualValue = 0,
                                TotalCapacityValue = 0,
                                VerifyManual = false,
                                VerifySystem = false,
                                Flag_Manual = false,
                                Remark = null,
                                CreatedBy = dbExePlantWorkerAssignment.CreatedBy,
                                CreatedDate = dbExePlantWorkerAssignment.CreatedDate,
                                UpdatedBy = dbExePlantWorkerAssignment.UpdatedBy,
                                UpdatedDate = dbExePlantWorkerAssignment.UpdatedDate,
                            };

                            _exePlantProductionEntryVerificationRepo.Insert(newDestProdEntryVerification);
                            _uow.SaveChanges();
                        }
                        

                        //get destination production entry
                        var destProdEntry = _exePlantProductionEntryRepo.GetByID(destCombineCodeDummy, dbExePlantWorkerAssignment.EmployeeID);
                        //var destProdEntry = _exePlantProductionEntryRepo.GetByID(destGroupCombineCode, dbExePlantWorkerAssignment.EmployeeID);

                        if (destProdEntry != null)
                        {
                            destProdEntry.AbsentType = null;
                            destProdEntry.AbsentCodeEblek = null;
                            destProdEntry.AbsentCodePayroll = null;
                            _exePlantProductionEntryRepo.Update(destProdEntry);
                        }
                        else
                        {
                            var newDestProductionEntry = new ExePlantProductionEntry()
                            {
                                ProductionEntryCode = destCombineCodeDummy,
                                EmployeeID = dbExePlantWorkerAssignment.EmployeeID,
                                EmployeeNumber = _masterDataBll.GetMstEmployeeJobsDataActives(dbExePlantWorkerAssignment.EmployeeID).EmployeeNumber,
                                AbsentType = null,
                                AbsentCodePayroll = null,
                                AbsentCodeEblek = null,
                                AbsentRemark = null,
                                StartDateAbsent = dbExePlantWorkerAssignment.StartDate,
                                StatusEmp = dbExePlantWorkerAssignment.SourceProcessGroup.ToLower() == dbExePlantWorkerAssignment.DestinationProcessGroup.ToLower() ? Enums.StatusEmp.Resmi.ToString() : Enums.StatusEmp.Multiskill.ToString(),
                                CreatedBy = dbExePlantWorkerAssignment.CreatedBy,
                                CreatedDate = dbExePlantWorkerAssignment.CreatedDate,
                                UpdatedBy = dbExePlantWorkerAssignment.UpdatedBy,
                                UpdatedDate = dbExePlantWorkerAssignment.UpdatedDate,
                                ProdCapacity = GetProdCapacityWorkerAssignment(workerAssignment, destProdEntryVef == null ? 0 : destProdEntryVef.WorkHour)
                            };

                            newDestProductionEntry.StatusIdentifier =
                                Convert.ToInt32(
                                    _masterDataBll.GetGenEmpStatusIdentifierByStatusEmp(newDestProductionEntry.StatusEmp));

                            _exePlantProductionEntryRepo.Insert(newDestProductionEntry);
                        }


                        //check WorkerAssignment Removal Destination/Dummy Group
                        var listExistingWorkerAssignmentRemovalDest = _workerAssignmentRemovalRepository.Get(c => c.ProductionEntryCode == destCombineCodeDummy
                                                                                                            && c.EmployeeID == dbExePlantWorkerAssignment.EmployeeID
                                                                                                            && c.StartDate == dbExePlantWorkerAssignment.StartDate
                                                                                                            && c.EndDate == dbExePlantWorkerAssignment.EndDate).ToList();

                        //Delete Assignment Removal if Exist in range start date and end date
                        if (listExistingWorkerAssignmentRemovalDest.Any())
                        {
                            foreach (var workerAssignmentRemoval in listExistingWorkerAssignmentRemovalDest)
                                _workerAssignmentRemovalRepository.Delete(workerAssignmentRemoval);
                        }
                    }
                }
            }

            _uow.SaveChanges();

            return Mapper.Map<ExePlantWorkerAssignmentDTO>(dbExePlantWorkerAssignment);
        }

       public decimal GetProdCapacityWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignment, int workHour)
        {
            var result = 0m;

            var mstGenBrand = _mstGenBrandRepo.GetByID(workerAssignment.DestinationBrandCode);
            var brandGroupCode = mstGenBrand == null ? "" : mstGenBrand.BrandGroupCode;
            var planPlantIndividualCapacity = _planPlantIndividualCapacityRepo.GetByID(brandGroupCode,
                                                        workerAssignment.EmployeeID, workerAssignment.DestinationGroupCode,
                                                        workerAssignment.DestinationUnitCode, workerAssignment.DestinationLocationCode,
                                                        workerAssignment.DestinationProcessGroup);

            switch (workHour)
            {
                case 3:
                    result = planPlantIndividualCapacity == null ? 0 : planPlantIndividualCapacity.HoursCapacity3 == null ? 0 : planPlantIndividualCapacity.HoursCapacity3.Value;
                    break;
                case 5:
                    result = planPlantIndividualCapacity == null ? 0 : planPlantIndividualCapacity.HoursCapacity5 == null ? 0 : planPlantIndividualCapacity.HoursCapacity5.Value;
                    break;
                case 6:
                    result = planPlantIndividualCapacity == null ? 0 : planPlantIndividualCapacity.HoursCapacity6 == null ? 0 : planPlantIndividualCapacity.HoursCapacity6.Value;
                    break;
                case 7:
                    result = planPlantIndividualCapacity == null ? 0 : planPlantIndividualCapacity.HoursCapacity7 == null ? 0 : planPlantIndividualCapacity.HoursCapacity7.Value;
                    break;
                case 8:
                    result = planPlantIndividualCapacity == null ? 0 : planPlantIndividualCapacity.HoursCapacity8 == null ? 0 : planPlantIndividualCapacity.HoursCapacity8.Value;
                    break;
                case 9:
                    result = planPlantIndividualCapacity == null ? 0 : planPlantIndividualCapacity.HoursCapacity9 == null ? 0 : planPlantIndividualCapacity.HoursCapacity9.Value;
                    break;
                case 10:
                    result = planPlantIndividualCapacity == null ? 0 : planPlantIndividualCapacity.HoursCapacity10 == null ? 0 : planPlantIndividualCapacity.HoursCapacity10.Value;
                    break;
            }

            return result;
        }

        private void DeleteExistingProdEntryFromAssignment(ExePlantWorkerAssignment dbExistingWorkerAssignment, ExePlantWorkerAssignmentDTO inputWorkerAssignment)
        {
            for (var date = dbExistingWorkerAssignment.StartDate; date <= dbExistingWorkerAssignment.EndDate; date = date.AddDays(1))
            {
                var mstGenWeek = _masterDataBll.GetWeekByDate(date);
                var week = mstGenWeek == null ? 0 : mstGenWeek.Week == null ? 0 : mstGenWeek.Week.Value;
                var year = mstGenWeek == null ? 0 : mstGenWeek.Year == null ? 0 : mstGenWeek.Year.Value;

                var sourceCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                            Enums.CombineCode.EBL,
                            dbExistingWorkerAssignment.SourceLocationCode,
                            dbExistingWorkerAssignment.SourceShift,
                            dbExistingWorkerAssignment.SourceUnitCode,
                            dbExistingWorkerAssignment.SourceGroupCode,
                            dbExistingWorkerAssignment.SourceBrandCode,
                            year,
                            week,
                            (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var destCombineCodeDummy = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                           Enums.CombineCode.EBL,
                           dbExistingWorkerAssignment.DestinationLocationCode,
                           dbExistingWorkerAssignment.SourceShift,
                           dbExistingWorkerAssignment.DestinationUnitCode,
                           dbExistingWorkerAssignment.DestinationGroupCodeDummy,
                           dbExistingWorkerAssignment.DestinationBrandCode,
                           year,
                           week,
                           (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var existingProdEntrySourceGroup = _exePlantProductionEntryRepo.GetByID(sourceCombineCode, dbExistingWorkerAssignment.EmployeeID);

                if (existingProdEntrySourceGroup != null) _exePlantProductionEntryRepo.Delete(existingProdEntrySourceGroup);

                var existingProdEntryDummyGroup = _exePlantProductionEntryRepo.GetByID(destCombineCodeDummy, dbExistingWorkerAssignment.EmployeeID);

                if (existingProdEntryDummyGroup != null) _exePlantProductionEntryRepo.Delete(existingProdEntryDummyGroup);

                var existingProdEntryVerifikasiDummyGroup = _exePlantProductionEntryVerificationRepo.GetByID(destCombineCodeDummy);

                if (existingProdEntryVerifikasiDummyGroup != null && !existingProdEntryVerifikasiDummyGroup.ExePlantProductionEntries.Any())
                    _exePlantProductionEntryVerificationRepo.Delete(existingProdEntryVerifikasiDummyGroup);

                var deleteDateRemoval = _workerAssignmentRemovalRepository.Get(c => c.ProductionEntryCode == destCombineCodeDummy
                                                                                && c.EmployeeID == dbExistingWorkerAssignment.EmployeeID
                                                                                && c.StartDate == dbExistingWorkerAssignment.StartDate
                                                                                && c.EndDate == dbExistingWorkerAssignment.EndDate);
                if (deleteDateRemoval.Any())
                {
                    var removalToDelete = deleteDateRemoval.Where(c => c.DeleteDate >= inputWorkerAssignment.EndDate);
                    foreach (var workerAssignmentRemoval in removalToDelete)
                    {
                        if (workerAssignmentRemoval != null)
                        {
                            _workerAssignmentRemovalRepository.Delete(workerAssignmentRemoval);
                            _uow.SaveChanges();
                        }

                    }
                }
            }
            _uow.SaveChanges();
        }

        public ExePlantWorkerAssignmentDTO UpdateWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignment)
        {
            // get existing data to be edited
            var dbExePlantWorkerAssignment = _exePlantWorkerAssignmentRepo.GetByID(workerAssignment.OldEmployeeID, workerAssignment.OldStartDate);

            if (dbExePlantWorkerAssignment == null) throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            // check if existing primary key (oldEmployeeId and odlStartdate) different from new edit value
            if (workerAssignment.EmployeeID != workerAssignment.OldEmployeeID || workerAssignment.StartDate != workerAssignment.OldStartDate)
            {
                // delete old prod entry, prod entry verification dummy group, prod entry dummy group
                DeleteExistingProdEntryFromAssignment(dbExePlantWorkerAssignment, workerAssignment);

                // insert new worker assignment
                InsertWorkerAssignment(workerAssignment);

                // delete existing worker assignment
                _exePlantWorkerAssignmentRepo.Delete(dbExePlantWorkerAssignment);

                // detele existing worker absenteeism
                var existingWorkerAbsenteeism = _exePlantWorkerAbsenteesimRepo.GetByID(dbExePlantWorkerAssignment.StartDate,
                    dbExePlantWorkerAssignment.EmployeeID, dbExePlantWorkerAssignment.SourceShift);

                if (existingWorkerAbsenteeism != null)
                    _exePlantWorkerAbsenteesimRepo.Delete(existingWorkerAbsenteeism);

                _uow.SaveChanges();

                return Mapper.Map<ExePlantWorkerAssignmentDTO>(dbExePlantWorkerAssignment);
            }

            //validate update
            ValidateUpdateWorkerAssignment(workerAssignment);

            // delete old prod entry, prod entry verification dummy group, prod entry dummy group
            DeleteExistingProdEntryFromAssignment(dbExePlantWorkerAssignment, workerAssignment);

            workerAssignment.CreatedDate = dbExePlantWorkerAssignment.CreatedDate;
            workerAssignment.CreatedBy = dbExePlantWorkerAssignment.CreatedBy;

            Mapper.Map(workerAssignment, dbExePlantWorkerAssignment);

            dbExePlantWorkerAssignment.UpdatedDate = DateTime.Now;
            dbExePlantWorkerAssignment.DestinationGroupCodeDummy = workerAssignment.DestinationGroupCode != null ? workerAssignment.DestinationGroupCode.Remove(1, 1).Insert(1, "5") : null;

            _exePlantWorkerAssignmentRepo.Update(dbExePlantWorkerAssignment);

            //var workerAbsenteeismExisting = _exePlantWorkerAbsenteesimRepo.GetByID(workerAssignment.StartDate, workerAssignment.EmployeeID, workerAssignment.SourceShift);

            //workerAbsenteeismExisting.StartDateAbsent = workerAssignment.StartDate;
            //workerAbsenteeismExisting.EndDateAbsent = workerAssignment.EndDate;

            //// update worker absenteeism
            //_exePlantWorkerAbsenteesimRepo.Update(workerAbsenteeismExisting);

            var absentType = _mstPlantAbsentType.GetByID(EnumHelper.GetDescription(Enums.SKTAbsentCode.MO));

            string remark;
            if (absentType.Remark != null && absentType.Remark.Length > 8)
            {
                remark = absentType.Remark.Substring(0, 8);
            }
            else
            {
                remark = absentType.Remark;
            }

            for (DateTime date = workerAssignment.StartDate; date <= workerAssignment.EndDate; date = date.AddDays(1))
            {
                var mstGenWeek = _masterDataBll.GetWeekByDate(date);
                var week = mstGenWeek == null ? 0 : mstGenWeek.Week == null ? 0 : mstGenWeek.Week.Value;
                var year = mstGenWeek == null ? 0 : mstGenWeek.Year == null ? 0 : mstGenWeek.Year.Value;

                var sourceCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                            Enums.CombineCode.EBL,
                            dbExePlantWorkerAssignment.SourceLocationCode,
                            dbExePlantWorkerAssignment.SourceShift,
                            dbExePlantWorkerAssignment.SourceUnitCode,
                            dbExePlantWorkerAssignment.SourceGroupCode,
                            dbExePlantWorkerAssignment.SourceBrandCode,
                            year,
                            week,
                            (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var destCombineCodeDummy = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                            Enums.CombineCode.EBL,
                            dbExePlantWorkerAssignment.DestinationLocationCode,
                            dbExePlantWorkerAssignment.SourceShift,
                            dbExePlantWorkerAssignment.DestinationUnitCode,
                            dbExePlantWorkerAssignment.DestinationGroupCodeDummy,
                            dbExePlantWorkerAssignment.DestinationBrandCode,
                            year,
                            week,
                            (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var destGroupCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                           Enums.CombineCode.EBL,
                           dbExePlantWorkerAssignment.DestinationLocationCode,
                           dbExePlantWorkerAssignment.SourceShift,
                           dbExePlantWorkerAssignment.DestinationUnitCode,
                           dbExePlantWorkerAssignment.DestinationGroupCode,
                           dbExePlantWorkerAssignment.DestinationBrandCode,
                           year,
                           week,
                           (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var destProdEntryVef = _exePlantProductionEntryVerificationRepo.GetByID(destGroupCombineCode);

                var sourceProdEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(sourceCombineCode);

                if (sourceProdEntryVerification != null)
                {
                    var sourceProdEntry = _exePlantProductionEntryRepo.GetByID(sourceCombineCode, dbExePlantWorkerAssignment.EmployeeID);

                    if (sourceProdEntry != null)
                    {
                        sourceProdEntry.AbsentType = EnumHelper.GetDescription(Enums.SKTAbsentCode.MO);
                        sourceProdEntry.AbsentCodeEblek = absentType.SktAbsentCode;
                        sourceProdEntry.AbsentCodePayroll = absentType.PayrollAbsentCode;
                        _exePlantProductionEntryRepo.Update(sourceProdEntry);
                    }
                    else
                    {
                        var newDestProductionEntry = new ExePlantProductionEntry()
                        {
                            ProductionEntryCode = destCombineCodeDummy,
                            EmployeeID = dbExePlantWorkerAssignment.EmployeeID,
                            EmployeeNumber = _masterDataBll.GetMstEmployeeJobsDataActives(dbExePlantWorkerAssignment.EmployeeID).EmployeeNumber,
                            AbsentType = absentType.AbsentType,
                            AbsentCodePayroll = absentType.PayrollAbsentCode,
                            AbsentCodeEblek = absentType.SktAbsentCode,
                            AbsentRemark = remark,
                            StartDateAbsent = dbExePlantWorkerAssignment.StartDate,
                            StatusEmp = dbExePlantWorkerAssignment.SourceProcessGroup.ToLower() == dbExePlantWorkerAssignment.DestinationProcessGroup.ToLower() ? Enums.StatusEmp.Resmi.ToString() : Enums.StatusEmp.Multiskill.ToString(),
                            CreatedBy = dbExePlantWorkerAssignment.CreatedBy,
                            CreatedDate = dbExePlantWorkerAssignment.CreatedDate,
                            UpdatedBy = dbExePlantWorkerAssignment.UpdatedBy,
                            UpdatedDate = dbExePlantWorkerAssignment.UpdatedDate,
                            ProdCapacity = GetProdCapacityWorkerAssignment(workerAssignment, destProdEntryVef == null ? 0 : destProdEntryVef.WorkHour)
                        };

                        newDestProductionEntry.StatusIdentifier =
                            Convert.ToInt32(
                                _masterDataBll.GetGenEmpStatusIdentifierByStatusEmp(newDestProductionEntry.StatusEmp));

                        _exePlantProductionEntryRepo.Insert(newDestProductionEntry);
                    }

                    //check WorkerAssignment Removal Destination/Dummy Group
                    var listExistingWorkerAssignmentRemovalSource = _workerAssignmentRemovalRepository.Get(c => c.ProductionEntryCode == sourceCombineCode
                                                                                                      && c.EmployeeID == dbExePlantWorkerAssignment.EmployeeID
                                                                                                      && c.StartDate == dbExePlantWorkerAssignment.StartDate
                                                                                                      && c.EndDate == dbExePlantWorkerAssignment.EndDate).ToList();

                    //Delete Assignment Removal if Exist in range start date and end date
                    if (listExistingWorkerAssignmentRemovalSource.Any())
                    {
                        foreach (var workerAssignmentRemoval in listExistingWorkerAssignmentRemovalSource)
                            _workerAssignmentRemovalRepository.Delete(workerAssignmentRemoval);
                    }

                    //get destination production entry verification
                    var destProdEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(destCombineCodeDummy);

                    if (destProdEntryVerification == null)
                    {
                        var newDestProdEntryVerification = new ExePlantProductionEntryVerification
                        {
                            ProductionEntryCode = destCombineCodeDummy,
                            LocationCode = dbExePlantWorkerAssignment.DestinationLocationCode,
                            UnitCode = dbExePlantWorkerAssignment.DestinationUnitCode,
                            Shift = dbExePlantWorkerAssignment.SourceShift == null ? 0 : dbExePlantWorkerAssignment.SourceShift.Value,
                            ProcessGroup = dbExePlantWorkerAssignment.DestinationProcessGroup,
                            ProcessOrder = _mstGenProcess.GetByID(dbExePlantWorkerAssignment.DestinationProcessGroup).ProcessOrder,
                            GroupCode = dbExePlantWorkerAssignment.DestinationGroupCodeDummy,
                            BrandCode = dbExePlantWorkerAssignment.DestinationBrandCode,
                            KPSYear = Convert.ToInt32(destCombineCodeDummy.Split('/')[6]),
                            KPSWeek = Convert.ToInt32(destCombineCodeDummy.Split('/')[7]),
                            ProductionDate = date,
                            WorkHour = sourceProdEntryVerification.WorkHour,
                            TPKValue = sourceProdEntryVerification.TPKValue,
                            TotalTargetValue = sourceProdEntryVerification.TotalTargetValue,
                            TotalActualValue = sourceProdEntryVerification.TotalActualValue,
                            TotalCapacityValue = sourceProdEntryVerification.TotalCapacityValue,
                            VerifyManual = sourceProdEntryVerification.VerifyManual,
                            VerifySystem = sourceProdEntryVerification.VerifySystem,
                            Flag_Manual = sourceProdEntryVerification.Flag_Manual,
                            Remark = sourceProdEntryVerification.Remark,
                            CreatedBy = dbExePlantWorkerAssignment.CreatedBy,
                            CreatedDate = dbExePlantWorkerAssignment.CreatedDate,
                            UpdatedBy = dbExePlantWorkerAssignment.UpdatedBy,
                            UpdatedDate = dbExePlantWorkerAssignment.UpdatedDate,
                        };

                        _exePlantProductionEntryVerificationRepo.Insert(newDestProdEntryVerification);
                        _uow.SaveChanges();
                    }

                    //get destination production entry
                    var destProdEntry = _exePlantProductionEntryRepo.GetByID(destCombineCodeDummy, dbExePlantWorkerAssignment.EmployeeID);

                    if (destProdEntry != null)
                    {
                        destProdEntry.AbsentType = EnumHelper.GetDescription(Enums.SKTAbsentCode.MO);
                        destProdEntry.AbsentCodeEblek = absentType.SktAbsentCode;
                        destProdEntry.AbsentCodePayroll = absentType.PayrollAbsentCode;
                        _exePlantProductionEntryRepo.Update(destProdEntry);
                    }
                    else
                    {
                        var newDestProductionEntry = new ExePlantProductionEntry()
                        {
                            ProductionEntryCode = destCombineCodeDummy,
                            EmployeeID = dbExePlantWorkerAssignment.EmployeeID,
                            EmployeeNumber = _masterDataBll.GetMstEmployeeJobsDataActives(dbExePlantWorkerAssignment.EmployeeID).EmployeeNumber,
                            AbsentType = absentType.AbsentType,
                            AbsentCodePayroll = absentType.PayrollAbsentCode,
                            AbsentCodeEblek = absentType.SktAbsentCode,
                            AbsentRemark = remark,
                            StartDateAbsent = dbExePlantWorkerAssignment.StartDate,
                            StatusEmp = dbExePlantWorkerAssignment.SourceProcessGroup.ToLower() == dbExePlantWorkerAssignment.DestinationProcessGroup.ToLower() ? Enums.StatusEmp.Resmi.ToString() : Enums.StatusEmp.Multiskill.ToString(),
                            CreatedBy = dbExePlantWorkerAssignment.CreatedBy,
                            CreatedDate = dbExePlantWorkerAssignment.CreatedDate,
                            UpdatedBy = dbExePlantWorkerAssignment.UpdatedBy,
                            UpdatedDate = dbExePlantWorkerAssignment.UpdatedDate,
                            ProdCapacity = GetProdCapacityWorkerAssignment(workerAssignment, destProdEntryVef == null ? 0 : destProdEntryVef.WorkHour)
                        };

                        newDestProductionEntry.StatusIdentifier =
                            Convert.ToInt32(
                                _masterDataBll.GetGenEmpStatusIdentifierByStatusEmp(newDestProductionEntry.StatusEmp));

                        _exePlantProductionEntryRepo.Insert(newDestProductionEntry);
                    }


                    //check WorkerAssignment Removal Destination/Dummy Group
                    var listExistingWorkerAssignmentRemovalDest = _workerAssignmentRemovalRepository.Get(c => c.ProductionEntryCode == destCombineCodeDummy
                                                                                                      && c.EmployeeID == dbExePlantWorkerAssignment.EmployeeID
                                                                                                      && c.StartDate == dbExePlantWorkerAssignment.StartDate
                                                                                                      && c.EndDate == dbExePlantWorkerAssignment.EndDate).ToList();

                    //Delete Assignment Removal if Exist in range start date and end date
                    if (listExistingWorkerAssignmentRemovalDest.Any())
                    {
                        foreach (var workerAssignmentRemoval in listExistingWorkerAssignmentRemovalDest)
                            _workerAssignmentRemovalRepository.Delete(workerAssignmentRemoval);
                    }
                }

            }

            _uow.SaveChanges();

            return Mapper.Map<ExePlantWorkerAssignmentDTO>(dbExePlantWorkerAssignment);
        }

        private void ValidateInsertWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignmentToValidate)
        {
            //validate existing data
            ValidateExistingPlantWorkerAssignmentOnInsert(workerAssignmentToValidate);

            //validate end date
            ValidateEndDatePlantWorkerAssignment(workerAssignmentToValidate);

            //validate date range
            ValidateDateRangePlantWorkerAssignment(workerAssignmentToValidate);

            //validate check existing source entry production except for DAILY
            //ValidateProductionSourceIsExist(workerAssignmentToValidate);
        }

        private void ValidateProductionSourceIsExist(ExePlantWorkerAssignmentDTO workerAssignmentToValidate) 
        {
            if (workerAssignmentToValidate.SourceProcessGroup != Enums.Process.Daily.ToString().ToUpper())
            {
                for (DateTime date = workerAssignmentToValidate.StartDate; date <= workerAssignmentToValidate.EndDate; date = date.AddDays(1))
                {
                    var mstGenWeek = _masterDataBll.GetWeekByDate(date);
                    var week = mstGenWeek == null ? 0 : mstGenWeek.Week == null ? 0 : mstGenWeek.Week.Value;
                    var year = mstGenWeek == null ? 0 : mstGenWeek.Year == null ? 0 : mstGenWeek.Year.Value;

                    var sourceCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                                Enums.CombineCode.EBL,
                                workerAssignmentToValidate.SourceLocationCode,
                                workerAssignmentToValidate.SourceShift,
                                workerAssignmentToValidate.SourceUnitCode,
                                workerAssignmentToValidate.SourceGroupCode,
                                workerAssignmentToValidate.SourceBrandCode,
                                year,
                                week,
                                (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                    var sourceProdEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(sourceCombineCode);

                    if (sourceProdEntryVerification == null)
                        throw new BLLException(ExceptionCodes.BLLExceptions.SourceProductionEntryVerificationNULL);
                }
            }
        }
        private void ValidateUpdateWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignmentToValidate)
        {
            //validate end date
            ValidateEndDatePlantWorkerAssignment(workerAssignmentToValidate);
        }

        private void ValidateExistingPlantWorkerAssignmentOnInsert(ExePlantWorkerAssignmentDTO workerAssignmentToValidate)
        {
            var dbExePlantWorkerAssignment = _exePlantWorkerAssignmentRepo.GetByID(workerAssignmentToValidate.EmployeeID, workerAssignmentToValidate.StartDate);

            if (dbExePlantWorkerAssignment != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        private void ValidateEndDatePlantWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignmentToValidate)
        {
            if (workerAssignmentToValidate.EndDate.Date < workerAssignmentToValidate.StartDate.Date)
                throw new BLLException(ExceptionCodes.BLLExceptions.EndDateLessThanStartDate);
        }

        private void ValidateDateRangePlantWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignmentToValidate)
        {
            //worker assignment
            var existingPlantWorkerAssignment =
                _exePlantWorkerAssignmentRepo.Get(m => m.EmployeeID == workerAssignmentToValidate.EmployeeID);

            if (existingPlantWorkerAssignment.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(workerAssignmentToValidate.StartDate,
                workerAssignmentToValidate.EndDate, existingPlantWorkerAbsenteeism.StartDate,
                existingPlantWorkerAbsenteeism.EndDate)))
            {
                var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnionWorkerAssignment),
                        workerAssignmentToValidate.StartDate.Date, workerAssignmentToValidate.EndDate.Date);
                throw new BLLException(errorCode, errorMessage);
            }

            // check absenteeism
            var existingAbsenteeism =
                _exePlantWorkerAbsenteesimRepo.Get(m => m.EmployeeID == workerAssignmentToValidate.EmployeeID);

            if (existingAbsenteeism.Any(c => !DataRangeIsNotUnionOrOverlap(workerAssignmentToValidate.StartDate,
                workerAssignmentToValidate.EndDate,
                c.StartDateAbsent,
                c.EndDateAbsent)))
            {
                var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                var errorMessage =
                    string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnionProductionEntry),
                        workerAssignmentToValidate.StartDate.Date, workerAssignmentToValidate.EndDate.Date);
                throw new BLLException(errorCode, errorMessage);
            }
        }

        private void ValidateDateRangePlantWorkerAssignmentCompleteParam(ExePlantWorkerAssignmentDTO workerAssignmentToValidate)
        {
            //worker assignment
            var existingPlantWorkerAssignment =
                _exePlantWorkerAssignmentRepo.Get(m => m.EmployeeID == workerAssignmentToValidate.EmployeeID
                && m.DestinationLocationCode == workerAssignmentToValidate.DestinationLocationCode
                && m.DestinationUnitCode == workerAssignmentToValidate.DestinationUnitCode
                && m.DestinationProcessGroup == workerAssignmentToValidate.DestinationProcessGroup
                && m.DestinationGroupCode == workerAssignmentToValidate.DestinationGroupCode
                && m.DestinationBrandCode == workerAssignmentToValidate.DestinationBrandCode
                && m.StartDate < workerAssignmentToValidate.OldStartDate
                && m.EndDate > workerAssignmentToValidate.OldEndDate
                );
            if (existingPlantWorkerAssignment.Count() > 0)
            {
                if (existingPlantWorkerAssignment.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(workerAssignmentToValidate.StartDate,
                workerAssignmentToValidate.EndDate, existingPlantWorkerAbsenteeism.StartDate,
                existingPlantWorkerAbsenteeism.EndDate)))
                {
                    var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                    var errorMessage =
                        string.Format(EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnionWorkerAssignment),
                            workerAssignmentToValidate.StartDate.Date, workerAssignmentToValidate.EndDate.Date);
                    throw new BLLException(errorCode, errorMessage);
                }
            }
            

            // check absenteeism
            var existingAbsenteeism =
                _exePlantWorkerAbsenteesimRepo.Get(m => m.EmployeeID == workerAssignmentToValidate.EmployeeID
                && m.StartDateAbsent >= workerAssignmentToValidate.StartDate
                && m.EndDateAbsent <= workerAssignmentToValidate.EndDate
                );

            if (existingAbsenteeism.Count() > 0)
            {
                if (existingAbsenteeism.Any(c => !DataRangeIsNotUnionOrOverlap(workerAssignmentToValidate.StartDate,
                    workerAssignmentToValidate.EndDate,
                    c.StartDateAbsent,
                    c.EndDateAbsent)))
                {
                    var errorCode = ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnion;
                    var errorMessage =
                        string.Format(
                            EnumHelper.GetDescription(
                                ExceptionCodes.BLLExceptions.RangeDateOverlapOrUnionProductionEntry),
                            workerAssignmentToValidate.StartDate.Date, workerAssignmentToValidate.EndDate.Date);
                    throw new BLLException(errorCode, errorMessage);
                }
            }
        }

        private ExePlantProductionEntryVerificationDTO GetEntryVerificationPopUpGroupCode(GetGroupCodePopUpWorkerAssignmentInput input, string groupCode)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);

            if (!string.IsNullOrEmpty(input.ProcessCode))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.ProcessCode);

            queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(m.ProductionDate) == DbFunctions.TruncateTime(input.Date));

            queryFilter = queryFilter.And(m => m.GroupCode == groupCode);

            var entryVerification = _exePlantProductionEntryVerificationRepo.Get(queryFilter).FirstOrDefault();

            return Mapper.Map<ExePlantProductionEntryVerificationDTO>(entryVerification);
        }

        public List<ExePlantWorkerAssignmentDTO> IsExistWorkerAssignment(GetExePlantWorkerAssignmentInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantWorkerAssignment>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.SourceLocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.SourceUnitCode == input.UnitCode);
            if (input.Shift > 0)
                queryFilter = queryFilter.And(m => m.SourceShift == input.Shift);
            if (input.Date.HasValue && input.DateTypeFilter == "rdbDate")
                queryFilter = queryFilter.And(m => m.TransactionDate == input.Date);
            if (input.ProductionDateFrom.HasValue && input.DateTypeFilter == "rdbProductionDate")
                queryFilter = queryFilter.And(m => m.StartDate >= input.ProductionDateFrom);
            if (input.ProductionDateTo.HasValue && input.DateTypeFilter == "rdbProductionDate")
                queryFilter = queryFilter.And(m => m.EndDate <= input.ProductionDateTo);
            if (!string.IsNullOrEmpty(input.SourceBrandCode))
                queryFilter = queryFilter.And(m => m.SourceBrandCode == input.SourceBrandCode);

            var existingWorkerAssignment = _exePlantWorkerAssignmentRepo.Get(queryFilter).ToList();
            return Mapper.Map<List<ExePlantWorkerAssignmentDTO>>(existingWorkerAssignment);

            //if (existingWorkerAssignment.Any())
            //    return true;

            //return false;
        }

        private List<ExePlantWorkerAssignmentDTO> IsExistWorkerAssignmentByDestination(GetExePlantWorkerAssignmentInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantWorkerAssignment>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.DestinationLocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.DestinationUnitCode == input.UnitCode);
            if (input.Shift > 0)
                queryFilter = queryFilter.And(m => m.DestinationShift == input.Shift);
            if (input.Date.HasValue && input.DateTypeFilter == "rdbDate")
                queryFilter = queryFilter.And(m => m.TransactionDate == input.Date);
            if (input.ProductionDateFrom.HasValue && input.DateTypeFilter == "rdbProductionDate")
                queryFilter = queryFilter.And(m => m.StartDate >= input.ProductionDateFrom);
            if (input.ProductionDateTo.HasValue && input.DateTypeFilter == "rdbProductionDate")
                queryFilter = queryFilter.And(m => m.EndDate <= input.ProductionDateTo);
            if (!string.IsNullOrEmpty(input.SourceBrandCode))
                queryFilter = queryFilter.And(m => m.DestinationBrandCode == input.SourceBrandCode);

            var existingWorkerAssignment = _exePlantWorkerAssignmentRepo.Get(queryFilter).ToList();
            return Mapper.Map<List<ExePlantWorkerAssignmentDTO>>(existingWorkerAssignment);

            //if (existingWorkerAssignment.Any())
            //    return true;

            //return false;
        }

        public bool IsExistWorkerAbsenteeismByWorkerAssignment(GetExePlantWorkerAbsenteeismInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantWorkerAbsenteeism>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            if (!string.IsNullOrEmpty(input.GroupCode))
                queryFilter = queryFilter.And(m => m.GroupCode == input.GroupCode);

            queryFilter = queryFilter.And(m => m.StartDateAbsent == input.StartDateAbsent);
            queryFilter = queryFilter.And(m => m.EmployeeID == input.EmployeeID);

            var existingWorkerAbsenteeism = _exePlantWorkerAbsenteesimRepo.Get(queryFilter).ToList();

            if (!existingWorkerAbsenteeism.Any())
                return false;

            var result = true;

            foreach (var workerAbsent in existingWorkerAbsenteeism)
            {
                if (!DataRangeIsNotUnionOrOverlap(input.StartDateAbsent, input.EndDateAbsent,
                workerAbsent.StartDateAbsent, workerAbsent.EndDateAbsent)) ;
                result = false;
            }

            return result;
        }

        public List<string> GetGroupCodePopUpDaily(string locationCode, string unitCode, string processCode)
        {
            var queryFilter = PredicateHelper.True<MstPlantEmpJobsDataAcv>();
            queryFilter = queryFilter.And(x => x.Status == "4");
            queryFilter = queryFilter.And(x => x.LocationCode == locationCode);
            queryFilter = queryFilter.And(x => x.UnitCode == unitCode);
            var mstJobsData = _mstPlantEmpJobsdataAcvRepo.Get(queryFilter);
            return mstJobsData.OrderBy(x => x.GroupCode).Select(x => x.GroupCode).Distinct().ToList();
        }

        public List<GroupCodesPopUpDTO> GetGroupCodePopUp(GetGroupCodePopUpWorkerAssignmentInput input)
        {
            var listGroupCodesPopUp = new List<GroupCodesPopUpDTO>();
            var listGroupCode = new List<string>();
            var dateTimeStr = DateTime.Parse("01/01/0001");
            if (DateTime.Parse((input.Date).ToString("dd/MM/yyyy")) == dateTimeStr)
            {
                input.Date = DateTime.Parse((DateTime.Now).ToString("dd/MM/yyyy"));
            }
            
            //input.Date = DateTime.Parse("01-08-2016");
            if (input.ProcessCode != "DAILY")
            {
                var shift = input.Shift.HasValue ? input.Shift.Value : 1;
                //var listVerifications = GetVerificationForFilterGroupCode(input.LocationCode, input.UnitCode, shift, input.ProcessCode, input.Date);

                //var realOnly = new List<ExePlantProductionEntryVerificationViewDTO>();
                //foreach (var item in listVerifications)
                //{
                //    var sub = item.GroupCode.Substring(1, 1);
                //    if (sub != "5")
                //    {
                //        realOnly.Add(item);
                //    }
                //}
                //// get list group code
                listGroupCode = GetGroupCodePlantVerification(input.LocationCode, input.UnitCode, input.ProcessCode, input.KPSWeek);
                //listGroupCode = _masterDataBll.GetMasterPlantProductionGroups(new GetMstPlantProductionGroupsInput
                //{
                //    LocationCode = input.LocationCode,
                //    UnitCode = input.UnitCode,
                //    ProcessSettingsCode = input.ProcessCode
                //}).Select(c => c.GroupCode).ToList();
            }
            else
            {
                listGroupCode = GetGroupCodePopUpDaily(input.LocationCode,input.UnitCode,input.ProcessCode);
            }
            
            // create group code pop up list by GroupCodesPopUpDTO
            foreach (var groupCode in listGroupCode)
            {
                int day = input.Date.DayOfWeek == 0 ? 7 : (int)input.Date.DayOfWeek;
                var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                          + input.LocationCode + "/"
                                          + input.Shift + "/"
                                          + input.UnitCode + "/"
                                          + groupCode + "/"
                                          + input.BrandCode + "/"
                                          + input.KPSYear + "/"
                                          + input.KPSWeek + "/"
                                          + day;

                var resultAttendanceAndAllocation = CalculateAttendanceAndAllocation(productionEntryCode);

                listGroupCodesPopUp.Add(new GroupCodesPopUpDTO
                {
                    GroupCode = groupCode,
                    Attendance = resultAttendanceAndAllocation[1],
                    Allocation = resultAttendanceAndAllocation[0],
                });
            }

            return listGroupCodesPopUp;
        }

        private int[] CalculateAttendanceAndAllocation(string productionCode)
        {
            var result = new int[2];
            
            var allocation = 0;
            var attendance = 0;

            // get production entry verification
            var entryVerification = _exePlantProductionEntryVerificationRepo.GetByID(productionCode);

            // calculate row count employee ID from production entry
            if (entryVerification != null) 
            {
                if (entryVerification.ExePlantProductionEntries.Any()) 
                {
                    allocation = entryVerification.ExePlantProductionEntries.Count(c => c.AbsentType == null);
                    attendance = allocation - entryVerification.ExePlantProductionEntries.Count(c => c.AbsentType != null);
                }
            }
            
            result[0] = allocation;
            result[1] = attendance;

            return result;
        }

        public List<string> GetBrandFromPlantEntryVerification(string locationCode, string unitCode, int? shift)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(locationCode)) queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(unitCode)) queryFilter = queryFilter.And(m => m.UnitCode == unitCode);
            if (shift != null) queryFilter = queryFilter.And(m => m.Shift == shift);

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);

            return dbResult.Select(c => c.BrandCode).Distinct().ToList();
        }

        public List<string> GetBrandFromPlantEntryVerificationByDate(string locationCode, string unitCode, int? shift, int? year, int? week, DateTime? date)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(locationCode)) queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(unitCode)) queryFilter = queryFilter.And(m => m.UnitCode == unitCode);
            if (shift != null) queryFilter = queryFilter.And(m => m.Shift == shift);
            if (year != null) queryFilter = queryFilter.And(m => m.KPSYear == year);
            if (week != null) queryFilter = queryFilter.And(m => m.KPSWeek == week);
            if (date != null) queryFilter = queryFilter.And(m => m.ProductionDate == date);

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);

            return dbResult.Select(c => c.BrandCode).Distinct().ToList();
        }

        public List<string> GetBrandFromPlantTpu(string locationCode, string unitCode, int? shift, int? year, int? week)
        {
            var queryFilter = PredicateHelper.True<PlanTargetProductionUnit>();

            if (!string.IsNullOrEmpty(locationCode)) queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(unitCode)) queryFilter = queryFilter.And(m => m.UnitCode == unitCode);
            if (shift != null) queryFilter = queryFilter.And(m => m.Shift == shift);
            if (year != null) queryFilter = queryFilter.And(m => m.KPSYear == year);
            if (week != null) queryFilter = queryFilter.And(m => m.KPSWeek == week);

            var dbResult = _exePlantPlanTargetProductionUnitRepo.Get(queryFilter);

            return dbResult.Select(c => c.BrandCode).Distinct().ToList();
        }

        public void DeletetWorkerAssignment(ExePlantWorkerAssignmentDTO workerAssignment)
        {
            //1. delete from table ExePlantWorkerAssignment
            var dbWorker = _exePlantWorkerAssignmentRepo.GetByID(workerAssignment.OldEmployeeID, workerAssignment.OldStartDate);
            if (dbWorker == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound); 

            //2. delete from exeplantproductionentryverification dan exeplantproductionentry 
            //berdasarkan group dummy dan destination
            for (DateTime date = dbWorker.StartDate;
                date <= dbWorker.EndDate;
                date = date.AddDays(1))
            {
                var mstGenWeek = _masterDataBll.GetWeekByDate(date);
                var week = mstGenWeek == null ? 0 : mstGenWeek.Week == null ? 0 : mstGenWeek.Week.Value;
                var year = mstGenWeek == null ? 0 : mstGenWeek.Year == null ? 0 : mstGenWeek.Year.Value;

                var destCombineCodeDummy = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                         Enums.CombineCode.EBL,
                         dbWorker.DestinationLocationCode,
                         dbWorker.SourceShift,
                         dbWorker.DestinationUnitCode,
                         dbWorker.DestinationGroupCodeDummy,
                         dbWorker.DestinationBrandCode,
                         year,
                         week,
                         (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var destProdEntry = _exePlantProductionEntryRepo.GetByID(destCombineCodeDummy, dbWorker.EmployeeID);
                if (destProdEntry != null)
                    _exePlantProductionEntryRepo.Delete(destProdEntry);

                var destProdEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(destCombineCodeDummy);
                if (destProdEntryVerification != null)
                    _exePlantProductionEntryVerificationRepo.Delete(destProdEntryVerification);


                //set field AbsentType = null in table exeplantproductionentry
                //berdasarkan source production code
                var sourceCombineCode = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                           Enums.CombineCode.EBL,
                           dbWorker.SourceLocationCode,
                           dbWorker.SourceShift,
                           dbWorker.SourceUnitCode,
                           dbWorker.SourceGroupCode,
                           dbWorker.SourceBrandCode,
                           year,
                           week,
                           (int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);

                var sourceProdEntry = _exePlantProductionEntryRepo.GetByID(sourceCombineCode, dbWorker.EmployeeID);
                if (sourceProdEntry != null)
                {
                    sourceProdEntry.AbsentType = null;
                    sourceProdEntry.AbsentCodeEblek = null;
                    sourceProdEntry.AbsentCodePayroll = null;

                    sourceProdEntry.UpdatedDate = DateTime.Now;
                    sourceProdEntry.UpdatedBy = workerAssignment.UpdatedBy;

                    _exePlantProductionEntryRepo.Update(sourceProdEntry);
                }
            }

            //keep in the last, cause the data is used
            _exePlantWorkerAssignmentRepo.Delete(dbWorker);

            //
            _uow.SaveChanges();

        }

        public ExePlantWorkerAssignmentDTO InsertWorkerAssignment_SP(ExePlantWorkerAssignmentDTO workerAssignment)
        {
            // validate insert
            ValidateInsertWorkerAssignment(workerAssignment);

            var dbExePlantWorkerAssignment = Mapper.Map<ExePlantWorkerAssignment>(workerAssignment);

            _sqlSPRepo.InsertWorkerAssignment_SP(dbExePlantWorkerAssignment);

            return workerAssignment;
        }

        public ExePlantWorkerAssignmentDTO UpdateWorkerAssignment_SP(ExePlantWorkerAssignmentDTO workerAssignment)
        {
            //validate end date
            ValidateEndDatePlantWorkerAssignment(workerAssignment);

            //validate date range
            ValidateDateRangePlantWorkerAssignmentCompleteParam(workerAssignment);   

            using (SKTISEntities context = new SKTISEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        //1. delete from table ExePlantWorkerAssignment
                        var dbWorker = _exePlantWorkerAssignmentRepo.GetByID(workerAssignment.OldEmployeeID, workerAssignment.OldStartDate);
                        if (dbWorker == null)
                            throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                        context.DELETE_WORKER_ASSIGNMENT
                        (
                            dbWorker.SourceLocationCode,
                            dbWorker.SourceUnitCode,
                            dbWorker.SourceShift,
                            dbWorker.SourceProcessGroup,
                            dbWorker.SourceGroupCode,
                            dbWorker.SourceBrandCode,
                            dbWorker.DestinationLocationCode,
                            dbWorker.DestinationUnitCode,
                            dbWorker.DestinationShift,
                            dbWorker.DestinationProcessGroup,
                            dbWorker.DestinationGroupCode,
                            dbWorker.DestinationGroupCodeDummy,
                            dbWorker.DestinationBrandCode,
                            dbWorker.EmployeeID,
                            dbWorker.EmployeeNumber,
                            dbWorker.StartDate,
                            dbWorker.EndDate,
                            dbWorker.CreatedBy,
                            dbWorker.UpdatedBy
                        );

                        context.SaveChanges();

                        context.INSERT_WORKER_ASSIGNMENT
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

                        context.SaveChanges();

                        transaction.Commit();

                        return workerAssignment;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public ExePlantWorkerAssignmentDTO DeleteWorkerAssignment_SP(ExePlantWorkerAssignmentDTO workerAssignment) 
        {
            //1. delete from table ExePlantWorkerAssignment
            var dbWorker = _exePlantWorkerAssignmentRepo.GetByID(workerAssignment.OldEmployeeID, workerAssignment.OldStartDate);
            if (dbWorker == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFoundOrDeleted);

            _sqlSPRepo.DeleteWorkerAssignment_SP(dbWorker);

            return workerAssignment;
        }

        public bool CheckEblekSubmittedOnEditAssingment(GetExePlantWorkerAssignmentInput input) 
        {
            bool result = false;

            if (input.StartDate != input.OldStartDate && input.EndDate != input.OldEndDate) 
            {
                var checkDateStartDateFrom = DateTime.Now;
                var checkDateStartDateTo = DateTime.Now;

                var checkDateEndDateFrom = DateTime.Now;
                var checkDateEndDateTo = DateTime.Now;

                if (input.StartDate > input.OldStartDate) 
                {
                    checkDateStartDateFrom = input.OldStartDate;
                    checkDateStartDateTo = input.StartDate.AddDays(-1);
                }else{
                    checkDateStartDateFrom = input.StartDate;
                    checkDateStartDateTo = input.OldStartDate.AddDays(-1);
                }

                for (DateTime date = checkDateStartDateFrom; date <= checkDateStartDateTo; date = date.AddDays(1)) {
                    int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;

                    int week = 0;
                    using (SKTISEntities context = new SKTISEntities()) 
                    {
                        week = context.MstGenWeeks.Where(c => date >= c.StartDate && date <= c.EndDate).Select(c => c.Week).FirstOrDefault().Value;
                    }

                    // source check
                    var prodEntryCodeSource = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + input.LocationSource + "/"
                                              + input.Shift + "/"
                                              + input.UnitSource + "/"
                                              + input.GroupCodeSource + "/"
                                              + input.BrandSource + "/"
                                              + date.Year + "/"
                                              + week + "/"
                                              + day;
                    result = IsEblekSubmitted(prodEntryCodeSource, Enums.PageName.PlantProductionEntry.ToString());

                    // dest check
                    var prodEntryCodeDest = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + input.LocationDest + "/"
                                              + input.Shift + "/"
                                              + input.UnitDest + "/"
                                              + input.GroupCodeDest + "/"
                                              + input.BrandDest + "/"
                                              + input.Year + "/"
                                              + week + "/"
                                              + day;
                    result = IsEblekSubmitted(prodEntryCodeDest, Enums.PageName.PlantProductionEntry.ToString());
                }

                if (input.EndDate > input.OldEndDate) 
                {
                    checkDateEndDateFrom = input.OldEndDate.AddDays(1);
                    checkDateEndDateTo = input.EndDate;
                }else{
                    checkDateEndDateFrom = input.EndDate.AddDays(1);
                    checkDateEndDateTo = input.OldEndDate;
                }

                for (DateTime date = checkDateEndDateFrom; date <= checkDateEndDateTo; date = date.AddDays(1)) {
                    int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;

                    int week = 0;
                    using (SKTISEntities context = new SKTISEntities()) {
                        week = context.MstGenWeeks.Where(c => date >= c.StartDate && date <= c.EndDate).Select(c => c.Week).FirstOrDefault().Value;
                    }

                    // source check
                    var prodEntryCodeSource = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + input.LocationSource + "/"
                                              + input.Shift + "/"
                                              + input.UnitSource + "/"
                                              + input.GroupCodeSource + "/"
                                              + input.BrandSource + "/"
                                              + date.Year + "/"
                                              + week + "/"
                                              + day;
                    result = IsEblekSubmitted(prodEntryCodeSource, Enums.PageName.PlantProductionEntry.ToString());

                    // dest check
                    var prodEntryCodeDest = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + input.LocationDest + "/"
                                              + input.Shift + "/"
                                              + input.UnitDest + "/"
                                              + input.GroupCodeDest + "/"
                                              + input.BrandDest + "/"
                                              + date.Year + "/"
                                              + week + "/"
                                              + day;
                    result = IsEblekSubmitted(prodEntryCodeDest, Enums.PageName.PlantProductionEntry.ToString());
                }
            }
            else 
            {
                var checkDateFrom = DateTime.Now;
                var checkDateTo = DateTime.Now;

                if (input.StartDate == input.OldStartDate && input.EndDate != input.OldEndDate) {
                    if (input.EndDate > input.OldEndDate) 
                    {
                        checkDateFrom = input.OldEndDate.AddDays(1);
                        checkDateTo = input.EndDate;
                    }else{
                        checkDateFrom = input.EndDate.AddDays(1);
                        checkDateTo = input.OldEndDate;
                    }
                }
                else if (input.StartDate != input.OldStartDate && input.EndDate == input.OldEndDate) {
                   if (input.StartDate > input.OldStartDate) 
                    {
                        checkDateFrom = input.OldStartDate;
                        checkDateTo = input.StartDate.AddDays(-1);
                    }else{
                        checkDateFrom = input.StartDate;
                        checkDateTo = input.OldStartDate.AddDays(-1);
                    }
                }

                for (DateTime date = checkDateFrom; date <= checkDateTo; date = date.AddDays(1)) 
                {
                    int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;

                    int week = 0;
                    using (SKTISEntities context = new SKTISEntities()) {
                        week = context.MstGenWeeks.Where(c => date >= c.StartDate && date <= c.EndDate).Select(c => c.Week).FirstOrDefault().Value;
                    }

                    // source check
                    var prodEntryCodeSource = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + input.LocationSource + "/"
                                              + input.Shift + "/"
                                              + input.UnitSource + "/"
                                              + input.GroupCodeSource + "/"
                                              + input.BrandSource + "/"
                                              + date.Year + "/"
                                              + week + "/"
                                              + day;
                    result = IsEblekSubmitted(prodEntryCodeSource, Enums.PageName.PlantProductionEntry.ToString());

                    // dest check
                    var prodEntryCodeDest = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + input.LocationDest + "/"
                                              + input.Shift + "/"
                                              + input.UnitDest + "/"
                                              + input.GroupCodeDest + "/"
                                              + input.BrandDest + "/"
                                              + date.Year + "/"
                                              + week + "/"
                                              + day;
                    result = IsEblekSubmitted(prodEntryCodeDest, Enums.PageName.PlantProductionEntry.ToString());
                }
            }

            return result;
        }

        private Tuple<DateTime, DateTime> CompareDateFromTo(DateTime date1, DateTime date2) 
        {
            if (date1 >= date2) {
                return new Tuple<DateTime, DateTime>(date2, date1);
            }
            else {
                return new Tuple<DateTime, DateTime>(date1, date2);
            }
        }

        private bool IsEblekSubmitted(string productionEntryCode, string pageName) 
        {
            bool result = false;
            var transactionLog = _utilitiesBll.GetLatestActionTransLogExceptSave(productionEntryCode, pageName);
            if (transactionLog != null) {
                result = transactionLog.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
            }
            return result;
        }
        #endregion

        #region Material Usages
        public List<ExePlantMaterialUsagesDTO> GetMaterialUsages(GetExePlantMaterialUsagesInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantProductionGroup>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            else
                return new List<ExePlantMaterialUsagesDTO>();

            if (!string.IsNullOrEmpty(input.Unit))
                queryFilter = queryFilter.And(m => m.UnitCode == input.Unit);
            else
                return new List<ExePlantMaterialUsagesDTO>();

            if (!string.IsNullOrEmpty(input.Process))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.Process);
            else
                return new List<ExePlantMaterialUsagesDTO>();

            if (string.IsNullOrEmpty(input.BrandGroup))
                return new List<ExePlantMaterialUsagesDTO>();

            if (string.IsNullOrEmpty(input.Material))
                return new List<ExePlantMaterialUsagesDTO>();

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "GroupCode" }, "ASC");
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantProductionGroup>();

            var dbResult = _mstPlantProductionGroup.Get(queryFilter, orderByFilter);

            var materials = new List<ExePlantMaterialUsagesDTO>();

            var brandGroupMaterial = _masterDataBll.GetMaterialByCode(input.Material, input.BrandGroup);

            foreach (MstPlantProductionGroup grp in dbResult)
            {
                var materialUsageExist = _exeMaterialUsagesRepo.GetByID(input.LocationCode, input.BrandGroup, input.Material, grp.GroupCode, input.Date);

                var production = GetProductionFromPlantProductionVerificationView(input.LocationCode, input.Unit,
                    input.Shift, input.Process, input.BrandGroup, grp.GroupCode, input.Material, input.Date);

                if (brandGroupMaterial != null)
                {
                    if (brandGroupMaterial.Uom == Enums.Uom.KG.ToString())
                    {
                        production = production / 1000;
                    }
                }

                materials.Add(new ExePlantMaterialUsagesDTO()
                {
                    LocationCode = input.LocationCode,
                    UnitCode = input.Unit,
                    Shift = input.Shift.Value,
                    BrandGroupCode = input.BrandGroup,
                    MaterialCode = input.Material,
                    GroupCode = grp.GroupCode,
                    ProcessGroup = input.Process,
                    ProductionDate = input.Date.Value,
                    Sisa = materialUsageExist != null ? materialUsageExist.Sisa : 0,
                    Ambil1 = materialUsageExist != null ? materialUsageExist.Ambil1 : 0,
                    Ambil2 = materialUsageExist != null ? materialUsageExist.Ambil2 : 0,
                    Ambil3 = materialUsageExist != null ? materialUsageExist.Ambil3 : 0,
                    TobFM = materialUsageExist != null ? materialUsageExist.TobFM : 0,
                    TobStem = materialUsageExist != null ? materialUsageExist.TobStem : 0,
                    TobSapon = materialUsageExist != null ? materialUsageExist.TobSapon : 0,
                    UncountableWaste = materialUsageExist != null ? materialUsageExist.UncountableWaste : 0,
                    CountableWaste = materialUsageExist != null ? materialUsageExist.CountableWaste : 0,
                    CreatedDate = null,
                    CreatedBy = null,
                    UpdatedDate = null,
                    UpdatedBy = null,
                    Production = production
                });
            }

            return materials;
        }

        private double? GetProductionFromPlantProductionVerificationView(
            string location,
            string unitCode,
            int? shift,
            string processGroup,
            string brandGroup,
            string groupCode,
            string material,
            DateTime? productionDate
        )
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerificationView>();

            if (!string.IsNullOrEmpty(location))
                queryFilter = queryFilter.And(q => q.LocationCode == location);

            if (!string.IsNullOrEmpty(unitCode))
                queryFilter = queryFilter.And(q => q.UnitCode == unitCode);

            if (shift != null)
                queryFilter = queryFilter.And(q => q.Shift == shift);

            if (!string.IsNullOrEmpty(processGroup))
                queryFilter = queryFilter.And(q => q.ProcessGroup == processGroup);

            if (!string.IsNullOrEmpty(groupCode))
            {
                var statusEmp = _masterDataBll.GetGenEmpStatusIdentifierByStatusEmp(Enums.StatusEmp.Multiskill.ToString());
                var processGroupCodes = new List<string>();

                processGroupCodes.Add(groupCode);

                if (statusEmp != null)
                {
                    var multiSkillGroupCode = new StringBuilder(groupCode);
                    multiSkillGroupCode[1] = Char.Parse(statusEmp);

                    processGroupCodes.Add(multiSkillGroupCode.ToString());

                }

                queryFilter = queryFilter.And(q => processGroupCodes.Contains(q.GroupCode));
            }

            if (!string.IsNullOrEmpty(location))
                queryFilter = queryFilter.And(q => q.LocationCode == location);

            if (!string.IsNullOrEmpty(brandGroup))
            {
                var groupFilter = PredicateHelper.True<MstGenBrand>();

                groupFilter = groupFilter.And(a => a.BrandGroupCode == brandGroup);

                var groupCodes = _mstGenBrandRepo.Get(groupFilter).Select(s => s.BrandCode).ToList();

                if (groupCodes != null)
                    queryFilter = queryFilter.And(q => groupCodes.Contains(q.BrandCode));
            }

            if (productionDate.HasValue)
                queryFilter = queryFilter.And(q => q.ProductionDate == productionDate.Value);

            var dbResult = _exePlantProductionEntryVerificationViewRepo.Get(queryFilter).Sum(s => s.TotalActualValue);
            var genBrandGroup = _mstGenBrandGroup.GetByID(brandGroup);

            if (genBrandGroup == null) return null;

            var isTobaco = true;
            var db = _masterDataBll.GetMaterialByCode(material, brandGroup);
            if (db != null && db.MaterialName.Contains("Non"))
            {
                isTobaco = false;
            }

            var production = isTobaco ? (float)(genBrandGroup.CigarreteWeight * dbResult) : (float)dbResult;

            var query = PredicateHelper.True<ProcessSettingsAndLocationView>();

            query = query.And(x => x.BrandGroupCode == brandGroup);
            query = query.And(x => x.LocationCode == location);
            query = query.And(x => x.ProcessGroup == processGroup);

            var genProcessSetting = _mstGenProcessSettingLocationViewRepo.Get(query).FirstOrDefault();
            decimal result = 0;

            if (genProcessSetting != null)
            {
                switch (processGroup.ToUpper())
                {
                    case "ROLLING":
                        result = (decimal)production * (decimal)genProcessSetting.UOMEblek;
                        break;
                    case "CUTTING":
                        result = (decimal)production * (decimal)genProcessSetting.UOMEblek;
                        break;
                    case "PACKING":
                        result = (decimal)production * (decimal)(genProcessSetting.UOMEblek / genProcessSetting.StickPerPack);
                        break;
                    case "STAMPING":
                        result = (decimal)production * (decimal)(genProcessSetting.UOMEblek / genProcessSetting.StickPerPack) / (decimal)genProcessSetting.PackPerSlof;
                        break;
                }
            }

            return Math.Truncate((double) result);
        }

        public ExePlantMaterialUsagesDTO SaveMaterialUsage(ExePlantMaterialUsagesDTO materialUsagesDto)
        {
            var dbMaterialUsage = _exeMaterialUsagesRepo.GetByID(materialUsagesDto.LocationCode, materialUsagesDto.BrandGroupCode,
                                                                    materialUsagesDto.MaterialCode, materialUsagesDto.GroupCode, materialUsagesDto.ProductionDate);

            //Check Material Code and Brand Group Code in MstGenMaterial
            var dbMaterial = _masterDataBll.GetBrandGroupMaterial(new GetBrandGroupMaterialInput { BrandGroupCode = materialUsagesDto.BrandGroupCode });
            dbMaterial = dbMaterial.Where(m => m.MaterialCode == materialUsagesDto.MaterialCode).ToList();
            if (dbMaterial.Count < 1)
                throw new BLLException(ExceptionCodes.BLLExceptions.MaterialCodeAndBrandGroupCodeNotExistinMstGenMaterial);

            if (dbMaterialUsage == null)
            {
                dbMaterialUsage = Mapper.Map<ExeMaterialUsage>(materialUsagesDto);

                _exeMaterialUsagesRepo.Insert(dbMaterialUsage);

                if (dbMaterialUsage.CreatedBy == null)
                    dbMaterialUsage.CreatedBy = dbMaterialUsage.UpdatedBy;

                dbMaterialUsage.CreatedDate = DateTime.Now;
                dbMaterialUsage.UpdatedDate = DateTime.Now;
            }
            else
            {
                Mapper.Map(materialUsagesDto, dbMaterialUsage);
                dbMaterialUsage.UpdatedDate = DateTime.Now;

                _exeMaterialUsagesRepo.Update(dbMaterialUsage);
            }
            _uow.SaveChanges();

            return Mapper.Map<ExePlantMaterialUsagesDTO>(dbMaterialUsage);
        }
        #endregion

        #region WorkerAssignmentRemoval

        public void SaveWorkerAssignmentRemoval(ExePlantProductionEntryDTO plantProductionEntryDto)
        {
            var dbProductionEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(plantProductionEntryDto.ProductionEntryCode);
            if (dbProductionEntryVerification != null)
            {
                var dbWorkerAssigmnt = _exePlantWorkerAssignmentRepo.Get(m => m.EmployeeID == plantProductionEntryDto.EmployeeID
                                                                         && dbProductionEntryVerification.ProductionDate >= m.StartDate
                                                                         && dbProductionEntryVerification.ProductionDate <= m.StartDate).FirstOrDefault();
                if (dbWorkerAssigmnt != null)
                {
                    var workerAssigmntRemovalDto = new WorkerAssignmentRemovalDTO
                    {
                        CreatedBy = plantProductionEntryDto.CreatedBy,
                        UpdatedBy = plantProductionEntryDto.UpdatedBy,
                        ProductionEntryCode = plantProductionEntryDto.ProductionEntryCode,
                        StartDate = dbWorkerAssigmnt.StartDate,
                        EndDate = dbWorkerAssigmnt.EndDate,
                        DeleteDate = dbProductionEntryVerification.ProductionDate,
                        EmployeeID = dbWorkerAssigmnt.EmployeeID,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };

                    var dbWorkerAssigmntRemoval = Mapper.Map<WorkerAssignmentRemoval>(workerAssigmntRemovalDto);
                    _workerAssignmentRemovalRepository.Insert(dbWorkerAssigmntRemoval);

                    _uow.SaveChanges();
                }
            }
        }

        #endregion

        #region ExePlant Production Entry Verification

        public List<ExePlantProductionEntryVerificationViewDTO> GetExePlantProductionEntryVerificationViews(GetExePlantProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerificationView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            if (input.Shift != null)
                queryFilter = queryFilter.And(m => m.Shift == input.Shift);

            if (input.BrandCode != null)
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);

            if (input.KpsYear != null)
                queryFilter = queryFilter.And(m => m.KPSYear == input.KpsYear);

            if (input.KpsWeek != null)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.KpsWeek);

            if (input.ProductionDate != null || input.ProductionDate != DateTime.MinValue)
                queryFilter = queryFilter.And(m => m.ProductionDate == input.ProductionDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantProductionEntryVerificationView>();

            var checkActual = this.CheckCompletedActualWorkHours(input);
            var dbResult = _exePlantProductionEntryVerificationViewRepo.Get(queryFilter, orderByFilter);
            var oderByProcess = dbResult.OrderBy(c => c.ProcessOrder);
            var result = Mapper.Map<List<ExePlantProductionEntryVerificationViewDTO>>(oderByProcess);

            //result.ForEach(m => m.AlreadySubmit = _utilitiesBll.GetLatestActionTransLogExceptSave(m.ProductionEntryCode, Enums.PageName.ProductionEntryVerification.ToString()));
            //result.ForEach(m => m.DisableReturn = _utilitiesBll.CheckDataAlreadySaveInProdCard(m.ProductionEntryCode));

            // Check Production Card Submitted
            foreach (var item in result.Where(c => c.GroupCode.Substring(1,1) != "5"))
            {
                // Check history if it has been submitted in any previous time
                item.IsHadBeenSubmitted = _utilitiesBll.GetTransLogSubmittedEntryVerification(item.ProductionEntryCode);

                var transLogVerification = _utilitiesBll.GetLatestActionTransLogExceptSave(item.ProductionEntryCode, Enums.PageName.ProductionEntryVerification.ToString());
                if (transLogVerification != null)
                {
                    item.IsHadBeenReturned = transLogVerification.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Return.ToString();
                    item.AlreadySubmit = transLogVerification.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                    var translogEntry = _utilitiesBll.GetLatestActionTransLogExceptSave(item.ProductionEntryCode, Enums.PageName.PlantProductionEntry.ToString());
                    if (translogEntry != null) {
                            if (translogEntry.CreatedDate > transLogVerification.CreatedDate) {
                                item.AlreadySubmit = false;
                            }
                    }

                    var transLogProdCard = item.ProductionEntryCode + "/" + GetLatestProdCardRevType(item.LocationCode, item.UnitCode, item.BrandCode, item.ProcessGroup, item.GroupCode, item.ProductionDate);

                    var transLogProdCardSubmitted = _utilitiesBll.GetLatestActionTransLogExceptSave(transLogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCard.ToString());
                    if (transLogProdCardSubmitted != null)
                    {
                        item.ProductionCardSubmit = true;
                        item.ProductionCardSubmit = transLogProdCardSubmitted.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                        if (transLogVerification.CreatedDate < transLogProdCardSubmitted.CreatedDate)
                        {
                            item.ProductionCardSubmit = transLogProdCardSubmitted.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();

                        }

                        if (translogEntry != null) 
                        {
                            if (translogEntry.CreatedDate > transLogProdCardSubmitted.CreatedDate) {
                                item.ProductionCardSubmit = false;
                            }
                        }

                        var transLogEntryReleaseApproval = _utilitiesBll.GetLatestActionTransLog(item.ProductionEntryCode, Enums.PageName.EblekReleaseApproval.ToString());
                        if (transLogEntryReleaseApproval != null)
                        {
                            item.ProductionEntryRelease = true;
                            if (transLogProdCardSubmitted.CreatedDate < transLogEntryReleaseApproval.CreatedDate)
                            {
                                item.ProductionEntryRelease = transLogEntryReleaseApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString()
                                                   && transLogEntryReleaseApproval.IDFlow == (int)HMS.SKTIS.Core.Enums.IdFlow.EblekReleaseApprovalFinal;
                                if (item.ProductionEntryRelease) { item.ProductionCardSubmit = false; }
                            }
                            else
                            {
                                item.ProductionEntryRelease = false;
                            }
                        }
                    }
                }
            }

            // Checking for Dummy Group
            foreach (var item in result.Where(c => c.GroupCode.Substring(1, 1) == "5"))
            {
                string transactionCode = item.ProductionEntryCode;
                string groupCode = item.GroupCode;

                transactionCode = transactionCode.Remove(17, 1).Insert(17, "1");
                groupCode = groupCode.Remove(1, 1).Insert(1, "1");

                var transLogVerification = _utilitiesBll.GetLatestActionTransLogExceptSave(transactionCode, Enums.PageName.ProductionEntryVerification.ToString());
                if (transLogVerification != null)
                {
                    item.IsHadBeenReturned = transLogVerification.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Return.ToString();
                    item.AlreadySubmit = transLogVerification.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                    var translogEntry = _utilitiesBll.GetLatestActionTransLogExceptSave(transactionCode, Enums.PageName.PlantProductionEntry.ToString());
                    if (translogEntry != null)
                    {
                        if (translogEntry.CreatedDate > transLogVerification.CreatedDate)
                        {
                            item.AlreadySubmit = false;
                        }
                    }

                    var transLogProdCard = transactionCode + "/" + GetLatestProdCardRevType(item.LocationCode, item.UnitCode, item.BrandCode, item.ProcessGroup, groupCode, item.ProductionDate);

                    var transLogProdCardSubmitted = _utilitiesBll.GetLatestActionTransLogExceptSave(transLogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCard.ToString());
                    if (transLogProdCardSubmitted != null)
                    {
                        item.ProductionCardSubmit = true;
                        item.ProductionCardSubmit = transLogProdCardSubmitted.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                        if (transLogVerification.CreatedDate < transLogProdCardSubmitted.CreatedDate)
                        {
                            item.ProductionCardSubmit = transLogProdCardSubmitted.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();

                        }

                        if (translogEntry != null) {
                            if (translogEntry.CreatedDate > transLogProdCardSubmitted.CreatedDate) {
                                item.ProductionCardSubmit = false;
                            }
                        }

                        var transLogEntryReleaseApproval = _utilitiesBll.GetLatestActionTransLog(transactionCode, Enums.PageName.EblekReleaseApproval.ToString());
                        if (transLogEntryReleaseApproval != null)
                        {
                            item.ProductionEntryRelease = true;
                            if (transLogProdCardSubmitted.CreatedDate < transLogEntryReleaseApproval.CreatedDate)
                            {
                                item.ProductionEntryRelease = transLogEntryReleaseApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString()
                                                   && transLogEntryReleaseApproval.IDFlow == (int)HMS.SKTIS.Core.Enums.IdFlow.EblekReleaseApprovalFinal;
                                if (item.ProductionEntryRelease) { item.ProductionCardSubmit = false; }
                            }
                            else
                            {
                                item.ProductionEntryRelease = false;
                            }
                        }
                    }
                }
            }

            //result.ForEach(m => m.ProductionCardSubmit = _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(m.ProductionEntryCode.Replace("EBL", "WPC"), (int)HMS.SKTIS.Core.Enums.IdFlow.ProductionCardSubmitComplete) == null ? false : _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(m.ProductionEntryCode, (int)HMS.SKTIS.Core.Enums.IdFlow.PlantProductionEntryVerificationSubmit) == null ? false : _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(m.ProductionEntryCode.Replace("EBL", "WPC"), (int)HMS.SKTIS.Core.Enums.IdFlow.ProductionCardSubmitComplete).UpdatedDate > _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(m.ProductionEntryCode, (int)HMS.SKTIS.Core.Enums.IdFlow.PlantProductionEntryVerificationSubmit).UpdatedDate);  //_utilitiesBll.CheckLatestActionTransLogExceptSaveIsSubmit(m.ProductionEntryCode.Replace("EBL", "WPC"), Enums.PageName.ProductionCard.ToString()));
            //result.ForEach(m => m.ProductionEntryRelease = _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(m.ProductionEntryCode, (int)HMS.SKTIS.Core.Enums.IdFlow.EblekReleaseApprovalFinal) == null ? false : _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(m.ProductionEntryCode, (int)HMS.SKTIS.Core.Enums.IdFlow.PlantProductionEntryVerificationSubmit) == null ? true : _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(m.ProductionEntryCode, (int)HMS.SKTIS.Core.Enums.IdFlow.EblekReleaseApprovalFinal).UpdatedDate > _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(m.ProductionEntryCode, (int)HMS.SKTIS.Core.Enums.IdFlow.PlantProductionEntryVerificationSubmit).UpdatedDate);
            result.ForEach(m => m.CompletedActualWorkHours = checkActual);


            if (result.Any())
            {
                var groupby = result.Select(m => m.ProcessGroup);
                var distict = groupby.Distinct();

                var result2 = new List<ExePlantProductionEntryVerificationViewDTO>();

                foreach (var processGroup in distict)
                {
                    result2.AddRange(result.Where(dto => dto.ProcessGroup == processGroup));
                    var plantDto = new ExePlantProductionEntryVerificationViewDTO
                    {
                        ProcessGroup = "Total",
                        GroupCode = result.Count(m => m.ProcessGroup == processGroup).ToString(),
                        A = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.A),
                        I = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.I),
                        S = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.S),
                        C = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.C),
                        CH = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.CH),
                        CT = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.CT),
                        SLS_SLP = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.SLS_SLP),
                        ETC = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.ETC),
                        Plant = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.Plant),
                        Actual = result.Where(m => m.ProcessGroup == processGroup).Sum(m => m.Actual)
                    };
                    result2.Add(plantDto);
                }
                return result2;
            }

            return result;
        }

        public ExePlantProductionEntryVerificationViewDTO UpdatExePlantProductionEntryVerification(ExePlantProductionEntryVerificationViewDTO verificationViewDto)
        {
            var dbPlantEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(verificationViewDto.ProductionEntryCode);
            if (dbPlantEntryVerification == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            Mapper.Map(verificationViewDto, dbPlantEntryVerification);
            dbPlantEntryVerification.UpdatedDate = DateTime.Now;

            _exePlantProductionEntryVerificationRepo.Update(dbPlantEntryVerification);

            _uow.SaveChanges();
            return Mapper.Map<ExePlantProductionEntryVerificationViewDTO>(dbPlantEntryVerification);
        }

        public void UpdatExePlantProductionEntryVerificationWhenSubmit(ExePlantProductionEntryVerificationViewDTO verificationViewDto)
        {
            var dbPlantEntryVerification = _exePlantProductionEntryVerificationRepo.GetByID(verificationViewDto.ProductionEntryCode);
            if (dbPlantEntryVerification == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            Mapper.Map(verificationViewDto, dbPlantEntryVerification);
            dbPlantEntryVerification.UpdatedDate = DateTime.Now;

            _exePlantProductionEntryVerificationRepo.Update(dbPlantEntryVerification);

            _uow.SaveChanges();
        }

        public void SaveProductionCardFromProductionEntry(ExePlantProductionEntryVerificationViewDTO verificationViewDto)
        {
            var dbEblekVer = _exePlantProductionEntryVerificationRepo.GetByID(verificationViewDto.ProductionEntryCode);
            if (dbEblekVer == null) return;
            var dbEblek = _exePlantProductionEntryRepo.Get(m => m.ProductionEntryCode == dbEblekVer.ProductionEntryCode);

            foreach (ExePlantProductionEntry exePlantProductionEntry in dbEblek)
            {
                var updahLain = new float?();
                //var updahLain = exePlantProductionEntry.ProdActual;
                //Get mstGenBrand
                var mstGenBrand = _mstGenBrandRepo.GetByID(dbEblekVer.BrandCode);
                //Get mstAbsentType
                var mstAbsentType = _mstPlantAbsentType.GetByID(exePlantProductionEntry.AbsentType);

                var criteria = new GetMasterProcessSettingsInput()
                {
                    BrandCode = mstGenBrand.BrandGroupCode,
                    Process = dbEblekVer.ProcessGroup
                };
                var mstProcessSetting = _masterDataBll.GetMasterProcessSettings(criteria).FirstOrDefault();

                var isHoliday = _masterDataBll.IsHoliday(dbEblekVer.ProductionDate, dbEblekVer.LocationCode);
                var dayWeek = (int)dbEblekVer.ProductionDate.DayOfWeek == 0 ? 7 : (int)dbEblekVer.ProductionDate.DayOfWeek;
                var mstStandardHour = (isHoliday)
                    ? _masterDataBll.GetStandardHourByDayTypeDay(dayWeek, "Holiday")
                    : _masterDataBll.GetStandardHourByDayTypeDay(dayWeek, "Non-Holiday");

                var production = exePlantProductionEntry.ProdActual;
                var groupCode = verificationViewDto.GroupCode;

                var input = new GetExePlantWorkerAssignmentInput()
                {
                    LocationCode = verificationViewDto.LocationCode,
                    UnitCode = verificationViewDto.UnitCode,
                    Shift = verificationViewDto.Shift,
                    Year = verificationViewDto.KPSYear,
                    Week = verificationViewDto.KPSWeek,
                    Date = verificationViewDto.ProductionDate,
                    SourceBrandCode = verificationViewDto.BrandCode,
                    ProductionDateFrom = verificationViewDto.ProductionDate,
                    DateTypeFilter = "rdbProductionDate"
                };

                var existWorkerAssignment = IsExistWorkerAssignment(input).FirstOrDefault(t => t.EmployeeID == exePlantProductionEntry.EmployeeID);

                //#1 For Employee ID with 2nd digit GroupCode != 5 then:
                //Insert Production Card row of each worker:

                // Production = ProdActual from Eblek
                // Remark = Payroll Absent Code
                // If AbsentType is NOT NULL then check to MstPlantAbsentType
                // if PrdTargetFlg = 1 and PayrollAbsentCode != MO, then UpahLain = ProdTarget Eblek - ProdActual Eblek

                if (verificationViewDto.GroupCode.Substring(1, 1) != "5")
                {
                    //If AbsentType is NOT NULL then check to MstPlantAbsentType
                    //if PrdTargetFlg = 1 and PayrollAbsentCode != MO, then UpahLain = ProdTarget Eblek - ProdActual Eblek
                    if (mstAbsentType != null)
                    {
                        var calculation = "";
                        if (!String.IsNullOrEmpty(mstAbsentType.Calculation))
                        {
                            if (mstAbsentType.Calculation.StartsWith("*"))
                                calculation = mstAbsentType.Calculation.Substring(1);
                            else
                                calculation = mstAbsentType.Calculation;
                            //? mstAbsentType.Calculation.Substring(1, mstAbsentType.Calculation.Length) : "");
                        }
                        if (calculation == "T")
                        {
                            production = exePlantProductionEntry.ProdActual;
                            updahLain = exePlantProductionEntry.ProdTarget - exePlantProductionEntry.ProdActual;
                        }
                        if (mstAbsentType.PayrollAbsentCode != "MO")
                        {
                            //if (mstAbsentType.AbsentType == "Pulang Pagi (Ijin)")
                            production = exePlantProductionEntry.ProdActual;
                            updahLain = 0;
                            if (calculation == "40/6")
                            {
                                production = exePlantProductionEntry.ProdActual;
                                var calc = (double)(40.0 / 6.0) * ((double)mstProcessSetting.StdStickPerHour.Value / (double)mstProcessSetting.UOMEblek.Value);
                                updahLain = (float)calc - exePlantProductionEntry.ProdActual;

                            }
                            else if (calculation == "JKN")
                            {
                                production = exePlantProductionEntry.ProdActual;
                                var calc = (double)(mstStandardHour.JknHour * (double)mstProcessSetting.StdStickPerHour.Value / (double)mstProcessSetting.UOMEblek.Value);
                                updahLain = (float)calc - exePlantProductionEntry.ProdActual;
                            }
                        }
                        if (mstAbsentType.PayrollAbsentCode == "MO" && existWorkerAssignment != null)
                        {
                            var inputEntrySource = new GetExePlantProductionEntryInput
                            {
                                LocationCode = existWorkerAssignment.SourceLocationCode,
                                UnitCode = existWorkerAssignment.SourceUnitCode,
                                Shift = existWorkerAssignment.SourceShift.ToString(),
                                ProcessGroup = existWorkerAssignment.SourceProcessGroup,
                                Group = existWorkerAssignment.SourceGroupCode,
                                Brand = existWorkerAssignment.SourceBrandCode,
                                Year = verificationViewDto.KPSYear,
                                Week = verificationViewDto.KPSWeek,
                                Date = verificationViewDto.ProductionDate
                            };
                            var inputEntryDest = new GetExePlantProductionEntryInput
                            {
                                LocationCode = existWorkerAssignment.DestinationLocationCode,
                                UnitCode = existWorkerAssignment.DestinationUnitCode,
                                Shift = existWorkerAssignment.DestinationShift.ToString(),
                                ProcessGroup = existWorkerAssignment.DestinationProcessGroup,
                                Group = existWorkerAssignment.DestinationGroupCodeDummy,
                                Brand = existWorkerAssignment.DestinationBrandCode,
                                Year = verificationViewDto.KPSYear,
                                Week = verificationViewDto.KPSWeek,
                                Date = verificationViewDto.ProductionDate
                            };
                            var prodEntrySource = GetExePlantProductionEntryByCodeEmployeeID(inputEntrySource,
                                exePlantProductionEntry.EmployeeID);
                            var prodEntryDest = GetExePlantProductionEntryByCodeEmployeeID(inputEntryDest,
                                exePlantProductionEntry.EmployeeID);

                            if (existWorkerAssignment.SourceProcessGroup ==
                                existWorkerAssignment.DestinationProcessGroup)
                            {
                                production = prodEntrySource.ProdActual + prodEntryDest.ProdActual;
                                updahLain = 0;

                            }
                            else
                            {
                                production = prodEntrySource.ProdActual;
                                updahLain = prodEntrySource.ProdTarget - prodEntrySource.ProdActual;
                            }
                        }
                    }
                }
                if (verificationViewDto.GroupCode.Substring(1, 1) == "5")
                {
                    var groupCodeReal = new StringBuilder(verificationViewDto.GroupCode);
                    groupCodeReal = groupCodeReal.Remove(1, 1);
                    groupCodeReal = groupCodeReal.Insert(1, "1");
                    groupCode = groupCodeReal.ToString();

                    if (mstAbsentType != null)
                    {
                        if (mstAbsentType.PayrollAbsentCode == "MO" && existWorkerAssignment != null)
                        {
                            var inputEntrySource = new GetExePlantProductionEntryInput
                            {
                                LocationCode = existWorkerAssignment.SourceLocationCode,
                                UnitCode = existWorkerAssignment.SourceUnitCode,
                                Shift = existWorkerAssignment.SourceShift.ToString(),
                                ProcessGroup = existWorkerAssignment.SourceProcessGroup,
                                Group = existWorkerAssignment.SourceGroupCode,
                                Brand = existWorkerAssignment.SourceBrandCode,
                                Year = verificationViewDto.KPSYear,
                                Week = verificationViewDto.KPSWeek,
                                Date = verificationViewDto.ProductionDate
                            };
                            var inputEntryDest = new GetExePlantProductionEntryInput
                            {
                                LocationCode = existWorkerAssignment.DestinationLocationCode,
                                UnitCode = existWorkerAssignment.DestinationUnitCode,
                                Shift = existWorkerAssignment.DestinationShift.ToString(),
                                ProcessGroup = existWorkerAssignment.DestinationProcessGroup,
                                Group = existWorkerAssignment.DestinationGroupCodeDummy,
                                Brand = existWorkerAssignment.DestinationBrandCode,
                                Year = verificationViewDto.KPSYear,
                                Week = verificationViewDto.KPSWeek,
                                Date = verificationViewDto.ProductionDate
                            };
                            var prodEntrySource = GetExePlantProductionEntryByCodeEmployeeID(inputEntrySource, exePlantProductionEntry.EmployeeID);
                            var prodEntryDest = GetExePlantProductionEntryByCodeEmployeeID(inputEntryDest, exePlantProductionEntry.EmployeeID);

                            if (existWorkerAssignment.SourceProcessGroup ==
                                existWorkerAssignment.DestinationProcessGroup)
                            {
                                production = prodEntrySource.ProdActual + prodEntryDest.ProdActual;
                                updahLain = 0;

                            }
                            else
                            {
                                production = prodEntrySource.ProdActual;
                                updahLain = prodEntrySource.ProdTarget - prodEntrySource.ProdActual;
                            }
                        }
                    }
                }
                var prodCard = _plantWagesBll.GetProductionCard(new ProductionCardDTO()
                {
                    RevisionType = 0,
                    LocationCode = dbEblekVer.LocationCode,
                    UnitCode = dbEblekVer.UnitCode,
                    BrandCode = dbEblekVer.BrandCode,
                    ProcessGroup = dbEblekVer.ProcessGroup,
                    GroupCode = groupCode,
                    EmployeeID = exePlantProductionEntry.EmployeeID,
                    ProductionDate = dbEblekVer.ProductionDate
                });
                if (prodCard == null)
                {
                    prodCard = new ProductionCardDTO
                    {
                        ProductionCardCode =
                            string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}", Enums.CombineCode.WPC,
                                dbEblekVer.LocationCode, dbEblekVer.Shift, dbEblekVer.UnitCode, groupCode,
                                dbEblekVer.BrandCode, dbEblekVer.KPSYear, dbEblekVer.KPSWeek,
                                (int)dbEblekVer.ProductionDate.Date.DayOfWeek),
                        RevisionType = 0,
                        LocationCode = dbEblekVer.LocationCode,
                        UnitCode = dbEblekVer.UnitCode,
                        Shift = dbEblekVer.Shift,
                        BrandCode = dbEblekVer.BrandCode,
                        BrandGroupCode = mstGenBrand.BrandGroupCode,
                        ProcessGroup = dbEblekVer.ProcessGroup,
                        //GroupCode = dbEblekVer.GroupCode,
                        GroupCode = groupCode,
                        EmployeeID = exePlantProductionEntry.EmployeeID,
                        EmployeeNumber = exePlantProductionEntry.EmployeeNumber,
                        Production = production,
                        ProductionDate = dbEblekVer.ProductionDate,
                        WorkHours = dbEblekVer.WorkHour,
                        Absent = exePlantProductionEntry.AbsentCodePayroll,
                        UpahLain = updahLain,
                        EblekAbsentType = exePlantProductionEntry.AbsentCodeEblek,
                        Remark = null,
                        Comments = null,
                        CreatedBy = verificationViewDto.UpdatedBy,
                        UpdatedBy = verificationViewDto.UpdatedBy
                    };
                }
                else
                {
                    var listDate = new List<DateTime>();
                    var endDate = _masterDataBll.GetClosingPayrollBeforeTodayDateTime(DateTime.Now.Date);
                    var startDate = _masterDataBll.GetClosingPayrollBeforeTodayDateTime(endDate.Date).AddDays(1);
                    for (int i = 0; i < 7; i++)
                    {
                        listDate.Add(startDate.AddDays(i));
                    }
                    if (listDate.Contains(dbEblekVer.ProductionDate))
                    {
                        var pc = _plantWagesBll.GetProductionCards(new GetProductionCardInput
                        {
                            LocationCode = dbEblekVer.LocationCode,
                            Unit = dbEblekVer.UnitCode,
                            Shift = dbEblekVer.Shift,
                            Brand = dbEblekVer.BrandCode,
                            Process = dbEblekVer.ProcessGroup,
                            Group = groupCode,
                            Date = dbEblekVer.ProductionDate
                        });
                        var currentRevType = pc.Select(m => m.RevisionType).Max();
                        var oldPc = _plantWagesBll.GetProductionCard(new ProductionCardDTO
                        {
                            RevisionType = 0,
                            LocationCode = dbEblekVer.LocationCode,
                            UnitCode = dbEblekVer.UnitCode,
                            BrandCode = dbEblekVer.BrandCode,
                            ProcessGroup = dbEblekVer.ProcessGroup,
                            GroupCode = groupCode,
                            EmployeeID = exePlantProductionEntry.EmployeeID,
                            ProductionDate = dbEblekVer.ProductionDate
                        });
                        if ((production - oldPc.Production) != 0)
                        {
                            prodCard = new ProductionCardDTO
                            {
                                ProductionCardCode =
                                    string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}", Enums.CombineCode.WPC,
                                        dbEblekVer.LocationCode, dbEblekVer.Shift, dbEblekVer.UnitCode, groupCode,
                                        dbEblekVer.BrandCode, dbEblekVer.KPSYear, dbEblekVer.KPSWeek,
                                        (int)dbEblekVer.ProductionDate.Date.DayOfWeek),
                                RevisionType = currentRevType + 1,
                                LocationCode = dbEblekVer.LocationCode,
                                UnitCode = dbEblekVer.UnitCode,
                                Shift = dbEblekVer.Shift,
                                BrandCode = dbEblekVer.BrandCode,
                                BrandGroupCode = mstGenBrand.BrandGroupCode,
                                ProcessGroup = dbEblekVer.ProcessGroup,
                                //GroupCode = dbEblekVer.GroupCode,
                                GroupCode = groupCode,
                                EmployeeID = exePlantProductionEntry.EmployeeID,
                                EmployeeNumber = exePlantProductionEntry.EmployeeNumber,
                                Production = production - oldPc.Production,
                                ProductionDate = dbEblekVer.ProductionDate,
                                WorkHours = dbEblekVer.WorkHour,
                                Absent = exePlantProductionEntry.AbsentCodePayroll,
                                UpahLain = updahLain - oldPc.UpahLain,
                                EblekAbsentType = exePlantProductionEntry.AbsentCodeEblek,
                                Remark = null,
                                Comments = null,
                                CreatedBy = verificationViewDto.UpdatedBy,
                                UpdatedBy = verificationViewDto.UpdatedBy
                            };
                        }
                    }
                    else
                    {
                        prodCard.Absent = exePlantProductionEntry.AbsentCodePayroll;
                        prodCard.EblekAbsentType = exePlantProductionEntry.AbsentCodeEblek;
                        prodCard.Production = production;
                        prodCard.UpahLain = updahLain;
                    }
                }
                _plantWagesBll.InsertProductionCard(prodCard);

            }

            _uow.SaveChanges();
        }

        public void SaveProductionCardFromProductionEntryTuning(ExePlantProductionEntryVerificationViewDTO verificationViewDto, string brandGroupCode, MstGenStandardHourDTO mstGenStdHour)
        {
            var dbEblek = _exePlantProductionEntryRepo.Get(m => m.ProductionEntryCode == verificationViewDto.ProductionEntryCode);
            if (!dbEblek.Any()) return;

            var groupCode = verificationViewDto.GroupCode;
            if (verificationViewDto.GroupCode.Substring(1, 1) == "5")
            {
                var groupCodeReal = new StringBuilder(verificationViewDto.GroupCode);
                groupCodeReal = groupCodeReal.Remove(1, 1);
                groupCodeReal = groupCodeReal.Insert(1, "1");
                groupCode = groupCodeReal.ToString();
            }

            var listEmp = dbEblek.Select(c => c.EmployeeID).Distinct();

            var listProdCardRev0 = _productionCardRepo.Get(c => c.LocationCode == verificationViewDto.LocationCode &&
                                                                c.UnitCode == verificationViewDto.UnitCode &&
                                                                c.BrandCode == verificationViewDto.BrandCode &&
                                                                c.ProcessGroup == verificationViewDto.ProcessGroup &&
                                                                c.GroupCode == verificationViewDto.GroupCode &&
                                                                c.GroupCode == groupCode &&
                                                                c.ProductionDate == verificationViewDto.ProductionDate &&
                                                                c.RevisionType == 0);

            var listProdcardRev0Emp = listProdCardRev0.Where(c => listEmp.Contains(c.EmployeeID)).ToList();


             var listProdCardInsert = new List<ProductionCard>();

            foreach (ExePlantProductionEntry exePlantProductionEntry in dbEblek)
            {
                // Get absent from master
                string payrollAbsent = "";
                if (!String.IsNullOrEmpty(exePlantProductionEntry.AbsentType))
                {
                    var mstAbsent = _masterDataBll.GetMstPlantAbsentTypeById(exePlantProductionEntry.AbsentType);
                    if (mstAbsent != null) payrollAbsent = mstAbsent.PayrollAbsentCode;
                }
                
                var updahLain = new float?();

                var criteria = new GetMasterProcessSettingsInput()
                {
                    BrandCode = brandGroupCode,
                    Process = verificationViewDto.ProcessGroup
                };

                var production = exePlantProductionEntry.ProdActual ?? 0;

                //#1 For Employee ID with 2nd digit GroupCode != 5 then:
                //Insert Production Card row of each worker:

                // Production = ProdActual from Eblek
                // Remark = Payroll Absent Code
                // If AbsentType is NOT NULL then check to MstPlantAbsentType
                // if PrdTargetFlg = 1 and PayrollAbsentCode != MO, then UpahLain = ProdTarget Eblek - ProdActual Eblek

                if (verificationViewDto.GroupCode.Substring(1, 1) != "5")
                {
                    //If AbsentType is NOT NULL then check to MstPlantAbsentType
                    //if PrdTargetFlg = 1 and PayrollAbsentCode != MO, then UpahLain = ProdTarget Eblek - ProdActual Eblek
                    if (!string.IsNullOrEmpty(exePlantProductionEntry.AbsentType))
                    {
                        var calculation = "";
                        if (!String.IsNullOrEmpty(exePlantProductionEntry.MstPlantAbsentType.Calculation))
                        {
                            if (exePlantProductionEntry.MstPlantAbsentType.Calculation.StartsWith("*"))
                                calculation = exePlantProductionEntry.MstPlantAbsentType.Calculation.Substring(1);
                            else
                                calculation = exePlantProductionEntry.MstPlantAbsentType.Calculation;
                            //? mstAbsentType.Calculation.Substring(1, mstAbsentType.Calculation.Length) : "");
                        }
                        if (calculation == "T" && exePlantProductionEntry.MstPlantAbsentType.SktAbsentCode != "MO") // change PayrollAbsentCode to SktAbsentCode
                        {
                            production = exePlantProductionEntry.ProdActual ?? 0;
                            updahLain = (exePlantProductionEntry.ProdTarget ?? 0) - (exePlantProductionEntry.ProdActual ?? 0);
                        }
                        else if (exePlantProductionEntry.MstPlantAbsentType.SktAbsentCode != "MO") // change PayrollAbsentCode to SktAbsentCode
                        {
                            //if (mstAbsentType.AbsentType == "Pulang Pagi (Ijin)")
                            production = exePlantProductionEntry.ProdActual ?? 0;
                            updahLain = 0;
                            var mstProcessSetting = _masterDataBll.GetMasterProcessSettings(criteria).FirstOrDefault();
                            if (calculation == "40/6")
                            {
                                production = exePlantProductionEntry.ProdActual ?? 0;
                                var calc = (double)(40.0 / 6.0) * ((double)mstProcessSetting.StdStickPerHour.Value / (double)mstProcessSetting.UOMEblek.Value);
                                updahLain = (float)calc - exePlantProductionEntry.ProdActual;

                            }
                            else if (calculation == "JKN")
                            {
                                production = exePlantProductionEntry.ProdActual ?? 0;
                                var calc = (double)(mstGenStdHour.JknHour * (double)mstProcessSetting.StdStickPerHour.Value / (double)mstProcessSetting.UOMEblek.Value);
                                updahLain = (float)calc - exePlantProductionEntry.ProdActual;
                            }
                        }
                        else if (exePlantProductionEntry.MstPlantAbsentType.SktAbsentCode == "MO") // change PayrollAbsentCode to SktAbsentCode
                        {
                            var input = new GetExePlantWorkerAssignmentInput()
                            {
                                LocationCode = verificationViewDto.LocationCode,
                                UnitCode = verificationViewDto.UnitCode,
                                Shift = verificationViewDto.Shift,
                                Year = verificationViewDto.KPSYear,
                                Week = verificationViewDto.KPSWeek,
                                Date = verificationViewDto.ProductionDate,
                                SourceBrandCode = verificationViewDto.BrandCode,
                                ProductionDateFrom = verificationViewDto.ProductionDate,
                                DateTypeFilter = "rdbProductionDate"
                            };

                            var existWorkerAssignment = IsExistWorkerAssignment(input).FirstOrDefault(t => t.EmployeeID == exePlantProductionEntry.EmployeeID);

                            if (existWorkerAssignment != null)
                            {
                                var inputEntrySource = new GetExePlantProductionEntryInput
                                {
                                    LocationCode = existWorkerAssignment.SourceLocationCode,
                                    UnitCode = existWorkerAssignment.SourceUnitCode,
                                    Shift = existWorkerAssignment.SourceShift.ToString(),
                                    ProcessGroup = existWorkerAssignment.SourceProcessGroup,
                                    Group = existWorkerAssignment.SourceGroupCode,
                                    Brand = existWorkerAssignment.SourceBrandCode,
                                    Year = verificationViewDto.KPSYear,
                                    Week = verificationViewDto.KPSWeek,
                                    Date = verificationViewDto.ProductionDate
                                };
                                var inputEntryDest = new GetExePlantProductionEntryInput
                                {
                                    LocationCode = existWorkerAssignment.DestinationLocationCode,
                                    UnitCode = existWorkerAssignment.DestinationUnitCode,
                                    Shift = existWorkerAssignment.DestinationShift.ToString(),
                                    ProcessGroup = existWorkerAssignment.DestinationProcessGroup,
                                    Group = existWorkerAssignment.DestinationGroupCodeDummy,
                                    Brand = existWorkerAssignment.DestinationBrandCode,
                                    Year = verificationViewDto.KPSYear,
                                    Week = verificationViewDto.KPSWeek,
                                    Date = verificationViewDto.ProductionDate
                                };
                                var prodEntrySource = GetExePlantProductionEntryByCodeEmployeeID(inputEntrySource,
                                    exePlantProductionEntry.EmployeeID);
                                var prodEntryDest = GetExePlantProductionEntryByCodeEmployeeID(inputEntryDest,
                                    exePlantProductionEntry.EmployeeID);

                                if (existWorkerAssignment.SourceProcessGroup ==
                                    existWorkerAssignment.DestinationProcessGroup)
                                {
                                    production = (prodEntrySource.ProdActual ?? 0) + (prodEntryDest.ProdActual ?? 0);
                                    updahLain = 0;
                                }
                                else
                                {
                                    production = prodEntrySource.ProdActual??0;
                                    updahLain = (prodEntrySource.ProdTarget ?? 0) - (prodEntrySource.ProdActual ?? 0);
                                }
                            }
                        }
                    }
                }
                if (verificationViewDto.GroupCode.Substring(1, 1) == "5")
                {
                    var input = new GetExePlantWorkerAssignmentInput()
                    {
                        LocationCode = verificationViewDto.LocationCode,
                        UnitCode = verificationViewDto.UnitCode,
                        Shift = verificationViewDto.Shift,
                        Year = verificationViewDto.KPSYear,
                        Week = verificationViewDto.KPSWeek,
                        Date = verificationViewDto.ProductionDate,
                        SourceBrandCode = verificationViewDto.BrandCode,
                        ProductionDateFrom = verificationViewDto.ProductionDate,
                        DateTypeFilter = "rdbProductionDate"
                    };

                    var existWorkerAssignment = IsExistWorkerAssignmentByDestination(input).FirstOrDefault(t => t.EmployeeID == exePlantProductionEntry.EmployeeID);

                    if (existWorkerAssignment != null)
                    {
                        var inputEntrySource = new GetExePlantProductionEntryInput
                        {
                            LocationCode = existWorkerAssignment.SourceLocationCode,
                            UnitCode = existWorkerAssignment.SourceUnitCode,
                            Shift = existWorkerAssignment.SourceShift.ToString(),
                            ProcessGroup = existWorkerAssignment.SourceProcessGroup,
                            Group = existWorkerAssignment.SourceGroupCode,
                            Brand = existWorkerAssignment.SourceBrandCode,
                            Year = verificationViewDto.KPSYear,
                            Week = verificationViewDto.KPSWeek,
                            Date = verificationViewDto.ProductionDate
                        };
                        var inputEntryDest = new GetExePlantProductionEntryInput
                        {
                            LocationCode = existWorkerAssignment.DestinationLocationCode,
                            UnitCode = existWorkerAssignment.DestinationUnitCode,
                            Shift = existWorkerAssignment.DestinationShift.ToString(),
                            ProcessGroup = existWorkerAssignment.DestinationProcessGroup,
                            Group = existWorkerAssignment.DestinationGroupCodeDummy,
                            Brand = existWorkerAssignment.DestinationBrandCode,
                            Year = verificationViewDto.KPSYear,
                            Week = verificationViewDto.KPSWeek,
                            Date = verificationViewDto.ProductionDate
                        };

                        var prodEntryDest = GetExePlantProductionEntryByCodeEmployeeID(inputEntryDest,
                            exePlantProductionEntry.EmployeeID);
                        
                        var prodEntrySource = GetExePlantProductionEntryByCodeEmployeeID(inputEntrySource,
                            exePlantProductionEntry.EmployeeID);

                        var sourceActual = prodEntrySource == null ? 0 : prodEntrySource.ProdActual ?? 0;
                        var destActual = prodEntryDest == null ? 0 : prodEntryDest.ProdActual ?? 0;

                        var queryFilterProdCard = PredicateHelper.True<ProductionCard>();
                        queryFilterProdCard = queryFilterProdCard.And(c => c.LocationCode == existWorkerAssignment.SourceLocationCode);
                        queryFilterProdCard = queryFilterProdCard.And(c => c.UnitCode == existWorkerAssignment.SourceUnitCode);
                        queryFilterProdCard = queryFilterProdCard.And(c => c.Shift == existWorkerAssignment.SourceShift);
                        queryFilterProdCard = queryFilterProdCard.And(c => c.BrandCode == existWorkerAssignment.SourceBrandCode);
                        queryFilterProdCard = queryFilterProdCard.And(c => c.ProcessGroup == existWorkerAssignment.SourceProcessGroup);
                        queryFilterProdCard = queryFilterProdCard.And(c => c.GroupCode == existWorkerAssignment.SourceGroupCode);
                        queryFilterProdCard = queryFilterProdCard.And(c => c.RevisionType == 0);
                        queryFilterProdCard = queryFilterProdCard.And(c => c.EmployeeID == exePlantProductionEntry.EmployeeID);
                        queryFilterProdCard = queryFilterProdCard.And(c => c.ProductionDate == verificationViewDto.ProductionDate);

                        var productionCardSource = _productionCardRepo.Get(queryFilterProdCard).FirstOrDefault();

                        if (!String.IsNullOrEmpty(exePlantProductionEntry.AbsentType))
                        {
                            if (exePlantProductionEntry.AbsentType == EnumHelper.GetDescription(Enums.SKTAbsentCode.SLS) ||
                                exePlantProductionEntry.AbsentType == EnumHelper.GetDescription(Enums.MinValueAbsenType.SLPIjin) ||
                                exePlantProductionEntry.AbsentType == EnumHelper.GetDescription(Enums.SKTAbsentCode.SL4)) 
                            { 
                                var brandGroupCodeSource = _masterDataBll.GetBrandGruopCodeByBrandCode(existWorkerAssignment.SourceBrandCode);
                                var brandGroupCodeDest = _masterDataBll.GetBrandGruopCodeByBrandCode(existWorkerAssignment.DestinationBrandCode);

                                var processSettingsViewSource = _mstGenProcessSettingLocationViewRepo.Get(c => 
                                                                            c.LocationCode ==  existWorkerAssignment.SourceLocationCode &&
                                                                            c.ProcessGroup == existWorkerAssignment.SourceProcessGroup &&
                                                                            c.BrandGroupCode == brandGroupCodeSource).FirstOrDefault();

                                var processSettingsViewDest = _mstGenProcessSettingLocationViewRepo.Get(c => 
                                                                            c.LocationCode ==  existWorkerAssignment.DestinationLocationCode &&
                                                                            c.ProcessGroup == existWorkerAssignment.DestinationProcessGroup &&
                                                                            c.BrandGroupCode == brandGroupCodeDest).FirstOrDefault();
                                
                                var stickPerHourUomEblekSource = processSettingsViewSource == null ? 0 : (processSettingsViewSource.UOMEblek??0) == 0 ? 0 : (processSettingsViewSource.StdStickPerHour??0) / processSettingsViewSource.UOMEblek;
                                var stickPerHourUomEblekDest = processSettingsViewDest == null ? 0 : (processSettingsViewDest.UOMEblek??0) == 0 ? 0 : (processSettingsViewDest.StdStickPerHour??0) / processSettingsViewDest.UOMEblek;

                                var workhourSource = (exePlantProductionEntry.ProdActual ?? 0) / (stickPerHourUomEblekSource);
                                var workhourDest = (exePlantProductionEntry.ProdActual ?? 0) / (stickPerHourUomEblekDest);

                                var paidOthers = new float?();
                                if ((workhourSource + workhourDest) < 4)
                                {
                                    paidOthers = workhourDest * stickPerHourUomEblekSource;
                                }
                                else
                                {
                                    var mstProcessSetting = _masterDataBll.GetMasterProcessSettings(criteria).FirstOrDefault();
                                    var calc = (double)(mstGenStdHour.JknHour * (double)mstProcessSetting.StdStickPerHour.Value / (double)mstProcessSetting.UOMEblek.Value);
                                    paidOthers = (float)calc - exePlantProductionEntry.ProdActual;
                                }
                                if (productionCardSource != null)
                                {
                                    if (existWorkerAssignment.SourceProcessGroup ==
                                    existWorkerAssignment.DestinationProcessGroup)
                                    {
                                        productionCardSource.UpahLain = paidOthers;
                                    }
                                }
                            }
                        }

                        if (productionCardSource != null)
                        {
                            if (existWorkerAssignment.SourceProcessGroup ==
                            existWorkerAssignment.DestinationProcessGroup)
                            {
                                productionCardSource.Production = sourceActual + destActual;
                            }
                        }
                    }
                }
                if (!listProdcardRev0Emp.Any())
                {
                    var prodCard = new ProductionCardDTO
                    {
                        ProductionCardCode =
                            string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}", Enums.CombineCode.WPC,
                                verificationViewDto.LocationCode, verificationViewDto.Shift, verificationViewDto.UnitCode, groupCode,
                                verificationViewDto.BrandCode, verificationViewDto.KPSYear, verificationViewDto.KPSWeek,
                                (int)verificationViewDto.ProductionDate.Date.DayOfWeek),
                        RevisionType = 0,
                        LocationCode = verificationViewDto.LocationCode,
                        UnitCode = verificationViewDto.UnitCode,
                        Shift = verificationViewDto.Shift,
                        BrandCode = verificationViewDto.BrandCode,
                        BrandGroupCode = brandGroupCode,
                        ProcessGroup = verificationViewDto.ProcessGroup,
                        //GroupCode = dbEblekVer.GroupCode,
                        GroupCode = groupCode,
                        EmployeeID = exePlantProductionEntry.EmployeeID,
                        EmployeeNumber = exePlantProductionEntry.EmployeeNumber,
                        Production = production,
                        ProductionDate = verificationViewDto.ProductionDate,
                        WorkHours = exePlantProductionEntry.ExePlantProductionEntryVerification.WorkHour,
                        Absent = payrollAbsent,
                        UpahLain = updahLain,
                        EblekAbsentType = exePlantProductionEntry.AbsentCodeEblek,
                        Remark = null,
                        Comments = null,
                        CreatedBy = verificationViewDto.UpdatedBy,
                        UpdatedBy = verificationViewDto.UpdatedBy
                    };

                    var prodCardToInsert = Mapper.Map<ProductionCard>(prodCard);
                    prodCardToInsert.CreatedDate = DateTime.Now;
                    prodCardToInsert.UpdatedDate = DateTime.Now;

                    listProdCardInsert.Add(prodCardToInsert);
                }
                else
                {
                    var listDate = new List<DateTime>();
                    var endDate = _masterDataBll.GetClosingPayrollBeforeTodayDateTime(DateTime.Now.Date);
                    var startDate = _masterDataBll.GetClosingPayrollBeforeTodayDateTime(endDate.Date).AddDays(1);
                    for (int i = 0; i < 7; i++)
                    {
                        listDate.Add(startDate.AddDays(i));
                    }
                    if (listDate.Contains(verificationViewDto.ProductionDate))
                    {
                        var pc = _plantWagesBll.GetProductionCards(new GetProductionCardInput
                        {
                            LocationCode = verificationViewDto.LocationCode,
                            Unit = verificationViewDto.UnitCode,
                            Shift = verificationViewDto.Shift,
                            Brand = verificationViewDto.BrandCode,
                            Process = verificationViewDto.ProcessGroup,
                            Group = groupCode,
                            Date = verificationViewDto.ProductionDate
                        });
                        var currentRevType = pc.Select(m => m.RevisionType).Max();
                        var oldPc = _plantWagesBll.GetProductionCard(new ProductionCardDTO
                        {
                            RevisionType = 0,
                            LocationCode = verificationViewDto.LocationCode,
                            UnitCode = verificationViewDto.UnitCode,
                            BrandCode = verificationViewDto.BrandCode,
                            ProcessGroup = verificationViewDto.ProcessGroup,
                            GroupCode = groupCode,
                            EmployeeID = exePlantProductionEntry.EmployeeID,
                            ProductionDate = verificationViewDto.ProductionDate
                        });
                        if ((production - (oldPc.Production ?? 0)) != 0)
                        {
                            var prodCard = new ProductionCardDTO
                            {
                                ProductionCardCode =
                                    string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}", Enums.CombineCode.WPC,
                                        verificationViewDto.LocationCode, verificationViewDto.Shift, verificationViewDto.UnitCode, groupCode,
                                        verificationViewDto.BrandCode, verificationViewDto.KPSYear, verificationViewDto.KPSWeek,
                                        (int)verificationViewDto.ProductionDate.Date.DayOfWeek),
                                RevisionType = currentRevType + 1,
                                LocationCode = verificationViewDto.LocationCode,
                                UnitCode = verificationViewDto.UnitCode,
                                Shift = verificationViewDto.Shift,
                                BrandCode = verificationViewDto.BrandCode,
                                BrandGroupCode = brandGroupCode,
                                ProcessGroup = verificationViewDto.ProcessGroup,
                                //GroupCode = dbEblekVer.GroupCode,
                                GroupCode = groupCode,
                                EmployeeID = exePlantProductionEntry.EmployeeID,
                                EmployeeNumber = exePlantProductionEntry.EmployeeNumber,
                                Production = production - (oldPc.Production ?? 0),
                                ProductionDate = verificationViewDto.ProductionDate,
                                WorkHours = exePlantProductionEntry.ExePlantProductionEntryVerification.WorkHour,
                                Absent = payrollAbsent,
                                UpahLain = updahLain - oldPc.UpahLain,
                                EblekAbsentType = exePlantProductionEntry.AbsentCodeEblek,
                                Remark = null,
                                Comments = null,
                                CreatedBy = verificationViewDto.UpdatedBy,
                                UpdatedBy = verificationViewDto.UpdatedBy
                            };

                            var prodCardToInsert = Mapper.Map<ProductionCard>(prodCard);
                            prodCardToInsert.CreatedDate = DateTime.Now;
                            prodCardToInsert.UpdatedDate = DateTime.Now;

                            listProdCardInsert.Add(prodCardToInsert);
                        }
                    }
                    else
                    {
                         var prodCard = _plantWagesBll.GetProductionCard(new ProductionCardDTO()
                        {
                            RevisionType = 0,
                            LocationCode = verificationViewDto.LocationCode,
                            UnitCode = verificationViewDto.UnitCode,
                            BrandCode = verificationViewDto.BrandCode,
                            ProcessGroup = verificationViewDto.ProcessGroup,
                            GroupCode = groupCode,
                            EmployeeID = exePlantProductionEntry.EmployeeID,
                            ProductionDate = verificationViewDto.ProductionDate
                        });

                        prodCard.Absent = payrollAbsent;
                        prodCard.EblekAbsentType = exePlantProductionEntry.AbsentCodeEblek;
                        prodCard.Production = production;
                        prodCard.UpahLain = updahLain;

                        var prodCardToUpdate = Mapper.Map<ProductionCard>(prodCard);
                        prodCardToUpdate.UpdatedDate = DateTime.Now;

                        _productionCardRepo.Update(prodCardToUpdate);
                    }
                }
            }

            // Insert BULK to ProductionCard
            if (listProdCardInsert.Any())
            {
                using (var ctx = new SKTISEntities())
                {
                    var connectionString = ctx.Database.Connection.ConnectionString;
                    IDataReader reader = ObjectReader.Create(listProdCardInsert.AsEnumerable());

                    var bulkCopy = new SqlBulkCopy(connectionString);
                    bulkCopy.BatchSize = 50000;
                    bulkCopy.ColumnMappings.Clear();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i).Trim();
                        if (name != "MstGenBrand" && name != "MstPlantEmpJobsDataAll" && name != "MstPlantProductionGroup")
                            bulkCopy.ColumnMappings.Add(name, name);
                    }

                    bulkCopy.DestinationTableName = "ProductionCard";
                    bulkCopy.WriteToServer(reader);
                    bulkCopy.Close();
                }
            }

            _uow.SaveChanges();
        }

        public void GenerateProductionCardVerification_SP(IEnumerable<ExePlantProductionEntryVerificationViewDTO> listVerificationDTO, GetProductionCardInput input) 
        {
            // Get groups from verification which is want to generate
            var groupsConcat = new StringBuilder();
            var groups = listVerificationDTO.Select(c => c.GroupCode).Distinct().ToArray();
            for (int i = 0; i < groups.Count(); i++)
            {
                if (i == groups.Count() - 1) groupsConcat.Append(groups[i]);
                else
                {
                    groupsConcat.Append(groups[i]);
                    groupsConcat.Append("-");
                }
            }

            input.Group = groupsConcat.ToString();

            _sqlSPRepo.GenerateProductionCard_SP(input);

        }

        public void SendEmailSubmitTpotEntryVerif(GetExeTPOProductionEntryVerificationInput input, string currUserName)
        {
            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = input.LocationCode,
                BrandCode = input.BrandCode,
                KpsWeek = input.KPSWeek,
                KpsYear = input.KPSYear,
                FunctionName = Enums.PageName.TPOProductionEntryVerification.ToString(),
                ButtonName = Enums.ButtonName.Submit.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.TPOProductionEntryVerification),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.TPOFeeActual)
            };

            // Get User, Email, Responsibility Destination Recipient
            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            // Get User and Email Current/Sender
            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            // Create Email Input
            var listEmail = new List<MailInput>();
            foreach (var item in listUserAndEmailDestination)
            {
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                emailInput.UnitCode = item.Unit;
                var email = new MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailSubmitTpoEntryVerif(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }
        private string CreateBodyMailSubmitTpoEntryVerif(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            //var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("TPO Fee Draft sudah tersedia, Silakan melanjutkan proses berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/TPOFeeExeActual/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.KpsYear + "/"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + emailInput.IDResponsibility.ToString()
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public void SubmitProductionEntryVerification(string locationCode, string unit, string brand, int? shift, int? year, int? week, DateTime? date, string groupCode, string createdBy)
        {
            _sqlSPRepo.InsertPlantExeReportByGroups(locationCode, unit, brand, shift, year, week, date, groupCode, createdBy);
        }

        public void InsertReportByProcess(string locationCode, string brand, string process, int? year, int? week, string createdBy,
            string updatedBy, DateTime productionDate, string unitCode)
        {
            _sqlSPRepo.InsertPlantExeReportByProcess(locationCode, brand, process, year, week, createdBy, updatedBy, productionDate, unitCode);
        }

        public void InsertReportByProcessAsync(string locationCode, string brand, string process, int? year, int? week, string createdBy,
            string updatedBy, DateTime productionDate, string unitCode)
        {
            _sqlSPRepo.InsertPlantExeReportByProcessTask(locationCode, brand, process, year, week, createdBy, updatedBy, productionDate, unitCode);
        }

        public void InsertReportByProcessGenerator(string locationCode, string brand, string process, int? year, int? week, string createdBy,
            string updatedBy, DateTime productionDate, string unitCode) {
            _sqlSPRepo.GenerateByProcessGenerator(locationCode, brand, process, year, week, createdBy, updatedBy, productionDate, unitCode);
        }

        public void DefaultExeReportByProcess(string locationCode, string brandCode, string unitCode, int? year, int? week, DateTime productionDate, string createdBy, string updatedBy)
        {
            _sqlSPRepo.DefaultExeReportByProcess(locationCode, brandCode, unitCode, year, week, productionDate, createdBy, updatedBy);
        }

        public void InsertDefaultExeReportByProcess(string locationCode, string brandCode, string unitCode, int? year, int? week, DateTime productionDate, string createdBy, string updatedBy)
        {
            _sqlSPRepo.InsertDefaultExeReportByProcess(locationCode, brandCode, unitCode, year, week, productionDate, createdBy, updatedBy);
        }

        public void switchBrandExeReportByProcess(string locationCode, string brandGroupCode, DateTime productionDate)
        {
            _sqlSPRepo.switchBrandExeReportByProcess(locationCode, brandGroupCode, productionDate);
        }

        public void recalculateStockExeReportByProcess(string locationCode, string brandCode, DateTime productionDate)
        {
            _sqlSPRepo.recalculateStockExeReportByProcess(locationCode, brandCode, productionDate);
        }

        public void DeleteReportByProccess(string location, string unitCode, string brandCode, DateTime productionDateFrom, DateTime productionDateTo)
        {
            var listByProcess = _exeReportByProcessRepo.Get(c => c.LocationCode == location && 
                                                                 c.UnitCode == unitCode && 
                                                                 c.BrandCode == brandCode &&
                                                                 c.ProductionDate >= productionDateFrom &&
                                                                 c.ProductionDate <= productionDateTo);
            foreach (var item in listByProcess)
            {
                _exeReportByProcessRepo.Delete(item);
            }
            _uow.SaveChanges();
        }

        public void ReturnProductionEntryVerification(string productionEntryCode)
        {
            var currentData = _exePlantProductionEntryVerificationRepo.GetByID(productionEntryCode);
            if (currentData != null)
            {
                currentData.VerifySystem = false;
                currentData.VerifyManual = false;
                _exePlantProductionEntryVerificationRepo.Update(currentData);

                _uow.SaveChanges();
            }
        }
        public void UpdateWorkHourReportByGroup(GetExePlantProductionEntryVerificationInput input, float? totalActualValue, string currUsername)
        {
            int day = input.ProductionDate.Value.DayOfWeek == 0 ? 7 : (int)input.ProductionDate.Value.DayOfWeek;
            var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                     + input.LocationCode + "/"
                                     + input.Shift + "/"
                                     + input.UnitCode + "/"
                                     + input.GroupCode + "/"
                                     + input.BrandCode + "/"
                                     + input.KpsYear + "/"
                                     + input.KpsWeek + "/"
                                     + day;

            var productionEntryCodeDummy = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                     + input.LocationCode + "/"
                                     + input.Shift + "/"
                                     + input.UnitCode + "/"
                                     + input.GroupCode.Remove(1, 1).Insert(1, "5") + "/"
                                     + input.BrandCode + "/"
                                     + input.KpsYear + "/"
                                     + input.KpsWeek + "/"
                                     + day;

            var reportByGroup = _exeReportByGroupRepo.Get(c => c.LocationCode == input.LocationCode &&
                                                               c.GroupCode == input.GroupCode &&
                                                               c.BrandCode == input.BrandCode &&
                                                               c.ProductionDate == input.ProductionDate);

            if (reportByGroup.Any())
            {
                foreach (var item in reportByGroup)
                {
                    item.WorkHour = (float)_sqlSPRepo.GetWorkHour(input.LocationCode, input.UnitCode, input.GroupCode, input.BrandCode, input.Shift,
                                                                  input.ProcessGroup, input.ProductionDate, input.KpsYear, input.KpsWeek, day);

                    item.ValueHour = item.WorkHour == 0 ? 0 : item.Production / item.WorkHour;
                    item.ValuePeopleHour = item.ActualWorker == 0 ? 0 : item.ValueHour / item.ActualWorker;
                    item.ValuePeople = item.ActualWorker == 0 ? 0 : item.Production / item.ActualWorker;
                    item.UpdatedBy = currUsername;
                    item.UpdatedDate = DateTime.Now;

                    _exeReportByGroupRepo.Update(item);
                }
            }

            var groupCodeDummy = input.GroupCode.Remove(1, 1).Insert(1, "5");

            var reportByGroupDummy = _exeReportByGroupRepo.Get(c => c.LocationCode == input.LocationCode &&
                                                               c.GroupCode == groupCodeDummy &&
                                                               c.BrandCode == input.BrandCode &&
                                                               c.ProductionDate == input.ProductionDate);

            if (reportByGroupDummy.Any())
            {
                foreach (var item in reportByGroupDummy)
                {
                    item.WorkHour = (float)_sqlSPRepo.GetWorkHour(input.LocationCode, input.UnitCode, input.GroupCode, input.BrandCode, input.Shift, input.ProcessGroup,
                                                                  input.ProductionDate, input.KpsYear, input.KpsWeek, day);

                    item.ValueHour = item.WorkHour == 0 ? 0 : item.Production / item.WorkHour;
                    item.ValuePeopleHour = item.ActualWorker == 0 ? 0 : item.ValueHour / item.ActualWorker;
                    item.ValuePeople = item.ActualWorker == 0 ? 0 : item.Production / item.ActualWorker;
                    item.UpdatedBy = currUsername;
                    item.UpdatedDate = DateTime.Now;

                    _exeReportByGroupRepo.Update(item);
                }
            }

            _uow.SaveChanges();
        }

        public void RunSSISUpdateByGroupWeekly() 
        {
            _sqlSPRepo.RunSSISUpdateReportByGroupWeekly();
        }

        public void RunSSISUpdateByGroupMonthly()
        {
            _sqlSPRepo.RunSSISUpdateReportByGroupMonthly();
        }

        public List<ExePlantProductionEntryVerificationViewDTO> GetVerificationForFilterGroupCode(string locationCode, string unitCode, int shift, string processGroup, DateTime productionDate) 
        {
            var dbPlantEntryVerification = _exePlantProductionEntryVerificationRepo.Get(c => c.LocationCode == locationCode &&
                                                                                             c.UnitCode == unitCode &&
                                                                                             c.Shift == shift &&
                                                                                             c.ProcessGroup == processGroup &&
                                                                                             c.ProductionDate == productionDate);

            return Mapper.Map<List<ExePlantProductionEntryVerificationViewDTO>>(dbPlantEntryVerification);

        }

        public List<string> GetVerificationForFilterProcessGroup(string locationCode, string unitCode, int shift, DateTime productionDate)
        {
            var dbPlantEntryVerification = _exePlantProductionEntryVerificationRepo.Get(c => c.LocationCode == locationCode &&
                                                                                             c.UnitCode == unitCode &&
                                                                                             c.Shift == shift &&
                                                                                             c.ProductionDate == productionDate).OrderBy(c => c.ProcessOrder);

            return dbPlantEntryVerification.Select(c => c.ProcessGroup).Distinct().ToList();

        }

        public IEnumerable<string> GetListProcessVerification(string locationCode, string unitCode, string brandCode, int shift, DateTime productionDate) {
            // not verifiation but [ProcessSettingsAndLocationView]

            var mstgenbrand = _mstGenBrandRepo.GetByID(brandCode);
            var brandGroupCode = mstgenbrand == null ? string.Empty : mstgenbrand.BrandGroupCode;

            var dbPlantEntryVerification = _mstGenProcessSettingLocationViewRepo.Get(c => c.LocationCode == locationCode &&
                                                                                             c.BrandGroupCode == brandGroupCode).OrderByDescending(c => c.ProcessOrder);

            return dbPlantEntryVerification.Select(c => c.ProcessGroup).Distinct().ToList();
        }
        #endregion

        #region WorkerBalancing
        public List<ExePlantWorkerBalancingViewDTO> GetExePlantWorkerLoadBalancing(GetExePlantWorkerLoadBalancingMulti input)
        {
            var queryFilter = PredicateHelper.True<ExePlantWorkerBalancingMulti>();
            if (!string.IsNullOrEmpty(input.SourceLocationCode))
                queryFilter = queryFilter.And(m => m.SourceLocationCode == input.SourceLocationCode);
            if (!string.IsNullOrEmpty(input.SourceUnitCode))
                queryFilter = queryFilter.And(m => m.SourceUnitCode == input.SourceUnitCode);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantWorkerBalancingMulti>();
            var dbResult = _exePlantWorkerBalancingViewRepo.Get(queryFilter);
            return Mapper.Map<List<ExePlantWorkerBalancingViewDTO>>(dbResult);
        }

        public List<ExePlantWorkerBalancingSingleViewDTO> GetPlantWorkerLoadBalancingSingle(GetExePlantWorkerLoadBalancingSingle input)
        {
            var queryFilter = PredicateHelper.True<ExePlantWorkerBalancingSingle>();
            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCodeSource))
                queryFilter = queryFilter.And(m => m.UnitCodeSource == input.UnitCodeSource);
            if (!string.IsNullOrEmpty(input.GroupCode))
                queryFilter = queryFilter.And(m => m.GroupCode == input.GroupCode);
            if (!string.IsNullOrEmpty(input.ProcessGroup))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.ProcessGroup);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantWorkerBalancingSingle>();
            var dbResult = _exePlantWorkerBalancingSingleViewRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<ExePlantWorkerBalancingSingleViewDTO>>(dbResult);
        }
        #endregion

        #region  ExePlantProductionEntryVerification
        public List<string> GetBrandCodeByLocationYearAndWeekEntryVerification(string locationCode, int? KPSYear, int? KPSWeek)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (KPSYear != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSYear == KPSYear);
            }

            if (KPSWeek != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSWeek == KPSWeek);
            }

            if (locationCode != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.LocationCode == locationCode);
            }

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);

            if (dbResult.Count() > 0)
            {
                return dbResult.Select(m => m.BrandCode).Distinct().ToList();
            }

            return new List<string>();
        }

        public List<ExePlantProductionEntryVerificationDTO> GetBrandCodeByLocationYearWeekProcessFromEntryVerification(string locationCode, string ProcessGroup, int? KPSYear, int? KPSWeek)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (KPSYear != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSYear == KPSYear);
            }

            if (KPSWeek != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSWeek == KPSWeek);
            }

            if (!string.IsNullOrEmpty(locationCode))
            {
                queryFilter = queryFilter.And(wpp => wpp.LocationCode == locationCode);
            }

            if (!string.IsNullOrEmpty(ProcessGroup))
            {
                queryFilter = queryFilter.And(wpp => wpp.ProcessGroup.ToLower() == ProcessGroup.ToLower());
            }

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);

            return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult);
        }

        public List<ExePlantProductionEntryVerificationDTO> GetExePlantProductionEntryVerification(
            GetExePlantProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(q => q.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.ProcessGroup))
            {
                queryFilter = queryFilter.And(q => q.ProcessGroup.ToLower() == input.ProcessGroup.ToLower());
            }

            if (input.KpsYear != null)
            {
                queryFilter = queryFilter.And(q => q.KPSYear == input.KpsYear);
            }

            if (input.KpsWeek != null)
            {
                queryFilter = queryFilter.And(q => q.KPSWeek == input.KpsWeek);
            }

            if (!string.IsNullOrEmpty(input.GroupCode))
            {
                queryFilter = queryFilter.And(q => q.GroupCode == input.GroupCode);
            }

            if (input.Shift != null)
            {
                queryFilter = queryFilter.And(q => q.Shift == input.Shift);
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(q => q.UnitCode == input.UnitCode);
            }

            if (input.Date.HasValue)
            {
                queryFilter = queryFilter.And(q => q.ProductionDate == input.ProductionDate.Value.Date);
            }

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);

            return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult);
        }

        public List<ExePlantProductionEntryVerificationDTO> GetPlantProductionEntryVerificationByLocationYearWeek(
            string locationCode,
            int? KPSYear,
            int? KPSWeek
        )
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (KPSYear != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSYear == KPSYear);
            }

            if (KPSWeek != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSWeek == KPSWeek);
            }

            if (!string.IsNullOrEmpty(locationCode))
            {
                queryFilter = queryFilter.And(wpp => wpp.LocationCode == locationCode);
            }

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);

            return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult);
        }

        public List<string> GetGroupCodePlantVerification(string locationCode, string unitCode, string process, int week)
        {
            return _exePlantProductionEntryVerificationRepo.Get(c => c.LocationCode == locationCode && c.UnitCode == unitCode &&
                c.ProcessGroup == process && c.KPSWeek == week && c.GroupCode.Substring(1, 1) != "5").Select(c => c.GroupCode).Distinct().ToList();
        }

        public List<string> GetGroupCodePlantTPK(string locationCode, string unitCode, string process, string brandCode, int year, int week)
        {
            var listGroup = _planPlantTargetProductionKelompokRepo.Get(c => c.LocationCode == locationCode && c.UnitCode == unitCode && c.ProcessGroup == process
                && c.BrandCode == brandCode && c.KPSYear == year && c.KPSWeek == week);

            return listGroup.Select(c => c.GroupCode).Distinct().ToList();
        }
        #endregion

        public int GetLatestProdCardRevType(string location, string unit, string brand, string process, string group, DateTime? productionDate)
        {
            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (!string.IsNullOrEmpty(location))
                queryFilter = queryFilter.And(m => m.LocationCode == location);

            if (!string.IsNullOrEmpty(unit))
                queryFilter = queryFilter.And(m => m.UnitCode == unit);

            if (!string.IsNullOrEmpty(brand))
                queryFilter = queryFilter.And(m => m.BrandCode == brand);

            if (!string.IsNullOrEmpty(process))
                queryFilter = queryFilter.And(m => m.ProcessGroup == process);

            if (!string.IsNullOrEmpty(group))
                queryFilter = queryFilter.And(m => m.GroupCode == group);

            if (productionDate != null)
                queryFilter = queryFilter.And(m => m.ProductionDate == productionDate);

            var prodCard = _productionCardRepo.Get(queryFilter).OrderByDescending(c => c.RevisionType).FirstOrDefault();

            if (prodCard != null)
                return prodCard.RevisionType;

            return 0;
        }

        #region EMS Source Data
        public List<string> GetBrandCodeByLocationDateFromDateToEntryVerification(string locationCode, DateTime? DateFrom, DateTime? DateTo)
        {
            var queryFilter = PredicateHelper.True<EMSSourceDataBrandView>();

            if (DateFrom != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.StartDate >= DateFrom);
            }

            if (DateTo != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.EndDate <= DateTo);
            }

            if (locationCode != null)
            {
                if (locationCode == Enums.LocationCode.SKT.ToString())
                { }
                else if (locationCode == Enums.LocationCode.PLNT.ToString())
                { queryFilter = queryFilter.And(wpp => wpp.ParentLocationCode == locationCode); }
                else if (locationCode == Enums.LocationCode.TPO.ToString())
                { queryFilter = queryFilter.And(wpp => wpp.ParentLocationCode.Contains("REG")); }
                else if (locationCode == "REG1" || locationCode == "REG2" || locationCode == "REG3" || locationCode == "REG4")
                { queryFilter = queryFilter.And(wpp => wpp.ParentLocationCode.Contains(locationCode)); }
                else
                { queryFilter = queryFilter.And(wpp => wpp.LocationCode == locationCode); }
            }

            var dbResult = _EMSSourceDataBrandViewRepo.Get(queryFilter);

            if (dbResult.Count() > 0)
            {
                return dbResult.Select(m => m.BrandCode).Distinct().ToList();
            }

            return new List<string>();
        }

        public List<string> GetBrandCodeByLocationDateFromDateToBySp(string locationCode, DateTime? dateFrom, DateTime? dateTo)
        {
            var spResult = _sqlSPRepo.GetEmsSourceDataBrandCode(locationCode, dateFrom, dateTo);
            if (spResult == null)
            {
                return null;
            }
            return spResult.ToList();
        }

        #endregion

        public IEnumerable<string> GetListProcessGeneratorByProcess(string locationCode, string unitCode, string brandCode, int shift, DateTime productionDate) {
            // not verifiation but [ProcessSettingsAndLocationView]

            var mstgenbrand = _mstGenBrandRepo.GetByID(brandCode);
            var brandGroupCode = mstgenbrand == null ? string.Empty : mstgenbrand.BrandGroupCode;

            var dbPlantEntryVerification = _mstGenProcessSettingLocationViewRepo.Get(c => c.LocationCode == locationCode &&
                                                                                             c.BrandGroupCode == brandGroupCode).OrderByDescending(c => c.ProcessOrder);

            return dbPlantEntryVerification.Select(c => c.ProcessGroup).Distinct().ToList();
        }

        #region Multiple Absenteeism Pop Up - CR1

        public IEnumerable<string> GetListAbsentActiveOnAbsenteeism() 
        {
            IEnumerable<string> listAbsent = _exeMstPlantAbsentTypeRepo.Get(c => c.ActiveInAbsent == true).Select(c => c.AbsentType);

            return listAbsent;
        }

        public List<EmployeeMultipleInsertAbsenteeismDTO> InsertMultipleAbsenteeism(InsertMultipleAbsenteeismDTO datas)
        {
            // Get Master Absent Type
            var mstAbsent = _exeMstPlantAbsentTypeRepo.GetByID(datas.AbsentType);
            var sktAbsentCode = mstAbsent == null ? "" : mstAbsent.SktAbsentCode;
            var payrollAbsentCode = mstAbsent == null ? "" : mstAbsent.PayrollAbsentCode;
            var maxDayAbsent = mstAbsent == null ? 0 : mstAbsent.MaxDay;

            // Validate to Production Entry
            ValidateToEblek(datas);

            // Validate to current Absenteeism
            ValidateToAbsenteeism(datas, maxDayAbsent);

            foreach (var employee in datas.ListEmployees.Where(c => c.IsChecked == true && c.IsValidated == true))
            {
                employee.ResponseType = "Success";
                var newEmployeeInsertAbsenteeism = new ExePlantWorkerAbsenteeismDTO()
                {
                    EmployeeID = employee.EmployeeID,
                    EmployeeNumber = employee.EmployeeNumber,
                    StartDateAbsent = datas.StartDateAbsent,
                    EndDateAbsent = datas.EndDateAbsent,
                    AbsentType = datas.AbsentType,
                    LocationCode = datas.LocationCode,
                    UnitCode = datas.UnitCode,
                    GroupCode = datas.GroupCode,
                    Shift = datas.Shift,
                    TransactionDate = DateTime.Now,
                    SktAbsentCode = mstAbsent == null ? "" : mstAbsent.SktAbsentCode,
                    PayrollAbsentCode = mstAbsent == null ? "" : mstAbsent.PayrollAbsentCode,
                    CreatedBy = datas.CreatedBy,
                    UpdatedBy = datas.UpdatedBy,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                if (newEmployeeInsertAbsenteeism.GroupCode == null) newEmployeeInsertAbsenteeism.GroupCode = _mstPlantEmpJobsdataAcvRepo.GetByID(newEmployeeInsertAbsenteeism.EmployeeID).GroupCode;

                var dbExePlantWorkerAbsenteeism = Mapper.Map<ExePlantWorkerAbsenteeism>(newEmployeeInsertAbsenteeism);

                dbExePlantWorkerAbsenteeism.CreatedDate = DateTime.Now;
                dbExePlantWorkerAbsenteeism.UpdatedDate = DateTime.Now;

                using (SKTISEntities context = new SKTISEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            context.INSERT_WORKER_ABSENTEEISM
                            (
                                dbExePlantWorkerAbsenteeism.StartDateAbsent,
                                dbExePlantWorkerAbsenteeism.EmployeeID,
                                dbExePlantWorkerAbsenteeism.AbsentType,
                                dbExePlantWorkerAbsenteeism.EndDateAbsent,
                                dbExePlantWorkerAbsenteeism.SktAbsentCode,
                                dbExePlantWorkerAbsenteeism.PayrollAbsentCode,
                                dbExePlantWorkerAbsenteeism.ePaf,
                                dbExePlantWorkerAbsenteeism.Attachment,
                                dbExePlantWorkerAbsenteeism.AttachmentPath,
                                dbExePlantWorkerAbsenteeism.CreatedDate,
                                dbExePlantWorkerAbsenteeism.CreatedBy,
                                dbExePlantWorkerAbsenteeism.UpdatedDate,
                                dbExePlantWorkerAbsenteeism.UpdatedBy,
                                dbExePlantWorkerAbsenteeism.EmployeeNumber,
                                dbExePlantWorkerAbsenteeism.LocationCode,
                                dbExePlantWorkerAbsenteeism.UnitCode,
                                dbExePlantWorkerAbsenteeism.GroupCode,
                                dbExePlantWorkerAbsenteeism.TransactionDate,
                                dbExePlantWorkerAbsenteeism.Shift
                            );

                            context.SaveChanges();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }

            return datas.ListEmployees;
        }

        private void ValidateToEblek(InsertMultipleAbsenteeismDTO datas)
        {
            var closingDatePayroll = _masterDataBll.GetClosingPayrollBeforeTodayDateTime(DateTime.Now);

            // Check closing payroll
            foreach (var employee in datas.ListEmployees.Where(c => c.IsChecked == true && c.IsValidated == true))
            {
                if (datas.StartDateAbsent <= closingDatePayroll) 
                {
                    if (employee.IsValidated)
                    {
                        employee.ResponseType = "Failed - Start Date absent < Closing Payroll";
                        employee.IsValidated = false;
                    }
                }
            }

            // Create combine code Production Entry Code
            var prodEntryCode = string.Format("{0}/{1}/{2}/{3}/{4}",
                                Enums.CombineCode.EBL.ToString(),
                                datas.LocationCode,
                                datas.Shift,
                                datas.UnitCode,
                                datas.GroupCode);

            var listProdEntryCode = new[] { new { ProductionEntryCode = "", EmployeeID = "", EmployeeNumber = "", ProductionDate = DateTime.Now.Date } }.ToList();
            listProdEntryCode.Clear();

            foreach (var employee in datas.ListEmployees.Where(c => c.IsChecked == true && c.IsValidated == true))
            {
                // Validate enddate
                if (datas.EndDateAbsent.Date < datas.StartDateAbsent.Date)
                {
                    if (employee.IsValidated)
                    {
                        employee.ResponseType = "Failed - End date absent less than start date absent";
                        employee.IsValidated = false;
                    }
                }

                // Validate
                if (string.IsNullOrEmpty(datas.AbsentType))
                {
                    if (employee.IsValidated)
                    {
                        employee.ResponseType = "Failed - Absent type null ";
                        employee.IsValidated = false;
                    }

                }

                using (SKTISEntities dbContext = new SKTISEntities())
                {
                    dbContext.Configuration.AutoDetectChangesEnabled = false;

                    listProdEntryCode.AddRange(dbContext.ExePlantProductionEntries.AsNoTracking().Where(m => m.ProductionEntryCode.StartsWith(prodEntryCode)
                                                                   && m.EmployeeID == employee.EmployeeID
                                                                   && m.ExePlantProductionEntryVerification.ProductionDate >= datas.StartDateAbsent.Date
                                                                   && m.ExePlantProductionEntryVerification.ProductionDate <= datas.EndDateAbsent.Date)
                                                                   .Select(c => new
                                                                   {
                                                                       ProductionEntryCode = c.ProductionEntryCode,
                                                                       EmployeeID = c.EmployeeID,
                                                                       EmployeeNumber = c.EmployeeNumber,
                                                                       ProductionDate = c.ExePlantProductionEntryVerification.ProductionDate
                                                                   }).Distinct());
                }

            }

            if (listProdEntryCode.Any())
            {
                var listTranslog = _utilitiesBll.GetLatestTransLogExceptSaveEblekAbsenteeism(listProdEntryCode.Select(c => c.ProductionEntryCode).Distinct().ToList(), Enums.PageName.PlantProductionEntry.ToString());

                foreach (var item in listTranslog)
                {
                    if (item != null)
                    {
                        var employeeID = listProdEntryCode.Where(c => c.ProductionEntryCode == item.TransactionCode).Select(c => c.EmployeeID).FirstOrDefault();
                        var productionDate = listProdEntryCode.Where(c => c.ProductionEntryCode == item.TransactionCode).Select(c => c.ProductionDate).FirstOrDefault();

                        var translogRelease = _utilitiesBll.GetLatestActionTransLogExceptSave(item.TransactionCode, Enums.PageName.EblekReleaseApproval.ToString());
                        if (translogRelease != null)
                        {
                            if (translogRelease.CreatedDate < item.CreatedDate)
                            {
                                foreach (var employee in listProdEntryCode.Where(c => c.ProductionEntryCode == item.TransactionCode).Select(c => c.EmployeeID))
                                {
                                    foreach (var em in datas.ListEmployees.Where(c => c.EmployeeID == employee && c.IsChecked))
                                    {
                                        if (em.IsValidated) 
                                        {
                                            em.ResponseType = "Failed - " + "Already Submitted Production Entry between " + datas.StartDateAbsent.ToShortDateString() + " and " + datas.EndDateAbsent.ToShortDateString();
                                            em.IsValidated = false;
                                            em.ProductionDate = productionDate; 
                                        }
                                    }
                                }
                                
                                //throw new BLLException(ExceptionCodes.BLLExceptions.EblekSubmitted, "There is <strong> Already Submitted Production Entry for Employee </strong>" + employeeID + "<strong> in " + productionDate.ToShortDateString() + " </strong>");
                            }
                        }
                        else
                        {
                            if (item.UtilFlow.UtilFunction.FunctionName == HMS.SKTIS.Core.Enums.ButtonName.Submit.ToString())
                            {
                                foreach (var employee in listProdEntryCode.Where(c => c.ProductionEntryCode == item.TransactionCode).Select(c => c.EmployeeID))
                                {
                                    foreach (var em in datas.ListEmployees.Where(c => c.EmployeeID == employee && c.IsChecked))
                                    {
                                        if (em.IsValidated)
                                        {
                                            em.ResponseType = "Failed - " + "Already Submitted Production Entry between " + datas.StartDateAbsent.ToShortDateString() + " and " + datas.EndDateAbsent.ToShortDateString();
                                            em.IsValidated = false;
                                            em.ProductionDate = productionDate;
                                        }
                                    }
                                }
                                //throw new BLLException(ExceptionCodes.BLLExceptions.EblekSubmitted, "There is <strong> Already Submitted Production Entry for Employee </strong>" + employeeID + "<strong> in " + productionDate.ToShortDateString() + " </strong>");
                            }
                        }
                    }
                }
            }
        }

        private void ValidateToAbsenteeism(InsertMultipleAbsenteeismDTO datas, int maxDayAbsent)
        {
            var allEmps = datas.ListEmployees.Select(c => c.EmployeeID).ToList();
            var listMstEmployee = _mstPlantEmpJobsdataAcvRepo.Get(c => allEmps.Contains(c.EmployeeID)).ToList();
            foreach (var item in datas.ListEmployees)
            {
                item.EmployeeName = listMstEmployee.Where(c => c.EmployeeID == item.EmployeeID).FirstOrDefault().EmployeeName;
            }

            var listEmployees = datas.ListEmployees.Where(c => c.IsChecked == true && c.IsValidated == true).Select(c => c.EmployeeID);

            using (SKTISEntities dbContext = new SKTISEntities())
            {
                dbContext.Configuration.AutoDetectChangesEnabled = false;

                var listCurrentAbsenteeism = dbContext.ExePlantWorkerAbsenteeism.AsNoTracking().Where(c => listEmployees.Contains(c.EmployeeID)).ToList();

                // Validate primary key
                foreach (var employee in datas.ListEmployees.Where(c => c.IsChecked == true && c.IsValidated == true))
                {
                    var currAbsenteeism = listCurrentAbsenteeism.Where(c => c.EmployeeID == employee.EmployeeID && c.StartDateAbsent == datas.StartDateAbsent && c.Shift == datas.Shift);
                    if (currAbsenteeism.Any())
                    {
                        if (employee.IsValidated) 
                        {
                            employee.ResponseType = "Failed - There is already absenteeism between " + datas.StartDateAbsent.ToShortDateString() + " and " + datas.EndDateAbsent.ToShortDateString();
                            employee.IsValidated = false;
                        }
                        
                    }

                    // Validate range date
                    //worker absen same type
                    var existingPlantWorkerAbsenteeisms = listCurrentAbsenteeism.Where(m => m.AbsentType == datas.AbsentType && m.EmployeeID == employee.EmployeeID).ToList();

                    if (existingPlantWorkerAbsenteeisms.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(datas.StartDateAbsent,
                        datas.EndDateAbsent, existingPlantWorkerAbsenteeism.StartDateAbsent,
                        existingPlantWorkerAbsenteeism.EndDateAbsent)))
                    {
                        if (employee.IsValidated)
                        {
                            employee.ResponseType = "Failed - There is already absenteeism between " + datas.StartDateAbsent.ToShortDateString() + " and " + datas.EndDateAbsent.ToShortDateString();
                            employee.IsValidated = false;
                        }
                        
                    }

                    var existingPlantWorkerAbsenteeismsDifType = listCurrentAbsenteeism.Where(m => m.EmployeeID == employee.EmployeeID).ToList();

                    if (existingPlantWorkerAbsenteeismsDifType.Any(existingPlantWorkerAbsenteeism => !DataRangeIsNotUnionOrOverlap(datas.StartDateAbsent,
                        datas.EndDateAbsent, existingPlantWorkerAbsenteeism.StartDateAbsent,
                        existingPlantWorkerAbsenteeism.EndDateAbsent)))
                    {
                        if (employee.IsValidated)
                        {
                            employee.ResponseType = "Failed - There is already absenteeism between " + datas.StartDateAbsent.ToShortDateString() + " and " + datas.EndDateAbsent.ToShortDateString();
                            employee.IsValidated = false;
                        }
                    }

                    // validate max absent
                    //get maxday absent by absent type
                    var absentMaxDays = maxDayAbsent;

                    if (absentMaxDays <= 0) continue;

                    //get current total absen day
                    var existPlantWorkerAbsenteeism = listCurrentAbsenteeism.Where(m => m.AbsentType == datas.AbsentType
                                                                                                && m.EmployeeID == employee.EmployeeID
                                                                                                && m.StartDateAbsent.Year == datas.StartDateAbsent.Year).ToList();

                    var totalWorkerAbsentDays = existPlantWorkerAbsenteeism.Sum(c => (c.EndDateAbsent.Date - c.StartDateAbsent.Date).Days);

                    //validate if the absent max days less than current total worker absent days + user input
                    var userInputAbsentDays = (datas.EndDateAbsent.Date - datas.StartDateAbsent.Date).Days;
                    var maxDaysOverage = absentMaxDays - (totalWorkerAbsentDays + userInputAbsentDays);

                    //validate if total input absent days > max day absent type
                    if (userInputAbsentDays > absentMaxDays)
                    {
                        if (employee.IsValidated)
                        {
                            employee.ResponseType = "Failed - Max Day reached for absent " + datas.AbsentType;
                            employee.IsValidated = false;
                        }
                    }
                    else if (userInputAbsentDays < totalWorkerAbsentDays && userInputAbsentDays < absentMaxDays)
                    {
                        continue;
                    }

                    //validate if the absent max days less than current total worker absent days
                    if (totalWorkerAbsentDays >= absentMaxDays)
                    {
                        if (employee.IsValidated)
                        {
                            employee.ResponseType = "Failed - Max Day reached for absent " + datas.AbsentType;
                            employee.IsValidated = false;
                        }
                    }

                    if (maxDaysOverage > absentMaxDays)
                    //if (maxDaysOverage > 0)
                    {
                        if (employee.IsValidated)
                        {
                            employee.ResponseType = "Failed - Max Day reached for absent " + datas.AbsentType;
                            employee.IsValidated = false;
                        }
                        
                    }
                }
            }
        }

        #endregion
    }
}
