using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using AutoMapper;
using AutoMapper.Internal;
using HMS.SKTIS.AutoMapperExtensions;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;

namespace HMS.SKTIS.BLL
{
    public class BLLMapper
    {
        public static void Initialize()
        {
            #region NewsInfo

            Mapper.CreateMap<NewsInfo, NewsInfoCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IsSlider,
                    opt => opt.ResolveUsing<NullableBooleanToBooleanResolver>().FromMember(src => src.IsSlider))
                    .ForMember(dest => dest.IsInformatips,
                    opt => opt.ResolveUsing<NullableBooleanToBooleanResolver>().FromMember(src => src.IsInformatips));
            Mapper.CreateMap<NewsInfo, NewsInfoDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IsSlider,
                    opt => opt.ResolveUsing<NullableBooleanToBooleanResolver>().FromMember(src => src.IsSlider))
                    .ForMember(dest => dest.IsInformatips,
                    opt => opt.ResolveUsing<NullableBooleanToBooleanResolver>().FromMember(src => src.IsInformatips));
            Mapper.CreateMap<NewsInfo, SliderNewsInfoDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<NewsInfo, InformatipsNewsInfoDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<NewsInfoDTO, NewsInfo>().IgnoreAllNonExisting();

            #endregion

            #region Master Data

            #region General

            #region General List
            Mapper.CreateMap<MstGenList, MstGeneralListCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGeneralListCompositeDTO, MstGenList>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGeneralListDTO, MstGenList>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<MstGenList, MstGeneralListDTO>().IgnoreAllNonExisting();
            #endregion

            #region Location
            Mapper.CreateMap<MstGenLocation, MstGenLocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenLocationDTO, MstGenLocation>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetLocations_Result, MstGenLocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetMstGenLocationsByParentCode_Result, MstGenLocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetLastChildLocation_Result, LocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetLastChildLocation_Result, LocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<BrandCodeByLocationView, BrandCodeByLocationCodeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetLocationsByLevel_Result, MstGenLocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetLocations_Result, MstGenLocationDTO>().IgnoreAllNonExisting();
            #endregion

            #region Standard Hours
            Mapper.CreateMap<MstGenStandardHour, MstGenStandardHourDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenStandardHourDTO, MstGenStandardHour>().IgnoreAllNonExisting();
            #endregion

            #region Holiday
            Mapper.CreateMap<MstGenHoliday, MstHolidayDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstHolidayDTO, MstGenHoliday>().IgnoreAllNonExisting();
            #endregion

            #region Brand Group
            Mapper.CreateMap<MstGenBrandGroup, MstGenBrandGroupDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenBrandGroupDTO, MstGenBrandGroup>().IgnoreAllNonExisting();
            #endregion

            #region Process
            Mapper.CreateMap<MstGenProcess, MstGenProcessDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenProcessDTO, MstGenProcess>().IgnoreAllNonExisting();
            #endregion

