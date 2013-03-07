using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribApps.Core.Events
{
    /*
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
