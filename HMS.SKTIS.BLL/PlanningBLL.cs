using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System.Data.Entity;

namespace HMS.SKTIS.BLL
{
    public partial class PlanningBLL : IPlanningBLL
    {
        private IUnitOfWork _uow;
        private IMasterDataBLL _masterDataBll;
        private IUserBLL _userBll;
        private IGenericRepository<PlanWeeklyProductionPlanning> _planWeeklyProductionPlaningRepo;
        private IGenericRepository<PlanTmpWeeklyProductionPlanning> _planTmpWeeklyProductionPlaningRepo;
        private IGenericRepository<PlanTargetProductionUnit> _planTPURepo;
        private IGenericRepository<PlanTPOTargetProductionKelompok> _planTPOTPKRepo;
        private IGenericRepository<PlanPlantGroupShift> _planGroupShiftRepo;
        private IGenericRepository<TargetProductionUnitView> _targetProductionUnitViewRepo;
        private IGenericRepository<PlanPlantIndividualCapacityWorkHour> _planningPlantIndividualCapacityWorkHourRepo;
        private IGenericRepository<PlanPlantIndividualCapacityByReferenceView> _planningPlantIndividualCapacityByReference;
        private IGenericRepository<MstPlantProductionGroup> _mstPlantProductionGroupRepo;
        private IGenericRepository<PlanPlantTargetProductionKelompok> _planPlantTargetProductionKelompokRepo;
        private IGenericRepository<PlanPlantWIPDetail> _planPlantWIPDetailRepo;
        private ISqlSPRepository _sqlSPRepo;
        private IGenericRepository<ExePlantProductionEntryVerification> _exePlantProductionEntryVerificationRepo;
        private IGenericRepository<PlanPlantAllocation> _planPlantAllocationRepository;
        private IGenericRepository<MstGenBrand> _mstGenBrandRepo;
        private IGenericRepository<TPOTargetProductionKelompokView> _planTPOTPKViewRepo;
        private IGenericRepository<TargetProductionUnitPerBoxView> _targetProductionUnitPerBoxViewRepo;
        private IGenericRepository<PlanTPOTargetProductionKelompokBox> _planTPOTPKInBoxRepo;
        private IGenericRepository<UtilTransactionLog> _utilTransactionLogRepo;
        private IGenericRepository<MstGenProcessSettingsLocation> _processSettingLocationRepo;
        private IGenericRepository<MstTableauReport> _mstTableauReportRepo;
        private IGenericRepository<MstGenProcess> _mstGenProcess;
        private IGenericRepository<MstADTemp> _mstAdTemp;
        private IGenericRepository<ExeTPOProductionEntryVerification> _exeTPOProductionEntryVerification;
        private IGenericRepository<MstPlantEmpJobsDataAcv> _MstPlantEmpJobsDataAcv;
        private IGenericRepository<ExeReportDailyProductionAchievementView> _exeReportDailyProductionAchievmentRepo;
        private IUtilitiesBLL _utilitiesBll;
        private IGenericRepository<AvailabelPositionNumberGroup> _AvailabelPositionNumberGroupRepo;
        private IGenericRepository<ProcessSettingsAndLocationView> _mstGenProcessSettingLocationViewRepo; 

