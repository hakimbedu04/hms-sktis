using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;

namespace HMS.SKTIS.BLL
{
    public partial class MasterDataBLL
    {
        #region Master Plant Production Group

        /// <summary>
        /// Gets the MST plant production groups.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MstPlantProductionGroupCompositeDTO> GetMstPlantProductionGroups(
            GetMstPlantProductionGroupsInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantProductionGroupView>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            if (!string.IsNullOrEmpty(input.ProcessSettingsCode))
                queryFilter = queryFilter.And(m => m.ProcessSettingsCode == input.ProcessSettingsCode);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantProductionGroupView>();

            var dbResult = _mstPlantProductionGroupViewRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstPlantProductionGroupCompositeDTO>>(dbResult);
        }

        /// <summary>
        /// Gets the group code plant productions by process.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        public List<string> GetGroupCodePlantProductionsByProcess(string process)
        {
            var processDto = GetMasterProcessByProcess(process);

            var processIdentifier = 0;
            if (!string.IsNullOrEmpty(processDto.ProcessIdentifier))
                processIdentifier = Convert.ToInt32(processDto.ProcessIdentifier) + 1;

            var queryFilter = PredicateHelper.True<MstPlantProductionGroup>();
            queryFilter = queryFilter.And(m => m.GroupCode.StartsWith(processIdentifier.ToString()));

            Func<IQueryable<MstPlantProductionGroup>, IOrderedQueryable<MstPlantProductionGroup>> orderByFilter = m => m.OrderBy(z => z.GroupCode);

            var dbResult = _mstPlantProductionGroupRepo.Get(queryFilter, orderByFilter).Select(m => m.GroupCode).Distinct();

            return dbResult.ToList();
        }

        public List<string> GetGroupCodePlantProductionByProcessLocationAndUnit(string process, string locationCode, string unitCode)
        {
            var processDto = GetMasterProcessByProcess(process);

            var processIdentifier = 0;
            if (!string.IsNullOrEmpty(processDto.ProcessIdentifier))
                processIdentifier = Convert.ToInt32(processDto.ProcessIdentifier);

            var queryFilter = PredicateHelper.True<MstPlantProductionGroup>();
            queryFilter = queryFilter.And(m => m.GroupCode.StartsWith(processIdentifier.ToString()));
            queryFilter = queryFilter.And(m => m.LocationCode == locationCode);
            queryFilter = queryFilter.And(m => m.UnitCode == unitCode);

            Func<IQueryable<MstPlantProductionGroup>, IOrderedQueryable<MstPlantProductionGroup>> orderByFilter = m => m.OrderBy(z => z.GroupCode);

            var dbResult = _mstPlantProductionGroupRepo.Get(queryFilter, orderByFilter).Select(m => m.GroupCode).Distinct();

            return dbResult.ToList();
        }

        public MstPlantProductionGroupDTO SaveMstPlantProductionGroup(MstPlantProductionGroupDTO plantProductionGroup)
        {
            var dbPlantProductionGroup = _mstPlantProductionGroupRepo.GetByID(
                plantProductionGroup.GroupCode, plantProductionGroup.UnitCode, plantProductionGroup.LocationCode, plantProductionGroup.ProcessGroup);

            // It block should be only executed when user edited the data with Unit or process pointed to "All" which will give empty string ("") as it values
            if (plantProductionGroup.UnitCode == "" || plantProductionGroup.ProcessGroup == "")
            {
                var queryFilter = PredicateHelper.True<MstPlantProductionGroup>();

                if (!string.IsNullOrEmpty(plantProductionGroup.LocationCode))
                    queryFilter = queryFilter.And(m => m.LocationCode == plantProductionGroup.LocationCode);
                if (!string.IsNullOrEmpty(plantProductionGroup.GroupCode))
                    queryFilter = queryFilter.And(m => m.GroupCode == plantProductionGroup.GroupCode);

                dbPlantProductionGroup = _mstPlantProductionGroupRepo.Get(queryFilter).FirstOrDefault();
                plantProductionGroup.UnitCode = dbPlantProductionGroup.UnitCode;
                plantProductionGroup.ProcessGroup = dbPlantProductionGroup.ProcessGroup;
                //dbPlantProductionGroup = Mapper.Map<List<MstPlantProductionGroup>>(dbResult);

                //throw new BLLException(ExceptionCodes.BLLExceptions.UnitAndProcessNotFound);
            }

            if (dbPlantProductionGroup == null)
            {
                dbPlantProductionGroup = Mapper.Map<MstPlantProductionGroup>(plantProductionGroup);
                dbPlantProductionGroup.CreatedDate = DateTime.Now;
                _mstPlantProductionGroupRepo.Insert(dbPlantProductionGroup);
            }
            else
            {
                Mapper.Map(plantProductionGroup, dbPlantProductionGroup);
                _mstPlantProductionGroupRepo.Update(dbPlantProductionGroup);
            }

            dbPlantProductionGroup.UpdatedDate = DateTime.Now;


            _uow.SaveChanges();

            return Mapper.Map(dbPlantProductionGroup, plantProductionGroup);
        }
        /// <summary>
        /// Gets the master plant production groups.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        //public MstPlantProductionGroupDTO InsertMstPlantProductionGroups(MstPlantProductionGroupDTO plantProductionGroup)
        //{
        //    var dbPlantProductionGroup = Mapper.Map<MstPlantProductionGroup>(plantProductionGroup);

