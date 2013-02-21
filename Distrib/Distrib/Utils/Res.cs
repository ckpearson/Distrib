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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    [DebuggerDisplay("Success: {Success}; CoreResult: {Result}")]
    public class Res<T>
    {
        private readonly bool m_bSuccess = false;
        private readonly T m_objResult = default(T);

        public Res(bool success, T result)
        {
            m_bSuccess = success;
            m_objResult = result;
        }

        public bool Success
        {
            get { return m_bSuccess; }
        }

        public T Result
        {
            get { return m_objResult; }
        }

        public static implicit operator bool(Res<T> res)
        {
            return res.Success;
        }
    }

    public class Res<T1, T2> : Res<T1>
    {
        private readonly T2 m_objResult = default(T2);

        public Res(bool success, T1 resultOne, T2 resultTwo)
            : base(success, resultOne)
        {
            m_objResult = resultTwo;
        }

        public T2 ResultTwo
        {
            get { return m_objResult; }
        }
    }
}
