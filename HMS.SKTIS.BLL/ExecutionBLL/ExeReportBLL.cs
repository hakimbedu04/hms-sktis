using System.Data.Entity;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.BusinessObjects.Outputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Enums = HMS.SKTIS.Core.Enums;
using System.Data.Entity.Core.Objects;
using System.Globalization;

namespace HMS.SKTIS.BLL.ExecutionBLL
{
    public partial class ExeReportBLL : IExeReportBLL
    {
        private IUnitOfWork _uow;
        private ISqlSPRepository _sqlSPRepo;
        

        private IMasterDataBLL _masterDataBll;
        private IGenericRepository<ExeReportByGroup> _exeReportByGroupRepo;
        private IGenericRepository<ExeReportByStatusView> _exeReportByStatusRepo;
        private IGenericRepository<ExeReportByStatusWeeklyView> _exeReportByStatusWeeklyRepo;
        private IGenericRepository<ExeReportByStatusMonthlyView> _exeReportByStatusMonthlyRepo;
        private IGenericRepository<MstGenBrandGroup> _mstGenBrandGroupRepo;
        private IGenericRepository<ExeReportDailyProductionAchievementView> _exeReportDailyProductionRepo;
        private IGenericRepository<ExeReportByProcess> _exeReportByProcessRepo;
        private IGenericRepository<ExeReportByProcessView> _exeReportByProcessViewRepo;
        private IGenericRepository<MstGenProcess> _mstGenProcess;
        private IGenericRepository<ExeReportByGroupsWeekly> _exeReportByGroupWeeklyRepo;
        private IGenericRepository<ExeReportByGroupsMonthly> _exeReportByGroupMonthlyRepo;
        private IGenericRepository<MstGenWeek> _mstGenWeekRepo;
        private IGenericRepository<MstGenBrand> _mstGenBrand;
        private IExeReportByStatusBLL _reportByStatusAdditionalBLL;

        public ExeReportBLL(IUnitOfWork uow, IMasterDataBLL masterDataBll, IExeReportByStatusBLL reportByStatusAdditionalBLL)
        {
            _uow = uow;
            _sqlSPRepo = _uow.GetSPRepository();

            _masterDataBll = masterDataBll;
            _exeReportByGroupRepo = _uow.GetGenericRepository<ExeReportByGroup>();
            _exeReportByProcessViewRepo = _uow.GetGenericRepository<ExeReportByProcessView>();
            _exeReportByStatusRepo = _uow.GetGenericRepository<ExeReportByStatusView>();
            _exeReportByStatusWeeklyRepo = _uow.GetGenericRepository<ExeReportByStatusWeeklyView>();
            _exeReportByStatusMonthlyRepo = _uow.GetGenericRepository<ExeReportByStatusMonthlyView>();
            _mstGenBrandGroupRepo = _uow.GetGenericRepository<MstGenBrandGroup>();
            _exeReportByProcessRepo = _uow.GetGenericRepository<ExeReportByProcess>();
            _exeReportDailyProductionRepo = _uow.GetGenericRepository<ExeReportDailyProductionAchievementView>();
            _mstGenProcess = _uow.GetGenericRepository<MstGenProcess>();
            _exeReportByGroupWeeklyRepo = _uow.GetGenericRepository<ExeReportByGroupsWeekly>();
            _exeReportByGroupMonthlyRepo = _uow.GetGenericRepository<ExeReportByGroupsMonthly>();
            _mstGenWeekRepo = _uow.GetGenericRepository<MstGenWeek>();
            _reportByStatusAdditionalBLL = reportByStatusAdditionalBLL;
            _mstGenBrand = _uow.GetGenericRepository<MstGenBrand>();
        }

        #region Production Report by Group

        public List<ExeReportByGroupDTO> GetReportByGroups(GetExeReportByGroupInput input)
        {

            if (input.FilterType == "YearWeek")
                return GetReportByGroupsWeekly(input);

            if (input.FilterType == "YearMonth")
                return GetReportByGroupsMonthly(input);

            return GetReportByGroupsDailyYearly(input);

        }


        private List<ExeReportByGroupDTO> GetReportByGroupsDailyYearly(GetExeReportByGroupInput input)
        {

            var mstGenBrandGroup = _mstGenBrandGroupRepo.Get(c => c.SKTBrandCode == input.Brand);
            var brandGroupCode = mstGenBrandGroup.Select(c => c.BrandGroupCode);

            var queryFilter = PredicateHelper.True<ExeReportByGroup>();
            queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            queryFilter = queryFilter.And(p => p.ProcessGroup == input.ProcessGroup);
            queryFilter = queryFilter.And(p => brandGroupCode.Contains(p.BrandGroupCode));

            if (input.FilterType == "Year")
            {
                queryFilter = queryFilter.And(p => p.KPSYear >= input.YearFrom);
            }
            else
            {
                queryFilter = queryFilter.And(p => p.ProductionDate >= input.DateFrom);
                queryFilter = queryFilter.And(p => p.ProductionDate <= input.DateTo);
            }

            var dbResult = _exeReportByGroupRepo.Get(queryFilter);

            return GetAverageReportGroupByProductionDate(dbResult);
        }

        private List<ExeReportByGroupDTO> GetReportByGroupsWeekly(GetExeReportByGroupInput input)
        {

            var mstGenBrandGroup = _mstGenBrandGroupRepo.Get(c => c.SKTBrandCode == input.Brand);
            var brandGroupCode = mstGenBrandGroup.Select(c => c.BrandGroupCode);

            var queryFilter = PredicateHelper.True<ExeReportByGroupsWeekly>();
            queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            queryFilter = queryFilter.And(p => p.ProcessGroup == input.ProcessGroup);
            queryFilter = queryFilter.And(p => brandGroupCode.Contains(p.BrandGroupCode));

            queryFilter = queryFilter.And(p => p.KPSYear >= input.YearFromWeek);
            queryFilter = queryFilter.And(p => p.KPSWeek >= input.WeekFrom);
            queryFilter = queryFilter.And(p => p.KPSYear <= input.YearToWeek);
            queryFilter = queryFilter.And(p => p.KPSWeek <= input.WeekTo);

            var dbResult = _exeReportByGroupWeeklyRepo.Get(queryFilter);

            return GetAverageReportWeeklyGroupBy(dbResult);
        }

        private List<ExeReportByGroupDTO> GetReportByGroupsMonthly(GetExeReportByGroupInput input)
        {

            var mstGenBrandGroup = _mstGenBrandGroupRepo.Get(c => c.SKTBrandCode == input.Brand);
            var brandGroupCode = mstGenBrandGroup.Select(c => c.BrandGroupCode);

            var queryFilter = PredicateHelper.True<ExeReportByGroupsMonthly>();
            queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            queryFilter = queryFilter.And(p => p.ProcessGroup == input.ProcessGroup);
            queryFilter = queryFilter.And(p => brandGroupCode.Contains(p.BrandGroupCode));

            queryFilter = queryFilter.And(p => p.Year >= input.YearFromMonth);
            queryFilter = queryFilter.And(p => p.Month >= input.MonthFrom);
            queryFilter = queryFilter.And(p => p.Year <= input.YearToMonth);
            queryFilter = queryFilter.And(p => p.Month <= input.MonthTo);

            var dbResult = _exeReportByGroupMonthlyRepo.Get(queryFilter);

            return GetAverageReportMonthlyGroupBy(dbResult);
        }

