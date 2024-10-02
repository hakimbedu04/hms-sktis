using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs
{
    public class MstGenLocationDTO
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string CostCenter { get; set; }
        public string ABBR { get; set; }
        public int Shift { get; set; }
        public string ParentLocationCode { get; set; }
        public decimal UMK { get; set; }
        public string KPPBC { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public bool StatusActive { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string LocationCompat {
            get
            {
                return String.Format("{0} - {1}", LocationCode, LocationName);
            }
        }
    }
}
