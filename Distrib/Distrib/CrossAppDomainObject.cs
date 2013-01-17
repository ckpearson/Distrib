﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Distrib
{
    /// <summary>
    /// CrossAppDomainObject use instead of MarshalByRefObject
    /// </summary>
    /// <remarks>
    /// Taken from: http://nbevans.wordpress.com/2011/04/17/memory-leaks-with-an-infinite-lifetime-instance-of-marshalbyrefobject/
    /// </remarks>
    public abstract class CrossAppDomainObject : MarshalByRefObject, IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Gets an enumeration of nested <see cref="MarshalByRefObject"/> objects.
        /// </summary>
        protected virtual IEnumerable<MarshalByRefObject> NestedMarshalByRefObjects
        {
            get { yield break; }
        }

        ~CrossAppDomainObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disconnects the remoting channel(s) of this object and all nested objects.
        /// </summary>
        private void Disconnect()
        {
            RemotingServices.Disconnect(this);

            foreach (var tmp in NestedMarshalByRefObjects)
                RemotingServices.Disconnect(tmp);
        }

        public sealed override object InitializeLifetimeService()
        {
            //
            // Returning null designates an infinite non-expiring lease.
            // We must therefore ensure that RemotingServices.Disconnect() is called when
            // it's no longer needed otherwise there will be a memory leak.
            //
            return null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            Disconnect();
            _disposed = true;
        }

    }

}
