using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.UtilSecurityFunctions
{
    public class UtilSecurityFunctionsViewModel : ViewModelBase
    {
        public int IDFunction { get; set; }
        public int? ParentIDFunction { get; set; }
        public string ParentNameFunction { get; set; }
        public string FunctionName { get; set; }
        public string FunctionType { get; set; }
        public string FunctionDescription { get; set; }
    }
}