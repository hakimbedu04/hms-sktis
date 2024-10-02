using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterPlantAbsentType
{
    public class MstPlantAbsentTypeViewModel : ViewModelBase
    {
        public string AbsentType { get; set; }
        public string SktAbsentCode { get; set; }
        public string PayrollAbsentCode { get; set; }
        public string AlphaReplace { get; set; }
        public int? MaxDay { get; set; }
        public bool? ActiveInAbsent { get; set; }
        public bool? ActiveInProductionEntry { get; set; }
        public string Calculation { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string OldAbsentType { get; set; }
    }
}