using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.TPOFeeSettingCalculation
{
    public class TPOFeeSettingCalculationViewModel : ViewModelBase
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? Value { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
