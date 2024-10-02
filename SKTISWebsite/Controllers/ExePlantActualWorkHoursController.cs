using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using SKTISWebsite.Models.ExePlantActualWorkHours;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using HMS.SKTIS.Contracts;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using SKTISWebsite.Models.Common;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.Utils;

namespace SKTISWebsite.Controllers
{
    public class ExePlantActualWorkHoursController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionPlantBLL _executionPlantBll;
        private IVTLogger _vtlogger;

        public ExePlantActualWorkHoursController(IApplicationService applicationService, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IExecutionPlantBLL executionPlantBll)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _executionPlantBll = executionPlantBll;
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/Plant/ActualWorkHours");
        }
        
        // GET: ExePlantActualWorkHours
        public ActionResult Index()
        {
            var init = new InitExePlantActualWorkHoursViewModel()
            {
                PLNTChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now)

            };

            return View("Index", init);
        }
        [HttpGet]
        public JsonResult GetPlantLocationCode()
        {
            var locCodes = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            return Json(locCodes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetExePlantActualWorkHours(GetExePlantActualWorkHoursInput criteria)
        {
            try
            {
                var masterLists = _executionPlantBll.GetExePlantActualWorkHours(criteria);
                var viewModel = Mapper.Map<List<ExePlantActualWorkHoursViewModel>>(masterLists);
                var pageResult = new PageResult<ExePlantActualWorkHoursViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Get Exe Plant Actual Work Hours on ");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetUnitCodeSelectListByLocationCode(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShiftByLocation(string locationCode)
        {
            var shifts = _svc.GetShiftByLocationCode(locationCode);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateByYearWeek(int year, int week)
        {
            var date = _masterDataBLL.GetWeekByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandFromExePlantProductionEntryVerificationByLocation(string locationCode, string unitCode, int shift, int year, int week, string date)
        {
            DateTime productionDate = DateTime.ParseExact(date, Constants.DefaultDateOnlyFormat,
                System.Globalization.CultureInfo.InvariantCulture);
            var model = _executionPlantBll.GetBrandFromPlantEntryVerificationByDate(locationCode, unitCode, shift, year, week, productionDate);
            //var model = _masterDataBLL.GetBrandCodeByLocationCode(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetGenEmpStatusListByLocationCode(string locationCode)
        {
            try
            {
                var model = _svc.GetStatusByParentLocationCode(locationCode);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode }, "GetGenEmpStatusListByLocationCode");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string unitCode, int shift, string status, string brand, int year, int week, DateTime date)
        {
            try
            {
                var input = new GetExePlantActualWorkHoursInput
                {
                    LocationCode = locationCode,
                    UnitCode = unitCode,
                    Shift = shift,
                    Brand = brand,
                    Year = year,
                    Week = week,
                    Date = date,
                    SortOrder = "ASC",
                    SortExpression = "ProcessOrder"
                };

                var allLocations = _svc.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == locationCode)
                    {
                        locationCompat = item.Text;
                    }
                }

                var executionPlantActualWorkHours = _executionPlantBll.GetExePlantActualWorkHours(input);

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExecutePlantActualWorkHours + ".xlsx";
                var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    slDoc.SetCellValue(4, 2, ": " + unitCode);
                    slDoc.SetCellValue(5, 2, ": " + shift.ToString());
                    slDoc.SetCellValue(6, 2, ": " + brand);
                    slDoc.SetCellValue(3, 4, ": " + year.ToString());
                    slDoc.SetCellValue(4, 4, ": " + week.ToString());
                    slDoc.SetCellValue(5, 4, ": " + date.ToString(Constants.DefaultDateOnlyFormat));

                    //row values
                    var iRow = 9;

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
                var fileName = " ProductionExecution_ActualWorkHours_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, shift, status, brand, year, week, date }, "Exe Plant Actual Work Hours - Excel");
                return null;
            }
        }

        /// <summary>
        /// Save all new and updated actual work hours
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllExePlantActualWorkHours(InsertUpdateData<ExePlantActualWorkHoursViewModel> bulkData)
        {
            try
            {
                if (bulkData.Edit != null)
                {
                    int err = 0;

                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;

                        var postData = Mapper.Map<ExePlantActualWorkHoursDTO>(bulkData.Edit[i]);
                        if (postData.TimeOut < postData.TimeIn)
                        {
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = "Out can't less than IN";
                            err = 1;
                        }
                    }

                    if (err == 0)
                    {
                        for (var i = 0; i < bulkData.Edit.Count; i++)
                        {
                            //check row is null
                            if (bulkData.Edit[i] == null) continue;

                            var postData = Mapper.Map<ExePlantActualWorkHoursDTO>(bulkData.Edit[i]);

                            postData.UpdatedBy = GetUserName();
                            postData.ProductionDate = DateTime.Parse(bulkData.Parameters["Date"]);

                            try
                            {
                                var item = _executionPlantBll.InsertUpdateExePlantActualWorkHours(postData);
                                bulkData.Edit[i] = Mapper.Map<ExePlantActualWorkHoursViewModel>(item);
                                bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                            }
                            catch (ExceptionBase ex)
                            {
                                _vtlogger.Err(ex, new List<object> {postData},
                                    "Exe Plant Actual Work Hours - SaveAllExePlantActualWorkHours - Update");
                                bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                                bulkData.Edit[i].Message = ex.Message;
                            }
                            catch (Exception ex)
                            {
                                _vtlogger.Err(ex, new List<object> {postData},
                                    "Exe Plant Actual Work Hours - SaveAllExePlantActualWorkHours - Update");
                                bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                                bulkData.Edit[i].Message =
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
                _vtlogger.Err(ex, new List<object> { bulkData }, "Exe Plant Actual Work Hours - Save");
                return null;
            }
        }
	}
}