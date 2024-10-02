using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGenProcessDTO
    {
        public string ProcessGroup { get; set; }
        public string ProcessIdentifier { get; set; }
        public int ProcessOrder { get; set; }
        public bool? StatusActive { get; set; }
        public bool? WIP { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
