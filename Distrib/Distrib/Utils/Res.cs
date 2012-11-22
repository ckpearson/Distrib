using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    [DebuggerDisplay("Success: {Success}")]
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

        public object ResultTwo
        {
            get { return m_objResult; }
        }
    }
}
