using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExePlantWorkerBalancing;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs;
using SKTISWebsite.Models.ExePlantWorkerAssignment;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using System.Globalization;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.UtilTransactionLog;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace SKTISWebsite.Controllers
{
    public class ExePlantWorkerBalancingController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IGeneralBLL _generalBll;
        private IPlanningBLL _planningBll;
        private IExecutionPlantBLL _executionPlantBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;

        public ExePlantWorkerBalancingController(IApplicationService applicationService, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IPlanningBLL planningBll, IExecutionPlantBLL executionPlantBll, IUtilitiesBLL utilitiesBLL, IGeneralBLL generalBll)
        {
            _vtlogger = vtlogger;
            _generalBll = generalBll;
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _utilitiesBLL = utilitiesBLL;
            _planningBll = planningBll;
            _executionPlantBll = executionPlantBll;
            SetPage("ProductionExecution/Plant/WorkerBalancing");
        }


        private List<ExePlantWorkerBalancingViewDTO> CheckMultiModel( List<ExePlantWorkerBalancingViewDTO> viewDTOCheck){
            if (viewDTOCheck.Count > 0)
            {
                var checkModelReturn = new List<ExePlantWorkerBalancingViewDTO>();
                int counting = viewDTOCheck.Count;
                for (int i = 0; i < counting; i++)
                {
                    for (int j = 0; j < counting; j++)
                    {
                        if (viewDTOCheck[i].EmployeeID == viewDTOCheck[j].EmployeeID && viewDTOCheck[i].statfrom != viewDTOCheck[j].statfrom && viewDTOCheck[j].statfrom == true)
                        {
                            checkModelReturn.Add(viewDTOCheck[i]);
                        }

                    }
                }

                for (int i = 0; i < checkModelReturn.Count; i++)
                {
                    if (viewDTOCheck.Contains(checkModelReturn[i]))
                    {
                        int del = viewDTOCheck.IndexOf(checkModelReturn[i]);
                        viewDTOCheck.RemoveAt(del);
                    }
                }
            }
            return viewDTOCheck;
        }
        private List<ExePlantWorkerBalancingSingleViewDTO> CheckSingleModel(List<ExePlantWorkerBalancingSingleViewDTO> viewDTOCheck)
        {
            if (viewDTOCheck.Count > 0)
            { 
                var checkModelReturn = new List<ExePlantWorkerBalancingSingleViewDTO>();
                int counting = viewDTOCheck.Count;
                for (int i = 0; i < counting; i++)
                {
                    for (int j = 0; j < counting ; j++)
                    {
                        if (viewDTOCheck[i].EmployeeID == viewDTOCheck[j].EmployeeID && viewDTOCheck[i].statfrom != viewDTOCheck[j].statfrom && viewDTOCheck[j].statfrom == true)
                        {
                            checkModelReturn.Add(viewDTOCheck[i]);
                        }
                    }
                }
                for (int i = 0; i < checkModelReturn.Count; i++)
                {
                    if (viewDTOCheck.Contains(checkModelReturn[i]))
                    {
                        int del = viewDTOCheck.IndexOf(checkModelReturn[i]);
                        viewDTOCheck.RemoveAt(del);
                    }
                }
            }
            return viewDTOCheck;
        }
        // GET: ExePlantWorkerBalancing
        public ActionResult Index()
        {
            var defaultLocationCode = _masterDataBLL.GetMstGenLocation(new GetMstGenLocationInput
            {
                //ParentLocationCode = CurrentUser.Location[0].Code
                LocationCode =  CurrentUser.Location[0].Code
            });

            InitExePlantWorkerBalancing init;
            if (defaultLocationCode != null)
            {
                var DefaultLocationCode = defaultLocationCode.LocationCode ?? "";
                var FilterLocation = _svc.GetLocationByUserResponsibility(CurrentUser.Location[0].Code);
                init = new InitExePlantWorkerBalancing
                {
                    //TransactionDate = DateTime.Now.ToShortDateString(),
                    PLNTChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                    DefaultLocationCode = defaultLocationCode.LocationCode ?? "",
                    FilterLocation = _svc.GetLocationByUserResponsibility(CurrentUser.Location[0].Code)
                };
            }
            else
            {
                init = new InitExePlantWorkerBalancing
                {
                    //TransactionDate = DateTime.Now.ToShortDateString(),
                    PLNTChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                    DefaultLocationCode = CurrentUser.Location[0].Code,
                    FilterLocation = _svc.GetLocationByUserResponsibility(CurrentUser.Location[0].Code)
                };
            }
            
            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetUnitCodeSelectListByLocationCode(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShiftByLocationCode(string locationCode)
        {
            
            var shift = _svc.GetShiftByLocationCode(locationCode);
            return Json(shift, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandEntryVerification(string locationCode, string unitCode)
        {
            var input = new GetExePlantProductionEntryVerificationBrandInput
            {
                LocationCode = locationCode,
                UnitCode = unitCode
                //Date = DateTime.Now
            };
            var brands = _executionPlantBll.GetExePlantProductionEntryVerificationBrand(input).Distinct();
            return Json(brands, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupCodeByLocationUnit(string locationCode, string unitCode, string process)
        {

            var GroupCode = _svc.GetGroupFromPlantProductionGroupByLocationUnitProcess(locationCode, unitCode, process);
            return Json(GroupCode, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetWorkerBalancingMultiSkill(GetExePlantWorkerLoadBalancingMulti Criteria)
        {
            var datamultiskill = _executionPlantBll.GetExePlantWorkerLoadBalancing(Criteria);
            var datamulti = CheckMultiModel(datamultiskill);
            var viewModel = Mapper.Map<List<ExePlantWorkerBalancingViewModel>>(datamulti);
            var pageResult = new PageResult<ExePlantWorkerBalancingViewModel>(viewModel, Criteria);
            pageResult.TotalRecords = datamulti.Count;
            pageResult.TotalPages = (datamulti.Count / Criteria.PageSize) +
                                    (datamulti.Count % Criteria.PageSize != 0 ? 1 : 0);
            var result = datamulti.Skip((Criteria.PageIndex - 1) * Criteria.PageSize).Take(Criteria.PageSize);
            //return Json(datamultiskill, JsonRequestBehavior.AllowGet);
            return Json(pageResult);
        }

        [HttpPost]
        public ActionResult SaveAllPlantWorkerMulti(InsertUpdateData<ExePlantWorkerBalancingViewModel> bulkData)
        {
            var locationCodeSource = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var unitCodeSource = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var transactionDate = bulkData.Parameters != null ? bulkData.Parameters["TransactionDate"] : "";
            var shiftSource = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";
            var brandSource = bulkData.Parameters != null ? bulkData.Parameters["Brand"] : "";
            var kpsYear = _masterDataBLL.GetWeekByDate(DateTime.Now).Year;
            var kpsWeek = _masterDataBLL.GetWeekByDate(DateTime.Now).Week;
            var resultJSonSaveData = "";
            var listResultJSon = new List<string>();

            //insert data
            
            if (bulkData.Edit != null)
            {

                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    //check insert only that from loadbalancing
                    //if (bulkData.Edit[i].StatFrom.Equals("False"))
                    //{
                    if (bulkData.Edit[i].Checkbox == true && bulkData.Edit[i].statfrom == false)
                    {
                        var multibalancing = Mapper.Map<ExePlantWorkerBalancingViewDTO>(bulkData.Edit[i]);
                        var plantWorkerAssignment = new ExePlantWorkerAssignmentDTO();
                        //set createdby and updatedby
                        plantWorkerAssignment.CreatedBy = GetUserName();
                        plantWorkerAssignment.UpdatedBy = GetUserName();
                        //set transaction date
                        plantWorkerAssignment.TransactionDate = DateTime.ParseExact(transactionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                        //set employee
                        plantWorkerAssignment.EmployeeID = multibalancing.EmployeeID;
                        plantWorkerAssignment.EmployeeNumber = multibalancing.EmployeeNumber;
                        //set source
                        plantWorkerAssignment.SourceLocationCode = locationCodeSource;
                        plantWorkerAssignment.SourceUnitCode = multibalancing.SourceUnitCode;
                        plantWorkerAssignment.SourceGroupCode = multibalancing.SourceGroupcode;
                        plantWorkerAssignment.SourceShift = Convert.ToInt32(shiftSource);
                        plantWorkerAssignment.SourceProcessGroup = multibalancing.SourceProcess;
                        plantWorkerAssignment.SourceBrandCode = brandSource;
                        //set destination
                        plantWorkerAssignment.KPSWeek = kpsWeek.ToString();
                        plantWorkerAssignment.KPSYear = kpsYear.ToString();
                        plantWorkerAssignment.DestinationLocationCode = locationCodeSource;
                        plantWorkerAssignment.DestinationUnitCode = multibalancing.UnitCodeDestination;
                        plantWorkerAssignment.DestinationShift = Convert.ToInt32(shiftSource);
                        plantWorkerAssignment.DestinationGroupCode = multibalancing.GroupCodeDestination;
                        plantWorkerAssignment.DestinationProcessGroup = multibalancing.DestinationProcess;
                        plantWorkerAssignment.DestinationBrandCode = brandSource;
                        //set start-end
                        //plantWorkerAssignment.CreatedDate = 
                        plantWorkerAssignment.StartDate = plantWorkerAssignment.TransactionDate;
                        plantWorkerAssignment.EndDate = plantWorkerAssignment.TransactionDate;
                        plantWorkerAssignment.DestinationBrandCode = brandSource;
                        //
                        try
                        {
                            var item = _executionPlantBll.InsertWorkerAssignment(plantWorkerAssignment);
                            //bulkData.New[i] = Mapper.Map<ExePlantWorkerAssignmentViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();

                            // IMPLEMENT TRANSLOG
                            var TransCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                                + locationCodeSource + "/"
                                                + shiftSource + "/"
                                                + multibalancing.SourceUnitCode + "/"
                                                + multibalancing.GroupCodeDestination + "/"
                                                + brandSource;

                            try
                            {
                                //Generate Data for transaction log
                                _generalBll.ExeTransactionLog(new TransactionLogInput()
                                {
                                    page = Enums.PageName.LoadBalancing.ToString(),
                                    ActionButton = Enums.ButtonName.Save.ToString(),
                                    UserName = GetUserName(),
                                    TransactionCode = TransCode,
                                    IDRole = CurrentUser.Responsibility.Role
                                });
                            }
                            catch (Exception e)
                            {
                                _vtlogger.Err(e, new List<object> { TransCode }, "Save - WorkerBalancingLog");
                                resultJSonSaveData = "Failed to run submit data on background process.";
                                listResultJSon.Add(resultJSonSaveData);
                                return Json(listResultJSon);
                            }
                            //####################
                        }
                        catch (ExceptionBase ex)
                        {
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        }
                    }
                    else if (bulkData.Edit[i].statfrom == true)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = "Employee already assign today";
                    }
                    /*else if (bulkData.Edit[i].statfrom == false && bulkData.Edit[i].Checkbox == false)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = "Please Checked for save data";
                    }*/
                }
            }
            return Json(bulkData);
        }

        //singleskill
        [HttpPost]
        public JsonResult GetWorkerBalancingSingleSkill(GetExePlantWorkerLoadBalancingSingle Criteria)
        {
            var singleskill = _executionPlantBll.GetPlantWorkerLoadBalancingSingle(Criteria);
            var datasingleskill = CheckSingleModel(singleskill);
            var viewModel = Mapper.Map<List<ExePlantWorkerBalancingSingleViewDTO>>(datasingleskill);
            var pageResult = new PageResult<ExePlantWorkerBalancingSingleViewDTO>(viewModel, Criteria);
            pageResult.TotalRecords = datasingleskill.Count;
            pageResult.TotalPages = (datasingleskill.Count / Criteria.PageSize) +
                                    (datasingleskill.Count % Criteria.PageSize != 0 ? 1 : 0);
            var result = datasingleskill.Skip((Criteria.PageIndex - 1) * Criteria.PageSize).Take(Criteria.PageSize);
            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetProcessGroupByLocation(string locationCode)
        {

            var processgroup = _svc.GetProcessGroupSelectListByLocationCode(locationCode);
            return Json(processgroup, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDetailDestinantion(string locationCode,string unitcode,string groupcode)
        {

            var processgroup = _svc.GetProcessGroupSelectListByLocationCode(locationCode);
            return Json(processgroup, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveAllPlantWorkerSingle(InsertUpdateData<ExePlantWorkerSingleBalancingViewModel> bulkData)
        {
            var locationCodeSource = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var unitCodeSource = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var transactionDate = bulkData.Parameters != null ? bulkData.Parameters["TransactionDate"] : "";
            var shiftSource = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";
            var brandSource = bulkData.Parameters != null ? bulkData.Parameters["Brand"] : "";
            var kpsYear = _masterDataBLL.GetWeekByDate(DateTime.Now).Year;
            var kpsWeek = _masterDataBLL.GetWeekByDate(DateTime.Now).Week;
            var resultJSonSaveData = "";
            var listResultJSon = new List<string>();

            //insert data

            if (bulkData.Edit != null)
            {

                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    //check insert only that from loadbalancing
                    //if (bulkData.Edit[i].StatFrom.Equals("False"))
                    //{
                    if (bulkData.Edit[i].Checkbox == true && bulkData.Edit[i].statfrom == false) { 
                        var singlebalancing = Mapper.Map<ExePlantWorkerBalancingSingleViewDTO>(bulkData.Edit[i]);
                        var plantWorkerAssignment = new ExePlantWorkerAssignmentDTO();
                        //set createdby and updatedby
                        plantWorkerAssignment.CreatedBy = GetUserName();
                        plantWorkerAssignment.UpdatedBy = GetUserName();
                        //set transaction date
                        plantWorkerAssignment.TransactionDate = DateTime.ParseExact(transactionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                        //set employee
                        plantWorkerAssignment.EmployeeID = singlebalancing.EmployeeID;
                        plantWorkerAssignment.EmployeeNumber = singlebalancing.EmployeeNumber;
                        //set source
                        plantWorkerAssignment.SourceLocationCode = locationCodeSource;
                        plantWorkerAssignment.SourceUnitCode = singlebalancing.UnitCodeSource;
                        plantWorkerAssignment.SourceGroupCode = singlebalancing.GroupCode;
                        plantWorkerAssignment.SourceShift = Convert.ToInt32(shiftSource);
                        plantWorkerAssignment.SourceProcessGroup = singlebalancing.ProcessGroup;
                        plantWorkerAssignment.SourceBrandCode = brandSource;
                        //set destination
                        plantWorkerAssignment.KPSWeek = kpsWeek.ToString();
                        plantWorkerAssignment.KPSYear = kpsYear.ToString();
                        plantWorkerAssignment.DestinationLocationCode = locationCodeSource;
                        plantWorkerAssignment.DestinationUnitCode = singlebalancing.UnitCodeDestination;
                        plantWorkerAssignment.DestinationShift = Convert.ToInt32(shiftSource);
                        plantWorkerAssignment.DestinationGroupCode = singlebalancing.GroupCodeDestination;
                        plantWorkerAssignment.DestinationProcessGroup = singlebalancing.ProcessGroup;
                        plantWorkerAssignment.DestinationBrandCode = brandSource;
                        //set start-end
                        //plantWorkerAssignment.CreatedDate = 
                        plantWorkerAssignment.StartDate = plantWorkerAssignment.TransactionDate;
                        plantWorkerAssignment.EndDate = plantWorkerAssignment.TransactionDate;
                        plantWorkerAssignment.DestinationBrandCode = brandSource;
                        //
                        try
                        {
                            var item = _executionPlantBll.InsertWorkerAssignment(plantWorkerAssignment);
                            //bulkData.New[i] = Mapper.Map<ExePlantWorkerAssignmentViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();

                            // IMPLEMENT TRANSLOG
                            var TransCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                                + locationCodeSource + "/"
                                                + shiftSource + "/"
                                                + singlebalancing.UnitCodeSource + "/"
                                                + singlebalancing.GroupCodeDestination + "/"
                                                + brandSource;

                            try
                            {
                                //Generate Data for transaction log
                                _generalBll.ExeTransactionLog(new TransactionLogInput()
                                {
                                    page = Enums.PageName.LoadBalancing.ToString(),
                                    ActionButton = Enums.ButtonName.Save.ToString(),
                                    UserName = GetUserName(),
                                    TransactionCode = TransCode,
                                    IDRole = CurrentUser.Responsibility.Role
                                });
                            }
                            catch (Exception e)
                            {
                                _vtlogger.Err(e, new List<object> { TransCode }, "Save - WorkerBalancingLog");
                                resultJSonSaveData = "Failed to run submit data on background process.";
                                listResultJSon.Add(resultJSonSaveData);
                                return Json(listResultJSon);
                            }
                            //####################
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
                    else if (bulkData.Edit[i].statfrom == true)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = "Employee already assign today";
                    }
                    //else if (bulkData.Edit[i].statfrom == false && bulkData.Edit[i].Checkbox == false)
                    //{
                        //bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        //bulkData.Edit[i].Message = "Please Checked for save data";
                    //}
                }
                
                //}
            }
            return Json(bulkData);
        }

        [HttpGet]
        public JsonResult GetGroupCodePopUp(GetGroupCodePopUpWorkerAssignmentInput input)
        {
            //input.KPSWeek = _masterDataBLL.GetWeekByDate(DateTime.Now).Year;
            var model = _executionPlantBll.GetGroupCodePopUp(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetLocationName(string locationCode)
        {
            var result = _masterDataBLL.GetMstGenLocation(new GetMstGenLocationInput
            {
                LocationCode = locationCode
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnits(string locationCode)
        {
            var result = _svc.GetUnitCodeSelectListByLocationCode(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShifts(string locationCode)
        {
            var result = _svc.GetShiftByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandCodes(string locationCode, string unitCode)
        {
            var input = new GetExePlantProductionEntryVerificationBrandInput
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                Date = DateTime.Now.Date
            };
            var brands = _executionPlantBll.GetExePlantProductionEntryVerificationBrand(input).Distinct().ToList();
            return Json(brands, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProcessBalancingPerUnit(string locationCode, string unitCode, int? shift, string brandCode)
        {
            var view = "../ExePlantWorkerBalancing/GridBalancingProcessPerUnit";
            var result = _executionPlantBll.GetWorkerBalancingProcessPerUnit(locationCode, unitCode, shift, brandCode);
            return PartialView(view, result);
        }

        /// <summary>
        /// Transaction History
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHistory(TransactionLogInput input)
        {
            var pageResult = new PageResult<TransactionHistoryViewModel>();
            var transactionLog = _utilitiesBLL.GetTransactionHistoryByPage(input, Enums.PageName.LoadBalancing.ToString());
            pageResult.TotalRecords = transactionLog.Count;
            pageResult.TotalPages = (transactionLog.Count / input.PageSize) + (transactionLog.Count % input.PageSize != 0 ? 1 : 0);
            var result = transactionLog.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            pageResult.Results = Mapper.Map<List<TransactionHistoryViewModel>>(result);
            return Json(pageResult);
        }

        /// <summary>
        /// Transaction Flow
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetFlow(TransactionLogInput input)
        {
            var pageResult = new PageResult<TransactionFlowViewModel>();
            var transactionFlow = _utilitiesBLL.GetTransactionFlow(input);
            pageResult.TotalRecords = transactionFlow.Count;
            pageResult.TotalPages = (transactionFlow.Count / input.PageSize) + (transactionFlow.Count % input.PageSize != 0 ? 1 : 0);
            var result = transactionFlow.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            pageResult.Results = Mapper.Map<List<TransactionFlowViewModel>>(result);
            return Json(pageResult);
        }

    }
}