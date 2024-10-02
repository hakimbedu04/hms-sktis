using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;

namespace HMS.SKTIS.BLL.TPOFeeBLL
{
    public class TPOFeeExeGLAccruedBLL : ITPOFeeExeGLAccruedBLL
    {
        private readonly IUnitOfWork _uow;
        private readonly IMasterDataBLL _masterDataBll;
        private ISqlSPRepository _sqlSPRepo;
        private readonly IGenericRepository<TPOFeeExeGLAccruedListView> _tpoFeeExeGlAccruedListViewRepo;
        private readonly IGenericRepository<MstGenStandardHour> _mstGenStandarHourRepo;
        private readonly IGenericRepository<MstTPOFeeRate> _mstTpoFeeRateRepo;
        private readonly IGenericRepository<MstGenLocation> _mstGenLocationRepo;
        private readonly IGenericRepository<MstGenBrandGroup> _mstGenBrandGroupRepo;
        private readonly IGenericRepository<MstTPOPackage> _mstTPOPackageRepo;
        private readonly IGenericRepository<PlanTPOTargetProductionKelompokBox> _plantTpotpkbox;
        private readonly IGenericRepository<MstGenWeek> _mstGenWeekRepo;
        private readonly IGenericRepository<TPOFeeProductionDailyPlan> _tpoFeeProductionDailyPlanRepo;
        private readonly IGenericRepository<MstGenBrand> _mstGenBrand;

        public TPOFeeExeGLAccruedBLL
        (
            IUnitOfWork uow,
            IMasterDataBLL masterdateDataBll
        )
        {
            _uow = uow;
            _masterDataBll = masterdateDataBll;
            _sqlSPRepo = _uow.GetSPRepository();
            _tpoFeeExeGlAccruedListViewRepo = _uow.GetGenericRepository<TPOFeeExeGLAccruedListView>();
            _mstGenStandarHourRepo = _uow.GetGenericRepository<MstGenStandardHour>();
            _mstTpoFeeRateRepo = _uow.GetGenericRepository<MstTPOFeeRate>();
            _mstGenLocationRepo = _uow.GetGenericRepository<MstGenLocation>();
            _mstGenBrandGroupRepo = _uow.GetGenericRepository<MstGenBrandGroup>();
            _mstTPOPackageRepo = _uow.GetGenericRepository<MstTPOPackage>();
            _plantTpotpkbox = _uow.GetGenericRepository<PlanTPOTargetProductionKelompokBox>();
            _mstGenWeekRepo = _uow.GetGenericRepository<MstGenWeek>();
            _mstGenBrand = _uow.GetGenericRepository<MstGenBrand>();
            _tpoFeeProductionDailyPlanRepo = _uow.GetGenericRepository<TPOFeeProductionDailyPlan>();
        }

        #region View List
        public IEnumerable<TPOFeeExeGLAccruedViewListDTO> GetTPOFeeExeGLAccruedViewList(GetTPOFeeExeGLAccruedInput criteria)
        {
            var tpoFeeGlAccruedViewList = GetTpoFeeExeGlListView(criteria).ToList();

            //var locationDiscinct = tpoFeeGlAccruedViewList.Select(c => new{LocationCode = c.LocationCode, BrandGroupCode = c.BrandGroupCode});

            var startDate = _mstGenWeekRepo.Get(c => c.Week == criteria.KpsWeek && c.Year == criteria.KpsYear).FirstOrDefault();
            criteria.StartDate = startDate.StartDate.Value;

            var listResult = new List<TPOFeeExeGLAccruedViewListDTO>();

            var listData = from y in tpoFeeGlAccruedViewList
                           group y by new {y.LocationCode,y.LocationName,y.BrandGroupCode }
                           into z
                           select new { brandGroupCode = z.Key.BrandGroupCode, 
                               locationCode = z.Key.LocationCode,
                               locationName = z.Key.LocationName
                           };

            foreach (var data in listData)
            {
                var listTpoFeeGlAccruedByLocation = tpoFeeGlAccruedViewList.Where(c => c.LocationCode == data.locationCode && c.BrandGroupCode == data.brandGroupCode).ToList();
                //var dates = _masterDataBll.GetDateByWeek(criteria.KpsYear, criteria.KpsWeek);
                var newTpoFeeGlAccrued = new TPOFeeExeGLAccruedViewListDTO
                {
                    Location = data.locationCode,
                    LocationName = data.locationName, //listTpoFeeGlAccruedByLocation.Select(c => c.LocationName).FirstOrDefault(),
                    Brand = data.brandGroupCode //listTpoFeeGlAccruedByLocation.Where(c => c.BrandGroupCode == location.BrandGroupCode).Select(c => c.BrandGroupCode).FirstOrDefault()
                };
                CalculateViewListBox(listTpoFeeGlAccruedByLocation, newTpoFeeGlAccrued, criteria);
                //newTpoFeeGlAccrued.BiayaProduksi = GetBiayaProduksi(newTpoFeeGlAccrued);
                //newTpoFeeGlAccrued.JasaManagemen = GetJasaManagemen(location, newTpoFeeGlAccrued.Brand, dates);
                newTpoFeeGlAccrued.Regional = criteria.Regional;
                newTpoFeeGlAccrued.KpsWeek = criteria.KpsWeek;
                newTpoFeeGlAccrued.KpsYear = criteria.KpsYear;
                listResult.Add(newTpoFeeGlAccrued);
            }

            return listResult;
        }

