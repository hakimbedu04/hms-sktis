using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Core;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;


namespace HMS.SKTIS.Contracts
{
    public interface IUserBLL
    {
        MstADTemp GetLogin(string username);
        List<MstADTempDto> GetAllLoginActive();
        List<MstADTempDto> GetUsersActive();
    }
}
