using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.PlanningPlantIndividualCapacity;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class PlanningPlantIndividualCapacityController : BaseController
    {

        private IMasterDataBLL _bll;
        private IMaintenanceBLL _maintenanceBll;
        private IApplicationService _applicationService;
        private IPlanningBLL _planningBll;
        private IVTLogger _vtlogger;

        public PlanningPlantIndividualCapacityController(IMasterDataBLL masterDataBll, IVTLogger vtlogger, IMaintenanceBLL maintenanceBll, IApplicationService applicationService, IPlanningBLL planningBll)
        {
            _bll = masterDataBll;
            _maintenanceBll = maintenanceBll;
            _applicationService = applicationService;
            _planningBll = planningBll;
            _vtlogger = vtlogger;
            SetPage("Productionplanning/Plant/IndividualCapacity");
        }

        // GET: PlanningPlantIndividualCapacity
        public ActionResult Index()
        {
            var initPlanningPlanIndividualCapacity = new InitPlanningPlantIndividualCapacityViewModel
            {
                LocationSelectList = _applicationService.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                DefaultWeek = _bll.GetGeneralWeekWeekByDate(DateTime.Now),
            };

            return View("Index", initPlanningPlanIndividualCapacity);
        }

        [HttpGet]
        public JsonResult GetPlantLocationCode()
        {
            var locCodes = _applicationService.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            return Json(locCodes, JsonRequestBehavior.AllowGet);
        }

        #region All Work Hours

        [HttpGet]
        public JsonResult GetLastUpdated(GetPlanningPlantIndividualCapacityWorkHourInput criteria)
        {
            var result = _planningBll.GetPlanningPlantIndividualCapacityWorkHours(criteria).OrderByDescending(p => p.UpdatedDate).FirstOrDefault();
            string output = JsonConvert.SerializeObject(result, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "dd-MM-yyyy HH:mm:ss" });
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandGroupByLocationCode(string locationCode)
        {
            var result = _applicationService.GetBrandGroupCodeSelectListByParentLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnitsByLocationCode(string locationCode)
        {
            var result = _applicationService.GetPlantUnitCodeSelectListByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandGroupCodesByLocationCode(string locationCode)
        {
            var result = _bll.GetBrandCodeByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcessFromCapacityWorkHour(string locationCode, string unitCode, string brandGroupCode, string groupCode)
        {
            var result = _applicationService.GetProcessGroupSelectListByPlanPlantWorkHour(locationCode, unitCode, brandGroupCode, groupCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcessByLocationCodeAndBrandGroup(string locationCode, string brandGroupCode)
        {
            var result = _applicationService.GetProcessGroupSelectListByLocationCodeAndBrandGroupCode(locationCode, brandGroupCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupFromPlantProductionGroupByLocationUnitProcess(string locationCode, string unit, string process)
        {
            try
            {
                //var result = new SelectList(null);
                var result = new SelectList(new List<object>(), "", "");
                if ((locationCode == null) || (unit == null) || (process == null))
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result = _applicationService.GetGroupFromPlantProductionGroupByLocationUnitProcess(locationCode,
                        unit, process);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode , unit , process}, "GetGroupFromPlantProductionGroupByLocationUnitProcess");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetPlanningPlantIndividualCapacityWorkHours(GetPlanningPlantIndividualCapacityWorkHourInput criteria)
        {
            try
            {
                //var result = new SelectList(new List<object>(), "", "");
                var result =
                    _planningBll.GetPlanningPlantIndividualCapacityWorkHours(criteria).OrderBy(x => x.EmployeeNumber);
                var viewModel = Mapper.Map<List<PlanningPlantIndividualCapacityViewModel>>(result);

                //sorting
                var SortedList = viewModel.OrderBy(o => o.EmployeeNumber).ToList();

                var pageResult = new PageResult<PlanningPlantIndividualCapacityViewModel>(SortedList, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "GetPlanningPlantIndividualCapacityWorkHours");
                return null;
            }

        }

        [HttpPost]
        public ActionResult SaveAllPlanningPlantIndividualCapacityWorkHours(InsertUpdateData<PlanningPlantIndividualCapacityViewModel> bulkData)
        {
            try
            {
                // Save data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        var planningPlantIndividualCapacityWirkHours =
                            Mapper.Map<PlanningPlantIndividualCapacityWorkHourDTO>(bulkData.Edit[i]);

                        //set createdby and updatedby
                        planningPlantIndividualCapacityWirkHours.CreatedBy = GetUserName();
                        planningPlantIndividualCapacityWirkHours.UpdatedBy = GetUserName();

                        try
                        {
                            var item =
                                _planningBll.SavePlanningPlantIndividualCapacityWorkHour(
                                    planningPlantIndividualCapacityWirkHours);
                            bulkData.Edit[i] = Mapper.Map<PlanningPlantIndividualCapacityViewModel>(item);
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
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }
                }

                return Json(bulkData);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData  }, "Save - Save All Planning Plant Individual Capacity Work Hours");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string Location, string Unit, string BrandGroupCode, string Process, string Group, string CapacityOfProcess)
        {
            try
            {
                var unsortedplanningPlantIndividualCapacityWorkHours =
                    _planningBll.GetPlanningPlantIndividualCapacityWorkHours(new GetPlanningPlantIndividualCapacityWorkHourInput
                        ()
                    {
                        LocationCode = Location,
                        Unit = Unit,
                        BrandCode = BrandGroupCode,
                        Process = Process,
                        Group = Group,
                        CapacityOfProcess = CapacityOfProcess
                    });

                //sorting
                var planningPlantIndividualCapacityWorkHours =
                    unsortedplanningPlantIndividualCapacityWorkHours.OrderBy(o => o.EmployeeNumber).ToList();

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.PlanningExcelTemplate.PlanningPlantIndividualCapacity + ".xlsx";
                var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {

                    //filter values
                    slDoc.SetCellValue(3, 2, Location);
                    slDoc.SetCellValue(4, 2, Unit);
                    slDoc.SetCellValue(5, 2, BrandGroupCode);
                    slDoc.SetCellValue(6, 2, Process);
                    slDoc.SetCellValue(7, 2, Group);
                    slDoc.SetCellValue(8, 2, CapacityOfProcess);

                    //row values
                    var iRow = 12;

                    foreach (var masterListGroup in planningPlantIndividualCapacityWorkHours)
                    {
                        SLStyle style = slDoc.CreateStyle();
                        style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Font.FontName = "Calibri";
                        style.Font.FontSize = 10;

                        if (iRow%2 == 0)
                        {
                            style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                        }

                        slDoc.SetCellValue(iRow, 1, masterListGroup.EmployeeID);
                        slDoc.SetCellValue(iRow, 2, masterListGroup.EmployeeNumber);
                        slDoc.SetCellValue(iRow, 3, masterListGroup.EmployeeName);
                        slDoc.SetCellValue(iRow, 4,
                            masterListGroup.HoursCapacity3.HasValue ? (int) masterListGroup.HoursCapacity3 : 0);
                        slDoc.SetCellValue(iRow, 5,
                            masterListGroup.HoursCapacity5.HasValue ? (int) masterListGroup.HoursCapacity5 : 0);
                        slDoc.SetCellValue(iRow, 6,
                            masterListGroup.HoursCapacity6.HasValue ? (int) masterListGroup.HoursCapacity6 : 0);
                        slDoc.SetCellValue(iRow, 7,
                            masterListGroup.HoursCapacity7.HasValue ? (int) masterListGroup.HoursCapacity7 : 0);
                        slDoc.SetCellValue(iRow, 8,
                            masterListGroup.HoursCapacity8.HasValue ? (int) masterListGroup.HoursCapacity8 : 0);
                        slDoc.SetCellValue(iRow, 9,
                            masterListGroup.HoursCapacity9.HasValue ? (int) masterListGroup.HoursCapacity9 : 0);
                        slDoc.SetCellValue(iRow, 10,
                            masterListGroup.HoursCapacity10.HasValue ? (int) masterListGroup.HoursCapacity10 : 0);
                        slDoc.SetCellValue(iRow, 11, masterListGroup.UpdatedBy);
                        slDoc.SetCellValue(iRow, 12, masterListGroup.UpdatedDate.ToString(Constants.DefaultDateFormat));
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
                    slDoc.AutoFitColumn(2, 10);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "PlanningPlantIndividualCapacity_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { Location, Unit, BrandGroupCode, Process, Group, CapacityOfProcess }, "Excel - Planning Plant Individual Capacity");
                return null;
            }
        }

        #endregion


        #region By Reference

        [HttpPost]
        public JsonResult GetPlanningPlantIndividualCapacityByReference(GetPlanningPlantIndividualCapacityByReferenceInput criteria)
        {
            try
            {
                var result = _planningBll.GetPlanningPlantIndividualCapacityByReference(criteria);
                var resultAverage = _planningBll.GetPlanningPlantIndividualCapacityAverageByReference(result);

                var viewModel = Mapper.Map<List<PlanningPlantIndividualCapacityByReferenceViewModel>>(resultAverage);

                //sorting
                var SortedList = viewModel.OrderBy(o => o.EmployeeNumber).ToList();

                var pageResult = new PageResult<PlanningPlantIndividualCapacityByReferenceViewModel>(SortedList,
                    criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "GetPlanningPlantIndividualCapacityByReference");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveAllPlanningPlantIndividualByReference(InsertUpdateData<PlanningPlantIndividualCapacityByReferenceViewModel> bulkData)
        {
            try
            {
                var locationCode = bulkData.Parameters != null ? bulkData.Parameters["Location"] : string.Empty;
                var unit = bulkData.Parameters != null ? bulkData.Parameters["Unit"] : string.Empty;
                var brandGroupCode = bulkData.Parameters != null ? bulkData.Parameters["BrandGroupCode"] : string.Empty;
                var process = bulkData.Parameters != null ? bulkData.Parameters["Process"] : string.Empty;
                var group = bulkData.Parameters != null ? bulkData.Parameters["Group"] : string.Empty;
                var workHours = bulkData.Parameters != null ? bulkData.Parameters["WorkHours"] : null;

                // Save data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        var planningPlantIndividualCapacityByReference =
                            Mapper.Map<PlanningPlantIndividualCapacityWorkHourDTO>(bulkData.Edit[i]);

                        //set updatedby
                        planningPlantIndividualCapacityByReference.UpdatedBy = GetUserName();

                        // set parameter
                        planningPlantIndividualCapacityByReference.LocationCode = locationCode;
                        planningPlantIndividualCapacityByReference.UnitCode = unit;
                        planningPlantIndividualCapacityByReference.BrandGroupCode = brandGroupCode;
                        planningPlantIndividualCapacityByReference.ProcessGroup = process;
                        planningPlantIndividualCapacityByReference.GroupCode = group;
                        planningPlantIndividualCapacityByReference.WorkHours = Convert.ToInt32(workHours);

                        try
                        {
                            var item =
                                _planningBll.SavePlanningPlantIndividualCapacityByReference(
                                    planningPlantIndividualCapacityByReference, GetUserName());
                            bulkData.Edit[i] = Mapper.Map<PlanningPlantIndividualCapacityByReferenceViewModel>(item);
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
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData }, "Save All Planning Plant Individual By Reference");
                return null;
            }
            return Json(bulkData);
        }

        [HttpPost]
        public FileStreamResult GenerateExcelByReference(string Location, string Unit, string BrandGroupCode, string Process, string Group, int WorkHours, string DateFrom, string DateTo, string LastUpdated)
        {
            try
            {
                var planningPlantIndividualCapacityByRefence =
                    _planningBll.GetPlanningPlantIndividualCapacityByReference(new GetPlanningPlantIndividualCapacityByReferenceInput
                    {
                        Location = Location,
                        Unit = Unit,
                        BrandGroupCode = BrandGroupCode,
                        Process = Process,
                        Group = Group,
                        WorkHours = WorkHours,
                        DateFrom = DateFrom,
                        DateTo = DateTo
                    });

                var resultAverage =
                    _planningBll.GetPlanningPlantIndividualCapacityAverageByReference(
                        planningPlantIndividualCapacityByRefence);
                var viewModel = Mapper.Map<List<PlanningPlantIndividualCapacityByReferenceViewModel>>(resultAverage);

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.PlanningExcelTemplate.PlanningPlantIndividualCapacityByRefence + ".xlsx";
                var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, Location);
                    slDoc.SetCellValue(4, 2, Unit);
                    slDoc.SetCellValue(5, 2, BrandGroupCode);
                    slDoc.SetCellValue(6, 2, Process);
                    slDoc.SetCellValue(7, 2, Group);
                    slDoc.SetCellValue(3, 4, WorkHours);
                    slDoc.SetCellValue(4, 4, DateFrom);
                    slDoc.SetCellValue(4, 6, DateTo);
                    slDoc.SetCellValue(5, 4, LastUpdated);

                    //row values
                    var iRow = 11;

                    foreach (var masterListGroup in viewModel)
                    {
                        SLStyle style = slDoc.CreateStyle();
                        style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Font.FontName = "Calibri";
                        style.Font.FontSize = 10;

                        if (iRow%2 == 0)
                        {
                            style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                        }

                        slDoc.SetCellValue(iRow, 1, masterListGroup.EmployeeID);
                        slDoc.SetCellValue(iRow, 2,
                            masterListGroup.EmployeeNumber + " - " + masterListGroup.EmployeeName);
                        slDoc.SetCellValue(iRow, 3, masterListGroup.HoursCapacity.Value);

                        slDoc.SetCellValue(iRow, 4,
                            masterListGroup.MinimumValue.HasValue ? (int) masterListGroup.MinimumValue : 0);
                        slDoc.SetCellValue(iRow, 5,
                            masterListGroup.MaximumValue.HasValue ? (int) masterListGroup.MaximumValue : 0);
                        slDoc.SetCellValue(iRow, 6,
                            masterListGroup.AverageValue.HasValue ? (int) masterListGroup.AverageValue : 0);
                        slDoc.SetCellValue(iRow, 7,
                            masterListGroup.MedianValue.HasValue ? (int) masterListGroup.MedianValue : 0);
                        slDoc.SetCellValue(iRow, 8,
                            masterListGroup.LatestValue.HasValue ? (int) masterListGroup.LatestValue : 0);
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
                    slDoc.AutoFitColumn(1, 8);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "PlanningPlantIndividualCapacity_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { Location,  Unit,  BrandGroupCode,  Process,  Group,  WorkHours,  DateFrom,  DateTo,  LastUpdated }, "Excel - GenerateExcelByReference");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetLastUpdatedByReference(GetPlanningPlantIndividualCapacityWorkHourInput criteria)
        {
            try
            {
                var result =
                    _planningBll.GetPlanningPlantIndividualCapacityWorkHours(criteria)
                        .OrderByDescending(p => p.UpdatedDate)
                        .FirstOrDefault();
                //var result = _planningBll.GeteExePlantProductionEntryVerification(criteria).OrderByDescending(p => p.UpdatedDate).FirstOrDefault();
                string output = JsonConvert.SerializeObject(result, Formatting.None,
                    new IsoDateTimeConverter() {DateTimeFormat = "dd-MM-yyyy HH:mm:ss"});
                return Json(output, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria  }, "GetLastUpdatedByReference");
                return null;
            }
        }

        #endregion
    }
}