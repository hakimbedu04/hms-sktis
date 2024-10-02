using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using SKTISWebsite.Models.LookupList;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;

namespace SKTISWebsite.Code
{
    public interface IApplicationService
    {
        SelectList GetBrandGroupCodeSelectList();
        SelectList GetLocationCodeSelectList();

        SelectList GetLocationCodeCompat();
        List<LocationLookupList> GetLocationCodeSelectListByShift(Enums.Shift shift);
        List<LocationLookupList> GetLocationCodeSelectListByLevel(Enums.LocationCode sourceLocation, int level);
        SelectList GetProcessGroupSelectList();
        List<LocationLookupList> GetLocationNamesLookupList();
        SelectList GetPlantChildLocationCode();

        SelectList GetPlantChildLocationCompat();
        SelectList GetUnitCodeSelectListByLocationCode(string locationCode, bool responsibilities = true);
        SelectList GetUnitCodeSelectListByLocationCodeReportByGroup(string locationcode, bool responsibilities = true);
        SelectList GetPlantUnitCodeSelectListByLocationCode(string locationCode);
        SelectList GetProcessGroupSelectListByLocationCode(string locationCode);
        SelectList GetProcessGroupFromProdcard(GetProductionCardInput input);
        SelectList GetProcessSettingIDProcessList();
        List<MandorsLookupList> GetAllMandorLookupList();
        SelectList GetGroupCodeSelectList();
        SelectList GetTPOLocationCodeSelectList();

        SelectList GetTPOLocationCodeSelectListCompat();
        SelectList GetPLNTLocationCodeSelectList();

        SelectList GetPLNTLocationCodeSelectListCompat();
        SelectList GetMonthSelectList();
        SelectList GetWeekSelectList(object selectedValue = null);
        SelectList GetYearSelectList(object selectedValue = null);
        SelectList GetDescendantLocationByLocationCode(string locationCode, int level);
        List<ItemLocationLookupList> GetMaintenanceItemCodeByLocationCode(string locationCode);
        List<ItemLocationLookupList> GetMaintenanceItemCodeByLocationCodeAndType(string locationCode, Enums.ItemType itemType);
        SelectList GetBrandGroupCodeSelectListByParentLocationCode(string locationCode);
        SelectList GetBrandGroupCodeSelectListByPlantProdEntryVerification(string locationCode, string Process, int? KPSYear, int? KPSWeek);
        SelectList GetBrandGroupCodeDestinationSelectList(string BrandFrom);
        List<MstGenProcessSettingDTO> GetProcessGroupCodeSelectListByParentLocationCode(string locationCode);
        List<LocationLookupList> GetChildLocationsLookupList(string parentLocationCode, bool responsibilities = true);
        IEnumerable<SelectListItem> GetGenWeekYears();
        SelectList GetStatusByParentLocationCode(string locationCode);
        List<MstGenEmpStatusCompositeDTO> GetStatusByParentLocationCodeAsValue(string locationCode);
        List<LocationLookupList> GetLastLocationChildList(string parentCode);

        List<LocationLookupList> GetTpoRegSktLocationList();
        SelectList GetLastChildLocationByTPO();
        List<LocationLookupList> GetPlantAndRegionalLocationLookupList(bool responsibility = true);
        List<LocationLookupList> GetAllPlantAndRegionalLocationLookupList();
        SelectList GetRequestNumberSelectList();
        SelectList GetEquipmentRequestRequestors();
        SelectList GetWeekByYear(int year);
        SelectList GetWeekByMonth(int month);
        SelectList GetItemCodeSourceByLocationCodeAndConversionType(string locationCode, bool conversionType, string sourceStatus, DateTime? date);
        SelectList GetItemCodeSourceByLocationCodeAndConversionTypeAndDate(GetMaintenanceEquipmentItemConvertInput input);
        SelectList GetItemCodeDestinationByLocationCodeAndConversionType(string locationCode, string itemCodeSource, bool conversionType);
        SelectList GetGroupFromPlantProductionGroupByLocationUnitProcess(string locationCode, string unit,string process);
        SelectList GetRequestNumberByLocationCode(string locationCode);
        SelectList GetRequestNumberByLocationCodeAndDate(string locationCode, DateTime? requestDate);
        SelectList GetItemCodeFromEquipmentRequestByRequestNumber(string requestNumber);
        SelectList GetItemCodeFromEquipmentRequestByRequestNumberFulfillment(string requestNumber);
        List<ItemLocationLookupList> GetItemCodeByLocationCodeAndTypeNotInEquipmentRepair(string locationCode, Enums.ItemType itemType, DateTime transactionDate);
        List<SparepartDTO> GetSparepartsByItemCode(string itemCode);
        SelectList GetUnitCodeSelectListByLocationCodeSourceAndLocationCodeDestination(string locationCode,string locationDestination);
        SelectList GetReadyToUseAndBadStock(bool conversionType);
        SelectList GetConversion();
        SelectList GetGroupFromExePlantProductionEntryVerificationByLocationUnitAndProcess(string locationCode, string unit, string process);
        SelectList GetGroupBrandExePlantProductionEntryVerificationByLocationUnitAndProcess(string locationCode, string unit, string process);
        SelectList GetGenListByListGroup(string listGroup, object selectedValue = null);
        SelectList GetShiftByLocationCode(string locationCode);
        SelectList GetAllAbsentTypes();
        SelectList GetLocationByUserResponsibility(string locationCode);
        SelectList GetAllProcessGroupFromMstGenProcSettLocAndMstGenProcSettAnfMstGenProcByLocation(string locationCode, int year, int week);

