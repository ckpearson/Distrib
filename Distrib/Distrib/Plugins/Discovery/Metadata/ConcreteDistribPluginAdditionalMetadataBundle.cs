using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery.Metadata
{
    [Serializable()]
    internal sealed class ConcreteDistribPluginAdditionalMetadataBundle
        : IDistribPluginAdditionalMetadataBundle
    {
        private readonly Type m_type = null;
        private readonly object m_metadataObject = null;
        private readonly Type m_attrType = null;
        private readonly Dictionary<string, object> m_dictKVP = new Dictionary<string, object>();

        internal ConcreteDistribPluginAdditionalMetadataBundle(Type type,
            object metadataObject, Type attributeType, Dictionary<string, object> kvps)
        {
            m_type = type;
            m_metadataObject = metadataObject;
            m_attrType = attributeType;
            m_dictKVP = kvps;
        }

        public T GetMetadataObject<T>()
        {
            return (T)m_metadataObject;
        }

        public object GetMetadataObject()
        {
            return m_metadataObject;
        }

        public Type AdditionalMetadataAttributeType
        {
            get { return m_attrType; }
        }

        public Dictionary<string, object> MetadataKVPs
        {
            get { return m_dictKVP; }
        }
    }
}
