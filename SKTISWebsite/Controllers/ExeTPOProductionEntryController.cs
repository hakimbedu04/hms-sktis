using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.ExeTPOProductionEntry;
using Enums = HMS.SKTIS.Core.Enums;
using AutoMapper;
using SKTISWebsite.Models.Common;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using System.Globalization;
using HMS.SKTIS.BusinessObjects.Inputs;
using SKTISWebsite.Models.UtilTransactionLog;
using Excel;
using System.Data;
using System.Net;

namespace SKTISWebsite.Controllers
{
    public class ExeTPOProductionEntryController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionPlantBLL _executionPlantBll;
        private IExecutionTPOBLL _executionTpobll;
        private IPlanningBLL _planningbll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;

        public ExeTPOProductionEntryController
        (
            IApplicationService applicationService, 
            IMasterDataBLL masterDataBll,
            IVTLogger vtlogger,
            IExecutionTPOBLL executionTpobll, 
            IExecutionPlantBLL executionPlantBll,
            IPlanningBLL planningbll, 
            IGeneralBLL generalBll,
            IUtilitiesBLL utilitiesBLL
        )
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _vtlogger = vtlogger;
            _executionTpobll = executionTpobll;
            _executionPlantBll = executionPlantBll;
            _planningbll = planningbll;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            SetPage("ProductionExecution/TPO/TPOProductionEntry");
        }

        // GET: ExeTPOProductionEntry
        public ActionResult Index(string param1, string param2, string param3, string param4, int? param5, int? param6, string param7, int? param8)
        {
            if(param8.HasValue) setResponsibility(param8.Value);
            var init = new InitExeTPOProductionEntryViewModel
            {
                YearSelectList = _svc.GetGenWeekYears(),
                PLNTChildLocationLookupList = _svc.GetLastLocationChildList(Enums.LocationCode.TPO.ToString()),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                AbsentTypes = _svc.GetAllAbsentTypes(),
                Param1LocationCode = param1,
                Param2ProcessGroup = param2,
                Param3StatusEmp = param3,
                Param4BrandCode = param4,
                Param5KPSYear = param5,
                Param6KPSWeek = param6,
                Param7Date = String.IsNullOrEmpty(param7) ? DateTime.Now.Date : DateTime.ParseExact(param7, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };
            return View("index", init);
        }

        [HttpGet]
        public JsonResult GetProcess(string locationCode, int year, int week)
        {
            var process = _svc.GetAllProcessGroupFromMstGenProcSettLocAndMstGenProcSettAnfMstGenProcByLocation(locationCode, year, week);
            // exclude daily http://tp.voxteneo.com/entity/59002
            var processFiltered = process.Where(m => m.Text != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily));
            return Json(processFiltered, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcessByLocationCodeAndDate(string locationCode, int year, int week, string date)
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
            var process = _svc.GetAllProcessFromExeTPOProductionVerification(locationCode, year, week, prodDate);
            var processFiltered = process.Where(m => m.Text != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily));
            return Json(processFiltered, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStatusEmp(string locationCode)
        {
            var process = _svc.GetAllStatusEmpByLocationCode(locationCode);
            return Json(process, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getStatusSelectListByLocationCodeAndDate(string locationCode ,string date, string BrandCode, string ProcessGroup)
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

            var status = _executionTpobll.GetStatusEmpActiveByLocationAndDate(locationCode, prodDate, BrandCode, ProcessGroup);
            //var status = _executionTpobll.GetStatusEmpActiveByLocationAndDateTPOTPK(locationCode, prodDate);
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrand(string locationCode, int Year, int Week)
        {
            var input = new GetExeTPOProductionEntryVerificationInput
            {
                LocationCode = locationCode,
                KPSYear = Year,
                KPSWeek = Week
            };

            var brands = _executionTpobll.GetBrandCodeFromExeTPOProductionEntryVerification(input);
            return Json(brands, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBrandByLocationCodeAndDate(string locationCode, int Year, int Week, string date)
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

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateByYearWeek(int year, int week)
        {
            var date = _svc.GetSelectListDateByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetTargetManual(string locationCode,string Process,string Status,string Brand,string Year,string Week,string Date)
        {
            try
            {
                var input = new GetTPOTPKInput
                {
                    LocationCode = locationCode,
                    KPSWeek = Convert.ToInt32(Week),
                    KPSYear = Convert.ToInt32(Year),
                    BrandCode = Brand
                };
                DateTime checkDate = DateTime.ParseExact(Date, Constants.DefaultDateOnlyFormat,
                    CultureInfo.InvariantCulture);
                int checkDay = (int) checkDate.DayOfWeek;
                if (checkDay == 1)
                {
                    var dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(input)
                            .Where(m => m.StatusEmp == Status)
                            .Where(m => m.ProcessGroup == Process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual1});
                    return Json(dataPlanningTPK, JsonRequestBehavior.AllowGet);
                }
                else if (checkDay == 2)
                {
                    var dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(input)
                            .Where(m => m.StatusEmp == Status)
                            .Where(m => m.ProcessGroup == Process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual2});
                    return Json(dataPlanningTPK, JsonRequestBehavior.AllowGet);
                }
                else if (checkDay == 3)
                {
                    var dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(input)
                            .Where(m => m.StatusEmp == Status)
                            .Where(m => m.ProcessGroup == Process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual3});
                    return Json(dataPlanningTPK, JsonRequestBehavior.AllowGet);
                }
                else if (checkDay == 4)
                {
                    var dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(input)
                            .Where(m => m.StatusEmp == Status)
                            .Where(m => m.ProcessGroup == Process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual4});
                    return Json(dataPlanningTPK, JsonRequestBehavior.AllowGet);
                }
                else if (checkDay == 5)
                {
                    var dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(input)
                            .Where(m => m.StatusEmp == Status)
                            .Where(m => m.ProcessGroup == Process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual5});
                    return Json(dataPlanningTPK, JsonRequestBehavior.AllowGet);
                }
                else if (checkDay == 6)
                {
                    var dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(input)
                            .Where(m => m.StatusEmp == Status)
                            .Where(m => m.ProcessGroup == Process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual6});
                    return Json(dataPlanningTPK, JsonRequestBehavior.AllowGet);
                }
                else if (checkDay == 0)
                {
                    var dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(input)
                            .Where(m => m.StatusEmp == Status)
                            .Where(m => m.ProcessGroup == Process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual7});
                    return Json(dataPlanningTPK, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(input)
                            .Where(m => m.StatusEmp == Status)
                            .Where(m => m.ProcessGroup == Process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual1});
                    return Json(dataPlanningTPK, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, Process, Status, Brand, Year, Week, Date }, "Exe TPO Production Entry - GetTargetManual");
                return null;
            }

        }

        [HttpPost]
        public JsonResult GetExeTPOProductionEntry(GetExeTPOProductionInput criteria)
        {
            try
            {
                var pageResult = new PageResult<ExeTPOProductionEntryViewModel>();
                var masterLists = _executionTpobll.GetExeTPOProductionEntry(criteria);
                pageResult.Results = Mapper.Map<List<ExeTPOProductionEntryViewModel>>(masterLists);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Exe TPO Production Entry - GetExeTPOProductionEntry");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetTpkValue(string locationCode, string brand, int year, int week, DateTime date, string process, string status)
        {
            try
            {
                var weekStartDate = _masterDataBLL.GetFirstDateByYearWeek(year, week);
                var plantTPK = _executionTpobll.GetTpoTpkValue(locationCode, brand, year, week, weekStartDate, process,
                    status);
                float? tpkValue = 0;
                if (plantTPK != null)
                {
                    int number = (int) date.DayOfWeek;
                    switch (number)
                    {
                        case 1:
                            tpkValue = plantTPK.Sum(m => m.TargetManual1);
                            break;
                        case 2:
                            tpkValue = plantTPK.Sum(m => m.TargetManual2);
                            break;
                        case 3:
                            tpkValue = plantTPK.Sum(m => m.TargetManual3);
                            break;
                        case 4:
                            tpkValue = plantTPK.Sum(m => m.TargetManual4);
                            break;
                        case 5:
                            tpkValue = plantTPK.Sum(m => m.TargetManual5);
                            break;
                        case 6:
                            tpkValue = plantTPK.Sum(m => m.TargetManual6);
                            break;
                        case 0:
                            tpkValue = plantTPK.Sum(m => m.TargetManual7);
                            break;
                    }
                }
                return Json(tpkValue, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, brand, year, week, date, process, status }, "Exe TPO Production Entry - GetTpkValue");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string process, string status, string brand, int year, int week, DateTime date)
        {
            try
            {
                var input = new GetExeTPOProductionInput
                {
                    LocationCode = locationCode,
                    Process = process,
                    Status = status,
                    Brand = brand,
                    Year = year,
                    Week = week,
                    Date = date
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

                var inputCheck = new GetTPOTPKInput
                {
                    LocationCode = locationCode,
                    KPSWeek = Convert.ToInt32(week),
                    KPSYear = Convert.ToInt32(year),
                    BrandCode = brand
                };
                var dataPlanningTPK =
                    _planningbll.GetPlanningTPOTPK(inputCheck)
                        .Where(m => m.StatusEmp == status)
                        .Where(m => m.ProcessGroup == process)
                        .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual1});
                ;
                var executionTPOProductionEntrys = _executionTpobll.GetExeTPOProductionEntry(input);
                int checkDay = (int) date.DayOfWeek;
                if (checkDay == 1)
                {
                    dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(inputCheck)
                            .Where(m => m.StatusEmp == status)
                            .Where(m => m.ProcessGroup == process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual1});
                }
                else if (checkDay == 2)
                {
                    dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(inputCheck)
                            .Where(m => m.StatusEmp == status)
                            .Where(m => m.ProcessGroup == process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual2});
                }
                else if (checkDay == 3)
                {
                    dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(inputCheck)
                            .Where(m => m.StatusEmp == status)
                            .Where(m => m.ProcessGroup == process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual3});
                }
                else if (checkDay == 4)
                {
                    dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(inputCheck)
                            .Where(m => m.StatusEmp == status)
                            .Where(m => m.ProcessGroup == process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual4});
                }
                else if (checkDay == 5)
                {
                    dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(inputCheck)
                            .Where(m => m.StatusEmp == status)
                            .Where(m => m.ProcessGroup == process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual5});
                }
                else if (checkDay == 6)
                {
                    dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(inputCheck)
                            .Where(m => m.StatusEmp == status)
                            .Where(m => m.ProcessGroup == process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual6});
                }
                else if (checkDay == 0)
                {
                    dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(inputCheck)
                            .Where(m => m.StatusEmp == status)
                            .Where(m => m.ProcessGroup == process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual7});
                }
                else
                {
                    dataPlanningTPK =
                        _planningbll.GetPlanningTPOTPK(inputCheck)
                            .Where(m => m.StatusEmp == status)
                            .Where(m => m.ProcessGroup == process)
                            .Select(m => new {m.ProdGroup, targetmanual = m.TargetManual1});
                }

                var checkedData = dataPlanningTPK.ToList();
                for (int i = 0; i < executionTPOProductionEntrys.Count(); i++)
                {
                    for (int j = 0; j < checkedData.Count; j++)
                    {
                        if (executionTPOProductionEntrys[i].ProductionGroup == checkedData[j].ProdGroup)
                        {
                            executionTPOProductionEntrys[i].TotalTargetManual = checkedData[j].targetmanual;
                        }
                    }
                }

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                //excel template for multiskill status
                if (status == "Multiskill")
                {
                    var templateFile = Enums.ExecuteExcelTemplate.ExecuteTPOProductionEntryMultiskill + ".xlsx";
                    var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                    if (System.IO.File.Exists(templateFileName))
                    {
                        System.IO.File.Copy(templateFileName, strFileName);
                    }

                    using (SLDocument slDoc = new SLDocument(strFileName))
                    {
                        //filter values
                        slDoc.SetCellValue(3, 2, ": " + locationCompat);
                        slDoc.SetCellValue(4, 2, ": " + process);
                        slDoc.SetCellValue(5, 2, ": " + status);
                        slDoc.SetCellValue(6, 2, ": " + brand);
                        slDoc.SetCellValue(3, 5, ": " + year.ToString());
                        slDoc.SetCellValue(4, 5, ": " + week.ToString());
                        slDoc.SetCellValue(5, 5, ": " + date.ToString("dd/MM/yyyy"));

                        //row values
                        var iRow = 11;

                        int WorkerCountTotal = 0;
                        int AbsentTotal = 0;
                        float ActualProductionTotal = 0;
                        float TotalTargetManualTotal = 0;

                        foreach (var masterListGroup in executionTPOProductionEntrys)
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

                            slDoc.SetCellValue(iRow, 1, masterListGroup.ProductionGroup);
                            //slDoc.SetCellValue(iRow, 2, masterListGroup.WorkerCount.HasValue ? (int) masterListGroup.WorkerCount : 0);
                            slDoc.SetCellValue(iRow, 2,
                                masterListGroup.Absent.HasValue ? (int) masterListGroup.Absent : 0);
                            slDoc.SetCellValue(iRow, 3,
                                String.Format(CultureInfo.CurrentCulture, "{0:n2}",
                                    masterListGroup.ActualProduction.HasValue
                                        ? masterListGroup.ActualProduction.Value
                                        : 0));
                            //slDoc.SetCellValue(iRow, 5, String.Format(CultureInfo.CurrentCulture, "{0:n2}", masterListGroup.TotalTargetManual.HasValue ? (float) masterListGroup.TotalTargetManual : 0));
                            slDoc.SetCellStyle(iRow, 1, iRow, 3, style);
                            iRow++;

                            //WorkerCountTotal = WorkerCountTotal +(masterListGroup.WorkerCount.HasValue ? (int) masterListGroup.WorkerCount : 0);
                            AbsentTotal = AbsentTotal +
                                          (masterListGroup.Absent.HasValue ? (int) masterListGroup.Absent : 0);
                            ActualProductionTotal = ActualProductionTotal +
                                                    (masterListGroup.ActualProduction.HasValue
                                                        ? masterListGroup.ActualProduction.Value
                                                        : 0);
                            //TotalTargetManualTotal = TotalTargetManualTotal + (masterListGroup.TotalTargetManual.HasValue ? (float) masterListGroup.TotalTargetManual : 0);
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
                        //slDoc.SetCellValue(iRow, 2, WorkerCountTotal);
                        slDoc.SetCellValue(iRow, 2, AbsentTotal);
                        slDoc.SetCellValue(iRow, 3,
                            String.Format(CultureInfo.CurrentCulture, "{0:n2}", ActualProductionTotal));
                        //slDoc.SetCellValue(iRow, 5, String.Format(CultureInfo.CurrentCulture, "{0:n2}", TotalTargetManualTotal));
                        slDoc.SetCellStyle(iRow, 1, iRow, 3, totalStyle);

                        slDoc.SetCellValue(6, 5,
                            ": " + String.Format(CultureInfo.CurrentCulture, "{0:n2}", TotalTargetManualTotal));
                        slDoc.SetCellValue(7, 5,
                            ": " + String.Format(CultureInfo.CurrentCulture, "{0:n2}", ActualProductionTotal));

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
                        //slDoc.AutoFitColumn(1, 5);

                        System.IO.File.Delete(strFileName);
                        slDoc.SaveAs(ms);
                    }
                    // this is important. Otherwise you get an empty file
                    // (because you'd be at EOF after the stream is written to, I think...).
                    //ms.Position = 0;
                    //var fileName = "ProductionExecution_TPOProductionEntry_" + DateTime.Now.ToShortDateString() + ".xlsx";
                    //return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
                else //excel template for not multiskill status
                {
                    var templateFile = Enums.ExecuteExcelTemplate.ExecuteTPOProductionEntry + ".xlsx";
                    var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                    if (System.IO.File.Exists(templateFileName))
                    {
                        System.IO.File.Copy(templateFileName, strFileName);
                    }

                    using (SLDocument slDoc = new SLDocument(strFileName))
                    {
                        //filter values
                        slDoc.SetCellValue(3, 2, ": " + locationCompat);
                        slDoc.SetCellValue(4, 2, ": " + process);
                        slDoc.SetCellValue(5, 2, ": " + status);
                        slDoc.SetCellValue(6, 2, ": " + brand);
                        slDoc.SetCellValue(3, 5, ": " + year.ToString());
                        slDoc.SetCellValue(4, 5, ": " + week.ToString());
                        slDoc.SetCellValue(5, 5, ": " + date.ToString("dd/MM/yyyy"));

                        //row values
                        var iRow = 11;

                        int WorkerCountTotal = 0;
                        int AbsentTotal = 0;
                        float ActualProductionTotal = 0;
                        float TotalTargetManualTotal = 0;

                        foreach (var masterListGroup in executionTPOProductionEntrys)
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

                            slDoc.SetCellValue(iRow, 1, masterListGroup.ProductionGroup);
                            slDoc.SetCellValue(iRow, 2,
                                masterListGroup.WorkerCount.HasValue ? (int) masterListGroup.WorkerCount : 0);
                            slDoc.SetCellValue(iRow, 3,
                                masterListGroup.Absent.HasValue ? (int) masterListGroup.Absent : 0);
                            slDoc.SetCellValue(iRow, 4,
                                String.Format(CultureInfo.CurrentCulture, "{0:n2}",
                                    masterListGroup.ActualProduction.HasValue
                                        ? masterListGroup.ActualProduction.Value
                                        : 0));
                            slDoc.SetCellValue(iRow, 5,
                                String.Format(CultureInfo.CurrentCulture, "{0:n2}",
                                    masterListGroup.TotalTargetManual.HasValue
                                        ? (float) masterListGroup.TotalTargetManual
                                        : 0));
                            slDoc.SetCellStyle(iRow, 1, iRow, 5, style);
                            iRow++;

                            WorkerCountTotal = WorkerCountTotal +
                                               (masterListGroup.WorkerCount.HasValue
                                                   ? (int) masterListGroup.WorkerCount
                                                   : 0);
                            AbsentTotal = AbsentTotal +
                                          (masterListGroup.Absent.HasValue ? (int) masterListGroup.Absent : 0);
                            ActualProductionTotal = ActualProductionTotal +
                                                    (masterListGroup.ActualProduction.HasValue
                                                        ? masterListGroup.ActualProduction.Value
                                                        : 0);
                            TotalTargetManualTotal = TotalTargetManualTotal +
                                                     (masterListGroup.TotalTargetManual.HasValue
                                                         ? (float) masterListGroup.TotalTargetManual
                                                         : 0);
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
                        slDoc.SetCellValue(iRow, 2, WorkerCountTotal);
                        slDoc.SetCellValue(iRow, 3, AbsentTotal);
                        slDoc.SetCellValue(iRow, 4,
                            String.Format(CultureInfo.CurrentCulture, "{0:n2}", ActualProductionTotal));
                        slDoc.SetCellValue(iRow, 5,
                            String.Format(CultureInfo.CurrentCulture, "{0:n2}", TotalTargetManualTotal));
                        slDoc.SetCellStyle(iRow, 1, iRow, 5, totalStyle);

                        slDoc.SetCellValue(6, 5,
                            ": " + String.Format(CultureInfo.CurrentCulture, "{0:n2}", TotalTargetManualTotal));
                        slDoc.SetCellValue(7, 5,
                            ": " + String.Format(CultureInfo.CurrentCulture, "{0:n2}", ActualProductionTotal));

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
                        //slDoc.AutoFitColumn(1, 5);

                        System.IO.File.Delete(strFileName);
                        slDoc.SaveAs(ms);
                    }
                    // this is important. Otherwise you get an empty file
                    // (because you'd be at EOF after the stream is written to, I think...).
                }
                ms.Position = 0;
                var fileName = "ProductionExecution_TPOProductionEntry_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, brand, year, week, date, process, status }, "Exe TPO Production Entry - Excel");
                return null;
            }
        }

        /// <summary>
        /// Transaction History
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHistory(TransactionLogInput input)
        {
            if (!input.code_3.Equals(""))
            {
                var process = _masterDataBLL.GetMasterProcessByProcess(input.code_3);
                input.code_3 = process.ProcessIdentifier;
            }

            if (!input.code_7.Equals(""))
            {
                DateTime enteredDate = DateTime.Parse(input.code_7);
                int day = enteredDate.DayOfWeek == 0 ? 7 : (int)enteredDate.DayOfWeek;
                input.code_7 = day.ToString();
            }
            
            var pageResult = new PageResult<TransactionHistoryViewModel>();
            var transactionLog = _utilitiesBLL.GetTransactionHistoryByPage(input, Enums.PageName.TPOProductionEntry.ToString());
            pageResult.TotalRecords = transactionLog.Count;
            pageResult.TotalPages = (transactionLog.Count / input.PageSize) + (transactionLog.Count % input.PageSize != 0 ? 1 : 0);
            var result = transactionLog.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            
            //cek sudah save atau belum
            var TransactionCode = this.GenerateTransactionCode(input.separator[0], new string[] { input.code_1, input.code_2, input.code_3, input.code_4, input.code_5, input.code_6, input.code_7, input.code_8, input.code_9 });

            if (!input.statusEmployee.Equals(""))
            {
                var test = _executionTpobll.getAbsentActualProdNotNullByProdEntryCodeAndStatusIdentifier(TransactionCode, input.statusEmployee);
                if (test.Count ==0 ){
                    result = null;
                }
            }
            
            pageResult.Results = Mapper.Map<List<TransactionHistoryViewModel>>(result);
            return Json(pageResult);
        }

        private string GenerateTransactionCode(char separator, params string[] codes)
        {
            var tempTransactionCode = "";
            foreach (var code in codes)
            {
                tempTransactionCode += code;
                tempTransactionCode += separator;
            }
            return tempTransactionCode.TrimEnd(separator);
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

        [HttpPost]
        public ActionResult SaveExeTPOProductionEntry(InsertUpdateData<ExeTPOProductionEntryViewModel> bulkData)
        {
            try
            {
                var OriginalEmpStatus = bulkData.Parameters.ContainsKey("originalEmpStatus")
                    ? bulkData.Parameters["originalEmpStatus"]
                    : "";
                var kpsYear = bulkData.Parameters != null ? bulkData.Parameters["KPSYear"] : "";
                var kpsWeek = bulkData.Parameters != null ? bulkData.Parameters["KPSWeek"] : "";
                var productionDate = bulkData.Parameters != null ? bulkData.Parameters["ProductionDate"] : "";

                var input = new GetExeTPOProductionInput()
                {
                    LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "",
                    Brand = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "",
                    Year = Convert.ToInt32(kpsYear),
                    Week = Convert.ToInt32(kpsWeek),
                    Date =
                        DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat,
                            CultureInfo.InvariantCulture)
                };

                // Update data
                if (bulkData.Edit != null)
                {
                    // Check if all actual has value then verify system YES
                    var verifySystem = bulkData.Edit.Where(c => c.ActualProduction == null).ToList().Any()
                        ? false
                        : true;

                    var param = new ExeTPOProductionViewDTO();
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        var TPOProduction = Mapper.Map<ExeTPOProductionViewDTO>(bulkData.Edit[i]);
                        param = TPOProduction;
                        //set updatedby
                        TPOProduction.UpdatedBy = GetUserName();

                        try
                        {
                            var item = _executionTpobll.SaveExeTPOProductionEntry(TPOProduction, OriginalEmpStatus,
                                verifySystem);
                            bulkData.Edit[i] = Mapper.Map<ExeTPOProductionEntryViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }

                    var process = _masterDataBLL.GetMasterProcessByProcess(param.ProcessGroup);
                    DateTime date = param.ProductionDate;
                    int day = date.DayOfWeek == 0 ? 7 : (int) date.DayOfWeek;
                    var transactionCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                          + param.LocationCode + "/"
                                          + process.ProcessIdentifier + "/"
                                          + param.BrandCode + "/"
                                          + param.KPSYear + "/"
                                          + param.KPSWeek + "/"
                                          + day;

                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.TPOProductionEntry.ToString(),
                        ActionButton = Enums.ButtonName.Save.ToString(),
                        UserName = GetUserName(),
                        TransactionCode = transactionCode,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                    // generate report by group when save production entry, report by status view from report by group
                    _executionTpobll.InsertTPOExeReportByGroups(input.LocationCode, input.Brand, input.Year, input.Week,
                        input.Date, GetUserName());
                }

                return Json(bulkData);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData }, "Exe TPO Production Entry - SaveExeTPOProductionEntry");
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetLastConditionTranslogTPOProdEntry(GetExeTPOProductionInput input)
        {
            try
            {
                DateTime date = input.Date.Value;
                int day = date.DayOfWeek == 0 ? 7 : (int) date.DayOfWeek;

                var transLogProdEntryVerification = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) +
                                                    "/"
                                                    + input.LocationCode + "/"
                                                    + '%' + "/"
                                                    + input.Brand + "/"
                                                    + input.Year + "/"
                                                    + input.Week + "/"
                                                    + day;

                var process = _masterDataBLL.GetMasterProcessByProcess(input.Process);
                if (process != null)
                {
                    transLogProdEntryVerification = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) +
                                                    "/"
                                                    + input.LocationCode + "/"
                                                    + process.ProcessIdentifier + "/"
                                                    + input.Brand + "/"
                                                    + input.Year + "/"
                                                    + input.Week + "/"
                                                    + day;
                }



                var latestTransLogProdCard =
                    _utilitiesBLL.GetLatestActionTransLogExceptSave(transLogProdEntryVerification,
                        Enums.PageName.TPOProductionEntryVerification.ToString());

                var status = "Open";

                if (latestTransLogProdCard != null)
                {
                    var submitted = latestTransLogProdCard.UtilFlow.UtilFunction.FunctionName ==
                                    Enums.ButtonName.Submit.ToString();
                    if (submitted)
                    {
                        status = "Submitted";
                    }
                    var latestTransLogTPOEntry = _utilitiesBLL.GetLatestActionTransLog(transLogProdEntryVerification,
                        Enums.PageName.TPOProductionEntry.ToString());
                    if (latestTransLogTPOEntry != null)
                    {
                        if (latestTransLogTPOEntry.CreatedDate > latestTransLogProdCard.CreatedDate)
                            status = "Open";
                    }
                }

                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "Exe TPO Production Entry - GetLastConditionTranslogTPOProdEntry");
                return null;
            }
        }

        [HttpPost]
        public JsonResult UploadTPODaily()
        {
            IExcelDataReader excelReader = null;
            string fileName = string.Empty;
            string filePath = string.Empty;

            try
            {
                fileName = Request.Headers["X-File-Name"];

                //File's content is available in Request.InputStream property
                System.IO.Stream fileContent = Request.InputStream;
                //Creating a FileStream to save file's content

                filePath = Server.MapPath("~/Upload/TPODaily/") + fileName;

                System.IO.FileStream fileStream = System.IO.File.Create(filePath);
                fileContent.Seek(0, System.IO.SeekOrigin.Begin);
                //Copying file's content to FileStream
                fileContent.CopyTo(fileStream);
                fileStream.Dispose();


                // file extention validation
                string extension = Path.GetExtension(filePath);
                if (extension != EnumHelper.GetDescription(Enums.ExcelFormat.Xls) && extension != EnumHelper.GetDescription(Enums.ExcelFormat.Xlsx))
                {
                    throw new Exception("Format files must either xls/xlxs !");
                }

                FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);

                if (extension == EnumHelper.GetDescription(Enums.ExcelFormat.Xls))
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }

                if (extension == EnumHelper.GetDescription(Enums.ExcelFormat.Xlsx))
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                IEnumerable<string> listUserAccessLocation = _svc.GetLastLocationChildList(Enums.LocationCode.TPO.ToString()).Select(v => v.LocationCode).Distinct();

                _executionTpobll.UploadExcelTPOEntryDaily(excelReader, GetUserName(), listUserAccessLocation);

            }
            catch (Exception e)
            {
                _vtlogger.Err(e, new List<object> { fileName, filePath }, "Upload file TPO Daily");
                Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                return Json(e.Message);
                //throw new Exception(e.Message);
            }

            return Json("success");
        }
    }
}