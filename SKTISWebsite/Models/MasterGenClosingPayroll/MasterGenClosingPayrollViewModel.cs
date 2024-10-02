using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenClosingPayroll
{
    public class MasterGenClosingPayrollViewModel : ViewModelBase
    {
        public bool? Checkbox { get; set; }
        public DateTime ClosingDate { get; set; }
    }
}