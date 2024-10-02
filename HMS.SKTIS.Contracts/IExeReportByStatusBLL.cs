using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using System.Collections.Generic;

namespace HMS.SKTIS.Contracts
{
    public interface IExeReportByStatusBLL
    {
        #region FILTER DROPDOWN
        IEnumerable<string> GetAllUnits(string locationCode, bool responsibilities = true);
        IEnumerable<int> GetShiftByLocationCode(string parentLocationCode);
        IEnumerable<string> GetActiveBrandGroupCode(GetExeReportByStatusInput inputFilter);
        IEnumerable<string> GetBrandCode(GetExeReportByStatusInput inputFilter);
        #endregion

        #region GET DATA
        IEnumerable<GetReportByStatusCompositeDTO> GetReportByStatusFunc(GetExeReportByStatusInput input);
        #endregion
    }
}
