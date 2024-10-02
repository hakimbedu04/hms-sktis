using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MaintenanceExecutionItemConversion;
using SKTISWebsite.Models.MasterMntcItemLocation;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using Path = DocumentFormat.OpenXml.Drawing.Path;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceExecutionItemConversionController : BaseController
    {
        private IMasterDataBLL _bll;
        private IMaintenanceBLL _maintenanceBll;
        private IUploadService _uploadService;
        private IApplicationService _svc;
        private IVTLogger _vtlogger;

        public MaintenanceExecutionItemConversionController(IMasterDataBLL masterDataBll, IVTLogger vtlogger, IMaintenanceBLL maintenanceBll, IUploadService uploadService, IApplicationService applicationService)
        {
            _bll = masterDataBll;
            _maintenanceBll = maintenanceBll;
            _uploadService = uploadService;
            _svc = applicationService;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/ItemConversion");
        }

        // GET: MaintenanceExecutionItemConversion
        public ActionResult Index()
        {
            var plntChild = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.PLNT, 1);
            var tpoChildLevel2 = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO, 2);

            var initmntcMaintenanceExcItemConversion = new InitMaintenanceExecutionItemConversionViewModel
            {
                //Locations = _svc.GetDescendantLocationByLocationCode(CurrentUser.Location.Code, -1),
                //Locations = _svc.GetPlantAndRegionalLocationLookupList(),
                Locations = plntChild.Concat(tpoChildLevel2).ToList(),
                LocationNameLookupList = _svc.GetLocationNamesLookupList(),
                YearSelectList = _svc.GetGenWeekYears(),
                Months = GenericHelper.GetListOfMonth().Select(m => new SelectListItem { Text = m.Value, Value = m.Key.ToString() }).ToList(),
                ItemDescriptions = Mapper.Map<List<MstMntcItemDescription>>(_bll.GetAllMaintenanceItems()),
                MsterItemConversionComposites = Mapper.Map<List<MasterItemConversionComposite>>(_bll.GetMstMntvConverts(new GetMstMntcConvertInput())),
                DefaultWeek = _bll.GetGeneralWeekWeekByDate(DateTime.Now),
                TodayDate = DateTime.Now.ToShortDateString(),
            };
            return View("Index", initmntcMaintenanceExcItemConversion);
        }

        /// <summary>
        /// Gets the maintenance execution item conversion es.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMaintenanceExecutionItemConversionES(GetMaintenanceEquipmentItemConvertInput criteria)
        {
            try
            {
                var result = _maintenanceBll.GetMaintenanceEquipmentItemConverts(criteria);
                var viewModel = Mapper.Map<List<MaintenanceExecutionItemConversionViewModel>>(result);
                var pageResult = new PageResult<MaintenanceExecutionItemConversionViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Maintenance Execution Item Conversion - GetMaintenanceExecutionItemConversionES");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetItemCodeSource(string locationCode, bool conversionType, string sourceStatus, DateTime? date)
        {

            return Json(_svc.GetItemCodeSourceByLocationCodeAndConversionType(locationCode, conversionType, sourceStatus, date), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetItemCodeSourceSS(string locationCode, bool conversionType, string sourceStatus, DateTime? date)
        {
            return Json(_svc.GetItemCodeSourceByLocationCodeAndConversionType(locationCode, conversionType, sourceStatus, date), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEndingStock(string locationCode, string itemCode, DateTime date)
        {
            try
            {
                GetMaintenanceInventoryInput input = new GetMaintenanceInventoryInput
                {
                    LocationCode = locationCode,
                    ItemCode = itemCode,
                    InventoryDate = date,
                    ItemStatus = "READY TO USE"
                };
                var result = _maintenanceBll.GetMntcInventoryAllView(input);
                var endingStock = 0;
                if (result != null)
                    endingStock = !result.EndingStock.HasValue ? 0 : Convert.ToInt32(result.EndingStock.Value);
                return Json(endingStock, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, itemCode, date }, "Maintenance Execution Item Conversion - GetEndingStock");
                return null;
            }
        }

        /// <summary>
        /// Gets the maintenance execution item conversion.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMaintenanceExecutionItemConversion(GetMaintenanceEquipmentItemConvertInput criteria)
        {
            try
            {
                var result = _maintenanceBll.GetMaintenanceEquipmentItemConverts(criteria);
                var viewModel = Mapper.Map<List<MaintenanceExecutionItemConversionViewModel>>(result);
                var pageResult = new PageResult<MaintenanceExecutionItemConversionViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Maintenance Execution Item Conversion - GetMaintenanceExecutionItemConversion");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetReadyToUseAndBadStock(bool conversionType)
        {
            var result = _svc.GetReadyToUseAndBadStock(conversionType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the detail item code destination.
        /// </summary>
        /// <param name="sourceItemCode">The source item code.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDetailItemCodeDestination(string sourceItemCode, GetMaintenanceEquipmentItemConvertInput criteria)
        {
            var result = _maintenanceBll.GetMaintenanceExecutionItemConversionComposites(sourceItemCode, criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
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
        /// Gets the item code destination.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="itemCodeSource">The item code source.</param>
        /// <param name="conversionType">if set to <c>true</c> [conversion type].</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetItemCodeDestination(string locationCode, string itemCodeSource, bool conversionType)
        {
            return Json(_svc.GetItemCodeDestinationByLocationCodeAndConversionType(locationCode, itemCodeSource, conversionType), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves all maintenance execution item conversion.
        /// </summary>
        /// <param name="bulkData">The bulk data.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllMaintenanceExecutionItemConversion(InsertUpdateData<MaintenanceExecutionItemConversionViewModel> bulkData)
        {
            try
            {
                if (bulkData.New != null)
                {
                    for (int i = 0; i < bulkData.New.Count; i++)
                    {
                        var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
                        var transactionDate = bulkData.Parameters != null ? bulkData.Parameters["TransactionDate"] : "";

                        //check row is null
                        if (bulkData.New[i] == null) continue;

                        // set locationCode and transactionDate
                        bulkData.New[i].TransactionDate = transactionDate;
                        bulkData.New[i].LocationCode = locationCode;

                        var maintenanceItemConversionDto =
                            Mapper.Map<MaintenanceExecutionItemConversionDTO>(bulkData.New[i]);
                        maintenanceItemConversionDto.QtyDisposal = (bulkData.New[i].QtyDisposal == null)
                            ? 0
                            : bulkData.New[i].QtyDisposal;
                        //set createdby and updatedby
                        maintenanceItemConversionDto.CreatedBy = GetUserName();
                        maintenanceItemConversionDto.UpdatedBy = GetUserName();

                        try
                        {
                            var item =
                                _maintenanceBll.InsertMaintenanceExecutionItemConversion(maintenanceItemConversionDto);
                            bulkData.New[i] = Mapper.Map<MaintenanceExecutionItemConversionViewModel>(item);
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, maintenanceItemConversionDto },
                                "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversion - Insert");
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, maintenanceItemConversionDto },
                                "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversion - Insert");
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }
                }

                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        var maintenanceItemConversionDto =
                            Mapper.Map<MaintenanceExecutionItemConversionDTO>(bulkData.Edit[i]);
                        maintenanceItemConversionDto.QtyDisposal = (bulkData.Edit[i].QtyDisposal == null)
                            ? 0
                            : bulkData.Edit[i].QtyDisposal;
                        //set createdby and updatedby
                        maintenanceItemConversionDto.UpdatedBy = GetUserName();

                        try
                        {
                            var item =
                                _maintenanceBll.UpdateMaintenanceExecutionItemConversion(maintenanceItemConversionDto);
                            bulkData.Edit[i] = Mapper.Map<MaintenanceExecutionItemConversionViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, maintenanceItemConversionDto },
                                "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversion - Update");
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, maintenanceItemConversionDto }, 
                                "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversion - Update");
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversion");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveAllMaintenanceExecutionItemConversionES(InsertUpdateData<MaintenanceExecutionItemConversionViewModel> bulkData)
        {
            try
            {
                if (bulkData.New != null)
                {
                    for (int i = 0; i < bulkData.New.Count; i++)
                    {
                        var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
                        var transactionDate = bulkData.Parameters != null ? bulkData.Parameters["TransactionDate"] : "";

                        //check row is null
                        if (bulkData.New[i] == null) continue;

                        // set locationCode and transactionDate
                        bulkData.New[i].TransactionDate = transactionDate;
                        bulkData.New[i].LocationCode = locationCode;

                        var maintenanceItemConversionDto =
                            Mapper.Map<MaintenanceExecutionItemConversionDTO>(bulkData.New[i]);

                        //set createdby and updatedby
                        maintenanceItemConversionDto.CreatedBy = GetUserName();
                        maintenanceItemConversionDto.UpdatedBy = GetUserName();

                        try
                        {
                            if (bulkData.New[i].ItemDestinationEquipments != null)
                            {
                                foreach (var detail in bulkData.New[i].ItemDestinationEquipments)
                                {
                                    maintenanceItemConversionDto.ItemCodeDestination = detail.ItemCodeDestination;
                                    maintenanceItemConversionDto.SourceStock = maintenanceItemConversionDto.SourceStock;
                                    maintenanceItemConversionDto.QtyGood = detail.QtyGood;
                                    maintenanceItemConversionDto.QtyDisposal = detail.QtyDisposal;
                                    var destinationStock = maintenanceItemConversionDto.SourceStock*detail.QtyConvert*
                                                           detail.QtyGood;
                                    //maintenanceItemConversionDto.DestinationStock = destinationStock != null ? (int)destinationStock : 0;

                                    var item =
                                        _maintenanceBll.SaveMaintenanceExecutionItemConversionESDetail(
                                            maintenanceItemConversionDto);
                                    bulkData.New[i] = Mapper.Map<MaintenanceExecutionItemConversionViewModel>(item);
                                    bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                                }
                            }
                            else
                            {
                                var item =
                                    _maintenanceBll.UpdateMaintenanceExecutionItemConversionES(
                                        maintenanceItemConversionDto);
                                bulkData.New[i] = Mapper.Map<MaintenanceExecutionItemConversionViewModel>(item);
                                bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                            }
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, maintenanceItemConversionDto },
                               "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversionES - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, maintenanceItemConversionDto },
                               "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversionES - Insert");
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }
                }

                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        var maintenanceItemConversionDto =
                            Mapper.Map<MaintenanceExecutionItemConversionDTO>(bulkData.Edit[i]);

                        //set createdby and updatedby
                        maintenanceItemConversionDto.UpdatedBy = GetUserName();

                        try
                        {
                            if (bulkData.Edit[i].ItemDestinationEquipments != null)
                            {
                                foreach (var detail in bulkData.Edit[i].ItemDestinationEquipments)
                                {
                                    maintenanceItemConversionDto.ItemCodeDestination = detail.ItemCodeDestination;
                                    maintenanceItemConversionDto.SourceStock = maintenanceItemConversionDto.SourceStock;
                                    maintenanceItemConversionDto.QtyGood = detail.QtyGood;
                                    maintenanceItemConversionDto.QtyDisposal = detail.QtyDisposal;
                                    var destinationStock = maintenanceItemConversionDto.SourceStock*detail.QtyConvert*
                                                           detail.QtyGood;
                                    // maintenanceItemConversionDto.DestinationStock = destinationStock != null ? (int)destinationStock : 0;

                                    var item =
                                        _maintenanceBll.SaveMaintenanceExecutionItemConversionESDetail(
                                            maintenanceItemConversionDto);
                                    bulkData.Edit[i] = Mapper.Map<MaintenanceExecutionItemConversionViewModel>(item);
                                    bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                }
                            }
                            else
                            {
                                var item =
                                    _maintenanceBll.UpdateMaintenanceExecutionItemConversionES(
                                        maintenanceItemConversionDto);
                                bulkData.Edit[i] = Mapper.Map<MaintenanceExecutionItemConversionViewModel>(item);
                                bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                            }
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, maintenanceItemConversionDto },
                               "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversionES - Update");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, maintenanceItemConversionDto },
                               "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversionES - Update");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Maintenance Execution Item Conversion - SaveAllMaintenanceExecutionItemConversionES");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcelSE(bool conversionTypeES, string locationCodeES, int? kpsYearES, int? kpsWeekES, DateTime? transactionDateES, string locationNameES)
        {
            try
            {
                var input = new GetMaintenanceEquipmentItemConvertInput
                {
                    ConversionType = conversionTypeES,
                    KpsYear = kpsYearES.Value,
                    KpsWeek = kpsWeekES.Value,
                    LocationCode = locationCodeES,
                    TransactionDate = transactionDateES
                };
                var masterListGroups = _maintenanceBll.GetMaintenanceEquipmentItemConvertsExcel(input);

                MemoryStream ms = new MemoryStream();

                string strTempFileName = System.IO.Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.MaintenanceItemConversionES + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, locationCodeES + " - " + locationNameES);
                    slDoc.SetCellValue(4, 2, kpsYearES.ToString());
                    slDoc.SetCellValue(5, 2, kpsWeekES.ToString());
                    slDoc.SetCellValue(6, 2,
                        transactionDateES.HasValue ? transactionDateES.Value.Date.ToShortDateString() : "");

                    //row values
                    var iRow = 10;

                    foreach (
                        var masterListGroup in
                            masterListGroups.Select((value, index) => new {Value = value, Index = index}))
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
                        slDoc.SetCellValue(iRow, 1, masterListGroup.Value.SourceStatus);
                        slDoc.SetCellValue(iRow, 2, masterListGroup.Value.ItemCodeSource);
                        slDoc.SetCellValue(iRow, 3, masterListGroup.Value.ItemCodeSourceDescription);
                        slDoc.SetCellValue(iRow, 4, masterListGroup.Value.SourceStock);
                        slDoc.SetCellValue(iRow, 5, masterListGroup.Value.ItemCodeDestination);
                        slDoc.SetCellValue(iRow, 6, masterListGroup.Value.ItemCodeDestinationDescription);
                        //slDoc.SetCellValue(iRow, 6, masterListGroup.Value.DestinationStock);
                        slDoc.SetCellValue(iRow, 7,
                            masterListGroup.Value.QtyGood != null ? (int) masterListGroup.Value.QtyGood : 0);
                        slDoc.SetCellValue(iRow, 8,
                            masterListGroup.Value.QtyDisposal != null ? (int) masterListGroup.Value.QtyDisposal : 0);
                        slDoc.SetCellStyle(iRow, 1, iRow, 8, style);
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

                    slDoc.SetColumnWidth(1, 20);
                    slDoc.SetColumnWidth(2, 20);
                    slDoc.SetColumnWidth(3, 20);
                    slDoc.SetColumnWidth(4, 5);
                    slDoc.SetColumnWidth(5, 20);
                    slDoc.SetColumnWidth(6, 20);
                    slDoc.SetColumnWidth(7, 10);
                    slDoc.SetColumnWidth(8, 10);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "MaintenanceItemConversion_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> {  conversionTypeES, locationCodeES, kpsYearES, kpsWeekES, transactionDateES, locationNameES }, "Maintenance Execution Item Conversion - Generate Excel");
                return null;
            }
        }

        /// <summary>
        /// Generates the excel.
        /// </summary>
        /// <param name="conversionType">if set to <c>true</c> [conversion type].</param>
        /// <param name="locationCode">The location code.</param>
        /// <param name="kpsYear">The KPS year.</param>
        /// <param name="kpsWeek">The KPS week.</param>
        /// <param name="transactionDate">The transaction date.</param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(bool conversionType, string locationCode, int? kpsYear, int? kpsWeek, DateTime? transactionDate, string locationName)
        {
            try
            {
                var input = new GetMaintenanceEquipmentItemConvertInput
                {
                    ConversionType = conversionType,
                    KpsYear = kpsYear.Value,
                    KpsWeek = kpsWeek.Value,
                    LocationCode = locationCode,
                    TransactionDate = transactionDate
                };
                var masterListGroups = _maintenanceBll.GetMaintenanceEquipmentItemConverts(input);

                MemoryStream ms = new MemoryStream();

                string strTempFileName = System.IO.Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExcelTemplate.MaintenanceItemConversion + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, locationCode + " - " + locationName);
                    slDoc.SetCellValue(4, 2, kpsYear.ToString());
                    slDoc.SetCellValue(5, 2, kpsWeek.ToString());
                    slDoc.SetCellValue(6, 2,
                        transactionDate.HasValue ? transactionDate.Value.Date.ToShortDateString() : "");

                    //row values
                    var iRow = 10;

                    foreach (
                        var masterListGroup in
                            masterListGroups.Select((value, index) => new {Value = value, Index = index}))
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
                        slDoc.SetCellValue(iRow, 1, masterListGroup.Value.SourceStatus);
                        slDoc.SetCellValue(iRow, 2, masterListGroup.Value.ItemCodeSource);
                        slDoc.SetCellValue(iRow, 3, masterListGroup.Value.ItemCodeSourceDescription);
                        slDoc.SetCellValue(iRow, 4, masterListGroup.Value.SourceStock);
                        slDoc.SetCellValue(iRow, 5, masterListGroup.Value.ItemCodeDestination);
                        slDoc.SetCellValue(iRow, 6, masterListGroup.Value.ItemCodeDestinationDescription);
                        slDoc.SetCellValue(iRow, 7, masterListGroup.Value.QtyGood.ToString());
                        //slDoc.SetCellValue(iRow, 7, masterListGroup.Value.UpdatedBy);
                        //slDoc.SetCellValue(iRow, 8, masterListGroup.Value.UpdatedDate.ToString(Constants.DefaultDateFormat));
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
                    //slDoc.AutoFitColumn(1, 8);
                    slDoc.SetColumnWidth(1, 20);
                    slDoc.SetColumnWidth(2, 20);
                    slDoc.SetColumnWidth(3, 20);
                    slDoc.SetColumnWidth(4, 5);
                    slDoc.SetColumnWidth(5, 20);
                    slDoc.SetColumnWidth(6, 20);
                    slDoc.SetColumnWidth(7, 5);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "MaintenanceItemConversion_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { conversionType, locationCode, kpsYear, kpsWeek, transactionDate, locationName }, "Maintenance Execution Item Conversion - Generate Excel");
                return null;
            }
        }
    }
}