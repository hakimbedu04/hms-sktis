using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.DAL;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.Inputs;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using System.Configuration;
using System.Web.SessionState;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace HMS.SKTIS.BLL.PlantWagesBLL
{
    public class PlantWagesExecutionBLL : IPlantWagesExecutionBLL
    {
        private IUnitOfWork _uow;
        private IMasterDataBLL _masterDataBll;
        private IGeneralBLL _generalBll;
        private IUtilitiesBLL _utilitiesBll;
        private IGenericRepository<ProductionCard> _productionCardRepository;
        private IGenericRepository<ExeProductionEntryRelease> _exeProductionEntryRelease; 
        private IGenericRepository<WagesProductionCardApprovalView> _productionCardApprovalRepository;
        private IGenericRepository<WagesProductionCardApprovalDetailView> _productionCardApprovalDetailRepository;
        private IGenericRepository<WagesProductionCardApprovalDetailViewGroup> _productionCardApprovalDetailGroupRepository;
        private IGenericRepository<ExePlantProductionEntryVerification> _exePlantProductionEntryVerificationRepository;
        private IGenericRepository<ExeProductionEntryRelease> _exeProductionEntryReleaseRepository;
        private IGenericRepository<ExeProductionEntryReleaseTransactLogsView> _exeProductionEntryReleaseTransactLogsViewRepository;
        private IGenericRepository<UtilTransactionLog> _utilTransactionLogRepository;
        private IGenericRepository<UtilFlow> _utilFlowRepository;
        private ISqlSPRepository _sqlSPRepo;
        private IGenericRepository<MstADTemp> _mstAdTemp;
        private IGenericRepository<AvailabelPositionNumberGroup> _availablePositionNumber;
        private IGenericRepository<UserADResponsibilityRolesView> _userADView;
        private IGenericRepository<MstGenBrandGroup> _mstGenBrandGroupRepo;
        private IGenericRepository<MstGenBrand> _mstGenBrandRepo;
        private IGenericRepository<MstGenProcess> _mstGenProcessRepo;
        private IGenericRepository<ProcessSettingsAndLocationView> _processSettingsAndLocationViewRepo;
        private IGenericRepository<GetProductionCardApprovalList_Result> _getProductionCardApprovalList;
        private IGenericRepository<PlanPlantTargetProductionKelompok> _planPlantTargetProductionKelompokRepo;
        

        public PlantWagesExecutionBLL(IUnitOfWork uow, IMasterDataBLL masterDataBll, IUtilitiesBLL utilitiesBll, IGeneralBLL generalBll)
        {
            _uow = uow;
            _utilitiesBll = utilitiesBll;
            _productionCardRepository = _uow.GetGenericRepository<ProductionCard>();
            _productionCardApprovalRepository = _uow.GetGenericRepository<WagesProductionCardApprovalView>();
            _productionCardApprovalDetailRepository = _uow.GetGenericRepository<WagesProductionCardApprovalDetailView>();
            _productionCardApprovalDetailGroupRepository =
                uow.GetGenericRepository<WagesProductionCardApprovalDetailViewGroup>();
            _masterDataBll = masterDataBll;            
            _exePlantProductionEntryVerificationRepository = _uow.GetGenericRepository<ExePlantProductionEntryVerification>();
            _exeProductionEntryReleaseRepository = _uow.GetGenericRepository<ExeProductionEntryRelease>();
            _exeProductionEntryReleaseTransactLogsViewRepository = _uow.GetGenericRepository<ExeProductionEntryReleaseTransactLogsView>();
            _masterDataBll = masterDataBll;
            _exeProductionEntryRelease = _uow.GetGenericRepository<ExeProductionEntryRelease>();
            _generalBll = generalBll;
            _utilTransactionLogRepository = _uow.GetGenericRepository<UtilTransactionLog>();
            _utilFlowRepository = _uow.GetGenericRepository<UtilFlow>();
            _mstAdTemp = _uow.GetGenericRepository<MstADTemp>();
            _sqlSPRepo = _uow.GetSPRepository();
            _userADView = _uow.GetGenericRepository<UserADResponsibilityRolesView>();
            _mstGenBrandGroupRepo = _uow.GetGenericRepository<MstGenBrandGroup>();
            _mstGenBrandRepo = _uow.GetGenericRepository<MstGenBrand>();
            _mstGenProcessRepo = _uow.GetGenericRepository<MstGenProcess>();
            _processSettingsAndLocationViewRepo = _uow.GetGenericRepository<ProcessSettingsAndLocationView>();
            _getProductionCardApprovalList = _uow.GetGenericRepository<GetProductionCardApprovalList_Result>();
            _planPlantTargetProductionKelompokRepo = _uow.GetGenericRepository<PlanPlantTargetProductionKelompok>();
        }

        #region Production Card
        public List<ProductionCardDTO> GetProductionCards(GetProductionCardInput input)
        {
            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (string.IsNullOrEmpty(input.LocationCode))
                input.LocationCode = "X";
            queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

            if (string.IsNullOrEmpty(input.Unit))
                input.Unit = "X";
            queryFilter = queryFilter.And(p => p.UnitCode == input.Unit);

            if (input.Shift > 0)
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);

            if (!string.IsNullOrEmpty(input.Process))
                //input.Process = "X";
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.Process);

            if (!string.IsNullOrEmpty(input.Group))
                //input.Group = "X";
                queryFilter = queryFilter.And(p => p.GroupCode == input.Group);

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                //input.BrandGroupCode = "X";
                queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);

            if (!string.IsNullOrEmpty(input.Brand))
                //input.Brand = "X";
                queryFilter = queryFilter.And(p => p.BrandCode == input.Brand);

            if (input.KPSYear > 0 && input.KPSWeek > 0)
            {
                var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(input.KPSYear, input.KPSWeek);
                queryFilter = queryFilter.And(p => p.ProductionDate >= mstGenWeek.StartDate && p.ProductionDate <= mstGenWeek.EndDate);
            }
            if (input.Date != DateTime.MinValue)
                queryFilter = queryFilter.And(p => p.ProductionDate == input.Date);

            queryFilter = queryFilter.And(p => p.RevisionType == input.RevisionType);


            var dbResult = _productionCardRepository.Get(queryFilter).OrderBy(p => p.EmployeeNumber);
            return Mapper.Map<List<ProductionCardDTO>>(dbResult);
        }

        public List<ProductionCardCalculateTotalPayotherDTO> GetProductionCardsGroupAll(GetProductionCardInputGroupAll input)
        {
            var hasil = new List<ProductionCardCalculateTotalPayotherDTO>();
            
            var loopInput = new GetProductionCardInput();
            var sementara = new List<ProductionCardDTO>();
            foreach (var t in input.Group)
            {
                var TotalSementara = new ProductionCardCalculateTotalPayotherDTO();
                loopInput.Group = t;
                loopInput.Brand = input.Brand;
                loopInput.BrandGroupCode = input.BrandGroupCode;
                loopInput.Date = input.Date;
                loopInput.KPSWeek = input.KPSWeek;
                loopInput.KPSYear = input.KPSYear;
                loopInput.LocationCode = input.LocationCode;
                //loopInput.Process = input.Process;
                loopInput.RevisionType = input.RevisionType;
                loopInput.Shift = input.Shift;
                loopInput.Unit = input.Unit;
                loopInput.UserName = input.UserName;
                loopInput.endDate = input.endDate;
                loopInput.starDate = input.starDate;

                if (GetLastConditionTranslogProdCard(loopInput) != "Open") continue;
                sementara = GetProductionCards(loopInput);
                if (!sementara.Any()) continue;
                var totalProd = sementara.Sum(m => m.Production);
                var totalUpahLain = sementara.Sum(m => m.UpahLain);
                TotalSementara.Group = t;
                TotalSementara.ActualProduction = totalProd.ToString();
                TotalSementara.UpahLain = totalUpahLain.ToString();
                TotalSementara.RevisionType = input.RevisionType;
                hasil.Add(TotalSementara);
            }
            return hasil;
        }

        public String GetLastConditionTranslogProdCard(GetProductionCardInput input)
        {
            DateTime date = input.Date;
            int day = date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek;

            var transLogProdCardCode = EnumHelper.GetDescription(Enums.CombineCode.WPC) + "/"
                                      + input.LocationCode + "/"
                                      + input.Shift + "/"
                                      + input.Unit + "/"
                                      + input.Group + "/"
                                      + input.Brand + "/"
                                      + input.KPSYear + "/"
                                      + input.KPSWeek + "/"
                                      + day + "/"
                                      + input.RevisionType;

            var latestTransLogProdCard = _utilitiesBll.GetLatestActionTransLogExceptSave(transLogProdCardCode, Enums.PageName.ProductionCard.ToString());

            var status = "Open";

            //return latestTransLogProdCard == null ? "1" : "0";

            if (latestTransLogProdCard != null)
            {
                var submitted = latestTransLogProdCard.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString();
                if (submitted)
                {
                    status = "Submitted";
                    var latestTransLogProdCardApproval = _utilitiesBll.GetLatestActionTransLog(transLogProdCardCode, Enums.PageName.ProductionCardApprovalDetail.ToString());
                    var isProdCardReturn = false;
                    if (latestTransLogProdCardApproval != null)
                    {
                        isProdCardReturn = latestTransLogProdCardApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Return.ToString();
                    }

                    if (!isProdCardReturn)
                    {
                        //var latestTransLogProdCardApproval = _utilitiesBLL.GetLatestActionTransLog(transLogProdCardCode, Enums.PageName.ProductionCardApprovalDetail.ToString());
                        if (latestTransLogProdCardApproval != null)
                        {
                            var isProdCardApproved = latestTransLogProdCardApproval.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Approve.ToString();
                            if (latestTransLogProdCard.CreatedDate < latestTransLogProdCardApproval.CreatedDate)
                            {
                                if (isProdCardApproved)
                                {
                                    status = "Locked";
                                }
                            }
                        }
                    }

                }
            }

            return status;
        }

        public List<ProductionCardDTO> GetProductionCardsBrandGroupCode(GetProductionCardInput input)
        {
            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (string.IsNullOrEmpty(input.LocationCode))
                input.LocationCode = "X";
            queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

            if (string.IsNullOrEmpty(input.Unit))
                input.Unit = "X";
            queryFilter = queryFilter.And(p => p.UnitCode == input.Unit);

            if (input.Shift > 0)
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);

            if (input.Date != DateTime.MinValue)
                queryFilter = queryFilter.And(p => p.ProductionDate == input.Date);

            queryFilter = queryFilter.And(p => p.RevisionType == input.RevisionType);

            var dbResult = _productionCardRepository.Get(queryFilter).Distinct(); //.OrderBy(p => p.EmployeeNumber);
            var res = from abc in dbResult
                      orderby abc.EmployeeNumber
                      select new ProductionCardDTO
                      {
                          BrandGroupCode = abc.BrandGroupCode
                      };


            return Mapper.Map<List<ProductionCardDTO>>(res);
        }

        public List<ProductionCardDTO> GetProcessGroupProductionCards(GetProductionCardInput input)
        {
            if (input.LocationCode == HMS.SKTIS.Core.Enums.LocationCode.PLNT.ToString())
            {
                input.LocationCode = null;
            }
            
            var dbResult = _sqlSPRepo.GetProcessFromProdCard(input.starDate, input.endDate, input.LocationCode, input.Unit);
            return Mapper.Map<List<ProductionCardDTO>>(dbResult);
        }

        public IEnumerable<string> GetProcessGroupFromReportByGroup(GetProductionCardInput input) 
        {
            var result = new List<string>();

            using (SKTISEntities context = new SKTISEntities()) {
                var mstGenProcess = context.MstGenProcesses.Select(c => new { c.ProcessGroup, c.ProcessOrder }).Distinct().ToList();
                if (input.LocationCode == HMS.SKTIS.Core.Enums.LocationCode.PLNT.ToString()) {
                    if (String.IsNullOrEmpty(input.Unit)) {
                        var listProcess = context.ExeReportByGroups.Where(c => c.ProductionDate >= input.starDate && c.ProductionDate <= input.endDate)
                                                                        .Select(c => c.ProcessGroup)
                                                                        .Distinct()
                                                                        .ToList();
                        result = mstGenProcess.OrderBy(c => c.ProcessOrder).Where(c => listProcess.Contains(c.ProcessGroup))
                                                                            .Select(c => c.ProcessGroup)
                                                                            .Distinct()
                                                                            .ToList();
                    }
                    else {
                        var listProcess = context.ExeReportByGroups.Where(c => c.ProductionDate >= input.starDate && c.ProductionDate <= input.endDate && c.UnitCode == input.Unit)
                                                                       .Select(c => c.ProcessGroup)
                                                                       .Distinct()
                                                                       .ToList();
                        result = mstGenProcess.OrderBy(c => c.ProcessOrder).Where(c => listProcess.Contains(c.ProcessGroup))
                                                                            .Select(c => c.ProcessGroup)
                                                                            .Distinct()
                                                                            .ToList();
                    }
                }
                else {
                    if (String.IsNullOrEmpty(input.Unit)) {
                        var listProcess = context.ExeReportByGroups.Where(c => c.ProductionDate >= input.starDate && c.ProductionDate <= input.endDate && c.LocationCode == input.LocationCode)
                                                                        .Select(c => c.ProcessGroup)
                                                                        .Distinct()
                                                                        .ToList();
                        result = mstGenProcess.OrderBy(c => c.ProcessOrder).Where(c => listProcess.Contains(c.ProcessGroup))
                                                                            .Select(c => c.ProcessGroup)
                                                                            .Distinct()
                                                                            .ToList();
                    }
                    else {
                        var listProcess = context.ExeReportByGroups.Where(c => c.ProductionDate >= input.starDate && c.ProductionDate <= input.endDate && c.UnitCode == input.Unit && c.LocationCode == input.LocationCode)
                                                                       .Select(c => c.ProcessGroup)
                                                                       .Distinct()
                                                                       .ToList();
                        result = mstGenProcess.OrderBy(c => c.ProcessOrder).Where(c => listProcess.Contains(c.ProcessGroup))
                                                                            .Select(c => c.ProcessGroup)
                                                                            .Distinct()
                                                                            .ToList();
                    }
                }
            }

            return result;
        }

        public List<ProductionCardDTO> GetProductionCardsCorrection(GetProductionCardInput input)
        {
            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.Unit))
                queryFilter = queryFilter.And(p => p.UnitCode == input.Unit);

            if (input.Shift > 0)
                queryFilter = queryFilter.And(p => p.Shift == input.Shift);

            if (!string.IsNullOrEmpty(input.Process))
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.Process);

            if (!string.IsNullOrEmpty(input.Group))
                queryFilter = queryFilter.And(p => p.GroupCode == input.Group);

            if (!string.IsNullOrEmpty(input.Brand))
                queryFilter = queryFilter.And(p => p.BrandCode == input.Brand);

            if (input.KPSYear > 0 && input.KPSWeek > 0)
            {
                var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(input.KPSYear, input.KPSWeek);
                queryFilter = queryFilter.And(p => p.ProductionDate >= mstGenWeek.StartDate && p.ProductionDate <= mstGenWeek.EndDate);
            }
            if (input.Date != DateTime.MinValue)
                queryFilter = queryFilter.And(p => p.ProductionDate == input.Date);

            queryFilter = queryFilter.And(p => p.RevisionType != 0);

            var dbResult = _productionCardRepository.Get(queryFilter).OrderBy(p => p.EmployeeNumber);
            return Mapper.Map<List<ProductionCardDTO>>(dbResult);
        }

        public ProductionCardDTO SaveProductionCard(ProductionCardDTO productionCardDto)
        {
            var dbProductionCard = _productionCardRepository.GetByID(productionCardDto.RevisionType,
                productionCardDto.LocationCode, productionCardDto.UnitCode, productionCardDto.BrandCode,
                productionCardDto.ProcessGroup, productionCardDto.GroupCode, productionCardDto.EmployeeID,
                productionCardDto.ProductionDate);
            if (dbProductionCard == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dbProductionCard.UpdatedDate = DateTime.Now;

            Mapper.Map(productionCardDto, dbProductionCard);

            _productionCardRepository.Update(dbProductionCard);

            // Update From Remark
            UpdateProductionCardFromRemark(productionCardDto);

            _uow.SaveChanges();

            return Mapper.Map<ProductionCardDTO>(dbProductionCard);
        }

        public void UpdateSuratPeriodeLalu(string employeeId, string productionDate, string locationCode, string unitCode, int shift, string groupCode, string processGroup, string brandCode, int revisionType, string Remark,List<SuratPeriodeLaluInput> SuratPeriodeLalu)
        {
            var date = Convert.ToDateTime(productionDate);
            foreach (SuratPeriodeLaluInput i in SuratPeriodeLalu.Where(c => c.Status == true))
            {
                var queryFilter = PredicateHelper.True<ProductionCard>();

                queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
                queryFilter = queryFilter.And(p => p.RevisionType == revisionType);
                queryFilter = queryFilter.And(p => p.UnitCode == unitCode);
                queryFilter = queryFilter.And(p => p.ProcessGroup == processGroup);
                queryFilter = queryFilter.And(p => p.GroupCode == groupCode);
                queryFilter = queryFilter.And(p => p.EmployeeID == employeeId);
                queryFilter = queryFilter.And(p => p.ProductionDate == i.AlphaDate);

                var listDbProductionCard = _productionCardRepository.Get(queryFilter).ToList();

                if (listDbProductionCard.Count() == 0) return;
                //var dbProductionCard = _productionCardRepository.GetByID(revisionType, locationCode,
                //   unitCode, brandCode, processGroup, groupCode, employeeId, i.AlphaDate);

                //if (dbProductionCard == null) return;

                foreach (var dbProductionCard in listDbProductionCard)
                {
                    if(String.IsNullOrEmpty(dbProductionCard.Remark))
                    {
                         dbProductionCard.Remark = "X;";
                    }
                    else if(dbProductionCard.Remark == "NULL")
                    {
                        dbProductionCard.Remark = "X;";
                    }
                    else if(dbProductionCard.Remark[0] == 'J')
                    {
                        dbProductionCard.Remark = "X;" + dbProductionCard.Remark;
                    }
                    
                    //dbProductionCard.Remark = i.Status == true ? (dbProductionCard.Remark != "x"
                    //? "X;" + (String.IsNullOrEmpty(dbProductionCard.Remark) || dbProductionCard.Remark == "NULL" ? string.Empty : dbProductionCard.Remark) : "X") : null;

                    dbProductionCard.UpdatedDate = date;

                    _productionCardRepository.Update(dbProductionCard);
                }

                _uow.SaveChanges();

            }

            foreach (SuratPeriodeLaluInput i in SuratPeriodeLalu.Where(c => c.Status == false))
            {
                var queryFilterXMark = PredicateHelper.True<ProductionCard>();

                queryFilterXMark = queryFilterXMark.And(p => p.LocationCode == locationCode);
                queryFilterXMark = queryFilterXMark.And(p => p.RevisionType == revisionType);
                queryFilterXMark = queryFilterXMark.And(p => p.UnitCode == unitCode);
                queryFilterXMark = queryFilterXMark.And(p => p.ProcessGroup == processGroup);
                queryFilterXMark = queryFilterXMark.And(p => p.GroupCode == groupCode);
                queryFilterXMark = queryFilterXMark.And(p => p.EmployeeID == employeeId);
                queryFilterXMark = queryFilterXMark.And(p => p.ProductionDate == i.AlphaDate);
                queryFilterXMark = queryFilterXMark.And(p => p.Remark != "" || p.Remark == "X;");

                var listDbProductionCardXMark = _productionCardRepository.Get(queryFilterXMark).ToList();

                foreach (var dbProductionCard in listDbProductionCardXMark)
                {

                    dbProductionCard.Remark = "";

                    _productionCardRepository.Update(dbProductionCard);

                }

                _uow.SaveChanges();

            }

            var queryFilterCurr = PredicateHelper.True<ProductionCard>();

            queryFilterCurr = queryFilterCurr.And(p => p.LocationCode == locationCode);
            queryFilterCurr = queryFilterCurr.And(p => p.RevisionType == revisionType);
            queryFilterCurr = queryFilterCurr.And(p => p.UnitCode == unitCode);
            queryFilterCurr = queryFilterCurr.And(p => p.ProcessGroup == processGroup);
            queryFilterCurr = queryFilterCurr.And(p => p.GroupCode == groupCode);
            queryFilterCurr = queryFilterCurr.And(p => p.EmployeeID == employeeId);
            queryFilterCurr = queryFilterCurr.And(p => p.ProductionDate == date);

            var listCurrentDbProductionCard = _productionCardRepository.Get(queryFilterCurr).ToList();

            if (listCurrentDbProductionCard.Count() == 0) return;

            //// update current prodcard
            //var dbCurrentProductionCard = _productionCardRepository.GetByID(revisionType, locationCode,
            //       unitCode, brandCode, processGroup, groupCode, employeeId, date);
            //if (dbCurrentProductionCard == null) return;

            foreach (var dbCurrentProductionCard in listCurrentDbProductionCard)
            {
                dbCurrentProductionCard.Remark = Remark;
                _productionCardRepository.Update(dbCurrentProductionCard);

            }

            
            _uow.SaveChanges();
            // update current prodcard
        }

        public void DeleteProductionCardsByReturnPlantVerification(GetProductionCardInput input) 
        {
            // Set Query Filter Production Card
            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.Unit))
                queryFilter = queryFilter.And(p => p.UnitCode == input.Unit);

            if (!string.IsNullOrEmpty(input.Brand))
                queryFilter = queryFilter.And(p => p.BrandCode == input.Brand);

            if (!string.IsNullOrEmpty(input.Process))
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.Process);

            if (!string.IsNullOrEmpty(input.Group))
                queryFilter = queryFilter.And(p => p.GroupCode == input.Group);

            if (input.Date != DateTime.MinValue)
                queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.ProductionDate) == DbFunctions.TruncateTime(input.Date));

            var dbResult = _productionCardRepository.Get(queryFilter);

            foreach (var item in dbResult)
            {
                _productionCardRepository.Delete(item);
            }

            _uow.SaveChanges();
        }

        public void DeleteProdCardByReturnVerificationRevType(GetProductionCardInput input, string productionEntryCode, string userName)
        {
            // Set Query Filter Production Card
            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.Unit))
                queryFilter = queryFilter.And(p => p.UnitCode == input.Unit);

            if (!string.IsNullOrEmpty(input.Brand))
                queryFilter = queryFilter.And(p => p.BrandCode == input.Brand);

            if (!string.IsNullOrEmpty(input.Process))
                queryFilter = queryFilter.And(p => p.ProcessGroup == input.Process);

            if (!string.IsNullOrEmpty(input.Group))
                queryFilter = queryFilter.And(p => p.GroupCode == input.Group);

            if (input.Date != DateTime.MinValue)
                queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.ProductionDate) == DbFunctions.TruncateTime(input.Date));

            var dbResult = _productionCardRepository.Get(queryFilter);

            // Checking Revision Type
            // Delete the biggest/MAX revision type
            if (dbResult.Any()) 
            {
                var maxRevisionType = dbResult.Max(c => c.RevisionType);

                var translog = _utilTransactionLogRepository.Get(c => c.TransactionCode == productionEntryCode && c.IDFlow == (int)Enums.IdFlow.EblekReleaseApprovalFinal);

                if (translog.Any())
                    dbResult = dbResult.Where(c => c.RevisionType == maxRevisionType && maxRevisionType != 0);
                else
                    dbResult = dbResult.Where(c => c.RevisionType == maxRevisionType);

                foreach (var item in dbResult.Where(c => c.RevisionType == maxRevisionType))
                {
                    // Checking condition remark
                    // http://tp.voxteneo.co.id/entity/11050
                    if (!String.IsNullOrEmpty(item.Remark))
                    {
                        var splitRemark = item.Remark.Split(';');

                        if (splitRemark.Length > 1)
                        {
                            for (int i = 1; i < splitRemark.Length; i++)
                            {
                                var queryFilter1 = PredicateHelper.True<ProductionCard>();

                                if (!string.IsNullOrEmpty(input.LocationCode))
                                    queryFilter1 = queryFilter1.And(p => p.LocationCode == input.LocationCode);

                                if (!string.IsNullOrEmpty(input.Unit))
                                    queryFilter1 = queryFilter1.And(p => p.UnitCode == input.Unit);

                                if (!string.IsNullOrEmpty(input.Brand))
                                    queryFilter1 = queryFilter1.And(p => p.BrandCode == input.Brand);

                                if (!string.IsNullOrEmpty(input.Process))
                                    queryFilter1 = queryFilter1.And(p => p.ProcessGroup == input.Process);

                                if (!string.IsNullOrEmpty(input.Group))
                                    queryFilter1 = queryFilter1.And(p => p.GroupCode == input.Group);

                                queryFilter1 = queryFilter1.And(p => p.EmployeeID == item.EmployeeID);

                                var prodDate = DateTime.ParseExact(splitRemark[i], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                queryFilter1 = queryFilter1.And(p => DbFunctions.TruncateTime(p.ProductionDate) == DbFunctions.TruncateTime(prodDate));

                                var dbResult1 = _productionCardRepository.Get(queryFilter1).FirstOrDefault();

                                if (dbResult1 != null)
                                {
                                    dbResult1.Remark = null;
                                    dbResult1.UpdatedDate = DateTime.Now;
                                    dbResult1.UpdatedBy = userName;
                                    _productionCardRepository.Update(dbResult1);
                                }
                            }
                        }
                    }
                    _productionCardRepository.Delete(item);
                }

                _uow.SaveChanges();
            }
        }

        public void sendMail(GetProductionCardInput input, string currUserName, bool submit)
        {
            var buttonName = Enums.ButtonName.Submit.ToString().ToUpper();
            if (!submit)
            {
                buttonName = EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit);
            }
            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = input.LocationCode,
                BrandCode = input.Brand,
                KpsWeek = input.KPSWeek,
                KpsYear = input.KPSYear,
                Shift = input.Shift,
                UnitCode = input.Unit,
                FunctionName = Enums.PageName.ProductionCard.ToString(),
                ButtonName = buttonName,
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.ProductionCard),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.ProductionCard),
                Date = input.Date
            };

            // Get User, Email, Responsibility Destination Recipient
            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            // Get User and Email Current/Sender
            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            // Create Email Input
            var listEmail = new List<MailInput>();
            if (submit)
            {
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
                        BodyEmail = CreateBodyMailProductionCardApprovalDetail(emailInput)
                    };
                    listEmail.Add(email);
                }
            }
            else
            {
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
                        BodyEmail = CreateBodyMailProductionCardCancel(emailInput)
                    };
                    listEmail.Add(email);
                }
            }
            

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmailPlantTpu(mail);
            }
        }

        private string CreateBodyMailProductionCardApprovalDetail(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Card sudah dikirimkan, Silakan melanjutkan proses  berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("webrooturl/WagesProductionCardApprovalDetail/index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.UnitCode + "/"
                                                                   + "0"
                                                                   + "?"
                                                                   + "P3=inprogress"
                                                                   + "&P4=0"
                                                                   + "&P5=" + emailInput.Date.ToString("dd/MM/yyyy")
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        private string CreateBodyMailProductionCardCancel(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Card  sudah direturn,Silakan melakukan revisi " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("webrooturl/ProductionCard/index"
                                                                   + "/" + emailInput.LocationCode
                                                                   + "/" + emailInput.UnitCode
                                                                   + "/" + emailInput.Shift
                                                                   + "/" + emailInput.BrandCode
                                                                   + "/" + emailInput.KpsYear
                                                                   + "/" + emailInput.KpsWeek
                                                                   + "/" + emailInput.Date.ToString("yyyy-MM-dd")
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }
        #endregion

        #region Eblek Release

        public List<ExePlantProductionEntryVerificationDTO> GetExeProductionEntryReleases(GetExeProductionEntryReleaseInput input)
        {
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (input.Unit.HasValue)
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.Unit.ToString());
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

            if (input.Date.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ProductionDate == input.Date);
            }

            queryFilter = queryFilter.And(p => p.ExeProductionEntryRelease != null && p.ExeProductionEntryRelease.IsLocked == true);

            var dbResult = _exePlantProductionEntryVerificationRepository.Get(queryFilter);

            return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult); 

        }

        public List<ExeProductionEntryReleaseDTO> GetExeProductionEntryReleasesNew(GetExeProductionEntryReleaseInput input)
        {
            var queryFilter = PredicateHelper.True<ExeProductionEntryRelease>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.ExePlantProductionEntryVerification.LocationCode == input.LocationCode);
            }

            if (input.Unit.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ExePlantProductionEntryVerification.UnitCode == input.Unit.ToString());
            }

            if (input.Shift.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ExePlantProductionEntryVerification.Shift == input.Shift);
            }

            //if (input.KpsYear.HasValue)
            //{
            //    queryFilter = queryFilter.And(p => p.KPSYear == input.KpsYear);
            //}

            //if (input.KpsWeek.HasValue)
            //{
            //    queryFilter = queryFilter.And(p => p.KPSWeek == input.KpsWeek);
            //}

            if (input.Date.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ExePlantProductionEntryVerification.ProductionDate >= input.Date);
            }

            if (input.DateTo.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ExePlantProductionEntryVerification.ProductionDate <= input.DateTo);
            }

            //queryFilter = queryFilter.And(p => p.ExeProductionEntryRelease != null && p.ExeProductionEntryRelease.IsLocked == true);

            var dbResult = _exeProductionEntryReleaseRepository.Get(queryFilter);

            return Mapper.Map<List<ExeProductionEntryReleaseDTO>>(dbResult);

        }

        public ExeProductionEntryReleaseDTO DeleteExeProductionEntryRelease(ExeProductionEntryReleaseDTO data)
        {
            var dbResult = _exeProductionEntryRelease.GetByID(data.ProductionEntryCode);

            if (dbResult == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            _exeProductionEntryRelease.Delete(dbResult);

            _uow.SaveChanges();

            return data;
        }

        public bool checkIsLockedState(string productionEntryCode, bool? isChecked)
        {
            var dbResult = _exeProductionEntryRelease.GetByID(productionEntryCode);
            var data = false;
            if(dbResult != null){
                data = dbResult.IsLocked == false ? false : true;
            }
            else
            {
                data = isChecked.GetValueOrDefault() == false ? false : true;
            }
            return data;
        }

        public ExeProductionEntryReleaseDTO SaveExeProductionEntryRelease(ExeProductionEntryReleaseDTO data, bool? isChecked)
        {
            //hakim
            if (isChecked.HasValue)
            {
                var dbResult = _exeProductionEntryRelease.GetByID(data.ProductionEntryCode);

                if (dbResult == null && isChecked == true)
                {
                    data.CreatedDate = DateTime.Now;
                    data.UpdatedDate = DateTime.Now;
                    data.IsLocked = true;
                    var eblekRelease = Mapper.Map<ExeProductionEntryRelease>(data);
                    _exeProductionEntryRelease.Insert(eblekRelease);
                }

                if (dbResult != null && isChecked == false)
                {
                    if (dbResult.IsLocked == true)
                    {
                        _exeProductionEntryRelease.Delete(dbResult);
                    }
                }

                if (dbResult != null && isChecked == true) 
                {
                    dbResult.CreatedDate = DateTime.Now;
                    dbResult.UpdatedDate = DateTime.Now;
                    dbResult.IsLocked = dbResult.IsLocked == false ? false : true;
                    _exeProductionEntryRelease.Update(dbResult);
                }

                _uow.SaveChanges();
            }

            return data;

        }

        public ExeProductionEntryReleaseDTO UpdateExeProductionEntryRelease(ExeProductionEntryReleaseDTO data)
        {
            var dbResult = _exeProductionEntryRelease.GetByID(data.ProductionEntryCode);

            if (dbResult == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            dbResult.Remark = data.Remark;
            dbResult.IsLocked = false;

            _exeProductionEntryRelease.Update(dbResult);

            _uow.SaveChanges();

            return data;
        }

        public ProductionCardDTO UpdateProductionCardFromRemark(ProductionCardDTO cardDto)
        {
            if (string.IsNullOrEmpty(cardDto.Remark)) return cardDto;
            var datas = cardDto.Remark.Split(';');
            foreach (var data in datas)
            {
                DateTime dateTime;
                if (!DateTime.TryParseExact(data, Constants.DefaultDateOnlyFormat, new CultureInfo("en-US"), DateTimeStyles.None, out dateTime)) continue;
                DateTime productionDate = DateTime.ParseExact(data, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);

                var dbProductionCard = _productionCardRepository.GetByID(cardDto.RevisionType, cardDto.LocationCode,
                    cardDto.UnitCode, cardDto.BrandCode, cardDto.ProcessGroup, cardDto.GroupCode, cardDto.EmployeeID, productionDate);
                if (dbProductionCard == null) continue;

                cardDto.UpdatedDate = DateTime.Now;
                Mapper.Map(dbProductionCard,cardDto);
                dbProductionCard.Remark = "x";
                dbProductionCard.ProductionDate = productionDate;

                _productionCardRepository.Update(dbProductionCard);
                //_uow.SaveChanges();
            }
            return cardDto;
        }

        public ProductionCardDTO InsertProductionCard(ProductionCardDTO cardDto)
        {
            var dbProdCard = _productionCardRepository.GetByID(cardDto.RevisionType, cardDto.LocationCode,
                cardDto.UnitCode, cardDto.BrandCode, cardDto.ProcessGroup, cardDto.GroupCode, cardDto.EmployeeID,
                cardDto.ProductionDate);
            if (dbProdCard == null)
            {
                var prodCard = Mapper.Map<ProductionCard>(cardDto);
                prodCard.CreatedDate = DateTime.Now;
                prodCard.UpdatedDate = DateTime.Now;
                _productionCardRepository.Insert(prodCard);

                return Mapper.Map<ProductionCardDTO>(prodCard);
            }
            else
            {
                dbProdCard.UpdatedDate = DateTime.Now;
                Mapper.Map(cardDto, dbProdCard);
                _productionCardRepository.Update(dbProdCard);
                return Mapper.Map<ProductionCardDTO>(dbProdCard);
            }
        }

        public ProductionCardDTO GetProductionCardById(ProductionCardDTO cardDto)
        {
            var dbProdCard = _productionCardRepository.GetByID(cardDto.RevisionType, cardDto.LocationCode, cardDto.UnitCode, cardDto.BrandCode,
                cardDto.ProcessGroup, cardDto.GroupCode, cardDto.EmployeeID, cardDto.ProductionDate);

            if (dbProdCard == null) return new ProductionCardDTO();
            return Mapper.Map<ProductionCardDTO>(dbProdCard);
        }

        public ProductionCardDTO GetProductionCard(ProductionCardDTO cardDto)
        {
            var dbProdCard = _productionCardRepository.GetByID(cardDto.RevisionType, cardDto.LocationCode, cardDto.UnitCode, cardDto.BrandCode,
                cardDto.ProcessGroup, cardDto.GroupCode, cardDto.EmployeeID, cardDto.ProductionDate);

            if (dbProdCard == null) return null;
            return Mapper.Map<ProductionCardDTO>(dbProdCard);
        }

        public List<ExePlantProductionEntryVerificationDTO> GetPlantProdVerificationFromEntryRelease(GetExePlantProductionEntryVerificationInput input)
        {
            var result = new List<ExePlantProductionEntryVerification>();

            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();
            if (!string.IsNullOrEmpty(input.ProcessGroup))  queryFilter = queryFilter.And(p => p.ProcessGroup == input.ProcessGroup);
            if (!string.IsNullOrEmpty(input.LocationCode))  queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))      queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            if (!string.IsNullOrEmpty(input.BrandCode))     queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            if (input.Shift.HasValue)                       queryFilter = queryFilter.And(p => p.Shift == input.Shift);
            //if (input.Date.HasValue) queryFilter = queryFilter.And(p => p.ProductionDate >= input.Date);
            //if (input.DateTo.HasValue) queryFilter = queryFilter.And(p => p.ProductionDate <= input.DateTo);
            if (input.DatePopUp.HasValue) queryFilter = queryFilter.And(p => p.ProductionDate == input.DatePopUp);

            if (input.UtilTransactionLogsCannotNull == true)
            {
                var exePlantProductionEntryVerificationResult = _exePlantProductionEntryVerificationRepository.Get(queryFilter);

                if (exePlantProductionEntryVerificationResult != null)
                {
                    foreach (var exePlantProductionEntryVerification in exePlantProductionEntryVerificationResult)
                    {
                        var queryFilterTransLog = PredicateHelper.True<UtilTransactionLog>();

                        var maxRevisionTypeProdCard = GetLatestProdCardRevTypeForRelease(input.LocationCode, input.UnitCode, input.BrandCode, input.ProcessGroup, "", input.DatePopUp);

                        var transCodeProdCard = exePlantProductionEntryVerification.ProductionEntryCode + "/" + maxRevisionTypeProdCard;

                        var transactionCode = transCodeProdCard.Replace("EBL", "WPC");
                        //var transactionCode = exePlantProductionEntryVerification.ProductionEntryCode;

                        //queryFilterTransLog = queryFilterTransLog.And(c => c.TransactionCode == transactionCode && c.IDFlow == (int)Core.Enums.IdFlow.ProdEntryVerificationSubmitProdCard);
                        queryFilterTransLog = queryFilterTransLog.And(c => c.TransactionCode == transactionCode);

                        var utilTransactionLog = _utilTransactionLogRepository.Get(queryFilterTransLog).OrderByDescending(p => p.CreatedDate).FirstOrDefault();

                        if (utilTransactionLog != null) result.Add(exePlantProductionEntryVerification);
                    }
                }
            }

            return Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(result);
        }

        public int GetLatestProdCardRevTypeForRelease(string location, string unit, string brand, string process, string group, DateTime? productionDate)
        {
            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (!string.IsNullOrEmpty(location))
                queryFilter = queryFilter.And(m => m.LocationCode == location);

            if (!string.IsNullOrEmpty(unit))
                queryFilter = queryFilter.And(m => m.UnitCode == unit);

            if (!string.IsNullOrEmpty(brand))
                queryFilter = queryFilter.And(m => m.BrandCode == brand);

            if (!string.IsNullOrEmpty(process))
                queryFilter = queryFilter.And(m => m.ProcessGroup == process);

            if (!string.IsNullOrEmpty(group))
                queryFilter = queryFilter.And(m => m.GroupCode == group);

            if (productionDate != null)
                queryFilter = queryFilter.And(m => m.ProductionDate == productionDate);

            var prodCard = _productionCardRepository.Get(queryFilter).OrderByDescending(c => c.RevisionType).FirstOrDefault();

            if (prodCard != null)
                return prodCard.RevisionType;

            return 0;
        }

        #region WagesEblekRelease Emailing

        public void SendEmailWagesEblekRelease(GetUserAndEmailInput input, string currUserName)
        {
            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = input.LocationCode,
                Shift = input.Shift,
                UnitCode = "%",
                FunctionName = Enums.PageName.EblekRelease.ToString(),
                ButtonName = Enums.ButtonName.SendApproval.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.EblekRelease),
                Date = input.Date
            };

            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            queryFilter = queryFilter.And(p => p.ProductionEntryCode == input.ProductionEntryCode);
            //queryFilter = queryFilter.And(p => p.ProcessGroup == eblekReleaseApprovalToApprove.ProcessGroup);

            var entryVerification = _exePlantProductionEntryVerificationRepository.Get(queryFilter).FirstOrDefault();

            input.Process = entryVerification.ProcessGroup;
            input.FunctionName = emailInput.FunctionName;

            var userAndEmail = _sqlSPRepo.GetUserAndEmail(emailInput);

            var listEmail = new List<MailInput>();

            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            foreach (var item in userAndEmail)
            {
                input.Recipient = item.Name;
                input.IDResponsibility = item.IDResponsibility ?? 0;
                var email = new MailInput
                {
                    FromName = currUserEmail.UserAD,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailSendApprovalWagesEblekRelease(input)
                };
                listEmail.Add(email);
            }

            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        private string CreateBodyMailSendApprovalWagesEblekRelease(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Ada permohohan release lock Production Entry (Eblek): " + Environment.NewLine + Environment.NewLine);

            if (emailInput.FunctionName == Enums.PageName.EblekRelease.ToString())
            {
                bodyMail.Append("Remark: " + emailInput.Remark + Environment.NewLine + Environment.NewLine);
                bodyMail.Append(emailInput.FunctionName + " - webrooturl/EblekReleaseApproval/Index/"
                                                                       + emailInput.LocationCode + "/"
                                                                       + emailInput.UnitCode + "/"
                                                                       + emailInput.Shift + "/"
                                                                       + emailInput.Process + "/"
                                                                       + emailInput.Date.ToString("dd-MM-yyyy") + "/"
                                                                       + emailInput.IDResponsibility.ToString()
                                                                       + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        #endregion
        #endregion

        #region Production Card Approval



        public List<WagesProductionCardApprovalCompositeDTO> GetProductionCardApproval(string locationCode, string unitCode, int revisionType)
        {
            //var queryFilter = PredicateHelper.True<WagesProductionCardApprovalView>();

            //queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
            //queryFilter = queryFilter.And(p => p.UnitCode == unitCode);
            //queryFilter = queryFilter.And(p => p.RevisionType == revisionType);

            //var dbResult = _productionCardApprovalRepository.Get(queryFilter);
            var dbResult = _sqlSPRepo.GetWagesProductionCardApprovalView(locationCode, unitCode, revisionType.ToString());
            return Mapper.Map<List<WagesProductionCardApprovalCompositeDTO>>(dbResult);
        }

        public IEnumerable<RoleButtonWagesApprovalDetail_Result> GetButtonState(ButtonStateInput input)
        {
            return _sqlSPRepo.RoleButtonWagesApprovalDetail(input);
        }
        
        public List<WagesProductionCardApprovalCompositeDTO> GetProductionCardApprovalList(GetProductionCardApprovalListInput input)
        {
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];

            var dbResult = _sqlSPRepo.GetProductionCardApprovalList(strUserID.Username, input.StartDate, input.TransactionStatus, strUserID.Responsibility.IDResponsibility);
            var listApproval = dbResult
            .Select(g => new WagesProductionCardApprovalCompositeDTO
            {
                LocationCode = g.LocationCode,
                UnitCode = g.UnitCode,
                BrandCode = g.BrandCode,
                ProductionDate = g.ProductionDate.Value,
                Shift = g.Shift.Value,
                Status = g.Status ,
                IDRole = g.IDRole.HasValue ? g.IDRole.Value : 0 ,
                RolesName = g.RolesName,
                RevisionType = g.RevisionType
            }).OrderBy(x=>x.LocationCode).ToList();
            return listApproval;
            //var pcApproval = new List<WagesProductionCardApprovalCompositeDTO>();
            //const char multiSkill = '5';
            //var text = multiSkill.ToString();
            //UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            //var location = strUserID.Location.Where(x => x.Code.Contains("ID2")).Select(x => x.Code).ToList();
            //var units = strUserID.Responsibility.Location.Select(s => s.Units).FirstOrDefault();
            //var i=1;
            //var j=1;

            //GetMstPlantUnitsInput inputForUnit = new GetMstPlantUnitsInput();
            //List<WagesProductionCardApprovalCompositeDTO> listApproval = new List<WagesProductionCardApprovalCompositeDTO>();
            //List<string> validLocationUnit = new List<String>();
            //List<string> validUnit = new List<String>();

            //var year = input.StartDate.Year;
            //var week = _masterDataBll.GetWeekByDate(input.StartDate).Week;
            //int dayOfWeek = (int)input.StartDate.DayOfWeek;
            ////Cari Location dan Unit yang valid
            ////Location diambil dari User login yang mengandung "ID2"
            ////Unit diambil dari GetMstPlantUnits menggunakan location
            //for (i = 1; i <= location.Count(); i++)
            //{ 
            //    inputForUnit.LocationCode = location[i - 1];
            //    inputForUnit.UnitCode = units;
            //    var unitByLocation = _masterDataBll.GetMstPlantUnits(inputForUnit);
            //    for (j = 1; j <= unitByLocation.Count(); j++)
            //    {
            //        var unit = unitByLocation[j - 1].UnitCode;
            //        var totalTransactionlogValid = _utilTransactionLogRepository.Get(p => p.IDFlow == 17
            //                                                                && p.TransactionCode.Contains("EBL/" + inputForUnit.LocationCode + "/1/" + unit)
            //                                                                && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek))
            //                                                                .GroupBy(a => a.TransactionCode)
            //                                                                .Count();
            //        if (totalTransactionlogValid > 0)
            //        {
            //            validLocationUnit.Add(inputForUnit.LocationCode + "," + unit);
            //        }
            //    }
            //}

            //var getAllEntryVerification = _exePlantProductionEntryVerificationRepository.Get(p => p.ProductionDate == input.StartDate
            //                                                            && p.GroupCode.Substring(1, 1) != text).ToList();

            ////Ambil jumlah transaction log yang sudah submit
            //var getAllTotalTransactionlogSubmit = _utilTransactionLogRepository.Get(p => p.IDFlow == 22
            //                                                        && p.TransactionCode.Contains("WPC/")
            //                                                        && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/")).ToList();

            ////Ambil jumlah transaction log yang sudah cancel submit
            //var getAllTotalTransactionlogCancelSubmit = _utilTransactionLogRepository.Get(p => p.IDFlow == 57
            //                                                        && p.TransactionCode.Contains("WPC/")
            //                                                        && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/")).ToList();

            ////var entryVerification = _exePlantProductionEntryVerificationRepository.Get(p => p.ProductionDate == input.StartDate
            ////                                                            && p.GroupCode.Substring(1, 1) != text).ToList();

            //var idNextRole21 = 0;
            //var idNextRole22 = 0;
            //var idNextRole24 = 0;
            //var idNextRole25 = 0;
            //var idNextRole56 = 0;

            //if (units == null)
            //{
            //    idNextRole21 = _utilFlowRepository.Get(p => p.IDFlow == 21).FirstOrDefault().DestinationRole.Value;
            //    idNextRole22 = _utilFlowRepository.Get(p => p.IDFlow == 22).FirstOrDefault().DestinationRole.Value;
            //    idNextRole24 = _utilFlowRepository.Get(p => p.IDFlow == 24).FirstOrDefault().DestinationRole.Value;
            //    idNextRole25 = _utilFlowRepository.Get(p => p.IDFlow == 25).FirstOrDefault().DestinationRole.Value;
            //    idNextRole56 = _utilFlowRepository.Get(p => p.IDFlow == 56).FirstOrDefault().DestinationRole.Value;
            //}
            
            ////Looping sesuai dengan Location dan Unit yang valid
            //for (i = 1; i <= validLocationUnit.Count(); i++)
            //{
            //    String[] locationUnit = validLocationUnit[i - 1].Split(',');
            //    var locationCode = locationUnit[0];
            //    var unitCode = locationUnit[1];

            //    var totalEntryVerification = getAllEntryVerification.Where(q => q.ProductionDate == input.StartDate
            //                                                    && q.LocationCode == locationCode
            //                                                    && q.UnitCode == unitCode
            //                                                    && q.GroupCode.Substring(1, 1) != text)
            //                                                    .Count();

            //    var totalTransactionlogSubmit = getAllTotalTransactionlogSubmit.Where(p => p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //                                                    && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //                                                    .Count();

            //    var totalTransactionlogCancelSubmit = getAllTotalTransactionlogCancelSubmit.Where(p => p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //                                                            && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //                                                            .Count();

            //    var entryVerification = getAllEntryVerification.Where(p => p.LocationCode == locationCode
            //                                                            && p.UnitCode == unitCode)
            //                                                            .GroupBy(g => new { LocationCode = g.LocationCode, UnitCode = g.UnitCode, BrandCode = g.BrandCode, ProductionDate = g.ProductionDate, Shift = g.Shift });

            //    ////Ambil jumlah entry verification
            //    //var totalEntryVerification = _exePlantProductionEntryVerificationRepository.Get(p => p.ProductionDate == input.StartDate
            //    //                                                        && p.LocationCode == locationCode
            //    //                                                        && p.UnitCode == unitCode
            //    //                                                        && p.GroupCode.Substring(1, 1) != text)
            //    //                                                        .GroupBy(a => a.ProductionEntryCode)
            //    //                                                        .Count();

            //    ////Ambil jumlah transaction log yang sudah submit
            //    //var totalTransactionlogSubmit = _utilTransactionLogRepository.Get(p => p.IDFlow == 22
            //    //                                                        && p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //    //                                                        && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //    //                                                        //.GroupBy(a => a.TransactionCode)
            //    //                                                        .Count();

            //    ////Ambil jumlah transaction log yang sudah cancel submit
            //    //var totalTransactionlogCancelSubmit = _utilTransactionLogRepository.Get(p => p.IDFlow == 57
            //    //                                                        && p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //    //                                                        && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //    //                                                        //.GroupBy(a => a.TransactionCode)
            //    //                                                        .Count();

            //    ////Ambil data entryVerification untuk ditampilkan di Grid
            //    //var entryVerification = _exePlantProductionEntryVerificationRepository.Get(p => p.ProductionDate == input.StartDate
            //    //                                                        && p.LocationCode == locationCode
            //    //                                                        && p.UnitCode == unitCode
            //    //                                                        && p.GroupCode.Substring(1, 1) != text)
            //    //                                                        .GroupBy(g => new { LocationCode = g.LocationCode, UnitCode = g.UnitCode, BrandCode = g.BrandCode, ProductionDate = g.ProductionDate, Shift = g.Shift });

            //    ////Ambil jumlah transaction log yang sudah submit
            //    //var totalLastTransactionLogs = _utilTransactionLogRepository.Get(p => (p.IDFlow == 17 || p.IDFlow == 18 || p.IDFlow == 19 || p.IDFlow == 57)
            //    //                                                        && p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //    //                                                        && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //    //                                                        .OrderByDescending(p => p.UpdatedDate)
            //    //                                                        .Count();

            //    var status = "";
            //    var nextRole = "";
            //    int? idNextRole = 0;
            //    //var LastTransactionLogs = new List<int>();
            //    if (totalTransactionlogSubmit > 0)
            //    {
            //        var whereClause = _utilTransactionLogRepository.Get(p => p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //                                                            && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/")).Max(p => p.UpdatedDate);

            //        var LastTransactionLogs = _utilTransactionLogRepository.Get(p => p.UpdatedDate == whereClause).Select(p => p.IDFlow).ToList();

            //        //var util = _utilTransactionLogRepository.Get(p => p.UpdatedDate ).Where(p => p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //        //                                                    && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/")).Max(p => p.UpdatedDate)

            //        if (LastTransactionLogs.Contains(21) || LastTransactionLogs.Contains(22) || LastTransactionLogs.Contains(23) || LastTransactionLogs.Contains(69))
            //        {
            //            // ======================================= BEGIN : STEP 2 ===================================================//
            //            //Jika Jumlah entry verification - Jumlah transaction log submit - Jumlah transaction log cancel submit = 0, maka set status="Submitted"
            //            if (totalEntryVerification - (totalTransactionlogSubmit - totalTransactionlogCancelSubmit) == 0)
            //            {
            //                status = "SUBMITTED";
            //                idNextRole = units == null ? idNextRole22 : _utilFlowRepository.Get(p => p.IDFlow == 22).FirstOrDefault().DestinationRole;
            //            }

            //            //Jika Jumlah entry verification - Jumlah transaction log submit - Jumlah transaction log cancel submit, maka set status="Draft"
            //            else if (totalEntryVerification - (totalTransactionlogSubmit - totalTransactionlogCancelSubmit) > 0)
            //            {
            //                status = "DRAFT";
            //                idNextRole = units == null ? idNextRole21 : _utilFlowRepository.Get(p => p.IDFlow == 21).FirstOrDefault().DestinationRole;
            //            }
            //            // ======================================= END : STEP 2 ===================================================//
            //        }
            //        else
            //        {
            //            // ======================================= BEGIN : STEP 3 ===================================================//
            //            //Cek TransLog dengan IDFlow 56, jika ada set status="Approved"
            //            //var transactionLog56 = _utilTransactionLogRepository.Get(p => p.IDFlow == 56
            //            //                                                        && p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //            //                                                        && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //            //                                                        .GroupBy(a => a.TransactionCode)
            //            //                                                        .Count();

            //            //if (transactionLog56 > 0)
            //            if (LastTransactionLogs.Contains(56))
            //            {
            //                status = "APPROVED";
            //                idNextRole = units == null ? idNextRole56 : _utilFlowRepository.Get(p => p.IDFlow == 56).FirstOrDefault().DestinationRole;
            //            }

            //            //Cek TransLog dengan IDFlow 25, jika ada set status="Approved"
            //            //var transactionLog25 = _utilTransactionLogRepository.Get(p => p.IDFlow == 25
            //            //                                                        && p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //            //                                                        && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //            //                                                        .GroupBy(a => a.TransactionCode)
            //            //                                                        .Count();

            //            //if (transactionLog25 > 0)
            //            if (LastTransactionLogs.Contains(25))
            //            {
            //                status = "APPROVED";
            //                idNextRole = units == null ? idNextRole25 : _utilFlowRepository.Get(p => p.IDFlow == 25).FirstOrDefault().DestinationRole;
            //            }

            //            //Cek TransLog 26, jika ada set status="Completed"
            //            //var transactionLog26 = _utilTransactionLogRepository.Get(p => p.IDFlow == 26
            //            //                                                        && p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //            //                                                        && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //            //                                                        .GroupBy(a => a.TransactionCode)
            //            //                                                        .Count();

            //            //if (transactionLog26 > 0)
            //            if (LastTransactionLogs.Contains(26))
            //            {
            //                status = "COMPLETED";
            //                idNextRole = 0;
            //            }

            //            if (LastTransactionLogs.Contains(24))
            //            {
            //                status = "SUBMITTED";
            //                idNextRole = units == null ? idNextRole24 :  _utilFlowRepository.Get(p => p.IDFlow == 24).FirstOrDefault().DestinationRole;
            //            }

            //            if (LastTransactionLogs.Contains(57))
            //            {
            //                status = "DRAFT";
            //                idNextRole = units == null ? idNextRole21 : _utilFlowRepository.Get(p => p.IDFlow == 21).FirstOrDefault().DestinationRole;
            //            }
            //            // ======================================= END : STEP 3 ===================================================//
            //        }
            //    }
            //    else
            //    {
            //        status = "DRAFT";
            //        idNextRole = units == null ? idNextRole21 : _utilFlowRepository.Get(p => p.IDFlow == 21).FirstOrDefault().DestinationRole;
            //    }

            //    var userAdView = (idNextRole != 0) ? _userADView.Get(c => c.IDRole == idNextRole).FirstOrDefault() : null;
            //    nextRole = userAdView != null ? userAdView.RolesName : null;

            //    List<WagesProductionCardApprovalCompositeDTO> approvalListGroupBy2 = new List<WagesProductionCardApprovalCompositeDTO>();

            //    if (input.TransactionStatus == "completed" && status!="COMPLETED")
            //    {
            //        approvalListGroupBy2 = null;
            //    }
            //    else if (input.TransactionStatus == "inprogress" && status == "COMPLETED")
            //    {
            //        approvalListGroupBy2 = null;
            //    }
            //    else
            //    {
            //        //Ambil digit terakhir Transaction Code untuk dipakai sebagai Revision Type
            //        //var totalTransactionlogSubmitData = _utilTransactionLogRepository.Get(p => p.IDFlow == 22
            //        //                                                    && p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //        //                                                    && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //        //                                                    .Select(b => new {TransactionCode = b.TransactionCode.Substring(b.TransactionCode.Length-1,1)})
            //        //                                                    .ToList();

            //        var totalTransactionlogSubmitData = getAllTotalTransactionlogSubmit.Where(p => p.TransactionCode.Contains("WPC/" + locationCode + "/1/" + unitCode)
            //                                                            && p.TransactionCode.Contains(year + "/" + week + "/" + dayOfWeek + "/"))
            //                                                            .Select(b => new { TransactionCode = b.TransactionCode.Substring(b.TransactionCode.Length - 1, 1) })
            //                                                            .ToList();

            //        //Cari nilai Revision Type tertinggi
            //        String maxRevisionTypeString = totalTransactionlogSubmitData.Max(p => p.TransactionCode);
            //        int maxRevisionTypeInt = 0;
            //        if (maxRevisionTypeString != null)
            //        {
            //            maxRevisionTypeInt = Convert.ToInt32(maxRevisionTypeString);
            //        } 

            //        approvalListGroupBy2 = entryVerification
            //        .Select(g => new WagesProductionCardApprovalCompositeDTO
            //        {
            //            LocationCode = g.Key.LocationCode,
            //            UnitCode = g.Key.UnitCode,
            //            BrandCode = _masterDataBll.GetBrandGruopCodeByBrandCode(g.Key.BrandCode),
            //            ProductionDate = g.Key.ProductionDate,
            //            Shift = g.Key.Shift,
            //            Status = status,
            //            IDRole = idNextRole.Value,
            //            RolesName = nextRole,
            //            RevisionType = maxRevisionTypeInt
            //        }).ToList();
            //    }
            //    if (approvalListGroupBy2 !=null)
            //    {
            //        foreach (var list in approvalListGroupBy2)
            //        {
            //            listApproval.Add(list);
            //        }
            //    }
            //}

            //return listApproval;
        }
        
        /*
        public List<WagesProductionCardApprovalCompositeDTO> GetProductionCardApprovalList(GetProductionCardApprovalListInput input)
        {
            var pcApproval = new List<WagesProductionCardApprovalCompositeDTO>();
            const char multiSkill = '5';
            var text = multiSkill.ToString();
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var location = strUserID.Location.Where(x => x.Code.Contains("ID2")).Select(x => x.Code).ToList();
            var units = strUserID.Responsibility.Location.Select(s => s.Units).FirstOrDefault();
            var entryVerification = new List<ExePlantProductionEntryVerification>(); //filter by location and unitcode depend on responsibility rules
            if (units == null) // get all unit
            {
                // ReSharper disable ImplicitlyCapturedClosure
                entryVerification = _exePlantProductionEntryVerificationRepository.Get(p => (p.ProductionDate == input.StartDate)
                                                                            && location.Contains(p.LocationCode)
                                                                            && p.GroupCode.Substring(1, 1) != text).ToList();
            }
            else
            {
                entryVerification = _exePlantProductionEntryVerificationRepository.Get(p => (p.ProductionDate == input.StartDate)
                && location.Contains(p.LocationCode)
                && p.UnitCode == units
                && p.GroupCode.Substring(1, 1) != text).ToList();
            }

            //var translog = _utilTransactionLogRepository.Get(p => p.TransactionCode.Contains("WPC/ID22/1/2021/1111/FA010783.18/2016/37/5/0"))

            if (entryVerification != null)
            {
                foreach (var item in entryVerification)
                {
                    var approval = new WagesProductionCardApprovalCompositeDTO();
                    var brandGroup = _masterDataBll.GetBrandGruopCodeByBrandCode(item.BrandCode);
                    var latestRevType = 0;
                    //var c = strUserID.Responsibility; un used variable
                    //var unitLocations = strUserID.Responsibility.Location.Where(w => w.LocationData.LocationCode == item.LocationCode).Select(x => x.Units).Distinct(); //move before loop
                    var entryRelease = _exeProductionEntryRelease.Get(w => w.ProductionEntryCode == item.ProductionEntryCode && w.IsLocked == false).FirstOrDefault();
                    //if (!location.Contains(item.LocationCode) && (units.Contains(item.LocationCode) || !unitLocations.Contains(null))) continue;

                    approval.LocationCode = item.LocationCode;


                    if (entryRelease != null)
                    {
                        var latestPC = GetLatestProdCardRevType(item.LocationCode, item.UnitCode, item.BrandCode, item.ProcessGroup, item.GroupCode, item.ProductionDate);
                        latestRevType = latestPC.RevisionType;
                    }

                    approval.RevisionType = latestRevType;
                    approval.ProductionCardCode = item.ProductionEntryCode.Replace("EBL", "WPC");

                    approval.UnitCode = item.UnitCode;
                    approval.BrandCode = brandGroup;
                    approval.ProductionDate = item.ProductionDate;
                    approval.Shift = item.Shift;
                    pcApproval.Add(approval);
                }
            }

            #region old
            //var queryFilter = PredicateHelper.True<WagesProductionCardApprovalView>();

            //if (input.IsMyAction)
            //    queryFilter = queryFilter.And(p => p.UserAD == input.CurrentUser);
            //queryFilter = queryFilter.And(p => (DbFunctions.TruncateTime(p.ProductionDate) == DbFunctions.TruncateTime(input.StartDate) && p.RevisionType == 0) || (DbFunctions.TruncateTime(p.UpdatedDate.Value) == DbFunctions.TruncateTime(input.StartDate) && p.RevisionType != 0));
            //var dbResult = _productionCardApprovalRepository.Get(queryFilter);

            //var approvalList = Mapper.Map<List<WagesProductionCardApprovalCompositeDTO>>(dbResult);

            #endregion old
            var approvalListGroupBy = pcApproval
                .GroupBy(
                    c =>
                        new
                        {
                            c.RevisionType,
                            c.LocationCode,
                            c.UnitCode,
                            c.BrandCode,
                            c.ProductionDate,
                            c.Shift
                        })
                .Select(g => new WagesProductionCardApprovalCompositeDTO
                {
                    RevisionType = g.Key.RevisionType,
                    LocationCode = g.Key.LocationCode,
                    UnitCode = g.Key.UnitCode,
                    BrandCode = g.Key.BrandCode,
                    ProductionDate = g.Key.ProductionDate,
                    Shift = g.Key.Shift
                });

            List<WagesProductionCardApprovalCompositeDTO> listApproval = new List<WagesProductionCardApprovalCompositeDTO>();
            //var units = strUserID.Responsibility.Location.Select(s => s.Units).ToList(); // move to head function definition
            foreach (var list in approvalListGroupBy)
            {
                list.Status = GetStatusFromProductionApproval(list);
                //if (list.Status != "NOT IN PRODUCTION CARD" && (units.Contains(list.UnitCode) || units.Contains(null))) // simplicity by reSharper, enable when reSharper code do fail
                if (units == null)
                {
                    if (list.Status != "NOT IN PRODUCTION CARD")
                    {
                        listApproval.Add(list);
                    }
                }
                else
                {
                    if (list.Status != "NOT IN PRODUCTION CARD" && list.UnitCode == units)
                    {
                        listApproval.Add(list);
                    }
                }
            }

            if (input.TransactionStatus == "completed")
            {
                return listApproval.Where(p => p.Status == "COMPLETED").ToList();
            }

            // simplicity by reSharper, enable when reSharper code do fail
            //else if (input.TransactionStatus == "inprogress")
            //{
            //    return listApproval.Where(p => p.Status != "COMPLETED").ToList();
            //}

            //return listApproval;

            return input.TransactionStatus == "inprogress" ? listApproval.Where(p => p.Status != "COMPLETED").ToList() : listApproval;
        }
        */
        private string GetStatusFromProductionApproval(WagesProductionCardApprovalCompositeDTO dto)
        {
            var status = "";
            var flag = true;
            var submit = false;
            var approve = false;
            var complete = false;
            const char multiSkill = '5';
            var currUserSubmit = 0;
            var currUserApprove = 0;
            var currUserDraft = 0;
            var currUserComplete = 0;
            var text = multiSkill.ToString();
            var queryFilter = PredicateHelper.True<ProductionCard>();
            queryFilter = queryFilter.And(p => p.RevisionType == dto.RevisionType);
            queryFilter = queryFilter.And(p => p.LocationCode == dto.LocationCode);
            queryFilter = queryFilter.And(p => p.UnitCode == dto.UnitCode);
            queryFilter = queryFilter.And(p => p.ProductionDate == dto.ProductionDate);
            queryFilter = queryFilter.And(p => p.GroupCode.Substring(1, 1) != text);
            var dbResult = _productionCardRepository.Get(queryFilter);//.Select(p => p.ProductionCardCode).Distinct().ToList();
            var listEntryVerification = _exePlantProductionEntryVerificationRepository.Get(p => p.LocationCode == dto.LocationCode && p.UnitCode == dto.UnitCode && p.ProductionDate == dto.ProductionDate && p.GroupCode.Substring(1, 1) != text);

            var prodCardGroupby = dbResult
                                    .GroupBy(c => new { c.ProductionCardCode, c.RevisionType })
                                    .Select(g => new { g.Key.ProductionCardCode, g.Key.RevisionType }).Distinct();

            if (prodCardGroupby.Count() == 0) return "NOT IN PRODUCTION CARD";
            foreach (var item in prodCardGroupby)
            {
                var translogProdCard = item.ProductionCardCode + "/" + item.RevisionType;
                var latestProdCard = _utilitiesBll.GetLatestActionTransLogExceptSave(translogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCard.ToString());
                var latestProdCardApprovalDetail = _utilitiesBll.GetLatestActionTransLogExceptSave(translogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCardApprovalDetail.ToString());
                var returnList = _utilitiesBll.GetTransactionLogsByTransactionCode(translogProdCard.Replace("EBL", "WPC")).Where(x => x.IDFlow == 57);
                var latestIdFlow = _utilitiesBll.GetTransactionLogByTransactionCode(translogProdCard.Replace("EBL", "WPC")) != null ? _utilitiesBll.GetTransactionLogByTransactionCode(translogProdCard.Replace("EBL", "WPC")).IDFlow : 0;
                var isReturn = (returnList.Count() != 0) ? true : false;
                submit = (latestProdCard != null) ? latestProdCard.UtilFlow.UtilFunction.FunctionName == Enums.ButtonName.Submit.ToString() : false;
                if (latestProdCard != null)
                {
                    if (submit)
                    {
                        currUserSubmit = latestProdCard.UtilFlow.DestinationRole.HasValue ? latestProdCard.UtilFlow.DestinationRole.Value : 0;
                        //isReturn = (isReturn) ? false : true;
                    }
                    else
                    {
                        currUserDraft = latestProdCard.UtilFlow.DestinationRole.HasValue ? latestProdCard.UtilFlow.DestinationRole.Value : 0;
                        flag = submit;
                    }
                }
                else
                {
                    flag = false;
                    currUserDraft = 7;
                }
                var listIdFlowComplete = new List<int?>() { 26, 70 }; // change idflow for complete in 26 or 70
                if (isReturn)
                {
                    //approve = (latestProdCardApprovalDetail != null) ? latestIdFlow == 56 : false; // simplicity by reSharper, enable when reSharper code do fail
                    approve = (latestProdCardApprovalDetail != null) && latestIdFlow == 56;

                    if (approve) currUserApprove = latestProdCardApprovalDetail.UtilFlow.DestinationRole.HasValue ? latestProdCardApprovalDetail.UtilFlow.DestinationRole.Value : 0;
                    else
                    {
                        //approve = (latestProdCardApprovalDetail != null) ? latestIdFlow == 25 : false; // simplicity by reSharper, enable when reSharper code do fail
                        approve = (latestProdCardApprovalDetail != null) && latestIdFlow == 25;
                        if (approve) currUserApprove = latestProdCardApprovalDetail.UtilFlow.DestinationRole.HasValue ? latestProdCardApprovalDetail.UtilFlow.DestinationRole.Value : 0;
                    }
                    //complete = (latestProdCardApprovalDetail != null) ? latestIdFlow == 70 : false; // simplicity by reSharper, enable when reSharper code do fail
                    complete = (latestProdCardApprovalDetail != null) && listIdFlowComplete.Contains(latestIdFlow); // change idflow for complete in 26 or 70
                    if (complete) currUserComplete = latestProdCardApprovalDetail.UtilFlow.DestinationRole.HasValue ? latestProdCardApprovalDetail.UtilFlow.DestinationRole.Value : 0;
                }
                else
                {
                    //approve = (latestProdCardApprovalDetail != null) ? latestProdCardApprovalDetail.IDFlow == 56 : false; // simplicity by reSharper, enable when reSharper code do fail
                    approve = (latestProdCardApprovalDetail != null) && latestProdCardApprovalDetail.IDFlow == 56;

                    if (approve) currUserApprove = latestProdCardApprovalDetail.UtilFlow.DestinationRole.HasValue ? latestProdCardApprovalDetail.UtilFlow.DestinationRole.Value : 0;
                    else
                    {
                        //approve = (latestProdCardApprovalDetail != null) ? latestProdCardApprovalDetail.IDFlow == 25 : false; // simplicity by reSharper, enable when reSharper code do fail
                        approve = (latestProdCardApprovalDetail != null) && latestProdCardApprovalDetail.IDFlow == 25;
                        if (approve) currUserApprove = latestProdCardApprovalDetail.UtilFlow.DestinationRole.HasValue ? latestProdCardApprovalDetail.UtilFlow.DestinationRole.Value : 0;
                    }
                    //complete = (latestProdCardApprovalDetail != null) ? latestProdCardApprovalDetail.IDFlow == 70 : false; // simplicity by reSharper, enable when reSharper code do fail
                    complete = (latestProdCardApprovalDetail != null) && listIdFlowComplete.Contains(latestProdCardApprovalDetail.IDFlow); // change idflow for complete in 26 or 70
                    if (complete) currUserComplete = latestProdCardApprovalDetail.UtilFlow.DestinationRole.HasValue ? latestProdCardApprovalDetail.UtilFlow.DestinationRole.Value : 0;
                }
            }
            if (listEntryVerification.Count() != prodCardGroupby.Count())
            {
                flag = false;
                currUserDraft = 7;
            }
            submit = flag;
            var roleId = 0;
            if (!submit && !approve && !complete)
            {
                status = "DRAFT";
                //dto.UserAD = currUserDraft;
                roleId = currUserDraft;
            }

            if (submit && !approve)
            {
                status = "SUBMITTED";
                //dto.UserAD = currUserSubmit;
                roleId = currUserSubmit;
            }

            if (approve)
            {
                status = "APPROVED";
                //dto.UserAD = currUserApprove;
                roleId = currUserApprove;
            }

            if (complete)
            {
                status = "COMPLETED";
                //dto.UserAD = currUserComplete;
                roleId = currUserComplete;
            }
            // simplicity by reSharper, enable when reSharper code do fail
            //var userADView = (roleId != 0) ? _userADView.Get(c => c.IDRole == roleId).FirstOrDefault() : null;
            //if (userADView != null)
            //    dto.RolesName = userADView.RolesName;
            //else
            //    dto.RolesName = null;

            var userAdView = (roleId != 0) ? _userADView.Get(c => c.IDRole == roleId).FirstOrDefault() : null;
            dto.RolesName = userAdView != null ? userAdView.RolesName : null;
            #region unused
            //var queryFilter = PredicateHelper.True<WagesProductionCardApprovalView>();
            //queryFilter = queryFilter.And(c => c.RevisionType == revisionType);
            ////queryFilter = queryFilter.And(c => c.ProductionCardCode == prodCardCode);
            //queryFilter = queryFilter.And(c => c.LocationCode == locationCode);
            //queryFilter = queryFilter.And(c => c.UnitCode == unitCode);
            //queryFilter = queryFilter.And(c => c.BrandCode == brandCode);
            //queryFilter = queryFilter.And(c => c.ProductionDate == prodDate);
            ////queryFilter = queryFilter.And(c => c.IDFlow == idFlow);

            //var minIdFlow = _productionCardApprovalRepository.Get(queryFilter).Min(c => c.IDFlow);
            //var status = _productionCardApprovalRepository.Get(queryFilter).Select(c => c.Status).FirstOrDefault();
            //var status = "";
            #endregion
            return status;
        }

        private string GetStatusProdCardApprovalFromEntryVerification(List<ExePlantProductionEntryVerificationDTO> dto)
        {
            var status = "";
            var flag = true;
            var submit = false;
            var approve = false;
            var approve2 = false;
            var complete = false;

            foreach (var item in dto)
            {
                var latestRevType = 0;
                var entryRelease = _exeProductionEntryRelease.Get(w => w.ProductionEntryCode == item.ProductionEntryCode && w.IsLocked == false).FirstOrDefault();
                if (entryRelease != null)
                    latestRevType = GetLatestProdCardRevType(item.LocationCode, item.UnitCode, item.BrandCode, item.ProcessGroup, item.GroupCode, item.ProductionDate).RevisionType;

                var translogProdCard = item.ProductionEntryCode + "/" + latestRevType;
                var latestProdCard = _utilitiesBll.GetLatestActionTransLogExceptSave(translogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCard.ToString());
                var latestProdCardApprovalDetail = _utilitiesBll.GetLatestActionTransLogExceptSave(translogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCardApprovalDetail.ToString());
                submit = (latestProdCard != null) ? latestProdCard.UtilFlow.UtilFunction.FunctionName ==
                        Enums.ButtonName.Submit.ToString() : false;
                if (latestProdCard != null)
                {
                    if (!submit)
                        flag = submit;
                }
                else
                {
                    flag = false;
                }
                approve = (latestProdCardApprovalDetail != null) ? latestProdCardApprovalDetail.IDFlow == 56 : false;

                if (!approve) 
                    approve2 = (latestProdCardApprovalDetail != null) ? latestProdCardApprovalDetail.IDFlow == 25 : false;
                    
                complete = (latestProdCardApprovalDetail != null) ? latestProdCardApprovalDetail.IDFlow == 26 : false;
            }
            
            submit = flag;
            var roleId = 0;
            if (!submit && !approve && !complete)
                status = "DRAFT";

            if (submit && !approve)
                status = "SUBMITTED";

            if (approve)
                status = "APPROVED1";

            if (approve2)
                status = "APPROVED2";

            if (complete)
                status = "COMPLETED";

            return status;
        }

        public List<WagesProductionCardApprovalDetailViewDTO> GetProductionCardApprovalDetail(GetProductionCardApprovalDetailInput input)
        {
            var queryFilter = PredicateHelper.True<WagesProductionCardApprovalDetailView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);

            if (input.ProductionDate.HasValue)
                queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate);

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);

            if (!string.IsNullOrEmpty(input.BrandCode) && input.BrandCode != input.BrandGroupCode)
                queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);

            var dbResult = _productionCardApprovalDetailRepository.Get(queryFilter);
            return Mapper.Map<List<WagesProductionCardApprovalDetailViewDTO>>(dbResult);

        }

        public List<WagesProductionCardApprovalDetailViewDTO> GetProductionCardApprovalBrand(
            GetProductionCardApprovalDetailInput input)
        {
            var queryFilter = PredicateHelper.True<WagesProductionCardApprovalDetailView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);

            if (input.ProductionDate.HasValue)
                queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate);

            var dbResult = _productionCardApprovalDetailRepository.Get(queryFilter);
            return Mapper.Map<List<WagesProductionCardApprovalDetailViewDTO>>(dbResult);
        }

        public List<WagesProductionCardApprovalDetailViewGroupDTO> GetProductionCardApprovalDetailGroup(GetProductionCardApprovalDetailInput input)
        {
            var queryFilter = PredicateHelper.True<WagesProductionCardApprovalDetailViewGroup>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);

            if (input.ProductionDate.HasValue)
                queryFilter = queryFilter.And(p => p.ProductionDate == input.ProductionDate);

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);

            if(input.RevisionType.HasValue)
                queryFilter = queryFilter.And(p => p.RevisionType == input.RevisionType);

            var dbResult = _productionCardApprovalDetailGroupRepository.Get(queryFilter);
            return Mapper.Map<List<WagesProductionCardApprovalDetailViewGroupDTO>>(dbResult);

        }

        public bool GetStatusApproval(string locationCode, string unitCode, DateTime productionDate)
        {
            var statusSubmit = false;
            var statusApprove = false;
            var statusApprove2 = false;
            var statusComplete = false;

            var multiSkill = '5';
            var text = multiSkill.ToString();
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();
            queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
            queryFilter = queryFilter.And(p => p.UnitCode == unitCode);
            queryFilter = queryFilter.And(p => p.ProductionDate == productionDate);
            queryFilter = queryFilter.And(p => p.GroupCode.Substring(1, 1) != text);

            var dbResult = _exePlantProductionEntryVerificationRepository.Get(queryFilter);
            var status = GetStatusProdCardApprovalFromEntryVerification(Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult));

            if (status == "SUBMITTED" || status == "APPROVED1")
                return true;
            else
                return false;
        }

        public bool GetStatusComplete(string locationCode, string unitCode, DateTime productionDate)
        {
            #region unused
            //var statusApprove = false;
            //var multiSkill = '5';
            //var text = multiSkill.ToString();
            //var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();
            //queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
            //queryFilter = queryFilter.And(p => p.UnitCode == unitCode);
            //queryFilter = queryFilter.And(p => p.ProductionDate == productionDate);
            //queryFilter = queryFilter.And(p => p.GroupCode.Substring(1, 1) != text);

            //var dbResult = _exePlantProductionEntryVerificationRepository.Get(queryFilter);

            ////Check Production Card Submitted
            //foreach (var item in dbResult)
            //{
            //    var latestRevType = GetLatestProdCardRevType(item.LocationCode, item.UnitCode,
            //                               item.BrandCode, item.ProcessGroup, item.GroupCode, item.ProductionDate);
            //    if (latestRevType != null)
            //    {
            //        var translogProdCard = item.ProductionEntryCode + "/" + latestRevType.RevisionType;
            //        var translogProdCardApproved = _utilitiesBll.GetLatestActionTransLogExceptSave(translogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCardApprovalDetail.ToString());
            //        statusApprove = (translogProdCardApproved != null) ? translogProdCardApproved.IDFlow == 25 : false;
            //    }
            //    else
            //    {
            //        statusApprove = false;
            //    }
            //}
            //return statusApprove;
            #endregion
            var statusSubmit = false;
            var statusApprove = false;
            var statusApprove2 = false;
            var statusComplete = false;

            var multiSkill = '5';
            var text = multiSkill.ToString();
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();
            queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
            queryFilter = queryFilter.And(p => p.UnitCode == unitCode);
            queryFilter = queryFilter.And(p => p.ProductionDate == productionDate);
            queryFilter = queryFilter.And(p => p.GroupCode.Substring(1, 1) != text);

            var dbResult = _exePlantProductionEntryVerificationRepository.Get(queryFilter);
            var status = GetStatusProdCardApprovalFromEntryVerification(Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult));

            if (status == "APPROVED2")
                return true;
            else
                return false;
        }

        public bool GetStatusReturn(string locationCode, string unitCode, DateTime productionDate)
        {
            #region unused
            //var statusComplete = false;
            //var multiSkill = '5';
            //var text = multiSkill.ToString();
            //var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();
            //queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
            //queryFilter = queryFilter.And(p => p.UnitCode == unitCode);
            //queryFilter = queryFilter.And(p => p.ProductionDate == productionDate);
            //queryFilter = queryFilter.And(p => p.GroupCode.Substring(1, 1) != text);

            //var dbResult = _exePlantProductionEntryVerificationRepository.Get(queryFilter);

            ////Check Production Card Submitted
            //foreach (var item in dbResult)
            //{
            //    var latestRevType = GetLatestProdCardRevType(item.LocationCode, item.UnitCode,
            //                               item.BrandCode, item.ProcessGroup, item.GroupCode, item.ProductionDate);
            //    if (latestRevType != null)
            //    {
            //        var translogProdCard = item.ProductionEntryCode + "/" + latestRevType.RevisionType;
            //        var translogProdCardCompleted = _utilitiesBll.GetLatestActionTransLogExceptSave(translogProdCard.Replace("EBL", "WPC"), Enums.PageName.ProductionCardApprovalDetail.ToString());
            //        statusComplete = (translogProdCardCompleted != null) ? translogProdCardCompleted.IDFlow == 26 : false;
            //    }
            //    else
            //    {
            //        statusComplete = false;
            //    }
            //}
            //return statusComplete;
            #endregion
            var statusSubmit = false;
            var statusApprove = false;
            var statusApprove2 = false;
            var statusComplete = false;

            var multiSkill = '5';
            var text = multiSkill.ToString();
            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();
            queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
            queryFilter = queryFilter.And(p => p.UnitCode == unitCode);
            queryFilter = queryFilter.And(p => p.ProductionDate == productionDate);
            queryFilter = queryFilter.And(p => p.GroupCode.Substring(1, 1) != text);

            var dbResult = _exePlantProductionEntryVerificationRepository.Get(queryFilter);
            var status = GetStatusProdCardApprovalFromEntryVerification(Mapper.Map<List<ExePlantProductionEntryVerificationDTO>>(dbResult));

            if (status == "COMPLETED")
                return false;
            else
                return true;
        }

        public void ApproveProductionCardApprovalDetail(string userName, List<ProductionCardDTO> prodCard, UserSession CurrentUser)
        {
            var distinct = prodCard.Select(p => p.ProductionCardCode).Distinct();
            //var locationCode = prodCard.Select(p => p.LocationCode).First().ToString();
            //var brandCode = prodCard.Select(p => p.BrandCode).First().ToString();
            //var shift = prodCard.Select(p => p.Shift).First().ToString();
            //var unitCode = prodCard.Select(p => p.UnitCode).First().ToString();
            var productionDate = prodCard.Select(p => p.ProductionDate).First();
            var raw = distinct.First().Split('/');
            var locationCode = raw[1];
            var shift = raw[2];
            var unitCode = raw[3];
            var brandCode = raw[5];
            var year = raw[6];
            var week = raw[7];
            
            bool isEmail = false;
            var idflow = 25; //default
            foreach(var data in distinct)
            {
                var prodCardCodeSplit = data.Split('/');
                var groupCode = prodCardCodeSplit[4];
                var dataBrandCode = prodCardCodeSplit[5];
                var queryFilter = PredicateHelper.True<ProductionCard>();
                queryFilter = queryFilter.And(m => m.ProductionCardCode == data);
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
                queryFilter = queryFilter.And(m => m.UnitCode == unitCode);
                queryFilter = queryFilter.And(m => m.GroupCode == groupCode);
                queryFilter = queryFilter.And(m => m.BrandCode == dataBrandCode);
                queryFilter = queryFilter.And(m => m.ProductionDate == productionDate);
                var pc = _productionCardRepository.Get(queryFilter).OrderByDescending(c => c.RevisionType).FirstOrDefault();

                _generalBll.ExeTransactionLog(new TransactionLogInput()
                {
                    page = Enums.PageName.ProductionCardApprovalDetail.ToString(),
                    ActionButton = Enums.ButtonName.Approve.ToString(),
                    UserName = userName,
                    TransactionCode = pc.ProductionCardCode + "/" + pc.RevisionType,
                    ActionTime = DateTime.Now,
                    TransactionDate = DateTime.Now,
                    IDRole = CurrentUser.Responsibility.Role
                });
                var translog = _utilitiesBll.GetTransactionLog(
                    new TransactionLogInput()
                    {
                        page = Enums.PageName.ProductionCardApprovalDetail.ToString(),
                        ActionButton = Enums.ButtonName.Approve.ToString(),
                        UserName = userName,
                        TransactionCode = pc.ProductionCardCode + "/" + pc.RevisionType,
                        ActionTime = DateTime.Now.AddMilliseconds(500),
                        TransactionDate = DateTime.Now.AddMilliseconds(500)
                    });
                var check = translog.OrderByDescending(m => m.TransactionDate).FirstOrDefault();
                if (check.IDFlow == 25 || check.IDFlow == 56)
                {
                    isEmail = true;
                    if (check.IDFlow.HasValue) idflow = check.IDFlow.Value;
                }
                else
                {
                    isEmail = false;
                }
            }
            #region send mail
            if (isEmail)
            {
                // Initial Input To Get Recipient User, Email, Responsibility
                var emailInput = new GetUserAndEmailInput
                {
                    LocationCode = locationCode,
                    BrandCode = brandCode,
                    KpsWeek = int.Parse(year),
                    KpsYear = int.Parse(week),
                    Shift = int.Parse(shift),
                    UnitCode = unitCode,
                    FunctionName = Enums.PageName.ProductionCardApprovalDetail.ToString(),
                    ButtonName = Enums.ButtonName.Approve.ToString(),
                    EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.ProductionCardApprovalDetail),
                    FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.ProductionCardApprovalDetail),
                    IDFlow = idflow,
                    Date = productionDate
                };

                // Get User, Email, Responsibility Destination Recipient
                var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

                // Get User and Email Current/Sender
                var username = userName.Substring(4);
                var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

                // Create Email Input
                var listEmail = new List<MailInput>();
                foreach (var item in listUserAndEmailDestination)
                {
                    emailInput.Recipient = item.Name;
                    emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                    var email = new MailInput
                    {
                        FromName = currUserEmail.UserAD,
                        FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                        ToName = item.Name,
                        ToEmailAddress = item.Email,
                        Subject = emailInput.EmailSubject,
                        BodyEmail = CreateBodyMailProductionCardApprovalDetailApprove(emailInput)
                    };
                    listEmail.Add(email);
                }

                foreach (var mail in listEmail)
                {
                    try
                    {
                        _sqlSPRepo.InsertEmail(mail);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            #endregion

        }

        private string CreateBodyMailProductionCardApprovalDetailApprove(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();
            var idNextRole = _utilFlowRepository.Get(p => p.IDFlow == emailInput.IDFlow).FirstOrDefault().DestinationRole;
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Card Approval sudah diapprove, Silakan melanjutkan proses  berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<p><a href= webrooturl/WagesProductionCardApprovalDetail/index/"
                                                                    + emailInput.LocationCode + "/"
                                                                    + emailInput.UnitCode + "/"
                                                                    + "0" + "/"
                                                                    + emailInput.IDResponsibility.ToString()
                                                                    + "?"
                                                                    + "P3=inprogress"
                                                                    + "&P4=0"
                                                                    + "&P5=" + emailInput.Date.ToString("dd/MM/yyyy") 
                                                                    + "&P6=" + "APPROVED"
                                                                    + "&P7=" + idNextRole + ">"
                                                                    + emailInput.FunctionNameDestination + "</a></p>"
                                                                    + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public void CompleteProductionCardApprovalDetail(string userName, List<ProductionCardDTO> prodCard, UserSession CurrentUser)
        {
            //var distinct = prodCard.Select(p => p.ProductionCardCode).Distinct();
            //foreach (var data in distinct)
            //{
            //    var queryFilter = PredicateHelper.True<ProductionCard>();
            //    queryFilter = queryFilter.And(m => m.ProductionCardCode == data);
            //    var pc = _productionCardRepository.Get(queryFilter).OrderByDescending(c => c.RevisionType).FirstOrDefault();

            //    _generalBll.ExeTransactionLog(new TransactionLogInput()
            //    {
            //        page = Enums.PageName.ProductionCardApprovalDetail.ToString(),
            //        ActionButton = Enums.ButtonName.Complete.ToString(),
            //        UserName = userName,
            //        TransactionCode = pc.ProductionCardCode + "/" + pc.RevisionType,
            //        ActionTime = DateTime.Now,
            //        TransactionDate = DateTime.Now,
            //        IDRole = CurrentUser.Responsibility.Role
            //    });
            //}

            var dsctDatatemp = prodCard.Select(p => new
            {
                ProductionCardCode = p.ProductionCardCode,
                RevisionType = p.RevisionType
            }).Distinct();

            foreach (var data in dsctDatatemp)
            {
                _generalBll.ExeTransactionLog(new TransactionLogInput()
                {
                    page = Enums.PageName.ProductionCardApprovalDetail.ToString(),
                    ActionButton = Enums.ButtonName.Complete.ToString(),
                    UserName = userName,
                    TransactionCode = data.ProductionCardCode + "/" + data.RevisionType,
                    ActionTime = DateTime.Now,
                    TransactionDate = DateTime.Now,
                    IDRole = CurrentUser.Responsibility.Role
                });
            }
        }

        public void ReturnProductionCardApprovalDetail(string userName, List<ProductionCardDTO> prodCard, UserSession CurrentUser)
        {
            var dsctDatatemp = prodCard.Select(p => new
            {
                ProductionCardCode = p.ProductionCardCode,
                RevisionType = p.RevisionType
            }).Distinct();
            foreach (var data in dsctDatatemp)
            {
                //the comment depend this ticket http://tp.voxteneo.co.id/entity/10455
                //_generalBll.ExeTransactionLog(new TransactionLogInput()
                //{
                //    page = Enums.PageName.ProductionCard.ToString(),
                //    ActionButton = EnumHelper.GetDescription(Enums.ButtonName.CancelSubmit),
                //    UserName = userName,
                //    TransactionCode = data.ProductionCardCode + "/" + data.RevisionType,
                //    ActionTime = DateTime.Now,
                //    TransactionDate = DateTime.Now,
                //    IDRole = CurrentUser.Responsibility.Role
                //});
               
                _generalBll.ExeTransactionLog(new TransactionLogInput()
                {
                    page = Enums.PageName.ProductionCardApprovalDetail.ToString(),
                    ActionButton = Enums.ButtonName.Return.ToString(),
                    UserName = userName,
                    TransactionCode = data.ProductionCardCode + "/" + data.RevisionType,
                    ActionTime = DateTime.Now.AddMilliseconds(500),
                    TransactionDate = DateTime.Now.AddMilliseconds(500),
                    IDRole = CurrentUser.Responsibility.Role
                });
            }
        }

        public ProductionCard GetLatestProdCardRevType(string location, string unit, string brand, string process, string group, DateTime? productionDate)
        {
            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (!string.IsNullOrEmpty(location))
                queryFilter = queryFilter.And(m => m.LocationCode == location);

            if (!string.IsNullOrEmpty(unit))
                queryFilter = queryFilter.And(m => m.UnitCode == unit);

            if (!string.IsNullOrEmpty(brand))
                queryFilter = queryFilter.And(m => m.BrandCode == brand);

            if (!string.IsNullOrEmpty(process))
                queryFilter = queryFilter.And(m => m.ProcessGroup == process);

            if (!string.IsNullOrEmpty(group))
                queryFilter = queryFilter.And(m => m.GroupCode == group);

            if (productionDate != null)
                queryFilter = queryFilter.And(m => m.ProductionDate == productionDate);

            var prodCard = _productionCardRepository.Get(queryFilter).OrderByDescending(c => c.RevisionType).FirstOrDefault();

            return prodCard;
        }
        #endregion

        #region Eblek Release Approval
        public IEnumerable<EblekReleaseApprovalDTO> GetEblekReleaseApproval(GetEblekReleaseApprovalInput input)
        {
            var queryFilter = PredicateHelper.True<ExeProductionEntryReleaseTransactLogsView>();

            if (!string.IsNullOrEmpty(input.LocationCode))  queryFilter = queryFilter.And(p => p.LocationCode   == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))      queryFilter = queryFilter.And(p => p.UnitCode       == input.UnitCode);
            if (input.Shift != null)                        queryFilter = queryFilter.And(p => p.Shift          == input.Shift.Value);
            if (!string.IsNullOrEmpty(input.Process))       queryFilter = queryFilter.And(p => p.ProcessGroup   == input.Process);
            
            queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.EblekDate) == DbFunctions.TruncateTime(input.Date));

            var dbResultEntryVerifications = _exeProductionEntryReleaseTransactLogsViewRepository.Get(queryFilter).ToList();

            var listEblekApproval = new List<ExeProductionEntryReleaseTransactLogsView>();

            var listProdEntryCode = dbResultEntryVerifications.Select(c => c.ProductionEntryCode).ToList();

            // Get The Latest Eblek Release Approval (only 1 in trasaction code, IdFlow = 27)
            foreach (var item in listProdEntryCode.Distinct())
            {
                string item1 = item;
                //var tempEblekReleaseApproval = dbResultEntryVerifications.Where(c => c.ProductionEntryCode == item1).ToList();
                var tempEblekReleaseApproval = dbResultEntryVerifications.Where(c => c.ProductionEntryCode == item1 && c.IsLocked == false).OrderByDescending(p => p.LogCreatedDate).FirstOrDefault();
                if (tempEblekReleaseApproval != null)
                {
                    if (tempEblekReleaseApproval.IDFlow == (int)HMS.SKTIS.Core.Enums.IdFlow.EblekReleaseApproval || tempEblekReleaseApproval.IDFlow == (int)HMS.SKTIS.Core.Enums.IdFlow.EblekReleaseSendApproval)
                        listEblekApproval.Add(tempEblekReleaseApproval);
                }
                
                //if (tempEblekReleaseApproval.Count() == 1)
                //{
                    //listEblekApproval.Add(tempEblekReleaseApproval.FirstOrDefault());
                //}
            }

            //var result = dbResultEntryVerifications.Where(c => c.IsLocked == false).ToList();

            return Mapper.Map<List<EblekReleaseApprovalDTO>>(listEblekApproval).ToList();
        }

        public EblekReleaseApprovalDTO ApproveEblekReleaseApproval(EblekReleaseApprovalDTO eblekReleaseApprovalToApprove, UserSession CurrentUser)
        {
            var eblekReleaseApprovalExisting = _exeProductionEntryReleaseRepository.GetByID(eblekReleaseApprovalToApprove.ProductionEntryCode);
            
            if (eblekReleaseApprovalExisting == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            eblekReleaseApprovalExisting.Remark = eblekReleaseApprovalToApprove.Remark;
            eblekReleaseApprovalExisting.UpdatedDate = DateTime.Now;
            //eblekReleaseApprovalExisting.IsLocked = true;
            

            _exeProductionEntryRelease.Update(eblekReleaseApprovalExisting);
            _uow.SaveChanges();

            //Generate Data for transaction log
            _generalBll.ExeTransactionLog(new TransactionLogInput()
            {
                page = HMS.SKTIS.Core.Enums.PageName.EblekReleaseApproval.ToString(),
                ActionButton = HMS.SKTIS.Core.Enums.ButtonName.Approve.ToString(),
                UserName = CurrentUser.Username.Replace(@"\\",@"\"),
                TransactionCode = eblekReleaseApprovalToApprove.ProductionEntryCode,
                IDRole = CurrentUser.Responsibility.Role
                
            });

            var queryFilter = PredicateHelper.True<ExePlantProductionEntryVerification>();

            queryFilter = queryFilter.And(p => p.ProductionEntryCode == eblekReleaseApprovalExisting.ProductionEntryCode);
            //queryFilter = queryFilter.And(p => p.ProcessGroup == eblekReleaseApprovalToApprove.ProcessGroup);

            var entryVerification = _exePlantProductionEntryVerificationRepository.Get(queryFilter).FirstOrDefault();
            
            if (entryVerification != null)
            {
                //entryVerification.VerifyManual = false;
                //entryVerification.VerifySystem = false;
                entryVerification.UpdatedDate = DateTime.Now;
                _exePlantProductionEntryVerificationRepository.Update(entryVerification);

                _uow.SaveChanges();
            }

            

            return eblekReleaseApprovalToApprove;
        }

        public EblekReleaseApprovalDTO ReturnEblekReleaseApproval(EblekReleaseApprovalDTO eblekReleaseApprovalToApprove, UserSession CurrentUser)
        {
            var eblekReleaseApprovalExisting = _exeProductionEntryReleaseRepository.GetByID(eblekReleaseApprovalToApprove.ProductionEntryCode);

            if (eblekReleaseApprovalExisting == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            eblekReleaseApprovalExisting.Remark = eblekReleaseApprovalToApprove.Remark;
            eblekReleaseApprovalExisting.UpdatedDate = DateTime.Now;
            eblekReleaseApprovalExisting.IsLocked = true;

            _exeProductionEntryRelease.Update(eblekReleaseApprovalExisting);
            _uow.SaveChanges();

            //Generate Data for transaction log
            _generalBll.ExeTransactionLog(new TransactionLogInput()
            {
                page = HMS.SKTIS.Core.Enums.PageName.EblekReleaseApproval.ToString(),
                ActionButton = HMS.SKTIS.Core.Enums.ButtonName.Return.ToString(),
                UserName = CurrentUser.Username,
                TransactionCode = eblekReleaseApprovalToApprove.ProductionEntryCode,
                IDRole = CurrentUser.Responsibility.Role
            });

            return eblekReleaseApprovalToApprove;
        }

        public void SendEmail(GetUserAndEmailInput input)
        {
            var userAndEmail = _sqlSPRepo.GetUserAndEmail(input);

            var listEmail = new List<MailInput>();

            var username = input.UserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(input.UserName)).FirstOrDefault();

            foreach (var item in userAndEmail)
            {
                input.Recipient = item.Name;
                input.IDResponsibility = item.IDResponsibility ?? 0;
                var email = new MailInput
                {
                    FromName = currUserEmail.UserAD,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = input.EmailSubject,
                    BodyEmail = CreateBodyMail(input)
                };
                listEmail.Add(email);
            }

            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        private string CreateBodyMail(GetUserAndEmailInput input)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + input.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            if (input.FunctionName == Enums.PageName.EblekReleaseApproval.ToString()) 
            {
                bodyMail.Append("Permohohan release lock Production Entry (Eblek) dikembalikan " + Environment.NewLine + Environment.NewLine);
                bodyMail.Append("Remark: " + input.Remark + Environment.NewLine + Environment.NewLine);
                bodyMail.Append(input.FunctionName + " - webrooturl/WagesEblekRelease/Index/"
                                                                       + input.LocationCode + "/"
                                                                       + input.UnitCode + "/"
                                                                       + input.Shift + "/"
                                                                       + input.Date.ToString("dd-MM-yyyy") + "/"
                                                                       + input.IDResponsibility.ToString()
                                                                       + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }

        public void SendEmailApprove(GetUserAndEmailInput input)
        {
            var userAndEmail = _sqlSPRepo.GetUserAndEmail(input);

            var listEmail = new List<MailInput>();

            var username = input.UserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(input.UserName)).FirstOrDefault();

            foreach (var item in userAndEmail)
            {
                // email to Production Entry Release Approval
                input.Recipient = item.Name;
                input.IDResponsibility = item.IDResponsibility ?? 0;
                input.FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.EblekReleaseApproval);
                var email = new MailInput
                {
                    FromName = currUserEmail.UserAD,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = input.EmailSubject,
                    BodyEmail = CreateBodyMailToEblekRelease(input)
                };
                listEmail.Add(email);
            }

            foreach (var item in userAndEmail)
            {
                // email to eblek
                input.Recipient = item.Name;
                input.IDResponsibility = item.IDResponsibility ?? 0;
                input.FunctionNameDestination = EnumHelper.GetDescription(Enums.PageName.PlantProductionEntry);
                var email = new MailInput
                {
                    FromName = currUserEmail.UserAD,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = input.EmailSubject,
                    BodyEmail = CreateBodyMailToEblek(input)
                };
                listEmail.Add(email);
            }

            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        private string CreateBodyMailToEblek(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Production Entry (Eblek)  sudah diunlock, silakan melakukan revisi: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/ExePlantProductionEntry/Index/"
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

        private string CreateBodyMailToEblekRelease(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Ada permohohan release lock Production Entry (Eblek): " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append(emailInput.FunctionNameDestination + ": webrooturl/EblekReleaseApproval/Index/"
                                                                   + emailInput.LocationCode + "/"
                                                                   + emailInput.UnitCode + "/"
                                                                   + emailInput.Shift + "/"
                                                                   + emailInput.Process + "/"
                                                                   + emailInput.Date.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString()
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");

            return bodyMail.ToString();
        }
        

        #endregion

        #region Wages Absent Report
        public List<GetWagesReportAbsentViewDTO> GetWagesReport(GetWagesReportAbsentViewInput input)
        {

            if (input.LocationCode == HMS.SKTIS.Core.Enums.LocationCode.PLNT.ToString())
            {
                input.LocationCode = null;
            }

            
            var dbResult = _sqlSPRepo.GetWagesAbsentReportDialy(input.DateFrom,input.LocationCode,input.UnitCode,input.Process);
            var resultdto = Mapper.Map<List<GetWagesReportAbsentViewDTO>>(dbResult);
            return resultdto;
        }

        public List<GetWagesReportAbsentViewDTO> GetWagesReportMore(GetWagesReportAbsentViewInput input)
        {
            var dbResult = _sqlSPRepo.GetWagesAbsentReportMore(input.DateFrom, input.DateTo, input.LocationCode, input.UnitCode, input.Process).ToList();
            var resultdto = Mapper.Map<List<GetWagesReportAbsentViewDTO>>(dbResult);
            return resultdto;
        }

        public List<GetWagesReportAbsentGroupViewDTO> GetWagesReportGroup(GetWagesReportAbsentViewInput input)
        {
            var dbResult = _sqlSPRepo.GetWagesAbsentReportDetailDialy(input.DateFrom, input.DateTo, input.LocationCode, input.UnitCode, input.Process, input.ProdGroup).OrderBy(m => m.EmployeeName);
            var resultdto = Mapper.Map<List<GetWagesReportAbsentGroupViewDTO>>(dbResult);
            return resultdto;
        }

        public IEnumerable<WagesReportDetailEmployeeDTO> GetWagesReportDetailPerEmployee(string employeeID, DateTime startDate, DateTime endDate) 
        {
            var dbResult = _sqlSPRepo.GetWagesAbsentReportDetailDialy(startDate.Date, endDate.Date, employeeID);
            var resultdto = Mapper.Map<List<WagesReportDetailEmployeeDTO>>(dbResult);
            return resultdto;
        }
        #endregion

        #region Wages Report Summary Production Card

        //public List<string> GetProductionCardBrandGroupCode(GetWagesReportSummaryInput input)
        //{
        //    var queryFilter = PredicateHelper.True<ProductionCard>();

        //    if (input.FilterType != "Daily")
        //    {
        //        var listDateInWeek = _masterDataBll.GetDateByWeek(input.Year, input.Week);
        //        input.DateFrom = listDateInWeek.FirstOrDefault();
        //        input.DateTo = listDateInWeek.LastOrDefault();
        //    }

        //    //queryFilter =
        //    //      queryFilter.And(
        //    //          c => DbFunctions.TruncateTime(c.ProductionDate) >= DbFunctions.TruncateTime(input.DateFrom));

        //    //queryFilter =
        //    //    queryFilter.And(
        //    //        c => DbFunctions.TruncateTime(c.ProductionDate) <= DbFunctions.TruncateTime(input.DateTo));

        //    queryFilter = queryFilter.And(c => c.ProductionDate >= input.DateFrom);

        //    queryFilter =queryFilter.And(c => c.ProductionDate <= input.DateTo);

        //    // Get DB result ExeReportByGroup
        //    var dbResult = _productionCardRepository.Get(queryFilter).OrderBy(c => c.BrandGroupCode);

        //    // Init list brandGroupCode
        //    var brandGroupCodeList = new List<string>();

        //    // Get brandgroupcode list
        //    if (dbResult.Any())
        //        brandGroupCodeList = dbResult.Select(c => c.BrandGroupCode).Distinct().ToList();

        //    // Get DB result MstGenBrandGroup
        //    var dbMstGenBrandGroup = _mstGenBrandGroupRepo.Get(c => brandGroupCodeList.Contains(c.BrandGroupCode)).OrderBy(c => c.SKTBrandCode);

        //    // Init list SKT Brand Code
        //    var listSktBrandCode = new List<string>();

        //    // Get SKT Brand Code List
        //    if (dbMstGenBrandGroup.Any())
        //        listSktBrandCode = dbMstGenBrandGroup.Select(c => c.SKTBrandCode).Distinct().ToList();

        //    return listSktBrandCode;
        //}

        public List<string> GetProductionCardBrandGroupCode(GetWagesReportSummaryInput input)
        {
            var queryFilter = PredicateHelper.True<PlanPlantTargetProductionKelompok>();

            if (input.FilterType == "Daily")
            {
                input.Week = _masterDataBll.GetWeekByDate(input.DateFrom).Week.Value;
                input.Year = input.DateFrom.Year;

                input.WeekTo = _masterDataBll.GetWeekByDate(input.DateTo).Week.Value;
                input.YearTo = input.DateTo.Year;


                queryFilter = queryFilter.And(c => c.KPSWeek >= input.Week);
                queryFilter = queryFilter.And(c => c.KPSWeek <= input.WeekTo);

                queryFilter = queryFilter.And(c => c.KPSYear >= input.Year);
                queryFilter = queryFilter.And(c => c.KPSYear <= input.YearTo);
            }
            else
            {
                queryFilter = queryFilter.And(c => c.KPSWeek == input.Week);
                queryFilter = queryFilter.And(c => c.KPSYear == input.Year);
            }


            var dbResult = _planPlantTargetProductionKelompokRepo.Get(queryFilter).Distinct().ToList();

            var listSktBrandCode = new List<string>();
            if (dbResult.Any())
            {
                listSktBrandCode = (from data in dbResult
                                    join mstBrand in _mstGenBrandRepo.Get() on data.BrandCode equals mstBrand.BrandCode
                                    join mstBrandGroupCode in _mstGenBrandGroupRepo.Get() on mstBrand.BrandGroupCode equals mstBrandGroupCode.BrandGroupCode
                                    orderby mstBrandGroupCode.SKTBrandCode
                                    select mstBrandGroupCode.SKTBrandCode
                                    ).Distinct().ToList();
            }
             
            return listSktBrandCode;
        }

        public List<WagesReportSummaryDTO> GetWagesReportSummary(GetWagesReportSummaryInput input)
        {
            if (string.IsNullOrEmpty(input.BrandGroupCode))
            {
                return new List<WagesReportSummaryDTO>();
            }

            var queryFilter = PredicateHelper.True<ProductionCard>();

            if (input.FilterType != "Daily")
            {
                var listDateInWeek = _masterDataBll.GetDateByWeek(input.Year, input.Week);
                input.DateFrom = listDateInWeek.FirstOrDefault();
                input.DateTo = listDateInWeek.LastOrDefault();
            }

            queryFilter = queryFilter.And(c => c.ProductionDate >= input.DateFrom);

            queryFilter = queryFilter.And(c => c.ProductionDate <= input.DateTo);

            if (input.BrandGroupCode != "All")
            {
                var mstGenBrandGroup = _mstGenBrandGroupRepo.Get(c => c.SKTBrandCode == input.BrandGroupCode);
                var brandGroupCode = mstGenBrandGroup.Select(c => c.BrandGroupCode);

                queryFilter = queryFilter.And(c => brandGroupCode.Contains(c.BrandGroupCode));
            }

            var dbResult = _productionCardRepository.Get(queryFilter).OrderBy(x=>x.LocationCode);

            var result = new List<WagesReportSummaryDTO>();

            result.AddRange(CreateSummaryProductionCard(dbResult.ToList(), false, input.BrandGroupCode));
            result.AddRange(CreateSummaryProductionCard(dbResult.ToList(), true, input.BrandGroupCode));

            return result;
        }

        private List<WagesReportSummaryDTO> CreateSummaryProductionCard(List<ProductionCard> dbResult, bool isProductionCard, string filterBrand)
        {
            var result = new List<WagesReportSummaryDTO>();
            IEnumerable<ProductionCard> dtResult;

            dtResult = isProductionCard ? dbResult.Where(c => c.RevisionType == 0)
                        : dbResult.Where(c => c.RevisionType > 0);

            var mstGenProcess = _mstGenProcessRepo.Get();
            var uomResult = _processSettingsAndLocationViewRepo.Get();
            var listGroup = new List<WagesReportSummaryProductionCardDTO>();
            if (filterBrand != "All")
            {
                listGroup = dtResult
                    .Join(mstGenProcess, y => y.ProcessGroup, a => a.ProcessGroup, (y, a) =>
                    new
                    {
                        LocationCode = y.LocationCode,
                        BrandGroupCode = y.BrandGroupCode,
                        ProcessGroup = y.ProcessGroup,
                        ProcessIdentifier = a.ProcessIdentifier,
                        Produksi = y.Production,
                        UpahLain = y.UpahLain
                    }).OrderBy(a => a.ProcessIdentifier)
                    .GroupBy(y => new
                    {
                        y.LocationCode,
                        y.BrandGroupCode,
                        y.ProcessGroup,
                    })
                    .AsEnumerable()
                    .Select(x => new WagesReportSummaryProductionCardDTO()
                    {
                        Location = x.Key.LocationCode,
                        Process = x.Key.ProcessGroup,
                        Produksi = x.Sum(y => Convert.ToDecimal(y.Produksi)),
                        UpahLain = x.Sum(y => Convert.ToDecimal(y.UpahLain)),
                        BrandGroupCode = x.Key.BrandGroupCode
                    }).OrderBy(c => c.Location).ToList();
            }
            else
            {
                listGroup = dtResult
                    .Join(mstGenProcess, y => y.ProcessGroup, a => a.ProcessGroup, (y, a) =>
                    new
                    {
                        LocationCode = y.LocationCode,
                        BrandGroupCode = y.BrandGroupCode,
                        ProcessGroup = y.ProcessGroup,
                        ProcessIdentifier = a.ProcessIdentifier,
                        Produksi = y.Production,
                        UpahLain = y.UpahLain
                    }).OrderBy(a => a.ProcessIdentifier)
                    .GroupBy(y => new
                    {
                        y.LocationCode,
                        //y.BrandGroupCode,
                        y.ProcessGroup,
                    })
                    .AsEnumerable()
                    .Select(x => new WagesReportSummaryProductionCardDTO()
                    {
                        Location = x.Key.LocationCode,
                        Process = x.Key.ProcessGroup,
                        Produksi = x.Sum(y => Convert.ToDecimal(y.Produksi)),
                        UpahLain = x.Sum(y => Convert.ToDecimal(y.UpahLain)),
                        BrandGroupCode = x.Select(y => y.BrandGroupCode).FirstOrDefault() //x.Key.BrandGroupCode
                    }).OrderBy(c => c.Location).ToList();
            }

            foreach (var locationCode in listGroup.Select(c => c.Location).Distinct())
            {
                //get location abbr
                string locationAbbr = locationCode;
                var genLocation = _masterDataBll.GetMstLocationById(locationCode);
                if (genLocation != null)
                {
                    locationAbbr = genLocation.ABBR;
                }
                var summaryProductionCard = new WagesReportSummaryDTO();
                summaryProductionCard.IsProductionCard = isProductionCard;
                summaryProductionCard.Location = locationAbbr;

                foreach (var reportSummaryProductionCardDto in listGroup.Where(c => c.Location == locationCode))
                {
                    //get uomeblek
                    int? uomValue = uomResult.Where(x => x.LocationCode == reportSummaryProductionCardDto.Location
                    && x.BrandGroupCode == reportSummaryProductionCardDto.BrandGroupCode
                    && x.ProcessGroup == reportSummaryProductionCardDto.Process).Select(x => x.UOMEblek).FirstOrDefault();
                    // select total produksi & upah lain per process
                    var listDetail = new WagesReportSummaryProductionCardDTO();
                    listDetail.Location = locationAbbr;
                    listDetail.Process = reportSummaryProductionCardDto.Process;
                    listDetail.Produksi = Math.Round(reportSummaryProductionCardDto.Produksi.Value * uomValue.Value, 2);
                    listDetail.UpahLain = Math.Round(reportSummaryProductionCardDto.UpahLain.Value * uomValue.Value, 2);
                    //count total produksi & upah lain all process per location
                    if (isProductionCard)
                    {
                        summaryProductionCard.TotalProduksi = (summaryProductionCard.TotalProduksi == null ? 0 : summaryProductionCard.TotalProduksi) + listDetail.Produksi;
                        summaryProductionCard.TotalUpahLain = (summaryProductionCard.TotalUpahLain == null ? 0 : summaryProductionCard.TotalUpahLain) + listDetail.UpahLain;
                        summaryProductionCard.TotalProduksiCorrection = 0;
                        summaryProductionCard.TotalUpahLainCorrection = 0;
                    }
                    else
                    {
                        summaryProductionCard.TotalProduksi = 0;
                        summaryProductionCard.TotalUpahLain = 0;
                        summaryProductionCard.TotalProduksiCorrection = (summaryProductionCard.TotalProduksiCorrection == null ? 0 : summaryProductionCard.TotalProduksiCorrection) + listDetail.Produksi;
                        summaryProductionCard.TotalUpahLainCorrection = (summaryProductionCard.TotalUpahLainCorrection == null ? 0 : summaryProductionCard.TotalUpahLainCorrection) + listDetail.Produksi;
                    }
                    summaryProductionCard.ProductionCards.Add(listDetail);
                }
                result.Add(summaryProductionCard);
            }

            return result;
        }
        
        #endregion

        #region Available Position Number

        
        public List<GetWagesReportAvailablePositionNumberViewDTO> WagesReportAvailablePositionNumber(WagesReportAvailablePositionNumberInput input)
        {

            var dbResultAvailablePositionNumber = _sqlSPRepo.GetReportAvailablePositionNumberView1(input.LocationCode, input.GroupCode, input.UnitCode);

            return Mapper.Map<List<GetWagesReportAvailablePositionNumberViewDTO>>(dbResultAvailablePositionNumber);
        }

        #endregion
    }
}
