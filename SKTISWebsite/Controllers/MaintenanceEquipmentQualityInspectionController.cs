using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MaintenanceEquipmentQualityInspection;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceEquipmentQualityInspectionController : BaseController
    {
        private IMaintenanceBLL _maintenanceBll;
        private IApplicationService _applicationService;
        private IMasterDataBLL _masterDataBll;
        private IVTLogger _vtlogger;

        public MaintenanceEquipmentQualityInspectionController(IMaintenanceBLL maintenanceBll, IVTLogger vtlogger, IApplicationService applicationService, IMasterDataBLL masterDataBll)
        {
            _maintenanceBll = maintenanceBll;
            _applicationService = applicationService;
            _masterDataBll = masterDataBll;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/QualityInspection");
        }

        // GET: MaintenanceEquipmentQualityInspection
        public ActionResult Index(string param1, string param2, int? param3)
        {
            var locationsDefault = _applicationService.GetPlantAndRegionalLocationLookupList();
            var plntChild = _applicationService.GetLocationCodeSelectListByLevel(Enums.LocationCode.PLNT, 1);
            var tpoChildLevel2 = _applicationService.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO, 2);
            if (param3.HasValue) setResponsibility(param3.Value);
            var init = new InitMaintenanceExecutionQualityInspection
            {
                //Locations = new SelectList(_applicationService.GetLastLocationChildList(Enums.LocationCode.SKT.ToString()), "LocationCode", "LocationCode"),
                //Locations = _applicationService.GetPlantAndRegionalLocationLookupList(),
                LocationLookupList = locationsDefault,
                Locations = plntChild.Concat(tpoChildLevel2).ToList(),
                LocationNameLookupList = _applicationService.GetLocationNamesLookupList(),
                RequestNumber = _applicationService.GetRequestNumberSelectList(),
                ItemsList = _masterDataBll.GetMstMaintenanceItems(new MstMntcItemInput()),
                Param1LocationCode = param1,
                Param2 = param2
                
            };
            return View("Index", init);
        }
        public JsonResult GetMaintenanceExecutionQualityInspection(GetMaintenanceExecutionQualityInspectionInput criteria)
        {
            try
            {
                var inspectionResults = _maintenanceBll.GetMaintenanceExecutionQualityInspections(criteria);

                foreach (var maintenanceExecutionQualityInspectionDto in inspectionResults)
                {
                    var result =
                        _maintenanceBll.GetEquipmentRequestQtyLeftOver(
                            maintenanceExecutionQualityInspectionDto.LocationCode,
                            maintenanceExecutionQualityInspectionDto.ItemCode,
                            maintenanceExecutionQualityInspectionDto.RequestNumber);

                    maintenanceExecutionQualityInspectionDto.QtyLeftOver = Convert.ToInt32(result.QtyLeftOver);
                    maintenanceExecutionQualityInspectionDto.QtyLeftOverCount = Convert.ToInt32(result.QtyLeftOver) +
                                                                                (maintenanceExecutionQualityInspectionDto
                                                                                    .QtyReceiving.HasValue
                                                                                    ? maintenanceExecutionQualityInspectionDto
                                                                                        .QtyReceiving.Value
                                                                                    : 0);
                }
                var viewModel = Mapper.Map<List<MaintenanceEquipmentQualityInspectionViewModel>>(inspectionResults);
                var pageResult = new PageResult<MaintenanceEquipmentQualityInspectionViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Maintenance Equipment Quality Inspection - GetMaintenanceExecutionQualityInspection");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetRequestNumberByLocationCode(string locationCode)
        {
            var result = _applicationService.GetRequestNumberByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPreviousOutstanding(string locationCode, string itemCode, DateTime? inventoryDate, string requestNumber)
        {
            try
            {
                var prevOutstanding = 0.00;
                var result = _maintenanceBll.GetMntcEquipmentQualityInspecton(locationCode, itemCode, inventoryDate,
                    requestNumber);

                if (result != null)
                {
                    prevOutstanding = Convert.ToDouble(result.QtyOutstanding);
                }

                return Json(prevOutstanding, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, itemCode, inventoryDate, requestNumber }, "Maintenance Equipment Quality Inspection - GetPreviousOutstanding");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetQtyLeftOver(string locationCode, string itemCode, string requestNumber)
        {
            var result = _maintenanceBll.GetEquipmentRequestQtyLeftOver(locationCode, itemCode, requestNumber);
            int ApprovedQty = 0;
            if (result != null)
            {
                ApprovedQty = Convert.ToInt32(result.QtyLeftOver) == null ? 0 : Convert.ToInt32(result.QtyLeftOver);
            }
            return Json(ApprovedQty, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemCodeFromEquipmentRequestByRequestNumber(string requestNumber)
        {
            var result = new SelectList(new List<object>(), "", "");
            if(requestNumber==""){
                return Json(result, JsonRequestBehavior.AllowGet);
            }else{
                result = _applicationService.GetItemCodeFromEquipmentRequestByRequestNumber(requestNumber);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetPurchaseNumberFromFullFillment(string requestLocation, string itemCode, string requestNumber)
        {
            //var fulFillmentDate = _masterDataBll.
            var param = new GetEquipmentFulfillmentInput
            {
                RequestLocation = requestLocation,
                //FulfillmentDate = Convert.ToDateTime(fulfillmentDate),
                ItemCode = itemCode,
                RequestNumber = requestNumber
            };
            var result = _maintenanceBll.GetPurchaseNumber(param);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveMaintenanceEquipmentQualityInspection(InsertUpdateData<MaintenanceEquipmentQualityInspectionViewModel> bulkData)
        {
            try
            {
                if (bulkData.New != null)
                {
                    for (var i = 0; i < bulkData.New.Count; i++)
                    {
                        var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
                        var transactionDate = bulkData.Parameters != null ? bulkData.Parameters["TransactionDate"] : "";
                        var itemCode = bulkData.Parameters != null ? bulkData.Parameters["ItemCode"] : "";
                        var requestNumber = bulkData.Parameters != null ? bulkData.Parameters["RequestNumber"] : "";

                        //check row is null
                        if (bulkData.New[i] == null) continue;

                        bulkData.New[i].TransactionDate = transactionDate;
                        bulkData.New[i].LocationCode = locationCode;

                        var inspectionDto = Mapper.Map<MaintenanceExecutionQualityInspectionDTO>(bulkData.New[i]);
                        inspectionDto.QTYTransit = (bulkData.New[i].QTYTransit == null) ? 0 : bulkData.New[i].QTYTransit;
                        inspectionDto.QtyReceiving = (bulkData.New[i].QtyReceiving == null)
                            ? 0
                            : bulkData.New[i].QtyReceiving;
                        inspectionDto.QtyPass = (bulkData.New[i].QtyPass == null) ? 0 : bulkData.New[i].QtyPass;
                        inspectionDto.QtyReject = (bulkData.New[i].QtyReject == null) ? 0 : bulkData.New[i].QtyReject;
                        inspectionDto.QtyReturn = (bulkData.New[i].QtyReturn == null) ? 0 : bulkData.New[i].QtyReturn;
                        //set createdby and updatedby
                        inspectionDto.CreatedBy = GetUserName();
                        inspectionDto.UpdatedBy = GetUserName();

                        try
                        {
                            var item = _maintenanceBll.InsertMaintenanceExecutionQualityInspection(inspectionDto);
                            bulkData.New[i] = Mapper.Map<MaintenanceEquipmentQualityInspectionViewModel>(item);
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();

                            // Update table MntcEquipmentRequest
                            //var maintenanceEquipmentRequestModel = Mapper.Map<equipmentrequest>();
                            //maintenanceEquipmentRequestModel.UpdatedBy = GetUserName();
                            //_maintenanceBll.UpdateEquipmentRequest(maintenanceEquipmentRequestModel, equipmentFulfillment, true);

                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, inspectionDto }, "Maintenance Equipment Quality Inspection - SaveMaintenanceEquipmentQualityInspection - Insert");
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, inspectionDto }, "Maintenance Equipment Quality Inspection - SaveMaintenanceEquipmentQualityInspection - Insert");
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }
                }

                // Update data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        var inspectionDto = Mapper.Map<MaintenanceExecutionQualityInspectionDTO>(bulkData.Edit[i]);
                        inspectionDto.QTYTransit = (bulkData.Edit[i].QTYTransit == null)
                            ? 0
                            : bulkData.Edit[i].QTYTransit;
                        inspectionDto.QtyReceiving = (bulkData.Edit[i].QtyReceiving == null)
                            ? 0
                            : bulkData.Edit[i].QtyReceiving;
                        inspectionDto.QtyPass = (bulkData.Edit[i].QtyPass == null) ? 0 : bulkData.Edit[i].QtyPass;
                        inspectionDto.QtyReject = (bulkData.Edit[i].QtyReject == null) ? 0 : bulkData.Edit[i].QtyReject;
                        inspectionDto.QtyReturn = (bulkData.Edit[i].QtyReturn == null) ? 0 : bulkData.Edit[i].QtyReturn;
                        //set updatedby
                        inspectionDto.UpdatedBy = GetUserName();

                        try
                        {
                            var item = _maintenanceBll.UpdateMaintenanceExecutionQualityInspection(inspectionDto);
                            bulkData.Edit[i] = Mapper.Map<MaintenanceEquipmentQualityInspectionViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, inspectionDto }, "Maintenance Equipment Quality Inspection - SaveMaintenanceEquipmentQualityInspection - Update");
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, inspectionDto }, "Maintenance Equipment Quality Inspection - SaveMaintenanceEquipmentQualityInspection - Update");
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }
                }

                if (bulkData.New != null || bulkData.Edit != null)
                {
                    try
                    {
                        _maintenanceBll.RefreshDeltaViewTable();
                    }
                    catch (Exception e)
                    {
                        _vtlogger.Err(e, new List<object> { bulkData }, "Error Copy Delta View");
                    }
                }


                return Json(bulkData);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData }, "Maintenance Equipment Quality Inspection - SaveMaintenanceEquipmentQualityInspection");
                return null;
            }

        }

        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="listGroup"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string location, DateTime? TransactionDate, string locationName)
        {
            try
            {
                var input = new GetMaintenanceExecutionQualityInspectionInput {Location = location};
                if (TransactionDate.HasValue) input.TransactionDate = TransactionDate;
                var inspectionResults = _maintenanceBll.GetMaintenanceExecutionQualityInspections(input);

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.MaintenanceEquipmentQualityInspection + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + location + " - " + locationName);
                    string tanggal = TransactionDate.ToString();
                    tanggal = tanggal.Substring(0, 10);
                    //slDoc.SetCellValue(4, 2, ": " + TransactionDate.ToString().Replace("0:00:00", ""));
                    //Yg lama ada 0 dibbelakang tanggal
                    slDoc.SetCellValue(4, 2, ": " + tanggal);

                    //row values
                    var iRow = 7;

                    foreach (var inspectionResult in inspectionResults)
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

                        slDoc.SetCellValue(iRow, 1, inspectionResult.RequestNumber);
                        slDoc.SetCellValue(iRow, 2, inspectionResult.PurchaseNumber);
                        slDoc.SetCellValue(iRow, 3, inspectionResult.ItemCode);
                        slDoc.SetCellValue(iRow, 4, "");
                        slDoc.SetCellValue(iRow, 5, "");
                        slDoc.SetCellValue(iRow, 6, inspectionResult.DeliveryNote);
                        slDoc.SetCellValue(iRow, 7, inspectionResult.Comments);
                        slDoc.SetCellValue(iRow, 8, inspectionResult.Supplier);
                        slDoc.SetCellValue(iRow, 9, inspectionResult.PreviousOutstanding);
                        slDoc.SetCellValue(iRow, 10, inspectionResult.QTYTransit.ToString());
                        slDoc.SetCellValue(iRow, 11, inspectionResult.QtyReceiving.ToString());
                        slDoc.SetCellValue(iRow, 12, inspectionResult.QtyPass.ToString());
                        slDoc.SetCellValue(iRow, 13, inspectionResult.QtyReject.ToString());
                        slDoc.SetCellValue(iRow, 14, inspectionResult.QtyOutstanding.ToString());
                        slDoc.SetCellValue(iRow, 15, inspectionResult.QtyReturn.ToString());
                        slDoc.SetCellStyle(iRow, 1, iRow, 15, style);
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
                    slDoc.AutoFitColumn(1, 6);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "MaintenanceEquipmentQualityInspection_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { location, TransactionDate, locationName }, "Maintenance Equipment Quality Inspection - Generate Excel");
                return null;
            }
        }
    }
}