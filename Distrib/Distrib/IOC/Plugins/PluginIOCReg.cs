using Distrib.IOC.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC.Plugins
{
    /// <summary>
    /// Performs the IOC registration for the plugin types
    /// </summary>
    public sealed class PluginIOCReg : IOCReg<PluginIOCReg>, IIOCRegistrationModule
    {
        public void PerformBindings(Action<Type, Type> bindAction)
        {
            
        }
    }
}
