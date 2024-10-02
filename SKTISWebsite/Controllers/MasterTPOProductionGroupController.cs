using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BLL.ExecutionBLL;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.MasterTPOProductionGroup;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using SKTISWebsite.Models.PlanningTPOTPK;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using System.Globalization;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;

namespace SKTISWebsite.Controllers
{
    public class MasterTPOProductionGroupController : BaseController
    {
        private IMasterDataBLL _masterDataBll;
        private IApplicationService _applicationService;
        private IPlanningBLL _planningBLL;
        private IExecutionTPOBLL _exeTPOBLL;
        private IUtilitiesBLL _utilitiesBll;

        public MasterTPOProductionGroupController(
            IMasterDataBLL masterDataBll,
            IApplicationService applicationService,
            IPlanningBLL planningBLL,
            IExecutionTPOBLL exeTPOBLL,
            IUtilitiesBLL utilitiesBll
        )
        {
            _masterDataBll = masterDataBll;
            _applicationService = applicationService;
            SetPage("MasterData/TPO/ProductionGroup");
            _planningBLL = planningBLL;
            _exeTPOBLL = exeTPOBLL;
            _utilitiesBll = utilitiesBll;
        }

        // GET: MasterTPOProductionGroup
        public ActionResult Index()
        {
            var locationsDefault = _applicationService.GetPlantAndRegionalLocationLookupList();
            var initTPOProductionGroup = new InitMasterTPOProductionGroup()
            {
                LocationLookupList = locationsDefault,
                //LocationLists = Mapper.Map<List<SelectListItem>>(_masterDataBll.GetMasterTPOInfos(new GetMasterTPOInfoInput())),
                LocationLists = _applicationService.GetLastChildLocationByTPO(),
                LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(_masterDataBll.GetMstLocationLists(new GetMstGenLocationInput()))
            };
            return View("index", initTPOProductionGroup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetGenLocProcessSettingListByLocationCode(string locationCode)
        {
            //var model = _masterDataBll.GetMasterProcessSettingByLocationCode(locationCode);
            //return Json(model, JsonRequestBehavior.AllowGet);
            var brandGroupCodes = _applicationService.GetProcessGroupCodeSelectListByParentLocationCode(locationCode);
            return Json(brandGroupCodes, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetGenEmpStatusListByLocationCode(string locationCode)
        {
            var model = _applicationService.GetStatusByParentLocationCodeAsValue(locationCode); ;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetTPOProductionGroup(GetMstTPOProductionGroupInput criteria)
        {
            var pageResult = new PageResult<MasterTPOProductionGroupViewModel>();
            var masterLists = _masterDataBll.GetTPOProductionGroupLists(criteria);
            pageResult.TotalRecords = masterLists.Count;
            pageResult.TotalPages = (masterLists.Count / criteria.PageSize) + (masterLists.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = masterLists.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MasterTPOProductionGroupViewModel>>(result);
            return Json(pageResult);
        }

        [HttpPost]
        public ActionResult SaveAllTPOProductionGroup(InsertUpdateData<MasterTPOProductionGroupViewModel> bulkData)
        {
            var readyTPOTPK = new List<TPOTPKDTO>();

            if ((bulkData.New != null) || (bulkData.Edit != null))
            {
                var ProcessGroups = new List<string>();
                var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";

                if (bulkData.New != null)
                    ProcessGroups.AddRange(bulkData.New.Select(x => x.ProcessGroup).Distinct());

                if (bulkData.Edit != null)
                {
                    foreach (var dataEdit in bulkData.Edit)
                    {
                        if (dataEdit != null && !ProcessGroups.Exists(x => x == dataEdit.ProcessGroup))
                            ProcessGroups.Add(dataEdit.ProcessGroup);
                    }
                }
                    //ProcessGroups.AddRange(bulkData.Edit.Select(x => x.ProcessGroup).Distinct());

                // get current week and last week
                MstGenWeekDTO week = _masterDataBll.GetWeekByDate(DateTime.Now.AddDays(-7));

                // get un submitted tpo tpk data
                readyTPOTPK = _planningBLL.GetLatestReadyToSubmitTPOTPK(locationCode, ProcessGroups, week);
            }

            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";

                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var mstTPOProdGroup = Mapper.Map<MstTPOProductionGroupDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    mstTPOProdGroup.CreatedBy = GetUserName();
                    mstTPOProdGroup.UpdatedBy = GetUserName();
                    mstTPOProdGroup.UpdatedDate = DateTime.Now;

                    //set Location Code
                    mstTPOProdGroup.LocationCode = locationCode;

                    //set
                    mstTPOProdGroup.ProdGroup = mstTPOProdGroup.ProdGroupComputed;
                    try
                    {
                        //set Status
                        var status = bulkData.New[i].StatusEmp;
                        mstTPOProdGroup.StatusEmp = status;

                        //set Process
                        var process = bulkData.New[i].ProcessGroup;
                        mstTPOProdGroup.ProcessGroup = process;

                        var item = _masterDataBll.InsertTPOProductionGroup(mstTPOProdGroup);

                        var tpoTPKByProcess = Mapper.Map<TPOTPKDTO>(bulkData.New[i]);
                        tpoTPKByProcess.WorkerAlocation = bulkData.New[i].WorkerCount;
                        tpoTPKByProcess.WorkerAvailable = bulkData.New[i].WorkerCount;
                        tpoTPKByProcess.WorkerRegister = bulkData.New[i].WorkerCount;
                        tpoTPKByProcess.LocationCode = mstTPOProdGroup.LocationCode;
                        tpoTPKByProcess.ProdGroup = mstTPOProdGroup.ProdGroup;
                        tpoTPKByProcess.ProcessGroup = mstTPOProdGroup.ProcessGroup;
                        tpoTPKByProcess.StatusEmp = mstTPOProdGroup.StatusEmp;

                        //RecalculateTpoTpk(tpoTPKByProcess, bulkData.New[i].StatusActive);
                        _planningBLL.SaveLatestPlanTPOTPK(readyTPOTPK, tpoTPKByProcess, bulkData.New[i].StatusActive, GetUserName());

                        UpdateTPOProductionEntry(bulkData.Parameters["LocationCode"], process, status, mstTPOProdGroup, Enums.SaveType.New, GetUserName());

                        bulkData.New[i] = Mapper.Map<MasterTPOProductionGroupViewModel>(item);
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
                    var mstListGroup = Mapper.Map<MstTPOProductionGroupDTO>(bulkData.Edit[i]);
                    var mstListGroupOriginal = _masterDataBll.GetTpoProductionGroupById(mstListGroup.ProdGroup,
                        mstListGroup.ProcessGroup, mstListGroup.LocationCode, mstListGroup.StatusEmp);

                    var ReActivate = !mstListGroupOriginal.StatusActive.Value && mstListGroup.StatusActive.Value ? true : false;

                    //set updatedby
                    mstListGroup.CreatedBy = GetUserName();
                    mstListGroup.UpdatedBy = GetUserName();
                    mstListGroup.UpdatedDate = DateTime.Now;

                    var tpoTPKByProcess = Mapper.Map<TPOTPKDTO>(bulkData.Edit[i]);
                    tpoTPKByProcess.WorkerAlocation = bulkData.Edit[i].WorkerCount;
                    tpoTPKByProcess.WorkerAvailable = bulkData.Edit[i].WorkerCount;
                    tpoTPKByProcess.WorkerRegister = bulkData.Edit[i].WorkerCount;
                    try
                    {
                        var item = _masterDataBll.UpdateTPOProductionGroup(mstListGroup);
                        var status = bulkData.Edit[i].StatusEmp;

                        //set Process
                        var process = bulkData.Edit[i].ProcessGroup;

                        //RecalculateTpoTpk(tpoTPKByProcess, bulkData.Edit[i].StatusActive);
                        _planningBLL.SaveLatestPlanTPOTPK(readyTPOTPK, tpoTPKByProcess, bulkData.Edit[i].StatusActive, GetUserName());

                        #region Update TPO Production Entry

                        if (ReActivate)
                        {
                            UpdateTPOProductionEntry(bulkData.Parameters["LocationCode"], process, status, mstListGroup, Enums.SaveType.New, GetUserName());
                        }
                        else
                        {
                            UpdateTPOProductionEntry(bulkData.Parameters["LocationCode"], process, status, mstListGroup, Enums.SaveType.Edit, GetUserName());
                        }
                        
                        #endregion

                        bulkData.Edit[i] = Mapper.Map<MasterTPOProductionGroupViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        // Retrieve the error messages as a list of strings.
                        var errorMessages = ex.EntityValidationErrors
                                .SelectMany(x => x.ValidationErrors)
                                .Select(x => x.ErrorMessage);

                        // Join the list to a single string.
                        var fullErrorMessage = string.Join("; ", errorMessages);

                        // Combine the original exception message with the new one.
                        var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
    
                        throw new System.Data.Entity.Validation.DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);

                    }
                }
            }

            if ((bulkData.New != null) || (bulkData.Edit != null))
            {
                // recalculate
                foreach (var tpotpkdto in readyTPOTPK)
                {
                    RecalculateTpoTpk(tpotpkdto);
                }
            }

            return Json(bulkData);
        }

        private void RecalculateTpoTpk(TPOTPKDTO tpotpk)
        {

            #region save and recalculate TPOTPK
            if (tpotpk != null)
            {
                var criteria = new GetTPOTPKInput
                {
                    TPKCode = tpotpk.TPKCode,
                    LocationCode = tpotpk.LocationCode,
                    BrandCode = tpotpk.BrandCode,
                    KPSYear = tpotpk.KPSYear,
                    KPSWeek = tpotpk.KPSWeek
                };

                var viewModels = new List<PlanTPOTPKViewModel>();

                //get list of tpk;
                var tpoTPKs = _planningBLL.GetPlanningTPOTPK(criteria);

                //get list of process
                var listProcess = _masterDataBll.GetAllActiveMasterProcess()
                    .OrderBy(p => p.ProcessOrder)
                    .ToList();

                foreach (var process in listProcess)
                {
                    var processGroup = process.ProcessGroup;
                    var viewModel = new PlanTPOTPKViewModel
                    {
                        ProcessGroup = processGroup,
                        LocationCode = criteria.LocationCode,
                        BrandCode = criteria.BrandCode,
                        KPSYear = criteria.KPSYear,
                        KPSWeek = criteria.KPSWeek,
                        ShowInWIPStock = process.WIP ?? false,
                        UnitCode = Constants.TPOUnitCode
                    };

                    var tpks = tpoTPKs.Where(m => m.ProcessGroup == processGroup).OrderBy(m => m.ProdGroup).ToList();
                    if (!tpks.Any()) continue;

                    //add target wpp
                    var getTargetWPPInput = new GetTargetWPPInput()
                    {
                        LocationCode = criteria.LocationCode,
                        BrandCode = criteria.BrandCode,
                        KPSYear = criteria.KPSYear.Value,
                        KPSWeek = criteria.KPSWeek.Value
                    };

                    viewModel.TargetWPP = _planningBLL.GetTargetWPP(getTargetWPPInput);

                    //add wip stock
                    if (process.WIP.HasValue && process.WIP.Value)
                    {
                        var input = new GetPlantWIPStockInput()
                        {
                            LocationCode = criteria.LocationCode,
                            UnitCode = Constants.TPOUnitCode,
                            BrandCode = criteria.BrandCode,
                            ProcessGroup = processGroup,
                            KPSYear = criteria.KPSYear,
                            KPSWeek = criteria.KPSWeek
                        };

                        var wipStock = _planningBLL.GetPlantWIPStock(input);
                        if (wipStock != null)
                        {
                            viewModel.WIPPreviousValue = wipStock.WIPPreviousValue;
                            viewModel.WIPStock1 = wipStock.WIPStock1;
                            viewModel.WIPStock2 = wipStock.WIPStock2;
                            viewModel.WIPStock3 = wipStock.WIPStock3;
                            viewModel.WIPStock4 = wipStock.WIPStock4;
                            viewModel.WIPStock5 = wipStock.WIPStock5;
                            viewModel.WIPStock6 = wipStock.WIPStock6;
                            viewModel.WIPStock7 = wipStock.WIPStock7;
                        }
                    }

                    //add jk process
                    var firstTpk = tpks.FirstOrDefault();
                    if (firstTpk != null)
                    {
                        viewModel.JKProcess1 = firstTpk.ProcessWorkHours1;
                        viewModel.JKProcess2 = firstTpk.ProcessWorkHours2;
                        viewModel.JKProcess3 = firstTpk.ProcessWorkHours3;
                        viewModel.JKProcess4 = firstTpk.ProcessWorkHours4;
                        viewModel.JKProcess5 = firstTpk.ProcessWorkHours5;
                        viewModel.JKProcess6 = firstTpk.ProcessWorkHours6;
                        viewModel.JKProcess7 = firstTpk.ProcessWorkHours7;
                    }

                    //add tpk list
                    foreach (var tpkModel in tpks.Select(Mapper.Map<PlanTPOTPKModel>))
                    {
                        viewModel.PlanTPOTPK.Add(tpkModel);
                    }

                    viewModels.Add(viewModel);
                }

                var inputTPKInBox = new PlanTPOTPKTotalBoxInput()
                {
                    LocationCode = criteria.LocationCode,
                    BrandCode = criteria.BrandCode,
                    KPSYear = criteria.KPSYear.Value,
                    KPSWeek = criteria.KPSWeek.Value
                };

                var totals = GetTPKTotalResult(inputTPKInBox);

                var headerWorkHours = _planningBLL.GetTPOTPKInBox(new PlanTPOTPKTotalBoxInput() { LocationCode = criteria.LocationCode, BrandCode = criteria.BrandCode, KPSYear = Convert.ToInt16(criteria.KPSYear), KPSWeek = Convert.ToInt16(criteria.KPSWeek) });

                var inputTPOTPK = new CalculateTPOTPKInput()
                {
                    KPSYear = Convert.ToInt16(criteria.KPSYear),
                    KPSWeek = Convert.ToInt16(criteria.KPSWeek),
                    LocationCode = criteria.LocationCode,
                    BrandCode = criteria.BrandCode,
                    ListTPOTPK = Mapper.Map<List<TPOTPKByProcessDTO>>(viewModels),
                    TotalTPOTPK = Mapper.Map<List<PlanTPOTPKTotalBoxDTO>>(totals),
                    HeaderProcessWorkHours = new List<float>() { 
                                float.Parse(headerWorkHours.ProcessWorkHours1.ToString(), CultureInfo.CurrentCulture), 
                                float.Parse(headerWorkHours.ProcessWorkHours2.ToString(), CultureInfo.CurrentCulture),
                                float.Parse(headerWorkHours.ProcessWorkHours3.ToString(), CultureInfo.CurrentCulture),
                                float.Parse(headerWorkHours.ProcessWorkHours4.ToString(), CultureInfo.CurrentCulture),
                                float.Parse(headerWorkHours.ProcessWorkHours5.ToString(), CultureInfo.CurrentCulture),
                                float.Parse(headerWorkHours.ProcessWorkHours6.ToString(), CultureInfo.CurrentCulture),
                                float.Parse(headerWorkHours.ProcessWorkHours7.ToString(), CultureInfo.CurrentCulture)
                            },
                    FilterCurrentDayForward = DateTime.Now,
                    IsFilterCurrentDayForward = false
                };

                //_planningBLL.CalculateTPOTPKByTPKCode(tpotpk.TPKCode);

                var result = _planningBLL.CalculateTPOTPK(inputTPOTPK);
                foreach (var saveTPO in result.TPOTPKByProcess)
                {
                    saveTPO.UpdatedBy = GetUserName();
                    foreach (var plantpk in saveTPO.PlanTPOTPK)
                    {
                        plantpk.UpdatedBy = GetUserName();
                    }
                    var hmm = Mapper.Map<TPOTPKByProcessDTO>(saveTPO);
                    _planningBLL.SaveTPOTPKByGroup(hmm);
                }
            }
            #endregion save and recalculate TPOTPK
        }

        private void UpdateTPOProductionEntry(string LocationCode, string Process, string Status, MstTPOProductionGroupDTO ProductionGroup, Enums.SaveType Action, string User)
        {
            //var now = DateTime.Parse("2016-05-02");
            var now = DateTime.Now.AddDays(-7);
            var mstGenWeek = _masterDataBll.GetWeekByDate(now);
            var eptev = _exeTPOBLL.GetExeTPOProductionEntryVerificationBetweenProductionDates(LocationCode, Process, mstGenWeek.StartDate.Value, null);

            if (eptev.Count > 0)
            {
                foreach (var entry in eptev.Where(w => w.ProductionDate >= mstGenWeek.StartDate.Value).ToList())
                {
                    var util = _utilitiesBll.GetLatestActionTransLog(entry.ProductionEntryCode,
                        Enums.PageName.TPOProductionEntryVerification.ToString());

                    // Skip the next process if verification already submitted
                    if ((util != null && util.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString())) continue;

                    if (ProductionGroup.StatusActive.HasValue && ProductionGroup.StatusActive.Value)
                    {
                        switch (Action)
                        {
                            case Enums.SaveType.New :
                                _exeTPOBLL.InsertGroupTPOProductionEntry(entry.ProductionEntryCode,
                            Status, ProductionGroup, entry, User);
                                break;
                            case Enums.SaveType.Edit :
                                _exeTPOBLL.UpdateTPOProductionEntryWorkerCount(entry.ProductionEntryCode,
                            Status, ProductionGroup, entry, User);
                                break;
                        }
                        
                    }
                    else
                    {
                        // Delete group grom TPO Production entries
                        _exeTPOBLL.DeleteTPOProductionEntry(entry.ProductionEntryCode,
                            Status, ProductionGroup.ProdGroup);
                    }
                    
                }
            }
        }

        private List<PlanTPOTPKTotalModel> GetTPKTotalResult(PlanTPOTPKTotalBoxInput inputTPKInBox)
        {
            var totalViewModel = new List<PlanTPOTPKTotalModel>();

            //get total in box
            var tpkInBox = _planningBLL.GetTPOTPKInBox(inputTPKInBox);
            if (tpkInBox != null)
            {
                //get total instick
                var brandForStick = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = inputTPKInBox.BrandCode });

                var tpkInStick = new PlanTPOTPKTotalModel
                {
                    LocationCode = inputTPKInBox.LocationCode,
                    BrandCode = inputTPKInBox.BrandCode,
                    KPSYear = inputTPKInBox.KPSYear,
                    KPSWeek = inputTPKInBox.KPSWeek,
                    TotalType = "STICK",
                    TargetSystem1 = tpkInBox.TargetSystem1 * brandForStick.StickPerBox,
                    TargetSystem2 = tpkInBox.TargetSystem2 * brandForStick.StickPerBox,
                    TargetSystem3 = tpkInBox.TargetSystem3 * brandForStick.StickPerBox,
                    TargetSystem4 = tpkInBox.TargetSystem4 * brandForStick.StickPerBox,
                    TargetSystem5 = tpkInBox.TargetSystem5 * brandForStick.StickPerBox,
                    TargetSystem6 = tpkInBox.TargetSystem6 * brandForStick.StickPerBox,
                    TargetSystem7 = tpkInBox.TargetSystem7 * brandForStick.StickPerBox,
                    TargetManual1 = tpkInBox.TargetManual1 * brandForStick.StickPerBox,
                    TargetManual2 = tpkInBox.TargetManual2 * brandForStick.StickPerBox,
                    TargetManual3 = tpkInBox.TargetManual3 * brandForStick.StickPerBox,
                    TargetManual4 = tpkInBox.TargetManual4 * brandForStick.StickPerBox,
                    TargetManual5 = tpkInBox.TargetManual5 * brandForStick.StickPerBox,
                    TargetManual6 = tpkInBox.TargetManual6 * brandForStick.StickPerBox,
                    TargetManual7 = tpkInBox.TargetManual7 * brandForStick.StickPerBox,
                    TotalTargetSystem = (tpkInBox.TargetSystem1 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem2 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem3 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem4 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem5 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem6 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem7 * brandForStick.StickPerBox),
                    TotalTargetManual = (tpkInBox.TargetManual1 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual2 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual3 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual4 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual5 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual6 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual7 * brandForStick.StickPerBox)
                };
                totalViewModel.Add(tpkInStick);

                var resultInBox = Mapper.Map<PlanTPOTPKTotalModel>(tpkInBox);
                resultInBox.TotalType = "BOX";
                totalViewModel.Add(resultInBox);
            }

            return totalViewModel;
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string process, string status)
        {
            var processGroup = "";
            var statusEmp = "";
            
            // Get ProcessGroup based Proess Identifier
            var processDto = _masterDataBll.GetMasterProcessByIdentifier(process);
            if (processDto != null)
                processGroup = processDto.ProcessGroup;
            // Get Status Emp based Status Identifier
            var statusDto = _masterDataBll.GetGenEmpStatusByIdentifier(status);
            if (statusDto != null)
                statusEmp = statusDto.StatusEmp;

            var input = new GetMstTPOProductionGroupInput()
            {
                LocationCode = locationCode,
                Process =  process,
                Status = status
            };

            var allLocations = _applicationService.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == locationCode)
                {
                    locationCompat = item.Text;
                }
            }

            var listTpo = _masterDataBll.GetTPOProductionGroupLists(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstTPOProductionGroup + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 2, locationCompat);
                slDoc.SetCellValue(3, 2, processGroup == "" ? "All" : processGroup);
                slDoc.SetCellValue(4, 2, statusEmp == "" ? "All" : statusEmp);


                //row values
                var iRow = 7;

                foreach (var masterItem in listTpo.Select((value, index) => new { value = value, index = index }))
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (masterItem.index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, masterItem.value.ProdGroup);
                    slDoc.SetCellValue(iRow, 2, masterItem.value.WorkerCount.ToString());
                    slDoc.SetCellValue(iRow, 3, masterItem.value.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 4, masterItem.value.Remark);
                    slDoc.SetCellValue(iRow, 5, masterItem.value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 6, masterItem.value.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 6, style);
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
            var fileName = "MasterTPOProductionGroup_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}