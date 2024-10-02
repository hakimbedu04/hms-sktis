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
using SKTISWebsite.Models.ExeEMSSourceData;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.Common;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class ExeEMSSourceDataController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IPlanningBLL _planningBll;
        private IExecutionPlantBLL _executionPlantBll;
        private IExeReportBLL _exeReportBLL;
        private IVTLogger _vtlogger;

        public ExeEMSSourceDataController(IApplicationService applicationService, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IPlanningBLL planningBll, IExecutionPlantBLL executionPlantBll, IExeReportBLL exeReportBLL)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _planningBll = planningBll;
            _executionPlantBll = executionPlantBll;
            _exeReportBLL = exeReportBLL;
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/Report/EMSSubmitted");
        }
        // GET: /ExeEMSSourceData/
        public ActionResult Index()
        {
            var maintenanitems = _masterDataBLL.GetAllMaintenanceItems();
            var location = _masterDataBLL.GetLocationInfo(CurrentUser.Location[0].Code) ?? new List<LocationInfoDTO>();
            var init = new InitExeEMSSourceDataViewModel()
            {
                PLNTChildLocation = Mapper.Map<List<SelectListItem>>(location),
                LocationNameLookupList = _svc.GetLocationNamesLookupList(),
                defaultLocation = CurrentUser.Location[0].Code
            };

            return View("Index", init);
        }
        [HttpGet]
        public JsonResult GetBrandCodeByLocationCode(string locationCode, DateTime? DateFrom, DateTime? DateTo)
        {
            var brandCodes = _executionPlantBll.GetBrandCodeByLocationDateFromDateToBySp(locationCode, DateFrom, DateTo);
            return Json(brandCodes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetExeEMSSourceData(GetExeEMSSourceDataInput criteria)
        {
            try
            {
                var masterLists = _exeReportBLL.GetReportEMSSourceData(criteria);
                var viewModel = Mapper.Map<List<ExeReportEMSSourceDataViewModel>>(masterLists);
                //sorting


                var sortedCollection =
                    (from item in viewModel
                        orderby item.ProductionDate, item.ProducedQTY, item.PackageQTY
                        select item).ToList();


                //var SortedList = viewModel.OrderBy(o => o.ProductionDate).ToList();
                var pageResult = new PageResult<ExeReportEMSSourceDataViewModel>(sortedCollection, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Exe EMS Source Data - GetExeEMSSourceData");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string brandCode, DateTime? datefrom, DateTime? dateto)
        {
            try
            {
                var input = new GetExeEMSSourceDataInput
                {
                    LocationCode = locationCode,
                    BrandCode = brandCode,
                    DateFrom = datefrom == null ? DateTime.Now : datefrom.Value,
                    DateTo = dateto == null ? DateTime.Now : dateto.Value
                };

                input.SortExpression = "LocationCode";
                input.SortOrder = "ASC";

                var exeReportEMSSourceDataDaily = _exeReportBLL.GetReportEMSSourceData(input);

                var exeReportEMSSourceDataDailyViewModel =
                    Mapper.Map<List<ExeReportEMSSourceDataViewModel>>(exeReportEMSSourceDataDaily);

                //sorting
                var sortedexeReportEMSSourceDataDailyViewModel =
                    (from item in exeReportEMSSourceDataDailyViewModel
                        orderby item.ProductionDate, item.ProducedQTY, item.PackageQTY
                        select item).ToList();

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExeEMSSourceData + ".xlsx";
                var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                //var allLocations = _svc.GetLocationCodeCompat();
                //string locationCompat = "";
                //foreach (SelectListItem item in allLocations)
                //{
                //    if (item.Value == locationCode)
                //    {
                //        locationCompat = item.Text;
                //    }
                //}

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    //slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    //slDoc.SetCellValue(4, 2, ": " + brandCode);
                    //slDoc.SetCellValue(3, 4, "Date : " + datefrom.Value.ToShortDateString());
                    //slDoc.SetCellValue(3, 5, "To ");
                    //slDoc.SetCellValue(3, 6, "Date : " + dateto.Value.ToShortDateString());


                    //row values
                    var iRow = 3;

                    foreach (var masterListGroup in sortedexeReportEMSSourceDataDailyViewModel)
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

                        slDoc.SetCellValue(iRow, 1, masterListGroup.Company);
                        slDoc.SetCellValue(iRow, 2, masterListGroup.LocationCode);
                        slDoc.SetCellValue(iRow, 3, masterListGroup.BrandCode);
                        //slDoc.SetCellValue(iRow, 4, masterListGroup.BrandGroupCode);
                        //slDoc.SetCellValue(iRow, 4, masterListGroup.Description);
                        slDoc.SetCellValue(iRow, 4,
                            masterListGroup.PackageQTY.HasValue ? Math.Round((double) masterListGroup.PackageQTY, 2) : 0);
                        slDoc.SetCellValue(iRow, 5,
                            masterListGroup.ProducedQTY.HasValue
                                ? Math.Round((double) masterListGroup.ProducedQTY, 2)
                                : 0);
                        //slDoc.SetCellValue(iRow, 8, masterListGroup.UOM);
                        slDoc.SetCellValue(iRow, 6, "BTG");
                        slDoc.SetCellValue(iRow, 7, masterListGroup.ProductionDate);
                        slDoc.SetCellValue(iRow, 8, "");
                        slDoc.SetCellValue(iRow, 9, "");
                        slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
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
                        AllowAutoFilter = false,
                        AllowSort = false
                    };


                    slDoc.ProtectWorksheet(slSheetProtection);
                    //slDoc.AutoFitColumn(1, 10);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "ProductionExecution_EMS_Source_Data_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> {  locationCode, brandCode, datefrom, dateto }, "Exe EMS Source Data - Generate Excel");
                return null;
            }
        }
    }
}