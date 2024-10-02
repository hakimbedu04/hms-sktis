using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.ExeReportByProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using SKTISWebsite.Models.ExePlantProductionEntryVerification;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ExeTPOProductionEntryVerification;
using System.Globalization;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using WebGrease;

namespace SKTISWebsite.Controllers
{
    public class GeneratorReportByProccessController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IExecutionPlantBLL _executionPlantBll;
        private IExecutionTPOBLL _executionTpobll;
        private IExecutionOtherBLL _executionOtherBll;
        private IExeReportBLL _executionReportBll;
        private IVTLogger _vtlogger;

        public GeneratorReportByProccessController
        (
            IApplicationService applicationService, 
            IMasterDataBLL masterDataBll,
            IExecutionPlantBLL executionPlantBll,
            IExecutionTPOBLL executionTpobll,
            IExecutionOtherBLL executionOtherBll,
            IExeReportBLL executionReportBll,
            IVTLogger vtlogger
        )
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _executionPlantBll = executionPlantBll;
            _executionTpobll = executionTpobll;
            _executionOtherBll = executionOtherBll;
            _executionReportBll = executionReportBll;
            //SetPage("GeneratorReportByProccess");
            _vtlogger = vtlogger;
            SetPage("Homepage");
        }
        //
        // GET: /GeneratorReportByProccess/
        public ActionResult Index()
        {
            var init = new InitExeReportByProcessViewModel()
            {
                PLNTChildLocationLookupList = _svc.GetLastLocationChildList(Enums.LocationCode.SKT.ToString())
            };
            return View("index",init);
        }

        public JsonResult GetData(GetExeReportByProcessInput input)
        {
            var check = _svc.CheckLocationByLocation(input.LocationCode);
            if (check == Enums.LocationCode.PLNT.ToString())
            {
                var criteria = new GetExePlantProductionEntryVerificationInput() {
                    LocationCode = input.LocationCode,
                    UnitCode = input.UnitCode,
                    ProductionDate = input.DateFrom,
                    BrandCode = input.BrandCode,
                    PageSize = input.PageSize
                };
                var masterLists = _executionPlantBll.GetExePlantProductionEntryVerificationViews(criteria);
                var viewModel = Mapper.Map<List<ExePlantProductionEntryVerificationViewViewModel>>(masterLists);
                var pageResult = new PageResult<ExePlantProductionEntryVerificationViewViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
            else{
                var criteria = new GetExeTPOProductionEntryVerificationInput() { 
                    LocationCode = input.LocationCode,
                    BrandCode = input.BrandCode,
                    ProductionDate = input.DateFrom,
                    PageSize = input.PageSize
                };
                var TPOProductionEntryVerifications = _executionTpobll.GetExeTPOProductionEntryVerification(criteria);
                var viewModel = new List<ExeTPOProductionEntryVerificationCompositeViewModel>(); 
                foreach(var i in TPOProductionEntryVerifications){
                    var data = Mapper.Map<ExeTPOProductionEntryVerificationCompositeViewModel>(i);
                    viewModel.Add(data); 
                }
                var pageResult = new PageResult<ExeTPOProductionEntryVerificationCompositeViewModel>(viewModel, criteria);
                return Json(pageResult);
            }
        }

        public JsonResult GetActiveBrandFromByLocation(string locationCode)
        {
            var model = _masterDataBLL.GetActiveBrandCodeByLocationCode(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private List<ExeReportByProcessViewDTO> CheckPreviousDayData(string locationCode, string unitCode, string brandCode, DateTime productionDate)
        {
            var input = new GetExeReportByProcessInput() 
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                BrandCode = brandCode,
                DateFrom = productionDate.AddDays(-1),
                DateTo = productionDate.AddDays(-1)
            };

            var listDataPrevious = _executionReportBll.GetReportByProcessGenerator(input);

            return !listDataPrevious.Any() ? null : listDataPrevious;
        }

        public JsonResult generateData(InsertUpdateData<ExePlantProductionEntryVerificationViewViewModel> bulkData)
        {
            var resultJSonSubmitData = "";
            var statusResult = "";
            var listResultJSon = new List<string>();
            //bool isEmpty = true;


            var paramProdDateFrom = bulkData.Parameters != null ? bulkData.Parameters["ProductionDateFrom"] : "";
            var paramProdDateTo = bulkData.Parameters != null ? bulkData.Parameters["ProductionDateTo"] : "";
            var paramLocationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var paramUnitCode = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var paramBrandCode = bulkData.Parameters != null ? bulkData.Parameters["BrandCode"] : "";

            var productionDateFrom = DateTime.ParseExact(paramProdDateFrom, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
            var productionDateTo = DateTime.ParseExact(paramProdDateTo, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);

            var parentLocation = _svc.CheckLocationByLocation(paramLocationCode);
            var isPrevDataExist = true;
            //try
            //{
            //    // Check report by process H-1
            //    var listPreviousData = CheckPreviousDayData(paramLocationCode, paramUnitCode, paramBrandCode, productionDateFrom);
            //    isPrevDataExist = true;
            //}
            //catch (Exception ex)
            //{
            //    resultJSonSubmitData = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            //    statusResult = "error";
            //    listResultJSon.Add(resultJSonSubmitData);
            //    listResultJSon.Add(statusResult);
            //    return Json(listResultJSon);
            //}

            //var listPreviousData = CheckPreviousDayData(paramLocationCode, paramUnitCode, paramBrandCode, productionDateFrom);
            var kpsWeekFromData = _masterDataBLL.GetWeekByDate(productionDateFrom);
            var kpsWeekFrom = kpsWeekFromData == null ? 0 : kpsWeekFromData.Week;
            

            // Delete/Clean data first
            //_executionPlantBll.DeleteReportByProccess(paramLocationCode, paramUnitCode, paramBrandCode, productionDateFrom, productionDateTo);

            // Generate default by process UOM 1 - 14 based on data H-1
            //for (DateTime productionDate = productionDateFrom; productionDate <= productionDateTo; productionDate = productionDate.AddDays(1))
            //{
            //    //var kpsWeek = _masterDataBLL.GetWeekByDate(productionDate);
            //    //_executionPlantBll.InsertDefaultExeReportByProcess(paramLocationCode, paramBrandCode, paramUnitCode, productionDate.Year, kpsWeek.Week, productionDate, GetUserName(), GetUserName());
            //    //_executionReportBll.UpdateEndingStockByProcess(paramLocationCode, paramUnitCode, paramBrandCode, productionDate);
            //}
           

            //try
            //{
            //    if (listPreviousData == null)
            //    {
            //        isPrevDataExist = false;
            //        _executionPlantBll.InsertDefaultExeReportByProcess(paramLocationCode, paramBrandCode, paramUnitCode,productionDateFrom.Year, kpsWeekFrom, productionDateFrom, GetUserName(), GetUserName());
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _vtlogger.Err(ex, new List<object> { paramLocationCode, paramBrandCode, paramUnitCode, productionDateFrom.Year, kpsWeekFrom, productionDateFrom, GetUserName(), GetUserName() }, "GeneratorReportByProcess - InsertDefaultExeReportByProcess");
            //    return null;
            //}

            for (DateTime productionDate = productionDateFrom; productionDate <= productionDateTo; productionDate = productionDate.AddDays(1))
            {
                try
                {
                    var kpsWeek = _masterDataBLL.GetWeekByDate(productionDate);

                    var listDataPlantEntryVerifications = new List<ExePlantProductionEntryVerificationViewDTO>();
                    var listDateTPOEntryVerifications = new List<ExeTPOProductionEntryVerificationViewDTO>();

                    if (parentLocation == Enums.LocationCode.PLNT.ToString())
                    {
                        var input = new GetExePlantProductionEntryVerificationInput()
                        {
                            LocationCode = paramLocationCode,
                            UnitCode = paramUnitCode,
                            BrandCode = paramBrandCode,
                            KpsWeek = kpsWeek == null ? 0 : kpsWeek.Week,
                            KpsYear = productionDate.Year,
                            ProductionDate = productionDate
                        };
                        //listDataPlantEntryVerifications = _executionPlantBll.GetExePlantProductionEntryVerificationViews(input);
                        ////
                        //if (!listDataPlantEntryVerifications.Any()) isEmpty = false;

                        if (isPrevDataExist)
                        {
                            foreach (var process in _executionPlantBll.GetListProcessGeneratorByProcess(paramLocationCode, paramUnitCode, paramBrandCode, 1, productionDate))
                            {
                                try
                                {
                                    _executionPlantBll.InsertReportByProcessGenerator(paramLocationCode, paramBrandCode, process, input.KpsYear, input.KpsWeek, GetUserName(), GetUserName(), productionDate, paramUnitCode);
                                }
                                catch (Exception ex)
                                {
                                    _vtlogger.Err(ex, new List<object> { paramLocationCode, paramBrandCode, process, input.KpsYear, input.KpsWeek, GetUserName(), GetUserName(), productionDate, paramUnitCode }, "GeneratorReportByProcess - InsertReportByProcess");
                                    return null;
                                }
                                
                            }

                            //if (!isEmpty)
                            //{
                            //    try
                            //    {
                            //        _executionPlantBll.DefaultExeReportByProcess(paramLocationCode, paramBrandCode, paramUnitCode, productionDate.Year, input.KpsWeek, productionDate, GetUserName(), GetUserName());
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        _vtlogger.Err(ex, new List<object> { paramLocationCode, paramBrandCode, paramUnitCode, productionDate.Year, input.KpsWeek, productionDate, GetUserName(), GetUserName() }, "GeneratorReportByProcess - DefaultExeReportByProcess");
                            //        return null;
                            //    }

                            //    try
                            //    {
                            //        _executionReportBll.UpdateEndingStockByProcess(paramLocationCode, paramUnitCode, paramBrandCode, productionDate);
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        _vtlogger.Err(ex, new List<object> { paramLocationCode, paramUnitCode, paramBrandCode, productionDate }, "GeneratorReportByProcess - UpdateEndingStockByProcess");
                            //        return null;
                            //    }
                                
                                
                            //    isEmpty = true;
                            //}    
                        }

                        isPrevDataExist = true;

                    }
                    else
                    {
                        var input = new GetExeTPOProductionEntryVerificationInput()
                        {
                            LocationCode = paramLocationCode,
                            BrandCode = paramBrandCode,
                            KPSWeek = kpsWeek == null ? 0 : kpsWeek.Week.Value,
                            KPSYear = productionDate.Year,
                            ProductionDate = productionDate
                        };
                        //listDateTPOEntryVerifications = _executionTpobll.GetExeTPOProductionEntryVerification(input);

                        //if (!listDateTPOEntryVerifications.Any()) isEmpty = false;

                        var listMstGenProcess = _masterDataBLL.GetAllMasterProcess();

                        var listProcess = listDateTPOEntryVerifications.Join(listMstGenProcess, c => c.ProcessGroup, d => d.ProcessGroup, (c, d) => new { c.ProcessGroup, d.ProcessOrder }).OrderByDescending(d => d.ProcessOrder).ToList();

                        if (isPrevDataExist)
                        {
                            foreach (var process in _executionPlantBll.GetListProcessGeneratorByProcess(paramLocationCode, paramUnitCode, paramBrandCode, 1, productionDate))
                            {
                                try
                                {
                                    _executionPlantBll.InsertReportByProcessGenerator(paramLocationCode, paramBrandCode, process, productionDate.Year, input.KPSWeek, GetUserName(), GetUserName(), productionDate, paramUnitCode);
                                }
                                catch (Exception ex)
                                {
                                    _vtlogger.Err(ex, new List<object> { paramLocationCode, paramBrandCode, process, productionDate.Year, input.KPSWeek, GetUserName(), GetUserName(), productionDate, paramUnitCode }, "GeneratorReportByProcess - InsertReportByProcess");
                                    return null;
                                }
                                
                            }

                            //if (!isEmpty)
                            //{
                            //    try
                            //    {
                            //        _executionPlantBll.DefaultExeReportByProcess(paramLocationCode, paramBrandCode,paramUnitCode, productionDate.Year, input.KPSWeek, productionDate, GetUserName(),GetUserName());
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        _vtlogger.Err(ex, new List<object> { paramLocationCode, paramBrandCode, paramUnitCode, productionDate.Year, input.KPSWeek, productionDate, GetUserName(), GetUserName() }, "GeneratorReportByProcess - DefaultExeReportByProcess");
                            //        return null;
                            //    }

                            //    try
                            //    {
                            //        _executionReportBll.UpdateEndingStockByProcess(paramLocationCode, paramUnitCode,paramBrandCode, productionDate);
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        _vtlogger.Err(ex, new List<object> { paramLocationCode, paramUnitCode, paramBrandCode, productionDate }, "GeneratorReportByProcess - UpdateEndingStockByProcess");
                            //        return null;
                            //    }
                                
                            //    isEmpty = true;
                            //}    
                        }

                        isPrevDataExist = true;

                    }
                }
                catch (Exception ex)
                {
                    _vtlogger.Err(ex, new List<object> { bulkData }, "GeneratorReportByProcess - GenerateData");
                    resultJSonSubmitData = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    statusResult = "error";
                    listResultJSon.Add(resultJSonSubmitData);
                    listResultJSon.Add(statusResult);
                    return Json(listResultJSon);
                }
            }

            //for (DateTime productionDate = productionDateFrom; productionDate <= productionDateTo; productionDate = productionDate.AddDays(1))
            //{
            //    #region check adjusment
            //    //If adjustment exist
            //    var adjusmentInput = new ProductAdjustmentInput()
            //    {
            //        BrandCode = paramBrandCode,
            //        LocationCode = paramLocationCode,
            //        ProductionDate = productionDate
            //    };

            //    var adjustment = _executionOtherBll.GetProductAdjustments(adjusmentInput);
            //    foreach (var adj in adjustment)
            //    {
            //        var updateInput = new ProductAdjustmentDTO()
            //        {
            //            LocationCode = adj.LocationCode,
            //            BrandCode = adj.BrandCode,
            //            ProductionDate = adj.ProductionDate,
            //            UnitCode = adj.UnitCode,
            //            AdjustmentType = adj.AdjustmentType,
            //            AdjustmentRemark = adj.AdjustmentRemark,
            //            AdjustmentValue = adj.AdjustmentValue,
            //            CreatedBy = adj.CreatedBy,
            //            UpdatedBy = adj.UpdatedBy,
            //            GroupCode = adj.GroupCode,
            //            Shift = adj.Shift
            //        };

            //        _executionOtherBll.UpdateProductAdjustment(updateInput);
            //    }
            //    #endregion
            //    #region switching brand
            //    try
            //    {
            //        var brandGroup = _masterDataBLL.GetBrandGruopCodeByBrandCode(paramBrandCode);
            //        _executionPlantBll.switchBrandExeReportByProcess(paramLocationCode, brandGroup, productionDate);
            //    }
            //    catch (Exception ex)
            //    {
            //        resultJSonSubmitData = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            //        statusResult = "error";
            //        listResultJSon.Add(resultJSonSubmitData);
            //        listResultJSon.Add(statusResult);
            //        return Json(listResultJSon);
            //    }
            //    #endregion
            //    #region Recalculate Stock
            //    try
            //    {
            //        _executionPlantBll.recalculateStockExeReportByProcess(paramLocationCode, paramBrandCode, productionDate);
            //    }
            //    catch (Exception ex)
            //    {
            //        resultJSonSubmitData = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            //        statusResult = "error";
            //        listResultJSon.Add(resultJSonSubmitData);
            //        listResultJSon.Add(statusResult);
            //        return Json(listResultJSon);
            //    }
            //    #endregion

            //    resultJSonSubmitData = "Generate Report by Process Running in background";
            //    statusResult = "success";
            //    listResultJSon.Add(resultJSonSubmitData);
            //    listResultJSon.Add(statusResult);
            //}

            //var weeks = _masterDataBLL.GetWeekByDateRange(productionDateFrom, productionDateTo);

            //try
            //{
            //    if (weeks.Count > 0)
            //    {
            //        var input = new GetExeTPOProductionEntryVerificationInput
            //        {
            //            LocationCode = paramLocationCode,
            //            BrandCode = paramBrandCode
            //        };

            //        foreach (var w in weeks)
            //        {
            //            input.KPSYear = w.Year.Value;
            //            input.KPSWeek = w.Week.Value;
            //            input.ProductionDate = w.StartDate;

            //            var result = _executionTpobll.TPOProductionEntryVerificationGenerateReport(input, GetUserName());
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    resultJSonSubmitData = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            //    statusResult = "error";
            //    listResultJSon.Add(resultJSonSubmitData);
            //    listResultJSon.Add(statusResult);
            //    return Json(listResultJSon);
            //}

            return Json(listResultJSon);
        }
	}
}