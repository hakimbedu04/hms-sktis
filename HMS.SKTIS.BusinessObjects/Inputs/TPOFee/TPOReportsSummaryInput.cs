using HMS.SKTIS.Core;

namespace HMS.SKTIS.BusinessObjects.Inputs.TPOFee
{
    public class GetTPOReportsSummaryInput : BaseInput
    {
        public int Year { get; set; }
        public Enums.TpoFeeReportsSummaryProductionFeeType ProductionFeeType { get; set; }

        public string ProductionType { get; set; }
    }

}
