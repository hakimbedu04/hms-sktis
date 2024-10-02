using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.BusinessObjects.DTOs.Utils;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Utils;

namespace HMS.SKTIS.BLL
{
    public class EmailApprovalsBLL : IEmailApprovalsBLL
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TPOFeeHdr> _tpofee;
        private IGenericRepository<MstGenLocation> _genloc;
        private IGenericRepository<UtilUsersResponsibility> _utiluserres;
        private IGenericRepository<UtilResponsibility> _utilres;

        public EmailApprovalsBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _tpofee = _uow.GetGenericRepository<TPOFeeHdr>();
            _genloc = _uow.GetGenericRepository<MstGenLocation>();
            _utiluserres = _uow.GetGenericRepository<UtilUsersResponsibility>();
            _utilres = _uow.GetGenericRepository<UtilResponsibility>();
        }
        public List<TPOFeeHdrDTO> GetUtilTransactionLogDtos(string parentlocationcode,int kpsyear, int kpsweek)
        {
            var queryFilter = PredicateHelper.True<TPOFeeHdr>();
            queryFilter = queryFilter.And(x => x.KPSYear == kpsyear);
            queryFilter = queryFilter.And(x => x.KPSWeek == kpsweek);
            queryFilter = queryFilter.And(x => x.TaxtNoProd != null);
            var listtpo = _tpofee.Get(queryFilter).ToList();

            var queryFilter2 = PredicateHelper.True<MstGenLocation>();
            if (!string.IsNullOrEmpty(parentlocationcode))
            {
                queryFilter2 = queryFilter2.And(x => x.ParentLocationCode == parentlocationcode);    
            }
            
            var listlocation = _genloc.Get(queryFilter2).ToList();

            var dbResult = (from data1 in listtpo
                join data2 in listlocation on data1.LocationCode equals data2.LocationCode
                select new TPOFeeHdrDTO()
                {
                    TPOFeeCode = data1.TPOFeeCode
                }).ToList();


            return dbResult;
        }
    }
}
