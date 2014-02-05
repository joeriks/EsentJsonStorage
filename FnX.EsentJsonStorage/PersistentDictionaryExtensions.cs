using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FnX
{
    public static class PersistentDictionaryExtensions
    {
        public static EsentJsonStore GetStore(this PersistentDictionary<string, string> self, EsentJsonStore.StoreOptions options)
        {
            return new EsentJsonStore(self, options);
        }
        public static void Export(this PersistentDictionary<string, string> self, string path)
        {
            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(self));
        }
        public static void Import(this PersistentDictionary<string, string> self, string path)
        {
            var content = System.IO.File.ReadAllText(path);
            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(content);

            foreach (KeyValuePair<string, string> item in dictionary)
                self[item.Key] = item.Value;
        }

    }
}
