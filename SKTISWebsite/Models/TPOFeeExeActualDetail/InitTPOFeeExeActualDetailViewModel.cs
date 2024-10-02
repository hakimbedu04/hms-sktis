using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;

namespace SKTISWebsite.Models.TPOFeeExeActualDetail
{
    public class InitTPOFeeExeActualDetailViewModel
    {
        public string TPOFeeCode { get; set; }
        public string Regional { get; set; }
        public string RegionalName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string CostCenter { get; set; }
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string Brand { get; set; }
        public int? StickPerBox { get; set; }
        public float? Paket { get; set; }
        public List<HMS.SKTIS.BusinessObjects.DTOs.TPOFee.TPOFeeProductionDailyDTO> Daily { get; set; }
        public double? TotalProdStick { get; set; }
        public double? TotalProdBox { get; set; }
        public double? TotalProdJKN { get; set; }
        public double? TotalProdJl1 { get; set; }
        public double? TotalProdJl2 { get; set; }
        public double? TotalProdJl3 { get; set; }
        public double? TotalProdJl4 { get; set; }
        public double? TotalDibayarJKN { get; set; }
        public double? TotalDibayarJL1 { get; set; }
        public double? TotalDibayarJL2 { get; set; }
        public double? TotalDibayarJL3 { get; set; }
        public double? TotalDibayarJL4 { get; set; }
        public List<TPOFeeCalculationDTO> Calculations { get; set; }
        public string PengirimanL1 { get; set; }
        public string PengirimanL2 { get; set; }
        public string PengirimanL3 { get; set; }
        public string PengirimanL4 { get; set; }
        public string TaxtNoProd { get; set; }
        public string TaxtNoMgmt { get; set; }
        public string PreparedBy  { get; set; }
        public string ApprovedBy  { get; set; }
        public string AuthorizedBy  { get; set; }
        public bool DisableBtnSave { get; set; }
        public string PageFrom { get; set; }
        public string Status { get; set; }
        public int? RoleSave { get; set; }
        public int? RoleSubmit { get; set; }
        public int? RoleApprove { get; set; }
        public int? RoleAuthorize { get; set; }
        public int? RoleRevise { get; set; }
        public int? RoleComplete { get; set; }
        public int? RoleReturn { get; set; }

        public List<string> HolidayDay { get; set; }
    }
}
