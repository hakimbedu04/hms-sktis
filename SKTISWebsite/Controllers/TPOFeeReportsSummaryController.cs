using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeReportsSummary;
using SpreadsheetLight;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeReportsSummaryController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;

        public TPOFeeReportsSummaryController(IApplicationService applicationService, IMasterDataBLL masterDataBll, ITPOFeeBLL tpoFeeBll)
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            SetPage("TPOFee/Report/SummaryTPOFee");
        }

        //
        // GET: /TPOFeeReportsSummary/
        public ActionResult Index()
        {
            int yearOld = DateTime.Now.Year;
            if (TempData["year1"] != null)
            {
                yearOld = (int)TempData["year1"];

            }

            var init = new InitTPOFeeReportsSummaryViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultYear = yearOld
            };

            return View("Index", init);
        }

        private Enums.TpoFeeReportsSummaryProductionFeeType GetProductionTypeEnums(string productionType)
        {
            switch (productionType)
            {
                case "Biaya Produksi":
                    return Enums.TpoFeeReportsSummaryProductionFeeType.BiayaProduksi;
                case "Jasa Manajemen":
                    return Enums.TpoFeeReportsSummaryProductionFeeType.JasaManajemen;
                case "Productivity Incentives":
                    return Enums.TpoFeeReportsSummaryProductionFeeType.ProductivityIncentives;
                case "Total Bayar":
                    return  Enums.TpoFeeReportsSummaryProductionFeeType.TotalBayar;
            }

            return Enums.TpoFeeReportsSummaryProductionFeeType.BiayaProduksi;
        }
        [HttpPost]
        public JsonResult GetTPOReportsSummary(GetTPOReportsSummaryInput criteria)
        {
            criteria.ProductionFeeType = GetProductionTypeEnums(criteria.ProductionType);
            
            var result = _tpoFeeBll.GetTpoFeeReportsSummary(criteria);
            var viewModel = Mapper.Map<List<TPOFeeReportsSummaryViewModel>>(result);
            var pageResult = new PageResult<TPOFeeReportsSummaryViewModel>(viewModel, criteria);

            return Json(pageResult);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetTPOReportsSummaryInput criteria)
        {
            criteria.ProductionFeeType = GetProductionTypeEnums(criteria.ProductionType);

            var tpoFeeReportsSummary = _tpoFeeBll.GetTpoFeeReportsSummary(criteria);
            var viewModel = Mapper.Map<List<TPOFeeReportsSummaryViewModel>>(tpoFeeReportsSummary);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeReportsSummary + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 2, criteria.Year);
                

                //row values
                var iRow = 7;
                bool blFirst = true;
                //list week
                int iColumn = 4;//start from

                foreach (var tpoFeeReportsSummaryViewModel in viewModel)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;
                   
                    if (iRow % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }
                    if (tpoFeeReportsSummaryViewModel.IsParentRow || tpoFeeReportsSummaryViewModel.IsSummary)
                    {
                        slDoc.SetCellValue(iRow, 1, tpoFeeReportsSummaryViewModel.ParentLocation);
                        slDoc.MergeWorksheetCells(iRow, 1, iRow, 3);
                    }
                    else
                    {
                        slDoc.UnmergeWorksheetCells(iRow, 1, iRow, 3);
                        slDoc.SetCellValue(iRow, 1, tpoFeeReportsSummaryViewModel.LocationCode);
                        slDoc.SetCellValue(iRow, 2, tpoFeeReportsSummaryViewModel.LocationAbbr);
                        slDoc.SetCellValue(iRow, 3, tpoFeeReportsSummaryViewModel.LocationName);
                        
                    }

                    //list week
                    iColumn = 4;//start from
                    foreach (var tpoFeeReportsSummaryWeeklyViewModel in tpoFeeReportsSummaryViewModel.ListWeekValue)
                    {
                        if (tpoFeeReportsSummaryViewModel.IsParentRow)
                        {
                            slDoc.SetCellValue(iRow, iColumn, tpoFeeReportsSummaryWeeklyViewModel.Week);
                        }
                        else if (tpoFeeReportsSummaryWeeklyViewModel.Location == null)
                        {
                            slDoc.SetCellValue(iRow, iColumn, tpoFeeReportsSummaryWeeklyViewModel.WeekValueString);
                        }
                        else
                        {
                            slDoc.SetCellValue(iRow, iColumn, tpoFeeReportsSummaryWeeklyViewModel.Location);
                        }
                        iColumn++;
                    }

                    //once only
                    if (blFirst)
                    {
                        slDoc.MergeWorksheetCells(6, 4, 6, iColumn-1);
                        blFirst = false;

                        //create header total
                        slDoc.SetCellValue(6, iColumn , "Total");
                        //slDoc.CopyCellStyle(6,1,6, iColumn);
                        SLStyle styleHeader = slDoc.CreateStyle();
                        styleHeader = slDoc.GetCellStyle(6, 1);
                        slDoc.SetCellStyle(6, iColumn, styleHeader);
                    }



                    slDoc.SetCellValue(iRow, iColumn, tpoFeeReportsSummaryViewModel.TotalCalculateString);
                    slDoc.SetCellStyle(iRow, 1, iRow, iColumn, style);
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
                slDoc.AutoFitColumn(1, iColumn);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFeeSummary_" + criteria.ProductionType.Replace(" ","") + "_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
	}
}