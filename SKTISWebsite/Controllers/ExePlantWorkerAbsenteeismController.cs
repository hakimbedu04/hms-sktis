using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExePlantWorkerAbsenteeism;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Controllers
{
    public class ExePlantWorkerAbsenteeismController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _bll;
        private IExecutionPlantBLL _exe;
        private IUploadService _uploadService;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;

        public ExePlantWorkerAbsenteeismController(IApplicationService svc, IMasterDataBLL bll, IExecutionPlantBLL exe, IUploadService uploadService, IUtilitiesBLL utilitiesBLL, IVTLogger vtlogger)
        {
            _svc = svc;
            _bll = bll;
            _exe = exe;
            _uploadService = uploadService;
            _utilitiesBLL = utilitiesBLL;
            SetPage("ProductionExecution/Plant/WorkerAbsenteeism");
            _vtlogger = vtlogger;
        }

        public ActionResult Index()
        {
            var nearestClosingPayrollBeforeToday = _svc.GetNearestClosingPayrollBeforeToday(DateTime.Today);
            var model = new InitExePlantWorkerAbsenteeism()
            {
                AbsentTypePopUpList = _exe.GetListAbsentActiveOnAbsenteeism(),
                LocationCodeSelectList = _svc.GetPlantChildLocationCompat(),
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultYear = DateTime.Now.Year,
                DefaultWeek = _bll.GetGeneralWeekWeekByDate(DateTime.Now),
                TodayDate = DateTime.Today.ToShortDateString(),
                ClosingPayrollDate = nearestClosingPayrollBeforeToday.ToShortDateString(),
                PLNTChildLocationLookupList = _svc.GetChildLocationsLookupList(HMS.SKTIS.Core.Enums.LocationCode.PLNT.ToString()),
                //AllActiveEmployees = Mapper.Map<List<MstEmployeeJobsDataActiveDTO>>(_bll.GetAllEmployeeJobsDataActives(new GetAllEmployeeJobsDataActivesInput() { })),
                AllAbsentType = Mapper.Map<List<MstPlantAbsentTypeDTO>>(_bll.GetMstPlantAbsentTypes(new GetMstAbsentTypeInput())),
                AllAbsentTypeActiveInAbsenteeism = Mapper.Map<List<MstPlantAbsentTypeDTO>>(_bll.GetMstPlantAbsentTypes(new GetMstAbsentTypeInput() { ActiveInAbsent = true })),
                UploadPath = new Uri(Request.Url, Url.Content("~/Upload/PlantWorkerAbsenteeism/")).AbsoluteUri
            };

            if (model.AllAbsentType.Count > 1) model.AllAbsentType.Insert(0, new MstPlantAbsentTypeDTO { AbsentType = "", SktAbsentCode = "", PayrollAbsentCode = "" });

            return View(model);
        }

        /// <summary>
        /// Gets the unit code select list.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetPlantUnitCodeSelectListByLocationCode(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets Shift Select List by Location Code
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetShiftSelectListByLocationCode(string locationCode)
        {
            var model = _bll.GetShiftByLocationCode(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets Max Day Absent Type
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMaxDayAsbentType(string absentType)
        {
            var maxday = string.IsNullOrEmpty(absentType) ? 0 : _exe.GetMaxDayByAbsentType(absentType);
            return Json(maxday, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Process by Location
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetProcessByLocationCode(string locationCode)
        {
            var input = new GetAllProcessSettingsInput() { LocationCode = locationCode };
            var processSettings = _bll.GetMasterProcessSettingByLocationCode(locationCode);
            var processSettingsDistinctByProcessGroup = processSettings
                                                        .Where(c => c.ProcessGroup != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily))
                                                        .DistinctBy(x => x.ProcessGroup);
            return Json(new SelectList(processSettingsDistinctByProcessGroup, "ProcessGroup", "ProcessGroup"), JsonRequestBehavior.AllowGet);
            //var model = _svc.GetAllProcessGroupTPKPlant(locationCode);
            //var modelfiltered = model.Where(m => m.Text != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily));
            //return Json(modelfiltered, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Process by Location for DAily
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetProcessByLocationCodeDaily(string locationCode)
        {
            var result = _svc.GetProcessGroupListByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcessDailyEmployeeJobData(string locationCode, string unitCode)
        {
            var model = _bll.GetMstEmployeeJobsDataActivesForDaily(new GetMstEmployeeJobsDataActivesInput
            {
                LocationCode = locationCode,
                UnitCode = unitCode
            }).Select(c => c.ProcessSettingsCode).Distinct().ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Group By Location, Unit, and Process
        /// </summary>
        /// <param name="locationCode"></param>
        /// <param name="unit"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetGroupByLocationUnitProcess(string locationCode, string unit, string process)
        {
            var result = _svc.GetGroupFromPlantProductionGroupByLocationUnitProcess(locationCode, unit, process);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupFromExePlantProductionEntryVerificationByLocationUnitAndProcess(string locationCode, string unit, int? shift, string processGroup, string productionDate)
        {
            //var model = _svc.GetGroupFromExePlantProductionEntryVerificationByLocationUnitAndProcess(locationCode, unit, process);
            var model = _svc.GetGroupCodeFromPlantEntryVerification(locationCode, unit, shift ?? 0, processGroup, !string.IsNullOrEmpty(productionDate) ? DateTime.Parse(productionDate) : DateTime.Now.Date);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the Weeks by Year.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _bll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the date by week and year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDateByYearWeek(int year, int week)
        {
            var date = _bll.GetWeekByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the maintenance execution item conversion es.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWorkerAbsenteeism(GetExePlantWorkerAbsenteeismInput criteria)
        {
            var result = _exe.GetWorkerAbsenteeism(criteria);

            var nearestClosingPayrollBeforeToday = _svc.GetNearestClosingPayrollBeforeToday(DateTime.Today).AddDays(1);
            foreach (var item in result)
            {
                if (!item.IsFromEblek && item.OldValueStartDateAbsent.Date < nearestClosingPayrollBeforeToday.Date && item.OldValueEndDateAbsent.Date >= nearestClosingPayrollBeforeToday.Date)
                    item.StateOnEdit = "EDIT";
                else
                    item.StateOnEdit = "INSERT";
            }

            var viewModel = Mapper.Map<List<ExePlantWorkerAbsenteeismViewModel>>(result);
            var pageResult = new PageResult<ExePlantWorkerAbsenteeismViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public JsonResult GetWorkerAbsenteeismDaily(GetExePlantWorkerAbsenteeismInput criteria)
        {
            var result = _exe.GetWorkerAbsenteeismDaily(criteria);
            var viewModel = Mapper.Map<List<ExePlantWorkerAbsenteeismViewModel>>(result);
            var pageResult = new PageResult<ExePlantWorkerAbsenteeismViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        /// <summary>
        /// Get Active Employee by Location, Unit, Process, and Group
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeesActive(GetMstEmployeeJobsDataActivesInput input)
        {
            var model = _bll.GetMstEmployeeJobsDataActives(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Active Employee by Location, Unit, Process, and Group for Piece Rate
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPieceRateEmployeesActive(GetMstEmployeeJobsDataActivesInput input)
        {   
            if(!CheckExistingUnitCode(input.LocationCode, input.UnitCode))
                return Json(new List<MstEmployeeJobsDataActiveCompositeDTO>(), JsonRequestBehavior.AllowGet);

            var model = _bll.GetMstEmployeeJobsDataActivesForPieceRate(input).OrderBy(t => t.EmployeeNumber);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Active Employee by Location, Unit, Process, and Group for Daily
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDailyEmployeesActive(GetMstEmployeeJobsDataActivesInput input)
        {
            if (!CheckExistingUnitCode(input.LocationCode, input.UnitCode))
                return Json(new List<MstEmployeeJobsDataActiveCompositeDTO>(), JsonRequestBehavior.AllowGet);

            var model = _bll.GetMstEmployeeJobsDataActivesForDaily(input);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Check Exisiting UnitCode in MstPlantUnit
        /// </summary>
        /// <param name="locationCode">Filter Location Code</param>
        /// <param name="unitCode">Filter Unit Code</param>
        /// <returns></returns>
        private bool CheckExistingUnitCode(string locationCode, string unitCode)
        {
            if (String.IsNullOrEmpty(unitCode)) return false;

            //Get Existing UnitCode
            var existingUnitCode = _bll.GetAllUnits(new GetAllUnitsInput() { LocationCode = locationCode })
                                    .Where(c => c.UnitCode == unitCode).ToList();

            return existingUnitCode.Any();
        }

        /// <summary>
        /// Employee Detail
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResult GetEmployeeDetail(string EmployeeID)
        {
            var model = _bll.GetMstEmployeeJobsDataActives(EmployeeID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get All Absent Type
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllAbsentType()
        {
            var model = _bll.GetMstPlantAbsentTypesWithoutSLSorSLP(new GetMstAbsentTypeInput() { ActiveInAbsent = true });
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllAbsentTypesActiveInAbsent()
        {
            var result = _bll.GetAllAbsentTypesActiveInAbsent();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Employee Actual in Production Entry / StdStickPerHour in MstGenProcessSetting
        /// </summary>
        /// <param name="input">Model Worker Absenteeism Input</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeActualStdStickPerHour(GetExePlantWorkerAbsenteeismInput input)
        {
            var result = _exe.CalculateEmployeeActualAndStdStickPerHour(input);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveAllPlantWokerAbsenteeism(InsertUpdateData<ExePlantWorkerAbsenteeismViewModel> bulkData)
        {
            var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var unitCode = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var transactionDate = bulkData.Parameters != null ? bulkData.Parameters["TransactionDate"] : "";
            var shift = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";
            var groupCode = "";
            bool pieceRateTab;
            if (bulkData.Parameters.ContainsKey("GroupCodePieceRate"))
            {
                groupCode = bulkData.Parameters != null ? bulkData.Parameters["GroupCodePieceRate"] : "";
                pieceRateTab = true;
            }
            else
            {
                pieceRateTab = false;
            }

            //insert data
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var plantWorkerAbs = Mapper.Map<ExePlantWorkerAbsenteeismDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    plantWorkerAbs.CreatedBy = GetUserName();
                    plantWorkerAbs.UpdatedBy = GetUserName();

                    //set shift
                    plantWorkerAbs.Shift = Convert.ToInt32(shift);

                    //set transaction date
                    plantWorkerAbs.TransactionDate = DateTime.ParseExact(transactionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);

                    plantWorkerAbs.StartDateAbsent = plantWorkerAbs.StartDateAbsent.Date;
                    plantWorkerAbs.EndDateAbsent = plantWorkerAbs.EndDateAbsent.Date;

                    //set location and unitcode
                    plantWorkerAbs.LocationCode = locationCode;
                    plantWorkerAbs.UnitCode = unitCode;

                    try
                    {
                        //CheckEblekSubmitted(plantWorkerAbs.EmployeeID, plantWorkerAbs.StartDateAbsent, plantWorkerAbs.EndDateAbsent, locationCode, plantWorkerAbs.Shift.ToString(), unitCode, plantWorkerAbs.GroupCode, "INSERT");

                        if (!pieceRateTab)
                        {
                            groupCode = _bll.GetMstEmployeeJobsDataActives(plantWorkerAbs.EmployeeID).GroupCode;
                        }

                        var input = new GetExePlantWorkerAbsenteeismExcelPieceRateInput
                        {
                            LocationCode = plantWorkerAbs.LocationCode,
                            UnitCode = plantWorkerAbs.UnitCode,
                            Shift = plantWorkerAbs.Shift,
                            Group = groupCode
                        };
                        CheckEblekStatusOnInsert(input, plantWorkerAbs.StartDateAbsent, plantWorkerAbs.EndDateAbsent, plantWorkerAbs.EmployeeID);
                        var item = _exe.InsertWorkerAbsenteeism_SP(plantWorkerAbs);
                        bulkData.New[i] = Mapper.Map<ExePlantWorkerAbsenteeismViewModel>(item);
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtlogger.Err(ex, new List<object> { plantWorkerAbs }, "SavePlantWorkerAbsenteeism");
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> { plantWorkerAbs }, "SavePlantWorkerAbsenteeism");
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
                    var plantWorkerAbs = Mapper.Map<ExePlantWorkerAbsenteeismDTO>(bulkData.Edit[i]);

                    //set updatedby
                    plantWorkerAbs.UpdatedBy = GetUserName();

                    try
                    {
                        //if (bulkData.Edit[i].StateOnEdit == "INSERT")
                        //    CheckEblekSubmitted(plantWorkerAbs.EmployeeID, plantWorkerAbs.StartDateAbsent, plantWorkerAbs.EndDateAbsent, locationCode, plantWorkerAbs.Shift.ToString(), unitCode, plantWorkerAbs.GroupCode, bulkData.Edit[i].StateOnEdit);
                        //else if (bulkData.Edit[i].StateOnEdit == "EDIT")
                        //    CheckEblekSubmitted(plantWorkerAbs.EmployeeID, plantWorkerAbs.OldValueStartDateAbsent, plantWorkerAbs.OldValueEndDateAbsent, locationCode, plantWorkerAbs.Shift.ToString(), unitCode, plantWorkerAbs.GroupCode, bulkData.Edit[i].StateOnEdit);

                        if (!pieceRateTab)
                        {
                            groupCode = _bll.GetMstEmployeeJobsDataActives(plantWorkerAbs.EmployeeID).GroupCode;
                        }

                        var input = new GetExePlantWorkerAbsenteeismExcelPieceRateInput
                        {
                            LocationCode = plantWorkerAbs.LocationCode,
                            UnitCode = plantWorkerAbs.UnitCode,
                            Shift = plantWorkerAbs.Shift,
                            Group = groupCode
                        };
                        CheckEblekStatusOnEdit(input, plantWorkerAbs.StartDateAbsent, plantWorkerAbs.OldValueStartDateAbsent, plantWorkerAbs.EndDateAbsent, plantWorkerAbs.OldValueEndDateAbsent, plantWorkerAbs.EmployeeID);
                        var item = _exe.UpdateWorkerAbsenteeism_SP(plantWorkerAbs);
                        bulkData.Edit[i] = Mapper.Map<ExePlantWorkerAbsenteeismViewModel>(item);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtlogger.Err(ex, new List<object> { bulkData.Edit }, "SavePlantWorkerAbsenteeism - Update");
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> { bulkData.Edit }, "SavePlantWorkerAbsenteeism - Update");
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    }
                }
            }
            return Json(bulkData);
        }

        [HttpPost]
        public Task<JsonResult> Upload(string modulePath = "")
        {
            return _uploadService.Upload(modulePath);
        }

        [HttpPost]
        public FileStreamResult GenerateExcelPieceRate(GetExePlantWorkerAbsenteeismExcelPieceRateInput input)
        {
            var inputWorkerAbsenteeism = new GetExePlantWorkerAbsenteeismInput
            {
                LocationCode = input.LocationCode,
                UnitCode = input.UnitCode,                               
                KPSYear = input.KPSYear,
                KPSWeek = input.KPSWeek,
                TransactionDate = input.Date,
                Shift = input.Shift,
                Process = input.Process,
                GroupCode = input.Group
            };

            var executionWorkerAbsenteeism = _exe.GetWorkerAbsenteeism(inputWorkerAbsenteeism);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.ExecuteExcelTemplate.ExecutePlantWorkerAbsenteeismPieceRate + ".xlsx";
            var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                
                //filter values
                slDoc.SetCellValue(3, 2, ": " + input.LocationCode + " - " + input.LocationName);
                //slDoc.SetCellValue(3, 3, );
                slDoc.SetCellValue(4, 2, ": " + input.UnitCode);
                slDoc.SetCellValue(5, 2, ": " + input.Shift);
                slDoc.SetCellValue(6, 2, ": " + input.Process);
                slDoc.SetCellValue(7, 2, ": " + input.Group);
                slDoc.SetCellValue(3, 7, ": " + input.KPSYear.ToString());
                slDoc.SetCellValue(4, 7, ": " + input.KPSWeek.ToString());
                slDoc.SetCellValue(5, 7, ": " + input.Date.ToString(Constants.DefaultDateOnlyFormat));

                //row values
                var iRow = 10;
                int totalRow = 0;

                foreach (var masterListGroup in executionWorkerAbsenteeism)
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
                        //style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, masterListGroup.EmployeeID);
                    slDoc.SetCellValue(iRow, 2, masterListGroup.EmployeeNumber.Substring(masterListGroup.EmployeeNumber.Length-2, 2) + " - " + masterListGroup.EmployeeName);
                    slDoc.SetCellValue(iRow, 3, masterListGroup.AbsentType);
                    slDoc.SetCellValue(iRow, 4, masterListGroup.SktAbsentCode);
                    slDoc.SetCellValue(iRow, 5, masterListGroup.PayrollAbsentCode);
                    slDoc.SetCellValue(iRow, 6, masterListGroup.StartDateAbsent.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(iRow, 7, masterListGroup.EndDateAbsent.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(iRow, 8, masterListGroup.ePaf);
                    slDoc.SetCellValue(iRow, 9, masterListGroup.AttachmentPath);
                    //slDoc.SetCellStyle(iRow, 1, iRow, 12, style);
                    slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
                    iRow++;
                    totalRow++;
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
            var fileName = "ProductionExecution_WorkerAbsenteeismPieceRate_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public FileStreamResult GenerateExcelDaily(GetExePlantWorkerAbsenteeismExcelDailyInput input)
        {
            var inputWorkerAbsenteeism = new GetExePlantWorkerAbsenteeismInput
            {
                LocationCode = input.LocationCode,
                UnitCode = input.UnitCode,
                KPSYear = input.KPSYear,
                KPSWeek = input.KPSWeek,
                TransactionDate = input.Date,
                Shift = input.Shift
            };

            var executionWorkerAbsenteeism = _exe.GetWorkerAbsenteeism(inputWorkerAbsenteeism);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.ExecuteExcelTemplate.ExecutePlantWorkerAbsenteeismDaily + ".xlsx";
            var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 2, ": " + input.LocationCode + " - " + input.LocationName);
                slDoc.SetCellValue(4, 2, ": " + input.UnitCode);
                slDoc.SetCellValue(5, 2, ": " + input.Shift);
                slDoc.SetCellValue(3, 7, ": " + input.KPSYear.ToString());
                slDoc.SetCellValue(4, 7, ": " + input.KPSWeek.ToString());
                slDoc.SetCellValue(5, 7, ": " + input.Date.ToString(Constants.DefaultDateOnlyFormat));

                //row values
                var iRow = 8;

                foreach (var masterListGroup in executionWorkerAbsenteeism)
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
                        //style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, masterListGroup.ProcessSettingsCode);
                    slDoc.SetCellValue(iRow, 2, masterListGroup.EmployeeID);
                    slDoc.SetCellValue(iRow, 3, masterListGroup.EmployeeName);
                    slDoc.SetCellValue(iRow, 4, masterListGroup.AbsentType);
                    slDoc.SetCellValue(iRow, 5, masterListGroup.SktAbsentCode);
                    slDoc.SetCellValue(iRow, 6, masterListGroup.PayrollAbsentCode);
                    slDoc.SetCellValue(iRow, 7, masterListGroup.StartDateAbsent.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(iRow, 8, masterListGroup.EndDateAbsent.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(iRow, 9, masterListGroup.ePaf);
                    slDoc.SetCellValue(iRow, 10, masterListGroup.AttachmentPath);
                    //slDoc.SetCellStyle(iRow, 1, iRow, 12, style);
                    slDoc.SetCellStyle(iRow, 1, iRow, 10, style);
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
            var fileName = "ProductionExecution_WorkerAbsenteeismDaily_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public JsonResult GetProductionCardSubmitted(string employeeID, 
            string startDateAbsent,
            string endDateAbsent,
            string locationCode,
            string shift,
            string unitCode,
            string group)
        {
            var absenteeismInput = new GetExePlantWorkerAbsenteeismInput
            {
                EmployeeID = employeeID,
                StartDateAbsent = String.IsNullOrEmpty(startDateAbsent) ? new DateTime() : Convert.ToDateTime(startDateAbsent),
                EndDateAbsent = String.IsNullOrEmpty(endDateAbsent) ? new DateTime() : Convert.ToDateTime(endDateAbsent)
            };
            var listProdCard = _exe.GetProductionCardSubmitted(absenteeismInput).ToList();
            var arrOutput = new string[0];
            if (listProdCard.Any())
            {
                var arrData = new string[3];
                arrData[0] = listProdCard.Select(c => c.EmployeeID).FirstOrDefault(); // EmployeeID
                arrData[1] = listProdCard.Select(c => c.ProductionDate).Max().ToShortDateString(); // EndDate
                arrData[2] = listProdCard.Select(c => c.ProductionDate).Min().ToShortDateString(); // StartDate
                return Json(arrData, JsonRequestBehavior.AllowGet);
            }

            // check eblek submit translog
            while (absenteeismInput.StartDateAbsent <= absenteeismInput.EndDateAbsent)
            {
                //get brand
                var brand = _exe.GetPlantProductionEntryBrand(employeeID, absenteeismInput.StartDateAbsent, shift, unitCode, group);

                var yearWeek = _bll.GetWeekByDate(absenteeismInput.StartDateAbsent);

                DateTime date = absenteeismInput.StartDateAbsent;
                int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                          + locationCode + "/"
                                          + shift + "/"
                                          + unitCode + "/"
                                          + group + "/"
                                          + brand + "/"
                                          + yearWeek.Year + "/"
                                          + yearWeek.Week + "/"
                                          + day;

                var translog = _utilitiesBLL.CheckDataAlreadyOnTransactionLog(productionEntryCode, HMS.SKTIS.Core.Enums.PageName.PlantProductionEntry.ToString(), HMS.SKTIS.Core.Enums.ButtonName.Submit.ToString());
                if (translog)
                {
                    var arrData = new string[1];
                    arrData[0] = "eblekSubmited";
                    return Json(arrData, JsonRequestBehavior.AllowGet);
                }

                absenteeismInput.StartDateAbsent = absenteeismInput.StartDateAbsent.AddDays(1);
            }

            // return array to build notif "EmployeeID - Range Date"
            return Json(arrOutput, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllAbsentTypeIsfromEntryOrNot()
        {
            var model = _bll.GetAllAbsentTypesActiveInEblek();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private void CheckEblekSubmitted(string employeeID, DateTime startDateAbsent, DateTime endDateAbsent, string locationCode, string shift, string unitCode, string group, string state)
        {
            var absenteeismInput = new GetExePlantWorkerAbsenteeismInput
            {
                EmployeeID = employeeID,
                StartDateAbsent = startDateAbsent,
                EndDateAbsent = endDateAbsent
            };

            if (state == "INSERT")
            {
                
                var listProdCard = _exe.GetProductionCardSubmitted(absenteeismInput).ToList();
                var arrOutput = new string[0];
                if (listProdCard.Any())
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.EblekSubmitted, "There is <strong> Already Submitted Production Entry </strong> for This Employee in Selected Range Date.");
                }

                // check eblek submit translog
                while (absenteeismInput.StartDateAbsent <= absenteeismInput.EndDateAbsent)
                {
                    //get brand
                    var brand = _exe.GetPlantProductionEntryBrand(employeeID, absenteeismInput.StartDateAbsent, shift, unitCode, group);

                    var yearWeek = _bll.GetWeekByDate(absenteeismInput.StartDateAbsent);

                    DateTime date = absenteeismInput.StartDateAbsent;
                    int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                    var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                              + locationCode + "/"
                                              + shift + "/"
                                              + unitCode + "/"
                                              + group + "/"
                                              + brand + "/"
                                              + yearWeek.Year + "/"
                                              + yearWeek.Week + "/"
                                              + day;

                    var translog = _utilitiesBLL.GetLatestEblekProductionEntryTransLog(productionEntryCode);
                    var alreadySubmitted = translog != null ? translog.UtilFlow.UtilFunction.FunctionName == HMS.SKTIS.Core.Enums.ButtonName.Submit.ToString() : false;

                    if (alreadySubmitted)
                    {
                        throw new BLLException(ExceptionCodes.BLLExceptions.EblekSubmitted, "There is <strong> Already Submitted Production Entry </strong> for This Employee in Selected Range Date.");
                    }

                    absenteeismInput.StartDateAbsent = absenteeismInput.StartDateAbsent.AddDays(1);
                }
            }
            else if(state == "EDIT")
            {
                // check eblek submit translog

                //get brand
                var brand = _exe.GetPlantProductionEntryBrand(employeeID, absenteeismInput.StartDateAbsent, shift, unitCode, group);

                var yearWeek = _bll.GetWeekByDate(absenteeismInput.EndDateAbsent);

                DateTime date = absenteeismInput.EndDateAbsent;
                int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                          + locationCode + "/"
                                          + shift + "/"
                                          + unitCode + "/"
                                          + group + "/"
                                          + brand + "/"
                                          + yearWeek.Year + "/"
                                          + yearWeek.Week + "/"
                                          + day;

                var translog = _utilitiesBLL.GetLatestEblekProductionEntryTransLog(productionEntryCode);
                var alreadySubmitted = translog != null ? translog.UtilFlow.UtilFunction.FunctionName == HMS.SKTIS.Core.Enums.ButtonName.Submit.ToString() : false;

                if (alreadySubmitted)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.EblekSubmitted, "There is <strong> Already Submitted Production Entry </strong> for This Employee in Selected Range Date.");
                }
            }
        }

        private void CheckEblekStatusOnInsert(GetExePlantWorkerAbsenteeismExcelPieceRateInput input, DateTime newStartAbsentDate, DateTime newEndAbsentDate, string employeeID) 
        {
            _exe.CheckEblekStatusOnInsertAbsenteeism(input, newStartAbsentDate, newEndAbsentDate, employeeID);
        }

        private void CheckEblekStatusOnEdit(GetExePlantWorkerAbsenteeismExcelPieceRateInput input, DateTime newStartAbsentDate, DateTime oldStartDateAbsent, DateTime newEndAbsentDate, DateTime oldEndAbsentDate, string employeeID) 
        {
            _exe.CheckEblekStatusOnEditAbsenteeism(input, newStartAbsentDate, oldStartDateAbsent, newEndAbsentDate, oldEndAbsentDate, employeeID);
        }

        [HttpPost]
        public ActionResult SaveMultipleAbsenteeism(InsertMultipleAbsenteeism postDataAbsenteeism)
        {
            var dto = Mapper.Map<InsertMultipleAbsenteeismDTO>(postDataAbsenteeism);

            dto.CreatedBy = GetUserName();
            dto.UpdatedBy = GetUserName();

            var listEmployeeValidate = _exe.InsertMultipleAbsenteeism(dto);
            return Json(listEmployeeValidate);
        }
    }
}