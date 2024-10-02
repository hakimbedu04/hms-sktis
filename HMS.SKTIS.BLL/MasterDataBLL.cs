using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System.Web.SessionState;
using Microsoft.SqlServer.Server;
using Enums = HMS.SKTIS.Core.Enums;
using System.Data.Entity;

namespace HMS.SKTIS.BLL
{
    public partial class MasterDataBLL : IMasterDataBLL
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MstGenList> _mstGeneralListRepo;
        private IGenericRepository<MstGenProcess> _mstProcessRepo;
        private IGenericRepository<MstGenHoliday> _mstHoliday;
        private IGenericRepository<MstGenStandardHour> _standardHourRepository;
        private IGenericRepository<MstGenBrandGroup> _brandGroupRepository;
        private IGenericRepository<MstGenLocation> _mstLocationRepo;
        private IGenericRepository<MstGenBrandPackageItem> _mstBrandPackageItem;
        private IGenericRepository<MstMntcItem> _mstMaintenanceItemRepo;
        private IGenericRepository<MstMntcItemLocation> _mstItemLocationRepo;
        private IGenericRepository<MstGenBrandPkgMapping> _mstBrandPackageMapping;
        private IGenericRepository<NewsInfo> _newsInfoRepo;
        private IGenericRepository<MstTPOInfo> _mstTpointoRepo;
        //private IGenericRepository<MstGenBrand> _brandRepository;
        private IGenericRepository<MstGenMaterial> _materialRepository;
        private IGenericRepository<MstPlantUnit> _mstPlantUnitRepo;
        private IGenericRepository<MstTPOPackage> _mstTPOPackage;
        private IGenericRepository<MstTPOFeeRate> _mstTPOFreeRateRepo;
        private IGenericRepository<MstPlantEmpJobsDataAcv> _mstEmployeeJobsDataActiveRepo;
        private IGenericRepository<MstPlantProductionGroup> _mstPlantProductionGroupRepo;
        private IGenericRepository<MstGenBrand> _mstGenBrandRepo;
        private IGenericRepository<MstTPOProductionGroup> _mstTPOProductionGroupRepo;
        private IGenericRepository<MstGenLocStatu> _mstGenLocStatusRepo;
        private IGenericRepository<MstGenEmpStatu> _mstGenEmpStatusRepo;
        private IGenericRepository<MstGenProcessSettingsLocation> _mstGenProcessSettingLocationRepo;
        private IGenericRepository<ProcessSettingsAndLocationView> _mstGenProcessSettingLocationViewRepo;
        private IGenericRepository<MstGenProcessSetting> _mstGenProcessSettingRepo;
        private IGenericRepository<MstPlantAbsentType> _mstPlantAbsentTypeRepo;
        private IGenericRepository<MstPlantUnitView> _mstPlantUnitViewRepo;
        private IGenericRepository<MstPlantProductionGroupView> _mstPlantProductionGroupViewRepo;
        private ISqlSPRepository _sqlSPRepo;
        private IGenericRepository<MstMntcConvert> _mstMntcConvertRepo;
        private IGenericRepository<MstMntcConvertGetItemDestinationView> _mstMntcConvertGetItemDestinationViewRepo;
        private IGenericRepository<MstMntcItemLocation> _mstMntcItemLocationRepository;
        private IGenericRepository<MntcRepairItemUsage> _mntcRepairItemUsageRepo;
        private IGenericRepository<MntcEquipmentItemConvert> _mntcEquipmentItemConvert;
        private IGenericRepository<MntcRepairItemUsageView> _mntcRepairItemUsageViewRepo;
        private IGenericRepository<MstProcessBrandView> _mstProcessBrandViewRepo;

        private IGenericRepository<MstGenWeek> _mstGenWeekRepo;
        private IGenericRepository<BrandCodeByLocationView> _brandCodeByLocationViewRepo;
        private IGenericRepository<MstClosingPayroll> _mstClosingPayrollRepo;
        private IGenericRepository<TPOFeeSettingCalculation> _tpoFeeSettingCalculationRepo;
        private IGenericRepository<AdjustmentTypeByBrandCode> _adjustmentTypeByBrandCode;

        public MasterDataBLL(IUnitOfWork uow)
        {
            _uow = uow;

            _mstGeneralListRepo = _uow.GetGenericRepository<MstGenList>();
            _mstProcessRepo = _uow.GetGenericRepository<MstGenProcess>();
            _mstHoliday = _uow.GetGenericRepository<MstGenHoliday>();
            _standardHourRepository = _uow.GetGenericRepository<MstGenStandardHour>();
            _brandGroupRepository = _uow.GetGenericRepository<MstGenBrandGroup>();
            _mstLocationRepo = _uow.GetGenericRepository<MstGenLocation>();
            _mstBrandPackageItem = _uow.GetGenericRepository<MstGenBrandPackageItem>();
            _mstBrandPackageMapping = _uow.GetGenericRepository<MstGenBrandPkgMapping>();
            _mstMaintenanceItemRepo = _uow.GetGenericRepository<MstMntcItem>();
            _mstItemLocationRepo = _uow.GetGenericRepository<MstMntcItemLocation>();
            _newsInfoRepo = _uow.GetGenericRepository<NewsInfo>();
            _mstTpointoRepo = _uow.GetGenericRepository<MstTPOInfo>();
            //_brandRepository = _uow.GetGenericRepository<MstGenBrand>();
            _materialRepository = _uow.GetGenericRepository<MstGenMaterial>();
            _mstPlantUnitRepo = _uow.GetGenericRepository<MstPlantUnit>();
            _mstTPOPackage = _uow.GetGenericRepository<MstTPOPackage>();
            _mstTPOFreeRateRepo = _uow.GetGenericRepository<MstTPOFeeRate>();
            _mstEmployeeJobsDataActiveRepo = _uow.GetGenericRepository<MstPlantEmpJobsDataAcv>();
            _mstPlantProductionGroupRepo = _uow.GetGenericRepository<MstPlantProductionGroup>();
            _mstGenBrandRepo = _uow.GetGenericRepository<MstGenBrand>();
            _mstTPOProductionGroupRepo = _uow.GetGenericRepository<MstTPOProductionGroup>();
            _mstGenLocStatusRepo = _uow.GetGenericRepository<MstGenLocStatu>();
            _mstGenEmpStatusRepo = _uow.GetGenericRepository<MstGenEmpStatu>();
            _mstGenProcessSettingLocationRepo = _uow.GetGenericRepository<MstGenProcessSettingsLocation>();
            _mstGenProcessSettingLocationViewRepo = _uow.GetGenericRepository<ProcessSettingsAndLocationView>();
            _mstGenProcessSettingRepo = _uow.GetGenericRepository<MstGenProcessSetting>();
            _mstPlantAbsentTypeRepo = _uow.GetGenericRepository<MstPlantAbsentType>();
            _mstPlantUnitViewRepo = _uow.GetGenericRepository<MstPlantUnitView>();
            _mstPlantProductionGroupViewRepo = _uow.GetGenericRepository<MstPlantProductionGroupView>();
            _mstProcessBrandViewRepo = _uow.GetGenericRepository<MstProcessBrandView>();
            _sqlSPRepo = _uow.GetSPRepository();
            _mstMntcConvertRepo = _uow.GetGenericRepository<MstMntcConvert>();
            _mstMntcConvertGetItemDestinationViewRepo = _uow.GetGenericRepository<MstMntcConvertGetItemDestinationView>();
            _mstGenWeekRepo = _uow.GetGenericRepository<MstGenWeek>();
            _brandCodeByLocationViewRepo = _uow.GetGenericRepository<BrandCodeByLocationView>();
            _mstMntcItemLocationRepository = _uow.GetGenericRepository<MstMntcItemLocation>();
            _mntcRepairItemUsageRepo = _uow.GetGenericRepository<MntcRepairItemUsage>();
            _mntcEquipmentItemConvert = _uow.GetGenericRepository<MntcEquipmentItemConvert>();
            _mntcRepairItemUsageViewRepo = _uow.GetGenericRepository<MntcRepairItemUsageView>();
            _mstClosingPayrollRepo = _uow.GetGenericRepository<MstClosingPayroll>();
            _tpoFeeSettingCalculationRepo = _uow.GetGenericRepository<TPOFeeSettingCalculation>();
            _adjustmentTypeByBrandCode = _uow.GetGenericRepository<AdjustmentTypeByBrandCode>();

        }

        #region NewsInfo

        public List<NewsInfoCompositeDTO> GetNewsInfos()
        {
            var dbNewsInfos = _newsInfoRepo.Get().OrderByDescending(m => m.UpdatedDate);
            return Mapper.Map<List<NewsInfoCompositeDTO>>(dbNewsInfos);
        }
        public List<NewsInfoCompositeDTO> GetNewsInfosHome()
        {
            var dbNewsInfos = _newsInfoRepo.Get().OrderByDescending(m => m.UpdatedDate).Skip(5).Take(6);
            return Mapper.Map<List<NewsInfoCompositeDTO>>(dbNewsInfos);
        }

        public NewsInfoDTO GetNewsInfoById(int id)
        {
            var dbNewsInfo = _newsInfoRepo.GetByID(id);
            return Mapper.Map<NewsInfoDTO>(dbNewsInfo);

        }
        public NewsInfoDTO InsertNewsInfo(NewsInfoDTO newsInfo)
        {
            var item = Mapper.Map<NewsInfo>(newsInfo);

            // set created date and updated date
            item.CreatedDate = DateTime.Now;
            item.UpdatedDate = DateTime.Now;

            try
            {
                _newsInfoRepo.Insert(item);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.HResult);
            }

