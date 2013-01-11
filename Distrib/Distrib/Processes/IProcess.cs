using Distrib.Separation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distrib.Utils;

namespace Distrib.Processes
{
    /// <summary>
    /// Core interface for defining a Distrib-enabled process
    /// </summary>
    public interface IProcess
    {
        void InitProcess();
        void UninitProcess();
        IJobDefinition JobDefinition { get; }
    }

    public interface IJobDefinition
    {
        IReadOnlyList<ProcessJobField> InputFields { get; }
        IReadOnlyList<ProcessJobField> OutputFields { get; }
    }

    [Serializable()]
    public class ProcessJobDefinition : IJobDefinition
    {
        private readonly Type _inputInterfaceType;
        private readonly Type _outputInterfaceType;

        private List<ProcessJobField> _inputFields =
            new List<ProcessJobField>();

        private List<ProcessJobField> _outputFields =
            new List<ProcessJobField>();

        public ProcessJobDefinition(Type inputInterfaceType, Type outputInterfaceType)
        {
            if (inputInterfaceType == null) throw new ArgumentNullException("input interface type required");
            if (outputInterfaceType == null) throw new ArgumentNullException("output interface type required");

            if (!inputInterfaceType.IsInterface) throw new ArgumentException("input interface type must be an interface");
            if (!outputInterfaceType.IsInterface) throw new ArgumentException("output interface type must be an interface");

            _inputInterfaceType = inputInterfaceType;
            _outputInterfaceType = outputInterfaceType;
        }

        protected void AddField(ProcessJobField field)
        {
            switch (field.Mode)
            {
                case ProcessJobFieldMode.Input:
                    _inputFields.Add(field);
                    break;
                case ProcessJobFieldMode.Output:
                    _outputFields.Add(field);
                    break;
                default:
                    throw new Exception();
            }
        }

        public IReadOnlyList<ProcessJobField> InputFields
        {
            get
            {
                return _inputFields.AsReadOnly();
            }
        }

        public IReadOnlyList<ProcessJobField> OutputFields
        {
            get
            {
                return _outputFields.AsReadOnly();
            }
        }
    }

    [Serializable()]
    public class ProcessJobDefinition<TInput, TOutput> : ProcessJobDefinition
        where TInput : class
        where TOutput : class
    {
        public ProcessJobDefinition()
            : base(typeof(TInput), typeof(TOutput))
        {
        }

        public void RegInput<TProp>(Expression<Func<TInput, TProp>> expr)
        {

        }

        public ProcessJobFieldConfig<TProp> ConfigInput<TProp>(Expression<Func<TInput, TProp>> expr)
        {
            var pi = expr.GetPropertyInfo();
            var field = base.InputFields
                .SingleOrDefault(f => f.Name.Equals(pi.Name) && f.Type.Equals(pi.PropertyType));
            if (field == null)
            {
                // Assume not been created yet
#warning be sure to implement checks to ensure the pointed property actually has a getter & setter

                var nf = new ProcessJobField<TProp>(pi.Name, ProcessJobFieldMode.Input);
                base.AddField(nf);
                return nf.Config as ProcessJobFieldConfig<TProp>;
            }

            return field.Config as ProcessJobFieldConfig<TProp>;
        }

        //public ProcessJobFieldConfig<TProp> ConfigInput<TProp>(Expression<Func<TInput, TProp>> expr)
        //{
        //    if (expr == null) throw new ArgumentNullException("Property expression must be supplied");

        //    var pi = expr.GetPropertyInfo();
        //    var field = base.InputFields.DefaultIfEmpty(null)
        //        .SingleOrDefault(f => f.Name.Equals(pi.Name) && f.Type.Equals(pi.PropertyType));
        //    if (field == null)
        //    {
        //        throw new InvalidOperationException("Couldn't find the input field for property");
        //    }

        //    return (ProcessJobFieldConfig<TProp>)field.Config;
        //}

        public ProcessJobFieldConfig ConfigOutput<TProp>(Expression<Func<TOutput, TProp>> expr)
        {
            if (expr == null) throw new ArgumentNullException("Property expression must be supplied");

            var pi = expr.GetPropertyInfo();
            var field = base.OutputFields.DefaultIfEmpty(null)
                .SingleOrDefault(f => f.Name.Equals(pi.Name) && f.Type.Equals(pi.PropertyType));
            if (field == null)
            {
                throw new InvalidOperationException("Couldn't find the output field for property");
            }

            return field.Config;
        }
    }

    [Serializable()]
    public sealed class ProcessJobField<T> : ProcessJobField
    {
        public ProcessJobField(string fieldName, ProcessJobFieldMode fieldMode)
            : base(fieldName, typeof(T), fieldMode)
        {
            base.Config = new ProcessJobFieldConfig<T>();
        }
    }

    [Serializable()]
    public class ProcessJobField
    {
        private readonly Type _fieldType;
        private readonly string _fieldName;
        private readonly ProcessJobFieldMode _fieldMode = ProcessJobFieldMode.Input;

        private WriteOnce<ProcessJobFieldConfig> _fieldConfig = new WriteOnce<ProcessJobFieldConfig>();

        public ProcessJobField(string fieldName, Type fieldType, ProcessJobFieldMode fieldMode)
        {
            if (fieldType == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentNullException();

            if (!fieldType.IsClass && !fieldType.IsValueType && !fieldType.IsSerializable)
            {
                throw new ArgumentException("Field type must be either a class / value type and must be serializable");
            }

            _fieldName = fieldName;
            _fieldType = fieldType;
            _fieldMode = fieldMode;
        }

        public string Name
        {
            get
            {
                return _fieldName;
            }
        }

        public Type Type
        {
            get
            {
                return _fieldType;
            }
        }

        public ProcessJobFieldMode Mode
        {
            get
            {
                return _fieldMode;
            }
        }

        public ProcessJobFieldConfig Config
        {
            get
            {
                return _fieldConfig.Value;
            }

            protected set
            {
                _fieldConfig.Value = value;
            }
        }
    }

    [Serializable()]
    public class ProcessJobFieldConfig
    {
        private readonly object _lock = new object();

        private object _defaultValue;
        public virtual object DefaultValue
        {
            get
            {
                lock (_lock)
                    return _defaultValue;
            }

            set
            {
                lock (_lock)
                    _defaultValue = value;
            }
        }
    }

    [Serializable()]
    public class ProcessJobFieldConfig<T> : ProcessJobFieldConfig
    {
        public new T DefaultValue
        {
            get { return (T)base.DefaultValue; }
            set { base.DefaultValue = value; }
        }
    }

    public enum ProcessJobFieldMode
    {
        Input,
        Output
    }
}
