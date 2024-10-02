using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using SKTISWebsite.Models.PlanningTPOTPK;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs;
using SKTISWebsite.Models.UtilTransactionLog;
using System.Globalization;

namespace SKTISWebsite.Controllers
{
    public class PlanningTPOTPKController : BaseController
    {
        private IPlanningBLL _planningBLL;
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private ISSISPackageService _ssisPackageService;
        private IGeneralBLL _generalBLL;
        private IUtilitiesBLL _utilitiesBLL;
        private IVTLogger _vtlogger;

        public PlanningTPOTPKController(IPlanningBLL planningBLL, IVTLogger vtlogger, IApplicationService svc, IMasterDataBLL masterDataBLL, ISSISPackageService ssisPackageService, IGeneralBLL generalBLL, IUtilitiesBLL UtilitiesBLL)
        {
            _planningBLL = planningBLL;
            _svc = svc;
            _masterDataBLL = masterDataBLL;
            _ssisPackageService = ssisPackageService;
            _generalBLL = generalBLL;
            _utilitiesBLL = UtilitiesBLL;
            _vtlogger = vtlogger;
            SetPage("Productionplanning/TPO/TPOTargetProductionGroup");
        }
        public ActionResult Index()
        {
            var tpoChildLocation = _svc.GetLastLocationChildList(Enums.LocationCode.TPO.ToString());
            var defaultLocationCode = GetDefaultLocationCode(tpoChildLocation);
            var defaultBrandCode = GetDefaultBrandCodeByLocation(defaultLocationCode);
            var defaultWeek = _masterDataBLL.GetGeneralWeekWeekByDate(DateTime.Now);
            var targetWPP = GetTargetWPPValue(defaultLocationCode, defaultBrandCode, DateTime.Now.Year, defaultWeek);

            var model = new InitTPOTPKViewModel
            {
                YearSelectList = _svc.GetGenWeekYears(),
                TPOChildLocationLookupList = tpoChildLocation,
                DefaultWeek = defaultWeek,
                DefaultBrandCode = defaultBrandCode,
                DefaultTargetWPP = (int)targetWPP,
                TodayDate = DateTime.Now.ToShortDateString()
            };

            return View(model);
        }

