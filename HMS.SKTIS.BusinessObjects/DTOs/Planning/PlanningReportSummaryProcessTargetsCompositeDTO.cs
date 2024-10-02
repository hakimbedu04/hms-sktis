using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class PlanningReportSummaryProcessTargetsCompositeDTO
    {
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public double Giling { get; set; }
        public double Gunting { get; set; }
        public double WIPGunting { get; set; }
        public double Pak { get; set; }
        public double WIPPak { get; set; }
        public double Banderol { get; set; }
        public double Box { get; set; }
    }
}
