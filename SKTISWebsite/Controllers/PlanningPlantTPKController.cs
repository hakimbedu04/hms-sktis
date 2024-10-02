using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.PlanningPlantTPK;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs;
using SKTISWebsite.Models.UtilTransactionLog;

namespace SKTISWebsite.Controllers
{
    public class PlanningPlantTPKController : BaseController
    {
        private IPlanningBLL _planningBLL;
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private ISSISPackageService _ssisPackageService;
        private IGeneralBLL _generalBLL;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;

        public PlanningPlantTPKController(IPlanningBLL planningBLL, IVTLogger vtlogger, IApplicationService svc, IMasterDataBLL masterDataBLL, ISSISPackageService ssisPackageService, IGeneralBLL GeneralBLL, IUtilitiesBLL UtilitiesBLL)
        {
            _planningBLL = planningBLL;
            _svc = svc;
            _masterDataBLL = masterDataBLL;
            _ssisPackageService = ssisPackageService;
            _generalBLL = GeneralBLL;
            _utilitiesBLL = UtilitiesBLL;
            _vtlogger = vtlogger;
            SetPage("Productionplanning/Plant/PlantTargetProductionGroup");
        }

        // GET: PlanningPlantTPK
        public ActionResult Index(string param1, string param2, string param3, int? param4, int? param5, int? param6, int? param7)
        {
            if (param7.HasValue) setResponsibility(param7.Value);
            var plntChildLocationLookupList = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            var defaultLocationCode = GetDefaultLocationCode(plntChildLocationLookupList);
            var defaultBrandCode = GetDefaultBrandCodeByLocation(defaultLocationCode);
            var defaultKPSWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now);
            

            var targetWPP = GetTargetWPPValue(defaultLocationCode, defaultBrandCode, DateTime.Now.Year, defaultKPSWeek);