        [HttpGet]
        public JsonResult GetDateByYearWeek(int? year, int? week)
        {
            var inputYear = year.HasValue ? year.Value : DateTime.Now.Year;
            var inputWeek = week.HasValue ? week.Value : 1;
            var date = _masterDataBLL.GetWeekByYearAndWeek(inputYear, inputWeek);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        private string GetDefaultLocationCode(List<LocationLookupList> locations)
        {
            var defaultLocationCode = string.Empty;
            var locationLookupList = locations.FirstOrDefault();
            if (locationLookupList != null)
            {
                defaultLocationCode = locationLookupList.LocationName;
            }
            return defaultLocationCode;
        }

        private string GetDefaultBrandCodeByLocation(string defaultLocationCode)
        {
            var defaultBrandCode = string.Empty;
            if (string.IsNullOrEmpty(defaultLocationCode)) return defaultBrandCode;
            var brandCodes = _masterDataBLL.GetBrandCodeByLocationCode(defaultLocationCode);
            defaultBrandCode = brandCodes.FirstOrDefault();
            return defaultBrandCode;
        }

        [HttpPost]
        public JsonResult GetTPOTPK(GetTPOTPKInput criteria)
        {
            var viewModels = new List<PlanTPOTPKViewModel>();

            //get list of tpk;
            var tpoTPKs = _planningBLL.GetPlanningTPOTPK(criteria);

            //get list of process
            var listProcess = _masterDataBLL.GetAllActiveMasterProcess()
                .OrderBy(p => p.ProcessOrder)
                .ToList();

            foreach (var process in listProcess)
            {
                var processGroup = process.ProcessGroup;
                var viewModel = new PlanTPOTPKViewModel
                {
                    ProcessGroup = processGroup,
                    LocationCode = criteria.LocationCode,
                    BrandCode = criteria.BrandCode,
                    KPSYear = criteria.KPSYear,
                    KPSWeek = criteria.KPSWeek,
                    ShowInWIPStock = process.WIP ?? false,
                    UnitCode = Constants.TPOUnitCode
                };

                var tpks = tpoTPKs.Where(m => m.ProcessGroup == processGroup).OrderBy(m => m.ProdGroup).ToList();
                if (!tpks.Any()) continue;

                //add target wpp
                var getTargetWPPInput = new GetTargetWPPInput()
                {
                    LocationCode = criteria.LocationCode,
                    BrandCode = criteria.BrandCode,
                    KPSYear = criteria.KPSYear.Value,
                    KPSWeek = criteria.KPSWeek.Value
                };

                viewModel.TargetWPP = _planningBLL.GetTargetWPP(getTargetWPPInput);

                //add wip stock
                if (process.WIP.HasValue && process.WIP.Value)
                {
                    var input = new GetPlantWIPStockInput()
                    {
                        LocationCode = criteria.LocationCode,
                        UnitCode = Constants.TPOUnitCode,
                        BrandCode = criteria.BrandCode,
                        ProcessGroup = processGroup,
                        KPSYear = criteria.KPSYear,
                        KPSWeek = criteria.KPSWeek
                    };

                    var wipStock = _planningBLL.GetPlantWIPStock(input);
                    if (wipStock != null)
                    {
                        viewModel.WIPPreviousValue = wipStock.WIPPreviousValue;
                        viewModel.WIPStock1 = wipStock.WIPStock1;
                        viewModel.WIPStock2 = wipStock.WIPStock2;
                        viewModel.WIPStock3 = wipStock.WIPStock3;
                        viewModel.WIPStock4 = wipStock.WIPStock4;
                        viewModel.WIPStock5 = wipStock.WIPStock5;
                        viewModel.WIPStock6 = wipStock.WIPStock6;
                        viewModel.WIPStock7 = wipStock.WIPStock7;
                    }
                }

                //add jk process
                var firstTpk = tpks.FirstOrDefault();
                if (firstTpk != null)
                {
                    viewModel.JKProcess1 = firstTpk.ProcessWorkHours1;
                    viewModel.JKProcess2 = firstTpk.ProcessWorkHours2;
                    viewModel.JKProcess3 = firstTpk.ProcessWorkHours3;
                    viewModel.JKProcess4 = firstTpk.ProcessWorkHours4;
                    viewModel.JKProcess5 = firstTpk.ProcessWorkHours5;
                    viewModel.JKProcess6 = firstTpk.ProcessWorkHours6;
                    viewModel.JKProcess7 = firstTpk.ProcessWorkHours7;
                }

                //add tpk list
                foreach (var tpkModel in tpks.Select(Mapper.Map<PlanTPOTPKModel>))
                {
                    if (criteria.FromPopup)
                    {
                        if (criteria.ExpelledGroup != null)
                        {
                            if (!criteria.ExpelledGroup.Contains(tpkModel.ProdGroup))
                            {
                                viewModel.PlanTPOTPK.Add(tpkModel);
                            }
                        }
                    }
                    else
                    {
                        viewModel.PlanTPOTPK.Add(tpkModel);
                    }
                    
                }

                viewModels.Add(viewModel);
            }



            // get total information per box
            var inputTPKInBox = new PlanTPOTPKTotalBoxInput()
            {
                LocationCode = criteria.LocationCode,
                BrandCode = criteria.BrandCode,
                KPSYear = criteria.KPSYear.Value,
                KPSWeek = criteria.KPSWeek.Value
            };


            var pageResult = new TPKPageResult<PlanTPOTPKViewModel, PlanTPOTPKTotalModel>
            {
                Results = viewModels,
                CustomResults = Mapper.Map<List<PlanTPOTPKTotalModel>>(GetTPKTotalResult(inputTPKInBox, viewModels))
            };


            //var result = new TPKPageResult()
            //             {
            //                 Results = viewModels,
            //                 ResultTotals = GetTPKTotalResult(inputTPKInBox, viewModels)
            //             };

            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetGroupList(string LocationCode, string BrandCode, string TPKCode, int? KPSYear, int? KPSWeek)
        {
            GetTPOTPKInput criteria = new GetTPOTPKInput();
            criteria.LocationCode = LocationCode;
            criteria.BrandCode = BrandCode;
            criteria.TPKCode = TPKCode;
            criteria.KPSYear = KPSYear;
            criteria.KPSWeek = KPSWeek;


            var viewModels = new List<PlanTPOTPKViewModel>();

            //get list of tpk;
            var tpoTPKs = _planningBLL.GetPlanningTPOTPK(criteria);

            //get list of process
            var listProcess = _masterDataBLL.GetAllActiveMasterProcess()
                .OrderBy(p => p.ProcessOrder)
                .ToList();

            foreach (var process in listProcess)
            {
                var processGroup = process.ProcessGroup;
                var viewModel = new PlanTPOTPKViewModel
                {
                    ProcessGroup = processGroup,
                    LocationCode = criteria.LocationCode,
                    BrandCode = criteria.BrandCode,
                    KPSYear = criteria.KPSYear,
                    KPSWeek = criteria.KPSWeek,
                    ShowInWIPStock = process.WIP ?? false,
                    UnitCode = Constants.TPOUnitCode
                };

                var tpks = tpoTPKs.Where(m => m.ProcessGroup == processGroup).OrderBy(m => m.ProdGroup).ToList();
                if (!tpks.Any()) continue;

                //add target wpp
                var getTargetWPPInput = new GetTargetWPPInput()
                {
                    LocationCode = criteria.LocationCode,
                    BrandCode = criteria.BrandCode,
                    KPSYear = criteria.KPSYear.Value,
                    KPSWeek = criteria.KPSWeek.Value
                };

                viewModel.TargetWPP = _planningBLL.GetTargetWPP(getTargetWPPInput);

                //add wip stock
                if (process.WIP.HasValue && process.WIP.Value)
                {
                    var input = new GetPlantWIPStockInput()
                    {
                        LocationCode = criteria.LocationCode,
                        UnitCode = Constants.TPOUnitCode,
                        BrandCode = criteria.BrandCode,
                        ProcessGroup = processGroup,
                        KPSYear = criteria.KPSYear,
                        KPSWeek = criteria.KPSWeek
                    };

                    var wipStock = _planningBLL.GetPlantWIPStock(input);
                    if (wipStock != null)
                    {
                        viewModel.WIPPreviousValue = wipStock.WIPPreviousValue;
                        viewModel.WIPStock1 = wipStock.WIPStock1;
                        viewModel.WIPStock2 = wipStock.WIPStock2;
                        viewModel.WIPStock3 = wipStock.WIPStock3;
                        viewModel.WIPStock4 = wipStock.WIPStock4;
                        viewModel.WIPStock5 = wipStock.WIPStock5;
                        viewModel.WIPStock6 = wipStock.WIPStock6;
                        viewModel.WIPStock7 = wipStock.WIPStock7;
                    }
                }

                //add jk process
                var firstTpk = tpks.FirstOrDefault();
                if (firstTpk != null)
                {
                    viewModel.JKProcess1 = firstTpk.ProcessWorkHours1;
                    viewModel.JKProcess2 = firstTpk.ProcessWorkHours2;
                    viewModel.JKProcess3 = firstTpk.ProcessWorkHours3;
                    viewModel.JKProcess4 = firstTpk.ProcessWorkHours4;
                    viewModel.JKProcess5 = firstTpk.ProcessWorkHours5;
                    viewModel.JKProcess6 = firstTpk.ProcessWorkHours6;
                    viewModel.JKProcess7 = firstTpk.ProcessWorkHours7;
                }

                //add tpk list
                foreach (var tpkModel in tpks.Select(Mapper.Map<PlanTPOTPKModel>))
                {
                    viewModel.PlanTPOTPK.Add(tpkModel);
                }

                viewModels.Add(viewModel);
            }

            //var listGroup = new List<String>();
            //foreach (var vm in viewModels)
            //{
            //    foreach (var tpotpk in vm.PlanTPOTPK)
            //    {
            //        listGroup.Add(tpotpk.ProdGroup);
            //    }
            //}

            //// get total information per box
            //var inputTPKInBox = new PlanTPOTPKTotalBoxInput()
            //{
            //    LocationCode = criteria.LocationCode,
            //    BrandCode = criteria.BrandCode,
            //    KPSYear = criteria.KPSYear.Value,
            //    KPSWeek = criteria.KPSWeek.Value
            //};


            //var pageResult = new TPKPageResult<PlanTPOTPKViewModel, PlanTPOTPKTotalModel>
            //{
            //    Results = viewModels,
            //    CustomResults = Mapper.Map<List<PlanTPOTPKTotalModel>>(GetTPKTotalResult(inputTPKInBox, viewModels))
            //};


            //var result = new TPKPageResult()
            //             {
            //                 Results = viewModels,
            //                 ResultTotals = GetTPKTotalResult(inputTPKInBox, viewModels)
            //             };

            return Json(viewModels, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves all plan tp ks.
        /// </summary>
        /// <param name="bulkData">The bulk data.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllTPOTPKs(CustomInsertUpdateData<PlanTPOTPKViewModel, PlanTPOTPKTotalModel> bulkData)
        {

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

                        var tpoTPKByProcess = Mapper.Map<TPOTPKByProcessDTO>(bulkData.Edit[i]);

                        //set updatedby
                        tpoTPKByProcess.UpdatedBy = GetUserName();
                        foreach (var tpk in tpoTPKByProcess.PlanTPOTPK)
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
                            var item = _planningBLL.SaveTPOTPKByGroup(tpoTPKByProcess);
                            bulkData.Edit[i] = Mapper.Map<PlanTPOTPKViewModel>(item);
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                            var transactionInput = new TransactionLogInput
                            {
                                page = "TPKTPO",
                                code_1 = "TPK",
                                code_2 = bulkData.Parameters.ContainsKey("LocationCode") ? bulkData.Parameters["LocationCode"] : "",
                                code_3 = bulkData.Parameters.ContainsKey("BrandCode") ? bulkData.Parameters["BrandCode"] : "",
                                code_4 = bulkData.Parameters.ContainsKey("KPSYear") ? bulkData.Parameters["KPSYear"] : "",
                                code_5 = bulkData.Parameters.ContainsKey("KPSWeek") ? bulkData.Parameters["KPSWeek"] : "",
                                ActionButton = "Save",
                                UserName = GetUserName(),
                                IDRole = CurrentUser.Responsibility.Role
                            };
                            _generalBLL.ExeTransactionLog(transactionInput);
                        }
                        catch (ExceptionBase ex)
                        {
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = ex.Message;
                            _vtlogger.Err(ex, new List<object> { bulkData.Edit[i] }, "SaveAllTPOTPKs");
                        }
                        catch (Exception ex)
                        {
                            bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                            bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            _vtlogger.Err(ex, new List<object> { bulkData.Edit[i] }, "SaveAllTPOTPKs");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { bulkData.Edit }, "Button save on edit TPOTPK");
                }
            }
            if (bulkData.Total != null)
            {
                try
                {
                    var total = bulkData.Total.First(t => t.TotalType.ToUpper() == Enums.Conversion.Box.ToString().ToUpper());
                    var index = bulkData.Total.Count - 1;
                    if (total == null) return Json(bulkData);
                    var totaldto = Mapper.Map<PlanTPOTPKTotalBoxDTO>(total);
                    totaldto.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _planningBLL.SaveTPOTPKTotal(totaldto);
                        bulkData.Total[index] = Mapper.Map<PlanTPOTPKTotalModel>(item);
                        bulkData.Total[index].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Total[index].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Total[index].Message = ex.Message;
                        _vtlogger.Err(ex, new List<object> { bulkData.Total[index] }, "SaveTotalTPOTPKs");
                    }
                    catch (Exception ex)
                    {
                        bulkData.Total[index].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Total[index].Message = ex.Message;
                        _vtlogger.Err(ex, new List<object> { bulkData.Total[index] }, "SaveTotalTPOTPKs");
                    }
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { bulkData.Total }, "Button save on total TPOTPK");
                }
            }

