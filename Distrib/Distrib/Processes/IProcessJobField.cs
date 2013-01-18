using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IProcessJobField
    {
        Type Type { get; }
        string Name { get; }
        FieldMode Mode { get; }
        object Value { get; set; }

        IProcessJobFieldConfig Config { get; }
    }

    public interface IProcessJobField<T> : IProcessJobField
    {
        new T Value { get; set; }
        new IProcessJobFieldConfig<T> Config { get; }
    }
}
