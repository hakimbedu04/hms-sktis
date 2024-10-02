using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;

namespace HMS.SKTIS.BusinessObjects.Inputs.Planning
{
    public class CalculateTPOTPKInput
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string LocationCode { get; set; }
        public string BrandCode { get; set; }
        public List<TPOTPKByProcessDTO> ListTPOTPK { get; set; }
        public List<PlanTPOTPKTotalBoxDTO> TotalTPOTPK { get; set; }
        public List<float> HeaderProcessWorkHours { get; set; }
        public DateTime FilterCurrentDayForward { get; set; }
        public bool IsFilterCurrentDayForward { get; set; }
        public bool IsWhChanged { get; set; }
    }
}

