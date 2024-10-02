using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using PagedList;
using SKTISWebsite.Code;
using SKTISWebsite.Helper;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.EquipmentRequest;
using SKTISWebsite.Models.MaintenanceItemDisposal;
using SKTISWebsite.Models.MasterGenLocations;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Models.EquipmentFulfillment;

namespace SKTISWebsite.Controllers
{
    public class EquipmentRequestController : BaseController
    {
        private IMasterDataBLL _masterDataBll;
        private IMaintenanceBLL _bll;
        private IApplicationService _svc;
        private IVTLogger _vtlogger;

        public EquipmentRequestController(IMasterDataBLL masterDataBll, IVTLogger vtlogger, IMaintenanceBLL bll, IApplicationService svc)
        {
            _masterDataBll = masterDataBll;
            _bll = bll;
            _svc = svc;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/EquipmentRequest");
        }

        // GET: MaintExecEquipRequest
        public ActionResult Index()
        {
            var location = _masterDataBll.GetAllLocationByLocationCode(CurrentUser.Location[0].Code, -1);
            var plntChild = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.PLNT, 1);
            var tpoChildLevel2 = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO, 2);

            var initEquipmentRequest = new InitEquipmentRequest
            {
                FilterLocationCode = CurrentUser.Location[0].Code,
                FilterRequestDate = DateTime.Now.ToShortDateString(),
                //LocationCodeSelectList = _svc.GetDescendantLocationByLocationCode(CurrentUser.Location.Code, -1),
                //LocationCodeSelectList = _svc.GetPlantAndRegionalLocationLookupList(),
                LocationCodeSelectList = plntChild.Concat(tpoChildLevel2).ToList(),
                LocationNameLookupList = _svc.GetLocationNamesLookupList(),
                LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(location),
            };

