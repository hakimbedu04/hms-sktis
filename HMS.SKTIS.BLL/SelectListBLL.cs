using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;

namespace HMS.SKTIS.BLL
{
    //todo: remove this class
    public class SelectListBLL : ISelectListBLL
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MstGenBrandGroup> _repositoryBrandGroup;
        private IGenericRepository<MstGenLocation> _repositoryLocation;

        public SelectListBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _repositoryBrandGroup = _uow.GetGenericRepository<MstGenBrandGroup>();
            _repositoryLocation = _uow.GetGenericRepository<MstGenLocation>();
        }

        /// <summary>
        /// Get Brand Group Code from MstBrandGroup
        /// </summary>
        /// <returns>List BrandDTO</returns>
        public List<BrandDTO> GetBrandGroupCodes()
        {
            return _repositoryBrandGroup.Get().Distinct()
                .Select(p => new BrandDTO
                {
                    BrandGroupCode = p.BrandGroupCode
                }).ToList();
        }

        public List<MstGenLocationDTO> GetLocations()
        {
            return _repositoryLocation.Get().Where(m => m.StatusActive == true)
                .Select(p => new MstGenLocationDTO()
                {
                    LocationCode = p.LocationCode,
                    LocationName = p.LocationName
                }).ToList();
        }
    }
}
