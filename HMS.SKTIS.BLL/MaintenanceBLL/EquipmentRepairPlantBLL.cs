using System;
using System.Collections.Generic;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace HMS.SKTIS.BLL
{
    public partial class MaintenanceBLL
    {
        /// <summary>
        /// Gets the plant equipment repairs.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<EquipmentRepairDTO> GetPlantEquipmentRepairs(GetPlantEquipmentRepairsInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentRepair>();
            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (!string.IsNullOrEmpty(input.ItemCode))
                queryFilter = queryFilter.And(m => m.ItemCode == input.ItemCode);

            if (input.TransactionDate.HasValue)
            {
                var transactionDate = input.TransactionDate.Value.Date;
                queryFilter = queryFilter.And(m => m.TransactionDate == transactionDate);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentRepair>();

            var dbResults = _equipmentRepairRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<EquipmentRepairDTO>>(dbResults);
        }

        /// <summary>
        /// Saves the equipment repair.
        /// </summary>
        /// <param name="equipmentRepair">The equipment repair.</param>
        /// <returns></returns>
        public EquipmentRepairDTO SaveEquipmentRepair(EquipmentRepairDTO equipmentRepair)
        {
            var dbEquipmentRepair = _equipmentRepairRepo.GetByID(equipmentRepair.TransactionDate, equipmentRepair.UnitCode, equipmentRepair.LocationCode, equipmentRepair.ItemCode);

            if (dbEquipmentRepair == null)
            {
                //Insert
                dbEquipmentRepair = Mapper.Map<MntcEquipmentRepair>(equipmentRepair);

                dbEquipmentRepair.CreatedDate = DateTime.Now;
                dbEquipmentRepair.UpdatedDate = DateTime.Now;

                _equipmentRepairRepo.Insert(dbEquipmentRepair);
            }
            else
            {
                //Update
                Mapper.Map(equipmentRepair, dbEquipmentRepair);
                dbEquipmentRepair.UpdatedDate = DateTime.Now;
                _equipmentRepairRepo.Update(dbEquipmentRepair);
            }

            _uow.SaveChanges();

            return Mapper.Map<EquipmentRepairDTO>(dbEquipmentRepair);
        }

        public MntcRepairItemUsageDTO SaveRepairItemUsage(MntcRepairItemUsageDTO repairItemUsage)
        {

            var dbRepairItemUsage = _equipmentRepairItemUsageRepo.GetByID(repairItemUsage.TransactionDate, repairItemUsage.UnitCode, repairItemUsage.LocationCode, repairItemUsage.ItemCodeSource, repairItemUsage.ItemCodeDestination);
            if (dbRepairItemUsage == null)
            {
                // Insert
                dbRepairItemUsage = Mapper.Map<MntcRepairItemUsage>(repairItemUsage);

                dbRepairItemUsage.CreatedDate = DateTime.Now;
                dbRepairItemUsage.UpdatedDate = DateTime.Now;

                _equipmentRepairItemUsageRepo.Insert(dbRepairItemUsage);
            }
            else
            {
                // Update
                Mapper.Map(repairItemUsage, dbRepairItemUsage);
                dbRepairItemUsage.UpdatedDate = DateTime.Now;
                _equipmentRepairItemUsageRepo.Update(dbRepairItemUsage);
            }
            
            _uow.SaveChanges();
            return Mapper.Map<MntcRepairItemUsageDTO>(dbRepairItemUsage);


            //var dbEquipmentRepair = _equipmentRepairRepo.GetByID(equipmentRepair.TransactionDate, equipmentRepair.UnitCode, equipmentRepair.LocationCode, equipmentRepair.ItemCode);

            //if (dbEquipmentRepair == null)
            //{
            //    //Insert
            //    dbEquipmentRepair = Mapper.Map<MntcEquipmentRepair>(equipmentRepair);

            //    dbEquipmentRepair.CreatedDate = DateTime.Now;
            //    dbEquipmentRepair.UpdatedDate = DateTime.Now;

            //    _equipmentRepairRepo.Insert(dbEquipmentRepair);
            //}
            //else
            //{
            //    //Update
            //    Mapper.Map(equipmentRepair, dbEquipmentRepair);
            //    dbEquipmentRepair.UpdatedDate = DateTime.Now;
            //    _equipmentRepairRepo.Update(dbEquipmentRepair);
            //}

            //_uow.SaveChanges();

            //return Mapper.Map<EquipmentRepairDTO>(dbEquipmentRepair);

        }
    }
}
