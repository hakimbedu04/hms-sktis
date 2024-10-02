using HMS.SKTIS.BusinessObjects.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.Contracts
{
    public interface IGeneralBLL
    {
        #region
        int ExeTransactionLog(TransactionLogInput input);

        void ExeTransactionLogAsync(TransactionLogInput input);

        IEnumerable<DateTime> EachDay(DateTime fromDateTime, DateTime toDateTime);
       

        #endregion
    }
}
