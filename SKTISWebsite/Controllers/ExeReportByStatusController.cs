using System.Globalization;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExeReportByStatus;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Color = System.Drawing.Color;
using Path = System.IO.Path;

namespace SKTISWebsite.Controllers
{
    public class ExeReportByStatusController : BaseController
    {

        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IPlanningBLL _planningBll;
        private IExecutionPlantBLL _executionPlantBll;
        private IExeReportBLL _exeReportBLL;
        private IVTLogger _vtlogger;
        private IReportByStatusService _reportByStatusService;

        public ExeReportByStatusController(
            IApplicationService applicationService, 
            IVTLogger vtlogger, 
            IMasterDataBLL masterDataBll, 
            IPlanningBLL planningBll, 
            IExecutionPlantBLL executionPlantBll, 
            IExeReportBLL exeReportBLL,
            IReportByStatusService reportByStatusService)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _planningBll = planningBll;
            _executionPlantBll = executionPlantBll;
            _exeReportBLL = exeReportBLL;
            _vtlogger = vtlogger;
            _reportByStatusService = reportByStatusService;
            SetPage("ProductionExecution/Report/ProductionReportbyStatus");
        }

        public ActionResult Index()
        {
            var init = new InitExeReportByStatusViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                MonthSelectList = _svc.GetMonthSelectList(),
                MonthToSelectList = _svc.GetMonthSelectList(),
                PLNTChildLocationLookupList = _svc.GetLocationNamesLookupList(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                DefaultMonth = DateTime.Now.Month,
            };
            return View("index",init);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _reportByStatusService.GetListFilterUnitCode(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShiftByLocation(string locationCode)
        {
            var shifts = _reportByStatusService.GetListFilterShift(locationCode);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetActiveBrandFromByLocation(string locationCode, string unitCode, string brandGroupCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string DateFrom, string DateTo, string FilterType)
        {
            DateTime dtFrom, dtTo;
            var FromDate = DateTime.TryParse(DateFrom, out dtFrom) ? DateTime.Parse(DateFrom).ToShortDateString() : DateTime.Today.ToShortDateString();
            var ToDate = DateTime.TryParse(DateTo, out dtTo) ? DateTime.Parse(DateTo).ToShortDateString() : DateTime.Today.ToShortDateString();
            var model = _reportByStatusService.GetlistFilterBrandCode(new GetExeReportByStatusInput {
                LocationCode = locationCode,
                UnitCode = unitCode,
                BrandGroupCode = brandGroupCode,
                FilterType = FilterType,
                YearFrom = Convert.ToInt32(YearFrom),
                YearTo = Convert.ToInt32(YearTo),
                MonthFrom = Convert.ToInt32(MonthFrom),
                MonthTo = Convert.ToInt32(MonthTo),
                WeekFrom = Convert.ToInt32(WeekFrom),
                WeekTo = Convert.ToInt32(WeekTo),
                DateFrom = DateTime.Parse(DateFrom),
                DateTo = DateTime.Parse(DateTo)
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getActiveBrandGroupByLocationUnit(string locationCode, string unitCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string DateFrom, string DateTo, string FilterType)
        {
            DateTime dtFrom, dtTo;
            var FromDate = DateTime.TryParse(DateFrom, out dtFrom) ? DateTime.Parse(DateFrom).ToShortDateString() : DateTime.Today.ToShortDateString();
            var ToDate = DateTime.TryParse(DateTo, out dtTo) ? DateTime.Parse(DateTo).ToShortDateString() : DateTime.Today.ToShortDateString();
            var model = _reportByStatusService.GetListFilterBrandGroup(new GetExeReportByStatusInput
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                FilterType = FilterType,
                YearFrom = Convert.ToInt32(YearFrom),
                YearTo = Convert.ToInt32(YearTo),
                MonthFrom = Convert.ToInt32(MonthFrom),
                MonthTo = Convert.ToInt32(MonthTo),
                WeekFrom = Convert.ToInt32(WeekFrom),
                WeekTo = Convert.ToInt32(WeekTo),
                DateFrom = DateTime.Parse(DateFrom),
                DateTo = DateTime.Parse(DateTo)
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetExeReportByStatus(GetExeReportByStatusInput criteria)
        {
            //var statusPerProcess = ReportStatusIndexResult(criteria);
            //var pageResult = new PageResult<ExeReportByStatusCompositeViewModel<ExeReportByStatusViewModel>>(statusPerProcess,criteria);
            var data = _reportByStatusService.GetReportByStatus(criteria);
            var pageResult = new PageResult<GetReportByStatusCompositeViewModel>(data.ToList(), criteria);
            return Json(pageResult);
        }

        private List<ExeReportByStatusCompositeViewModel<ExeReportByStatusViewModel>> ReportStatusIndexResult(GetExeReportByStatusInput criteria)
        {
            try
            {
                var ci = CultureInfo.CurrentCulture;
                var listProcess = _masterDataBLL.GetAllActiveMasterProcess()
                    .OrderBy(p => p.ProcessOrder)
                    .ToList();

                var statusPerProcess = new List<ExeReportByStatusCompositeViewModel<ExeReportByStatusViewModel>>();
                
                var report = _exeReportBLL.GetReportByStatus(criteria);

                foreach (var process in listProcess.OrderBy(c => c.ProcessOrder))
                {
                    var processGroup = process.ProcessGroup;
                    var plantTPKViewModel = new ExeReportByStatusCompositeViewModel<ExeReportByStatusViewModel>
                    {
                        ProcessGroup = processGroup,
                        LocationCode = criteria.LocationCode,
                        UnitCode = criteria.UnitCode,
                        BrandGroupCode = criteria.BrandGroupCode,
                    };

                    var statusEmps =
                        report.Where(m => m.ProcessGroup == processGroup).ToList();
                    //report.Where(m => m.ProcessGroup == processGroup).OrderBy(u => u.StatusEmp).ToList();
                    if (!statusEmps.Any()) continue;

                    var listStatusProcess = statusEmps.Select(Mapper.Map<ExeReportByStatusViewModel>).ToList();

                    if (listStatusProcess.Count > 0)
                    {
                        var listGroup = listStatusProcess.GroupBy(c => new
                        {
                            c.ProcessGroup,
                            c.StatusEmp,
                            c.LocationCode,
                            c.UnitCode,
                            c.Shift,
                            c.BrandGroupCode,

                        }).Select(x => new ExeReportByStatusViewModel()
                        {
                            ProcessGroup = x.Key.ProcessGroup,
                            StatusEmp = x.Key.StatusEmp,
                            LocationCode = x.Key.LocationCode,
                            UnitCode = x.Key.UnitCode,
                            Shift = x.Key.Shift,
                            BrandCode = criteria.BrandCode,
                            BrandGroupCode = x.Key.BrandGroupCode,
                            ActualWorker = x.Sum(c => c.ActualWorker),
                            ActualAbsWorker = x.Sum(c => c.ActualAbsWorker),
                            ActualWorkHour = x.Sum(c => c.ActualWorkHour),
                            PrdPerStk = x.Sum(c => c.PrdPerStk),
                            StkPerHrPerPpl =
                                Convert.ToDouble(Convert.ToDouble(x.Sum(c => c.StkPerHrPerPpl).ToString())
                                    .ToString("f2")),
                            StkPerHr =
                                Convert.ToDouble(Convert.ToDouble(x.Sum(c => c.StkPerHr).ToString()).ToString("f2")),
                            BalanceIndex = x.Sum(c => c.BalanceIndex),

                        });

                        var listActualWorkHour = _exeReportBLL.GetActualWork(report);

                        foreach (var exeReportByStatusViewModel in listGroup)
                        {

                            foreach (var exeReportByStatusOutput in listActualWorkHour)
                            {
                                if (exeReportByStatusViewModel.ProcessGroup ==
                                    exeReportByStatusOutput.ProcessGroup &&
                                    exeReportByStatusViewModel.StatusEmp == exeReportByStatusOutput.StatusEmp)
                                {
                                    exeReportByStatusViewModel.ActualWorkHour =
                                        exeReportByStatusOutput.ActualWorkHour;
                                }
                            }


                            exeReportByStatusViewModel.StkPerHrPerPpl =
                                GetStickHourPeople(exeReportByStatusViewModel.PrdPerStk,
                                    exeReportByStatusViewModel.ActualWorkHour, exeReportByStatusViewModel.ActualWorker);

                            exeReportByStatusViewModel.StkPerHr = GetStickHour(exeReportByStatusViewModel.PrdPerStk,
                                exeReportByStatusViewModel.ActualWorkHour);

                            plantTPKViewModel.StatusPerProcess.Add(exeReportByStatusViewModel);
                        }
                    }

                    //plantTPKViewModel.TotalWorkHour = _exeReportBLL.GetTotalWorkHour(report);

                    var totalWorkHour = _exeReportBLL.GetTotalWorkHour(report);
                    plantTPKViewModel.TotalWorkHour = totalWorkHour;

                    //total work hour per proses
                    var inputProcess = Mapper.Map<List<ExeReportByStatusDTO>>(plantTPKViewModel.StatusPerProcess);
                    plantTPKViewModel.TotalActualWorkHourPerProcess =
                        _exeReportBLL.GetTotalActualWorkPerProcess(inputProcess);

                    //Total Production Stick Per Process
                    plantTPKViewModel.TotalProductionPerProcess =
                        _exeReportBLL.GetTotalProductionStickPerHour(inputProcess);

                    // Total Stick Per Hour
                    var stickPerHour = plantTPKViewModel.TotalActualWorkHourPerProcess == "0" ? 0 : Convert.ToDouble(plantTPKViewModel.TotalProductionPerProcess) /
                                       Convert.ToDouble(plantTPKViewModel.TotalActualWorkHourPerProcess);

                    plantTPKViewModel.TotalStickHourPerProcess = stickPerHour.ToString("F2", ci);

                    //Total

                    statusPerProcess.Add(plantTPKViewModel);

                }

                GetBalanceIndex(statusPerProcess);

                return statusPerProcess;
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Exe Report By Status - ReportStatusIndexResult");
                return null;
            }
        }
        
        private void GetBalanceIndex(List<ExeReportByStatusCompositeViewModel<ExeReportByStatusViewModel>> listData)
        {
            try
            {
                //get the highest total per process
                var ci = CultureInfo.CurrentCulture;
                double total = 0;
                foreach (var exeReportByStatusCompositeViewModel in listData)
                {
                    //var sumProcess = exeReportByStatusCompositeViewModel.StatusPerProcess.Sum(c => c.StkPerHr);
                    var sumProcess = Convert.ToDouble(exeReportByStatusCompositeViewModel.TotalStickHourPerProcess);
                    exeReportByStatusCompositeViewModel.TotalStickHourPerProcess =
                        Convert.ToDouble(sumProcess).ToString("0.##", ci);
                    if (sumProcess > total)
                        total = sumProcess;
                }


                //put the value 
                foreach (var exeReportByStatusCompositeViewModel in listData)
                {
                    if (total > 0)
                    {
                        //var sumProcess = exeReportByStatusCompositeViewModel.StatusPerProcess.Sum(c => c.StkPerHr);
                        var sumProcess = Convert.ToDouble(exeReportByStatusCompositeViewModel.TotalStickHourPerProcess);
                        exeReportByStatusCompositeViewModel.TotalBalanceIndexPerProcess =
                            Math.Round((Convert.ToDouble(sumProcess)/total), 3).ToString("0.##", ci);
                    }
                    else
                        exeReportByStatusCompositeViewModel.TotalBalanceIndexPerProcess = "0";

                }

            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { listData }, "Exe Report By Status - GetBalanceIndex");
            }
        }
        
        private double GetStickHourPeople(int? prodStick, double? actualWorkHour, double? actual)
        {
            if (!actualWorkHour.HasValue || !actual.HasValue)
                return 0;
            if (actualWorkHour == 0 || actual == 0)
                return 0;
            if (!prodStick.HasValue)
                return 0;
            
            return Math.Round((prodStick.Value/actualWorkHour.Value/actual.Value), 2);
        }

        private double GetStickHour(int? prodStick, double? actualWorkHour)
        {
            if (!actualWorkHour.HasValue)
                return 0;
            if (actualWorkHour == 0 )
                return 0;
            if (!prodStick.HasValue)
                return 0;

            return Math.Round((prodStick.Value / actualWorkHour.Value), 2);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string unitCode, int? shift, string brand, string filterType, int? yearFrom, int? monthFrom, int? yearTo, int? monthTo, int? weekFrom, int? weekTo, DateTime? dateFrom, DateTime? dateTo, string brandGroupCode)
        {
            try
            {
                var input = new GetExeReportByStatusInput
                {
                    LocationCode = locationCode,
                    UnitCode = unitCode,
                    Shift = shift,
                    BrandCode = brand,
                    BrandGroupCode = brandGroupCode,
                    FilterType = filterType,
                    YearFrom = yearFrom,
                    MonthFrom = monthFrom,
                    YearTo = yearTo,
                    MonthTo = monthTo,
                    WeekFrom = weekFrom,
                    WeekTo = weekTo,
                    DateFrom = dateFrom,
                    DateTo = dateTo
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

                input.SortExpression = "ProcessGroup";
                input.SortOrder = "ASC";

                var ci = CultureInfo.CurrentCulture;

                //var listProcess = _masterDataBLL.GetAllActiveMasterProcess()
                //    .OrderBy(p => p.ProcessOrder)
                //    .ToList();

                //var ExeReportByGroups = _exeReportBLL.GetReportByStatus(input);
                //var ExeReportByGroupsMonthly = _exeReportBLL.GetReportByStatusMonthly(input);
                //var ExeReportByGroupsWeekly = _exeReportBLL.GetReportByStatusWeekly(input);
                int TActualWorker = 0;
                double TActualAbsWorker = 0;
                double TActualWorkHour = 0;
                double TPrdPerStk = 0;
                double TStkPerHrPerPpl = 0;
                double TStkPerHr = 0;
                decimal TBalanceIndex = 0;
                double AVGTBalanceIndex = 0;
                var totalWorkHour = "";
                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExeReportByStatus + ".xlsx";
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

                    SLStyle styleTotal = slDoc.CreateStyle();
                    styleTotal.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Font.FontName = "Calibri";
                    styleTotal.Font.FontSize = 10;
                    styleTotal.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);

                    var filter = input.FilterType;
                    var statusPerProcess = _reportByStatusService.GetReportByStatus(input);

                    slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    slDoc.SetCellValue(4, 2, ": " + unitCode);
                    slDoc.SetCellValue(5, 2, ": " + shift);
                    slDoc.SetCellValue(6, 2, ": " + brandGroupCode);
                    slDoc.SetCellValue(7, 2, ": " + brand);

                    if (filter == "YearMonth")
                    {
                        slDoc.SetCellValue(3, 7,
                            "From Year : " + yearFrom.ToString() + "  Month : " + monthFrom.ToString() +
                            "            To Year : " + yearTo.ToString() + "  Month : " + monthTo.ToString());
                        //slDoc.SetCellValue(3, 6, "Month     : " + monthFrom.ToString());
                        //slDoc.SetCellValue(3, 7, "To Year   : " + yearTo.ToString());
                        //slDoc.SetCellValue(3, 8, "Month     : " + monthTo.ToString());
                    }
                    else if (filter == "YearWeek")
                    {
                        slDoc.SetCellValue(3, 7,
                            "From Year : " + yearFrom.ToString() + "  Week : " + weekFrom.ToString() +
                            "            To Year : " + yearTo.ToString() + " Week : " + weekTo.ToString());
                        //slDoc.SetCellValue(3, 6, "Week      : " + weekFrom.ToString());
                        //slDoc.SetCellValue(3, 7, "To Year   : " + yearTo.ToString());
                        //slDoc.SetCellValue(3, 8, "Week      : " + weekTo.ToString());
                    }
                    else
                    {
                        if (filter == "Year")
                        {
                            slDoc.SetCellValue(3, 7, "Year : " + yearFrom.ToString());
                        }
                        else
                        {
                            slDoc.SetCellValue(3, 6, "From Date : ");
                            slDoc.SetCellValue(3, 7, dateFrom.Value.ToString(Constants.DefaultDateOnlyFormat));
                            slDoc.SetCellValue(3, 8, "To : " + dateTo.Value.ToString(Constants.DefaultDateOnlyFormat));
                        }
                    }

                    //row values
                    var counter = 0;
                    var iRow = 11;

                    foreach (var item in statusPerProcess)
                    {
                        slDoc.SetCellValue(iRow, 1, item.ProcessGroup);

                        foreach (var item2 in item.listProcess)
                        {

                            //if (iRow % 2 == 0)
                            //{
                            //    style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                            //}
                            slDoc.SetCellValue(iRow, 2, item2.StatusEmp);
                            slDoc.SetCellValue(iRow, 3, item2.ActualWorker);
                            slDoc.SetCellValue(iRow, 4, item2.ActualAbsWorker);
                            slDoc.SetCellValue(iRow, 5, item2.ActualWorkHourPerDay);
                            slDoc.SetCellValue(iRow, 6, item2.ProductionStick);
                            slDoc.SetCellValue(iRow, 7, item2.StickHourPeople);
                            slDoc.SetCellValue(iRow, 8, item2.StickHour);
                            //slDoc.SetCellValue(iRow, 9, item2.BalanceIndex.HasValue ? (item2.BalanceIndex != 0 ? item2.BalanceIndex.Value.ToString("F3", ci) : "0") : "0");
                            slDoc.SetCellStyle(iRow, 1, iRow, 9, style);

                            //TActualWorker = TActualWorker + (item2.ActualWorker.HasValue ? item2.ActualWorker.Value : 0);

                            //TActualAbsWorker = TActualAbsWorker +
                            //                   (item2.ActualAbsWorker != null ? item2.ActualAbsWorker.Value : 0);
                            //TActualWorkHour = TActualWorkHour +
                            //                  (item2.ActualWorkHour != null ? item2.ActualWorkHour.Value : 0);

                            //TPrdPerStk = TPrdPerStk + (item2.PrdPerStk.HasValue ? item2.PrdPerStk.Value : 0);

                            //TStkPerHrPerPpl = TStkPerHrPerPpl +
                            //                  (item2.StkPerHrPerPpl != null ? item2.StkPerHrPerPpl.Value : 0);
                            //TStkPerHr = TStkPerHr + (item2.StkPerHr != null ? item2.StkPerHr.Value : 0);
                            //TBalanceIndex = TBalanceIndex + (item2.BalanceIndex != null ? item2.BalanceIndex.Value : 0);

                            counter++;
                            iRow++;
                        }

                        var doubleTotalActualWorker = Convert.ToDouble(TActualWorker);

                        slDoc.SetCellValue(iRow, 1, "Total");
                        slDoc.MergeWorksheetCells(iRow, 1, iRow, 2);
                        slDoc.SetCellValue(iRow, 3, item.TotalActual);
                        slDoc.SetCellValue(iRow, 4, item.TotalAbsen);
                        slDoc.SetCellValue(iRow, 5, item.TotalWorkHourPerDay);
                        slDoc.SetCellValue(iRow, 6, item.TotalProductionStick);
                        slDoc.SetCellValue(iRow, 7, item.TotalStickHourPeople);
                        slDoc.SetCellValue(iRow, 8, item.TotalStickHour);
                        slDoc.SetCellValue(iRow, 9, item.TotalBalanceIndex);

                        var doubleTotalActualWorkHourPerProcess =
                            GenericHelper.ConvertStringToDoublel(item.TotalWorkHourPerDay);


                        //if (doubleTotalActualWorkHourPerProcess > 0
                        //    && TActualWorker > 0)
                        //{
                        //    //TPrdPerStk = TPrdPerStk / TActualWorker / doubleTotalActualWorkHourPerProcess;

                        //    slDoc.SetCellValue(iRow, 6, TPrdPerStk.ToString("0.##", CultureInfo.CurrentCulture));
                        //}
                        //else
                        //{
                        //    slDoc.SetCellValue(iRow, 6, "0");
                        //}
                        ////slDoc.SetCellValue(iRow, 6, TPrdPerStk.ToString());

                        //TStkPerHrPerPpl = (TPrdPerStk/Convert.ToDouble(item.TotalActualWorkHourPerProcess))/
                        //                  doubleTotalActualWorker;
                        //slDoc.SetCellValue(iRow, 7, TStkPerHrPerPpl.ToString("0.##"));

                        //generate total stick per hour

                        //TStkPerHr = TPrdPerStk / Convert.ToDouble(item.TotalActualWorkHourPerProcess);
                        //slDoc.SetCellValue(iRow, 8, TStkPerHr.ToString("F2", ci));
                        //slDoc.SetCellValue(iRow, 8, item.TotalStickHourPerProcess);
                        //slDoc.SetCellValue(iRow, 9, item.TotalBalanceIndexPerProcess);

                        //AVGTBalanceIndex = AVGTBalanceIndex + Convert.ToDouble(item.TotalBalanceIndexPerProcess) / 1000;
                        AVGTBalanceIndex = AVGTBalanceIndex + Convert.ToDouble(item.TotalBalanceIndex);
                        totalWorkHour = item.TotalWorkHour;

                        TActualWorker = 0;
                        TActualAbsWorker = 0;
                        TActualWorkHour = 0;
                        TPrdPerStk = 0;
                        TStkPerHrPerPpl = 0;
                        TStkPerHr = 0;
                        TBalanceIndex = 0;

                        slDoc.SetCellStyle(iRow, 1, iRow, 9, styleTotal);
                        //slDoc.AutoFitColumn(7,8);
                        iRow++;
                    }

                    AVGTBalanceIndex = AVGTBalanceIndex/statusPerProcess.Count();
                    slDoc.MergeWorksheetCells(iRow, 1, iRow, 7);
                    slDoc.SetCellValue(iRow, 1, "Average Balance Index");
                    slDoc.MergeWorksheetCells(iRow, 8, iRow, 9);
                    slDoc.SetCellValue(iRow, 8, AVGTBalanceIndex.ToString("0.##"));

                    slDoc.SetCellStyle(iRow, 1, iRow, 9, styleTotal);
                    slDoc.MergeWorksheetCells(iRow + 1, 1, iRow + 1, 7);
                    slDoc.SetCellValue(iRow + 1, 1, "Total Work Hour");
                    slDoc.MergeWorksheetCells(iRow + 1, 8, iRow + 1, 9);
                    slDoc.SetCellValue(iRow + 1, 8, totalWorkHour);

                    slDoc.SetCellStyle(iRow + 1, 1, iRow + 1, 9, styleTotal);

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
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "ProductionExecution_Reports_ProductionStatus_" + DateTime.Now.ToShortDateString() +
                               ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, shift, brand, filterType, yearFrom, monthFrom, yearTo, monthTo, weekFrom, weekTo, dateFrom, dateTo, brandGroupCode }, "Exe Report By Status - Generate Excel");
                return null;
            }
        }

      
        //private List<ExeReportByStatusCompositeViewModel<ExeReportByStatusMonthlyViewModel>> YearMonthIndexResult(GetExeReportByStatusInput criteria, List<MstGenProcessDTO> listProcess)
        //{
        //    var statusPerProcess = new List<ExeReportByStatusCompositeViewModel<ExeReportByStatusMonthlyViewModel>>();
        //    var report = _exeReportBLL.GetReportByStatusMonthly(criteria);



        //    foreach (var process in listProcess.OrderBy(c => c.ProcessIdentifier))
        //    {
        //        var processGroup = process.ProcessGroup;
        //        var plantTPKViewModel = new ExeReportByStatusCompositeViewModel<ExeReportByStatusMonthlyViewModel>
        //        {
        //            ProcessGroup = processGroup,
        //            LocationCode = criteria.LocationCode,
        //            UnitCode = criteria.UnitCode,
        //            BrandGroupCode = criteria.BrandGroupCode,
        //           // TotalWorkHour =  totalWorkHour
        //        };

        //        var statusEmps = report.Where(m => m.ProcessGroup == processGroup).ToList();
        //        if (!statusEmps.Any()) continue;

        //        //foreach (var statusEmp in statusEmps.Select(Mapper.Map<ExeReportByStatusMonthlyViewModel>))
        //        //{
        //        //    plantTPKViewModel.StatusPerProcess.Add(statusEmp);
        //        //}

        //        var listStatusProcess = statusEmps.Select(Mapper.Map<ExeReportByStatusMonthlyViewModel>).ToList();

        //        if (listStatusProcess.Count > 0)
        //        {
        //            var listGroup = listStatusProcess.GroupBy(c => new
        //            {
        //                c.ProcessGroup,
        //                c.StatusEmp,
        //                c.LocationCode,
        //                c.UnitCode,
        //                c.Shift,

        //            }).Select(x => new ExeReportByStatusMonthlyViewModel()
        //            {
        //                ProcessGroup = x.Key.ProcessGroup,
        //                StatusEmp = x.Key.StatusEmp,
        //                LocationCode = x.Key.LocationCode,
        //                UnitCode = x.Key.UnitCode,
        //                Shift = x.Key.Shift,
        //                BrandCode = criteria.BrandCode,
        //                ActualWorker = x.Sum(c => c.ActualWorker),
        //                ActualAbsWorker = x.Sum(c => c.ActualAbsWorker),
        //                ActualWorkHour = x.Sum(c => c.ActualWorkHour),
        //                PrdPerStk = x.Sum(c => c.PrdPerStk),
        //                StkPerHrPerPpl =
        //                    Convert.ToDouble(Convert.ToDouble(x.Sum(c => c.StkPerHrPerPpl).ToString()).ToString("f2")),
        //                StkPerHr = Convert.ToDouble(Convert.ToDouble(x.Sum(c => c.StkPerHr).ToString()).ToString("f2")),
        //                BalanceIndex = x.Sum(c => c.BalanceIndex)
        //            });

        //            var listActualWorkHour = _exeReportBLL.GetActualWorkMonthly(report);

        //            foreach (var exeReportByStatusViewModel in listGroup)
        //            {
        //                foreach (var exeReportByStatusWeeklyOutput in listActualWorkHour)
        //                {
        //                    if (exeReportByStatusViewModel.ProcessGroup ==
        //                        exeReportByStatusWeeklyOutput.ProcessGroup &&
        //                        exeReportByStatusViewModel.StatusEmp == exeReportByStatusWeeklyOutput.StatusEmp)
        //                    {
        //                        exeReportByStatusViewModel.ActualWorkHour =
        //                            exeReportByStatusWeeklyOutput.ActualWorkHour;
        //                    }
        //                }

        //                exeReportByStatusViewModel.StkPerHrPerPpl =
        //                    GetStickHourPeople(exeReportByStatusViewModel.PrdPerStk,
        //                        exeReportByStatusViewModel.ActualWorkHour, exeReportByStatusViewModel.ActualWorker);

        //                exeReportByStatusViewModel.StkPerHr = GetStickHour(exeReportByStatusViewModel.PrdPerStk,
        //                    exeReportByStatusViewModel.ActualWorkHour);


        //                plantTPKViewModel.StatusPerProcess.Add(exeReportByStatusViewModel);
        //            }
        //        }

        //        var totalWorkHour = _exeReportBLL.GetTotalWorkHourMonthly(report);
        //        plantTPKViewModel.TotalWorkHour = totalWorkHour;

        //        //total work hour per proses
        //        var inputProcess = Mapper.Map<List<ExeReportByStatusDTO>>(plantTPKViewModel.StatusPerProcess);
        //        plantTPKViewModel.TotalActualWorkHourPerProcess =
        //            _exeReportBLL.GetTotalActualWorkPerProcess(inputProcess);

        //        statusPerProcess.Add(plantTPKViewModel);
        //    }


        //    GetBalanceIndex(statusPerProcess);

        //    return statusPerProcess;
        //}

        //private List<ExeReportByStatusCompositeViewModel<ExeReportByStatusWeeklyViewModel>> YearWeekIndexResult(GetExeReportByStatusInput criteria, List<MstGenProcessDTO> listProcess)
        //{
        //    var statusPerProcess = new List<ExeReportByStatusCompositeViewModel<ExeReportByStatusWeeklyViewModel>>();
        //    var report = _exeReportBLL.GetReportByStatusWeekly(criteria);


        //    foreach (var process in listProcess.OrderBy(c => c.ProcessIdentifier))
        //    {
        //        var processGroup = process.ProcessGroup;
        //        var plantTPKViewModel = new ExeReportByStatusCompositeViewModel<ExeReportByStatusWeeklyViewModel>
        //        {
        //            ProcessGroup = processGroup,
        //            LocationCode = criteria.LocationCode,
        //            UnitCode = criteria.UnitCode,
        //            BrandGroupCode = criteria.BrandGroupCode,
                    
        //        };

        //        var statusEmps = report.Where(m => m.ProcessGroup == processGroup).ToList();
        //        if (!statusEmps.Any()) continue;

        //        var listStatusProcess = statusEmps.Select(Mapper.Map<ExeReportByStatusWeeklyViewModel>).ToList();

        //        if (listStatusProcess.Count > 0)
        //        {
        //            var listGroup = listStatusProcess.GroupBy(c => new
        //            {
        //                c.ProcessGroup,
        //                c.StatusEmp,
        //                c.LocationCode,
        //                c.UnitCode,
        //                c.Shift,
        //                //c.BrandCode,
        //                //c.BrandGroupCode
        //            }).Select(x => new ExeReportByStatusWeeklyViewModel()
        //            {
        //                ProcessGroup = x.Key.ProcessGroup,
        //                StatusEmp = x.Key.StatusEmp,
        //                LocationCode = x.Key.LocationCode,
        //                UnitCode = x.Key.UnitCode,
        //                Shift = x.Key.Shift,
        //                BrandCode = criteria.BrandCode,
        //                //BrandGroupCode = criteria.BrandGroupCode,
        //                ActualWorker = x.Sum(c => c.ActualWorker),
        //                ActualAbsWorker = x.Sum(c => c.ActualAbsWorker),
        //                ActualWorkHour = x.Sum(c => c.ActualWorkHour),
        //                PrdPerStk = x.Sum(c => c.PrdPerStk),
        //                StkPerHrPerPpl = Convert.ToDouble(Convert.ToDouble(x.Sum(c => c.StkPerHrPerPpl).ToString()).ToString("f2")),
        //                StkPerHr = Convert.ToDouble(Convert.ToDouble(x.Sum(c => c.StkPerHr).ToString()).ToString("f2")),
        //                BalanceIndex = x.Sum(c => c.BalanceIndex)
        //            });

        //            var listActualWorkHour = _exeReportBLL.GetActualWorkWeekly(report);

        //            foreach (var exeReportByStatusViewModel in listGroup)
        //            {
        //                foreach (var exeReportByStatusWeeklyOutput in listActualWorkHour)
        //                {
        //                    if (exeReportByStatusViewModel.ProcessGroup ==
        //                        exeReportByStatusWeeklyOutput.ProcessGroup &&
        //                        exeReportByStatusViewModel.StatusEmp == exeReportByStatusWeeklyOutput.StatusEmp)
        //                    {
        //                        exeReportByStatusViewModel.ActualWorkHour =
        //                            exeReportByStatusWeeklyOutput.ActualWorkHour;
        //                    }
        //                }


        //                exeReportByStatusViewModel.StkPerHrPerPpl =
        //                    GetStickHourPeople(exeReportByStatusViewModel.PrdPerStk,
        //                        exeReportByStatusViewModel.ActualWorkHour, exeReportByStatusViewModel.ActualWorker);

        //                exeReportByStatusViewModel.StkPerHr = GetStickHour(exeReportByStatusViewModel.PrdPerStk,
        //                    exeReportByStatusViewModel.ActualWorkHour);

        //                plantTPKViewModel.StatusPerProcess.Add(exeReportByStatusViewModel);
        //            }
        //        }
        //        var totalWorkHour = _exeReportBLL.GetTotalWorkHourWeekly(report);
        //        plantTPKViewModel.TotalWorkHour = totalWorkHour;

        //        //total work hour per proses
        //        var inputProcess = Mapper.Map<List<ExeReportByStatusDTO>>(plantTPKViewModel.StatusPerProcess);
        //        plantTPKViewModel.TotalActualWorkHourPerProcess =
        //            _exeReportBLL.GetTotalActualWorkPerProcess(inputProcess);

        //        statusPerProcess.Add(plantTPKViewModel);
        //    }
        //    GetBalanceIndex(statusPerProcess);

        //    return statusPerProcess;
        //}

        #region NEW 
            
        #endregion
    }
}