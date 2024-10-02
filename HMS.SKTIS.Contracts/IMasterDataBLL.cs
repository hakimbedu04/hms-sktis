using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using System;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;

namespace HMS.SKTIS.Contracts
{
    public interface IMasterDataBLL
    {
        #region NewsInfo

        List<NewsInfoCompositeDTO> GetNewsInfos();
        List<NewsInfoCompositeDTO> GetNewsInfosHome();
        NewsInfoDTO GetNewsInfoById(int id);
        List<SliderNewsInfoDTO> GetSliderNewsInfo(string locationCode);
        InformatipsNewsInfoDTO GetInformatipsNewsInfo(string locationCode);
        List<MstHolidayDTO> GetHolidayInformation(string locationCode);
        NewsInfoDTO InsertNewsInfo(NewsInfoDTO newsInfo);
        NewsInfoDTO UpdateNewsInfo(NewsInfoDTO newsInfo);
        bool DeleteNewsInfo(int id);
        #endregion

        #region General

        #region General Week
        List<MstGenWeekDTO> GetMstGenWeeks(GetMstGenWeekInput input);
        MstGenWeekDTO GetMstGenWeek(GetMstGenWeekInput input);
        #endregion

        #region General List
        List<MstGeneralListCompositeDTO> GetMstGeneralLists(GetMstGenListsInput input);
        List<MstGeneralListCompositeDTO> GetGeneralListsItemType();
        List<MstGeneralListCompositeDTO> GetGeneralListUOM();
        List<MstGeneralListCompositeDTO> GetGeneralListPriceType();
        List<string> GetListGroups();
        List<KeyValuePair<string, string>> GetListGroupsEnum();
        /// <summary>
        /// Get data source for SelectList
        /// </summary>
        /// <param name="parameter">string[] parameter</param>
        /// <returns></returns>
        List<MstGeneralListCompositeDTO> GetGeneralLists(params string[] parameter);

        MstGeneralListDTO UpdateMstGeneralList(MstGeneralListDTO mstGeneralList);
        MstGeneralListDTO InsertMstGeneralList(MstGeneralListDTO mstGeneralList);

        List<MstGeneralListCompositeDTO> GetGeneralListShift();

        List<MstGeneralListCompositeDTO> GetGeneralListTPORank();
        List<MstGeneralListCompositeDTO> GetGeneralListFunctionType();
        List<MstGeneralListCompositeDTO> GetGeneralListHolidayType();

        #endregion

        #region Location
        MstGenLocation GetMstLocationById(string locationCode);

        List<string> GetHolidayByWeek(MstGenWeekDTO mstGenWeek, string locationCode);

        List<MstGenLocationDTO> GetMstGenLocationsByParentCode(GetMstGenLocationsByParentCodeInput input, bool responsibilities = true);

        List<MstGenLocationDTO> GetMstLocationLists(GetMstGenLocationInput input);
        MstGenLocationDTO GetMstGenLocation(GetMstGenLocationInput input);
        /// <summary>
        /// Get all Location under location code we input as parameter
        /// </summary>
        /// <param name="locationCode">Location code</param>
        /// <param name="level">level (-1) for all child</param>
        /// <returns></returns>
        List<MstGenLocationDTO> GetAllLocationByLocationCode(string locationCode, int level); MstGenLocationDTO InsertLocation(MstGenLocationDTO location);
        MstGenLocationDTO UpdateLocation(MstGenLocationDTO location);
        List<string> GetLocationCodes();
        //Get All Location Code with Status Active and Non Active
        List<string> GetAllLocationCode();
        List<MstGenLocationDTO> GetAllLocationCodeInfo();
        /// <summary>
        /// Get location for dropdown list only code for text dan value
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        List<LocationInfoDTO> GetLocationInfo(string locationCode);

        /// <summary>
        /// Get location for dropdown list code for 'Text' and name for 'value'
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        List<LocationInfoDTO> GetLocationInfoWithName(string locationCode);

        List<MstGenLocationDTO> GetAllLocations(GetAllLocationsInput input, bool responsibilities = true);

