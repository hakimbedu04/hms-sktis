using System;

namespace HMS.SKTIS.BusinessObjects.Inputs.Maintenance
{
    public class GetMaintenanceExecutionQualityInspectionInput : BaseInput
    {
        public string Location { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
