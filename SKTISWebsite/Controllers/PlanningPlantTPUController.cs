using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.PlanningPlantTPU;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Models.UtilTransactionLog;
using System.Globalization;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;

namespace SKTISWebsite.Controllers
{
    public class PlanningPlantTPUController : BaseController
    {
        private IPlanningBLL _planningBLL;
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private ISSISPackageService _ssisPackageService;
        private IGeneralBLL _generalBLL;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;

        public PlanningPlantTPUController(IPlanningBLL planningBLL,IVTLogger vtlogger, IApplicationService svc, IMasterDataBLL masterDataBLL, ISSISPackageService ssisPackageService, IGeneralBLL GeneralBLL, IUtilitiesBLL UtilitiesBLL)
        {
            _planningBLL = planningBLL;
            _svc = svc;
            _masterDataBLL = masterDataBLL;
            _ssisPackageService = ssisPackageService;
            _generalBLL = GeneralBLL;
            _utilitiesBLL = UtilitiesBLL;
            _vtlogger = vtlogger;
            SetPage("Productionplanning/Plant/TargetProductionUnit");
        }

        public JsonResult ValidateSubmittedEntry(string param1,string param2)
        {
            var isAlreadySubmitted = _utilitiesBLL.CheckAllDataAlreadySubmitForTPU(param1,param2);
            var message = new BLLException(ExceptionCodes.BLLExceptions.SubmittedPlantEntry);
            return Json(new { isAlreadySubmitted = isAlreadySubmitted, notification = message }, JsonRequestBehavior.AllowGet);
        }

        // GET: PlanningPlantTPU
        public ActionResult Index(string location, string brand, int? shift, int? year, int? week)
        {
            var plntChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            var defaultLocationCode = !String.IsNullOrEmpty(location)?location: GetDefaultLocationCode(plntChildLocationLookupList);
            var defaultBrandCode = !String.IsNullOrEmpty(brand) ? brand.Replace("-", ".") : GetDefaultBrandCodeByLocation(defaultLocationCode);
            //var defaultKPSWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now);
            var defaultKPSWeek = week.HasValue ? week : DefaultWeekForPlanningPlantTpu(defaultLocationCode, defaultBrandCode, DateTime.Now.Year);

            var targetWPP = GetTargetWPPValue(defaultLocationCode, defaultBrandCode, DateTime.Now.Year, defaultKPSWeek);

            var model = new InitPlantTPUViewModel
            {
                YearSelectList = _svc.GetGenWeekYears(),
                ConversionSelectList = _svc.GetConversion(),
                PLNTChildLocationLookupList = plntChildLocationLookupList,
                DefaultWeek = defaultKPSWeek,
                DefaultYear = year.HasValue ? year.Value : DateTime.Now.Year,
                DefaultBrandCode = defaultBrandCode,
                DefaultTargetWPP = targetWPP,
                DefaultShift = shift.HasValue ? shift.Value : 1,
                LocationCode = defaultLocationCode,
                Brand = brand,
                Shift = shift.HasValue ? shift.Value : 0,
                Year = year.HasValue ? year.Value : 0,
                Week = week.HasValue ? week.Value : 0,
                TodayDate = DateTime.Now.ToShortDateString()
            };

            return View(model);
        }

        public JsonResult GetPlantLocationCode()
        {
            var locCodes = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            return Json(locCodes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetState(string locationCode, string brandCode, int kpsYear, int kpsWeek, int shift)
        {
            var statusModelDTO = _planningBLL.GetStatePlanTPU(locationCode, brandCode, kpsYear, kpsWeek, shift);
            var statusViewModel = Mapper.Map<TPUStatusModel>(statusModelDTO);

            return Json(statusViewModel, JsonRequestBehavior.AllowGet);
        }

        private bool GetTPUSubmitStatus(string locationCode, string brandCode, int kpsYear, int kpsWeek, int shift)
        {
            var TPUTransCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.TPU) + "/"
                                          + locationCode + "/"
                                          + brandCode + "/"
                                          + kpsYear + "/"
                                          + kpsWeek + "/"
                                          + shift.ToString();
            var WPPTransCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.WPP) + "/"
                                      + kpsYear + "/"
                                      + kpsWeek;

            var WPPTransLog = _utilitiesBLL.GetTransactionLogByTransCodeAndIDFlow(WPPTransCode, (int)HMS.SKTIS.Core.Enums.IdFlow.WPPTPKTPOSubmit);

            var TPUSubmitLog = _utilitiesBLL.GetTransactionLogByTransCodeAndIDFlow(TPUTransCode, (int)HMS.SKTIS.Core.Enums.IdFlow.PlanningPlantTPUSubmit);


            if (WPPTransLog != null && TPUSubmitLog != null)
                return false;
            else if (WPPTransLog != null && TPUSubmitLog == null)
                return true;
            else return false;
        }

