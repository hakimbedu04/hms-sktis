using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper.Internal;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ProductionCard;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using AutoMapper;
using Color = System.Drawing.Color;
using SKTISWebsite.Models.UtilTransactionLog;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;

namespace SKTISWebsite.Controllers
{
    public class WagesProductionCardCorrectionController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionPlantBLL _executionPlant;
        private IPlantWagesExecutionBLL _exePlantWagesExecutionBll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtLogger;

        public WagesProductionCardCorrectionController(IApplicationService applicationService, IMasterDataBLL masterDataBll, IPlantWagesExecutionBLL plantWagesExecutionBll, IExecutionPlantBLL executionPlantBll, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBll, IVTLogger vt)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _exePlantWagesExecutionBll = plantWagesExecutionBll;
            _executionPlant = executionPlantBll;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBll;
            _vtLogger = vt;
            SetPage("PlantWages/Execution/ProductionCardRev.");
        }

        // GET: ProductionCard
        public ActionResult Index()
        {
            var init = new InitProductionCardViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                PLNTChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                AbsentTypes = _svc.GetAllAbsentTypes(),
                AbsentTypeLookupLists = _svc.GetAbsentTypeLookupListsForSuratPeriode()
            };

            return View("Index", init);
        }

        public JsonResult RoleButton(GetProductionCardInput input)
        {
            ////generate transactioncode
            //int day = input.Date.DayOfWeek == 0 ? 7 : (int)input.Date.DayOfWeek;
            //var transactionCode = "EBL" + "/" + input.LocationCode + "/" + input.Shift + "/" + input.Unit + "/" + input.Group + "/" + input.Brand + "/" + input.KPSYear + "/" + input.KPSWeek + "/" + day;

            //generate transactioncode
            int day = input.Date.DayOfWeek == 0 ? 7 : (int)input.Date.DayOfWeek;
            var transactionCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.WPC) + "/"
                                + input.LocationCode + "/"
                                + input.Shift + "/"
                                + input.Unit + "/"
                                + input.Group + "/"
                                + input.Brand + "/"
                                + input.KPSYear + "/"
                                + input.KPSWeek + "/"
                                + +day + "/"
                                + +input.RevisionType;
            var eblTransCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                + input.LocationCode + "/"
                                + input.Shift + "/"
                                + input.Unit + "/"
                                + input.Group + "/"
                                + input.Brand + "/"
                                + input.KPSYear + "/"
                                + input.KPSWeek + "/"
                                + day;

            //var latestTransLogProdCard = _utilitiesBLL.GetLatestActionTransLogExceptSave(transactionCode, Enums.PageName.ProductionCard.ToString());
            var latestTransLogProdCard = _utilitiesBLL.GetAvailableProdCardTranssactionLog(transactionCode);
            

            var init = new ButtonState()
            {
                Save = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker((latestTransLogProdCard == null ? eblTransCode : transactionCode), CurrentUser.Responsibility.Role, Enums.PageName.ProductionCard.ToString(), Enums.ButtonName.Save.ToString())),
                Submit = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker(transactionCode, CurrentUser.Responsibility.Role, Enums.PageName.ProductionCard.ToString(), Enums.ButtonName.Submit.ToString())),
                CancelSubmit = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker(transactionCode, CurrentUser.Responsibility.Role, Enums.PageName.ProductionCard.ToString(), Enums.ButtonName.CancelSubmit.ToString()))
            };
            return Json(init, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetUnitCodeSelectListByLocationCodeReportByGroup(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
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

        [HttpGet]
        public JsonResult GetProcessByLocationCode(string locationCode, string unit, int? shift, string productionDate)
        {
            var model = _svc.GetProcessGroupFromPlantEntryVerification(locationCode, unit, shift ?? 0, !string.IsNullOrEmpty(productionDate) ? DateTime.Parse(productionDate) : DateTime.Now.Date);
            var modelfiltered = model.Where(m => m.Text != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily));
            return Json(modelfiltered, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandCodeByLocationCode(string locationCode, string unit, int shift, string process, string productionDate, int KpsYear, int KpsWeek)
        {
            var result = _svc.GetBrandCodeFromProductionCardByLocation(locationCode, unit, shift, process, KpsYear, KpsWeek);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupCodeByLocationCode(string locationCode, string unit, int shift, string process, string productionDate, int KpsYear, int KpsWeek)
        {
            var result = _svc.GetGroupCodeFromProductionCardByLocation(locationCode, unit, shift, process, KpsYear, KpsWeek);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMaxDaySuratPeriodeLalu(string employeeId, string absentType, int year)
        {
            var result = _executionPlant.GetMaxDayProdCard(employeeId, absentType, year);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSuratPeriode(string employeeId, string productionDate, string locationCode, string unitCode, int shift, string groupCode, string processGroup, string brandCode, string remark)
        {
            var result = _executionPlant.GetAbsenteeimForSuratPeriodeComposites(employeeId, productionDate, locationCode, unitCode, shift, groupCode, processGroup, brandCode, remark);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveProductionCards(InsertUpdateData<ProductionCardViewModel> bulkData)
        {
            var saveButton = Boolean.Parse(bulkData.Parameters["saveButton"]);

            // save data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    var plantProductionGroup = Mapper.Map<ProductionCardDTO>(bulkData.Edit[i]);
                    plantProductionGroup.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _exePlantWagesExecutionBll.SaveProductionCard(plantProductionGroup);

                        if (saveButton)
                            GenerateDataToTransactionLog(new TransactionLogInput()
                            {
                                page = Enums.PageName.ProductionCard.ToString(),
                                ActionButton = Enums.ButtonName.Save.ToString(),
                                UserName = GetUserName(),
                                TransactionCode = plantProductionGroup.ProductionCardCode + "/" + plantProductionGroup.RevisionType,
                                IDRole = CurrentUser.Responsibility.Role
                            });

                        bulkData.Edit[i] = Mapper.Map<ProductionCardViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtLogger.Err(ex, new List<object> { plantProductionGroup });
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtLogger.Err(ex, new List<object> { plantProductionGroup });
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                    }
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public JsonResult SubmitProductionCards(InsertUpdateData<ProductionCardViewModel> bulkData)
        {
            var submitButton = Boolean.Parse(bulkData.Parameters["submitButton"]);

            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    var plantProductionGroup = Mapper.Map<ProductionCardDTO>(bulkData.Edit[i]);
                    try
                    {
                        GenerateDataToTransactionLog(new TransactionLogInput()
                        {
                            page = Enums.PageName.ProductionCard.ToString(),
                            ActionButton = submitButton ? Enums.ButtonName.Submit.ToString() : EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit),
                            UserName = GetUserName(),
                            TransactionCode = plantProductionGroup.ProductionCardCode + "/" + plantProductionGroup.RevisionType,
                            ActionTime = DateTime.Now.AddSeconds(i),
                            TransactionDate = DateTime.Now.AddSeconds(i)
                            
                        });

                    }
                    catch (ExceptionBase ex)
                    {
                        _vtLogger.Err(ex);
                        return Json("Failed to run submit data on background process." + ex.Message);
                    }
                    catch (Exception ex)
                    {
                        _vtLogger.Err(ex);
                        return Json("Failed to run submit data on background process." + ex.Message);
                    }
                }
            }
            return Json("Run submit data on background process.");
        }

        public void GenerateDataToTransactionLog(TransactionLogInput input)
        {
            _generalBll.ExeTransactionLog(input);
        }

        [HttpPost]
        public JsonResult GetProductionCards(GetProductionCardInput criteria)
        {
            var result = _exePlantWagesExecutionBll.GetProductionCardsCorrection(criteria);
            var viewModel = Mapper.Map<List<ProductionCardViewModel>>(result);
            var pageResult = new PageResult<ProductionCardViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        /// <summary>
        /// Transaction History
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHistory(TransactionLogInput input)
        {
            var TransactionCode = this.GenerateTransactionCode(input.separator[0], new string[] { input.code_1, input.code_2, input.code_3, input.code_4, input.code_5, input.code_6, input.code_7, input.code_8, input.code_9 });

            if (input.code_9 != "")
            {
                DateTime enteredDate = DateTime.Parse(input.code_9);
                int day = enteredDate.DayOfWeek == 0 ? 7 : (int)enteredDate.DayOfWeek;
                input.code_9 = day.ToString();
            }


            var coba = _utilitiesBLL.GetTransactionLog(input);

            var pageResult = new PageResult<TransactionHistoryViewModel>();

            var transactionLogAll = _utilitiesBLL.GetTransactionHistoryWagesProdcardCorrection(input);
            var transactionLog = new List<TransactionHistoryDTO>();
            foreach (TransactionHistoryDTO item in transactionLogAll)
            {
                if (String.Equals(item.action, "Cancel Submit") || String.Equals(item.action, "Save") || String.Equals(item.action, "Submit"))
                {
                    transactionLog.Add(item);
                }
            }
            pageResult.TotalRecords = transactionLog.Count;
            pageResult.TotalPages = (transactionLog.Count / input.PageSize) + (transactionLog.Count % input.PageSize != 0 ? 1 : 0);
            var result = transactionLog.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
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


        public FileStreamResult GenerateExcel(GetProductionCardInput criteria)
        {
            criteria.SortExpression = "EmployeeID";
            criteria.SortOrder = "ASC";

            var productionCards = _exePlantWagesExecutionBll.GetProductionCardsCorrection(criteria);

            var ms = new MemoryStream();

            var allLocations = _svc.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == criteria.LocationCode)
                {
                    locationCompat = item.Text;
                }
            }

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.PlantWagesExcelTemplate.PlantWagesExecutionProductionCard + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 2, locationCompat);
                slDoc.SetCellValue(4, 2, criteria.Unit);
                slDoc.SetCellValue(5, 2, criteria.Shift.ToString());
                slDoc.SetCellValue(6, 2, criteria.Process);
                slDoc.SetCellValue(7, 2, criteria.Group);
                slDoc.SetCellValue(8, 2, criteria.Brand);
                slDoc.SetCellValue(3, 6, criteria.KPSYear.ToString());
                slDoc.SetCellValue(4, 6, criteria.KPSWeek.ToString());
                slDoc.SetCellValue(5, 6, criteria.Date.ToString(Constants.DefaultDateOnlyFormat));

                //row values
                var iRow = 12;

                foreach (var prodCard in productionCards)
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

                    slDoc.SetCellValue(iRow, 1, prodCard.EmployeeID);
                    slDoc.SetCellValue(iRow, 2, prodCard.EmployeeName);
                    slDoc.SetCellValue(iRow, 3, prodCard.Production.HasValue ? prodCard.Production.Value : 0);
                    slDoc.SetCellValue(iRow, 4, prodCard.UpahLain.HasValue ? prodCard.UpahLain.Value : 0);
                    slDoc.SetCellValue(iRow, 5, prodCard.Remark);
                    slDoc.SetCellValue(iRow, 6, prodCard.Comments);
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
                //slDoc.AutoFitColumn(1, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "PlantWages_ProductionCardCorrection_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public JsonResult GetLastConditionTranslogProdCardCorrection(GetProductionCardInput input)
        {
            DateTime date = input.Date;
            int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;

            var transLogProdCardCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.WPC) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.Unit + "/"
                                      + input.Group + "/"
                                      + input.Brand + "/"
                                      + input.KPSYear + "/"
                                      + input.KPSWeek + "/"
                                      + day + "/"
                                      + input.RevisionType;

            var productionEntryCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.EBL) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.Unit + "/"
                                      + input.Group + "/"
                                      + input.Brand + "/"
                                      + input.KPSYear + "/"
                                      + input.KPSWeek + "/"
                                      + day;

            var latestTransLogProdCard = _utilitiesBLL.GetLatestActionTransLogExceptSave(transLogProdCardCode, Enums.PageName.ProductionCard.ToString());

            var status = "Open";

            if (latestTransLogProdCard != null)
            {
                var submitted = latestTransLogProdCard.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                if (submitted)
                {
                    status = "Submitted";

                    var latestTransLogProdCardApproval = _utilitiesBLL.GetLatestActionTransLog(transLogProdCardCode, Enums.PageName.ProductionCardApprovalDetail.ToString());
                    if (latestTransLogProdCardApproval != null)
                    {
                        var isProdCardApproved = latestTransLogProdCardApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString();
                        if (latestTransLogProdCard.CreatedDate < latestTransLogProdCardApproval.CreatedDate)
                        {
                            if (isProdCardApproved)
                            {
                                status = "Locked";
                            }
                            else
                            {
                                var isProdCardReturn = latestTransLogProdCardApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Return.ToString();
                                if (isProdCardReturn)
                                {
                                    status = "Open";
                                }
                            }
                        }

                        var transLogVerification = _utilitiesBLL.GetLatestActionTransLogExceptSave(productionEntryCode, Enums.PageName.ProductionEntryVerification.ToString());
                        if (transLogVerification != null)
                        {
                            var isVerificationSubmitted = transLogVerification.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                            if (latestTransLogProdCard != null)
                            {
                                if (latestTransLogProdCardApproval.CreatedDate < transLogVerification.CreatedDate)
                                {
                                    if (isVerificationSubmitted)
                                    {
                                        status = "Open";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }
    }
}