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
