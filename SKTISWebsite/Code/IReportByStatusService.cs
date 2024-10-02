using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using SKTISWebsite.Models.ExeReportByStatus;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SKTISWebsite.Code
{
    public interface IReportByStatusService
    {
        #region FILTER DROPDOWN

        SelectList GetListFilterUnitCode(string locationCode, bool responsibility = true);
        SelectList GetListFilterShift(string locationCode);
        IEnumerable<string> GetListFilterBrandGroup(GetExeReportByStatusInput inputFilter);
        IEnumerable<string> GetlistFilterBrandCode(GetExeReportByStatusInput inputFilter);

        #endregion

        #region GET DATA
        IEnumerable<GetReportByStatusCompositeViewModel> GetReportByStatus(GetExeReportByStatusInput input);
        #endregion
    }
}
