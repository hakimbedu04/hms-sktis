using System.Globalization;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MaintenanceReportEquipmentRequirement;
using SKTISWebsite.Models.MasterGenProccessSetting;
using SKTISWebsite.Models.MstTPOPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MaintenanceReportEquipmentRequirementController : BaseController
    {
        private IMaintenanceBLL _maintenanceBll;
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private IVTLogger _vtlogger;

        public MaintenanceReportEquipmentRequirementController(IMaintenanceBLL maintenanceBll, IVTLogger vtlogger, IApplicationService svc, IMasterDataBLL masterDataBll)
        {
            this._maintenanceBll = maintenanceBll;
            this._svc = svc;
            this._masterDataBll = masterDataBll;
            this._vtlogger = vtlogger;
            SetPage("Maintenance/Report/EquipmentRequirement");
        }

        public ActionResult Index()
        {            
            var model = new InitMaintenanceReportEquipmentRequirement()
            {
                RegionalChildLocation = _svc.GetLocationCodeSelectListByLevel(Enums.LocationCode.TPO,2),
                PLNTAndRegionalChildLocation = _svc.GetPlantAndRegionalLocationLookupList()
            };
            return View(model);
        }

        public JsonResult GetBrandGroupByLocationCode(string locationCode)
        {            
            var brandGroupCodes = _svc.GetBrandGroupCodeSelectListByParentLocationCode(locationCode);
            return Json(brandGroupCodes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTPOPackage(string LocationCode, string BrandGroupCode)
        {
            try
            {
                var input = new GetMstTPOPackagesInput()
                {
                    LocationCode = LocationCode,
                    BrandGroupCode = BrandGroupCode
                };
                var dbResult = _masterDataBll.GetTPOPackages(input).FirstOrDefault();
                var TPOPackages = Mapper.Map<MstTPOPackageViewModel>(dbResult);
                return Json(TPOPackages, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { LocationCode, BrandGroupCode }, "Maintenance Report Equipment Requirement - GetTPOPackage");
                return null;
            }
        }

        [HttpGet]
        public JsonResult GetBrandGroupTo(string BrandFrom)
        {            
            var brandGroupCodes = _svc.GetBrandGroupCodeDestinationSelectList(BrandFrom);
            return Json(brandGroupCodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEquipmentRequirementReport(string LocationCode, string BrandGroupCode, string UserPackage)
        {

            try
            {
                //float? UserPackages = (float?)Convert.ToDecimal(UserPackage);

                var dbResult = _maintenanceBll.GetMaintenanceEquipmentRequirementReport(LocationCode, BrandGroupCode,
                    float.Parse(UserPackage, CultureInfo.CurrentCulture.NumberFormat), DateTime.Now);

                //var input = new GetMaintenanceInventoryInput(){
                //    LocationCode = LocationCode,
                //    InventoryDate = DateTime.Now
                //};

                //var MntcInventoryAll = _maintenanceBll.GetReportMntcInventoryAllView(input);

                //var stock = _maintenanceBll.GetReportMstGenBrandPackageItem(input);

                var equipmentRequirement = Mapper.Map<List<MaintenanceEquipmentRequirementViewModel>>(dbResult);
                var pageResult = new PageResult<MaintenanceEquipmentRequirementViewModel>(equipmentRequirement);
                return Json(pageResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { LocationCode, BrandGroupCode, UserPackage }, "Maintenance Report Equipment Requirement - GetEquipmentRequirementReport");
                return null;
            }
        }

        public JsonResult GetEquipmentRequirementReport2(string LocationCode, string BrandGroupCodeFrom, string BrandGroupCodeTo)
        {
            try
            {
                var dbResult = _maintenanceBll.GetMaintenanceEquipmentRequirementReport2(LocationCode,
                    BrandGroupCodeFrom, BrandGroupCodeTo, DateTime.Now);
                var equipmentRequirement = Mapper.Map<List<MaintenanceEquipmentRequirementViewModel>>(dbResult);
                var pageResult = new PageResult<MaintenanceEquipmentRequirementViewModel>(equipmentRequirement);
                return Json(pageResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { LocationCode, BrandGroupCodeFrom, BrandGroupCodeTo }, "Maintenance Report Equipment Requirement - GetEquipmentRequirementReport2");
                return null;
            }
        }

        public JsonResult GetSummaryLocations(string BrandGroupCode)
        {
            try
            {
                var input = new GetEquipmentRequirementSummaryInput()
                {
                    BrandGroupCode = BrandGroupCode,
                    SortExpression = "LocationCode",
                    SortOrder = "ASC"
                };
                var dbResult = _maintenanceBll.GetEquipmentRequirementSummaryLocations(input);
                var processSettingLocation = Mapper.Map<List<EquipmentRequirementViewModel>>(dbResult);
                return Json(processSettingLocation, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { BrandGroupCode }, "Maintenance Report Equipment Requirement - GetSummaryLocations");
                return null;
            }
        }

        public JsonResult GetEquipmentRequirementReportSummary(string LocationCode, string BrandGroupCode)
        {
            try
            {
                var dbResultItems = _maintenanceBll.GetEquipmentSummaryItem(LocationCode, BrandGroupCode, DateTime.Now);
                var equipmentRequirementItem = Mapper.Map<List<EquipmentRequirementItemViewModel>>(dbResultItems);

                var input = new GetEquipmentRequirementSummaryInput()
                {
                    BrandGroupCode = BrandGroupCode,
                    SortExpression = "LocationCode",
                    SortOrder = "ASC"
                };
                var dbResultLocations = _maintenanceBll.GetEquipmentRequirementSummaryLocations(input);

                foreach (var item in equipmentRequirementItem)
                {
                    var quantityList = new List<int?>();
                    foreach (var location in dbResultLocations)
                    {
                        var quantity = _maintenanceBll.GetRealStock(location.LocationCode, item.ItemCode, DateTime.Now);
                        item.Quantity.Add(quantity.GetValueOrDefault(0));
                    }

                }

                var pageResult = new PageResult<EquipmentRequirementItemViewModel>(equipmentRequirementItem);
                return Json(pageResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { LocationCode, BrandGroupCode }, "Maintenance Report Equipment Requirement - GetEquipmentRequirementReportSummary");
                return null;
            }
        }

	}
}