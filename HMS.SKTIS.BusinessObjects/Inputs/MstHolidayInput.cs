using System;

namespace HMS.SKTIS.BusinessObjects.Inputs
{
    public class MstHolidayInput : BaseInput
    {
        public DateTime? HolidayDate { get; set; }
        public int Year { get; set; }
        public string LocationCode { get; set; }
    }
}
