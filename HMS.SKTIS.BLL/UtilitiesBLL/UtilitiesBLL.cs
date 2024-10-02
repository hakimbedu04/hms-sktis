using System.Net.Configuration;
using AutoMapper;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.BusinessObjects.Inputs.Utils;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enums = HMS.SKTIS.Core.Enums;

namespace HMS.SKTIS.BLL.UtilitiesBLL
{
    public class UtilitiesBLL : IUtilitiesBLL
    {
        private IUnitOfWork _uow;
        private IMasterDataBLL _masterDataBll;
        private ISqlSPRepository _sqlSPRepo;
        private IGenericRepository<UtilTransactionLog> _utilTransactionLog;
        private IGenericRepository<UtilFunction> _utilFunctionRepo;
        private IGenericRepository<UtilUsersResponsibility> _utilUserResponsibilityRepo;
        private IGenericRepository<UtilFlowFunctionView> _utilFlowFunctionView;
        private IGenericRepository<UtilRolesFunctionView> _utilRolesFunctionView;
        private IGenericRepository<UtilFlow> _utilFlow;
        private IGenericRepository<UtilRole> _utilRole;
        private IGenericRepository<UtilResponsibilityRule> _utilResponsibilityRuleRepo;
        private IGenericRepository<PlanWeeklyProductionPlanning> _wpp;
        private IGenericRepository<MstGenLocation> _mstGenLocationRepo;
        private IGenericRepository<UtilRule> _utilRule;
        private IGenericRepository<UtilFunction> _utilFunction;
        private IGenericRepository<UtilResponsibility> _utilResponsibility;
        private IGenericRepository<UtilUserResponsibilitiesRoleView> _utilUserResponsibilityViewRepo;
        private IGenericRepository<UtilRolesFunction> _utilRoleFunction;
        private IGenericRepository<UtilDelegation> _utilDelegation;
        private IGenericRepository<MstADTemp> _userADtemp;

        public UtilitiesBLL(IUnitOfWork uow, IMasterDataBLL masterDataBll)
        {
            _uow = uow;
            _sqlSPRepo = _uow.GetSPRepository();
            _utilTransactionLog = _uow.GetGenericRepository<UtilTransactionLog>();
            _utilFunctionRepo = _uow.GetGenericRepository<UtilFunction>();
            _utilUserResponsibilityRepo = _uow.GetGenericRepository<UtilUsersResponsibility>();
            _utilFlowFunctionView = _uow.GetGenericRepository<UtilFlowFunctionView>();
            _utilRolesFunctionView = _uow.GetGenericRepository<UtilRolesFunctionView>();
            _utilResponsibilityRuleRepo = _uow.GetGenericRepository<UtilResponsibilityRule>();
            _utilFlow = _uow.GetGenericRepository<UtilFlow>();
            _utilRole = _uow.GetGenericRepository<UtilRole>();
            _mstGenLocationRepo = _uow.GetGenericRepository<MstGenLocation>();
            _wpp = _uow.GetGenericRepository<PlanWeeklyProductionPlanning>();
            _utilRule = _uow.GetGenericRepository<UtilRule>();
            _utilFunction = _uow.GetGenericRepository<UtilFunction>();
            _utilResponsibility = _uow.GetGenericRepository<UtilResponsibility>();
            _utilRoleFunction = _uow.GetGenericRepository<UtilRolesFunction>();
            _utilDelegation = _uow.GetGenericRepository<UtilDelegation>();
            _utilUserResponsibilityViewRepo = _uow.GetGenericRepository<UtilUserResponsibilitiesRoleView>();
            _userADtemp = _uow.GetGenericRepository<MstADTemp>();
            _masterDataBll = masterDataBll;
        }

        #region Transaction History and Flow
        public int DeleteTransactionLog(TransactionLogInput input)
        {
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            if (!string.IsNullOrEmpty(input.code_1) && !string.IsNullOrEmpty(input.code_2) && !string.IsNullOrEmpty(input.code_3))
            {
                var TransactionCode = this.GenerateTransactionCode(input.separator[0], new string[] { input.code_1, input.code_2, input.code_3 });
                queryFilter = queryFilter.And(m => m.TransactionCode == TransactionCode);
            }

            if (!string.IsNullOrEmpty(input.TransactionCode))
            {
                queryFilter = queryFilter.And(p => p.TransactionCode.Contains(input.TransactionCode));
            }

            if (input.NotEqualIdFlow.HasValue)
            {
                queryFilter = queryFilter.And(p => p.IDFlow != input.NotEqualIdFlow);
            }

            var dbResult = _utilTransactionLog.Get(queryFilter);

            if (dbResult != null)
            {
                foreach(var q in dbResult){
                    var a = _utilTransactionLog.GetByID(q.TransactionCode, q.TransactionDate);
                    _utilTransactionLog.Delete(a);
                }
                _uow.SaveChanges();
                
            }

            return 1;
        }

        public void DeleteTransactionLogByListIDFlow(string transactionCode, IEnumerable<int> listIDFlow) 
        {
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            if (!string.IsNullOrEmpty(transactionCode))
            {
                queryFilter = queryFilter.And(p => p.TransactionCode == transactionCode);
            }

            if (listIDFlow.Any())
            {
                queryFilter = queryFilter.And(p => listIDFlow.Contains(p.IDFlow.HasValue ? p.IDFlow.Value : 1));
            }

            var dbResult = _utilTransactionLog.Get(queryFilter);

            if (dbResult != null)
            {
                foreach (var q in dbResult)
                {
                    var a = _utilTransactionLog.GetByID(q.TransactionCode, q.TransactionDate);
                    _utilTransactionLog.Delete(a);
                }
                _uow.SaveChanges();

            }
        }

