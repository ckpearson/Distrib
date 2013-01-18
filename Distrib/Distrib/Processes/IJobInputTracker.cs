using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Represents an input tracker for a job
    /// </summary>
    public interface IJobInputTracker
    {
        /// <summary>
        /// Get the input in the given type form for the given job with the given name
        /// </summary>
        /// <typeparam name="T">The type to return the input as</typeparam>
        /// <param name="forJob">The job the input belongs to</param>
        /// <param name="prop">The input property name</param>
        /// <returns>The input value</returns>
        T GetInput<T>(IJob forJob, [CallerMemberName] string prop = null);
    }
}
