using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{

    [Serializable()]
    internal class ProcessJobFieldDefinition : IProcessJobDefinitionField
    {
        private readonly object _lock = new object();

        private readonly Type _type;
        private readonly string _name;
        private readonly FieldMode _mode;

        private IProcessJobFieldConfig _config = ProcessJobFieldConfigFactory.CreateConfig();

        public ProcessJobFieldDefinition(Type type, string name, FieldMode mode)
        {
            if (type == null) throw Ex.ArgNull(() => type);
            if (string.IsNullOrEmpty(name)) throw Ex.ArgNull(() => name);

            _type = type;
            _name = name;
            _mode = mode;
        }

        public Type Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public FieldMode Mode
        {
            get { return _mode; }
        }

        public IProcessJobFieldConfig Config
        {
            get
            {
                lock (_lock)
                {
                    return _config;
                }
            }
            protected set
            {
                lock (_lock)
                {
                    _config = value;
                }
            }
        }

        public string DisplayName
        {
            get
            {
                lock (_lock)
                {
                    if (_config.HasDisplayName)
                    {
                        return _config.DisplayName;
                    }
                    else
                    {
                        return _name;
                    }
                }
            }
        }
    }

    [Serializable()]
    internal sealed class ProcessJobFieldDefinition<T> : ProcessJobFieldDefinition, IProcessJobDefinitionField<T>
    {
        public ProcessJobFieldDefinition(string name, FieldMode mode)
            : base(typeof(T), name, mode)
        {
            base.Config = ProcessJobFieldConfigFactory.CreateConfig<T>();
        }

        public new IProcessJobFieldConfig<T> Config
        {
            get { return (IProcessJobFieldConfig<T>)base.Config; }
        }
    }
}
