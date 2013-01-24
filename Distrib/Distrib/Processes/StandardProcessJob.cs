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
using Distrib.IOC;
using System.Linq.Expressions;
using Distrib.Utils;

namespace Distrib.Processes
{
    /// <summary>
    /// Standard implementation of a process job
    /// </summary>
    public sealed class StandardProcessJob : CrossAppDomainObject, IJob, IJob_Internal
    {
        private readonly IJobInputTracker _inputTracker;
        private readonly IJobOutputTracker _outputTracker;
        private readonly IJobDefinition _jobDefinition;

        private readonly LockValue<List<IProcessJobValueField>> _inputValueFields =
            new LockValue<List<IProcessJobValueField>>(new List<IProcessJobValueField>());

        private readonly LockValue<List<IProcessJobValueField>> _outputValueFields =
            new LockValue<List<IProcessJobValueField>>(new List<IProcessJobValueField>());

        public StandardProcessJob(
            [IOC(false)] IJobInputTracker inputTracker,
            [IOC(false)] IJobOutputTracker outputTracker,
            [IOC(false)] IJobDefinition jobDefinition)
        {
            if (inputTracker == null) throw Ex.ArgNull(() => inputTracker);
            if (outputTracker == null) throw Ex.ArgNull(() => outputTracker);
            if (jobDefinition == null) throw Ex.ArgNull(() => jobDefinition);

            _inputTracker = inputTracker;
            _outputTracker = outputTracker;
            _jobDefinition = jobDefinition;
        }

        IJobInputTracker IJob.InputTracker
        {
            get { return _inputTracker; }
        }

        IJobOutputTracker IJob.OutputTracker
        {
            get { return _outputTracker; }
        }

        List<IProcessJobValueField> IJob_Internal.InputValueFields
        {
            get
            {
                return _inputValueFields.Value;
            }
        }

        List<IProcessJobValueField> IJob_Internal.OutputValueFields
        {
            get
            {
                return _outputValueFields.Value;
            }
        }

        void IJob_Internal.SetInputValue(IProcessJobDefinitionField defField, object value)
        {
            if (defField == null) throw Ex.ArgNull(() => defField);

            try
            {
                _inputValueFields.ReadWrite((inFields) =>
                    {
                        var vField = inFields.SingleOrDefault(f => f.Definition.Name == defField.Name);

                        if (vField != null)
                        {
                            vField.Value = value;
                        }
                        else
                        {
                            var pf = ProcessJobFieldFactory.CreateValueField(defField);
                            ((IProcessJobFieldConfig_Internal)pf.Definition.Config).Adopt(defField.Config);
                            pf.Value = value;
                            inFields.Add(pf);
                        }

                        return inFields;
                    });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to set input value", ex);
            }
        }

        void IJob_Internal.SetOutputValue(IProcessJobDefinitionField defField, object value)
        {
            if (defField == null) throw Ex.ArgNull(() => defField);

            try
            {
                _outputValueFields.ReadWrite((outFields) =>
                {
                    var vField = outFields.SingleOrDefault(f => f.Definition.Name == defField.Name);

                    if (vField != null)
                    {
                        vField.Value = value;
                    }
                    else
                    {
                        var pf = ProcessJobFieldFactory.CreateValueField(defField);
                        ((IProcessJobFieldConfig_Internal)pf.Definition.Config).Adopt(defField.Config);
                        pf.Value = value;
                        outFields.Add(pf);
                    }

                    return outFields;
                });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to set output value", ex);
            }
        }

        IJobDefinition IJob.Definition
        {
            get { return _jobDefinition; }
        }
    }
}