        public PlanningBLL(IUnitOfWork uow, IMasterDataBLL masterDataBll, IUserBLL userBll, IUtilitiesBLL utilitiesBll)
        {
            _uow = uow;
            _masterDataBll = masterDataBll;
            _userBll = userBll;
            _planWeeklyProductionPlaningRepo = _uow.GetGenericRepository<PlanWeeklyProductionPlanning>();
            _planTmpWeeklyProductionPlaningRepo = _uow.GetGenericRepository<PlanTmpWeeklyProductionPlanning>();
            _planTPURepo = _uow.GetGenericRepository<PlanTargetProductionUnit>();
            _planTPOTPKRepo = _uow.GetGenericRepository<PlanTPOTargetProductionKelompok>();
            _planGroupShiftRepo = _uow.GetGenericRepository<PlanPlantGroupShift>();
            _targetProductionUnitViewRepo = _uow.GetGenericRepository<TargetProductionUnitView>();
            _planningPlantIndividualCapacityWorkHourRepo = _uow.GetGenericRepository<PlanPlantIndividualCapacityWorkHour>();
            _planningPlantIndividualCapacityByReference = _uow.GetGenericRepository<PlanPlantIndividualCapacityByReferenceView>();
            _sqlSPRepo = _uow.GetSPRepository();
            _mstPlantProductionGroupRepo = _uow.GetGenericRepository<MstPlantProductionGroup>();
            _planPlantTargetProductionKelompokRepo = _uow.GetGenericRepository<PlanPlantTargetProductionKelompok>();
            _planPlantWIPDetailRepo = _uow.GetGenericRepository<PlanPlantWIPDetail>();
            _exePlantProductionEntryVerificationRepo = _uow.GetGenericRepository<ExePlantProductionEntryVerification>();
            _planPlantAllocationRepository = _uow.GetGenericRepository<PlanPlantAllocation>();
            _mstGenBrandRepo = _uow.GetGenericRepository<MstGenBrand>();
            _planTPOTPKViewRepo = _uow.GetGenericRepository<TPOTargetProductionKelompokView>();
            _targetProductionUnitPerBoxViewRepo = _uow.GetGenericRepository<TargetProductionUnitPerBoxView>();
            _planTPOTPKInBoxRepo = _uow.GetGenericRepository<PlanTPOTargetProductionKelompokBox>();
            _utilTransactionLogRepo = _uow.GetGenericRepository<UtilTransactionLog>();
            _processSettingLocationRepo = _uow.GetGenericRepository<MstGenProcessSettingsLocation>();
            _mstTableauReportRepo = _uow.GetGenericRepository<MstTableauReport>();
            _mstGenProcess = _uow.GetGenericRepository<MstGenProcess>();
            _mstAdTemp = _uow.GetGenericRepository<MstADTemp>();
            _exeTPOProductionEntryVerification = _uow.GetGenericRepository<ExeTPOProductionEntryVerification>();
            _MstPlantEmpJobsDataAcv = _uow.GetGenericRepository<MstPlantEmpJobsDataAcv>();
            _exeReportDailyProductionAchievmentRepo = _uow.GetGenericRepository<ExeReportDailyProductionAchievementView>();
            _utilitiesBll = utilitiesBll;
            _AvailabelPositionNumberGroupRepo = _uow.GetGenericRepository<AvailabelPositionNumberGroup>();
            _mstGenProcessSettingLocationViewRepo = _uow.GetGenericRepository<ProcessSettingsAndLocationView>();
        }

        #region WPP

        public double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        /// <summary>
        /// Get WPP
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<PlanWeeklyProductionPlanningDTO> GetPlanWeeklyProductionPlannings(PlanWeeklyProductionPlanningInput input)
        {
            var queryFilter = PredicateHelper.True<PlanWeeklyProductionPlanning>();

            if (!string.IsNullOrEmpty(input.BrandFamily))
            {
                queryFilter = queryFilter.And(wpp => wpp.MstGenBrand.MstGenBrandGroup.BrandFamily == input.BrandFamily);
            }

            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                var brandcodes = input.BrandCode.Split(',');
                queryFilter = queryFilter.And(wpp => brandcodes.Contains(wpp.BrandCode));
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var location = _sqlSPRepo.GetLocations(input.LocationCode).Select(loc => loc.LocationCode).ToList();
                queryFilter = queryFilter.And(wpp => location.Contains(wpp.LocationCode));
            }

            if (input.KPSYear != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSYear == input.KPSYear);
            }

            if (input.KPSWeek != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSWeek == input.KPSWeek);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<PlanWeeklyProductionPlanning>();

