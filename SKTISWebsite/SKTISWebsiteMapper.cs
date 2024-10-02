using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;
using HMS.SKTIS.AutoMapperExtensions;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Core;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using SKTISWebsite.Models.Account;
using SKTISWebsite.Models.EquipmentFulfillment;
using SKTISWebsite.Models.EquipmentReceive;
using SKTISWebsite.Models.EquipmentRepairPlant;
using SKTISWebsite.Models.EquipmentRepairTPO;
using SKTISWebsite.Models.ExePlantProductionEntry;
using SKTISWebsite.Models.ExePlantProductionEntryVerification;
using SKTISWebsite.Models.ExeProductAdjustment;
using SKTISWebsite.Models.ExeReportDailyProductionAchievement;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.MaintenanceEquipmentQualityInspection;
using SKTISWebsite.Models.MaintenanceExecutionInventory;
using SKTISWebsite.Models.MaintenanceExecutionItemConversion;
using SKTISWebsite.Models.MaintenanceItemDisposal;
using SKTISWebsite.Models.MasterBrand;
using SKTISWebsite.Models.MasterBrandGroupMaterial;
using SKTISWebsite.Models.MasterBrandPackageMapping;
using SKTISWebsite.Models.MasterGenBrandGroup;
using SKTISWebsite.Models.MasterGenBrandPackageItem;
using SKTISWebsite.Models.MasterGenBrandPackageMapping;
using SKTISWebsite.Models.MasterGenClosingPayroll;
using SKTISWebsite.Models.MasterGenHoliday;
using SKTISWebsite.Models.MasterGenList;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.MasterGenProcess;
using SKTISWebsite.Models.MasterGenProccessSetting;
using SKTISWebsite.Models.MasterGenStandardHour;
using SKTISWebsite.Models.MasterGenWeek;
using SKTISWebsite.Models.MasterMaintenanceConvert;
using SKTISWebsite.Models.MasterMntcItem;
using SKTISWebsite.Models.MasterMntcItemLocation;
using SKTISWebsite.Models.MasterPlantAbsentType;
using SKTISWebsite.Models.MasterPlantProductionGroup;
using SKTISWebsite.Models.MasterTPOProductionGroup;
using SKTISWebsite.Models.MstPlantEmployeeJobsData;
using SKTISWebsite.Models.MstUnit;
using SKTISWebsite.Models.MasterTOPInfo;
using SKTISWebsite.Models.MstTPOFeeRate;
using SKTISWebsite.Models.MstTPOPackage;
using SKTISWebsite.Models.EquipmentRequest;
using SKTISWebsite.Models.EquipmentTransfer;
using SKTISWebsite.Models.PlanningReportSummaryProcessTargets;
using SKTISWebsite.Models.PlanningTPOTPK;
using SKTISWebsite.Models.PlanningPlantIndividualCapacity;
using SKTISWebsite.Models.PlanningReportProductionTarget;
using SKTISWebsite.Models.PlantGroupShift;
using SKTISWebsite.Models.Home;
using SKTISWebsite.Models.News;
using SKTISWebsite.Models.PlanningWPP;
using SKTISWebsite.Models.PlanningPlantTPU;
using SKTISWebsite.Models.MaintenanceReportEquipmentStock;
using SKTISWebsite.Models.MaintenanceReportEquipmentRequirement;
using SKTISWebsite.Models.PlanningPlantTPK;
using SKTISWebsite.Models.MaintenanceExecutionInventoryAdjustment;
using SKTISWebsite.Models.PlanTmpWeeklyProductionPlanning;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using SKTISWebsite.Models.ProductionCard;
using SKTISWebsite.Models.ReportPlan;
using SKTISWebsite.Models.ReportTpo;
using SKTISWebsite.Models.TPOFeeAPOpen;
using SKTISWebsite.Models.TPOFeeExeAPOpen;
using SKTISWebsite.Models.TPOFeeApproval;
using SKTISWebsite.Models.TPOFeeExeActual;
using SKTISWebsite.Models.TPOFeeExePlan;
using SKTISWebsite.Models.TPOFeeExePlanDetail;
using SKTISWebsite.Models.TPOFeeSettingCalculation;
using SKTISWebsite.Models.UtilTransactionLog;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using SKTISWebsite.Models.ExePlantWorkerAbsenteeism;
using SKTISWebsite.Models.ExePlantActualWorkHours;
using SKTISWebsite.Models.ExePlantWorkerAssignment;
using SKTISWebsite.Models.ExeTPOProductionEntry;
using SKTISWebsite.Models.ExePlantMaterialUsages;
using SKTISWebsite.Models.ExeTPOProductionEntryVerification;
using SKTISWebsite.Models.ExeOthersProductionEntryPrint;
using SKTISWebsite.Models.ExePlantWorkerBalancing;
using SKTISWebsite.Models.WagesEblekRelease;
using SKTISWebsite.Models.WagesProductionCardApproval;
using SKTISWebsite.Models.WagesProductionCardApprovalDetail;
using SKTISWebsite.Models.EblekReleaseApproval;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using SKTISWebsite.Models.TPOFeeExeGLAccrued;
using SKTISWebsite.Models.ExeReportByGroup;
using SKTISWebsite.Models.TPOFeeAPClose;
using SKTISWebsite.Models.ExeReportByStatus;
using SKTISWebsite.Models.ExeReportByProcess;
using SKTISWebsite.Models.ExeReportProductionStock;
using SKTISWebsite.Models.ExeTPOActualWorkHours;
using SKTISWebsite.Models.WagesReportAbsents;
using SKTISWebsite.Models.TPOFeeReportsPackage;
using SKTISWebsite.Models.UtilSecurityRoles;
using SKTISWebsite.Models.ExeEMSSourceData;
using SKTISWebsite.Models.UtilSecurityRules;
using SKTISWebsite.Models.UtilWorkflowFlow;
using SKTISWebsite.Models.UtilSecurityFunctions;
using SKTISWebsite.Models.UtilSecurityResponsibilities;
using SKTISWebsite.Models.TPOGenerateP1TemplateView;
using SKTISWebsite.Models.TPOFeeReportsSummary;
using SKTISWebsite.Models.WagesReportAvailablePositionNumber;
using SKTISWebsite.Models.WagesReportSummaryProductionCard;
using SKTISWebsite.Models.TPOFeeReportsProduction;
using SKTISWebsite.Models.UtilSecurityDelegations;
using SKTISWebsite.Models.GenerateP1TemplateGL;
using SKTISWebsite.Models.GenerateP1TemplateAP;

namespace SKTISWebsite
{
    public static class SKTISWebsiteMapper
    {
        public static void Initialize()
        {
            #region NewsInfo
            Mapper.CreateMap<SliderNewsInfoDTO, SliderNewsInfoViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Title, opt => opt.ResolveUsing<SliderNewsInfoTitleResolver>())
                .ForMember(dest => dest.ShortDescription, opt => opt.ResolveUsing<SliderNewsInfoShortDescriptionResolver>());

            Mapper.CreateMap<InformatipsNewsInfoDTO, InformatipsNewsInfoViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Title, opt => opt.ResolveUsing<InformatipsNewsInfoTitleResolver>())
                .ForMember(dest => dest.ShortDescription, opt => opt.ResolveUsing<InformatipsNewsInfoShortDescriptionResolver>());

