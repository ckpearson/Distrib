/*
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
