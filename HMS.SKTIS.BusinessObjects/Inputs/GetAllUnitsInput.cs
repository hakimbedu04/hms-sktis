using System.Collections.Generic;
namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class GetAllUnitsInput
    {
        public string LocationCode { get; set; }
        public List<string> IgnoreList { get; set; }
    }
}
