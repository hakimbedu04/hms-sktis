using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeHdrDTO
    {
        public string TPOFeeCode { get; set; }
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public Nullable<int> TPOFeeTempID { get; set; }
        public string TaxtNoProd { get; set; }
        public string TaxtNoMgmt { get; set; }
        public string CurrentApproval { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Status { get; set; }
        public string PengirimanL1 { get; set; }
        public string PengirimanL2 { get; set; }
        public string PengirimanL3 { get; set; }
        public string PengirimanL4 { get; set; }
        public Nullable<int> StickPerBox { get; set; }
        public Nullable<double> TPOPackageValue { get; set; }
        public Nullable<double> StickPerPackage { get; set; }
        public Nullable<double> TotalProdStick { get; set; }
        public Nullable<double> TotalProdBox { get; set; }
        public Nullable<double> TotalProdJKN { get; set; }
        public Nullable<double> TotalProdJl1 { get; set; }
        public Nullable<double> TotalProdJl2 { get; set; }
        public Nullable<double> TotalProdJl3 { get; set; }
        public Nullable<double> TotalProdJl4 { get; set; }
    }
}
