using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System.Text;
using System.Net.Mail;

namespace HMS.SKTIS.BLL
{
    public partial class PlanningBLL
    {
        public List<string> GetTPKCodeByLocations(GetTPOTPKInput input)
        {
            var dbTPKCode = _planTPOTPKRepo.Get(tpk => tpk.LocationCode == input.LocationCode && tpk.BrandCode == input.BrandCode &&
                                                       tpk.KPSYear == input.KPSYear && tpk.KPSWeek == input.KPSWeek).Select(s => s.TPKCode);
            return dbTPKCode.Distinct().ToList();
        }

        public List<string> GetTPOTPKProcessByLocations(string locationCode, int year, int week)
        {
            var dbTPKProcess = _planTPOTPKRepo.Get(tpk => tpk.LocationCode == locationCode && tpk.KPSYear == year && tpk.KPSWeek == week);

            // @todo refactor this; similar to PlanningPlantBLL@GetPlantTPKProcessByLocations
            var MstGenProc = _mstGenProcess.Get();
            var Joined = from dbTPKPGroup in dbTPKProcess
                         join pOrder in MstGenProc
                         on dbTPKPGroup.ProcessGroup equals pOrder.ProcessGroup
                         orderby pOrder.ProcessOrder ascending
                         select dbTPKPGroup.ProcessGroup;

            return Joined.Distinct().ToList();
        }

        public List<string> GetAllProcessFromExeTPOProductionVerificationByLocationCodeAndDate(string locationCode, int year, int week, DateTime? date)
        {
            var queryFilter = PredicateHelper.True<TPOTargetProductionKelompokView>();
            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (date.HasValue)
            {
                var mstGenWeek = _masterDataBll.GetWeekByDate(date.Value);
                if (mstGenWeek != null)
                    queryFilter = queryFilter.And(m => m.TPKTPOStartProductionDate == mstGenWeek.StartDate);
            }

            var processData = _planTPOTPKViewRepo.Get(queryFilter).Select(s => s.ProcessGroup);
            return processData.Distinct().ToList();
        }

        public List<PlanTPOTPKCompositeDTO> GetPlanningTPOTPK(GetTPOTPKInput input)
        {
            var queryFilter = PredicateHelper.True<TPOTargetProductionKelompokView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);

            if (!string.IsNullOrEmpty(input.TPKCode))
                queryFilter = queryFilter.And(m => m.TPKCode == input.TPKCode);

            if (input.KPSYear != null)
                queryFilter = queryFilter.And(m => m.KPSYear == input.KPSYear);

            if (input.KPSWeek != null)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.KPSWeek);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TPOTargetProductionKelompokView>();

