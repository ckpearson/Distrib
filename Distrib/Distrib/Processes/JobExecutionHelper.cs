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
                    if (def.Match(definition))
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
