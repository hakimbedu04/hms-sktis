namespace HMS.SKTIS.BusinessObjects.DTOs.Planning
{
    public class TargetManualTPUDTO
    {
        public TargetManualTPUDTO()
        {
            TargetManual1 = 0;
            TargetManual2 = 0;
            TargetManual3 = 0;
            TargetManual4 = 0;
            TargetManual5 = 0;
            TargetManual6 = 0;
            TargetManual7 = 0;
            TargetSystem1 = 0;
            TargetSystem2 = 0;
            TargetSystem3 = 0;
            TargetSystem4 = 0;
            TargetSystem5 = 0;
            TargetSystem6 = 0;
            TargetSystem7 = 0;
        }

        public float? TargetManual1 { get; set; }
        public float? TargetManual2 { get; set; }
        public float? TargetManual3 { get; set; }
        public float? TargetManual4 { get; set; }
        public float? TargetManual5 { get; set; }
        public float? TargetManual6 { get; set; }
        public float? TargetManual7 { get; set; }
        public float? TargetSystem1 { get; set; }
        public float? TargetSystem2 { get; set; }
        public float? TargetSystem3 { get; set; }
        public float? TargetSystem4 { get; set; }
        public float? TargetSystem5 { get; set; }
        public float? TargetSystem6 { get; set; }
        public float? TargetSystem7 { get; set; }
        public float? TotalTargetSystem
        {
            get
            {
                return TargetSystem1 + TargetSystem2 + TargetSystem3 + TargetSystem4 + TargetSystem5 + TargetSystem6 + TargetSystem7;
            }
        }
        public float? TotalTargetManual
        {
            get
            {
                return TargetManual1 + TargetManual2 + TargetManual3 + TargetManual4 + TargetManual5 + TargetManual6 + TargetManual7;
            }
        }
    }
}