        private float GetTargetWPPValue(string locationCode, string brandCode, int? kpsYear, int? kpsWeek)
        {
            var getTargetWPPInput = new GetTargetWPPInput()
            {
                BrandCode = brandCode,
                KPSWeek = kpsWeek ?? 1,
                KPSYear = kpsYear ?? DateTime.Now.Year,
                LocationCode = locationCode,
            };

            var targetWPP = _planningBLL.GetTargetWPP(getTargetWPPInput);
            return targetWPP;
        }

        public int? DefaultWeekForPlanningPlantTpu(string location, string brandCode, int year)
        {
            var resultWeeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now);
            //var getTargetWPPInput = new PlanWeeklyProductionPlanningInput
            //{
            //    BrandCode = brandCode,
            //    LocationCode = location,
            //    KPSYear = year
            //};

            //var targetWPP = _planningBLL.GetPlanWeeklyProductionPlannings(getTargetWPPInput);
            //if (targetWPP.Count == 0)
            //    return resultWeeek + 1;
            return resultWeeek; //> targetWPP.Max(m => m.KPSWeek) ? targetWPP.Max(m => m.KPSWeek) : resultWeeek + 1;
        }

        /// <summary>
        /// Gets the default brand code by location.
        /// </summary>
        /// <param name="defaultLocationCode">The default location code.</param>
        /// <returns></returns>
        private string GetDefaultBrandCodeByLocation(string defaultLocationCode)
        {
            var defaultBrandCode = string.Empty;
            if (string.IsNullOrEmpty(defaultLocationCode)) return defaultBrandCode;
            var brandCodes = _masterDataBLL.GetBrandCodeByLocationCode(defaultLocationCode);
            defaultBrandCode = brandCodes.FirstOrDefault();
            return defaultBrandCode;
        }

        /// <summary>
        /// Gets the default location code.
        /// </summary>
        /// <param name="locations">The locations.</param>
        /// <returns></returns>
        private string GetDefaultLocationCode(List<LocationLookupList> locations)
        {
            var defaultLocationCode = string.Empty;
            var locationLookupList = locations.FirstOrDefault();
            if (locationLookupList != null)
            {
                defaultLocationCode = locationLookupList.LocationCode;
            }
            return defaultLocationCode;
        }

        /// <summary>
        /// Gets the plan tp us.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPlanTPUs(GetPlanTPUsInput criteria)
        {
            var planTPUs = _planningBLL.GetPlanTPUs(criteria);
            var viewModel = Mapper.Map<List<PlanTPUViewModel>>(planTPUs);
            var pageResult = new PageResult<PlanTPUViewModel>(viewModel);
            return Json(pageResult);
        }

