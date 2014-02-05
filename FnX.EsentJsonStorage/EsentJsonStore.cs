using Microsoft.Isam.Esent.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FnX
{
    public class EsentJsonStore : IDisposable
    {
        public PersistentDictionary<string, string> Dictionary { get; set; }

        public class StoreOptions
        {
            public bool WithRevisions { get; set; }
            public bool WithTimeStamps { get; set; }
            
            public string DeserializeIdTo { get; set; }
            public string DeserializeUpdateTo { get; set; }

            public StoreOptions()
            {
                WithRevisions = true;
                WithTimeStamps = true;
                DeserializeIdTo = "Id";
                DeserializeUpdateTo = "UpdateDate";
            }
        }
        public StoreOptions Options { get; set; }

        public PersistentDictionary<string, string> KeysDictionary { get; set; }
        public EsentJsonStore(PersistentDictionary<string, string> dictionary, StoreOptions options)
        {
            Dictionary = dictionary;
            if (options == null) options = new StoreOptions();
            Options = options;

        }

        public T Deserialize<T>(string value)
        {
            if (Options.DeserializeIdTo != "" || Options.DeserializeUpdateTo != "")
            {
                var jObject = JObject.Parse(value);
                if (Options.DeserializeIdTo != "")
                {
                    jObject[Options.DeserializeIdTo] = jObject["_id"];
                }
                if (Options.DeserializeUpdateTo != "")
                {
                    jObject[Options.DeserializeUpdateTo] = jObject["_date"];
                }
                return jObject.ToObject<T>();
            }

            return JsonConvert.DeserializeObject<T>(value);
        }


        public string Serialize<T>(T value, string id = "", int revision = 0)
        {

            return Stamp(JObject.FromObject(value), id, revision).ToString();
        }

        public JObject Stamp(string json, string id = "", int revision = 0)
        {

            JObject parsedValue;
            try
            {
                parsedValue = JObject.Parse(json);
            }
            catch (Exception)
            {

                parsedValue = new JObject();
                parsedValue["_val"] = json;
            }

            return Stamp(parsedValue, id, revision);
        }

        public JObject Stamp(JObject jObject, string id = "", int revision = 0)
        {

            if (Options.WithTimeStamps) jObject["_date"] = DateTime.Now;
            if (id == "") id = Guid.NewGuid().ToString().Replace("-", "").ToUpper();

            if (revision != 0)
            {
                id = id + "-" + revision.ToString();
                jObject["_rev"] = revision;
            }

            jObject["_id"] = id;

            return jObject;
        }
        public string Get(string id, int revision = 0)
        {
            if (revision != 0) id = id + "-" + revision.ToString();
            if (Dictionary.ContainsKey(id)) return Dictionary[id];
            return null;
        }
        public T Get<T>(string id, int revision = 0)
        {
            var result = Get(id, revision);
            if (result == null) return default(T);

            return Deserialize<T>(result);

        }
        public string Set(string id, string jsonValue)
        {
            lock (Dictionary)
            {
                var json = Stamp(jsonValue, id);
                if (id == "") id = json["_id"].ToString();

                if (Options.WithRevisions && Dictionary.ContainsKey(id))
                {
                    var revision = 1;
                    while (Dictionary.ContainsKey(id + "-" + revision)) revision += 1;
                    Dictionary[id + "-" + revision] = Get(id);
                }
                Dictionary[id] = json.ToString();
            }
            return id;
        }
        public string Set(string jsonValue)
        {
            return Set("", jsonValue);
        }
        public string Set<T>(T value)
        {
            if (Options.DeserializeIdTo != "")
            {
                var jObject = JObject.FromObject(value);
                return Set(jObject[Options.DeserializeIdTo].ToString(), value);
            }
            return Set("", JsonConvert.SerializeObject(value));
        }
        public string Set<T>(string id, T value)
        {
            return Set(id, JsonConvert.SerializeObject(value));
        }
        public T Do<T>(string id, Func<T, T> func)
        {
            lock (Dictionary)
            {
                var originalValue = Get<T>(id);
                var newValue = func(originalValue);
                Set<T>(id, newValue);
                return newValue;
            }
        }


        public object GetAll()
        {
            throw new NotImplementedException();
        }

        public string All()
        {
            return "[" + String.Join(",",Dictionary.Where(t => !t.Key.Contains("-")).Select(t=>t.Value)) + "]";
        }
        public Dictionary<string, T> All<T>()
        {
            return Dictionary.Where(t => !t.Key.Contains("-")).ToDictionary(t => t.Key, t => Deserialize<T>(t.Value));
        }
        public void Dispose()
        {
            // 
        }

    }

}
