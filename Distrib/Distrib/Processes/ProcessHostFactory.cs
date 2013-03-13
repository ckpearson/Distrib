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
using Distrib.Plugins;
using Distrib.Separation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Default process host factory
    /// </summary>
    public sealed class ProcessHostFactory : IProcessHostFactory
    {
        private readonly IIOC _ioc;
        private readonly ISeparateInstanceCreatorFactory _instFactory;

        public ProcessHostFactory(IIOC ioc, ISeparateInstanceCreatorFactory instFactory)
        {
            _ioc = ioc;
            _instFactory = instFactory;
        }

        public IProcessHost CreateHostFromPlugin(IPluginDescriptor descriptor)
        {
            return (IProcessHost)_instFactory.CreateCreator()
                .CreateInstanceWithSeparation(_ioc.Get<IPluginPoweredProcessHost>(new[]
                {
                    new IOCConstructorArgument("descriptor", descriptor),
                }).GetType(), new[]
                {
                    new IOCConstructorArgument("descriptor", descriptor),
                });
        }

        public IProcessHost CreateHostFromType(Type type)
        {
            // Need to create the instance and have the assembly the type lives in loaded into the domain
            return (IProcessHost)_instFactory.CreateCreator()
                .CreateInstanceSeparatedWithLoadedAssembly(_ioc.Get<ITypePoweredProcessHost>(new[]
                {
                    new IOCConstructorArgument("instanceType", type),
                }).GetType(), type.Assembly.Location,
                new[]
                {
                    new IOCConstructorArgument("instanceType", type),
                });
        }
    }
}
