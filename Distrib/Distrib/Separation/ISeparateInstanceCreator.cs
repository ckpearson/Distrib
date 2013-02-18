using Distrib.IOC;
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
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    /// <summary>
    /// Creates instances of a given type separated into its own AppDomain
    /// </summary>
    public interface ISeparateInstanceCreator
    {
        /// <summary>
        /// Create an instance of the given type, separated by its own AppDomain
        /// </summary>
        /// <param name="type">The type of the instance to create</param>
        /// <param name="args">The constructor arguments required, those bound with IOC will be provided automatically.</param>
        /// <returns>The instance</returns>
        object CreateInstanceWithSeparation(Type type, IOCConstructorArgument[] args);

        T CreateInstanceWithSeparation<T>(IOCConstructorArgument[] args) where T : class;

        /// <summary>
        /// Creates an instance of the given type, no separation is used
        /// </summary>
        /// <param name="type">The type of the instance to create</param>
        /// <param name="args">The constructor arguments required, those bound with IOC will be provided automatically.</param>
        /// <returns>The instance</returns>
        object CreateInstanceWithoutSeparation(Type type, IOCConstructorArgument[] args);

        T CreateInstanceWithoutSeparation<T>(IOCConstructorArgument[] args) where T : class;

        object CreateInstanceSeparatedWithLoadedAssembly(Type type, string assemblyPath, IOCConstructorArgument[] args);

        T CreateInstanceSeparatedWithLoadedAssembly<T>(string assemblyPath, IOCConstructorArgument[] args) where T : class;
    }
}