        List<int> GetShiftByLocationCode(string locationCode);

        List<MstGenLocationDTO> GetLastChildLocation(string parentCode);
        List<MstGenLocationDTO> GetLastChildLocationByTPO();
        List<MstGenLocationDTO> GetLocationsByLevel(string sourceLocationCode, int level);
        MstGenLocationDTO GetLocation(string locationCode);
        List<MstGenLocationDTO> GetPlantOrTPOLocations(string plantOrTPK);
        List<string> GetPlantLocationCodes();

        #endregion

        #region Standard Hours

        MstGenStandardHourDTO GetStandardHourByDayTypeDay(int day, string dayType);
        List<MstGenStandardHourDTO> GetStandardHours(BaseInput input);
        MstGenStandardHourDTO InsertStandardHour(MstGenStandardHourDTO input);
        MstGenStandardHourDTO UpdateStandardHour(MstGenStandardHourDTO input);
        #endregion

        #region Master Brand

        BrandCompositeDTO GetBrand(GetBrandInput input);
        List<BrandDTO> GetBrands(GetBrandInput input);
        BrandDTO InsertBrand(BrandDTO brand);
        BrandDTO UpdateBrand(BrandDTO brand);
        List<BrandDTO> GetBrandByBrandFamily(string brandFamily = null);
        List<BrandDTO> GetBrandByBrandGroupCode(string brandGroupCode);
        List<BrandDTO> GetAllActiveBrand();
        List<string> GetBrandCodeByLocationCode(string locationCode);
        List<string> GetActiveBrandCodeByLocationCode(string locationCode);
        List<BrandCodeByLocationCodeDTO> GetAllBrandCodeByLocationCode();
        BrandDTO GetMstGenBrandById(string brandCode);
        List<BrandDTO> GetAllMstGenBrandWithExpireStillActive(string locationCode);
        #endregion

        #region Brand Group
        List<MstGenBrandGroupDTO> GetBrandGroups(GetMstGenBrandGroupInput input = null);
        MstGenBrandGroupDTO InsertBrandGroup(MstGenBrandGroupDTO brandGroup);
        MstGenBrandGroupDTO UpdateBrandGroup(MstGenBrandGroupDTO brandGroup);
        List<MstGenBrandGroupDTO> GetActiveBrandGroups();
        MstGenBrandGroup GetBrandGroupById(string brandGroupCode);
        List<MstGenBrandGroupDTO> GetAllBrandGroups();

        string GetBrandGruopCodeByBrandCode(string brandCode);
        #endregion

        #region Process
        List<MstGenProcessDTO> GetMasterProcesses(GetMstGenProcessesInput input);
        MstGenProcessDTO InsertMasterProcess(MstGenProcessDTO processDTO);
        MstGenProcessDTO UpdateMasterProcess(MstGenProcessDTO processDTO);
        List<MstGenProcessDTO> GetAllActiveMasterProcess();
        MstGenProcessDTO GetMasterProcessByIdentifier(string identifierProcess);

        List<MstGenProcessDTO> GetAllMasterProcess();
        MstGenProcessDTO GetMasterProcessByProcess(string process);


        #endregion

        #region Brand Package Item
        List<MstGenBrandPackageItemDTO> GetMstGenBrandPackageItem(MstGenBrandPackageItemInput input);

        //List<MstGenBrandPackageItemDTO> GetReportMstGenBrandPackageItem()
        MstGenBrandPackageItemDTO InsertBrandPackageItem(MstGenBrandPackageItemDTO packageEquipmentDTO);
        MstGenBrandPackageItemDTO UpdateBrandPackageItem(MstGenBrandPackageItemDTO packageEquipmentDTO);
        #endregion

        #region Holiday
        bool IsHoliday(DateTime date, string locationCode);
        List<MstHolidayDTO> GetMstHoliday(MstHolidayInput input);
        MstHolidayDTO InsertHoliday(MstHolidayDTO holidayDTO);
        MstHolidayDTO UpdateHoliday(MstHolidayDTO holidayDTO);
        bool IsHOlidayOrSunday(DateTime date, String locationCode);

