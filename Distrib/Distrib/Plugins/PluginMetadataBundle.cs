/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
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
