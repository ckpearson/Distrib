using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Simple fluent helper class for managing execution of multiple job types
    /// </summary>
    public class JobExecutionHelper
    {
        private sealed class _jobEx
        {
            private readonly Func<IJobDefinition> _funcCheck;
            private readonly Action _act;

            public _jobEx(Func<IJobDefinition> funcCheck, Action act)
            {
                _funcCheck = funcCheck;
                _act = act;
            }

            public Func<IJobDefinition> FuncCheck { get { return _funcCheck; } }
            public Action Act { get { return _act; } }
        }

        private List<_jobEx> _lstJobsExs = new List<_jobEx>();

        private JobExecutionHelper() { }

        /// <summary>
        /// Create a new helper for use
        /// </summary>
        /// <returns>The execution helper</returns>
        public static JobExecutionHelper New()
        {
            return new JobExecutionHelper();
        }

        /// <summary>
        /// Add a handler for a given job definition
        /// </summary>
        /// <param name="funcCheck">The function to retrieve the job definition in question</param>
        /// <param name="act">The action to invoke when the given job definition is the one in question</param>
        /// <returns>The execution helper with the handler added</returns>
        public JobExecutionHelper AddHandler(Func<IJobDefinition> funcCheck, Action act)
        {
            _lstJobsExs.Add(new _jobEx(funcCheck, act));
            return this;
        }

        /// <summary>
        /// Execute the required action on the handler registered that matches the given definition
        /// </summary>
        /// <param name="definition">The definition to match and use</param>
        public void Execute(IJobDefinition definition)
        {
            _jobEx foundEx = null;

            try
            {
                foreach (var _ex in _lstJobsExs)
                {
                    var def = _ex.FuncCheck();
                    if (def == definition)
                    {
                        foundEx = _ex;
                        break;
                    }
                }

                if (foundEx == null)
                {
                    throw new ApplicationException(string.Format("Couldn't execute job of definition '{0}' no handler was registered for this type", definition.Name));
                }
                else
                {
                    foundEx.Act();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to execute action for given definition", ex);
            }
        }
    }
}
