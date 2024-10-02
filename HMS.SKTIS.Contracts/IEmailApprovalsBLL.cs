using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;

namespace HMS.SKTIS.Contracts
{
    public interface IEmailApprovalsBLL
    {
        List<TPOFeeHdrDTO> GetUtilTransactionLogDtos(string parentlocationcode, int kpsyear, int kpsweek);
    }
}