        private void CalculateViewListBox(IEnumerable<TPOFeeExeGLAccruedListView> dataView, TPOFeeExeGLAccruedViewListDTO dto, GetTPOFeeExeGLAccruedInput criteria)
        {
            //var data = from listData in dataView
            //           group listData by listData.BrandGroupCode
            //               into g
            //               select new { brandGroupCode = g.Key, location = g.Select(x => x.LocationCode).FirstOrDefault() };

            foreach (var tpoTpkBox in dataView)
            {
                //if (tpoTpkBox.BrandGroupCode != lastBrandGroupCode.bran) continue;
                var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.KpsYear, criteria.KpsWeek);
                var mstTpoPackage = _masterDataBll.GetTpoPackage(new BusinessObjects.Inputs.GetMstTPOPackagesInput
                {
                    LocationCode = dto.Location,
                    BrandGroupCode = dto.Brand,
                    StartDate = mstGenWeek.StartDate,
                    EndDate = mstGenWeek.EndDate
                });

                var paket = mstTpoPackage == null ? 0 : mstTpoPackage.Package;
                var mstGenBrand = _mstGenBrandGroupRepo.GetByID(dto.Brand);
                var stickPerBox = mstGenBrand.StickPerBox ?? 0;
                var empPackage = mstGenBrand.EmpPackage;
                var stdPerHour =
                    _masterDataBll.GetStdPerhour(new BusinessObjects.Inputs.GetMstGenProcessSettingLocationInput
                    {
                        LocationCode = dto.Location,
                        BrandGroupCode = dto.Brand,
                        ProcessGroup = Enums.Process.Rolling.ToString()
                    });

                var tpoFeeCode = "FEE/" + dto.Location + "/" + dto.Brand + "/" + criteria.KpsYear + "/" +
                                 criteria.KpsWeek;
                var dailyPlan = GetProductionDailyPlan(tpoFeeCode, criteria.ClosingDate);


                var sumBox = true ? (int)dailyPlan.Sum(c => c.OutputBox) : 0;
                var sumJknJam = dailyPlan.Sum(c => c.JKNJam) ?? 0;
                var sumJl1Jam = dailyPlan.Sum(c => c.JL1Jam) ?? 0;
                var sumJl2Jam = dailyPlan.Sum(c => c.JL2Jam) ?? 0;
                var sumJl3Jam = dailyPlan.Sum(c => c.JL3Jam) ?? 0;
                var sumJl4Jam = dailyPlan.Sum(c => c.JL4Jam) ?? 0;

                var sumJkn = dailyPlan.Sum(c => c.JKN) ?? 0;
                var sumJl1 = dailyPlan.Sum(c => c.JL1) ?? 0;
                var sumJl2 = dailyPlan.Sum(c => c.Jl2) ?? 0;
                var sumJl3 = dailyPlan.Sum(c => c.Jl3) ?? 0;
                var sumJl4 = dailyPlan.Sum(c => c.Jl4) ?? 0;

                //dto.JknBox = sumBox;

                dto.JknBox = 0;
                dto.Jl1Box = 0;
                dto.Jl2Box = 0;
                dto.Jl3Box = 0;
                dto.Jl4Box = 0;


                if (stickPerBox != 0)
                {
                    //dto.JknBox = (int)Math.Ceiling((Decimal)((paket * empPackage * stdPerHour * sumJknJam) / stickPerBox));
                    dto.JknBox =
                        (int)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJknJam) / stickPerBox), 0);
                    //if ((int)Math.Ceiling((Decimal)(sumJkn + sumJl1 + sumJl2 - sumJl3 - sumJl4)) <= dto.JknBox)
                    if ((int)Math.Round(((Decimal)(sumJkn + sumJl1 + sumJl2 - sumJl3 - sumJl4)), 0) <= dto.JknBox)
                    {
                        //dto.JknBox = (int)Math.Ceiling((Decimal)(sumJkn + sumJl1 + sumJl2 - sumJl3 - sumJl4));
                        dto.JknBox = (int)Math.Round(((Decimal)(sumJkn + sumJl1 + sumJl2 - sumJl3 - sumJl4)), 0);
                    }

