using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.Application.Resources.Views.PlanningWPP;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.Factories.Contract;
using SKTISWebsite.Models.MasterBrand;
using SKTISWebsite.Models.PlanningWPP;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Models.UtilTransactionLog;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Code;

namespace SKTISWebsite.Controllers
{
    public class PlanningWPPController : BaseController
    {
        private IMasterDataBLL _masterDataBLL;
        private IPlanningBLL _planingBLL;
        private AbstractSelectList _abstractSelectList;
        private ISSISPackageService _ssisPackageService;
        private IGeneralBLL _generalBLL;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;
        private IApplicationService _applicationService;

        public PlanningWPPController(IApplicationService applicationService, IMasterDataBLL masterDataBLL, IVTLogger vtlogger, AbstractSelectList abstractSelectList, IPlanningBLL planingBLL, ISSISPackageService ssisPackageService, IGeneralBLL GeneralBLL, IUtilitiesBLL UtilitiesBLL)
        {
            _masterDataBLL = masterDataBLL;
            _planingBLL = planingBLL;
            _abstractSelectList = abstractSelectList;
            _ssisPackageService = ssisPackageService;
            _generalBLL = GeneralBLL;
            _utilitiesBLL = UtilitiesBLL;
            _vtlogger = vtlogger;
            _applicationService = applicationService;
            SetPage("Productionplanning/WeeklyProductionPlanning");
        }

        //
        // GET: /PlanningWPP/
        public ActionResult Index()
        {
            //initialize filter

            var currentYear = DateTime.Now.Year;

            // initialize year filter
            var listYear = (Enumerable.Range(Convert.ToInt32(currentYear - Enums.Years.End+1), Convert.ToInt32(Enums.Years.End))).Select(fi => fi).ToList();
            var kpsYear = listYear.Select(x => new SelectListItem
                                               {
                                                   Text = x.ToString(),
                                                   Value = x.ToString(),
                                                   Selected = (x == currentYear)
                                               }).ToList();
            // initialize month filter

            var defaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now);
            var kpsWeek = _masterDataBLL.GetWeekByYear(currentYear).Select(x => new SelectListItem
                                                                      {
                                                                          Text = x.ToString(),
                                                                          Value = x.ToString(),
                                                                          Selected = (x == defaultWeek)
                                                                      }).ToList();



            _abstractSelectList._DataSource = _masterDataBLL.GetGeneralLists(new[] { Enums.MasterGeneralList.BrandFamily.ToString() });
            var locationInfo = _masterDataBLL.GetLocationInfo(CurrentUser.Location[0].Code);
            var brandcode = _masterDataBLL.GetBrandByBrandFamily();

            // initialize header table
            var tableResult = new WPPResultModel
            {
                WeeklyProductionPlannings = new List<WPP13WeekModel>(),
                Weeks = new List<int>()
            };
            for (var i = 1; i < 14; i++)
            {
                tableResult.Weeks.Add(i);
            }


