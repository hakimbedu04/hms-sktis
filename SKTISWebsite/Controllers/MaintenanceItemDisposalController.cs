using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.EntitySql;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BLL;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Helper;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MaintenanceItemDisposal;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.MasterGenStandardHour;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using Color = System.Drawing.Color;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceItemDisposalController : BaseController
    {
        private IApplicationService _svc;
        private IMaintenanceBLL _bll;
        private IMasterDataBLL _masterDataBll;
        private IVTLogger _vtlogger;

        public MaintenanceItemDisposalController(IApplicationService svc, IVTLogger vtlogger, IMaintenanceBLL bll, IMasterDataBLL masterDataBll)
        {
            _svc = svc;
            _bll = bll;
            _masterDataBll = masterDataBll;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/ItemDisposal");
        }

        // GET: MaintenanceItemDisposal
        public ActionResult Index()
        {
            //var plntChild = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.PLNT, 1);
            //var tpoChild = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO, 1);
            //var tpoChildLevel2 = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO, 2);

            var model = new InitItemDisposal();
            //model.FilterLocation = plntChild.Concat(tpoChild).Concat(tpoChildLevel2).ToList();
            model.itemDesc = _masterDataBll.GetAllMaintenanceItems();
            model.FilterLocation = _svc.GetPlantAndRegionalLocationLookupList();
            model.FilterMonth = new SelectList(GenericHelper.GetListOfMonth(), "Key", "Value");
            model.FilterWeek = _svc.GetWeekSelectList();
            model.YearSelectList = _svc.GetGenWeekYears();
            model.DateNow = DateTime.Now.Date;
            model.DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            return View("Index", model);
        }

        public JsonResult GetBadStock(GetMaintenanceInventoryInput input)
        {
            //var result = _bll.GetMaintenanceInventory(input);
            //return Json(result, JsonRequestBehavior.AllowGet);

            //var result = _bll.GetMntcInventoryAllView(input);
            var result = _bll.GetMntcBadStock(input);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemDescription(string itemCode)
        {
            var result = _masterDataBll.GetMstMaintenanceItem(new MstMntcItemInput
            {
                ItemCode = itemCode
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetItemCode(GetItemDisposalInput data)
        {
            
            //var result = _bll.GetItemLocations(locationCode);
            //var result = _svc.GetItemCodeFromInventoryHaveEndingStockByLocation(data.LocationCode, data.DateFrom, data.DateTo);
            var result = _svc.GetItemCodeFromInventoryHaveEndingStockByLocationWithBadStock(data.LocationCode, data.DateFrom, data.DateTo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMaintenanceItemDisposal(GetItemDisposalInput criteria)
        {
            try
            {
                var pageResult = new PageResult<GetItemDisposalViewModel>();
                var itemDisposals = _bll.GetMntcEquipmentItemDisposals(criteria);
                pageResult.TotalRecords = itemDisposals.Count;
                pageResult.TotalPages = (itemDisposals.Count/criteria.PageSize) +
                                        (itemDisposals.Count%criteria.PageSize != 0 ? 1 : 0);
                var result = itemDisposals.Skip((criteria.PageIndex - 1)*criteria.PageSize).Take(criteria.PageSize);
                pageResult.Results = Mapper.Map<List<GetItemDisposalViewModel>>(result);

                return Json(pageResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Maintenance Item Disposal - GetMaintenanceItemDisposal");
                return null;
            }
        }

        [HttpPost]
        public JsonResult Save(InsertUpdateData<GetItemDisposalViewModel> bulkData)
        {
            try
            {
                var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
                DateTime transactionDate =
                    Convert.ToDateTime(bulkData.Parameters != null ? bulkData.Parameters["TransactionDate"] : "").Date;

                // get shift
                var shift = _masterDataBll.GetShiftByLocationCode(locationCode).FirstOrDefault();

                // Insert Data
                if (bulkData.New != null)
                    for (var i = 0; i < bulkData.New.Count; i++)
                    {
                        //check row is null
                        if (bulkData.New[i] == null) continue;

                        var itemDisposal = Mapper.Map<MntcEquipmentItemDisposalCompositeDTO>(bulkData.New[i]);

                        // set createdby and updatedby
                        itemDisposal.CreatedBy = GetUserName();
                        itemDisposal.UpdatedBy = GetUserName();

                        // set shift
                        itemDisposal.Shift = shift;

                        // set params
                        itemDisposal.LocationCode = locationCode;
                        itemDisposal.TransactionDate = transactionDate;

                        try
                        {
                            var item = _bll.InsertItemDisposal(itemDisposal);
                            bulkData.New[i] = Mapper.Map<GetItemDisposalViewModel>(item);
                            bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, itemDisposal }, "Maintenance Item Disposal - Save - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;

                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, itemDisposal }, "Maintenance Item Disposal - Save - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();

                            if (ex.InnerException != null && ex.InnerException.InnerException != null)
                            {
                                if (ex.InnerException.InnerException.Message.Contains("FOREIGN KEY"))
                                    bulkData.New[i].Message =
                                        EnumHelper.GetDescription(
                                            ExceptionCodes.BLLExceptions.ForeignKeyErrorItemDisposal);
                                else
                                    bulkData.New[i].Message =
                                        EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            }
                            else
                                bulkData.New[i].Message =
                                    EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        }
                    }

                // Update data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;

                        var itemDisposal = Mapper.Map<MntcEquipmentItemDisposalCompositeDTO>(bulkData.Edit[i]);

                        // set updated by
                        itemDisposal.UpdatedBy = GetUserName();

                        // set shift
                        itemDisposal.Shift = shift;

                        try
                        {
                            var item = _bll.UpdateItemDisposal(itemDisposal);
                            bulkData.Edit[i] = Mapper.Map<GetItemDisposalViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }

                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> {bulkData, itemDisposal},
                                "Maintenance Item Disposal - Save - Update");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> {bulkData, itemDisposal},
                                "Maintenance Item Disposal - Save - Update");
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Maintenance Item Disposal - Save");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetItemDisposalInput input)
        {
            try
            {
                input.SortExpression = "UpdatedDate";
                input.SortOrder = "DESC";
                var itemDisposals = _bll.GetMntcEquipmentItemDisposals(input);

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.MaintenanceItemDisposal + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == input.LocationCode)
                    {
                        locationCompat = item.Text;
                    }
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //row values
                    slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    if (input.FilterType == "Year")
                    {
                        slDoc.SetCellValue(4, 2, ": " + input.FilterType + " " + input.Year);
                    }
                    else
                    {
                        slDoc.SetCellValue(4, 2, ": " + input.FilterType);
                    }
                    if (input.FilterType == "Monthly")
                    {
                        slDoc.SetCellValue(5, 1, "Month");
                        slDoc.SetCellValue(5, 2,
                            ": " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(input.MonthFrom) + " " +
                            input.YearMonthFrom + " - " +
                            CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(input.MonthTo) + " " +
                            input.YearMonthTo);
                    }
                    if (input.FilterType == "Weekly")
                    {
                        slDoc.SetCellValue(5, 1, "Week");
                        slDoc.SetCellValue(5, 2,
                            ": " + "Week " + input.WeekFrom.ToString() + " Year " + input.YearWeekFrom + " - " + "Week " +
                            input.WeekTo.ToString() + " Year " + input.YearWeekTo);
                    }
                    if (input.FilterType == "Daily")
                    {
                        slDoc.SetCellValue(5, 1, "Date");
                        DateTime start = Convert.ToDateTime(input.DateFrom);
                        DateTime end = Convert.ToDateTime(input.DateTo);
                        slDoc.SetCellValue(5, 2, ": " + start.ToShortDateString() + " to " + end.ToShortDateString());
                    }

                    var iRow = 8;

                    foreach (var item in itemDisposals.Select((value, index) => new {Value = value, Index = index}))
                    {
                        //get default style
                        var style = ExcelHelper.GetDefaultExcelStyle(slDoc);
                        if (item.Index%2 == 0)
                        {
                            style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                        }
                        var badStock = item.Value.EndingStock - item.Value.QtyDisposal;
                        slDoc.SetCellValue(iRow, 1, item.Value.ItemCode);
                        slDoc.SetCellValue(iRow, 2, item.Value.ItemDescription);
                        slDoc.SetCellValue(iRow, 3, badStock.ToString());
                        slDoc.SetCellValue(iRow, 4, item.Value.QtyDisposal);

                        //set style
                        slDoc.SetCellStyle(iRow, 1, iRow, 4, style);
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
                var fileName = "MaintenanceItemDisposal_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Maintenance Item Disposal - Generate Excel");
                return null;
            }
        }
        
    }
}