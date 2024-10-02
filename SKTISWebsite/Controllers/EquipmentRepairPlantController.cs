using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
using SKTISWebsite.Models.EquipmentRepairPlant;
using SKTISWebsite.Models.LookupList;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace SKTISWebsite.Controllers
{
    public class EquipmentRepairPlantController : BaseController
    {
        private IApplicationService _svc;
        private IMaintenanceBLL _maintenanceBLL;
        private IMasterDataBLL _masterDataBLL;
        private IVTLogger _vtlogger;

        public EquipmentRepairPlantController(IVTLogger vtlogger, IApplicationService svc, IMaintenanceBLL maintenanceBLL, IMasterDataBLL masterDataBLL)
        {
            _svc = svc;
            _maintenanceBLL = maintenanceBLL;
            _masterDataBLL = masterDataBLL;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/EquipmentRepairPlant");
        }

        // GET: EquipmentRepairPlant
        public ActionResult Index()
        {
            var plntChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            var defaultLocationCode = GetDefaultLocationCode(plntChildLocationLookupList);
            var defaultItemCode = GetDefaultItemCodeByLocation(defaultLocationCode);

            var model = new InitEquipmentRepairPlantViewModel
            {
                PLNTChildLocationLookupList = plntChildLocationLookupList,
                DefaultLocation = defaultLocationCode,
                DefaultItemCode = defaultItemCode
            };

            return View(model);
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

        private string GetDefaultItemCodeByLocation(string defaultLocationCode)
        {
            var defaultItemCode = string.Empty;

            if (string.IsNullOrEmpty(defaultLocationCode)) return defaultItemCode;

            var itemCodes = _svc.GetMaintenanceItemCodeByLocationCodeAndType(defaultLocationCode, Enums.ItemType.Equipment);

            if (!itemCodes.Any()) return defaultItemCode;

            var firstItemCode = itemCodes.FirstOrDefault();
            if (firstItemCode != null)
            {
                defaultItemCode = firstItemCode.ItemCode;
            }

            return defaultItemCode;
        }
        /// <summary>
        /// Gets the equipment repairs.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetEquipmentRepairs(GetPlantEquipmentRepairsInput criteria)
        {
            try
            {
                var equipmentTransfers = _maintenanceBLL.GetPlantEquipmentRepairs(criteria);
                var viewModel = Mapper.Map<List<EquipmentRepairViewModel>>(equipmentTransfers);
                var pageResult = new PageResult<EquipmentRepairViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Equipment Repair Plant - GetEquipmentRepairs");
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
            var model = _svc.GetMaintenanceItemCodeByLocationCodeAndType(locationCode, Enums.ItemType.Equipment);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the unit code select list.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUnitCodeSelectList(string locationCode)
        {
            var input = new GetAllUnitsInput()
            {
                LocationCode = locationCode,
                IgnoreList = new List<string>() { Enums.UnitCodeDefault.MTNC.ToString(), Enums.UnitCodeDefault.PROD.ToString() }
            };
            var units = _masterDataBLL.GetAllUnits(input);
            var model = new SelectList(units, "UnitCode", "UnitCode");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetUnitCodeSelectListByTransactionDate(GetPlantEquipmentRepairsInput criteria)
        {
            var exist = _maintenanceBLL.GetPlantEquipmentRepairs(criteria).Select(m => m.UnitCode).ToList();
            exist.Add(Enums.UnitCodeDefault.MTNC.ToString());
            exist.Add(Enums.UnitCodeDefault.PROD.ToString());

            var input = new GetAllUnitsInput()
            {
                LocationCode = criteria.LocationCode,
                IgnoreList = exist
            };

            // Show all units by input, removed responsibilities checks!
            var units = _masterDataBLL.GetAllUnits(input, false);

            var model = new SelectList(units, "UnitCode", "UnitCode");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the spareparts usage.
        /// </summary>
        /// <param name="itemSourceCode">The item source code.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSparepartsUsage(GetEquipmentRepairItemUsage input)
        {
            var model = _masterDataBLL.GetSparepartsByItemCode(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetInventoryStock(string inventoryDate, string itemCode, string locationCode,string unitCode)
        {
            var input = new GetInventoryStockInput
            {
                InventoryDate = DateTime.ParseExact(inventoryDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
                ItemCode = itemCode,
                ItemStatus = EnumHelper.GetDescription(Enums.ItemStatus.OnRepair),
                LocationCode = locationCode,
                UnitCode = unitCode
            };

            var model = _maintenanceBLL.GetInventoryStock(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Saves all equipment repair.
        /// </summary>
        /// <param name="bulkData">The bulk data.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllEquipmentRepair(InsertUpdateRepairPlant<EquipmentRepairViewModel> bulkData)
        {
            try
            {
                var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : string.Empty;
                var itemCode = bulkData.Parameters != null ? bulkData.Parameters["ItemCode"] : string.Empty;
                var transactionDate = bulkData.Parameters != null
                    ? bulkData.Parameters["TransactionDate"]
                    : string.Empty;
                var editedFlag = "true";

                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;

                        //set value
                        bulkData.Edit[i].LocationCode = locationCode;
                        bulkData.Edit[i].ItemCode = itemCode;
                        bulkData.Edit[i].TransactionDate = transactionDate;

                        var equipmentRepair = Mapper.Map<EquipmentRepairDTO>(bulkData.Edit[i]);

                        //set createdby and updatedby
                        equipmentRepair.CreatedBy = GetUserName();
                        equipmentRepair.UpdatedBy = GetUserName();

                        //pencocokan dengan item convert
                        DateTime dt = Convert.ToDateTime(transactionDate);
                        var model = _masterDataBLL.GetTpoSparepartsByItemCode(new GetEquipmentRepairItemUsage { ItemSourceCode = bulkData.Edit[i].ItemCode, LocationCode = locationCode, TransactionDate = dt });


                        if (editedFlag.Equals(bulkData.Edit[i].Message))
                        {
                            try
                            {
                                var item = new EquipmentRepairDTO();
                                if (bulkData.Sparepart != null)
                                {
                                    if (bulkData.Sparepart.Count > 0)
                                    {
                                        if (bulkData.Sparepart.Count != model.Count)
                                        {
                                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                                            bulkData.Edit[i].Message = "Please Check Usages";
                                            continue;
                                        }

                                        // Save Repair Item Usage
                                        if (bulkData.Sparepart != null)
                                        {
                                            //for (int a = 0; a < model.Count; a++)
                                            for (var s = 0; s < bulkData.Sparepart.Count; s++)
                                            {
                                                if (bulkData.Sparepart[s].ItemCode == model[s].ItemCode)
                                                {
                                                    //Untuk checking apakah ada data yang tidak sesuai
                                                }
                                                else
                                                {
                                                    bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                                                    bulkData.Edit[i].Message = "Please Check Usages";
                                                    break;
                                                }
                                            }
                                        }

                                        
                                        if (bulkData.Edit[i].ResponseType != Enums.ResponseType.Error.ToString())
                                        {
                                            if (bulkData.Sparepart != null)
                                            {
                                                for (var s = 0; s < bulkData.Sparepart.Count; s++)
                                                {

                                                    var repairItemUsage = new MntcRepairItemUsageDTO()
                                                    {
                                                        TransactionDate =
                                                            DateTime.ParseExact(transactionDate,
                                                                Constants.DefaultDateOnlyFormat,
                                                                CultureInfo.InvariantCulture),
                                                        LocationCode = locationCode,
                                                        ItemCodeSource = itemCode,
                                                        ItemCodeDestination = bulkData.Sparepart[s].ItemCode,
                                                        UnitCode = Enums.UnitCodeDefault.MTNC.ToString(),
                                                        QtyUsage = bulkData.Sparepart[s].Quantity
                                                    };

                                                    var itemSparepart =
                                                        _maintenanceBLL.SaveRepairItemUsage(repairItemUsage);
                                                }
                                            }
                                            item = _maintenanceBLL.SaveEquipmentRepair(equipmentRepair);
                                            bulkData.Edit[i] = Mapper.Map<EquipmentRepairViewModel>(item);
                                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (bulkData.Edit[i].ResponseType != Enums.ResponseType.Error.ToString())
                                        {
                                            item = _maintenanceBLL.SaveEquipmentRepair(equipmentRepair);
                                            bulkData.Edit[i] = Mapper.Map<EquipmentRepairViewModel>(item);
                                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (bulkData.Edit[i].ResponseType != Enums.ResponseType.Error.ToString())
                                    {
                                        item = _maintenanceBLL.SaveEquipmentRepair(equipmentRepair);
                                        bulkData.Edit[i] = Mapper.Map<EquipmentRepairViewModel>(item);
                                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                    }
                                }
                                //var item = _maintenanceBLL.SaveEquipmentRepair(equipmentRepair);
                                //bulkData.Edit[i] = Mapper.Map<EquipmentRepairViewModel>(item);
                                //bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();

                            }
                            catch (ExceptionBase ex)
                            {
                                _vtlogger.Err(ex, new List<object> { bulkData, equipmentRepair }, "Equipment Repair Plant - SaveAllEquipmentRepair");
                                bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                                bulkData.Edit[i].Message = ex.Message;
                            }
                            catch (Exception ex)
                            {
                                _vtlogger.Err(ex, new List<object> { bulkData, equipmentRepair }, "Equipment Repair Plant - SaveAllEquipmentRepair");
                                bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                                bulkData.Edit[i].Message =
                                    EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                                ;
                            }

                        }
                    }

                    //// Save Repair Item Usage
                    //if (bulkData.Sparepart != null)
                    //{
                    //    for (var s = 0; s < bulkData.Sparepart.Count; s++)
                    //    {
                    //        var repairItemUsage = new MntcRepairItemUsageDTO()
                    //        {
                    //            TransactionDate =
                    //                DateTime.ParseExact(transactionDate, Constants.DefaultDateOnlyFormat,
                    //                    CultureInfo.InvariantCulture),
                    //            LocationCode = locationCode,
                    //            ItemCodeSource = itemCode,
                    //            ItemCodeDestination = bulkData.Sparepart[s].ItemCode,
                    //            UnitCode = Enums.UnitCodeDefault.MTNC.ToString(),
                    //            QtyUsage = bulkData.Sparepart[s].Quantity
                    //        };

                    //        var itemSparepart = _maintenanceBLL.SaveRepairItemUsage(repairItemUsage);
                    //    }
                    //}
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Equipment Repair Plant - SaveAllEquipmentRepair");
                return null;
            }
        }

        /// <summary>
        /// Generates the excel.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="itemCode">The item code.</param>
        /// <param name="transferDate">The transfer date.</param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string itemCode, DateTime transactionDate)
        {
            try
            {
                var input = new GetPlantEquipmentRepairsInput
                {
                    LocationCode = locationCode,
                    ItemCode = itemCode,
                    TransactionDate = transactionDate
                };

                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == locationCode)
                    {
                        locationCompat = item.Text;
                    }
                }

                var allItems = _masterDataBLL.GetMstMaintenanceItem(new MstMntcItemInput
                {
                    ItemCode = itemCode
                });
                string itemCompat = allItems.ItemCode + " - " + allItems.ItemDescription;

                var plantEquipmentRepairs = _maintenanceBLL.GetPlantEquipmentRepairs(input);

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.PlantEquipmentRepair + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, locationCompat);
                    slDoc.SetCellValue(4, 2, itemCompat);
                    slDoc.SetCellValue(5, 2, transactionDate.Date.ToString(Constants.DefaultDateOnlyFormat));

                    //row values
                    var iRow = 8;

                    foreach (var equipmentRepair in plantEquipmentRepairs)
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

                        slDoc.SetCellValue(iRow, 1, equipmentRepair.UnitCode);
                        slDoc.SetCellValue(iRow, 2, equipmentRepair.PreviousOutstanding);
                        slDoc.SetCellValue(iRow, 3, equipmentRepair.QtyRepairRequest.ToString());
                        slDoc.SetCellValue(iRow, 4, equipmentRepair.QtyCompletion.ToString());
                        slDoc.SetCellValue(iRow, 5, equipmentRepair.QtyOutstanding.ToString());
                        slDoc.SetCellValue(iRow, 6, equipmentRepair.QtyBadStock.ToString());
                        slDoc.SetCellValue(iRow, 7, equipmentRepair.QtyTakenByUnit.ToString());
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
                var fileName = "MaintenanceExecution_EquipmentRepairPlant_" + DateTime.Now.ToString("dd_MM_yyyy") +
                               ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> {  locationCode, itemCode, transactionDate }, "Equipment Repair Plant - Generate Excel");
                return null;
            }
        }
    }
}