            var dbResult = _planWeeklyProductionPlaningRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<PlanWeeklyProductionPlanningDTO>>(dbResult);
        }

        /// <summary>
        /// Delete all Temporary  weekly production planning
        /// </summary>
        public void DeleteAllTempWPP()
        {
            _planTmpWeeklyProductionPlaningRepo.DeleteAll();
            _uow.SaveChanges();
        }

        /// <summary>
        /// Insert temp WPP
        /// </summary>
        /// <param name="wpp"></param>
        public void InsertTempWPP(PlanWeeklyProductionPlanningDTO wpp)
        {
            var db = Mapper.Map<PlanTmpWeeklyProductionPlanning>(wpp);

            db.CreatedBy = wpp.UpdatedBy;
            db.CreatedDate = DateTime.Now;
            db.UpdatedDate = DateTime.Now;

            _planTmpWeeklyProductionPlaningRepo.Insert(db);


            _uow.SaveChanges();
        }

        /// <summary>
        /// Insert temp WPP
        /// </summary>
        /// <param name="wpp"></param>
        public void UpdateTempWPP(PlanWeeklyProductionPlanningDTO wpp)
        {
            var dbWPP = _planTmpWeeklyProductionPlaningRepo.GetByID(wpp.KPSYear, wpp.KPSWeek, wpp.BrandCode, wpp.LocationCode);

            // keep original value of CreatedBy & CreatedDate
            wpp.CreatedBy = dbWPP.CreatedBy;
            wpp.CreatedDate = dbWPP.CreatedDate;

            // set UpdatedDate
            wpp.UpdatedDate = DateTime.Now;

            Mapper.Map(wpp, dbWPP);

            _planTmpWeeklyProductionPlaningRepo.Update(dbWPP);
            _uow.SaveChanges();
        }

        public bool IsValidWPP()
        {
            var wpptemp = _planTmpWeeklyProductionPlaningRepo.Get(wpp => wpp.IsValid == false);
            return (!wpptemp.Any());
        }

        public float GetTargetWPP(GetTargetWPPInput input)
        {
            var wpp = _planWeeklyProductionPlaningRepo.GetByID(input.KPSYear, input.KPSWeek, input.BrandCode,
                input.LocationCode);
            //var currentWeek = _masterDataBll.GetWeekByDate(DateTime.Now);
            //var weekDiff = 0;
            //if (wpp != null && currentWeek != null)
            //{
            //    float[] weekValues = { wpp.Value1, wpp.Value2, wpp.Value3, wpp.Value4, wpp.Value5, wpp.Value6, wpp.Value7, wpp.Value8, wpp.Value9, wpp.Value10, wpp.Value11, wpp.Value12, wpp.Value13 };
            //    weekDiff = (wpp.KPSWeek - (currentWeek.Week ?? 0)) - 1;
            //    if (weekDiff >= 0 && weekDiff <= (weekValues.Length - 1))
            //        return weekValues[weekDiff];
            //    else
            //        return 0;
            //}
            return wpp != null ? Convert.ToSingle(wpp.Value1 * Constants.WPPConvert) : 0;
        }

        public int GetStickPerBoxByBrandCode(string BrandCode)
        {
            var brandCode = _mstGenBrandRepo.GetByID(BrandCode);
            if (brandCode != null)
            {
                var brandGroup = brandCode.MstGenBrandGroup;
                if (brandGroup != null)
                {
                    return brandGroup.StickPerBox ?? 0;
                }
            }
            return 0;
        }

        // Get brand by LocationCode, KPSYear & KPSWeek
        public List<string> GetBrandCodeByLocationYearAndWeek(string location, int? KPSYear, int? KPSWeek)
        {
            var queryFilter = PredicateHelper.True<PlanTPOTargetProductionKelompok>();

            if (KPSYear != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSYear == KPSYear);
            }

            if (KPSWeek != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSWeek == KPSWeek);
            }

            if (location != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.LocationCode == location);
            }

            var dbResult = _planTPOTPKRepo.Get(queryFilter);

            if ( dbResult.Count() > 0 )
            {
                return dbResult.Select(m => m.BrandCode).Distinct().ToList();
            }

            return new List<string>();
        }

        public List<string> GetBrandCodeByLocationYearAndWeekTPU(string location, int? KPSYear, int? KPSWeek)
        {
            var queryFilter = PredicateHelper.True<PlanTargetProductionUnit>();

            if (KPSYear != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSYear == KPSYear);
            }

            if (KPSWeek != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSWeek == KPSWeek);
            }

            if (location != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.LocationCode == location);
            }

            var dbResult = _planTPURepo.Get(queryFilter);

            if (dbResult.Count() > 0)
            {
                return dbResult.Select(m => m.BrandCode).Distinct().ToList();
            }

            return new List<string>();
        }

        // Get brand by LocationCode, KPSYear & KPSWeek for Plant TPK Only
        public List<string> GetBrandCodeTPKByLocationYearAndWeek(string location, int? KPSYear, int? KPSWeek)
        {
            var queryFilter = PredicateHelper.True<PlanPlantTargetProductionKelompok>();

            if (KPSYear != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSYear == KPSYear);
            }

            if (KPSWeek != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSWeek == KPSWeek);
            }

            if (location != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.LocationCode == location);
            }

            var dbResult = _planPlantTargetProductionKelompokRepo.Get(queryFilter);

            if (dbResult.Count() > 0)
            {
                return dbResult.Select(m => m.BrandCode).Distinct().ToList();
            }

            return new List<string>();
        }

        public List<string> GetBrandCodeTPUByLocationYearAndWeek(string location, int? KPSYear, int? KPSWeek)
        {
            var queryFilter = PredicateHelper.True<PlanTargetProductionUnit>();

            if (KPSYear != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSYear == KPSYear);
            }

            if (KPSWeek != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.KPSWeek == KPSWeek);
            }

            if (location != null)
            {
                queryFilter = queryFilter.And(wpp => wpp.LocationCode == location);
            }

            var dbResult = _planTPURepo.Get(queryFilter);

            if (dbResult.Count() > 0)
            {
                return dbResult.Select(m => m.BrandCode).Distinct().ToList();
            }

            return new List<string>();
        }


        #endregion

        #region TPK

        public List<PlanPlantAllocationDTO> SavePlanPlantAllocations(List<PlanPlantAllocationDTO> planPlantAllocations)
        {
            foreach (var planPlantAllocation in planPlantAllocations)
            {
                var location = _masterDataBll.GetMstLocationById(planPlantAllocation.LocationCode);
                if (location != null) planPlantAllocation.Shift = location.Shift;
                var db = _planPlantAllocationRepository.GetByID(planPlantAllocation.KPSYear, planPlantAllocation.KPSWeek, planPlantAllocation.GroupCode,
                    planPlantAllocation.ProcessGroup, planPlantAllocation.UnitCode, planPlantAllocation.LocationCode, planPlantAllocation.BrandCode,
                    planPlantAllocation.Shift, planPlantAllocation.TPKPlantStartProductionDate, planPlantAllocation.EmployeeID);
                if (db == null)
                {
                    if (planPlantAllocation.Status) continue;
                    db = Mapper.Map<PlanPlantAllocation>(planPlantAllocation);
                    db.CreatedDate = DateTime.Now;
                    db.UpdatedDate = DateTime.Now;
                    _planPlantAllocationRepository.Insert(db);
                }
                else
                    if (planPlantAllocation.Status) _planPlantAllocationRepository.Delete(db);

            }

            _uow.SaveChanges();
            return planPlantAllocations;
        }

        //public List<PlantTPKCompositeDTO> GetPlantTPKs(GetPlantTPKsInput input)
        //{

        //}
        public ExePlantProductionEntryVerificationDTO GetPlanningPlantTPKByGroup(string group, string locationCode, string unit, string brand, int year, int week, DateTime? date, int shift)
        {
            //var datas = _planPlantTargetProductionKelompokRepo.Get(m => m.GroupCode == group).OrderByDescending(m => m.TPKPlantStartProductionDate).FirstOrDefault();
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            var productionEntryCode = EnumHelper.GetDescription(Core.Enums.CombineCode.EBL) + "/"
                                      + locationCode + "/"
                                      + shift + "/"
                                      + unit + "/"
                                      + group + "/"
                                      + brand + "/"
                                      + year + "/"
                                      + week;

            queryFilter = queryFilter.And(m => m.ProductionEntryCode.Contains(productionEntryCode));

            if (date.HasValue)
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(m.ProductionDate) == DbFunctions.TruncateTime(date));

            var datas = _exePlantProductionEntryVerificationRepo.Get(queryFilter).FirstOrDefault();
            return Mapper.Map<ExePlantProductionEntryVerificationDTO>(datas);
        }
        #endregion

        #region Plant Group Shift

        /// <summary>
        /// Get list of Group Shift
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<PlanPlantGroupShiftDTO> GetGroupShift(GetPlanPlantGroupShiftInput input)
        {
            var queryFilter = PredicateHelper.True<PlanPlantGroupShift>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            if (input.StartDate != null)
                queryFilter = queryFilter.And(m => m.StartDate == input.StartDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<PlanPlantGroupShift>();

            var dbResult = _planGroupShiftRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<PlanPlantGroupShiftDTO>>(dbResult);
        }

        /// <summary>
        /// Get Group Code list
        /// </summary>
        /// <param name="input">Location Code, Unit Code, and Start Date</param>
        /// <returns>Group Code List</returns>
        public List<PlanPlantGroupShiftDTO> GetPlanPlantGroupShifts(GetPlanPlantGroupShiftInput input)
        {
            var result = new List<PlanPlantGroupShiftDTO>();
            var mstPlantProductionGroupCodes = _mstPlantProductionGroupRepo.Get(ppg => ppg.LocationCode == input.LocationCode && ppg.UnitCode == input.UnitCode);
            foreach (var plantProdGroupCode in mstPlantProductionGroupCodes)
            {
                var groupShiftDTO = new PlanPlantGroupShiftDTO();
                groupShiftDTO.GroupCode = plantProdGroupCode.GroupCode;
                groupShiftDTO.ProcessGroup = plantProdGroupCode.ProcessGroup;
                groupShiftDTO.UnitCode = plantProdGroupCode.UnitCode;
                groupShiftDTO.LocationCode = plantProdGroupCode.LocationCode;
                groupShiftDTO.StartDate = (input.StartDate != null) ? input.StartDate : DateTime.Now;
                groupShiftDTO.EndDate = (input.EndDate != null) ? input.EndDate : DateTime.Now;

                var existingGroupShift = _planGroupShiftRepo.Get(gs => gs.LocationCode == input.LocationCode && gs.UnitCode == input.UnitCode && (gs.StartDate <= input.StartDate && gs.EndDate >= input.StartDate) && gs.GroupCode == plantProdGroupCode.GroupCode);
                PlanPlantGroupShift singleGroupShift = existingGroupShift.SingleOrDefault();
                if (singleGroupShift != null)
                {
                    groupShiftDTO.Shift = singleGroupShift.Shift;
                    groupShiftDTO.StartDate = singleGroupShift.StartDate;
                    groupShiftDTO.EndDate = singleGroupShift.EndDate;
                }
                result.Add(groupShiftDTO);
            }
            return result;
        }

        /// <summary>
        /// Save or create group code
        /// </summary>
        /// <param name="plantGroupShift">Group Shift DTO</param>
        /// <param name="userName">Current User Name</param>
        /// <returns></returns>
        public PlanPlantGroupShiftDTO SavePlanPlantGroupShift(PlanPlantGroupShiftDTO plantGroupShift, string userName)
        {
            var dbPlantGroupShift = _planGroupShiftRepo.GetByID(plantGroupShift.StartDate, plantGroupShift.GroupCode, plantGroupShift.ProcessGroup, plantGroupShift.UnitCode, plantGroupShift.LocationCode);

            if (dbPlantGroupShift != null)
            {

                Mapper.Map(plantGroupShift, dbPlantGroupShift);
                dbPlantGroupShift.UpdatedBy = userName;
                dbPlantGroupShift.UpdatedDate = DateTime.Now;
                _planGroupShiftRepo.Update(dbPlantGroupShift);
            }
            else
            {
                var noRange = _planGroupShiftRepo.Get(
                    gs => (
                            (gs.StartDate >= plantGroupShift.StartDate && gs.EndDate <= plantGroupShift.EndDate) ||
                            (gs.StartDate <= plantGroupShift.StartDate && gs.EndDate >= plantGroupShift.EndDate) ||
                            (gs.StartDate <= plantGroupShift.StartDate && gs.EndDate <= plantGroupShift.EndDate && gs.EndDate >= plantGroupShift.StartDate) ||
                            (gs.StartDate >= plantGroupShift.StartDate && gs.EndDate >= plantGroupShift.EndDate && gs.StartDate <= plantGroupShift.EndDate)
                        ) && (gs.LocationCode == plantGroupShift.LocationCode)
                        && (gs.UnitCode == plantGroupShift.UnitCode)
                        && (gs.GroupCode == plantGroupShift.GroupCode)
                        && (gs.ProcessGroup == plantGroupShift.ProcessGroup));
                if (noRange.Any())
                    throw new BLLException(ExceptionCodes.BLLExceptions.DateInRange);
                dbPlantGroupShift = Mapper.Map<PlanPlantGroupShift>(plantGroupShift);
                _planGroupShiftRepo.Insert(dbPlantGroupShift);
                dbPlantGroupShift.CreatedBy = userName;
                dbPlantGroupShift.UpdatedBy = userName;
                dbPlantGroupShift.CreatedDate = DateTime.Now;
                dbPlantGroupShift.UpdatedDate = DateTime.Now;
            }
            _uow.SaveChanges();
            return Mapper.Map<PlanPlantGroupShiftDTO>(dbPlantGroupShift);
        }

        public bool CheckLocationGroupShift(string locationCode, int kpsYear, int kpsWeek)
        {
            var genWeek = _masterDataBll.GetMstGenWeeks(new GetMstGenWeekInput { Year = kpsYear, Week = kpsWeek });
            if (genWeek == null) return false;

            var week = genWeek.FirstOrDefault();
            var groupShift = _planGroupShiftRepo.Get(s => s.LocationCode == locationCode && s.StartDate <= week.StartDate && s.EndDate >= week.EndDate);
            return groupShift.Count() != 0;
        }

        #endregion

        #region Report

        #region Summary Daily Production Group

        public List<PlanningReportProductionTargetCompositeDTO> GetPlanningReportProductionTargets(GetPlanningReportProductionTargetInput input)
        {
            var dbResult = _sqlSPRepo.GetReportSummaryDailyProductionTargets(input.Year, input.Week, input.Decimal, input.Location);
            return Mapper.Map<List<PlanningReportProductionTargetCompositeDTO>>(dbResult);
        }

        #endregion

        #region Summary Daily Production Group

        public List<PlanningReportSummaryProcessTargetsCompositeDTO> GetReportSummaryProcessTargets(GetPlanningReportSummaryProcessTargetsInput input)
        {
            //if (input.FilterType != "Date")
            //{
            //    var genWeek = _masterDataBll.GetWeekByYearAndWeek(input.Year, input.Week);
            //    if (genWeek != null)
            //    {
            //        input.DateFrom = genWeek.StartDate;
            //        input.DateTo = genWeek.StartDate;
            //    }

            //}

            var dbResult = _sqlSPRepo.GetReportSummaryProcessTargets(input.Location, input.Year, input.Week, input.DateFrom, input.DateTo, input.Decimal,input.FilterType);
            var result = Mapper.Map<List<PlanningReportSummaryProcessTargetsCompositeDTO>>(dbResult);
            if (input.SortOrder == "ASC")
            {
                result = (input.SortExpression == "LocationCode") ? result.OrderBy(t => t.LocationCode).ToList() : result;
                result = (input.SortExpression == "UnitCode") ? result.OrderBy(t => t.UnitCode).ToList() : result;
                result = (input.SortExpression == "BrandCode") ? result.OrderBy(t => t.BrandCode).ToList() : result;
                result = (input.SortExpression == "Giling") ? result.OrderBy(t => t.Giling).ToList() : result;
                result = (input.SortExpression == "Gunting") ? result.OrderBy(t => t.Gunting).ToList() : result;
                result = (input.SortExpression == "Pak") ? result.OrderBy(t => t.Pak).ToList() : result;
                result = (input.SortExpression == "WIPGunting") ? result.OrderBy(t => t.WIPGunting).ToList() : result;
                result = (input.SortExpression == "WIPPak") ? result.OrderBy(t => t.WIPPak).ToList() : result;
                result = (input.SortExpression == "Banderol") ? result.OrderBy(t => t.Banderol).ToList() : result;
                result = (input.SortExpression == "Box") ? result.OrderBy(t => t.Box).ToList() : result;

            }
            if (input.SortOrder == "DESC")
            {
                result = (input.SortExpression == "LocationCode") ? result.OrderByDescending(t => t.LocationCode).ToList() : result;
                result = (input.SortExpression == "UnitCode") ? result.OrderByDescending(t => t.UnitCode).ToList() : result;
                result = (input.SortExpression == "BrandCode") ? result.OrderByDescending(t => t.BrandCode).ToList() : result;
                result = (input.SortExpression == "Giling") ? result.OrderByDescending(t => t.Giling).ToList() : result;
                result = (input.SortExpression == "Gunting") ? result.OrderByDescending(t => t.Gunting).ToList() : result;
                result = (input.SortExpression == "Pak") ? result.OrderByDescending(t => t.Pak).ToList() : result;
                result = (input.SortExpression == "WIPGunting") ? result.OrderByDescending(t => t.WIPGunting).ToList() : result;
                result = (input.SortExpression == "WIPPak") ? result.OrderByDescending(t => t.WIPPak).ToList() : result;
                result = (input.SortExpression == "Banderol") ? result.OrderByDescending(t => t.Banderol).ToList() : result;
                result = (input.SortExpression == "Box") ? result.OrderByDescending(t => t.Box).ToList() : result;
            }
            return result;
        }

        #endregion

        #endregion

        #region TPU

        public List<PlanTPUDTO> GetpPlanTargetProductionUnits(GetTargetWPPInput input)
        {
            var queryFilter = PredicateHelper.True<PlanTargetProductionUnit>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            }

            queryFilter = queryFilter.And(p => p.Shift == input.Shift);

            queryFilter = queryFilter.And(p => p.KPSYear == input.KPSYear);

            queryFilter = queryFilter.And(p => p.KPSWeek == input.KPSWeek);

            var dbResult = _planTPURepo.Get(queryFilter);

            return Mapper.Map<List<PlanTPUDTO>>(dbResult);
        }

        public float? CalculateInStock(List<PlanTPUDTO> input, int? stickPerBox)
        {
            var targetManual = input.Sum(p => p.TargetManual1) + input.Sum(p => p.TargetManual2) +
                               input.Sum(p => p.TargetManual3) + input.Sum(p => p.TargetManual4) +
                               input.Sum(p => p.TargetManual5) + input.Sum(p => p.TargetManual6) +
                               input.Sum(p => p.TargetManual7);

            var targetSystem = input.Sum(p => p.TargetSystem1) + input.Sum(p => p.TargetSystem2) +
                               input.Sum(p => p.TargetSystem3) + input.Sum(p => p.TargetSystem4) +
                               input.Sum(p => p.TargetSystem5) + input.Sum(p => p.TargetSystem6) +
                               input.Sum(p => p.TargetSystem7);

            return (targetManual + targetSystem) * 1000000 / stickPerBox;
        }

        public decimal CalculateTargetWPP(PlanWeeklyProductionPlanningInput input, bool Inbox, int stickPerBox)
        {
            var wpp = GetPlanWeeklyProductionPlannings(input).FirstOrDefault();

            decimal WPPValue = 0;

            if (wpp != null)
            {
                WPPValue = Convert.ToDecimal(wpp.Value1 * Constants.WPPConvert);
                WPPValue = Math.Round(WPPValue / stickPerBox);
            }


            return WPPValue;
        }

        #endregion

        #region Report Available Position Number

        public List<PlanPlantGroupShiftDTO> GetGroupShiftProcess(GetPlanPlantGroupShiftInput input)
        {
            var queryFilter = PredicateHelper.True<AvailabelPositionNumberGroup>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            //if (input.shift != null)
            //    queryFilter = queryFilter.And(m => m.Shift == input.shift);
            if (!string.IsNullOrEmpty(input.ProcessGroup))
                queryFilter = queryFilter.And(m => m.ProcessSettingsCode == input.ProcessGroup);
            if (!string.IsNullOrEmpty(input.Status))
                queryFilter = queryFilter.And(m => m.StatusEmp == input.Status);
            //if (input.StartDate != null)
            //    queryFilter = queryFilter.And(m => input.StartDate >= m.StartDate && input.StartDate <= m.EndDate );

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<AvailabelPositionNumberGroup>();

            var dbResult = _AvailabelPositionNumberGroupRepo.Get(queryFilter, orderByFilter);
            var data = Mapper.Map<List<PlanPlantGroupShiftDTO>>(dbResult);
            return data;

        }
        #endregion
    }
}
