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
