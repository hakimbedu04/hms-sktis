using AutoMapper;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using SKTISWebsite.Code;
using SKTISWebsite.Models.WagesReportAbsents;
using SKTISWebsite.Models.MasterGenWeek;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using SKTISWebsite.Models.ExePlantWorkerAbsenteeism;
using SKTISWebsite.Models.Common;
using System.IO;
using HMS.SKTIS.Core;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;

namespace SKTISWebsite.Controllers
{
    public class WagesReportAbsentsController : BaseController
    {
        private IApplicationService _applicationService;
        private IMasterDataBLL _masterDataBll;
        private IExecutionPlantBLL _exe;
        private IPlantWagesExecutionBLL _plantWages;

        public WagesReportAbsentsController(IApplicationService applicationService, IMasterDataBLL masterDataBll, IExecutionPlantBLL exe, IPlantWagesExecutionBLL plantWages)
        {
            _applicationService = applicationService;
            _masterDataBll = masterDataBll;
            _exe = exe;
            _plantWages = plantWages;
            SetPage("PlantWages/Execution/Report/AbsentsReport");
        }

        // GET: PlantWagesReportAbsents
        public ActionResult Index()
        {
            var week = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(DateTime.Now.Year, week.HasValue ? week.Value : 1);

            var init = new InitWagesReportAbsentsViewModel
            {
                LocationSelectList = _applicationService.GetPLNTLocationCodeSelectListCompat(),
                YearSelectList = _applicationService.GetGenWeekYears(),
                MonthSelectList = GenericHelper.GetListOfMonth().Select(m => new SelectListItem { Text = m.Key.ToString(), Value = m.Key.ToString() }).ToList(),
                DefaultLocation = _applicationService.GetPLNTLocationCodeSelectList() == null ? string.Empty : _applicationService.GetPLNTLocationCodeSelectList().Skip(0).First().Text,
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                DefaultDateFrom = mstGenWeek.StartDate
            };
            return View("Index", init);
        }

        // GET: PlantWagesReportAbsents/WagesReportAbsentsGroup/1
        public ActionResult WagesReportAbsentsGroup()
        {
            var week = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(DateTime.Now.Year, week.HasValue ? week.Value : 1);

            var uri = Request.RawUrl.Split(new[] { '/' });

            var init = new InitWagesReportAbsentsViewModel
            {
                LocationSelectList = _applicationService.GetPLNTLocationCodeSelectList(),
                YearSelectList = _applicationService.GetGenWeekYears(),
                MonthSelectList = GenericHelper.GetListOfMonth().Select(m => new SelectListItem { Text = m.Key.ToString(), Value = m.Key.ToString() }).ToList(),
                DefaultLocation = uri[3],
                //DefaultBrand = uri[4],
                DefaultUnit = uri[4],
                DefaultGroup = uri[5],
                DefaultProcess = uri[6],
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                DefaultDateFrom = mstGenWeek.StartDate
            };
            return View("WagesReportAbsentsGroup", init);
        }

        public ActionResult WagesReportAbsentsDetail()
        {
            var week = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(DateTime.Now.Year, week.HasValue ? week.Value : 1);
            var uri = Request.RawUrl.Split(new[] { '/' });
            var employeeId = uri[1] != "WagesReportAbsents" ? uri[4] : uri[3];

            var employee = _masterDataBll.GetMstEmployeeJobsDataActives(employeeId);

            var init = new InitWagesReportAbsentsDetailViewModel {
                YearSelectList = _applicationService.GetGenWeekYears(),
                MonthSelectList = GenericHelper.GetListOfMonth().Select(m => new SelectListItem { Text = m.Key.ToString(), Value = m.Key.ToString() }).ToList(),
                DefaultEmployeeID = employee == null ? "" : employee.EmployeeID,
                DefaultEmployee = employee == null ? "" : employee.EmployeeName,
                DefaultLocation = employee == null ? "" : employee.LocationCode,
                DefaultUnit = employee == null ? "" : employee.UnitCode,
                DefaultGroup = employee == null ? "" : employee.GroupCode,
                DefaultProcess = employee == null ? "" : employee.ProcessSettingsCode,
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                DefaultDateFrom = mstGenWeek.StartDate
            };
            return View("WagesReportAbsentsDetail", init);
        }

