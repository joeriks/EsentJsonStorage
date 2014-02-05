using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace EsentJsonStorage
{
    public static class Storage
    {
        public static Store GetStore(string physicalName = "", Store.StoreOptions options = null)
        {
            if (physicalName == "") physicalName = "default";
            return GetDictionary(physicalName).GetStore(options);
        }
        public static PersistentDictionary<string, string> GetDictionary(string physicalName = "default")
        {

            var path = Path.Combine(Environment.CurrentDirectory, @"..\App_Data\" + physicalName);
            System.IO.Directory.CreateDirectory(path);

            //if (HttpContext.Current != null)
            //    path = HttpContext.Current.Server.MapPath("/App_Data/" + physicalName);
            //else
            //    path = System.IO.Path.GetTempPath() + "/EsentObject/" + physicalName;

            return existingOrNew(path);

        }
        private static ConcurrentDictionary<string, PersistentDictionary<string, string>> _dictionaries = new ConcurrentDictionary<string, PersistentDictionary<string, string>>();
        private static PersistentDictionary<string, string> existingOrNew(string path, int retry = 0)
        {
            lock (_dictionaries)
            {
                if (_dictionaries.ContainsKey(path)) return _dictionaries[path];
                var dictionary = new PersistentDictionary<string, string>(path);
                _dictionaries[path] = dictionary;
                return dictionary;
            }
        }
    }
}