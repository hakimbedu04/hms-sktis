using System.Collections;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BLL
{
    public class GeneralBLL : IGeneralBLL
    {
        private IUnitOfWork _uow;
        private ISqlSPRepository _sqlSPRepo;

        public GeneralBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _sqlSPRepo = _uow.GetSPRepository();
        }

        public int ExeTransactionLog(TransactionLogInput input)
        {
            return _sqlSPRepo.ExeTransactionLog(input.separator, input.page, input.year, input.week, input.code_1, input.code_2, input.code_3, input.code_4, input.code_5, input.code_6, input.code_7, input.code_8, input.code_9, input.TransactionDate, input.ActionButton, input.ActionTime, input.UserName, input.TransactionCode, input.Message, input.IDRole);
        }

        public void ExeTransactionLogAsync(TransactionLogInput input)
        {
            _sqlSPRepo.ExeTransactionLogTask(input.separator, input.page, input.year, input.week, input.code_1, input.code_2, input.code_3, input.code_4, input.code_5, input.code_6, input.code_7, input.code_8, input.code_9, input.TransactionDate, input.ActionButton, input.ActionTime, input.UserName, input.TransactionCode, input.Message, input.IDRole);
        }

        public IEnumerable<DateTime> EachDay(DateTime fromDateTime, DateTime toDateTime)
        {
            for (var day = fromDateTime.Date; day.Date <= toDateTime.Date; day = day.AddDays(1))
                yield return day;
        }

    }
}
