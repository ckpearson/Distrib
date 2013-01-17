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
using System.Runtime.CompilerServices;

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

        void ProcessJob(IJob job);
    }

    public interface IJob_Internal : IJob
    {
        List<ProcessJobField> InputValueFields { get; }
        List<ProcessJobField> OutputValueFields { get; }
        void SetInputValue(ProcessJobField defField, object value);
        void SetOutputValue(ProcessJobField defField, object value);
        IJobDefinition JobDefinition { get; }
    }

    public interface IJob
    {
        IJobInputTracker InputTracker { get; }
        IJobOutputTracker OutputTracker { get; }
    }

    public sealed class ProcessJob : CrossAppDomainObject, IJob, IJob_Internal
    {
        private readonly IJobInputTracker _inputTracker;
        private readonly IJobOutputTracker _outputTracker;
        private readonly IJobDefinition _jobDefinition;

        private readonly List<ProcessJobField> _inputValueFields = new List<ProcessJobField>();
        private readonly List<ProcessJobField> _outputValueFields = new List<ProcessJobField>();

        public ProcessJob(IJobInputTracker inputTracker, IJobOutputTracker outputTracker, IJobDefinition jobDefinition)
        {
            _inputTracker = inputTracker;
            _outputTracker = outputTracker;
            _jobDefinition = jobDefinition;
        }

        public IJobInputTracker InputTracker
        {
            get { return _inputTracker; }
        }

        List<ProcessJobField> IJob_Internal.InputValueFields
        {
            get
            {
                lock (_inputValueFields)
                {
                    return _inputValueFields; 
                }
            }
        }

        List<ProcessJobField> IJob_Internal.OutputValueFields
        {
            get
            {
                lock (_outputValueFields)
                {
                    return _outputValueFields;
                }
            }
        }


        public void SetInputValue(ProcessJobField defField, object value)
        {
            lock (_inputValueFields)
            {
                var valField = _inputValueFields.SingleOrDefault(f => f.Name == defField.Name);

                if (valField != null)
                {
                    valField.Value = value;
                }
                else
                {
                    var pf = new ProcessJobField(defField.Type, defField.Name, defField.Mode);
                    pf.Config.Adopt(defField.Config);
                    pf.Value = value;
                    _inputValueFields.Add(pf);
                } 
            }
        }

        public void SetOutputValue(ProcessJobField defField, object value)
        {
            lock (_outputValueFields)
            {
                var valField = _outputValueFields.SingleOrDefault(f => f.Name == defField.Name);

                if (valField != null)
                {
                    valField.Value = value;
                }
                else
                {
                    var pf = new ProcessJobField(defField.Type, defField.Name, defField.Mode);
                    pf.Config.Adopt(defField.Config);
                    pf.Value = value;
                    _outputValueFields.Add(pf);
                }
            }
        }


        public IJobOutputTracker OutputTracker
        {
            get { return _outputTracker; }
        }


        public IJobDefinition JobDefinition
        {
            get { return _jobDefinition; }
        }
    }

    public interface IJobInputTracker
    {
        T GetInput<T>(IJob forJob, [CallerMemberName] string prop = null);
    }

    public interface IJobOutputTracker
    {
        T GetOutput<T>(IJob forJob, [CallerMemberName] string prop = null);
        void SetOutput<T>(IJob forJob, T value, [CallerMemberName] string prop = null);
    }

    public interface IJobDefinition_Internal
    {
    }

    public interface IJobDefinition
    {
        IReadOnlyList<ProcessJobField> InputFields { get; }
        IReadOnlyList<ProcessJobField> OutputFields { get; }
        string Name { get; }
    }

    [Serializable()]
    public class ProcessJobDefinition : IJobDefinition, IJobDefinition_Internal
    {
        private readonly Type _inputInterfaceType;
        private readonly Type _outputInterfaceType;

        private readonly string _jobName;

        private readonly List<ProcessJobField> _fields =
            new List<ProcessJobField>();

        public ProcessJobDefinition(string jobName, Type inputInterfaceType, Type outputInterfaceType)
        {
            if (string.IsNullOrEmpty(jobName)) throw new ArgumentNullException();

            if (inputInterfaceType == null) throw new ArgumentNullException();
            if (!inputInterfaceType.IsInterface) throw new ArgumentException();

            if (outputInterfaceType == null) throw new ArgumentNullException();
            if (!outputInterfaceType.IsInterface) throw new ArgumentException();

            _jobName = jobName;
            _inputInterfaceType = inputInterfaceType;
            _outputInterfaceType = outputInterfaceType;

            _buildInitialFields();
        }

        private void _buildInitialFields()
        {
            // Input need only be getter on the interface, output needs to be getter & setter
            var fields = (_inputInterfaceType.GetProperties()
                .Where(p => p.CanRead && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable)
                .Select(p => new ProcessJobField(p.PropertyType, p.Name, FieldMode.Input))
            .Concat(
                _outputInterfaceType.GetProperties()
                    .Where(p => (p.CanRead && p.CanWrite) && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable)
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


        public string Name
        {
            get { return _jobName; }
        }
    }

    [Serializable()]
    public sealed class ProcessJobDefinition<TInput, TOutput> : ProcessJobDefinition
    {
        public ProcessJobDefinition(string jobName)
            : base(jobName, typeof(TInput), typeof(TOutput))
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

        private object _value;

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

        public object Value
        {
            get
            {
                lock(_lock)
                {
                    return _value;
                }
            }

            set
            {
               lock(_lock)
               {
                   _value = value;
               }
            }
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

        private TrackWritten<object> _defaultValue = new TrackWritten<object>(null);

        public object DefaultValue
        {
            get
            {
                lock (_lock)
                {
                    return _defaultValue.Value;
                }
            }
            set
            {
                lock (_lock)
                {
                    _defaultValue.Value = value;
                }
            }
        }

        public bool HasDefaultValue
        {
            get
            {
                lock (_lock)
                {
                    return _defaultValue.Written;
                }
            }
        }

        internal void Adopt(ProcessJobFieldConfig config)
        {
            lock (_lock)
            {
                this.DefaultValue = config.DefaultValue;
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