        // GET: PlantWagesReportAbsents/WagesReportAbsentsDetail/1
        //public ActionResult WagesReportAbsentsDetail(string id)
        //{
        //    var week = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
        //    var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(DateTime.Now.Year, week.HasValue ? week.Value : 1);
        //    var employee = _masterDataBll.GetMstEmployeeJobsDataActives(id);
        //    var employees = _masterDataBll.GetMstEmployeeJobsDataActives(new GetMstEmployeeJobsDataActivesInput());

        //    var init = new InitWagesReportAbsentsDetailViewModel
        //    {
        //        EmployeeSelectList = new SelectList(employees, "EmployeeID", "EmployeeID"),
        //        DefaultEmployee = id,
        //        LocationSelectList = _applicationService.GetLocationCodeSelectList(),
        //        YearSelectList = _applicationService.GetGenWeekYears(),
        //        MonthSelectList = GenericHelper.GetListOfMonth().Select(m => new SelectListItem { Text = m.Key.ToString(), Value = m.Key.ToString() }).ToList(),
        //        DefaultLocation = _applicationService.GetLocationCodeSelectList() == null ? string.Empty : employee.LocationCode,
        //        DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
        //        DefaultDateFrom = mstGenWeek.StartDate
        //    };
        //    return View("WagesReportAbsentsDetail", init);
        //}

        

