using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.TPOFeeAPOpen
{
    public class TPOFeeAPOpenViewModel : ViewModelBase
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string SKTBrandCode { get; set; }
        public string Note { get; set; }
        public double? JKN { get; set; }
        public double? JL1 { get; set; }
        public double? JL2 { get; set; }
        public double? JL3 { get; set; }
        public double? JL4 { get; set; }
        public double? BiayaProduksi { get; set; }
        public double? JasaManajemen { get; set; }
        public decimal? ProductivityIncentives { get; set; }
        public double? JasaManajemen2Percent { get; set; }
        public decimal? ProductivityIncentives2Percent { get; set; }
        public double? BiayaProduksi10Percent { get; set; }
        public double? JasaMakloon10Percent { get; set; }
        public decimal? ProductivityIncentives10Percent { get; set; }
        public double? TotalBayar { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public int? IDFlow { get; set; }
        public string TPOFeeCode { get; set; }
        public string Status { get; set; }
        public bool Check { get; set; }
        public string TaxtNoMgmt { get; set; }
        public string TaxtNoProd { get; set; }
    }
}
