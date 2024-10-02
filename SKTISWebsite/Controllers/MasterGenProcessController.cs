using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenProcess;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenProcessController : BaseController
    {
        private IMasterDataBLL _masterDataBLL;

        public MasterGenProcessController(IMasterDataBLL masterDataBll)
        {
            _masterDataBLL = masterDataBll;
            SetPage("MasterData/General/Process");
        }

        // GET: MasterProcess
        public ActionResult Index()
        {
            var style = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            var route = RouteData;
            return View("Index");
        }

        /// <summary>
        /// Get all process list
        /// </summary>
        /// <param name="criteria"> </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetListProcess(GetMstGenProcessesInput criteria)
        {
            var pageResult = new PageResult<MasterGenProcessViewModel>();
            var masterProcess = _masterDataBLL.GetMasterProcesses(criteria);
            pageResult.TotalRecords = masterProcess.Count;
            pageResult.TotalPages = (masterProcess.Count / criteria.PageSize) + (masterProcess.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = masterProcess.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MasterGenProcessViewModel>>(result);
            return Json(pageResult);
        }

        /// <summary>
        /// save all master process
        /// </summary>
        /// <param name="bulkData">List of edited master process</param>
        [HttpPost]
        public ActionResult SaveProcess(InsertUpdateData<MasterGenProcessViewModel> bulkData)
        {
            if (bulkData.New != null)
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var process = Mapper.Map<MstGenProcessDTO>(bulkData.New[i]);

                    // set createdby and updatedby
                    process.CreatedBy = GetUserName();
                    process.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBLL.InsertMasterProcess(process);
                        bulkData.New[i] = Mapper.Map<MasterGenProcessViewModel>(item);
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

                    var process = Mapper.Map<MstGenProcessDTO>(bulkData.Edit[i]);
                    process.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBLL.UpdateMasterProcess(process);

                        bulkData.Edit[i] = Mapper.Map<MasterGenProcessViewModel>(item);
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
        /// Generate excel by filter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel()
        {
            var masterProcess = _masterDataBLL.GetMasterProcesses(new GetMstGenProcessesInput());

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstProcess + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {

                //row values
                var iRow = 4;

                foreach (var process in masterProcess)
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

                    slDoc.SetCellValue(iRow, 1, process.ProcessGroup);
                    if (process.ProcessIdentifier != null) slDoc.SetCellValue(iRow,2, process.ProcessIdentifier);
                    slDoc.SetCellValue(iRow, 3, process.ProcessOrder.ToString());
                    slDoc.SetCellValue(iRow, 4, process.WIP.ToString());
                    slDoc.SetCellValue(iRow, 5, process.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 6, process.Remark);
                    slDoc.SetCellValue(iRow, 7, process.UpdatedBy);
                    slDoc.SetCellValue(iRow, 8, process.UpdatedDate.ToString(Constants.DefaultDateFormat));
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
                slDoc.AutoFitColumn(2, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterProcess_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}