                    //dto.Jl1Box = (int)Math.Ceiling((Decimal)((paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox));
                    dto.Jl1Box =
                        (int)Math.Round(((Decimal)((paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox)), 0);
                    //if ((int)Math.Ceiling((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - sumJl3 - sumJl4)) > 0)
                    if ((int)Math.Round(((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - sumJl3 - sumJl4)), 0) >
                        0)
                    {
                        //if ((int)Math.Ceiling((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - sumJl3 - sumJl4)) <= dto.Jl1Box)
                        if (
                            (int)
                                Math.Round(((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - sumJl3 - sumJl4)), 0) <=
                            dto.Jl1Box)
                        {
                            //dto.Jl1Box = (int)Math.Ceiling((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - sumJl3 - sumJl4));
                            dto.Jl1Box =
                                (int)
                                    Math.Round(
                                        ((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - sumJl3 - sumJl4)), 0);
                        }
                    }
                    else
                    {
                        dto.Jl1Box = 0;
                    }

                    dto.Jl2Box =
                        (int)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl2Jam) / stickPerBox), 0);

                    //dto.Jl2Box = (int)Math.Ceiling((Decimal)((paket * empPackage * stdPerHour * sumJl2Jam) / stickPerBox));
                    //if ((int)Math.Ceiling((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - dto.Jl1Box - sumJl3 - sumJl4)) > 0)
                    if (
                        (int)
                            Math.Round(
                                ((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - dto.Jl1Box - sumJl3 - sumJl4)),
                                0) > 0)
                    {
                        //dto.Jl2Box = (int)Math.Ceiling((Decimal)(sumJkn + sumJl1 + sumJl2 - dto.JknBox - dto.Jl1Box - sumJl3 - sumJl4));
                        dto.Jl2Box =
                            (int)
                                Math.Round(
                                    ((Decimal)
                                        (sumJkn + sumJl1 + sumJl2 - dto.JknBox - dto.Jl1Box - sumJl3 - sumJl4)), 0);
                    }
                    else
                    {
                        dto.Jl2Box = 0;
                    }

                    var standard60 =
                        (int)
                            Math.Round(
                                (Decimal)((paket * empPackage * stdPerHour * (sumJkn + sumJl1 + sumJl2)) / stickPerBox), 0);

                    //var standard60 = (int)Math.Ceiling((Decimal)((paket * empPackage * stdPerHour * (sumJkn + sumJl1 + sumJl2)) / stickPerBox));

                    dto.Jl3Box =
                        (int)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl3Jam) / stickPerBox), 0);

                    //dto.Jl3Box = (int)Math.Ceiling((Decimal)((paket * empPackage * stdPerHour * sumJl3Jam) / stickPerBox));
                    if ((sumBox - standard60) <= dto.Jl3Box)
                    {
                        if (sumBox - standard60 > 0)
                        {
                            dto.Jl3Box = sumBox - standard60;
                        }
                        else
                        {
                            dto.Jl3Box = 0;
                        }
                    }

                    dto.Jl4Box =
                        (int)Math.Round((Decimal)((paket * empPackage * stdPerHour * sumJl4Jam) / stickPerBox), 0);
                    //dto.Jl4Box = (int)Math.Ceiling((Decimal)((paket * empPackage * stdPerHour * sumJl4Jam) / stickPerBox));
                    if (sumBox - standard60 - dto.Jl3Box > 0)
                    {
                        dto.Jl4Box = sumBox - standard60 - dto.Jl3Box;
                    }
                    else
                    {
                        dto.Jl4Box = 0;
                    }
                }

                //var feeRate = _masterDataBll.GetTPOFeeRate(new HMS.SKTIS.BusinessObjects.Inputs.MstTPOFeeRateInput
                //{
                //    LocationCode = dto.Location,
                //    BrandGroupCode = dto.Brand,
                //    Year = criteria.KpsYear.ToString()
                //}).FirstOrDefault();

                var feeRate =
                    _masterDataBll.GetTPOFeeRateByExpiredDate(new BusinessObjects.Inputs.MstTPOFeeRateInput
                    {
                        LocationCode = dto.Location,
                        BrandGroupCode = dto.Brand,
                        ExpiredDate = criteria.ClosingDate,
                        StartDate = criteria.StartDate
                    }).OrderByDescending(c => c.ExpiredDate).FirstOrDefault();

                if (feeRate == null) continue;
                dto.BiayaProduksi =
                    (Int64)
                        Math.Round(
                            (Decimal)(dto.JknBox * feeRate.JKN) + (dto.Jl1Box * feeRate.Jl1) + (dto.Jl2Box * feeRate.Jl2),
                            0);
                dto.JasaManagemen = (Int64)Math.Round((Decimal)(sumBox * feeRate.ManagementFee), 0);
                //var mstTpoFeeRate = _masterDataBll.GetTPOFeeRateByExpiredDate(new HMS.SKTIS.BusinessObjects.Inputs.MstTPOFeeRateInput
                //{
                //    LocationCode = dto.Location,
                //    BrandGroupCode = dto.Brand,
                //    ExpiredDate = criteria.ClosingDate,
                //    StartDate = criteria.StartDate
                //}).OrderByDescending(c=>c.ExpiredDate).FirstOrDefault();

                //var sumJKNRp = dailyPlan.Sum(c => c.JKNRp) != null ? dailyPlan.Sum(c => c.JKNRp) : 0;
                //var sumJL1Rp = dailyPlan.Sum(c => c.JL1Rp) != null ? dailyPlan.Sum(c => c.JL1Rp) : 0;
                //var sumJL2Rp = dailyPlan.Sum(c => c.JL2Rp) != null ? dailyPlan.Sum(c => c.JL2Rp) : 0;
                //var sumJL3Rp = dailyPlan.Sum(c => c.JL3Rp) != null ? dailyPlan.Sum(c => c.JL3Rp) : 0;
                //var sumJL4Rp = dailyPlan.Sum(c => c.JL4Rp) != null ? dailyPlan.Sum(c => c.JL4Rp) : 0;

                //var mstJKN = mstTpoFeeRate.JKN != null ? mstTpoFeeRate.JKN : 0;
                //var mstJL1 = mstTpoFeeRate.Jl1 != null ? mstTpoFeeRate.Jl1 : 0;
                //var mstJL2 = mstTpoFeeRate.Jl2 != null ? mstTpoFeeRate.Jl2 : 0;
                //var mstJL3 = mstTpoFeeRate.Jl3 != null ? mstTpoFeeRate.Jl3 : 0;
                //var mstJL4 = mstTpoFeeRate.Jl4 != null ? mstTpoFeeRate.Jl4 : 0;
                //var mstMgmtFee = mstTpoFeeRate.ManagementFee != null ? mstTpoFeeRate.ManagementFee : 0;

                ////dto.JknBox = mstJKN != 0 ? (int)Math.Ceiling(Math.Floor((Decimal)((Decimal)sumJKNRp / mstJKN))) : 0;
                //dto.JknBox = mstJKN != 0 ? (int)Math.Round(((Decimal)sumJKNRp / mstJKN),0) : 0;
                //dto.Jl1Box = mstJL1 != 0 ? (int)Math.Ceiling(Math.Floor((Decimal)((Decimal)sumJL1Rp / mstJL1))) : 0;
                //dto.Jl2Box = mstJL2 != 0 ? (int)Math.Ceiling(Math.Floor((Decimal)((Decimal)sumJL2Rp / mstJL2))) : 0;
                //dto.Jl3Box = mstJL3 != 0 ? (int)Math.Ceiling(Math.Floor((Decimal)((Decimal)sumJL3Rp / mstJL3))) : 0;
                //dto.Jl4Box = mstJL4 != 0 ? (int)Math.Ceiling(Math.Floor((Decimal)((Decimal)sumJL4Rp / mstJL4))) : 0;

                //if (mstTpoFeeRate != null)
                //{
                //    //dto.BiayaProduksi = (Int64)Math.Ceiling((Decimal)(sumJKNRp + sumJL1Rp + sumJL2Rp + sumJL3Rp + sumJL4Rp));
                //    //dto.JasaManagemen = (int)Math.Ceiling((Decimal)(mstMgmtFee * dto.JknBox));
                //    dto.BiayaProduksi = (Int64)((dto.JknBox * mstJKN) + (dto.Jl1Box * mstJL1) + (dto.Jl2Box * mstJL2) + (dto.Jl3Box * mstJL3) + (dto.Jl4Box * mstJL4));
                //    dto.JasaManagemen = (Int64)(mstMgmtFee * (dto.JknBox + dto.Jl1Box + dto.Jl2Box + dto.Jl3Box + dto.Jl4Box)); 
                //}

                #region Old code

                //int day = criteria.ClosingDate.DayOfWeek == 0 ? 7 : (int)criteria.ClosingDate.DayOfWeek;
                //var processWorkHour = 0;
                //var targetManualPerHour = 0;
                //var totalProcessWorkHour = 0;

                //for (int i = 1; i <= day; i++)
                //{
                //    var mstGenStandarHour = GetMstGenStandardHour(i, EnumHelper.GetDescription(Enums.DayType.NonHoliday));

                //    switch (i)
                //    {
                //        case 1:
                //            var targetManual1 = tpoTpkBox.TargetManual1 == null ? 0 : (int)tpoTpkBox.TargetManual1.Value;
                //            processWorkHour = tpoTpkBox.ProcessWorkHours1 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours1.Value;
                //            targetManualPerHour = processWorkHour == 0 ? 0 : targetManual1 / processWorkHour;
                //            break;
                //        case 2:
                //            var targetManual2 = tpoTpkBox.TargetManual2 == null ? 0 : (int)tpoTpkBox.TargetManual2.Value;
                //            processWorkHour = tpoTpkBox.ProcessWorkHours2 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours2.Value;
                //            targetManualPerHour = processWorkHour == 0 ? 0 : targetManual2 / processWorkHour;
                //            break;
                //        case 3:
                //            var targetManual3 = tpoTpkBox.TargetManual3 == null ? 0 : (int)tpoTpkBox.TargetManual3.Value;
                //            processWorkHour = tpoTpkBox.ProcessWorkHours3 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours3.Value;
                //            targetManualPerHour = processWorkHour == 0 ? 0 : targetManual3 / processWorkHour;
                //            break;
                //        case 4:
                //            var targetManual4 = tpoTpkBox.TargetManual4 == null ? 0 : (int)tpoTpkBox.TargetManual4.Value;
                //            processWorkHour = tpoTpkBox.ProcessWorkHours4 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours4.Value;
                //            targetManualPerHour = processWorkHour == 0 ? 0 : targetManual4 / processWorkHour;
                //            break;
                //        case 5:
                //            var targetManual5 = tpoTpkBox.TargetManual5 == null ? 0 : (int)tpoTpkBox.TargetManual5.Value;
                //            processWorkHour = tpoTpkBox.ProcessWorkHours5 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours5.Value;
                //            targetManualPerHour = processWorkHour == 0 ? 0 : targetManual5 / processWorkHour;
                //            break;
                //        case 6:
                //            var targetManual6 = tpoTpkBox.TargetManual6 == null ? 0 : (int)tpoTpkBox.TargetManual6.Value;
                //            processWorkHour = tpoTpkBox.ProcessWorkHours6 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours6.Value;
                //            targetManualPerHour = processWorkHour == 0 ? 0 : targetManual6 / processWorkHour;
                //            break;
                //        case 7:
                //            var targetManual7 = tpoTpkBox.TargetManual7 == null ? 0 : (int)tpoTpkBox.TargetManual7.Value;
                //            processWorkHour = tpoTpkBox.ProcessWorkHours7 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours7.Value;
                //            targetManualPerHour = processWorkHour == 0 ? 0 : targetManual7 / processWorkHour;
                //            break;
                //    }

                //    totalProcessWorkHour += processWorkHour;

                //if (processWorkHour >= mstGenStandarHour.JknHour)
                //{
                //    dto.JknBox += (mstGenStandarHour.JknHour * targetManualPerHour);
                //    var sisaJkn = processWorkHour - mstGenStandarHour.JknHour;
                //    if (sisaJkn <= 0)
                //    {
                //        dto.Jl1Box += 0;
                //        dto.Jl2Box += 0;
                //        dto.Jl3Box += 0;
                //        dto.Jl4Box += 0;
                //    }
                //    else
                //    {
                //        if (sisaJkn >= mstGenStandarHour.Jl1Hour)
                //        {
                //            dto.Jl1Box += (mstGenStandarHour.Jl1Hour * targetManualPerHour);
                //            var sisaJl1 = sisaJkn - mstGenStandarHour.Jl1Hour;
                //            if (sisaJl1 <= 0)
                //            {
                //                dto.Jl2Box += 0;
                //                dto.Jl3Box += 0;
                //                dto.Jl4Box += 0;
                //            }
                //            else
                //            {
                //                if (sisaJl1 >= mstGenStandarHour.Jl2Hour)
                //                {
                //                    dto.Jl2Box += (mstGenStandarHour.Jl1Hour * targetManualPerHour);
                //                    var sisaJl2 = sisaJkn - mstGenStandarHour.Jl2Hour;
                //                    if (sisaJl2 <= 0)
                //                    {
                //                        dto.Jl3Box += 0;
                //                        dto.Jl4Box += 0;
                //                    }
                //                    else
                //                    {
                //                        if (sisaJl2 >= mstGenStandarHour.Jl3Hour)
                //                        {
                //                            dto.Jl3Box += (mstGenStandarHour.Jl1Hour * targetManualPerHour); ;
                //                            var sisaJl3 = sisaJkn - mstGenStandarHour.Jl3Hour;
                //                            if (sisaJl3 <= 0) dto.Jl4Box = 0;
                //                            else dto.Jl4Box += (mstGenStandarHour.Jl1Hour * targetManualPerHour); ;
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    dto.JknBox += (int)(processWorkHour * targetManualPerHour);
                //    dto.Jl1Box += 0;
                //    dto.Jl2Box += 0;
                //    dto.Jl3Box += 0;
                //    dto.Jl4Box += 0;
                //}

                //}
                #endregion

            }
        }

        //private int GetJasaManagemen(string locationCode, string brandGroupCode, IEnumerable<DateTime> dates)
        //{
        //    var listFeeMaklon = new List<int>();

        //    foreach (var date in dates)
        //    {
        //        var dbResult = _mstTpoFeeRateRepo.GetByID(date, brandGroupCode, locationCode);
        //        if (dbResult != null) listFeeMaklon.Add((int)dbResult.ManagementFee);
        //    }
        //    var result = !listFeeMaklon.Any() ? 0 : listFeeMaklon.Max();
        //    return result;
        //}

        //private static int GetBiayaProduksi(TPOFeeExeGLAccruedViewListDTO tpoFeeExeGlAccrued)
        //{
        //    return tpoFeeExeGlAccrued.JknBox + tpoFeeExeGlAccrued.Jl1Box + tpoFeeExeGlAccrued.Jl2Box + tpoFeeExeGlAccrued.Jl3Box + tpoFeeExeGlAccrued.Jl4Box;
        //}

        

        private MstGenStandardHour GetMstGenStandardHour(int day, string dayType)
        {
            var dbResult = _mstGenStandarHourRepo.GetByID(dayType, day);

            return dbResult;
        }

        private List<TPOFeeProductionDailyPlanDto> GetProductionDailyPlan(string feeCode, DateTime closingDate)
        {
            var queryFilter = PredicateHelper.True<TPOFeeProductionDailyPlan>();

            queryFilter = queryFilter.And(c => c.TPOFeeCode == feeCode);
            queryFilter = queryFilter.And(c => c.FeeDate <= closingDate);

            var dbResult = _tpoFeeProductionDailyPlanRepo.Get(queryFilter);

            return Mapper.Map<List<TPOFeeProductionDailyPlanDto>>(dbResult); ;
        }

        public List<GenerateP1TemplateGLDTO> GetP1TemplateGL(GetTPOFeeExeGLAccruedInput input)
        {
            var dbResult = _sqlSPRepo.GenerateP1TemplateGL(input.ClosingDate, input.KpsWeek, input.KpsYear, input.Location);

            return Mapper.Map<List<GenerateP1TemplateGLDTO>>(dbResult);
        }

        private IEnumerable<TPOFeeExeGLAccruedListView> GetTpoFeeExeGlListView(GetTPOFeeExeGLAccruedInput criteria)
        {
            var queryFilter = PredicateHelper.True<TPOFeeExeGLAccruedListView>();

            if (!string.IsNullOrEmpty(criteria.Regional) && criteria.Regional != Enums.LocationCode.TPO.ToString())
            {
                queryFilter = queryFilter.And(c => c.ParentLocationCode == criteria.Regional);
            }
            if (criteria.KpsWeek > 0) queryFilter = queryFilter.And(m => m.KPSWeek == criteria.KpsWeek);
            if (criteria.KpsYear > 0) queryFilter = queryFilter.And(m => m.KPSYear == criteria.KpsYear);
            //var dbResult = new List<TPOFeeExeGLAccruedViewListDTO>();
            //if (criteria.Regional != HMS.SKTIS.Core.Enums.LocationCode.TPO.ToString())
            //{
            //    dbResult = _tpoFeeExeGlAccruedListViewRepo.Get(queryFilter).OrderBy(x => x.LocationName);
            //}
            //else
            //{
            //    dbResult = _tpoFeeExeGlAccruedListViewRepo.Get(queryFilter).OrderBy(x => x.ParentLocationCode).OrderBy(x=>x.LocationCode);
            //}
             

            //return dbResult;
            return criteria.Regional != Enums.LocationCode.TPO.ToString() ? _tpoFeeExeGlAccruedListViewRepo.Get(queryFilter).OrderBy(x => x.LocationCode) : _tpoFeeExeGlAccruedListViewRepo.Get(queryFilter).OrderBy(x => x.ParentLocationCode);
        }
        #endregion

        #region Detail
        public void SetTPOFeeExeGLAccruedDetailHdr(GetTPOFeeExeGLAccruedInput criteria, TPOFeeExeGLAccruedDetailDTO dto)
        {
            //var tpoFeeGLAccruedHdr = new TPOFeeExeGLAccruedHdrDTO();

            var mstGenLoc = _mstGenLocationRepo.GetByID(criteria.Location);
            var regional = _mstGenLocationRepo.GetByID(mstGenLoc.ParentLocationCode).LocationCode;
            var regionalName = _mstGenLocationRepo.GetByID(mstGenLoc.ParentLocationCode).LocationName;
            var stickPerBox = _mstGenBrandGroupRepo.GetByID(criteria.Brand) != null ? _mstGenBrandGroupRepo.GetByID(criteria.Brand).StickPerBox : null;

            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.KpsYear, criteria.KpsWeek);
            var mstTpoPackage = _masterDataBll.GetTpoPackage(new HMS.SKTIS.BusinessObjects.Inputs.GetMstTPOPackagesInput
            {
                LocationCode = criteria.Location,
                BrandGroupCode = criteria.Brand,
                StartDate = mstGenWeek.StartDate,
                EndDate = mstGenWeek.EndDate
            });

            var paket = mstTpoPackage == null ? 0 : mstTpoPackage.Package;
            //var startDate = _masterDataBll.GetFirstDateByYearWeek(criteria.KpsYear, criteria.KpsWeek);
            //if (startDate >= criteria.ClosingDate)
            //{
            //    for (var date = criteria.ClosingDate; date <= startDate; date = date.AddDays(1))
            //    {
            //        var dbResult = _mstTPOPackageRepo.GetByID(criteria.Location, criteria.Brand, date);
            //        if (dbResult != null) paket = (int)dbResult.Package;
            //    }
            //}
            //else
            //{
            //    for (var date = startDate; date <= criteria.ClosingDate; date = date.AddDays(1))
            //    {
            //        var dbResult = _mstTPOPackageRepo.GetByID(criteria.Location, criteria.Brand, date);
            //        if (dbResult != null) paket = (int)dbResult.Package;
            //    }
            //}

            dto.Brand = criteria.Brand;
            dto.Location = criteria.Location;
            dto.Regional = regional;
            dto.ClosingDate = criteria.ClosingDate;
            dto.KpsWeek = criteria.KpsWeek;
            dto.KpsYear = criteria.KpsYear;
            dto.RegionalName = regionalName;
            dto.CostCenter = mstGenLoc.CostCenter;
            dto.StickPerBox = stickPerBox == null ? 0 : stickPerBox.Value;
            dto.Paket = paket;
        }

        public TPOFeeExeGLAccruedDetailDTO GetTpoFeeExeGlAccruedDetailDaily(GetTPOFeeExeGLAccruedInput criteria)
        {
            var tpoFeeExeGlAccruedDailyDictionary = new Dictionary<int, TPOFeeExeGLAccruedDailyDTO>();
            var tpoExeDetailFinal = new TPOFeeExeGLAccruedDetailDTO();

            var mstgenBrandGroup = _mstGenBrandGroupRepo.GetByID(criteria.Brand);

            criteria.StickPerBox = mstgenBrandGroup == null ? 0 : mstgenBrandGroup.StickPerBox == null ? 0 : mstgenBrandGroup.StickPerBox.Value;

            SetTPOFeeExeGLAccruedDetailHdr(criteria, tpoExeDetailFinal);

            var tpoTpkBoxList = GetTpoTpkBox(criteria).ToList();

            var newDto = new TPOFeeExeGLAccruedDailyDTO();

            foreach (var tpoTpkBox in tpoTpkBoxList)
            {
                var startDate = _mstGenWeekRepo.GetByID(Convert.ToInt32(criteria.KpsYear + criteria.KpsWeek.ToString("00"))).StartDate;
                var isOnlyOneWeek = tpoTpkBoxList.Count() == 1;
                if (startDate != null)
                {
                    if (startDate.Value <= criteria.ClosingDate)
                    {
                        for (var date = startDate.Value; date <= criteria.ClosingDate; date = date.AddDays(1))
                        {
                            var mstGenWeek = _masterDataBll.GetWeekByDate(date);
                            var week = mstGenWeek == null ? 0 : mstGenWeek.Week;
                            if (week == tpoTpkBox.KPSWeek)
                            {
                                switch ((int)date.DayOfWeek)
                                {
                                    case 1:
                                        var tm1 = tpoTpkBox.TargetManual1 == null ? 0 : (int)tpoTpkBox.TargetManual1.Value;
                                        var pwh1 = tpoTpkBox.ProcessWorkHours1 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours1.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm1, pwh1, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add(1, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm1, pwh1, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm1, pwh1, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 2:
                                        var tm2 = tpoTpkBox.TargetManual2 == null ? 0 : (int)tpoTpkBox.TargetManual2.Value;
                                        var pwh2 = tpoTpkBox.ProcessWorkHours2 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours2.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), 2, tm2, pwh2, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add(2, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm2, pwh2, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm2, pwh2, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 3:
                                        var tm3 = tpoTpkBox.TargetManual3 == null ? 0 : (int)tpoTpkBox.TargetManual3.Value;
                                        var pwh3 = tpoTpkBox.ProcessWorkHours3 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours3.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm3, pwh3, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm3, pwh3, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm3, pwh3, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 4:
                                        var tm4 = tpoTpkBox.TargetManual4 == null ? 0 : (int)tpoTpkBox.TargetManual4.Value;
                                        var pwh4 = tpoTpkBox.ProcessWorkHours4 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours4.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm4, pwh4, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm4, pwh4, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm4, pwh4, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 5:
                                        var tm5 = tpoTpkBox.TargetManual5 == null ? 0 : (int)tpoTpkBox.TargetManual5.Value;
                                        var pwh5 = tpoTpkBox.ProcessWorkHours5 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours5.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm5, pwh5, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm5, pwh5, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm5, pwh5, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 6:
                                        var tm6 = tpoTpkBox.TargetManual6 == null ? 0 : (int)tpoTpkBox.TargetManual6.Value;
                                        var pwh6 = tpoTpkBox.ProcessWorkHours6 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours6.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm6, pwh6, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add(6, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm6, pwh6, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm6, pwh6, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 0:
                                        var tm7 = tpoTpkBox.TargetManual7 == null ? 0 : (int)tpoTpkBox.TargetManual7.Value;
                                        var pwh7 = tpoTpkBox.ProcessWorkHours7 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours7.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add(7, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), 7, tm7, pwh7, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey(7)) tpoFeeExeGlAccruedDailyDictionary.Add(7, CreateTpoFeeExeGlAccruedDtoDaily(newDto, 7, tm7, pwh7, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[7] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, 7, tm7, pwh7, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                }
                            }

                            var mstTpoFeeRate = _mstTpoFeeRateRepo.GetByID(date, criteria.Brand, criteria.Location);
                            if (mstTpoFeeRate != null)
                            {
                                tpoExeDetailFinal.JknFeeRate = (int)mstTpoFeeRate.JKN;
                                tpoExeDetailFinal.Jl1FeeRate = (int)mstTpoFeeRate.JL1;
                                tpoExeDetailFinal.Jl2FeeRate = (int)mstTpoFeeRate.Jl2;
                                tpoExeDetailFinal.Jl3FeeRate = (int)mstTpoFeeRate.Jl3;
                                tpoExeDetailFinal.Jl4FeeRate = (int)mstTpoFeeRate.Jl4;
                                tpoExeDetailFinal.ManajemenFee = (int)mstTpoFeeRate.ManagementFee;
                            }

                        }
                    }
                    else if ((startDate.Value >= criteria.ClosingDate))
                    {
                        for (var date = criteria.ClosingDate; date <= startDate.Value; date = date.AddDays(1))
                        {
                            var mstGenWeek = _masterDataBll.GetWeekByDate(date);
                            var week = mstGenWeek == null ? 0 : mstGenWeek.Week;
                            if (week == tpoTpkBox.KPSWeek)
                            {
                                switch ((int)date.DayOfWeek)
                                {
                                    case 1:
                                        var tm1 = tpoTpkBox.TargetManual1 == null ? 0 : (int)tpoTpkBox.TargetManual1.Value;
                                        var pwh1 = tpoTpkBox.ProcessWorkHours1 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours1.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm1, pwh1, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add(1, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm1, pwh1, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm1, pwh1, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 2:
                                        var tm2 = tpoTpkBox.TargetManual2 == null ? 0 : (int)tpoTpkBox.TargetManual2.Value;
                                        var pwh2 = tpoTpkBox.ProcessWorkHours2 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours2.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), 2, tm2, pwh2, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add(2, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm2, pwh2, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm2, pwh2, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 3:
                                        var tm3 = tpoTpkBox.TargetManual3 == null ? 0 : (int)tpoTpkBox.TargetManual3.Value;
                                        var pwh3 = tpoTpkBox.ProcessWorkHours3 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours3.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm3, pwh3, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm3, pwh3, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm3, pwh3, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 4:
                                        var tm4 = tpoTpkBox.TargetManual4 == null ? 0 : (int)tpoTpkBox.TargetManual4.Value;
                                        var pwh4 = tpoTpkBox.ProcessWorkHours4 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours4.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm4, pwh4, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm4, pwh4, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm4, pwh4, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 5:
                                        var tm5 = tpoTpkBox.TargetManual5 == null ? 0 : (int)tpoTpkBox.TargetManual5.Value;
                                        var pwh5 = tpoTpkBox.ProcessWorkHours5 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours5.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm5, pwh5, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm5, pwh5, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm5, pwh5, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 6:
                                        var tm6 = tpoTpkBox.TargetManual6 == null ? 0 : (int)tpoTpkBox.TargetManual6.Value;
                                        var pwh6 = tpoTpkBox.ProcessWorkHours6 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours6.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add((int)date.DayOfWeek, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), (int)date.DayOfWeek, tm6, pwh6, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey((int)date.DayOfWeek)) tpoFeeExeGlAccruedDailyDictionary.Add(6, CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm6, pwh6, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[(int)date.DayOfWeek] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, (int)date.DayOfWeek, tm6, pwh6, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                    case 0:
                                        var tm7 = tpoTpkBox.TargetManual7 == null ? 0 : (int)tpoTpkBox.TargetManual7.Value;
                                        var pwh7 = tpoTpkBox.ProcessWorkHours7 == null ? 0 : (int)tpoTpkBox.ProcessWorkHours7.Value;
                                        if (isOnlyOneWeek) tpoFeeExeGlAccruedDailyDictionary.Add(7, CreateTpoFeeExeGlAccruedDtoDaily(new TPOFeeExeGLAccruedDailyDTO(), 7, tm7, pwh7, date, criteria.StickPerBox, isOnlyOneWeek));
                                        else
                                        {
                                            if (!tpoFeeExeGlAccruedDailyDictionary.ContainsKey(7)) tpoFeeExeGlAccruedDailyDictionary.Add(7, CreateTpoFeeExeGlAccruedDtoDaily(newDto, 7, tm7, pwh7, date, criteria.StickPerBox, isOnlyOneWeek));
                                            else tpoFeeExeGlAccruedDailyDictionary[7] = CreateTpoFeeExeGlAccruedDtoDaily(newDto, 7, tm7, pwh7, date, criteria.StickPerBox, isOnlyOneWeek);
                                        }
                                        break;
                                }
                            }
                            
                            var mstTpoFeeRate = _mstTpoFeeRateRepo.GetByID(date, criteria.Brand, criteria.Location);
                            if (mstTpoFeeRate != null)
                            {
                                tpoExeDetailFinal.JknFeeRate = (int)mstTpoFeeRate.JKN;
                                tpoExeDetailFinal.Jl1FeeRate = (int)mstTpoFeeRate.JL1;
                                tpoExeDetailFinal.Jl2FeeRate = (int)mstTpoFeeRate.Jl2;
                                tpoExeDetailFinal.Jl3FeeRate = (int)mstTpoFeeRate.Jl3;
                                tpoExeDetailFinal.Jl4FeeRate = (int)mstTpoFeeRate.Jl4;
                                tpoExeDetailFinal.ManajemenFee = (int)mstTpoFeeRate.ManagementFee;
                            }

                        }
                    }
                }
            }
            tpoExeDetailFinal.TpoFeeExeGlAccruedDailyDictionary = tpoFeeExeGlAccruedDailyDictionary;
            return tpoExeDetailFinal;
        }

        private void CalculateJknJl1Jl2Jl3Jl4(int day, int targetManual, int processWorkHours, TPOFeeExeGLAccruedDailyDTO dto)
        {
            var mstGenStandarHour = GetMstGenStandardHour(day, EnumHelper.GetDescription(Enums.DayType.NonHoliday));
            var tmPerPwh = processWorkHours == 0 ? 0 : Math.Round((decimal)targetManual / processWorkHours, 2);
            if (processWorkHours >= mstGenStandarHour.JknHour)
            {
                dto.Jkn += (int)(mstGenStandarHour.JknHour * tmPerPwh);
                var sisaJkn = processWorkHours - mstGenStandarHour.JknHour;
                if (sisaJkn <= 0)
                {
                    dto.Jl1 = 0;
                    dto.Jl2 = 0;
                    dto.Jl3 = 0;
                    dto.Jl4 = 0;
                }
                else
                {
                    if (sisaJkn >= mstGenStandarHour.Jl1Hour)
                    {
                        dto.Jl1 += (int)(mstGenStandarHour.Jl1Hour * tmPerPwh);
                        var sisaJl1 = sisaJkn - mstGenStandarHour.Jl1Hour;
                        if (sisaJl1 <= 0)
                        {
                            dto.Jl2 = 0;
                            dto.Jl3 = 0;
                            dto.Jl4 = 0;
                        }
                        else
                        {
                            if (sisaJl1 >= mstGenStandarHour.Jl2Hour)
                            {
                                dto.Jl2 += (int)(mstGenStandarHour.Jl2Hour * tmPerPwh);
                                var sisaJl2 = sisaJkn - mstGenStandarHour.Jl2Hour;
                                if (sisaJl2 <= 0)
                                {
                                    dto.Jl3 = 0;
                                    dto.Jl4 = 0;
                                }
                                else
                                {
                                    if (sisaJl2 >= mstGenStandarHour.Jl3Hour)
                                    {
                                        dto.Jl3 += (int)(mstGenStandarHour.Jl3Hour * tmPerPwh);
                                        var sisaJl3 = sisaJkn - mstGenStandarHour.Jl3Hour;
                                        if (sisaJl3 <= 0) dto.Jl4 = 0;
                                        else dto.Jl4 += (int)(mstGenStandarHour.Jl4Hour * tmPerPwh);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                dto.Jkn += (int)(processWorkHours * tmPerPwh);
                dto.Jl1 = 0;
                dto.Jl2 = 0;
                dto.Jl3 = 0;
                dto.Jl4 = 0;
            }
        }

        private TPOFeeExeGLAccruedDailyDTO CreateTpoFeeExeGlAccruedDtoDaily(TPOFeeExeGLAccruedDailyDTO dto, int day, int targetManual, int processWorkHour, DateTime date, int stickPerBox, bool isOnlyOneWeek)
        {
            CalculateJknJl1Jl2Jl3Jl4(day, targetManual, processWorkHour, dto);

            if (isOnlyOneWeek)
            {
                dto.Date = date;
                dto.IsOnlyOneWeek = true;
            }
            dto.Batang += (int)targetManual * stickPerBox;
            dto.Box += (int)targetManual;

            return dto;
        }

        private IEnumerable<PlanTPOTargetProductionKelompokBox> GetTpoTpkBox(GetTPOFeeExeGLAccruedInput criteria)
        {
            var closingDateYear = criteria.ClosingDate.Year;
            var closingDateWeek = _masterDataBll.GetWeekByDate(criteria.ClosingDate);
            var closingWeek     = closingDateWeek == null ? 0 : closingDateWeek.Week == null ? 0 : closingDateWeek.Week.Value;    

            var mstGenBrand = _mstGenBrand.Get(c => c.BrandGroupCode == criteria.Brand).FirstOrDefault();
            var brandCode   = mstGenBrand == null ? "" : mstGenBrand.BrandCode;

            var queryFilter = PredicateHelper.True<PlanTPOTargetProductionKelompokBox>();

            if (!string.IsNullOrEmpty(criteria.Location)) queryFilter = queryFilter.And(c => c.LocationCode == criteria.Location);
            if (!string.IsNullOrEmpty(criteria.Brand)) queryFilter = queryFilter.And(c => c.BrandCode == brandCode);
            if (criteria.KpsYear > 0 && closingDateYear > 0)
            {
                queryFilter = criteria.KpsYear >= closingDateYear 
                            ? queryFilter.And(m => m.KPSYear <= criteria.KpsYear && m.KPSYear >= closingDateYear) 
                            : queryFilter.And(m => m.KPSYear >= criteria.KpsYear && m.KPSYear <= closingDateYear);
            }

            if (criteria.KpsWeek > 0 && closingWeek > 0)
            {
                queryFilter = criteria.KpsWeek >= closingWeek
                            ? queryFilter.And(m => m.KPSWeek <= criteria.KpsWeek && m.KPSWeek >= closingWeek)
                            : queryFilter.And(m => m.KPSWeek >= criteria.KpsWeek && m.KPSWeek <= closingWeek);
            }

            var dbResult = _plantTpotpkbox.Get(queryFilter);

            return dbResult;
        }

        #endregion

    }
}
