using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    public sealed class LO<T> : IDisposable
    {
        private T m_value = default(T);
        private ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public LO(T initialValue = default(T))
        {
            m_value = initialValue;
        }

        public T Value
        {
            get
            {
                bool bRead = false;
                try
                {
                    m_lock.EnterReadLock();
                    bRead = true;
                    return m_value;
                }
                finally
                {
                    if (bRead)
                    {
                        m_lock.ExitReadLock();
                    }
                }
            }

            set
            {
                bool bWritten = false;
                try
                {
                    m_lock.EnterWriteLock();
                    m_value = value;
                    bWritten = true;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to set value", ex);
                }
                finally
                {
                    if (bWritten)
                    {
                        m_lock.ExitWriteLock();
                    }
                }
            }
        }

        public static implicit operator T(LO<T> lockObj)
        {
            return lockObj.Value;
        }

        public void Dispose()
        {
            if (m_lock != null)
            {
                m_lock.Dispose();
            }
        }
    }
}