        SelectList GetAllProcessFromExeTPOProductionVerification(string locationCode, int year, int week, DateTime? date);
        SelectList GetAllStatusEmpByLocationCode(string locationCode);
        SelectList GetAllMstGenBrandWithExpireStillActiveByLocationCode(string locationCode);
        SelectList GetSelectListDateByYearAndWeek(int year, int week);
        SelectList GetSelectListNearestClosingPayroll(DateTime now);
        SelectList GetClosingPayrollList(DateTime now, string location, string unit, int revType);
        DateTime GetNearestClosingPayrollBeforeToday(DateTime now);
        SelectList GetMaterialByBrandGroup(string locationCode, string ProcessGroup);
        SelectList GetAllBrandGroupCodeActive();
        List<MstGenProcessSettingDTO> GetProcessGroupListByLocationCode(string locationCode);
        SelectList GetYearClosingPayroll();
        //SelectList GetItemCodeFromInventoryHaveEndingStockByLocation(string location, DateTime? dateFrom, DateTime? dateTo);

        IEnumerable<MaintenanceInventoryDTO> GetItemCodeFromInventoryHaveEndingStockByLocationWithBadStock(string location, DateTime? dateFrom, DateTime? dateTo);
        List<AbsentTypeLookupList> GetAbsentTypeLookupLists();
        SelectList GetBrandCodeFromExePlantProductionEntryVerification(GetExePlantProductionEntryVerificationInput input);
        SelectList GetProcessFromExePlantProductionEntryVerification(GetExePlantProductionEntryVerificationInput input);
        List<AbsentTypeLookupList> GetAbsentTypeLookupListsFroEblek(string location, DateTime date);
        SelectList GetBrandCodeFromProductionCardByLocation(string locationCode, string unit, string process, DateTime productionDate);

        SelectList GetBrandCodeFromProductionCardByLocation(string locationCode, string unit, int shift, string process, int kpsYear, int kpsWeek);
        SelectList GetBrandCodeFromProductionCardByDate(string locationCode, string unit, string shift, DateTime date, string revisiontype);
        SelectList GetGroupCodeFromProductionCardByLocation(string locationCode, string unit, string process, DateTime productionDate, string page);

        SelectList GetAllGroupCodeFromProductionCardByLocation(string locationCode, string unit, DateTime productionDate);

        SelectList GetGroupCodeFromProductionCardByLocation(string locationCode, string unit, int shift, string process, int kpsYear, int kpsWeek);
        List<AbsentTypeLookupList> GetAbsentTypeLookupListsForSuratPeriode();
        SelectList DatePlantWagesSelectList();
        SelectList GetAllRegionals();
        SelectList GetAllProcessGroupTPKPlant(string locationCode);
        string CheckLocationByLocation(string locationCode);
        SelectList GetGroupBrandExeReportByGroupByLocationAndProcess(string locationCode, string process);
        SelectList GetActiveBrandCodeByLocation(string locationCode);
        SelectList GetProcessGroupSelectListByLocationCodeAndBrandGroupCode(string locationCode, string brandGroupCode);
        SelectList GetProcessGroupSelectListByLocationYearWeekFromPlantProdEntryVerification(
            string locationCode,
            int KPSYear,
            int KPSWeek
        );
        List<LocationLookupList> GetPLNTTPOChildLocationsLookupList(bool responsibilities = true);
        #region Roles
        SelectList GetRolesSelectList();
        #endregion

        SelectList GetProcessGroupSelectListByPlanPlantWorkHour(string locationCode, string unitCode,
            string brandGroupCode, string groupCode);

        SelectList GetBrandCodeFromReportDailyAchievment(GetExePlantProductionEntryVerificationInput input);

        SelectList GetBrandCodeUnionEntryVerification(GetExePlantProductionEntryVerificationInput input);
        SelectList GetBrandCodeFromReportByProcess(GetExePlantProductionEntryVerificationInput input);

        SelectList GetLocStatusByParentLocationCode(string locationCode);

        SelectList GetGroupAvailabelPositionNumberByLocationUniShiftAndProcess(string locationCode, string unit,
            string process, string status);

        SelectList GetAdjusmnetTypeList(string brandcode);
        SelectList GetGroupCodeFromPlantEntryVerification(string locationCode, string unitCode, int shift, string processGroup, DateTime productionDate);
        SelectList GetProcessGroupFromPlantEntryVerification(string locationCode, string unitCode, int shift, DateTime productionDate);

        SelectList GetUtilRolesSelectList();
        List<UtilRoleDTO> GetUtilRoles();

        SelectList GetGroupCodeFromPlantEntryVerification(string locationCode, string unitCode, string process, string brandCode, int year, int week);

        SelectList GetShiftFilterByProcess(string locationCode, string unitCode);

        SelectList GetUnitCodeByProcess(string locationCode);
    }
}
