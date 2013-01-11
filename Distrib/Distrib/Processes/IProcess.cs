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
        private readonly Type _inputType;
        private readonly Type _outputType;

        private readonly List<ProcessJobField> _fields =
            new List<ProcessJobField>();

        public ProcessJobDefinition(Type inputInterfaceType, Type outputInterfaceType)
        {
            _inputType = inputInterfaceType;
            _outputType = outputInterfaceType;

            _buildInitialFields();
        }

        private void _buildInitialFields()
        {
            var fields = (_inputType.GetProperties()
                .Where(p => p.HasGetterAndSetter() && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable)
                .Select(p => new ProcessJobField(p.PropertyType, p.Name, FieldMode.Input))
            .Concat(
                _outputType.GetProperties()
                    .Where(p => p.HasGetterAndSetter() && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable)
                    .Select(p => new ProcessJobField(p.PropertyType, p.Name, FieldMode.Output)))).ToList();

            if (fields == null || fields.Count == 0)
            {
                throw new InvalidOperationException("No fields could be found on either the input or output types");
            }

            _fields.Clear();
            _fields.AddRange(fields);
        }

        public IReadOnlyList<ProcessJobField> InputFields
        {
            get
            {
                return _fields.Where(f => f.Mode == FieldMode.Input).ToList().AsReadOnly();
            }
        }

        public IReadOnlyList<ProcessJobField> OutputFields
        {
            get { return _fields.Where(f => f.Mode == FieldMode.Output).ToList().AsReadOnly(); }
        }

        protected ProcessJobField GetField(PropertyInfo pi)
        {
            return _fields.SingleOrDefault(f => f.Name == pi.Name && f.Type.Equals(pi.PropertyType));
        }

        protected ProcessJobField ReplaceField(ProcessJobField field, ProcessJobField replacement)
        {
            _fields[_fields.IndexOf(field)] = replacement;
            return replacement;
        }
    }

    [Serializable()]
    public sealed class ProcessJobDefinition<TInput, TOutput> : ProcessJobDefinition
    {
        public ProcessJobDefinition()
            : base(typeof(TInput), typeof(TOutput))
        {

        }

        public ProcessJobFieldConfig<TProp> ConfigInput<TProp>(Expression<Func<TInput, TProp>> expr)
        {
            var pi = expr.GetPropertyInfo();
            var field = GetField(pi);
            if (!field.GetType().ContainsGenericParameters)
            {
                // The field isn't the strongly-typed generic version, so it's an initial field
                // given we'd only be here if in the first place it was a generic definition
                // we can replace the non-generic field with a correct generic one (because we know the property type here)
                // Preserve the original config though, in case there are any values configured (they may have cast to the non-generic type
                // and done so then come back here)
                var origConfig = field.Config;
                field = base.ReplaceField(field, new ProcessJobField<TProp>(field.Name, field.Mode));
                // At this point the config will be empty of any values that may have existed before turning it generic
                // add those back
                field.Config.Adopt(origConfig);
            }

            return field.Config as ProcessJobFieldConfig<TProp>;
        }

        public ProcessJobFieldConfig<TProp> ConfigOutput<TProp>(Expression<Func<TOutput, TProp>> expr)
        {
            var pi = expr.GetPropertyInfo();
            var field = GetField(pi);
            if (!field.GetType().ContainsGenericParameters)
            {
                // The field isn't the strongly-typed generic version, so it's an initial field
                // given we'd only be here if in the first place it was a generic definition
                // we can replace the non-generic field with a correct generic one (because we know the property type here)
                // Preserve the original config though, in case there are any values configured (they may have cast to the non-generic type
                // and done so then come back here)
                var origConfig = field.Config;
                field = base.ReplaceField(field, new ProcessJobField<TProp>(field.Name, field.Mode));
                // At this point the config will be empty of any values that may have existed before turning it generic
                // add those back
                field.Config.Adopt(origConfig);
            }

            return field.Config as ProcessJobFieldConfig<TProp>;
        }
    }

    [Serializable()]
    public class ProcessJobField
    {
        private readonly object _lock = new object();

        private readonly Type _fieldType;
        private readonly string _fieldName;
        private readonly FieldMode _fieldMode;

        private ProcessJobFieldConfig _fieldConfig = new ProcessJobFieldConfig();

        public ProcessJobField(Type fieldType, string fieldName, FieldMode fieldMode)
        {
            _fieldType = fieldType;
            _fieldName = fieldName;
            _fieldMode = fieldMode;
        }

        public Type Type
        {
            get { return _fieldType; }
        }

        public string Name
        {
            get { return _fieldName; }
        }

        public FieldMode Mode
        {
            get { return _fieldMode; }
        }

        public ProcessJobFieldConfig Config
        {
            get { return _fieldConfig; }
            protected set
            {
                lock (_lock)
                {
                    _fieldConfig = value;
                }
            }
        }
    }

    [Serializable()]
    public sealed class ProcessJobField<T> : ProcessJobField
    {
        public ProcessJobField(string fieldName, FieldMode fieldMode)
            : base(typeof(T), fieldName, fieldMode)
        {
            base.Config = new ProcessJobFieldConfig<T>();
        }
    }

    [Serializable()]
    public class ProcessJobFieldConfig
    {
        protected readonly object _lock = new object();

        private object _defaultValue;
        public object DefaultValue
        {
            get
            {
                lock (_lock)
                {
                    return _defaultValue;
                }
            }
            set
            {
                lock (_lock)
                {
                    _defaultValue = value;
                }
            }
        }

        internal void Adopt(ProcessJobFieldConfig config)
        {
            lock (_lock)
            {
                foreach (var pi in config.GetType().GetProperties())
                {
                    pi.SetValue(this, pi.GetValue(config));
                }
            }
        }
    }

    [Serializable()]
    public sealed class ProcessJobFieldConfig<T> : ProcessJobFieldConfig
    {
        public new T DefaultValue
        {
            get { return (T)base.DefaultValue; }
            set { base.DefaultValue = value; }
        }
    }

    public enum FieldMode
    {
        Input,
        Output,
    }
}
