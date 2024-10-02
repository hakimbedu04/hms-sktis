using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MaintenanceExecutionInventory;
using SpreadsheetLight;
using AutoMapper;
using Color = System.Drawing.Color;
using Path = System.IO.Path;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceExecutionInventoryController : BaseController
    {
        private IMasterDataBLL _bll;
        private IMaintenanceBLL _maintenanceBll;
        private IApplicationService _svc;
        private IVTLogger _vtlogger;

        public MaintenanceExecutionInventoryController(IMasterDataBLL bll, IVTLogger vtlogger, IMaintenanceBLL maintenanceBll, IApplicationService service)
        {
            _bll = bll;
            _maintenanceBll = maintenanceBll;
            _svc = service;
            _vtlogger = vtlogger;
            SetPage("Maintenance/Execution/Inventory");
        }

        // GET: MaintenanceExecutionInventory
        public ActionResult Index()
        {
            var locations = _svc.GetPlantAndRegionalLocationLookupList();
            var init = new InitMaintenanceExecutionInventoryViewModel()
            {
                LocationLookupList = locations,
                CurrentDate = DateTime.Now.Date.ToString(),
                ItemTypes = _svc.GetGenListByListGroup(HMS.SKTIS.Core.Enums.MasterGeneralList.MtncItemType.ToString(), HMS.SKTIS.Core.Enums.Conversion.Box.ToString())
            };
            return View("Index", init);
        }

        [HttpPost]
        public JsonResult GetMaintenanceExecutionInventory(MaintenanceExecutionInventoryViewInput criteria)
        {
            try
            {
                UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
                var UserAD = strUserID.Username;
                var QParam = criteria.LocationCode + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + UserAD;
                DateTime dt = Convert.ToDateTime(criteria.Date);
                //var masterLists = _maintenanceBll.GetMaintenanceExecutionInventoryView(criteria);
                //var masterLists = _maintenanceBll.GetInventory(criteria.Date.Value.ToString("yyyy-MM-dd"), criteria.LocationCode, criteria.ItemType);
                var masterLists = _maintenanceBll.GetInventoryView(dt, criteria.LocationCode, criteria.ItemType, QParam, UserAD);
                var viewModel = Mapper.Map<List<MaintenanceExecutionInventoryFunction_Result>>(masterLists);

                var sortedCollection = criteria.SortOrder == "ASC" ?
                    (from item in viewModel
                     //orderby criteria.SortExpression ascending
                     select item).OrderBy(s => s.GetType().GetProperty(criteria.SortExpression).GetValue(s,null)).ToList() :
                    (from item in viewModel
                     //orderby criteria.SortExpression descending
                     select item).OrderByDescending(s => s.GetType().GetProperty(criteria.SortExpression).GetValue(s,null)).ToList();

                //Func<Item, Object> orderByFunc = null;

                //if(sortOrder == SortOrde)
                //sorting
                //var sortedCollection =
                //   (from item in viewModel
                //    orderby item.ItemCode, item.ItemDescription
                //    select item).ToList();

                var pageResult = new PageResult<MaintenanceExecutionInventoryFunction_Result>(sortedCollection, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Maintenance Execution Inventory - GetMaintenanceExecutionInventory");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string itemType, DateTime? date, string sortExpression, string sortOrder)
        {
            try
            {
                if (string.IsNullOrEmpty(sortExpression))
                {
                    sortExpression = "ItemCode"; //default
                    sortOrder = "DESC";
                }


                var input = new MaintenanceExecutionInventoryViewInput
                {
                    LocationCode = locationCode,
                    ItemType = itemType,
                    Date = date,
                    SortExpression = sortExpression,
                    SortOrder = sortOrder
                };

                UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
                var UserAD = strUserID.Username;
                var QParam = input.LocationCode + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + UserAD;
                DateTime dt = Convert.ToDateTime(input.Date);
                //var masterLists = _maintenanceBll.GetMaintenanceExecutionInventoryView(criteria);
                //var masterLists = _maintenanceBll.GetInventory(criteria.Date.Value.ToString("yyyy-MM-dd"), criteria.LocationCode, criteria.ItemType);
                var inventories = _maintenanceBll.GetInventoryView(dt, input.LocationCode, input.ItemType, QParam, UserAD);
                
                //var inventories = _maintenanceBll.GetMaintenanceExecutionInventoryView(input);

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = HMS.SKTIS.Core.Enums.ExcelTemplate.MaintenanceExecutionInventory + ".xlsx";
                var templateFileName = Server.MapPath(Constants.MaintenanceDataExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == locationCode)
                    {
                        locationCompat = item.Text;
                    }
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, locationCompat);
                    slDoc.SetCellValue(4, 2, itemType);
                    slDoc.SetCellValue(5, 2, date.HasValue ? date.Value.Date.ToShortDateString() : "");
                    //row values
                    var iRow = 9;

                    foreach (var inventory in inventories)
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

                        slDoc.SetCellValue(iRow, 1, inventory.ItemCode);
                        slDoc.SetCellValue(iRow, 2, inventory.ItemDescription);
                        slDoc.SetCellValue(iRow, 3, inventory.StawIT.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 4, inventory.InIT.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 5, inventory.OutIT.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 6, inventory.StackIT.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 7, inventory.StawQI.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 8, inventory.InQI.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 9, inventory.OutQI.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 10, inventory.StackQI.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 11, inventory.StawReady.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 12, inventory.InReady.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 13, inventory.OutReady.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 14, inventory.StackReady.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 15, inventory.StawOU.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 16, inventory.InOU.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 17, inventory.OutOU.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 18, inventory.StackOU.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 19, inventory.StawOR.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 20, inventory.InOR.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 21, inventory.OutOR.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 22, inventory.StackOR.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 23, inventory.StawBS.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 24, inventory.InBS.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 25, inventory.OutBS.GetValueOrDefault());
                        slDoc.SetCellValue(iRow, 26, inventory.StackBS.GetValueOrDefault());
                        slDoc.SetCellStyle(iRow, 1, iRow, 26, style);
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
                    slDoc.AutoFitColumn(1, 25);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "MaintenanceExecutionInventory" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, itemType, date, sortExpression, sortOrder }, "Maintenance Execution Inventory - Generate Excel");
                return null;
            }
        }
    }
}