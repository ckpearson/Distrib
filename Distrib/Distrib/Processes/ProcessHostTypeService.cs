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

namespace Distrib.Processes
{
    public sealed class ProcessHostTypeService : CrossAppDomainObject, IProcessHostTypeService
    {
        public bool IsTypePowered(IProcessHost host)
        {
            if (host == null) throw Ex.ArgNull(() => host);

            return (host is ITypePoweredProcessHost);
        }

        public bool IsPluginPowered(IProcessHost host)
        {
            if (host == null) throw Ex.ArgNull(() => host);

            return (host is IPluginPoweredProcessHost);
        }

        public ITypePoweredProcessHost GetTypePoweredInterface(IProcessHost host)
        {
            if (host == null) throw Ex.ArgNull(() => host);

            if (!IsTypePowered(host))
            {
                throw Ex.Arg(() => host, "Host isn't type powered");
            }

            return (ITypePoweredProcessHost)host;
        }

        public IPluginPoweredProcessHost GetPluginPoweredInterface(IProcessHost host)
        {
            if (host == null) throw Ex.ArgNull(() => host);

            if (!IsPluginPowered(host))
            {
                throw Ex.Arg(() => host, "Host isn't plugin powered");
            }

            return (IPluginPoweredProcessHost)host;
        }


        public SystemPowerType GetPowerType(IProcessHost host)
        {
            if (host == null) throw Ex.ArgNull(() => host);

            if (IsPluginPowered(host))
            {
                return SystemPowerType.Plugin;
            }
            else if (IsTypePowered(host))
            {
                return SystemPowerType.Type;
            }
            else
            {
                throw Ex.Arg(() => host, "Host is of unknown power type");
            }
        }
    }
}
