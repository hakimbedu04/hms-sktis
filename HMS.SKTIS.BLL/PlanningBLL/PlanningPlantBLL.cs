using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;

namespace HMS.SKTIS.BLL
{
    public partial class PlanningBLL
    {
        #region TPU
        public List<PlanTPUCompositeDTO> GetPlanTPUs(GetPlanTPUsInput input)
        {
            return input.Conversion.ToLower() == Enums.Conversion.Box.ToString().ToLower() ? GetPlantTpUsPerStick(input) : GetPlantTpUsPerBox(input);
        }

        public List<PlanTPUCompositeDTO> GetPlantTpUsPerStick(GetPlanTPUsInput input)
        {
            var queryFilter = PredicateHelper.True<TargetProductionUnitView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);

            if (input.Shift.HasValue)
                queryFilter = queryFilter.And(m => m.Shift == input.Shift);

            if (input.KPSYear.HasValue)
                queryFilter = queryFilter.And(m => m.KPSYear == input.KPSYear);

            if (input.KPSWeek.HasValue)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.KPSWeek);

            var dbResult = _targetProductionUnitViewRepo.Get(queryFilter).OrderBy(m => m.UnitCode);

            return Mapper.Map<List<PlanTPUCompositeDTO>>(dbResult);
        }

        public List<PlanTPUCompositeDTO> GetPlantTpUsPerBox(GetPlanTPUsInput input)
        {
            var queryFilter = PredicateHelper.True<TargetProductionUnitPerBoxView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);

            if (input.Shift.HasValue)
                queryFilter = queryFilter.And(m => m.Shift == input.Shift);

            if (input.KPSYear.HasValue)
                queryFilter = queryFilter.And(m => m.KPSYear == input.KPSYear);

            if (input.KPSWeek.HasValue)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.KPSWeek);

            var dbResult = _targetProductionUnitPerBoxViewRepo.Get(queryFilter).OrderBy(m => m.UnitCode);

            return Mapper.Map<List<PlanTPUCompositeDTO>>(dbResult);
        }

        public PlanTPUDTO UpdatePlanTPU(PlanTPUDTO planTPU)
        {
            var dbPlanTPU = _planTPURepo.GetByID(planTPU.ProductionStartDate, planTPU.KPSYear,
                planTPU.KPSWeek, planTPU.BrandCode, planTPU.LocationCode, planTPU.UnitCode, planTPU.Shift);

            if (dbPlanTPU == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);


            // Convert if Stick to Box, and leave if box as is
            if (planTPU.Conversion.ToLower() == Enums.Conversion.Stick.ToString().ToLower())
            {
                var brand = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = planTPU.BrandCode });
                planTPU.TargetSystem1 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetSystem1) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetSystem2 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetSystem2) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetSystem3 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetSystem3) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetSystem4 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetSystem4) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetSystem5 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetSystem5) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetSystem6 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetSystem6) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetSystem7 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetSystem7) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TotalTargetSystem = planTPU.TargetSystem1 + planTPU.TargetSystem2 + planTPU.TargetSystem3 + planTPU.TargetSystem4 + planTPU.TargetSystem5 + planTPU.TargetSystem6 + planTPU.TargetSystem7;

                planTPU.TargetManual1 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetManual1) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetManual2 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetManual2) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetManual3 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetManual3) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetManual4 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetManual4) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetManual5 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetManual5) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetManual6 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetManual6) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TargetManual7 = Convert.ToInt64(Math.Round((decimal)(Convert.ToDecimal(planTPU.TargetManual7) / brand.StickPerBox), 0, MidpointRounding.AwayFromZero));
                planTPU.TotalTargetManual = planTPU.TargetManual1 + planTPU.TargetManual2 + planTPU.TargetManual3 + planTPU.TargetManual4 + planTPU.TargetManual5 + planTPU.TargetManual6 + planTPU.TargetManual7;

            }

            //keep original CreatedBy and CreatedDate
            planTPU.CreatedBy = dbPlanTPU.CreatedBy;
            planTPU.CreatedDate = dbPlanTPU.CreatedDate;

            //set update time
            planTPU.UpdatedDate = DateTime.Now;

            Mapper.Map(planTPU, dbPlanTPU);
            _planTPURepo.Update(dbPlanTPU);
            _uow.SaveChanges();

            return Mapper.Map<PlanTPUDTO>(dbPlanTPU);
        }

        public TargetManualTPUDTO GetTargetManualTPU(GetTargetManualTPUInput input)
        {
            //float result = 0;
            PlanTargetProductionUnit dbPlanTPU = null;
            var dbResults = _planTPURepo.Get(x => x.LocationCode == input.LocationCode && x.UnitCode == input.UnitCode
                                                  && x.BrandCode == input.BrandCode && x.KPSYear == input.KPSYear &&
                                                  x.KPSWeek == input.KPSWeek);

            try
            {//Makes sure this list only contains one element
                if (input.Shift.HasValue && input.Shift.Value != 0)
                    dbPlanTPU = dbResults.Where(c => c.Shift == input.Shift.Value).FirstOrDefault();
                else
                    dbPlanTPU = dbResults.SingleOrDefault();
            }//SingleOrDefault => If this exception is thrown, means there is a duplicate data !
            catch (InvalidOperationException ex)
            {
                //_logger.Error(ex.Message);
            }

            return dbPlanTPU != null ? Mapper.Map<TargetManualTPUDTO>(dbPlanTPU) : new TargetManualTPUDTO();
        }

        public List<PlanTPUDTO> CalculatePlantTPU(CalculatePlantTPUInput InputPlantTPU, float TargetWpp, bool SubmitStatus)
        {
            var wpp = _planWeeklyProductionPlaningRepo.GetByID(InputPlantTPU.KPSYear, InputPlantTPU.KPSWeek, InputPlantTPU.BrandCode, InputPlantTPU.LocationCode);
            var targetWPP = (decimal)wpp.Value1 * Constants.WPPConvert;
            var tempTargetWpp = targetWPP;
            var totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(t => t.Target1 + t.Target2 + t.Target3 + t.Target4 + t.Target5 + t.Target6 + t.Target7);
            var totalAllUnitDailyTargetBOX = InputPlantTPU.ListPlantTPU.Sum(t => t.TargetSystem1 + t.TargetSystem2 + t.TargetSystem3 + t.TargetSystem4 + t.TargetSystem5 + t.TargetSystem6 + t.TargetSystem7);
            totalUnitWeighted = (decimal?)Math.Round(Convert.ToDouble(totalUnitWeighted));
            totalAllUnitDailyTargetBOX = (float?)Math.Round(Convert.ToDouble(totalUnitWeighted));

            // get start date by input week
            var listDateInWeek = _masterDataBll.GetDateByWeek(InputPlantTPU.KPSYear, InputPlantTPU.KPSWeek);
            var startDate = listDateInWeek.Any() ? listDateInWeek.FirstOrDefault() : DateTime.Now;

            //var LockDateBefore = InputPlantTPU.IsFilterCurrentDayForward ? new DateTime(Math.Max(DateTime.Now.AddDays(1).Ticks, InputPlantTPU.FilterCurrentDayForward.Ticks)) : DateTime.Now;
            var LockDateBefore = InputPlantTPU.IsFilterCurrentDayForward ? InputPlantTPU.FilterCurrentDayForward : startDate;
            var ProductionStartDate = InputPlantTPU.ListPlantTPU.FirstOrDefault().ProductionStartDate;

            var lowestUnit = InputPlantTPU.ListPlantTPU.Where(p => p.WorkerAlocation >= 0).Min(u => u.UnitCode);
            var unitCodeNonZero = InputPlantTPU.ListPlantTPU.Where(p => p.WorkerAlocation > 0).Min(u => u.UnitCode);

            var bCode1 = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = InputPlantTPU.ListPlantTPU.FirstOrDefault().BrandCode });

            var TotalDailyTargetManual = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = InputPlantTPU.ListPlantTPU.Sum(t => t.TargetManual1) * bCode1.StickPerBox.Value,
                        Value2 = InputPlantTPU.ListPlantTPU.Sum(t => t.TargetManual2) * bCode1.StickPerBox.Value,
                        Value3 = InputPlantTPU.ListPlantTPU.Sum(t => t.TargetManual3) * bCode1.StickPerBox.Value,
                        Value4 = InputPlantTPU.ListPlantTPU.Sum(t => t.TargetManual4) * bCode1.StickPerBox.Value,
                        Value5 = InputPlantTPU.ListPlantTPU.Sum(t => t.TargetManual5) * bCode1.StickPerBox.Value,
                        Value6 = InputPlantTPU.ListPlantTPU.Sum(t => t.TargetManual6) * bCode1.StickPerBox.Value,
                        Value7 = InputPlantTPU.ListPlantTPU.Sum(t => t.TargetManual7) * bCode1.StickPerBox.Value
                    };

            if (InputPlantTPU.IsFilterCurrentDayForward)
            {
                var dayOfWeek = (int)InputPlantTPU.FilterCurrentDayForward.DayOfWeek;
                if (dayOfWeek == 0) dayOfWeek = 7;
                switch (dayOfWeek)
                {
                    case 1:
                        tempTargetWpp = targetWPP;
                        totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(t => t.Target1 + t.Target2 + t.Target3 + t.Target4 + t.Target5 + t.Target6 + t.Target7);
                        break;
                    case 2:
                        tempTargetWpp -= (decimal)(TotalDailyTargetManual.Value1);
                        totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(t => t.Target2 + t.Target3 + t.Target4 + t.Target5 + t.Target6 + t.Target7);
                        break;
                    case 3:
                        tempTargetWpp -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2);
                        totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(t => t.Target3 + t.Target4 + t.Target5 + t.Target6 + t.Target7);
                        break;
                    case 4:
                        tempTargetWpp -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3);
                        totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(t => t.Target4 + t.Target5 + t.Target6 + t.Target7);
                        break;
                    case 5:
                        tempTargetWpp -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3 + TotalDailyTargetManual.Value4);
                        totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(t => t.Target5 + t.Target6 + t.Target7);
                        break;
                    case 6:
                        tempTargetWpp -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3 + TotalDailyTargetManual.Value4 + TotalDailyTargetManual.Value5);
                        totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(t => t.Target6 + t.Target7);
                        break;
                    case 7:
                        tempTargetWpp -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3 + TotalDailyTargetManual.Value4 + TotalDailyTargetManual.Value5 + TotalDailyTargetManual.Value6);
                        totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(t => t.Target7);
                        break;
                }


            }

            foreach (var PlantTPU in InputPlantTPU.ListPlantTPU)
            {
                if (PlantTPU.UnitCode == lowestUnit)
                {
                    #region Formula for Lowest Unit with Worker Allocation > 0

                    #region Monday
                    if (ProductionStartDate.Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        if (PlantTPU.ProcessWorkHours1 == 0)
                            PlantTPU.TargetSystem1 = 0;
                        else
                            PlantTPU.TargetSystem1 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(targetWPP * PlantTPU.Target1 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                        //PlantTPU.TargetSystem1 = (float)targetWPP - (totalAllUnitDailyTargetBOX - PlantTPU.TargetSystem1);
                    }

                    #endregion

                    #region Tuesday
                    if (ProductionStartDate.AddDays(1).Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        //targetWPP -= (decimal)PlantTPU.TargetManual1;
                        if (PlantTPU.ProcessWorkHours2 == 0)
                        {
                            PlantTPU.TargetSystem2 = 0;
                        }
                        else if (PlantTPU.ProcessWorkHours1 == 0 && PlantTPU.ProcessWorkHours2 != 0)
                        {
                            PlantTPU.TargetSystem2 = (float?)Math.Round(Convert.ToDouble((float)tempTargetWpp - (totalAllUnitDailyTargetBOX - PlantTPU.TargetSystem2)), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            PlantTPU.TargetSystem2 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target2 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                        }
                    }
                    #endregion

                    #region Wednesday
                    if (ProductionStartDate.AddDays(2).Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        //targetWPP -= (decimal)(PlantTPU.TargetManual1 + PlantTPU.TargetManual2);
                        if (PlantTPU.ProcessWorkHours3 == 0)
                        {
                            PlantTPU.TargetSystem3 = 0;
                        }
                        else if (PlantTPU.ProcessWorkHours1 == 0 && PlantTPU.ProcessWorkHours2 == 0 && PlantTPU.ProcessWorkHours3 != 0)
                        {
                            PlantTPU.TargetSystem3 = (float?)Math.Round(Convert.ToDouble((float)tempTargetWpp - (totalAllUnitDailyTargetBOX - PlantTPU.TargetSystem3)), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            PlantTPU.TargetSystem3 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target3 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                        }
                    }
                    #endregion

                    #region Thursday
                    if (ProductionStartDate.AddDays(3).Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        //targetWPP -= (decimal)(PlantTPU.TargetManual1 + PlantTPU.TargetManual2 + PlantTPU.TargetManual3);
                        if (PlantTPU.ProcessWorkHours4 == 0)
                        {
                            PlantTPU.TargetSystem4 = 0;
                        }
                        else if (PlantTPU.ProcessWorkHours1 == 0 && PlantTPU.ProcessWorkHours2 == 0 && PlantTPU.ProcessWorkHours3 == 0 && PlantTPU.ProcessWorkHours4 != 0)
                        {
                            PlantTPU.TargetSystem4 = (float?)Math.Round(Convert.ToDouble((float)tempTargetWpp - (totalAllUnitDailyTargetBOX - PlantTPU.TargetSystem4)), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            PlantTPU.TargetSystem4 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target4 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                        }
                    }
                    #endregion

                    #region Friday
                    if (ProductionStartDate.AddDays(4).Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        //targetWPP -= (decimal)(PlantTPU.TargetManual1 + PlantTPU.TargetManual2 + PlantTPU.TargetManual3 + PlantTPU.TargetManual4);
                        if (PlantTPU.ProcessWorkHours5 == 0)
                        {
                            PlantTPU.TargetSystem5 = 0;
                        }
                        else if (PlantTPU.ProcessWorkHours1 == 0 && PlantTPU.ProcessWorkHours2 == 0 && PlantTPU.ProcessWorkHours3 == 0 && PlantTPU.ProcessWorkHours4 == 0 && PlantTPU.ProcessWorkHours5 != 0)
                        {
                            PlantTPU.TargetSystem5 = (float?)Math.Round(Convert.ToDouble((float)tempTargetWpp - (totalAllUnitDailyTargetBOX - PlantTPU.TargetSystem5)), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            PlantTPU.TargetSystem5 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target5 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                        }
                    }
                    #endregion

                    #region Saturday
                    if (ProductionStartDate.AddDays(5).Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        //var tempTargetWpp = targetWPP;
                        //tempTargetWpp -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3 + TotalDailyTargetManual.Value4 + TotalDailyTargetManual.Value5);
                        if (PlantTPU.ProcessWorkHours6 == 0)
                        {
                            PlantTPU.TargetSystem6 = 0;
                        }
                        else if (PlantTPU.ProcessWorkHours1 == 0 && PlantTPU.ProcessWorkHours2 == 0 && PlantTPU.ProcessWorkHours3 == 0 && PlantTPU.ProcessWorkHours4 == 0 && PlantTPU.ProcessWorkHours5 == 0 && PlantTPU.ProcessWorkHours6 != 0)
                        {
                            PlantTPU.TargetSystem6 = (float?)Math.Round(Convert.ToDouble((float)tempTargetWpp - (totalAllUnitDailyTargetBOX - PlantTPU.TargetSystem6)), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            PlantTPU.TargetSystem6 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target6 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                        }
                    }
                    #endregion

                    #region Sunday
                    if (ProductionStartDate.AddDays(6).Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        //var a = targetWPP;
                        //totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(c => c.Target6 + c.Target7);
                        //a -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3 + TotalDailyTargetManual.Value4 + TotalDailyTargetManual.Value5);
                        if (PlantTPU.ProcessWorkHours7 == 0)
                        {
                            PlantTPU.TargetSystem7 = 0;
                        }
                        else if (PlantTPU.ProcessWorkHours1 == 0 && PlantTPU.ProcessWorkHours2 == 0 && PlantTPU.ProcessWorkHours3 == 0 && PlantTPU.ProcessWorkHours4 == 0 && PlantTPU.ProcessWorkHours5 == 0 && PlantTPU.ProcessWorkHours6 == 0 && PlantTPU.ProcessWorkHours7 != 0)
                        {
                            PlantTPU.TargetSystem7 = (float?)Math.Round(Convert.ToDouble((float)tempTargetWpp - (totalAllUnitDailyTargetBOX - PlantTPU.TargetSystem7)), 0, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            PlantTPU.TargetSystem7 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target7 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                        }
                    }
                    #endregion

                    #endregion
                }
                else
                {
                    #region Formula for Greatest Unit from above condition
                    if (ProductionStartDate.Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem1 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target1 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                    if (ProductionStartDate.AddDays(1).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem2 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target2 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                    if (ProductionStartDate.AddDays(2).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem3 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target3 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                    if (ProductionStartDate.AddDays(3).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem4 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target4 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                    if (ProductionStartDate.AddDays(4).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem5 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target5 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                    if (ProductionStartDate.AddDays(5).Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        //var a = targetWPP;
                        //totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(c => c.Target6 + c.Target7);
                        //a -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3 + TotalDailyTargetManual.Value4 + TotalDailyTargetManual.Value5);
                        PlantTPU.TargetSystem6 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target6 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                    }

                    if (ProductionStartDate.AddDays(6).Date >= LockDateBefore.Date || SubmitStatus)
                    {
                        //var a = targetWPP;
                        //totalUnitWeighted = InputPlantTPU.ListPlantTPU.Sum(c => c.Target6 + c.Target7);
                        //a -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3 + TotalDailyTargetManual.Value4 + TotalDailyTargetManual.Value5);
                        PlantTPU.TargetSystem7 = totalUnitWeighted != 0 ? (float)Math.Round(Convert.ToDouble(tempTargetWpp * PlantTPU.Target7 / totalUnitWeighted), 0, MidpointRounding.AwayFromZero) : 0;
                    }

                    #endregion
                }

                var brand = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = PlantTPU.BrandCode });

                if (ProductionStartDate.Date >= LockDateBefore.Date || SubmitStatus)
                    PlantTPU.TargetSystem1 = (float?)Math.Round((double)(PlantTPU.TargetSystem1 / brand.StickPerBox));
                if (ProductionStartDate.AddDays(1).Date >= LockDateBefore.Date || SubmitStatus)
                    PlantTPU.TargetSystem2 = (float?)Math.Round((double)(PlantTPU.TargetSystem2 / brand.StickPerBox));
                if (ProductionStartDate.AddDays(2).Date >= LockDateBefore.Date || SubmitStatus)
                    PlantTPU.TargetSystem3 = (float?)Math.Round((double)(PlantTPU.TargetSystem3 / brand.StickPerBox));
                if (ProductionStartDate.AddDays(3).Date >= LockDateBefore.Date || SubmitStatus)
                    PlantTPU.TargetSystem4 = (float?)Math.Round((double)(PlantTPU.TargetSystem4 / brand.StickPerBox));
                if (ProductionStartDate.AddDays(4).Date >= LockDateBefore.Date || SubmitStatus)
                    PlantTPU.TargetSystem5 = (float?)Math.Round((double)(PlantTPU.TargetSystem5 / brand.StickPerBox));
                if (ProductionStartDate.AddDays(5).Date >= LockDateBefore.Date || SubmitStatus)
                    PlantTPU.TargetSystem6 = (float?)Math.Round((double)(PlantTPU.TargetSystem6 / brand.StickPerBox));
                if (ProductionStartDate.AddDays(6).Date >= LockDateBefore.Date || SubmitStatus)
                    PlantTPU.TargetSystem7 = (float?)Math.Round((double)(PlantTPU.TargetSystem7 / brand.StickPerBox));


                // Stick
                if (InputPlantTPU.Conversion.ToLower() == Enums.Conversion.Stick.ToString().ToLower())
                {
                    if (ProductionStartDate.Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem1 = (float?)Math.Round((double)(PlantTPU.TargetSystem1 * brand.StickPerBox));
                    if (ProductionStartDate.AddDays(1).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem2 = (float?)Math.Round((double)(PlantTPU.TargetSystem2 * brand.StickPerBox));
                    if (ProductionStartDate.AddDays(2).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem3 = (float?)Math.Round((double)(PlantTPU.TargetSystem3 * brand.StickPerBox));
                    if (ProductionStartDate.AddDays(3).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem4 = (float?)Math.Round((double)(PlantTPU.TargetSystem4 * brand.StickPerBox));
                    if (ProductionStartDate.AddDays(4).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem5 = (float?)Math.Round((double)(PlantTPU.TargetSystem5 * brand.StickPerBox));
                    if (ProductionStartDate.AddDays(5).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem6 = (float?)Math.Round((double)(PlantTPU.TargetSystem6 * brand.StickPerBox));
                    if (ProductionStartDate.AddDays(6).Date >= LockDateBefore.Date || SubmitStatus)
                        PlantTPU.TargetSystem7 = (float?)Math.Round((double)(PlantTPU.TargetSystem7 * brand.StickPerBox));
                }

                PlantTPU.TargetManual1 = PlantTPU.TargetSystem1;
                PlantTPU.TargetManual2 = PlantTPU.TargetSystem2;
                PlantTPU.TargetManual3 = PlantTPU.TargetSystem3;
                PlantTPU.TargetManual4 = PlantTPU.TargetSystem4;
                PlantTPU.TargetManual5 = PlantTPU.TargetSystem5;
                PlantTPU.TargetManual6 = PlantTPU.TargetSystem6;
                PlantTPU.TargetManual7 = PlantTPU.TargetSystem7;

                PlantTPU.TotalTargetSystem = PlantTPU.TargetSystem1 +
                                              PlantTPU.TargetSystem2 + PlantTPU.TargetSystem3 +
                                              PlantTPU.TargetSystem4 + PlantTPU.TargetSystem5 +
                                              PlantTPU.TargetSystem6 + PlantTPU.TargetSystem7;
                PlantTPU.TotalTargetManual = PlantTPU.TotalTargetSystem;

                PlantTPU.CreatedDate = DateTime.Now;
                PlantTPU.UpdatedDate = DateTime.Now;
            }

            var bCode = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = InputPlantTPU.ListPlantTPU.FirstOrDefault().BrandCode });
            var tempTotalTargetSystem = InputPlantTPU.ListPlantTPU.Sum(t => t.TotalTargetSystem);
            if ((TargetWpp / bCode.StickPerBox) != tempTotalTargetSystem)
            {
                //var a = targetWPP;
                //a -= (decimal)(TotalDailyTargetManual.Value1 + TotalDailyTargetManual.Value2 + TotalDailyTargetManual.Value3 + TotalDailyTargetManual.Value4 + TotalDailyTargetManual.Value5);
                
                var tempTargetWPP = InputPlantTPU.Conversion.ToLower() == Enums.Conversion.Stick.ToString().ToLower() ?  
                                               (float)TargetWpp :
                                               (float)Math.Round((Decimal)(targetWPP / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);

                var rest = (float)Math.Round((Decimal)(tempTargetWPP - tempTotalTargetSystem), 0, MidpointRounding.AwayFromZero);

                //var rest = InputPlantTPU.Conversion.ToLower() == Enums.Conversion.Stick.ToString().ToLower() ?
                //                   (float)Math.Round((Decimal)((float)TargetWpp - (tempTotalTargetSystem)), 0, MidpointRounding.AwayFromZero) :
                //                   (float)Math.Round((Decimal)((float)(targetWPP / bCode.StickPerBox) - (tempTotalTargetSystem)), 0, MidpointRounding.AwayFromZero);

                var checkGroupCodeAfter = false;

                foreach (var plantTPU in InputPlantTPU.ListPlantTPU)
                {
                    if (plantTPU.UnitCode == unitCodeNonZero)
                    {
                        var tempSum = plantTPU.TargetSystem1 +
                                                  plantTPU.TargetSystem2 + plantTPU.TargetSystem3 +
                                                  plantTPU.TargetSystem4 + plantTPU.TargetSystem5 +
                                                  plantTPU.TargetSystem6 + plantTPU.TargetSystem7;

                        var plantTPUNonZeroAllocation = InputPlantTPU.ListPlantTPU.Where(p => p.UnitCode == unitCodeNonZero).FirstOrDefault();

                        if (plantTPUNonZeroAllocation.ProcessWorkHours7 != 0)
                        {
                            var tempTargetSystem7 = plantTPUNonZeroAllocation.TargetSystem7;
                            plantTPUNonZeroAllocation.TargetSystem7 += rest;
                            if (plantTPUNonZeroAllocation.TargetSystem7 < 0) {
                                plantTPUNonZeroAllocation.TargetSystem7 = 0;
                                rest = rest + tempTargetSystem7.Value;
                                checkGroupCodeAfter = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours6 != 0)
                        {
                            var tempTargetSystem6 = plantTPUNonZeroAllocation.TargetSystem6;
                            plantTPUNonZeroAllocation.TargetSystem6 += rest;
                            if (plantTPUNonZeroAllocation.TargetSystem6 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem6 = 0;
                                rest = rest + tempTargetSystem6.Value;
                                checkGroupCodeAfter = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours5 != 0)
                        {
                            var tempTargetSystem5 = plantTPUNonZeroAllocation.TargetSystem5;
                            plantTPUNonZeroAllocation.TargetSystem5 += rest;
                            if (plantTPUNonZeroAllocation.TargetSystem5 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem5 = 0;
                                rest = rest + tempTargetSystem5.Value;
                                checkGroupCodeAfter = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours4 != 0)
                        {
                            var tempTargetSystem4 = plantTPUNonZeroAllocation.TargetSystem4;
                            plantTPUNonZeroAllocation.TargetSystem4 += rest;
                            if (plantTPUNonZeroAllocation.TargetSystem4 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem4 = 0;
                                rest = rest + tempTargetSystem4.Value;
                                checkGroupCodeAfter = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours3 != 0)
                        {
                            var tempTargetSystem3 = plantTPUNonZeroAllocation.TargetSystem3;
                            plantTPUNonZeroAllocation.TargetSystem3 += rest;
                            if (plantTPUNonZeroAllocation.TargetSystem3 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem3 = 0;
                                rest = rest + tempTargetSystem3.Value;
                                checkGroupCodeAfter = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours2 != 0)
                        {
                            var tempTargetSystem2 = plantTPUNonZeroAllocation.TargetSystem2;
                            plantTPUNonZeroAllocation.TargetSystem2 += rest;
                            if (plantTPUNonZeroAllocation.TargetSystem2 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem2 = 0;
                                rest = rest + tempTargetSystem2.Value;
                                checkGroupCodeAfter = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours1 != 0)
                        {
                            var tempTargetSystem1 = plantTPUNonZeroAllocation.TargetSystem1;
                            plantTPUNonZeroAllocation.TargetSystem1 += rest;
                            if (plantTPUNonZeroAllocation.TargetSystem1 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem1 = 0;
                                rest = rest + tempTargetSystem1.Value;
                                checkGroupCodeAfter = true;
                            }
                        }

                        plantTPU.TargetManual1 = plantTPU.TargetSystem1;
                        plantTPU.TargetManual2 = plantTPU.TargetSystem2;
                        plantTPU.TargetManual3 = plantTPU.TargetSystem3;
                        plantTPU.TargetManual4 = plantTPU.TargetSystem4;
                        plantTPU.TargetManual5 = plantTPU.TargetSystem5;
                        plantTPU.TargetManual6 = plantTPU.TargetSystem6;
                        plantTPU.TargetManual7 = plantTPU.TargetSystem7;

                        plantTPU.TotalTargetSystem = plantTPU.TargetSystem1 +
                                                      plantTPU.TargetSystem2 + plantTPU.TargetSystem3 +
                                                      plantTPU.TargetSystem4 + plantTPU.TargetSystem5 +
                                                      plantTPU.TargetSystem6 + plantTPU.TargetSystem7;
                        plantTPU.TotalTargetManual = plantTPU.TotalTargetSystem;
                    }

                    if (checkGroupCodeAfter && plantTPU.UnitCode != unitCodeNonZero) 
                    {
                        if (plantTPU.ProcessWorkHours7 != 0)
                        {
                            var tempTargetSystem7 = plantTPU.TargetSystem7;
                            plantTPU.TargetSystem7 += rest;
                            if (plantTPU.TargetSystem7 < 0) plantTPU.TargetSystem7 = 0;
                            else checkGroupCodeAfter = false;
                        }
                        else if (plantTPU.ProcessWorkHours6 != 0)
                        {
                            var tempTargetSystem6 = plantTPU.TargetSystem6;
                            plantTPU.TargetSystem6 += rest;
                            if (plantTPU.TargetSystem6 < 0) { 
                                plantTPU.TargetSystem6 = 0;
                                rest = rest + tempTargetSystem6.Value;
                            }
                            else checkGroupCodeAfter = false;
                        }
                        else if (plantTPU.ProcessWorkHours5 != 0)
                        {
                            var tempTargetSystem5 = plantTPU.TargetSystem5;
                            plantTPU.TargetSystem5 += rest;
                            if (plantTPU.TargetSystem5 < 0)
                            {
                                plantTPU.TargetSystem5 = 0;
                                rest = rest + tempTargetSystem5.Value;
                            }
                            else checkGroupCodeAfter = false;
                        }
                        else if (plantTPU.ProcessWorkHours4 != 0)
                        {
                            var tempTargetSystem4 = plantTPU.TargetSystem4;
                            plantTPU.TargetSystem4 += rest;
                            if (plantTPU.TargetSystem4 < 0)
                            {
                                plantTPU.TargetSystem4 = 0;
                                rest = rest + tempTargetSystem4.Value;
                            }
                            else checkGroupCodeAfter = false;
                        }
                        else if (plantTPU.ProcessWorkHours3 != 0)
                        {
                            var tempTargetSystem3 = plantTPU.TargetSystem3;
                            plantTPU.TargetSystem3 += rest;
                            if (plantTPU.TargetSystem3 < 0)
                            {
                                plantTPU.TargetSystem3 = 0;
                                rest = rest + tempTargetSystem3.Value;
                            }
                            else checkGroupCodeAfter = false;
                        }
                        else if (plantTPU.ProcessWorkHours2 != 0)
                        {
                            var tempTargetSystem2 = plantTPU.TargetSystem2;
                            plantTPU.TargetSystem2 += rest;
                            if (plantTPU.TargetSystem2 < 0)
                            {
                                plantTPU.TargetSystem2 = 0;
                                rest = rest + tempTargetSystem2.Value;
                            }
                            else checkGroupCodeAfter = false;
                        }
                        else if (plantTPU.ProcessWorkHours1 != 0)
                        {
                            var tempTargetSystem1 = plantTPU.TargetSystem1;
                            plantTPU.TargetSystem1 += rest;
                            if (plantTPU.TargetSystem1 < 0)
                            {
                                plantTPU.TargetSystem1 = 0;
                                rest = rest + tempTargetSystem1.Value;
                            }
                            else checkGroupCodeAfter = false;
                        }

                        plantTPU.TargetManual1 = plantTPU.TargetSystem1;
                        plantTPU.TargetManual2 = plantTPU.TargetSystem2;
                        plantTPU.TargetManual3 = plantTPU.TargetSystem3;
                        plantTPU.TargetManual4 = plantTPU.TargetSystem4;
                        plantTPU.TargetManual5 = plantTPU.TargetSystem5;
                        plantTPU.TargetManual6 = plantTPU.TargetSystem6;
                        plantTPU.TargetManual7 = plantTPU.TargetSystem7;

                        plantTPU.TotalTargetSystem = plantTPU.TargetSystem1 +
                                                      plantTPU.TargetSystem2 + plantTPU.TargetSystem3 +
                                                      plantTPU.TargetSystem4 + plantTPU.TargetSystem5 +
                                                      plantTPU.TargetSystem6 + plantTPU.TargetSystem7;
                        plantTPU.TotalTargetManual = plantTPU.TotalTargetSystem; 
                    }
                }
            }

            return InputPlantTPU.ListPlantTPU;
        }

        public List<PlanTPUDTO> CalculatePlantTPURevision(CalculatePlantTPUInput InputPlantTPU, float TargetWpp, bool SubmitStatus)
        {
            var wpp = _planWeeklyProductionPlaningRepo.GetByID(InputPlantTPU.KPSYear, InputPlantTPU.KPSWeek, InputPlantTPU.BrandCode, InputPlantTPU.LocationCode);
            var targetWPP = (decimal)wpp.Value1 * Constants.WPPConvert;
            var tempTargetWpp = targetWPP;
            
            // get start date by input week
            var listDateInWeek = _masterDataBll.GetDateByWeek(InputPlantTPU.KPSYear, InputPlantTPU.KPSWeek);
            var startDate = listDateInWeek.Any() ? listDateInWeek.FirstOrDefault() : DateTime.Now;

            //var LockDateBefore = InputPlantTPU.IsFilterCurrentDayForward ? new DateTime(Math.Max(DateTime.Now.AddDays(1).Ticks, InputPlantTPU.FilterCurrentDayForward.Ticks)) : DateTime.Now;
            var LockDateBefore = InputPlantTPU.IsFilterCurrentDayForward ? InputPlantTPU.FilterCurrentDayForward : startDate;
            var ProductionStartDate = InputPlantTPU.ListPlantTPU.FirstOrDefault().ProductionStartDate;

            var lowestUnit = InputPlantTPU.ListPlantTPU.Where(p => p.WorkerAlocation >= 0).Min(u => u.UnitCode);
            var unitCodeNonZero = InputPlantTPU.ListPlantTPU.Where(p => p.WorkerAlocation > 0).Min(u => u.UnitCode);

            var bCode = _masterDataBll.GetBrand(new GetBrandInput { BrandCode = InputPlantTPU.ListPlantTPU.FirstOrDefault().BrandCode });

            #region CALCULATE BOBOT
            foreach (var plantTPU in InputPlantTPU.ListPlantTPU)
            {
                if (InputPlantTPU.Conversion.ToLower() == Enums.Conversion.Stick.ToString().ToLower())
                {
                    plantTPU.TargetSystem1 = (float)Math.Round((decimal)(plantTPU.TargetSystem1 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetSystem2 = (float)Math.Round((decimal)(plantTPU.TargetSystem2 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetSystem3 = (float)Math.Round((decimal)(plantTPU.TargetSystem3 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetSystem4 = (float)Math.Round((decimal)(plantTPU.TargetSystem4 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetSystem5 = (float)Math.Round((decimal)(plantTPU.TargetSystem5 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetSystem6 = (float)Math.Round((decimal)(plantTPU.TargetSystem6 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetSystem7 = (float)Math.Round((decimal)(plantTPU.TargetSystem7 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);

                    plantTPU.BobotSystem1 = plantTPU.Target1 * Convert.ToDecimal(plantTPU.TargetSystem1);
                    plantTPU.BobotSystem2 = plantTPU.Target2 * Convert.ToDecimal(plantTPU.TargetSystem2);
                    plantTPU.BobotSystem3 = plantTPU.Target3 * Convert.ToDecimal(plantTPU.TargetSystem3);
                    plantTPU.BobotSystem4 = plantTPU.Target4 * Convert.ToDecimal(plantTPU.TargetSystem4);
                    plantTPU.BobotSystem5 = plantTPU.Target5 * Convert.ToDecimal(plantTPU.TargetSystem5);
                    plantTPU.BobotSystem6 = plantTPU.Target6 * Convert.ToDecimal(plantTPU.TargetSystem6);
                    plantTPU.BobotSystem7 = plantTPU.Target7 * Convert.ToDecimal(plantTPU.TargetSystem7);

                    plantTPU.TargetManual1 = (float)Math.Round((decimal)(plantTPU.TargetManual1 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetManual2 = (float)Math.Round((decimal)(plantTPU.TargetManual2 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetManual3 = (float)Math.Round((decimal)(plantTPU.TargetManual3 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetManual4 = (float)Math.Round((decimal)(plantTPU.TargetManual4 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetManual5 = (float)Math.Round((decimal)(plantTPU.TargetManual5 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetManual6 = (float)Math.Round((decimal)(plantTPU.TargetManual6 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);
                    plantTPU.TargetManual7 = (float)Math.Round((decimal)(plantTPU.TargetManual7 / bCode.StickPerBox), 0, MidpointRounding.AwayFromZero);

                    plantTPU.BobotManual1 = plantTPU.Target1 * Convert.ToDecimal(plantTPU.TargetManual1);
                    plantTPU.BobotManual2 = plantTPU.Target2 * Convert.ToDecimal(plantTPU.TargetManual2);
                    plantTPU.BobotManual3 = plantTPU.Target3 * Convert.ToDecimal(plantTPU.TargetManual3);
                    plantTPU.BobotManual4 = plantTPU.Target4 * Convert.ToDecimal(plantTPU.TargetManual4);
                    plantTPU.BobotManual5 = plantTPU.Target5 * Convert.ToDecimal(plantTPU.TargetManual5);
                    plantTPU.BobotManual6 = plantTPU.Target6 * Convert.ToDecimal(plantTPU.TargetManual6);
                    plantTPU.BobotManual7 = plantTPU.Target7 * Convert.ToDecimal(plantTPU.TargetManual7);
                }
                else 
                {
                    plantTPU.BobotSystem1 = plantTPU.Target1 * Convert.ToDecimal(plantTPU.TargetSystem1);
                    plantTPU.BobotSystem2 = plantTPU.Target2 * Convert.ToDecimal(plantTPU.TargetSystem2);
                    plantTPU.BobotSystem3 = plantTPU.Target3 * Convert.ToDecimal(plantTPU.TargetSystem3);
                    plantTPU.BobotSystem4 = plantTPU.Target4 * Convert.ToDecimal(plantTPU.TargetSystem4);
                    plantTPU.BobotSystem5 = plantTPU.Target5 * Convert.ToDecimal(plantTPU.TargetSystem5);
                    plantTPU.BobotSystem6 = plantTPU.Target6 * Convert.ToDecimal(plantTPU.TargetSystem6);
                    plantTPU.BobotSystem7 = plantTPU.Target7 * Convert.ToDecimal(plantTPU.TargetSystem7);

                    plantTPU.BobotManual1 = plantTPU.Target1 * Convert.ToDecimal(plantTPU.TargetManual1);
                    plantTPU.BobotManual2 = plantTPU.Target2 * Convert.ToDecimal(plantTPU.TargetManual2);
                    plantTPU.BobotManual3 = plantTPU.Target3 * Convert.ToDecimal(plantTPU.TargetManual3);
                    plantTPU.BobotManual4 = plantTPU.Target4 * Convert.ToDecimal(plantTPU.TargetManual4);
                    plantTPU.BobotManual5 = plantTPU.Target5 * Convert.ToDecimal(plantTPU.TargetManual5);
                    plantTPU.BobotManual6 = plantTPU.Target6 * Convert.ToDecimal(plantTPU.TargetManual6);
                    plantTPU.BobotManual7 = plantTPU.Target7 * Convert.ToDecimal(plantTPU.TargetManual7);
                }
            }
            #endregion

            // Total Bobot System 1 Week All Unit
            var totalBobotSystem = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotSystem1 + c.BobotSystem2 +
                                                                       c.BobotSystem3 + c.BobotSystem4 +
                                                                       c.BobotSystem5 + c.BobotSystem6 + c.BobotSystem7);

            // Current Total Target System 1 Week All Unit
            var totalTargetSystem = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetSystem1 + c.TargetSystem2 +
                                                                                 c.TargetSystem3 + c.TargetSystem4 +
                                                                                 c.TargetSystem5 + c.TargetSystem6 + c.TargetSystem7);

            // Total Bobot Manual 1 Week All Unit
            var totalBobotManual = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotManual1 + c.BobotManual2 +
                                                                       c.BobotManual3 + c.BobotManual4 +
                                                                       c.BobotManual5 + c.BobotManual6 + c.BobotManual7);

            // Current Total Target Manual 1 Week All Unit
            var totalTargetManual = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetManual1 + c.TargetManual2 +
                                                                                 c.TargetManual3 + c.TargetManual4 +
                                                                                 c.TargetManual5 + c.TargetManual6 + c.TargetManual7);

            #region CURRENT DAY FOWARD CONDITION
            if (InputPlantTPU.IsFilterCurrentDayForward)
            {
                var dayOfWeek = (int)InputPlantTPU.FilterCurrentDayForward.DayOfWeek;
                if (dayOfWeek == 0) dayOfWeek = 7;
                switch (dayOfWeek)
                {
                    case 2:
                        totalBobotSystem = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotSystem2 + c.BobotSystem3 + c.BobotSystem4 + 
                                                                               c.BobotSystem5 + c.BobotSystem6 + c.BobotSystem7);
                        totalTargetSystem = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetSystem2 + c.TargetSystem3 + c.TargetSystem4 +
                                                                                         c.TargetSystem5 + c.TargetSystem6 + c.TargetSystem7);
                        totalBobotManual = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotManual2 + c.BobotManual3 + c.BobotManual4 + 
                                                                               c.BobotManual5 + c.BobotManual6 + c.BobotManual7);
                        totalTargetManual = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetManual2 + c.TargetManual3 + c.TargetManual4 +
                                                                                         c.TargetManual5 + c.TargetManual6 + c.TargetManual7);
                        break;
                    case 3:
                        totalBobotSystem = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotSystem3 + c.BobotSystem4 + 
                                                                               c.BobotSystem5 + c.BobotSystem6 + c.BobotSystem7);
                        totalTargetSystem = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetSystem3 + c.TargetSystem4 +
                                                                                         c.TargetSystem5 + c.TargetSystem6 + c.TargetSystem7);
                        totalBobotManual = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotManual3 + c.BobotManual4 + 
                                                                               c.BobotManual5 + c.BobotManual6 + c.BobotManual7);
                        totalTargetManual = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetManual3 + c.TargetManual4 +
                                                                                         c.TargetManual5 + c.TargetManual6 + c.TargetManual7);
                        break;
                    case 4:
                        totalBobotSystem = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotSystem4 + c.BobotSystem5 + c.BobotSystem6 + c.BobotSystem7);
                        totalTargetSystem = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetSystem4 + c.TargetSystem5 + c.TargetSystem6 + c.TargetSystem7);
                        totalBobotManual = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotManual4 + c.BobotManual5 + c.BobotManual6 + c.BobotManual7);
                        totalTargetManual = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetManual4 + c.TargetManual5 + c.TargetManual6 + c.TargetManual7);
                        break;
                    case 5:
                        totalBobotSystem = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotSystem5 + c.BobotSystem6 + c.BobotSystem7);
                        totalTargetSystem = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetSystem5 + c.TargetSystem6 + c.TargetSystem7);
                        totalBobotManual = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotManual5 + c.BobotManual6 + c.BobotManual7);
                        totalTargetManual = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetManual5 + c.TargetManual6 + c.TargetManual7);
                        break;
                    case 6:
                        totalBobotSystem = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotSystem6 + c.BobotSystem7);
                        totalTargetSystem = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetSystem6 + c.TargetSystem7);
                        totalBobotManual = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotManual6 + c.BobotManual7);
                        totalTargetManual = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetManual6 + c.TargetManual7);
                        break;
                    case 7:
                        totalBobotSystem = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotSystem7);
                        totalTargetSystem = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetSystem7);
                        totalBobotManual = InputPlantTPU.ListPlantTPU.Sum(c => c.BobotManual7);
                        totalTargetManual = (decimal)InputPlantTPU.ListPlantTPU.Sum(c => c.TargetManual7);
                        break;
                }
            }
            #endregion

            foreach (var PlantTPU in InputPlantTPU.ListPlantTPU)
            {
                #region FORMULA SYSTEM
                // Monday
                if (ProductionStartDate.Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotSystem == 0) PlantTPU.TargetSystem1 = 0;
                    else 
                        PlantTPU.TargetSystem1 = (float)Math.Round(Convert.ToDouble(PlantTPU.BobotSystem1 / totalBobotSystem * totalTargetSystem), 0, MidpointRounding.AwayFromZero);
                }                                                                                                                                  
                // Tuesday
                if (ProductionStartDate.AddDays(1).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotSystem == 0) PlantTPU.TargetSystem2 = 0;
                    else
                        PlantTPU.TargetSystem2 = (float)Math.Round(Convert.ToDouble(PlantTPU.BobotSystem2 / totalBobotSystem * totalTargetSystem), 0, MidpointRounding.AwayFromZero);
                }
                // Wednesday
                if (ProductionStartDate.AddDays(2).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotSystem == 0) PlantTPU.TargetSystem3 = 0;
                    else
                        PlantTPU.TargetSystem3 = (float)Math.Round(Convert.ToDouble(PlantTPU.BobotSystem3 / totalBobotSystem * totalTargetSystem), 0, MidpointRounding.AwayFromZero);
                }
                // Thursday
                if (ProductionStartDate.AddDays(3).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotSystem == 0) PlantTPU.TargetSystem4 = 0;
                    else
                        PlantTPU.TargetSystem4 = (float)Math.Round(Convert.ToDouble(PlantTPU.BobotSystem4 / totalBobotSystem * totalTargetSystem), 0, MidpointRounding.AwayFromZero);
                }
                // Friday
                if (ProductionStartDate.AddDays(4).Date >= LockDateBefore.Date || SubmitStatus) 
                {
                    if (totalBobotSystem == 0) PlantTPU.TargetSystem5 = 0;
                    else
                        PlantTPU.TargetSystem5 = (float)Math.Round(Convert.ToDouble(PlantTPU.BobotSystem5 / totalBobotSystem * totalTargetSystem), 0, MidpointRounding.AwayFromZero);
                }
                // Saturday
                if (ProductionStartDate.AddDays(5).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotSystem == 0) PlantTPU.TargetSystem6 = 0;
                    else
                        PlantTPU.TargetSystem6 = (float)Math.Round(Convert.ToDouble(PlantTPU.BobotSystem6 / totalBobotSystem * totalTargetSystem), 0, MidpointRounding.AwayFromZero);
                }
                // Sunday
                if (ProductionStartDate.AddDays(6).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotSystem == 0) PlantTPU.TargetSystem7 = 0;
                    else
                        PlantTPU.TargetSystem7 = (float)Math.Round(Convert.ToDouble(PlantTPU.BobotSystem7 / totalBobotSystem * totalTargetSystem), 0, MidpointRounding.AwayFromZero);
                }

                PlantTPU.TotalTargetSystem = PlantTPU.TargetSystem1 + PlantTPU.TargetSystem2 + 
                                             PlantTPU.TargetSystem3 + PlantTPU.TargetSystem4 + 
                                             PlantTPU.TargetSystem5 + PlantTPU.TargetSystem6 + PlantTPU.TargetSystem7;

                #endregion

                #region FORMULA MANUAL
                // Monday
                if (ProductionStartDate.Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotManual == 0) PlantTPU.TargetManual1 = 0;
                    else
                        PlantTPU.TargetManual1 = (float)Math.Round(Convert.ToDouble((PlantTPU.BobotManual1 / totalBobotManual) * totalTargetManual), 0, MidpointRounding.AwayFromZero);
                }
                // Tuesday
                if (ProductionStartDate.AddDays(1).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotManual == 0) PlantTPU.TargetManual2 = 0;
                    else
                        PlantTPU.TargetManual2 = (float)Math.Round(Convert.ToDouble((PlantTPU.BobotManual2 / totalBobotManual) * totalTargetManual), 0, MidpointRounding.AwayFromZero);
                }
                // Wednesday
                if (ProductionStartDate.AddDays(2).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotManual == 0) PlantTPU.TargetManual3 = 0;
                    else
                        PlantTPU.TargetManual3 = (float)Math.Round(Convert.ToDouble((PlantTPU.BobotManual3 / totalBobotManual) * totalTargetManual), 0, MidpointRounding.AwayFromZero);
                }
                // Thursday
                if (ProductionStartDate.AddDays(3).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotManual == 0) PlantTPU.TargetManual4 = 0;
                    else
                        PlantTPU.TargetManual4 = (float)Math.Round(Convert.ToDouble((PlantTPU.BobotManual4 / totalBobotManual) * totalTargetManual), 0, MidpointRounding.AwayFromZero);
                }
                // Friday
                if (ProductionStartDate.AddDays(4).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotManual == 0) PlantTPU.TargetManual5 = 0;
                    else
                        PlantTPU.TargetManual5 = (float)Math.Round(Convert.ToDouble((PlantTPU.BobotManual5 / totalBobotManual) * totalTargetManual), 0, MidpointRounding.AwayFromZero);
                }
                // Saturday
                if (ProductionStartDate.AddDays(5).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotManual == 0) PlantTPU.TargetManual6 = 0;
                    else
                        PlantTPU.TargetManual6 = (float)Math.Round(Convert.ToDouble((PlantTPU.BobotManual6 / totalBobotManual) * totalTargetManual), 0, MidpointRounding.AwayFromZero);
                }
                // Sunday
                if (ProductionStartDate.AddDays(6).Date >= LockDateBefore.Date || SubmitStatus)
                {
                    if (totalBobotManual == 0) PlantTPU.TargetManual7 = 0;
                    else
                        PlantTPU.TargetManual7 = (float)Math.Round(Convert.ToDouble((PlantTPU.BobotManual7 / totalBobotManual) * totalTargetManual), 0, MidpointRounding.AwayFromZero);
                }

                PlantTPU.TotalTargetManual = PlantTPU.TargetManual1 + PlantTPU.TargetManual2 + 
                                             PlantTPU.TargetManual3 + PlantTPU.TargetManual4 + 
                                             PlantTPU.TargetManual5 + PlantTPU.TargetManual6 + PlantTPU.TargetManual7;

                #endregion

                PlantTPU.CreatedDate = DateTime.Now;
                PlantTPU.UpdatedDate = DateTime.Now;
            }

            #region CONVERT STICK OR BOX
            foreach (var plantTPU in InputPlantTPU.ListPlantTPU)
            {
                if (InputPlantTPU.Conversion.ToLower() == Enums.Conversion.Stick.ToString().ToLower())
                {
                    plantTPU.TargetSystem1 *= bCode.StickPerBox;
                    plantTPU.TargetSystem2 *= bCode.StickPerBox;
                    plantTPU.TargetSystem3 *= bCode.StickPerBox;
                    plantTPU.TargetSystem4 *= bCode.StickPerBox;
                    plantTPU.TargetSystem5 *= bCode.StickPerBox;
                    plantTPU.TargetSystem6 *= bCode.StickPerBox;
                    plantTPU.TargetSystem7 *= bCode.StickPerBox;

                    plantTPU.TargetManual1 *= bCode.StickPerBox;
                    plantTPU.TargetManual2 *= bCode.StickPerBox;
                    plantTPU.TargetManual3 *= bCode.StickPerBox;
                    plantTPU.TargetManual4 *= bCode.StickPerBox;
                    plantTPU.TargetManual5 *= bCode.StickPerBox;
                    plantTPU.TargetManual6 *= bCode.StickPerBox;
                    plantTPU.TargetManual7 *= bCode.StickPerBox;

                    plantTPU.TotalTargetSystem = plantTPU.TargetSystem1 + plantTPU.TargetSystem2 + plantTPU.TargetSystem3 +
                                                            plantTPU.TargetSystem4 + plantTPU.TargetSystem5 + plantTPU.TargetSystem6 + plantTPU.TargetSystem7;

                    plantTPU.TotalTargetManual = plantTPU.TargetManual1 + plantTPU.TargetManual2 + plantTPU.TargetManual3 +
                                                                 plantTPU.TargetManual4 + plantTPU.TargetManual5 + plantTPU.TargetManual6 + plantTPU.TargetManual7;
                }
            }
            #endregion

            var tempTotalTargetSystem = (decimal)InputPlantTPU.ListPlantTPU.Sum(t => t.TotalTargetSystem);
            var tempTotalTargetManual = (decimal)InputPlantTPU.ListPlantTPU.Sum(t => t.TotalTargetManual);

            #region CALCULATE SELISIH

            var tempTargetWPP = 0m;
            if (InputPlantTPU.Conversion.ToLower() != Enums.Conversion.Stick.ToString().ToLower())
                tempTargetWPP = (decimal)Math.Round((targetWPP / bCode.StickPerBox.Value), 2, MidpointRounding.AwayFromZero);
            else
                tempTargetWPP = (decimal)targetWPP;

            #region CALCULATE SELISIH SYSTEM
            if (tempTargetWPP != tempTotalTargetSystem)
            {
                var restSystem = (float)Math.Round((tempTargetWPP - tempTotalTargetSystem), 2, MidpointRounding.AwayFromZero);

                var checkGroupCodeAfterSystem = false;

                foreach (var plantTPU in InputPlantTPU.ListPlantTPU)
                {
                    if (plantTPU.UnitCode == unitCodeNonZero)
                    {
                        var plantTPUNonZeroAllocation = InputPlantTPU.ListPlantTPU.Where(p => p.UnitCode == unitCodeNonZero).FirstOrDefault();

                        if (plantTPUNonZeroAllocation.ProcessWorkHours7 != 0)
                        {
                            var tempTargetSystem7 = plantTPUNonZeroAllocation.TargetSystem7;
                            plantTPUNonZeroAllocation.TargetSystem7 += restSystem;
                            if (plantTPUNonZeroAllocation.TargetSystem7 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem7 = 0;
                                restSystem = restSystem + tempTargetSystem7.Value;
                                checkGroupCodeAfterSystem = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours6 != 0)
                        {
                            var tempTargetSystem6 = plantTPUNonZeroAllocation.TargetSystem6;
                            plantTPUNonZeroAllocation.TargetSystem6 += restSystem;
                            if (plantTPUNonZeroAllocation.TargetSystem6 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem6 = 0;
                                restSystem = restSystem + tempTargetSystem6.Value;
                                checkGroupCodeAfterSystem = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours5 != 0)
                        {
                            var tempTargetSystem5 = plantTPUNonZeroAllocation.TargetSystem5;
                            plantTPUNonZeroAllocation.TargetSystem5 += restSystem;
                            if (plantTPUNonZeroAllocation.TargetSystem5 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem5 = 0;
                                restSystem = restSystem + tempTargetSystem5.Value;
                                checkGroupCodeAfterSystem = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours4 != 0)
                        {
                            var tempTargetSystem4 = plantTPUNonZeroAllocation.TargetSystem4;
                            plantTPUNonZeroAllocation.TargetSystem4 += restSystem;
                            if (plantTPUNonZeroAllocation.TargetSystem4 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem4 = 0;
                                restSystem = restSystem + tempTargetSystem4.Value;
                                checkGroupCodeAfterSystem = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours3 != 0)
                        {
                            var tempTargetSystem3 = plantTPUNonZeroAllocation.TargetSystem3;
                            plantTPUNonZeroAllocation.TargetSystem3 += restSystem;
                            if (plantTPUNonZeroAllocation.TargetSystem3 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem3 = 0;
                                restSystem = restSystem + tempTargetSystem3.Value;
                                checkGroupCodeAfterSystem = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours2 != 0)
                        {
                            var tempTargetSystem2 = plantTPUNonZeroAllocation.TargetSystem2;
                            plantTPUNonZeroAllocation.TargetSystem2 += restSystem;
                            if (plantTPUNonZeroAllocation.TargetSystem2 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem2 = 0;
                                restSystem = restSystem + tempTargetSystem2.Value;
                                checkGroupCodeAfterSystem = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours1 != 0)
                        {
                            var tempTargetSystem1 = plantTPUNonZeroAllocation.TargetSystem1;
                            plantTPUNonZeroAllocation.TargetSystem1 += restSystem;
                            if (plantTPUNonZeroAllocation.TargetSystem1 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetSystem1 = 0;
                                restSystem = restSystem + tempTargetSystem1.Value;
                                checkGroupCodeAfterSystem = true;
                            }
                        }
                    }

                    if (checkGroupCodeAfterSystem && plantTPU.UnitCode != unitCodeNonZero)
                    {
                        if (plantTPU.ProcessWorkHours7 != 0)
                        {
                            var tempTargetSystem7 = plantTPU.TargetSystem7;
                            plantTPU.TargetSystem7 += restSystem;
                            if (plantTPU.TargetSystem7 < 0) plantTPU.TargetSystem7 = 0;
                            else checkGroupCodeAfterSystem = false;
                        }
                        else if (plantTPU.ProcessWorkHours6 != 0)
                        {
                            var tempTargetSystem6 = plantTPU.TargetSystem6;
                            plantTPU.TargetSystem6 += restSystem;
                            if (plantTPU.TargetSystem6 < 0)
                            {
                                plantTPU.TargetSystem6 = 0;
                                restSystem = restSystem + tempTargetSystem6.Value;
                            }
                            else checkGroupCodeAfterSystem = false;
                        }
                        else if (plantTPU.ProcessWorkHours5 != 0)
                        {
                            var tempTargetSystem5 = plantTPU.TargetSystem5;
                            plantTPU.TargetSystem5 += restSystem;
                            if (plantTPU.TargetSystem5 < 0)
                            {
                                plantTPU.TargetSystem5 = 0;
                                restSystem = restSystem + tempTargetSystem5.Value;
                            }
                            else checkGroupCodeAfterSystem = false;
                        }
                        else if (plantTPU.ProcessWorkHours4 != 0)
                        {
                            var tempTargetSystem4 = plantTPU.TargetSystem4;
                            plantTPU.TargetSystem4 += restSystem;
                            if (plantTPU.TargetSystem4 < 0)
                            {
                                plantTPU.TargetSystem4 = 0;
                                restSystem = restSystem + tempTargetSystem4.Value;
                            }
                            else checkGroupCodeAfterSystem = false;
                        }
                        else if (plantTPU.ProcessWorkHours3 != 0)
                        {
                            var tempTargetSystem3 = plantTPU.TargetSystem3;
                            plantTPU.TargetSystem3 += restSystem;
                            if (plantTPU.TargetSystem3 < 0)
                            {
                                plantTPU.TargetSystem3 = 0;
                                restSystem = restSystem + tempTargetSystem3.Value;
                            }
                            else checkGroupCodeAfterSystem = false;
                        }
                        else if (plantTPU.ProcessWorkHours2 != 0)
                        {
                            var tempTargetSystem2 = plantTPU.TargetSystem2;
                            plantTPU.TargetSystem2 += restSystem;
                            if (plantTPU.TargetSystem2 < 0)
                            {
                                plantTPU.TargetSystem2 = 0;
                                restSystem = restSystem + tempTargetSystem2.Value;
                            }
                            else checkGroupCodeAfterSystem = false;
                        }
                        else if (plantTPU.ProcessWorkHours1 != 0)
                        {
                            var tempTargetSystem1 = plantTPU.TargetSystem1;
                            plantTPU.TargetSystem1 += restSystem;
                            if (plantTPU.TargetSystem1 < 0)
                            {
                                plantTPU.TargetSystem1 = 0;
                                restSystem = restSystem + tempTargetSystem1.Value;
                            }
                            else checkGroupCodeAfterSystem = false;
                        }
                    }
                }
            }
            #endregion

            #region CALCULATE SELISIH MANUAL
            if (tempTargetWPP != tempTotalTargetManual)
            {
                var restManual = (float)Math.Round((tempTargetWPP - tempTotalTargetManual), 2, MidpointRounding.AwayFromZero);

                var checkGroupCodeAfterManual = false;

                foreach (var plantTPU in InputPlantTPU.ListPlantTPU)
                {
                    if (plantTPU.UnitCode == unitCodeNonZero)
                    {
                        var plantTPUNonZeroAllocation = InputPlantTPU.ListPlantTPU.Where(p => p.UnitCode == unitCodeNonZero).FirstOrDefault();

                        if (plantTPUNonZeroAllocation.ProcessWorkHours7 != 0)
                        {
                            var tempTargetManual7 = plantTPUNonZeroAllocation.TargetManual7;
                            plantTPUNonZeroAllocation.TargetManual7 += restManual;
                            if (plantTPUNonZeroAllocation.TargetManual7 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetManual7 = 0;
                                restManual = restManual + tempTargetManual7.Value;
                                checkGroupCodeAfterManual = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours6 != 0)
                        {
                            var tempTargetManual6 = plantTPUNonZeroAllocation.TargetManual6;
                            plantTPUNonZeroAllocation.TargetManual6 += restManual;
                            if (plantTPUNonZeroAllocation.TargetManual6 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetManual6 = 0;
                                restManual = restManual + tempTargetManual6.Value;
                                checkGroupCodeAfterManual = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours5 != 0)
                        {
                            var tempTargetManual5 = plantTPUNonZeroAllocation.TargetManual5;
                            plantTPUNonZeroAllocation.TargetManual5 += restManual;
                            if (plantTPUNonZeroAllocation.TargetManual5 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetManual5 = 0;
                                restManual = restManual + tempTargetManual5.Value;
                                checkGroupCodeAfterManual = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours4 != 0)
                        {
                            var tempTargetManual4 = plantTPUNonZeroAllocation.TargetManual4;
                            plantTPUNonZeroAllocation.TargetManual4 += restManual;
                            if (plantTPUNonZeroAllocation.TargetManual4 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetManual4 = 0;
                                restManual = restManual + tempTargetManual4.Value;
                                checkGroupCodeAfterManual = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours3 != 0)
                        {
                            var tempTargetManual3 = plantTPUNonZeroAllocation.TargetManual3;
                            plantTPUNonZeroAllocation.TargetManual3 += restManual;
                            if (plantTPUNonZeroAllocation.TargetManual3 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetManual3 = 0;
                                restManual = restManual + tempTargetManual3.Value;
                                checkGroupCodeAfterManual = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours2 != 0)
                        {
                            var tempTargetManual2 = plantTPUNonZeroAllocation.TargetManual2;
                            plantTPUNonZeroAllocation.TargetManual2 += restManual;
                            if (plantTPUNonZeroAllocation.TargetManual2 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetManual2 = 0;
                                restManual = restManual + tempTargetManual2.Value;
                                checkGroupCodeAfterManual = true;
                            }
                        }
                        else if (plantTPUNonZeroAllocation.ProcessWorkHours1 != 0)
                        {
                            var tempTargetManual1 = plantTPUNonZeroAllocation.TargetManual1;
                            plantTPUNonZeroAllocation.TargetManual1 += restManual;
                            if (plantTPUNonZeroAllocation.TargetManual1 < 0)
                            {
                                plantTPUNonZeroAllocation.TargetManual1 = 0;
                                restManual = restManual + tempTargetManual1.Value;
                                checkGroupCodeAfterManual = true;
                            }
                        }
                    }

                    if (checkGroupCodeAfterManual && plantTPU.UnitCode != unitCodeNonZero)
                    {
                        if (plantTPU.ProcessWorkHours7 != 0)
                        {
                            var tempTargetManual7 = plantTPU.TargetManual7;
                            plantTPU.TargetManual7 += restManual;
                            if (plantTPU.TargetManual7 < 0) plantTPU.TargetManual7 = 0;
                            else checkGroupCodeAfterManual = false;
                        }
                        else if (plantTPU.ProcessWorkHours6 != 0)
                        {
                            var tempTargetManual6 = plantTPU.TargetManual6;
                            plantTPU.TargetManual6 += restManual;
                            if (plantTPU.TargetManual6 < 0)
                            {
                                plantTPU.TargetManual6 = 0;
                                restManual = restManual + tempTargetManual6.Value;
                            }
                            else checkGroupCodeAfterManual = false;
                        }
                        else if (plantTPU.ProcessWorkHours5 != 0)
                        {
                            var tempTargetManual5 = plantTPU.TargetManual5;
                            plantTPU.TargetManual5 += restManual;
                            if (plantTPU.TargetManual5 < 0)
                            {
                                plantTPU.TargetManual5 = 0;
                                restManual = restManual + tempTargetManual5.Value;
                            }
                            else checkGroupCodeAfterManual = false;
                        }
                        else if (plantTPU.ProcessWorkHours4 != 0)
                        {
                            var tempTargetManual4 = plantTPU.TargetManual4;
                            plantTPU.TargetManual4 += restManual;
                            if (plantTPU.TargetManual4 < 0)
                            {
                                plantTPU.TargetManual4 = 0;
                                restManual = restManual + tempTargetManual4.Value;
                            }
                            else checkGroupCodeAfterManual = false;
                        }
                        else if (plantTPU.ProcessWorkHours3 != 0)
                        {
                            var tempTargetManual3 = plantTPU.TargetManual3;
                            plantTPU.TargetManual3 += restManual;
                            if (plantTPU.TargetManual3 < 0)
                            {
                                plantTPU.TargetManual3 = 0;
                                restManual = restManual + tempTargetManual3.Value;
                            }
                            else checkGroupCodeAfterManual = false;
                        }
                        else if (plantTPU.ProcessWorkHours2 != 0)
                        {
                            var tempTargetManual2 = plantTPU.TargetManual2;
                            plantTPU.TargetManual2 += restManual;
                            if (plantTPU.TargetManual2 < 0)
                            {
                                plantTPU.TargetManual2 = 0;
                                restManual = restManual + tempTargetManual2.Value;
                            }
                            else checkGroupCodeAfterManual = false;
                        }
                        else if (plantTPU.ProcessWorkHours1 != 0)
                        {
                            var tempTargetManual1 = plantTPU.TargetManual1;
                            plantTPU.TargetManual1 += restManual;
                            if (plantTPU.TargetManual1 < 0)
                            {
                                plantTPU.TargetManual1 = 0;
                                restManual = restManual + tempTargetManual1.Value;
                            }
                            else checkGroupCodeAfterManual = false;
                        }
                    }
                }
            }
            #endregion

            #endregion
            return InputPlantTPU.ListPlantTPU;
        }

        public void SendEmailSubmitPlantTpu(GetPlanTPUsInput input, string currUserName)
        {
            // Get Unit Code TPU
            var tpoTPK = _planTPURepo.Get(c => c.LocationCode == input.LocationCode && c.BrandCode == input.BrandCode && 
                                               c.Shift == input.Shift && c.KPSWeek == input.KPSWeek && c.KPSYear == input.KPSYear).Select(c => c.UnitCode);

            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = input.LocationCode,
                BrandCode = input.BrandCode,
                KpsWeek = input.KPSWeek,
                KpsYear = input.KPSYear,
                Shift = input.Shift,
                FunctionName = Enums.PageName.TargetProductionUnit.ToString(),
                ButtonName = Enums.ButtonName.Submit.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.PlantTPU),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.PlantTargetProductionGroup),
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
                emailInput.UnitCode = item.Unit;
                var email = new MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailPlantTPK(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }

        private string CreateBodyMailPlantTPK(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Target Produksi Kelompok (TPK) sudah tersedia, Silakan melanjutkan proses berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/PlanningPlantTPK/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.UnitCode + "/"
                                                                   + emailInput.BrandCode + "/"
                                                                    + emailInput.Shift + "/"
                                                                   + emailInput.KpsYear + "/"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + emailInput.IDResponsibility.ToString()
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public void SendEmailSubmitPlantTpk(GetPlantTPKsInput input, string currUserName)
        {
            // Get First Data PlantTPK
            var plantTPK = _planPlantTargetProductionKelompokRepo.Get(c => c.LocationCode == input.LocationCode && 
                                                                            c.UnitCode == input.UnitCode && 
                                                                            c.BrandCode == input.BrandCode &&
                                                                            c.KPSYear == input.KPSYear &&
                                                                            c.KPSWeek == input.KPSWeek &&
                                                                            c.Shift == input.Shift).FirstOrDefault();

            // Get first date of week
            var availDate = _masterDataBll.GetFirstDateByYearWeek(input.KPSYear.Value, input.KPSWeek.Value).Date;

            // Set Date where ProcessWorkHour1-7 > 0 to used on hyperlink
            if ((int)plantTPK.ProcessWorkHours1 > 0)
            {
                availDate = availDate.AddDays(0);
            }
            else if ((int)plantTPK.ProcessWorkHours2 > 0)
            {
                availDate = availDate.AddDays(1);
            }
            else if ((int)plantTPK.ProcessWorkHours3 > 0)
            {
                availDate = availDate.AddDays(2);
            }
            else if ((int)plantTPK.ProcessWorkHours4 > 0)
            {
                availDate = availDate.AddDays(3);
            }
            else if ((int)plantTPK.ProcessWorkHours5 > 0)
            {
                availDate = availDate.AddDays(4);
            }
            else if ((int)plantTPK.ProcessWorkHours6 > 0)
            {
                availDate = availDate.AddDays(5);
            }
            else if ((int)plantTPK.ProcessWorkHours7 > 0)
            {
                availDate = availDate.AddDays(6);
            }

            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = input.LocationCode,
                BrandCode = input.BrandCode,
                KpsWeek = input.KPSWeek,
                KpsYear = input.KPSYear,
                Shift = input.Shift,
                UnitCode = input.UnitCode,
                Process = plantTPK == null ? null : plantTPK.ProcessGroup,
                GroupCode = plantTPK == null ? null : plantTPK.GroupCode,
                Date = plantTPK == null ? _masterDataBll.GetFirstDateByYearWeek(input.KPSYear.Value, input.KPSWeek.Value).Date : availDate,
                FunctionName = Enums.PageName.PlantTargetProductionGroup.ToString(),
                ButtonName = Enums.ButtonName.Submit.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.PlanTPK),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.PlantProductionEntry),
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
                emailInput.UnitCode = item.Unit;
                var email = new MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailSubmitPlantTPK(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }

        private string CreateBodyMailSubmitPlantTPK(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Entry (Eblek)  sudah tersedia, Silakan melanjutkan proses berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<p><a href= webrooturl/ExePlantProductionEntry/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.UnitCode + "/"
                                                                   + emailInput.Shift + "/"
                                                                   + emailInput.Process + "/"
                                                                   + emailInput.GroupCode + "/"
                                                                   + emailInput.BrandCode + "/"
                                                                   + emailInput.KpsYear + "/"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + emailInput.Date.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString() + ">" 
                                                                   + emailInput.FunctionNameDestination +"</a></p>"
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public PlanTPUStatusDTO GetStatePlanTPU(string locationCode, string brandCode, int kpsYear, int kpsWeek, int shift)
        {
            var dateRange = _masterDataBll.GetWeekByYearAndWeek(kpsYear, kpsWeek);

            var StatusModel = new PlanTPUStatusDTO();

            if (dateRange.StartDate.HasValue)
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

                var WPPTransLog = _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(WPPTransCode, (int)HMS.SKTIS.Core.Enums.IdFlow.WPPTPKTPOSubmit);

                var TPUSubmitLog = _utilitiesBll.GetTransactionLogByTransCodeAndIDFlow(TPUTransCode, (int)HMS.SKTIS.Core.Enums.IdFlow.PlanningPlantTPUSubmit);

                var isWPPReSubmitted = -1;

                if (WPPTransLog != null && TPUSubmitLog != null)
                {
                    isWPPReSubmitted = DateTime.Compare(WPPTransLog.TransactionDate, TPUSubmitLog.TransactionDate);
                    StatusModel.Resubmit = isWPPReSubmitted > 0 ? true : false;
                }


                StatusModel.SubmitLog = isWPPReSubmitted < 0 && TPUSubmitLog != null ? TPUSubmitLog : null;
                StatusModel.Dates = new List<DateStateDTO>();

                for (int day = 0; day <= 6; day++)
                {
                    var dateState = new DateStateDTO()
                    {
                        Date = dateRange.StartDate.Value.AddDays(day)
                    };

                    if (StatusModel.SubmitLog == null && isWPPReSubmitted < 0)
                    {
                        dateState.IsActive = true;
                    }
                    else
                    {
                        if (isWPPReSubmitted > 0)
                        {
                            dateState.IsActive = (dateState.Date.Date >= DateTime.Now.Date) ? true : false;
                        }
                        if (dateState.Date.Date >= DateTime.Now.Date)
                        {
                            dateState.IsActive = true;
                        }
                    }

                    StatusModel.Dates.Add(dateState);
                }
            }

            return StatusModel;
        }

        #endregion

        #region Individual Capacity

        /// <summary>
        /// Gets the planning plant individual capacity work hours.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<PlanningPlantIndividualCapacityWorkHourDTO> GetPlanningPlantIndividualCapacityWorkHours(GetPlanningPlantIndividualCapacityWorkHourInput input)
        {
            var queryFilter = PredicateHelper.True<PlanPlantIndividualCapacityWorkHour>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.Unit))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.Unit);
            }

            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandCode);
            }

            if (!string.IsNullOrEmpty(input.Process) && input.Process == input.CapacityOfProcess)
            {
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.Process);
            }

            if (!string.IsNullOrEmpty(input.Group))
            {
                queryFilter = queryFilter.And(p => p.GroupCode == input.Group);
            }

            if (!string.IsNullOrEmpty(input.CapacityOfProcess) && input.Process != input.CapacityOfProcess)
            {
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.CapacityOfProcess);
            }

            queryFilter = queryFilter.And(p => p.StatusActive == true);

            Func<IQueryable<PlanPlantIndividualCapacityWorkHour>, IOrderedQueryable<PlanPlantIndividualCapacityWorkHour>> orderByFilter = null;
            if (!string.IsNullOrEmpty(input.SortExpression) && !string.IsNullOrEmpty(input.SortOrder))
            {
                var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
                orderByFilter = sortCriteria.GetOrderByFunc<PlanPlantIndividualCapacityWorkHour>();
            }

            var dbResult = _planningPlantIndividualCapacityWorkHourRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<PlanningPlantIndividualCapacityWorkHourDTO>>(dbResult);
        }

        /// <summary>
        /// Inserts the planning plant individual capacity work hour.
        /// </summary>
        /// <param name="ppcwhDto">The PPCWH dto.</param>
        /// <returns></returns>
        public PlanningPlantIndividualCapacityWorkHourDTO SavePlanningPlantIndividualCapacityWorkHour(PlanningPlantIndividualCapacityWorkHourDTO ppcwhDto)
        {
            var dbResult = _planningPlantIndividualCapacityWorkHourRepo.GetByID(ppcwhDto.BrandGroupCode, ppcwhDto.EmployeeID, ppcwhDto.GroupCode, ppcwhDto.UnitCode, ppcwhDto.LocationCode, ppcwhDto.ProcessGroup);
            if (dbResult == null)
            {
                dbResult = Mapper.Map<PlanPlantIndividualCapacityWorkHour>(ppcwhDto);

                dbResult.CreatedDate = DateTime.Now;
                dbResult.UpdatedDate = DateTime.Now;
                _planningPlantIndividualCapacityWorkHourRepo.Insert(dbResult);
            }
            else
            {
                dbResult.UpdatedDate = DateTime.Now;
                Mapper.Map(ppcwhDto, dbResult);
                _planningPlantIndividualCapacityWorkHourRepo.Update(dbResult);
            }
            _uow.SaveChanges();
            return Mapper.Map(dbResult, ppcwhDto);
        }

        public List<ExePlantProductionEntryVerificationDTO> GeteExePlantProductionEntryVerificationBll(
            GetExePlantProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (input.ProductionDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate); //hold dulu ini
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            }

            if (!string.IsNullOrEmpty(input.ProcessGroup))
            {
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.ProcessGroup);
            }

            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            }

            if (input.Shift.HasValue)
            {
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            }

            if (input.KpsYear.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSYear == input.KpsYear);
            }

            if (input.KpsWeek.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSWeek == input.KpsWeek);
            }

            if (input.ExeProductionEntryReleaseCannotNull == true)
            {
                queryFilter = queryFilter.And(p => p.ExeProductionEntryRelease != null);
            }

            //if (input.ProductionDate.HasValue)
            //{
            //    queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate.Value);
            //}

            if (input.UtilTransactionLogsCannotNull == true)
            {
                var exePlantProductionEntryVerificationResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);

                if (exePlantProductionEntryVerificationResult != null)
                {
                    var queryFilterutilTransactionLog = PredicateHelper.True<UtilTransactionLog>();

                    foreach (var exePlantProductionEntryVerification in exePlantProductionEntryVerificationResult)
                    {

                        var transactionCode = exePlantProductionEntryVerification.ProductionEntryCode.Replace("EBL", "WPC");

                        var utilTransactionLog = _utilTransactionLogRepo.Get(p => p.TransactionCode == transactionCode).OrderByDescending(p => p.CreatedDate).FirstOrDefault();

                        if (utilTransactionLog != null && utilTransactionLog.IDFlow == 26)
                        {
                            queryFilterutilTransactionLog =
                            queryFilterutilTransactionLog.And(
                                p => p.TransactionCode == transactionCode);
                        }

                    }

                    var utilTransactionLogs = _utilTransactionLogRepo.Get(queryFilterutilTransactionLog);

                    if (utilTransactionLogs != null)
                    {
                        foreach (var transactionLog in utilTransactionLogs)
                        {
                            var transactionCode = transactionLog.TransactionCode.Replace("WPC",
                            "EBL");

                            exePlantProductionEntryVerificationResult =
                                exePlantProductionEntryVerificationResult.Where(
                                    p => p.ProductionEntryCode == transactionCode);
                        }
                    }
                    else
                    {
                        exePlantProductionEntryVerificationResult = null;
                    }


                }


                //return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(exePlantProductionEntryVerificationResult);
                var hasilMapper2 = exePlantProductionEntryVerificationResult.Select((
                    s => new ExePlantProductionEntryVerificationDTO
                    {
                        LocationCode = s.LocationCode,
                        ProductionEntryCode = s.ProductionEntryCode,
                        UnitCode = s.UnitCode,
                        Shift = s.Shift,
                        UpdatedDate = s.UpdatedDate.GetValueOrDefault(),
                        ProcessGroup = s.ProcessGroup,
                        ProcessOrder = s.ProcessOrder,
                        GroupCode = s.GroupCode,
                        BrandCode = s.BrandCode,
                        ProductionDate = s.ProductionDate,
                        TPKValue = s.TPKValue
                    }
                    )).ToList();
                return hasilMapper2;
            }

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);
            //return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult);
            var hasilMapper = dbResult.Select((
                s => new ExePlantProductionEntryVerificationDTO
                {
                    LocationCode = s.LocationCode,
                    ProductionEntryCode = s.ProductionEntryCode,
                    UnitCode = s.UnitCode,
                    Shift = s.Shift,
                    UpdatedDate = s.UpdatedDate.GetValueOrDefault(),
                    ProcessGroup = s.ProcessGroup,
                    ProcessOrder  = s.ProcessOrder,       
                    GroupCode = s.GroupCode,
                    BrandCode  =s.BrandCode,
                    ProductionDate = s.ProductionDate,
                    TPKValue = s.TPKValue
                }
                )).ToList();

            return hasilMapper;
        }

        public List<ExePlantProductionEntryVerificationDTO> GeteExePlantProductionEntryVerification(
            GetExePlantProductionEntryVerificationInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (input.DatePopUp.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ProductionDate == input.DatePopUp); //hold dulu ini
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            }

            if (!string.IsNullOrEmpty(input.ProcessGroup))
            {
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.ProcessGroup);
            }

            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            }

            if (input.Shift.HasValue)
            {
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            }

            if (input.KpsYear.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSYear == input.KpsYear);
            }

            if (input.KpsWeek.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSWeek == input.KpsWeek);
            }

            if (input.ExeProductionEntryReleaseCannotNull == true)
            {
                queryFilter = queryFilter.And(p => p.ExeProductionEntryRelease != null);
            }

            //if (input.ProductionDate.HasValue)
            //{
            //    queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate.Value);
            //}

            if (input.UtilTransactionLogsCannotNull == true)
            {
                var exePlantProductionEntryVerificationResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);

                if (exePlantProductionEntryVerificationResult != null)
                {
                    var queryFilterutilTransactionLog = PredicateHelper.True<UtilTransactionLog>();

                    foreach (var exePlantProductionEntryVerification in exePlantProductionEntryVerificationResult)
                    {

                        var transactionCode = exePlantProductionEntryVerification.ProductionEntryCode.Replace("EBL", "WPC");

                        var utilTransactionLog = _utilTransactionLogRepo.Get(p => p.TransactionCode == transactionCode).OrderByDescending(p => p.CreatedDate).FirstOrDefault();

                        if (utilTransactionLog != null && utilTransactionLog.IDFlow == 26)
                        {
                            queryFilterutilTransactionLog =
                            queryFilterutilTransactionLog.And(
                                p => p.TransactionCode == transactionCode);
                        }

                    }

                    var utilTransactionLogs = _utilTransactionLogRepo.Get(queryFilterutilTransactionLog);

                    if (utilTransactionLogs != null)
                    {
                        foreach (var transactionLog in utilTransactionLogs)
                        {
                            var transactionCode = transactionLog.TransactionCode.Replace("WPC",
                            "EBL");

                            exePlantProductionEntryVerificationResult =
                                exePlantProductionEntryVerificationResult.Where(
                                    p => p.ProductionEntryCode == transactionCode);
                        }
                    }
                    else
                    {
                        exePlantProductionEntryVerificationResult = null;
                    }


                }


                return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(exePlantProductionEntryVerificationResult);
            }

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);
            return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult);
        }

        public List<ExePlantProductionEntryVerificationDTO> GeteExePlantProductionEntryVerification(
            GetPlanningPlantIndividualCapacityByReferenceInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(input.Location))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.Location);
            }

            if (!string.IsNullOrEmpty(input.Unit))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.Unit);
            }

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
            {
                var dbResultMstGenBrand = _masterDataBll.GetBrands(new GetBrandInput
                {
                    BrandGroupCode = input.BrandGroupCode
                }).Select(c => c.BrandCode);

                if (dbResultMstGenBrand.Any())
                {
                    queryFilter = queryFilter.And(p => dbResultMstGenBrand.Contains(p.BrandCode));
                    //foreach (var data in dbResultMstGenBrand)
                    //{
                    //    queryFilter = queryFilter.And(p => dbResultMstGenBrand.Contains(p.BrandCode));
                    //    queryFilter = queryFilter.And(p => data.BrandCode == p.BrandCode);
                    //}

                }
            }

            if (!string.IsNullOrEmpty(input.Process))
            {
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.Process);
            }

            if (!string.IsNullOrEmpty(input.Group))
            {
                queryFilter = queryFilter.And(p => p.GroupCode == input.Group);
            }

            if (input.WorkHours > 0)
                queryFilter = queryFilter.And(p => p.WorkHour == input.WorkHours);

            if (input.DateFrom != null && input.DateTo != null)
            {
                var dateFrom = DateTime.ParseExact(input.DateFrom, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
                var dateTo = DateTime.ParseExact(input.DateTo, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);

                queryFilter = queryFilter.And(p => p.ProductionDate >= dateFrom && p.ProductionDate <= dateTo);
            }

            var dbResult = _exePlantProductionEntryVerificationRepo.Get(queryFilter);
            return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult);
        }

        public List<PlanningPlantIndividualCapacityByReferenceDTO> GetPlanningPlantIndividualCapacityAverageByReference(
            List<PlanningPlantIndividualCapacityByReferenceDTO> icReferences)
        {
            var list = new List<PlanningPlantIndividualCapacityByReferenceDTO>();
            var queryFilter = PredicateHelper.True<PlanPlantIndividualCapacityByReferenceView>();

            var avg = from r in icReferences
                      group r by
                          new
                          {
                              r.EmployeeID,
                              r.EmployeeNumber,
                              r.LocationCode,
                              r.UnitCode,
                              r.ProcessGroup,
                              r.GroupCode,
                              r.WorkHours
                          }
                          into grouped
                          select new
                          {
                              GroupCode = grouped.Key.GroupCode,
                              UnitCode = grouped.Key.UnitCode,
                              LocationCode = grouped.Key.LocationCode,
                              ProcessGroup = grouped.Key.ProcessGroup,
                              WorkHours = grouped.Key.WorkHours,
                              MinimumValue = grouped.Average(x => x.MinimumValue),
                              MaximumValue = grouped.Average(x => x.MaximumValue),
                              AverageValue = grouped.Average(x => x.AverageValue),
                              MedianValue = grouped.Average(x => x.MedianValue),
                              LatestValue = grouped.Where(x => x.ProductionDate == grouped.Max(y => y.ProductionDate)).Sum(x => x.LatestValue),
                              EmployeeID = grouped.Key.EmployeeID,
                              EmployeeNumber = grouped.Key.EmployeeNumber,
                              HoursCapacity3 = grouped.Average(x => x.HoursCapacity3),
                              HoursCapacity5 = grouped.Average(x => x.HoursCapacity5),
                              HoursCapacity6 = grouped.Average(x => x.HoursCapacity6),
                              HoursCapacity7 = grouped.Average(x => x.HoursCapacity7),
                              HoursCapacity8 = grouped.Average(x => x.HoursCapacity8),
                              HoursCapacity9 = grouped.Average(x => x.HoursCapacity9),
                              HoursCapacity10 = grouped.Average(x => x.HoursCapacity10)
                          };

            foreach (var average in avg)
            {
                //var newList = new List<PlanningPlantIndividualCapacityByReferenceDTO>();
                var item = new PlanningPlantIndividualCapacityByReferenceDTO();
                item.GroupCode = average.GroupCode;
                item.UnitCode = average.UnitCode;
                item.LocationCode = average.LocationCode;
                item.ProcessGroup = average.ProcessGroup;
                item.WorkHours = average.WorkHours;
                item.MinimumValue = (int?)average.MinimumValue;
                item.MaximumValue = (int?)average.MaximumValue;
                item.AverageValue = (int?)average.AverageValue;
                item.MedianValue = (int?)average.MedianValue;
                item.LatestValue = (int?)average.LatestValue;
                item.EmployeeID = average.EmployeeID;
                item.EmployeeNumber = average.EmployeeNumber;
                item.HoursCapacity3 = average.HoursCapacity3;
                item.HoursCapacity5 = average.HoursCapacity5;
                item.HoursCapacity6 = average.HoursCapacity6;
                item.HoursCapacity7 = average.HoursCapacity7;
                item.HoursCapacity8 = average.HoursCapacity8;
                item.HoursCapacity9 = average.HoursCapacity9;
                item.HoursCapacity10 = average.HoursCapacity10;

                list.Add(item);
                //newList.Add(average);
            }

            foreach (var item in list)
            {
                var name = _MstPlantEmpJobsDataAcv.Get(c => c.EmployeeID == item.EmployeeID).ToList();
                var a = name.FirstOrDefault(c => c.EmployeeID == item.EmployeeID);
                if (a != null)
                {
                    item.EmployeeName = a.EmployeeName;
                }

            }

            return list;

        }

        public List<PlanningPlantIndividualCapacityByReferenceDTO> GetPlanningPlantIndividualCapacityByReference(GetPlanningPlantIndividualCapacityByReferenceInput input)
        {
            //var queryFilter = PredicateHelper.True<PlanPlantIndividualCapacityByReferenceView>();//hakim updte by abud yang ini sekarang ga di pake see http://tp.voxteneo.co.id/entity/3747 for more detail

            //if (!string.IsNullOrEmpty(input.Location))
            //{
            //    queryFilter = queryFilter.And(p => p.LocationCode == input.Location);
            //}

            //if (!string.IsNullOrEmpty(input.Unit))
            //{
            //    queryFilter = queryFilter.And(p => p.UnitCode == input.Unit);
            //}

            //if (!string.IsNullOrEmpty(input.BrandGroupCode))
            //{
            //    var dbResultMstGenBrand = _masterDataBll.GetBrands(new GetBrandInput
            //    {
            //        BrandGroupCode = input.BrandGroupCode
            //    }).Select(c => c.BrandCode);

            //    if (dbResultMstGenBrand.Any())
            //    {
            //        queryFilter = queryFilter.And(p => dbResultMstGenBrand.Contains(p.BrandCode));

            //    }

            //}

            //if (!string.IsNullOrEmpty(input.Process))
            //{
            //    queryFilter = queryFilter.And(p => p.ProcessGroup == input.Process);
            //}

            //if (!string.IsNullOrEmpty(input.Group))
            //{
            //    queryFilter = queryFilter.And(p => p.GroupCode == input.Group);
            //}

            //queryFilter = queryFilter.And(p => p.WorkHour == input.WorkHours);
            //queryFilter = queryFilter.And(p => p.LatestValue != null || p.LatestValue != 0);

            var dateFrom = DateTime.ParseExact(input.DateFrom, Constants.DefaultDateOnlyFormat,
                CultureInfo.InvariantCulture);
            var dateTo = DateTime.ParseExact(input.DateTo, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);

            //queryFilter = queryFilter.And(p => p.ProductionDate >= dateFrom && p.ProductionDate <= dateTo);


            //Func<IQueryable<PlanPlantIndividualCapacityByReferenceView>, IOrderedQueryable<PlanPlantIndividualCapacityByReferenceView>> orderByFilter = null;
            //if (!string.IsNullOrEmpty(input.SortExpression) && !string.IsNullOrEmpty(input.SortOrder) && !string.IsNullOrEmpty(input.SortExpression2) && !string.IsNullOrEmpty(input.SortOrder2))
            //{
            //    //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //    var sortCriteria = new Tuple<IEnumerable<string>, string, IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder, new[] { input.SortExpression2 }, input.SortOrder2);
            //    //orderByFilter = sortCriteria.GetOrderByFunc<PlanPlantIndividualCapacityByReferenceView>();
            //}

            //var dbResult = _planningPlantIndividualCapacityByReference.Get(queryFilter, orderByFilter).OrderBy(c => c.EmployeeNumber);

            var dbResult = _sqlSPRepo.GetPlanPlantIndividualCapacityByReference(input.Location, input.Unit, input.BrandGroupCode, input.Process, input.Group, input.WorkHours, dateFrom, dateTo).OrderBy(x=>x.EmployeeNumber);

            return Mapper.Map<List<PlanningPlantIndividualCapacityByReferenceDTO>>(dbResult);
        }

        /// <summary>
        /// Inserts the planning plant individual capacity work hour.
        /// </summary>
        /// <param name="ppcwhDto">The PPCWH dto.</param>
        /// <returns></returns>
        public PlanningPlantIndividualCapacityWorkHourDTO SavePlanningPlantIndividualCapacityByReference(PlanningPlantIndividualCapacityWorkHourDTO input, string name)
        {
            var capacityValue = false;
            var dbResult = _planningPlantIndividualCapacityWorkHourRepo.GetByID(input.BrandGroupCode, input.EmployeeID, input.GroupCode, input.UnitCode, input.LocationCode, input.ProcessGroup);
            if (dbResult == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }
            else
            {
                if (input.WorkHours == 3)
                {
                    capacityValue = (dbResult.HoursCapacity3 != input.HoursCapacity) ? true : false;
                    dbResult.HoursCapacity3 = input.HoursCapacity;
                }
                else if (input.WorkHours == 5)
                {
                    capacityValue = (dbResult.HoursCapacity5 != input.HoursCapacity) ? true : false;
                    dbResult.HoursCapacity5 = input.HoursCapacity;
                }
                else if (input.WorkHours == 6)
                {
                    capacityValue = (dbResult.HoursCapacity6 != input.HoursCapacity) ? true : false;
                    dbResult.HoursCapacity6 = input.HoursCapacity;
                }
                else if (input.WorkHours == 7)
                {
                    capacityValue = (dbResult.HoursCapacity7 != input.HoursCapacity) ? true : false;
                    dbResult.HoursCapacity7 = input.HoursCapacity;
                }
                else if (input.WorkHours == 8)
                {
                    capacityValue = (dbResult.HoursCapacity8 != input.HoursCapacity) ? true : false;
                    dbResult.HoursCapacity8 = input.HoursCapacity;
                }
                else if (input.WorkHours == 9)
                {
                    capacityValue = (dbResult.HoursCapacity9 != input.HoursCapacity) ? true : false;
                    dbResult.HoursCapacity9 = input.HoursCapacity;
                }
                else if (input.WorkHours == 10)
                {
                    capacityValue = (dbResult.HoursCapacity10 != input.HoursCapacity) ? true : false;
                    dbResult.HoursCapacity10 = input.HoursCapacity;
                }

                if (capacityValue)
                {
                    dbResult.UpdatedBy = name;
                    dbResult.UpdatedDate = DateTime.Now;
                }

                _planningPlantIndividualCapacityWorkHourRepo.Update(dbResult);
            }
            _uow.SaveChanges();
            return Mapper.Map(dbResult, input);
        }

        public List<DataDailyProductionAchievmentDTO> GetBrandCodeFromReportDailyAchievment(
            GetExePlantProductionEntryVerificationInput input)
        {
            var queryfilter = PredicateHelper.True<ExeReportDailyProductionAchievementView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryfilter = queryfilter.And(c => c.LocationCode == input.LocationCode);
            }
            var dbresult = _exeReportDailyProductionAchievmentRepo.Get(queryfilter);
            var data = Mapper.Map<List<DataDailyProductionAchievmentDTO>>(dbresult);

            return data;

        }

        public List<string> GeteExePlantProductionEntryVerificationWithUnion(
            GetExePlantProductionEntryVerificationInput input)
        {
            //Rntry Verification
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            }

            if (input.Shift.HasValue)
            {
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            }

            if (input.KpsYear.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSYear == input.KpsYear);
            }

            if (input.KpsWeek.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSWeek == input.KpsWeek);
            }
            if (input.ProductionDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate);
            }

            //PlantTpo
            var queryFilterTPO = PredicateHelper.True<ExeTPOProductionEntryVerification>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilterTPO = queryFilterTPO.And(p => p.LocationCode == input.LocationCode);
            }

            if (input.KpsYear.HasValue)
            {
                queryFilterTPO = queryFilterTPO.And(p => p.KPSYear == input.KpsYear);
            }

            if (input.KpsWeek.HasValue)
            {
                queryFilterTPO = queryFilterTPO.And(p => p.KPSWeek == input.KpsWeek);

            }
            if (input.ProductionDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate);
            }

            var brandCodeEv = _exePlantProductionEntryVerificationRepo.Get(queryFilter).Select(ev => ev.BrandCode).Distinct().ToList();
            var brandCodeTpo = _exeTPOProductionEntryVerification.Get(queryFilterTPO).Select(tpo => tpo.BrandCode).Distinct().ToList();

            var dbResultUnion = brandCodeEv.Union(brandCodeTpo);

            return dbResultUnion.ToList();

        }

        #endregion

        #region TPK

        public List<PlantTPKCompositeDTO> GetPlanningPlantTPK(GetPlantTPKsInput input)
        {
            var queryFilter = PredicateHelper.True<PlanPlantTargetProductionKelompok>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandCode == input.BrandCode);

            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            if (input.KPSYear != null)
                queryFilter = queryFilter.And(m => m.KPSYear == input.KPSYear);

            if (input.KPSWeek != null)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.KPSWeek);

            if (input.Shift != null)
                queryFilter = queryFilter.And(m => m.Shift == input.Shift);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<PlanPlantTargetProductionKelompok>();

            var dbResult = _planPlantTargetProductionKelompokRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<PlantTPKCompositeDTO>>(dbResult);

        }

        public PlantTPKByProcessDTO SavePlantTPKByGroup(PlantTPKByProcessDTO plantTPKbyProcess)
        {
            //save wip
            var wipStock = Mapper.Map<WIPStockDTO>(plantTPKbyProcess);
            UpdateWIPStock(wipStock);

            //save tpks
            foreach (var plantTpk in plantTPKbyProcess.PlantTPK)
            {
                UpdatePlanTPK(plantTpk);
            }
            _uow.SaveChanges();

            return plantTPKbyProcess;
        }

        private void UpdatePlanTPK(PlantTPKDTO plantTPK)
        {
            var dbPlanTPK = _planPlantTargetProductionKelompokRepo.GetByID(plantTPK.TPKPlantStartProductionDate, plantTPK.KPSYear,
                plantTPK.KPSWeek, plantTPK.GroupCode, plantTPK.ProcessGroup, plantTPK.UnitCode, plantTPK.LocationCode, plantTPK.BrandCode, plantTPK.Shift);

            if (dbPlanTPK == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            plantTPK.CreatedBy = dbPlanTPK.CreatedBy;
            plantTPK.CreatedDate = dbPlanTPK.CreatedDate;

            //set update time
            plantTPK.UpdatedDate = DateTime.Now;

            Mapper.Map(plantTPK, dbPlanTPK);
            _planPlantTargetProductionKelompokRepo.Update(dbPlanTPK);
        }

        public List<WorkerBrandAssigmentDTO> GetWorkerBrandAssignmentPlanningPlantTpk(GetPlanPlantAllocation filter)
        {
            return Mapper.Map<List<WorkerBrandAssigmentDTO>>(_sqlSPRepo.GetWorkerBrandAssignmentPlanningPlantTpk(filter));
        }

        public PlantTPKCalculateDTO CalculatePlantTPK(CalculatePlantTPKInput InputPlantTPK)
        {

            var UomEblek = _masterDataBll.GetUOMEblekByBrandCode(InputPlantTPK.BrandCode, InputPlantTPK.LocationCode);
            var TotalBox = InputPlantTPK.TotalPlantTPK.FirstOrDefault();

            var total = new List<TargetManualTPUDTO>();

            // Global Variable
            var currentProcess = "";
            var previousProcess = "";
            var previousProcessWorkHours = new GenericValuePerWeekDTO<float?>();
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
            var TotalDailyManual = new GenericValuePerWeekDTO<float?>();
            int? UOMEblekPacking = 0;
            int? UOMEblekWrapping = 0;

            #region Get Ordered Process from Process Settings
            var mstGenProcessSettingLocation = _processSettingLocationRepo.Get(p => p.LocationCode == InputPlantTPK.LocationCode).FirstOrDefault();
            var mstBrand = _masterDataBll.GetBrand(new GetBrandInput() { BrandCode = InputPlantTPK.BrandCode });
            var brandGroupCode = "";
            if (mstBrand != null)
                brandGroupCode = mstBrand.BrandGroupCode;
            var listProcessSettings = mstGenProcessSettingLocation.MstGenProcessSettings.Where(p => p.BrandGroupCode == brandGroupCode).OrderByDescending(p => p.MstGenProcess.ProcessOrder).Select(p => p.ProcessGroup).Distinct().ToList();
            #endregion

            int groupIndex;
            bool groupEmptyAllocation;

            var weekDate = _masterDataBll.GetDateByWeek(InputPlantTPK.KPSYear, InputPlantTPK.KPSWeek);

            #region Loop By Process

            foreach (var TPOTPKByProcess in InputPlantTPK.ListPlantTPK.OrderBy(p => listProcessSettings.IndexOf(p.ProcessGroup)))
            {
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
                var CurrentProcessWorkHours = TPOTPKByProcess.PlantTPK.FirstOrDefault();
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
                    #region Formula Stamping
                    #region Get Previous Greatest Process Work Hour By Process
                    var PreviousProcessWorkHoursStickStamping = InputPlantTPK.ListPlantTPK.Where(c => c.ProcessGroup.ToUpper() == listProcessSettings.FirstOrDefault()).Select(c => c.PlantTPK).FirstOrDefault().FirstOrDefault();
                    if (PreviousProcessWorkHoursStickStamping != null)
                    {
                        previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
                        {
                            Value1 = PreviousProcessWorkHoursStickStamping.ProcessWorkHours1,
                            Value2 = PreviousProcessWorkHoursStickStamping.ProcessWorkHours2,
                            Value3 = PreviousProcessWorkHoursStickStamping.ProcessWorkHours3,
                            Value4 = PreviousProcessWorkHoursStickStamping.ProcessWorkHours4,
                            Value5 = PreviousProcessWorkHoursStickStamping.ProcessWorkHours5,
                            Value6 = PreviousProcessWorkHoursStickStamping.ProcessWorkHours6,
                            Value7 = PreviousProcessWorkHoursStickStamping.ProcessWorkHours7
                        };
                    }
                    #endregion

                    int index = 0;
                    foreach (var PlantTPKByGroup in TPOTPKByProcess.PlantTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        PlantTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        PlantTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        PlantTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        PlantTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        PlantTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        PlantTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        PlantTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroupPlant(PlantTPKByGroup, weekDate); // pengecekan inactive group
                        #region Group check
                        if ((!c.groupStatus && weekDate[0] >= c.updateDate) || (currentProcessWorkHours.Value1 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult1 = 0;
                        }
                        if ((!c.groupStatus && weekDate[1] >= c.updateDate) || (currentProcessWorkHours.Value2 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult2 = 0;
                        }
                        if ((!c.groupStatus && weekDate[2] >= c.updateDate) || (currentProcessWorkHours.Value3 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult3 = 0;
                        }
                        if ((!c.groupStatus && weekDate[3] >= c.updateDate) || (currentProcessWorkHours.Value4 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult4 = 0;
                        }
                        if ((!c.groupStatus && weekDate[4] >= c.updateDate) || (currentProcessWorkHours.Value5 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult5 = 0;
                        }
                        if ((!c.groupStatus && weekDate[5] >= c.updateDate) || (currentProcessWorkHours.Value6 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult6 = 0;
                        }
                        if ((!c.groupStatus && weekDate[6] >= c.updateDate) || (currentProcessWorkHours.Value7 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult7 = 0;
                        }
                        #endregion

                        if (groupEmptyAllocation)
                        {
                            if (PlantTPKByGroup.WorkerAllocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        foreach (var PlantTPKByGroup in TPOTPKByProcess.PlantTPK)
                        {
                            #region Calculate Target System
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[0] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                PlantTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)PlantTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * TotalBox.TargetSystem1 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[1] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                PlantTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)PlantTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * TotalBox.TargetSystem2 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[2] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                PlantTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)PlantTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * TotalBox.TargetSystem3 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[3] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                PlantTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)PlantTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * TotalBox.TargetSystem4 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[4] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                PlantTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)PlantTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * TotalBox.TargetSystem5 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[5] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                PlantTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)PlantTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * TotalBox.TargetSystem6 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[6] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                PlantTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)PlantTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * TotalBox.TargetSystem7 * brand.StickPerBox / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            #endregion

                            PlantTPKByGroup.TotalTargetSystem = PlantTPKByGroup.TargetSystem1 + PlantTPKByGroup.TargetSystem2 +
                                                          PlantTPKByGroup.TargetSystem3 + PlantTPKByGroup.TargetSystem4 +
                                                          PlantTPKByGroup.TargetSystem5 + PlantTPKByGroup.TargetSystem6 +
                                                          PlantTPKByGroup.TargetSystem7;

                            PlantTPKByGroup.TargetManual1 = PlantTPKByGroup.TargetSystem1;
                            PlantTPKByGroup.TargetManual2 = PlantTPKByGroup.TargetSystem2;
                            PlantTPKByGroup.TargetManual3 = PlantTPKByGroup.TargetSystem3;
                            PlantTPKByGroup.TargetManual4 = PlantTPKByGroup.TargetSystem4;
                            PlantTPKByGroup.TargetManual5 = PlantTPKByGroup.TargetSystem5;
                            PlantTPKByGroup.TargetManual6 = PlantTPKByGroup.TargetSystem6;
                            PlantTPKByGroup.TargetManual7 = PlantTPKByGroup.TargetSystem7;

                            PlantTPKByGroup.TotalTargetManual = PlantTPKByGroup.TargetManual1 + PlantTPKByGroup.TargetManual2 +
                                                          PlantTPKByGroup.TargetManual3 + PlantTPKByGroup.TargetManual4 +
                                                          PlantTPKByGroup.TargetManual5 + PlantTPKByGroup.TargetManual6 +
                                                          PlantTPKByGroup.TargetManual7;

                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlantTPK = fillEmptyAllocationPlantTpk(TPOTPKByProcess.PlantTPK);
                    }

                    // Variables for Next Process
                    TotalDailyTargetSystemWrappingStamping = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualWrappingStamping = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        var ratio = brand.StickPerBox / convertUOM;
                        // System
                        if (((TotalBox.TargetSystem1 * ratio) != TotalDailyTargetSystemWrappingStamping.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 + ((TotalBox.TargetSystem1 * ratio) - TotalDailyTargetSystemWrappingStamping.Value1);
                            TotalDailyTargetSystemWrappingStamping.Value1 = TotalBox.TargetSystem1 * ratio;
                        }
                        if (((TotalBox.TargetSystem2 * ratio) != TotalDailyTargetSystemWrappingStamping.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 + ((TotalBox.TargetSystem2 * ratio) - TotalDailyTargetSystemWrappingStamping.Value2);
                            TotalDailyTargetSystemWrappingStamping.Value2 = TotalBox.TargetSystem2 * ratio;
                        }
                        if (((TotalBox.TargetSystem3 * ratio) != TotalDailyTargetSystemWrappingStamping.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 + ((TotalBox.TargetSystem3 * ratio) - TotalDailyTargetSystemWrappingStamping.Value3);
                            TotalDailyTargetSystemWrappingStamping.Value3 = TotalBox.TargetSystem3 * ratio;
                        }
                        if (((TotalBox.TargetSystem4 * ratio) != TotalDailyTargetSystemWrappingStamping.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 + ((TotalBox.TargetSystem4 * ratio) - TotalDailyTargetSystemWrappingStamping.Value4);
                            TotalDailyTargetSystemWrappingStamping.Value4 = TotalBox.TargetSystem4 * ratio;
                        }
                        if (((TotalBox.TargetSystem5 * ratio) != TotalDailyTargetSystemWrappingStamping.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 + ((TotalBox.TargetSystem5 * ratio) - TotalDailyTargetSystemWrappingStamping.Value5);
                            TotalDailyTargetSystemWrappingStamping.Value5 = TotalBox.TargetSystem5 * ratio;
                        }
                        if (((TotalBox.TargetSystem6 * ratio) != TotalDailyTargetSystemWrappingStamping.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 + ((TotalBox.TargetSystem6 * ratio) - TotalDailyTargetSystemWrappingStamping.Value6);
                            TotalDailyTargetSystemWrappingStamping.Value6 = TotalBox.TargetSystem6 * ratio;
                        }
                        if (((TotalBox.TargetSystem7 * ratio) != TotalDailyTargetSystemWrappingStamping.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 + ((TotalBox.TargetSystem7 * ratio) - TotalDailyTargetSystemWrappingStamping.Value7);
                            TotalDailyTargetSystemWrappingStamping.Value7 = TotalBox.TargetSystem7 * ratio;
                        }

                        // Manual
                        if (((TotalBox.TargetManual1 * ratio) != TotalDailyTargetManualWrappingStamping.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 + ((TotalBox.TargetManual1 * ratio) - TotalDailyTargetManualWrappingStamping.Value1);
                            TotalDailyTargetManualWrappingStamping.Value1 = TotalBox.TargetManual1 * ratio;
                        }
                        if (((TotalBox.TargetManual2 * ratio) != TotalDailyTargetManualWrappingStamping.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 + ((TotalBox.TargetManual2 * ratio) - TotalDailyTargetManualWrappingStamping.Value2);
                            TotalDailyTargetManualWrappingStamping.Value2 = TotalBox.TargetManual2 * ratio;
                        }
                        if (((TotalBox.TargetManual3 * ratio) != TotalDailyTargetManualWrappingStamping.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 + ((TotalBox.TargetManual3 * ratio) - TotalDailyTargetManualWrappingStamping.Value3);
                            TotalDailyTargetManualWrappingStamping.Value3 = TotalBox.TargetManual3 * ratio;
                        }
                        if (((TotalBox.TargetManual4 * ratio) != TotalDailyTargetManualWrappingStamping.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 + ((TotalBox.TargetManual4 * ratio) - TotalDailyTargetManualWrappingStamping.Value4);
                            TotalDailyTargetManualWrappingStamping.Value4 = TotalBox.TargetManual4 * ratio;
                        }
                        if (((TotalBox.TargetManual5 * ratio) != TotalDailyTargetManualWrappingStamping.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 + ((TotalBox.TargetManual5 * ratio) - TotalDailyTargetManualWrappingStamping.Value5);
                            TotalDailyTargetManualWrappingStamping.Value5 = TotalBox.TargetManual5 * ratio;
                        }
                        if (((TotalBox.TargetManual6 * ratio) != TotalDailyTargetManualWrappingStamping.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 + ((TotalBox.TargetManual6 * ratio) - TotalDailyTargetManualWrappingStamping.Value6);
                            TotalDailyTargetManualWrappingStamping.Value6 = TotalBox.TargetManual6 * ratio;
                        }
                        if (((TotalBox.TargetManual7 * ratio) != TotalDailyTargetManualWrappingStamping.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 + ((TotalBox.TargetManual7 * ratio) - TotalDailyTargetManualWrappingStamping.Value7);
                            TotalDailyTargetManualWrappingStamping.Value7 = TotalBox.TargetManual7 * ratio;
                        }
                        #endregion
                    }

                    UOMEblekWrapping = convertUOM;

                    #endregion
                }
                else if (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Packing.ToString().ToUpper())
                {
                    #region Formula Packing
                    #region Get Previous Greatest Process Work Hour By Process
                    var PreviousProcessWorkHoursStickPacking = InputPlantTPK.ListPlantTPK.Where(c => c.ProcessGroup.ToUpper() == listProcessSettings.FirstOrDefault()).Select(c => c.PlantTPK).FirstOrDefault().FirstOrDefault();
                    if (PreviousProcessWorkHoursStickPacking != null)
                    {
                        previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
                        {
                            Value1 = PreviousProcessWorkHoursStickPacking.ProcessWorkHours1,
                            Value2 = PreviousProcessWorkHoursStickPacking.ProcessWorkHours2,
                            Value3 = PreviousProcessWorkHoursStickPacking.ProcessWorkHours3,
                            Value4 = PreviousProcessWorkHoursStickPacking.ProcessWorkHours4,
                            Value5 = PreviousProcessWorkHoursStickPacking.ProcessWorkHours5,
                            Value6 = PreviousProcessWorkHoursStickPacking.ProcessWorkHours6,
                            Value7 = PreviousProcessWorkHoursStickPacking.ProcessWorkHours7
                        };
                    }
                    #endregion

                    int index = 0;
                    foreach (var PlantTPKByGroup in TPOTPKByProcess.PlantTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        PlantTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        PlantTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        PlantTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        PlantTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        PlantTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        PlantTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        PlantTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroupPlant(PlantTPKByGroup, weekDate); // pengecekan inactive group
                        #region Group check
                        if ((!c.groupStatus && weekDate[0] >= c.updateDate) || (currentProcessWorkHours.Value1 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult1 = 0;
                        }
                        if ((!c.groupStatus && weekDate[1] >= c.updateDate) || (currentProcessWorkHours.Value2 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult2 = 0;
                        }
                        if ((!c.groupStatus && weekDate[2] >= c.updateDate) || (currentProcessWorkHours.Value3 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult3 = 0;
                        }
                        if ((!c.groupStatus && weekDate[3] >= c.updateDate) || (currentProcessWorkHours.Value4 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult4 = 0;
                        }
                        if ((!c.groupStatus && weekDate[4] >= c.updateDate) || (currentProcessWorkHours.Value5 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult5 = 0;
                        }
                        if ((!c.groupStatus && weekDate[5] >= c.updateDate) || (currentProcessWorkHours.Value6 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult6 = 0;
                        }
                        if ((!c.groupStatus && weekDate[6] >= c.updateDate) || (currentProcessWorkHours.Value7 == 0))
                        {
                            PlantTPKByGroup.DailyWHWeightedResult7 = 0;
                        }
                        #endregion

                        if (groupEmptyAllocation)
                        {
                            if (PlantTPKByGroup.WorkerAllocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                        {
                            #region Assign target
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[0] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * (TotalDailyTargetSystemWrappingStamping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[1] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * (TotalDailyTargetSystemWrappingStamping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[2] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * (TotalDailyTargetSystemWrappingStamping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[3] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * (TotalDailyTargetSystemWrappingStamping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[4] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * (TotalDailyTargetSystemWrappingStamping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[5] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * (TotalDailyTargetSystemWrappingStamping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[6] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * (TotalDailyTargetSystemWrappingStamping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            #endregion

                            TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TargetManual1 = TPOTPKByGroup.TargetSystem1;
                            TPOTPKByGroup.TargetManual2 = TPOTPKByGroup.TargetSystem2;
                            TPOTPKByGroup.TargetManual3 = TPOTPKByGroup.TargetSystem3;
                            TPOTPKByGroup.TargetManual4 = TPOTPKByGroup.TargetSystem4;
                            TPOTPKByGroup.TargetManual5 = TPOTPKByGroup.TargetSystem5;
                            TPOTPKByGroup.TargetManual6 = TPOTPKByGroup.TargetSystem6;
                            TPOTPKByGroup.TargetManual7 = TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlantTPK = fillEmptyAllocationPlantTpk(TPOTPKByProcess.PlantTPK);
                    }

                    // Variables for Next Process
                    TotalDailyTargetSystemPacking = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualPacking = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        // System
                        if ((TotalDailyTargetSystemPacking.Value1 != (TotalDailyTargetSystemWrappingStamping.Value1 + (TPOTPKByProcess.VarianceWIP1 / convertUOM)) && (currentProcessWorkHours.Value1 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 + ((TotalDailyTargetSystemWrappingStamping.Value1 + (TPOTPKByProcess.VarianceWIP1 / convertUOM)) - TotalDailyTargetSystemPacking.Value1);
                            TotalDailyTargetSystemPacking.Value1 = (TotalDailyTargetSystemWrappingStamping.Value1 + (TPOTPKByProcess.VarianceWIP1 / convertUOM));
                        }
                        if ((TotalDailyTargetSystemPacking.Value2 != (TotalDailyTargetSystemWrappingStamping.Value2 + (TPOTPKByProcess.VarianceWIP2 / convertUOM)) && (currentProcessWorkHours.Value2 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 + ((TotalDailyTargetSystemWrappingStamping.Value2 + (TPOTPKByProcess.VarianceWIP2 / convertUOM)) - TotalDailyTargetSystemPacking.Value2);
                            TotalDailyTargetSystemPacking.Value2 = (TotalDailyTargetSystemWrappingStamping.Value2 + (TPOTPKByProcess.VarianceWIP2 / convertUOM));
                        }
                        if ((TotalDailyTargetSystemPacking.Value3 != (TotalDailyTargetSystemWrappingStamping.Value3 + (TPOTPKByProcess.VarianceWIP3 / convertUOM)) && (currentProcessWorkHours.Value3 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 + ((TotalDailyTargetSystemWrappingStamping.Value3 + (TPOTPKByProcess.VarianceWIP3 / convertUOM)) - TotalDailyTargetSystemPacking.Value3);
                            TotalDailyTargetSystemPacking.Value3 = (TotalDailyTargetSystemWrappingStamping.Value3 + (TPOTPKByProcess.VarianceWIP3 / convertUOM));
                        }
                        if ((TotalDailyTargetSystemPacking.Value4 != (TotalDailyTargetSystemWrappingStamping.Value4 + (TPOTPKByProcess.VarianceWIP4 / convertUOM)) && (currentProcessWorkHours.Value4 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 + ((TotalDailyTargetSystemWrappingStamping.Value4 + (TPOTPKByProcess.VarianceWIP4 / convertUOM)) - TotalDailyTargetSystemPacking.Value4);
                            TotalDailyTargetSystemPacking.Value4 = (TotalDailyTargetSystemWrappingStamping.Value4 + (TPOTPKByProcess.VarianceWIP4 / convertUOM));
                        }
                        if ((TotalDailyTargetSystemPacking.Value5 != (TotalDailyTargetSystemWrappingStamping.Value5 + (TPOTPKByProcess.VarianceWIP5 / convertUOM)) && (currentProcessWorkHours.Value5 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 + ((TotalDailyTargetSystemWrappingStamping.Value5 + (TPOTPKByProcess.VarianceWIP5 / convertUOM)) - TotalDailyTargetSystemPacking.Value5);
                            TotalDailyTargetSystemPacking.Value5 = (TotalDailyTargetSystemWrappingStamping.Value5 + (TPOTPKByProcess.VarianceWIP5 / convertUOM));
                        }
                        if ((TotalDailyTargetSystemPacking.Value6 != (TotalDailyTargetSystemWrappingStamping.Value6 + (TPOTPKByProcess.VarianceWIP6 / convertUOM)) && (currentProcessWorkHours.Value6 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 + ((TotalDailyTargetSystemWrappingStamping.Value6 + (TPOTPKByProcess.VarianceWIP6 / convertUOM)) - TotalDailyTargetSystemPacking.Value6);
                            TotalDailyTargetSystemPacking.Value6 = (TotalDailyTargetSystemWrappingStamping.Value6 + (TPOTPKByProcess.VarianceWIP6 / convertUOM));
                        }
                        if ((TotalDailyTargetSystemPacking.Value7 != (TotalDailyTargetSystemWrappingStamping.Value7 + (TPOTPKByProcess.VarianceWIP7 / convertUOM)) && (currentProcessWorkHours.Value7 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 + ((TotalDailyTargetSystemWrappingStamping.Value7 + (TPOTPKByProcess.VarianceWIP7 / convertUOM)) - TotalDailyTargetSystemPacking.Value7);
                            TotalDailyTargetSystemPacking.Value7 = (TotalDailyTargetSystemWrappingStamping.Value7 + (TPOTPKByProcess.VarianceWIP7 / convertUOM));
                        }

                        // Manual
                        if ((TotalDailyTargetManualPacking.Value1 != (TotalDailyTargetManualWrappingStamping.Value1 + (TPOTPKByProcess.VarianceWIP1 / convertUOM)) && (currentProcessWorkHours.Value1 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 + ((TotalDailyTargetManualWrappingStamping.Value1 + (TPOTPKByProcess.VarianceWIP1 / convertUOM)) - TotalDailyTargetManualPacking.Value1);
                            TotalDailyTargetManualPacking.Value1 = (TotalDailyTargetManualWrappingStamping.Value1 + (TPOTPKByProcess.VarianceWIP1 / convertUOM));
                        }
                        if ((TotalDailyTargetManualPacking.Value2 != (TotalDailyTargetManualWrappingStamping.Value2 + (TPOTPKByProcess.VarianceWIP2 / convertUOM)) && (currentProcessWorkHours.Value2 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 + ((TotalDailyTargetManualWrappingStamping.Value2 + (TPOTPKByProcess.VarianceWIP2 / convertUOM)) - TotalDailyTargetManualPacking.Value2);
                            TotalDailyTargetManualPacking.Value2 = (TotalDailyTargetManualWrappingStamping.Value2 + (TPOTPKByProcess.VarianceWIP2 / convertUOM));
                        }
                        if ((TotalDailyTargetManualPacking.Value3 != (TotalDailyTargetManualWrappingStamping.Value3 + (TPOTPKByProcess.VarianceWIP3 / convertUOM)) && (currentProcessWorkHours.Value3 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 + ((TotalDailyTargetManualWrappingStamping.Value3 + (TPOTPKByProcess.VarianceWIP3 / convertUOM)) - TotalDailyTargetManualPacking.Value3);
                            TotalDailyTargetManualPacking.Value3 = (TotalDailyTargetManualWrappingStamping.Value3 + (TPOTPKByProcess.VarianceWIP3 / convertUOM));
                        }
                        if ((TotalDailyTargetManualPacking.Value4 != (TotalDailyTargetManualWrappingStamping.Value4 + (TPOTPKByProcess.VarianceWIP4 / convertUOM)) && (currentProcessWorkHours.Value4 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 + ((TotalDailyTargetManualWrappingStamping.Value4 + (TPOTPKByProcess.VarianceWIP4 / convertUOM)) - TotalDailyTargetManualPacking.Value4);
                            TotalDailyTargetManualPacking.Value4 = (TotalDailyTargetManualWrappingStamping.Value4 + (TPOTPKByProcess.VarianceWIP4 / convertUOM));
                        }
                        if ((TotalDailyTargetManualPacking.Value5 != (TotalDailyTargetManualWrappingStamping.Value5 + (TPOTPKByProcess.VarianceWIP5 / convertUOM)) && (currentProcessWorkHours.Value5 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 + ((TotalDailyTargetManualWrappingStamping.Value5 + (TPOTPKByProcess.VarianceWIP5 / convertUOM)) - TotalDailyTargetManualPacking.Value5);
                            TotalDailyTargetManualPacking.Value5 = (TotalDailyTargetManualWrappingStamping.Value5 + (TPOTPKByProcess.VarianceWIP5 / convertUOM));
                        }
                        if ((TotalDailyTargetManualPacking.Value6 != (TotalDailyTargetManualWrappingStamping.Value6 + (TPOTPKByProcess.VarianceWIP6 / convertUOM)) && (currentProcessWorkHours.Value6 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 + ((TotalDailyTargetManualWrappingStamping.Value6 + (TPOTPKByProcess.VarianceWIP6 / convertUOM)) - TotalDailyTargetManualPacking.Value6);
                            TotalDailyTargetManualPacking.Value6 = (TotalDailyTargetManualWrappingStamping.Value6 + (TPOTPKByProcess.VarianceWIP6 / convertUOM));
                        }
                        if ((TotalDailyTargetManualPacking.Value7 != (TotalDailyTargetManualWrappingStamping.Value7 + (TPOTPKByProcess.VarianceWIP7 / convertUOM)) && (currentProcessWorkHours.Value7 > 0)))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 + ((TotalDailyTargetManualWrappingStamping.Value7 + (TPOTPKByProcess.VarianceWIP7 / convertUOM)) - TotalDailyTargetManualPacking.Value7);
                            TotalDailyTargetManualPacking.Value7 = (TotalDailyTargetManualWrappingStamping.Value7 + (TPOTPKByProcess.VarianceWIP7 / convertUOM));
                        }
                        #endregion
                    }

                    UOMEblekPacking = convertUOM;

                    #endregion
                }
                else if (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Stickwrapping.ToString().ToUpper())
                {
                    #region Formula Stickwrapping
                    #region Get Previous Greatest Process Work Hour By Process
                    var PreviousProcessWorkHoursStickwrapping = InputPlantTPK.ListPlantTPK.Where(c => c.ProcessGroup.ToUpper() == listProcessSettings.FirstOrDefault()).Select(c => c.PlantTPK).FirstOrDefault().FirstOrDefault();
                    if (PreviousProcessWorkHoursStickwrapping != null)
                    {
                        previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
                        {
                            Value1 = PreviousProcessWorkHoursStickwrapping.ProcessWorkHours1,
                            Value2 = PreviousProcessWorkHoursStickwrapping.ProcessWorkHours2,
                            Value3 = PreviousProcessWorkHoursStickwrapping.ProcessWorkHours3,
                            Value4 = PreviousProcessWorkHoursStickwrapping.ProcessWorkHours4,
                            Value5 = PreviousProcessWorkHoursStickwrapping.ProcessWorkHours5,
                            Value6 = PreviousProcessWorkHoursStickwrapping.ProcessWorkHours6,
                            Value7 = PreviousProcessWorkHoursStickwrapping.ProcessWorkHours7
                        };
                    }
                    #endregion

                    int index = 0;
                    foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        TPOTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        TPOTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        TPOTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        TPOTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        TPOTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        TPOTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        TPOTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroupPlant(TPOTPKByGroup, weekDate); // pengecekan inactive group
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
                            if (TPOTPKByGroup.WorkerAllocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                        {
                            #region target system
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[0] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[1] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[2] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[3] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[4] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[5] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[6] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            #endregion

                            TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TargetManual1 = TPOTPKByGroup.TargetSystem1;
                            TPOTPKByGroup.TargetManual2 = TPOTPKByGroup.TargetSystem2;
                            TPOTPKByGroup.TargetManual3 = TPOTPKByGroup.TargetSystem3;
                            TPOTPKByGroup.TargetManual4 = TPOTPKByGroup.TargetSystem4;
                            TPOTPKByGroup.TargetManual5 = TPOTPKByGroup.TargetSystem5;
                            TPOTPKByGroup.TargetManual6 = TPOTPKByGroup.TargetSystem6;
                            TPOTPKByGroup.TargetManual7 = TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                         TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                         TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                         TPOTPKByGroup.TargetManual7;

                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlantTPK = fillEmptyAllocationPlantTpk(TPOTPKByProcess.PlantTPK);
                    }

                    // Variables for Next Process
                    TotalDailyTargetSystemStickWrapping = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualStickWrapping = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        // System
                        if ((TotalDailyTargetSystemStickWrapping.Value1 != TotalDailyTargetSystemPacking.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 + (TotalDailyTargetSystemPacking.Value1 - TotalDailyTargetSystemStickWrapping.Value1);
                            TotalDailyTargetSystemStickWrapping.Value1 = TotalDailyTargetSystemPacking.Value1;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value2 != TotalDailyTargetSystemPacking.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 + (TotalDailyTargetSystemPacking.Value2 - TotalDailyTargetSystemStickWrapping.Value2);
                            TotalDailyTargetSystemStickWrapping.Value2 = TotalDailyTargetSystemPacking.Value2;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value3 != TotalDailyTargetSystemPacking.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 + (TotalDailyTargetSystemPacking.Value3 - TotalDailyTargetSystemStickWrapping.Value3);
                            TotalDailyTargetSystemStickWrapping.Value3 = TotalDailyTargetSystemPacking.Value3;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value4 != TotalDailyTargetSystemPacking.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 + (TotalDailyTargetSystemPacking.Value4 - TotalDailyTargetSystemStickWrapping.Value4);
                            TotalDailyTargetSystemStickWrapping.Value4 = TotalDailyTargetSystemPacking.Value4;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value5 != TotalDailyTargetSystemPacking.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 + (TotalDailyTargetSystemPacking.Value5 - TotalDailyTargetSystemStickWrapping.Value5);
                            TotalDailyTargetSystemStickWrapping.Value5 = TotalDailyTargetSystemPacking.Value5;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value6 != TotalDailyTargetSystemPacking.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 + (TotalDailyTargetSystemPacking.Value6 - TotalDailyTargetSystemStickWrapping.Value6);
                            TotalDailyTargetSystemStickWrapping.Value6 = TotalDailyTargetSystemPacking.Value6;
                        }
                        if ((TotalDailyTargetSystemStickWrapping.Value7 != TotalDailyTargetSystemPacking.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 + (TotalDailyTargetSystemPacking.Value7 - TotalDailyTargetSystemStickWrapping.Value7);
                            TotalDailyTargetSystemStickWrapping.Value7 = TotalDailyTargetSystemPacking.Value7;
                        }
                        // Manual
                        if ((TotalDailyTargetManualStickWrapping.Value1 != TotalDailyTargetManualPacking.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 + (TotalDailyTargetManualPacking.Value1 - TotalDailyTargetManualStickWrapping.Value1);
                            TotalDailyTargetManualStickWrapping.Value1 = TotalDailyTargetManualPacking.Value1;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value2 != TotalDailyTargetManualPacking.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 + (TotalDailyTargetManualPacking.Value2 - TotalDailyTargetManualStickWrapping.Value2);
                            TotalDailyTargetManualStickWrapping.Value2 = TotalDailyTargetManualPacking.Value2;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value3 != TotalDailyTargetManualPacking.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 + (TotalDailyTargetManualPacking.Value3 - TotalDailyTargetManualStickWrapping.Value3);
                            TotalDailyTargetManualStickWrapping.Value3 = TotalDailyTargetManualPacking.Value3;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value4 != TotalDailyTargetManualPacking.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 + (TotalDailyTargetManualPacking.Value4 - TotalDailyTargetManualStickWrapping.Value4);
                            TotalDailyTargetManualStickWrapping.Value4 = TotalDailyTargetManualPacking.Value4;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value5 != TotalDailyTargetManualPacking.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 + (TotalDailyTargetManualPacking.Value5 - TotalDailyTargetManualStickWrapping.Value5);
                            TotalDailyTargetManualStickWrapping.Value5 = TotalDailyTargetManualPacking.Value5;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value6 != TotalDailyTargetManualPacking.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 + (TotalDailyTargetManualPacking.Value6 - TotalDailyTargetManualStickWrapping.Value6);
                            TotalDailyTargetManualStickWrapping.Value6 = TotalDailyTargetManualPacking.Value6;
                        }
                        if ((TotalDailyTargetManualStickWrapping.Value7 != TotalDailyTargetManualPacking.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 + (TotalDailyTargetManualPacking.Value7 - TotalDailyTargetManualStickWrapping.Value7);
                            TotalDailyTargetManualStickWrapping.Value7 = TotalDailyTargetManualPacking.Value7;
                        }
                        #endregion
                    }

                    #endregion
                }
                else if (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Cutting.ToString().ToUpper())
                {
                    #region Formula Cutting
                    #region Get Previous Greatest Process Work Hour By Process
                    var PreviousProcessWorkHoursCutting = InputPlantTPK.ListPlantTPK.Where(c => c.ProcessGroup.ToUpper() == listProcessSettings.FirstOrDefault()).Select(c => c.PlantTPK).FirstOrDefault().FirstOrDefault();
                    if (PreviousProcessWorkHoursCutting != null)
                    {
                        previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
                        {
                            Value1 = PreviousProcessWorkHoursCutting.ProcessWorkHours1,
                            Value2 = PreviousProcessWorkHoursCutting.ProcessWorkHours2,
                            Value3 = PreviousProcessWorkHoursCutting.ProcessWorkHours3,
                            Value4 = PreviousProcessWorkHoursCutting.ProcessWorkHours4,
                            Value5 = PreviousProcessWorkHoursCutting.ProcessWorkHours5,
                            Value6 = PreviousProcessWorkHoursCutting.ProcessWorkHours6,
                            Value7 = PreviousProcessWorkHoursCutting.ProcessWorkHours7
                        };
                    }
                    #endregion

                    int index = 0;
                    foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        TPOTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        TPOTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        TPOTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        TPOTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        TPOTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        TPOTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        TPOTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroupPlant(TPOTPKByGroup, weekDate); // pengecekan inactive group
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
                            if (TPOTPKByGroup.WorkerAllocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        if (previousProcess.ToUpper() == Enums.Process.Wrapping.ToString().ToUpper())
                        {
                            #region Previous Process is Wrapping
                            foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                            {
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[0] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    if (TotalDailyTargetSystemStickWrapping.Value1 != null)
                                    {
                                        TPOTPKByGroup.TargetSystem1 =
                                            (float)
                                                Math.Round(
                                                    (Decimal)
                                                        ((float) TotalDailyWeighted.Value1 != 0
                                                            ? (float) TPOTPKByGroup.Target1/
                                                              (float) TotalDailyWeighted.Value1*
                                                              ((TotalDailyTargetSystemStickWrapping.Value1*
                                                                UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1)/
                                                              convertUOM
                                                            : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        TPOTPKByGroup.TargetSystem1 =
                                            (float)
                                                Math.Round(
                                                    (Decimal)
                                                        ((float)TotalDailyWeighted.Value1 != 0
                                                            ? (float)TPOTPKByGroup.Target1 /
                                                              (float)TotalDailyWeighted.Value1 *
                                                              ((TotalDailyTargetSystemWrappingStamping.Value1 *
                                                                UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) /
                                                              convertUOM
                                                            : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[1] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    if (TotalDailyTargetSystemStickWrapping.Value2 != null)
                                    {
                                        TPOTPKByGroup.TargetSystem2 =
                                            (float)
                                                Math.Round(
                                                    (Decimal)
                                                        ((float) TotalDailyWeighted.Value2 != 0
                                                            ? (float) TPOTPKByGroup.Target2/
                                                              (float) TotalDailyWeighted.Value2*
                                                              ((TotalDailyTargetSystemStickWrapping.Value2*
                                                                UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2)/
                                                              convertUOM
                                                            : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        TPOTPKByGroup.TargetSystem2 =
                                            (float)
                                                Math.Round(
                                                    (Decimal)
                                                        ((float)TotalDailyWeighted.Value2 != 0
                                                            ? (float)TPOTPKByGroup.Target2 /
                                                              (float)TotalDailyWeighted.Value2 *
                                                              ((TotalDailyTargetSystemWrappingStamping.Value2 *
                                                                UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) /
                                                              convertUOM
                                                            : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[2] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    if (TotalDailyTargetSystemStickWrapping.Value3 != null)
                                    {
                                        TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemWrappingStamping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    
                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[3] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    if (TotalDailyTargetSystemStickWrapping.Value4 != null)
                                    {
                                        TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemWrappingStamping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    
                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[4] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    if (TotalDailyTargetSystemStickWrapping.Value5 != null)
                                    {
                                        TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemWrappingStamping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    
                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[5] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    if (TotalDailyTargetSystemStickWrapping.Value6 != null)
                                    {
                                        TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemWrappingStamping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    
                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[6] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    if (TotalDailyTargetSystemStickWrapping.Value7 != null)
                                    {
                                        TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemWrappingStamping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    }
                                    
                                }

                                //TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetSystemStickWrapping.Value1 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetSystemStickWrapping.Value2 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemStickWrapping.Value3 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemStickWrapping.Value4 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemStickWrapping.Value5 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemStickWrapping.Value6 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemStickWrapping.Value7 * UOMEblekWrapping) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                                TPOTPKByGroup.TargetManual1 = TPOTPKByGroup.TargetSystem1;
                                TPOTPKByGroup.TargetManual2 = TPOTPKByGroup.TargetSystem2;
                                TPOTPKByGroup.TargetManual3 = TPOTPKByGroup.TargetSystem3;
                                TPOTPKByGroup.TargetManual4 = TPOTPKByGroup.TargetSystem4;
                                TPOTPKByGroup.TargetManual5 = TPOTPKByGroup.TargetSystem5;
                                TPOTPKByGroup.TargetManual6 = TPOTPKByGroup.TargetSystem6;
                                TPOTPKByGroup.TargetManual7 = TPOTPKByGroup.TargetSystem7;

                                TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                            }
                            #endregion
                        }
                        else if (previousProcess.ToUpper() == Enums.Process.Stickwrapping.ToString().ToUpper())
                        {
                            #region Previous Process is Stickwrapping
                            foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                            {
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[0] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * (TotalDailyTargetManualStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[1] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * (TotalDailyTargetManualStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[2] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * (TotalDailyTargetManualStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[3] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * (TotalDailyTargetManualStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[4] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * (TotalDailyTargetManualStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[5] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * (TotalDailyTargetManualStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[6] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * (TotalDailyTargetManualStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                //TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                                //TPOTPKByGroup.TargetManual1 = TPOTPKByGroup.TargetSystem1;
                                //TPOTPKByGroup.TargetManual2 = TPOTPKByGroup.TargetSystem2;
                                //TPOTPKByGroup.TargetManual3 = TPOTPKByGroup.TargetSystem3;
                                //TPOTPKByGroup.TargetManual4 = TPOTPKByGroup.TargetSystem4;
                                //TPOTPKByGroup.TargetManual5 = TPOTPKByGroup.TargetSystem5;
                                //TPOTPKByGroup.TargetManual6 = TPOTPKByGroup.TargetSystem6;
                                //TPOTPKByGroup.TargetManual7 = TPOTPKByGroup.TargetSystem7;

                                TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                            }
                            #endregion
                        }
                        else if (previousProcess.ToUpper() == Enums.Process.Packing.ToString().ToUpper())
                        {
                            #region Previous Process is Packing
                            foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                            {
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[0] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[1] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[2] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[3] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[4] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[5] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                }
                                if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[6] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                                {
                                    TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);
                                    TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM : 0), 0, MidpointRounding.AwayFromZero);

                                }

                                //TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? ((float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? ((float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? ((float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? ((float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? ((float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? ((float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);
                                //TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? ((float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM) : 0), 0, MidpointRounding.AwayFromZero);

                                TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                                //TPOTPKByGroup.TargetManual1 = TPOTPKByGroup.TargetSystem1;
                                //TPOTPKByGroup.TargetManual2 = TPOTPKByGroup.TargetSystem2;
                                //TPOTPKByGroup.TargetManual3 = TPOTPKByGroup.TargetSystem3;
                                //TPOTPKByGroup.TargetManual4 = TPOTPKByGroup.TargetSystem4;
                                //TPOTPKByGroup.TargetManual5 = TPOTPKByGroup.TargetSystem5;
                                //TPOTPKByGroup.TargetManual6 = TPOTPKByGroup.TargetSystem6;
                                //TPOTPKByGroup.TargetManual7 = TPOTPKByGroup.TargetSystem7;

                                TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlantTPK = fillEmptyAllocationPlantTpk(TPOTPKByProcess.PlantTPK);
                    }

                    // Variables for Next Process
                    TotalDailyTargetSystemCutting = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem7)
                    };
                    TotalDailyTargetManualCutting = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        if (previousProcess.ToUpper() == Enums.Process.Wrapping.ToString().ToUpper())
                        {
                            #region Wrapping

                            if (TotalDailyTargetSystemStickWrapping.Value1 != null)
                            {
                                #region Normal

                                // System
                                if ((TotalDailyTargetSystemCutting.Value1 !=
                                     (((TotalDailyTargetSystemStickWrapping.Value1*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP1)/convertUOM)) &&
                                    (currentProcessWorkHours.Value1 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 -
                                        (TotalDailyTargetSystemCutting.Value1 -
                                         ((TotalDailyTargetSystemStickWrapping.Value1*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP1)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value1 = ((TotalDailyTargetSystemStickWrapping.Value1*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP1)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value2 !=
                                     (((TotalDailyTargetSystemStickWrapping.Value2*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP2)/convertUOM)) &&
                                    (currentProcessWorkHours.Value2 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 -
                                        (TotalDailyTargetSystemCutting.Value2 -
                                         ((TotalDailyTargetSystemStickWrapping.Value2*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP2)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value2 = ((TotalDailyTargetSystemStickWrapping.Value2*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP2)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value3 !=
                                     (((TotalDailyTargetSystemStickWrapping.Value3*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP3)/convertUOM)) &&
                                    (currentProcessWorkHours.Value3 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 -
                                        (TotalDailyTargetSystemCutting.Value3 -
                                         ((TotalDailyTargetSystemStickWrapping.Value3*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP3)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value3 = ((TotalDailyTargetSystemStickWrapping.Value3*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP3)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value4 !=
                                     (((TotalDailyTargetSystemStickWrapping.Value4*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP4)/convertUOM)) &&
                                    (currentProcessWorkHours.Value4 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 -
                                        (TotalDailyTargetSystemCutting.Value4 -
                                         ((TotalDailyTargetSystemStickWrapping.Value4*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP4)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value4 = ((TotalDailyTargetSystemStickWrapping.Value4*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP4)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value5 !=
                                     (((TotalDailyTargetSystemStickWrapping.Value5*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP5)/convertUOM)) &&
                                    (currentProcessWorkHours.Value5 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 -
                                        (TotalDailyTargetSystemCutting.Value5 -
                                         ((TotalDailyTargetSystemStickWrapping.Value5*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP5)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value5 = ((TotalDailyTargetSystemStickWrapping.Value5*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP5)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value6 !=
                                     (((TotalDailyTargetSystemStickWrapping.Value6*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP6)/convertUOM)) &&
                                    (currentProcessWorkHours.Value6 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 -
                                        (TotalDailyTargetSystemCutting.Value6 -
                                         ((TotalDailyTargetSystemStickWrapping.Value6*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP6)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value6 = ((TotalDailyTargetSystemStickWrapping.Value6*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP6)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value7 !=
                                     (((TotalDailyTargetSystemStickWrapping.Value7*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP7)/convertUOM)) &&
                                    (currentProcessWorkHours.Value7 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 -
                                        (TotalDailyTargetSystemCutting.Value7 -
                                         ((TotalDailyTargetSystemStickWrapping.Value7*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP7)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value7 = ((TotalDailyTargetSystemStickWrapping.Value7*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP7)/convertUOM;
                                }
                                // Manual
                                if ((TotalDailyTargetManualCutting.Value1 !=
                                     (((TotalDailyTargetManualStickWrapping.Value1*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP1)/convertUOM)) &&
                                    (currentProcessWorkHours.Value1 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 -
                                        (TotalDailyTargetManualCutting.Value1 -
                                         ((TotalDailyTargetManualStickWrapping.Value1*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP1)/convertUOM);
                                    TotalDailyTargetManualCutting.Value1 = ((TotalDailyTargetManualStickWrapping.Value1*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP1)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value2 !=
                                     (((TotalDailyTargetManualStickWrapping.Value2*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP2)/convertUOM)) &&
                                    (currentProcessWorkHours.Value2 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 -
                                        (TotalDailyTargetManualCutting.Value2 -
                                         ((TotalDailyTargetManualStickWrapping.Value2*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP2)/convertUOM);
                                    TotalDailyTargetManualCutting.Value2 = ((TotalDailyTargetManualStickWrapping.Value2*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP2)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value3 !=
                                     (((TotalDailyTargetManualStickWrapping.Value3*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP3)/convertUOM)) &&
                                    (currentProcessWorkHours.Value3 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 -
                                        (TotalDailyTargetManualCutting.Value3 -
                                         ((TotalDailyTargetManualStickWrapping.Value3*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP3)/convertUOM);
                                    TotalDailyTargetManualCutting.Value3 = ((TotalDailyTargetManualStickWrapping.Value3*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP3)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value4 !=
                                     (((TotalDailyTargetManualStickWrapping.Value4*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP4)/convertUOM)) &&
                                    (currentProcessWorkHours.Value4 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 -
                                        (TotalDailyTargetManualCutting.Value4 -
                                         ((TotalDailyTargetManualStickWrapping.Value4*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP4)/convertUOM);
                                    TotalDailyTargetManualCutting.Value4 = ((TotalDailyTargetManualStickWrapping.Value4*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP4)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value5 !=
                                     (((TotalDailyTargetManualStickWrapping.Value5*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP5)/convertUOM)) &&
                                    (currentProcessWorkHours.Value5 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 -
                                        (TotalDailyTargetManualCutting.Value5 -
                                         ((TotalDailyTargetManualStickWrapping.Value5*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP5)/convertUOM);
                                    TotalDailyTargetManualCutting.Value5 = ((TotalDailyTargetManualStickWrapping.Value5*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP5)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value6 !=
                                     (((TotalDailyTargetManualStickWrapping.Value6*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP6)/convertUOM)) &&
                                    (currentProcessWorkHours.Value6 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 -
                                        (TotalDailyTargetManualCutting.Value6 -
                                         ((TotalDailyTargetManualStickWrapping.Value6*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP6)/convertUOM);
                                    TotalDailyTargetManualCutting.Value6 = ((TotalDailyTargetManualStickWrapping.Value6*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP6)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value7 !=
                                     (((TotalDailyTargetManualStickWrapping.Value7*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP7)/convertUOM)) &&
                                    (currentProcessWorkHours.Value7 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 -
                                        (TotalDailyTargetManualCutting.Value7 -
                                         ((TotalDailyTargetManualStickWrapping.Value7*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP7)/convertUOM);
                                    TotalDailyTargetManualCutting.Value7 = ((TotalDailyTargetManualStickWrapping.Value7*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP7)/convertUOM;
                                }

                                #endregion
                            }
                            else
                            {
                                #region Without StickWrapping

                                // System
                                if ((TotalDailyTargetSystemCutting.Value1 !=
                                     (((TotalDailyTargetSystemWrappingStamping.Value1*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP1)/convertUOM)) &&
                                    (currentProcessWorkHours.Value1 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 -
                                        (TotalDailyTargetSystemCutting.Value1 -
                                         ((TotalDailyTargetSystemWrappingStamping.Value1*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP1)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value1 = ((TotalDailyTargetSystemWrappingStamping.Value1*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP1)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value2 !=
                                     (((TotalDailyTargetSystemWrappingStamping.Value2*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP2)/convertUOM)) &&
                                    (currentProcessWorkHours.Value2 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 -
                                        (TotalDailyTargetSystemCutting.Value2 -
                                         ((TotalDailyTargetSystemWrappingStamping.Value2*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP2)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value2 = ((TotalDailyTargetSystemWrappingStamping.Value2*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP2)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value3 !=
                                     (((TotalDailyTargetSystemWrappingStamping.Value3*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP3)/convertUOM)) &&
                                    (currentProcessWorkHours.Value3 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 -
                                        (TotalDailyTargetSystemCutting.Value3 -
                                         ((TotalDailyTargetSystemWrappingStamping.Value3*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP3)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value3 = ((TotalDailyTargetSystemWrappingStamping.Value3*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP3)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value4 !=
                                     (((TotalDailyTargetSystemWrappingStamping.Value4*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP4)/convertUOM)) &&
                                    (currentProcessWorkHours.Value4 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 -
                                        (TotalDailyTargetSystemCutting.Value4 -
                                         ((TotalDailyTargetSystemWrappingStamping.Value4*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP4)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value4 = ((TotalDailyTargetSystemWrappingStamping.Value4*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP4)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value5 !=
                                     (((TotalDailyTargetSystemWrappingStamping.Value5*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP5)/convertUOM)) &&
                                    (currentProcessWorkHours.Value5 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 -
                                        (TotalDailyTargetSystemCutting.Value5 -
                                         ((TotalDailyTargetSystemWrappingStamping.Value5*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP5)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value5 = ((TotalDailyTargetSystemWrappingStamping.Value5*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP5)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value6 !=
                                     (((TotalDailyTargetSystemWrappingStamping.Value6*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP6)/convertUOM)) &&
                                    (currentProcessWorkHours.Value6 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 -
                                        (TotalDailyTargetSystemCutting.Value6 -
                                         ((TotalDailyTargetSystemWrappingStamping.Value6*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP6)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value6 = ((TotalDailyTargetSystemWrappingStamping.Value6*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP6)/convertUOM;
                                }
                                if ((TotalDailyTargetSystemCutting.Value7 !=
                                     (((TotalDailyTargetSystemWrappingStamping.Value7*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP7)/convertUOM)) &&
                                    (currentProcessWorkHours.Value7 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 -
                                        (TotalDailyTargetSystemCutting.Value7 -
                                         ((TotalDailyTargetSystemWrappingStamping.Value7*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP7)/convertUOM);
                                    TotalDailyTargetSystemCutting.Value7 = ((TotalDailyTargetSystemWrappingStamping.Value7*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP7)/convertUOM;
                                }
                                // Manual
                                if ((TotalDailyTargetManualCutting.Value1 !=
                                     (((TotalDailyTargetManualWrappingStamping.Value1*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP1)/convertUOM)) &&
                                    (currentProcessWorkHours.Value1 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 -
                                        (TotalDailyTargetManualCutting.Value1 -
                                         ((TotalDailyTargetManualWrappingStamping.Value1*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP1)/convertUOM);
                                    TotalDailyTargetManualCutting.Value1 = ((TotalDailyTargetManualWrappingStamping.Value1*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP1)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value2 !=
                                     (((TotalDailyTargetManualWrappingStamping.Value2*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP2)/convertUOM)) &&
                                    (currentProcessWorkHours.Value2 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 -
                                        (TotalDailyTargetManualCutting.Value2 -
                                         ((TotalDailyTargetManualWrappingStamping.Value2*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP2)/convertUOM);
                                    TotalDailyTargetManualCutting.Value2 = ((TotalDailyTargetManualWrappingStamping.Value2*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP2)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value3 !=
                                     (((TotalDailyTargetManualWrappingStamping.Value3*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP3)/convertUOM)) &&
                                    (currentProcessWorkHours.Value3 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 -
                                        (TotalDailyTargetManualCutting.Value3 -
                                         ((TotalDailyTargetManualWrappingStamping.Value3*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP3)/convertUOM);
                                    TotalDailyTargetManualCutting.Value3 = ((TotalDailyTargetManualWrappingStamping.Value3*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP3)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value4 !=
                                     (((TotalDailyTargetManualWrappingStamping.Value4*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP4)/convertUOM)) &&
                                    (currentProcessWorkHours.Value4 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 -
                                        (TotalDailyTargetManualCutting.Value4 -
                                         ((TotalDailyTargetManualWrappingStamping.Value4*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP4)/convertUOM);
                                    TotalDailyTargetManualCutting.Value4 = ((TotalDailyTargetManualWrappingStamping.Value4*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP4)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value5 !=
                                     (((TotalDailyTargetManualWrappingStamping.Value5*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP5)/convertUOM)) &&
                                    (currentProcessWorkHours.Value5 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 -
                                        (TotalDailyTargetManualCutting.Value5 -
                                         ((TotalDailyTargetManualWrappingStamping.Value5*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP5)/convertUOM);
                                    TotalDailyTargetManualCutting.Value5 = ((TotalDailyTargetManualWrappingStamping.Value5*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP5)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value6 !=
                                     (((TotalDailyTargetManualWrappingStamping.Value6*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP6)/convertUOM)) &&
                                    (currentProcessWorkHours.Value6 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 -
                                        (TotalDailyTargetManualCutting.Value6 -
                                         ((TotalDailyTargetManualWrappingStamping.Value6*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP6)/convertUOM);
                                    TotalDailyTargetManualCutting.Value6 = ((TotalDailyTargetManualWrappingStamping.Value6*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP6)/convertUOM;
                                }
                                if ((TotalDailyTargetManualCutting.Value7 !=
                                     (((TotalDailyTargetManualWrappingStamping.Value7*UOMEblekWrapping) +
                                       TPOTPKByProcess.VarianceWIP7)/convertUOM)) &&
                                    (currentProcessWorkHours.Value7 > 0))
                                {
                                    TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 =
                                        TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 -
                                        (TotalDailyTargetManualCutting.Value7 -
                                         ((TotalDailyTargetManualWrappingStamping.Value7*UOMEblekWrapping) +
                                          TPOTPKByProcess.VarianceWIP7)/convertUOM);
                                    TotalDailyTargetManualCutting.Value7 = ((TotalDailyTargetManualWrappingStamping.Value7*
                                                                             UOMEblekWrapping) +
                                                                            TPOTPKByProcess.VarianceWIP7)/convertUOM;
                                }

                                #endregion
                            }

                            #endregion
                        }
                        else if (previousProcess.ToUpper() == Enums.Process.Stickwrapping.ToString().ToUpper())
                        {
                            #region Stickwrapping
                            // System
                            if ((TotalDailyTargetSystemCutting.Value1 != (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM)) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 - (TotalDailyTargetSystemCutting.Value1 - (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM));
                                TotalDailyTargetSystemCutting.Value1 = (TotalDailyTargetSystemStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value2 != (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM)) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 - (TotalDailyTargetSystemCutting.Value2 - (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM));
                                TotalDailyTargetSystemCutting.Value2 = (TotalDailyTargetSystemStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value3 != (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM)) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 - (TotalDailyTargetSystemCutting.Value3 - (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM));
                                TotalDailyTargetSystemCutting.Value3 = (TotalDailyTargetSystemStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value4 != (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM)) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 - (TotalDailyTargetSystemCutting.Value4 - (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM));
                                TotalDailyTargetSystemCutting.Value4 = (TotalDailyTargetSystemStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value5 != (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM)) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 - (TotalDailyTargetSystemCutting.Value5 - (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM));
                                TotalDailyTargetSystemCutting.Value5 = (TotalDailyTargetSystemStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value6 != (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM)) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 - (TotalDailyTargetSystemCutting.Value6 - (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM));
                                TotalDailyTargetSystemCutting.Value6 = (TotalDailyTargetSystemStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM);
                            }
                            if ((TotalDailyTargetSystemCutting.Value7 != (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM)) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 - (TotalDailyTargetSystemCutting.Value7 - (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM));
                                TotalDailyTargetSystemCutting.Value7 = (TotalDailyTargetSystemStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM);
                            }
                            // Manual
                            if ((TotalDailyTargetManualCutting.Value1 != (TotalDailyTargetManualStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM)) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 - (TotalDailyTargetManualCutting.Value1 - (TotalDailyTargetManualStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM));
                                TotalDailyTargetManualCutting.Value1 = (TotalDailyTargetManualStickWrapping.Value1 + TPOTPKByProcess.VarianceWIP1 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value2 != (TotalDailyTargetManualStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM)) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 - (TotalDailyTargetManualCutting.Value2 - (TotalDailyTargetManualStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM));
                                TotalDailyTargetManualCutting.Value2 = (TotalDailyTargetManualStickWrapping.Value2 + TPOTPKByProcess.VarianceWIP2 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value3 != (TotalDailyTargetManualStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM)) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 - (TotalDailyTargetManualCutting.Value3 - (TotalDailyTargetManualStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM));
                                TotalDailyTargetManualCutting.Value3 = (TotalDailyTargetManualStickWrapping.Value3 + TPOTPKByProcess.VarianceWIP3 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value4 != (TotalDailyTargetManualStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM)) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 - (TotalDailyTargetManualCutting.Value4 - (TotalDailyTargetManualStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM));
                                TotalDailyTargetManualCutting.Value4 = (TotalDailyTargetManualStickWrapping.Value4 + TPOTPKByProcess.VarianceWIP4 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value5 != (TotalDailyTargetManualStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM)) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 - (TotalDailyTargetManualCutting.Value5 - (TotalDailyTargetManualStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM));
                                TotalDailyTargetManualCutting.Value5 = (TotalDailyTargetManualStickWrapping.Value5 + TPOTPKByProcess.VarianceWIP5 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value6 != (TotalDailyTargetManualStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM)) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 - (TotalDailyTargetManualCutting.Value6 - (TotalDailyTargetManualStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM));
                                TotalDailyTargetManualCutting.Value6 = (TotalDailyTargetManualStickWrapping.Value6 + TPOTPKByProcess.VarianceWIP6 / convertUOM);
                            }
                            if ((TotalDailyTargetManualCutting.Value7 != (TotalDailyTargetManualStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM)) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 - (TotalDailyTargetManualCutting.Value7 - (TotalDailyTargetManualStickWrapping.Value7 + TPOTPKByProcess.VarianceWIP7 / convertUOM));
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
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 - (TotalDailyTargetSystemCutting.Value1 - (((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM));
                                TotalDailyTargetSystemCutting.Value1 = ((TotalDailyTargetSystemPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value2 != ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 - (TotalDailyTargetSystemCutting.Value2 - (((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM));
                                TotalDailyTargetSystemCutting.Value2 = ((TotalDailyTargetSystemPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value3 != ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 - (TotalDailyTargetSystemCutting.Value3 - (((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM));
                                TotalDailyTargetSystemCutting.Value3 = ((TotalDailyTargetSystemPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value4 != ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 - (TotalDailyTargetSystemCutting.Value4 - (((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM));
                                TotalDailyTargetSystemCutting.Value4 = ((TotalDailyTargetSystemPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value5 != ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 - (TotalDailyTargetSystemCutting.Value5 - (((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM));
                                TotalDailyTargetSystemCutting.Value5 = ((TotalDailyTargetSystemPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value6 != ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 - (TotalDailyTargetSystemCutting.Value6 - (((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM));
                                TotalDailyTargetSystemCutting.Value6 = ((TotalDailyTargetSystemPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM;
                            }
                            if ((TotalDailyTargetSystemCutting.Value7 != ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 - (TotalDailyTargetSystemCutting.Value7 - (((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM));
                                TotalDailyTargetSystemCutting.Value7 = ((TotalDailyTargetSystemPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM;
                            }
                            // Manual
                            if ((TotalDailyTargetManualCutting.Value1 != ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM) && (currentProcessWorkHours.Value1 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 - (TotalDailyTargetManualCutting.Value1 - (((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM));
                                TotalDailyTargetManualCutting.Value1 = ((TotalDailyTargetManualPacking.Value1 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP1) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value2 != ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM) && (currentProcessWorkHours.Value2 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 - (TotalDailyTargetManualCutting.Value2 - (((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM));
                                TotalDailyTargetManualCutting.Value2 = ((TotalDailyTargetManualPacking.Value2 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP2) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value3 != ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM) && (currentProcessWorkHours.Value3 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 - (TotalDailyTargetManualCutting.Value3 - (((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM));
                                TotalDailyTargetManualCutting.Value3 = ((TotalDailyTargetManualPacking.Value3 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP3) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value4 != ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM) && (currentProcessWorkHours.Value4 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 - (TotalDailyTargetManualCutting.Value4 - (((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM));
                                TotalDailyTargetManualCutting.Value4 = ((TotalDailyTargetManualPacking.Value4 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP4) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value5 != ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM) && (currentProcessWorkHours.Value5 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 - (TotalDailyTargetManualCutting.Value5 - (((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM));
                                TotalDailyTargetManualCutting.Value5 = ((TotalDailyTargetManualPacking.Value5 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP5) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value6 != ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM) && (currentProcessWorkHours.Value6 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 - (TotalDailyTargetManualCutting.Value6 - (((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM));
                                TotalDailyTargetManualCutting.Value6 = ((TotalDailyTargetManualPacking.Value6 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP6) / convertUOM;
                            }
                            if ((TotalDailyTargetManualCutting.Value7 != ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM) && (currentProcessWorkHours.Value7 > 0))
                            {
                                TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 - (TotalDailyTargetManualCutting.Value7 - (((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM));
                                TotalDailyTargetManualCutting.Value7 = ((TotalDailyTargetManualPacking.Value7 * UOMEblekPacking) + TPOTPKByProcess.VarianceWIP7) / convertUOM;
                            }
                            #endregion
                        }
                        #endregion
                    }

                    #endregion
                }
                else if (TPOTPKByProcess.ProcessGroup.ToUpper() == Enums.Process.Rolling.ToString().ToUpper())
                {
                    #region Formula Rolling
                    #region Get Previous Greatest Process Work Hour By Process
                    var PreviousProcessWorkHoursRolling = InputPlantTPK.ListPlantTPK.Where(c => c.ProcessGroup.ToUpper() == listProcessSettings.FirstOrDefault()).Select(c => c.PlantTPK).FirstOrDefault().FirstOrDefault();
                    if (PreviousProcessWorkHoursRolling != null)
                    {
                        previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
                        {
                            Value1 = PreviousProcessWorkHoursRolling.ProcessWorkHours1,
                            Value2 = PreviousProcessWorkHoursRolling.ProcessWorkHours2,
                            Value3 = PreviousProcessWorkHoursRolling.ProcessWorkHours3,
                            Value4 = PreviousProcessWorkHoursRolling.ProcessWorkHours4,
                            Value5 = PreviousProcessWorkHoursRolling.ProcessWorkHours5,
                            Value6 = PreviousProcessWorkHoursRolling.ProcessWorkHours6,
                            Value7 = PreviousProcessWorkHoursRolling.ProcessWorkHours7
                        };
                    }
                    #endregion

                    int index = 0;
                    foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                    {

                        var DailyWHWeightedResults = CalculateDailyWHWeighted(currentProcessWorkHours, previousProcessWorkHours);

                        TPOTPKByGroup.DailyWHWeightedResult1 = DailyWHWeightedResults.Value1;
                        TPOTPKByGroup.DailyWHWeightedResult2 = DailyWHWeightedResults.Value2;
                        TPOTPKByGroup.DailyWHWeightedResult3 = DailyWHWeightedResults.Value3;
                        TPOTPKByGroup.DailyWHWeightedResult4 = DailyWHWeightedResults.Value4;
                        TPOTPKByGroup.DailyWHWeightedResult5 = DailyWHWeightedResults.Value5;
                        TPOTPKByGroup.DailyWHWeightedResult6 = DailyWHWeightedResults.Value6;
                        TPOTPKByGroup.DailyWHWeightedResult7 = DailyWHWeightedResults.Value7;

                        var c = CheckGroupPlant(TPOTPKByGroup, weekDate); // pengecekan inactive group
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
                            if (TPOTPKByGroup.WorkerAllocation > 0)
                            {
                                groupIndex = index;
                                groupEmptyAllocation = false;
                            }
                        }
                        index++;
                    }

                    TotalDailyWeighted = new GenericValuePerWeekDTO<decimal?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.Target7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        foreach (var TPOTPKByGroup in TPOTPKByProcess.PlantTPK)
                        {
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[0] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * TotalDailyTargetSystemCutting.Value1 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * TotalDailyTargetManualCutting.Value1 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[1] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * TotalDailyTargetSystemCutting.Value2 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * TotalDailyTargetManualCutting.Value2 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[2] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * TotalDailyTargetSystemCutting.Value3 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * TotalDailyTargetManualCutting.Value3 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[3] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * TotalDailyTargetSystemCutting.Value4 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * TotalDailyTargetManualCutting.Value4 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[4] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * TotalDailyTargetSystemCutting.Value5 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * TotalDailyTargetManualCutting.Value5 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[5] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * TotalDailyTargetSystemCutting.Value6 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * TotalDailyTargetManualCutting.Value6 : 0), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((InputPlantTPK.IsFilterCurrentDayForward && weekDate[6] >= InputPlantTPK.FilterCurrentDayForward) || (!InputPlantTPK.IsFilterCurrentDayForward))
                            {
                                TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * TotalDailyTargetSystemCutting.Value7 : 0), 0, MidpointRounding.AwayFromZero);
                                TPOTPKByGroup.TargetManual7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * TotalDailyTargetManualCutting.Value7 : 0), 0, MidpointRounding.AwayFromZero);
                            }

                            //TPOTPKByGroup.TargetSystem1 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value1 != 0 ? (float)TPOTPKByGroup.Target1 / (float)TotalDailyWeighted.Value1 * TotalDailyTargetSystemCutting.Value1 : 0), 0, MidpointRounding.AwayFromZero);
                            //TPOTPKByGroup.TargetSystem2 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value2 != 0 ? (float)TPOTPKByGroup.Target2 / (float)TotalDailyWeighted.Value2 * TotalDailyTargetSystemCutting.Value2 : 0), 0, MidpointRounding.AwayFromZero);
                            //TPOTPKByGroup.TargetSystem3 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value3 != 0 ? (float)TPOTPKByGroup.Target3 / (float)TotalDailyWeighted.Value3 * TotalDailyTargetSystemCutting.Value3 : 0), 0, MidpointRounding.AwayFromZero);
                            //TPOTPKByGroup.TargetSystem4 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value4 != 0 ? (float)TPOTPKByGroup.Target4 / (float)TotalDailyWeighted.Value4 * TotalDailyTargetSystemCutting.Value4 : 0), 0, MidpointRounding.AwayFromZero);
                            //TPOTPKByGroup.TargetSystem5 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value5 != 0 ? (float)TPOTPKByGroup.Target5 / (float)TotalDailyWeighted.Value5 * TotalDailyTargetSystemCutting.Value5 : 0), 0, MidpointRounding.AwayFromZero);
                            //TPOTPKByGroup.TargetSystem6 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value6 != 0 ? (float)TPOTPKByGroup.Target6 / (float)TotalDailyWeighted.Value6 * TotalDailyTargetSystemCutting.Value6 : 0), 0, MidpointRounding.AwayFromZero);
                            //TPOTPKByGroup.TargetSystem7 = (float)Math.Round((Decimal)((float)TotalDailyWeighted.Value7 != 0 ? (float)TPOTPKByGroup.Target7 / (float)TotalDailyWeighted.Value7 * TotalDailyTargetSystemCutting.Value7 : 0), 0, MidpointRounding.AwayFromZero);

                            TPOTPKByGroup.TotalTargetSystem = TPOTPKByGroup.TargetSystem1 + TPOTPKByGroup.TargetSystem2 +
                                                          TPOTPKByGroup.TargetSystem3 + TPOTPKByGroup.TargetSystem4 +
                                                          TPOTPKByGroup.TargetSystem5 + TPOTPKByGroup.TargetSystem6 +
                                                          TPOTPKByGroup.TargetSystem7;

                            //TPOTPKByGroup.TargetManual1 = TPOTPKByGroup.TargetSystem1;
                            //TPOTPKByGroup.TargetManual2 = TPOTPKByGroup.TargetSystem2;
                            //TPOTPKByGroup.TargetManual3 = TPOTPKByGroup.TargetSystem3;
                            //TPOTPKByGroup.TargetManual4 = TPOTPKByGroup.TargetSystem4;
                            //TPOTPKByGroup.TargetManual5 = TPOTPKByGroup.TargetSystem5;
                            //TPOTPKByGroup.TargetManual6 = TPOTPKByGroup.TargetSystem6;
                            //TPOTPKByGroup.TargetManual7 = TPOTPKByGroup.TargetSystem7;

                            TPOTPKByGroup.TotalTargetManual = TPOTPKByGroup.TargetManual1 + TPOTPKByGroup.TargetManual2 +
                                                          TPOTPKByGroup.TargetManual3 + TPOTPKByGroup.TargetManual4 +
                                                          TPOTPKByGroup.TargetManual5 + TPOTPKByGroup.TargetManual6 +
                                                          TPOTPKByGroup.TargetManual7;
                        }
                    }
                    else
                    {
                        TPOTPKByProcess.PlantTPK = fillEmptyAllocationPlantTpk(TPOTPKByProcess.PlantTPK);
                    }

                    // Variables for Next Process
                    TotalDailyTargetSystemRolling = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetSystem7)
                    };

                    TotalDailyTargetManualRolling = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual1),
                        Value2 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual2),
                        Value3 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual3),
                        Value4 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual4),
                        Value5 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual5),
                        Value6 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual6),
                        Value7 = TPOTPKByProcess.PlantTPK.Sum(t => t.TargetManual7)
                    };

                    if (!groupEmptyAllocation)
                    {
                        #region Different checking
                        // System
                        if ((TotalDailyTargetSystemRolling.Value1 != TotalDailyTargetSystemCutting.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem1 + (TotalDailyTargetSystemCutting.Value1 - TotalDailyTargetSystemRolling.Value1);
                            TotalDailyTargetSystemRolling.Value1 = TotalDailyTargetSystemCutting.Value1;
                        }
                        if ((TotalDailyTargetSystemRolling.Value2 != TotalDailyTargetSystemCutting.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem2 + (TotalDailyTargetSystemCutting.Value2 - TotalDailyTargetSystemRolling.Value2);
                            TotalDailyTargetSystemRolling.Value2 = TotalDailyTargetSystemCutting.Value2;
                        }
                        if ((TotalDailyTargetSystemRolling.Value3 != TotalDailyTargetSystemCutting.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem3 + (TotalDailyTargetSystemCutting.Value3 - TotalDailyTargetSystemRolling.Value3);
                            TotalDailyTargetSystemRolling.Value3 = TotalDailyTargetSystemCutting.Value3;
                        }
                        if ((TotalDailyTargetSystemRolling.Value4 != TotalDailyTargetSystemCutting.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem4 + (TotalDailyTargetSystemCutting.Value4 - TotalDailyTargetSystemRolling.Value4);
                            TotalDailyTargetSystemRolling.Value4 = TotalDailyTargetSystemCutting.Value4;
                        }
                        if ((TotalDailyTargetSystemRolling.Value5 != TotalDailyTargetSystemCutting.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem5 + (TotalDailyTargetSystemCutting.Value5 - TotalDailyTargetSystemRolling.Value5);
                            TotalDailyTargetSystemRolling.Value5 = TotalDailyTargetSystemCutting.Value5;
                        }
                        if ((TotalDailyTargetSystemRolling.Value6 != TotalDailyTargetSystemCutting.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem6 + (TotalDailyTargetSystemCutting.Value6 - TotalDailyTargetSystemRolling.Value6);
                            TotalDailyTargetSystemRolling.Value6 = TotalDailyTargetSystemCutting.Value6;
                        }
                        if ((TotalDailyTargetSystemRolling.Value7 != TotalDailyTargetSystemCutting.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetSystem7 + (TotalDailyTargetSystemCutting.Value7 - TotalDailyTargetSystemRolling.Value7);
                            TotalDailyTargetSystemRolling.Value7 = TotalDailyTargetSystemCutting.Value7;
                        }
                        // Manual
                        if ((TotalDailyTargetManualRolling.Value1 != TotalDailyTargetManualCutting.Value1) && (currentProcessWorkHours.Value1 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual1 + (TotalDailyTargetManualCutting.Value1 - TotalDailyTargetManualRolling.Value1);
                            TotalDailyTargetManualRolling.Value1 = TotalDailyTargetManualCutting.Value1;
                        }
                        if ((TotalDailyTargetManualRolling.Value2 != TotalDailyTargetManualCutting.Value2) && (currentProcessWorkHours.Value2 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual2 + (TotalDailyTargetManualCutting.Value2 - TotalDailyTargetManualRolling.Value2);
                            TotalDailyTargetManualRolling.Value2 = TotalDailyTargetManualCutting.Value2;
                        }
                        if ((TotalDailyTargetManualRolling.Value3 != TotalDailyTargetManualCutting.Value3) && (currentProcessWorkHours.Value3 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual3 + (TotalDailyTargetManualCutting.Value3 - TotalDailyTargetManualRolling.Value3);
                            TotalDailyTargetManualRolling.Value3 = TotalDailyTargetManualCutting.Value3;
                        }
                        if ((TotalDailyTargetManualRolling.Value4 != TotalDailyTargetManualCutting.Value4) && (currentProcessWorkHours.Value4 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual4 + (TotalDailyTargetManualCutting.Value4 - TotalDailyTargetManualRolling.Value4);
                            TotalDailyTargetManualRolling.Value4 = TotalDailyTargetManualCutting.Value4;
                        }
                        if ((TotalDailyTargetManualRolling.Value5 != TotalDailyTargetManualCutting.Value5) && (currentProcessWorkHours.Value5 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual5 + (TotalDailyTargetManualCutting.Value5 - TotalDailyTargetManualRolling.Value5);
                            TotalDailyTargetManualRolling.Value5 = TotalDailyTargetManualCutting.Value5;
                        }
                        if ((TotalDailyTargetManualRolling.Value6 != TotalDailyTargetManualCutting.Value6) && (currentProcessWorkHours.Value6 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual6 + (TotalDailyTargetManualCutting.Value6 - TotalDailyTargetManualRolling.Value6);
                            TotalDailyTargetManualRolling.Value6 = TotalDailyTargetManualCutting.Value6;
                        }
                        if ((TotalDailyTargetManualRolling.Value7 != TotalDailyTargetManualCutting.Value7) && (currentProcessWorkHours.Value7 > 0))
                        {
                            TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 = TPOTPKByProcess.PlantTPK[groupIndex].TargetManual7 + (TotalDailyTargetManualCutting.Value7 - TotalDailyTargetManualRolling.Value7);
                            TotalDailyTargetManualRolling.Value7 = TotalDailyTargetManualCutting.Value7;
                        }
                        #endregion
                    }

                    #endregion
                }
                #endregion

                #region Get Previous Process Work Hour By Process
                var PreviousProcessWorkHours = TPOTPKByProcess.PlantTPK.FirstOrDefault();
                if (PreviousProcessWorkHours != null)
                {
                    previousProcessWorkHours = new GenericValuePerWeekDTO<float?>()
                    {
                        Value1 = PreviousProcessWorkHours.ProcessWorkHours1,
                        Value2 = PreviousProcessWorkHours.ProcessWorkHours2,
                        Value3 = PreviousProcessWorkHours.ProcessWorkHours3,
                        Value4 = PreviousProcessWorkHours.ProcessWorkHours4,
                        Value5 = PreviousProcessWorkHours.ProcessWorkHours5,
                        Value6 = PreviousProcessWorkHours.ProcessWorkHours6,
                        Value7 = PreviousProcessWorkHours.ProcessWorkHours7
                    };
                }
                #endregion

                // Set PREVIOUS Process State
                previousProcess = TPOTPKByProcess.ProcessGroup;
            }

            #region Total Calculation
            var totalBoxResponse = new TargetManualTPUDTO()
            {
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
            };

            total.Add(totalBoxResponse);
            #endregion

            #endregion

            var result = new PlantTPKCalculateDTO()
            {
                PlantTPKByProcess = InputPlantTPK.ListPlantTPK,
                PlantTPKTotals = total
            };

            return result;
        }

        public CalculateTPOTPKCheck CheckGroupPlant(PlantTPKDTO TPOTPKByGroup, List<DateTime> weekDate)
        {
            // pengecekan inactive group
            var groupInactive = _masterDataBll.GetPlantProductionGroupById(TPOTPKByGroup.GroupCode, TPOTPKByGroup.UnitCode, TPOTPKByGroup.LocationCode, TPOTPKByGroup.ProcessGroup);
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

        private List<PlantTPKDTO> fillEmptyAllocationPlantTpk(List<PlantTPKDTO> tpoTpk)
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

        public List<PlanningPlantIndividualCapacityWorkHourDTO> GetProsesGroupFromCapacityWorkHour(string locationCode, string unitCode, string brandGruopCode, string gruopCode)
        {
            var queryFilter = PredicateHelper.True<PlanPlantIndividualCapacityWorkHour>();

            if (!string.IsNullOrEmpty(locationCode))
            {
                queryFilter = queryFilter.And(wh => wh.LocationCode == locationCode);
            }

            if (!string.IsNullOrEmpty(unitCode))
            {
                queryFilter = queryFilter.And(wh => wh.UnitCode == unitCode);
            }

            if (!string.IsNullOrEmpty(brandGruopCode))
            {
                queryFilter = queryFilter.And(wh => wh.BrandGroupCode == brandGruopCode);
            }

            if (!string.IsNullOrEmpty(gruopCode))
            {
                queryFilter = queryFilter.And(wh => wh.GroupCode == gruopCode);
            }

            var dbResult = _planningPlantIndividualCapacityWorkHourRepo.Get(queryFilter);

            return Mapper.Map<List<PlanningPlantIndividualCapacityWorkHourDTO>>(dbResult);
        }

        #endregion

        #region WIPDetail

        public WIPStockDTO GetPlantWIPStock(GetPlantWIPStockInput input)
        {
            PlanPlantWIPDetail dbPlantWIPDetail;
            var dbResults = _planPlantWIPDetailRepo.Get(m => m.KPSYear == input.KPSYear && m.KPSWeek == input.KPSWeek
                                                             && m.LocationCode == input.LocationCode &&
                                                             m.ProcessGroup == input.ProcessGroup &&
                                                             m.UnitCode == input.UnitCode &&
                                                             m.BrandCode == input.BrandCode);

            try
            {
                dbPlantWIPDetail = dbResults.SingleOrDefault();
            }
            catch (Exception ex)
            {
                //_logger.Info("GetPlantWIPStock", ex.Message);
                return null;
            }

            return Mapper.Map<WIPStockDTO>(dbPlantWIPDetail);
        }

        private void UpdateWIPStock(WIPStockDTO wipStock)
        {
            var dbResults = _planPlantWIPDetailRepo.Get(m => m.KPSYear == wipStock.KPSYear && m.KPSWeek == wipStock.KPSWeek
                                                             && m.LocationCode == wipStock.LocationCode &&
                                                             m.ProcessGroup == wipStock.ProcessGroup &&
                                                             m.BrandCode == wipStock.BrandCode &&
                                                             m.UnitCode == wipStock.UnitCode);

            if (dbResults.Count() > 0)
            {
                foreach (var dbWIPStock in dbResults)
                {
                    //keep original CreatedBy and CreatedDate
                    wipStock.CreatedBy = dbWIPStock.CreatedBy;
                    wipStock.CreatedDate = dbWIPStock.CreatedDate;

                    //set update time
                    wipStock.UpdatedDate = DateTime.Now;

                    Mapper.Map(wipStock, dbWIPStock);
                    _planPlantWIPDetailRepo.Update(dbWIPStock);
                }
            }
            else
            {
                wipStock.UpdatedDate = DateTime.Now;
                wipStock.CreatedDate = DateTime.Now;
                wipStock.CreatedBy = "SYSTEM";
                wipStock.UpdatedBy = "SYSTEM";
                var obj = Mapper.Map<PlanPlantWIPDetail>(wipStock);
                _planPlantWIPDetailRepo.Insert(obj);

                _uow.SaveChanges();
            }

        }
        #endregion
        #region PlanTmpWeeklyProductionPlanning
        public List<PlanTmpWeeklyProductionPlanningDTO> GetKPSPlanTempWeeklyProductionGroup()
        {
            var queryFilter = PredicateHelper.True<PlanTmpWeeklyProductionPlanning>();
            var dbResult = _planTmpWeeklyProductionPlaningRepo.Get(queryFilter);
            return Mapper.Map<List<PlanTmpWeeklyProductionPlanningDTO>>(dbResult);
        }


        #endregion
        #region getprocessbyPlantTPK
        public List<string> GetPlantTPKProcessByLocations(string locationCode)
        {
            var PlantProcess = _planPlantTargetProductionKelompokRepo.Get(planttpk => planttpk.LocationCode == locationCode);
            //var input = new getmar
            //var checkOrder = _masterDataBll.GetMasterProcesses();
            var MstGenProc = _mstGenProcess.Get();

            var Joined = from pGroup in PlantProcess
                         join pOrder in MstGenProc
                         on pGroup.ProcessGroup equals pOrder.ProcessGroup
                         orderby pOrder.ProcessOrder ascending
                         select pGroup.ProcessGroup;

            return Joined.Distinct().ToList();

        }
        #endregion

        #region Report Plan

        public List<MstTableauReportDto> GetReportTableau(Enums.ReportTableau input)
        {
            var dbResult = _mstTableauReportRepo.Get(p => p.ReportType == input.ToString());

            return Mapper.Map<List<MstTableauReportDto>>(dbResult);
        }

        public MstTableauReportDto GetReportTableau(int id)
        {
            var dbResult = _mstTableauReportRepo.GetByID(id);

            return Mapper.Map<MstTableauReportDto>(dbResult);
        }

        public void SaveReport(MstTableauReportDto input)
        {
            var model = Mapper.Map<MstTableauReport>(input);

            _mstTableauReportRepo.Insert(model);

            _uow.SaveChanges();
        }

        public void DeleteReport(MstTableauReportDto input)
        {
            var dbResult = _mstTableauReportRepo.GetByID(input.Id);

            if (dbResult == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            _mstTableauReportRepo.Delete(dbResult);
            _uow.SaveChanges();
        }

        #endregion
    }
}