            #region Brand Package Item
            Mapper.CreateMap<MstGenBrandPackageItem, MstGenBrandPackageItemDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src => src.MstMntcItem.ItemDescription));
            Mapper.CreateMap<MstGenBrandPackageItemDTO, MstGenBrandPackageItem>().IgnoreAllNonExisting();
            #endregion

            #region Brand Package Mapping
            Mapper.CreateMap<MstGenBrandPkgMapping, MstGenBrandPkgMappingDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenBrandPkgMappingDTO, MstGenBrandPkgMapping>().IgnoreAllNonExisting();
            #endregion

            #region Week

            Mapper.CreateMap<MstGenWeek, MstGenWeekDTO>().IgnoreAllNonExisting()
                .ForMember(src => src.IdMasterWeek, opt => opt.MapFrom(dest => dest.IDMstWeek));

            #endregion

            #endregion

            #region PLANT

            #region Production Group
            Mapper.CreateMap<MstPlantProductionGroup, MstPlantProductionGroupDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Leader1Name, opt => opt.MapFrom(src => src.MstPlantEmpJobsDataAll1.EmployeeName))
                .ForMember(dest => dest.Leader2Name, opt => opt.MapFrom(src => src.MstPlantEmpJobsDataAll2.EmployeeName))
                .ForMember(dest => dest.LeaderInspectionName, opt => opt.MapFrom(src => src.MstPlantEmpJobsDataAll.EmployeeName));
            Mapper.CreateMap<MstPlantProductionGroupDTO, MstPlantProductionGroup>().IgnoreAllNonExisting()
                .ForMember(dest => dest.InspectionLeader, opt => opt.MapFrom(src => src.LeaderInspection))
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            Mapper.CreateMap<MstPlantProductionGroupView, MstPlantProductionGroupCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstPlantProductionGroup, MstPlantProductionGroupCompositeDTO>().IgnoreAllNonExisting();
            #endregion

            #region Employee Job Data
            Mapper.CreateMap<MstPlantEmpJobsDataAcv, MstEmployeeJobsDataActiveCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstPlantEmpJobsDataAcv, MstEmployeeJobsDataActiveDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TitleId, opt => opt.MapFrom(src => src.Title_id));
            #endregion

            #endregion

            #region TPO

            #region Master TPO Production Group
            Mapper.CreateMap<MstTPOProductionGroup, MstTPOProductionGroupDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdGroupComputed, opt => opt.MapFrom(src => src.ProdGroup));
            Mapper.CreateMap<MstTPOProductionGroupDTO, MstTPOProductionGroup>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            #endregion

            #endregion

            #region Maintenance

            #region Item
            Mapper.CreateMap<MstMntcItem, MstMntcItemCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstMntcItemCompositeDTO, MstMntcItem>().IgnoreAllNonExisting();
            #endregion

            #region Item Location
            Mapper.CreateMap<MstMntcItemLocation, MstMntcItemLocationDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src => src.MstMntcItem.ItemDescription))
                .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.MstMntcItem.UOM))
                .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.MstMntcItem.ItemType));
            Mapper.CreateMap<MstMntcItemLocationDTO, MstMntcItemLocation>().IgnoreAllNonExisting();
            #endregion

            #region Inventory
            Mapper.CreateMap<InventoryByStatusView, InventoryByStatusViewDTO>().IgnoreAllNonExisting();
            #endregion

            #endregion

            #endregion

            #region MasterTPO
            Mapper.CreateMap<MstTPOInfo, MstTPOInfoDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.MstGenLocation.LocationName));
            Mapper.CreateMap<MstTPOInfoDTO, MstTPOInfo>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            #endregion

            #region Master Brand

            Mapper.CreateMap<MstGenBrand, BrandDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<BrandDTO, MstGenBrand>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenBrand, BrandCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StickPerBox, opt => opt.MapFrom(src => src.MstGenBrandGroup.StickPerBox));
            Mapper.CreateMap<MstProcessBrandView, MstProcessBrandViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstProcessBrandViewDTO, MstProcessBrandView>().IgnoreAllNonExisting();

            #endregion

            #region Master Brand Group Material

            Mapper.CreateMap<MstGenMaterial, BrandGroupMaterialDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<BrandGroupMaterialDTO, MstGenMaterial>().IgnoreAllNonExisting();

            #endregion

            #region Master Unit
            Mapper.CreateMap<MstPlantUnitView, MstPlantUnitCompositeDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<MstPlantUnitDTO, MstPlantUnit>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            Mapper.CreateMap<MstPlantUnit, MstPlantUnitDTO>().IgnoreAllNonExisting();
            #endregion

            #region MstTPOPackage

            Mapper.CreateMap<MstTPOPackage, MstTPOPackageDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.MstTPOInfo.MstGenLocation.LocationName))
                .ForMember(dest => dest.BrandGroupCode, opt => opt.MapFrom(src => src.MstGenBrandGroup.BrandGroupCode))
                .ForMember(dest => dest.EffectiveDateOld, opt => opt.MapFrom(src => src.EffectiveDate));
            Mapper.CreateMap<MstTPOPackageDTO, MstTPOPackage>().IgnoreAllNonExisting();
            #endregion

            #region MstTPOFreeRate

            Mapper.CreateMap<MstTPOFeeRate, MstTPOFeeRateDTO>().IgnoreAllNonExisting()
               .ForMember(dest => dest.PreviousEffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate));
            Mapper.CreateMap<MstTPOFeeRateDTO, MstTPOFeeRate>().IgnoreAllNonExisting();
            #endregion

            #region Master General Emp Status
            Mapper.CreateMap<MstGenEmpStatu, MstGenEmpStatusCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenEmpStatu, MstGenEmpStatusDTO>().IgnoreAllNonExisting();
            #endregion

            #region Master General Process Setting
            Mapper.CreateMap<MstGenProcessSetting, MasterProcessSettingLocationDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenProcessSetting, MstGenProcessSettingCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenProcessSetting, MstGenProcessSettingDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProcessOrder, opt => opt.MapFrom(src => src.MstGenProcess.ProcessOrder));
            Mapper.CreateMap<MstGenProcessSettingDTO, MstGenProcessSetting>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<MstGenProcessSettingsLocation, MstGenProcessSettingsLocationCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.MstGenLocation.LocationName));
            Mapper.CreateMap<MstGenProcessSettingsLocation, MstGenProcessSettingLocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenProcessSettingLocationDTO, MstGenProcessSettingsLocation>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            Mapper.CreateMap<ProcessSettingsAndLocationView, ProcessSettingsAndLocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ProcessSettingsAndLocationView, MstGenProcessSettingsLocationCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.IDProcess, opt => opt.MapFrom(src => src.IDProcess))
                .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.LocationCode))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationName));
            #endregion

            #region Master Absent Type
            Mapper.CreateMap<MstPlantAbsentType, MstPlantAbsentTypeDTO>().IgnoreAllNonExisting()
            .ForMember(dest => dest.OldAbsentType, opt => opt.MapFrom(src => src.AbsentType));
            Mapper.CreateMap<MstPlantAbsentTypeDTO, MstPlantAbsentType>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            #endregion

            #region Maintainance

            #region Maintainance Execution Equipment Request Old

            Mapper.CreateMap<MntcEquipmentRequest, EquipmentRequestDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src => src.MstMntcItemLocation.MstMntcItem.ItemDescription));
            Mapper.CreateMap<EquipmentRequestDTO, MntcEquipmentRequest>().IgnoreAllNonExisting();

            Mapper.CreateMap<EquipmentFulfillmentCompositeDTO, EquipmentRequestDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ApprovedQty, opt => opt.MapFrom(src => src.ApprovedQty));

            #endregion

            #region Equipment Request

            Mapper.CreateMap<MntcEquipmentRequest, MntcEquipmentRequestCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<EquipmentRequestView, MntcEquipmentRequestCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.OnUsed, opt => opt.MapFrom(src => src.OnUse));

            #endregion

            #region Equipment Fulfillment

            Mapper.CreateMap<MntcRequestToLocationDTO, MntcRequestToLocation>().IgnoreAllNonExisting();

            Mapper.CreateMap<MntcEquipmentFulfillment, EquipmentFulfillmentCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemDescription,
                    opt => opt.MapFrom(src => src.MntcEquipmentRequest.MstMntcItemLocation.MstMntcItem.ItemDescription))
                .ForMember(dest => dest.ReadyToUse,
                    opt =>
                        opt.ResolveUsing<MappingReadyToUse>()
                            .FromMember(src => src.MntcEquipmentRequest.MstMntcItemLocation.MntcInventories))
                .ForMember(dest => dest.OnUse,
                    opt =>
                        opt.ResolveUsing<MappingOnUse>()
                            .FromMember(src => src.MntcEquipmentRequest.MstMntcItemLocation.MntcInventories))
                .ForMember(dest => dest.OnRepair,
                    opt =>
                        opt.ResolveUsing<MappingOnRepair>()
                            .FromMember(src => src.MntcEquipmentRequest.MstMntcItemLocation.MntcInventories))
                .ForMember(dest => dest.RequestedQuantity, opt => opt.MapFrom(src => src.MntcEquipmentRequest.Qty))
                .ForMember(dest => dest.ApprovedQty, opt => opt.MapFrom(src => src.MntcEquipmentRequest.Qty));
            Mapper.CreateMap<EquipmentRequestView, EquipmentFulfillmentCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RequestedQuantity, opt => opt.MapFrom(src => src.TotalQty))
                .ForMember(dest => dest.ApprovedQty, opt => opt.MapFrom(src => src.TotalQty));

            Mapper.CreateMap<EquipmentFulfillmentCompositeDTO, MntcEquipmentFulfillment>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.UpdatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.PurchaseQty, opt => opt.MapFrom(src => src.PurchaseQuantity));

            Mapper.CreateMap<MaintenanceEquipmentFulfillmentView, EquipmentFulfillmentCompositeDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<MntcFulfillmentView, MntcFulfillmentViewDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<MaintenanceEquipmentFulfillmentDetailView, MaintenanceInventoryDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetDeltaViewFunction_Result, MaintenanceInventoryDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MaintenanceEquipmentFulfillmentItemDetailView, MstMntcItemCompositeDTO>()
                .IgnoreAllNonExisting();
            #endregion

            #region Inventory

            Mapper.CreateMap<MntcInventory, MaintenanceInventoryDTO>().IgnoreAllNonExisting();
            
            Mapper.CreateMap<MaintenanceInventoryDTO, MntcInventory>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BeginningStock, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.StockIn, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.EndingStock, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            Mapper.CreateMap<MntcInventory, InventoryStockDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BeginningStock,
                    opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.BeginningStock))
                    .ForMember(dest => dest.StockIn,
                    opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.StockIn))
                    .ForMember(dest => dest.StockOut,
                    opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.StockOut))
                    .ForMember(dest => dest.EndingStock,
                    opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.EndingStock));

            Mapper.CreateMap<MntcInventoryAll, InventoryStockDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BeginningStock,
                    opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.BeginningStock))
                    .ForMember(dest => dest.StockIn,
                    opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.StockIn))
                    .ForMember(dest => dest.StockOut,
                    opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.StockOut))
                    .ForMember(dest => dest.EndingStock,
                    opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.EndingStock));

            Mapper.CreateMap<MntcInventoryAll, MaintenanceInventoryDTO>().IgnoreAllNonExisting();
            //.ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCode))
            //.ForMember(dest => dest.EndingStock, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.EndingStock));
            #endregion

            #region Equipment Transfer
            Mapper.CreateMap<MntcEquipmentMovement, EquipmentTransferCompositeDTO>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src => src.MstMntcItem.ItemDescription))
                 .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.MstMntcItem.UOM));

            Mapper.CreateMap<EquipmentTransferDTO, MntcEquipmentMovement>().IgnoreAllNonExisting();
            Mapper.CreateMap<MntcEquipmentMovement, EquipmentTransferDTO>().IgnoreAllNonExisting();
            #endregion

            #region Equipment Receive
            Mapper.CreateMap<MntcEquipmentMovement, EquipmentReceiveCompositeDTO>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src => src.MstMntcItem.ItemDescription))
                 .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.MstMntcItem.UOM));

            Mapper.CreateMap<EquipmentReceiveDTO, MntcEquipmentMovement>().IgnoreAllNonExisting();
            Mapper.CreateMap<MntcEquipmentMovement, EquipmentReceiveDTO>().IgnoreAllNonExisting();
            #endregion

            #region Equipment Stock Report
            Mapper.CreateMap<MaintenanceEquipmentStockView, MaintenanceEquipmentStockReportDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<GetMaintenanceEquipmentStockView_Result, MaintenanceEquipmentStockReportDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<GetMaintenanceEquipmentStockFunction_Result, MaintenanceEquipmentStockReportDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<MntcRequestToLocation, MntcRequestToLocationDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<EquipmentFulfillmentCompositeDTO, MntcRequestToLocation>().IgnoreAllNonExisting()
                //TODO reminder to check this mapper
                //.ForMember(dest => dest.QtyFromLocation, opt => opt.MapFrom(src => src.Quantity))
                //.ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.LocationCodeForReqToLocation))
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.UpdatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));



            #endregion

            #region Equipment Requirement Report
            Mapper.CreateMap<GetEquipmentRequirementReport_Result, MaintenanceEquipmentRequirementReportDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetEquipmenrRequiremenrReport2_Result, MaintenanceEquipmentRequirementReportDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<EquipmentRequirementView, EquipmentRequirementDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetEquipmentRequirementItem_Result, EquipmentRequirementSummaryItemDTO>().IgnoreAllNonExisting();
            #endregion

            #region Equipment Repair
            Mapper.CreateMap<MntcEquipmentRepair, EquipmentRepairDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<EquipmentRepairDTO, MntcEquipmentRepair>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            Mapper.CreateMap<MntcEquipmentRepair, EquipmentRepairTPODTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<EquipmentRepairTPODTO, MntcEquipmentRepair>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            #endregion

            #region Equipment Repair Item Usage
            Mapper.CreateMap<MntcRepairItemUsageDTO, MntcRepairItemUsage>().IgnoreAllNonExisting();
            Mapper.CreateMap<MntcRepairItemUsage, MntcRepairItemUsageDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<EquipmentRepairTPODTO, MntcRepairItemUsageDTO>().IgnoreAllNonExisting()
                .ForMember(m => m.ItemCodeSource, opt => opt.MapFrom(p => p.ItemCode));

            Mapper.CreateMap<MntcRepairItemUsage, SparepartDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCodeDestination))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QtyUsage))
                .ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src => src.MstMntcItem.ItemDescription))
                .ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.MstMntcItem.UOM));
            #endregion

            #region Equipment Repair Item Usage
            Mapper.CreateMap<MntcRepairItemUsageDTO, MntcRepairItemUsage>().IgnoreAllNonExisting();
            Mapper.CreateMap<MntcRepairItemUsage, MntcRepairItemUsageDTO>().IgnoreAllNonExisting();
            #endregion

            #endregion

            #region Master Maintenance Convert

            Mapper.CreateMap<MstMntcConvert, MstMntcConvertDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemCodeSourceDescription,
                    opt => opt.MapFrom(src => src.MstMntcItem.ItemDescription))
                .ForMember(dest => dest.ItemCodeDestinationDescription,
                    opt => opt.MapFrom(src => src.MstMntcItem1.ItemDescription));
            Mapper.CreateMap<MstMntcConvertDTO, MstMntcConvert>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<MstMntcConvertGetItemDestinationView, MstMntcConvertCompositeDTO>()
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCodeDest))
                .ForMember(dest => dest.ItemCodeSource, opt => opt.MapFrom(src => src.ItemCode))
                .ForMember(dest => dest.QtyConvert, opt => opt.MapFrom(src => src.Qty));
            Mapper.CreateMap<MstMntcItemLocation, MasterMaintenanceItemLocationDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<MstMntcConvert, SparepartDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemCode,
                    opt => opt.MapFrom(src => src.ItemCodeDestination))
                .ForMember(dest => dest.UOM,
                    opt => opt.MapFrom(src => src.MstMntcItem1.UOM))
                .ForMember(dest => dest.ItemDescription,
                    opt => opt.MapFrom(src => src.MstMntcItem1.ItemDescription));

            Mapper.CreateMap<MstMntcItem, MasterMaintenanceItemLocationDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCode))
                .ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src => src.ItemDescription));
            #endregion

            #region Maintenance Repair Item Usage
            Mapper.CreateMap<MntcRepairItemUsageView, SparepartDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCodeDestination));
            //.ForMember(dest => dest.ItemDescription, opt => opt.MapFrom(src => src.MstMntcItem.ItemDescription))
            //.ForMember(dest => dest.UOM, opt => opt.MapFrom(src => src.MstMntcItem.UOM))
            //.ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QtyUsage));
            #endregion

            #region Item Disposal

            Mapper.CreateMap<MntcEquipmentItemDisposal, MntcEquipmentItemDisposalCompositeDTO>()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCode))
                .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.LocationCode))
                .ForMember(dest => dest.QtyDisposal, opt => opt.MapFrom(src => src.QtyDisposal))
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => src.Shift))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.ItemDescription,
                    opt => opt.MapFrom(src => src.MstMntcItemLocation.MstMntcItem.ItemDescription))
                .ForMember(dest => dest.EndingStock,
                    opt => opt.MapFrom(src => src.MstMntcItemLocation.MntcInventories.OrderByDescending(m => m.InventoryDate).Where(m => m.ItemStatus.ToUpper() == EnumHelper.GetDescription(Enums.ItemStatus.BadStock).ToUpper()).Select(p => p.EndingStock).FirstOrDefault()));


            Mapper.CreateMap<MntcEquipmentItemDisposalCompositeDTO, MntcEquipmentItemDisposal>().IgnoreAllNonExisting();

            #endregion

            #region Plan Plant Group Shift

            Mapper.CreateMap<PlanPlantGroupShiftDTO, PlanPlantGroupShift>().IgnoreAllNonExisting()
            .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
            .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<PlanPlantGroupShift, PlanPlantGroupShiftDTO>().IgnoreAllNonExisting();

            #endregion

            #region Planning
            #region WPP
            Mapper.CreateMap<PlanWeeklyProductionPlanning, PlanWeeklyProductionPlanningDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<PlanWeeklyProductionPlanningDTO, PlanWeeklyProductionPlanning>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            Mapper.CreateMap<PlanTmpWeeklyProductionPlanning, PlanWeeklyProductionPlanningDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<PlanWeeklyProductionPlanningDTO, PlanTmpWeeklyProductionPlanning>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            #endregion

            #region Plant
            #region TPU

            // Temporarty Fix Only!
            // @todo Change view !
            Mapper.CreateMap<TargetProductionUnitView, PlanTPUCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Value1, opt => opt.ResolveUsing<DecimalToFloatResolver>().FromMember(src => src.Value1));
            Mapper.CreateMap<TargetProductionUnitPerBoxView, PlanTPUCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Value1, opt => opt.ResolveUsing<DecimalToFloatResolver>().FromMember(src => src.Value1));

            Mapper.CreateMap<PlanTargetProductionUnit, PlanTPUDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanTPUDTO, PlanTargetProductionUnit>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanTargetProductionUnit, TargetManualTPUDTO>().IgnoreAllNonExisting();
            #endregion

            #region TPK
            Mapper.CreateMap<PlanPlantTargetProductionKelompok, PlantTPKCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlantTPKDTO, PlanPlantTargetProductionKelompok>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlantTPKByProcessDTO, WIPStockDTO>().IgnoreAllNonExisting();
            #endregion

            #region Individual Capacity

            Mapper.CreateMap<PlanPlantIndividualCapacityWorkHour, PlanningPlantIndividualCapacityWorkHourDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeName))
                .ForMember(dest => dest.EmployeeNumber, opt => opt.MapFrom(src => src.MstPlantEmpJobsDataAll.EmployeeNumber));
            Mapper.CreateMap<PlanningPlantIndividualCapacityWorkHourDTO, PlanPlantIndividualCapacityWorkHour>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            Mapper.CreateMap<PlanPlantIndividualCapacityByReferenceView, PlanningPlantIndividualCapacityByReferenceDTO>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.WorkHours, opt => opt.MapFrom(src => src.WorkHour));
            Mapper.CreateMap<ExePlantProductionEntryVerification, ExePlantProductionEntryVerificationDTO>().IgnoreAllNonExisting()
                .ForMember(
                dest => dest.ExeProductionEntryRelease, 
                opt => opt.MapFrom(src => (src.ExeProductionEntryRelease == null ? null : src.ExeProductionEntryRelease)));


            Mapper.CreateMap<GetPlanPlantIndividualCapacityByReference_Result, PlanningPlantIndividualCapacityByReferenceDTO>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.WorkHours, opt => opt.MapFrom(src => src.WorkHour));
            #endregion

            #region WIPDetails
            Mapper.CreateMap<PlanPlantWIPDetail, WIPStockDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<WIPStockDTO, PlanPlantWIPDetail>().IgnoreAllNonExisting();
            #endregion
            #endregion

            #region TPO
            Mapper.CreateMap<TPOTargetProductionKelompokView, PlanTPOTPKCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanTPOTPKCompositeDTO, TPOTargetProductionKelompokView>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOTPKDTO, PlanTPOTargetProductionKelompok>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanTPOTargetProductionKelompok, TPOTPKDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOTPKByProcessDTO, WIPStockDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanTPOTargetProductionKelompokBox, PlanTPOTPKTotalBoxDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanTPOTPKTotalBoxDTO, PlanTPOTargetProductionKelompokBox>().IgnoreAllNonExisting();
            #endregion

            #region Report

            Mapper.CreateMap<GetReportSummaryDailyProductionTargets_Result, PlanningReportProductionTargetCompositeDTO>();
            
            Mapper.CreateMap<PlanningReportProductionTargetCompositeDTO, GetReportSummaryDailyProductionTargets_Result>();

            Mapper.CreateMap<GetReportSummaryProcessTargets_Result, PlanningReportSummaryProcessTargetsCompositeDTO>();

            Mapper.CreateMap<GetTPOReportsProduction_Result, TPOFeeReportsProductionDTO>();

            Mapper.CreateMap<TPOFeeReportsProductionDailyView, TPOFeeReportsProductionDTO>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.FeeDate))
                .ForMember(dest => dest.EndDate,   opt => opt.MapFrom(src => src.FeeDate));
            #endregion

            #region PlanTmpWeeklyProductionPlanning
            Mapper.CreateMap<PlanTmpWeeklyProductionPlanning, PlanTmpWeeklyProductionPlanningDTO>().IgnoreAllNonExisting();
            #endregion
            Mapper.CreateMap<GetWorkerBrandAssignmentPlanningPlantTPK_Result, WorkerBrandAssigmentDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == "1"));

            #endregion

            #region Maintenance

            #region MaintenanceEquipmentItemConvert
            Mapper.CreateMap<MntcEquipmentItemConvert, MaintenanceExecutionItemConversionDTO>().IgnoreAllNonExisting()
               .ForMember(dest => dest.ItemCodeSourceDescription, opt => opt.MapFrom(src => src.MstMntcConvert.MstMntcItem.ItemDescription))
               .ForMember(dest => dest.ItemCodeDestinationDescription, opt => opt.MapFrom(src => src.MstMntcConvert.MstMntcItem1.ItemDescription))
               .ForMember(dest => dest.SourceStock, opt => opt.MapFrom(src => src.SourceQty));
            Mapper.CreateMap<MaintenanceExecutionItemConversionDTO, MntcEquipmentItemConvert>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.SourceQty, opt => opt.MapFrom(src => src.SourceStock));

            Mapper.CreateMap<MaintenanceItemConversionDestinationView, MaintenanceExecutionItemConversionCompositeDTO>().IgnoreAllNonExisting();

            #endregion

            #region MaintenanceExecutionQualityInspection
            Mapper.CreateMap<MntcEquipmentQualityInspection, MaintenanceExecutionQualityInspectionDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MaintenanceExecutionQualityInspectionDTO, MntcEquipmentQualityInspection>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            #endregion

            #region Maintenance Execution Inventory
            Mapper.CreateMap<MaintenanceExecutionInventoryView, MaintenanceExecutionInventoryViewDTO>().IgnoreAllNonExisting();
            #endregion

            #region Maintenance Execution Inventory Adjustment
            Mapper.CreateMap<MntcInventoryAdjustment, MaintenanceExecutionInventoryAdjustmentDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemCodeDescription, opt => opt.MapFrom(src => src.MstMntcItem.ItemDescription));
            Mapper.CreateMap<MaintenanceExecutionInventoryAdjustmentDTO, MntcInventoryAdjustment>().IgnoreAllNonExisting();
            #endregion

            #endregion

            #region Utilities
            Mapper.CreateMap<UtilTransactionLog, UtilTransactionLogDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetTransactionHistory_Result, TransactionHistoryDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetTransactionHistoryWagesProdcardCorrection_Result, TransactionHistoryDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetTransactionFlow_Result, TransactionFlowDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilFlowFunctionView, UtilFlowFunctionViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilRole, UtilRoleDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilRoleDTO, UtilRole>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilRuleDTO, UtilRule>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilRule, UtilRuleDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilFunction, UtilFunctionDTO>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.ParentNameFunction, opt => opt.MapFrom(src => src.UtilFunction1.FunctionName));
            Mapper.CreateMap<UtilFunctionDTO, UtilFunction>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilResponsibilityDTO, UtilResponsibility>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilUsersResponsibilityDTO, UtilUsersResponsibility>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilUsersResponsibility, UtilUsersResponsibilityDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilUserResponsibilitiesRoleViewDTO, UtilUserResponsibilitiesRoleView>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilUserResponsibilitiesRoleView, UtilUserResponsibilitiesRoleViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilResponsibility, UtilResponsibilityDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RolesName, opt => opt.MapFrom(src => src.UtilRole.RolesName)); ;
            
            #region UtilRolesFunction
            Mapper.CreateMap<UtilRolesFunction, UtilRolesFunctionDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilRolesFunctionDTO, UtilRolesFunction>().IgnoreAllNonExisting();
            #endregion

            Mapper.CreateMap<UtilDelegation, UtilDelegationDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UserADToOld, opt => opt.MapFrom(src => src.UserADTo))
                .ForMember(dest => dest.IDResponsibilityOld, opt => opt.MapFrom(src => src.IDResponsibility))
                .ForMember(dest => dest.UserADFromOld, opt => opt.MapFrom(src => src.UserADFrom))
                .ForMember(dest => dest.StartDateOld, opt => opt.MapFrom(src => src.StartDate))
                ;
            Mapper.CreateMap<UtilDelegationDto, UtilDelegation>().IgnoreAllNonExisting();
            #endregion

            #region UtilWorkflow
            Mapper.CreateMap<UtilFlowDTO, UtilFlow>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilFlow, UtilFlowDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.FormSourceName, opt => opt.MapFrom(src => src.UtilFunction1.FunctionName))
                .ForMember(dest => dest.ActionButtonName, opt => opt.MapFrom(src => src.UtilFunction.FunctionName))
                .ForMember(dest => dest.DestinationFormName, opt => opt.MapFrom(src => src.UtilRolesFunction.UtilFunction.FunctionName))
                .ForMember(dest => dest.DestinationRoleName, opt => opt.MapFrom(src => src.UtilRolesFunction.UtilRole.RolesCode));
            #endregion

            #region Execution

            #region Worker Absenteeism
            Mapper.CreateMap<ExePlantWorkerAbsenteeism, ExePlantWorkerAbsenteeismDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                ;
            Mapper.CreateMap<ExePlantWorkerAbsenteeismDTO, ExePlantWorkerAbsenteeism>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                ;
            Mapper.CreateMap<ExePlantWorkerAbsenteeismView, ExePlantWorkerAbsenteeismViewDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.OldValueEmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.OldValueStartDateAbsent, opt => opt.MapFrom(src => src.StartDateAbsent))
                .ForMember(dest => dest.OldValueEndDateAbsent, opt => opt.MapFrom(src => src.EndDateAbsent))
                .ForMember(dest => dest.OldValueShift, opt => opt.MapFrom(src => src.Shift))
                .ForMember(dest => dest.OldValueAbsentType, opt => opt.MapFrom(src => src.AbsentType))
                ;

            //Mapper.CreateMap<ExePlantProductionEntryDTO, ExePlantWorkerAbsenteeismDTO>()
            //    .ForMember(dest => dest.StartDateAbsent, opt => opt.MapFrom(m => m.StartDateAbsent.HasValue ? DateTime.Now : m.StartDateAbsent.Value.Date))
            //    .ForMember(dest => dest.EndDateAbsent, opt => opt.MapFrom(m => m.StartDateAbsent.HasValue ? DateTime.Now : m.StartDateAbsent.Value.Date))
            //    .ForMember(dest => dest.EndDateAbsent, opt => opt.MapFrom(m => m.StartDateAbsent.HasValue ? DateTime.Now : m.StartDateAbsent.Value.Date))
            //    .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(m => m.ProductionEntryCode.Trim('/')[1]));
            #endregion

            #region Reports

            #region Production Report By Process
            Mapper.CreateMap<ExeReportByProcess, ExeReportByProcessDTO>().IgnoreAllNonExisting();
            //.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeNumber.Substring(m.MstPlantEmpJobsDataAll.EmployeeNumber.Length - 2, 2) + " - " + m.MstPlantEmpJobsDataAll.EmployeeName));
            Mapper.CreateMap<ExeReportByProcessDTO, ExeReportByProcess>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<ExeReportByProcessView, ExeReportByProcessViewDTO>().IgnoreAllNonExisting();
            //.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeNumber.Substring(m.MstPlantEmpJobsDataAll.EmployeeNumber.Length - 2, 2) + " - " + m.MstPlantEmpJobsDataAll.EmployeeName));
            Mapper.CreateMap<ExeReportByProcessViewDTO, ExeReportByProcessView>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            #endregion

            #region Production Report by Group

            Mapper.CreateMap<ExeReportByGroup, ExeReportByGroupDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetExeReportByGroupDaily_Result, ExeReportByGroupViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetExeReportByGroupAnnualy_Result, ExeReportByGroupViewDTO>().IgnoreAllNonExisting();

            //.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeNumber.Substring(m.MstPlantEmpJobsDataAll.EmployeeNumber.Length - 2, 2) + " - " + m.MstPlantEmpJobsDataAll.EmployeeName));
            Mapper.CreateMap<ExeReportByGroupDTO, ExeReportByGroup>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString()))); ;
            Mapper.CreateMap<ExeReportByGroupsMonthly, ExeReportByGroupMonthlyDTO>().IgnoreAllNonExisting();
            //.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeNumber.Substring(m.MstPlantEmpJobsDataAll.EmployeeNumber.Length - 2, 2) + " - " + m.MstPlantEmpJobsDataAll.EmployeeName));
            Mapper.CreateMap<ExeReportByGroupMonthlyDTO, ExeReportByGroupsMonthly>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString()))); ;
            Mapper.CreateMap<ExeReportByGroupsWeekly, ExeReportByGroupWeeklyDTO>().IgnoreAllNonExisting();
            //.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeNumber.Substring(m.MstPlantEmpJobsDataAll.EmployeeNumber.Length - 2, 2) + " - " + m.MstPlantEmpJobsDataAll.EmployeeName));
            Mapper.CreateMap<ExeReportByGroupWeeklyDTO, ExeReportByGroupsWeekly>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString()))); ;

            #endregion

            #region Production Report By Status
            Mapper.CreateMap<ExeReportByStatusView, ExeReportByStatusDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByStatusWeeklyView, ExeReportByStatusDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByStatusMonthlyView, ExeReportByStatusDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByStatusWeeklyView, ExeReportByStatusWeeklyDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByStatusMonthlyView, ExeReportByStatusMonthlyDTO>().IgnoreAllNonExisting();

            #endregion

            #region Production Report Stock By Process
            Mapper.CreateMap<GetReportProdStockProcessView_Result, ExeReportProductionStockDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetReportProdStockProcessAllUnitView_Result, ExeReportProductionStockDTO>().IgnoreAllNonExisting();
            #endregion

            #endregion

            #endregion

            Mapper.CreateMap<PlanPlantAllocationDTO, PlanPlantAllocation>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));

            Mapper.CreateMap<MstClosingPayroll, MstClosingPayrollDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstClosingPayrollDTO, MstClosingPayroll>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExeProductionEntryPrintView, ExeProductionEntryPrintViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeProductionEntryPrintViewDTO, ExeProductionEntryPrintView>().IgnoreAllNonExisting();

            #region Production Entry

            Mapper.CreateMap<ExePlantProductionEntry, ExePlantProductionEntryDTO>().IgnoreAllNonExisting()
                //.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeNumber.Substring(m.MstPlantEmpJobsDataAll.EmployeeNumber.Length - 2, 2) + " - " + m.MstPlantEmpJobsDataAll.EmployeeName));
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.EmployeeNumber.Substring(m.EmployeeNumber.Length - 2, 2) + " - " + m.MstPlantEmpJobsDataAll.EmployeeName));
                //.ForMember(dest => dest.ProdTarget, opt => opt.MapFrom(src => Math.Round(Convert.ToDouble(src.ProdTarget.Value), 2)))
                //.ForMember(dest => dest.ProdActual, opt => opt.MapFrom(src => Math.Round(Convert.ToDouble(src.ProdActual.Value), 2)));

            Mapper.CreateMap<ExePlantProductionEntryDTO, ExePlantProductionEntry>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.UpdatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<ExePlantProductionEntry, ExePlantProductionEntryAllocationCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeName));

            #endregion

            #region Actual Work Hours Plant
            Mapper.CreateMap<ExePlantActualWorkHoursView, ExePlantActualWorkHoursDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExePlantActualWorkHoursDTO, ExeActualWorkHour>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeActualWorkHour, ExePlantActualWorkHoursDTO>().IgnoreAllNonExisting();
            #endregion

            #region Actual Work Hours TPO
            Mapper.CreateMap<ExePlantActualWorkHoursView, ExeTPOActualWorkHoursDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeTPOActualWorkHour, ExeTPOActualWorkHoursDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeTPOActualWorkHoursDTO, ExeTPOActualWorkHour>().IgnoreAllNonExisting();
            #endregion

            #region Worker AssignmentExePlantWorkerAssignmentDTO
            Mapper.CreateMap<ExePlantWorkerAssignment, ExePlantWorkerAssignmentDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeName))
                .ForMember(dest => dest.OldEmployeeID, opt => opt.MapFrom(m => m.EmployeeID))
                .ForMember(dest => dest.OldStartDate, opt => opt.MapFrom(m => m.StartDate))
                .ForMember(dest => dest.OldEndDate, opt => opt.MapFrom(m => m.EndDate))
                ;
            Mapper.CreateMap<ExePlantWorkerAssignmentDTO, ExePlantWorkerAssignment>().IgnoreAllNonExisting();
            Mapper.CreateMap<WorkerAssignmentRemovalDTO, WorkerAssignmentRemoval>().IgnoreAllNonExisting();
            Mapper.CreateMap<WorkerAssignmentRemoval, WorkerAssignmentRemovalDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExePlantWorkerAssignmentDTO, ExePlantWorkerAbsenteeismDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StartDateAbsent, opt => opt.MapFrom(m => m.StartDate))
                .ForMember(dest => dest.EndDateAbsent, opt => opt.MapFrom(m => m.EndDate))
                .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(m => m.SourceLocationCode))
                .ForMember(dest => dest.UnitCode, opt => opt.MapFrom(m => m.SourceUnitCode))
                .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(m => m.SourceGroupCode))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(m => m.CreatedBy))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(m => m.UpdatedBy))
                ;
            #endregion

            #region TPO Production Entry
            Mapper.CreateMap<ExeTPOProduction, ExeTPOProductionViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeTPOProductionView, ExeTPOProductionViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeTPOProductionViewDTO, ExeTPOProduction>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExeProductionEntryMinimumValue, ExeProductionEntryMinimumValueDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeProductionEntryMinimumValueDTO, ExeProductionEntryMinimumValue>().IgnoreAllNonExisting();
            #endregion

            #region TPO Production Entry Verification
            Mapper.CreateMap<ExeTPOProductionEntryVerificationView, ExeTPOProductionEntryVerificationViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeTPOProductionEntryVerification, ExeTPOProductionEntryVerificationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeTPOProductionEntryVerificationDTO, ExeTPOProductionEntryVerification>().IgnoreAllNonExisting();

            #endregion

            #region Plant Material Usages
            Mapper.CreateMap<ExeMaterialUsageView, ExePlantMaterialUsagesDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeMaterialUsage, ExePlantMaterialUsagesDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExePlantMaterialUsagesDTO, ExeMaterialUsage>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            #endregion

            #region Production Adjustment

            Mapper.CreateMap<ProductAdjustment, ProductAdjustmentDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ProductAdjustmentDTO, ProductAdjustment>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExeReportByGroupDTO, ProductAdjustmentDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ProductAdjustmentDTO, ExeReportByGroupDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExeReportByGroup, ExeReportByProcess>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByGroupDTO, ExeReportByProcess>().IgnoreAllNonExisting();

            #endregion

            #region Production Entry Verification
            Mapper.CreateMap<ExePlantProductionEntryVerificationView, ExePlantProductionEntryVerificationViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExePlantProductionEntryVerificationViewDTO, ExePlantProductionEntryVerification>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            Mapper.CreateMap<ExePlantProductionEntryVerification, ExePlantProductionEntryVerificationViewDTO>().IgnoreAllNonExisting()
            .ForMember(dest => dest.VerifySystem, opt => opt.MapFrom(m => m.VerifySystem == true ? 1 : 0));
            #endregion

            #region workerloadbalancing
            Mapper.CreateMap<ExePlantWorkerBalancingMulti, ExePlantWorkerBalancingViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExePlantWorkerBalancingViewDTO, ExePlantWorkerBalancingMulti>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExePlantWorkerBalancingSingle, ExePlantWorkerBalancingSingleViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExePlantWorkerBalancingSingleViewDTO, ExePlantWorkerBalancingSingle>().IgnoreAllNonExisting();
            #endregion

            #region Plant Wages
            #region Production Card
            Mapper.CreateMap<ProductionCard, ProductionCardDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeNumber.Substring(m.MstPlantEmpJobsDataAll.EmployeeNumber.Length - 2, 2) + " - " + m.MstPlantEmpJobsDataAll.EmployeeName))
                .ForMember(dest => dest.EmployeeNameOnly, opt => opt.MapFrom(m => m.MstPlantEmpJobsDataAll.EmployeeName));
            Mapper.CreateMap<ProductionCardDTO, ProductionCard>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.CreatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())))
                .ForMember(dest => dest.UpdatedDate, opt => opt.Condition(srs => string.IsNullOrEmpty(srs.DestinationValue.ToNullSafeString())));
            #endregion

            #region Eblek Release

            Mapper.CreateMap<ExeProductionEntryReleaseDTO, ExeProductionEntryRelease>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeProductionEntryRelease, ExeProductionEntryReleaseDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExePlantProductionEntryVerification, ExeProductionEntryReleaseDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionEntryCode,
                    opt => opt.MapFrom(src => src.ExeProductionEntryRelease.ProductionEntryCode))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.ExeProductionEntryRelease.Remark))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.ExeProductionEntryRelease.IsLocked))
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom(src => src.ExeProductionEntryRelease.CreatedDate))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.ExeProductionEntryRelease.CreatedBy))
                .ForMember(dest => dest.UpdatedDate,
                    opt => opt.MapFrom(src => src.ExeProductionEntryRelease.UpdatedDate))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.ExeProductionEntryRelease.UpdatedBy))
                .ForMember(dest => dest.ExePlantProductionEntryVerification, opt => opt.MapFrom(src => src));
            #endregion

            #region Production Card Approval
            Mapper.CreateMap<GetWagesProductionCardApprovalView_Result, WagesProductionCardApprovalCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<WagesProductionCardApprovalView, WagesProductionCardApprovalCompositeDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<WagesProductionCardApprovalDetailView, WagesProductionCardApprovalDetailViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<WagesProductionCardApprovalDetailViewGroup, WagesProductionCardApprovalDetailViewGroupDTO>()
                .IgnoreAllNonExisting();
            #endregion

            #region Eblek Release Approval
            Mapper.CreateMap<ExePlantProductionEntryVerification, EblekReleaseApprovalDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionEntryCode, opt => opt.MapFrom(src => src.ProductionEntryCode))
                .ForMember(dest => dest.EblekDate, opt => opt.MapFrom(src => src.ProductionDate))
                .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.GroupCode))
                .ForMember(dest => dest.BrandCode, opt => opt.MapFrom(src => src.BrandCode))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.ExeProductionEntryRelease.Remark))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.ExeProductionEntryRelease.IsLocked))
                ;

            Mapper.CreateMap<ExeProductionEntryReleaseTransactLogsView, EblekReleaseApprovalDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionEntryCode, opt => opt.MapFrom(src => src.ProductionEntryCode))
                .ForMember(dest => dest.EblekDate, opt => opt.MapFrom(src => src.EblekDate))
                .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.GroupCode))
                .ForMember(dest => dest.BrandCode, opt => opt.MapFrom(src => src.BrandCode))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.OldValueRemark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
                ;
            #endregion
            #endregion

            #region TPOFee
            Mapper.CreateMap<TPOFeeExeActualView, TPOFeeExeActualViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeApprovalView, TPOFeeExeActualViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeExeActualViewDTO, TPOFeeApprovalView>().IgnoreAllNonExisting();

            Mapper.CreateMap<TPOFeeExeActualView, TPOFeeExeAPOpenViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeExeAPOpenViewDTO, TPOFeeExeActualView>().IgnoreAllNonExisting();

            Mapper.CreateMap<TPOFeeHdr, TPOFeeHdrDTO>().IgnoreAllNonExisting();
            #endregion

            #region TPO Fee Production Daily
            Mapper.CreateMap<TPOFeeProductionDaily, TPOFeeProductionDailyDTO>().IgnoreAllNonExisting()
                //.ForMember(dest => dest.OutputBox, opt => opt.MapFrom(src => Math.Round(src.OutputBox.HasValue ? src.OutputBox.Value : 0, 2, MidpointRounding.AwayFromZero)))
                //.ForMember(dest => dest.OutputBox, opt => opt.MapFrom(src => String.Format("0:0.00", src.OutputBox)))
                .ForMember(dest => dest.OutputBoxS, opt => opt.MapFrom(src => String.Format(new System.Globalization.CultureInfo("en-GB"), "{0:N2}", src.OutputBox)))
                .ForMember(dest => dest.FeeDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.FeeDate))
                .ForMember(dest => dest.Hari, opt => opt.ResolveUsing<DayOfWeekToDayName>().FromMember(src => (int)src.FeeDate.DayOfWeek))
                .ForMember(dest => dest.JKN, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => Math.Round( src.JKN.Value,3,MidpointRounding.AwayFromZero)))
                .ForMember(dest => dest.JL1, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => Math.Round(src.JL1.Value, 3, MidpointRounding.AwayFromZero)))
                .ForMember(dest => dest.Jl2, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => Math.Round(src.Jl2.Value, 3, MidpointRounding.AwayFromZero)))
                .ForMember(dest => dest.Jl3, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => Math.Round(src.Jl3.Value, 3, MidpointRounding.AwayFromZero)))
                .ForMember(dest => dest.Jl4, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => Math.Round(src.Jl4.Value, 3, MidpointRounding.AwayFromZero)))
                .ForMember(dest => dest.JKNJam, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => src.JKNJam))
                .ForMember(dest => dest.JL1Jam, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => src.JL1Jam))
                .ForMember(dest => dest.JL2Jam, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => src.JL2Jam))
                .ForMember(dest => dest.JL3Jam, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => src.JL3Jam))
                .ForMember(dest => dest.JL4Jam, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => src.JL4Jam));

            #endregion

            #region TPO Fee Calculation
            Mapper.CreateMap<TPOFeeCalculation, TPOFeeCalculationDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Calculate, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => src.Calculate))
                .ForMember(dest => dest.OutputBiaya, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => src.OutputBiaya))
                .ForMember(dest => dest.OutputProduction, opt => opt.ResolveUsing<NullableDoubleToZero>().FromMember(src => src.OutputProduction));
            #endregion

            #region Tpo Fee Exe Plan 

            Mapper.CreateMap<TPOFeeHdrPlan, TpoFeeHdrPlanDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TpoFeeProductionDailyPlans, opt => opt.MapFrom(src => src.TPOFeeProductionDailyPlans))
                .ForMember(dest => dest.TpoFeeCalculationPlans, opt => opt.MapFrom(src => src.TPOFeeCalculationPlans));
            Mapper.CreateMap<TPOFeeProductionDailyPlan, TPOFeeProductionDailyPlanDto>();
            Mapper.CreateMap<TPOFeeCalculationPlan, TPOFeeCalculationPlanDto>().IgnoreAllNonExisting();

            #endregion
            #region TPO Fee Approval
            Mapper.CreateMap<TPOFeeApprovalView, TPOFeeApprovalViewDTO>().IgnoreAllNonExisting();
            #endregion
            #region TPO Fee GL Accrued

            Mapper.CreateMap<TPOFeeExeGLAccruedListView, TPOFeeExeGLAccruedViewListDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.LocationCode))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationName))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.BrandGroupCode))
                ;

            Mapper.CreateMap<GenerateP1TemplateGL_Result, GenerateP1TemplateGLDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GenerateP1TemplateAP_Result, GenerateP1TemplateAPDTO>().IgnoreAllNonExisting();
            #endregion

            #region TPO Fee AP Open
            Mapper.CreateMap<TPOFeeApprovalView, TPOFeeAPOpenDTO>().IgnoreAllNonExisting();
            #endregion

            Mapper.CreateMap<TPOGenerateP1TemplateView, TPOGenerateP1TemplateViewDTO>().IgnoreAllNonExisting();

            #region TPO Fee AP Open
            Mapper.CreateMap<TPOFeeApprovalView, TPOFeeAPCloseDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeApprovalView, TPOFeeExeAPOpenViewDTO>().IgnoreAllNonExisting();
            #endregion

            #region Report Plan

            Mapper.CreateMap<MstTableauReport, MstTableauReportDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstTableauReportDto, MstTableauReport>().IgnoreAllNonExisting();

            #endregion

            Mapper.CreateMap<TPOFeeSettingCalculation, TPOFeeSettingCalculationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeSettingCalculationDTO, TPOFeeSettingCalculation>().IgnoreAllNonExisting();

            #region Reports Daily Production Achievement

            Mapper.CreateMap<ExeReportDailyProductionAchievementView, ExeReportDailyProductionAchievementViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportDailyProductionAchievementViewDTO, ExeReportDailyProductionAchievementView>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExeReportDailyProductionAchievementView, DataDailyProductionAchievmentDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<GetExeReportDailyProductionAchievement_Result, GetExeReportDailyProductionAchievementDTO>().IgnoreAllNonExisting();
           #endregion

            #region Absent Report   
            Mapper.CreateMap<GetWagesReportAbsentDialy_Result, GetWagesReportAbsentViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetWagesReportAbsentDialyMore_Result, GetWagesReportAbsentViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<WAGES_ABSENT_REPORT_BYGROUP_Result, GetWagesReportAbsentViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetWagesReportAbsentDetailDialy_Result, GetWagesReportAbsentGroupViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<WAGES_ABSENT_REPORT_BYEMPLOYEE_Result, GetWagesReportAbsentGroupViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetWagesReportAbsentDialyMore_Result, ProductionCardDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetProcessFromProdCard_Result, ProductionCardDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<WAGES_ABSENT_REPORT_BYEMPLOYEE_DETAIL_Result, WagesReportDetailEmployeeDTO>().IgnoreAllNonExisting();
            #endregion

            #region EMS Source Data
            Mapper.CreateMap<GetEMSSourceData_Result, ExeEMSSourceDataDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeEMSSourceDataDTO, GetEMSSourceData_Result>().IgnoreAllNonExisting();
            #endregion

            #region Wages Available Position Number
            Mapper.CreateMap<AvailablePositionNumberView_Result, GetWagesReportAvailablePositionNumberViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetWagesReportAvailablePositionNumberViewDTO, AvailablePositionNumberView_Result>().IgnoreAllNonExisting();

            Mapper.CreateMap<PlanPlantGroupShiftDTO, AvailabelPositionNumberGroup>().IgnoreAllNonExisting();
            Mapper.CreateMap<AvailabelPositionNumberGroup, PlanPlantGroupShiftDTO>().IgnoreAllNonExisting();
            #endregion

            Mapper.CreateMap<MstADTemp, MstADTempDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstADTempDto, MstADTemp>().IgnoreAllNonExisting();

        }
    }

    public class MappingReadyToUse : ValueResolver<ICollection<MntcInventory>, int?>
    {
        protected override int? ResolveCore(ICollection<MntcInventory> source)
        {
            return
                source.Where(p => p.ItemStatus == Enums.ItemStatus.ReadyToUse.ToString())
                    .Select(c => c.EndingStock).FirstOrDefault();
        }
    }

    public class MappingOnUse : ValueResolver<ICollection<MntcInventory>, int?>
    {
        protected override int? ResolveCore(ICollection<MntcInventory> source)
        {
            return
                source.Where(p => p.ItemStatus == Enums.ItemStatus.OnUsed.ToString())
                    .Select(c => c.EndingStock).FirstOrDefault();
        }
    }

    public class MappingOnRepair : ValueResolver<ICollection<MntcInventory>, int?>
    {
        protected override int? ResolveCore(ICollection<MntcInventory> source)
        {
            return
                source.Where(p => p.ItemStatus == Enums.ItemStatus.OnRepair.ToString())
                    .Select(c => c.EndingStock).FirstOrDefault();
        }
    }
    public class StringToBoolean : ValueResolver<bool?, bool>
    {
        protected override bool ResolveCore(bool? source)
        {
            if (source == null)
                return false;

            return (bool)source;
        }
    }


}
