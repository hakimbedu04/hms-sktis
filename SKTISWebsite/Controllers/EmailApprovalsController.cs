using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office.CustomUI;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;

namespace SKTISWebsite.Controllers
{
    public class EmailApprovalsController : Controller
    {
        private IGeneralBLL _generalBll;
        private IEmailApprovalsBLL _emailApprovals;
        private IUtilitiesBLL _utilitiesBll;
        private ITPOFeeBLL _tpoFeeBll;

        public EmailApprovalsController(IGeneralBLL generalBll, IEmailApprovalsBLL emailApprovals, IUtilitiesBLL utilitiesBll, ITPOFeeBLL tpoFeeBll)
        {
            _generalBll = generalBll;
            _emailApprovals = emailApprovals;
            _utilitiesBll = utilitiesBll;
            _tpoFeeBll = tpoFeeBll;
        }
        //
        // GET: /EmailApprovals/
        public JsonResult ApprovalLevel1Result()
        {
            var transactionId = Request.Form["TransactionID"];
            var result = new EmailApprovalLogDto();
            //var email = Request.Form["email"];
            var response = Request.Form["Response"];
            
            //var ActionName = Request.Form["ActionName"];
            //var StatusApproval = Request.Form["StatusApproval"];
            //var Reason = Request.Form["Reason"];
            string[] id = Regex.Split(transactionId, "/");
            var parentLocation = id[0];
            var kpsyear = Convert.ToInt32(id[1]);
            var kpsweek = Convert.ToInt32(id[2]);
            var approvedBy = id[3].Replace("~", "\\");
            var actionButton = response == "Approved"
                ? Enums.ButtonName.Approve.ToString()
                : Enums.ButtonName.Revise.ToString();
            var idrole = _utilitiesBll.GetRoleByRoleCode("RM").IDRole;
            var listTransCode = _emailApprovals.GetUtilTransactionLogDtos(parentLocation, kpsyear, kpsweek);
            try
            {
                foreach (var utilTransactionLogDto in listTransCode)
                {
                    
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.ApprovalPage.ToString(),
                        ActionButton = actionButton,
                        UserName = approvedBy,
                        TransactionCode = utilTransactionLogDto.TPOFeeCode,
                        ActionTime = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        IDRole = idrole
                    });
                }
                result.Status = 1;
                result.Reason = string.Empty;
            }
            catch (Exception ex)
            {
                result.Status = 0;
                result.Reason = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApprovalLevel2Result()
        {
            var transactionId = Request.Form["TransactionID"];
            var result = new EmailApprovalLogDto();
            var response = Request.Form["Response"];
             //2018/21/PMI\yprobori/42
            string[] id = Regex.Split(transactionId, "/");
            var parentLocation = id[0];
            var kpsyear = Convert.ToInt32(id[1]);
            var kpsweek = Convert.ToInt32(id[2]);
            var approvedBy = id[3].Replace("~", "\\");
            var actionButton = response == "Approved"
                ? Enums.ButtonName.Authorize.ToString()
                : Enums.ButtonName.Revise.ToString();
            var idrole = _utilitiesBll.GetRoleByRoleCode("SKTHD").IDRole;
            var listTransCode = _emailApprovals.GetUtilTransactionLogDtos(null, kpsyear, kpsweek);
            try
            {
                foreach (var utilTransactionLogDto in listTransCode)
                {
                    _tpoFeeBll.AuthorizeActual(utilTransactionLogDto.TPOFeeCode, approvedBy);
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.ApprovalPage.ToString(),
                        ActionButton = actionButton,
                        UserName = approvedBy,
                        TransactionCode = utilTransactionLogDto.TPOFeeCode,
                        ActionTime = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        IDRole = idrole
                    });
                }
                result.Status = 1;
                result.Reason = string.Empty;
            }
            catch (Exception ex)
            {
                result.Status = 0;
                result.Reason = ex.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ReviseApprovalResult()
        //{
        //    var transactionId = Request.Form["TransactionID"];
        //    var result = new EmailApprovalLogDto();
        //    var response = Request.Form["Response"];
        //    //2018/21/PMI\yprobori/42
        //    string[] id = Regex.Split(transactionId, "/");
        //    var parentLocation = id[0];
        //    var kpsyear = Convert.ToInt32(id[1]);
        //    var kpsweek = Convert.ToInt32(id[2]);
        //    var approvedBy = id[3].Replace("~", "\\");
        //    var actionButton = response == "Approved"
        //        ? Enums.ButtonName.Authorize.ToString()
        //        : Enums.ButtonName.Revise.ToString();
        //    var idrole = _utilitiesBll.GetRoleByRoleCode("SKTHD").IDRole;
        //    var listTransCode = _emailApprovals.GetUtilTransactionLogDtos(null, kpsyear, kpsweek);


        //    var lastTransactionFlow = _utilitiesBll.GetLatestActionTransLogWithoutPage(data.TPOFeeCode);

        //    if (lastTransactionFlow.IDFlow > 43)
        //    {
        //        //Revise from ACCT
        //        _generalBll.ExeTransactionLog(new TransactionLogInput()
        //        {
        //            page = Enums.PageName.TPOFeeAP.ToString(),
        //            ActionButton = Enums.ButtonName.Revise.ToString(),
        //            UserName = GetUserName(),
        //            TransactionCode = data.TPOFeeCode,
        //            ActionTime = DateTime.Now,
        //            TransactionDate = DateTime.Now,
        //            IDRole = CurrentUser.Responsibility.Role
        //        });
        //    }
        //    else
        //    {
        //        //Revise from SKTHD
        //        _generalBll.ExeTransactionLog(new TransactionLogInput()
        //        {
        //            page = Enums.PageName.ApprovalPage.ToString(),
        //            ActionButton = Enums.ButtonName.Revise.ToString(),
        //            UserName = GetUserName(),
        //            TransactionCode = data.TPOFeeCode,
        //            ActionTime = DateTime.Now,
        //            TransactionDate = DateTime.Now,
        //            IDRole = CurrentUser.Responsibility.Role
        //        });
        //    }
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

    }
}