        /// <summary>
        /// Saves all plan tp us.
        /// </summary>
        /// <param name="bulkData">The bulk data.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllPlanTPUs(InsertUpdateData<PlanTPUViewModel> bulkData)
        {
            var editedFlag = "true";
            var conversion = bulkData.Parameters.ContainsKey("Conversion") ? bulkData.Parameters["Conversion"] : "";

            // Update data

            if (bulkData.Edit != null)
            {
                try
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        if (bulkData.Edit[i] == null) continue;
                        var tpu = Mapper.Map<PlanTPUDTO>(bulkData.Edit[i]);

                        if (editedFlag.Equals(bulkData.Edit[i].Message)) // proccess edited row only
                        {

                            //set updatedby
                            tpu.UpdatedBy = GetUserName();

                            try
                            {
                                tpu.Conversion = conversion;
                                var item = _planningBLL.UpdatePlanTPU(tpu);
                                bulkData.Edit[i] = Mapper.Map<PlanTPUViewModel>(item);
                                bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                            }
                            catch (ExceptionBase ex)
                            {
                                _vtlogger.Err(ex, new List<object> { tpu }, "SaveAllPlanTPU");
                                bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                                bulkData.Edit[i].Message = ex.Message;
                            }
                            catch (Exception ex)
                            {
                                _vtlogger.Err(ex, new List<object> { tpu }, "SaveAllPlanTPU");
                                bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                                bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                            }
                            //insert into transaction log
                            var transactionInput = new TransactionLogInput
                            {
                                page = "TPU",
                                code_1 = "TPU",
                                code_2 = bulkData.Parameters.ContainsKey("LocationCode") ? bulkData.Parameters["LocationCode"] : "",
                                code_3 = bulkData.Parameters.ContainsKey("BrandCode") ? bulkData.Parameters["BrandCode"] : "",
                                code_4 = bulkData.Parameters.ContainsKey("KPSYear") ? bulkData.Parameters["KPSYear"] : "",
                                code_5 = bulkData.Parameters.ContainsKey("KPSWeek") ? bulkData.Parameters["KPSWeek"] : "",
                                code_6 = bulkData.Parameters.ContainsKey("Shift") ? bulkData.Parameters["Shift"] : "",
                                ActionButton = "Save",
                                UserName = GetUserName(),
                                IDRole = CurrentUser.Responsibility.Role
                            };
                            _generalBLL.ExeTransactionLog(transactionInput);

                            //
                        }
                    }
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { bulkData }, "Save Planning Plant TPU");
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public ActionResult SubmitData(int year, int week, string locationCode, string brandCode, int shift)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            var plantTpuInput = new GetPlanTPUsInput
            {
                LocationCode = locationCode,
                BrandCode = brandCode,
                KPSYear = year,
                KPSWeek = week,
                Shift = shift
            };

            try
            {
                _ssisPackageService.RunPlanTPKSSISPackage(GetUserName(), year, week, locationCode, brandCode, shift);
                var transactionInput = new TransactionLogInput
                {
                    page = "TPU",
                    code_1 = "TPU",
                    code_2 = locationCode,
                    code_3 = brandCode,
                    code_4 = year.ToString(),
                    code_5 = week.ToString(),
                    code_6 = shift.ToString(),
                    ActionButton = "Submit",
                    UserName = GetUserName(),
                    IDRole = CurrentUser.Responsibility.Role
                };
                _generalBLL.ExeTransactionLog(transactionInput);

                resultJSonSubmitData = "Run submit data on background process.";
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = "Failed to run submit data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
                _vtlogger.Err(ex, new List<object> { listResultJSon }, "Submit Planning Plant TPU");
                return Json(listResultJSon);
            }

            try
            {
                _planningBLL.SendEmailSubmitPlantTpu(plantTpuInput, GetUserName());
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSubmitData);
                listResultJSon.Add(resultJSonSendEmail);

                _vtlogger.Err(ex, new List<object> { listResultJSon }, "Failed to send email when Submit Planning Plant TPU");