            var model = new InitPlantTPKViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                PLNTChildLocationLookupList = plntChildLocationLookupList,
                DefaultWeek = defaultKPSWeek,
                DefaultBrandCode = defaultBrandCode,
                DefaultTargetWPP = targetWPP,
                TodayDate = DateTime.Now.ToShortDateString(),
                Param1LocationCode = param1,
                Param2UnitCode = param2,
                Param3BrandCode = param3,
                Param4Shift = param4,
                Param5KPSYear = param5,
                Param6KPSWeek = param6
            };
            return View(model);
        }

        [HttpGet]
        public JsonResult GetPlantLocationCode()
        {
            var locCodes = _svc.GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString());
            return Json(locCodes, JsonRequestBehavior.AllowGet);
        }

       [HttpGet]
        public JsonResult GetShift(string locationCode)
        {
            var shift = _svc.GetShiftByLocationCode(locationCode);
            return Json(shift, JsonRequestBehavior.AllowGet);
        }

       public JsonResult ValidateSubmittedEntry(string param1, string param2)
       {
           var isAlreadySubmitted = _utilitiesBLL.CheckAllDataAlreadySubmit(param1,param2, Enums.PageName.PlantProductionEntry.ToString());
           var message = new BLLException(ExceptionCodes.BLLExceptions.SubmittedPlantEntry);
           return Json(new { isAlreadySubmitted = isAlreadySubmitted, notification = message }, JsonRequestBehavior.AllowGet);
       }

        /// <summary>
        /// Gets the plant tp ks.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPlantTPKs(GetPlantTPKsInput criteria)
        {
            var plantTPKViewModels = new List<PlanTPKViewModel>();

            //get list of tpk;
            var plantTPKs = _planningBLL.GetPlanningPlantTPK(criteria);

            //get list og process
            var listProcess = _masterDataBLL.GetAllActiveMasterProcess()
                .OrderBy(p => p.ProcessOrder)
                .ToList();

            foreach (var process in listProcess)
            {
                var processGroup = process.ProcessGroup;
                var plantTPKViewModel = new PlanTPKViewModel
                {
                    ProcessGroup = processGroup,
                    LocationCode = criteria.LocationCode,
                    UnitCode = criteria.UnitCode,
                    BrandCode = criteria.BrandCode,
                    KPSYear = criteria.KPSYear,
                    KPSWeek = criteria.KPSWeek,
                    ShowInWIPStock = process.WIP ?? false
                };

                var tpks = plantTPKs.Where(m => m.ProcessGroup == processGroup).OrderBy(u => u.GroupCode).ToList();
                if (!tpks.Any()) continue;

                //add wip stock
                if (process.WIP.HasValue && process.WIP.Value)
                {
                    var input = new GetPlantWIPStockInput()
                    {
                        LocationCode = criteria.LocationCode,
                        UnitCode = criteria.UnitCode,
                        BrandCode = criteria.BrandCode,
                        ProcessGroup = processGroup,
                        KPSYear = criteria.KPSYear,
                        KPSWeek = criteria.KPSWeek
                    };

                    var wipStock = _planningBLL.GetPlantWIPStock(input);
                    if (wipStock != null)
                    {
                        plantTPKViewModel.WIPPreviousValue = wipStock.WIPPreviousValue;
                        plantTPKViewModel.WIPStock1 = wipStock.WIPStock1;
                        plantTPKViewModel.WIPStock2 = wipStock.WIPStock2;
                        plantTPKViewModel.WIPStock3 = wipStock.WIPStock3;
                        plantTPKViewModel.WIPStock4 = wipStock.WIPStock4;
                        plantTPKViewModel.WIPStock5 = wipStock.WIPStock5;
                        plantTPKViewModel.WIPStock6 = wipStock.WIPStock6;
                        plantTPKViewModel.WIPStock7 = wipStock.WIPStock7;
                    }
                }

                //add jk process
                var firstTpk = tpks.FirstOrDefault();
                if (firstTpk != null)
                {
                    plantTPKViewModel.JKProcess1 = firstTpk.ProcessWorkHours1;
                    plantTPKViewModel.JKProcess2 = firstTpk.ProcessWorkHours2;
                    plantTPKViewModel.JKProcess3 = firstTpk.ProcessWorkHours3;
                    plantTPKViewModel.JKProcess4 = firstTpk.ProcessWorkHours4;
                    plantTPKViewModel.JKProcess5 = firstTpk.ProcessWorkHours5;
                    plantTPKViewModel.JKProcess6 = firstTpk.ProcessWorkHours6;
                    plantTPKViewModel.JKProcess7 = firstTpk.ProcessWorkHours7;
                }

                //add tpk list
                foreach (var plantTPKModel in tpks.Select(Mapper.Map<PlantTPKModel>))
                {
                    plantTPKViewModel.PlantTPK.Add(plantTPKModel);
                }

                plantTPKViewModels.Add(plantTPKViewModel);
            }

            var pageResult = new TPKPageResult<PlanTPKViewModel, TargetManualTPUDTO>
            {
                Results = plantTPKViewModels,
                CustomResults = Mapper.Map<List<TargetManualTPUDTO>>(GetTPKTotalResult(criteria))
            };
            
            return Json(pageResult);
        }

        private List<TargetManualTPUDTO> GetTPKTotalResult(GetPlantTPKsInput criteria)
        {
            var totalViewModel = new List<TargetManualTPUDTO>();
            var input = new GetTargetManualTPUInput()
            {
                LocationCode = criteria.LocationCode,
                UnitCode = criteria.UnitCode,
                BrandCode = criteria.BrandCode,
                KPSYear = criteria.KPSYear ?? 0,
                KPSWeek = criteria.KPSWeek ?? 0,
                Shift = criteria.Shift ?? 0
            };
            var total = _planningBLL.GetTargetManualTPU(input);
            total.TargetSystem1 = total.TargetManual1;
            total.TargetSystem2 = total.TargetManual2;
            total.TargetSystem3 = total.TargetManual3;
            total.TargetSystem4 = total.TargetManual4;
            total.TargetSystem5 = total.TargetManual5;
            total.TargetSystem6 = total.TargetManual6;
            total.TargetSystem7 = total.TargetManual7;
            totalViewModel.Add(total);
            return totalViewModel;
        }

        /// <summary>
        /// Saves all plan tp ks.
        /// </summary>
        /// <param name="bulkData">The bulk data.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllPlanTPKs(InsertUpdateData<PlanTPKViewModel> bulkData)
        {
            //var editedFlag = "true";

            // Update data
            if (bulkData.Edit != null)
            {
                try
                {
                    for (var i = 0; i < bulkData.Edit.Count; i++)
                    {
                        //check row is null
                        var viewModel = bulkData.Edit[i];
                        if (viewModel == null) continue;

                        //if (editedFlag.Equals(bulkData.Edit[i].Message)) // proccess edited row only
                        //{

                        var plantTPKByProcess = Mapper.Map<PlantTPKByProcessDTO>(bulkData.Edit[i]);

                        //set updatedby
                        plantTPKByProcess.UpdatedBy = GetUserName();
                        foreach (var tpk in plantTPKByProcess.PlantTPK)
                        {
                            tpk.UpdatedBy = GetUserName();
                            tpk.ProcessWorkHours1 = viewModel.JKProcess1;
                            tpk.ProcessWorkHours2 = viewModel.JKProcess2;
                            tpk.ProcessWorkHours3 = viewModel.JKProcess3;
                            tpk.ProcessWorkHours4 = viewModel.JKProcess4;
                            tpk.ProcessWorkHours5 = viewModel.JKProcess5;
                            tpk.ProcessWorkHours6 = viewModel.JKProcess6;
                            tpk.ProcessWorkHours7 = viewModel.JKProcess7;
                        }

                        try
                        {
                            if (plantTPKByProcess.ProcessGroup == "PACKING")
                            {
                                var sd = plantTPKByProcess;
                            }
                            var item = _planningBLL.SavePlantTPKByGroup(plantTPKByProcess);
                            bulkData.Edit[i] = Mapper.Map<PlanTPKViewModel>(item);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                            _vtlogger.Err(ex, new List<object> { plantTPKByProcess, bulkData.Edit[i].ResponseType, bulkData.Edit[i].Message }, "SaveAllPlanTPKs");
                        }
                        catch (Exception ex)
                        {
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            _vtlogger.Err(ex, new List<object> { plantTPKByProcess, bulkData.Edit[i].ResponseType, bulkData.Edit[i].Message }, "SaveAllPlanTPKs");
                        }
                        //insert into transactionlog
                        var transactionInput = new TransactionLogInput
                        {
                            page = "TPKPLANT",
                            code_1 = "TPK",
                            code_2 = bulkData.Parameters.ContainsKey("LocationCode") ? bulkData.Parameters["LocationCode"] : "",
                            code_3 = bulkData.Parameters.ContainsKey("BrandCode") ? bulkData.Parameters["BrandCode"] : "",
                            code_4 = bulkData.Parameters.ContainsKey("UnitCode") ? bulkData.Parameters["UnitCode"] : "",
                            code_5 = bulkData.Parameters.ContainsKey("KPSYear") ? bulkData.Parameters["KPSYear"] : "",
                            code_6 = bulkData.Parameters.ContainsKey("KPSWeek") ? bulkData.Parameters["KPSWeek"] : "",
                            code_7 = bulkData.Parameters.ContainsKey("Shift") ? bulkData.Parameters["Shift"] : "",
                            ActionButton = "Save",
                            UserName = GetUserName(),
                            IDRole = CurrentUser.Responsibility.Role
                        };
                        _generalBLL.ExeTransactionLog(transactionInput);
                        //
                        //}
                    }
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { bulkData }, "Save Button on PlanningPlanTPK");
                }
            }

            return Json(bulkData);
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

        [HttpGet]
        public JsonResult GetDateByYearWeek(int? year, int? week)
        {
            var inputYear = year.HasValue ? year.Value : DateTime.Now.Year;
            var inputWeek = week.HasValue ? week.Value : 1;
            var date = _masterDataBLL.GetWeekByYearAndWeek(inputYear, inputWeek);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the unit code by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUnitCodeByLocationCode(string locationCode)
        {
            var unitCodes = _svc.GetPlantUnitCodeSelectListByLocationCode(locationCode);
            return Json(unitCodes, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public JsonResult GetBrandCodeByLocationYearWeek(string locationCode, int? KPSYear, int? KPSWeek)
        {
            var brandCodes = _planningBLL.GetBrandCodeTPUByLocationYearAndWeek(locationCode, KPSYear, KPSWeek);
            return Json(brandCodes, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the KPS week.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetKPSWeek(int year)
        {
            var kpsWeeks = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeeks, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the production start date.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        public JsonResult GetProductionStartDate(int year, int week, string locationCode, string brandCode, int? unitCode)
        {
            var startDate = string.Empty;
            var startEnableDate = string.Empty;
            var kpsWeek = _masterDataBLL.GetWeekByYearAndWeek(year, week);
            if (kpsWeek != null && kpsWeek.StartDate.HasValue)
            {
                startDate = kpsWeek.StartDate.Value.ToString(Constants.DefaultDateOnlyFormat);
            }
            var tpkTransCode = EnumHelper.GetDescription(Enums.CombineCode.TPK) + "/"
                                          + locationCode + "/"
                                          + brandCode + "/"
                                          + unitCode + "/"
                                          + year + "/"
                                          + week;
            var alreadySubmit = false;
            var transactionLog = _utilitiesBLL.GetTransactionLogByTransCodeAndIDFlow(tpkTransCode, (int)Enums.IdFlow.PlanningPlantTPKSubmit);
            if (transactionLog != null)
            {
                alreadySubmit = true;
                startEnableDate = transactionLog.UpdatedDate.ToString(Constants.DefaultDateOnlyFormat);
            }

            return Json(new { startDate = startDate, alreadySubmit = alreadySubmit, startEnableDate = startEnableDate }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkerBrandAssigments(string groupCode, string processSettingsCode, int year, int week, string unitCode, string locationCode, string brandCode, DateTime? tPkPlantStartProdDate, string shift)
        {
            var locations = _masterDataBLL.GetMstLocationById(locationCode);
            var getPlanPlantAllocation = new GetPlanPlantAllocation()
            {
                GroupCode = groupCode,
                ProcessSettingsCode = processSettingsCode,
                Year = year,
                Week = week,
                UnitCode = unitCode,
                LocationCode = locationCode,
                BrandCode = brandCode,
                Shift = shift,
                PkPlantStartProdDate = tPkPlantStartProdDate
            };
            var employeeList = _planningBLL.GetWorkerBrandAssignmentPlanningPlantTpk(getPlanPlantAllocation);
            return Json(employeeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveWorkerBrandAssignment(List<WorkerBrandAssigmentDTO> workerBrandAssigments)
        {
            var workerBrandAssigmentDtos = Mapper.Map<List<PlanPlantAllocationDTO>>(workerBrandAssigments);
            workerBrandAssigmentDtos = workerBrandAssigmentDtos.Select(m =>
            {
                m.UpdatedBy = GetUserName();
                m.CreatedBy = GetUserName();
                return m;
            }).ToList();
            var result = _planningBLL.SavePlanPlantAllocations(workerBrandAssigmentDtos);
            return Json(result);
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

        private float GetTargetWPPValue(string locationCode, string brandCode, int? kpsYear, int? kpsWeek)
        {
            var getTargetWPPInput = new GetTargetWPPInput()
            {
                BrandCode = brandCode,
                KPSWeek = kpsWeek ?? 1,
                KPSYear = kpsYear ?? DateTime.Now.Year,
                LocationCode = locationCode
            };

            var targetWPP = _planningBLL.GetTargetWPP(getTargetWPPInput);
            return targetWPP;
        }

        /// <summary>
        /// Gets the total target manual tpu.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="unitCode">The unit code.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTargetManualTPU(string locationCode, string unitCode, string brandCode, int year, int week, int? shift)
        {
            var input = new GetTargetManualTPUInput()
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                BrandCode = brandCode,
                KPSYear = year,
                KPSWeek = week,
                Shift = shift
            };

            var targetManualTPU = _planningBLL.GetTargetManualTPU(input);

            return Json(targetManualTPU, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Generates the excel.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="unitCode">The unit code.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <param name="shift">Shift.</param>
        /// <returns></returns>
        public FileStreamResult GenerateExcel(string locationCode, string unitCode, string brandCode, int year, int week, int? shift)
        {
            var stickPerBox = 0;

            var mstGenBrand = _masterDataBLL.GetMstGenBrandById(brandCode);
            if (mstGenBrand != null)
            {
                var mstGenBrandGroup = _masterDataBLL.GetBrandGroupById(mstGenBrand.BrandGroupCode);
                if (mstGenBrandGroup != null)
                    stickPerBox = mstGenBrandGroup.StickPerBox ?? 0;
            }


            var plantTPKs = PrepareDataForExcel(locationCode, unitCode, brandCode, year, week, shift.HasValue ? shift.Value : 1);

            // data plant tpu (total box)
            var plantTpuTotalBox = Mapper.Map<List<TargetManualTPUDTO>>(GetTPKTotalResult(new GetPlantTPKsInput
            {
               LocationCode = locationCode,
               UnitCode = unitCode,
               BrandCode = brandCode,
               KPSWeek = week,
               KPSYear = year,
               Shift = shift
            })).FirstOrDefault();

            var locationInfo = _masterDataBLL.GetLocation(locationCode);
            //prepare data for excel
            var productionStartDate = GetProductionStartDate(plantTPKs.FirstOrDefault());
            var maxHoursMon = plantTPKs.Max(x => x.ProcessWorkHours1);
            var maxHoursTue = plantTPKs.Max(x => x.ProcessWorkHours2);
            var maxHoursWed = plantTPKs.Max(x => x.ProcessWorkHours3);
            var maxHoursThu = plantTPKs.Max(x => x.ProcessWorkHours4);
            var maxHoursFri = plantTPKs.Max(x => x.ProcessWorkHours5);
            var maxHoursSat = plantTPKs.Max(x => x.ProcessWorkHours6);
            var maxHoursSun = plantTPKs.Max(x => x.ProcessWorkHours7);
            var totalMaxHours = maxHoursMon + maxHoursTue + maxHoursWed + maxHoursThu + maxHoursFri + maxHoursSat +
                                maxHoursSun;

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.PlanningExcelTemplate.ProductionPlantTPK + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 4, locationCode);
                slDoc.SetCellValue(2, 6, locationInfo.LocationName);
                slDoc.SetCellValue(3, 4, unitCode);
                slDoc.SetCellValue(4, 4, brandCode);
                slDoc.SetCellValue(2, 9, year);
                slDoc.SetCellValue(3, 9, week);
                slDoc.SetCellValue(5, 4, "TPK/" + locationCode + "/" + brandCode + "/" + unitCode + "/" + year + "/" + week + "/" + (shift.HasValue ? shift.Value : 1));

                //production date
                slDoc.SetCellValue(9, 6, productionStartDate.HasValue ? productionStartDate.Value.ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(9, 12, productionStartDate.HasValue ? productionStartDate.Value.AddDays(1).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(9, 18, productionStartDate.HasValue ? productionStartDate.Value.AddDays(2).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(9, 24, productionStartDate.HasValue ? productionStartDate.Value.AddDays(3).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(9, 30, productionStartDate.HasValue ? productionStartDate.Value.AddDays(4).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(9, 36, productionStartDate.HasValue ? productionStartDate.Value.AddDays(5).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);
                slDoc.SetCellValue(9, 42, productionStartDate.HasValue ? productionStartDate.Value.AddDays(6).ToString(Constants.DefaultDateOnlyFormat) : string.Empty);

                //max hours per day
                slDoc.SetCellValue(10, 6, maxHoursMon.ToString());
                slDoc.SetCellValue(10, 12, maxHoursTue.ToString());
                slDoc.SetCellValue(10, 18, maxHoursWed.ToString());
                slDoc.SetCellValue(10, 24, maxHoursThu.ToString());
                slDoc.SetCellValue(10, 30, maxHoursFri.ToString());
                slDoc.SetCellValue(10, 36, maxHoursSat.ToString());
                slDoc.SetCellValue(10, 42, maxHoursSun.ToString());
                slDoc.SetCellValue(10, 48, totalMaxHours.ToString());

                //row values
                var iRow = 13;

                var totalTargetSystemBox1 = 0.0;
                var totalTargetSystemBox2 = 0.0;
                var totalTargetSystemBox3 = 0.0;
                var totalTargetSystemBox4 = 0.0;
                var totalTargetSystemBox5 = 0.0;
                var totalTargetSystemBox6 = 0.0;
                var totalTargetSystemBox7 = 0.0;

                var totalTargetManualBox1 = 0.0;
                var totalTargetManualBox2 = 0.0;
                var totalTargetManualBox3 = 0.0;
                var totalTargetManualBox4 = 0.0;
                var totalTargetManualBox5 = 0.0;
                var totalTargetManualBox6 = 0.0;
                var totalTargetManualBox7 = 0.0;

                var totalTargetSystemBox = 0.0;
                var totalTargetManualBox = 0.0;
                if (plantTPKs.Count > 0)
                {
                    foreach (var tpk in plantTPKs)
                    {
                        SLStyle style = slDoc.CreateStyle();
                        style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Font.FontName = "Calibri";
                        style.Font.FontSize = 10;

                        //if (iRow % 2 == 0)
                        if (tpk.ProcessGroup == "Total")
                        {
                            style.Font.Bold = true;
                            style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);

                            totalTargetSystemBox1 += tpk.TargetSystem1 ?? 0;
                            totalTargetSystemBox2 += tpk.TargetSystem2 ?? 0;
                            totalTargetSystemBox3 += tpk.TargetSystem3 ?? 0;
                            totalTargetSystemBox4 += tpk.TargetSystem4 ?? 0;
                            totalTargetSystemBox5 += tpk.TargetSystem5 ?? 0;
                            totalTargetSystemBox6 += tpk.TargetSystem6 ?? 0;
                            totalTargetSystemBox7 += tpk.TargetSystem7 ?? 0;

                            totalTargetManualBox1 += tpk.TargetManual1 ?? 0;
                            totalTargetManualBox2 += tpk.TargetManual2 ?? 0;
                            totalTargetManualBox3 += tpk.TargetManual3 ?? 0;
                            totalTargetManualBox4 += tpk.TargetManual4 ?? 0;
                            totalTargetManualBox5 += tpk.TargetManual5 ?? 0;
                            totalTargetManualBox6 += tpk.TargetManual6 ?? 0;
                            totalTargetManualBox7 += tpk.TargetManual7 ?? 0;

                            totalTargetSystemBox += tpk.TotalTargetSystem ?? 0;
                            totalTargetManualBox += tpk.TotalTargetManual ?? 0;
                        }

                        slDoc.SetCellValue(iRow, 1, tpk.ProcessGroup);
                        slDoc.SetCellValue(iRow, 2, tpk.GroupCode);
                        slDoc.SetCellValue(iRow, 3, tpk.WorkerRegister.ToString());
                        slDoc.SetCellValue(iRow, 4, tpk.WorkerAvailable.ToString());
                        slDoc.SetCellValue(iRow, 5, tpk.WorkerAllocation.ToString());
                        //monday
                        slDoc.SetCellValue(iRow, 6, tpk.PercentAttendance1.ToString());
                        slDoc.SetCellValue(iRow, 7, tpk.HistoricalCapacityWorker1.ToString());
                        slDoc.SetCellValue(iRow, 8, tpk.HistoricalCapacityGroup1.ToString());
                        slDoc.SetCellValue(iRow, 9, tpk.WIP1.ToString());
                        slDoc.SetCellValue(iRow, 10, tpk.TargetSystem1.ToString());
                        slDoc.SetCellValue(iRow, 11, tpk.TargetManual1.ToString());
                        //tuesday
                        slDoc.SetCellValue(iRow, 12, tpk.PercentAttendance2.ToString());
                        slDoc.SetCellValue(iRow, 13, tpk.HistoricalCapacityWorker2.ToString());
                        slDoc.SetCellValue(iRow, 14, tpk.HistoricalCapacityGroup2.ToString());
                        slDoc.SetCellValue(iRow, 15, tpk.WIP2.ToString());
                        slDoc.SetCellValue(iRow, 16, tpk.TargetSystem2.ToString());
                        slDoc.SetCellValue(iRow, 17, tpk.TargetManual2.ToString());
                        //wednesday
                        slDoc.SetCellValue(iRow, 18, tpk.PercentAttendance3.ToString());
                        slDoc.SetCellValue(iRow, 19, tpk.HistoricalCapacityWorker3.ToString());
                        slDoc.SetCellValue(iRow, 20, tpk.HistoricalCapacityGroup3.ToString());
                        slDoc.SetCellValue(iRow, 21, tpk.WIP3.ToString());
                        slDoc.SetCellValue(iRow, 22, tpk.TargetSystem3.ToString());
                        slDoc.SetCellValue(iRow, 23, tpk.TargetManual3.ToString());
                        //thursday
                        slDoc.SetCellValue(iRow, 24, tpk.PercentAttendance4.ToString());
                        slDoc.SetCellValue(iRow, 25, tpk.HistoricalCapacityWorker4.ToString());
                        slDoc.SetCellValue(iRow, 26, tpk.HistoricalCapacityGroup4.ToString());
                        slDoc.SetCellValue(iRow, 27, tpk.WIP4.ToString());
                        slDoc.SetCellValue(iRow, 28, tpk.TargetSystem4.ToString());
                        slDoc.SetCellValue(iRow, 29, tpk.TargetManual4.ToString());
                        //friday
                        slDoc.SetCellValue(iRow, 30, tpk.PercentAttendance5.ToString());
                        slDoc.SetCellValue(iRow, 31, tpk.HistoricalCapacityWorker5.ToString());
                        slDoc.SetCellValue(iRow, 32, tpk.HistoricalCapacityGroup5.ToString());
                        slDoc.SetCellValue(iRow, 33, tpk.WIP5.ToString());
                        slDoc.SetCellValue(iRow, 34, tpk.TargetSystem5.ToString());
                        slDoc.SetCellValue(iRow, 35, tpk.TargetManual5.ToString());
                        //saturday
                        slDoc.SetCellValue(iRow, 36, tpk.PercentAttendance6.ToString());
                        slDoc.SetCellValue(iRow, 37, tpk.HistoricalCapacityWorker6.ToString());
                        slDoc.SetCellValue(iRow, 38, tpk.HistoricalCapacityGroup6.ToString());
                        slDoc.SetCellValue(iRow, 39, tpk.WIP6.ToString());
                        slDoc.SetCellValue(iRow, 40, tpk.TargetSystem6.ToString());
                        slDoc.SetCellValue(iRow, 41, tpk.TargetManual6.ToString());
                        //sunday
                        slDoc.SetCellValue(iRow, 42, tpk.PercentAttendance7.ToString());
                        slDoc.SetCellValue(iRow, 43, tpk.HistoricalCapacityWorker7.ToString());
                        slDoc.SetCellValue(iRow, 44, tpk.HistoricalCapacityGroup7.ToString());
                        slDoc.SetCellValue(iRow, 45, tpk.WIP7.ToString());
                        slDoc.SetCellValue(iRow, 46, tpk.TargetSystem7.ToString());
                        slDoc.SetCellValue(iRow, 47, tpk.TargetManual7.ToString());
                        //total target system
                        slDoc.SetCellValue(iRow, 48, tpk.TotalTargetSystem.ToString());
                        //total target manual
                        slDoc.SetCellValue(iRow, 49, tpk.TotalTargetManual.ToString());

                        slDoc.SetCellStyle(iRow, 1, iRow, 49, style);
                        iRow++;
                    }

                    SLStyle style2 = slDoc.CreateStyle();
                    style2.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style2.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style2.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style2.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style2.Font.FontName = "Calibri";
                    style2.Font.FontSize = 10;
                    style2.Font.Bold = true;
                    style2.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);

                    slDoc.SetCellStyle(iRow, 1, iRow, 49, style2);
                    slDoc.SetCellValue(iRow, 1, "Total Box");

                    slDoc.SetCellValue(iRow, 10, plantTpuTotalBox.TargetSystem1.ToString());
                    slDoc.SetCellValue(iRow, 11, plantTpuTotalBox.TargetManual1.ToString());
                    slDoc.SetCellValue(iRow, 16, plantTpuTotalBox.TargetSystem2.ToString());
                    slDoc.SetCellValue(iRow, 17, plantTpuTotalBox.TargetManual2.ToString());
                    slDoc.SetCellValue(iRow, 22, plantTpuTotalBox.TargetSystem3.ToString());
                    slDoc.SetCellValue(iRow, 23, plantTpuTotalBox.TargetManual3.ToString());
                    slDoc.SetCellValue(iRow, 28, plantTpuTotalBox.TargetSystem4.ToString());
                    slDoc.SetCellValue(iRow, 29, plantTpuTotalBox.TargetManual4.ToString());
                    slDoc.SetCellValue(iRow, 34, plantTpuTotalBox.TargetSystem5.ToString());
                    slDoc.SetCellValue(iRow, 35, plantTpuTotalBox.TargetManual5.ToString());
                    slDoc.SetCellValue(iRow, 40, plantTpuTotalBox.TargetSystem6.ToString());
                    slDoc.SetCellValue(iRow, 41, plantTpuTotalBox.TargetManual6.ToString());
                    slDoc.SetCellValue(iRow, 46, plantTpuTotalBox.TargetSystem7.ToString());
                    slDoc.SetCellValue(iRow, 47, plantTpuTotalBox.TargetManual7.ToString());
                    slDoc.SetCellValue(iRow, 48, plantTpuTotalBox.TotalTargetSystem.ToString());
                    slDoc.SetCellValue(iRow, 49, plantTpuTotalBox.TotalTargetManual.ToString());

                    //slDoc.SetCellValue(iRow, 10, (Math.Round(totalTargetSystemBox1 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 11, (Math.Round(totalTargetManualBox1 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 16, (Math.Round(totalTargetSystemBox2 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 17, (Math.Round(totalTargetManualBox2 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 22, (Math.Round(totalTargetSystemBox3 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 23, (Math.Round(totalTargetManualBox3 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 28, (Math.Round(totalTargetSystemBox4 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 29, (Math.Round(totalTargetManualBox4 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 34, (Math.Round(totalTargetSystemBox5 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 35, (Math.Round(totalTargetManualBox5 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 40, (Math.Round(totalTargetSystemBox6 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 41, (Math.Round(totalTargetManualBox6 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 46, (Math.Round(totalTargetSystemBox7 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 47, (Math.Round(totalTargetManualBox7 / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 48, (Math.Round(totalTargetSystemBox / stickPerBox)).ToString());
                    //slDoc.SetCellValue(iRow, 49, (Math.Round(totalTargetManualBox / stickPerBox)).ToString());
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
            var fileName = "ProductionPlanning_PlantTargetProductionKelompok_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// Prepares the data for excel.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="unitCode">The unit code.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="kpsYear">The KPS year.</param>
        /// <param name="kpsWeek">The KPS week.</param>
        /// <returns></returns>
        private List<PlantTPKExcelModel> PrepareDataForExcel(string locationCode, string unitCode, string brandCode, int? kpsYear, int? kpsWeek, int shift)
        {
            var excelModels = new List<PlantTPKExcelModel>();

            //get list of tpk;
            var getPlantTPKsInput = new GetPlantTPKsInput()
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                BrandCode = brandCode,
                KPSYear = kpsYear,
                KPSWeek = kpsWeek,
                Shift = shift
            };

            var plantTPKs = _planningBLL.GetPlanningPlantTPK(getPlantTPKsInput);

            //get list og process
            var listProcess = _masterDataBLL.GetAllActiveMasterProcess()
                .OrderBy(p => p.ProcessOrder)
                .ToList(); ;

            foreach (var process in listProcess)
            {
                var processGroup = process.ProcessGroup;

                var tpks = plantTPKs.Where(m => m.ProcessGroup == processGroup).ToList();
                if (!tpks.Any()) continue;

                //add tpk list
                for (var i = 0; i <= tpks.Count; i++)
                {
                    var tpkExcelModel = new PlantTPKExcelModel();
                    if (i < tpks.Count)
                    {
                        var plantTpk = tpks[i];
                        tpkExcelModel = Mapper.Map<PlantTPKExcelModel>(plantTpk);
                        if (i != 0)
                            tpkExcelModel.ProcessGroup = String.Empty;
                    }
                    else
                    {
                        tpkExcelModel.ProcessGroup = "Total";
                        tpkExcelModel.GroupCode = tpks.Count.ToString();
                        tpkExcelModel.WorkerRegister = tpks.Sum(m => m.WorkerRegister);
                        tpkExcelModel.WorkerAvailable = tpks.Sum(m => m.WorkerAvailable);
                        tpkExcelModel.WorkerAllocation = tpks.Sum(m => m.WorkerAllocation);
                        tpkExcelModel.TargetSystem1 = tpks.Sum(m => m.TargetSystem1);
                        tpkExcelModel.TargetManual1 = tpks.Sum(m => m.TargetManual1);
                        tpkExcelModel.TargetSystem2 = tpks.Sum(m => m.TargetSystem2);
                        tpkExcelModel.TargetManual2 = tpks.Sum(m => m.TargetManual2);
                        tpkExcelModel.TargetSystem3 = tpks.Sum(m => m.TargetSystem3);
                        tpkExcelModel.TargetManual3 = tpks.Sum(m => m.TargetManual3);
                        tpkExcelModel.TargetSystem4 = tpks.Sum(m => m.TargetSystem4);
                        tpkExcelModel.TargetManual4 = tpks.Sum(m => m.TargetManual4);
                        tpkExcelModel.TargetSystem5 = tpks.Sum(m => m.TargetSystem5);
                        tpkExcelModel.TargetManual5 = tpks.Sum(m => m.TargetManual5);
                        tpkExcelModel.TargetSystem6 = tpks.Sum(m => m.TargetSystem6);
                        tpkExcelModel.TargetManual6 = tpks.Sum(m => m.TargetManual6);
                        tpkExcelModel.TargetSystem7 = tpks.Sum(m => m.TargetSystem7);
                        tpkExcelModel.TargetManual7 = tpks.Sum(m => m.TargetManual7);
                        tpkExcelModel.TotalTargetSystem = tpks.Sum(m => m.TargetSystem1) +
                                                          tpks.Sum(m => m.TargetSystem2) +
                                                          tpks.Sum(m => m.TargetSystem3) +
                                                          tpks.Sum(m => m.TargetSystem4) +
                                                          tpks.Sum(m => m.TargetSystem5) +
                                                          tpks.Sum(m => m.TargetSystem6) +
                                                          tpks.Sum(m => m.TargetSystem7);
                        //tpkExcelModel.TotalTargetManual = tpks.Sum(m => m.TotalTargetManual);
                        tpkExcelModel.TotalTargetManual = tpks.Sum(m => m.TargetManual1) +
                                                          tpks.Sum(m => m.TargetManual2) +
                                                          tpks.Sum(m => m.TargetManual3) +
                                                          tpks.Sum(m => m.TargetManual4) +
                                                          tpks.Sum(m => m.TargetManual5) +
                                                          tpks.Sum(m => m.TargetManual6) +
                                                          tpks.Sum(m => m.TargetManual7);
                    }

                    excelModels.Add(tpkExcelModel);
                }
            }

            return excelModels;
        }

        /// <summary>
        /// Gets the production start date.
        /// </summary>
        /// <param name="planTpk">The plan TPK.</param>
        /// <returns></returns>
        private static DateTime? GetProductionStartDate(PlantTPKExcelModel planTpk)
        {
            if (planTpk != null)
                return DateTime.ParseExact(planTpk.TPKPlantStartProductionDate, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);

            return null;
        }

        [HttpGet]
        public JsonResult GetMasterGenBrandCodeByBrandCode(string brandCode)
        {
            var mstGenBrand = _masterDataBLL.GetMstGenBrandById(brandCode);
            var mstGenBrandGroup = _masterDataBLL.GetBrandGroupById(mstGenBrand.BrandGroupCode);
            return Json(mstGenBrandGroup.StickPerBox, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitData(int year, int week, string locationCode, string brandCode, string unitCode, int shift)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {
                _ssisPackageService.RunSSISProductionEntryPlant(GetUserName(), year, week, locationCode, brandCode, unitCode);
                var transactionInput = new TransactionLogInput
                {
                    page = "TPKPLANT",
                    code_1 = "TPK",
                    code_2 = locationCode,
                    code_3 = brandCode,
                    code_4 = unitCode,
                    code_5 = year.ToString(),
                    code_6 = week.ToString(),
                    code_7 = shift.ToString(),
                    ActionButton = Enums.ButtonName.Submit.ToString().ToUpper(),
                    UserName = GetUserName(),
                    IDRole = CurrentUser.Responsibility.Role
                };
                _generalBLL.ExeTransactionLog(transactionInput);

                resultJSonSubmitData = "Run submit data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = "Failed to run submit data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
                _vtlogger.Err(ex, new List<object> { resultJSonSubmitData }, "Failed to run submit data on background process.");
                return Json(listResultJSon);
            }

            try
            {
                var tpkInput = new GetPlantTPKsInput
                {
                    KPSYear = year,
                    KPSWeek = week,
                    LocationCode = locationCode,
                    BrandCode = brandCode,
                    UnitCode = unitCode,
                    Shift = shift
                };

                _planningBLL.SendEmailSubmitPlantTpk(tpkInput, GetUserName());
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";
                listResultJSon.Add(resultJSonSendEmail);
                _vtlogger.Err(ex, new List<object> { resultJSonSendEmail }, "Failed to send email on submit planning plant TPK");
            }

            return Json(listResultJSon);
        }

        [HttpPost]
        public JsonResult CalculatePlantTPK(CustomInsertUpdateData<PlanTPKViewModel, TargetManualTPUDTO> bulkData)
        {
            var inputPlantTPK = new CalculatePlantTPKInput()
            {
                KPSYear = Convert.ToInt16(bulkData.Parameters["KPSYear"]),
                KPSWeek = Convert.ToInt16(bulkData.Parameters["KPSWeek"]),
                LocationCode = bulkData.Parameters["LocationCode"],
                BrandCode = bulkData.Parameters["BrandCode"],
                UnitCode = bulkData.Parameters["UnitCode"],
                ListPlantTPK = Mapper.Map<List<PlantTPKByProcessDTO>>(bulkData.Edit),
                TotalPlantTPK = Mapper.Map<List<TargetManualTPUDTO>>(bulkData.Total),
                //FilterCurrentDayForward = Convert.ToDateTime(bulkData.Parameters["FilterCurrentDayForward"]),
                FilterCurrentDayForward = DateTime.ParseExact(bulkData.Parameters["FilterCurrentDayForward"], Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
                IsFilterCurrentDayForward = Convert.ToBoolean(bulkData.Parameters["IsFilterCurrentDayForward"])
            };
            
            var result = _planningBLL.CalculatePlantTPK(inputPlantTPK);

            var viewModels = Mapper.Map<List<PlanTPKViewModel>>(result.PlantTPKByProcess);
            foreach (var plantTPKViewModel in viewModels)
            {
                try
                {
                    plantTPKViewModel.JKProcess1 = plantTPKViewModel.PlantTPK.Max(jk1 => jk1.ProcessWorkHours1);
                    plantTPKViewModel.JKProcess2 = plantTPKViewModel.PlantTPK.Max(jk2 => jk2.ProcessWorkHours2);
                    plantTPKViewModel.JKProcess3 = plantTPKViewModel.PlantTPK.Max(jk3 => jk3.ProcessWorkHours3);
                    plantTPKViewModel.JKProcess4 = plantTPKViewModel.PlantTPK.Max(jk4 => jk4.ProcessWorkHours4);
                    plantTPKViewModel.JKProcess5 = plantTPKViewModel.PlantTPK.Max(jk5 => jk5.ProcessWorkHours5);
                    plantTPKViewModel.JKProcess6 = plantTPKViewModel.PlantTPK.Max(jk6 => jk6.ProcessWorkHours6);
                    plantTPKViewModel.JKProcess7 = plantTPKViewModel.PlantTPK.Max(jk7 => jk7.ProcessWorkHours7);

                    var WIPProcess = _masterDataBLL.GetMasterProcesses(new GetMstGenProcessesInput() { ProcessName = plantTPKViewModel.ProcessGroup, WIP = true, StatusActive = true });
                    if (WIPProcess.Count > 0)
                    {
                        plantTPKViewModel.ShowInWIPStock = true;
                        var input = new GetPlantWIPStockInput()
                        {
                            LocationCode = inputPlantTPK.LocationCode,
                            UnitCode = inputPlantTPK.UnitCode,
                            BrandCode = inputPlantTPK.BrandCode,
                            ProcessGroup = plantTPKViewModel.ProcessGroup,
                            KPSYear = inputPlantTPK.KPSYear,
                            KPSWeek = inputPlantTPK.KPSWeek
                        };
                        var wipStock = _planningBLL.GetPlantWIPStock(input);
                        if (wipStock != null)
                        {
                            plantTPKViewModel.WIPPreviousValue = wipStock.WIPPreviousValue;
                            plantTPKViewModel.WIPStock1 = wipStock.WIPStock1;
                            plantTPKViewModel.WIPStock2 = wipStock.WIPStock2;
                            plantTPKViewModel.WIPStock3 = wipStock.WIPStock3;
                            plantTPKViewModel.WIPStock4 = wipStock.WIPStock4;
                            plantTPKViewModel.WIPStock5 = wipStock.WIPStock5;
                            plantTPKViewModel.WIPStock6 = wipStock.WIPStock6;
                            plantTPKViewModel.WIPStock7 = wipStock.WIPStock7;
                        }
                    }
                }
                catch(Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { viewModels, plantTPKViewModel }, "calculate button on planning plant TPK");
                }
            }

            var pageResult = new TPKPageResult<PlanTPKViewModel, TargetManualTPUDTO>
            {
                Results = viewModels,
                CustomResults = Mapper.Map<List<TargetManualTPUDTO>>(result.PlantTPKTotals)
            };


            return Json(pageResult);            
        }

        [HttpPost]
        public JsonResult GetState(string locationCode, string brandCode, string unitCode, int kpsYear, int kpsWeek, int shift)
        {
            var dateRange = _masterDataBLL.GetWeekByYearAndWeek(kpsYear, kpsWeek);
            var StatusModel = new TPKStatusModel();

            if (dateRange.StartDate.HasValue)
            {
                var TPKTransCode = EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.CombineCode.TPK) + "/"
                                          + locationCode + "/"
                                          + brandCode + "/"
                                          + unitCode + "/"
                                          + kpsYear + "/"
                                          + kpsWeek + "/"
                                          + shift.ToString();
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

                var TPUTransLog = _utilitiesBLL.GetTransactionLogByTransCodeAndIDFlow(TPUTransCode, (int)HMS.SKTIS.Core.Enums.IdFlow.PlanningPlantTPUSubmit);

                var TPKSubmitLog = _utilitiesBLL.GetTransactionLogByTransCodeAndIDFlow(TPKTransCode, (int)HMS.SKTIS.Core.Enums.IdFlow.PlanningPlantTPKSubmit);

                var isWPPReSubmitted = -1;
                var isTPUReSubmitted = -1;

                var submitTPU = false;
                if (WPPTransLog != null && TPUTransLog != null && TPKSubmitLog != null)
                {
                    isWPPReSubmitted = DateTime.Compare(WPPTransLog.TransactionDate, TPUTransLog.TransactionDate);
                    submitTPU = isWPPReSubmitted < 0 ? true : false;
                    if(submitTPU){
                        isTPUReSubmitted = DateTime.Compare(TPUTransLog.TransactionDate, TPKSubmitLog.TransactionDate);
                        StatusModel.Resubmit = isTPUReSubmitted > 0 ? true : false;
                    }
                }

                //StatusModel.SubmitLog = isTPUReSubmitted < 0 && TPKSubmitLog != null ? TPKSubmitLog : null;
                StatusModel.SubmitLog = isTPUReSubmitted < 0 && TPKSubmitLog != null ? true : false;
                StatusModel.Dates = new List<DateStateModel>();

                for (int day = 0; day <= 6; day++)
                {
                    var dateState = new DateStateModel()
                    {
                        Date = dateRange.StartDate.Value.AddDays(day)
                    };

                    if (TPKSubmitLog == null)
                    {
                        dateState.IsActive = true;
                    }
                    else
                    {
                        if (isTPUReSubmitted > 0)
                        {
                            dateState.IsActive = (dateState.Date.Date >= DateTime.Now.Date) ? true : false;
                        }
                        else if(dateState.Date.Date >= DateTime.Now.Date)
                        {
                            dateState.IsActive = true;
                        }
                    }

                    StatusModel.Dates.Add(dateState);
                }
            }

            return Json(StatusModel);
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