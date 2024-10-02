using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenProcess
{
    public class MasterGenProcessIdentifier : ViewModelBase
    {
        public string ProcessGroup { get; set; }
        public string ProcessIdentifier { get; set; }
        public int ProcessOrder { get; set; }
    }
}