        public List<UtilTransactionLogDTO> GetTransactionLog(TransactionLogInput input)
        {
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            if (!string.IsNullOrEmpty(input.code_1) && !string.IsNullOrEmpty(input.code_2) && !string.IsNullOrEmpty(input.code_3))
            {
                var TransactionCode = this.GenerateTransactionCode(input.separator[0], new string[] { input.code_1, input.code_2, input.code_3 });
                queryFilter = queryFilter.And(m => m.TransactionCode == TransactionCode);
            }

            if (!string.IsNullOrEmpty(input.TransactionCode))
            {
                queryFilter = queryFilter.And(p => p.TransactionCode.Contains(input.TransactionCode));
            }

            if (input.NotEqualIdFlow.HasValue)
            {
                queryFilter = queryFilter.And(p => p.IDFlow != input.NotEqualIdFlow);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilTransactionLog>();

            var dbResult = _utilTransactionLog.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<UtilTransactionLogDTO>>(dbResult);
        }

        /// <summary>
        /// Generate dynamicaly Transaction Code for History
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="codes"></param>
        /// <returns></returns>
        private string GenerateTransactionCode(char separator, params string[] codes)
        {
            var tempTransactionCode = "";
            foreach (var code in codes)
            {
                tempTransactionCode += code;
                tempTransactionCode += separator;
            }
            return tempTransactionCode.TrimEnd(separator);
        }

        public List<TransactionHistoryDTO> GetTransactionHistory(TransactionLogInput input)
        {
            var TransactionCode = this.GenerateTransactionCode(input.separator[0], new string[] { input.code_1, input.code_2, input.code_3, input.code_4, input.code_5, input.code_6, input.code_7, input.code_8, input.code_9 });
            var transactionHistory = _sqlSPRepo.GetTransactionHistory(TransactionCode, null);
            return Mapper.Map<List<TransactionHistoryDTO>>(transactionHistory);
        }
        

        public List<TransactionHistoryDTO> GetTransactionHistoryWagesApprovalDetail(TransactionLogInput input)
        {
             var week = _masterDataBll.GetWeekByDate(input.TransactionDate).Week;
             input.code_8 = week.ToString();
            var TransactionCode = this.GenerateTransactionCode(input.separator[0], new string[] { input.code_1, input.code_2, input.code_3, input.code_4, input.code_5, input.code_6, input.code_7, input.code_8, input.code_9 });
            var transactionHistory = _sqlSPRepo.GetTransactionHistory(TransactionCode, "ProductionCardApprovalDetail");
            return Mapper.Map<List<TransactionHistoryDTO>>(transactionHistory);
        }

        public int? RoleButtonChecker(string tPOFeeCode, int iDRole, string page, string button)
        {
            return _sqlSPRepo.RoleButtonChecker(tPOFeeCode, iDRole, page, button).SingleOrDefault();
        }

        public List<TransactionHistoryDTO> GetTransactionHistoryByPage(TransactionLogInput input, string pageSource)
        {
            var TransactionCode = this.GenerateTransactionCode(input.separator[0], new string[] { input.code_1, input.code_2, input.code_3, input.code_4, input.code_5, input.code_6, input.code_7, input.code_8, input.code_9 });
            var transactionHistory = _sqlSPRepo.GetTransactionHistory(TransactionCode, pageSource);
            return Mapper.Map<List<TransactionHistoryDTO>>(transactionHistory);
        }

        public List<TransactionHistoryDTO> GetTransactionHistoryWagesProdcardCorrection(TransactionLogInput input)
        {
            var TransactionCode = this.GenerateTransactionCode(input.separator[0], new string[] { input.code_1, input.code_2, input.code_3, input.code_4, input.code_5, input.code_6, input.code_7, input.code_8, input.code_9 });
            var transactionHistory = _sqlSPRepo.GetTransactionHistoryWagesProdcardCorrection(TransactionCode);
            return Mapper.Map<List<TransactionHistoryDTO>>(transactionHistory);
        }

        public List<TransactionFlowDTO> GetTransactionFlow(TransactionLogInput input)
        {
            var transactionFlow = _sqlSPRepo.GetTransactionFlow(input.FunctionName).OrderBy(f => f.IDFlow);
            return Mapper.Map<List<TransactionFlowDTO>>(transactionFlow);
        }

        public List<TransactionFlowDTO> GetTransactionFlowApproval(TransactionLogInput input)
        {
            var transactionFlow = _sqlSPRepo.GetTransactionFlow(input.FunctionName);
            return Mapper.Map<List<TransactionFlowDTO>>(transactionFlow);
        }

        public UtilTransactionLogDTO GetTransactionLogById(string productionCardCode, DateTime transactionDate)
        {
            var transactionLog = _utilTransactionLog.GetByID(productionCardCode, transactionDate);
            return Mapper.Map<UtilTransactionLogDTO>(transactionLog);
        }

        public UtilTransactionLogDTO GetTransactionLogByTransactionCode(string transactionCode)
        {
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(p => p.TransactionCode.Contains(transactionCode));

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "TransactionDate" }, "DESC");
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilTransactionLog>();

            var dbResult = _utilTransactionLog.Get(queryFilter, orderByFilter).FirstOrDefault();

            return Mapper.Map<UtilTransactionLogDTO>(dbResult);
        }

        public bool CheckDataAlreadySumbit(string transactionCode)
        {
            var transactionLog = _utilTransactionLog.Get(m => m.TransactionCode == transactionCode && m.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString());
            return transactionLog.Any();
        }

        public bool CheckDataAlreadySumbit(string transactionCode, string page)
        {
            var transactionLog = _utilTransactionLog.Get(m => m.TransactionCode == transactionCode && m.UtilFlow.UtilFunction.UtilFunction1.FunctionName == page).OrderByDescending(o => o.UpdatedDate);
            var submit = transactionLog.FirstOrDefault();
            if (submit != null && submit.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString())
            {
                return true;
            }
            return false;
        }

        public bool CheckAllDataAlreadySubmit(string transactionCode, string page)
        {
            var transactionLog = _utilTransactionLog.Get(m => m.TransactionCode.Contains(transactionCode) && m.UtilFlow.UtilFunction.UtilFunction1.FunctionName == page).OrderByDescending(o => o.UpdatedDate);
            var submit = transactionLog.FirstOrDefault();
            if (submit != null && submit.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString())
            {
                return true;
            }
            return false;
        }

        public bool CheckAllDataAlreadySubmit(string param1, string param2, string page)
        {
            var transactionLog = _utilTransactionLog.Get(m => m.TransactionCode.Contains(param1) 
                && m.TransactionCode.Contains(param2)
                && m.UtilFlow.UtilFunction.UtilFunction1.FunctionName == page).OrderByDescending(o => o.UpdatedDate);
            //var submit = transactionLog.FirstOrDefault();
            var submit = transactionLog.Where(m => m.UtilFlow.UtilFunction.FunctionName.Contains(Enums.ButtonName.Submit.ToString())).FirstOrDefault();
            if (submit != null)
            {
                return true;
            }
            return false;
        }

        public bool CheckAllDataAlreadySubmitForTPU(string param1, string param2)
        {
            var submitTransactionLog = _utilTransactionLog.Get(m => m.TransactionCode.Contains(param1)
                && m.TransactionCode.Contains(param2)
                && m.IDFlow >= 13 && m.IDFlow != 15).OrderByDescending(m=>m.UpdatedDate).FirstOrDefault();
            if (submitTransactionLog != null)
            {
                return true;
            }
            return false;
        }

        public bool CheckDataAlreadySaveInProdCard(string transactionCode)
        {
            var btnCancelSubmit = EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit);
            var formSourceCancelSubmit = _utilFunctionRepo.Get(m => m.FunctionName == Enums.PageName.ProductionCard.ToString()).Select(m => m.IDFunction).SingleOrDefault();
            var actionButtonCancelSubmit = _utilFunctionRepo.Get(m => m.FunctionName == btnCancelSubmit
                                                && m.ParentIDFunction == formSourceCancelSubmit).Select(m => m.IDFunction).SingleOrDefault();

            var transactionLogsCancelSubmit = _utilTransactionLog.Get(m => m.TransactionCode.Replace(Enums.CombineCode.WPC.ToString(), "") == transactionCode.Replace(Enums.CombineCode.EBL.ToString(), "")
                                                       && m.UtilFlow.FormSource == formSourceCancelSubmit
                                                       && m.UtilFlow.ActionButton == actionButtonCancelSubmit);
            if (transactionLogsCancelSubmit.Any()) return true;

            var formSourceSubmit = _utilFunctionRepo.Get(m => m.FunctionName == Enums.PageName.ProductionCard.ToString()).Select(m => m.IDFunction).SingleOrDefault();
            var actionButtonSubmit = _utilFunctionRepo.Get(m => m.FunctionName == Enums.ButtonName.Submit.ToString()
                                                && m.ParentIDFunction == formSourceSubmit).Select(m => m.IDFunction).SingleOrDefault();

            var transactionLogsSubmit = _utilTransactionLog.Get(m => m.TransactionCode.Replace(Enums.CombineCode.WPC.ToString(), "") == transactionCode.Replace(Enums.CombineCode.EBL.ToString(), "")
                                                       && m.UtilFlow.FormSource == formSourceSubmit
                                                       && m.UtilFlow.ActionButton == actionButtonSubmit);
            if (transactionLogsSubmit.Any()) return false;


            var formSourceSave = _utilFunctionRepo.Get(m => m.FunctionName == Enums.PageName.ProductionCard.ToString()).Select(m => m.IDFunction).SingleOrDefault();
            var actionButtonSave = _utilFunctionRepo.Get(m => m.FunctionName == Enums.ButtonName.Save.ToString()
                                                && m.ParentIDFunction == formSourceSave).Select(m => m.IDFunction).SingleOrDefault();

