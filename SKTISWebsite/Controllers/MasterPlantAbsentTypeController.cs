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
using SKTISWebsite.Models.MasterPlantAbsentType;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterPlantAbsentTypeController : BaseController
    {
        private IMasterDataBLL _bll;

        public MasterPlantAbsentTypeController(IMasterDataBLL masterDataBll)
        {
            _bll = masterDataBll;
            SetPage("MasterData/Plant/AbsentType");
        }

        // GET: MasterPlantAbsentType
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetPlantAbsentTypes(GetMstAbsentTypeInput criteria)
        {
            var pageResult = new PageResult<MstPlantAbsentTypeViewModel>();
            var list = _bll.GetMstPlantAbsentTypes(criteria);
            pageResult.TotalRecords = list.Count;
            pageResult.TotalPages = (list.Count / criteria.PageSize) + (list.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = list.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MstPlantAbsentTypeViewModel>>(result);
            return Json(pageResult);
        }

        [HttpPost]
        public ActionResult SaveAllPlantAbsentTypes(InsertUpdateData<MstPlantAbsentTypeViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var plantAbsentTypeDto = Mapper.Map<MstPlantAbsentTypeDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    plantAbsentTypeDto.CreatedBy = GetUserName();
                    plantAbsentTypeDto.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.InsertMstPlantAbsentType(plantAbsentTypeDto);
                        bulkData.New[i] = Mapper.Map<MstPlantAbsentTypeViewModel>(item);
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
                    var plantAbsentTypeDto = Mapper.Map<MstPlantAbsentTypeDTO>(bulkData.Edit[i]);

                    //set updatedby
                    plantAbsentTypeDto.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.UpdateMstPlantAbsentType(plantAbsentTypeDto);
                        bulkData.Edit[i] = Mapper.Map<MstPlantAbsentTypeViewModel>(item);
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
        public FileStreamResult GenerateExcel(string absentType)
        {
            var input = new GetMstAbsentTypeInput { AbsentType = absentType };
            var masterListGroups = _bll.GetMstPlantAbsentTypes(input);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstPlantAbsentType + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
               // slDoc.SetCellValue(2, 2, absentType);
                slDoc.SetCellValue(2, 2, absentType == "" ? "All" : absentType);
                //row values
                var iRow = 6;

                foreach (var item in masterListGroups.Select((value, index) => new { value = value, index = index }))
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

                    slDoc.SetCellValue(iRow, 1, item.value.AbsentType);
                    slDoc.SetCellValue(iRow, 2, item.value.SktAbsentCode);
                    slDoc.SetCellValue(iRow, 3, item.value.PayrollAbsentCode);
                    slDoc.SetCellValue(iRow, 4, item.value.AlphaReplace);
                    slDoc.SetCellValue(iRow, 5, item.value.MaxDay.ToString());
                    slDoc.SetCellValue(iRow, 6, item.value.ActiveInAbsent.ToString());
                    slDoc.SetCellValue(iRow, 7, item.value.ActiveInProductionEntry.ToString());
                    slDoc.SetCellValue(iRow, 8, item.value.Calculation);
                    slDoc.SetCellValue(iRow, 9, item.value.Remark);
                    slDoc.SetCellValue(iRow, 10, item.value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 11, item.value.UpdatedDate.ToString(Constants.DefaultDateFormat));
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
                //slDoc.AutoFitColumn(2, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterPlantAbsentType_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}