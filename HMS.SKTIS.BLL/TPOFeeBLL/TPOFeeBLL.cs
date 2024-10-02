using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace HMS.SKTIS.BLL.TPOFeeBLL
{
    public class TPOFeeBLL : ITPOFeeBLL
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TPOFeeExeActualView> _tpoFeeExeActualViewRepo;
        private IGenericRepository<TPOFeeHdrPlan> _tpoFeeHdrPlanRepo;
        private IGenericRepository<TPOFeeHdr> _tpoFeeHdrRepo;
        private IMasterDataBLL _masterDataBll;
        private IGenericRepository<TPOFeeProductionDaily> _tpoFeeProductionDailyRepo;
        private IGenericRepository<TPOFeeCalculation> _tpoFeeCalculationRepo;
        private IGenericRepository<TPOFeeCalculationPlan> _tpoFeeCalculationPlanRepo;
        private IGenericRepository<TpoFeePlanView> _tpoFeePlanViewRepo;
        private ISqlSPRepository _sqlSPRepo;
        private IExecutionTPOBLL _executionTpobll;
        private IGenericRepository<TPOFeeApprovalView> _tpoFeeApprovalRepo;
        private IUtilitiesBLL _utilitiesBll;
        private IGenericRepository<MstTPOPackage> _mstTPOPackageRepo;
        private IGenericRepository<MstGenLocation> _mstGenLocationRepo;
        private IGenericRepository<MstGenWeek> _mstGenWeekRepo;
        private IGenericRepository<MstADTemp> _mstAdTemp;
        private IGenericRepository<TPOFeeReportsProductionDailyView> _tpoFeeReportsProductionView;
        private IGenericRepository<TPOFeeReportsProductionWeeklyView> _tpoFeeReportsProductionWeeklyView;
        private IGenericRepository<TPOGenerateP1TemplateView> _tpoGenerateP1TemplateView;

        public TPOFeeBLL(IUnitOfWork uow, IMasterDataBLL masterDataBll, IExecutionTPOBLL executionTpobll, IUtilitiesBLL utilitiesBll)
        {
            _uow = uow;
            _tpoFeeExeActualViewRepo = _uow.GetGenericRepository<TPOFeeExeActualView>();
            _tpoFeeHdrPlanRepo = _uow.GetGenericRepository<TPOFeeHdrPlan>();
            _tpoFeeHdrRepo = _uow.GetGenericRepository<TPOFeeHdr>();
            _masterDataBll = masterDataBll;
            _tpoFeeProductionDailyRepo = _uow.GetGenericRepository<TPOFeeProductionDaily>();
            _tpoFeeCalculationRepo = _uow.GetGenericRepository<TPOFeeCalculation>();
            _tpoFeeCalculationPlanRepo = _uow.GetGenericRepository<TPOFeeCalculationPlan>();
            _tpoFeePlanViewRepo = _uow.GetGenericRepository<TpoFeePlanView>();
            _sqlSPRepo = _uow.GetSPRepository();
            _executionTpobll = executionTpobll;
            _tpoFeeApprovalRepo = _uow.GetGenericRepository<TPOFeeApprovalView>();
            _utilitiesBll = utilitiesBll;
            _mstTPOPackageRepo = _uow.GetGenericRepository<MstTPOPackage>();
            _mstGenLocationRepo = _uow.GetGenericRepository<MstGenLocation>();
            _mstGenWeekRepo = _uow.GetGenericRepository<MstGenWeek>();
            _mstAdTemp = _uow.GetGenericRepository<MstADTemp>();
            _tpoFeeReportsProductionView = _uow.GetGenericRepository<TPOFeeReportsProductionDailyView>();
            _tpoFeeReportsProductionWeeklyView = _uow.GetGenericRepository<TPOFeeReportsProductionWeeklyView>();
            _tpoGenerateP1TemplateView = _uow.GetGenericRepository<TPOGenerateP1TemplateView>();
        }

        #region TPO Fee Actual
        public TPOFeeExeActualViewDTO GetTPOFeeActualByProductionCode(string TPOFeeCode)
        {
            var queryFilter = PredicateHelper.True<TPOFeeExeActualView>();

            if (!string.IsNullOrEmpty(TPOFeeCode))
                queryFilter = queryFilter.And(m => m.TPOFeeCode == TPOFeeCode);

            var dbResult = _tpoFeeExeActualViewRepo.Get(queryFilter).FirstOrDefault();

            return Mapper.Map<TPOFeeExeActualViewDTO>(dbResult);
        }
        public List<TPOFeeExeActualViewDTO> GetTpoFeeExeActuals(GetTPOFeeExeActualInput input)
        {
            var queryFilter = PredicateHelper.True<TPOFeeApprovalView>();

            if (string.IsNullOrEmpty(input.Regional))
                return new List<TPOFeeExeActualViewDTO>();

            if (input.Regional != HMS.SKTIS.Core.Enums.LocationCode.TPO.ToString())
            {
                //var locations = _masterDataBll.GetAllLocationByLocationCode(input.Regional, 1).Select(m => m.LocationCode);
                //queryFilter = queryFilter.And(m => locations.Contains(m.LocationCode));
                queryFilter = queryFilter.And(m => m.ParentLocationCode == input.Regional);
            }

            if (input.Year > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);

            if (input.Week > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.Week);

            if (input.ApprovalPage)
            {
                // for approval page list
                queryFilter = queryFilter.And(m => m.Status != "Open").And(m => m.Status != "Draft");
            }

            queryFilter = queryFilter.And(m => m.TPOPackageValue != null).And(m => m.TPOPackageValue != 0);

            //var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];

            //queryFilter = queryFilter.And(m => m.DestinationRole == session.Responsibility.Role);

            input.SortExpression = "ParentLocationCode";
            input.SortOrder = "ASC";

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TPOFeeApprovalView>();
            var dbResult = _tpoFeeApprovalRepo.Get(queryFilter, orderByFilter);

            //return destination roles codes as PIC
            foreach (var tpoFeeApprovalView in dbResult)
            {
                tpoFeeApprovalView.PIC = tpoFeeApprovalView.DestinationRolesCodes;
            }
            return Mapper.Map<List<TPOFeeExeActualViewDTO>>(dbResult.OrderBy(m => m.ParentLocationCode).ThenBy(m => m.LocationCode).ThenBy(m => m.SKTBrandCode));
        }

        public List<TPOFeeExeAPOpenViewDTO> GetTpoFeeExeAPOpens(GetTPOFeeExeAPOpenInput input)
        {
            //var queryFilter = PredicateHelper.True<TPOFeeExeActualView>();
            var queryFilter = PredicateHelper.True<TPOFeeApprovalView>();

            if (!string.IsNullOrEmpty(input.Regional) && input.Regional != HMS.SKTIS.Core.Enums.LocationCode.TPO.ToString())
            {
                var locations = _masterDataBll.GetAllLocationByLocationCode(input.Regional, 1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locations.Contains(m.LocationCode));
            }

            if (input.Year > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);

            if (input.Week > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.Week);

            queryFilter = queryFilter.And(m => m.TPOPackageValue != 0);
            queryFilter = queryFilter.And(m => m.TPOPackageValue != null);
            //// begin : comment by this ticket http://tp.voxteneo.co.id/entity/10980 and http://tp.voxteneo.co.id/entity/11063
            //if (input.ApprovalPage)
            //{
                
            //    // for approval page list
            //    queryFilter = queryFilter.And(m => m.Status == "AUTHORIZED");
                
            //}
            
            //var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];

            //queryFilter = queryFilter.And(m => m.DestinationRole == session.Responsibility.Role);
            //// end : comment by this ticket http://tp.voxteneo.co.id/entity/10980 and http://tp.voxteneo.co.id/entity/11063

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TPOFeeApprovalView>();
            var dbResult = _tpoFeeApprovalRepo.Get(queryFilter, orderByFilter);
            foreach (var tpoFeeApprovalView in dbResult)
            {
                tpoFeeApprovalView.PIC = tpoFeeApprovalView.DestinationRolesCodes;
            }
            return Mapper.Map<List<TPOFeeExeAPOpenViewDTO>>(dbResult);
        }

        public TPOFeeHdrDTO GetTpoFeeHdrById(string tpoFeeCode)
        {
            var dbTpoFeeHdr = _tpoFeeHdrRepo.GetByID(tpoFeeCode);
            return Mapper.Map<TPOFeeHdrDTO>(dbTpoFeeHdr);
        }

        public TPOFeeHdrDTO GetTpoFeeHdrByParam(string locationCode, string brandCode, int? year, int? week)
        {
            var input = new GetBrandInput();
            input.BrandCode = brandCode;
            var brand = _masterDataBll.GetBrand(input);
            var dbTpoFeeHdr = _tpoFeeHdrRepo.Get(m => m.LocationCode == locationCode
                && m.BrandGroupCode == brand.BrandGroupCode
                && m.KPSYear == year
                && m.KPSWeek == week
                ).FirstOrDefault();

            return Mapper.Map<TPOFeeHdrDTO>(dbTpoFeeHdr);
        }

        public TpoFeeHdrPlanDto GetTpoFeeHdrPlanByParam(string locationCode, string brandCode, int? year, int? week)
        {
            var input = new GetBrandInput();
            input.BrandCode = brandCode;
            var brand = _masterDataBll.GetBrand(input);
            var dbTpoFeeHdr = _tpoFeeHdrPlanRepo.Get(m => m.LocationCode == locationCode
                && m.BrandGroupCode == brand.BrandGroupCode
                && m.KPSYear == year
                && m.KPSWeek == week
                ).FirstOrDefault();

            return Mapper.Map<TpoFeeHdrPlanDto>(dbTpoFeeHdr);
        }

        public void AuthorizeActual(string tpoFeeCode, string userName)
        {
            var tpoFeeHdrTpo = GetTpoFeeHdrById(tpoFeeCode);
            var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrTpo.BrandGroupCode);
            var daily = GetTpoFeeProductionDailys(tpoFeeCode);
            var calculations = GetTpoFeeCalculation(tpoFeeCode);
            var tpoFeeHdr = _tpoFeeHdrRepo.GetByID(tpoFeeCode);

            double? TotalProdBox = Convert.ToDouble(Math.Round((decimal)daily.Sum(m => m.OutputBox), 5));//tpoFeeHdrTpo.TotalProdBox,
            var TotalProdJKN = daily.Sum(m => m.JKN);//tpoFeeHdrTpo.TotalProdJKN ?? 0,
            var TotalProdJl1 = daily.Sum(m => m.JL1);//tpoFeeHdrTpo.TotalProdJl1 ?? 0,
            var TotalProdJl2 = daily.Sum(m => m.Jl2);//tpoFeeHdrTpo.TotalProdJl2 ?? 0,
            var TotalProdJl3 = daily.Sum(m => m.Jl3);//tpoFeeHdrTpo.TotalProdJl3 ?? 0,
            var TotalProdJl4 = daily.Sum(m => m.Jl4);//tpoFeeHdrTpo.TotalProdJl4 ?? 0,

            if (tpoFeeHdr != null)
            {
                TotalProdBox = Math.Round(tpoFeeHdr.TotalProdBox.Value, 5);
                TotalProdJKN = Math.Round(tpoFeeHdr.TotalProdJKN.Value, 5);
                TotalProdJl1 = Math.Round(tpoFeeHdr.TotalProdJl1.Value, 5);
                TotalProdJl2 = Math.Round(tpoFeeHdr.TotalProdJl2.Value, 5);
                TotalProdJl3 = Math.Round(tpoFeeHdr.TotalProdJl3.Value, 5);
                TotalProdJl4 = Math.Round(tpoFeeHdr.TotalProdJl4.Value, 5);
            }

            var TotalDibayarJKN = tpoFeeHdrTpo.TotalProdJKN;
            var TotalDibayarJL1 = tpoFeeHdrTpo.TotalProdJl1;
            var TotalDibayarJL2 = tpoFeeHdrTpo.TotalProdJl2;
            var TotalDibayarJL3 = tpoFeeHdrTpo.TotalProdJl3;
            var TotalDibayarJL4 = tpoFeeHdrTpo.TotalProdJl4;

            var convertJKN = new double[7];
            var convertJL1 = new double[7];
            var convertJL2 = new double[7];
            var convertJL3 = new double[7];
            var convertJL4 = new double[7];

            var totalRupiahJKN = calculations.Where(m => m.ProductionFeeType == "JKN").Select(s => s.Calculate).FirstOrDefault();
            var totalRupiahJL1 = calculations.Where(m => m.ProductionFeeType == "JL1").Select(s => s.Calculate).FirstOrDefault();
            var totalRupiahJL2 = calculations.Where(m => m.ProductionFeeType == "JL2").Select(s => s.Calculate).FirstOrDefault();
            var totalRupiahJL3 = calculations.Where(m => m.ProductionFeeType == "JL3").Select(s => s.Calculate).FirstOrDefault();
            var totalRupiahJL4 = calculations.Where(m => m.ProductionFeeType == "JL4").Select(s => s.Calculate).FirstOrDefault();

            for (var i = 0; i < daily.Count; i++)
            {
                convertJKN[i] = 0;
                convertJL1[i] = 0;
                convertJL2[i] = 0;
                convertJL3[i] = 0;
                convertJL4[i] = 0;
                
                if (daily[i].JKN.HasValue && TotalProdJKN.HasValue && TotalDibayarJKN.HasValue)
                {
                    if (TotalProdJKN.Value*TotalDibayarJKN.Value > 0)
                    {
                        convertJKN[i] = Math.Round((daily[i].OutputBox.Value / TotalProdBox.Value * TotalProdJKN.Value), 5);
                        daily[i].JKNRp = convertJKN[i]/TotalDibayarJKN.Value*totalRupiahJKN;
                        if (daily[i].JKNRp == null)
                        {
                            daily[i].JKNRp = 0;
                        }
                        else if (double.IsNaN(daily[i].JKNRp.Value))
                        {
                            daily[i].JKNRp = 0;
                        }

                    }
                    else
                    {
                        daily[i].JKNRp = 0;
                    }
                }
                if (daily[i].JL1.HasValue && TotalProdJl1.HasValue && TotalDibayarJL1.HasValue)
                {
                    if (TotalProdJl1.Value*TotalDibayarJL1.Value > 0)
                    {
                        convertJL1[i] = Math.Round((daily[i].OutputBox.Value / TotalProdBox.Value * TotalProdJl1.Value), 5);
                        daily[i].JL1Rp = convertJL1[i]/TotalDibayarJL1.Value*totalRupiahJL1;
                        if (daily[i].JL1Rp == null)
                        {
                            daily[i].JL1Rp = 0;
                        }
                        else if (double.IsNaN(daily[i].JL1Rp.Value))
                        {
                            daily[i].JL1Rp = 0;
                        }
                    }
                    else
                    {
                        daily[i].JL1Rp = 0;
                    }
                }
                if (daily[i].Jl2.HasValue && TotalProdJl2.HasValue && TotalDibayarJL2.HasValue)
                {
                    if (TotalProdJl2.Value*TotalDibayarJL2.Value > 0)
                    {
                        convertJL2[i] = Math.Round((daily[i].OutputBox.Value / TotalProdBox.Value * TotalProdJl2.Value), 5);
                        daily[i].JL2Rp = convertJL2[i]/TotalDibayarJL2.Value*totalRupiahJL2;
                        if (daily[i].JL2Rp == null)
                        {
                            daily[i].JL2Rp = 0;
                        }
                        else if (double.IsNaN(daily[i].JL2Rp.Value))
                        {
                            daily[i].JL2Rp = 0;
                        }
                    }
                    else
                    {
                        daily[i].JL2Rp = 0;
                    }
                }
                if (daily[i].Jl3.HasValue && TotalProdJl3.HasValue && TotalDibayarJL3.HasValue)
                {
                    if (TotalProdJl3.Value*TotalDibayarJL3.Value > 0)
                    {
                        convertJL3[i] = Math.Round((daily[i].OutputBox.Value / TotalProdBox.Value * TotalProdJl3.Value), 5);
                        daily[i].JL3Rp = convertJL3[i]/TotalDibayarJL3.Value*totalRupiahJL3;
                        if (daily[i].JL3Rp == null)
                        {
                            daily[i].JL3Rp = 0;
                        }
                        else if (double.IsNaN(daily[i].JL3Rp.Value))
                        {
                            daily[i].JL3Rp = 0;
                        }
                    }
                    else
                    {
                        daily[i].JL3Rp = 0;
                    }
                }
                if (daily[i].Jl4.HasValue && TotalProdJl4.HasValue && TotalDibayarJL4.HasValue)
                {
                    if (TotalProdJl4.Value*TotalDibayarJL4.Value > 0)
                    {
                        convertJL4[i] = Math.Round((daily[i].OutputBox.Value / TotalProdBox.Value * TotalProdJl4.Value), 5);
                        daily[i].JL4Rp = convertJL4[i]/TotalDibayarJL4.Value*totalRupiahJL4;
                        if (daily[i].JL4Rp == null)
                        {
                            daily[i].JL4Rp = 0;
                        }
                        else if (double.IsNaN(daily[i].JL4Rp.Value))
                        {
                            daily[i].JL4Rp = 0;
                        }
                    }
                    else
                    {
                        daily[i].JL4Rp = 0;
                    }
                }

                // update rupiah to daily table
                var dbDaily = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[i].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                dbDaily.JKNRp = daily[i].JKNRp;
                dbDaily.JL1Rp = daily[i].JL1Rp;
                dbDaily.JL2Rp = daily[i].JL2Rp;
                dbDaily.JL3Rp = daily[i].JL3Rp;
                dbDaily.JL4Rp = daily[i].JL4Rp;
                dbDaily.JKNBoxFinal = convertJKN[i];
                dbDaily.JL1BoxFinal = convertJL1[i];
                dbDaily.JL2BoxFinal = convertJL2[i];
                dbDaily.JL3BoxFinal = convertJL3[i];
                dbDaily.JL4BoxFinal = convertJL4[i];
                dbDaily.UpdatedDate = DateTime.Now;
                dbDaily.UpdatedBy = userName;
                _tpoFeeProductionDailyRepo.Update(dbDaily);

            }

            // Checking last index non zero 
            var indexLastnonzeroJKN = Array.FindLastIndex(convertJKN, c => c != 0);
            var indexLastnonzeroJL1 = Array.FindLastIndex(convertJL1, c => c != 0);
            var indexLastnonzeroJL2 = Array.FindLastIndex(convertJL2, c => c != 0);
            var indexLastnonzeroJL3 = Array.FindLastIndex(convertJL3, c => c != 0);
            var indexLastnonzeroJL4 = Array.FindLastIndex(convertJL4, c => c != 0);

            /** Check if sum JKN, JL1, JL2, JL3, JL4 greater or min from TotalProdJKN, TotalProdJL1, TotalProdJl2, TotalProdJL3, TotalProdJL4 **/
            // JKN
            if (indexLastnonzeroJKN >= 0) 
            {
                if (TotalProdJKN.Value > convertJKN.Sum())
                {
                    convertJKN[indexLastnonzeroJKN] += Math.Round((TotalProdJKN.Value - convertJKN.Sum()), 5);
                    var dbDailyUpdJKN = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJKN].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJKN.JKNBoxFinal = convertJKN[indexLastnonzeroJKN];
                    dbDailyUpdJKN.JKNRp = convertJKN[indexLastnonzeroJKN] / TotalDibayarJKN.Value * totalRupiahJKN;
                    dbDailyUpdJKN.UpdatedDate = DateTime.Now;
                    dbDailyUpdJKN.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJKN);
                }
                else if (TotalProdJKN.Value < convertJKN.Sum())
                {
                    convertJKN[indexLastnonzeroJKN] -= Math.Round((convertJKN.Sum() - TotalProdJKN.Value), 5);
                    var dbDailyUpdJKN = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJKN].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJKN.JKNBoxFinal = convertJKN[indexLastnonzeroJKN];
                    dbDailyUpdJKN.JKNRp = convertJKN[indexLastnonzeroJKN] / TotalDibayarJKN.Value * totalRupiahJKN;
                    dbDailyUpdJKN.UpdatedDate = DateTime.Now;
                    dbDailyUpdJKN.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJKN);
                }
            }
            
            // JL1
            if (indexLastnonzeroJL1 >= 0)
            {
                if (TotalProdJl1.Value > convertJL1.Sum())
                {
                    convertJL1[indexLastnonzeroJL1] += Math.Round((TotalProdJl1.Value - convertJL1.Sum()), 5);
                    var dbDailyUpdJL1 = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJL1].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJL1.JL1BoxFinal = convertJL1[indexLastnonzeroJL1];
                    dbDailyUpdJL1.JL1Rp = convertJL1[indexLastnonzeroJL1] / TotalDibayarJL1.Value * totalRupiahJL1;
                    dbDailyUpdJL1.UpdatedDate = DateTime.Now;
                    dbDailyUpdJL1.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJL1);
                }
                else if (TotalProdJl1.Value < convertJL1.Sum())
                {
                    convertJL1[indexLastnonzeroJL1] -= Math.Round((convertJL1.Sum() - TotalProdJl1.Value), 5);
                    var dbDailyUpdJL1 = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJL1].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJL1.JL1BoxFinal = convertJL1[indexLastnonzeroJL1];
                    dbDailyUpdJL1.JL1Rp = convertJL1[indexLastnonzeroJL1] / TotalDibayarJL1.Value * totalRupiahJL1;
                    dbDailyUpdJL1.UpdatedDate = DateTime.Now;
                    dbDailyUpdJL1.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJL1);
                }
            }
            
            // JL2
            if (indexLastnonzeroJL2 >= 0)
            {
                if (TotalProdJl2.Value > convertJL2.Sum())
                {
                    convertJL2[indexLastnonzeroJL2] += Math.Round((TotalProdJl2.Value - convertJL2.Sum()), 5);
                    var dbDailyUpdJL2 = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJL2].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJL2.JL2BoxFinal = convertJL2[indexLastnonzeroJL2];
                    dbDailyUpdJL2.JL2Rp = convertJL2[indexLastnonzeroJL2] / TotalDibayarJL2.Value * totalRupiahJL2;
                    dbDailyUpdJL2.UpdatedDate = DateTime.Now;
                    dbDailyUpdJL2.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJL2);
                }
                else if (TotalProdJl2.Value < convertJL2.Sum())
                {
                    convertJL2[indexLastnonzeroJL2] -= Math.Round((convertJL2.Sum() - TotalProdJl2.Value), 5);
                    var dbDailyUpdJL2 = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJL2].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJL2.JL2BoxFinal = convertJL2[indexLastnonzeroJL2];
                    dbDailyUpdJL2.JL2Rp = convertJL2[indexLastnonzeroJL2] / TotalDibayarJL2.Value * totalRupiahJL2;
                    dbDailyUpdJL2.UpdatedDate = DateTime.Now;
                    dbDailyUpdJL2.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJL2);
                }
            }
            
            // JL3
            if (indexLastnonzeroJL3 >= 0)
            {
                if (TotalProdJl3.Value > convertJL3.Sum())
                {
                    convertJL3[indexLastnonzeroJL3] += Math.Round((TotalProdJl3.Value - convertJL3.Sum()), 5);
                    var dbDailyUpdJL3 = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJL3].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJL3.JL3BoxFinal = convertJL3[indexLastnonzeroJL3];
                    dbDailyUpdJL3.JL3Rp = convertJL3[indexLastnonzeroJL3] / TotalDibayarJL3.Value * totalRupiahJL3;
                    dbDailyUpdJL3.UpdatedDate = DateTime.Now;
                    dbDailyUpdJL3.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJL3);
                }
                else if (TotalProdJl3.Value < convertJL3.Sum())
                {
                    convertJL3[indexLastnonzeroJL3] -= Math.Round((convertJL3.Sum() - TotalProdJl3.Value), 5);
                    var dbDailyUpdJL3 = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJL3].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJL3.JL3BoxFinal = convertJL3[indexLastnonzeroJL3];
                    dbDailyUpdJL3.JL3Rp = convertJL3[indexLastnonzeroJL3] / TotalDibayarJL3.Value * totalRupiahJL3;
                    dbDailyUpdJL3.UpdatedDate = DateTime.Now;
                    dbDailyUpdJL3.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJL3);
                }
            }
            
            // JL4
            if (indexLastnonzeroJL4 >= 0)
            {
                if (TotalProdJl4.Value > convertJL4.Sum())
                {
                    convertJL4[indexLastnonzeroJL4] += Math.Round((TotalProdJl4.Value - convertJL4.Sum()), 3);
                    var dbDailyUpdJL4 = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJL4].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJL4.JL4BoxFinal = convertJL4[indexLastnonzeroJL4];
                    dbDailyUpdJL4.JL4Rp = convertJL4[indexLastnonzeroJL4] / TotalDibayarJL4.Value * totalRupiahJL4;
                    dbDailyUpdJL4.UpdatedDate = DateTime.Now;
                    dbDailyUpdJL4.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJL4);
                }
                else if (TotalProdJl4.Value < convertJL4.Sum())
                {
                    convertJL4[indexLastnonzeroJL4] -= Math.Round((convertJL4.Sum() - TotalProdJl4.Value), 3);
                    var dbDailyUpdJL4 = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJL4].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    dbDailyUpdJL4.JL4BoxFinal = convertJL4[indexLastnonzeroJL4];
                    dbDailyUpdJL4.JL4Rp = convertJL4[indexLastnonzeroJL4] / TotalDibayarJL4.Value * totalRupiahJL4;
                    dbDailyUpdJL4.UpdatedDate = DateTime.Now;
                    dbDailyUpdJL4.UpdatedBy = userName;
                    _tpoFeeProductionDailyRepo.Update(dbDailyUpdJL4);
                }
            }

            //if (indexLastnonzeroJKN >= 0)
            //{
            //    var dbDailyUpd = _tpoFeeProductionDailyRepo.GetByID(tpoFeeCode, DateTime.ParseExact(daily[indexLastnonzeroJKN].FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
            //    dbDailyUpd.JKNBoxFinal = convertJKN[indexLastnonzeroJKN];
            //    if (indexLastnonzeroJL1 >= 0) dbDailyUpd.JL1BoxFinal = convertJL1[indexLastnonzeroJL1];
            //    if (indexLastnonzeroJL2 >= 0) dbDailyUpd.JL2BoxFinal = convertJL2[indexLastnonzeroJL2];
            //    if (indexLastnonzeroJL3 >= 0) dbDailyUpd.JL3BoxFinal = convertJL3[indexLastnonzeroJL3];
            //    if (indexLastnonzeroJL4 >= 0) dbDailyUpd.JL4BoxFinal = convertJL4[indexLastnonzeroJL4];
            //    dbDailyUpd.JKNRp = convertJKN[indexLastnonzeroJKN] / TotalDibayarJKN.Value * totalRupiahJKN;
            //    if (indexLastnonzeroJL1 >= 0) dbDailyUpd.JL1Rp = convertJL1[indexLastnonzeroJL1] / TotalDibayarJL1.Value * totalRupiahJL1;
            //    if (indexLastnonzeroJL2 >= 0) dbDailyUpd.JL2Rp = convertJL2[indexLastnonzeroJL2] / TotalDibayarJL2.Value * totalRupiahJL2;
            //    if (indexLastnonzeroJL3 >= 0) dbDailyUpd.JL3Rp = convertJL3[indexLastnonzeroJL3] / TotalDibayarJL3.Value * totalRupiahJL3;
            //    if (indexLastnonzeroJL4 >= 0) dbDailyUpd.JL4Rp = convertJL4[indexLastnonzeroJL4] / TotalDibayarJL4.Value * totalRupiahJL4;
            //    dbDailyUpd.UpdatedDate = DateTime.Now;
            //    dbDailyUpd.UpdatedBy = userName;
            //    _tpoFeeProductionDailyRepo.Update(dbDailyUpd);
            //}
            
            _uow.SaveChanges();
        }

        public bool CheckBtnSave(string tpoFeeCode)
        {
            var dbTpoFeeCode = GetTpoFeeHdrById(tpoFeeCode);
            if (dbTpoFeeCode == null) return false;
            var eblekTpoVer = _executionTpobll.GeTpoProductionEntryVerifications(new GetExeTPOProductionEntryVerificationInput()
            {
                LocationCode = dbTpoFeeCode.LocationCode,
                BrandCode = "",
                KPSWeek = dbTpoFeeCode.KPSWeek,
                KPSYear = dbTpoFeeCode.KPSYear
            }).OrderByDescending(m => m.ProcessOrder).ToList().FirstOrDefault();
            if (eblekTpoVer == null) return false;

            var eblekTpoVerification = _executionTpobll.GeTpoProductionEntryVerifications(new GetExeTPOProductionEntryVerificationInput()
            {
                LocationCode = dbTpoFeeCode.LocationCode,
                BrandCode = "",
                KPSWeek = dbTpoFeeCode.KPSWeek,
                KPSYear = dbTpoFeeCode.KPSYear
            }).Where(m => m.ProcessGroup == eblekTpoVer.ProcessGroup);

            var total = eblekTpoVerification.Sum(m => m.TotalTPKValue);

            return eblekTpoVerification.Sum(m => m.TotalTPKValue) == dbTpoFeeCode.TotalProdStick;
        }

        public TPOFeeHdrDTO SaveTpoFeeHdr(string tpoFeeHrd, string taxtNoProd, string taxtNoMgmt)
        {
            var dbTpoFeeHdr = _tpoFeeHdrRepo.GetByID(tpoFeeHrd);
            if (dbTpoFeeHdr == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            dbTpoFeeHdr.TaxtNoProd = taxtNoProd;
            dbTpoFeeHdr.TaxtNoMgmt = taxtNoMgmt;
            _tpoFeeHdrRepo.Update(dbTpoFeeHdr);

            _uow.SaveChanges();

            return Mapper.Map<TPOFeeHdrDTO>(dbTpoFeeHdr);
        }

        #endregion

        #region TPO Fee Plan

        public List<TpoFeePlanViewDto> GetTpoFeePlanView(GetTPOFeeHdrPlanInput input)
        {
            var mstGenLocInput = new GetMstGenLocationsByParentCodeInput()
            {
                ParentLocationCode = input.ParentLocationCode
            };

            var locations = _masterDataBll.GetMstGenLocationsByParentCode(mstGenLocInput).Select(s => s.LocationCode).ToList();

            var queryFilter = PredicateHelper.True<TpoFeePlanView>();

            if (locations != null)
            {
                queryFilter = queryFilter.And(p => locations.Contains(p.LocationCode));
            }

            if (input.KpsYear.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSYear == input.KpsYear);
            }

            if (input.KpsWeek.HasValue)
            {
                queryFilter = queryFilter.And(p => p.KPSWeek == input.KpsWeek);
            }

            var dbResult = _tpoFeePlanViewRepo.Get(queryFilter);

            var resultMapping = MappingTpoFeePlanView(dbResult.ToList());

            return resultMapping;
        }

        private List<TpoFeePlanViewDto> MappingTpoFeePlanView(List<TpoFeePlanView> input)
        {
            var datas = input.OrderBy(p => p.TPOFeeCode);

            List<TpoFeePlanViewDto> resultList = new List<TpoFeePlanViewDto>();
            TpoFeePlanViewDto result;
            string tpoFeeCode = string.Empty;

            foreach (var data in datas)
            {
                if (tpoFeeCode != data.TPOFeeCode)
                {
                    tpoFeeCode = data.TPOFeeCode;

                    result = new TpoFeePlanViewDto();
                    result.TpoFeeCode = data.TPOFeeCode;
                    result.LocationCode = data.LocationCode;
                    result.LocationName = data.LocationName;
                    result.SktBrandCode = data.SKTBrandCode;
                    result.JknBox =
                        input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.JknBox) && p.TPOFeeCode == tpoFeeCode)
                            .Select(p => p.Calculate)
                            .FirstOrDefault();
                    result.Jl1Box =
                        input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.Jl1Box) && p.TPOFeeCode == tpoFeeCode)
                            .Select(p => p.Calculate)
                            .FirstOrDefault();
                    result.Jl2Box =
                       input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.Jl2Box) && p.TPOFeeCode == tpoFeeCode)
                           .Select(p => p.Calculate)
                           .FirstOrDefault();
                    result.Jl3Box =
                       input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.Jl3Box) && p.TPOFeeCode == tpoFeeCode)
                           .Select(p => p.Calculate)
                           .FirstOrDefault();
                    result.Jl4Box =
                       input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.Jl4Box) && p.TPOFeeCode == tpoFeeCode)
                           .Select(p => p.Calculate)
                           .FirstOrDefault();
                    result.ProductionCost =
                       input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.ProductionCost) && p.TPOFeeCode == tpoFeeCode)
                           .Select(p => p.Calculate)
                           .FirstOrDefault();
                    result.MaklonFee =
                       input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.MaklonFee) && p.TPOFeeCode == tpoFeeCode)
                           .Select(p => p.Calculate)
                           .FirstOrDefault();
                    result.ProductivityIncentives =
                        input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.ProductivityIncentives) && p.TPOFeeCode == tpoFeeCode)
                           .Select(p => p.Calculate)
                           .FirstOrDefault();
                    result.MaklonFeeTwoPercent = result.MaklonFee * 2 / 100;
                    result.ProductivityIncentivesTwoPercent = result.ProductivityIncentives * 2 / 100;
                    result.ProductionCostTenPercent = result.ProductionCost * 10 / 100;
                    result.MaklonFeeTenPercent = result.MaklonFee * 10 / 100;
                    // kosongin dulu
                    //result.ProductivityIncentivesTenPercent = result.ProductivityIncentives * 10 / 100;
                    result.ProductivityIncentivesTenPercent = null;
                    result.TotalCost =
                        input.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.TotalCost) && p.TPOFeeCode == tpoFeeCode)
                           .Select(p => p.Calculate)
                           .FirstOrDefault();

                    resultList.Add(result);
                }

            }

            return resultList;

        }

        public TpoFeeHdrPlanDto GetTpoFeeHdrPlan(string id)
        {
            var dbResult = _tpoFeeHdrPlanRepo.GetByID(id);

            return Mapper.Map<TpoFeeHdrPlanDto>(dbResult);
        }

        #endregion

        #region TPO Fee Production Daily
        public List<TPOFeeProductionDailyDTO> GetTpoFeeProductionDailys(string tpoFeeCode)
        {
            var dbTpoFeeProdDailys = _tpoFeeProductionDailyRepo.Get(m => m.TPOFeeCode == tpoFeeCode).OrderBy(m => m.FeeDate);
            return Mapper.Map<List<TPOFeeProductionDailyDTO>>(dbTpoFeeProdDailys);
        }
        #endregion

        #region TPO Fee Calculation
        public List<TPOFeeCalculationDTO> GetTpoFeeCalculation(string tpoFeeCode)
        {
            var dbTpoFeeProdDailys = _tpoFeeCalculationRepo.Get(m => m.TPOFeeCode == tpoFeeCode).OrderBy(m => m.OrderFeeType);
            var result = Mapper.Map<List<TPOFeeCalculationDTO>>(dbTpoFeeProdDailys);
            var totalBayarRow = result.Where(m => m.OrderFeeType == 15).ToList();
            result = result.Where(m => m.OrderFeeType != 15).ToList();
            result.AddRange(totalBayarRow);

            var resultTPOFee = new List<TPOFeeCalculationDTO>();

            foreach (var tpoFeeCalculationDto in result)
            {
                if (tpoFeeCalculationDto.OrderFeeType == 4 || tpoFeeCalculationDto.OrderFeeType == 5)
                {
                    if (tpoFeeCalculationDto.OutputProduction > 0)
                    {
                        resultTPOFee.Add(tpoFeeCalculationDto);
                    }
                }
                else
                {
                    if (tpoFeeCalculationDto.OrderFeeType != 16 && tpoFeeCalculationDto.OrderFeeType != 17 &&
                        tpoFeeCalculationDto.OrderFeeType != 18)
                    {
                        tpoFeeCalculationDto.OutputProduction = (float) Math.Round((Decimal)tpoFeeCalculationDto.OutputProduction, 2, MidpointRounding.AwayFromZero);
                        resultTPOFee.Add(tpoFeeCalculationDto);
                    }
                }

            }

            return resultTPOFee;
        }

        public List<TPOFeeCalculationPlanDto> GetTpoFeeCalculationPlan(string tpoFeeCode)
        {
            var dbTpoFeeProdDailys = _tpoFeeCalculationPlanRepo.Get(m => m.TPOFeeCode == tpoFeeCode).OrderBy(m => m.OrderFeeType);
            var result = Mapper.Map<List<TPOFeeCalculationPlanDto>>(dbTpoFeeProdDailys);
            var totalBayarRow = result.Where(m => m.OrderFeeType == 15).ToList();
            result = result.Where(m => m.OrderFeeType != 15).ToList();
            result.AddRange(totalBayarRow);

            var resultTPOFee = new List<TPOFeeCalculationPlanDto>();

            foreach (var tpoFeeCalculationDto in result)
            {
                if (tpoFeeCalculationDto.OrderFeeType == 4 || tpoFeeCalculationDto.OrderFeeType == 5)
                {
                    if (tpoFeeCalculationDto.OutputProduction > 0)
                    {
                        resultTPOFee.Add(tpoFeeCalculationDto);
                    }
                }
                else
                {
                    if (tpoFeeCalculationDto.OrderFeeType != 16 && tpoFeeCalculationDto.OrderFeeType != 17 && tpoFeeCalculationDto.OrderFeeType != 18)
                        resultTPOFee.Add(tpoFeeCalculationDto);
                }

            }

            return resultTPOFee;
        }
        #endregion

        public string GetUserAdByRoleLocation(string role, string location)
        {
            var result = "";
            var datas = _sqlSPRepo.GetUserAdByRoleLocation(role, location).FirstOrDefault();
            if (datas != null)
            {
                result = datas.UserAD;
            }
            return result;
        }

        #region TPO Fee Approval
        public List<TPOFeeApprovalViewDTO> GetTpoFeeApprovals(GetTPOFeeExeActualInput input)
        {
            var queryFilter = PredicateHelper.True<TPOFeeApprovalView>();

            if (!string.IsNullOrEmpty(input.Regional) && input.Regional != HMS.SKTIS.Core.Enums.LocationCode.TPO.ToString())
            {
                var locations = _masterDataBll.GetAllLocationByLocationCode(input.Regional, 1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locations.Contains(m.LocationCode));
            }

            if (input.Year > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);

            if (input.Week > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.Week);

            var list = new List<string>() { "SUBMITTED", "APPROVED", "AUTHORIZED" };
            var SLTStatus = new List<string>() { "DRAFT", "REVISED" };

            // Check if pajak != null
            queryFilter = queryFilter.And(m => (
                SLTStatus.Contains(m.Status) && !string.IsNullOrEmpty(m.TaxtNoMgmt) && !string.IsNullOrEmpty(m.TaxtNoProd)
            ) || list.Contains(m.Status));

            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];

            queryFilter = queryFilter.And(m => m.DestinationRole == session.Responsibility.Role);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TPOFeeApprovalView>();
            var dbResult = _tpoFeeApprovalRepo.Get(queryFilter, orderByFilter)
                .OrderBy(m => m.ParentLocationCode)
                .ThenBy(m => m.LocationCode)
                .ThenBy(m => m.SKTBrandCode);

            //var allLocations = _masterDataBll.GetAllLocations(new GetAllLocationsInput(), false);

            //// to order by missing field parent location code
            //var joinQuery = from res in dbResult
            //    join loc in allLocations on res.LocationCode equals loc.LocationCode
            //    orderby loc.ParentLocationCode ascending, res.LocationCode ascending, res.SKTBrandCode ascending 
            //    select new TPOFeeApprovalViewDTO()
            //    {
            //        LocationCode = res.LocationCode,
            //        LocationName = loc.LocationName,
            //        ParentLocationCode = loc.ParentLocationCode,
            //        SKTBrandCode = res.SKTBrandCode,
            //        Note = res.Note,
            //        JKN = res.JKN,
            //        JL1 = res.JL1,
            //        JL2 = res.JL2,
            //        JL3 = res.JL3,
            //        JL4 = res.JL4,
            //        BiayaProduksi = res.BiayaProduksi,
            //        JasaManajemen = res.JasaManajemen,
            //        ProductivityIncentives = res.ProductivityIncentives,
            //        JasaManajemen2Percent = res.JasaManajemen2Percent,
            //        ProductivityIncentives2Percent = res.ProductivityIncentives2Percent,
            //        BiayaProduksi10Percent = res.BiayaProduksi10Percent,
            //        JasaMakloon10Percent = res.JasaMakloon10Percent,
            //        ProductivityIncentives10Percent = res.ProductivityIncentives10Percent,
            //        TotalBayar = res.TotalBayar,
            //        KPSYear = res.KPSYear,
            //        KPSWeek = res.KPSWeek,
            //        IDFlow = res.IDFlow,
            //        TPOFeeCode = res.TPOFeeCode,
            //        Status = res.Status,
            //        TaxtNoMgmt = res.TaxtNoMgmt,
            //        TaxtNoProd = res.TaxtNoProd
            //    };

            var result = Mapper.Map<List<TPOFeeApprovalViewDTO>>(dbResult);
            result.ForEach(m => m.AlreadyApprove = _utilitiesBll.CheckDataAlreadyOnTransactionLog(m.TPOFeeCode, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Approve.ToString()));
            
            return result;
        }
        #endregion
        //hakim
        #region TPO Fee AP Open 
        public List<TPOFeeAPOpenDTO> GetTpoFeeAPOpen(GetTPOFeeAPOpenInput input)
        {
            var queryFilter = PredicateHelper.True<TPOFeeApprovalView>();

            if (!string.IsNullOrEmpty(input.Regional) && input.Regional != HMS.SKTIS.Core.Enums.LocationCode.TPO.ToString())
            {
                var locations = _masterDataBll.GetAllLocationByLocationCode(input.Regional, 1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locations.Contains(m.LocationCode));
            }

            if (input.Year > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);

            if (input.Week > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.Week);

            queryFilter = queryFilter.And(m => m.Status == "AUTHORIZED");

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TPOFeeApprovalView>();
            var dbResult = _tpoFeeApprovalRepo.Get(queryFilter, orderByFilter);

            var result = Mapper.Map<List<TPOFeeAPOpenDTO>>(dbResult);
            //result.ForEach(m => m.AlreadyApprove = _utilitiesBll.CheckDataAlreadyOnTransactionLog(m.TPOFeeCode, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Approve.ToString()));
            return result;
        }

        public List<TPOFeeAPOpenDTO> GetCompletedTpoFeeAPOpen(GetTPOFeeAPOpenInput input)
        {
            var queryFilter = PredicateHelper.True<TPOFeeApprovalView>();

            if (!string.IsNullOrEmpty(input.Regional) && input.Regional != HMS.SKTIS.Core.Enums.LocationCode.TPO.ToString())
            {
                var locations = _masterDataBll.GetAllLocationByLocationCode(input.Regional, 1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locations.Contains(m.LocationCode));
            }

            if (input.Year > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);

            if (input.Week > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.Week);

            queryFilter = queryFilter.And(m => m.Status == "COMPLETED");

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TPOFeeApprovalView>();
            var dbResult = _tpoFeeApprovalRepo.Get(queryFilter, orderByFilter);

            var result = Mapper.Map<List<TPOFeeAPOpenDTO>>(dbResult);
            //result.ForEach(m => m.AlreadyApprove = _utilitiesBll.CheckDataAlreadyOnTransactionLog(m.TPOFeeCode, Enums.PageName.ApprovalPage.ToString(), Enums.ButtonName.Approve.ToString()));
            return result;
        }

        public List<GenerateP1TemplateAPDTO> GetP1TemplateAP(GetTPOFeeAPOpenInput input)
        {
            var dbResult = _sqlSPRepo.GenerateP1TemplateAP(input.Regional, input.Week, input.Year);
            return Mapper.Map<List<GenerateP1TemplateAPDTO>>(dbResult);
        }

        public List<TPOGenerateP1TemplateViewDTO> GetP1Template(GetTPOFeeAPOpenInput input)
        {
            var queryFilter = PredicateHelper.True<TPOGenerateP1TemplateView>();

            if (!string.IsNullOrEmpty(input.Regional) && input.Regional != HMS.SKTIS.Core.Enums.LocationCode.TPO.ToString())
            {
                var locations = _masterDataBll.GetAllLocationByLocationCode(input.Regional, 1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locations.Contains(m.LocationCode));
            }

            if (input.Year > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);

            if (input.Week > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.Week);
            // add state by this ticket http://tp.voxteneo.co.id/entity/10980
            queryFilter = queryFilter.And(m => m.Status == "COMPLETED" || m.Status == "AUTHORIZED" || m.Status == "END"); 

            
            var dbResult = _tpoGenerateP1TemplateView.Get(queryFilter).OrderBy(m=> m.TPOFeeCode);
            var result = Mapper.Map<List<TPOGenerateP1TemplateViewDTO>>(dbResult);
            return result;
        }
        #endregion

        #region TPO Fee AP Close
        public List<TPOFeeAPCloseDTO> GetTpoFeeAPClose(GetTPOFeeAPCloseInput input)
        {
            var queryFilter = PredicateHelper.True<TPOFeeApprovalView>();

            if (!string.IsNullOrEmpty(input.Regional) && input.Regional != HMS.SKTIS.Core.Enums.LocationCode.TPO.ToString())
            {
                var locations = _masterDataBll.GetAllLocationByLocationCode(input.Regional, 1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locations.Contains(m.LocationCode));
            }

            if (input.Year > 0)
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);

            if (input.Week > 0)
                queryFilter = queryFilter.And(m => m.KPSWeek == input.Week);

            queryFilter = queryFilter.And(m => m.Status == "END" || m.Status ==  "COMPLETED");

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TPOFeeApprovalView>();
            var dbResult = _tpoFeeApprovalRepo.Get(queryFilter, orderByFilter);

            var result = Mapper.Map<List<TPOFeeAPCloseDTO>>(dbResult);
            return result;
        }
        #endregion

        #region TPO Reports Package
        //public List<TPOFeeReportsPackageCompositeDTO> GetTpoFeeReportsPackage(GetTPOReportsPackageInput input)
        //{
        //    var queryFilter = PredicateHelper.True<MstTPOPackage>();
            
        //    if (input.Year > 0)
        //    {
        //        queryFilter = queryFilter.And(m => m.EffectiveDate.Year == input.Year);
        //    }

        //    input.SortExpression = "LocationCode";
        //    input.SortExpression2 = "BrandGroupCode";

        //    var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
        //    var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOPackage>();
        //    var dbResult = _mstTPOPackageRepo.Get(queryFilter, orderByFilter);

        //    var listLocation = _mstGenLocationRepo.Get();

        //    var listWeek = _mstGenWeekRepo.Get(c => c.Year == input.Year);
        //    int maxWeek = listWeek.Count();

        //    var listResult = new List<TPOFeeReportsPackageCompositeDTO>();

        //    foreach (var mstTpoPackage in dbResult)
        //    {
        //        var result = new TPOFeeReportsPackageCompositeDTO();

        //        result.LocationCode = mstTpoPackage.LocationCode;
        //        var location = listLocation.FirstOrDefault(c => c.LocationCode == mstTpoPackage.LocationCode);
        //        if (location != null)
        //            result.LocationName = location.LocationName;
        //        result.BrandCode = mstTpoPackage.BrandGroupCode;
        //        result.MemoReff = mstTpoPackage.MemoRef;

        //        result.Year = input.Year;
        //        result.MaxWeek = maxWeek;


        //        var startWeek = listWeek.FirstOrDefault(c => c.StartDate <= mstTpoPackage.EffectiveDate && c.EndDate >= mstTpoPackage.EffectiveDate);

        //        var endWeek = listWeek.FirstOrDefault(c => c.StartDate <= mstTpoPackage.ExpiredDate && c.EndDate >= mstTpoPackage.ExpiredDate);

        //        int iStartWeek = 0;
        //        int iEndWeek = 0;

        //        if (startWeek != null)
        //            iStartWeek = startWeek.Week.HasValue ? startWeek.Week.Value : 0;

        //        if (endWeek != null)
        //            iEndWeek = endWeek.Week.HasValue ? endWeek.Week.Value : 0;

        //        result.StartWeek = iStartWeek;
        //        result.EndWeek = iEndWeek;

        //        for (int i = 1; i <= maxWeek; i++)
        //        {
                    
        //            var weekValue = new TPOFeeReportsPackageWeeklyDTO();

        //            if (i >= iStartWeek && i <= iEndWeek)
        //            {
        //                weekValue.StartWeek = iStartWeek;
        //                weekValue.EndWeek = iEndWeek;
        //                weekValue.PackageValue = Math.Round(mstTpoPackage.Package,2);
        //            }
        //            else
        //            {
        //                weekValue.StartWeek = 0;
        //                weekValue.EndWeek = 0;
        //                weekValue.PackageValue = 0;
        //            }

        //            result.ListWeekValue.Add(weekValue);
        //        }

        //        listResult.Add(result);
        //    }

        //    return listResult;
        //}

        public List<TPOFeeReportsPackageCompositeDTO> GetTpoFeeReportsPackageNew(int year) 
        {
            var listResult = new List<TPOFeeReportsPackageCompositeDTO>();

            var queryFilter = PredicateHelper.True<MstTPOPackage>();

            queryFilter = queryFilter.And(m => m.EffectiveDate.Year == year);

            var dbResult = _mstTPOPackageRepo.Get(queryFilter).ToList().OrderBy(c => c.LocationCode);

            var listGroupBy = dbResult.GroupBy(c => new {
                c.LocationCode,
                c.BrandGroupCode
            }).Select(x => new TPOFeeReportsPackageCompositeDTO() {
                LocationCode = x.Key.LocationCode,
                BrandCode = x.Key.BrandGroupCode
            });

            //get master brand
            var listBrandGroup = _masterDataBll.GetAllBrandGroups();
            var listLocation = _mstGenLocationRepo.Get();

            var listWeek = _mstGenWeekRepo.Get(c => c.Year == year);
            int maxWeek = listWeek.Count();

            foreach (var dtoComposite in listGroupBy) 
            {
                dtoComposite.Year = year;
                
                dtoComposite.LocationName = listLocation.Where(c => c.LocationCode == dtoComposite.LocationCode).Select(c => c.LocationName).FirstOrDefault();
                dtoComposite.MaxWeek = maxWeek;

                var listData = dbResult.Where(c => c.LocationCode == dtoComposite.LocationCode &&
                                                      c.BrandGroupCode == dtoComposite.BrandCode).ToList();

                foreach (var memoref in listData.Select(c => c.MemoRef).Distinct()) 
                {
                    dtoComposite.MemoReff += memoref + "; ";
                }

                dtoComposite.BrandCode = listBrandGroup.Where(c => c.BrandGroupCode == dtoComposite.BrandCode).Select(c => c.SKTBrandCode).FirstOrDefault();

                foreach (var dataPerWeek in listData)
                {
                    var listWeekMst = _mstGenWeekRepo.Get(c => c.StartDate >= dataPerWeek.EffectiveDate && c.EndDate <= dataPerWeek.ExpiredDate && c.Year == year).OrderBy(c => c.Week).Select(c => c.Week).ToList();

                    if(dtoComposite.ListWeekValue.Count == 0)
                        for (int i = 1; i <= maxWeek; i++) dtoComposite.ListWeekValue.Add(new TPOFeeReportsPackageWeeklyDTO());

                    foreach (var week in listWeekMst) {
                        dtoComposite.ListWeekValue[week.Value == 0 ? 0 : week.Value - 1].PackageValue = dataPerWeek.Package.ToString("n2");
                        dtoComposite.ListWeekValue[week.Value == 0 ? 0 : week.Value - 1].PackageInFloat = dataPerWeek.Package;
                    }
                }

                for (int i = 0; i < maxWeek; i++) 
                {
                    var val = dtoComposite.ListWeekValue[i].PackageInFloat;
                    if (i != maxWeek - 1)
                    {
                        if (dtoComposite.ListWeekValue[i + 1].PackageInFloat > val || dtoComposite.ListWeekValue[i + 1].PackageInFloat < val)
                        {
                            if (!String.IsNullOrEmpty(dtoComposite.ListWeekValue[i+1].PackageValue))
                                if (!String.IsNullOrEmpty(dtoComposite.ListWeekValue[i].PackageValue))
                                    dtoComposite.ListWeekValue[i + 1].IsChangedValue = true;
                        }
                    }
                }

                    listResult.Add(dtoComposite);
            }

            return listResult;
        }

        public List<TPOFeeReportsPackageCompositeDTO> GetTpoFeeReportsPackage(GetTPOReportsPackageInput input)
        {
            var queryFilter = PredicateHelper.True<MstTPOPackage>();

            if (input.Year > 0)
            {
                queryFilter = queryFilter.And(m => m.EffectiveDate.Year == input.Year);
            }

            input.SortExpression = "LocationCode";
            input.SortExpression2 = "BrandGroupCode";

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOPackage>();
            var dbResult = _mstTPOPackageRepo.Get(queryFilter, orderByFilter);

            var listLocation = _mstGenLocationRepo.Get();

            var listWeek = _mstGenWeekRepo.Get(c => c.Year == input.Year);
            int maxWeek = listWeek.Count();

            var listResult = new List<TPOFeeReportsPackageCompositeDTO>();

            var listGroupBy = dbResult.GroupBy(c => new
            {
                c.LocationCode,
                c.BrandGroupCode
            }).Select(x => new TPOFeeReportsPackageCompositeDTO()
            {
                LocationCode = x.Key.LocationCode,
                BrandCode = x.Key.BrandGroupCode
            });

            //get master brand
            var listBrandGroup = _masterDataBll.GetAllBrandGroups();

            foreach (var tpoFeeReportsPackageCompositeDto in listGroupBy)
            {

                var listData =
                    dbResult.Where(
                        c =>
                            c.LocationCode == tpoFeeReportsPackageCompositeDto.LocationCode &&
                            c.BrandGroupCode == tpoFeeReportsPackageCompositeDto.BrandCode);

                int iFound = 0;
                double oldPackageValue = 0;
                
                var result = new TPOFeeReportsPackageCompositeDTO();

                foreach (var mstTpoPackage in listData)
                {
                   
                    iFound++;

                    result.LocationCode = mstTpoPackage.LocationCode;
                    var location = listLocation.FirstOrDefault(c => c.LocationCode == mstTpoPackage.LocationCode);
                    if (location != null)
                        result.LocationName = location.LocationName;

                    var brand =
                        listBrandGroup.Where(c => c.BrandGroupCode == mstTpoPackage.BrandGroupCode)
                            .Select(c => c.SKTBrandCode)
                            .FirstOrDefault();

                    result.BrandCode = brand;
                    result.MemoReff = mstTpoPackage.MemoRef;

                    result.Year = input.Year;
                    result.MaxWeek = maxWeek;

                    var startWeek = listWeek.FirstOrDefault(c => c.StartDate <= mstTpoPackage.EffectiveDate && c.EndDate >= mstTpoPackage.EffectiveDate);

                    var endWeek = listWeek.FirstOrDefault(c => c.StartDate <= mstTpoPackage.ExpiredDate && c.EndDate >= mstTpoPackage.ExpiredDate);

                    int iStartWeek = 0;
                    int iEndWeek = 0;

                    if (startWeek != null)
                        iStartWeek = startWeek.Week.HasValue ? startWeek.Week.Value : 0;

                    if (endWeek != null)
                        iEndWeek = endWeek.Week.HasValue ? endWeek.Week.Value : 0;

                    result.StartWeek = iStartWeek;
                    result.EndWeek = iEndWeek;

                    
                    for (int i = 1; i <= maxWeek; i++)
                    {
                        if (result.ListWeekValue.Count < maxWeek)
                        {

                            if (iFound > 1)
                            {

                                iFound = 0;
                                //weekValue.IsChangedValue = true;
                                //update the first with
                                result.ListWeekValue[iStartWeek].IsChangedValue = true;
                                result.ListWeekValue[iStartWeek].StartWeek = iStartWeek;
                                result.ListWeekValue[iStartWeek].EndWeek = iEndWeek;
                                if (result.ListWeekValue[iStartWeek].PackageValue !=
                                    Math.Round(mstTpoPackage.Package, 2).ToString())
                                    result.ListWeekValue[iStartWeek].PackageValue =
                                        GenericHelper.ConvertDoubleToString2FormatDecimal(
                                            Math.Round(mstTpoPackage.Package, 2));

                                //update
                                for (int j = iStartWeek + 1; j < iEndWeek; j++)
                                {
                                    result.ListWeekValue[j].StartWeek = iStartWeek;
                                    result.ListWeekValue[j].EndWeek = iEndWeek;
                                    result.ListWeekValue[j].PackageValue =
                                        GenericHelper.ConvertDoubleToString2FormatDecimal(
                                            Math.Round(mstTpoPackage.Package, 2));
                                }

                                //exit loop
                                break;
                            }
                            else
                            {
                                var weekValue = new TPOFeeReportsPackageWeeklyDTO();
                                if (i >= iStartWeek && i <= iEndWeek)
                                {
                                    weekValue.StartWeek = iStartWeek;
                                    weekValue.EndWeek = iEndWeek;
                                    weekValue.PackageValue =
                                        GenericHelper.ConvertDoubleToString2FormatDecimal(
                                            Math.Round(mstTpoPackage.Package, 2));
                                }
                                //else
                                //{
                                //    weekValue.StartWeek = 0;
                                //    weekValue.EndWeek = 0;
                                //    weekValue.PackageValue = 0;
                                //}

                                result.ListWeekValue.Add(weekValue);
                            }
                        }

                    }

                
                }

                listResult.Add(result);
            }
          

            return listResult;
        }

        
        #endregion



        #region TPO Reports Summary

        public List<TPOFeeReportsSummaryCompositeDTO> GetTpoFeeReportsSummary(GetTPOReportsSummaryInput input)
        {
            var queryFilter = PredicateHelper.True<TPOFeeHdr>();

            if (input.Year > 0)
            {
                queryFilter = queryFilter.And(m => m.KPSYear == input.Year);
            }

            input.SortExpression = "LocationCode";
            input.SortExpression2 = "KPSWeek";

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<TPOFeeHdr>();
            var dbResultHeader = _tpoFeeHdrRepo.Get(queryFilter, orderByFilter);

            var dbResultCalculation  = _tpoFeeCalculationRepo.Get(c => c.KPSYear == input.Year);

            var listLocation = _mstGenLocationRepo.Get();

            var listWeek = _mstGenWeekRepo.Get(c => c.Year == input.Year);
            int maxWeek = listWeek.Count();

            var result = new List<TPOFeeReportsSummaryCompositeDTO>();


            //create group by location from header
            var listGroupByHeaderByLocationWeek = dbResultHeader.GroupBy(c => new
            {
                c.LocationCode,
            }).Select(x => new TPOFeeReportsSummaryGoupByLocationWeek()
            {
                LocationCode = x.Key.LocationCode,
                CountData = x.Count()
            });

         
            //iterate the result
            var listSummaryHeader = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "Summary",
                LocationName = "Summary",
                IsLocation = false,
                IsSummary = true,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = s.Week.HasValue ? s.Week.Value : 0
                }).ToList()
            };

            var listMaxRupiah = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "Max Rupiah",
                LocationName = "Max Rupiah",
                IsSummary = true,
                IsLocation = false,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 0
                }).ToList()
            };

            var listMaxLokasi = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "Max Lokasi",
                LocationName = "Max Lokasi",
                IsSummary = true,
                IsLocation = true,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 0
                }).ToList()
            };

            var listMinRupiah = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "Min Rupiah",
                LocationName = "Min Rupiah",
                IsSummary = true,
                IsLocation = false,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 999999999999
                }).ToList()
            };

            var listMinLokasi = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "Min Lokasi",
                LocationName = "Min Lokasi",
                IsSummary = true,
                IsLocation = true,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 0
                }).ToList()
            };

            var listReg1 = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "TPO Reg 1",
                LocationName = "TPO Reg 1",
                IsSummary = true,
                IsLocation = false,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 0
                }).ToList()
            };

            var listReg2 = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "TPO Reg 2",
                LocationName = "TPO Reg 2",
                IsSummary = true,
                IsLocation = false,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 0
                }).ToList()
            };

            var listReg3 = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "TPO Reg 3",
                LocationName = "TPO Reg 3",
                IsSummary = true,
                IsLocation = false,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 0
                }).ToList()
            };

            var listReg4 = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "TPO Reg 4",
                LocationName = "TPO Reg 4",
                IsSummary = true,
                IsLocation = false,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 0
                }).ToList()
            };

            var listGrandTotal = new TPOFeeReportsSummaryCompositeDTO()
            {
                ParentLocation = "Grand Total",
                LocationName = "Grand Total",
                IsSummary = true,
                IsLocation = false,
                ListWeekValue = listWeek.Select(s => new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = s.Week.HasValue ? s.Week.Value : 0,
                    WeekValue = 0
                }).ToList()
            };


            foreach (var tpoFeeReportsSummaryGoupByLocationWeek in listGroupByHeaderByLocationWeek)
            {
                ////create header 
                //result.Add(CreateHeaderTpoFeeSummary(listLocation.ToList(), tpoFeeReportsSummaryGoupByLocationWeek.LocationCode,maxWeek));

                var feeChildResult = CreateChildTpoFeeSummary(listLocation.ToList(),
                    tpoFeeReportsSummaryGoupByLocationWeek.LocationCode, 
                    maxWeek, input.ProductionFeeType, dbResultHeader.ToList(), dbResultCalculation.ToList());


                foreach (TPOFeeReportsSummaryWeeklyDTO x in feeChildResult.ListWeekValue)
                {
                    //isi list max Value
                    if (x.WeekValue >= listMaxRupiah.ListWeekValue.Where(c => c.Week == x.Week).Select(c => c.WeekValue).FirstOrDefault())
                    {
                        TPOFeeReportsSummaryWeeklyDTO max = listMaxRupiah.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                        if (max != null) max.WeekValue = x.WeekValue;

                        TPOFeeReportsSummaryWeeklyDTO maxLokasi = listMaxLokasi.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                        if (maxLokasi != null) maxLokasi.Location = feeChildResult.LocationCode + " - " + feeChildResult.LocationAbbr;
                    }

                    //isi list min Value
                    if (x.WeekValue <= listMinRupiah.ListWeekValue.Where(c => c.Week == x.Week).Select(c => c.WeekValue).FirstOrDefault())
                    {
                        TPOFeeReportsSummaryWeeklyDTO min = listMinRupiah.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                        if (min != null) min.WeekValue = x.WeekValue;

                        TPOFeeReportsSummaryWeeklyDTO minLokasi = listMinLokasi.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                        minLokasi.Location = feeChildResult.LocationCode + " - " + feeChildResult.LocationAbbr;
                    }

                    //isi list Reg1
                    if (feeChildResult.ParentLocationCode == "REG1")
                    {
                        TPOFeeReportsSummaryWeeklyDTO Reg1 = listReg1.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                        if (Reg1 != null) Reg1.WeekValue = Reg1.WeekValue + x.WeekValue;
                    }

                    //isi list Reg2
                    if (feeChildResult.ParentLocationCode == "REG2")
                    {
                        TPOFeeReportsSummaryWeeklyDTO Reg2 = listReg2.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                        if (Reg2 != null) Reg2.WeekValue = Reg2.WeekValue + x.WeekValue;
                    }

                    //isi list Reg3
                    if (feeChildResult.ParentLocationCode == "REG3")
                    {
                        TPOFeeReportsSummaryWeeklyDTO Reg3 = listReg3.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                        if (Reg3 != null) Reg3.WeekValue = Reg3.WeekValue + x.WeekValue;
                    }

                    //isi list Reg4
                    if (feeChildResult.ParentLocationCode == "REG4")
                    {
                        TPOFeeReportsSummaryWeeklyDTO Reg4 = listReg4.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                        if (Reg4 != null) Reg4.WeekValue = Reg4.WeekValue + x.WeekValue;
                    }

                    //isi list grand total
                    TPOFeeReportsSummaryWeeklyDTO gt = listGrandTotal.ListWeekValue.FirstOrDefault(c => c.Week == x.Week);
                    if (gt != null) gt.WeekValue = gt.WeekValue + x.WeekValue;


                }


                result.Add(feeChildResult);
            }

            //var result = listGroupByHeaderByLocationWeek.Select(tpoFeeReportsSummaryGoupByLocationWeek => CreateChildTpoFeeSummary(listLocation.ToList(), tpoFeeReportsSummaryGoupByLocationWeek.LocationCode, maxWeek, input.ProductionFeeType, dbResultHeader.ToList(), dbResultCalculation.ToList())).ToList();

            result = result.OrderBy(c => c.ParentLocationCode).ToList();
            //group by parent code and create header 

            //return CreateGroupByHeaderTpoFeeSummary(result, listLocation.ToList());

            var tpoFeeSummaryList = CreateGroupByHeaderTpoFeeSummary(result, listLocation.ToList());

            tpoFeeSummaryList.Add(listSummaryHeader);
            
            tpoFeeSummaryList.Add(listMinRupiah);
            tpoFeeSummaryList.Add(listMinLokasi);
            tpoFeeSummaryList.Add(listMaxRupiah);
            tpoFeeSummaryList.Add(listMaxLokasi);
            tpoFeeSummaryList.Add(listReg1);
            tpoFeeSummaryList.Add(listReg2);
            tpoFeeSummaryList.Add(listReg3);
            tpoFeeSummaryList.Add(listReg4);
            tpoFeeSummaryList.Add(listGrandTotal);

            

            return tpoFeeSummaryList;

        }

        private List<TPOFeeReportsSummaryCompositeDTO> CreateGroupByHeaderTpoFeeSummary(List<TPOFeeReportsSummaryCompositeDTO> result, 
            List<MstGenLocation> listLocation)
        {
            var groupResult = new List<TPOFeeReportsSummaryCompositeDTO>();
            string parentCode = "";
            foreach (var tpoFeeReportsSummaryCompositeDto in result)
            {
                if (parentCode != tpoFeeReportsSummaryCompositeDto.ParentLocationCode)
                {
                    parentCode = tpoFeeReportsSummaryCompositeDto.ParentLocationCode;
                    
                    int maxWeek = tpoFeeReportsSummaryCompositeDto.MaxWeek;

                    //create header 
                    var feeHdrResult = new TPOFeeReportsSummaryCompositeDTO();
                    feeHdrResult.IsParentRow = true;
                    feeHdrResult.MaxWeek = maxWeek;

                    //get parent location code
                    var parentLocationCode = listLocation.FirstOrDefault(c => c.LocationCode == parentCode);
                    if (parentLocationCode != null)
                    {
                        //get parent desc
                        var parentLocation =
                            listLocation.FirstOrDefault(c => c.LocationCode == parentLocationCode.LocationCode);
                        if (parentLocation != null)
                        {
                            feeHdrResult.ParentLocation = parentLocation.ParentLocationCode + " " +
                                                          parentLocation.LocationName;
                        }

                    }

                    //create list week value
                    for (int i = 1; i <= maxWeek; i++)
                    {
                        feeHdrResult.ListWeekValue.Add(new TPOFeeReportsSummaryWeeklyDTO()
                        {
                            Week = i,
                            IsParentRow = true
                        });
                    }

                    groupResult.Add(feeHdrResult);
                }
                groupResult.Add(tpoFeeReportsSummaryCompositeDto);
            }

            return groupResult;

        }

        //private TPOFeeReportsSummaryCompositeDTO CreateHeaderTpoFeeSummary(List<MstGenLocation> listLocation, string locationCode, int maxWeek)
        //{
        //    //create header 
        //    var feeHdrResult = new TPOFeeReportsSummaryCompositeDTO();
        //    feeHdrResult.IsParentRow = true;
        //    feeHdrResult.MaxWeek = maxWeek;

        //    //get parent location code
        //    var parentLocationCode = listLocation.FirstOrDefault(c => c.LocationCode == locationCode);
        //    if (parentLocationCode != null)
        //    {
        //        //get parent desc
        //        var parentLocation = listLocation.FirstOrDefault(c => c.LocationCode == parentLocationCode.ParentLocationCode);
        //        if (parentLocation != null)
        //        {
        //            feeHdrResult.ParentLocation = parentLocation.ParentLocationCode + " " + parentLocation.LocationName;
        //        }
                
        //    }
           
        //    //create list week value
        //    for (int i = 1; i <= maxWeek; i++)
        //    {
        //        feeHdrResult.ListWeekValue.Add(new TPOFeeReportsSummaryWeeklyDTO()
        //        {
        //            Week = i,
        //            IsParentRow = true
        //        });
        //    }

        //    return feeHdrResult;
        //}

        private TPOFeeReportsSummaryCompositeDTO CreateChildTpoFeeSummary(List<MstGenLocation> listLocation,
            string locationCode,  int maxWeek, Enums.TpoFeeReportsSummaryProductionFeeType inputProductionFeeType,
            List<TPOFeeHdr> dbResultHeader, List<TPOFeeCalculation> dbResultCalculation)
        {
            //create child 
            var feeChildResult = new TPOFeeReportsSummaryCompositeDTO();

            feeChildResult.IsParentRow = false;
            feeChildResult.MaxWeek = maxWeek;
            var location = listLocation.FirstOrDefault(c => c.LocationCode == locationCode);

            if (location != null)
            {
                feeChildResult.LocationCode = location.LocationCode;
                feeChildResult.LocationName = location.LocationName;
                feeChildResult.LocationAbbr = location.ABBR;
                feeChildResult.ParentLocationCode = location.ParentLocationCode;
            }


            //create list week value
            for (int i = 1; i <= maxWeek; i++)
            {
                feeChildResult.ListWeekValue.Add(new TPOFeeReportsSummaryWeeklyDTO()
                {
                    Week = i
                });
            }

            var listHeaderByLocation =
                     dbResultHeader.Where(c => c.LocationCode == locationCode).ToList();


            //group by location code and week again
            var listGroupLocationAndWeek = listHeaderByLocation.GroupBy(c => new
            {
                c.LocationCode,
                c.KPSWeek
            }).Select(x => new TPOFeeReportsSummaryGoupByLocationWeek()
            {
                LocationCode = x.Key.LocationCode,
                Week = x.Key.KPSWeek,
                CountData = x.Count()
            });

            foreach (var tpoFeeReportsSummaryGoupByLocationWeek in listGroupLocationAndWeek)
            {

                var listTpoFeeCode =
                          dbResultHeader.Where(c => c.LocationCode == tpoFeeReportsSummaryGoupByLocationWeek.LocationCode && c.KPSWeek == tpoFeeReportsSummaryGoupByLocationWeek.Week)
                              .Select(x => x.TPOFeeCode)
                              .ToList();

                string productionFeeTypeDescription = EnumHelper.GetDescription(inputProductionFeeType);
                double? dbCalculate =
                    dbResultCalculation.Where(
                        c => c.ProductionFeeType == productionFeeTypeDescription
                            && listTpoFeeCode.Contains(c.TPOFeeCode))
                        .Sum(x => x.Calculate);

                //insert into week value
                if (tpoFeeReportsSummaryGoupByLocationWeek.Week - 1 <= feeChildResult.ListWeekValue.Count)
                {
                    feeChildResult.ListWeekValue[tpoFeeReportsSummaryGoupByLocationWeek.Week - 1].WeekValue = Convert.ToDouble(dbCalculate.Value);
                }
            }
           

            feeChildResult.TotalCalculate = feeChildResult.ListWeekValue.Sum(c => c.WeekValue);

            return feeChildResult;
        }

        #endregion

        #region TPO Reports Production

        //public List<TPOFeeReportsProductionDTO> GetTpoFeeReportsProduction(GetTPOReportsProductionInput input)
        //{
        //    var dbResult = _sqlSPRepo.GetTpoReportsProduction(input);

        //    return Mapper.Map<List<TPOFeeReportsProductionDTO>>(dbResult);

        //    // using db function, below code is skipped
        //    bool isProductionDaily = false;

        //    var queryFilter = PredicateHelper.True<MstTPOPackage>();

        //    queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

        //    if (input.FilterType == Enums.FilterType.Daily.ToString())
        //    {
        //        var weekFrom = _masterDataBll.GetWeekByDate(input.DateFrom);
        //        var weekTo = _masterDataBll.GetWeekByDate(input.DateTo);

        //        isProductionDaily = true;
        //        queryFilter =
        //            queryFilter.And(
        //                p =>
        //                    (p.EffectiveDate <= input.DateFrom && p.ExpiredDate >= input.DateTo));
        //    }
        //    else if (input.FilterType == Enums.FilterType.YearMonth.ToString())
        //    {
        //        isProductionDaily = true;
        //        input.DateFrom = new DateTime(input.Year, input.Month, 1);
        //        input.DateTo = input.DateFrom.AddMonths(1).AddDays(-1);

        //        var weekFrom = _masterDataBll.GetWeekByDate(input.DateFrom);
        //        var weekTo = _masterDataBll.GetWeekByDate(input.DateTo);

        //        queryFilter =
        //            queryFilter.And(
        //                p =>
        //                    (p.EffectiveDate <= input.DateFrom && p.ExpiredDate >= input.DateTo));
        //    }
        //    else if (input.FilterType == Enums.FilterType.YearWeek.ToString())
        //    {
        //        //get week from masterweek
        //        var week = _mstGenWeekRepo.Get(c => (c.Year >= input.YearFrom && c.Week >= input.WeekFrom)
        //                                            && (c.Year <= input.YearTo && c.Week <= input.WeekTo));

        //        input.DateFrom = DateTime.Now;
        //        input.DateTo = DateTime.Now;

        //        if (week != null)
        //        {
        //            input.DateFrom = week.Min(c => c.StartDate.HasValue ? c.StartDate.Value : DateTime.Now);
        //            input.DateTo = week.Max(c => c.EndDate.HasValue ? c.EndDate.Value : DateTime.Now);
        //        }

        //        queryFilter =
        //            queryFilter.And(
        //                p =>
        //                    (p.EffectiveDate <= input.DateFrom && p.ExpiredDate >= input.DateTo));

        //    }

        //    input.SortExpression = "LocationCode";
        //    input.SortExpression2 = "BrandGroupCode";

        //    var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
        //    var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOPackage>();
        //    var package = _mstTPOPackageRepo.Get(queryFilter, orderByFilter);


        //    var dbResultHeader = _tpoFeeHdrRepo.Get(c => c.LocationCode == input.LocationCode);

         
        //    var listLocation = _mstGenLocationRepo.Get();

        //    return CreateListDataTpoFeeProduction(package.ToList(), dbResultHeader.ToList(),
        //        listLocation.ToList(), isProductionDaily, input);
        //}

        public List<TPOFeeReportsProductionDTO> GetTpoFeeReportsProductionViewYearWeek(GetTPOReportsProductionInput input)
        {
            // Create empty result
            var result = new List<TPOFeeReportsProductionDTO>();

            // Declare Query Filter to TPOFeeReportsProductionDailyView
            var queryFilter = PredicateHelper.True<TPOFeeReportsProductionWeeklyView>();
            // Get query by location
            if (input.LocationCode != Enums.LocationCode.SKT.ToString() && input.LocationCode != Enums.LocationCode.TPO.ToString())
            {
                queryFilter = queryFilter.And(p => p.Location == input.LocationCode);
                queryFilter = queryFilter.Or(p => p.Regional == input.LocationCode);
            }

            if (input.FilterType == Enums.FilterType.YearWeek.ToString())
            {
                queryFilter = queryFilter.And(p => (p.Week >= input.WeekFrom && p.Week <= input.WeekTo));
                queryFilter = queryFilter.And(p => (p.Year >= input.YearFrom && p.Year <= input.YearFrom));
            }

            // Get from database by query filter
            var dbResult = _tpoFeeReportsProductionWeeklyView.Get(queryFilter);

            // Grouping to get SUM on db result
            var groupby = dbResult.GroupBy(c => new
            {
                c.Regional,
                c.LocationName,
                c.Location,
                c.LocationCode,
                c.Package,
                c.BrandGroupCode,
                c.MemoRef,
                c.StartDate,
                c.EndDate,
                c.Week

            }).Select(x => new TPOFeeReportsProductionDTO()
            {
                Regional = x.Key.Regional,
                LocationCode = x.Key.Location,
                LocationAbbr = x.Key.LocationCode,
                LocationName = x.Key.LocationName,
                UMK = x.Sum(c => c.UMK),
                Brand = x.Key.BrandGroupCode,
                Package = x.Key.Package,

                JKNProductionFee = x.Sum(c => c.JKN),
                JL1ProductionFee = x.Sum(c => c.JL1),
                JL2ProductionFee = x.Sum(c => c.JL2),
                JL3ProductionFee = x.Sum(c => c.JL3),
                JL4ProductionFee = x.Sum(c => c.JL4),

                ManagementFee = x.Sum(c => c.JasaManajemen),
                ProductivityIncentives = Convert.ToDouble(x.Sum(c => c.ProductivityIncentives)),

                Year = x.Select(c => c.Year).FirstOrDefault(),
                StartDate = input.DateFrom.Date,
                EndDate = input.DateTo.Date,

                WeekFrom = x.Key.Week,
                WeekTo = x.Key.Week,
                NoMemo = x.Select(c => c.MemoRef).FirstOrDefault(),

                JKNProductionVolume = x.Sum(c => c.TotalProdJKN),
                JL1ProductionVolume = x.Sum(c => c.TotalProdJl1),
                JL2ProductionVolume = x.Sum(c => c.TotalProdJl2),
                JL3ProductionVolume = x.Sum(c => c.TotalProdJl3),
                JL4ProductionVolume = x.Sum(c => c.TotalProdJl4)

            }).OrderBy(x => x.Regional).ThenBy(x => x.LocationCode);
            result = Mapper.Map<List<TPOFeeReportsProductionDTO>>(groupby);
            

            return result;

        }

        public List<TPOFeeReportsProductionDTO> GetTpoFeeReportsProductionViewAll(GetTPOReportsProductionInput input)
        {
            // Create empty result
            var result = new List<TPOFeeReportsProductionDTO>();

            // Declare Query Filter to TPOFeeReportsProductionDailyView
            var queryFilter = PredicateHelper.True<TPOFeeReportsProductionWeeklyView>();
            // Get query by location
            if (input.LocationCode != Enums.LocationCode.SKT.ToString() && input.LocationCode != Enums.LocationCode.TPO.ToString())
            {
                queryFilter = queryFilter.And(p => p.Location == input.LocationCode);
                queryFilter = queryFilter.Or(p => p.Regional == input.LocationCode);
            }

            // Get from database by query filter
            var dbResult = _tpoFeeReportsProductionWeeklyView.Get(queryFilter);

            // Grouping to get SUM on db result
            var groupby = dbResult.Select(x => new TPOFeeReportsProductionDTO()
            {
                Regional = x.Regional,
                LocationCode = x.Location,
                LocationAbbr = x.LocationCode,
                LocationName = x.LocationName,
                UMK = x.UMK,
                Brand = x.BrandGroupCode,
                Package = x.Package,

                JKNProductionFee = x.JKN,
                JL1ProductionFee = x.JL1,
                JL2ProductionFee = x.JL2,
                JL3ProductionFee = x.JL3,
                JL4ProductionFee = x.JL4,

                ManagementFee = x.JasaManajemen,
                ProductivityIncentives = Convert.ToDouble(x.ProductivityIncentives),

                Year = x.Year,
                StartDate = x.StartDate,
                EndDate = x.EndDate,

                WeekFrom = x.Week,
                WeekTo = x.Week,
                NoMemo = x.MemoRef,

                JKNProductionVolume = x.TotalProdJKN,
                JL1ProductionVolume = x.TotalProdJl1,
                JL2ProductionVolume = x.TotalProdJl2,
                JL3ProductionVolume = x.TotalProdJl3,
                JL4ProductionVolume = x.TotalProdJl4

            }).OrderBy(x => x.Regional).ThenBy(x => x.LocationCode);
            result = Mapper.Map<List<TPOFeeReportsProductionDTO>>(groupby);


            return result;

        }

        public List<TPOFeeReportsProductionDTO> GetTpoFeeReportsProductionView(GetTPOReportsProductionInput input)
        {
            // Create empty result
            var result = new List<TPOFeeReportsProductionDTO>();

            // Declare Query Filter to TPOFeeReportsProductionDailyView
            var queryFilter = PredicateHelper.True<TPOFeeReportsProductionDailyView>();

            // Get query by location
            if (input.LocationCode != Enums.LocationCode.SKT.ToString() && input.LocationCode != Enums.LocationCode.TPO.ToString())
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
                queryFilter = queryFilter.Or(p => p.Regional == input.LocationCode);
            }

            // Get data daily
            // Example: date 1 - 5 then just show 5 rows without SUM and GROUP BY
            if (input.FilterType == Enums.FilterType.Daily.ToString())
            {
                queryFilter = queryFilter.And(c => c.FeeDate >= input.DateFrom);
                queryFilter = queryFilter.And(c => c.FeeDate <= input.DateTo);

                // Get from database by query filter
                var dbResult = _tpoFeeReportsProductionView.Get(queryFilter);

                result = Mapper.Map<List<TPOFeeReportsProductionDTO>>(dbResult);
            }
            else 
            { 
                // Modified input.DateFrom and input.DateTo by filter type
                if (input.FilterType == Enums.FilterType.All.ToString())
                {
                    // TODO not implemented yet.
                }
                else if (input.FilterType == Enums.FilterType.YearWeek.ToString())
                {
                    // Get from MstGenWeek by year from/week from and year to/week to
                    // Except for filter type Period
                    var masterWeek = _mstGenWeekRepo.Get(c => (c.Year >= input.YearFrom && c.Week >= input.WeekFrom) && 
                                                              (c.Year <= input.YearTo && c.Week <= input.WeekTo));

                    input.DateFrom  = DateTime.Now.Date;
                    input.DateTo    = DateTime.Now.Date;

                    if (masterWeek.Any())
                    {
                        input.DateFrom  = masterWeek.Min(c => c.StartDate.HasValue ? c.StartDate.Value : DateTime.Now);
                        input.DateTo    = masterWeek.Max(c => c.EndDate.HasValue ? c.EndDate.Value : DateTime.Now);
                    }
                }
                else if (input.FilterType == Enums.FilterType.YearMonth.ToString())
                {
                    input.DateFrom  = new DateTime(input.Year, input.Month, 1);
                    input.DateTo    = input.DateFrom.AddMonths(1).AddDays(-1);
                }

                // Get query by DateFrom and DateTo
                queryFilter = queryFilter.And(p => (p.FeeDate >= input.DateFrom && p.FeeDate <= input.DateTo));

                // Get from database by query filter
                var dbResult = _tpoFeeReportsProductionView.Get(queryFilter);

                // Grouping to get SUM on db result
                var groupby = dbResult.GroupBy(c => new
                {
                    c.Regional,
                    c.LocationName,
                    c.LocationCode,
                    c.LocationAbbr,
                    c.Package,
                    c.Brand,
                    c.NoMemo,
                    c.StartDate,
                    c.EndDate,
                    c.UMK

                }).Select(x => new TPOFeeReportsProductionDTO()
                {
                    Regional = x.Key.Regional,
                    LocationName = x.Select(c => c.LocationName).FirstOrDefault(),
                    LocationCode = x.Key.LocationCode,
                    LocationAbbr = x.Select(c => c.LocationAbbr).FirstOrDefault(),
                    //UMK = x.Sum(c => c.UMK),
                    UMK = x.Key.UMK,
                    Brand = x.Select(c => c.Brand).FirstOrDefault(),
                    Package = x.Key.Package,

                    JKNProductionFee = x.Sum(c => c.JKNProductionFee),
                    JL1ProductionFee = x.Sum(c => c.JL1ProductionFee),
                    JL2ProductionFee = x.Sum(c => c.JL2ProductionFee),
                    JL3ProductionFee = x.Sum(c => c.JL3ProductionFee),
                    JL4ProductionFee = x.Sum(c => c.JL4ProductionFee),

                    ManagementFee = x.Sum(c => c.ManagementFee),
                    ProductivityIncentives = Convert.ToDouble(x.Sum(c => c.ProductivityIncentives)),

                    Year = x.Select(c => c.Year).FirstOrDefault(),
                    StartDate = input.DateFrom.Date,
                    EndDate = input.DateTo.Date,
                    //WeekFrom = Convert.ToInt16(x.Select(c => c.Week)),
                    //WeekTo = Convert.ToInt16(x.Select(c => c.Week)),
                    WeekFrom = x.Select(c => c.Week).FirstOrDefault(),
                    WeekTo = x.Select(c => c.Week).Last(),
                    NoMemo = x.Select(c => c.NoMemo).FirstOrDefault(),

                    JKNProductionVolume = x.Sum(c => c.JKNProductionVolume),
                    JL1ProductionVolume = x.Sum(c => c.JL1ProductionVolume),
                    JL2ProductionVolume = x.Sum(c => c.JL2ProductionVolume),
                    JL3ProductionVolume = x.Sum(c => c.JL3ProductionVolume),
                    JL4ProductionVolume = x.Sum(c => c.JL4ProductionVolume)

                }).OrderBy(x=>x.Regional);//.ThenBy(x=>x.LocationCode);
                result = Mapper.Map<List<TPOFeeReportsProductionDTO>>(groupby);
            }

            return result;
        }

        private List<TPOFeeReportsProductionDTO> CreateListDataTpoFeeProduction(
                            List<MstTPOPackage> listMasterPackage,
                            List<TPOFeeHdr> dbResultHeader, List<MstGenLocation> listLocation, 
                            bool isProductionDaily,
                            GetTPOReportsProductionInput input)
        {
            var result = new List<TPOFeeReportsProductionDTO>();


            foreach (var mstTpoPackage in listMasterPackage)
            {

                //get tpofeecode from header
                //var rangeWeek =
                //    _mstGenWeekRepo.Get(
                //        c => c.StartDate >= mstTpoPackage.EffectiveDate && c.EndDate <= mstTpoPackage.ExpiredDate);
                //var minDate = rangeWeek.FirstOrDefault();
                //var maxDate = rangeWeek.LastOrDefault();

                var minDate = _masterDataBll.GetWeekByDate(input.DateFrom);
                var maxDate = _masterDataBll.GetWeekByDate(input.DateTo);

                if (minDate == null)
                    minDate = new HMS.SKTIS.BusinessObjects.DTOs.MstGenWeekDTO();
                if (maxDate == null)
                    maxDate = new HMS.SKTIS.BusinessObjects.DTOs.MstGenWeekDTO();

                var listResultHeader = dbResultHeader.Where(c => c.BrandGroupCode == mstTpoPackage.BrandGroupCode
                   && (c.KPSYear >= minDate.Year && c.KPSWeek >= minDate.Week)
                   && (c.KPSYear <= maxDate.Year && c.KPSWeek <= maxDate.Week));

                var listTpoFeeCode = listResultHeader.Select(c => c.TPOFeeCode).Distinct();

               

                var dbResultCalculation = _tpoFeeCalculationRepo.Get(c => listTpoFeeCode.Contains(c.TPOFeeCode));
                var dbResultProductionDaily = _tpoFeeProductionDailyRepo.Get(c => listTpoFeeCode.Contains(c.TPOFeeCode));

                var tpoFeeProduction = new TPOFeeReportsProductionDTO();

                //get location
                var location = listLocation.FirstOrDefault(c => c.LocationCode == mstTpoPackage.LocationCode);

                if (location != null)
                {
                    tpoFeeProduction.Regional = location.ParentLocationCode;
                    tpoFeeProduction.LocationCode = location.LocationCode;
                    tpoFeeProduction.LocationAbbr = location.ABBR;
                    tpoFeeProduction.LocationName = location.LocationName;
                    tpoFeeProduction.UMK = location.UMK;
                }

                tpoFeeProduction.Brand = mstTpoPackage.BrandGroupCode;
                tpoFeeProduction.Package = mstTpoPackage.Package;

                //get list tfo fee code

                if (isProductionDaily)
                {
                    tpoFeeProduction.JKNProductionFee = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JKNRp);
                    tpoFeeProduction.JL1ProductionFee = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JL1Rp);
                    tpoFeeProduction.JL2ProductionFee = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JL2Rp);
                    tpoFeeProduction.JL3ProductionFee = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JL3Rp);
                    tpoFeeProduction.JL4ProductionFee = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JL4Rp);
                    tpoFeeProduction.JKNProductionVolume = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JKNBoxFinal);
                    tpoFeeProduction.JL1ProductionVolume = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JL1BoxFinal);
                    tpoFeeProduction.JL2ProductionVolume = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JL2BoxFinal);
                    tpoFeeProduction.JL3ProductionVolume = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JL3BoxFinal);
                    tpoFeeProduction.JL4ProductionVolume = dbResultProductionDaily.Where(n => n.FeeDate >= input.DateFrom && n.FeeDate <= input.DateTo).Sum(x => x.JL4BoxFinal);
                }
                else
                {
                    tpoFeeProduction.JKNProductionFee = dbResultCalculation.Where(c => c.OrderFeeType == 1).Sum(x => x.Calculate);
                    tpoFeeProduction.JL1ProductionFee = dbResultCalculation.Where(c => c.OrderFeeType == 2).Sum(x => x.Calculate);
                    tpoFeeProduction.JL2ProductionFee = dbResultCalculation.Where(c => c.OrderFeeType == 3).Sum(x => x.Calculate);
                    tpoFeeProduction.JL3ProductionFee = dbResultCalculation.Where(c => c.OrderFeeType == 4).Sum(x => x.Calculate);
                    tpoFeeProduction.JL4ProductionFee = dbResultCalculation.Where(c => c.OrderFeeType == 5).Sum(x => x.Calculate);
                    tpoFeeProduction.JKNProductionVolume = listResultHeader.Sum(x => x.TotalJKN);
                    tpoFeeProduction.JL1ProductionVolume = listResultHeader.Sum(x => x.TotalJL1);
                    tpoFeeProduction.JL2ProductionVolume = listResultHeader.Sum(x => x.TotalJL2);
                    tpoFeeProduction.JL3ProductionVolume = listResultHeader.Sum(x => x.TotalJL3);
                    tpoFeeProduction.JL4ProductionVolume = listResultHeader.Sum(x => x.TotalJL4);
                }

                tpoFeeProduction.ManagementFee =
                  dbResultCalculation.Where(
                      c => c.ProductionFeeType == "Jasa Manajemen").Sum(x => x.Calculate);

                tpoFeeProduction.ProductivityIncentives =
                   dbResultCalculation.Where(
                       c => c.ProductionFeeType == "Productivity Incentives").Sum(x => x.Calculate);

                tpoFeeProduction.Year = mstTpoPackage.EffectiveDate.Year;
                //tpoFeeProduction.StartDate = GenericHelper.ConvertDateTimeToString(mstTpoPackage.EffectiveDate);
                //tpoFeeProduction.EndDate = GenericHelper.ConvertDateTimeToString(mstTpoPackage.ExpiredDate);
                tpoFeeProduction.NoMemo = mstTpoPackage.MemoRef;

                tpoFeeProduction.WeekFrom = minDate.Week.HasValue ? minDate.Week.Value : 0;

                tpoFeeProduction.WeekTo = maxDate.Week.HasValue ? maxDate.Week.Value : 0;

                result.Add(tpoFeeProduction);
            }


            return result;
        }

        #endregion

        #region EMAILs

        #region TPO Fee Actual Emailing

        public void SendEmailSubmitTPOFeeActual(string tpoFeeCode, string regional, string currUserName) 
        {
            // Split Fee Code 'FEE/IDAH/DSB12HP-20/2016/22'
            var tpoParams = tpoFeeCode.Split('/');
            
            var locationCode = tpoParams[1];
            var brandGroupCode = tpoParams[2];
            var kpsYear = Convert.ToInt32(tpoParams[3]);
            var kpsWeek = Convert.ToInt32(tpoParams[4]);

            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = regional,
                Regional = locationCode,
                BrandCode = brandGroupCode,
                KpsWeek = kpsWeek,
                KpsYear = kpsYear,
                FunctionName = Enums.PageName.TPOFeeActualDetail.ToString(),
                ButtonName = Enums.ButtonName.Submit.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.TPOFeeActualDetail),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.TPOFeeActualDetail),
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
                    BodyEmail = CreateBodyMailSubmitTPOFeeActual(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }

        private string CreateBodyMailSubmitTPOFeeActual(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("TPO Fee membutuhkan approval anda: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/TPOFeeExeActualDetail/Index/"
                                                                   + "FEE" + "_"
                                                                   + emailInput.Regional + "_"
                                                                   + emailInput.BrandCode + "_"
                                                                   + emailInput.KpsYear + "_"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + "approval" + "/"
                                                                   + "TPO" + "/"
                                                                   + emailInput.IDResponsibility
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public void SendEmailReturnTPOFeeActual(string tpoFeeCode, string regional, string currUserName)
        {
            var tpoParams = tpoFeeCode.Split('/');

            var locationCode = tpoParams[1];
            var brandGroupCode = tpoParams[2];
            var kpsYear = Convert.ToInt32(tpoParams[3]);
            var kpsWeek = Convert.ToInt32(tpoParams[4]);

            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = regional,
                Regional = locationCode,
                BrandCode = brandGroupCode,
                KpsWeek = kpsWeek,
                KpsYear = kpsYear,
                FunctionName = Enums.PageName.TPOFeeActualDetail.ToString(),
                ButtonName = Enums.ButtonName.Revise.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.TPOFeeApproval),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.ApprovalPage),
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
                    BodyEmail = CreateBodyMailReturnTPOFeeActual(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }

        private string CreateBodyMailReturnTPOFeeActual(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("TPO Fee telah direturn, Silakan melanjutkan proses brikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<a target='_blank' href='webrooturl/TPOFeeExeActualDetail/Index/"
                                                                   + "FEE" + "_"
                                                                   + emailInput.Regional + "_"
                                                                   + emailInput.BrandCode + "_"
                                                                   + emailInput.KpsYear + "_"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + "approval" + "/"
                                                                   + "TPO" + "/"
                                                                   + emailInput.IDResponsibility
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine
                                                                   + "> " + emailInput.FunctionNameDestination + "</a>");

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public void SendEmailApprovalTPOFeeActual(string tpoFeeCode, string regional, string currUserName)
        {
            var tpoParams = tpoFeeCode.Split('/');

            var locationCode = tpoParams[1];
            var brandGroupCode = tpoParams[2];
            var kpsYear = Convert.ToInt32(tpoParams[3]);
            var kpsWeek = Convert.ToInt32(tpoParams[4]);

            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = regional,
                Regional = locationCode,
                BrandCode = brandGroupCode,
                KpsWeek = kpsWeek,
                KpsYear = kpsYear,
                FunctionName = Enums.PageName.TPOFeeActualDetail.ToString(),
                ButtonName = Enums.ButtonName.Approve.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.TPOFeeApproval),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.ApprovalPage),
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
                    BodyEmail = CreateBodyMailApproveTPOFeeActual(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }

        private string CreateBodyMailApproveTPOFeeActual(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("TPO Fee membutuhkan autorize anda: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<a target='_blank' href='webrooturl/TPOFeeExeActualDetail/Index/"
                                                                   + "FEE" + "_"
                                                                   + emailInput.Regional + "_"
                                                                   + emailInput.BrandCode + "_"
                                                                   + emailInput.KpsYear + "_"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + "approval" + "/"
                                                                   + "TPO" + "/"
                                                                   + emailInput.IDResponsibility
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine
                                                                   + "> " + emailInput.FunctionNameDestination + "</a>");

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public void SendEmailAuthorizeTPOFeeActual(string tpoFeeCode, string regional, string currUserName)
        {
            var tpoParams = tpoFeeCode.Split('/');

            var locationCode = tpoParams[1];
            var brandGroupCode = tpoParams[2];
            var kpsYear = Convert.ToInt32(tpoParams[3]);
            var kpsWeek = Convert.ToInt32(tpoParams[4]);

            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = regional,
                Regional = locationCode,
                BrandCode = brandGroupCode,
                KpsWeek = kpsWeek,
                KpsYear = kpsYear,
                FunctionName = Enums.PageName.TPOFeeActualDetail.ToString(),
                ButtonName = Enums.ButtonName.Authorize.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.TPOFeeApproval),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.ApprovalPage),
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
                    BodyEmail = CreateBodyMailAuthorizeTPOFeeActual(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }

        private string CreateBodyMailAuthorizeTPOFeeActual(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("TPO Fee Approval sudah completed: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<p><a href= webrooturl/TPOFeeExeActualDetail/Index/"
                                                                   + "FEE" + "_"
                                                                   + emailInput.Regional + "_"
                                                                   + emailInput.BrandCode + "_"
                                                                   + emailInput.KpsYear + "_"
                                                                   + emailInput.KpsWeek + "/"
                                                                   + "approval" + "/"
                                                                   + "TPO" + "/"
                                                                   + emailInput.IDResponsibility
                                                                   + "> " + emailInput.FunctionNameDestination + "</a></p>"
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public void SendEmailTPOApprovalPage(IEnumerable<string> listTPOFeeCode, string regional, string currUserName, string action) 
        {
            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput {
                Regional = regional,
                FunctionName = Enums.PageName.ApprovalPage.ToString(),
                ButtonName = action,
                FunctionNameDestination = action == Enums.ButtonName.Approve.ToString() ? Enums.PageName.ApprovalPage.ToString() : Enums.PageName.TPOFeeAP.ToString()
            };

            // Get User, Email, Responsibility Destination Recipient
            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            if (action == Enums.ButtonName.Authorize.ToString()) 
            {
                var distinctEmail = listUserAndEmailDestination.Select(c => new { c.UserAD, c.Name, c.Email }).Distinct();
                listUserAndEmailDestination = distinctEmail
                                                .Select(c => new GetUserAndEmail_Result { UserAD = c.UserAD, Name = c.Name, Email = c.Email,
                                                                                            IDResponsibility = listUserAndEmailDestination.Where(a => a.UserAD == c.UserAD)
                                                                                                                .Select(a => a.IDResponsibility).FirstOrDefault()}).ToList();
            }

            // Get User and Email Current/Sender
            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            // Create Email Input
            var listEmail = new List<MailInput>();
            foreach (var item in listUserAndEmailDestination) {
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                var email = new MailInput {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = EnumHelper.GetDescription(Enums.EmailSubject.TPOFeeApproval),
                    BodyEmail = action == Enums.ButtonName.Approve.ToString() ? CreateBodyMailApprovalPageApprove(emailInput, listTPOFeeCode) : CreateBodyMailApprovalPageAuthorize(emailInput, listTPOFeeCode)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail) {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }

        private string CreateBodyMailApprovalPageApprove(GetUserAndEmailInput emailInput, IEnumerable<string> listTPOFeeCode) {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("TPO Fee membutuhkan approval anda: " + Environment.NewLine + Environment.NewLine);
            foreach (var tpoFeeCode in listTPOFeeCode) {
                var tpoParams = tpoFeeCode.Split('/');

                var locationCode = tpoParams[1];
                var brandGroupCode = tpoParams[2];
                var kpsYear = Convert.ToInt32(tpoParams[3]);
                var kpsWeek = Convert.ToInt32(tpoParams[4]);

                bodyMail.Append("<p><a href= webrooturl/TPOFeeApproval/Index/List/Actual/"
                                                                   + "FEE" + "_"
                                                                   + locationCode + "_"
                                                                   + brandGroupCode + "_"
                                                                   + kpsYear + "_"
                                                                   + kpsWeek + "/"
                                                                   + "approval" + "/"
                                                                   + "TPO" + "/"
                                                                   + emailInput.IDResponsibility
                                                                   + ">" + emailInput.FunctionNameDestination + "</a></p>");
                bodyMail.Append(Environment.NewLine); bodyMail.Append(Environment.NewLine); bodyMail.Append(Environment.NewLine);
                break;
            }
            

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        private string CreateBodyMailApprovalPageAuthorize(GetUserAndEmailInput emailInput, IEnumerable<string> listTPOFeeCode) {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("TPO Fee membutuhkan authorize anda: " + Environment.NewLine + Environment.NewLine);
            foreach (var tpoFeeCode in listTPOFeeCode) {
                var tpoParams = tpoFeeCode.Split('/');

                var locationCode = tpoParams[1];
                var brandGroupCode = tpoParams[2];
                var kpsYear = Convert.ToInt32(tpoParams[3]);
                var kpsWeek = Convert.ToInt32(tpoParams[4]);

                bodyMail.Append("<a target='_blank' href='webrooturl/TPOFeeExeAPOpen/Index/"
                                                                   + "FEE" + "_"
                                                                   + locationCode + "_"
                                                                   + brandGroupCode + "_"
                                                                   + kpsYear + "_"
                                                                   + kpsWeek + "/"
                                                                   + "approval" + "/"
                                                                   + "TPO" + "/"
                                                                   + emailInput.IDResponsibility
                                                                   + "> " + emailInput.FunctionNameDestination + "</a>");
                bodyMail.Append(Environment.NewLine); bodyMail.Append(Environment.NewLine); bodyMail.Append(Environment.NewLine);
                break;
            }


            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }
        #endregion

        #endregion
    }
}