            Mapper.CreateMap<NewsInfoCompositeDTO, NewsInfoCompositeViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Title, opt => opt.ResolveUsing<NewsInfoCompositeTitleResolver>())
                .ForMember(dest => dest.ShortDescription,
                    opt => opt.ResolveUsing<NewsInfoCompositeShortDescriptionResolver>());

            Mapper.CreateMap<NewsInfoDTO, NewsInfoDetailsViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Title, opt => opt.ResolveUsing<NewsInfoDetailsTitleResolver>())
                .ForMember(dest => dest.ShortDescription, opt => opt.ResolveUsing<NewsInfoDetailsShortDescriptionResolver>())
                .ForMember(dest => dest.Content, opt => opt.ResolveUsing<NewsInfoDetailsContentResolver>());

            Mapper.CreateMap<NewsInfoDTO, NewsInfoViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<NewsInfoViewModel, NewsInfoDTO>().IgnoreAllNonExisting();
            #endregion

            #region Master Data

            #region General

            #region General List

            Mapper.CreateMap<MstGeneralListCompositeDTO, MasterGenListViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterGenListViewModel, MstGeneralListCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstGeneralListCompositeDTO, SelectListItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.ListDetail))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.ListDetail));
            Mapper.CreateMap<MstGeneralListDTO, MasterGenListViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<MasterGenListViewModel, MstGeneralListDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));

            #endregion

            #region Location

            Mapper.CreateMap<MstGenLocationDTO, MstGenLocationViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstGenLocationViewModel, MstGenLocationDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<LocationInfoDTO, SelectListItem>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenLocationDTO, LocationLookupList>().IgnoreAllNonExisting();

            #endregion

            #region Holiday
            Mapper.CreateMap<MasterGenHolidayViewModel, MstHolidayDTO>().IgnoreAllNonExisting()
              .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
              .ForMember(dest => dest.HolidayDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.HolidayDate));
            Mapper.CreateMap<MstHolidayDTO, MasterGenHolidayViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.HolidayDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.HolidayDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #region Standard Hours
            Mapper.CreateMap<MstGenStandardHourDTO, MasterGenStandardHourViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterGenStandardHourViewModel, MstGenStandardHourDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #region Brand Group
            Mapper.CreateMap<MstGenBrandGroupDTO, MasterGenBrandGroupViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterGenBrandGroupViewModel, MstGenBrandGroupDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));

            #endregion

            #region Process

            Mapper.CreateMap<MstGenProcessDTO, MasterGenProcessViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstGenProcessDTO, MasterGenProcessIdentifier>().IgnoreAllNonExisting();
            Mapper.CreateMap<MasterGenProcessViewModel, MstGenProcessDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstGenProcessDTO, SelectListItem>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.ProcessGroup))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.ProcessGroup));
            #endregion

            #region Process Setting

            Mapper.CreateMap<MstGenProcessSettingDTO, MstGenProccessSettingViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstGenProccessSettingViewModel, MstGenProcessSettingDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstGenProcessSettingsLocationCompositeDTO, MstGenProcessSettingLocationViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenProcessSettingLocationDTO, MstGenProccessSettingLocationViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenProccessSettingLocationViewModel, MstGenProcessSettingLocationDTO>().IgnoreAllNonExisting();
            #endregion

            #region Brand Package Item
            Mapper.CreateMap<MstGenBrandPackageItemDTO, MasterGenBrandPackageItemViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterGenBrandPackageItemViewModel, MstGenBrandPackageItemDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstGenBrandGroupDTO, BrandCodeSelectItem>().IgnoreAllNonExisting()
                         .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.BrandGroupCode))
                         .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.BrandGroupCode));
            #endregion

            #region Brand Package Mapping
            Mapper.CreateMap<MstGenBrandPkgMappingDTO, BrandPkgMappingViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MappingValue, opt => opt.ResolveUsing<FloatToString3DecimalCommaSeparatorResolver>().FromMember(src => src.MappingValue));
            Mapper.CreateMap<BrandPkgMappingViewModel, MstGenBrandPkgMappingDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MappingValue, opt => opt.ResolveUsing<StringToFloatResolver>().FromMember(src => src.MappingValue));
            #endregion

            #endregion

            #region Maintenance

            #region Item
            Mapper.CreateMap<MstMntcItemCompositeDTO, MasterMntcItemViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterMntcItemViewModel, MstMntcItemCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstMntcItemCompositeDTO, SelectListItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.ItemCode))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.ItemCode));
            #endregion

            #region Item Location
            Mapper.CreateMap<MstMntcItemLocationDTO, MstMntcItemLocationViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstMntcItemLocationViewModel, MstMntcItemLocationDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstMntcItemCompositeDTO, MstMntcItemDescription>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenLocationDTO, SelectListItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.LocationCode))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.LocationName));
            Mapper.CreateMap<MstMntcItemLocationDTO, ItemLocationLookupList>().IgnoreAllNonExisting();
            #endregion

            #region Inventory

            Mapper.CreateMap<InventoryByStatusViewDTO, EquipmentRequestViewModel>().IgnoreAllNonExisting();

            #endregion

            #endregion

            #region TPO

            Mapper.CreateMap<MstTPOInfoDTO, MasterTPOInfoViewModels>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.Established, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.Established));
            Mapper.CreateMap<MasterTPOInfoViewModels, MstTPOInfoDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.Established, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.Established));
            #endregion

            #region Master Brand

            Mapper.CreateMap<BrandDTO, GetMasterBrandViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.EffectiveDate,
                    opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EffectiveDate))
                .ForMember(dest => dest.ExpiredDate,
                    opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ExpiredDate));
            Mapper.CreateMap<GetMasterBrandViewModel, BrandDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.EffectiveDate,
                    opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EffectiveDate))
                .ForMember(dest => dest.ExpiredDate, opt =>
                    opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.ExpiredDate));
            Mapper.CreateMap<BrandDTO, BrandCodeModel>().IgnoreAllNonExisting();
            #endregion

            #region Master Brand Group Material

            Mapper.CreateMap<BrandGroupMaterialDTO, MasterBrandGroupMaterialData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.OldMaterialCode, opt => opt.MapFrom(src => src.MaterialCode))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterBrandGroupMaterialData, BrandGroupMaterialDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.OldMaterialCode, opt => opt.MapFrom(src => src.OldMaterialCode))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));

            #endregion

            #region Master Plant Unit
            Mapper.CreateMap<MstPlantUnitCompositeDTO, MstPlantUnitViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<MstPlantUnitViewModel, MstPlantUnitDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<MstPlantUnitDTO, MstPlantUnitViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #region Master TPO Package

            Mapper.CreateMap<MstTPOPackageDTO, MstTPOPackageViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.EffectiveDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EffectiveDate))
                .ForMember(dest => dest.ExpiredDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ExpiredDate))
                .ForMember(dest => dest.EffectiveDateOld, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EffectiveDateOld));
            Mapper.CreateMap<MstTPOPackageViewModel, MstTPOPackageDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.EffectiveDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EffectiveDate))
                .ForMember(dest => dest.ExpiredDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.ExpiredDate))
                .ForMember(dest => dest.EffectiveDateOld, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EffectiveDateOld));

            Mapper.CreateMap<MstGenLocationDTO, MasterGenLocationDesc>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenBrandGroupDTO, SelectListItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.BrandGroupCode))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.BrandGroupCode));
            Mapper.CreateMap<MstTPOInfoDTO, SelectListItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.LocationCode))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.LocationCode));
            #endregion

            #region Master TPO Fee Rate
            Mapper.CreateMap<MstTPOFeeRateDTO, MstTPOFeeRateViewModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
               .ForMember(dest => dest.EffectiveDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EffectiveDate))
               .ForMember(dest => dest.PreviousEffectiveDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.PreviousEffectiveDate))
               .ForMember(dest => dest.ExpiredDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ExpiredDate));
            Mapper.CreateMap<MstTPOFeeRateViewModel, MstTPOFeeRateDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.EffectiveDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EffectiveDate))
                .ForMember(dest => dest.PreviousEffectiveDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.PreviousEffectiveDate))
                .ForMember(dest => dest.ExpiredDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.ExpiredDate));
            #endregion

            #region Master Brand Package Mapping

            Mapper.CreateMap<MstGenBrandGroupDTO, MasterBrandPackageMappingIndexViewModel>().IgnoreAllNonExisting();

            #endregion

            #region MstEmployeeJobsData
            Mapper.CreateMap<MstEmployeeJobsDataActiveCompositeDTO, MstEmployeeJobsDataViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.JoinDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.JoinDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #region Master Plant Prduction Group

            Mapper.CreateMap<MstPlantProductionGroupDTO, MstPlantProductionGroupViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstPlantProductionGroupCompositeDTO, MstPlantProductionGroupViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MstPlantProductionGroupViewModel, MstPlantProductionGroupDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<MstPlantProductionGroupDTO, SelectListItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.GroupCode))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.GroupCode));

            Mapper.CreateMap<MstEmployeeJobsDataActiveDTO, MandorsLookupList>().IgnoreAllNonExisting();
            #endregion

            #region Master TPO Production Group

            Mapper.CreateMap<MstTPOProductionGroupDTO, MasterTPOProductionGroupViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterTPOProductionGroupViewModel, MstTPOProductionGroupDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterTPOProductionGroupViewModel, TPOTPKDTO>().IgnoreAllNonExisting();
            #endregion

            #region Master Plant Absent Type
            Mapper.CreateMap<MstPlantAbsentTypeDTO, MstPlantAbsentTypeViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.OldAbsentType, opt => opt.MapFrom(src => src.OldAbsentType));
            Mapper.CreateMap<MstPlantAbsentTypeViewModel, MstPlantAbsentTypeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.OldAbsentType, opt => opt.MapFrom(src => src.OldAbsentType));

            Mapper.CreateMap<MstPlantAbsentTypeDTO, AbsentTypeLookupList>().IgnoreAllNonExisting();
            #endregion

            #region Master Maintenance Convert
            Mapper.CreateMap<MstMntcConvertDTO, MasterMntcConvertViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MasterMntcConvertViewModel, MstMntcConvertDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            Mapper.CreateMap<MstGenWeekDTO, MasterGenWeekViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StartDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EndDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));

            #endregion

            #region Maintenance

            #region Maintainance Execution

            #region Equipment Request

            Mapper.CreateMap<MntcEquipmentRequestCompositeDTO, EquipmentRequestViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ReadyToUse, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.ReadyToUse))
                .ForMember(dest => dest.OnUsed, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.OnUsed))
                .ForMember(dest => dest.OnRepair, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.OnRepair))
                .ForMember(dest => dest.RequestDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.RequestDate))
                .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(src => src.TotalQty));
            Mapper.CreateMap<EquipmentRequestViewModel, EquipmentRequestDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RequestDate,
                    opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.RequestDate))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.TotalQuantity));

            Mapper.CreateMap<EquipmentRequestDTO, EquipmentRequestViewModel>().IgnoreAllNonExisting();

            #endregion

            #region ItemConversion
            Mapper.CreateMap<MaintenanceExecutionItemConversionDTO, MaintenanceExecutionItemConversionViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransactionDate));
            //.ForMember(dest => dest.DestinationStock, opt => opt.MapFrom(m => m.QtyGood));
            Mapper.CreateMap<MaintenanceExecutionItemConversionViewModel, MaintenanceExecutionItemConversionDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TransactionDate));
            //.ForMember(dest => dest.QtyGood, opt => opt.MapFrom(m => m.DestinationStock));
            Mapper.CreateMap<MstMntcConvertDTO, MasterItemConversionComposite>().IgnoreAllNonExisting();

            #endregion

            #region Inventory Adjustment

            Mapper
                .CreateMap<MaintenanceExecutionInventoryAdjustmentDTO, MaintenanceExecutionInventoryAdjustmentViewModel>()
                .ForMember(dest => dest.AdjustmentDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.AdjustmentDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate))
                .IgnoreAllNonExisting();
            Mapper
                .CreateMap<MaintenanceExecutionInventoryAdjustmentViewModel, MaintenanceExecutionInventoryAdjustmentDTO>()
                .ForMember(dest => dest.AdjustmentDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.AdjustmentDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.UpdatedDate))
                .IgnoreAllNonExisting();
            #endregion

            #endregion

            #region Item Disposal

            Mapper.CreateMap<MntcEquipmentItemDisposalCompositeDTO, GetItemDisposalViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<GetItemDisposalViewModel, MntcEquipmentItemDisposalCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EndingStock, opt => opt.MapFrom(src => src.CalculatedEndingStock))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.UpdatedDate));

            #endregion

            #region Equipment Transfer
            Mapper.CreateMap<EquipmentTransferCompositeDTO, EquipmentTransferViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransferDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransferDate))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QtyTransfer))
                .ForMember(dest => dest.DeliveryNote, opt => opt.MapFrom(src => src.TransferNote))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<EquipmentTransferViewModel, EquipmentTransferDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransferDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TransferDate))
                .ForMember(dest => dest.QtyTransfer, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.TransferNote, opt => opt.MapFrom(src => src.DeliveryNote))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<EquipmentTransferDTO, EquipmentTransferViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransferDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransferDate))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QtyTransfer))
                .ForMember(dest => dest.DeliveryNote, opt => opt.MapFrom(src => src.TransferNote))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));

            #endregion

            #region Equipment Receive
            Mapper.CreateMap<EquipmentReceiveCompositeDTO, EquipmentReceiveViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransferDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransferDate))
                .ForMember(dest => dest.ReceiveDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ReceiveDate))
                .ForMember(dest => dest.DeliveryNote, opt => opt.MapFrom(src => src.TransferNote))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<EquipmentReceiveViewModel, EquipmentReceiveDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransferDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TransferDate))
                .ForMember(dest => dest.ReceiveDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.ReceiveDate))
                .ForMember(dest => dest.TransferNote, opt => opt.MapFrom(src => src.DeliveryNote))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<EquipmentReceiveDTO, EquipmentReceiveViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransferDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransferDate))
                .ForMember(dest => dest.ReceiveDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ReceiveDate))
                .ForMember(dest => dest.DeliveryNote, opt => opt.MapFrom(src => src.TransferNote))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));

            #endregion

            #region Equipment Fulfillment

            Mapper.CreateMap<EquipmentFulfillmentCompositeDTO, EquipmentRequestDTO>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.RequestedQuantity));

            Mapper.CreateMap<EquipmentFulfillmentCompositeDTO, MntcRequestToLocationDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.LocationCode, opt => opt.MapFrom(src => src.LocationCodeForReqToLocation))
                .ForMember(dest => dest.QtyFromLocation, opt => opt.MapFrom(src => src.Quantity));

            Mapper.CreateMap<EquipmentFulfillmentCompositeDTO, EquipmentFulfillmentViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.FulfillmentDate,
                    opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.FulFillmentDate))
                .ForMember(dest => dest.ApprovedQuantity, opt => opt.MapFrom(src => src.ApprovedQty))
                .ForMember(dest => dest.RequestToOthersQuantity, opt => opt.MapFrom(src => src.RequestToQty))
                .ForMember(dest => dest.PurchaseQty, opt => opt.MapFrom(src => src.PurchaseQuantity));

            Mapper.CreateMap<EquipmentFulfillmentViewModel, EquipmentFulfillmentCompositeDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PurchaseQuantity, opt => opt.MapFrom(src => src.PurchaseQty))
                .ForMember(dest => dest.ApprovedQty, opt => opt.MapFrom(src => src.ApprovedQuantity))
                .ForMember(dest => dest.RequestToQty, opt => opt.MapFrom(src => src.RequestToOthersQuantity))
                .ForMember(dest => dest.FulFillmentDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.FulfillmentDate));

            // ------------------ hakim
            Mapper.CreateMap<MntcFulfillmentViewDTO, EquipmentFulfillmentViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.FulfillmentDate,
                    opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.FulFillmentDate))
                .ForMember(dest => dest.ApprovedQuantity, opt => opt.MapFrom(src => src.ApprovedQty))
                .ForMember(dest => dest.RequestToOthersQuantity, opt => opt.MapFrom(src => src.RequestToQty))
                .ForMember(dest => dest.PurchaseQty, opt => opt.MapFrom(src => src.PurchaseQuantity));

            Mapper.CreateMap<EquipmentFulfillmentViewModel, MntcFulfillmentViewDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PurchaseQuantity, opt => opt.MapFrom(src => src.PurchaseQty))
                .ForMember(dest => dest.ApprovedQty, opt => opt.MapFrom(src => src.ApprovedQuantity))
                .ForMember(dest => dest.RequestToQty, opt => opt.MapFrom(src => src.RequestToOthersQuantity))
                .ForMember(dest => dest.FulFillmentDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.FulfillmentDate));
            // ------------------ hakim
            #endregion

            #region Maintenance Equipment Quaity Inspection

            Mapper.CreateMap<MaintenanceExecutionQualityInspectionDTO, MaintenanceEquipmentQualityInspectionViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<MaintenanceEquipmentQualityInspectionViewModel, MaintenanceExecutionQualityInspectionDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #region Equipment Repair
            Mapper.CreateMap<EquipmentRepairDTO, EquipmentRepairViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<EquipmentRepairViewModel, EquipmentRepairDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<EquipmentRepairTPODTO, EquipmentRepairTPOViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<EquipmentRepairTPOViewModel, EquipmentRepairTPODTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.UpdatedDate));

            Mapper.CreateMap<MntcRepairItemUsageDTO, SparepartDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(src => src.ItemCodeDestination))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.QtyUsage));

            #endregion

            #region Equipment Stock Report
            Mapper.CreateMap<MaintenanceEquipmentStockReportDTO, MaintenanceEquipmentStockReportViewModel>().IgnoreAllNonExisting();
            #endregion

            #region Equipment Requirement Report
            Mapper.CreateMap<MaintenanceEquipmentRequirementReportDTO, MaintenanceEquipmentRequirementViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<EquipmentRequirementDTO, EquipmentRequirementViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<EquipmentRequirementViewModel, EquipmentRequirementDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<EquipmentRequirementSummaryItemDTO, EquipmentRequirementItemViewModel>().IgnoreAllNonExisting();
            #endregion
            #endregion

            #region Plan

            #region Plant Group Shift

            Mapper.CreateMap<PlanPlantGroupShiftViewModel, PlanPlantGroupShiftDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<PlanPlantGroupShiftDTO, PlanPlantGroupShiftViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #endregion

            #region Planning
            #region WPP

            Mapper.CreateMap<PlanWeeklyProductionPlanningDTO, WPP13WeekModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate));
            Mapper.CreateMap<WPP13WeekModel, PlanWeeklyProductionPlanningDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));

            #endregion

            #region Plant Individual Capacity
            Mapper.CreateMap<PlanningPlantIndividualCapacityWorkHourDTO, PlanningPlantIndividualCapacityViewModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
            .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.CreatedDate));
            Mapper.CreateMap<PlanningPlantIndividualCapacityViewModel, PlanningPlantIndividualCapacityWorkHourDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.CreatedDate));

            Mapper
                .CreateMap
                <PlanningPlantIndividualCapacityByReferenceDTO, PlanningPlantIndividualCapacityByReferenceViewModel>()
                .IgnoreAllNonExisting()
                //.ForMember(dest => dest.LatestValue, opt => opt.ResolveUsing<IndividualCapacityByReferenceWorkHours>().FromMember(src => src))
                .ForMember(dest => dest.HoursCapacity, opt => opt.ResolveUsing<IndividualCapacityByReferenceWorkHours>().FromMember(src => src));
            Mapper
                .CreateMap
                <PlanningPlantIndividualCapacityByReferenceViewModel, PlanningPlantIndividualCapacityByReferenceDTO>()
                .IgnoreAllNonExisting();
            Mapper
                .CreateMap
                <PlanningPlantIndividualCapacityByReferenceViewModel, PlanningPlantIndividualCapacityWorkHourDTO>()
                .IgnoreAllNonExisting();
            Mapper
                .CreateMap
                <PlanningPlantIndividualCapacityWorkHourDTO, PlanningPlantIndividualCapacityByReferenceViewModel>()
                .IgnoreAllNonExisting();

            #endregion

            #region Plant

            #region TPU

            Mapper.CreateMap<PlanTPUCompositeDTO, PlanTPUViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.ProductionStartDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ProductionStartDate));

            Mapper.CreateMap<PlanTPUDTO, PlanTPUViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.ProductionStartDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ProductionStartDate));

            Mapper.CreateMap<PlanTPUViewModel, PlanTPUDTO>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.ProductionStartDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.ProductionStartDate));

            Mapper.CreateMap<DateStateDTO, SKTISWebsite.Models.PlanningPlantTPU.DateStateModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<PlanTPUStatusDTO, TPUStatusModel>().IgnoreAllNonExisting();
            #endregion

            #region TPK

            Mapper.CreateMap<PlantTPKCompositeDTO, PlantTPKModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TPKPlantStartProductionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TPKPlantStartProductionDate))
                .ForMember(dest => dest.HistoricalCapacityWorker1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker1))
                .ForMember(dest => dest.HistoricalCapacityWorker2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker2))
                .ForMember(dest => dest.HistoricalCapacityWorker3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker3))
                .ForMember(dest => dest.HistoricalCapacityWorker4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker4))
                .ForMember(dest => dest.HistoricalCapacityWorker5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker5))
                .ForMember(dest => dest.HistoricalCapacityWorker6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker6))
                .ForMember(dest => dest.HistoricalCapacityWorker7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker7))
                .ForMember(dest => dest.HistoricalCapacityGroup1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup1))
                .ForMember(dest => dest.HistoricalCapacityGroup2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup2))
                .ForMember(dest => dest.HistoricalCapacityGroup3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup3))
                .ForMember(dest => dest.HistoricalCapacityGroup4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup4))
                .ForMember(dest => dest.HistoricalCapacityGroup5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup5))
                .ForMember(dest => dest.HistoricalCapacityGroup6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup6))
                .ForMember(dest => dest.HistoricalCapacityGroup7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup7))
                .ForMember(dest => dest.WIP1, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP1))
                .ForMember(dest => dest.WIP2, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP2))
                .ForMember(dest => dest.WIP3, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP3))
                .ForMember(dest => dest.WIP4, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP4))
                .ForMember(dest => dest.WIP5, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP5))
                .ForMember(dest => dest.WIP6, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP6))
                .ForMember(dest => dest.WIP7, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP7))
                .ForMember(dest => dest.TargetSystem1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem1))
                .ForMember(dest => dest.TargetSystem2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem2))
                .ForMember(dest => dest.TargetSystem3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem3))
                .ForMember(dest => dest.TargetSystem4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem4))
                .ForMember(dest => dest.TargetSystem5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem5))
                .ForMember(dest => dest.TargetSystem6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem6))
                .ForMember(dest => dest.TargetSystem7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem7))
                .ForMember(dest => dest.TargetManual1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual1))
                .ForMember(dest => dest.TargetManual2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual2))
                .ForMember(dest => dest.TargetManual3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual3))
                .ForMember(dest => dest.TargetManual4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual4))
                .ForMember(dest => dest.TargetManual5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual5))
                .ForMember(dest => dest.TargetManual6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual6))
                .ForMember(dest => dest.TargetManual7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual7))
                .ForMember(dest => dest.PercentAttendance1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance1))
                .ForMember(dest => dest.PercentAttendance2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance2))
                .ForMember(dest => dest.PercentAttendance3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance3))
                .ForMember(dest => dest.PercentAttendance4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance4))
                .ForMember(dest => dest.PercentAttendance5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance5))
                .ForMember(dest => dest.PercentAttendance6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance6))
                .ForMember(dest => dest.PercentAttendance7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance7));

            Mapper.CreateMap<PlantTPKModel, PlantTPKDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TPKPlantStartProductionDate,
                    opt =>
                        opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TPKPlantStartProductionDate));

            Mapper.CreateMap<PlantTPKDTO, PlantTPKModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TPKPlantStartProductionDate,
                    opt =>
                        opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TPKPlantStartProductionDate));

            Mapper.CreateMap<PlantTPKCompositeDTO, PlantTPKExcelModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TPKPlantStartProductionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TPKPlantStartProductionDate))
                .ForMember(dest => dest.HistoricalCapacityWorker1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker1))
                .ForMember(dest => dest.HistoricalCapacityWorker2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker2))
                .ForMember(dest => dest.HistoricalCapacityWorker3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker3))
                .ForMember(dest => dest.HistoricalCapacityWorker4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker4))
                .ForMember(dest => dest.HistoricalCapacityWorker5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker5))
                .ForMember(dest => dest.HistoricalCapacityWorker6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker6))
                .ForMember(dest => dest.HistoricalCapacityWorker7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker7))
                .ForMember(dest => dest.HistoricalCapacityGroup1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup1))
                .ForMember(dest => dest.HistoricalCapacityGroup2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup2))
                .ForMember(dest => dest.HistoricalCapacityGroup3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup3))
                .ForMember(dest => dest.HistoricalCapacityGroup4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup4))
                .ForMember(dest => dest.HistoricalCapacityGroup5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup5))
                .ForMember(dest => dest.HistoricalCapacityGroup6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup6))
                .ForMember(dest => dest.HistoricalCapacityGroup7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup7))
                .ForMember(dest => dest.WIP1, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP1))
                .ForMember(dest => dest.WIP2, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP2))
                .ForMember(dest => dest.WIP3, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP3))
                .ForMember(dest => dest.WIP4, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP4))
                .ForMember(dest => dest.WIP5, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP5))
                .ForMember(dest => dest.WIP6, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP6))
                .ForMember(dest => dest.WIP7, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP7))
                .ForMember(dest => dest.TargetSystem1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem1))
                .ForMember(dest => dest.TargetSystem2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem2))
                .ForMember(dest => dest.TargetSystem3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem3))
                .ForMember(dest => dest.TargetSystem4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem4))
                .ForMember(dest => dest.TargetSystem5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem5))
                .ForMember(dest => dest.TargetSystem6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem6))
                .ForMember(dest => dest.TargetSystem7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem7))
                .ForMember(dest => dest.TargetManual1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual1))
                .ForMember(dest => dest.TargetManual2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual2))
                .ForMember(dest => dest.TargetManual3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual3))
                .ForMember(dest => dest.TargetManual4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual4))
                .ForMember(dest => dest.TargetManual5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual5))
                .ForMember(dest => dest.TargetManual6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual6))
                .ForMember(dest => dest.TargetManual7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual7))
                .ForMember(dest => dest.PercentAttendance1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance1))
                .ForMember(dest => dest.PercentAttendance2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance2))
                .ForMember(dest => dest.PercentAttendance3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance3))
                .ForMember(dest => dest.PercentAttendance4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance4))
                .ForMember(dest => dest.PercentAttendance5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance5))
                .ForMember(dest => dest.PercentAttendance6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance6))
                .ForMember(dest => dest.PercentAttendance7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance7));

            Mapper.CreateMap<PlanTPOTPKCompositeDTO, PlantTPOTPKExcelModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.WorkerRegister, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WorkerRegister))
        .ForMember(dest => dest.WorkerAvailable, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WorkerAvailable))
        .ForMember(dest => dest.WorkerAlocation, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WorkerAlocation))
        .ForMember(dest => dest.WIP1, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP1))
        .ForMember(dest => dest.WIP2, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP2))
        .ForMember(dest => dest.WIP3, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP3))
        .ForMember(dest => dest.WIP4, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP4))
        .ForMember(dest => dest.WIP5, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP5))
        .ForMember(dest => dest.WIP6, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP6))
        .ForMember(dest => dest.WIP7, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP7))
        .ForMember(dest => dest.PercentAttendance1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance1))
        .ForMember(dest => dest.PercentAttendance2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance2))
        .ForMember(dest => dest.PercentAttendance3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance3))
        .ForMember(dest => dest.PercentAttendance4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance4))
        .ForMember(dest => dest.PercentAttendance5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance5))
        .ForMember(dest => dest.PercentAttendance6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance6))
        .ForMember(dest => dest.PercentAttendance7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance7))
                //.ForMember(dest => dest.HistoricalCapacityWorker1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker1))
                //.ForMember(dest => dest.HistoricalCapacityWorker2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker2))
                //.ForMember(dest => dest.HistoricalCapacityWorker3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker3))
                //.ForMember(dest => dest.HistoricalCapacityWorker4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker4))
                //.ForMember(dest => dest.HistoricalCapacityWorker5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker5))
                //.ForMember(dest => dest.HistoricalCapacityWorker6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker6))
                //.ForMember(dest => dest.HistoricalCapacityWorker7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker7))
        .ForMember(dest => dest.HistoricalCapacityGroup1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup1))
        .ForMember(dest => dest.HistoricalCapacityGroup2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup2))
        .ForMember(dest => dest.HistoricalCapacityGroup3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup3))
        .ForMember(dest => dest.HistoricalCapacityGroup4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup4))
        .ForMember(dest => dest.HistoricalCapacityGroup5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup5))
        .ForMember(dest => dest.HistoricalCapacityGroup6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup6))
        .ForMember(dest => dest.HistoricalCapacityGroup7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup7))
        .ForMember(dest => dest.TargetSystem1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem1))
        .ForMember(dest => dest.TargetSystem2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem2))
        .ForMember(dest => dest.TargetSystem3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem3))
        .ForMember(dest => dest.TargetSystem4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem4))
        .ForMember(dest => dest.TargetSystem5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem5))
        .ForMember(dest => dest.TargetSystem6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem6))
        .ForMember(dest => dest.TargetSystem7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem7))
        .ForMember(dest => dest.TargetManual1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual1))
        .ForMember(dest => dest.TargetManual2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual2))
        .ForMember(dest => dest.TargetManual3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual3))
        .ForMember(dest => dest.TargetManual4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual4))
        .ForMember(dest => dest.TargetManual5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual5))
        .ForMember(dest => dest.TargetManual6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual6))
        .ForMember(dest => dest.TargetManual7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual7))
        .ForMember(dest => dest.ProcessWorkHours1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.ProcessWorkHours1))
        .ForMember(dest => dest.ProcessWorkHours2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.ProcessWorkHours2))
        .ForMember(dest => dest.ProcessWorkHours3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.ProcessWorkHours3))
        .ForMember(dest => dest.ProcessWorkHours4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.ProcessWorkHours4))
        .ForMember(dest => dest.ProcessWorkHours5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.ProcessWorkHours5))
        .ForMember(dest => dest.ProcessWorkHours6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.ProcessWorkHours6))
        .ForMember(dest => dest.ProcessWorkHours7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.ProcessWorkHours7))
        .ForMember(dest => dest.TotalWorkhours, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TotalWorkhours))
        .ForMember(dest => dest.TotalTargetSystem, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TotalTargetSystem))
        .ForMember(dest => dest.TotalTargetManual, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TotalTargetManual));

            Mapper.CreateMap<PlanTPKViewModel, PlantTPKByProcessDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlantTPKByProcessDTO, PlanTPKViewModel>().IgnoreAllNonExisting();
            #endregion

            #endregion

            #region TPO
            //Mapper.CreateMap<PlanTPOTPKCompositeDTO, PlanTPOTPKViewModel>().IgnoreAllNonExisting()
            //     .ForMember(dest => dest.TPKTPOStartProductionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TPKTPOStartProductionDate));
            //Mapper.CreateMap<PlanTPOTPKViewModel, PlanTPOTPKCompositeDTO>().IgnoreAllNonExisting()
            //     .ForMember(dest => dest.TPKTPOStartProductionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TPKTPOStartProductionDate));

            Mapper.CreateMap<PlanTPOTPKCompositeDTO, PlanTPOTPKModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TPKTPOStartProductionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TPKTPOStartProductionDate))
                //.ForMember(dest => dest.HistoricalCapacityWorker1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker1))
                //.ForMember(dest => dest.HistoricalCapacityWorker2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker2))
                //.ForMember(dest => dest.HistoricalCapacityWorker3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker3))
                //.ForMember(dest => dest.HistoricalCapacityWorker4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker4))
                //.ForMember(dest => dest.HistoricalCapacityWorker5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker5))
                //.ForMember(dest => dest.HistoricalCapacityWorker6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker6))
                //.ForMember(dest => dest.HistoricalCapacityWorker7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityWorker7))
                .ForMember(dest => dest.HistoricalCapacityGroup1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup1))
                .ForMember(dest => dest.HistoricalCapacityGroup2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup2))
                .ForMember(dest => dest.HistoricalCapacityGroup3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup3))
                .ForMember(dest => dest.HistoricalCapacityGroup4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup4))
                .ForMember(dest => dest.HistoricalCapacityGroup5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup5))
                .ForMember(dest => dest.HistoricalCapacityGroup6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup6))
                .ForMember(dest => dest.HistoricalCapacityGroup7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.HistoricalCapacityGroup7))
                .ForMember(dest => dest.WIP1, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP1))
                .ForMember(dest => dest.WIP2, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP2))
                .ForMember(dest => dest.WIP3, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP3))
                .ForMember(dest => dest.WIP4, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP4))
                .ForMember(dest => dest.WIP5, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP5))
                .ForMember(dest => dest.WIP6, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP6))
                .ForMember(dest => dest.WIP7, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.WIP7))
                .ForMember(dest => dest.TargetSystem1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem1))
                .ForMember(dest => dest.TargetSystem2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem2))
                .ForMember(dest => dest.TargetSystem3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem3))
                .ForMember(dest => dest.TargetSystem4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem4))
                .ForMember(dest => dest.TargetSystem5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem5))
                .ForMember(dest => dest.TargetSystem6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem6))
                .ForMember(dest => dest.TargetSystem7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetSystem7))
                .ForMember(dest => dest.TargetManual1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual1))
                .ForMember(dest => dest.TargetManual2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual2))
                .ForMember(dest => dest.TargetManual3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual3))
                .ForMember(dest => dest.TargetManual4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual4))
                .ForMember(dest => dest.TargetManual5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual5))
                .ForMember(dest => dest.TargetManual6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual6))
                .ForMember(dest => dest.TargetManual7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.TargetManual7))
                .ForMember(dest => dest.PercentAttendance1, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance1))
                .ForMember(dest => dest.PercentAttendance2, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance2))
                .ForMember(dest => dest.PercentAttendance3, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance3))
                .ForMember(dest => dest.PercentAttendance4, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance4))
                .ForMember(dest => dest.PercentAttendance5, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance5))
                .ForMember(dest => dest.PercentAttendance6, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance6))
                .ForMember(dest => dest.PercentAttendance7, opt => opt.ResolveUsing<NullableFloatToZero>().FromMember(src => src.PercentAttendance7))
                .ForMember(dest => dest.TotalWorkHours1Prev3Weeks, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.TotalWorkHours1Prev3Weeks))
                .ForMember(dest => dest.TotalWorkHours2Prev3Weeks, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.TotalWorkHours2Prev3Weeks))
                .ForMember(dest => dest.TotalWorkHours3Prev3Weeks, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.TotalWorkHours3Prev3Weeks))
                .ForMember(dest => dest.TotalWorkHours4Prev3Weeks, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.TotalWorkHours4Prev3Weeks))
                .ForMember(dest => dest.TotalWorkHours5Prev3Weeks, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.TotalWorkHours5Prev3Weeks))
                .ForMember(dest => dest.TotalWorkHours6Prev3Weeks, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.TotalWorkHours6Prev3Weeks))
                .ForMember(dest => dest.TotalWorkHours7Prev3Weeks, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.TotalWorkHours7Prev3Weeks))
                .ForMember(dest => dest.TotalActualProductionPrev3Weeks, opt => opt.ResolveUsing<NullableIntToZero>().FromMember(src => src.TotalActualProductionPrev3Weeks));

            Mapper.CreateMap<PlanTPOTPKModel, TPOTPKDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TPKTPOStartProductionDate,
                    opt =>
                        opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TPKTPOStartProductionDate));

            Mapper.CreateMap<TPOTPKDTO, PlanTPOTPKModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TPKTPOStartProductionDate,
                    opt =>
                        opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TPKTPOStartProductionDate));

            Mapper.CreateMap<PlanTPOTPKViewModel, TPOTPKByProcessDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOTPKByProcessDTO, PlanTPOTPKViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanTPOTPKTotalBoxDTO, PlanTPOTPKTotalModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanTPOTPKTotalModel, PlanTPOTPKTotalBoxDTO>().IgnoreAllNonExisting();

            #region Report
            #region Summary Daily Production Target

            Mapper.CreateMap<PlanningReportProductionTargetCompositeDTO, PlanningReportProductionTargetViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<PlanningReportProductionTargetViewModel, PlanningReportProductionTargetCompositeDTO>().IgnoreAllNonExisting();

            #endregion

            #region Summary Daily Production Target

            Mapper.CreateMap<PlanningReportSummaryProcessTargetsCompositeDTO, PlanningReportSummaryProcessTargetsViewModel>().IgnoreAllNonExisting();

            #endregion

            #endregion

            #endregion

            #region PlanTmpWeeklyProductionPlanning
            Mapper.CreateMap<PlanTmpWeeklyProductionPlanningDTO, PlanTmpWeeklyProductionPlanningViewModel>().IgnoreAllNonExisting();
            #endregion
            #endregion

            #region Utilities
            Mapper.CreateMap<UtilTransactionLogDTO, UtilTransactionLogViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<TransactionHistoryDTO, TransactionHistoryViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<TransactionFlowDTO, TransactionFlowViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilRoleDTO, UtilSecurityRolesViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilSecurityRolesViewModel, UtilRoleDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilRuleDTO, UtilSecurityRulesViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilSecurityRulesViewModel, UtilRuleDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilFunctionDTO, UtilSecurityFunctionsViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilSecurityFunctionsViewModel, UtilFunctionDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UserSessionLocation, MstGenLocationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenLocationDTO, UserSessionLocation>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(m => m.LocationCode))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(m => m.LocationName));
            Mapper.CreateMap<UserSessionLocation, ResponsibilityLocation>().IgnoreAllNonExisting();
            Mapper.CreateMap<ResponsibilityLocation, UserSessionLocation>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(m => m.LocationData.LocationCode))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(m => m.LocationData.LocationName));
            Mapper.CreateMap<UtilResponsibilityDTO, UtilSecurityResponsibilitiesViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilSecurityResponsibilitiesViewModel, UtilResponsibilityDTO>().IgnoreAllNonExisting();


            Mapper.CreateMap<UtilUsersResponsibilityDTO, UtilSecurityUsersResponsibilitiesViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilSecurityUsersResponsibilitiesViewModel, UtilUsersResponsibilityDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilUsersResponsibilityDTO, UtilUsersResponsibilitiesRoleViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilUsersResponsibilitiesRoleViewModel, UtilUsersResponsibilityDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilUserResponsibilitiesRoleViewDTO, UtilUsersResponsibilitiesRoleViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.EffectiveDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EffectiveDate))
                 .ForMember(dest => dest.ExpiredDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ExpiredDate));
            Mapper.CreateMap<UtilUsersResponsibilitiesRoleViewModel, UtilUserResponsibilitiesRoleViewDTO>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.EffectiveDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EffectiveDate))
                 .ForMember(dest => dest.ExpiredDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.ExpiredDate));

            Mapper.CreateMap<UtilDelegationDto, UtilSecurityDelegationsViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.FromUser, opt => opt.MapFrom(m => m.UserADFrom))
                 .ForMember(dest => dest.ToUser, opt => opt.MapFrom(m => m.UserADTo))
                 .ForMember(dest => dest.Responsibility, opt => opt.MapFrom(m => m.ResponsibilityDesc))
                 .ForMember(dest => dest.StartDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.StartDate))
                 .ForMember(dest => dest.EndDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EndDate))
                 .ForMember(dest => dest.StartDateOldString, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.StartDateOld))
                ;

            Mapper.CreateMap<UtilSecurityDelegationsViewModel, UtilDelegationDto>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.UserADFrom, opt => opt.MapFrom(m => m.FromUser))
                 .ForMember(dest => dest.UserADTo, opt => opt.MapFrom(m => m.ToUser))
                 .ForMember(dest => dest.StartDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.StartDate))
                 .ForMember(dest => dest.EndDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EndDate))
                 .ForMember(dest => dest.StartDateOld, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.StartDateOldString))
                ;
            #endregion

            #region Util Workflow
            Mapper.CreateMap<UtilFlowDTO, UtilWorkflowFlowViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<UtilWorkflowFlowViewModel, UtilFlowDTO>().IgnoreAllNonExisting();
            #endregion

            #region Execution
            Mapper.CreateMap<ExePlantWorkerAbsenteeismDTO, ExePlantWorkerAbsenteeismViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.StartDateAbsent, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.StartDateAbsent))
                .ForMember(dest => dest.EndDateAbsent, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EndDateAbsent))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                ;
            Mapper.CreateMap<ExePlantWorkerAbsenteeismViewModel, ExePlantWorkerAbsenteeismDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.StartDateAbsent, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.StartDateAbsent))
                .ForMember(dest => dest.EndDateAbsent, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EndDateAbsent))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.OldValueStartDateAbsent, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.OldValueStartDateAbsent))
                .ForMember(dest => dest.OldValueEndDateAbsent, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.OldValueEndDateAbsent))
                ;
            Mapper.CreateMap<ExePlantWorkerAbsenteeismViewDTO, ExePlantWorkerAbsenteeismViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.StartDateAbsent, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.StartDateAbsent))
                .ForMember(dest => dest.EndDateAbsent, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EndDateAbsent))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.OldValueEmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.OldValueStartDateAbsent, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.StartDateAbsent))
                .ForMember(dest => dest.OldValueEndDateAbsent, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EndDateAbsent))
                .ForMember(dest => dest.OldValueShift, opt => opt.MapFrom(src => src.Shift))
                ;

            #region Report

            #region Report By Status

            Mapper.CreateMap<ExeReportByStatusDTO, ExeReportByStatusViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ProductionDate));
            Mapper.CreateMap<ExeReportByStatusWeeklyDTO, ExeReportByStatusWeeklyViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByStatusMonthlyDTO, ExeReportByStatusMonthlyViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExeReportByStatusViewModel, ExeReportByStatusDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.ProductionDate))
                ;
            Mapper.CreateMap<ExeReportByStatusMonthlyViewModel, ExeReportByStatusDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByStatusWeeklyViewModel, ExeReportByStatusDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetReportByStatus_Result, GetReportByStatusViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ActualWorker, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ActualWorker))
                .ForMember(dest => dest.ActualAbsWorker, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ActualAbsWorker))
                .ForMember(dest => dest.ProductionStick, opt => opt.ResolveUsing<LongToStringCommaSeparatorResolver>().FromMember(src => src.ProductionStick))
                .ForMember(dest => dest.ActualWorkHourPerDay, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorTrailingZerosResolver>().FromMember(src => src.ActualWorkHourPerDay))
                .ForMember(dest => dest.StickHourPeople, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorTrailingZerosResolver>().FromMember(src => src.StickHourPeople))
                .ForMember(dest => dest.StickHour, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorTrailingZerosResolver>().FromMember(src => src.StickHour));

            Mapper.CreateMap<GetReportByStatusCompositeDTO, GetReportByStatusCompositeViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TotalActual, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalActual))
                .ForMember(dest => dest.TotalAbsen, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalAbsen))
                .ForMember(dest => dest.TotalProductionStick, opt => opt.ResolveUsing<LongToStringCommaSeparatorResolver>().FromMember(src => src.TotalProductionStick))
                .ForMember(dest => dest.TotalWorkHourPerDay, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorTrailingZerosResolver>().FromMember(src => src.TotalWorkHourPerDay))
                .ForMember(dest => dest.TotalStickHourPeople, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorTrailingZerosResolver>().FromMember(src => src.TotalStickHourPeople))
                .ForMember(dest => dest.TotalStickHour, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorTrailingZerosResolver>().FromMember(src => src.TotalStickHour))
                .ForMember(dest => dest.TotalBalanceIndex, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorTrailingZerosResolver>().FromMember(src => src.TotalBalanceIndex))
                .ForMember(dest => dest.TotalWorkHour, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorTrailingZerosResolver>().FromMember(src => src.TotalWorkHour));
            #endregion

            #region Production Report by Process
            Mapper.CreateMap<ExeReportByProcessDTO, ExeReportByProcessViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByProcessViewModel, ExeReportByProcessDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByProcessViewDTO, ExeReportByProcessViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByProcessViewModel, ExeReportByProcessViewDTO>().IgnoreAllNonExisting();
            #endregion

            #region Report Daily Production

            Mapper.CreateMap<ExeReportDailyProductionAchievementViewDTO, ExeReportDailyProductionAchievementViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.SumAllTpkValue, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.SumAllTpkValue.Value, 2)))
                .ForMember(dest => dest.SumAllVarianStick, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.SumAllVarianStick.Value, 2)))
                .ForMember(dest => dest.SumAllVarianPercen, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.SumAllVarianPercen.Value, 2)))
                .ForMember(dest => dest.CumulativeMonday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.CumulativeMonday.Value, 2)))
                .ForMember(dest => dest.CumulativeTuesday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.CumulativeTuesday.Value, 2)))
                .ForMember(dest => dest.CumulativeWednesday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.CumulativeWednesday.Value, 2)))
                .ForMember(dest => dest.CumulativeThursday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.CumulativeThursday.Value, 2)))
                .ForMember(dest => dest.CumulativeFriday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.CumulativeFriday.Value, 2)))
                .ForMember(dest => dest.CumulativeSaturday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.CumulativeSaturday.Value, 2)))
                .ForMember(dest => dest.CumulativeSunday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.CumulativeSunday.Value, 2)))
                .ForMember(dest => dest.TotalAllCumulative, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.TotalAllCumulative.Value, 2)))
                .ForMember(dest => dest.DetailsData, opt => opt.ResolveUsing<DataDailyReportAchievmentResolver>().FromMember(src => src.Details))
                .ForMember(dest => dest.StickCumulativeMonday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.StickCumulativeMonday))
                .ForMember(dest => dest.StickCumulativeTuesday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.StickCumulativeTuesday))
                .ForMember(dest => dest.StickCumulativeWednesday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.StickCumulativeWednesday))
                .ForMember(dest => dest.StickCumulativeThursday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.StickCumulativeThursday))
                .ForMember(dest => dest.StickCumulativeFriday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.StickCumulativeFriday))
                .ForMember(dest => dest.StickCumulativeSaturday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.StickCumulativeSaturday))
                .ForMember(dest => dest.StickCumulativeSunday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.StickCumulativeSunday))
                .ForMember(dest => dest.CumulativeMonday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativeMonday))
                .ForMember(dest => dest.CumulativeTuesday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativeTuesday))
                .ForMember(dest => dest.CumulativeWednesday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativeWednesday))
                .ForMember(dest => dest.CumulativeThursday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativeThursday))
                .ForMember(dest => dest.CumulativeFriday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativeFriday))
                .ForMember(dest => dest.CumulativeSaturday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativeSaturday))
                .ForMember(dest => dest.CumulativeSunday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativeSunday))
                .ForMember(dest => dest.TotalHandRoleMonday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalHandRoleMonday))
                .ForMember(dest => dest.TotalHandRoleTuesday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalHandRoleTuesday))
                .ForMember(dest => dest.TotalHandrRoleWednesday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalHandrRoleWednesday))
                .ForMember(dest => dest.TotalHandrRoleThursday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalHandrRoleThursday))
                .ForMember(dest => dest.TotalHandrRoleFriday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalHandrRoleFriday))
                .ForMember(dest => dest.TotalHandrRoleSaturday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalHandrRoleSaturday))
                .ForMember(dest => dest.TotalHandrRoleSunday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalHandrRoleSunday))
                .ForMember(dest => dest.TotalMonday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalMonday))
                .ForMember(dest => dest.TotalTuesday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalTuesday))
                .ForMember(dest => dest.TotalWednesday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalWednesday))
                .ForMember(dest => dest.TotalThursday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalThursday))
                .ForMember(dest => dest.TotalFriday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalFriday))
                .ForMember(dest => dest.TotalSaturday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalSaturday))
                .ForMember(dest => dest.TotalSunday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalSunday))
                .ForMember(dest => dest.SumAllReliabilty, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.SumAllReliabilty == null ? 0 : src.SumAllReliabilty))
                .ForMember(dest => dest.SumTotalAllDay, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.SumTotalAllDay))
                ;


            Mapper.CreateMap<DataDailyProductionAchievmentDTO, DataDailyProductionAchievment>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Production, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.Production == null ? 0 : src.Production.Value))
                .ForMember(dest => dest.TPKValue, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TPKValue == null ? 0 : src.TPKValue.Value))
                .ForMember(dest => dest.WorkerCount, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.WorkerCount == null ? 0 : src.WorkerCount.Value))
                .ForMember(dest => dest.Package, opt => opt.ResolveUsing<FloatToString2DecimalCommaSeparatorResolver>().FromMember(src => src.Package))
                .ForMember(dest => dest.StdStickPerHour, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.StdStickPerHour == null ? 0 : src.StdStickPerHour.Value))
                .ForMember(dest => dest.ProductionMonday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ProductionMonday))
                .ForMember(dest => dest.ProductionTuesday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ProductionTuesday))
                .ForMember(dest => dest.ProductionWednesday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ProductionWednesday))
                .ForMember(dest => dest.ProductionThursday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ProductionThursday))
                .ForMember(dest => dest.ProductionFriday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ProductionFriday))
                .ForMember(dest => dest.ProductionSaturday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ProductionSaturday))
                .ForMember(dest => dest.ProductionSunday, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.ProductionSunday))
                .ForMember(dest => dest.TotalAllDay, opt => opt.ResolveUsing<IntToStringCommaSeparatorResolver>().FromMember(src => src.TotalAllDay))
                .ForMember(dest => dest.Realiability, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.Realiability == null ? 0 : src.Realiability.Value))
                .ForMember(dest => dest.SumTpkValue, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.SumTpkValue.Value, 2)))
                .ForMember(dest => dest.VarianceStick, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.VarianceStick.Value, 2)))
                .ForMember(dest => dest.VariancePersen, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => Math.Round(src.VariancePersen.Value, 2)))
                ;

            Mapper.CreateMap<DataDailyProductionAchievmentDTO, ReportDailyProductionViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<GetExeReportDailyProductionAchievementDTO, GetExeReportDailyProductionAchievementViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Monday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Monday))
                .ForMember(dest => dest.Tuesday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Tuesday))
                .ForMember(dest => dest.Wednesday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Wednesday))
                .ForMember(dest => dest.Thursday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Thursday))
                .ForMember(dest => dest.Friday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Friday))
                .ForMember(dest => dest.Saturday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Saturday))
                .ForMember(dest => dest.Sunday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Sunday))
                .ForMember(dest => dest.Total, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Total))
                .ForMember(dest => dest.Planning, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.Planning))
                .ForMember(dest => dest.VarianceStick, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.VarianceStick))
                .ForMember(dest => dest.VariancePercent, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.VariancePercent))
                .ForMember(dest => dest.ReliabilityPercent, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.ReliabilityPercent))
                .ForMember(dest => dest.VariancePercent, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.VariancePercent))
                .ForMember(dest => dest.ReliabilityPercent, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.ReliabilityPercent))
                .ForMember(dest => dest.Package, opt => opt.ResolveUsing<FloatToString2DecimalCommaSeparatorResolver>().FromMember(src => src.Package))
                .ForMember(dest => dest.TWHEqv, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TWHEqv))
                ;

            Mapper.CreateMap<ExeReportingDailyProductionAchievementDTOBrandCode, ExeReportingDailyProductionAchievementBrandCodeViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.SubTotalMonday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalMonday == null ? 0 : src.SubTotalMonday.Value))
                .ForMember(dest => dest.SubTotalTuesday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalTuesday == null ? 0 : src.SubTotalTuesday.Value))
                .ForMember(dest => dest.SubTotalWednesday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalWednesday == null ? 0 : src.SubTotalWednesday.Value))
                .ForMember(dest => dest.SubTotalThursday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalThursday == null ? 0 : src.SubTotalThursday.Value))
                .ForMember(dest => dest.SubTotalFriday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalFriday == null ? 0 : src.SubTotalFriday.Value))
                .ForMember(dest => dest.SubTotalSaturday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalSaturday == null ? 0 : src.SubTotalSaturday.Value))
                .ForMember(dest => dest.SubTotalSunday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalSunday == null ? 0 : src.SubTotalSunday.Value))
                .ForMember(dest => dest.SubTotalTotal, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalTotal == null ? 0 : src.SubTotalTotal.Value))
                .ForMember(dest => dest.SubTotalPlanning, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalPlanning == null ? 0 : src.SubTotalPlanning.Value))
                .ForMember(dest => dest.SubTotalVarianceStick, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.SubTotalVarianceStick == null ? 0 : src.SubTotalVarianceStick.Value))
                .ForMember(dest => dest.SubTotalVariancePercent, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.SubTotalVariancePercent == null ? 0 : src.SubTotalVariancePercent.Value))
                .ForMember(dest => dest.SubTotalReliabilityPercent, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.SubTotalReliabilityPercent == null ? 0 : src.SubTotalReliabilityPercent.Value))
                .ForMember(dest => dest.SubTotalPackage, opt => opt.ResolveUsing<FloatToString2DecimalCommaSeparatorResolver>().FromMember(src => src.SubTotalPackage))
                .ForMember(dest => dest.SubTotalTWHEqv, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.SubTotalTWHEqv == null ? 0 : src.SubTotalTWHEqv.Value))
                .ForMember(dest => dest.ListPerBrandCode, opt => opt.ResolveUsing<ExeReportDailyProdReportAchievementResolver>().FromMember(src => src.ListPerBrandCode))
                ;

            Mapper.CreateMap<ExeReportingDailyProductionAchievementDTOSKTBrandCode, ExeReportingDailyProductionAchievementSKTBrandCodeViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TotalMonday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalMonday == null ? 0 : src.TotalMonday.Value))
                .ForMember(dest => dest.TotalTuesday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalTuesday == null ? 0 : src.TotalTuesday.Value))
                .ForMember(dest => dest.TotalWednesday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalWednesday == null ? 0 : src.TotalWednesday.Value))
                .ForMember(dest => dest.TotalThursday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalThursday == null ? 0 : src.TotalThursday.Value))
                .ForMember(dest => dest.TotalFriday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalFriday == null ? 0 : src.TotalFriday.Value))
                .ForMember(dest => dest.TotalSaturday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalSaturday == null ? 0 : src.TotalSaturday.Value))
                .ForMember(dest => dest.TotalSunday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalSunday == null ? 0 : src.TotalSunday.Value))
                .ForMember(dest => dest.TotalTotal, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalTotal == null ? 0 : src.TotalTotal.Value))
                .ForMember(dest => dest.TotalPlanning, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalPlanning == null ? 0 : src.TotalPlanning.Value))
                .ForMember(dest => dest.TotalVarianceStick, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.TotalVarianceStick == null ? 0 : src.TotalVarianceStick.Value))
                .ForMember(dest => dest.TotalVariancePercent, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TotalVariancePercent == null ? 0 : src.TotalVariancePercent.Value))
                .ForMember(dest => dest.TotalReliabilityPercent, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TotalReliabilityPercent == null ? 0 : src.TotalReliabilityPercent.Value))
                .ForMember(dest => dest.TotalPackage, opt => opt.ResolveUsing<FloatToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TotalPackage))
                .ForMember(dest => dest.TotalTWHEqv, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TotalTWHEqv == null ? 0 : src.TotalTWHEqv.Value))
                .ForMember(dest => dest.ListPerSKTBrandCode, opt => opt.ResolveUsing<ExeReportDailyProdReportAchievementBandCodeResolver>().FromMember(src => src.ListPerSKTBrandCode))
                .ForMember(dest => dest.CumulativeTotalMonday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.CumulativeTotalMonday))
                .ForMember(dest => dest.CumulativeTotalTuesday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.CumulativeTotalTuesday))
                .ForMember(dest => dest.CumulativeTotalWednesday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.CumulativeTotalWednesday))
                .ForMember(dest => dest.CumulativeTotalThursday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.CumulativeTotalThursday))
                .ForMember(dest => dest.CumulativeTotalFriday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.CumulativeTotalFriday))
                .ForMember(dest => dest.CumulativeTotalSaturday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.CumulativeTotalSaturday))
                .ForMember(dest => dest.CumulativeTotalSunday, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(src => src.CumulativeTotalSunday))
                .ForMember(dest => dest.CumulativePercentTotalMonday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativePercentTotalMonday))
                .ForMember(dest => dest.CumulativePercentTotalTuesday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativePercentTotalTuesday))
                .ForMember(dest => dest.CumulativePercentTotalWednesday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativePercentTotalWednesday))
                .ForMember(dest => dest.CumulativePercentTotalThursday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativePercentTotalThursday))
                .ForMember(dest => dest.CumulativePercentTotalFriday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativePercentTotalFriday))
                .ForMember(dest => dest.CumulativePercentTotalSaturday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativePercentTotalSaturday))
                .ForMember(dest => dest.CumulativePercentTotalSunday, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.CumulativePercentTotalSunday))
                ;
            
            #endregion

            #endregion

            #endregion

            #region Production Report by Group
            Mapper.CreateMap<ExeReportByGroupViewDTO, ExeReportByGroupViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Register, opt => opt.MapFrom(src => src.Register.HasValue ? src.Register.Value : 0))
                .ForMember(dest => dest.A, opt => opt.MapFrom(src => src.A.HasValue ? src.A.Value : 0))
                .ForMember(dest => dest.I, opt => opt.MapFrom(src => src.I.HasValue ? src.I.Value : 0))
                .ForMember(dest => dest.S, opt => opt.MapFrom(src => src.S.HasValue ? src.S.Value : 0))
                .ForMember(dest => dest.C, opt => opt.MapFrom(src => src.C.HasValue ? src.C.Value : 0))
                .ForMember(dest => dest.CH, opt => opt.MapFrom(src => src.CH.HasValue ? src.CH.Value : 0))
                .ForMember(dest => dest.CT, opt => opt.MapFrom(src => src.CT.HasValue ? src.CT.Value : 0))
                .ForMember(dest => dest.SLSSLP, opt => opt.MapFrom(src => src.SLSSLP.HasValue ? src.SLSSLP.Value : 0))
                .ForMember(dest => dest.ETC, opt => opt.MapFrom(src => src.ETC.HasValue ? src.ETC.Value : 0))
                .ForMember(dest => dest.Multi_TPO, opt => opt.MapFrom(src => src.Multi_TPO.HasValue ? src.Multi_TPO.Value : 0))
                .ForMember(dest => dest.Multi_ROLL, opt => opt.MapFrom(src => src.Multi_ROLL.HasValue ? src.Multi_ROLL.Value : 0))
                .ForMember(dest => dest.Multi_CUTT, opt => opt.MapFrom(src => src.Multi_CUTT.HasValue ? src.Multi_CUTT.Value : 0))
                .ForMember(dest => dest.Multi_PACK, opt => opt.MapFrom(src => src.Multi_PACK.HasValue ? src.Multi_PACK.Value : 0))
                .ForMember(dest => dest.Multi_STAMP, opt => opt.MapFrom(src => src.Multi_STAMP.HasValue ? src.Multi_STAMP.Value : 0))
                .ForMember(dest => dest.Multi_FWRP, opt => opt.MapFrom(src => src.Multi_FWRP.HasValue ? src.Multi_FWRP.Value : 0))
                .ForMember(dest => dest.Multi_SWRP, opt => opt.MapFrom(src => src.Multi_SWRP.HasValue ? src.Multi_SWRP.Value : 0))
                .ForMember(dest => dest.Multi_GEN, opt => opt.MapFrom(src => src.Multi_GEN.HasValue ? src.Multi_GEN.Value : 0))
                .ForMember(dest => dest.Multi_WRP, opt => opt.MapFrom(src => src.Multi_WRP.HasValue ? src.Multi_WRP.Value : 0))
                .ForMember(dest => dest.In, opt => opt.MapFrom(src => src.In.HasValue ? src.In.Value : 0))
                .ForMember(dest => dest.Out, opt => opt.MapFrom(src => src.Out.HasValue ? src.Out.Value : 0))
                .ForMember(dest => dest.ActualWorker, opt => opt.MapFrom(src => src.ActualWorker.HasValue ? src.ActualWorker.Value : 0))
                .ForMember(dest => dest.WorkHour, opt => opt.MapFrom(src => src.WorkHour.HasValue ? src.WorkHour.Value : 0))
                .ForMember(dest => dest.Production, opt => opt.MapFrom(src => src.Production.HasValue ? src.Production.Value : 0))
                .ForMember(dest => dest.ValuePeople, opt => opt.MapFrom(src => src.ValuePeople.HasValue ? Math.Round(src.ValuePeople.Value,2) : 0))
                .ForMember(dest => dest.ValueHour, opt => opt.MapFrom(src => src.ValueHour.HasValue ? Math.Round(src.ValueHour.Value,2) : 0))
                .ForMember(dest => dest.ValuePeopleHour, opt => opt.MapFrom(src => src.ValuePeopleHour.HasValue ? Math.Round(src.ValuePeopleHour.Value,2) : 0))
            ;
            Mapper.CreateMap<ExeReportByGroupMonthlyDTO, ExeReportByGroupViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Register, opt => opt.MapFrom(src => src.Register))
                .ForMember(dest => dest.A, opt => opt.MapFrom(src => src.Absennce_A.HasValue ? src.Absennce_A.Value : 0))
                .ForMember(dest => dest.I, opt => opt.MapFrom(src => src.Absence_I.HasValue ? src.Absence_I.Value : 0))
                .ForMember(dest => dest.S, opt => opt.MapFrom(src => src.Absence_S.HasValue ? src.Absence_S.Value : 0))
                .ForMember(dest => dest.C, opt => opt.MapFrom(src => src.Absence_C.HasValue ? src.Absence_C.Value : 0))
                .ForMember(dest => dest.CH, opt => opt.MapFrom(src => src.Absence_CH.HasValue ? src.Absence_CH.Value : 0))
                .ForMember(dest => dest.CT, opt => opt.MapFrom(src => src.Absence_CT.HasValue ? src.Absence_CT.Value : 0))
                .ForMember(dest => dest.SLSSLP, opt => opt.MapFrom(src => Math.Round((src.Absence_SLP.HasValue ? src.Absence_SLP.Value : 0) + (src.Absence_SLS.HasValue ? src.Absence_SLS.Value : 0),2)))
                .ForMember(dest => dest.ETC, opt => opt.MapFrom(src => src.Absence_ETC.HasValue ? src.Absence_ETC.Value : 0))
                .ForMember(dest => dest.Multi_TPO, opt => opt.MapFrom(src => src.Multi_TPO.HasValue ? src.Multi_TPO.Value : 0))
                .ForMember(dest => dest.Multi_ROLL, opt => opt.MapFrom(src => src.Multi_ROLL.HasValue ? src.Multi_ROLL.Value : 0))
                .ForMember(dest => dest.Multi_CUTT, opt => opt.MapFrom(src => src.Multi_CUTT.HasValue ? src.Multi_CUTT.Value : 0))
                .ForMember(dest => dest.Multi_PACK, opt => opt.MapFrom(src => src.Multi_PACK.HasValue ? src.Multi_PACK.Value : 0))
                .ForMember(dest => dest.Multi_STAMP, opt => opt.MapFrom(src => src.Multi_STAMP.HasValue ? src.Multi_STAMP.Value : 0))
                .ForMember(dest => dest.Multi_FWRP, opt => opt.MapFrom(src => src.Multi_FWRP.HasValue ? src.Multi_FWRP.Value : 0))
                .ForMember(dest => dest.Multi_SWRP, opt => opt.MapFrom(src => src.Multi_SWRP.HasValue ? src.Multi_SWRP.Value : 0))
                .ForMember(dest => dest.Multi_GEN, opt => opt.MapFrom(src => src.Multi_GEN.HasValue ? src.Multi_GEN.Value : 0))
                .ForMember(dest => dest.Multi_WRP, opt => opt.MapFrom(src => src.Multi_WRP.HasValue ? src.Multi_WRP.Value : 0))
                .ForMember(dest => dest.In, opt => opt.MapFrom(src => src.EmpIn.HasValue ? src.EmpIn.Value : 0))
                .ForMember(dest => dest.Out, opt => opt.MapFrom(src => src.Out.HasValue ? src.Out.Value : 0))
                .ForMember(dest => dest.ActualWorker, opt => opt.MapFrom(src => src.ActualWorker.HasValue ? src.ActualWorker.Value : 0))
                .ForMember(dest => dest.WorkHour, opt => opt.MapFrom(src => src.WorkHour.HasValue ? src.WorkHour.Value : 0))
                .ForMember(dest => dest.Production, opt => opt.MapFrom(src => src.Production.HasValue ? src.Production.Value : 0))
                .ForMember(dest => dest.ValuePeople, opt => opt.MapFrom(src => src.ValuePeople.HasValue ? Math.Round(src.ValuePeople.Value,2) : 0))
                .ForMember(dest => dest.ValueHour, opt => opt.MapFrom(src => src.ValueHour.HasValue ? Math.Round(src.ValueHour.Value,2) : 0))
                .ForMember(dest => dest.ValuePeopleHour, opt => opt.MapFrom(src => src.ValuePeopleHour.HasValue ? Math.Round(src.ValuePeopleHour.Value,2) : 0))
            ;
            Mapper.CreateMap<ExeReportByGroupWeeklyDTO, ExeReportByGroupViewModel>().IgnoreAllNonExisting()
               .ForMember(dest => dest.Register, opt => opt.MapFrom(src => src.Register))
               .ForMember(dest => dest.A, opt => opt.MapFrom(src => src.Absennce_A.HasValue ? src.Absennce_A.Value : 0))
               .ForMember(dest => dest.I, opt => opt.MapFrom(src => src.Absence_I.HasValue ? src.Absence_I.Value : 0))
               .ForMember(dest => dest.S, opt => opt.MapFrom(src => src.Absence_S.HasValue ? src.Absence_S.Value : 0))
               .ForMember(dest => dest.C, opt => opt.MapFrom(src => src.Absence_C.HasValue ? src.Absence_C.Value : 0))
               .ForMember(dest => dest.CH, opt => opt.MapFrom(src => src.Absence_CH.HasValue ? src.Absence_CH.Value : 0))
               .ForMember(dest => dest.CT, opt => opt.MapFrom(src => src.Absence_CT.HasValue ? src.Absence_CT.Value : 0))
               .ForMember(dest => dest.SLSSLP, opt => opt.MapFrom(src => Math.Round((src.Absence_SLP.HasValue ? src.Absence_SLP.Value : 0) + (src.Absence_SLS.HasValue ? src.Absence_SLS.Value : 0),2)))
               .ForMember(dest => dest.ETC, opt => opt.MapFrom(src => src.Absence_ETC.HasValue ? src.Absence_ETC.Value : 0))
               .ForMember(dest => dest.Multi_TPO, opt => opt.MapFrom(src => src.Multi_TPO.HasValue ? src.Multi_TPO.Value : 0))
               .ForMember(dest => dest.Multi_ROLL, opt => opt.MapFrom(src => src.Multi_ROLL.HasValue ? src.Multi_ROLL.Value : 0))
               .ForMember(dest => dest.Multi_CUTT, opt => opt.MapFrom(src => src.Multi_CUTT.HasValue ? src.Multi_CUTT.Value : 0))
               .ForMember(dest => dest.Multi_PACK, opt => opt.MapFrom(src => src.Multi_PACK.HasValue ? src.Multi_PACK.Value : 0))
               .ForMember(dest => dest.Multi_STAMP, opt => opt.MapFrom(src => src.Multi_STAMP.HasValue ? src.Multi_STAMP.Value : 0))
               .ForMember(dest => dest.Multi_FWRP, opt => opt.MapFrom(src => src.Multi_FWRP.HasValue ? src.Multi_FWRP.Value : 0))
               .ForMember(dest => dest.Multi_SWRP, opt => opt.MapFrom(src => src.Multi_SWRP.HasValue ? src.Multi_SWRP.Value : 0))
               .ForMember(dest => dest.Multi_GEN, opt => opt.MapFrom(src => src.Multi_GEN.HasValue ? src.Multi_GEN.Value : 0))
               .ForMember(dest => dest.Multi_WRP, opt => opt.MapFrom(src => src.Multi_WRP.HasValue ? src.Multi_WRP.Value : 0))
               .ForMember(dest => dest.In, opt => opt.MapFrom(src => src.EmpIn.HasValue ? src.EmpIn.Value : 0))
               .ForMember(dest => dest.Out, opt => opt.MapFrom(src => src.Out.HasValue ? src.Out.Value : 0))
               .ForMember(dest => dest.ActualWorker, opt => opt.MapFrom(src => src.ActualWorker.HasValue ? src.ActualWorker.Value : 0))
               .ForMember(dest => dest.WorkHour, opt => opt.MapFrom(src => src.WorkHour.HasValue ? src.WorkHour.Value : 0))
               .ForMember(dest => dest.Production, opt => opt.MapFrom(src => src.Production.HasValue ? src.Production.Value : 0))
               .ForMember(dest => dest.ValuePeople, opt => opt.MapFrom(src => src.ValuePeople.HasValue ? Math.Round(src.ValuePeople.Value, 2) : 0))
                .ForMember(dest => dest.ValueHour, opt => opt.MapFrom(src => src.ValueHour.HasValue ? Math.Round(src.ValueHour.Value, 2) : 0))
                .ForMember(dest => dest.ValuePeopleHour, opt => opt.MapFrom(src => src.ValuePeopleHour.HasValue ? Math.Round(src.ValuePeopleHour.Value, 2) : 0))
           ;
            Mapper.CreateMap<ExeReportByGroupDTO, ExeReportByGroupViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Register, opt => opt.MapFrom(src => src.Register))
               .ForMember(dest => dest.A, opt => opt.MapFrom(src => src.Absennce_A))
               .ForMember(dest => dest.I, opt => opt.MapFrom(src => src.Absence_I))
               .ForMember(dest => dest.S, opt => opt.MapFrom(src => src.Absence_S.HasValue ? src.Absence_S.Value : 0))
               .ForMember(dest => dest.C, opt => opt.MapFrom(src => src.Absence_C))
               .ForMember(dest => dest.CH, opt => opt.MapFrom(src =>src.Absence_CH))
               .ForMember(dest => dest.CT, opt => opt.MapFrom(src =>src.Absence_CT))
               .ForMember(dest => dest.SLSSLP, opt => opt.MapFrom(src => Math.Round((src.Absence_SLP) + (src.Absence_SLS),2)))
               .ForMember(dest => dest.ETC, opt => opt.MapFrom(src => src.Absence_ETC))
               .ForMember(dest => dest.Multi_TPO, opt => opt.MapFrom(src => src.Multi_TPO))
               .ForMember(dest => dest.Multi_ROLL, opt => opt.MapFrom(src => src.Multi_ROLL))
               .ForMember(dest => dest.Multi_CUTT, opt => opt.MapFrom(src => src.Multi_CUTT))
               .ForMember(dest => dest.Multi_PACK, opt => opt.MapFrom(src => src.Multi_PACK))
               .ForMember(dest => dest.Multi_STAMP, opt => opt.MapFrom(src => src.Multi_STAMP))
               .ForMember(dest => dest.Multi_FWRP, opt => opt.MapFrom(src => src.Multi_FWRP))
               .ForMember(dest => dest.Multi_SWRP, opt => opt.MapFrom(src => src.Multi_SWRP))
               .ForMember(dest => dest.Multi_GEN, opt => opt.MapFrom(src => src.Multi_GEN))
               .ForMember(dest => dest.Multi_WRP, opt => opt.MapFrom(src => src.Multi_WRP))
               .ForMember(dest => dest.In, opt => opt.MapFrom(src => src.In))
               .ForMember(dest => dest.Out, opt => opt.MapFrom(src => src.Out))
               .ForMember(dest => dest.ActualWorker, opt => opt.MapFrom(src => src.ActualWorker.HasValue ? src.ActualWorker.Value : 0))
               .ForMember(dest => dest.WorkHour, opt => opt.MapFrom(src => src.WorkHour))
               .ForMember(dest => dest.Production, opt => opt.MapFrom(src => src.Production))
               .ForMember(dest => dest.ValuePeople, opt => opt.MapFrom(src => Math.Round(src.ValuePeople, 2) ))
               .ForMember(dest => dest.ValueHour, opt => opt.MapFrom(src =>  Math.Round(src.ValueHour, 2)))
               .ForMember(dest => dest.ValuePeopleHour, opt => opt.MapFrom(src => Math.Round(src.ValuePeopleHour, 2) ))
                ;


            Mapper.CreateMap<ExeReportByGroupViewModel, ExeReportByGroupDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByGroupMonthlyDTO, ExeReportByGroupMonthlyViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByGroupMonthlyViewModel, ExeReportByGroupMonthlyDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByGroupWeeklyDTO, ExeReportByGroupWeeklyViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeReportByGroupWeeklyViewModel, ExeReportByGroupWeeklyDTO>().IgnoreAllNonExisting();
            #endregion

            #region Production Stock By Process
            Mapper.CreateMap<ExeReportProdStockPerBrandGroupCodeDTO, ExeReportProdStockPerBrandGroupCodeViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TotalBeginStockInMovePerBrandGroupCode, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalBeginStockInMovePerBrandGroupCode))
                .ForMember(dest => dest.TotalBeginStockExtMovePerBrandGroupCode, opt =>  opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalBeginStockExtMovePerBrandGroupCode))
                .ForMember(dest => dest.TotalProdPerBrandGroupCode, opt =>  opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalProdPerBrandGroupCode))
                .ForMember(dest => dest.TotalPAPPerBrandGroupCode, opt =>  opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalPAPPerBrandGroupCode))
                .ForMember(dest => dest.TotalPAGPerBrandGroupCode, opt =>  opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalPAGPerBrandGroupCode))
                .ForMember(dest => dest.TotalPlanningPerBrandGroupCode, opt =>  opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalPlanningPerBrandGroupCode))
                .ForMember(dest => dest.TotalEndStockInMovePerBrandGroupCode,  opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalEndStockInMovePerBrandGroupCode))
                .ForMember(dest => dest.TotalEndStockExtMovePerBrandGroupCode, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalEndStockExtMovePerBrandGroupCode))
                .ForMember(dest => dest.TotalVarStickPerBrandGroupCode, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalVarStickPerBrandGroupCode))
                .ForMember(dest => dest.TotalVarStickPercentPerBrandGroupCode, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(m => m.TotalVarStickPercentPerBrandGroupCode))
                ;

            Mapper.CreateMap<ExeReportProdStockPerBrandDTO, ExeReportProdStockPerBrandViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TotalBeginStockInMovePerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalBeginStockInMovePerBrand))
                .ForMember(dest => dest.TotalBeginStockExtMovePerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalBeginStockExtMovePerBrand))
                .ForMember(dest => dest.TotalProdPerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalProdPerBrand))
                .ForMember(dest => dest.TotalPAPPerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalPAPPerBrand))
                .ForMember(dest => dest.TotalPAGPerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalPAGPerBrand))
                .ForMember(dest => dest.TotalPlanningPerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalPlanningPerBrand))
                .ForMember(dest => dest.TotalEndStockInMovePerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalEndStockInMovePerBrand))
                .ForMember(dest => dest.TotalEndStockExtMovePerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalEndStockExtMovePerBrand))
                .ForMember(dest => dest.TotalVarStickPerBrand, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.TotalVarStickPerBrand))
                .ForMember(dest => dest.TotalVarStickPercentPerBrand, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(m => m.TotalVarStickPercentPerBrand))
                ;

            Mapper.CreateMap<ExeReportProductionStockDTO, ExeReportProductionStockViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.BeginStockInternalMove, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.BeginStockInternalMove))
                .ForMember(dest => dest.BeginStockExternalMove, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.BeginStockExternalMove))
                .ForMember(dest => dest.Production, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.Production))
                .ForMember(dest => dest.PAP, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.PAP))
                .ForMember(dest => dest.PAG, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.PAG))
                .ForMember(dest => dest.Planning, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.Planning))
                .ForMember(dest => dest.EndingStockInternalMove, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.EndingStockInternalMove))
                .ForMember(dest => dest.EndingStockExternalMove, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.EndingStockExternalMove))
                .ForMember(dest => dest.VarianceStick, opt => opt.ResolveUsing<DoubleToStringCommaSeparatorResolver>().FromMember(m => m.VarianceStick))
                .ForMember(dest => dest.VariancePercent, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(m => m.VariancePercent))
                ;
            #endregion

            Mapper.CreateMap<MaintenanceExecutionInventoryViewDTO, MaintenanceExecutionInventoryViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetInventoryView_Result, MaintenanceExecutionInventoryViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<WorkerBrandAssigmentDTO, PlanPlantAllocationDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(m => m.EmployeeID));

            Mapper.CreateMap<ExeProductionEntryPrintDataViewDTO, PrintDataModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.PrintDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.PrintDate));
            Mapper.CreateMap<PrintDataModel, ExeProductionEntryPrintDataViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeProductionEntryPrintViewDTO, ExeOthersProductionEntryPrintViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeOthersProductionEntryPrintViewModel, ExeProductionEntryPrintViewDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExeProductionEntryPrintViewPartitionModel, ExeProductionEntryPrintViewPartitionDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeProductionEntryPrintViewPartitionDTO, ExeProductionEntryPrintViewPartitionModel>().IgnoreAllNonExisting();


            #region Production Entry
            Mapper.CreateMap<ExePlantProductionEntryDTO, ExePlantProductionEntryViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdTarget, opt => opt.ResolveUsing<NullableFloatToString>().FromMember(src => src.ProdTarget))
                .ForMember(dest => dest.ProdCapacity, opt => opt.ResolveUsing<DecimalToString3DecimalPlacesTrailingZerosResolver>().FromMember(src => src.ProdCapacity))
                ;
            Mapper.CreateMap<ExePlantProductionEntryViewModel, ExePlantProductionEntryDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProdTarget, opt => opt.ResolveUsing<StringToNullableFloat>().FromMember(src => src.ProdTarget))
                .ForMember(dest => dest.ProdCapacity, opt => opt.ResolveUsing<StringToNullableDecimal>().FromMember(src => src.ProdCapacity))
                ;

            Mapper.CreateMap<ExePlantProductionEntryAllocationCompositeDTO, ExePlantProductionEntryDTO>().IgnoreAllNonExisting();
            #endregion

            #region Actual Work Hours
            Mapper.CreateMap<ExePlantActualWorkHoursDTO, ExePlantActualWorkHoursViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TimeIn, opt => opt.ResolveUsing<TimeToStringHourAndMinuteOnlyResolver>().FromMember(src => src.TimeIn))
                .ForMember(dest => dest.TimeOut, opt => opt.ResolveUsing<TimeToStringHourAndMinuteOnlyResolver>().FromMember(src => src.TimeOut))
                .ForMember(dest => dest.BreakTime, opt => opt.ResolveUsing<TimeToStringHourAndMinuteOnlyResolver>().FromMember(src => src.BreakTime));
            Mapper.CreateMap<ExePlantActualWorkHoursViewModel, ExePlantActualWorkHoursDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExeTPOActualWorkHoursDTO, ExeTPOActualWorkHoursViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TimeIn,
                    opt => opt.ResolveUsing<TimeToStringHourAndMinuteOnlyResolver>().FromMember(src => src.TimeIn))
                .ForMember(dest => dest.TimeOut,
                    opt => opt.ResolveUsing<TimeToStringHourAndMinuteOnlyResolver>().FromMember(src => src.TimeOut))
                .ForMember(dest => dest.BreakTime,
                    opt => opt.ResolveUsing<TimeToStringHourAndMinuteOnlyResolver>().FromMember(src => src.BreakTime));
            Mapper.CreateMap<ExeTPOActualWorkHoursViewModel, ExeTPOActualWorkHoursDTO>().IgnoreAllNonExisting();
            #endregion

            #region WOrker Assignment
            Mapper.CreateMap<ExePlantWorkerAssignmentDTO, ExePlantWorkerAssignmentViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.StartDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EndDate))
                .ForMember(dest => dest.OldStartDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.OldStartDate))
                .ForMember(dest => dest.OldEndDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.OldEndDate))
                ;
            Mapper.CreateMap<ExePlantWorkerAssignmentViewModel, ExePlantWorkerAssignmentDTO>().IgnoreAllNonExisting()
            .ForMember(dest => dest.TransactionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.TransactionDate))
                .ForMember(dest => dest.StartDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EndDate))
                .ForMember(dest => dest.OldStartDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.OldStartDate))
                .ForMember(dest => dest.OldEndDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.OldEndDate))
                ;
            #endregion

            #region TPO Production Entry
            Mapper.CreateMap<ExeTPOProductionViewDTO, ExeTPOProductionEntryViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.ProductionDate));
            Mapper.CreateMap<ExeTPOProductionEntryViewModel, ExeTPOProductionViewDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.ProductionDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #region TPO Production Entry Verification
            Mapper.CreateMap<ExeTPOProductionEntryVerificationViewDTO, ExeTPOProductionEntryVerificationCompositeViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.ProductionDate));
            Mapper.CreateMap<ExeTPOProductionEntryVerificationCompositeViewModel, ExeTPOProductionEntryVerificationDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.ProductionDate));
            Mapper.CreateMap<ExeTPOProductionEntryVerificationDTO, ExeTPOProductionEntryVerificationCompositeViewModel>().IgnoreAllNonExisting();
            #endregion

            #region Material Usages
            Mapper.CreateMap<ExePlantMaterialUsagesDTO, ExePlantMaterialUsagesViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ProductionDate));
            Mapper.CreateMap<ExePlantMaterialUsagesViewModel, ExePlantMaterialUsagesDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.ProductionDate));
            #endregion

            #region Closing Payroll

            Mapper.CreateMap<MasterGenClosingPayrollViewModel, MstClosingPayrollDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstClosingPayrollDTO, GetMasterGenClosingPayrollViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ClosingDate, opt => opt.MapFrom(src => src.ClosingDate.ToShortDateString()));
            #endregion

            #region Product Adjustment

            Mapper.CreateMap<ProductAdjustmentDTO, ExeProductAdjustmentViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeProductAdjustmentViewModel, ProductAdjustmentDTO>().IgnoreAllNonExisting();

            #endregion

            #region Plant Production Entry Verification
            Mapper.CreateMap<ExePlantProductionEntryVerificationViewDTO, ExePlantProductionEntryVerificationViewViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.ProductionDate))
                .ForMember(dest => dest.VerifyManual, opt => opt.ResolveUsing<NullableBooleanToBooleanResolver>().FromMember(src => src.VerifyManual))
                .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.Plant.HasValue ? Math.Round(src.Plant.Value, 2) : 0))
                .ForMember(dest => dest.Actual, opt => opt.MapFrom(src => src.Actual.HasValue ? Math.Round(src.Actual.Value, 2) : 0))
                ;
            Mapper.CreateMap<ExePlantProductionEntryVerificationViewViewModel, ExePlantProductionEntryVerificationViewDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.ProductionDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #region Worker balancing
            Mapper.CreateMap<ExePlantWorkerBalancingViewDTO, ExePlantWorkerBalancingViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExePlantWorkerBalancingViewModel, ExePlantWorkerBalancingViewDTO>().IgnoreAllNonExisting();

            Mapper.CreateMap<ExePlantWorkerBalancingSingleViewDTO, ExePlantWorkerSingleBalancingViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExePlantWorkerSingleBalancingViewModel, ExePlantWorkerBalancingSingleViewDTO>().IgnoreAllNonExisting();
            #endregion

            #region Plant Wages

            #region Production Card
            Mapper.CreateMap<ProductionCardDTO, ProductionCardViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.ProductionDate))
                .ForMember(dest => dest.UpahLain, opt => opt.ResolveUsing<FloatToStringResolver>().FromMember(src => src.UpahLain));
            Mapper.CreateMap<ProductionCardViewModel, ProductionCardDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate,
                    opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.ProductionDate))
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate,
                    opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.UpahLain,
                    opt => opt.ResolveUsing<StringToFloatResolver>().FromMember(src => src.UpahLain));
            #endregion

            #region Plant Wages

            Mapper.CreateMap<ExePlantProductionEntryVerificationDTO, ExePlantProductionEntryVerificationViewModel>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate,
                    opt => opt.MapFrom(src => src.ProductionDate.ToShortDateString()))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.ExeProductionEntryRelease.Remark))
                .ForMember(dest => dest.Checkbox, opt => opt.ResolveUsing<SetCheckboxEblekVerificationEntry>().FromMember(src => src.ExeProductionEntryRelease));

            Mapper.CreateMap<ExePlantProductionEntryVerificationDTO, ExeProductionEntryReleaseViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(src => src.ProductionDate))
                .ForMember(dest => dest.BrandCode, opt => opt.MapFrom(src => src.BrandCode))
                .ForMember(dest => dest.ProcessGroup, opt => opt.MapFrom(src => src.ProcessGroup))
                .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.GroupCode));
            Mapper.CreateMap<ExePlantProductionEntryVerificationViewModel, ExePlantProductionEntryVerificationDTO>()
                .IgnoreAllNonExisting();
                                                                    

            Mapper.CreateMap<ExeProductionEntryReleaseModel, ExeProductionEntryReleaseDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeProductionEntryReleaseDTO, ExeProductionEntryReleaseModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ExePlantProductionEntryVerification, opt => opt.MapFrom(src => new ExePlantProductionEntryVerificationViewModel { ProductionDate = src.ExePlantProductionEntryVerification.ProductionDate.ToShortDateString(), 
                    BrandCode = src.ExePlantProductionEntryVerification.BrandCode, GroupCode = src.ExePlantProductionEntryVerification.GroupCode }))
                ;

            #endregion

            #region Production Card Approval
            Mapper.CreateMap<WagesProductionCardApprovalCompositeDTO, WagesProductionCardApprovalViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ProductionDate));
            Mapper.CreateMap<WagesProductionCardApprovalDetailViewDTO, WagesProductionCardApprovalDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.ProductionDate));
            Mapper.CreateMap<WagesProductionCardApprovalDetailViewGroupDTO, WagesProductionCardApprovalDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.ProductionDate));
            #endregion

            #region Eblek Release Approval
            Mapper.CreateMap<EblekReleaseApprovalDTO, EblekReleaseApprovalViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EblekDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EblekDate));
            Mapper.CreateMap<EblekReleaseApprovalViewModel, EblekReleaseApprovalDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EblekDate, opt => opt.ResolveUsing<StringToDateOnlyResolver>().FromMember(src => src.EblekDate));
            #endregion

            #region Production Card Correction
            Mapper.CreateMap<ProductionCardDTO, ProductionCardViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.UpdatedDate))
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateToStringResolver>().FromMember(src => src.ProductionDate));
            Mapper.CreateMap<ProductionCardViewModel, ProductionCardDTO>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.ProductionDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.ResolveUsing<StringToDateResolver>().FromMember(src => src.UpdatedDate));
            #endregion

            #endregion

            #region TPOFee
            Mapper.CreateMap<TPOFeeExeActualViewDTO, TPOFeeExeActualViewModel>().IgnoreAllNonExisting();

            #region TPO Fee GL Accrued
            Mapper.CreateMap<TPOFeeExeGLAccruedViewListDTO, TPOFeeExeGLAccruedViewListModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeExeGLAccruedHdrDTO, InitTPOFeeExeGLAccruedDetailViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationName))
                .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.Regional))
                .ForMember(dest => dest.RegionalName, opt => opt.MapFrom(src => src.RegionalName))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.KpsYear, opt => opt.MapFrom(src => src.KpsYear))
                .ForMember(dest => dest.KpsWeek, opt => opt.MapFrom(src => src.KpsWeek))
                .ForMember(dest => dest.ClosingDate, opt => opt.MapFrom(src => src.ClosingDate))
                .ForMember(dest => dest.StickPerBox, opt => opt.MapFrom(src => src.StickPerBox))
                .ForMember(dest => dest.Paket, opt => opt.MapFrom(src => src.Paket))
                ;
            Mapper.CreateMap<TPOFeeExeGLAccruedDetailDTO, InitTPOFeeExeGLAccruedDetailViewModel>().IgnoreAllNonExisting();
            #endregion

            #endregion
            #region TPO Fee Approval
            Mapper.CreateMap<TPOFeeApprovalViewDTO, TPOFeeApprovalViewModel>().IgnoreAllNonExisting();
            #endregion

            #region TPO Fee Execution Plan

            Mapper.CreateMap<TpoFeePlanViewDto, TpoFeeExePlanViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeProductionDailyPlanDto, TpoFeeProductionDailyPlanModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.Hari, opt => opt.ResolveUsing<DateToDayNameResolver>().FromMember(src => src.FeeDate));

            #endregion

            #region TPO Fee AP Open
            Mapper.CreateMap<TPOFeeAPOpenDTO, TPOFeeAPOpenViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<TPOFeeExeAPOpenViewDTO, TPOFeeExeAPOpenViewModel>().IgnoreAllNonExisting();

            #endregion

            #region Generate P1 Template
            Mapper.CreateMap<TPOGenerateP1TemplateViewDTO, TPOGenerateP1TemplateViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<TPOGenerateP1TemplateViewModel, TPOGenerateP1TemplateViewDTO>().IgnoreAllNonExisting();

            #endregion

            #region TPO Fee AP Close
            Mapper.CreateMap<TPOFeeAPCloseDTO, TPOFeeAPCloseViewModel>().IgnoreAllNonExisting();
            #endregion

            #region Report Plan

            Mapper.CreateMap<MstTableauReportDto, ReportPlanViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstTableauReportDto, ReportTpoViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<ReportPlanViewModel, MstTableauReportDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<ReportTpoViewModel, MstTableauReportDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<ReportPlanModel, MstTableauReportDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<ReportTpoModel, MstTableauReportDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstTableauReportDto, ReportPlanViewReportViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstTableauReportDto, ReportTpoViewReportViewModel>().IgnoreAllNonExisting();

            #endregion

            #region eblek release

            Mapper.CreateMap<ExePlantProductionEntryVerificationModel, ExeProductionEntryReleaseDTO>()
                .IgnoreAllNonExisting();

            #endregion

            Mapper.CreateMap<TPOFeeSettingCalculationViewModel, TPOFeeSettingCalculationDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeSettingCalculationDTO, TPOFeeSettingCalculationViewModel>().IgnoreAllNonExisting();

            Mapper.CreateMap<MstGenLocationDTO, Level0>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenLocationDTO, Level1>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenLocationDTO, Level2>().IgnoreAllNonExisting();
            Mapper.CreateMap<MstGenLocationDTO, Level3>().IgnoreAllNonExisting();

            #region Wages Absent Report
            Mapper.CreateMap<GetWagesReportAbsentViewModel, GetWagesReportAbsentViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetWagesReportAbsentViewDTO, GetWagesReportAbsentViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetWagesReportAbsentDetailViewModel, GetWagesReportAbsentGroupViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetWagesReportAbsentGroupViewDTO, GetWagesReportAbsentDetailViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<WagesReportDetailEmployeeDTO, WagesReportDetailEmployeeViewModel>().IgnoreAllNonExisting()
             .ForMember(dest => dest.ProductionDate, opt => opt.MapFrom(m => m.ProductionDate.Value.ToShortDateString()));
            #endregion

            #region TPO Reports Package
            Mapper.CreateMap<TPOFeeReportsPackageCompositeDTO, TPOFeeReportsPackageViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<TPOFeeReportsPackageWeeklyDTO, TPOFeeReportsPackageWeeklyViewModel>().IgnoreAllNonExisting();

            #endregion

            #region EMS Source Data
            Mapper.CreateMap<ExeReportEMSSourceDataViewModel, ExeEMSSourceDataDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<ExeEMSSourceDataDTO, ExeReportEMSSourceDataViewModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.ProductionDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.ProductionDate));
            #endregion

            #region TPO Reports Summary
            Mapper.CreateMap<TPOFeeReportsSummaryCompositeDTO, TPOFeeReportsSummaryViewModel>().IgnoreAllNonExisting()
            .ForMember(dest => dest.TotalCalculateString, opt => opt.ResolveUsing<DoubleToStringMoneyResolver>().FromMember(src => src.TotalCalculate))
            ;
            Mapper.CreateMap<TPOFeeReportsSummaryWeeklyDTO, TPOFeeReportsSummaryWeeklyViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.WeekValueString, opt => opt.ResolveUsing<DoubleToStringMoneyResolver>().FromMember(src => src.WeekValue))
                ;

            Mapper.CreateMap<GenerateP1TemplateGLDTO, GenerateP1TemplateGLViewModel>().IgnoreAllNonExisting();
            Mapper.CreateMap<GenerateP1TemplateAPDTO, GenerateP1TemplateAPViewModel>().IgnoreAllNonExisting();

            #endregion

            #region Wages Report Summary
            Mapper.CreateMap<WagesReportSummaryDTO, WagesReportSummaryViewModel>().IgnoreAllNonExisting()
             .ForMember(dest => dest.TotalProduksiString, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TotalProduksi))
             .ForMember(dest => dest.TotalUpahLainString, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TotalUpahLain))
             .ForMember(dest => dest.TotalProduksiCorrectionString, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TotalProduksiCorrection))
             .ForMember(dest => dest.TotalUpahLainCorrectionString, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.TotalUpahLainCorrection))
                 ;
           
            Mapper.CreateMap<WagesReportSummaryProductionCardDTO, WagesReportSummaryProductionCardViewModel>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.Produksi, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.Produksi))
                 .ForMember(dest => dest.UpahLain, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.UpahLain))
                ;
            #endregion


            #region Wages Available Position Number
            Mapper.CreateMap<ExeWagesReportAvailablePositionNumberViewModel, GetWagesReportAvailablePositionNumberViewDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<GetWagesReportAvailablePositionNumberViewDTO, ExeWagesReportAvailablePositionNumberViewModel>().IgnoreAllNonExisting();
            #endregion

            #region TPO Reports Production

            Mapper.CreateMap<TPOFeeReportsProductionDTO, TPOFeeReportsProductionViewModel>().IgnoreAllNonExisting()
                .ForMember(dest => dest.StartDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.ResolveUsing<DateOnlyToStringResolver>().FromMember(src => src.EndDate))
                //.ForMember(dest => dest.Umk, opt => opt.ResolveUsing<DecimalToString2DecimalCommaSeparatorResolver>().FromMember(src => src.UMK))
                //.ForMember(dest => dest.Package, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.Package))
                //.ForMember(dest => dest.JKNProductionFee, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JKNProductionFee))
                //.ForMember(dest => dest.JL1ProductionFee, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JL1ProductionFee))
                //.ForMember(dest => dest.JL2ProductionFee, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JL2ProductionFee))
                //.ForMember(dest => dest.JL3ProductionFee, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JL3ProductionFee))
                //.ForMember(dest => dest.JL4ProductionFee, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JL4ProductionFee))

                //.ForMember(dest => dest.ManagementFee, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.ManagementFee))
                //.ForMember(dest => dest.ProductivityIncentives, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.ProductivityIncentives))

                //.ForMember(dest => dest.JKNProductionVolume, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JKNProductionVolume))
                //.ForMember(dest => dest.JL1ProductionVolume, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JL1ProductionVolume))
                //.ForMember(dest => dest.JL2ProductionVolume, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JL2ProductionVolume))
                //.ForMember(dest => dest.JL3ProductionVolume, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JL3ProductionVolume))
                //.ForMember(dest => dest.JL4ProductionVolume, opt => opt.ResolveUsing<DoubleToString2DecimalCommaSeparatorResolver>().FromMember(src => src.JL4ProductionVolume))


               ;

            #endregion

            #region Multiple Absenteeism
            Mapper.CreateMap<InsertMultipleAbsenteeism, InsertMultipleAbsenteeismDTO>().IgnoreAllNonExisting();
            Mapper.CreateMap<EmployeeMultipleInsertAbsenteeism, EmployeeMultipleInsertAbsenteeismDTO>().IgnoreAllNonExisting();
            #endregion
        }
    }

    public class DateToDayNameResolver : ValueResolver<DateTime, string>
    {
        protected override string ResolveCore(DateTime source)
        {
            if (source.DayOfWeek.ToString() == "Monday")
            {
                return "Senin";
            }
            else if (source.DayOfWeek.ToString() == "Tuesday")
            {
                return "Selasa";
            }
            else if (source.DayOfWeek.ToString() == "Wednesday")
            {
                return "Rabu";
            }
            else if (source.DayOfWeek.ToString() == "Thursday")
            {
                return "Kamis";
            }
            else if (source.DayOfWeek.ToString() == "Friday")
            {
                return "Jumat";
            }
            else if (source.DayOfWeek.ToString() == "Saturday")
            {
                return "Sabtu";
            }
            else if (source.DayOfWeek.ToString() == "Sunday")
            {
                return "Minggu";
            }

            return string.Empty;
        }
    }

    public class SetCheckboxEblekVerificationEntry : ValueResolver<ExeProductionEntryReleaseDTO, bool>
    {
        protected override bool ResolveCore(ExeProductionEntryReleaseDTO source)
        {
            if (source == null)
            {
                return false;
            }

            return true;
        }
    }

    public class SliderNewsInfoTitleResolver : ValueResolver<SliderNewsInfoDTO, string>
    {
        protected override string ResolveCore(SliderNewsInfoDTO slider)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? slider.TitleID : slider.TitleEN;
        }
    }

    public class SliderNewsInfoShortDescriptionResolver : ValueResolver<SliderNewsInfoDTO, string>
    {
        protected override string ResolveCore(SliderNewsInfoDTO slider)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? slider.ShortDescriptionID : slider.ShortDescriptionEN;
        }
    }

    public class NewsInfoCompositeTitleResolver : ValueResolver<NewsInfoCompositeDTO, string>
    {
        protected override string ResolveCore(NewsInfoCompositeDTO slider)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? slider.TitleID : slider.TitleEN;
        }
    }

    public class NewsInfoCompositeShortDescriptionResolver : ValueResolver<NewsInfoCompositeDTO, string>
    {
        protected override string ResolveCore(NewsInfoCompositeDTO slider)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? slider.ShortDescriptionID : slider.ShortDescriptionEN;
        }
    }

    #region NewsInfoDetails Resolver

    public class NewsInfoDetailsTitleResolver : ValueResolver<NewsInfoDTO, string>
    {
        protected override string ResolveCore(NewsInfoDTO m)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? m.TitleID : m.TitleEN;
        }
    }

    public class NewsInfoDetailsShortDescriptionResolver : ValueResolver<NewsInfoDTO, string>
    {
        protected override string ResolveCore(NewsInfoDTO m)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? m.ShortDescriptionID : m.ShortDescriptionEN;
        }
    }

    public class NewsInfoDetailsContentResolver : ValueResolver<NewsInfoDTO, string>
    {
        protected override string ResolveCore(NewsInfoDTO m)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? m.ContentID : m.ContentEN;
        }
    }

    #endregion

    #region InformatipsNewsInfo Resolver

    public class InformatipsNewsInfoTitleResolver : ValueResolver<InformatipsNewsInfoDTO, string>
    {
        protected override string ResolveCore(InformatipsNewsInfoDTO m)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? m.TitleID : m.TitleEN;
        }
    }

    public class InformatipsNewsInfoShortDescriptionResolver : ValueResolver<InformatipsNewsInfoDTO, string>
    {
        protected override string ResolveCore(InformatipsNewsInfoDTO m)
        {
            var lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            return lang == Enums.Languages.id.ToString() ? m.ShortDescriptionID : m.ShortDescriptionEN;
        }
    }

    #endregion

    #region DataDailyProduction

    public class DataDailyReportAchievmentResolver :
        ValueResolver<List<DataDailyProductionAchievmentDTO>, List<DataDailyProductionAchievment>>
    {
        protected override List<DataDailyProductionAchievment> ResolveCore(List<DataDailyProductionAchievmentDTO> m)
        {
            return Mapper.Map<List<DataDailyProductionAchievment>>(m);
        }
        
    }

    public class ExeReportDailyProdReportAchievementResolver :
    ValueResolver<List<GetExeReportDailyProductionAchievementDTO>, List<GetExeReportDailyProductionAchievementViewModel>>
    {
        protected override List<GetExeReportDailyProductionAchievementViewModel> ResolveCore(List<GetExeReportDailyProductionAchievementDTO> m)
        {
            return Mapper.Map<List<GetExeReportDailyProductionAchievementViewModel>>(m);
        }

    }

    public class ExeReportDailyProdReportAchievementBandCodeResolver :
    ValueResolver<List<ExeReportingDailyProductionAchievementDTOBrandCode>, List<ExeReportingDailyProductionAchievementBrandCodeViewModel>>
    {
        protected override List<ExeReportingDailyProductionAchievementBrandCodeViewModel> ResolveCore(List<ExeReportingDailyProductionAchievementDTOBrandCode> m)
        {
            return Mapper.Map<List<ExeReportingDailyProductionAchievementBrandCodeViewModel>>(m);
        }

    }

    public class ExeReportDailyProdReportAchievementSKTBandCodeResolver :
    ValueResolver<List<ExeReportingDailyProductionAchievementDTOSKTBrandCode>, List<ExeReportingDailyProductionAchievementSKTBrandCodeViewModel>>
    {
        protected override List<ExeReportingDailyProductionAchievementSKTBrandCodeViewModel> ResolveCore(List<ExeReportingDailyProductionAchievementDTOSKTBrandCode> m)
        {
            return Mapper.Map<List<ExeReportingDailyProductionAchievementSKTBrandCodeViewModel>>(m);
        }

    }

    #endregion

}
