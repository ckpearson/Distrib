using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Persistence
{
    [Serializable()]
    public sealed class PersistenceDataBag : IPersistenceDataBag
    {
        [NonSerialized()]
        private ConcurrentDictionary<string, object> _dict =
            new ConcurrentDictionary<string, object>();

        public void AddData(string key, object value)
        {
            try
            {
                if (!Attribute.IsDefined(value.GetType(), typeof(SerializableAttribute)))
                {
                    throw new InvalidOperationException("Value must be of a serialisable type");
                }

                if (!_dict.TryAdd(key, value))
                {
                    throw new ApplicationException("Failed to add item to dictionary");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add data to bag", ex);
            }
        }

        public object GetData(string key)
        {
            object value = null;

            try
            {
                if (!_dict.TryGetValue(key, out value))
                {
                    throw new ApplicationException("Failed to get item from dictionary");
                }

                return key;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get data from bag", ex);
            }
        }

        public bool HasData(string key)
        {
            try
            {
                return _dict.ContainsKey(key);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public bool TryGetData(string key, out object data)
        {
            try
            {
                return _dict.TryGetValue(key, out data);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to try to get data from bag", ex);
            }
        }


        public IEnumerable<KVP> GetEntries()
        {
            lock (_dict)
            {
                foreach (var key in _dict.Keys)
                {
                    object value = null;
                    _dict.TryGetValue(key, out value);
                    yield return new KVP()
                    {
                        Key = key,
                        Value = value,
                    };
                }
            }
        }
    }
}
