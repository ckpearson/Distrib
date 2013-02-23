using Distrib.Nodes.Process;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
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

namespace ProcessNode.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class ConnectionStatusViewModel : ViewModelBase
    {
        private readonly INewEventAggregator _eventAgg;
        private readonly ICommsService _commsService;

        [ImportingConstructor]
        public ConnectionStatusViewModel(INewEventAggregator eventAgg, ICommsService commsService)
        {
            _eventAgg = eventAgg;
            _commsService = commsService;

            _eventAgg.Subscribe<Events.NodeListeningChangedEvent>(OnListeningChanged);
        }

        private void OnListeningChanged(NodeListeningChangedEvent obj)
        {
            PropChange("IsListening");
        }

        public bool IsListening
        {
            get { return _commsService.IsListening; }
        }
    }
}