        private List<ExeReportByGroupDTO> GetAverageReportGroupByProductionDate(IEnumerable<ExeReportByGroup> listData)
        {
            var listGroup = listData.GroupBy(c => new
            {
                c.LocationCode,
                c.UnitCode,
                c.Shift,
                c.StatusEmp,
                c.GroupCode,
                c.BrandGroupCode,
                c.ProductionDate
            }).Select(x => new ExeReportByGroupDTO()
            {
                LocationCode = x.Key.LocationCode,
                UnitCode = x.Key.UnitCode,
                Shift = x.Key.Shift,
                StatusEmp = x.Key.StatusEmp,
                GroupCode = x.Key.GroupCode,
                BrandCode = x.Select(y=>y.BrandCode).FirstOrDefault(),
                BrandGroupCode = x.Key.BrandGroupCode,
                ProductionDate = x.Key.ProductionDate,
                Register = Math.Ceiling(x.Sum(c => c.Register.HasValue ? c.Register.Value : 0) / Convert.ToDouble(x.Count())),
                Absennce_A = Math.Ceiling(x.Sum(c => c.Absennce_A).Value / Convert.ToDouble(x.Count())),
                Absence_I = Math.Ceiling(x.Sum(c => c.Absence_I).Value / Convert.ToDouble(x.Count())),
                Absence_C = Math.Ceiling(x.Sum(c => c.Absence_C).Value / Convert.ToDouble(x.Count())),
                Absence_CH = Math.Ceiling(x.Sum(c => c.Absence_CH).Value / Convert.ToDouble(x.Count())),
                Absence_CT = Math.Ceiling(x.Sum(c => c.Absence_CT).Value / Convert.ToDouble(x.Count())),
                Absence_SLS = Math.Ceiling(x.Sum(c => c.Absence_SLS).Value / Convert.ToDouble(x.Count())),
                Absence_SLP = Math.Ceiling(x.Sum(c => c.Absence_SLP).Value / Convert.ToDouble(x.Count())),
                Absence_ETC = Math.Ceiling(x.Sum(c => c.Absence_ETC).Value / Convert.ToDouble(x.Count())),
                Absence_S = Math.Ceiling(x.Sum(c => c.Absence_S).Value / Convert.ToDouble(x.Count())),
                Multi_TPO = Math.Ceiling(x.Sum(c => c.Multi_TPO).Value / Convert.ToDouble(x.Count())),
                Multi_ROLL = Math.Ceiling(x.Sum(c => c.Multi_ROLL).Value / Convert.ToDouble(x.Count())),
                Multi_CUTT = Math.Ceiling(x.Sum(c => c.Multi_CUTT).Value / Convert.ToDouble(x.Count())),
                Multi_PACK = Math.Ceiling(x.Sum(c => c.Multi_PACK).Value / Convert.ToDouble(x.Count())),
                Multi_STAMP = Math.Ceiling(x.Sum(c => c.Multi_STAMP).Value / Convert.ToDouble(x.Count())),
                Multi_FWRP = Math.Ceiling(x.Sum(c => c.Multi_FWRP).Value / Convert.ToDouble(x.Count())),
                Multi_SWRP = Math.Ceiling(x.Sum(c => c.Multi_SWRP).Value / Convert.ToDouble(x.Count())),
                Multi_WRP = Math.Ceiling(x.Sum(c => c.Multi_WRP).Value / Convert.ToDouble(x.Count())),
                Multi_GEN = Math.Ceiling(x.Sum(c => c.Multi_GEN).Value / Convert.ToDouble(x.Count())),
                In = Math.Ceiling(x.Sum(c => c.EmpIn).Value / Convert.ToDouble(x.Count())),
                Out = Math.Ceiling(x.Sum(c => c.Out).Value / Convert.ToDouble(x.Count())),
                ActualWorker = Math.Ceiling(x.Sum(c => c.ActualWorker).Value / Convert.ToDouble(x.Count())),
                WorkHour = Math.Round(x.Sum(c => c.WorkHour).Value / Convert.ToDouble(x.Count()), 2),
                Production = Math.Round(x.Sum(c => c.Production.HasValue ? c.Production.Value : 0) / Convert.ToDouble(x.Count()), 2),
                ValueHour = Convert.ToDouble(x.Sum(c => c.ValueHour)),
                ValuePeople = Convert.ToDouble(x.Sum(c => c.ValuePeople)),
                ValuePeopleHour = Convert.ToDouble(x.Sum(c => c.ValuePeopleHour)),
                CountGroupData = x.Count()
            });

            var listResult = new List<ExeReportByGroupDTO>();

            //foreach (var exeReportByGroupDto in listGroup.ToList())
            //{
            //    double multiskill = exeReportByGroupDto.Multi_TPO + exeReportByGroupDto.Multi_ROLL +
            //                        exeReportByGroupDto.Multi_CUTT + exeReportByGroupDto.Multi_PACK +
            //                        exeReportByGroupDto.Multi_STAMP + exeReportByGroupDto.Multi_FWRP +
            //                        exeReportByGroupDto.Multi_SWRP + exeReportByGroupDto.Multi_WRP +
            //                        exeReportByGroupDto.Multi_GEN;

            //    var actualWorker = exeReportByGroupDto.In - multiskill - exeReportByGroupDto.Out;
            //    actualWorker = Math.Round((actualWorker / exeReportByGroupDto.CountGroupData), 2);

            //    exeReportByGroupDto.ActualWorker = actualWorker;
            //    if (actualWorker == 0)
            //        exeReportByGroupDto.ValuePeople = 0;
            //    else
            //        exeReportByGroupDto.ValuePeople = (Math.Round(exeReportByGroupDto.Production / actualWorker, 2) /
            //                                           exeReportByGroupDto.CountGroupData);

            //    if (exeReportByGroupDto.WorkHour == 0)
            //        exeReportByGroupDto.ValueHour = 0;
            //    else
            //        exeReportByGroupDto.ValueHour = (Math.Round(exeReportByGroupDto.Production / exeReportByGroupDto.WorkHour, 2) / exeReportByGroupDto.CountGroupData);

            //    if (actualWorker == 0 || exeReportByGroupDto.WorkHour == 0)
            //        exeReportByGroupDto.ValuePeopleHour = 0;
            //    else
            //        exeReportByGroupDto.ValuePeopleHour = (Math.Round(exeReportByGroupDto.Production / actualWorker / exeReportByGroupDto.WorkHour, 2) / exeReportByGroupDto.CountGroupData);

            //    listResult.Add(exeReportByGroupDto);
            //}


            listGroup = listGroup.GroupBy(c => new
            {
                c.LocationCode,
                c.UnitCode,
                c.Shift,
                c.StatusEmp,
                c.GroupCode,
                c.BrandGroupCode
            }).Select(x => new ExeReportByGroupDTO()
            {
                LocationCode = x.Key.LocationCode,
                UnitCode = x.Key.UnitCode,
                Shift = x.Key.Shift,
                StatusEmp = x.Key.StatusEmp,
                GroupCode = x.Key.StatusEmp == Enums.StatusEmp.Resmi.ToString() && x.Key.GroupCode.Substring(1, 1) == "5" ? x.Key.GroupCode + "R" : x.Key.StatusEmp == Enums.StatusEmp.Multiskill.ToString() && x.Key.GroupCode.Substring(1, 1) == "5" ? x.Key.GroupCode + "M" : x.Key.GroupCode,
                BrandCode = x.Select(y => y.BrandCode).FirstOrDefault(),
                BrandGroupCode = x.Key.BrandGroupCode,
                Register = Math.Ceiling(x.Sum(c => c.Register) / Convert.ToDouble(x.Count())),
                Absennce_A = Math.Ceiling(x.Sum(c => c.Absennce_A) / Convert.ToDouble(x.Count())),
                Absence_I = Math.Ceiling(x.Sum(c => c.Absence_I) / Convert.ToDouble(x.Count())),
                Absence_C = Math.Ceiling(x.Sum(c => c.Absence_C) / Convert.ToDouble(x.Count())),
                Absence_CH = Math.Ceiling(x.Sum(c => c.Absence_CH) / Convert.ToDouble(x.Count())),
                Absence_CT = Math.Ceiling(x.Sum(c => c.Absence_CT) / Convert.ToDouble(x.Count())),
                Absence_SLS = Math.Ceiling(x.Sum(c => c.Absence_SLS) / Convert.ToDouble(x.Count())),
                Absence_SLP = Math.Ceiling(x.Sum(c => c.Absence_SLP) / Convert.ToDouble(x.Count())),
                Absence_ETC = Math.Ceiling(x.Sum(c => c.Absence_ETC) / Convert.ToDouble(x.Count())),
                Absence_S = Math.Ceiling(x.Sum(c => c.Absence_S.HasValue ? c.Absence_S.Value : 0) / Convert.ToDouble(x.Count())),
                Multi_TPO = Math.Ceiling(x.Sum(c => c.Multi_TPO) / Convert.ToDouble(x.Count())),
                Multi_ROLL = Math.Ceiling(x.Sum(c => c.Multi_ROLL) / Convert.ToDouble(x.Count())),
                Multi_CUTT = Math.Ceiling(x.Sum(c => c.Multi_CUTT) / Convert.ToDouble(x.Count())),
                Multi_PACK = Math.Ceiling(x.Sum(c => c.Multi_PACK) / Convert.ToDouble(x.Count())),
                Multi_STAMP = Math.Ceiling(x.Sum(c => c.Multi_STAMP) / Convert.ToDouble(x.Count())),
                Multi_FWRP = Math.Ceiling(x.Sum(c => c.Multi_FWRP) / Convert.ToDouble(x.Count())),
                Multi_SWRP = Math.Ceiling(x.Sum(c => c.Multi_SWRP) / Convert.ToDouble(x.Count())),
                Multi_WRP = Math.Ceiling(x.Sum(c => c.Multi_WRP) / Convert.ToDouble(x.Count())),
                Multi_GEN = Math.Ceiling(x.Sum(c => c.Multi_GEN) / Convert.ToDouble(x.Count())),
                In = Math.Ceiling(x.Sum(c => c.In) / Convert.ToDouble(x.Count())),
                Out = Math.Ceiling(x.Sum(c => c.Out) / Convert.ToDouble(x.Count())),
                ActualWorker = Math.Ceiling(x.Sum(c => c.ActualWorker).Value / Convert.ToDouble(x.Count())),
                WorkHour = Math.Round(x.Sum(c => c.WorkHour) / Convert.ToDouble(x.Count()), 2),
                Production = Math.Round(x.Sum(c => c.Production) / Convert.ToDouble(x.Count()), 2),
                ValueHour = Math.Round(x.Sum(c => c.ValueHour) / Convert.ToDouble(x.Count()), 2, MidpointRounding.AwayFromZero),
                ValuePeople = Math.Round(x.Sum(c => c.ValuePeople) / Convert.ToDouble(x.Count()), 2, MidpointRounding.AwayFromZero),
                ValuePeopleHour = Math.Round(x.Sum(c => c.ValuePeopleHour) / Convert.ToDouble(x.Count()), 2, MidpointRounding.AwayFromZero),
                CountGroupData = x.Count()
            }).ToList();


            return listGroup.ToList();
        }

        private List<ExeReportByGroupDTO> GetAverageReportWeeklyGroupBy(IEnumerable<ExeReportByGroupsWeekly> listData)
        {
            var listGroup = listData.GroupBy(c => new
            {
                c.LocationCode,
                c.UnitCode,
                c.Shift,
                c.StatusEmp,
                c.GroupCode,
                c.BrandGroupCode,
                c.KPSWeek,
                c.KPSYear
            }).Select(x => new ExeReportByGroupDTO()
            {
                LocationCode = x.Key.LocationCode,
                UnitCode = x.Key.UnitCode,
                Shift = x.Key.Shift,
                StatusEmp = x.Key.StatusEmp,
                GroupCode = x.Key.GroupCode,
                BrandCode = x.Select(y => y.BrandCode).FirstOrDefault(),
                BrandGroupCode = x.Key.BrandGroupCode,
                Register = Math.Ceiling(x.Sum(c => c.Register) / Convert.ToDouble(x.Count())),
                Absennce_A = Math.Ceiling(x.Sum(c => c.Absennce_A).Value / Convert.ToDouble(x.Count())),
                Absence_I = Math.Ceiling(x.Sum(c => c.Absence_I).Value / Convert.ToDouble(x.Count())),
                Absence_C = Math.Ceiling(x.Sum(c => c.Absence_C).Value / Convert.ToDouble(x.Count())),
                Absence_CH = Math.Ceiling(x.Sum(c => c.Absence_CH).Value / Convert.ToDouble(x.Count())),
                Absence_CT = Math.Ceiling(x.Sum(c => c.Absence_CT).Value / Convert.ToDouble(x.Count())),
                Absence_SLS = Math.Ceiling(x.Sum(c => c.Absence_SLS).Value / Convert.ToDouble(x.Count())),
                Absence_SLP = Math.Ceiling(x.Sum(c => c.Absence_SLP).Value / Convert.ToDouble(x.Count())),
                Absence_ETC = Math.Ceiling(x.Sum(c => c.Absence_ETC).Value / Convert.ToDouble(x.Count())),
                Absence_S = Math.Ceiling(x.Sum(c => c.Absence_S).Value / Convert.ToDouble(x.Count())),
                Multi_TPO = Math.Ceiling(x.Sum(c => c.Multi_TPO).Value / Convert.ToDouble(x.Count())),
                Multi_ROLL = Math.Ceiling(x.Sum(c => c.Multi_ROLL).Value / Convert.ToDouble(x.Count())),
                Multi_CUTT = Math.Ceiling(x.Sum(c => c.Multi_CUTT).Value / Convert.ToDouble(x.Count())),
                Multi_PACK = Math.Ceiling(x.Sum(c => c.Multi_PACK).Value / Convert.ToDouble(x.Count())),
                Multi_STAMP = Math.Ceiling(x.Sum(c => c.Multi_STAMP).Value / Convert.ToDouble(x.Count())),
                Multi_FWRP = Math.Ceiling(x.Sum(c => c.Multi_FWRP).Value / Convert.ToDouble(x.Count())),
                Multi_SWRP = Math.Ceiling(x.Sum(c => c.Multi_SWRP).Value / Convert.ToDouble(x.Count())),
                Multi_WRP = Math.Ceiling(x.Sum(c => c.Multi_WRP).Value / Convert.ToDouble(x.Count())),
                Multi_GEN = Math.Ceiling(x.Sum(c => c.Multi_GEN).Value / Convert.ToDouble(x.Count())),
                In = Math.Ceiling(x.Sum(c => c.EmpIn).Value / Convert.ToDouble(x.Count())),
                Out = Math.Ceiling(x.Sum(c => c.Out).Value / Convert.ToDouble(x.Count())),
                ActualWorker = Math.Ceiling(x.Sum(c => c.ActualWorker).Value / Convert.ToDouble(x.Count())),
                WorkHour = Math.Round(x.Sum(c => c.WorkHour).Value / Convert.ToDouble(x.Count()), 2),
                Production = Math.Round(x.Sum(c => c.Production.HasValue ? c.Production.Value : 0) / Convert.ToDouble(x.Count()), 2),
                ValueHour = Math.Round(x.Sum(c => c.ValueHour.HasValue ? c.ValueHour.Value : 0) / Convert.ToDouble(x.Count()), 2),
                ValuePeople = Math.Round(x.Sum(c => c.ValuePeople.HasValue ? c.ValuePeople.Value : 0) / Convert.ToDouble(x.Count()), 2),
                ValuePeopleHour = Math.Round(x.Sum(c => c.ValuePeopleHour.HasValue ? c.ValuePeopleHour.Value : 0) / Convert.ToDouble(x.Count()), 2),
                CountGroupData = x.Count()
            });


            return listGroup.ToList();
        }

