using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BLL.ExecutionBLL
{
    public class ExeReportProdStockProcessBLL : IExeReportProdStockProcessBLL
    {
        private readonly IUnitOfWork _uow;
        private readonly IMasterDataBLL _masterDataBll;
        private readonly IGenericRepository<ExeReportByProcess> _exeReportByProcessRepo;
        private readonly IGenericRepository<ExeReportProdStockProcessView> _exeReportProdStockRepo;
        private readonly ISqlSPRepository _sqlSPRepo;

        public ExeReportProdStockProcessBLL
        (
            IUnitOfWork uow,
            IMasterDataBLL masterDataBll
        ) 
        {
            _uow = uow;
            _sqlSPRepo = _uow.GetSPRepository();
            _masterDataBll = masterDataBll;
            _exeReportByProcessRepo = _uow.GetGenericRepository<ExeReportByProcess>();
            _exeReportProdStockRepo = _uow.GetGenericRepository<ExeReportProdStockProcessView>();
        }

        public IEnumerable<string> GetUnitCodeList(string locationCode) 
        {
            // Get location from last child and parent
            var lastChildLocation = GetLastChildLocationCode(locationCode);

            // Get row data from ExeReportByProcess by location code
            var dbResult = _exeReportByProcessRepo.Get(c => lastChildLocation.Contains(c.LocationCode));
            
            // Get unit code list
            var unitCodeList = dbResult.Select(c => c.UnitCode).Distinct().ToList();

            // Insert option "All"
            if (unitCodeList.Count() > 1) unitCodeList.Insert(0, Enums.All.All.ToString());

            return unitCodeList;
        }

        public IEnumerable<ExeReportProdStockPerBrandGroupCodeDTO> GetExeReportProductionStock(GetExeProdStockInput input) 
        {
            var dateFrom = DateTime.Now.Date;
            var dateTo = DateTime.Now.Date;

            // Set up query filter date
            if (input.IsFilterAnnualy)
            {
                dateFrom = new DateTime(input.FilterYear.Value, 1, 1);
                dateTo = new DateTime(input.FilterYear.Value, 12, 31);
            }
            else if (input.IsFilterMonthly)
            {
                dateFrom = new DateTime(input.FilterYearMonthFrom.Value, input.FilterMonthFrom.Value, 1);
                dateTo = new DateTime(input.FilterYearMonthTo.Value, input.FilterMonthTo.Value, DateTime.DaysInMonth(input.FilterYearMonthTo.Value, input.FilterMonthTo.Value));
            }
            else if (input.IsFilterWeekly)
            {
                var weekFrom = _masterDataBll.GetWeekByYearAndWeek(input.FilterYearWeekFrom.Value, input.FilterWeekFrom.Value);
                var weekTo = _masterDataBll.GetWeekByYearAndWeek(input.FilterYearWeekFrom.Value, input.FilterYearWeekTo.Value);

                if (weekFrom != null)
                    if (weekFrom.StartDate != null) dateFrom = weekFrom.StartDate.Value;

                if (weekTo != null)
                    if (weekTo.EndDate != null) dateTo = weekTo.EndDate.Value;
            }
            else if (input.IsFilterDaily)
            {
                if (input.FilterDateFrom != null) dateFrom = input.FilterDateFrom.Value;
                if (input.FilterDateTo != null) dateTo = input.FilterDateTo.Value;
            }

            // Get database result
            //var dbResult = _exeReportProdStockRepo.Get(queryFilter).OrderBy(c => c.BrandGroupCode);
           
            // Mapper db object to DTO
            var resultDto = new List<ExeReportProductionStockDTO>();

           if(input.FilterUnitCode.Equals(Enums.All.All.ToString()))
           {
                var dbResultAllUnit =  _sqlSPRepo.GetReportProdStockProcessAllUnitView(input.FilterLocation, dateFrom, dateTo);
               resultDto = Mapper.Map<List<ExeReportProductionStockDTO>>(dbResultAllUnit);
           }
           else
           {
                var dbResult = _sqlSPRepo.GetReportProdStockProcessView(input.FilterLocation, input.FilterUnitCode, dateFrom, dateTo);
               resultDto = Mapper.Map<List<ExeReportProductionStockDTO>>(dbResult);
           }

           resultDto = resultDto
                        .GroupBy(c => new { c.BrandGroupCode, c.BrandCode, c.LocationCode })
                        .Select(g => new ExeReportProductionStockDTO { 
                            BrandGroupCode = g.Key.BrandGroupCode,
                            BrandCode = g.Key.BrandCode,
                            LocationCode = g.Key.LocationCode,
                            BeginStockInternalMove = g.Sum(c => c.BeginStockInternalMove),
                            BeginStockExternalMove = g.Sum(c => c.BeginStockExternalMove),
                            Production = Math.Round(g.Sum(c => c.Production), 2),
                            PAP = g.Sum(c => c.PAP),
                            PAG = g.Sum(c => c.PAG),
                            EndingStockExternalMove = g.Sum(c => c.EndingStockExternalMove),
                            EndingStockInternalMove = g.Sum(c => c.EndingStockInternalMove),
                            Planning = g.Sum(c => c.Planning),
                            VarianceStick = g.Sum(c => c.VarianceStick),
                            VariancePercent = g.Sum(c => c.Planning) == 0 ? 0 : (g.Sum(c => c.VarianceStick) / g.Sum(c => c.Planning)) * 100,
                        }).ToList();
                        
            // Grouping by Brand
            var groupByBrand = resultDto
                               .GroupBy(c => new { c.BrandGroupCode, c.BrandCode })
                               .Select(g => new ExeReportProdStockPerBrandDTO
                                    {
                                        BrandGroupCode = g.Key.BrandGroupCode,
                                        BrandGroup = g.Key.BrandCode,
                                        CountBrandGroup = g.Count(),
                                        TotalBeginStockInMovePerBrand = g.Sum(c => c.BeginStockInternalMove),
                                        TotalBeginStockExtMovePerBrand = g.Sum(c => c.BeginStockExternalMove),
                                        TotalProdPerBrand = g.Sum(c => c.Production),
                                        TotalPAPPerBrand = g.Sum(c => c.PAP),
                                        TotalPAGPerBrand = g.Sum(c => c.PAG),
                                        TotalEndStockInMovePerBrand = g.Sum(c => c.EndingStockInternalMove),
                                        TotalEndStockExtMovePerBrand = g.Sum(c => c.EndingStockInternalMove),
                                        TotalPlanningPerBrand = g.Sum(c => c.Planning),
                                        TotalVarStickPerBrand = g.Sum(c => c.VarianceStick),
                                        TotalVarStickPercentPerBrand = g.Sum(c => c.Planning) == 0 ? 0 : (g.Sum(c => c.VarianceStick) / g.Sum(c => c.Planning)) * 100,
                                        ListReportProdStock = g.ToList()
                                    });

            // Grouping by Brand Group Code
            var groupByBrandCode = groupByBrand
                                    .GroupBy(c => new { c.BrandGroupCode })
                                    .Select(g => new ExeReportProdStockPerBrandGroupCodeDTO
                                    {
                                       BrandGroupCode = g.Key.BrandGroupCode,
                                       CountBrandGroupCode = g.Sum(c => c.CountBrandGroup),
                                       TotalBeginStockInMovePerBrandGroupCode = g.Sum(c => c.TotalBeginStockInMovePerBrand),
                                       TotalBeginStockExtMovePerBrandGroupCode = g.Sum(c => c.TotalBeginStockExtMovePerBrand),
                                       TotalProdPerBrandGroupCode = g.Sum(c => c.TotalProdPerBrand),
                                       TotalPAPPerBrandGroupCode = g.Sum(c => c.TotalPAPPerBrand),
                                       TotalPAGPerBrandGroupCode = g.Sum(c => c.TotalPAGPerBrand),
                                       TotalEndStockInMovePerBrandGroupCode = g.Sum(c => c.TotalEndStockInMovePerBrand),
                                       TotalEndStockExtMovePerBrandGroupCode = g.Sum(c => c.TotalEndStockExtMovePerBrand),
                                       TotalPlanningPerBrandGroupCode = g.Sum(c => c.TotalPlanningPerBrand),
                                       TotalVarStickPerBrandGroupCode = g.Sum(c => c.TotalVarStickPerBrand),
                                       TotalVarStickPercentPerBrandGroupCode = g.Sum(c => c.TotalPlanningPerBrand) == 0 ? 0 : (g.Sum(c => c.TotalVarStickPerBrand) / g.Sum(c => c.TotalPlanningPerBrand)) * 100,
                                       ListReportProdStockPerBrand = g.ToList()
                                   });

            return groupByBrandCode;
        }

        private Expression<Func<ExeReportProdStockProcessView, bool>> SetQueryFilter(GetExeProdStockInput input) 
        { 
            // Set query filter ExeReportProdStockProcessView
            var queryFilter = PredicateHelper.True<ExeReportProdStockProcessView>();

            var lastChildLocation = GetLastChildLocationCode(input.FilterLocation);

            if (!String.IsNullOrEmpty(input.FilterLocation)) 
            {
                queryFilter = queryFilter.And(c => lastChildLocation.Contains(c.LocationCode));
            }

            if(!String.IsNullOrEmpty(input.FilterUnitCode))
            {
                if(input.FilterUnitCode.Equals(Enums.All.All.ToString()))
                {
                    var unitCodeList = GetUnitCodeList(input.FilterLocation);
                    queryFilter = queryFilter.And(c => unitCodeList.Contains(c.UnitCode));
                }
                else
                {
                    queryFilter = queryFilter.And(c => c.UnitCode == input.FilterUnitCode);
                }
            }

            if(input.IsFilterAnnualy)
            {
                queryFilter = queryFilter.And(c => c.KPSYear == input.FilterYear);
            }
            else if(input.IsFilterMonthly)
            {
                queryFilter = queryFilter.And(c => c.KPSYear >= input.FilterYearMonthFrom && c.KPSYear <= input.FilterYearMonthTo);
                queryFilter = queryFilter.And(c => c.ProductionDate.Month >= input.FilterMonthFrom && c.ProductionDate.Month <= input.FilterMonthTo);
            }
            else if(input.IsFilterWeekly)
            {
                queryFilter = queryFilter.And(c => c.KPSYear >= input.FilterYearWeekFrom && c.KPSYear <= input.FilterYearWeekTo);
                queryFilter = queryFilter.And(c => c.KPSWeek >= input.FilterWeekFrom && c.KPSWeek <= input.FilterWeekTo);
            }
            else if(input.IsFilterDaily)
            {
                queryFilter = queryFilter.And(c => DbFunctions.TruncateTime(c.ProductionDate) >= DbFunctions.TruncateTime(input.FilterDateFrom) && DbFunctions.TruncateTime(c.ProductionDate) <= DbFunctions.TruncateTime(input.FilterDateTo));
            }

            return queryFilter;
        }

        private IEnumerable<string> GetLastChildLocationCode(string location) 
        {
            // Get location from last child and parent
            var mstGenLocationDTO = _masterDataBll.GetLastChildLocation(location);
            var lastChildLocation = mstGenLocationDTO.Select(c => c.LocationCode).Distinct().ToList();

            return lastChildLocation;
        }
    }
}
