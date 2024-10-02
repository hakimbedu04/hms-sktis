using System;
using System.Collections.Generic;
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
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterMntcItemLocation;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Code;

namespace SKTISWebsite.Controllers
{
    public class MasterMntcItemLocationController : BaseController
    {
        private IMasterDataBLL _masterDataBLL;
        private IApplicationService _svc;

        public MasterMntcItemLocationController(IMasterDataBLL masterDataBLL, IApplicationService svc)
        {
            _masterDataBLL = masterDataBLL;
            _svc = svc;
            SetPage("MasterData/Maintenance/ItemLocation");
        }

        // GET: MasterItemLocation
        public ActionResult Index()
        {
            var maintenanitems = _masterDataBLL.GetAllMaintenanceItems();
            var location = _masterDataBLL.GetLocationInfo(Enums.LocationCode.SKT.ToString()) ?? new List<LocationInfoDTO>();
            var initItemLocation = new InitItemLocationViewModel
                                   {
                                       Item = Mapper.Map<List<SelectListItem>>(maintenanitems),
                                       Location = Mapper.Map<List<SelectListItem>>(location),
                                       ItemDescription = Mapper.Map<List<MstMntcItemDescription>>(maintenanitems),
                                       LocationNameLookupList = _svc.GetLocationNamesLookupList()
                                   };
            return View("Index", initItemLocation);
        }

        [HttpPost]
        public ActionResult GetMstItemLocation(MstMntcItemLocationInput criteria)
        {
            var pageResult = new PageResult<MstMntcItemLocationViewModel>();
            var itemLocations = _masterDataBLL.GetMstItemLocations(criteria);

            Func<MstMntcItemLocationDTO, Object> orderByFunc = null;

            switch (criteria.SortExpression)
            {
                case "ItemCode":
                    orderByFunc = i => i.ItemCode;
                    break;
                case "ItemDescription":
                    orderByFunc = i => i.ItemDescription;
                    break;
                case "BufferStock":
                    orderByFunc = i => i.BufferStock;
                    break;
                case "MinOrder":
                    orderByFunc = i => i.MinOrder;
                    break;
                case "StockReadyToUse":
                    orderByFunc = i => i.StockReadyToUse;
                    break;
                case "StockAll":
                    orderByFunc = i => i.StockAll;
                    break;
                default:
                    orderByFunc = i => i.UpdatedDate;
                    break;
            }

            if (criteria.SortOrder == "DESC")
                itemLocations = itemLocations.OrderByDescending(orderByFunc).ToList();
            else
                itemLocations = itemLocations.OrderBy(orderByFunc).ToList();

            pageResult.TotalRecords = itemLocations.Count;
            pageResult.TotalPages = (itemLocations.Count / criteria.PageSize) + (itemLocations.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = itemLocations.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MstMntcItemLocationViewModel>>(result);
            return Json(pageResult);
        }

        [HttpPost]
        public ActionResult SaveItemLocation(InsertUpdateData<MstMntcItemLocationViewModel> bulkData)
        {
            var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            if (bulkData.New != null)
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var itemlocation = Mapper.Map<MstMntcItemLocationDTO>(bulkData.New[i]);

                    // set createdby and updatedby
                    itemlocation.CreatedBy = GetUserName();
                    itemlocation.UpdatedBy = GetUserName();

                    // set location code
                    itemlocation.LocationCode = locationCode;

                    try
                    {
                        var item = _masterDataBLL.InsertItemLocation(itemlocation);
                        bulkData.New[i] = Mapper.Map<MstMntcItemLocationViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                    }
                }

            // Update data
            if (bulkData.Edit != null)
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var itemLocation = Mapper.Map<MstMntcItemLocationDTO>(bulkData.Edit[i]);
                    itemLocation.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBLL.UpdateItemLocation(itemLocation);

                        bulkData.Edit[i] = Mapper.Map<MstMntcItemLocationViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                    }
                }

            return Json(bulkData);
        }
        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode)
        {
            var input = new MstMntcItemLocationInput { LocationCode = locationCode };
            var masterItemLocation = _masterDataBLL.GetMstItemLocations(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstItemLocation + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (var slDoc = new SLDocument(strFileName))
            {
                //filter values
                var locationName = _masterDataBLL.GetMstLocationById(locationCode);
                if (locationName != null)
                    slDoc.SetCellValue(3, 2, locationCode + " - " + locationName.LocationName);

                //row values
                var iRow = 7;

                foreach (var masterItem in masterItemLocation.Select((value, index) => new { Value = value, Index = index }))
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (masterItem.Index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, masterItem.Value.ItemCode);
                    slDoc.SetCellValue(iRow, 2, masterItem.Value.ItemDescription);
                    slDoc.SetCellValue(iRow, 3, masterItem.Value.BufferStock);
                    slDoc.SetCellValue(iRow, 4, masterItem.Value.MinOrder);
                    slDoc.SetCellValue(iRow, 5, masterItem.Value.StockReadyToUse);
                    slDoc.SetCellValue(iRow, 6, masterItem.Value.StockAll);
                    slDoc.SetCellValue(iRow, 7, masterItem.Value.Remark);
                    slDoc.SetCellValue(iRow, 8, masterItem.Value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 9, masterItem.Value.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
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
                slDoc.AutoFitColumn(1, 9);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterItemLocation_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}