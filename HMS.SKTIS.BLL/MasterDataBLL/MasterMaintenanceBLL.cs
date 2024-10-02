using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;

namespace HMS.SKTIS.BLL
{
    public partial class MasterDataBLL
    {
        #region Item
        /// <summary>
        /// Get Maintenance Item data
        /// </summary>
        /// <param name="input"></param>
        /// <returns> List of maintenance item</returns>
        public List<MstMntcItemCompositeDTO> GetMstMaintenanceItems(MstMntcItemInput input)
        {
            var queryFilter = PredicateHelper.True<MstMntcItem>();

            if (!string.IsNullOrEmpty(input.ItemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == input.ItemCode);

            if (!string.IsNullOrEmpty(input.ItemType))
                queryFilter = queryFilter.And(m => m.ItemType == input.ItemType);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstMntcItem>();

            var dbResult = _mstMaintenanceItemRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<MstMntcItemCompositeDTO>>(dbResult);
        }

        public MstMntcItemCompositeDTO GetMstMaintenanceItem(MstMntcItemInput input)
        {
            var queryFilter = PredicateHelper.True<MstMntcItem>();

            if (!string.IsNullOrEmpty(input.ItemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == input.ItemCode);

            if (!string.IsNullOrEmpty(input.ItemType))
                queryFilter = queryFilter.And(m => m.ItemType == input.ItemType);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstMntcItem>();

            var dbResult = _mstMaintenanceItemRepo.Get(queryFilter, orderByFilter).FirstOrDefault();
            return Mapper.Map<MstMntcItemCompositeDTO>(dbResult);
        }

        public List<MstMntcItemCompositeDTO> GetMstMaintenanceItemNotEqualItemCode(string itemCode, string itemType)
        {
            var sourceItemCode = _mstMntcConvertRepo.Get(x => x.ConversionType == false).Select(m => m.ItemCodeSource);

            var queryFilter = PredicateHelper.True<MstMntcItem>();

            if (!string.IsNullOrEmpty(itemCode))
                queryFilter = queryFilter.And(m => !m.ItemCode.Contains(itemCode));

            if (!string.IsNullOrEmpty(itemType))
                queryFilter = queryFilter.And(m => m.ItemType == itemType);

            var dbResult = _mstMaintenanceItemRepo.Get(queryFilter);

            //if (!string.IsNullOrEmpty(itemCode))
            //    dbResult = (from mstMntcItem in dbResult from code in sourceItemCode where code != mstMntcItem.ItemCode select mstMntcItem).Distinct().ToList();

            return Mapper.Map<List<MstMntcItemCompositeDTO>>(dbResult);
        }

        public List<MstMntcItemCompositeDTO> GetMstMaintenanceItemNotContainItemType(string itemType)
        {
            var sourceItemCode = _mstMntcConvertRepo.Get(x => x.ConversionType == true).Distinct();

            var queryFilter = PredicateHelper.True<MstMntcItem>();
            if (!string.IsNullOrEmpty(itemType))
                queryFilter = queryFilter.And(m => !m.ItemType.Contains(itemType));
            var dbResult = _mstMaintenanceItemRepo.Get(queryFilter);

            dbResult = from item in dbResult
                       where !(from o in sourceItemCode
                               select o.ItemCodeSource)
                           .Contains(item.ItemCode)
                       orderby item.ItemCode ascending
                       select item;

            //if (sourceItemCode.Any())
            //    dbResult = (from mstMntcItem in dbResult from itemCode in sourceItemCode where itemCode != mstMntcItem.ItemCode select mstMntcItem).Distinct().ToList();

            return Mapper.Map<List<MstMntcItemCompositeDTO>>(dbResult);
        }

        public List<MstMntcConvertCompositeDTO> GetEqupmentItemDestinationDetail(string sourceItemCode)
        {
            var db = _mstMntcConvertGetItemDestinationViewRepo.Get(c => c.ItemCode == sourceItemCode, orderBy => orderBy.OrderBy(f => f.ItemCodeDest));
            return Mapper.Map<List<MstMntcConvertCompositeDTO>>(db);

        }

        /// <summary>
        /// Get all active Maintenance item
        /// </summary>
        /// <returns></returns>
        public List<MstMntcItemCompositeDTO> GetAllMaintenanceItems()
        {
            var dbResult = _mstMaintenanceItemRepo.Get(filter => filter.StatusActive == true,
                                                       orderBy => orderBy.OrderBy(f => f.ItemCode));
            return Mapper.Map<List<MstMntcItemCompositeDTO>>(dbResult);
        }

        /// <summary>
        /// Insert new master maintenance item
        /// </summary>
        /// <param name="maintenanceItemDTO"></param>
        public MstMntcItemCompositeDTO InsertMaintenanceItem(MstMntcItemCompositeDTO maintenanceItemDTO)
        {
            var dbMstMaintenanceItems = Mapper.Map<MstMntcItem>(maintenanceItemDTO);

            dbMstMaintenanceItems.CreatedDate = DateTime.Now;
            dbMstMaintenanceItems.UpdatedDate = DateTime.Now;

            _mstMaintenanceItemRepo.Insert(dbMstMaintenanceItems);
            _uow.SaveChanges();

            Mapper.Map(dbMstMaintenanceItems, maintenanceItemDTO);
            return maintenanceItemDTO;
        }

        /// <summary>
        /// Update existing master maintenance item
        /// </summary>
        /// <param name="maintenanceItemDTO"></param>
        public MstMntcItemCompositeDTO UpdateMaintenanceItem(MstMntcItemCompositeDTO maintenanceItemDTO)
        {
            var dbMstMaintenanceItem = _mstMaintenanceItemRepo.GetByID(maintenanceItemDTO.ItemCode);
            if (dbMstMaintenanceItem == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            maintenanceItemDTO.CreatedBy = dbMstMaintenanceItem.CreatedBy;
            maintenanceItemDTO.CreatedDate = dbMstMaintenanceItem.CreatedDate;

            //set update time
            maintenanceItemDTO.UpdatedDate = DateTime.Now;
            Mapper.Map(maintenanceItemDTO, dbMstMaintenanceItem);
            _mstMaintenanceItemRepo.Update(dbMstMaintenanceItem);
            _uow.SaveChanges();

            return Mapper.Map<MstMntcItemCompositeDTO>(dbMstMaintenanceItem);
        }

        #endregion

        #region Item Location
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<MstMntcItemLocationDTO> GetMstItemLocations(MstMntcItemLocationInput input)
        {
            var queryFilter = PredicateHelper.True<MstMntcItemLocation>();

            if (!string.IsNullOrEmpty(input.ItemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == input.ItemCode);

            //check is skt or not
            var sktparent = GetMstLocationById(input.LocationCode);
            if (sktparent != null && !string.IsNullOrEmpty(sktparent.ParentLocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            /**
             * Replace query using LINQ because property null reference error
             * http://tp.voxteneo.co.id/entity/3159
             */
            //var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            //var orderByFilter = sortCriteria.GetOrderByFunc<MstMntcItemLocation>();

            var dbResult = _mstItemLocationRepo.Get(queryFilter);


            return Mapper.Map<List<MstMntcItemLocationDTO>>(dbResult);
        }
        /// <summary>
        /// Insert new master item location
        /// </summary>
        /// <param name="itemLocation"></param>
        /// <returns>Process object</returns>
        public MstMntcItemLocationDTO InsertItemLocation(MstMntcItemLocationDTO itemLocation)
        {
            ValidateInsertItemLocation(itemLocation);
            var dbItemLocation = Mapper.Map<MstMntcItemLocation>(itemLocation);

            dbItemLocation.CreatedDate = DateTime.Now;
            dbItemLocation.UpdatedDate = DateTime.Now;

            _mstItemLocationRepo.Insert(dbItemLocation);
            _uow.SaveChanges();

            Mapper.Map(dbItemLocation, itemLocation);
            return itemLocation;
        }

        /// <summary>
        /// Validates the insert item location.
        /// </summary>
        /// <param name="itemLocation">The item location.</param>
        /// <exception cref="BLLException"></exception>
        private void ValidateInsertItemLocation(MstMntcItemLocationDTO itemLocation)
        {
            var dbItemLocation = _mstItemLocationRepo.GetByID(itemLocation.ItemCode, itemLocation.LocationCode);
            if (dbItemLocation != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        /// <summary>
        /// Update existing master item location
        /// </summary>
        /// <param name="itemLocationDTO"></param>
        public MstMntcItemLocationDTO UpdateItemLocation(MstMntcItemLocationDTO itemLocationDTO)
        {
            var dbItemLocation = _mstItemLocationRepo.GetByID(itemLocationDTO.ItemCode, itemLocationDTO.LocationCode);
            if (dbItemLocation == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //keep original CreatedBy and CreatedDate
            itemLocationDTO.CreatedBy = dbItemLocation.CreatedBy;
            itemLocationDTO.CreatedDate = dbItemLocation.CreatedDate;

            //set update time
            itemLocationDTO.UpdatedDate = DateTime.Now;

            Mapper.Map(itemLocationDTO, dbItemLocation);
            _mstItemLocationRepo.Update(dbItemLocation);
            _uow.SaveChanges();

            return Mapper.Map<MstMntcItemLocationDTO>(dbItemLocation);
        }

        /// <summary>
        /// Gets the item locations by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public List<MstMntcItemLocationDTO> GetItemLocationsByLocationCode(string locationCode)
        {
            var dbResult = _mstItemLocationRepo.Get(m => m.LocationCode == locationCode);
            return Mapper.Map<List<MstMntcItemLocationDTO>>(dbResult);
        }

        public List<MstMntcItemLocationDTO> GetItemLocationsByLocationCodeAndType(string locationCode, Enums.ItemType itemType)
        {
            var dbResult = _mstItemLocationRepo.Get(m => m.LocationCode == locationCode && m.ItemType.Contains(itemType.ToString()));
            return Mapper.Map<List<MstMntcItemLocationDTO>>(dbResult);
        }
        #endregion

        #region Master Maintenance Convert
        public List<MstMntcConvertDTO> GetMstMntvConverts(GetMstMntcConvertInput input)
        {
            var queryFilter = PredicateHelper.True<MstMntcConvert>();
            queryFilter = queryFilter.And(m => m.ConversionType == input.ConversionType);

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstMntcConvert>();

            var dbResult = _mstMntcConvertRepo.Get(queryFilter, orderByFilter);
            if (input.ConversionType)
            {
                dbResult = dbResult.OrderBy(m => m.ItemCodeSource);
                var mstConverts = new List<MstMntcConvert>();
                var mstConvert = new MstMntcConvert();
                foreach (var item in dbResult.Where(item => item.ItemCodeSource != mstConvert.ItemCodeSource))
                {
                    mstConvert = item;
                    mstConverts.Add(mstConvert);
                }
                return Mapper.Map<List<MstMntcConvertDTO>>(mstConverts);
            }
            return Mapper.Map<List<MstMntcConvertDTO>>(dbResult);
        }

        public List<MstMntcConvertDTO> GetMstMntvConvertsForExcel(bool conversionType)
        {
            var queryFilter = PredicateHelper.True<MstMntcConvert>();
            queryFilter = queryFilter.And(m => m.ConversionType == conversionType);

            var dbResult = _mstMntcConvertRepo.Get(queryFilter);
            return Mapper.Map<List<MstMntcConvertDTO>>(dbResult);
        }

        public MstMntcConvertDTO InsertMstMntcConvert(MstMntcConvertDTO mntcConvertDto)
        {
            ValidationInsertMstMntcConvert(mntcConvertDto);

            var db = Mapper.Map<MstMntcConvert>(mntcConvertDto);

            db.CreatedDate = DateTime.Now;
            db.UpdatedDate = DateTime.Now;

            _mstMntcConvertRepo.Insert(db);
            _uow.SaveChanges();
            return mntcConvertDto;
        }

        private void ValidationInsertMstMntcConvert(MstMntcConvertDTO mntcConvertDto)
        {
            var db = _mstMntcConvertRepo.GetByID(mntcConvertDto.ItemCodeSource, mntcConvertDto.ItemCodeDestination);
            if (db != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }

        public MstMntcConvertDTO UpdateMstMntcConvert(MstMntcConvertDTO mntcConvertDto)
        {
            var db = _mstMntcConvertRepo.GetByID(mntcConvertDto.ItemCodeSource, mntcConvertDto.ItemCodeDestination);
            if (db == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            Mapper.Map(mntcConvertDto, db);

            //set update time
            db.UpdatedDate = DateTime.Now;

            _mstMntcConvertRepo.Update(db);
            _uow.SaveChanges();

            return Mapper.Map<MstMntcConvertDTO>(db);
        }

        public MstMntcConvertDTO InsertUpdateMstmntcConvertEquipment(MstMntcConvertDTO mntcConvertDto)
        {
            var db = _mstMntcConvertRepo.GetByID(mntcConvertDto.ItemCodeSource, mntcConvertDto.ItemCodeDestination);
            if (db == null)
            {
                db = Mapper.Map<MstMntcConvert>(mntcConvertDto);

                db.CreatedDate = DateTime.Now;
                db.UpdatedDate = DateTime.Now;
                _mstMntcConvertRepo.Insert(db);
                _uow.SaveChanges();
            }
            else
            {
                db.UpdatedDate = DateTime.Now;
                Mapper.Map(mntcConvertDto, db);
                _mstMntcConvertRepo.Update(db);
                _uow.SaveChanges();
            }
            return Mapper.Map<MstMntcConvertDTO>(db);
        }

        public List<MstMntcConvertDTO> GetMntcConvertByLocationCode(string locationCode)
        {
            var dbResult = (from mstMntcItemLoc in _mstMntcItemLocationRepository.Get(m => m.LocationCode == locationCode)
                            join mntcConvert in _mstMntcConvertRepo.Get(m => m.StatusActive == true) on mstMntcItemLoc.ItemCode equals mntcConvert.ItemCodeSource
                            select new MstMntcConvert
                            {
                                ItemCodeSource = mntcConvert.ItemCodeSource,
                                ItemCodeDestination = mntcConvert.ItemCodeDestination,
                                ConversionType = mntcConvert.ConversionType,
                                QtyConvert = mntcConvert.QtyConvert
                            }).ToList();
            return Mapper.Map<List<MstMntcConvertDTO>>(dbResult);
        }

        public MstMntcConvertDTO SaveMstMntcConvertEquipmentDetails(MstMntcConvertDTO mntcConvertDto)
        {
            var db = _mstMntcConvertRepo.GetByID(mntcConvertDto.ItemCodeSource, mntcConvertDto.ItemCodeDestination);
            if (db == null)
            {
                db = Mapper.Map<MstMntcConvert>(mntcConvertDto);

                db.CreatedDate = DateTime.Now;
                db.UpdatedDate = DateTime.Now;
                _mstMntcConvertRepo.Insert(db);
            }
            else
            {
                db.UpdatedDate = DateTime.Now;
                Mapper.Map(mntcConvertDto, db);
                _mstMntcConvertRepo.Update(db);
            }
            _uow.SaveChanges();
            return Mapper.Map<MstMntcConvertDTO>(db);
        }

        public MstMntcConvertDTO SaveMstMntcConvertEquipment(MstMntcConvertDTO mntcConvertDto)
        {
            var db = _mstMntcConvertRepo.Get(m => m.ItemCodeSource == mntcConvertDto.ItemCodeSource && m.ConversionType == true);
            foreach (var mstMntcConvert in db)
            {
                mstMntcConvert.Remark = mntcConvertDto.Remark;
                mstMntcConvert.StatusActive = mntcConvertDto.StatusActive;
                mstMntcConvert.UpdatedDate = DateTime.Now;
                mstMntcConvert.UpdatedBy = mntcConvertDto.UpdatedBy;
                _mstMntcConvertRepo.Update(mstMntcConvert);
            }
            _uow.SaveChanges();
            return mntcConvertDto;
        }

        public MstMntcConvertDTO GetMasterMaintenanceBySourceAndDestination(string itemCodeSource, string itemCodeDestination)
        {
            var db = _mstMntcConvertRepo.GetByID(itemCodeSource, itemCodeDestination);
            return Mapper.Map<MstMntcConvertDTO>(db);
        }

        public List<SparepartDTO> GetSparepartsByItemCode(GetEquipmentRepairItemUsage input)
        {
            var date = input.TransactionDate.Date;
            var checkExisting = _mntcRepairItemUsageViewRepo.Get()
                .Where(r => r.TransactionDate == date
                    && r.LocationCode == input.LocationCode 
                    && r.ItemCodeSource == input.ItemSourceCode);
            if(checkExisting.Count() > 0)
            {                
                return Mapper.Map<List<SparepartDTO>>(checkExisting);
            }
            else
            {
                var dbResults =
                _mstMntcConvertRepo.Get(
                    m => m.ItemCodeSource == input.ItemSourceCode && m.ConversionType == true && m.StatusActive == true && m.QtyConvert > 0);
                return Mapper.Map<List<SparepartDTO>>(dbResults);
            }            
        }

        public List<SparepartDTO> GetTpoSparepartsByItemCode(GetEquipmentRepairItemUsage input)
        {
            //mstmntcconvert
            var itemConvert = _mstMntcConvertRepo.Get()
                .Where(x => x.ItemCodeSource == input.ItemSourceCode
                    && x.QtyConvert > 0
                    && (x.StatusActive ?? false)
                ).ToList();

            var result = new List<SparepartDTO>();
            if (!itemConvert.Any()) return result;
            foreach (var i in itemConvert)
            {
                var detail = _mstMaintenanceItemRepo.GetByID(i.ItemCodeDestination);
                var usage = _mntcRepairItemUsageRepo.GetByID(input.TransactionDate,
                    Enums.UnitCodeDefault.MTNC.ToString(), input.LocationCode, i.ItemCodeSource,
                    i.ItemCodeDestination);
                int? qty = 0;
                if (usage != null) qty = usage.QtyUsage;

                var item = new SparepartDTO
                {
                    ItemCode = i.ItemCodeDestination,
                    ItemDescription = detail.ItemDescription,
                    UOM = detail.UOM,
                    QtyConvert = i.QtyConvert.ToString(),
                    Quantity = qty.ToString()
                };

                result.Add(item);
            }
            return result;
        }
        
        #endregion
    }
}