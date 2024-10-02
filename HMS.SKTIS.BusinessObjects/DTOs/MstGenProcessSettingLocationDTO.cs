using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGenProcessSettingLocationDTO
    {
        public long ID { get; set; }
        public int IDProcess { get; set; }
        public string LocationCode { get; set; }
        public int? MaxWorker { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
