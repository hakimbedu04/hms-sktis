using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using System.Web.SessionState;

namespace HMS.SKTIS.BLL
{
    public partial class MasterDataBLL
    {
        #region Location

        /// <summary>
        /// Gets the MST location by identifier.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        //todo: throw bll exception if location not found and use the repo instead uow 
        public MstGenLocation GetMstLocationById(string locationCode)
        {
            var repo = _uow.GetGenericRepository<MstGenLocation>();
            return repo.GetByID(locationCode);
        }

        /// <summary>
        /// Gets the MST gen locations by parent code.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MstGenLocationDTO> GetMstGenLocationsByParentCode(GetMstGenLocationsByParentCodeInput input, bool responsibilities = true)
        {
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var dbLocations = _sqlSPRepo.GetMstGenLocationsByParentCode(input).ToList();

            if (responsibilities)
            {
                var location = strUserID.Location.Select(s => s.Code).ToList();
                dbLocations = dbLocations.Where(w => location.Contains(w.LocationCode)).ToList();
            }
            return Mapper.Map<List<MstGenLocationDTO>>(dbLocations);
        }

        /// <summary>
        /// Gets the MST location lists.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MstGenLocationDTO> GetMstLocationLists(GetMstGenLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenLocation>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenLocation>();
            var dbResult = _mstLocationRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<MstGenLocationDTO>>(dbResult);
        }

