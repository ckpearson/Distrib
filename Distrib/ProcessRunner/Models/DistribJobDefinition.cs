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
    }
}
