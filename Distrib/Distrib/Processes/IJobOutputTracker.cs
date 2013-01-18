using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Represents an output tracker for a job
    /// </summary>
    public interface IJobOutputTracker
    {
        /// <summary>
        /// Get the output in the given type form for the given job with the given name
        /// </summary>
        /// <typeparam name="T">The type to return the output as</typeparam>
        /// <param name="forJob">The job the output belongs to</param>
        /// <param name="prop">The output property name</param>
        /// <returns>The output value</returns>
        T GetOutput<T>(IJob forJob, [CallerMemberName] string prop = null);

        /// <summary>
        /// Sets the output of the given type form for the given job with the given name to the given value
        /// </summary>
        /// <typeparam name="T">The type the out value is</typeparam>
        /// <param name="forJob">The job the output belongs to</param>
        /// <param name="value">The value to set the output to</param>
        /// <param name="prop">The output property name</param>
        void SetOutput<T>(IJob forJob, T value, [CallerMemberName] string prop = null);
    }
}
