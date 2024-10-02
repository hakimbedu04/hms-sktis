using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.TPOFeeExeActualDetail;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Models.TPOFeeApproval;
using System.Globalization;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.UtilTransactionLog;
using AutoMapper;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeExeActualDetailController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBll;


        public TPOFeeExeActualDetailController(IApplicationService applicationService, IMasterDataBLL masterDataBll, ITPOFeeBLL tpoFeeBll, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBll)
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            _generalBll = generalBll;
            _utilitiesBll = utilitiesBll;
            SetPage("TPOFee/Execution/TPOFeeActualDetail");
        }
        // GET: TPOFeeExeActualDetail
        public ActionResult Index(string param1, string param2, string param3, int? param4, string param5)
        {
            if (param4.HasValue) setResponsibility(param4.Value);
            var ci = CultureInfo.CurrentCulture;
            var tpofeeactual = _tpoFeeBll.GetTPOFeeActualByProductionCode(param1.Replace("_", "/"));
            var tpoFeeHdrTpo = _tpoFeeBll.GetTpoFeeHdrById(param1.Replace("_", "/"));
            var locationTpo = _masterDataBll.GetLocation(tpoFeeHdrTpo.LocationCode);
            var locationDto = _masterDataBll.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput() { ParentLocationCode = tpoFeeHdrTpo.LocationCode }).SingleOrDefault();
            var regional = _masterDataBll.GetLocation(locationDto.ParentLocationCode);
            var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrTpo.BrandGroupCode);
            var daily = _tpoFeeBll.GetTpoFeeProductionDailys(param1.Replace("_", "/"));
            

            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(tpoFeeHdrTpo.KPSYear, tpoFeeHdrTpo.KPSWeek);
            var modelHoliday = _masterDataBll.GetHolidayByWeek(mstGenWeek, locationTpo.LocationCode.ToString());

            var mstTpoPackage = _masterDataBll.GetTpoPackage(new GetMstTPOPackagesInput
            {
                LocationCode = tpoFeeHdrTpo.LocationCode,
                BrandGroupCode = tpoFeeHdrTpo.BrandGroupCode,
                StartDate = mstGenWeek.StartDate,
                EndDate = mstGenWeek.EndDate
            });

            var init = new InitTPOFeeExeActualDetailViewModel()
            {
                TPOFeeCode = param1.Replace("_", "/"),
                LocationCode = locationTpo.LocationCode,
                LocationName = locationTpo.LocationName,
                CostCenter = locationTpo.CostCenter,
                KPSWeek = tpoFeeHdrTpo.KPSWeek,
                KPSYear = tpoFeeHdrTpo.KPSYear,
                Regional = regional.LocationCode,
                RegionalName = regional.LocationName,
                Brand = mstGenBrandGroup.SKTBrandCode,
                StickPerBox = mstGenBrandGroup.StickPerBox,
                Paket = mstTpoPackage == null ? null : mstTpoPackage.Package,
                Daily = daily,
                TotalProdStick = tpoFeeHdrTpo.TotalProdStick,
                TotalProdBox = daily.Sum(m => m.OutputBox),//tpoFeeHdrTpo.TotalProdBox,
                TotalProdJKN = daily.Sum(m => m.JKN),//tpoFeeHdrTpo.TotalProdJKN ?? 0,
                TotalProdJl1 = daily.Sum(m => m.JL1),//tpoFeeHdrTpo.TotalProdJl1 ?? 0,
                TotalProdJl2 = daily.Sum(m => m.Jl2),//tpoFeeHdrTpo.TotalProdJl2 ?? 0,
                TotalProdJl3 = daily.Sum(m => m.Jl3),//tpoFeeHdrTpo.TotalProdJl3 ?? 0,
                TotalProdJl4 = daily.Sum(m => m.Jl4),//tpoFeeHdrTpo.TotalProdJl4 ?? 0,
                //TotalDibayarJKN = (tpoFeeHdrTpo.TotalProdJKN + tpoFeeHdrTpo.TotalProdJl1 + tpoFeeHdrTpo.TotalProdJl2 + tpoFeeHdrTpo.TotalProdJl3 + tpoFeeHdrTpo.TotalProdJl4) ?? 0,
                TotalDibayarJKN = tpoFeeHdrTpo.TotalProdJKN,
                TotalDibayarJL1 = tpoFeeHdrTpo.TotalProdJl1,
                TotalDibayarJL2 = tpoFeeHdrTpo.TotalProdJl2,
                TotalDibayarJL3 = tpoFeeHdrTpo.TotalProdJl3,
                TotalDibayarJL4 = tpoFeeHdrTpo.TotalProdJl4,
                Calculations = _tpoFeeBll.GetTpoFeeCalculation(param1.Replace("_", "/")),
                PengirimanL1 = tpoFeeHdrTpo.PengirimanL1,
                PengirimanL2 = tpoFeeHdrTpo.PengirimanL2,
                PengirimanL3 = tpoFeeHdrTpo.PengirimanL3,
                PengirimanL4 = tpoFeeHdrTpo.PengirimanL4,
                TaxtNoMgmt = tpoFeeHdrTpo.TaxtNoMgmt ?? "" ,
                TaxtNoProd = tpoFeeHdrTpo.TaxtNoProd ?? "",
                //PreparedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SLT.ToString(), regional.LocationCode),
                //ApprovedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.RM.ToString(), regional.LocationCode),
                //AuthorizedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SKTHD.ToString(), regional.LocationCode),
                PreparedBy = _utilitiesBll.GetUserOnTransactionLog(param1.Replace("_", "/"), Enums.PageName.TPOFeeActualDetail.ToString(), Enums.ButtonName.Submit.ToString()),
                ApprovedBy = _utilitiesBll.GetUserOnTransactionLog(param1.Replace("_", "/"), Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Approve.ToString()),
                AuthorizedBy = _utilitiesBll.GetUserOnTransactionLog(param1.Replace("_", "/"), Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Authorize.ToString()),
                DisableBtnSave = _tpoFeeBll.CheckBtnSave(param1.Replace("_", "/")),
                PageFrom = param2,
                Status = tpofeeactual.Status,
                RoleSave = _utilitiesBll.RoleButtonChecker(param1.Replace("_", "/"), CurrentUser.Responsibility.Role, Enums.PageName.TPOFeeActualDetail.ToString(), Enums.ButtonName.Save.ToString()),
                RoleSubmit = _utilitiesBll.RoleButtonChecker(param1.Replace("_", "/"), CurrentUser.Responsibility.Role, Enums.PageName.TPOFeeActualDetail.ToString(), Enums.ButtonName.Submit.ToString()),
                RoleApprove = _utilitiesBll.RoleButtonChecker(param1.Replace("_", "/"), CurrentUser.Responsibility.Role, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Approve.ToString()),
                RoleAuthorize = _utilitiesBll.RoleButtonChecker(param1.Replace("_", "/"), CurrentUser.Responsibility.Role, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Authorize.ToString()),
                RoleRevise = _utilitiesBll.RoleButtonChecker(param1.Replace("_", "/"), CurrentUser.Responsibility.Role, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Revise.ToString()),
                RoleComplete = _utilitiesBll.RoleButtonChecker(param1.Replace("_", "/"), CurrentUser.Responsibility.Role, Enums.PageName.TPOFeeAP.ToString(), Enums.ButtonName.Complete.ToString()),
                RoleReturn = _utilitiesBll.RoleButtonChecker(param1.Replace("_", "/"), CurrentUser.Responsibility.Role, Enums.PageName.TPOFeeAP.ToString(), Enums.ButtonName.Return.ToString()),
                
                HolidayDay = modelHoliday
            };

            //Back To last list filter
            TempData["year1"] = TempData["Year"];
            TempData["week1"] = TempData["week"];

            return View("Index", init);
        }

        public ActionResult SaveDatas(string tpoFeeCode, string taxtNoProd, string taxtNoMgmt)
        {
            try
            {
                taxtNoProd = taxtNoProd.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                taxtNoMgmt = taxtNoMgmt.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                _tpoFeeBll.SaveTpoFeeHdr(tpoFeeCode, taxtNoProd, taxtNoMgmt);
                //Generate Data for transaction log
                _generalBll.ExeTransactionLog(new TransactionLogInput()
                {
                    page = Enums.PageName.TPOFeeActualDetail.ToString(),
                    ActionButton = Enums.ButtonName.Save.ToString(),
                    UserName = GetUserName(),
                    TransactionCode = tpoFeeCode,
                    IDRole = CurrentUser.Responsibility.Role
                });
            }
            catch (Exception)
            {
                return Json("Failed to run save data");
            }
            return Json("Save data success.");
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

        public ActionResult CompleteDatas(string tpoFeeCode)
        {
            try
            {
                //Generate Data for transaction log
                _generalBll.ExeTransactionLog(new TransactionLogInput()
                {
                    page = Enums.PageName.TPOFeeAP.ToString(),
                    ActionButton = Enums.ButtonName.Complete.ToString(),
                    UserName = GetUserName(),
                    TransactionCode = tpoFeeCode,
                    IDRole = CurrentUser.Responsibility.Role
                });
            }
            catch (Exception)
            {
                return Json("Failed to run complete data");
            }
            return Json("Complete data success.");
        }

        public JsonResult Approval(TPOFeeApprovalViewModel data)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                //if (!_utilitiesBll.CheckDataAlreadyOnTransactionLog(data.TPOFeeCode, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Approve.ToString()))
                //{
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.ApprovalPage.ToString(),
                        ActionButton = Enums.ButtonName.Approve.ToString(),
                        UserName = GetUserName(),
                        TransactionCode = data.TPOFeeCode,
                        ActionTime = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                //}
                

                resultJSonSubmitData = "Run Approval data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
            }
            catch (Exception e)
            {
                resultJSonSubmitData = "Failed to run Approval data on background process." + e.Message;
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }

            try
            {
                _tpoFeeBll.SendEmailApprovalTPOFeeActual(data.TPOFeeCode, data.Regional, GetUserName());
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSendEmail);
            }

            return Json(listResultJSon);
        }

        public JsonResult sendNotif(TPOFeeApprovalViewModel data)
        {
            
            var listResultJSon = new List<string>();
            var resultJSonSendEmail = "";

            var lastTransactionFlow = _utilitiesBll.GetLatestActionTransLogWithoutPage(data.TPOFeeCode);
            switch (lastTransactionFlow.IDFlow)
            {
                case 42: //submitted
                    try
                    {
                        _tpoFeeBll.SendEmailSubmitTPOFeeActual(data.TPOFeeCode, data.Regional, GetUserName());
                    }
                    catch (Exception ex)
                    {
                        resultJSonSendEmail = "Failed to send email.";

                        listResultJSon.Add(resultJSonSendEmail);

                        return Json(listResultJSon);
                    }
                    break;
                case 43: //revised SKTHD
                    try
                    {
                        _tpoFeeBll.SendEmailReturnTPOFeeActual(data.TPOFeeCode, data.Regional, GetUserName());
                    }
                    catch (Exception ex)
                    {
                        resultJSonSendEmail = "Failed to send email.";

                        listResultJSon.Add(resultJSonSendEmail);

                        return Json(listResultJSon);
                    }
                    break;
                case 44: //Approved
                    try
                    {
                        _tpoFeeBll.SendEmailApprovalTPOFeeActual(data.TPOFeeCode, data.Regional, GetUserName());
                    }
                    catch (Exception ex)
                    {
                        resultJSonSendEmail = "Failed to send email.";

                        listResultJSon.Add(resultJSonSendEmail);

                        return Json(listResultJSon);
                    }
                    break;

                case 45: //authorized
                    try
                    {
                        _tpoFeeBll.SendEmailAuthorizeTPOFeeActual(data.TPOFeeCode, data.Regional, GetUserName());
                    }
                    catch (Exception ex)
                    {
                        resultJSonSendEmail = "Failed to send email.";

                        listResultJSon.Add(resultJSonSendEmail);

                        return Json(listResultJSon);
                    }
                    break;
                    
                case 47: //revise ACCT
                    try
                    {
                        _tpoFeeBll.SendEmailReturnTPOFeeActual(data.TPOFeeCode, data.Regional, GetUserName());
                    }
                    catch (Exception ex)
                    {
                        resultJSonSendEmail = "Failed to send email.";

                        listResultJSon.Add(resultJSonSendEmail);

                        return Json(listResultJSon);
                    }
                    break;

            }
            resultJSonSendEmail = "Send Notification";
            listResultJSon.Add(resultJSonSendEmail);
            return Json(listResultJSon);
        }

        public JsonResult Authorize(TPOFeeApprovalViewModel data)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                _tpoFeeBll.AuthorizeActual(data.TPOFeeCode, GetUserName());

                //if (!_utilitiesBll.CheckDataAlreadyOnTransactionLog(data.TPOFeeCode, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Authorize.ToString()))
                //{
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.ApprovalPage.ToString(),
                        ActionButton = Enums.ButtonName.Authorize.ToString(),
                        UserName = GetUserName(),
                        TransactionCode = data.TPOFeeCode,
                        ActionTime = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                //}

                resultJSonSubmitData = "Run Authorize data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
            }
            catch (Exception e)
            {
                resultJSonSubmitData = "Failed to run Authorize data on background process." + e.Message;
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }

            try
            {
                _tpoFeeBll.SendEmailAuthorizeTPOFeeActual(data.TPOFeeCode, data.Regional, GetUserName());
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSendEmail);
            }

            return Json(listResultJSon);
        }

        public JsonResult Revise(TPOFeeApprovalViewModel data)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                //if (!_utilitiesBll.CheckDataAlreadyOnTransactionLog(data.TPOFeeCode, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Revise.ToString()))
                //{
                var lastTransactionFlow = _utilitiesBll.GetLatestActionTransLogWithoutPage(data.TPOFeeCode);

                if (lastTransactionFlow.IDFlow > 43)
                {
                    //Revise from ACCT
                    _generalBll.ExeTransactionLog(new TransactionLogInput()
                    {
                        page = Enums.PageName.TPOFeeAP.ToString(),
                        ActionButton = Enums.ButtonName.Revise.ToString(),
                        UserName = GetUserName(),
                        TransactionCode = data.TPOFeeCode,
                        ActionTime = DateTime.Now,
                        TransactionDate = DateTime.Now,
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
                        TransactionCode = data.TPOFeeCode,
                        ActionTime = DateTime.Now,
                        TransactionDate = DateTime.Now,
                        IDRole = CurrentUser.Responsibility.Role
                    });
                }
                    
                    
                //}

                resultJSonSubmitData = "Run Revise data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = "Failed to run Revise data on background process." + ex.Message;
                listResultJSon.Add(resultJSonSubmitData);
                return Json(listResultJSon);
            }

            try
            {
                _tpoFeeBll.SendEmailReturnTPOFeeActual(data.TPOFeeCode, data.Regional, GetUserName());
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSendEmail);
            }

            return Json(listResultJSon);
        }

        public JsonResult CheckButtonDisable(string tpoFeeCode, string pageName, string btnName)
        {
            return Json(_utilitiesBll.CheckDataAlreadyOnTransactionLog(tpoFeeCode, pageName, btnName));
        }

        public JsonResult GetStatus(string tpoFeeCode)
        {
            var tpofeeactual = _tpoFeeBll.GetTPOFeeActualByProductionCode(tpoFeeCode);
            return Json(tpofeeactual.Status);
        }


        /// <summary>
        /// Transaction History
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHistory(TransactionLogInput input)
        {
            var pageResult = new PageResult<TransactionHistoryViewModel>();
            //var transactionLog = _utilitiesBll.GetTransactionHistoryByPage(input, Enums.PageName.TPOFeeActualDetail.ToString());
            var transactionLog = _utilitiesBll.GetTransactionHistoryByPage(input, null);
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
            var transactionFlow = _utilitiesBll.GetTransactionFlow(input);
            pageResult.TotalRecords = transactionFlow.Count;
            pageResult.TotalPages = (transactionFlow.Count / input.PageSize) + (transactionFlow.Count % input.PageSize != 0 ? 1 : 0);
            var result = transactionFlow.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            pageResult.Results = Mapper.Map<List<TransactionFlowViewModel>>(result);
            return Json(pageResult);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string id, string pageFrom)
        {
            var tpoFeeHdrTpo = _tpoFeeBll.GetTpoFeeHdrById(id.Replace("_", "/"));
            var locationTpo = _masterDataBll.GetLocation(tpoFeeHdrTpo.LocationCode);
            var location = _masterDataBll.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput() { ParentLocationCode = tpoFeeHdrTpo.LocationCode }).SingleOrDefault();
            var regional = _masterDataBll.GetLocation(location.ParentLocationCode);
            var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrTpo.BrandGroupCode);

            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(tpoFeeHdrTpo.KPSYear, tpoFeeHdrTpo.KPSWeek);

            var mstTpoPackage = _masterDataBll.GetTpoPackage(new GetMstTPOPackagesInput
            {
                LocationCode = tpoFeeHdrTpo.LocationCode,
                BrandGroupCode = tpoFeeHdrTpo.BrandGroupCode,
                StartDate = mstGenWeek.StartDate,
                EndDate = mstGenWeek.EndDate
            });

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeExeActualDetail + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                SLStyle styleAlign = slDoc.CreateStyle();
                styleAlign.Alignment.Horizontal = HorizontalAlignmentValues.Right;

                if (pageFrom == "approval")
                {
                    slDoc.SetCellValue(1, 1, "TPO Fee Approval - Detail");
                }
                else if (pageFrom == "open")
                {
                    slDoc.SetCellValue(1, 1, "TPO Fee Open - Detail");
                }
                else if (pageFrom == "list")
                {
                    slDoc.SetCellValue(1, 1, "TPO Fee Actual - Detail");
                }
                else if (pageFrom == "close")
                {
                    slDoc.SetCellValue(1, 1, "TPO Fee CLose - Detail");
                }

                //filter values
                slDoc.SetCellValue(3, 2, regional.LocationCode + " - " + regional.LocationName);
                slDoc.SetCellValue(4, 2, location.LocationCode + " - " + location.LocationName);
                slDoc.SetCellValue(5, 2, locationTpo.CostCenter);
                slDoc.SetCellValue(7, 2, mstGenBrandGroup.SKTBrandCode);
                slDoc.SetCellValue(3, 7, tpoFeeHdrTpo.KPSYear);
                slDoc.SetCellValue(4, 7, tpoFeeHdrTpo.KPSWeek);
                slDoc.SetCellValue(7, 5, mstGenBrandGroup.StickPerBox.ToString());
                slDoc.SetCellValue(7, 8, mstTpoPackage == null ? null : mstTpoPackage.Package.ToString());
                //row values
                var iRow = 11;
                var daily = _tpoFeeBll.GetTpoFeeProductionDailys(id.Replace("_", "/"));
                foreach (var tpoFeeProductionDailyDto in daily)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    //if (iRow % 2 == 0)
                    //{
                    //    style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    //}

                    slDoc.SetCellValue(iRow, 1, tpoFeeProductionDailyDto.Hari);
                    slDoc.SetCellValue(iRow, 2, tpoFeeProductionDailyDto.FeeDate);
                    //slDoc.SetCellValue(iRow, 3, tpoFeeProductionDailyDto.OuputSticks.ToString());
                    slDoc.SetCellValue(iRow, 3, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeProductionDailyDto.OuputSticks));
                    //slDoc.SetCellValue(iRow, 4, tpoFeeProductionDailyDto.OutputBox.ToString());
                    slDoc.SetCellValue(iRow, 4, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeProductionDailyDto.OutputBox));
                    slDoc.SetCellValue(iRow, 5, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeProductionDailyDto.JKN));
                    slDoc.SetCellValue(iRow, 6, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeProductionDailyDto.JL1));
                    slDoc.MergeWorksheetCells(iRow, 6, iRow, 7);
                    slDoc.SetCellValue(iRow, 8, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeProductionDailyDto.Jl2));
                    slDoc.MergeWorksheetCells(iRow, 8, iRow, 9);
                    slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
                    slDoc.SetCellStyle(iRow, 3, iRow, 9, styleAlign);
                    iRow++;
                }
                slDoc.SetCellValue(iRow, 1, "Total Produksi");
                slDoc.MergeWorksheetCells(iRow, 1, iRow, 2);
                slDoc.SetCellValue(iRow, 3, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeHdrTpo.TotalProdStick)); //tpoFeeHdrTpo.TotalProdStick.ToString());
                slDoc.SetCellValue(iRow, 4, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeHdrTpo.TotalProdBox)); //tpoFeeHdrTpo.TotalProdBox.ToString());
                slDoc.SetCellValue(iRow, 5, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeHdrTpo.TotalProdJKN));
                slDoc.SetCellValue(iRow, 6, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeHdrTpo.TotalProdJl1));
                slDoc.MergeWorksheetCells(iRow, 6, iRow, 7);
                slDoc.SetCellValue(iRow, 8, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoFeeHdrTpo.TotalProdJl2));
                slDoc.MergeWorksheetCells(iRow, 8, iRow, 9);

                SLStyle style2 = slDoc.CreateStyle();
                style2.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                style2.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                style2.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                style2.Font.FontName = "Calibri";
                style2.Font.FontSize = 10;
                style2.Font.Bold = true;
                style2.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                slDoc.SetCellStyle(iRow, 1, iRow, 9, style2);
                slDoc.SetCellStyle(iRow, 3, iRow, 9, styleAlign);
                iRow++;
                slDoc.SetCellValue(iRow, 1, "Total Dibayar");
                slDoc.MergeWorksheetCells(iRow, 1, iRow, 2);
                //slDoc.SetCellValue(iRow, 5, (tpoFeeHdrTpo.TotalProdJKN + tpoFeeHdrTpo.TotalProdJl1 + tpoFeeHdrTpo.TotalProdJl2 + tpoFeeHdrTpo.TotalProdJl3 + tpoFeeHdrTpo.TotalProdJl4).ToString());
                slDoc.SetCellValue(iRow, 5, tpoFeeHdrTpo.TotalProdJKN.ToString());
                slDoc.SetCellValue(iRow, 6, tpoFeeHdrTpo.TotalProdJl1.ToString());
                slDoc.MergeWorksheetCells(iRow, 6, iRow, 7);
                slDoc.SetCellValue(iRow, 8, tpoFeeHdrTpo.TotalProdJl2.ToString());
                slDoc.MergeWorksheetCells(iRow, 8, iRow, 9);
                slDoc.SetCellStyle(iRow, 1, iRow, 9, style2);
                slDoc.SetCellStyle(iRow, 3, iRow, 9, styleAlign);
                iRow++;

                SLStyle styleHeader = slDoc.CreateStyle();
                styleHeader.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                styleHeader.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                styleHeader.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                styleHeader.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                styleHeader.Font.FontName = "Calibri";
                styleHeader.Font.FontSize = 10;
                styleHeader.Font.Bold = true;
                styleHeader.Alignment.Horizontal = HorizontalAlignmentValues.Center;
                styleHeader.Font.FontColor = Color.White;
                styleHeader.Fill.SetPattern(PatternValues.Solid, Color.DarkBlue, Color.DarkBlue);

                var calculations = _tpoFeeBll.GetTpoFeeCalculation(id.Replace("_", "/"));
                slDoc.SetCellValue(iRow, 1, "Perhitungan Biaya Produksi");
                slDoc.MergeWorksheetCells(iRow, 1, iRow, 3);
                slDoc.SetCellValue(iRow, 4, "Output Produksi yg Dibayar");
                slDoc.MergeWorksheetCells(iRow, 4, iRow, 5);
                slDoc.SetCellValue(iRow, 6, "Biaya Produksi per Box (rupiah)");
                slDoc.MergeWorksheetCells(iRow, 6, iRow, 7);
                slDoc.SetCellValue(iRow, 8, "Total Biaya Produksi per Box (rupiah)");
                slDoc.MergeWorksheetCells(iRow, 8, iRow, 9)
