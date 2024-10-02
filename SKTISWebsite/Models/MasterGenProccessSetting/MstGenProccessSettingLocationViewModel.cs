using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKTISWebsite.Models.MasterGenProccessSetting
{
    public class MstGenProccessSettingLocationViewModel : ViewModelBase
    {
        public string IDProcess { get; set; }
        public string LocationCode { get; set; }
        public string MaxWorker { get; set; }
    }
}
