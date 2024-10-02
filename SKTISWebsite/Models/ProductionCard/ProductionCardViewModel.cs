using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.ProductionCard
{
    public class ProductionCardViewModel : ViewModelBase
    {
        public int RevisionType { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int? Shift { get; set; }
        public string BrandCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public string GroupCode { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNameOnly { get; set; }
        public float? Production { get; set; }
        public string ProductionDate { get; set; }
        public int? WorkHours { get; set; }
        public string Absent { get; set; }
        public string UpahLain { get; set; }
        public string EblekAbsentType { get; set; }
        public string Remark { get; set; }
        public string Comments { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string ProductionCardCode { get; set; }
    }
}
