using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Distrib.Utils;

namespace Distrib.Processes
{
    public sealed class JobDataHelper<T> where T : class
    {
        private readonly IJobDefinition _definition;

        private JobDataHelper(IJobDefinition definition)
        {
            _definition = definition;
        }

        public static JobDataHelper<T> New(IJobDefinition definition)
        {
            return new JobDataHelper<T>(definition);
        }

        public IJobInputSetDataHelper<T> ForInputSet()
        {
            return new JobInputSetDataHelper<T>(_definition);
        }

        public IJobInputGetDataHelper<T> ForInputGet(IJob job)
        {
            return new JobInputGetDataHelper<T>(_definition, job);
        }

        public IJobOutputSetHelper<T> ForOutputSet(IJob job)
        {
            return new JobOutputSetHelper<T>(_definition, job);
        }

        public IJobOutputGetHelper<T> ForOutputGet(IReadOnlyList<IProcessJobValueField> valueFields)
        {
            return new JobOutputGetHelper<T>(_definition, valueFields);
        }

        public IJobOutputGetHelper<T> ForOutputGet(IJob job)
        {
            return new JobOutputGetHelper<T>(_definition, job);
        }
    }

    public interface IJobInputSetDataHelper<T> where T : class
    {
        IJobInputSetDataHelper<T> Set<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        IReadOnlyList<IProcessJobValueField> ToValueFields();
    }

    internal sealed class JobInputSetDataHelper<T> : IJobInputSetDataHelper<T> where T : class
    {
        private readonly IJobDefinition _definition;
        private readonly Dictionary<IProcessJobDefinitionField, IProcessJobValueField>
            _dictFields = new Dictionary<IProcessJobDefinitionField, IProcessJobValueField>();

        public JobInputSetDataHelper(IJobDefinition definition)
        {
            _definition = definition;
        }

        IJobInputSetDataHelper<T> IJobInputSetDataHelper<T>.Set<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            var prop = expr.GetPropertyInfo();
            var dfld = _definition.InputFields
                .SingleOrDefault(f => f.Name == prop.Name && f.Type == prop.PropertyType);

            if (dfld == null)
            {
                throw new InvalidOperationException();
            }

            var vfld = ProcessJobFieldFactory.CreateValueField(dfld);
            vfld.Value = value;
            _dictFields[dfld] = vfld;
            return this;
        }

        IReadOnlyList<IProcessJobValueField> IJobInputSetDataHelper<T>.ToValueFields()
        {
            return _dictFields.Values.ToList().AsReadOnly();
        }
    }

    public interface IJobInputGetDataHelper<T> where T : class
    {
        TProp Get<TProp>(Expression<Func<T, TProp>> expr);
    }

    internal sealed class JobInputGetDataHelper<T> : IJobInputGetDataHelper<T> where T : class
    {
        private readonly IJobDefinition _definition;
        private readonly IJob _job;

        public JobInputGetDataHelper(IJobDefinition definition, IJob job)
        {
            _definition = definition;
            _job = job;
        }

        TProp IJobInputGetDataHelper<T>.Get<TProp>(Expression<Func<T, TProp>> expr)
        {
            var prop = expr.GetPropertyInfo();
            var dfld = _definition.InputFields
                .SingleOrDefault(f => f.Name == prop.Name && f.Type == prop.PropertyType);

            if (dfld == null)
            {
                throw new InvalidOperationException();
            }

            return _job.InputTracker.GetInput<TProp>(_job, dfld.Name);
        }
    }

    public interface IJobOutputSetHelper<T> where T : class
    {
        IJobOutputSetHelper<T> Set<TProp>(Expression<Func<T, TProp>> expr, TProp value);
    }

    internal sealed class JobOutputSetHelper<T> : IJobOutputSetHelper<T> where T : class
    {
        private readonly IJobDefinition _definition;
        private readonly IJob _job;

        public JobOutputSetHelper(IJobDefinition definition, IJob job)
        {
            _definition = definition;
            _job = job;
        }

        IJobOutputSetHelper<T> IJobOutputSetHelper<T>.Set<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            var prop = expr.GetPropertyInfo();
            var dfld = _definition.OutputFields
                .SingleOrDefault(f => f.Name == prop.Name && f.Type == prop.PropertyType);

            if (dfld == null)
            {
                throw new InvalidOperationException();
            }

            _job.OutputTracker.SetOutput<TProp>(_job, value, dfld.Name);
            return this;
        }
    }

    public interface IJobOutputGetHelper<T> where T : class
    {
        TProp Get<TProp>(Expression<Func<T, TProp>> expr);
    }

    internal sealed class JobOutputGetHelper<T> : IJobOutputGetHelper<T> where T : class
    {
        private readonly IJobDefinition _definition;
        private readonly IReadOnlyList<IProcessJobValueField> _lstValueFields = null;
        private readonly IJob _job = null;

        public JobOutputGetHelper(IJobDefinition definition, IReadOnlyList<IProcessJobValueField> valueFields)
        {
            _definition = definition;
            _lstValueFields = valueFields;
        }

        public JobOutputGetHelper(IJobDefinition definition, IJob job)
        {
            _definition = definition;
            _job = job;
        }

        TProp IJobOutputGetHelper<T>.Get<TProp>(Expression<Func<T, TProp>> expr)
        {
            var prop = expr.GetPropertyInfo();
            var dfld = _definition.OutputFields
                .SingleOrDefault(f => f.Name == prop.Name && f.Type == prop.PropertyType);

            if (dfld == null)
            {
                throw new InvalidOperationException();
            }

            if (_job == null && _lstValueFields != null)
            {
                var vfld = _lstValueFields.SingleOrDefault(f => f.Definition.Name ==
                        dfld.Name && f.Definition.Type.Equals(dfld.Type));

                if (vfld == null)
                {
                    throw new InvalidOperationException();
                }

                return (TProp)vfld.Value;
            }
            else
            {
                return _job.OutputTracker.GetOutput<TProp>(_job, dfld.Name);
            }
        }
    }
}