        //    dbPlantProductionGroup.CreatedDate = DateTime.Now;
        //    dbPlantProductionGroup.UpdatedDate = DateTime.Now;

        //    var dbProdGroup =
        //        _mstPlantProductionGroupRepository.Get().OrderByDescending(m => m.GroupCode)
        //            .FirstOrDefault(m => m.ProcessGroup == plantProductionGroup.ProcessGroup);
        //    var lastCode = 0;
        //    if (dbProdGroup != null)
        //        lastCode = Convert.ToInt32(dbProdGroup.GroupCode.Substring(1));
        //    lastCode = lastCode + 1;

        //    var dbProcess = _mstProcessRepo.GetByID(plantProductionGroup.ProcessGroup);

        //    dbPlantProductionGroup.GroupCode = dbProcess.ProcessIdentifier + lastCode.ToString("000");
        //    _mstPlantProductionGroupRepository.Insert(dbPlantProductionGroup);
        //    _uow.SaveChanges();
        //    return plantProductionGroup;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="plantProductionGroup"></param>
        ///// <returns></returns>
        //public MstPlantProductionGroupDTO UpdateMstPlantProductionGroups(MstPlantProductionGroupDTO plantProductionGroup)
        //{
        //    var dbPlantProductionGroup = _mstPlantProductionGroupRepository.GetByID(plantProductionGroup.UnitCode,
        //                                                                            plantProductionGroup.LocationCode,
        //                                                                            plantProductionGroup.GroupCode);
        //    if (dbPlantProductionGroup == null)
        //        throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

        //    //keep original CreatedBy and CreatedDate
        //    dbPlantProductionGroup.CreatedBy = dbPlantProductionGroup.CreatedBy;
        //    dbPlantProductionGroup.CreatedDate = dbPlantProductionGroup.CreatedDate;

        //    //set update time
        //    dbPlantProductionGroup.UpdatedDate = DateTime.Now;
        //    Mapper.Map(plantProductionGroup, dbPlantProductionGroup);
        //    _mstPlantProductionGroupRepository.Update(dbPlantProductionGroup);
        //    _uow.SaveChanges();

        //    return Mapper.Map<MstPlantProductionGroupDTO>(dbPlantProductionGroup);
        //}

        public List<MstPlantProductionGroupCompositeDTO> GetMasterPlantProductionGroups(GetMstPlantProductionGroupsInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantProductionGroup>();

            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            if (!string.IsNullOrEmpty(input.UnitCode))
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            if (!string.IsNullOrEmpty(input.ProcessSettingsCode))
                queryFilter = queryFilter.And(m => m.ProcessGroup == input.ProcessSettingsCode);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantProductionGroup>();

            var dbResult = _mstPlantProductionGroupRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstPlantProductionGroupCompositeDTO>>(dbResult);
        }

        #endregion

