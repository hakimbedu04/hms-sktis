using System;

namespace SKTISWebsite.Models.UtilWorkflowFlow
{
    public class UtilWorkflowFlowViewModel : ViewModelBase
    {
        public int IDFlow { get; set; }
        public int FormSource { get; set; }
        public string FormSourceName { get; set; }
        public Nullable<int> ActionButton { get; set; }
        public string ActionButtonName { get; set; }
        public Nullable<int> DestinationForm { get; set; }
        public string DestinationFormName { get; set; }
        public Nullable<int> DestinationRole { get; set; }
        public string DestinationRoleName { get; set; }
        public string MessageText { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}