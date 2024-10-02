using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HMS.SKTIS.Utils
{
    public static class JsonUtils
    {
        public static string ConvertToJson(object obj)
        {
            var settings = new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() };
            return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
        }
    }
}
