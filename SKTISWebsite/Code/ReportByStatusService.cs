using HMS.SKTIS.BusinessObjects.Inputs.Execution;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Linq.Expressions;
using SKTISWebsite.Models.ExeReportByStatus;
using HMS.SKTIS.AutoMapperExtensions;
using System.Globalization;
using AutoMapper;

namespace SKTISWebsite.Code
{
    public class ReportByStatusService : IReportByStatusService
    {
        private readonly IExeReportByStatusBLL _repotyByStatusBLL;

        public ReportByStatusService(IExeReportByStatusBLL repotyByStatusBLL) {
            _repotyByStatusBLL = repotyByStatusBLL;
        }

        #region FILTER DROPDOWN

        public SelectList GetListFilterUnitCode(string locationCode, bool responsibility = true) {
            IDictionary<string, string> dictionaryUnits = new Dictionary<string, string>();

            if (locationCode == Enums.LocationCode.PLNT.ToString() || locationCode == Enums.LocationCode.TPO.ToString() ||
               locationCode == Enums.LocationCode.SKT.ToString() || locationCode == Enums.LocationCode.REG1.ToString() ||
               locationCode == Enums.LocationCode.REG2.ToString() || locationCode == Enums.LocationCode.REG3.ToString() || locationCode == Enums.LocationCode.REG4.ToString()) {
                dictionaryUnits.Add(Enums.All.All.ToString(), Enums.All.All.ToString());
            } else {
                var units = _repotyByStatusBLL.GetAllUnits(locationCode, responsibility);
                foreach (var unit in units) { dictionaryUnits.Add(unit, unit); }
            }
            return new SelectList(dictionaryUnits, "Key", "Value");
        }

        public SelectList GetListFilterShift(string locationCode) {
            var listShift = _repotyByStatusBLL.GetShiftByLocationCode(locationCode).Distinct();

            IDictionary<int, int> shifts = new Dictionary<int, int>();

            //if (listShift.Count() > 1) { shifts.Add(Enums.All.All.ToString(), Enums.All.All.ToString()); }

            foreach (var data in listShift) {
                shifts.Add(data, data);
            }

            return new SelectList(shifts, "Key", "Value");
        }

        public IEnumerable<string> GetListFilterBrandGroup(GetExeReportByStatusInput inputFilter) {
            return _repotyByStatusBLL.GetActiveBrandGroupCode(inputFilter);
        }

        public IEnumerable<string> GetlistFilterBrandCode(GetExeReportByStatusInput inputFilter) {
            return _repotyByStatusBLL.GetBrandCode(inputFilter);
        }

        #endregion

        #region GET DATA
        public IEnumerable<GetReportByStatusCompositeViewModel> GetReportByStatus(GetExeReportByStatusInput input) {
            var listDataResult = _repotyByStatusBLL.GetReportByStatusFunc(input);

            var result = Mapper.Map<List<GetReportByStatusCompositeViewModel>>(listDataResult);
            return result;
        }
        #endregion
    }
}