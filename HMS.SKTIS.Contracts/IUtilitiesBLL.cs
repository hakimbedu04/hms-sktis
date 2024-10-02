using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.BusinessObjects.Inputs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.Core;

namespace HMS.SKTIS.Contracts
{
    public interface IUtilitiesBLL
    {
        #region Transaction History and Flow
        List<UtilTransactionLogDTO> GetTransactionLog(TransactionLogInput input);
        int DeleteTransactionLog(TransactionLogInput input);
        List<TransactionHistoryDTO> GetTransactionHistory(TransactionLogInput input);
        List<TransactionHistoryDTO> GetTransactionHistoryWagesApprovalDetail(TransactionLogInput input);
        int? RoleButtonChecker(string tPOFeeCode, int iDRole, string page, string button);
        List<TransactionHistoryDTO> GetTransactionHistoryByPage(TransactionLogInput input, string pageSource);
        List<TransactionHistoryDTO> GetTransactionHistoryWagesProdcardCorrection(TransactionLogInput input);
        List<TransactionFlowDTO> GetTransactionFlow(TransactionLogInput input);
        List<TransactionFlowDTO> GetTransactionFlowApproval(TransactionLogInput input);
        bool CheckDataAlreadySumbit(string transactionCode);
        bool CheckDataAlreadySumbit(string transactionCode, string page);
        bool CheckAllDataAlreadySubmit(string transactionCode, string page);
        bool CheckAllDataAlreadySubmit(string param1, string param2, string page);
        bool CheckAllDataAlreadySubmitForTPU(string param1, string param2);
        bool CheckDataAlreadySaveInProdCard(string transactionCode);
        UtilTransactionLogDTO GetTransactionLogById(string productionCardCode, DateTime transactionDate);
        UtilTransactionLogDTO GetTransactionLogByTransactionCode(string transactionCode);
        List<UtilTransactionLogDTO> GetTransactionLogsByTransactionCode(string transactionCode);
        bool CheckDataAlreadyOnTransactionLog(string transactionCode, string pageName, string btnName);
        string GetUserOnTransactionLog(string transactionCode, string pageName, string btnName);
        UtilTransactionLogDTO GetLatestEblekProductionEntryTransLog(string transactionCode);
        UtilTransactionLogDTO GetTransactionLogByTransCodeAndIDFlow(string transcode, int idflow);
        bool[] getSubmittedWpp(string data);
        void DeleteTransactionLogByListIDFlow(string transactionCode, IEnumerable<int> listIDFlow);
        IEnumerable<int> GetIDFlow(string sourceFunctionFrom, string functionName);
        UtilRoleDTO GetRoleByIDFlow(int IDFlow);

        UtilRoleDTO GetRoleByRoleCode(string roleCode);

        #endregion

        #region Permissions
        UtilTransactionLogDTO GetLatestActionTransLog(string transCode, string pageName);

        UtilTransactionLogDTO GetLatestActionTransLogWithoutPage(string transCode);
        Responsibility GetResponsibilityPage(int IDResponsibility);
        UtilUsersResponsibilityDTO GetUsersResponsibility(int IDResponsibility, string UserAD);
        List<UtilResponsibilityDTO> GetListResponsibility(string userAD);

        List<UtilResponsibilityDTO> GetListResponsibility2(string userAD);
        ResponsibilityButton GetResponsibilityButton(int Role, int PageID);
        #endregion

        #region Roles
        List<UtilRoleDTO> GetAllRoles();
        List<UtilRoleDTO> GetListRoles(BaseInput criteria);
        UtilRoleDTO InsertRole(UtilRoleDTO data);
        UtilRoleDTO UpdateRole(UtilRoleDTO data);
        List<int> GetListRolesFunctionByRoles(int roleID);
        #endregion

        #region Rules
        UtilRuleDTO GetRuleByID(int ID);
        List<UtilRuleDTO> GetListRules(GetUtilSecurityRulesViewInput criteria);
        UtilRuleDTO InsertRule(UtilRuleDTO data);
        UtilRuleDTO UpdateRule(UtilRuleDTO data);
        UtilRuleDTO InsertRuleUnit(UtilRuleDTO data);
        UtilRuleDTO UpdateRuleUnit(UtilRuleDTO data);
        #endregion

        #region Functions
        List<UtilFunctionDTO> GetListFunctions(BaseInput criteria);
        List<UtilFunctionDTO> GetListFunctionsByType(string type, int parentID = 0);
        UtilFunctionDTO InsertFunction(UtilFunctionDTO data);
        UtilFunctionDTO UpdateFunction(UtilFunctionDTO data);
        void UpdateRolesFunction(List<UtilRolesFunctionDTO> rolesFunctions, string username);
        #endregion

        #region Responsibilities
        List<UtilUserResponsibilitiesRoleViewDTO> GetListUserResponsibilitesByResponsibility(UtilSecurityResponsibilitiesInput input);
        List<UtilResponsibilityDTO> GetListResponsibilities(UtilSecurityResponsibilitiesInput criteria);
        bool GetValidStatusResponsibility(int id, string userAd);
        UtilResponsibilityDTO GetUtilResponsibilityById(int id);
        List<UtilRuleDTO> GetListRulesByResponsibility(int IDResponsibility);
        int GetNewIDResponsibility();
        UtilResponsibilityDTO SaveRoleResponsibility(UtilResponsibilityDTO responsibility);
        void UpdateRoleResponsibility(UtilResponsibilityDTO responsibility);
        UtilRuleDTO SaveRulesResponsibilities(UtilRuleDTO rules, int IDResponsibility);
        UtilUsersResponsibilityDTO SaveUsersResponsibilities(UtilUsersResponsibilityDTO respon);
        UtilUsersResponsibilityDTO UpdateUsersAd(UtilUsersResponsibilityDTO respon);
        UtilUsersResponsibilityDTO InsertUsersAd(UtilUsersResponsibilityDTO respon);
        bool DeleteAllRulesResponsibilities(int IDResponsibility);
        bool DeleteAllUsersResponsibilities(string UserAD);
        #endregion

        UtilTransactionLogDTO GetLatestActionTransLogExceptSave(string transCode, string pageName);
        UtilTransactionLogDTO GetAvailableProdCardTranssactionLog(string transCode);
        bool CheckLatestActionTransLogExceptSaveIsSubmit(string transCode, string pageName);
        List<UtilTransactionLogDTO> GetLatestTransLogExceptSaveEblekAbsenteeism(List<string> transCode, string pageName);
        bool GetTransLogSubmittedEntryVerification(string transCode);

        #region Delegations
        List<UtilDelegationDto> GetListDelegations(BaseInput input);
        UtilDelegationDto InsertDelegations(UtilDelegationDto data);
        UtilDelegationDto UpdateDelegation(UtilDelegationDto data);

        #endregion

        #region WorkFlow
        List<UtilFlowDTO> GetListWorkflow(BaseInput input);
        UtilFlowDTO InsertWorkflow(UtilFlowDTO data);
        UtilFlowDTO UpdateWorkflow(UtilFlowDTO data);

        #endregion
    }
}
