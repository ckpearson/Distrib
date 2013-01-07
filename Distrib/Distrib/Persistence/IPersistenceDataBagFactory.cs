using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Persistence
{
    public interface IPersistenceDataBagFactory
    {
        IPersistenceDataBag CreateDataBag();
        IPersistenceDataBag CreateDataBagFromEntries(IEnumerable<KVP> kvps);
    }
}
