using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeApprovalViewDTO
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
        public bool AlreadyApprove { get; set; }
        public string TaxtNoMgmt { get; set; }
        public string TaxtNoProd { get; set; }
        public int? DestinationRole { get; set; }
        public string ParentLocationCode { get; set; }
        public string DestinationFunctionForm { get; set; }
        public string DestinationRolesCodes { get; set; }
        public string PIC { get; set; }
        public string BrandGroupCode { get; set; }
        public int? TPOFeeTempID { get; set; }
        public string CurrentApproval { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Expr1 { get; set; }
        public string PengirimanL1 { get; set; }
        public string PengirimanL2 { get; set; }
        public string PengirimanL3 { get; set; }
        public string PengirimanL4 { get; set; }
        public int? StickPerBox { get; set; }
        public double? TPOPackageValue { get; set; }
        public double? StickPerPackage { get; set; }
        public double? TotalProdStick { get; set; }
        public double? TotalProdBox { get; set; }
        public double? TotalProdJKN { get; set; }
        public double? TotalProdJl1 { get; set; }
        public double? TotalProdJl2 { get; set; }
        public double? TotalProdJl3 { get; set; }
        public double? TotalProdJl4 { get; set; }
    }
}
