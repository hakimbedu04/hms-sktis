using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeAPOpen;
using SpreadsheetLight;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeAPOpenController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBll;

        public TPOFeeAPOpenController(IApplicationService applicationService, IMasterDataBLL masterDataBll, ITPOFeeBLL tpoFeeBll, IGeneralBLL generalBll, IUtilitiesBLL utilitiesBll)
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            _generalBll = generalBll;
            _utilitiesBll = utilitiesBll;
            SetPage("TPOFee/Execution/TPOFeeAP");
        }

        // GET: TPOFeeAPOpen
        public ActionResult Index(string param1, string param2)
        {
            var init = new InitTPOFeeAPOpenViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                Regional = _svc.GetTPOLocationCodeSelectList(),
                BackToList = param1,
                Param2 = param2,
                DisableBtnBack = param1 != null && param2 != null
            };

            TempData["year1"] = TempData["Year"];
            TempData["week1"] = TempData["week"];

            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTPOFeeAPOpen(GetTPOFeeAPOpenInput criteria)
        {
            var result = _tpoFeeBll.GetTpoFeeAPOpen(criteria);
            var viewModel = Mapper.Map<List<TPOFeeAPOpenViewModel>>(result);
            var pageResult = new PageResult<TPOFeeAPOpenViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public JsonResult Complete(InsertUpdateData<TPOFeeAPOpenViewModel> bulkData)
        {
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
                        _generalBll.ExeTransactionLog(new TransactionLogInput()
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
                }
            }
            catch (Exception ex)
            {
                return Json("Failed to run Complete data on background process." + ex.Message);
            }
            return Json("Run Complete data on background process.");
        }

        public JsonResult Revise(InsertUpdateData<TPOFeeAPOpenViewModel> bulkData)
        {
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
                }
            }
            catch (Exception ex)
            {
                return Json("Failed to Revise data on background process." + ex.Message);
            }
            return Json("Run Revise data on background process.");
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetTPOFeeAPOpenInput criteria)
        {
            var tpoFeeApOpenDtos = _tpoFeeBll.GetTpoFeeAPOpen(criteria);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeAPOpen + ".xlsx";
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

                foreach (var tpoApp in tpoFeeApOpenDtos)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    //if (tpoApp.Status == "APPROVED")
                    //{
                    //    style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.DarkSalmon, System.Drawing.Color.DarkSalmon);
                    //}

                   
                    slDoc.SetCellValue(iRow, 1, tpoApp.LocationCode);
                    slDoc.SetCellValue(iRow, 2, tpoApp.LocationName);
                    slDoc.SetCellValue(iRow, 3, tpoApp.SKTBrandCode);
                    slDoc.SetCellValue(iRow, 4, tpoApp.Note);
                    slDoc.SetCellValue(iRow, 5, tpoApp.JKN.ToString());
                    slDoc.SetCellValue(iRow, 6, tpoApp.JL1.ToString());
                    slDoc.SetCellValue(iRow, 7, tpoApp.JL2.ToString());
                    slDoc.SetCellValue(iRow, 8, tpoApp.JL3.ToString());
                    slDoc.SetCellValue(iRow, 9, tpoApp.JL4.ToString());
                    slDoc.SetCellValue(iRow, 10, tpoApp.BiayaProduksi.ToString());
                    slDoc.SetCellValue(iRow, 11, tpoApp.JasaManajemen.ToString());
                    slDoc.SetCellValue(iRow, 12, tpoApp.ProductivityIncentives.ToString());
                    slDoc.SetCellValue(iRow, 13, tpoApp.JasaManajemen2Percent.ToString());
                    slDoc.SetCellValue(iRow, 14, tpoApp.ProductivityIncentives2Percent.ToString());
                    slDoc.SetCellValue(iRow, 15, tpoApp.BiayaProduksi10Percent.ToString());
                    slDoc.SetCellValue(iRow, 16, tpoApp.JasaMakloon10Percent.ToString());
                    slDoc.SetCellValue(iRow, 17, tpoApp.ProductivityIncentives10Percent.ToString());
                    slDoc.SetCellValue(iRow, 18, tpoApp.TotalBayar.ToString());
                    slDoc.SetCellValue(iRow, 19, tpoApp.TaxtNoProd.ToString());
                    slDoc.SetCellValue(iRow, 20, tpoApp.TaxtNoMgmt.ToString());
                    slDoc.SetCellValue(iRow, 21, tpoApp.Status.ToString());
                    slDoc.SetCellStyle(iRow, 1, iRow, 21, style);
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
                slDoc.AutoFitColumn(19, 22);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFee_TPOFeeApOpen_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}