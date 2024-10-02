using System;
using System.Net.Configuration;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;

namespace HMS.SKTIS.BLL
{
    public class UserBLL : IUserBLL
    {

        private IGenericRepository<MstADTemp> _repositoryLogin;
        private IUnitOfWork _uow;
        public UserBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _repositoryLogin = _uow.GetGenericRepository<MstADTemp>();
        }

        public MstADTemp GetLogin(string username)
        {
            return _repositoryLogin.Get(c => c.UserAD == username).FirstOrDefault();
        }

        public List<MstADTempDto> GetAllLoginActive()
        {
            var result = _repositoryLogin.Get(c=>c.StatusActive.HasValue && c.StatusActive.Value).ToList();

            return AutoMapper.Mapper.Map<List<MstADTempDto>>(result);
        }

        public List<MstADTempDto> GetUsersActive()
        {
            var users = _repositoryLogin.Get(s => s.StatusActive == true);
            return Mapper.Map<List<MstADTempDto>>(users);
		}
    }
}
