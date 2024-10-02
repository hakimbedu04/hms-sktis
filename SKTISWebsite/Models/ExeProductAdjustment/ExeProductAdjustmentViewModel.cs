using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeProductAdjustment
{
    public class ExeProductAdjustmentViewModel : ViewModelBase
    {
        public DateTime ProductionDate { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string BrandCode { get; set; }
        public string AdjustmentType { get; set; }
        public int AdjustmentValue { get; set; }
        public string AdjustmentRemark { get; set; }
    }
}