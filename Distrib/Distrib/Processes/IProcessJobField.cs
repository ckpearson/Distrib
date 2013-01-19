using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IProcessJobDefinitionField
    {
        Type Type { get; }
        string Name { get; }
        FieldMode Mode { get; }

        string DisplayName { get; }

        IProcessJobFieldConfig Config { get; }
    }

    public interface IProcessJobDefinitionField<T> : IProcessJobDefinitionField
    {
        new IProcessJobFieldConfig<T> Config { get; }
    }

    public interface IProcessJobValueField
    {
        IProcessJobDefinitionField Definition { get; }
        object Value { get; set; }
    }

    public interface IProcessJobValueField<T> : IProcessJobValueField
    {
        new IProcessJobDefinitionField<T> Definition { get; }
        new T Value { get; set; }
    }
}
