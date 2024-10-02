using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeEMSSourceData
{
    public class ExeReportEMSSourceDataViewModel : ViewModelBase 
    {
        public String Company { get; set; }
        public string LocationCode { get; set; }
        public string BrandGroupCode { get; set; }
        public string BrandCode { get; set; }
        public string Description { get; set; }
        public Nullable<int> PackageQTY { get; set; }
        public Nullable<int> ProducedQTY { get; set; }
        public string UOM { get; set; }
        public string ProductionDate { get; set; }
    }
}