                return Json(listResultJSon);
            }

            listResultJSon.Add(resultJSonSubmitData);
            return Json(listResultJSon);
        }

        /// <summary>
        /// Gets the brand group by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBrandCodeByLocationCode(string locationCode)
        {
            var brandCodes = _masterDataBLL.GetBrandCodeByLocationCode(locationCode);
            return Json(brandCodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBrandCodeByLocationYearWeek(string locationCode, int? KPSYear, int? KPSWeek)
        {
            var brandCodes = _planningBLL.GetBrandCodeByLocationYearAndWeekTPU(locationCode, KPSYear, KPSWeek);
            return Json(brandCodes, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the shift by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetShiftByLocationCode(string locationCode)
        {
            var shiftList = _masterDataBLL.GetShiftByLocationCode(locationCode);
            return Json(shiftList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the KPS week.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetKPSWeek(int year, string locationCode, string brandCode)
        {
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            var listWeek = new WeekList
            {
                Weeks = kpsWeek,
                DefaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now)
                //DefaultWeek = DefaultWeekForPlanningPlantTpu(locationCode, brandCode, year)
            };
            return Json(listWeek, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the target WPP.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTargetWPP(string locationCode, string brandCode, int year, int week)
        {
            var targetWPP = GetTargetWPPValue(locationCode, brandCode, year, week);
            return Json(targetWPP, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Stick per Box by Brand Code
        /// </summary>
        /// <param name="brandCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetStickPerBox(string brandCode)
        {
            var stickPerBox = _planningBLL.GetStickPerBoxByBrandCode(brandCode);
            return Json(stickPerBox, JsonRequestBehavior.AllowGet);
        }

        private int GetMaxCountFromTPU(List<int> processWorkHours)
        {
            var array = new int[processWorkHours.Max()+1];
            foreach (var a in processWorkHours)
            {
                array[a] = array[a] + 1;
            }
            return Array.IndexOf(array, array.Max());
        }

        /// <summary>
        /// Generates the excel.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="shift">The shift.</param>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string brandCode, int shift, int year, int week, string conversion)
        {
            var input = new GetPlanTPUsInput
            {
                LocationCode = locationCode,
                BrandCode = brandCode,
                Shift = shift,
                KPSYear = year,
                KPSWeek = week,
                Conversion = conversion
            };

            var planTPUs = _planningBLL.GetPlanTPUs(input);

            //prepare data for excel
            var productionStartDate = GetProductionStartDate(planTPUs.FirstOrDefault());
            var locationInfo = _masterDataBLL.GetLocation(locationCode);
            var pwh1 = planTPUs.Select(x => x.ProcessWorkHours1.Value).ToList();
            var pwh2 = planTPUs.Select(x => x.ProcessWorkHours2.Value).ToList();
            var pwh3 = planTPUs.Select(x => x.ProcessWorkHours3.Value).ToList();
            var pwh4 = planTPUs.Select(x => x.ProcessWorkHours4.Value).ToList();
            var pwh5 = planTPUs.Select(x => x.ProcessWorkHours5.Value).ToList();
            var pwh6 = planTPUs.Select(x => x.ProcessWorkHours6.Value).ToList();
            var pwh7 = planTPUs.Select(x => x.ProcessWorkHours7.Value).ToList();

            var maxHoursMon = GetMaxCountFromTPU(pwh1);
            var maxHoursTue = GetMaxCountFromTPU(pwh2);
            var maxHoursWed = GetMaxCountFromTPU(pwh3);
            var maxHoursThu = GetMaxCountFromTPU(pwh4);
            var maxHoursFri = GetMaxCountFromTPU(pwh5);
            var maxHoursSat = GetMaxCountFromTPU(pwh6);
            var maxHoursSun = GetMaxCountFromTPU(pwh7);
            //var maxHoursMon = planTPUs.Max(x => x.ProcessWorkHours1);
            //var maxHoursTue = planTPUs.Max(x => x.ProcessWorkHours2);
            //var maxHoursWed = planTPUs.Max(x => x.ProcessWorkHours3);
            //var maxHoursThu = planTPUs.Max(x => x.ProcessWorkHours4);
            //var maxHoursFri = planTPUs.Max(x => x.ProcessWorkHours5);
            //var maxHoursSat = planTPUs.Max(x => x.ProcessWorkHours6);
            //var maxHoursSun = planTPUs.Max(x => x.ProcessWorkHours7);
            var totalMaxHours = maxHoursMon + maxHoursTue + maxHoursWed + maxHoursThu + maxHoursFri + maxHoursSat +
                                maxHoursSun;

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.PlanningExcelTemplate.ProductionPlanTPU + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 5, locationCode);
                slDoc.SetCellValue(2, 7, locationInfo.LocationName);
                slDoc.SetCellValue(3, 5, brandCode);
                slDoc.SetCellValue(4, 5, shift);
                slDoc.SetCellValue(2, 12, ": "+ year);
                slDoc.SetCellValue(3, 12, ": " + week);
                slDoc.SetCellValue(5, 5, conversion);

                //production day
                slDoc.SetCellValue(7, 5, productionStartDate.HasValue ? productionStartDate.Value.DayOfWeek.ToString() : string.Empty);
                slDoc.SetCellValue(7, 10, productionStartDate.HasValue ? productionStartDate.Value.AddDays(1).DayOfWeek.ToString() : string.Empty);
                slDoc.SetCellValue(7, 15, productionStartDate.HasValue ? productionStartDate.Value.AddDays(2).DayOfWeek.ToString() : string.Empty);
                slDoc.SetCellValue(7, 20, productionStartDate.HasValue ? productionStartDate.Value.AddDays(3).DayOfWeek.ToString() : string.Empty);
                slDoc.SetCellValue(7, 25, productionStartDate.HasValue ? productionStartDate.Value.AddDays(4).DayOfWeek.ToString() : string.Empty);
                slDoc.SetCellValue(7, 30, productionStartDate.HasValue ? productionStartDate.Value.AddDays(5).DayOfWeek.ToString() : string.Empty);
                slDoc.SetCellValue(7, 35, productionStartDate.HasValue ? productionStartDate.Value.AddDays(6).DayOfWeek.ToString() : string.Empty);

                //production date
                slDoc.SetCellValue(8, 5, productionStartDate.HasValue ? productionStartDate.Value.ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(8, 10, productionStartDate.HasValue ? productionStartDate.Value.AddDays(1).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(8, 15, productionStartDate.HasValue ? productionStartDate.Value.AddDays(2).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(8, 20, productionStartDate.HasValue ? productionStartDate.Value.AddDays(3).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(8, 25, productionStartDate.HasValue ? productionStartDate.Value.AddDays(4).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(8, 30, productionStartDate.HasValue ? productionStartDate.Value.AddDays(5).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(8, 35, productionStartDate.HasValue ? productionStartDate.Value.AddDays(6).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);

                //max hours per day
                slDoc.SetCellValue(9, 5, maxHoursMon.ToString());
                slDoc.SetCellValue(9, 10, maxHoursTue.ToString());
                slDoc.SetCellValue(9, 15, maxHoursWed.ToString());
                slDoc.SetCellValue(9, 20, maxHoursThu.ToString());
                slDoc.SetCellValue(9, 25, maxHoursFri.ToString());
                slDoc.SetCellValue(9, 30, maxHoursSat.ToString());
                slDoc.SetCellValue(9, 35, maxHoursSun.ToString());
                slDoc.SetCellValue(9, 40, totalMaxHours.ToString());

                //conversion
                slDoc.SetCellValue(10, 6, "Historical Capacity ("+conversion.ToLower()+"/d)");
                slDoc.SetCellValue(10, 8, "Target ("+conversion.ToLower()+"/d)");
                slDoc.SetCellValue(10, 11, "Historical Capacity (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 13, "Target (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 16, "Historical Capacity (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 18, "Target (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 21, "Historical Capacity (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 23, "Target (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 26, "Historical Capacity (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 28, "Target (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 31, "Historical Capacity (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 33, "Target (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 36, "Historical Capacity (" + conversion.ToLower() + "/d)");
                slDoc.SetCellValue(10, 38, "Target (" + conversion.ToLower() + "/d)");

                //row values
                var iRow = 12;
                var totalRow = planTPUs.Count;
                var totalTargetSystem1 = 0.0;
                var totalTargetSystem2 = 0.0;
                var totalTargetSystem3 = 0.0;
                var totalTargetSystem4 = 0.0;
                var totalTargetSystem5 = 0.0;
                var totalTargetSystem6 = 0.0;
                var totalTargetSystem7 = 0.0;

                var totalTargetManual1 = 0.0;
                var totalTargetManual2 = 0.0;
                var totalTargetManual3 = 0.0;
                var totalTargetManual4 = 0.0;
                var totalTargetManual5 = 0.0;
                var totalTargetManual6 = 0.0;
                var totalTargetManual7 = 0.0;

                var totalTargetSystemAll = 0.0;
                var totalTargetManualAll = 0.0;

                SLStyle cellBgStyle = slDoc.CreateStyle();
                cellBgStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

                foreach (var tpu in planTPUs)
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

                    //slDoc.SetCellValue(iRow, 1, tpu.LocationCode);
                    slDoc.SetCellValue(iRow, 1, tpu.UnitCode);
                    slDoc.SetCellValue(iRow, 2, tpu.WorkerRegister.ToString());
                    slDoc.SetCellValue(iRow, 3, tpu.WorkerAvailable.ToString());
                    slDoc.SetCellValue(iRow, 4, tpu.WorkerAlocation.ToString());
                    //monday
                    slDoc.SetCellValue(iRow, 5, tpu.PercentAttendance1.ToString());
                    slDoc.SetCellValue(iRow, 6, (tpu.HistoricalCapacityWorker1 != null ? tpu.HistoricalCapacityWorker1 : 0).ToString());
                    slDoc.SetCellValue(iRow, 7, tpu.HistoricalCapacityGroup1.ToString());
                    slDoc.SetCellValue(iRow, 8, tpu.TargetSystem1.ToString());
                    slDoc.SetCellValue(iRow, 9, tpu.TargetManual1.ToString());
                    slDoc.SetCellStyle(iRow, 9, cellBgStyle);
                    //tuesday
                    slDoc.SetCellValue(iRow, 10, tpu.PercentAttendance2.ToString());
                    slDoc.SetCellValue(iRow, 11, (tpu.HistoricalCapacityWorker2 != null ? tpu.HistoricalCapacityWorker2 : 0).ToString());
                    slDoc.SetCellValue(iRow, 12, tpu.HistoricalCapacityGroup2.ToString());
                    slDoc.SetCellValue(iRow, 13, tpu.TargetSystem2.ToString());
                    slDoc.SetCellValue(iRow, 14, tpu.TargetManual2.ToString());
                    slDoc.SetCellStyle(iRow, 14, cellBgStyle);

                    //wednesday
                    slDoc.SetCellValue(iRow, 15, tpu.PercentAttendance3.ToString());
                    slDoc.SetCellValue(iRow, 16, (tpu.HistoricalCapacityWorker3 != null ? tpu.HistoricalCapacityWorker3 : 0).ToString());
                    slDoc.SetCellValue(iRow, 17, tpu.HistoricalCapacityGroup3.ToString());
                    slDoc.SetCellValue(iRow, 18, tpu.TargetSystem3.ToString());
                    slDoc.SetCellValue(iRow, 19, tpu.TargetManual3.ToString());
                    slDoc.SetCellStyle(iRow, 19, cellBgStyle);

                    //thursday
                    slDoc.SetCellValue(iRow, 20, tpu.PercentAttendance4.ToString());
                    slDoc.SetCellValue(iRow, 21, (tpu.HistoricalCapacityWorker4 != null ? tpu.HistoricalCapacityWorker4 : 0).ToString());
                    slDoc.SetCellValue(iRow, 22, tpu.HistoricalCapacityGroup4.ToString());
                    slDoc.SetCellValue(iRow, 23, tpu.TargetSystem4.ToString());
                    slDoc.SetCellValue(iRow, 24, tpu.TargetManual4.ToString());
                    slDoc.SetCellStyle(iRow, 24, cellBgStyle);

                    //friday
                    slDoc.SetCellValue(iRow, 25, tpu.PercentAttendance5.ToString());
                    slDoc.SetCellValue(iRow, 26, (tpu.HistoricalCapacityWorker5 != null ? tpu.HistoricalCapacityWorker4 : 0).ToString());
                    slDoc.SetCellValue(iRow, 27, tpu.HistoricalCapacityGroup5.ToString());
                    slDoc.SetCellValue(iRow, 28, tpu.TargetSystem5.ToString());
                    slDoc.SetCellValue(iRow, 29, tpu.TargetManual5.ToString());
                    slDoc.SetCellStyle(iRow, 29, cellBgStyle);

                    //saturday
                    slDoc.SetCellValue(iRow, 30, tpu.PercentAttendance6.ToString());
                    slDoc.SetCellValue(iRow, 31, (tpu.HistoricalCapacityWorker6 != null ? tpu.HistoricalCapacityWorker6 : 0).ToString());
                    slDoc.SetCellValue(iRow, 32, tpu.HistoricalCapacityGroup6.ToString());
                    slDoc.SetCellValue(iRow, 33, tpu.TargetSystem6.ToString());
                    slDoc.SetCellValue(iRow, 34, tpu.TargetManual6.ToString());
                    slDoc.SetCellStyle(iRow, 34, cellBgStyle);

                    //sunday
                    slDoc.SetCellValue(iRow, 35, tpu.PercentAttendance7.ToString());
                    slDoc.SetCellValue(iRow, 36, (tpu.HistoricalCapacityWorker7 != null ? tpu.HistoricalCapacityWorker7 : 0).ToString());
                    slDoc.SetCellValue(iRow, 37, tpu.HistoricalCapacityGroup7.ToString());
                    slDoc.SetCellValue(iRow, 38, tpu.TargetSystem7.ToString());
                    slDoc.SetCellValue(iRow, 39, tpu.TargetManual7.ToString());
                    slDoc.SetCellStyle(iRow, 39, cellBgStyle);

                    //total target system
                    var totalTargetSystem = tpu.TargetSystem1 + tpu.TargetSystem2 + tpu.TargetSystem3 +
                                            tpu.TargetSystem4 + tpu.TargetSystem5 + tpu.TargetSystem6 +
                                            tpu.TargetSystem7;
                    slDoc.SetCellValue(iRow, 40, totalTargetSystem.ToString());
                    //total target manual
                    var totalTargetManual = tpu.TargetManual1 + tpu.TargetManual2 + tpu.TargetManual3 +
                                            tpu.TargetManual4 + tpu.TargetManual5 + tpu.TargetManual6 +
                                            tpu.TargetManual7;
                    slDoc.SetCellValue(iRow, 41, totalTargetManual.ToString());
                    slDoc.SetCellStyle(iRow, 41, cellBgStyle);

                    totalTargetSystem1 += tpu.TargetSystem1 ?? 0.0;
                    totalTargetSystem2 += tpu.TargetSystem2 ?? 0.0;
                    totalTargetSystem3 += tpu.TargetSystem3 ?? 0.0;
                    totalTargetSystem4 += tpu.TargetSystem4 ?? 0.0;
                    totalTargetSystem5 += tpu.TargetSystem5 ?? 0.0;
                    totalTargetSystem6 += tpu.TargetSystem6 ?? 0.0;
                    totalTargetSystem7 += tpu.TargetSystem7 ?? 0.0;

                    totalTargetManual1 += tpu.TargetManual1 ?? 0.0;
                    totalTargetManual2 += tpu.TargetManual2 ?? 0.0;
                    totalTargetManual3 += tpu.TargetManual3 ?? 0.0;
                    totalTargetManual4 += tpu.TargetManual4 ?? 0.0;
                    totalTargetManual5 += tpu.TargetManual5 ?? 0.0;
                    totalTargetManual6 += tpu.TargetManual6 ?? 0.0;
                    totalTargetManual7 += tpu.TargetManual7 ?? 0.0;

                    totalTargetSystemAll += totalTargetSystem ?? 0.0;
                    totalTargetManualAll += totalTargetManual ?? 0.0;

                    slDoc.SetCellStyle(iRow, 1, iRow, 41, style);
                    iRow++;
                }
                slDoc.SetCellValue(iRow, 1, "Total");
                slDoc.SetCellValue(iRow, 2, totalRow);
                slDoc.SetCellValue(iRow, 8, Math.Round(totalTargetSystem1, 3).ToString());
                slDoc.SetCellValue(iRow, 9, Math.Round(totalTargetManual1,3).ToString());
                slDoc.SetCellValue(iRow, 13, Math.Round(totalTargetSystem2,3).ToString());
                slDoc.SetCellValue(iRow, 14, Math.Round(totalTargetManual2,3).ToString());
                slDoc.SetCellValue(iRow, 18, Math.Round(totalTargetSystem3,3).ToString());
                slDoc.SetCellValue(iRow, 19, Math.Round(totalTargetManual3,3).ToString());
                slDoc.SetCellValue(iRow, 23, Math.Round(totalTargetSystem4,3).ToString());
                slDoc.SetCellValue(iRow, 24, Math.Round(totalTargetManual4,3).ToString());
                slDoc.SetCellValue(iRow, 28, Math.Round(totalTargetSystem5,3).ToString());
                slDoc.SetCellValue(iRow, 29, Math.Round(totalTargetManual5,3).ToString());
                slDoc.SetCellValue(iRow, 33, Math.Round(totalTargetSystem6,3).ToString());
                slDoc.SetCellValue(iRow, 34, Math.Round(totalTargetManual6,3).ToString());
                slDoc.SetCellValue(iRow, 38, Math.Round(totalTargetSystem7,3).ToString());
                slDoc.SetCellValue(iRow, 39, Math.Round(totalTargetManual7,3).ToString());
                slDoc.SetCellValue(iRow, 40, Math.Round(totalTargetManualAll,3).ToString());
                slDoc.SetCellValue(iRow, 41, Math.Round(totalTargetManualAll,3).ToString());


                SLStyle style2 = slDoc.CreateStyle();
                style2.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                style2.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                style2.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                style2.Font.FontName = "Calibri";
                style2.Font.FontSize = 10;
                style2.Font.Bold = true;
                style2.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                slDoc.SetCellStyle(iRow, 1, iRow, 41, style2);

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
            var fileName = "ProductionPlanning_TargetProductionUnit_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private static DateTime? GetProductionStartDate(PlanTPUCompositeDTO planTpu)
        {
            if (planTpu != null)
                return planTpu.ProductionStartDate;

            return null;
        }

        [HttpGet]
        public JsonResult GetDateByYearWeek(int? year, int? week)
        {
            var inputYear = year.HasValue ? year.Value : DateTime.Now.Year;
            var inputWeek = week.HasValue ? week.Value : 1;
            var date = _masterDataBLL.GetWeekByYearAndWeek(inputYear, inputWeek);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the production start date.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        public JsonResult GetProductionStartDate(int year, int week)
        {
            var startDate = string.Empty;
            var kpsWeek = _masterDataBLL.GetWeekByYearAndWeek(year, week);
            if (kpsWeek != null && kpsWeek.StartDate.HasValue)
            {
                startDate = kpsWeek.StartDate.Value.ToString(Constants.DefaultDateOnlyFormat);
            }

            return Json(startDate, JsonRequestBehavior.AllowGet);
        }

        public class WeekList
        {
            public int? DefaultWeek { get; set; }
            public List<int> Weeks { get; set; }
        }

        [HttpPost]
        public JsonResult CalculatePlantTPU(InsertUpdateData<PlanTPUViewModel> bulkData)
        {
            var inputPlantTPU = new CalculatePlantTPUInput()
            {
                KPSYear = Convert.ToInt16(bulkData.Parameters["KPSYear"]),
                KPSWeek = Convert.ToInt16(bulkData.Parameters["KPSWeek"]),
                LocationCode = bulkData.Parameters["LocationCode"],
                BrandCode = bulkData.Parameters["BrandCode"],
                ListPlantTPU = Mapper.Map<List<PlanTPUDTO>>(bulkData.Edit),
                Conversion = bulkData.Parameters["Conversion"],
                FilterCurrentDayForward = Convert.ToDateTime(bulkData.Parameters["FilterCurrentDayForward"]),
                //FilterCurrentDayForward = DateTime.ParseExact(bulkData.Parameters["FilterCurrentDayForward"], Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
                IsFilterCurrentDayForward = Convert.ToBoolean(bulkData.Parameters["IsFilterCurrentDayForward"]),
                Shift = String.IsNullOrEmpty(bulkData.Parameters["Shift"]) ? 0 : Convert.ToInt32(bulkData.Parameters["Shift"])
            };

            var targetWpp = GetTargetWPPValue(inputPlantTPU.LocationCode, inputPlantTPU.BrandCode, inputPlantTPU.KPSYear, inputPlantTPU.KPSWeek);

            var submitStatus = GetTPUSubmitStatus(inputPlantTPU.LocationCode, inputPlantTPU.BrandCode,  inputPlantTPU.KPSYear, inputPlantTPU.KPSWeek, inputPlantTPU.Shift);
            var result = _planningBLL.CalculatePlantTPURevision(inputPlantTPU, targetWpp, submitStatus);


            var viewModels = Mapper.Map<List<PlanTPUViewModel>>(result);
            var pageResult = new PageResult<PlanTPUViewModel>
            {
                Results = viewModels
            };
            try
            {
                foreach (var viewModel in viewModels)
                {
                    viewModel.Message = "true";
                }

                

                return Json(pageResult);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { submitStatus, viewModels, result, inputPlantTPU, targetWpp }, "CalculatePlantTPU");
            }
            
            return Json(pageResult);
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