            return Mapper.Map<NewsInfoDTO>(item);
        }

        public NewsInfoDTO UpdateNewsInfo(NewsInfoDTO newsInfo)
        {
            var item = _newsInfoRepo.GetByID(newsInfo.NewsId);

            if (item == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            // keep original value created by & created date
            newsInfo.CreatedBy = item.CreatedBy;
            newsInfo.CreatedDate = item.CreatedDate;

            // set updated date
            newsInfo.UpdatedDate = DateTime.Now;

            Mapper.Map(newsInfo, item);
            try
            {
                _newsInfoRepo.Update(item);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.HResult);
            }

            return Mapper.Map<NewsInfoDTO>(item);
        }

        public bool DeleteNewsInfo(int id)
        {
            try
            {
                _newsInfoRepo.Delete(id);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.HResult);
                return false;
            }
            return true;

        }
        public List<SliderNewsInfoDTO> GetSliderNewsInfo(string locationCode)
        {
            Expression<Func<NewsInfo, bool>> queryFilter = n => n.IsSlider == true && n.Active == true;

            Func<IQueryable<NewsInfo>, IOrderedQueryable<NewsInfo>> orderByFilter = n => n.OrderByDescending(e => e.UpdatedDate) as IOrderedQueryable<NewsInfo>;

            var dbSliderNewsInfos = _newsInfoRepo.Get(queryFilter, orderByFilter).Take(5);

            return Mapper.Map<List<SliderNewsInfoDTO>>(dbSliderNewsInfos);
        }

        public InformatipsNewsInfoDTO GetInformatipsNewsInfo(string locationCode)
        {
            Expression<Func<NewsInfo, bool>> queryFilter = n => n.Location == locationCode && n.IsInformatips == true;

            Func<IQueryable<NewsInfo>, IOrderedQueryable<NewsInfo>> orderByFilter = n => n.OrderBy(e => e.UpdatedDate) as IOrderedQueryable<NewsInfo>;

            var dbInformatifsNewsInfo = _newsInfoRepo.Get(queryFilter, orderByFilter).FirstOrDefault();

            return Mapper.Map<InformatipsNewsInfoDTO>(dbInformatifsNewsInfo);
        }

        public List<MstHolidayDTO> GetHolidayInformation(string locationCode)
        {
            Expression<Func<MstGenHoliday, bool>> queryFilter = n => n.LocationCode == locationCode && n.HolidayDate.Year == DateTime.Now.Year;

            Func<IQueryable<MstGenHoliday>, IOrderedQueryable<MstGenHoliday>> orderByFilter = n => n.OrderByDescending(e => e.HolidayDate) as IOrderedQueryable<MstGenHoliday>;

            var dbHolidayInformation = _mstHoliday.Get(queryFilter, orderByFilter).Take(5);

            return Mapper.Map<List<MstHolidayDTO>>(dbHolidayInformation);
        }

        public List<MstHolidayDTO> GetDateHolidayLookLocation(string locationCode, DateTime date)
        {
            Expression<Func<MstGenHoliday, bool>> queryFilter = n => n.LocationCode == locationCode && n.HolidayDate.Year == DateTime.Now.Year && n.HolidayDate == date && n.StatusActive == true;

            Func<IQueryable<MstGenHoliday>, IOrderedQueryable<MstGenHoliday>> orderByFilter = n => n.OrderByDescending(e => e.HolidayDate) as IOrderedQueryable<MstGenHoliday>;

            var dbHolidayInformation = _mstHoliday.Get(queryFilter, orderByFilter).Take(5);

            return Mapper.Map<List<MstHolidayDTO>>(dbHolidayInformation);
        }
        #endregion

        //todo: move this general region to MasterGeneralBLL
        #region General

        #region General List

        /// <summary>
        /// Get list of master general list
        /// </summary>
        /// <param name="input"></param>
        /// <returns>list of MstGeneralListCompositeDTO</returns>
        public List<MstGeneralListCompositeDTO> GetMstGeneralLists(GetMstGenListsInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenList>();

            if (!string.IsNullOrEmpty(input.ListGroup))
                queryFilter = queryFilter.And(m => m.ListGroup == input.ListGroup);

            if (input.ListDetail != null)
            {
                queryFilter = queryFilter.And(p => input.ListDetail.Contains(p.ListDetail));
            }

            // queryFilter = queryFilter.And(p => p.StatusActive == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenList>();

            var dbResult = _mstGeneralListRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstGeneralListCompositeDTO>>(dbResult);
        }

        /// <summary>
        /// Get list of listgroup
        /// </summary>
        /// <returns>list of ListGroup</returns>
        //todo: put it on ApplicationService
        public List<string> GetListGroups()
        {
            //var queryFilter = PredicateHelper.True<MstGenList>();
            //Func<IQueryable<MstGenList>, IOrderedQueryable<MstGenList>> orderByFilter = m => m.OrderBy(z => z.ListGroup) as IOrderedQueryable<MstGenList>;
            //var dbResult = _mstGeneralListRepo.Get(queryFilter, orderByFilter).Select(m => m.ListGroup).Distinct();
            //return dbResult.ToList();           
            return (from Enum fi in Enum.GetValues(typeof(Enums.MasterGeneralList)) select EnumHelper.GetDescription(fi)).ToList();
        }

        public List<KeyValuePair<string, string>> GetListGroupsEnum()
        {
            //var queryFilter = PredicateHelper.True<MstGenList>();
            //Func<IQueryable<MstGenList>, IOrderedQueryable<MstGenList>> orderByFilter = m => m.OrderBy(z => z.ListGroup) as IOrderedQueryable<MstGenList>;
            //var dbResult = _mstGeneralListRepo.Get(queryFilter, orderByFilter).Select(m => m.ListGroup).Distinct();
            //return dbResult.ToList();  
            var dataSource = new Dictionary<string, string>();
            var data = (from Enum fi in Enum.GetValues(typeof(Enums.MasterGeneralList)) select new { Text = EnumHelper.GetDescription(fi), Value = fi.ToString() });
            return data.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Text, item.Value)).ToList();


        }

        /// <summary>
        /// Insert new master general list
        /// </summary>
        /// <param name="mstGeneralList"></param>
        public MstGeneralListDTO InsertMstGeneralList(MstGeneralListDTO mstGeneralList)
        {
            ValidateInsertMstGeneralList(mstGeneralList);

            var dbMstGeneralList = Mapper.Map<MstGenList>(mstGeneralList);

            _mstGeneralListRepo.Insert(dbMstGeneralList);

            dbMstGeneralList.CreatedDate = DateTime.Now;
            dbMstGeneralList.UpdatedDate = DateTime.Now;

            _uow.SaveChanges();

            return Mapper.Map<MstGeneralListDTO>(dbMstGeneralList);
        }

        private void ValidateInsertMstGeneralList(MstGeneralListDTO mstGeneralListToValidate)
        {
            var dbMstGeneralList = _mstGeneralListRepo.GetByID(mstGeneralListToValidate.ListGroup, mstGeneralListToValidate.ListDetail);

            if (dbMstGeneralList != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        /// <summary>
        /// Update existing master general list
        /// </summary>
        /// <param name="mstGeneralList"></param>
        public MstGeneralListDTO UpdateMstGeneralList(MstGeneralListDTO mstGeneralList)
        {
            var dbMstGeneralList = _mstGeneralListRepo.GetByID(mstGeneralList.ListGroup, mstGeneralList.ListDetail);

            if (dbMstGeneralList == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            Mapper.Map(mstGeneralList, dbMstGeneralList);
            _mstGeneralListRepo.Update(dbMstGeneralList);

            dbMstGeneralList.UpdatedDate = DateTime.Now;

            _uow.SaveChanges();

            return Mapper.Map<MstGeneralListDTO>(dbMstGeneralList);
        }

        //todo: create generic GetGeneralListByListGroup(Enums.MasterGeneralList listGroupEnum)
        public List<MstGeneralListCompositeDTO> GetGeneralListTPORank()
        {
            return GetGeneralListByListGroup(EnumHelper.GetDescription(Enums.MasterGeneralList.TPORank));
        }

        public List<MstGeneralListCompositeDTO> GetGeneralListFunctionType()
        {
            return GetGeneralListByListGroup(EnumHelper.GetDescription(Enums.MasterGeneralList.FunctionType));
        }
        /// <summary>
        /// Get Shift from General List
        /// </summary>
        /// <returns></returns>
        //todo: create generic GetGeneralListByListGroup(Enums.MasterGeneralList listGroupEnum)
        public List<MstGeneralListCompositeDTO> GetGeneralListShift()
        {
            return GetGeneralListByListGroup(EnumHelper.GetDescription(Enums.MasterGeneralList.Shift));
        }

        /// <summary>
        /// Get Item Type from general list
        /// </summary>
        /// <returns></returns>
        //todo: create generic GetGeneralListByListGroup(Enums.MasterGeneralList listGroupEnum)
        public List<MstGeneralListCompositeDTO> GetGeneralListsItemType()
        {
            return GetGeneralListByListGroup(Enums.MasterGeneralList.MtncItemType.ToString());
        }

        /// <summary>
        /// Get UOM from general list
        /// </summary>
        /// <returns></returns>
        //todo: create generic GetGeneralListByListGroup(Enums.MasterGeneralList listGroupEnum)
        public List<MstGeneralListCompositeDTO> GetGeneralListUOM()
        {
            return GetGeneralListByListGroup(Enums.MasterGeneralList.MtrlUOM.ToString());
        }

        /// <summary>
        /// Get Price Type from general list
        /// </summary>
        /// <returns></returns>
        //todo: create generic GetGeneralListByListGroup(Enums.MasterGeneralList listGroupEnum)
        public List<MstGeneralListCompositeDTO> GetGeneralListPriceType()
        {
            return GetGeneralListByListGroup(Enums.MasterGeneralList.MtncPrice.ToString());
        }
        /// <summary>
        /// Get Holiday Type from general list
        /// </summary>
        /// <returns></returns>
        //todo: create generic GetGeneralListByListGroup(Enums.MasterGeneralList listGroupEnum)
        public List<MstGeneralListCompositeDTO> GetGeneralListHolidayType()
        {
            return GetGeneralListByListGroup(Enums.MasterGeneralList.HolidayType.ToString());
        }

        /// <summary>
        /// Get General list by listgroup
        /// </summary>
        /// <param name="listGroup">Name of List Group</param>
        /// <returns></returns>
        private List<MstGeneralListCompositeDTO> GetGeneralListByListGroup(string listGroup)
        {
            var dbResult = _mstGeneralListRepo.Get(filter => filter.ListGroup == listGroup && filter.StatusActive == true,
                                                   orderby => orderby.OrderBy(f => f.ListDetail));
            return Mapper.Map<List<MstGeneralListCompositeDTO>>(dbResult);
        }


        //todo: use list instead of array
        public List<MstGeneralListCompositeDTO> GetGeneralLists(params string[] parameter)
        {
            var queryFilter = PredicateHelper.True<MstGenList>();

            if (parameter.Any())
                queryFilter = queryFilter.And(p => parameter.Contains(p.ListGroup));

            var result = _mstGeneralListRepo.Get(queryFilter);

            return Mapper.Map<List<MstGeneralListCompositeDTO>>(result);
        }

        #endregion

        //#region Location

        //public MstGenLocation GetMstLocationById(string locationCode)
        //{
        //    var repo = _uow.GetGenericRepository<MstGenLocation>();
        //    return repo.GetByID(locationCode);
        //}

        //public List<MstGenLocationDTO> GetMstGenLocationsByParentCode(GetMstGenLocationsByParentCodeInput input)
        //{
        //    var dbLocations = _sqlSPRepo.GetMstGenLocationsByParentCode(input).ToList();
        //    return Mapper.Map<List<MstGenLocationDTO>>(dbLocations);
        //}

        //public List<MstGenLocationDTO> GetMstLocationLists(GetMstGenLocationInput input)
        //{
        //    var queryFilter = PredicateHelper.True<MstGenLocation>();

        //    if (!string.IsNullOrEmpty(input.LocationCode))
        //        queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

        //    var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
        //    var orderByFilter = sortCriteria.GetOrderByFunc<MstGenLocation>();
        //    var dbResult = _mstLocationRepo.Get(queryFilter, orderByFilter);
        //    return Mapper.Map<List<MstGenLocationDTO>>(dbResult);
        //}

        ///// <summary>
        ///// Get All Location under SKT with level as parameter
        ///// </summary>
        ///// <param name="level"></param>
        ///// <returns></returns>
        //public List<MstGenLocationDTO> GetSKTLocationByLevels(int level)
        //{
        //    return GetAllLocationByLocationCode(Enums.LocationCode.SKT.ToString(), level);
        //}

        ///// <summary>
        ///// Get all Location under location code we input as parameter
        ///// </summary>
        ///// <param name="locationCode">Location code</param>
        ///// <param name="level">level (-1) for all child</param>
        ///// <returns></returns>
        //public List<MstGenLocationDTO> GetAllLocationByLocationCode(string locationCode, int level)
        //{
        //    return Mapper.Map<List<MstGenLocationDTO>>(_sqlSPRepo.GetLocations(locationCode, level));
        //}

        //public MstGenLocationDTO InsertLocation(MstGenLocationDTO location)
        //{
        //    ValidateInsertMstGenLocation(location);

        //    var dbLocation = Mapper.Map<MstGenLocation>(location);

        //    _mstLocationRepo.Insert(dbLocation);

        //    dbLocation.CreatedDate = DateTime.Now;
        //    dbLocation.UpdatedDate = DateTime.Now;

        //    SaveTpoInfoAfterSaveLocation(dbLocation);
        //    _uow.SaveChanges();

        //    return Mapper.Map(dbLocation, location);
        //}

        ///// <summary>
        ///// Saves the tpo information after save location.
        ///// </summary>
        ///// <param name="locationDto">The location dto.</param>
        //public void SaveTpoInfoAfterSaveLocation(MstGenLocation locationDto)
        //{
        //    if (locationDto.ParentLocationCode != Enums.LocationCode.TPO.ToString()) return;
        //    var dbTpoInfo = _mstTpointoRepo.GetByID(locationDto.LocationCode);
        //    if (dbTpoInfo != null) return;
        //    var tpoInfo = new MstTPOInfo
        //    {
        //        LocationCode = locationDto.LocationCode,
        //        VendorNumber = "",
        //        Established = DateTime.Now,
        //        CreatedBy = locationDto.CreatedBy,
        //        CreatedDate = DateTime.Now,
        //        UpdatedBy = locationDto.UpdatedBy,
        //        UpdatedDate = DateTime.Now
        //    };
        //    _mstTpointoRepo.Insert(tpoInfo);
        //    //_uow.SaveChanges();
        //}

        //private void ValidateInsertMstGenLocation(MstGenLocationDTO location)
        //{
        //    var dbMstGenLocation = _mstLocationRepo.GetByID(location.LocationCode);

        //    if (dbMstGenLocation != null)
        //        throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);

        //    ValidationLocationAndParentSameMstGenLocation(location);
        //}

        //public MstGenLocationDTO UpdateLocation(MstGenLocationDTO location)
        //{
        //    var dbLocation = _mstLocationRepo.GetByID(location.LocationCode);
        //    if (dbLocation == null)
        //        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

        //    ValidationLocationAndParentSameMstGenLocation(location);

        //    //keep original CreatedBy and CreatedDate
        //    location.CreatedBy = dbLocation.CreatedBy;
        //    location.CreatedDate = dbLocation.CreatedDate;

        //    //set update time
        //    location.UpdatedDate = DateTime.Now;

        //    Mapper.Map(location, dbLocation);
        //    dbLocation.UpdatedDate = DateTime.Now;
        //    try
        //    {
        //        _mstLocationRepo.Update(dbLocation);

        //        SaveTpoInfoAfterSaveLocation(dbLocation);

        //        _uow.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BLLException(ex.HResult);
        //    }

        //    return location;
        //}

        ///// <summary>
        ///// Validations the location and parent same MST gen location.
        ///// </summary>
        ///// <param name="location">The location.</param>
        ///// <exception cref="BLLException"></exception>
        //private void ValidationLocationAndParentSameMstGenLocation(MstGenLocationDTO location)
        //{
        //    if (location.LocationCode == location.ParentLocationCode)
        //        throw new BLLException(ExceptionCodes.BLLExceptions.LocationCodeAndParentSame);
        //}

        //public List<string> GetAllLocationCode()
        //{
        //    var queryFilter = PredicateHelper.True<MstGenLocation>();
        //    Func<IQueryable<MstGenLocation>, IOrderedQueryable<MstGenLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstGenLocation>;

        //    var dbResult = _mstLocationRepo.Get(queryFilter).Select(m => m.LocationCode).Distinct();

        //    return dbResult.ToList();
        //}

        ///// <summary>
        ///// Get list of locationcode
        ///// </summary>
        ///// <returns>list of LocationCode</returns>
        ////todo: we can remove it and use ApplicationService
        //public List<string> GetLocationCodes()
        //{
        //    var queryFilter = PredicateHelper.True<MstGenLocation>();
        //    queryFilter = queryFilter.And(m => m.StatusActive == true);
        //    Func<IQueryable<MstGenLocation>, IOrderedQueryable<MstGenLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstGenLocation>;
        //    //Func<IQueryable<MstLocation>, IOrderedQueryable<MstLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstLocation>;

        //    var dbResult = _mstLocationRepo.Get(queryFilter).Select(m => m.LocationCode).Distinct();

        //    return dbResult.ToList();
        //}

        ///// <summary>
        ///// Get location for dropdown list only code for text dan value
        ///// </summary>
        ///// <param name="locationCode"></param>
        ///// <returns></returns>
        //public List<LocationInfoDTO> GetLocationInfo(string locationCode)
        //{
        //    var locations = GetAllLocationByLocationCode(locationCode, -1);
        //    return locations.Select(location => new LocationInfoDTO
        //                                        {
        //                                            Text = location.LocationCode,
        //                                            Value = location.LocationCode
        //                                        }).ToList();
        //}

        ///// <summary>
        ///// Get location for dropdown list code for 'Text' and name for 'value'
        ///// </summary>
        ///// <param name="locationCode"></param>
        ///// <returns></returns>
        //public List<LocationInfoDTO> GetLocationInfoWithName(string locationCode)
        //{
        //    var locations = GetAllLocationByLocationCode(locationCode, -1);
        //    return locations.Select(location => new LocationInfoDTO
        //                                        {
        //                                            Text = location.LocationName,
        //                                            Value = location.LocationCode
        //                                        }).ToList();
        //}

        //public List<MstGenLocationDTO> GetAllLocations(GetAllLocationsInput input)
        //{
        //    var queryFilter = PredicateHelper.True<MstGenLocation>();

        //    if (!string.IsNullOrEmpty(input.ParentLocationCode))
        //        queryFilter = queryFilter.And(m => m.ParentLocationCode == input.ParentLocationCode);

        //    queryFilter = queryFilter.And(m => m.StatusActive == true);
        //    //Func<IQueryable<MstGenLocationDTO>, IOrderedQueryable<MstGenLocationDTO>> orderByFilter = m => m.OrderBy(z => z.BrandGroupCode);

        //    var dbResult = _mstLocationRepo.Get(queryFilter);

        //    return Mapper.Map<List<MstGenLocationDTO>>(dbResult);
        //}
        //#endregion


        #region Brand Group

        /// <summary>
        /// Get list of brand groups
        /// </summary>
        /// <param name="input">object GetBrandGroupInput</param>
        /// <returns>list brand group domain model</returns>
        public List<MstGenBrandGroupDTO> GetBrandGroups(GetMstGenBrandGroupInput input = null)
        {
            if (input != null)
            {
                var queryFilter = PredicateHelper.True<MstGenBrandGroup>();

                if (!string.IsNullOrEmpty(input.BrandFamily))
                    queryFilter = queryFilter.And(p => p.BrandFamily == input.BrandFamily);

                if (!string.IsNullOrEmpty(input.BrandGroupCode))
                    queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);

                if (!string.IsNullOrEmpty(input.PackType))
                    queryFilter = queryFilter.And(p => p.PackType == input.PackType);

                if (!string.IsNullOrEmpty(input.ClassType))
                    queryFilter = queryFilter.And(p => p.ClassType == input.ClassType);

                var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
                var orderByFilter = sortCriteria.GetOrderByFunc<MstGenBrandGroup>();

                var result = _brandGroupRepository.Get(queryFilter, orderByFilter);

                return Mapper.Map<List<MstGenBrandGroupDTO>>(result);
            }

            var brandGroups = _brandGroupRepository.Get();

            return Mapper.Map<List<MstGenBrandGroupDTO>>(brandGroups);

        }

        /// <summary>
        /// Insert brand group
        /// </summary>
        /// <param name="brandGroup">object BrandGroupDTO</param>
        public MstGenBrandGroupDTO InsertBrandGroup(MstGenBrandGroupDTO brandGroup)
        {
            var checkIfKeyExists = _brandGroupRepository.GetByID(brandGroup.BrandGroupCode);

            if (checkIfKeyExists != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);

            var item = Mapper.Map<MstGenBrandGroup>(brandGroup);

            // set created date & updated date
            item.CreatedDate = DateTime.Now;
            item.UpdatedDate = DateTime.Now;

            _brandGroupRepository.Insert(item);
            _uow.SaveChanges();

            return Mapper.Map<MstGenBrandGroupDTO>(item);

        }

        /// <summary>
        /// Update Brand Group
        /// </summary>
        /// <param name="brandGroup"></param>
        /// <returns></returns>
        public MstGenBrandGroupDTO UpdateBrandGroup(MstGenBrandGroupDTO brandGroup)
        {
            var item = _brandGroupRepository.GetByID(brandGroup.BrandGroupCode);

            if (item == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            // keep original value created by & created date
            brandGroup.CreatedBy = item.CreatedBy;
            brandGroup.CreatedDate = item.CreatedDate;

            // set updated date
            brandGroup.UpdatedDate = DateTime.Now;

            Mapper.Map(brandGroup, item);
            _brandGroupRepository.Update(item);

            _uow.SaveChanges();

            return Mapper.Map<MstGenBrandGroupDTO>(item);
        }

        /// <summary>
        /// Get all active brand group
        /// </summary>
        /// <returns>list of brand group</returns>
        public List<MstGenBrandGroupDTO> GetActiveBrandGroups()
        {
            var resultdb = _brandGroupRepository.Get(b => b.StatusActive == true);
            return Mapper.Map<List<MstGenBrandGroupDTO>>(resultdb);
        }

        /// <summary>
        /// Get BrandGroup
        /// </summary>
        /// <param name="brandGroupCode">Brand Code</param>
        /// <returns></returns>
        public MstGenBrandGroup GetBrandGroupById(string brandGroupCode)
        {
            var repo = _uow.GetGenericRepository<MstGenBrandGroup>();
            return repo.GetByID(brandGroupCode);
        }

        public List<MstGenBrandGroupDTO> GetAllBrandGroups()
        {
            var queryFilter = PredicateHelper.True<MstGenBrandGroup>();

            Func<IQueryable<MstGenBrandGroup>, IOrderedQueryable<MstGenBrandGroup>> orderByFilter = m => m.OrderBy(z => z.BrandGroupCode);

            var dbResult = _brandGroupRepository.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstGenBrandGroupDTO>>(dbResult);
        }

        public string GetBrandGruopCodeByBrandCode(string brandCode)
        {
            var dbResult = _mstGenBrandRepo.GetByID(brandCode);

            return dbResult.BrandGroupCode;
        }

        #endregion

        #region Process
        /// <summary>
        /// Get Master process
        /// </summary>
        /// <param name="input"></param>
        /// <returns>List of process</returns>
        public List<MstGenProcessDTO> GetMasterProcesses(GetMstGenProcessesInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenProcess>();

            if (!string.IsNullOrEmpty(input.ProcessName))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.ProcessName);

            if (input.WIP)
                queryFilter = queryFilter.And(m => m.WIP == input.WIP);

            if (input.StatusActive)
                queryFilter = queryFilter.And(m => m.StatusActive == input.StatusActive);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenProcess>();

            var dbResult = _mstProcessRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<MstGenProcessDTO>>(dbResult);
        }

        /// <summary>
        /// Insert new master process
        /// </summary>
        /// <param name="processDTO"></param>
        /// <returns>Process object</returns>
        public MstGenProcessDTO InsertMasterProcess(MstGenProcessDTO processDTO)
        {
            ValidateInsertMasterProcess(processDTO);

            var dbProcess = Mapper.Map<MstGenProcess>(processDTO);

            dbProcess.CreatedDate = DateTime.Now;
            dbProcess.UpdatedDate = DateTime.Now;

            _mstProcessRepo.Insert(dbProcess);
            _uow.SaveChanges();

            Mapper.Map(dbProcess, processDTO);
            return processDTO;
        }

        private void ValidateInsertMasterProcess(MstGenProcessDTO processDto)
        {
            var dbResult = _mstProcessRepo.GetByID(processDto.ProcessGroup);
            if (dbResult != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.ProcessGroupExist);

            var dbResultByProcessIdentifier = _mstProcessRepo.Get(m => m.ProcessIdentifier == processDto.ProcessIdentifier);
            if (dbResultByProcessIdentifier.Any())
                throw new BLLException(ExceptionCodes.BLLExceptions.ProcessIdentifierExist);
        }

        /// <summary>
        /// Update existing master process
        /// </summary>
        /// <param name="processDTO"></param>
        public MstGenProcessDTO UpdateMasterProcess(MstGenProcessDTO processDTO)
        {
            var dbProcess = _mstProcessRepo.GetByID(processDTO.ProcessGroup);
            if (dbProcess == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            processDTO.CreatedBy = dbProcess.CreatedBy;
            processDTO.CreatedDate = dbProcess.CreatedDate;

            //set update time
            processDTO.UpdatedDate = DateTime.Now;
            Mapper.Map(processDTO, dbProcess);

            _mstProcessRepo.Update(dbProcess);
            _uow.SaveChanges();

            return Mapper.Map<MstGenProcessDTO>(dbProcess);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MstGenProcessDTO> GetAllActiveMasterProcess()
        {
            var dbResult = _mstProcessRepo.Get(m => m.StatusActive == true);
            return Mapper.Map<List<MstGenProcessDTO>>(dbResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public MstGenProcessDTO GetMasterProcessByProcess(string process)
        {
            var dbResult = _mstProcessRepo.GetByID(process);
            return Mapper.Map<MstGenProcessDTO>(dbResult);
        }

        public MstGenProcessDTO GetMasterProcessByIdentifier(string identifierProcess)
        {
            var result = _mstProcessRepo.Get(m => m.ProcessIdentifier == identifierProcess).SingleOrDefault();
            return Mapper.Map<MstGenProcessDTO>(result);
        }

        public List<MstGenProcessDTO> GetAllMasterProcess()
        {
            var queryFilter = PredicateHelper.True<MstGenProcess>();

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "ProcessOrder" }, "ASC");
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenProcess>();

            var dbResult = _mstProcessRepo.Get(queryFilter, orderByFilter);

            //var dbResult = _mstProcessRepo.Get(queryFilter);

            return Mapper.Map<List<MstGenProcessDTO>>(dbResult);
        }
        #endregion

        #region Brand Package Item

        /// <summary>
        /// Get master package equipment
        /// </summary>
        /// <param name="input"></param>
        /// <returns> List of master package equipment</returns>
        public List<MstGenBrandPackageItemDTO> GetMstGenBrandPackageItem(MstGenBrandPackageItemInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenBrandPackageItem>();

            if (!string.IsNullOrEmpty(input.ItemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == input.ItemCode);

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(m => m.BrandGroupCode == input.BrandGroupCode);

            var dbResult = _mstBrandPackageItem.Get(queryFilter);
            return Mapper.Map<List<MstGenBrandPackageItemDTO>>(dbResult);
        }

        /// <summary>
        /// Insert new master package equipment
        /// </summary>
        /// <param name="packageEquipmentDTO"></param>
        /// <returns></returns>
        public MstGenBrandPackageItemDTO InsertBrandPackageItem(MstGenBrandPackageItemDTO packageEquipmentDTO)
        {
            var dbMstPackageEquipment = Mapper.Map<MstGenBrandPackageItem>(packageEquipmentDTO);

            dbMstPackageEquipment.CreatedDate = DateTime.Now;
            dbMstPackageEquipment.UpdatedDate = DateTime.Now;

            _mstBrandPackageItem.Insert(dbMstPackageEquipment);
            _uow.SaveChanges();

            Mapper.Map(dbMstPackageEquipment, packageEquipmentDTO);
            return packageEquipmentDTO;
        }

        /// <summary>
        /// Update existing master package equipment
        /// </summary>
        /// <param name="packageEquipmentDTO"></param>
        /// <returns></returns>
        public MstGenBrandPackageItemDTO UpdateBrandPackageItem(MstGenBrandPackageItemDTO packageEquipmentDTO)
        {
            var dbMstPackageEquipment = _mstBrandPackageItem.GetByID(packageEquipmentDTO.ItemCode, packageEquipmentDTO.BrandGroupCode);
            if (dbMstPackageEquipment == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            packageEquipmentDTO.CreatedBy = dbMstPackageEquipment.CreatedBy;
            packageEquipmentDTO.CreatedDate = dbMstPackageEquipment.CreatedDate;

            //set update time
            packageEquipmentDTO.UpdatedDate = DateTime.Now;
            Mapper.Map(packageEquipmentDTO, dbMstPackageEquipment);
            _mstBrandPackageItem.Update(dbMstPackageEquipment);
            _uow.SaveChanges();

            return Mapper.Map<MstGenBrandPackageItemDTO>(dbMstPackageEquipment);
        }

        #endregion

        #region Holiday

        public bool IsHoliday(DateTime date, string locationCode)
        {
            var queryFilter = PredicateHelper.True<MstGenHoliday>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (date != null)
                queryFilter = queryFilter.And(m => m.HolidayDate == date);

            var dbResult = _mstHoliday.Get(queryFilter).FirstOrDefault();
            if (dbResult != null)
                return true;

            return false;
        }

        /// <summary>
        /// Get all holiday
        /// </summary>
        /// <param name="input">input</param>
        /// <returns></returns>
        public List<MstHolidayDTO> GetMstHoliday(MstHolidayInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenHoliday>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var location = GetLocationInfo(input.LocationCode).Select(l => l.Value).ToList();
                queryFilter = queryFilter.And(m => location.Contains(m.LocationCode));
            }

            queryFilter = input.HolidayDate != null
                                ? queryFilter.And(m => m.HolidayDate == input.HolidayDate)
                                : queryFilter.And(m => m.HolidayDate.Year == input.Year);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenHoliday>();

            var dbResult = _mstHoliday.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<MstHolidayDTO>>(dbResult);
        }

        public List<String> GetHolidayByWeek(MstGenWeekDTO mstGenWeek, string locationCode)
        {

            var queryFilter = PredicateHelper.True<MstGenHoliday>();
            queryFilter = queryFilter.And(m => m.HolidayDate >= mstGenWeek.StartDate);
            queryFilter = queryFilter.And(m => m.HolidayDate <= mstGenWeek.EndDate);
            queryFilter = queryFilter.And(m => m.LocationCode == locationCode);

            return _mstHoliday.Get(queryFilter).Select(m => m.HolidayDate.ToString("dd/MM/yyyy")).ToList();
        }

        /// <summary>
        /// Insert holiday
        /// </summary>
        /// <param name="holidayDTO"></param>
        /// <returns></returns>
        public MstHolidayDTO InsertHoliday(MstHolidayDTO holidayDTO)
        {
            ValidationInsertHoliday(holidayDTO);

            var dbMstHoliday = Mapper.Map<MstGenHoliday>(holidayDTO);

            dbMstHoliday.CreatedDate = DateTime.Now;
            dbMstHoliday.UpdatedDate = DateTime.Now;

            _mstHoliday.Insert(dbMstHoliday);
            _uow.SaveChanges();

            Mapper.Map(dbMstHoliday, holidayDTO);
            return holidayDTO;
        }

        private void ValidationInsertHoliday(MstHolidayDTO holidayDto)
        {
            var db = _mstHoliday.GetByID(holidayDto.HolidayDate, holidayDto.HolidayType, holidayDto.LocationCode);
            if (db != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        /// <summary>
        /// Update Holiday
        /// </summary>
        /// <param name="holidayDTO"></param>
        /// <returns></returns>
        public MstHolidayDTO UpdateHoliday(MstHolidayDTO holidayDTO)
        {
            var dbMstHoliday = _mstHoliday.GetByID(holidayDTO.HolidayDate, holidayDTO.HolidayType, holidayDTO.LocationCode);
            if (dbMstHoliday == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            holidayDTO.CreatedBy = dbMstHoliday.CreatedBy;
            holidayDTO.CreatedDate = dbMstHoliday.CreatedDate;

            //set update time
            holidayDTO.UpdatedDate = DateTime.Now;
            Mapper.Map(holidayDTO, dbMstHoliday);
            _mstHoliday.Update(dbMstHoliday);
            _uow.SaveChanges();

            return Mapper.Map<MstHolidayDTO>(dbMstHoliday);
        }

        public MstHolidayDTO GetMstHolidayByID(DateTime date, string holidayType, string locationCode)
        {
            var dbResult = _mstHoliday.GetByID(date, holidayType, locationCode);

            return Mapper.Map<MstHolidayDTO>(dbResult);
        }

        #endregion

        #region Brand Package Mapping
        /// <summary>
        /// Get brand package mapping with brandgroupcode is active
        /// </summary>
        /// <returns></returns>
        public List<MstGenBrandPkgMappingDTO> GetMstGenBrandPkgMappings()
        {
            var brandPackages = _mstBrandPackageMapping.Get(brand => brand.MstGenBrandGroup.StatusActive == true &&
                                                                     brand.MstGenBrandGroup1.StatusActive == true);
            return Mapper.Map<List<MstGenBrandPkgMappingDTO>>(brandPackages);
        }
        /// <summary>
        /// Get Brand Group Code Destination By Brand Group Code Source
        /// </summary>
        /// <param name="BrandGroupCodeSource"></param>
        /// <returns></returns>
        public List<MstGenBrandPkgMappingDTO> GetBrandGroupCodeDestination(string BrandGroupCodeSource)
        {
            var brandPackages = _mstBrandPackageMapping.Get(brand => brand.BrandGroupCodeSource == BrandGroupCodeSource);
            return Mapper.Map<List<MstGenBrandPkgMappingDTO>>(brandPackages);
        }


        public MstGenBrandPkgMappingDTO InsertUpdateMstGenBrandPkgMapping(MstGenBrandPkgMappingDTO brandPackageDTO)
        {
            var package = _mstBrandPackageMapping.GetByID(brandPackageDTO.BrandGroupCodeSource, brandPackageDTO.BrandGroupCodeDestination);

            if (package == null)
            {
                var item = Mapper.Map<MstGenBrandPkgMapping>(brandPackageDTO);

                // set created date & updated date
                item.CreatedDate = DateTime.Now;
                item.CreatedBy = brandPackageDTO.UpdatedBy;
                item.UpdatedDate = DateTime.Now;

                _mstBrandPackageMapping.Insert(item);
                _uow.SaveChanges();

                return Mapper.Map<MstGenBrandPkgMappingDTO>(item);
            }
            else
            {

                //keep original CreatedBy and CreatedDate
                brandPackageDTO.CreatedBy = package.CreatedBy;
                brandPackageDTO.CreatedDate = package.CreatedDate;

                //set update time
                brandPackageDTO.UpdatedDate = DateTime.Now;
                Mapper.Map(brandPackageDTO, package);

                if (package.MappingValue == null)
                    _mstBrandPackageMapping.Delete(package);
                else
                    _mstBrandPackageMapping.Update(package);
                _uow.SaveChanges();
                return Mapper.Map<MstGenBrandPkgMappingDTO>(package);
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Get all active brand group
        /// </summary>
        /// <returns>list of brand group</returns>
        //public List<BrandGroupDTO> GetActiveBrandGroups()
        //{
        //    var resultdb = _brandGroupRepository.Get(b => b.StatusActive == true);
        //    return Mapper.Map<List<BrandGroupDTO>>(resultdb);
        //}

        //public MstBrandGroup GetBrandGroupById(string brandGroupCode)
        //{
        //    var repo = _uow.GetGenericRepository<MstBrandGroup>();
        //    return repo.GetByID(brandGroupCode);
        //}


        //#endregion

        #region Master Brand Group Material
        /// <summary>
        /// Remove Location Code
        /// http://tp.voxteneo.com/entity/57142
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<BrandGroupMaterialDTO> GetBrandGroupMaterial(GetBrandGroupMaterialInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenMaterial>();

            //if (!string.IsNullOrEmpty(input.Location))
            //{
            //    queryFilter = queryFilter.And(p => p.LocationCode == input.Location);
            //}

            if (!string.IsNullOrEmpty(input.ProcessGroup) && input.ProcessGroup == "CUTTING")
            {
                return new List<BrandGroupMaterialDTO>();
            }

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
            {
                queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);
            }

            if (!string.IsNullOrEmpty(input.ProcessGroup))
            {
                switch (input.ProcessGroup)
                {
                    case "ROLLING":
                        queryFilter = queryFilter.And(q => q.MaterialName == "Non-tobacco/1" || q.MaterialName == "Tobacco/1");
                        break;
                    case "PACKING":
                        queryFilter = queryFilter.And(q => q.MaterialName == "Non-tobacco/3");
                        break;
                    case "STAMPING":
                        queryFilter = queryFilter.And(q => q.MaterialName == "Non-tobacco/4");
                        break;
                }
            }

            //queryFilter = queryFilter.And(m => m.MstGenLocation.StatusActive == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenMaterial>();

            var result = _materialRepository.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<BrandGroupMaterialDTO>>(result);
        }

        public BrandGroupMaterialDTO GetMaterialByCode(string materialCode, string brandCode)
        {
            var db = _materialRepository.GetByID(materialCode, brandCode);

            return Mapper.Map<BrandGroupMaterialDTO>(db);
        }

        public bool GetMaterialIsTobbaco(string materialCode, string brandCode)
        {
            var db = _materialRepository.GetByID(materialCode, brandCode);
            if (db.MaterialName.Contains("Non"))
            {
                return false;
            }
            return true;
        }

        public BrandGroupMaterialDTO InsertBrandGroupMaterial(BrandGroupMaterialDTO brandGroupMaterial)
        {
            var check = _materialRepository.GetByID(brandGroupMaterial.MaterialCode, brandGroupMaterial.BrandGroupCode);

            if (check != null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
            }

            var item = Mapper.Map<MstGenMaterial>(brandGroupMaterial);

            // set created date and updated date
            item.CreatedDate = DateTime.Now;
            item.UpdatedDate = DateTime.Now;

            try
            {
                _materialRepository.Insert(item);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.HResult);
            }

            return Mapper.Map<BrandGroupMaterialDTO>(item);
        }

        public BrandGroupMaterialDTO UpdateBrandGroupMaterial(BrandGroupMaterialDTO brandGroupMaterialDto)
        {
            var item = _materialRepository.GetByID(brandGroupMaterialDto.OldMaterialCode, brandGroupMaterialDto.BrandGroupCode);

            if (item == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            var isNewItemExist = _materialRepository.GetByID(brandGroupMaterialDto.MaterialCode, brandGroupMaterialDto.BrandGroupCode);

            if (isNewItemExist != null && brandGroupMaterialDto.MaterialCode != brandGroupMaterialDto.OldMaterialCode)
                throw new BLLException(ExceptionCodes.BLLExceptions.BrandGroupMaterialCodeExistBrandGroupMaterial);

            // keep original value created by & created date
            brandGroupMaterialDto.CreatedBy = item.CreatedBy;
            brandGroupMaterialDto.CreatedDate = item.CreatedDate;

            // set updated date
            brandGroupMaterialDto.UpdatedDate = DateTime.Now;

            //Mapper.Map(brandGroupMaterialDto, item);

            using (SKTISEntities context = new SKTISEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var existingBrandGroupMaterial = context.MstGenMaterials.Find(brandGroupMaterialDto.OldMaterialCode, brandGroupMaterialDto.BrandGroupCode);

                        // Delete Existing Brand Group Material
                        //context.MstGenMaterials.Remove(existingBrandGroupMaterial);

                        //context.SaveChanges();

                        

                        //var dbExeMstGenMaterial = Mapper.Map<MstGenMaterial>(brandGroupMaterialDto);
                        existingBrandGroupMaterial.MaterialCode = brandGroupMaterialDto.MaterialCode;
                        existingBrandGroupMaterial.BrandGroupCode = brandGroupMaterialDto.BrandGroupCode;
                        existingBrandGroupMaterial.MaterialName = brandGroupMaterialDto.MaterialName;
                        existingBrandGroupMaterial.Description = brandGroupMaterialDto.Description;
                        existingBrandGroupMaterial.UOM = brandGroupMaterialDto.Uom;
                        existingBrandGroupMaterial.StatusActive = brandGroupMaterialDto.StatusActive;
                        existingBrandGroupMaterial.Remark = brandGroupMaterialDto.Remark;
                        existingBrandGroupMaterial.CreatedBy = brandGroupMaterialDto.CreatedBy;
                        existingBrandGroupMaterial.CreatedDate = brandGroupMaterialDto.CreatedDate;
                        existingBrandGroupMaterial.UpdatedDate = DateTime.Now;

                        
                        
                        

                        // Insert Worker Abseteeism
                        //context.MstGenMaterials.Add(dbExePlantWorkerAbsenteeism);

                        
                        context.SaveChanges();

                        transaction.Commit();

                        return Mapper.Map<BrandGroupMaterialDTO>(existingBrandGroupMaterial);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

            //try
            //{
            //    _materialRepository.Update(item);
            //    _uow.SaveChanges();
            //}
            //catch (Exception ex)
            //{
            //    throw new BLLException(ex.HResult);
            //}

            //return Mapper.Map<BrandGroupMaterialDTO>(item);
        }

        #endregion

        #region Master TPO Package

        public MstTPOPackageDTO GetTpoPackage(GetMstTPOPackagesInput input)
        {
            var queryFilter = PredicateHelper.True<MstTPOPackage>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(p => p.LocationCode == input.LocationCode);
            }

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
            {
                queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);
            }

            if (input.StartDate.HasValue && input.EndDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.EffectiveDate <= input.StartDate && p.ExpiredDate >= input.EndDate);
            }

            var dbResult = _mstTPOPackage.Get(queryFilter).FirstOrDefault();

            return Mapper.Map<MstTPOPackageDTO>(dbResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<MstTPOPackageDTO> GetTPOPackages(GetMstTPOPackagesInput input)
        {
            var queryFilter = PredicateHelper.True<MstTPOPackage>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.Year))
                queryFilter = queryFilter.And(m => m.EffectiveDate.Year.ToString() == input.Year);

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
            {
                queryFilter = queryFilter.And(q => q.BrandGroupCode.Contains(input.BrandGroupCode));
            }

            queryFilter = queryFilter.And(m => m.MstTPOInfo.MstGenLocation.StatusActive == true);
            //check is skt or not
            if (!string.IsNullOrEmpty(input.SortExpression) && input.SortExpression.ToLower() == "locationname")
                input.SortExpression = "MstTPOInfo.MstGenLocation." + input.SortExpression;

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOPackage>();

            var dbResult = _mstTPOPackage.Get(queryFilter, orderByFilter, "MstTPOInfo.MstGenLocation");

            return Mapper.Map<List<MstTPOPackageDTO>>(dbResult);
        }

        public DateTime GetEffectiveDate()
        {
            var queryFilter = PredicateHelper.True<MstTPOPackage>();

            queryFilter = queryFilter.And(m => m.ExpiredDate <= DateTime.Today);
            var dbResult = _mstTPOPackage.Get(queryFilter).OrderByDescending(desc => desc.ExpiredDate).FirstOrDefault();
            var effectiveDate = dbResult.ExpiredDate;

            while (effectiveDate.DayOfWeek != DayOfWeek.Monday)
            {
                effectiveDate = effectiveDate.AddDays(1);
            }

            return effectiveDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mstTPOPackageDTO"></param>
        /// <returns></returns>
        public MstTPOPackageDTO InsertTPOPackage(MstTPOPackageDTO mstTPOPackageDTO)
        {
            ValidateInsertTPOPackage(mstTPOPackageDTO);

            var dbTPOPackage = Mapper.Map<MstTPOPackage>(mstTPOPackageDTO);
            dbTPOPackage.CreatedDate = DateTime.Now;
            dbTPOPackage.UpdatedDate = DateTime.Now;

            //if (tpoPackage.EffectiveDate == DateTime.MinValue)
            //    tpoPackage.EffectiveDate = DateTime.Now.Date;
            if (dbTPOPackage.ExpiredDate == DateTime.MinValue)
                dbTPOPackage.ExpiredDate = DateTime.Now.Date;


            _mstTPOPackage.Insert(dbTPOPackage);
            _uow.SaveChanges();

            Mapper.Map(dbTPOPackage, mstTPOPackageDTO);
            return mstTPOPackageDTO;
        }

        private void ValidateInsertTPOPackage(MstTPOPackageDTO mstTpoPackageDto)
        {
            var dbResult = _mstTPOPackage.GetByID(mstTpoPackageDto.LocationCode, mstTpoPackageDto.BrandGroupCode, mstTpoPackageDto.EffectiveDate);

            if (dbResult != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);

            ValidateDateTPOPackage(mstTpoPackageDto);
        }

        private void ValidateDateTPOPackage(MstTPOPackageDTO mstTpoPackageDto)
        {
            var dbResult = _mstTPOPackage.GetByID(mstTpoPackageDto.LocationCode, mstTpoPackageDto.BrandGroupCode, mstTpoPackageDto.EffectiveDate);

            var exist = _mstTPOPackage.Get(m => m.BrandGroupCode.Contains(mstTpoPackageDto.BrandGroupCode)
                                             && m.LocationCode == mstTpoPackageDto.LocationCode
                                             && ((
                                                    m.EffectiveDate <= mstTpoPackageDto.EffectiveDate
                                                    && m.ExpiredDate >= mstTpoPackageDto.EffectiveDate
                                                ) || (
                                                    m.EffectiveDate <= mstTpoPackageDto.ExpiredDate
                                                    && m.ExpiredDate >= mstTpoPackageDto.ExpiredDate
                                                ))
                                             );
            if (exist.Count() > 0)
            {
                if (dbResult != null)
                {
                    var error = true;
                    foreach (var package in exist)
                    {
                        if (package.EffectiveDate == mstTpoPackageDto.EffectiveDate)
                        {
                            error = false;
                            break;
                        }
                    }

                    if (error)
                    {
                        throw new BLLException(ExceptionCodes.BLLExceptions.ExpiredAndEffectiveExist);
                    }
                }
                else
                {
                    throw new BLLException(ExceptionCodes.BLLExceptions.ExpiredAndEffectiveExist);
                }
            }
        }

        public MstTPOPackageDTO UpdateTPOPackage(MstTPOPackageDTO tpoPackage)
        {
            if (tpoPackage.EffectiveDate != tpoPackage.EffectiveDateOld)
            {
                //check is key already exist
                var dbResult = _mstTPOPackage.GetByID(tpoPackage.LocationCode, tpoPackage.BrandGroupCode, tpoPackage.EffectiveDate);

                if (dbResult != null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);

                //delete first old effective date
                var dbTpoPackageOld = _mstTPOPackage.GetByID(tpoPackage.LocationCode, tpoPackage.BrandGroupCode, tpoPackage.EffectiveDateOld);
                if (dbTpoPackageOld != null)
                {
                    _mstTPOPackage.Delete(dbTpoPackageOld);
                }

                //then insert new one
                var dbTPOPackage = Mapper.Map<MstTPOPackage>(tpoPackage);
              
                //set update time
                dbTPOPackage.UpdatedDate = DateTime.Now;
                dbTPOPackage.UpdatedBy = tpoPackage.UpdatedBy;

                dbTPOPackage.CreatedBy = dbTpoPackageOld.CreatedBy;
                dbTPOPackage.CreatedDate = dbTpoPackageOld.CreatedDate;


                if (dbTPOPackage.ExpiredDate == DateTime.MinValue)
                    dbTPOPackage.ExpiredDate = DateTime.Now.Date;

                _mstTPOPackage.Insert(dbTPOPackage);
                _uow.SaveChanges();
                return Mapper.Map<MstTPOPackageDTO>(dbTPOPackage);
            }
            else
            {
                ValidateDateTPOPackage(tpoPackage);
              
                var dbTPOPackage = _mstTPOPackage.GetByID(tpoPackage.LocationCode, tpoPackage.BrandGroupCode,
                    tpoPackage.EffectiveDate);

                if (dbTPOPackage == null)
                    throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

                //keep original
                tpoPackage.CreatedBy = dbTPOPackage.CreatedBy;
                tpoPackage.CreatedDate = dbTPOPackage.CreatedDate;
                tpoPackage.BrandGroupCode = dbTPOPackage.BrandGroupCode;

                //set update time
                tpoPackage.UpdatedDate = DateTime.Now;

                Mapper.Map(tpoPackage, dbTPOPackage);
                _mstTPOPackage.Update(dbTPOPackage);
                _uow.SaveChanges();
                return Mapper.Map<MstTPOPackageDTO>(dbTPOPackage);
            }
        }
        #endregion

        #region Master Unit
        /// <summary>
        /// Gets the MST plant units.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MstPlantUnitCompositeDTO> GetMstPlantUnits(GetMstPlantUnitsInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantUnitView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
                var locationCodes = GetAllLocationByLocationCode(input.LocationCode, -1).Select(m => m.LocationCode);
                queryFilter = queryFilter.Or(m => locationCodes.Contains(m.LocationCode));
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantUnitView>();

            var dbResult = _mstPlantUnitViewRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstPlantUnitCompositeDTO>>(dbResult);
        }


        /// <summary>
        /// Inserts the MST plant unit.
        /// </summary>
        /// <param name="mstPlantUnit">The MST plant unit.</param>
        /// <returns></returns>
        public MstPlantUnitDTO InsertMstPlantUnit(MstPlantUnitDTO mstPlantUnit)
        {
            ValidateInsertMstPlantUnit(mstPlantUnit);

            var dbMstPlantUnit = Mapper.Map<MstPlantUnit>(mstPlantUnit);

            _mstPlantUnitRepo.Insert(dbMstPlantUnit);

            dbMstPlantUnit.CreatedDate = DateTime.Now;
            dbMstPlantUnit.UpdatedDate = DateTime.Now;

            _uow.SaveChanges();

            return Mapper.Map<MstPlantUnitDTO>(dbMstPlantUnit);
        }

        /// <summary>
        /// Validates the insert MST plant unit.
        /// </summary>
        /// <param name="mstPlantUnitToValidate">The MST plant unit to validate.</param>
        /// <exception cref="BLLException"></exception>
        private void ValidateInsertMstPlantUnit(MstPlantUnitDTO mstPlantUnitToValidate)
        {
            var dbMstPlantUnit = _mstPlantUnitRepo.GetByID(mstPlantUnitToValidate.LocationCode, mstPlantUnitToValidate.UnitCode);

            if (dbMstPlantUnit != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        /// <summary>
        /// Updates the MST plant unit.
        /// </summary>
        /// <param name="mstPlantUnit">The MST plant unit.</param>
        /// <returns></returns>
        /// <exception cref="BLLException"></exception>
        public MstPlantUnitDTO UpdateMstPlantUnit(MstPlantUnitDTO mstPlantUnit)
        {
            var dbMstPlantUnit = _mstPlantUnitRepo.GetByID(mstPlantUnit.LocationCode, mstPlantUnit.UnitCode);

            if (dbMstPlantUnit == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            Mapper.Map(mstPlantUnit, dbMstPlantUnit);
            _mstPlantUnitRepo.Update(dbMstPlantUnit);

            dbMstPlantUnit.UpdatedDate = DateTime.Now;

            _uow.SaveChanges();

            return Mapper.Map<MstPlantUnitDTO>(dbMstPlantUnit);
        }

        public List<MstPlantUnitDTO> GetAllUnits(GetAllUnitsInput input, bool responsibilities = true)
        {
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];

            var queryFilter = PredicateHelper.True<MstPlantUnit>();

            if (!string.IsNullOrEmpty(input.LocationCode)){
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
                if (responsibilities && strUserID.Responsibility.Location.Where(w => w.LocationData.LocationCode == input.LocationCode).FirstOrDefault().Units != null)
                {
                    var unit = strUserID.Responsibility.Location.Where(w => w.LocationData.LocationCode == input.LocationCode).FirstOrDefault().Units.Split(';').ToList();
                    queryFilter = queryFilter.And(w => unit.Contains(w.UnitCode));

                }
            }

            queryFilter = queryFilter.And(m => m.StatusActive == true);
            //Func<IQueryable<MstGenLocationDTO>, IOrderedQueryable<MstGenLocationDTO>> orderByFilter = m => m.OrderBy(z => z.BrandGroupCode);

            

            var dbResult = _mstPlantUnitRepo.Get(queryFilter);
            if (input.IgnoreList != null)
                if (input.IgnoreList.Count > 0)
                    dbResult = dbResult.Where(m => !input.IgnoreList.Contains(m.UnitCode));

            return Mapper.Map<List<MstPlantUnitDTO>>(dbResult);
        }

        public bool CheckMstPlantUnit(string locationCode, string unitCode)
        {
            var dbMstPlantUnit = _mstPlantUnitRepo.GetByID(locationCode, unitCode);
            return dbMstPlantUnit == null;
        }

        #endregion

        #region TPO Free Rate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<MstTPOFeeRateDTO> GetTPOFeeRate(MstTPOFeeRateInput input)
        {
            var queryFilter = PredicateHelper.True<MstTPOFeeRate>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(m => m.BrandGroupCode == input.BrandGroupCode);
            if (!string.IsNullOrEmpty(input.Year))
                queryFilter = queryFilter.And(m => m.EffectiveDate.Year.ToString() == input.Year);

            queryFilter = queryFilter.And(m => m.MstTPOInfo.MstGenLocation.StatusActive == true);

            //check is skt or not
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOFeeRate>();

            var dbResult = _mstTPOFreeRateRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstTPOFeeRateDTO>>(dbResult);
        }

        public List<MstTPOFeeRateDTO> GetTPOFeeRateByExpiredDate(MstTPOFeeRateInput input)
        {
            var queryFilter = PredicateHelper.True<MstTPOFeeRate>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(m => m.BrandGroupCode == input.BrandGroupCode);

            queryFilter = queryFilter.And(m => m.EffectiveDate <= input.StartDate);
            queryFilter = queryFilter.And(m => m.ExpiredDate >= input.ExpiredDate);

            queryFilter = queryFilter.And(m => m.MstTPOInfo.MstGenLocation.StatusActive == true);

            //check is skt or not
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOFeeRate>();

            var dbResult = _mstTPOFreeRateRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstTPOFeeRateDTO>>(dbResult);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TPOFeeRate"></param>
        /// <returns></returns>
        public MstTPOFeeRateDTO InsertTPOFeeRate(MstTPOFeeRateDTO TPOFeeRate)
        {
            var dbTPOFeeRate = _mstTPOFreeRateRepo.GetByID(TPOFeeRate.EffectiveDate, TPOFeeRate.BrandGroupCode, TPOFeeRate.LocationCode);
            if (dbTPOFeeRate != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);

            var dbTPOFreeRate = Mapper.Map<MstTPOFeeRate>(TPOFeeRate);
            dbTPOFreeRate.CreatedDate = DateTime.Now;
            dbTPOFreeRate.UpdatedDate = DateTime.Now;

            _mstTPOFreeRateRepo.Insert(dbTPOFreeRate);
            _uow.SaveChanges();

            Mapper.Map(dbTPOFreeRate, TPOFeeRate);
            return TPOFeeRate;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TPOFeeRate"></param>
        /// <returns></returns>
        public MstTPOFeeRateDTO UpdateTPOFeeRate(MstTPOFeeRateDTO TPOFeeRate)
        {
            var dbTPOFeeRate = _mstTPOFreeRateRepo.GetByID(TPOFeeRate.PreviousEffectiveDate, TPOFeeRate.BrandGroupCode, TPOFeeRate.LocationCode);
            if (dbTPOFeeRate == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            using (SKTISEntities context = new SKTISEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var existingData = context.MstTPOFeeRates.Find(TPOFeeRate.PreviousEffectiveDate, TPOFeeRate.BrandGroupCode, TPOFeeRate.LocationCode);
                        //delete old data because pk is modified
                        context.MstTPOFeeRates.Remove(existingData);

                        //keep original CreatedBy and CreatedDate
                        TPOFeeRate.CreatedBy = dbTPOFeeRate.CreatedBy;
                        TPOFeeRate.CreatedDate = dbTPOFeeRate.CreatedDate;

                        //set update time
                        TPOFeeRate.UpdatedDate = DateTime.Now;

                        var dbInsertTPOFeeRate = new MstTPOFeeRate();
                        Mapper.Map(TPOFeeRate, dbInsertTPOFeeRate);

                        context.MstTPOFeeRates.Add(dbInsertTPOFeeRate);

                        context.SaveChanges();

                        transaction.Commit();

                        return Mapper.Map<MstTPOFeeRateDTO>(dbTPOFeeRate);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            
        }
        #endregion

        //#region Master Brand Package Mapping

        //#endregion

        #region Master Process Setting Location

        public List<MstGenProcessSettingsLocationCompositeDTO> GetMstGenProcessSettingLocations(
            GetMstGenProcessSettingLocationInput input)
        {

            var queryFilter = PredicateHelper.True<ProcessSettingsAndLocationView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var locationCodes = GetAllLocationByLocationCode(input.LocationCode, -1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locationCodes.Contains(m.LocationCode));
            }

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(m => m.BrandGroupCode == input.BrandGroupCode);

            if (input.IDProcess.HasValue)
                queryFilter = queryFilter.And(m => m.IDProcess == input.IDProcess);

            queryFilter = queryFilter.And(m => m.StatusActiveLocation == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ProcessSettingsAndLocationView>();

            var dbResult = _mstGenProcessSettingLocationViewRepo.Get(queryFilter);

            return Mapper.Map<List<MstGenProcessSettingsLocationCompositeDTO>>(dbResult);
        }

        public MstGenProcessSettingLocationDTO SaveMstGenProcessLocation(MstGenProcessSettingLocationDTO processSettingLocationDTO, bool isInsert)
        {
            var oldLocationCode = processSettingLocationDTO.LocationCode;
            var locationCodes = GetAllLocationByLocationCode(processSettingLocationDTO.LocationCode, -1).Select(m => m.LocationCode);
            MstGenProcessSettingsLocation dbProcessSettingLocation = null;

            foreach (var locationCode in locationCodes)
            {
                var procLoc = _mstGenProcessSettingLocationRepo.Get(l => l.LocationCode == locationCode).FirstOrDefault();

                if (isInsert) // INSERT
                {
                    if (procLoc == null)
                    {
                        dbProcessSettingLocation = Mapper.Map<MstGenProcessSettingsLocation>(processSettingLocationDTO);
                        dbProcessSettingLocation.LocationCode = locationCode;
                        dbProcessSettingLocation.CreatedDate = DateTime.Now;
                        dbProcessSettingLocation.UpdatedDate = DateTime.Now;
                        var procs = _mstGenProcessSettingRepo.Get(p => p.IDProcess == processSettingLocationDTO.IDProcess);
                        foreach (var proc in procs)
                        {
                            dbProcessSettingLocation.MstGenProcessSettings.Add(proc);
                        }
                        _mstGenProcessSettingLocationRepo.Insert(dbProcessSettingLocation);
                    }
                    else
                    {
                        throw new BLLException(ExceptionCodes.BLLExceptions.LocationSettingProcessExist);
                    }
                }
                else // UPDATE
                {
                    dbProcessSettingLocation = Mapper.Map<MstGenProcessSettingsLocation>(processSettingLocationDTO);
                    Mapper.Map(procLoc, processSettingLocationDTO);
                    procLoc.LocationCode = locationCode;
                    procLoc.UpdatedDate = DateTime.Now;
                    //procLoc.MaxWorker = dbProcessSettingLocation.MaxWorker;
                    var procs = _mstGenProcessSettingRepo.Get(p => p.IDProcess == processSettingLocationDTO.IDProcess);
                    var newProcs = procs.Except(procLoc.MstGenProcessSettings);
                    procLoc.MstGenProcessSettings.Clear();

                    foreach (var item in procs)
                    {
                        procLoc.MstGenProcessSettings.Add(item);
                    }
                    _mstGenProcessSettingLocationRepo.Update(procLoc);
                }

            }

            _uow.SaveChanges();

            Mapper.Map(dbProcessSettingLocation, processSettingLocationDTO);
            processSettingLocationDTO.LocationCode = oldLocationCode;
            return processSettingLocationDTO;
        }
        public MstGenProcessSettingLocationDTO DeleteProsesSettingLocId(MstGenProcessSettingLocationDTO location)
        {

            MstGenProcessSettingsLocation dbProcessSettingLocation = null;
            var dataToDelete = _mstGenProcessSettingLocationRepo.Get(c => c.LocationCode == location.LocationCode);
            var procLoc = _mstGenProcessSettingLocationRepo.Get(l => l.LocationCode == location.LocationCode).FirstOrDefault();
            if (procLoc.ID != null)
            {
                procLoc.MstGenProcessSettings.Clear();
            }

            if (dataToDelete != null)
            {
                _mstGenProcessSettingLocationRepo.Delete(dataToDelete.Select(c => c.ID).FirstOrDefault());
            }

            _uow.SaveChanges();

            Mapper.Map(dbProcessSettingLocation, location);
            return location;

        }

        public List<MstGenProcessSettingLocationDTO> GetAllProcessSettingLocations(
            GetAllProcessSettingsLocationsInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenProcessSettingsLocation>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            //Func<IQueryable<MstGenLocationDTO>, IOrderedQueryable<MstGenLocationDTO>> orderByFilter = m => m.OrderBy(z => z.BrandGroupCode);

            var dbResult = _mstGenProcessSettingLocationRepo.Get(queryFilter);

            return Mapper.Map<List<MstGenProcessSettingLocationDTO>>(dbResult);
        }

        private List<int> GetProcessSettingLocationIDProcessByLocationCode(string locationCode)
        {
            var procLoc = _mstGenProcessSettingLocationRepo.Get(l => l.LocationCode == locationCode).FirstOrDefault();
            if (procLoc == null)
                return new List<int>();
            return procLoc.MstGenProcessSettings.Select(p => p.IDProcess).ToList();
            //var inputLocation = new GetAllProcessSettingsLocationsInput() { LocationCode = locationCode };
            //var idProcesses = GetAllProcessSettingLocations(inputLocation).Select(x => x.IDProcess);
            //return idProcesses.ToList();
        }

        public List<MstGenProcessSettingsLocationCompositeDTO> GetMstGenProcessSettingLocationsDistinct(
            GetMstGenProcessSettingLocationInput input)
        {
            var queryFilter = PredicateHelper.True<ProcessSettingsAndLocationView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var locationCodes = GetAllLocationByLocationCode(input.LocationCode, -1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locationCodes.Contains(m.LocationCode));
            }

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(m => m.BrandGroupCode == input.BrandGroupCode);

            if (input.IDProcess.HasValue)
                queryFilter = queryFilter.And(m => m.IDProcess == input.IDProcess);

            queryFilter = queryFilter.And(m => m.StatusActiveLocation == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ProcessSettingsAndLocationView>();

            var dbResult = _mstGenProcessSettingLocationViewRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstGenProcessSettingsLocationCompositeDTO>>(dbResult);
        }

        public int GetStdPerhour(
            GetMstGenProcessSettingLocationInput input)
        {
            var queryFilter = PredicateHelper.True<ProcessSettingsAndLocationView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var locationCodes = GetAllLocationByLocationCode(input.LocationCode, -1).Select(m => m.LocationCode);
                queryFilter = queryFilter.And(m => locationCodes.Contains(m.LocationCode));
            }

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(m => m.BrandGroupCode == input.BrandGroupCode);

            if (!string.IsNullOrEmpty(input.ProcessGroup))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.ProcessGroup);

            queryFilter = queryFilter.And(m => m.StatusActiveLocation == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<ProcessSettingsAndLocationView>();

            var dbResult = _mstGenProcessSettingLocationViewRepo.Get(queryFilter, orderByFilter).FirstOrDefault();

            int StdPerHour = 0;
            if (dbResult != null)
            {
                if (dbResult.StdStickPerHour.HasValue)
                    StdPerHour = dbResult.StdStickPerHour.Value;
            }

            return StdPerHour;
        }


        #endregion

        #region Master TPO Production Group

        public List<MstTPOProductionGroupDTO> GetTPOProductionGroupLists(GetMstTPOProductionGroupInput input)
        {
            var queryFilter = PredicateHelper.True<MstTPOProductionGroup>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.Process))
                queryFilter = queryFilter.And(m => m.MstGenProcess.ProcessGroup == input.Process);
            if (!string.IsNullOrEmpty(input.Status))
                queryFilter = queryFilter.And(m => m.MstGenLocStatu.MstGenEmpStatu.StatusEmp == input.Status);

            queryFilter = queryFilter.And(m => m.MstGenLocStatu.MstGenLocation.StatusActive == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOProductionGroup>();

            var dbResult = _mstTPOProductionGroupRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstTPOProductionGroupDTO>>(dbResult);
        }

        public MstTPOProductionGroupDTO InsertTPOProductionGroup(MstTPOProductionGroupDTO productionGroupDto)
        {
            ValidateInsertTPOProductionGroup(productionGroupDto);

            ValidationCheckMasterGenLocStatus(productionGroupDto);

            var dbTpoProductionGroup = Mapper.Map<MstTPOProductionGroup>(productionGroupDto);

            dbTpoProductionGroup.CreatedDate = DateTime.Now;
            dbTpoProductionGroup.UpdatedDate = DateTime.Now;

            _mstTPOProductionGroupRepo.Insert(dbTpoProductionGroup);
            _uow.SaveChanges();
            return productionGroupDto;
        }

        private void ValidationCheckMasterGenLocStatus(MstTPOProductionGroupDTO productionGroupDto)
        {
            var dbGenLoc = _mstGenLocStatusRepo.GetByID(productionGroupDto.LocationCode, productionGroupDto.StatusEmp);
            if (dbGenLoc == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.LocationAndStatusEmpNotFound);
        }

        private void ValidateInsertTPOProductionGroup(MstTPOProductionGroupDTO mstTpoProdListToValidate)
        {
            var dbMstGeneralList = _mstTPOProductionGroupRepo.GetByID(mstTpoProdListToValidate.ProdGroup, mstTpoProdListToValidate.ProcessGroup, mstTpoProdListToValidate.LocationCode, mstTpoProdListToValidate.StatusEmp);
            if (dbMstGeneralList != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        public MstTPOProductionGroupDTO GetTpoProductionGroupById(string prodGroup, string processGroup, string locationCode, string status)
        {
            var dbTpoProductionGroup = _mstTPOProductionGroupRepo.GetByID(prodGroup, processGroup, locationCode, status);

            return Mapper.Map<MstTPOProductionGroupDTO>(dbTpoProductionGroup);
        }

        public MstTPOProductionGroupDTO UpdateTPOProductionGroup(MstTPOProductionGroupDTO productionGroupDto)
        {
            var dbTpoProductionGroup = _mstTPOProductionGroupRepo.GetByID(productionGroupDto.ProdGroup,
                productionGroupDto.ProcessGroup, productionGroupDto.LocationCode, productionGroupDto.StatusEmp);
            if (dbTpoProductionGroup == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //set update time
            dbTpoProductionGroup.UpdatedDate = DateTime.Now;

            Mapper.Map(productionGroupDto, dbTpoProductionGroup);
            _mstTPOProductionGroupRepo.Update(dbTpoProductionGroup);
            _uow.SaveChanges();

            return Mapper.Map<MstTPOProductionGroupDTO>(dbTpoProductionGroup);
        }
        #endregion

        #region Master General Emp Status
        public List<MstGenEmpStatusCompositeDTO> GetGenEmpStatusByLocationCode(string locationCode)
        {
            var queryFilter = PredicateHelper.True<MstGenLocStatu>();
            var locationCodes = GetAllLocationByLocationCode(locationCode, -1).Select(m => m.LocationCode);
            queryFilter = queryFilter.And(m => locationCodes.Contains(locationCode));

            var dbMstGenLocs = _mstGenLocStatusRepo.Get(queryFilter);

            var result = (from emp in _mstGenEmpStatusRepo.Get()
                          join loc in dbMstGenLocs on emp.StatusEmp equals loc.StatusEmp
                          select new MstGenEmpStatu
                          {
                              StatusEmp = loc.StatusEmp,
                              StatusIdentifier = emp.StatusIdentifier
                          }).ToList();
            return Mapper.Map<List<MstGenEmpStatusCompositeDTO>>(result);

            //return _mstGenLocStatusRepo.Get(m => m.LocationCode == locationCode).Select(m => m.StatusEmp).ToList();
        }

        public MstGenEmpStatusDTO GetGenEmpStatusByIdentifier(string identifierStatus)
        {
            var result = _mstGenEmpStatusRepo.Get(m => m.StatusIdentifier == identifierStatus).SingleOrDefault();
            return Mapper.Map<MstGenEmpStatusDTO>(result);
        }

        public string GetGenEmpStatusIdentifierByStatusEmp(string statusEmp)
        {
            var result = _mstGenEmpStatusRepo.Get(m => m.StatusEmp == statusEmp).SingleOrDefault().StatusIdentifier;
            return result;
        }

        public MstGenEmpStatusDTO GetGenEmpIdentifierByStatus(string status)
        {
            var result = _mstGenEmpStatusRepo.Get(m => m.StatusEmp == status).SingleOrDefault();
            return Mapper.Map<MstGenEmpStatusDTO>(result);
        }

        #endregion

        #region Master General Process Setting
        public List<MstGenProcessSettingDTO> GetStdStickPerHourByProcessAndBrandGroupCode(string process, string brandGroupCode)
        {
            var queryFilter = PredicateHelper.True<MstGenProcessSetting>();

            if (!string.IsNullOrEmpty(process))
                queryFilter = queryFilter.And(m => m.ProcessGroup == process);

            if (!string.IsNullOrEmpty(brandGroupCode))
                queryFilter = queryFilter.And(m => m.ProcessGroup == brandGroupCode);

            var genProcessSetting = _mstGenProcessSettingRepo.Get(queryFilter);

            return Mapper.Map<List<MstGenProcessSettingDTO>>(genProcessSetting);
        }
        public List<MstGenProcessSettingCompositeDTO> GetMasterProcessSettingByLocationCode(string locationCode)
        {
            var result = new List<MstGenProcessSetting>();
            var procLoc = _mstGenProcessSettingLocationRepo.Get(l => l.LocationCode == locationCode);
            if (procLoc.Any())
            {
                result = procLoc.FirstOrDefault().MstGenProcessSettings.Where(c => c.MstGenProcess != null).OrderBy(c => c.MstGenProcess.ProcessIdentifier).ToList();
            }
            //var result = (from proSet in _mstGenProcessSettingRepo.Get()
            //              join proSetLoc in _mstGenProcessSettingLocationRepo.Get(m => m.LocationCode == locationCode) on proSet.IDProcess equals proSetLoc.IDProcess
            //              select new MstGenProcessSetting()
            //              {
            //                  ProcessGroup = proSet.ProcessGroup,
            //                  ProcessIdentifier = proSet.ProcessIdentifier
            //              }).ToList();
            return Mapper.Map<List<MstGenProcessSettingCompositeDTO>>(result);
        }

        public List<MstGenProcessSettingDTO> GetMasterProcessSettings(GetMasterProcessSettingsInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenProcessSetting>();

            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(m => m.BrandGroupCode == input.BrandCode);

            if (!string.IsNullOrEmpty(input.Process))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.Process);

            if (input.IDProcess != null)
                queryFilter = queryFilter.And(m => m.IDProcess == input.IDProcess);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenProcessSetting>();

            var db = _mstGenProcessSettingRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstGenProcessSettingDTO>>(db);
        }

        public MstGenProcessSettingDTO InsertProcessSetting(MstGenProcessSettingDTO processSettingDto)
        {
            ValidateInsertProcessSettings(processSettingDto);

            var dbProcessSetting = Mapper.Map<MstGenProcessSetting>(processSettingDto);

            var procs = _mstGenProcessSettingRepo.Get(p => p.IDProcess == processSettingDto.IDProcess);

            dbProcessSetting.CreatedDate = DateTime.Now;
            dbProcessSetting.UpdatedDate = DateTime.Now;

            _mstGenProcessSettingRepo.Insert(dbProcessSetting);

            var procLocs = procs.SelectMany(p => p.MstGenProcessSettingsLocations);
            foreach (var procLoc in procLocs)
            {
                procLoc.MstGenProcessSettings.Add(dbProcessSetting);
                _mstGenProcessSettingLocationRepo.Update(procLoc);
            }

            _uow.SaveChanges();
            return processSettingDto;
        }

        private void ValidateInsertProcessSettings(MstGenProcessSettingDTO processSettingListToValidate)
        {
            var dbProcessSetting = _mstGenProcessSettingRepo.Get(p => p.IDProcess == processSettingListToValidate.IDProcess
                && p.BrandGroupCode == processSettingListToValidate.BrandGroupCode
                && p.ProcessGroup == processSettingListToValidate.ProcessGroup);
            if (dbProcessSetting.Any())
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
        }

        public MstGenProcessSettingDTO UpdateProcessSetting(MstGenProcessSettingDTO processSettingDto)
        {
            ValidateUpdateProcessSettings(processSettingDto);

            var dbProcessSetting = _mstGenProcessSettingRepo.Get(p => p.IDProcess == processSettingDto.IDProcess && p.BrandGroupCode == processSettingDto.BrandGroupCode && p.ProcessGroup == processSettingDto.ProcessGroup).FirstOrDefault();

            // Mapper.Map(processSettingDto, dbProcessSetting);

            //set update time
            dbProcessSetting.UpdatedDate = DateTime.Now;
            dbProcessSetting.StdStickPerHour = processSettingDto.StdStickPerHour;
            dbProcessSetting.MinStickPerHour = processSettingDto.MinStickPerHour;
            dbProcessSetting.UOMEblek = processSettingDto.UOMEblek;
            dbProcessSetting.Remark = processSettingDto.Remark;
            dbProcessSetting.MaxWorker = processSettingDto.MaxWorker;

            //_mstGenProcessSettingRepo.Update(dbProcessSetting);
            _uow.SaveChanges();

            return Mapper.Map<MstGenProcessSettingDTO>(dbProcessSetting);
        }

        public List<MstGenProcessSettingDTO> GetAllProcessSettings(GetAllProcessSettingsInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenProcessSetting>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var idProcesses = GetProcessSettingLocationIDProcessByLocationCode(input.LocationCode);
                queryFilter =
                    queryFilter.And(
                        m => idProcesses.Contains(m.IDProcess));
            }
            //Func<IQueryable<MstGenLocationDTO>, IOrderedQueryable<MstGenLocationDTO>> orderByFilter = m => m.OrderBy(z => z.BrandGroupCode);

            var dbResult = _mstGenProcessSettingRepo.Get(queryFilter);

            return Mapper.Map<List<MstGenProcessSettingDTO>>(dbResult);
        }

        public List<MstProcessBrandViewDTO> GetAllProcessSettingsByBrand(string locationCode, string brandGroupCode)
        {
            var queryFilter = PredicateHelper.True<MstProcessBrandView>();

            queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            queryFilter = queryFilter.And(m => m.BrandGroupCode == brandGroupCode);

            var dbResult = _mstProcessBrandViewRepo.Get(queryFilter);

            return Mapper.Map<List<MstProcessBrandViewDTO>>(dbResult);
        }

        public List<MstGenBrandGroupDTO> GetSktBrand(string locationCode, string process)
        {
            var queryFilter = PredicateHelper.True<MstProcessBrandView>();

            queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            queryFilter = queryFilter.And(m => m.ProcessGroup == process);

            var dbResult = _mstProcessBrandViewRepo.Get(queryFilter);

            var brandGroup = Mapper.Map<List<MstProcessBrandViewDTO>>(dbResult);

            var brandGroupDistinct = brandGroup.Select(l => l.BrandGroupCode).Distinct();

            var queryFilterBrand = PredicateHelper.True<MstGenBrandGroup>();

            queryFilterBrand = queryFilterBrand.And(m => brandGroupDistinct.Contains(m.BrandGroupCode));

            var dbResultBrand = _brandGroupRepository.Get(queryFilterBrand);

            return Mapper.Map<List<MstGenBrandGroupDTO>>(dbResultBrand);
        }

        private void ValidateUpdateProcessSettings(MstGenProcessSettingDTO input)
        {

            var isKeyExist = _mstGenProcessSettingRepo.Get(p => p.IDProcess == input.IDProcess && p.BrandGroupCode == input.BrandGroupCode && p.ProcessGroup == input.ProcessGroup).FirstOrDefault();

            if (isKeyExist == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            //var isDataExist = _mstGenProcessSettingRepo.Get(p => p.BrandGroupCode == input.BrandGroupCode
            //                                                && p.ProcessGroup == input.ProcessGroup
            //                                                && p.StdStickPerHour == input.StdStickPerHour
            //                                                && p.MinStickPerHour == input.MinStickPerHour
            //                                                && p.IDProcess != input.IDProcess).FirstOrDefault();

            //if (isDataExist != null)
            //{
            //    throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
            //}

        }

        public List<MstGenProcessSettingCompositeDTO> GetAllProcessGroupFromMstGenProcSettLocAndMstGenProcSettAnfMstGenProcByLocation(string locationCode)
        {
            var proLoc = _mstGenProcessSettingLocationRepo.Get(l => l.LocationCode == locationCode).FirstOrDefault();
            if (proLoc == null)
                return new List<MstGenProcessSettingCompositeDTO>();
            var result = proLoc.MstGenProcessSettings.Where(p => p.MstGenProcess.StatusActive == true).OrderBy(o => o.MstGenProcess.ProcessOrder);
            //var mstGenProcessSettingLocation = _mstGenProcessSettingLocationRepo.Get().Where(m => m.LocationCode == locationCode).Select(m => m.IDProcess);
            //var result = _mstGenProcessSettingRepo.Get().Where(m => m.MstGenProcess.StatusActive == true && mstGenProcessSettingLocation.Contains(m.IDProcess)).OrderBy(m => m.MstGenProcess.ProcessOrder);
            return Mapper.Map<List<MstGenProcessSettingCompositeDTO>>(result);
        }

        public List<int> GetProcessSettingIDProcess(bool increment = false)
        {
            var processIDList = new List<int>();
            int maxProcessID = 0;
            var dbResult = _mstGenProcessSettingRepo.Get().Select(p => p.IDProcess).Distinct();
            if (dbResult.Any())
            {
                processIDList.AddRange(dbResult.ToList());
                maxProcessID = dbResult.Max();
            }
            if (increment)
                processIDList.Add(maxProcessID + 1); ;
            return processIDList;
        }

        #endregion

        #region Master Absent Type
        public List<MstPlantAbsentTypeDTO> GetMstPlantAbsentTypes(GetMstAbsentTypeInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantAbsentType>();

            if (!string.IsNullOrEmpty(input.AbsentType))
                //queryFilter = queryFilter.And(m => m.AbsentType.Contains(input.AbsentType) && m.AlphaReplace != "" && m.AlphaReplace != null);
            queryFilter = queryFilter.And(m => m.AbsentType.Contains(input.AbsentType));

            if (input.ActiveInAbsent != null)
                //queryFilter = queryFilter.And(m => m.ActiveInAbsent == input.ActiveInAbsent && m.AlphaReplace != "" && m.AlphaReplace != null);
            queryFilter = queryFilter.And(m => m.ActiveInAbsent == input.ActiveInAbsent);

            if (input.ActiveInEntry != null)
                //queryFilter = queryFilter.And(m => m.ActiveInProductionEntry == input.ActiveInEntry && m.AlphaReplace != "" && m.AlphaReplace != null);
                queryFilter = queryFilter.And(m => m.ActiveInProductionEntry == input.ActiveInEntry);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantAbsentType>();

            var dbResult = _mstPlantAbsentTypeRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstPlantAbsentTypeDTO>>(dbResult);
        }

        public MstPlantAbsentTypeDTO InsertMstPlantAbsentType(MstPlantAbsentTypeDTO absentTypeDto)
        {
            ValidationInsertMstPlantAbsentType(absentTypeDto);

            var db = Mapper.Map<MstPlantAbsentType>(absentTypeDto);

            db.CreatedDate = DateTime.Now;
            db.UpdatedDate = DateTime.Now;

            _mstPlantAbsentTypeRepo.Insert(db);
            _uow.SaveChanges();
            return absentTypeDto;
        }

        private void ValidationInsertMstPlantAbsentType(MstPlantAbsentTypeDTO absentTypeDto)
        {
            var db = _mstPlantAbsentTypeRepo.GetByID(absentTypeDto.AbsentType);
            if (db != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        public MstPlantAbsentTypeDTO UpdateMstPlantAbsentType(MstPlantAbsentTypeDTO absentTypeDto)
        {
            var newAbsentType = new MstPlantAbsentType();

            var oldDataDB = _mstPlantAbsentTypeRepo.GetByID(absentTypeDto.OldAbsentType);
            if (oldDataDB == null) throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            using (SKTISEntities context = new SKTISEntities())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        newAbsentType = Mapper.Map(absentTypeDto, newAbsentType);
                        newAbsentType.UpdatedDate = DateTime.Now;
                        newAbsentType.CreatedDate = DateTime.Now;
                        newAbsentType.CreatedBy = newAbsentType.UpdatedBy;
                        
                        var oldData = context.MstPlantAbsentTypes.SingleOrDefault(c => c.AbsentType == absentTypeDto.OldAbsentType);
                        if (oldData != null)
                        {
                            newAbsentType.CreatedBy = oldData.CreatedBy;
                            newAbsentType.CreatedDate = oldData.CreatedDate;
                            context.MstPlantAbsentTypes.Remove(oldData);
                            context.SaveChanges();
                        }

                        var checkExistingData = context.MstPlantAbsentTypes.SingleOrDefault(c => c.AbsentType == absentTypeDto.AbsentType);
                        if (checkExistingData != null) { throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist); }

                        context.MstPlantAbsentTypes.Add(newAbsentType);

                        context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw e;
                    }
                }
            }

            return Mapper.Map<MstPlantAbsentTypeDTO>(newAbsentType);
        }

        public List<MstPlantAbsentTypeDTO> GetAllAbsentTypesActiveInEblek()
        {
            var queryFilter = PredicateHelper.True<MstPlantAbsentType>();

            queryFilter = queryFilter.And(m => m.ActiveInProductionEntry == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "AbsentType" }, "ASC");
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantAbsentType>();

            var dbResult = _mstPlantAbsentTypeRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstPlantAbsentTypeDTO>>(dbResult);
        }

        public MstPlantAbsentTypeDTO GetMstPlantAbsentTypeById(string absentType)
        {
            var absentype = _mstPlantAbsentTypeRepo.GetByID(absentType);
            return Mapper.Map<MstPlantAbsentTypeDTO>(absentype);
        }

        public List<MstPlantAbsentTypeDTO> GetMstPlantAbsentTypesWithoutSLSorSLP(GetMstAbsentTypeInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantAbsentType>();

            if (!string.IsNullOrEmpty(input.AbsentType))
                queryFilter = queryFilter.And(m => m.AbsentType.Contains(input.AbsentType));

            if (input.ActiveInAbsent != null)
                queryFilter = queryFilter.And(m => m.ActiveInAbsent == input.ActiveInAbsent);

            var mstHoliday = GetMstHolidayByID(DateTime.Now, Enums.DayType.Holiday.ToString().ToLower(), input.LocationCode);

            var absentSakit = EnumHelper.GetDescription(Enums.SKTAbsentCode.S).ToLower();

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || mstHoliday != null)
                queryFilter = queryFilter.And(m => m.AbsentType.ToLower() != absentSakit);

            //queryFilter = queryFilter.And(m => m.SktAbsentCode != Enums.SKTAbsentCode.SLP.ToString() && m.SktAbsentCode != Enums.SKTAbsentCode.SLS.ToString());

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantAbsentType>();

            var dbResult = _mstPlantAbsentTypeRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstPlantAbsentTypeDTO>>(dbResult);
        }

        public List<MstPlantAbsentTypeDTO> GetAllAbsentTypesActiveInAbsent()
        {
            var queryFilter = PredicateHelper.True<MstPlantAbsentType>();

            queryFilter = queryFilter.And(m => m.ActiveInAbsent == true);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "AbsentType" }, "ASC");
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantAbsentType>();

            var dbResult = _mstPlantAbsentTypeRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstPlantAbsentTypeDTO>>(dbResult);
        }

        public List<MstPlantAbsentTypeDTO> GetAllAbsentTypesActiveInEblekOnly()
        {
            var queryFilter = PredicateHelper.True<MstPlantAbsentType>();

            queryFilter = queryFilter.And(m => m.ActiveInProductionEntry == true && m.ActiveInAbsent == false);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { "AbsentType" }, "ASC");
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantAbsentType>();

            var dbResult = _mstPlantAbsentTypeRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstPlantAbsentTypeDTO>>(dbResult);
        }

        public bool GetAllAbsentTypesGetCalCulation(string id)
        {
            var data = _mstPlantAbsentTypeRepo.GetByID(id);
            if (data == null)
            {
                return false;
            }
            return data.Calculation == "T";
        }

        public List<MstPlantAbsentTypeDTO> GetAllAbsentTypeCalCulation()
        {
            var data = _mstPlantAbsentTypeRepo.Get();
            if (data == null)
            {
                return null;
            }
            return Mapper.Map<List<MstPlantAbsentTypeDTO>>(data);
        }

        #endregion

        #region Master Maintenance Convert


        #endregion

        #region Master GenWeek
        /// <summary>
        /// Get Week Id between year and month
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<int> GetWeekByYear(int year)
        {
            var dbResult = _mstGenWeekRepo.Get(week => week.Year == year);
            return dbResult.Select(w => w.Week != null ? (int)w.Week : 0).ToList();
        }

        public string GetProcessGroup(string locationCode, string unitCode, string groupCode)
        {
            var dbResult = _mstPlantProductionGroupRepo.Get(x => x.LocationCode == locationCode && x.UnitCode == unitCode && x.GroupCode == groupCode).ToList();
            var processGroup = dbResult.Select(x => x.ProcessGroup).SingleOrDefault();
            return processGroup;
        }
        /// <summary>
        /// get 13 week from database genweek
        /// </summary>
        /// <param name="year"></param>
        /// <param name="startWeek"></param>
        /// <returns></returns>
        public List<int> Get13Weeks(int year, int startWeek)
        {
            var weeks = new List<int>();
            var dbWeek = _mstGenWeekRepo.Get(w => w.Year == year && w.Week >= startWeek && w.Week <= startWeek + 12);

            weeks.AddRange(dbWeek.Select(w => w.Week != null ? (int)w.Week : 0));

            var restWeek = 13 - weeks.Count;
            if (restWeek <= 0) return weeks;

            dbWeek = _mstGenWeekRepo.Get(w => w.Year == year && w.Week >= 1 && w.Week <= restWeek);
            weeks.AddRange(dbWeek.Select(w => w.Week != null ? (int)w.Week : 0));

            return weeks;
        }

        /// <summary>
        /// get valid week depend on year
        /// </summary>
        /// <param name="year"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        public bool IsValidWeek(int year, int week)
        {
            var dbWeek = _mstGenWeekRepo.Get(w => w.Year == year && w.Week == week).ToList();
            return (dbWeek.Count > 0);
        }

        public List<MstGenWeekDTO> GetWeekByMonth(int month)
        {
            return _mstGenWeekRepo.Get(m => m.Month == month).Select(p => new MstGenWeekDTO
            {
                Week = p.Week
            }).ToList();
        }

        /// <summary>
        /// Gets the general week years.
        /// </summary>
        /// <returns></returns>
        public List<int?> GetGeneralWeekYears()
        {
            var mstGenWeek = _mstGenWeekRepo.Get(m => m.Year.HasValue);
            return mstGenWeek.Select(m => m.Year).Distinct().ToList();
        }

        public int? GetGeneralWeekWeekByDate(DateTime date)
        {
            int week = 1;
            var dbResult = _mstGenWeekRepo.Get(m => (m.Year == DateTime.Now.Year && m.Month == DateTime.Now.Month) ||
                (m.Year == DateTime.Now.Year && m.Month == DateTime.Now.Month - 1) ||
                (m.Year == DateTime.Now.Year - 1 && m.Month == 12));
            foreach (var mstGenWeek in dbResult.Where(mstGenWeek => date.Date >= mstGenWeek.StartDate && date.Date <= mstGenWeek.EndDate))
            {
                week = mstGenWeek.Week.HasValue ? (int)mstGenWeek.Week : 0;
                break;
            }
            return week;
        }

        public MstGenWeekDTO GetWeekByYearAndWeek(int year, int week)
        {
            return _mstGenWeekRepo.Get(m => m.Year == year && m.Week == week).Select(m => new MstGenWeekDTO { StartDate = m.StartDate, EndDate = m.EndDate }).SingleOrDefault();
        }

        public List<DateTime> GetDateByWeek(int year, int week)
        {
            var result = new List<DateTime>();
            var dbgenweek = _mstGenWeekRepo.Get(m => m.Year == year && m.Week == week).Select(m => new MstGenWeekDTO { StartDate = m.StartDate, EndDate = m.EndDate }).SingleOrDefault();
            if (dbgenweek != null)
            {
                var firstDay = (DateTime)dbgenweek.StartDate;
                for (int i = 0; i < 7; i++)
                {
                    result.Add(firstDay.AddDays(i));
                }
            }
            return result;
        }

        public DateTime GetClosingPayrollBeforeTodayDateTime(DateTime now)
        {
            var closingPayroll =
                _mstClosingPayrollRepo.Get(m => m.ClosingDate < now.Date)
                    .Select(m => new MstClosingPayrollDTO { ClosingDate = m.ClosingDate })
                    .Max(m => m.ClosingDate);

            return closingPayroll;
        }

        public DateTime GetClosingPayrollThreePeriodBeforeLastClosingPayroll(DateTime now)
        {
            var lastClosing = GetClosingPayrollBeforeTodayDateTime(now.Date);
            var closingPayroll =
                _mstClosingPayrollRepo.Get(m => m.ClosingDate < lastClosing)
                    .Select(m => new MstClosingPayrollDTO { ClosingDate = m.ClosingDate })
                    .Max(m => m.ClosingDate);

            return closingPayroll;
        }

        public DateTime GetClosingPayrollAfterTodayDateTime(DateTime now)
        {
            var closingPayroll =
                _mstClosingPayrollRepo.Get(m => m.ClosingDate >= now.Date)
                    .Select(m => new MstClosingPayrollDTO { ClosingDate = m.ClosingDate })
                    .Min(m => m.ClosingDate);
            return closingPayroll;
        }

        public List<DateTime> GetNearestClosingPayrollDate(DateTime now)
        {
            var result = new List<DateTime>();
            var nearestClosingDate =
                _mstClosingPayrollRepo.Get(m => m.ClosingDate < now.Date)
                    .Select(m => new MstClosingPayrollDTO { ClosingDate = m.ClosingDate })
                    .Max(m => m.ClosingDate);
            var beforeClosingDate =
                _mstClosingPayrollRepo.Get(m => m.ClosingDate < nearestClosingDate.Date)
                    .Select(m => new MstClosingPayrollDTO { ClosingDate = m.ClosingDate })
                    .Max(m => m.ClosingDate)
                    .AddDays(1);
            int days = (Int32)(nearestClosingDate - beforeClosingDate).TotalDays;

            if (days != 0)
            {
                for (int i = 0; i <= days; i++)
                {
                    result.Add(beforeClosingDate.AddDays(i));
                }
            }

            return result;
        }

        public List<DateTime> GetClosingPayrollOnePeriode(DateTime now)
        {
            var result = new List<DateTime>();
            var closingDateBefore =
                _mstClosingPayrollRepo.Get(m => m.ClosingDate < now.Date)
                    .Select(m => new MstClosingPayrollDTO { ClosingDate = m.ClosingDate })
                    .Max(m => m.ClosingDate)
                    .AddDays(1);
           
           var closingDateAfter =
               _mstClosingPayrollRepo.Get(m => m.ClosingDate >= now.Date)
                   .Select(m => new MstClosingPayrollDTO { ClosingDate = m.ClosingDate })
                   .Min(m=>m.ClosingDate);
            
            var days = (closingDateAfter - closingDateBefore).TotalDays;

            if (days != 0)
            {
                for (int i = 0; i <= days; i++)
                {
                    result.Add(closingDateBefore.AddDays(i));
                }
            }
            return result;
        }

        public DateTime GetFirstDateByYearWeek(int year, int week)
        {
            var dbgenweek = _mstGenWeekRepo.Get(m => m.Year == year && m.Week == week).Select(m => new MstGenWeekDTO { StartDate = m.StartDate, EndDate = m.EndDate }).SingleOrDefault();
            if (dbgenweek != null)
            {
                return (DateTime)dbgenweek.StartDate;
            }
            return new DateTime();
        }

        public MstGenWeekDTO GetWeekByDate(DateTime date)
        {
            var dbResult = _mstGenWeekRepo.Get(week => week.StartDate <= date.Date && week.EndDate >= date.Date).FirstOrDefault();
            return Mapper.Map<MstGenWeekDTO>(dbResult);
        }

        public List<MstGenWeekDTO> GetWeekByDateRange(DateTime date, DateTime dateTo)
        {
            var dbResult = _mstGenWeekRepo.Get(week => (week.StartDate <= date.Date && week.EndDate >= date.Date)
                || (week.StartDate <= dateTo.Date && week.EndDate >= dateTo.Date)
                || (week.StartDate >= date.Date && week.EndDate <= dateTo.Date));
            return Mapper.Map<List<MstGenWeekDTO>>(dbResult);
        }

        public MstGenWeekDTO GetDatesFromMonthRange(int StartMonth, int EndMonth, int Year)
        {
            var startDate = _mstGenWeekRepo.Get(m => m.Year == Year && m.Month == StartMonth).Select(m => new MstGenWeekDTO { StartDate = m.StartDate, EndDate = m.EndDate }).Min(m => m.StartDate);
            var endDate = _mstGenWeekRepo.Get(m => m.Year == Year && m.Month == EndMonth).Select(m => new MstGenWeekDTO { StartDate = m.StartDate, EndDate = m.EndDate }).Max(m => m.EndDate);
            var Result = new MstGenWeekDTO() { StartDate = startDate, EndDate = endDate };
            return Result;
        }
        public MstGenWeekDTO GetDatesFromWeekRange(int StartWeek, int EndWeek, int Year)
        {
            var startDate = _mstGenWeekRepo.Get(m => m.Year == Year && m.Week == StartWeek).Select(m => new MstGenWeekDTO { StartDate = m.StartDate, EndDate = m.EndDate }).FirstOrDefault().StartDate;
            var endDate = _mstGenWeekRepo.Get(m => m.Year == Year && m.Week == EndWeek).Select(m => new MstGenWeekDTO { StartDate = m.StartDate, EndDate = m.EndDate }).FirstOrDefault().EndDate;
            var Result = new MstGenWeekDTO() { StartDate = startDate, EndDate = endDate };
            return Result;
        }


        #endregion

        #region Master Gen Brand
        //Get BrandGroupCode in MstGenBrand
        public MstGenBrand GetMstGenByBrandCode(string brandCode)
        {
            var genBrand = _mstGenBrandRepo.GetByID(brandCode);
            return genBrand;
        }

        public List<BrandDTO> GetMstGenBrandByBrandCodes(List<string> brandCode)
        {
            var queryFilter = PredicateHelper.True<MstGenBrand>();

            queryFilter = queryFilter.And(q => brandCode.Contains(q.BrandCode));

            var genBrand = _mstGenBrandRepo.Get(queryFilter);

            return Mapper.Map<List<BrandDTO>>(genBrand); ;
        }
        #endregion

        /// <summary>
        /// Gets all process setting by parent location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public List<MstGenProcessSettingDTO> GetAllProcessSettingByParentLocationCode(string locationCode)
        {
            var queryFilter = PredicateHelper.True<MstGenProcessSettingsLocation>();

            var locationCodes = GetAllLocationByLocationCode(locationCode, -1).Select(m => m.LocationCode);
            queryFilter = queryFilter.And(m => locationCodes.Contains(m.LocationCode));


            var dbResult = _mstGenProcessSettingLocationRepo
                .Get(queryFilter)
                .SelectMany(l => l.MstGenProcessSettings.Where(
                    w => !string.IsNullOrEmpty(w.ProcessGroup)
                ))
                .Distinct();
            //.Select(p => p.MstGenProcessSettings);

            return Mapper.Map<List<MstGenProcessSettingDTO>>(dbResult);
        }

        public MstClosingPayrollDTO GetMasterClosingPayroll(GetMstClosingPayrollInput input)
        {
            var queryFilter = PredicateHelper.True<MstClosingPayroll>();

            if (input.StartDate.HasValue && input.EndDate.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ClosingDate >= input.StartDate && p.ClosingDate <= input.EndDate);
            }

            var closingPayroll = _mstClosingPayrollRepo.Get(queryFilter).OrderBy(m => m.ClosingDate).FirstOrDefault();
            return Mapper.Map<MstClosingPayrollDTO>(closingPayroll);
        }

        public MstClosingPayrollDTO GetMasterClosingPayrollByDate(DateTime? date)
        {
            var queryFilter = PredicateHelper.True<MstClosingPayroll>();

            if (date.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ClosingDate >= date);
            }

            var closingPayroll = _mstClosingPayrollRepo.Get(queryFilter).OrderBy(m => m.ClosingDate).FirstOrDefault();
            return Mapper.Map<MstClosingPayrollDTO>(closingPayroll);
        }

        public List<MstClosingPayrollDTO> GetMasterClosingPayrolls(GetMstClosingPayrollInput input)
        {
            var queryFilter = PredicateHelper.True<MstClosingPayroll>();

            if (input.Year.HasValue)
            {
                queryFilter = queryFilter.And(p => p.ClosingDate.Year == input.Year);
            }

            var dbResult = _mstClosingPayrollRepo.Get(queryFilter).OrderByDescending(p => p.ClosingDate);
            return Mapper.Map<List<MstClosingPayrollDTO>>(dbResult);
        }

        public MstClosingPayrollDTO SaveMasterClosingPayroll(MstClosingPayrollDTO input)
        {
            var dbResult = _mstClosingPayrollRepo.GetByID(input.ClosingDate);
            if (dbResult != null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataAlreadyExist);
            }

            var data = Mapper.Map<MstClosingPayroll>(input);

            data.CreatedDate = DateTime.Now;
            data.UpdatedDate = DateTime.Now;

            _mstClosingPayrollRepo.Insert(data);
            _uow.SaveChanges();

            return input;
        }

        public MstClosingPayrollDTO DeleteMasterClosingPayroll(MstClosingPayrollDTO input)
        {
            var dbResult = _mstClosingPayrollRepo.GetByID(input.ClosingDate);
            if (dbResult == null)
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);
            }

            var data = Mapper.Map<MstClosingPayroll>(input);

            _mstClosingPayrollRepo.Delete(data.ClosingDate);
            _uow.SaveChanges();

            return input;
        }

        public IEnumerable<string> GetAllClosingDatePayroll()
        {
            var dbResult = _mstClosingPayrollRepo.Get().OrderByDescending(c => c.ClosingDate.Date).Select(c => c.ClosingDate.Date.ToShortDateString());
            return dbResult;
        }

        public MstGenWeekDTO GetClosingDatePayrollByYear(int year, int week)
        {
            var GenWeek = _mstGenWeekRepo
                .Get(c => c.Year == year && c.Week == week)
                .FirstOrDefault();

            var data = Mapper.Map<MstGenWeekDTO>(GenWeek);

            return data;
        }

        /// <summary>
        /// Gets brand group code by parent location code and process.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public List<MstGenBrandGroupDTO> GetSKTBrandCode(string locationCode, string process)
        {
            var locationCodes = GetAllLocationByLocationCode(locationCode, -1).Select(m => m.LocationCode);
            var queryFilter = PredicateHelper.True<MstGenProcessSettingsLocation>();

            queryFilter = queryFilter.And(m => locationCodes.Contains(m.LocationCode));

            var MstGenProcessSettingsLocationResult = _mstGenProcessSettingLocationRepo.Get(queryFilter).SelectMany(l => l.MstGenProcessSettings).Distinct();
            var MstGenBrandGroupResult = _brandGroupRepository.Get(null, null);

            var dbResult = from a in MstGenBrandGroupResult
                           join b in MstGenProcessSettingsLocationResult on a.BrandGroupCode equals b.BrandGroupCode
                           where b.ProcessGroup == process
                           select a;

            return Mapper.Map<List<MstGenBrandGroupDTO>>(dbResult);
        }

        /// <summary>
        /// Gets brand group code by parent location code
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public List<MstGenBrandGroupDTO> GetSKTBrandCodeLocation(string locationCode)
        {
            var locationCodes = GetAllLocationByLocationCode(locationCode, -1).Select(m => m.LocationCode);
            var queryFilter = PredicateHelper.True<MstGenProcessSettingsLocation>();

            queryFilter = queryFilter.And(m => locationCodes.Contains(m.LocationCode));

            var mstGenProcessSettingsLocationResult = _mstGenProcessSettingLocationRepo.Get(queryFilter).SelectMany(l => l.MstGenProcessSettings).Distinct();
            var mstGenBrandGroupResult = _brandGroupRepository.Get(null, null);

            var dbResult = from a in mstGenBrandGroupResult
                           join b in mstGenProcessSettingsLocationResult on a.BrandGroupCode equals b.BrandGroupCode
                           select a;

            return Mapper.Map<List<MstGenBrandGroupDTO>>(dbResult);
        }


        #region TPOFeeSettingCalculation
        public List<TPOFeeSettingCalculationDTO> GetTpoFeeSettingCalculations()
        {
            var dbResult = _tpoFeeSettingCalculationRepo.Get();
            return Mapper.Map<List<TPOFeeSettingCalculationDTO>>(dbResult);
        }

        public TPOFeeSettingCalculationDTO UpdateTPOFeeSettingCalculation(TPOFeeSettingCalculationDTO dto)
        {
            var db = _tpoFeeSettingCalculationRepo.GetByID(dto.ID);
            if (db == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            dto.CreatedDate = DateTime.Now;
            dto.UpdatedDate = DateTime.Now;

            Mapper.Map(dto, db);
            _tpoFeeSettingCalculationRepo.Update(db);

            _uow.SaveChanges();

            return Mapper.Map<TPOFeeSettingCalculationDTO>(db);
        }
        #endregion

        public List<MstProcessBrandViewDTO> GetAllProcessSettingsByBrandCode(string locationCode, string brandCode)
        {
            var queryFilter = PredicateHelper.True<MstProcessBrandView>();

            queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            queryFilter = queryFilter.And(m => m.BrandCode == brandCode);

            var dbResult = _mstProcessBrandViewRepo.Get(queryFilter);

            return Mapper.Map<List<MstProcessBrandViewDTO>>(dbResult);
        }

        public MstPlantProductionGroupDTO GetPlantProductionGroupById(string prodGroup, string unitCode, string locationCode, string processGroup)
        {
            var dbTpoProductionGroup = _mstPlantProductionGroupRepo.GetByID(prodGroup, unitCode, locationCode, processGroup);

            return Mapper.Map<MstPlantProductionGroupDTO>(dbTpoProductionGroup);
        }

        public bool IsHOlidayOrSunday(DateTime date, String locationCode)
        {
            var queryFilter = PredicateHelper.True<MstGenHoliday>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            if (date != null)
                queryFilter = queryFilter.And(m => m.HolidayDate == date);

            queryFilter = queryFilter.And(m => m.StatusActive == true);

            var dbResult = _mstHoliday.Get(queryFilter).FirstOrDefault();
            if ((dbResult != null) || (int)date.DayOfWeek == 0)
            {
                return true;
            }
            return false;
        }

        #region For report Available Position Number
        public List<MstGenLocStatu> GetGenLocStatusByLocationCode(string locationCode)
        {
            var queryFilter = PredicateHelper.True<MstGenLocStatu>();

            if (!string.IsNullOrEmpty(locationCode))
                queryFilter = queryFilter.And(c => c.LocationCode == locationCode);

            var dbResult = _mstGenLocStatusRepo.Get(queryFilter);

            return dbResult.ToList();
        }

        public List<LocationInfoDTO> GetLocationInfoParent(string locationCode)
        {
            var locations = GetLocationsByLevel(locationCode, 1);
            return locations.Select(location => new LocationInfoDTO
            {
                Text = location.LocationCompat,
                Value = location.LocationCode
            }).ToList();
        }
        #endregion

        public List<AdjustmentTypeByBrandCode> GetMstGeneralLists(string brandCode)
        {
            var adjType = _adjustmentTypeByBrandCode.Get(c => c.BrandCode == brandCode);

            //if (adjType.Count() == 0)
            //{
            //    var adjTypeNull = _adjustmentTypeByBrandCode.Get(c => c.ProcessID == "0");
            //    return adjTypeNull.ToList();
            //}

            return adjType.ToList();

        }



    }
}

