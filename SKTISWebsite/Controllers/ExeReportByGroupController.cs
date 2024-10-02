using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.ExeReportByGroup;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.Common;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class ExeReportByGroupController : BaseController
    {

        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExeReportBLL _exeReportBLL;
        private IVTLogger _vtlogger;

        public ExeReportByGroupController(IApplicationService applicationService, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IExeReportBLL exeReportBLL)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _exeReportBLL = exeReportBLL;  
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/Report/ProductionReportbyGroup");
        }

        // GET: ProdExeReportByGroup
        public ActionResult Index()
        {
            var init = new InitExeReportByGroupViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                MonthSelectList = _svc.GetMonthSelectList(),
                MonthToSelectList = _svc.GetMonthSelectList(),
                PLNTChildLocationLookupList = _svc.GetLastLocationChildList(Enums.LocationCode.SKT.ToString()),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                DefaultMonth = DateTime.Now.Month,
                AbsentTypes = _svc.GetAllAbsentTypes()
            };

            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetUnitCodeSelectListByLocationCodeReportByGroup(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShiftByLocation(string locationCode)
        {
            var shifts = _svc.GetShiftByLocationCode(locationCode);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult GetProcessGroupSelectListByLocationCode(string locationCode)
        {
            var processSettings = _masterDataBLL.GetMasterProcessSettingByLocationCode(locationCode);
            var processSettingsDistinctByProcessGroup = processSettings
                                                         .Where(c => c.ProcessGroup != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily))
                                                         .DistinctBy(x => x.ProcessGroup);
            return Json(new SelectList(processSettingsDistinctByProcessGroup, "ProcessGroup", "ProcessGroup"), JsonRequestBehavior.AllowGet);
         
        }

        [HttpGet]
        public JsonResult GetBrandFromExeReportByGroupByLocationAndProcess(string locationCode, string process)
        {
            var model = _svc.GetGroupBrandExeReportByGroupByLocationAndProcess(locationCode, process);
            //var model = _svc.GetBrandGroupCodeSelectList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateByYearWeek(int year, int week)
        {
            var date = _masterDataBLL.GetWeekByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult GetExeReportByGroup(GetExeReportByGroupInput criteria)
        {
            try
            {
                var masterLists = _exeReportBLL.GetReportByGroups(criteria);
                var viewModel = Mapper.Map<List<ExeReportByGroupViewModel>>(masterLists);
                var pageResult = new PageResult<ExeReportByGroupViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "GetExeReportByGroup");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string unitCode, int? shift, string process, string brand, string filterType, int? yearFrom, int? monthFrom, int? monthTo, int? weekFrom, int? weekTo, DateTime? dateFrom, DateTime? dateTo, int? yearFromMonth, int? yearToMonth, int? yearFromWeek, int? yearToWeek)
        {
            try
            {
                var input = new GetExeReportByGroupInput
                {
                    LocationCode = locationCode,
                    UnitCode = unitCode,
                    Shift = shift,
                    ProcessGroup = process,
                    Brand = brand,
                    FilterType = filterType,
                    YearFrom = yearFrom,
                    MonthFrom = monthFrom,
                    MonthTo = monthTo,
                    WeekFrom = weekFrom,
                    WeekTo = weekTo,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    YearFromMonth = yearFromMonth,
                    YearToMonth = yearToMonth,
                    YearFromWeek = yearFromWeek,
                    YearToWeek = yearToWeek
                };
                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == locationCode)
                    {
                        locationCompat = item.Text;
                    }
                }
                input.SortExpression = "BrandGroupCode";
                input.SortOrder = "ASC";

                //var ExeReportByGroups = _exeReportBLL.GetReportByGroups(input);
                //var ExeReportByGroupsMonthly = _exeReportBLL.GetReportByGroupsMonthly(input);
                //var ExeReportByGroupsWeekly = _exeReportBLL.GetReportByGroupsWeekly(input);
                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExeReportByGroup + ".xlsx";
                var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    var filter = input.FilterType;
                    if (filter == "YearMonth")
                    {
                        //filter values
                        slDoc.SetCellValue(3, 2, ": " + locationCompat);
                        slDoc.SetCellValue(4, 2, ": " + unitCode);
                        slDoc.SetCellValue(5, 2, ": " + shift);
                        slDoc.SetCellValue(6, 2, ": " + process);
                        slDoc.SetCellValue(7, 2, ": " + brand);
                        slDoc.SetCellValue(7, 15, "Year : " + yearFromMonth.ToString());
                        slDoc.SetCellValue(7, 18, "Month : " + monthFrom.ToString());
                        slDoc.SetCellValue(7, 21, "To Year : " + yearToMonth.ToString());
                        slDoc.SetCellValue(7, 25, "Month : " + monthTo.ToString());

                        //row values
                        var iRow = 13;

                        var exeReportByGroupsMonthly = _exeReportBLL.GetReportByGroups(input);

                        var exeReportByGroupMonthlyViewModel =
                            Mapper.Map<List<ExeReportByGroupViewModel>>(exeReportByGroupsMonthly);

                        foreach (var masterListGroup in exeReportByGroupMonthlyViewModel)
                        {
                            SLStyle style = slDoc.CreateStyle();
                            style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                            style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                            style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                            style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                            style.Font.FontName = "Calibri";
                            style.Font.FontSize = 10;


                            if (iRow%2 == 0)
                            {
                                style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                            }

                            slDoc.SetCellValue(iRow, 1, masterListGroup.BrandGroupCode);
                            slDoc.SetCellValue(iRow, 2, masterListGroup.LocationCode);
                            slDoc.SetCellValue(iRow, 3, masterListGroup.UnitCode);
                            slDoc.SetCellValue(iRow, 4, masterListGroup.GroupCode);
                            slDoc.SetCellValue(iRow, 5, masterListGroup.Register);
                            slDoc.SetCellValue(iRow, 6, masterListGroup.A);
                            slDoc.SetCellValue(iRow, 7, masterListGroup.I);
                            slDoc.SetCellValue(iRow, 8, masterListGroup.S);
                            slDoc.SetCellValue(iRow, 9, masterListGroup.C);
                            slDoc.SetCellValue(iRow, 10, masterListGroup.CH);
                            slDoc.SetCellValue(iRow, 11, masterListGroup.CT);
                            slDoc.SetCellValue(iRow, 12, masterListGroup.SLSSLP);
                            slDoc.SetCellValue(iRow, 13, masterListGroup.ETC);
                            slDoc.SetCellValue(iRow, 14, masterListGroup.Multi_TPO);
                            slDoc.SetCellValue(iRow, 15, masterListGroup.Multi_ROLL);
                            slDoc.SetCellValue(iRow, 16, masterListGroup.Multi_CUTT);
                            slDoc.SetCellValue(iRow, 17, masterListGroup.Multi_PACK);
                            slDoc.SetCellValue(iRow, 18, masterListGroup.Multi_STAMP);
                            slDoc.SetCellValue(iRow, 19, masterListGroup.Multi_FWRP);
                            slDoc.SetCellValue(iRow, 20, masterListGroup.Multi_SWRP);
                            slDoc.SetCellValue(iRow, 21, masterListGroup.Multi_WRP);
                            slDoc.SetCellValue(iRow, 22, masterListGroup.Multi_GEN);
                            slDoc.SetCellValue(iRow, 23, masterListGroup.In);
                            slDoc.SetCellValue(iRow, 24, masterListGroup.Out);
                            slDoc.SetCellValue(iRow, 25, masterListGroup.ActualWorker);
                            slDoc.SetCellValue(iRow, 26, masterListGroup.WorkHour);
                            slDoc.SetCellValue(iRow, 27, masterListGroup.Production);
                            slDoc.SetCellValue(iRow, 28, masterListGroup.ValuePeople);
                            slDoc.SetCellValue(iRow, 29, masterListGroup.ValueHour);
                            slDoc.SetCellValue(iRow, 30, masterListGroup.ValuePeopleHour);
                            slDoc.SetCellStyle(iRow, 1, iRow, 30, style);
                            iRow++;
                        }
                    }
                    else if (filter == "YearWeek")
                    {
                        //filter values
                        slDoc.SetCellValue(3, 2, ": " + locationCompat);
                        slDoc.SetCellValue(4, 2, ": " + unitCode);
                        slDoc.SetCellValue(5, 2, ": " + shift);
                        slDoc.SetCellValue(6, 2, ": " + process);
                        slDoc.SetCellValue(7, 2, ": " + brand);
                        slDoc.SetCellValue(7, 15, "Year : " + yearFromWeek.ToString());
                        slDoc.SetCellValue(7, 18, "Week : " + weekFrom.ToString());
                        slDoc.SetCellValue(7, 21, "To Year : " + yearToWeek.ToString());
                        slDoc.SetCellValue(7, 25, "Week : " + weekTo.ToString());

                        //row values
                        var iRow = 13;

                        var exeReportByGroupsWeekly = _exeReportBLL.GetReportByGroups(input);

                        var exeReportByGroupsWeeklyViewModel =
                            Mapper.Map<List<ExeReportByGroupViewModel>>(exeReportByGroupsWeekly);

                        foreach (var masterListGroup in exeReportByGroupsWeeklyViewModel)
                        {
                            SLStyle style = slDoc.CreateStyle();
                            style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                            style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                            style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                            style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                            style.Font.FontName = "Calibri";
                            style.Font.FontSize = 10;

                            //var ActualWorkers = masterListGroup.Attend - (masterListGroup.Multi_TPO + masterListGroup.Multi_ROLL + masterListGroup.Multi_CUTT + masterListGroup.Multi_PACK + masterListGroup.Multi_STAMP + masterListGroup.Multi_FWRP + masterListGroup.Multi_SWRP + masterListGroup.Multi_GEN + masterListGroup.Multi_WRP);
                            //var AvgPeople = ActualWorkers == 0 ? 0 : Math.Round((double)masterListGroup.Production / ActualWorkers, 2);
                            //var AvgHour = 0;// Math.Round((double)masterListGroup.Production / masterListGroup.WorkHour, 2);

                            if (iRow%2 == 0)
                            {
                                style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                            }

                            slDoc.SetCellValue(iRow, 1, masterListGroup.BrandGroupCode);
                            slDoc.SetCellValue(iRow, 2, masterListGroup.LocationCode);
                            slDoc.SetCellValue(iRow, 3, masterListGroup.UnitCode);
                            slDoc.SetCellValue(iRow, 4, masterListGroup.GroupCode);
                            slDoc.SetCellValue(iRow, 5, masterListGroup.Register);
                            slDoc.SetCellValue(iRow, 6, masterListGroup.A);
                            slDoc.SetCellValue(iRow, 7, masterListGroup.I);
                            slDoc.SetCellValue(iRow, 8, masterListGroup.S);
                            slDoc.SetCellValue(iRow, 9, masterListGroup.C);
                            slDoc.SetCellValue(iRow, 10, masterListGroup.CH);
                            slDoc.SetCellValue(iRow, 11, masterListGroup.CT);
                            slDoc.SetCellValue(iRow, 12, masterListGroup.SLSSLP);
                            slDoc.SetCellValue(iRow, 13, masterListGroup.ETC);
                            slDoc.SetCellValue(iRow, 14, masterListGroup.Multi_TPO);
                            slDoc.SetCellValue(iRow, 15, masterListGroup.Multi_ROLL);
                            slDoc.SetCellValue(iRow, 16, masterListGroup.Multi_CUTT);
                            slDoc.SetCellValue(iRow, 17, masterListGroup.Multi_PACK);
                            slDoc.SetCellValue(iRow, 18, masterListGroup.Multi_STAMP);
                            slDoc.SetCellValue(iRow, 19, masterListGroup.Multi_FWRP);
                            slDoc.SetCellValue(iRow, 20, masterListGroup.Multi_SWRP);
                            slDoc.SetCellValue(iRow, 21, masterListGroup.Multi_WRP);
                            slDoc.SetCellValue(iRow, 22, masterListGroup.Multi_GEN);
                            slDoc.SetCellValue(iRow, 23, masterListGroup.In);
                            slDoc.SetCellValue(iRow, 24, masterListGroup.Out);
                            slDoc.SetCellValue(iRow, 25, masterListGroup.ActualWorker);
                            slDoc.SetCellValue(iRow, 26, masterListGroup.WorkHour);
                            slDoc.SetCellValue(iRow, 27, masterListGroup.Production);
                            slDoc.SetCellValue(iRow, 28, masterListGroup.ValuePeople);
                            slDoc.SetCellValue(iRow, 29, masterListGroup.ValueHour);
                            slDoc.SetCellValue(iRow, 30, masterListGroup.ValuePeopleHour);
                            slDoc.SetCellStyle(iRow, 1, iRow, 30, style);
                            iRow++;
                        }
                    }
                    else
                    {
                        if (filter == "Year")
                        {
                            //filter values
                            slDoc.SetCellValue(3, 2, ": " + locationCompat);
                            slDoc.SetCellValue(4, 2, ": " + unitCode);
                            slDoc.SetCellValue(5, 2, ": " + shift);
                            slDoc.SetCellValue(6, 2, ": " + process);
                            slDoc.SetCellValue(7, 2, ": " + brand);
                            slDoc.SetCellValue(7, 25, "Year : " + yearFrom.ToString());

                            //row values
                            var iRow = 13;

                            var reportByGroupAnnualy = _exeReportBLL.GetReportByGroups(input);

                            var reportByGroupAnnualyViewModel =
                                Mapper.Map<List<ExeReportByGroupViewModel>>(reportByGroupAnnualy);

                            foreach (var masterListGroup in reportByGroupAnnualyViewModel)
                            {
                                SLStyle style = slDoc.CreateStyle();
                                style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Font.FontName = "Calibri";
                                style.Font.FontSize = 10;

                                //var ActualWorkers = masterListGroup.Attend - (masterListGroup.Multi_TPO + masterListGroup.Multi_ROLL + masterListGroup.Multi_CUTT + masterListGroup.Multi_PACK + masterListGroup.Multi_STAMP + masterListGroup.Multi_FWRP + masterListGroup.Multi_SWRP + masterListGroup.Multi_GEN + masterListGroup.Multi_WRP);
                                //var AvgPeople = ActualWorkers == 0 ? 0 : Math.Round((double)masterListGroup.Production / ActualWorkers, 2);
                                //var AvgHour = 0;// Math.Round((double)masterListGroup.Production / masterListGroup.WorkHour, 2);

                                if (iRow%2 == 0)
                                {
                                    style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                                }

                                slDoc.SetCellValue(iRow, 1, masterListGroup.BrandGroupCode);
                                slDoc.SetCellValue(iRow, 2, masterListGroup.LocationCode);
                                slDoc.SetCellValue(iRow, 3, masterListGroup.UnitCode);
                                slDoc.SetCellValue(iRow, 4, masterListGroup.GroupCode);
                                slDoc.SetCellValue(iRow, 5, masterListGroup.Register);
                                slDoc.SetCellValue(iRow, 6, masterListGroup.A);
                                slDoc.SetCellValue(iRow, 7, masterListGroup.I);
                                slDoc.SetCellValue(iRow, 8, masterListGroup.S);
                                slDoc.SetCellValue(iRow, 9, masterListGroup.C);
                                slDoc.SetCellValue(iRow, 10, masterListGroup.CH);
                                slDoc.SetCellValue(iRow, 11, masterListGroup.CT);
                                slDoc.SetCellValue(iRow, 12, masterListGroup.SLSSLP);
                                slDoc.SetCellValue(iRow, 13, masterListGroup.ETC);
                                slDoc.SetCellValue(iRow, 14, masterListGroup.Multi_TPO);
                                slDoc.SetCellValue(iRow, 15, masterListGroup.Multi_ROLL);
                                slDoc.SetCellValue(iRow, 16, masterListGroup.Multi_CUTT);
                                slDoc.SetCellValue(iRow, 17, masterListGroup.Multi_PACK);
                                slDoc.SetCellValue(iRow, 18, masterListGroup.Multi_STAMP);
                                slDoc.SetCellValue(iRow, 19, masterListGroup.Multi_FWRP);
                                slDoc.SetCellValue(iRow, 20, masterListGroup.Multi_SWRP);
                                slDoc.SetCellValue(iRow, 21, masterListGroup.Multi_WRP);
                                slDoc.SetCellValue(iRow, 22, masterListGroup.Multi_GEN);
                                slDoc.SetCellValue(iRow, 23, masterListGroup.In);
                                slDoc.SetCellValue(iRow, 24, masterListGroup.Out);
                                slDoc.SetCellValue(iRow, 25, masterListGroup.ActualWorker);
                                slDoc.SetCellValue(iRow, 26, masterListGroup.WorkHour);
                                slDoc.SetCellValue(iRow, 27, masterListGroup.Production);
                                slDoc.SetCellValue(iRow, 28, masterListGroup.ValuePeople);
                                slDoc.SetCellValue(iRow, 29, masterListGroup.ValueHour);
                                slDoc.SetCellValue(iRow, 30, masterListGroup.ValuePeopleHour);
                                slDoc.SetCellStyle(iRow, 1, iRow, 30, style);
                                iRow++;
                            }
                        }
                        else
                        {
                            //filter values
                            slDoc.SetCellValue(3, 2, ": " + locationCompat);
                            slDoc.SetCellValue(4, 2, ": " + unitCode);
                            slDoc.SetCellValue(5, 2, ": " + shift);
                            slDoc.SetCellValue(6, 2, ": " + process);
                            slDoc.SetCellValue(7, 2, ": " + brand);
                            slDoc.SetCellValue(7, 15, "Date : " + dateFrom.Value.ToShortDateString());
                            slDoc.SetCellValue(7, 21, "To Date : " + dateTo.Value.ToShortDateString());

                            //row values
                            var iRow = 13;

                            var reportByGroupDaily = _exeReportBLL.GetReportByGroups(input);

                            var reportByGroupDailyViewModel =
                                Mapper.Map<List<ExeReportByGroupViewModel>>(reportByGroupDaily);

                            foreach (var masterListGroup in reportByGroupDailyViewModel)
                            {
                                SLStyle style = slDoc.CreateStyle();
                                style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Font.FontName = "Calibri";
                                style.Font.FontSize = 10;

                                //var ActualWorkers = masterListGroup.Attend - (masterListGroup.Multi_TPO + masterListGroup.Multi_ROLL + masterListGroup.Multi_CUTT + masterListGroup.Multi_PACK + masterListGroup.Multi_STAMP + masterListGroup.Multi_FWRP + masterListGroup.Multi_SWRP + masterListGroup.Multi_GEN + masterListGroup.Multi_WRP);
                                //var AvgPeople = ActualWorkers == 0 ? 0 : Math.Round((double)masterListGroup.Production / ActualWorkers, 2);
                                //var AvgHour = 0;// Math.Round((double)masterListGroup.Production / masterListGroup.WorkHour, 2);

                                if (iRow%2 == 0)
                                {
                                    style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                                }

                                slDoc.SetCellValue(iRow, 1, masterListGroup.BrandGroupCode);
                                slDoc.SetCellValue(iRow, 2, masterListGroup.LocationCode);
                                slDoc.SetCellValue(iRow, 3, masterListGroup.UnitCode);
                                slDoc.SetCellValue(iRow, 4, masterListGroup.GroupCode);
                                slDoc.SetCellValue(iRow, 5, masterListGroup.Register);
                                slDoc.SetCellValue(iRow, 6, masterListGroup.A);
                                slDoc.SetCellValue(iRow, 7, masterListGroup.I);
                                slDoc.SetCellValue(iRow, 8, masterListGroup.S);
                                slDoc.SetCellValue(iRow, 9, masterListGroup.C);
                                slDoc.SetCellValue(iRow, 10, masterListGroup.CH);
                                slDoc.SetCellValue(iRow, 11, masterListGroup.CT);
                                slDoc.SetCellValue(iRow, 12, masterListGroup.SLSSLP);
                                slDoc.SetCellValue(iRow, 13, masterListGroup.ETC);
                                slDoc.SetCellValue(iRow, 14, masterListGroup.Multi_TPO);
                                slDoc.SetCellValue(iRow, 15, masterListGroup.Multi_ROLL);
                                slDoc.SetCellValue(iRow, 16, masterListGroup.Multi_CUTT);
                                slDoc.SetCellValue(iRow, 17, masterListGroup.Multi_PACK);
                                slDoc.SetCellValue(iRow, 18, masterListGroup.Multi_STAMP);
                                slDoc.SetCellValue(iRow, 19, masterListGroup.Multi_FWRP);
                                slDoc.SetCellValue(iRow, 20, masterListGroup.Multi_SWRP);
                                slDoc.SetCellValue(iRow, 21, masterListGroup.Multi_WRP);
                                slDoc.SetCellValue(iRow, 22, masterListGroup.Multi_GEN);
                                slDoc.SetCellValue(iRow, 23, masterListGroup.In);
                                slDoc.SetCellValue(iRow, 24, masterListGroup.Out);
                                slDoc.SetCellValue(iRow, 25, masterListGroup.ActualWorker);
                                slDoc.SetCellValue(iRow, 26, masterListGroup.WorkHour);
                                slDoc.SetCellValue(iRow, 27, masterListGroup.Production);
                                slDoc.SetCellValue(iRow, 28, masterListGroup.ValuePeople);
                                slDoc.SetCellValue(iRow, 29, masterListGroup.ValueHour);
                                slDoc.SetCellValue(iRow, 30, masterListGroup.ValuePeopleHour);
                                slDoc.SetCellStyle(iRow, 1, iRow, 30, style);
                                iRow++;
                            }

                        }

                    }

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
                    slDoc.AutoFitColumn(1, 7);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "ProductionExecution_Reports_ProductionGroup_" + DateTime.Now.ToShortDateString() +
                               ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, shift, process, brand, filterType, yearFrom, monthFrom, monthTo, weekFrom, weekTo, dateFrom, dateTo, yearFromMonth, yearToMonth, yearFromWeek, yearToWeek }, "Generate Excel on Exe Report By Group");
                return null;
            }
        }

        // Get filter process from ExeReportByGroups
        public JsonResult GetProcessList(GetExeReportByGroupInput criteria) 
        {
            var listProcess = _exeReportBLL.GetProcessListFilter(criteria);
            return Json(listProcess, JsonRequestBehavior.AllowGet);
        }

        // Get filter brand group code from ExeReportByGroups
        public JsonResult GetBrandGroupCodeList(GetExeReportByGroupInput criteria)
        {
            var listSktBrandCode = _exeReportBLL.GetBrandGroupCodeFilter(criteria);
            return Json(listSktBrandCode, JsonRequestBehavior.AllowGet);
        }
    }
}