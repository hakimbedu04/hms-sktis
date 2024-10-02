using HMS.SKTIS.BusinessObjects.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class TransactionLogInput : BaseInput
    {
        public TransactionLogInput()
        {
            separator = "/";
            page = "";
            code_1 = "";
            code_2 = "";
            code_3 = "";
            code_4 = "";
            code_5 = "";
            code_6 = "";
            code_7 = "";
            code_8 = "";
            code_9 = "";
            ActionButton = "";
            ActionTime = DateTime.Now;
            TransactionDate = DateTime.Now;
            UserName = "";
            FunctionName = "";
            Message = "";
            statusEmployee = "";
            IDRole = null;
        }
        public string separator { get; set; }
        public string page { get; set; }
        public int year { get; set; }
        public int week { get; set; }
        public string code_1 { get; set; }
        public string code_2 { get; set; }
        public string code_3 { get; set; }
        public string code_4 { get; set; }
        public string code_5 { get; set; }
        public string code_6 { get; set; }
        public string code_7 { get; set; }
        public string code_8 { get; set; }
        public string code_9 { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ActionButton { get; set; }
        public DateTime ActionTime { get; set; }
        public string UserName { get; set; }
        public string FunctionName { get; set; }
        public string TransactionCode { get; set; }
        public int? NotEqualIdFlow { get; set; }
        public string Message { get; set; }

        public string statusEmployee { get; set; }
        public int? IDRole { get; set; }
    }
}
