﻿/*
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    /// <summary>
    /// Simple class for providing anytime write-once capability
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    [DebuggerDisplay("Written: {IsWritten}, Value: {Value}")]
    [Serializable()]
    public sealed class WriteOnce<T>
    {
        private T m_value = default(T);
        private bool m_bWritten = false;
        private object m_objLock = new object();

        public WriteOnce() { }
        public WriteOnce(T initialValue)
        {
            m_value = initialValue;
        }

        /// <summary>
        /// Gets whether the value has been written
        /// </summary>
        public bool IsWritten
        {
            get
            {
                lock (m_objLock)
                {
                    return m_bWritten;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the value
        /// </summary>
        public T Value
        {
            get
            {
                lock (m_objLock)
                {
                    return m_value;
                }
            }

            set
            {
                lock (m_objLock)
                {
                    if (!IsWritten)
                    {
                        m_value = value;
                        m_bWritten = true;
                    }
                    else
                    {
                        throw new InvalidOperationException("Value is already written");
                    }
                }
            }
        }

        /// <summary>
        /// Allows the write-once value to be gotten via implicit means
        /// </summary>
        /// <param name="writeOnce">The <see cref="WriteOnce[T]"/></param>
        /// <returns>The value</returns>
        public static implicit operator T(WriteOnce<T> writeOnce)
        {
            return writeOnce.Value;
        }
    }
}
