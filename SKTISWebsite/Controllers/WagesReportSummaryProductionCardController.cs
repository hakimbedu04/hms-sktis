using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.WagesReportSummaryProductionCard;
using SKTISWebsite.Models.Common;
using SpreadsheetLight;

namespace SKTISWebsite.Controllers
{
    public class WagesReportSummaryProductionCardController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
       
        private IPlantWagesExecutionBLL _plantWages;

        public WagesReportSummaryProductionCardController(IApplicationService applicationService, IMasterDataBLL masterDataBll, IPlantWagesExecutionBLL plantWages)
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _plantWages = plantWages;
            SetPage("PlantWages/Execution/Report/SummaryProductionCard");
        }

        //
        // GET: /WagesReportSummaryProductionCard/
        public ActionResult Index()
        {
            var init = new InitWagesReportSummary()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
            };

            return View("index", init);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult GetBrandGroupCodeList(GetWagesReportSummaryInput criteria)
        {
            var listSktBrandCode = _plantWages.GetProductionCardBrandGroupCode(criteria);
            return Json(listSktBrandCode, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetWagesReportSummary(GetWagesReportSummaryInput criteria)
        {
            var masterLists = _plantWages.GetWagesReportSummary(criteria);
            var viewModel = Mapper.Map<List<WagesReportSummaryViewModel>>(masterLists);
            var pageResult = new PageResult<WagesReportSummaryViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetWagesReportSummaryInput criteria)
        {

            var masterLists = _plantWages.GetWagesReportSummary(criteria);
            var viewModel = Mapper.Map<List<WagesReportSummaryViewModel>>(masterLists);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.PlantWagesExcelTemplate.WagesReportSummary + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                if (criteria.FilterType == "Daily")
                {
                    slDoc.SetCellValue(3, 1, "Daily Basic");
                    slDoc.SetCellValue(4, 1, "Start Date");
                    slDoc.SetCellValue(4, 2, ": " + GenericHelper.ConvertDateTimeToString(criteria.DateFrom));

                    slDoc.SetCellValue(4, 3, "End Date");
                    slDoc.SetCellValue(4, 4, ": " + GenericHelper.ConvertDateTimeToString(criteria.DateTo));    
                }
                else
                {
                    slDoc.SetCellValue(3, 1, "Payroll Weekly Basic");
                    slDoc.SetCellValue(4, 1, "Week");
                    slDoc.SetCellValue(4, 2, ": " + criteria.Week);
                    slDoc.SetCellValue(4, 3, "Year");
                    slDoc.SetCellValue(4, 4, ": " + criteria.Year);    
                }
                slDoc.SetCellValue(5, 1, "Brand");
                slDoc.SetCellValue(5, 2, ": " + criteria.BrandGroupCode);

                #region style

                SLStyle style = slDoc.CreateStyle();
                style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                style.Font.FontName = "Calibri";
                style.Font.FontSize = 10;

                SLStyle styleNumeric = slDoc.CreateStyle();
                styleNumeric.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                styleNumeric.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                styleNumeric.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                styleNumeric.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                styleNumeric.Font.FontName = "Calibri";
                styleNumeric.Font.FontSize = 10;
                styleNumeric.SetHorizontalAlignment(HorizontalAlignmentValues.Right);

                SLStyle styleLocation = slDoc.CreateStyle();
                styleLocation.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                styleLocation.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                styleLocation.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                styleLocation.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                styleLocation.Font.FontName = "Calibri";
                styleLocation.Font.FontSize = 10;
                styleLocation.SetVerticalAlignment(VerticalAlignmentValues.Center);

                SLStyle styleTotal = slDoc.CreateStyle();
                styleTotal.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                styleTotal.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                styleTotal.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                styleTotal.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                styleTotal.Font.FontName = "Calibri";
                styleTotal.Font.FontSize = 10;
                styleTotal.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

                SLStyle styleTotalNumeric = slDoc.CreateStyle();
                styleTotalNumeric.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                styleTotalNumeric.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                styleTotalNumeric.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                styleTotalNumeric.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                styleTotalNumeric.Font.FontName = "Calibri";
                styleTotalNumeric.Font.FontSize = 10;
                styleTotalNumeric.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                styleTotalNumeric.SetHorizontalAlignment(HorizontalAlignmentValues.Right);
                #endregion

                //row values
                var iRow = 9;
                var iRowCorrection = 9;

                foreach (var wagesReportSummaryViewModel in viewModel)
                {
                    if (wagesReportSummaryViewModel.IsProductionCard)
                    {
                        slDoc.SetCellValue(iRow, 1, wagesReportSummaryViewModel.Location);
                        slDoc.MergeWorksheetCells(iRow, 1, iRow + wagesReportSummaryViewModel.ProductionCards.Count-1, 1);
                        slDoc.SetCellStyle(iRow, 1, iRow,2, styleLocation);

                        foreach (var wagesReportSummaryProductionCardViewModel in wagesReportSummaryViewModel.ProductionCards)
                        {
                            
                            slDoc.SetCellValue(iRow, 2, wagesReportSummaryProductionCardViewModel.Process);
                            slDoc.SetCellValue(iRow, 3, wagesReportSummaryProductionCardViewModel.Produksi);
                            slDoc.SetCellValue(iRow, 4, wagesReportSummaryProductionCardViewModel.UpahLain);
                            slDoc.SetCellStyle(iRow, 2, iRow, 2, style);
                            slDoc.SetCellStyle(iRow, 3, iRow, 4, styleNumeric);
                            iRow++;
                        }
                     
                        slDoc.SetCellValue(iRow, 1, "Total");
                        slDoc.MergeWorksheetCells(iRow, 1, iRow , 2);
                        slDoc.SetCellStyle(iRow, 1, iRow, 2, styleTotal);

                        slDoc.SetCellValue(iRow, 3, wagesReportSummaryViewModel.TotalProduksiString);
                        slDoc.SetCellValue(iRow, 4, wagesReportSummaryViewModel.TotalUpahLainString);
                        slDoc.SetCellStyle(iRow, 3, iRow, 4, styleTotalNumeric);
                        iRow++;
                    }
                    else
                    {
                        //correction
                        slDoc.SetCellValue(iRowCorrection, 6, wagesReportSummaryViewModel.Location);
                        slDoc.MergeWorksheetCells(iRowCorrection, 6, iRowCorrection + wagesReportSummaryViewModel.ProductionCards.Count-1, 6);
                        slDoc.SetCellStyle(iRowCorrection, 6, iRowCorrection + wagesReportSummaryViewModel.ProductionCards.Count - 1, 6, styleLocation);

                        foreach (var wagesReportSummaryProductionCardViewModel in wagesReportSummaryViewModel.ProductionCards)
                        {
                            slDoc.SetCellValue(iRowCorrection, 7, wagesReportSummaryProductionCardViewModel.Process);
                            slDoc.SetCellValue(iRowCorrection, 8, wagesReportSummaryProductionCardViewModel.Produksi);
                            slDoc.SetCellValue(iRowCorrection, 9, wagesReportSummaryProductionCardViewModel.UpahLain);
                            slDoc.SetCellStyle(iRowCorrection, 7, iRowCorrection, 7, style);
                            slDoc.SetCellStyle(iRowCorrection, 8, iRowCorrection, 9, styleNumeric);
                            iRowCorrection++;
                        }

                        slDoc.SetCellValue(iRowCorrection, 6, "Total");
                        slDoc.MergeWorksheetCells(iRowCorrection, 6, iRowCorrection, 7);
                        slDoc.SetCellStyle(iRowCorrection, 6, iRowCorrection, 7, styleTotal);

                        slDoc.SetCellValue(iRowCorrection, 8, wagesReportSummaryViewModel.TotalProduksiCorrectionString);
                        slDoc.SetCellValue(iRowCorrection, 9, wagesReportSummaryViewModel.TotalUpahLainCorrectionString);
                        slDoc.SetCellStyle(iRowCorrection, 8, iRowCorrection, 9, styleTotalNumeric);
                        iRowCorrection++;
                    }
                }

                //grand total
                var grandTotalProduksi = viewModel.Where(c => c.IsProductionCard).Sum(x => x.TotalProduksi);
                var grandTotalUpahLain = viewModel.Where(c => c.IsProductionCard).Sum(x => x.TotalUpahLain);
                
                slDoc.SetCellValue(iRow, 1, "Grand Total");
                slDoc.MergeWorksheetCells(iRow, 1, iRow, 2);
                slDoc.SetCellStyle(iRow, 1, iRow, 2, styleTotal);
                slDoc.SetCellValue(iRow, 3, GenericHelper.ConvertDecimalToString2FormatDecimalCommaThousand(grandTotalProduksi));
                slDoc.SetCellValue(iRow, 4, GenericHelper.ConvertDecimalToString2FormatDecimalCommaThousand(grandTotalUpahLain));
                slDoc.SetCellStyle(iRow, 3, iRow, 4, styleTotalNumeric);

                grandTotalProduksi = viewModel.Where(c => c.IsProductionCard == false).Sum(x => x.TotalProduksiCorrection);
                grandTotalUpahLain = viewModel.Where(c => c.IsProductionCard == false).Sum(x => x.TotalUpahLainCorrection);

                slDoc.SetCellValue(iRowCorrection, 6, "Grand Total");
                slDoc.MergeWorksheetCells(iRowCorrection, 6, iRowCorrection, 7);
                slDoc.SetCellStyle(iRowCorrection, 6, iRowCorrection, 7, styleTotal);
                slDoc.SetCellValue(iRowCorrection, 8, GenericHelper.ConvertDecimalToString2FormatDecimalCommaThousand(grandTotalProduksi));
                slDoc.SetCellValue(iRowCorrection, 9, GenericHelper.ConvertDecimalToString2FormatDecimalCommaThousand(grandTotalUpahLain));
                slDoc.SetCellStyle(iRowCorrection, 8, iRowCorrection, 9, styleTotalNumeric);

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
            var fileName = "PlantWages_ Reports_ProdCardSummary_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

	}
}