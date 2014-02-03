using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FnX
{
    public class EsentStore<T> {

        private string _physicalName;

        public EsentStore(string physicalName)
        {
            this._physicalName = physicalName;
        }

        public EsentObject<T> Get(string key)
        {
            return new EsentObject<T>(_physicalName, key);
        }
        public EsentObject<T> Set(string key, T value)
        {
            var obj = new EsentObject<T>(_physicalName, key);
            obj.Value = value;
            return obj;
        }

    }

    public class EsentStore
    {

        private string _physicalName;

        public EsentStore(string physicalName="")
        {            
            if (physicalName=="") physicalName="___DefaultStore";
            this._physicalName = physicalName;
        }

        public EsentObject<T> Get<T>(string key)
        {
            return new EsentObject<T>(_physicalName, key);
        }
        public EsentObject<T> Set<T>(string key, T value)
        {
            var obj = new EsentObject<T>(_physicalName, key);
            obj.Value = value;
            return obj;
        }

    }

    public class EsentObject
    {        
        public static EsentObject<T> Get<T>()
        {
            return new EsentObject<T>(typeof(T).Name);
        }

        public static EsentObject<T> Get<T>(string key)
        {
            return new EsentObject<T>(key);
        }
        public static EsentObject<T> Set<T>(string key, T value)
        {
            var n = new EsentObject<T>(key);
            n.Value = value;
            return n;
        }
        public static EsentObject<T> Set<T>(string key, Func<T, T> action)
        {
            var n = new EsentObject<T>(key);
            n.Do(action);
            return n;
        }
        public static EsentObject<T> Set<T>(Func<T, T> action)
        {
            var n = new EsentObject<T>();
            n.Do(action);
            return n;
        }

    }

    public class EsentObject<T>
    {
        private string _physicalName;
        private string _key;

        public EsentObject()
        {
            _physicalName = typeof(T).Name;
            _key = "";
        }
        public EsentObject(string key)
        {
            _physicalName = typeof(T).Name;
            _key = key;
        }
        public EsentObject(string name, string key)
        {
            _physicalName = name;
            _key = key;
        }
        public T Value
        {
            get
            {
                return EsentKeyValue.Get<T>(_physicalName, _key);
            }
            set
            {
                EsentKeyValue.Set<T>(_physicalName, _key, value);
            }
        }


        public EsentObject<T> Do(Func<T, T> action)
        {
            var originalValue = EsentKeyValue.Get<T>(_physicalName, _key);
            var newValue = action(originalValue);
            EsentKeyValue.Set(_physicalName, _key, newValue);
            return this;
        }
        public EsentObject<T> Do(Action<T> action)
        {
            var obj = EsentKeyValue.Get<T>(_physicalName, _key);
            action(obj);
            EsentKeyValue.Set(_physicalName, _key, obj);
            return this;
        }

    }
}
