using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGenProcessSettingDTO
    {
        public int IDProcess { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }        
        public int? StdStickPerHour { get; set; }
        public int? MinStickPerHour { get; set; }
        public int? UOMEblek { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int ProcessOrder { get; set; }
        public MstGenProcessDTO MstGenProcess { get; set; }
        public int? MaxWorker { get; set; }
    }
}
