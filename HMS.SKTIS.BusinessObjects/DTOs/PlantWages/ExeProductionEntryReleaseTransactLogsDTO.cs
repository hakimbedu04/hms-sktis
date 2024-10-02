using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.PlantWages
{
    public class ExeProductionEntryReleaseTransactLogsDTO
    {
        public string ProductionEntryCode { get; set; }

        public DateTime EblekDate { get; set; }

        public string GroupCode { get; set; }

        public string BrandCode { get; set; }

        public bool? IsLocked { get; set; }

        public string Remark { get; set; }

        public string TransactionCode { get; set; }

        public int? IDFlow { get; set; }
    }
}
