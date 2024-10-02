using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using SKTISWebsite.Models.PlanningReportProductionTarget;
using SpreadsheetLight;
using SKTISWebsite.Models.Common;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.Core;

namespace SKTISWebsite.Controllers
{
    public class PlanningReportProductionTargetController : BaseController
    {
        private IPlanningBLL _planningBll;
        private IApplicationService _applicationService;
        private IMasterDataBLL _masterDataBll;
        private IVTLogger _vtlogger;

        public PlanningReportProductionTargetController(IVTLogger vtlogger, IPlanningBLL planning, IApplicationService applicationService, IMasterDataBLL masterDataBll)
        {
            _planningBll = planning;
            _applicationService = applicationService;
            _masterDataBll = masterDataBll;
            _vtlogger = vtlogger;
            SetPage("Productionplanning/Report/SummaryDailyProductionTarget");
        }

        // GET: PlanningReportProductionTargetController
        public ActionResult Index()
        {
            var init = new InitPlanningReportProductionTargetViewModel
            {
                LocationSelectList = _applicationService.GetLocationCodeCompat(),
                YearSelectList = _applicationService.GetGenWeekYears(),
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                TodayDate = DateTime.Now.ToShortDateString()
            };
            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDate(int year, int week)
        {
            var date = _masterDataBll.GetWeekByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateList(int year, int week, string date)
        {
            var dateList = _masterDataBll.GetDateByWeek(year, week);
            return Json(dateList.Select(m => m.Date.ToString(Constants.DefaultDateOnlyFormat)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPlanningReportProductionTargets(GetPlanningReportProductionTargetInput criteria)
        {
            try
            {
                var masterLists = _planningBll.GetPlanningReportProductionTargets(criteria);
                var viewModel = Mapper.Map<List<PlanningReportProductionTargetViewModel>>(masterLists);
                var pageResult = new PageResult<PlanningReportProductionTargetViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Get Planning Report Production Targets");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetPlanningReportProductionTargetInput input)
        {
            try
            {
                var listDatas = _planningBll.GetPlanningReportProductionTargets(input);

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.PlanningExcelTemplate.ReportProductionTarget + ".xlsx";
                var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {

                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + input.Year);
                    slDoc.SetCellValue(4, 2, ": " + input.Location);
                    slDoc.SetCellValue(5, 2, ": " + input.Decimal);
                    slDoc.SetCellValue(3, 6, ": " + input.Week);

                    // set header date list
                    var dateList = _masterDataBll.GetDateByWeek(input.Year, input.Week);
                    if (dateList.Any())
                    {
                        for (var i = 0; i < 7; i++)
                            slDoc.SetCellValue(8, i + 3, dateList[i].ToShortDateString());
                    }

                    //row values
                    var iRow = 9;

                    double totalTarget1 = 0;
                    double totalTarget2 = 0;
                    double totalTarget3 = 0;
                    double totalTarget4 = 0;
                    double totalTarget5 = 0;
                    double totalTarget6 = 0;
                    double totalTarget7 = 0;
                    double totalTarget = 0;
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    foreach (var data in listDatas)
                    {
                        //if (iRow % 2 == 0)
                        //{
                        //    style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                        //}

                        double total = 0;

                        slDoc.SetCellValue(iRow, 1, data.BrandCode);
                        slDoc.SetCellValue(iRow, 2, data.LocationCode);
                        if (data.TargetManual1.HasValue)
                        {
                            slDoc.SetCellValue(iRow, 3, data.TargetManual1.Value);
                            total += data.TargetManual1.Value;
                            totalTarget1 += data.TargetManual1.Value;
                        }
                        if (data.TargetManual2.HasValue)
                        {
                            slDoc.SetCellValue(iRow, 4, data.TargetManual2.Value);
                            total += data.TargetManual2.Value;
                            totalTarget2 += data.TargetManual2.Value;
                        }
                        if (data.TargetManual3.HasValue)
                        {
                            slDoc.SetCellValue(iRow, 5, data.TargetManual3.Value);
                            total += data.TargetManual3.Value;
                            totalTarget3 += data.TargetManual3.Value;
                        }
                        if (data.TargetManual4.HasValue)
                        {
                            slDoc.SetCellValue(iRow, 6, data.TargetManual4.Value);
                            total += data.TargetManual4.Value;
                            totalTarget4 += data.TargetManual4.Value;
                        }
                        if (data.TargetManual5.HasValue)
                        {
                            slDoc.SetCellValue(iRow, 7, data.TargetManual5.Value);
                            total += data.TargetManual5.Value;
                            totalTarget5 += data.TargetManual5.Value;
                        }
                        if (data.TargetManual6.HasValue)
                        {
                            slDoc.SetCellValue(iRow, 8, data.TargetManual6.Value);
                            total += data.TargetManual6.Value;
                            totalTarget6 += data.TargetManual6.Value;
                        }
                        if (data.TargetManual7.HasValue)
                        {
                            slDoc.SetCellValue(iRow, 9, data.TargetManual7.Value);
                            total += data.TargetManual7.Value;
                            totalTarget7 += data.TargetManual7.Value;
                        }
                        totalTarget += total;
                        slDoc.SetCellValue(iRow, 10, Math.Round(total, input.Decimal));
                        slDoc.SetCellStyle(iRow, 1, iRow, 10, style);
                        iRow++;
                    }

                    slDoc.SetCellValue(iRow, 1, "Total");
                    slDoc.MergeWorksheetCells(iRow, 1, iRow, 2);
                    slDoc.SetCellValue(iRow, 3, Math.Round(totalTarget1, input.Decimal));
                    slDoc.SetCellValue(iRow, 4, Math.Round(totalTarget2, input.Decimal));
                    slDoc.SetCellValue(iRow, 5, Math.Round(totalTarget3, input.Decimal));
                    slDoc.SetCellValue(iRow, 6, Math.Round(totalTarget4, input.Decimal));
                    slDoc.SetCellValue(iRow, 7, Math.Round(totalTarget5, input.Decimal));
                    slDoc.SetCellValue(iRow, 8, Math.Round(totalTarget6, input.Decimal));
                    slDoc.SetCellValue(iRow, 9, Math.Round(totalTarget7, input.Decimal));
                    slDoc.SetCellValue(iRow, 10, Math.Round(totalTarget, input.Decimal));
                    style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightSlateGray,
                        System.Drawing.Color.LightSlateGray);
                    slDoc.SetCellStyle(iRow, 1, iRow, 10, style);

                    var slSheetProtection = new SLSheetProtection
                    {
                        AllowInsertRows = false,
                        AllowInsertColumns = false,
                        AllowDeleteRows = false,
                        AllowDeleteColumns = false,
                        AllowFormatCells = true,
                        AllowFormatColumns = true,
                        AllowFormatRows = true,
                        AllowAutoFilter = true,
                        AllowSort = true
                    };


                    slDoc.ProtectWorksheet(slSheetProtection);
                    slDoc.AutoFitColumn(1, 6);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "PlanningReportProductionTarget_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Excel - Planning Report Production Targets");
                return null;
            }
        }
    }
}