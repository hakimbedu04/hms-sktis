﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstPlantProductionGroupDTO
    {
        public string UnitCode { get; set; }
        public string LocationCode { get; set; }
        public string GroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public int WorkerCount { get; set; }
        public string NextGroupCode { get; set; }
        public string LeaderInspection { get; set; }
        public string LeaderInspectionName { get; set; }
        public string Leader1 { get; set; }
        public string Leader1Name { get; set; }
        public string Leader2 { get; set; }
        public string Leader2Name { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string AvaiableNumbers { get; set; }
    }
}
