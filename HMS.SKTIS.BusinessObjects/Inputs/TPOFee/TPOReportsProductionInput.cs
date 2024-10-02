using System;

namespace HMS.SKTIS.BusinessObjects.Inputs.TPOFee
{
    public class GetTPOReportsProductionInput : BaseInput
    {
        public string LocationCode { get; set; }

        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public int WeekFrom { get; set; }
        public int WeekTo { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string FilterType { get; set; }
    }
}
