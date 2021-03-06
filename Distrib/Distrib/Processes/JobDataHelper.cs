﻿/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Distrib.Utils;

namespace Distrib.Processes
{
    /// <summary>
    /// Provides helper functions for dealing with job data
    /// </summary>
    /// <typeparam name="T">The input or output interface to work with</typeparam>
    public sealed class JobDataHelper<T> where T : class
    {
        private readonly IJobDefinition _definition;

        private JobDataHelper(IJobDefinition definition)
        {
            _definition = definition;
        }

        /// <summary>
        /// Creates a new data helper
        /// </summary>
        /// <param name="definition">The definition of the job being worked with</param>
        /// <returns>The data helper</returns>
        public static JobDataHelper<T> New(IJobDefinition definition)
        {
            return new JobDataHelper<T>(definition);
        }

        /// <summary>
        /// Gets a data helper for setting input data
        /// </summary>
        /// <returns>The data helper</returns>
        public IJobInputSetDataHelper<T> ForInputSet()
        {
            return new JobInputSetDataHelper<T>(_definition);
        }

        /// <summary>
        /// Gets a data helper for getting input data
        /// </summary>
        /// <param name="job">The job the data is for</param>
        /// <returns></returns>
        public IJobInputGetDataHelper<T> ForInputGet(IJob job)
        {
            return new JobInputGetDataHelper<T>(_definition, job);
        }

        /// <summary>
        /// Gets a data helper for setting output data
        /// </summary>
        /// <param name="job">The job the data is for</param>
        /// <returns>The data helper</returns>
        public IJobOutputSetHelper<T> ForOutputSet(IJob job)
        {
            return new JobOutputSetHelper<T>(_definition, job);
        }

        /// <summary>
        /// Gets a data helper for getting output data from the value fields
        /// </summary>
        /// <param name="valueFields">The value fields</param>
        /// <returns>The data helper</returns>
        public IJobOutputGetHelper<T> ForOutputGet(IReadOnlyList<IProcessJobValueField> valueFields)
        {
            return new JobOutputGetHelper<T>(_definition, valueFields);
        }

        /// <summary>
        /// Gets a data helper for getting output data from a given job
        /// </summary>
        /// <param name="job">The job the data is on</param>
        /// <returns>The data helper</returns>
        public IJobOutputGetHelper<T> ForOutputGet(IJob job)
        {
            return new JobOutputGetHelper<T>(_definition, job);
        }
    }

    /// <summary>
    /// Job data helper for setting inputs
    /// </summary>
    /// <typeparam name="T">The input interface</typeparam>
    public interface IJobInputSetDataHelper<T> where T : class
    {
        /// <summary>
        /// Set the given input
        /// </summary>
        /// <typeparam name="TProp">The input field property type</typeparam>
        /// <param name="expr">The expression pointing to the input property</param>
        /// <param name="value">The value to set</param>
        /// <returns>The data helper</returns>
        IJobInputSetDataHelper<T> Set<TProp>(Expression<Func<T, TProp>> expr, TProp value);

        /// <summary>
        /// Produces the input value fields
        /// </summary>
        /// <returns>The input value fields</returns>
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
