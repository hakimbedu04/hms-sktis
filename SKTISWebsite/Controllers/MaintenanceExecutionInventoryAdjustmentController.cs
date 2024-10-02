using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using SKTISWebsite.Models.MaintenanceExecutionInventoryAdjustment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Models.MasterMntcItem;
using AutoMapper;
using SKTISWebsite.Models.Common;
using System.IO;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.Core;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceExecutionInventoryAdjustmentController : BaseController
    {
        private IMasterDataBLL _bll;
        private IMaintenanceBLL _maintenanceBll;
        private IApplicationService _svc;
        private IVTLogger _vtlogger;

        public MaintenanceExecutionInventoryAdjustmentController(IMasterDataBLL bll, IVTLogger vtlogger, IMaintenanceBLL maintenanceBll, IApplicationService service)
        {
            _bll = bll;
            _maintenanceBll = maintenanceBll;
            _svc = service;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/InventoryAdjustment");
        }

        public ActionResult Index()
        {
            var locations = _svc.GetPlantAndRegionalLocationLookupList();
            var init = new InitMaintenanceExecutionInventoryAdjustment()
            {
                LocationLookupList = locations,
                DefaultUnitCode = _svc.GetUnitCodeSelectListByLocationCode(locations.FirstOrDefault().LocationCode).FirstOrDefault().Value,
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _bll.GetGeneralWeekWeekByDate(DateTime.Now),
                TodayDate = DateTime.Now.ToShortDateString(),
                ItemsList = _bll.GetMstMaintenanceItems(new MstMntcItemInput())
            };
            return View("Index", init);
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
        /// Gets the week by month.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _bll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the date by week and year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDateByYearWeek(int year, int week)
        {
            var date = _bll.GetWeekByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Inventory Item By Location, Unit Code, and Item Status
        /// </summary>
        /// <param name="locationCode"></param>
        /// <param name="unitCode"></param>
        /// <param name="inventoryStatus"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetInventoryItems(string locationCode, string unitCode, string inventoryStatus, string itemType, string inventoryDate)
        {
            try
            {
                var input = new GetMaintenanceInventoryInput()
                {
                    LocationCode = locationCode,
                    UnitCode = unitCode,
                    ItemStatus = inventoryStatus,
                    InventoryDate = Convert.ToDateTime(inventoryDate)
                };
                var dbResult = _maintenanceBll.GetMaintenanceInventorys(input).DistinctBy(i => i.ItemCode);

                // Filter item code if item type is not "All"
                if ((itemType != "All") && (dbResult != null))
                {
                    var items =
                        _bll.GetMstMaintenanceItems(new MstMntcItemInput() {ItemType = itemType})
                            .Select(m => m.ItemCode);
                    var dbResultFiltered = dbResult.Where(m => items.Contains(m.ItemCode));

                    var inventoryItemList = new SelectList(dbResultFiltered, "ItemCode", "ItemCode");
                    return Json(inventoryItemList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var inventoryItemList = new SelectList(dbResult, "ItemCode", "ItemCode");
                    return Json(inventoryItemList, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, inventoryStatus, itemType }, "Maintenance Execution Inventory Adjustment - GetInventoryItems");
                return null;
            }

        }

        /// <summary>
        /// Get Adjustment Type from Master General List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAdjustmentType()
        {
            var input = new GetMstGenListsInput(){
                ListGroup = "MtncAdjType"
            };
            var dbResults = _bll.GetMstGeneralLists(input);
            var adjustmentTypeList = new SelectList(dbResults, "ListDetail", "ListDetail");
            return Json(adjustmentTypeList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemInformations(string itemCode)
        {
            var input = new MstMntcItemInput(){
                ItemCode = itemCode
            };
            var dbResult = _bll.GetMstMaintenanceItem(input);
            var viewModel = Mapper.Map<MasterMntcItemViewModel>(dbResult);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the maintenance execution inventory adjustment
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMaintenanceExecutionInventoryAdjustment(GetMaintenanceExecutionInventoryAdjustmentInput input)
        {
            try
            {
                var result = _maintenanceBll.GetMaintenanceExecutionInventoryAdjustment(input);
                var viewModel = Mapper.Map<List<MaintenanceExecutionInventoryAdjustmentViewModel>>(result);
                var pageResult = new PageResult<MaintenanceExecutionInventoryAdjustmentViewModel>(viewModel, input);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Maintenance Execution Inventory Adjustment - GetMaintenanceExecutionInventoryAdjustment");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveInventoryAdjustment(InsertUpdateData<MaintenanceExecutionInventoryAdjustmentViewModel> bulkData)
        {
            try
            {
                if (bulkData.New != null)
                {
                    for (int i = 0; i < bulkData.New.Count; i++)
                    {
                        if (bulkData.New[i] == null) continue;
                        var inventoryAdjustment = Mapper.Map<MaintenanceExecutionInventoryAdjustmentDTO>(bulkData.New[i]);

                        //set ceatedby and updatedby
                        inventoryAdjustment.CreatedBy = GetUserName();
                        inventoryAdjustment.UpdatedBy = GetUserName();

                        try
                        {
                            var item = _maintenanceBll.InsertInventoryAdjustment(inventoryAdjustment);
                            bulkData.New[i] = Mapper.Map<MaintenanceExecutionInventoryAdjustmentViewModel>(item);
                            bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, inventoryAdjustment }, "Maintenance Execution Inventory Adjustment - SaveInventoryAdjustment - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, inventoryAdjustment }, "Maintenance Execution Inventory Adjustment - SaveInventoryAdjustment - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.DataAlreadyExist);
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

                        bulkData.Edit[i].CreatedDate = null;
                        bulkData.Edit[i].UpdatedDate = null;
                        bulkData.Edit[i].AdjustmentDate = bulkData.Edit[i].AdjustmentDate.Replace(" 0:00:00", "");
                        var inventoryAdjustment =
                            Mapper.Map<MaintenanceExecutionInventoryAdjustmentDTO>(bulkData.Edit[i]);
                        inventoryAdjustment.UpdatedBy = GetUserName();
                        try
                        {
                            var item = _maintenanceBll.UpdateInventoryAdjustment(inventoryAdjustment);

                            bulkData.Edit[i] = Mapper.Map<MaintenanceExecutionInventoryAdjustmentViewModel>(item);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> {bulkData, inventoryAdjustment},
                                "Maintenance Execution Inventory Adjustment - SaveInventoryAdjustment - Update");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> {bulkData, inventoryAdjustment},
                                "Maintenance Execution Inventory Adjustment - SaveInventoryAdjustment - Update");
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Maintenance Execution Inventory Adjustment - SaveInventoryAdjustment");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteInventoryAdjustment(MaintenanceExecutionInventoryAdjustmentViewModel inventoryData)
        {
            var inventoryAdjustment = Mapper.Map<MaintenanceExecutionInventoryAdjustmentDTO>(inventoryData);
            try
            {               
                _maintenanceBll.DeleteInventoryAdjustment(inventoryAdjustment);
                inventoryData.ResponseType = Enums.ResponseType.Success.ToString();
            }
            catch (ExceptionBase ex)
            {
                _vtlogger.Err(ex, new List<object> { inventoryData, inventoryAdjustment }, "Maintenance Execution Inventory Adjustment - InventoryExecutionAdjustment - Delete");
                inventoryData.ResponseType = Enums.ResponseType.Error.ToString();
                inventoryData.Message = ex.Message;
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { inventoryData, inventoryAdjustment }, "Maintenance Execution Inventory Adjustment - InventoryExecutionAdjustment - Delete");
                inventoryData.ResponseType = Enums.ResponseType.Error.ToString();
                inventoryData.Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
            }
            return Json(inventoryData);
        }

        /// <summary>
        /// Generate To Excel File
        /// </summary>
        /// <param name="locationCode"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string unitCode, DateTime createDate)
        {
            try
            {
                var input = new GetMaintenanceExecutionInventoryAdjustmentInput
                {
                    LocationCode = locationCode,
                    UnitCode = unitCode,
                    CreatedDate = createDate
                };

                var inventoryAdjustments = _maintenanceBll.GetMaintenanceExecutionInventoryAdjustment(input);
                var ms = new MemoryStream();
                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.MaintenanceExecutionInventoryAdjustment + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    var locationName = _bll.GetMstLocationById(locationCode);
                    if (locationName != null)
                        slDoc.SetCellValue(3, 2, ": " + locationCode + " - " + locationName.LocationName);

                    if (unitCode != null)
                        slDoc.SetCellValue(4, 2, ": " + unitCode);

                    if (createDate != null)
                        slDoc.SetCellValue(5, 2, ": " + createDate.ToShortDateString());

                    //row values
                    var iRow = 8;

                    foreach (var inventoryAdjustment in inventoryAdjustments)
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


                        slDoc.SetCellValue(iRow, 1, inventoryAdjustment.LocationCode);
                        slDoc.SetCellValue(iRow, 2, inventoryAdjustment.UnitCode);
                        slDoc.SetCellValue(iRow, 3, inventoryAdjustment.UnitCodeDestination);
                        slDoc.SetCellValue(iRow, 4, inventoryAdjustment.ItemStatusFrom);
                        slDoc.SetCellValue(iRow, 5, inventoryAdjustment.ItemStatusTo);
                        slDoc.SetCellValue(iRow, 6, inventoryAdjustment.ItemCode);
                        slDoc.SetCellValue(iRow, 7, inventoryAdjustment.ItemCodeDescription);
                        slDoc.SetCellValue(iRow, 8,
                            inventoryAdjustment.AdjustmentDate.ToString(Constants.DefaultDateFormat));
                        slDoc.SetCellValue(iRow, 9, inventoryAdjustment.AdjustmentValue);
                        slDoc.SetCellValue(iRow, 10, inventoryAdjustment.AdjustmentType);
                        slDoc.SetCellValue(iRow, 11, inventoryAdjustment.Remark);
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
                var fileName = "MaintenanceExecutionInventoryAdjustment" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, createDate }, "Maintenance Execution Inventory Adjustment - Generate Excel");
                return null;
            }
        }
	}
}