using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Events
{
    public sealed class ApplicationTaskQueuedEvent : CompositePresentationEvent<int>
    {
    }
}