        #endregion

        #region Brand Package Mapping
        List<MstGenBrandPkgMappingDTO> GetMstGenBrandPkgMappings();
        List<MstGenBrandPkgMappingDTO> GetBrandGroupCodeDestination(string BrandGroupCodeSource);
        MstGenBrandPkgMappingDTO InsertUpdateMstGenBrandPkgMapping(MstGenBrandPkgMappingDTO brandPackageDTO);
        #endregion

        #region Brand Gruop
        int GetMasterGenBrandGroupPack(string id);
        #endregion

        #endregion

        #region Maintenance

        #region Item
        List<MstMntcItemCompositeDTO> GetMstMaintenanceItems(MstMntcItemInput input);
        MstMntcItemCompositeDTO GetMstMaintenanceItem(MstMntcItemInput input);
        List<MstMntcItemCompositeDTO> GetAllMaintenanceItems();
        MstMntcItemCompositeDTO InsertMaintenanceItem(MstMntcItemCompositeDTO maintenanceItem);
        MstMntcItemCompositeDTO UpdateMaintenanceItem(MstMntcItemCompositeDTO maintenanceItem);
        List<MstMntcItemCompositeDTO> GetMstMaintenanceItemNotEqualItemCode(string itemCode, string itemType);
        List<MstMntcItemCompositeDTO> GetMstMaintenanceItemNotContainItemType(string itemCode);
        List<MstMntcConvertCompositeDTO> GetEqupmentItemDestinationDetail(string sourceItemCode);

        #endregion

        #region Item Location
        List<MstMntcItemLocationDTO> GetMstItemLocations(MstMntcItemLocationInput input);
        MstMntcItemLocationDTO InsertItemLocation(MstMntcItemLocationDTO itemLocationDTO);
        MstMntcItemLocationDTO UpdateItemLocation(MstMntcItemLocationDTO itemLocationDTO);
        List<MstMntcItemLocationDTO> GetItemLocationsByLocationCode(string locationCode);
        List<MstMntcItemLocationDTO> GetItemLocationsByLocationCodeAndType(string locationCode, Enums.ItemType itemType);
        #endregion

        #endregion

        #region MasterTPO
        List<MstTPOInfoDTO> GetMasterTPOInfos(GetMasterTPOInfoInput input);
        List<MstTPOInfoDTO> GetMstTPOInfos(GetMasterTPOInfoInput input);
        MstTPOInfoDTO InsertMasterTPOInfo(MstTPOInfoDTO mstTpodto);
        MstTPOInfoDTO UpdateMasterTPOInfo(MstTPOInfoDTO mstTpodto);
        MstTPOInfoDTO GetMstTpoInfo(string id);

        #endregion

        //List<MstGenBrandGroupDTO> GetBrandGroups(MstGenBrandGroupInput input = null);
        //BrandGroupDTO InsertBrandGroup(BrandGroupDTO brandGroup);
        //BrandGroupDTO UpdateBrandGroup(BrandGroupDTO brandGroup);
        //List<BrandGroupDTO> GetActiveBrandGroups();

        #region Master Brand Group Material

        List<BrandGroupMaterialDTO> GetBrandGroupMaterial(GetBrandGroupMaterialInput input);
        BrandGroupMaterialDTO InsertBrandGroupMaterial(BrandGroupMaterialDTO brandGroupMaterial);
        BrandGroupMaterialDTO UpdateBrandGroupMaterial(BrandGroupMaterialDTO brandGroupMaterialDto);
        BrandGroupMaterialDTO GetMaterialByCode(string materialCode, string brandGroup);

        bool GetMaterialIsTobbaco(string materialCode, string brandCode);
        #endregion

        #region Master TPO Package

        MstTPOPackageDTO GetTpoPackage(GetMstTPOPackagesInput input);
        List<MstTPOPackageDTO> GetTPOPackages(GetMstTPOPackagesInput criteria);
        MstTPOPackageDTO InsertTPOPackage(MstTPOPackageDTO TPOPackage);
        MstTPOPackageDTO UpdateTPOPackage(MstTPOPackageDTO TPOPackage);
        DateTime GetEffectiveDate();

