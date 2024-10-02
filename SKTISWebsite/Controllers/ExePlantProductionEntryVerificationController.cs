using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.ExePlantProductionEntryVerification;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using Color = System.Drawing.Color;
using SKTISWebsite.Models.Common;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using System.Globalization;
using SKTISWebsite.Models.UtilTransactionLog;

namespace SKTISWebsite.Controllers
{
    public class ExePlantProductionEntryVerificationController : BaseController
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
        private IExeReportBLL _executionReportBll;

        public ExePlantProductionEntryVerificationController
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
             IVTLogger vtlogger,
            IExeReportBLL executionReportBll
         )
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _planningBll = planningBll;
            _executionPlantBll = executionPlantBll;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBLL;
            _exeReportBLL = exeReportBLL;
            _plantWagesExecutionBll = plantWagesExecutionBll;
            _executionOtherBll = executionOtherBll;
            SetPage("ProductionExecution/Plant/ProductionEntryVerification");
            _vtlogger = vtlogger;
            _executionReportBll = executionReportBll;
        }

        /// <summary>
        /// Transaction History
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHistory(TransactionLogInput input)
        {
            DateTime enteredDate = DateTime.Parse(input.code_9);
            int day = enteredDate.DayOfWeek == 0 ? 7 : (int)enteredDate.DayOfWeek;
            input.code_9 = day.ToString();
            var pageResult = new PageResult<TransactionHistoryViewModel>();
            var transactionLog = _utilitiesBLL.GetTransactionHistoryByPage(input, Enums.PageName.ProductionEntryVerification.ToString());
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

        // GET: ExePlantProductionEntryVerification
        public ActionResult Index(string param1, string param2, int? param3, string param4, int? param5, int? param6, string param7, int? param8)
        {
            if (param8.HasValue) setResponsibility(param8.Value);
            var init = new InitExePlantProductionEntryVerificationViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                PLNTChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                AbsentTypes = _svc.GetAllAbsentTypes(),
                Param1LocationCode = param1,
                Param2UnitCode = param2,
                Param3Shift = param3,
                Param4BrandCode = param4,
                Param5KPSYear = param5,
                Param6KPSWeek = param6,
                Param7Date = String.IsNullOrEmpty(param7) ? DateTime.Now.Date : DateTime.ParseExact(param7, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };

            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetUnitCodeSelectListByLocationCode(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(model, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetShiftByLocation(string locationCode)
        {
            var shifts = _svc.GetShiftByLocationCode(locationCode);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetClosingPayroll(DateTime? date)
        {
            //var closingPayroll = _masterDataBLL.GetMasterClosingPayroll(new GetMstClosingPayrollInput());
            var closingPayroll = _masterDataBLL.GetMasterClosingPayrollByDate(date);
            return closingPayroll != null ? Json(String.Format("{0:dddd, d MMMM yyyy}", closingPayroll.ClosingDate), JsonRequestBehavior.AllowGet) : null;
        }

        [HttpPost]
        public JsonResult GetExePlantProductionEntryVerification(GetExePlantProductionEntryVerificationInput criteria)
        {
            var masterLists = _executionPlantBll.GetExePlantProductionEntryVerificationViews(criteria);
            var viewModel = Mapper.Map<List<ExePlantProductionEntryVerificationViewViewModel>>(masterLists);
            var pageResult = new PageResult<ExePlantProductionEntryVerificationViewViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        /// <summary>
        /// Gets the brand group by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBrandCodeByLocationCode(string locationCode)
        {
            var brandCodes = _masterDataBLL.GetBrandCodeByLocationCode(locationCode);
            return Json(brandCodes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandSelectList(GetExePlantProductionEntryVerificationInput input)
        {
            var result = _svc.GetBrandCodeFromExePlantProductionEntryVerification(input);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SaveAllProductionEntryVerification(InsertUpdateData<ExePlantProductionEntryVerificationViewViewModel> bulkData)
        {
            // save data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var plantProductionGroup = Mapper.Map<ExePlantProductionEntryVerificationViewDTO>(bulkData.Edit[i]);
                    plantProductionGroup.UpdatedBy = GetUserName();

                    if (plantProductionGroup.ProcessGroup == "Total") continue;

                    try
                    {
                        var tanggal = bulkData.Edit[i].ProductionDate;
                        var productionDate = String.IsNullOrEmpty(tanggal) ? DateTime.Now.Date : Convert.ToDateTime(tanggal);
                        var day = (int)productionDate.DayOfWeek == 0 ? 7 : (int)productionDate.DayOfWeek;


                        // Create combine code
                        var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                                  + bulkData.Edit[i].LocationCode + "/"
                                                  + bulkData.Edit[i].Shift + "/"
                                                  + bulkData.Edit[i].UnitCode + "/"
                                                  + bulkData.Edit[i].GroupCode + "/"
                                                  + bulkData.Edit[i].BrandCode + "/"
                                                  + bulkData.Edit[i].KPSYear + "/"
                                                  + bulkData.Edit[i].KPSWeek + "/"
                                                  + day;

                        //checking ke translog
                        var transLogVerification = _utilitiesBLL.GetLatestActionTransLogExceptSave(productionEntryCode, Enums.PageName.ProductionEntryVerification.ToString());

                        if (transLogVerification != null)
                        {
                            if (transLogVerification.UtilFlow.UtilFunction.FunctionName != Enums.ButtonName.Submit.ToString())
                            {
                                var item = _executionPlantBll.UpdatExePlantProductionEntryVerification(plantProductionGroup);

                                bulkData.Edit[i] = Mapper.Map<ExePlantProductionEntryVerificationViewViewModel>(item);

                                _generalBll.ExeTransactionLog(new TransactionLogInput()
                                {
                                    page = Enums.PageName.ProductionEntryVerification.ToString(),
                                    ActionButton = Enums.ButtonName.Save.ToString(),
                                    UserName = GetUserName(),
                                    TransactionCode = productionEntryCode,
                                    IDRole = CurrentUser.Responsibility.Role
                                });
                                bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                            }
                            else
                            {
                                var transLogEntry = _utilitiesBLL.GetLatestActionTransLogExceptSave(productionEntryCode, Enums.PageName.PlantProductionEntry.ToString());
                                if (transLogEntry != null)
                                {
                                    if (transLogEntry.CreatedDate > transLogVerification.CreatedDate)
                                    {
                                        var item = _executionPlantBll.UpdatExePlantProductionEntryVerification(plantProductionGroup);

                                        bulkData.Edit[i] = Mapper.Map<ExePlantProductionEntryVerificationViewViewModel>(item);

                                        _generalBll.ExeTransactionLog(new TransactionLogInput()
                                        {
                                            page = Enums.PageName.ProductionEntryVerification.ToString(),
                                            ActionButton = Enums.ButtonName.Save.ToString(),
                                            UserName = GetUserName(),
                                            TransactionCode = productionEntryCode,
                                            IDRole = CurrentUser.Responsibility.Role
                                        });
                                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            var item = _executionPlantBll.UpdatExePlantProductionEntryVerification(plantProductionGroup);

                            bulkData.Edit[i] = Mapper.Map<ExePlantProductionEntryVerificationViewViewModel>(item);

                            _generalBll.ExeTransactionLog(new TransactionLogInput()
                            {
                                page = Enums.PageName.ProductionEntryVerification.ToString(),
                                ActionButton = Enums.ButtonName.Save.ToString(),
                                UserName = GetUserName(),
                                TransactionCode = productionEntryCode,
                                IDRole = CurrentUser.Responsibility.Role
                            });
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }

                    }
                    catch (ExceptionBase ex)
                    {
                        _vtlogger.Err(ex, new List<object> { bulkData.Edit[i], plantProductionGroup }, "EditAllProductionEntryVerification");
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> { bulkData.Edit[i], plantProductionGroup }, "EditAllProductionEntryVerification");
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                    }
                }
            }

            return Json(bulkData);
        }

        public FileStreamResult GenerateExcel(GetExePlantProductionEntryVerificationInput criteria)
        {
            criteria.SortExpression = "ProcessGroup";
            criteria.SortOrder = "SortOrder";

            var plantProductionEntryVerification = _executionPlantBll.GetExePlantProductionEntryVerificationViews(criteria);
            var locationObj = _masterDataBLL.GetLocation(criteria.LocationCode);

            var closingPayroll = _masterDataBLL.GetMasterClosingPayrollByDate(criteria.Date);
            var closingPayrollDate = closingPayroll != null ? String.Format("{0:dddd, d MMMM yyyy}", closingPayroll.ClosingDate) : null;

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExecuteExcelTemplate.ExecutePlantProductionEntryVerification + ".xlsx";
            var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 3, locationObj.LocationCompat);
                slDoc.SetCellValue(4, 3, criteria.UnitCode);
                slDoc.SetCellValue(5, 3, criteria.Shift.ToString());
                slDoc.SetCellValue(6, 3, criteria.BrandCode);
                slDoc.SetCellValue(3, 8, criteria.KpsYear.ToString());
                slDoc.SetCellValue(4, 8, criteria.KpsWeek.ToString());
                slDoc.SetCellValue(5, 8, criteria.ProductionDate.Value.ToShortDateString());
                slDoc.SetCellValue(6, 8, closingPayrollDate);

                //row values
                var iRow = 10;

                foreach (var verificationViewDto in plantProductionEntryVerification)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (verificationViewDto.ProcessGroup == "Total")
                    {
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, verificationViewDto.ProcessGroup);
                    slDoc.SetCellValue(iRow, 2, verificationViewDto.GroupCode);
                    slDoc.SetCellValue(iRow, 3, verificationViewDto.A.HasValue ? (int)verificationViewDto.A : 0);
                    slDoc.SetCellValue(iRow, 4, verificationViewDto.I.HasValue ? (int)verificationViewDto.I : 0);
                    slDoc.SetCellValue(iRow, 5, verificationViewDto.S.HasValue ? (int)verificationViewDto.S : 0);
                    slDoc.SetCellValue(iRow, 6, verificationViewDto.C.HasValue ? (int)verificationViewDto.C : 0);
                    slDoc.SetCellValue(iRow, 7, verificationViewDto.CH.HasValue ? (int)verificationViewDto.CH : 0);
                    slDoc.SetCellValue(iRow, 8, verificationViewDto.CT.HasValue ? (int)verificationViewDto.CT : 0);
                    slDoc.SetCellValue(iRow, 9, verificationViewDto.SLS_SLP.HasValue ? (int)verificationViewDto.SLS_SLP : 0);
                    slDoc.SetCellValue(iRow, 10, verificationViewDto.ETC.HasValue ? (int)verificationViewDto.ETC : 0);
                    slDoc.SetCellValue(iRow, 11, verificationViewDto.Plant.HasValue ? (int)verificationViewDto.Plant : 0);
                    slDoc.SetCellValue(iRow, 12, verificationViewDto.Actual.HasValue ? (int)verificationViewDto.Actual : 0);
                    if (verificationViewDto.ProcessGroup != "Total")
                    {
                        slDoc.SetCellValue(iRow, 13, verificationViewDto.VerifySystem == 1 ? "Y" : "N");
                        slDoc.SetCellValue(iRow, 14, verificationViewDto.VerifyManual.HasValue ? (verificationViewDto.VerifyManual.Value ? "Y" : "N") : "N");
                    }
                    slDoc.SetCellStyle(iRow, 1, iRow, 14, style);
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
                //slDoc.AutoFitColumn(1, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "ProductionExecution_ProductionEntryVerification_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public ActionResult SubmitDatas(InsertUpdateData<ExePlantProductionEntryVerificationViewViewModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            var BrandCode = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "";
            var LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var paramsDate = bulkData.Parameters != null ? bulkData.Parameters["ProductionDate"] : "";
            var paramsUnit = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var paramsShift = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";
            var paramsYear = bulkData.Parameters != null ? bulkData.Parameters["Year"] : "";
            var paramsWeek = bulkData.Parameters != null ? bulkData.Parameters["Week"] : "";

            var productionDate = DateTime.ParseExact(paramsDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
            // save data
            if (bulkData.Edit != null)
            {
                foreach (var plantProductionGroup in from t in bulkData.Edit where t != null select Mapper.Map<ExePlantProductionEntryVerificationViewDTO>(t))
                {
                    try
                    {
                        if (!plantProductionGroup.AlreadySubmit || plantProductionGroup.ProductionEntryRelease) 
                        {
                            if (!string.IsNullOrEmpty(plantProductionGroup.ProductionEntryCode))
                            {
                                _executionPlantBll.UpdatExePlantProductionEntryVerificationWhenSubmit(plantProductionGroup);
                            }

                            if (!plantProductionGroup.Flag_Manual.HasValue) continue;
                            if (!plantProductionGroup.Flag_Manual.Value) continue;
                            //Generate ExeReportByGroups
                            _executionPlantBll.SubmitProductionEntryVerification(plantProductionGroup.LocationCode, plantProductionGroup.UnitCode, plantProductionGroup.BrandCode, plantProductionGroup.Shift, plantProductionGroup.KPSYear, plantProductionGroup.KPSWeek, plantProductionGroup.ProductionDate, plantProductionGroup.GroupCode, GetUserName());
                            if (!plantProductionGroup.VerifyManual.HasValue) continue;
                            if (!plantProductionGroup.VerifyManual.Value) continue;

                            //Generate Data for transaction log
                            _generalBll.ExeTransactionLog(new TransactionLogInput()
                            {
                                page = Enums.PageName.ProductionEntryVerification.ToString(),
                                ActionButton = Enums.ButtonName.Submit.ToString(),
                                UserName = GetUserName(),
                                TransactionCode = plantProductionGroup.ProductionEntryCode,
                                IDRole = CurrentUser.Responsibility.Role
                            });

                            //Generate Production Card
                            _executionPlantBll.SaveProductionCardFromProductionEntry(plantProductionGroup);

                            //Generate ExeReportByProcess
                            plantProductionGroup.UpdatedBy = GetUserName();
                            _executionPlantBll.InsertReportByProcess(plantProductionGroup.LocationCode, plantProductionGroup.BrandCode, plantProductionGroup.ProcessGroup, plantProductionGroup.KPSYear, plantProductionGroup.KPSWeek, plantProductionGroup.CreatedBy, plantProductionGroup.UpdatedBy, plantProductionGroup.ProductionDate, plantProductionGroup.UnitCode);
                            //If adjustment exist
                            var adjusmentInput = new ProductAdjustmentInput()
                            {
                                BrandCode = plantProductionGroup.BrandCode,
                                LocationCode = plantProductionGroup.LocationCode,
                                ProductionDate = plantProductionGroup.ProductionDate
                            };
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
                        }

                        resultJSonSubmitData = "Run submit data on background process.";
                        listResultJSon.Add(resultJSonSubmitData);
                    }
                    catch (Exception ex)
                    {
                        _vtlogger.Err(ex, new List<object> { bulkData.Edit }, "Failed to run submit data on background process.");
                        resultJSonSubmitData = "Failed to run submit data on background process.";
                        listResultJSon.Add(resultJSonSubmitData);
                        return Json(listResultJSon);
                    }
                }
            }
            #region switching brand
            try
            {
                var brandGroup = _masterDataBLL.GetBrandGruopCodeByBrandCode(BrandCode);
                _executionPlantBll.switchBrandExeReportByProcess(LocationCode, brandGroup, productionDate);
            }
            catch (Exception ex)
            {
                var brandGroup = _masterDataBLL.GetBrandGruopCodeByBrandCode(BrandCode);
                _vtlogger.Err(ex, new List<object> { brandGroup }, "EditAllProductionEntryVerification");
                return Json("Failed to run submit data on background process.");
            }
            #endregion
            #region Recalculate Stock
            _executionPlantBll.recalculateStockExeReportByProcess(LocationCode, BrandCode, productionDate);
            #endregion
            // comment email method based on ticket : http://tp.voxteneo.co.id/entity/8403
            //try
            //{
            //    var paramInput = new GetExePlantProductionEntryVerificationInput
            //    {
            //        KpsYear = Int32.Parse(paramsYear),
            //        KpsWeek = Int32.Parse(paramsWeek),
            //        LocationCode = LocationCode,
            //        BrandCode = BrandCode,
            //        UnitCode = paramsUnit,
            //        Shift = Int32.Parse(paramsShift),
            //        Date = productionDate
            //    };

            //    _executionOtherBll.SendEmailSubmitPlantEntryVerif(paramInput, GetUserName());
            //}
            //catch (Exception ex)
            //{
            //    resultJSonSendEmail = "Failed to send email.";
            //    listResultJSon.Add(resultJSonSendEmail);
            //}

            return Json(listResultJSon);
        }

        public ActionResult ReturnDatas(InsertUpdateData<ExePlantProductionEntryVerificationViewViewModel> bulkData) 
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            // Get parameters from filter
            var filterLocation = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var filterUnitCode = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var filterShift = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";
            var filterBrand = bulkData.Parameters != null ? bulkData.Parameters["Brand"] : "";
            var filterYear = bulkData.Parameters != null ? bulkData.Parameters["Year"] : "";
            var filterWeek = bulkData.Parameters != null ? bulkData.Parameters["Week"] : "";
            var filterDate = bulkData.Parameters != null ? Convert.ToDateTime(bulkData.Parameters["Date"]) : DateTime.Now;

            // Return data
            if (bulkData.Edit != null)
            {
                try
                {
                    foreach (var plantProductionGroup in from t in bulkData.Edit where t != null && t.ProcessGroup != "Total" && t.Flag_Manual == true && t.VerifySystem > 0 select Mapper.Map<ExePlantProductionEntryVerificationViewDTO>(t))
                    {
                        bool isProdCardSubmitted = false;

                        int day = filterDate.DayOfWeek == 0 ? 7 : (int)filterDate.DayOfWeek;

                        var destinationGroupCode = plantProductionGroup.GroupCode;

                        if (plantProductionGroup.GroupCode.Substring(1, 1) == "5") destinationGroupCode = plantProductionGroup.GroupCode.Remove(1, 1).Insert(1, "1");

                        // Create combine code
                        var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                                  + filterLocation + "/"
                                                  + filterShift + "/"
                                                  + filterUnitCode + "/"
                                                  + destinationGroupCode + "/"
                                                  + filterBrand + "/"
                                                  + filterYear + "/"
                                                  + filterWeek + "/"
                                                  + day;

                        var transLogVerification = _utilitiesBLL.GetLatestActionTransLogExceptSave(productionEntryCode, Enums.PageName.ProductionEntryVerification.ToString());
                        var translogEntry = _utilitiesBLL.GetLatestActionTransLogExceptSave(productionEntryCode, Enums.PageName.PlantProductionEntry.ToString());

                        var transLogProdCard = productionEntryCode + "/" + _executionPlantBll.GetLatestProdCardRevType(filterLocation, filterUnitCode, filterBrand, plantProductionGroup.ProcessGroup, destinationGroupCode, filterDate);

                        var transLogProdCardSubmitted = _utilitiesBLL.GetLatestActionTransLogExceptSave(transLogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCard.ToString());

                        if (transLogProdCardSubmitted != null) 
                        {
                            isProdCardSubmitted = transLogProdCardSubmitted.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                            if (transLogVerification != null) 
                            {
                                if (transLogVerification.CreatedDate < transLogProdCardSubmitted.CreatedDate) {
                                    isProdCardSubmitted = transLogProdCardSubmitted.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                                }
                                else {
                                    isProdCardSubmitted = false;
                                }
                            }

                            if (translogEntry != null) {
                                if (translogEntry.CreatedDate > transLogProdCardSubmitted.CreatedDate) {
                                    isProdCardSubmitted = false;
                                }
                            }
                        }

                        if (!isProdCardSubmitted && plantProductionGroup.VerifySystem == 1) 
                        {
                            // Create combine code
                            productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                                      + filterLocation + "/"
                                                      + filterShift + "/"
                                                      + filterUnitCode + "/"
                                                      + plantProductionGroup.GroupCode + "/"
                                                      + filterBrand + "/"
                                                      + filterYear + "/"
                                                      + filterWeek + "/"
                                                      + day;

                            //Update Verify System = false
                            _executionPlantBll.ReturnProductionEntryVerification(productionEntryCode);

                            //Delete ExeReportByGroups
                            _exeReportBLL.DeleteReportByGroups(filterLocation, plantProductionGroup.GroupCode, filterBrand, filterDate);

                            // Dete ExeReportByGroups for Dummy group
                            _exeReportBLL.DeleteReportByGroups(filterLocation, plantProductionGroup.GroupCode.Remove(1, 1).Insert(1, "5"), filterBrand, filterDate);

                            // Delete ExeReportByProcess
                            _exeReportBLL.DeleteReportByProcess(filterLocation, filterUnitCode, plantProductionGroup.ProcessGroup, filterBrand, filterDate);

                            //Generate transaction log return verifikasi
                            _generalBll.ExeTransactionLog(new TransactionLogInput()
                            {
                                page = Enums.PageName.ProductionEntryVerification.ToString(),
                                ActionButton = Enums.ButtonName.Return.ToString(),
                                UserName = GetUserName(),
                                TransactionCode = productionEntryCode,
                                IDRole = CurrentUser.Responsibility.Role
                            });

                            // Delete production card
                            _plantWagesExecutionBll.DeleteProdCardByReturnVerificationRevType(new GetProductionCardInput
                            {
                                LocationCode = filterLocation,
                                Unit = filterUnitCode,
                                Brand = filterBrand,
                                Process = plantProductionGroup.ProcessGroup,
                                Group = plantProductionGroup.GroupCode,
                                Date = filterDate
                            }, productionEntryCode, GetUserName());

                            //Generate transaction for cancel submit in production entry
                            _generalBll.ExeTransactionLog(new TransactionLogInput()
                            {
                                page = Enums.PageName.PlantProductionEntry.ToString(),
                                ActionButton = EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit),
                                UserName = GetUserName(),
                                TransactionCode = productionEntryCode,
                                IDRole = CurrentUser.Responsibility.Role
                            });
                        }
                    }

                    resultJSonSubmitData = "Returning data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { filterLocation, filterUnitCode, filterShift, filterBrand, filterYear, filterWeek, filterDate }, "Return - Plant Production Entry Verification");
                    resultJSonSubmitData = "Failed to run submit data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                    return Json(listResultJSon);
                }

                //disable email method based on ticket : http://tp.voxteneo.co.id/entity/8403
                //try
                //{
                //    foreach (var plantProductionGroup in from t in bulkData.Edit where t != null && t.ProcessGroup != "Total" && t.Flag_Manual == true select Mapper.Map<ExePlantProductionEntryVerificationViewDTO>(t))
                //    {
                //        if (!plantProductionGroup.ProductionCardSubmit && plantProductionGroup.VerifySystem == 1)
                //        {
                //            var paramInput = new GetExePlantProductionEntryVerificationInput
                //            {
                //                LocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "",
                //                UnitCode = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "",
                //                Shift = bulkData.Parameters != null ? Convert.ToInt32(bulkData.Parameters["Shift"]) : 1,
                //                GroupCode = plantProductionGroup.GroupCode,
                //                BrandCode = bulkData.Parameters != null ? bulkData.Parameters["Brand"] : "",
                //                ProcessGroup = plantProductionGroup.ProcessGroup,
                //                KpsYear = bulkData.Parameters != null ? Convert.ToInt32(bulkData.Parameters["Year"]) : DateTime.Now.Year,
                //                KpsWeek = bulkData.Parameters != null ? Convert.ToInt32(bulkData.Parameters["Week"]) : 1,
                //                ProductionDate = bulkData.Parameters != null ? Convert.ToDateTime(bulkData.Parameters["Date"]) : DateTime.Now.Date
                //            };

                //            _executionOtherBll.SendEmailReturnEntryVerification(paramInput, GetUserName());
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    resultJSonSendEmail = "Failed to send email.";

                //    listResultJSon.Add(resultJSonSendEmail);

                //    return Json(listResultJSon);
                //}
            }
            return Json(listResultJSon);
        }

        public ActionResult SubmitEntryVerificationOnTuning(InsertUpdateData<ExePlantProductionEntryVerificationViewViewModel> bulkData) 
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            var isEmpty = true;

            var locationCode    = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var unitCode        = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var shift           = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";
            var brandCode       = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "";
            var kpsYear         = bulkData.Parameters != null ? bulkData.Parameters["Year"] : "";
            var kpsWeek         = bulkData.Parameters != null ? bulkData.Parameters["Week"] : "";
            var dateString      = bulkData.Parameters != null ? bulkData.Parameters["Date"] : "";

            var productionDate = String.IsNullOrEmpty(dateString) ? DateTime.Now.Date : Convert.ToDateTime(dateString);
            var dayOfWeek = (int)productionDate.DayOfWeek == 0 ? 7 : (int)productionDate.DayOfWeek;

            // Submit Verification
            if (bulkData.Edit != null)
            {
                isEmpty = false;
                try
                {
                    // Get Data from View Model bulkData
                    var listViewDto = from t in bulkData.Edit where t != null select Mapper.Map<ExePlantProductionEntryVerificationViewDTO>(t);
                    var listEntryVerificationDto = listViewDto.Where(c => c.ProcessGroup != "Total" && c.GroupCode.Substring(1, 1) != "5" && (c.AlreadySubmit == false || c.ProductionEntryRelease == true)).ToList();
                    var listEntryVerificationDtoWithDummy = listViewDto.Where(c => c.ProcessGroup != "Total" && (c.AlreadySubmit == false || c.ProductionEntryRelease == true)).ToList();

                    var brandGroupCode = _masterDataBLL.GetBrandGruopCodeByBrandCode(brandCode);
                    var isHoliday = _masterDataBLL.IsHoliday(productionDate, locationCode);
                    var mstStandardHour = (isHoliday) ? _masterDataBLL.GetStandardHourByDayTypeDay(dayOfWeek, "Holiday") :
                                                    _masterDataBLL.GetStandardHourByDayTypeDay(dayOfWeek, "Non-Holiday");

                    //foreach (var itemVerification in listEntryVerificationDtoWithDummy)
                    //{
                    //    if (!itemVerification.Flag_Manual.HasValue) continue;
                    //    if (!itemVerification.Flag_Manual.Value) continue;
                    //    if (!itemVerification.VerifyManual.HasValue) continue;
                    //    if (!itemVerification.VerifyManual.Value) continue;

                    //    // Generate Production Card
                    //    _executionPlantBll.SaveProductionCardFromProductionEntryTuning(itemVerification, brandGroupCode, mstStandardHour);
                    //}

                    var inputProdCard = new GetProductionCardInput(){
                        LocationCode = locationCode,
                        Unit         = unitCode,
                        Brand        = brandCode,
                        Shift        = Convert.ToInt32(shift),
                        KPSWeek      = Convert.ToInt32(kpsWeek),
                        KPSYear      = Convert.ToInt32(kpsYear),
                        Date         = productionDate,
                        UserName     = GetUserName()
                    };

                    _executionPlantBll.GenerateProductionCardVerification_SP(
                            listEntryVerificationDtoWithDummy.Where(c => c.Flag_Manual.HasValue && c.Flag_Manual.Value && c.VerifyManual.HasValue && c.VerifyManual.Value).ToList(),
                            inputProdCard);

                    var listProcessToBeSubmitted = new List<string>();

                    foreach (var itemVerification in listEntryVerificationDto)
                    {
                        if (!itemVerification.Flag_Manual.HasValue) continue;
                        if (!itemVerification.Flag_Manual.Value) continue;
                        if (!itemVerification.VerifyManual.HasValue) continue;
                        if (!itemVerification.VerifyManual.Value) continue;

                        listProcessToBeSubmitted.Add(itemVerification.ProcessGroup);

                        // Update WorkHour and Productivity ExeReportByGroups
                        var input = new GetExePlantProductionEntryVerificationInput
                        {
                            LocationCode = locationCode,
                            UnitCode = unitCode,
                            GroupCode = itemVerification.GroupCode,
                            ProcessGroup = itemVerification.ProcessGroup,
                            BrandCode = brandCode,
                            KpsWeek = String.IsNullOrEmpty(kpsWeek) ? 0 : Convert.ToInt32(kpsWeek),
                            KpsYear = String.IsNullOrEmpty(kpsYear) ? 0 : Convert.ToInt32(kpsYear),
                            ProductionDate = productionDate,
                            Shift = String.IsNullOrEmpty(shift) ? 0 : Convert.ToInt32(shift)
                        };
                        _executionPlantBll.UpdateWorkHourReportByGroup(input, itemVerification.TotalActualValue, GetUserName());

                        // Generate Data for transaction log
                        _generalBll.ExeTransactionLog(new TransactionLogInput()
                        {
                            page = Enums.PageName.ProductionEntryVerification.ToString(),
                            ActionButton = Enums.ButtonName.Submit.ToString(),
                            UserName = GetUserName(),
                            TransactionCode = itemVerification.ProductionEntryCode,
                            IDRole = CurrentUser.Responsibility.Role
                        });

                    }

                    var weekParam = String.IsNullOrEmpty(kpsWeek) ? 0 : Convert.ToInt32(kpsWeek);
                    var yearParam = String.IsNullOrEmpty(kpsYear) ? DateTime.Now.Year : Convert.ToInt32(kpsYear);
                   
                    // Generate default by process UOM 1 - 14 based on data H-1
                    //_executionPlantBll.InsertDefaultExeReportByProcess(locationCode, brandCode, unitCode, yearParam, weekParam, productionDate, GetUserName(), GetUserName());
                    //_executionReportBll.UpdateEndingStockByProcess(locationCode, unitCode, brandCode, productionDate);

                    // Change all process must be submitted, keep all process in listProcessToBeSubmitted List
                    if (listProcessToBeSubmitted.Any()) {
                        listProcessToBeSubmitted.Clear();
                        listProcessToBeSubmitted = _executionPlantBll.GetListProcessVerification(locationCode, unitCode, brandCode, Convert.ToInt32(shift), productionDate).ToList();
                    }

                    //Generate ExeReportByProcess
                    foreach (var process in listProcessToBeSubmitted)
                    {
                        var dto = listEntryVerificationDto.FirstOrDefault();

                        _executionPlantBll.InsertReportByProcess(locationCode, brandCode, process, yearParam, weekParam, GetUserName(), GetUserName(), productionDate, unitCode);

                        //If adjustment exist
                        var adjusmentInput = new ProductAdjustmentInput()
                        {
                            BrandCode = brandCode,
                            LocationCode = locationCode,
                            ProductionDate = productionDate
                        };
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
                    }

                    //_executionPlantBll.RunSSISUpdateByGroupWeekly();
                    //_executionPlantBll.RunSSISUpdateByGroupMonthly();

                    resultJSonSubmitData = "Run submit data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { locationCode, unitCode, shift, brandCode, kpsYear, kpsWeek, productionDate }, "Submit - Plant Production Entry Verification");
                    resultJSonSubmitData = "Failed to run submit data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                    return Json(listResultJSon);
                }
            }
            #region switching brand
            try
            {
                var brandGroup = _masterDataBLL.GetBrandGruopCodeByBrandCode(brandCode);
                _executionPlantBll.switchBrandExeReportByProcess(locationCode, brandGroup, productionDate);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, shift, brandCode, kpsYear, kpsWeek, productionDate }, "Submit - Plant Production Entry Verification - switching brand");
                return Json("Failed to run submit data on background process.");
            }
            #endregion
            #region Recalculate Stock
            try
            {
                _executionPlantBll.recalculateStockExeReportByProcess(locationCode, brandCode, productionDate);
                var week = String.IsNullOrEmpty(kpsWeek) ? 0 : Convert.ToInt32(kpsWeek);
                var year = String.IsNullOrEmpty(kpsYear) ? DateTime.Now.Year : Convert.ToInt32(kpsYear);
                //if(!isEmpty)
                   // _executionPlantBll.DefaultExeReportByProcess(locationCode, brandCode, unitCode, year, week, productionDate, GetUserName(), GetUserName());
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, shift, brandCode, kpsYear, kpsWeek, productionDate }, "Submit - Plant Production Entry Verification - Recalculate Stock");
                return Json("Failed to run submit data on background process.");
            }
            #endregion

            try
            {
                var paramInput = new GetExePlantProductionEntryVerificationInput
                {
                    KpsWeek = String.IsNullOrEmpty(kpsWeek) ? 0 : Convert.ToInt32(kpsWeek),
                    KpsYear = String.IsNullOrEmpty(kpsYear) ? 0 : Convert.ToInt32(kpsYear),
                    LocationCode = locationCode,
                    BrandCode = brandCode,
                    UnitCode = unitCode,
                    Shift = String.IsNullOrEmpty(shift) ? 0 : Convert.ToInt32(shift),
                    Date = productionDate
                };

                _executionOtherBll.SendEmailSubmitPlantEntryVerif(paramInput, GetUserName());
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unitCode, shift, brandCode, kpsYear, kpsWeek, productionDate }, "Submit - Plant Production Entry Verification - Send Email");
                resultJSonSendEmail = "Failed to send email.";
                listResultJSon.Add(resultJSonSendEmail);
            }

            return Json(listResultJSon);
        }
    }
}