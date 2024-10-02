
using System.Collections.Generic;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeReportsPackageCompositeDTO
    {

        public TPOFeeReportsPackageCompositeDTO()
        {
          ListWeekValue = new List<TPOFeeReportsPackageWeeklyDTO>();  
        }

        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string BrandCode { get; set; }
        public string MemoReff { get; set; }

        public List<TPOFeeReportsPackageWeeklyDTO> ListWeekValue { get; set; }

        public int Year { get; set; }
        public int MaxWeek { get; set; }

        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
    }

    public class TPOFeeReportsPackageWeeklyDTO
    {
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string PackageValue { get; set; }
        public float PackageInFloat { get; set; }
        public bool IsChangedValue { get; set; }
    }
}