            // Save Header Work Hour Box            
            var boxWorkHours = new GenericValuePerWeekDTO<float>();
            boxWorkHours.Value1 = bulkData.Parameters.ContainsKey("HeaderProcessWorkHour1") ? float.Parse(bulkData.Parameters["HeaderProcessWorkHour1"]) : 0f;
            boxWorkHours.Value2 = bulkData.Parameters.ContainsKey("HeaderProcessWorkHour2") ? float.Parse(bulkData.Parameters["HeaderProcessWorkHour2"]) : 0f;
            boxWorkHours.Value3 = bulkData.Parameters.ContainsKey("HeaderProcessWorkHour3") ? float.Parse(bulkData.Parameters["HeaderProcessWorkHour3"]) : 0f;
            boxWorkHours.Value4 = bulkData.Parameters.ContainsKey("HeaderProcessWorkHour4") ? float.Parse(bulkData.Parameters["HeaderProcessWorkHour4"]) : 0f;
            boxWorkHours.Value5 = bulkData.Parameters.ContainsKey("HeaderProcessWorkHour5") ? float.Parse(bulkData.Parameters["HeaderProcessWorkHour5"]) : 0f;
            boxWorkHours.Value6 = bulkData.Parameters.ContainsKey("HeaderProcessWorkHour6") ? float.Parse(bulkData.Parameters["HeaderProcessWorkHour6"]) : 0f;
            boxWorkHours.Value7 = bulkData.Parameters.ContainsKey("HeaderProcessWorkHour7") ? float.Parse(bulkData.Parameters["HeaderProcessWorkHour7"]) : 0f;             

            var boxInput = new PlanTPOTPKTotalBoxInput(){
                LocationCode = bulkData.Parameters.ContainsKey("LocationCode") ? bulkData.Parameters["LocationCode"] : "",
                BrandCode = bulkData.Parameters.ContainsKey("BrandCode") ? bulkData.Parameters["BrandCode"] : "",
                KPSYear = bulkData.Parameters.ContainsKey("KPSYear") ? Convert.ToInt32(bulkData.Parameters["KPSYear"]) : 0,
                KPSWeek = bulkData.Parameters.ContainsKey("KPSWeek") ? Convert.ToInt32(bulkData.Parameters["KPSWeek"]) : 0,
            };
            _planningBLL.SaveTPOTPKWorkHourInBox(boxInput, boxWorkHours);

