using System;
using System.Collections.Generic;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.BusinessObjects;

namespace HMS.SKTIS.Contracts
{
    public interface IMaintenanceBLL
    {
        #region Equipment Request Old

        EquipmentRequestDTO InsertEquipmentRequest(EquipmentRequestDTO equipmentRequestDTO);

        EquipmentRequestDTO UpdateEquipmentRequest(EquipmentRequestDTO equipmentRequestDTO,
            EquipmentFulfillmentCompositeDTO equipmentFulfillmentDto = null, bool isUpdate = false);
        List<DateTime> GetEquipmentRequestDates();
        Dictionary<string, List<string>> GetItemCodeFromLocation();

        #endregion

        #region Item Disposal

        List<MasterMaintenanceItemLocationDTO> GetItemLocations(string locationCode);
        
        List<MntcEquipmentItemDisposalCompositeDTO> GetMntcEquipmentItemDisposals(GetItemDisposalInput input);

        MntcEquipmentItemDisposalCompositeDTO InsertItemDisposal(MntcEquipmentItemDisposalCompositeDTO itemDisposalDto);

        MntcEquipmentItemDisposalCompositeDTO UpdateItemDisposal(MntcEquipmentItemDisposalCompositeDTO itemDisposal);

        #endregion

        #region Maintenance Inventory

        //List<MstGenBrandPackageItemDTO> GetReportMstGenBrandPackageItem(GetMaintenanceInventoryInput input);
        MaintenanceInventoryDTO GetMntcInventoryAllView(GetMaintenanceInventoryInput input);

        MaintenanceInventoryDTO GetMntcBadStock(GetMaintenanceInventoryInput input);
        List<MaintenanceInventoryDTO> GetMntcInventoryAllViewDisposal(GetMaintenanceInventoryInput input);
        List<MaintenanceInventoryDTO> GetReportMntcInventoryAllView(GetMaintenanceInventoryInput input);
        MaintenanceInventoryDTO GetMaintenanceInventory(GetMaintenanceInventoryInput input);
        List<MaintenanceInventoryDTO> GetMaintenanceInventorys(GetMaintenanceInventoryInput input);

        List<GetDeltaViewFunction_Result> GetDeltaView(string LocationCode, DateTime dateTo);
        void SaveMntcInventory(MaintenanceInventoryDTO param);
        #endregion

        #region Equipment Request

        MntcEquipmentRequestCompositeDTO GetEquipmentRequest(GetEquipmentRequestsInput input);
        List<MntcEquipmentRequestCompositeDTO> GetEquipmentRequestsTable(GetEquipmentRequestsInput input);
        List<MntcEquipmentRequestCompositeDTO> GetEquipmentRequests(GetEquipmentRequestsInput input);
        List<MntcEquipmentRequestCompositeDTO> GetEquipmentRequestsLocation(GetEquipmentRequestsInput input);
        MntcEquipmentRequestCompositeDTO GetEquipmentRequestById(GetEquipmentRequestByIdInput input);

        #endregion

        #region Inventory

        InventoryByStatusViewDTO GetInventoryCurrentStock(GetInventoryCurrentStockInput input);
        InventoryStockDTO GetInventoryStock(GetInventoryStockInput input);
        #endregion

        #region Equipment Transfer

        List<EquipmentTransferCompositeDTO> GetEquipmentTransfers(GetEquipmentTransfersInput input);
        EquipmentTransferDTO InsertEquipmentTransfer(EquipmentTransferDTO equipmentTransfer);

        EquipmentTransferDTO UpdateEquipmentTransfer(EquipmentTransferDTO equipmentTransfer);
        int GetAvailableEndingStockForTransfer(EquipmentTransferDTO equipmentTransfer);
        string GetItemStatusForTransferByLocationCode(string locationCode);

        #endregion

        #region Equipment Fulfillment

