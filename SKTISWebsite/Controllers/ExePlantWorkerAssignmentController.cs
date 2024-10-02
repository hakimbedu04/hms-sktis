using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExePlantWorkerAssignment;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs;
using System.Globalization;

namespace SKTISWebsite.Controllers
{
    public class ExePlantWorkerAssignmentController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IPlanningBLL _planningBll;
        private IExecutionPlantBLL _executionPlantBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;

        public ExePlantWorkerAssignmentController(IApplicationService applicationService, IMasterDataBLL masterDataBll, IPlanningBLL planningBll, IExecutionPlantBLL executionPlantBll, IUtilitiesBLL utilitiesBLL, IVTLogger vtlogger)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _planningBll = planningBll;
            _executionPlantBll = executionPlantBll;
            _utilitiesBLL = utilitiesBLL;
            SetPage("ProductionExecution/Plant/WorkerAssignment");
            _vtlogger = vtlogger;
        }

        // GET: ExePlantWorkerAssignment
        public ActionResult Index()
        {
            var nearestClosingPayrollBeforeToday = _svc.GetNearestClosingPayrollBeforeToday(DateTime.Today).AddDays(1);
            var init = new InitExePlantWorkerAssignmentViewModel
            {
                TodayDate = DateTime.Now.ToShortDateString(),
                YearSelectList = _svc.GetGenWeekYears(),
                PLNTChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                //PLNTTPOChildLocationLookupList = _svc.GetPLNTTPOChildLocationsLookupList(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                ClosingPayrollDate = nearestClosingPayrollBeforeToday.ToShortDateString(),
            };
            return View("Index", init);
        }

        #region FILTER

        [HttpGet]
        public JsonResult GetListUnitCode(string locationCode)
        {

            var model = _svc.GetUnitCodeSelectListByLocationCode(locationCode, true).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult GetListDestinationUnitCode(string locationCode)
        {

            var model = _svc.GetUnitCodeSelectListByLocationCode(locationCode, false).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public JsonResult GetPLNTTPOChildLocationLookupList()
        {
            var model = _svc.GetPLNTTPOChildLocationsLookupList(false);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListShift(string locationCode)
        {
            var shifts = _svc.GetShiftByLocationCode(locationCode);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListBrandFromPlantTpu(string locationCode, string unitCode, int? shift, int? year, int? week)
        {
            var listBrand = _executionPlantBll.GetBrandFromPlantTpu(locationCode, unitCode, shift, year, week);
            return Json(listBrand, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeek(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDate(int year, int week)
        {
            var date = _masterDataBLL.GetWeekByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcess(string locationCode, string brandCode)
        {
            var processSettings = _masterDataBLL.GetAllProcessSettingsByBrandCode(locationCode, brandCode).OrderBy(m => m.ProcessOrder);
            var processSettingsDistinctByProcessGroup = processSettings.DistinctBy(x => x.ProcessGroup);
            return Json(new SelectList(processSettingsDistinctByProcessGroup, "ProcessGroup", "ProcessGroup"), JsonRequestBehavior.AllowGet);
            //var result = _svc.GetProcessGroupSelectListByLocationCodeAndBrandGroupCode(locationCode, brandCode);
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupCode(string locationCode, string unitCode, string process, string brandCode, int year, int week)
        {
            var result = _svc.GetGroupCodeFromPlantEntryVerification(locationCode, unitCode, process, brandCode, year, week);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupCodePopUp(GetGroupCodePopUpWorkerAssignmentInput input)
        {
            var model = _executionPlantBll.GetGroupCodePopUp(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeesActive(GetMstEmployeeJobsDataActivesInput input)
        {
            var model = _masterDataBLL.GetMstEmployeeJobsDataActives(input);

            model = model.OrderBy(o => o.EmployeeNumber.Substring(2)).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeeDetail(string EmployeeID)
        {
            var model = _masterDataBLL.GetMstEmployeeJobsDataActives(EmployeeID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public JsonResult GetExePlantWorkerAssignment(GetExePlantWorkerAssignmentInput criteria)
        {
            var plantWorkerAssignment = _executionPlantBll.GetExePlantWorkerAssignments(criteria);
            var viewModel = Mapper.Map<List<ExePlantWorkerAssignmentViewModel>>(plantWorkerAssignment);
            var pageResult = new PageResult<ExePlantWorkerAssignmentViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetExistingWorkerAbsenteeism(GetExePlantWorkerAbsenteeismInput input)
        {
            var isWorkerAbsenteeismExist = _executionPlantBll.IsExistWorkerAbsenteeismByWorkerAssignment(input);
            return Json(isWorkerAbsenteeismExist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveAllPlantWokerAssignment(InsertUpdateData<ExePlantWorkerAssignmentViewModel> bulkData)
        {
            var locationCodeSource = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var unitCodeSource = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var transactionDate = bulkData.Parameters != null ? bulkData.Parameters["TransactionDate"] : "";
            var shiftSource = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";
            var brandSource = bulkData.Parameters != null ? bulkData.Parameters["Brand"] : "";
            var kpsYear = bulkData.Parameters != null ? bulkData.Parameters["KPSYear"] : "";
            var kpsWeek = bulkData.Parameters != null ? bulkData.Parameters["KPSWeek"] : "";
                
            //insert data
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var plantWorkerAssignment = Mapper.Map<ExePlantWorkerAssignmentDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    plantWorkerAssignment.CreatedBy = GetUserName();
                    plantWorkerAssignment.UpdatedBy = GetUserName();

                    //set transaction date
                    plantWorkerAssignment.TransactionDate = DateTime.ParseExact(transactionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);

                    //set source
                    plantWorkerAssignment.SourceLocationCode = locationCodeSource;
                    plantWorkerAssignment.SourceUnitCode = unitCodeSource;
                    plantWorkerAssignment.SourceShift = Convert.ToInt32(shiftSource);
                    plantWorkerAssignment.DestinationShift = plantWorkerAssignment.SourceShift;
                    plantWorkerAssignment.SourceBrandCode = brandSource;
                    plantWorkerAssignment.KPSWeek = kpsWeek;
                    plantWorkerAssignment.KPSYear = kpsYear;
                    
                    try
                    {
                        var item = _executionPlantBll.InsertWorkerAssignment_SP(plantWorkerAssignment);
                        bulkData.New[i] = Mapper.Map<ExePlantWorkerAssignmentViewModel>(item);
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtlogger.Err(ex, new List<object> { plantWorkerAssignment }, "SavePlantWorkerAssignment");
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> { plantWorkerAssignment }, "SavePlantWorkerAssignment");
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
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
                    var plantWorkerAssignment = Mapper.Map<ExePlantWorkerAssignmentDTO>(bulkData.Edit[i]);

                    //set updatedby
                    plantWorkerAssignment.UpdatedBy = GetUserName();
                    //set createdby
                    plantWorkerAssignment.CreatedBy = GetUserName();

                    try
                    {
                        var item = _executionPlantBll.UpdateWorkerAssignment_SP(plantWorkerAssignment);
                        bulkData.Edit[i] = Mapper.Map<ExePlantWorkerAssignmentViewModel>(item);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtlogger.Err(ex, new List<object> { plantWorkerAssignment, bulkData.Edit }, "SavePlantWorkerAssignment - Update");
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> { plantWorkerAssignment, bulkData.Edit }, "SavePlantWorkerAssignment - Update");
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    }
                }
            }
            return Json(bulkData);
        }


        [HttpGet]
        public JsonResult CheckEblek(
            string startDate,
            string endDate,
            string locationCode,
            string shift,
            string unitCode,
            string group,
            string brand,
            string locationCodeSource,
            string unitCodeSource,
            string groupSource,
            string brandSource,
            string oldStartDate,
            string oldEndDate)
        {
            var arrOutput = new string[0];

            // Edit Mode
            if (!String.IsNullOrEmpty(oldStartDate) || !String.IsNullOrEmpty(oldEndDate)) 
            {
                var input = new GetExePlantWorkerAssignmentInput() { 
                    StartDate = String.IsNullOrEmpty(startDate) ? new DateTime() : Convert.ToDateTime(startDate),
                    EndDate = String.IsNullOrEmpty(endDate)         ? new DateTime() : Convert.ToDateTime(endDate),
                    OldStartDate = String.IsNullOrEmpty(oldStartDate)    ? new DateTime() : Convert.ToDateTime(oldStartDate),
                    OldEndDate = String.IsNullOrEmpty(oldEndDate)      ? new DateTime() : Convert.ToDateTime(oldEndDate),
                    LocationSource = locationCodeSource,
                    LocationDest = locationCode,
                    Shift = Convert.ToInt32(shift),
                    UnitSource = unitCodeSource,
                    UnitDest = unitCode,
                    BrandSource = brandSource,
                    BrandDest = brand,
                    GroupCodeSource = groupSource,
                    GroupCodeDest = group
                };

                var isSubmitted = _executionPlantBll.CheckEblekSubmittedOnEditAssingment(input);
                if (isSubmitted) 
                {
                    var arrData = new string[1];
                    arrData[0] = "eblekSubmited";
                    return Json(arrData, JsonRequestBehavior.AllowGet);
                }
            }
            else 
            {
                var absenteeismInput = new GetExePlantWorkerAbsenteeismInput {
                    StartDateAbsent = String.IsNullOrEmpty(startDate) ? new DateTime() : Convert.ToDateTime(startDate),
                    EndDateAbsent = String.IsNullOrEmpty(endDate) ? new DateTime() : Convert.ToDateTime(endDate)
                };


                // check eblek submit translog
                while (absenteeismInput.StartDateAbsent <= absenteeismInput.EndDateAbsent) {
                    var yearWeek = _masterDataBLL.GetWeekByDate(absenteeismInput.StartDateAbsent);

                    DateTime date = absenteeismInput.StartDateAbsent;
                    int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;

                    // destination check
                    var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + locationCode + "/"
                                              + shift + "/"
                                              + unitCode + "/"
                                              + group + "/"
                                              + brand + "/"
                                              + yearWeek.Year + "/"
                                              + yearWeek.Week + "/"
                                              + day;

                    //var translog = _utilitiesBLL.CheckDataAlreadyOnTransactionLog(productionEntryCode, HMS.SKTIS.Core.Enums.PageName.PlantProductionEntry.ToString(), HMS.SKTIS.Core.Enums.ButtonName.Submit.ToString());
                    var transactionLog = _utilitiesBLL.GetLatestActionTransLogExceptSave(productionEntryCode, Enums.PageName.PlantProductionEntry.ToString());
                    if (transactionLog != null) {
                        var submited = transactionLog.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                        if (submited) {
                            var arrData = new string[1];
                            arrData[0] = "eblekSubmited";
                            return Json(arrData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    //if (translog)
                    //{
                    //    var arrData = new string[1];
                    //    arrData[0] = "eblekSubmited";
                    //    return Json(arrData, JsonRequestBehavior.AllowGet);
                    //}

                    // source check
                    var productionEntryCodeSource = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + locationCodeSource + "/"
                                              + shift + "/"
                                              + unitCodeSource + "/"
                                              + groupSource + "/"
                                              + brandSource + "/"
                                              + yearWeek.Year + "/"
                                              + yearWeek.Week + "/"
                                              + day;

                    //var translogSource = _utilitiesBLL.CheckDataAlreadyOnTransactionLog(productionEntryCodeSource, HMS.SKTIS.Core.Enums.PageName.PlantProductionEntry.ToString(), HMS.SKTIS.Core.Enums.ButtonName.Submit.ToString());
                    var transactionLogSource = _utilitiesBLL.GetLatestActionTransLogExceptSave(productionEntryCodeSource, Enums.PageName.PlantProductionEntry.ToString());
                    if (transactionLogSource != null) {
                        var submited = transactionLogSource.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                        if (submited) {
                            var arrData = new string[1];
                            arrData[0] = "eblekSubmited";
                            return Json(arrData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    //if (translogSource)
                    //{
                    //    var arrData = new string[1];
                    //    arrData[0] = "eblekSubmited";
                    //    return Json(arrData, JsonRequestBehavior.AllowGet);
                    //}

                    // add day to loop
                    absenteeismInput.StartDateAbsent = absenteeismInput.StartDateAbsent.AddDays(1);
                }
            }

            // return array to build notif "EmployeeID - Range Date"
            return Json(arrOutput, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetExePlantWorkerAssignmentExcelInput inputExcel)
        {
            var input = new GetExePlantWorkerAssignmentInput
            {
                LocationCompat = inputExcel.LocationCompat,
                LocationCode = inputExcel.LocationCode,
                UnitCode = inputExcel.UnitCode,                               
                Year = inputExcel.KPSYear,
                Week = inputExcel.KPSWeek,
                Date = inputExcel.Date,
                Shift = inputExcel.Shift,
                ProductionDateFrom = inputExcel.ProductionDateFrom,
                ProductionDateTo = inputExcel.ProductionDateTo,
                DateTypeFilter = inputExcel.DateTypeFilter
            };

            var executionPlantWorkerAssignments = _executionPlantBll.GetExePlantWorkerAssignments(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExecuteExcelTemplate.ExecutePlantWorkerAssignment + ".xlsx";
            var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                var date = inputExcel.Date == null ? "" : inputExcel.Date.Value.ToString(Constants.DefaultDateOnlyFormat);
                var prodDateFrom = inputExcel.ProductionDateFrom == null ? "" : inputExcel.ProductionDateFrom.Value.ToString(Constants.DefaultDateOnlyFormat);
                var prodDateTo = inputExcel.ProductionDateTo == null ? "" : inputExcel.ProductionDateTo.Value.ToString(Constants.DefaultDateOnlyFormat);

                //filter values
                slDoc.SetCellValue(3, 2, ": " + inputExcel.LocationCompat);
                slDoc.SetCellValue(4, 2, ": " + inputExcel.UnitCode);
                slDoc.SetCellValue(5, 2, ": " + inputExcel.Shift);
                slDoc.SetCellValue(6, 2, ": " + inputExcel.BrandCode);
                slDoc.SetCellValue(3, 5, ": " + inputExcel.KPSYear);
                slDoc.SetCellValue(4, 5, ": " + inputExcel.KPSWeek);
                slDoc.SetCellValue(5, 5, ": " + date);
                slDoc.SetCellValue(6, 5, ": " + prodDateFrom);
                slDoc.SetCellValue(6, 7, ": " + prodDateTo);

                //row values
                var iRow = 10;

                foreach (var masterListGroup in executionPlantWorkerAssignments)
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
                     
                    slDoc.SetCellValue(iRow, 1, masterListGroup.SourceProcessGroup);
                    slDoc.SetCellValue(iRow, 2, masterListGroup.SourceGroupCode);
                    slDoc.SetCellValue(iRow, 3, masterListGroup.EmployeeID);
                    slDoc.SetCellValue(iRow, 4, masterListGroup.EmployeeNumber.Substring(masterListGroup.EmployeeNumber.Length - 2, 2) + " - " + masterListGroup.EmployeeName);
                    slDoc.SetCellValue(iRow, 5, masterListGroup.DestinationLocationCode);
                    slDoc.SetCellValue(iRow, 6, masterListGroup.DestinationUnitCode);
                    slDoc.SetCellValue(iRow, 7, masterListGroup.DestinationBrandCode);
                    slDoc.SetCellValue(iRow, 8, masterListGroup.DestinationProcessGroup);
                    slDoc.SetCellValue(iRow, 9, masterListGroup.DestinationGroupCode);
                    slDoc.SetCellValue(iRow, 10, masterListGroup.DestinationGroupCodeDummy);
                    slDoc.SetCellValue(iRow, 11, masterListGroup.StartDate.Date.ToShortDateString());
                    slDoc.SetCellValue(iRow, 12, masterListGroup.EndDate.Date.ToShortDateString());
                    slDoc.SetCellStyle(iRow, 1, iRow, 12, style);
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
                slDoc.AutoFitColumn(1, 12);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "ProductionExecution_WorkerAssignment_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public ActionResult DeleteWorkerAssignment(ExePlantWorkerAssignmentViewModel workerAssignment)
        {
            var workerAssignmentDto = Mapper.Map<ExePlantWorkerAssignmentDTO>(workerAssignment);
            try
            {
                workerAssignmentDto.UpdatedBy = GetUserName();
                _executionPlantBll.DeleteWorkerAssignment_SP(workerAssignmentDto);

                workerAssignment.ResponseType = Enums.ResponseType.Success.ToString();
            }
            catch (ExceptionBase ex)
            {
                _vtlogger.Err(ex, new List<object> { workerAssignmentDto }, "DeleteWorkerAssignment");
                workerAssignment.ResponseType = Enums.ResponseType.Error.ToString();
                workerAssignment.Message = ex.Message;
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { workerAssignmentDto }, "DeleteWorkerAssignment");
                workerAssignment.ResponseType = Enums.ResponseType.Error.ToString();
                workerAssignment.Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            return Json(workerAssignment);
        }

    }
}