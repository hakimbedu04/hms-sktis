using AutoMapper;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.ProductionCard;
using SKTISWebsite.Models.UtilTransactionLog;
using SKTISWebsite.Models.WagesProductionCardApprovalDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using Color = System.Drawing.Color;
using Newtonsoft.Json;

namespace SKTISWebsite.Controllers
{
    public class WagesProductionCardApprovalDetailController : BaseController
    {
        private IMasterDataBLL _masterDataBll;
        private IPlantWagesExecutionBLL _plantWagesBll;
        private IUtilitiesBLL _utilitiesBLL;
        private IGeneralBLL _generalBll;
        private IApplicationService _applicationService;
        //public string _location = "";
        //public string _unit = "";
        //public int _revType = 0;

        public WagesProductionCardApprovalDetailController(IApplicationService applicationService, IMasterDataBLL masterDataBLL, IPlantWagesExecutionBLL plantWagesBLL, IUtilitiesBLL utilitiesBLL, IGeneralBLL generalBLL)
        {
            _masterDataBll = masterDataBLL;
            _plantWagesBll = plantWagesBLL;
            _utilitiesBLL = utilitiesBLL;
            _generalBll = generalBLL;
            _applicationService = applicationService;
            SetPage("PlantWages/Execution/ProductionCardApprovalDetail");
        }

        public ActionResult Index(string param1, string param2, string param3, int? param4)
        {
            //_location = param1;
            //_unit = param2;
            //_revType = Convert.ToInt32(param3);
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            if (param4.HasValue) setResponsibility(param4.Value);
            var model = new InitWagesProductionCardApprovalDetailViewModel()
            {
                LocationCode = param1,
                LocationName = _masterDataBll.GetLocation(param1).LocationName ?? "",
                UnitCode = param2,
                Shift = _masterDataBll.GetLocation(param1).Shift,
                RevisionType = param3,
                IDRole = strUserID.Responsibility.Role
                //ProductionDateList = _plantWagesBll.GetProductionCardApprovalDetail(new GetProductionCardApprovalDetailInput() { LocationCode = param1, UnitCode = param2 }).Select(p => p.ProductionDate).Distinct().OrderBy(p => p.Date).ToList()
                //BrandCodeList = _plantWagesBll.GetProductionCardApprovalDetail(new GetProductionCardApprovalDetailInput() { LocationCode = param1, UnitCode = param2 }).OrderBy(p => p.BrandCode).Select(p => p.BrandCode).Distinct().ToList()
            };
            return View(model);
        }
        
        

        [HttpGet]
        public ActionResult GetBrandListPartial(string LocationCode, string UnitCode, string BrandGroup, string ProductionDate)
        {
            var model = this.GetFullAndPartialViewModel(LocationCode, UnitCode, BrandGroup, ProductionDate);
            return PartialView("partialbrand", model);
        }

        private InitWagesProductionCardApprovalDetailViewModel GetFullAndPartialViewModel(string LocationCode, string UnitCode, string BrandGroup, string ProductionDate)
        {
            var date = !string.IsNullOrEmpty(ProductionDate) ? Convert.ToDateTime(ProductionDate) : DateTime.Now;
            var brandCodeList =
                _plantWagesBll.GetProductionCardApprovalDetail(new GetProductionCardApprovalDetailInput()
                {
                    LocationCode = LocationCode,
                    UnitCode = UnitCode,
                    BrandGroupCode = BrandGroup,
                    ProductionDate = date
                })
                    .OrderBy(p => p.BrandCode)
                    .Select(p => p.BrandCode)
                    .Distinct()
                    .ToList();
            var model = new InitWagesProductionCardApprovalDetailViewModel()
            {
                //BrandCodeList = _plantWagesBll.GetProductionCardApprovalDetail(new GetProductionCardApprovalDetailInput() { LocationCode = LocationCode, UnitCode = UnitCode, BrandGroupCode = BrandGroup, ProductionDate = ProductionDate }).OrderBy(p => p.BrandCode).Select(p => p.BrandCode).Distinct().ToList()
                BrandCodeList = brandCodeList,
                BrandGroupCode = BrandGroup
            };
            return model;
        }

        [HttpGet]
        public ActionResult GetBrandListDetailPartial(string LocationCode, string UnitCode, string ProductionDate, string BrandCode)
        {
            var model = this.GetFullAndPartialDetailViewModel(LocationCode, UnitCode, ProductionDate, BrandCode);
            return PartialView("partialbranddetail", model);
        }

        private List<WagesProductionCardApprovalDetailViewModel> GetFullAndPartialDetailViewModel(string LocationCode, string UnitCode, string ProductionDate, string BrandCode)
        {
            var date = !string.IsNullOrEmpty(ProductionDate) ? Convert.ToDateTime(ProductionDate) : DateTime.Now;
            var input = new GetProductionCardApprovalDetailInput()
            {
                LocationCode = LocationCode,
                UnitCode = UnitCode,
                ProductionDate = date,
                BrandCode = BrandCode
            };
            var ProductionCardApprovalDetail = _plantWagesBll.GetProductionCardApprovalDetail(input);
            var viewModel = Mapper.Map<List<WagesProductionCardApprovalDetailViewModel>>(ProductionCardApprovalDetail);
            //var pageResult = new PageResult<WagesProductionCardApprovalDetailViewModel>(viewModel);
            return viewModel;
        }

