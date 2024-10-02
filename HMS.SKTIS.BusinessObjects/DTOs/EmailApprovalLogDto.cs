using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class EmailApprovalLogDto
    {
        public string EmailApprovalLoglD { get; set; }
        public string AppName { get; set; }
        public string ModuleName { get; set; }
        public string ActionName { get; set; }
        public string TransactionID { get; set; }
        public string OrderNo { get; set; }
        public DateTime SentDate { get; set; }
        public int Status { get; set; }
        public int StatusApproval { get; set; }
        public string Subject { get; set; }
        public string ApprovedBy { get; set; }
        public int Response { get; set; }
        public string Reason { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
