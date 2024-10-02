using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.MasterGenProccessSetting
{
    public class MstGenProcessSettingLocationViewModel : ViewModelBase
    {
        public int IDProcess { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
    }
}
