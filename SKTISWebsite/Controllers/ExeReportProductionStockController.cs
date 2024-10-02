using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.ExeReportProductionStock;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using AutoMapper;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using Color = System.Drawing.Color;

namespace SKTISWebsite.Controllers
{
    public class ExeReportProductionStockController : BaseController
    {
        private readonly IMasterDataBLL _masterDataBll;
        private readonly IApplicationService _appService;
        private readonly IExeReportProdStockProcessBLL _exeReportProdStockProcessBll;
        private IVTLogger _vtlogger;

        public ExeReportProductionStockController
        (
            IMasterDataBLL masterDataBll,
            IApplicationService appService,
            IExeReportProdStockProcessBLL exeReportProdStockProcessBll,
            IVTLogger vtlogger
        )
        {
            _masterDataBll = masterDataBll;
            _appService = appService;
            _exeReportProdStockProcessBll = exeReportProdStockProcessBll;
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/Report/ProductionandStockReportbyProcess");
        }

        public ActionResult Index()
        {
            // Get current week
            var currentWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            
            // Get default list location for filter dropdown 
            var defaultLocationCodeList = _appService.GetLocationNamesLookupList();
            
            // Get default year list for filter dropdown
            var yearSelectList = _appService.GetGenWeekYears();

            // Get default month list for filter dropdown
            var monthSelectList = _appService.GetMonthSelectList();
            var monthToSelectList = _appService.GetMonthSelectList();

            // Initial model for view
            var initialViewModel = new InitExeReportProductionStockViewModel
            {
                CurrentYear = DateTime.Now.Year,
                CurrentMonth = DateTime.Now.Month,
                CurrentWeek = currentWeek.HasValue ? currentWeek.Value : 1,
                CurrentDay = DateTime.Now.Date.ToShortDateString(),
                ListYear = yearSelectList,
                ListMonth = monthSelectList,
                ListMonthTo = monthToSelectList,
                LocationCodeSelectList = defaultLocationCodeList
            };

            return View(initialViewModel);
        }

