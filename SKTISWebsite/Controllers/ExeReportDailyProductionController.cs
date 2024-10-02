using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExeReportDailyProductionAchievement;
using System.IO;
using SpreadsheetLight;
using HMS.SKTIS.Utils;
using Color = System.Drawing.Color;

namespace SKTISWebsite.Controllers
{
    public class ExeReportDailyProductionController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IPlanningBLL _planningBll;
        private IExecutionPlantBLL _executionPlantBll;
        private IExeReportBLL _exeReportBLL;
        private IVTLogger _vtlogger;

        public ExeReportDailyProductionController(IApplicationService applicationService, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IPlanningBLL planningBll, IExecutionPlantBLL executionPlantBll, IExeReportBLL exeReportBLL)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _planningBll = planningBll;
            _executionPlantBll = executionPlantBll;
            _exeReportBLL = exeReportBLL;
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/Report/DailyProductionAchievement");
        }

        //
        // GET: /ExeReportDailyProduction/
        public ActionResult Index(string param1, int? param2, int? param3, int? param4)
        {
            if (param4.HasValue) setResponsibility(param4.Value);
            var init = new InitExeReportDailyProductionAchievementViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                LocationSelectList = _svc.GetLocationNamesLookupList(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                Param1LocationCode = param1,
                Param2Year = param2,
                Param3Week = param3
            };
            return View("index", init);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateList(int year, int week)
        {
            var dateList = _masterDataBLL.GetDateByWeek(year, week);
            return Json(dateList.Select(m => m.Date.ToString(Constants.DefaultDateOnlyFormat)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetExeReportsDailyProduction(GetExeReportDailyProductionInput input)
        {
            var model = GetExeReportDailyAchievement(input);

            return PartialView("_GridIReportDailyProductionIndex", model);
        }

        private ExeReportDailyProdAchievementFinalViewModel GetExeReportDailyAchievement(GetExeReportDailyProductionInput input) 
        {
            try
            {
                var dailyReport = _exeReportBLL.GetExeReportProductionDailyAchievement(input);
                var detailDto =
                    Mapper.Map<List<ExeReportingDailyProductionAchievementSKTBrandCodeViewModel>>(dailyReport);

                var model = new ExeReportDailyProdAchievementFinalViewModel
                {
                    ListPerSKTBrandCode = detailDto,
                    TotalMondayHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(
                            dailyReport.Sum(c => c.TotalMonday == null ? 0 : c.TotalMonday.Value)),
                    TotalTuesdayHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(
                            dailyReport.Sum(c => c.TotalTuesday == null ? 0 : c.TotalTuesday.Value)),
                    TotalWednesdayHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(
                            dailyReport.Sum(c => c.TotalWednesday == null ? 0 : c.TotalWednesday.Value)),
                    TotalThursdayHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(
                            dailyReport.Sum(c => c.TotalThursday == null ? 0 : c.TotalThursday.Value)),
                    TotalFridayHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(
                            dailyReport.Sum(c => c.TotalFriday == null ? 0 : c.TotalFriday.Value)),
                    TotalSaturdayHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(
                            dailyReport.Sum(c => c.TotalSaturday == null ? 0 : c.TotalSaturday.Value)),
                    TotalSundayHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(
                            dailyReport.Sum(c => c.TotalSunday == null ? 0 : c.TotalSunday.Value)),
                    TotalTotalHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.TotalTotal)),
                    TotalPlanningHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.TotalPlanning)),
                    TotalVarianceStickHandRolled =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.TotalVarianceStick)),
                    TotalVariancePercentHandRolled =
                        GenericHelper.ConvertDecimalToString2FormatDecimalCommaThousand(
                            dailyReport.Sum(c => c.TotalVariancePercent)),
                    TotalReliabilityPercentHandRolled =
                        GenericHelper.ConvertDecimalToString2FormatDecimalCommaThousand(
                            dailyReport.Sum(c => c.TotalReliabilityPercent)),
                    TotalPackageHandRolled =
                        GenericHelper.ConvertFloatToString2FormatDecimal(dailyReport.Sum(c => c.TotalPackage)),
                    TotalTWHEqvHandRolled =
                        GenericHelper.ConvertDecimalToString2FormatDecimalCommaThousand(
                            dailyReport.Sum(c => c.TotalTWHEqv)),

                    //SumAllTotalHandRole = GenericHelper.ConvertIntToString2FormatDecimal(dailyReport.Sum(c => c.SumTotalAllDay)),
                    //SumAllTotalHandRolePlanning = GenericHelper.ConvertDoubleToString2FormatDecimal(dailyReport.Sum(c => c.SumAllTpkValue == null ? 0 : Math.Round(c.SumAllTpkValue.Value, 2))),
                    //SumAllTotalHandRoleVarianceStick = GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(dailyReport.Sum(c => c.SumAllVarianStick == null ? 0 : Math.Round(c.SumAllVarianStick.Value, 2))),
                    //SumAllTotalHandRoleVariancePercen = GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(dailyReport.Sum(c => c.SumAllVarianPercen == null ? 0 : Math.Round(c.SumAllVarianPercen.Value, 2))),
                    //SumAllTotalHandRoleReliability = GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(dailyReport.Sum(c => c.SumAllReliabilty == null ? 0 : Math.Round(c.SumAllReliabilty.Value, 2))),
                    //SumAllTotalHandRolePackage = GenericHelper.ConvertFloatToString2FormatDecimal(dailyReport.Sum(c => c.SumAllPackage)),

                    TotalAllCumulativeStickMonday =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.CumulativeTotalMonday)),
                    TotalAllCumulativeStickTuesday =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.CumulativeTotalTuesday)),
                    TotalAllCumulativeStickWednesday =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.CumulativeTotalWednesday)),
                    TotalAllCumulativeStickThursday =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.CumulativeTotalThursday)),
                    TotalAllCumulativeStickFriday =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.CumulativeTotalFriday)),
                    TotalAllCumulativeStickSaturday =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.CumulativeTotalSaturday)),
                    TotalAllCumulativeStickSunday =
                        GenericHelper.ConvertDoubleToStringCommaThousand(dailyReport.Sum(c => c.CumulativeTotalSunday)),

                    TotalAllCumulativePercentMonday =
                        dailyReport.Sum(c => c.TotalPlanning) == 0
                            ? "0.00"
                            : GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(
                                (dailyReport.Sum(c => c.CumulativeTotalMonday)/dailyReport.Sum(c => c.TotalPlanning))*
                                100),
                    TotalAllCumulativePercentTuesday =
                        dailyReport.Sum(c => c.TotalPlanning) == 0
                            ? "0.00"
                            : GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(
                                (dailyReport.Sum(c => c.CumulativeTotalTuesday)/dailyReport.Sum(c => c.TotalPlanning))*
                                100),
                    TotalAllCumulativePercentWednesday =
                        dailyReport.Sum(c => c.TotalPlanning) == 0
                            ? "0.00"
                            : GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(
                                (dailyReport.Sum(c => c.CumulativeTotalWednesday)/dailyReport.Sum(c => c.TotalPlanning))*
                                100),
                    TotalAllCumulativePercentThursday =
                        dailyReport.Sum(c => c.TotalPlanning) == 0
                            ? "0.00"
                            : GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(
                                (dailyReport.Sum(c => c.CumulativeTotalThursday)/dailyReport.Sum(c => c.TotalPlanning))*
                                100),
                    TotalAllCumulativePercentFriday =
                        dailyReport.Sum(c => c.TotalPlanning) == 0
                            ? "0.00"
                            : GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(
                                (dailyReport.Sum(c => c.CumulativeTotalFriday)/dailyReport.Sum(c => c.TotalPlanning))*
                                100),
                    TotalAllCumulativePercentSaturday =
                        dailyReport.Sum(c => c.TotalPlanning) == 0
                            ? "0.00"
                            : GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(
                                (dailyReport.Sum(c => c.CumulativeTotalSaturday)/dailyReport.Sum(c => c.TotalPlanning))*
                                100),
                    TotalAllCumulativePercentSunday =
                        dailyReport.Sum(c => c.TotalPlanning) == 0
                            ? "0.00"
                            : GenericHelper.ConvertDoubleToString2FormatDecimalCommaThousand(
                                (dailyReport.Sum(c => c.CumulativeTotalSunday)/dailyReport.Sum(c => c.TotalPlanning))*
                                100)

                    //SumAllTotalHandRoleTheoriticalWhEqv = detailDto.Sum(c => c.SumAllPackage)

                };

                return model;
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Exe Report Daily Production - GetExeReportDailyAchievement");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetExeReportDailyProductionInput input)
        {
            try
            {
                // Get source data report
                var dailyReport = GetExeReportDailyAchievement(input);
                var listSKTBrandCode = dailyReport.ListPerSKTBrandCode;

                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == input.LocationCode)
                    {
                        locationCompat = item.Text;
                    }
                }


                var ms = new MemoryStream();
                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";

                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExeReportDailyProductionAchievement + ".xlsx";
                var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;
                    style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray,
                        System.Drawing.Color.LightGray);
                    style.Alignment.Vertical = VerticalAlignmentValues.Center;

                    SLStyle styleColor = slDoc.CreateStyle();
                    styleColor.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    styleColor.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    styleColor.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    styleColor.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    styleColor.Font.FontName = "Calibri";
                    styleColor.Font.FontSize = 10;
                    styleColor.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray,
                        System.Drawing.Color.LightGray);
                    styleColor.Alignment.Vertical = VerticalAlignmentValues.Center;

                    SLStyle styleTotal = slDoc.CreateStyle();
                    styleTotal.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Font.FontName = "Calibri";
                    styleTotal.Font.FontSize = 11;
                    styleTotal.Font.Bold = true;
                    styleTotal.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Gray, System.Drawing.Color.Gray);

                    SLStyle styleCenterAlignment = slDoc.CreateStyle();
                    styleCenterAlignment.Alignment.Horizontal = HorizontalAlignmentValues.Center;

                    SLStyle styleCenterAlignmentTotal = slDoc.CreateStyle();
                    styleCenterAlignmentTotal.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                    styleCenterAlignmentTotal.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    styleCenterAlignmentTotal.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    styleCenterAlignmentTotal.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    styleCenterAlignmentTotal.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    styleCenterAlignmentTotal.Font.FontName = "Calibri";
                    styleCenterAlignmentTotal.Font.FontSize = 11;
                    styleCenterAlignmentTotal.Font.Bold = true;
                    styleCenterAlignmentTotal.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.Gray,
                        System.Drawing.Color.Gray);

                    SLStyle styleVerticalCenterAlignment = slDoc.CreateStyle();
                    styleCenterAlignment.Alignment.Vertical = VerticalAlignmentValues.Center;

                    //row values
                    var iRow = 11;

                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    slDoc.SetCellValue(3, 3, input.FilterLocationName);
                    slDoc.SetCellValue(4, 2, ": " + input.YearFrom);
                    slDoc.SetCellValue(5, 2, ": " + input.WeekFrom);

                    foreach (var item in listSKTBrandCode)
                    {
                        slDoc.SetCellValue(iRow, 1, item.SKTBrandCode);
                        var mergeSktBrandCode = iRow + (item.ListPerSKTBrandCode.Count() + item.CountSKTBrandCode) - 1;
                        slDoc.MergeWorksheetCells(iRow, 1, mergeSktBrandCode, 1);
                        slDoc.SetCellStyle(iRow, 1, mergeSktBrandCode, 17, style);
                        var iRowSKTBrandCode = iRow;
                        foreach (var itemDetail in item.ListPerSKTBrandCode)
                        {
                            slDoc.SetCellValue(iRowSKTBrandCode, 2, itemDetail.BrandCode);
                            var mergeBrandCode = iRowSKTBrandCode + itemDetail.CountBrandCode - 1;
                            slDoc.MergeWorksheetCells(iRowSKTBrandCode, 2, mergeBrandCode, 2);
                            var iRowBrandCode = iRowSKTBrandCode;
                            foreach (var itemPerBrandCode in itemDetail.ListPerBrandCode)
                            {
                                switch (itemPerBrandCode.ParentLocationCode)
                                {
                                    case "PLNT":
                                        styleColor.SetFontColor(Color.Black);
                                        break;
                                    case "REG1":
                                        styleColor.SetFontColor(Color.Blue);
                                        break;
                                    case "REG2":
                                        styleColor.SetFontColor(Color.Green);
                                        break;
                                    case "REG3":
                                        styleColor.SetFontColor(Color.Magenta);
                                        break;
                                    case "REG4":
                                        styleColor.SetFontColor(Color.Red);
                                        break;
                                }
                                slDoc.SetCellStyle(iRowBrandCode, 3, iRowBrandCode, 17, styleColor);
                                slDoc.SetCellValue(iRowBrandCode, 3, itemPerBrandCode.ABBR);
                                slDoc.SetCellValue(iRowBrandCode, 4, itemPerBrandCode.Monday);
                                slDoc.SetCellValue(iRowBrandCode, 5, itemPerBrandCode.Tuesday);
                                slDoc.SetCellValue(iRowBrandCode, 6, itemPerBrandCode.Wednesday);
                                slDoc.SetCellValue(iRowBrandCode, 7, itemPerBrandCode.Thursday);
                                slDoc.SetCellValue(iRowBrandCode, 8, itemPerBrandCode.Friday);
                                slDoc.SetCellValue(iRowBrandCode, 9, itemPerBrandCode.Saturday);
                                slDoc.SetCellValue(iRowBrandCode, 10, itemPerBrandCode.Sunday);
                                slDoc.SetCellValue(iRowBrandCode, 11, itemPerBrandCode.Total);
                                slDoc.SetCellValue(iRowBrandCode, 12, itemPerBrandCode.Planning);
                                slDoc.SetCellValue(iRowBrandCode, 13, itemPerBrandCode.VarianceStick);
                                slDoc.SetCellValue(iRowBrandCode, 14, itemPerBrandCode.VariancePercent);
                                slDoc.SetCellValue(iRowBrandCode, 15, itemPerBrandCode.ReliabilityPercent);
                                slDoc.SetCellValue(iRowBrandCode, 16, itemPerBrandCode.Package);
                                slDoc.SetCellValue(iRowBrandCode, 17, itemPerBrandCode.TWHEqv);
                                iRowBrandCode++;
                            }
                            styleTotal.SetFontColor(Color.Black);
                            iRowSKTBrandCode = mergeBrandCode;
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 2, "Sub Total :");
                            slDoc.MergeWorksheetCells(iRowSKTBrandCode + 1, 2, iRowSKTBrandCode + 1, 2);
                            slDoc.SetCellStyle(iRowSKTBrandCode + 1, 1, iRowSKTBrandCode + 1, 17, styleTotal);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 4, itemDetail.SubTotalMonday);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 5, itemDetail.SubTotalTuesday);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 6, itemDetail.SubTotalWednesday);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 7, itemDetail.SubTotalThursday);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 8, itemDetail.SubTotalFriday);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 9, itemDetail.SubTotalSaturday);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 10, itemDetail.SubTotalSunday);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 11, itemDetail.SubTotalTotal);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 12, itemDetail.SubTotalPlanning);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 13, itemDetail.SubTotalVarianceStick);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 14, itemDetail.SubTotalVariancePercent);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 15, itemDetail.SubTotalReliabilityPercent);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 16, itemDetail.SubTotalPackage);
                            slDoc.SetCellValue(iRowSKTBrandCode + 1, 17, itemDetail.SubTotalTWHEqv);

                            iRowSKTBrandCode = iRowSKTBrandCode + 2;
                        }

                        iRow = mergeSktBrandCode;
                        slDoc.SetCellValue(iRow + 1, 1, "Total/Day :");
                        slDoc.MergeWorksheetCells(iRow + 1, 1, iRow + 1, 3);
                        slDoc.SetCellStyle(iRow + 1, 1, iRow + 1, 17, styleTotal);
                        slDoc.SetCellValue(iRow + 1, 4, item.TotalMonday);
                        slDoc.SetCellValue(iRow + 1, 5, item.TotalTuesday);
                        slDoc.SetCellValue(iRow + 1, 6, item.TotalWednesday);
                        slDoc.SetCellValue(iRow + 1, 7, item.TotalThursday);
                        slDoc.SetCellValue(iRow + 1, 8, item.TotalFriday);
                        slDoc.SetCellValue(iRow + 1, 9, item.TotalSaturday);
                        slDoc.SetCellValue(iRow + 1, 10, item.TotalSunday);
                        slDoc.SetCellValue(iRow + 1, 11, item.TotalTotal);
                        slDoc.SetCellValue(iRow + 1, 12, item.TotalPlanning);
                        slDoc.SetCellValue(iRow + 1, 13, item.TotalVarianceStick);
                        slDoc.SetCellValue(iRow + 1, 14, item.TotalVariancePercent);
                        slDoc.SetCellValue(iRow + 1, 15, item.TotalReliabilityPercent);
                        slDoc.SetCellValue(iRow + 1, 16, item.TotalPackage);
                        slDoc.SetCellValue(iRow + 1, 16, item.TotalTWHEqv);

                        slDoc.SetCellValue(iRow + 2, 1, "Total Cumulative");
                        slDoc.MergeWorksheetCells(iRow + 2, 1, iRow + 3, 2);

                        slDoc.SetCellValue(iRow + 2, 3, "Stick :");
                        slDoc.SetCellStyle(iRow + 2, 1, iRow + 2, 17, styleCenterAlignment);
                        slDoc.SetCellValue(iRow + 2, 4, item.CumulativeTotalMonday);
                        slDoc.SetCellValue(iRow + 2, 5, item.CumulativeTotalTuesday);
                        slDoc.SetCellValue(iRow + 2, 6, item.CumulativeTotalWednesday);
                        slDoc.SetCellValue(iRow + 2, 7, item.CumulativeTotalThursday);
                        slDoc.SetCellValue(iRow + 2, 8, item.CumulativeTotalFriday);
                        slDoc.SetCellValue(iRow + 2, 9, item.CumulativeTotalSaturday);
                        slDoc.SetCellValue(iRow + 2, 10, item.CumulativeTotalSunday);

                        slDoc.SetCellValue(iRow + 3, 3, "% :");
                        slDoc.SetCellStyle(iRow + 3, 1, iRow + 3, 17, styleCenterAlignment);
                        slDoc.SetCellValue(iRow + 3, 4, item.CumulativePercentTotalMonday);
                        slDoc.SetCellValue(iRow + 3, 5, item.CumulativePercentTotalTuesday);
                        slDoc.SetCellValue(iRow + 3, 6, item.CumulativePercentTotalWednesday);
                        slDoc.SetCellValue(iRow + 3, 7, item.CumulativePercentTotalThursday);
                        slDoc.SetCellValue(iRow + 3, 8, item.CumulativePercentTotalFriday);
                        slDoc.SetCellValue(iRow + 3, 9, item.CumulativePercentTotalSaturday);
                        slDoc.SetCellValue(iRow + 3, 10, item.CumulativePercentTotalSunday);

                        iRow = iRow + 4;
                    }

                    slDoc.SetCellValue(iRow, 1, "Total Hand-rolled :");
                    slDoc.MergeWorksheetCells(iRow, 1, iRow, 3);
                    slDoc.SetCellStyle(iRow, 1, iRow, 17, styleTotal);
                    slDoc.SetCellValue(iRow, 4, dailyReport.TotalMondayHandRolled);
                    slDoc.SetCellValue(iRow, 5, dailyReport.TotalTuesdayHandRolled);
                    slDoc.SetCellValue(iRow, 6, dailyReport.TotalWednesdayHandRolled);
                    slDoc.SetCellValue(iRow, 7, dailyReport.TotalThursdayHandRolled);
                    slDoc.SetCellValue(iRow, 8, dailyReport.TotalFridayHandRolled);
                    slDoc.SetCellValue(iRow, 9, dailyReport.TotalSaturdayHandRolled);
                    slDoc.SetCellValue(iRow, 10, dailyReport.TotalSundayHandRolled);
                    slDoc.SetCellValue(iRow, 11, dailyReport.TotalTotalHandRolled);
                    slDoc.SetCellValue(iRow, 12, dailyReport.TotalPlanningHandRolled);
                    slDoc.SetCellValue(iRow, 13, dailyReport.TotalVarianceStickHandRolled);
                    slDoc.SetCellValue(iRow, 14, dailyReport.TotalVariancePercentHandRolled);
                    slDoc.SetCellValue(iRow, 15, dailyReport.TotalReliabilityPercentHandRolled);
                    slDoc.SetCellValue(iRow, 16, dailyReport.TotalPackageHandRolled);
                    slDoc.SetCellValue(iRow, 17, dailyReport.TotalTWHEqvHandRolled);

                    slDoc.SetCellValue(iRow + 1, 1, "Total Cumulative All :");
                    slDoc.MergeWorksheetCells(iRow + 1, 1, iRow + 2, 2);

                    slDoc.SetCellValue(iRow + 1, 3, "Stick");
                    slDoc.SetCellStyle(iRow + 1, 1, iRow + 1, 17, styleCenterAlignmentTotal);
                    slDoc.SetCellValue(iRow + 1, 4, dailyReport.TotalAllCumulativeStickMonday);
                    slDoc.SetCellValue(iRow + 1, 5, dailyReport.TotalAllCumulativeStickTuesday);
                    slDoc.SetCellValue(iRow + 1, 6, dailyReport.TotalAllCumulativeStickWednesday);
                    slDoc.SetCellValue(iRow + 1, 7, dailyReport.TotalAllCumulativeStickThursday);
                    slDoc.SetCellValue(iRow + 1, 8, dailyReport.TotalAllCumulativeStickFriday);
                    slDoc.SetCellValue(iRow + 1, 9, dailyReport.TotalAllCumulativeStickSaturday);
                    slDoc.SetCellValue(iRow + 1, 10, dailyReport.TotalAllCumulativeStickSunday);

                    slDoc.SetCellValue(iRow + 2, 3, "%");
                    slDoc.SetCellStyle(iRow + 2, 1, iRow + 2, 17, styleCenterAlignmentTotal);
                    slDoc.SetCellValue(iRow + 2, 4, dailyReport.TotalAllCumulativePercentMonday);
                    slDoc.SetCellValue(iRow + 2, 5, dailyReport.TotalAllCumulativePercentTuesday);
                    slDoc.SetCellValue(iRow + 2, 6, dailyReport.TotalAllCumulativePercentWednesday);
                    slDoc.SetCellValue(iRow + 2, 7, dailyReport.TotalAllCumulativePercentThursday);
                    slDoc.SetCellValue(iRow + 2, 8, dailyReport.TotalAllCumulativePercentFriday);
                    slDoc.SetCellValue(iRow + 2, 9, dailyReport.TotalAllCumulativePercentSaturday);
                    slDoc.SetCellValue(iRow + 2, 10, dailyReport.TotalAllCumulativePercentSunday);

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
                    //slDoc.AutoFitColumn(1, 9);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);

                    ms.Position = 0;


                }
                var fileName = "ProductionExecution_Reports_DailyProductionAchievement_" +
                               DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Exe Report Daily Production - Generate Excel");
                return null;
            }
        }
    }
}