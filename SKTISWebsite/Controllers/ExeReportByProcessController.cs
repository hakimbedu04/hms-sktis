using System.Globalization;
using System.Web.Configuration;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExeReportByProcess;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Color = System.Drawing.Color;

namespace SKTISWebsite.Controllers
{
    public class ExeReportByProcessController : BaseController
    {

        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IPlanningBLL _planningBll;
        private IExecutionPlantBLL _executionPlantBll;
        private IExeReportBLL _exeReportBLL;
        private IVTLogger _vtlogger;

        public ExeReportByProcessController(IApplicationService applicationService, IMasterDataBLL masterDataBll,
            IPlanningBLL planningBll, IExecutionPlantBLL executionPlantBll, IExeReportBLL exeReportBLL, IVTLogger vtlogger)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _planningBll = planningBll;
            _executionPlantBll = executionPlantBll;
            _exeReportBLL = exeReportBLL;
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/Report/ProductionReportbyProcess");
        }

        public ActionResult Index(string param1, string param2, int? param3, string param4, string param5, string param6, int? param7)
        {
            if (param7.HasValue) setResponsibility(param7.Value);
            var init = new InitExeReportByProcessViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                MonthSelectList = _svc.GetMonthSelectList(),
                MonthToSelectList = _svc.GetMonthSelectList(),
                PLNTChildLocationLookupList = _svc.GetLocationNamesLookupList(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                DefaultMonth = DateTime.Now.Month,
                Param1LocationCode = param1,
                Param2UnitCode = param2,
                Param3Shift = param3,
                Param4BrandCode = param4,
                Param5BrandGroupCode = param5,
                Param6Date = String.IsNullOrEmpty(param6) ? DateTime.Now.Date : DateTime.ParseExact(param6, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };
            return View("index", init);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            if (locationCode != Enums.LocationCode.SKT.ToString() && locationCode != Enums.LocationCode.PLNT.ToString() &&
                locationCode != Enums.LocationCode.TPO.ToString() && locationCode != Enums.LocationCode.REG1.ToString() &&
                locationCode != Enums.LocationCode.REG2.ToString() && locationCode != Enums.LocationCode.REG3.ToString() &&
                locationCode != Enums.LocationCode.REG4.ToString()) {
                var model = _svc.GetUnitCodeByProcess(locationCode);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else {
                IDictionary<string, string> units = new Dictionary<string, string>();
                units.Add("All", "All");
                var model = new SelectList(units, "Key", "Value");
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetShiftByLocation(string locationCode, string unitCode)
        {
            var shifts = _svc.GetShiftFilterByProcess(locationCode, unitCode);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getActiveBrandGroupByLocationUnit(string locationCode, string unitCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string DateFrom, string DateTo, string FilterType)
        {
            DateTime dtFrom, dtTo;
            var FromDate = DateTime.TryParse(DateFrom, out dtFrom) ? DateTime.Parse(DateFrom).ToShortDateString() : DateTime.Today.ToShortDateString();
            var ToDate = DateTime.TryParse(DateTo, out dtTo) ? DateTime.Parse(DateTo).ToShortDateString() : DateTime.Today.ToShortDateString();
            var model = _exeReportBLL.GetActiveBrandGroupCodeReportByProcess(locationCode, unitCode, YearFrom, YearTo, MonthFrom, MonthTo, WeekFrom, WeekTo, FromDate, ToDate,
                    FilterType);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetActiveBrandFromByLocation(string locationCode, string unitCode, string brandGroupCode, string YearFrom, string YearTo, string MonthFrom, string MonthTo, string WeekFrom, string WeekTo, string DateFrom, string DateTo, string FilterType)
        {
            DateTime dtFrom, dtTo;
            var FromDate = DateTime.TryParse(DateFrom, out dtFrom) ? DateTime.Parse(DateFrom).ToShortDateString() : DateTime.Today.ToShortDateString();
            var ToDate = DateTime.TryParse(DateTo, out dtTo) ? DateTime.Parse(DateTo).ToShortDateString() : DateTime.Today.ToShortDateString();
            var model = _exeReportBLL.GetActiveBrandCodeReportByProcess(locationCode, unitCode, brandGroupCode, YearFrom, YearTo, MonthFrom, MonthTo, WeekFrom, WeekTo, FromDate, ToDate,
                    FilterType);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandDescriptionByBrandCode(string brandCode)
        {
            var model = _masterDataBLL.GetMstGenByBrandCode (brandCode);
            if (model != null)
                return Json(model.Description, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetExeReportByProcess(GetExeReportByProcessInput criteria)
        {
            try
            {
                var filter = criteria.FilterType;

                //var masterLists = _exeReportBLL.GetReportByProcess(criteria);
                //var viewModel = Mapper.Map<List<ExeReportByProcessViewModel>>(masterLists);
                //var pageResult = new PageResult<ExeReportByProcessViewModel>(viewModel, criteria);

                var pageResult = new PageResult<ExeReportByProcessViewModel>();
                var masterLists = _exeReportBLL.GetReportByProcess(criteria);
                pageResult.TotalRecords = masterLists.Count;
                pageResult.TotalPages = (masterLists.Count/criteria.PageSize) +
                                        (masterLists.Count%criteria.PageSize != 0 ? 1 : 0);
                var result = masterLists.Skip((criteria.PageIndex - 1)*criteria.PageSize).Take(pageResult.TotalRecords);
                pageResult.Results = Mapper.Map<List<ExeReportByProcessViewModel>>(result);
                var ci = CultureInfo.CurrentUICulture;

                foreach (var item in   pageResult.Results)
                {
                    item.BeginningStock = Math.Round(item.BeginningStock, 3, MidpointRounding.AwayFromZero);
                    item.Production = Math.Round(item.Production, 3, MidpointRounding.AwayFromZero);
                    item.KeluarBersih = Math.Round(item.KeluarBersih, 3, MidpointRounding.AwayFromZero);
                    item.RejectSample = Math.Round(item.RejectSample, 3, MidpointRounding.AwayFromZero);
                    item.EndingStock = Math.Round(item.EndingStock, 3, MidpointRounding.AwayFromZero);
                }


                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Exe Report By Process - GetExeReportByProcess");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string unitCode, int? shift, string brand,
            string filterType, int? yearFrom, int? monthFrom, int? yearTo, int? monthTo, int? weekFrom, int? weekTo,
            DateTime? dateFrom, DateTime? dateTo, string brandGroupCode)
        {
            var input = new GetExeReportByProcessInput
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
            var ci = CultureInfo.CurrentCulture;

            String BrandDescription = "";

            try{
                BrandDescription = _masterDataBLL.GetMstGenByBrandCode(brand).Description;
            }
            catch{}
                
            try
            {
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


                var exeReportByProcess = _exeReportBLL.GetReportByProcess(input);
                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExeReportByProcess + ".xlsx";
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

                    var filter = input.FilterType;

                    slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    slDoc.SetCellValue(4, 2, ": " + unitCode);
                    slDoc.SetCellValue(5, 2, ": " + shift);
                    slDoc.SetCellValue(6, 2, ": " + brandGroupCode);
                    slDoc.SetCellValue(7, 2, ": " + brand);
                    slDoc.SetCellValue(7, 3, BrandDescription);

                    if (filter == "YearMonth")
                    {
                        slDoc.SetCellValue(3, 4, "From Year : " + yearFrom.ToString());
                        slDoc.SetCellValue(3, 5, "Month     : " + monthFrom.ToString());
                        slDoc.SetCellValue(3, 7, "To Year   : " + yearTo.ToString());
                        slDoc.SetCellValue(3, 8, "Month     : " + monthTo.ToString());
                    }
                    else if (filter == "YearWeek")
                    {
                        slDoc.SetCellValue(3, 4, "From Year : " + yearFrom.ToString());
                        slDoc.SetCellValue(3, 5, "Week      : " + weekFrom.ToString());
                        slDoc.SetCellValue(3, 7, "To Year   : " + yearTo.ToString());
                        slDoc.SetCellValue(3, 8, "Week      : " + weekTo.ToString());
                    }
                    else
                    {
                        if (filter == "Year")
                        {
                            slDoc.SetCellValue(3, 4, "Year : " + yearFrom.ToString());
                        }
                        else
                        {
                            slDoc.SetCellValue(3, 4,
                                "From Date : " + dateFrom.Value.ToString(Constants.DefaultDateOnlyFormat));
                            slDoc.SetCellValue(3, 7,
                                "To Date   : " + dateTo.Value.ToString(Constants.DefaultDateOnlyFormat));
                        }
                    }

                    //row values
                    var iRow = 11;

                    foreach (var item in exeReportByProcess)
                    {
                        //slDoc.SetCellValue(iRow, 1, item.Description);
                        //slDoc.SetCellValue(iRow, 2, item.UOM);
                        //slDoc.SetCellValue(iRow, 3, item.BeginningStock.ToString("F2", ci));
                        //slDoc.SetCellValue(iRow, 4, item.Production.ToString("F2", ci));
                        //slDoc.SetCellValue(iRow, 5, item.KeluarBersih.ToString("F2", ci));
                        //slDoc.SetCellValue(iRow, 6, item.RejectSample.ToString("F2", ci));
                        //slDoc.SetCellValue(iRow, 7, item.EndingStock.ToString("F2", ci));


                        slDoc.SetCellValue(iRow, 1, item.Description);
                        slDoc.SetCellValue(iRow, 2, item.UOM);
                        slDoc.SetCellValue(iRow, 3,
                            String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.BeginningStock));
                        slDoc.SetCellValue(iRow, 4, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Production));
                        slDoc.SetCellValue(iRow, 5,
                            String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.KeluarBersih));
                        slDoc.SetCellValue(iRow, 6,
                            String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.RejectSample));
                        slDoc.SetCellValue(iRow, 7,
                            String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.EndingStock));

                        slDoc.SetCellStyle(iRow, 1, iRow, 7, style);
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
                    //slDoc.AutoFitColumn(1, 9);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }

                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "ProductionExecution_Reports_ProductionByProcess_" + DateTime.Now.ToShortDateString() +
                               ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, shift, brand,
                                            filterType, yearFrom, monthFrom, yearTo, monthTo,
                                            weekFrom, weekTo,dateFrom, dateTo, brandGroupCode },
                                            "Exe Report By Process - Generate Excel");
                return null;
            }
        }
    }
}