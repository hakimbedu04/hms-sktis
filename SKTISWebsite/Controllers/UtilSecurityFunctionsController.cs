using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.UtilSecurityFunctions;
using SKTISWebsite.Models.Common;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Utils;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;


namespace SKTISWebsite.Controllers
{
    public class UtilSecurityFunctionsController : BaseController
    {
        private IMasterDataBLL _bll;
        private IApplicationService _svc;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IExeReportBLL _exeReportBLL;

        public UtilSecurityFunctionsController(IMasterDataBLL bll, IApplicationService applicationService, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBLL, IExeReportBLL exeReportBLL)
        {
            _bll = bll;
            _svc = applicationService;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            _exeReportBLL = exeReportBLL;
            SetPage("Utilities/Security/Function");
        }

        //
        // GET: /UtilSecurityFunctions/
        public ActionResult Index()
        {
            return View("Index");
        }

         //<summary>
         //Get all roles data by filter
         //</summary>
         //<param name="criteria"></param>
         //<returns>list of master process setting</returns>
        [HttpPost]
        public JsonResult GetFunctions(BaseInput criteria)
        {
            if (criteria.SortExpression == "ParentNameFunction") {
                criteria.SortExpression = "UtilFunction1.FunctionName";
            }
            var functiosn = _utilitiesBLL.GetListFunctions(criteria);
            var viewModel = Mapper.Map<List<UtilSecurityFunctionsViewModel>>(functiosn);
            var pageResult = new PageResult<UtilSecurityFunctionsViewModel>(viewModel, criteria);
            return Json(pageResult);
        }
        
        /// <summary>
        /// Save all new and updated general list item
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllFunctions(InsertUpdateData<UtilSecurityFunctionsViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var functionList = Mapper.Map<UtilFunctionDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    functionList.CreatedBy = GetUserName();
                    functionList.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.InsertFunction(functionList);
                        bulkData.New[i] = Mapper.Map<UtilSecurityFunctionsViewModel>(item);
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
            }

            // Update data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    var functionList = Mapper.Map<UtilFunctionDTO>(bulkData.Edit[i]);

                    //set updatedby
                    functionList.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.UpdateFunction(functionList);
                        bulkData.Edit[i] = Mapper.Map<UtilSecurityFunctionsViewModel>(item);
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
                        throw ex;
                    }
                }
            }

            return Json(bulkData);
        }
                /// <summary>
        /// Get Active Employee by Location, Unit, Process, and Group for Piece Rate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFunctionType()
        {
            var model = _bll.GetGeneralListFunctionType().OrderBy(t => t.ListDetail);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
                        /// <summary>
        /// Get Active Employee by Location, Unit, Process, and Group for Piece Rate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFunctionsTree()
        {
            var functions = _utilitiesBLL.GetListFunctions(new BaseInput() { SortExpression = "ParentIDFunction", SortOrder = "ASC"});

            return Json(functions, JsonRequestBehavior.AllowGet);
        }

        public FileStreamResult GenerateExcel(BaseInput criteria)
        {
            //if (criteria.SortExpression == "ParentNameFunction")
            //{
            //    criteria.SortExpression = "UtilFunction1.FunctionName";
            //}
            criteria.SortExpression = "UpdatedDate";
            criteria.SortOrder = "DESC";
            var functionList = _utilitiesBLL.GetListFunctions(criteria);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExecuteExcelTemplate.UtilSecurityFunctions + ".xlsx";
            var templateFileName = Server.MapPath(Constants.UtilitiesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //row values
                var iRow = 4;

                foreach (var function in functionList)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    slDoc.SetCellValue(iRow, 1, function.IDFunction);
                    slDoc.SetCellValue(iRow, 2, function.FunctionType);
                    slDoc.SetCellValue(iRow, 3, function.FunctionName);
                    slDoc.SetCellValue(iRow, 4, function.ParentIDFunction.ToString());
                    slDoc.SetCellValue(iRow, 5, function.ParentNameFunction);
                    
                    slDoc.SetCellStyle(iRow, 1, iRow, 5, style);
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
                //slDoc.AutoFitColumn(1, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "UtilitiesSecurity_Function" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        
	}
}