            var model = new InitPlanningWPPModel
                        {
                            WPPResult = tableResult,
                            KPSYears = _applicationService.GetGenWeekYears().ToList(),
                            KPSWeeks = kpsWeek,
                            BrandFamily = _abstractSelectList.CreateBrandFamily(),
                            LocationCodes = Mapper.Map<List<SelectListItem>>(locationInfo),
                            BrandCodes = Mapper.Map<List<BrandCodeModel>>(brandcode),
                            DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now),
                        };
            return View(model);
        }

        /// <summary>
        /// get PlanWeeklyProductionPlanning with parameter
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult GetPlanWeeklyProductionPlanning(PlanWeeklyProductionPlanningInput data)
        {
            var year = data.KPSYear != null ? (int)data.KPSYear : 0;
            var week = data.KPSWeek != null ? (int)data.KPSWeek : 0;

            var wpp = _planingBLL.GetPlanWeeklyProductionPlannings(data);
            var result = new WPPResultModel
                         {
                             WeeklyProductionPlannings = Mapper.Map<List<WPP13WeekModel>>(wpp),
                             Weeks = _masterDataBLL.Get13Weeks(year, week)
                         };

            return PartialView("WPPTablePartial", result);

        }

        [HttpPost]
        public ActionResult UploadWPP()
        {

            string fileName = Request.Headers["X-File-Name"];

            string kpsYear = Request.Headers["KPSYear"];

            //File's content is available in Request.InputStream property
            System.IO.Stream fileContent = Request.InputStream;
            //Creating a FileStream to save file's content

            string filepath = Server.MapPath("~/Upload/WPP/") + fileName;

            System.IO.FileStream fileStream = System.IO.File.Create(filepath);
            fileContent.Seek(0, System.IO.SeekOrigin.Begin);
            //Copying file's content to FileStream
            fileContent.CopyTo(fileStream);
            fileStream.Dispose();

            int year = DateTime.Now.Year;

            // file extention validation
            string extension = Path.GetExtension(filepath);
            if (extension == EnumHelper.GetDescription(Enums.ExcelFormat.Xls))
            {
                return Json(PlanningWPP.ExcelFormatError);
            }

            // file size and file extention validation
            FileInfo fileInfo = new FileInfo(filepath);
            if (extension != EnumHelper.GetDescription(Enums.ExcelFormat.Xlsx) ||
                _planingBLL.ConvertBytesToMegabytes(fileInfo.Length) > 1)
            {
                return Json(PlanningWPP.FileSizeError);
            }

            var result = new WPPResultModel
            {
                WeeklyProductionPlannings = new List<WPP13WeekModel>()
            };

            using (var slDoc = new SLDocument(filepath))
            {
                try
                {
                    _planingBLL.DeleteAllTempWPP();

                    var week = slDoc.GetCellValueAsInt32(1, 3);

                    int number;
                    // validate week is number
                    for (var i = 3; i <= 15; i++)
                    {
                        var columnWeek = slDoc.GetCellValueAsString(1, i);
                        bool isNumber = int.TryParse(columnWeek, out number);
                        if (!isNumber)
                        {
                            return Json(PlanningWPP.NotValidWeekNonNumeric);
                        }
                    }

                    
                    if (week < _masterDataBLL.GetWeekByDate(DateTime.Today.Date).Week) {
                        year = year + 1;
                    }


                    var validWeek = _masterDataBLL.IsValidWeek(year, week);

                    if (!validWeek) return Json(PlanningWPP.NotValidWeek);

                    result.Weeks = _masterDataBLL.Get13Weeks(year, week);

                    var lastUsedRow = slDoc.GetWorksheetStatistics().EndRowIndex;
                    const int firstRow = 2;
                    const int firstWeek = 3;

                    var allBrands = _masterDataBLL.GetBrands(new GetBrandInput());
                    var allLocation = _masterDataBLL.GetMstLocationLists(new GetMstGenLocationInput());
                    var plantLocations = _masterDataBLL.GetPlantOrTPOLocations(Enums.LocationCode.PLNT.ToString());

                    var emptyrow = 0;

                    for (var i = firstRow; i <= lastUsedRow; i++)
                    {
                        if (emptyrow > Constants.MaxExcelEmptyRow) break;

                        var isvalid = true;
                        var isWarning = false;
                        var validationMessage = "";
                        //validate brand
                        var brandCode = slDoc.GetCellValueAsString(i, 1).Trim();
                        var brand = allBrands.FirstOrDefault(b => b.BrandCode == brandCode);

                        // variable to validate expired brand
                        var currentWeek = _masterDataBLL.GetWeekByDate(DateTime.Now);
                        int getWeek = (int)currentWeek.Week + 2;
                        var twoWeekLater = _masterDataBLL.GetWeekByYearAndWeek(year, getWeek);
                        if (brand == null)
                        {
                            validationMessage += PlanningWPP.ValidationBrandNotFound;
                            isvalid = false;
                        }
                        else
                        {
                            // check if expired date is current week or not
                            if (currentWeek.StartDate <= brand.ExpiredDate && brand.ExpiredDate <= currentWeek.EndDate)
                            {
                                validationMessage += PlanningWPP.ValidationBrandExpired;
                                isvalid = false;
                            }
                            // check expired date is warning or not.
                            else if (currentWeek.StartDate <= brand.ExpiredDate && twoWeekLater != null && twoWeekLater.EndDate >= brand.ExpiredDate)
                            {
                                validationMessage += PlanningWPP.ValidationBrandIsWarning;
                                isWarning = true;
                            }
                            // check if brand is expired.
                            else if (currentWeek.StartDate > brand.ExpiredDate)
                            {
                                validationMessage += PlanningWPP.ValidationBrandExpired;
                                isvalid = false;
                            }
                            // check if brand not efficient
                            else if (currentWeek.EndDate < brand.EffectiveDate)
                            {
                                validationMessage += PlanningWPP.ValidationBrandNotEffective;
                                isvalid = false;
                            }

                        }

                        //validate location
                        var LocationCode = slDoc.GetCellValueAsString(i, 2).Trim();
                        var loc = allLocation.FirstOrDefault(l => l.LocationCode == LocationCode);
                        if (loc == null)
                        {
                            validationMessage += PlanningWPP.ValidationLocationNotFound;
                            isvalid = false;
                        }
                        else
                        {
                            if (!loc.StatusActive)
                            {
                                validationMessage += PlanningWPP.ValidationLocationInActive;
                                isvalid = false;
                            }

                            var lctn = plantLocations.FirstOrDefault(p => p.LocationCode == LocationCode);
                            if (lctn != null && lctn.Shift > 1)
                            {
                                var validloc = _planingBLL.CheckLocationGroupShift(LocationCode, year, week);
                                if (!validloc)
                                {
                                    validationMessage += PlanningWPP.GroupShiftNotExist;
                                    isvalid = false;
                                }
                            }
                        }



                        if (string.IsNullOrEmpty(brandCode) && string.IsNullOrEmpty(brandCode))
                        {
                            emptyrow++;
                            continue;
                        }

                        // validate week value is decimal
                        decimal typeDecimal;
                        for (var x = 3; x <= 15; x++)
                        {
                            var columnWeekValue = slDoc.GetCellValueAsString(i, x);
                            bool isDecimal = decimal.TryParse(columnWeekValue, out typeDecimal);
                            if (!isDecimal)
                            {
                                return Json("error - WPP value must be in numeric/decimal format");
                            }
                        }


                        //validate value
                        var val = new decimal[13];
                        //var valValid = true;
                        //var valMessage = "";
                        for (var j = 0; j < 13; j++)
                        {
                            var Value = slDoc.GetCellValueAsDecimal(i, firstWeek + j);
                            /* if (Value == 0)
                             {
                                 valValid = false;
                                 valMessage += result.Weeks[j] + ", ";
                             }*/
                            val[j] = Value;
                        }
                        /*
                        if (valValid == false)
                        {
                            validationMessage += string.Format(PlanningWPP.ValidationValue, valMessage.Trim(','));
                            isvalid = false;
                        }*/


                        var BrandLocation = _masterDataBLL.GetAllBrandCodeByLocationCode();
                        var bl = BrandLocation.FirstOrDefault(b => b.LocationCode == LocationCode && b.BrandCode == brandCode);
                        if (bl == null)
                        {
                            validationMessage += PlanningWPP.ValidationBrandLocation;
                            isvalid = false;
                        }

                        var wpp13Exsist = result.WeeklyProductionPlannings.FirstOrDefault(wpp => wpp.BrandCode == brandCode && wpp.LocationCode == LocationCode);
                        var wpp13Model = new WPP13WeekModel()
                                              {
                                                  KPSYear = year,
                                                  KPSWeek = week,
                                                  BrandCode = brandCode.Trim(),
                                                  LocationCode = LocationCode.Trim(),
                                                  Value1 = Convert.ToDecimal(val[0]),
                                                  Value2 = Convert.ToDecimal(val[1]),
                                                  Value3 = Convert.ToDecimal(val[2]),
                                                  Value4 = Convert.ToDecimal(val[3]),
                                                  Value5 = Convert.ToDecimal(val[4]),
                                                  Value6 = Convert.ToDecimal(val[5]),
                                                  Value7 = Convert.ToDecimal(val[6]),
                                                  Value8 = Convert.ToDecimal(val[7]),
                                                  Value9 = Convert.ToDecimal(val[8]),
                                                  Value10 = Convert.ToDecimal(val[9]),
                                                  Value11 = Convert.ToDecimal(val[10]),
                                                  Value12 = Convert.ToDecimal(val[11]),
                                                  Value13 = Convert.ToDecimal(val[12]),
                                                  IsValid = isvalid,
                                                  IsWarning = isWarning,
                                                  UpdatedBy = GetUserName(),
                                                  Message = validationMessage
                                              };
                        var wppDTO = Mapper.Map<PlanWeeklyProductionPlanningDTO>(wpp13Model);

                        if (wpp13Exsist == null)
                        {
                            //insert to table PlanTmpWeeklyProductionPlanning
                            _planingBLL.InsertTempWPP(wppDTO);
                        }
                        else
                        {
                            result.WeeklyProductionPlannings.Remove(wpp13Exsist);
                            _planingBLL.UpdateTempWPP(wppDTO);
                        }

                        //add to result
                        result.WeeklyProductionPlannings.Add(wpp13Model);
                    }
                    var transactionInput = new TransactionLogInput
                    {
                        page = "WPP",
                        code_1 = "WPP",
                        code_2 = year.ToString(),
                        code_3 = week.ToString(),
                        ActionButton = "Upload",
                        UserName = GetUserName(),
                        IDRole = CurrentUser.Responsibility.Role
                    };
                    _generalBLL.ExeTransactionLog(transactionInput);
                }
                catch (Exception ex)
                {
                    var week = slDoc.GetCellValueAsInt32(1, 3);
                    var validWeek = _masterDataBLL.IsValidWeek(year, week);
                    _vtlogger.Err(ex, new List<object> { fileName, filepath, validWeek, slDoc, result }, "Upload WPP");

                }
            }
            return PartialView("WPPTablePartial", result);
        }


        [HttpGet]
        public JsonResult GetKPSWeek(int year)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBrandCode(string brandFamily)
        {
            var brandcodes = _masterDataBLL.GetBrandByBrandFamily(brandFamily);
            return PartialView("BrandGroupCodePartial", Mapper.Map<List<BrandCodeModel>>(brandcodes));
        }

        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="brandFamily"></param>
        /// <param name="brandGroupCode"></param>
        /// <param name="brandCode"></param>
        /// <param name="locationCode"></param>
        /// <param name="year"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string brandFamily, string brandGroupCode, string brandCode, string locationCode, int year, int week)
        {

            var data = new PlanWeeklyProductionPlanningInput
                       {
                           KPSWeek = week,
                           KPSYear = year,
                           LocationCode = locationCode,
                           BrandCode = brandCode
                       };

            var result = new WPPResultModel
            {
                WeeklyProductionPlannings = Mapper.Map<List<WPP13WeekModel>>(_planingBLL.GetPlanWeeklyProductionPlannings(data)),
                Weeks = _masterDataBLL.Get13Weeks(year, week)
            };

            var locName = _masterDataBLL.GetLocation(locationCode);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.PlanningExcelTemplate.PlanWeeklyProductionPlanning + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (var slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 2, !string.IsNullOrEmpty(brandFamily) ? brandFamily : "All");
                slDoc.SetCellValue(3, 2, !string.IsNullOrEmpty(brandGroupCode) ? brandGroupCode : "All");
                slDoc.SetCellValue(4, 2, !string.IsNullOrEmpty(brandCode) ? brandCode : "All");
                slDoc.SetCellValue(5, 2, data.LocationCode);
                slDoc.SetCellValue(5, 3, locName.LocationName);
                slDoc.SetCellValue(3, 15, data.KPSYear.ToString());
                slDoc.SetCellValue(4, 15, data.KPSWeek.ToString());


                for (var i = 0; i < result.Weeks.Count; i++)
                {
                    slDoc.SetCellValue(7, 3 + i, result.Weeks[i]);
                }

                //row values
                var iRow = 8;

                foreach (var wpp in result.WeeklyProductionPlannings)
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

                    slDoc.SetCellValue(iRow, 1, wpp.BrandCode);
                    slDoc.SetCellValue(iRow, 2, wpp.LocationCode);
                    slDoc.SetCellValue(iRow, 3, wpp.Value1);
                    slDoc.SetCellValue(iRow, 4, wpp.Value2);
                    slDoc.SetCellValue(iRow, 5, wpp.Value3);
                    slDoc.SetCellValue(iRow, 6, wpp.Value4);
                    slDoc.SetCellValue(iRow, 7, wpp.Value5);
                    slDoc.SetCellValue(iRow, 8, wpp.Value6);
                    slDoc.SetCellValue(iRow, 9, wpp.Value7);
                    slDoc.SetCellValue(iRow, 10, wpp.Value8);
                    slDoc.SetCellValue(iRow, 11, wpp.Value9);
                    slDoc.SetCellValue(iRow, 12, wpp.Value10);
                    slDoc.SetCellValue(iRow, 13, wpp.Value11);
                    slDoc.SetCellValue(iRow, 14, wpp.Value12);
                    slDoc.SetCellValue(iRow, 15, wpp.Value13);
                    slDoc.SetCellStyle(iRow, 1, iRow, 15, style);
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

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "ProductionPlanning_WeeklyProductionPlanning" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        //[HttpPost]
        public JsonResult SubmitData(string year)
        {
            var msg = new MessageModel();

            try
            {
                #region check re-upload wpp disabled checked this ticket : http://tp.voxteneo.co.id/entity/10079
                //if (!_planingBLL.IsValidWPP())
                //{
                //    msg.Type = "warning";
                //    msg.Message = PlanningWPP.ValidWPP;

                //    return Json(msg, JsonRequestBehavior.AllowGet);
                //}
                #endregion

                var transactionInput = new TransactionLogInput
                {
                    page = "WPP",
                    code_1 = "WPP",
                    code_2 = year,
                    code_3 = _planingBLL.GetKPSPlanTempWeeklyProductionGroup().FirstOrDefault().KPSWeek.ToString(),
                    ActionButton = "Submit",
                    UserName = GetUserName(),
                    IDRole = CurrentUser.Responsibility.Role
                };
                _generalBLL.ExeTransactionLog(transactionInput);

                _ssisPackageService.RunWPPSSISPackage(GetUserName());
                
                msg.Type = "success";
                msg.Message = PlanningWPP.SubmitSuccess;
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                msg.Type = "danger";
                msg.Message = PlanningWPP.SubmitFailed;
                _vtlogger.Err(ex, new List<object> { msg }, "Submit WPP");
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
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