            return Json(bulkData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CalculateTPOTPK(CustomInsertUpdateData<PlanTPOTPKViewModel, PlanTPOTPKTotalModel> bulkData)
        {
            var inputTPOTPK = new CalculateTPOTPKInput()
            {
                KPSYear = Convert.ToInt16(bulkData.Parameters["KPSYear"]),
                KPSWeek = Convert.ToInt16(bulkData.Parameters["KPSWeek"]),
                LocationCode = bulkData.Parameters["LocationCode"],
                BrandCode = bulkData.Parameters["BrandCode"],
                ListTPOTPK = Mapper.Map<List<TPOTPKByProcessDTO>>(bulkData.Edit),
                TotalTPOTPK = Mapper.Map<List<PlanTPOTPKTotalBoxDTO>>(bulkData.Total),
                HeaderProcessWorkHours = new List<float>() { 
                    float.Parse(bulkData.Parameters["HeaderProcessWorkHour1"], CultureInfo.CurrentCulture), 
                    float.Parse(bulkData.Parameters["HeaderProcessWorkHour2"], CultureInfo.CurrentCulture),
                    float.Parse(bulkData.Parameters["HeaderProcessWorkHour3"], CultureInfo.CurrentCulture),
                    float.Parse(bulkData.Parameters["HeaderProcessWorkHour4"], CultureInfo.CurrentCulture),
                    float.Parse(bulkData.Parameters["HeaderProcessWorkHour5"], CultureInfo.CurrentCulture),
                    float.Parse(bulkData.Parameters["HeaderProcessWorkHour6"], CultureInfo.CurrentCulture),
                    float.Parse(bulkData.Parameters["HeaderProcessWorkHour7"], CultureInfo.CurrentCulture)
                },
                FilterCurrentDayForward = DateTime.ParseExact(bulkData.Parameters["FilterCurrentDayForward"], Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture),
                IsFilterCurrentDayForward = Convert.ToBoolean(bulkData.Parameters["IsFilterCurrentDayForward"]),
                IsWhChanged = Convert.ToBoolean(bulkData.Parameters["IsWhChanged"])
            };

            var result = _planningBLL.CalculateTPOTPK(inputTPOTPK);

            var viewModels = Mapper.Map<List<PlanTPOTPKViewModel>>(result.TPOTPKByProcess);              
            foreach (var planTpotpkViewModel in viewModels)
            {
                try
                {
                    planTpotpkViewModel.JKProcess1 = planTpotpkViewModel.PlanTPOTPK.Max(jk1 => jk1.ProcessWorkHours1);
                    planTpotpkViewModel.JKProcess2 = planTpotpkViewModel.PlanTPOTPK.Max(jk2 => jk2.ProcessWorkHours2);
                    planTpotpkViewModel.JKProcess3 = planTpotpkViewModel.PlanTPOTPK.Max(jk3 => jk3.ProcessWorkHours3);
                    planTpotpkViewModel.JKProcess4 = planTpotpkViewModel.PlanTPOTPK.Max(jk4 => jk4.ProcessWorkHours4);
                    planTpotpkViewModel.JKProcess5 = planTpotpkViewModel.PlanTPOTPK.Max(jk5 => jk5.ProcessWorkHours5);
                    planTpotpkViewModel.JKProcess6 = planTpotpkViewModel.PlanTPOTPK.Max(jk6 => jk6.ProcessWorkHours6);
                    planTpotpkViewModel.JKProcess7 = planTpotpkViewModel.PlanTPOTPK.Max(jk7 => jk7.ProcessWorkHours7);

                    var WIPProcess = _masterDataBLL.GetMasterProcesses(new GetMstGenProcessesInput() { ProcessName = planTpotpkViewModel.ProcessGroup, WIP = true, StatusActive = true });
                    if (WIPProcess.Count > 0)
                    {
                        planTpotpkViewModel.ShowInWIPStock = true;
                        var input = new GetPlantWIPStockInput()
                        {
                            LocationCode = inputTPOTPK.LocationCode,
                            UnitCode = Constants.TPOUnitCode,
                            BrandCode = inputTPOTPK.BrandCode,
                            ProcessGroup = planTpotpkViewModel.ProcessGroup,
                            KPSYear = inputTPOTPK.KPSYear,
                            KPSWeek = inputTPOTPK.KPSWeek
                        };
                        var wipStock = _planningBLL.GetPlantWIPStock(input);
                        if (wipStock != null)
                        {
                            planTpotpkViewModel.WIPPreviousValue = wipStock.WIPPreviousValue;
                            planTpotpkViewModel.WIPStock1 = wipStock.WIPStock1;
                            planTpotpkViewModel.WIPStock2 = wipStock.WIPStock2;
                            planTpotpkViewModel.WIPStock3 = wipStock.WIPStock3;
                            planTpotpkViewModel.WIPStock4 = wipStock.WIPStock4;
                            planTpotpkViewModel.WIPStock5 = wipStock.WIPStock5;
                            planTpotpkViewModel.WIPStock6 = wipStock.WIPStock6;
                            planTpotpkViewModel.WIPStock7 = wipStock.WIPStock7;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { planTpotpkViewModel }, "Calculate TPOTPK");
                }
            }

            var pageResult = new TPKPageResult<PlanTPOTPKViewModel, PlanTPOTPKTotalModel>
            {
                Results = viewModels,
                CustomResults = Mapper.Map<List<PlanTPOTPKTotalModel>>(result.TPOTPTotals)
            };            


            //var pageResult = new TPKPageResult<PlanTPOTPKTotalModel>
            //{
            //    Results = viewModels,
            //    CustomModel = new CustomResult<PlanTPOTPKTotalModel>
            //                       {
            //                           CustomList = Mapper.Map<List<PlanTPOTPKTotalModel>>(result.TPOTPTotals)
            //                       }
            //};

            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetWorkHourHeaders(string locationCode, string brandCode, int year, int week)
        {
            var tpkInBox = _planningBLL.GetTPOTPKInBox(new PlanTPOTPKTotalBoxInput() { LocationCode = locationCode, BrandCode = brandCode, KPSYear = year, KPSWeek = week });
            return Json(tpkInBox, JsonRequestBehavior.AllowGet);
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
            var brandCodes = _planningBLL.GetBrandCodeByLocationYearAndWeek(locationCode, KPSYear, KPSWeek);
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
            var kpsWeek = _masterDataBLL.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckSubmittedWpp(string data)
        {
            var ResultCheck = _utilitiesBLL.getSubmittedWpp(data);
            return Json(new { readyToSubmit = ResultCheck[0], reSubmit = ResultCheck[1] }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ValidateSubmittedEntry(string data)
        {
            var isAlreadySubmitted = _utilitiesBLL.CheckAllDataAlreadySubmit(data, Enums.PageName.TPOProductionEntry.ToString());
            var message = new BLLException(ExceptionCodes.BLLExceptions.SubmittedTPOEntry);
            return Json(new { isAlreadySubmitted = isAlreadySubmitted, notification = message }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Gets the brand group by location code.
        /// </summary>
        /// <param name="criteria">The location code.</param>
        /// <returns></returns>
        //[HttpGet]
        //public JsonResult GetTPKCodeByLocationCode(GetTPOTPKInput criteria)
        //{
        //    var tpkCodes = _planningBLL.GetTPKCodeByLocations(criteria);
        //    return Json(tpkCodes, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Gets the production start date.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        public JsonResult GetProductionStartDate(int year, int week, string locationCode, string brandCode)
        {
            var startDate = string.Empty;
            var startEnableDate = string.Empty;
            var kpsWeek = _masterDataBLL.GetWeekByYearAndWeek(year, week);
            if (kpsWeek != null && kpsWeek.StartDate.HasValue)
            {
                startDate = kpsWeek.StartDate.Value.ToString(Constants.DefaultDateOnlyFormat);
            }
            var code = "TPK" + "/" + locationCode + "/" +brandCode+ "/" +year.ToString()+ "/" + week.ToString();
            var alreadySubmit  = false;

            var log = _utilitiesBLL.GetTransactionLogByTransCodeAndIDFlow(code, (int)Enums.IdFlow.TPOTPKSubmit);

            if(log != null){
                alreadySubmit = true;
                startEnableDate = log.UpdatedDate.ToString(Constants.DefaultDateOnlyFormat);
            }

            return Json(new { startDate = startDate, alreadySubmit = alreadySubmit, startEnableDate = startEnableDate }, JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// Generates the excel.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="brandCode">The brand code.</param>
        /// <param name="tpkCode">The shift.</param>
        /// <param name="year">The year.</param>
        /// <param name="week">The week.</param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string brandCode, string tpkCode, int year, int week)
        {
            var PlanningTPOTPKs = PrepareDataForExcel(locationCode, brandCode, year, week);
            var input = new GetTPOTPKInput()
            {
                LocationCode = locationCode,
                BrandCode = brandCode,
                KPSYear = year,
                KPSWeek = week
            };

            var inputTPKInBox = new PlanTPOTPKTotalBoxInput()
            {
                LocationCode = locationCode,
                BrandCode = brandCode,
                KPSYear = year,
                KPSWeek = week
            };

            var tpoTPKs = _planningBLL.GetPlanningTPOTPK(input);
            var weeks = _masterDataBLL.GetDateByWeek(year, week);

            var tpkInBox = _planningBLL.GetTPOTPKInBox(new PlanTPOTPKTotalBoxInput() { LocationCode = locationCode, BrandCode = brandCode, KPSYear = year, KPSWeek = week });

            var locationInfo = _masterDataBLL.GetLocation(locationCode);
            ////prepare data for excel
            //var productionStartDate = GetProductionStartDate(planTPUs.FirstOrDefault());
            var maxHoursMon = tpkInBox.ProcessWorkHours1;
            var maxHoursTue = tpkInBox.ProcessWorkHours2;
            var maxHoursWed = tpkInBox.ProcessWorkHours3;
            var maxHoursThu = tpkInBox.ProcessWorkHours4;
            var maxHoursFri = tpkInBox.ProcessWorkHours5;
            var maxHoursSat = tpkInBox.ProcessWorkHours6;
            var maxHoursSun = tpkInBox.ProcessWorkHours7;

            var totalMaxHours = maxHoursMon + maxHoursTue + maxHoursWed + maxHoursThu + maxHoursFri + maxHoursSat +
                                maxHoursSun;

            var totalProcess = PlanningTPOTPKs.Sum(m => m.TotalProdGroup);

            var attendance1 = PlanningTPOTPKs.Sum(m => m.PercentAttendance1) / totalProcess;
            var attendance2 = PlanningTPOTPKs.Sum(m => m.PercentAttendance2) / totalProcess;
            var attendance3 = PlanningTPOTPKs.Sum(m => m.PercentAttendance3) / totalProcess;
            var attendance4 = PlanningTPOTPKs.Sum(m => m.PercentAttendance4) / totalProcess;
            var attendance5 = PlanningTPOTPKs.Sum(m => m.PercentAttendance5) / totalProcess;
            var attendance6 = PlanningTPOTPKs.Sum(m => m.PercentAttendance6) / totalProcess;
            var attendance7 = PlanningTPOTPKs.Sum(m => m.PercentAttendance7) / totalProcess;

            var stick = tpoTPKs.Where(m => String.Equals(m.ProcessGroup, Enums.Process.Rolling.ToString(), StringComparison.CurrentCultureIgnoreCase));
            var brandForStick = _masterDataBLL.GetBrand(new GetBrandInput { BrandCode = brandCode });
            var box = _planningBLL.GetTPOTPKInBox(inputTPKInBox);

            var totalStickSystem1 = box != null ? box.TargetSystem1 * brandForStick.StickPerBox : 0;
            var totalStickManual1 = box != null ? box.TargetManual1 * brandForStick.StickPerBox : 0;
            var totalStickSystem2 = box != null ? box.TargetSystem2 * brandForStick.StickPerBox : 0;
            var totalStickManual2 = box != null ? box.TargetManual2 * brandForStick.StickPerBox : 0;
            var totalStickSystem3 = box != null ? box.TargetSystem3 * brandForStick.StickPerBox : 0;
            var totalStickManual3 = box != null ? box.TargetManual3 * brandForStick.StickPerBox : 0;
            var totalStickSystem4 = box != null ? box.TargetSystem4 * brandForStick.StickPerBox : 0;
            var totalStickManual4 = box != null ? box.TargetManual4 * brandForStick.StickPerBox : 0;
            var totalStickSystem5 = box != null ? box.TargetSystem5 * brandForStick.StickPerBox : 0;
            var totalStickManual5 = box != null ? box.TargetManual5 * brandForStick.StickPerBox : 0;
            var totalStickSystem6 = box != null ? box.TargetSystem6 * brandForStick.StickPerBox : 0;
            var totalStickManual6 = box != null ? box.TargetManual6 * brandForStick.StickPerBox : 0;
            var totalStickSystem7 = box != null ? box.TargetSystem7 * brandForStick.StickPerBox : 0;
            var totalStickManual7 = box != null ? box.TargetManual7 * brandForStick.StickPerBox : 0;

            var totalStickManual = totalStickManual1 + totalStickManual2 + totalStickManual3 + totalStickManual4 + totalStickManual5 + totalStickManual6 + totalStickManual7;
            var totalStickSystem = totalStickSystem1 + totalStickSystem2 + totalStickSystem3 + totalStickSystem4 + totalStickSystem5 + totalStickSystem6 + totalStickSystem7;

            var totalBoxSystem1 = box != null ? box.TargetSystem1 : 0;
            var totalBoxManual1 = box != null ? box.TargetManual1 : 0;
            var totalBoxSystem2 = box != null ? box.TargetSystem2 : 0;
            var totalBoxManual2 = box != null ? box.TargetManual2 : 0;
            var totalBoxSystem3 = box != null ? box.TargetSystem3 : 0;
            var totalBoxManual3 = box != null ? box.TargetManual3 : 0;
            var totalBoxSystem4 = box != null ? box.TargetSystem4 : 0;
            var totalBoxManual4 = box != null ? box.TargetManual4 : 0;
            var totalBoxSystem5 = box != null ? box.TargetSystem5 : 0;
            var totalBoxManual5 = box != null ? box.TargetManual5 : 0;
            var totalBoxSystem6 = box != null ? box.TargetSystem6 : 0;
            var totalBoxManual6 = box != null ? box.TargetManual6 : 0;
            var totalBoxSystem7 = box != null ? box.TargetSystem7 : 0;
            var totalBoxManual7 = box != null ? box.TargetManual7 : 0;

            var totalBoxManual = totalBoxManual1 + totalBoxManual2 + totalBoxManual3 + totalBoxManual4 + totalBoxManual5 + totalBoxManual6 + totalBoxManual7;
            var totalBoxSystem = totalBoxSystem1 + totalBoxSystem2 + totalBoxSystem3 + totalBoxSystem4 + totalBoxSystem5 + totalBoxSystem6 + totalBoxSystem7;

            var TotalPercent = (attendance1 + attendance2 + attendance3 + attendance4 + attendance5 + attendance6 + attendance7)/7;

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.PlanningExcelTemplate.PlanningTPOTPK + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 3, locationCode.ToString());
                slDoc.SetCellValue(2, 5, locationInfo.LocationName);
                slDoc.SetCellValue(3, 3, brandCode.ToString());
                slDoc.SetCellValue(4, 3, tpkCode.ToString());
                slDoc.SetCellValue(5, 3, year.ToString());
                slDoc.SetCellValue(6, 3, week.ToString());


                slDoc.SetCellValue(11, 6, maxHoursMon.ToString());
                slDoc.SetCellValue(11, 10, maxHoursTue.ToString());
                slDoc.SetCellValue(11, 14, maxHoursWed.ToString());
                slDoc.SetCellValue(11, 18, maxHoursThu.ToString());
                slDoc.SetCellValue(11, 22, maxHoursFri.ToString());
                slDoc.SetCellValue(11, 26, maxHoursSat.ToString());
                slDoc.SetCellValue(11, 30, maxHoursSun.ToString());
                slDoc.SetCellValue(11, 34, totalMaxHours.ToString());

                slDoc.SetCellValue(12, 6, String.Format("{0:0.00} %", attendance1));
                slDoc.SetCellValue(12, 10, String.Format("{0:0.00} %", attendance2));
                slDoc.SetCellValue(12, 14, String.Format("{0:0.00} %", attendance3));
                slDoc.SetCellValue(12, 18, String.Format("{0:0.00} %", attendance4));
                slDoc.SetCellValue(12, 22, String.Format("{0:0.00} %", attendance5));
                slDoc.SetCellValue(12, 26, String.Format("{0:0.00} %", attendance6));
                slDoc.SetCellValue(12, 30, String.Format("{0:0.00} %", attendance7));

                slDoc.SetCellValue(12, 34, String.Format("{0:0.00} %", TotalPercent));

                var dateColumn = 6;
                foreach (var date in weeks)
                {
                    slDoc.SetCellValue(10, dateColumn, date.ToString(Constants.DefaultDateOnlyFormat));
                    dateColumn = dateColumn + 4;
                }

                //    //row values
                var iRow = 15;

                foreach (var tpo in PlanningTPOTPKs)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (tpo.ProcessGroup == "Total")
                    {
                        style.Font.Bold = true;
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, tpo.ProcessGroup);
                    slDoc.SetCellValue(iRow, 2, tpo.ProdGroup);
                    slDoc.SetCellValue(iRow, 3, tpo.WorkerRegister.ToString());
                    slDoc.SetCellValue(iRow, 4, tpo.WorkerAvailable.ToString());
                    slDoc.SetCellValue(iRow, 5, tpo.WorkerAlocation.ToString());

                    //monday
                    slDoc.SetCellValue(iRow, 6, tpo.HistoricalCapacityGroup1.ToString());
                    slDoc.SetCellValue(iRow, 7, tpo.WIP1.ToString());
                    slDoc.SetCellValue(iRow, 8, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetSystem1, 0, MidpointRounding.AwayFromZero)));
                    slDoc.SetCellValue(iRow, 9, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetManual1, 0, MidpointRounding.AwayFromZero)));
                    //tuesday
                    slDoc.SetCellValue(iRow, 10, tpo.HistoricalCapacityGroup2.ToString());
                    slDoc.SetCellValue(iRow, 11, tpo.WIP2.ToString());
                    slDoc.SetCellValue(iRow, 12, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetSystem2, 0, MidpointRounding.AwayFromZero)));
                    slDoc.SetCellValue(iRow, 13, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetManual2, 0, MidpointRounding.AwayFromZero)));
                    //wednesday
                    slDoc.SetCellValue(iRow, 14, tpo.HistoricalCapacityGroup3.ToString());
                    slDoc.SetCellValue(iRow, 15, tpo.WIP3.ToString());
                    slDoc.SetCellValue(iRow, 16, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetSystem3, 0, MidpointRounding.AwayFromZero)));
                    slDoc.SetCellValue(iRow, 17, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetManual3, 0, MidpointRounding.AwayFromZero)));
                    //thursday
                    slDoc.SetCellValue(iRow, 18, tpo.HistoricalCapacityGroup4.ToString());
                    slDoc.SetCellValue(iRow, 19, tpo.WIP4.ToString());
                    slDoc.SetCellValue(iRow, 20, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetSystem4, 0, MidpointRounding.AwayFromZero)));
                    slDoc.SetCellValue(iRow, 21, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetManual4, 0, MidpointRounding.AwayFromZero)));
                    //friday
                    slDoc.SetCellValue(iRow, 22, tpo.HistoricalCapacityGroup5.ToString());
                    slDoc.SetCellValue(iRow, 23, tpo.WIP5.ToString());
                    slDoc.SetCellValue(iRow, 24, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetSystem5, 0, MidpointRounding.AwayFromZero)));
                    slDoc.SetCellValue(iRow, 25, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetManual5, 0, MidpointRounding.AwayFromZero)));
                    //saturday
                    slDoc.SetCellValue(iRow, 26, tpo.HistoricalCapacityGroup6.ToString());
                    slDoc.SetCellValue(iRow, 27, tpo.WIP6.ToString());
                    slDoc.SetCellValue(iRow, 28, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetSystem6, 0, MidpointRounding.AwayFromZero)));
                    slDoc.SetCellValue(iRow, 29, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetManual6, 0, MidpointRounding.AwayFromZero)));
                    //sunday
                    slDoc.SetCellValue(iRow, 30, tpo.HistoricalCapacityGroup7.ToString());
                    slDoc.SetCellValue(iRow, 31, tpo.WIP7.ToString());
                    slDoc.SetCellValue(iRow, 32, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetSystem7, 0, MidpointRounding.AwayFromZero)));
                    slDoc.SetCellValue(iRow, 33, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TargetManual7, 0, MidpointRounding.AwayFromZero)));

                    //total target system
                    slDoc.SetCellValue(iRow, 34, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TotalTargetSystem, 0, MidpointRounding.AwayFromZero)));
                    //total target manual
                    slDoc.SetCellValue(iRow, 35, String.Format(CultureInfo.CurrentCulture, "{0:n0}", Math.Round((decimal)tpo.TotalTargetManual, 0, MidpointRounding.AwayFromZero)));


                    slDoc.SetCellStyle(iRow, 1, iRow, 35, style);
                    iRow++;
                }
                //
                SLStyle styles = slDoc.CreateStyle();
                styles.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                styles.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                styles.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                styles.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                styles.Font.FontName = "Calibri";
                styles.Font.FontSize = 10;
                styles.Font.Bold = true;
                styles.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                slDoc.SetCellValue(iRow, 1, "Total Stick");
                slDoc.SetCellValue(iRow, 8, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickSystem1));
                slDoc.SetCellValue(iRow, 9, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickManual1));
                slDoc.SetCellValue(iRow, 12, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickSystem2));
                slDoc.SetCellValue(iRow, 13, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickManual2));
                slDoc.SetCellValue(iRow, 16, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickSystem3));
                slDoc.SetCellValue(iRow, 17, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickManual3));
                slDoc.SetCellValue(iRow, 20, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickSystem4));
                slDoc.SetCellValue(iRow, 21, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickManual4));
                slDoc.SetCellValue(iRow, 24, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickSystem5));
                slDoc.SetCellValue(iRow, 25, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickManual5));
                slDoc.SetCellValue(iRow, 28, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickSystem6));
                slDoc.SetCellValue(iRow, 29, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickManual6));
                slDoc.SetCellValue(iRow, 32, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickSystem7));
                slDoc.SetCellValue(iRow, 33, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickManual7));
                slDoc.SetCellValue(iRow, 34, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickSystem));
                slDoc.SetCellValue(iRow, 35, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalStickManual));
                slDoc.SetCellValue(iRow + 1, 1, "Total Box");
                slDoc.SetCellValue(iRow + 1, 8, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxSystem1));
                slDoc.SetCellValue(iRow + 1, 9, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxManual1));
                slDoc.SetCellValue(iRow + 1, 12, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxSystem2));
                slDoc.SetCellValue(iRow + 1, 13, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxManual2));
                slDoc.SetCellValue(iRow + 1, 16, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxSystem3));
                slDoc.SetCellValue(iRow + 1, 17, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxManual3));
                slDoc.SetCellValue(iRow + 1, 20, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxSystem4));
                slDoc.SetCellValue(iRow + 1, 21, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxManual4));
                slDoc.SetCellValue(iRow + 1, 24, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxSystem5));
                slDoc.SetCellValue(iRow + 1, 25, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxManual5));
                slDoc.SetCellValue(iRow + 1, 28, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxSystem6));
                slDoc.SetCellValue(iRow + 1, 29, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxManual6));
                slDoc.SetCellValue(iRow + 1, 32, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxSystem7));
                slDoc.SetCellValue(iRow + 1, 33, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxManual7));
                slDoc.SetCellValue(iRow + 1, 34, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxSystem));
                slDoc.SetCellValue(iRow + 1, 35, String.Format(CultureInfo.CurrentCulture, "{0:n0}", totalBoxManual));
                slDoc.SetCellStyle(iRow, 1, iRow + 1, 35, styles);

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
                //slDoc.AutoFitColumn(1, 35);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "ProductionPlanning_TPOTargetProductionKelompok_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private List<PlantTPOTPKExcelModel> PrepareDataForExcel(string locationCode, string brandCode, int? kpsYear, int? kpsWeek)
        {
            var excelModels = new List<PlantTPOTPKExcelModel>();

            //get list of tpk;
            var getTpotpkInput = new GetTPOTPKInput();
            getTpotpkInput.LocationCode = locationCode;
            getTpotpkInput.BrandCode = brandCode;
            getTpotpkInput.KPSYear = kpsYear;
            getTpotpkInput.KPSWeek = kpsWeek;


            var tpotpk = _planningBLL.GetPlanningTPOTPK(getTpotpkInput);

            //get list og process
            var listProcess = _masterDataBLL.GetAllActiveMasterProcess()
                .OrderBy(p => p.ProcessOrder)
                .ToList(); ;

            foreach (var process in listProcess)
            {
                var processGroup = process.ProcessGroup;

                var tpks = tpotpk.Where(m => m.ProcessGroup == processGroup).OrderBy(m => m.ProdGroup).ToList();
                if (!tpks.Any()) continue;

                //add tpk list
                for (var i = 0; i <= tpks.Count; i++)
                {
                    var tpkExcelModel = new PlantTPOTPKExcelModel();
                    if (i < tpks.Count)
                    {
                        var plantTpk = tpks[i];
                        tpkExcelModel = Mapper.Map<PlantTPOTPKExcelModel>(plantTpk);

                        if (i != 0)
                            tpkExcelModel.ProcessGroup = String.Empty;
                    }
                    else
                    {
                        tpkExcelModel.ProcessGroup = "Total";
                        //tpkExcelModel.ProcessGroup = tpks.Count.ToString();
                        tpkExcelModel.ProdGroup = tpks.Count.ToString();
                        tpkExcelModel.TotalProdGroup = tpks.Count;
                        tpkExcelModel.WorkerRegister = tpks.Sum(m => m.WorkerRegister);
                        tpkExcelModel.WorkerAvailable = tpks.Sum(m => m.WorkerAvailable);
                        tpkExcelModel.WorkerAlocation = tpks.Sum(m => m.WorkerAlocation);
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
                        tpkExcelModel.TotalTargetSystem = tpks.Sum(m => m.TotalTargetSystem);
                        tpkExcelModel.TotalTargetManual = tpks.Sum(m => m.TotalTargetManual);
                    }

                    excelModels.Add(tpkExcelModel);
                }
            }

            return excelModels;
        }

        [HttpPost]
        public ActionResult SubmitData(int year, int week, string locationCode, string brandCode)
        {
            var resultJSonSubmitData = "";
            var resultJSonSendEmail = "";
            var listResultJSon = new List<string>();

            try
            {

                _ssisPackageService.RunSSISProductionEntryTPO(GetUserName(), year, week, locationCode, brandCode);
                var transactionInput = new TransactionLogInput
                {
                    page = "TPKTPO",
                    code_1 = "TPK",
                    code_2 = locationCode,
                    code_3 = brandCode,
                    code_4 = year.ToString(),
                    code_5 = week.ToString(),
                    ActionButton = "Submit",
                    UserName = GetUserName(),
                    IDRole = CurrentUser.Responsibility.Role
                };
                _generalBLL.ExeTransactionLog(transactionInput);
                _planningBLL.SubmitTpoTpk(locationCode, brandCode, year, week, GetUserName());
                //return Json(string.Format("Nama {0}, Year {1}, Week {2}, LocationCode {3}, BrandCode {4}", GetUserName(), year, week, locationCode, brandCode));
                resultJSonSubmitData = "Run submit data on background process.";
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = "Failed to run submit data on background process.";
                listResultJSon.Add(resultJSonSubmitData);
                _vtlogger.Err(ex, new List<object> { listResultJSon }, "Failed to run submit data on background process");
                return Json(listResultJSon);
            }

            try
            {
                _planningBLL.SendEmailSubmitTPOTPK(locationCode, brandCode, year, week, GetUserName());
            }
            catch (Exception ex)
            {
                resultJSonSendEmail = "Failed to send email.";

                listResultJSon.Add(resultJSonSubmitData);
                listResultJSon.Add(resultJSonSendEmail);

                _vtlogger.Err(ex, new List<object> { listResultJSon }, "Failed send email when submit data on background process");

                return Json(listResultJSon);
            }

            listResultJSon.Add(resultJSonSubmitData);
            return Json(listResultJSon);
        }

        private List<PlanTPOTPKTotalModel> GetTPKTotalResult(PlanTPOTPKTotalBoxInput inputTPKInBox, List<PlanTPOTPKViewModel> TPKList)
        {
            var totalViewModel = new List<PlanTPOTPKTotalModel>();

            //get total in box
            var tpkInBox = _planningBLL.GetTPOTPKInBox(inputTPKInBox);
            if (tpkInBox != null)
            {
                //get total instick
                var brandForStick = _masterDataBLL.GetBrand(new GetBrandInput { BrandCode = inputTPKInBox.BrandCode });

                var tpkInStick = new PlanTPOTPKTotalModel
                {
                    LocationCode = inputTPKInBox.LocationCode,
                    BrandCode = inputTPKInBox.BrandCode,
                    KPSYear = inputTPKInBox.KPSYear,
                    KPSWeek = inputTPKInBox.KPSWeek,
                    TotalType = "STICK",
                    TargetSystem1 = tpkInBox.TargetSystem1 * brandForStick.StickPerBox,
                    TargetSystem2 = tpkInBox.TargetSystem2 * brandForStick.StickPerBox,
                    TargetSystem3 = tpkInBox.TargetSystem3 * brandForStick.StickPerBox,
                    TargetSystem4 = tpkInBox.TargetSystem4 * brandForStick.StickPerBox,
                    TargetSystem5 = tpkInBox.TargetSystem5 * brandForStick.StickPerBox,
                    TargetSystem6 = tpkInBox.TargetSystem6 * brandForStick.StickPerBox,
                    TargetSystem7 = tpkInBox.TargetSystem7 * brandForStick.StickPerBox,
                    TargetManual1 = tpkInBox.TargetManual1 * brandForStick.StickPerBox,
                    TargetManual2 = tpkInBox.TargetManual2 * brandForStick.StickPerBox,
                    TargetManual3 = tpkInBox.TargetManual3 * brandForStick.StickPerBox,
                    TargetManual4 = tpkInBox.TargetManual4 * brandForStick.StickPerBox,
                    TargetManual5 = tpkInBox.TargetManual5 * brandForStick.StickPerBox,
                    TargetManual6 = tpkInBox.TargetManual6 * brandForStick.StickPerBox,
                    TargetManual7 = tpkInBox.TargetManual7 * brandForStick.StickPerBox,
                    TotalTargetSystem = (tpkInBox.TargetSystem1 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem2 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem3 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem4 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem5 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem6 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetSystem7 * brandForStick.StickPerBox),
                    TotalTargetManual = (tpkInBox.TargetManual1 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual2 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual3 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual4 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual5 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual6 * brandForStick.StickPerBox) +
                                        (tpkInBox.TargetManual7 * brandForStick.StickPerBox)
                };
                totalViewModel.Add(tpkInStick);

                var resultInBox = Mapper.Map<PlanTPOTPKTotalModel>(tpkInBox);
                resultInBox.TotalType = "BOX";
                totalViewModel.Add(resultInBox);
            }

            return totalViewModel;
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