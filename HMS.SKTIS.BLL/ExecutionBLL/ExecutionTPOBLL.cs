using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using System.Globalization;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using Excel;
using System.Data;
using FastMember;
using System.Data.SqlClient;

namespace HMS.SKTIS.BLL.ExecutionBLL
{
    public class ExecutionTPOBLL : IExecutionTPOBLL 
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ExeTPOProductionView> _exeTPOProductionViewRepo;
        private IGenericRepository<ExeTPOProduction> _exeTPOProductionRepo;
        private IGenericRepository<MstGenProcess> _mstGenProcess;
        private IGenericRepository<ExeTPOProductionEntryVerification> _exeTPOProductionEntryVerficationRepo;
        private IGenericRepository<ExeTPOProductionEntryVerificationView> _exeTPOProductionEntryVerficationViewRepo;
        private IGenericRepository<ExeActualWorkHour> _exePlantActualWorkHoursRepo;
        private IGenericRepository<ExePlantActualWorkHoursView> _exeExePlantActualWorkHoursViewRepo;
        private IGenericRepository<ExeTPOActualWorkHour> _exeTpoActualWorkHoursRepo; 
        private IGenericRepository<TPOTargetProductionKelompokView> _exePlanTPOTargetProductionKelompokViewRepo;
        private IGenericRepository<MstGenEmpStatu> _mstGenStatusRepo;
        private IGenericRepository<TPOFeeApprovalView> _tpoFeeApprovalViewRepo;
        private IGenericRepository<MstGenBrand> _mstGenBrandRepo;
        private IGenericRepository<ProcessSettingsAndLocationView> _processSettingsAndLocationsView;
        private IGenericRepository<MstGenLocStatu> _mstGenLocStatus;
        private ISqlSPRepository _sqlSPRepo;
        private IUtilitiesBLL _utilitiesBll;
        private IMasterDataBLL _masterDataBLL;


        public ExecutionTPOBLL(IUnitOfWork uow, IUtilitiesBLL utilitiesBll, IMasterDataBLL masterDataBll)
        {
            _uow = uow;
            _exeTPOProductionViewRepo = _uow.GetGenericRepository<ExeTPOProductionView>();
            _exeExePlantActualWorkHoursViewRepo = _uow.GetGenericRepository<ExePlantActualWorkHoursView>();
            _exeTPOProductionRepo = _uow.GetGenericRepository<ExeTPOProduction>();
            _mstGenProcess = _uow.GetGenericRepository<MstGenProcess>();
            _exeTPOProductionEntryVerficationRepo = _uow.GetGenericRepository<ExeTPOProductionEntryVerification>();
            _exeTPOProductionEntryVerficationViewRepo = _uow.GetGenericRepository<ExeTPOProductionEntryVerificationView>();
            _exePlantActualWorkHoursRepo = _uow.GetGenericRepository<ExeActualWorkHour>();
            _exePlanTPOTargetProductionKelompokViewRepo = _uow.GetGenericRepository<TPOTargetProductionKelompokView>();
            _exeTpoActualWorkHoursRepo = _uow.GetGenericRepository<ExeTPOActualWorkHour>();
            _mstGenStatusRepo = _uow.GetGenericRepository<MstGenEmpStatu>();
            _tpoFeeApprovalViewRepo = _uow.GetGenericRepository<TPOFeeApprovalView>();
            _mstGenBrandRepo = _uow.GetGenericRepository<MstGenBrand>();
            _processSettingsAndLocationsView = _uow.GetGenericRepository<ProcessSettingsAndLocationView>();
            _mstGenLocStatus = _uow.GetGenericRepository<MstGenLocStatu>();
            _sqlSPRepo = _uow.GetSPRepository();
            _utilitiesBll = utilitiesBll;
            _masterDataBLL = masterDataBll;
        }

        #region TPO Actual Work Hour
        public ExeTPOActualWorkHoursDTO InsertUpdateExeTpoActualWorkHours(ExeTPOActualWorkHoursDTO post)
        {
            var exist = _exeTpoActualWorkHoursRepo.GetByID(post.LocationCode, post.UnitCode, post.BrandCode,
                post.ProductionDate, post.ProcessGroup, post.StatusEmp);
            if (exist == null)
            {
                var dbExeTpoActualWorkHours = Mapper.Map<ExeTPOActualWorkHour>(post);

                //dbExeTpoActualWorkHours.StatusIdentifier = 
                    //GetStatusIdentifierFromTpoProductionByStatusEmp(post.StatusEmp).ToString();
                dbExeTpoActualWorkHours.StatusIdentifier = _masterDataBLL.GetGenEmpStatusIdentifierByStatusEmp(post.StatusEmp);
                dbExeTpoActualWorkHours.CreatedDate = DateTime.Now;
                dbExeTpoActualWorkHours.UpdatedDate = DateTime.Now;
                dbExeTpoActualWorkHours.CreatedBy = dbExeTpoActualWorkHours.UpdatedBy;
                _exeTpoActualWorkHoursRepo.Insert(dbExeTpoActualWorkHours);

                _uow.SaveChanges();

                return Mapper.Map<ExeTPOActualWorkHoursDTO>(dbExeTpoActualWorkHours);
            }
            else
            {
                post.CreatedBy = exist.CreatedBy;
                post.CreatedDate = exist.CreatedDate;
                post.StatusIdentifier = _masterDataBLL.GetGenEmpStatusIdentifierByStatusEmp(post.StatusEmp);
                Mapper.Map(post, exist);
                exist.UpdatedDate = DateTime.Now;
                _exeTpoActualWorkHoursRepo.Update(exist);

                _uow.SaveChanges();

                return Mapper.Map<ExeTPOActualWorkHoursDTO>(exist);
            }
        }

        public List<ExeTPOActualWorkHoursDTO> GetExeTpoActualWorkHours(GetExePlantActualWorkHoursInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantActualWorkHoursView>();

            queryFilter = queryFilter
                .And(m => m.LocationCode == input.LocationCode)
                .And(m => m.BrandCode == input.Brand);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExePlantActualWorkHoursView>();

            var dbResult = _exeExePlantActualWorkHoursViewRepo.Get(queryFilter, orderByFilter);

            var masterLists = Mapper.Map<List<ExeTPOActualWorkHoursDTO>>(dbResult);

            if (masterLists == null) return null;

            foreach (ExeTPOActualWorkHoursDTO t in masterLists)
            {
                var exist = _exeTpoActualWorkHoursRepo.GetByID(input.LocationCode, input.UnitCode, input.Brand, input.Date, t.ProcessGroup, input.StatusEmp);
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

                t.BrandCode = input.Brand;
                t.UnitCode = input.UnitCode;
            }

            return masterLists;
        }
        #endregion

        #region TPO Production Entry

