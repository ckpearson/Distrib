/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    [Serializable()]
    [System.Diagnostics.DebuggerDisplay("Metadata Bundle of: {_instance.GetType().Name}, ident: {_identity}, pol: {_existencePolicy}")]
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

        public string MetadataBundleIdentity
        {
            get { return _identity; }
        }

        public PluginMetadataBundleExistencePolicy MetadataInstanceExistencePolicy
        {
            get { return _existencePolicy; }
        }
    }
}
