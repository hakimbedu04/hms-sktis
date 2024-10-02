using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeApproval;
using HMS.SKTIS.Core;
using SpreadsheetLight;
using SKTISWebsite.Models.TPOFeeExeActual;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeApprovalController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBll;

        public TPOFeeApprovalController(IApplicationService applicationService, IMasterDataBLL masterDataBll,
            ITPOFeeBLL tpoFeeBll, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBll)
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            _generalBll = generalBll;
            _utilitiesBll = utilitiesBll;
            SetPage("TPOFee/Execution/ApprovalPage");
        }

        // GET: TPOFeeApproval
        public ActionResult Index(string param1, string param2)
        {
            var init = new InitTPOFeeApprovalViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                Regional = _svc.GetTPOLocationCodeSelectListCompat(),
                BackToList = param1,
                Param2 = param2,
                DisableBtnBack = param1 != null && param2 != null
            };

            TempData["year1"] = TempData["Year"];
            TempData["week1"] = TempData["week"];

            return View("Index", init);
        }

        public ActionResult SubmitedDatas(string tpoFeeCode, string regional)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                //Generate Data for transaction log
                _generalBll.ExeTransactionLog(new TransactionLogInput()
                {
                    page = Enums.PageName.TPOFeeActualDetail.ToString(),
                    ActionButton = Enums.ButtonName.Submit.ToString(),
                    UserName = GetUserName(),
                    TransactionCode = tpoFeeCode,
                    IDRole = CurrentUser.Responsibility.Role
                });

                resultJSonSubmitData = "Run submit data on background process.";
            }
            catch (Exception)
            {
                resultJSonSubmitData = "Failed to run submit data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }

            try
            {
                _tpoFeeBll.SendEmailSubmitTPOFeeActual(tpoFeeCode, regional, GetUserName());
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSubmitData);
                listResultJSon.Add(resultJSonSendEmail);

                return Json(listResultJSon);
            }

            listResultJSon.Add(resultJSonSubmitData);
            return Json(listResultJSon);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTPOFeeApproval(GetTPOFeeExeActualInput criteria)
        {
            var result = _tpoFeeBll.GetTpoFeeApprovals(criteria);
            var viewModel = Mapper.Map<List<TPOFeeApprovalViewModel>>(result);
            var pageResult = new PageResult<TPOFeeApprovalViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public JsonResult Approval(InsertUpdateData<TPOFeeApprovalViewModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                // save data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        if (!bulkData.Edit[i].Check) continue;
                        InsertIntoTransactionLog(new TransactionLogInput() {
                            page = Enums.PageName.ApprovalPage.ToString(),
                            ActionButton = Enums.ButtonName.Approve.ToString(),
                            UserName = GetUserName(),
                            TransactionCode = bulkData.Edit[i].TPOFeeCode,
                            ActionTime = DateTime.Now.AddSeconds(i),
                            TransactionDate = DateTime.Now.AddSeconds(i),
                            IDRole = CurrentUser.Responsibility.Role
                        });
                    }

                    resultJSonSubmitData = "Run Approval data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                }
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = "Failed to run Approval data on background process." + ex.Message;
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }

            try
            {
                if (bulkData.Edit != null)
                {
                    List<string> listTPOFeeCode = new List<string>();
                    var regional = string.Empty;
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        if (!bulkData.Edit[i].Check) continue;
                        listTPOFeeCode.Add(bulkData.Edit[i].TPOFeeCode);
                        regional = bulkData.Edit[i].ParentLocationCode;
                    }
                    _tpoFeeBll.SendEmailTPOApprovalPage(listTPOFeeCode, regional, GetUserName(), Enums.ButtonName.Approve.ToString());
                }
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSendEmail);
            }

            return Json(listResultJSon);
        }

        [HttpPost]
        public JsonResult Authorize(InsertUpdateData<TPOFeeApprovalViewModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                // save data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        if (!bulkData.Edit[i].Check) continue;
                        //if (!_utilitiesBll.CheckDataAlreadyOnTransactionLog(bulkData.Edit[i].TPOFeeCode, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Authorize.ToString()))
                            //InsertIntoTransactionLog(new TransactionLogInput()
                            //{
                            //    page = Enums.PageName.ApprovalPage.ToString(),
                            //    ActionButton = Enums.ButtonName.Authorize.ToString(),
                            //    UserName = GetUserName(),
                            //    TransactionCode = bulkData.Edit[i].TPOFeeCode,
                            //    ActionTime = DateTime.Now.AddSeconds(i),
                            //    TransactionDate = DateTime.Now.AddSeconds(i),
                            //    IDRole = CurrentUser.Responsibility.Role
                            //});
                    }

                    resultJSonSubmitData = "Run Authorize data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                }
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = "Failed to run Authorize data on background process." + ex.Message;
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }

            try
            {
                List<string> listTPOFeeCode = new List<string>();
                var regional = string.Empty;
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    if (!bulkData.Edit[i].Check) continue;
                    if (!_utilitiesBll.CheckDataAlreadyOnTransactionLog(bulkData.Edit[i].TPOFeeCode, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Authorize.ToString())) {
                        listTPOFeeCode.Add(bulkData.Edit[i].TPOFeeCode);

                        _tpoFeeBll.AuthorizeActual(bulkData.Edit[i].TPOFeeCode, GetUserName());

                        InsertIntoTransactionLog(new TransactionLogInput() {
                            page = Enums.PageName.ApprovalPage.ToString(),
                            ActionButton = Enums.ButtonName.Authorize.ToString(),
                            UserName = GetUserName(),
                            TransactionCode = bulkData.Edit[i].TPOFeeCode,
                            ActionTime = DateTime.Now.AddSeconds(i),
                            TransactionDate = DateTime.Now.AddSeconds(i),
                            IDRole = CurrentUser.Responsibility.Role
                        });
                    }
                        
                    regional = bulkData.Edit[i].ParentLocationCode;
                }
                _tpoFeeBll.SendEmailTPOApprovalPage(listTPOFeeCode, regional, GetUserName(), Enums.ButtonName.Authorize.ToString());
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSendEmail);
            }

            return Json(listResultJSon);
        }

        public JsonResult Revise(InsertUpdateData<TPOFeeApprovalViewModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                // save data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        var lastTransactionFlow = _utilitiesBll.GetLatestActionTransLogWithoutPage(bulkData.Edit[i].TPOFeeCode);
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        if (!bulkData.Edit[i].Check) continue;

                        if (lastTransactionFlow.IDFlow > 43)
                        {
                            //Revise from ACCT
                            _generalBll.ExeTransactionLog(new TransactionLogInput()
                            {
                                page = Enums.PageName.TPOFeeAP.ToString(),
                                ActionButton = Enums.ButtonName.Revise.ToString(),
                                UserName = GetUserName(),
                                TransactionCode = bulkData.Edit[i].TPOFeeCode,
                                ActionTime = DateTime.Now.AddSeconds(i),
                                TransactionDate = DateTime.Now.AddSeconds(i),
                                IDRole = CurrentUser.Responsibility.Role
                            });
                        }
                        else
                        {
                            //Revise from SKTHD
                            _generalBll.ExeTransactionLog(new TransactionLogInput()
                            {
                                page = Enums.PageName.ApprovalPage.ToString(),
                                ActionButton = Enums.ButtonName.Revise.ToString(),
                                UserName = GetUserName(),
                                TransactionCode = bulkData.Edit[i].TPOFeeCode,
                                ActionTime = DateTime.Now.AddSeconds(i),
                                TransactionDate = DateTime.Now.AddSeconds(i),
                                IDRole = CurrentUser.Responsibility.Role
                            });
                        }

                    }

                    resultJSonSubmitData = "Run Revise data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                }
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = "Failed to run Revise data on background process." + ex.Message;
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }

            try
            {
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        if (!bulkData.Edit[i].Check) continue;
                        _tpoFeeBll.SendEmailReturnTPOFeeActual(bulkData.Edit[i].TPOFeeCode, bulkData.Edit[i].Regional, GetUserName());
                    }
                }
                
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSendEmail);
            }

            return Json(listResultJSon);
        }

        public JsonResult Complete(InsertUpdateData<TPOFeeApprovalViewModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                // save data
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        if (!bulkData.Edit[i].Check) continue;
                        InsertIntoTransactionLog(new TransactionLogInput()
                        {
                            page = Enums.PageName.TPOFeeAP.ToString(),
                            ActionButton = Enums.ButtonName.Complete.ToString(),
                            UserName = GetUserName(),
                            TransactionCode = bulkData.Edit[i].TPOFeeCode,
                            ActionTime = DateTime.Now.AddSeconds(i),
                            TransactionDate = DateTime.Now.AddSeconds(i),
                            IDRole = CurrentUser.Responsibility.Role
                        });
                    }

                    resultJSonSubmitData = "Run Complete data on background process.";
                    listResultJSon.Add(resultJSonSubmitData);
                }
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = "Failed to run Complete data on background process." + ex.Message;
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }

            try
            {
                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        if (!bulkData.Edit[i].Check) continue;
                        _tpoFeeBll.SendEmailReturnTPOFeeActual(bulkData.Edit[i].TPOFeeCode, bulkData.Edit[i].Regional, GetUserName());
                    }
                }

            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSendEmail);
            }

            return Json(listResultJSon);
        }

        public void InsertIntoTransactionLog(TransactionLogInput input)
        {
            _generalBll.ExeTransactionLog(input);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetTPOFeeExeActualInput criteria)
        {
            var tpoFeeExeActuals = _tpoFeeBll.GetTpoFeeApprovals(criteria);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeExeApproval + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            var allLocations = _svc.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == criteria.Regional)
                {
                    locationCompat = item.Text;
                }
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 2, locationCompat);
                slDoc.SetCellValue(3, 16, criteria.Year);
                slDoc.SetCellValue(4, 16, criteria.Week);

                //row values
                var iRow = 8;

                foreach (var tpoApp in tpoFeeExeActuals)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (tpoApp.Status == "APPROVED")
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.DarkSalmon, System.Drawing.Color.DarkSalmon);
                    }

                    
                    slDoc.SetCellValue(iRow, 1, tpoApp.LocationCode);
                    slDoc.SetCellValue(iRow, 2, tpoApp.LocationName);
                    slDoc.SetCellValue(iRow, 3, tpoApp.SKTBrandCode);
                    slDoc.SetCellValue(iRow, 4, tpoApp.Note);
                    slDoc.SetCellValue(iRow, 5, tpoApp.JKN.ToString());
                    slDoc.SetCellValue(iRow, 6, tpoApp.JL1.ToString());
                    slDoc.SetCellValue(iRow, 7, tpoApp.JL2.ToString());
                    slDoc.SetCellValue(iRow, 8, tpoApp.BiayaProduksi.ToString());
                    slDoc.SetCellValue(iRow, 9, tpoApp.JasaManajemen.ToString());
                    slDoc.SetCellValue(iRow, 10, tpoApp.ProductivityIncentives.ToString());
                    slDoc.SetCellValue(iRow, 11, tpoApp.JasaManajemen2Percent.ToString());
                    slDoc.SetCellValue(iRow, 12, tpoApp.ProductivityIncentives2Percent.ToString());
                    slDoc.SetCellValue(iRow, 13, tpoApp.BiayaProduksi10Percent.ToString());
                    slDoc.SetCellValue(iRow, 14, tpoApp.JasaMakloon10Percent.ToString());
                    slDoc.SetCellValue(iRow, 15, tpoApp.ProductivityIncentives10Percent.ToString());
                    slDoc.SetCellValue(iRow, 16, tpoApp.TotalBayar.ToString());
                    slDoc.SetCellStyle(iRow, 1, iRow, 16, style);
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
                //slDoc.AutoFitColumn(1, 6);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFee_TPOFeeApproval_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}