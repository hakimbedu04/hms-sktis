using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Utils
{
    public class UtilFunctionDTO
    {
        public int IDFunction { get; set; }
        public int? ParentIDFunction { get; set; }
        public string ParentNameFunction { get; set; }
        public string FunctionName { get; set; }
        public string FunctionType { get; set; }
        public string FunctionDescription { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
