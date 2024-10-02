using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.DTOs.Execution
{
    public class ExeProductionEntryPrintViewPartitionDTO
    {
        public ExeProductionEntryPrintViewPartitionDTO(List<ExeProductionEntryPrintViewDTO> data)
        {
            PrintData = data;
        }
        public List<ExeProductionEntryPrintViewDTO> PrintData { get; set; }
    }
}
