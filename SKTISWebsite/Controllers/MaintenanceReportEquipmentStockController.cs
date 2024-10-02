using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using SKTISWebsite.Models.MaintenanceReportEquipmentStock;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Enums = HMS.SKTIS.Core.Enums;
using AutoMapper;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenWeek;
using System.IO;
using HMS.SKTIS.Core;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceReportEquipmentStockController : BaseController
    {
        private IMaintenanceBLL _maintenanceBll;
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private IVTLogger _vtlogger;

        public MaintenanceReportEquipmentStockController(IMaintenanceBLL maintenanceBll, IVTLogger vtlogger, IApplicationService svc, IMasterDataBLL masterDataBll)
        {
            this._maintenanceBll = maintenanceBll;
            this._svc = svc;
            this._masterDataBll = masterDataBll;
            this._vtlogger = vtlogger;
            SetPage("Maintenance/Report/EquipmentStock");
        }

        public ActionResult Index()
        {
            var plntChildLocationLookupList = _svc.GetLastLocationChildList(Enums.LocationCode.SKT.ToString());
            var locationLookupList = _svc.GetPlantAndRegionalLocationLookupList();
            var week = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            var model = new InitMaintenanceReportEquipmentStock()
            {                
                PLNTChildLocationLookupList = plntChildLocationLookupList,
                LocationLookupList = locationLookupList,
                DefaultUnitCode = _svc.GetUnitCodeSelectListByLocationCode( plntChildLocationLookupList.FirstOrDefault().LocationCode).FirstOrDefault().Value,
                DefaultDate =DateTime.Now,
                DefaultWeek = _masterDataBll.GetWeekByDate(DateTime.Now).Week,
                DefaultYear = _masterDataBll.GetWeekByDate(DateTime.Now).Year
                
            };
            return View(model);
        }

        /// <summary>
        /// Gets the unit code by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUnitCodeByLocationCode(string locationCode)
        {
            var unitCodes = _svc.GetUnitCodeSelectListByLocationCode(locationCode);
            return Json(unitCodes, JsonRequestBehavior.AllowGet);
        }       

        /// <summary>
        /// Get KPS Weeks and KPS Year By Date
        /// </summary>
        /// <param name="year">KPS Year</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWeekAndYearByDate(DateTime date)
        {
            var dbResult = _masterDataBll.GetWeekByDate(date);
            var kpsWeekYear = Mapper.Map<MasterGenWeekViewModel>(dbResult);
            return Json(kpsWeekYear, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetMaintenanceEquipmentReport(GetMaintenanceEquipmentStockReportInput criteria)
        {
            try
            {
                //var maintenanteEquipmentReport = _maintenanceBll.GetMaintenanceEquipmentStockReport(criteria);

                ////var masterLists = _planningBll.GetReportSummaryProcessTargets(criteria);
                //var viewModel = Mapper.Map<List<MaintenanceEquipmentStockReportViewModel>>(maintenanteEquipmentReport);

                UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
                var UserAD = strUserID.Username;
                var QParam = criteria.LocationCode + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + UserAD;
                DateTime dt = Convert.ToDateTime(criteria.InventoryDate);
                //var masterLists = _maintenanceBll.GetMaintenanceExecutionInventoryView(criteria);
                //var masterLists = _maintenanceBll.GetInventory(criteria.Date.Value.ToString("yyyy-MM-dd"), criteria.LocationCode, criteria.ItemType);
                var masterLists = _maintenanceBll.GetReportMaintenanceEquipmentStock(dt, criteria.LocationCode, criteria.UnitCode, QParam, UserAD);
                var viewModel = Mapper.Map<List<MntcEquipmentStockFunction_Result>>(masterLists);


                //sorting
                var SortedList = viewModel.OrderBy(o => o.ItemDescription).ToList();

                //suming
                //var summedList = SortedList.Sum(i=>i.ItemCode);
                var SummedList = from val in SortedList
                    group val by new {val.ItemCode, val.ItemDescription}
                    into grouped
                    select new MaintenanceEquipmentStockReportViewModel
                    {
                        BadStock = grouped.Sum(s => s.BadStock),
                        InTransit = grouped.Sum(s => s.InTransit),
                        ReadyToUse = grouped.Sum(s => s.ReadyToUse),
                        Repair = grouped.Sum(s => s.Repair),
                        TotalStockMntc = grouped.Sum(s => s.TotalStockMntc),
                        TotalStockProd = grouped.Sum(s => s.TotalStockProd),
                        Used = grouped.Sum(s => s.Used),
                        InventoryDate = grouped.First().InventoryDate.ToString(),
                        ItemCode = grouped.Key.ItemCode,
                        ItemDescription = grouped.Key.ItemDescription,
                        LocationCode = grouped.First().LocationCode,
                        QI = grouped.First().QI,
                        RowID = grouped.First().RowID,
                        UnitCode = grouped.First().UnitCode,
                    };

                var pageResult = new PageResult<MaintenanceEquipmentStockReportViewModel>(SummedList.ToList(), criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Maintenance Report Equipment Stock - GetMaintenanceEquipmentReport");
                return null;
            }
        }

        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="LocationCode"></param>
        /// <param name="UnitCode"></param>
        /// <param name="InventoryDate"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string LocationCode,string UnitCode, DateTime? InventoryDate)
        {
            try
            {
                var input = new GetMaintenanceEquipmentStockReportInput
                {
                    LocationCode = LocationCode,
                    UnitCode = UnitCode,
                    InventoryDate = InventoryDate
                };
                var unsortedmaintenanceEquipmentReports = _maintenanceBll.GetMaintenanceEquipmentStockReport(input);

                //var maintenanceEquipmentReports = unsortedmaintenanceEquipmentReports.OrderBy(o => o.ItemDescription).ToList();

                //var masterLists = _planningBll.GetReportSummaryProcessTargets(criteria);
                var viewModel =
                    Mapper.Map<List<MaintenanceEquipmentStockReportViewModel>>(unsortedmaintenanceEquipmentReports);

                //sorting
                var SortedList = viewModel.OrderBy(o => o.ItemDescription).ToList();

                //suming
                //var summedList = SortedList.Sum(i=>i.ItemCode);
                var SummedList = from val in SortedList
                    group val by new {val.ItemCode, val.ItemDescription}
                    into grouped
                    select new MaintenanceEquipmentStockReportViewModel
                    {
                        BadStock = grouped.Sum(s => s.BadStock),
                        InTransit = grouped.Sum(s => s.InTransit),
                        ReadyToUse = grouped.Sum(s => s.ReadyToUse),
                        Repair = grouped.Sum(s => s.Repair),
                        TotalStockMntc = grouped.Sum(s => s.TotalStockMntc),
                        TotalStockProd = grouped.Sum(s => s.TotalStockProd),
                        Used = grouped.Sum(s => s.Used),
                        InventoryDate = grouped.First().InventoryDate,
                        ItemCode = grouped.Key.ItemCode,
                        ItemDescription = grouped.Key.ItemDescription,
                        LocationCode = grouped.First().LocationCode,
                        QI = grouped.First().QI,
                        RowID = grouped.First().RowID,
                        UnitCode = grouped.First().UnitCode,
                    };

                var maintenanceEquipmentReports = SummedList.ToList();

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.MaintenanceEquipmentStockReport + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }


                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == LocationCode)
                    {
                        locationCompat = item.Text;
                    }
                }

                var UnitCodeExcel = UnitCode;
                if (UnitCode.Equals("%"))
                {
                    UnitCodeExcel = "All";
                }
                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, locationCompat);
                    slDoc.SetCellValue(4, 2, UnitCodeExcel);
                    slDoc.SetCellValue(5, 2, InventoryDate.Value.ToString(Constants.DefaultDateOnlyFormat));

                    //row values
                    var iRow = 9;
                    var rowNUmber = 0;
                    foreach (var maintenanceEquipmentReport in maintenanceEquipmentReports)
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
                            style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray,
                                System.Drawing.Color.LightGray);
                        }

                        slDoc.SetCellValue(iRow, 1, ++rowNUmber);
                        slDoc.SetCellValue(iRow, 2, maintenanceEquipmentReport.ItemCode);
                        slDoc.SetCellValue(iRow, 3, maintenanceEquipmentReport.ItemDescription);
                        slDoc.SetCellValue(iRow, 4, maintenanceEquipmentReport.InTransit.ToString());
                        slDoc.SetCellValue(iRow, 5, maintenanceEquipmentReport.QI.ToString());
                        slDoc.SetCellValue(iRow, 6, maintenanceEquipmentReport.ReadyToUse.ToString());
                        slDoc.SetCellValue(iRow, 7, maintenanceEquipmentReport.BadStock.ToString());
                        slDoc.SetCellValue(iRow, 8, maintenanceEquipmentReport.TotalStockMntc.ToString());
                        slDoc.SetCellValue(iRow, 9, maintenanceEquipmentReport.Used.ToString());
                        slDoc.SetCellValue(iRow, 10, maintenanceEquipmentReport.Repair.ToString());
                        slDoc.SetCellValue(iRow, 11, maintenanceEquipmentReport.TotalStockProd.ToString());
                        slDoc.SetCellStyle(iRow, 1, iRow, 11, style);
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
                    slDoc.AutoFitColumn(1, 11);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "MaintenanceEquipmentStockReport_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { LocationCode, UnitCode, InventoryDate }, "Maintenance Report Equipment Stock - Generate Excel");
                return null;
            }
        }
	}
}