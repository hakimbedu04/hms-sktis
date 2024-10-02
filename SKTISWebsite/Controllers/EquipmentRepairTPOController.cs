using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
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
using SKTISWebsite.Models.EquipmentRepairTPO;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.MasterGenList;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using System.IO;
using HMS.SKTIS.BusinessObjects.Inputs;

namespace SKTISWebsite.Controllers
{
    public class EquipmentRepairTPOController : BaseController
    {
        private IApplicationService _svc;
        private IMaintenanceBLL _maintenanceBLL;
        private IMasterDataBLL _masterDataBLL;
        private IVTLogger _vtlogger;

        public EquipmentRepairTPOController(IVTLogger vtlogger, IApplicationService svc, IMaintenanceBLL maintenanceBLL, IMasterDataBLL masterDataBLL)
        {
            _svc = svc;
            _maintenanceBLL = maintenanceBLL;
            _masterDataBLL = masterDataBLL;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/EquipmentRepairTPO");
        }

        // GET: EquipmentRepairTPO
        public ActionResult Index()
        {
            //var plntChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.TPO.ToString());
            var plntChildLocationLookupList = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO, 2);
            var defaultLocationCode = GetDefaultLocationCode(plntChildLocationLookupList);
            var initViewModel = new InitEquipmentRepairTPOViewModel()
            {
                DefaultLocation = defaultLocationCode,
                TPOChildLocationLookupList = plntChildLocationLookupList
            };
            return View("Index", initViewModel);
        }

