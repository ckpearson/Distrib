using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Persistence
{
    public interface IPersistenceDataBag
    {
        void AddData(string key, object value);
        object GetData(string key);
        bool HasData(string key);
        bool TryGetData(string key, out object data);

        IEnumerable<KVP> GetEntries();
    }

    public sealed class KVP
    {
        [System.Xml.Serialization.XmlAttribute(AttributeName = "k")]
        public string Key { get; set; }
        [System.Xml.Serialization.XmlElement(ElementName = "v")]
        public object Value { get; set; }
    }
}
