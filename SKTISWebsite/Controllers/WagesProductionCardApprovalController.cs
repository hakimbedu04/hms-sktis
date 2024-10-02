using AutoMapper;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.WagesProductionCardApproval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace SKTISWebsite.Controllers
{
    public class WagesProductionCardApprovalController : BaseController
    {
        private IApplicationService _svc;
        private IPlantWagesExecutionBLL _exePlantWagesExecutionBll;
        private IMasterDataBLL _masterDataBll;

        public WagesProductionCardApprovalController(IApplicationService svc, IPlantWagesExecutionBLL plantWagesExecutionBll, IMasterDataBLL masterDataBll)
        {
            _svc = svc;
            _exePlantWagesExecutionBll = plantWagesExecutionBll;
            _masterDataBll = masterDataBll;
            SetPage("PlantWages/Execution/ProductionCardApprovalList");
        }

        //
        // GET: /WagesProductionCardApproval/
        public ActionResult Index()
        {
            var model = new InitWagesProductionCardApprovalViewModel()
            {
                WeekList = _svc.GetWeekByYear(DateTime.Now.Year),
                Week = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now)
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult GetProductionCardApprovalList(GetProductionCardApprovalListInput input)
        {
            input.CurrentUser = GetUserName();
            var result = _exePlantWagesExecutionBll.GetProductionCardApprovalList(input);
            var viewModel = Mapper.Map<List<WagesProductionCardApprovalViewModel>>(result);
            var pageResult = new PageResult<WagesProductionCardApprovalViewModel>(viewModel);
            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetDate()
        {
            var endClosingPayroll = _masterDataBll.GetClosingPayrollAfterTodayDateTime(DateTime.Now);
            var startClosingPayroll = _masterDataBll.GetClosingPayrollBeforeTodayDateTime(endClosingPayroll);
            var week = new MstGenWeekDTO();
            week.StartDate = startClosingPayroll;
            week.EndDate = endClosingPayroll;

            return Json(week, JsonRequestBehavior.AllowGet);
        }
	}
}