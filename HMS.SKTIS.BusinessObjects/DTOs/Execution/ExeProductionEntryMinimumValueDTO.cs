using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeProductionEntryMinimumValueDTO
    {
        public string LocationCode { get; set; }
        public int IDProcess { get; set; }
        public string BrandGroupCode { get; set; }
        public string UnitCode { get; set; }
        public string BrandCode { get; set; }
        public string ProcessGroup { get; set; }
        public string GroupCode { get; set; }
        public Nullable<int> StdStickPerHour { get; set; }
        public Nullable<int> MinimalStickPerHour { get; set; }
        public Nullable<int> UOMEblek { get; set; }
        public Nullable<int> Shift { get; set; }
    }
}
