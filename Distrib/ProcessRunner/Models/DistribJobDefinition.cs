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
using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Models
{
    public sealed class DistribJobDefinition : INotifyPropertyChanged
    {
        private IJobDefinition _definition;

        public DistribJobDefinition(IJobDefinition definition)
        {
            _definition = definition;
        }

        public string Name
        {
            get { return _definition.Name; }
        }

        public string Description
        {
            get { return _definition.Description; }
        }

        private IReadOnlyList<DistribProcessInputField> _inputFields = null;
        public IReadOnlyList<DistribProcessInputField> InputFields
        {
            get
            {
                if (_inputFields == null)
                {
                    _inputFields = _definition.InputFields.Select(d =>
                                new DistribProcessInputField(
                                    ProcessJobFieldFactory.CreateValueField(d)))
                                    .ToList()
                                    .AsReadOnly(); 
                }

                return _inputFields;
            }
        }

        private IReadOnlyList<Distrib.Processes.IProcessJobValueField> _outputFields;
        public IReadOnlyList<Distrib.Processes.IProcessJobValueField> OutputFields
        {
            get
            {
                return _outputFields;
            }
            set
            {
                _outputFields = value;
                OnPropChange();
            }
        }

        public IJobDefinition UnderlyingJobDefinition
        {
            get { return _definition; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropChange([CallerMemberName] string prop = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        private string _executionError;
        public string ExecutionError
        {
            get { return _executionError; }
            set
            {
                _executionError = value;
                OnPropChange();
                OnPropChange("HasExecutionError");
            }
        }

        public bool HasExecutionError
        {
            get
            {
                return !string.IsNullOrEmpty(ExecutionError);
            }
        }
    }
}
