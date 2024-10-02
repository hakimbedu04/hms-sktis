using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class InsertMultipleAbsenteeismDTO
    {
        public List<EmployeeMultipleInsertAbsenteeismDTO> ListEmployees { get; set; }
        public string AbsentType { get; set; }
        public DateTime StartDateAbsent { get; set; }
        public DateTime EndDateAbsent { get; set; }
        public string LocationCode { get; set; }
        public string GroupCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class EmployeeMultipleInsertAbsenteeismDTO
    {
        public EmployeeMultipleInsertAbsenteeismDTO()
        {
            IsValidated = true;
        }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ErrorMessage { get; set; }
        public string ResponseType { get; set; }
        public bool IsValidated { get; set; }
        public bool IsChecked { get; set; }
    }
}
