using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterTPOProductionGroup
{
    public class MasterTPOProductionGroupViewModel : ViewModelBase
    {
        public string ProdGroup { get; set; }
        public string ProcessGroup { get; set; }
        public string LocationCode { get; set; }
        public string StatusEmp { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int? WorkerCount { get; set; }
        public string ProdGroupComputed { get; set; }
    }
}