        #region Employee Jobs Data Active
        /// <summary>
        /// Gets the MST employee jobs data actives.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MstEmployeeJobsDataActiveCompositeDTO> GetMstEmployeeJobsDataActives(GetMstEmployeeJobsDataActivesInput input)
        {

            //var result = (from emp in _mstGenEmpStatusRepo.Get()
            //              join loc in _mstGenLocStatusRepo.Get(m => m.LocationCode == locationCode) on emp.StatusEmp equals loc.StatusEmp
            //              select new MstGenEmpStatu
            //              {
            //                  StatusEmp = loc.StatusEmp,
            //                  StatusIdentifier = emp.StatusIdentifier
            //              }).ToList();
            //return Mapper.Map<List<MstGenEmpStatusCompositeDTO>>(result);

            var queryFilter = PredicateHelper.True<MstPlantEmpJobsDataAcv>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var location = GetLocationInfo(input.LocationCode).Select(l => l.Value).ToList();
                queryFilter = queryFilter.And(m => location.Contains(m.LocationCode));
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {                
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            }
            if (input.ProcessSettingCode != "DAILY")
            {
                if (!string.IsNullOrEmpty(input.ProcessSettingCode))
                {
                    queryFilter = queryFilter.And(m => m.ProcessSettingsCode == input.ProcessSettingCode);
                }
            }
            else
            {
                queryFilter = queryFilter.And(m => m.Status == "4");
            }

            if (!string.IsNullOrEmpty(input.GroupCode))
            {                
                queryFilter = queryFilter.And(m => m.GroupCode == input.GroupCode);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantEmpJobsDataAcv>();

            var dbResult = _mstEmployeeJobsDataActiveRepo.Get(queryFilter, orderByFilter);

            //return (from emp in _mstEmployeeJobsDataActiveRepo.Get(queryFilter, orderByFilter)
            //                 join set in _mstGenProcessSettingRepo.Get() on emp.ProcessSettingsCode equals set.ProcessGroup
            //                 select new MstEmployeeJobsDataActiveCompositeDTO
            //                 {
            //                     EmployeeID = emp.EmployeeID,
            //                     EmployeeNumber = emp.EmployeeNumber,
            //                     EmployeeName = emp.EmployeeName,
            //                     JoinDate = emp.JoinDate,
            //                     Title_id = emp.Title_id,
            //                     ProcessIdentifier = set.ProcessIdentifier,
            //                     ProcessSettingsCode = emp.ProcessSettingsCode,
            //                     Status = emp.Status,
            //                     CCT = emp.CCT,
            //                     CCTDescription = emp.CCTDescription,
            //                     HCC = emp.HCC,
            //                     LocationCode = emp.LocationCode,
            //                     GroupCode = emp.GroupCode,
            //                     UnitCode = emp.UnitCode,
            //                     Loc_id = emp.Loc_id,
            //                     Remark = emp.Remark,
            //                     CreatedDate = emp.CreatedDate,
            //                     CreatedBy = emp.CreatedBy,
            //                     UpdatedDate = emp.UpdatedDate,
            //                     UpdatedBy = emp.UpdatedBy
            //                 }).ToList();

            return Mapper.Map<List<MstEmployeeJobsDataActiveCompositeDTO>>(dbResult);
        }

        public MstEmployeeJobsDataActiveCompositeDTO GetMstEmployeeJobsDataActives(string EmployeeID)
        {
           
            var queryFilter = PredicateHelper.True<MstPlantEmpJobsDataAcv>();            

            if (!string.IsNullOrEmpty(EmployeeID))
            {
                queryFilter = queryFilter.And(m => m.EmployeeID == EmployeeID);
            }            

            var dbResult = _mstEmployeeJobsDataActiveRepo.Get(queryFilter).SingleOrDefault();            

            return Mapper.Map<MstEmployeeJobsDataActiveCompositeDTO>(dbResult);
        }

        public List<MstEmployeeJobsDataActiveDTO> GetAllEmployeeJobsDataActives(GetAllEmployeeJobsDataActivesInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantEmpJobsDataAcv>();

            if (!string.IsNullOrEmpty(input.ProcessSettingsCode))
                queryFilter = queryFilter.And(m => m.ProcessSettingsCode == input.ProcessSettingsCode);

            var dbResult = _mstEmployeeJobsDataActiveRepo.Get(queryFilter);

            return Mapper.Map<List<MstEmployeeJobsDataActiveDTO>>(dbResult);
        }

        /// <summary>
        /// Gets the MST employee jobs data actives for Plant Worker Absenteeism Piece Rate.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MstEmployeeJobsDataActiveCompositeDTO> GetMstEmployeeJobsDataActivesForPieceRate(GetMstEmployeeJobsDataActivesInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantEmpJobsDataAcv>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var location = GetLocationInfo(input.LocationCode).Select(l => l.Value).ToList();
                queryFilter = queryFilter.And(m => location.Contains(m.LocationCode));
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {

                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            }

            if (!string.IsNullOrEmpty(input.ProcessSettingCode))
            {
                queryFilter = queryFilter.And(m => m.ProcessSettingsCode == input.ProcessSettingCode);
            }

            if (!string.IsNullOrEmpty(input.GroupCode))
            {
                queryFilter = queryFilter.And(m => m.GroupCode == input.GroupCode);
            }
            
            queryFilter = queryFilter.And(m => !String.IsNullOrEmpty(m.Status) && m.Status == ((int)Enums.StatusEmployeeJobsData.PieceRate).ToString());

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantEmpJobsDataAcv>();

            var dbResult = _mstEmployeeJobsDataActiveRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstEmployeeJobsDataActiveCompositeDTO>>(dbResult);
        }

        /// <summary>
        /// Gets the MST employee jobs data actives for Plant Worker Absenteeism Piece Rate.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<MstEmployeeJobsDataActiveCompositeDTO> GetMstEmployeeJobsDataActivesForDaily(GetMstEmployeeJobsDataActivesInput input)
        {
            var queryFilter = PredicateHelper.True<MstPlantEmpJobsDataAcv>();

            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var location = GetLocationInfo(input.LocationCode).Select(l => l.Value).ToList();
                queryFilter = queryFilter.And(m => location.Contains(m.LocationCode));
            }

            if (!string.IsNullOrEmpty(input.UnitCode))
            {
                queryFilter = queryFilter.And(m => m.UnitCode == input.UnitCode);
            }

            if (!string.IsNullOrEmpty(input.ProcessSettingCode))
            {
                queryFilter = queryFilter.And(m => m.ProcessSettingsCode == input.ProcessSettingCode);
            }

            queryFilter = queryFilter.And(m => !String.IsNullOrEmpty(m.Status) && m.Status == ((int)Enums.StatusEmployeeJobsData.Mandor).ToString());

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstPlantEmpJobsDataAcv>();

            var dbResult = _mstEmployeeJobsDataActiveRepo.Get(queryFilter, orderByFilter);

            return Mapper.Map<List<MstEmployeeJobsDataActiveCompositeDTO>>(dbResult);
        }
        #endregion
    }
}
