using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.Outputs.Execution;

namespace HMS.SKTIS.Contracts
{
    public interface IExeReportBLL
    {
        #region Production Report by Group
        List<ExeReportByGroupDTO> GetReportByGroups(GetExeReportByGroupInput input);
        void DeleteReportByGroups(string locationCode, string groupCode, string brandCode, DateTime productionDate);
     
        List<string> GetProcessListFilter(GetExeReportByGroupInput input);
        List<string> GetBrandGroupCodeFilter(GetExeReportByGroupInput input);
    
        #endregion

        #region Production Report by Status
        List<ExeReportByStatusDTO> GetReportByStatus(GetExeReportByStatusInput input);
        //List<ExeReportByStatusDTO> GetReportByStatusWeekly(GetExeReportByStatusInput input);
        //List<ExeReportByStatusDTO> GetReportByStatusMonthly(GetExeReportByStatusInput input);
        List<string> GetActiveBrandCode(string locationCode, string brandGroupCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType);
        List<string> GetActiveBrandGroupCode(string locationCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType);

            #endregion

        #region Production Report by Process
        List<ExeReportByProcessViewDTO> GetReportByProcess(GetExeReportByProcessInput input);
        IEnumerable<string> GetActiveBrandGroupCodeReportByProcess(string locationCode, string unitCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType);
        List<string> GetActiveBrandCodeReportByProcess(string location, string unitCode, string brandGroupCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string FromDate, string ToDate, string FilterType);
        void UpdateEndingStockByProcess(string locationCode, string unitCode, string brandCode, DateTime productionDate);
        List<ExeReportByProcessViewDTO> GetReportByProcessGenerator(GetExeReportByProcessInput input);
        #endregion

        #region reports Daily Production Achievmenet

        List<ExeReportDailyProductionAchievementViewDTO> GetReportByDaily(GetExeReportDailyProductionInput input);
        List<ExeReportingDailyProductionAchievementDTOSKTBrandCode> GetExeReportProductionDailyAchievement(GetExeReportDailyProductionInput input);
        #endregion

        void DeleteReportByProcess(string locationCode, string unitCode, string processGroup, string brandCode, DateTime productionDate);

        List<ExeReportByStatusActualWorkHourOutput> GetActualWorkWeekly(List<ExeReportByStatusWeeklyDTO> listData);

        List<ExeReportByStatusActualWorkHourOutput> GetActualWorkMonthly(List<ExeReportByStatusMonthlyDTO> listData);

        List<ExeReportByStatusActualWorkHourOutput> GetActualWork(List<ExeReportByStatusDTO> listData);

        string GetTotalWorkHour(List<ExeReportByStatusDTO> listData);

        string GetTotalWorkHourWeekly(List<ExeReportByStatusWeeklyDTO> listData);

        string GetTotalWorkHourMonthly(List<ExeReportByStatusMonthlyDTO> listData);

        string GetTotalActualWorkPerProcess(List<ExeReportByStatusDTO> listData);

        string GetTotalProductionStickPerHour(List<ExeReportByStatusDTO> listData);

        #region EMS Source Data
        List<ExeEMSSourceDataDTO> GetReportEMSSourceData(GetExeEMSSourceDataInput input);
        #endregion

        IEnumerable<string> GetUnitByProcessFilter(string locationCode);
    }
}
