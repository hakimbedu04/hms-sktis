using System;
using System.Collections.Generic;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Core;

namespace HMS.SKTIS.Contracts
{
    public interface IPlanningBLL
    {
        #region WPP

        double ConvertBytesToMegabytes(long bytes);
        List<PlanWeeklyProductionPlanningDTO> GetPlanWeeklyProductionPlannings(PlanWeeklyProductionPlanningInput input);
        void DeleteAllTempWPP();
        void InsertTempWPP(PlanWeeklyProductionPlanningDTO wpp);
        void UpdateTempWPP(PlanWeeklyProductionPlanningDTO wpp);
        bool IsValidWPP();
        float GetTargetWPP(GetTargetWPPInput input);
        int GetStickPerBoxByBrandCode(string BrandCode);
        bool CheckLocationGroupShift(string locationCode, int kpsYear, int kpsWeek);
        #endregion

        #region Plant

        #region TPU
        //asd
        List<PlanTPUCompositeDTO> GetPlanTPUs(GetPlanTPUsInput input);
        PlanTPUDTO UpdatePlanTPU(PlanTPUDTO planTPU);

        TargetManualTPUDTO GetTargetManualTPU(GetTargetManualTPUInput input);
        List<PlanTPUDTO> CalculatePlantTPU(CalculatePlantTPUInput InputPlantTPU, float TargetWpp, bool SubmitStatus);
        List<PlanTPUDTO> CalculatePlantTPURevision(CalculatePlantTPUInput InputPlantTPU, float TargetWpp, bool SubmitStatus);
        void SendEmailSubmitPlantTpu(GetPlanTPUsInput input, string currUserName);
        void SendEmailSubmitPlantTpk(GetPlantTPKsInput input, string currUserName);
        #endregion

        #region TPK
        List<PlantTPKCompositeDTO> GetPlanningPlantTPK(GetPlantTPKsInput input);
        PlantTPKByProcessDTO SavePlantTPKByGroup(PlantTPKByProcessDTO plantTPKbyProcess);

        List<PlanPlantAllocationDTO> SavePlanPlantAllocations(List<PlanPlantAllocationDTO> planPlantAllocations);
        List<WorkerBrandAssigmentDTO> GetWorkerBrandAssignmentPlanningPlantTpk(GetPlanPlantAllocation filter);
        PlantTPKCalculateDTO CalculatePlantTPK(CalculatePlantTPKInput InputPlantTPK);
        ExePlantProductionEntryVerificationDTO GetPlanningPlantTPKByGroup(string group, string locationCode, string unit, string brand, int year, int week, DateTime? date, int shift);
        #endregion

        #region Individual Capacity
        List<PlanningPlantIndividualCapacityWorkHourDTO> GetPlanningPlantIndividualCapacityWorkHours(GetPlanningPlantIndividualCapacityWorkHourInput input);
        PlanningPlantIndividualCapacityWorkHourDTO SavePlanningPlantIndividualCapacityWorkHour(PlanningPlantIndividualCapacityWorkHourDTO ppcwhDto);

        List<ExePlantProductionEntryVerificationDTO> GeteExePlantProductionEntryVerification(
            GetExePlantProductionEntryVerificationInput input);

        List<ExePlantProductionEntryVerificationDTO> GeteExePlantProductionEntryVerificationBll(
            GetExePlantProductionEntryVerificationInput input);
        List<ExePlantProductionEntryVerificationDTO> GeteExePlantProductionEntryVerification(
            GetPlanningPlantIndividualCapacityByReferenceInput input);
        List<PlanningPlantIndividualCapacityByReferenceDTO> GetPlanningPlantIndividualCapacityByReference(GetPlanningPlantIndividualCapacityByReferenceInput input);

        List<PlanningPlantIndividualCapacityByReferenceDTO> GetPlanningPlantIndividualCapacityAverageByReference(
            List<PlanningPlantIndividualCapacityByReferenceDTO> icReferences);

        PlanningPlantIndividualCapacityWorkHourDTO SavePlanningPlantIndividualCapacityByReference(
            PlanningPlantIndividualCapacityWorkHourDTO input, string name);

        List<PlanningPlantIndividualCapacityWorkHourDTO> GetProsesGroupFromCapacityWorkHour(string locationCode,
            string unitCode, string brandGruopCode, string gruopCode);

        List<DataDailyProductionAchievmentDTO> GetBrandCodeFromReportDailyAchievment(
            GetExePlantProductionEntryVerificationInput input);

        List<string> GeteExePlantProductionEntryVerificationWithUnion(
            GetExePlantProductionEntryVerificationInput input);

        #endregion

