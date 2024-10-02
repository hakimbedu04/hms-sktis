using System.Globalization;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeReportsProduction;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;


namespace SKTISWebsite.Controllers
{
    public class TPOFeeReportsProductionController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private ITPOFeeBLL _tpoFeeBll;

        public TPOFeeReportsProductionController(
            IApplicationService applicationService, 
            IMasterDataBLL masterDataBll,
            ITPOFeeBLL tpoFeeBll
        )
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            SetPage("TPOFee/Report/TPORateReport");
        }

        // GET: TPOFeeReportsProduction
        public ActionResult Index()
        {
            var init = new InitTPOFeeReportsProductionViewModel()
            {
                //LocationLookupList = _svc.GetLastLocationChildList(Enums.LocationCode.TPO.ToString()),
                LocationLookupList = _svc.GetTpoRegSktLocationList(),
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now)
            };
            return View("index", init);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GeTpoFeeReportProduction(GetTPOReportsProductionInput criteria)
        {
            var masterLists = new List<TPOFeeReportsProductionDTO>();
            if(criteria.FilterType == "YearWeek"){
                masterLists = _tpoFeeBll.GetTpoFeeReportsProductionViewYearWeek(criteria);
            }
            else if (criteria.FilterType == "All")
            {
                masterLists = _tpoFeeBll.GetTpoFeeReportsProductionViewAll(criteria);
            }
            else{
                masterLists = _tpoFeeBll.GetTpoFeeReportsProductionView(criteria).OrderBy(x=>x.Regional).ThenBy(x => x.LocationCode).ToList();
            }
                
            var viewModel = Mapper.Map<List<TPOFeeReportsProductionViewModel>>(masterLists);
            var pageResult = new PageResult<TPOFeeReportsProductionViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        //public FileStreamResult GenerateExcel(string locationCode, string unitCode, int? shift, string brand, string filterType, int? yearFrom, int? monthFrom, int? yearTo, int? monthTo, int? weekFrom, int? weekTo, DateTime? dateFrom, DateTime? dateTo, string brandGroupCode)
        //public FileStreamResult GenerateExcel(string LocationCode, DateTime? dateFrom, DateTime? dateTo, int? week, int? year, string brandGroupCode,string FilterType)
        public FileStreamResult GenerateExcel(GetTPOReportsProductionInput criteria)
        {
            //var masterLists = _tpoFeeBll.GetTpoFeeReportsProductionView(criteria);
            var masterLists = new List<TPOFeeReportsProductionDTO>();
            if (criteria.FilterType == "YearWeek")
            {
                masterLists = _tpoFeeBll.GetTpoFeeReportsProductionViewYearWeek(criteria);
            }
            else if (criteria.FilterType == "All")
            {
                masterLists = _tpoFeeBll.GetTpoFeeReportsProductionViewAll(criteria);
            }
            else
            {
                masterLists = _tpoFeeBll.GetTpoFeeReportsProductionView(criteria);
            }

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeReportsProduction + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            var allLocations = _svc.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == criteria.LocationCode)
                {
                    locationCompat = item.Text;
                }
            }
            var ci = CultureInfo.CurrentCulture;
            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                var Filter = criteria.FilterType;


                //filter values
                slDoc.SetCellValue(3, 1, "Location : ");
                slDoc.SetCellValue(3, 2, locationCompat);
                if(Filter == "Daily"){
                    slDoc.SetCellValue(4, 1, "Daily        : ");
                    slDoc.SetCellValue(4, 2, "From : " + criteria.DateFrom.ToString("dd/MM/yyyy"));
                    slDoc.SetCellValue(4, 4, "To : " + criteria.DateTo.ToString("dd/MM/yyyy"));
                }else if(Filter == "YearMonth"){
                    slDoc.SetCellValue(4, 1, "Monthly : ");
                    slDoc.SetCellValue(4, 2, "Year   : " + criteria.Year);
                    slDoc.SetCellValue(4, 4, "Month  : " + criteria.Month);
                }else if(Filter == "YearWeek"){
                    slDoc.SetCellValue(4, 1, "Effective Year/Week : ");
                    slDoc.SetCellValue(4, 2, "From : ");
                    slDoc.SetCellValue(4, 4, "Year  : " + criteria.YearFrom);
                    slDoc.SetCellValue(4, 6, "Week  : " + criteria.WeekFrom);
                    slDoc.SetCellValue(5, 2, "To : ");
                    slDoc.SetCellValue(5, 4, "Year  : " + criteria.YearTo);
                    slDoc.SetCellValue(5, 6, "Week  : " + criteria.WeekTo);
                }else if (Filter == "Period")
                {
                    slDoc.SetCellValue(4, 1, "Period        : ");
                    slDoc.SetCellValue(4, 2, "From : " + criteria.DateFrom.ToString("dd/MM/yyyy"));
                    slDoc.SetCellValue(4, 4, "To : " + criteria.DateTo.ToString("dd/MM/yyyy"));
                }
                else{
                    slDoc.SetCellValue(4, 1, "ALL :   ");
                }

                //row values
                var iRow = 10;

                foreach (var data in masterLists)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    slDoc.SetCellValue(iRow, 1, data.Regional);
                    slDoc.SetCellValue(iRow, 2, data.LocationCode);
                    slDoc.SetCellValue(iRow, 3, data.LocationAbbr);
                    slDoc.SetCellValue(iRow, 4, data.LocationName);
                    slDoc.SetCellValue(iRow, 5, data.UMK.HasValue ? (data.UMK != 0 ? data.UMK.Value.ToString("N2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 6, data.Brand);
                    slDoc.SetCellValue(iRow, 7, data.Package.HasValue ? (data.Package != 0 ? data.Package.Value.ToString("F2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 8, data.JKNProductionFee.HasValue ? (data.JKNProductionFee != 0 ? data.JKNProductionFee.Value.ToString("N2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 9, data.JL1ProductionFee.HasValue ? (data.JL1ProductionFee != 0 ? data.JL1ProductionFee.Value.ToString("N2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 10, data.JL2ProductionFee.HasValue ? (data.JL2ProductionFee != 0 ? data.JL2ProductionFee.Value.ToString("N2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 11, data.JL3ProductionFee.HasValue ? (data.JL3ProductionFee != 0 ? data.JL3ProductionFee.Value.ToString("N2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 12, data.JL4ProductionFee.HasValue ? (data.JL4ProductionFee != 0 ? data.JL4ProductionFee.Value.ToString("N2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 13, data.ManagementFee.HasValue ? (data.ManagementFee != 0 ? data.ManagementFee.Value.ToString("N2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 14, data.ProductivityIncentives.HasValue ? (data.ProductivityIncentives != 0 ? data.ProductivityIncentives.Value.ToString("n2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 15, data.Year);
                    slDoc.SetCellValue(iRow, 16, data.StartDate.ToShortDateString());
                    slDoc.SetCellValue(iRow, 17, data.EndDate.ToShortDateString());
                    slDoc.SetCellValue(iRow, 18, data.WeekFrom);
                    slDoc.SetCellValue(iRow, 19, data.WeekTo);
                    slDoc.SetCellValue(iRow, 20, data.NoMemo);
                    slDoc.SetCellValue(iRow, 21, data.JKNProductionVolume.HasValue ? (data.JKNProductionVolume != 0 ? data.JKNProductionVolume.Value.ToString("F2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 22, data.JL1ProductionVolume.HasValue ? (data.JL1ProductionVolume != 0 ? data.JL1ProductionVolume.Value.ToString("F2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 23, data.JL2ProductionVolume.HasValue ? (data.JL2ProductionVolume != 0 ? data.JL2ProductionVolume.Value.ToString("F2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 24, data.JL3ProductionVolume.HasValue ? (data.JL3ProductionVolume != 0 ? data.JL3ProductionVolume.Value.ToString("F2", ci) : "0") : "0");
                    slDoc.SetCellValue(iRow, 25, data.JL4ProductionVolume.HasValue ? (data.JL4ProductionVolume != 0 ? data.JL4ProductionVolume.Value.ToString("F2", ci) : "0") : "0");
                    slDoc.SetCellStyle(iRow, 1, iRow, 25, style);
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

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFee_TPOFeeandProductionReport_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}