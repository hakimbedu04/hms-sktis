using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ReportPlan;

namespace SKTISWebsite.Controllers
{
    public class ReportPlanController : BaseController
    {
        private IPlanningBLL _planningBll;

        public ReportPlanController(IPlanningBLL planningBll)
        {
            _planningBll = planningBll;
        }

        // GET: ReportPlan
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult ViewReport(int id)
        {

            var report = _planningBll.GetReportTableau(id);

            var model = Mapper.Map<ReportPlanViewReportViewModel>(report);

            return View("ViewReport", model);
        }

        [HttpPost]
        public JsonResult GetReportPlan(BaseInput input)
        {
            var pageResult = new PageResult<ReportPlanViewModel>();
            var reportPlans = _planningBll.GetReportTableau(Enums.ReportTableau.ReportPlan);
            pageResult.TotalRecords = reportPlans.Count;
            pageResult.TotalPages = (reportPlans.Count / input.PageSize) +
                                    (reportPlans.Count % input.PageSize != 0 ? 1 : 0);
            var result = reportPlans.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            pageResult.Results = Mapper.Map<List<ReportPlanViewModel>>(result);

            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Insert(ReportPlanModel input)
        {

            if (!ModelState.IsValid)
            {
                input.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                input.Message = "Report name & report url cannot empty";

                return Json(input);
            }

            if ( !Uri.IsWellFormedUriString(input.ReportUrl, UriKind.Absolute) )
            {
                input.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                input.Message = "Report url must be a valid url";

                return Json(input);
            }

            var model = Mapper.Map<MstTableauReportDto>(input);
            model.ReportType = Enums.ReportTableau.ReportPlan.ToString();
            model.CreatedBy = GetUserName();
            model.CreatedDate = DateTime.Now;

            try
            {
                _planningBll.SaveReport(model);
                input.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
            }
            catch (ExceptionBase ex)
            {
                input.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                input.Message = ex.Message;
            }
            catch (Exception ex)
            {
                input.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                input.Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
            }

            return Json(input);
        }

        [HttpPost]
        public JsonResult Delete(List<ReportPlanViewModel> models)
        {
            foreach (var data in models)
            {
                if (data.Checkbox == true)
                {
                    try
                    {
                        var model = Mapper.Map<MstTableauReportDto>(data);
                        _planningBll.DeleteReport(model);
                        data.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        data.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        data.Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        data.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        data.Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                    }
                }
            }
            return Json(models);
        }



    }
}