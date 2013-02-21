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
using Distrib.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginMetadataBundleFactory : CrossAppDomainObject, IPluginMetadataBundleFactory
    {
        private IIOC _ioc;

        public PluginMetadataBundleFactory(IIOC ioc)
        {
            _ioc = ioc;
        }

        public IPluginMetadataBundle CreateBundle(Type interfaceType,
            object instance, 
            IReadOnlyDictionary<string, object> kvps, 
            string identity, 
            PluginMetadataBundleExistencePolicy existencePolicy)
        {
            return _ioc.Get<IPluginMetadataBundle>(new[]
            {
                new IOCConstructorArgument("interfaceType", interfaceType),
                new IOCConstructorArgument("instance", instance),
                new IOCConstructorArgument("kvps", kvps),
                new IOCConstructorArgument("identity", identity),
                new IOCConstructorArgument("existencePolicy", existencePolicy),
            });
        }


        public IPluginMetadataBundle CreateBundleFromAdditionalMetadataObject(PluginAdditionalMetadataObject additionalMetadataObject)
        {
            return CreateBundle(
                additionalMetadataObject.MetadataInterfaceType,
                additionalMetadataObject.ProvideMetadataInstance(),
                additionalMetadataObject.ProvideMetadataKVPs(),
                additionalMetadataObject.MetadataIdentity,
                additionalMetadataObject.MetadataExistencePolicy);
        }
    }
}
