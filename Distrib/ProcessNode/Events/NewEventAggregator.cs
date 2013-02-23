using Microsoft.Practices.Prism.Events;
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

namespace ProcessNode.Events
{
    /*
     * This is a wrapper for the event aggregator currently supplied to make it easier to push
     * events around using POCOs
     * 
     * Inspired by: http://www.thejoyofcode.com/A_Suck_Less_Event_Aggregator_for_Prism.aspx
     */
    [Export(typeof(INewEventAggregator))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public sealed class NewEventAggregator : INewEventAggregator
    {
        private readonly IEventAggregator _eventAgg;

        [ImportingConstructor()]
        public NewEventAggregator(IEventAggregator eventAgg)
        {
            _eventAgg = eventAgg;
        }

        private CompositePresentationEvent<T> GetComp<T>()
        {
            return _eventAgg.GetEvent<CompositePresentationEvent<T>>();
        }

        public void Send<T>(T message)
        {
            var comp = GetComp<T>();
            comp.Publish(message);
        }

        public void Subscribe<T>(Action<T> action)
        {
            var comp = GetComp<T>();
            comp.Subscribe(action);
        }

        public void Subscribe<T>(Action<T> action, bool keepAlive)
        {
            var comp = GetComp<T>();
            comp.Subscribe(action, keepAlive);
        }

        public void Subscribe<T>(Action<T> action, Microsoft.Practices.Prism.Events.ThreadOption threadOption)
        {
            var comp = GetComp<T>();
            comp.Subscribe(action, threadOption);
        }

        public void Subscribe<T>(Action<T> action, Microsoft.Practices.Prism.Events.ThreadOption threadOption, bool keepAlive)
        {
            var comp = GetComp<T>();
            comp.Subscribe(action, threadOption, keepAlive);
        }

        public void Unsubscribe<T>(Action<T> action)
        {
            var comp = GetComp<T>();
            comp.Unsubscribe(action);
        }
    }
}
