using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.Utils;

namespace Distrib.Processes
{
    /// <summary>
    /// Used for creating a job definition
    /// </summary>
    /// <typeparam name="TInput">The interface type that the job takes input on</typeparam>
    /// <typeparam name="TOutput">The interface type that the job takes output on</typeparam>
    public sealed class ProcessJobDefinition<TInput, TOutput> : ProcessJobDefinitionBase
        , IJobDefinition<TInput, TOutput>
    {
        public ProcessJobDefinition(string jobName)
            : base(jobName, typeof(TInput), typeof(TOutput))
        {

        }

        public IProcessJobFieldConfig<TProp> ConfigInput<TProp>(System.Linq.Expressions.Expression<Func<TInput, TProp>> expr)
        {
            var pi = expr.GetPropertyInfo();
            var field = GetField(pi);
            if (!field.GetType().ContainsGenericParameters)
            {
                var oc = field.Config;
                var pjf = ProcessJobFieldFactory.CreateField<TProp>(field.Name, field.Mode);
                base.ReplaceField(field, pjf);
                field = pjf;
                ((IProcessJobFieldConfig_Internal)field.Config).Adopt(oc);
            }

            return field.Config as ProcessJobFieldConfig<TProp>;
        }

        public IProcessJobFieldConfig<TProp> ConfigOutput<TProp>(System.Linq.Expressions.Expression<Func<TOutput, TProp>> expr)
        {
            var pi = expr.GetPropertyInfo();
            var field = GetField(pi);
            if (!field.GetType().ContainsGenericParameters)
            {
                var oc = field.Config;
                var pjf = ProcessJobFieldFactory.CreateField<TProp>(field.Name, field.Mode);
                base.ReplaceField(field, pjf);
                field = pjf;
                ((IProcessJobFieldConfig_Internal)field.Config).Adopt(oc);
            }

            return field.Config as ProcessJobFieldConfig<TProp>;
        }
    }
}
