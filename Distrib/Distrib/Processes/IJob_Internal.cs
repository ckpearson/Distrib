using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Represents the internally available version of a job
    /// </summary>
    internal interface IJob_Internal : IJob
    {
        /// <summary>
        /// Gets the input value fields
        /// </summary>
        List<IProcessJobValueField> InputValueFields { get; }

        /// <summary>
        /// Gets the output value fields
        /// </summary>
        List<IProcessJobValueField> OutputValueFields { get; }

        /// <summary>
        /// Sets the value for the given input definition field
        /// </summary>
        /// <param name="defField">The definition field to use</param>
        /// <param name="value">The value to set</param>
        void SetInputValue(IProcessJobDefinitionField defField, object value);

        /// <summary>
        /// Sets the value for the given output definition field
        /// </summary>
        /// <param name="defField">The definition field to use</param>
        /// <param name="value">The value to set</param>
        void SetOutputValue(IProcessJobDefinitionField defField, object value);

        /// <summary>
        /// Gets the job definition
        /// </summary>
        IJobDefinition JobDefinition { get; }
    }
}
