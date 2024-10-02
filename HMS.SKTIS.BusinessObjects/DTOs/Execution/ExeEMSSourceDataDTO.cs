using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeEMSSourceDataDTO
    {
        public string Company { get; set; }
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string BrandCode { get; set; }
        public string Description { get; set; }
        public Nullable<int> PackageQTY { get; set; }
        public Nullable<int> ProducedQTY { get; set; }
        public string UOM { get; set; }
        public DateTime ProductionDate { get; set; }
    }
}
