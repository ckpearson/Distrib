using Distrib.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginDescriptorPersistor : IPluginDescriptorPersistor
    {
        private readonly IPersistenceDataBagFactory _bagFactory;

        public PluginDescriptorPersistor(IPersistenceDataBagFactory bagFactory)
        {
            _bagFactory = bagFactory;
        }

        public IPersistenceDataBag Save(IPluginDescriptor descriptor)
        {
            try
            {
                var bag = _bagFactory.CreateDataBag();

                ((IPersistable)descriptor).Persist(bag);

                return bag;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to save descriptor to bag", ex);
            }
        }

        public IPluginDescriptor Load(IPersistenceDataBag bag)
        {
            return null;
        }
    }
}
