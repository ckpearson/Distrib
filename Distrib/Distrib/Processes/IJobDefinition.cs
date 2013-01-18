using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Represents a job definition
    /// </summary>
    public interface IJobDefinition
    {
        /// <summary>
        /// Gets the input definition fields
        /// </summary>
        IReadOnlyList<IProcessJobField> InputFields { get; }

        /// <summary>
        /// Gets the output definition fields
        /// </summary>
        IReadOnlyList<IProcessJobField> OutputFields { get; }

        /// <summary>
        /// Gets the name of the job definition
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Represents a generic job definition - used by processes
    /// </summary>
    /// <typeparam name="TInput">The input interface that the job takes input on</typeparam>
    /// <typeparam name="TOutput">The output interface that the job takes output on</typeparam>
    public interface IJobDefinition<TInput, TOutput> : IJobDefinition
    {
        /// <summary>
        /// Access the configuration for the given input field
        /// </summary>
        /// <typeparam name="TProp">The input field property type</typeparam>
        /// <param name="expr">Expression pointing to input field property</param>
        /// <returns>The input field configuration</returns>
        IProcessJobFieldConfig<TProp> ConfigInput<TProp>(Expression<Func<TInput, TProp>> expr);

        /// <summary>
        /// Access the configuration for the given output field
        /// </summary>
        /// <typeparam name="TProp">The output field property type</typeparam>
        /// <param name="expr">Expression pointing to output field property</param>
        /// <returns>The output field configuration</returns>
        IProcessJobFieldConfig<TProp> ConfigOutput<TProp>(Expression<Func<TOutput, TProp>> expr);
    }
}
