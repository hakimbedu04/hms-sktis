using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs.Planning;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Models.LookupList;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;

namespace SKTISWebsite.Code
{
    public class ApplicationService : IApplicationService
    {
        private IMasterDataBLL _masterDataBLL;
        private IMaintenanceBLL _maintenanceBll;
        private IPlanningBLL _planningBll;
        private IPlantWagesExecutionBLL _plantWagesExecutionBll;
        private IUtilitiesBLL _utilitiesBll;
        private IGeneralBLL _generalBll;
        private IExecutionPlantBLL _exePlantBll;
        private IExecutionOtherBLL _exeOtherBll;
        private IExeReportBLL _exeReportBll;

        public ApplicationService(
            IMasterDataBLL masterDataBLL,
            IMaintenanceBLL maintenanceBll,
            IPlanningBLL planningBll,
            IPlantWagesExecutionBLL plantWagesExecutionBll,
            IUtilitiesBLL utilitiesBll,
            IGeneralBLL generalBll,
            IExecutionPlantBLL exePlantBll,
            IExecutionOtherBLL exeOtherBll,
            IExeReportBLL exeReportBll
        )
        {
            _masterDataBLL = masterDataBLL;
            _maintenanceBll = maintenanceBll;
            _planningBll = planningBll;
            _plantWagesExecutionBll = plantWagesExecutionBll;
            _utilitiesBll = utilitiesBll;
            _generalBll = generalBll;
            _exePlantBll = exePlantBll;
            _exeOtherBll = exeOtherBll;
            _exeReportBll = exeReportBll;
        }

        /// <summary>
        /// Gets the brand group code select list.
        /// </summary>
        /// <returns></returns>
        public SelectList GetBrandGroupCodeSelectList()
        {
            var brandGroups = _masterDataBLL.GetAllBrandGroups().Where(m => m.StatusActive == true);
            return new SelectList(brandGroups, "BrandGroupCode", "BrandGroupCode");
        }


        /// <summary>
        /// Gets the location code select list.
        /// </summary>
        /// <returns></returns>
        public SelectList GetLocationCodeSelectList()
        {
            var locations = _masterDataBLL.GetAllLocations(new GetAllLocationsInput());
            return new SelectList(locations, "LocationCode", "LocationCode");
        }

        public SelectList GetLocationCodeCompat()
        {
            var locations = _masterDataBLL.GetAllLocations(new GetAllLocationsInput());
            return new SelectList(locations, "LocationCode", "LocationCompat");
        }

        /// <summary>
        /// Get the location code Location List by shift.
        /// </summary>
        /// <param name="shift">Shift</param>
        /// <returns>List<LocationLookupList></returns>
        public List<LocationLookupList> GetLocationCodeSelectListByShift(Enums.Shift shift)
        {
            var locations = _masterDataBLL.GetAllLocations(new GetAllLocationsInput()).Where(l => l.Shift == (int)shift);
            return Mapper.Map<List<LocationLookupList>>(locations);
        }

        /// <summary>
        /// Get the location code Location List by level.
        /// </summary>
        /// <param name="sourceLocation">Source Location</param>
        /// <param name="level">Level</param>
        /// <returns></returns>
        public List<LocationLookupList> GetLocationCodeSelectListByLevel(Enums.LocationCode sourceLocation, int level)
        {
            var locations = _masterDataBLL.GetLocationsByLevel(sourceLocation.ToString(), level);
            return Mapper.Map<List<LocationLookupList>>(locations);
        }

        /// <summary>
        /// Gets the process group select list.
        /// </summary>
        /// <returns></returns>
        public SelectList GetProcessGroupSelectList()
        {
            var processs = _masterDataBLL.GetAllMasterProcess();
            return new SelectList(processs, "ProcessGroup", "ProcessGroup");
        }

        /// <summary>
        /// Gets the location names lookup list.
        /// </summary>
        /// <returns>LocationNameLookupList (LocationCode, LocationName)</returns>
        public List<LocationLookupList> GetLocationNamesLookupList()
        {
            var locations = _masterDataBLL.GetAllLocations(new GetAllLocationsInput());
            return Mapper.Map<List<LocationLookupList>>(locations);
        }

        public List<LocationLookupList> GetLastLocationChildList(string parentCode)
        {
            var locations = _masterDataBLL.GetLastChildLocation(parentCode);
            return Mapper.Map<List<LocationLookupList>>(locations);
        }

        public List<LocationLookupList> GetTpoRegSktLocationList()
        {
            var Allocations = _masterDataBLL.GetAllLocations(new GetAllLocationsInput());
            var locations = new List<MstGenLocationDTO>();
            foreach (var loc in Allocations)
            {
                if (loc.ParentLocationCode != Enums.LocationCode.PLNT.ToString() && loc.LocationCode != Enums.LocationCode.PLNT.ToString())
                {
                    locations.Add(loc);
                }
            }

            return Mapper.Map<List<LocationLookupList>>(locations);
        }

        /// <summary>
        /// Gets the plant child location code.
        /// </summary>
        /// <returns></returns>
        public SelectList GetPlantChildLocationCode()
        {
            var input = new GetAllLocationsInput() { ParentLocationCode = Enums.LocationCode.PLNT.ToString() };
            var locations = _masterDataBLL.GetAllLocations(input);
            return new SelectList(locations, "LocationCode", "LocationCode");
        }

        public SelectList GetPlantChildLocationCompat()
        {
            var input = new GetAllLocationsInput() { ParentLocationCode = Enums.LocationCode.PLNT.ToString() };
            var locations = _masterDataBLL.GetAllLocations(input);
            return new SelectList(locations, "LocationCode", "LocationCompat");
        }

        /// <summary>
        /// Gets the unit code select list by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public SelectList GetUnitCodeSelectListByLocationCode(string locationCode, bool responsibilities = true)
        {
            var input = new GetAllUnitsInput();
            if (locationCode == Enums.LocationCode.PLNT.ToString())
            {
                input.IgnoreList = new List<string>() { Enums.UnitCodeDefault.MNTC.ToString(), Enums.UnitCodeDefault.PROD.ToString(), Enums.UnitCodeDefault.WHSE.ToString() };
            }
            else
            {
                input.LocationCode = locationCode;
            }


            var units = _masterDataBLL.GetAllUnits(input, responsibilities).Select(p => p.UnitCode).Distinct();

            IDictionary<string, string> dictionaryUnits = new Dictionary<string, string>();

            foreach (var unit in units)
            {
                //if (!input.IgnoreList.Contains(unit))
                    dictionaryUnits.Add(unit, unit);
            }

            return new SelectList(dictionaryUnits, "Key", "Value");
        }

        public SelectList GetUnitCodeSelectListByLocationCodeReportByGroup(string locationCode, bool responsibilities = true)
        {
            var input = new GetAllUnitsInput();
            if (locationCode.Contains(Enums.LocationCode.REG.ToString()))
            {
                input.IgnoreList = new List<string>() { Enums.UnitCodeDefault.MNTC.ToString() };
            }
            else
            {
                input.IgnoreList = new List<string>() { Enums.UnitCodeDefault.MNTC.ToString(), Enums.UnitCodeDefault.WHSE.ToString() };
            }
            
            input.LocationCode = locationCode;

            var units = _masterDataBLL.GetAllUnits(input, responsibilities).Select(p => p.UnitCode).Distinct();

            IDictionary<string, string> dictionaryUnits = new Dictionary<string, string>();

            foreach (var unit in units)
            {
                if (!input.IgnoreList.Contains(unit))
                    dictionaryUnits.Add(unit, unit);
            }

            return new SelectList(dictionaryUnits, "Key", "Value");
        }


        public SelectList GetUnitCodeByProcess(string locationCode) {
            var listUnitCode = _exeReportBll.GetUnitByProcessFilter(locationCode);

            IDictionary<string, string> dictionaryUnits = new Dictionary<string, string>();

            foreach (var unit in listUnitCode) {
                dictionaryUnits.Add(unit, unit);
            }

            return new SelectList(dictionaryUnits, "Key", "Value");
        }

        /// <summary>
        /// Gets the plant unit code select list by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public SelectList GetPlantUnitCodeSelectListByLocationCode(string locationCode)
        {
            var input = new GetAllUnitsInput()
            {
                LocationCode = locationCode,
                IgnoreList = new List<string>() { Enums.UnitCodeDefault.MTNC.ToString(), Enums.UnitCodeDefault.PROD.ToString(), Enums.UnitCodeDefault.WHSE.ToString() }
            };
            var units = _masterDataBLL.GetAllUnits(input).OrderBy(o => o.UnitCode);
            return new SelectList(units, "UnitCode", "UnitCode");
        }