        #endregion

        #region Master Unit

        List<MstPlantUnitCompositeDTO> GetMstPlantUnits(GetMstPlantUnitsInput input);

        MstPlantUnitDTO UpdateMstPlantUnit(MstPlantUnitDTO mstPlantUnit);

        MstPlantUnitDTO InsertMstPlantUnit(MstPlantUnitDTO mstPlantUnit);

        List<MstPlantUnitDTO> GetAllUnits(GetAllUnitsInput input, bool responsibilities = true);
        bool CheckMstPlantUnit(string locationCode, string unitCode);
        #endregion

        #region Master TPO Fee Rate
        List<MstTPOFeeRateDTO> GetTPOFeeRate(MstTPOFeeRateInput input);
        List<MstTPOFeeRateDTO> GetTPOFeeRateByExpiredDate(MstTPOFeeRateInput input);
        MstTPOFeeRateDTO InsertTPOFeeRate(MstTPOFeeRateDTO TPOFeeRate);
        MstTPOFeeRateDTO UpdateTPOFeeRate(MstTPOFeeRateDTO TPOFeeRate);
        #endregion

        #region PLANT

        #region Employee Jobs Data

        List<MstEmployeeJobsDataActiveCompositeDTO> GetMstEmployeeJobsDataActives(
            GetMstEmployeeJobsDataActivesInput input);
        MstEmployeeJobsDataActiveCompositeDTO GetMstEmployeeJobsDataActives(string EmployeeID);

        List<MstEmployeeJobsDataActiveDTO> GetAllEmployeeJobsDataActives(GetAllEmployeeJobsDataActivesInput input);

        List<MstEmployeeJobsDataActiveCompositeDTO> GetMstEmployeeJobsDataActivesForPieceRate(
            GetMstEmployeeJobsDataActivesInput input);

        List<MstEmployeeJobsDataActiveCompositeDTO> GetMstEmployeeJobsDataActivesForDaily(GetMstEmployeeJobsDataActivesInput input);
        #endregion

        #region Production Group
        List<MstPlantProductionGroupCompositeDTO> GetMstPlantProductionGroups(GetMstPlantProductionGroupsInput input);
        List<string> GetGroupCodePlantProductionsByProcess(string process);
        List<string> GetGroupCodePlantProductionByProcessLocationAndUnit(string process, string locationCode, string unitCode);
        MstPlantProductionGroupDTO SaveMstPlantProductionGroup(MstPlantProductionGroupDTO plantProductionGroup);
        List<MstPlantProductionGroupCompositeDTO> GetMasterPlantProductionGroups(GetMstPlantProductionGroupsInput input);
        #endregion

        #endregion

        #region Master Process Setting Location

        List<MstGenProcessSettingsLocationCompositeDTO> GetMstGenProcessSettingLocations(
            GetMstGenProcessSettingLocationInput input);

        MstGenProcessSettingLocationDTO SaveMstGenProcessLocation(
            MstGenProcessSettingLocationDTO processSettingLocationDTO, bool isInsert);

        List<MstGenProcessSettingLocationDTO> GetAllProcessSettingLocations(
            GetAllProcessSettingsLocationsInput input);
        List<MstGenProcessSettingsLocationCompositeDTO> GetMstGenProcessSettingLocationsDistinct(
            GetMstGenProcessSettingLocationInput input);
        int GetStdPerhour(GetMstGenProcessSettingLocationInput input);

        List<MstGenProcessSettingDTO> GetUOMEblekByBrandCode(string brandCode, string locationCode);

        MstGenProcessSettingLocationDTO DeleteProsesSettingLocId(MstGenProcessSettingLocationDTO id);
        #endregion

        #region Master TPO Production Group

