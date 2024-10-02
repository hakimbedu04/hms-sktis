using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.Execution;

namespace HMS.SKTIS.Contracts
{
    public interface IExecutionOtherBLL
    {
        List<ProductAdjustmentDTO> GetProductAdjustments(ProductAdjustmentInput input);
        void SaveProductAdjustment(ProductAdjustmentDTO input);
        void UpdateProductAdjustment(ProductAdjustmentDTO input);

        List<ExeProductionEntryPrintDataViewDTO> GetExeProductionEntryPrintData(GetExeOthersProductionEntryPrintInput input);
        void SendEmailSubmitPlantEntryVerif(GetExePlantProductionEntryVerificationInput input, string currUserName);
        void SendEmailSubmitPlantEntry(GetExePlantProductionEntryInput input, string currUserName);
        void SendEmailReturnEntryVerification(GetExePlantProductionEntryVerificationInput input, string currUserName);
        List<string> GetBrandCodeByInput(GetExeOthersProductionEntryPrintInput input);

        List<string> GetBrandCodeFromReportByProcess(GetExePlantProductionEntryVerificationInput input);
    }
}
