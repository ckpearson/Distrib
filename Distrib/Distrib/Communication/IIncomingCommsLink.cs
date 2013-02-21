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

namespace Distrib.Communication
{
    /// <summary>
    /// Represents an incoming comms link
    /// </summary>
    public interface IIncomingCommsLink : ICommsLink
    {
        /// <summary>
        /// Starts listening for messages for processing against the given target
        /// </summary>
        /// <param name="target">The target the messages are to be processed against</param>
        void StartListening(object target);

        /// <summary>
        /// Stops listening for comms messages
        /// </summary>
        void StopListening();

        /// <summary>
        /// Gets whether the comms link is currently listening
        /// </summary>
        bool IsListening { get; }

        object GetEndpointDetails();

        IOutgoingCommsLink CreateOutgoingOfSameTransport(object endpoint);
    }

    /// <summary>
    /// Represents an incoming comms link with a type parameter for compile-time checks
    /// </summary>
    /// <typeparam name="T">The comms interface type</typeparam>
    public interface IIncomingCommsLink<T> : IIncomingCommsLink where T : class
    {
        /// <summary>
        /// Starts listening for message for processing against the given target
        /// </summary>
        /// <param name="target">The target the messages are to be processed against</param>
        void StartListening(T target);

        new IOutgoingCommsLink<T> CreateOutgoingOfSameTransport(object endpoint);
        IOutgoingCommsLink<K> CreateOutgoingOfSameTransportDiffContract<K>(object endpoint) where K : class;
    }
}
