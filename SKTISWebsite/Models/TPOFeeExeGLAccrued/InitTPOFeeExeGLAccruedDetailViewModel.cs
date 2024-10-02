using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;

namespace SKTISWebsite.Models.TPOFeeExeGLAccrued
{
    public class InitTPOFeeExeGLAccruedDetailViewModel
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

        public Dictionary<int, TPOFeeExeGLAccruedDailyDTO> TpoFeeExeGlAccruedDailyDictionary { get; set; }
        public int JknFeeRate { get; set; }
        public int Jl1FeeRate { get; set; }
        public int Jl2FeeRate { get; set; }
        public int Jl3FeeRate { get; set; }
        public int Jl4FeeRate { get; set; }
        public int ManajemenFee { get; set; }
        public int SumStick { get; set; }
        public int SumBox { get; set; }
        public int SumBoxJkn { get; set; }
        public int SumBoxJl1 { get; set; }
        public int SumBoxJl2 { get; set; }
        public int SumBoxJl3 { get; set; }
        public int SumBoxJl4 { get; set; }
        public int TotalDibayar { get; set; }
        public int TotalBiayaProduksiPerBoxJkn { get; set; }
        public int TotalBiayaProduksiPerBoxJl1 { get; set; }
        public int TotalBiayaProduksiPerBoxJl2 { get; set; }
        public int TotalBiayaProduksiPerBoxJl3 { get; set; }
        public int TotalBiayaProduksiPerBoxJl4 { get; set; }
        public int BiayaProduksiTotal { get; set; }
        public int TotalJasaManagemen { get; set; }
    }
}