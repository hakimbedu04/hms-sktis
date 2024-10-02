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
using SKTISWebsite.Helper;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.Factories.Contract;
using SKTISWebsite.Models.MasterGenProcess;
using SKTISWebsite.Models.MasterGenStandardHour;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenStandardHourController : BaseController
    {
        private AbstractSelectList _abstractSelectList;
        private IMasterDataBLL _masterDataBll;

        public MasterGenStandardHourController(AbstractSelectList abstractSelectList, IMasterDataBLL masterDataBll)
        {
            _abstractSelectList = abstractSelectList;
            _masterDataBll = masterDataBll;
            SetPage("MasterData/General/StandardHours");
        }

        // GET: MasterStandardHour
        public ActionResult Index()
        {
            string[] parameters = 
            { 
                Enums.MasterGeneralList.Day.ToString(),
                Enums.MasterGeneralList.DayType.ToString()
            };
            _abstractSelectList._DataSource = _masterDataBll.GetGeneralLists(parameters);

            var model = new IndexMasterStandardHourViewModel();
            model.DayType = _abstractSelectList.CreateDayType();
            model.Day = _abstractSelectList.CreateDay();
            return View("Index", model);
        }

        /// <summary>
        /// Get StandardHours List
        /// </summary>
        /// <param name="input">BaseInput Object</param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult Get(BaseInput input)
        {
            var pageResult = new PageResult<MasterGenStandardHourViewModel>();
            var standardHourList = _masterDataBll.GetStandardHours(input);
            pageResult.TotalRecords = standardHourList.Count;
            pageResult.TotalPages = (standardHourList.Count / input.PageSize) +
                                    (standardHourList.Count % input.PageSize != 0 ? 1 : 0);
            var result = standardHourList.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            pageResult.Results = Mapper.Map<List<MasterGenStandardHourViewModel>>(result);

            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Insert & Update StandardHour
        /// </summary>
        /// <param name="bulkData"></param>
        [HttpPost]
        public ActionResult Save(InsertUpdateData<MasterGenStandardHourViewModel> bulkData)
        {
            if (bulkData.New != null)
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var standardHour = Mapper.Map<MstGenStandardHourDTO>(bulkData.New[i]);

                    // set createdby and updatedby
                    standardHour.CreatedBy = GetUserName();
                    standardHour.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBll.InsertStandardHour(standardHour);
                        bulkData.New[i] = Mapper.Map<MasterGenStandardHourViewModel>(item);
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
                        bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }

            // Update data
            if (bulkData.Edit != null)
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var standardHour = Mapper.Map<MstGenStandardHourDTO>(bulkData.Edit[i]);
                    standardHour.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBll.UpdateStandardHour(standardHour);

                        bulkData.Edit[i] = Mapper.Map<MasterGenStandardHourViewModel>(item);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }

                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            return Json(bulkData);

        }

        /// <summary>
        /// Generate Excel
        /// </summary>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        public FileStreamResult GenerateExcel()
        {
            var criteria = new BaseInput();
            var standardHours = _masterDataBll.GetStandardHours(criteria);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstStandardHour + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }


            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //row values
                var iRow = 4;

                foreach (var item in standardHours.Select((value, index) => new { Value = value, Index = index }))
                {
                    //get default style
                    var style = ExcelHelper.GetDefaultExcelStyle(slDoc);
                    if (item.Index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }
                    slDoc.SetCellValue(iRow, 1, item.Value.DayType);
                    slDoc.SetCellValue(iRow, 2, item.Value.DayName);
                    slDoc.SetCellValue(iRow, 3, item.Value.JknHour.ToString());
                    slDoc.SetCellValue(iRow, 4, item.Value.Jl1Hour.ToString());
                    slDoc.SetCellValue(iRow, 5, item.Value.Jl2Hour.ToString());
                    slDoc.SetCellValue(iRow, 6, item.Value.Jl3Hour.ToString());
                    slDoc.SetCellValue(iRow, 7, item.Value.Jl4Hour.ToString());
                    slDoc.SetCellValue(iRow, 8, item.Value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 9, item.Value.UpdatedDate.ToString(Constants.DefaultDateFormat));

                    //set style
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
            var fileName = "MasterStandardHour_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}