﻿/*
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
    public sealed class PluginDescriptorFactory : CrossAppDomainObject, IPluginDescriptorFactory
    {
        private readonly IIOC _ioc;

        public PluginDescriptorFactory(IIOC ioc)
        {
            _ioc = ioc;
        }

        public IPluginDescriptor GetDescriptor(string typeFullName, IPluginMetadata pluginMetadata, 
            string assemblyPath)
        {
            return _ioc.Get<IPluginDescriptor>(new[]
            {
                new IOCConstructorArgument("typeFullName", typeFullName),
                new IOCConstructorArgument("metadata", pluginMetadata),
                new IOCConstructorArgument("assemblyPath", assemblyPath),
            });
        }
    }
}
