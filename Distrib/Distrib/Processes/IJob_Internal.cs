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
    }
}
