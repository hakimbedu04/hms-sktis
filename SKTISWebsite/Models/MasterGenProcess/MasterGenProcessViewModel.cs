using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenProcess
{
    public class MasterGenProcessViewModel : ViewModelBase
    {
        public string ProcessGroup { get; set; }
        public string ProcessIdentifier { get; set; }
        public int ProcessOrder { get; set; }
        public bool? WIP { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}