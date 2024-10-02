using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.MasterGenProccessSetting
{
    public class ProcessSettingsAndLocationViewModel
    {
        public string LocationCode { get; set; }
        public int? MaxWorker { get; set; }
        public int IDProcess { get; set; }
        public string BrandGroupCode { get; set; }
        public string ProcessGroup { get; set; }
        public int? StdStickPerHour { get; set; }
        public int? MinStickPerHour { get; set; }
        public int? UOMEblek { get; set; }
        public string LocationName { get; set; }
        public string ParentLocationCode { get; set; }
        public string CostCenter { get; set; }
        public string ABBR { get; set; }
        public int? Shift { get; set; }
        public decimal? UMK { get; set; }
        public string KPPBC { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public bool? StatusActiveLocation { get; set; }
        public string ProcessIdentifier { get; set; }
        public int ProcessOrder { get; set; }
        public bool? StatusActiveProcess { get; set; }
        public bool WIP { get; set; }
        public string BrandFamily { get; set; }
        public string PackType { get; set; }
        public string ClassType { get; set; }
        public int StickPerPack { get; set; }
        public int PackPerSlof { get; set; }
        public int SlofPerBal { get; set; }
        public int BalPerBox { get; set; }
        public string SKTBrandCode { get; set; }
        public string Description { get; set; }
        public int CapacityPackage { get; set; }
        public double? CigarreteWeight { get; set; }
        public int EmpPackage { get; set; }
        public int? StickPerBox { get; set; }
        public int? StickPerSlof { get; set; }
        public int? StickPerBal { get; set; }
        public bool? StatusActiveBrand { get; set; }
    }
}