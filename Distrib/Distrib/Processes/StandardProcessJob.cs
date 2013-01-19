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

        IJobDefinition IJob_Internal.JobDefinition
        {
            get { return _jobDefinition; }
        }
    }
}
