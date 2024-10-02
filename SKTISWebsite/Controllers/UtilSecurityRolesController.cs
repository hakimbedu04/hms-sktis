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
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.UtilSecurityRoles;
using SKTISWebsite.Models.Common;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using HMS.SKTIS.BusinessObjects.Inputs.Utils;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;


namespace SKTISWebsite.Controllers
{
    public class UtilSecurityRolesController : BaseController
    {
        private IApplicationService _svc;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IExeReportBLL _exeReportBLL;

        public UtilSecurityRolesController(IApplicationService applicationService, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBLL, IExeReportBLL exeReportBLL)
        {
            _svc = applicationService;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            _exeReportBLL = exeReportBLL;
            SetPage("Utilities/Security/Roles");
        }

        //
        // GET: /UtilSecurityRoles/
        public ActionResult Index()
        {
            var initModel = new InitUtilSecurityRoles()
            {
                RolesList = _svc.GetRolesSelectList()
            };
            return View("Index", initModel);
        }

         /// <summary>
        /// Get all roles data by filter
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>list of master process setting</returns>
        [HttpPost]
        public JsonResult GetRoles(BaseInput criteria)
        {

            var roles = _utilitiesBLL.GetListRoles(criteria);
            var viewModel = Mapper.Map<List<UtilSecurityRolesViewModel>>(roles);
            var pageResult = new PageResult<UtilSecurityRolesViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        /// <summary>
        /// Get Active Employee by Location, Unit, Process, and Group for Piece Rate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFunctionsTree()
        {
            var functions = _utilitiesBLL.GetListFunctions(new BaseInput() { SortExpression = "ParentIDFunction", SortOrder = "ASC" });

            return Json(functions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveRolesFunctions(List<UtilRolesFunctionDTO> input)
        {
            try
            {
                _utilitiesBLL.UpdateRolesFunction(input, GetUserName());
                return Json(input, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

            /// <summary>
        /// Get Active Employee by Location, Unit, Process, and Group for Piece Rate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRolesFunctions(int IDRole)
        {
            var functions = _utilitiesBLL.GetListRolesFunctionByRoles(IDRole);

            return Json(functions, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Save all new and updated general list item
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllRoles(InsertUpdateData<UtilSecurityRolesViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var rolesList = Mapper.Map<UtilRoleDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    rolesList.CreatedBy = GetUserName();
                    rolesList.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.InsertRole(rolesList);
                        bulkData.New[i] = Mapper.Map<UtilSecurityRolesViewModel>(item);
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
                    var rolesList = Mapper.Map<UtilRoleDTO>(bulkData.Edit[i]);

                    //set updatedby
                    rolesList.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _utilitiesBLL.UpdateRole(rolesList);
                        bulkData.Edit[i] = Mapper.Map<UtilSecurityRolesViewModel>(item);
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
	}
}