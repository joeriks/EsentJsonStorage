using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FnX
{
    public class EsentTypedStore<T> : IDisposable
    {
        public PersistentDictionary<string, string> Dictionary { get; set; }

        public class EsentTypedStoreOptions
        {
            public Func<T, string> GetId { get; set; }
            public Func<string> NewId { get; set; }
            public bool WithRevisions { get; set; }
            public EsentTypedStoreOptions()
            {
                //SetNewId = new Func<T, string>(t => new Guid().ToString());
                NewId = new Func<string>(() =>
                {
                    return Guid.NewGuid().ToString().Replace("-", "");
                });
                GetId = new Func<T, string>(t =>
                {
                    var id = t.GetType().GetProperty("Id").GetValue(t);
                    if (id == null) return "";
                    return id.ToString();
                });
                WithRevisions = true;

            }
        }
        public EsentTypedStoreOptions Options { get; set; }

        public PersistentDictionary<string, string> KeysDictionary { get; set; }
        public EsentTypedStore(PersistentDictionary<string, string> dictionary, EsentTypedStoreOptions options)
        {
            Dictionary = dictionary;
            //KeysDictionary = new PersistentDictionary<string, string>("keys");
            //GetId = options.GetId;
            if (options == null) options = new EsentTypedStoreOptions();
            Options = options;

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
        public string Set(T value)
        {
            var id = Options.GetId(value);
            if (string.IsNullOrEmpty(id)) id = Options.NewId();
            if (Options.WithRevisions && Dictionary.ContainsKey(id))
            {
                var revision = 1;
                while (Dictionary.ContainsKey(id + "-" + revision)) revision += 1;
                Set(id + "-" + revision, Get(id));
            }
            Set(id, value);
            return id;
        }
        public T Do(string key, Func<T, T> func)
        {
            lock (Dictionary)
            {
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

}