;
                slDoc.SetCellStyle(iRow, 1, iRow, 9, styleHeader);
                iRow++;
                foreach (var tpoCal in calculations)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    slDoc.SetCellValue(iRow, 1, tpoCal.ProductionFeeType);
                    slDoc.MergeWorksheetCells(iRow, 1, iRow, 3);
                    if (iRow < 26)
                    {
                        slDoc.SetCellValue(iRow, 4, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoCal.OutputProduction));
                        slDoc.SetCellValue(iRow, 6, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoCal.OutputBiaya));
                    }
                    slDoc.MergeWorksheetCells(iRow, 4, iRow, 5);
                    slDoc.MergeWorksheetCells(iRow, 6, iRow, 7);
                    slDoc.SetCellValue(iRow, 8, String.Format(CultureInfo.CurrentCulture, "{0:n2}", tpoCal.Calculate));
                    slDoc.MergeWorksheetCells(iRow, 8, iRow, 9);
                    slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
                    slDoc.SetCellStyle(iRow, 4, iRow, 9, styleAlign);
                    iRow++;
                }
                slDoc.SetCellStyle(iRow - 1, 1, iRow - 1, 9, style2);
                iRow++;

                slDoc.SetCellValue(iRow, 1, "Pengiriman Ke");
                slDoc.SetCellValue(iRow, 8, "No Seri Faktur Pajak");
                iRow++;
                slDoc.SetCellValue(iRow, 1, tpoFeeHdrTpo.PengirimanL1);
                slDoc.SetCellValue(iRow, 8, "Biaya Produksi : " + tpoFeeHdrTpo.TaxtNoMgmt);
                iRow++;
                slDoc.SetCellValue(iRow, 1, tpoFeeHdrTpo.PengirimanL2);
                slDoc.SetCellValue(iRow, 8, "Jasa Maklon : " + tpoFeeHdrTpo.TaxtNoProd);
                iRow++;
                slDoc.SetCellValue(iRow, 1, tpoFeeHdrTpo.PengirimanL3);
                iRow++;
                slDoc.SetCellValue(iRow, 1, tpoFeeHdrTpo.PengirimanL4);
                iRow++;
                iRow++;
                slDoc.SetCellValue(iRow, 1, "Prepared By");
                slDoc.SetCellValue(iRow, 5, "Approved By");
                slDoc.SetCellValue(iRow, 8, "Authorized By");
                iRow++;
                //slDoc.SetCellValue(iRow, 1, _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SLT.ToString(), regional.LocationCode));
                //slDoc.SetCellValue(iRow, 5, _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.RM.ToString(), regional.LocationCode));
                //slDoc.SetCellValue(iRow, 8, _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SKTHD.ToString(), regional.LocationCode));

                slDoc.SetCellValue(iRow, 1, _utilitiesBll.GetUserOnTransactionLog(id.Replace("_", "/"), Enums.PageName.TPOFeeActualDetail.ToString(), Enums.ButtonName.Submit.ToString()));
                slDoc.SetCellValue(iRow, 5, _utilitiesBll.GetUserOnTransactionLog(id.Replace("_", "/"), Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Approve.ToString()));
                slDoc.SetCellValue(iRow, 8, _utilitiesBll.GetUserOnTransactionLog(id.Replace("_", "/"), Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Authorize.ToString()));

                iRow++;
                slDoc.SetCellValue(iRow, 1, "Supervisor Logistic TPO (SLT) ");
                slDoc.SetCellValue(iRow, 5, "Regional Manager");
                slDoc.SetCellValue(iRow, 8, "Head of Hand Rolled Mnf ");
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

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            if (pageFrom == "list")
            {
                var fileName = "TPOFee_TPOFeeActualDetail_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else if (pageFrom == "approval")
            {
                var fileName = "TPOFee_TPOFeeApprovalDetail_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else if (pageFrom == "open")
            {
                var fileName = "TPOFee_TPOFeeOpenDetail_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                var fileName = "TPOFee_TPOFeeCloseDetail_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}