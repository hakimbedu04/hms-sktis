using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeExeGLAccruedHdrDTO
    {
        public string Regional { get; set; }
        public string RegionalName { get; set; }
        public int StickPerBox { get; set; }
        public decimal Paket { get; set; }
        public int KpsYear { get; set; }
        public int KpsWeek { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public string LocationName { get; set; }
        public string CostCenter { get; set; }
    }
}
