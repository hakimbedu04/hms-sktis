using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UtilFlowFunctionViewDTO
    {
        public string UserAD { get; set; }
        public string SourceRolesCodes { get; set; }
        public string SourceFunctionForm { get; set; }
        public string FunctionType { get; set; }
        public string FunctionName { get; set; }
        public string FunctionTypeChild { get; set; }
        public string DestinationFunctionForm { get; set; }
        public string DestinationRolesCodes { get; set; }
        public int IDFlow { get; set; }
        public string MessageText { get; set; }
    }
}
