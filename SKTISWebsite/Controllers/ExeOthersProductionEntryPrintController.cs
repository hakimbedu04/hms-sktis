using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.ExeOthersProductionEntryPrint;
using SKTISWebsite.Code;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.Contracts;
using System.IO;
using HMS.SKTIS.Core;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using AutoMapper;

namespace SKTISWebsite.Controllers
{
    public class ExeOthersProductionEntryPrintController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionOtherBLL _exe;
        private IVTLogger _vtlogger;

        public ExeOthersProductionEntryPrintController(IApplicationService applicationService, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IExecutionOtherBLL executionOtherBll)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _exe = executionOtherBll;
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/Plant/ProductionEntryPrint");
        }

        // GET: /ExeOthersProductionEntryPrint/
        public ActionResult Index()
        {
            var init = new InitExeOthersProductionEntryPrintModel()
            {
                LocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now)
            };
            return View("Index", init);
        }

        public ActionResult GetPrint(GetExeOthersProductionEntryPrintInput input)
        {
            try
            {
                var data = _exe.GetExeProductionEntryPrintData(input);

                return PartialView("PrintPartial", Mapper.Map<List<PrintDataModel>>(data));
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Exe Others Production Entry Print - GetPrint");
                return null;
            }

        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            try
            {
                var model = _svc.GetUnitCodeSelectListByLocationCode(locationCode);

                model =
                    new SelectList(
                        model.Where(x => x.Value.ToUpper() != Enums.UnitCodeDefault.MTNC.ToString())
                            .OrderBy(o => o.Value)
                            .ToList(), "Value", "Text");

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode }, "Get Unit Code Select List By Location Code");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetBrandFromExePlantProductionEntryVerificationByLocation(string locationCode, int week, int year)
        {
            try
            {
                var input = new GetExeOthersProductionEntryPrintInput()
                {
                    Year = year,
                    Week = week,
                    LocationCode = locationCode
                };

                List<string> BrandCodes = _exe.GetBrandCodeByInput(input);
                return Json(BrandCodes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, week, year }, "Exe Others Production Entry Print Page");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetShiftByLocation(string locationCode)
        {
            var shifts = _svc.GetShiftByLocationCode(locationCode);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcessByLocation(string locationCode)
        {
            var model = _svc.GetAllProcessGroupTPKPlant(locationCode);
            var modelfiltered = model.Where(m => m.Text != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily));
            return Json(modelfiltered, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGoupByParam(string locationCode, string unitCode, string processCode)
        {
            try
            {
                var group = _masterDataBLL.GetGroupCodePlantProductionByProcessLocationAndUnit(processCode, locationCode,
                    unitCode);
                return Json(group, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, processCode }, "Exe Others Production Entry Print Page - GetGoupByParam");
                return null;
            }
        }

	}
}