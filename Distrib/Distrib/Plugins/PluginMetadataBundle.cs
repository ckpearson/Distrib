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
        private readonly Type _interface;
        private readonly object _instance;
        private readonly IReadOnlyDictionary<string, object> _kvps;

        private readonly string _identity = Guid.NewGuid().ToString();
        private readonly PluginMetadataBundleExistencePolicy _existencePolicy =
            PluginMetadataBundleExistencePolicy.NotImportant;

        public PluginMetadataBundle(Type interfaceType,
            object instance,
            IReadOnlyDictionary<string, object> kvps,
            string identity,
            PluginMetadataBundleExistencePolicy existencePolicy)
        {
            _interface = interfaceType;
            _instance = instance;
            _kvps = kvps;
            _identity = identity;
            _existencePolicy = existencePolicy;
        }

        public T GetMetadataInstance<T>()
        {
            return (T)_instance;
        }

        public object GetMetadataInstance()
        {
            return _instance;
        }

        public IReadOnlyDictionary<string, object> MetadataKVPs
        {
            get { return _kvps; }
        }

        public string MetadataInstanceIdentity
        {
            get { return _identity; }
        }

        public PluginMetadataBundleExistencePolicy MetadataInstanceExistencePolicy
        {
            get { return _existencePolicy; }
        }
    }
}
