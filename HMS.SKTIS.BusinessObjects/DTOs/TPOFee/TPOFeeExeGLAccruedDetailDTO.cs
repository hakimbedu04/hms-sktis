using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeExeGLAccruedDetailDTO
    {
        public string Regional { get; set; }
        public string RegionalName { get; set; }
        public int StickPerBox { get; set; }
        public float? Paket { get; set; }
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
        public int SumStick()
        {
            var listDailyStick = TpoFeeExeGlAccruedDailyDictionary.Select(c => c.Value).ToList();
            return listDailyStick.Sum(c => c.Batang);
        }
        public int SumBox()
        {
            var listDailyStick = TpoFeeExeGlAccruedDailyDictionary.Select(c => c.Value).ToList();
            return listDailyStick.Sum(c => c.Box);
        }
        public int SumBoxJkn()
        {
            var listDailyStick = TpoFeeExeGlAccruedDailyDictionary.Select(c => c.Value).ToList();
            return listDailyStick.Sum(c => c.Jkn);
        }
        public int SumBoxJl1()
        {
            var listDailyStick = TpoFeeExeGlAccruedDailyDictionary.Select(c => c.Value).ToList();
            return listDailyStick.Sum(c => c.Jl1);
        }
        public int SumBoxJl2()
        {
            var listDailyStick = TpoFeeExeGlAccruedDailyDictionary.Select(c => c.Value).ToList();
            return listDailyStick.Sum(c => c.Jl2);
        }
        public int SumBoxJl3()
        {
            var listDailyStick = TpoFeeExeGlAccruedDailyDictionary.Select(c => c.Value).ToList();
            return listDailyStick.Sum(c => c.Jl3);
        }
        public int SumBoxJl4()
        {
            var listDailyStick = TpoFeeExeGlAccruedDailyDictionary.Select(c => c.Value).ToList();
            return listDailyStick.Sum(c => c.Jl4);
        }
        public int TotalDibayar()
        {
            return SumBoxJkn() + SumBoxJl1() + SumBoxJl2() + SumBoxJl3() + SumBoxJl4();
        }
        public int TotalBiayaProduksiPerBoxJkn()
        {
            return SumBoxJkn()*JknFeeRate;
        }
        public int TotalBiayaProduksiPerBoxJl1()
        {
            return SumBoxJl1() * Jl1FeeRate;
        }
        public int TotalBiayaProduksiPerBoxJl2()
        {
            return SumBoxJl2() * Jl2FeeRate;
        }
        public int TotalBiayaProduksiPerBoxJl3()
        {
            return SumBoxJl3() * Jl3FeeRate;
        }
        public int TotalBiayaProduksiPerBoxJl4()
        {
            return SumBoxJl4() * Jl4FeeRate;
        }
        public int BiayaProduksiTotal()
        {
            return TotalBiayaProduksiPerBoxJkn() + TotalBiayaProduksiPerBoxJl1() + TotalBiayaProduksiPerBoxJl2() +
                   TotalBiayaProduksiPerBoxJl3() + TotalBiayaProduksiPerBoxJl4();
        }
        public int TotalJasaManagemen()
        {
            return BiayaProduksiTotal()*ManajemenFee;
        }

    }
}
