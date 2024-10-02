using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.UtilSecurityDelegations;
using SKTISWebsite.Models.UtilSecurityFunctions;
using SKTISWebsite.Models.UtilSecurityRules;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class UtilSecurityDelegationsController : BaseController
    {
        private IApplicationService _svc;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IExeReportBLL _exeReportBLL;
        private IUserBLL _userBll;
        public UtilSecurityDelegationsController(IApplicationService applicationService, 
            IGeneralBLL generalBll, IUtilitiesBLL utilitiesBLL, IExeReportBLL exeReportBLL,
            IUserBLL userBll)
        {
            _svc = applicationService;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            _exeReportBLL = exeReportBLL;
            _userBll = userBll;
            SetPage("Utilities/Security/Delegation");
        }

        // GET: /UtilSecurityDelegations/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDelegations(BaseInput criteria)
        {
            var roles = _utilitiesBLL.GetListDelegations(criteria);
            var viewModel = Mapper.Map<List<UtilSecurityDelegationsViewModel>>(roles);
            var pageResult = new PageResult<UtilSecurityDelegationsViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetListUserAd()
        {
            var model = _userBll.GetAllLoginActive();
            var userAd = model.Select(m => m.UserAD).ToList();
            //var userAd = new SelectList(model, "UserAD", "UserAD");
            return Json(userAd, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListUserAdForEdit()
        {
            var model = _userBll.GetAllLoginActive();
            var userAd = new SelectList(model, "UserAD", "UserAD");
            return Json(userAd, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListResponsibility(string user)
        {
            var result  = _utilitiesBLL.GetListResponsibility(user);

            var model = new SelectList(result, "IDResponsibility","ResponsibilityName");

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetResponsibilityDetail(int idResponsibility)
        {
            var model = _utilitiesBLL.GetUtilResponsibilityById(idResponsibility);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveAllDelegations(InsertUpdateData<UtilSecurityDelegationsViewModel> bulkData)
        {
            #region New
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var delegation = Mapper.Map<UtilDelegationDto>(bulkData.New[i]);

                    //set createdby and updatedby
                    delegation.CreatedBy = GetUserName();
                    delegation.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.InsertDelegations(delegation);
                        bulkData.New[i] = Mapper.Map<UtilSecurityDelegationsViewModel>(item);
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.delegation_exception);
                    }
                }
            }
            #endregion

            // Update data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    var delegation = Mapper.Map<UtilDelegationDto>(bulkData.Edit[i]);

                    //set updatedby
                    delegation.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.UpdateDelegation(delegation);
                        bulkData.Edit[i] = Mapper.Map<UtilSecurityDelegationsViewModel>(item);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        throw ex;
                    }
                }
            }

            return Json(bulkData);
        }
	}
}