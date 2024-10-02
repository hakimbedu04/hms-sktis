using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantProductionEntryDTO
    {
        public string ProductionEntryCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? StartDateAbsent { get; set; }
        public string AbsentType { get; set; }
        public string EmployeeNumber { get; set; }
        public decimal? ProdCapacity { get; set; }
        public float? ProdTarget { get; set; }
        public float? ProdActual { get; set; }
        public string AbsentRemark { get; set; }
        public string AbsentCodeEblek { get; set; }
        public string AbsentCodePayroll { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        //public ExePlantProductionEntryVerification ExePlantProductionEntryVerification { get; set; }
        public int TotalTarget { get; set; }
        public bool AlreadySubmitFromEblekVer { get; set; }
        public string SaveType { get; set; }
        public bool? IsFromAbsenteeism { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string GroupCode { get; set; }
        public int Shift { get; set; }
        public int MinimumValueActual { get; set; }
        public float? SourceProdActual { get; set; }

    }
}