        #region WIP Detail
        WIPStockDTO GetPlantWIPStock(GetPlantWIPStockInput input);
        #endregion
        #endregion

        #region Plant Group Shift

        List<PlanPlantGroupShiftDTO> GetGroupShift(GetPlanPlantGroupShiftInput input);
        List<PlanPlantGroupShiftDTO> GetPlanPlantGroupShifts(GetPlanPlantGroupShiftInput input);
        PlanPlantGroupShiftDTO SavePlanPlantGroupShift(PlanPlantGroupShiftDTO plantGroupShift, string username);

        #endregion

        #region TPO

        List<string> GetTPKCodeByLocations(GetTPOTPKInput input);
        List<string> GetTPOTPKProcessByLocations(string locationCode, int year, int week);

        List<string> GetAllProcessFromExeTPOProductionVerificationByLocationCodeAndDate(string locationDate, int year, int week, DateTime? date);
        List<PlanTPOTPKCompositeDTO> GetPlanningTPOTPK(GetTPOTPKInput input);
        void CalculateTPOTPKByTPKCode(string TPKCode);

        TPOTPKByProcessDTO SaveTPOTPKByGroup(TPOTPKByProcessDTO tpoTPKbyProcess);
        TPOTPKDTO SavePlanTPOTPK(TPOTPKDTO plantoptpk, bool? status, string userName);
        void SaveLatestPlanTPOTPK(List<TPOTPKDTO> readyTPOTPK, TPOTPKDTO plantoptpk, bool? status, string userName);
        List<TPOTPKDTO> GetLatestReadyToSubmitTPOTPK(string locationCode, List<string> processGroup, MstGenWeekDTO StartDate);
        PlanTPOTPKTotalBoxDTO SaveTPOTPKTotal(PlanTPOTPKTotalBoxDTO tpoTPKTotal);
        TPOTPKCalculateDTO CalculateTPOTPK(CalculateTPOTPKInput InputTPOTPK);
        PlanTPOTPKTotalBoxDTO GetTPOTPKInBox(PlanTPOTPKTotalBoxInput input);
        void SaveTPOTPKWorkHourInBox(PlanTPOTPKTotalBoxInput input, GenericValuePerWeekDTO<float> workHour);
        #endregion

        #region Report

        #region Summary Daily Production Group

        List<PlanningReportProductionTargetCompositeDTO> GetPlanningReportProductionTargets(GetPlanningReportProductionTargetInput input);

        #endregion

        List<PlanningReportSummaryProcessTargetsCompositeDTO> GetReportSummaryProcessTargets(GetPlanningReportSummaryProcessTargetsInput input);

        #endregion

        #region PlanTmpWeeklyProductionPlanning
        List<PlanTmpWeeklyProductionPlanningDTO> GetKPSPlanTempWeeklyProductionGroup();
        #endregion

        #region TPU

        List<PlanTPUDTO> GetpPlanTargetProductionUnits(GetTargetWPPInput input);
        float? CalculateInStock(List<PlanTPUDTO> input, int? stickPerBox);
        decimal CalculateTargetWPP(PlanWeeklyProductionPlanningInput input, bool Inbox, int stickPerBox);
        PlanTPUStatusDTO GetStatePlanTPU(string locationCode, string brandCode, int kpsYear, int kpsWeek, int shift);
        #endregion
         #region getprocessbyTPK
        List<string> GetPlantTPKProcessByLocations(string locationCode);
         #endregion

        #region Report Plan

        List<MstTableauReportDto> GetReportTableau(Enums.ReportTableau input);
        MstTableauReportDto GetReportTableau(int id);
        void SaveReport(MstTableauReportDto input);
        void DeleteReport(MstTableauReportDto input);
        #endregion

        void SubmitTpoTpk(string locationCode, string brandCode, int? kpsYear, int? kpsWeek, string userName);


        List<string> GetBrandCodeByLocationYearAndWeek(string locationCode, int? KPSYear, int? KPSWeek);
        List<string> GetBrandCodeByLocationYearAndWeekTPU(string locationCode, int? KPSYear, int? KPSWeek);

        List<string> GetBrandCodeTPKByLocationYearAndWeek(string locationCode, int? KPSYear, int? KPSWeek);
        List<string> GetBrandCodeTPUByLocationYearAndWeek(string location, int? KPSYear, int? KPSWeek);

        #region For  Report Available Position Number

        List<PlanPlantGroupShiftDTO> GetGroupShiftProcess(GetPlanPlantGroupShiftInput input);

        #endregion

        #region EMAILs
        void SendEmailSubmitTPOTPK(string locationCode, string brandCode, int kpsYear, int kpsWeek, string currUserName);
        #endregion
    }
}
