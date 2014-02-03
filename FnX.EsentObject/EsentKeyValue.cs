using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FnX
{
    public class EsentKeyValue
    {
        public static PersistentDictionary<string, string> getDictionary(string physicalName)
        {

            var path = "";


            if (HttpContext.Current != null)
                path = HttpContext.Current.Server.MapPath("/App_Data/" + physicalName);
            else
                path = System.IO.Path.GetTempPath() + "/EsentObject/" + physicalName;

            return new PersistentDictionary<string, string>(path);

        }

        public static string Get(string key)
        {
            return Get<string>("String", key);
        }
        public static string Get(string physicalName, string key)
        {
            return Get<string>(physicalName, key);
        }

        public static T Get<T>(string key)
        {
            return Get<T>(typeof(T).Name, key);
        }

        public static T Get<T>(string physicalName, string key)
        {
            using (var pd = getDictionary(physicalName))
            {
                if (pd.ContainsKey(key)) return JsonConvert.DeserializeObject<T>(pd[key]);
                return default(T);
            }
        }

        public static void Set<T>(string key, T value)
        {
            Set<T>(typeof(T).Name, key, value);
        }

        public static void Set<T>(string physicalName, string key, T value)
        {
            using (var pd = getDictionary(physicalName))
            {
                pd[key] = JsonConvert.SerializeObject(value);
            }
        }

    }
}

