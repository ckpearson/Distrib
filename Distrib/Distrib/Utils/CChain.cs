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
        private readonly T m_result = default(T);
        private readonly CChain<T> m_prevChainItem = null;

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="result">The result to set</param>
        /// <param name="previousChainItem">The previous item in the condition chain</param>
        private CChain(T result, CChain<T> previousChainItem)
        {
            m_result = result;
            m_prevChainItem = previousChainItem;
        }

        /// <summary>
        /// Starts a new conditional chain
        /// </summary>
        /// <param name="func">The conditional function to perform</param>
        /// <param name="result">The result in case of conditional truth</param>
        /// <param name="falseResult">The result in case of conditional false</param>
        /// <returns>The stage in the conditional chain</returns>
        public static CChain<T> If(Func<bool> func, T result, T falseResult = default(T))
        {
            return new CChain<T>(func() ? result : falseResult, null);
        }

        /// <summary>
        /// Continues a conditional chain
        /// </summary>
        /// <param name="func">The conditional function to perform</param>
        /// <param name="result">The result in case of conditional truth, the previous stage's result will be used if false</param>
        /// <returns>The stage in the conditional chain</returns>
        public CChain<T> ThenIf(Func<bool> func, T result)
        {
            return new CChain<T>(func() ? result : this.m_result, this);
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