        private List<ExeReportByGroupDTO> GetAverageReportMonthlyGroupBy(IEnumerable<ExeReportByGroupsMonthly> listData)
        {
            var listGroup = listData.GroupBy(c => new
            {
                c.LocationCode,
                c.UnitCode,
                c.Shift,
                c.StatusEmp,
                c.GroupCode,
                c.BrandGroupCode,
                c.Year,
                c.Month
            }).Select(x => new ExeReportByGroupDTO()
            {
                LocationCode = x.Key.LocationCode,
                UnitCode = x.Key.UnitCode,
                Shift = x.Key.Shift,
                StatusEmp = x.Key.StatusEmp,
                GroupCode = x.Key.GroupCode,
                BrandCode = x.Select(y => y.BrandCode).FirstOrDefault(),
                BrandGroupCode = x.Key.BrandGroupCode,
                Register = Math.Ceiling(x.Sum(c => c.Register) / Convert.ToDouble(x.Count())),
                Absennce_A = Math.Ceiling(x.Sum(c => c.Absennce_A).Value / Convert.ToDouble(x.Count())),
                Absence_I = Math.Ceiling(x.Sum(c => c.Absence_I).Value / Convert.ToDouble(x.Count())),
                Absence_C = Math.Ceiling(x.Sum(c => c.Absence_C).Value / Convert.ToDouble(x.Count())),
                Absence_CH = Math.Ceiling(x.Sum(c => c.Absence_CH).Value / Convert.ToDouble(x.Count())),
                Absence_CT = Math.Ceiling(x.Sum(c => c.Absence_CT).Value / Convert.ToDouble(x.Count())),
                Absence_SLS = Math.Ceiling(x.Sum(c => c.Absence_SLS).Value / Convert.ToDouble(x.Count())),
                Absence_SLP = Math.Ceiling(x.Sum(c => c.Absence_SLP).Value / Convert.ToDouble(x.Count())),
                Absence_ETC = Math.Ceiling(x.Sum(c => c.Absence_ETC).Value / Convert.ToDouble(x.Count())),
                Absence_S = Math.Ceiling(x.Sum(c => c.Absence_S).Value / Convert.ToDouble(x.Count())),
                Multi_TPO = Math.Ceiling(x.Sum(c => c.Multi_TPO).Value / Convert.ToDouble(x.Count())),
                Multi_ROLL = Math.Ceiling(x.Sum(c => c.Multi_ROLL).Value / Convert.ToDouble(x.Count())),
                Multi_CUTT = Math.Ceiling(x.Sum(c => c.Multi_CUTT).Value / Convert.ToDouble(x.Count())),
                Multi_PACK = Math.Ceiling(x.Sum(c => c.Multi_PACK).Value / Convert.ToDouble(x.Count())),
                Multi_STAMP = Math.Ceiling(x.Sum(c => c.Multi_STAMP).Value / Convert.ToDouble(x.Count())),
                Multi_FWRP = Math.Ceiling(x.Sum(c => c.Multi_FWRP).Value / Convert.ToDouble(x.Count())),
                Multi_SWRP = Math.Ceiling(x.Sum(c => c.Multi_SWRP).Value / Convert.ToDouble(x.Count())),
                Multi_WRP = Math.Ceiling(x.Sum(c => c.Multi_WRP).Value / Convert.ToDouble(x.Count())),
                Multi_GEN = Math.Ceiling(x.Sum(c => c.Multi_GEN).Value / Convert.ToDouble(x.Count())),
                In = Math.Ceiling(x.Sum(c => c.EmpIn).Value / Convert.ToDouble(x.Count())),
                Out = Math.Ceiling(x.Sum(c => c.Out).Value / Convert.ToDouble(x.Count())),
                ActualWorker = Math.Ceiling(x.Sum(c => c.ActualWorker).Value / Convert.ToDouble(x.Count())),
                WorkHour = Math.Round(x.Sum(c => c.WorkHour).Value / Convert.ToDouble(x.Count()), 2),
                Production = Math.Round(x.Sum(c => c.Production.HasValue ? c.Production.Value : 0) / Convert.ToDouble(x.Count()), 2),
                ValueHour = Math.Round(x.Sum(c => c.ValueHour.HasValue ? c.ValueHour.Value : 0) / Convert.ToDouble(x.Count()), 2),
                ValuePeople = Math.Round(x.Sum(c => c.ValuePeople.HasValue ? c.ValuePeople.Value : 0) / Convert.ToDouble(x.Count()), 2),
                ValuePeopleHour = Math.Round(x.Sum(c => c.ValuePeopleHour.HasValue ? c.ValuePeopleHour.Value : 0) / Convert.ToDouble(x.Count()), 2),
                CountGroupData = x.Count()
            });


            return listGroup.ToList();
        }


        public void DeleteReportByGroups(string locationCode, string groupCode, string brandCode, DateTime productionDate)
        {
            // Get report by group by Primary Key (LocationCode, GroupCode, BrandCode, ProductionDate)
            var reportByGroup = _exeReportByGroupRepo.Get(c => c.LocationCode == locationCode
                                                                    && c.GroupCode == groupCode
                                                                    && c.BrandCode == brandCode
                                                                    && c.ProductionDate == productionDate);
                
            foreach (var item in reportByGroup)
	        {
		        _exeReportByGroupRepo.Delete(item);
	        }

            _uow.SaveChanges();
        }

        public List<string> GetProcessListFilter(GetExeReportByGroupInput input)
        {
            // Create query filter to ExeReportByGroup
            var queryFilter = PredicateHelper.True<ExeReportByGroup>();

            if (!String.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(c => c.LocationCode == input.LocationCode);

            if (!String.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(c => c.UnitCode == input.UnitCode);

            if (input.FilterType == "Year")
            {
                if (input.YearFrom.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear == input.YearFrom.Value);
            }
            else if (input.FilterType == "YearMonth")
            {
                if (input.YearFromMonth.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear >= input.YearFromMonth.Value);
                if (input.YearToMonth.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear <= input.YearToMonth.Value);
                if (input.MonthFrom.HasValue)
                    queryFilter = queryFilter.And(c => c.ProductionDate.Month >= input.MonthFrom.Value);
                if (input.MonthTo.HasValue)
                    queryFilter = queryFilter.And(c => c.ProductionDate.Month <= input.MonthTo.Value);
            }
            else if (input.FilterType == "YearWeek")
            {
                if (input.YearFromWeek.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear >= input.YearFromWeek.Value);
                if (input.YearToWeek.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear <= input.YearToWeek.Value);
                if (input.WeekFrom.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSWeek >= input.WeekFrom.Value);
                if (input.WeekTo.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSWeek <= input.WeekTo.Value);
            }
            else if (input.FilterType == "Date")
            {
                if (input.DateFrom.HasValue)
                    queryFilter = queryFilter.And(c => DbFunctions.TruncateTime(c.ProductionDate) >= DbFunctions.TruncateTime(input.DateFrom.Value));
                if (input.DateTo.HasValue)
                    queryFilter = queryFilter.And(c => DbFunctions.TruncateTime(c.ProductionDate) <= DbFunctions.TruncateTime(input.DateTo.Value));
            }

            // Get DB result ExeReportByGroup
            var dbResultReportByGroup = _exeReportByGroupRepo.Get(queryFilter);

            // Init list process
            var processList = new List<string>();

            // Get process list
            if (dbResultReportByGroup.Any())
                processList = dbResultReportByGroup.Select(c => c.ProcessGroup).Distinct().ToList();

            // Create query filter to master process
            var queryFilterProc = PredicateHelper.True<MstGenProcess>();

            // Get DB result ExeReportByGroup by processList to get ProcessOrder and order by 
            var dbResultMstProc = _mstGenProcess.Get(c => processList.Contains(c.ProcessGroup)).OrderBy(c => c.ProcessOrder);

            // Assign again list process after order by ProcessOrder
            if (dbResultMstProc.Any())
                processList = dbResultMstProc.Select(c => c.ProcessGroup).Distinct().ToList();

            return processList;
        }

        public List<string> GetBrandGroupCodeFilter(GetExeReportByGroupInput input)
        {
            // Create query filter to ExeReportByGroup
            var queryFilter = PredicateHelper.True<ExeReportByGroup>();

            if (!String.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(c => c.LocationCode == input.LocationCode);

            if (!String.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(c => c.UnitCode == input.UnitCode);

            //if (!String.IsNullOrEmpty(input.ProcessGroup))
            queryFilter = queryFilter.And(c => c.ProcessGroup == input.ProcessGroup);

            if (input.FilterType == "Year")
            {
                if (input.YearFrom.HasValue)
                    queryFilter = queryFilter.And(c => c.ProductionDate.Year == input.YearFrom.Value);
            }
            else if (input.FilterType == "YearMonth")
            {
                if (input.YearFromMonth.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear >= input.YearFromMonth.Value);
                if (input.YearToMonth.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear <= input.YearToMonth.Value);
                if (input.MonthFrom.HasValue)
                    queryFilter = queryFilter.And(c => c.ProductionDate.Month >= input.MonthFrom.Value);
                if (input.MonthTo.HasValue)
                    queryFilter = queryFilter.And(c => c.ProductionDate.Month <= input.MonthTo.Value);
            }
            else if (input.FilterType == "YearWeek")
            {
                if (input.YearFromWeek.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear >= input.YearFromWeek.Value);
                if (input.YearToWeek.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSYear <= input.YearToWeek.Value);
                if (input.WeekFrom.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSWeek >= input.WeekFrom.Value);
                if (input.WeekTo.HasValue)
                    queryFilter = queryFilter.And(c => c.KPSWeek <= input.WeekTo.Value);
            }
            else if (input.FilterType == "Date")
            {
                if (input.DateFrom.HasValue)
                    queryFilter = queryFilter.And(c => DbFunctions.TruncateTime(c.ProductionDate) >= DbFunctions.TruncateTime(input.DateFrom.Value));
                if (input.DateTo.HasValue)
                    queryFilter = queryFilter.And(c => DbFunctions.TruncateTime(c.ProductionDate) <= DbFunctions.TruncateTime(input.DateTo.Value));
            }

            // Get DB result ExeReportByGroup
            var dbResultReportByGroup = _exeReportByGroupRepo.Get(queryFilter).OrderBy(c => c.BrandGroupCode);

            // Init list brandGroupCode
            var brandGroupCodeList = new List<string>();

            // Get brandgroupcode list
            if (dbResultReportByGroup.Any())
                brandGroupCodeList = dbResultReportByGroup.Select(c => c.BrandGroupCode).Distinct().ToList();

            // Init list SKT Brand Code
            var listSktBrandCode = new List<string>();
            foreach (var data in brandGroupCodeList)
            {
                // Get DB result MstGenBrandGroup
                var dbMstGenBrandGroup = _mstGenBrandGroupRepo.Get(c => data.Contains(c.BrandGroupCode)).OrderBy(c => c.SKTBrandCode);

                // Get SKT Brand Code List
                var dbResult = dbMstGenBrandGroup.Select(c => c.SKTBrandCode).Distinct().ToList();
                listSktBrandCode.AddRange(dbResult);
            }
            return listSktBrandCode;
        }

        #endregion

        #region Production Report by Status
       
        public List<ExeReportByStatusDTO> GetReportByStatus(GetExeReportByStatusInput input)
        {
            if (input.FilterType == "YearMonth")
                return GetReportByStatusMonthly(input);

            //if (input.FilterType == "YearWeek")
            //    return GetReportByStatusWeekly(input);

            return GetReportByStatusDailyAnnualy(input);
        }

        private List<ExeReportByStatusDTO> GetReportByStatusDailyAnnualy(GetExeReportByStatusInput input)
        {
            var queryFilter = PredicateHelper.True<ExeReportByStatusView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            if (input.Shift.HasValue)
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                if (input.BrandCode == "All")
                    queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);
                else
                    queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            }
            if (input.FilterType == "Date")
            {
                if (input.DateFrom != null && input.DateTo != null)
                    queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(input.DateFrom) && DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(input.DateTo));
            }
            else if (input.FilterType == "YearWeek")
            {
                var dateFrom = _masterDataBll.GetWeekByYearAndWeek(input.YearFrom.Value, input.WeekFrom.Value);
                var dateTo = _masterDataBll.GetWeekByYearAndWeek(input.YearTo.Value, input.WeekTo.Value);
                if (input.YearFrom.HasValue && input.MonthFrom.HasValue && input.YearTo.HasValue && input.MonthTo.HasValue)
                    queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(dateFrom.StartDate) && DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(dateTo.EndDate));
            }else{
                if (input.YearFrom.HasValue)
                {
                    input.DateFrom = DateTime.Parse(string.Format("{0000}", input.YearFrom.Value) + "-01-01");
                    input.DateTo = DateTime.Parse(string.Format("{0000}", input.YearFrom.Value) + "-12-31");
                    queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(input.DateFrom) && DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(input.DateTo));
                }
            }
            var dbResult = _exeReportByStatusRepo.Get(queryFilter).OrderBy(c => c.StatusIdentifier);

