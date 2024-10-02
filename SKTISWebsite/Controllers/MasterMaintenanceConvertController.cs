using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterMaintenanceConvert;
using SKTISWebsite.Models.MasterMntcItemLocation;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterMaintenanceConvertController : BaseController
    {
        private IMasterDataBLL _bll;

        public MasterMaintenanceConvertController(IMasterDataBLL masterDataBll)
        {
            _bll = masterDataBll;
            SetPage("MasterData/Maintenance/ItemConversion");
        }

        // GET: MasterItemConversion
        public ActionResult Index()
        {
            var model = new InitMntcConvert
            {
                ItemDescriptions = Mapper.Map<List<MstMntcItemDescription>>(_bll.GetAllMaintenanceItems())
            };
            return View("Index", model);
        }

        [HttpPost]
        public JsonResult GetMaintenanceConvert(GetMstMntcConvertInput criteria)
        {
            var pageResult = new PageResult<MasterMntcConvertViewModel>();
            var list = _bll.GetMstMntvConverts(criteria);
            foreach (var mstMntcConvertDto in list)
            {
                mstMntcConvertDto.ItemDestinationEquipment = _bll.GetEqupmentItemDestinationDetail(mstMntcConvertDto.ItemCodeSource);
            }
            pageResult.TotalRecords = list.Count;
            pageResult.TotalPages = (list.Count / criteria.PageSize) + (list.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = list.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MasterMntcConvertViewModel>>(result);
            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEqupmentItemDestinationDetail(string sourceItemCode)
        {
            var model = _bll.GetEqupmentItemDestinationDetail(sourceItemCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemCodes(string itemCode, string itemType)
        {
            var model = _bll.GetMstMaintenanceItemNotEqualItemCode(itemCode, itemType);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMstMaintenanceItemNotContainItemType(string itemType)
        {
            var model = _bll.GetMstMaintenanceItemNotContainItemType(itemType);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveAllMstMntcConvert(InsertUpdateData<MasterMntcConvertViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var mstMntcConvertDto = Mapper.Map<MstMntcConvertDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    mstMntcConvertDto.CreatedBy = GetUserName();
                    mstMntcConvertDto.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.InsertMstMntcConvert(mstMntcConvertDto);
                        bulkData.New[i] = Mapper.Map<MasterMntcConvertViewModel>(item);
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
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
                    var mstMntcConvertDto = Mapper.Map<MstMntcConvertDTO>(bulkData.Edit[i]);

                    //set updatedby
                    mstMntcConvertDto.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.UpdateMstMntcConvert(mstMntcConvertDto);
                        bulkData.Edit[i] = Mapper.Map<MasterMntcConvertViewModel>(item);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public ActionResult SaveAllMstMntcConvertEquipment(InsertUpdateData<MasterMntcConvertViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var mstMntcConvertDto = Mapper.Map<MstMntcConvertDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    mstMntcConvertDto.CreatedBy = GetUserName();
                    mstMntcConvertDto.UpdatedBy = GetUserName();

                    foreach (var detailEquipment in bulkData.New[i].ItemDestinationEquipment)
                    {
                        try
                        {
                            if (!detailEquipment.QtyConvert.HasValue) continue;
                            mstMntcConvertDto.ItemCodeDestination = detailEquipment.ItemCode;
                            mstMntcConvertDto.QtyConvert = detailEquipment.QtyConvert;

                            var item = _bll.SaveMstMntcConvertEquipmentDetails(mstMntcConvertDto);
                            bulkData.New[i] = Mapper.Map<MasterMntcConvertViewModel>(item);
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                        }
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
                    var mstMntcConvertDto = Mapper.Map<MstMntcConvertDTO>(bulkData.Edit[i]);

                    //set createdby and updatedby
                    mstMntcConvertDto.CreatedBy = GetUserName();
                    mstMntcConvertDto.UpdatedBy = GetUserName();
                    try
                    {
                        if (bulkData.Edit[i].ItemDestinationEquipment != null)
                        {
                            foreach (var detailEquipment in bulkData.Edit[i].ItemDestinationEquipment)
                            {
                                mstMntcConvertDto.ItemCodeDestination = detailEquipment.ItemCode;
                                mstMntcConvertDto.QtyConvert = detailEquipment.QtyConvert;

                                var item = _bll.SaveMstMntcConvertEquipmentDetails(mstMntcConvertDto);
                                bulkData.Edit[i] = Mapper.Map<MasterMntcConvertViewModel>(item);
                                bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                            }
                        }
                        else
                        {
                            var item = _bll.SaveMstMntcConvertEquipment(mstMntcConvertDto);
                            bulkData.Edit[i] = Mapper.Map<MasterMntcConvertViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(bool conversionType)
        {
            var mntcConvertDtos = _bll.GetMstMntvConvertsForExcel(conversionType);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstMntcConvertSts + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //row values
                var iRow = 5;
                slDoc.SetCellValue(1, 1,
                    conversionType
                        ? "Master Maintenance Convert - Equipment to Sparepart"
                        : "Master Maintenance Convert - Sparepart to Sparepart");

                foreach (var item in mntcConvertDtos.Select((value, index) => new { value = value, index = index }))
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (item.index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, item.value.ItemCodeSource);
                    slDoc.SetCellValue(iRow, 2, item.value.ItemCodeSourceDescription);
                    slDoc.SetCellValue(iRow, 3, item.value.ItemCodeDestination);
                    slDoc.SetCellValue(iRow, 4, item.value.ItemCodeDestinationDescription);
                    slDoc.SetCellValue(iRow, 5, item.value.QtyConvert.ToString());
                    slDoc.SetCellValue(iRow, 6, item.value.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 7, item.value.Remark);
                    slDoc.SetCellValue(iRow, 8, item.value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 9, item.value.UpdatedDate.ToString());
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
                //slDoc.AutoFitColumn(2, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterMaintenanceConvert_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}