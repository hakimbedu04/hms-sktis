using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGenStandardHourDTO
    {
        public string DayType { get; set; }
        public string DayName { get; set; }
        public int? JknHour { get; set; }
        public int? Jl1Hour { get; set; }
        public int? Jl2Hour { get; set; }
        public int? Jl3Hour { get; set; }
        public int? Jl4Hour { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