        MntcRequestToLocationDTO SaveMaintenanceRequestToLocation(MntcRequestToLocationDTO param);
        MstMntcItemCompositeDTO GetItemDetail(string itemCode, string locationCode, string itemStatus = "");
        List<MaintenanceInventoryDTO> GetEquipmentFulfillmentDetails(string itemCode, string itemStatus);
        EquipmentFulfillmentCompositeDTO GetEquipmentFulfillment(GetEquipmentFulfillmentInput input);
        List<MntcFulfillmentViewDTO> GetEquipmentFulfillments(GetEquipmentFulfillmentInput input);
        EquipmentFulfillmentCompositeDTO SaveEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto);
        bool IsExistEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto);
        bool IsExistFulfillmentByRequestDate(EquipmentFulfillmentCompositeDTO fulfillmentDto);
        void DeleteEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto);
        EquipmentFulfillmentCompositeDTO UpdateEquipmentFulfillment(EquipmentFulfillmentCompositeDTO fulfillmentDto);
        List<MntcRequestToLocationDTO> GetMntcRequestToLocation(string requestNumber, DateTime? fulfillmentDate);
        #endregion

        #region Equipment Receive

        List<EquipmentReceiveCompositeDTO> GetEquipmentReceives(GetEquipmentReceivesInput input);
        EquipmentReceiveDTO UpdateEquipmentReceive(EquipmentReceiveDTO equipmentReceive);

        #endregion

        #region EquipmentConversion

        List<MaintenanceExecutionItemConversionDTO> GetMaintenanceEquipmentItemConvertsExcel(GetMaintenanceEquipmentItemConvertInput input);
        List<MaintenanceExecutionItemConversionDTO> GetMaintenanceEquipmentItemConverts(GetMaintenanceEquipmentItemConvertInput input);
        MaintenanceExecutionItemConversionDTO SaveMaintenanceExecutionItemConversionESDetail(MaintenanceExecutionItemConversionDTO executionItemConversionDto);
        MaintenanceExecutionItemConversionDTO UpdateMaintenanceExecutionItemConversionES(MaintenanceExecutionItemConversionDTO executionItemConversionDto);
        MaintenanceExecutionItemConversionDTO InsertMaintenanceExecutionItemConversion(MaintenanceExecutionItemConversionDTO executionItemConversionDto);
        MaintenanceExecutionItemConversionDTO UpdateMaintenanceExecutionItemConversion(MaintenanceExecutionItemConversionDTO executionItemConversionDto);
        List<MaintenanceExecutionItemConversionCompositeDTO> GetMaintenanceExecutionItemConversionComposites(string itemCodeSource, GetMaintenanceEquipmentItemConvertInput criteria);

        #endregion

        #region Maintenance Equipment Quaity Inspection

        List<MaintenanceExecutionQualityInspectionDTO> GetMaintenanceExecutionQualityInspections(GetMaintenanceExecutionQualityInspectionInput input);
        MaintenanceExecutionQualityInspectionDTO InsertMaintenanceExecutionQualityInspection(MaintenanceExecutionQualityInspectionDTO inspectionDto);
        MaintenanceExecutionQualityInspectionDTO UpdateMaintenanceExecutionQualityInspection(MaintenanceExecutionQualityInspectionDTO inspectionDto);
        EquipmentFulfillmentCompositeDTO GetPurchaseNumber(GetEquipmentFulfillmentInput input);
        #endregion

        #region Equipment Repair
        List<EquipmentRepairDTO> GetPlantEquipmentRepairs(GetPlantEquipmentRepairsInput input);
        EquipmentRepairDTO SaveEquipmentRepair(EquipmentRepairDTO equipmentRepair);
        MntcRepairItemUsageDTO SaveRepairItemUsage(MntcRepairItemUsageDTO repairItemUsage);
        List<EquipmentRepairTPODTO> GetTPOEquipmentRepairs(GetPlantEquipmentRepairsTPOInput input);
        EquipmentRepairTPODTO SaveEquipmentRepairTPO(EquipmentRepairTPODTO equipmentRepair);
        #endregion

        #region Equipment Stock Report
        List<MaintenanceEquipmentStockReportDTO> GetMaintenanceEquipmentStockReport(GetMaintenanceEquipmentStockReportInput input);
        #endregion

        #region Equipment Requirement Report
        List<MaintenanceEquipmentRequirementReportDTO> GetMaintenanceEquipmentRequirementReport(string locationCode, string brandGroupCode, float? userPackage, DateTime date);
        List<MaintenanceEquipmentRequirementReportDTO> GetMaintenanceEquipmentRequirementReport2(string locationCode, string brandGroupCodeFrom, string brandGroupCodeTo, DateTime date);
        List<EquipmentRequirementDTO> GetEquipmentRequirementSummaryLocations(GetEquipmentRequirementSummaryInput input);
        List<EquipmentRequirementSummaryItemDTO> GetEquipmentSummaryItem(string locationCode, string brandGroupCode, DateTime date);
        int? GetRealStock(string locationCode, string itemCode, DateTime date);
        #endregion

        List<SparepartDTO> GetAllRepairItemUsages(GetRepairItemUsageInput input);

        #region Maintenance Execution Inventory
        List<MaintenanceExecutionInventoryViewDTO> GetMaintenanceExecutionInventoryView(MaintenanceExecutionInventoryViewInput input);

        //List<MaintenanceExecutionInventoryViewDTO> GetInventoryView(MaintenanceExecutionInventoryViewInput input);

        List<MaintenanceExecutionInventoryViewDTO> GetInventory(string date, string locationCode, string itemType);
        MaintenanceExecutionInventoryViewDTO GetMaintenanceExecutionInventoryView(string locationCode, string itemCode, DateTime? inventoryDate);

        List<MaintenanceExecutionInventoryFunction_Result> GetInventoryView(DateTime date, string locationCode, string itemType,
            string QParam, string UserAD);

        MaintenanceExecutionInventoryViewDTO GetMaintenanceExecutionInventoryTransferView(string locationCode, string itemCode, DateTime? inventoryDate);

        MntcEquipmentQualityInspection GetMntcEquipmentQualityInspecton(string locationCode, string itemCode, DateTime? transactionDate, string requestNumber);

        MntcEquipmentRequestCompositeDTO GetEquipmentRequestQtyLeftOver(string locationCode, string itemCode, string requestNumber);
        #endregion

        #region Maintenance Execution Inventory Adjustment
        List<MaintenanceExecutionInventoryAdjustmentDTO> GetMaintenanceExecutionInventoryAdjustment(GetMaintenanceExecutionInventoryAdjustmentInput input);
        MaintenanceExecutionInventoryAdjustmentDTO InsertInventoryAdjustment(MaintenanceExecutionInventoryAdjustmentDTO inventoryAdjustmentDTO);
        MaintenanceExecutionInventoryAdjustmentDTO UpdateInventoryAdjustment(MaintenanceExecutionInventoryAdjustmentDTO inventoryAdjustmentDTO);
        MaintenanceExecutionInventoryAdjustmentDTO DeleteInventoryAdjustment(MaintenanceExecutionInventoryAdjustmentDTO inventoryAdjustmentDTO);
        #endregion

        #region Mails
        void SendEmailSaveEquipmentRequest(string locationCode, DateTime RequestDate, string RequestNumber, string currUserName);
        void SendEmailSaveEquipmentFulfillment(string locationCode, DateTime Date, string RequestNumber, string currUserName);
        void SendEmailSaveEquipmentTransfer(string SourceLocationCode, DateTime TransferDate, string DestinationLocationCode, string DestinationUnitCode, string currUserName);
        #endregion

        void RefreshDeltaViewTable();

        List<MntcEquipmentStockFunction_Result> GetReportMaintenanceEquipmentStock(DateTime date, string locationCode,
            string unitCode, string QParam, string UserAD);
    }
}
