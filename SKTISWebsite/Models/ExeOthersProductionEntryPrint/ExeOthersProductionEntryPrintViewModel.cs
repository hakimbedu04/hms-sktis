﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.ExeOthersProductionEntryPrint
{
    public class ExeOthersProductionEntryPrintViewModel
    {
        public int KPSYear { get; set; }
        public int KPSWeek { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeNumber { get; set; }
        public string MonAbsentType { get; set; }
        public string MonAbsentRemark { get; set; }
        public int? MonProdCapacity { get; set; }
        public float? MonProdTarget { get; set; }
        public float? MonProdActual { get; set; }
        public string TueAbsentType { get; set; }
        public string TueAbsentRemark { get; set; }
        public int? TueProdCapacity { get; set; }
        public float? TueProdTarget { get; set; }
        public float? TueProdActual { get; set; }
        public string WedAbsentType { get; set; }
        public string WedAbsentRemark { get; set; }
        public int? WedProdCapacity { get; set; }
        public float? WedProdTarget { get; set; }
        public float? WedProdActual { get; set; }
        public string ThuAbsentType { get; set; }
        public string ThuAbsentRemark { get; set; }
        public int? ThuProdCapacity { get; set; }
        public float? ThuProdTarget { get; set; }
        public float? ThuProdActual { get; set; }
        public string FriAbsentType { get; set; }
        public string FriAbsentRemark { get; set; }
        public int? FriProdCapacity { get; set; }
        public float? FriProdTarget { get; set; }
        public float? FriProdActual { get; set; }
        public string SatAbsentType { get; set; }
        public string SatAbsentRemark { get; set; }
        public int? SatProdCapacity { get; set; }
        public float? SatProdTarget { get; set; }
        public float? SatProdActual { get; set; }
        public string SunAbsentType { get; set; }
        public string SunAbsentRemark { get; set; }
        public int? SunProdCapacity { get; set; }
        public float? SunProdTarget { get; set; }
        public float? SunProdActual { get; set; }
        public string LocationCode { get; set; }
        public string UnitCode { get; set; }
        public int Shift { get; set; }
        public string ProcessGroup { get; set; }
        public string GroupCode { get; set; }
        public string BrandCode { get; set; }
        public string EmployeeNumberCompact
        {
            get
            {
                return EmployeeNumber.Substring(EmployeeNumber.Length - 2);
            }
        }
    }
}