        public JsonResult GetUnitCodeList(string locationCode)
        {
            // Get unit code list from BLL
            var model = _exeReportProdStockProcessBll.GetUnitCodeList(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetReportProductionStockByProcess(GetExeProdStockInput input)
        {
            try
            {
                var view = "../ExeReportProductionStock/GridViewExeReportProductionStock";
                var listExeReportProdStock = _exeReportProdStockProcessBll.GetExeReportProductionStock(input);
                var result = Mapper.Map<List<ExeReportProdStockPerBrandGroupCodeViewModel>>(listExeReportProdStock);
                return PartialView(view, result);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Exe Report Production Stock - GetReportProductionStockByProcess");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetExeProdStockInput input)
        {
            try
            {
                // Get source data report
                var listExeReportProdStock = _exeReportProdStockProcessBll.GetExeReportProductionStock(input);
                var result = Mapper.Map<List<ExeReportProdStockPerBrandGroupCodeViewModel>>(listExeReportProdStock);

                var ms = new MemoryStream();
                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";

                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExeReportProductionStockByProcess + ".xlsx";
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
                    style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    style.Alignment.Vertical = VerticalAlignmentValues.Center;

                    SLStyle styleTotal = slDoc.CreateStyle();
                    styleTotal.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    styleTotal.Font.FontName = "Calibri";
                    styleTotal.Font.FontSize = 11;
                    styleTotal.Font.Bold = true;
                    styleTotal.Fill.SetPattern(PatternValues.Solid, Color.Gray, Color.Gray);

                    SLStyle styleCenterAlignment = slDoc.CreateStyle();
                    styleCenterAlignment.Alignment.Horizontal = HorizontalAlignmentValues.Center;

                    //row values
                    var iRow = 8;
                    var iRowBrand = 8;
                    var iRowLocation = 8;

                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + input.FilterLocation + " - " + input.FilterLocationName);
                    slDoc.SetCellValue(3, 3, "");
                    slDoc.SetCellValue(4, 2, ": " + input.FilterUnitCode);

                    if (input.IsFilterAnnualy)
                    {
                        slDoc.SetCellValue(3, 9, "Year");
                        slDoc.SetCellValue(3, 10, ": " + input.FilterYear);
                    }
                    else if (input.IsFilterMonthly)
                    {
                        slDoc.SetCellValue(3, 6, "Year");
                        slDoc.SetCellValue(4, 6, "Month");
                        slDoc.SetCellValue(3, 7,
                            input.FilterYearMonthFrom.HasValue ? input.FilterYearMonthFrom.Value.ToString() : "");
                        slDoc.SetCellValue(4, 7,
                            input.FilterMonthFrom.HasValue ? input.FilterMonthFrom.Value.ToString() : "");
                        slDoc.SetCellValue(3, 8, "To");
                        slDoc.SetCellValue(3, 9, "Year");
                        slDoc.SetCellValue(4, 9, "Month");
                        slDoc.SetCellValue(3, 10,
                            input.FilterYearMonthTo.HasValue ? input.FilterYearMonthTo.Value.ToString() : "");
                        slDoc.SetCellValue(4, 10,
                            input.FilterMonthTo.HasValue ? input.FilterMonthTo.Value.ToString() : "");
                        slDoc.SetCellStyle(3, 8, 3, 8, styleCenterAlignment);
                    }
                    else if (input.IsFilterWeekly)
                    {
                        slDoc.SetCellValue(3, 6, "Year");
                        slDoc.SetCellValue(4, 6, "Week");
                        slDoc.SetCellValue(3, 7,
                            input.FilterYearWeekFrom.HasValue ? input.FilterYearWeekFrom.Value.ToString() : "");
                        slDoc.SetCellValue(4, 7,
                            input.FilterWeekFrom.HasValue ? input.FilterWeekFrom.Value.ToString() : "");
                        slDoc.SetCellValue(3, 8, "To");
                        slDoc.SetCellValue(3, 9, "Year");
                        slDoc.SetCellValue(4, 9, "Week");
                        slDoc.SetCellValue(3, 10,
                            input.FilterYearWeekTo.HasValue ? input.FilterYearWeekTo.Value.ToString() : "");
                        slDoc.SetCellValue(4, 10, input.FilterWeekTo.HasValue ? input.FilterWeekTo.Value.ToString() : "");
                        slDoc.SetCellStyle(3, 8, 3, 8, styleCenterAlignment);
                    }
                    else if (input.IsFilterDaily)
                    {
                        slDoc.SetCellValue(3, 6, "Date");
                        slDoc.SetCellValue(3, 7,
                            input.FilterDateFrom.HasValue ? input.FilterDateFrom.Value.ToString() : "");
                        slDoc.SetCellValue(3, 8, "To");
                        slDoc.SetCellStyle(3, 8, 3, 8, styleCenterAlignment);
                        slDoc.SetCellValue(3, 9, "Date");
                        slDoc.SetCellValue(3, 10, input.FilterDateTo.HasValue ? input.FilterDateTo.Value.ToString() : "");
                    }

                    foreach (var itemBrandGroup in result)
                    {
                        slDoc.SetCellValue(iRow, 1, itemBrandGroup.BrandGroupCode);
                        var mergeCountBrandGroup = iRow + itemBrandGroup.CountBrandGroupCode +
                                                   (itemBrandGroup.ListReportProdStockPerBrand.Count() - 1);
                        slDoc.MergeWorksheetCells(iRow, 1, mergeCountBrandGroup, 1);
                        slDoc.SetCellStyle(iRow, 1, mergeCountBrandGroup, 13, style);
                        iRowBrand = iRow;
                        foreach (var itemBrand in itemBrandGroup.ListReportProdStockPerBrand)
                        {
                            slDoc.SetCellValue(iRowBrand, 2, itemBrand.BrandGroup);
                            var mergeCountBrand = iRowBrand + itemBrand.CountBrandGroup - 1;
                            slDoc.MergeWorksheetCells(iRowBrand, 2, mergeCountBrand, 2);
                            iRowLocation = iRowBrand;
                            foreach (var item in itemBrand.ListReportProdStock)
                            {
                                slDoc.SetCellValue(iRowLocation, 3, item.LocationCode);
                                slDoc.SetCellValue(iRowLocation, 4, item.BeginStockInternalMove);
                                slDoc.SetCellValue(iRowLocation, 5, item.BeginStockExternalMove);
                                slDoc.SetCellValue(iRowLocation, 6, item.Production);
                                slDoc.SetCellValue(iRowLocation, 7, item.PAP);
                                slDoc.SetCellValue(iRowLocation, 8, item.PAG);
                                slDoc.SetCellValue(iRowLocation, 9, item.EndingStockInternalMove);
                                slDoc.SetCellValue(iRowLocation, 10, item.EndingStockExternalMove);
                                slDoc.SetCellValue(iRowLocation, 11, item.Planning);
                                slDoc.SetCellValue(iRowLocation, 12, item.VarianceStick);
                                slDoc.SetCellValue(iRowLocation, 13, item.VariancePercent);
                                iRowLocation++;
                            }
                            iRowBrand = mergeCountBrand;
                            slDoc.SetCellValue(iRowBrand + 1, 2, "Total");
                            slDoc.SetCellValue(iRowBrand + 1, 4, itemBrand.TotalBeginStockInMovePerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 5, itemBrand.TotalBeginStockExtMovePerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 6, itemBrand.TotalProdPerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 7, itemBrand.TotalPAPPerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 8, itemBrand.TotalPAGPerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 9, itemBrand.TotalEndStockInMovePerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 10, itemBrand.TotalEndStockExtMovePerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 11, itemBrand.TotalPlanningPerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 12, itemBrand.TotalVarStickPerBrand);
                            slDoc.SetCellValue(iRowBrand + 1, 13, itemBrand.TotalVarStickPercentPerBrand);
                            slDoc.MergeWorksheetCells(iRowBrand + 1, 2, iRowBrand + 1, 3);
                            slDoc.SetCellStyle(iRowBrand + 1, 1, iRowBrand + 1, 13, styleTotal);
                            iRowBrand += 2;
                        }

                        iRow = mergeCountBrandGroup;
                        slDoc.SetCellValue(iRow + 1, 1, "Total");
                        slDoc.SetCellValue(iRow + 1, 4, itemBrandGroup.TotalBeginStockInMovePerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 5, itemBrandGroup.TotalBeginStockExtMovePerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 6, itemBrandGroup.TotalProdPerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 7, itemBrandGroup.TotalPAPPerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 8, itemBrandGroup.TotalPAGPerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 9, itemBrandGroup.TotalEndStockInMovePerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 10, itemBrandGroup.TotalEndStockExtMovePerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 11, itemBrandGroup.TotalPlanningPerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 12, itemBrandGroup.TotalVarStickPerBrandGroupCode);
                        slDoc.SetCellValue(iRow + 1, 13, itemBrandGroup.TotalVarStickPercentPerBrandGroupCode);
                        slDoc.MergeWorksheetCells(iRow + 1, 1, iRow + 1, 3);
                        slDoc.SetCellStyle(iRow + 1, 1, iRow + 1, 13, styleTotal);
                        iRow += 2;
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
                var fileName = "ProductionExecution_Reports_ProductionStock_" + DateTime.Now.ToShortDateString() +
                               ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Exe Report Production Stock - Generate Excel");
                return null;
            }
        }
    }
}