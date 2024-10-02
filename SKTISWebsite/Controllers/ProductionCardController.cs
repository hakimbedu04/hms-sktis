using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper.Internal;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ProductionCard;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using AutoMapper;
using Color = System.Drawing.Color;
using SKTISWebsite.Models.UtilTransactionLog;

namespace SKTISWebsite.Controllers
{
    public class ProductionCardController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionPlantBLL _executionPlant;
        private IPlantWagesExecutionBLL _exePlantWagesExecutionBll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtLogger;

        public ProductionCardController(IApplicationService applicationService, IMasterDataBLL masterDataBll, IPlantWagesExecutionBLL plantWagesExecutionBll, IExecutionPlantBLL executionPlantBll, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBll, IVTLogger vt)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _exePlantWagesExecutionBll = plantWagesExecutionBll;
            _executionPlant = executionPlantBll;
            _generalBll = generalBll;
            _utilitiesBLL = utilitiesBll;
            _vtLogger = vt;
            SetPage("PlantWages/Execution/ProductionCard");
        }

        // GET: ProductionCard
        public ActionResult Index(string param1, string param2, int? param3, string param4, int? param5, int? param6, string param7, int? param8)
        {
            if (param8.HasValue) setResponsibility(param8.Value);
            var init = new InitProductionCardViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                PLNTChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString()),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                AbsentTypes = _svc.GetAllAbsentTypes(),
                AbsentTypeLookupLists = _svc.GetAbsentTypeLookupListsForSuratPeriode(),
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

        public JsonResult RoleButton(GetProductionCardInput input)
        {
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
                Save = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker((latestTransLogProdCard == null ? eblTransCode:transactionCode), CurrentUser.Responsibility.Role, Enums.PageName.ProductionCard.ToString(), Enums.ButtonName.Save.ToString())),
                Submit = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker(transactionCode, CurrentUser.Responsibility.Role, Enums.PageName.ProductionCard.ToString(), Enums.ButtonName.Submit.ToString())),
                CancelSubmit = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker(transactionCode, CurrentUser.Responsibility.Role, Enums.PageName.ProductionCard.ToString(), EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit)))
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
        public JsonResult GetBrandCodeByLocationCode(string locationCode, string unit, string process, string productionDate)
        {
            var result = _svc.GetBrandCodeFromProductionCardByLocation(locationCode, unit, process, !string.IsNullOrEmpty(productionDate) ? DateTime.Parse(productionDate) : DateTime.Now.Date);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupCodeByLocationCode(string locationCode, string unit, string process, string productionDate, string page)
        {
            var result = _svc.GetGroupCodeFromProductionCardByLocation(locationCode, unit, process, !string.IsNullOrEmpty(productionDate) ? DateTime.Parse(productionDate) : DateTime.Now.Date, page);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllGroupCodeByLocationCode(string locationCode, string unit, string productionDate)
        {
            var result = _svc.GetAllGroupCodeFromProductionCardByLocation(locationCode, unit, !string.IsNullOrEmpty(productionDate) ? DateTime.Parse(productionDate) : DateTime.Now.Date);
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
            var result = _executionPlant.GetAbsenteeimForSuratPeriodeComposites(employeeId, productionDate, locationCode, unitCode, shift, groupCode, processGroup, brandCode, remark).OrderBy(c => c.ProductionDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSuratPeriodeLalu(string employeeId, string productionDate, string locationCode, string unitCode, int shift, string groupCode, string processGroup, string brandCode, int revisionType, string Remark,List<SuratPeriodeLaluInput> SuratPeriodeLalu)
        {
                // update / save remark
            if (SuratPeriodeLalu.Count() > 0)
            {
                _exePlantWagesExecutionBll.UpdateSuratPeriodeLalu(employeeId, productionDate,locationCode, unitCode, shift, groupCode, processGroup, brandCode, revisionType, Remark,SuratPeriodeLalu);
            }
                

            return Json(SuratPeriodeLalu, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult SaveProductionCards(InsertUpdateData<ProductionCardViewModel> bulkData)
        {
            var saveButton = Boolean.Parse(bulkData.Parameters["saveButton"]);
            int counter = 0;
            // save data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    //var upah = float.Parse(bulkData.Edit[i].UpahLain, CultureInfo.InvariantCulture.NumberFormat);
                    var plantProductionGroup = Mapper.Map<ProductionCardDTO>(bulkData.Edit[i]);
                    plantProductionGroup.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _exePlantWagesExecutionBll.SaveProductionCard(plantProductionGroup);

                        if (saveButton && counter ==0)
                        {

                            var transactionCode =
                                item.ProductionCardCode + "/" + item.RevisionType;

                            //var latestTransLogProdCard = _utilitiesBLL.GetLatestActionTransLogExceptSave(transactionCode, Enums.PageName.ProductionCard.ToString());
                            var latestTransLogOnProdcard = _utilitiesBLL.GetAvailableProdCardTranssactionLog(transactionCode);
                            if (latestTransLogOnProdcard != null)
                            {
                                if (latestTransLogOnProdcard.IDFlow != 22 && latestTransLogOnProdcard.IDFlow != 23 &&
                                    latestTransLogOnProdcard.IDFlow != 69 && latestTransLogOnProdcard.IDFlow != 56 && latestTransLogOnProdcard.IDFlow != 25)
                                {
                                    GenerateDataToTransactionLog(new TransactionLogInput()
                                    {
                                        page = Enums.PageName.ProductionCard.ToString(),
                                        ActionButton = Enums.ButtonName.Save.ToString(),
                                        UserName = GetUserName(),
                                        TransactionCode =
                                            //plantProductionGroup.ProductionCardCode + "/" +
                                            //plantProductionGroup.RevisionType,
                                            transactionCode,
                                        IDRole = CurrentUser.Responsibility.Role
                                    });
                                }
                            }
                            else
                            {
                                GenerateDataToTransactionLog(new TransactionLogInput()
                                {
                                    page = Enums.PageName.ProductionCard.ToString(),
                                    ActionButton = Enums.ButtonName.Save.ToString(),
                                    UserName = GetUserName(),
                                    TransactionCode =
                                        //plantProductionGroup.ProductionCardCode + "/" +
                                        //plantProductionGroup.RevisionType,
                                        transactionCode,
                                    IDRole = CurrentUser.Responsibility.Role
                                });
                            }
                            counter = 1;
                        }

                        bulkData.Edit[i] = Mapper.Map<ProductionCardViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtLogger.Err(ex, new List<object> {plantProductionGroup});
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

        // save & submit all group (only insert translog)
        [HttpPost]
        public JsonResult SubmitAllGroupProductionCards(InsertUpdateData<ProductionCardViewModel> bulkData)
        {
            //if (bulkData.Edit == null) continue;
            // initialize parameter
            var locationCode = bulkData.Parameters["locationCode"];
            var unitCode = bulkData.Parameters["unitCode"];
            var shift = bulkData.Parameters["shift"];
            var brand = bulkData.Parameters["brandCode"];
            var kpsYear = bulkData.Parameters["year"];
            var kpsWeek = bulkData.Parameters["week"];
            var day = DateTime.Parse(bulkData.Parameters["date"]).DayOfWeek == 0 ? 7 : (int)DateTime.Parse(bulkData.Parameters["date"]).DayOfWeek;

            if (bulkData.Edit == null) return Json("Run submit data on background process.");
            try
            {
                // loop save (insert translog save)
                foreach (var transactionCode in bulkData.Edit.Select(t => EnumHelper.GetDescription(Enums.CombineCode.WPC) + "/"
                                                                          + locationCode + "/"
                                                                          + shift + "/"
                                                                          + unitCode + "/"
                                                                          + t.GroupCode + "/"
                                                                          + brand + "/"
                                                                          + kpsYear + "/"
                                                                          + kpsWeek + "/"
                                                                          + day + "/"
                                                                          + t.RevisionType))
                {
                    GenerateDataToTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.ProductionCard.ToString(),
                        ActionButton = Enums.ButtonName.Save.ToString(),
                        UserName = GetUserName(),
                        TransactionCode = transactionCode,
                        ActionTime = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                }

                // loop save (insert translog save)
                foreach (var transactionCode in bulkData.Edit.Select(t => EnumHelper.GetDescription(Enums.CombineCode.WPC) + "/"
                                                                          + locationCode + "/"
                                                                          + shift + "/"
                                                                          + unitCode + "/"
                                                                          + t.GroupCode + "/"
                                                                          + brand + "/"
                                                                          + kpsYear + "/"
                                                                          + kpsWeek + "/"
                                                                          + day + "/"
                                                                          + t.RevisionType))
                {
                    GenerateDataToTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.ProductionCard.ToString(),
                        ActionButton = Enums.ButtonName.Submit.ToString(),
                        UserName = GetUserName(),
                        TransactionCode = transactionCode,
                        ActionTime = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                }
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

            return Json("Run submit data on background process.");
        }

        [HttpPost]
        public JsonResult SubmitProductionCards(InsertUpdateData<ProductionCardViewModel> bulkData)
        {
            var submitButton = Boolean.Parse(bulkData.Parameters["submitButton"]);

            if (bulkData.Edit == null)
                return submitButton ? Json("Run submit data on background process.") : Json("Run cancel submit data on background process.");
            
            var plantProductionGroup = Mapper.Map<ProductionCardDTO>(bulkData.Edit[0]);
            try
            {
                var transactionCode = plantProductionGroup.ProductionCardCode + "/" + plantProductionGroup.RevisionType;

                //var latestTransLogProdCard = _utilitiesBLL.GetLatestActionTransLogExceptSave(transactionCode, Enums.PageName.ProductionCard.ToString());
                var latestTransLogOnProdcard = _utilitiesBLL.GetAvailableProdCardTranssactionLog(transactionCode);

                if (latestTransLogOnProdcard != null)
                {

                    if (latestTransLogOnProdcard.IDFlow != 22 && latestTransLogOnProdcard.IDFlow != 23 &&
                        latestTransLogOnProdcard.IDFlow != 69 && latestTransLogOnProdcard.IDFlow != 56 && latestTransLogOnProdcard.IDFlow != 25)
                    {
                        GenerateDataToTransactionLog(new TransactionLogInput()
                        {
                            page = Enums.PageName.ProductionCard.ToString(),
                            ActionButton =
                                submitButton
                                    ? Enums.ButtonName.Submit.ToString()
                                    : EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit),
                            UserName = GetUserName(),
                            TransactionCode =
                                plantProductionGroup.ProductionCardCode + "/" + plantProductionGroup.RevisionType,
                            ActionTime = DateTime.Now,
                            TransactionDate = DateTime.Now,
                            IDRole = CurrentUser.Responsibility.Role

                        });
                    }
                }

            }
            catch (ExceptionBase ex)
            {
                _vtLogger.Err(ex);
                return submitButton ? Json("Failed to run submit data on background process." + ex.Message) : Json("Failed to run cancel submit data on background process." + ex.Message);
            }
            catch (Exception ex)
            {
                _vtLogger.Err(ex);
                return submitButton ? Json("Failed to run submit data on background process." + ex.Message) : Json("Failed to run cancel submit data on background process." + ex.Message);
            }
            #region Send Mail
            // disable method send email based on related ticket :http://tp.voxteneo.co.id/entity/8427
            //try
            //{
            //    //SEND MAIL
            //    var input = new GetProductionCardInput
            //    {
            //        LocationCode = bulkData.Parameters["locationCode"],
            //        Unit = bulkData.Parameters["unitCode"],
            //        Group = bulkData.Parameters["groupCode"],
            //        Brand = bulkData.Parameters["brandCode"],
            //        Shift = int.Parse(bulkData.Parameters["shift"]),
            //        KPSYear = int.Parse(bulkData.Parameters["year"]),
            //        KPSWeek = int.Parse(bulkData.Parameters["week"]),
            //        Date = DateTime.ParseExact(bulkData.Parameters["date"], Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
            //    };
            //    _exePlantWagesExecutionBll.sendMail(input, GetUserName(), submitButton);
            //}
            //catch (Exception e)
            //{
            //    return Json("Failed to send email.");
            //}
            #endregion

            return submitButton ? Json("Run submit data on background process.") : Json("Run cancel submit data on background process.");
        }

        [HttpPost]
        public JsonResult CancelSubmitProductionCards(InsertUpdateData<ProductionCardViewModel> bulkData)
        {
            var submitButton = Boolean.Parse(bulkData.Parameters["submitButton"]);

            if (bulkData.Edit != null)
            {
                //for (var i = 0; i < bulkData.Edit.Count; i++)
                //{
                //    //check row is null
                //    if (bulkData.Edit[i] == null) continue;

                //}
                var plantProductionGroup = Mapper.Map<ProductionCardDTO>(bulkData.Edit[0]);
                try
                {
                    var transactionCode =
                               plantProductionGroup.ProductionCardCode + "/" + plantProductionGroup.RevisionType;

                    //var latestTransLogProdCard = _utilitiesBLL.GetLatestActionTransLogExceptSave(transactionCode, Enums.PageName.ProductionCard.ToString());
                    var latestTransLogOnProdcard = _utilitiesBLL.GetAvailableProdCardTranssactionLog(transactionCode);

                    if (latestTransLogOnProdcard != null)
                    {

                        if (latestTransLogOnProdcard.IDFlow != 57)
                        {
                            GenerateDataToTransactionLog(new TransactionLogInput()
                            {
                                page = Enums.PageName.ProductionCard.ToString(),
                                ActionButton = submitButton ? Enums.ButtonName.Submit.ToString() : EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit),
                                UserName = GetUserName(),
                                TransactionCode = plantProductionGroup.ProductionCardCode + "/" + plantProductionGroup.RevisionType,
                                ActionTime = DateTime.Now,
                                TransactionDate = DateTime.Now,
                                IDRole = CurrentUser.Responsibility.Role
                            });

                        }
                    }

                }
                catch (ExceptionBase ex)
                {
                    _vtLogger.Err(ex);
                    return submitButton ? Json("Failed to run submit data on background process." + ex.Message) : Json("Failed to run cancel submit data on background process." + ex.Message);
                }
                catch (Exception ex)
                {
                    _vtLogger.Err(ex);
                    return submitButton ? Json("Failed to run submit data on background process." + ex.Message) : Json("Failed to run cancel submit data on background process." + ex.Message);
                }
                #region Send Mail
                // disable method send email based on related ticket :http://tp.voxteneo.co.id/entity/8427
                //try
                //{
                //    //SEND MAIL
                //    var input = new GetProductionCardInput
                //    {
                //        LocationCode = bulkData.Parameters["locationCode"],
                //        Unit = bulkData.Parameters["unitCode"],
                //        Group = bulkData.Parameters["groupCode"],
                //        Brand = bulkData.Parameters["brandCode"],
                //        Shift = int.Parse(bulkData.Parameters["shift"]),
                //        KPSYear = int.Parse(bulkData.Parameters["year"]),
                //        KPSWeek = int.Parse(bulkData.Parameters["week"]),
                //        Date = DateTime.ParseExact(bulkData.Parameters["date"], Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
                //    };
                //    _exePlantWagesExecutionBll.sendMail(input, GetUserName(), submitButton);
                //}
                //catch (Exception e)
                //{
                //    return Json("Failed to send email.");
                //}
                #endregion
            }
            return submitButton ? Json("Run submit data on background process.") : Json("Run cancel submit data on background process.");
        }

        public void GenerateDataToTransactionLog(TransactionLogInput input)
        {
            _generalBll.ExeTransactionLog(input);
        }

        [HttpPost]
        public JsonResult GetProductionCards(GetProductionCardInput criteria)
        {
            var result = _exePlantWagesExecutionBll.GetProductionCards(criteria);
            var viewModel = Mapper.Map<List<ProductionCardViewModel>>(result);
            var pageResult = new PageResult<ProductionCardViewModel>(viewModel);
            return Json(pageResult);
        }

        [HttpPost]
        public JsonResult GetTotalProdAndUpahLainBasedOnGroupCode(GetProductionCardInputGroupAll criteria)
        {
            var result = _exePlantWagesExecutionBll.GetProductionCardsGroupAll(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
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
            var transactionLog = _utilitiesBLL.GetTransactionHistory(input);
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
            //var pageResult = new PageResult<TransactionFlowViewModel>();
            //var transactionFlow = _utilitiesBLL.GetTransactionFlow(input);
            //pageResult.TotalRecords = transactionFlow.Count;
            //pageResult.TotalPages = (transactionFlow.Count / input.PageSize) + (transactionFlow.Count % input.PageSize != 0 ? 1 : 0);
            //var result = transactionFlow.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            //pageResult.Results = Mapper.Map<List<TransactionFlowViewModel>>(result);
            //return Json(pageResult);

            var pageResult = new PageResult<TransactionFlowViewModel>();
            var transactionFlow = _utilitiesBLL.GetTransactionFlowApproval(input);
            pageResult.Results = Mapper.Map<List<TransactionFlowViewModel>>(transactionFlow);
            return Json(pageResult);
        }

        public FileStreamResult GenerateExcel(GetProductionCardInput criteria)
        {
            criteria.SortExpression = "EmployeeID";
            criteria.SortOrder = "ASC";

            var productionCards = _exePlantWagesExecutionBll.GetProductionCards(criteria);

            var ms = new MemoryStream();

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

            var allLocations = _svc.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == criteria.LocationCode)
                {
                    locationCompat = item.Text;
                }
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
                slDoc.SetCellValue(3, 7, criteria.KPSYear.ToString());
                slDoc.SetCellValue(4, 7, criteria.KPSWeek.ToString());
                slDoc.SetCellValue(5, 7, criteria.Date.ToString(Constants.DefaultDateOnlyFormat));
                var ci = CultureInfo.CurrentCulture;
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
                    slDoc.SetCellValue(iRow, 3, prodCard.Production.HasValue ? prodCard.Production.Value.ToString() : "");
                    slDoc.SetCellValue(iRow, 4, prodCard.Absent);
                    slDoc.SetCellValue(iRow, 5, prodCard.UpahLain.HasValue ? prodCard.UpahLain.Value.ToString("f2", ci) : "");
                    slDoc.SetCellValue(iRow, 6, prodCard.Remark);
                    slDoc.SetCellValue(iRow, 7, prodCard.Comments);
                    slDoc.SetCellStyle(iRow, 1, iRow, 7, style);
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
            var fileName = "PlantWages_ProductionCard_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public JsonResult GetLastConditionTranslogProdCard(GetProductionCardInput input)
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
                    var isProdCardReturn = false;
                    if (latestTransLogProdCardApproval != null)
                    {
                        isProdCardReturn = latestTransLogProdCardApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Return.ToString();
                    }
                     
                    if (!isProdCardReturn)
                    {
                        //var latestTransLogProdCardApproval = _utilitiesBLL.GetLatestActionTransLog(transLogProdCardCode, Enums.PageName.ProductionCardApprovalDetail.ToString());
                        if (latestTransLogProdCardApproval != null)
                        {
                            var isProdCardApproved = latestTransLogProdCardApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString();
                            if (latestTransLogProdCard.CreatedDate < latestTransLogProdCardApproval.CreatedDate)
                            {
                                if (isProdCardApproved)
                                {
                                    status = "Locked";
                                }
                                //else
                                //{
                                    //var isProdCardReturn = latestTransLogProdCardApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Return.ToString();
                                    //if (isProdCardReturn) 
                                    //{
                                    //    //status = "Open";
                                    //    status = "Submitted"; // jika prodcardapproval di return tetap di lock, data tetap berada di prodcard approval dengan status submitted dan current approver is ppc. Ticket http://tp.voxteneo.co.id/entity/10455
                                    //}
                                //}
                            }
                        }
                    }

                    //var transLogEntryReleaseApproval = _utilitiesBLL.GetLatestActionTransLog(productionEntryCode, Enums.PageName.EblekReleaseApproval.ToString());
                    //if (transLogEntryReleaseApproval != null)
                    //{
                    //    var isEblekReleaseApproved = transLogEntryReleaseApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString()
                    //                                   && transLogEntryReleaseApproval.IDFlow == (int)HMS.SKTIS.Core.Enums.IdFlow.EblekReleaseApprovalFinal;
                    //    if (latestTransLogProdCard != null)
                    //    {
                    //        if (latestTransLogProdCard.CreatedDate < transLogEntryReleaseApproval.CreatedDate)
                    //        {
                    //            if (isEblekReleaseApproved)
                    //            {
                    //                status = "Open";
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }
    }
}