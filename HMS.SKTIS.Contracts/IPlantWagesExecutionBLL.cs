using System.Collections.Generic;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using System;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace HMS.SKTIS.Contracts
{
    public interface IPlantWagesExecutionBLL
    {
        #region Production Card
        List<ProductionCardDTO> GetProductionCards(GetProductionCardInput input);
        List<ProductionCardCalculateTotalPayotherDTO> GetProductionCardsGroupAll(GetProductionCardInputGroupAll input);
        List<ProductionCardDTO> GetProductionCardsBrandGroupCode(GetProductionCardInput input);
        List<ProductionCardDTO> GetProcessGroupProductionCards(GetProductionCardInput input);
        List<ProductionCardDTO> GetProductionCardsCorrection(GetProductionCardInput input);
        ProductionCardDTO SaveProductionCard(ProductionCardDTO productionCardDto);
        void UpdateSuratPeriodeLalu(string employeeId, string productionDate, string locationCode, string unitCode, int shift, string groupCode, string processGroup, string brandCode, int revisionType, string Remark,List<SuratPeriodeLaluInput> SuratPeriodeLalu);
        ProductionCardDTO InsertProductionCard(ProductionCardDTO cardDto);
        ProductionCardDTO GetProductionCardById(ProductionCardDTO cardDto);
        ProductionCardDTO GetProductionCard(ProductionCardDTO cardDto);
        void DeleteProductionCardsByReturnPlantVerification(GetProductionCardInput input);
        void DeleteProdCardByReturnVerificationRevType(GetProductionCardInput input, string productionEntryCode, string userName);
        void sendMail(GetProductionCardInput input, string username, bool isSubmit);

        IEnumerable<RoleButtonWagesApprovalDetail_Result> GetButtonState(ButtonStateInput input);
        #endregion

        #region Production Card Approval
        List<WagesProductionCardApprovalCompositeDTO> GetProductionCardApprovalList(GetProductionCardApprovalListInput input);
        List<WagesProductionCardApprovalCompositeDTO> GetProductionCardApproval(string locationCode, string unitCode, int revisionType);
        List<WagesProductionCardApprovalDetailViewDTO> GetProductionCardApprovalDetail(GetProductionCardApprovalDetailInput input);
        List<WagesProductionCardApprovalDetailViewDTO> GetProductionCardApprovalBrand(GetProductionCardApprovalDetailInput input);
        List<WagesProductionCardApprovalDetailViewGroupDTO> GetProductionCardApprovalDetailGroup(
            GetProductionCardApprovalDetailInput input);
        bool GetStatusApproval(string locationCode, string unitCode, DateTime productionDate);
        bool GetStatusComplete(string locationCode, string unitCode, DateTime productionDate);
        bool GetStatusReturn(string locationCode, string unitCode, DateTime productionDate);
        void ApproveProductionCardApprovalDetail(string userName, List<ProductionCardDTO> prodCard, UserSession CurrentUser);
        void CompleteProductionCardApprovalDetail(string userName, List<ProductionCardDTO> prodCard, UserSession CurrentUser);
        void ReturnProductionCardApprovalDetail(string userName, List<ProductionCardDTO> prodCard, UserSession CurrentUser);
        ProductionCard GetLatestProdCardRevType(string location, string unit, string brand, string process,
            string group, DateTime? productionDate);
        #endregion

        #region Eblek Release Approval
        IEnumerable<EblekReleaseApprovalDTO> GetEblekReleaseApproval(GetEblekReleaseApprovalInput input);
        EblekReleaseApprovalDTO ApproveEblekReleaseApproval(EblekReleaseApprovalDTO eblekReleaseApprovalToApprove, UserSession CurrentUser);
        EblekReleaseApprovalDTO ReturnEblekReleaseApproval(EblekReleaseApprovalDTO eblekReleaseApprovalToApprove, UserSession CurrentUser);
        void SendEmail(GetUserAndEmailInput input);
        void SendEmailApprove(GetUserAndEmailInput input);
        #endregion

        #region Eblek Release

        List<ExePlantProductionEntryVerificationDTO> GetExeProductionEntryReleases(GetExeProductionEntryReleaseInput input);
        List<ExeProductionEntryReleaseDTO> GetExeProductionEntryReleasesNew(GetExeProductionEntryReleaseInput input);
        ExeProductionEntryReleaseDTO DeleteExeProductionEntryRelease(ExeProductionEntryReleaseDTO data);
        ExeProductionEntryReleaseDTO SaveExeProductionEntryRelease(ExeProductionEntryReleaseDTO data, bool? isChecked);

        bool checkIsLockedState(string productionEntryCode, bool? isChecked);
        ExeProductionEntryReleaseDTO UpdateExeProductionEntryRelease(ExeProductionEntryReleaseDTO data);
        List<ExePlantProductionEntryVerificationDTO> GetPlantProdVerificationFromEntryRelease(GetExePlantProductionEntryVerificationInput input);
        int GetLatestProdCardRevTypeForRelease(string location, string unit, string brand, string process, string group, DateTime? productionDate);
        void SendEmailWagesEblekRelease(GetUserAndEmailInput input, string currUserName);

        #endregion

        #region Absent Report
        List<GetWagesReportAbsentViewDTO> GetWagesReport(GetWagesReportAbsentViewInput input);
        List<GetWagesReportAbsentViewDTO> GetWagesReportMore(GetWagesReportAbsentViewInput input);
        List<GetWagesReportAbsentGroupViewDTO> GetWagesReportGroup(GetWagesReportAbsentViewInput input);
        IEnumerable<WagesReportDetailEmployeeDTO> GetWagesReportDetailPerEmployee(string employeeID, DateTime startDate, DateTime endDate);
        #endregion

        #region Wages Report Summary Production Card

        List<string> GetProductionCardBrandGroupCode(GetWagesReportSummaryInput input);
        List<WagesReportSummaryDTO> GetWagesReportSummary(GetWagesReportSummaryInput input);

        #endregion

        #region Available Position Number

        List<GetWagesReportAvailablePositionNumberViewDTO> WagesReportAvailablePositionNumber(
            WagesReportAvailablePositionNumberInput input);

        #endregion

        IEnumerable<string> GetProcessGroupFromReportByGroup(GetProductionCardInput input);
    }
}
