using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.ExePlantMaterialUsages;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Models.Common;
using AutoMapper;
using SpreadsheetLight;
using Color = System.Drawing.Color;

namespace SKTISWebsite.Controllers
{
    public class ExePlantMaterialUsagesController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionPlantBLL _executionPlantBll;
        private IVTLogger _vtlogger;

        public ExePlantMaterialUsagesController(IApplicationService applicationService, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IExecutionPlantBLL executionPlantBll)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _executionPlantBll = executionPlantBll;
            _vtlogger = vtlogger;
            SetPage("ProductionExecution/Plant/MaterialUsages");
        }

        // GET: ExePlantMaterialUsages
        public ActionResult Index()
        {
            var init = new InitExePlantMaterialUsagesViewModel
            {
                YearSelectList = _svc.GetGenWeekYears(),
                PLNTChildLocationLookupList = _svc.GetLastLocationChildList(Enums.LocationCode.PLNT.ToString()),
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now)
            };
            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetUnits(string locationCode)
        {
            var process = _svc.GetPlantUnitCodeSelectListByLocationCode(locationCode);
            return Json(process, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShifts(string locationCode)
        {
            var process = _svc.GetShiftByLocationCode(locationCode);
            return Json(process, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcess(string locationCode, int KPSYear, int KPSWeek)
        {
            var process = _svc.GetProcessGroupSelectListByLocationYearWeekFromPlantProdEntryVerification(locationCode, KPSYear, KPSWeek);
            return Json(process, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandGroup(string locationCode, string ProcessGroup, int KPSYear, int KPSWeek)
        {
            var process = _svc.GetBrandGroupCodeSelectListByPlantProdEntryVerification(locationCode, ProcessGroup, KPSYear, KPSWeek);
            return Json(process, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMaterial(string brandGroup, string ProcessGroup)
        {
            var process = _svc.GetMaterialByBrandGroup(brandGroup, ProcessGroup);
            return Json(process, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckMaterial(string materialCode, string brandGroup)
        {
            try
            {
                // todo pengecekan tobaco or non
                var isTobaco = true;
                var db = _masterDataBLL.GetMaterialByCode(materialCode, brandGroup);
                if (db != null)
                {
                    if (db.MaterialName.Contains("Non"))
                    {
                        isTobaco = false;
                    }
                }

                return Json(isTobaco, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                const bool isTobaco = true;
                _vtlogger.Err(ex, new List<object> { materialCode, brandGroup }, "Check Material");
                return Json(isTobaco, JsonRequestBehavior.AllowGet);
            }
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

        [HttpPost]
        public JsonResult GetExePlantMaterialUsages(GetExePlantMaterialUsagesInput criteria)
        {
            try
            {
                var masterLists = _executionPlantBll.GetMaterialUsages(criteria);
                var viewModel = Mapper.Map<List<ExePlantMaterialUsagesViewModel>>(masterLists);
                var pageResult = new PageResult<ExePlantMaterialUsagesViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { criteria }, "Get Exe Plant Material Usages");
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveAllMaterialUsages(InsertUpdateData<ExePlantMaterialUsagesViewModel> bulkData)
        {
            try
            {
                var material = bulkData.Parameters != null ? bulkData.Parameters["Material"] : "";
                var productionDate = bulkData.Parameters != null ? bulkData.Parameters["ProductionDate"] : "";
                // Update data
                if (bulkData.Edit != null)
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;

                        bulkData.Edit[i].ProductionDate = productionDate;
                        bulkData.Edit[i].MaterialCode = material;

                        var materialUsageDto = Mapper.Map<ExePlantMaterialUsagesDTO>(bulkData.Edit[i]);

                        materialUsageDto.UpdatedBy = GetUserName();
                        try
                        {
                            var item = _executionPlantBll.SaveMaterialUsage(materialUsageDto);

                            bulkData.Edit[i] = Mapper.Map<ExePlantMaterialUsagesViewModel>(item);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, materialUsageDto }, "Save All Material Usages - Update");
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            _vtlogger.Err(ex, new List<object> { bulkData, materialUsageDto }, "Save All Material Usages - Update");
                            bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }

                return Json(bulkData);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { bulkData }, "Save All Material Usages");
                return null;
            }
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string unit, int shift, string process, string brandGroup, string material, int year, int week, DateTime date)
        {
            try
            {
                var input = new GetExePlantMaterialUsagesInput
                {
                    LocationCode = locationCode,
                    Unit = unit,
                    Shift = shift,
                    Process = process,
                    BrandGroup = brandGroup,
                    Material = material,
                    Year = year,
                    Week = week,
                    Date = date
                };

                input.SortExpression = "GroupCode";
                input.SortOrder = "ASC";

                var location = _masterDataBLL.GetAllLocationByLocationCode(locationCode, 0).FirstOrDefault();

                var isTobacco = _masterDataBLL.GetMaterialIsTobbaco(input.Material, input.BrandGroup);
                var executionTPOProductionEntrys = _executionPlantBll.GetMaterialUsages(input);

                double? TotalAmbil1 = 0d;
                double? TotalAmbil2 = 0d;
                double? TotalAmbil3 = 0d;
                double? TotalProduction = 0d;
                double? TotalResidue = 0d;
                double? TotalReject = 0d;
                double? TotalFM = 0d;
                double? TotalStem = 0d;
                double? TotalSapon = 0d;
                double? TotalLost = 0d;

                var ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }
                if (isTobacco)
                {
                    var templateFile = Enums.ExecuteExcelTemplate.ExecutePlantMaterialUsageTobacco + ".xlsx";
                    var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                    if (System.IO.File.Exists(templateFileName))
                    {
                        System.IO.File.Copy(templateFileName, strFileName);
                    }
                }
                else
                {
                    var templateFile = Enums.ExecuteExcelTemplate.ExecutePlantMaterialUsageNonTobacco + ".xlsx";
                    var templateFileName = Server.MapPath(Constants.ExecuteExcelTemplatesFolder + templateFile);
                    if (System.IO.File.Exists(templateFileName))
                    {
                        System.IO.File.Copy(templateFileName, strFileName);
                    }
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(3, 2, ": " + location.LocationCompat);
                    slDoc.SetCellValue(4, 2, ": " + unit);
                    slDoc.SetCellValue(5, 2, ": " + shift.ToString());
                    slDoc.SetCellValue(6, 2, ": " + process);
                    slDoc.SetCellValue(7, 2, ": " + brandGroup);
                    slDoc.SetCellValue(8, 2, ": " + material);
                    slDoc.SetCellValue(3, 10, ": " + year.ToString());
                    slDoc.SetCellValue(4, 10, ": " + week.ToString());
                    slDoc.SetCellValue(5, 10, ": " + date.ToString(Constants.DefaultDateOnlyFormat));

                    //row values
                    var iRow = 12;

                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;


                    foreach (var masterListGroup in executionTPOProductionEntrys)
                    {
                        TotalAmbil1 += masterListGroup.Ambil1;
                        TotalAmbil2 += masterListGroup.Ambil2;
                        TotalAmbil3 += masterListGroup.Ambil3;
                        TotalProduction += masterListGroup.Production;
                        TotalResidue += masterListGroup.Sisa;
                        TotalReject += masterListGroup.CountableWaste;
                        TotalFM += masterListGroup.TobFM;
                        TotalStem += masterListGroup.TobStem;
                        TotalSapon += masterListGroup.TobSapon;
                        TotalLost += masterListGroup.UncountableWaste;


                        if (iRow%2 == 0)
                        {
                            style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                        }

                        if (isTobacco)
                        {
                            var lost = masterListGroup.Ambil1 + masterListGroup.Ambil2 + masterListGroup.Ambil3 -
                                       masterListGroup.Production - masterListGroup.TobFM - masterListGroup.TobStem -
                                       masterListGroup.TobStem;

                            slDoc.SetCellValue(iRow, 1, masterListGroup.GroupCode);
                            slDoc.SetCellValue(iRow, 2,
                                masterListGroup.Ambil1.HasValue ? Math.Round((double) masterListGroup.Ambil1, 2) : 0);
                            slDoc.SetCellValue(iRow, 3,
                                masterListGroup.Ambil2.HasValue ? Math.Round((double) masterListGroup.Ambil2, 2) : 0);
                            slDoc.SetCellValue(iRow, 4,
                                masterListGroup.Ambil3.HasValue ? Math.Round((double) masterListGroup.Ambil3, 2) : 0);
                            slDoc.SetCellValue(iRow, 5,
                                masterListGroup.Production.HasValue
                                    ? Math.Round((double) masterListGroup.Production, 3)
                                    : 0);
                            slDoc.SetCellValue(iRow, 6,
                                masterListGroup.TobFM.HasValue ? Math.Round((double) masterListGroup.TobFM, 2) : 0);
                            slDoc.SetCellValue(iRow, 7,
                                masterListGroup.TobSapon.HasValue ? Math.Round((double) masterListGroup.TobSapon, 2) : 0);
                            slDoc.SetCellValue(iRow, 8,
                                masterListGroup.TobStem.HasValue ? Math.Round((double) masterListGroup.TobStem, 2) : 0);
                            slDoc.SetCellValue(iRow, 9, lost.HasValue ? Math.Round((double) lost, 2) : 0);
                            slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
                        }
                        else
                        {
                            var lost = masterListGroup.Ambil1 + masterListGroup.Ambil2 + masterListGroup.Ambil3 -
                                       masterListGroup.Production - masterListGroup.Sisa -
                                       masterListGroup.CountableWaste;

                            slDoc.SetCellValue(iRow, 1, masterListGroup.GroupCode);
                            slDoc.SetCellValue(iRow, 2,
                                masterListGroup.Ambil1.HasValue ? Math.Round((double) masterListGroup.Ambil1, 2) : 0);
                            slDoc.SetCellValue(iRow, 3,
                                masterListGroup.Ambil2.HasValue ? Math.Round((double) masterListGroup.Ambil2, 2) : 0);
                            slDoc.SetCellValue(iRow, 4,
                                masterListGroup.Ambil3.HasValue ? Math.Round((double) masterListGroup.Ambil3, 2) : 0);
                            slDoc.SetCellValue(iRow, 5,
                                masterListGroup.Production.HasValue
                                    ? Math.Round((double) masterListGroup.Production, 3)
                                    : 0);
                            slDoc.SetCellValue(iRow, 6,
                                masterListGroup.Sisa.HasValue ? Math.Round((double) masterListGroup.Sisa, 2) : 0);
                            slDoc.SetCellValue(iRow, 7,
                                masterListGroup.CountableWaste.HasValue
                                    ? Math.Round((double) masterListGroup.CountableWaste, 2)
                                    : 0);
                            slDoc.SetCellValue(iRow, 8, lost.HasValue ? Math.Round((double) lost, 2) : 0);
                            slDoc.SetCellStyle(iRow, 1, iRow, 8, style);
                        }
                        iRow++;
                    }

                    slDoc.SetCellValue(iRow, 1, "Total");
                    slDoc.SetCellValue(iRow, 2, TotalAmbil1.HasValue ? TotalAmbil1.Value : 0);
                    slDoc.SetCellValue(iRow, 3, TotalAmbil2.HasValue ? TotalAmbil2.Value : 0);
                    slDoc.SetCellValue(iRow, 4, TotalAmbil3.HasValue ? TotalAmbil3.Value : 0);
                    slDoc.SetCellValue(iRow, 5, TotalProduction.HasValue ? Math.Round(TotalProduction.Value, 3) : 0);

                    if (isTobacco)
                    {
                        slDoc.SetCellValue(iRow, 6, TotalFM.HasValue ? TotalFM.Value : 0);
                        slDoc.SetCellValue(iRow, 7, TotalStem.HasValue ? TotalStem.Value : 0);
                        slDoc.SetCellValue(iRow, 8, TotalSapon.HasValue ? TotalSapon.Value : 0);
                        slDoc.SetCellValue(iRow, 9, TotalLost.HasValue ? TotalLost.Value : 0);
                        slDoc.SetCellStyle(iRow, 1, iRow + 1, 9, style);
                    }
                    else
                    {
                        slDoc.SetCellValue(iRow, 6, TotalResidue.HasValue ? TotalResidue.Value : 0);
                        slDoc.SetCellValue(iRow, 7, TotalReject.HasValue ? TotalReject.Value : 0);
                        slDoc.SetCellValue(iRow, 8, TotalLost.HasValue ? TotalLost.Value : 0);
                        slDoc.SetCellStyle(iRow, 1, iRow + 1, 8, style);
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
                    //slDoc.AutoFitColumn(1, 10);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "ProductionExecution_MaterialUsages_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { locationCode, unit, shift, process, brandGroup, material, year, week, date }, "Excel on exe plant material usages");
                return null;
            }
        }
    }
}