            var transactionLogsSave = _utilTransactionLog.Get(m => m.TransactionCode.Replace(Enums.CombineCode.WPC.ToString(), "") == transactionCode.Replace(Enums.CombineCode.EBL.ToString(), "")
                                                       && m.UtilFlow.FormSource == formSourceSave
                                                       && m.UtilFlow.ActionButton == actionButtonSave);
            return transactionLogsSave.Any();
        }

        public bool CheckDataAlreadyOnTransactionLog(string transactionCode, string pageName, string btnName)
        {
            var formSourceSubmit = _utilFunctionRepo.Get(m => m.FunctionName == pageName
                                                        && m.FunctionType == "Page")
                                                    .Select(m => m.IDFunction).FirstOrDefault();
            var actionButtonSubmit = _utilFunctionRepo.Get(m => m.FunctionName == btnName
                                                       && m.ParentIDFunction == formSourceSubmit).Select(m => m.IDFunction).FirstOrDefault();

            var transactionLogsSubmit = _utilTransactionLog.Get(m => m.TransactionCode == transactionCode
                                                       && m.UtilFlow.FormSource == formSourceSubmit
                                                       && m.UtilFlow.ActionButton == actionButtonSubmit);
            return transactionLogsSubmit.Any();
        }

        public string GetUserOnTransactionLog(string transactionCode, string pageName, string btnName)
        {
            var formSourceSubmit = _utilFunctionRepo.Get(m => m.FunctionName == pageName
                                                        && m.FunctionType == "Page")
                                                    .Select(m => m.IDFunction).FirstOrDefault();
            var actionButtonSubmit = _utilFunctionRepo.Get(m => m.FunctionName == btnName
                                                       && m.ParentIDFunction == formSourceSubmit).Select(m => m.IDFunction).FirstOrDefault();

            var transactionLogsSubmit = _utilTransactionLog.Get(m => m.TransactionCode == transactionCode
                                                       && m.UtilFlow.FormSource == formSourceSubmit
                                                       && m.UtilFlow.ActionButton == actionButtonSubmit).OrderByDescending(m => m.TransactionDate);
            string userName = "";
            var realn = "";
            if (transactionLogsSubmit.Any())
            {
                userName = transactionLogsSubmit.FirstOrDefault().CreatedBy;
                var userAD = _userADtemp.Get(m => m.UserAD == userName).FirstOrDefault();
                if (userAD != null)
                {
                    realn = userAD.Name;
                }
            }

            return realn;
        }

        public UtilTransactionLogDTO GetLatestEblekProductionEntryTransLog(string transactionCode)
        {
            var transactionLogsSubmit = _utilTransactionLog
                .Get(m => m.TransactionCode == transactionCode)
                .OrderByDescending(o => o.CreatedDate)
                .FirstOrDefault();

            return Mapper.Map<UtilTransactionLogDTO>(transactionLogsSubmit);
        }
        #endregion

        public bool checkTransLogByIDFlowAndTransCode(string transcode, int idflow)
        {
            var transactionLog = _utilTransactionLog.Get(m => m.TransactionCode == transcode && m.IDFlow == idflow)
                .OrderByDescending(o => o.UpdatedDate);
            var submit = transactionLog.FirstOrDefault();

            return (submit != null) ? true : false;
        }

        public UtilTransactionLogDTO GetTransactionLogByTransCodeAndIDFlow(string transCode, int IDFlow)
        {
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(p => p.TransactionCode.Equals(transCode));
            queryFilter = queryFilter.And(p => p.IDFlow == IDFlow);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "TransactionDate" }, "DESC");
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilTransactionLog>();

            var dbResult = _utilTransactionLog.Get(queryFilter, orderByFilter).FirstOrDefault();

            return Mapper.Map<UtilTransactionLogDTO>(dbResult);
        }

        public List<UtilTransactionLogDTO> GetTransactionLogsByTransactionCode(string TransactionCode)
        {
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(p => p.TransactionCode.Equals(TransactionCode));

            var dbResult = _utilTransactionLog.Get(queryFilter);

            return Mapper.Map<List<UtilTransactionLogDTO>>(dbResult);
        }

        public UtilRoleDTO GetRoleByIDFlow(int IDFlow)
        {
            var utilFlowFilter = PredicateHelper.True<UtilFlow>();

            utilFlowFilter = utilFlowFilter.And(p => p.IDFlow == IDFlow);

            var utilFlow = _utilFlow.Get(utilFlowFilter).Select(s => s.DestinationRole).FirstOrDefault();

            if (utilFlow != null)
            {
                var utilRole = PredicateHelper.True<UtilRole>();

                utilRole = utilRole.And(p => p.IDRole == utilFlow);

                var role = _utilRole.Get(utilRole).FirstOrDefault();

                return Mapper.Map<UtilRoleDTO>(role);
            }
            else
            {
                return null;
            }

        }

        public UtilRoleDTO GetRoleByRoleCode(string roleCode)
        {
            var utilRole = PredicateHelper.True<UtilRole>();

            utilRole = utilRole.And(p => p.RolesCode == roleCode);

            var role = _utilRole.Get(utilRole).FirstOrDefault();

            return Mapper.Map<UtilRoleDTO>(role);
        }

        public bool[] getSubmittedWpp(string data)
        {
            bool[] ret = new bool[2];
            var readyToSubmit = true;
            var reSubmitWpp = false;
            string[] ListArray = data.Split(',');
            var tpk = ListArray[0];
            var wpp = ListArray[1];

            string[] tpkarray = tpk.Split('/');
            var locationCode = tpkarray[1];	 
            var brandcode = tpkarray[2];
            var kpsYear = Convert.ToInt32(tpkarray[3]);
            var kpsWeek = Convert.ToInt32(tpkarray[4]);

            if (tpk == null || wpp == null)
                readyToSubmit = false;
            else{ 
                var datetpk = _utilTransactionLog.Get(m => m.TransactionCode == tpk && m.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString()).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                var datewpp = _utilTransactionLog.Get(m => m.TransactionCode == wpp && m.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString()).OrderByDescending(o => o.CreatedDate).FirstOrDefault();
                var existWpp = _wpp.Get(m => m.LocationCode == locationCode && m.BrandCode == brandcode && m.KPSYear == kpsYear && m.KPSWeek == kpsWeek).OrderByDescending(o => o.UpdatedDate).FirstOrDefault();
            
                if (datewpp == null)
                    readyToSubmit = false;
                else if (datetpk == null)
                    readyToSubmit = true;
                else if (datetpk.CreatedDate < datewpp.CreatedDate)
                {
                    readyToSubmit = true;
                    reSubmitWpp = true;
                    if (existWpp.UpdatedDate < datewpp.UpdatedDate)
                    {
                        readyToSubmit = false;
                        reSubmitWpp = false;
                    }
                }
                else {
                    readyToSubmit = false;
                }
            }
            ret[0] = readyToSubmit;
            ret[1] = reSubmitWpp;

            return ret;
        }

        public IEnumerable<int> GetIDFlow(string sourceFunctionFrom, string functionName) 
        {
            var queryFilter = PredicateHelper.True<UtilFlowFunctionView>();

            if (!String.IsNullOrEmpty(sourceFunctionFrom))
                queryFilter = queryFilter.And(c => c.SourceFunctionForm == sourceFunctionFrom);

            if(!String.IsNullOrEmpty(functionName))
                queryFilter = queryFilter.And(c => c.FunctionName == functionName);

            var dbResult = _utilFlowFunctionView.Get(queryFilter);

            var utilFunctFlowDTO = Mapper.Map<List<UtilFlowFunctionViewDTO>>(dbResult);

            // Get list ID flow
            var listIDFlow = utilFunctFlowDTO.Select(c => c.IDFlow).Distinct().ToList();

            return listIDFlow;
        }
        #region Roles
        public List<UtilRoleDTO> GetAllRoles() {
            var data = _utilRole.Get();
            return Mapper.Map<List<UtilRoleDTO>>(data);
        }
        public List<UtilRoleDTO> GetListRoles(BaseInput input) {
            var queryFilter = PredicateHelper.True<UtilRole>();
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilRole>();

            var db = _utilRole.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<UtilRoleDTO>>(db);

        }
        public UtilRoleDTO InsertRole(UtilRoleDTO data)
        {
            ValidateInsertRoles(data);

            data.IDRole = GetMaxIDRole() + 1;

            var dbRoles = Mapper.Map<UtilRole>(data);

            //var procs = _utilRole.Get(p => p.IDRole == data.IDRole);
            dbRoles.CreatedDate = DateTime.Now;
            dbRoles.UpdatedDate = DateTime.Now;

            _utilRole.Insert(dbRoles);

            //var procLocs = procs.SelectMany(p => p.MstGenProcessSettingsLocations);
            //foreach (var procLoc in procLocs)
            //{
            //    procLoc.MstGenProcessSettings.Add(dbRole);
            //    _utilRole.Update(procLoc);
            //}

            _uow.SaveChanges();
            return data;
        }

        private int GetMaxIDRole()
        {
            return _utilRole.Get().Select(p => p.IDRole).Max();
        }
        private void ValidateInsertRoles(UtilRoleDTO rolesListToValidate)
        {
            var dbRoles = _utilRole.Get(p => p.RolesCode == rolesListToValidate.RolesCode && p.RolesName == rolesListToValidate.RolesName);
            if (dbRoles.Any())
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
        }
        public UtilRoleDTO UpdateRole(UtilRoleDTO data)
        {
            ValidateUpdateRoles(data);

            var dbRole = _utilRole.Get(p => p.IDRole == data.IDRole).FirstOrDefault();

            // Mapper.Map(processSettingDto, dbRole);

            //set update time
            dbRole.UpdatedDate = DateTime.Now;
            dbRole.RolesCode = data.RolesCode;
            dbRole.RolesName = data.RolesName;

            //_utilRole.Update(dbRole);
            _uow.SaveChanges();

            return Mapper.Map<UtilRoleDTO>(dbRole);
        }
        private void ValidateUpdateRoles(UtilRoleDTO rolesListToValidate)
        {
            var dbRoles = _utilRole.Get(p => p.IDRole == rolesListToValidate.IDRole);
            if (dbRoles == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
        }

        public List<int> GetListRolesFunctionByRoles(int roleID)
        {
            var function = _utilRolesFunctionView.Get(s => s.IDRole == roleID).OrderBy(o => o.ParentIDFunction).Select(s  => s.IDFunction).ToList();
            return function;

        }
        #endregion

        #region Rules
        public UtilRuleDTO GetRuleByID(int ID)
        {
            var db = _utilRule.GetByID(ID);

            return Mapper.Map<UtilRuleDTO>(db);

        }
        public List<UtilRuleDTO> GetListRules(GetUtilSecurityRulesViewInput input)
        {
            var queryFilter = PredicateHelper.True<UtilRule>();
            
            if (!string.IsNullOrEmpty(input.Location))
            {
                queryFilter = queryFilter.And(p => p.Location.Contains(input.Location));
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilRule>();

            var db = _utilRule.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<UtilRuleDTO>>(db);

        }

        public UtilRuleDTO InsertRule(UtilRuleDTO data)
        {
            ValidateInsertRules(data);

            var dbRules = Mapper.Map<UtilRule>(data);

            //var procs = _utilRole.Get(p => p.IDRole == data.IDRole);
            var maxIdRule = _utilRule.Get().Select(p => p.IDRule).Max();
            dbRules.IDRule = maxIdRule+1;
            dbRules.CreatedDate = DateTime.Now;
            dbRules.UpdatedDate = DateTime.Now;

            _utilRule.Insert(dbRules);

            //var procLocs = procs.SelectMany(p => p.MstGenProcessSettingsLocations);
            //foreach (var procLoc in procLocs)
            //{
            //    procLoc.MstGenProcessSettings.Add(dbRole);
            //    _utilRole.Update(procLoc);
            //}

            _uow.SaveChanges();
            return data;
        }

        public UtilRuleDTO InsertRuleUnit(UtilRuleDTO data)
        {
            ValidateInsertRulesUnit(data);

            var dbRules = Mapper.Map<UtilRule>(data);

            var maxIdRule = _utilRule.Get().Select(p => p.IDRule).Max();
            dbRules.IDRule = maxIdRule + 1;
            dbRules.CreatedDate = DateTime.Now;
            dbRules.UpdatedDate = DateTime.Now;

            _utilRule.Insert(dbRules);

            _uow.SaveChanges();
            return data;
        }

        private void ValidateInsertRules(UtilRuleDTO rulesListToValidate)
        {
            var dbRoles = _utilRule.Get(p => p.RulesName == rulesListToValidate.RulesName);
            if (dbRoles.Any())
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
        }

        private void ValidateInsertRulesUnit(UtilRuleDTO rulesListToValidate)
        {
            var dbRoles = _utilRule.Get(p => p.RulesName == rulesListToValidate.RulesName && p.Unit == rulesListToValidate.Unit);
            if (dbRoles.Any())
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
        }

        public UtilRuleDTO UpdateRule(UtilRuleDTO data)
        {
            ValidateUpdateRules(data);

            var dbRule = _utilRule.Get(p => p.IDRule == data.IDRule).FirstOrDefault();

            // Mapper.Map(processSettingDto, dbRole);

            //set update time
            dbRule.UpdatedDate = DateTime.Now;
            dbRule.RulesName = data.RulesName;

            //_utilRole.Update(dbRole);
            _uow.SaveChanges();

            return Mapper.Map<UtilRuleDTO>(dbRule);
        }

        public UtilRuleDTO UpdateRuleUnit(UtilRuleDTO data)
        {
            ValidateUpdateRulesUnit(data);

            var dbRule = _utilRule.Get(p => p.IDRule == data.IDRule).FirstOrDefault();

            // Mapper.Map(processSettingDto, dbRole);

            //set update time
            dbRule.UpdatedDate = DateTime.Now;
            dbRule.Unit = data.Unit;

            //_utilRole.Update(dbRole);
            _uow.SaveChanges();

            return Mapper.Map<UtilRuleDTO>(dbRule);
        }

        private void ValidateUpdateRules(UtilRuleDTO rulesListToValidate)
        {
            var dbRules = _utilRule.Get(p => p.IDRule == rulesListToValidate.IDRule);
            if (dbRules == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
        }

        private void ValidateUpdateRulesUnit(UtilRuleDTO rulesListToValidate)
        {
            var dbRules = _utilRule.Get(p => p.IDRule == rulesListToValidate.IDRule && p.Unit == rulesListToValidate.Unit);
            if (dbRules == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
        }
        #endregion
        #region Function
        public List<UtilFunctionDTO> GetListFunctions(BaseInput input)
        {
            var queryFilter = PredicateHelper.True<UtilFunction>();


            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilFunction>();

            var db = _utilFunction.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<UtilFunctionDTO>>(db);

        }
        public List<UtilFunctionDTO> GetListFunctionsByType(string type, int parentID = 0)
        {
            var db = _utilFunction.Get(s => s.FunctionType == type && (parentID == 0 || s.ParentIDFunction == parentID));

            return Mapper.Map<List<UtilFunctionDTO>>(db);

        }
        public UtilFunctionDTO InsertFunction(UtilFunctionDTO data)
        {
            ValidateInsertFunctions(data);

            var dbFunctions = Mapper.Map<UtilFunction>(data);

            //var procs = _utilRole.Get(p => p.IDRole == data.IDRole);
            dbFunctions.CreatedDate = DateTime.Now;
            dbFunctions.UpdatedDate = DateTime.Now;

            _utilFunction.Insert(dbFunctions);

            //var procLocs = procs.SelectMany(p => p.MstGenProcessSettingsLocations);
            //foreach (var procLoc in procLocs)
            //{
            //    procLoc.MstGenProcessSettings.Add(dbRole);
            //    _utilRole.Update(procLoc);
            //}

            _uow.SaveChanges();
            return data;
        }
        private void ValidateInsertFunctions(UtilFunctionDTO functionListToValidate)
        {
            var dbFunctions = _utilFunction.Get(p => p.FunctionName == functionListToValidate.FunctionName && p.ParentIDFunction ==  functionListToValidate.ParentIDFunction && p.FunctionType == functionListToValidate.FunctionType);
            if (dbFunctions.Any())
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
        }
        public UtilFunctionDTO UpdateFunction(UtilFunctionDTO data)
        {
            ValidateUpdateFunction(data);

            var dbFunctions = _utilFunction.Get(p => p.IDFunction == data.IDFunction).FirstOrDefault();

            // Mapper.Map(processSettingDto, dbRole);

            //set update time
            dbFunctions.UpdatedDate = DateTime.Now;
            dbFunctions.FunctionName = data.FunctionName;
            dbFunctions.FunctionType =data.FunctionType;
            dbFunctions.FunctionDescription=data.FunctionDescription;
            dbFunctions.ParentIDFunction = data.ParentIDFunction;


            //_utilRole.Update(dbRole);
            _uow.SaveChanges();

            return Mapper.Map<UtilFunctionDTO>(dbFunctions);
        }

        public UtilRolesFunctionDTO isUtilRolesFunctionExist(UtilRolesFunctionDTO rolesFunction)
        {
            var data = _utilRoleFunction.GetByID(rolesFunction.IDFunction, rolesFunction.IDRole);

            return Mapper.Map<UtilRolesFunctionDTO>(data);
        }

        public void UpdateRolesFunction(List<UtilRolesFunctionDTO> rolesFunctions, string username)
        {
            if (rolesFunctions != null)
            {
                foreach (var item in rolesFunctions)
                {
                    if (item.Enable)
                    {
                        // Insert
                        var roleFunction = Mapper.Map<UtilRolesFunction>(item);
                        roleFunction.CreatedBy = username;
                        roleFunction.CreatedDate = DateTime.Now;
                        roleFunction.UpdatedBy = username;
                        roleFunction.UpdatedDate = DateTime.Now;

                        _utilRoleFunction.Insert(roleFunction);
                    }
                    else
                    {
                        // Delete
                        var roleFunction = _utilRoleFunction.GetByID(item.IDFunction, item.IDRole);
                        _utilRoleFunction.Delete(roleFunction);
                    }
                }

                _uow.SaveChanges();
            }
        }

        private void ValidateUpdateFunction(UtilFunctionDTO functionListToValidate)
        {
            var dbFunctions = _utilFunction.Get(p => p.IDFunction == functionListToValidate.IDFunction).FirstOrDefault();
            if (dbFunctions == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            
            var dbFunctions2 = _utilFunction.Get(p => (p.FunctionName == functionListToValidate.FunctionName && p.IDFunction != functionListToValidate.IDFunction) && p.ParentIDFunction ==  functionListToValidate.ParentIDFunction && p.FunctionType == functionListToValidate.FunctionType);
            
            if (dbFunctions2.Any()){
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
            }
        }
        #endregion

        #region Responsibilities
        public List<UtilUserResponsibilitiesRoleViewDTO> GetListUserResponsibilitesByResponsibility(UtilSecurityResponsibilitiesInput input)
        {
            var queryFilter = PredicateHelper.True<UtilUserResponsibilitiesRoleView>();

            if (input.IDResponsibility != null)
                queryFilter = queryFilter.And(m => m.IDResponsibility == input.IDResponsibility);

            var db = _utilUserResponsibilityViewRepo.Get(queryFilter);
            return Mapper.Map<List<UtilUserResponsibilitiesRoleViewDTO>>(db);
        }

        public bool GetValidStatusResponsibility(int id, string userAd)
        {
            var queryFilter = PredicateHelper.True<UtilUserResponsibilitiesRoleView>();

            queryFilter = queryFilter.And(m => m.IDResponsibility == id);
            queryFilter = queryFilter.And(m => m.UserAD == userAd);

            var row = _utilUserResponsibilityViewRepo.Get(queryFilter).FirstOrDefault();
            var now = DateTime.Today;
            return row == null || !(row.ExpiredDate < now);
        }

        public List<UtilResponsibilityDTO> GetListResponsibilities(UtilSecurityResponsibilitiesInput input)
        {
            var queryFilter = PredicateHelper.True<UtilResponsibility>();

            if (input.IDRole > 0)
                queryFilter = queryFilter.And(m => m.IDRole == input.IDRole);

            if(!String.IsNullOrEmpty(input.ResponsibilityName))
                queryFilter = queryFilter.And(m => m.ResponsibilityName == input.ResponsibilityName);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilResponsibility>();

            var db = _utilResponsibility.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<UtilResponsibilityDTO>>(db);

        }

        public UtilResponsibilityDTO GetUtilResponsibilityById(int id)
        {
            var dbResponsibility = _utilResponsibility.GetByID(id);
            if (dbResponsibility == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<UtilResponsibilityDTO>(dbResponsibility);
        }
        public List<UtilRuleDTO> GetListRulesByResponsibility(int IDResponsibility) {
            var listIDRules = _utilResponsibilityRuleRepo.Get(w => w.IDResponsibility == IDResponsibility).Select(s => s.IDRule).ToList();
            var listRules = _utilRule.Get(w => listIDRules.Contains(w.IDRule));
            return Mapper.Map<List<UtilRuleDTO>>(listRules);
        }

        #region UtilRoles

        public int GetNewIDResponsibility()
        {
            var maxID = _utilResponsibility.Get().Max(s => s.IDResponsibility);

            return maxID + 1;
        }

        public UtilResponsibilityDTO SaveRoleResponsibility(UtilResponsibilityDTO responsibility)
        {
            var utilResponsibility = new UtilResponsibility();
            utilResponsibility.IDResponsibility = responsibility.IDResponsibility;
            utilResponsibility.IDRole = responsibility.IDRole;
            utilResponsibility.ResponsibilityName = responsibility.ResponsibilityName;
            utilResponsibility.CreatedDate = responsibility.CreatedDate;
            utilResponsibility.UpdatedDate = responsibility.UpdatedDate;
            utilResponsibility.CreatedBy = responsibility.CreatedBy;
            utilResponsibility.UpdatedBy = responsibility.UpdatedBy;

            _utilResponsibility.Insert(utilResponsibility);

            _uow.SaveChanges();

            return responsibility;
        }

        public void UpdateRoleResponsibility(UtilResponsibilityDTO responsibility)
        {
            var utilResponsibility = _utilResponsibility.Get(x => x.IDResponsibility == responsibility.IDResponsibility).First();

            if (utilResponsibility == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            utilResponsibility.IDRole = responsibility.IDRole;
            utilResponsibility.ResponsibilityName = responsibility.ResponsibilityName;
            utilResponsibility.UpdatedDate = responsibility.UpdatedDate;
            utilResponsibility.UpdatedBy = responsibility.UpdatedBy;

            _utilResponsibility.Update(utilResponsibility);

            _uow.SaveChanges();
        }
        #endregion

        public UtilRuleDTO SaveRulesResponsibilities(UtilRuleDTO rules, int IDResponsibility) {
            var dbRules = new UtilResponsibilityRule();

            dbRules.CreatedBy = rules.CreatedBy;
            dbRules.UpdatedBy = rules.UpdatedBy;

            dbRules.CreatedDate = DateTime.Now;
            dbRules.UpdatedDate = DateTime.Now;
            dbRules.IDResponsibility = IDResponsibility;
            dbRules.IDRule = rules.IDRule;
            _utilResponsibilityRuleRepo.Insert(dbRules);


            _uow.SaveChanges();
            return rules;
        }

        public UtilUsersResponsibilityDTO SaveUsersResponsibilities(UtilUsersResponsibilityDTO respon)
        {
            var dbUserRes = new UtilUsersResponsibility();

            dbUserRes.CreatedBy = respon.CreatedBy;
            dbUserRes.UpdatedBy = respon.UpdatedBy;

            dbUserRes.CreatedDate = DateTime.Now;
            dbUserRes.UpdatedDate = DateTime.Now;
            dbUserRes.IDResponsibility = respon.IDResponsibility;
            dbUserRes.UserAD = respon.UserAD;
            dbUserRes.EffectiveDate = respon.EffectiveDate;
            dbUserRes.ExpiredDate = respon.ExpiredDate;

            //_utilUserResponsibilityRepo.InsertOrUpdate(dbUserRes);
            _utilUserResponsibilityRepo.Insert(dbUserRes);


            _uow.SaveChanges();
            return respon;
        }

        public UtilUsersResponsibilityDTO UpdateUsersAd(UtilUsersResponsibilityDTO respon)
        {
            var dbUserRes = _utilUserResponsibilityRepo.GetByID(respon.IDResponsibility, respon.UserAD);

            dbUserRes.UpdatedBy = respon.UpdatedBy;
            dbUserRes.UpdatedDate = DateTime.Now;
            dbUserRes.EffectiveDate = respon.EffectiveDate;
            dbUserRes.ExpiredDate = respon.ExpiredDate;

            _utilUserResponsibilityRepo.Update(dbUserRes);
            _uow.SaveChanges();

            return respon;
        }

        public UtilUsersResponsibilityDTO InsertUsersAd(UtilUsersResponsibilityDTO respon)
        {
            var dbUserRes = new UtilUsersResponsibility();

            dbUserRes.CreatedBy = respon.CreatedBy;
            dbUserRes.UpdatedBy = respon.UpdatedBy;
            dbUserRes.CreatedDate = DateTime.Now;
            dbUserRes.UpdatedDate = DateTime.Now;
            dbUserRes.IDResponsibility = respon.IDResponsibility;
            dbUserRes.UserAD = respon.UserAD;
            dbUserRes.EffectiveDate = respon.EffectiveDate;
            dbUserRes.ExpiredDate = respon.ExpiredDate;

            //_utilUserResponsibilityRepo.InsertOrUpdate(dbUserRes);
            _utilUserResponsibilityRepo.Insert(dbUserRes);
            _uow.SaveChanges();

            return respon;

        }
        public bool DeleteAllRulesResponsibilities(int IDResponsibility) {
            try
            {
                var delete = _utilResponsibilityRuleRepo.Get(s => s.IDResponsibility == IDResponsibility);
                foreach (var res in delete) {
                    _utilResponsibilityRuleRepo.Delete(res);                    
                }

                _uow.SaveChanges();
                return true;
            }
            catch (ExceptionBase ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteAllUsersResponsibilities(string UserAD)
        {
            try
            {
                var delete = _utilUserResponsibilityRepo.Get(s => s.UserAD== UserAD);
                foreach (var res in delete)
                {
                    _utilUserResponsibilityRepo.Delete(res);
                }

                _uow.SaveChanges();
                return true;
            }
            catch (ExceptionBase ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        public UtilTransactionLogDTO GetLatestActionTransLog(string transCode, string pageName)
        {
            // Get Latest Trans Log Order By Created Date DESC, EXCEPT for SAVE
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(c => c.TransactionCode == transCode);
            queryFilter = queryFilter.And(c => c.UtilFlow.UtilFunction1.FunctionName == pageName);

            var latestTranslog = _utilTransactionLog.Get(queryFilter).OrderByDescending(c => c.CreatedDate).FirstOrDefault();

            return Mapper.Map<UtilTransactionLogDTO>(latestTranslog);

        }

        public UtilTransactionLogDTO GetLatestActionTransLogWithoutPage(string transCode)
        {
            // Get Latest Trans Log Order By Created Date DESC, EXCEPT for SAVE
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(c => c.TransactionCode == transCode);

            var latestTranslog = _utilTransactionLog.Get(queryFilter).OrderByDescending(c => c.CreatedDate).FirstOrDefault();

            return Mapper.Map<UtilTransactionLogDTO>(latestTranslog);

        }

        public UtilTransactionLogDTO GetAvailableProdCardTranssactionLog(string transCode)
        {
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(c => c.TransactionCode == transCode);
            var latestTranslog = _utilTransactionLog.Get(queryFilter).OrderByDescending(c => c.CreatedDate).FirstOrDefault();

            return Mapper.Map<UtilTransactionLogDTO>(latestTranslog);
         }

        public UtilTransactionLogDTO GetLatestActionTransLogExceptSave(string transCode, string pageName)
        {
            // Get Latest Trans Log Order By Created Date DESC, EXCEPT for SAVE
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(c => c.TransactionCode == transCode);
            queryFilter = queryFilter.And(c => c.IDFlow != 69);
            queryFilter = queryFilter.And(c => c.UtilFlow.UtilFunction.FunctionName != Enums.ButtonName.Save.ToString());
            queryFilter = queryFilter.And(c => c.UtilFlow.UtilFunction1.FunctionName == pageName);

            var latestTranslog = _utilTransactionLog.Get(queryFilter).OrderByDescending(c => c.CreatedDate).FirstOrDefault();

            return Mapper.Map<UtilTransactionLogDTO>(latestTranslog);

        }

        public bool GetTransLogSubmittedEntryVerification(string transCode)
        {
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(c => c.TransactionCode == transCode);
            queryFilter = queryFilter.And(c => c.UtilFlow.UtilFunction1.FunctionName == Enums.PageName.ProductionEntryVerification.ToString());

            var latestTranslog = _utilTransactionLog.Get(queryFilter).Any(c => c.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString());

            return latestTranslog;
        }

        public bool CheckLatestActionTransLogExceptSaveIsSubmit(string transCode, string pageName)
        {
            // Get Latest Trans Log Order By Created Date DESC, EXCEPT for SAVE
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            queryFilter = queryFilter.And(c => c.TransactionCode == transCode);
            queryFilter = queryFilter.And(c => c.UtilFlow.UtilFunction.FunctionName != Enums.ButtonName.Save.ToString());
            queryFilter = queryFilter.And(c => c.UtilFlow.UtilFunction1.FunctionName == pageName);

            var latestTranslog = _utilTransactionLog.Get(queryFilter).OrderByDescending(c => c.CreatedDate).FirstOrDefault();

            if (latestTranslog != null) {
                if (latestTranslog.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString())
                    return true;
            }
            return false;
        }



        #region Permissions
        public UtilUsersResponsibilityDTO GetUsersResponsibility(int IDResponsibility, string UserAD)
        {
            var userRes = _utilUserResponsibilityRepo.Get(w => w.UserAD == UserAD && w.IDResponsibility == IDResponsibility).FirstOrDefault();
            return Mapper.Map<UtilUsersResponsibilityDTO>(userRes);
        }
        public List<UtilResponsibilityDTO> GetListResponsibility(string userAD)
        {
            var userRes = _utilUserResponsibilityRepo.Get(w => w.UserAD == userAD).Where(m => m.EffectiveDate <= DateTime.Now.Date && m.ExpiredDate >= DateTime.Now.Date).Select(s => s.IDResponsibility).ToList();
            var userDelegations = GetActiveUtilDelegationByUserAD(userAD).Where(m => m.StartDate <= DateTime.Now.Date && m.EndDate >= DateTime.Now.Date).Select(s => s.IDResponsibility);
            userRes = userRes.Concat(userDelegations).ToList();
            var res = _utilResponsibility.Get(w => userRes.Contains(w.IDResponsibility));

            return Mapper.Map<List<UtilResponsibilityDTO>>(res);
        }

        public List<UtilDelegationDto> GetActiveUtilDelegationByUserAD(string userAD)
        {
            var queryFilter = PredicateHelper.True<UtilDelegation>();
            var currentDate = DateTime.Now;

            queryFilter = queryFilter.And(x => (x.StartDate <= currentDate) && (x.EndDate >= currentDate));
            queryFilter = queryFilter.And(x => x.UserADTo == userAD);

            var result = _utilDelegation.Get(queryFilter);

            return Mapper.Map<List<UtilDelegationDto>>(result);
        }

        public List<UtilResponsibilityDTO> GetListResponsibility2(string userAD)
        {
            //var userRes = _utilUserResponsibilityRepo.Get(w => w.UserAD == userAD).Select(s => s.IDResponsibility).ToList();
            var userRes2 = _utilUserResponsibilityRepo.Get(w => w.UserAD == userAD).ToList();
            var listRespon = userRes2.Select((s => s.IDResponsibility)).ToList();
            var res = _utilResponsibility.Get(w => listRespon.Contains(w.IDResponsibility));

            //var dbResult = (from mstMntcItemLoc in _mstItemLocationRepo.Get(m => m.LocationCode == locationCode)
            //                join mstMntcItem in _MstMntcItemRepo.Get() on mstMntcItemLoc.ItemCode equals mstMntcItem.ItemCode
            //                select new MstMntcItem
            //                {
            //                    ItemCode = mstMntcItemLoc.ItemCode,
            //                    ItemDescription = mstMntcItem.ItemDescription
            //                }).OrderBy(m => m.ItemDescription).ToList();

            var dbResultDto = (from userResdata in userRes2
                            join resData in res on userResdata.IDResponsibility equals resData.IDResponsibility
                            select new UtilResponsibilityDTO
                            {
                               IDResponsibility = resData.IDResponsibility,
                               ResponsibilityName = resData.ResponsibilityName,
                               IDRole = resData.IDRole,

                               EffectiveDate = userResdata.EffectiveDate,
                               ExpiredDate = userResdata.ExpiredDate,
                               CreatedDate = userResdata.CreatedDate,
                               CreatedBy = userResdata.CreatedBy ,
                               UpdatedDate = userResdata.UpdatedDate,
                               UpdatedBy = userResdata.UpdatedBy
                            }).ToList();

            //var res2 = _utilResponsibility.Get().ToList();
            //return Mapper.Map<List<UtilResponsibilityDTO>>(res);
            return dbResultDto;
        }

        public Responsibility GetResponsibilityPage(int IDResponsibility)
        {
            var responsibility = _utilResponsibility.GetByID(IDResponsibility);
            if (responsibility == null)
                return new Responsibility();
            var respons = _utilRolesFunctionView.Get(w => w.IDRole == responsibility.IDRole && (w.FunctionType == "Page" || w.FunctionType == "Menu" )).OrderBy(o => o.ParentIDFunction);
            var Rules = _utilResponsibilityRuleRepo.Get(p => p.IDResponsibility == responsibility.IDResponsibility);

            var listLocation = new List<ResponsibilityLocation>();
            var permission = new Responsibility();

            permission.IDResponsibility = responsibility.IDResponsibility;
            permission.ResponsibilityName = responsibility.ResponsibilityName;
            permission.Role = (int)responsibility.IDRole;
            var dicUnit = new Dictionary<string, string>();
            foreach (var u in Rules)
            {
                if (!dicUnit.ContainsKey(u.UtilRule.Location))
                    dicUnit.Add(u.UtilRule.Location, u.UtilRule.Unit);
            }

            if (Rules != null)
            {
                var lLocation = Rules.Select(s => s.UtilRule.Location).ToList();
                var location = _mstGenLocationRepo.Get(s => lLocation.Contains(s.LocationCode));
                var dicLocation = new List<string>();
                foreach(var l in location){
                    if (l.LocationCode == "SKT")
                    {
                        listLocation = new List<ResponsibilityLocation>();
                        var data = _mstGenLocationRepo.Get();
                        foreach (var d in data) {
                            var resLocSKT = new ResponsibilityLocation();
                            resLocSKT.LocationData = Mapper.Map<MstGenLocationDTO>(d); ;
                            if (dicUnit.ContainsKey(d.LocationCode))
                                resLocSKT.Units = dicUnit[d.LocationCode];

                            listLocation.Add(resLocSKT);

                        }
                        break;
                    }
                    else {
                        if(!dicLocation.Contains(l.LocationCode)){
                            listLocation.Add(new ResponsibilityLocation { LocationData = Mapper.Map<MstGenLocationDTO>(l), Units = dicUnit.ContainsKey(l.LocationCode) ? dicUnit[l.LocationCode] : null });

                            var locationChild = _mstGenLocationRepo.Get(s => s.ParentLocationCode == l.LocationCode);
                            foreach(var child in locationChild){
                                if (!dicLocation.Contains(child.LocationCode))
                                {
                                    listLocation.Add(new ResponsibilityLocation { LocationData = Mapper.Map<MstGenLocationDTO>(child), Units = dicUnit.ContainsKey(child.LocationCode) ? dicUnit[child.LocationCode] : null });

                                    var locationCC = _mstGenLocationRepo.Get(s => s.ParentLocationCode == child.LocationCode);
                                    if (locationCC != null) {
                                        foreach (var cc in locationCC)
                                        {
                                            var resLocSKT = new ResponsibilityLocation();
                                            resLocSKT.LocationData = Mapper.Map<MstGenLocationDTO>(cc); ;
                                            if (dicUnit.ContainsKey(cc.LocationCode))
                                                resLocSKT.Units = dicUnit[cc.LocationCode];

                                            listLocation.Add(resLocSKT);

                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                
                
            }
            else {
                listLocation = new List<ResponsibilityLocation>();
                var data = _mstGenLocationRepo.Get();
                foreach (var d in data)
                {
                    var resLocSKT = new ResponsibilityLocation();
                    resLocSKT.LocationData = Mapper.Map<MstGenLocationDTO>(d); ;
                    if (dicUnit.ContainsKey(d.LocationCode))
                        resLocSKT.Units = dicUnit[d.LocationCode];

                    listLocation.Add(resLocSKT);

                }

            }

            permission.Location = listLocation;
            permission.Page = new Dictionary<string, ResponsibilityPage>();
            var PageList = new Dictionary<int, string>();
            var Parent = new Dictionary<int, int>();
            foreach(var p in respons){
                if (p.ParentIDFunction == null)
                {
                    permission.Page.Add(p.FunctionName.Replace(" ", String.Empty), new ResponsibilityPage()
                    {
                        PageID = p.IDFunction,
                        Name = p.FunctionName.Replace(" ", String.Empty),
                        Child = new Dictionary<string, ResponsibilityPage>()
                    });
                    Parent.Add(p.IDFunction, 0);
                }
                else {
                    if (!PageList.ContainsKey((int)p.ParentIDFunction))
                        continue;

                    // Duplicate found when trying to find its parent by using FunctionName as Key
                    // Example => MasterData/Maintenance/Item
                    // In this foreach loop, Item will be catch as Child of Maintenance instead of MasterData/Maintenance
                    // So the fix is first find if current p match its ParentIDFunction with parent IDFunction
                    // if So, skip this process and let "else if" catch the condition where the child should be
                    // appended to parent instead their grand parent just because
                    // they have the same Key (which in this case is Name)
                    // @ticket http://tp.voxteneo.co.id/entity/7010
                    if (permission.Page.ContainsKey(PageList[(int)p.ParentIDFunction]) && permission.Page[PageList[(int)p.ParentIDFunction]].PageID == p.ParentIDFunction)
                    {
                        permission.Page[PageList[(int) p.ParentIDFunction]].Child.Add(
                            p.FunctionName.Replace(" ", String.Empty), new ResponsibilityPage()
                            {
                                PageID = p.IDFunction,
                                Name = p.FunctionName.Replace(" ", String.Empty),
                                Child = new Dictionary<string, ResponsibilityPage>()
                            });
                        
                    }
                    else if (permission.Page.ContainsKey(PageList[Parent[(int)p.ParentIDFunction]]))
                    {
                        permission.Page[PageList[Parent[(int)p.ParentIDFunction]]].Child[PageList[(int)p.ParentIDFunction]].Child.Add(p.FunctionName.Replace(" ", String.Empty), new ResponsibilityPage()
                        {
                            PageID = p.IDFunction,
                            Name = p.FunctionName.Replace(" ", String.Empty),
                            Child = new Dictionary<string, ResponsibilityPage>()
                        });
                    }
                    else {
                        permission.Page[PageList[Parent[Parent[(int)p.ParentIDFunction]]]].Child[PageList[Parent[(int)p.ParentIDFunction]]].Child[PageList[(int)p.ParentIDFunction]].Child.Add(p.FunctionName.Replace(" ", String.Empty), new ResponsibilityPage()
                        {
                            PageID = p.IDFunction,
                            Name = p.FunctionName.Replace(" ", String.Empty),
                            Child = new Dictionary<string, ResponsibilityPage>()
                        });
                    }
                    Parent.Add(p.IDFunction, (int)p.ParentIDFunction);
                }
                PageList.Add(p.IDFunction, p.FunctionName.Replace(" ", String.Empty));
            }
            return permission;
        }
        
        public List<UtilTransactionLogDTO> GetLatestTransLogExceptSaveEblekAbsenteeism(List<string> transCode, string pageName) 
        {
            var listTransLog = new List<UtilTransactionLogDTO>();

            // Get Latest Trans Log Order By Created Date DESC, EXCEPT for SAVE
            var queryFilter = PredicateHelper.True<UtilTransactionLog>();

            var firstProdEntryCode = transCode.FirstOrDefault().Substring(0, 21);

            queryFilter = queryFilter.And(c => c.TransactionCode.StartsWith(firstProdEntryCode));
            queryFilter = queryFilter.And(c => c.UtilFlow.UtilFunction.FunctionName != Enums.ButtonName.Save.ToString());
            queryFilter = queryFilter.And(c => c.UtilFlow.UtilFunction1.FunctionName == pageName);

            var dbResult = _utilTransactionLog.Get(queryFilter);

            if (dbResult.Any()) 
            {
                var dbResultDisctTransCode = dbResult.Where(c => transCode.Contains(c.TransactionCode)).Select(c => c.TransactionCode).Distinct();

                foreach (var item in dbResultDisctTransCode)
                {
                    listTransLog.Add(Mapper.Map<UtilTransactionLogDTO>(dbResult.Where(c => c.TransactionCode == item).OrderByDescending(c => c.CreatedDate).FirstOrDefault()));
                }
            }

            return listTransLog;
        }
        
        public ResponsibilityButton GetResponsibilityButton(int Role, int PageID)
        {
            var respons = _utilRolesFunctionView.Get(w => w.IDRole == Role && (w.FunctionType.ToLower() == "button" || w.FunctionType.ToLower() == "tab") && w.ParentIDFunction == PageID);
            var permission = new ResponsibilityButton();
            permission.PageID = PageID;
            permission.Button = new List<string>();
            foreach (var p in respons) {
                if (p.FunctionType.ToLower() == "tab") {
                    var buttonTab = _utilRolesFunctionView.Get(w => w.IDRole == Role && w.FunctionType.ToLower() == "button" && w.ParentIDFunction == p.IDFunction);
                    foreach (var t in buttonTab)
                    {
                        permission.Button.Add(p.FunctionName.Replace(" ", String.Empty) + "/" + t.FunctionName.Replace(" ", String.Empty));
                    }                    
                }
                else {
                    permission.Button.Add(p.FunctionName.Replace(" ", String.Empty));
                }
            }

            return permission;
        }
        #endregion


        #region Delegations
        public List<UtilDelegationDto> GetListDelegations(BaseInput input)
        {
            var queryFilter = PredicateHelper.True<UtilDelegation>();

            var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            queryFilter = queryFilter.And(c => c.EndDate >= currentDateTime);
           

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilDelegation>();

            //var db = _utilDelegation.Get(queryFilter, orderByFilter);
            var db = _utilDelegation.Get();

            var listUtilDelegationDto =  Mapper.Map<List<UtilDelegationDto>>(db);
            
            //join to table UtilResponsibility
            var listResponsibility = _utilResponsibility.Get();

            foreach (var utilDelegationDto in listUtilDelegationDto)
            {
                var responsibility =
                    listResponsibility.FirstOrDefault(c => c.IDResponsibility == utilDelegationDto.IDResponsibility);
                if (responsibility != null)
                {
                    utilDelegationDto.ResponsibilityDesc = responsibility.ResponsibilityName;
                }

            }

            return listUtilDelegationDto;

        }

        public UtilDelegationDto InsertDelegations(UtilDelegationDto data)
        {
            ValidateInsertDelegation(data);

            var dbDelegations = Mapper.Map<UtilDelegation>(data);
           
            dbDelegations.CreatedDate = DateTime.Now;
            dbDelegations.UpdatedDate = DateTime.Now;

            _utilDelegation.Insert(dbDelegations);
            
            _uow.SaveChanges();
            return data;
        }
        private void ValidateInsertDelegation(UtilDelegationDto delegation)
        {
            var dbDelegation = _utilDelegation.Get(p => p.UserADFrom == delegation.UserADFrom 
                                          && p.UserADTo == delegation.UserADTo 
                                          && p.IDResponsibility == delegation.IDResponsibility
                                          && p.StartDate == delegation.StartDate);
            if (dbDelegation.Any())
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
        }

        public UtilDelegationDto UpdateDelegation(UtilDelegationDto data)
        {
            UtilDelegation dbDelegation;

            if (data.UserADFrom != data.UserADFromOld || data.UserADTo != data.UserADToOld
                || data.IDResponsibility != data.IDResponsibilityOld || data.StartDate != data.StartDateOld)
            {
                //check first is the key already exist
                dbDelegation = _utilDelegation.Get(p => p.UserADFrom == data.UserADFrom
                                          && p.UserADTo == data.UserADTo
                                          && p.IDResponsibility == data.IDResponsibility
                                          && p.StartDate == data.StartDate).FirstOrDefault();
                if (dbDelegation != null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);

                //delete the old one
                dbDelegation = _utilDelegation.Get(p => p.UserADFrom == data.UserADFromOld
                                       && p.UserADTo == data.UserADToOld
                                       && p.IDResponsibility == data.IDResponsibilityOld
                                       && p.StartDate == data.StartDateOld).FirstOrDefault();

                if (dbDelegation == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);

                var createdBy = dbDelegation.CreatedBy;
                var createdDate = dbDelegation.CreatedDate;
 
                _utilDelegation.Delete(dbDelegation);

                //insert the new one
                var dbDelegations = Mapper.Map<UtilDelegation>(data);

                dbDelegations.CreatedBy = createdBy;
                dbDelegations.CreatedDate = createdDate;

                dbDelegations.UpdatedDate = DateTime.Now;
                
                _utilDelegation.Insert(dbDelegations);
                _uow.SaveChanges();

                return Mapper.Map<UtilDelegationDto>(dbDelegations);
            }

            else
            {
                
                dbDelegation = _utilDelegation.Get(p => p.UserADFrom == data.UserADFrom
                                                        && p.UserADTo == data.UserADTo
                                                        && p.IDResponsibility == data.IDResponsibility
                                                        && p.StartDate == data.StartDate).FirstOrDefault();

                if (dbDelegation == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);

                dbDelegation.EndDate = data.EndDate;

                //set update time
                dbDelegation.UpdatedDate = DateTime.Now;
                _uow.SaveChanges();

                return Mapper.Map<UtilDelegationDto>(dbDelegation);
            }
          
           
        }

        #endregion

        #region WorkFlow
        public List<UtilFlowDTO> GetListWorkflow(BaseInput input)
        {
            var queryFilter = PredicateHelper.True<UtilFlow>();

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<UtilFlow>();

            var db = _utilFlow.Get(queryFilter, orderByFilter);

            var listUtilFlow = Mapper.Map<List<UtilFlowDTO>>(db);

            return listUtilFlow;

        }

        public UtilFlowDTO InsertWorkflow(UtilFlowDTO data)
        {
            ValidateInsertWorkFlow(data);
            var lastID = _utilFlow.Get().OrderByDescending(d => d.IDFlow).FirstOrDefault();
            var dbWorkflow = Mapper.Map<UtilFlow>(data);
            dbWorkflow.IDFlow = lastID.IDFlow + 1;
            dbWorkflow.CreatedDate = DateTime.Now;
            dbWorkflow.UpdatedDate = DateTime.Now;
            
            _utilFlow.Insert(dbWorkflow);

            _uow.SaveChanges();
            return data;
        }
        private void ValidateInsertWorkFlow(UtilFlowDTO data)
        {
            var dbWorkflow = _utilFlow.Get(p => p.ActionButton == data.ActionButton && p.FormSource == data.FormSource && p.DestinationForm == data.DestinationForm && p.DestinationRole == data.DestinationRole);
            
            if (dbWorkflow.Any())
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
        }

        public UtilFlowDTO UpdateWorkflow(UtilFlowDTO data)
        {
            var dbWorkFlow = _utilFlow.GetByID(data.IDFlow);

            dbWorkFlow.FormSource = data.FormSource;
            dbWorkFlow.ActionButton = data.ActionButton;
            dbWorkFlow.DestinationForm = data.DestinationForm;
            dbWorkFlow.DestinationRole = data.DestinationRole;
            dbWorkFlow.MessageText = data.MessageText;
            dbWorkFlow.UpdatedBy = data.UpdatedBy;
            dbWorkFlow.UpdatedDate = DateTime.Now;

            _utilFlow.Update(dbWorkFlow);
            _uow.SaveChanges();

            return data;

        }

        #endregion
    }

    
}
