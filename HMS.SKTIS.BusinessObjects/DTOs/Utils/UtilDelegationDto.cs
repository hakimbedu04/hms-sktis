
using System;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UtilDelegationDto
    {
        public string UserADTo { get; set; }
        public int IDResponsibility { get; set; }
        public string UserADFrom { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public string ResponsibilityDesc { get; set; }

        public string UserADToOld { get; set; }
        public int IDResponsibilityOld { get; set; }
        public string UserADFromOld { get; set; }
        public DateTime StartDateOld { get; set; }
    }
}
