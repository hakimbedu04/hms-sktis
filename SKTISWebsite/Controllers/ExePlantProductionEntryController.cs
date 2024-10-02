using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection.Emit;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.ExePlantProductionEntry;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.Common;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using SKTISWebsite.Models;
using SKTISWebsite.Models.UtilTransactionLog;

namespace SKTISWebsite.Controllers
{
    public class ExePlantProductionEntryController : BaseController
    {

        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IPlanningBLL _planningBll;
        private IExecutionPlantBLL _executionPlantBll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IExeReportBLL _exeReportBLL;
        private IPlantWagesExecutionBLL _plantWagesExecutionBll;
        private IExecutionOtherBLL _executionOtherBll;
        private IVTLogger _vtlogger;

        public ExePlantProductionEntryController
        (
            IApplicationService applicationService, 
            IMasterDataBLL masterDataBll, 
            IPlanningBLL planningBll, 
            IExecutionPlantBLL executionPlantBll, 
            IGeneralBLL generalBll, 
            IUtilitiesBLL utilitiesBLL, 
            IExeReportBLL exeReportBLL,
            IPlantWagesExecutionBLL plantWagesExecutionBll,
            IExecutionOtherBLL executionOtherBll,
            IVTLogger vtlogger
        )
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _planningBll = planningBll;
            _executionPlantBll = executionPlantBll;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            _exeReportBLL = exeReportBLL;
            SetPage("ProductionExecution/Plant/PlantProductionEntry");
            _plantWagesExecutionBll = plantWagesExecutionBll;
            _executionOtherBll = executionOtherBll;
            _vtlogger = vtlogger;
        }

        // GET: ProdExePlantProductionEntry
        public ActionResult Index(string param1, string param2, int? param3, string param4, string param5, string param6, int? param7, int? param8, string param9, int? param10)
        {
            if (param10.HasValue) setResponsibility(param10.Value);
            var plntChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            var defaultLocationCode = GetDefaultLocationCode(plntChildLocationLookupList);
            var defaultBrandCode = GetDefaultBrandCodeByLocation(defaultLocationCode);

            var init = new InitExePlantProductionEntryViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                PLNTChildLocationLookupList = plntChildLocationLookupList,
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                //AbsentTypes = _svc.GetAllAbsentTypes(),
                AbsentTypeLookupLists = _svc.GetAbsentTypeLookupListsFroEblek(defaultLocationCode, DateTime.Now),
                DefaultBrandCode = defaultBrandCode,
                Param1LocationCode = param1,
                Param2UnitCode = param2,
                Param3Shift = param3,
                Param4ProcessGroup = param4,
                Param5GroupCode = param5,
                Param6BrandCode = param6,
                Param7KPSYear = param7,
                Param8KPSWeek = param8,
                Param9ProductionDate = String.IsNullOrEmpty(param9) ? DateTime.Now.Date : DateTime.ParseExact(param9, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };

            if (param7.HasValue)
            {
                init.DefaultYear = param7.Value;
            }

            return View("Index", init);
        }

