using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FnX
{
    public class EsentTypedStore<T> : IDisposable
    {
        public PersistentDictionary<string, string> Dictionary {get;set;}
        public Func<T, string> GetId { get; set; }

        public PersistentDictionary<string, string> KeysDictionary { get; set; }
        

        public EsentTypedStore(PersistentDictionary<string, string> dictionary, Func<T, string> getId)
        {
            Dictionary = dictionary;
            //KeysDictionary = new PersistentDictionary<string, string>("keys");
            GetId = getId;
        }
        public T Get(string key)
        {
            if (Dictionary.ContainsKey(key)) return JsonConvert.DeserializeObject<T>(Dictionary[key]);
            return default(T);
        }
        public T Get<T>(string key)
        {
            if (Dictionary.ContainsKey(key)) return JsonConvert.DeserializeObject<T>(Dictionary[key]);
            return default(T);
        }
        public void Set(T value)
        {            
            Set(GetId(value), value);
        }
        public T Do(string key, Func<T,T> func){
            lock (Dictionary) {
                var originalValue = Get<T>(key);
                var newValue = func(originalValue);
                Set(key, newValue);
                return newValue;
            }
        }
        public void Set<T>(string key, T value)
        {
            lock (Dictionary)
            {
                Dictionary[key] = JsonConvert.SerializeObject(value);
            }
        }

        public void Dispose()
        {
            // 
        }
    }
    public static class EsentKeyValueExtensions
    {
        public static EsentTypedStore<T> GetStore<T>(this PersistentDictionary<string, string> self, Func<T, string> getId)
        {
            return new EsentTypedStore<T>(self, getId);
        }
        public static T Get<T>(this PersistentDictionary<string, string> self, string key)
        {
            if (self.ContainsKey(key)) return JsonConvert.DeserializeObject<T>(self[key]);
            return default(T);
        }
        public static void Set(this PersistentDictionary<string, string> self, string key, object value)
        {
            self[key] = JsonConvert.SerializeObject(value);
        }

        public static void Export(this PersistentDictionary<string, string> self, string path)
        {
            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(self));
        }
        public static void Import(this PersistentDictionary<string, string> self, string path)
        {
            var content = System.IO.File.ReadAllText(path);
            var dictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(content);

            foreach(KeyValuePair<string,string> item in dictionary)
                self[item.Key]=item.Value;
        }
    }

    public class EsentKeyValue
    {
        
        public static EsentTypedStore<string> GetStore(string physicalName="")
        {
            return GetStore<string>(physicalName);
        }

        public static EsentTypedStore<T> GetStore<T>(Func<T, string> GetId)
        {
            return GetStore<T>("", GetId);
        }
        public static EsentTypedStore<T> GetStore<T>(string physicalName = "", Func<T, string> GetId = null)
        {
            if (physicalName == "") physicalName = typeof(T).Name;
            return GetDictionary(physicalName).GetStore<T>(GetId);
        }
        public static ConcurrentDictionary<string, PersistentDictionary<string, string>> _dictionaries = new ConcurrentDictionary<string, PersistentDictionary<string, string>>();
        public static PersistentDictionary<string, string> GetDictionary(string physicalName="default")
        {
    
            var path = "";

            if (HttpContext.Current != null)
                path = HttpContext.Current.Server.MapPath("/App_Data/" + physicalName);
            else
                path = System.IO.Path.GetTempPath() + "/EsentObject/" + physicalName;

            return existingOrNew(path);

        }

        private static PersistentDictionary<string, string> existingOrNew(string path, int retry=0) {


            lock (_dictionaries)
            {
                if (_dictionaries.ContainsKey(path)) return _dictionaries[path];
                var dictionary = new PersistentDictionary<string, string>(path);
                _dictionaries[path] = dictionary;
                return dictionary;
            }
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
            using (var pd = GetDictionary(physicalName))
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
            using (var pd = GetDictionary(physicalName))
            {
                pd[key] = JsonConvert.SerializeObject(value);
            }
        }


    }
}