        List<MstTPOProductionGroupDTO> GetTPOProductionGroupLists(GetMstTPOProductionGroupInput input);
        MstTPOProductionGroupDTO InsertTPOProductionGroup(MstTPOProductionGroupDTO productionGroupDto);
        MstTPOProductionGroupDTO UpdateTPOProductionGroup(MstTPOProductionGroupDTO productionGroupDto);
        MstTPOProductionGroupDTO GetTpoProductionGroupById(string prodGroup, string processGroup, string locationCode, string status);

        #endregion

        #region Master General Emp Status
        List<MstGenEmpStatusCompositeDTO> GetGenEmpStatusByLocationCode(string locationCode);
        MstGenEmpStatusDTO GetGenEmpStatusByIdentifier(string identifierStatus);
        string GetGenEmpStatusIdentifierByStatusEmp(string statusEmp);
        MstGenEmpStatusDTO GetGenEmpIdentifierByStatus(string status);
        #endregion

        #region Master General Process Setting

        List<MstGenProcessSettingDTO> GetStdStickPerHourByProcessAndBrandGroupCode(string process, string brandGroupCode);
        List<MstGenProcessSettingCompositeDTO> GetMasterProcessSettingByLocationCode(string locationCode);
        List<MstGenProcessSettingDTO> GetMasterProcessSettings(GetMasterProcessSettingsInput input);
        MstGenProcessSettingDTO InsertProcessSetting(MstGenProcessSettingDTO processSettingDto);
        MstGenProcessSettingDTO UpdateProcessSetting(MstGenProcessSettingDTO processSettingDto);
        List<MstProcessBrandViewDTO> GetAllProcessSettingsByBrand(string locationCode, string brandGroupCode);
        List<MstGenProcessSettingDTO> GetAllProcessSettings(GetAllProcessSettingsInput input);
        List<MstGenProcessSettingCompositeDTO> GetAllProcessGroupFromMstGenProcSettLocAndMstGenProcSettAnfMstGenProcByLocation(string locationCode);
        List<int> GetProcessSettingIDProcess(bool increment = false);
        #endregion

        #region Master Plant Absent Type

        List<MstPlantAbsentTypeDTO> GetMstPlantAbsentTypes(GetMstAbsentTypeInput input);
        MstPlantAbsentTypeDTO InsertMstPlantAbsentType(MstPlantAbsentTypeDTO absentTypeDto);
        MstPlantAbsentTypeDTO UpdateMstPlantAbsentType(MstPlantAbsentTypeDTO absentTypeDto);
        List<MstPlantAbsentTypeDTO> GetAllAbsentTypesActiveInEblek();
        MstPlantAbsentTypeDTO GetMstPlantAbsentTypeById(string absentType);
        List<MstPlantAbsentTypeDTO> GetMstPlantAbsentTypesWithoutSLSorSLP(GetMstAbsentTypeInput input);
        List<MstPlantAbsentTypeDTO> GetAllAbsentTypesActiveInAbsent();
        List<MstPlantAbsentTypeDTO> GetAllAbsentTypesActiveInEblekOnly();
        bool GetAllAbsentTypesGetCalCulation(string id);

        List<MstPlantAbsentTypeDTO> GetAllAbsentTypeCalCulation();

        #endregion

        #region Master Maintenance Convert
        List<MstMntcConvertDTO> GetMstMntvConverts(GetMstMntcConvertInput input);
        List<MstMntcConvertDTO> GetMstMntvConvertsForExcel(bool conversionType);
        MstMntcConvertDTO InsertMstMntcConvert(MstMntcConvertDTO mntcConvertDto);
        MstMntcConvertDTO UpdateMstMntcConvert(MstMntcConvertDTO mntcConvertDto);
        MstMntcConvertDTO SaveMstMntcConvertEquipmentDetails(MstMntcConvertDTO mntcConvertDto);
        List<MstMntcConvertDTO> GetMntcConvertByLocationCode(string locationCode);
        MstMntcConvertDTO SaveMstMntcConvertEquipment(MstMntcConvertDTO mntcConvertDto);
        MstMntcConvertDTO GetMasterMaintenanceBySourceAndDestination(string itemCodeSource, string itemCodeDestination);
        List<SparepartDTO> GetSparepartsByItemCode(GetEquipmentRepairItemUsage input);
        List<SparepartDTO> GetTpoSparepartsByItemCode(GetEquipmentRepairItemUsage input);
        #endregion

