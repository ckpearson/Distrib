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
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessRunner.Models
{
    public sealed class DistribProcessHost : INotifyPropertyChanged
    {
        private DistribProcess _process;
        private IProcessHost _host;

        public DistribProcessHost(DistribProcess process, IProcessHost host)
        {
            _process = process;
            _host = host;
        }

        public DistribProcess Process
        {
            get { return _process; }
        }

        public void Uninitialise()
        {
            if (_host != null && _host.IsInitialised)
            {
                _host.Unitialise();
            }

            _process.HostUninitialising(this);
        }

        public string CreationStamp
        {
            get
            {
                var culture = CultureInfo.CurrentCulture;
                var pattern = culture.DateTimeFormat.LongTimePattern;

                return new DateTime(_host.InstanceCreationStamp.Ticks)
                .ToString(pattern);
            }
        }

        public string InstanceID
        {
            get
            {
                return _host.InstanceID;
            }
        }

        private volatile bool _isProcessing;
        public bool IsProcessing
        {
            get
            {
                return _isProcessing;
            }
            set
            {
                lock (_lock)
                {
                    _isProcessing = value;
                    OnPropChange(); 
                }
            }
        }

        private object _lock = new object();

        public Task<IReadOnlyList<Distrib.Processes.IProcessJobValueField>> ProcessJob
            (DistribJobDefinition def, Action actBeginning)
        {
            this.IsProcessing = true;
            return Task<IReadOnlyList<Distrib.Processes.IProcessJobValueField>>.Factory.StartNew(() =>
                {
                    lock (_lock)
                    {
                        actBeginning();
                        var fields = def.InputFields.Select(f => f.UnderlyingValueField);
                        return _host.QueueJobAndWait(def.UnderlyingJobDefinition, fields.ToList()).ToList().AsReadOnly(); 
                    }
                }).ContinueWith<IReadOnlyList<Distrib.Processes.IProcessJobValueField>>(t =>
                    {
                        this.IsProcessing = false;
                        return t.Result;
                    });
        }

        public IReadOnlyList<DistribJobDefinition> JobDefinitions
        {
            get
            {
                return _host.JobDefinitions.Select(j => new DistribJobDefinition(j))
                    .ToList().AsReadOnly();
            }
        }

        private DelegateCommand _killCommand;
        public ICommand KillCommand
        {
            get
            {
                if (_killCommand == null)
                {
                    _killCommand = new DelegateCommand(() =>
                        {
                            Process.KillHost(this);
                        },() => !IsProcessing);
                }

                return _killCommand;
            }
        }

        private DelegateCommand _interactCommand;
        public ICommand InteractCommand
        {
            get
            {
                if (_interactCommand == null)
                {
                    _interactCommand = new DelegateCommand(() =>
                        {
                            Process.InteractWithHost(this);
                        });
                }

                return _interactCommand;
            }
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
