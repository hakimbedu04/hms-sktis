using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenWeek;
using SKTISWebsite.Models.PlanningReportSummaryProcessTargets;
using SpreadsheetLight;

namespace SKTISWebsite.Controllers
{
    public class PlanningReportSummaryProcessTargetsController : BaseController
    {
        private IPlanningBLL _planningBll;
        private IApplicationService _applicationService;
        private IMasterDataBLL _masterDataBll;
        private IVTLogger _vtlogger;

        public PlanningReportSummaryProcessTargetsController(IVTLogger vtlogger, IPlanningBLL planning, IApplicationService applicationService, IMasterDataBLL masterDataBll)
        {
            _planningBll = planning;
            _applicationService = applicationService;
            _masterDataBll = masterDataBll;
            _vtlogger = vtlogger;
            SetPage("Productionplanning/Report/SummaryProcessTarget");
        }

        // GET: PlanningSummaryProcessTargets
        public ActionResult Index()
        {
            var week = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(DateTime.Now.Year, week.HasValue ? week.Value : 1);

            var init = new InitPlanningReportSummaryProcessTargetsViewModel
            {
                LocationSelectList = _applicationService.GetLocationCodeCompat(),
                YearSelectList = _applicationService.GetGenWeekYears(),
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                DefaultDateFrom = mstGenWeek.StartDate,
                DefaultDateTo = mstGenWeek.EndDate
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
        public JsonResult GetWeekByYearAndWeek(int year, int week)
        {
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(year, week);
            //if (_masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now) == week)
                //mstGenWeek.EndDate = DateTime.Now.Date;
            var model = Mapper.Map<MasterGenWeekViewModel>(mstGenWeek);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPlanningReportProductionTargets(GetPlanningReportSummaryProcessTargetsInput criteria)
        {
            try
            {
                // Klo nyari kenapa decimal nya fixed, silahkan cari di view nya.
                // Tiket = http://tp.voxteneo.co.id/entity/6843
                var masterLists = _planningBll.GetReportSummaryProcessTargets(criteria);
                var viewModel = Mapper.Map<List<PlanningReportSummaryProcessTargetsViewModel>>(masterLists);
                var pageResult = new PageResult<PlanningReportSummaryProcessTargetsViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Get Planning Report Production Targets");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(DateTime? dateFrom, DateTime? dateTo, int comma, string filterType, string location, int week, int year)
        {
            try
            {
                var input = new GetPlanningReportSummaryProcessTargetsInput
                {
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    Decimal = comma,
                    FilterType = filterType,
                    Location = location,
                    Week = week,
                    Year = year
                };
                var listDatas = _planningBll.GetReportSummaryProcessTargets(input);
                string dateFroms = string.Format("{0:dd/MM/yyyy}", dateFrom);
                string dateTos = string.Format("{0:dd/MM/yyyy}", dateTo);

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = HMS.SKTIS.Core.Enums.PlanningExcelTemplate.ReportSummaryProcessTargets + ".xlsx";
                var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(2, 3, year);
                    slDoc.SetCellValue(3, 3, location);
                    slDoc.SetCellValue(4, 3, comma);
                    if (input.FilterType == "Date")
                    {
                        slDoc.SetCellValue(2, 7, "");
                        slDoc.SetCellValue(2, 8, "");
                        slDoc.SetCellValue(2, 9, "");
                    }
                    else
                    {
                        slDoc.SetCellValue(2, 9, week);
                    }
                    slDoc.SetCellValue(3, 9, dateFroms + " To " + dateTos);

                    var iRow = 7;

                    double totalGiling = 0;
                    double totalGunting = 0;
                    double totalWIPGunting = 0;
                    double totalPack = 0;
                    double totalWIPPack = 0;
                    double totalBanderol = 0;
                    double totalBox = 0;

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

                        slDoc.SetCellValue(iRow, 1, data.LocationCode);
                        slDoc.SetCellValue(iRow, 2, data.UnitCode);
                        slDoc.SetCellValue(iRow, 3, data.BrandCode);
                        slDoc.SetCellValue(iRow, 4, data.Giling);
                        totalGiling += data.Giling;
                        slDoc.SetCellValue(iRow, 5, data.Gunting);
                        totalGunting += data.Gunting;
                        slDoc.SetCellValue(iRow, 6, data.WIPGunting);
                        totalWIPGunting += data.WIPGunting;
                        slDoc.SetCellValue(iRow, 7, data.Pak);
                        totalPack += data.Pak;
                        slDoc.SetCellValue(iRow, 8, data.WIPPak);
                        totalWIPPack += data.WIPPak;
                        slDoc.SetCellValue(iRow, 9, data.Banderol);
                        totalBanderol += data.Banderol;
                        slDoc.SetCellValue(iRow, 10, data.Box);
                        totalBox += data.Box;
                        slDoc.SetCellStyle(iRow, 1, iRow, 10, style);
                        iRow++;
                    }

                    slDoc.SetCellValue(iRow, 1, "Total");
                    slDoc.MergeWorksheetCells(iRow, 1, iRow, 3);
                    slDoc.SetCellValue(iRow, 4, totalGiling);
                    slDoc.SetCellValue(iRow, 5, totalGunting);
                    slDoc.SetCellValue(iRow, 6, totalWIPGunting);
                    slDoc.SetCellValue(iRow, 7, totalPack);
                    slDoc.SetCellValue(iRow, 8, totalWIPPack);
                    slDoc.SetCellValue(iRow, 9, totalBanderol);
                    slDoc.SetCellValue(iRow, 10, totalBox);
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
                    slDoc.AutoFitColumn(1, 10);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "PlanningReportSummaryProcesstarget_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { dateFrom, dateTo, comma, filterType, location, week, year }, "Excel - Planning Report Production Targets");
                return null;
            }
        }
    }
}