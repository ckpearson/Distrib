/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
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
        IReadOnlyList<IProcessJobDefinitionField> InputFields { get; }

        /// <summary>
        /// Gets the output definition fields
        /// </summary>
        IReadOnlyList<IProcessJobDefinitionField> OutputFields { get; }

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
