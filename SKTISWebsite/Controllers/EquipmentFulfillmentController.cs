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
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.EquipmentFulfillment;
using SKTISWebsite.Models.EquipmentRequest;
using SKTISWebsite.Models.MasterGenLocations;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class EquipmentFulfillmentController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private IMaintenanceBLL _maintenanceBll;
        private IUnitOfWork _uow; 
        private IVTLogger _vtlogger;

        public EquipmentFulfillmentController(IApplicationService svc, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IMaintenanceBLL maintenanceBll, IUnitOfWork uow)
        {
            _svc = svc;
            _masterDataBll = masterDataBll;
            _maintenanceBll = maintenanceBll;
            _uow = uow;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/EquipmentFullfillment");
        }

        public ActionResult Index(string param1, string param2, int? param3)
        {
            var location = _masterDataBll.GetAllLocationByLocationCode(CurrentUser.Location[0].Code, -1);

            // Filter location code to only show location with request
            var requestLocation = _maintenanceBll.GetEquipmentRequestsLocation(new GetEquipmentRequestsInput());
            var requestLocationDistinct = requestLocation.Select(m => m.LocationCode);

            var plntChild = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.PLNT, 1);
            var tpoChildLevel2 = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO, 2);
            var filterLocation = plntChild.Concat(tpoChildLevel2).ToList();
            var filteredLocation = filterLocation.Where(m => requestLocationDistinct.Contains(m.LocationCode));
            if (param3.HasValue) setResponsibility(param3.Value);
            var model = new InitEquipmentFulfillment
            {
                FilterLocationCode = CurrentUser.Location[0].Code,
                //FilterLocation = _svc.GetDescendantLocationByLocationCode(CurrentUser.Location.Code, -1),
                //FilterLocation = _svc.GetPlantAndRegionalLocationLookupList(),
                DefaultRequestor = GetUserName(),
                FilterLocation = filterLocation.ToList(),
                FilterRequestNumber = _svc.GetRequestNumberSelectList(),
                FilterRequestor = _svc.GetEquipmentRequestRequestors(),
                LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(location),
                Param1Locationcode = param1,
                Param2Date = String.IsNullOrEmpty(param2) ? DateTime.Now.Date : DateTime.ParseExact(param2, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                //EquipmentRequestList = Mapper.Map<List<EquipmentRequestViewModel>>(_maintenanceBll.GetEquipmentRequestsTable(new GetEquipmentRequestsInput()))
                ItemsList = _masterDataBll.GetMstMaintenanceItems(new MstMntcItemInput())
            };

            return View("Index", model);
        }

        [HttpPost]
        public JsonResult GetRequestDate(string locationCode)
        {
            var input = new GetEquipmentRequestsInput()
            {
                LocationCode = locationCode,
                SortExpression = "RequestDate",
                SortOrder = "DESC",
                PageIndex = 0,
                PageSize = 1000
            };

            var requestLocation = _maintenanceBll.GetEquipmentRequestsLocation(input);
            var requestLocationDistinct = requestLocation.Select(m => m.RequestDate.ToShortDateString());

            return Json(requestLocationDistinct, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetRequestorByRequestNumber(string requestNumber)
        {
            if (!string.IsNullOrEmpty(requestNumber))
            {
                var input = new GetEquipmentRequestsInput()
                {
                    RequestNumber = requestNumber
                };

                var requestor = _maintenanceBll.GetEquipmentRequestsTable(input);
                var requestorDistinct = requestor.Select(m => m.CreatedBy).Distinct();
                return Json(requestorDistinct, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
            

        }

        public JsonResult GetMaintenanceInventory(string LocationCode, string RequestDate, string RequestNumber, string Requestor, string ItemCode)
        {
            try
            {
                // Since The Input is automatically converting the date from Javascript ajax
                // when it used directly as a method parameter
                // so the parameter is using string for RequestDate and Input class is manualy mapped using DateTime Formatter
                var input = new GetEquipmentRequestsInput()
                {
                    LocationCode = LocationCode,
                    RequestDate =
                        DateTime.ParseExact(RequestDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
                    RequestNumber = RequestNumber,
                    Requestor = Requestor,
                    ItemCode = ItemCode
                };

                var inventory = _maintenanceBll.GetMaintenanceExecutionInventoryView(input.LocationCode, input.ItemCode,
                    input.RequestDate);

                var result = new ItemDetailViewModel
                {
                    ItemDescription =
                        inventory != null
                            ? inventory.ItemDescription
                            : _maintenanceBll.GetItemDetail(input.ItemCode, input.LocationCode).ItemDescription,
                    ReadyToUse = inventory != null ? inventory.StackReady : 0,
                    OnRepair = inventory != null ? inventory.StackOR : 0,
                    OnUse = inventory != null ? inventory.StackOU : 0,
                    RequestedQuantity =
                        _maintenanceBll.GetEquipmentRequest(input) == null
                            ? 0
                            : _maintenanceBll.GetEquipmentRequest(input).Qty
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { LocationCode, RequestDate, RequestNumber, Requestor, ItemCode }, "Equipment fulfillment - GetMaintenanceInventory");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetRequestNumberByLocationCodeAndRequestDate(string locationCode, DateTime? requestDate)
        {
            var result = _svc.GetRequestNumberByLocationCodeAndDate(locationCode, requestDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemCodeFromEquipmentRequestByRequestNumber(string requestNumber)
        {
            var result = _svc.GetItemCodeFromEquipmentRequestByRequestNumberFulfillment(requestNumber);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRequestToOthersQuantityDetail(string itemCode)
        {
            if (string.IsNullOrEmpty(itemCode)) return Json(null, JsonRequestBehavior.AllowGet);
            var result = _maintenanceBll.GetEquipmentFulfillmentDetails(itemCode, EnumHelper.GetDescription(Enums.ItemStatus.ReadyToUse))
                .GroupBy(m => m.LocationCode).Select(group => group.First()).Where(p => p.EndingStock >= 0);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetQtyMntcRequestToLocation(string requestNumber, DateTime? fulfillmentDate)
        {
            if (string.IsNullOrEmpty(requestNumber)) return Json(null, JsonRequestBehavior.AllowGet);
            var result = _maintenanceBll.GetMntcRequestToLocation(requestNumber, fulfillmentDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEquipmentFulfillment(GetEquipmentFulfillmentInput criteria)
        {
            try
            {
                var pageResult = new PageResult<EquipmentFulfillmentViewModel>();
                var equipmentFulfillments = _maintenanceBll.GetEquipmentFulfillments(criteria);
                pageResult.TotalRecords = equipmentFulfillments.Count;
                pageResult.TotalPages = (equipmentFulfillments.Count/criteria.PageSize) +
                                        (equipmentFulfillments.Count%criteria.PageSize != 0 ? 1 : 0);
                var result =
                    equipmentFulfillments.Skip((criteria.PageIndex - 1)*criteria.PageSize).Take(criteria.PageSize);
                pageResult.Results = Mapper.Map<List<EquipmentFulfillmentViewModel>>(result);

                return Json(pageResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Equipment fulfillment - GetEquipmentFulfillment");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveAllEquipmentFulfillments(InsertUpdateData<EquipmentFulfillmentViewModel> bulkData)
        {
            try
            {
                var locationCode = bulkData.Parameters.ContainsKey("LocationCode")
                    ? bulkData.Parameters["LocationCode"]
                    : "";
                var requestDate = bulkData.Parameters.ContainsKey("RequestDate")
                    ? bulkData.Parameters["RequestDate"]
                    : "";
                var requestNumber = bulkData.Parameters.ContainsKey("RequestNumber")
                    ? bulkData.Parameters["RequestNumber"]
                    : "";
                var resultJSonSendEmail = "";
                var listResultJSon = new List<string>();
                DateTime Date = Convert.ToDateTime(requestDate);

                #region new unused

                //todo need to refactor this code later, uow savechange should called once at the end of transaction
                //if (bulkData.New != null)
                //{
                //    for (var i = 0; i < bulkData.New.Count; i++)
                //    {
                //        //check row is null
                //        if (bulkData.New[i] == null) continue;

                //        var equipmentFulfillment = Mapper.Map<EquipmentFulfillmentCompositeDTO>(bulkData.New[i]);

                //        equipmentFulfillment.LocationCode = locationCode;
                //        equipmentFulfillment.RequestDate = DateTime.ParseExact(requestDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                //        equipmentFulfillment.RequestNumber = requestNumber;


                //        try
                //        {
                //            var item = new EquipmentFulfillmentCompositeDTO();
                //            // Insert MntcEquipmentFulfillment
                //            equipmentFulfillment.CreatedBy = GetUserName();
                //            equipmentFulfillment.UpdateBy = GetUserName();
                //            item = _maintenanceBll.SaveEquipmentFulfillment(equipmentFulfillment);

                //            if (bulkData.New[i].RequestToOthersQuantityDetails != null)
                //            {
                //                foreach (var detail in bulkData.New[i].RequestToOthersQuantityDetails)
                //                {
                //                    if (detail != null)
                //                    {
                //                        // Insert or Update table MntcRequestToLocation
                //                        equipmentFulfillment.Quantity = detail.Quantity;
                //                        equipmentFulfillment.LocationCodeForReqToLocation = detail.LocationCode;
                //                        var maintenanceRequestToLocationModel =
                //                            Mapper.Map<MntcRequestToLocationDTO>(equipmentFulfillment);
                //                        maintenanceRequestToLocationModel.CreatedBy = GetUserName();
                //                        maintenanceRequestToLocationModel.UpdatedBy = GetUserName();
                //                        _maintenanceBll.SaveMaintenanceRequestToLocation(maintenanceRequestToLocationModel);
                //                    }
                //                }
                //            }

                //// Update table MntcEquipmentRequest
                //var maintenanceEquipmentRequestModel =
                //    Mapper.Map<EquipmentRequestDTO>(equipmentFulfillment);
                //maintenanceEquipmentRequestModel.UpdatedBy = GetUserName();
                ////maintenanceEquipmentRequestModel.OutstandingQty =
                ////    (maintenanceEquipmentRequestModel.ApprovedQty ?? 0) +
                ////    (maintenanceEquipmentRequestModel.FullfillmentQty ?? 0);
                //_maintenanceBll.UpdateEquipmentRequest(maintenanceEquipmentRequestModel, equipmentFulfillment);

                //            bulkData.New[i] = Mapper.Map<EquipmentFulfillmentViewModel>(item);
                //            bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                //        }
                //        catch (ExceptionBase ex)
                //        {
                //            _logger.Error("SaveEquipmentFulfillment - Insert", ex, equipmentFulfillment);
                //            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                //            bulkData.New[i].Message = ex.Message;
                //        }
                //        catch (Exception ex)
                //        {
                //            _logger.Error("SaveEquipmentFulfillment - Insert", ex, equipmentFulfillment);
                //            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                //            bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                //        }

                //    }
                //}

                #endregion

                // Update data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;

                        var equipmentFulfillment = Mapper.Map<EquipmentFulfillmentCompositeDTO>(bulkData.Edit[i]);

                        equipmentFulfillment.LocationCode = locationCode;
                        equipmentFulfillment.RequestDate = DateTime.ParseExact(requestDate,
                            Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                        equipmentFulfillment.RequestNumber = requestNumber;

                        try
                        {
                            var item = new EquipmentFulfillmentCompositeDTO();
                            // Insert MntcEquipmentFulfillment
                            equipmentFulfillment.UpdateBy = GetUserName();

                            var isExistForChangeDate =
                                _maintenanceBll.IsExistFulfillmentByRequestDate(equipmentFulfillment);
                            var exist = _maintenanceBll.IsExistEquipmentFulfillment(equipmentFulfillment);
                            if (!exist && !isExistForChangeDate)
                            {
                                equipmentFulfillment.CreatedBy = GetUserName();
                                item = _maintenanceBll.SaveEquipmentFulfillment(equipmentFulfillment);

                                // Update table MntcEquipmentRequest
                                var maintenanceEquipmentRequestModel =
                                    Mapper.Map<EquipmentRequestDTO>(equipmentFulfillment);
                                maintenanceEquipmentRequestModel.UpdatedBy = GetUserName();
                                _maintenanceBll.UpdateEquipmentRequest(maintenanceEquipmentRequestModel,
                                    equipmentFulfillment, exist);
                            }
                            else if (isExistForChangeDate)
                            {
                                _maintenanceBll.DeleteEquipmentFulfillment(equipmentFulfillment);

                                equipmentFulfillment.CreatedBy = GetUserName();
                                item = _maintenanceBll.SaveEquipmentFulfillment(equipmentFulfillment);

                                // Update table MntcEquipmentRequest
                                var maintenanceEquipmentRequestModel =
                                    Mapper.Map<EquipmentRequestDTO>(equipmentFulfillment);
                                maintenanceEquipmentRequestModel.UpdatedBy = GetUserName();
                                _maintenanceBll.UpdateEquipmentRequest(maintenanceEquipmentRequestModel,
                                    equipmentFulfillment, false);
                            }
                            else
                            {
                                // Update table MntcEquipmentRequest
                                var maintenanceEquipmentRequestModel =
                                    Mapper.Map<EquipmentRequestDTO>(equipmentFulfillment);
                                maintenanceEquipmentRequestModel.UpdatedBy = GetUserName();
                                _maintenanceBll.UpdateEquipmentRequest(maintenanceEquipmentRequestModel,
                                    equipmentFulfillment, exist);

                                item = _maintenanceBll.UpdateEquipmentFulfillment(equipmentFulfillment);
                            }

                            if (bulkData.Edit[i].RequestToOthersQuantityDetails != null)
                            {
                                foreach (var detail in bulkData.Edit[i].RequestToOthersQuantityDetails)
                                {
                                    if (detail != null)
                                    {
                                        // Insert or Update table MntcRequestToLocation
                                        equipmentFulfillment.Quantity = detail.Quantity;
                                        equipmentFulfillment.LocationCodeForReqToLocation = detail.LocationCode;
                                        var maintenanceRequestToLocationModel =
                                            Mapper.Map<MntcRequestToLocationDTO>(equipmentFulfillment);
                                        maintenanceRequestToLocationModel.CreatedBy = GetUserName();
                                        maintenanceRequestToLocationModel.UpdatedBy = GetUserName();
                                        _maintenanceBll.SaveMaintenanceRequestToLocation(
                                            maintenanceRequestToLocationModel);
                                    }
                                }
                            }

                            _uow.SaveChanges();


                            bulkData.Edit[i] = Mapper.Map<EquipmentFulfillmentViewModel>(item);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentFulfillment }, "Equipment fulfillment - Save - Update");
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentFulfillment }, "Equipment fulfillment - Save - Update");
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }

                        try
                        {
                            _maintenanceBll.SendEmailSaveEquipmentFulfillment(locationCode, Date, requestNumber,
                                GetUserName());
                        }
                        catch (Exception ex)
                        {
                            resultJSonSendEmail = "Failed to send email." + resultJSonSendEmail;
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentFulfillment }, "Equipment fulfillment - Save - Failed to send email");
                            //listResultJSon.Add(resultJSonSubmitData);
                            //listResultJSon.Add(resultJSonSendEmail);

                            //return Json(listResultJSon);
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Equipment fulfillment - Save");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string reqLocation, DateTime? reqDate, string reqNo, string requestor)
        {
            try
            {
                var input = new GetEquipmentFulfillmentInput
                {
                    RequestLocation = reqLocation,
                    RequestDate = reqDate,
                    RequestNumber = reqNo,
                    Requestor = requestor,
                    SortExpression = "UpdatedDate",
                    SortOrder = "DESC"
                };
                var masterListGroups = _maintenanceBll.GetEquipmentFulfillments(input);

                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == reqLocation)
                    {
                        locationCompat = item.Text;
                    }
                }

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.PlanningExcelTemplate.EquipmentFulfillment + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    slDoc.SetCellValue(4, 2,
                        ": " + (reqDate.HasValue ? reqDate.Value.ToString("dd/MM/yyyy") : string.Empty));
                    slDoc.SetCellValue(5, 2, ": " + reqNo);
                    slDoc.SetCellValue(6, 2, ": " + GenericHelper.ReplaceHexadecimalSymbols(requestor));


                    //row values
                    var iRow = 10;

                    foreach (var masterListGroup in masterListGroups)
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

                        slDoc.SetCellValue(iRow, 1,
                            GenericHelper.ConvertDateTimeToString(masterListGroup.FulFillmentDate));
                        slDoc.SetCellValue(iRow, 2, masterListGroup.ItemCode);
                        slDoc.SetCellValue(iRow, 3, masterListGroup.ItemDescription);
                        slDoc.SetCellValue(iRow, 4, masterListGroup.ReadyToUse.ToString());
                        slDoc.SetCellValue(iRow, 5, masterListGroup.OnUse.ToString());
                        slDoc.SetCellValue(iRow, 6, masterListGroup.OnRepair.ToString());
                        slDoc.SetCellValue(iRow, 7, masterListGroup.RequestedQuantity);
                        slDoc.SetCellValue(iRow, 8, masterListGroup.ApprovedQty.ToString());
                        slDoc.SetCellValue(iRow, 9, masterListGroup.RequestToQty.ToString());
                        slDoc.SetCellValue(iRow, 10, masterListGroup.PurchaseQuantity.ToString());
                        slDoc.SetCellValue(iRow, 11, masterListGroup.PurchaseNumber);
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
                    //slDoc.AutoFitColumn(1, 11);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "MaintenanceFulfillment" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { reqLocation, reqDate, reqNo, requestor }, "Equipment fulfillment - Generate Excel");
                return null;
            }
        }
    }
}