        /// <summary>
        /// Gets the process group select list by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>

        public SelectList GetProcessGroupSelectListByLocationCode(string locationCode)
        {
            var input = new GetAllProcessSettingsInput()
            {
                LocationCode = locationCode
            };
            var processSettings = _masterDataBLL.GetAllProcessSettings(input).Where(c => c.MstGenProcess != null).OrderBy(m => m.MstGenProcess.ProcessIdentifier);
            var processSettingsDistinctByProcessGroup = processSettings.DistinctBy(x => x.ProcessGroup);
            return new SelectList(processSettingsDistinctByProcessGroup, "ProcessGroup", "ProcessGroup");
        }

        /// <summary>
        /// Gets the process group select list from production card.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>

        public SelectList GetProcessGroupFromProdcard(GetProductionCardInput input)
        {
            var processSettings = _plantWagesExecutionBll.GetProcessGroupFromReportByGroup(input);
            var result = processSettings.Select(c => new { ProcessGroup = c });
            return new SelectList(result, "ProcessGroup", "ProcessGroup");
        }

        /// <summary>
        /// Gets the process group select list by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="brandGroupCode">The brand group code.</param>
        /// <returns></returns>
        public SelectList GetProcessGroupSelectListByLocationCodeAndBrandGroupCode(string locationCode, string brandGroupCode)
        {
            var processSettings = _masterDataBLL.GetAllProcessSettingsByBrand(locationCode, brandGroupCode).OrderBy(m => m.ProcessOrder);
            var processSettingsDistinctByProcessGroup = processSettings.DistinctBy(x => x.ProcessGroup);
            return new SelectList(processSettingsDistinctByProcessGroup, "ProcessGroup", "ProcessGroup");
        }

        public SelectList GetProcessGroupSelectListByLocationYearWeekFromPlantProdEntryVerification(
            string locationCode,
            int KPSYear,
            int KPSWeek
        )
        {
            var processSettings = _exePlantBll.GetPlantProductionEntryVerificationByLocationYearWeek(locationCode, KPSYear, KPSWeek);
            var processSettingsDistinctByProcessGroup = processSettings
                .OrderBy(o => o.ProcessOrder)
                .DistinctBy(x => x.ProcessGroup);
            return new SelectList(processSettingsDistinctByProcessGroup, "ProcessGroup", "ProcessGroup");
        }

        /// <summary>
        /// Gets the process group list by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public List<MstGenProcessSettingDTO> GetProcessGroupListByLocationCode(string locationCode)
        {
            var input = new GetAllProcessSettingsInput()
            {
                LocationCode = locationCode
            };
            var processSettings = _masterDataBLL.GetAllProcessSettings(input);
            var processSettingsDistinctByProcessGroup = processSettings.DistinctBy(x => x.ProcessGroup).ToList();
            return processSettingsDistinctByProcessGroup;
        }

        public SelectList GetProcessSettingIDProcessList()
        {
            var idProcess = _masterDataBLL.GetProcessSettingIDProcess();
            return new SelectList(idProcess);
        }

        /// <summary>
        /// Gets all mandor.
        /// </summary>
        /// <returns></returns>
        public List<MandorsLookupList> GetAllMandorLookupList()
        {
            //var input = new GetAllEmployeeJobsDataActivesInput()
            //{
            //    ProcessSettingsCode = Enums.ProcessSettings.Mandor.ToString()
            //};
            var mandors = _masterDataBLL.GetAllEmployeeJobsDataActives(new GetAllEmployeeJobsDataActivesInput());
            mandors = mandors.Where(m => m.Status == Enums.StatusEmployeeJobsData.Mandor.ToString("D")).ToList();
            return Mapper.Map<List<MandorsLookupList>>(mandors);
        }


        /// <summary>
        /// Gets the group code select list.
        /// </summary>
        /// <returns></returns>
        public SelectList GetGroupCodeSelectList()
        {
            var input = new GetAllEmployeeJobsDataActivesInput();
            var employees = _masterDataBLL.GetAllEmployeeJobsDataActives(input);
            var employeesDistinctByGroupCode = employees.DistinctBy(x => x.GroupCode);
            return new SelectList(employeesDistinctByGroupCode, "GroupCode", "GroupCode");
        }

        public SelectList GetTPOLocationCodeSelectList()
        {
            var tpoInfo = _masterDataBLL.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
            {
                ParentLocationCode = "TPO"
            }).Where(x => x.ParentLocationCode == "TPO" || x.LocationCode == "TPO");
            var tpoLocationCodeDisticByLocationCode = tpoInfo;
            return new SelectList(tpoLocationCodeDisticByLocationCode, "LocationCode", "LocationCode");
        }

