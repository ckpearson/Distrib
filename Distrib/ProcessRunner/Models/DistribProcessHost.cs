using Distrib.Processes;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessRunner.Models
{
    public sealed class DistribProcessHost
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

        public bool IsProcessing
        {
            get
            {
                return false;
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
    }
}
