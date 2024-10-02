using System.Collections.Generic;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeReportsSummaryCompositeDTO
    {
        public TPOFeeReportsSummaryCompositeDTO()
        {
            ListWeekValue = new List<TPOFeeReportsSummaryWeeklyDTO>();  
        }

        public string ParentLocation { get; set; }
        public string ParentLocationCode { get; set; }
        public bool IsParentRow { get; set; }
        public bool IsSummary { get; set; }

        public bool IsLocation { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string LocationAbbr { get; set; }

        public int Year { get; set; }
        public int MaxWeek { get; set; }

        public double? TotalCalculate { get; set; }

        public List<TPOFeeReportsSummaryWeeklyDTO> ListWeekValue { get; set; }
    }

    public class TPOFeeReportsSummaryWeeklyDTO
    {
        public bool IsParentRow { get; set; }
        public int Week { get; set; }
        public double WeekValue { get; set; }

        public string Location { get; set; }

    }

    public class TPOFeeReportsSummaryGoupByLocationWeek
    {
        public string LocationCode { get; set; }
        public int Week { get; set; }
        public int CountData { get; set; }
    }
}
