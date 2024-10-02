using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.TPOFee
{
    public class TPOFeeExeGLAccruedDailyDTO
    {
        public DateTime Date { get; set; }
        public int TargetManual { get; set; }
        public int ProcessWorkHours { get; set; }
        public int Batang { get; set; }
        public int Box { get; set; }
        public int Jkn { get; set; }
        public int Jl1 { get; set; }
        public int Jl2 { get; set; }
        public int Jl3 { get; set; }
        public int Jl4 { get; set; }
        public bool IsOnlyOneWeek { get; set; }
    }
}
