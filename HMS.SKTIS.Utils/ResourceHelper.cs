using System.Resources;

namespace HMS.SKTIS.Utils
{
    public class ResourceHelper
    {
        public static string GetResourceValueByName<T>(string name)
        {
            ResourceManager rm = new ResourceManager(typeof(T));
            return rm.GetString(name);
        }
    }
}
