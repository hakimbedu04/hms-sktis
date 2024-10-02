using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;

namespace HMS.SKTIS.Contracts
{
    public interface ITPOFeeBLL
    {
        #region TPO Fee Actual
        TPOFeeExeActualViewDTO GetTPOFeeActualByProductionCode(string TPOFeeCode);
        List<TPOFeeExeActualViewDTO> GetTpoFeeExeActuals(GetTPOFeeExeActualInput input);
        List<TPOFeeExeAPOpenViewDTO> GetTpoFeeExeAPOpens(GetTPOFeeExeAPOpenInput input);
        TPOFeeHdrDTO GetTpoFeeHdrById(string tpoFeeCode);
        TPOFeeHdrDTO GetTpoFeeHdrByParam(string locationCode, string brandCode, int? year, int? week);
        TpoFeeHdrPlanDto GetTpoFeeHdrPlanByParam(string locationCode, string brandCode, int? year, int? week);
        bool CheckBtnSave(string tpoFeeCode);
        void AuthorizeActual(string tpoFeeCode, string userName);
        TPOFeeHdrDTO SaveTpoFeeHdr(string tpoFeeHrd, string taxtNoProd, string taxtNoMgmt);
        #endregion

        #region TPO Fee Plan

        List<TpoFeePlanViewDto> GetTpoFeePlanView(GetTPOFeeHdrPlanInput input);
        TpoFeeHdrPlanDto GetTpoFeeHdrPlan(string id);

        #endregion

        #region TPO Fee Production Daily
        List<TPOFeeProductionDailyDTO> GetTpoFeeProductionDailys(string tpoFeeCode);
        #endregion

        #region TPO Fee Calculation
        List<TPOFeeCalculationDTO> GetTpoFeeCalculation(string tpoFeeCode);
        List<TPOFeeCalculationPlanDto> GetTpoFeeCalculationPlan(string tpoFeeCode);
        
        #endregion
        string GetUserAdByRoleLocation(string role, string location);

        #region TPO Fee Approval
        List<TPOFeeApprovalViewDTO> GetTpoFeeApprovals(GetTPOFeeExeActualInput input);
        #endregion
        #region TPO Fee AP Open
        List<TPOFeeAPOpenDTO> GetTpoFeeAPOpen(GetTPOFeeAPOpenInput input);
        List<TPOFeeAPOpenDTO> GetCompletedTpoFeeAPOpen(GetTPOFeeAPOpenInput input);

        List<TPOGenerateP1TemplateViewDTO> GetP1Template(GetTPOFeeAPOpenInput input);

        List<GenerateP1TemplateAPDTO> GetP1TemplateAP(GetTPOFeeAPOpenInput input);
        #endregion
        #region TPO Fee AP Close
        List<TPOFeeAPCloseDTO> GetTpoFeeAPClose(GetTPOFeeAPCloseInput input);
        #endregion

        #region TPO Reports Package

        List<TPOFeeReportsPackageCompositeDTO> GetTpoFeeReportsPackage(GetTPOReportsPackageInput input);
        List<TPOFeeReportsPackageCompositeDTO> GetTpoFeeReportsPackageNew(int year);
        #endregion

          //object GetTpoFeeExeAPOpens(GetTPOFeeExeAPOpenInput criteria);
          
        #region TPO Reports Summary

        List<TPOFeeReportsSummaryCompositeDTO> GetTpoFeeReportsSummary(GetTPOReportsSummaryInput input);
      
        #endregion

        #region TPO Reports Production

        //List<TPOFeeReportsProductionDTO> GetTpoFeeReportsProduction(GetTPOReportsProductionInput input);
        List<TPOFeeReportsProductionDTO> GetTpoFeeReportsProductionView(GetTPOReportsProductionInput input);

        List<TPOFeeReportsProductionDTO> GetTpoFeeReportsProductionViewYearWeek(GetTPOReportsProductionInput input);
        List<TPOFeeReportsProductionDTO> GetTpoFeeReportsProductionViewAll(GetTPOReportsProductionInput input);
        #endregion

        #region EMAILs

        #region TPO Fee Actual Emailing
        void SendEmailSubmitTPOFeeActual(string tpoFeeCode, string regional, string currUserName);
        void SendEmailReturnTPOFeeActual(string tpoFeeCode, string regional, string currUserName);
        void SendEmailApprovalTPOFeeActual(string tpoFeeCode, string regional, string currUserName);
        void SendEmailAuthorizeTPOFeeActual(string tpoFeeCode, string regional, string currUserName);
        void SendEmailTPOApprovalPage(IEnumerable<string> listTPOFeeCode, string regional, string currUserName, string action);
        #endregion

        #endregion
    }
}
