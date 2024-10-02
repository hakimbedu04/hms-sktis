﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExePlantActualWorkHoursDTO
    {
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string ProcessGroup { get; set; }
        public string BrandCode { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string StatusEmp { get; set; }
        public string StatusIdentifier { get; set; }
        public int? Shift { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public TimeSpan? BreakTime { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
