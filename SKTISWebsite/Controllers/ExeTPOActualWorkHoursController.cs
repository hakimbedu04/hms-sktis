using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SKTISWebsite.Models.ExeTPOActualWorkHours;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using AutoMapper;
using SKTISWebsite.Models.ExePlantActualWorkHours;
using SKTISWebsite.Models.Common;
using HMS.SKTIS.Utils;
using System.IO;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SKTISWebsite.Controllers
{
    public class ExeTPOActualWorkHoursController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionPlantBLL _executionPlantBll;
        private IExecutionTPOBLL _executionTpobll;
        private IVTLogger _vtlogger;

        public ExeTPOActualWorkHoursController(IApplicationService applicationService, IVTLogger vtlogger, IExecutionTPOBLL executionTpobll, IMasterDataBLL masterDataBll, IExecutionPlantBLL executionPlantBll)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _executionPlantBll = executionPlantBll;
            _executionTpobll = executionTpobll;
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/TPO/TPOActualWorksHours");
        }
        
        // GET: /ExeTPOActualWorkHours/
        public ActionResult Index()
        {
            var init = new InitExeTPOActualWorkHoursViewModel()
            {
                TpoLocationLookupLists = _svc.GetLastLocationChildList(Enums.LocationCode.TPO.ToString()),
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now)

            };

            return View("Index", init);
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
            var date = _masterDataBLL.GetWeekByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmpStatusNested(string locationCode, string brandCode, string date)
        {
            DateTime prodDate = Convert.ToDateTime(date);
            var empStatus = _executionTpobll.GetEmpStatusFromExeTPOProductionEntry(locationCode, brandCode, prodDate);
            return Json(empStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandFromExeTPOProductionEntryVerificationByLocation(string locationCode, string date)
        {
            DateTime prodDate = new DateTime();
            try
            {
                prodDate = Convert.ToDateTime(date);
            }
            catch
            {
                prodDate = DateTime.Today;
            }
            var brands = _executionTpobll.GetBrandCodeFromExeTPOProductionEntryVerificationByLocationDate(locationCode, prodDate);
            return Json(brands, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetExeTPOActualWorkHours(GetExePlantActualWorkHoursInput criteria)
        {
            try
            {
                criteria.Shift = 1;
                criteria.UnitCode = Constants.TPOUnitCode;

                //var masterLists = _executionPlantBll.GetExePlantActualWorkHours(criteria);
                var masterLists = _executionTpobll.GetExeTpoActualWorkHours(criteria);
                var viewModel = Mapper.Map<List<ExeTPOActualWorkHoursViewModel>>(masterLists);
                var pageResult = new PageResult<ExeTPOActualWorkHoursViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Exe TPO Actual Work Hours - GetExeTPOActualWorkHours");
                return null;
            }
        }

        /// <summary>
        /// Save all new and updated actual work hours
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllExeTPOActualWorkHours(InsertUpdateData<ExeTPOActualWorkHoursViewModel> bulkData)
        {
            try
            {
                if (bulkData.New != null)
                {
                    int err = 0;

                    for (var i = 0; i < bulkData.New.Count; i++)
                    {
                        //check row is null
                        if (bulkData.New[i] == null) continue;

                        var postData = Mapper.Map<ExeTPOActualWorkHoursDTO>(bulkData.New[i]);
                        if (postData.TimeOut < postData.TimeIn)
                        {
                            bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = "Out can't less than IN";
                            err = 1;
                        }
                    }

                    if (err == 0)
                    {
                        for (var i = 0; i < bulkData.New.Count; i++)
                        {
                            //check row is null
                            if (bulkData.New[i] == null) continue;

                            var postData = Mapper.Map<ExeTPOActualWorkHoursDTO>(bulkData.New[i]);

                            postData.UnitCode = Constants.TPOUnitCode;
                            postData.StatusEmp = bulkData.Parameters["StatusEmp"];
                            postData.UpdatedBy = GetUserName();
                            postData.ProductionDate = DateTime.Parse(bulkData.Parameters["Date"]);

                            try
                            {
                                var item = _executionTpobll.InsertUpdateExeTpoActualWorkHours(postData);
                                bulkData.New[i] = Mapper.Map<ExeTPOActualWorkHoursViewModel>(item);
                                bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                            }
                            catch (ExceptionBase ex)
                            {
                                _vtlogger.Err(ex, new List<object> {bulkData, postData},
                                    "Exe TPO Actual Work Hours - SaveAllExePlantActualWorkHours - Update");
                                bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                                bulkData.New[i].Message = ex.Message;
                            }
                            catch (Exception ex)
                            {
                                _vtlogger.Err(ex, new List<object> {bulkData, postData},
                                    "Exe TPO Actual Work Hours - SaveAllExePlantActualWorkHours - Update");
                                bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                                bulkData.New[i].Message =
                                    EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                                ;
                            }
                        }
                    }
                }

                return Json(bulkData);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData }, "Exe TPO Actual Work Hours - SaveAllExePlantActualWorkHours");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string brand, int year, int week, DateTime date, string empStatus)
        {
            var input = new GetExePlantActualWorkHoursInput
            {
                LocationCode = locationCode,
                UnitCode = Constants.TPOUnitCode,
                Shift = 1,
                Brand = brand,
                Year = year,
                Week = week,
                Date = date,
                SortOrder = "ASC",
                SortExpression = "ProcessOrder",
                StatusEmp = empStatus
            };

            try
            {
                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == locationCode)
                    {
                        locationCompat = item.Text;
                    }
                }

                var executionPlantActualWorkHours = _executionTpobll.GetExeTpoActualWorkHours(input);

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExecuteTPOActualWorkHours + ".xlsx";
                var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    slDoc.SetCellValue(4, 2, ": " + brand);
                    slDoc.SetCellValue(5, 2, ": " + empStatus);
                    slDoc.SetCellValue(3, 4, ": " + year.ToString());
                    slDoc.SetCellValue(4, 4, ": " + week.ToString());
                    slDoc.SetCellValue(5, 4, ": " + date.ToString(Constants.DefaultDateOnlyFormat));

                    //row values
                    var iRow = 8;

                    foreach (var masterListGroup in executionPlantActualWorkHours)
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

                        slDoc.SetCellValue(iRow, 1, masterListGroup.ProcessGroup);
                        slDoc.SetCellValue(iRow, 2,
                            masterListGroup.TimeIn.HasValue
                                ? masterListGroup.TimeIn.ToString()
                                : Constants.DefaultTimeIn);
                        slDoc.SetCellValue(iRow, 3,
                            masterListGroup.BreakTime.HasValue
                                ? masterListGroup.BreakTime.ToString()
                                : Constants.DefaultBreakTime);
                        slDoc.SetCellValue(iRow, 4,
                            masterListGroup.TimeOut.HasValue
                                ? masterListGroup.TimeOut.ToString()
                                : Constants.DefaultTimeOut);

                        slDoc.SetCellStyle(iRow, 1, iRow, 4, style);
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
                    slDoc.AutoFitColumn(1, 4);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = " ProductionExecution_TPOActualWorkHours_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, brand, year, week, date, empStatus }, "Exe TPO Actual Work Hours - Generate Excel");
                return null;
            }
        }
	}
}