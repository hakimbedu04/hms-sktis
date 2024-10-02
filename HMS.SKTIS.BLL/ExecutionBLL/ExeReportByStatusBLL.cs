using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HMS.SKTIS.BLL.ExecutionBLL
{
    public class ExeReportByStatusBLL : IExeReportByStatusBLL
    {
        private readonly IUnitOfWork _uow;
        private readonly ISqlSPRepository _sqlSPRepo;
        private readonly IGenericRepository<MstGenLocation> _mstLocationRepo;
        private readonly IMasterDataBLL _masterDataBLL;

        public ExeReportByStatusBLL(IUnitOfWork uow, IMasterDataBLL masterDataBLL) {
            _uow = uow;
            _sqlSPRepo = _uow.GetSPRepository();
            _mstLocationRepo = _uow.GetGenericRepository<MstGenLocation>();
            _masterDataBLL = masterDataBLL;
        }

        #region FILTER DROPDOWN

        public IEnumerable<string> GetAllUnits(string locationCode, bool responsibilities = true) {
            var result = new List<string>();
            
            using (SKTISEntities context = new SKTISEntities()) {
                result.AddRange(context.ExeReportByGroups
                    .Where(c => c.LocationCode == locationCode)
                    .Select(c => c.UnitCode).Distinct());
            }

            result = result.OrderBy(c => c).ToList();

            if (GetParentLocation(locationCode) == Enums.LocationCode.PLNT.ToString()) {
                if (result.Count > 1)
                    result.Insert(0, "All");
            }

            return result;
        }

        public IEnumerable<int> GetShiftByLocationCode(string parentLocationCode) {
            var shiftList = new List<int>();

            if (parentLocationCode == Enums.LocationCode.PLNT.ToString() || parentLocationCode == Enums.LocationCode.TPO.ToString() ||
                parentLocationCode == Enums.LocationCode.SKT.ToString() || parentLocationCode == Enums.LocationCode.REG1.ToString() ||
                parentLocationCode == Enums.LocationCode.REG2.ToString() || parentLocationCode == Enums.LocationCode.REG3.ToString() ||
                parentLocationCode == Enums.LocationCode.REG4.ToString()) {
                using (SKTISEntities context = new SKTISEntities()) {
                    shiftList.AddRange(context.ExeReportByGroups.OrderBy(c => c.Shift).Select(c => c.Shift).Distinct());
                }
            }
            else {
                using (SKTISEntities context = new SKTISEntities()) {
                    shiftList.AddRange(context.ExeReportByGroups.Where(c => c.LocationCode == parentLocationCode).OrderBy(c => c.Shift).Select(c => c.Shift).Distinct());
                }
            }
            return shiftList;
        }

        public IEnumerable<string> GetActiveBrandGroupCode(GetExeReportByStatusInput inputFilter) {
            IEnumerable<string> listChildLocationCode = GetLastChildLocation(inputFilter.LocationCode).Select(c => c.LocationCode).Distinct();
            var result = new List<string>();

            Tuple<DateTime, DateTime> rangeDate = GetProdDateFromAndTo(inputFilter);
            var productionDateFrom = rangeDate.Item1;
            var productionDateTo = rangeDate.Item2;

            using (SKTISEntities context = new SKTISEntities()) {
                if (inputFilter.UnitCode != "All") {
                    result.AddRange(context.ExeReportByGroups
                                .Where(c => listChildLocationCode.Contains(c.LocationCode) && c.UnitCode == inputFilter.UnitCode && c.ProductionDate >= productionDateFrom && c.ProductionDate <= productionDateTo)
                                .Select(c => c.BrandGroupCode).Distinct());
                }
                else {
                    result.AddRange(context.ExeReportByGroups
                                .Where(c => listChildLocationCode.Contains(c.LocationCode) && c.ProductionDate >= productionDateFrom && c.ProductionDate <= productionDateTo)
                                .Select(c => c.BrandGroupCode).Distinct());
                }
            }

            return result.OrderBy(c => c).ToList();
        }

        public IEnumerable<string> GetBrandCode(GetExeReportByStatusInput inputFilter) {
            //var listBrandCode = _wppBrandGroupViewRepo.Get(c => c.BrandGroupCode == brandGroupCode).OrderBy(c => c.BrandCode).Select(c => c.BrandCode).Distinct();
            //return listBrandCode;
            IEnumerable<string> listChildLocationCode = GetLastChildLocation(inputFilter.LocationCode).Select(c => c.LocationCode).Distinct();
            var result = new List<string>();

            Tuple<DateTime, DateTime> rangeDate = GetProdDateFromAndTo(inputFilter);
            var productionDateFrom = rangeDate.Item1;
            var productionDateTo = rangeDate.Item2;

            using (SKTISEntities context = new SKTISEntities()) {
                if (inputFilter.UnitCode != "All") {
                    result.AddRange(context.ExeReportByGroups
                                .Where(c => listChildLocationCode.Contains(c.LocationCode) && c.UnitCode == inputFilter.UnitCode && c.BrandGroupCode == inputFilter.BrandGroupCode && c.ProductionDate >= productionDateFrom && c.ProductionDate <= productionDateTo)
                                .Select(c => c.BrandCode).Distinct());
                }
                else {
                    result.AddRange(context.ExeReportByGroups
                                .Where(c => listChildLocationCode.Contains(c.LocationCode) && c.BrandGroupCode == inputFilter.BrandGroupCode && c.ProductionDate >= productionDateFrom && c.ProductionDate <= productionDateTo)
                                .Select(c => c.BrandCode).Distinct());
                }
            }

            return result.OrderBy(c => c).ToList();
        }

        private IEnumerable<MstGenLocationDTO> GetLastChildLocation(string parentCode) {
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var location = strUserID.Location.Select(s => s.Code).ToList();
            var locations = _sqlSPRepo.GetLastChildLocationByLocationCode(parentCode).Select(m => m.LocationCode);
            var dblocation = _mstLocationRepo.Get(m => locations.Contains(m.LocationCode) && location.Contains(m.LocationCode));
            return Mapper.Map(dblocation, new List<MstGenLocationDTO>());
        }

        private string GetParentLocation(string childLocation) {
            string result = string.Empty;
            using (SKTISEntities context = new SKTISEntities()) {
                result = context.MstGenLocations.Where(c => c.LocationCode == childLocation).Select(c => c.ParentLocationCode).FirstOrDefault().ToString();
            }
            return result;
        }

        private Tuple<DateTime, DateTime> GetProdDateFromAndTo(GetExeReportByStatusInput inputFilter) {
            // Get <productionDateFrom, productionDateTo>
            var productionDateFrom = DateTime.Now.Date;
            var productionDateTo = DateTime.Now.Date;

            if (inputFilter.FilterType == "Year") {
                productionDateFrom = new DateTime(inputFilter.YearFrom.Value, 1, 1);
                productionDateTo = new DateTime(inputFilter.YearTo.Value, 12, 31);
            }
            else if (inputFilter.FilterType == "YearMonth") {
                productionDateFrom = new DateTime(inputFilter.YearFrom.Value, inputFilter.MonthFrom.Value, 1);
                var daysInMonth = DateTime.DaysInMonth(inputFilter.YearTo.Value, inputFilter.MonthTo.Value);
                productionDateTo = new DateTime(inputFilter.YearTo.Value, inputFilter.MonthTo.Value, daysInMonth);
            }
            else if (inputFilter.FilterType == "YearWeek") {
                using (SKTISEntities context = new SKTISEntities()) {
                    productionDateFrom = context.MstGenWeeks.Where(c => c.Year == inputFilter.YearFrom.Value && c.Week == inputFilter.WeekFrom.Value).Select(c => c.StartDate.Value).FirstOrDefault();
                    productionDateTo = context.MstGenWeeks.Where(c => c.Year == inputFilter.YearTo.Value && c.Week == inputFilter.WeekTo.Value).Select(c => c.EndDate.Value).FirstOrDefault();
                }
            }
            else {
                productionDateFrom = inputFilter.DateFrom.Value;
                productionDateTo = inputFilter.DateTo.Value;
            }

            // Return <productionDateFrom, productionDateTo>
            return new Tuple<DateTime, DateTime>(productionDateFrom, productionDateTo);
        }
        #endregion

        #region GET DATA
        public IEnumerable<GetReportByStatusCompositeDTO> GetReportByStatusFunc(GetExeReportByStatusInput input) {
            Tuple<DateTime, DateTime> rangeDate = GetProdDateFromAndTo(input);
            var productionDateFrom = rangeDate.Item1;
            var productionDateTo = rangeDate.Item2;

            var dbResult = _sqlSPRepo.getReportByStatusFunc(input.LocationCode, input.UnitCode, input.Shift.Value, input.BrandGroupCode, input.BrandCode, productionDateFrom, productionDateTo).ToList();

            decimal totalWorkHour = 0m;
            using(SKTISEntities context = new SKTISEntities())
            {
                var mstStatusEmp = context.MstGenEmpStatus.Select(c => new {StatusEmp = c.StatusEmp, StatusIdentifier = c.StatusIdentifier}).ToList();
                dbResult = (from c in dbResult
                            join o in mstStatusEmp on c.StatusEmp equals o.StatusEmp
                            orderby o.StatusIdentifier ascending
                            select new GetReportByStatus_Result {
                                ProcessGroup = c.ProcessGroup,
                                StatusEmp = c.StatusEmp,
                                ActualWorker = c.ActualWorker,
                                ActualAbsWorker = c.ActualAbsWorker,
                                ActualWorkHourPerDay = c.ActualWorkHourPerDay,
                                ProductionStick = c.ProductionStick,
                                StickHourPeople = c.StickHourPeople,
                                StickHour = c.StickHour
                            }).ToList();

                var getTotalWorkHour = context.GetReportByStatusTotalWorkHour(input.LocationCode, input.UnitCode, input.Shift, input.BrandGroupCode, input.BrandCode, productionDateFrom, productionDateTo).FirstOrDefault();
                if (getTotalWorkHour != null) totalWorkHour = getTotalWorkHour.TotalWorkHour.HasValue ? getTotalWorkHour.TotalWorkHour.Value : 0;
            }

            var listProcess = _masterDataBLL.GetAllActiveMasterProcess()
                    .OrderBy(p => p.ProcessOrder).Select(c => c.ProcessGroup).Distinct();

            var result = new List<GetReportByStatusCompositeDTO>();

            foreach (var process in listProcess) {
                var composite = new GetReportByStatusCompositeDTO();
                if (dbResult.Any(c => c.ProcessGroup == process)) {
                    var dataPerProcess = dbResult.Where(c => c.ProcessGroup == process).ToList();
                    composite.ProcessGroup = process;
                    composite.listProcess = dataPerProcess;
                    composite.TotalActual = dataPerProcess.Sum(c => c.ActualWorker.HasValue ? c.ActualWorker.Value : 0);
                    composite.TotalAbsen = dataPerProcess.Sum(c => c.ActualAbsWorker.HasValue ? c.ActualAbsWorker.Value : 0);
                    //composite.TotalWorkHourPerDay = dataPerProcess.Average(c => c.ActualWorkHourPerDay.HasValue ? c.ActualWorkHourPerDay.Value : 0);
                    composite.TotalProductionStick = dataPerProcess.Sum(c => c.ProductionStick.HasValue ? c.ProductionStick.Value : 0);
                    //composite.TotalStickHourPeople = dataPerProcess.Average(c => c.StickHourPeople.HasValue ? c.StickHourPeople.Value : 0);
                    //composite.TotalStickHour = dataPerProcess.Sum(c => c.StickHour.HasValue ? c.StickHour.Value : 0);
                    composite.TotalWorkHour = totalWorkHour;

                    var actualXworkHourday = 0m;
                    foreach (var item in composite.listProcess) {
                        actualXworkHourday += Math.Round((item.ActualWorker.Value * (item.ActualWorkHourPerDay.HasValue ? item.ActualWorkHourPerDay.Value : 0)), 2);
                    }

                    composite.TotalWorkHourPerDay = composite.TotalActual == 0 ? 0m : Math.Round((actualXworkHourday / composite.TotalActual), 2);
                    composite.TotalStickHour = composite.TotalWorkHourPerDay == 0 ? 0m : (decimal)Math.Round((decimal)(composite.TotalProductionStick / composite.TotalWorkHourPerDay), 2);
                    composite.TotalStickHourPeople = composite.TotalActual == 0 ? 0m : (decimal)Math.Round((decimal)(composite.TotalStickHour / composite.TotalActual), 2);

                    result.Add(composite);
                }
            }

            var maxTotalStickHour = result.Any() ? result.Max(c => c.TotalStickHour) : 0;
            foreach (var item in result) {
                item.TotalBalanceIndex = maxTotalStickHour == 0 ? 0 : item.TotalStickHour / maxTotalStickHour;
            }

            return result;
        }
        #endregion
    }
}
