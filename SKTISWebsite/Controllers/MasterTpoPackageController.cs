using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.MstTPOPackage;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using System.Threading.Tasks;
using SKTISWebsite.Code;

namespace SKTISWebsite.Controllers
{
    public class MasterTpoPackageController : BaseController
    {
        private IMasterDataBLL _masterDataBLL;
        private IUploadService _uploadService;
        private IApplicationService _applicationService;
        private IPlanningBLL _planningBLL;
        private ITPOFeeBLL _tpoFeeBLL;
        private IExecutionTPOBLL _executionTpobll;

        public MasterTpoPackageController(IMasterDataBLL masterDataBll, IUploadService uploadService, IApplicationService applicationService, IPlanningBLL planningBLL, ITPOFeeBLL tpoFeeBll, IExecutionTPOBLL executionTpobll)
        {
            _masterDataBLL = masterDataBll;
            _uploadService = uploadService;
            _applicationService = applicationService;
            _planningBLL = planningBLL;
            _tpoFeeBLL = tpoFeeBll;
            _executionTpobll = executionTpobll;
            SetPage("MasterData/TPO/TPOPackages");
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var listYear = (Enumerable.Range(Convert.ToInt32(Enums.Years.Start), Convert.ToInt32(Enums.Years.End))).Select(fi => fi).ToList();
            var effectiveDate = _masterDataBLL.GetEffectiveDate();
            var expiredDate = effectiveDate.AddDays(6);

            var initTpoPackage = new InitMstTPOPackageViewModel()
            {
                Locations = Mapper.Map<List<SelectListItem>>(_masterDataBLL.GetMasterTPOInfos(new GetMasterTPOInfoInput())),
                BrandGroups = Mapper.Map<List<SelectListItem>>(_masterDataBLL.GetBrandGroups(new GetMstGenBrandGroupInput())),
                LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(_masterDataBLL.GetMstLocationLists(new GetMstGenLocationInput())),
                Years = _applicationService.GetGenWeekYears().ToList(),
                EffectiveDate = effectiveDate.ToShortDateString(),
                ExpiredDate = expiredDate.ToShortDateString(),
                UploadPath = new Uri(Request.Url, Url.Content("~/Upload/MasterTpoPackage/")).AbsoluteUri
            };
            return View("Index", initTpoPackage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMstTPOPackage(GetMstTPOPackagesInput criteria)
        {
            var TPOPackages = _masterDataBLL.GetTPOPackages(criteria);
            var viewModel = Mapper.Map<List<MstTPOPackageViewModel>>(TPOPackages);
            var pageResult = new PageResult<MstTPOPackageViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpGet]
        public ActionResult GetStartWeek()
        {
            var week = _masterDataBLL.GetWeekByDate(DateTime.Today);
            var date = _masterDataBLL.GetWeekByYearAndWeek(DateTime.Today.Year, week.Week.Value);
            var dates = new
            {
                StartDate = date.StartDate.Value.AddDays(-1),
                EndDate = date.EndDate
            };
            
            return Json(dates, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetEffectiveDates(GetMstTPOPackagesInput criteria)
        {
            var TPOPackages = _masterDataBLL.GetTPOPackages(criteria);
            var latestEffectiveDate = (TPOPackages.Count() != 0) ? TPOPackages.Max(m => m.ExpiredDate != null ? m.ExpiredDate : DateTime.Now.AddDays(-7)) : DateTime.Now.AddDays(-7);

            var input = new GetMstGenWeekInput()
            {
                CurrentDate = latestEffectiveDate.Value.AddDays(7)
            };

            var result = _masterDataBLL.GetMstGenWeek(input);
            var dates = new
            {
                StartDate = result.StartDate,
                EndDate = result.EndDate
            };

            return Json(dates);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllTPOPackages(InsertUpdateData<MstTPOPackageViewModel> bulkData)
        {
            var monday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            var sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 7);
            var weekYear = _masterDataBLL.GetWeekByDate(DateTime.Now);
            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var tpoPackage = Mapper.Map<MstTPOPackageDTO>(bulkData.New[i]);

                    //set ceatedby and updatedby
                    tpoPackage.CreatedBy = GetUserName();
                    tpoPackage.UpdatedBy = GetUserName();
                    
                    try
                    {
                        var item = _masterDataBLL.InsertTPOPackage(tpoPackage);

                        // re-calculate TPO Fee if current week edited
                        if (monday <= tpoPackage.EffectiveDate && tpoPackage.EffectiveDate <= sunday)
                        {
                            var brands = _masterDataBLL.GetBrandByBrandGroupCode(tpoPackage.BrandGroupCode);
                            if (brands.Count > 0)
                            {
                                foreach (var brand in brands)
                                {
                                    var existPlan = _tpoFeeBLL.GetTpoFeeHdrByParam(tpoPackage.LocationCode, brand.BrandCode, weekYear.Year, weekYear.Week);
                                    if (existPlan != null)
                                    {
                                        _planningBLL.SubmitTpoTpk(tpoPackage.LocationCode, brand.BrandCode, weekYear.Year, weekYear.Week, GetUserName());
                                    }

                                    var existActual = _tpoFeeBLL.GetTpoFeeHdrPlanByParam(tpoPackage.LocationCode, brand.BrandCode, weekYear.Year, weekYear.Week);
                                    if (existActual != null)
                                    {
                                        var input = new GetExeTPOProductionEntryVerificationInput
                                        {
                                            LocationCode = tpoPackage.LocationCode,
                                            BrandCode = brand.BrandCode,
                                            KPSYear = (int)weekYear.Year,
                                            KPSWeek = (int)weekYear.Week,
                                            ProductionDate = tpoPackage.EffectiveDate
                                        };
                                        _executionTpobll.TPOProductionEntryVerificationGenerateReport(input, GetUserName());
                                        _executionTpobll.InsertTPOExeReportByGroups(input.LocationCode, input.BrandCode, input.KPSYear, input.KPSWeek, input.ProductionDate, GetUserName());

                                    }
                                }
                            }
                        }

                        bulkData.New[i] = Mapper.Map<MstTPOPackageViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            }
            // Update data
            if (bulkData.Edit != null)
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var tpoPackage = Mapper.Map<MstTPOPackageDTO>(bulkData.Edit[i]);
                    tpoPackage.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBLL.UpdateTPOPackage(tpoPackage);

                        // re-calculate TPO Fee if current week edited
                        if (monday <= tpoPackage.EffectiveDate && tpoPackage.EffectiveDate <= sunday)
                        {
                            var brands = _masterDataBLL.GetBrandByBrandGroupCode(tpoPackage.BrandGroupCode);
                            if (brands.Count > 0)
                            {
                                foreach (var brand in brands)
                                {
                                    var existPlan = _tpoFeeBLL.GetTpoFeeHdrByParam(tpoPackage.LocationCode, brand.BrandCode, weekYear.Year, weekYear.Week);
                                    if (existPlan != null)
                                    {
                                        _planningBLL.SubmitTpoTpk(tpoPackage.LocationCode, brand.BrandCode, weekYear.Year, weekYear.Week, GetUserName());
                                    }

                                    var existActual = _tpoFeeBLL.GetTpoFeeHdrPlanByParam(tpoPackage.LocationCode, brand.BrandCode, weekYear.Year, weekYear.Week);
                                    if (existActual != null)
                                    {
                                        var input = new GetExeTPOProductionEntryVerificationInput
                                        {
                                            LocationCode = tpoPackage.LocationCode,
                                            BrandCode = brand.BrandCode,
                                            KPSYear = (int) weekYear.Year,
                                            KPSWeek = (int) weekYear.Week,
                                            ProductionDate = tpoPackage.EffectiveDate
                                        };
                                        _executionTpobll.TPOProductionEntryVerificationGenerateReport(input, GetUserName());
                                        _executionTpobll.InsertTPOExeReportByGroups(input.LocationCode, input.BrandCode, input.KPSYear, input.KPSWeek, input.ProductionDate, GetUserName());
            
                                    }
                                }
                            }
                        }

                        bulkData.Edit[i] = Mapper.Map<MstTPOPackageViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }

            return Json(bulkData);
        }

        /// <summary>
        /// Upload file function for both insert and update row on GridView
        /// </summary>
        /// <param name="modulePath">Module/Menu name on sidebar</param>
        /// <returns>UploadService</returns>
        [HttpPost]
        public Task<JsonResult> Upload(string modulePath="")
        {
            return _uploadService.Upload(modulePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationCode"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string year)
        {
            var input = new GetMstTPOPackagesInput { LocationCode = locationCode, Year = year };

            var masterTpoPackage = _masterDataBLL.GetTPOPackages(input);
            var ms = new MemoryStream();
            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstTPOPackage + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                var locationName = _masterDataBLL.GetMstLocationById(locationCode);

                slDoc.SetCellValue(2, 2, locationName == null ? "All" : locationCode+" - "+locationName.LocationName);

                if (year != null)
                    slDoc.SetCellValue(3, 2, year == "" ? "All" : year);

                //row values
                var iRow = 6;

                foreach (var masterItem in masterTpoPackage)
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


                    slDoc.SetCellValue(iRow, 1, masterItem.LocationCode);
                    slDoc.SetCellValue(iRow, 2, masterItem.LocationName);
                    slDoc.SetCellValue(iRow, 3, masterItem.BrandGroupCode);
                    slDoc.SetCellValue(iRow, 4, masterItem.Package.ToString());
                    slDoc.SetCellValue(iRow, 5, masterItem.EffectiveDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellValue(iRow, 6, masterItem.ExpiredDate.ToString());
                    slDoc.SetCellValue(iRow, 7, masterItem.MemoRef);
                    slDoc.SetCellValue(iRow, 8, masterItem.MemoPath);
                    slDoc.SetCellValue(iRow, 9, masterItem.Remark);
                    slDoc.SetCellValue(iRow, 10, masterItem.UpdatedBy);
                    slDoc.SetCellValue(iRow, 11, masterItem.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 11, style);
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
                slDoc.AutoFitColumn(1, 11);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterTPOPackage_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public JsonResult GetBrandGroupByLocationCode(string locationCode)
        {
            var brandGroupCodes = _applicationService.GetBrandGroupCodeSelectListByParentLocationCode(locationCode);
            return Json(brandGroupCodes, JsonRequestBehavior.AllowGet);
        }
    }
}