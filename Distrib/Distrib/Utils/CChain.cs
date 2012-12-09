using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    /// <summary>
    /// Utility class for super simple chained conditions where the prior result is used in case of conditional fail
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CChain<T>
    {
        private readonly WriteOnce<T> m_result = new WriteOnce<T>(default(T));
        private readonly CChain<T> m_prevChainItem = null;

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="result">The result to set</param>
        /// <param name="previousChainItem">The previous item in the condition chain</param>
        /// <param name="valueForSetting">Whether the value is to be set as final or simply as a stand-in until next piece of chain</param>
        private CChain(T result, CChain<T> previousChainItem, bool valueForSetting = true)
        {
            if (valueForSetting)
            {
                m_result.Value = result;
            }
            else
            {
                m_result = new WriteOnce<T>(result);
            }

            m_prevChainItem = previousChainItem;
        }

        /// <summary>
        /// Starts a new conditional chain
        /// </summary>
        /// <param name="func">The conditional function to perform</param>
        /// <param name="result">The result in case of conditional truth</param>
        /// <returns>The stage in the conditional chain</returns>
        public static CChain<T> If(Func<bool> func, T result)
        {
            //return new CChain<T>(func() ? result : default(T), null, true);
            var res = func();

            if (res)
            {
                return new CChain<T>(result, null, true);
            }
            else
            {
                return new CChain<T>(default(T), null, false);
            }
        }

        /// <summary>
        /// Continues a conditional chain
        /// </summary>
        /// <param name="func">The conditional function to perform</param>
        /// <param name="result">The result in case of conditional truth, the previous stage's result will be used if false</param>
        /// <returns>The stage in the conditional chain</returns>
        public CChain<T> ThenIf(Func<bool> func, T result)
        {
            // If the result was actually set previously then the previous condition in the chain
            // was true and the result was provided, this means this condition is no longer required
            // therefore simply initialise the next piece in the chain with the result prior to this one (to propogate to the end)
            // If the result wasn't written however, this piece of the chain needs computing.

            if (!this.m_result.IsWritten)
            {
                var res = func();

                if (res)
                {
                    return new CChain<T>(result, this, true);
                }
                else
                {
                    return new CChain<T>(m_result, this, false);
                }
            }
            else
            {
                return new CChain<T>(m_result, this);
            }
        }

        /// <summary>
        /// Gets the result of the condition chain
        /// </summary>
        public T Result
        {
            get { return m_result; }
        }
    }
}
