/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
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
