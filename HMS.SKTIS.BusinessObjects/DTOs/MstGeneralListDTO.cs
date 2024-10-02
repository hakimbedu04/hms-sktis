using System;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGeneralListDTO
    {
        public string ListGroup { get; set; }
        public string ListDetail { get; set; }
        public bool? StatusActive { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
