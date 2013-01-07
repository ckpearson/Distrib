using Distrib.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginDescriptorPersistor
    {
        IPersistenceDataBag Save(IPluginDescriptor descriptor);
        IPluginDescriptor Load(IPersistenceDataBag bag);
    }
}
