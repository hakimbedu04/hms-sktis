using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstHolidayByWeek
    {
        public bool Senin { get; set; }

        public bool Selasa { get; set; }

        public bool Rabu { get; set; }

        public bool Kamis { get; set; }

        public bool Jumat { get; set; }

        public bool Sabtu { get; set; }

        public bool Minggu { get; set; }
    }
}
