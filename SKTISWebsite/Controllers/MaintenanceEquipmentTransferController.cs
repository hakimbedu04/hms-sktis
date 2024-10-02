using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.EquipmentTransfer;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using Core = HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.BusinessObjects;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceEquipmentTransferController : BaseController
    {
        private IMaintenanceBLL _maintenanceBLL;
        private IApplicationService _svc;
        private IVTLogger _vtlogger;

        public MaintenanceEquipmentTransferController(IVTLogger vtlogger, IMaintenanceBLL maintenanceBLL, IApplicationService svc)
        {
            _maintenanceBLL = maintenanceBLL;
            _svc = svc;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/EquipmentTransfer");
        }

        // GET: MaintenanceEquipmentTransfer
        public ActionResult Index()
        {
            var plntChild = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.PLNT, 1);
            var tpoChildLevel2 = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO, 2);

            var locations = _svc.GetPlantAndRegionalLocationLookupList();
            var locationsDestination = _svc.GetAllPlantAndRegionalLocationLookupList();
            var model = new InitEquipmentTransferViewModel
            {
                LocationLookupList = locations,
                LocationDestinationLookupList = locationsDestination
                //LocationLookupList = plntChild.Concat(tpoChildLevel2).ToList()
            };
            return View(model);
        }

        /// <summary>
        /// Gets the equipment transfers.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetEquipmentTransfers(GetEquipmentTransfersInput criteria)
        {
            try
            {
                var itemStatus = _maintenanceBLL.GetItemStatusForTransferByLocationCode(criteria.SourceLocationCode);
                var equipmentTransfers = _maintenanceBLL.GetEquipmentTransfers(criteria);
                var viewModel = Mapper.Map<List<EquipmentTransferViewModel>>(equipmentTransfers);
                // insert stock to every row
                for (var i = 0; i < viewModel.Count; i++)
                {
                    var inventory = _maintenanceBLL.GetMaintenanceExecutionInventoryView(
                        viewModel[i].LocationCodeSource, viewModel[i].ItemCode,
                        criteria.TransferDate.Value.Date);
                    if (itemStatus == EnumHelper.GetDescription(Core.Enums.ItemStatus.InTransit))
                    {
                        viewModel[i].Stock = inventory.StackIT;
                    }
                    else
                    {
                        viewModel[i].Stock = inventory.StackReady;
                    }
                }
                var pageResult = new PageResult<EquipmentTransferViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Maintenance Equipment Transfer - GetEquipmentTransfers");
                return null;
            }
        }

        /// <summary>
        /// Gets the item code select list.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetItemCodeSelectList(string locationCode)
        {
            var model = _svc.GetMaintenanceItemCodeByLocationCode(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int GetItemStock(EquipmentTransferDTO input)
        {
            //return _maintenanceBLL.GetAvailableEndingStockForTransfer(input);
            //var inventory = _maintenanceBLL.GetMaintenanceExecutionInventoryView(input.LocationCodeSource, input.ItemCode,
            //    input.TransferDate);
            //get endingStock for transfer
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var UserAD = strUserID.Username;
            var QParam = input.LocationCodeSource + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + UserAD;
            DateTime dt = Convert.ToDateTime(input.TransferDate);
            //var masterLists = _maintenanceBll.GetMaintenanceExecutionInventoryView(criteria);
            //var masterLists = _maintenanceBll.GetInventory(criteria.Date.Value.ToString("yyyy-MM-dd"), criteria.LocationCode, criteria.ItemType);
            var inventory = _maintenanceBLL.GetInventoryView(dt, input.LocationCodeSource, "%", QParam, UserAD);

            var itemStatus = _maintenanceBLL.GetItemStatusForTransferByLocationCode(input.LocationCodeSource);
            if (inventory != null)
            {
                int ret = inventory.Where(x=>x.ItemCode == input.ItemCode).Select(m => m.StackReady).FirstOrDefault().GetValueOrDefault() ;
                if (itemStatus == EnumHelper.GetDescription(Core.Enums.ItemStatus.InTransit))
                {
                    ret = inventory.Where(x => x.ItemCode == input.ItemCode).Select(m => m.StackIT).FirstOrDefault().GetValueOrDefault();
                    return ret;
                }
                else
                {
                    return ret;
                }
            }else{
                return 0;
            }
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectList(string locationCode, string locationCodeDestination)
        {
            var model = _svc.GetUnitCodeSelectListByLocationCodeSourceAndLocationCodeDestination(locationCode,locationCodeDestination);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save all new and updated general list item
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllEquipmentTransfers(InsertUpdateData<EquipmentTransferViewModel> bulkData)
        {
            try
            {
                var locationCodeSource = bulkData.Parameters != null
                    ? bulkData.Parameters["LocationCodeSource"]
                    : string.Empty;
                var locationCodeDestination = bulkData.Parameters != null
                    ? bulkData.Parameters["LocationCodeDestination"]
                    : string.Empty;
                var transferDate = bulkData.Parameters != null ? bulkData.Parameters["TransferDate"] : string.Empty;
                var unitCodeDestination = bulkData.Parameters != null
                    ? bulkData.Parameters["UnitCodeDestination"]
                    : string.Empty;
                var resultJSonSendEmail = "";
                var listResultJSon = new List<string>();

                if (bulkData.New != null)
                {
                    for (var i = 0; i < bulkData.New.Count; i++)
                    {
                        //check row is null
                        if (bulkData.New[i] == null) continue;
                        var equipmentTransfer = Mapper.Map<EquipmentTransferDTO>(bulkData.New[i]);

                        //set createdby and updatedby
                        equipmentTransfer.CreatedBy = GetUserName();
                        equipmentTransfer.UpdatedBy = GetUserName();

                        try
                        {
                            equipmentTransfer.LocationCodeSource = locationCodeSource;
                            equipmentTransfer.LocationCodeDestination = locationCodeDestination;
                            equipmentTransfer.TransferDate = DateTime.ParseExact(transferDate,
                                Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                            equipmentTransfer.UpdatedDate = DateTime.ParseExact(transferDate,
                                Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                            equipmentTransfer.CreatedDate = DateTime.ParseExact(transferDate,
                                Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                            equipmentTransfer.UnitCodeDestination = unitCodeDestination;

                            var item = _maintenanceBLL.InsertEquipmentTransfer(equipmentTransfer);
                            bulkData.New[i] = Mapper.Map<EquipmentTransferViewModel>(item);
                            bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentTransfer }, "Maintenance Equipment Transfer - save - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentTransfer }, "Maintenance Equipment Transfer - save - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            resultJSonSendEmail += bulkData.New[i].Message;
                        }
                    }
                    if (locationCodeSource != locationCodeDestination)
                    {
                        try
                        {
                            var date = DateTime.ParseExact(transferDate,
                                Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                            _maintenanceBLL.SendEmailSaveEquipmentTransfer(locationCodeSource, date,
                                locationCodeDestination, unitCodeDestination, GetUserName());
                        }
                        catch (Exception ex)
                        {
                            resultJSonSendEmail = "Failed to send email." + resultJSonSendEmail;
                            _vtlogger.Err(ex, new List<object> { bulkData, resultJSonSendEmail }, "Maintenance Equipment Transfer - save - Failed to send email");
                            return Json(listResultJSon);
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
                        var equipmentTransfer = Mapper.Map<EquipmentTransferDTO>(bulkData.Edit[i]);

                        //set updatedby
                        equipmentTransfer.UpdatedBy = GetUserName();

                        try
                        {
                            var item = _maintenanceBLL.UpdateEquipmentTransfer(equipmentTransfer);
                            bulkData.Edit[i] = Mapper.Map<EquipmentTransferViewModel>(item);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentTransfer }, "Maintenance Equipment Transfer - save - Update");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentTransfer }, "Maintenance Equipment Transfer - save - Update");
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Maintenance Equipment Transfer - save");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string sourceLocationCode, string destinationLocationCode, DateTime transferDate, string unitCodeDestination)
        {
            try
            {
                var input = new GetEquipmentTransfersInput
                {
                    SourceLocationCode = sourceLocationCode,
                    DestinationLocationCode = destinationLocationCode,
                    TransferDate = transferDate,
                    UnitCodeDestination = unitCodeDestination
                };
                var equipmentTransfers = _maintenanceBLL.GetEquipmentTransfers(input);

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

                var templateFile = Enums.ExcelTemplate.EquipmentTransfer + ".xlsx";
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
                    slDoc.SetCellValue(5, 2, unitCodeDestination);
                    slDoc.SetCellValue(6, 2, transferDate.Date.ToString(Constants.DefaultDateOnlyFormat));

                    //row values
                    var iRow = 9;

                    var itemStatus = _maintenanceBLL.GetItemStatusForTransferByLocationCode(sourceLocationCode);
                    var ci = CultureInfo.CurrentCulture;
                    foreach (var equipmentTransfer in equipmentTransfers.OrderByDescending(x => x.UpdatedDate))
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

                        int stock = 0;
                        var inventory = _maintenanceBLL.GetMaintenanceExecutionInventoryView(sourceLocationCode,
                            equipmentTransfer.ItemCode, transferDate);
                        stock = itemStatus == EnumHelper.GetDescription(Core.Enums.ItemStatus.InTransit)
                            ? inventory.StackIT
                            : inventory.StackReady;

                        slDoc.SetCellValue(iRow, 1, equipmentTransfer.ItemCode);
                        slDoc.SetCellValue(iRow, 2, equipmentTransfer.ItemDescription);
                        slDoc.SetCellValue(iRow, 3, equipmentTransfer.UOM);
                        slDoc.SetCellValue(iRow, 4, stock.ToString("F2", ci));
                        slDoc.SetCellValue(iRow, 5, equipmentTransfer.QtyTransfer.Value.ToString("F2", ci));
                        slDoc.SetCellValue(iRow, 6, equipmentTransfer.TransferNote);
                        slDoc.SetCellStyle(iRow, 1, iRow, 6, style);
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
                var fileName = "EquipmentTransfer_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { sourceLocationCode, destinationLocationCode, transferDate, unitCodeDestination }, "Maintenance Equipment Transfer - Generate Excel");
                return null;
            }
        }
    }
}