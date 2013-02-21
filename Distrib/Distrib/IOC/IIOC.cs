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

namespace Distrib.IOC
{
    public interface IIOC
    {
        /// <summary>
        /// Gets an instance of the given service type
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <param name="args">The constructor arguments</param>
        /// <returns>The instance</returns>
        T Get<T>(params IOCConstructorArgument[] args);

        /// <summary>
        /// Gets an instance of the given service type
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="args">The constructor arguments</param>
        /// <returns>The instance</returns>
        object Get(Type serviceType, params IOCConstructorArgument[] args);

        /// <summary>
        /// Determines whether the given service type is bound
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns><c>True</c> if bound, <c>False</c> otherwise</returns>
        bool IsTypeBound<T>();

        /// <summary>
        /// Determines whether the given service type is bound
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns><c>True</c> if bound, <c>False</c> otherwise</returns>
        bool IsTypeBound(Type serviceType);

        /// <summary>
        /// Starts this bootstrapper off as the current bootstrapper
        /// </summary>
        void Start();
    }
}
