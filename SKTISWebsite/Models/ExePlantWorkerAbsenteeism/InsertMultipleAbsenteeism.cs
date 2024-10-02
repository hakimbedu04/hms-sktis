using System;
using System.Collections.Generic;

namespace SKTISWebsite.Models.ExePlantWorkerAbsenteeism
{
    public class InsertMultipleAbsenteeism
    {
        public List<EmployeeMultipleInsertAbsenteeism> ListEmployees { get; set; }
        public List<EmployeeMultipleInsertAbsenteeism> ListEmployeesFails { get; set; }
        public string AbsentType { get; set; }
        public DateTime StartDateAbsent { get; set; }
        public DateTime EndDateAbsent { get; set; }
        public string LocationCode { get; set; }
        public string GroupCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
    }

    public class EmployeeMultipleInsertAbsenteeism {
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public DateTime ProductionDate { get; set; }
        public bool IsChecked { get; set; }
    }
}