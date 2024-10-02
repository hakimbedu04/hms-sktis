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
using SKTISWebsite.Models.MasterMntcItem;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterMntcItemController : BaseController
    {
        private IMasterDataBLL _masterDataBLL;

        public MasterMntcItemController(IMasterDataBLL masterDataBLL)
        {
            _masterDataBLL = masterDataBLL;
            SetPage("MasterData/Maintenance/Item");
        }


        // GET: MasterItem
        public ActionResult Index()
        {
            var initMaintenanceItem = new InitMntcItem
                                      {
                                          UOMs = Mapper.Map<List<SelectListItem>>(_masterDataBLL.GetGeneralListUOM()),
                                          ItemTypes = Mapper.Map<List<SelectListItem>>(_masterDataBLL.GetGeneralListsItemType()),
                                          PriceTypes = Mapper.Map<List<SelectListItem>>(_masterDataBLL.GetGeneralListPriceType())
                                      };
            return View("Index", initMaintenanceItem);
        }

        /// <summary>
        /// Get all master maintenaance item
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMstMaintenanceItems(MstMntcItemInput criteria)
        {
            var pageResult = new PageResult<MasterMntcItemViewModel>();
            var maintenanItems = _masterDataBLL.GetMstMaintenanceItems(criteria);
            pageResult.TotalRecords = maintenanItems.Count;
            pageResult.TotalPages = (maintenanItems.Count / criteria.PageSize) + (maintenanItems.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = maintenanItems.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MasterMntcItemViewModel>>(result);
            return Json(pageResult);
        }


        [HttpPost]
        public ActionResult SaveMaintenanceItems(InsertUpdateData<MasterMntcItemViewModel> bulkData)
        {
            if (bulkData.New != null)
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var maintenanceItem = Mapper.Map<MstMntcItemCompositeDTO>(bulkData.New[i]);

                    // set createdby and updatedby
                    maintenanceItem.CreatedBy = GetUserName();
                    maintenanceItem.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBLL.InsertMaintenanceItem(maintenanceItem);
                        bulkData.New[i] = Mapper.Map<MasterMntcItemViewModel>(item);
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

                    var maintenanceItem = Mapper.Map<MstMntcItemCompositeDTO>(bulkData.Edit[i]);
                    maintenanceItem.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBLL.UpdateMaintenanceItem(maintenanceItem);

                        bulkData.Edit[i] = Mapper.Map<MasterMntcItemViewModel>(item);
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
        /// <param name="itemType"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string itemType)
        {
            var input = new MstMntcItemInput { ItemType = itemType };
            var masteritem = _masterDataBLL.GetMstMaintenanceItems(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstMaintenanceItem + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (var slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(4, 2, itemType == "" ? "All" : itemType);

                //row values
                var iRow = 7;

                foreach (var masterItem in masteritem)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (iRow % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, masterItem.ItemCode);
                    slDoc.SetCellValue(iRow, 2, masterItem.ItemDescription);
                    slDoc.SetCellValue(iRow, 3, masterItem.ItemType);
                    slDoc.SetCellValue(iRow, 4, masterItem.UOM);
                    slDoc.SetCellValue(iRow, 5, masterItem.PriceType);
                    slDoc.SetCellValue(iRow, 6, masterItem.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 7, masterItem.Remark);
                    slDoc.SetCellValue(iRow, 8, masterItem.UpdatedBy);
                    slDoc.SetCellValue(iRow, 9, Convert.ToString(masterItem.UpdatedDate));
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
            var fileName = "MasterMaintenanceItem_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}