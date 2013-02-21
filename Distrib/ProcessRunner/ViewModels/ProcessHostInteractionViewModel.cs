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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using ProcessRunner.Models;
using ProcessRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessRunner.ViewModels
{
    public sealed class ProcessHostInteractionViewModel : ViewModelBase
    {
        private DistribProcessHost _processHost;
        public ProcessHostInteractionViewModel(
            IAppStateService appState,
            IDistribInteractionService distrib,
            IEventAggregator eventAggregator,
            DistribProcessHost processHost)
        {
            _processHost = processHost;
        }

        public DistribProcessHost ProcessHost
        {
            get
            {
                return _processHost;
            }
        }

        public string Title
        {
            get
            {
                return string.Format("Process Host: '{0}' (id:'{1}')",
                    _processHost.Process.Plugin.PluginName,
                    _processHost.InstanceID);
            }
        }

        private DelegateCommand<DistribJobDefinition> _executeCommand;
        public ICommand ExecuteJobCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new DelegateCommand<DistribJobDefinition>((def) =>
                        {
                            _processHost.ProcessJob(def, () =>
                                {
                                    SelectedJob.ExecutionError = null;
                                    SelectedJob.OutputFields = def.UnderlyingJobDefinition.OutputFields.Select(f =>
                                        {
                                            var field = Distrib.Processes.ProcessJobFieldFactory.CreateValueField(f);
                                            field.Value = "JOB IN PROGRESS";
                                            return field;
                                        }).ToList().AsReadOnly();
                                    OnPropertyChanged("CanExecuteJob");
                                    OnPropertyChanged("CanEnterInputs");

                                    // This seems to update the binding on the IsEnabled so seems to be needed
                                    // the OnPropertyChanged doesn't appear to be enough to inform it
                                    CommandManager.InvalidateRequerySuggested();
                                })
                                .ContinueWith(t =>
                                    {
                                        if (t.Exception != null)
                                        {
                                            var baseExcep = t.Exception.GetBaseException();
                                            SelectedJob.ExecutionError = string.Format("An error occurred: '{0}' ({1}) {2}",
                                                baseExcep.Message,
                                                baseExcep.Source,
                                                baseExcep.InnerException != null ?
                                                " - \"" + baseExcep.GetBaseException().Message + "\" (" + 
                                                    baseExcep.GetBaseException().Source + ")" : "");

                                            foreach (var of in SelectedJob.OutputFields)
                                            {
                                                of.Value = "EXECUTION ERROR";
                                            }
                                        }
                                        else
                                        {
                                            SelectedJob.OutputFields = t.Result;
                                        }


                                        OnPropertyChanged("CanExecuteJob");
                                        OnPropertyChanged("CanEnterInputs");
                                    }, TaskScheduler.FromCurrentSynchronizationContext());
                        });
                }

                return _executeCommand;
            }
        }

        private DistribJobDefinition _selectedJob;
        public DistribJobDefinition SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                _selectedJob = value;
                if (_selectedJob != null)
                {
                    foreach (var field in _selectedJob.InputFields)
                    {
                        field.PropertyChanged -= InputFieldChanged;
                        field.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(InputFieldChanged);
                    }
                }
                OnPropertyChanged("CanExecuteJob");
                OnPropertyChanged();
            }
        }

        private void InputFieldChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("CanExecuteJob");
        }

        public bool CanEnterInputs
        {
            get
            {
                if (_processHost.IsProcessing)
                {
                    return false;
                }

                return true;
            }
        }

        public bool CanExecuteJob
        {
            get
            {
                if (SelectedJob == null)
                {
                    return false;
                }

                if (_processHost.IsProcessing)
                {
                    return false;
                }

                // re-validate the fields here as the property change notification comes before the validation has been performed.
                // As a result the last input field being set doesn't trigger the binding for this property
                foreach (var f in SelectedJob.InputFields)
                {
                    var err = f["Value"];
                }

                var errorFields = SelectedJob.InputFields.Where(f => !string.IsNullOrEmpty(f.Error)).ToList();

                return errorFields.Count == 0;
            }
        }
    }
}
