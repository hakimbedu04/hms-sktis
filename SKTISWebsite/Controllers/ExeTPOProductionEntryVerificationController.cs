using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExeTPOProductionEntryVerification;
using SKTISWebsite.Models.UtilTransactionLog;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using Microsoft.Ajax.Utilities;
using System.Globalization;
using System.Text;

namespace SKTISWebsite.Controllers
{
    public class ExeTPOProductionEntryVerificationController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionPlantBLL _executionPlantBll;
        private IExecutionTPOBLL _executionTpobll;
        private IUtilitiesBLL _utilitiesBLL;
        private IGeneralBLL _generalBll;
        private IExecutionOtherBLL _executionOtherBll;
        private IVTLogger _vtlogger;
        private IExeReportBLL _executionReportBll;

        public ExeTPOProductionEntryVerificationController(IApplicationService applicationService, IVTLogger vtlogger, IExecutionPlantBLL executionPlantBll, IMasterDataBLL masterDataBll, IExecutionTPOBLL executionTpobll, IUtilitiesBLL utilitiesBLL, IGeneralBLL generalBll, IExecutionOtherBLL executionOtherBll, IExeReportBLL executionReportBll)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _executionTpobll = executionTpobll;
            _utilitiesBLL = utilitiesBLL;
            _generalBll = generalBll;
            _executionPlantBll = executionPlantBll;
            _executionOtherBll = executionOtherBll;
            _vtlogger = vtlogger;
            _executionReportBll = executionReportBll;
            SetPage("ProductionExecution/TPO/TPOProductionEntryVerification");
        }

        public ActionResult Index()
        {
            var init = new InitExeTPOProductionEntryVerificationViewModel()
            {
                LocationLookupList = _svc.GetLastLocationChildList(Enums.LocationCode.TPO.ToString()),
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                CanSubmit = false,
                CanCancelSubmit = true
            };
            return View("index", init);
        }

        [HttpGet]
        public JsonResult GetBrand(string LocationCode, int KPSYear, int KPSWeek, string ProductionDate)
        {
            var input = new GetExeTPOProductionEntryVerificationInput
            {
                LocationCode = LocationCode,
                KPSYear = KPSYear,
                KPSWeek = KPSWeek,
                ProductionDate = DateTime.Parse(ProductionDate)
            };

            var brands = _executionTpobll.GetBrandCodeFromExeTPOProductionEntryVerification(input);
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

        [HttpPost]
        public JsonResult GetExeTPOProductionEntryVerification(GetExeTPOProductionEntryVerificationInput criteria)
        {
            try
            {
                var TPOProductionEntryVerifications = _executionTpobll.GetExeTPOProductionEntryVerification(criteria);
                var viewModel = new List<ExeTPOProductionEntryVerificationCompositeViewModel>();
                foreach (var i in TPOProductionEntryVerifications)
                {
                    var data = Mapper.Map<ExeTPOProductionEntryVerificationCompositeViewModel>(i);
                    //data.AlreadySubmit = _utilitiesBLL.CheckDataAlreadySumbit(data.ProductionEntryCode, Enums.PageName.TPOProductionEntryVerification.ToString());
                    //data.CheckedSubmit = data.AlreadySubmit;
                    viewModel.Add(data);
                }
                var pageResult = new PageResult<ExeTPOProductionEntryVerificationCompositeViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Exe TPO Production Entry Verification - GetExeTPOProductionEntryVerification");
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
            if (!input.code_7.Equals(""))
            {
                DateTime enteredDate = DateTime.Parse(input.code_7);
                int day = enteredDate.DayOfWeek == 0 ? 7 : (int)enteredDate.DayOfWeek;
                input.code_7 = day.ToString();
            }

            var pageResult = new PageResult<TransactionHistoryViewModel>();
            var transactionLog = _utilitiesBLL.GetTransactionHistoryByPage(input, Enums.PageName.TPOProductionEntryVerification.ToString());
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

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string brand, int year, int week, DateTime date)
        {
            try
            {
                var input = new GetExeTPOProductionEntryVerificationInput
                {
                    LocationCode = locationCode,
                    BrandCode = brand,
                    KPSYear = year,
                    KPSWeek = week,
                    ProductionDate = date
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

                var executionTPOProductionEntryVerification =
                    _executionTpobll.GetExeTPOProductionEntryVerification(input);

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExecuteTPOProductionEntryVerification + ".xlsx";
                var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    try
                    {
                        //filter values
                        slDoc.SetCellValue(3, 2, ": " + locationCompat);
                        slDoc.SetCellValue(4, 2, ": " + brand);
                        slDoc.SetCellValue(3, 5, ": " + year.ToString());
                        slDoc.SetCellValue(4, 5, ": " + week.ToString());
                        slDoc.SetCellValue(5, 5, ": " + date.ToString("dd/MM/yyyy"));

                        //row values
                        var iRow = 10;

                        foreach (var ProductionEntryVerification in executionTPOProductionEntryVerification)
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

                            float tpkValue = ProductionEntryVerification.TotalTPKValue.HasValue
                                ? (float) ProductionEntryVerification.TotalTPKValue
                                : 0;
                            float actualValue = ProductionEntryVerification.TotalActualValue.HasValue
                                ? (float) ProductionEntryVerification.TotalActualValue
                                : 0;
                            slDoc.SetCellValue(iRow, 1, ProductionEntryVerification.ProcessGroup);
                            slDoc.SetCellValue(iRow, 2, ProductionEntryVerification.Absent);
                            slDoc.SetCellValue(iRow, 3, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpkValue));
                            slDoc.SetCellValue(iRow, 4, String.Format(CultureInfo.CurrentCulture, "{0:n2}", actualValue));

                            bool vs = true;
                            if (tpkValue > 0 &&
                                actualValue == 0)
                            {
                                vs = false;
                            }
                            else if (tpkValue > 0 &&
                                     actualValue > 0)
                            {
                                vs = true;
                            }
                            else if (tpkValue == 0 &&
                                     actualValue == 0)
                            {
                                vs = true;
                            }

                            slDoc.SetCellValue(iRow, 5, (vs) ? "Yes" : "No");
                            var vm = ProductionEntryVerification.VerifyManual.HasValue
                                ? (bool) ProductionEntryVerification.VerifyManual
                                : false;
                            slDoc.SetCellValue(iRow, 6, (vm) ? "Yes" : "No");
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
                        slDoc.AutoFitColumn(1, 6);

                        System.IO.File.Delete(strFileName);
                        slDoc.SaveAs(ms);
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> {slDoc}, "Excell button on TPO Entry Verification");
                    }
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "ProductionExecution_ProductionEntryVerification_" + DateTime.Now.ToShortDateString() +
                               ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> {  locationCode, brand, year, week, date }, "Excell button on TPO Entry Verification");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveTPOProductionEntryVerification(InsertUpdateData<ExeTPOProductionEntryVerificationCompositeViewModel> bulkData)
        {
            try
            {
                // Update data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        if ((bulkData.Edit[i].State == "SUBMIT") || (!bulkData.Edit[i].VerifyManual)) continue;

                        var TPOProductionEntryVerification = Mapper.Map<ExeTPOProductionEntryVerificationDTO>(bulkData.Edit[i]);

                        //set updatedby
                        TPOProductionEntryVerification.UpdatedBy = GetUserName();

                        try
                        {
                            var item =
                                _executionTpobll.SaveExeTPOProductionEntryVerification(TPOProductionEntryVerification);
                            //_generalBll.ExeTransactionLog(new TransactionLogInput()
                            //{
                            //    page = Enums.PageName.TPOProductionEntryVerification.ToString(),
                            //    ActionButton = Enums.ButtonName.Save.ToString(),
                            //    UserName = GetUserName(),
                            //    TransactionCode = TPOProductionEntryVerification.ProductionEntryCode
                            //});

                            bulkData.Edit[i] = Mapper.Map<ExeTPOProductionEntryVerificationCompositeViewModel>(item);
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                            _vtlogger.Err(ex, new List<object> { TPOProductionEntryVerification, bulkData.Edit[i] }, "Save Production TPO Entry Verification");
                        }
                        catch (Exception ex)
                        {
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            _vtlogger.Err(ex, new List<object> { TPOProductionEntryVerification, bulkData.Edit[i] }, "Save Production TPO Entry Verification");
                        }
                    }
                }

                return Json(bulkData);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData }, "Save button on TPO Entry Verification");
                return null;
            }
        }


        [HttpPost]
        public JsonResult CheckSubmitAndCancel(GetExeTPOProductionEntryVerificationInput criteria)
        {
            try
            {
                // Can Submit Condition
                // First Condition : Check if every Verify Manual on grid is valid / true
                var TPOProductionEntryVerifications = _executionTpobll.GetExeTPOProductionEntryVerification(criteria);
                var notVerifyExistSystem =
                    TPOProductionEntryVerifications.Count(v => v.VerifySystem == true && v.VerifyManual == true);
                //var notVerifyExistManual = TPOProductionEntryVerifications.Count(v => v.VerifyManual == true);
                var TPOProductionEntryVerificationsDistinct =
                    TPOProductionEntryVerifications.DistinctBy(v => v.ProcessGroup)
                        .OrderBy(v => v.ProcessGroup)
                        .Select(v => v.ProcessGroup)
                        .ToList();

                // Second Condition : Check if both ExeActualWorkHours and ExeTPOProductionEntryVerificationView has same criteria, include it's distinct process group count
                // should be related to ExeTPOActualWorkHours not ExeActualWorkHours
                //var ActualWorkHours = _executionTpobll.GetActualWorkHoursByProductionEntryVerification(criteria);
                //var ActualWorkHoursDistinct = ActualWorkHours.DistinctBy(a => a.ProcessGroup).OrderBy(v => v.ProcessGroup).Select(a => a.ProcessGroup).ToList();
                // should be related to ExeTPOActualWorkHours not ExeActualWorkHours
                var ActualWorkHours = _executionTpobll.GetTpoActualWorkHoursByProductionEntryVerivication(criteria);
                var ActualWorkHoursDistinct =
                    ActualWorkHours.OrderBy(a => a.ProcessOrder)
                        .GroupBy(a => a.ProcessGroup)
                        .Select(a => new {PG = a.Key, CPG = a.Select(x => x.ProcessGroup).Count()})
                        .ToList();

                var tpoTPKValue = _executionTpobll.GetTpoTpkValueDistinct(criteria.LocationCode, criteria.BrandCode,
                    criteria.KPSYear, criteria.KPSWeek, criteria.ProductionDate, criteria.Check);
                var tpkPG = tpoTPKValue.Select(a => a.ProcessGroup).FirstOrDefault();
                var tpkPWHTemp = tpoTPKValue.Select(a => a.ProcessWorkHoursTemp).FirstOrDefault();
                var tpotpkResult = false;
                foreach (var checkData in TPOProductionEntryVerifications)
                {
                    if (criteria.Check == checkData.ProcessGroup)
                    {
                        if (checkData.TotalActualValue > 0 && tpkPWHTemp > 0)
                        {
                            tpotpkResult = true;
                        }
                        if (tpkPWHTemp == 0 || tpkPWHTemp == null)
                        {
                            tpotpkResult = true;
                        }
                    }
                }

                var TPOProdEntry = _executionTpobll.GetExeTPOProductionEntry(new GetExeTPOProductionInput
                {
                    LocationCode = criteria.LocationCode,
                    Brand = criteria.BrandCode,
                    Year = criteria.KPSYear,
                    Week = criteria.KPSWeek,
                    Date = criteria.ProductionDate
                });

                var totalEmp = TPOProdEntry.DistinctBy(s => s.StatusEmp).Select(s => s.StatusEmp).ToList();

                var result = false;
                var cutting = -1;
                var rolling = -1;
                var stickwrapping = -1;
                var wrapping = -1;
                var packing = -1;
                var stamping = -1;
                foreach (var data in ActualWorkHoursDistinct)
                {
                    cutting = data.PG == "CUTTING" ? data.CPG : cutting;
                    rolling = data.PG == "ROLLING" ? data.CPG : rolling;
                    stickwrapping = data.PG == "STICKWRAPPING" ? data.CPG : stickwrapping;
                    wrapping = data.PG == "WRAPPING" ? data.CPG : wrapping;
                    packing = data.PG == "PACKING" ? data.CPG : packing;
                    stamping = data.PG == "STAMPING" ? data.CPG : stamping;
                }
                if (ActualWorkHoursDistinct.Count == 5)
                {
                    if ((cutting == totalEmp.Count()) && (rolling == totalEmp.Count()) &&
                        (stickwrapping == totalEmp.Count()))
                    {
                        result = true;
                    }
                    else if (result == true && packing == totalEmp.Count())
                    {
                        result = true;
                    }
                    else if (result == true && stamping == totalEmp.Count())
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (ActualWorkHoursDistinct.Count == 4)
                {
                    if ((cutting == totalEmp.Count()) && (rolling == totalEmp.Count()))
                    {
                        result = true;
                    }
                    else if (result == true && packing == totalEmp.Count())
                    {
                        result = true;
                    }
                    else if (result == true && stamping == totalEmp.Count())
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }


                var valid = true;
                if (rolling == totalEmp.Count() && criteria.Check == "ROLLING")
                {
                    valid = true;
                }
                else if (cutting == totalEmp.Count() && criteria.Check == "CUTTING")
                {
                    valid = true;
                }
                else if (stickwrapping == totalEmp.Count() && criteria.Check == "STICKWRAPPING")
                {
                    valid = true;
                }
                else if (valid == true && wrapping == totalEmp.Count() && criteria.Check == "WRAPPING")
                {
                    valid = true;
                }
                else if (valid == true && packing == totalEmp.Count() && criteria.Check == "PACKING")
                {
                    valid = true;
                }
                else if (valid == true && stamping == totalEmp.Count() && criteria.Check == "STAMPING")
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                }

                DateTime date = criteria.ProductionDate ?? DateTime.Now.Date;
                int day = date.DayOfWeek == 0 ? 7 : (int) date.DayOfWeek;
                //var translogCode = "";
                //var cuttingV = false;
                //var rollingV = false;
                //var stickwrappingV = false;
                //var wrappingV = false;
                //var packingV = false;
                //var stampingV = false;
                //if (criteria.Check == null)
                //{
                //    var a = TPOProductionEntryVerifications.Where(x=>criteria.Val.Contains(x.ProcessGroup));
                //    foreach (var data in a)
                //    {
                //        if(data.ProcessGroup)
                //    }
                //}

                var getProcIdentifier = _masterDataBLL.GetMasterProcessByProcess(criteria.Check);
                    //get process identifier
                var ProcIdentifier = "";
                if (getProcIdentifier.ProcessIdentifier != null)
                {
                    ProcIdentifier = getProcIdentifier.ProcessIdentifier;
                }
                else
                {
                    ProcIdentifier = "4";
                }

                // Check if both distinct process not empty
                //var ListIsEqual = false;
                //if (TPOProductionEntryVerificationsDistinct.Count > 0 && ActualWorkHoursDistinct.Count > 0)
                //    ListIsEqual = TPOProductionEntryVerificationsDistinct.SequenceEqual(ActualWorkHoursDistinct);
                var EBLTransCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                   + criteria.LocationCode + "/"
                    //+ "4/"
                                   + ProcIdentifier + "/"
                                   + criteria.BrandCode + "/"
                                   + criteria.KPSYear + "/"
                                   + criteria.KPSWeek + "/"
                                   + day;
                //EBL/IDAB/4/FA010784.15/2016/23/1
                var EBLSubmitLog = _utilitiesBLL.GetTransactionLogByTransCodeAndIDFlow(EBLTransCode, 40);

                //var EBLCancelSubmitLog = _utilitiesBLL.GetTransactionLogByTransCodeAndIDFlow(EBLTransCode, 35);

                var AlreadySubmitted = EBLSubmitLog != null ? true : false;

                //var AlreadyCancelled = EBLCancelSubmitLog != null ? true : false;

                // Checking Total Actual Value ROLLING and CUTTING when submit must have the same value
                var totalActualValueRolling =
                    TPOProductionEntryVerifications.Where(
                        c => c.ProcessGroup == Enums.Process.Rolling.ToString().ToUpper())
                        .Select(c => c.TotalActualValue)
                        .FirstOrDefault();
                var totalActualValueCutting =
                    TPOProductionEntryVerifications.Where(
                        c => c.ProcessGroup == Enums.Process.Cutting.ToString().ToUpper())
                        .Select(c => c.TotalActualValue)
                        .FirstOrDefault();
                var isTotalActualValueSame = totalActualValueRolling == totalActualValueCutting;

                //todo CanSubmit for now always true
                var CanSubmit = (valid && !AlreadySubmitted && result && tpotpkResult && isTotalActualValueSame);
                // End OF Can Submit Condition

                // Can Cancel Submit Condition
                // Not implemented yet

                return
                    Json(new SubmitAndCancelViewModel()
                    {
                        CanSubmit = CanSubmit,
                        CanCancelSubmit = (!CanSubmit && AlreadySubmitted)
                    });
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "CheckSubmitAndCancel on TPO Entry Verification");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SubmitTPOProductionEntryVerification(InsertUpdateData<ExeTPOProductionEntryVerificationCompositeViewModel> bulkData)
        {
            try
            {
                var listResultJSon = new List<string>();
                var generateTpo = false;
                var kpsYear = bulkData.Parameters != null ? bulkData.Parameters["KPSYear"] : "";
                var kpsWeek = bulkData.Parameters != null ? bulkData.Parameters["KPSWeek"] : "";
                var productionDate = bulkData.Parameters != null ? bulkData.Parameters["ProductionDate"] : "";

                var input = new GetExeTPOProductionEntryVerificationInput()
                {
                    BrandCode = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "",
                    LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "",
                    KPSYear = Convert.ToInt32(kpsYear),
                    KPSWeek = Convert.ToInt32(kpsWeek),
                    ProductionDate = DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture)
                };

                var statusTPOFee = _executionTpobll.GetStatusTPOFee(input);

                if (statusTPOFee == Enums.StatusTPOFee.DRAFT.ToString() || statusTPOFee == Enums.StatusTPOFee.OPEN.ToString() || statusTPOFee == Enums.StatusTPOFee.REVISED.ToString() || String.IsNullOrEmpty(statusTPOFee))
                {
                    var resultJSonSubmitData = "";
                    try
                    {
                        if (bulkData.Edit != null)
                        {
                            var date = input.ProductionDate ?? DateTime.Now.Date;
                            var day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;

                            //unused method sejak submit dibuat async

                            // cancel previously submitted entry first
                            //var report = _executionTpobll.TPOProductionEntryVerificationCancelReport(input);
                            //var TPOProductionEntryVerifications = _executionTpobll.GetExeTPOProductionEntryVerification(input);
                            // Generate default by process UOM 1 - 14 based on data H-1
                            //_executionPlantBll.InsertDefaultExeReportByProcess(input.LocationCode, input.BrandCode, Enums.UnitCodeDefault.PROD.ToString(), input.KPSYear, input.KPSWeek, input.ProductionDate.Value, GetUserName(), GetUserName());
                            //_executionReportBll.UpdateEndingStockByProcess(input.LocationCode, Enums.UnitCodeDefault.PROD.ToString(), input.BrandCode, input.ProductionDate.Value);

                            //unused method sejak submit dibuat async

                            var listProcessToBeSubmitted = new List<string>();

                            #region begin loop process
                            foreach (var t in bulkData.Edit.Where(t => t.State == "INITIAL" || t.State == "CANCELSUBMIT"))
                            {
                                if (t.ProcessGroup == "CUTTING")
                                {
                                    generateTpo = true;
                                }

                                if (!t.Flag_Manual) continue;
                                if (!t.VerifyManual) continue;

                                var process = _masterDataBLL.GetMasterProcessByProcess(t.ProcessGroup);
                                var transactionCode = EnumHelper.GetDescription(Enums.CombineCode.EBL) + "/"
                                                      + input.LocationCode + "/"
                                                      + process.ProcessIdentifier + "/"
                                                      + input.BrandCode + "/"
                                                      + input.KPSYear + "/"
                                                      + input.KPSWeek + "/"
                                                      + day;

                                // ketika cancel submit verify system tetap Y 
                                _executionTpobll.UpdateVerifyAndFlagTPOEntryVerification(transactionCode, t.VerifySystem, t.VerifyManual, t.Flag_Manual);

                                _generalBll.ExeTransactionLog(new TransactionLogInput()
                                {
                                    page = Enums.PageName.TPOProductionEntryVerification.ToString(),
                                    ActionButton = Enums.ButtonName.Submit.ToString(),
                                    UserName = GetUserName(),
                                    TransactionCode = transactionCode,
                                    IDRole = CurrentUser.Responsibility.Role
                                });

                                /*
                                 * ditutup dulu untuk async
                                // begin generate translog
                                var transactionInput = new TransactionLogInput
                                {
                                    page = Enums.PageName.TPOProductionEntryVerification.ToString(),
                                    ActionButton = Enums.ButtonName.Submit.ToString(),
                                    UserName = GetUserName(),
                                    TransactionCode = transactionCode,
                                    IDRole = CurrentUser.Responsibility.Role
                                };

                                _generalBll.ExeTransactionLogAsync(transactionInput);
                                // end generate translog
                                */
                                listProcessToBeSubmitted.Add(t.ProcessGroup);
                            }
                            #endregion end loop process

                            // Change all process must be submitted, keep all process in listProcessToBeSubmitted List
                            if (listProcessToBeSubmitted.Any()) {
                                listProcessToBeSubmitted.Clear();
                                listProcessToBeSubmitted = _executionTpobll.GetListProcessVerification(input.LocationCode, input.BrandCode, input.ProductionDate.Value.Date).ToList();
                            }

                            //generate by groups
                            _executionTpobll.InsertTPOExeReportByGroups(input.LocationCode, input.BrandCode, input.KPSYear,
                               input.KPSWeek, input.ProductionDate, GetUserName());


                            //generate by process
                            foreach (var process in listProcessToBeSubmitted)
                            {
                                _executionPlantBll.InsertReportByProcess(input.LocationCode, input.BrandCode,
                                        process, input.KPSYear, input.KPSWeek, GetUserName(),
                                        GetUserName(), input.ProductionDate.Value, Enums.UnitCodeDefault.PROD.ToString());
                            }

                            /* g dipake tutup dl
                             * generate by group, by process, dan tpo fee disatukan dengan async pada method GenerateAllAsyncSequence
                             * * 

                            // submit
                            // generate report by groups
                            _executionTpobll.InsertTPOExeReportByGroupsAsync(input.LocationCode, input.BrandCode, input.KPSYear, input.KPSWeek, input.ProductionDate, GetUserName());

                            // generate report by process
                            foreach (var process in listProcessToBeSubmitted) {
                                _executionPlantBll.InsertReportByProcessAsync(input.LocationCode, input.BrandCode, process, input.KPSYear, input.KPSWeek, GetUserName(), GetUserName(), input.ProductionDate.Value, Enums.UnitCodeDefault.PROD.ToString());
                            }
                            // submit
                             */

                            //new code(async), ditutup dulu balikin ke awal
                            /*
                             * try
                            {
                                //new code
                                _executionTpobll.GenerateAllAsyncSequence(listProcessToBeSubmitted, input.LocationCode, input.BrandCode, input.KPSYear, input.KPSWeek, input.ProductionDate, GetUserName(), GetUserName(), Enums.UnitCodeDefault.PROD.ToString(), generateTpo);

                            }
                            catch (Exception ex)
                            {
                                _vtlogger.Err(ex, null, "failed generate tpo fee");
                                return Json("Failed to generate tpo fee data on background process.");
                            }
                            //new code
                             * */

                        }

                        resultJSonSubmitData = "Run submit data on background process.";
                        listResultJSon.Add(resultJSonSubmitData);

                    }
                    catch (Exception ex)
                    {
                        resultJSonSubmitData = "Failed to run submit data on background process.";
                        listResultJSon.Add(resultJSonSubmitData);
                        _vtlogger.Err(ex, new List<object> { resultJSonSubmitData },
                            "SubmitTPOProductionEntryVerification, Failed to run submit data on background process.");
                        return Json(listResultJSon);
                    }

                    #region If adjustment exist OR Switching Brand
                    /*var adjusmentInput = new ProductAdjustmentInput()
                    {
                        BrandCode = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "",
                        LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "",
                        ProductionDate =
                            DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat,
                                CultureInfo.InvariantCulture)
                    };

                    try
                    {
                        var adjustment = _executionOtherBll.GetProductAdjustments(adjusmentInput);
                        foreach (var adj in adjustment)
                        {
                            var updateInput = new ProductAdjustmentDTO()
                            {
                                LocationCode = adj.LocationCode,
                                BrandCode = adj.BrandCode,
                                ProductionDate = adj.ProductionDate,
                                UnitCode = adj.UnitCode,
                                AdjustmentType = adj.AdjustmentType,
                                AdjustmentRemark = adj.AdjustmentRemark,
                                AdjustmentValue = adj.AdjustmentValue,
                                CreatedBy = adj.CreatedBy,
                                UpdatedBy = adj.UpdatedBy,
                                GroupCode = adj.GroupCode,
                                Shift = adj.Shift
                            };

                            _executionOtherBll.UpdateProductAdjustment(updateInput);
                        }

                        #region switching brand

                        var brandGroup = _masterDataBLL.GetBrandGruopCodeByBrandCode(input.BrandCode);
                        _executionPlantBll.switchBrandExeReportByProcess(input.LocationCode, brandGroup,
                            input.ProductionDate.Value);

                        #endregion

                        #region Recalculate Stock

                        _executionPlantBll.recalculateStockExeReportByProcess(input.LocationCode, input.BrandCode,
                            input.ProductionDate.Value);

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, null, "switching brand or Recalculate Stock");
                        return Json("Failed to recalculate stock.");
                    }

                    */
                    #endregion

                    try
                    {
                        if (generateTpo)
                        {
                            // execute tpo fee SP if stamping submited
                            // tutup dl, dipindah ke method GenerateAllAsyncSequence
                            //_executionTpobll.TpoProductionEntryVerificationGenerateReportAsync(input, GetUserName());
                            // execute tpo fee SP if stamping submited
                            var result = _executionTpobll.TPOProductionEntryVerificationGenerateReport(input, GetUserName());
                        }
                        //if (isEmpty)
                        //{
                        //    //Generate Default Report by Process
                        //    string unit = "PROD";
                        //    _executionPlantBll.DefaultExeReportByProcess(input.LocationCode, input.BrandCode, unit, input.KPSYear, input.KPSWeek, input.ProductionDate.Value, GetUserName(), GetUserName());
                        //}

                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, null, "failed generate tpo fee");
                        return Json("Failed to generate tpo fee data on background process.");
                    }

                    try
                    {
                        //_executionPlantBll.SendEmailSubmitTpotEntryVerif(input, GetUserName());
                    }
                    catch (Exception ex)
                    {
                        var resultJSonSendEmail = "Failed to send email.";
                        listResultJSon.Add(resultJSonSendEmail);
                    }

                    return Json(listResultJSon);
                }
                else 
                {
                    listResultJSon.Add("<strong>Failed</strong> to Submit, TPO Fee Already has already in approval process");
                    return Json(listResultJSon);
                } 
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData }, "SubmitTPOProductionEntryVerification on TPO Entry Verification");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SubmitTPOProductionEntryVerificationTuneUp(InsertUpdateData<ExeTPOProductionEntryVerificationCompositeViewModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var listResultJSon = new List<string>();
            try
            {
                var kpsYear = bulkData.Parameters != null ? bulkData.Parameters["KPSYear"] : "";
                var kpsWeek = bulkData.Parameters != null ? bulkData.Parameters["KPSWeek"] : "";
                var productionDate = bulkData.Parameters != null ? bulkData.Parameters["ProductionDate"] : "";

                var input = new GetExeTPOProductionEntryVerificationInput()
                {
                    BrandCode = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "",
                    LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "",
                    KPSYear = Convert.ToInt32(kpsYear),
                    KPSWeek = Convert.ToInt32(kpsWeek),
                    ProductionDate = DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture)
                };

                if (bulkData.Edit != null)
                {
                    StringBuilder listProcessToSubmit = new StringBuilder();

                    foreach (var t in bulkData.Edit.Where(t => t.State == "INITIAL" || t.State == "CANCELSUBMIT"))
                    {
                        if (!t.Flag_Manual) continue;
                        if (!t.VerifyManual) continue;

                        listProcessToSubmit.Append(t.ProcessGroup);
                        listProcessToSubmit.Append(";");
                    }

                    _executionTpobll.SubmitTPOEntryVerificationSP(bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "",
                                bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "", 
                                Convert.ToInt32(kpsYear),
                                Convert.ToInt32(kpsWeek),
                                DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture), 
                                listProcessToSubmit.ToString(), GetUserName(), CurrentUser.Responsibility.Role);
                }

                resultJSonSubmitData = "Run submit data on background process.";
                listResultJSon.Add(resultJSonSubmitData);

                return Json(listResultJSon);

            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData }, "SubmitTPOProductionEntryVerification on TPO Entry Verification");
                resultJSonSubmitData = "Failed to run submit data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }
        }

        [HttpPost]
        public ActionResult CancelSubmitTPOProductionEntryVerification(InsertUpdateData<ExeTPOProductionEntryVerificationCompositeViewModel> bulkData)
        {
            var kpsYear = bulkData.Parameters != null ? bulkData.Parameters["KPSYear"] : "";
            var kpsWeek = bulkData.Parameters != null ? bulkData.Parameters["KPSWeek"] : "";
            var productionDate = bulkData.Parameters != null ? bulkData.Parameters["ProductionDate"] : "";
            var BrandCode = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "";
            var LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";

            var inputCheckTPOFee = new GetExeTPOProductionEntryVerificationInput()
            {
                BrandCode = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "",
                LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "",
                KPSYear = Convert.ToInt32(kpsYear),
                KPSWeek = Convert.ToInt32(kpsWeek),
                ProductionDate =
                    DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat,
                        CultureInfo.InvariantCulture)
            };

            var statusTPOFee = _executionTpobll.GetStatusTPOFee(inputCheckTPOFee);

            if (statusTPOFee == Enums.StatusTPOFee.DRAFT.ToString() || statusTPOFee == Enums.StatusTPOFee.OPEN.ToString() || statusTPOFee == Enums.StatusTPOFee.REVISED.ToString() || String.IsNullOrEmpty(statusTPOFee))
            {
                try
                {
                    if (bulkData.Edit != null)
                    {
                        for (var i = 0; i < bulkData.Edit.Count; i++)
                        {
                            if (bulkData.Edit[i].State == "SUBMIT")
                            {
                                if (!bulkData.Edit[i].Flag_Manual) continue;

                                var process = _masterDataBLL.GetMasterProcessByProcess(bulkData.Edit[i].ProcessGroup);
                                DateTime date = DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                                int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;
                                var transactionCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                                              + LocationCode + "/"
                                                              + process.ProcessIdentifier + "/"
                                                              + BrandCode + "/"
                                                              + kpsYear + "/"
                                                              + kpsWeek + "/"
                                                              + day;

                                #region
                                //_executionTpobll.UpdateVerifyAndFlagTPOEntryVerification(transactionCode, false, false, false);
                                // ketika cancel submit verify system tetap Y 
                                // based on ticket http://tp.voxteneo.co.id/entity/10405 point 2
                                _executionTpobll.UpdateVerifyAndFlagTPOEntryVerification(transactionCode, true, false, false); // ketika cancel submit verify system tetap Y 
                                #endregion

                                _generalBll.ExeTransactionLog(new TransactionLogInput()
                                {
                                    page = Enums.PageName.TPOProductionEntryVerification.ToString(),
                                    ActionButton = EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit),
                                    UserName = GetUserName(),
                                    TransactionCode = transactionCode,
                                    IDRole = CurrentUser.Responsibility.Role
                                });

                                var input = new GetExeTPOProductionEntryVerificationInput()
                                {
                                    BrandCode = BrandCode,
                                    LocationCode = LocationCode,
                                    KPSYear = Convert.ToInt32(kpsYear),
                                    KPSWeek = Convert.ToInt32(kpsWeek),
                                    ProductionDate = DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture)
                                };

                                var result = _executionTpobll.TPOProductionEntryVerificationCancelReport(input);
                                var recalculate = _executionTpobll.TPOProductionEntryVerificationGenerateReport(input, GetUserName());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, null, "Failed to Cancel Submit TPO Verification");
                    return Json("Failed to Cancel Submit TPO Verification");
                }
            }
            else
            {
                return Json("<strong>FAILED</strong> to Cancel Submit, TPO Fee Already has already in approval process");
            }

 

           return Json("Run cancel submit data on background process.");
        }
    }
}