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
    public sealed class PluginAssemblyManagerFactory : IPluginAssemblyManagerFactory
    {
        private readonly IIOC _ioc;

        public PluginAssemblyManagerFactory(IIOC ioc)
        {
            _ioc = ioc;
        }

        public IPluginAssemblyManager CreateManagerForAssembly(string assemblyPath)
        {
            return _ioc.Get<IPluginAssemblyManager>(new[]
            {
                new IOCConstructorArgument("assemblyPath", assemblyPath),
            });
        }

        public IPluginAssemblyManager CreateManagerForAssemblyInGivenDomain(string assemblyPath, AppDomain domain)
        {
            var t = this.CreateManagerForAssembly(assemblyPath).GetType();

            return (IPluginAssemblyManager)domain.CreateInstanceAndUnwrap(
                t.Assembly.FullName,
                t.FullName,
                true,
                System.Reflection.BindingFlags.CreateInstance,
                null,
                new object[] { _ioc.Get<IPluginDescriptorFactory>(), _ioc.Get<IPluginMetadataFactory>(),
                    _ioc.Get<IPluginMetadataBundleFactory>(), assemblyPath },
                null,
                null);
        }
    }
}