        public MstGenLocationDTO GetMstGenLocation(GetMstGenLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenLocation>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.ParentLocationCode))
            {
                queryFilter = queryFilter.And(p => p.ParentLocationCode == input.ParentLocationCode);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenLocation>();

            var dbResult = _mstLocationRepo.Get(queryFilter, orderByFilter).FirstOrDefault();
            return Mapper.Map<MstGenLocationDTO>(dbResult);
        }

        /// <summary>
        /// Get all Location under location code we input as parameter
        /// </summary>
        /// <param name="locationCode">Location code</param>
        /// <param name="level">level (-1) for all child</param>
        /// <returns></returns>
        public List<MstGenLocationDTO> GetAllLocationByLocationCode(string locationCode, int level)
        {
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var locations = Mapper.Map<List<MstGenLocationDTO>>(_sqlSPRepo.GetLocations(locationCode, level));
            var location = strUserID.Location.Select(s => s.Code).ToList();
            var list = locations.Where(w => location.Contains(w.LocationCode)).ToList();
            return list;
        }

        /// <summary>
        /// Inserts the location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        public MstGenLocationDTO InsertLocation(MstGenLocationDTO location)
        {
            ValidateInsertMstGenLocation(location);

            var dbLocation = Mapper.Map<MstGenLocation>(location);

            _mstLocationRepo.Insert(dbLocation);

            dbLocation.CreatedDate = DateTime.Now;
            dbLocation.UpdatedDate = DateTime.Now;

            SaveTpoInfoAfterSaveLocation(dbLocation);
            _uow.SaveChanges();

            return Mapper.Map(dbLocation, location);
        }

        /// <summary>
        /// Validates the insert MST gen location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <exception cref="BLLException"></exception>
        private void ValidateInsertMstGenLocation(MstGenLocationDTO location)
        {
            var dbMstGenLocation = _mstLocationRepo.GetByID(location.LocationCode);

            if (dbMstGenLocation != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);

            ValidationLocationAndParentSameMstGenLocation(location);
        }

        /// <summary>
        /// Updates the location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        /// <exception cref="BLLException">
        /// </exception>
        public MstGenLocationDTO UpdateLocation(MstGenLocationDTO location)
        {
            var dbLocation = _mstLocationRepo.GetByID(location.LocationCode);
            if (dbLocation == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            location.CreatedBy = dbLocation.CreatedBy;
            location.CreatedDate = dbLocation.CreatedDate;

            // Check Location Code and Parent Location Can't be same
            ValidationLocationAndParentSameMstGenLocation(location);

            ValidationTPOInfoWhenUpdatedLocation(location, dbLocation);

            //set update time
            location.UpdatedDate = DateTime.Now;

            Mapper.Map(location, dbLocation);
            dbLocation.UpdatedDate = DateTime.Now;
            try
            {
                _mstLocationRepo.Update(dbLocation);

                //SaveTpoInfoAfterSaveLocation(dbLocation);

                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.HResult);
            }

            return location;
        }

        /// <summary>
        /// Validations the location and parent same MST gen location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <exception cref="BLLException"></exception>
        private void ValidationLocationAndParentSameMstGenLocation(MstGenLocationDTO location)
        {
            if (location.LocationCode == location.ParentLocationCode)
                throw new BLLException(ExceptionCodes.BLLExceptions.LocationCodeAndParentSame);
        }

        /// <summary>
        /// Validations the tpo information when updated location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="dbLocation">The database location.</param>
        /// <exception cref="BLLException"></exception>
        private void ValidationTPOInfoWhenUpdatedLocation(MstGenLocationDTO location, MstGenLocation dbLocation)
        {
            var loc = Mapper.Map<MstGenLocation>(location);
            if (!CheckChildTpo(loc, location.LocationCode) && CheckChildTpo(dbLocation, dbLocation.LocationCode))
                throw new BLLException(ExceptionCodes.BLLExceptions.ParentTPOCanNotBeChanged);

            SaveTpoInfoAfterSaveLocation(loc);
        }

        /// <summary>
        /// Saves the tpo information after save location.
        /// </summary>
        /// <param name="locationDto">The location dto.</param>
        public void SaveTpoInfoAfterSaveLocation(MstGenLocation locationDto)
        {

            if (!CheckChildTpo(locationDto, locationDto.LocationCode)) return;

            var dbTpoInfo = _mstTpointoRepo.GetByID(locationDto.LocationCode);
            if (dbTpoInfo != null) return;
            var tpoInfo = new MstTPOInfo
            {
                LocationCode = locationDto.LocationCode,
                VendorNumber = "",
                Established = DateTime.Now,
                CreatedBy = locationDto.CreatedBy,
                CreatedDate = DateTime.Now,
                UpdatedBy = locationDto.UpdatedBy,
                UpdatedDate = DateTime.Now
            };
            _mstTpointoRepo.Insert(tpoInfo);
            //_uow.SaveChanges();
        }

        /// <summary>
        /// Checks the child tpo.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        private bool CheckChildTpo(MstGenLocation location, string locationCode)
        {
            if (location.LocationCode == locationCode && location.ParentLocationCode == Enums.LocationCode.TPO.ToString()) return true;
            if (string.IsNullOrEmpty(location.ParentLocationCode))
                return false;
            if (location.LocationCode == Enums.LocationCode.TPO.ToString())
                return true;
            var locations = _mstLocationRepo.GetByID(location.ParentLocationCode);
            return CheckChildTpo(locations, location.ParentLocationCode);
        }

        public List<string> GetAllLocationCode()
        {
            var queryFilter = PredicateHelper.True<MstGenLocation>();
            Func<IQueryable<MstGenLocation>, IOrderedQueryable<MstGenLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstGenLocation>;

            var dbResult = _mstLocationRepo.Get(queryFilter).Select(m => m.LocationCode).Distinct();

            return dbResult.ToList();
        }

        public List<MstGenLocationDTO> GetAllLocationCodeInfo()
        {
            var queryFilter = PredicateHelper.True<MstGenLocation>();
            string[] rejectlist = new string[] { "SKT", "PLNT"};
            string[] ParentRejectlist = new string[] {"PLNT"};
            queryFilter =   queryFilter.And(c=>!rejectlist.Contains(c.LocationCode));
            queryFilter = queryFilter.And(d => !ParentRejectlist.Contains(d.ParentLocationCode));
            Func<IQueryable<MstGenLocation>, IOrderedQueryable<MstGenLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstGenLocation>;

            var dbResult = _mstLocationRepo.Get(queryFilter);

            return Mapper.Map<List<MstGenLocationDTO>>(dbResult);
        }

        /// <summary>
        /// Get list of locationcode
        /// </summary>
        /// <returns>list of LocationCode</returns>
        //todo: we can remove it and use ApplicationService
        public List<string> GetLocationCodes()
        {
            var queryFilter = PredicateHelper.True<MstGenLocation>();
            queryFilter = queryFilter.And(m => m.StatusActive == true);
            Func<IQueryable<MstGenLocation>, IOrderedQueryable<MstGenLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstGenLocation>;
            //Func<IQueryable<MstLocation>, IOrderedQueryable<MstLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstLocation>;

            var dbResult = _mstLocationRepo.Get(queryFilter).Select(m => m.LocationCode).Distinct();

            return dbResult.ToList();
        }

        public List<string> GetPlantLocationCodes()
        {
            var queryFilter = PredicateHelper.True<MstGenLocation>();
            var locations = _sqlSPRepo.GetLastChildLocationByLocationCode(Enums.LocationCode.PLNT.ToString()).Select(m => m.LocationCode);
            queryFilter = queryFilter.And(m => m.StatusActive == true && locations.Contains(m.LocationCode));
            
            Func<IQueryable<MstGenLocation>, IOrderedQueryable<MstGenLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstGenLocation>;
            //Func<IQueryable<MstLocation>, IOrderedQueryable<MstLocation>> orderByFilter = m => m.OrderBy(z => z.LocationCode) as IOrderedQueryable<MstLocation>;

            var dbResult = _mstLocationRepo.Get(queryFilter).Select(m => m.LocationCode).Distinct();

            return dbResult.ToList();
        }



        /// <summary>
        /// Get location for dropdown list only code for text dan value
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        //todo: we can remove it and use ApplicationService
        public List<LocationInfoDTO> GetLocationInfo(string locationCode)
        {
            var locations = GetAllLocationByLocationCode(locationCode, -1);
            return locations.Select(location => new LocationInfoDTO
            {
                Text = location.LocationCompat,
                Value = location.LocationCode
            }).ToList();
        }

        /// <summary>
        /// Get location for dropdown list code for 'Text' and name for 'value'
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        //todo: we can remove it and use ApplicationService
        public List<LocationInfoDTO> GetLocationInfoWithName(string locationCode)
        {
            var locations = GetAllLocationByLocationCode(locationCode, -1);
            return locations.Select(location => new LocationInfoDTO
            {
                Text = location.LocationName,
                Value = location.LocationCode
            }).ToList();
        }

        public List<MstGenLocationDTO> GetAllLocations(GetAllLocationsInput input, bool responsibilities = true)
        {
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            
            var queryFilter = PredicateHelper.True<MstGenLocation>();

            if (!string.IsNullOrEmpty(input.ParentLocationCode))
                queryFilter = queryFilter.And(m => m.ParentLocationCode.Contains(input.ParentLocationCode));

            queryFilter = queryFilter.And(m => m.StatusActive == true);
            //Func<IQueryable<MstGenLocationDTO>, IOrderedQueryable<MstGenLocationDTO>> orderByFilter = m => m.OrderBy(z => z.BrandGroupCode);
            if (responsibilities)
            {
                var location = strUserID.Location.Select(s => s.Code).ToList();
                queryFilter = queryFilter.And(w => location.Contains(w.LocationCode));
            }

            var dbResult = _mstLocationRepo.Get(queryFilter);

            return Mapper.Map<List<MstGenLocationDTO>>(dbResult);
        }

        public List<int> GetShiftByLocationCode(string locationCode)
        {
            var shiftList = new List<int>();
            var location = _mstLocationRepo.GetByID(locationCode);

            if (location == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            if (!location.Shift.HasValue) return shiftList;

            for (var i = 1; i < location.Shift + 1; i++)
            {
                shiftList.Add(i);
            }

            return shiftList;
        }

        public IEnumerable<int> GetShiftFilterByProcess(string location, string unitCode) {
            IEnumerable<int> result = new List<int>();
            var listLocation = GetLastChildLocation(location).Select(c => c.LocationCode);
            using (SKTISEntities context = new SKTISEntities()) {
                if (unitCode == "All") {
                    result = context.ExeReportByProcesses.OrderBy(c => c.Shift).Where(c => listLocation.Contains(c.LocationCode)).Select(c => c.Shift).Distinct().ToList();
                }
                else {
                    result = context.ExeReportByProcesses.OrderBy(c => c.Shift).Where(c => listLocation.Contains(c.LocationCode) && c.UnitCode == unitCode).Select(c => c.Shift).Distinct().ToList();
                }
            }
            return result;
        }


        public List<MstGenLocationDTO> GetLastChildLocation(string parentCode)
        {
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var location = strUserID.Location.Select(s => s.Code).ToList();
            var locations = _sqlSPRepo.GetLastChildLocationByLocationCode(parentCode).Select(m => m.LocationCode);
            var dblocation = _mstLocationRepo.Get(m => locations.Contains(m.LocationCode) && location.Contains(m.LocationCode));
            return Mapper.Map(dblocation, new List<MstGenLocationDTO>());
        }
        public List<MstGenLocationDTO> GetLastChildLocationByTPO()
        {
            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var location = strUserID.Location.Select(s => s.Code).ToList();
            var locations = _sqlSPRepo.GetLastChildLocationByLocationCode(Enums.LocationCode.TPO.ToString()).Select(m => m.LocationCode);
            var dblocation = _mstLocationRepo.Get(m => locations.Contains(m.LocationCode) && location.Contains(m.LocationCode));
            return Mapper.Map(dblocation, new List<MstGenLocationDTO>());
            //return _sqlSPRepo.GetLastChildLocationByLocationCode(Enums.LocationCode.TPO.ToString()).Select(m => m.LocationCode).ToList();
        }

        /// <summary>
        /// Get all Location under location code by level
        /// </summary>
        /// <param name="locationCode">Source Location Code</param>
        /// <param name="level">Hierachy Level</param>
        /// <returns></returns>
        public List<MstGenLocationDTO> GetLocationsByLevel(string sourceLocationCode, int level)
        {

            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var location = strUserID.Location.Select(s => s.Code).ToList();
            var locations = _sqlSPRepo.GetLocationsByLevel(sourceLocationCode, level).Where(s => location.Contains(s.LocationCode)).OrderBy(m => m.LocationCode);

            return Mapper.Map<List<MstGenLocationDTO>>(locations);
        }

        /// <summary>
        /// Get Single Location Information by Location Code
        /// </summary>
        /// <param name="locationCode">Location Code</param>
        /// <returns></returns>
        public MstGenLocationDTO GetLocation(string locationCode)
        {
            var location = _mstLocationRepo.GetByID(locationCode);
            if (location == null)
                return new MstGenLocationDTO() { };
            else
                return Mapper.Map<MstGenLocationDTO>(location);
        }
        /// <summary>
        /// Get Locations of TPO or PLANT
        /// </summary>
        /// <param name="plantOrTPK"></param>
        /// <returns></returns>
        public List<MstGenLocationDTO> GetPlantOrTPOLocations(string plantOrTPK)
        {

            UserSession strUserID = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var location = strUserID.Location.Select(s => s.Code).ToList();
            var locations = _sqlSPRepo.GetLocations(plantOrTPK).Where(s => location.Contains(s.LocationCode));
            return Mapper.Map<List<MstGenLocationDTO>>(locations);
        }

        #endregion

        #region Standard Hour(s)

        public MstGenStandardHourDTO GetStandardHourByDayTypeDay(int day, string dayType)
        {
            var queryFilter = PredicateHelper.True<MstGenStandardHour>();

            if (day != 0)
                queryFilter = queryFilter.And(m => m.Day == day);
            if (!string.IsNullOrEmpty(dayType))
                queryFilter = queryFilter.And(m => m.DayType == dayType);

            var dbResult = _standardHourRepository.Get(queryFilter).FirstOrDefault();
            return Mapper.Map<MstGenStandardHourDTO>(dbResult);
        }
        /// <summary>
        /// Get Standard Hours List
        /// </summary>
        /// <param name="input">BaseInput</param>
        /// <returns>List</returns>
        public List<MstGenStandardHourDTO> GetStandardHours(BaseInput input)
        {
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenStandardHour>();

            var result = _standardHourRepository.Get(null, orderByFilter);

            return Mapper.Map<List<MstGenStandardHourDTO>>(result);
        }

        /// <summary>
        /// Insert Standard Hour
        /// </summary>
        /// <param name="input">object StandardHourDTO</param>
        public MstGenStandardHourDTO InsertStandardHour(MstGenStandardHourDTO input)
        {
            ValidateInsertMstStandardHour(input);

            var data = Mapper.Map<MstGenStandardHour>(input);

            _standardHourRepository.Insert(data);

            data.CreatedDate = DateTime.Now;
            data.UpdatedDate = DateTime.Now;

            data.Day = GenericHelper.ConvertDayToString(data.DayName);
            _uow.SaveChanges();

            return Mapper.Map<MstGenStandardHourDTO>(data);
        }

        public MstGenStandardHourDTO UpdateStandardHour(MstGenStandardHourDTO standardHourDTO)
        {
            var day = GenericHelper.ConvertDayToString(standardHourDTO.DayName);
            var dbStandardHour = _standardHourRepository.GetByID(standardHourDTO.DayType, day);

            if (dbStandardHour == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            // keep original value of CreatedBy & CreatedDate
            standardHourDTO.CreatedBy = dbStandardHour.CreatedBy;
            standardHourDTO.CreatedDate = dbStandardHour.CreatedDate;

            // set UpdatedDate
            standardHourDTO.UpdatedDate = DateTime.Now;

            // update Day
            dbStandardHour.Day = day;

            Mapper.Map(standardHourDTO, dbStandardHour);


            _standardHourRepository.Update(dbStandardHour);
            _uow.SaveChanges();

            return Mapper.Map<MstGenStandardHourDTO>(dbStandardHour);
        }
        private void ValidateInsertMstStandardHour(MstGenStandardHourDTO mstStandardHourToValidate)
        {
            //validate empty day
            if (string.IsNullOrEmpty(mstStandardHourToValidate.DayName))
                throw new BLLException(ExceptionCodes.BLLExceptions.EmptyDay);

            //validate duplicate standard hours
            var day = GenericHelper.ConvertDayToString(mstStandardHourToValidate.DayName);
            var dbMstStandardHour = _standardHourRepository.GetByID(mstStandardHourToValidate.DayType, day);

            if (dbMstStandardHour != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        #endregion

        #region Master Brand

        public BrandCompositeDTO GetBrand(GetBrandInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenBrand>();

            if (!string.IsNullOrEmpty(input.BrandCode))
            {
                queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);
            }

            var dbResult = _mstGenBrandRepo.Get(queryFilter, null, "MstGenBrandGroup").FirstOrDefault();

            return Mapper.Map<BrandCompositeDTO>(dbResult);
        }

        /// <summary>
        /// Gets the brands.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<BrandDTO> GetBrands(GetBrandInput input)
        {
            //var result = _brandRepository.Get();
            var queryFilter = PredicateHelper.True<MstGenBrand>();

            if (!string.IsNullOrEmpty(input.BrandGroupCode))
                queryFilter = queryFilter.And(p => p.BrandGroupCode == input.BrandGroupCode);

            if (!string.IsNullOrEmpty(input.BrandCode))
                queryFilter = queryFilter.And(p => p.BrandCode == input.BrandCode);

            if (input.EffectiveDate != null)
                queryFilter = queryFilter.And(p => p.EffectiveDate >= input.EffectiveDate);

            if (input.ExpiredDate != null)
                queryFilter = queryFilter.And(p => p.ExpiredDate <= input.ExpiredDate);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstGenBrand>();

            var result = _mstGenBrandRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<BrandDTO>>(result);
        }

        /// <summary>
        /// Insert Brand
        /// </summary>
        /// <param name="brand"></param>
        public BrandDTO InsertBrand(BrandDTO brand)
        {
            ValidateInsertMstBrand(brand);
            var db = Mapper.Map<MstGenBrand>(brand);

            _mstGenBrandRepo.Insert(db);

            db.CreatedDate = DateTime.Now;
            db.UpdatedDate = DateTime.Now;

            _uow.SaveChanges();

            return Mapper.Map<BrandDTO>(db);

        }

        private void ValidateInsertMstBrand(BrandDTO brandDto)
        {
            var db = _mstGenBrandRepo.GetByID(brandDto.BrandCode);
            ValidateMstBrand(brandDto);
            if (db != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        private void ValidateMstBrand(BrandDTO brandDTO)
        {
            if ((brandDTO.EffectiveDate > brandDTO.ExpiredDate) || (brandDTO.ExpiredDate < brandDTO.EffectiveDate))
            {
                throw new BLLException(ExceptionCodes.BLLExceptions.ExpirationDateLessThanEffectiveDate);
            }
        }

        /// <summary>
        /// Update brand
        /// </summary>
        /// <param name="brand"></param>
        public BrandDTO UpdateBrand(BrandDTO brand)
        {
            var dbBrand = _mstGenBrandRepo.GetByID(brand.BrandCode);
            ValidateMstBrand(brand);
            if (dbBrand == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            brand.CreatedBy = dbBrand.CreatedBy;
            brand.CreatedDate = dbBrand.CreatedDate;

            Mapper.Map(brand, dbBrand);
            _mstGenBrandRepo.Update(dbBrand);

            dbBrand.UpdatedDate = DateTime.Now;

            _uow.SaveChanges();

            return Mapper.Map<BrandDTO>(dbBrand);
        }

        /// <summary>
        /// Get all active brand group by brand family
        /// </summary>
        /// <param name="brandFamily"></param>
        /// <returns></returns>
        public List<BrandDTO> GetBrandByBrandFamily(string brandFamily = null)
        {
            var queryFilter = PredicateHelper.True<MstGenBrand>();
            queryFilter = queryFilter.And(p => p.MstGenBrandGroup.StatusActive == true);

            if (!string.IsNullOrEmpty(brandFamily))
            {
                queryFilter = queryFilter.And(p => p.MstGenBrandGroup.BrandFamily == brandFamily);
            }

            var dbResult = _mstGenBrandRepo.Get(queryFilter).OrderBy(order => order.BrandGroupCode);
            return Mapper.Map<List<BrandDTO>>(dbResult);
        }

        /// <summary>
        /// Get all active brand by brand group code
        /// </summary>
        /// <param name="brandGroupCode"></param>
        /// <returns></returns>
        public List<BrandDTO> GetBrandByBrandGroupCode(string brandGroupCode)
        {
            var queryFilter = PredicateHelper.True<MstGenBrand>();
            queryFilter = queryFilter.And(p => p.MstGenBrandGroup.StatusActive == true);

            queryFilter = queryFilter.And(p => p.BrandGroupCode == brandGroupCode);

            var dbResult = _mstGenBrandRepo.Get(queryFilter).OrderBy(order => order.BrandGroupCode);
            return Mapper.Map<List<BrandDTO>>(dbResult);
        }

        /// <summary>
        /// Get all brand depend on active brandgroup and brand not expired
        /// </summary>
        /// <returns></returns>
        public List<BrandDTO> GetAllActiveBrand()
        {
            var currentDate = DateTime.Now.Date;
            var queryFilter = PredicateHelper.True<MstGenBrand>();
            queryFilter = queryFilter.And(p => p.MstGenBrandGroup.StatusActive == true && currentDate >= p.EffectiveDate && currentDate <= p.ExpiredDate);
            var dbResult = _mstGenBrandRepo.Get(queryFilter).OrderBy(order => order.BrandGroupCode);
            return Mapper.Map<List<BrandDTO>>(dbResult);
        }

        public List<string> GetBrandCodeByLocationCode(string locationCode)
        {
            var dbResult = _brandCodeByLocationViewRepo.Get(m => m.LocationCode == locationCode);
            return dbResult.Select(m => m.BrandCode).ToList();
        }
        public List<string> GetActiveBrandCodeByLocationCode(string locationCode)
        {
            var dbResult = _brandCodeByLocationViewRepo.Get(m => m.LocationCode == locationCode && m.StatusActive == true);
            return dbResult.OrderBy(b => b.SKTBrandCode).Select(m => m.BrandCode).Distinct().ToList();
        }
        public List<BrandCodeByLocationCodeDTO> GetAllBrandCodeByLocationCode()
        {
            var dbResult = _brandCodeByLocationViewRepo.Get();
            return Mapper.Map<List<BrandCodeByLocationCodeDTO>>(dbResult);
        }

        public BrandDTO GetMstGenBrandById(string brandCode)
        {
            return Mapper.Map<BrandDTO>(_mstGenBrandRepo.GetByID(brandCode));
        }

        public List<BrandDTO> GetAllMstGenBrandWithExpireStillActive(string locationCode)
        {
            var procLoc = _mstGenProcessSettingLocationRepo.Get(l => l.LocationCode == locationCode).FirstOrDefault();
            if (procLoc == null)
                return new List<BrandDTO>();
            var proc = procLoc.MstGenProcessSettings.Select(p => p.BrandGroupCode);
            //var processSettingLocation = _mstGenProcessSettingLocationRepo.Get().Where(m => m.LocationCode == locationCode).Select(m => m.IDProcess);
            //var processSetting = _mstGenProcessSettingRepo.Get().Where(m => processSettingLocation.Contains(m.IDProcess)).Select(m => m.BrandGroupCode);
            var dbResult = _mstGenBrandRepo.Get().Where(m => m.ExpiredDate >= m.EffectiveDate && proc.Contains(m.BrandGroupCode)).OrderBy(m => m.BrandCode);
            return Mapper.Map<List<BrandDTO>>(dbResult);
        }

        public List<MstGenProcessSettingDTO> GetUOMEblekByBrandCode(string brandCode, string locationCode)
        {
            var brand = _mstGenBrandRepo.GetByID(brandCode);
            var brandgroup = "";
            var processsetings = new List<MstGenProcessSetting>();
            if (brand != null)
                brandgroup = brand.BrandGroupCode;

            var processsetinglocation = _mstGenProcessSettingLocationRepo.Get(l => l.LocationCode == locationCode).FirstOrDefault();

            if (processsetinglocation != null)
                processsetings = processsetinglocation.MstGenProcessSettings.Where(p => p.BrandGroupCode == brandgroup).ToList();

            return Mapper.Map<List<MstGenProcessSettingDTO>>(processsetings);
        }

        #endregion

        #region General Week

        public List<MstGenWeekDTO> GetMstGenWeeks(GetMstGenWeekInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenWeek>();
            if (input.Year != null)
            {
                queryFilter = queryFilter.And(p => p.Year == input.Year);
            }

            if (input.Week != null)
            {
                queryFilter = queryFilter.And(p => p.Week == input.Week);
            }

            if (input.Month != null)
            {
                queryFilter = queryFilter.And(p => p.Month == input.Month);
            }

            var dbResult = _mstGenWeekRepo.Get(queryFilter);
            return Mapper.Map<List<MstGenWeekDTO>>(dbResult);
        }

        public MstGenWeekDTO GetMstGenWeek(GetMstGenWeekInput input)
        {
            var queryFilter = PredicateHelper.True<MstGenWeek>();

            if (input.CurrentDate.HasValue)
            {
                queryFilter = queryFilter.And(p => DbFunctions.TruncateTime(p.StartDate) <= DbFunctions.TruncateTime(input.CurrentDate) && DbFunctions.TruncateTime(p.EndDate) >= DbFunctions.TruncateTime(input.CurrentDate));
            }

            var dbResult = _mstGenWeekRepo.Get(queryFilter).FirstOrDefault();

            return Mapper.Map<MstGenWeekDTO>(dbResult);
        }

        #endregion

        #region Master General Brand Group

        public int GetMasterGenBrandGroupPack(string id)
        {
            var data = _brandGroupRepository.GetByID(id);
            var db = Mapper.Map<MstGenBrandGroupDTO>(data);

            return db.StickPerPack.HasValue ? db.StickPerPack.Value : 0;

        }

        #endregion
    }
}
