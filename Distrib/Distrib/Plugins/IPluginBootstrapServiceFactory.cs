using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginBootstrapServiceFactory
    {
        IPluginBootstrapService CreateService();
    }
}