        public List<ExeTPOProductionViewDTO> GetExeTPOProductionEntry(GetExeTPOProductionInput input)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProductionView>();
            var status = input.Status;

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.Process))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.Process);
            if (!string.IsNullOrEmpty(input.Status))
            {
                // Hardcoded based on requirement
                input.Status = input.Status == "Multiskill"  ? "Resmi": input.Status;
                queryFilter = queryFilter.And(m => m.StatusEmp == input.Status);
            }
            if (!string.IsNullOrEmpty(input.Brand))
                queryFilter = queryFilter.And(m => m.BrandCode == input.Brand);
            if(input.Year > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);
            if (input.Week > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.Week);
            if (input.Date.HasValue)
                queryFilter = queryFilter.And(m => m.ProductionDate == input.Date);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExeTPOProductionView>();

            var dbResult = _exeTPOProductionViewRepo.Get(queryFilter, orderByFilter);

            var result = Mapper.Map<List<ExeTPOProductionViewDTO>>(dbResult);

            // dummy group handler
            if (status == "Multiskill")
            {
                int index = 0;
                foreach (var q in result)
                {
                    result[index].ProductionGroup = q.ProductionGroup.Substring(0, 1) + '5' + q.ProductionGroup.Substring(2, 2);
                    var dbTPOProductionDummy = _exeTPOProductionRepo.GetByID(q.ProductionEntryCode, "Multiskill", result[index].ProductionGroup);

                    result[index].WorkerCount = dbTPOProductionDummy != null ? dbTPOProductionDummy.WorkerCount : null;
                    result[index].Absent = dbTPOProductionDummy != null ? dbTPOProductionDummy.Absent : null;
                    result[index].ActualProduction = dbTPOProductionDummy != null ? dbTPOProductionDummy.ActualProduction : null;
                    result[index].StatusEmp = "Multiskill";

                    index++;
                }
            }
            return result;
        }

        public int GetStatusIdentifierFromTpoProductionByStatusEmp(string statusEmp)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProductionView>();
            if (!string.IsNullOrEmpty(statusEmp))
                queryFilter.And(m => m.StatusEmp == statusEmp);

            var dbResult = _exeTPOProductionViewRepo.Get(queryFilter).FirstOrDefault();

            return dbResult.StatusIdentifier;
        }

        public List<string> GetEmpStatusFromExeTPOProductionEntry(string locationCode, string brandCode, DateTime? date)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProduction>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.ExeTPOProductionEntryVerification.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(brandCode))
                queryFilter = queryFilter.And(m => m.ExeTPOProductionEntryVerification.BrandCode == brandCode);
            if (date.HasValue)
                queryFilter = queryFilter.And(m => m.ExeTPOProductionEntryVerification.ProductionDate == date);

            var EmpStatusData = _exeTPOProductionRepo.Get(queryFilter).OrderBy(s => s.StatusIdentifier).Select(s => s.StatusEmp);
            return EmpStatusData.Distinct().ToList();
        }

        public List<string> GetStatusEmpActiveByLocationAndDate(string locationCode, DateTime? date, string BrandCode, string ProcessGroup)
        {
            //var queryFilter = PredicateHelper.True<ExeTPOProduction>();

            //if (!string.IsNullOrEmpty(locationCode))
            //    queryFilter = queryFilter.And(m => m.ExeTPOProductionEntryVerification.LocationCode == locationCode);
            //if (date.HasValue)
            //    queryFilter = queryFilter.And(m => m.ExeTPOProductionEntryVerification.ProductionDate == date);
            //if (!string.IsNullOrEmpty(BrandCode))
            //    queryFilter = queryFilter.And(m => m.ExeTPOProductionEntryVerification.BrandCode == BrandCode);

            //var EmpStatusData = _exeTPOProductionRepo.Get(queryFilter).OrderBy(o => o.StatusIdentifier).Select(m => m.StatusEmp);

            //return EmpStatusData.Distinct().ToList();
            var queryFilter = PredicateHelper.True<ExeTPOProductionView>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);

            if (!string.IsNullOrEmpty(BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == BrandCode);

            if (date.HasValue)
                queryFilter = queryFilter.And(m => m.ProductionDate == date);

            if (!string.IsNullOrEmpty(ProcessGroup))
                queryFilter = queryFilter.And(m => m.ProcessGroup == ProcessGroup);

            var EmpStatus = _exeTPOProductionViewRepo.Get(queryFilter).OrderBy(s => s.StatusIdentifier).Select(s => s.StatusEmp);

            return EmpStatus.Distinct().ToList();
        }

        

        public List<string> GetStatusEmpActiveByLocationAndDateTPOTPK(string locationCode, DateTime? date)
        {
            var queryFilter = PredicateHelper.True<TPOTargetProductionKelompokView>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);

            if (date.HasValue)
            {
                var mstGenWeek = _masterDataBLL.GetWeekByDate(date.Value);
                if (mstGenWeek != null)
                    queryFilter = queryFilter.And(m => m.TPKTPOStartProductionDate == mstGenWeek.StartDate);
            }

            var mstGenStat = _mstGenStatusRepo.Get();

            var listTPOTPK = _exePlanTPOTargetProductionKelompokViewRepo.Get(queryFilter).Join(mstGenStat, c => c.StatusEmp, m => m.StatusEmp, (c, m) => new { c.StatusEmp, m.StatusIdentifier}).Distinct();

            return listTPOTPK.OrderBy(c => c.StatusIdentifier).Distinct().Select(c => c.StatusEmp).ToList();
        }

        public List<string> GetBrandCodeFromExeTPOProductionEntryVerificationByLocationDate(string locationcode, DateTime? date)
        {
            var queryFilter = PredicateHelper.True<TPOTargetProductionKelompokView>();

            if (!string.IsNullOrEmpty(locationcode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationcode);

            if (date.HasValue)
            {
                var mstGenWeek = _masterDataBLL.GetWeekByDate(date.Value);
                if (mstGenWeek != null)
                    queryFilter = queryFilter.And(m => m.TPKTPOStartProductionDate == mstGenWeek.StartDate);
            }
                

            var BrandCodeData = _exePlanTPOTargetProductionKelompokViewRepo.Get(queryFilter).Select(s => s.BrandCode);
            return BrandCodeData.Distinct().ToList();
        }

        

        public List<PlanTPOTPKCompositeDTO> GetTpoTpkValue(string locationCode, string brand, int year, int week, DateTime? date,
            string process, string status)
        {
            var queryFilter = PredicateHelper.True<TPOTargetProductionKelompokView>();
            queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            queryFilter = queryFilter.And(m => m.BrandCode == brand);
            queryFilter = queryFilter.And(m => m.KPSYear == year);
            queryFilter = queryFilter.And(m => m.KPSWeek == week);
            queryFilter = queryFilter.And(m => m.TPKTPOStartProductionDate == date);
            if (!string.IsNullOrEmpty(process))
            queryFilter = queryFilter.And(m => m.ProcessGroup == process);
            if (!string.IsNullOrEmpty(status))
            queryFilter = queryFilter.And(m => m.StatusEmp == status);

            var dbResult = _exePlanTPOTargetProductionKelompokViewRepo.Get(queryFilter);

            return Mapper.Map<List<PlanTPOTPKCompositeDTO>>(dbResult);
        }

        public List<PlanTPOTPKCompositeDTO> GetTpoTpkValueDistinct(string locationCode, string brand, int year, int week, DateTime? date,string processGroup)
        {
            var queryFilter = PredicateHelper.True<TPOTargetProductionKelompokView>();
            queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            queryFilter = queryFilter.And(m => m.BrandCode == brand);
            queryFilter = queryFilter.And(m => m.KPSYear == year);
            queryFilter = queryFilter.And(m => m.KPSWeek == week);
            queryFilter = queryFilter.And(m => m.ProcessGroup == processGroup);

            var dbResult = _exePlanTPOTargetProductionKelompokViewRepo.Get(queryFilter).GroupBy(a => new
            {
                a.ProcessGroup,
                a.ProcessWorkHours1,
                a.ProcessWorkHours2,
                a.ProcessWorkHours3,
                a.ProcessWorkHours4,
                a.ProcessWorkHours5,
                a.ProcessWorkHours6,
                a.ProcessWorkHours7
            });

            var today = (int)date.Value.DayOfWeek;
            var Res = new List<PlanTPOTPKCompositeDTO>();
            if (today == 1)
            {
                Res = dbResult.Select(a => new PlanTPOTPKCompositeDTO { ProcessGroup = a.Key.ProcessGroup, ProcessWorkHoursTemp = a.Key.ProcessWorkHours1 }).ToList();
            }else if (today == 2)
            {
                Res = dbResult.Select(a => new PlanTPOTPKCompositeDTO { ProcessGroup = a.Key.ProcessGroup, ProcessWorkHoursTemp = a.Key.ProcessWorkHours2 }).ToList();
            }else if (today == 3)
            {
                Res = dbResult.Select(a => new PlanTPOTPKCompositeDTO { ProcessGroup = a.Key.ProcessGroup, ProcessWorkHoursTemp = a.Key.ProcessWorkHours3 }).ToList();
            }else if (today == 4)
            {
                Res = dbResult.Select(a => new PlanTPOTPKCompositeDTO { ProcessGroup = a.Key.ProcessGroup, ProcessWorkHoursTemp = a.Key.ProcessWorkHours4 }).ToList();
            }else if (today == 5)
            {
                Res = dbResult.Select(a => new PlanTPOTPKCompositeDTO { ProcessGroup = a.Key.ProcessGroup, ProcessWorkHoursTemp = a.Key.ProcessWorkHours5 }).ToList();
            }else if (today == 6)
            {
                Res = dbResult.Select(a => new PlanTPOTPKCompositeDTO { ProcessGroup = a.Key.ProcessGroup, ProcessWorkHoursTemp = a.Key.ProcessWorkHours6 }).ToList();
            }else {
                Res = dbResult.Select(a => new PlanTPOTPKCompositeDTO { ProcessGroup = a.Key.ProcessGroup, ProcessWorkHoursTemp = a.Key.ProcessWorkHours7 }).ToList();
            }

            return Mapper.Map<List<PlanTPOTPKCompositeDTO>>(Res);
        }

        /// <summary>
        /// Save TPO Production Entry
        /// </summary>
        /// <param name="ExeTPOProductionDTO">TPO Production Row</param>
        /// <param name="OriginalEmpStatus">Get original Employee Status if using Multiskill</param>
        /// <returns></returns>
        public ExeTPOProductionViewDTO SaveExeTPOProductionEntry(ExeTPOProductionViewDTO ExeTPOProductionDTO, string OriginalEmpStatus, bool verifySystem)
        {            

            var dbTPOProduction = _exeTPOProductionRepo.GetByID(ExeTPOProductionDTO.ProductionEntryCode, ExeTPOProductionDTO.StatusEmp, ExeTPOProductionDTO.ProductionGroup);
            
            if (dbTPOProduction != null)
            {
                // Hardcoded based on requirement
                //if (OriginalEmpStatus == "Multiskill")
                //{
                //    var dbTPOProductionEntryVerfication = _exeTPOProductionEntryVerficationRepo.GetByID(ExeTPOProductionDTO.ProductionEntryCode);                   
               
                //    dbTPOProduction = Mapper.Map<ExeTPOProduction>(ExeTPOProductionDTO);
                //    dbTPOProduction.StatusEmp = OriginalEmpStatus;                    

                //    dbTPOProduction.CreatedDate = DateTime.Now;
                //    dbTPOProduction.UpdatedDate = DateTime.Now;
                //    dbTPOProductionEntryVerfication.ExeTPOProductions.Add(dbTPOProduction);
                //    _exeTPOProductionEntryVerficationRepo.Update(dbTPOProductionEntryVerfication);
                //}
                //else
                //{
                    //ExeTPOProductionDTO.StatusEmp = ExeTPOProductionDTO.StatusEmp == "Multiskill" ? "Resmi" : ExeTPOProductionDTO.StatusEmp;
                    Mapper.Map(ExeTPOProductionDTO, dbTPOProduction);                    
                    dbTPOProduction.UpdatedDate = DateTime.Now;
                    _exeTPOProductionRepo.Update(dbTPOProduction);
                    _uow.SaveChanges();

                    //Check Verify System
                    var queryFilter = PredicateHelper.True<ExeTPOProduction>();
                    queryFilter = queryFilter.And(m => m.ProductionEntryCode == ExeTPOProductionDTO.ProductionEntryCode);
                    var check = _exeTPOProductionRepo.Get(queryFilter);
                    var vSystem = check.Where(c => c.ActualProduction == null).ToList().Any() ? false : true;

                    dbTPOProduction.ExeTPOProductionEntryVerification.VerifySystem = vSystem;
                    _exeTPOProductionRepo.Update(dbTPOProduction);
                    _uow.SaveChanges();
                //}
            }
            else
            {
                if (OriginalEmpStatus == "Multiskill")
                {
                    var dbTPOProductionEntryVerfication = _exeTPOProductionEntryVerficationRepo.GetByID(ExeTPOProductionDTO.ProductionEntryCode);

                    dbTPOProduction = Mapper.Map<ExeTPOProduction>(ExeTPOProductionDTO);
                    dbTPOProduction.StatusEmp = OriginalEmpStatus;

                    dbTPOProduction.CreatedDate = DateTime.Now;
                    dbTPOProduction.UpdatedDate = DateTime.Now;

                    // Check if Attend/Actual != null
                    if (dbTPOProduction.Absent != null || dbTPOProduction.WorkerCount != null)
                    {
                        dbTPOProductionEntryVerfication.ExeTPOProductions.Add(dbTPOProduction);
                        _exeTPOProductionEntryVerficationRepo.Update(dbTPOProductionEntryVerfication);
                        _uow.SaveChanges();
                    }

                    //Check Verify System
                    var queryFilter = PredicateHelper.True<ExeTPOProduction>();
                    queryFilter = queryFilter.And(m => m.ProductionEntryCode == ExeTPOProductionDTO.ProductionEntryCode);
                    var check = _exeTPOProductionRepo.Get(queryFilter);
                    var vSystem = check.Where(c => c.ActualProduction == null).ToList().Any() ? false : true;

                    dbTPOProduction.ExeTPOProductionEntryVerification.VerifySystem = vSystem;
                    _exeTPOProductionRepo.Update(dbTPOProduction);
                    _uow.SaveChanges();
                }
            }
            //_uow.SaveChanges();
            return Mapper.Map<ExeTPOProductionViewDTO>(dbTPOProduction);
        }

        public void InsertGroupTPOProductionEntry(string ProductionCode, string Status,
            MstTPOProductionGroupDTO MstProductionGroup, ExeTPOProductionEntryVerificationViewDTO entry, string Username)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProduction>();

            queryFilter = queryFilter.And(q => q.ProductionEntryCode == ProductionCode);
            queryFilter = queryFilter.And(q => q.StatusEmp == Status);
            queryFilter = queryFilter.And(q => q.ProductionGroup == MstProductionGroup.ProdGroup);

            var dbTPOProductionEntry = _exeTPOProductionRepo.Get(queryFilter).FirstOrDefault();

            if (dbTPOProductionEntry == null)
            {
                var exeTPOProductionEntry = new ExeTPOProduction
                {
                    ProductionEntryCode = ProductionCode,
                    StatusEmp = Status,
                    StatusIdentifier = Convert.ToInt32(_masterDataBLL.GetGenEmpStatusIdentifierByStatusEmp(Status)),
                    ProductionGroup = MstProductionGroup.ProdGroup,
                    WorkerCount = MstProductionGroup.WorkerCount.HasValue ? MstProductionGroup.WorkerCount : null,
                    Absent = null,
                    ActualProduction = null,
                    CreatedDate = DateTime.Now,
                    CreatedBy = Username,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = Username
                };

                _exeTPOProductionRepo.Insert(exeTPOProductionEntry);
                _uow.SaveChanges();
            }
        }

        public void UpdateTPOProductionEntryWorkerCount(string ProductionCode, string Status, MstTPOProductionGroupDTO MstProductionGroup, ExeTPOProductionEntryVerificationViewDTO entry, string Username)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProduction>();

            queryFilter = queryFilter.And(q => q.ProductionEntryCode == ProductionCode);
            queryFilter = queryFilter.And(q => q.StatusEmp == Status);
            queryFilter = queryFilter.And(q => q.ProductionGroup == MstProductionGroup.ProdGroup);

            var dbTPOProductionEntry = _exeTPOProductionRepo.Get(queryFilter).FirstOrDefault();

            // Do nothing when group not exist in Production Entry
            if (dbTPOProductionEntry != null)
            {
                dbTPOProductionEntry.WorkerCount = MstProductionGroup.WorkerCount;
                dbTPOProductionEntry.UpdatedDate = DateTime.Now;
                dbTPOProductionEntry.UpdatedBy = Username;

                _exeTPOProductionRepo.Update(dbTPOProductionEntry);
                _uow.SaveChanges();
            }
        }

        public void DeleteTPOProductionEntry(string ProductionCode, string StatusEmployee, string ProductionGroup)
        {
            var tpo = _exeTPOProductionRepo.Get(
                q =>
                    q.ProductionEntryCode == ProductionCode && q.StatusEmp == StatusEmployee &&
                    q.ProductionGroup == ProductionGroup).FirstOrDefault();

            if (tpo != null)
            {
                _exeTPOProductionRepo.Delete(tpo);
                _uow.SaveChanges();
            }
        }


        #endregion

        #region TPO Production Entry Verification
        public List<ExeTPOProductionEntryVerificationViewDTO> GetExeTPOProductionEntryVerification(GetExeTPOProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProductionEntryVerificationView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);
            if (input.KPSYear > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.KPSYear);
            if (input.KPSWeek > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.KPSWeek);
            if (input.ProductionDate.HasValue)
                queryFilter = queryFilter.And(m => m.ProductionDate == input.ProductionDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExeTPOProductionEntryVerificationView>();
            var tpos = _exeTPOProductionEntryVerficationViewRepo.Get(queryFilter, orderByFilter);
            var masterProcess = _mstGenProcess.Get();
            var joined = from tpo in tpos
                         join process in masterProcess on tpo.ProcessGroup equals process.ProcessGroup
                         orderby process.ProcessOrder
                         select new ExeTPOProductionEntryVerificationView
                         {
                             ProductionEntryCode = tpo.ProductionEntryCode,
                             LocationCode = tpo.LocationCode,
                             BrandCode = tpo.BrandCode,
                             KPSYear = tpo.KPSYear,
                             KPSWeek = tpo.KPSWeek,
                             ProductionDate = tpo.ProductionDate,
                             ProcessGroup = tpo.ProcessGroup,
                             Absent = tpo.Absent,
                             TotalTPKValue = tpo.TotalTPKValue,
                             TotalActualValue = tpo.TotalActualValue,
                             VerifySystem = tpo.VerifySystem,
                             VerifyManual = tpo.VerifyManual,
                             Flag_Manual = tpo.Flag_Manual
                         };

            var listResult = Mapper.Map<List<ExeTPOProductionEntryVerificationViewDTO>>(joined);

            // Checking Validation for Submit Enable

            var listTPOProdEntry = GetExeTPOProductionEntry(new GetExeTPOProductionInput
            {
                LocationCode = input.LocationCode,
                Brand = input.BrandCode,
                Year = input.KPSYear,
                Week = input.KPSWeek,
                Date = input.ProductionDate
            });
            var totalEmp = listTPOProdEntry.Select(s => s.StatusEmp).Distinct().ToList();
            var ActualWorkHours = GetTpoActualWorkHoursByProductionEntryVerivication(input).OrderBy(x=>x.ProcessOrder);
            var ActualWorkHoursDistinct = ActualWorkHours.OrderBy(a => a.ProcessOrder).GroupBy(a => a.ProcessGroup).Select(a => new { PG = a.Key, CPG = a.Select(x => x.ProcessGroup).Count() }).ToList();

            var dataProductionView = listTPOProdEntry.OrderBy(x=>x.ProcessOrder).Where(x => x.StatusEmp != "Multiskill").Select(x => new { nProcessGroup = x.ProcessGroup, nStatusEmp = x.StatusEmp, nProcessOrder = x.ProcessOrder }).Distinct().ToList();
            var dataWorkHour = GetTpoActualWorkHoursByProductionEntryVerivication(input).OrderBy(x => x.ProcessOrder).Where(x => x.StatusEmp != "Multiskill").Select(x => new { mProcessGroup = x.ProcessGroup, mStatusEmp = x.StatusEmp, mProcessOrder = x.ProcessOrder, mStatusIdentifier = x.StatusIdentifier }).Distinct().ToList();
            
            var validRolling = false;
            var validCutting = false;
            var validStickwrapping = false;
            var validWrapping = false;
            var validPacking = false;
            var validStamping = false;
            foreach (var a in dataProductionView)
            {
                
                foreach (var b in dataWorkHour)
                {
                    if (a.nProcessGroup == b.mProcessGroup)
                    {
                        if (a.nStatusEmp == b.mStatusEmp)
                        {
                            if (a.nProcessGroup == "ROLLING" )
                                validRolling = true;
                            if (a.nProcessGroup == "CUTTING" )
                                validCutting = true;
                            if (a.nProcessGroup == "STICKWRAPPING" )
                                validStickwrapping = true;
                            if (a.nProcessGroup == "WRAPPING" )
                                validWrapping = true;
                            if (a.nProcessGroup == "PACKING" )
                                validPacking = true;
                            if (a.nProcessGroup == "STAMPING" )
                                validStamping = true;
                            break;
                        }
                        else if (a.nStatusEmp != b.mStatusEmp)
                        {
                            if (a.nProcessGroup == "ROLLING" )
                                validRolling = false;
                            if (a.nProcessGroup == "CUTTING" )
                                validCutting = false;
                            if (a.nProcessGroup == "STICKWRAPPING" )
                                validStickwrapping = false;
                            if (a.nProcessGroup == "WRAPPING" )
                                validWrapping = false;
                            if (a.nProcessGroup == "PACKING" )
                                validPacking = false;
                            if (a.nProcessGroup == "STAMPING" )
                                validStamping = false;
                        }
                    }
                }
                
            }
             
            var valid = true;
            foreach (var tpoVer in listResult)
            {
                if (validRolling && tpoVer.ProcessGroup == Enums.Process.Rolling.ToString().ToUpper())
                {
                    valid = true;
                }
                else if (validCutting && tpoVer.ProcessGroup == Enums.Process.Cutting.ToString().ToUpper())
                {
                    valid = true;
                }
                else if (validStickwrapping && tpoVer.ProcessGroup == Enums.Process.Stickwrapping.ToString().ToUpper())
                {
                    valid = true;
                }
                else if (valid == true && validWrapping && tpoVer.ProcessGroup == Enums.Process.Wrapping.ToString().ToUpper())
                {
                    valid = true;
                }
                else if (valid == true && validPacking && tpoVer.ProcessGroup == Enums.Process.Packing.ToString().ToUpper())
                {
                    valid = true;
                }
                else if (valid == true && validStamping && tpoVer.ProcessGroup == Enums.Process.Stamping.ToString().ToUpper())
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                }

                // Validation TPO TPK
                var isTPOTPKResultValid = false;
                var tpoTPKValue = GetTpoTpkValueDistinct(tpoVer.LocationCode, tpoVer.BrandCode, tpoVer.KPSYear, tpoVer.KPSWeek, tpoVer.ProductionDate, tpoVer.ProcessGroup);
                var tpkPWHTemp = tpoTPKValue.Select(a => a.ProcessWorkHoursTemp).FirstOrDefault();
                if (tpoVer.TotalActualValue > 0 && tpkPWHTemp > 0)
                    isTPOTPKResultValid = true;
                if (tpkPWHTemp == 0 || tpkPWHTemp == null)
                    isTPOTPKResultValid = true;

                // Check Transaction Log
                var transLogVerification = _utilitiesBll.GetLatestActionTransLogExceptSave(tpoVer.ProductionEntryCode, Enums.PageName.TPOProductionEntryVerification.ToString());
                if(transLogVerification != null)
                {
                    if (transLogVerification.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString())
                    {
                        tpoVer.State = "SUBMIT";
                        tpoVer.AlreadySubmit = true;
                    }
                    else 
                    {
                        tpoVer.State = "CANCELSUBMIT";
                        tpoVer.AlreadySubmit = false;
                    }
                }
                else
                {
                    tpoVer.State = "INITIAL";
                    tpoVer.AlreadySubmit = false;
                }

                // Result Submit Validation
                //tpoVer.IsValidForSubmit = (valid && result && isTPOTPKResultValid && tpoVer.VerifySystem.Value && tpoVer.VerifyManual.Value && validData);
                tpoVer.IsValidForSubmit = (valid && isTPOTPKResultValid && tpoVer.VerifySystem.Value && tpoVer.VerifyManual.Value);
            }

            return listResult;
        }

        public List<ExeTPOProductionViewDTO> getDataProductionView(GetExeTPOProductionInput input)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProductionView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (input.Date.HasValue)
                queryFilter = queryFilter.And(m => m.ProductionDate == input.Date);

            queryFilter = queryFilter.And(m => m.StatusEmp != "Multiskill");

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExeTPOProductionView>();

            var dbResult = _exeTPOProductionViewRepo.Get(queryFilter, orderByFilter);

            var result = Mapper.Map<List<ExeTPOProductionViewDTO>>(dbResult);

            return result;
        }

        //wahyu
        public List<ExeTPOProductionViewDTO> getAbsentActualProdNotNullByProdEntryCodeAndStatusIdentifier(String TransactionCode, String statusEmployee)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProductionView>();

            if (!string.IsNullOrEmpty(TransactionCode))
                queryFilter = queryFilter.And(m => m.ProductionEntryCode == TransactionCode);

            if (!statusEmployee.Equals(""))
                queryFilter = queryFilter.And(m => m.StatusEmp == statusEmployee);

            queryFilter = queryFilter.And(m => m.Absent != null);
            queryFilter = queryFilter.And(m => m.ActualProduction != null);

            var dbResult = _exeTPOProductionViewRepo.Get(queryFilter);

            var result = Mapper.Map<List<ExeTPOProductionViewDTO>>(dbResult);

            return result;
        }

        public List<ExeTPOProductionEntryVerificationViewDTO> GetExeTPOProductionEntryVerificationBetweenProductionDates(string LocationCode, string Process, DateTime StartDate, DateTime? EndDate)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProductionEntryVerificationView>();

            if (!string.IsNullOrEmpty(LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == LocationCode);

                queryFilter = queryFilter.And(m => m.ProductionDate >= StartDate);

            if (EndDate.HasValue)
                queryFilter = queryFilter.And(m => m.ProductionDate <= EndDate.Value);

            if (!string.IsNullOrEmpty(Process))
                queryFilter = queryFilter.And(m => m.ProcessGroup == Process);

            var tpos = _exeTPOProductionEntryVerficationViewRepo.Get(queryFilter);
            var masterProcess = _mstGenProcess.Get();
            var joined = from tpo in tpos
                         join process in masterProcess on tpo.ProcessGroup equals process.ProcessGroup
                         orderby process.ProcessOrder
                         select new ExeTPOProductionEntryVerificationView
                         {
                             ProductionEntryCode = tpo.ProductionEntryCode,
                             LocationCode = tpo.LocationCode,
                             BrandCode = tpo.BrandCode,
                             KPSYear = tpo.KPSYear,
                             KPSWeek = tpo.KPSWeek,
                             ProductionDate = tpo.ProductionDate,
                             ProcessGroup = tpo.ProcessGroup,
                             Absent = tpo.Absent,
                             TotalTPKValue = tpo.TotalTPKValue,
                             TotalActualValue = tpo.TotalActualValue,
                             VerifySystem = tpo.VerifySystem,
                             VerifyManual = tpo.VerifyManual
                         };

            var result = Mapper.Map<List<ExeTPOProductionEntryVerificationViewDTO>>(joined);
            return result;
        }

        public ExeTPOProductionEntryVerificationDTO SaveExeTPOProductionEntryVerification(ExeTPOProductionEntryVerificationDTO ExeTPOProductionVerificationDTO)
        {

            var dbTPOProductionVerification = _exeTPOProductionEntryVerficationRepo.GetByID(ExeTPOProductionVerificationDTO.ProductionEntryCode);

            if (dbTPOProductionVerification != null)
            {

                ExeTPOProductionVerificationDTO.CreatedBy = dbTPOProductionVerification.CreatedBy;
                ExeTPOProductionVerificationDTO.CreatedDate = dbTPOProductionVerification.CreatedDate;
                ExeTPOProductionVerificationDTO.ProcessOrder = dbTPOProductionVerification.ProcessOrder;
                ExeTPOProductionVerificationDTO.WorkHour = dbTPOProductionVerification.WorkHour; 


                Mapper.Map(ExeTPOProductionVerificationDTO, dbTPOProductionVerification);
                dbTPOProductionVerification.UpdatedDate = DateTime.Now;
                _exeTPOProductionEntryVerficationRepo.Update(dbTPOProductionVerification);
                _uow.SaveChanges();
            }            
            return Mapper.Map<ExeTPOProductionEntryVerificationDTO>(dbTPOProductionVerification);
        }

        public List<ExePlantActualWorkHoursDTO> GetActualWorkHoursByProductionEntryVerification(GetExeTPOProductionEntryVerificationInput input)
        {            
           
            var queryFilter = PredicateHelper.True<ExeActualWorkHour>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);            
            if (input.ProductionDate.HasValue)
                queryFilter = queryFilter.And(m => m.ProductionDate == input.ProductionDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExeActualWorkHour>();
            var dbResult = _exePlantActualWorkHoursRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<ExePlantActualWorkHoursDTO>>(dbResult);
        }

        public List<ExeTPOActualWorkHoursDTO> GetTpoActualWorkHoursByProductionEntryVerivication(GetExeTPOProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExeTPOActualWorkHour>();
            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);
            if (input.ProductionDate.HasValue)
                queryFilter = queryFilter.And(m => m.ProductionDate == input.ProductionDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExeTPOActualWorkHour>();
            var dbResult = _exeTpoActualWorkHoursRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<ExeTPOActualWorkHoursDTO>>(dbResult);

        }

        public int TPOProductionEntryVerificationGenerateReport(GetExeTPOProductionEntryVerificationInput input, string userName)
        {
            return _sqlSPRepo.TPOProductionEntryVerificationGenerateReport(input.LocationCode, input.BrandCode, input.KPSYear, input.KPSWeek, input.ProductionDate, userName);
        }

        public void TpoProductionEntryVerificationGenerateReportAsync(GetExeTPOProductionEntryVerificationInput input, string userName)
        {
            _sqlSPRepo.TpoProductionEntryVerificationGenerateReportTask(input.LocationCode, input.BrandCode, input.KPSYear, input.KPSWeek, input.ProductionDate, userName);
        }

        public void GenerateAllAsyncSequence(List<string> input, string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy, string updatedBy, string unitCode, bool generateTpo)
        {
            _sqlSPRepo.GenerateAllAsyncSequenceTask(input, locationCode, brand, year, week, date, createdBy, updatedBy,unitCode, generateTpo);
        }

        public string GetStatusTPOFee(GetExeTPOProductionEntryVerificationInput input)
        { 
            // Get brandgroupcode by brandgroup
            var mstGenBrand = _mstGenBrandRepo.GetByID(input.BrandCode);

            var tpofeeCode = EnumHelper.GetDescription(Enums.CombineCode.FEE) + "/" 
                            + input.LocationCode + "/" 
                            + (mstGenBrand == null ? string.Empty : mstGenBrand.BrandGroupCode) + "/" 
                            + input.KPSYear.ToString() + "/" 
                            + input.KPSWeek.ToString();

            var tpoFeeApprovalView = _tpoFeeApprovalViewRepo.Get(c => c.LocationCode == input.LocationCode && c.TPOFeeCode == tpofeeCode).FirstOrDefault();

            if (tpoFeeApprovalView != null)
                return tpoFeeApprovalView.Status;
            else
                return string.Empty;
        }

        public int TPOProductionEntryVerificationCancelReport(GetExeTPOProductionEntryVerificationInput input)
        {
            return _sqlSPRepo.TPOProductionEntryVerificationCancelReport(input.LocationCode, input.BrandCode, input.KPSYear, input.KPSWeek, input.ProductionDate);
        }

        public List<ExeTPOProductionEntryVerificationDTO> GeTpoProductionEntryVerifications(GetExeTPOProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExeTPOProductionEntryVerification>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);
            if (input.KPSYear > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.KPSYear);
            if (input.KPSWeek > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.KPSWeek);
            if (input.ProductionDate.HasValue)
                queryFilter = queryFilter.And(m => m.ProductionDate == input.ProductionDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ExeTPOProductionEntryVerification>();
            var dbResult = _exeTPOProductionEntryVerficationRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<ExeTPOProductionEntryVerificationDTO>>(dbResult);
        }
        public List<string> GetBrandCodeFromExeTPOProductionEntryVerification(GetExeTPOProductionEntryVerificationInput input)
        {
             var queryFilter = PredicateHelper.True<ExeTPOProductionEntryVerification>();

             if (!string.IsNullOrEmpty(input.LocationCode))
                 queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (input.KPSYear > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.KPSYear);
            if (input.KPSWeek > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.KPSWeek);

             if (input.ProductionDate.HasValue)
                 queryFilter = queryFilter.And(m => m.ProductionDate == input.ProductionDate);

            var BrandCodeData = _exeTPOProductionEntryVerficationRepo.Get(queryFilter).Select(s => s.BrandCode);
            return BrandCodeData.Distinct().ToList();
         }

        public void UpdateVerifyAndFlagTPOEntryVerification(string productionCode, bool verifySystem, bool verifyManual, bool flagManual) 
        {
            var dbResult = _exeTPOProductionEntryVerficationRepo.GetByID(productionCode);
            if (dbResult != null)
            {
                dbResult.VerifySystem = verifySystem;
                dbResult.VerifyManual = verifyManual;
                dbResult.Flag_Manual = flagManual;
                _exeTPOProductionEntryVerficationRepo.Update(dbResult);
                _uow.SaveChanges();
            }
        }

        public void InsertTPOExeReportByGroups(string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy)
        {
            _sqlSPRepo.InsertTPOExeReportByGroups(locationCode, brand, year, week, date, createdBy);
        }

        public void InsertTPOExeReportByGroupsAsync(string locationCode, string brand, int? year, int? week, DateTime? date, string createdBy)
        {
            _sqlSPRepo.InsertTPOExeReportByGroupsTask(locationCode, brand, year, week, date, createdBy);
        }

        public IEnumerable<string> GetListProcessVerification(string locationCode, string brandCode, DateTime productionDate) {
            // not verifiation but [ProcessSettingsAndLocationView]

            var mstgenbrand = _mstGenBrandRepo.GetByID(brandCode);
            var brandGroupCode = mstgenbrand == null ? string.Empty : mstgenbrand.BrandGroupCode;

            var dbTpoEntryVerification = _processSettingsAndLocationsView.Get(c => c.LocationCode == locationCode &&
                                                                                             c.BrandGroupCode == brandGroupCode).OrderByDescending(c => c.ProcessOrder);

            return dbTpoEntryVerification.Select(c => c.ProcessGroup).Distinct().ToList();

        }
        #endregion

        public void UploadExcelTPOEntryDaily(IExcelDataReader excelReader, string username, IEnumerable<string> listUserAccesLocationCode)
        {
            // Get First Sheet
            var firstSheet = excelReader.AsDataSet(true).Tables[0];

            // READ HEADER
            string LocationCode = firstSheet.Rows[2][1].ToString();

            if (String.IsNullOrEmpty(LocationCode)) throw new InvalidOperationException("Location cannot be empty");
            if (!listUserAccesLocationCode.Contains(LocationCode)) throw new InvalidOperationException("Location " + LocationCode + " is not accessible for this user");

            string BrandCode = firstSheet.Rows[3][1].ToString();
            if (String.IsNullOrEmpty(BrandCode)) throw new InvalidOperationException("Brand Code cannot be empty");

            DateTime ProductionDate = Convert.ToDateTime(firstSheet.Rows[4][1]);
            
            var KPSYear = 0;
            var KPSWeek = 0;
            var DayOfWeek = 0;

            var dateNow = DateTime.Now;

            var errMsg = string.Empty;

            // Get Master Week
            var mstGenWeek = _masterDataBLL.GetWeekByDate(ProductionDate);
            if (mstGenWeek != null)
            {
                KPSYear = mstGenWeek.Year ?? 0;
                KPSWeek = mstGenWeek.Week ?? 0;
            }
            DayOfWeek = ProductionDate.DayOfWeek == 0 ? 7 : (int)ProductionDate.DayOfWeek;

            var listMasterProcess = GetListProcessSettingLocationViews(LocationCode, BrandCode, ProductionDate).Where(c => c.ProcessGroup != Enums.ProcessGroup.FOILROLL.ToString()).OrderBy(c => c.ProcessIdentifier);

            var mstGenStatus = _mstGenLocStatus.Get(c => c.LocationCode == LocationCode)
                                                .Select(c => new {  StatusEmp = c.StatusEmp, 
                                                                    StatusIdentifier = c.MstGenEmpStatu.StatusIdentifier
                                                }).OrderBy(c => c.StatusIdentifier);

            // Declare list Data To be Inserted
            var listTPOVerificationToBeInserted = new List<ExeTPOProductionEntryVerification>();
            var listTPOEntryToBeInserted        = new List<ExeTPOProduction>();
            var listTPOActualWorkHoursToBeInserted = new List<ExeTPOActualWorkHour>();
            var listProductionEntry             = new List<string>();

            _sqlSPRepo.CleanTPOWorkHourTemp(LocationCode, BrandCode, "", "", ProductionDate);

            foreach (var masterProcess in listMasterProcess)
            {
                // Get Sheet Excel per Process
                var ExcelSheetWorkbook = excelReader.AsDataSet(true).Tables[masterProcess.ProcessGroup];

                var productionEntryCode = EnumHelper.GetDescription(Enums.CombineCode.EBL) + "/"
                                                       + LocationCode + "/"
                                                       + masterProcess.ProcessIdentifier + "/"
                                                       + BrandCode + "/"
                                                       + KPSYear + "/"
                                                       + KPSWeek + "/"
                                                       + DayOfWeek;

                listProductionEntry.Add(productionEntryCode);

                var newVerification = new ExeTPOProductionEntryVerification();

                newVerification.ProductionEntryCode = productionEntryCode;
                newVerification.LocationCode = LocationCode;
                newVerification.ProcessGroup = masterProcess.ProcessGroup;
                newVerification.ProcessOrder = masterProcess.ProcessOrder;
                newVerification.BrandCode = BrandCode;
                newVerification.KPSYear = KPSYear;
                newVerification.KPSWeek = KPSWeek;
                newVerification.ProductionDate = ProductionDate.Date;
                newVerification.WorkHour = 0;// (float)Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][4]).Hour;
                newVerification.CreatedDate = dateNow;
                newVerification.CreatedBy = username;
                newVerification.UpdatedDate = dateNow;
                newVerification.UpdatedBy = username;

                listTPOVerificationToBeInserted.Add(newVerification);
                
                string TDF = string.Empty;
                string ABS = string.Empty;
                string PROD = string.Empty;

                var timeIn = new DateTime();
                var timeOut = new DateTime();
                var breakTime = new DateTime();

                foreach (var statusEmp in mstGenStatus)
                {
                    //_sqlSPRepo.CleanTPOWorkHourTemp(LocationCode, BrandCode, statusEmp.StatusEmp, masterProcess.ProcessGroup, ProductionDate);

                    var StatusIdentifier = Convert.ToInt32(mstGenStatus.Where(c => c.StatusEmp == statusEmp.StatusEmp).Select(c => c.StatusIdentifier).FirstOrDefault());

                    switch (statusEmp.StatusIdentifier)
                    {
                        case "1":
                            timeIn = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][1]);
                            timeOut = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][2]);
                            breakTime = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][3]);
                            break;
                        case "2":
                            timeIn = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][5]);
                           timeOut = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][6]);
                            breakTime = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][7]);
                            break;
                        case "3":
                            timeIn = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][9]);
                            timeOut = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][10]);
                            breakTime = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][11]);
                            break;
                        case "4":
                            timeIn = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][13]);
                            timeOut = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][14]);
                            breakTime = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][15]);
                            break;
                        case "5":
                            timeIn = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][17]);
                           timeOut = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][18]);
                            breakTime = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][19]);
                            break;
                        case "6":
                            timeIn = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][21]);
                            timeOut = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][22]);
                            breakTime = Convert.ToDateTime(ExcelSheetWorkbook.Rows[8][23]);
                            break;
                    }

                    var newTPOWorkhour = new ExeTPOActualWorkHour();

                    newTPOWorkhour.LocationCode = LocationCode;
                    newTPOWorkhour.UnitCode = Enums.UnitCodeDefault.PROD.ToString();
                    newTPOWorkhour.BrandCode = BrandCode;
                    newTPOWorkhour.ProductionDate = ProductionDate;
                    newTPOWorkhour.ProcessGroup = masterProcess.ProcessGroup;
                    newTPOWorkhour.ProcessOrder = masterProcess.ProcessOrder;
                    newTPOWorkhour.StatusEmp = statusEmp.StatusEmp;
                    newTPOWorkhour.StatusIdentifier = StatusIdentifier.ToString();

                    if (timeOut < timeIn) throw new InvalidOperationException("Jam Keluar < Jam Masuk");

                    newTPOWorkhour.TimeIn = timeIn.TimeOfDay;
                    newTPOWorkhour.TimeOut = timeOut.TimeOfDay;
                    newTPOWorkhour.BreakTime = breakTime.TimeOfDay;
                    newTPOWorkhour.CreatedDate = dateNow;
                    newTPOWorkhour.UpdatedDate = dateNow;
                    newTPOWorkhour.UpdatedBy = username;
                    newTPOWorkhour.CreatedBy = username;

                    listTPOActualWorkHoursToBeInserted.Add(newTPOWorkhour);

                    int iRow = 10;
                    while (ExcelSheetWorkbook.Rows[iRow][0].ToString() != "TOTAL")
                    {
                        var dataRow = ExcelSheetWorkbook.Rows[iRow];
                        switch (statusEmp.StatusIdentifier)
                        {
                            case "1":
                                TDF = dataRow[1].ToString();
                                ABS = dataRow[2].ToString();
                                PROD = dataRow[4].ToString();
                                break;
                            case "2":
                                TDF = dataRow[5].ToString();
                                ABS = dataRow[6].ToString();
                                PROD = dataRow[8].ToString();
                                break;
                            case "3":
                                TDF = dataRow[9].ToString();
                                ABS = dataRow[10].ToString();
                                PROD = dataRow[12].ToString();
                                break;
                            case "4":
                                TDF = dataRow[13].ToString();
                                ABS = dataRow[14].ToString();
                                PROD = dataRow[16].ToString();
                                break;
                            case "5":
                                TDF = dataRow[17].ToString();
                                ABS = dataRow[18].ToString();
                                PROD = dataRow[20].ToString();
                                break;
                            case "6":
                                TDF = dataRow[21].ToString();
                                ABS = dataRow[22].ToString();
                                PROD = dataRow[24].ToString();
                                break;
                        }

                        if (!String.IsNullOrEmpty(TDF))
                        {
                            StringBuilder ProductionGroup = new StringBuilder();
                            ProductionGroup.Append(masterProcess.ProcessIdentifier).Append(StatusIdentifier).Append(Convert.ToInt16(ExcelSheetWorkbook.Rows[iRow][0].ToString()).ToString("D2"));

                            // Check existing TPO Production
                            var entryTPOexisting = _exeTPOProductionRepo.GetByID(productionEntryCode, statusEmp.StatusEmp, ProductionGroup.ToString());

                            var newTPOEntry = new ExeTPOProduction();

                            
                            try
                            {
                                int registered = Convert.ToInt32(TDF);

                                if (registered < Convert.ToInt32(String.IsNullOrEmpty(ABS) ? "0" : ABS))
                                {
                                    errMsg = "Absent cannot greater than worker count";
                                    throw new Exception(errMsg);
                                }

                                if ((float)Convert.ToDouble(String.IsNullOrEmpty(PROD) ? "0" : PROD) < 0)
                                {
                                    errMsg = "Production cannot be negative value";
                                    throw new Exception(errMsg);
                                }

                                newTPOEntry.ProductionEntryCode = productionEntryCode;
                                newTPOEntry.StatusEmp = statusEmp.StatusEmp;
                                newTPOEntry.StatusIdentifier = StatusIdentifier;
                                newTPOEntry.ProductionGroup = ProductionGroup.ToString();
                                newTPOEntry.WorkerCount = registered;

                                //if (registered == 0)
                                //{
                                    //newTPOEntry.Absent = 0;
                                    //newTPOEntry.ActualProduction = 0;
                                //}
                                //else 
                                //{
                                    newTPOEntry.Absent = Convert.ToInt32(String.IsNullOrEmpty(ABS) ? "0" : ABS);
                                    newTPOEntry.ActualProduction = (float)Convert.ToDouble(String.IsNullOrEmpty(PROD) ? "0" : PROD);
                                //}

                                newTPOEntry.CreatedDate = dateNow;
                                newTPOEntry.UpdatedDate = dateNow;
                                newTPOEntry.CreatedBy = username;
                                newTPOEntry.UpdatedBy = username;

                                
                            }
                            catch 
                            {
                                if (String.IsNullOrEmpty(errMsg))
                                    throw new Exception("Number format is incorrect");
                                else
                                    throw new Exception(errMsg);
                            }
                            

                            listTPOEntryToBeInserted.Add(newTPOEntry);
                        }

                        iRow++;
                    }
                }
                
            }

            excelReader.Close();

            StringBuilder listProdEntryParamSP = new StringBuilder();

            foreach (var item in listProductionEntry.Distinct())
            {
                listProdEntryParamSP.Append(item);
                listProdEntryParamSP.Append(";");
            }

            _sqlSPRepo.CleanTPOEntryAndVerTemp(listProdEntryParamSP.ToString());

            // Insert TPO Verification to temp Table
            if (listTPOVerificationToBeInserted.Any())
            {
                using (var ctx = new SKTISEntities())
                {
                    var connectionString = ctx.Database.Connection.ConnectionString;
                    IDataReader reader = ObjectReader.Create(listTPOVerificationToBeInserted.AsEnumerable());

                    var bulkCopy = new SqlBulkCopy(connectionString);
                    bulkCopy.BatchSize = 50000;
                    bulkCopy.ColumnMappings.Clear();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i).Trim();
                        if (name != "ExeTPOProductions")
                            bulkCopy.ColumnMappings.Add(name, name);
                    }

                    bulkCopy.DestinationTableName = "ExeTPOProductionEntryVerificationTemp";
                    bulkCopy.WriteToServer(reader);
                    bulkCopy.Close();
                }
            }

            // Insert TPO Production entry to temp Table 
            if (listTPOEntryToBeInserted.Any())
            {
                using (var ctx = new SKTISEntities())
                {
                    var connectionString = ctx.Database.Connection.ConnectionString;
                    IDataReader reader = ObjectReader.Create(listTPOEntryToBeInserted.AsEnumerable());

                    var bulkCopy = new SqlBulkCopy(connectionString);
                    bulkCopy.BatchSize = 50000;
                    bulkCopy.ColumnMappings.Clear();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i).Trim();
                        if (name != "ExeTPOProductionEntryVerification")
                            bulkCopy.ColumnMappings.Add(name, name);
                    }

                    bulkCopy.DestinationTableName = "ExeTPOProductionTemp";
                    bulkCopy.WriteToServer(reader);
                    bulkCopy.Close();
                }
            }

            if (listTPOActualWorkHoursToBeInserted.Any())
            {
                using (var ctx = new SKTISEntities())
                {
                    var connectionString = ctx.Database.Connection.ConnectionString;
                    IDataReader reader = ObjectReader.Create(listTPOActualWorkHoursToBeInserted.AsEnumerable());

                    var bulkCopy = new SqlBulkCopy(connectionString);
                    bulkCopy.BatchSize = 50000;
                    bulkCopy.ColumnMappings.Clear();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i).Trim();
                        if (name != "MstGenBrand" && name != "MstGenEmpStatu" && name != "MstGenProcess" && name != "MstPlantUnit")
                            bulkCopy.ColumnMappings.Add(name, name);
                    }

                    bulkCopy.DestinationTableName = "ExeTPOActualWorkHoursTemp";
                    bulkCopy.WriteToServer(reader);
                    bulkCopy.Close();
                }
            }

            foreach (var item in listTPOActualWorkHoursToBeInserted)
            {
                _sqlSPRepo.CopyTPOActualWorkHourTemp(LocationCode, BrandCode, item.StatusEmp, item.ProcessGroup, ProductionDate);
            }

            _sqlSPRepo.UploadTPOEntryCopyTemp(listProdEntryParamSP.ToString(), username);
        }

        private IEnumerable<ProcessSettingsAndLocationView> GetListProcessSettingLocationViews(string locationCode, string brandCode, DateTime productionDate)
        {
            // not verifiation but [ProcessSettingsAndLocationView]

            var mstgenbrand = _mstGenBrandRepo.GetByID(brandCode);
            var brandGroupCode = mstgenbrand == null ? string.Empty : mstgenbrand.BrandGroupCode;

            var dbTpoEntryVerification = _processSettingsAndLocationsView.Get(c => c.LocationCode == locationCode &&
                                                                                             c.BrandGroupCode == brandGroupCode).OrderByDescending(c => c.ProcessOrder);

            return dbTpoEntryVerification;

        }

        public void SubmitTPOEntryVerificationSP(string locationCode, string brandCode, int year, int week, DateTime prodDate, string process, string user, int role)
        {
            _sqlSPRepo.SubmitTPOEntryVerificationSP(locationCode, brandCode, year, week, prodDate, process, user, role);
        }
    }
}
