using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    /// <summary>
    /// Class allowing a value to be accessed in a many-read single-write fashion
    /// </summary>
    /// <typeparam name="T">The type of value to store</typeparam>
    [Serializable()]
    public sealed class LockValue<T>
    {
        private T _value;
        [NonSerialized()]
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private object _lockLock = new object();

        public LockValue(T initialValue = default(T))
        {
            _value = initialValue;
        }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public T Value
        {
            get
            {
                bool _didRead = false;
                try
                {
                    _initLock();
                    _lock.EnterReadLock();
                    _didRead = true;
                    return _value;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (_didRead)
                    {
                        _lock.ExitReadLock();
                    }
                }
            }

            set
            {
                bool _didWrite = false;
                try
                {
                    _initLock();
                    _lock.EnterWriteLock();
                    _didWrite = true;
                    _value = value;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (_didWrite)
                    {
                        _lock.ExitWriteLock();
                    }
                }
            }
        }

        private void _initLock()
        {
            lock (_lockLock)
            {
                if (_lock == null)
                {
                    _lock = new ReaderWriterLockSlim();
                } 
            }
        }

        /// <summary>
        /// Performs the given action while holding a read lock
        /// </summary>
        /// <param name="act">The action to perform</param>
        public void DoInRead(Action<T> act)
        {
            bool _didRead = false;
            try
            {
                _initLock();
                _lock.EnterReadLock();
                _didRead = true;
                act(_value);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to read", ex);
            }
            finally
            {
                if (_didRead)
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Performs the given action while holding a write lock
        /// </summary>
        /// <param name="act">The action to perform</param>
        public void DoInWrite(Action act)
        {
            bool _didWrite = false;
            try
            {
                _initLock();
                _lock.EnterWriteLock();
                _didWrite = true;
                act();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to do in write", ex);
            }
            finally
            {
                if (_didWrite)
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Performs the given function feeding in the latest value and returning the value to set
        /// </summary>
        /// <param name="func">The function to perform</param>
        public void ReadWrite(Func<T, T> func)
        {
            bool _didWrite = false;
            try
            {
                _initLock();
                _lock.EnterWriteLock();
                _didWrite = true;
                _value = func(_value);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (_didWrite)
                {
                    _lock.ExitWriteLock();
                }
            }
        }
    }
}