        private string GetDefaultBrandCodeByLocation(string defaultLocationCode)
        {
            var defaultBrandCode = string.Empty;
            if (string.IsNullOrEmpty(defaultLocationCode)) return defaultBrandCode;
            var brandCodes = _masterDataBLL.GetBrandCodeByLocationCode(defaultLocationCode);
            defaultBrandCode = brandCodes.FirstOrDefault();
            return defaultBrandCode;
        }
        private string GetDefaultLocationCode(List<LocationLookupList> locations)
        {
            var defaultLocationCode = string.Empty;
            var locationLookupList = locations.FirstOrDefault();
            if (locationLookupList != null)
            {
                defaultLocationCode = locationLookupList.LocationCode;
            }
            return defaultLocationCode;
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetUnitCodeSelectListByLocationCode(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcessGroupSelectListByLocationCode(string locationCode, string unit, int? shift, string productionDate)
        {
            //var model = _svc.GetAllProcessGroupTPKPlant(locationCode);
            var model = _svc.GetProcessGroupFromPlantEntryVerification(locationCode, unit, shift ?? 0, !string.IsNullOrEmpty(productionDate) ? DateTime.Parse(productionDate) : DateTime.Now.Date);
            var modelfiltered = model.Where(m => m.Text != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily));
            return Json(modelfiltered, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupFromExePlantProductionEntryVerificationByLocationUnitAndProcess(string locationCode, string unit, int? shift, string processGroup, string productionDate)
        {
            //var model = _svc.GetGroupFromExePlantProductionEntryVerificationByLocationUnitAndProcess(locationCode, unit, process);
            var model = _svc.GetGroupCodeFromPlantEntryVerification(locationCode, unit, shift ?? 0, processGroup, !string.IsNullOrEmpty(productionDate) ? DateTime.Parse(productionDate) : DateTime.Now.Date);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandFromExePlantProductionEntryVerificationByLocationUnitAndProcess(string locationCode, string unit, string process)
        {
            var model = _svc.GetGroupBrandExePlantProductionEntryVerificationByLocationUnitAndProcess(locationCode, unit, process);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetBrandCodeByLocationYearWeek(string locationCode, int? KPSYear, int? KPSWeek)
        {
            var brandCodes = _executionPlantBll.GetBrandCodeByLocationYearAndWeekEntryVerification(locationCode, KPSYear, KPSWeek);
            return Json(brandCodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBrandCodeFromExePlantProductionEntryVerification(GetExePlantProductionEntryVerificationInput input)
        {
            var brandCodes = _executionPlantBll.GetExePlantProductionEntryVerification(input);

            var selectList = new SelectList(brandCodes.DistinctBy(m => m.BrandCode), "BrandCode", "BrandCode");

            return Json(selectList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateByYearWeek(int year, int week)
        {
            //var date = _masterDataBLL.GetWeekByYearAndWeek(year, week);
            var date = _svc.GetSelectListDateByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPlanTpkByGroup(string group, string locationCode, string unit, string brand, int year, int week, DateTime? date, int shift)
        {
            if (string.IsNullOrEmpty(group) || group == "undefined") return null;
            var plantTPK = _planningBll.GetPlanningPlantTPKByGroup(group, locationCode, unit, brand, year, week, date, shift);
            return Json(plantTPK, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetClosingPayroll(DateTime? date)
        {
            //var mstGenWeek = _masterDataBLL.GetMstGenWeek(new GetMstGenWeekInput
            //{
            //    CurrentDate = DateTime.Now
            //});

            //var closingDateRange = new GetMstClosingPayrollInput
            //{
            //    StartDate = mstGenWeek.StartDate,
            //    EndDate = mstGenWeek.EndDate
            //};

            //var closingPayroll = _masterDataBLL.GetMasterClosingPayroll(closingDateRange);
            var closingPayroll = _masterDataBLL.GetMasterClosingPayrollByDate(date);
            return closingPayroll != null ? Json(String.Format("{0:dddd, d MMMM yyyy}", closingPayroll.ClosingDate), JsonRequestBehavior.AllowGet) : null;
        }

        [HttpGet]
        public JsonResult GetShiftByLocation(string locationCode)
        {
            var shifts = _svc.GetShiftByLocationCode(locationCode);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetExePlantProductionAllocations(GetExePlantProductionEntryInput criteria)
        {
            var masterLists = _executionPlantBll.GetPlantProductionEntryAllocation(criteria).DistinctBy(m => m.EmployeeID);
            return Json(masterLists, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult GetLastUpdatedByEblekCode(GetExePlantProductionEntryInput input)
        {
            DateTime date = input.Date.Value;
            int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
            var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.UnitCode + "/"
                                      + input.Group + "/"
                                      + input.Brand + "/"
                                      + input.Year + "/"
                                      + input.Week + "/"
                                      + day;
            var transactionLog = _utilitiesBLL.GetLatestActionTransLog(productionEntryCode, Enums.PageName.PlantProductionEntry.ToString());

            //var actualWorkHours = _executionPlantBll.CheckCompletedExePlantActualWorkHours(input);
            var actualWorkHours = true;
            var status = "In Progress";
            DateTime duration = date;

            if (transactionLog != null)
            {
                status = "Save";
                if (transactionLog.IDFlow != 15)  
                    duration = transactionLog.TransactionDate.AddSeconds(30);
                
                var submited = _utilitiesBLL.CheckDataAlreadySumbit(productionEntryCode, Enums.PageName.PlantProductionEntry.ToString());
                if (submited)// && transactionLog.IDFlow > 12)
                {
                    status = "Submit";

                    var transLogVerification = _utilitiesBLL.GetLatestActionTransLogExceptSave(productionEntryCode, Enums.PageName.ProductionEntryVerification.ToString());
                    if (transLogVerification != null)
                    {
                        var isVerificationSubmitted = transLogVerification.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                        if (transactionLog.CreatedDate < transLogVerification.CreatedDate)
                        {
                            if (!isVerificationSubmitted)
                            {
                                status = "Save";
                            }
                        }
                    }

                    var transLogProdCard = productionEntryCode + "/" + _executionPlantBll.GetLatestProdCardRevType(input.LocationCode,
                                                                                                                    input.UnitCode,
                                                                                                                    input.Brand,
                                                                                                                    input.ProcessGroup,
                                                                                                                    input.Group,
                                                                                                                    input.Date.Value);

                    var transLogProdCardSubmitted = _utilitiesBLL.GetLatestActionTransLogExceptSave(transLogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCard.ToString());
                    if (transLogProdCardSubmitted != null)
                    {
                        var isProdCardSubmitted = transLogProdCardSubmitted.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                        if (transactionLog.CreatedDate < transLogProdCardSubmitted.CreatedDate)
                        {
                            if (isProdCardSubmitted)
                            {
                                status = "Closed";
                            }
                        }
                    }

                    var transLogEntryReleaseApproval = _utilitiesBLL.GetLatestActionTransLog(productionEntryCode, Enums.PageName.EblekReleaseApproval.ToString());
                    if (transLogEntryReleaseApproval != null)
                    {
                        var isEblekReleaseApproved = transLogEntryReleaseApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString()
                                                       && transLogEntryReleaseApproval.IDFlow == (int)HMS.SKTIS.Core.Enums.IdFlow.EblekReleaseApprovalFinal;
                        if (transLogProdCardSubmitted != null)
                        {
                            if (transLogProdCardSubmitted.CreatedDate < transLogEntryReleaseApproval.CreatedDate)
                            {
                                if (transactionLog.CreatedDate < transLogEntryReleaseApproval.CreatedDate)
                                {
                                    if (isEblekReleaseApproved)
                                    {
                                        status = "Save";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (transactionLog.CreatedDate < transLogEntryReleaseApproval.CreatedDate)
                            {
                                if (isEblekReleaseApproved)
                                {
                                    status = "Save";
                                }
                            }
                        }

                    }
                }
            }

            return Json(new { status = status, actualWorkhours = actualWorkHours, duration = duration }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetExePlantProductionEntry(GetExePlantProductionEntryInput criteria)
        {
            //var masterLists = _executionPlantBll.GetPlantProductionEntrys(criteria).DistinctBy(m => m.EmployeeID);
            var masterLists = _executionPlantBll.GetPlantProductionEntrysTuning(criteria).DistinctBy(m => m.EmployeeID);
            var viewModel = Mapper.Map<List<ExePlantProductionEntryViewModel>>(masterLists);
            //var total = viewModel.Aggregate(1, (current, exePlantProductionEntryViewModel) => (int) (current * exePlantProductionEntryViewModel.ProdTarget ?? -1));
            //viewModel.ForEach(z => z.TotalProdTarget = total);
            //List<ExePlantProductionEntryViewModel> entryListWithoutMO = viewModel.Where(x => x.AbsentType != "Multiskill Out").ToList();
            //List<ExePlantProductionEntryViewModel> entryListWithMO = viewModel.Where(x => x.AbsentType == "Multiskill Out").ToList();
            //entryListWithoutMO.AddRange(entryListWithMO);
            var pageResult = new PageResult<ExePlantProductionEntryViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        public ActionResult SaveAllocations(List<ExePlantProductionEntryAllocationCompositeDTO> allocationComposites)
        {
            try
            {
                foreach (var exePlantProductionEntryAllocationCompositeDto in allocationComposites)
                {
                    if (exePlantProductionEntryAllocationCompositeDto.Status) continue;
                    var exeplantProdentry = Mapper.Map<ExePlantProductionEntryDTO>(exePlantProductionEntryAllocationCompositeDto);

                    exeplantProdentry.CreatedBy = GetUserName();
                    exeplantProdentry.UpdatedBy = GetUserName();

                    var result = _executionPlantBll.DeleteProductionEntry(exeplantProdentry);
                }
                return Json(allocationComposites);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { allocationComposites }, "SaveAllocations");
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult SaveAllProductionEntry(InsertUpdateData<ExePlantProductionEntryViewModel> bulkData)
        {
            var listResultJSon = new List<ViewModelBase>();

            var saveType = bulkData.Parameters["SaveType"];
            string dateString = bulkData.Parameters["Date"];
            DateTime date = String.IsNullOrEmpty(dateString) ? DateTime.Now.Date : Convert.ToDateTime(dateString);
            int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
            var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                      + bulkData.Parameters["LocationCode"] + "/"
                                      + bulkData.Parameters["Shift"] + "/"
                                      + bulkData.Parameters["UnitCode"] + "/"
                                      + bulkData.Parameters["Group"] + "/"
                                      + bulkData.Parameters["Brand"] + "/"
                                      + bulkData.Parameters["Year"] + "/"
                                      + bulkData.Parameters["Week"] + "/"
                                      + day;

            var productionEntryCodeDummy = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                      + bulkData.Parameters["LocationCode"] + "/"
                                      + bulkData.Parameters["Shift"] + "/"
                                      + bulkData.Parameters["UnitCode"] + "/"
                                      + bulkData.Parameters["Group"].Remove(1, 1).Insert(1, "5") + "/"
                                      + bulkData.Parameters["Brand"] + "/"
                                      + bulkData.Parameters["Year"] + "/"
                                      + bulkData.Parameters["Week"] + "/"
                                      + day;
            // save data
            if (bulkData.Edit != null)
            {
                try
                {
                    _executionPlantBll.SaveDefaultTargetActualProdEntry_SP(productionEntryCode, saveType);
                    _executionPlantBll.SaveDefaultTargetActualProdEntry_SP(productionEntryCodeDummy, saveType);
                } catch(Exception e)
                {
                    _vtlogger.Err(e, new List<object> { productionEntryCode, productionEntryCodeDummy }, "Save - Default Target Actual Production Entry");
                }
                

                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var plantProductionGroup = Mapper.Map<ExePlantProductionEntryDTO>(bulkData.Edit[i]);
                    
                    //plantProductionGroup.Shift = bulkData.Parameters.Any() ? (int)bulkData.Parameters["Shift"] : 1;
                    plantProductionGroup.StartDateAbsent = date.Date;
                    plantProductionGroup.ProductionDate = date.Date;

                    if (string.IsNullOrEmpty(plantProductionGroup.AbsentType))
                    {
                        plantProductionGroup.AbsentCodePayroll = null;
                    }
                    else
                    {
                        var absentType = _masterDataBLL.GetMstPlantAbsentTypeById(plantProductionGroup.AbsentType);
                        plantProductionGroup.AbsentCodePayroll = absentType == null ? null : absentType.PayrollAbsentCode;
                    } 

                    plantProductionGroup.UpdatedBy = GetUserName();
                    plantProductionGroup.SaveType = saveType;
                    plantProductionGroup.UpdatedDate = DateTime.Now;

                    if (plantProductionGroup.AbsentType == null && plantProductionGroup.AbsentCodePayroll == HMS.SKTIS.Core.Enums.SKTAbsentCode.MO.ToString())
                    {
                        // harcoded
                        plantProductionGroup.AbsentType = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.SKTAbsentCode.MO);
                        plantProductionGroup.AbsentCodeEblek = HMS.SKTIS.Core.Enums.SKTAbsentCode.MO.ToString();
                    }

                    try
                    {
                        var item = _executionPlantBll.SaveProductionEntry_SP(plantProductionGroup);

                        bulkData.Edit[i] = Mapper.Map<ExePlantProductionEntryViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();

                        //if (plantProductionGroup.ProdActual == 0 && (plantProductionGroup.AbsentType == "Alpa" | plantProductionGroup.AbsentType == "Ijin"))
                        //{
                        //    DateTime dateSting = Convert.ToDateTime(bulkData.Parameters["Date"]);
                        //    var plantWorkerAbs = new ExePlantWorkerAbsenteeismDTO()
                        //    {
                        //        EmployeeID = plantProductionGroup.EmployeeID,
                        //        EmployeeNumber = plantProductionGroup.EmployeeNumber,
                        //        AbsentType = plantProductionGroup.AbsentType,
                        //        SktAbsentCode = plantProductionGroup.AbsentCodeEblek,
                        //        PayrollAbsentCode = plantProductionGroup.AbsentCodePayroll,
                        //        StartDateAbsent = dateSting,
                        //        EndDateAbsent = dateSting,
                        //        CreatedBy = GetUserName(),
                        //        UpdatedBy = GetUserName(),
                        //        LocationCode = bulkData.Parameters["LocationCode"],
                        //        UnitCode = bulkData.Parameters["UnitCode"],
                        //        GroupCode = bulkData.Parameters["Group"],
                        //        TransactionDate = dateSting,
                        //        Shift = Convert.ToInt32(bulkData.Parameters["Shift"])
                        //    };

                        //    var itemWorker = _executionPlantBll.InsertWorkerAbsenteeism(plantWorkerAbs);
                        //}
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtlogger.Err(ex, new List<object> { plantProductionGroup }, "Save - SaveProductionEntry_SP");
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = "Save Production Entry Error Employee: " + bulkData.Edit[i].EmployeeNumber + " - " + bulkData.Edit[i].EmployeeName + " (" + ex.Message + ")";
                        listResultJSon.Add(new ViewModelBase { ResponseType = Enums.ResponseType.Error.ToString().ToLower(), Message = ex.Message, State = bulkData.Parameters["SaveType"] });
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> { plantProductionGroup }, "Save - SaveProductionEntry_SP");
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString().ToLower();
                        bulkData.Edit[i].Message = "Save Production Entry Error Employee: " + bulkData.Edit[i].EmployeeNumber + " - " + bulkData.Edit[i].EmployeeName + " (" + ex.Message + ")";
                        var resultJSon = "Save Production Entry Error Employee: " + bulkData.Edit[i].EmployeeNumber + " - " + bulkData.Edit[i].EmployeeName + " (" + bulkData.Edit[i].Message + ")";
                        listResultJSon.Add(new ViewModelBase { ResponseType = Enums.ResponseType.Error.ToString().ToLower(), Message = resultJSon, State = bulkData.Parameters["SaveType"] });
                    }
                }

                try
                {
                    // this section is for transaction log insertion
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = "PlantProductionEntry",
                        ActionButton = "Save",
                        UserName = GetUserName(),
                        TransactionCode = productionEntryCode,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = "PlantProductionEntry",
                        ActionButton = "Save",
                        UserName = GetUserName(),
                        TransactionCode = productionEntryCodeDummy,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                } catch (Exception e)
                {
                    _vtlogger.Err(e, new List<object> { productionEntryCode, productionEntryCodeDummy }, "Save - Generate Translog");
                }
                
            }

            listResultJSon.Add(new ViewModelBase { ResponseType = Enums.ResponseType.Success.ToString().ToLower(), Message = "Save Production Entry Success", State = bulkData.Parameters["SaveType"] });
            return Json(listResultJSon);
        }
        [HttpPost]
        public ActionResult SubmitDatas(InsertUpdateData<ExePlantProductionEntryViewModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();
            // save data
            if (bulkData.Edit != null)
            {
                //Check if Production Entry Contains MO
                //Add by Indra 
                //refer to ticket : http://tp.voxteneo.co.id/entity/3051
                //for (var i = 0; i < bulkData.Edit.Count; i++)
                //{
                //    var plantProductionGroup = Mapper.Map<ExePlantProductionEntryDTO>(bulkData.Edit[i]);
                //    //check row is null
                //    if (bulkData.Edit[i].AbsentType != null &&
                //        bulkData.Edit[i].AbsentCodeEblek == HMS.SKTIS.Core.Enums.SKTAbsentCode.MO.ToString())
                //    {
                //        var input = new GetExePlantWorkerAssignmentInput()
                //        {
                //            LocationCode = bulkData.Parameters["LocationCode"],
                //            UnitCode = bulkData.Parameters["UnitCode"],
                //            Shift = Convert.ToInt32(bulkData.Parameters["Shift"]),
                //            Year = Convert.ToInt32(bulkData.Parameters["Year"]),
                //            Week = Convert.ToInt32(bulkData.Parameters["Week"]),
                //            Date = Convert.ToDateTime(bulkData.Parameters["Date"]),
                //            SourceBrandCode = bulkData.Parameters["Brand"],
                //            ProductionDateFrom = Convert.ToDateTime(bulkData.Parameters["Date"]),
                //            DateTypeFilter = "rdbProductionDate"
                //        };

                //        var existWorkerAssignment = _executionPlantBll.IsExistWorkerAssignment(input).FirstOrDefault(t => t.EmployeeID == bulkData.Edit[i].EmployeeID);
                //        if (existWorkerAssignment != null)
                //        {
                //            string dateString = bulkData.Parameters["Date"];
                //            DateTime date = Convert.ToDateTime(dateString);
                //            int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;

                //            var productionEntryCode =
                //                    EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                //                    + existWorkerAssignment.DestinationLocationCode + "/"
                //                    + bulkData.Parameters["Shift"] + "/"
                //                    + existWorkerAssignment.DestinationUnitCode + "/"
                //                    + existWorkerAssignment.DestinationGroupCode + "/"
                //                    + existWorkerAssignment.DestinationBrandCode + "/"
                //                    + bulkData.Parameters["Year"] + "/"
                //                    + bulkData.Parameters["Week"] + "/"
                //                    + day;
                //            var submittedProductionEntry = _utilitiesBLL.CheckDataAlreadySumbit(productionEntryCode);
                //            if (!submittedProductionEntry && (existWorkerAssignment.DestinationLocationCode != bulkData.Parameters["LocationCode"] || 
                //                existWorkerAssignment.DestinationUnitCode != bulkData.Parameters["UnitCode"] ||
                //                existWorkerAssignment.DestinationGroupCode != bulkData.Parameters["Group"] ||
                //                existWorkerAssignment.DestinationBrandCode != bulkData.Parameters["Brand"]))
                //                return Json("There is no submitted Production Entry for " + productionEntryCode + ". Please Submit Data First in Employee's MO Destination");
                //        }
                //    }
                //}

                string dateString = bulkData.Parameters["Date"];
                DateTime date = Convert.ToDateTime(dateString);
                int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                            + bulkData.Parameters["LocationCode"] + "/"
                                            + bulkData.Parameters["Shift"] + "/"
                                            + bulkData.Parameters["UnitCode"] + "/"
                                            + bulkData.Parameters["Group"] + "/"
                                            + bulkData.Parameters["Brand"] + "/"
                                            + bulkData.Parameters["Year"] + "/"
                                            + bulkData.Parameters["Week"] + "/"
                                            + day;
                string location = bulkData.Parameters["LocationCode"];
                string unitCode = bulkData.Parameters["UnitCode"];
                string brandCode = bulkData.Parameters["Brand"];
                int shift = Int32.Parse(bulkData.Parameters["Shift"]);
                int KPSYear = Int32.Parse(bulkData.Parameters["Year"]);
                int KPSWeek = Int32.Parse(bulkData.Parameters["Week"]);
                string groupCode = bulkData.Parameters["Group"];

                try
                {
                    _executionPlantBll.SubmitProductionEntry(location, unitCode, brandCode, shift, KPSYear, KPSWeek, date, groupCode, GetUserName());
                } catch (Exception e)
                {
                    _vtlogger.Err(e, new List<object> { productionEntryCode }, "Submit - SubmitProductionEntry");
                    resultJSonSubmitData = "Failed to run submit data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                    return Json(listResultJSon);
                }
                

                var productionEntryCodeDummy = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                            + bulkData.Parameters["LocationCode"] + "/"
                                            + bulkData.Parameters["Shift"] + "/"
                                            + bulkData.Parameters["UnitCode"] + "/"
                                            + bulkData.Parameters["Group"].Remove(1, 1).Insert(1, "5") + "/"
                                            + bulkData.Parameters["Brand"] + "/"
                                            + bulkData.Parameters["Year"] + "/"
                                            + bulkData.Parameters["Week"] + "/"
                                            + day;

                try
                {
                    //Generate Data for transaction log
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.PlantProductionEntry.ToString(),
                        ActionButton = Enums.ButtonName.Submit.ToString(),
                        UserName = GetUserName(),
                        TransactionCode = productionEntryCode,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.PlantProductionEntry.ToString(),
                        ActionButton = Enums.ButtonName.Submit.ToString(),
                        UserName = GetUserName(),
                        TransactionCode = productionEntryCodeDummy,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                } catch (Exception e)
                {
                    _vtlogger.Err(e, new List<object> { productionEntryCode, productionEntryCodeDummy }, "Submit - ExeTransactionLog");
                    resultJSonSubmitData = "Failed to run submit data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                    return Json(listResultJSon);
                }

                resultJSonSubmitData = "Run submit data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
            }

            var LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var UnitCode = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var BrandCode = bulkData.Parameters != null ? bulkData.Parameters["Brand"] : "";
            var Shift = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";
            var Year = bulkData.Parameters != null ? bulkData.Parameters["Year"] : "";
            var Week = bulkData.Parameters != null ? bulkData.Parameters["Week"] : "";
            var GroupCode = bulkData.Parameters != null ? bulkData.Parameters["Group"] : "";
            var DateString = bulkData.Parameters != null ? bulkData.Parameters["Date"] : "";
            DateTime Date = Convert.ToDateTime(DateString);
            // disable method send email based on related ticket :http://tp.voxteneo.co.id/entity/8422
            //try
            //{
            //    var param = new GetExePlantProductionEntryInput
            //    {
            //        LocationCode = LocationCode,
            //        UnitCode = UnitCode,
            //        Brand = BrandCode,
            //        Shift = Shift,
            //        Year = Convert.ToInt32(Year),
            //        Week = Convert.ToInt32(Week),
            //        Group = GroupCode,
            //        Date = Date
            //    };

            //    _executionOtherBll.SendEmailSubmitPlantEntry(param, GetUserName());
            //}
            //catch(Exception ex)
            //{
            //    resultJSonSendEmail = "Failed to send email.";

            //    listResultJSon.Add(resultJSonSendEmail);

            //    return Json(listResultJSon);
            //}

            return Json(listResultJSon);
        }

        [HttpPost]
        public ActionResult CancelSubmitDatas(InsertUpdateData<ExePlantProductionEntryViewModel> bulkData)
        {
            try
            {
                string dateString = bulkData.Parameters["Date"];
                DateTime date = Convert.ToDateTime(dateString);
                int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                          + bulkData.Parameters["LocationCode"] + "/"
                                          + bulkData.Parameters["Shift"] + "/"
                                          + bulkData.Parameters["UnitCode"] + "/"
                                          + bulkData.Parameters["Group"] + "/"
                                          + bulkData.Parameters["Brand"] + "/"
                                          + bulkData.Parameters["Year"] + "/"
                                          + bulkData.Parameters["Week"] + "/"
                                          + day;
                string location = bulkData.Parameters["LocationCode"];
                string unitCode = bulkData.Parameters["UnitCode"];
                string brandCode = bulkData.Parameters["Brand"];
                int shift = Int32.Parse(bulkData.Parameters["Shift"]);
                int KPSYear = Int32.Parse(bulkData.Parameters["Year"]);
                int KPSWeek = Int32.Parse(bulkData.Parameters["Week"]);
                string groupCode = bulkData.Parameters["Group"];

                //Update Verify System = false
                _executionPlantBll.ReturnProductionEntryVerification(productionEntryCode);

                //Delete ExeReportByGroups
                _exeReportBLL.DeleteReportByGroups(location, groupCode, brandCode, date);

                // Dete ExeReportByGroups for Dummy group
                _exeReportBLL.DeleteReportByGroups(location, groupCode.Remove(1,1).Insert(1, "5"), brandCode, date);

                //Delete transaction log
                //_utilitiesBLL.DeleteTransactionLog(new TransactionLogInput()
                //{
                //    TransactionCode = productionEntryCode,
                //    NotEqualIdFlow = 11
                //});

                //Generate transaction log cancel submit
                _generalBll.ExeTransactionLog(new TransactionLogInput()
                {
                    page = Enums.PageName.PlantProductionEntry.ToString(),
                    ActionButton = EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit),
                    UserName = GetUserName(),
                    TransactionCode = productionEntryCode,
                    IDRole = CurrentUser.Responsibility.Role
                });

                var productionEntryCodeDummy = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                             + bulkData.Parameters["LocationCode"] + "/"
                                             + bulkData.Parameters["Shift"] + "/"
                                             + bulkData.Parameters["UnitCode"] + "/"
                                             + bulkData.Parameters["Group"].Remove(1, 1).Insert(1, "5") + "/"
                                             + bulkData.Parameters["Brand"] + "/"
                                             + bulkData.Parameters["Year"] + "/"
                                             + bulkData.Parameters["Week"] + "/"
                                             + day;

                //Update Verify System = false
                _executionPlantBll.ReturnProductionEntryVerification(productionEntryCodeDummy);

                //Delete transaction log dummy
                //_utilitiesBLL.DeleteTransactionLog(new TransactionLogInput()
                //{
                //    TransactionCode = productionEntryCodeDummy,
                //    NotEqualIdFlow = 11
                //});

                //Generate transaction log cancel submit dummy group
                _generalBll.ExeTransactionLog(new TransactionLogInput()
                {
                    page = Enums.PageName.PlantProductionEntry.ToString(),
                    ActionButton = EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit),
                    UserName = GetUserName(),
                    TransactionCode = productionEntryCodeDummy,
                    IDRole = CurrentUser.Responsibility.Role
                });

                // Check condition before delete prodcard for remark (surat periode lalu)


                // Delete production card
                _plantWagesExecutionBll.DeleteProdCardByReturnVerificationRevType(new GetProductionCardInput
                {
                    LocationCode = location,
                    Unit = unitCode,
                    Brand = brandCode,
                    Process = bulkData.Parameters["ProcessGroup"],
                    Group = groupCode,
                    Date = date
                }, productionEntryCode, GetUserName());
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> {bulkData.Parameters});
                return Json("Failed to run cancel submit data.");
            }

            return Json("Cancel submit data success.");
        }

        //public JsonResult GetPlanTpkByGroup(string group, string locationCode, string unit, string brand, int year, int week, DateTime? date, int shift)
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCompat, string locationCode, string unitCode, string shift, string process, string group, string brandCode, int year, int week, DateTime date)
        {
            var input = new GetExePlantProductionEntryInput
            {
                LocationCompat = locationCompat,
                LocationCode = locationCode,
                UnitCode = unitCode,
                Shift = shift,
                ProcessGroup = process,
                Group = group,
                Brand = brandCode,
                Year = year,
                Week = week,
                Date = date,
                SortExpression = "EmployeeNumber",
                SortOrder = "ASC"
            };

            var executionPlantProductionEntrys = _executionPlantBll.GetPlantProductionEntrysTuning(input).DistinctBy(m => m.EmployeeID); ;

            if (string.IsNullOrEmpty(group) || group == "undefined") return null;
            var plantTPK = _planningBll.GetPlanningPlantTPKByGroup(group, locationCode, unitCode, brandCode, year, week, date, Convert.ToInt32(shift));
            var closingPayroll = _masterDataBLL.GetMasterClosingPayrollByDate(date);
            //sorting
            //var executionPlantProductionEntrys = unsortedexecutionPlantProductionEntrys.OrderBy(o => o.EmployeeNumber).ToList();

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExecuteExcelTemplate.ExecutePlantProductionEntry + ".xlsx";
            var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }
            var payrollDate = closingPayroll != null ? (string.Format("{0:dddd, d MMMM yyyy}",closingPayroll.ClosingDate)) : null;
            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 2, ": " + locationCompat);
                slDoc.SetCellValue(4, 2, ": " + unitCode);
                slDoc.SetCellValue(5, 2, ": " + shift);
                slDoc.SetCellValue(6, 2, ": " + process);
                slDoc.SetCellValue(7, 2, ": " + group);
                slDoc.SetCellValue(8, 2, ": " + brandCode);
                slDoc.SetCellValue(3, 7, ": " + year.ToString());
                slDoc.SetCellValue(4, 7, ": " + week.ToString());
                slDoc.SetCellValue(5, 7, ": " + date.ToString("dd/MM/yyyy"));
                slDoc.SetCellValue(6, 7, ": " + payrollDate); 
                slDoc.SetCellValue(7, 7, ": " + plantTPK.TPKValue);

                //row values
                var iRow = 13;

                int totalRow = 0;
                int totalProdCapacity = 0;
                int totalProdTarget = 0;
                double totalProdActual = 0.00;

                foreach (var masterListGroup in executionPlantProductionEntrys)
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
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, masterListGroup.EmployeeID);
                    slDoc.SetCellValue(iRow, 2, masterListGroup.EmployeeName);
                    slDoc.SetCellValue(iRow, 3, masterListGroup.ProdCapacity.HasValue ? (int)masterListGroup.ProdCapacity : 0);
                    slDoc.SetCellValue(iRow, 4, masterListGroup.ProdTarget.HasValue ? (int)masterListGroup.ProdTarget : 0);
                    slDoc.SetCellValue(iRow, 5, masterListGroup.ProdActual.HasValue ? Math.Round((float)masterListGroup.ProdActual,2) : 0);
                    slDoc.SetCellValue(iRow, 6, masterListGroup.AbsentType);
                    slDoc.SetCellValue(iRow, 7, masterListGroup.AbsentCodeEblek);
                    slDoc.SetCellStyle(iRow, 1, iRow, 7, style);
                    iRow++;

                    totalRow++;
                    totalProdCapacity = totalProdCapacity + (masterListGroup.ProdCapacity.HasValue ? (int)masterListGroup.ProdCapacity : 0);
                    totalProdTarget = totalProdTarget + (masterListGroup.ProdTarget.HasValue ? (int)masterListGroup.ProdTarget : 0);
                    totalProdActual = totalProdActual + (masterListGroup.ProdActual.HasValue ? Math.Round((float)masterListGroup.ProdActual, 2):0);
                }

                SLStyle totalStyle = slDoc.CreateStyle();
                totalStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                totalStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                totalStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                totalStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                totalStyle.Font.FontName = "Calibri";
                totalStyle.Font.FontSize = 10;
                totalStyle.Font.Bold = true;
                totalStyle.Fill.SetPattern(PatternValues.Solid, Color.DarkCyan, Color.DarkCyan);

                slDoc.SetCellValue(iRow, 1, "TOTAL");
                slDoc.SetCellValue(iRow, 2, totalRow);
                slDoc.SetCellValue(iRow, 3, totalProdCapacity);
                slDoc.SetCellValue(iRow, 4, totalProdTarget);
                slDoc.SetCellValue(iRow, 5, totalProdActual);
                slDoc.SetCellValue(iRow, 6, "");
                slDoc.SetCellValue(iRow, 7, "");
                slDoc.SetCellStyle(iRow, 1, iRow, 7, totalStyle);

                //slDoc.SetCellValue(6, 7, ": " + plantTPK.TPKValue);
                slDoc.SetCellValue(8, 7, ": " + totalProdTarget);
                slDoc.SetCellValue(9, 7, ": " + totalProdActual);

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
            var fileName = "ProductionExecution_ProductionEntry_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// Transaction History
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHistory(TransactionLogInput input)
        {
            if (!input.code_9.Equals(""))
            {
                DateTime enteredDate = DateTime.Parse(input.code_9);
                int day = enteredDate.DayOfWeek == 0 ? 7 : (int)enteredDate.DayOfWeek;
                input.code_9 = day.ToString();
            }

            var pageResult = new PageResult<TransactionHistoryViewModel>();
            var transactionLog = _utilitiesBLL.GetTransactionHistoryByPage(input, Enums.PageName.PlantProductionEntry.ToString());
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

        [HttpGet]
        public JsonResult GetMinimumValueByProdEntryFilter(GetExePlantProductionEntryInput criteria)
        {
            var minValueActual = _executionPlantBll.GetMinimumValueForActualProdEntry(criteria);

            return Json(minValueActual, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListAbsentTypeHoliday(string locationCode, DateTime date)
        {
            //var model = _svc.GetAbsentTypeLookupListsFroEblek(locationCode, date );
            var init = new InitExePlantProductionEntryViewModel
            {
             AbsentTypeLookupLists = _svc.GetAbsentTypeLookupListsFroEblek(locationCode, date )
            };
            return Json(init.AbsentTypeLookupLists, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListAbsentTypeCalculation(string id)
        {
            var model = _masterDataBLL.GetAllAbsentTypesGetCalCulation(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllAbsentTypeCalculation()
        {
            var model = _masterDataBLL.GetAllAbsentTypeCalCulation();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsHolidayOrNot(DateTime date, string locationCode)
        {
            var model = _masterDataBLL.IsHOlidayOrSunday(date, locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}