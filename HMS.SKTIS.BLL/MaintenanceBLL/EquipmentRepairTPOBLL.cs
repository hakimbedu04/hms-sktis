using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Maintenance;
using HMS.SKTIS.BusinessObjects.Inputs.Maintenance;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;

namespace HMS.SKTIS.BLL
{
    public partial class MaintenanceBLL
    {
        /// <summary>
        /// Gets the TPO equipment repairs.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public List<EquipmentRepairTPODTO> GetTPOEquipmentRepairs(GetPlantEquipmentRepairsTPOInput input)
        {
            var queryFilter = PredicateHelper.True<MntcEquipmentRepair>();
            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);

            if (input.TransactionDate.HasValue)
            {
                var transactionDate = input.TransactionDate.Value.Date;
                queryFilter = queryFilter.And(m => m.TransactionDate == transactionDate);
            }

            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MntcEquipmentRepair>();

            var dbResults = _equipmentRepairRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<EquipmentRepairTPODTO>>(dbResults);
        }

        /// <summary>
        /// Saves the equipment repair.
        /// </summary>
        /// <param name="equipmentRepair">The equipment repair.</param>
        /// <returns></returns>
        public EquipmentRepairTPODTO SaveEquipmentRepairTPO(EquipmentRepairTPODTO equipmentRepair)
        {
            //Check data in MstPlantUnitCode table
            var unitCodes = _masterDataBll.CheckMstPlantUnit(equipmentRepair.LocationCode, equipmentRepair.UnitCode);
            if (unitCodes)
                throw new BLLException(ExceptionCodes.BLLExceptions.UnitForSelectedLocationNotExist);

            // Save MntcEquipmentRepair
            var equipmentDto = InsertUpdateEquipmentRepair(equipmentRepair);

            // Save MntcRepairItemUsage
            if (!string.IsNullOrEmpty(equipmentRepair.ItemCodeDestination))
            {
                var mntcRepairItemUsageDto = Mapper.Map<MntcRepairItemUsageDTO>(equipmentRepair);
                if (mntcRepairItemUsageDto.QtyUsage.HasValue && mntcRepairItemUsageDto.LastQtyUsage.HasValue)
                {
                    var valid = false;
                    if (mntcRepairItemUsageDto.QtyUsage.Value > 0 && mntcRepairItemUsageDto.LastQtyUsage.Value == 0)
                    {
                        valid = true;
                    }
                    else if (mntcRepairItemUsageDto.QtyUsage.Value == 0 && mntcRepairItemUsageDto.LastQtyUsage.Value > 0)
                    {
                        valid = true;
                    }
                    else if (mntcRepairItemUsageDto.QtyUsage.Value > 0 && mntcRepairItemUsageDto.LastQtyUsage.Value > 0)
                    {
                        valid = true;
                    }


                    if (valid)
                    {
                        //set default repair item usage unit code
                        mntcRepairItemUsageDto.UnitCode = Enums.UnitCodeDefault.MTNC.ToString();
                        mntcRepairItemUsageDto.UpdatedDate = equipmentDto.UpdatedDate;
                        mntcRepairItemUsageDto.UpdatedBy = equipmentDto.UpdatedBy;

                        mntcRepairItemUsageDto.CreatedDate = equipmentDto.CreatedDate;
                        mntcRepairItemUsageDto.CreatedBy = equipmentDto.CreatedBy;

                        SaveMaintenanceRepairItemUsage(mntcRepairItemUsageDto);
                    }
                }
            }

            _uow.SaveChanges();

            return equipmentDto;
        }

        public EquipmentRepairTPODTO InsertUpdateEquipmentRepair(EquipmentRepairTPODTO equipmentRepair)
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
            return Mapper.Map<EquipmentRepairTPODTO>(dbEquipmentRepair);
        }

        public void SaveMaintenanceRepairItemUsage(MntcRepairItemUsageDTO itemUsageDto)
        {
            var db = _maintenanceRepairItemUsageRepository.GetByID(itemUsageDto.TransactionDate, itemUsageDto.UnitCode, itemUsageDto.LocationCode, itemUsageDto.ItemCodeSource, itemUsageDto.ItemCodeDestination);
            if (db == null)
            {
                db = Mapper.Map<MntcRepairItemUsage>(itemUsageDto);
                db.CreatedDate = DateTime.Now;
                db.UpdatedDate = DateTime.Now;
                _maintenanceRepairItemUsageRepository.Insert(db);
            }
            else
            {
                Mapper.Map(itemUsageDto, db);
                db.UpdatedDate = DateTime.Now;
                _maintenanceRepairItemUsageRepository.Update(db);
            }
        }

    }
}