        [HttpGet]
        public JsonResult GetDateNearestClosingPayrollList(string locationCode, string unitCode, int revisionType, string URLdate)
        {
            DateTime now = DateTime.Now;
            DateTime myDate = Convert.ToDateTime(URLdate);
            var result = _applicationService.GetClosingPayrollList(myDate, locationCode, unitCode, revisionType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStatusApproval(string locationCode, string unitCode, string date)
        {
            var result = _plantWagesBll.GetStatusApproval(locationCode, unitCode, Convert.ToDateTime(date));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStatusComplete(string locationCode, string unitCode, string date)
        {
            var result = _plantWagesBll.GetStatusComplete(locationCode, unitCode, Convert.ToDateTime(date));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStatusReturn(string locationCode, string unitCode, string date)
        {
            var result = _plantWagesBll.GetStatusReturn(locationCode, unitCode, Convert.ToDateTime(date));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBrandCodeByLocationCode(string locationCode, string unit, string shift, string date, string revisiontype)
        {
            var result = _applicationService.GetBrandCodeFromProductionCardByDate(locationCode, unit, shift, Convert.ToDateTime(date), revisiontype);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTotalDetailProcess(string LocationCode, string UnitCode, string ProductionDate, string BrandCode, string BrandGroupCode, string RevisionType)
        {
            var input = new GetProductionCardApprovalDetailInput()
            {
                LocationCode = LocationCode,
                UnitCode = UnitCode,
                ProductionDate = Convert.ToDateTime(ProductionDate),
                BrandCode = BrandCode,
                BrandGroupCode = BrandGroupCode,
                RevisionType = String.IsNullOrEmpty(RevisionType) ? 0 : Convert.ToInt32(RevisionType)
            };

            if (input.BrandCode == input.BrandGroupCode) {
                var result = GetDataProdCardApprovalGridViewGroup(input);
                var totalWorkerByBrand = result.Sum(c => c.TotalWorker);
                var totalProduksiByBrand = result.Sum(c => c.TotalProduksi);
                var totalUpahLainByBrand = result.Sum(c => c.TotalUpahLain);
                var totalGroupByBrand = result.Sum(c => c.TotalGroup);
                var total_A_ByBrand = result.Sum(c => c.Total_A);
                var total_C_ByBrand = result.Sum(c => c.Total_C);
                var total_CH_ByBrand = result.Sum(c => c.Total_CH);
                var total_CT_ByBrand = result.Sum(c => c.Total_CT);
                var total_I_ByBrand = result.Sum(c => c.Total_I);
                var total_LL_ByBrand = result.Sum(c => c.Total_LL);
                var total_LO_ByBrand = result.Sum(c => c.Total_LO);
                var total_LP_ByBrand = result.Sum(c => c.Total_LP);
                var total_MO_ByBrand = result.Sum(c => c.Total_MO);
                var total_PG_ByBrand = result.Sum(c => c.Total_PG);
                var total_SS_ByBrand = result.Sum(c => c.Total_SS);
                var total_SB_ByBrand = result.Sum(c => c.Total_SB);
                var total_SKR_ByBrand = result.Sum(c => c.Total_SKR);
                var total_SL4_ByBrand = result.Sum(c => c.Total_SL4);
                var total_SLP_ByBrand = result.Sum(c => c.Total_SLP);
                var total_SLS_ByBrand = result.Sum(c => c.Total_SLS);
                var total_T_ByBrand = result.Sum(c => c.Total_T);
                var total_TL_ByBrand = result.Sum(c => c.Total_TL);

                var prodCardTotalGridItem = new WagesProductionCardApprovalDetailTotalViewGroupModel() {
                    TotalUpahLainBrand = totalUpahLainByBrand,
                    TotalWorkerBrand = totalWorkerByBrand,
                    TotalProduksiBrand = totalProduksiByBrand,
                    TotalGroupBrand = totalGroupByBrand,
                    TotalA = total_A_ByBrand,
                    TotalC = total_C_ByBrand,
                    TotalCH = total_CH_ByBrand,
                    TotalCT = total_CT_ByBrand,
                    TotalI = total_I_ByBrand,
                    TotalLL = total_LL_ByBrand,
                    TotalLO = total_LO_ByBrand,
                    TotalLP = total_LP_ByBrand,
                    TotalMO = total_MO_ByBrand,
                    TotalPG = total_PG_ByBrand,
                    TotalSS = total_SS_ByBrand,
                    TotalSB = total_SB_ByBrand,
                    TotalSKR = total_SKR_ByBrand,
                    TotalSL4 = total_SL4_ByBrand,
                    TotalSLP = total_SLP_ByBrand,
                    TotalSLS = total_SLS_ByBrand,
                    TotalT = total_T_ByBrand,
                    TotalTL = total_TL_ByBrand,
                };
                return Json(prodCardTotalGridItem, JsonRequestBehavior.AllowGet);
            }
            else 
            {
                var result = GetDataProdCardApprovalGridView(input);
                var totalWorkerByBrand = result.Sum(c => c.TotalWorker);
                var totalProduksiByBrand = result.Sum(c => c.TotalProduksi);
                var totalUpahLainByBrand = result.Sum(c => c.TotalUpahLain);
                var totalGroupByBrand = result.Sum(c => c.TotalGroup);
                var total_A_ByBrand = result.Sum(c => c.Total_A);
                var total_C_ByBrand = result.Sum(c => c.Total_C);
                var total_CH_ByBrand = result.Sum(c => c.Total_CH);
                var total_CT_ByBrand = result.Sum(c => c.Total_CT);
                var total_I_ByBrand = result.Sum(c => c.Total_I);
                var total_LL_ByBrand = result.Sum(c => c.Total_LL);
                var total_LO_ByBrand = result.Sum(c => c.Total_LO);
                var total_LP_ByBrand = result.Sum(c => c.Total_LP);
                var total_MO_ByBrand = result.Sum(c => c.Total_MO);
                var total_PG_ByBrand = result.Sum(c => c.Total_PG);
                var total_SS_ByBrand = result.Sum(c => c.Total_SS);
                var total_SB_ByBrand = result.Sum(c => c.Total_SB);
                var total_SKR_ByBrand = result.Sum(c => c.Total_SKR);
                var total_SL4_ByBrand = result.Sum(c => c.Total_SL4);
                var total_SLP_ByBrand = result.Sum(c => c.Total_SLP);
                var total_SLS_ByBrand = result.Sum(c => c.Total_SLS);
                var total_T_ByBrand = result.Sum(c => c.Total_T);
                var total_TL_ByBrand = result.Sum(c => c.Total_TL);

                var prodCardTotalGridItem = new WagesProductionCardApprovalDetailTotalViewGroupModel() {
                    TotalUpahLainBrand = totalUpahLainByBrand,
                    TotalWorkerBrand = totalWorkerByBrand,
                    TotalProduksiBrand = totalProduksiByBrand,
                    TotalGroupBrand = totalGroupByBrand,
                    TotalA = total_A_ByBrand,
                    TotalC = total_C_ByBrand,
                    TotalCH = total_CH_ByBrand,
                    TotalCT = total_CT_ByBrand,
                    TotalI = total_I_ByBrand,
                    TotalLL = total_LL_ByBrand,
                    TotalLO = total_LO_ByBrand,
                    TotalLP = total_LP_ByBrand,
                    TotalMO = total_MO_ByBrand,
                    TotalPG = total_PG_ByBrand,
                    TotalSS = total_SS_ByBrand,
                    TotalSB = total_SB_ByBrand,
                    TotalSKR = total_SKR_ByBrand,
                    TotalSL4 = total_SL4_ByBrand,
                    TotalSLP = total_SLP_ByBrand,
                    TotalSLS = total_SLS_ByBrand,
                    TotalT = total_T_ByBrand,
                    TotalTL = total_TL_ByBrand,
                };
                return Json(prodCardTotalGridItem, JsonRequestBehavior.AllowGet);
            }
        }

        private WagesProductionCardApprovalDetailTotalViewGroupModel GetTotalDetailProc(string LocationCode, string UnitCode, string ProductionDate, string BrandCode, string BrandGroupCode)
        {
            var input = new GetProductionCardApprovalDetailInput()
            {
                LocationCode = LocationCode,
                UnitCode = UnitCode,
                ProductionDate = Convert.ToDateTime(ProductionDate),
                BrandCode = BrandCode,
                BrandGroupCode = BrandGroupCode
            };
            var prodCards = _plantWagesBll.GetProductionCardApprovalDetail(input);
            var totalWorkerByBrand = prodCards.Where(p => p.BrandGroupCode == input.BrandGroupCode).Select(p => p.Worker).Sum();
            var totalProduksiByBrand = prodCards.Where(p => p.BrandGroupCode == input.BrandGroupCode).Select(p => p.Production).Sum();
            var totalUpahLainByBrand = prodCards.Where(p => p.BrandGroupCode == input.BrandGroupCode).Select(p => p.UpahLain).Sum();
            var totalGroupByBrand = prodCards.Where(p => p.BrandGroupCode == input.BrandGroupCode).DistinctBy(p => p.GroupCode).Count();
            var prodCardTotalGridItem = new WagesProductionCardApprovalDetailTotalViewGroupModel()
            {
                TotalUpahLainBrand = totalUpahLainByBrand,
                TotalWorkerBrand = totalWorkerByBrand,
                TotalProduksiBrand = totalProduksiByBrand,
                TotalGroupBrand = totalGroupByBrand
            };
            return prodCardTotalGridItem;
        }

        public JsonResult GetRoleButton(string locationCode, string unitCode, string productionDate, string brandCode, string brandGroupCode, string shift, int RoleId, string revisionType, string currentDate)
        {
            var date = Convert.ToDateTime(productionDate);
            var currentdate = Convert.ToDateTime(currentDate);
            var input = new ButtonStateInput()
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                Shift = shift,
                Date = date.ToString("yyyy-MM-dd"),
                BrandGroupCode = brandGroupCode,
                BrandCode = brandCode,
                RoleId = CurrentUser.Responsibility.Role,
                revisiontype = revisionType,
                currentDate = currentdate.ToString("yyyy-MM-dd"),
            };
            
            var state = _plantWagesBll.GetButtonState(input).FirstOrDefault();

            var init = new ButtonState
            {
                Approve = state != null && (state.Result1 != null && state.Result1),
                Return = state != null && (state.Result2 != null && state.Result2),
                Complete = state != null && (state.Result3 != null && state.Result3),
            };
            return Json(init, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]//cek
        public JsonResult GetProductionCardApprovalDetailList(GetProductionCardApprovalDetailInput input)
        {
            if (input.BrandCode == input.BrandGroupCode)
            {
                var prodCardGrid = GetDataProdCardApprovalGridViewGroup(input);
                
                var viewModel = Mapper.Map<List<WagesProductionCardApprovalDetailGridViewGroupModel>>(prodCardGrid);
                var pageResult = new PageResult<WagesProductionCardApprovalDetailGridViewGroupModel>(viewModel, input);
                return Json(pageResult);
            }
            else
            {
                var prodCardGrid = GetDataProdCardApprovalGridView(input);

                var viewModel = Mapper.Map<List<WagesProductionCardApprovalDetailGridViewModel>>(prodCardGrid);
                var pageResult = new PageResult<WagesProductionCardApprovalDetailGridViewModel>(viewModel, input);
                return Json(pageResult);
            }

        }

        private List<WagesProductionCardApprovalDetailGridViewModel> GetDataProdCardApprovalGridView(GetProductionCardApprovalDetailInput input) 
        {
            var prodCardGrid = new List<WagesProductionCardApprovalDetailGridViewModel>();

            var prodCardList = _plantWagesBll.GetProductionCardApprovalDetail(input);
            var availableGroup = prodCardList.Select(p => p.ProcessGroup).Distinct().ToList();
            foreach (var group in availableGroup) {
                var prodCardPerGroup = prodCardList.Where(p => p.ProcessGroup == group);
                var prodCardPerGroupViewModel = Mapper.Map<List<WagesProductionCardApprovalDetailViewModel>>(prodCardPerGroup);
                var prodCardTotalGroup = prodCardList.Where(p => p.ProcessGroup == group).Count();
                var prodCardTotalWorker = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.Worker).Sum();
                var prodCardTotalProduksi = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.Production).Sum();
                var prodCardTotalUpahLain = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.UpahLain).Sum();
                var prodCardTotal_A = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.A).Sum();
                var prodCardTotal_C = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.C).Sum();
                var prodCardTotal_CH = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.CH).Sum();
                var prodCardTotal_CT = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.CT).Sum();
                var prodCardTotal_I = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.I).Sum();
                var prodCardTotal_LL = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.LL).Sum();
                var prodCardTotal_LO = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.LO).Sum();
                var prodCardTotal_LP = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.LP).Sum();
                var prodCardTotal_MO = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.MO).Sum();
                var prodCardTotal_PG = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.PG).Sum();
                var prodCardTotal_SS = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.S).Sum();
                var prodCardTotal_SB = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SB).Sum();
                var prodCardTotal_SKR = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SKR).Sum();
                var prodCardTotal_SL4 = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SL4).Sum();
                var prodCardTotal_SLP = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SLP).Sum();
                var prodCardTotal_SLS = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SLS).Sum();
                var prodCardTotal_T = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.T).Sum();
                var prodCardTotal_TL = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.TL).Sum();

                var prodCardGridItem = new WagesProductionCardApprovalDetailGridViewModel() {
                    ProdCardApprovalList = prodCardPerGroupViewModel,
                    TotalGroup = prodCardTotalGroup,
                    TotalWorker = prodCardTotalWorker,
                    TotalProduksi = prodCardTotalProduksi,
                    TotalUpahLain = prodCardTotalUpahLain,
                    Total_A = prodCardTotal_A,
                    Total_C = prodCardTotal_C,
                    Total_CH = prodCardTotal_CH,
                    Total_CT = prodCardTotal_CT,
                    Total_I = prodCardTotal_I,
                    Total_LL = prodCardTotal_LL,
                    Total_LO = prodCardTotal_LO,
                    Total_LP = prodCardTotal_LP,
                    Total_MO = prodCardTotal_MO,
                    Total_PG = prodCardTotal_PG,
                    Total_SS = prodCardTotal_SS,
                    Total_SB = prodCardTotal_SB,
                    Total_SKR = prodCardTotal_SKR,
                    Total_SL4 = prodCardTotal_SL4,
                    Total_SLP = prodCardTotal_SLP,
                    Total_SLS = prodCardTotal_SLS,
                    Total_T = prodCardTotal_T,
                    Total_TL = prodCardTotal_TL
                };

                prodCardGrid.Add(prodCardGridItem);
            }
            return prodCardGrid;
        }

        private List<WagesProductionCardApprovalDetailGridViewGroupModel> GetDataProdCardApprovalGridViewGroup(GetProductionCardApprovalDetailInput input) 
        {
            var prodCardGrid = new List<WagesProductionCardApprovalDetailGridViewGroupModel>();
            //var prodCardTotalGrid = new List<WagesProductionCardApprovalDetailTotalViewGroupModel>();
            var prodCardList = _plantWagesBll.GetProductionCardApprovalDetailGroup(input);
            var availableGroup = prodCardList.Select(p => p.ProcessGroup).Distinct().ToList();
            foreach (var group in availableGroup) {
                var prodCardPerGroup = prodCardList.Where(p => p.ProcessGroup == group);
                var prodCardPerGroupViewModel = Mapper.Map<List<WagesProductionCardApprovalDetailViewModel>>(prodCardPerGroup);
                var prodCardTotalGroup = prodCardList.Where(p => p.ProcessGroup == group).Count();
                var prodCardTotalWorker = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.Worker).Sum();
                var prodCardTotalProduksi = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.Production).Sum();
                var prodCardTotalUpahLain = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.UpahLain).Sum();
                var prodCardTotal_A = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.A).Sum();
                var prodCardTotal_C = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.C).Sum();
                var prodCardTotal_CH = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.CH).Sum();
                var prodCardTotal_CT = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.CT).Sum();
                var prodCardTotal_I = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.I).Sum();
                var prodCardTotal_LL = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.LL).Sum();
                var prodCardTotal_LO = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.LO).Sum();
                var prodCardTotal_LP = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.LP).Sum();
                var prodCardTotal_MO = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.MO).Sum();
                var prodCardTotal_PG = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.PG).Sum();
                var prodCardTotal_SS = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.S).Sum();
                var prodCardTotal_SB = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SB).Sum();
                var prodCardTotal_SKR = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SKR).Sum();
                var prodCardTotal_SL4 = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SL4).Sum();
                var prodCardTotal_SLP = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SLP).Sum();
                var prodCardTotal_SLS = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.SLS).Sum();
                var prodCardTotal_T = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.T).Sum();
                var prodCardTotal_TL = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.TL).Sum();

                var prodCardGridItem = new WagesProductionCardApprovalDetailGridViewGroupModel() {
                    ProdCardApprovalList = prodCardPerGroupViewModel,
                    TotalGroup = prodCardTotalGroup,
                    TotalWorker = prodCardTotalWorker,
                    TotalProduksi = prodCardTotalProduksi,
                    TotalUpahLain = prodCardTotalUpahLain,
                    Total_A = prodCardTotal_A,
                    Total_C = prodCardTotal_C,
                    Total_CH = prodCardTotal_CH,
                    Total_CT = prodCardTotal_CT,
                    Total_I = prodCardTotal_I,
                    Total_LL = prodCardTotal_LL,
                    Total_LO = prodCardTotal_LO,
                    Total_LP = prodCardTotal_LP,
                    Total_MO = prodCardTotal_MO,
                    Total_PG = prodCardTotal_PG,
                    Total_SS = prodCardTotal_SS,
                    Total_SB = prodCardTotal_SB,
                    Total_SKR = prodCardTotal_SKR,
                    Total_SL4 = prodCardTotal_SL4,
                    Total_SLP = prodCardTotal_SLP,
                    Total_SLS = prodCardTotal_SLS,
                    Total_T = prodCardTotal_T,
                    Total_TL = prodCardTotal_TL
                };

                prodCardGrid.Add(prodCardGridItem);
            }
            return prodCardGrid;
        }

        [HttpPost]
        public JsonResult GetEmployeeList(GetProductionCardInput criteria)
        {
            var result = _plantWagesBll.GetProductionCards(criteria);
            var viewModel = Mapper.Map<List<ProductionCardViewModel>>(result);
            //var pageResult = new PageResult<ProductionCardViewModel>(viewModel, criteria);
            return Json(viewModel);
        }

        [HttpPost]
        public ActionResult ProductionCardApprovalActions(Enums.ButtonName action, List<WagesProductionCardApprovalDetailGridViewModel> bulkData, string locationCode, string unitCode, string brandGroupCode, string date, string revisiontype)
        {
            var pc = _plantWagesBll.GetProductionCards(new GetProductionCardInput
            {
                LocationCode = locationCode,
                Unit = unitCode,
                BrandGroupCode = brandGroupCode,
                Date = Convert.ToDateTime(date),
                RevisionType = Convert.ToInt32(revisiontype)
            }).Select(p => new ProductionCardDTO
            {
                ProductionCardCode = p.ProductionCardCode,
                RevisionType = p.RevisionType,
                ProductionDate = p.ProductionDate
            }).Distinct().ToList();

            switch (action)
            {
                case Enums.ButtonName.Approve:
                    _plantWagesBll.ApproveProductionCardApprovalDetail(GetUserName(), pc, CurrentUser);
                    return Json("Run approve data on background process.");
                    break;
                case Enums.ButtonName.Complete:
                    _plantWagesBll.CompleteProductionCardApprovalDetail(GetUserName(), pc, CurrentUser);
                    return Json("Run complete data on background process.");
                    break;
                case Enums.ButtonName.Return:
                    _plantWagesBll.ReturnProductionCardApprovalDetail(GetUserName(), pc, CurrentUser);
                    return Json("Return data on background process.");
                    break;
            }

            return Json(bulkData);
        }

        public FileStreamResult GenerateExcel(GetProductionCardInput criteria)
        {
            #region production card approval detail group
            var prodCardGrid = new List<WagesProductionCardApprovalDetailGridViewGroupModel>();
            var prodCardList = _plantWagesBll.GetProductionCardApprovalDetailGroup(new GetProductionCardApprovalDetailInput{
                                    LocationCode = criteria.LocationCode,
                                    UnitCode = criteria.Unit,
                                    ProductionDate = criteria.Date,
                                    BrandGroupCode = criteria.BrandGroupCode,
                                    RevisionType = criteria.RevisionType
                                });
            var availableGroup = prodCardList.Select(p => p.ProcessGroup).Distinct().ToList();
            var brandCodeList = _plantWagesBll.GetProductionCards(new GetProductionCardInput
            {
                LocationCode = criteria.LocationCode,
                Unit = criteria.Unit,
                Date = criteria.Date,
                BrandGroupCode = criteria.BrandGroupCode
            }).Select(p => p.BrandCode).Distinct().ToList();

            var allLocations = _applicationService.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == criteria.LocationCode)
                {
                    locationCompat = item.Text;
                }
            }

            foreach (var group in availableGroup)
            {
                var prodCardPerGroup = prodCardList.Where(p => p.ProcessGroup == group);
                var prodCardPerGroupViewModel = Mapper.Map<List<WagesProductionCardApprovalDetailViewModel>>(prodCardPerGroup);
                var prodCardTotalGroup = prodCardList.Where(p => p.ProcessGroup == group).Count();
                var prodCardTotalWorker = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.Worker).Sum();
                var prodCardTotalProduksi = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.Production).Sum();
                var prodCardTotalUpahLain = prodCardList.Where(p => p.ProcessGroup == group).Select(p => p.UpahLain).Sum();

                var prodCardGridItem = new WagesProductionCardApprovalDetailGridViewGroupModel()
                {
                    ProdCardApprovalList = prodCardPerGroupViewModel,
                    TotalGroup = prodCardTotalGroup,
                    TotalWorker = prodCardTotalWorker,
                    TotalProduksi = prodCardTotalProduksi,
                    TotalUpahLain = prodCardTotalUpahLain
                };
                prodCardGrid.Add(prodCardGridItem);
            }
            var modelGroup = Mapper.Map<List<WagesProductionCardApprovalDetailGridViewGroupModel>>(prodCardGrid);
            var totalGroup = GetTotalDetailProc(criteria.LocationCode, criteria.Unit, criteria.Date.ToShortDateString(), criteria.Brand, criteria.BrandGroupCode);
            #endregion

            #region excel configuration
            var ms = new MemoryStream();
                        string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.PlantWagesExcelTemplate.PlantWagesProductionCardApprovalDetail + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }


            #endregion

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                #region header excel
                slDoc.SetCellValue(3, 2, criteria.Date.ToString(Constants.DefaultDateOnlyFormat));
                slDoc.SetCellValue(4, 2, locationCompat);
                slDoc.SetCellValue(5, 2, criteria.Shift);
                slDoc.SetCellValue(6, 2, criteria.Unit);
                

                SLStyle style = slDoc.CreateStyle();
                
                style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                style.Font.FontName = "Calibri";
                style.Font.FontSize = 12;
                style.Font.Bold = true;
                style.Fill.SetPattern(PatternValues.Solid, Color.Blue, Color.Blue);
                slDoc.SetCellValue(8, 1, criteria.BrandGroupCode);
                slDoc.SetCellStyle(9, 1, 9, 23, style);
                slDoc.SetCellValue(9, 1, "Process");
                slDoc.SetCellValue(9, 2, "Group");
                slDoc.SetCellValue(9, 3, "Worker");
                slDoc.SetCellValue(9, 4, "Produksi");
                slDoc.SetCellValue(9, 5, "Upah Lain");
                slDoc.SetCellValue(9, 6, "A");
                slDoc.SetCellValue(9, 7, "C");
                slDoc.SetCellValue(9, 8, "CH");
                slDoc.SetCellValue(9, 9, "CT");
                slDoc.SetCellValue(9, 10, "I");
                slDoc.SetCellValue(9, 11, "LL");
                slDoc.SetCellValue(9, 12, "LO");
                slDoc.SetCellValue(9, 13, "LP");
                slDoc.SetCellValue(9, 14, "MO");
                slDoc.SetCellValue(9, 15, "PG");
                slDoc.SetCellValue(9, 16, "S");
                slDoc.SetCellValue(9, 17, "SB");
                slDoc.SetCellValue(9, 18, "SKR");
                slDoc.SetCellValue(9, 19, "SL4");
                slDoc.SetCellValue(9, 20, "SLP");
                slDoc.SetCellValue(9, 21, "SLS");
                slDoc.SetCellValue(9, 22, "T");
                slDoc.SetCellValue(9, 23, "TL");
                #endregion
                
                ////row values
                var iRow = 10;

                #region group 
                foreach (var model in modelGroup)
                {
                    foreach (var approval in model.ProdCardApprovalList)
                    {
                        #region excel style
                        style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Font.FontName = "Calibri";
                        style.Font.FontSize = 10;
                        style.Font.Bold = false;

                        if (iRow % 2 != 0)
                        {
                            style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                        }
                        else
                        {
                            style.Fill.SetPattern(PatternValues.Solid, Color.White, Color.White);
                        }
                        #endregion

                        slDoc.SetCellValue(iRow, 1, approval.ProcessGroup);
                        slDoc.SetCellValue(iRow, 2, approval.GroupCode);
                        slDoc.SetCellValue(iRow, 3, approval.Worker.HasValue ? approval.Worker.Value : 0);
                        slDoc.SetCellValue(iRow, 4, approval.Production.HasValue ? approval.Production.Value : 0);
                        slDoc.SetCellValue(iRow, 5, approval.UpahLain.HasValue ? approval.UpahLain.Value : 0);
                        slDoc.SetCellValue(iRow, 6, approval.A.HasValue ? approval.A.Value : 0);
                        slDoc.SetCellValue(iRow, 7, approval.C.HasValue ? approval.C.Value : 0);
                        slDoc.SetCellValue(iRow, 8, approval.CH.HasValue ? approval.CH.Value : 0);
                        slDoc.SetCellValue(iRow, 9, approval.CT.HasValue ? approval.CT.Value : 0);
                        slDoc.SetCellValue(iRow, 10, approval.I.HasValue ? approval.I.Value : 0);
                        slDoc.SetCellValue(iRow, 11, approval.LL.HasValue ? approval.LL.Value : 0);
                        slDoc.SetCellValue(iRow, 12, approval.LO.HasValue ? approval.LO.Value : 0);
                        slDoc.SetCellValue(iRow, 13, approval.LP.HasValue ? approval.LP.Value : 0);
                        slDoc.SetCellValue(iRow, 14, approval.MO.HasValue ? approval.MO.Value : 0);
                        slDoc.SetCellValue(iRow, 15, approval.PG.HasValue ? approval.PG.Value : 0);
                        slDoc.SetCellValue(iRow, 16, approval.S.HasValue ? approval.S.Value : 0);
                        slDoc.SetCellValue(iRow, 17, approval.SB.HasValue ? approval.SB.Value : 0);
                        slDoc.SetCellValue(iRow, 18, approval.SKR.HasValue ? approval.SKR.Value : 0);
                        slDoc.SetCellValue(iRow, 19, approval.SL4.HasValue ? approval.SL4.Value : 0);
                        slDoc.SetCellValue(iRow, 20, approval.SLP.HasValue ? approval.SLP.Value : 0);
                        slDoc.SetCellValue(iRow, 21, approval.SLS.HasValue ? approval.SLS.Value : 0);
                        slDoc.SetCellValue(iRow, 22, approval.T.HasValue ? approval.T.Value : 0);
                        slDoc.SetCellValue(iRow, 23, approval.TL.HasValue ? approval.TL.Value : 0);
                        slDoc.SetCellStyle(iRow, 1, iRow, 23, style);
                        iRow++;
                    }
                    style.Font.Bold = true;
                    slDoc.SetCellValue(iRow, 1, "Total");
                    slDoc.SetCellValue(iRow, 2, model.TotalGroup.HasValue ? model.TotalGroup.Value : 0);
                    slDoc.SetCellValue(iRow, 3, model.TotalWorker.HasValue ? model.TotalWorker.Value : 0);
                    slDoc.SetCellValue(iRow, 4, model.TotalProduksi.HasValue ? model.TotalProduksi.Value : 0);
                    slDoc.SetCellValue(iRow, 5, model.TotalUpahLain.HasValue ? model.TotalUpahLain.Value : 0);
                    slDoc.SetCellStyle(iRow, 1, iRow, 23, style);
                    iRow++;
                }
                slDoc.SetCellValue(iRow, 1, "Total");
                slDoc.SetCellValue(iRow, 2, modelGroup.Sum(c => c.TotalGroup).ToString());
                slDoc.SetCellValue(iRow, 3, modelGroup.Sum(c => c.TotalWorker).ToString());
                slDoc.SetCellValue(iRow, 4, modelGroup.Sum(c => c.TotalProduksi).ToString());
                slDoc.SetCellValue(iRow, 5, modelGroup.Sum(c => c.TotalUpahLain).ToString());
                slDoc.SetCellStyle(iRow, 1, iRow, 23, style);
                #endregion

                if (brandCodeList != null)
                {
                    iRow = iRow + 3;
                    foreach (var brand in brandCodeList)
                    {
                        var pcGrid = new List<WagesProductionCardApprovalDetailGridViewModel>();

                        var pcList = _plantWagesBll.GetProductionCardApprovalDetail(new GetProductionCardApprovalDetailInput
                        {
                            LocationCode = criteria.LocationCode,
                            UnitCode = criteria.Unit,
                            ProductionDate = criteria.Date,
                            BrandGroupCode = criteria.BrandGroupCode,
                            BrandCode = brand
                        });
                        var avaGroup = pcList.Select(p => p.ProcessGroup).Distinct().ToList();
                        foreach (var group in avaGroup)
                        {
                            var prodCardPerGroup = pcList.Where(p => p.ProcessGroup == group);
                            var prodCardPerGroupViewModel = Mapper.Map<List<WagesProductionCardApprovalDetailViewModel>>(prodCardPerGroup);
                            var prodCardTotalGroup = pcList.Where(p => p.ProcessGroup == group).Count();
                            var prodCardTotalWorker = pcList.Where(p => p.ProcessGroup == group).Select(p => p.Worker).Sum();
                            var prodCardTotalProduksi = pcList.Where(p => p.ProcessGroup == group).Select(p => p.Production).Sum();
                            var prodCardTotalUpahLain = pcList.Where(p => p.ProcessGroup == group).Select(p => p.UpahLain).Sum();
                            var prodCardTotal_A = pcList.Where(p => p.ProcessGroup == group).Select(p => p.A).Sum();
                            var prodCardTotal_C = pcList.Where(p => p.ProcessGroup == group).Select(p => p.C).Sum();
                            var prodCardTotal_CH = pcList.Where(p => p.ProcessGroup == group).Select(p => p.CH).Sum();
                            var prodCardTotal_CT = pcList.Where(p => p.ProcessGroup == group).Select(p => p.CT).Sum();
                            var prodCardTotal_I = pcList.Where(p => p.ProcessGroup == group).Select(p => p.I).Sum();
                            var prodCardTotal_LL = pcList.Where(p => p.ProcessGroup == group).Select(p => p.LL).Sum();
                            var prodCardTotal_LO = pcList.Where(p => p.ProcessGroup == group).Select(p => p.LO).Sum();
                            var prodCardTotal_LP = pcList.Where(p => p.ProcessGroup == group).Select(p => p.LP).Sum();
                            var prodCardTotal_MO = pcList.Where(p => p.ProcessGroup == group).Select(p => p.MO).Sum();
                            var prodCardTotal_PG = pcList.Where(p => p.ProcessGroup == group).Select(p => p.PG).Sum();
                            var prodCardTotal_SS = pcList.Where(p => p.ProcessGroup == group).Select(p => p.S).Sum();
                            var prodCardTotal_SB = pcList.Where(p => p.ProcessGroup == group).Select(p => p.SB).Sum();
                            var prodCardTotal_SKR = pcList.Where(p => p.ProcessGroup == group).Select(p => p.SKR).Sum();
                            var prodCardTotal_SL4 = pcList.Where(p => p.ProcessGroup == group).Select(p => p.SL4).Sum();
                            var prodCardTotal_SLP = pcList.Where(p => p.ProcessGroup == group).Select(p => p.SLP).Sum();
                            var prodCardTotal_SLS = pcList.Where(p => p.ProcessGroup == group).Select(p => p.SLS).Sum();
                            var prodCardTotal_T = pcList.Where(p => p.ProcessGroup == group).Select(p => p.T).Sum();
                            var prodCardTotal_TL = pcList.Where(p => p.ProcessGroup == group).Select(p => p.TL).Sum();

                            var prodCardGridItem = new WagesProductionCardApprovalDetailGridViewModel()
                            {
                                ProdCardApprovalList = prodCardPerGroupViewModel,
                                TotalGroup = prodCardTotalGroup,
                                TotalWorker = prodCardTotalWorker,
                                TotalProduksi = prodCardTotalProduksi,
                                TotalUpahLain = prodCardTotalUpahLain,
                                Total_A = prodCardTotal_A,
                                Total_C = prodCardTotal_C,
                                Total_CH = prodCardTotal_CH,
                                Total_CT = prodCardTotal_CT,
                                Total_I = prodCardTotal_I,
                                Total_LL = prodCardTotal_LL,
                                Total_LO = prodCardTotal_LO,
                                Total_LP = prodCardTotal_LP,
                                Total_MO = prodCardTotal_MO,
                                Total_PG = prodCardTotal_PG,
                                Total_SS = prodCardTotal_SS,
                                Total_SB = prodCardTotal_SB,
                                Total_SKR = prodCardTotal_SKR,
                                Total_SL4 = prodCardTotal_SL4,
                                Total_SLP = prodCardTotal_SLP,
                                Total_SLS = prodCardTotal_SLS,
                                Total_T = prodCardTotal_T,
                                Total_TL = prodCardTotal_TL
                            };

                            pcGrid.Add(prodCardGridItem);
                        }

                        var vmProdCard = Mapper.Map<List<WagesProductionCardApprovalDetailGridViewModel>>(pcGrid);
                        var totalProdCardByBrandCode = GetTotalDetailProc(criteria.LocationCode, criteria.Unit, criteria.Date.ToShortDateString(), brand, criteria.BrandGroupCode);

                        #region header brand code
                        style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Font.FontName = "Calibri";
                        style.Font.FontSize = 12;
                        style.Fill.SetPattern(PatternValues.Solid, Color.Blue, Color.Blue);
                        slDoc.SetCellValue(iRow, 1, brand);
                        iRow++;
                        slDoc.SetCellStyle(iRow, 1, iRow, 23, style);
                        slDoc.SetCellValue(iRow, 1, "Process");
                        slDoc.SetCellValue(iRow, 2, "Group");
                        slDoc.SetCellValue(iRow, 3, "Worker");
                        slDoc.SetCellValue(iRow, 4, "Produksi");
                        slDoc.SetCellValue(iRow, 5, "Upah Lain");
                        slDoc.SetCellValue(iRow, 6, "A");
                        slDoc.SetCellValue(iRow, 7, "C");
                        slDoc.SetCellValue(iRow, 8, "CH");
                        slDoc.SetCellValue(iRow, 9, "CT");
                        slDoc.SetCellValue(iRow, 10, "I");
                        slDoc.SetCellValue(iRow, 11, "LL");
                        slDoc.SetCellValue(iRow, 12, "LO");
                        slDoc.SetCellValue(iRow, 13, "LP");
                        slDoc.SetCellValue(iRow, 14, "MO");
                        slDoc.SetCellValue(iRow, 15, "PG");
                        slDoc.SetCellValue(iRow, 16, "S");
                        slDoc.SetCellValue(iRow, 17, "SB");
                        slDoc.SetCellValue(iRow, 18, "SKR");
                        slDoc.SetCellValue(iRow, 19, "SL4");
                        slDoc.SetCellValue(iRow, 20, "SLP");
                        slDoc.SetCellValue(iRow, 21, "SLS");
                        slDoc.SetCellValue(iRow, 22, "T");
                        slDoc.SetCellValue(iRow, 23, "TL");
                        #endregion

                        iRow++;
                        #region group brand code
                        foreach (var model in vmProdCard)
                        {
                            foreach (var approval in model.ProdCardApprovalList)
                            {
                                #region excel style
                                style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                                style.Font.FontName = "Calibri";
                                style.Font.FontSize = 10;
                                style.Font.Bold = false;

                                if (iRow % 2 == 0)
                                {
                                    style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                                }
                                else
                                {
                                    style.Fill.SetPattern(PatternValues.Solid, Color.White, Color.White);
                                }
                                #endregion

                                slDoc.SetCellValue(iRow, 1, approval.ProcessGroup);
                                slDoc.SetCellValue(iRow, 2, approval.GroupCode);
                                slDoc.SetCellValue(iRow, 3, approval.Worker.HasValue ? approval.Worker.Value : 0);
                                slDoc.SetCellValue(iRow, 4, approval.Production.HasValue ? approval.Production.Value : 0);
                                slDoc.SetCellValue(iRow, 5, approval.UpahLain.HasValue ? approval.UpahLain.Value : 0);
                                slDoc.SetCellValue(iRow, 6, approval.A.HasValue ? approval.A.Value : 0);
                                slDoc.SetCellValue(iRow, 7, approval.C.HasValue ? approval.C.Value : 0);
                                slDoc.SetCellValue(iRow, 8, approval.CH.HasValue ? approval.CH.Value : 0);
                                slDoc.SetCellValue(iRow, 9, approval.CT.HasValue ? approval.CT.Value : 0);
                                slDoc.SetCellValue(iRow, 10, approval.I.HasValue ? approval.I.Value : 0);
                                slDoc.SetCellValue(iRow, 11, approval.LL.HasValue ? approval.LL.Value : 0);
                                slDoc.SetCellValue(iRow, 12, approval.LO.HasValue ? approval.LO.Value : 0);
                                slDoc.SetCellValue(iRow, 13, approval.LP.HasValue ? approval.LP.Value : 0);
                                slDoc.SetCellValue(iRow, 14, approval.MO.HasValue ? approval.MO.Value : 0);
                                slDoc.SetCellValue(iRow, 15, approval.PG.HasValue ? approval.PG.Value : 0);
                                slDoc.SetCellValue(iRow, 16, approval.S.HasValue ? approval.S.Value : 0);
                                slDoc.SetCellValue(iRow, 17, approval.SB.HasValue ? approval.SB.Value : 0);
                                slDoc.SetCellValue(iRow, 18, approval.SKR.HasValue ? approval.SKR.Value : 0);
                                slDoc.SetCellValue(iRow, 19, approval.SL4.HasValue ? approval.SL4.Value : 0);
                                slDoc.SetCellValue(iRow, 20, approval.SLP.HasValue ? approval.SLP.Value : 0);
                                slDoc.SetCellValue(iRow, 21, approval.SLS.HasValue ? approval.SLS.Value : 0);
                                slDoc.SetCellValue(iRow, 22, approval.T.HasValue ? approval.T.Value : 0);
                                slDoc.SetCellValue(iRow, 23, approval.TL.HasValue ? approval.TL.Value : 0);
                                slDoc.SetCellStyle(iRow, 1, iRow, 23, style);
                                iRow++;
                            }
                            style.Font.Bold = true;
                            slDoc.SetCellValue(iRow, 1, "Total");
                            slDoc.SetCellValue(iRow, 2, model.TotalGroup.HasValue ? model.TotalGroup.Value : 0);
                            slDoc.SetCellValue(iRow, 3, model.TotalWorker.HasValue ? model.TotalWorker.Value : 0);
                            slDoc.SetCellValue(iRow, 4, model.TotalProduksi.HasValue ? model.TotalProduksi.Value : 0);
                            slDoc.SetCellValue(iRow, 5, model.TotalUpahLain.HasValue ? model.TotalUpahLain.Value : 0);
                            slDoc.SetCellStyle(iRow, 1, iRow, 23, style);
                            iRow++;
                        }
                        slDoc.SetCellValue(iRow, 1, "Total");
                        slDoc.SetCellValue(iRow, 2, totalProdCardByBrandCode.TotalGroupBrand.HasValue ? totalProdCardByBrandCode.TotalGroupBrand.Value : 0);
                        slDoc.SetCellValue(iRow, 3, totalProdCardByBrandCode.TotalWorkerBrand.HasValue ? totalProdCardByBrandCode.TotalWorkerBrand.Value : 0);
                        slDoc.SetCellValue(iRow, 4, totalProdCardByBrandCode.TotalProduksiBrand.HasValue ? totalProdCardByBrandCode.TotalProduksiBrand.Value : 0);
                        slDoc.SetCellValue(iRow, 5, totalProdCardByBrandCode.TotalUpahLainBrand.HasValue ? totalProdCardByBrandCode.TotalUpahLainBrand.Value : 0);
                        slDoc.SetCellStyle(iRow, 1, iRow, 23, style);
                        #endregion
                        iRow = iRow + 3;
                    }
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
                slDoc.AutoFitColumn(1, 23);
                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "PlantWages_ProductionCardApprovalDetail_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        #region Transaction History and Flow

        ///// <summary>
        ///// Transaction History
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult GetHistory(TransactionLogInput input)
        //{
        //    var pageResult = new PageResult<TransactionHistoryViewModel>();
        //    var transactionLog = _utilitiesBLL.GetTransactionHistory(input);
        //    pageResult.TotalRecords = transactionLog.Count;
        //    pageResult.TotalPages = (transactionLog.Count / input.PageSize) + (transactionLog.Count % input.PageSize != 0 ? 1 : 0);
        //    var result = transactionLog.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
        //    pageResult.Results = Mapper.Map<List<TransactionHistoryViewModel>>(result);
        //    return Json(pageResult);
        //}

        ///// <summary>
        ///// Transaction Flow
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult GetFlow(TransactionLogInput input)
        //{
        //    var pageResult = new PageResult<TransactionFlowViewModel>();
        //    var transactionFlow = _utilitiesBLL.GetTransactionFlow(input);
        //    pageResult.TotalRecords = transactionFlow.Count;
        //    pageResult.TotalPages = (transactionFlow.Count / input.PageSize) + (transactionFlow.Count % input.PageSize != 0 ? 1 : 0);
        //    var result = transactionFlow.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
        //    pageResult.Results = Mapper.Map<List<TransactionFlowViewModel>>(result);
        //    return Json(pageResult);
        //}

        [HttpPost]
        public JsonResult GetHistory(TransactionLogInput input)
        {
            //WPC/ID22/1/2021/1111/FA010783.18/2016/37/5/0
            DateTime enteredDate = DateTime.Parse(input.code_9);
            int day = enteredDate.DayOfWeek == 0 ? 7 : (int)enteredDate.DayOfWeek;
            input.code_5 = "%";
            input.code_7 = enteredDate.Year.ToString();
            input.code_9 = day.ToString();
            input.TransactionDate = enteredDate;
            var pageResult = new PageResult<TransactionHistoryViewModel>();
            var transactionLog = _utilitiesBLL.GetTransactionHistoryWagesApprovalDetail(input);
            pageResult.TotalRecords = transactionLog.Count;
            pageResult.TotalPages = (transactionLog.Count / input.PageSize) + (transactionLog.Count % input.PageSize != 0 ? 1 : 0);
            var result = transactionLog.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            pageResult.Results = Mapper.Map<List<TransactionHistoryViewModel>>(result);
            return Json(pageResult);
        }

        [HttpPost]
        public JsonResult GetFlow(TransactionLogInput input)
        {
            var pageResult = new PageResult<TransactionFlowViewModel>();
            var transactionFlow = _utilitiesBLL.GetTransactionFlowApproval(input);
            pageResult.Results = Mapper.Map<List<TransactionFlowViewModel>>(transactionFlow);
            return Json(pageResult);
            //pageResult.TotalRecords = transactionFlow.Count;
            //pageResult.TotalPages = (transactionFlow.Count / input.PageSize) + (transactionFlow.Count % input.PageSize != 0 ? 1 : 0);
            //var result = transactionFlow.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            //pageResult.Results = Mapper.Map<List<TransactionFlowViewModel>>(result);
            //return Json(pageResult);
        }
        #endregion

    }
}