using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenClosingPayroll;
using SKTISWebsite.Models.MasterGenWeek;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenClosingPayrollController : BaseController
    {
        private IMasterDataBLL _masterDataBll;
        private IApplicationService _applicationService;

        public MasterGenClosingPayrollController(IMasterDataBLL masterDataBll, IApplicationService applicationService)
        {
            _masterDataBll = masterDataBll;
            _applicationService = applicationService;
            SetPage("MasterData/Plant/ClosingPayroll");
        }

        // GET: MasterGenClosingPayroll
        public ActionResult Index()
        {
            //var model = new InitMasterGenClosingPayroll
            //{
            //    YearSelectList = _applicationService.GetYearClosingPayroll()
            //};

            return View("Index");
        }

        [HttpGet]
        public JsonResult GetYearSelectList()
        {
            var result = _applicationService.GetYearClosingPayroll();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Get(GetMstClosingPayrollInput criteria)
        {
            var pageResult = new PageResult<GetMasterGenClosingPayrollViewModel>();
            var masterLists = _masterDataBll.GetMasterClosingPayrolls(criteria);
            pageResult.TotalRecords = masterLists.Count;
            pageResult.TotalPages = (masterLists.Count / criteria.PageSize) +
                                    (masterLists.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = masterLists.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<GetMasterGenClosingPayrollViewModel>>(result);

            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveMasterGenClosingPayroll(MasterGenClosingPayrollViewModel bulkData)
        {
            var data = Mapper.Map<MstClosingPayrollDTO>(bulkData);

            // set createdby and updatedby
            data.CreatedBy = GetUserName();
            data.UpdatedBy = GetUserName();

            var weekData = _masterDataBll.GetWeekByDate(data.ClosingDate);
            
            GetMstClosingPayrollInput criteria = new GetMstClosingPayrollInput();
            criteria.StartDate = weekData.StartDate;
            criteria.EndDate = weekData.EndDate;
            criteria.Year = weekData.Year;
            var existData = _masterDataBll.GetMasterClosingPayroll(criteria);

            try
            {
                if (data.ClosingDate == new DateTime(0001, 01, 01))
                {
                    bulkData.ResponseType = Enums.ResponseType.Error.ToString();
                    bulkData.Message = "Date cannot be empty";
                }
                else if(existData != null)
                {
                    bulkData.ResponseType = Enums.ResponseType.Error.ToString();
                    bulkData.Message = "Closing Date already exist in this week";
                }
                else
                {
                    _masterDataBll.SaveMasterClosingPayroll(data);
                    bulkData.ResponseType = Enums.ResponseType.Success.ToString();
                    bulkData.Message = "Closing Payroll - Insert " + bulkData.ClosingDate.ToShortDateString() + " is successful";
                }

            }
            catch (ExceptionBase ex)
            {
                bulkData.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                bulkData.Message = "Closing Payroll - Insert " + bulkData.ClosingDate.ToShortDateString() + " " + ex.Message;
            }
            catch (Exception ex)
            {
                bulkData.ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                bulkData.Message = "Closing Payroll - Insert " + bulkData.ClosingDate.ToShortDateString() + " " + ex.Message;
            }

            return Json(bulkData);
        }

        [HttpPost]
        public ActionResult DeleteMasterGenClosingPayroll(List<MasterGenClosingPayrollViewModel> bulkData)
        {

            for (int i = 0; i < bulkData.Count; i++)
            {
                if (bulkData[i].Checkbox == true)
                {
                    if (bulkData[i].ClosingDate >= DateTime.Now.Date)
                    {
                        try
                        {
                            _masterDataBll.DeleteMasterClosingPayroll(new MstClosingPayrollDTO
                            {
                                ClosingDate = bulkData[i].ClosingDate
                            });
                            bulkData[i].ResponseType = Enums.ResponseType.Success.ToString();
                            bulkData[i].Message = "Closing Payroll - Delete " + bulkData[i].ClosingDate.ToShortDateString() + " is successful";
                        }
                        catch (ExceptionBase ex)
                        {
                            bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData[i].Message = "Closing Payroll - Delete " + bulkData[i].ClosingDate.ToShortDateString() + " " + ex.Message;
                        }
                        catch (Exception ex)
                        {
                            bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData[i].Message = "Closing Payroll - Delete " + bulkData[i].ClosingDate.ToShortDateString() + " " + ex.Message;
                        }
                    }
                    else
                    {
                        bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData[i].Message = "Closing Payroll - Delete " + bulkData[i].ClosingDate.ToShortDateString() + " is less than today";
                    }

                }

            }
            return Json(bulkData);
        }
    }
}