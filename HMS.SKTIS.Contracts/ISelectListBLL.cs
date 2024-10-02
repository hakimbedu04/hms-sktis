using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace HMS.SKTIS.Contracts
{
    public interface ISelectListBLL
    {
        List<BrandDTO> GetBrandGroupCodes();
        List<MstGenLocationDTO> GetLocations();
    }
}
