using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Core interface for defining a Distrib-enabled process
    /// </summary>
    public interface IProcess
    {
    }

    public abstract class ProcessJobDefinition
    {
        private readonly string _jobTypeName;

        private readonly Type _inputAccessorType;
        private readonly Type _outputAccessorType;

        private readonly List<ProcessJobInputField> _inputFields =
            new List<ProcessJobInputField>();

        private readonly List<ProcessJobOutputField> _outputFields =
            new List<ProcessJobOutputField>();

        protected ProcessJobDefinition(string jobTypeName,
            Type inputAccessorType, 
            Type outputAccessorType)
        {
            if (string.IsNullOrEmpty(jobTypeName)) throw new ArgumentNullException();

            if (inputAccessorType == null) throw new ArgumentNullException();
            if (outputAccessorType == null) throw new ArgumentNullException();

            if (!inputAccessorType.IsInterface) throw new InvalidOperationException();
            if (!outputAccessorType.IsInterface) throw new InvalidOperationException();

            _inputAccessorType = inputAccessorType;
            _outputAccessorType = outputAccessorType;
        }
    }

    public abstract class ProcessJobDefinition<TInput, TOutput>
        : ProcessJobDefinition where TInput : class where TOutput: class
    {
        protected ProcessJobDefinition(string jobTypeName)
            : base(jobTypeName, typeof(TInput), typeof(TOutput))
        {

        }

        protected void ConfigInput(Expression<Func<TInput, object>> propExpression,
            ProcessJobInputConfig config)
        {

        }

        protected void ConfigOutput(Expression<Func<TOutput, object>> propExpression,
            ProcessJobOutputConfig config)
        {

        }

        //protected void ConfigAccessor(Expression<Func<TInput, object>> propExpression)
        //{
        //    var memExpr = propExpression.Body as MemberExpression;
        //    if (memExpr == null) throw new InvalidOperationException();

        //    var prop = memExpr.Member as PropertyInfo;
        //    if (prop == null) throw new InvalidOperationException();

        //}
    }

    public sealed class ProcessJobInputField
    {

    }

    public sealed class ProcessJobOutputField
    {

    }

    public sealed class ProcessJobInputConfig
    {

    }

    public sealed class ProcessJobOutputConfig
    {

    }
}
