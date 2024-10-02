using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeReportsPackage;
using HMS.SKTIS.Core;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeReportsPackageController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;

        public TPOFeeReportsPackageController(IApplicationService applicationService, IMasterDataBLL masterDataBll, ITPOFeeBLL tpoFeeBll)
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            SetPage("TPOFee/Report/TPOPackageReport");
        }

        //
        // GET: /TPOFeeReportsPackage/
        public ActionResult Index()
        {
            int yearOld = DateTime.Now.Year;
            if (TempData["year1"] != null)
            {
                yearOld = (int)TempData["year1"];

            }

            var init = new InitTPOFeeReportsPackageViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultYear = yearOld
            };

            return View("Index", init);
        }

        [HttpPost]
        public JsonResult GetTPOReportsPackage(GetTPOReportsPackageInput criteria)
        {
            var result = _tpoFeeBll.GetTpoFeeReportsPackageNew(criteria.Year);
            var viewModel = Mapper.Map<List<TPOFeeReportsPackageViewModel>>(result);
            var pageResult = new PageResult<TPOFeeReportsPackageViewModel>(viewModel, criteria);

            return Json(pageResult);
        }

        public FileStreamResult GenerateExcel(GetTPOReportsPackageInput input)
        {

            var TPOReportsPackage = _tpoFeeBll.GetTpoFeeReportsPackageNew(input.Year);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.TpoFeeExcelTemplate.TPOFeeReportsPackage + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);

            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SpreadsheetLight.SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 2, ": " + input.Year);

                // row values
                var iRow = 10;
                
                foreach (var data in TPOReportsPackage)
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

                    slDoc.SetCellValue(iRow, 1, data.LocationCode);
                    slDoc.SetCellValue(iRow, 2, data.LocationName);
                    slDoc.SetCellValue(iRow, 3, data.BrandCode);

                    var count = 4;
                    foreach (TPOFeeReportsPackageWeeklyDTO x in data.ListWeekValue)
                    {
                        slDoc.SetCellValue(iRow, count, x.PackageValue);
                        count++;
                    }

                    slDoc.SetCellValue(iRow, 56, data.MemoReff);

                    slDoc.SetCellStyle(iRow, 1, iRow, 56, style);
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
                slDoc.AutoFitColumn(1, 56);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);

            }

            //// this is important. Otherwise you get an empty file
            //// (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFeeReportsPackage_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
	}
}