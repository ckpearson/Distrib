using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IDeferredValueProvider
    {
        object RetrieveValue();
    }

    public interface IDeferredValueProvider<T> : IDeferredValueProvider
    {
        new T RetrieveValue();
    }
}