        [HttpGet]
        public JsonResult GetEmployeeName(string employeeId)
        {
            var employeeName = _masterDataBll.GetMstEmployeeJobsDataActives(employeeId).EmployeeName;
            return Json(employeeName, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectList(string locationCode)
        {
            var result = _applicationService.GetUnitCodeSelectListByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcessByLocationCode(string locationCode)
        {
            var result = _applicationService.GetProcessGroupSelectListByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProcessFromProdcard(GetExePlantWorkerAbsenteeismInput criteria)
        {
            var input = new GetProductionCardInput();

            if (criteria.FilterType == "Year")
            {
                var startDate = new DateTime(criteria.Year, 1, 1);
                var endDate = new DateTime(criteria.Year, 12, DateTime.DaysInMonth(criteria.Year, 12));
                input.starDate = startDate;
                input.endDate = endDate;
            }

            if (criteria.FilterType == "YearMonth")
            {
                var startDate = new DateTime(criteria.YearMonthFrom, criteria.MonthFrom, 1);
                var endDate = new DateTime(criteria.YearMonthTo, criteria.MonthTo, DateTime.DaysInMonth(criteria.YearMonthTo, criteria.MonthTo));
                input.starDate = startDate;
                input.endDate = endDate;
            }

            if (criteria.FilterType == "YearWeek")
            {
                var startWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.YearWeekFrom, criteria.WeekFrom);
                var endWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.YearWeekTo, criteria.WeekTo);

                if (startWeek.StartDate.HasValue)
                    input.starDate = startWeek.StartDate.Value;

                if (endWeek.StartDate.HasValue)
                    input.endDate = endWeek.EndDate.Value;
            }

            if (criteria.FilterType == "Date")
            {
                input.starDate = criteria.DateFrom;
                input.endDate = criteria.DateTo;
            }

            input.Unit = criteria.UnitCode;
            input.LocationCode = criteria.LocationCode;
            var result = _applicationService.GetProcessGroupFromProdcard(input);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupCodeByLocationCode(string locationCode, string unit, string process, string productionDate, string page)
        {
            page = "wages";
            var result = _applicationService.GetGroupCodeFromProductionCardByLocation(locationCode, unit, process, !string.IsNullOrEmpty(productionDate) ? DateTime.Parse(productionDate) : DateTime.Now.Date, page);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcessGroup(string locationCode, string unitCode, string groupCode)
        {
            var dbResult = _masterDataBll.GetProcessGroup(locationCode,unitCode,groupCode);
            return Json(dbResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetWagesReportAbsents(GetWagesReportAbsentViewInput criteria)
        {
            if (criteria.FilterType == "Date" && criteria.DateFrom == criteria.DateTo)
            {
                var result = _plantWages.GetWagesReportMore(criteria);
                var pageResult = new PageResult<GetWagesReportAbsentViewModel>();

                pageResult.TotalRecords = result.Count;
                //pageResult.TotalPages = (result.Count / criteria.PageSize) + (result.Count % criteria.PageSize != 0 ? 1 : 0);
                //var data = result.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
                pageResult.TotalPages = 1;
                var data = result;
                pageResult.Results = Mapper.Map<List<GetWagesReportAbsentViewModel>>(data);
                return Json(pageResult);
            }
            else
            {
                if (criteria.FilterType == "YearWeek")
                {
                    var startWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.YearWeekFrom, criteria.WeekFrom);
                    var endWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.YearWeekTo, criteria.WeekTo);

                    if (startWeek.StartDate.HasValue)
                        criteria.DateFrom = startWeek.StartDate.Value;

                    if (endWeek.StartDate.HasValue)
                        criteria.DateTo = endWeek.EndDate.Value;

                }

                if (criteria.FilterType == "YearMonth")
                {
                    criteria.DateFrom = new DateTime(criteria.YearMonthFrom, criteria.MonthFrom, 1);
                    criteria.DateTo = new DateTime(criteria.YearMonthTo, criteria.MonthTo, DateTime.DaysInMonth(criteria.YearMonthTo, criteria.MonthTo));

                }

                if (criteria.FilterType == "Year")
                {
                    criteria.DateFrom = new DateTime(criteria.Year, 1, 1);
                    criteria.DateTo = new DateTime(criteria.Year, 12, DateTime.DaysInMonth(criteria.Year, 12));

                }

                var result = _plantWages.GetWagesReportMore(criteria);
                var pageResult = new PageResult<GetWagesReportAbsentViewModel>();

                pageResult.TotalRecords = result.Count;
                //pageResult.TotalPages = (result.Count / criteria.PageSize) + (result.Count % criteria.PageSize != 0 ? 1 : 0);
                //var data = result.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
                pageResult.TotalPages = 1;
                var data = result;
                pageResult.Results = Mapper.Map<List<GetWagesReportAbsentViewModel>>(data);
                return Json(pageResult);

            }

        }

        [HttpPost]
        public JsonResult GetWagesReportAbsentsGroup(GetWagesReportAbsentViewInput criteria)
        {
            if (criteria.FilterType == "Date" && criteria.DateFrom == criteria.DateTo)
            {
                var result = _plantWages.GetWagesReportGroup(criteria);
                var pageResult = new PageResult<GetWagesReportAbsentDetailViewModel>();

                pageResult.TotalRecords = result.Count;
                //pageResult.TotalPages = (result.Count / criteria.PageSize) + (result.Count % criteria.PageSize != 0 ? 1 : 0);
                //var data = result.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
                pageResult.TotalPages = 1;
                var data = result;
                pageResult.Results = Mapper.Map<List<GetWagesReportAbsentDetailViewModel>>(data);
                return Json(pageResult);
            }
            else
            {
                if (criteria.FilterType == "YearWeek")
                {
                    var startWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.YearWeekFrom, criteria.WeekFrom);
                    var endWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.YearWeekTo, criteria.WeekTo);

                    if (startWeek.StartDate.HasValue)
                        criteria.DateFrom = startWeek.StartDate.Value;

                    if (endWeek.StartDate.HasValue)
                        criteria.DateTo = endWeek.EndDate.Value;

                }

                if (criteria.FilterType == "YearMonth")
                {
                    criteria.DateFrom = new DateTime(criteria.YearMonthFrom, criteria.MonthFrom, 1);
                    criteria.DateTo = new DateTime(criteria.YearMonthTo, criteria.MonthTo, DateTime.DaysInMonth(criteria.YearMonthTo, criteria.MonthTo));

                }

                if (criteria.FilterType == "Year")
                {
                    criteria.DateFrom = new DateTime(criteria.Year, 1, 1);
                    criteria.DateTo = new DateTime(criteria.Year, 12, DateTime.DaysInMonth(criteria.Year, 12));

                }

                var result = _plantWages.GetWagesReportGroup(criteria);
                var pageResult = new PageResult<GetWagesReportAbsentDetailViewModel>();

                pageResult.TotalRecords = result.Count;
                //pageResult.TotalPages = (result.Count / criteria.PageSize) + (result.Count % criteria.PageSize != 0 ? 1 : 0);
                //var data = result.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
                pageResult.TotalPages = 1;
                var data = result;
                pageResult.Results = Mapper.Map<List<GetWagesReportAbsentDetailViewModel>>(data);
                return Json(pageResult);

            }

        }

        [HttpPost]
        public JsonResult GetWagesReportAbsentsDetail(GetExePlantWorkerAbsenteeismInput criteria)
        {
            if (criteria.FilterType == "Date" && criteria.DateFrom == criteria.DateTo) {
                var result = _plantWages.GetWagesReportDetailPerEmployee(criteria.EmployeeID, criteria.DateFrom, criteria.DateTo);
                var viewModel = Mapper.Map<List<WagesReportDetailEmployeeViewModel>>(result);
                var pageResult = new PageResult<WagesReportDetailEmployeeViewModel>(viewModel);
                return Json(pageResult);
            }
            else {
                if (criteria.FilterType == "YearWeek") {
                    var startWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.YearWeekFrom, criteria.WeekFrom);
                    var endWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.YearWeekTo, criteria.WeekTo);

                    if (startWeek.StartDate.HasValue)
                        criteria.DateFrom = startWeek.StartDate.Value;

                    if (endWeek.StartDate.HasValue)
                        criteria.DateTo = endWeek.EndDate.Value;

                }

                if (criteria.FilterType == "YearMonth") {
                    criteria.DateFrom = new DateTime(criteria.YearMonthFrom, criteria.MonthFrom, 1);
                    criteria.DateTo = new DateTime(criteria.YearMonthTo, criteria.MonthTo, DateTime.DaysInMonth(criteria.YearMonthTo, criteria.MonthTo));

                }

                if (criteria.FilterType == "Year") {
                    criteria.DateFrom = new DateTime(criteria.Year, 1, 1);
                    criteria.DateTo = new DateTime(criteria.Year, 12, DateTime.DaysInMonth(criteria.Year, 12));

                }

                var result = _plantWages.GetWagesReportDetailPerEmployee(criteria.EmployeeID, criteria.DateFrom, criteria.DateTo);
                var viewModel = Mapper.Map<List<WagesReportDetailEmployeeViewModel>>(result);
                var pageResult = new PageResult<WagesReportDetailEmployeeViewModel>(viewModel);
                return Json(pageResult);
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcelAbsentsDetail(string locationCode, string unitCode, string process, int? year, string filterType,
            int? yearMonthFrom, int? monthFrom, int? yearMonthTo, int? monthTo,
            int? yearWeekFrom, int? weekFrom, int? yearWeekTo, int? weekTo,
            DateTime? dateFrom, DateTime? dateTo)
        {
            
            var input = new GetWagesReportAbsentViewInput
            {
                LocationCode = locationCode,
                UnitCode = unitCode == "" ? null : unitCode,
                Process = process == "" ? null : process,
                FilterType = filterType,
                YearMonthFrom = yearMonthFrom.Value,
                MonthFrom = monthFrom.Value,
                YearMonthTo = yearMonthTo.Value,
                MonthTo = monthTo.Value,
                YearWeekFrom = yearWeekFrom.Value,
                WeekFrom = weekFrom.Value,
                YearWeekTo = yearWeekTo.Value,
                WeekTo = weekTo.Value,
                DateFrom = dateFrom.Value,
                DateTo = dateTo.Value,
                Year = year.Value
            };

            var allLocations = _applicationService.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == locationCode)
                {
                    locationCompat = item.Text;
                }
            }

            var result = new List<GetWagesReportAbsentViewDTO>();
            if (input.FilterType == "Date" && input.DateFrom == input.DateTo)
            {
                result = _plantWages.GetWagesReport(input);
            }
            else
            {
                if (input.FilterType == "YearWeek")
                {
                    var startWeek = _masterDataBll.GetWeekByYearAndWeek(input.YearWeekFrom, input.WeekFrom);
                    var endWeek = _masterDataBll.GetWeekByYearAndWeek(input.YearWeekTo, input.WeekTo);

                    if (startWeek.StartDate.HasValue)
                        input.DateFrom = startWeek.StartDate.Value;

                    if (endWeek.StartDate.HasValue)
                        input.DateTo = endWeek.EndDate.Value;
                }
                if (input.FilterType == "YearMonth")
                {
                    input.DateFrom = new DateTime(input.YearMonthFrom, input.MonthFrom, 1);
                    input.DateTo = new DateTime(input.YearMonthTo, input.MonthTo, DateTime.DaysInMonth(input.YearMonthTo, input.MonthTo));
                }
                if (input.FilterType == "Year")
                {
                    input.DateFrom = new DateTime(input.Year, 1, 1);
                    input.DateTo = new DateTime(input.Year, 12, DateTime.DaysInMonth(input.Year, 12));
                }
                result = _plantWages.GetWagesReportMore(input);
            }

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.PlantWagesExcelTemplate.WagesReportAbsentsDetail + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                #region filter
                SLStyle style = slDoc.CreateStyle();
                style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                style.Font.FontName = "Calibri";
                style.Font.FontSize = 10;

                var filter = input.FilterType;
                var strUnitCode = unitCode == "" ? "All" : unitCode;
                var strProcess = process == "" ? "All" : process;
                process = process == "" ? "All" : process;
                slDoc.SetCellValue(3, 2, ": " + locationCompat);
                slDoc.SetCellValue(4, 2, ": " + strUnitCode);
                slDoc.SetCellValue(5, 2, ": " + strProcess);

                if (filter == "YearMonth")
                {
                    slDoc.SetCellValue(3, 7, "From Year : " + yearMonthFrom.ToString());
                    slDoc.SetCellValue(4, 7, "Month     : " + monthFrom.ToString());
                    slDoc.SetCellValue(5, 7, "To Year   : " + yearMonthTo.ToString());
                    slDoc.SetCellValue(6, 7, "Month     : " + monthTo.ToString());
                }
                else if (filter == "YearWeek")
                {
                    slDoc.SetCellValue(3, 7, "From Year : " + yearWeekFrom.ToString());
                    slDoc.SetCellValue(4, 7, "Week      : " + weekFrom.ToString());
                    slDoc.SetCellValue(5, 7, "To Year   : " + yearWeekTo.ToString());
                    slDoc.SetCellValue(6, 7, "Week      : " + weekTo.ToString());
                }
                else
                {
                    if (filter == "Year")
                    {
                        slDoc.SetCellValue(3, 7, "Year : " + year.ToString());
                    }
                    else
                    {
                        slDoc.SetCellValue(3, 7,
                            "From Date : " + dateFrom.Value.ToString(Constants.DefaultDateOnlyFormat));
                        slDoc.SetCellValue(4, 7, "To Date   : " + dateTo.Value.ToString(Constants.DefaultDateOnlyFormat));
                    }
                }
                #endregion

                var iRow = 9;

                foreach (var item in result)
                {
                    slDoc.SetCellValue(iRow, 1, item.BrandGroupCode);
                    slDoc.SetCellValue(iRow, 2, item.LocationCode);
                    slDoc.SetCellValue(iRow, 3, item.ProdUnit);
                    slDoc.SetCellValue(iRow, 4, item.ProdGroup);
                    slDoc.SetCellValue(iRow, 5, item.Terdaftar.Value);
                    slDoc.SetCellValue(iRow, 6, item.Alpa.Value);
                    slDoc.SetCellValue(iRow, 7, item.Ijin.Value);
                    slDoc.SetCellValue(iRow, 8, item.Sakit.Value);
                    slDoc.SetCellValue(iRow, 9, item.Cuti.Value);
                    slDoc.SetCellValue(iRow, 10, item.CutiHamil.Value);
                    slDoc.SetCellValue(iRow, 11, item.CutiTahunan.Value);
                    slDoc.SetCellValue(iRow, 12, item.Skorsing.Value);
                    slDoc.SetCellValue(iRow, 13, item.Keluar.Value);
                    slDoc.SetCellValue(iRow, 14, item.Masuk.Value);
                    slDoc.SetCellValue(iRow, 15, item.Turnover.ToString("N2", CultureInfo.CurrentCulture));
                    slDoc.SetCellValue(iRow, 16, item.TurnoverPersen.Value.ToString("N2", CultureInfo.CurrentCulture));
                    slDoc.SetCellValue(iRow, 17, item.Kehadiran.Value.ToString("N2", CultureInfo.CurrentCulture));
                    slDoc.SetCellValue(iRow, 18, item.Absensi.Value.ToString("N2", CultureInfo.CurrentCulture));

                    slDoc.SetCellStyle(iRow, 1, iRow, 18, style);
                    iRow++;
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
                slDoc.AutoFitColumn(1, 18);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "WagesReportAbsentsDetail_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public FileStreamResult GenerateExcelAbsentsEmployee(string locationCode, string unitCode, string process, int? year, string filterType, string group,
            int? yearMonthFrom, int? monthFrom, int? yearMonthTo, int? monthTo,
            int? yearWeekFrom, int? weekFrom, int? yearWeekTo, int? weekTo,
            DateTime? dateFrom, DateTime? dateTo) 
        {
            var input = new GetWagesReportAbsentViewInput {
                LocationCode = locationCode,
                UnitCode = unitCode == "" ? null : unitCode,
                Process = process == "" ? null : process,
                ProdGroup = group == "" ? null : group,
                FilterType = filterType,
                YearMonthFrom = yearMonthFrom.Value,
                MonthFrom = monthFrom.Value,
                YearMonthTo = yearMonthTo.Value,
                MonthTo = monthTo.Value,
                YearWeekFrom = yearWeekFrom.Value,
                WeekFrom = weekFrom.Value,
                YearWeekTo = yearWeekTo.Value,
                WeekTo = weekTo.Value,
                DateFrom = dateFrom.Value,
                DateTo = dateTo.Value,
                Year = year.Value
            };

            var allLocations = _applicationService.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations) {
                if (item.Value == locationCode) {
                    locationCompat = item.Text;
                }
            }

            var result = new List<GetWagesReportAbsentGroupViewDTO>();
            if (input.FilterType == "Date" && input.DateFrom == input.DateTo) {
                result = _plantWages.GetWagesReportGroup(input);
            }
            else {
                if (input.FilterType == "YearWeek") {
                    var startWeek = _masterDataBll.GetWeekByYearAndWeek(input.YearWeekFrom, input.WeekFrom);
                    var endWeek = _masterDataBll.GetWeekByYearAndWeek(input.YearWeekTo, input.WeekTo);

                    if (startWeek.StartDate.HasValue)
                        input.DateFrom = startWeek.StartDate.Value;

                    if (endWeek.StartDate.HasValue)
                        input.DateTo = endWeek.EndDate.Value;
                }
                if (input.FilterType == "YearMonth") {
                    input.DateFrom = new DateTime(input.YearMonthFrom, input.MonthFrom, 1);
                    input.DateTo = new DateTime(input.YearMonthTo, input.MonthTo, DateTime.DaysInMonth(input.YearMonthTo, input.MonthTo));
                }
                if (input.FilterType == "Year") {
                    input.DateFrom = new DateTime(input.Year, 1, 1);
                    input.DateTo = new DateTime(input.Year, 12, DateTime.DaysInMonth(input.Year, 12));
                }
                result = _plantWages.GetWagesReportGroup(input);
            }

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName)) {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.PlantWagesExcelTemplate.WagesReportAbsentsEmployee + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName)) {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName)) {
                #region filter
                SLStyle style = slDoc.CreateStyle();
                style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                style.Font.FontName = "Calibri";
                style.Font.FontSize = 10;

                var filter = input.FilterType;
                var strUnitCode = unitCode == "" ? "All" : unitCode;
                var strProcess = process == "" ? "All" : process;
                var strGroup = group == "" ? "All" : group;
                process = process == "" ? "All" : process;
                slDoc.SetCellValue(3, 2, ": " + locationCompat);
                slDoc.SetCellValue(4, 2, ": " + strUnitCode);
                slDoc.SetCellValue(5, 2, ": " + strProcess);
                slDoc.SetCellValue(6, 2, ": " + strGroup);

                if (filter == "YearMonth") {
                    slDoc.SetCellValue(3, 7, "From Year : " + yearMonthFrom.ToString());
                    slDoc.SetCellValue(4, 7, "Month     : " + monthFrom.ToString());
                    slDoc.SetCellValue(5, 7, "To Year   : " + yearMonthTo.ToString());
                    slDoc.SetCellValue(6, 7, "Month     : " + monthTo.ToString());
                }
                else if (filter == "YearWeek") {
                    slDoc.SetCellValue(3, 7, "From Year : " + yearWeekFrom.ToString());
                    slDoc.SetCellValue(4, 7, "Week      : " + weekFrom.ToString());
                    slDoc.SetCellValue(5, 7, "To Year   : " + yearWeekTo.ToString());
                    slDoc.SetCellValue(6, 7, "Week      : " + weekTo.ToString());
                }
                else {
                    if (filter == "Year") {
                        slDoc.SetCellValue(3, 7, "Year : " + year.ToString());
                    }
                    else {
                        slDoc.SetCellValue(3, 7,
                            "From Date : " + dateFrom.Value.ToString(Constants.DefaultDateOnlyFormat));
                        slDoc.SetCellValue(4, 7, "To Date   : " + dateTo.Value.ToString(Constants.DefaultDateOnlyFormat));
                    }
                }
                #endregion

                var iRow = 9;

                foreach (var item in result) {
                    slDoc.SetCellValue(iRow, 1, item.LocationCode);
                    slDoc.SetCellValue(iRow, 2, item.ProdUnit);
                    slDoc.SetCellValue(iRow, 3, item.EmployeeID);
                    slDoc.SetCellValue(iRow, 4, item.EmployeeName);
                    slDoc.SetCellValue(iRow, 5, item.Alpa.Value);
                    slDoc.SetCellValue(iRow, 6, item.Ijin.Value);
                    slDoc.SetCellValue(iRow, 7, item.Sakit.Value);
                    slDoc.SetCellValue(iRow, 8, item.Cuti.Value);
                    slDoc.SetCellValue(iRow, 9, item.CutiHamil.Value);
                    slDoc.SetCellValue(iRow, 10, item.CutiTahunan.Value);
                    slDoc.SetCellValue(iRow, 11, item.Skorsing.Value);
                    slDoc.SetCellValue(iRow, 12, item.HKNTotal.Value);
                    slDoc.SetCellValue(iRow, 13, item.AbsentTotal.Value);
                    slDoc.SetCellValue(iRow, 14, item.ProductionTotal.Value.ToString("N2", CultureInfo.CurrentCulture));
                    slDoc.SetCellValue(iRow, 15, item.JKTotal.Value);
                    slDoc.SetCellValue(iRow, 16, item.Productivity.Value.ToString("N2", CultureInfo.CurrentCulture));

                    slDoc.SetCellStyle(iRow, 1, iRow, 16, style);
                    iRow++;
                }

                var slSheetProtection = new SLSheetProtection {
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
                slDoc.AutoFitColumn(1, 16);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "WagesReportAbsentsEmployee_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public FileStreamResult GenerateExcelAbsentsEmployeeDetail(string employeeID, string filterType, string group, int? year,
            int? yearMonthFrom, int? monthFrom, int? yearMonthTo, int? monthTo,
            int? yearWeekFrom, int? weekFrom, int? yearWeekTo, int? weekTo,
            DateTime? dateFrom, DateTime? dateTo) 
        {

            var employee = _masterDataBll.GetMstEmployeeJobsDataActives(employeeID);

            var input = new GetWagesReportAbsentViewInput {
                LocationCode = employee == null ? "" : employee.LocationCode,
                UnitCode = employee == null ? "" : employee.UnitCode,
                Process = employee == null ? "" : employee.ProcessSettingsCode,
                ProdGroup = employee == null ? "" : employee.GroupCode,
                FilterType = filterType,
                YearMonthFrom = yearMonthFrom.Value,
                MonthFrom = monthFrom.Value,
                YearMonthTo = yearMonthTo.Value,
                MonthTo = monthTo.Value,
                YearWeekFrom = yearWeekFrom.Value,
                WeekFrom = weekFrom.Value,
                YearWeekTo = yearWeekTo.Value,
                WeekTo = weekTo.Value,
                DateFrom = dateFrom.Value,
                DateTo = dateTo.Value,
                Year = year.Value
            };

            var allLocations = _applicationService.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations) {
                if (item.Value == (employee == null ? "" : employee.LocationCode)) {
                    locationCompat = item.Text;
                }
            }

            var result = new List<WagesReportDetailEmployeeDTO>();
            if (input.FilterType == "Date" && input.DateFrom == input.DateTo) {
                result = _plantWages.GetWagesReportDetailPerEmployee(employeeID, input.DateFrom, input.DateTo).ToList();
            }
            else {
                if (input.FilterType == "YearWeek") {
                    var startWeek = _masterDataBll.GetWeekByYearAndWeek(input.YearWeekFrom, input.WeekFrom);
                    var endWeek = _masterDataBll.GetWeekByYearAndWeek(input.YearWeekTo, input.WeekTo);

                    if (startWeek.StartDate.HasValue)
                        input.DateFrom = startWeek.StartDate.Value;

                    if (endWeek.StartDate.HasValue)
                        input.DateTo = endWeek.EndDate.Value;
                }
                if (input.FilterType == "YearMonth") {
                    input.DateFrom = new DateTime(input.YearMonthFrom, input.MonthFrom, 1);
                    input.DateTo = new DateTime(input.YearMonthTo, input.MonthTo, DateTime.DaysInMonth(input.YearMonthTo, input.MonthTo));
                }
                if (input.FilterType == "Year") {
                    input.DateFrom = new DateTime(input.Year, 1, 1);
                    input.DateTo = new DateTime(input.Year, 12, DateTime.DaysInMonth(input.Year, 12));
                }
                result = _plantWages.GetWagesReportDetailPerEmployee(employeeID, input.DateFrom, input.DateTo).ToList();
            }

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName)) {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.PlantWagesExcelTemplate.WagesReportAbsentsEmployeeDetail + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName)) {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName)) {
                #region filter
                SLStyle style = slDoc.CreateStyle();
                style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                style.Font.FontName = "Calibri";
                style.Font.FontSize = 10;

                var filter = input.FilterType;

                slDoc.SetCellValue(3, 2, ": " + (employee == null ? "" : employee.EmployeeID));
                slDoc.SetCellValue(3, 3, (employee == null ? "" : employee.EmployeeName));
                slDoc.SetCellValue(4, 2, ": " + locationCompat);
                slDoc.SetCellValue(5, 2, ": " + (employee == null ? "" : employee.UnitCode));
                slDoc.SetCellValue(6, 2, ": " + (employee == null ? "" : employee.ProcessSettingsCode));
                slDoc.SetCellValue(7, 2, ": " + (employee == null ? "" : employee.GroupCode));

                if (filter == "YearMonth") {
                    slDoc.SetCellValue(3, 7, "From Year : " + yearMonthFrom.ToString());
                    slDoc.SetCellValue(4, 7, "Month     : " + monthFrom.ToString());
                    slDoc.SetCellValue(5, 7, "To Year   : " + yearMonthTo.ToString());
                    slDoc.SetCellValue(6, 7, "Month     : " + monthTo.ToString());
                }
                else if (filter == "YearWeek") {
                    slDoc.SetCellValue(3, 7, "From Year : " + yearWeekFrom.ToString());
                    slDoc.SetCellValue(4, 7, "Week      : " + weekFrom.ToString());
                    slDoc.SetCellValue(5, 7, "To Year   : " + yearWeekTo.ToString());
                    slDoc.SetCellValue(6, 7, "Week      : " + weekTo.ToString());
                }
                else {
                    if (filter == "Year") {
                        slDoc.SetCellValue(3, 7, "Year : " + year.ToString());
                    }
                    else {
                        slDoc.SetCellValue(3, 7,
                            "From Date : " + dateFrom.Value.ToString(Constants.DefaultDateOnlyFormat));
                        slDoc.SetCellValue(4, 7, "To Date   : " + dateTo.Value.ToString(Constants.DefaultDateOnlyFormat));
                    }
                }
                #endregion

                var iRow = 10;

                foreach (var item in result) {
                    slDoc.SetCellValue(iRow, 1, item.EmployeeID);
                    slDoc.SetCellValue(iRow, 2, item.EmployeeName);
                    slDoc.SetCellValue(iRow, 3, item.EmployeeNumber);
                    slDoc.SetCellValue(iRow, 4, item.AbsentType);
                    slDoc.SetCellValue(iRow, 5, item.ProductionDate.Value.ToShortDateString());

                    slDoc.SetCellStyle(iRow, 1, iRow, 5, style);
                    iRow++;
                }

                var slSheetProtection = new SLSheetProtection {
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
                slDoc.AutoFitColumn(1, 5);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "WagesReportAbsentsEmployee_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}