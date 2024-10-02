using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeExeAPOpenViewDTO
    {
        public string TPOFeeCode { get; set; }
        public string LocationName { get; set; }
        public string SKTBrandCode { get; set; }
        public string Status { get; set; }
        public string PIC { get; set; }
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string DestinationRolesCodes { get; set; }
    }
}
