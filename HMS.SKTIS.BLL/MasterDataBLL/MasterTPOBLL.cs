using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;

namespace HMS.SKTIS.BLL
{
    public partial class MasterDataBLL
    {
        #region Master TPO Info
        public List<MstTPOInfoDTO> GetMasterTPOInfos(GetMasterTPOInfoInput input)
        {
            
            var queryFilter = PredicateHelper.True<MstTPOInfo>();
            if (!string.IsNullOrEmpty(input.LocationCode))
                queryFilter = queryFilter.And(m => m.LocationCode == input.LocationCode);
            queryFilter = queryFilter.And(m => m.MstGenLocation.StatusActive == true);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOInfo>();

            var dbResult = _mstTpointoRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<MstTPOInfoDTO>>(dbResult);
        }

        public List<MstTPOInfoDTO> GetMstTPOInfos(GetMasterTPOInfoInput input)
        {
            var a = GetLastChildLocation(input.LocationCode).Select(x => x.LocationCode);
            var queryFilter = PredicateHelper.True<MstTPOInfo>();
            if (!string.IsNullOrEmpty(input.LocationCode))
            {
                var locations = _sqlSPRepo.GetLocations(input.LocationCode, 1);
                var locationCodes = locations.Select(x => x.LocationCode);
                queryFilter = queryFilter.And(m => a.Contains(m.LocationCode));
            }
            queryFilter = queryFilter.And(m => m.MstGenLocation.StatusActive == true);
            var sortCriteria = new Tuple<IEnumerable<string>, string>(new[] { input.SortExpression }, input.SortOrder);
            var orderByFilter = sortCriteria.GetOrderByFunc<MstTPOInfo>();

            var dbResult = _mstTpointoRepo.Get(queryFilter, orderByFilter);
            return Mapper.Map<List<MstTPOInfoDTO>>(dbResult);
        }

        public MstTPOInfoDTO InsertMasterTPOInfo(MstTPOInfoDTO mstTPOInfoDTO)
        {
            ValidateInsertMasterTPOInfo(mstTPOInfoDTO);

            var dbTpo = Mapper.Map<MstTPOInfo>(mstTPOInfoDTO);
            dbTpo.CreatedDate = DateTime.Now;
            dbTpo.UpdatedDate = DateTime.Now;

            if (!dbTpo.Established.HasValue)
                dbTpo.Established = DateTime.Now;

            try
            {
                _mstTpointoRepo.Insert(dbTpo);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.HResult);
            }
            Mapper.Map(dbTpo, mstTPOInfoDTO);
            return mstTPOInfoDTO;
        }

        private void ValidateInsertMasterTPOInfo(MstTPOInfoDTO mstTPOInfoDTO)
        {
            var dbResult = _mstTpointoRepo.GetByID(mstTPOInfoDTO.LocationCode);
            if (dbResult != null)
                throw new BLLException(ExceptionCodes.BLLExceptions.KeyExist);
        }


        public MstTPOInfoDTO UpdateMasterTPOInfo(MstTPOInfoDTO tpodto)
        {
            var dbtpo = _mstTpointoRepo.GetByID(tpodto.LocationCode);
            if (dbtpo == null)
                throw new BLLException(ExceptionCodes.BLLExceptions.DataNotFound);

            //set update time
            tpodto.UpdatedDate = DateTime.Now;
            Mapper.Map(tpodto, dbtpo);
            try
            {
                _mstTpointoRepo.Update(dbtpo);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BLLException(ex.HResult);
            }

            return tpodto;
        }

        public MstTPOInfoDTO GetMstTpoInfo(string id)
        {
            var dbResult = _mstTpointoRepo.GetByID(id);

            return Mapper.Map<MstTPOInfoDTO>(dbResult);
        }
        #endregion
    }
}
