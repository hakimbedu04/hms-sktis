namespace SKTISWebsite.Models.TPOFeeReportsProduction
{
    public class TPOFeeReportsProductionViewModel
    {
        public string Regional { get; set; }
        public string LocationCode { get; set; }
        public string LocationAbbr { get; set; }
        public string LocationName { get; set; }

        public decimal? Umk { get; set; }
        public string Brand { get; set; }
        public double? Package { get; set; }

        public double? JKNProductionFee { get; set; }
        public double? JL1ProductionFee { get; set; }
        public double? JL2ProductionFee { get; set; }
        public double? JL3ProductionFee { get; set; }
        public double? JL4ProductionFee { get; set; }

        public double? ManagementFee { get; set; }
        public double? ProductivityIncentives { get; set; }

        public int Year { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public int WeekFrom { get; set; }
        public int WeekTo { get; set; }

        public string NoMemo { get; set; }

        public double? JKNProductionVolume { get; set; }
        public double? JL1ProductionVolume { get; set; }
        public double? JL2ProductionVolume { get; set; }
        public double? JL3ProductionVolume { get; set; }
        public double? JL4ProductionVolume { get; set; }


    }
}