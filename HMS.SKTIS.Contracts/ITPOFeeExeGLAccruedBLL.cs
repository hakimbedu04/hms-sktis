using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;

namespace HMS.SKTIS.Contracts
{
    public interface ITPOFeeExeGLAccruedBLL
    {
        #region View List
        IEnumerable<TPOFeeExeGLAccruedViewListDTO> GetTPOFeeExeGLAccruedViewList(GetTPOFeeExeGLAccruedInput criteria);
        List<GenerateP1TemplateGLDTO> GetP1TemplateGL(GetTPOFeeExeGLAccruedInput input);
        #endregion

        #region Detail
        //TPOFeeExeGLAccruedHdrDTO GetTPOFeeExeGLAccruedDetailHdr(GetTPOFeeExeGLAccruedInput criteria);
        TPOFeeExeGLAccruedDetailDTO GetTpoFeeExeGlAccruedDetailDaily(GetTPOFeeExeGLAccruedInput criteria);

        #endregion
    }
}