        [HttpPost]
        public JsonResult GetItemCodeSelectList(string locationCode, DateTime transactionDate)
        {
            var model = _svc.GetItemCodeByLocationCodeAndTypeNotInEquipmentRepair(locationCode, Enums.ItemType.Equipment, transactionDate);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInventoryStock(string inventoryDate, string itemCode, string locationCode)
        {
            var input = new GetInventoryStockInput
            {
                InventoryDate = DateTime.ParseExact(inventoryDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
                ItemCode = itemCode,
                ItemStatus = EnumHelper.GetDescription(Enums.ItemStatus.OnRepair),
                LocationCode = locationCode,
                UnitCode = "PROD"
            };

            var model = _maintenanceBLL.GetInventoryStock(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSparepartsUsage(string sourceItemCode, DateTime transactionDate, string locationCode)
        {
            //var model = _svc.GetSparepartsByItemCode(sourceItemCode);
            //var model = _masterDataBLL.GetSparepartsByItemCode(new GetEquipmentRepairItemUsage { ItemSourceCode = sourceItemCode, LocationCode = locationCode, TransactionDate = transactionDate });
            var model = _masterDataBLL.GetTpoSparepartsByItemCode(new GetEquipmentRepairItemUsage { ItemSourceCode = sourceItemCode, LocationCode = locationCode, TransactionDate = transactionDate });
            //for(int i = 0; i < model.Count; i++)
            //{
            //    var inventory = _maintenanceBLL.GetMaintenanceInventory(
            //        new GetMaintenanceInventoryInput
            //        {
            //            ItemCode = model[i].ItemCode, 
            //            LocationCode = locationCode, 
            //            ItemStatus = Enums.ItemStatus.ReadyToUse.ToString(),
            //            SortExpression = "InventoryDate",
            //            SortOrder = "DESC"
            //        }
            //    );

            //    if (inventory != null)
            //    {
            //        if ((inventory.EndingStock == 0))
            //        {
            //            model.RemoveAt(i);
            //        }
            //    }
            //    else
            //    {
            //        model.RemoveAt(i);
            //    }
            //}
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEquipmentRepairs(GetPlantEquipmentRepairsTPOInput criteria)
        {
            try
            {
                var equipmentTransfers = _maintenanceBLL.GetTPOEquipmentRepairs(criteria);
                var viewModel = Mapper.Map<List<EquipmentRepairTPOViewModel>>(equipmentTransfers);
                var pageResult = new PageResult<EquipmentRepairTPOViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Equipment Repair TPO - GetEquipmentRepairs");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveAllEquipmentRepair(InsertUpdateData<EquipmentRepairTPOViewModel> bulkData)
        {
            try
            {
                if (bulkData.New != null)
                {
                    for (var i = 0; i < bulkData.New.Count; i++)
                    {
                        //check row is null
                        if (bulkData.New[i] == null) continue;

                        var locationCode = bulkData.Parameters != null
                            ? bulkData.Parameters["LocationCode"]
                            : string.Empty;
                        var transactionDate = bulkData.Parameters != null
                            ? bulkData.Parameters["TransactionDate"]
                            : string.Empty;

                        //set value
                        bulkData.New[i].LocationCode = locationCode;
                        bulkData.New[i].TransactionDate = transactionDate;

                        var equipmentRepair = Mapper.Map<EquipmentRepairTPODTO>(bulkData.New[i]);

                        //set createdby and updatedby
                        equipmentRepair.CreatedBy = GetUserName();
                        equipmentRepair.UpdatedBy = GetUserName();

                        //Set Shift
                        var locationInfo = _masterDataBLL.GetMstLocationById(locationCode);
                        equipmentRepair.Shift = locationInfo.Shift;

                        //Set UnitCode PROD
                        equipmentRepair.UnitCode = Enums.UnitCodeDefault.PROD.ToString();
                        //equipmentRepair.UnitCode = Enums.UnitCodeDefault.MTNC.ToString();

                        //pencocokan dengan item convert
                        DateTime dt = Convert.ToDateTime(transactionDate);
                        var model = _masterDataBLL.GetTpoSparepartsByItemCode(new GetEquipmentRepairItemUsage { ItemSourceCode = bulkData.New[i].ItemCode, LocationCode = locationCode, TransactionDate = dt });

                        try
                        {
                            var item = new EquipmentRepairTPODTO();
                            if (bulkData.New[i].ItemSelectionItemCodeDetails != null)
                            {
                                if (bulkData.New[i].ItemSelectionItemCodeDetails.Count > 0)
                                {
                                    if (bulkData.New[i].ItemSelectionItemCodeDetails.Count != model.Count)
                                    {
                                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                                        bulkData.New[i].Message = "Please Check Usages";
                                        continue;
                                    }
                                    for (int a = 0; a < model.Count; a++)
                                    {
                                        if (bulkData.New[i].ItemSelectionItemCodeDetails[a].ItemCodeDestination == model[a].ItemCode)
                                        {
                                            equipmentRepair.ItemCodeDestination = bulkData.New[i].ItemSelectionItemCodeDetails[a].ItemCodeDestination;
                                            equipmentRepair.QtyUsage = bulkData.New[i].ItemSelectionItemCodeDetails[a].Quantity;
                                            equipmentRepair.LastQtyUsage = 0;

                                            item = _maintenanceBLL.SaveEquipmentRepairTPO(equipmentRepair);
                                            bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                                        }
                                        else
                                        {
                                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                                            bulkData.New[i].Message = "Please Check Usages";
                                            break;
                                        }
                                    }

                                    if (bulkData.New[i].ResponseType != Enums.ResponseType.Error.ToString())
                                    {
                                        bulkData.New[i] = Mapper.Map<EquipmentRepairTPOViewModel>(item);
                                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                                    }
                                }
                                else
                                {
                                    if (bulkData.New[i].ResponseType != Enums.ResponseType.Error.ToString())
                                    {
                                        item = _maintenanceBLL.SaveEquipmentRepairTPO(equipmentRepair);
                                        bulkData.New[i] = Mapper.Map<EquipmentRepairTPOViewModel>(item);
                                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (bulkData.New[i].ResponseType != Enums.ResponseType.Error.ToString())
                                {
                                    item = _maintenanceBLL.SaveEquipmentRepairTPO(equipmentRepair);
                                    bulkData.New[i] = Mapper.Map<EquipmentRepairTPOViewModel>(item);
                                    bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                                }
                            }
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentRepair }, "Equipment Repair TPO - SaveAllEquipmentRepairTPO");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentRepair }, "Equipment Repair TPO - SaveAllEquipmentRepairTPO");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
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
                        var equipmentRepair = Mapper.Map<EquipmentRepairTPODTO>(bulkData.Edit[i]);

                        //set updatedby
                        equipmentRepair.UpdatedBy = GetUserName();

                        //pencocokan dengan item convert
                        var locationCode = bulkData.Parameters != null
                            ? bulkData.Parameters["LocationCode"]
                            : string.Empty;
                        var transactionDate = bulkData.Parameters != null
                            ? bulkData.Parameters["TransactionDate"]
                            : string.Empty;
                        DateTime dt = Convert.ToDateTime(transactionDate);
                        var model = _masterDataBLL.GetTpoSparepartsByItemCode(new GetEquipmentRepairItemUsage { ItemSourceCode = bulkData.Edit[i].ItemCode, LocationCode = locationCode, TransactionDate = dt });


                        try
                        {
                            var item = new EquipmentRepairTPODTO();
                            if (bulkData.Edit[i].ItemSelectionItemCodeDetails != null)
                            {
                                if (bulkData.Edit[i].ItemSelectionItemCodeDetails.Count > 0)
                                {
                                    if (bulkData.Edit[i].ItemSelectionItemCodeDetails.Count != model.Count)
                                    {
                                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                                        bulkData.Edit[i].Message = "Please Check Usages";
                                        continue;
                                    }
                                    for(int a = 0; a<model.Count ; a++)
                                    {
                                        if (bulkData.Edit[i].ItemSelectionItemCodeDetails[a].ItemCodeDestination == model[a].ItemCode)
                                        {
                                            equipmentRepair.ItemCodeDestination = bulkData.Edit[i].ItemSelectionItemCodeDetails[a].ItemCodeDestination;
                                            equipmentRepair.QtyUsage = bulkData.Edit[i].ItemSelectionItemCodeDetails[a].Quantity;
                                            equipmentRepair.LastQtyUsage = Convert.ToInt32(model[a].Quantity);
                                            
                                            item = _maintenanceBLL.SaveEquipmentRepairTPO(equipmentRepair);
                                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                        }
                                        else
                                        {
                                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                                            bulkData.Edit[i].Message = "Please Check Usages";
                                            break;
                                        }
                                    }

                                    if (bulkData.Edit[i].ResponseType != Enums.ResponseType.Error.ToString())
                                    {
                                        bulkData.Edit[i] = Mapper.Map<EquipmentRepairTPOViewModel>(item);
                                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                    }
                                }
                                else
                                {
                                    if (bulkData.Edit[i].ResponseType != Enums.ResponseType.Error.ToString())
                                    {
                                        item = _maintenanceBLL.SaveEquipmentRepairTPO(equipmentRepair);
                                        bulkData.Edit[i] = Mapper.Map<EquipmentRepairTPOViewModel>(item);
                                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (bulkData.Edit[i].ResponseType != Enums.ResponseType.Error.ToString())
                                {
                                    item = _maintenanceBLL.SaveEquipmentRepairTPO(equipmentRepair);
                                    bulkData.Edit[i] = Mapper.Map<EquipmentRepairTPOViewModel>(item);
                                    bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                }
                            }
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentRepair }, "Equipment Repair TPO - SaveAllEquipmentRepairTPO - Update");
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, equipmentRepair }, "Equipment Repair TPO - SaveAllEquipmentRepairTPO - Update");
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Equipment Repair TPO - SaveAllEquipmentRepair");
                return null;
            }
        }

        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="listGroup"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, DateTime? transactionDate)
        {
            try
            {
                var input = new GetPlantEquipmentRepairsTPOInput
                {
                    LocationCode = locationCode,
                    TransactionDate = transactionDate
                };
                var plantEquipmentRepairs = _maintenanceBLL.GetTPOEquipmentRepairs(input);

                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == locationCode)
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

                var templateFile = Enums.ExcelTemplate.PlantEquipmentRepairTPO + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, locationCompat);
                    slDoc.SetCellValue(4, 2, GenericHelper.ConvertDateTimeToString(transactionDate));

                    //row values
                    var iRow = 7;

                    foreach (var masterListGroup in plantEquipmentRepairs)
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

                        slDoc.SetCellValue(iRow, 1, masterListGroup.ItemCode);
                        slDoc.SetCellValue(iRow, 2, masterListGroup.PreviousOutstanding);
                        slDoc.SetCellValue(iRow, 3, masterListGroup.QtyRepairRequest.ToString());
                        slDoc.SetCellValue(iRow, 4, masterListGroup.QtyCompletion.ToString());
                        slDoc.SetCellValue(iRow, 5, masterListGroup.QtyOutstanding.ToString());
                        slDoc.SetCellValue(iRow, 6, masterListGroup.QtyBadStock.ToString());
                        slDoc.SetCellValue(iRow, 7, masterListGroup.QtyTakenByUnit.ToString());
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
                    slDoc.AutoFitColumn(1, 7);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "MaintenanceEquipmentRepairTPO" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, transactionDate }, "Equipment Repair TPO - Generate Excel");
                return null;
            }
        }

        private string GetDefaultLocationCode(List<LocationLookupList> locations)
        {
            var defaultLocationCode = string.Empty;
            var locationLookupList = locations.FirstOrDefault();
            if (locationLookupList != null)
            {
                defaultLocationCode = locationLookupList.LocationCode;
            }
            return defaultLocationCode;
        }

    }
}
