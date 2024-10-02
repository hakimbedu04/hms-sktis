using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.WagesProductionCardApprovalDetail
{
    public class InitWagesProductionCardApprovalDetailViewModel
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public int Shift { get; set; }
        public string UnitCode { get; set; }
        public string RevisionType { get; set; }
        public List<DateTime> ProductionDateList { get; set; }
        public List<string> BrandCodeList { get; set; }
        public string BrandGroupCode { get; set; }
        public int IDRole { get; set; }
    }
}