            return Mapper.Map<List<ExeReportByStatusDTO>>(dbResult);
        }

        private List<ExeReportByStatusDTO> GetReportByStatusWeekly(GetExeReportByStatusInput input)
        {
            var queryFilter = PredicateHelper.True<ExeReportByStatusWeeklyView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            if (input.Shift.HasValue)
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                if (input.BrandCode == "All")
                    queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);
                else
                    queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            }
            //queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            if (input.YearFrom.HasValue && input.MonthFrom.HasValue && input.YearTo.HasValue && input.MonthTo.HasValue)
                queryFilter = queryFilter.And(p => (p.KPSYear >= input.YearFrom && p.KPSWeek >= input.WeekFrom) && (p.KPSYear <= input.YearTo && p.KPSWeek <= input.WeekTo));
            var dbResult = _exeReportByStatusWeeklyRepo.Get(queryFilter).OrderBy(c => c.StatusIdentifier);

            var result = Mapper.Map<List<ExeReportByStatusDTO>>(dbResult);

            return result;

        }

        private List<ExeReportByStatusDTO> GetReportByStatusMonthly(GetExeReportByStatusInput input)
        {
            var queryFilter = PredicateHelper.True<ExeReportByStatusMonthlyView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            if (input.Shift.HasValue)
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            //if (!string.IsNullOrEmpty(input.BrandCode))
            //    queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                if (input.BrandCode == "All")
                    queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);
                else
                    queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            }
            if (input.YearFrom.HasValue && input.MonthFrom.HasValue && input.YearTo.HasValue && input.MonthTo.HasValue)
                queryFilter = queryFilter.And(p => (p.Year >= input.YearFrom && p.Month >= input.MonthFrom) && (p.Year <= input.YearTo && p.Month <= input.MonthTo));
            var dbResult = _exeReportByStatusMonthlyRepo.Get(queryFilter).OrderBy(c => c.StatusIdentifier);

            return Mapper.Map<List<ExeReportByStatusDTO>>(dbResult);
        }

        public List<string> GetActiveBrandCode(string location, string brandGroupCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType)
        {
            var DateFrom = Convert.ToDateTime(FromDate);
            var DateTo = Convert.ToDateTime(ToDate);
            var FromYear = (!string.IsNullOrEmpty(YearFrom)) ? int.Parse(YearFrom) : 0;
            var ToYear = (!string.IsNullOrEmpty(YearTo)) ? int.Parse(YearTo) : 0;
            var FromMonth = (!string.IsNullOrEmpty(MonthFrom)) ? int.Parse(MonthFrom) : 0;
            var ToMonth = (!string.IsNullOrEmpty(MonthTo)) ? int.Parse(MonthTo) : 0;
            var FromWeek = (!string.IsNullOrEmpty(WeekFrom)) ? int.Parse(WeekFrom) : 0;
            var ToWeek = (!string.IsNullOrEmpty(WeekTo)) ? int.Parse(WeekTo) : 0;

            if (FilterType == "YearMonth")
            {
                var queryFilter = PredicateHelper.True<ExeReportByStatusMonthlyView>();

                if (!string.IsNullOrEmpty(location))
                    queryFilter = queryFilter.And(p => p.LocationCode == location);
                if (brandGroupCode != null)
                    queryFilter = queryFilter.And(m => m.BrandGroupCode == brandGroupCode);
                if (!string.IsNullOrEmpty(YearFrom) && !string.IsNullOrEmpty(MonthFrom) && !string.IsNullOrEmpty(YearTo) && !string.IsNullOrEmpty(MonthTo))
                    queryFilter = queryFilter.And(p => (p.Year >= FromYear && p.Month >= FromMonth) && (p.Year <= FromYear && p.Month <= FromMonth));

                var dbResult = _exeReportByStatusMonthlyRepo.Get(queryFilter);

                if (dbResult.Count() > 0)
                {
                    return dbResult.Select(m => m.BrandCode).Distinct().ToList();
                }
            }
            else if (FilterType == "YearWeek")
            {
                var queryFilter = PredicateHelper.True<ExeReportByStatusWeeklyView>();

                if (!string.IsNullOrEmpty(location))
                    queryFilter = queryFilter.And(p => p.LocationCode == location);
                if (brandGroupCode != null)
                    queryFilter = queryFilter.And(m => m.BrandGroupCode == brandGroupCode);
                if (!string.IsNullOrEmpty(YearFrom) && !string.IsNullOrEmpty(WeekFrom) && !string.IsNullOrEmpty(YearTo) && !string.IsNullOrEmpty(WeekTo))
                    queryFilter = queryFilter.And(p => (p.KPSYear >= FromYear && p.KPSWeek >= FromWeek) && (p.KPSYear <= ToYear && p.KPSWeek <= ToWeek));
                var dbResult = _exeReportByStatusWeeklyRepo.Get(queryFilter);

                if (dbResult.Count() > 0)
                {
                    return dbResult.Select(m => m.BrandCode).Distinct().ToList();
                }
            }
            else
            {
                var queryFilter = PredicateHelper.True<ExeReportByStatusView>();

                if (location != null)
                    queryFilter = queryFilter.And(m => m.LocationCode == location);
                if (brandGroupCode != null)
                    queryFilter = queryFilter.And(m => m.BrandGroupCode == brandGroupCode);
                if (FilterType == "Date")
                {
                    if (DateFrom != null && DateTo != null)
                        queryFilter =
                            queryFilter.And(
                                p =>
                                    DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(DateFrom) &&
                                    DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(DateTo));
                }
                else
                {
                    if (!string.IsNullOrEmpty(YearFrom))
                    {
                        DateFrom = DateTime.Parse(string.Format("{0000}", YearFrom) + "-01-01");
                        DateTo = DateTime.Parse(string.Format("{0000}", YearFrom) + "-12-31");
                        queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(DateTo));
                    }
                }

                var dbResult = _exeReportByStatusRepo.Get(queryFilter);

                if (dbResult.Count() > 0)
                {
                    return dbResult.Select(m => m.BrandCode).Distinct().ToList();
                }
            }

            return new List<string>();


        }

        public List<string> GetActiveBrandGroupCode(string locationCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType)
        {
            var DateFrom = Convert.ToDateTime(FromDate);
            var DateTo = Convert.ToDateTime(ToDate);
            var FromYear = (!string.IsNullOrEmpty(YearFrom)) ? int.Parse(YearFrom) : 0;
            var ToYear = (!string.IsNullOrEmpty(YearTo)) ? int.Parse(YearTo) : 0;
            var FromMonth = (!string.IsNullOrEmpty(MonthFrom)) ? int.Parse(MonthFrom) : 0;
            var ToMonth = (!string.IsNullOrEmpty(MonthTo)) ? int.Parse(MonthTo) : 0;
            var FromWeek = (!string.IsNullOrEmpty(WeekFrom)) ? int.Parse(WeekFrom) : 0;
            var ToWeek = (!string.IsNullOrEmpty(WeekTo)) ? int.Parse(WeekTo) : 0;

            if (FilterType == "YearMonth")
            {
                var queryFilter = PredicateHelper.True<ExeReportByStatusMonthlyView>();

                if (!string.IsNullOrEmpty(locationCode))
                    queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
                if (!string.IsNullOrEmpty(YearFrom) && !string.IsNullOrEmpty(MonthFrom) && !string.IsNullOrEmpty(YearTo) && !string.IsNullOrEmpty(MonthTo))
                    queryFilter = queryFilter.And(p => (p.Year >= FromYear && p.Month >= FromMonth) && (p.Year <= FromYear && p.Month <= FromMonth));

                var dbResult = _exeReportByStatusMonthlyRepo.Get(queryFilter);

                if (dbResult.Count() > 0)
                {
                    return dbResult.Select(m => m.BrandGroupCode).Distinct().ToList();
                }
            }
            else if (FilterType == "YearWeek")
            {
                var queryFilter = PredicateHelper.True<ExeReportByStatusWeeklyView>();

                if (!string.IsNullOrEmpty(locationCode))
                    queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
                if (!string.IsNullOrEmpty(YearFrom) && !string.IsNullOrEmpty(WeekFrom) && !string.IsNullOrEmpty(YearTo) && !string.IsNullOrEmpty(WeekTo))
                    queryFilter = queryFilter.And(p => (p.KPSYear >= FromYear && p.KPSWeek >= FromWeek) && (p.KPSYear <= ToYear && p.KPSWeek <= ToWeek));
                var dbResult = _exeReportByStatusWeeklyRepo.Get(queryFilter);

                if (dbResult.Count() > 0)
                {
                    return dbResult.Select(m => m.BrandGroupCode).Distinct().ToList();
                }
            }
            else
            {
                var queryFilter = PredicateHelper.True<ExeReportByStatusView>();

                if (locationCode != null)
                {
                    queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
                }
                if (FilterType == "Date")
                {
                    if (DateFrom != null && DateTo != null)
                        queryFilter =
                            queryFilter.And(
                                p =>
                                    DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(DateFrom) &&
                                    DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(DateTo));
                }
                else
                {
                    if (!string.IsNullOrEmpty(YearFrom))
                    {
                        DateFrom = DateTime.Parse(string.Format("{0000}", YearFrom) + "-01-01");
                        DateTo = DateTime.Parse(string.Format("{0000}", YearFrom) + "-12-31");
                        queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(DateTo));
                    }
                }

                var dbResult = _exeReportByStatusRepo.Get(queryFilter);

                if (dbResult.Count() > 0)
                {
                    return dbResult.Select(m => m.BrandGroupCode).Distinct().ToList();
                }
            }

            return new List<string>();

        }

        #endregion


        #region Report Daily Production

        public List<ExeReportDailyProductionAchievementViewDTO> GetReportByDaily(GetExeReportDailyProductionInput input)
        {
            var queryFilter = PredicateHelper.True<ExeReportDailyProductionAchievementView>();
            var alllevelLocation = _sqlSPRepo.GetLastChildLocationByLocationCode(input.LocationCode).Select(x => x.LocationCode);

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => alllevelLocation.Contains(p.LocationCode));


            var week = _masterDataBll.GetWeekByYearAndWeek(input.YearFrom, input.WeekFrom);
            queryFilter = queryFilter.And(m => m.ProductionDate >= week.StartDate && m.ProductionDate <= week.EndDate);


            var dbResult = _exeReportDailyProductionRepo.Get(queryFilter);
            //var dto = Mapper.Map<List<ExeReportDailyProductionAchievementViewDTO>>(dbResult);

            var gruopBrand =
                dbResult.GroupBy(c => c.SKTBrandCode).Select(c => new ExeReportDailyProductionAchievementViewDTO
                {
                    SKTBrandCode = c.Key,
                    Details = Mapper.Map<List<DataDailyProductionAchievmentDTO>>(c),
                    CountData = c.Count()

                });

            var totalDayMonday = 0;
            var totalDayTuesday = 0;
            var totalDayWednesday = 0;
            var totalDayThursday = 0;
            var totalDayFriday = 0;
            var totalDaySaturday = 0;
            var totalDaySunday = 0;

            var sumaAllday = 0;
            double? sumAllTpkValue = 0.0;
            var sumAllPackage = 0.0f;
            double? sumAllVarianceStick = 0.0;
            double? sumAllVariancePercen = 0.0;
            double? sumAllReliabilty = 0.0;

            //const int handRoleMonday = 0;
            //const int handRoleTuesday = 0;
            //const int handRoleWednesday = 0;
            //const int handRoleThursday = 0;
            //const int handRoleFriday = 0;
            //const int handRoleSaturday = 0;
            //const int handRoleSunday = 0;

            var list = new List<ExeReportDailyProductionAchievementViewDTO>();

            foreach (var item in gruopBrand)
            {
                var detailList = item.Details;
                var newDetail = new List<DataDailyProductionAchievmentDTO>();

                var j = 0;
                var itemDetail = new DataDailyProductionAchievmentDTO();
                foreach (var data in detailList)
                {
                    if (j == 0)
                    {
                        itemDetail = data;
                        j++;
                    }
                    else if (itemDetail.LocationCode != data.LocationCode)
                    {
                        itemDetail.TotalAllDay = itemDetail.ProductionMonday + itemDetail.ProductionTuesday + itemDetail.ProductionWednesday +
                        itemDetail.ProductionThursday + itemDetail.ProductionFriday + itemDetail.ProductionSaturday + itemDetail.ProductionSunday;

                        //package
                        sumAllPackage = sumAllPackage + itemDetail.Package;

                        //variance Stick
                        itemDetail.VarianceStick = itemDetail.TotalAllDay - itemDetail.TPKValue;
                        sumAllVarianceStick = sumAllVarianceStick + (itemDetail.TotalAllDay - itemDetail.TPKValue);


                        //variance %
                        itemDetail.VariancePersen = itemDetail.VarianceStick == 0 || itemDetail.SumTpkValue == 0 ? 0 : (itemDetail.VarianceStick / itemDetail.SumTpkValue) * 100;
                        sumAllVariancePercen = sumAllVariancePercen + itemDetail.VariancePersen;

                        //Reliabilty %
                        itemDetail.Realiability = itemDetail.TotalAllDay == 0 ? 0 : (itemDetail.SumTpkValue / itemDetail.TotalAllDay) * 100;
                        sumAllReliabilty = sumAllReliabilty + itemDetail.Realiability;

                        newDetail.Add(itemDetail);
                        itemDetail = data;
                    }

                    else if (itemDetail.LocationCode == data.LocationCode && itemDetail.BrandCode != data.BrandCode)
                    {
                        itemDetail.TotalAllDay = itemDetail.ProductionMonday + itemDetail.ProductionTuesday + itemDetail.ProductionWednesday +
                        itemDetail.ProductionThursday + itemDetail.ProductionFriday + itemDetail.ProductionSaturday + itemDetail.ProductionSunday;

                        //package
                        sumAllPackage = sumAllPackage + itemDetail.Package;

                        //variance Stick
                        itemDetail.VarianceStick = itemDetail.TotalAllDay - itemDetail.TPKValue;
                        sumAllVarianceStick = sumAllVarianceStick + (itemDetail.TotalAllDay - itemDetail.TPKValue);


                        //variance %
                        itemDetail.VariancePersen = itemDetail.VarianceStick == 0 || itemDetail.SumTpkValue == 0 ? 0 : (itemDetail.VarianceStick / itemDetail.SumTpkValue) * 100;
                        sumAllVariancePercen = sumAllVariancePercen + itemDetail.VariancePersen;

                        //Reliabilty %
                        itemDetail.Realiability = itemDetail.TotalAllDay == 0 ? 0 : (itemDetail.SumTpkValue / itemDetail.TotalAllDay) * 100;
                        sumAllReliabilty = sumAllReliabilty + itemDetail.Realiability;

                        newDetail.Add(itemDetail);
                        itemDetail = data;
                    }

                    switch ((int)data.ProductionDate.DayOfWeek)
                    {
                        case 1:
                            itemDetail.ProductionMonday = data.Production == null ? 0 : data.Production.Value;
                            totalDayMonday = totalDayMonday + itemDetail.ProductionMonday;
                            break;
                        case 2:
                            itemDetail.ProductionTuesday = data.Production == null ? 0 : data.Production.Value;
                            totalDayTuesday = totalDayTuesday + itemDetail.ProductionTuesday;
                            break;
                        case 3:
                            itemDetail.ProductionWednesday = data.Production == null ? 0 : data.Production.Value;
                            totalDayWednesday = itemDetail.ProductionWednesday;
                            break;
                        case 4:
                            itemDetail.ProductionThursday = data.Production == null ? 0 : data.Production.Value;
                            totalDayThursday = totalDayThursday + itemDetail.ProductionThursday;
                            break;
                        case 5:
                            itemDetail.ProductionFriday = data.Production == null ? 0 : data.Production.Value;
                            totalDayFriday = totalDayFriday + itemDetail.ProductionFriday;
                            break;
                        case 6:
                            itemDetail.ProductionSaturday = data.Production == null ? 0 : data.Production.Value;
                            totalDaySaturday = totalDaySaturday + itemDetail.ProductionSaturday;
                            break;
                        case 7:
                            itemDetail.ProductionSunday = data.Production == null ? 0 : data.Production.Value;
                            totalDaySunday = totalDaySunday + itemDetail.ProductionSunday;
                            break;
                    }

                    itemDetail.SumTpkValue = data.TPKValue + itemDetail.TPKValue;
                    itemDetail.TotalAllDay = itemDetail.ProductionMonday + itemDetail.ProductionTuesday + itemDetail.ProductionWednesday +
                                         itemDetail.ProductionThursday + itemDetail.ProductionFriday + itemDetail.ProductionSaturday +
                                         itemDetail.ProductionSunday;

                    sumaAllday = sumaAllday + itemDetail.TotalAllDay;
                }

                //variance Stick
                itemDetail.VarianceStick = itemDetail.TotalAllDay - itemDetail.TPKValue;
                sumAllVarianceStick = sumAllVarianceStick + (itemDetail.TotalAllDay - itemDetail.TPKValue);

                //variance %
                itemDetail.VariancePersen = itemDetail.VarianceStick == 0 || itemDetail.SumTpkValue == 0 ? 0 : (itemDetail.VarianceStick / itemDetail.SumTpkValue) * 100;
                sumAllVariancePercen = sumAllVariancePercen + itemDetail.VariancePersen;

                //Reliabilty %
                itemDetail.Realiability = itemDetail.TotalAllDay == 0 || itemDetail.SumTpkValue == 0 ? 0 : (itemDetail.SumTpkValue / itemDetail.TotalAllDay) * 100;
                sumAllReliabilty = sumAllReliabilty + itemDetail.Realiability;

                //package
                sumAllPackage = sumAllPackage + itemDetail.Package;

                newDetail.Add(itemDetail);

                //totalperday
                item.TotalMonday = totalDayMonday;
                item.TotalTuesday = totalDayTuesday;
                item.TotalWednesday = totalDayWednesday;
                item.TotalThursday = totalDayThursday;
                item.TotalFriday = totalDayFriday;
                item.TotalSaturday = totalDaySaturday;
                item.TotalSunday = totalDaySunday;
                item.SumTotalAllDay = sumaAllday;
                item.SumAllTpkValue = sumAllTpkValue + itemDetail.SumTpkValue; //Sum all planning one row 
                item.SumAllPackage = sumAllPackage;
                item.SumAllVarianStick = sumAllVarianceStick;
                item.SumAllVarianPercen = sumAllVariancePercen;
                item.SumAllReliabilty = sumAllReliabilty;

                //TotalComulative Stick
                item.StickCumulativeMonday = item.TotalMonday;
                item.StickCumulativeTuesday = item.StickCumulativeMonday + item.TotalTuesday;
                item.StickCumulativeWednesday = item.StickCumulativeTuesday + item.TotalWednesday;
                item.StickCumulativeThursday = item.StickCumulativeWednesday + item.TotalWednesday;
                item.StickCumulativeFriday = item.StickCumulativeThursday + item.TotalFriday;
                item.StickCumulativeSaturday = item.StickCumulativeFriday + item.TotalSaturday;
                item.StickCumulativeSunday = item.StickCumulativeSaturday + item.TotalSunday;

                //TotalComulative %
                item.CumulativeMonday = totalDayMonday == 0 || itemDetail.SumTpkValue == 0 ? 0 : (totalDayMonday / itemDetail.SumTpkValue) * 100;
                item.CumulativeTuesday = totalDayTuesday == 0 || itemDetail.SumTpkValue == 0 ? 0 : (totalDayTuesday / itemDetail.SumTpkValue) * 100;
                item.CumulativeWednesday = totalDayWednesday == 0 || itemDetail.SumTpkValue == 0 ? 0 : (totalDayWednesday / itemDetail.SumTpkValue) * 100;
                item.CumulativeThursday = totalDayThursday == 0 || itemDetail.SumTpkValue == 0 ? 0 : (totalDayThursday / itemDetail.SumTpkValue) * 100;
                item.CumulativeFriday = totalDayWednesday == 0 || itemDetail.SumTpkValue == 0 ? 0 : (totalDayThursday / itemDetail.SumTpkValue) * 100;
                item.CumulativeSaturday = totalDaySaturday == 0 || itemDetail.SumTpkValue == 0 ? 0 : (totalDayThursday / itemDetail.SumTpkValue) * 100;
                item.CumulativeSunday = totalDaySunday == 0 || itemDetail.SumTpkValue == 0 ? 0 : (totalDayThursday / itemDetail.SumTpkValue) * 100;
                item.TotalAllCumulative = item.CumulativeMonday + item.CumulativeTuesday + item.CumulativeWednesday +
                                          item.CumulativeThursday + item.CumulativeFriday + item.CumulativeSaturday +
                                          item.CumulativeSunday;

                ////TotalHandRole
                //item.TotalHandRoleMonday = handRoleMonday + item.TotalMonday;
                //item.TotalHandRoleTuesday = handRoleTuesday + item.TotalTuesday;
                //item.TotalHandrRoleWednesday = handRoleWednesday + item.TotalWednesday;
                //item.TotalHandrRoleThursday = handRoleThursday + item.TotalHandrRoleThursday;
                //item.TotalHandrRoleFriday = handRoleFriday + item.TotalHandrRoleFriday;
                //item.TotalHandrRoleSaturday = handRoleSaturday + item.TotalHandrRoleSaturday;
                //item.TotalHandrRoleSunday = handRoleSunday + item.TotalHandrRoleSunday;

                item.Details = newDetail;

                list.Add(item);

                totalDayMonday = 0;
                totalDayTuesday = 0;
                totalDayWednesday = 0;
                totalDayThursday = 0;
                totalDayFriday = 0;
                totalDaySaturday = 0;
                totalDaySunday = 0;
                sumaAllday = 0;
                sumAllTpkValue = 0;
                sumAllVarianceStick = 0;
                sumAllVariancePercen = 0;
                sumAllReliabilty = 0;
                sumAllPackage = 0;

            }

            var newList = list;

            return newList;
        }

        public List<ExeReportingDailyProductionAchievementDTOSKTBrandCode> GetExeReportProductionDailyAchievement(GetExeReportDailyProductionInput input)
        {
            var dbResult = _sqlSPRepo.GetExeReportDailyProductionAchievement(input.LocationCode, input.WeekFrom, input.YearFrom);

            var resultDto = Mapper.Map<List<GetExeReportDailyProductionAchievementDTO>>(dbResult);

            // Grouping by Brand Code
            var groupByBrandCode = resultDto
                               .GroupBy(c => new { c.SKTBrandCode, c.BrandCode })
                               .Select(g => new ExeReportingDailyProductionAchievementDTOBrandCode
                               {
                                   SKTBrandCode = g.Key.SKTBrandCode,
                                   BrandCode = g.Key.BrandCode,
                                   CountBrandCode = g.Count(),
                                   SubTotalMonday = g.Sum(c => c.Monday),
                                   SubTotalTuesday = g.Sum(c => c.Tuesday),
                                   SubTotalWednesday = g.Sum(c => c.Wednesday),
                                   SubTotalThursday = g.Sum(c => c.Thursday),
                                   SubTotalFriday = g.Sum(c => c.Friday),
                                   SubTotalSaturday = g.Sum(c => c.Saturday),
                                   SubTotalSunday = g.Sum(c => c.Sunday),
                                   SubTotalTotal = g.Sum(c => c.Total),
                                   SubTotalPlanning = g.Sum(c => c.Planning),
                                   SubTotalVarianceStick = g.Sum(c => c.VarianceStick),
                                   //SubTotalVariancePercent = g.Sum(c => c.VariancePercent),
                                   SubTotalVariancePercent = g.Sum(c => Convert.ToDecimal(c.Planning)) == 0 ? 0 : g.Sum(c => Convert.ToDecimal(c.VarianceStick)) / g.Sum(c => Convert.ToDecimal(c.Planning)),
                                   //SubTotalReliabilityPercent = g.Sum(c => c.ReliabilityPercent),
                                   SubTotalReliabilityPercent = g.Sum(c => Convert.ToDecimal(c.Planning)) == 0 ? 0 : g.Sum(c => Convert.ToDecimal(c.Total)) / g.Sum(c => Convert.ToDecimal(c.Planning)),
                                   SubTotalPackage = g.Sum(c => c.Package),
                                   //SubTotalTWHEqv = g.Sum(c => c.TWHEqv),
                                   SubTotalTWHEqv = g.Average(c => c.TWHEqv),
                                   ListPerBrandCode = g.ToList().OrderBy(m=>m.LocationCode).ToList()
                               });

            // Grouping by SKT Brand Group
            var groupBySKTBrandCode = groupByBrandCode
                .GroupBy(c => c.SKTBrandCode)
                .Select(g => new ExeReportingDailyProductionAchievementDTOSKTBrandCode
                {
                    SKTBrandCode = g.Key,
                    CountSKTBrandCode = g.Sum(c => c.CountBrandCode),
                    TotalMonday = g.Sum(c => c.SubTotalMonday),
                    TotalTuesday = g.Sum(c => c.SubTotalTuesday),
                    TotalWednesday = g.Sum(c => c.SubTotalWednesday),
                    TotalThursday = g.Sum(c => c.SubTotalThursday),
                    TotalFriday = g.Sum(c => c.SubTotalFriday),
                    TotalSaturday = g.Sum(c => c.SubTotalSaturday),
                    TotalSunday = g.Sum(c => c.SubTotalSunday),
                    TotalTotal = g.Sum(c => c.SubTotalTotal),
                    TotalPlanning = g.Sum(c => c.SubTotalPlanning),
                    TotalVarianceStick = g.Sum(c => c.SubTotalVarianceStick),
                    //TotalVariancePercent = g.Sum(c => c.SubTotalVariancePercent),
                    TotalVariancePercent = g.Sum(c => Convert.ToDecimal(c.SubTotalPlanning)) == 0 ? 0 : g.Sum(c => Convert.ToDecimal(c.SubTotalVarianceStick)) / g.Sum(c => Convert.ToDecimal(c.SubTotalPlanning)),
                    //TotalReliabilityPercent = g.Sum(c => c.SubTotalReliabilityPercent),
                    TotalReliabilityPercent = g.Sum(c => Convert.ToDecimal(c.SubTotalPlanning)) == 0 ? 0 : g.Sum(c => Convert.ToDecimal(c.SubTotalTotal)) / g.Sum(c => Convert.ToDecimal(c.SubTotalPlanning)),
                    TotalPackage = g.Sum(c => c.SubTotalPackage),
                    //TotalTWHEqv = g.Sum(c => c.SubTotalTWHEqv),
                    TotalTWHEqv = g.Average(c => c.SubTotalTWHEqv),
                    ListPerSKTBrandCode = g.ToList(),
                    CumulativeTotalMonday = g.Sum(c => c.SubTotalMonday),
                    CumulativeTotalTuesday = g.Sum(c => c.SubTotalMonday) + g.Sum(c => c.SubTotalTuesday),
                    CumulativeTotalWednesday = g.Sum(c => c.SubTotalMonday) + g.Sum(c => c.SubTotalTuesday) + g.Sum(c => c.SubTotalWednesday),
                    CumulativeTotalThursday = g.Sum(c => c.SubTotalMonday) + g.Sum(c => c.SubTotalTuesday) + g.Sum(c => c.SubTotalWednesday) + g.Sum(c => c.SubTotalThursday),
                    CumulativeTotalFriday = g.Sum(c => c.SubTotalMonday) + g.Sum(c => c.SubTotalTuesday) + g.Sum(c => c.SubTotalWednesday) + g.Sum(c => c.SubTotalThursday) + g.Sum(c => c.SubTotalFriday),
                    CumulativeTotalSaturday = g.Sum(c => c.SubTotalMonday) + g.Sum(c => c.SubTotalTuesday) + g.Sum(c => c.SubTotalWednesday) + g.Sum(c => c.SubTotalThursday) + g.Sum(c => c.SubTotalFriday) + g.Sum(c => c.SubTotalSaturday),
                    CumulativeTotalSunday = g.Sum(c => c.SubTotalMonday) + g.Sum(c => c.SubTotalTuesday) + g.Sum(c => c.SubTotalWednesday) + g.Sum(c => c.SubTotalThursday) + g.Sum(c => c.SubTotalFriday) + g.Sum(c => c.SubTotalSaturday) + g.Sum(c => c.SubTotalSunday),
                    CumulativePercentTotalMonday = g.Sum(c => c.SubTotalPlanning) == 0 ? 0 : (g.Sum(c => c.SubTotalMonday) / g.Sum(c => c.SubTotalPlanning)) * 100,
                }).ToList();

            foreach (var item in groupBySKTBrandCode)
            {
                item.CumulativePercentTotalMonday = item.TotalPlanning == null ? 0 : item.TotalPlanning.Value == 0 ? 0 : (item.CumulativeTotalMonday / item.TotalPlanning) * 100;
                item.CumulativePercentTotalTuesday = item.TotalPlanning == null ? 0 : item.TotalPlanning.Value == 0 ? 0 : (item.CumulativeTotalTuesday / item.TotalPlanning) * 100;
                item.CumulativePercentTotalWednesday = item.TotalPlanning == null ? 0 : item.TotalPlanning.Value == 0 ? 0 : (item.CumulativeTotalWednesday / item.TotalPlanning) * 100;
                item.CumulativePercentTotalThursday = item.TotalPlanning == null ? 0 : item.TotalPlanning.Value == 0 ? 0 : (item.CumulativeTotalThursday / item.TotalPlanning) * 100;
                item.CumulativePercentTotalFriday = item.TotalPlanning == null ? 0 : item.TotalPlanning.Value == 0 ? 0 : (item.CumulativeTotalFriday / item.TotalPlanning) * 100;
                item.CumulativePercentTotalSaturday = item.TotalPlanning == null ? 0 : item.TotalPlanning.Value == 0 ? 0 : (item.CumulativeTotalSaturday / item.TotalPlanning) * 100;
                item.CumulativePercentTotalSunday = item.TotalPlanning == null ? 0 : item.TotalPlanning.Value == 0 ? 0 : (item.CumulativeTotalSunday / item.TotalPlanning) * 100;
            }

            return groupBySKTBrandCode;
        }

        #endregion

        #region Report By Process

        public List<ExeReportByProcessViewDTO> GetReportByProcess(GetExeReportByProcessInput input)
        {
            var queryFilter = PredicateHelper.True<ExeReportByProcessView>();
            //var result = new List<ExeReportByProcessViewDTO>();
            if (input.FilterType == "Year")
            {
                input.DateFrom = DateTime.Parse(string.Format("{0000}", input.YearFrom.Value) + "-01-01");
                input.DateTo = DateTime.Parse(string.Format("{0000}", input.YearFrom.Value) + "-12-31");
            }
            else if (input.FilterType == "YearMonth")
            {
                input.DateFrom = DateTime.Parse(string.Format("{0000}", input.YearFrom.Value) + "-" + string.Format("{00}", input.MonthFrom.Value) + "-01");
                input.DateTo = DateTime.Parse(string.Format("{0000}", input.YearTo.Value) + "-" + string.Format("{00}", input.MonthTo.Value) + "-01").AddMonths(1).AddDays(-1);
            }
            else if (input.FilterType == "YearWeek")
            {
                var dateFrom = _masterDataBll.GetDateByWeek(input.YearFrom.Value, input.WeekFrom.Value);
                var dateTo = _masterDataBll.GetDateByWeek(input.YearTo.Value, input.WeekTo.Value);
                input.DateFrom = dateFrom.FirstOrDefault();
                input.DateTo = dateTo.LastOrDefault();

            }
            
            //queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode
            //        && p.UnitCode == input.UnitCode
            //        && p.Shift == input.Shift
            //        && DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(input.DateFrom)
            //        && DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(input.DateTo)
            //    );

            //if (!string.IsNullOrEmpty(input.BrandCode))
            //{
            //    if (input.BrandCode != "All")
            //        queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            //}
            
            //var dbResult = _exeReportByProcessViewRepo.Get(queryFilter);
            ////var ci = new CultureInfo("en-US");
            //var ci = CultureInfo.CurrentCulture;
            //var dbReturn = (from x in dbResult
            //                group x by new
            //                {

            //                    x.Description,
            //                    x.UOM,
            //                    x.ProcessOrder,
            //                    //x.BeginningStock,
            //                    //x.Production,
            //                    //x.KeluarBersih,
            //                    //x.RejectSample,
            //                    //x.EndingStock,

            //                }
            //                    into grouped
            //                    select new ExeReportByProcessViewDTO()
            //            {
            //                Description = grouped.Key.Description,
            //                UOM = grouped.Key.UOM,
            //                BeginningStock = grouped.Sum(y => y.BeginningStock),
            //                Production = grouped.Sum(y => y.Production),
            //                KeluarBersih = grouped.Sum(y => y.KeluarBersih),
            //                RejectSample = grouped.Sum(y => y.RejectSample),
            //                EndingStock = grouped.Sum(y => y.EndingStock),
            //                ProcessOrder = grouped.Key.ProcessOrder
            //            }).OrderBy(x => x.ProcessOrder).ToList();


            ////return Mapper.Map<List<ExeReportByProcessDTO>>(dbReturn);
            //return dbReturn;

            if(input.LocationCode == Enums.LocationCode.SKT.ToString() || input.LocationCode == Enums.LocationCode.PLNT.ToString() || 
                input.LocationCode == Enums.LocationCode.TPO.ToString() || input.LocationCode == Enums.LocationCode.REG1.ToString() ||
                input.LocationCode == Enums.LocationCode.REG2.ToString() || input.LocationCode == Enums.LocationCode.REG3.ToString() ||
                input.LocationCode == Enums.LocationCode.REG4.ToString() || input.UnitCode == "All" || input.BrandCode == "All")
            {
                var dbResult = _sqlSPRepo.GetExeReportByProcessFuncParent(input.DateFrom.Value, input.DateTo.Value, input.LocationCode, input.UnitCode, input.Shift.Value, input.BrandCode);
                var dbResultReturn = (from x in dbResult
                            select new ExeReportByProcessViewDTO()
                            {
                                Description = x.Description,
                                UOM = x.UOM,
                                BeginningStock = x.BeginningStock,
                                Production = x.Production,
                                KeluarBersih = x.KeluarBersih,
                                RejectSample = x.RejectSample,
                                EndingStock = x.EndingStock,
                                ProcessOrder = x.ProcessOrder
                            }).OrderBy(x => x.UOMOrder).ToList();
                return dbResultReturn;
            }

            var result = _sqlSPRepo.GetExeReportByProcessFunc(input.DateFrom.Value, input.DateTo.Value, input.LocationCode, input.UnitCode, input.Shift.Value, input.BrandCode);
            var dbReturn = (from x in result
                            select new ExeReportByProcessViewDTO()
                            {
                                Description = x.Description,
                                UOM = x.UOM,
                                BeginningStock = x.BeginningStock,
                                Production = x.Production,
                                KeluarBersih = x.KeluarBersih,
                                RejectSample = x.RejectSample,
                                EndingStock = x.EndingStock,
                                ProcessOrder = x.ProcessOrder
                            }).OrderBy(x => x.UOMOrder).ToList();
            return dbReturn;
        }

        public List<ExeReportByProcessViewDTO> GetReportByProcessGenerator(GetExeReportByProcessInput input)
        {
            var queryFilter = PredicateHelper.True<ExeReportByProcessView>();

            queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode
                    && p.UnitCode == input.UnitCode
                    && p.BrandCode == input.BrandCode
                    && p.ProductionDate >= input.DateFrom
                    && p.ProductionDate <= input.DateTo);

             var dbResult = _exeReportByProcessViewRepo.Get(queryFilter);

            return Mapper.Map<List<ExeReportByProcessViewDTO>>(dbResult);
        }

        //public List<string> GetActiveBrandGroupCodeReportByProcess(string locationCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType)
        //{
        //    var DateFrom = Convert.ToDateTime(FromDate);
        //    var DateTo = Convert.ToDateTime(ToDate);
        //    var FromYear = (!string.IsNullOrEmpty(YearFrom)) ? int.Parse(YearFrom) : 0;
        //    var ToYear = (!string.IsNullOrEmpty(YearTo)) ? int.Parse(YearTo) : 0;
        //    var FromMonth = (!string.IsNullOrEmpty(MonthFrom)) ? int.Parse(MonthFrom) : 0;
        //    var ToMonth = (!string.IsNullOrEmpty(MonthTo)) ? int.Parse(MonthTo) : 0;
        //    var FromWeek = (!string.IsNullOrEmpty(WeekFrom)) ? int.Parse(WeekFrom) : 0;
        //    var ToWeek = (!string.IsNullOrEmpty(WeekTo)) ? int.Parse(WeekTo) : 0;

        //    if (FilterType == "YearMonth")
        //    {
        //        var queryFilter = PredicateHelper.True<ExeReportByProcessView>();

        //        if (!string.IsNullOrEmpty(locationCode))
        //            queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
        //        if (!string.IsNullOrEmpty(YearFrom) && !string.IsNullOrEmpty(MonthFrom) && !string.IsNullOrEmpty(YearTo) && !string.IsNullOrEmpty(MonthTo))
        //            queryFilter = queryFilter.And(p => (p.KPSYear >= FromYear && p.ProductionDate.Month >= FromMonth) && (p.KPSYear <= FromYear && p.ProductionDate.Month <= FromMonth));

        //        var dbResult = _exeReportByProcessViewRepo.Get(queryFilter);

        //        if (dbResult.Count() > 0)
        //        {
        //            return dbResult.Select(m => m.BrandGroupCode).Distinct().ToList();
        //        }
        //    }
        //    else if (FilterType == "YearWeek")
        //    {
        //        var queryFilter = PredicateHelper.True<ExeReportByProcessView>();

        //        if (!string.IsNullOrEmpty(locationCode))
        //            queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
        //        if (!string.IsNullOrEmpty(YearFrom) && !string.IsNullOrEmpty(WeekFrom) && !string.IsNullOrEmpty(YearTo) && !string.IsNullOrEmpty(WeekTo))
        //            queryFilter = queryFilter.And(p => (p.KPSYear >= FromYear && p.KPSWeek >= FromWeek) && (p.KPSYear <= ToYear && p.KPSWeek <= ToWeek));
        //        var dbResult = _exeReportByProcessViewRepo.Get(queryFilter);

        //        if (dbResult.Count() > 0)
        //        {
        //            return dbResult.Select(m => m.BrandGroupCode).Distinct().ToList();
        //        }
        //    }
        //    else
        //    {
        //        var queryFilter = PredicateHelper.True<ExeReportByProcessView>();

        //        if (locationCode != null)
        //        {
        //            queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
        //        }
        //        if (FilterType == "Date")
        //        {
        //            if (DateFrom != null && DateTo != null)
        //                queryFilter =
        //                    queryFilter.And(
        //                        p =>
        //                            DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(DateFrom) &&
        //                            DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(DateTo));
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(YearFrom))
        //            {
        //                DateFrom = DateTime.Parse(string.Format("{0000}", YearFrom) + "-01-01");
        //                DateTo = DateTime.Parse(string.Format("{0000}", YearFrom) + "-12-31");
        //                queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.ProductionDate) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(p.ProductionDate) <= DbFunctions.TruncateTime(DateTo));
        //            }
        //        }

        //        var dbResult = _exeReportByProcessViewRepo.Get(queryFilter);

        //        if (dbResult.Count() > 0)
        //        {
        //            return dbResult.Select(m => m.BrandGroupCode).Distinct().ToList();
        //        }
        //    }

        //    return new List<string>();

        //}

        public IEnumerable<string> GetActiveBrandGroupCodeReportByProcess(string locationCode, string unitCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType) {
            var tupleRangeDate = GetProdDateFromAndTo(YearFrom, YearTo, MonthFrom, MonthTo, WeekFrom, WeekTo, FromDate, ToDate, FilterType);
            var productionDateFrom = tupleRangeDate.Item1.Date;
            var productionDateTo = tupleRangeDate.Item2.Date;

            var listLocationCode = _masterDataBll.GetLastChildLocation(locationCode).OrderBy(c => c.LocationCode).Select(c => c.LocationCode).Distinct();

            IEnumerable<string> listResult = new List<string>();

            using (SKTISEntities context = new SKTISEntities()) {
                if (unitCode == "All") {
                    var listBrandCode = context.ExeReportByProcesses.OrderBy(c => c.BrandCode).Where(c => listLocationCode.Contains(c.LocationCode) && c.ProductionDate >= productionDateFrom && c.ProductionDate <= productionDateTo).Select(c => c.BrandCode).Distinct();
                    listResult = context.MstGenBrands.OrderBy(c => c.BrandGroupCode).Where(c => listBrandCode.Contains(c.BrandCode)).Select(c => c.BrandGroupCode).Distinct().ToList();
                }
                else {
                    var listBrandCode = context.ExeReportByProcesses.OrderBy(c => c.BrandCode).Where(c => listLocationCode.Contains(c.LocationCode) && c.UnitCode == unitCode && c.ProductionDate >= productionDateFrom && c.ProductionDate <= productionDateTo).Select(c => c.BrandCode).Distinct();
                    listResult = context.MstGenBrands.OrderBy(c => c.BrandGroupCode).Where(c => listBrandCode.Contains(c.BrandCode)).Select(c => c.BrandGroupCode).Distinct().ToList();
                }
            }
            return listResult;
        }

        private Tuple<DateTime, DateTime> GetProdDateFromAndTo(string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType) {
            // Get <productionDateFrom, productionDateTo>
            var productionDateFrom = DateTime.Now.Date;
            var productionDateTo = DateTime.Now.Date;

            if (FilterType == "Year") {
                productionDateFrom = new DateTime(Convert.ToInt32(YearFrom), 1, 1);
                productionDateTo = new DateTime(Convert.ToInt32(YearTo), 12, 31);
            }
            else if (FilterType == "YearMonth") {
                productionDateFrom = new DateTime(Convert.ToInt32(YearFrom), Convert.ToInt32(MonthFrom), 1);
                var daysInMonth = DateTime.DaysInMonth(Convert.ToInt32(YearTo), Convert.ToInt32(MonthTo));
                productionDateTo = new DateTime(Convert.ToInt32(YearTo), Convert.ToInt32(MonthTo), daysInMonth);
            }
            else if (FilterType == "YearWeek") {
                using (SKTISEntities context = new SKTISEntities()) {
                    productionDateFrom = context.MstGenWeeks.Where(c => c.Year == Convert.ToInt32(YearFrom) && c.Week == Convert.ToInt32(WeekFrom)).Select(c => c.StartDate.Value).FirstOrDefault();
                    productionDateTo = context.MstGenWeeks.Where(c => c.Year == Convert.ToInt32(YearTo) && c.Week == Convert.ToInt32(WeekTo)).Select(c => c.EndDate.Value).FirstOrDefault();
                }
            }
            else {
                productionDateFrom = DateTime.Parse(FromDate);
                productionDateTo = DateTime.Parse(ToDate);
            }

            // Return <productionDateFrom, productionDateTo>
            return new Tuple<DateTime, DateTime>(productionDateFrom, productionDateTo);
        }

        public List<string> GetActiveBrandCodeReportByProcess(string location, string unitCode, string brandGroupCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType)
        {
            var tupleRangeDate = GetProdDateFromAndTo(YearFrom, YearTo, MonthFrom, MonthTo, WeekFrom, WeekTo, FromDate, ToDate, FilterType);
            var productionDateFrom = tupleRangeDate.Item1.Date;
            var productionDateTo = tupleRangeDate.Item2.Date;

            var brandCodeList = _mstGenBrand.Get(c => c.BrandGroupCode == brandGroupCode).Select(c => c.BrandCode).Distinct().ToList();

            var listLocationCode = _masterDataBll.GetLastChildLocation(location).OrderBy(c => c.LocationCode).Select(c => c.LocationCode).Distinct();

            var listResult = new List<string>();

            using (SKTISEntities context = new SKTISEntities()) {
                if (unitCode == "All") {
                    listResult = context.ExeReportByProcesses.OrderBy(c => c.BrandCode).Where(c => listLocationCode.Contains(c.LocationCode) && brandCodeList.Contains(c.BrandCode) && c.ProductionDate >= productionDateFrom && c.ProductionDate <= productionDateTo).Select(c => c.BrandCode).Distinct().ToList();
                }
                else {
                    listResult = context.ExeReportByProcesses.OrderBy(c => c.BrandCode).Where(c => listLocationCode.Contains(c.LocationCode) && brandCodeList.Contains(c.BrandCode) && c.UnitCode == unitCode && c.ProductionDate >= productionDateFrom && c.ProductionDate <= productionDateTo).Select(c => c.BrandCode).Distinct().ToList();
                }
            }

            return listResult;
        }
        #endregion

        public void DeleteReportByProcess(string locationCode, string unitCode, string processGroup, string brandCode, DateTime productionDate)
        {
            var reportByProcessList = _exeReportByProcessRepo.Get(c => c.LocationCode == locationCode
                                                            && c.ProcessGroup == processGroup
                                                            && c.UnitCode == unitCode
                                                            && c.BrandCode == brandCode
                                                            && DbFunctions.TruncateTime(productionDate) == DbFunctions.TruncateTime(c.ProductionDate));
            foreach (var item in reportByProcessList)
            {
                _exeReportByProcessRepo.Delete(item);
            }

            _uow.SaveChanges();
        }

        public void UpdateEndingStockByProcess(string locationCode, string unitCode, string brandCode, DateTime productionDate)
        {
            var previousDate = productionDate.AddDays(-1);
            var reportByProcessListPreviousDay = _exeReportByProcessRepo.Get(c => c.LocationCode == locationCode
                                                            && c.UnitCode == unitCode
                                                            && c.BrandCode == brandCode
                                                            && c.ProductionDate == previousDate);

            var reportByProcessList = _exeReportByProcessRepo.Get(c => c.LocationCode == locationCode
                                                            && c.UnitCode == unitCode
                                                            && c.BrandCode == brandCode
                                                            && c.ProductionDate == productionDate);

            foreach (var item in reportByProcessList)
	        {
		        var previousDayByProcess = reportByProcessListPreviousDay.Where(c => c.LocationCode == item.LocationCode &&
                                                                                     c.UnitCode == item.UnitCode &&
                                                                                     c.BrandCode == item.BrandCode &&
                                                                                     c.ProcessGroup == item.ProcessGroup &&
                                                                                     c.UOMOrder == item.UOMOrder &&
                                                                                     c.ProcessOrder == item.ProcessOrder).FirstOrDefault();

                if(previousDayByProcess != null)
                {
                    item.BeginningStock = previousDayByProcess.EndingStock;
                    item.EndingStock = previousDayByProcess.EndingStock;
                    _exeReportByProcessRepo.Update(item);
                }
	        }

            _uow.SaveChanges();
        }

        public List<ExeReportByStatusActualWorkHourOutput> GetActualWorkWeekly(List<ExeReportByStatusWeeklyDTO> listData)
        {
            var result = new List<ExeReportByStatusActualWorkHourOutput>();

            var countWeek = listData.GroupBy(c => new
            {
                c.KPSYear,
                c.KPSWeek
            }).Select(x => new ExeReportByStatusWeeklyDTO()
            {
                KPSYear = x.Key.KPSYear,
                KPSWeek = x.Key.KPSWeek,
            });

            if (countWeek.Count() > 1)
            {
                foreach (var exeReportByStatusWeeklyDto in listData)
                {
                    var processGroup = exeReportByStatusWeeklyDto.ProcessGroup;
                    var status = exeReportByStatusWeeklyDto.StatusEmp;
                    var week = exeReportByStatusWeeklyDto.KPSWeek;
                    var year = exeReportByStatusWeeklyDto.KPSYear;

                    double sumActualWorkHourWeek = 0;
                    double sumActual = 0;

                    foreach (var reportByStatusWeeklyDto in listData)
                    {
                        if (reportByStatusWeeklyDto.ProcessGroup == processGroup &&
                            reportByStatusWeeklyDto.StatusEmp == status)
                        {
                            int actualWorker = reportByStatusWeeklyDto.ActualWorker.HasValue
                                ? reportByStatusWeeklyDto.ActualWorker.Value
                                : 0;
                            double actualWorkHour = reportByStatusWeeklyDto.ActualWorkHour.HasValue
                                ? reportByStatusWeeklyDto.ActualWorkHour.Value
                                : 0;
                            sumActualWorkHourWeek += (actualWorker * actualWorkHour);
                            sumActual += actualWorker;
                        }
                    }

                    var processGroupby = new ExeReportByStatusActualWorkHourOutput();
                    processGroupby.ProcessGroup = processGroup;
                    processGroupby.StatusEmp = status;
                    if (sumActual == 0)
                        processGroupby.ActualWorkHour = 0;
                    else
                        processGroupby.ActualWorkHour = Math.Round((sumActualWorkHourWeek / sumActual), 2);
                    result.Add(processGroupby);

                }

            }
            return result;
        }

        public List<ExeReportByStatusActualWorkHourOutput> GetActualWorkMonthly(List<ExeReportByStatusMonthlyDTO> listData)
        {
            var result = new List<ExeReportByStatusActualWorkHourOutput>();

            var countWeek = listData.GroupBy(c => new
            {
                c.Month,
                c.Year
            }).Select(x => new ExeReportByStatusMonthlyDTO()
            {
                Month = x.Key.Month,
                Year = x.Key.Year,
            });

            if (countWeek.Count() > 1)
            {
                foreach (var exeReportByStatusMonthlyDto in listData)
                {
                    var processGroup = exeReportByStatusMonthlyDto.ProcessGroup;
                    var status = exeReportByStatusMonthlyDto.StatusEmp;

                    double sumActualWorkHourWeek = 0;
                    double sumActual = 0;

                    foreach (var reportByStatusMonthlyDto in listData)
                    {
                        if (reportByStatusMonthlyDto.ProcessGroup == processGroup &&
                            reportByStatusMonthlyDto.StatusEmp == status)
                        {
                            int actualWorker = reportByStatusMonthlyDto.ActualWorker.HasValue
                                ? reportByStatusMonthlyDto.ActualWorker.Value
                                : 0;
                            double actualWorkHour = reportByStatusMonthlyDto.ActualWorkHour.HasValue
                                ? reportByStatusMonthlyDto.ActualWorkHour.Value
                                : 0;
                            sumActualWorkHourWeek += (actualWorker * actualWorkHour);
                            sumActual += actualWorker;
                        }
                    }

                    var processGroupby = new ExeReportByStatusActualWorkHourOutput();
                    processGroupby.ProcessGroup = processGroup;
                    processGroupby.StatusEmp = status;
                    if (sumActual == 0)
                        processGroupby.ActualWorkHour = 0;
                    else
                        processGroupby.ActualWorkHour = Math.Round((sumActualWorkHourWeek / sumActual), 2);
                    result.Add(processGroupby);
                }

            }
            return result;
        }

        public List<ExeReportByStatusActualWorkHourOutput> GetActualWork(List<ExeReportByStatusDTO> listData)
        {
            var result = new List<ExeReportByStatusActualWorkHourOutput>();

            var countWeek = listData.GroupBy(c => new
            {
                c.ProductionDate
            }).Select(x => new ExeReportByStatusDTO()
            {
                ProductionDate = x.Key.ProductionDate
            });

            if (countWeek.Count() > 1)
            {
                foreach (var exeReportByStatusWeeklyDto in listData)
                {
                    var processGroup = exeReportByStatusWeeklyDto.ProcessGroup;
                    var status = exeReportByStatusWeeklyDto.StatusEmp;

                    double sumActualWorkHourWeek = 0;
                    double sumActual = 0;

                    foreach (var reportByStatusWeeklyDto in listData)
                    {
                        if (reportByStatusWeeklyDto.ProcessGroup == processGroup &&
                            reportByStatusWeeklyDto.StatusEmp == status)
                        {
                            int actualWorker = reportByStatusWeeklyDto.ActualWorker.HasValue
                                ? reportByStatusWeeklyDto.ActualWorker.Value
                                : 0;
                            double actualWorkHour = reportByStatusWeeklyDto.ActualWorkHour.HasValue
                                ? reportByStatusWeeklyDto.ActualWorkHour.Value
                                : 0;
                            sumActualWorkHourWeek += (actualWorker * actualWorkHour);
                            sumActual += actualWorker;
                        }
                    }

                    var processGroupby = new ExeReportByStatusActualWorkHourOutput();
                    processGroupby.ProcessGroup = processGroup;
                    processGroupby.StatusEmp = status;
                    if (sumActual == 0)
                        processGroupby.ActualWorkHour = 0;
                    else
                        processGroupby.ActualWorkHour = Math.Round((sumActualWorkHourWeek / sumActual), 2);
                    result.Add(processGroupby);
                    //exeReportByStatusWeeklyDto.ActualWorkHour = sumActualWorkHourWeek / sumActual;
                }

            }
            return result;
        }

        public string GetTotalWorkHour(List<ExeReportByStatusDTO> listData)
        {
            string result = "0";

            var listGroup = listData.GroupBy(c => new
            {
                c.ProcessGroup,
                c.StatusEmp,
                c.ProductionDate
            }).Select(x => new ExeReportByStatusDTO()
            {
                ProcessGroup = x.Key.ProcessGroup,
                StatusEmp = x.Key.StatusEmp,
                ProductionDate = x.Key.ProductionDate,
                ActualWorkHour = x.Sum(c => c.ActualWorkHour),

            }).ToList();

            var ci = CultureInfo.CurrentCulture;

            if (listGroup.Count() > 1)
            {
                //sum the highest each date
                double totalHighActual = 0;
                foreach (var productionDate in listGroup.Select(c => c.ProductionDate).Distinct())
                {

                    var highActual = listGroup.Where(c => c.ProductionDate == productionDate).Max(x => x.ActualWorkHour);
                    if (highActual.HasValue)
                        totalHighActual += highActual.Value;
                }

                result = totalHighActual.ToString("0.##", ci);
            }
            else
            {
                //get the highest
                var highActual = listData.Max(c => c.ActualWorkHour);
                if (highActual.HasValue)
                    result = highActual.Value.ToString("0.##", ci);
            }

            return result;
        }

        public string GetTotalWorkHourWeekly(List<ExeReportByStatusWeeklyDTO> listData)
        {
            string result = "0";

            var listGroup = listData.GroupBy(c => new
            {
                c.ProcessGroup,
                c.StatusEmp,
                c.KPSWeek,
                c.KPSYear
            }).Select(x => new ExeReportByStatusWeeklyDTO()
            {
                ProcessGroup = x.Key.ProcessGroup,
                StatusEmp = x.Key.StatusEmp,
                KPSWeek = x.Key.KPSWeek,
                KPSYear = x.Key.KPSYear,
                ActualWorkHour = x.Sum(c => c.ActualWorkHour),

            }).ToList();

            var ci = CultureInfo.CurrentCulture;

            if (listGroup.Count() > 1)
            {
                //sum the highest each date
                double totalHighActual = 0;
                foreach (var weekly in listGroup.Select(c => new { c.KPSWeek, c.KPSYear }).Distinct())
                {

                    var highActual = listGroup.Where(c => c.KPSWeek == weekly.KPSWeek && c.KPSYear == weekly.KPSYear).Max(x => x.ActualWorkHour);
                    if (highActual.HasValue)
                        totalHighActual += highActual.Value;

                }

                result = totalHighActual.ToString("f2", ci);
            }
            else
            {
                //get the highest
                var highActual = listData.Max(c => c.ActualWorkHour);
                if (highActual.HasValue)
                    result = highActual.Value.ToString("f2", ci);
            }

            return result;
        }

        public string GetTotalWorkHourMonthly(List<ExeReportByStatusMonthlyDTO> listData)
        {
            string result = "0";

            var listGroup = listData.GroupBy(c => new
            {
                c.ProcessGroup,
                c.StatusEmp,
                c.Month,
                c.Year
            }).Select(x => new ExeReportByStatusMonthlyDTO()
            {
                ProcessGroup = x.Key.ProcessGroup,
                StatusEmp = x.Key.StatusEmp,
                Month = x.Key.Month,
                Year = x.Key.Year,
                ActualWorkHour = x.Sum(c => c.ActualWorkHour),

            }).ToList();

            //var ci = new CultureInfo("en-US");
            var ci = CultureInfo.CurrentCulture;

            if (listGroup.Count() > 1)
            {
                //sum the highest each date
                double totalHighActual = 0;
                foreach (var monthly in listGroup.Select(c => new { c.Month, c.Year }).Distinct())
                {

                    var highActual = listGroup.Where(c => c.Month == monthly.Month && c.Year == monthly.Year).Max(x => x.ActualWorkHour);
                    if (highActual.HasValue)
                        totalHighActual += highActual.Value;
                }

                result = totalHighActual.ToString("f2", ci);
            }
            else
            {
                //get the highest
                var highActual = listData.Max(c => c.ActualWorkHour);
                if (highActual.HasValue)
                    result = highActual.Value.ToString("f2", ci);
            }

            return result;
        }

        public string GetTotalActualWorkPerProcess(List<ExeReportByStatusDTO> listData)
        {
            string result = "0";

            double sumActualWorkHourWeek = 0;
            double sumActual = 0;

            foreach (var exeReportByStatusWeeklyDto in listData)
            {

                int actualWorker = exeReportByStatusWeeklyDto.ActualWorker.HasValue
                    ? exeReportByStatusWeeklyDto.ActualWorker.Value
                    : 0;
                double actualWorkHour = exeReportByStatusWeeklyDto.ActualWorkHour.HasValue
                    ? exeReportByStatusWeeklyDto.ActualWorkHour.Value
                    : 0;
                sumActualWorkHourWeek += (actualWorker * actualWorkHour);
                sumActual += actualWorker;
            }

            if (sumActual > 0)
            {
                return
                    Math.Round((sumActualWorkHourWeek / sumActual), 2).ToString("0.##", CultureInfo.CurrentCulture);
            }

            return result;
        }

        public string GetTotalProductionStickPerHour(List<ExeReportByStatusDTO> listData)
        {
            string result = "0";
            double sumProductionStick = 0;

            foreach (var item in listData)
            {
                double productionStick = item.PrdPerStk.HasValue ? item.PrdPerStk.Value : 0;
                sumProductionStick += productionStick;
            }

            if (sumProductionStick > 0)
            {
                return
                    Math.Round(sumProductionStick, 2).ToString("0.##", CultureInfo.CurrentCulture);
            }

            return result;
        }
        
        #region EMS Source Data
        public List<ExeEMSSourceDataDTO> GetReportEMSSourceData(GetExeEMSSourceDataInput input)
        {

            var dbResultEMSSourceData = _sqlSPRepo.GetReportEMSSourceDataDaily(input.LocationCode, input.BrandCode, input.DateFrom, input.DateTo);

            return Mapper.Map<List<ExeEMSSourceDataDTO>>(dbResultEMSSourceData);
        }
        #endregion

        public IEnumerable<string> GetUnitByProcessFilter(string locationCode) {
            var result = new List<string>();
            bool isPlant = false;

            using (SKTISEntities context = new SKTISEntities()) {
                result = context.ExeReportByProcesses.OrderBy(c => c.UnitCode).Where(c => c.LocationCode == locationCode).Select(c => c.UnitCode).Distinct().ToList();
                if (context.MstGenLocations.Where(c => c.LocationCode == locationCode).Select(c => c.ParentLocationCode).FirstOrDefault().ToString() == Enums.LocationCode.PLNT.ToString()) {
                    isPlant = true;
                }
            }

            if (isPlant) {
                if(result.Count > 1)
                    result.Insert(0, "All");
            }
                
            return result;
        }
    }
}
