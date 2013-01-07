using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Persistence
{
    public sealed class PersistenceDataBagFactory : IPersistenceDataBagFactory
    {
        private readonly IKernel _kernel;

        public PersistenceDataBagFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPersistenceDataBag CreateDataBag()
        {
            return _kernel.Get<IPersistenceDataBag>();
        }

        public IPersistenceDataBag CreateDataBagFromEntries(IEnumerable<KVP> kvps)
        {
            var bag = CreateDataBag();
            foreach (var kvp in kvps)
            {
                bag.AddData(kvp.Key, kvp.Value);
            }
            return bag;
        }
    }
}