            return View("Index", initEquipmentRequest);
        }

        public JsonResult GetDetailInventory(string inventoryDate, string itemCode, string locationCode)
        {
            //var result = _bll.GetMaintenanceInventory(new GetMaintenanceInventoryInput
            //{
            //    InventoryDate = DateTime.ParseExact(inventoryDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
            //    ItemCode = itemCode,
            //    LocationCode = locationCode,
            //    ItemStatus = itemStatus
            //});

            try
            {
                var input = new GetEquipmentRequestsInput()
                {
                    LocationCode = locationCode,
                    RequestDate =
                        DateTime.ParseExact(inventoryDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
                    RequestNumber = "",
                    Requestor = "",
                    ItemCode = itemCode
                };

                var inventory = _bll.GetMaintenanceExecutionInventoryView(input.LocationCode, input.ItemCode,
                    input.RequestDate);

                var result = new ItemDetailViewModel
                {
                    ItemDescription =
                        inventory != null
                            ? inventory.ItemDescription
                            : _bll.GetItemDetail(input.ItemCode, input.LocationCode).ItemDescription,
                    ReadyToUse = inventory != null ? inventory.StackReady : 0,
                    OnRepair = inventory != null ? inventory.StackOR : 0,
                    OnUse = inventory != null ? inventory.StackOU : 0,
                    RequestedQuantity = 0
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { inventoryDate, itemCode, locationCode }, "Equipment Request - GetDetailInventory");
                return null;
            }
        }

        public JsonResult GetItemDescription(string itemCode)
        {
            var result = _masterDataBll.GetMstMaintenanceItem(new MstMntcItemInput
            {
                ItemCode = itemCode
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetItemCode(string locationCode)
        {
            var result = _bll.GetItemLocations(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEquipmentRequest(GetEquipmentRequestsInput criteria)
        {
            try
            {
                var pageResult = new PageResult<EquipmentRequestViewModel>();
                var equipmentRequests = _bll.GetEquipmentRequests(criteria);
                pageResult.TotalRecords = equipmentRequests.Count;
                pageResult.TotalPages = (equipmentRequests.Count/criteria.PageSize) +
                                        (equipmentRequests.Count%criteria.PageSize != 0 ? 1 : 0);
                var result = equipmentRequests.Skip((criteria.PageIndex - 1)*criteria.PageSize).Take(criteria.PageSize);
                pageResult.Results = Mapper.Map<List<EquipmentRequestViewModel>>(result);

                return Json(pageResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Equipment Request - GetEquipmentRequest");
                return null;
            }
        }

        [HttpPost]
        public JsonResult Save(InsertUpdateData<EquipmentRequestViewModel> bulkData)
        {
            try
            {
                var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
                var requestDate = bulkData.Parameters != null ? bulkData.Parameters["RequestDate"] : "";
                var requestNumber = bulkData.Parameters != null ? bulkData.Parameters["RequestNumber"] : "";
                var resultJSonSendEmail = "";
                var listResultJSon = new List<string>();
                DateTime reqDate = Convert.ToDateTime(requestDate);

                // Insert Data
                if (bulkData.New != null)
                {
                    for (var i = 0; i < bulkData.New.Count; i++)
                    {
                        //check row is null
                        if (bulkData.New[i] == null) continue;

                        var equipmentRequest = Mapper.Map<EquipmentRequestDTO>(bulkData.New[i]);
                        equipmentRequest.ApprovedQty = (bulkData.New[i].ApprovedQty == null)
                            ? 0
                            : bulkData.New[i].ApprovedQty;
                        equipmentRequest.FullfillmentQty = 0;
                        equipmentRequest.OutstandingQty = 0;
                        equipmentRequest.QtyLeftOver = 0;


                        // set createdby and updatedby
                        equipmentRequest.CreatedBy = GetUserName();
                        equipmentRequest.UpdatedBy = GetUserName();

                        // set params
                        equipmentRequest.LocationCode = locationCode;
                        equipmentRequest.RequestDate = DateTime.ParseExact(requestDate, Constants.DefaultDateOnlyFormat,
                            CultureInfo.InvariantCulture);
                        equipmentRequest.RequestNumber = requestNumber;

                        try
                        {
                            var item = _bll.InsertEquipmentRequest(equipmentRequest);
                            bulkData.New[i] = Mapper.Map<EquipmentRequestViewModel>(item);
                            bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentRequest }, "Equipment Request - Save - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentRequest }, "Equipment Request - Save - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                            resultJSonSendEmail += bulkData.New[i].Message;
                        }
                    }

                    try
                    {
                        _bll.SendEmailSaveEquipmentRequest(locationCode, reqDate, requestNumber, GetUserName());
                        //Dear Diah Marini,        Production Entry (Eblek) sudah tersedia, Silakan melanjutkan proses berikutnya:     Production Execution - TPO Production Entry: http://skt-is.voxteneo.co.id/ExeTPOProductionEntry/Index/IDAK/ROLLING/Resmi/FA054514.04/2016/26/2016-06-27/884        Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings to determine how attachments are handled
                    }
                    catch (Exception ex)
                    {
                        resultJSonSendEmail = "Failed to send email." + resultJSonSendEmail;

                        //listResultJSon.Add(resultJSonSubmitData);
                        //listResultJSon.Add(resultJSonSendEmail);
                        _vtlogger.Err(ex, new List<object> { bulkData, resultJSonSendEmail }, "Equipment Request - Save - Failed to send email");
                        return Json(listResultJSon);
                    }
                }

                // Update data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;

                        var equipmentRequest = Mapper.Map<EquipmentRequestDTO>(bulkData.Edit[i]);
                        equipmentRequest.ApprovedQty = (bulkData.Edit[i].ApprovedQty == null)
                            ? 0
                            : bulkData.Edit[i].ApprovedQty;
                        equipmentRequest.FullfillmentQty = 0;
                        equipmentRequest.OutstandingQty = 0;
                        equipmentRequest.QtyLeftOver = 0;

                        // set updated by
                        equipmentRequest.UpdatedBy = GetUserName();

                        // set params
                        equipmentRequest.LocationCode = locationCode;
                        equipmentRequest.RequestDate = DateTime.ParseExact(requestDate, Constants.DefaultDateOnlyFormat,
                            CultureInfo.InvariantCulture);
                        equipmentRequest.RequestNumber = requestNumber;

                        try
                        {
                            var item = _bll.UpdateEquipmentRequest(equipmentRequest);
                            bulkData.Edit[i] = Mapper.Map<EquipmentRequestViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }

                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> {bulkData, equipmentRequest},
                                "Equipment Request - Save - Update");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> {bulkData, equipmentRequest},
                                "Equipment Request - Save - Update");
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
                        _bll.RefreshDeltaViewTable();
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Equipment Request - Save");
                return null;
            }
        }

      
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string requestDate, string requestNo)
        {
            try
            {
                var input = new GetEquipmentRequestsInput()
                {
                    LocationCode = locationCode,
                    RequestDate =
                        DateTime.ParseExact(requestDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture)
                };
                var equipmentRequests = _bll.GetEquipmentRequests(input);

                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == locationCode)
                    {
                        locationCompat = item.Text;
                    }
                }

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.EquipmentRequest + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //row values
                    slDoc.SetCellValue(2, 2, locationCompat);
                    slDoc.SetCellValue(3, 2, requestDate);
                    slDoc.SetCellValue(4, 2, requestNo);
                    var iRow = 8;

                    foreach (var item in equipmentRequests.Select((value, index) => new {Value = value, Index = index}))
                    {
                        //get default style
                        var style = ExcelHelper.GetDefaultExcelStyle(slDoc);
                        if (item.Index%2 == 0)
                        {
                            style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                        }
                        slDoc.SetCellValue(iRow, 1, item.Value.ItemCode);
                        slDoc.SetCellValue(iRow, 2, item.Value.ItemDescription);
                        slDoc.SetCellValue(iRow, 3,
                            item.Value.ReadyToUse.HasValue ? item.Value.ReadyToUse.ToString() : "0");
                        slDoc.SetCellValue(iRow, 4, item.Value.OnUsed.HasValue ? item.Value.OnUsed.ToString() : "0");
                        slDoc.SetCellValue(iRow, 5, item.Value.OnRepair.HasValue ? item.Value.OnRepair.ToString() : "0");
                        slDoc.SetCellValue(iRow, 6, item.Value.TotalQty);
                        slDoc.SetCellValue(iRow, 7,
                            item.Value.ApprovedQty.HasValue ? item.Value.ApprovedQty.ToString() : "0");

                        //set style
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
                    //slDoc.AutoFitColumn(2, 7);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "EquipmentRequest_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, requestDate, requestNo }, "Equipment Request - Generate Excel");
                return null;
            }
        }
    }
}