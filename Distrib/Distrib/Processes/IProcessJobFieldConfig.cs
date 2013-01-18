using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IProcessJobFieldConfig
    {
        object DefaultValue { get; set; }
        bool HasDefaultValue { get; }

        IDeferredValueProvider DeferredValueProvider { get; set; }
        bool HasDeferredValueProvider { get; }
    }

    public interface IProcessJobFieldConfig<T> : IProcessJobFieldConfig
    {
        new T DefaultValue { get; set; }
        new IDeferredValueProvider<T> DeferredValueProvider { get; set; }
    }

    internal interface IProcessJobFieldConfig_Internal : IProcessJobFieldConfig
    {
        void Adopt(IProcessJobFieldConfig config);
    }
}
