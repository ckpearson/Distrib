using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery.Metadata
{
    /// <summary>
    /// Concrete implementation for plugin additional metadata provided through
    /// abstraction of <see cref="IDistribPluginAdditionalMetadataBundle"/>
    /// </summary>
    [Serializable()]
    internal sealed class ConcreteDistribPluginAdditionalMetadataBundle
        : IDistribPluginAdditionalMetadataBundle
    {
        private readonly Type m_typMetadataInterface = null;
        private readonly object m_objMetadataInstance = null;
        private readonly Type m_typAttributeType = null;
        private readonly Dictionary<string, object> m_dictKVP = new Dictionary<string, object>();

        private readonly string m_strIdentity = Guid.NewGuid().ToString();
        private readonly AdditionalPluginMetadataIdentityExistencePolicy m_enumExistencePolicy = AdditionalPluginMetadataIdentityExistencePolicy.NotImportant;

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="interfaceType">The type of the interface used for metadata access</param>
        /// <param name="attributeType">The type of the attribute that provided the metadata</param>
        /// <param name="metadataObject">The underlying object holding the metadata</param>
        /// <param name="kvps">The key-value pairs for the metadata</param>
        internal ConcreteDistribPluginAdditionalMetadataBundle(Type interfaceType, 
            Type attributeType, object metadataObject, Dictionary<string, object> kvps, string identity, AdditionalPluginMetadataIdentityExistencePolicy existencePolicy)
        {
            m_typMetadataInterface = interfaceType;
            m_objMetadataInstance = metadataObject;
            m_typAttributeType = attributeType;
            m_dictKVP = kvps;
            m_strIdentity = identity;
            m_enumExistencePolicy = existencePolicy;
        }

        /// <summary>
        /// Gets the instance holding the metadata cast to a provided type.
        /// </summary>
        /// <typeparam name="T">The type to cast the metadata instance to</typeparam>
        /// <returns>The cast result</returns>
        public T GetMetadataInstance<T>()
        {
            return (T)m_objMetadataInstance;
        }

        /// <summary>
        /// Gets the instance holding the metadata
        /// </summary>
        /// <returns>The metadata instance</returns>
        public object GetMetadataInstance()
        {
            return m_objMetadataInstance;
        }

        /// <summary>
        /// Gets the type for the attribute that provided the metadata
        /// </summary>
        public Type AdditionalMetadataAttributeType
        {
            get { return m_typAttributeType; }
        }

        /// <summary>
        /// Gets the metadata key-value pairs
        /// </summary>
        public Dictionary<string, object> MetadataKVPs
        {
            get { return m_dictKVP; }
        }

        /// <summary>
        /// Gets the identity tag associated with this type of additional metadata
        /// </summary>
        public string MetadataInstanceIdentity
        {
            get { return m_strIdentity; }
        }

        /// <summary>
        /// Gets the instance existence policy for this bundle of metadata (and at large its identity group)
        /// </summary>
        public AdditionalPluginMetadataIdentityExistencePolicy MetadataInstanceExistencePolicy
        {
            get { return m_enumExistencePolicy; }
        }
    }
}
