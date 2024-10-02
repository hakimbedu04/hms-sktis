using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BLL.ExecutionBLL
{
    public partial class ExecutionPlantBLL
    {
        private string PercentAttendanceColumn = "PercentAttendance";

        public GetLocationByResponsibilityViewDTO GetLocation(string userAd)
        {
            var dbResult = _getLocationByResponsibilityViewRepo.Get(p => p.UserAD == userAd).FirstOrDefault();

            return Mapper.Map<GetLocationByResponsibilityViewDTO>(dbResult);
        }

        public List<ExePlantWorkerBalancingProcessPerUnitDTO> GetWorkerBalancingProcessPerUnit(string locationCode, string unitCode, int? shift, string brandCode)
        {
            var listBalanceProcessPerUnit = new List<ExePlantWorkerBalancingProcessPerUnitDTO>();
            
            var listProcess = GetProcessGroupSelectListByLocationBrandCode(locationCode, brandCode).ToList();
            
            if (listProcess.Any())
            {
                var biggestValue = listProcess.Select(c => c.StdStickPerHour).Max();
                var totalValueProcess = GetTotalValueProcess(listProcess, biggestValue);
                foreach (var process in listProcess)
                {
                    var listGroup = GetListGroup(locationCode, unitCode, process.ProcessGroup).ToList();
                    var listBalanceProcessPerGroup = new List<ExePlantWorkerBalancingProcessPerGroupDTO>();
                    var balanceProcessPerUnit = new ExePlantWorkerBalancingProcessPerUnitDTO
                    {
                        Process = process.ProcessGroup,
                        AttendAllProcess = GetAttendAllProcess(locationCode, unitCode, shift, brandCode),
                        Attend = GetAttendPerProcess(locationCode, unitCode, shift, brandCode, process.ProcessGroup),
                        AfterBalancing = GetAfterBalancing(locationCode, unitCode, shift, brandCode, listGroup.ToList(), process.ProcessGroup)
                    };
                    //balanceProcessPerUnit.AfterBalancing =
                    //    GetAfterBalancing(locationCode, unitCode, shift, brandCode, listGroup.ToList(),
                    //        process.ProcessGroup) + balanceProcessPerUnit.Attend;
                    balanceProcessPerUnit.Recommendation = GetRecommendProcessPerUnit(biggestValue, totalValueProcess, balanceProcessPerUnit.AttendAllProcess, process);

                    foreach (var group in listGroup)
                    {
                        var balanceProcessPerGroup = new ExePlantWorkerBalancingProcessPerGroupDTO
                        {
                            GroupCode = group,
                            Attend = GetAttendPerGroup(locationCode, unitCode, shift, brandCode, process.ProcessGroup, group),
                            AfterBalancingPerGroup = GetAfterBalancingPerGroup(locationCode, unitCode, shift, brandCode, group, process.ProcessGroup)
                        };
                        balanceProcessPerGroup.IsBalance = balanceProcessPerGroup.Attend >= balanceProcessPerGroup.AfterBalancingPerGroup;//GetCurrentAttendPlantTPK(plantTpkInput);

                        listBalanceProcessPerGroup.Add(balanceProcessPerGroup);
                    }
                    balanceProcessPerUnit.ListBalancingProcessPerGroup = listBalanceProcessPerGroup;
                    balanceProcessPerUnit.AfterBalancing = listBalanceProcessPerGroup.Sum(c => c.AfterBalancingPerGroup);
                    listBalanceProcessPerUnit.Add(balanceProcessPerUnit);
                   
                }
            }

            return listBalanceProcessPerUnit;
        }

        private int GetAfterBalancingPerGroup(string locationCode, string unitCode, int? shift, string brandCode, string groupCode, string processGroup)
        {
            var dateNow = DateTime.Now.Date;

            var queryFilter = PredicateHelper.True<ExePlantProductionEntry>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.ExePlantProductionEntryVerification.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(unitCode))
                queryFilter = queryFilter.And(m => m.ExePlantProductionEntryVerification.UnitCode == unitCode);
            if (!string.IsNullOrEmpty(brandCode))
                queryFilter = queryFilter.And(m => m.ExePlantProductionEntryVerification.BrandCode == brandCode);
            if (!string.IsNullOrEmpty(processGroup))
                queryFilter = queryFilter.And(m => m.ExePlantProductionEntryVerification.ProcessGroup == processGroup);
            if (shift != null)
                queryFilter = queryFilter.And(m => m.ExePlantProductionEntryVerification.Shift == shift.Value);

            queryFilter = queryFilter.And(m => m.ExePlantProductionEntryVerification.ProductionDate == dateNow);

            var listProductionEntry = _exePlantProductionEntryRepo.Get(queryFilter).ToList();

            var countEmployee = listProductionEntry.Where(c => c.ExePlantProductionEntryVerification.GroupCode == groupCode).Select(c => c.EmployeeID).Distinct().Count();

            var countEmployeMO = listProductionEntry.Where(c => c.AbsentType == EnumHelper.GetDescription(Enums.SKTAbsentCode.MO) && c.ExePlantProductionEntryVerification.GroupCode == groupCode).Select(c => c.EmployeeID).Distinct().Count();

            var groupCodeDummy = groupCode.Remove(1, 1).Insert(1, "5");
            var listProductionEntryDummy = listProductionEntry.Where(c => c.ExePlantProductionEntryVerification.GroupCode == groupCodeDummy).ToList();

            var countEmployeeDummy = listProductionEntryDummy.Select(c => c.EmployeeID).Distinct().Count();

            return countEmployee - countEmployeMO + countEmployeeDummy;
        }

        private int GetAfterBalancing(string locationCode, string unitCode, int? shift, string brandCode, ICollection<string> groupCode, string process)
        {
            var attendProcess = 0;
            var dateNow = DateTime.Now.Date;

            var queryFilter = PredicateHelper.True<ExePlantWorkerAssignment>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.DestinationLocationCode == locationCode);
            if (!string.IsNullOrEmpty(unitCode))
                queryFilter = queryFilter.And(m => m.DestinationUnitCode == unitCode);
            if (!string.IsNullOrEmpty(brandCode))
                queryFilter = queryFilter.And(m => m.DestinationBrandCode == brandCode);
            if (!string.IsNullOrEmpty(process))
                queryFilter = queryFilter.And(m => m.DestinationProcessGroup == process);
            if (groupCode.Any())
                queryFilter = queryFilter.And(m => groupCode.Contains(m.DestinationGroupCode));
            if (shift != null)
                queryFilter = queryFilter.And(m => m.DestinationShift == shift.Value);

            queryFilter = queryFilter.And(m => dateNow >= m.StartDate && dateNow <= m.EndDate);

            var countEmployeeID = _exePlantWorkerAssignmentRepo.Get(queryFilter).Distinct().Select(c => c.EmployeeID).Count();

            return countEmployeeID;
        }

        private int GetCurrentAttendPlantTPK(GetPlantTPKsInput input)
        {
            var attendProcess = 0;
            var queryFilterVerification = PredicateHelper.True<PlanPlantTargetProductionKelompok>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilterVerification = queryFilterVerification.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilterVerification = queryFilterVerification.And(m => m.UnitCode == input.UnitCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilterVerification = queryFilterVerification.And(m => m.BrandCode == input.BrandCode);
            if (!string.IsNullOrEmpty(input.ProcessGroup))
                queryFilterVerification = queryFilterVerification.And(m => m.ProcessGroup == input.ProcessGroup);
            if (!string.IsNullOrEmpty(input.GroupCode))
                queryFilterVerification = queryFilterVerification.And(m => m.ProcessGroup == input.GroupCode);
            if (input.Shift != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == input.Shift.Value);
            if (input.KPSWeek != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == input.KPSWeek.Value);
            if (input.KPSYear != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == input.KPSYear.Value);

            var dayNow = (int)DateTime.Now.DayOfWeek == 0 ? 7 : (int)DateTime.Now.DayOfWeek;
            PercentAttendanceColumn += dayNow;

            attendProcess = Convert.ToInt32(_planPlantTargetProductionKelompokRepo.Get(queryFilterVerification).Select(c => PercentAttendanceColumn).FirstOrDefault());

            return attendProcess;
        }

        private int GetTPKAllocation(GetPlantTPKsInput input)
        {
            var attendProcess = 0;
            var queryFilterVerification = PredicateHelper.True<PlanPlantTargetProductionKelompok>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilterVerification = queryFilterVerification.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilterVerification = queryFilterVerification.And(m => m.UnitCode == input.UnitCode);
            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilterVerification = queryFilterVerification.And(m => m.BrandCode == input.BrandCode);
            if (!string.IsNullOrEmpty(input.ProcessGroup))
                queryFilterVerification = queryFilterVerification.And(m => m.ProcessGroup == input.ProcessGroup);
            if (!string.IsNullOrEmpty(input.GroupCode))
                queryFilterVerification = queryFilterVerification.And(m => m.ProcessGroup == input.GroupCode);
            if (input.Shift != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == input.Shift.Value);
            if (input.KPSWeek != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == input.KPSWeek.Value);
            if (input.KPSYear != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == input.KPSYear.Value);

            var dbPlantTpk = _planPlantTargetProductionKelompokRepo.Get(queryFilterVerification).FirstOrDefault();

            return dbPlantTpk == null ? 0 : dbPlantTpk.WorkerAllocation == null ? 0 : dbPlantTpk.WorkerAllocation.Value;
        }

        private IEnumerable<string> GetListGroup(string locationCode, string unitCode, string processGroup) {
            var dbPlantProductionGroup = _masterDataBll.GetMasterPlantProductionGroups(new GetMstPlantProductionGroupsInput
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                ProcessSettingsCode = processGroup
            });
            return dbPlantProductionGroup.Select(c => c.GroupCode).Distinct();
        }

        private int GetAttendPerProcess(string locationCode, string unitCode, int? shift, string brandCode, string process)
        {
            var attendProcess = 0;
            var queryFilterVerification = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilterVerification = queryFilterVerification.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(unitCode))
                queryFilterVerification = queryFilterVerification.And(m => m.UnitCode == unitCode);
            if (!string.IsNullOrEmpty(brandCode))
                queryFilterVerification = queryFilterVerification.And(m => m.BrandCode == brandCode);
            if (!string.IsNullOrEmpty(process))
                queryFilterVerification = queryFilterVerification.And(m => m.ProcessGroup == process);
            if (shift != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == shift.Value);

            var dateTimeNow = DateTime.Now.Date;
            queryFilterVerification = queryFilterVerification.And(m => m.ProductionDate == dateTimeNow);

            var prodEntryVerification = _exePlantProductionEntryVerificationRepo.Get(queryFilterVerification).ToList();

            if (prodEntryVerification.Any())
            {
                foreach (var entryVerification in prodEntryVerification)
                {
                    var countEntry = entryVerification.ExePlantProductionEntries.Where(m => m.ProductionEntryCode == entryVerification.ProductionEntryCode
                            && (m.ProdActual != 0 || m.ProdActual != null)
                            && m.AbsentType == null).Count();
                    attendProcess += countEntry;
                }
                return attendProcess;
            }
            return attendProcess;
        }

        private int GetAttendAllProcess(string locationCode, string unitCode, int? shift, string brandCode)
        {
            var attendProcess = 0;
            var queryFilterVerification = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilterVerification = queryFilterVerification.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(unitCode))
                queryFilterVerification = queryFilterVerification.And(m => m.UnitCode == unitCode);
            if (!string.IsNullOrEmpty(brandCode))
                queryFilterVerification = queryFilterVerification.And(m => m.BrandCode == brandCode);
            if (shift != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == shift.Value);

            var dateTimeNow = DateTime.Now.Date;
            queryFilterVerification = queryFilterVerification.And(m => m.ProductionDate == dateTimeNow);

            var prodEntryVerification = _exePlantProductionEntryVerificationRepo.Get(queryFilterVerification).ToList();

            if (prodEntryVerification.Any())
            {
                foreach (var entryVerification in prodEntryVerification)
                {
                    var countEntry = entryVerification.ExePlantProductionEntries.Where(m => m.ProductionEntryCode == entryVerification.ProductionEntryCode
                            && (m.ProdActual != 0 || m.ProdActual != null)
                            && m.AbsentType == null).Count();
                    attendProcess += countEntry;
                }
                return attendProcess;
            }
            return attendProcess;
        }

        private int GetAttendPerGroup(string locationCode, string unitCode, int? shift, string brandCode, string process, string groupCode)
        {
            var attendProcess = 0;
            var dateNow = DateTime.Now.Date;
            var queryFilterVerification = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilterVerification = queryFilterVerification.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(unitCode))
                queryFilterVerification = queryFilterVerification.And(m => m.UnitCode == unitCode);
            if (!string.IsNullOrEmpty(brandCode))
                queryFilterVerification = queryFilterVerification.And(m => m.BrandCode == brandCode);
            if (!string.IsNullOrEmpty(process))
                queryFilterVerification = queryFilterVerification.And(m => m.ProcessGroup == process);
            if (!string.IsNullOrEmpty(groupCode))
                queryFilterVerification = queryFilterVerification.And(m => m.GroupCode == groupCode);
            if (shift != null)
                queryFilterVerification = queryFilterVerification.And(m => m.Shift == shift.Value);

            queryFilterVerification = queryFilterVerification.And(m => m.KPSYear == dateNow.Year);

            var kpsWeek = _masterDataBll.GetGeneralWeekWeekByDate(dateNow);

            queryFilterVerification = queryFilterVerification.And(m => m.KPSWeek == kpsWeek);
            queryFilterVerification = queryFilterVerification.And(m => m.ProductionDate == dateNow);


            var prodEntryVerification = _exePlantProductionEntryVerificationRepo.Get(queryFilterVerification).ToList();

            if (prodEntryVerification.Any())
            {
                foreach (var entryVerification in prodEntryVerification)
                {
                    attendProcess +=
                        _exePlantProductionEntryRepo.Get(m => m.ProductionEntryCode == entryVerification.ProductionEntryCode
                            && m.ProdActual != 0
                            && (m.AbsentType == null || m.AbsentType == "")).Count();
                }
                return attendProcess;
            }
            return attendProcess;
        }

        private decimal GetTotalValueProcess(IEnumerable<MstGenProcessSettingCompositeDTO> listProcess, decimal biggestValue)
        {
            var totalValue = 0m;
            foreach (var process in listProcess)
            {
                totalValue += biggestValue / process.StdStickPerHour;
            }
            return totalValue;
        }

        private int GetRecommendProcessPerUnit(decimal biggestValue, decimal totalValues, int attendAllProcess, MstGenProcessSettingCompositeDTO process)
        {
            var recommendationProcess = 0m;
            var calculateByBiggestValue = 0m;
            if (process.StdStickPerHour == 0)
                calculateByBiggestValue = 0;
            else
                calculateByBiggestValue = biggestValue / process.StdStickPerHour;

            if (totalValues == 0)
                recommendationProcess = 0;
            else
                recommendationProcess = Math.Round((calculateByBiggestValue / totalValues) * attendAllProcess);

            return (int)recommendationProcess;
        }

        private IEnumerable<MstGenProcessSettingCompositeDTO> GetProcessGroupSelectListByLocationBrandCode(string locationCode, string brandCode)
        {
            //get BrandGroupCode by brandCode
            var mstGrnBrand = _masterDataBll.GetMstGenByBrandCode(brandCode);
            var brandGroupCode = mstGrnBrand == null ? "" : mstGrnBrand.BrandGroupCode;
            var input = new GetAllProcessSettingsInput()
            {
                LocationCode = locationCode
            };

            var listMasterProcessSetting = _masterDataBll.GetMasterProcessSettingByLocationCode(locationCode);
            var listProcessGroup = listMasterProcessSetting.Where(c => c.BrandGroupCode == brandGroupCode);
            return listProcessGroup.ToList();
        }
    }
}
