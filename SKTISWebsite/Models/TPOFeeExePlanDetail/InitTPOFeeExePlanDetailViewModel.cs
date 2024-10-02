using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;

namespace SKTISWebsite.Models.TPOFeeExePlanDetail
{
    public class InitTPOFeeExePlanDetailViewModel
    {
        public string PageFrom { get; set; }
        public string id { get; set; }
        public DateTime ClosingDate { get; set; }
        public string TpoFeeCode { get; set; }
        public string Regional { get; set; }
        public string RegionalName { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }

        public string LocationCompat { get; set; }
        public string CostCenter { get; set; }
        public string Brand { get; set; }
        public int? StickPerBox { get; set; }
        public int? KpsYear { get; set; }
        public int? KpsWeek { get; set; }
        public float? Paket { get; set; }
        public List<TpoFeeProductionDailyPlanModel> TpoFeeProductionDailyPlanModels { get; set; }
        public double? TotalProductionStick { get; set; }
        public double? TotalProductionBox { get; set; }
        public double? TotalProductionJkn { get; set; }
        public double? TotalProductionJl1 { get; set; }
        public double? TotalProductionJl2 { get; set; }
        public double? TotalProductionJl3 { get; set; }
        public double? TotalProductionJl4 { get; set; }
        public double? TotalCostDaily { get; set; }
        public double? TotalDibayarJKN { get; set; }
        public double? TotalDibayarJL1 { get; set; }
        public double? TotalDibayarJL2 { get; set; }
        public double? TotalDibayarJL3 { get; set; }
        public double? TotalDibayarJL4 { get; set; }
        public double? JknOutputProduksiYangDibayar { get; set; }
        public double? JknBiayaProduksiPerBox { get; set; }
        public double? JknTotalBiayaProduksiPerBox { get; set; }
        public double? Jl1OutputProduksiYangDibayar { get; set; }
        public double? Jl1BiayaProduksiPerBox { get; set; }
        public double? Jl1TotalBiayaProduksiPerBox { get; set; }
        public double? Jl2OutputProduksiYangDibayar { get; set; }
        public double? Jl2BiayaProduksiPerBox { get; set; }
        public double? Jl2TotalBiayaProduksiPerBox { get; set; }
        public double? Jl3OutputProduksiYangDibayar { get; set; }
        public double? Jl3BiayaProduksiPerBox { get; set; }
        public double? Jl3TotalBiayaProduksiPerBox { get; set; }
        public double? Jl4OutputProduksiYangDibayar { get; set; }
        public double? Jl4BiayaProduksiPerBox { get; set; }
        public double? Jl4TotalBiayaProduksiPerBox { get; set; }
        public double? BiayaProduksiOutputProduksiYangDibayar { get; set; }
        public double? BiayaProduksiBiayaProduksiPerBox { get; set; }
        public double? BiayaProduksiTotalBiayaProduksiPerBox { get; set; }
        public double? JasaManajemenOutputProduksiYangDibayar { get; set; }
        public double? JasaManajemenBiayaProduksiPerBox { get; set; }
        public double? JasaManajemenTotalBiayaProduksiPerBox { get; set; }
        public double? TotalBiayaProduksiDanJasaManagemenOutputProduksiYangDibayar { get; set; }
        public double? TotalBiayaProduksiDanJasaManagemenBiayaProduksiPerBox { get; set; }
        public double? TotalBiayaProduksiDanJasaManagemenTotalBiayaProduksiPerBox { get; set; }
        public double? JasaManajemenDuaPersenOutputProduksiYangDibayar { get; set; }
        public double? JasaManajemenDuaPersenBiayaProduksiPerBox { get; set; }
        public double? JasaManajemenDuaPersenTotalBiayaProduksiPerBox { get; set; }
        public double? TotalCostKeMpsOutputProduksiYangDibayar { get; set; }
        public double? TotalCostKeMpsBiayaProduksiPerBox { get; set; }
        public double? TotalCostKeMpsTotalBiayaProduksiPerBox { get; set; }
        public double? PembayaranOutputProduksiYangDibayar { get; set; }
        public double? PembayaranBiayaProduksiPerBox { get; set; }
        public double? PembayaranTotalBiayaProduksiPerBox { get; set; }
        public double? SisaYangHarusDiBayarOutputProduksiYangDibayar { get; set; }
        public double? SisaYangHarusDiBayarBiayaProduksiPerBox { get; set; }
        public double? SisaYangHarusDiBayarTotalBiayaProduksiPerBox { get; set; }
        public double? PpnBiayaProduksiSepuluhPersenOutputProduksiYangDibayar { get; set; }
        public double? PpnBiayaProduksiSepuluhPersenBiayaProduksiPerBox { get; set; }
        public double? PpnBiayaProduksiSepuluhPersenTotalBiayaProduksiPerBox { get; set; }
        public double? PpnJasaManajemenSepuluhPersenOutputProduksiYangDibayar { get; set; }
        public double? PpnJasaManajemenSepuluhPersenBiayaProduksiPerBox { get; set; }
        public double? PpnJasaManajemenSepuluhPersenTotalBiayaProduksiPerBox { get; set; }
        public double? TotalBayarOutputProduksiYangDibayar { get; set; }
        public double? TotalBayarBiayaProduksiPerBox { get; set; }
        public double? TotalBayarTotalBiayaProduksiPerBox { get; set; }
        public string VendorName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankType { get; set; }
        public string BankBranch { get; set; }
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string AuthorizedBy { get; set; }
        public List<TPOFeeCalculationPlanDto> Calculations { get; set; }

    }

    public class TpoFeeProductionDailyPlanModel
    {
        public string Hari { get; set; }
        public string TPOFeeCode { get; set; }
        public DateTime FeeDate { get; set; }
        public int? KPSYear { get; set; }
        public int? KPSWeek { get; set; }
        public double? OuputSticks { get; set; }
        public double? OutputBox { get; set; }
        public string OutputBoxS { get; set; }
        public double? JKN { get; set; }
        public double? JL1 { get; set; }
        public double? Jl2 { get; set; }
        public double? Jl3 { get; set; }
        public double? Jl4 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public double? JKNJam { get; set; }
        public double? JL1Jam { get; set; }
        public double? JL2Jam { get; set; }
        public double? JL3Jam { get; set; }
        public double? JL4Jam { get; set; }
    }

    
}