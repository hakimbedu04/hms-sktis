using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.Contracts
{
    public interface IExeReportProdStockProcessBLL
    {
        IEnumerable<string> GetUnitCodeList(string locationCode);
        IEnumerable<ExeReportProdStockPerBrandGroupCodeDTO> GetExeReportProductionStock(GetExeProdStockInput input);
    }
}
