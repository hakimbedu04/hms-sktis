using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeOthersProductionEntryPrint
{
    public class PrintDataModel
    {
        public List<string> TPKValue { get; set; }
        public List<int> WorkHours { get; set; } 
        public List<string> WeekDateList { get; set; }
        public string PrintDate { get; set; }
        public string Location { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string Process { get; set; }
        public string GroupCode { get; set; }
        public string BrandCode { get; set; }
        public int KpsYear { get; set; }
        public int KpsWeek { get; set; }
        public string Mandor { get; set; }
        public string Remark { get; set; }
        public List<ExeProductionEntryPrintViewPartitionModel> TableData { get; set; }

        public decimal MonTotalCapacity { get; set; }
        public decimal TueTotalCapacity { get; set; }
        public decimal WedTotalCapacity { get; set; }
        public decimal ThuTotalCapacity { get; set; }
        public decimal FriTotalCapacity { get; set; }
        public decimal SatTotalCapacity { get; set; }
        public decimal SunTotalCapacity { get; set; }

        public float MonTotalTarget { get; set; }
        public float TueTotalTarget { get; set; }
        public float WedTotalTarget { get; set; }
        public float ThuTotalTarget { get; set; }
        public float FriTotalTarget { get; set; }
        public float SatTotalTarget { get; set; }
        public float SunTotalTarget { get; set; }
               
        public float MonTotalActual { get; set; }
        public float TueTotalActual { get; set; }
        public float WedTotalActual { get; set; }
        public float ThuTotalActual { get; set; }
        public float FriTotalActual { get; set; }
        public float SatTotalActual { get; set; }
        public float SunTotalActual { get; set; }
    }
}