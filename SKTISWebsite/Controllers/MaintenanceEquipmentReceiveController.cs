using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.EquipmentReceive;
using SKTISWebsite.Models.MasterPlantProductionGroup;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceEquipmentReceiveController : BaseController
    {
        private IMaintenanceBLL _maintenanceBLL;
        private IApplicationService _svc;
        private IVTLogger _vtlogger;

        public MaintenanceEquipmentReceiveController(IVTLogger vtlogger, IMaintenanceBLL maintenanceBLL, IApplicationService svc)
        {
            _maintenanceBLL = maintenanceBLL;
            _svc = svc;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/EquipmentReceive");
        }

        // GET: MaintenanceEquipmentReceive
        public ActionResult Index()
        {
            var sourceLocations = _svc.GetPlantAndRegionalLocationLookupList(false);
            var locations = _svc.GetPlantAndRegionalLocationLookupList();
            var model = new InitEquipmentReceiveViewModel
            {
                LocationLookupList = locations,
                SourceLocationLookupList = sourceLocations
            };
            return View(model);
        }

        /// <summary>
        /// Gets the equipment receives.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetEquipmentReceives(GetEquipmentReceivesInput criteria)
        {
            try
            {
                var equipmentTransfers = _maintenanceBLL.GetEquipmentReceives(criteria);
                var viewModel = Mapper.Map<List<EquipmentReceiveViewModel>>(equipmentTransfers);
                var pageResult = new PageResult<EquipmentReceiveViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Maintenance Equipment Receive - GetEquipmentReceives");
                return null;
            }
        }

        /// <summary>
        /// Saves all equipment receive.
        /// </summary>
        /// <param name="bulkData">The bulk data.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllEquipmentReceive(InsertUpdateData<EquipmentReceiveViewModel> bulkData)
        {
            try
            {
                //var locationCodeSource = bulkData.Parameters != null ? bulkData.Parameters["LocationCodeSource"] : string.Empty;
                //var locationCodeDestination = bulkData.Parameters != null ? bulkData.Parameters["LocationCodeDestination"] : string.Empty;
                //var transferDate = bulkData.Parameters != null ? bulkData.Parameters["TransferDate"] : string.Empty;
                var receiveDate = bulkData.Parameters != null ? bulkData.Parameters["ReceiveDate"] : string.Empty;

                // save data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;

                        var equipmentReceiveViewModel = bulkData.Edit[i];
                        equipmentReceiveViewModel.ReceiveDate = receiveDate;

                        var equipmentReceive = Mapper.Map<EquipmentReceiveDTO>(equipmentReceiveViewModel);
                        equipmentReceive.UpdatedBy = GetUserName();
                        try
                        {
                            var item = _maintenanceBLL.UpdateEquipmentReceive(equipmentReceive);

                            bulkData.Edit[i] = Mapper.Map<EquipmentReceiveViewModel>(item);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentReceive }, "Maintenance Equipment Receive - SaveAllEquipmentReceive");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentReceive }, "Maintenance Equipment Receive - SaveAllEquipmentReceive");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
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
                        _maintenanceBLL.RefreshDeltaViewTable();
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Maintenance Equipment Receive - SaveAllEquipmentReceive");
                return null;
            }
        }

        /// <summary>
        /// Generates the excel.
        /// </summary>
        /// <param name="sourceLocationCode">The source location code.</param>
        /// <param name="destinationLocationCode">The destination location code.</param>
        /// <param name="transferDate">The transfer date.</param>
        /// <param name="receiveDate">The receive date.</param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string sourceLocationCode, string destinationLocationCode, DateTime transferDate, DateTime? receiveDate)
        {
            try
            {
                var input = new GetEquipmentReceivesInput
                {
                    SourceLocationCode = sourceLocationCode,
                    DestinationLocationCode = destinationLocationCode,
                    TransferDate = transferDate,
                    ReceiveDate = receiveDate
                };
                var equipmentReceives = _maintenanceBLL.GetEquipmentReceives(input);

                var allLocations = _svc.GetLocationCodeCompat();
                string sourceLocationCompat = "";
                string destinationLocationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == sourceLocationCode)
                    {
                        sourceLocationCompat = item.Text;
                    }
                    if (item.Value == destinationLocationCode)
                    {
                        destinationLocationCompat = item.Text;
                    }
                }

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.EquipmentReceive + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, sourceLocationCompat);
                    slDoc.SetCellValue(4, 2, destinationLocationCompat);
                    slDoc.SetCellValue(5, 2, transferDate.Date.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(6, 2,
                        receiveDate.HasValue
                            ? receiveDate.Value.Date.ToString(Constants.DefaultDateOnlyFormat)
                            : string.Empty);

                    //row values
                    var iRow = 10;

                    foreach (var equipmentReceive in equipmentReceives)
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
                            style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, Color.LightGray);
                        }

                        slDoc.SetCellValue(iRow, 1, equipmentReceive.ItemCode);
                        slDoc.SetCellValue(iRow, 2, equipmentReceive.ItemDescription);
                        slDoc.SetCellValue(iRow, 3, equipmentReceive.UOM);
                        slDoc.SetCellValue(iRow, 4, equipmentReceive.QtyTransfer.ToString());
                        slDoc.SetCellValue(iRow, 5, equipmentReceive.QtyReceive.ToString());
                        slDoc.SetCellValue(iRow, 6, equipmentReceive.TransferNote);
                        slDoc.SetCellValue(iRow, 7, equipmentReceive.ReceiveNote);
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
                    //slDoc.AutoFitColumn(1, 6);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "EquipmentReceive_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { sourceLocationCode, destinationLocationCode, transferDate, receiveDate }, "Maintenance Equipment Receive - Generate Excel");
                return null;
            }
        }
    }
}