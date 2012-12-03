using Distrib.Plugins.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IDistribPlugin
    {
        void InitPlugin(IDistribPluginControllerInterface cont);
        void UninitPlugin(IDistribPluginControllerInterface cont);
    }
}
