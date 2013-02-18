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
using Distrib.Utils;

namespace Distrib.Processes
{
    /// <summary>
    /// Used for creating a job definition
    /// </summary>
    /// <typeparam name="TInput">The interface type that the job takes input on</typeparam>
    /// <typeparam name="TOutput">The interface type that the job takes output on</typeparam>
    public sealed class ProcessJobDefinition<TInput, TOutput> : ProcessJobDefinitionBase
        , IJobDefinition<TInput, TOutput>
    {
        public ProcessJobDefinition(string jobName, string jobDescription)
            : base(jobName, jobDescription, typeof(TInput), typeof(TOutput))
        {

        }

        public IProcessJobFieldConfig<TProp> ConfigInput<TProp>(System.Linq.Expressions.Expression<Func<TInput, TProp>> expr)
        {
            var pi = expr.GetPropertyInfo();
            var field = GetField(pi);
            if (!field.GetType().ContainsGenericParameters)
            {
                var oc = field.Config;
                var pjf = ProcessJobFieldFactory.CreateDefinitionField<TProp>(field.Name, field.Mode);
                base.ReplaceField(field, pjf);
                field = pjf;
                ((IProcessJobFieldConfig_Internal)field.Config).Adopt(oc);
            }

            return field.Config as ProcessJobFieldConfig<TProp>;
        }

        public IProcessJobFieldConfig<TProp> ConfigOutput<TProp>(System.Linq.Expressions.Expression<Func<TOutput, TProp>> expr)
        {
            var pi = expr.GetPropertyInfo();
            var field = GetField(pi);
            if (!field.GetType().ContainsGenericParameters)
            {
                var oc = field.Config;
                var pjf = ProcessJobFieldFactory.CreateDefinitionField<TProp>(field.Name, field.Mode);
                base.ReplaceField(field, pjf);
                field = pjf;
                ((IProcessJobFieldConfig_Internal)field.Config).Adopt(oc);
            }

            return field.Config as ProcessJobFieldConfig<TProp>;
        }
    }

    [Serializable()]
    public sealed class FlattenedJobDefinition : IJobDefinition
    {
        private IReadOnlyList<IProcessJobDefinitionField> _inpFields;
        private IReadOnlyList<IProcessJobDefinitionField> _outFields;

        private string _name;
        private string _desc;

        public FlattenedJobDefinition(IJobDefinition definition)
        {
            _inpFields = definition.InputFields.Select(f =>
                {
                    var fld = ProcessJobFieldFactory.CreateDefinitionField(f.Type, f.Name, f.Mode);
                    ((IProcessJobFieldConfig_Internal)fld.Config).Adopt(f.Config);

                    return fld;
                }).ToList().AsReadOnly();

            _outFields = definition.OutputFields.Select(f =>
                {
                    var fld = ProcessJobFieldFactory.CreateDefinitionField(f.Type, f.Name, f.Mode);
                    ((IProcessJobFieldConfig_Internal)fld.Config).Adopt(f.Config);

                    return fld;

                }).ToList().AsReadOnly();

            _name = definition.Name;
            _desc = definition.Description;
        }

        public IReadOnlyList<IProcessJobDefinitionField> InputFields
        {
            get { return _inpFields; }
        }

        public IReadOnlyList<IProcessJobDefinitionField> OutputFields
        {
            get { return _outFields; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _desc; }
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
            //    {
            //        bool match = true;
            //        for (int i = 0; i < this.InputFields.Count; i++)
            //        {
            //            var leftField = this.InputFields[i];
            //            var rightField = definition.InputFields[i];

            //            if (!leftField.Match(rightField))
            //            {
            //                match = false;
            //                break;
            //            }
            //        }

            //        return match;
            //    }, true)
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
            return this;
        }
    }
}
