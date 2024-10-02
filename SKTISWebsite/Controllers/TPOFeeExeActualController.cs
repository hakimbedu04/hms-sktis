using System;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeExeActual;
using SpreadsheetLight;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeExeActualController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;
        private IUtilitiesBLL _utilitiesBll;

        public TPOFeeExeActualController(
            IApplicationService applicationService,
            IMasterDataBLL masterDataBll,
            ITPOFeeBLL tpoFeeBll,
            IUtilitiesBLL utilitiesBll
        )
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            _utilitiesBll = utilitiesBll;
            SetPage("TPOFee/Execution/TPOFeeActual");
        }

        // GET: TPOFeeExeActual
        public ActionResult Index(string param1, int? param2, int? param3, int? param4)
        {
            if (param4.HasValue) setResponsibility(param4.Value);
            int yearOld = DateTime.Now.Year;
            if (TempData["year1"] != null)
            {
                yearOld = (int)TempData["year1"];

            }
            int? weekOld = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            if (TempData["week1"] != null)
            {
                weekOld = (int)TempData["week1"];

            }

            var init = new InitTPOFeeExeActualViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultYear = yearOld,
                DefaultWeek = (weekOld.Value -1),
                Regional = _svc.GetTPOLocationCodeSelectListCompat(),
                Param2Year = param2,
                Param3Week = param3
            };

            
            if (param1 != null)
            {
                var location = _masterDataBll.GetLocation(param1);
                init.Param1LocationCode = location.ParentLocationCode;
            } 
            

            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTPOFeeActual(GetTPOFeeExeActualInput criteria)
        {
            criteria.DestinationRole = CurrentUser.Responsibility.IDResponsibility;

            var result = _tpoFeeBll.GetTpoFeeExeActuals(criteria);

            //foreach (TPOFeeExeActualViewDTO fee in result)
            //{
            //    var translog = _utilitiesBll.GetTransactionLogsByTransactionCode(fee.TPOFeeCode);
            //    var idFlow = translog.Max(m => m.IDFlow).ToString();

            //    if (idFlow != null)
            //    {
            //        var role = _utilitiesBll.GetRoleByIDFlow(Int32.Parse(idFlow));

            //        fee.PIC = role != null ? role.RolesName : "";
            //    }
            //}

            foreach (TPOFeeExeActualViewDTO fee in result)
            {
                fee.PIC = _utilitiesBll.GetRoleByRoleCode(fee.PIC).RolesName;
            }

            var viewModel = Mapper.Map<List<TPOFeeExeActualViewModel>>(result);
            var pageResult = new PageResult<TPOFeeExeActualViewModel>(viewModel, criteria);

            TempData["Year"] = criteria.Year;
            TempData["week"] = criteria.Week;
            
            

            return Json(pageResult);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetTPOFeeExeActualInput criteria)
        {
            //var tpoFeeExeActuals = _tpoFeeBll.GetTpoFeeExeActuals(criteria);
            var tpoFeeExeActuals = _tpoFeeBll.GetTpoFeeExeActuals(criteria);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeExeActual + ".xlsx";
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
                slDoc.SetCellValue(3, 5, criteria.Year);
                slDoc.SetCellValue(4, 5, criteria.Week);

                //row values
                var iRow = 7;

                foreach (var tpoFeeExeActualViewDto in tpoFeeExeActuals)
                {

                    foreach (TPOFeeExeActualViewDTO fee in tpoFeeExeActuals)
                    {
                        var translog = _utilitiesBll.GetTransactionLogsByTransactionCode(fee.TPOFeeCode);
                        var idFlow = translog.Max(m => m.IDFlow).ToString();

                        if (idFlow != null)
                        {
                            var role = _utilitiesBll.GetRoleByIDFlow(Int32.Parse(idFlow));

                            tpoFeeExeActualViewDto.PIC = role != null ? role.RolesName : "";

                            break;
                        }
                    }

                    var viewModel = Mapper.Map<List<TPOFeeExeActualViewModel>>(tpoFeeExeActuals);

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

                    
                    slDoc.SetCellValue(iRow, 1, tpoFeeExeActualViewDto.LocationCode);
                    slDoc.SetCellValue(iRow, 2, tpoFeeExeActualViewDto.LocationName);
                    slDoc.SetCellValue(iRow, 3, tpoFeeExeActualViewDto.SKTBrandCode);
                    slDoc.SetCellValue(iRow, 4, tpoFeeExeActualViewDto.Status);
                    slDoc.SetCellValue(iRow, 5, tpoFeeExeActualViewDto.PIC);
                    slDoc.SetCellStyle(iRow, 1, iRow, 5, style);
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
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFee_TPOFeeActual_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}