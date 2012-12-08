using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    [Serializable()]
    public sealed class PluginMetadataBundle : IPluginMetadataBundle
    {
        private readonly Type _metadataInterface;
        private readonly object _metadataInstance;
        private readonly Type _attributeType;
        private readonly IReadOnlyDictionary<string, object> _kvps;

        private readonly string _identity = Guid.NewGuid().ToString();
        private readonly PluginMetadataBundleExistencePolicy _existencePolicy =
            PluginMetadataBundleExistencePolicy.NotImportant;

        public PluginMetadataBundle(Type interfaceType,
            Type attributeType,
            object instance,
            IReadOnlyDictionary<string, object> kvps,
            string identity,
            PluginMetadataBundleExistencePolicy existencePolicy)
        {
            _metadataInterface = interfaceType;
            _attributeType = attributeType;
            _metadataInstance = instance;
            _kvps = kvps;
            _identity = identity;
            _existencePolicy = existencePolicy;
        }

        public Type AdditionalMetadataAttributeType
        {
            get { throw new NotImplementedException(); }
        }

        public T GetMetadataInstance<T>()
        {
            throw new NotImplementedException();
        }

        public object GetMetadataInstance()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> MetadataKVPs
        {
            get { throw new NotImplementedException(); }
        }

        public string MetadataInstanceIdentity
        {
            get { throw new NotImplementedException(); }
        }

        public PluginMetadataBundleExistencePolicy MetadataInstanceExistencePolicy
        {
            get { throw new NotImplementedException(); }
        }
    }
}
