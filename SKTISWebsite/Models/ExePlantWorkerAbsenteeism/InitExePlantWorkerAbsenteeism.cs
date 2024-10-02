using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs;
using SKTISWebsite.Models.LookupList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SKTISWebsite.Models.ExePlantWorkerAbsenteeism
{
    public class InitExePlantWorkerAbsenteeism
    {
        public string TodayDate { get; set; }
        public string ClosingPayrollDate { get; set; }
        public int? DefaultWeek { get; set; }
        public int DefaultYear { get; set; }
        public SelectList LocationCodeSelectList { get; set; }
        public IEnumerable<SelectListItem> YearSelectList { get; set; }
        public List<MstEmployeeJobsDataActiveDTO> AllActiveEmployees { get; set; }
        public List<MstPlantAbsentTypeDTO> AllAbsentType { get; set; }
        public List<MstPlantAbsentTypeDTO> AllAbsentTypeActiveInAbsenteeism { get; set; }
        public List<LocationLookupList> PLNTChildLocationLookupList { get; set; }
        public string UploadPath { get; set; }

        #region Multiple Absenteeism Pop Up - CR1
        public IEnumerable<string> AbsentTypePopUpList { get; set; }
        #endregion
    }
}