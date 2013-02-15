using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents an outgoing communications link
    /// </summary>
    public interface IOutgoingCommsLink : ICommsLink
    {
        object InvokeMethod(object[] args, [CallerMemberName] string methodName = "");
        object GetProperty([CallerMemberName] string propertyName = "");
        void SetProperty(object value, [CallerMemberName] string propertyName = "");
    }

    /// <summary>
    /// Represents an outgoing communications link with a type parameter to support compile-time checks
    /// </summary>
    /// <typeparam name="T">The comms interface type</typeparam>
    public interface IOutgoingCommsLink<T> : IOutgoingCommsLink where T :class
    {

    }
}
