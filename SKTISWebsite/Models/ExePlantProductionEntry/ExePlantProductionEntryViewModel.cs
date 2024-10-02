using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.ExePlantProductionEntry
{
    public class ExePlantProductionEntryViewModel : ViewModelBase
    {
        public string ProductionEntryCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string StartDateAbsent { get; set; }
        public string AbsentType { get; set; }
        public string EmployeeNumber { get; set; }
        public string ProdCapacity { get; set; }
        public string ProdTarget { get; set; }
        public float? ProdActual { get; set; }
        public string AbsentRemark { get; set; }
        public string AbsentCodeEblek { get; set; }
        public string AbsentCodePayroll { get; set; }
        public string ProductionDate { get; set; }
        public int TotalProdTarget { get; set; }
        public bool AlreadySubmitFromEblekVer { get; set; }
        public bool? IsFromAbsenteeism { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string GroupCode { get; set; }
        public int Shift { get; set; }
        public int MinimumValueActual { get; set; }
        public float? SourceProdActual { get; set; }
    }
}
