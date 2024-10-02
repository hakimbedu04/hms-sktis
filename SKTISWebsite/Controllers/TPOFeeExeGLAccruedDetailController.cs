using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Models.TPOFeeExeGLAccrued;
using SpreadsheetLight;
using HMS.SKTIS.BusinessObjects.Inputs;
using SKTISWebsite.Models.TPOFeeExePlanDetail;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeExeGLAccruedDetailController : BaseController
    {
        private readonly ITPOFeeExeGLAccruedBLL _tpoFeeExeGlAccruedBll;
        private IGeneralBLL _generalBLL;
        private ITPOFeeBLL _tpoFeeBll;
        private IMasterDataBLL _masterDataBll;

        public TPOFeeExeGLAccruedDetailController
        (
            ITPOFeeExeGLAccruedBLL tpoFeeExeGlAccruedBll,
            IGeneralBLL GeneralBLL,
            ITPOFeeBLL tpoFeeBll, 
            IMasterDataBLL masterDataBll
        )
        {
            _tpoFeeExeGlAccruedBll = tpoFeeExeGlAccruedBll;
            _generalBLL = GeneralBLL;
            _tpoFeeBll = tpoFeeBll;
            _masterDataBll = masterDataBll;
            SetPage("TPOFee/Execution/TPOFeeGL");

        }

        // GET: TPOFeeExeGLAccruedDetail
        public ActionResult Index(string id)
        {
            //var input = new GetTPOFeeExeGLAccruedInput
            //{
            //    Regional = parameters[0],
            //    KpsYear = Convert.ToInt32(parameters[1]),
            //    KpsWeek = Convert.ToInt32(parameters[2]),
            //    ClosingDate = new DateTime(Convert.ToInt32(dates[2]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[0])),
            //    Location = parameters[4],
            //    Brand = parameters[5]
            //};

            //var result = _tpoFeeExeGlAccruedBll.GetTpoFeeExeGlAccruedDetailDaily(input);
            //var viewModel = Mapper.Map<InitTPOFeeExeGLAccruedDetailViewModel>(result);
            //return View(viewModel);
            //var model = closingDate == genWeek.EndDate ? CreateInitTpoFeeExePlanDetailViewModelNew(id) : CreateInitTPOFeeExePlanDetailViewModel(id);
            var model = CreateInitTpoFeeExePlanDetailViewModelNew(id);

            ViewBag.Message = TempData["Message"];

            //Back To last list filter
            TempData["year1"] = TempData["Year"];
            TempData["week1"] = TempData["week"];
            TempData["regional1"] = TempData["regional"];

            return View("Index", model);
        }
        #region kode lama , digunakan jika view kurang dari satu minggu
        private InitTPOFeeExePlanDetailViewModel CreateInitTPOFeeExePlanDetailViewModel(string id)
        {
            var parameters = id.Split('$');
            var dates = parameters[3].Split('-');

            var GenWeek = _masterDataBll.GetWeekByDate(DateTime.Parse(parameters[3]));

            string feeCode = "FEE/" + parameters[4] + "/" + parameters[5] + "/" + parameters[1] + "/" + GenWeek.Week;

            var tpoFeeHdrPlan = _tpoFeeBll.GetTpoFeeHdrPlan(feeCode);
            var locationTpo = _masterDataBll.GetLocation(tpoFeeHdrPlan.LocationCode);
            var location = _masterDataBll.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
            {
                ParentLocationCode = tpoFeeHdrPlan.LocationCode
            }).FirstOrDefault();
            var regional = _masterDataBll.GetLocation(location == null ? string.Empty : location.ParentLocationCode);
            var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrPlan.BrandGroupCode);
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(tpoFeeHdrPlan.KPSYear, tpoFeeHdrPlan.KPSWeek);
            var mstTpoPackage = _masterDataBll.GetTpoPackage(new GetMstTPOPackagesInput
            {
                LocationCode = tpoFeeHdrPlan.LocationCode,
                BrandGroupCode = tpoFeeHdrPlan.BrandGroupCode,
                StartDate = mstGenWeek.StartDate,
                EndDate = mstGenWeek.EndDate
            });

            var mstTpoInfo = _masterDataBll.GetMstTpoInfo(tpoFeeHdrPlan.LocationCode);

            var model = new InitTPOFeeExePlanDetailViewModel();

            model.ClosingDate = new DateTime(Convert.ToInt32(dates[2]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[0]));
            model.TpoFeeCode = id;
            model.Regional = regional.LocationCode;
            model.RegionalName = regional.LocationName;
            model.Location = locationTpo.LocationCode;
            model.LocationName = locationTpo.LocationName;
            model.LocationCompat = locationTpo.LocationCode + "-" + locationTpo.LocationName;
            model.CostCenter = locationTpo.CostCenter;
            model.Brand = mstGenBrandGroup.SKTBrandCode;
            model.StickPerBox = mstGenBrandGroup.StickPerBox;
            model.KpsYear = tpoFeeHdrPlan.KPSYear;
            model.KpsWeek = tpoFeeHdrPlan.KPSWeek;
            model.PageFrom = "accrued";
            model.Paket = mstTpoPackage == null ? null : mstTpoPackage.Package;
            model.TpoFeeProductionDailyPlanModels =
                Mapper.Map<List<TpoFeeProductionDailyPlanModel>>(tpoFeeHdrPlan.TpoFeeProductionDailyPlans);
            foreach (var daily in model.TpoFeeProductionDailyPlanModels)
            {
                if (daily.FeeDate > model.ClosingDate)
                {
                    daily.JKN = 0;
                    daily.JL1 = 0;
                    daily.Jl2 = 0;
                    daily.Jl3 = 0;
                    daily.Jl4 = 0;
                    daily.OutputBox = 0;
                    daily.OuputSticks = 0;
                    daily.JKNJam = 0;
                    daily.JL1Jam = 0;
                    daily.JL2Jam = 0;
                    daily.JL3Jam = 0;
                    daily.JL4Jam = 0;
                }
                else
                {
                    daily.JKN = Math.Round(daily.JKN.GetValueOrDefault(), 3);
                    daily.JL1 = Math.Round(daily.JL1.GetValueOrDefault(), 3);
                    daily.Jl2 = Math.Round(daily.Jl2.GetValueOrDefault(), 3);
                }
            }

            model.TotalProductionStick = model.TpoFeeProductionDailyPlanModels.Sum(p => p.OuputSticks);
            model.TotalProductionBox = model.TpoFeeProductionDailyPlanModels.Sum(p => p.OutputBox);
            model.TotalProductionJkn = model.TpoFeeProductionDailyPlanModels.Sum(p => p.JKN);
            model.TotalProductionJl1 = model.TpoFeeProductionDailyPlanModels.Sum(p => p.JL1);
            model.TotalProductionJl2 = model.TpoFeeProductionDailyPlanModels.Sum(p => p.Jl2);
            model.TotalProductionJl3 = model.TpoFeeProductionDailyPlanModels.Sum(p => p.Jl3);
            model.TotalProductionJl4 = model.TpoFeeProductionDailyPlanModels.Sum(p => p.Jl4);

            #region total dibayar calculation
            var sumJknJam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JKNJam) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.JKNJam) : 0;
            var sumJl1Jam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL1Jam) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL1Jam) : 0;
            var sumJl2Jam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL2Jam) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL2Jam) : 0;
            var sumJl3Jam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL3Jam) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL3Jam) : 0;
            var sumJl4Jam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL4Jam) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL4Jam) : 0;

            var sumBox = model.TpoFeeProductionDailyPlanModels.Sum(c => c.OutputBox) != null ? (int)model.TpoFeeProductionDailyPlanModels.Sum(c => c.OutputBox) : 0;
            var sumJkn = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JKN) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.JKN) : 0;
            var sumJl1 = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL1) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL1) : 0;
            var sumJl2 = model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl2) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl2) : 0;
            var sumJl3 = model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl3) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl3) : 0;
            var sumJl4 = model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl4) != null ? model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl4) : 0;

            model.TotalDibayarJKN = 0;
            model.TotalDibayarJL1 = 0;
            model.TotalDibayarJL2 = 0;
            model.TotalDibayarJL3 = 0;
            model.TotalDibayarJL4 = 0;

            var paket = mstTpoPackage == null ? 0 : mstTpoPackage.Package;
            var mstGenBrand = _masterDataBll.GetBrandGroupById(tpoFeeHdrPlan.BrandGroupCode); // _mstGenBrandGroupRepo.GetByID(dto.Brand);
            var stickPerBox = mstGenBrand.StickPerBox ?? 0;
            var empPackage = mstGenBrand.EmpPackage;
            var stdPerHour = _masterDataBll.GetStdPerhour(new HMS.SKTIS.BusinessObjects.Inputs.GetMstGenProcessSettingLocationInput
            {
                LocationCode = tpoFeeHdrPlan.LocationCode,
                BrandGroupCode = tpoFeeHdrPlan.BrandGroupCode,
                ProcessGroup = HMS.SKTIS.Core.Enums.Process.Rolling.ToString()
            });


            if (stickPerBox != 0)
            {
                model.TotalDibayarJKN = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJknJam) / stickPerBox), 0);
                if ((sumJkn + sumJl1 + sumJl2 - sumJl3 - sumJl4) <= model.TotalDibayarJKN)
                {
                    model.TotalDibayarJKN = (float)(sumJkn + sumJl1 + sumJl2 - sumJl3 - sumJl4);
                }

                model.TotalDibayarJL1 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox), 0);
                if ((sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - sumJl3 - sumJl4) > 0)
                {
                    if ((sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - sumJl3 - sumJl4) <= model.TotalDibayarJL1)
                    {
                        model.TotalDibayarJL1 = (float)Math.Round((Decimal)(sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - sumJl3 - sumJl4),0);
                    }
                }
                else
                {
                    model.TotalDibayarJL1 = 0;
                }

                model.TotalDibayarJL2 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl2Jam) / stickPerBox), 0);
                if ((sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - model.TotalDibayarJL1 - sumJl3 - sumJl4) > 0)
                {
                    model.TotalDibayarJL2 = (float)Math.Round((Decimal)(sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - model.TotalDibayarJL1 - sumJl3 - sumJl4),0);
                }
                else
                {
                    model.TotalDibayarJL2 = 0;
                }

                var standard60 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * (sumJkn + sumJl1 + sumJl2)) / stickPerBox), 0);

                model.TotalDibayarJL3 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl3Jam) / stickPerBox), 0);
                if ((sumBox - standard60) <= model.TotalDibayarJL3)
                {
                    if (sumBox - standard60 > 0)
                    {
                        model.TotalDibayarJL3 = sumBox - standard60;
                    }
                    else
                    {
                        model.TotalDibayarJL3 = 0;
                    }
                }

                model.TotalDibayarJL4 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl4Jam) / stickPerBox), 0);
                if (sumBox - standard60 - model.TotalDibayarJL3 > 0)
                {
                    model.TotalDibayarJL4 = sumBox - standard60 - model.TotalDibayarJL3;
                }
                else
                {
                    model.TotalDibayarJL4 = 0;
                }
            }

            #endregion

            model.VendorName = mstTpoInfo.VendorName;
            model.BankAccountNumber = mstTpoInfo.BankAccountNumber;
            model.BankType = mstTpoInfo.BankType;
            model.BankBranch = mstTpoInfo.BankBranch;
            model.PreparedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SLT.ToString(), regional.LocationCode);
            model.ApprovedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.RM.ToString(), regional.LocationCode);
            model.AuthorizedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SKTHD.ToString(), regional.LocationCode);

            model.Calculations = _tpoFeeBll.GetTpoFeeCalculationPlan(feeCode);
            
            #region clculation
            //var feeRate = _masterDataBll.GetTPOFeeRateByExpiredDate(new HMS.SKTIS.BusinessObjects.Inputs.MstTPOFeeRateInput 
            //    {
            //        LocationCode = tpoFeeHdrPlan.LocationCode,
            //        BrandGroupCode = tpoFeeHdrPlan.BrandGroupCode,
            //        ExpiredDate = model.ClosingDate,
            //        StartDate = mstGenWeek.StartDate.Value
            //    }).FirstOrDefault();
            var feeRate = _masterDataBll.GetTPOFeeRateByExpiredDate(new HMS.SKTIS.BusinessObjects.Inputs.MstTPOFeeRateInput
            {
                LocationCode = tpoFeeHdrPlan.LocationCode,
                BrandGroupCode = tpoFeeHdrPlan.BrandGroupCode,
                ExpiredDate = model.ClosingDate,
                StartDate = mstGenWeek.StartDate.Value
            }).OrderByDescending(c => c.ExpiredDate).FirstOrDefault();

            // Prevent Null reference exception when MstFeeRate is not existed yet.
            // Remove this when you figure out something else!
            if (feeRate == null) return model;

            foreach (var cal in model.Calculations)
            {
                if (cal.OrderFeeType == 1)
                {
                    cal.OutputProduction = model.TotalDibayarJKN;
                    cal.OutputBiaya = (float)feeRate.JKN;
                    cal.Calculate = cal.OutputBiaya * cal.OutputProduction;
                }
                if (cal.OrderFeeType == 2)
                {
                    cal.OutputProduction = (float)model.TotalDibayarJL1;
                    cal.OutputBiaya = (float)feeRate.Jl1;
                    cal.Calculate = cal.OutputBiaya * cal.OutputProduction;
                }
                if (cal.OrderFeeType == 3)
                {
                    cal.OutputProduction = model.TotalDibayarJL2;
                    cal.OutputBiaya = (float)feeRate.Jl2;
                    cal.Calculate = cal.OutputBiaya * cal.OutputProduction;
                }
                if (cal.OrderFeeType == 4)
                {
                    cal.OutputProduction = model.TotalDibayarJL3;
                    cal.OutputBiaya = (float)feeRate.Jl3;
                    cal.Calculate = cal.OutputBiaya * cal.OutputProduction;
                }
                if (cal.OrderFeeType == 5)
                {
                    cal.OutputProduction = model.TotalDibayarJL4;
                    cal.OutputBiaya = (float)feeRate.Jl4;
                    cal.Calculate = cal.OutputBiaya * cal.OutputProduction;
                }
                if (cal.OrderFeeType == 6) // masukin jl1,2,3,4
                {
                    //cal.OutputProduction = model.TotalProductionBox;
                    cal.OutputProduction = model.TotalDibayarJKN + model.TotalDibayarJL1 + model.TotalDibayarJL2;
                    cal.OutputBiaya = (float)feeRate.JKN;
                    //cal.Calculate = cal.OutputBiaya * cal.OutputProduction;
                    cal.Calculate = (cal.OutputBiaya * model.TotalDibayarJKN) + (model.TotalDibayarJL1 * (float)feeRate.Jl1) + (model.TotalDibayarJL2 * (float)feeRate.Jl2) + (model.TotalDibayarJL3 * (float)feeRate.Jl3) + (model.TotalDibayarJL4 * (float)feeRate.Jl4);
                }
                if (cal.OrderFeeType == 7)
                {
                    cal.OutputProduction = model.TotalProductionBox;
                    cal.OutputBiaya = (float)feeRate.ManagementFee;
                    cal.Calculate = cal.OutputBiaya * cal.OutputProduction;
                }
                if (cal.OrderFeeType == 8)
                {
                    //cal.Calculate = ((model.TotalProductionBox * (float)feeRate.JKN) + (model.TotalProductionBox * (float)feeRate.ManagementFee)) - (model.TotalProductionBox * (float)feeRate.ProductivityIncentives);
                    cal.Calculate = ((((float)feeRate.JKN * model.TotalDibayarJKN) + (model.TotalDibayarJL1 * (float)feeRate.Jl1) + (model.TotalDibayarJL2 * (float)feeRate.Jl2) + (model.TotalDibayarJL3 * (float)feeRate.Jl3) + (model.TotalDibayarJL4 * (float)feeRate.Jl4))
                        + (model.TotalProductionBox * (float)feeRate.ManagementFee));

                }
                if (cal.OrderFeeType == 9)
                {
                    cal.Calculate = (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 2) / 100);
                }
                if (cal.OrderFeeType == 10)
                {
                    //cal.Calculate = (((model.TotalProductionBox * (float)feeRate.JKN) + (model.TotalProductionBox * (float)feeRate.ManagementFee)) - (model.TotalProductionBox * (float)feeRate.ProductivityIncentives) - (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 2) / 100));
                    cal.Calculate = ((((float) feeRate.JKN*model.TotalDibayarJKN) +
                                      (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                      (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                      (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                      (model.TotalDibayarJL4*(float) feeRate.Jl4))
                                     + (model.TotalProductionBox*(float) feeRate.ManagementFee)) -
                                    (((model.TotalProductionBox*(float) feeRate.ManagementFee)*2)/100);
                }
                if (cal.OrderFeeType == 11)
                {
                    //cal.Calculate = (((model.TotalProductionBox * (float)feeRate.JKN) + (model.TotalProductionBox * (float)feeRate.ManagementFee)) - (model.TotalProductionBox * (float)feeRate.ProductivityIncentives) - (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 2) / 100));
                    cal.Calculate = ((((float)feeRate.JKN * model.TotalDibayarJKN) +
                                      (model.TotalDibayarJL1 * (float)feeRate.Jl1) +
                                      (model.TotalDibayarJL2 * (float)feeRate.Jl2) +
                                      (model.TotalDibayarJL3 * (float)feeRate.Jl3) +
                                      (model.TotalDibayarJL4 * (float)feeRate.Jl4))
                                     + (model.TotalProductionBox * (float)feeRate.ManagementFee)) -
                                    (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 2) / 100);
                }
                if (cal.OrderFeeType == 13)
                {
                    cal.Calculate = ((((model.TotalDibayarJKN * (float)feeRate.JKN) + (model.TotalDibayarJL1 * (float)feeRate.Jl1) + (model.TotalDibayarJL2 * (float)feeRate.Jl2) + (model.TotalDibayarJL3 * (float)feeRate.Jl3) + (model.TotalDibayarJL4 * (float)feeRate.Jl4)) * 10) / 100);
                    //cal.Calculate = (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 10) / 100);
                }
                if (cal.OrderFeeType == 14)
                {
                    cal.Calculate = (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 10) / 100);
                    //cal.Calculate = (((model.TotalProductionBox * (float)feeRate.JKN) * 10) / 100);
                }
                if (cal.OrderFeeType == 15)
                {
                    //cal.Calculate = ((((model.TotalProductionBox * (float)feeRate.JKN) + (model.TotalProductionBox * (float)feeRate.ManagementFee) - (model.TotalProductionBox * (float)feeRate.ProductivityIncentives)) - (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 2) / 100)) + ((((model.TotalProductionBox * (float)feeRate.ManagementFee) * 10) / 100) + (((model.TotalProductionBox * (float)feeRate.JKN) * 10) / 100)));
                    cal.Calculate = ((((float) feeRate.JKN*model.TotalDibayarJKN) +
                                      (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                      (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                      (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                      (model.TotalDibayarJL4*(float) feeRate.Jl4))
                                     + (model.TotalProductionBox*(float) feeRate.ManagementFee)) -
                                    (((model.TotalProductionBox*(float) feeRate.ManagementFee)*2)/100)
                                    +
                                    ((((model.TotalDibayarJKN*(float) feeRate.JKN) +
                                       (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                       (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                       (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                       (model.TotalDibayarJL4*(float) feeRate.Jl4))*10)/100)
                                    +
                                    (((model.TotalProductionBox*(float) feeRate.ManagementFee)*10)/100);
                }
                cal.Calculate = Math.Floor(cal.Calculate.GetValueOrDefault());
            }
            #endregion

            model.id = id;
            return model;
        }
        #endregion 

        private InitTPOFeeExePlanDetailViewModel CreateInitTpoFeeExePlanDetailViewModelNew(string id)
        {
            var parameters = id.Split('$');
            var dates = parameters[3].Split('-');

            var genWeek = _masterDataBll.GetWeekByDate(DateTime.Parse(parameters[3]));

            string feeCode = "FEE/" + parameters[4] + "/" + parameters[5] + "/" + parameters[1] + "/" + genWeek.Week;

            var tpoFeeHdrPlan = _tpoFeeBll.GetTpoFeeHdrPlan(feeCode);
            var locationTpo = _masterDataBll.GetLocation(tpoFeeHdrPlan.LocationCode);
            var location = _masterDataBll.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
            {
                ParentLocationCode = tpoFeeHdrPlan.LocationCode
            }).FirstOrDefault();
            var regional = _masterDataBll.GetLocation(location == null ? string.Empty : location.ParentLocationCode);
            var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrPlan.BrandGroupCode);
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(tpoFeeHdrPlan.KPSYear, tpoFeeHdrPlan.KPSWeek);
            var mstTpoPackage = _masterDataBll.GetTpoPackage(new GetMstTPOPackagesInput
            {
                LocationCode = tpoFeeHdrPlan.LocationCode,
                BrandGroupCode = tpoFeeHdrPlan.BrandGroupCode,
                StartDate = mstGenWeek.StartDate,
                EndDate = mstGenWeek.EndDate
            });

            var mstTpoInfo = _masterDataBll.GetMstTpoInfo(tpoFeeHdrPlan.LocationCode);

            var model = new InitTPOFeeExePlanDetailViewModel();

            model.ClosingDate = new DateTime(Convert.ToInt32(dates[2]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[0]));
            model.TpoFeeCode = id;
            model.Regional = regional.LocationCode;
            model.RegionalName = regional.LocationName;
            model.Location = locationTpo.LocationCode;
            model.LocationName = locationTpo.LocationName;
            model.LocationCompat = locationTpo.LocationCode + "-" + locationTpo.LocationName;
            model.CostCenter = locationTpo.CostCenter;
            model.Brand = mstGenBrandGroup.SKTBrandCode;
            model.StickPerBox = mstGenBrandGroup.StickPerBox;
            model.KpsYear = tpoFeeHdrPlan.KPSYear;
            model.KpsWeek = tpoFeeHdrPlan.KPSWeek;
            model.PageFrom = "accrued";
            model.Paket = mstTpoPackage == null ? null : mstTpoPackage.Package;
            model.TpoFeeProductionDailyPlanModels =
                Mapper.Map<List<TpoFeeProductionDailyPlanModel>>(tpoFeeHdrPlan.TpoFeeProductionDailyPlans);
            foreach (var daily in model.TpoFeeProductionDailyPlanModels)
            {
                if (daily.FeeDate > model.ClosingDate)
                {
                    daily.JKN = 0;
                    daily.JL1 = 0;
                    daily.Jl2 = 0;
                    daily.Jl3 = 0;
                    daily.Jl4 = 0;
                    daily.OutputBox = 0;
                    daily.OuputSticks = 0;
                    daily.JKNJam = 0;
                    daily.JL1Jam = 0;
                    daily.JL2Jam = 0;
                    daily.JL3Jam = 0;
                    daily.JL4Jam = 0;
                }
                else
                {
                    daily.JKN = Math.Round(daily.JKN.GetValueOrDefault(), 3);
                    daily.JL1 = Math.Round(daily.JL1.GetValueOrDefault(), 3);
                    daily.Jl2 = Math.Round(daily.Jl2.GetValueOrDefault(), 3);
                }
            }

            model.VendorName = mstTpoInfo.VendorName;
            model.BankAccountNumber = mstTpoInfo.BankAccountNumber;
            model.BankType = mstTpoInfo.BankType;
            model.BankBranch = mstTpoInfo.BankBranch;
            model.PreparedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SLT.ToString(), regional.LocationCode);
            model.ApprovedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.RM.ToString(), regional.LocationCode);
            model.AuthorizedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SKTHD.ToString(), regional.LocationCode);

            model.TotalProductionStick = model.TpoFeeProductionDailyPlanModels.Sum(p => p.OuputSticks);
            model.TotalProductionBox = model.TpoFeeProductionDailyPlanModels.Sum(p => p.OutputBox);
            model.TotalProductionJkn = model.TpoFeeProductionDailyPlanModels.Sum(p => p.JKN);
            model.TotalProductionJl1 = model.TpoFeeProductionDailyPlanModels.Sum(p => p.JL1);
            model.TotalProductionJl2 = model.TpoFeeProductionDailyPlanModels.Sum(p => p.Jl2);
            model.TotalProductionJl3 = model.TpoFeeProductionDailyPlanModels.Sum(p => p.Jl3);
            model.TotalProductionJl4 = model.TpoFeeProductionDailyPlanModels.Sum(p => p.Jl4);

            model.TotalDibayarJKN = tpoFeeHdrPlan.TotalProdJKN;
            model.TotalDibayarJL1 = tpoFeeHdrPlan.TotalProdJl1;
            model.TotalDibayarJL2 = tpoFeeHdrPlan.TotalProdJl2;
            model.TotalDibayarJL3 = tpoFeeHdrPlan.TotalProdJl3;
            model.TotalDibayarJL4 = tpoFeeHdrPlan.TotalProdJl4;

            model.Calculations = _tpoFeeBll.GetTpoFeeCalculationPlan(feeCode);
            // hitung ulang untuk datafeehdrplan dan calculation plan jika kurang dari satu minggu
            if (model.ClosingDate != genWeek.EndDate)
            {
                #region total dibayar calculation
                var sumJknJam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JKNJam) ?? 0;
                var sumJl1Jam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL1Jam) ?? 0;
                var sumJl2Jam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL2Jam) ?? 0;
                var sumJl3Jam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL3Jam) ?? 0;
                var sumJl4Jam = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL4Jam) ?? 0;

                var sumBox = model.TpoFeeProductionDailyPlanModels.Sum(c => c.OutputBox) != null ? (int)model.TpoFeeProductionDailyPlanModels.Sum(c => c.OutputBox) : 0;
                var sumJkn = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JKN) ?? 0;
                var sumJl1 = model.TpoFeeProductionDailyPlanModels.Sum(c => c.JL1) ?? 0;
                var sumJl2 = model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl2) ?? 0;
                var sumJl3 = model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl3) ?? 0;
                var sumJl4 = model.TpoFeeProductionDailyPlanModels.Sum(c => c.Jl4) ?? 0;

                model.TotalDibayarJKN = 0;
                model.TotalDibayarJL1 = 0;
                model.TotalDibayarJL2 = 0;
                model.TotalDibayarJL3 = 0;
                model.TotalDibayarJL4 = 0;

                var paket = mstTpoPackage == null ? 0 : mstTpoPackage.Package;
                var mstGenBrand = _masterDataBll.GetBrandGroupById(tpoFeeHdrPlan.BrandGroupCode); // _mstGenBrandGroupRepo.GetByID(dto.Brand);
                var stickPerBox = mstGenBrand.StickPerBox ?? 0;
                var empPackage = mstGenBrand.EmpPackage;
                var stdPerHour = _masterDataBll.GetStdPerhour(new GetMstGenProcessSettingLocationInput
                {
                    LocationCode = tpoFeeHdrPlan.LocationCode,
                    BrandGroupCode = tpoFeeHdrPlan.BrandGroupCode,
                    ProcessGroup = Enums.Process.Rolling.ToString()
                });


                if (stickPerBox != 0)
                {
                    model.TotalDibayarJKN = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJknJam) / stickPerBox), 0);
                    if ((sumJkn + sumJl1 + sumJl2 - sumJl3 - sumJl4) <= model.TotalDibayarJKN)
                    {
                        model.TotalDibayarJKN = (float)(sumJkn + sumJl1 + sumJl2 - sumJl3 - sumJl4);
                    }

                    model.TotalDibayarJL1 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox), 0);
                    if ((sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - sumJl3 - sumJl4) > 0)
                    {
                        if ((sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - sumJl3 - sumJl4) <= model.TotalDibayarJL1)
                        {
                            model.TotalDibayarJL1 = (float)Math.Round((Decimal)(sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - sumJl3 - sumJl4), 0);
                        }
                    }
                    else
                    {
                        model.TotalDibayarJL1 = 0;
                    }

                    model.TotalDibayarJL2 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl2Jam) / stickPerBox), 0);
                    if ((sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - model.TotalDibayarJL1 - sumJl3 - sumJl4) > 0)
                    {
                        model.TotalDibayarJL2 = (float)Math.Round((Decimal)(sumJkn + sumJl1 + sumJl2 - model.TotalDibayarJKN - model.TotalDibayarJL1 - sumJl3 - sumJl4), 0);
                    }
                    else
                    {
                        model.TotalDibayarJL2 = 0;
                    }

                    var standard60 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * (sumJkn + sumJl1 + sumJl2)) / stickPerBox), 0);

                    model.TotalDibayarJL3 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl3Jam) / stickPerBox), 0);
                    if ((sumBox - standard60) <= model.TotalDibayarJL3)
                    {
                        if (sumBox - standard60 > 0)
                        {
                            model.TotalDibayarJL3 = sumBox - standard60;
                        }
                        else
                        {
                            model.TotalDibayarJL3 = 0;
                        }
                    }

                    model.TotalDibayarJL4 = (float)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl4Jam) / stickPerBox), 0);
                    if (sumBox - standard60 - model.TotalDibayarJL3 > 0)
                    {
                        model.TotalDibayarJL4 = sumBox - standard60 - model.TotalDibayarJL3;
                    }
                    else
                    {
                        model.TotalDibayarJL4 = 0;
                    }
                }

                #endregion
                #region calculation

                if (mstGenWeek.StartDate != null)
                {
                    var feeRate =
                        _masterDataBll.GetTPOFeeRateByExpiredDate(new MstTPOFeeRateInput
                        {
                            LocationCode = tpoFeeHdrPlan.LocationCode,
                            BrandGroupCode = tpoFeeHdrPlan.BrandGroupCode,
                            ExpiredDate = model.ClosingDate,
                            StartDate = mstGenWeek.StartDate.Value
                        }).OrderByDescending(c => c.ExpiredDate).FirstOrDefault();

                    // Prevent Null reference exception when MstFeeRate is not existed yet.
                    // Remove this when you figure out something else!
                    if (feeRate == null) return model;

                    foreach (var cal in model.Calculations)
                    {
                        if (cal.OrderFeeType == 1)
                        {
                            cal.OutputProduction = model.TotalDibayarJKN;
                            cal.OutputBiaya = (float) feeRate.JKN;
                            cal.Calculate = cal.OutputBiaya*cal.OutputProduction;
                        }
                        if (cal.OrderFeeType == 2)
                        {
                            if (model.TotalDibayarJL1 != null) cal.OutputProduction = (float) model.TotalDibayarJL1;
                            cal.OutputBiaya = (float) feeRate.Jl1;
                            cal.Calculate = cal.OutputBiaya*cal.OutputProduction;
                        }
                        if (cal.OrderFeeType == 3)
                        {
                            cal.OutputProduction = model.TotalDibayarJL2;
                            cal.OutputBiaya = (float) feeRate.Jl2;
                            cal.Calculate = cal.OutputBiaya*cal.OutputProduction;
                        }
                        if (cal.OrderFeeType == 4)
                        {
                            cal.OutputProduction = model.TotalDibayarJL3;
                            cal.OutputBiaya = (float) feeRate.Jl3;
                            cal.Calculate = cal.OutputBiaya*cal.OutputProduction;
                        }
                        if (cal.OrderFeeType == 5)
                        {
                            cal.OutputProduction = model.TotalDibayarJL4;
                            cal.OutputBiaya = (float) feeRate.Jl4;
                            cal.Calculate = cal.OutputBiaya*cal.OutputProduction;
                        }
                        if (cal.OrderFeeType == 6) // masukin jl1,2,3,4
                        {
                            //cal.OutputProduction = model.TotalProductionBox;
                            cal.OutputProduction = model.TotalDibayarJKN + model.TotalDibayarJL1 + model.TotalDibayarJL2;
                            cal.OutputBiaya = (float) feeRate.JKN;
                            //cal.Calculate = cal.OutputBiaya * cal.OutputProduction;
                            cal.Calculate = (cal.OutputBiaya*model.TotalDibayarJKN) +
                                            (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                            (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                            (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                            (model.TotalDibayarJL4*(float) feeRate.Jl4);
                        }
                        if (cal.OrderFeeType == 7)
                        {
                            cal.OutputProduction = model.TotalProductionBox;
                            cal.OutputBiaya = (float) feeRate.ManagementFee;
                            cal.Calculate = cal.OutputBiaya*cal.OutputProduction;
                        }
                        if (cal.OrderFeeType == 8)
                        {
                            //cal.Calculate = ((model.TotalProductionBox * (float)feeRate.JKN) + (model.TotalProductionBox * (float)feeRate.ManagementFee)) - (model.TotalProductionBox * (float)feeRate.ProductivityIncentives);
                            cal.Calculate = ((((float) feeRate.JKN*model.TotalDibayarJKN) +
                                              (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                              (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                              (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                              (model.TotalDibayarJL4*(float) feeRate.Jl4))
                                             + (model.TotalProductionBox*(float) feeRate.ManagementFee));

                        }
                        if (cal.OrderFeeType == 9)
                        {
                            cal.Calculate = (((model.TotalProductionBox*(float) feeRate.ManagementFee)*2)/100);
                        }
                        if (cal.OrderFeeType == 10)
                        {
                            //cal.Calculate = (((model.TotalProductionBox * (float)feeRate.JKN) + (model.TotalProductionBox * (float)feeRate.ManagementFee)) - (model.TotalProductionBox * (float)feeRate.ProductivityIncentives) - (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 2) / 100));
                            cal.Calculate = ((((float) feeRate.JKN*model.TotalDibayarJKN) +
                                              (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                              (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                              (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                              (model.TotalDibayarJL4*(float) feeRate.Jl4))
                                             + (model.TotalProductionBox*(float) feeRate.ManagementFee)) -
                                            (((model.TotalProductionBox*(float) feeRate.ManagementFee)*2)/100);
                        }
                        if (cal.OrderFeeType == 11)
                        {
                            //cal.Calculate = (((model.TotalProductionBox * (float)feeRate.JKN) + (model.TotalProductionBox * (float)feeRate.ManagementFee)) - (model.TotalProductionBox * (float)feeRate.ProductivityIncentives) - (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 2) / 100));
                            cal.Calculate = ((((float) feeRate.JKN*model.TotalDibayarJKN) +
                                              (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                              (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                              (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                              (model.TotalDibayarJL4*(float) feeRate.Jl4))
                                             + (model.TotalProductionBox*(float) feeRate.ManagementFee)) -
                                            (((model.TotalProductionBox*(float) feeRate.ManagementFee)*2)/100);
                        }
                        if (cal.OrderFeeType == 13)
                        {
                            cal.Calculate = ((((model.TotalDibayarJKN*(float) feeRate.JKN) +
                                               (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                               (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                               (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                               (model.TotalDibayarJL4*(float) feeRate.Jl4))*10)/100);
                            //cal.Calculate = (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 10) / 100);
                        }
                        if (cal.OrderFeeType == 14)
                        {
                            cal.Calculate = (((model.TotalProductionBox*(float) feeRate.ManagementFee)*10)/100);
                            //cal.Calculate = (((model.TotalProductionBox * (float)feeRate.JKN) * 10) / 100);
                        }
                        if (cal.OrderFeeType == 15)
                        {
                            //cal.Calculate = ((((model.TotalProductionBox * (float)feeRate.JKN) + (model.TotalProductionBox * (float)feeRate.ManagementFee) - (model.TotalProductionBox * (float)feeRate.ProductivityIncentives)) - (((model.TotalProductionBox * (float)feeRate.ManagementFee) * 2) / 100)) + ((((model.TotalProductionBox * (float)feeRate.ManagementFee) * 10) / 100) + (((model.TotalProductionBox * (float)feeRate.JKN) * 10) / 100)));
                            cal.Calculate = ((((float) feeRate.JKN*model.TotalDibayarJKN) +
                                              (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                              (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                              (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                              (model.TotalDibayarJL4*(float) feeRate.Jl4))
                                             + (model.TotalProductionBox*(float) feeRate.ManagementFee)) -
                                            (((model.TotalProductionBox*(float) feeRate.ManagementFee)*2)/100)
                                            +
                                            ((((model.TotalDibayarJKN*(float) feeRate.JKN) +
                                               (model.TotalDibayarJL1*(float) feeRate.Jl1) +
                                               (model.TotalDibayarJL2*(float) feeRate.Jl2) +
                                               (model.TotalDibayarJL3*(float) feeRate.Jl3) +
                                               (model.TotalDibayarJL4*(float) feeRate.Jl4))*10)/100)
                                            +
                                            (((model.TotalProductionBox*(float) feeRate.ManagementFee)*10)/100);
                        }
                        cal.Calculate = Math.Floor(cal.Calculate.GetValueOrDefault());
                    }
                }

                #endregion
            }

            model.id = id;
            return model;
        }


        [HttpPost]
        public JsonResult Submit(GetTPOFeeExeGLAccruedInput input)
        {
            try
            {
                var transactionInput = new TransactionLogInput()
                {
                    page = Enums.PageName.TPOFeeGL.ToString(),
                    ActionButton = Enums.ButtonName.Submit.ToString(),
                    UserName = GetUserName(),
                    TransactionCode = "FEE/" + input.Location + "/" + input.Brand + "/" + input.KpsYear + "/" + input.KpsWeek,
                    ActionTime = DateTime.Now,
                    TransactionDate = DateTime.Now,
                    Message = input.SubmitMessage,
                    IDRole = CurrentUser.Responsibility.Role
                };

                _generalBLL.ExeTransactionLog(transactionInput);

                return Json("Run Submit data on background process.");
            } catch (Exception e)
            {
                return Json("Failed to run Submit data on background process." + e.InnerException.Message);
            }
            
            return Json(input);
        }

        [HttpPost]
        public FileStreamResult GenerateExcelDetail(GetTPOFeeExeGLAccruedInput input)
        {
            var model = CreateInitTpoFeeExePlanDetailViewModelNew(input.param1);

            var tpoFeeExeGLDatas = _tpoFeeExeGlAccruedBll.GetTpoFeeExeGlAccruedDetailDaily(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeExeGLAccruedDetail + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 2, ": " + tpoFeeExeGLDatas.Regional);
                slDoc.SetCellValue(4, 2, ": " + tpoFeeExeGLDatas.Location);
                slDoc.SetCellValue(5, 2, ": " + tpoFeeExeGLDatas.CostCenter);
                slDoc.SetCellValue(6, 2, ": " + tpoFeeExeGLDatas.Brand);
                slDoc.SetCellValue(3, 3, tpoFeeExeGLDatas.RegionalName);
                slDoc.SetCellValue(4, 3, tpoFeeExeGLDatas.LocationName);
                slDoc.SetCellValue(3, 8, ": " + tpoFeeExeGLDatas.KpsYear);
                slDoc.SetCellValue(4, 8, ": " + tpoFeeExeGLDatas.KpsWeek);
                slDoc.SetCellValue(5, 8, ": " + tpoFeeExeGLDatas.ClosingDate.ToString(Constants.DefaultDateOnlyFormat));
                slDoc.SetCellValue(6, 8, ": " + model.Paket);
                slDoc.SetCellValue(6, 5, ": " + String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.StickPerBox));
                slDoc.SetCellValue(17, 3, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalProductionStick));
                slDoc.SetCellValue(17, 4, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalProductionBox));
                slDoc.SetCellValue(17, 5, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalProductionJkn));
                slDoc.SetCellValue(17, 6, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalProductionJl1));
                slDoc.SetCellValue(17, 7, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalProductionJl2));
                slDoc.SetCellValue(17, 8, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalProductionJl3));
                slDoc.SetCellValue(17, 9, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalProductionJl4));
                slDoc.SetCellValue(18, 5, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalDibayarJKN));
                slDoc.SetCellValue(18, 6, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalDibayarJL1));
                slDoc.SetCellValue(18, 7, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalDibayarJL2));
                slDoc.SetCellValue(18, 8, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalDibayarJL3));
                slDoc.SetCellValue(18, 9, String.Format(CultureInfo.CurrentCulture, "{0:n2}", model.TotalDibayarJL4));
                
                var iRow = 10;

                foreach (var gridView in model.TpoFeeProductionDailyPlanModels)
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
                    if (iRow == 10) slDoc.SetCellValue(iRow, 1, "Senin");
                    if (iRow == 11) slDoc.SetCellValue(iRow, 1, "Selasa");
                    if (iRow == 12) slDoc.SetCellValue(iRow, 1, "Rabu");
                    if (iRow == 13) slDoc.SetCellValue(iRow, 1, "Kamis");
                    if (iRow == 14) slDoc.SetCellValue(iRow, 1, "Jumat");
                    if (iRow == 15) slDoc.SetCellValue(iRow, 1, "Sabtu");
                    if (iRow == 16) slDoc.SetCellValue(iRow, 1, "Minggu");

                    if (gridView == null)
                    {
                        slDoc.SetCellValue(iRow, 2, "");
                        slDoc.SetCellValue(iRow, 3, 0);
                        slDoc.SetCellValue(iRow, 4, 0);
                        slDoc.SetCellValue(iRow, 5, 0);
                        slDoc.SetCellValue(iRow, 6, 0);
                        slDoc.SetCellValue(iRow, 7, 0);
                        slDoc.SetCellValue(iRow, 8, 0);
                        slDoc.SetCellValue(iRow, 9, 0);
                        slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
                    }
                    else
                    {
                        slDoc.SetCellValue(iRow, 2, gridView.FeeDate.Date);
                        slDoc.SetCellValue(iRow, 3, String.Format(CultureInfo.CurrentCulture, "{0:n2}", gridView.OuputSticks));
                        slDoc.SetCellValue(iRow, 4, String.Format(CultureInfo.CurrentCulture, "{0:n2}", gridView.OutputBox));
                        slDoc.SetCellValue(iRow, 5, String.Format(CultureInfo.CurrentCulture, "{0:n2}", gridView.JKN));
                        slDoc.SetCellValue(iRow, 6, String.Format(CultureInfo.CurrentCulture, "{0:n2}", gridView.JL1));
                        slDoc.SetCellValue(iRow, 7, String.Format(CultureInfo.CurrentCulture, "{0:n2}", gridView.Jl2));
                        slDoc.SetCellValue(iRow, 8, String.Format(CultureInfo.CurrentCulture, "{0:n2}", gridView.Jl3));
                        slDoc.SetCellValue(iRow, 9, String.Format(CultureInfo.CurrentCulture, "{0:n2}", gridView.Jl4));
                        slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
                    }
                    iRow++;

                }

                var iRowCal = 20;
                foreach (var data in model.Calculations)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (iRowCal % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }
                    slDoc.SetCellValue(iRowCal, 4, String.Format(CultureInfo.CurrentCulture, "{0:n2}", data.OutputProduction));
                    slDoc.SetCellValue(iRowCal, 6, String.Format(CultureInfo.CurrentCulture, "{0:n2}", data.OutputBiaya));
                    slDoc.SetCellValue(iRowCal, 8, String.Format(CultureInfo.CurrentCulture, "{0:n2}", data.Calculate));
                    slDoc.SetCellStyle(iRowCal, 1, iRowCal, 8, style);
                    iRowCal++;
                }

                 //row values
                

                //for (int i = 1; i <= 7; i++)
                //{
                //    SLStyle style = slDoc.CreateStyle();
                //    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                //    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                //    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                //    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                //    style.Font.FontName = "Calibri";
                //    style.Font.FontSize = 10;

                //    if (iRow % 2 == 0)
                //    {
                //        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                //    }

                //    var item = tpoFeeExeGLDatas.TpoFeeExeGlAccruedDailyDictionary.Where(c => c.Key == i).Select(c => c.Value).FirstOrDefault();
                    
                //    if(i == 1)  slDoc.SetCellValue(iRow, 1, "Senin");
                //    if(i == 2)  slDoc.SetCellValue(iRow, 1, "Selasa");
                //    if(i == 3)  slDoc.SetCellValue(iRow, 1, "Rabu");
                //    if(i == 4)  slDoc.SetCellValue(iRow, 1, "Kamis");
                //    if(i == 5)  slDoc.SetCellValue(iRow, 1, "Jumat");
                //    if(i == 6)  slDoc.SetCellValue(iRow, 1, "Sabtu");
                //    if(i == 7)  slDoc.SetCellValue(iRow, 1, "Minggu");
                    
                //    if (item == null)
                //    {
                //        slDoc.SetCellValue(iRow, 2, "");
                //        slDoc.SetCellValue(iRow, 3, 0);
                //        slDoc.SetCellValue(iRow, 4, 0);
                //        slDoc.SetCellValue(iRow, 5, 0);
                //        slDoc.SetCellValue(iRow, 6, 0);
                //        slDoc.SetCellValue(iRow, 7, 0);
                //        slDoc.SetCellValue(iRow, 8, 0);
                //        slDoc.SetCellValue(iRow, 9, 0);
                //        slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
                //    }
                //    else
                //    {
                //        slDoc.SetCellValue(iRow, 2, item.IsOnlyOneWeek ? item.Date.ToShortDateString() : "");
                //        slDoc.SetCellValue(iRow, 3, item.Batang);
                //        slDoc.SetCellValue(iRow, 4, item.Box);
                //        slDoc.SetCellValue(iRow, 5, item.Jkn);
                //        slDoc.SetCellValue(iRow, 6, item.Jl1);
                //        slDoc.SetCellValue(iRow, 7, item.Jl2);
                //        slDoc.SetCellValue(iRow, 8, item.Jl3);
                //        slDoc.SetCellValue(iRow, 9, item.Jl4);
                //        slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
                //    }

                //    iRow++;
                //}

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
                slDoc.AutoFitColumn(1, 12);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFee_ TPOFeeGLDetail_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}