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
using SKTISWebsite.Models.UtilWorkflowFlow;
using SKTISWebsite.Models.UtilSecurityRules;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class UtilWorkflowFlowController : BaseController
    {
        private IApplicationService _svc;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IExeReportBLL _exeReportBLL;
        private IUserBLL _userBll;
        private IVTLogger _VTLogger;
        public UtilWorkflowFlowController(IApplicationService applicationService,
            IGeneralBLL generalBll, IUtilitiesBLL utilitiesBLL, IExeReportBLL exeReportBLL, IVTLogger VTLogger,
            IUserBLL userBll)
        {
            _svc = applicationService;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            _exeReportBLL = exeReportBLL;
            _userBll = userBll;
            _VTLogger = VTLogger;
            SetPage("Utilities/Security/Workflows");
        }

        //
        // GET: /UtilWorkflowFlow/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetWorkflow(BaseInput criteria)
        {
            var roles = _utilitiesBLL.GetListWorkflow(criteria);
            var viewModel = Mapper.Map<List<UtilWorkflowFlowViewModel>>(roles);
            var pageResult = new PageResult<UtilWorkflowFlowViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public JsonResult GetListFunction(string functionType, int parentID = 0)
        {
            var model = _utilitiesBLL.GetListFunctionsByType(functionType, parentID).OrderBy(o => o.FunctionName);

            var selectList = new SelectList(model, "IDFunction", "FunctionName");

            return Json(selectList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListRoles()
        {
            BaseInput criteria = new BaseInput();
            var model = _utilitiesBLL.GetListRoles(criteria).OrderBy(o => o.RolesCode);

            var selectList = new SelectList(model, "IDRole", "RolesCode");

            return Json(selectList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Save all new and updated general list item
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllFlow(InsertUpdateData<UtilWorkflowFlowViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var flowList = Mapper.Map<UtilFlowDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    flowList.CreatedBy = GetUserName();
                    flowList.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.InsertWorkflow(flowList);
                        bulkData.New[i] = Mapper.Map<UtilWorkflowFlowViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            }

            // Update data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    var flowList = Mapper.Map<UtilFlowDTO>(bulkData.Edit[i]);

                    //set updatedby
                    flowList.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.UpdateWorkflow(flowList);
                        bulkData.Edit[i] = Mapper.Map<UtilWorkflowFlowViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        _VTLogger.Err(ex);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                        return Json(bulkData);
                    }
                    catch (Exception ex)
                    {
                        _VTLogger.Err(ex);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        //bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        bulkData.Edit[i].Message = "Database Constraint Violation";
                        //throw ex;
                        return Json(bulkData);
                    }
                }
            }

            return Json(bulkData);
        }
	}
    
}