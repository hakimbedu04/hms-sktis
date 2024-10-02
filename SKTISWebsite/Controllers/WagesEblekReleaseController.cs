using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.WagesEblekRelease;
using SpreadsheetLight;
using SKTISWebsite.Models.UtilTransactionLog;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;

namespace SKTISWebsite.Controllers
{
    public class WagesEblekReleaseController : BaseController
    {
        private IApplicationService _applicationService;
        private IMasterDataBLL _masterDataBll;
        private IPlanningBLL _planningBll;
        private IPlantWagesExecutionBLL _plantWagesExecutionBll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;

        public WagesEblekReleaseController(IApplicationService applicationService, IMasterDataBLL masterDataBll, IPlanningBLL planningBll, IPlantWagesExecutionBLL plantWagesExecutionBll, IGeneralBLL generalBll, IUtilitiesBLL UtilitiesBLL, IVTLogger vtlogger)
        {
            _applicationService = applicationService;
            _masterDataBll = masterDataBll;
            _planningBll = planningBll;
            _plantWagesExecutionBll = plantWagesExecutionBll;
            _generalBll = generalBll;
            _utilitiesBLL = UtilitiesBLL;
            SetPage("PlantWages/Execution/EblekRelease");
            _vtlogger = vtlogger;
        }

        [HttpGet]
        public JsonResult GetUnitSelectList(string locationCode)
        {
            var result = _applicationService.GetUnitCodeSelectListByLocationCode(locationCode).Where(p => p.Value != "MTNC"); //Exclude MNTC 
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShiftSelectList(string locationCode)
        {
            var result = _applicationService.GetShiftByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateSelectList(int year, int week)
        {
            var result = _applicationService.GetSelectListDateByYearAndWeek(year, week);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateNearestClosingPayrollList()
        {
            DateTime now = DateTime.Now;
            var result = _applicationService.GetSelectListNearestClosingPayroll(now);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandSelectList(string locationCode, string unitCode, int? shift, string datePopUp)
        {
            var result =
                _applicationService.GetBrandCodeFromExePlantProductionEntryVerification(new GetExePlantProductionEntryVerificationInput
                {
                    LocationCode = locationCode,
                    UnitCode = unitCode,
                    Shift = shift,
                    DatePopUp = String.IsNullOrEmpty(datePopUp) ? new DateTime() : Convert.ToDateTime(datePopUp)
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetProcessSelectList(string locationCode, string unitCode, int? shift, string datePopUp, string brand)
        {
            var result =
                _applicationService.GetProcessFromExePlantProductionEntryVerification(new GetExePlantProductionEntryVerificationInput
                {
                    LocationCode = locationCode,
                    UnitCode = unitCode,
                    Shift = shift,
                    DatePopUp = String.IsNullOrEmpty(datePopUp) ? new DateTime() : Convert.ToDateTime(datePopUp),
                    BrandCode = brand
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult RoleButton(GetExeProductionEntryReleaseInput input)
        //{
        //    //generate transactioncode
        //    var transactionCode = _plantWagesExecutionBll.GetExeProductionEntryReleasesNew(input).Select(x => x.ProductionEntryCode).FirstOrDefault();
        //    var transCode = "";
        //    if (transactionCode != null)
        //    {
        //        transCode = transactionCode.Replace("EBL", "WPC") + "/%";
        //    }
        //    var init = new ButtonState()
        //    {
        //        SendApproval = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker(transCode, CurrentUser.Responsibility.Role, Enums.PageName.EblekRelease.ToString(), Enums.ButtonName.SendApproval.ToString())),   
        //    };
        //    return Json(init, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult RoleButton(InsertUpdateData<ExeProductionEntryReleaseModel> bulkData)
        {
            var sendAppState = new List<bool>();
            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null || bulkData.New[i].Checkbox == false) continue;

                    var data = bulkData.New[i];
                    var transactionCode = data.ProductionEntryCode.Replace("EBL", "WPC")+"/%";
                    var content = new ButtonState()
                    {
                        SendApproval = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker(transactionCode, CurrentUser.Responsibility.Role, Enums.PageName.EblekRelease.ToString(), Enums.ButtonName.SendApproval.ToString())),
                    };
                    sendAppState.Add(content.SendApproval);

                }
            }
            var init = new ButtonState()
            {
                SendApproval = !sendAppState.Contains(false)
            };
            return Json(init, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListExeProductionEntryRelease(GetExeProductionEntryReleaseInput criteria)
        {
            var pageResult = new PageResult<ExeProductionEntryReleaseModel>();
            var eblekReleases = _plantWagesExecutionBll.GetExeProductionEntryReleasesNew(criteria);
            //pageResult.TotalRecords = eblekReleases.Count;
            //pageResult.TotalPages = (eblekReleases.Count / criteria.PageSize) + 
            //                        (eblekReleases.Count % criteria.PageSize != 0 ? 1 : 0);
            //var result = eblekReleases.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<ExeProductionEntryReleaseModel>>(eblekReleases);

            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetListExePlantProductionEntryVerification(GetExePlantProductionEntryVerificationInput criteria)
        {
            var pageResult = new PageResult<ExePlantProductionEntryVerificationViewModel>();
            var eblekReleases = _plantWagesExecutionBll.GetPlantProdVerificationFromEntryRelease(criteria);
            pageResult.TotalRecords = eblekReleases.Count;
            pageResult.TotalPages = (eblekReleases.Count / criteria.PageSize) +
                                    (eblekReleases.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = eblekReleases.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<ExePlantProductionEntryVerificationViewModel>>(eblekReleases);

            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocationName(string locationCode)
        {
            var result = _masterDataBll.GetMstGenLocation(new GetMstGenLocationInput
            {
                LocationCode = locationCode
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: WagesEblekRelease
        public ActionResult Index(string param1, string param2, int? param3, string param4, int? param5)
        {
            if (param5.HasValue) setResponsibility(param5.Value);
            var currentWeek = _masterDataBll.GetWeekByDate(DateTime.Now);
            var dateNearestClosingPayroll = _masterDataBll.GetNearestClosingPayrollDate(DateTime.Now).Select(m => m.Date.ToString(Constants.DefaultDateOnlyFormat));
            var model = new InitWagesEblekReleaseViewModel();
            //model.Date = _applicationService.DatePlantWagesSelectList();
            //model.LocationCode = _applicationService.GetLocationCodeSelectList();
            model.LocationCode = _applicationService.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            model.DefaultLocationCode = String.IsNullOrEmpty(param1) ? model.LocationCode.Select(p => p.LocationCode).FirstOrDefault() : param1;
            model.DefaultUnit = String.IsNullOrEmpty(param2) ? _masterDataBll.GetAllUnits(new GetAllUnitsInput
            {
                LocationCode = model.DefaultLocationCode
            }).Select(p => p.UnitCode).Distinct().FirstOrDefault() : param2;
            //model.DefaultUnit = unitCode;
            model.DefaultShift = param3 == null ? _masterDataBll.GetShiftByLocationCode(model.DefaultLocationCode).FirstOrDefault() : param3;
            //model.KpsYear = _applicationService.GetYearSelectList(DateTime.Now.Year);
            //model.DefaultYear = DateTime.Now.Year;
            //model.KpsWeek = _applicationService.GetWeekSelectList(currentWeek.Week);
            //model.DefaultWeek = currentWeek.Week;

            //model.PopupLocationCode = _applicationService.GetLocationCodeSelectList();
            model.PopupLocationCode = _applicationService.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            model.DefaultPopupLocationCode = model.PopupLocationCode.Select(p => p.LocationCode).FirstOrDefault();
            model.DefaultPopupUnit = _masterDataBll.GetAllUnits(new GetAllUnitsInput
            {
                LocationCode = model.DefaultPopupLocationCode
            }).Select(p => p.UnitCode).Distinct().FirstOrDefault();
            model.DefaultPopupShift = _masterDataBll.GetShiftByLocationCode(model.DefaultPopupLocationCode).FirstOrDefault();
            model.LocationFromUrl = param1;
            model.DateFromUrl = String.IsNullOrEmpty(param4) ? "" : param4.Replace("-", "/");
            model.DateNearestClosingPayrollFrom = dateNearestClosingPayroll.FirstOrDefault();
            model.DateNearestClosingPayrollTo = dateNearestClosingPayroll.LastOrDefault();
            return View("Index", model);
        }

        [HttpPost]
        public JsonResult Delete(InsertUpdateData<ExeProductionEntryReleaseModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null || bulkData.New[i].Checkbox == false)
                    {
                        continue;
                    }

                    var eblekRelease = Mapper.Map<ExeProductionEntryReleaseDTO>(bulkData.New[i]);

                    try
                    {
                        _plantWagesExecutionBll.DeleteExeProductionEntryRelease(eblekRelease);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase exception)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = exception.Message;
                    }
                    catch (Exception exception)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = exception.Message;
                    }

                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public JsonResult SaveAllWagesEblekRelease(InsertUpdateData<ExePlantProductionEntryVerificationModel> bulkData)
        {
            var listResultJSon = new List<ExePlantProductionEntryVerificationModel>();
            
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    // check if row null
                    //if ((bulkData.New[i] == null) || (bulkData.New[i].Checkbox == false)) continue;
                    var dbResult = _plantWagesExecutionBll.checkIsLockedState(bulkData.New[i].ProductionEntryCode, bulkData.New[i].Checkbox);

                    if (!dbResult) continue;
                        
                        
                    var eblekRelease = Mapper.Map<ExeProductionEntryReleaseDTO>(bulkData.New[i]);

                    // set created by & updated by
                    eblekRelease.CreatedBy = GetUserName();
                    eblekRelease.UpdatedBy = GetUserName();

                    try
                    {
                        _plantWagesExecutionBll.SaveExeProductionEntryRelease(eblekRelease, bulkData.New[i].Checkbox);

                        
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();

                        listResultJSon.Add(new ExePlantProductionEntryVerificationModel { ProductionEntryCode = bulkData.New[i].ProductionEntryCode });
                        
                    }
                    catch (ExceptionBase exception)
                    { 
                        _vtlogger.Err(exception, new List<object>(), "SaveAllWagesEblekRelease");
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = exception.Message;
                        listResultJSon.Add(new ExePlantProductionEntryVerificationModel { ProductionEntryCode = bulkData.New[i].ProductionEntryCode });
                    }
                    catch (Exception exception)
                    {
                        _vtlogger.Err(exception, new List<object>(), "SaveAllWagesEblekRelease");
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = exception.Message;
                        listResultJSon.Add(new ExePlantProductionEntryVerificationModel { ProductionEntryCode = bulkData.New[i].ProductionEntryCode });
                    }
                }
            }

            return Json(listResultJSon);

        }

        [HttpPost]
        public JsonResult SendApproval(InsertUpdateData<ExeProductionEntryReleaseModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();
            

            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    //var test = bulkData.New[i].Split('/')[1];
                    

                    if(bulkData.New[i] == null || bulkData.New[i].Checkbox == false) continue;

                    var data = bulkData.New[i];
                    var eblekRelease = Mapper.Map<ExeProductionEntryReleaseDTO>(data);

                    try
                    {
                        if (string.IsNullOrEmpty(eblekRelease.Remark))
                        {
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = "Remark cannot null";
                        }
                        else
                        {

                            _plantWagesExecutionBll.UpdateExeProductionEntryRelease(eblekRelease);

                            _generalBll.ExeTransactionLog(new TransactionLogInput
                            {
                                page = Enums.PageName.EblekRelease.ToString(),
                                ActionButton = Enums.ButtonName.SendApproval.ToString(),
                                UserName = GetUserName(),
                                TransactionCode = eblekRelease.ProductionEntryCode,
                                IDRole = CurrentUser.Responsibility.Role
                            });

                            bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();

                            try
                            {
                                var SplittedTransactionCode = eblekRelease.ProductionEntryCode.Split('/');
                                var locationCode = SplittedTransactionCode[1];
                                var unitCode = SplittedTransactionCode[3];
                                var shift = Int32.Parse(SplittedTransactionCode[2]);
                                //var dateString = bulkData.Parameters != null ? bulkData.Parameters["Date"] : "";
                                var productionDate = eblekRelease.ExePlantProductionEntryVerification.ProductionDate;
                                //var productionDate = String.IsNullOrEmpty(dateString) ? DateTime.Now.Date : Convert.ToDateTime(dateString);
                                var paramInput = new GetUserAndEmailInput
                                {
                                    LocationCode = locationCode,
                                    UnitCode = unitCode,
                                    Shift = shift,
                                    Date = productionDate,
                                    ProductionEntryCode = eblekRelease.ProductionEntryCode
                                };

                                _plantWagesExecutionBll.SendEmailWagesEblekRelease(paramInput, GetUserName());
                            }
                            catch (Exception ex)
                            {
                                _vtlogger.Err(ex, new List<object>(), "Failed to send email - SendApproval");
                                resultJSonSendEmail = "Failed to send email.";

                                listResultJSon.Add(resultJSonSubmitData);
                                listResultJSon.Add(resultJSonSendEmail);

                                return Json(listResultJSon);
                            }
                        }
                    }
                    catch (ExceptionBase exception)
                    {
                        _vtlogger.Err(exception, new List<object>(), "SendApproval");
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = exception.Message;
                    }
                    catch (Exception exception)
                    {
                        _vtlogger.Err(exception, new List<object>(), "SendApproval");
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = exception.Message;
                    }

                }
            }

            

            return Json(bulkData);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetExeProductionEntryReleaseInput criteria)
        {

            var eblekReleases = _plantWagesExecutionBll.GetExeProductionEntryReleasesNew(criteria);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.PlantWagesExcelTemplate.PlantWagesExecutionEblekRelease + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            var allLocations = _applicationService.GetLocationCodeCompat();
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
                slDoc.SetCellValue(4, 2, criteria.Unit.ToString());
                slDoc.SetCellValue(5, 2, criteria.Shift.ToString());
                slDoc.SetCellValue(3, 4, criteria.Date.Value.ToShortDateString() +"    to    "+criteria.DateTo.Value.ToShortDateString());
                //slDoc.SetCellValue(3, 6, criteria.DateTo.Value.ToShortDateString());

                //row values
                var iRow = 8;

                foreach (var eblekRelease in eblekReleases)
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
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, eblekRelease.ExePlantProductionEntryVerification.ProductionDate.ToShortDateString());
                    slDoc.SetCellValue(iRow, 2, eblekRelease.ExePlantProductionEntryVerification.BrandCode);
                    slDoc.SetCellValue(iRow, 3, eblekRelease.ExePlantProductionEntryVerification.GroupCode);
                    slDoc.SetCellValue(iRow, 4, eblekRelease.Remark);
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
                slDoc.AutoFitColumn(3, 5);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "PlantWagesExecutionEblekRelease_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName); 
        }

        #region Transaction History and Flow

        /// <summary>
        /// Transaction History
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHistory(TransactionLogInput input)
        {
            var pageResult = new PageResult<TransactionHistoryViewModel>();
            var transactionLogAll = _utilitiesBLL.GetTransactionHistory(input);
            var transactionLog = new List<TransactionHistoryDTO>();
            foreach (TransactionHistoryDTO item in transactionLogAll)
            {
                if (String.Equals(item.action, "SendApproval"))
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

        #endregion
    }
}