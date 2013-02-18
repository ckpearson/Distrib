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
using System.Reflection;

namespace Distrib.Processes
{
    public abstract class ProcessJobDefinitionBase : CrossAppDomainObject, IJobDefinition
    {
        private readonly Type _inputInterfaceType;
        private readonly Type _outputInterfaceType;
        private readonly string _jobName;
        private readonly string _jobDescription;

        private readonly LockValue<List<IProcessJobDefinitionField>> _fields =
            new LockValue<List<IProcessJobDefinitionField>>(new List<IProcessJobDefinitionField>());

        public ProcessJobDefinitionBase(
            [IOC(false)] string jobName,
            [IOC(false)] string jobDescription,
            [IOC(false)] Type inputInterfaceType,
            [IOC(false)] Type outputInterfaceType)
        {
            if (string.IsNullOrEmpty(jobName)) throw Ex.ArgNull(() => jobName);
            if (inputInterfaceType == null) throw Ex.ArgNull(() => inputInterfaceType);
            if (outputInterfaceType == null) throw Ex.ArgNull(() => outputInterfaceType);

            if (!inputInterfaceType.IsInterface) throw Ex.Arg(() => inputInterfaceType,
                "Must be an interface");

            if (!outputInterfaceType.IsInterface) throw Ex.Arg(() => outputInterfaceType,
                "Must be an interface");

            _jobName = jobName;
            _jobDescription = jobDescription;
            _inputInterfaceType = inputInterfaceType;
            _outputInterfaceType = outputInterfaceType;

            _buildInitialFields();
        }

        private void _buildInitialFields()
        {
            try
            {
                _fields.ReadWrite((fieldsList) =>
                    {

                        var foundFields =
                            ProcessJobFieldsGeneratorService.GetFieldsFromInterface(_inputInterfaceType, FieldMode.Input)
                            .Concat(
                                ProcessJobFieldsGeneratorService.GetFieldsFromInterface(_outputInterfaceType, FieldMode.Output));

                        return new List<IProcessJobDefinitionField>(foundFields);
                    });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to build initial fields", ex);
            }
        }

        public IReadOnlyList<IProcessJobDefinitionField> InputFields
        {
            get
            {
                return _fields.Value.Where(f => f.Mode == FieldMode.Input)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public IReadOnlyList<IProcessJobDefinitionField> OutputFields
        {
            get
            {
                return _fields.Value.Where(f => f.Mode == FieldMode.Output)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public string Name
        {
            get { return _jobName; }
        }

        protected IProcessJobDefinitionField GetField(PropertyInfo pi)
        {
            if (pi == null) throw Ex.ArgNull(() => pi);

            try
            {
                return _fields.Value.SingleOrDefault(f => f.Name == pi.Name && f.Type.Equals(pi.PropertyType));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get field", ex);
            }
        }

        protected void ReplaceField(IProcessJobDefinitionField field, IProcessJobDefinitionField replacement)
        {
            if (field == null) throw Ex.ArgNull(() => field);
            if (replacement == null) throw Ex.ArgNull(() => replacement);

            try
            {
                _fields.ReadWrite((fields) =>
                    {
                        var index = fields.IndexOf(field);
                        if (index < 0)
                        {
                            throw new InvalidOperationException("Couldn't find field in fields list");
                        }

                        fields[index] = replacement;

                        return fields;
                    });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to replace field", ex);
            }
        }


        public string Description
        {
            get { return _jobDescription; }
        }

        public bool Match(IJobDefinition definition)
        {
            return JobMatchingService.Match(this, definition, true);

            //return AllCChain<bool>
            //    .If(false, () => this.Name == definition.Name, true)
            //    .ThenIf(() => this.Description == definition.Description, true)
            //    .ThenIf(() => this.InputFields != null && definition.InputFields != null, true)
            //    .ThenIf(() => this.InputFields.Count == definition.InputFields.Count, true)
            //    .ThenIf(() =>
            //        {
            //            bool match = true;
            //            for (int i = 0; i < this.InputFields.Count; i++)
            //            {
            //                var leftField = this.InputFields[i];
            //                var rightField = definition.InputFields[i];

            //                if (!leftField.Match(rightField))
            //                {
            //                    match = false;
            //                    break;
            //                }
            //            }

            //            return match;
            //        }, true)
            //    .ThenIf(() => this.OutputFields != null && definition.OutputFields != null, true)
            //    .ThenIf(() => this.OutputFields.Count == definition.OutputFields.Count, true)
            //    .ThenIf(() =>
            //    {
            //        bool match = true;
            //        for (int i = 0; i < this.OutputFields.Count; i++)
            //        {
            //            var leftField = this.OutputFields[i];
            //            var rightField = definition.OutputFields[i];

            //            if (!leftField.Match(rightField))
            //            {
            //                match = false;
            //                break;
            //            }
            //        }

            //        return match;
            //    }, true)
            //    .Result;
        }


        public IJobDefinition ToFlattened()
        {
            return new FlattenedJobDefinition(this);
        }
    }

    public static class JobMatchingService
    {
        public static bool Match(IJobDefinition left, IJobDefinition right, bool matchConfig = true)
        {
            return AllCChain<bool>
                .If(false, () => left.Name == right.Name, true)
                .ThenIf(() => left.Description == right.Description, true)
                .ThenIf(() => left.InputFields != right && right.InputFields != null, true)
                .ThenIf(() => left.InputFields.Count == right.InputFields.Count, true)
                .ThenIf(() =>
                {
                    bool match = true;
                    for (int i = 0; i < left.InputFields.Count; i++)
                    {
                        var leftField = left.InputFields[i];
                        var rightField = right.InputFields[i];

                        if (!leftField.Match(rightField, matchConfig))
                        {
                            match = false;
                            break;
                        }
                    }

                    return match;
                }, true)
                .ThenIf(() => left.OutputFields != null && right.OutputFields != null, true)
                .ThenIf(() => left.OutputFields.Count == right.OutputFields.Count, true)
                .ThenIf(() =>
                {
                    bool match = true;
                    for (int i = 0; i < left.OutputFields.Count; i++)
                    {
                        var leftField = left.OutputFields[i];
                        var rightField = right.OutputFields[i];

                        if (!leftField.Match(rightField, matchConfig))
                        {
                            match = false;
                            break;
                        }
                    }

                    return match;
                }, true)
                .Result;
        }
    }
}
