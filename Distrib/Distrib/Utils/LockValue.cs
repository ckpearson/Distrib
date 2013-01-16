using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    [Serializable()]
    public sealed class LockValue<T>
    {
        private T _value;
        [NonSerialized()]
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public LockValue(T initialValue = default(T))
        {
            _value = initialValue;
        }

        public T Value
        {
            get
            {
                bool _didRead = false;
                try
                {
                    _lock.EnterReadLock();
                    _didRead = true;
                    return _value;
                }
                catch (Exception ex)
                {
                    throw ex;
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
                    _lock.EnterWriteLock();
                    _didWrite = true;
                    _value = value;
                }
                catch (Exception ex)
                {
                    throw ex;
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

        public void Read(Action<T> act)
        {
            bool _didRead = false;
            try
            {
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

        public void Write(T val)
        {
            bool _didWrite = false;
            try
            {
                _lock.EnterWriteLock();
                _didWrite = true;
                _value = val;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_didWrite)
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public void ReadWrite(Func<T, T> func)
        {
            bool _didWrite = false;
            try
            {
                _lock.EnterUpgradeableReadLock();
                var val = func(_value);
                _lock.EnterWriteLock();
                _didWrite = true;
                _value = val;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_didWrite)
                {
                    _lock.ExitWriteLock();
                    _lock.ExitUpgradeableReadLock();
                }
            }
        }
    }
}
