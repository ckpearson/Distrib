using Microsoft.Practices.Prism.Commands;
using ProcessNode.Events;
using ProcessNode.Models;
using ProcessNode.Services;
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
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessNode.ViewModels
{
    [Export]
    public sealed class ConnectionDetailsEditViewModel : ViewModelBase
    {
        private readonly INewEventAggregator _eventAgg;
        private readonly ICommsService _commsService;

        [ImportingConstructor]
        public ConnectionDetailsEditViewModel(
            INewEventAggregator eventAgg,
            ICommsService commsService)
        {
            _eventAgg = eventAgg;
            _commsService = commsService;

            SelectedDetail = AvailableDetails.First();
        }

        public IEnumerable<ConnectionDetails> AvailableDetails
        {
            get { return _commsService.AvailableConnectionDetails; }
        }

        private ConnectionDetails _selectedDetail;
        public ConnectionDetails SelectedDetail
        {
            get { return _selectedDetail; }
            set
            {
                _selectedDetail = value;
                foreach (var comp in _selectedDetail.Components)
                {
                    comp.PropertyChanged -= ComponentValueChanged;
                    comp.PropertyChanged += ComponentValueChanged;
                }
                PropChange();
                PropChange("CanStartListening");
            }
        }

        void ComponentValueChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropChange("CanStartListening");
        }

        private DelegateCommand _startListeningCommand;
        public ICommand StartListeningCommand
        {
            get
            {
                if (_startListeningCommand == null)
                {
                    _startListeningCommand = new DelegateCommand(OnDoStartListening);
                }

                return _startListeningCommand;
            }
        }

        public bool CanStartListening
        {
            get
            {
                if (_selectedDetail == null)
                {
                    return false;
                }

                return !(_selectedDetail.Components.Any(c => !string.IsNullOrEmpty(c["Value"])));
            }
        }

        private void OnDoStartListening()
        {
            _commsService.StartListening(this.SelectedDetail);
        }
    }
}
