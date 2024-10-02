using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExeProductAdjustment;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class ExeProductAdjustmentController : BaseController
    {

        private IExecutionOtherBLL _executionOtherBll;
        private IApplicationService _appService;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionTPOBLL _executionTpobll;
        private IVTLogger _vtLogger;

        public ExeProductAdjustmentController(IExecutionOtherBLL executionOtherBll, IApplicationService appService, IExecutionTPOBLL executionTpobll, IMasterDataBLL masterDataBll, IVTLogger vtlogger)
        {
            _executionOtherBll = executionOtherBll;
            _appService = appService;
            _masterDataBLL = masterDataBll;
            _executionTpobll = executionTpobll;
            _vtLogger = vtlogger;
            SetPage("ProductionExecution/Others/ProductAdjustment");
        }

        [HttpGet]
        public JsonResult GetAdjustmentTypeSelectList(string brandCode)
        {
            //var result = _appService.GetAdjusmnetTypeList(Enums.MasterGeneralList.ProdAdjType.ToString());
            var result = _appService.GetAdjusmnetTypeList(brandCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnitCodeSelectList(string locationCode)
        {
            var result = _appService.GetUnitCodeSelectListByLocationCodeReportByGroup(locationCode).Where(m => m.Value != Enums.UnitCodeDefault.MTNC.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShiftSelectList(string locationCode)
        {
            var result = _appService.GetShiftByLocationCode(locationCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandSelectList(GetExePlantProductionEntryVerificationInput input)
        {
            //var result = _appService.GetBrandCodeUnionEntryVerification(input);
            var result = _appService.GetBrandCodeFromReportByProcess(input);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeekSelectList(int year)
        {
            var result = _masterDataBLL.GetWeekByYear(year);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductionDateSelectList(int year, int week)
        {
            var result = _appService.GetSelectListDateByYearAndWeek(year, week);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            var model = new InitExeProductAdjustment();
            //model.LocationSelectList = _appService.GetPlantAndRegionalLocationLookupList();
            model.LocationSelectList = _appService.GetLastLocationChildList(Enums.LocationCode.SKT.ToString());
            //model.LocationSelectList = model.LocationSelectList.Select(x => x.LocationCode.Contains(Enums.LocationCode.REG.ToString()));
            model.DefaultLocation = model.LocationSelectList == null ? string.Empty : model.LocationSelectList.Skip(0).First().LocationCode;
            model.YearSelectList = _appService.GetYearSelectList(DateTime.Now.Year);
            model.DefaultYear = DateTime.Now.Year;
            model.DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now);

            return View("Index", model);
        }

        public ActionResult GetExeProductAdjustment(ProductAdjustmentInput criteria)
        {
            var pageResult = new PageResult<ExeProductAdjustmentViewModel>();
            var productAdjustments = _executionOtherBll.GetProductAdjustments(criteria);
            pageResult.TotalRecords = productAdjustments.Count;
            pageResult.TotalPages = (productAdjustments.Count / criteria.PageSize) + (productAdjustments.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = productAdjustments.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<ExeProductAdjustmentViewModel>>(result);
            return Json(pageResult);
        }

        [HttpPost]
        public ActionResult SaveProductAdjustment(InsertUpdateData<ExeProductAdjustmentViewModel> bulkData)
        {
            try
            {
                var productionDate = bulkData.Parameters.ContainsKey("ProductionDate")
                    ? bulkData.Parameters["ProductionDate"]
                    : string.Empty;
                var locationCode = bulkData.Parameters.ContainsKey("LocationCode")
                    ? bulkData.Parameters["LocationCode"]
                    : string.Empty;
                var unitCode = bulkData.Parameters.ContainsKey("UnitCode")
                    ? bulkData.Parameters["UnitCode"]
                    : string.Empty;
                var shift = bulkData.Parameters.ContainsKey("Shift") ? bulkData.Parameters["Shift"] : string.Empty;
                var brandCode = bulkData.Parameters.ContainsKey("BrandCode")
                    ? bulkData.Parameters["BrandCode"]
                    : string.Empty;

                bool success = false;

                var IsPanel0CigarettesExist = false;

                if (bulkData.New != null)
                {
                    for (var i = 0; i < bulkData.New.Count; i++)
                    {
                        if (bulkData.New[i] != null) {
                            if (bulkData.New[i].AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig))
                                IsPanel0CigarettesExist = true;
                        }

                        // check if row null
                        if (bulkData.New[i] == null) continue;

                        var productAdjustment = Mapper.Map<ProductAdjustmentDTO>(bulkData.New[i]);
                        productAdjustment.ProductionDate = DateTime.ParseExact(productionDate,
                            Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                        productAdjustment.LocationCode = locationCode;
                        productAdjustment.UnitCode = unitCode;
                        productAdjustment.Shift = int.Parse(shift);
                        productAdjustment.BrandCode = brandCode;
                        productAdjustment.CreatedBy = GetUserName();
                        productAdjustment.UpdatedBy = GetUserName();
                        productAdjustment.CreatedDate = DateTime.Now;
                        productAdjustment.UpdatedDate = DateTime.Now;

                        try
                        {
                            _executionOtherBll.SaveProductAdjustment(productAdjustment);
                            bulkData.New[i] = Mapper.Map<ExeProductAdjustmentViewModel>(productAdjustment);
                            bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtLogger.Err(ex);
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtLogger.Err(ex);
                            bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.New[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        }

                        success = true;

                    }

                }

                if (bulkData.Edit != null)
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        if (bulkData.Edit[i] == null) continue;

                        if (bulkData.Edit[i].AdjustmentType == EnumHelper.GetDescription(Enums.ProdAdjType.PanelCig))
                            IsPanel0CigarettesExist = true;

                        var productAdjustment = Mapper.Map<ProductAdjustmentDTO>(bulkData.Edit[i]);
                        productAdjustment.ProductionDate = DateTime.ParseExact(productionDate,
                            Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                        productAdjustment.LocationCode = locationCode;
                        productAdjustment.UnitCode = unitCode;
                        productAdjustment.Shift = int.Parse(shift);
                        productAdjustment.BrandCode = brandCode;
                        productAdjustment.UpdatedBy = GetUserName();
                        productAdjustment.UpdatedDate = DateTime.Now;

                        try
                        {
                            _executionOtherBll.UpdateProductAdjustment(productAdjustment);
                            bulkData.Edit[i] = Mapper.Map<ExeProductAdjustmentViewModel>(productAdjustment);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtLogger.Err(ex);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtLogger.Err(ex);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        }

                        success = true;
                    }
                }

                var tpoLocation = _appService.GetLastLocationChildList(Enums.LocationCode.TPO.ToString());
                var contain = tpoLocation.Count(m => m.LocationCode.Contains(locationCode));
                if (success && contain > 0)
                {
                    var date = DateTime.ParseExact(productionDate, Constants.DefaultDateOnlyFormat,
                        CultureInfo.InvariantCulture);
                    var yearWeek = _masterDataBLL.GetWeekByDate(date);
                    var input = new HMS.SKTIS.BusinessObjects.Inputs.Execution.GetExeTPOProductionEntryVerificationInput
                        ()
                    {
                        BrandCode = brandCode,
                        LocationCode = locationCode,
                        KPSYear = yearWeek.Year.Value,
                        KPSWeek = yearWeek.Week.Value,
                        ProductionDate = date
                    };

                    if (IsPanel0CigarettesExist)
                        _executionTpobll.TPOProductionEntryVerificationGenerateReport(input, GetUserName());
                }

                return Json(bulkData);
            }
            catch (Exception ex)
            {
                _vtLogger.Err(ex, new List<object> { bulkData }, "Exe Product Adjustment - SaveProductAdjustment");
                return null;
            }
        }

        public FileStreamResult GenerateExcel(ProductAdjustmentInput input)
        {
            try
            {
                var allLocations = _appService.GetLocationCodeCompat();
                string locationCompat = "";
                foreach (SelectListItem item in allLocations)
                {
                    if (item.Value == input.LocationCode)
                    {
                        locationCompat = item.Text;
                    }
                }

                var productAdjustments = _executionOtherBll.GetProductAdjustments(input);

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }

                var templateFile = Enums.ExecuteExcelTemplate.ExecuteProductAdjustment + ".xlsx";
                var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);

                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + locationCompat);
                    slDoc.SetCellValue(4, 2, ": " + input.UnitCode);
                    slDoc.SetCellValue(5, 2, ": " + input.Shift);
                    slDoc.SetCellValue(6, 2, ": " + input.BrandCode);
                    slDoc.SetCellValue(7, 2, ": " + input.ProductionDate.Value.ToString("dd/MM/yyyy"));
                    slDoc.SetCellValue(3, 5, ": " + input.KpsYear);
                    slDoc.SetCellValue(4, 5, ": " + input.KpsWeek);

                    // row values
                    var iRow = 10;

                    foreach (var data in productAdjustments)
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

                        slDoc.SetCellValue(iRow, 1, data.AdjustmentType);
                        slDoc.SetCellValue(iRow, 2, data.AdjustmentValue.ToString());
                        slDoc.SetCellValue(iRow, 3, data.AdjustmentRemark);

                        slDoc.SetCellStyle(iRow, 1, iRow, 3, style);
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
                    slDoc.AutoFitColumn(1, 4);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);

                }

                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = " ProductionExecution_ProductAdjustment_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtLogger.Err(ex, new List<object> { input }, "Exe Product Adjustment - Generate Excel");
                return null;
            }
        }


    }
}