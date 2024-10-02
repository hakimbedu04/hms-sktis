using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Web.UI.WebControls;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System.Net.Mail;
using Enums = HMS.SKTIS.Core.Enums;
using System.Text;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace HMS.SKTIS.BLL
{
    public partial class MaintenanceBLL : IMaintenanceBLL
    {
        private IUnitOfWork _uow;
        private IMasterDataBLL _masterDataBll;
        private ISqlSPRepository _sqlSPRepo;

        private IGenericRepository<MntcEquipmentRequest> _equipmentRequestRepo;
        private IGenericRepository<MstMntcItemLocation> _mstItemLocationRepo;
        private IGenericRepository<MntcInventory> _maintenanceInventoryRepo;
        private IGenericRepository<EquipmentRequestView> _equipmentRequestViewRepo;
        private IGenericRepository<MntcEquipmentItemDisposal> _equipmentItemDisposal;
        private IGenericRepository<InventoryByStatusView> _inventoryByStatusViewRepo;
        private IGenericRepository<PlanPlantGroupShift> _planPlantGroupShiftRepo;
        private IGenericRepository<MntcEquipmentMovement> _equipmentMovementRepo;
        //private IGenericRepository<GetInventoryView_Result> _getInventoryViewRepo;
        private IGenericRepository<MntcEquipmentFulfillment> _maintenanceEquipmentFulfillmentRepository;
        private IGenericRepository<MntcEquipmentItemConvert> _mntcEquipmentItemConvertRepository;
        private IGenericRepository<MaintenanceItemConversionDestinationView> _maintenanceItemConversionDestinationViewRepository;
        private IGenericRepository<MaintenanceEquipmentStockView> _maintenanceEquipmentStockViewRepo;
        private IGenericRepository<MntcEquipmentQualityInspection> _maintenanceEquipmentQualityInspectionRepository;
        private IGenericRepository<MntcEquipmentRepair> _equipmentRepairRepo;
        private IGenericRepository<MntcRepairItemUsage> _equipmentRepairItemUsageRepo;
        private IGenericRepository<MntcInventory> _maintenanceInventoryRepository;
        private IGenericRepository<MntcRequestToLocation> _maintenanceRequestToLocation;
        private IGenericRepository<MaintenanceEquipmentFulfillmentView> _maintenanceEquipmentFulfillmentViewRepository;
        private IGenericRepository<MaintenanceEquipmentFulfillmentDetailView> _maintenanceEquipmentFulfillmentDetailViewRepository;
        private IGenericRepository<MntcRepairItemUsage> _maintenanceRepairItemUsageRepository;
        private IGenericRepository<MaintenanceExecutionInventoryView> _maintenanceExecutionInventoryViewRepository;
        private IGenericRepository<EquipmentRequirementView> _equipmentRequirementRepo;
        private IGenericRepository<MntcInventoryAdjustment> _maintenanceInventoryAdjustmentRepo;
        private IGenericRepository<MaintenanceEquipmentFulfillmentItemDetailView> _maintenanceEquipmentFulfillmentItemDetailRepo;
        private IGenericRepository<MntcInventoryAll> _mntcInventoryAllRepo;
        private IGenericRepository<MstMntcItem> _MstMntcItemRepo;
        private IGenericRepository<MstGenBrandPackageItem> _MstGenBrandPackageItemRepo;
        private IGenericRepository<MstADTemp> _mstAdTemp;
        private IGenericRepository<QueueCopyDeltaView> _queueCopyDeltaView;
        private IGenericRepository<ViewInventory> _ViewInventory;
        private IGenericRepository<MntcFulfillmentView> _mntcFulfillmentViewRepository;
   

        public MaintenanceBLL(IUnitOfWork uow, IMasterDataBLL masterDataBll)
        {
            _uow = uow;
            _masterDataBll = masterDataBll;
            _equipmentRequestRepo = _uow.GetGenericRepository<MntcEquipmentRequest>();
            _mstItemLocationRepo = _uow.GetGenericRepository<MstMntcItemLocation>();
            _maintenanceInventoryRepo = _uow.GetGenericRepository<MntcInventory>();
            _equipmentRequestViewRepo = _uow.GetGenericRepository<EquipmentRequestView>();
            _equipmentItemDisposal = _uow.GetGenericRepository<MntcEquipmentItemDisposal>();
            _inventoryByStatusViewRepo = _uow.GetGenericRepository<InventoryByStatusView>();
            _planPlantGroupShiftRepo = _uow.GetGenericRepository<PlanPlantGroupShift>();
            _equipmentMovementRepo = _uow.GetGenericRepository<MntcEquipmentMovement>();
            _maintenanceEquipmentFulfillmentRepository = _uow.GetGenericRepository<MntcEquipmentFulfillment>();
            _mntcEquipmentItemConvertRepository = _uow.GetGenericRepository<MntcEquipmentItemConvert>();
            _maintenanceItemConversionDestinationViewRepository = _uow.GetGenericRepository<MaintenanceItemConversionDestinationView>();
            _maintenanceEquipmentStockViewRepo = _uow.GetGenericRepository<MaintenanceEquipmentStockView>();
            _maintenanceEquipmentQualityInspectionRepository = _uow.GetGenericRepository<MntcEquipmentQualityInspection>();
            _equipmentRepairItemUsageRepo = _uow.GetGenericRepository<MntcRepairItemUsage>();
            _equipmentRepairRepo = _uow.GetGenericRepository<MntcEquipmentRepair>();
            _sqlSPRepo = _uow.GetSPRepository();
            _maintenanceInventoryRepository = _uow.GetGenericRepository<MntcInventory>();
            _maintenanceRequestToLocation = _uow.GetGenericRepository<MntcRequestToLocation>();
            _maintenanceEquipmentFulfillmentViewRepository = _uow.GetGenericRepository<MaintenanceEquipmentFulfillmentView>();
            _maintenanceEquipmentFulfillmentDetailViewRepository = _uow.GetGenericRepository<MaintenanceEquipmentFulfillmentDetailView>();
            _maintenanceRepairItemUsageRepository = _uow.GetGenericRepository<MntcRepairItemUsage>();
            _maintenanceExecutionInventoryViewRepository = _uow.GetGenericRepository<MaintenanceExecutionInventoryView>();
            _equipmentRequirementRepo = _uow.GetGenericRepository<EquipmentRequirementView>();
            _maintenanceInventoryAdjustmentRepo = _uow.GetGenericRepository<MntcInventoryAdjustment>();
            _maintenanceEquipmentFulfillmentItemDetailRepo =
            _uow.GetGenericRepository<MaintenanceEquipmentFulfillmentItemDetailView>();
            _mntcInventoryAllRepo = _uow.GetGenericRepository<MntcInventoryAll>();
            _MstMntcItemRepo = _uow.GetGenericRepository<MstMntcItem>();
            _MstGenBrandPackageItemRepo = _uow.GetGenericRepository<MstGenBrandPackageItem>();
            _mstAdTemp = _uow.GetGenericRepository<MstADTemp>();
            _queueCopyDeltaView = _uow.GetGenericRepository<QueueCopyDeltaView>();
            _ViewInventory = _uow.GetGenericRepository<ViewInventory>();
            _mntcFulfillmentViewRepository = _uow.GetGenericRepository<MntcFulfillmentView>();
        }

        #region Equipment Request

        public MntcEquipmentRequestCompositeDTO GetEquipmentRequest(GetEquipmentRequestsInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentRequest>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (input.RequestDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.RequestDate == input.RequestDate);
            }

            if (!string.IsNullOrEmpty(input.RequestNumber))
            {
                queryFilter = queryFilter.And(p => p.RequestNumber == input.RequestNumber);
            }

            if (!string.IsNullOrEmpty(input.Requestor))
            {
                queryFilter = queryFilter.And(p => p.CreatedBy == input.Requestor);
            }

            if (!string.IsNullOrEmpty(input.ItemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == input.ItemCode);
            }

            var dbResult = _equipmentRequestRepo.Get(queryFilter).FirstOrDefault();

            return Mapper.Map<MntcEquipmentRequestCompositeDTO>(dbResult);
        }

        public List<MntcEquipmentRequestCompositeDTO> GetEquipmentRequestsTable(GetEquipmentRequestsInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentRequest>();

            if (!string.IsNullOrEmpty(input.RequestNumber))
            {
                queryFilter = queryFilter.And(p => p.RequestNumber == input.RequestNumber);
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (input.RequestDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.RequestDate == input.RequestDate.Value);
            }

            queryFilter = queryFilter.And(p => p.QtyLeftOver >= 0);

            Func<IQueryable<MntcEquipmentRequest>, IOrderedQueryable<MntcEquipmentRequest>> orderByFilter = null;
            if (!string.IsNullOrEmpty(input.SortExpression) && !string.IsNullOrEmpty(input.SortOrder))
            {
                var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression },
                    input.SortOrder);
                orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentRequest>();
            }

            var dbResult = _equipmentRequestRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MntcEquipmentRequestCompositeDTO>>(dbResult);
        }

        /// <summary>
        /// Gets the equipment requests.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MntcEquipmentRequestCompositeDTO> GetEquipmentRequests(GetEquipmentRequestsInput input)
        {
            var queryFilter = PredicateHelper.True<EquipmentRequestView>();
            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (input.RequestDate.HasValue)
                queryFilter = queryFilter.And(m => m.RequestDate == input.RequestDate);

            Func<IQueryable<EquipmentRequestView>, IOrderedQueryable<EquipmentRequestView>> orderByFilter = null;
            if (string.IsNullOrEmpty(input.SortExpression) && string.IsNullOrEmpty(input.SortOrder))
            {
                input.SortExpression = "UpdatedDate";
                input.SortOrder = "DESC";
            }
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression },
                    input.SortOrder);
            orderByFilter = sortCriteria.GetOrderByFunc<EquipmentRequestView>();

            var dbResult = _equipmentRequestViewRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MntcEquipmentRequestCompositeDTO>>(dbResult);
        }

        /// <summary>
        /// Gets the equipment requests.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MntcEquipmentRequestCompositeDTO> GetEquipmentRequestsLocation(GetEquipmentRequestsInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentRequest>();
            queryFilter = queryFilter.And(m => m.Qty > 0);
            queryFilter = queryFilter.And(m => m.Qty - m.ApprovedQty > 0 || m.ApprovedQty == null);

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            Func<IQueryable<MntcEquipmentRequest>, IOrderedQueryable<MntcEquipmentRequest>> orderByFilter = null;
            if (!string.IsNullOrEmpty(input.SortExpression) && !string.IsNullOrEmpty(input.SortOrder))
            {
                var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression },
                    input.SortOrder);
                orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentRequest>();
            }

            var dbResult = _equipmentRequestRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MntcEquipmentRequestCompositeDTO>>(dbResult);
        }

        /// <summary>
        /// Gets the equipment request by identifier.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="BLLException">
        /// </exception>
        public MntcEquipmentRequestCompositeDTO GetEquipmentRequestById(GetEquipmentRequestByIdInput input)
        {
            EquipmentRequestView dbEquipmentRequest = null;
            var dbResults = _equipmentRequestViewRepo.Get(m => m.RequestDate == input.RequestDate
                                                                        && m.ItemCode == input.ItemCode
                                                                        && m.LocationCode == input.LocationCode
                                                                        && m.RequestNumber == input.RequestNumber);

            try
            {//Makes sure this list only contains one element
                dbEquipmentRequest = dbResults.SingleOrDefault();
            }//SingleOrDefault => If this exception is thrown, means there is a duplicate data !
            catch (InvalidOperationException)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DuplicateEntity);
            }

            if (dbEquipmentRequest == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<MntcEquipmentRequestCompositeDTO>(dbEquipmentRequest);
        }

        /// <summary>
        /// Inserts the equipment request.
        /// </summary>
        /// <param name="equipmentRequestDTO">The equipment request dto.</param>
        /// <returns></returns>
        public EquipmentRequestDTO InsertEquipmentRequest(EquipmentRequestDTO equipmentRequestDTO)
        {
            var existingData = _equipmentRequestRepo.GetByID(equipmentRequestDTO.RequestDate, equipmentRequestDTO.ItemCode, equipmentRequestDTO.LocationCode, equipmentRequestDTO.RequestNumber);

            if (existingData != null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
            }

            var dbEquipmentRequest = Mapper.Map<MntcEquipmentRequest>(equipmentRequestDTO);

            dbEquipmentRequest.CreatedDate = DateTime.Now;
            dbEquipmentRequest.UpdatedDate = DateTime.Now;

            _equipmentRequestRepo.Insert(dbEquipmentRequest);
            _uow.SaveChanges();

            Mapper.Map(dbEquipmentRequest, equipmentRequestDTO);
            return equipmentRequestDTO;
        }

        /// <summary>
        /// Updates the equipment request.
        /// </summary>
        /// <param name="equipmentRequestDTO">The equipment request dto.</param>
        /// <returns></returns>
        /// <exception cref="BLLException"></exception>
        public EquipmentRequestDTO UpdateEquipmentRequest(EquipmentRequestDTO equipmentRequestDTO, EquipmentFulfillmentCompositeDTO equipmentFulfillmentDto = null, bool isUpdate = false)
        {
            var dbEquipmentRequest = _equipmentRequestRepo.GetByID(equipmentRequestDTO.RequestDate, equipmentRequestDTO.ItemCode, equipmentRequestDTO.LocationCode, equipmentRequestDTO.RequestNumber);

            if (dbEquipmentRequest == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);


            //keep original CreatedBy and CreatedDate
            equipmentRequestDTO.CreatedBy = dbEquipmentRequest.CreatedBy;
            equipmentRequestDTO.CreatedDate = dbEquipmentRequest.CreatedDate;

            //set update time
            equipmentRequestDTO.UpdatedDate = DateTime.Now;

            if (equipmentFulfillmentDto != null)
            {
                //if (isUpdate)
                //{
                //    var dbFulfillment = _maintenanceEquipmentFulfillmentRepository.GetByID(equipmentRequestDTO.FulFillmentDate, equipmentRequestDTO.ItemCode, equipmentRequestDTO.RequestNumber);

                //    equipmentRequestDTO.ApprovedQty = (dbEquipmentRequest.ApprovedQty ?? 0) - ((dbFulfillment.PurchaseQty ?? 0) + (dbFulfillment.RequestToQty ?? 0));

                //    equipmentRequestDTO.ApprovedQty = equipmentRequestDTO.ApprovedQty +
                //                                      (equipmentFulfillmentDto.ApprovedQty ?? 0);
                //}
                //else
                //{
                //    // update Approved Quantity Value
                //    equipmentRequestDTO.ApprovedQty = (dbEquipmentRequest.ApprovedQty ?? 0) + (equipmentFulfillmentDto.ApprovedQty ?? 0);
                //}



                // update Fulfillment Quantity Value
                //equipmentRequestDTO.FullfillmentQty = dbEquipmentRequest.FullfillmentQty ??
                //                                      0 + equipmentRequestDTO.ApprovedQty ?? 0;
                equipmentRequestDTO.ApprovedQty = equipmentFulfillmentDto.ApprovedQty;
                equipmentRequestDTO.FullfillmentQty = equipmentRequestDTO.ApprovedQty;
                equipmentRequestDTO.QtyLeftOver = equipmentRequestDTO.FullfillmentQty;

                // update Outstanding Quantity Value
                equipmentRequestDTO.OutstandingQty = equipmentRequestDTO.Qty - equipmentRequestDTO.FullfillmentQty;

                if (equipmentRequestDTO.Qty < equipmentRequestDTO.FullfillmentQty)
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.RequestLessThanFulfillment);
                }
            }
            else if (dbEquipmentRequest.ApprovedQty > 0)
                throw new BLLException(ExceptionCodes.BLLExceptions.ApprovedQtyAlreadyExist);

            Mapper.Map(equipmentRequestDTO, dbEquipmentRequest);
            _equipmentRequestRepo.Update(dbEquipmentRequest);
            _uow.SaveChanges();

            return Mapper.Map<EquipmentRequestDTO>(dbEquipmentRequest);
        }

        public EquipmentRequestDTO UpdateEquipmentRequestFromFulfillmentEquipment(EquipmentRequestDTO equipmentRequestDTO)
        {
            var dbEquipmentFulfillment = _maintenanceEquipmentFulfillmentViewRepository.Get(m => m.ItemCode == equipmentRequestDTO.ItemCode && m.LocationCode == equipmentRequestDTO.LocationCode);
            var dbEquipmentRequest = _equipmentRequestRepo.GetByID(equipmentRequestDTO.RequestDate, equipmentRequestDTO.ItemCode, equipmentRequestDTO.LocationCode, equipmentRequestDTO.RequestNumber);

            if (dbEquipmentRequest == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            equipmentRequestDTO.CreatedBy = dbEquipmentRequest.CreatedBy;
            equipmentRequestDTO.CreatedDate = dbEquipmentRequest.CreatedDate;

            //set update time
            equipmentRequestDTO.UpdatedDate = DateTime.Now;

            equipmentRequestDTO.Qty = dbEquipmentRequest.Qty;

            //Update ApprovedQty
            equipmentRequestDTO.ApprovedQty = dbEquipmentFulfillment.Sum(m => m.ApprovedQty) ?? decimal.Zero;

            Mapper.Map(equipmentRequestDTO, dbEquipmentRequest);
            _equipmentRequestRepo.Update(dbEquipmentRequest);

            return Mapper.Map<EquipmentRequestDTO>(dbEquipmentRequest);
        }
        #endregion

        #region Inventory

        public InventoryByStatusViewDTO GetInventoryCurrentStock(GetInventoryCurrentStockInput input)
        {
            var dbResults = _inventoryByStatusViewRepo.Get(m => m.ItemCode == input.ItemCode && m.LocationCode == input.LocationCode);
            return Mapper.Map<InventoryByStatusViewDTO>(dbResults.FirstOrDefault());
        }
        #endregion

        #region Equipment Request Old

        /// <summary>
        /// Gets the equipment request dates.
        /// </summary>
        /// <returns></returns>
        public List<DateTime> GetEquipmentRequestDates()
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentRequest>();
            var dbResult = _equipmentRequestRepo.Get(queryFilter);
            List<DateTime> dates = new List<DateTime>();
            foreach (var item in dbResult)
            {
                if (!dates.Contains(item.RequestDate)) ;
                dates.Add(item.RequestDate);
            }
            return dates;
        }

        /// <summary>
        /// Gets the item code from location.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetItemCodeFromLocation()
        {
            var queryFilter = PredicateHelper.True<MstMntcItemLocation>();
            var dbResult = _mstItemLocationRepo.Get(queryFilter);

            var result = dbResult.GroupBy(lr => lr.LocationCode).ToDictionary(g => g.Key, g => g.Select(i => i.ItemCode).ToList());

            return result;
        }

        #endregion

        #region Item Disposal

        public List<MasterMaintenanceItemLocationDTO> GetItemLocations(string locationCode)
        {
            //var dbResult = _mstItemLocationRepo.Get(p => p.LocationCode == locationCode);
            //return Mapper.Map<List<MasterMaintenanceItemLocationDTO>>(dbResult);
            var dbResult = (from mstMntcItemLoc in _mstItemLocationRepo.Get(m => m.LocationCode == locationCode)
                            join mstMntcItem in _MstMntcItemRepo.Get() on mstMntcItemLoc.ItemCode equals mstMntcItem.ItemCode
                            select new MstMntcItem
                            {
                                ItemCode = mstMntcItemLoc.ItemCode,
                                ItemDescription = mstMntcItem.ItemDescription
                            }).OrderBy(m => m.ItemDescription).ToList();
            return Mapper.Map<List<MasterMaintenanceItemLocationDTO>>(dbResult);
        }

        public List<MntcEquipmentItemDisposalCompositeDTO> GetMntcEquipmentItemDisposals(GetItemDisposalInput input)
        {
            MstGenWeekDTO dateFrom = null;
            MstGenWeekDTO dateTo = null;

            var queryFilter = PredicateHelper.True<MntcEquipmentItemDisposal>();
            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (input.FilterType == "Year")
            {
                
                dateFrom = _masterDataBll.GetMstGenWeeks(new GetMstGenWeekInput
                {
                    Month = 1,
                    Year = input.Year
                }).OrderBy(p => p.StartDate).FirstOrDefault();

                dateTo = _masterDataBll.GetMstGenWeeks(new GetMstGenWeekInput
                {
                    Month = 12,
                    Year = input.Year
                }).OrderByDescending(p => p.EndDate).FirstOrDefault();

              
                //queryFilter = queryFilter.And(p => p.TransactionDate >= dateFrom.StartDate && p.TransactionDate <= dateTo.EndDate);

            }
            else if (input.FilterType == "Monthly")
            {
               
                    //DateTime DateFrom = Convert.ToDateTime(DateTime.Now.Year + "-" + input.DateFrom.Value.Month + "-" + "01");
                    //DateTime DateTo = Convert.ToDateTime(DateTime.Now.Year + "-" + input.DateTo.Value.Month + "-" +
                    //                       DateTime.DaysInMonth(DateTime.Now.Year, input.DateTo.Value.Month));

                     dateFrom = _masterDataBll.GetMstGenWeeks(new GetMstGenWeekInput
                    {
                        Month = input.MonthFrom,
                        Year = input.YearMonthFrom
                    }).OrderBy(p => p.StartDate).FirstOrDefault();

                     dateTo = _masterDataBll.GetMstGenWeeks(new GetMstGenWeekInput
                    {
                        Month = input.MonthTo,
                        Year = input.YearMonthTo
                    }).OrderByDescending(p => p.EndDate).FirstOrDefault();

                    //queryFilter = queryFilter.And(p => p.TransactionDate >= dateFrom.StartDate && p.TransactionDate <= dateTo.EndDate);
               
            }
            else if (input.FilterType == "Weekly")
            {
                if (input.WeekFrom != null && input.WeekTo != null)
                {
                     dateFrom = _masterDataBll.GetMstGenWeeks(new GetMstGenWeekInput
                    {
                        Week = input.WeekFrom,
                        Year = input.YearWeekFrom
                    }).FirstOrDefault();

                     dateTo = _masterDataBll.GetMstGenWeeks(new GetMstGenWeekInput
                    {
                        Week = input.WeekTo,
                        Year = input.YearWeekTo
                    }).FirstOrDefault();

                    //queryFilter = queryFilter.And(p => p.TransactionDate >= dateFrom.StartDate && p.TransactionDate <= dateTo.EndDate);
                }
            }
            else 
            {
                //if (input.DateFrom != null && input.DateTo != null)
                //{
                //    queryFilter = queryFilter.And(p => p.TransactionDate >= input.DateFrom && p.TransactionDate <= input.DateTo);
                //}
                ////mas bagus request, reference http://tp.voxteneo.co.id/entity/7101
                //if (input.DateTo != null)
                //{
                //    queryFilter = queryFilter.And(p => p.TransactionDate == input.DateTo);
                //}
                dateFrom = new MstGenWeekDTO {StartDate = input.DateFrom};
                dateTo = new MstGenWeekDTO {EndDate = input.DateTo};
            }

            queryFilter = queryFilter.And(p => p.TransactionDate >= dateFrom.StartDate && p.TransactionDate <= dateTo.EndDate);
         
            //queryFilter = queryFilter.And(
            //        p => !p.MstMntcItemLocation.MntcInventories.Any() || p.MstMntcItemLocation.MntcInventories.Any(q => q.ItemStatus == "Bad Stock"));

            //queryFilter = queryFilter.And(p => p.MstMntcItemLocation.MntcInventories.Any(x => x.InventoryDate == p.MstMntcItemLocation.MntcInventories.Max(y => y.InventoryDate)));

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);

            var orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentItemDisposal>();

            //var dbResult = _equipmentItemDisposal.Get(queryFilter, orderByFilter,
            //    "MstMntcItemLocation, MstMntcItemLocation.MstMntcItem, MstMntcItemLocation.MntcInventories");

            var dbResult = _equipmentItemDisposal.Get(queryFilter, orderByFilter);

            var itemStatus = EnumHelper.GetDescription(Enums.ItemStatus.BadStock).ToUpper();
            var unitCode = Enums.UnitCodeDefault.MTNC.ToString();
            //get inventory
            var queryFilterInventory = PredicateHelper.True<MntcInventory>();
            //queryFilterInventory = queryFilterInventory.And(p => p.InventoryDate == input.DateTo);
            queryFilterInventory = queryFilterInventory.And(p => p.InventoryDate >= dateFrom.StartDate && p.InventoryDate <= dateTo.EndDate);
            queryFilterInventory = queryFilterInventory.And(p => p.LocationCode == input.LocationCode);
            queryFilterInventory = queryFilterInventory.And(p => p.UnitCode == unitCode);
            queryFilterInventory = queryFilterInventory.And(p => p.ItemStatus == itemStatus);
            queryFilterInventory = queryFilterInventory.And(p => p.EndingStock >= 0);

            var dbInventory = _maintenanceInventoryRepo.Get(queryFilterInventory);

            var dbItem = _MstMntcItemRepo.Get();

            //join 
            var dbJoin = (from data in dbResult
                          join inventory in dbInventory on new {data.TransactionDate, data.ItemCode, data.LocationCode} 
                                                    equals new {TransactionDate = inventory.InventoryDate, inventory.ItemCode, inventory.LocationCode}
                       join mstMntcItem in dbItem on data.ItemCode equals mstMntcItem.ItemCode
                select new MntcEquipmentItemDisposalCompositeDTO()
                {
                    ItemCode = data.ItemCode,
                    LocationCode = data.LocationCode,
                    QtyDisposal = data.QtyDisposal,
                    ItemDescription = mstMntcItem.ItemDescription,
                    EndingStock = inventory.BeginningStock,
                    EndingStockPastDate = inventory.EndingStock - data.QtyDisposal,
                    Shift = data.Shift
                }).ToList();

            return dbJoin;

            //var dbReturn = (
            //    from x in dbResult
            //    group x by new
            //    {
            //        x.LocationCode,
            //        x.ItemCode,
            //        x.Shift
            //    }
            //    into grouped
            //    select new MntcEquipmentItemDisposalCompositeDTO()
            //    {
            //        ItemCode = grouped.Key.ItemCode,
            //        LocationCode = grouped.Key.LocationCode,
            //        QtyDisposal = grouped.Sum(y => y.QtyDisposal),
            //        ItemDescription = grouped.Max(y => y.MstMntcItemLocation.MstMntcItem.ItemDescription),
            //        EndingStock =
            //            grouped.Sum(
            //                src =>
            //                    src.MstMntcItemLocation.MntcInventories.OrderByDescending(m => m.InventoryDate)
            //                        .Where(
            //                            m =>
            //                                m.ItemStatus.ToUpper() ==
            //                                EnumHelper.GetDescription(Enums.ItemStatus.BadStock).ToUpper())
            //                        .Select(p => p.BeginningStock)
            //                        .FirstOrDefault()),
            //        Shift = grouped.Key.Shift
            //    }
            //    ).ToList();

            //return dbReturn;
        }

        public MntcEquipmentItemDisposalCompositeDTO InsertItemDisposal(MntcEquipmentItemDisposalCompositeDTO itemDisposalDto)
        {
            var dbMntcEquipmentItemDisposal = _equipmentItemDisposal.GetByID(itemDisposalDto.TransactionDate,
                itemDisposalDto.ItemCode, itemDisposalDto.LocationCode);

            if (dbMntcEquipmentItemDisposal != null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }

            itemDisposalDto.CreatedDate = DateTime.Now;
            itemDisposalDto.UpdatedDate = DateTime.Now;

            var itemDisposal = Mapper.Map<MntcEquipmentItemDisposal>(itemDisposalDto);

            _equipmentItemDisposal.Insert(itemDisposal);
            _uow.SaveChanges();
          

            //UpdateStockOutFromItemDisposal(itemDisposalDto);

            return itemDisposalDto;
        }

        public void UpdateStockOutFromItemDisposal(MntcEquipmentItemDisposalCompositeDTO itemDisposalDto)
        {
            var db = _maintenanceInventoryRepo.GetByID(
                itemDisposalDto.TransactionDate,
                EnumHelper.GetDescription(Core.Enums.ItemStatus.BadStock),
                itemDisposalDto.ItemCode,
                itemDisposalDto.LocationCode,
                Core.Enums.UnitCodeDefault.MTNC.ToString()
                );

            if (db != null)
            {
                var inventoryDto = new MaintenanceInventoryDTO
                {
                    InventoryDate = itemDisposalDto.TransactionDate,
                    ItemStatus = EnumHelper.GetDescription(Core.Enums.ItemStatus.BadStock).ToUpper(),
                    ItemCode = itemDisposalDto.ItemCode,
                    LocationCode = itemDisposalDto.LocationCode,
                    UnitCode = Core.Enums.UnitCodeDefault.MTNC.ToString(),
                    StockOut = itemDisposalDto.QtyDisposal
                };

                Mapper.Map(inventoryDto, db);
                _maintenanceInventoryRepo.Update(db);

                _uow.SaveChanges();
            }
        }

        public MntcEquipmentItemDisposalCompositeDTO UpdateItemDisposal(MntcEquipmentItemDisposalCompositeDTO itemDisposalDto)
        {
            var dbItemDisposal = _equipmentItemDisposal.GetByID(itemDisposalDto.TransactionDate, itemDisposalDto.ItemCode, itemDisposalDto.LocationCode);

            if (dbItemDisposal == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            // keep original value of CreatedBy & CreatedDate
            itemDisposalDto.CreatedBy = dbItemDisposal.CreatedBy;
            itemDisposalDto.CreatedDate = dbItemDisposal.CreatedDate;

            // set UpdatedDate
            itemDisposalDto.UpdatedDate = DateTime.Now;


            Mapper.Map(itemDisposalDto, dbItemDisposal);


            _equipmentItemDisposal.Update(dbItemDisposal);
            _uow.SaveChanges();

            return Mapper.Map<MntcEquipmentItemDisposalCompositeDTO>(dbItemDisposal);
        }

        #endregion

        #region Maintenance Inventory

        public MaintenanceInventoryDTO GetMntcInventoryAllView(GetMaintenanceInventoryInput input)
        {
            var queryFilter = PredicateHelper.True<MntcInventoryAll>();

            if (!string.IsNullOrEmpty(input.ItemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == input.ItemCode);
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.ItemStatus))
            {
                queryFilter = queryFilter.And(p => p.ItemStatus == input.ItemStatus);
            }

            if (input.InventoryDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.InventoryDate == input.InventoryDate);
            }

            queryFilter = queryFilter.And(p => p.UnitCode == "MTNC");
            var dbResult = _mntcInventoryAllRepo.Get(queryFilter).FirstOrDefault();
            return Mapper.Map<MaintenanceInventoryDTO>(dbResult);

        }

        public MaintenanceInventoryDTO GetMntcBadStock(GetMaintenanceInventoryInput input)
        {
            var queryFilter = PredicateHelper.True<MntcInventory>();

            if (!string.IsNullOrEmpty(input.ItemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == input.ItemCode);
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.ItemStatus))
            {
                queryFilter = queryFilter.And(p => p.ItemStatus == input.ItemStatus);
            }

            if (input.InventoryDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.InventoryDate == input.InventoryDate);
            }

            queryFilter = queryFilter.And(p => p.UnitCode == "MTNC");
            //var dbResult = _mntcInventoryAllRepo.Get(queryFilter);
            var dbResult = _maintenanceInventoryRepo.Get(queryFilter);

            var dbDeltaView = _sqlSPRepo.getDeltaView(input.LocationCode, input.InventoryDate.GetValueOrDefault());

            //join 
            var dbJoin = (from data in dbResult
                          join deltaView in dbDeltaView on new
                          {
                              data.InventoryDate,
                              data.LocationCode,
                              data.ItemCode,
                              data.UnitCode,
                              data.ItemStatus
                          }
                          equals new
                          {
                              InventoryDate = deltaView.InventoryDate,
                              LocationCode = deltaView.LocationCode,
                              ItemCode = deltaView.ItemCode,
                              UnitCode = deltaView.UnitCode,
                              ItemStatus = deltaView.ItemStatus
                          }
                          select new MaintenanceInventoryDTO()
                          {
                              InventoryDate = data.InventoryDate,
                              ItemStatus = data.ItemStatus,
                              ItemCode = data.ItemCode,
                              LocationCode = data.LocationCode,
                              UnitCode = data.UnitCode,
                              BeginningStock = data.BeginningStock,
                              StockIn = data.StockIn,
                              StockOut = data.StockOut,
                              EndingStock = data.BeginningStock + deltaView.DEndingStock,
                          }).FirstOrDefault();

            return Mapper.Map<MaintenanceInventoryDTO>(dbJoin);

        }

        public List<MaintenanceInventoryDTO> GetReportMntcInventoryAllView(GetMaintenanceInventoryInput input)
        {
            var queryFilter = PredicateHelper.True<MntcInventoryAll>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            queryFilter = queryFilter.And(p => p.ItemStatus == "READY TO USE" && p.UnitCode == "MTNC");

            if (input.InventoryDate.HasValue)
            {
                queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.InventoryDate) == DbFunctions.TruncateTime(input.InventoryDate));
            }

            //var dbResult = _mntcInventoryAllRepo.Get(queryFilter);

            var dbResult = (from data in _MstGenBrandPackageItemRepo.Get()
                            join item in _mntcInventoryAllRepo.Get(queryFilter) on data.ItemCode equals item.ItemCode
                            select new MntcInventoryAll
                            {
                                ItemCode = data.ItemCode,
                                EndingStock = item.EndingStock
                            }).ToList();
            var result = Mapper.Map<List<MaintenanceInventoryDTO>>(dbResult);
            return result;
        }

        //public List<MstGenBrandPackageItemDTO> GetReportMstGenBrandPackageItem(GetMaintenanceInventoryInput input)
        //{
        //    var dbMstGenBrandPackageItem = PredicateHelper.True<MstGenBrandPackageItem>();



        //    var result = from packageItem in dbMstGenBrandPackageItem
        //                 join inventoryAll in input on packageItem.

        //}

        public MaintenanceInventoryDTO GetMaintenanceInventory(GetMaintenanceInventoryInput input)
        {
            var queryFilter = PredicateHelper.True<MntcInventory>();

            if (input.InventoryDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.InventoryDate == input.InventoryDate);
            }

            if (!string.IsNullOrEmpty(input.ItemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == input.ItemCode);
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.ItemStatus))
            {
                queryFilter = queryFilter.And(p => p.ItemStatus == input.ItemStatus);
            }

            //Func<IQueryable<MntcInventory>, IOrderedQueryable<MntcInventory>> orderByFilter = null;
            //if (!string.IsNullOrEmpty(input.SortOrder) && !string.IsNullOrEmpty(input.SortExpression))
            //{
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcInventory>();
            //}

            var dbResult = _maintenanceInventoryRepo.Get(queryFilter, orderByFilter).FirstOrDefault();

            return Mapper.Map<MaintenanceInventoryDTO>(dbResult);

            //return _maintenanceInventoryRepository.Get(
            //        p => p.ItemCode == itemCode && p.LocationCode == locationCode && p.ItemStatus == "Bad Stock")
            //        .Select(p => new MaintenanceInventoryDTO
            //        {
            //            EndingStock = p.EndingStock
            //        })
            //        .FirstOrDefault();
        }

        /// <summary>
        /// Gets the inventory stock.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public InventoryStockDTO GetInventoryStock(GetInventoryStockInput input)
        {
            MntcInventoryAll dbInventory = new MntcInventoryAll();
            var inventoryDate = input.InventoryDate.Date;
            var dbResults = _mntcInventoryAllRepo.Get(m => m.InventoryDate == inventoryDate
                && m.ItemStatus == input.ItemStatus
                && m.ItemCode == input.ItemCode
                && m.LocationCode == input.LocationCode
                && m.UnitCode == input.UnitCode);

            try
            {//Makes sure this list only contains one element
                dbInventory = dbResults.SingleOrDefault();
            }//SingleOrDefault => If this exception is thrown, means there is a duplicate data !
            catch (InvalidOperationException)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DuplicateEntity);
            }

            //if (dbInventory == null)
            //    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            return Mapper.Map<InventoryStockDTO>(dbInventory);
        }
        //hakim
        public List<MaintenanceInventoryDTO> GetMntcInventoryAllViewDisposal(GetMaintenanceInventoryInput input)
        {
            var queryFilter = PredicateHelper.True<MntcInventoryAll>();

            if (!string.IsNullOrEmpty(input.ItemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == input.ItemCode);
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.ItemStatus))
            {
                queryFilter = queryFilter.And(p => p.ItemStatus == input.ItemStatus);
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            }

            if (input.InventoryDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.InventoryDate == input.InventoryDate);
            }

            var dbResult = _mntcInventoryAllRepo.Get(queryFilter);
            //var dbJoined = (from x in dbResult
            //                join y in _mstItemLocationRepo.Get().Where(y => y.ItemCode == input.ItemCode && y.LocationCode == input.LocationCode) on x.LocationCode equals y.LocationCode
            //                select new MntcInventoryAll
            //                {
            //                    ItemCode = x.ItemCode,
            //                    EndingStock = x.EndingStock
            //                }).OrderBy(x => x.ItemCode).ToList();

            return Mapper.Map<List<MaintenanceInventoryDTO>>(dbResult);

        }

        public List<GetDeltaViewFunction_Result> GetDeltaView(string LocationCode, DateTime dateTo)
        {
            return _sqlSPRepo.getDeltaView(LocationCode, dateTo).ToList();
        }

        public List<MaintenanceInventoryDTO> GetMaintenanceInventorys(GetMaintenanceInventoryInput input)
        {
            var queryFilter = PredicateHelper.True<MntcInventory>();

            if (input.InventoryDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.InventoryDate == input.InventoryDate);
            }

            if (!string.IsNullOrEmpty(input.ItemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == input.ItemCode);
            }

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(p => p.UnitCode == input.UnitCode);
            }

            if (!string.IsNullOrEmpty(input.ItemStatus))
            {
                queryFilter = queryFilter.And(p => p.ItemStatus == input.ItemStatus);
            }

            var dbResult = _maintenanceInventoryRepo.Get(queryFilter).Distinct();

            return Mapper.Map<List<MaintenanceInventoryDTO>>(dbResult);

        }

        public void SaveMntcInventory(MaintenanceInventoryDTO param)
        {
            var data = Mapper.Map<MntcInventory>(param);

            var dbResultGetUnits = _masterDataBll.GetAllUnits(new GetAllUnitsInput
            {
                LocationCode = data.LocationCode
            });

            foreach (var dbResultGetUnit in dbResultGetUnits)
            {
                data.UnitCode = dbResultGetUnit.UnitCode;
                _maintenanceInventoryRepo.Insert(data);
            }

            _uow.SaveChanges();
        }


        #endregion

        #region Equipment Transfer

        /// <summary>
        /// Gets the equipment transfers.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<EquipmentTransferCompositeDTO> GetEquipmentTransfers(GetEquipmentTransfersInput input)
        {
            var childLocationSource = _sqlSPRepo.GetLastChildLocationByLocationCode(input.SourceLocationCode).Select(x => x.LocationCode); ;
            var childLocationDest = _sqlSPRepo.GetLastChildLocationByLocationCode(input.DestinationLocationCode).Select(x => x.LocationCode); ;

            var queryFilter = PredicateHelper.True<MntcEquipmentMovement>();
            if (!string.IsNullOrEmpty(input.SourceLocationCode))
            {
                if (input.SourceLocationCode.Contains(Enums.LocationCode.REG.ToString()))
                {
                    queryFilter = queryFilter.And(m => m.LocationCodeSource == input.SourceLocationCode); 
                }
                else
                {
                    queryFilter = queryFilter.And(m => childLocationSource.Contains(m.LocationCodeSource)); 
                }
                
            }
                

            if (!string.IsNullOrEmpty(input.DestinationLocationCode))
            {
                if (input.DestinationLocationCode.Contains(Enums.LocationCode.REG.ToString()))
                {
                    queryFilter = queryFilter.And(m => m.LocationCodeDestination == input.DestinationLocationCode);
                }
                else
                {
                    queryFilter = queryFilter.And(m => childLocationDest.Contains(m.LocationCodeDestination));
                }
                
            }

            if (input.TransferDate.HasValue)
            {
                var transferDate = input.TransferDate.Value.Date;
                queryFilter = queryFilter.And(m => m.TransferDate == transferDate);
            }
            if (!string.IsNullOrEmpty(input.UnitCodeDestination))
            {
                queryFilter = queryFilter.And(m => m.UnitCodeDestination == input.UnitCodeDestination);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentMovement>();

            var dbResults = _equipmentMovementRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<EquipmentTransferCompositeDTO>>(dbResults);
        }

        /// <summary>
        /// Inserts the equipment transfer.
        /// </summary>
        /// <param name="equipmentTransfer">The equipment transfer.</param>
        /// <returns></returns>
        public EquipmentTransferDTO InsertEquipmentTransfer(EquipmentTransferDTO equipmentTransfer)
        {
            ValidateInsertEquipmentTransfer(equipmentTransfer);

            var dbEquipmentTransfer = Mapper.Map<MntcEquipmentMovement>(equipmentTransfer);

            //automatic receive when unit code destination MTNC
            if (equipmentTransfer.UnitCodeDestination != "MTNC")
            {
                dbEquipmentTransfer.QtyReceive = dbEquipmentTransfer.QtyTransfer;
                dbEquipmentTransfer.ReceiveDate = dbEquipmentTransfer.TransferDate;
            }

            dbEquipmentTransfer.CreatedDate = DateTime.Now;
            dbEquipmentTransfer.UpdatedDate = DateTime.Now;

            //update inventory endingstock
            //UpdateStockOnTransferEquipment(equipmentTransfer);

            _equipmentMovementRepo.Insert(dbEquipmentTransfer);
            _uow.SaveChanges();

            Mapper.Map(dbEquipmentTransfer, equipmentTransfer);
            return equipmentTransfer;
        }

        /// <summary>
        /// Validates the insert equipment transfer.
        /// </summary>
        /// <param name="equipmentTransfer">The equipment transfer.</param>
        /// <exception cref="BLLException"></exception>
        private void ValidateInsertEquipmentTransfer(EquipmentTransferDTO equipmentTransfer)
        {
            var dbEquipmentTransfer = _equipmentMovementRepo.GetByID(equipmentTransfer.TransferDate, equipmentTransfer.LocationCodeSource, equipmentTransfer.ItemCode, equipmentTransfer.UnitCodeDestination, equipmentTransfer.LocationCodeDestination);

            if (dbEquipmentTransfer != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);

            ValidateEndingStockForTransfer(equipmentTransfer);
        }

        /// <summary>
        /// Updates the equipment transfer.
        /// </summary>
        /// <param name="equipmentTransfer">The equipment transfer.</param>
        /// <returns></returns>
        /// <exception cref="BLLException"></exception>
        public EquipmentTransferDTO UpdateEquipmentTransfer(EquipmentTransferDTO equipmentTransfer)
        {
            var dbEquipmentTransfer = _equipmentMovementRepo.GetByID(equipmentTransfer.TransferDate, equipmentTransfer.LocationCodeSource, equipmentTransfer.ItemCode, equipmentTransfer.UnitCodeDestination, equipmentTransfer.LocationCodeDestination);

            if (dbEquipmentTransfer == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            ValidateEndingStockForTransfer(equipmentTransfer);

            //keep original CreatedBy and CreatedDate
            equipmentTransfer.CreatedBy = dbEquipmentTransfer.CreatedBy;
            equipmentTransfer.CreatedDate = dbEquipmentTransfer.CreatedDate;

            //set update time
            equipmentTransfer.UpdatedDate = DateTime.Now;

            //update inventory endingstock
            //UpdateStockOnTransferEquipment(equipmentTransfer);

            Mapper.Map(equipmentTransfer, dbEquipmentTransfer);

            //automatic receive when unit code destination MTNC
            if (equipmentTransfer.UnitCodeDestination != "MTNC")
            {
                dbEquipmentTransfer.QtyReceive = equipmentTransfer.QtyTransfer;
                dbEquipmentTransfer.ReceiveDate = equipmentTransfer.TransferDate;
            }

            _equipmentMovementRepo.Update(dbEquipmentTransfer);
            _uow.SaveChanges();

            return Mapper.Map<EquipmentTransferDTO>(dbEquipmentTransfer);
        }

        /// <summary>
        /// Validates the update equipment transfer.
        /// </summary>
        /// <param name="equipmentTransfer">The equipment transfer.</param>
        /// <exception cref="BLLException"></exception>
        private void ValidateEndingStockForTransfer(EquipmentTransferDTO equipmentTransfer)
        {
            var endingStock = GetAvailableEndingStockForTransfer(equipmentTransfer);

            if (endingStock < equipmentTransfer.QtyTransfer)
                throw new BLLException(ExceptionCodes.BLLExceptions.TransferQtyLargerThanAvailableStock);
        }


        /// <summary>
        /// Gets the available ending stock for transfer.
        /// </summary>
        /// <param name="equipmentTransfer">The equipment transfer.</param>
        /// <returns></returns>
        public int GetAvailableEndingStockForTransfer(EquipmentTransferDTO equipmentTransfer)
        {
            //get item status by location code for equipment transfer
            var itemStatus = GetItemStatusForTransferByLocationCode(equipmentTransfer.LocationCodeSource);

            var transferDate = equipmentTransfer.TransferDate.Date;

            var LocationCodeSource = equipmentTransfer.LocationCodeSource;
            var UnitCodeSource = "";

            if (LocationCodeSource.Contains("REG") == true)
            {
                UnitCodeSource = Enums.UnitCodeDefault.WHSE.ToString();
            }
            else
            {
                UnitCodeSource = Enums.UnitCodeDefault.MTNC.ToString();
            }
            //get inventories
            DateTime dt = Convert.ToDateTime(transferDate);
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var UserAD = strUserID.Username;
            var QParam = LocationCodeSource + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + UserAD;
            _sqlSPRepo.MntcInventoryAllProcedure(dt, LocationCodeSource, QParam, UserAD);
            var inventories = _sqlSPRepo.MntcInventoryAllFunction(dt, LocationCodeSource,itemStatus,equipmentTransfer.ItemCode,UnitCodeSource, UserAD);

            //var inventories = _mntcInventoryAllRepo.Get(m => DbFunctions.TruncateTime(m.InventoryDate) == transferDate
            //                                                && m.ItemStatus == itemStatus
            //                                                && m.LocationCode == equipmentTransfer.LocationCodeSource
            //                                                && m.ItemCode == equipmentTransfer.ItemCode
            //                                                && m.UnitCode == UnitCodeSource).ToList();

            //var inventories =
            //    _maintenanceInventoryRepo.Get(m => DbFunctions.TruncateTime(m.InventoryDate) == transferDate
            //                                       && m.ItemStatus == itemStatus &&
            //                                       m.LocationCode == equipmentTransfer.LocationCodeSource &&
            //                                       m.ItemCode == equipmentTransfer.ItemCode).ToList();

            var inventory = inventories.FirstOrDefault();
            if (inventory == null) return 0;
            var endingStock = inventory.EndingStock ?? 0;
            return endingStock;
        }

        /// <summary>
        /// Updates the stock on transfer equipment.
        /// </summary>
        /// <param name="equipmentTransfer">The equipment transfer.</param>
        private void UpdateStockOnTransferEquipment(EquipmentTransferDTO equipmentTransfer)
        {
            //get item status by location code for equipment transfer
            var itemStatus = GetItemStatusForTransferByLocationCode(equipmentTransfer.LocationCodeSource);

            var transferDate = equipmentTransfer.TransferDate.Date;

            //get inventories
            var inventories =
                _maintenanceInventoryRepo.Get(m => DbFunctions.TruncateTime(m.InventoryDate) >= transferDate
                                                   && m.ItemStatus == itemStatus &&
                                                   m.LocationCode == equipmentTransfer.LocationCodeSource &&
                                                   m.ItemCode == equipmentTransfer.ItemCode).ToList();

            var transferQty = GetTransferQtyDeviation(equipmentTransfer);

            foreach (var inventory in inventories)
            {
                inventory.EndingStock = inventory.EndingStock - transferQty;

                if (inventory.InventoryDate.Date == transferDate.Date)
                    inventory.StockOut = inventory.StockOut + transferQty;

                if (inventory.InventoryDate.Date > transferDate.Date)
                    inventory.BeginningStock = inventory.BeginningStock - transferQty;

                _maintenanceInventoryRepo.Update(inventory);
            }
        }

        private int GetTransferQtyDeviation(EquipmentTransferDTO equipmentTransfer)
        {
            var bdLatestValueTransferQty = 0;
            var newTransferQty = equipmentTransfer.QtyTransfer ?? 0;

            var dbEquipmentTransfer = _equipmentMovementRepo.GetByID(equipmentTransfer.TransferDate, equipmentTransfer.LocationCodeSource, equipmentTransfer.ItemCode, equipmentTransfer.UnitCodeDestination, equipmentTransfer.LocationCodeDestination);
            if (dbEquipmentTransfer != null && dbEquipmentTransfer.QtyTransfer.HasValue)
            {
                bdLatestValueTransferQty = dbEquipmentTransfer.QtyTransfer.Value;
            }
            return newTransferQty - bdLatestValueTransferQty;
        }
        /// <summary>
        /// Gets the item status for transfer by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public string GetItemStatusForTransferByLocationCode(string locationCode)
        {
            if (locationCode.StartsWith(Core.Enums.LocationCode.REG.ToString()))
            {
                return EnumHelper.GetDescription(Core.Enums.ItemStatus.InTransit);
            }

            return EnumHelper.GetDescription(Core.Enums.ItemStatus.ReadyToUse);
        }

        #endregion

        #region Equipment Fulfillment

        public MntcRequestToLocationDTO SaveMaintenanceRequestToLocation(MntcRequestToLocationDTO param)
        {
            var dbResult = _maintenanceRequestToLocation.GetByID(param.FulFillmentDate, param.RequestNumber,
                param.LocationCode);

            if (dbResult == null)
            {
                var model = Mapper.Map<MntcRequestToLocation>(param);
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;

                _maintenanceRequestToLocation.Insert(model);
            }
            else
            {
                Mapper.Map(param, dbResult);
                dbResult.UpdatedDate = DateTime.Now;

                _maintenanceRequestToLocation.Update(dbResult);
            }

            return param;
        }

        public MstMntcItemCompositeDTO GetItemDetail(string itemCode, string locationCode, string itemStatus = "")
        {
            var queryFilter = PredicateHelper.True<MaintenanceEquipmentFulfillmentItemDetailView>();

            if (!string.IsNullOrEmpty(itemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == itemCode);
            }

            if (!string.IsNullOrEmpty(locationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == locationCode);
            }

            if (!string.IsNullOrEmpty(itemStatus))
            {
                queryFilter = queryFilter.And(p => p.ItemStatus == itemStatus);
            }


            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "InventoryDate" }, "DESC");
            var orderByFilter = sortCriteria.GetOrderByFunc<MaintenanceEquipmentFulfillmentItemDetailView>();

            var dbResult =
                _maintenanceEquipmentFulfillmentItemDetailRepo.Get(queryFilter, orderByFilter).FirstOrDefault();

            return Mapper.Map<MstMntcItemCompositeDTO>(dbResult);
        }

        public List<MaintenanceInventoryDTO> GetEquipmentFulfillmentDetails(string itemCode, string itemStatus)
        {
            var result = _maintenanceEquipmentFulfillmentDetailViewRepository.Get(m => m.ItemCode == itemCode && m.ItemStatus == itemStatus);
            return Mapper.Map<List<MaintenanceInventoryDTO>>(result);
        }

        public List<MntcRequestToLocationDTO> GetMntcRequestToLocation(string requestNumber, DateTime? fulfillmentDate)
        {
            var result = _maintenanceRequestToLocation.Get(m => m.RequestNumber == requestNumber && m.FulFillmentDate == fulfillmentDate);
            return Mapper.Map<List<MntcRequestToLocationDTO>>(result);
        }

        public EquipmentFulfillmentCompositeDTO GetEquipmentFulfillment(GetEquipmentFulfillmentInput input)
        {

            var queryFilter = PredicateHelper.True<MaintenanceEquipmentFulfillmentView>();


            if (!string.IsNullOrEmpty(input.RequestNumber))
            {
                queryFilter = queryFilter.And(p => p.RequestNumber == input.RequestNumber);
            }

            if (!string.IsNullOrEmpty(input.ItemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == input.ItemCode);
            }

            if (input.FulfillmentDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.FulFillmentDate == input.FulfillmentDate);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MaintenanceEquipmentFulfillmentView>();

            var dbResult = _maintenanceEquipmentFulfillmentViewRepository.Get(queryFilter, orderByFilter).FirstOrDefault();

            return Mapper.Map<EquipmentFulfillmentCompositeDTO>(dbResult);
        }

        //public List<EquipmentFulfillmentCompositeDTO> GetEquipmentFulfillments(GetEquipmentFulfillmentInput input)
        public List<MntcFulfillmentViewDTO> GetEquipmentFulfillments(GetEquipmentFulfillmentInput input)
        {
            //call sp MntcInventoryAllProcedure to generate tempinventoryall data
            DateTime dt = Convert.ToDateTime(input.RequestDate);
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var UserAD = strUserID.Username;
            var QParam = input.RequestLocation + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/" + input.Requestor;
            //_sqlSPRepo.MntcInventoryAllProcedure(dt, input.RequestLocation, QParam, UserAD);
            if (input.Requestor != null)
            {
                _sqlSPRepo.MaintenanceExecutionInventoryProcedure(dt, input.RequestLocation, QParam, UserAD);
                _sqlSPRepo.MntcEquipmentStockProcedure(input.RequestLocation, "%", dt, QParam, UserAD);
            }

            //var queryFilter = PredicateHelper.True<MaintenanceEquipmentFulfillmentView>();
            var queryFilter = PredicateHelper.True<MntcFulfillmentView>();

            if (!string.IsNullOrEmpty(input.RequestLocation))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.RequestLocation);
            }

            if (input.RequestDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.RequestDate == input.RequestDate);
            }

            if (!string.IsNullOrEmpty(input.RequestNumber))
            {
                queryFilter = queryFilter.And(p => p.RequestNumber == input.RequestNumber);
            }

            if (!string.IsNullOrEmpty(input.Requestor))
            {
                queryFilter = queryFilter.And(p => p.CreatedBy == input.Requestor);
            }

            if (!string.IsNullOrEmpty(input.ItemCode))
            {
                queryFilter = queryFilter.And(p => p.ItemCode == input.ItemCode);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcFulfillmentView>();

            var dbResult = _mntcFulfillmentViewRepository.Get(queryFilter, orderByFilter);

            //return Mapper.Map<List<EquipmentFulfillmentCompositeDTO>>(dbResult);
            return Mapper.Map<List<MntcFulfillmentViewDTO>>(dbResult);
        }

        public EquipmentFulfillmentCompositeDTO SaveEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto)
        {
            // Check already exist data in Equipment Fulfillment Table
            var db = _maintenanceEquipmentFulfillmentRepository.GetByID(fulfillmentDto.FulFillmentDate, fulfillmentDto.ItemCode, fulfillmentDto.RequestNumber);
            if (db != null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
            }

            // insert maintenance equipment fulfillment
            InsertEquipmentFulfillment(fulfillmentDto);

            //


            return fulfillmentDto;
        }

        public void InsertEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto)
        {
            var dbEquipmentFulfillment = Mapper.Map<MntcEquipmentFulfillment>(fulfillmentDto);

            dbEquipmentFulfillment.CreatedDate = DateTime.Now;
            dbEquipmentFulfillment.UpdatedDate = DateTime.Now;
            dbEquipmentFulfillment.UpdatedBy = fulfillmentDto.UpdateBy;
            dbEquipmentFulfillment.MntcReqFullFillment =
                CreateCombineKeyEquipmentFulfillment(dbEquipmentFulfillment.LocationCode);

            _maintenanceEquipmentFulfillmentRepository.Insert(dbEquipmentFulfillment);
        }

        public string CreateCombineKeyEquipmentFulfillment(string locationCode)
        {
            var dbResultGenWeek = _masterDataBll.GetMstGenWeek(new GetMstGenWeekInput
            {
                CurrentDate = DateTime.Now
            });

            if (dbResultGenWeek == null)
            {
                return "";
            }

            return Core.Enums.CombineCode.FUL + "/" + locationCode + "/" + dbResultGenWeek.Year + "/" + dbResultGenWeek.Week + "/" + (int)DateTime.Now.DayOfWeek;
        }

        public bool IsExistEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto)
        {
            var mntcEquipmentFulfillment = _maintenanceEquipmentFulfillmentRepository.GetByID(fulfillmentDto.FulFillmentDate, fulfillmentDto.ItemCode, fulfillmentDto.RequestNumber);
            if (mntcEquipmentFulfillment == null)
                return false;
            else
                return true;
        }

        public bool IsExistFulfillmentByRequestDate(EquipmentFulfillmentCompositeDTO fulfillmentDto)
        {
            var fulfillment = _maintenanceEquipmentFulfillmentRepository.Get(c => c.RequestDate == fulfillmentDto.RequestDate && c.ItemCode == fulfillmentDto.ItemCode && c.LocationCode == fulfillmentDto.LocationCode && c.RequestNumber == fulfillmentDto.RequestNumber).FirstOrDefault();
            if (fulfillment == null)
                return false;
            else
                return true;
        }

        public void DeleteEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto)
        {
            //var mntcEquipmentFulfillment = _maintenanceEquipmentFulfillmentRepository.GetByID(fulfillmentDto.FulFillmentDate, fulfillmentDto.ItemCode, fulfillmentDto.RequestNumber);
            var fulfillment = _maintenanceEquipmentFulfillmentRepository.Get(c => c.RequestDate == fulfillmentDto.RequestDate && c.ItemCode == fulfillmentDto.ItemCode && c.LocationCode == fulfillmentDto.LocationCode && c.RequestNumber == fulfillmentDto.RequestNumber).FirstOrDefault();
            if (fulfillment == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //var dto = Mapper.Map<EquipmentFulfillmentCompositeDTO>(fulfillment);
            _maintenanceEquipmentFulfillmentRepository.Delete(fulfillment);
            _uow.SaveChanges();

            //return Mapper.Map<EquipmentFulfillmentCompositeDTO>(fulfillmentDto);
        }

        public EquipmentFulfillmentCompositeDTO UpdateEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto)
        {
            // Check already exist data in Equipment Fulfillment Table
            var mntcEquipmentFulfillment = _maintenanceEquipmentFulfillmentRepository.GetByID(fulfillmentDto.FulFillmentDate, fulfillmentDto.ItemCode, fulfillmentDto.RequestNumber);
            if (mntcEquipmentFulfillment == null)
            {
                //throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
                InsertEquipmentFulfillment(fulfillmentDto);
            }
            else
            {
                Mapper.Map(fulfillmentDto, mntcEquipmentFulfillment);
                _maintenanceEquipmentFulfillmentRepository.Update(mntcEquipmentFulfillment);
            }


            return fulfillmentDto;
        }

        #endregion

        #region Equipment Receive
        public List<EquipmentReceiveCompositeDTO> GetEquipmentReceives(GetEquipmentReceivesInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentMovement>();
            if (!string.IsNullOrEmpty(input.SourceLocationCode))
                queryFilter = queryFilter.And(m => m.LocationCodeSource == input.SourceLocationCode);

            if (!string.IsNullOrEmpty(input.DestinationLocationCode))
                queryFilter = queryFilter.And(m => m.LocationCodeDestination == input.DestinationLocationCode);

            if (input.TransferDate.HasValue)
            {
                var transferDate = input.TransferDate.Value.Date;
                queryFilter = queryFilter.And(m => m.TransferDate == transferDate);
            }

            if (input.ReceiveDate.HasValue)
            {
                var receiveDate = input.ReceiveDate.Value.Date;
                queryFilter = queryFilter.And(m => m.ReceiveDate == receiveDate);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentMovement>();

            var dbResults = _equipmentMovementRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<EquipmentReceiveCompositeDTO>>(dbResults);
        }

        public EquipmentReceiveDTO UpdateEquipmentReceive(EquipmentReceiveDTO equipmentReceive)
        {
            var dbEquipmentReceive = _equipmentMovementRepo.GetByID(equipmentReceive.TransferDate, equipmentReceive.LocationCodeSource, equipmentReceive.ItemCode, equipmentReceive.UnitCodeDestination, equipmentReceive.LocationCodeDestination);

            if (dbEquipmentReceive == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (equipmentReceive.QtyTransfer < equipmentReceive.QtyReceive)
                throw new BLLException(ExceptionCodes.BLLExceptions.QtyReceiveLargerThanQtyTransfer);

            if (equipmentReceive.QtyTransfer != equipmentReceive.QtyReceive)
                throw new BLLException(ExceptionCodes.BLLExceptions.QtyRecieveIsNotSameWithQtyTransfer);

            //keep original CreatedBy and CreatedDate
            equipmentReceive.CreatedBy = dbEquipmentReceive.CreatedBy;
            equipmentReceive.CreatedDate = dbEquipmentReceive.CreatedDate;

            //set update time
            equipmentReceive.UpdatedDate = DateTime.Now;

            Mapper.Map(equipmentReceive, dbEquipmentReceive);
            _equipmentMovementRepo.Update(dbEquipmentReceive);
            _uow.SaveChanges();

            return Mapper.Map<EquipmentReceiveDTO>(dbEquipmentReceive);
        }
        #endregion

        #region Maintenance Item Conversion

        public List<MaintenanceExecutionItemConversionDTO> GetMaintenanceEquipmentItemConvertsExcel(GetMaintenanceEquipmentItemConvertInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentItemConvert>();
            queryFilter = queryFilter.And(m => m.ConversionType == input.ConversionType);
            if (input.LocationCode != null)
            {
                var locationCodes = _masterDataBll.GetAllLocationByLocationCode(input.LocationCode, -1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locationCodes.Contains(m.LocationCode));
            }

            if (input.KpsWeek != null && input.KpsYear != null)
            {
                var week = _masterDataBll.GetWeekByYearAndWeek(input.KpsYear, input.KpsWeek);
                queryFilter = queryFilter.And(m => m.TransactionDate >= week.StartDate && m.TransactionDate <= week.EndDate);
            }

            if (input.TransactionDate.HasValue)
                queryFilter = queryFilter.And(m => m.TransactionDate == input.TransactionDate);

            Func<IQueryable<MntcEquipmentItemConvert>, IOrderedQueryable<MntcEquipmentItemConvert>> orderByFilter = null;
            if (!string.IsNullOrEmpty(input.SortExpression) && !string.IsNullOrEmpty(input.SortOrder))
            {
                var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
                orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentItemConvert>();
            }
            var dbResult = _mntcEquipmentItemConvertRepository.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MaintenanceExecutionItemConversionDTO>>(dbResult);
        }

        /// <summary>
        /// Gets the maintenance equipment item converts.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MaintenanceExecutionItemConversionDTO> GetMaintenanceEquipmentItemConverts(GetMaintenanceEquipmentItemConvertInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentItemConvert>();
            queryFilter = queryFilter.And(m => m.ConversionType == input.ConversionType);
            if (input.LocationCode != null)
            {
                var locationCodes = _masterDataBll.GetAllLocationByLocationCode(input.LocationCode, -1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locationCodes.Contains(m.LocationCode));
            }

            if (input.KpsWeek > 0 && input.KpsYear > 0)
            {
                var week = _masterDataBll.GetWeekByYearAndWeek(input.KpsYear, input.KpsWeek);
                queryFilter = queryFilter.And(m => m.TransactionDate >= week.StartDate && m.TransactionDate <= week.EndDate);
            }

            if (input.TransactionDate.HasValue)
                queryFilter = queryFilter.And(m => m.TransactionDate == input.TransactionDate);

            //Func<IQueryable<MntcEquipmentItemConvert>, IOrderedQueryable<MntcEquipmentItemConvert>> orderByFilter = null;
            //if (!string.IsNullOrEmpty(input.SortExpression) && !string.IsNullOrEmpty(input.SortOrder))
            //{
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentItemConvert>();
            //}

            var dbResult = _mntcEquipmentItemConvertRepository.Get(queryFilter, orderByFilter);
            if (input.ConversionType)
            {
                //dbResult = dbResult.OrderBy(m => m.ItemCodeSource);
                var MntcEquipmentItemConverts = new List<MntcEquipmentItemConvert>();
                var mntcEquipmentItemConvert = new MntcEquipmentItemConvert();
                foreach (var item in dbResult.Where(item => item.ItemCodeSource != mntcEquipmentItemConvert.ItemCodeSource))
                {
                    mntcEquipmentItemConvert = item;
                    MntcEquipmentItemConverts.Add(mntcEquipmentItemConvert);
                }
                return Mapper.Map<List<MaintenanceExecutionItemConversionDTO>>(MntcEquipmentItemConverts);

            }

            return Mapper.Map<List<MaintenanceExecutionItemConversionDTO>>(dbResult);
        }

        /// <summary>
        /// Gets the maintenance execution item conversion composites.
        /// </summary>
        /// <param name="itemCodeSource">The item code source.</param>
        /// <returns></returns>
        public List<MaintenanceExecutionItemConversionCompositeDTO> GetMaintenanceExecutionItemConversionComposites(string itemCodeSource, GetMaintenanceEquipmentItemConvertInput criteria)
        {
            var data = GetMaintenanceEquipmentItemConverts(criteria);
            if (data.Count == 0)
            {
                var qtyDefault = _maintenanceItemConversionDestinationViewRepository.Get(m => m.ItemCodeSource == itemCodeSource && m.QtyConvert.HasValue).Distinct().ToList();
                foreach (var item in qtyDefault)
                {
                    item.QtyGood = item.QtyConvert;
                }
                return Mapper.Map<List<MaintenanceExecutionItemConversionCompositeDTO>>(qtyDefault);
            }
            else
            {
                var result = _maintenanceItemConversionDestinationViewRepository.Get(m => m.ItemCodeSource == itemCodeSource && m.QtyConvert.HasValue && m.LocationCode == criteria.LocationCode).Distinct().ToList();
                return Mapper.Map<List<MaintenanceExecutionItemConversionCompositeDTO>>(result);
            }

        }

        public MaintenanceExecutionItemConversionDTO SaveMaintenanceExecutionItemConversionESDetail(MaintenanceExecutionItemConversionDTO executionItemConversionDto)
        {
            var db = _mntcEquipmentItemConvertRepository.GetByID(executionItemConversionDto.TransactionDate,
                executionItemConversionDto.LocationCode, executionItemConversionDto.ItemCodeSource, executionItemConversionDto.ItemCodeDestination);

            //Get Shift
            var location = _masterDataBll.GetMstLocationById(executionItemConversionDto.LocationCode);

            if (db == null)
            {
                db = Mapper.Map<MntcEquipmentItemConvert>(executionItemConversionDto);

                db.Shift = location.Shift;
                db.CreatedDate = DateTime.Now;
                db.UpdatedDate = DateTime.Now;
                _mntcEquipmentItemConvertRepository.Insert(db);
            }
            else
            {
                db.UpdatedDate = DateTime.Now;
                Mapper.Map(executionItemConversionDto, db);
                _mntcEquipmentItemConvertRepository.Update(db);
            }
            _uow.SaveChanges();
            return Mapper.Map<MaintenanceExecutionItemConversionDTO>(db);
        }

        public MaintenanceExecutionItemConversionDTO UpdateMaintenanceExecutionItemConversionES(MaintenanceExecutionItemConversionDTO executionItemConversionDto)
        {
            var dbMaintenanceItemConversions = _mntcEquipmentItemConvertRepository.Get(m => m.ConversionType && m.LocationCode == executionItemConversionDto.LocationCode && m.ItemCodeSource == executionItemConversionDto.ItemCodeSource);
            foreach (var mntcEquipmentItemConvert in dbMaintenanceItemConversions)
            {
                var mstMntcConvert = _masterDataBll.GetMasterMaintenanceBySourceAndDestination(mntcEquipmentItemConvert.ItemCodeSource, mntcEquipmentItemConvert.ItemCodeDestination);
                var destinationStock = mstMntcConvert.QtyConvert * executionItemConversionDto.SourceStock * mntcEquipmentItemConvert.QtyGood;

                mntcEquipmentItemConvert.SourceQty = executionItemConversionDto.SourceStock;
                //mntcEquipmentItemConvert.DestinationStock = destinationStock != null ? (int)destinationStock : 0;
                mntcEquipmentItemConvert.UpdatedDate = DateTime.Now;
                _mntcEquipmentItemConvertRepository.Update(mntcEquipmentItemConvert);
            }
            _uow.SaveChanges();
            return executionItemConversionDto;
        }

        /// <summary>
        /// Inserts the equipment item convert.
        /// </summary>
        /// <param name="executionItemConversionDto">The equipment item convert dto.</param>
        /// <returns></returns>
        public MaintenanceExecutionItemConversionDTO InsertMaintenanceExecutionItemConversion(MaintenanceExecutionItemConversionDTO executionItemConversionDto)
        {
            //Get Shift
            var location = _masterDataBll.GetMstLocationById(executionItemConversionDto.LocationCode);
            executionItemConversionDto.Shift = location.Shift;

            ValidateInsertMaintenanceExecutionItemConversion(executionItemConversionDto);

            var dbMntcEquipmentItemConvert = Mapper.Map<MntcEquipmentItemConvert>(executionItemConversionDto);

            dbMntcEquipmentItemConvert.CreatedDate = DateTime.Now;
            dbMntcEquipmentItemConvert.UpdatedDate = DateTime.Now;

            _mntcEquipmentItemConvertRepository.Insert(dbMntcEquipmentItemConvert);
            _uow.SaveChanges();

            return Mapper.Map(dbMntcEquipmentItemConvert, executionItemConversionDto);
        }


        /// <summary>
        /// Validates the insert maintenance execution item conversion.
        /// </summary>
        /// <param name="mntcItemConDto">The MNTC item con dto.</param>
        /// <exception cref="BLLException"></exception>
        private void ValidateInsertMaintenanceExecutionItemConversion(MaintenanceExecutionItemConversionDTO mntcItemConDto)
        {
            var dbMntcExItemCon = _mntcEquipmentItemConvertRepository.GetByID(mntcItemConDto.TransactionDate, mntcItemConDto.LocationCode, mntcItemConDto.ItemCodeSource, mntcItemConDto.ItemCodeDestination);
            if (dbMntcExItemCon != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }
        /// <summary>
        /// Updates the equipment item convert.
        /// </summary>
        /// <param name="executionItemConversionDto">The equipment item convert dto.</param>
        /// <returns></returns>
        /// <exception cref="BLLException"></exception>
        public MaintenanceExecutionItemConversionDTO UpdateMaintenanceExecutionItemConversion(MaintenanceExecutionItemConversionDTO executionItemConversionDto)
        {
            var dbMntcEquipmentItemConvert = _mntcEquipmentItemConvertRepository.GetByID(executionItemConversionDto.TransactionDate,
                                                                                         executionItemConversionDto.LocationCode,
                                                                                         executionItemConversionDto.ItemCodeSource,
                                                                                         executionItemConversionDto.ItemCodeDestination);
            if (dbMntcEquipmentItemConvert == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            executionItemConversionDto.CreatedBy = dbMntcEquipmentItemConvert.CreatedBy;
            executionItemConversionDto.CreatedDate = dbMntcEquipmentItemConvert.CreatedDate;

            executionItemConversionDto.UpdatedDate = DateTime.Now;

            Mapper.Map(executionItemConversionDto, dbMntcEquipmentItemConvert);
            _mntcEquipmentItemConvertRepository.Update(dbMntcEquipmentItemConvert);
            _uow.SaveChanges();

            return Mapper.Map<MaintenanceExecutionItemConversionDTO>(dbMntcEquipmentItemConvert);
        }
        #endregion

        #region MaintenanceExecutionQualityInspection

        public List<MaintenanceExecutionQualityInspectionDTO> GetMaintenanceExecutionQualityInspections(GetMaintenanceExecutionQualityInspectionInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentQualityInspection>();

            if (!string.IsNullOrEmpty(input.Location))
                queryFilter = queryFilter.And(m => m.LocationCode == input.Location);

            if (input.TransactionDate.HasValue)
                queryFilter = queryFilter.And(m => m.TransactionDate == input.TransactionDate);

            Func<IQueryable<MntcEquipmentQualityInspection>, IOrderedQueryable<MntcEquipmentQualityInspection>> orderByFilter = null;
            if (!string.IsNullOrEmpty(input.SortExpression) && !string.IsNullOrEmpty(input.SortOrder))
            {
                var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
                orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentQualityInspection>();
            }
            var dbResult = _maintenanceEquipmentQualityInspectionRepository.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<MaintenanceExecutionQualityInspectionDTO>>(dbResult);
        }

        public MaintenanceExecutionQualityInspectionDTO InsertMaintenanceExecutionQualityInspection(MaintenanceExecutionQualityInspectionDTO inspectionDto)
        {
            ValidationInsertMaintenanceEquipmentQualityInspection(inspectionDto);
            var dbInspection = Mapper.Map<MntcEquipmentQualityInspection>(inspectionDto);

            dbInspection.CreatedDate = DateTime.Now;
            dbInspection.UpdatedDate = DateTime.Now;
            
            //checking qty approve
            CheckingTransitAndRecieving(dbInspection);

            _maintenanceEquipmentQualityInspectionRepository.Insert(dbInspection);

            //update request
            UpdateQtyLeftOverRequest(inspectionDto.LocationCode, inspectionDto.ItemCode, inspectionDto.RequestNumber,
                inspectionDto.QtyLeftOver);

            _uow.SaveChanges();

            Mapper.Map(dbInspection, inspectionDto);
            return inspectionDto;
        }

        private void CheckingTransitAndRecieving(MntcEquipmentQualityInspection input)
        {
            //var data = _maintenanceEquipmentFulfillmentRepository.GetByID(input.TransactionDate, input.ItemCode,
            //    input.RequestNumber);
            var data = _maintenanceEquipmentFulfillmentRepository.Get().Where(x => x.RequestNumber == input.RequestNumber && x.ItemCode == input.ItemCode).FirstOrDefault();
            var sum = (input.QTYTransit == null? 0:input.QTYTransit) + input.QtyReceiving;
            if (sum > data.PurchaseQty)
                throw new BLLException(ExceptionCodes.BLLExceptions.QtyTransitPlusReceiving, "<strong> Quantity Transit + Quantity Recieving large than Total Approved Quantity </strong>");

        }

        private void UpdateQtyLeftOverRequest(string locationCode, string itemCode, string requestNumber, decimal? qtyLeftOver)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentRequest>();
            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(itemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == itemCode);
            if (!string.IsNullOrEmpty(requestNumber))
                queryFilter = queryFilter.And(m => m.RequestNumber == requestNumber);

            var dbResult = _equipmentRequestRepo.Get(queryFilter).FirstOrDefault();

            //onbly update when the data is not null
            if (dbResult != null)
            {
                dbResult.QtyLeftOver = qtyLeftOver;
                _equipmentRequestRepo.Update(dbResult);
            }


        }

        public void ValidationInsertMaintenanceEquipmentQualityInspection(MaintenanceExecutionQualityInspectionDTO inspectionDto)
        {
            var dbInspection = _maintenanceEquipmentQualityInspectionRepository.GetByID(inspectionDto.TransactionDate, inspectionDto.ItemCode, inspectionDto.LocationCode, inspectionDto.PurchaseNumber);
            if (dbInspection != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
        }

        public MaintenanceExecutionQualityInspectionDTO UpdateMaintenanceExecutionQualityInspection(MaintenanceExecutionQualityInspectionDTO inspectionDto)
        {
            var dbInspection = _maintenanceEquipmentQualityInspectionRepository.GetByID(inspectionDto.TransactionDate, inspectionDto.ItemCode, inspectionDto.LocationCode, inspectionDto.PurchaseNumber);
            if (dbInspection == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            Mapper.Map(inspectionDto, dbInspection);
            dbInspection.UpdatedDate = DateTime.Now;

            //check qty approve
            CheckingTransitAndRecieving(dbInspection);

            _maintenanceEquipmentQualityInspectionRepository.Update(dbInspection);

            //update request
            var qtyLefOver = inspectionDto.QtyLeftOverCount - inspectionDto.QtyReceiving;
            UpdateQtyLeftOverRequest(inspectionDto.LocationCode, inspectionDto.ItemCode, inspectionDto.RequestNumber, qtyLefOver);

            _uow.SaveChanges();

            return Mapper.Map<MaintenanceExecutionQualityInspectionDTO>(dbInspection);
        }

        public EquipmentFulfillmentCompositeDTO GetPurchaseNumber(GetEquipmentFulfillmentInput input)
        {
            //var fulFillmentDate = _maintenanceEquipmentFulfillmentRepository.Get().Where(x => x.LocationCode == input.RequestLocation);
            var quertyFilter = PredicateHelper.True<MntcEquipmentFulfillment>();
            if (!string.IsNullOrEmpty(input.RequestLocation))
                quertyFilter = quertyFilter.And(c => c.LocationCode == input.RequestLocation);

            if (!string.IsNullOrEmpty(input.ItemCode))
                quertyFilter = quertyFilter.And(c => c.ItemCode == input.ItemCode);

            if (!string.IsNullOrEmpty(input.RequestNumber))
                quertyFilter = quertyFilter.And(c => c.RequestNumber == input.RequestNumber);

            //var date = input.FulfillmentDate.HasValue ? input.FulfillmentDate.Value : DateTime.Now;
            //if (date != null)
            //{
            //    quertyFilter = quertyFilter.And(c => c.FulFillmentDate == input.FulfillmentDate);
            //}

            var dbResult = _maintenanceEquipmentFulfillmentRepository.Get(quertyFilter).FirstOrDefault();
            if (dbResult == null)
            {
                return new EquipmentFulfillmentCompositeDTO();
            }
            var hasilMapper2 = new EquipmentFulfillmentCompositeDTO
            {
                FulFillmentDate = dbResult.FulFillmentDate,
                RequestDate = dbResult.RequestDate,
                ItemCode = dbResult.ItemCode,
                LocationCode = dbResult.LocationCode,
                RequestNumber = dbResult.RequestNumber,
                RequestToQty = dbResult.RequestToQty,
                PurchaseQuantity = dbResult.PurchaseQty,
                PurchaseNumber = dbResult.PurchaseNumber,
                CreatedBy = dbResult.CreatedBy,
                UpdateBy = dbResult.UpdatedBy

            };
            //var data = Mapper.Map<EquipmentFulfillmentCompositeDTO>(dbResult);

            return hasilMapper2;

        }

        #endregion

        #region "Equipment Stock Report"
        public List<MaintenanceEquipmentStockReportDTO> GetMaintenanceEquipmentStockReport(GetMaintenanceEquipmentStockReportInput input)
        {
            //var queryFilter = PredicateHelper.True<MaintenanceEquipmentStockView>();

            //if (!string.IsNullOrEmpty(input.LocationCode))
            //    queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            //if (!string.IsNullOrEmpty(input.UnitCode))
            //    queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            //if (input.InventoryDate != null)
            //    queryFilter = queryFilter.And(m => m.InventoryDate == input.InventoryDate);

            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var orderByFilter = sortCriteria.GetOrderByFunc<MaintenanceEquipmentStockView>();

            //var dbResult = _maintenanceEquipmentStockViewRepo.Get(queryFilter, orderByFilter);

            //return Mapper.Map<List<MaintenanceEquipmentStockReportDTO>>(dbResult);

            //return Mapper.Map<List<MaintenanceEquipmentStockReportDTO>>(dbResult);

            //var dbResult = _sqlSPRepo.GetMaintenanceEquipmentStockView(input.LocationCode, input.UnitCode, input.InventoryDate.Value);
            var dbResult = _sqlSPRepo.GetMaintenanceEquipmentStockFunction(input.LocationCode, input.UnitCode, input.InventoryDate.Value);

            return Mapper.Map<List<MaintenanceEquipmentStockReportDTO>>(dbResult);
        }

        public List<MntcEquipmentStockFunction_Result> GetReportMaintenanceEquipmentStock(DateTime date, string locationCode, string unitCode, string QParam, string UserAD)
        {
            

            DateTime dt = Convert.ToDateTime(date);
            //return _sqlSPRepo.getin(tPOFeeCode, iDRole, page, button).SingleOrDefault();
            _sqlSPRepo.MaintenanceExecutionInventoryProcedure(dt, locationCode, QParam, UserAD);
            _sqlSPRepo.MntcEquipmentStockProcedure(locationCode, unitCode, dt, QParam, UserAD);
            var dbResult = _sqlSPRepo.MntcEquipmentStockFunction(dt, locationCode, unitCode, UserAD);
            return Mapper.Map<List<MntcEquipmentStockFunction_Result>>(dbResult);
            
            //var dbResult = _sqlSPRepo.GetMaintenanceEquipmentStockFunction(input.LocationCode, input.UnitCode, input.InventoryDate.Value);
            //return Mapper.Map<List<MaintenanceEquipmentStockReportDTO>>(dbResult);
        }

        #endregion

        #region Equipment Requirement Report
        /// <summary>
        /// Get Equipment Requirement Report By User Entry Package
        /// </summary>
        /// <param name="locationCode">Location Code</param>
        /// <param name="brandGroupCode">Brand Group Code</param>
        /// <param name="userPackage">Eser Entry Package</param>
        /// <returns></returns>
        public List<MaintenanceEquipmentRequirementReportDTO> GetMaintenanceEquipmentRequirementReport(string locationCode, string brandGroupCode, float? userPackage, DateTime date)
        {
            return Mapper.Map<List<MaintenanceEquipmentRequirementReportDTO>>(_sqlSPRepo.GetMaintenanceEquipmentRequirementReport(locationCode, brandGroupCode, userPackage, date));
        }
        public List<MaintenanceEquipmentRequirementReportDTO> GetMaintenanceEquipmentRequirementReport2(string locationCode, string brandGroupCodeFrom, string brandGroupCodeTo, DateTime date)
        {
            return Mapper.Map<List<MaintenanceEquipmentRequirementReportDTO>>(_sqlSPRepo.GetMaintenanceEquipmentRequirementReport2(locationCode, brandGroupCodeFrom, brandGroupCodeTo, date));
        }
        public List<EquipmentRequirementDTO> GetEquipmentRequirementSummaryLocations(GetEquipmentRequirementSummaryInput input)
        {
            var queryFilter = PredicateHelper.True<EquipmentRequirementView>();
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<EquipmentRequirementView>();
            var dbResults = _equipmentRequirementRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<EquipmentRequirementDTO>>(dbResults);
        }
        public List<EquipmentRequirementSummaryItemDTO> GetEquipmentSummaryItem(string locationCode, string brandGroupCode, DateTime date)
        {
            return Mapper.Map<List<EquipmentRequirementSummaryItemDTO>>(_sqlSPRepo.GetEquipmentRequirementSummary(locationCode, brandGroupCode, date));
        }

        public int? GetRealStock(string locationCode, string itemCode, DateTime date)
        {
            return _sqlSPRepo.GetRealStock(locationCode, itemCode, date).SingleOrDefault();
        }

        #endregion

        #region Repair Item Usage

        public List<SparepartDTO> GetAllRepairItemUsages(GetRepairItemUsageInput input)
        {
            var queryFilter = PredicateHelper.True<MntcRepairItemUsage>();

            if (input.TransactionDate.HasValue)
                queryFilter = queryFilter.And(m => m.TransactionDate == input.TransactionDate);

            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.ItemCodeSource))
                queryFilter = queryFilter.And(m => m.ItemCodeSource == input.ItemCodeSource);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcRepairItemUsage>();

            var dbResult = _maintenanceRepairItemUsageRepository.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<SparepartDTO>>(dbResult);
        }
        #endregion

        #region Maintenance Execution Inventory
        public List<MaintenanceExecutionInventoryViewDTO> GetMaintenanceExecutionInventoryView(MaintenanceExecutionInventoryViewInput input)
        {
            var queryFilter = PredicateHelper.True<MaintenanceExecutionInventoryView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.ItemType))
                queryFilter = queryFilter.And(m => m.ItemType == input.ItemType);
            if (input.Date.HasValue)
                queryFilter = queryFilter.And(m => m.InventoryDate == input.Date);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MaintenanceExecutionInventoryView>();

            var dbResult = _maintenanceExecutionInventoryViewRepository.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MaintenanceExecutionInventoryViewDTO>>(dbResult);
        }

        public List<MaintenanceExecutionInventoryViewDTO> GetInventory(string date, string locationCode, string itemType)
        {
            //return _sqlSPRepo.getin(tPOFeeCode, iDRole, page, button).SingleOrDefault();
            var dbResult = _sqlSPRepo.GetInventoryView(date, locationCode, itemType);
            return Mapper.Map<List<MaintenanceExecutionInventoryViewDTO>>(dbResult);
        }

        public List<MaintenanceExecutionInventoryFunction_Result> GetInventoryView(DateTime date, string locationCode, string itemType, string QParam, string UserAD)
        {
            DateTime dt = Convert.ToDateTime(date);
            //return _sqlSPRepo.getin(tPOFeeCode, iDRole, page, button).SingleOrDefault();
            _sqlSPRepo.MaintenanceExecutionInventoryProcedure(dt, locationCode, QParam, UserAD);
            var dbResult = _sqlSPRepo.MaintenanceExecutionInventoryFunction(dt, locationCode, itemType, UserAD);
            return Mapper.Map<List<MaintenanceExecutionInventoryFunction_Result>>(dbResult);
        }

        public List<MaintenanceExecutionInventoryViewDTO> GetInventory(MaintenanceExecutionInventoryViewInput input)
        {
            var queryFilter = PredicateHelper.True<MaintenanceExecutionInventoryView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.ItemType))
                queryFilter = queryFilter.And(m => m.ItemType == input.ItemType);
            if (input.Date.HasValue)
                queryFilter = queryFilter.And(m => m.InventoryDate == input.Date);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MaintenanceExecutionInventoryView>();

            var dbResult = _maintenanceExecutionInventoryViewRepository.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MaintenanceExecutionInventoryViewDTO>>(dbResult);
        }

        public MaintenanceExecutionInventoryViewDTO GetMaintenanceExecutionInventoryView(string locationCode, string itemCode, DateTime? inventoryDate)
        {
            var childLocation = _sqlSPRepo.GetLastChildLocationByLocationCode(locationCode).Select(x => x.LocationCode);

            var queryFilter = PredicateHelper.True<MaintenanceExecutionInventoryView>();

            if (!string.IsNullOrEmpty(locationCode))
            {
                if (locationCode.Contains(Enums.LocationCode.REG.ToString()))
                {
                    queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
                }
                else
                {
                    queryFilter = queryFilter.And(m => childLocation.Contains(m.LocationCode));
                }
                
            }
                
            if (!string.IsNullOrEmpty(itemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == itemCode);
            if (inventoryDate.HasValue)
                queryFilter = queryFilter.And(m => m.InventoryDate == inventoryDate);

            var dbResult = _maintenanceExecutionInventoryViewRepository.Get(queryFilter).FirstOrDefault();

            return Mapper.Map<MaintenanceExecutionInventoryViewDTO>(dbResult);
        }

        public MaintenanceExecutionInventoryViewDTO GetMaintenanceExecutionInventoryTransferView(string locationCode, string itemCode, DateTime? inventoryDate)
        {
            var childLocation = _sqlSPRepo.GetLastChildLocationByLocationCode(locationCode).Select(x => x.LocationCode);

            var queryFilter = PredicateHelper.True<MaintenanceExecutionInventoryView>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(itemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == itemCode);
            if (inventoryDate.HasValue)
                queryFilter = queryFilter.And(m => m.InventoryDate == inventoryDate);

            var dbResult = _maintenanceExecutionInventoryViewRepository.Get(queryFilter).FirstOrDefault();

            return Mapper.Map<MaintenanceExecutionInventoryViewDTO>(dbResult);
        }

        public MntcEquipmentQualityInspection GetMntcEquipmentQualityInspecton(string locationCode, string itemCode, DateTime? transactionDate, string requestNumber)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentQualityInspection>();
            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(itemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == itemCode);

            DateTime dateYesterday = transactionDate.HasValue ? transactionDate.Value : DateTime.Now;
            dateYesterday = dateYesterday.Date.AddDays(-1);
            
            if (transactionDate.HasValue)
                queryFilter = queryFilter.And(m => m.TransactionDate == dateYesterday);
            if (!string.IsNullOrEmpty(requestNumber))
                queryFilter = queryFilter.And(m => m.RequestNumber == requestNumber);

            var dbResult = _maintenanceEquipmentQualityInspectionRepository.Get(queryFilter).OrderByDescending(m => m.CreatedDate).FirstOrDefault();

            return Mapper.Map<MntcEquipmentQualityInspection>(dbResult);
        }
        public MntcEquipmentRequestCompositeDTO GetEquipmentRequestQtyLeftOver(string locationCode, string itemCode, string requestNumber)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentRequest>();
            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (!string.IsNullOrEmpty(itemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == itemCode);
            if (!string.IsNullOrEmpty(requestNumber))
                queryFilter = queryFilter.And(m => m.RequestNumber == requestNumber);
            if (!string.IsNullOrEmpty(requestNumber))
                queryFilter = queryFilter.And(m => m.RequestNumber == requestNumber);

            var dbResult = _equipmentRequestRepo.Get(queryFilter).OrderByDescending(m => m.RequestDate).FirstOrDefault();

            return Mapper.Map<MntcEquipmentRequestCompositeDTO>(dbResult);
        }

        #endregion

        #region Maintenance Execution Inventory Adjustment
        public List<MaintenanceExecutionInventoryAdjustmentDTO> GetMaintenanceExecutionInventoryAdjustment(GetMaintenanceExecutionInventoryAdjustmentInput input)
        {
            var queryFilter = PredicateHelper.True<MntcInventoryAdjustment>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);

            if (input.CreatedDate != null)
                queryFilter = queryFilter.And(m => DbFunctions.TruncateTime(m.CreatedDate) == input.CreatedDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcInventoryAdjustment>();

            var dbResult = _maintenanceInventoryAdjustmentRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MaintenanceExecutionInventoryAdjustmentDTO>>(dbResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryAdjustmentDTO"></param>
        /// <returns></returns>
        public MaintenanceExecutionInventoryAdjustmentDTO InsertInventoryAdjustment(MaintenanceExecutionInventoryAdjustmentDTO inventoryAdjustmentDTO)
        {
            //ValidateInsertTPOPackage(inventoryAdjustmentDTO);

            var inventoryAdjustment = Mapper.Map<MntcInventoryAdjustment>(inventoryAdjustmentDTO);
            inventoryAdjustment.CreatedDate = DateTime.Now;
            inventoryAdjustment.UpdatedDate = DateTime.Now;

            _maintenanceInventoryAdjustmentRepo.Insert(inventoryAdjustment);
            _uow.SaveChanges();

            Mapper.Map(inventoryAdjustment, inventoryAdjustmentDTO);
            return inventoryAdjustmentDTO;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryAdjustmentDTO"></param>
        /// <returns></returns>
        public MaintenanceExecutionInventoryAdjustmentDTO UpdateInventoryAdjustment(MaintenanceExecutionInventoryAdjustmentDTO inventoryAdjustmentDTO)
        {
            //var inventoryAdjustment = _maintenanceInventoryAdjustmentRepo.GetByID(inventoryAdjustmentDTO.AdjustmentDate, inventoryAdjustmentDTO.LocationCode, inventoryAdjustmentDTO.UnitCode, inventoryAdjustmentDTO.UnitCodeDestination, inventoryAdjustmentDTO.ItemCode);
            var inventoryAdjustment = Mapper.Map<MntcInventoryAdjustment>(inventoryAdjustmentDTO);

            inventoryAdjustment.CreatedDate = DateTime.Now;
            inventoryAdjustment.UpdatedDate = DateTime.Now;

            if (inventoryAdjustment == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            inventoryAdjustmentDTO.CreatedBy = inventoryAdjustment.CreatedBy;
            inventoryAdjustmentDTO.CreatedDate = inventoryAdjustment.CreatedDate;

            //set update time
            inventoryAdjustmentDTO.UpdatedDate = DateTime.Now;

            Mapper.Map(inventoryAdjustmentDTO, inventoryAdjustment);
            _maintenanceInventoryAdjustmentRepo.Update(inventoryAdjustment);
            _uow.SaveChanges();

            //return Mapper.Map<MaintenanceExecutionInventoryAdjustmentDTO>(inventoryAdjustment);
            Mapper.Map(inventoryAdjustment, inventoryAdjustmentDTO);
            return inventoryAdjustmentDTO;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryAdjustmentDTO"></param>
        /// <returns></returns>
        public MaintenanceExecutionInventoryAdjustmentDTO DeleteInventoryAdjustment(MaintenanceExecutionInventoryAdjustmentDTO inventoryAdjustmentDTO)
        {
            var inventoryAdjustment = _maintenanceInventoryAdjustmentRepo.GetByID(inventoryAdjustmentDTO.AdjustmentDate, inventoryAdjustmentDTO.LocationCode, inventoryAdjustmentDTO.UnitCode, inventoryAdjustmentDTO.UnitCodeDestination, inventoryAdjustmentDTO.ItemCode, inventoryAdjustmentDTO.ItemStatusFrom, inventoryAdjustmentDTO.ItemStatusTo);

            if (inventoryAdjustment == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            _maintenanceInventoryAdjustmentRepo.Delete(inventoryAdjustment);
            _uow.SaveChanges();

            return Mapper.Map<MaintenanceExecutionInventoryAdjustmentDTO>(inventoryAdjustment);
        }
        #endregion

        #region e-mail
        public void SendEmailSaveEquipmentTransfer(string SourceLocationCode, DateTime TransferDate, string DestinationLocationCode, string DestinationUnitCode, string currUserName)
        {
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = SourceLocationCode,
                Date = TransferDate,
                DestinationLocationCode = DestinationLocationCode,
                UnitCode = DestinationUnitCode,
                FunctionName = Enums.PageNameEmail.EquipmentTransfer.ToString(),
                ButtonName = Enums.ButtonName.Save.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.EquipmentTransfer),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageNameEmail.EquipmentReceive)
            };
            
            var listUserEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput).Select(x => new
            {
                UserAd = x.UserAD,
                Name = x.Name,
                Email = x.Email,
                IDResponsibility = x.IDResponsibility,
                Location = x.Location
            }).ToList().Distinct();


            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            var listEmail = new List<HMS.SKTIS.BusinessObjects.Inputs.Planning.MailInput>();
            foreach (var item in listUserEmailDestination)
            {
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                var email = new HMS.SKTIS.BusinessObjects.Inputs.Planning.MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailEquipmentTransfer(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        public void SendEmailSaveEquipmentRequest(string locationCode, DateTime requestDate, string requestNumber, string currUserName)
        {
            //get first data equipment request
            var eqReq = _equipmentRequestRepo.Get(c => c.LocationCode == locationCode && c.RequestDate == requestDate && c.RequestNumber == requestNumber).FirstOrDefault();
            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = locationCode,
                RequestDate = requestDate,
                RequestNumber = requestNumber,
                //Date = tpoTPK == null ? _masterDataBll.GetFirstDateByYearWeek(kpsYear, kpsWeek).Date : tpoTPK.TPKTPOStartProductionDate.Date,
                FunctionName = Enums.PageNameEmail.EquipmentRequest.ToString(),
                ButtonName = Enums.ButtonName.Save.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.EquipmentRequest),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageNameEmail.EquipmentFullfillment),
            };

            // Get User, Email, Responsibility Destination Recipient
            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            // Get User and Email Current/Sender
            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            // Create Email Input
            var listEmail = new List<HMS.SKTIS.BusinessObjects.Inputs.Planning.MailInput>();
            foreach (var item in listUserAndEmailDestination)
            {
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                var email = new HMS.SKTIS.BusinessObjects.Inputs.Planning.MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailEquipmentRequest(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        private string CreateBodyMailEquipmentTransfer(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Terdapat Maintenance Transfer baru, Silakan melanjutkan proses berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<p><a href= webrooturl/MaintenanceEquipmentReceive/Index/" + emailInput.LocationCode + "/"
                                                                   + emailInput.DestinationLocationCode + "/"
                                                                   + emailInput.Date.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString() + ">"
                                                                   + emailInput.FunctionNameDestination + "</a></p>"
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.IsBodyHtml = true;
            message.Body = bodyMail.ToString();

            return bodyMail.ToString();
        }

        private string CreateBodyMailEquipmentRequest(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Equipment Fulfillment sudah tersedia, Silakan melanjutkan proses berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<p><a href= webrooturl/EquipmentFulfillment/Index/" + emailInput.LocationCode + "/"
                                                                   + emailInput.RequestDate.ToString("yyyy-MM-dd") + "/"
                                                                   + emailInput.IDResponsibility.ToString() + ">"
                                                                   + emailInput.FunctionNameDestination + "</a></p>"
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.IsBodyHtml = true;
            message.Body = bodyMail.ToString();

            return bodyMail.ToString();
        }

        public void SendEmailSaveEquipmentFulfillment(string locationCode, DateTime Date,string requestNumber, string currUserName)
        {
            //get first data equipment fulfillment
            var EquipmentFulfillment = _maintenanceEquipmentFulfillmentRepository.Get(c => c.LocationCode == locationCode && c.RequestDate == Date && c.RequestNumber == requestNumber).FirstOrDefault();
            // Initial Input To Get Recipient User, Email, Responsibility
            var emailInput = new GetUserAndEmailInput
            {
                LocationCode = locationCode,
                RequestDate = Date,
                RequestNumber = requestNumber,
                FunctionName = Enums.PageNameEmail.EquipmentFullfillment.ToString(),
                ButtonName = Enums.ButtonName.Save.ToString().ToUpper(),
                EmailSubject = EnumHelper.GetDescription(Enums.EmailSubject.EquipmentFulfillment),
                FunctionNameDestination = EnumHelper.GetDescription(Enums.PageNameEmail.QualityInspection),
            };

            // Get User, Email, Responsibility Destination Recipient
            var listUserAndEmailDestination = _sqlSPRepo.GetUserAndEmail(emailInput);

            // Get User and Email Current/Sender
            var username = currUserName.Substring(4);
            var currUserEmail = _mstAdTemp.Get(c => c.UserAD.Contains(username)).FirstOrDefault();

            // Create Email Input
            var listEmail = new List<HMS.SKTIS.BusinessObjects.Inputs.Planning.MailInput>();
            foreach (var item in listUserAndEmailDestination)
            {
                emailInput.Recipient = item.Name;
                emailInput.IDResponsibility = item.IDResponsibility ?? 0;
                var email = new HMS.SKTIS.BusinessObjects.Inputs.Planning.MailInput
                {
                    FromName = currUserEmail == null ? "" : currUserEmail.Name,
                    FromEmailAddress = currUserEmail == null ? "" : currUserEmail.Email,
                    ToName = item.Name,
                    ToEmailAddress = item.Email,
                    Subject = emailInput.EmailSubject,
                    BodyEmail = CreateBodyMailEquipmentFulfillment(emailInput)
                };
                listEmail.Add(email);
            }

            // Send/Insert email to tbl_mail
            foreach (var mail in listEmail)
            {
                _sqlSPRepo.InsertEmail(mail);
            }
        }

        private string CreateBodyMailEquipmentFulfillment(GetUserAndEmailInput emailInput)
        {
            var bodyMail = new StringBuilder();

            bodyMail.Append("Dear " + emailInput.Recipient + "," + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("Quality Inspections sudah tersedia, Silakan melanjutkan proses berikutnya: " + Environment.NewLine + Environment.NewLine);
            bodyMail.Append("<p><a href= webrooturl/MaintenanceEquipmentQualityInspection/Index/" + emailInput.LocationCode + "/"
                                                                   + emailInput.RequestNumber + "/"
                                                                   + emailInput.IDResponsibility.ToString() + ">"
                                                                   + emailInput.FunctionNameDestination + "</a></p>"
                                                                   + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);

            bodyMail.Append("Note: To Protect against viruses, e-mail programs may prevent sending or receiving certain types of file attachments. Check your e-mail security settings" +
                            " to determine how attachments are handled");
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.IsBodyHtml = true;
            message.Body = bodyMail.ToString();

            return bodyMail.ToString();
        }
        #endregion

        public void RefreshDeltaViewTable()
        {
            var run = new Random();
            var queueDV = new QueueCopyDeltaView()
            {
                ID = 1,
                CreatedDate = DateTime.Now.AddMilliseconds(run.Next(100, 500))
            };

            _queueCopyDeltaView.Insert(queueDV);
            _uow.SaveChanges();

            //return Mapper.Map<QueueCopyDeltaView>(queueDV);
            //_sqlSPRepo.CopyDeltaView();
        }
    }
}
