using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribApps.Core.Events
{
    public interface INewEventAggregator
    {
        void Send<T>(T message);
        void Subscribe<T>(Action<T> action);
        void Subscribe<T>(Action<T> action, bool keepAlive);
        void Subscribe<T>(Action<T> action, ThreadOption threadOption);
        void Subscribe<T>(Action<T> action, ThreadOption threadOption, bool keepAlive);
        void Unsubscribe<T>(Action<T> action);
    }
}
