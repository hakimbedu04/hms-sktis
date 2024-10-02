using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Models.MasterPlantProductionGroup
{
    public class MstPlantProductionGroupViewModel : ViewModelBase
    {
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string GroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public int WorkerCount { get; set; }
        public string NextGroupCode { get; set; }
        public string InspectionLeader { get; set; }
        public string Leader1 { get; set; }
        public string Leader2 { get; set; }
        public string LeaderInspectionName { get; set; }
        public string Leader1Name { get; set; }
        public string Leader2Name { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string AvaiableNumbers { get; set; }
    }
}