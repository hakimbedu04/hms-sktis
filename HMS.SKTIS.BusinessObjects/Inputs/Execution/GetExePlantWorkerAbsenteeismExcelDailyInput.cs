﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs.Execution
{
    public class GetExePlantWorkerAbsenteeismExcelDailyInput
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public DateTime Date { get; set; }
    }
}