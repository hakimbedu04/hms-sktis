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
using SKTISWebsite.Models.UtilSecurityRules;
using SKTISWebsite.Models.Common;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Utils;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;


namespace SKTISWebsite.Controllers
{
    public class UtilSecurityRulesController : BaseController
    {
        private IApplicationService _svc;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IExeReportBLL _exeReportBLL;

        public UtilSecurityRulesController(IApplicationService applicationService, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBLL, IExeReportBLL exeReportBLL)
        {
            _svc = applicationService;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            _exeReportBLL = exeReportBLL;
            SetPage("Utilities/Security/Rules");
        }

        //
        // GET: /UtilSecurityRules/
        public ActionResult Index()
        {
            var initModel = new InitUtilSecurityRules()
            {
                LocationNameLookupList = _svc.GetLocationNamesLookupList()
            };
            return View("Index", initModel);
        }

        /// <summary>
        /// Get all list of locationcode
        /// </summary>
        /// <returns>list of LocationCode</returns>
        [HttpGet]
        public JsonResult GetLocationCodeSelectList()
        {
            //var model = _svc.GetLocationCodeSelectList();
            var model = _svc.GetLocationCodeCompat();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

         //<summary>
         //Get all roles data by filter
         //</summary>
         //<param name="criteria"></param>
         //<returns>list of master process setting</returns>
        [HttpPost]
        public JsonResult GetRules(GetUtilSecurityRulesViewInput criteria)
        {

            var roles = _utilitiesBLL.GetListRules(criteria);
            var viewModel = Mapper.Map<List<UtilSecurityRulesViewModel>>(roles);
            var pageResult = new PageResult<UtilSecurityRulesViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        //<summary>
        //Get all roles data by filter
        //</summary>
        //<param name="criteria"></param>
        //<returns>list of master process setting</returns>
        [HttpPost]
        public JsonResult GetUnitByRules(GetUtilSecurityRulesViewInput criteria)
        {

            var rules = _utilitiesBLL.GetRuleByID(criteria.IDRule);
            var unit = new List<UtilSecurityRulesUnitViewModel>();

            if (rules != null) {
                if (rules.Unit != null)
                {
                    foreach (var u in rules.Unit.Split(';'))
                    {
                        unit.Add(new UtilSecurityRulesUnitViewModel
                        {
                            IDRule = rules.IDRule,
                            RulesName = rules.RulesName,
                            Location = rules.Location,
                            Unit = u
                        });
                    }
                }
                else
                {
                    unit.Add(new UtilSecurityRulesUnitViewModel
                    {
                        IDRule = rules.IDRule,
                        RulesName = rules.RulesName,
                        Location = rules.Location,
                        Unit = "ALL"
                    });
                }

            }
            

            var viewModel = unit; 
            var pageResult = new PageResult<UtilSecurityRulesUnitViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetUnitCodeSelectListByLocationCode(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Save all new and updated general list item
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllRules(InsertUpdateData<UtilSecurityRulesViewModel> bulkData)
        {
            var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var rolesList = Mapper.Map<UtilRuleDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    rolesList.CreatedBy = GetUserName();
                    rolesList.UpdatedBy = GetUserName();
                    rolesList.Location = locationCode;

                    try
                    {
                        var item = _utilitiesBLL.InsertRule(rolesList);
                        bulkData.New[i] = Mapper.Map<UtilSecurityRulesViewModel>(item);
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
                    var rolesList = Mapper.Map<UtilRuleDTO>(bulkData.Edit[i]);

                    //set updatedby
                    rolesList.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.UpdateRule(rolesList);
                        bulkData.Edit[i] = Mapper.Map<UtilSecurityRulesViewModel>(item);
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

        [HttpPost]
        public ActionResult SaveAllUnit(InsertUpdateData<UtilSecurityRulesViewModel> bulkData)
        {
            var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var rulesName = bulkData.Parameters != null ? bulkData.Parameters["RulesName"] : "";
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var rolesList = Mapper.Map<UtilRuleDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    rolesList.CreatedBy = GetUserName();
                    rolesList.UpdatedBy = GetUserName();
                    rolesList.RulesName = rulesName;
                    rolesList.Location = locationCode;

                    try
                    {
                        var item = _utilitiesBLL.InsertRuleUnit(rolesList);
                        bulkData.New[i] = Mapper.Map<UtilSecurityRulesViewModel>(item);
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
                    var rolesList = Mapper.Map<UtilRuleDTO>(bulkData.Edit[i]);

                    //set updatedby
                    rolesList.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.UpdateRuleUnit(rolesList);
                        bulkData.Edit[i] = Mapper.Map<UtilSecurityRulesViewModel>(item);
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

        public FileStreamResult GenerateExcel(string locationCode)
        {
            var criteria = new GetUtilSecurityRulesViewInput
            {
                Location = locationCode,
                SortExpression = "UpdatedDate",
                SortOrder = "DESC"
            };
            var rules = _utilitiesBLL.GetListRules(criteria);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExecuteExcelTemplate.UtilSecurityRules + ".xlsx";
            var templateFileName = Server.MapPath(Constants.UtilitiesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                slDoc.SetCellValue(3, 2, ": " + locationCode);
                //row values
                var iRow = 6;

                foreach (var rule in rules)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    slDoc.SetCellValue(iRow, 1, rule.IDRule);
                    slDoc.SetCellValue(iRow, 2, rule.RulesName);
                    slDoc.SetCellValue(iRow, 3, rule.Unit);

                    slDoc.SetCellStyle(iRow, 1, iRow, 3, style);
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
            var fileName = "UtilitiesSecurity_Rules" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
	}
}