            var dbResult = _planTPOTPKViewRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<PlanTPOTPKCompositeDTO>>(dbResult);
        }

        public TPOTPKByProcessDTO SaveTPOTPKByGroup(TPOTPKByProcessDTO tpoTPKbyProcess)
        {
            //save wip
            var wipStock = Mapper.Map<WIPStockDTO>(tpoTPKbyProcess);
            UpdateWIPStock(wipStock);

            //save tpks
            foreach (var tpoTpk in tpoTPKbyProcess.PlanTPOTPK)
            {
                UpdateTPOTPK(tpoTpk);
            }
            _uow.SaveChanges();

            return tpoTPKbyProcess;
        }

        public List<TPOTPKDTO> GetLatestReadyToSubmitTPOTPK(string locationCode, List<string> processGroup, MstGenWeekDTO StartDate)
        {
            var readyTPOTPK = new List<TPOTPKDTO>();
            MstGenWeekDTO toWeek = _masterDataBll.GetWeekByDate(StartDate.TodayDate.GetValueOrDefault());

            var dbTPOTPK0 = _planTPOTPKRepo.Get(m =>
                m.LocationCode == locationCode &&
                processGroup.Contains(m.ProcessGroup) &&
                ((m.KPSYear >= StartDate.Year && m.KPSWeek >= StartDate.Week) || (m.KPSYear >= toWeek.Year && m.KPSWeek >= toWeek.Week)) &&
                m.TPKCode != "" && m.TPKCode != "0").GroupBy(m => new
                {
                    m.TPKCode, 
                    m.BrandCode, 
                    m.KPSWeek, 
                    m.KPSYear
                }).Select(g => new TPOTPKDTO()
                {
                    TPKCode = g.Key.TPKCode,
                    LocationCode = locationCode,
                    BrandCode = g.Key.BrandCode,
                    KPSWeek = g.Key.KPSWeek,
                    KPSYear = g.Key.KPSYear,
                    TPKTPOStartProductionDate = _masterDataBll.GetWeekByYearAndWeek(g.Key.KPSYear, g.Key.KPSWeek).StartDate.Value,
                    ProcessWorkHours1 = g.Max(m => m.ProcessWorkHours1),
                    ProcessWorkHours2 = g.Max(m => m.ProcessWorkHours2),
                    ProcessWorkHours3 = g.Max(m => m.ProcessWorkHours3),
                    ProcessWorkHours4 = g.Max(m => m.ProcessWorkHours4),
                    ProcessWorkHours5 = g.Max(m => m.ProcessWorkHours5),
                    ProcessWorkHours6 = g.Max(m => m.ProcessWorkHours6),
                    ProcessWorkHours7 = g.Max(m => m.ProcessWorkHours7),
                    TotalWorkhours = g.Max(m => m.TotalWorkhours)
                });

            if (dbTPOTPK0.Any())
            {
                foreach (var raw in dbTPOTPK0)
                {

                    string data = raw.TPKCode + "," + "WPP/" + raw.KPSYear + "/" + raw.KPSWeek;
                    var ResultCheck = _utilitiesBll.getSubmittedWpp(data);
                    if (ResultCheck[0] == true)
                    {
                        readyTPOTPK.Add(raw);
                    }
                }
            }

            return readyTPOTPK;
        }

        public void SaveLatestPlanTPOTPK(List<TPOTPKDTO> readyTPOTPK, TPOTPKDTO tpoTPK, bool? status, string userName)
        {
            if (readyTPOTPK.Any())
            {
                var updatedTpk = new PlanTPOTargetProductionKelompok();

                foreach (var tpotpkdto in readyTPOTPK)
                {
                    var tpkCode = tpotpkdto.TPKCode;
                    updatedTpk = _planTPOTPKRepo.Get(m => m.TPKCode == tpkCode && m.ProcessGroup == tpoTPK.ProcessGroup && m.ProdGroup == tpoTPK.ProdGroup).FirstOrDefault();

                    if (updatedTpk != null)
                    {
                        if (status == true)
                        {
                            // update
                            updatedTpk.WorkerAlocation = tpoTPK.WorkerAlocation;
                            updatedTpk.WorkerAvailable = tpoTPK.WorkerAvailable;
                            updatedTpk.WorkerRegister = tpoTPK.WorkerRegister;
                            updatedTpk.UpdatedDate = DateTime.Now;
                            updatedTpk.UpdatedBy = userName;
                            _planTPOTPKRepo.Update(updatedTpk);
                        }
                        else
                        {
                            // delete
                            _planTPOTPKRepo.Delete(updatedTpk);
                        }
                    }
                    else
                    {
                        if (status == true)
                        {
                            // insert
                            var insertTpk = Mapper.Map<PlanTPOTargetProductionKelompok>(tpoTPK);
                            insertTpk.BrandCode = tpotpkdto.BrandCode;
                            insertTpk.TPKTPOStartProductionDate = tpotpkdto.TPKTPOStartProductionDate;
                            insertTpk.TPKCode = tpkCode;
                            insertTpk.CreatedDate = DateTime.Now;
                            insertTpk.UpdatedDate = DateTime.Now;
                            insertTpk.CreatedBy = userName;
                            insertTpk.UpdatedBy = userName;

                            insertTpk.KPSYear = tpotpkdto.KPSYear;
                            insertTpk.KPSWeek = tpotpkdto.KPSWeek;

                            insertTpk.PercentAttendance1 = 95;
                            insertTpk.PercentAttendance2 = 95;
                            insertTpk.PercentAttendance3 = 95;
                            insertTpk.PercentAttendance4 = 95;
                            insertTpk.PercentAttendance5 = 95;
                            insertTpk.PercentAttendance6 = 95;
                            insertTpk.PercentAttendance7 = 95;

                            var brand = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = insertTpk.BrandCode });

                            var query = PredicateHelper.True<ProcessSettingsAndLocationView>();

                            query = query.And(x => x.BrandGroupCode == brand.BrandGroupCode);
                            query = query.And(x => x.LocationCode == insertTpk.LocationCode);
                            query = query.And(x => x.ProcessGroup == Enums.Process.Rolling.ToString().ToUpper());

                            var genProcessSetting = _mstGenProcessSettingLocationViewRepo.Get(query).FirstOrDefault();

                            var defaultWorker = genProcessSetting.MinStickPerHour / genProcessSetting.UOMEblek;
                            var defaultGroup = insertTpk.WorkerAlocation * defaultWorker;

                            insertTpk.HistoricalCapacityWorker1 = defaultWorker;
                            insertTpk.HistoricalCapacityWorker2 = defaultWorker;
                            insertTpk.HistoricalCapacityWorker3 = defaultWorker;
                            insertTpk.HistoricalCapacityWorker4 = defaultWorker;
                            insertTpk.HistoricalCapacityWorker5 = defaultWorker;
                            insertTpk.HistoricalCapacityWorker6 = defaultWorker;
                            insertTpk.HistoricalCapacityWorker7 = defaultWorker;

                            insertTpk.HistoricalCapacityGroup1 = defaultGroup;
                            insertTpk.HistoricalCapacityGroup2 = defaultGroup;
                            insertTpk.HistoricalCapacityGroup3 = defaultGroup;
                            insertTpk.HistoricalCapacityGroup4 = defaultGroup;
                            insertTpk.HistoricalCapacityGroup5 = defaultGroup;
                            insertTpk.HistoricalCapacityGroup6 = defaultGroup;
                            insertTpk.HistoricalCapacityGroup7 = defaultGroup;

                            insertTpk.ProcessWorkHours1 = tpotpkdto.ProcessWorkHours1;
                            insertTpk.ProcessWorkHours2 = tpotpkdto.ProcessWorkHours1;
                            insertTpk.ProcessWorkHours3 = tpotpkdto.ProcessWorkHours1;
                            insertTpk.ProcessWorkHours4 = tpotpkdto.ProcessWorkHours1;
                            insertTpk.ProcessWorkHours5 = tpotpkdto.ProcessWorkHours1;
                            insertTpk.ProcessWorkHours6 = tpotpkdto.ProcessWorkHours1;
                            insertTpk.ProcessWorkHours7 = tpotpkdto.ProcessWorkHours1;

                            insertTpk.TotalWorkhours = tpotpkdto.TotalWorkhours;

                            _planTPOTPKRepo.Insert(insertTpk);
                        }
                    }
                }

                _uow.SaveChanges();
            }
        }

        public TPOTPKDTO SavePlanTPOTPK(TPOTPKDTO tpoTPK, bool? status, string userName)
        {
            var dbTPOTPK = _planTPOTPKRepo.Get(m => 
                m.LocationCode == tpoTPK.LocationCode && 
                m.ProcessGroup == tpoTPK.ProcessGroup && 
                m.ProdGroup == tpoTPK.ProdGroup && 
                
                m.TPKCode != "" && m.TPKCode != "0");
            var tpkCode = "";
            if (dbTPOTPK == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var updatedTpk = new PlanTPOTargetProductionKelompok();

            if (dbTPOTPK.Count() > 0)
            {
                foreach (var raw in dbTPOTPK)
                {
                    string data = raw.TPKCode + "," + "WPP/" + raw.KPSYear + "/" + raw.KPSWeek;
                    var ResultCheck = _utilitiesBll.getSubmittedWpp(data);
                    if (ResultCheck[0] == true)
                    {
                        tpkCode = raw.TPKCode;
                    }
                }
                updatedTpk = _planTPOTPKRepo.Get(m => m.TPKCode == tpkCode && m.ProcessGroup == tpoTPK.ProcessGroup && m.ProdGroup == tpoTPK.ProdGroup).FirstOrDefault();

                if (updatedTpk != null)
                {
                    updatedTpk.WorkerAlocation = tpoTPK.WorkerAlocation;
                    updatedTpk.WorkerAvailable = tpoTPK.WorkerAvailable;
                    updatedTpk.WorkerRegister = tpoTPK.WorkerRegister;
                    _planTPOTPKRepo.Update(updatedTpk);

                    _uow.SaveChanges();

                    return Mapper.Map<TPOTPKDTO>(updatedTpk);
                }
            }

            if ((!dbTPOTPK.Any() && status == true) || (updatedTpk == null))
            {
                dbTPOTPK =
                    _planTPOTPKRepo.Get(
                        m =>
                            m.LocationCode == tpoTPK.LocationCode && m.ProcessGroup == tpoTPK.ProcessGroup &&
                            m.TPKCode != "" && m.TPKCode != "0");
                var brandCode = "";
                int week = 0;
                int year = 0;
                foreach (var raw in dbTPOTPK)
                {
                    string data = raw.TPKCode + "," + "WPP/" + raw.KPSYear + "/" + raw.KPSWeek;
                    var ResultCheck = _utilitiesBll.getSubmittedWpp(data);
                    if (ResultCheck[0] == true)
                    {
                        tpkCode = raw.TPKCode;
                        brandCode = raw.BrandCode;
                        week = raw.KPSWeek;
                        year = raw.KPSYear;
                    }
                }
                var insertTpk = Mapper.Map<PlanTPOTargetProductionKelompok>(tpoTPK);
                insertTpk.BrandCode = brandCode;
                insertTpk.TPKCode = tpkCode;
                insertTpk.CreatedDate = DateTime.Now;
                insertTpk.UpdatedDate = DateTime.Now;
                insertTpk.CreatedBy = userName;
                insertTpk.UpdatedBy = userName;
                insertTpk.PercentAttendance1 = 95;
                insertTpk.PercentAttendance2 = 95;
                insertTpk.PercentAttendance3 = 95;
                insertTpk.PercentAttendance4 = 95;
                insertTpk.PercentAttendance5 = 95;
                insertTpk.PercentAttendance6 = 95;
                insertTpk.PercentAttendance7 = 95;

                var brand = _masterDataBll.GetBrand(new GetBrandInput{BrandCode = insertTpk.BrandCode});

                var query = PredicateHelper.True<ProcessSettingsAndLocationView>();

                query = query.And(x => x.BrandGroupCode == brand.BrandGroupCode);
                query = query.And(x => x.LocationCode == insertTpk.LocationCode);
                query = query.And(x => x.ProcessGroup == Enums.Process.Rolling.ToString().ToUpper());

                var genProcessSetting = _mstGenProcessSettingLocationViewRepo.Get(query).FirstOrDefault();

                var defaultWorker = genProcessSetting.MinStickPerHour / genProcessSetting.UOMEblek;
                var defaultGroup = insertTpk.WorkerAlocation * defaultWorker;

                insertTpk.HistoricalCapacityWorker1 = defaultWorker;
                insertTpk.HistoricalCapacityWorker2 = defaultWorker;
                insertTpk.HistoricalCapacityWorker3 = defaultWorker;
                insertTpk.HistoricalCapacityWorker4 = defaultWorker;
                insertTpk.HistoricalCapacityWorker5 = defaultWorker;
                insertTpk.HistoricalCapacityWorker6 = defaultWorker;
                insertTpk.HistoricalCapacityWorker7 = defaultWorker;

                insertTpk.HistoricalCapacityGroup1 = defaultGroup;
                insertTpk.HistoricalCapacityGroup2 = defaultGroup;
                insertTpk.HistoricalCapacityGroup3 = defaultGroup;
                insertTpk.HistoricalCapacityGroup4 = defaultGroup;
                insertTpk.HistoricalCapacityGroup5 = defaultGroup;
                insertTpk.HistoricalCapacityGroup6 = defaultGroup;
                insertTpk.HistoricalCapacityGroup7 = defaultGroup;

                insertTpk.KPSYear = year;
                insertTpk.KPSWeek = week;

                _planTPOTPKRepo.Insert(insertTpk);

                _uow.SaveChanges();

                return Mapper.Map<TPOTPKDTO>(insertTpk);
            }
            else
            {
                return Mapper.Map<TPOTPKDTO>(updatedTpk);
            }

        }

        public PlanTPOTPKTotalBoxDTO SaveTPOTPKTotal(PlanTPOTPKTotalBoxDTO tpoTPKTotal)
        {
            var dbTPOTPKTotal = _planTPOTPKInBoxRepo.GetByID(tpoTPKTotal.KPSYear, tpoTPKTotal.KPSWeek, tpoTPKTotal.LocationCode, tpoTPKTotal.BrandCode);

            if (dbTPOTPKTotal == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            tpoTPKTotal.CreatedBy = dbTPOTPKTotal.CreatedBy;
            tpoTPKTotal.CreatedDate = dbTPOTPKTotal.CreatedDate;

            //set update time

            Mapper.Map(tpoTPKTotal, dbTPOTPKTotal);          
            dbTPOTPKTotal.UpdatedDate = DateTime.Now;

            _planTPOTPKInBoxRepo.Update(dbTPOTPKTotal);
            _uow.SaveChanges();
            return tpoTPKTotal;
        }

        public GenericValuePerWeekDTO<decimal> TotalBoxCalculation(CalculateTPOTPKInput InputTPOTPK, PlanTPOTPKTotalBoxDTO TotalBox)
        {
            var response = new GenericValuePerWeekDTO<decimal>();
            // WPP For Total Box
            var brandForBox = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = InputTPOTPK.BrandCode });
            var wpp = _planWeeklyProductionPlaningRepo.GetByID(InputTPOTPK.KPSYear, InputTPOTPK.KPSWeek, InputTPOTPK.BrandCode, InputTPOTPK.LocationCode);            
            var targetWPP = wpp.Value1 * Constants.WPPConvert / brandForBox.StickPerBox;

            var TSystem = (TotalBox.TargetSystem1 + TotalBox.TargetSystem2 + TotalBox.TargetSystem3 +
                           TotalBox.TargetSystem4 + TotalBox.TargetSystem5 + TotalBox.TargetSystem6 +
                           TotalBox.TargetSystem7);

            response.Value1 = InputTPOTPK.HeaderProcessWorkHours[0] != 0 ? targetWPP.GetValueOrDefault() - (decimal)TSystem : 0;
            response.Value2 = InputTPOTPK.HeaderProcessWorkHours[1] != 0 ? targetWPP.GetValueOrDefault() - (decimal)TSystem : 0;
            response.Value3 = InputTPOTPK.HeaderProcessWorkHours[2] != 0 ? targetWPP.GetValueOrDefault() - (decimal)TSystem : 0;
            response.Value4 = InputTPOTPK.HeaderProcessWorkHours[3] != 0 ? targetWPP.GetValueOrDefault() - (decimal)TSystem : 0;
            response.Value5 = InputTPOTPK.HeaderProcessWorkHours[4] != 0 ? targetWPP.GetValueOrDefault() - (decimal)TSystem : 0;
            response.Value6 = InputTPOTPK.HeaderProcessWorkHours[5] != 0 ? targetWPP.GetValueOrDefault() - (decimal)TSystem : 0;
            response.Value7 = InputTPOTPK.HeaderProcessWorkHours[6] != 0 ? targetWPP.GetValueOrDefault() - (decimal)TSystem : 0;
            

            return response;
        }

        public PlanTPOTPKTotalBoxDTO TotalBoxCalc(CalculateTPOTPKInput InputTPOTPK, List<DateTime> weekDate)
        {
            // TODO : CONFIRM MAS BAGUS MASLAH PEMBOBOTAN BY WH
            // box manual as user input
            var totalBox = new PlanTPOTPKTotalBoxDTO();
            //totalBox.TargetManual1 = InputTPOTPK.TotalTPOTPK[1].TargetManual1;
            //totalBox.TargetManual2 = InputTPOTPK.TotalTPOTPK[1].TargetManual2;
            //totalBox.TargetManual3 = InputTPOTPK.TotalTPOTPK[1].TargetManual3;
            //totalBox.TargetManual4 = InputTPOTPK.TotalTPOTPK[1].TargetManual4;
            //totalBox.TargetManual5 = InputTPOTPK.TotalTPOTPK[1].TargetManual5;
            //totalBox.TargetManual6 = InputTPOTPK.TotalTPOTPK[1].TargetManual6;
            //totalBox.TargetManual7 = InputTPOTPK.TotalTPOTPK[1].TargetManual7;

            // GREATEST PROCESS ORDER WH sebagai pembagi
            //var mstGenProcessSettingLocation = _processSettingLocationRepo.Get(p => p.LocationCode == InputTPOTPK.LocationCode).FirstOrDefault();
            //var mstBrand = _masterDataBll.GetBrand(new GetBrandInput() { BrandCode = InputTPOTPK.BrandCode });
            //var brandGroupCode = "";
            //if (mstBrand != null)
            //    brandGroupCode = mstBrand.BrandGroupCode;
            //var listProcessSettings = mstGenProcessSettingLocation.MstGenProcessSettings.Where(p => p.BrandGroupCode == brandGroupCode).OrderByDescending(p => p.MstGenProcess.ProcessOrder).Select(p => p.ProcessGroup).Distinct().ToList();
            
            //var greatestProcess = InputTPOTPK.ListTPOTPK.OrderByDescending(p => listProcessSettings.IndexOf(p.ProcessGroup)).FirstOrDefault();
            //var greatestWh = greatestProcess.PlanTPOTPK.FirstOrDefault();
            //var processWorkHours = new GenericValuePerWeekDTO<float?>()
            //{
            //    Value1 = greatestWh.ProcessWorkHours1,
            //    Value2 = greatestWh.ProcessWorkHours2,
            //    Value3 = greatestWh.ProcessWorkHours3,
            //    Value4 = greatestWh.ProcessWorkHours4,
            //    Value5 = greatestWh.ProcessWorkHours5,
            //    Value6 = greatestWh.ProcessWorkHours6,
            //    Value7 = greatestWh.ProcessWorkHours7
            //};
            //var totalWH = processWorkHours.Value1 + processWorkHours.Value2 + processWorkHours.Value3 + processWorkHours.Value4 + processWorkHours.Value5 + processWorkHours.Value6 + processWorkHours.Value7;
            
            //var totalTS = InputTPOTPK.TotalTPOTPK[1].TotalTargetSystem;

            //totalBox.TargetSystem7 = (float)Math.Floor((Decimal)(processWorkHours.Value7 / totalWH * totalTS));
            //totalBox.TargetSystem6 = (float)Math.Floor((Decimal)(processWorkHours.Value6 / totalWH * totalTS));
            //totalBox.TargetSystem5 = (float)Math.Floor((Decimal)(processWorkHours.Value5 / totalWH * totalTS));
            //totalBox.TargetSystem4 = (float)Math.Floor((Decimal)(processWorkHours.Value4 / totalWH * totalTS));
            //totalBox.TargetSystem3 = (float)Math.Floor((Decimal)(processWorkHours.Value3 / totalWH * totalTS));
            //totalBox.TargetSystem2 = (float)Math.Floor((Decimal)(processWorkHours.Value2 / totalWH * totalTS));
            //totalBox.TargetSystem1 = totalTS -
            //                         (totalBox.TargetSystem2 + totalBox.TargetSystem3 + totalBox.TargetSystem4 +
            //                          totalBox.TargetSystem5 + totalBox.TargetSystem6 + totalBox.TargetSystem7);

            //return totalBox;
            //return InputTPOTPK.TotalTPOTPK[1];
            // code below is skipped

            if (InputTPOTPK.IsWhChanged)
            {
                //var totalBox = new PlanTPOTPKTotalBoxDTO();
                var calculatedBox = new PlanTPOTPKTotalBoxDTO();
                var weightedDaily = new float[7];
                var totalRollingAlocation = InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Sum(m => m.WorkerAlocation);
                var dailyAttendance = new float[7];
                var dailyProductivity = new float[7];

                #region set daily attendance

                dailyAttendance[0] = (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.PercentAttendance1);
                dailyAttendance[1] = (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.PercentAttendance2);
                dailyAttendance[2] = (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.PercentAttendance3);
                dailyAttendance[3] = (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.PercentAttendance4);
                dailyAttendance[4] = (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.PercentAttendance5);
                dailyAttendance[5] = (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.PercentAttendance6);
                dailyAttendance[6] = (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.PercentAttendance7);

                #endregion

                #region set daily productivity

                dailyProductivity[0] =
                    (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.HistoricalCapacityGroup1);
                dailyProductivity[1] =
                    (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.HistoricalCapacityGroup2);
                dailyProductivity[2] =
                    (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.HistoricalCapacityGroup3);
                dailyProductivity[3] =
                    (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.HistoricalCapacityGroup4);
                dailyProductivity[4] =
                    (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.HistoricalCapacityGroup5);
                dailyProductivity[5] =
                    (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.HistoricalCapacityGroup6);
                dailyProductivity[6] =
                    (float) InputTPOTPK.ListTPOTPK[0].PlanTPOTPK.Average(m => m.HistoricalCapacityGroup7);

                #endregion

                // calculate wighted daily 
                for (var i = 0; i < 7; i++)
                {
                    if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[i] >= InputTPOTPK.FilterCurrentDayForward) ||
                        (!InputTPOTPK.IsFilterCurrentDayForward))
                    {
                        weightedDaily[i] = (float) totalRollingAlocation*dailyAttendance[i]*dailyProductivity[i]*
                                           InputTPOTPK.HeaderProcessWorkHours[i];
                    }
                    else
                    {
                        weightedDaily[i] = 0;
                    }
                }

                var totalWeightedDaily = weightedDaily.Sum();

                // get calculated total box based on current day forward filter

                #region set calculated

                calculatedBox.TargetSystem1 = 0;
                calculatedBox.TargetSystem2 = 0;
                calculatedBox.TargetSystem3 = 0;
                calculatedBox.TargetSystem4 = 0;
                calculatedBox.TargetSystem5 = 0;
                calculatedBox.TargetSystem6 = 0;
                calculatedBox.TargetSystem7 = 0;
                calculatedBox.TargetManual1 = 0;
                calculatedBox.TargetManual2 = 0;
                calculatedBox.TargetManual3 = 0;
                calculatedBox.TargetManual4 = 0;
                calculatedBox.TargetManual5 = 0;
                calculatedBox.TargetManual6 = 0;
                calculatedBox.TargetManual7 = 0;

                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    calculatedBox.TargetSystem1 = InputTPOTPK.TotalTPOTPK[1].TargetSystem1;
                    calculatedBox.TargetManual1 = InputTPOTPK.TotalTPOTPK[1].TargetManual1;
                }
                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    calculatedBox.TargetSystem2 = InputTPOTPK.TotalTPOTPK[1].TargetSystem2;
                    calculatedBox.TargetManual2 = InputTPOTPK.TotalTPOTPK[1].TargetManual2;
                }
                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    calculatedBox.TargetSystem3 = InputTPOTPK.TotalTPOTPK[1].TargetSystem3;
                    calculatedBox.TargetManual3 = InputTPOTPK.TotalTPOTPK[1].TargetManual3;
                }
                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    calculatedBox.TargetSystem4 = InputTPOTPK.TotalTPOTPK[1].TargetSystem4;
                    calculatedBox.TargetManual4 = InputTPOTPK.TotalTPOTPK[1].TargetManual4;
                }
                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    calculatedBox.TargetSystem5 = InputTPOTPK.TotalTPOTPK[1].TargetSystem5;
                    calculatedBox.TargetManual5 = InputTPOTPK.TotalTPOTPK[1].TargetManual5;
                }
                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    calculatedBox.TargetSystem6 = InputTPOTPK.TotalTPOTPK[1].TargetSystem6;
                    calculatedBox.TargetManual6 = InputTPOTPK.TotalTPOTPK[1].TargetManual6;
                }
                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    calculatedBox.TargetSystem7 = InputTPOTPK.TotalTPOTPK[1].TargetSystem7;
                    calculatedBox.TargetManual7 = InputTPOTPK.TotalTPOTPK[1].TargetManual7;
                }

                #endregion

                var sumCalculatedBoxSystem = calculatedBox.TargetSystem1 + calculatedBox.TargetSystem2 +
                                             calculatedBox.TargetSystem3 + calculatedBox.TargetSystem4 +
                                             calculatedBox.TargetSystem5 + calculatedBox.TargetSystem6 +
                                             calculatedBox.TargetSystem7;
                var sumCalculatedBoxManual = calculatedBox.TargetManual1 + calculatedBox.TargetManual2 +
                                             calculatedBox.TargetManual3 + calculatedBox.TargetManual4 +
                                             calculatedBox.TargetManual5 + calculatedBox.TargetManual6 +
                                             calculatedBox.TargetManual7;

                var sisaSystem = sumCalculatedBoxSystem;
                var sisaManual = sumCalculatedBoxManual;

                // assign total box

                #region assign total box

                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    totalBox.TargetSystem1 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxSystem*weightedDaily[0]/totalWeightedDaily));
                    if (sisaSystem < totalBox.TargetSystem1)
                    {
                        totalBox.TargetSystem1 = sisaSystem;
                    }
                    else
                    {
                        sisaSystem = sisaSystem - totalBox.TargetSystem1;
                    }
                    totalBox.TargetManual1 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxManual*weightedDaily[0]/totalWeightedDaily));
                    if (sisaManual < totalBox.TargetManual1)
                    {
                        totalBox.TargetManual1 = sisaManual;
                    }
                    else
                    {
                        sisaManual = sisaManual - totalBox.TargetManual1;
                    }
                }
                else
                {
                    totalBox.TargetSystem1 = InputTPOTPK.TotalTPOTPK[1].TargetSystem1;
                    totalBox.TargetManual1 = InputTPOTPK.TotalTPOTPK[1].TargetManual1;
                }

                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    totalBox.TargetSystem2 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxSystem*weightedDaily[1]/totalWeightedDaily));
                    if (sisaSystem < totalBox.TargetSystem2)
                    {
                        totalBox.TargetSystem2 = sisaSystem;
                    }
                    else
                    {
                        sisaSystem = sisaSystem - totalBox.TargetSystem2;
                    }
                    totalBox.TargetManual2 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxManual*weightedDaily[1]/totalWeightedDaily));
                    if (sisaManual < totalBox.TargetManual2)
                    {
                        totalBox.TargetManual2 = sisaManual;
                    }
                    else
                    {
                        sisaManual = sisaManual - totalBox.TargetManual2;
                    }
                }
                else
                {
                    totalBox.TargetSystem2 = InputTPOTPK.TotalTPOTPK[1].TargetSystem2;
                    totalBox.TargetManual2 = InputTPOTPK.TotalTPOTPK[1].TargetManual2;
                }

                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    totalBox.TargetSystem3 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxSystem*weightedDaily[2]/totalWeightedDaily));
                    if (sisaSystem < totalBox.TargetSystem3)
                    {
                        totalBox.TargetSystem3 = sisaSystem;
                    }
                    else
                    {
                        sisaSystem = sisaSystem - totalBox.TargetSystem3;
                    }
                    totalBox.TargetManual3 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxManual*weightedDaily[2]/totalWeightedDaily));
                    if (sisaManual < totalBox.TargetManual3)
                    {
                        totalBox.TargetManual3 = sisaManual;
                    }
                    else
                    {
                        sisaManual = sisaManual - totalBox.TargetManual3;
                    }
                }
                else
                {
                    totalBox.TargetSystem3 = InputTPOTPK.TotalTPOTPK[1].TargetSystem3;
                    totalBox.TargetManual3 = InputTPOTPK.TotalTPOTPK[1].TargetManual3;
                }

                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    totalBox.TargetSystem4 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxSystem*weightedDaily[3]/totalWeightedDaily));
                    if (sisaSystem < totalBox.TargetSystem4)
                    {
                        totalBox.TargetSystem4 = sisaSystem;
                    }
                    else
                    {
                        sisaSystem = sisaSystem - totalBox.TargetSystem4;
                    }
                    totalBox.TargetManual4 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxManual*weightedDaily[3]/totalWeightedDaily));
                    if (sisaManual < totalBox.TargetManual4)
                    {
                        totalBox.TargetManual4 = sisaManual;
                    }
                    else
                    {
                        sisaManual = sisaManual - totalBox.TargetManual4;
                    }
                }
                else
                {
                    totalBox.TargetSystem4 = InputTPOTPK.TotalTPOTPK[1].TargetSystem4;
                    totalBox.TargetManual4 = InputTPOTPK.TotalTPOTPK[1].TargetManual4;
                }

                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    totalBox.TargetSystem5 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxSystem*weightedDaily[4]/totalWeightedDaily));
                    if (sisaSystem < totalBox.TargetSystem5)
                    {
                        totalBox.TargetSystem5 = sisaSystem;
                    }
                    else
                    {
                        sisaSystem = sisaSystem - totalBox.TargetSystem5;
                    }
                    totalBox.TargetManual5 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxManual*weightedDaily[4]/totalWeightedDaily));
                    if (sisaManual < totalBox.TargetManual5)
                    {
                        totalBox.TargetManual5 = sisaManual;
                    }
                    else
                    {
                        sisaManual = sisaManual - totalBox.TargetManual5;
                    }
                }
                else
                {
                    totalBox.TargetSystem5 = InputTPOTPK.TotalTPOTPK[1].TargetSystem5;
                    totalBox.TargetManual5 = InputTPOTPK.TotalTPOTPK[1].TargetManual5;
                }

                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    totalBox.TargetSystem6 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxSystem*weightedDaily[5]/totalWeightedDaily));
                    if (sisaSystem < totalBox.TargetSystem6)
                    {
                        totalBox.TargetSystem6 = sisaSystem;
                    }
                    else
                    {
                        sisaSystem = sisaSystem - totalBox.TargetSystem6;
                    }
                    totalBox.TargetManual6 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxManual*weightedDaily[5]/totalWeightedDaily));
                    if (sisaManual < totalBox.TargetManual6)
                    {
                        totalBox.TargetManual6 = sisaManual;
                    }
                    else
                    {
                        sisaManual = sisaManual - totalBox.TargetManual6;
                    }
                }
                else
                {
                    totalBox.TargetSystem6 = InputTPOTPK.TotalTPOTPK[1].TargetSystem6;
                    totalBox.TargetManual6 = InputTPOTPK.TotalTPOTPK[1].TargetManual6;
                }

                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) ||
                    (!InputTPOTPK.IsFilterCurrentDayForward))
                {
                    totalBox.TargetSystem7 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxSystem*weightedDaily[6]/totalWeightedDaily));
                    if (sisaSystem < totalBox.TargetSystem7)
                    {
                        totalBox.TargetSystem7 = sisaSystem;
                    }
                    else
                    {
                        sisaSystem = sisaSystem - totalBox.TargetSystem7;
                    }
                    totalBox.TargetManual7 =
                        (float) Math.Ceiling((Decimal) (sumCalculatedBoxManual*weightedDaily[6]/totalWeightedDaily));
                    if (sisaManual < totalBox.TargetManual7)
                    {
                        totalBox.TargetManual7 = sisaManual;
                    }
                    else
                    {
                        sisaManual = sisaManual - totalBox.TargetManual7;
                    }
                }
                else
                {
                    totalBox.TargetSystem7 = InputTPOTPK.TotalTPOTPK[1].TargetSystem7;
                    totalBox.TargetManual7 = InputTPOTPK.TotalTPOTPK[1].TargetManual7;
                }

                #endregion
            }
            else
            {
                totalBox.TargetSystem1 = InputTPOTPK.TotalTPOTPK[1].TargetSystem1;
                totalBox.TargetSystem2 = InputTPOTPK.TotalTPOTPK[1].TargetSystem2;
                totalBox.TargetSystem3 = InputTPOTPK.TotalTPOTPK[1].TargetSystem3;
                totalBox.TargetSystem4 = InputTPOTPK.TotalTPOTPK[1].TargetSystem4;
                totalBox.TargetSystem5 = InputTPOTPK.TotalTPOTPK[1].TargetSystem5;
                totalBox.TargetSystem6 = InputTPOTPK.TotalTPOTPK[1].TargetSystem6;
                totalBox.TargetSystem7 = InputTPOTPK.TotalTPOTPK[1].TargetSystem7;
            }
            
            totalBox.TargetManual1 = InputTPOTPK.TotalTPOTPK[1].TargetManual1;
            totalBox.TargetManual2 = InputTPOTPK.TotalTPOTPK[1].TargetManual2;
            totalBox.TargetManual3 = InputTPOTPK.TotalTPOTPK[1].TargetManual3;
            totalBox.TargetManual4 = InputTPOTPK.TotalTPOTPK[1].TargetManual4;
            totalBox.TargetManual5 = InputTPOTPK.TotalTPOTPK[1].TargetManual5;
            totalBox.TargetManual6 = InputTPOTPK.TotalTPOTPK[1].TargetManual6;
            totalBox.TargetManual7 = InputTPOTPK.TotalTPOTPK[1].TargetManual7;

            return totalBox;
        }

        private List<TPOTPKDTO> fillEmptyAllocation(List<TPOTPKDTO> tpoTpk)
        {
            foreach (var t in tpoTpk)
            {
                t.TargetManual1 = 0;
                t.TargetManual2 = 0;
                t.TargetManual3 = 0;
                t.TargetManual4 = 0;
                t.TargetManual5 = 0;
                t.TargetManual6 = 0;
                t.TargetManual7 = 0;
                t.TargetSystem1 = 0;
                t.TargetSystem2 = 0;
                t.TargetSystem3 = 0;
                t.TargetSystem4 = 0;
                t.TargetSystem5 = 0;
                t.TargetSystem6 = 0;
                t.TargetSystem7 = 0;
                t.TotalTargetManual = 0;
                t.TotalTargetSystem = 0;
            }

            return tpoTpk;
        }

        public CalculateTPOTPKCheck CheckGroup(TPOTPKDTO TPOTPKByGroup, List<DateTime> weekDate){
            // pengecekan inactive group
            var groupInactive = _masterDataBll.GetTpoProductionGroupById(TPOTPKByGroup.ProdGroup, TPOTPKByGroup.ProcessGroup, TPOTPKByGroup.LocationCode, TPOTPKByGroup.StatusEmp);
            bool groupStatus = true;
            DateTime updateDate = weekDate[0];
            if (!(bool)groupInactive.StatusActive)
            {
                groupStatus = false;
                updateDate = groupInactive.UpdatedDate.Date;
            }

            var result = new CalculateTPOTPKCheck();
            result.updateDate = updateDate;
            result.groupStatus = groupStatus;

            return result;
        }

        public TPOTPKCalculateDTO CalculateTPOTPK(CalculateTPOTPKInput InputTPOTPK)
        {
            var UomEblek = _masterDataBll.GetUOMEblekByBrandCode(InputTPOTPK.BrandCode, InputTPOTPK.LocationCode);
            var weekDate = _masterDataBll.GetDateByWeek(InputTPOTPK.KPSYear, InputTPOTPK.KPSWeek);

            // Totals
            //var TotalStick = InputTPOTPK.TotalTPOTPK.Where(t => t.TotalType.ToUpper() == Enums.Conversion.Stick.ToString().ToUpper()).FirstOrDefault();
            //var TotalBox = InputTPOTPK.TotalTPOTPK.Where(t => t.TotalType.ToUpper() == Enums.Conversion.Box.ToString().ToUpper()).FirstOrDefault();

            var TotalBox = TotalBoxCalc(InputTPOTPK, weekDate);

            var total = new List<PlanTPOTPKTotalBoxDTO>();

            // Global Variable
            var currentProcess = "";
            var previousProcess = "";
            var previousProcessWorkHours = new GenericValuePerWeekDTO<float?>(); // greates process order work hours
            var currentProcessWorkHours = new GenericValuePerWeekDTO<float?>();

            var TotalDailyTargetSystemWrappingStamping = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetManualWrappingStamping = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetSystemPacking = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetManualPacking = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetSystemStickWrapping = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetManualStickWrapping = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetSystemCutting = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetManualCutting = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetSystemRolling = new GenericValuePerWeekDTO<float?>();
            var TotalDailyTargetManualRolling = new GenericValuePerWeekDTO<float?>();
            var TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>();
            int? UOMEblekPacking = 0;
            int? UOMEblekWrapping = 0;

            // Get TPO TPK Box
            var DialyBox = _planTPOTPKInBoxRepo.GetByID(InputTPOTPK.KPSYear, InputTPOTPK.KPSWeek, InputTPOTPK.LocationCode, InputTPOTPK.BrandCode);

            #region Get Ordered Process from Process Settings
            var mstGenProcessSettingLocation = _processSettingLocationRepo.Get(p => p.LocationCode == InputTPOTPK.LocationCode).FirstOrDefault();
            var mstBrand = _masterDataBll.GetBrand(new GetBrandInput() { BrandCode = InputTPOTPK.BrandCode });
            var brandGroupCode = "";
            if (mstBrand != null)
                brandGroupCode = mstBrand.BrandGroupCode;
            var listProcessSettings = mstGenProcessSettingLocation.MstGenProcessSettings.Where(p => p.BrandGroupCode == brandGroupCode).OrderByDescending(p => p.MstGenProcess.ProcessOrder).Select(p => p.ProcessGroup).Distinct().ToList();
            #endregion

            // GREATEST PROCESS ORDER WH sebagai pembagi
            //var greatestProcess = InputTPOTPK.ListTPOTPK.OrderBy(p => listProcessSettings.IndexOf(p.ProcessGroup)).FirstOrDefault();
            //var greatestWh = greatestProcess.PlanTPOTPK.FirstOrDefault();
            //previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
            //{
            //    Value1 = greatestWh.ProcessWorkHours1,
            //    Value2 = greatestWh.ProcessWorkHours2,
            //    Value3 = greatestWh.ProcessWorkHours3,
            //    Value4 = greatestWh.ProcessWorkHours4,
            //    Value5 = greatestWh.ProcessWorkHours5,
            //    Value6 = greatestWh.ProcessWorkHours6,
            //    Value7 = greatestWh.ProcessWorkHours7
            //};

            // GREATEST WH from all process sebagai pembagi
            previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
            {
                Value1 = 0,
                Value2 = 0,
                Value3 = 0,
                Value4 = 0,
                Value5 = 0,
                Value6 = 0,
                Value7 = 0
            };
            foreach (var TPOTPKByProcess in InputTPOTPK.ListTPOTPK.OrderBy(p => listProcessSettings.IndexOf(p.ProcessGroup)))
            {
                var wh = TPOTPKByProcess.PlanTPOTPK.FirstOrDefault();
                if (wh.ProcessWorkHours1 > previousProcessWorkHours.Value1) { previousProcessWorkHours.Value1 = wh.ProcessWorkHours1; }
                if (wh.ProcessWorkHours2 > previousProcessWorkHours.Value2) { previousProcessWorkHours.Value2 = wh.ProcessWorkHours2; }
                if (wh.ProcessWorkHours3 > previousProcessWorkHours.Value3) { previousProcessWorkHours.Value3 = wh.ProcessWorkHours3; }
                if (wh.ProcessWorkHours4 > previousProcessWorkHours.Value4) { previousProcessWorkHours.Value4 = wh.ProcessWorkHours4; }
                if (wh.ProcessWorkHours5 > previousProcessWorkHours.Value5) { previousProcessWorkHours.Value5 = wh.ProcessWorkHours5; }
                if (wh.ProcessWorkHours6 > previousProcessWorkHours.Value6) { previousProcessWorkHours.Value6 = wh.ProcessWorkHours6; }
                if (wh.ProcessWorkHours7 > previousProcessWorkHours.Value7) { previousProcessWorkHours.Value7 = wh.ProcessWorkHours7; }
            }

            int groupIndex;
            bool groupEmptyAllocation;

            #region Loop By Process

            foreach (var TPOTPKByProcess in InputTPOTPK.ListTPOTPK.OrderBy(p => listProcessSettings.IndexOf(p.ProcessGroup)))
            {

                #region fill null variance wip

                if (TPOTPKByProcess.WIPPreviousValue == null) TPOTPKByProcess.WIPPreviousValue = 0;
                if (TPOTPKByProcess.WIPStock1 == null) TPOTPKByProcess.WIPStock1 = 0;
                if (TPOTPKByProcess.WIPStock2 == null) TPOTPKByProcess.WIPStock2 = 0;
                if (TPOTPKByProcess.WIPStock3 == null) TPOTPKByProcess.WIPStock3 = 0;
                if (TPOTPKByProcess.WIPStock4 == null) TPOTPKByProcess.WIPStock4 = 0;
                if (TPOTPKByProcess.WIPStock5 == null) TPOTPKByProcess.WIPStock5 = 0;
                if (TPOTPKByProcess.WIPStock6 == null) TPOTPKByProcess.WIPStock6 = 0;
                if (TPOTPKByProcess.WIPStock7 == null) TPOTPKByProcess.WIPStock7 = 0;

                #endregion
                // Set CURRENT Process State
                currentProcess = TPOTPKByProcess.ProcessGroup;
                groupIndex = 0;
                groupEmptyAllocation = true;

                #region Get UOMEblek and StickPerBox Value By Process
                var uomEblek = UomEblek.FirstOrDefault(u => u.ProcessGroup == TPOTPKByProcess.ProcessGroup);
                var convertUOM = uomEblek.UOMEblek != null ? uomEblek.UOMEblek : 0;
                var brand = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = TPOTPKByProcess.BrandCode });
                #endregion

                #region Get Current Process Work Hour By Process
                var CurrentProcessWorkHours = TPOTPKByProcess.PlanTPOTPK.FirstOrDefault();
                if (CurrentProcessWorkHours != null)
                {
                    currentProcessWorkHours = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = CurrentProcessWorkHours.ProcessWorkHours1,
                        Value2 = CurrentProcessWorkHours.ProcessWorkHours2,
                        Value3 = CurrentProcessWorkHours.ProcessWorkHours3,
                        Value4 = CurrentProcessWorkHours.ProcessWorkHours4,
                        Value5 = CurrentProcessWorkHours.ProcessWorkHours5,
                        Value6 = CurrentProcessWorkHours.ProcessWorkHours6,
                        Value7 = CurrentProcessWorkHours.ProcessWorkHours7
                    };
                }
                #endregion

                #region Main Conditional Block

                if ((TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Wrapping.ToString().ToUpper()) ||
                    (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Stamping.ToString().ToUpper()))
                {

                    int index = 0;
                    foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        TPOTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        TPOTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        TPOTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        TPOTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        TPOTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        TPOTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        TPOTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroup(TPOTPKByGroup, weekDate); // pengecekan inactive group
                        #region Group check
                        if ((!c.groupStatus && weekDate[0] >= c.updateDate) || (currentProcessWorkHours.Value1 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult1 = 0;
                        }
                        if ((!c.groupStatus && weekDate[1] >= c.updateDate) || (currentProcessWorkHours.Value2 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult2 = 0;
                        }
                        if ((!c.groupStatus && weekDate[2] >= c.updateDate) || (currentProcessWorkHours.Value3 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult3 = 0;
                        }
                        if ((!c.groupStatus && weekDate[3] >= c.updateDate) || (currentProcessWorkHours.Value4 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult4 = 0;
                        }
                        if ((!c.groupStatus && weekDate[4] >= c.updateDate) || (currentProcessWorkHours.Value5 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult5 = 0;
                        }
                        if ((!c.groupStatus && weekDate[5] >= c.updateDate) || (currentProcessWorkHours.Value6 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult6 = 0;
                        }
                        if ((!c.groupStatus && weekDate[6] >= c.updateDate) || (currentProcessWorkHours.Value7 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult7 = 0;
                        }
                        #endregion
                        
                        if (groupEmptyAllocation)
                        {
                            if (TPOTPKByGroup.WorkerAlocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                        {
                            #region Assign target
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * TotalBox.TargetSystem1 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * TotalBox.TargetManual1 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                   
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * TotalBox.TargetSystem2 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * TotalBox.TargetManual2 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                           
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * TotalBox.TargetSystem3 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * TotalBox.TargetManual3 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * TotalBox.TargetSystem4 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * TotalBox.TargetManual4 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * TotalBox.TargetSystem5 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * TotalBox.TargetManual5 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * TotalBox.TargetSystem6 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * TotalBox.TargetManual6 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * TotalBox.TargetSystem7 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * TotalBox.TargetManual7 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                            }

                            TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;

                            #endregion
                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlanTPOTPK = fillEmptyAllocation(TPOTPKByProcess.PlanTPOTPK);
                    }
                    

                    // Variables for Next Process
                    TotalDailyTargetSystemWrappingStamping = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualWrappingStamping = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        var ratio = brand.StickPerBox / convertUOM;
                        // System
                        if (((TotalBox.TargetSystem1 * ratio) != TotalDailyTargetSystemWrappingStamping.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 + ((TotalBox.TargetSystem1 * ratio) - TotalDailyTargetSystemWrappingStamping.Value1);
                            TotalDailyTargetSystemWrappingStamping.Value1 = TotalBox.TargetSystem1 * ratio;
                        }
                        if (((TotalBox.TargetSystem2 * ratio) != TotalDailyTargetSystemWrappingStamping.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 + ((TotalBox.TargetSystem2 * ratio) - TotalDailyTargetSystemWrappingStamping.Value2);
                            TotalDailyTargetSystemWrappingStamping.Value2 = TotalBox.TargetSystem2 * ratio;
                        }
                        if (((TotalBox.TargetSystem3 * ratio) != TotalDailyTargetSystemWrappingStamping.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 + ((TotalBox.TargetSystem3 * ratio) - TotalDailyTargetSystemWrappingStamping.Value3);
                            TotalDailyTargetSystemWrappingStamping.Value3 = TotalBox.TargetSystem3 * ratio;
                        }
                        if (((TotalBox.TargetSystem4 * ratio) != TotalDailyTargetSystemWrappingStamping.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 + ((TotalBox.TargetSystem4 * ratio) - TotalDailyTargetSystemWrappingStamping.Value4);
                            TotalDailyTargetSystemWrappingStamping.Value4 = TotalBox.TargetSystem4 * ratio;
                        }
                        if (((TotalBox.TargetSystem5 * ratio) != TotalDailyTargetSystemWrappingStamping.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 + ((TotalBox.TargetSystem5 * ratio) - TotalDailyTargetSystemWrappingStamping.Value5);
                            TotalDailyTargetSystemWrappingStamping.Value5 = TotalBox.TargetSystem5 * ratio;
                        }
                        if (((TotalBox.TargetSystem6 * ratio) != TotalDailyTargetSystemWrappingStamping.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 + ((TotalBox.TargetSystem6 * ratio) - TotalDailyTargetSystemWrappingStamping.Value6);
                            TotalDailyTargetSystemWrappingStamping.Value6 = TotalBox.TargetSystem6 * ratio;
                        }
                        if (((TotalBox.TargetSystem7 * ratio) != TotalDailyTargetSystemWrappingStamping.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 + ((TotalBox.TargetSystem7 * ratio) - TotalDailyTargetSystemWrappingStamping.Value7);
                            TotalDailyTargetSystemWrappingStamping.Value7 = TotalBox.TargetSystem7 * ratio;
                        }

                        // Manual
                        if (((TotalBox.TargetManual1 * ratio) != TotalDailyTargetManualWrappingStamping.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 + ((TotalBox.TargetManual1 * ratio) - TotalDailyTargetManualWrappingStamping.Value1);
                            TotalDailyTargetManualWrappingStamping.Value1 = TotalBox.TargetManual1 * ratio;
                        }
                        if (((TotalBox.TargetManual2 * ratio) != TotalDailyTargetManualWrappingStamping.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 + ((TotalBox.TargetManual2 * ratio) - TotalDailyTargetManualWrappingStamping.Value2);
                            TotalDailyTargetManualWrappingStamping.Value2 = TotalBox.TargetManual2 * ratio;
                        }
                        if (((TotalBox.TargetManual3 * ratio) != TotalDailyTargetManualWrappingStamping.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 + ((TotalBox.TargetManual3 * ratio) - TotalDailyTargetManualWrappingStamping.Value3);
                            TotalDailyTargetManualWrappingStamping.Value3 = TotalBox.TargetManual3 * ratio;
                        }
                        if (((TotalBox.TargetManual4 * ratio) != TotalDailyTargetManualWrappingStamping.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 + ((TotalBox.TargetManual4 * ratio) - TotalDailyTargetManualWrappingStamping.Value4);
                            TotalDailyTargetManualWrappingStamping.Value4 = TotalBox.TargetManual4 * ratio;
                        }
                        if (((TotalBox.TargetManual5 * ratio) != TotalDailyTargetManualWrappingStamping.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 + ((TotalBox.TargetManual5 * ratio) - TotalDailyTargetManualWrappingStamping.Value5);
                            TotalDailyTargetManualWrappingStamping.Value5 = TotalBox.TargetManual5 * ratio;
                        }
                        if (((TotalBox.TargetManual6 * ratio) != TotalDailyTargetManualWrappingStamping.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 + ((TotalBox.TargetManual6 * ratio) - TotalDailyTargetManualWrappingStamping.Value6);
                            TotalDailyTargetManualWrappingStamping.Value6 = TotalBox.TargetManual6 * ratio;
                        }
                        if (((TotalBox.TargetManual7 * ratio) != TotalDailyTargetManualWrappingStamping.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 + ((TotalBox.TargetManual7 * ratio) - TotalDailyTargetManualWrappingStamping.Value7);
                            TotalDailyTargetManualWrappingStamping.Value7 = TotalBox.TargetManual7 * ratio;
                        }
                        #endregion
                    }

                    UOMEblekWrapping = convertUOM;

                }
                else if (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Packing.ToString().ToUpper())
                {
                    int index = 0;
                    foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        TPOTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        TPOTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        TPOTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        TPOTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        TPOTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        TPOTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        TPOTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroup(TPOTPKByGroup, weekDate); // pengecekan inactive group
                        #region Group check
                        if ((!c.groupStatus && weekDate[0] >= c.updateDate) || (currentProcessWorkHours.Value1 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult1 = 0;
                        }
                        if ((!c.groupStatus && weekDate[1] >= c.updateDate) || (currentProcessWorkHours.Value2 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult2 = 0;
                        }
                        if ((!c.groupStatus && weekDate[2] >= c.updateDate) || (currentProcessWorkHours.Value3 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult3 = 0;
                        }
                        if ((!c.groupStatus && weekDate[3] >= c.updateDate) || (currentProcessWorkHours.Value4 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult4 = 0;
                        }
                        if ((!c.groupStatus && weekDate[4] >= c.updateDate) || (currentProcessWorkHours.Value5 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult5 = 0;
                        }
                        if ((!c.groupStatus && weekDate[5] >= c.updateDate) || (currentProcessWorkHours.Value6 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult6 = 0;
                        }
                        if ((!c.groupStatus && weekDate[6] >= c.updateDate) || (currentProcessWorkHours.Value7 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult7 = 0;
                        }
                        #endregion

                        if (groupEmptyAllocation)
                        {
                            if (TPOTPKByGroup.WorkerAlocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target7)
                    };
                    if (!groupEmptyAllocation)
                    {
                        foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                        {
                            #region Assign target
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * (TotalDailyTargetSystemWrappingStamping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * (TotalDailyTargetManualWrappingStamping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * (TotalDailyTargetSystemWrappingStamping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * (TotalDailyTargetManualWrappingStamping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * (TotalDailyTargetSystemWrappingStamping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * (TotalDailyTargetManualWrappingStamping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * (TotalDailyTargetSystemWrappingStamping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * (TotalDailyTargetManualWrappingStamping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * (TotalDailyTargetSystemWrappingStamping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * (TotalDailyTargetManualWrappingStamping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * (TotalDailyTargetSystemWrappingStamping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * (TotalDailyTargetManualWrappingStamping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * (TotalDailyTargetSystemWrappingStamping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * (TotalDailyTargetManualWrappingStamping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                            }
                            
                            TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                            #endregion
                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlanTPOTPK = fillEmptyAllocation(TPOTPKByProcess.PlanTPOTPK);
                    }
                    
                    // Variables for Next Process
                    TotalDailyTargetSystemPacking = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualPacking = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        // System
                        if ((TotalDailyTargetSystemPacking.Value1 != TotalDailyTargetSystemWrappingStamping.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 + ((TotalDailyTargetSystemWrappingStamping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) - TotalDailyTargetSystemPacking.Value1);
                            TotalDailyTargetSystemPacking.Value1 = (TotalDailyTargetSystemWrappingStamping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM);
                        }
                        if ((TotalDailyTargetSystemPacking.Value2 != TotalDailyTargetSystemWrappingStamping.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 + ((TotalDailyTargetSystemWrappingStamping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) - TotalDailyTargetSystemPacking.Value2);
                            TotalDailyTargetSystemPacking.Value2 = (TotalDailyTargetSystemWrappingStamping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM);
                        }
                        if ((TotalDailyTargetSystemPacking.Value3 != TotalDailyTargetSystemWrappingStamping.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 + ((TotalDailyTargetSystemWrappingStamping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) - TotalDailyTargetSystemPacking.Value3);
                            TotalDailyTargetSystemPacking.Value3 = (TotalDailyTargetSystemWrappingStamping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM);
                        }
                        if ((TotalDailyTargetSystemPacking.Value4 != TotalDailyTargetSystemWrappingStamping.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 + ((TotalDailyTargetSystemWrappingStamping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) - TotalDailyTargetSystemPacking.Value4);
                            TotalDailyTargetSystemPacking.Value4 = (TotalDailyTargetSystemWrappingStamping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM);
                        }
                        if ((TotalDailyTargetSystemPacking.Value5 != TotalDailyTargetSystemWrappingStamping.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 + ((TotalDailyTargetSystemWrappingStamping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) - TotalDailyTargetSystemPacking.Value5);
                            TotalDailyTargetSystemPacking.Value5 = (TotalDailyTargetSystemWrappingStamping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM);
                        }
                        if ((TotalDailyTargetSystemPacking.Value6 != TotalDailyTargetSystemWrappingStamping.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 + ((TotalDailyTargetSystemWrappingStamping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) - TotalDailyTargetSystemPacking.Value6);
                            TotalDailyTargetSystemPacking.Value6 = (TotalDailyTargetSystemWrappingStamping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM);
                        }
                        if ((TotalDailyTargetSystemPacking.Value7 != TotalDailyTargetSystemWrappingStamping.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 + ((TotalDailyTargetSystemWrappingStamping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) - TotalDailyTargetSystemPacking.Value7);
                            TotalDailyTargetSystemPacking.Value7 = (TotalDailyTargetSystemWrappingStamping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM);
                        }
                        // Manual
                        if ((TotalDailyTargetManualPacking.Value1 != TotalDailyTargetManualWrappingStamping.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 + ((TotalDailyTargetManualWrappingStamping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) - TotalDailyTargetManualPacking.Value1);
                            TotalDailyTargetManualPacking.Value1 = (TotalDailyTargetManualWrappingStamping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM);
                        }
                        if ((TotalDailyTargetManualPacking.Value2 != TotalDailyTargetManualWrappingStamping.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 + ((TotalDailyTargetManualWrappingStamping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) - TotalDailyTargetManualPacking.Value2);
                            TotalDailyTargetManualPacking.Value2 = (TotalDailyTargetManualWrappingStamping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM);
                        }
                        if ((TotalDailyTargetManualPacking.Value3 != TotalDailyTargetManualWrappingStamping.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 + ((TotalDailyTargetManualWrappingStamping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) - TotalDailyTargetManualPacking.Value3);
                            TotalDailyTargetManualPacking.Value3 = (TotalDailyTargetManualWrappingStamping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM);
                        }
                        if ((TotalDailyTargetManualPacking.Value4 != TotalDailyTargetManualWrappingStamping.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 + ((TotalDailyTargetManualWrappingStamping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) - TotalDailyTargetManualPacking.Value4);
                            TotalDailyTargetManualPacking.Value4 = (TotalDailyTargetManualWrappingStamping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM);
                        }
                        if ((TotalDailyTargetManualPacking.Value5 != TotalDailyTargetManualWrappingStamping.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 + ((TotalDailyTargetManualWrappingStamping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) - TotalDailyTargetManualPacking.Value5);
                            TotalDailyTargetManualPacking.Value5 = (TotalDailyTargetManualWrappingStamping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM);
                        }
                        if ((TotalDailyTargetManualPacking.Value6 != TotalDailyTargetManualWrappingStamping.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 + ((TotalDailyTargetManualWrappingStamping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) - TotalDailyTargetManualPacking.Value6);
                            TotalDailyTargetManualPacking.Value6 = (TotalDailyTargetManualWrappingStamping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM);
                        }
                        if ((TotalDailyTargetManualPacking.Value7 != TotalDailyTargetManualWrappingStamping.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 + ((TotalDailyTargetManualWrappingStamping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) - TotalDailyTargetManualPacking.Value7);
                            TotalDailyTargetManualPacking.Value7 = (TotalDailyTargetManualWrappingStamping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM);
                        }
                        
                        #endregion
                    }
                    UOMEblekPacking = convertUOM;

                }
                else if (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Stickwrapping.ToString().ToUpper())
                {
                    int index = 0;
                    foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        TPOTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        TPOTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        TPOTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        TPOTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        TPOTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        TPOTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        TPOTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroup(TPOTPKByGroup, weekDate); // pengecekan inactive group
                        #region Group check
                        if ((!c.groupStatus && weekDate[0] >= c.updateDate) || (currentProcessWorkHours.Value1 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult1 = 0;
                        }
                        if ((!c.groupStatus && weekDate[1] >= c.updateDate) || (currentProcessWorkHours.Value2 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult2 = 0;
                        }
                        if ((!c.groupStatus && weekDate[2] >= c.updateDate) || (currentProcessWorkHours.Value3 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult3 = 0;
                        }
                        if ((!c.groupStatus && weekDate[3] >= c.updateDate) || (currentProcessWorkHours.Value4 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult4 = 0;
                        }
                        if ((!c.groupStatus && weekDate[4] >= c.updateDate) || (currentProcessWorkHours.Value5 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult5 = 0;
                        }
                        if ((!c.groupStatus && weekDate[5] >= c.updateDate) || (currentProcessWorkHours.Value6 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult6 = 0;
                        }
                        if ((!c.groupStatus && weekDate[6] >= c.updateDate) || (currentProcessWorkHours.Value7 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult7 = 0;
                        }
                        #endregion

                        if (groupEmptyAllocation)
                        {
                            if (TPOTPKByGroup.WorkerAlocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                        {
                            #region Assign target
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                            }
                            
                            TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                         TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                         TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                         TPOTPKByGroup.TargetManual7;

                            #endregion
                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlanTPOTPK = fillEmptyAllocation(TPOTPKByProcess.PlanTPOTPK);
                    }
                    
                    // Variables for Next Process
                    TotalDailyTargetSystemStickWrapping = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualStickWrapping = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        // System
                        if ((TotalDailyTargetSystemStickWrapping.Value1 != ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 - (TotalDailyTargetSystemStickWrapping.Value1 - (((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM));
                            TotalDailyTargetSystemStickWrapping.Value1 = ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value2 != ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 - (TotalDailyTargetSystemStickWrapping.Value2 - (((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM));
                            TotalDailyTargetSystemStickWrapping.Value2 = ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value3 != ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 - (TotalDailyTargetSystemStickWrapping.Value3 - (((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM));
                            TotalDailyTargetSystemStickWrapping.Value3 = ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value4 != ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 - (TotalDailyTargetSystemStickWrapping.Value4 - (((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM));
                            TotalDailyTargetSystemStickWrapping.Value4 = ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value5 != ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 - (TotalDailyTargetSystemStickWrapping.Value5 - (((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM));
                            TotalDailyTargetSystemStickWrapping.Value5 = ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value6 != ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 - (TotalDailyTargetSystemStickWrapping.Value6 - (((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM));
                            TotalDailyTargetSystemStickWrapping.Value6 = ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value7 != ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 - (TotalDailyTargetSystemStickWrapping.Value7 - (((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM));
                            TotalDailyTargetSystemStickWrapping.Value7 = ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM;
                        }
                        // Manual
                        if ((TotalDailyTargetManualStickWrapping.Value1 != ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 - (TotalDailyTargetManualStickWrapping.Value1 - (((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM));
                            TotalDailyTargetManualStickWrapping.Value1 = ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value2 != ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 - (TotalDailyTargetManualStickWrapping.Value2 - (((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM));
                            TotalDailyTargetManualStickWrapping.Value2 = ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value3 != ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 - (TotalDailyTargetManualStickWrapping.Value3 - (((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM));
                            TotalDailyTargetManualStickWrapping.Value3 = ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value4 != ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 - (TotalDailyTargetManualStickWrapping.Value4 - (((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM));
                            TotalDailyTargetManualStickWrapping.Value4 = ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value5 != ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 - (TotalDailyTargetManualStickWrapping.Value5 - (((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM));
                            TotalDailyTargetManualStickWrapping.Value5 = ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value6 != ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 - (TotalDailyTargetManualStickWrapping.Value6 - (((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM));
                            TotalDailyTargetManualStickWrapping.Value6 = ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value7 != ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 - (TotalDailyTargetManualStickWrapping.Value7 - (((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM));
                            TotalDailyTargetManualStickWrapping.Value7 = ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM;
                        }
                        #endregion
                    }
                }
                else if (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Cutting.ToString().ToUpper())
                {
                    int index = 0;
                    foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        TPOTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        TPOTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        TPOTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        TPOTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        TPOTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        TPOTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        TPOTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroup(TPOTPKByGroup, weekDate); // pengecekan inactive group
                        #region Group check
                        if ((!c.groupStatus && weekDate[0] >= c.updateDate) || (currentProcessWorkHours.Value1 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult1 = 0;
                        }
                        if ((!c.groupStatus && weekDate[1] >= c.updateDate) || (currentProcessWorkHours.Value2 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult2 = 0;
                        }
                        if ((!c.groupStatus && weekDate[2] >= c.updateDate) || (currentProcessWorkHours.Value3 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult3 = 0;
                        }
                        if ((!c.groupStatus && weekDate[3] >= c.updateDate) || (currentProcessWorkHours.Value4 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult4 = 0;
                        }
                        if ((!c.groupStatus && weekDate[4] >= c.updateDate) || (currentProcessWorkHours.Value5 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult5 = 0;
                        }
                        if ((!c.groupStatus && weekDate[5] >= c.updateDate) || (currentProcessWorkHours.Value6 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult6 = 0;
                        }
                        if ((!c.groupStatus && weekDate[6] >= c.updateDate) || (currentProcessWorkHours.Value7 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult7 = 0;
                        }
                        #endregion

                        if (groupEmptyAllocation)
                        {
                            if (TPOTPKByGroup.WorkerAlocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target7)
                    };
                    if (!groupEmptyAllocation)
                    {
                        if (previousProcess.ToUpper() == Enums.Process.Wrapping.ToString().ToUpper())
                        {
                            foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                            {
                                #region Assign target
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetSystemStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetManualStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetSystemStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetManualStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetManualStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetManualStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetManualStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetManualStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetManualStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                }
                                                                
                                TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                                TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                                #endregion
                            }
                        }
                        else if (previousProcess.ToUpper() == Enums.Process.Stickwrapping.ToString().ToUpper())
                        {
                            foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                            {
                                #region Assign target
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * (TotalDailyTargetManualStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * (TotalDailyTargetManualStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * (TotalDailyTargetManualStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * (TotalDailyTargetManualStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * (TotalDailyTargetManualStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * (TotalDailyTargetManualStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward)) 
                                {
                                    TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * (TotalDailyTargetManualStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                
                                TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;
                                
                                TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;

                                #endregion
                            }
                        }
                        else if (previousProcess.ToUpper() == Enums.Process.Packing.ToString().ToUpper())
                        {
                            foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                            {
                                #region Assign target
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                
                                }
                                if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                
                                TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                                TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                                
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlanTPOTPK = fillEmptyAllocation(TPOTPKByProcess.PlanTPOTPK);
                    }
                    

                    // Variables for Next Process
                    TotalDailyTargetSystemCutting = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualCutting = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        if (previousProcess.ToUpper() == Enums.Process.Wrapping.ToString().ToUpper())
                        {
                            #region Wrapping
                            // System
                            if ((TotalDailyTargetSystemCutting.Value1 != (((TotalDailyTargetSystemStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM)) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 - (TotalDailyTargetSystemCutting.Value1 - ((TotalDailyTargetSystemStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM);
                                TotalDailyTargetSystemCutting.Value1 = ((TotalDailyTargetSystemStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value2 != (((TotalDailyTargetSystemStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM)) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 - (TotalDailyTargetSystemCutting.Value2 - ((TotalDailyTargetSystemStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM);
                                TotalDailyTargetSystemCutting.Value2 = ((TotalDailyTargetSystemStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value3 != (((TotalDailyTargetSystemStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM)) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 - (TotalDailyTargetSystemCutting.Value3 - ((TotalDailyTargetSystemStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM);
                                TotalDailyTargetSystemCutting.Value3 = ((TotalDailyTargetSystemStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value4 != (((TotalDailyTargetSystemStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM)) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 - (TotalDailyTargetSystemCutting.Value4 - ((TotalDailyTargetSystemStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM);
                                TotalDailyTargetSystemCutting.Value4 = ((TotalDailyTargetSystemStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value5 != (((TotalDailyTargetSystemStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM)) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 - (TotalDailyTargetSystemCutting.Value5 - ((TotalDailyTargetSystemStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM);
                                TotalDailyTargetSystemCutting.Value5 = ((TotalDailyTargetSystemStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value6 != (((TotalDailyTargetSystemStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM)) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 - (TotalDailyTargetSystemCutting.Value6 - ((TotalDailyTargetSystemStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM);
                                TotalDailyTargetSystemCutting.Value6 = ((TotalDailyTargetSystemStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value7 != (((TotalDailyTargetSystemStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM)) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 - (TotalDailyTargetSystemCutting.Value7 - ((TotalDailyTargetSystemStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM);
                                TotalDailyTargetSystemCutting.Value7 = ((TotalDailyTargetSystemStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM;
                            }
                            // Manual
                            if ((TotalDailyTargetManualCutting.Value1 != (((TotalDailyTargetManualStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM)) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 - (TotalDailyTargetManualCutting.Value1 - ((TotalDailyTargetManualStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM);
                                TotalDailyTargetManualCutting.Value1 = ((TotalDailyTargetManualStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value2 != (((TotalDailyTargetManualStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM)) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 - (TotalDailyTargetManualCutting.Value2 - ((TotalDailyTargetManualStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM);
                                TotalDailyTargetManualCutting.Value2 = ((TotalDailyTargetManualStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value3 != (((TotalDailyTargetManualStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM)) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 - (TotalDailyTargetManualCutting.Value3 - ((TotalDailyTargetManualStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM);
                                TotalDailyTargetManualCutting.Value3 = ((TotalDailyTargetManualStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value4 != (((TotalDailyTargetManualStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM)) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 - (TotalDailyTargetManualCutting.Value4 - ((TotalDailyTargetManualStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM);
                                TotalDailyTargetManualCutting.Value4 = ((TotalDailyTargetManualStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value5 != (((TotalDailyTargetManualStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM)) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 - (TotalDailyTargetManualCutting.Value5 - ((TotalDailyTargetManualStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM);
                                TotalDailyTargetManualCutting.Value5 = ((TotalDailyTargetManualStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value6 != (((TotalDailyTargetManualStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM)) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 - (TotalDailyTargetManualCutting.Value6 - ((TotalDailyTargetManualStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM);
                                TotalDailyTargetManualCutting.Value6 = ((TotalDailyTargetManualStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value7 != (((TotalDailyTargetManualStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM)) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 - (TotalDailyTargetManualCutting.Value7 - ((TotalDailyTargetManualStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM);
                                TotalDailyTargetManualCutting.Value7 = ((TotalDailyTargetManualStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM;
                            }
                            #endregion
                        }
                        else if (previousProcess.ToUpper() == Enums.Process.Stickwrapping.ToString().ToUpper())
                        {
                            #region Stickwrapping
                            // System
                            if ((TotalDailyTargetSystemCutting.Value1 != (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM)) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 - (TotalDailyTargetSystemCutting.Value1 - (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM));
                                TotalDailyTargetSystemCutting.Value1 = (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value2 != (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM)) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 - (TotalDailyTargetSystemCutting.Value2 - (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM));
                                TotalDailyTargetSystemCutting.Value2 = (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value3 != (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM)) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 - (TotalDailyTargetSystemCutting.Value3 - (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM));
                                TotalDailyTargetSystemCutting.Value3 = (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value4 != (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM)) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 - (TotalDailyTargetSystemCutting.Value4 - (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM));
                                TotalDailyTargetSystemCutting.Value4 = (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value5 != (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM)) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 - (TotalDailyTargetSystemCutting.Value5 - (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM));
                                TotalDailyTargetSystemCutting.Value5 = (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value6 != (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM)) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 - (TotalDailyTargetSystemCutting.Value6 - (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM));
                                TotalDailyTargetSystemCutting.Value6 = (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value7 != (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM)) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 - (TotalDailyTargetSystemCutting.Value7 - (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM));
                                TotalDailyTargetSystemCutting.Value7 = (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM);
                            }
                            // Manual
                            if ((TotalDailyTargetManualCutting.Value1 != (TotalDailyTargetManualStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM)) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 - (TotalDailyTargetManualCutting.Value1 - (TotalDailyTargetManualStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM));
                                TotalDailyTargetManualCutting.Value1 = (TotalDailyTargetManualStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value2 != (TotalDailyTargetManualStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM)) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 - (TotalDailyTargetManualCutting.Value2 - (TotalDailyTargetManualStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM));
                                TotalDailyTargetManualCutting.Value2 = (TotalDailyTargetManualStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value3 != (TotalDailyTargetManualStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM)) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 - (TotalDailyTargetManualCutting.Value3 - (TotalDailyTargetManualStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM));
                                TotalDailyTargetManualCutting.Value3 = (TotalDailyTargetManualStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value4 != (TotalDailyTargetManualStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM)) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 - (TotalDailyTargetManualCutting.Value4 - (TotalDailyTargetManualStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM));
                                TotalDailyTargetManualCutting.Value4 = (TotalDailyTargetManualStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value5 != (TotalDailyTargetManualStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM)) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 - (TotalDailyTargetManualCutting.Value5 - (TotalDailyTargetManualStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM));
                                TotalDailyTargetManualCutting.Value5 = (TotalDailyTargetManualStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value6 != (TotalDailyTargetManualStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM)) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 - (TotalDailyTargetManualCutting.Value6 - (TotalDailyTargetManualStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM));
                                TotalDailyTargetManualCutting.Value6 = (TotalDailyTargetManualStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value7 != (TotalDailyTargetManualStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM)) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 - (TotalDailyTargetManualCutting.Value7 - (TotalDailyTargetManualStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM));
                                TotalDailyTargetManualCutting.Value7 = (TotalDailyTargetManualStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM);
                            }
                            #endregion
                        }
                        else if (previousProcess.ToUpper() == Enums.Process.Packing.ToString().ToUpper())
                        {
                            #region Packing
                            // System
                            if ((TotalDailyTargetSystemCutting.Value1 != ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 - (TotalDailyTargetSystemCutting.Value1 - (((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM));
                                TotalDailyTargetSystemCutting.Value1 = ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value2 != ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 - (TotalDailyTargetSystemCutting.Value2 - (((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM));
                                TotalDailyTargetSystemCutting.Value2 = ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value3 != ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 - (TotalDailyTargetSystemCutting.Value3 - (((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM));
                                TotalDailyTargetSystemCutting.Value3 = ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value4 != ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 - (TotalDailyTargetSystemCutting.Value4 - (((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM));
                                TotalDailyTargetSystemCutting.Value4 = ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value5 != ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 - (TotalDailyTargetSystemCutting.Value5 - (((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM));
                                TotalDailyTargetSystemCutting.Value5 = ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value6 != ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 - (TotalDailyTargetSystemCutting.Value6 - (((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM));
                                TotalDailyTargetSystemCutting.Value6 = ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value7 != ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 - (TotalDailyTargetSystemCutting.Value7 - (((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM));
                                TotalDailyTargetSystemCutting.Value7 = ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM;
                            }
                            // Manual
                            if ((TotalDailyTargetManualCutting.Value1 != ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 - (TotalDailyTargetManualCutting.Value1 - (((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM));
                                TotalDailyTargetManualCutting.Value1 = ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value2 != ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 - (TotalDailyTargetManualCutting.Value2 - (((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM));
                                TotalDailyTargetManualCutting.Value2 = ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value3 != ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 - (TotalDailyTargetManualCutting.Value3 - (((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM));
                                TotalDailyTargetManualCutting.Value3 = ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value4 != ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 - (TotalDailyTargetManualCutting.Value4 - (((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM));
                                TotalDailyTargetManualCutting.Value4 = ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value5 != ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 - (TotalDailyTargetManualCutting.Value5 - (((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM));
                                TotalDailyTargetManualCutting.Value5 = ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value6 != ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 - (TotalDailyTargetManualCutting.Value6 - (((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM));
                                TotalDailyTargetManualCutting.Value6 = ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value7 != ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 - (TotalDailyTargetManualCutting.Value7 - (((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM));
                                TotalDailyTargetManualCutting.Value7 = ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM;
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
                else if (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Rolling.ToString().ToUpper())
                {
                    int index = 0;
                    foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        TPOTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        TPOTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        TPOTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        TPOTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        TPOTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        TPOTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        TPOTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroup(TPOTPKByGroup, weekDate); // pengecekan inactive group
                        #region Group check
                        if ((!c.groupStatus && weekDate[0] >= c.updateDate) || (currentProcessWorkHours.Value1 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult1 = 0;
                        }
                        if ((!c.groupStatus && weekDate[1] >= c.updateDate) || (currentProcessWorkHours.Value2 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult2 = 0;
                        }
                        if ((!c.groupStatus && weekDate[2] >= c.updateDate) || (currentProcessWorkHours.Value3 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult3 = 0;
                        }
                        if ((!c.groupStatus && weekDate[3] >= c.updateDate) || (currentProcessWorkHours.Value4 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult4 = 0;
                        }
                        if ((!c.groupStatus && weekDate[4] >= c.updateDate) || (currentProcessWorkHours.Value5 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult5 = 0;
                        }
                        if ((!c.groupStatus && weekDate[5] >= c.updateDate) || (currentProcessWorkHours.Value6 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult6 = 0;
                        }
                        if ((!c.groupStatus && weekDate[6] >= c.updateDate) || (currentProcessWorkHours.Value7 == 0))
                        {
                            TPOTPKByGroup.DailyWHWeightedResult7 = 0;
                        }
                        #endregion

                        if (groupEmptyAllocation)
                        {
                            if (TPOTPKByGroup.WorkerAlocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.Target7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        foreach (var TPOTPKByGroup in TPOTPKByProcess.PlanTPOTPK)
                        {

                            #region Assign target
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[0] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * TotalDailyTargetSystemCutting.Value1 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * TotalDailyTargetManualCutting.Value1 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[1] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * TotalDailyTargetSystemCutting.Value2 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * TotalDailyTargetManualCutting.Value2 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[2] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * TotalDailyTargetSystemCutting.Value3 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * TotalDailyTargetManualCutting.Value3 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[3] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * TotalDailyTargetSystemCutting.Value4 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * TotalDailyTargetManualCutting.Value4 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[4] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * TotalDailyTargetSystemCutting.Value5 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * TotalDailyTargetManualCutting.Value5 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[5] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * TotalDailyTargetSystemCutting.Value6 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * TotalDailyTargetManualCutting.Value6 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputTPOTPK.IsFilterCurrentDayForward && weekDate[6] >= InputTPOTPK.FilterCurrentDayForward) || (!InputTPOTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * TotalDailyTargetSystemCutting.Value7 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * TotalDailyTargetManualCutting.Value7 : 0), 0, MidpointRounding.AwayFromZero);
                            }

                            #endregion

                            TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlanTPOTPK = fillEmptyAllocation(TPOTPKByProcess.PlanTPOTPK);
                    }

                    // Variables for Next Process
                    TotalDailyTargetSystemRolling = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualRolling = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlanTPOTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        // System
                        if ((TotalDailyTargetSystemRolling.Value1 != TotalDailyTargetSystemCutting.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem1 + (TotalDailyTargetSystemCutting.Value1 - TotalDailyTargetSystemRolling.Value1);
                            TotalDailyTargetSystemRolling.Value1 = TotalDailyTargetSystemCutting.Value1;
                        }
                        if ((TotalDailyTargetSystemRolling.Value2 != TotalDailyTargetSystemCutting.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem2 + (TotalDailyTargetSystemCutting.Value2 - TotalDailyTargetSystemRolling.Value2);
                            TotalDailyTargetSystemRolling.Value2 = TotalDailyTargetSystemCutting.Value2;
                        }
                        if ((TotalDailyTargetSystemRolling.Value3 != TotalDailyTargetSystemCutting.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem3 + (TotalDailyTargetSystemCutting.Value3 - TotalDailyTargetSystemRolling.Value3);
                            TotalDailyTargetSystemRolling.Value3 = TotalDailyTargetSystemCutting.Value3;
                        }
                        if ((TotalDailyTargetSystemRolling.Value4 != TotalDailyTargetSystemCutting.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem4 + (TotalDailyTargetSystemCutting.Value4 - TotalDailyTargetSystemRolling.Value4);
                            TotalDailyTargetSystemRolling.Value4 = TotalDailyTargetSystemCutting.Value4;
                        }
                        if ((TotalDailyTargetSystemRolling.Value5 != TotalDailyTargetSystemCutting.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem5 + (TotalDailyTargetSystemCutting.Value5 - TotalDailyTargetSystemRolling.Value5);
                            TotalDailyTargetSystemRolling.Value5 = TotalDailyTargetSystemCutting.Value5;
                        }
                        if ((TotalDailyTargetSystemRolling.Value6 != TotalDailyTargetSystemCutting.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem6 + (TotalDailyTargetSystemCutting.Value6 - TotalDailyTargetSystemRolling.Value6);
                            TotalDailyTargetSystemRolling.Value6 = TotalDailyTargetSystemCutting.Value6;
                        }
                        if ((TotalDailyTargetSystemRolling.Value7 != TotalDailyTargetSystemCutting.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetSystem7 + (TotalDailyTargetSystemCutting.Value7 - TotalDailyTargetSystemRolling.Value7);
                            TotalDailyTargetSystemRolling.Value7 = TotalDailyTargetSystemCutting.Value7;
                        }
                        // Manual
                        if ((TotalDailyTargetManualRolling.Value1 != TotalDailyTargetManualCutting.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual1 + (TotalDailyTargetManualCutting.Value1 - TotalDailyTargetManualRolling.Value1);
                            TotalDailyTargetManualRolling.Value1 = TotalDailyTargetManualCutting.Value1;
                        }
                        if ((TotalDailyTargetManualRolling.Value2 != TotalDailyTargetManualCutting.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual2 + (TotalDailyTargetManualCutting.Value2 - TotalDailyTargetManualRolling.Value2);
                            TotalDailyTargetManualRolling.Value2 = TotalDailyTargetManualCutting.Value2;
                        }
                        if ((TotalDailyTargetManualRolling.Value3 != TotalDailyTargetManualCutting.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual3 + (TotalDailyTargetManualCutting.Value3 - TotalDailyTargetManualRolling.Value3);
                            TotalDailyTargetManualRolling.Value3 = TotalDailyTargetManualCutting.Value3;
                        }
                        if ((TotalDailyTargetManualRolling.Value4 != TotalDailyTargetManualCutting.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual4 + (TotalDailyTargetManualCutting.Value4 - TotalDailyTargetManualRolling.Value4);
                            TotalDailyTargetManualRolling.Value4 = TotalDailyTargetManualCutting.Value4;
                        }
                        if ((TotalDailyTargetManualRolling.Value5 != TotalDailyTargetManualCutting.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual5 + (TotalDailyTargetManualCutting.Value5 - TotalDailyTargetManualRolling.Value5);
                            TotalDailyTargetManualRolling.Value5 = TotalDailyTargetManualCutting.Value5;
                        }
                        if ((TotalDailyTargetManualRolling.Value6 != TotalDailyTargetManualCutting.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual6 + (TotalDailyTargetManualCutting.Value6 - TotalDailyTargetManualRolling.Value6);
                            TotalDailyTargetManualRolling.Value6 = TotalDailyTargetManualCutting.Value6;
                        }
                        if ((TotalDailyTargetManualRolling.Value7 != TotalDailyTargetManualCutting.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlanTPOTPK[groupIndex].TargetManual7 + (TotalDailyTargetManualCutting.Value7 - TotalDailyTargetManualRolling.Value7);
                            TotalDailyTargetManualRolling.Value7 = TotalDailyTargetManualCutting.Value7;
                        }
                        #endregion
                    }
                }

                #endregion

                #region Get Previous Process Work Hour By Process
                // FOR WH GET FROM PREVIOUS PROCESS
                //var PreviousProcessWorkHours = TPOTPKByProcess.PlanTPOTPK.FirstOrDefault();
                //if (PreviousProcessWorkHours != null)
                //{
                //    previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
                //    {
                //        Value1 = PreviousProcessWorkHours.ProcessWorkHours1,
                //        Value2 = PreviousProcessWorkHours.ProcessWorkHours2,
                //        Value3 = PreviousProcessWorkHours.ProcessWorkHours3,
                //        Value4 = PreviousProcessWorkHours.ProcessWorkHours4,
                //        Value5 = PreviousProcessWorkHours.ProcessWorkHours5,
                //        Value6 = PreviousProcessWorkHours.ProcessWorkHours6,
                //        Value7 = PreviousProcessWorkHours.ProcessWorkHours7
                //    };
                //}
                #endregion

                // Set PREVIOUS Process State
                previousProcess = TPOTPKByProcess.ProcessGroup;
            }

            #region Total Calculation
            var totalBoxResponse = new PlanTPOTPKTotalBoxDTO
            {
                LocationCode = InputTPOTPK.LocationCode,
                BrandCode = InputTPOTPK.BrandCode,
                KPSYear = InputTPOTPK.KPSYear,
                KPSWeek = InputTPOTPK.KPSWeek,
                TotalType = Enums.Conversion.Box.ToString().ToUpper(),
                //TargetSystem1 = DailyTargetBOXSystem.Value1,
                //TargetSystem2 = DailyTargetBOXSystem.Value2,
                //TargetSystem3 = DailyTargetBOXSystem.Value3,
                //TargetSystem4 = DailyTargetBOXSystem.Value4,
                //TargetSystem5 = DailyTargetBOXSystem.Value5,
                //TargetSystem6 = DailyTargetBOXSystem.Value6,
                //TargetSystem7 = DailyTargetBOXSystem.Value7,
                TargetSystem1 = TotalBox.TargetSystem1,
                TargetSystem2 = TotalBox.TargetSystem2,
                TargetSystem3 = TotalBox.TargetSystem3,
                TargetSystem4 = TotalBox.TargetSystem4,
                TargetSystem5 = TotalBox.TargetSystem5,
                TargetSystem6 = TotalBox.TargetSystem6,
                TargetSystem7 = TotalBox.TargetSystem7,
                TargetManual1 = TotalBox.TargetManual1,
                TargetManual2 = TotalBox.TargetManual2,
                TargetManual3 = TotalBox.TargetManual3,
                TargetManual4 = TotalBox.TargetManual4,
                TargetManual5 = TotalBox.TargetManual5,
                TargetManual6 = TotalBox.TargetManual6,
                TargetManual7 = TotalBox.TargetManual7,
                //TotalTargetSystem = DailyTargetBOXSystem.Value1 + 
                //                    DailyTargetBOXSystem.Value2 +
                //                    DailyTargetBOXSystem.Value3 +
                //                    DailyTargetBOXSystem.Value4 +
                //                    DailyTargetBOXSystem.Value5 +
                //                    DailyTargetBOXSystem.Value6 +
                //                    DailyTargetBOXSystem.Value7,
                TotalTargetSystem = TotalBox.TargetSystem1 +
                                   TotalBox.TargetSystem2 +
                                   TotalBox.TargetSystem3 +
                                   TotalBox.TargetSystem4 +
                                   TotalBox.TargetSystem5 +
                                   TotalBox.TargetSystem6 +
                                   TotalBox.TargetSystem7,
                TotalTargetManual = TotalBox.TargetManual1 +
                                    TotalBox.TargetManual2 +
                                    TotalBox.TargetManual3 +
                                    TotalBox.TargetManual4 +
                                    TotalBox.TargetManual5 +
                                    TotalBox.TargetManual6 +
                                    TotalBox.TargetManual7,
            };

            var brandForStick = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = InputTPOTPK.BrandCode });

            var totalStickResponse = new PlanTPOTPKTotalBoxDTO
                                 {
                                     LocationCode = InputTPOTPK.LocationCode,
                                     BrandCode = InputTPOTPK.BrandCode,
                                     KPSYear = InputTPOTPK.KPSYear,
                                     KPSWeek = InputTPOTPK.KPSWeek,
                                     TotalType = Enums.Conversion.Stick.ToString().ToUpper(),
                                     //TargetSystem1 = DailyTargetBOXSystem.Value1 * brandForStick.StickPerBox,
                                     //TargetSystem2 = DailyTargetBOXSystem.Value2 * brandForStick.StickPerBox,
                                     //TargetSystem3 = DailyTargetBOXSystem.Value3 * brandForStick.StickPerBox,
                                     //TargetSystem4 = DailyTargetBOXSystem.Value4 * brandForStick.StickPerBox,
                                     //TargetSystem5 = DailyTargetBOXSystem.Value5 * brandForStick.StickPerBox,
                                     //TargetSystem6 = DailyTargetBOXSystem.Value6 * brandForStick.StickPerBox,
                                     //TargetSystem7 = DailyTargetBOXSystem.Value7 * brandForStick.StickPerBox,
                                     TargetSystem1 = TotalBox.TargetSystem1 * brandForStick.StickPerBox,
                                     TargetSystem2 = TotalBox.TargetSystem2 * brandForStick.StickPerBox,
                                     TargetSystem3 = TotalBox.TargetSystem3 * brandForStick.StickPerBox,
                                     TargetSystem4 = TotalBox.TargetSystem4 * brandForStick.StickPerBox,
                                     TargetSystem5 = TotalBox.TargetSystem5 * brandForStick.StickPerBox,
                                     TargetSystem6 = TotalBox.TargetSystem6 * brandForStick.StickPerBox,
                                     TargetSystem7 = TotalBox.TargetSystem7 * brandForStick.StickPerBox,
                                     TargetManual1 = TotalBox.TargetManual1 * brandForStick.StickPerBox,
                                     TargetManual2 = TotalBox.TargetManual2 * brandForStick.StickPerBox,
                                     TargetManual3 = TotalBox.TargetManual3 * brandForStick.StickPerBox,
                                     TargetManual4 = TotalBox.TargetManual4 * brandForStick.StickPerBox,
                                     TargetManual5 = TotalBox.TargetManual5 * brandForStick.StickPerBox,
                                     TargetManual6 = TotalBox.TargetManual6 * brandForStick.StickPerBox,
                                     TargetManual7 = TotalBox.TargetManual7 * brandForStick.StickPerBox,
                                     //TotalTargetSystem = (DailyTargetBOXSystem.Value1 * brandForStick.StickPerBox) +
                                     //                    (DailyTargetBOXSystem.Value2 * brandForStick.StickPerBox) +
                                     //                    (DailyTargetBOXSystem.Value3 * brandForStick.StickPerBox) +
                                     //                    (DailyTargetBOXSystem.Value4 * brandForStick.StickPerBox) +
                                     //                    (DailyTargetBOXSystem.Value5 * brandForStick.StickPerBox) +
                                     //                    (DailyTargetBOXSystem.Value6 * brandForStick.StickPerBox) +
                                     //                    (DailyTargetBOXSystem.Value7 * brandForStick.StickPerBox),
                                     TotalTargetSystem = (TotalBox.TargetSystem1 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetSystem2 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetSystem3 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetSystem4 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetSystem5 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetSystem6 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetSystem7 * brandForStick.StickPerBox),
                                     TotalTargetManual = (TotalBox.TargetManual1 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetManual2 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetManual3 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetManual4 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetManual5 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetManual6 * brandForStick.StickPerBox) +
                                                         (TotalBox.TargetManual7 * brandForStick.StickPerBox)
                                 };

            total.Add(totalStickResponse);
            total.Add(totalBoxResponse);
            #endregion

            #endregion

            var result = new TPOTPKCalculateDTO()
                         {
                             TPOTPKByProcess = InputTPOTPK.ListTPOTPK,
                             TPOTPTotals = total
                         };

            return result;
        }
        
        public PlanTPOTPKTotalBoxDTO GetTPOTPKInBox(PlanTPOTPKTotalBoxInput input)
        {
            var dbResult = _planTPOTPKInBoxRepo.GetByID(input.KPSYear, input.KPSWeek, input.LocationCode, input.BrandCode);
            return Mapper.Map<PlanTPOTPKTotalBoxDTO>(dbResult);
        }

        public void SaveTPOTPKWorkHourInBox(PlanTPOTPKTotalBoxInput input, GenericValuePerWeekDTO<float> workHour)
        {
            var dbResult = _planTPOTPKInBoxRepo.GetByID(input.KPSYear, input.KPSWeek, input.LocationCode, input.BrandCode);
            dbResult.ProcessWorkHours1 = workHour.Value1;
            dbResult.ProcessWorkHours2 = workHour.Value2;
            dbResult.ProcessWorkHours3 = workHour.Value3;
            dbResult.ProcessWorkHours4 = workHour.Value4;
            dbResult.ProcessWorkHours5 = workHour.Value5;
            dbResult.ProcessWorkHours6 = workHour.Value6;
            dbResult.ProcessWorkHours7 = workHour.Value7;
            _planTPOTPKInBoxRepo.Update(dbResult);
            _uow.SaveChanges();
        }

        private void UpdateTPOTPK(TPOTPKDTO tpoTPK)
        {
            var dbTPOTPK = _planTPOTPKRepo.GetByID(tpoTPK.TPKTPOStartProductionDate, tpoTPK.KPSYear,
                tpoTPK.KPSWeek, tpoTPK.ProdGroup, tpoTPK.ProcessGroup, tpoTPK.LocationCode, tpoTPK.StatusEmp, tpoTPK.BrandCode);

            if (dbTPOTPK == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            tpoTPK.CreatedBy = dbTPOTPK.CreatedBy;
            tpoTPK.CreatedDate = dbTPOTPK.CreatedDate;

            //set update time
            tpoTPK.UpdatedDate = DateTime.Now;

            Mapper.Map(tpoTPK, dbTPOTPK);
            _planTPOTPKRepo.Update(dbTPOTPK);
        }

        public GenericValuePerWeekDTO<float?> CalculateDailyWHWeighted(GenericValuePerWeekDTO<float?> currentWH, GenericValuePerWeekDTO<float?> previousWH)
        {
            var result = new GenericValuePerWeekDTO<float?>();
            if (previousWH.Value1 == null)
                if (currentWH.Value1 == 0)
                    result.Value1 = 0;
                else
                    result.Value1 = 1; // since the formula will be like : TPOTPKByGroup.ProcessWorkHours1 / TPOTPKByGroup.ProcessWorkHours1
            else
                if (previousWH.Value1 == 0)
                    result.Value1 = 0;
                else
                    result.Value1 = currentWH.Value1 / previousWH.Value1;
            if (previousWH.Value2 == null)
                if (currentWH.Value2 == 0)
                    result.Value2 = 0;
                else
                    result.Value2 = 1;
            else
                if (previousWH.Value2 == 0)
                    result.Value2 = 0;
                else
                    result.Value2 = currentWH.Value2 / previousWH.Value2;
            if (previousWH.Value3 == null)
                if (currentWH.Value3 == 0)
                    result.Value3 = 0;
                else
                    result.Value3 = 1;
            else
                if (previousWH.Value3 == 0)
                    result.Value3 = 0;
                else
                    result.Value3 = currentWH.Value3 / previousWH.Value3;
            if (previousWH.Value4 == null)
                if (currentWH.Value4 == 0)
                    result.Value4 = 0;
                else
                    result.Value4 = 1;
            else
                if (previousWH.Value4 == 0)
                    result.Value4 = 0;
                else
                    result.Value4 = currentWH.Value4 / previousWH.Value4;
            if (previousWH.Value5 == null)
                if (currentWH.Value5 == 0)
                    result.Value5 = 0;
                else
                    result.Value5 = 1;
            else
                if (previousWH.Value5 == 0)
                    result.Value5 = 0;
                else
                    result.Value5 = currentWH.Value5 / previousWH.Value5;
            if (previousWH.Value6 == null)
                if (currentWH.Value6 == 0)
                    result.Value6 = 0;
                else
                    result.Value6 = 1;
            else
                if (previousWH.Value6 == 0)
                    result.Value6 = 0;
                else
                    result.Value6 = currentWH.Value6 / previousWH.Value6;
            if (previousWH.Value7 == null)
                if (currentWH.Value7 == 0)
                    result.Value7 = 0;
                else
                    result.Value7 = 1;
            else
                if (previousWH.Value7 == 0)
                    result.Value7 = 0;
                else
                    result.Value7 = currentWH.Value7 / previousWH.Value7;
            return result;
        }

        public void SubmitTpoTpk(string locationCode, string brandCode, int? kpsYear, int? kpsWeek, string userName)
        {
            _sqlSPRepo.SubmitTpoTpk(locationCode, brandCode, kpsYear, kpsWeek, userName);
        }

        #region Calculate TPOTPK from Master
        public void CalculateTPOTPKByTPKCode(string TPKCode)
        {
            var input = new GetTPOTPKInput();
            input.TPKCode = TPKCode;
            var buaya = GetPlanningTPOTPK(input).OrderBy(o => o.ProdGroup);

            var xc = Mapper.Map<TPOTPKByProcessDTO>(buaya);
            foreach (var item in buaya)
            {
                var anak_ucing = new CalculateTPOTPKInput();
                anak_ucing.LocationCode = item.LocationCode;
                anak_ucing.BrandCode = item.BrandCode;
            }
            
        }
        #endregion

        #region EMAILs
        public void SendEmailSubmitTPOTPK(string locationCode, string brandCode, int kpsYear, int kpsWeek, string currUserName)
        {
            // Get First Data TPOTPK
            var tpoTPK = _planTPOTPKRepo.Get(c => c.LocationCode == locationCode && c.BrandCode == brandCode && c.KPSYear == kpsYear && c.KPSWeek == kpsWeek).FirstOrDefault();
            // Get first date of week
            var availDate = _masterDataBll.GetFirstDateByYearWeek(kpsYear, kpsWeek).Date; 
            // Set Date where ProcessWorkHour1-7 > 0 to used on hyperlink
            if ((int)tpoTPK.ProcessWorkHours1 > 0)
            {
                availDate = availDate.AddDays(0);
            }
            else if ((int)tpoTPK.ProcessWorkHours2 > 0){
                availDate = availDate.AddDays(1);
            }
            else if ((int)tpoTPK.ProcessWorkHours3 > 0)
            {
                availDate = availDate.AddDays(2);
            }
            else if ((int)tpoTPK.ProcessWorkHours4 > 0)
            {
                availDate = availDate.AddDays(3);
            }
            else if ((int)tpoTPK.ProcessWorkHours5 > 0)
            {
                availDate = availDate.AddDays(4);
            }
            else if ((int)tpoTPK.ProcessWorkHours6 > 0)
            {
                availDate = availDate.AddDays(5);
            }
            else if ((int)tpoTPK.ProcessWorkHours7 > 0)
            {
                availDate = availDate.AddDays(6);
            }

            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput { 
                LocationCode = locationCode,
                BrandCode = brandCode,
                KpsWeek = kpsWeek,
                KpsYear = kpsYear,
                Process = tpoTPK == null ? Enums.Process.Rolling.ToString() : tpoTPK.ProcessGroup,
                StatusEmp = tpoTPK == null ? Enums.StatusEmp.Resmi.ToString() : tpoTPK.StatusEmp,
                //Date = tpoTPK == null ? _masterDataBll.GetFirstDateByYearWeek(kpsYear, kpsWeek).Date : tpoTPK.TPKTPOStartProductionDate.Date,
                Date = tpoTPK == null ? _masterDataBll.GetFirstDateByYearWeek(kpsYear, kpsWeek).Date : availDate,
                FunctionName = Enums.PageName.TPOTargetProductionGroup.ToString(),
                ButtonName = Enums.ButtonName.Submit.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.PlanTPOTPK),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.TPOProductionEntry),
            };

            // Get User, Email, Responsibility Destination Recipient
            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            // Get User and Email Current/Sender
            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            // Create Email Input
            var listEmail = new List<MailInput>();
            foreach (var item in listUserAndEmailDestination)
            {
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                var email = new MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailTPOTPK(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }
        private string CreateBodyMailTPOTPK(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();
            
            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Entry (Eblek) sudah tersedia, Silakan melanjutkan proses berikutnya: " + Environment.NewLine + Environment.NewLine);
            //bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/ExeTPOProductionEntry/Index/"
            //                                                       + emailInput.LocationCode + "/"
            //                                                       + emailInput.Process + "/"
            //                                                       + emailInput.StatusEmp + "/"
            //                                                       + emailInput.BrandCode + "/"
            //                                                       + emailInput.KpsYear + "/"
            //                                                       + emailInput.KpsWeek + "/"
            //                                                       + emailInput.Date.ToString("yyyy-MM-dd") + "/"
            //                                                       + emailInput.IDResponsibility.ToString()
            //                                                       + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("<p><a href= webrooturl/ExeTPOProductionEntry/Index/" + emailInput.LocationCode + "/"
                                                                   + emailInput.Process + "/"
                                                                   + emailInput.StatusEmp + "/"
                                                                   + emailInput.BrandCode + "/"
                                                                   + emailInput.KpsYear + "/"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + emailInput.Date.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString()+ ">" 
                                                                   + emailInput.FunctionNameDestination +"</a></p>"
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine );

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.IsBodyHtml = true;
            message.Body = bodyMail.ToString();

            return bodyMail.ToString();
        }
        #endregion
    }
}