        #region Master GenWeek

        List<int> GetWeekByYear(int year);
        List<int> Get13Weeks(int year, int startWeek);
        string GetProcessGroup(string locationCode, string unitCode, string groupCode);
        bool IsValidWeek(int year, int week);

        List<MstGenProcessSettingDTO> GetAllProcessSettingByParentLocationCode(string locationCode);

        List<int?> GetGeneralWeekYears();

        int? GetGeneralWeekWeekByDate(DateTime date);
        List<MstGenWeekDTO> GetWeekByMonth(int month);

        MstGenWeekDTO GetWeekByYearAndWeek(int year, int week);
        List<DateTime> GetDateByWeek(int year, int week);
        List<DateTime> GetNearestClosingPayrollDate(DateTime now);
        List<DateTime> GetClosingPayrollOnePeriode(DateTime now);
        DateTime GetClosingPayrollBeforeTodayDateTime(DateTime now);
        DateTime GetClosingPayrollThreePeriodBeforeLastClosingPayroll(DateTime now);
        DateTime GetClosingPayrollAfterTodayDateTime(DateTime now);
        MstGenWeekDTO GetWeekByDate(DateTime date);
        List<MstGenWeekDTO> GetWeekByDateRange(DateTime date, DateTime dateTo);
        MstGenWeekDTO GetDatesFromMonthRange(int StartMonth, int EndMonth, int Year);
        MstGenWeekDTO GetDatesFromWeekRange(int StartMonth, int EndMonth, int Year);
        DateTime GetFirstDateByYearWeek(int year, int week);
        #endregion

        #region Master Gen Brand
        MstGenBrand GetMstGenByBrandCode(string brandCode);
        List<BrandDTO> GetMstGenBrandByBrandCodes(List<string> brandCode);

        List<MstGenBrandGroupDTO> GetSKTBrandCode(string locationCode, string process);
        List<MstGenBrandGroupDTO> GetSktBrand(string locationCode, string process);
        List<MstGenBrandGroupDTO> GetSKTBrandCodeLocation(string locationCode);
        #endregion


        MstClosingPayrollDTO GetMasterClosingPayroll(GetMstClosingPayrollInput input);
        MstClosingPayrollDTO GetMasterClosingPayrollByDate(DateTime? date);

        List<MstClosingPayrollDTO> GetMasterClosingPayrolls(GetMstClosingPayrollInput input);

        MstClosingPayrollDTO SaveMasterClosingPayroll(MstClosingPayrollDTO input);

        MstClosingPayrollDTO DeleteMasterClosingPayroll(MstClosingPayrollDTO input);

        IEnumerable<string> GetAllClosingDatePayroll();

        MstGenWeekDTO GetClosingDatePayrollByYear(int year, int week);

        #region TPOFeeSettingCalculation
        List<TPOFeeSettingCalculationDTO> GetTpoFeeSettingCalculations();
        TPOFeeSettingCalculationDTO UpdateTPOFeeSettingCalculation(TPOFeeSettingCalculationDTO dto);
        #endregion

        List<MstProcessBrandViewDTO> GetAllProcessSettingsByBrandCode(string locationCode, string brandCode);

        MstPlantProductionGroupDTO GetPlantProductionGroupById(string prodGroup, string unitCode, string locationCode, string processGroup);

        List<MstHolidayDTO> GetDateHolidayLookLocation(string locationCode, DateTime date);

        #region For report Available Position Number
        List<MstGenLocStatu> GetGenLocStatusByLocationCode(string locationCode);

        List<LocationInfoDTO> GetLocationInfoParent(string locationCode);

        #region For Adjusment

        List<AdjustmentTypeByBrandCode> GetMstGeneralLists(string brandCode);

        #endregion

        #endregion

        IEnumerable<int> GetShiftFilterByProcess(string location, string unitCode);

    }
}
