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
