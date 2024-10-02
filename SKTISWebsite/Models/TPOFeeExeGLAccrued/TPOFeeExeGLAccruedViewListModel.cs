﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.TPOFeeExeGLAccrued
{
    public class TPOFeeExeGLAccruedViewListModel
    {
        public bool Checkbox { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public string Brand { get; set; }
        public string Note { get; set; }
        public int JknBox { get; set; }
        public int Jl1Box { get; set; }
        public int Jl2Box { get; set; }
        public int Jl3Box { get; set; }
        public int Jl4Box { get; set; }
        public Int64 BiayaProduksi { get; set; }
        public Int64 JasaManagemen { get; set; }
        public string Regional { get; set; }
        public int KpsWeek { get; set; }
        public int KpsYear { get; set; }
    }
}