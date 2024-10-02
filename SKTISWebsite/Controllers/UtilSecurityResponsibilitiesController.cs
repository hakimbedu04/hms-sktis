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
using SKTISWebsite.Models.UtilSecurityResponsibilities;
using SKTISWebsite.Models.UtilSecurityRules;
using SKTISWebsite.Models.UtilSecurityRoles;
using SKTISWebsite.Models.Common;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Utils;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class UtilSecurityResponsibilitiesController : BaseController
    {
        private IMasterDataBLL _bll;
        private IApplicationService _svc;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IExeReportBLL _exeReportBLL;
        private IUserBLL _userBLL;
        private IVTLogger _vtlogger;

        public UtilSecurityResponsibilitiesController(IMasterDataBLL bll, IVTLogger vtlogger, IApplicationService applicationService, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBLL, IExeReportBLL exeReportBLL, IUserBLL userBLL)
        {
            _bll = bll;
            _svc = applicationService;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            _exeReportBLL = exeReportBLL;
            _userBLL = userBLL;
            _svc = applicationService;
            _vtlogger = vtlogger;
            SetPage("Utilities/Security/Responsibilities");
        }

        public ActionResult Index()
        {
            var model = new InitUtilSecurityResponsibilities();
            model.ListAllRules = new List<UtilSecurityRulesViewModel>();
            model.ListActiveRules = new List<UtilSecurityRulesViewModel>();
            model.ListAllResponsibilities = new List<UtilSecurityResponsibilitiesViewModel>();
            model.ListActiveResponsibilities = new List<UtilSecurityResponsibilitiesViewModel>();
            model.UtilRoles = _svc.GetUtilRolesSelectList();
            return View("index", model);
        }
        /// <summary>
        /// Get all responsibilities data by filter
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>list of master responsibilities</returns>
        [HttpPost]
        public JsonResult GetResponsibility(UtilSecurityResponsibilitiesInput criteria)
        {

            var respons = _utilitiesBLL.GetListResponsibilities(criteria);
            var viewModel = Mapper.Map<List<UtilSecurityResponsibilitiesViewModel>>(respons);
            var pageResult = new PageResult<UtilSecurityResponsibilitiesViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public JsonResult GetUserByResponsibility(UtilSecurityResponsibilitiesInput criteria)
        {
            var list = _utilitiesBLL.GetListUserResponsibilitesByResponsibility(criteria);
            var viewModel = Mapper.Map<List<UtilUsersResponsibilitiesRoleViewModel>>(list);
            var pageResult = new PageResult<UtilUsersResponsibilitiesRoleViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetResponsibilitiesSelectList()
        {
            var criteria = new UtilSecurityResponsibilitiesInput();
            criteria.SortOrder = "ASC";
            criteria.SortExpression = "ResponsibilityName";
            var respons = _utilitiesBLL.GetListResponsibilities(criteria);

            var model =  new SelectList(respons, "IDResponsibility", "ResponsibilityName");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveRoles(InsertUpdateData<UtilSecurityResponsibilitiesViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                try
                {
                    foreach (var item in bulkData.New)
                    {
                        var criteria = new UtilSecurityResponsibilitiesInput();
                        criteria.IDRole = item.IDRole;
                        criteria.ResponsibilityName = item.ResponsibilityName;
                        var check = _utilitiesBLL.GetListResponsibilities(criteria);
                        if (check.Count() > 0)
                        {
                            return Json(new
                            {
                                Error = 1,
                                Message = "Failed to insert Responsibility. Responsibility Name and Role already exist"
                            });
                        }
                        else
                        {
                            var transactionDate = DateTime.Now;

                            var utilResponsibility = new UtilResponsibilityDTO();
                            utilResponsibility.IDResponsibility = _utilitiesBLL.GetNewIDResponsibility();
                            utilResponsibility.IDRole = item.IDRole;
                            utilResponsibility.ResponsibilityName = item.ResponsibilityName;
                            utilResponsibility.CreatedDate = transactionDate;
                            utilResponsibility.UpdatedDate = transactionDate;
                            utilResponsibility.CreatedBy = GetUserName();
                            utilResponsibility.UpdatedBy = GetUserName();

                            _utilitiesBLL.SaveRoleResponsibility(utilResponsibility);
                        }
                        
                    }
                    return Json(new
                    {
                        Error = 0,
                        Message = "Save Responsibility Success."
                    });
                }
                catch (Exception e)
                {
                    _vtlogger.Err(e, new List<object> { bulkData.New }, "Failed to insert Responsibility.");
                    return Json(new
                    {
                        Error = 1,
                        Message = "Failed to insert Responsibility."
                    });
                }
                
            }

            if (bulkData.Edit != null)
            {
                try
                {
                    foreach (var item in bulkData.Edit)
                    {
                        // 2 am, fix later
                        if (item == null) continue;
                        var criteria = new UtilSecurityResponsibilitiesInput();
                        criteria.IDRole = item.IDRole;
                        criteria.ResponsibilityName = item.ResponsibilityName;
                        var check = _utilitiesBLL.GetListResponsibilities(criteria);
                        if (check.Count() > 0)
                        {
                            return Json(new
                            {
                                Error = 1,
                                Message = "Failed to update Responsibility. Duplicate Responsibility Name and Role"
                            });
                        }
                        else
                        {
                            var transactionDate = DateTime.Now;

                            var utilResponsibility = new UtilResponsibilityDTO();
                            utilResponsibility.IDResponsibility = item.IDResponsibility;
                            utilResponsibility.IDRole = item.IDRole;
                            utilResponsibility.ResponsibilityName = item.ResponsibilityName;
                            utilResponsibility.UpdatedDate = transactionDate;
                            utilResponsibility.UpdatedBy = GetUserName();

                            _utilitiesBLL.UpdateRoleResponsibility(utilResponsibility);
                        }
                        
                    }
                    return Json(new
                    {
                        Error = 0,
                        Message = "Save Responsibility Success."
                    });
                }
                catch (Exception e)
                {
                    _vtlogger.Err(e, new List<object> { bulkData.Edit }, "Failed to insert Responsibility.");
                    return Json(new {
                        Error = 1,
                        Message = "Failed to update Responsibility."
                    });
                }
            }

            //return Json(new
            //{
            //    Error = 0,
            //    Message = ""
            //});
            return null;
            
        }

        [HttpGet]
        public JsonResult GetUsersSelectList()
        {
            var user = _userBLL.GetUsersActive();
            //var model = new SelectList(user, "UserAD", "UserAD");
            var userAd = user.Select(s => new
            {
                s.UserAD,
                s.Name
            }).ToList();
            return Json(userAd, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetRulesByResponsibility(int IDResponsibility)
        {
            var model = new InitUtilSecurityResponsibilities();
            var criteria = new GetUtilSecurityRulesViewInput();
            criteria.SortExpression = "RulesName";
            criteria.SortOrder = "ASC";

            var activeRules = _utilitiesBLL.GetListRulesByResponsibility(IDResponsibility);
            var allRules = _utilitiesBLL.GetListRules(criteria);

            model.ListActiveRules = Mapper.Map<List<UtilSecurityRulesViewModel>>(activeRules);

            var idRules = activeRules.Select(s => s.IDRule).ToList();
            model.ListAllRules = Mapper.Map<List<UtilSecurityRulesViewModel>>(allRules.Where(w => !idRules.Contains(w.IDRule)).ToList());

            return PartialView("_TabRulesPartial", model);
        }
        public PartialViewResult GetResponsibilitiesByUser(string UserAD)
        {
            var model = new InitUtilSecurityResponsibilities();
            var criteria = new UtilSecurityResponsibilitiesInput();
            criteria.SortExpression = "ResponsibilityName";
            criteria.SortOrder = "ASC";
            var activeRespons = _utilitiesBLL.GetListResponsibility2(UserAD);
            var allRespons = _utilitiesBLL.GetListResponsibilities(criteria);
            if (activeRespons.Count > 0)
            {
                var respons = _utilitiesBLL.GetUsersResponsibility(activeRespons[0].IDResponsibility, UserAD);
                ViewBag.EffectiveDate = respons.EffectiveDate;
                ViewBag.ExpiredDate = respons.ExpiredDate;
            }
            else {
                ViewBag.EffectiveDate = DateTime.Now;
                ViewBag.ExpiredDate = DateTime.Now.AddYears(1);
            }
            
            model.ListActiveResponsibilities = Mapper.Map<List<UtilSecurityResponsibilitiesViewModel>>(activeRespons);

            var idRespons = activeRespons.Select(s => s.IDResponsibility).ToList();
            model.ListAllResponsibilities = Mapper.Map<List<UtilSecurityResponsibilitiesViewModel>>(allRespons.Where(w => !idRespons.Contains(w.IDResponsibility)).ToList());


            return PartialView("_TabUsersPartial", model);
        }        
        
        /// <summary>
        /// Save Plant Group Shift
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveRulesResponsibilities(List<UtilSecurityRulesViewModel> bulkData)
        {
            if(bulkData.Count > 0)
            {
                _utilitiesBLL.DeleteAllRulesResponsibilities(bulkData[0].IDResponsibility);

                for (var i = 0; i < bulkData.Count; i++)
                {
                    var groupShift = Mapper.Map<UtilRuleDTO>(bulkData[i]);                    
                    try
                    {
                        groupShift.CreatedBy = CurrentUser.Name;
                        groupShift.UpdatedBy = CurrentUser.Name;
                        var item = _utilitiesBLL.SaveRulesResponsibilities(groupShift, bulkData[i].IDResponsibility);
                        bulkData[i] = Mapper.Map<UtilSecurityRulesViewModel>(item);
                        bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtlogger.Err(ex, new List<object> { groupShift }, "Failed save roles Responsibility.");
                        bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> { groupShift }, "Failed save roles Responsibility.");
                        bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            }            
            return Json(bulkData);
        }


        /// <summary>
        /// Save Plant Group Shift
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveUsersResponsibilities(List<UtilSecurityUsersResponsibilitiesViewModel> bulkData, string UserAD)
        {
            if (UserAD != null)
            {
                _utilitiesBLL.DeleteAllUsersResponsibilities(UserAD);
                if (bulkData != null)
                {
                    for (var i = 0; i < bulkData.Count; i++)
                    {
                        var respons = Mapper.Map<UtilUsersResponsibilityDTO>(bulkData[i]);
                        try
                        {
                            respons.CreatedBy = CurrentUser.Name;
                            respons.UpdatedBy = CurrentUser.Name;
                            var item = _utilitiesBLL.SaveUsersResponsibilities(respons);
                            bulkData[i] = Mapper.Map<UtilSecurityUsersResponsibilitiesViewModel>(item);
                            bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { respons }, "Failed save users Responsibility.");
                            bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { respons }, "Failed save users Responsibility.");
                            bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                        }
                    }
                }
                else {
                    var list = new List<UtilSecurityUsersResponsibilitiesViewModel>();
                    list.Add(new UtilSecurityUsersResponsibilitiesViewModel { Message = null, ResponseType = "Success" });
                    return Json(list);
                }
            }
            return Json(bulkData);
        }

        /// <summary>
        /// Save Plant Group Shift
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateUsersAd(InsertUpdateData<UtilUsersResponsibilitiesRoleViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                try
                {
                    for (var i = 0; i < bulkData.New.Count; i++)
                    {
                        if (bulkData.New[i] == null) continue;

                        DateTime dtAwal = DateTime.Parse(bulkData.New[i].EffectiveDate);
                        DateTime dtAkhir = DateTime.Parse(bulkData.New[i].ExpiredDate);
                        int result = DateTime.Compare(dtAwal, dtAkhir);
                        if (result > 0)
                        {
                            return Json(new
                            {
                                Error = 1,
                                Message = "Failed to Insert user. Please check Effective date and Expired date "
                            });
                        }
                        else
                        {
                            var respons = Mapper.Map<UtilUsersResponsibilityDTO>(bulkData.New[i]);

                            respons.CreatedBy = CurrentUser.Name;
                            respons.UpdatedBy = CurrentUser.Name;
                            respons.UpdatedDate = DateTime.Now;
                            respons.CreatedDate = DateTime.Now;
                            var item = _utilitiesBLL.SaveUsersResponsibilities(respons);
                        }
                    }
                    return Json(new
                    {
                        Error = 0,
                        Message = "Insert user responsibility success."
                    });
                }
                catch (Exception e)
                {
                    _vtlogger.Err(e, new List<object> { bulkData.New }, "Failed to Insert user.");
                    return Json(new
                    {
                        Error = 1,
                        Message = "Failed to insert user."
                    });
                }

            }

            if (bulkData.Edit != null)
            {
                try
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        if (bulkData.Edit[i] == null) continue;

                        DateTime dtAwal = DateTime.Parse(bulkData.Edit[i].EffectiveDate);
                        DateTime dtAkhir = DateTime.Parse(bulkData.Edit[i].ExpiredDate);
                        int result = DateTime.Compare(dtAwal, dtAkhir);
                        if (result>0)
                        {
                            return Json(new
                            {
                                Error = 1,
                                Message = "Failed to update user. Please check Effective date dan Expired date "
                            });
                        }
                        else
                        {
                            var respons = Mapper.Map<UtilUsersResponsibilityDTO>(bulkData.Edit[i]);

                            respons.UpdatedBy = CurrentUser.Name;
                            var item = _utilitiesBLL.UpdateUsersAd(respons);
                        }
                    }
                    return Json(new
                    {
                        Error = 0,
                        Message = "Update user responsibility success."
                    });
                }
                catch (Exception e)
                {
                    _vtlogger.Err(e, new List<object> { bulkData.Edit }, "Failed to update user.");
                    return Json(new
                    {
                        Error = 1,
                        Message = "Failed to update user."
                    });
                }
                
            }

            return null;
        }

        [HttpGet]
        public JsonResult GetRolesList()
        {
            var criteria = new BaseInput();
            criteria.SortExpression = "IDRole";
            criteria.SortOrder = "ASC";
            
            var roles = _utilitiesBLL.GetListRoles(criteria);
            var viewModel = Mapper.Map<List<UtilSecurityRolesViewModel>>(roles);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public FileStreamResult GenerateExcel()
        {

            var criteria = new UtilSecurityResponsibilitiesInput
            {
                SortExpression = "IDResponsibility",
                SortOrder = "ASC"
            };

            var respons = _utilitiesBLL.GetListResponsibilities(criteria);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExecuteExcelTemplate.UtilSecurityResponsibilities + ".xlsx";
            var templateFileName = Server.MapPath(Constants.UtilitiesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                slDoc.SetCellValue(3, 2, ": " + "All");
                //row values
                var iRow = 6;

                foreach (var rule in respons)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    slDoc.SetCellValue(iRow, 1, rule.IDResponsibility);
                    slDoc.SetCellValue(iRow, 2, rule.ResponsibilityName);
                    slDoc.SetCellValue(iRow, 3, rule.RolesName);

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
                slDoc.AutoFitColumn(1, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "UtilitiesSecurity_Responsibilities" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

	}
}