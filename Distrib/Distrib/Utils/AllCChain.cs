using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    /// <summary>
    /// Represents an all-or-nothing conditional chain
    /// Each step specifies a value if that step returns conditional truth, if false then the
    /// initial falseresult is used and no further conditions are evaluated
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AllCChain<T>
    {
        private readonly T m_FailResult = default(T);
        private readonly WriteOnce<T> m_result = new WriteOnce<T>(default(T));
        private readonly AllCChain<T> m_prevChainItem = null;

        private AllCChain(T result, T failResult, AllCChain<T> prevItem, bool valueForSetting = true)
        {
            if (valueForSetting)
            {
                m_result.Value = result;
            }
            else
            {
                m_result = new WriteOnce<T>(result);
            }

            m_prevChainItem = prevItem;
            m_FailResult = failResult;
        }

        public static AllCChain<T> If(T anyFailResult, Func<bool> func, T result)
        {
            var res = func();
            if (res)
            {
                return new AllCChain<T>(result, anyFailResult, null, false);
            }
            else
            {
                return new AllCChain<T>(anyFailResult, anyFailResult, null, true);
            }
        }

        public AllCChain<T> ThenIf(Func<bool> func, T result)
        {
            if (!this.m_result.IsWritten)
            {
                var res = func();

                if (res)
                {
                    return new AllCChain<T>(result, m_FailResult, this, false);
                }
                else
                {
                    return new AllCChain<T>(m_FailResult, m_FailResult, this, true);
                }
            }
            else
            {
                return new AllCChain<T>(m_result, m_FailResult, this, true);
            }
        }

        public T Result
        {
            get
            {
                return m_result;
            }
        }
    }
}