        public SelectList GetTPOLocationCodeSelectListCompat()
        {
            var tpoInfo = _masterDataBLL.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
            {
                ParentLocationCode = "TPO"
            }).Where(x => x.ParentLocationCode == "TPO" || x.LocationCode == "TPO");
            var tpoLocationCodeDisticByLocationCode = tpoInfo;
            return new SelectList(tpoLocationCodeDisticByLocationCode, "LocationCode", "LocationCompat");
        }

        public SelectList GetPLNTLocationCodeSelectList()
        {
            var plntInfo = _masterDataBLL.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
            {
                ParentLocationCode = Enums.LocationCode.PLNT.ToString()
            }).Where(x => x.ParentLocationCode == Enums.LocationCode.PLNT.ToString() || x.LocationCode == Enums.LocationCode.PLNT.ToString());
            var plntLocationCodeDisticByLocationCode = plntInfo;
            return new SelectList(plntLocationCodeDisticByLocationCode, "LocationCode", "LocationCode");
        }

        public SelectList GetPLNTLocationCodeSelectListCompat()
        {
            var plntInfo = _masterDataBLL.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
            {
                ParentLocationCode = Enums.LocationCode.PLNT.ToString()
            }).Where(x => x.ParentLocationCode == Enums.LocationCode.PLNT.ToString() || x.LocationCode == Enums.LocationCode.PLNT.ToString());
            var plntLocationCodeDisticByLocationCode = plntInfo;
            return new SelectList(plntLocationCodeDisticByLocationCode, "LocationCode", "LocationCompat");
        }

        public SelectList GetMonthSelectList()
        {
            var input = new GetMstGenWeekInput();
            var month = _masterDataBLL.GetMstGenWeeks(input);
            var monthdistinct = month.DistinctBy(m => m.Month);
            return new SelectList(monthdistinct, "Month", "Month");
        }

        public SelectList GetWeekSelectList(object selectedValue = null)
        {
            var result = _masterDataBLL.GetMstGenWeeks(new GetMstGenWeekInput
            {
                Year = DateTime.Now.Year
            }).Select(p => p.Week);

            IDictionary<int?, int?> listWeek = new Dictionary<int?, int?>();

            foreach (var i in result)
            {
                listWeek.Add(i, i);
            }

            return new SelectList(listWeek, "Key", "Value", selectedValue);

        }

        public SelectList GetYearSelectList(object selectedValue = null)
        {
            var result = _masterDataBLL.GetMstGenWeeks(new GetMstGenWeekInput()).OrderByDescending(p => p.Year).Select(p => p.Year).Distinct();

            IDictionary<int?, int?> listYear = new Dictionary<int?, int?>();

            foreach (var i in result)
            {
                listYear.Add(i, i);
            }

            return new SelectList(listYear, "Key", "Value", selectedValue);
        }


        /// <summary>
        /// Gets the descendant location by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public SelectList GetDescendantLocationByLocationCode(string locationCode, int level)
        {
            var locations = _masterDataBLL.GetAllLocationByLocationCode(locationCode, level);

            return new SelectList(locations, "LocationCode", "LocationCode");

        }

        /// <summary>
        /// Gets the maintenance item code by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public List<ItemLocationLookupList> GetMaintenanceItemCodeByLocationCode(string locationCode)
        {
            var itemLocations = _masterDataBLL.GetItemLocationsByLocationCode(locationCode);
            return Mapper.Map<List<ItemLocationLookupList>>(itemLocations);
        }

        public List<ItemLocationLookupList> GetMaintenanceItemCodeByLocationCodeAndType(string locationCode, Enums.ItemType itemType)
        {
            var itemLocations = _masterDataBLL.GetItemLocationsByLocationCodeAndType(locationCode, itemType);
            return Mapper.Map<List<ItemLocationLookupList>>(itemLocations);
        }

        /// <summary>
        /// Gets the brand group code select list by parent location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public SelectList GetBrandGroupCodeSelectListByParentLocationCode(string locationCode)
        {
            var processSettings = _masterDataBLL.GetAllProcessSettingByParentLocationCode(locationCode);
            var processSettingsDistinctByBrandGroupCode = processSettings.DistinctBy(m => m.BrandGroupCode);
            return new SelectList(processSettingsDistinctByBrandGroupCode, "BrandGroupCode", "BrandGroupCode");
        }

        public SelectList GetBrandGroupCodeSelectListByPlantProdEntryVerification(string locationCode, string Process, int? KPSYear, int? KPSWeek)
        {
            var availableBrandCodes = _exePlantBll.GetBrandCodeByLocationYearWeekProcessFromEntryVerification(locationCode, Process, KPSYear, KPSWeek);
            var brandCodes = availableBrandCodes
                .Select(s => s.BrandCode)
                .Distinct()
                .ToList();
            var brandGroupCodes = _masterDataBLL
                .GetMstGenBrandByBrandCodes(brandCodes)
                .OrderBy(o => o.BrandGroupCode)
                .DistinctBy(m => m.BrandGroupCode);

            //var processSettingsDistinctByBrandGroupCode = processSettings.DistinctBy(m => m.BrandGroupCode);
            return new SelectList(brandGroupCodes, "BrandGroupCode", "BrandGroupCode");
        }

        /// <summary>
        /// Get Brand Group Code Destination By Brand Group Code Source
        /// </summary>
        /// <param name="BrandFrom"></param>
        /// <returns></returns>
        public SelectList GetBrandGroupCodeDestinationSelectList(string BrandFrom)
        {
            var brandGroupCodeDestination = _masterDataBLL.GetBrandGroupCodeDestination(BrandFrom);
            return new SelectList(brandGroupCodeDestination, "BrandGroupCodeDestination", "BrandGroupCodeDestination");
        }

        /// <summary>
        /// Gets the process group code select list by parent location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public List<MstGenProcessSettingDTO> GetProcessGroupCodeSelectListByParentLocationCode(string locationCode)
        {
            var processSettings = _masterDataBLL.GetAllProcessSettingByParentLocationCode(locationCode);

            return processSettings.DistinctBy(m => m.ProcessGroup).OrderBy(m => m.ProcessOrder).ToList();
        }

        /// <summary>
        /// Gets the child locations lookup list.
        /// </summary>
        /// <param name="parentLocationCode">The parent location code.</param>
        /// <returns></returns>
        public List<LocationLookupList> GetChildLocationsLookupList(string parentLocationCode, bool responsibilities = true)
        {
            var input = new GetAllLocationsInput() { ParentLocationCode = parentLocationCode };
            var locations = _masterDataBLL.GetAllLocations(input, responsibilities);
            return Mapper.Map<List<LocationLookupList>>(locations);
        }

        /// <summary>
        /// Gets the gen week years.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetGenWeekYears()
        {
            var years = _masterDataBLL.GetGeneralWeekYears();
            return years.Select(i => new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
        }

        public SelectList GetStatusByParentLocationCode(string locationCode)
        {
            var db = _masterDataBLL.GetGenEmpStatusByLocationCode(locationCode);
            var dbGroupByEmpStatus = db.DistinctBy(m => m.StatusEmp);
            return new SelectList(dbGroupByEmpStatus, "StatusIdentifier", "StatusEmp");
        }

        public List<MstGenEmpStatusCompositeDTO> GetStatusByParentLocationCodeAsValue(string locationCode)
        {
            var db = _masterDataBLL.GetGenEmpStatusByLocationCode(locationCode);
            return db.DistinctBy(m => m.StatusEmp).OrderBy(m => m.StatusIdentifier).ToList();
        }

        public SelectList GetLastChildLocationByTPO()
        {
            var locations = _masterDataBLL.GetLastChildLocationByTPO();

            return new SelectList(locations, "LocationCode", "LocationCompat");
        }

        public List<LocationLookupList> GetPlantAndRegionalLocationLookupList(bool responsibility = true)
        {
            var locationLookupList = new List<LocationLookupList>();

            // TPO
            var regionsLocations = GetChildLocationsLookupList(Enums.LocationCode.TPO.ToString(), responsibility);
            locationLookupList.AddRange(regionsLocations);

            // PLNT
            var plantLocations = GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString(), responsibility);
            locationLookupList.AddRange(plantLocations);

            // Regionals
            var tpoLocations = GetChildLocationsLookupList("REG", responsibility).OrderBy(o => o.LocationCode);
            locationLookupList.AddRange(tpoLocations);

            return locationLookupList;
        }

        public List<LocationLookupList> GetAllPlantAndRegionalLocationLookupList()
        {
            var locationLookupList = new List<LocationLookupList>();

            // TPO
            var regionsLocations = GetChildLocationsLookupList(Enums.LocationCode.TPO.ToString(), false);
            locationLookupList.AddRange(regionsLocations);

            // PLNT
            var plantLocations = GetChildLocationsLookupList(Enums.LocationCode.PLNT.ToString(), false);
            locationLookupList.AddRange(plantLocations);

            // Regionals
            var tpoLocations = GetChildLocationsLookupList("REG", false).OrderBy(o => o.LocationCode);
            locationLookupList.AddRange(tpoLocations);

            return locationLookupList;
        }

        private List<LocationLookupList> GetRegionsLocations()
        {
            var regionsLocations = new List<LocationLookupList>();
            var input = new GetAllLocationsInput() { ParentLocationCode = Enums.LocationCode.TPO.ToString() };
            var regions = _masterDataBLL.GetAllLocations(input);
            var regionCodes = regions.Select(x => x.LocationCode);
            foreach (var regionCode in regionCodes)
            {
                var regionLocationList = _masterDataBLL.GetAllLocationByLocationCode(regionCode, 1);
                regionsLocations.AddRange(Mapper.Map<List<LocationLookupList>>(regionLocationList));
            }
            return regionsLocations;
        }

        public SelectList GetRequestNumberSelectList()
        {
            var equipmentRequests = _maintenanceBll.GetEquipmentRequestsTable(new GetEquipmentRequestsInput());
            return new SelectList(equipmentRequests, "RequestNumber", "RequestNumber");
        }

        public SelectList GetEquipmentRequestRequestors()
        {
            var equipmentRequests = _maintenanceBll.GetEquipmentRequestsTable(new GetEquipmentRequestsInput()).DistinctBy(p => p.UpdatedBy);
            return new SelectList(equipmentRequests, "UpdatedBy", "UpdatedBy");
        }

        public SelectList GetWeekByYear(int year)
        {
            var result = _masterDataBLL.GetWeekByYear(year);

            IDictionary<int, int> list = new Dictionary<int, int>();

            foreach (var i in result)
            {
                list.Add(i, i);
            }

            return new SelectList(list, "Key", "Value");
        }

        public SelectList GetWeekByMonth(int month)
        {
            var weeks = _masterDataBLL.GetWeekByMonth(month);
            var weeksDistict = weeks.DistinctBy(m => m.Week);
            return new SelectList(weeksDistict, "Week", "Week");
        }


        public SelectList GetItemCodeSourceByLocationCodeAndConversionType(string locationCode, bool conversionType, string sourceStatus, DateTime? date)
        {
            //if (conversionType)
            //{
            var mstConverts = _masterDataBLL.GetMntcConvertByLocationCode(locationCode);
            var mstInventorys = _maintenanceBll.GetMaintenanceInventorys(new GetMaintenanceInventoryInput { LocationCode = locationCode, ItemStatus = sourceStatus, InventoryDate = date }).Select(m => m.ItemCode);
            var mstInventoryDistict = mstInventorys.Distinct();
            var mstConvertsByConversionType = mstConverts.Where(m => m.ConversionType == conversionType && mstInventoryDistict.Contains(m.ItemCodeSource));
            var mstConvertsByConversionTypeDisticnt = mstConvertsByConversionType.DistinctBy(m => m.ItemCodeSource);
            if (conversionType)
            {
                var mntc = _maintenanceBll.GetMaintenanceEquipmentItemConverts(new GetMaintenanceEquipmentItemConvertInput { ConversionType = true, TransactionDate = date }).Select(m => m.ItemCodeSource);
                mstConvertsByConversionTypeDisticnt = mstConvertsByConversionTypeDisticnt.Where(m => !mntc.Contains(m.ItemCodeSource));
            }
            return new SelectList(mstConvertsByConversionTypeDisticnt, "ItemCodeSource", "ItemCodeSource");
            //}
            //else
            //{
            //    var mstConverts = _masterDataBLL.GetMntcConvertByLocationCode(locationCode);
            //    var mstInventorys = _maintenanceBll.GetMaintenanceInventorys(new GetMaintenanceInventoryInput { LocationCode = locationCode, ItemStatus = sourceStatus }).Select(m => m.ItemCode);
            //    var mstInventoryDistict = mstInventorys.Distinct();
            //    var mstConvertsByConversionType = mstConverts.Where(m => m.ConversionType == conversionType && mstInventoryDistict.Contains(m.ItemCodeSource));
            //    var mstConvertsByConversionTypeDisticnt = mstConvertsByConversionType.DistinctBy(m => m.ItemCodeSource);
            //    return new SelectList(mstConvertsByConversionTypeDisticnt, "ItemCodeSource", "ItemCodeSource");
            //}
        }

        /// <summary>
        /// Gets the type of the item code by location code and conversion.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public SelectList GetItemCodeSourceByLocationCodeAndConversionTypeAndDate(GetMaintenanceEquipmentItemConvertInput input)
        {

            var mstConverts = _masterDataBLL.GetMntcConvertByLocationCode(input.LocationCode);
            var mstConvertsByConversionType = mstConverts.Where(m => m.ConversionType == input.ConversionType);
            var mstConvertsByConversionTypeDisticnt = mstConvertsByConversionType.DistinctBy(m => m.ItemCodeSource);
            return new SelectList(mstConvertsByConversionTypeDisticnt, "ItemCodeSource", "ItemCodeSource");
        }

        /// <summary>
        /// Gets the type of the item code destination by location code and conversion.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="itemCodeSource">The item code source.</param>
        /// <param name="conversionType">if set to <c>true</c> [conversion type].</param>
        /// <returns></returns>
        public SelectList GetItemCodeDestinationByLocationCodeAndConversionType(string locationCode, string itemCodeSource, bool conversionType)
        {
            var mstConverts = _masterDataBLL.GetMntcConvertByLocationCode(locationCode);
            var mstConvertsByConversionType = mstConverts.Where(m => m.ConversionType == conversionType && m.ItemCodeSource == itemCodeSource);
            var mstConvertsByConversionTypeDistinct = mstConvertsByConversionType.DistinctBy(m => m.ItemCodeDestination);
            return new SelectList(mstConvertsByConversionTypeDistinct, "ItemCodeDestination", "ItemCodeDestination");
        }

        /// <summary>
        /// Gets the group from plant production group by location unit process.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public SelectList GetGroupFromPlantProductionGroupByLocationUnitProcess(string locationCode, string unit, string process)
        {
            var dbPlantProductionGroup = _masterDataBLL.GetMasterPlantProductionGroups(new GetMstPlantProductionGroupsInput
                {
                    LocationCode = locationCode,
                    UnitCode = unit,
                    ProcessSettingsCode = process
                });
            return new SelectList(dbPlantProductionGroup, "GroupCode", "GroupCode");
        }

        public SelectList GetGroupCodeFromPlantEntryVerification(string locationCode, string unitCode, string process, string brandCode, int year, int week)
        {
            var dbGroupCodeList = _exePlantBll.GetGroupCodePlantTPK(locationCode, unitCode, process, brandCode, year, week);

            var query = dbGroupCodeList.Select(c => new { GroupCode = c, GroupCodeX = c });
            return new SelectList(query, "GroupCode", "GroupCode");
        }

        public SelectList GetRequestNumberByLocationCode(string locationCode)
        {
            var equipmentRequests = _maintenanceBll.GetEquipmentRequestsTable(new GetEquipmentRequestsInput { LocationCode = locationCode, SortExpression = "RequestDate", SortOrder = "DESC" });
            var equipmentRequestsDistinct = equipmentRequests.DistinctBy(m => m.RequestNumber);
            return new SelectList(equipmentRequestsDistinct, "RequestNumber", "RequestNumber");
        }

        public SelectList GetRequestNumberByLocationCodeAndDate(string locationCode, DateTime? requestDate)
        {
            var equipmentRequests = _maintenanceBll.GetEquipmentRequestsTable(new GetEquipmentRequestsInput { LocationCode = locationCode, RequestDate = requestDate });
            var equipmentRequestsDistinct = equipmentRequests.DistinctBy(m => m.RequestNumber);
            return new SelectList(equipmentRequestsDistinct, "RequestNumber", "RequestNumber");
        }

        public SelectList GetItemCodeFromEquipmentRequestByRequestNumber(string requestNumber)
        {
            var equipmentRequests = _maintenanceBll.GetEquipmentRequestsTable(new GetEquipmentRequestsInput { RequestNumber = requestNumber });
            var equipmentRequestsDistinct = equipmentRequests.DistinctBy(m => m.ItemCode);
            return new SelectList(equipmentRequestsDistinct, "ItemCode", "ItemCode");
        }

        public SelectList GetItemCodeFromEquipmentRequestByRequestNumberFulfillment(string requestNumber)
        {
            var equipmentRequests = _maintenanceBll.GetEquipmentRequestsTable(new GetEquipmentRequestsInput());
            var equipmentRequestsDistinct = equipmentRequests.Where(m => m.RequestNumber == requestNumber && m.Qty - (m.ApprovedQty.HasValue ? m.ApprovedQty : 0) > 0).DistinctBy(m => m.ItemCode);
            //var equipmentRequestsDistinct = equipmentRequests.Where(m => m.RequestNumber == requestNumber).DistinctBy(m => m.ItemCode);
            return new SelectList(equipmentRequestsDistinct, "ItemCode", "ItemCode");
        }

        public List<ItemLocationLookupList> GetItemCodeByLocationCodeAndTypeNotInEquipmentRepair(string locationCode, Enums.ItemType itemType, DateTime transactionDate)
        {
            var itemLocations = _masterDataBLL.GetItemLocationsByLocationCodeAndType(locationCode, itemType);
            var itemCodeRepairs = _maintenanceBll.GetTPOEquipmentRepairs(new GetPlantEquipmentRepairsTPOInput { LocationCode = locationCode, TransactionDate = transactionDate }).Select(m => m.ItemCode);
            return Mapper.Map<List<ItemLocationLookupList>>(itemLocations.Where(m => !itemCodeRepairs.Contains(m.ItemCode)));
        }

        public List<SparepartDTO> GetSparepartsByItemCode(string itemCode)
        {
            //Edit Data
            return _maintenanceBll.GetAllRepairItemUsages(new GetRepairItemUsageInput { ItemCodeSource = itemCode });
            //return Mapper.Map<List<SparepartDTO>>(dbUsage);


            //New Data
            //var dbConvert = _masterDataBLL.GetSparepartsByItemCode(itemCode);
            //return Mapper.Map<List<SparepartDTO>>(dbConvert);
        }

        public SelectList GetUnitCodeSelectListByLocationCodeSourceAndLocationCodeDestination(string locationCode, string locationDestination)
        {
            var units = new List<MstPlantUnitDTO>();
            if (locationCode == locationDestination)
            {
                var input = new GetAllUnitsInput()
                {
                    LocationCode = locationCode,
                    IgnoreList = new List<string>() { Enums.UnitCodeDefault.MTNC.ToString() }
                };
                units = _masterDataBLL.GetAllUnits(input, false);
                return new SelectList(units, "UnitCode", "UnitCode");
            }
            else
            {
                units = _masterDataBLL.GetAllUnits(new GetAllUnitsInput(), false);
                if (locationDestination.Contains(Enums.LocationCode.REG.ToString()))
                {
                    return new SelectList(units.Where(m => m.UnitCode == Enums.UnitCodeDefault.WHSE.ToString()).DistinctBy(m => m.UnitCode), "UnitCode", "UnitCode");
                }
                else
                {
                    return new SelectList(units.Where(m => m.UnitCode == Enums.UnitCodeDefault.MTNC.ToString()).DistinctBy(m => m.UnitCode), "UnitCode", "UnitCode");
                }
            }

        }

        public SelectList GetReadyToUseAndBadStock(bool conversionType)
        {
            var filter = conversionType ?
                new List<string> { EnumHelper.GetDescription(Enums.ItemStatus.ReadyToUse), EnumHelper.GetDescription(Enums.ItemStatus.BadStock) } :
                new List<string> { EnumHelper.GetDescription(Enums.ItemStatus.ReadyToUse) };
            return new SelectList(filter);
        }

        public SelectList GetConversion()
        {
            var conversions = _masterDataBLL.GetMstGeneralLists(new GetMstGenListsInput
            {
                ListGroup = Enums.MasterGeneralList.CigUOM.ToString(),
                ListDetail = new[] { Enums.Conversion.Box.ToString(), Enums.Conversion.Stick.ToString() }
            });

            return new SelectList(conversions, "ListDetail", "ListDetail", Enums.Conversion.Box.ToString());
        }

        public SelectList GetGroupFromExePlantProductionEntryVerificationByLocationUnitAndProcess(string locationCode, string unit, string process)
        {
            var input = new GetPlanningPlantIndividualCapacityByReferenceInput
            {
                Location = locationCode,
                Unit = unit,
                Process = process
            };
            var exePlantProductionEntryVerivications = _planningBll.GeteExePlantProductionEntryVerification(input);
            var exePlantProductionEntryVerivicationsDistinct = exePlantProductionEntryVerivications.DistinctBy(m => m.GroupCode).OrderBy(m => m.GroupCode);

            var realOnly = new List<ExePlantProductionEntryVerificationDTO>();
            foreach (var row in exePlantProductionEntryVerivicationsDistinct)
            {
                var sub = row.GroupCode.Substring(1, 1);
                if (sub != "5")
                {
                    realOnly.Add(row);
                }
            }
            return new SelectList(realOnly, "GroupCode", "GroupCode");
        }

        public SelectList GetGroupCodeFromPlantEntryVerification(string locationCode, string unitCode, int shift, string processGroup, DateTime productionDate) 
        {
            var listVerifications = _exePlantBll.GetVerificationForFilterGroupCode(locationCode, unitCode, shift, processGroup, productionDate);

            var realOnly = new List<ExePlantProductionEntryVerificationViewDTO>();
            foreach (var item in listVerifications)
            {
                var sub = item.GroupCode.Substring(1, 1);
                if (sub != "5")
                {
                    realOnly.Add(item);
                }
            }

            return new SelectList(realOnly.DistinctBy(c => c.GroupCode).OrderBy(c => c.GroupCode), "GroupCode", "GroupCode");
        }

        public SelectList GetGroupBrandExePlantProductionEntryVerificationByLocationUnitAndProcess(string locationCode, string unit, string process)
        {
            var input = new GetPlanningPlantIndividualCapacityByReferenceInput
            {
                Location = locationCode,
                Unit = unit,
                Process = process
            };
            var exePlantProductionEntryVerivications = _planningBll.GeteExePlantProductionEntryVerification(input);
            var exePlantProductionEntryVerivicationsDistinct = exePlantProductionEntryVerivications.DistinctBy(m => m.BrandCode);
            return new SelectList(exePlantProductionEntryVerivicationsDistinct, "BrandCode", "BrandCode");
        }

        public SelectList GetGenListByListGroup(string listGroup, object selectedValue = null)
        {
            var conversions = _masterDataBLL.GetMstGeneralLists(new GetMstGenListsInput
            {
                ListGroup = listGroup
            });

            if (selectedValue != null)
            {
                return new SelectList(conversions, "ListDetail", "ListDetail", selectedValue);
            }

            return new SelectList(conversions, "ListDetail", "ListDetail");
        }

        public SelectList GetShiftByLocationCode(string locationCode)
        {
            var listShift = _masterDataBLL.GetShiftByLocationCode(locationCode);

            IDictionary<int, int> shifts = new Dictionary<int, int>();

            foreach (var data in listShift)
            {
                shifts.Add(data, data);
            }

            return new SelectList(shifts, "Key", "Value");

        }

        public SelectList GetShiftFilterByProcess(string locationCode, string unitCode) {
            var listShift = _masterDataBLL.GetShiftFilterByProcess(locationCode, unitCode);
            IDictionary<int, int> shiftResult = new Dictionary<int, int>();

            foreach (var item in listShift) {
                shiftResult.Add(item, item);
            }
            return new SelectList(shiftResult, "Key", "Value");
        }

        public SelectList GetAllAbsentTypes()
        {
            var absentTypes = _masterDataBLL.GetAllAbsentTypesActiveInEblek();
            return new SelectList(absentTypes, "AbsentType", "AbsentType");
        }
        public SelectList GetLocationByUserResponsibility(string locationCode)
        {
            var locationList = _masterDataBLL.GetMstLocationLists(new GetMstGenLocationInput
            {
                ParentLocationCode = locationCode
            });
            return new SelectList(locationList, "LocationCode", "LocationCode");
        }

        public string CheckLocationByLocation(string locationCode)
        {
            var locationList = _masterDataBLL.GetMstLocationLists(new GetMstGenLocationInput
            {
                LocationCode = locationCode
            });
            var check = locationList.FirstOrDefault().ParentLocationCode;

            return check;
        }

        public SelectList GetAllProcessGroupFromMstGenProcSettLocAndMstGenProcSettAnfMstGenProcByLocation(string locationCode, int year, int week)
        {
            //var mstGenProcessSettingLocations = _masterDataBLL.GetAllProcessGroupFromMstGenProcSettLocAndMstGenProcSettAnfMstGenProcByLocation(locationCode);
            // take from PlanTPOTargetProductionKelompok http://tp.voxteneo.com/entity/59002
            var distict = _planningBll.GetTPOTPKProcessByLocations(locationCode, year, week);
            return new SelectList(distict);
        }

        public SelectList GetAllProcessFromExeTPOProductionVerification(string locationCode, int year, int week, DateTime? date)
        {
            var process = _planningBll.GetAllProcessFromExeTPOProductionVerificationByLocationCodeAndDate(locationCode, year, week, date);
            return new SelectList(process);
        }

        public SelectList GetAllStatusEmpByLocationCode(string locationCode)
        {
            var datas = _masterDataBLL.GetGenEmpStatusByLocationCode(locationCode);
            var dataDistict = datas.DistinctBy(m => m.StatusEmp);
            return new SelectList(dataDistict, "StatusEmp", "StatusEmp");
        }

        public SelectList GetAllMstGenBrandWithExpireStillActiveByLocationCode(string locationCode)
        {
            var datas = _masterDataBLL.GetAllMstGenBrandWithExpireStillActive(locationCode);
            return new SelectList(datas, "BrandCode", "BrandCode");
        }

        public SelectList GetSelectListDateByYearAndWeek(int year, int week)
        {
            var datas = _masterDataBLL.GetDateByWeek(year, week).Select(m => m.Date.ToString(Constants.DefaultDateOnlyFormat));

            IDictionary<string, string> listDate = new Dictionary<string, string>();

            foreach (var data in datas)
            {
                listDate.Add(data, data);
            }

            return new SelectList(listDate, "Key", "Value");
        }

        public SelectList GetSelectListNearestClosingPayroll(DateTime now)
        {
            var datas = _masterDataBLL.GetNearestClosingPayrollDate(now).Select(m => m.Date.ToString(Constants.DefaultDateOnlyFormat));

            IDictionary<string, string> listDate = new Dictionary<string, string>();

            foreach (var data in datas)
            {
                listDate.Add(data, data);
            }

            return new SelectList(listDate, "Key", "Value");

        }

        public SelectList GetClosingPayrollList(DateTime now, string location, string unit, int revType)
        {
            IDictionary<string, string> listDate = new Dictionary<string, string>();
            var datas = new List<string>();
            var resultJSonSubmitData = "";
            var statusResult = "";
            var listResultJSon = new List<string>();
            try
            {
                datas = _masterDataBLL.GetClosingPayrollOnePeriode(now).Select(m => m.Date.ToString(Constants.DefaultDateOnlyFormat)).ToList();
            }
            catch (Exception ex)
            {
                resultJSonSubmitData = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.UndefinedLastPayrollDate);
                statusResult = "error";
                listResultJSon.Add(resultJSonSubmitData);
                listResultJSon.Add(statusResult);
                return new SelectList(listResultJSon);
            }
            
            if (revType != 0)
            {
                var dateCorrections = _plantWagesExecutionBll.GetProductionCardApproval(location, unit, revType).Select(p => p.ProductionDate).Distinct().ToList(); ;

                var list = new List<DateTime>();
                var endDate = _masterDataBLL.GetClosingPayrollBeforeTodayDateTime(DateTime.Now.Date);
                var startDate = _masterDataBLL.GetClosingPayrollBeforeTodayDateTime(endDate.Date).AddDays(1);
                for (int i = 0; i < 7; i++)
                {
                    list.Add(startDate.AddDays(i));
                }

                foreach (var dc in dateCorrections)
                {
                    if (list.Contains(dc))
                        listDate.Add(dc.ToString(Constants.DefaultDateOnlyFormat), dc.ToString(Constants.DefaultDateOnlyFormat));
                }
            }

            foreach (var data in datas)
            {
                if (!listDate.ContainsKey(data))
                //listDate.Select(c => c.Key).ToList();
                listDate.Add(data, data);
            }

            return new SelectList(listDate, "Key", "Value");
        }

        public DateTime GetNearestClosingPayrollBeforeToday(DateTime now)
        {
            var date = _masterDataBLL.GetClosingPayrollBeforeTodayDateTime(now);

            return date;
        }

        public SelectList GetMaterialByBrandGroup(string brandGroup, string ProcessGroup)
        {
            //var datas = _masterDataBLL.GetBrandGroupMaterial(new GetBrandGroupMaterialInput { Location = locationCode });
            var datas = _masterDataBLL.GetBrandGroupMaterial(new GetBrandGroupMaterialInput
            {
                BrandGroupCode = brandGroup,
                ProcessGroup = ProcessGroup
            });

            return new SelectList(datas, "MaterialCode", "MaterialCode");
        }

        public SelectList GetAllBrandGroupCodeActive()
        {
            var datas = _masterDataBLL.GetActiveBrandGroups();
            var dataDistict = datas.DistinctBy(m => m.BrandGroupCode);
            return new SelectList(dataDistict, "BrandGroupCode", "BrandGroupCode");
        }

        public SelectList GetYearClosingPayroll()
        {
            var data = _masterDataBLL.GetMasterClosingPayrolls(new GetMstClosingPayrollInput()).OrderByDescending(x => x.ClosingDate).Select(p => p.ClosingDate.Year).Distinct();
            IDictionary<int, int> list = new Dictionary<int, int>();

            foreach (var i in data)
            {
                list.Add(i, i);
            }

            return new SelectList(list, "Key", "Value");
        }

        //public SelectList GetItemCodeFromInventoryHaveEndingStockByLocation(string location, DateTime? dateFrom, DateTime? dateTo )
        //{

            //var dbItemDisposal = _maintenanceBll.GetMntcEquipmentItemDisposals(new GetItemDisposalInput
            //{
            //    LocationCode = location,
            //    DateFrom = dateFrom,
            //    DateTo = dateTo
            //});

            //var dbInventory = _maintenanceBll.GetMntcInventoryAllViewDisposal(new GetMaintenanceInventoryInput
            //{
            //    LocationCode = location,
            //    ItemStatus = EnumHelper.GetDescription(Enums.ItemStatus.BadStock),
            //    UnitCode = Enums.UnitCodeDefault.MTNC.ToString(),
            //    InventoryDate = dateTo
            //    //InventoryDate = date
            //}).Where(m => m.EndingStock > 0 && !dbItemDisposal.Select(p => p.ItemCode).Contains(m.ItemCode));
            //return new SelectList(dbInventory, "ItemCode", "ItemCode");
        //}

        public IEnumerable<MaintenanceInventoryDTO> GetItemCodeFromInventoryHaveEndingStockByLocationWithBadStock(string location, DateTime? dateFrom, DateTime? dateTo)
        {

            var dbItemDisposal = _maintenanceBll.GetMntcEquipmentItemDisposals(new GetItemDisposalInput
            {
                LocationCode = location,
                DateFrom = dateFrom,
                DateTo = dateTo
            });

            //var deltaView = _maintenanceBll.GetDeltaView(location, dateTo.GetValueOrDefault()).Where(
            //        m=> m.ItemStatus == EnumHelper.GetDescription(Enums.ItemStatus.BadStock).ToUpper()
            //    );

            var dbInventory = _maintenanceBll.GetMntcInventoryAllViewDisposal(new GetMaintenanceInventoryInput
                {
                    LocationCode = location,
                    ItemStatus = EnumHelper.GetDescription(Enums.ItemStatus.BadStock),
                    UnitCode = Enums.UnitCodeDefault.MTNC.ToString(),
                    InventoryDate = dateTo
                    //InventoryDate = date
                }).Where(m => m.EndingStock > 0 
                    && !dbItemDisposal.Select(p => p.ItemCode).Contains(m.ItemCode));

            //foreach (var inv in dbInventory)
            //{
            //    var a = deltaView.ToList().Where(m => m.ItemCode == inv.ItemCode);
            //    if (a.Any())
            //    {
            //        inv.EndingStock += a.Sum(c => c.DEndingStock);
            //    }

            //}

            return (dbInventory.ToList());
            //return new SelectList(dbInventory, "ItemCode", "ItemCode");
        }

        public SelectList GetBrandCodeFromExePlantProductionEntryVerification(
            GetExePlantProductionEntryVerificationInput input)
        {
            var listBrandCode = _planningBll.GeteExePlantProductionEntryVerification(input).OrderBy(p => p.BrandCode).Select(p => p.BrandCode).Distinct();

            IDictionary<string, string> dictionaryBrand = new Dictionary<string, string>();

            foreach (var data in listBrandCode)
            {
                dictionaryBrand.Add(data, data);
            }

            return new SelectList(dictionaryBrand, "Key", "Value");
        }

        public SelectList GetProcessFromExePlantProductionEntryVerification(GetExePlantProductionEntryVerificationInput input)
        {
            //var listMst = _masterDataBLL
            var processSettings = _masterDataBLL.GetAllMasterProcess().OrderBy(p => p.ProcessIdentifier).Select(p => p.ProcessGroup);
            var listProcess = _planningBll.GeteExePlantProductionEntryVerification(input).Select(p => p.ProcessGroup).Distinct();
            var processResult = new List<string>();
            foreach (var processOrder in processSettings)
            {
                if (listProcess.Contains(processOrder))
                {
                    processResult.Add(processOrder);
                }

            }
            IDictionary<string, string> dictionaryProcess = new Dictionary<string, string>();

            foreach (var data in processResult)
            {
                dictionaryProcess.Add(data, data);
            }

            return new SelectList(dictionaryProcess, "Key", "Value");
        }

        public List<AbsentTypeLookupList> GetAbsentTypeLookupLists()
        {
            var absentTypes = _masterDataBLL.GetMstPlantAbsentTypes(new GetMstAbsentTypeInput());
            return Mapper.Map<List<AbsentTypeLookupList>>(absentTypes);
        }

        public List<AbsentTypeLookupList> GetAbsentTypeLookupListsFroEblek(string location, DateTime date)
        {
            var absentTypes = _masterDataBLL.GetAllAbsentTypesActiveInEblek();
            var holiday = _masterDataBLL.GetDateHolidayLookLocation(location, date);

            var absentHolidayOrSunday = new List<MstPlantAbsentTypeDTO>();

            if (holiday.Count != 0)
            {
                absentHolidayOrSunday.AddRange(absentTypes.Where(item => item.AbsentType == EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HA)
                    || item.AbsentType == EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HC)
                    || item.AbsentType == EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HI)
                    || item.AbsentType == EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HS)).ToList());

                return Mapper.Map<List<AbsentTypeLookupList>>(absentHolidayOrSunday);
            }

            if (date.DayOfWeek == 0)
            {
                absentHolidayOrSunday.AddRange(absentTypes.Where(item => item.AbsentType == EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HA)
                   || item.AbsentType == EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HC)
                   || item.AbsentType == EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HI)
                   || item.AbsentType == EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HS)).ToList());

                return Mapper.Map<List<AbsentTypeLookupList>>(absentHolidayOrSunday);
            }

            if ((int)date.DayOfWeek == 6)
            {
                var absentTypeNew = new List<MstPlantAbsentTypeDTO>();

                foreach (var mstPlantAbsentTypeDto in absentTypes)
                {
                    if (!String.IsNullOrEmpty(mstPlantAbsentTypeDto.Calculation))
                    {
                        if (!mstPlantAbsentTypeDto.Calculation.Contains("*"))
                        {
                            absentTypeNew.Add(mstPlantAbsentTypeDto);
                        }
                    }
                    else
                    {
                        absentTypeNew.Add(mstPlantAbsentTypeDto);
                    }
                }
                return Mapper.Map<List<AbsentTypeLookupList>>(absentTypeNew).Where(a => a.AbsentType != EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HA) &&
                    a.AbsentType != EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HI) &&
                    a.AbsentType != EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HS) &&
                    a.AbsentType != EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HC)).ToList();
            }


            return Mapper.Map<List<AbsentTypeLookupList>>(absentTypes).Where(a => a.AbsentType != EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HA) &&
                a.AbsentType != EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HI) &&
                a.AbsentType != EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HS) &&
                a.AbsentType != EnumHelper.GetDescription(Enums.AbsenTypeHoliday.HC)).ToList();
        }

        public SelectList GetGroupCodeFromProductionCardByLocation(string locationCode, string unit, string process, DateTime productionDate, string page)
        {
            var exeProdEntVerification = new List<ExePlantProductionEntryVerificationDTO>();
            if (page == "ProdCard")
            {
                if (locationCode == Enums.LocationCode.PLNT.ToString())
                    exeProdEntVerification =
                        _planningBll.GeteExePlantProductionEntryVerificationBll(
                            new GetExePlantProductionEntryVerificationInput
                            {
                                UnitCode = unit,
                                ProcessGroup = process,
                                ProductionDate = productionDate
                            });
                else
                    exeProdEntVerification =
                        _planningBll.GeteExePlantProductionEntryVerificationBll(
                            new GetExePlantProductionEntryVerificationInput
                            {
                                LocationCode = locationCode,
                                UnitCode = unit,
                                ProcessGroup = process,
                                ProductionDate = productionDate
                            });

            }
            else
            {
                exeProdEntVerification = _planningBll.GeteExePlantProductionEntryVerificationBll(locationCode == Enums.LocationCode.PLNT.ToString() ? new GetExePlantProductionEntryVerificationInput { UnitCode = unit, ProcessGroup = process} : new GetExePlantProductionEntryVerificationInput { LocationCode = locationCode, UnitCode = unit, ProcessGroup = process });
            }
            var dataDistinct = exeProdEntVerification.Where(c => c.GroupCode != null && c.GroupCode[1] != '5').DistinctBy(m => m.GroupCode);
               
            
            //var prodCards = _plantWagesExecutionBll.GetProductionCards(new GetProductionCardInput { LocationCode = locationCode, Unit = unit, Process = process });
            //var dataDistict = prodCards.DistinctBy(m => m.GroupCode);
            return new SelectList(dataDistinct, "GroupCode", "GroupCode");
        }

        //get all group , page prodcard (popup submit)
        public SelectList GetAllGroupCodeFromProductionCardByLocation(string locationCode, string unit, DateTime productionDate)
        {
            List<ExePlantProductionEntryVerificationDTO> exeProdEntryVerification = _planningBll.GeteExePlantProductionEntryVerificationBll(locationCode == Enums.LocationCode.PLNT.ToString() ? new GetExePlantProductionEntryVerificationInput { UnitCode = unit, ProductionDate = productionDate} : new GetExePlantProductionEntryVerificationInput { LocationCode = locationCode, UnitCode = unit, ProductionDate = productionDate});
            var dataDistinct = exeProdEntryVerification.Where(c => c.GroupCode != null && c.GroupCode[1] != '5').DistinctBy(m => m.GroupCode).OrderBy(c => c.GroupCode);
            return new SelectList(dataDistinct, "GroupCode", "GroupCode");
        }

        public SelectList GetGroupCodeFromProductionCardByLocation(string locationCode, string unit,int shift, string process, int kpsYear, int kpsWeek)
        {
            var exeProdEntVerification = _planningBll.GeteExePlantProductionEntryVerification(new GetExePlantProductionEntryVerificationInput { LocationCode = locationCode, UnitCode = unit, Shift = shift, ProcessGroup = process, KpsYear = kpsYear, KpsWeek = kpsWeek });
            var dataDistinct = exeProdEntVerification.Where(c => c.GroupCode != null && c.GroupCode[1] != '5').DistinctBy(m => m.GroupCode);
            //var prodCards = _plantWagesExecutionBll.GetProductionCards(new GetProductionCardInput { LocationCode = locationCode, Unit = unit, Process = process });
            //var dataDistict = prodCards.DistinctBy(m => m.GroupCode);
            return new SelectList(dataDistinct, "GroupCode", "GroupCode");
        }

        public SelectList GetBrandCodeFromProductionCardByLocation(string locationCode, string unit, string process, DateTime productionDate)
        {
            var exeProdEntVerification = _planningBll.GeteExePlantProductionEntryVerificationBll(new GetExePlantProductionEntryVerificationInput { LocationCode = locationCode, UnitCode = unit, ProcessGroup = process, ProductionDate = productionDate });
            //var exeProdEntVerification = _planningBll.GeteExePlantProductionEntryVerification(new GetExePlantProductionEntryVerificationInput { LocationCode = locationCode, UnitCode = unit, ProcessGroup = process, ProductionDate = productionDate });
            var dataDistinct = exeProdEntVerification.DistinctBy(m => m.BrandCode);
            //var prodCards = _plantWagesExecutionBll.GetProductionCards(new GetProductionCardInput { LocationCode = locationCode, Unit = unit, Process = process });
            //var dataDistict = prodCards.DistinctBy(m => m.BrandCode);
            return new SelectList(dataDistinct, "BrandCode", "BrandCode");
        }

        public SelectList GetBrandCodeFromProductionCardByLocation(string locationCode, string unit, int shift, string process, int kpsYear, int kpsWeek)
        {
            var exeProdEntVerification = _planningBll.GeteExePlantProductionEntryVerification(new GetExePlantProductionEntryVerificationInput { LocationCode = locationCode, UnitCode = unit, Shift = shift, ProcessGroup = process, KpsYear = kpsYear, KpsWeek = kpsWeek });
            var dataDistinct = exeProdEntVerification.DistinctBy(m => m.BrandCode);
            //var prodCards = _plantWagesExecutionBll.GetProductionCards(new GetProductionCardInput { LocationCode = locationCode, Unit = unit, Process = process });
            //var dataDistict = prodCards.DistinctBy(m => m.BrandCode);
            return new SelectList(dataDistinct, "BrandCode", "BrandCode");
        }

        public SelectList GetBrandCodeFromProductionCardByDate(string locationCode, string unit, string shift, DateTime date, string revisiontype)
        {
            var prodCard =
                _plantWagesExecutionBll.GetProductionCardsBrandGroupCode(new GetProductionCardInput
                {
                    LocationCode = locationCode,
                    Unit = unit,
                    Shift = Convert.ToInt32(shift),
                    Date = date,
                    RevisionType = Convert.ToInt32(revisiontype)
                });
            var distinctBrand = prodCard.Select(c => c.BrandGroupCode).Distinct();
            var listBrandCode = distinctBrand.Select(c => new { BrandGroupCode = c });
            return new SelectList(listBrandCode, "BrandGroupCode", "BrandGroupCode");
        }

        public List<AbsentTypeLookupList> GetAbsentTypeLookupListsForSuratPeriode()
        {
            var absentTypes = _masterDataBLL.GetMstPlantAbsentTypes(new GetMstAbsentTypeInput()).Where(m => m.PayrollAbsentCode != "" && m.AlphaReplace != "" && m.AlphaReplace != null);
            return Mapper.Map<List<AbsentTypeLookupList>>(absentTypes);
        }

        public SelectList DatePlantWagesSelectList()
        {
            var mstGenWeek = _masterDataBLL.GetMstGenWeek(new GetMstGenWeekInput
            {
                CurrentDate = DateTime.Now
            });

            var mstClosingPayroll = _masterDataBLL.GetMasterClosingPayroll(new GetMstClosingPayrollInput
            {
                StartDate = mstGenWeek.StartDate,
                EndDate = mstGenWeek.EndDate
            });

            var mstClosingPayrolls =
                    _masterDataBLL.GetMasterClosingPayrolls(new GetMstClosingPayrollInput())
                        .OrderByDescending(p => p.ClosingDate);

            if (mstClosingPayroll == null)
            {
                if (mstClosingPayrolls.Count() >= 2)
                {
                    IDictionary<DateTime, DateTime> dictionaryDate =
                    _generalBll.EachDay(mstClosingPayrolls.ElementAt(1).ClosingDate.Date, DateTime.Now.Date)
                        .ToDictionary(p => p.Date);

                    return new SelectList(dictionaryDate, "Key", "Value");
                }

            }

            if (mstClosingPayroll.ClosingDate.Date < DateTime.Now.Date)
            {
                if (mstClosingPayrolls.Count() >= 3)
                {
                    IDictionary<DateTime, DateTime> dictionaryDate =
                    _generalBll.EachDay(mstClosingPayrolls.ElementAt(2).ClosingDate.Date, DateTime.Now.Date)
                        .ToDictionary(p => p.Date);

                    return new SelectList(dictionaryDate, "Key", "Value");
                }

            }

            if (mstClosingPayroll.ClosingDate.Date > DateTime.Now.Date)
            {
                if (mstClosingPayrolls.Count() >= 2)
                {
                    IDictionary<DateTime, DateTime> dictionaryDate =
                     _generalBll.EachDay(mstClosingPayrolls.ElementAt(1).ClosingDate.Date, DateTime.Now.Date)
                         .ToDictionary(p => p.Date);

                    return new SelectList(dictionaryDate, "Key", "Value");
                }
            }

            if (mstClosingPayroll.ClosingDate.Date == DateTime.Now.Date)
            {
                var utilities = _utilitiesBll.GetTransactionLog(new TransactionLogInput
                {
                    TransactionCode = Enums.CombineCode.WPC.ToString(),
                    NotEqualIdFlow = (int)Enums.IdFlow.ProductionCardApprovalComplete
                });

                if (utilities != null)
                {
                    if (mstClosingPayrolls.Count() >= 3)
                    {
                        IDictionary<DateTime, DateTime> dictionaryDate =
                    _generalBll.EachDay(mstClosingPayrolls.ElementAt(2).ClosingDate.Date, DateTime.Now.Date)
                        .ToDictionary(p => p.Date);

                        return new SelectList(dictionaryDate, "Key", "Value");
                    }
                }
                else
                {
                    if (mstClosingPayrolls.Count() >= 2)
                    {
                        IDictionary<DateTime, DateTime> dictionaryDate =
                   _generalBll.EachDay(mstClosingPayrolls.ElementAt(1).ClosingDate.Date, DateTime.Now.Date)
                       .ToDictionary(p => p.Date);

                        return new SelectList(dictionaryDate, "Key", "Value");
                    }
                }
            }

            return new SelectList(null);
        }

        public SelectList GetAllRegionals()
        {
            var input = new GetAllLocationsInput() { ParentLocationCode = Enums.LocationCode.TPO.ToString() };
            var regions = _masterDataBLL.GetAllLocations(input);
            return new SelectList(regions, "LocationCode", "LocationCode");
        }
        public SelectList GetAllProcessGroupTPKPlant(string locationCode)
        {

            // take from PlanTPOTargetProductionKelompok http://tp.voxteneo.com/entity/58794
            var distict = _planningBll.GetPlantTPKProcessByLocations(locationCode);
            return new SelectList(distict);
        }
        public SelectList GetProcessGroupFromPlantEntryVerification(string locationCode, string unitCode, int shift, DateTime productionDate)
        {
            var listVerifications = _exePlantBll.GetVerificationForFilterProcessGroup(locationCode, unitCode, shift, productionDate);

            if (listVerifications.Any())
                return new SelectList(listVerifications);
            else 
                return GetAllProcessGroupTPKPlant(locationCode);
        }


        /// <summary>
        /// Gets the SKT band group from by location unit process.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public SelectList GetGroupBrandExeReportByGroupByLocationAndProcess(string locationCode, string process)
        {
            var input = new GetExeReportByGroupInput
            {
                LocationCode = locationCode,
                ProcessGroup = process
            };
            var datas = _masterDataBLL.GetSktBrand(locationCode, process);
            var dataDistinct = datas.DistinctBy(m => m.BrandGroupCode).OrderBy(c => c.SKTBrandCode);
            return new SelectList(dataDistinct, "SKTBrandCode", "SKTBrandCode");
            //var brandGroups = _masterDataBLL.GetAllBrandGroups();
            //return new SelectList(brandGroups, "BrandGroupCode", "BrandGroupCode");
        }

        public SelectList GetActiveBrandCodeByLocation(string locationCode)
        {
            var result = _masterDataBLL.GetActiveBrandCodeByLocationCode(locationCode);
            return new SelectList(result);
        }

        /// <summary>
        /// Gets the child locations lookup list.
        /// </summary>
        /// <param name="parentLocationCode">The parent location code.</param>
        /// <returns></returns>
        public List<LocationLookupList> GetPLNTTPOChildLocationsLookupList(bool responsibilities = true)
        {
            var locations = new List<MstGenLocationDTO>();

            var inputPLNT = new GetAllLocationsInput() { ParentLocationCode = Enums.LocationCode.PLNT.ToString() };
            var inputTPO = new GetAllLocationsInput() { ParentLocationCode = Enums.LocationCode.TPO.ToString() };

            var locationsPLNT = _masterDataBLL.GetAllLocations(inputPLNT, responsibilities);
            locations.AddRange(locationsPLNT);

            var locationsTPO = _masterDataBLL.GetAllLocations(inputTPO, responsibilities);
            foreach (var loc in locationsTPO)
            {
                var locationsTPOChild = _masterDataBLL.GetAllLocations(new GetAllLocationsInput { ParentLocationCode = loc.LocationCode }, responsibilities);
                locations.AddRange(locationsTPOChild);
            }

            return Mapper.Map<List<LocationLookupList>>(locations);
        }
        #region Roles
        public SelectList GetRolesSelectList()
        {
            var roles = _utilitiesBll.GetAllRoles();
            return new SelectList(roles, "IDRole", "RolesCode");
        }
        #endregion

        public SelectList GetProcessGroupSelectListByPlanPlantWorkHour(string locationCode, string unitCode, string brandGroupCode, string groupCode)
        {
            var planPlantCapacityWorkHour = _planningBll.GetProsesGroupFromCapacityWorkHour(locationCode, unitCode, brandGroupCode, groupCode);
            var exePlantProductionEntryVerivicationsDistinct = planPlantCapacityWorkHour.DistinctBy(c => c.ProcessGroup);
            return new SelectList(exePlantProductionEntryVerivicationsDistinct, "ProcessGroup", "ProcessGroup");
        }


        public SelectList GetBrandCodeFromReportDailyAchievment(
           GetExePlantProductionEntryVerificationInput input)
        {
            var listBrandCode = _planningBll.GetBrandCodeFromReportDailyAchievment(input).OrderBy(p => p.BrandCode).Select(p => p.BrandCode).Distinct();

            IDictionary<string, string> dictionaryBrand = new Dictionary<string, string>();

            foreach (var data in listBrandCode)
            {
                dictionaryBrand.Add(data, data);
            }

            return new SelectList(dictionaryBrand, "Key", "Value");
        }

        public SelectList GetBrandCodeUnionEntryVerification(
          GetExePlantProductionEntryVerificationInput input)
        {
            var listBrandCode = _planningBll.GeteExePlantProductionEntryVerificationWithUnion(input);

            IDictionary<string, string> dictionaryBrand = new Dictionary<string, string>();

            foreach (var data in listBrandCode)
            {
                dictionaryBrand.Add(data, data);
            }

            return new SelectList(dictionaryBrand, "Key", "Value");
        }

        public SelectList GetBrandCodeFromReportByProcess(
          GetExePlantProductionEntryVerificationInput input)
        {
            var listBrandCode = _exeOtherBll.GetBrandCodeFromReportByProcess(input);

            IDictionary<string, string> dictionaryBrand = new Dictionary<string, string>();

            foreach (var data in listBrandCode)
            {
                dictionaryBrand.Add(data, data);
            }

            return new SelectList(dictionaryBrand, "Key", "Value");
        }

        public SelectList GetLocStatusByParentLocationCode(string locationCode)
        {
            var db = _masterDataBLL.GetGenLocStatusByLocationCode(locationCode);
            var dbGroupByEmpStatus = db.DistinctBy(m => m.StatusEmp);
            return new SelectList(dbGroupByEmpStatus, "StatusEmp", "StatusEmp");
        }

        public SelectList GetGroupAvailabelPositionNumberByLocationUniShiftAndProcess(string locationCode, string unit, string process, string status)
        {
            var input = new GetPlanPlantGroupShiftInput
            {
                LocationCode = locationCode,
                UnitCode = unit,
                ProcessGroup = process,
                Status = status
            };
            var exePlantProductionEntryVerivications = _planningBll.GetGroupShiftProcess(input);
            var exePlantProductionEntryVerivicationsDistinct = exePlantProductionEntryVerivications.DistinctBy(m => m.GroupCode).OrderBy(m => m.GroupCode);


            return new SelectList(exePlantProductionEntryVerivicationsDistinct, "GroupCode", "GroupCode");
        }

        public SelectList GetAdjusmnetTypeList(string brandcode)
        {
            var conversions = _masterDataBLL.GetMstGeneralLists(brandcode).OrderBy(c => c.ListDetail).Distinct();

            return new SelectList(conversions, "ListDetail", "ListDetail");
        }

        public SelectList GetUtilRolesSelectList()
        {
            var roles = _utilitiesBll.GetAllRoles();

            return new SelectList(roles, "IDRole", "RolesName");
        }

        public List<UtilRoleDTO> GetUtilRoles()
        {
            return _utilitiesBll.GetAllRoles();
        }


    }
}