using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGenWeekDTO
    {
        public MstGenWeekDTO()
        {
            TodayDate = DateTime.Now.Date;
        }

        public int IdMasterWeek { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Week { get; set; }
        public string Month { get; set; }
        public int? Year { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? TodayDate { get; set; }

    }
}
