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
